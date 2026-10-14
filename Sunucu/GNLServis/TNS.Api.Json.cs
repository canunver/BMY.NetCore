using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace TNS.Api
{
    /// <summary>
    /// API sunucusu ile istemci (ServisIstemci) arasında ve oturum verisinin saklanmasında kullanılan ortak JSON ayarları
    /// (System.Text.Json). İki taraf da aynı ayarları kullanmak zorundadır.
    ///
    /// Eski Newtonsoft ayarlarındaki (TypeNameHandling.Auto) davranış korunur: tipli alanlar düz JSON'dur; yalnızca object olarak
    /// bildirilmiş yerlerde (object alanlar, tipsiz listeler, Hashtable değerleri) taşınan nesnelere "$type" eklenir, liste ise
    /// { "$type": ..., "$values": [...] } olarak yazılır. Fark: tipli bir alanda alt sınıf nesnesi taşınırsa (alan Ust, değer Alt)
    /// yalnızca Ust'ün üyeleri yazılır.
    /// Ad eşleşmesi büyük/küçük harfe duyarlıdır: eski sınıflarda yalnızca harf büyüklüğü farklı üyeler var (Kullanici.ad / AD).
    /// </summary>
    public static class ApiJson
    {
        /// <summary>
        /// "$type" bilgisiyle gelen nesnelerde izin verilen ad alanı önekleri.
        /// Tipsiz (object) alanlarda taşınan sınıflar bu ad alanlarından birinde olmalıdır.
        /// </summary>
        public static readonly List<string> IzinliAdAlanlari = new List<string> { "TNS" };

        public static readonly JsonSerializerOptions Ayarlar = AyarOlustur();

        private static JsonSerializerOptions AyarOlustur()
        {
            JsonSerializerOptions a = new JsonSerializerOptions();
            a.IncludeFields = true;
            a.PropertyNameCaseInsensitive = false;
            a.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals;
            a.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            a.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            a.Converters.Add(new TNSDateTimeJsonCevirici());
            a.Converters.Add(new HashTableArrayListCevirici());
            a.Converters.Add(new BoyutlarCevirici());
            a.Converters.Add(new TipsizListeCevirici<TNSCollection>());
            a.Converters.Add(new TipsizListeCevirici<ArrayList>());
            a.Converters.Add(new HashtableCevirici());
            a.Converters.Add(new TipsizNesneCevirici());
            a.MakeReadOnly(true);
            return a;
        }

        public static string Yaz(object deger, Type bildirilenTip)
        {
            return JsonSerializer.Serialize(deger, bildirilenTip ?? typeof(object), Ayarlar);
        }

        public static object Oku(string json, Type tip)
        {
            if (string.IsNullOrWhiteSpace(json)) return VarsayilanDeger(tip);
            return TokenOku(JsonNode.Parse(json), tip);
        }

        /// <summary>JSON nesnesini ayrıştırır; boşsa ya da nesne değilse boş nesne döner.</summary>
        public static JsonObject NesneAyristir(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new JsonObject();
            return JsonNode.Parse(json) as JsonObject ?? new JsonObject();
        }

        public static JsonNode TokenYaz(object deger, Type bildirilenTip)
        {
            if (deger == null) return null;
            return JsonSerializer.SerializeToNode(deger, bildirilenTip ?? typeof(object), Ayarlar);
        }

        public static object TokenOku(JsonNode token, Type tip)
        {
            if (token == null)
            {
                // Null gelen TNSDateTime gibi tiplerde boş nesne üretilir.
                if (tip == typeof(TNSDateTime)) return new TNSDateTime();
                return VarsayilanDeger(tip);
            }
            return token.Deserialize(tip ?? typeof(object), Ayarlar);
        }

        public static object VarsayilanDeger(Type tip)
        {
            if (tip == null || tip == typeof(void)) return null;
            return tip.IsValueType ? Activator.CreateInstance(tip) : null;
        }

        /// <summary>Düğümün dizgi değeri (dizgi değilse JSON metni); yoksa null.</summary>
        public static string Metin(JsonNode dugum)
        {
            if (dugum == null) return null;
            if (dugum is JsonValue v && v.TryGetValue(out string s)) return s;
            return dugum.ToJsonString(Ayarlar);
        }

        /// <summary>
        /// Tipsiz (object) okunan değeri eski koda uygun hale getirir: tam sayılar sığıyorsa Int32, sığmıyorsa Int64 olur;
        /// "$type" taşıyan nesneler gerçek tipine, diziler ArrayList'e, "$type"sız nesneler Hashtable'a çevrilir.
        /// </summary>
        internal static object TipsizOku(JsonNode dugum)
        {
            if (dugum == null) return null;
            switch (dugum.GetValueKind())
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.String:
                    return dugum.GetValue<string>();
                case JsonValueKind.Number:
                    JsonValue v = (JsonValue)dugum;
                    if (v.TryGetValue(out int i)) return i;
                    if (v.TryGetValue(out long l)) return l;
                    if (v.TryGetValue(out JsonElement e))
                    {
                        if (e.TryGetInt32(out i)) return i;
                        if (e.TryGetInt64(out l)) return l;
                        return e.GetDouble();
                    }
                    return v.GetValue<double>();
                case JsonValueKind.Array:
                    ArrayList liste = new ArrayList();
                    foreach (JsonNode n in (JsonArray)dugum) liste.Add(TipsizOku(n));
                    return liste;
                default:
                    JsonObject o = (JsonObject)dugum;
                    if (o.TryGetPropertyValue("$type", out JsonNode tipDugum))
                    {
                        Type tip = TipBul(Metin(tipDugum));
                        if (o.TryGetPropertyValue("$values", out JsonNode degerler))
                            return degerler == null ? null : degerler.Deserialize(tip, Ayarlar);
                        JsonObject kopya = new JsonObject();
                        foreach (KeyValuePair<string, JsonNode> p in o)
                            if (p.Key != "$type") kopya[p.Key] = p.Value == null ? null : p.Value.DeepClone();
                        return kopya.Deserialize(tip, Ayarlar);
                    }
                    Hashtable h = new Hashtable();
                    foreach (KeyValuePair<string, JsonNode> p in o) h[p.Key] = TipsizOku(p.Value);
                    return h;
            }
        }

        /// <summary>object olarak bildirilmiş bir değeri yazar; nesne ve listelere "$type" eklenir.</summary>
        internal static void TipsizYaz(Utf8JsonWriter yazici, object deger)
        {
            if (deger == null) { yazici.WriteNullValue(); return; }
            Type tip = deger.GetType();
            if (tip == typeof(object)) { yazici.WriteStartObject(); yazici.WriteEndObject(); return; }
            // Düz değerler ve kendi çeviricisi olan tipler "$type" olmadan yazılır (eski davranış).
            if (DuzDeger(tip) || tip == typeof(TNSDateTime) || tip == typeof(HashTableArrayList) || tip == typeof(Boyutlar)
                || tip == typeof(TNSCollection) || tip == typeof(ArrayList))
            {
                JsonSerializer.Serialize(yazici, deger, tip, Ayarlar);
                return;
            }
            JsonNode dugum = JsonSerializer.SerializeToNode(deger, tip, Ayarlar);
            JsonObject sonuc = new JsonObject();
            sonuc["$type"] = TipAdi(tip);
            if (dugum is JsonObject o)
            {
                List<KeyValuePair<string, JsonNode>> uyeler = new List<KeyValuePair<string, JsonNode>>(o);
                o.Clear();
                foreach (KeyValuePair<string, JsonNode> p in uyeler) sonuc[p.Key] = p.Value;
            }
            else
                sonuc["$values"] = dugum;
            sonuc.WriteTo(yazici, Ayarlar);
        }

        private static bool DuzDeger(Type t)
        {
            return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime)
                || t == typeof(DateTimeOffset) || t == typeof(Guid) || t == typeof(TimeSpan);
        }

        private static string TipAdi(Type t)
        {
            if (t.IsGenericType || t.IsArray) return t.AssemblyQualifiedName;
            return t.FullName + ", " + t.Assembly.GetName().Name;
        }

        private static Type TipBul(string ad)
        {
            Type t = string.IsNullOrWhiteSpace(ad) ? null : Type.GetType(ad, false);
            if (t == null && !string.IsNullOrWhiteSpace(ad))
            {
                string tamAd = ad.Split(',')[0].Trim();
                foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = a.GetType(tamAd, false);
                    if (t != null) break;
                }
            }
            if (!Izinli(t)) throw new JsonException("JSON içinde izin verilmeyen tip: " + ad);
            return t;
        }

        private static bool Izinli(Type t)
        {
            if (t == null) return false;
            if (t.IsArray) return Izinli(t.GetElementType());
            if (t.IsGenericType)
            {
                foreach (Type arg in t.GetGenericArguments())
                    if (!Izinli(arg)) return false;
                t = t.GetGenericTypeDefinition();
            }
            if (DuzDeger(t) && !t.IsEnum) return true;
            if (t == typeof(object) || t == typeof(ArrayList) || t == typeof(Hashtable)
                || t == typeof(List<>) || t == typeof(Dictionary<,>) || t == typeof(Nullable<>))
                return true;

            string ns = t.Namespace ?? "";
            foreach (string onek in IzinliAdAlanlari)
                if (ns == onek || ns.StartsWith(onek + ".", StringComparison.Ordinal)) return true;
            return false;
        }
    }

    /// <summary>object olarak bildirilmiş yerler: okurken ApiJson.TipsizOku, yazarken ApiJson.TipsizYaz.</summary>
    internal class TipsizNesneCevirici : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ApiJson.TipsizOku(JsonNode.Parse(ref reader));
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            ApiJson.TipsizYaz(writer, value);
        }
    }

    /// <summary>Tipsiz TNSCollection ve ArrayList: elemanların dizisi; nesne elemanlar "$type" taşır.</summary>
    internal class TipsizListeCevirici<T> : JsonConverter<T> where T : ArrayList, new()
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(T);
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonArray dizi = JsonNode.Parse(ref reader) as JsonArray;
            if (dizi == null) return null;
            T liste = new T();
            foreach (JsonNode n in dizi) liste.Add(ApiJson.TipsizOku(n));
            return liste;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (object o in value) ApiJson.TipsizYaz(writer, o);
            writer.WriteEndArray();
        }
    }

    /// <summary>Boyutlar: elemanların dizisi.</summary>
    internal class BoyutlarCevirici : JsonConverter<Boyutlar>
    {
        public override Boyutlar Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonArray dizi = JsonNode.Parse(ref reader) as JsonArray;
            if (dizi == null) return null;
            Boyutlar b = new Boyutlar();
            foreach (JsonNode n in dizi) b.Add(ApiJson.TipsizOku(n));
            return b;
        }

        public override void Write(Utf8JsonWriter writer, Boyutlar value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (object o in value) ApiJson.TipsizYaz(writer, o);
            writer.WriteEndArray();
        }
    }

    /// <summary>Hashtable: anahtarlar metin, değerler tipsiz.</summary>
    internal class HashtableCevirici : JsonConverter<Hashtable>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Hashtable);
        }

        public override Hashtable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonObject o = JsonNode.Parse(ref reader) as JsonObject;
            if (o == null) return null;
            Hashtable h = new Hashtable();
            foreach (KeyValuePair<string, JsonNode> p in o) h[p.Key] = ApiJson.TipsizOku(p.Value);
            return h;
        }

        public override void Write(Utf8JsonWriter writer, Hashtable value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (DictionaryEntry de in value)
            {
                writer.WritePropertyName(Convert.ToString(de.Key, CultureInfo.InvariantCulture) ?? "");
                ApiJson.TipsizYaz(writer, de.Value);
            }
            writer.WriteEndObject();
        }
    }

    /// <summary>HashTableArrayList: { "anahtar": { "0": "deger", "1": "deger" } }</summary>
    internal class HashTableArrayListCevirici : JsonConverter<HashTableArrayList>
    {
        public override HashTableArrayList Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonObject o = JsonNode.Parse(ref reader) as JsonObject;
            if (o == null) return null;
            HashTableArrayList h = new HashTableArrayList();
            foreach (KeyValuePair<string, JsonNode> p in o)
            {
                JsonObject ic = p.Value as JsonObject;
                if (ic == null) continue;
                foreach (KeyValuePair<string, JsonNode> e in ic)
                {
                    int sira;
                    if (int.TryParse(e.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out sira))
                        h[p.Key, sira] = e.Value == null ? null : ApiJson.Metin(e.Value);
                }
            }
            return h;
        }

        public override void Write(Utf8JsonWriter writer, HashTableArrayList value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (DictionaryEntry de in value)
            {
                writer.WritePropertyName(Convert.ToString(de.Key, CultureInfo.InvariantCulture) ?? "");
                writer.WriteStartObject();
                Hashtable ic = de.Value as Hashtable;
                if (ic != null)
                {
                    foreach (DictionaryEntry e in ic)
                    {
                        writer.WritePropertyName(Convert.ToString(e.Key, CultureInfo.InvariantCulture) ?? "");
                        if (e.Value == null) writer.WriteNullValue();
                        else writer.WriteStringValue(e.Value.ToString());
                    }
                }
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }
    }
}

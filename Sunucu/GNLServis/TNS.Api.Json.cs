using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace TNS.Api
{
    /// <summary>
    /// API sunucusu ile istemci (ServisIstemci) arasında kullanılan ortak JSON ayarları.
    /// İki taraf da aynı ayarları kullanmak zorundadır.
    /// </summary>
    public static class ApiJson
    {
        /// <summary>
        /// "$type" bilgisiyle gelen nesnelerde izin verilen ad alanı önekleri.
        /// Tipsiz (object) alanlarda taşınan sınıflar bu ad alanlarından birinde olmalıdır.
        /// </summary>
        public static readonly List<string> IzinliAdAlanlari = new List<string> { "TNS" };

        public static readonly JsonSerializerSettings Ayarlar = AyarOlustur();

        public static JsonSerializer Serilestirici
        {
            get { return JsonSerializer.Create(Ayarlar); }
        }

        private static JsonSerializerSettings AyarOlustur()
        {
            JsonSerializerSettings a = new JsonSerializerSettings();
            // Tipli alanlar düz JSON'dur; yalnızca object / tipsiz liste içinde taşınan nesnelere "$type" eklenir.
            a.TypeNameHandling = TypeNameHandling.Auto;
            a.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            a.SerializationBinder = new GuvenliTipBaglayici();
            // Tarihe benzeyen dizgiler kendiliğinden DateTime'a çevrilmesin.
            a.DateParseHandling = DateParseHandling.None;
            a.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            a.NullValueHandling = NullValueHandling.Include;
            a.MissingMemberHandling = MissingMemberHandling.Ignore;
            a.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            a.Culture = CultureInfo.InvariantCulture;
            a.Converters.Add(new TNSDateTimeCevirici());
            a.Converters.Add(new HashTableArrayListCevirici());
            a.Converters.Add(new BoyutlarCevirici());
            a.Converters.Add(new TipsizListeCevirici());
            return a;
        }

        public static string Yaz(object deger, Type bildirilenTip)
        {
            return JsonConvert.SerializeObject(deger, bildirilenTip ?? typeof(object), Ayarlar);
        }

        public static object Oku(string json, Type tip)
        {
            if (string.IsNullOrWhiteSpace(json)) return VarsayilanDeger(tip);
            return JsonConvert.DeserializeObject(json, tip, Ayarlar);
        }

        /// <summary>JSON nesnesini, tarihe benzeyen dizgileri DateTime'a çevirmeden ayrıştırır.</summary>
        public static JObject NesneAyristir(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new JObject();
            using (JsonTextReader r = new JsonTextReader(new System.IO.StringReader(json)))
            {
                r.DateParseHandling = DateParseHandling.None;
                return JObject.Load(r);
            }
        }

        public static JToken TokenYaz(object deger, Type bildirilenTip)
        {
            if (deger == null) return JValue.CreateNull();
            using (JTokenWriter w = new JTokenWriter())
            {
                Serilestirici.Serialize(w, deger, bildirilenTip ?? typeof(object));
                return w.Token;
            }
        }

        public static object TokenOku(JToken token, Type tip)
        {
            if (token == null || token.Type == JTokenType.Null || token.Type == JTokenType.Undefined)
            {
                // Null gelen TNSDateTime gibi tiplerde çevirici boş nesne üretsin diye yine de okunur.
                if (tip == typeof(TNSDateTime)) return new TNSDateTime();
                return VarsayilanDeger(tip);
            }
            using (JsonReader r = token.CreateReader())
            {
                r.DateParseHandling = DateParseHandling.None;
                return Serilestirici.Deserialize(r, tip);
            }
        }

        public static object VarsayilanDeger(Type tip)
        {
            if (tip == null || tip == typeof(void)) return null;
            return tip.IsValueType ? Activator.CreateInstance(tip) : null;
        }

        /// <summary>
        /// Tipsiz (object) okunan değeri eski koda uygun hale getirir: JSON tam sayıları Int64 yerine,
        /// sığıyorsa Int32 olur; "$type" taşıyan nesneler gerçek tipine çevrilir.
        /// </summary>
        internal static object TipsizOku(JToken token, JsonSerializer serilestirici)
        {
            if (token == null) return null;
            switch (token.Type)
            {
                case JTokenType.Null:
                case JTokenType.Undefined:
                    return null;
                case JTokenType.Integer:
                    long l = token.Value<long>();
                    if (l >= int.MinValue && l <= int.MaxValue) return (int)l;
                    return l;
                case JTokenType.Float:
                    return token.Value<double>();
                case JTokenType.Boolean:
                    return token.Value<bool>();
                case JTokenType.String:
                    return token.Value<string>();
                case JTokenType.Date:
                    return token.Value<DateTime>();
                default:
                    using (JsonReader r = token.CreateReader())
                    {
                        r.DateParseHandling = DateParseHandling.None;
                        return serilestirici.Deserialize(r, typeof(object));
                    }
            }
        }
    }

    /// <summary>"$type" ile yalnızca izin verilen ad alanlarındaki tiplerin oluşturulmasına izin verir.</summary>
    internal class GuvenliTipBaglayici : DefaultSerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type t = base.BindToType(assemblyName, typeName);
            if (!Izinli(t))
                throw new JsonSerializationException("JSON içinde izin verilmeyen tip: " + typeName);
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
            if (t.IsPrimitive || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(Guid)
                || t == typeof(TimeSpan) || t == typeof(object) || t == typeof(ArrayList) || t == typeof(Hashtable)
                || t == typeof(List<>) || t == typeof(Dictionary<,>) || t == typeof(Nullable<>))
                return true;

            string ns = t.Namespace ?? "";
            foreach (string onek in ApiJson.IzinliAdAlanlari)
                if (ns == onek || ns.StartsWith(onek + ".", StringComparison.Ordinal)) return true;
            return false;
        }
    }

    /// <summary>TNSDateTime: boşsa null, doluysa ISO 8601 tarih dizgisi.</summary>
    internal class TNSDateTimeCevirici : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TNSDateTime);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TNSDateTime d = value as TNSDateTime;
            if (d == null || d.isNull) writer.WriteNull();
            else writer.WriteValue(d.Oku());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return new TNSDateTime();
            if (reader.TokenType == JsonToken.Date)
            {
                if (reader.Value is DateTimeOffset) return new TNSDateTime(((DateTimeOffset)reader.Value).DateTime);
                return new TNSDateTime((DateTime)reader.Value);
            }
            if (reader.TokenType == JsonToken.String)
            {
                string s = (string)reader.Value;
                if (string.IsNullOrWhiteSpace(s)) return new TNSDateTime();
                DateTime dt;
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt)) return new TNSDateTime(dt);
                return new TNSDateTime(s);
            }
            if (reader.TokenType == JsonToken.StartObject)
            {
                // { "isNull": true } gibi eski biçimli bir nesne gelirse yutulur.
                JObject.Load(reader);
                return new TNSDateTime();
            }
            throw new JsonSerializationException("TNSDateTime için beklenmeyen JSON değeri: " + reader.TokenType);
        }
    }

    /// <summary>HashTableArrayList: { "anahtar": { "0": "deger", "1": "deger" } }</summary>
    internal class HashTableArrayListCevirici : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashTableArrayList);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartObject();
            foreach (DictionaryEntry de in (HashTableArrayList)value)
            {
                writer.WritePropertyName(Convert.ToString(de.Key, CultureInfo.InvariantCulture));
                writer.WriteStartObject();
                Hashtable ic = de.Value as Hashtable;
                if (ic != null)
                {
                    foreach (DictionaryEntry e in ic)
                    {
                        writer.WritePropertyName(Convert.ToString(e.Key, CultureInfo.InvariantCulture));
                        writer.WriteValue(e.Value == null ? null : e.Value.ToString());
                    }
                }
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            HashTableArrayList h = new HashTableArrayList();
            JObject o = JObject.Load(reader);
            foreach (JProperty p in o.Properties())
            {
                JObject ic = p.Value as JObject;
                if (ic == null) continue;
                foreach (JProperty e in ic.Properties())
                {
                    int sira;
                    if (int.TryParse(e.Name, NumberStyles.Integer, CultureInfo.InvariantCulture, out sira))
                        h[p.Name, sira] = e.Value.Type == JTokenType.Null ? null : e.Value.Value<string>();
                }
            }
            return h;
        }
    }

    /// <summary>Boyutlar: elemanların dizisi.</summary>
    internal class BoyutlarCevirici : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Boyutlar);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartArray();
            foreach (object o in (Boyutlar)value) serializer.Serialize(writer, o, typeof(object));
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            Boyutlar b = new Boyutlar();
            foreach (JToken t in JArray.Load(reader)) b.Add(ApiJson.TipsizOku(t, serializer));
            return b;
        }
    }

    /// <summary>Tipsiz TNSCollection ve ArrayList: elemanların dizisi; nesne elemanlar "$type" taşır.</summary>
    internal class TipsizListeCevirici : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TNSCollection) || objectType == typeof(ArrayList);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartArray();
            foreach (object o in (IEnumerable)value) serializer.Serialize(writer, o, typeof(object));
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            ArrayList liste = objectType == typeof(TNSCollection) ? new TNSCollection() : new ArrayList();
            foreach (JToken t in JArray.Load(reader)) liste.Add(ApiJson.TipsizOku(t, serializer));
            return liste;
        }
    }
}

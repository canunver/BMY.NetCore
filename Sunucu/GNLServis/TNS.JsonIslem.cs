using System;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace TNS
{
    /// <summary>
    /// İş kurallarının dış servislerle ve veri tabanında sakladığı JSON için ortak ayarlar (System.Text.Json; eski Newtonsoft.Json yerine).
    /// Newtonsoft'un varsayılanlarına yakın davranır: ad eşleşmesi büyük/küçük harfe duyarsız, alanlar (field) da yazılır/okunur,
    /// tırnaklı sayılar okunur, Türkçe karakterler \u ile kaçırılmaz. TNSDateTime boşsa null, doluysa ISO tarih olarak yazılır.
    /// </summary>
    public static class JsonIslem
    {
        public static readonly JsonSerializerOptions Ayarlar = AyarOlustur();

        private static JsonSerializerOptions AyarOlustur()
        {
            JsonSerializerOptions a = new JsonSerializerOptions();
            a.PropertyNameCaseInsensitive = true;
            a.IncludeFields = true;
            a.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            a.ReadCommentHandling = JsonCommentHandling.Skip;
            a.AllowTrailingCommas = true;
            a.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            a.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            a.Converters.Add(new TNSDateTimeJsonCevirici());
            return a;
        }

        public static string Yaz(object deger)
        {
            return JsonSerializer.Serialize(deger, deger == null ? typeof(object) : deger.GetType(), Ayarlar);
        }

        public static T Oku<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default(T);
            return JsonSerializer.Deserialize<T>(json, Ayarlar);
        }

        /// <summary>JSON nesnesini ayrıştırır (eski JObject.Parse). Nesne değilse ya da boşsa null döner.</summary>
        public static JsonObject NesneAyristir(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonNode.Parse(json, new JsonNodeOptions { PropertyNameCaseInsensitive = false },
                new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true }) as JsonObject;
        }

        /// <summary>Düğümün değerini metin olarak verir (dizgi tırnaksız); yoksa "".</summary>
        public static string Metin(JsonNode dugum)
        {
            if (dugum == null) return "";
            if (dugum is JsonValue v && v.TryGetValue(out string s)) return s ?? "";
            return dugum.ToJsonString(Ayarlar);
        }
    }

    /// <summary>TNSDateTime: boşsa null, doluysa ISO 8601 tarih. Okurken eski biçimler de kabul edilir: tarih dizgisi
    /// ya da Newtonsoft'un yazdığı { "Yil":..., "Ay":..., "Gun":... } nesnesi.</summary>
    public class TNSDateTimeJsonCevirici : JsonConverter<TNSDateTime>
    {
        public override bool HandleNull
        {
            get { return true; }
        }

        public override TNSDateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return new TNSDateTime();
                case JsonTokenType.String:
                    string s = reader.GetString();
                    if (string.IsNullOrWhiteSpace(s)) return new TNSDateTime();
                    DateTime dt;
                    if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt)) return new TNSDateTime(dt);
                    return new TNSDateTime(s);
                case JsonTokenType.StartObject:
                    JsonObject o = JsonNode.Parse(ref reader) as JsonObject;
                    if (o == null) return new TNSDateTime();
                    bool bos = false;
                    JsonNode n = Bul(o, "isNull");
                    if (n != null && n is JsonValue bv && bv.TryGetValue(out bool b)) bos = b;
                    int yil = Sayi(Bul(o, "Yil")), ay = Sayi(Bul(o, "Ay")), gun = Sayi(Bul(o, "Gun"));
                    if (bos || yil <= 0 || ay <= 0 || gun <= 0) return new TNSDateTime();
                    return new TNSDateTime(new DateTime(yil, ay, gun, Sayi(Bul(o, "Saat")), Sayi(Bul(o, "Dakika")), Sayi(Bul(o, "Saniye"))));
                default:
                    throw new JsonException("TNSDateTime için beklenmeyen JSON değeri: " + reader.TokenType);
            }
        }

        public override void Write(Utf8JsonWriter writer, TNSDateTime value, JsonSerializerOptions options)
        {
            if (value == null || value.isNull) writer.WriteNullValue();
            else writer.WriteStringValue(value.Oku());
        }

        private static JsonNode Bul(JsonObject o, string ad)
        {
            foreach (var p in o)
                if (string.Equals(p.Key, ad, StringComparison.OrdinalIgnoreCase)) return p.Value;
            return null;
        }

        private static int Sayi(JsonNode n)
        {
            if (n is JsonValue v)
            {
                if (v.TryGetValue(out int i)) return i;
                if (v.TryGetValue(out string s) && int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) return i;
            }
            return 0;
        }
    }
}

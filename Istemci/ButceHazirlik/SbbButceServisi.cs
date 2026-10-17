using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using OrtakClass;

namespace Istemci.ButceHazirlik
{
    /// <summary>
    /// BÜMKO Aktarım ve AHP / AGP Girişi sayfalarının kullandığı SBB program bütçe servisleri (SOAP 1.1, .asmx). Eski sayfa WebServisler projesindeki
    /// ButceHazirlikFormlari ve butceuygulama web başvurularını kullanıyordu; .NET 8'de o vekil sınıflar olmadığından aynı çağrılar
    /// (DepremHarcamaAktarimi'ndaki gibi) elle hazırlanan iletilerle yapılır. İletiler eski vekil sınıfların XmlSerializer ile
    /// ürettiklerinin aynısıdır: zorunlu (minOccurs=1) alanlar her zaman, boş metinler ve belirtilmemiş isteğe bağlı sayılar hiç yazılmaz.
    /// Adresler ayar dosyasındaki "ButceHazirlikFormlariUrl" ve "ButceUygulamaUrl" anahtarlarından okunur; yoksa eski adresler kullanılır.
    /// Sunucu soap:Fault dönerse ya da yanıt okunamazsa istisna atılır.
    /// </summary>
    public static class SbbButceServisi
    {
        private const string VarsayilanFormlarAdres = "http://programbutce.sbb.gov.tr/Services/ButceHazirlikFormlari.asmx";
        private const string VarsayilanUygulamaAdres = "http://programbutce.sbb.gov.tr/services/butceuygulama.asmx";
        private static readonly XNamespace Soap = "http://schemas.xmlsoap.org/soap/envelope/";
        private static readonly XNamespace Tns = "http://tempuri.org/";
        private static readonly HttpClient istemci = new HttpClient() { Timeout = TimeSpan.FromSeconds(100) };

        /// <summary>Servisin Sonuc sınıfının kullanılan alanları.</summary>
        public class WsSonuc
        {
            public bool islemSonuc;
            public string hataStr;
        }

        /// <summary>GiderButcesiKaydet'e giden satır (eski OdenekHarcamaTahminWI; yalnızca eski sayfanın doldurduğu alanlar).</summary>
        public class OdenekSatiri
        {
            public long ProgramId, AltProgramId, FaaliyetId, AltFaaliyetId;
            public string KurKod, FinKod, EkoKod;
            public double teklif, tahmin1, tahmin2, ysht, odenekUstu;
        }

        /// <summary>ButceFisiKaydet'e giden model (eski OdenekHarcamaTahmin; yalnızca eski sayfanın doldurduğu alanlar).</summary>
        public class FisGerekcesi
        {
            public long Yil, Asama, ProgramId, AltProgramId, FaaliyetId, AltFaaliyetId;
            public string KurKod, FinKod, EkoKod, FisAciklama, TavanUstuTalepSebep;
        }

        /// <summary>AHPDetayListele / AHPDetayListeleSonDurum satırı, AHPKaydet / AHPDetayKaydet satırı (eski OdenekAHPDagilimWI).</summary>
        public class AhpSatiri
        {
            public double bloke;
            public long ProgramId, AltProgramId, FaaliyetId, AltFaaliyetId;
            public string KurKod, FinKod, EkoKod;
            public double[] ayDagilim = new double[0];
        }

        private static string FormlarAdres => Ayar.Oku("ButceHazirlikFormlariUrl", VarsayilanFormlarAdres);
        private static string UygulamaAdres => Ayar.Oku("ButceUygulamaUrl", VarsayilanUygulamaAdres);

        // ------------------------------------------------------------------ ButceHazirlikFormlari.asmx

        public static Task<WsSonuc> GiderButcesiKaydet(string kullaniciKodu, string parola, int yil, int asama, string kurum, IEnumerable<OdenekSatiri> odenekler)
        {
            XElement liste = new XElement(Tns + "odenekler");
            foreach (OdenekSatiri o in odenekler)
                liste.Add(new XElement(Tns + "OdenekHarcamaTahminWI",
                    new XElement(Tns + "ProgramId", o.ProgramId),
                    new XElement(Tns + "ProgramTur", 0),
                    new XElement(Tns + "AltProgramId", o.AltProgramId),
                    new XElement(Tns + "FaaliyetId", o.FaaliyetId),
                    new XElement(Tns + "AltFaaliyetId", o.AltFaaliyetId),
                    Metin("KurKod", o.KurKod),
                    Metin("FinKod", o.FinKod),
                    Metin("EkoKod", o.EkoKod),
                    Sayi("teklif", o.teklif),
                    Sayi("tahmin1", o.tahmin1),
                    Sayi("tahmin2", o.tahmin2),
                    Sayi("ysht", o.ysht),
                    Sayi("odenekUstu", o.odenekUstu)));

            XElement govde = new XElement(Tns + "GiderButcesiKaydet",
                Metin("kullaniciKodu", kullaniciKodu),
                Metin("parola", parola),
                new XElement(Tns + "yil", yil),
                new XElement(Tns + "asama", asama),
                Metin("kurum", kurum),
                liste);
            return SonucCagir(FormlarAdres, "GiderButcesiKaydet", govde);
        }

        public static Task<WsSonuc> ButceFisiKaydet(string kullaniciKodu, string parola, FisGerekcesi m)
        {
            const string bosTarih = "0001-01-01T00:00:00";
            XElement model = new XElement(Tns + "model",
                // Tanim (taban sınıf) alanları
                new XElement(Tns + "RowNum", 0),
                new XElement(Tns + "Action", "list"),
                new XElement(Tns + "PrevAction", "list"),
                new XElement(Tns + "BaslaTarihOrjDT", bosTarih),
                new XElement(Tns + "BaslaTarihDT", bosTarih),
                new XElement(Tns + "BitisTarihDT", bosTarih),
                // OdenekHarcamaTahmin alanları
                new XElement(Tns + "ucgenYer", 0),
                new XElement(Tns + "UzmanlarGorsun", "false"),
                new XElement(Tns + "Yil", m.Yil),
                new XElement(Tns + "Asama", m.Asama),
                new XElement(Tns + "ProgramId", m.ProgramId),
                new XElement(Tns + "ProgramTur", 0),
                new XElement(Tns + "AltProgramId", m.AltProgramId),
                new XElement(Tns + "FaaliyetId", m.FaaliyetId),
                new XElement(Tns + "AltFaaliyetId", m.AltFaaliyetId),
                Metin("KurKod", m.KurKod),
                Metin("FinKod", m.FinKod),
                Metin("EkoKod", m.EkoKod),
                Metin("FisAciklama", m.FisAciklama),
                // Eski sayfa gönderirken bu ikisini boş metne çeviriyordu.
                new XElement(Tns + "FisBaslangicFarkSebep", ""),
                new XElement(Tns + "FisOncekiYilFarkSebep", ""),
                Metin("TavanUstuTalepSebep", m.TavanUstuTalepSebep),
                new XElement(Tns + "IlId", 0),
                new XElement(Tns + "IlceId", 0),
                new XElement(Tns + "HarcBirimId", 0),
                new XElement(Tns + "Gelir", "false"),
                new XElement(Tns + "KayitTuru", 0));

            XElement govde = new XElement(Tns + "ButceFisiKaydet",
                Metin("KullaniciKodu", kullaniciKodu),
                Metin("Parola", parola),
                model);
            return SonucCagir(FormlarAdres, "ButceFisiKaydet", govde);
        }

        public static Task<WsSonuc> FaaliyetGerekceKaydet(string kullaniciKodu, string parola, int yil, int asama, string kurum, string faaliyetId,
            string fisAciklama, string dayanak, string maliyet)
        {
            XElement govde = new XElement(Tns + "FaaliyetGerekceKaydet",
                Metin("kullaniciKodu", kullaniciKodu),
                Metin("parola", parola),
                new XElement(Tns + "yil", yil),
                new XElement(Tns + "asama", asama),
                Metin("kurum", kurum),
                Metin("faaliyetId", faaliyetId),
                Metin("fisAciklama", fisAciklama),
                Metin("dayanak", dayanak),
                Metin("maliyet", maliyet));
            return SonucCagir(FormlarAdres, "FaaliyetGerekceKaydet", govde);
        }

        // ------------------------------------------------------------------ butceuygulama.asmx

        /// <summary>AHPDetayListeleSonDurum: kurumun ayrıntılı harcama programı son durumu. Yanıtta liste yoksa null.</summary>
        public static Task<List<AhpSatiri>> AHPDetayListeleSonDurum(string kullaniciKodu, string parola, int yil, string kurKod, string ekoKod)
        {
            return AhpListeCagir("AHPDetayListeleSonDurum", kullaniciKodu, parola, yil, kurKod, ekoKod);
        }

        /// <summary>AHPDetayListele: kurumun alt faaliyet düzeyinde ayrıntılı harcama programı (AHP / AGP Girişi "SBB'den Al"). Yanıtta liste yoksa null.</summary>
        public static Task<List<AhpSatiri>> AHPDetayListele(string kullaniciKodu, string parola, int yil, string kurKod, string ekoKod)
        {
            return AhpListeCagir("AHPDetayListele", kullaniciKodu, parola, yil, kurKod, ekoKod);
        }

        /// <summary>AHPKaydet: faaliyet düzeyinde AHP gönderir (AHP / AGP Girişi "SBB'ye Gönder (Faal)").</summary>
        public static Task<WsSonuc> AHPKaydet(string kullaniciKodu, string parola, int yil, string kurKod, IEnumerable<AhpSatiri> satirlar)
        {
            return SonucCagir(UygulamaAdres, "AHPKaydet", AhpKayitGovde("AHPKaydet", kullaniciKodu, parola, yil, kurKod, satirlar));
        }

        /// <summary>AHPDetayKaydet: alt faaliyet düzeyinde AHP gönderir (AHP / AGP Girişi "SBB'ye Gönder (Alt Faal)").</summary>
        public static Task<WsSonuc> AHPDetayKaydet(string kullaniciKodu, string parola, int yil, string kurKod, IEnumerable<AhpSatiri> satirlar)
        {
            return SonucCagir(UygulamaAdres, "AHPDetayKaydet", AhpKayitGovde("AHPDetayKaydet", kullaniciKodu, parola, yil, kurKod, satirlar));
        }

        /// <summary>
        /// AHPKaydet / AHPDetayKaydet iletisi (eski OdenekAHPDagilimWI dizisi). tertipBilgi'de (OdenekHarcamaTahminWI) eski sayfanın
        /// doldurduğu alanlar yazılır; isteğe bağlı (Specified) sayılar ve boş metinler eski vekil sınıftaki gibi yazılmaz.
        /// </summary>
        private static XElement AhpKayitGovde(string islem, string kullaniciKodu, string parola, int yil, string kurKod, IEnumerable<AhpSatiri> satirlar)
        {
            XElement liste = new XElement(Tns + "tertipBilgi");
            foreach (AhpSatiri a in satirlar)
            {
                XElement aylar = new XElement(Tns + "ayDagilim");
                foreach (double d in a.ayDagilim ?? new double[0])
                    aylar.Add(new XElement(Tns + "double", XmlConvert.ToString(d)));
                liste.Add(new XElement(Tns + "OdenekAHPDagilimWI",
                    Sayi("bloke", a.bloke),
                    new XElement(Tns + "tertipBilgi",
                        new XElement(Tns + "ProgramId", a.ProgramId),
                        new XElement(Tns + "ProgramTur", 0),
                        new XElement(Tns + "AltProgramId", a.AltProgramId),
                        new XElement(Tns + "FaaliyetId", a.FaaliyetId),
                        new XElement(Tns + "AltFaaliyetId", a.AltFaaliyetId),
                        Metin("KurKod", a.KurKod),
                        Metin("FinKod", a.FinKod),
                        Metin("EkoKod", a.EkoKod)),
                    aylar));
            }
            return new XElement(Tns + islem,
                Metin("KullaniciKodu", kullaniciKodu),
                Metin("Parola", parola),
                new XElement(Tns + "yil", yil),
                Metin("kurKod", kurKod),
                liste);
        }

        private static async Task<List<AhpSatiri>> AhpListeCagir(string islem, string kullaniciKodu, string parola, int yil, string kurKod, string ekoKod)
        {
            XElement govde = new XElement(Tns + islem,
                Metin("KullaniciKodu", kullaniciKodu),
                Metin("Parola", parola),
                new XElement(Tns + "yil", yil),
                Metin("kurKod", kurKod),
                Metin("ekoKod", ekoKod));
            XElement sonuc = await Cagir(UygulamaAdres, islem, govde);
            if (sonuc == null) return null;

            List<AhpSatiri> liste = new List<AhpSatiri>();
            foreach (XElement e in sonuc.Elements(Tns + "OdenekAHPDagilimWI"))
            {
                if (IsNil(e)) continue;
                AhpSatiri a = new AhpSatiri();
                a.bloke = Dbl(e.Element(Tns + "bloke"));
                XElement t = e.Element(Tns + "tertipBilgi");
                if (t != null)
                {
                    a.ProgramId = Lng(t.Element(Tns + "ProgramId"));
                    a.AltProgramId = Lng(t.Element(Tns + "AltProgramId"));
                    a.FaaliyetId = Lng(t.Element(Tns + "FaaliyetId"));
                    a.AltFaaliyetId = Lng(t.Element(Tns + "AltFaaliyetId"));
                    a.KurKod = (string)t.Element(Tns + "KurKod");
                    a.FinKod = (string)t.Element(Tns + "FinKod");
                    a.EkoKod = (string)t.Element(Tns + "EkoKod");
                }
                XElement ay = e.Element(Tns + "ayDagilim");
                if (ay != null)
                    a.ayDagilim = ay.Elements(Tns + "double").Select(Dbl).ToArray();
                liste.Add(a);
            }
            return liste;
        }

        // ------------------------------------------------------------------ ortak

        private static XElement Metin(string ad, string deger)
        {
            return deger == null ? null : new XElement(Tns + ad, deger);
        }

        private static XElement Sayi(string ad, double deger)
        {
            return new XElement(Tns + ad, XmlConvert.ToString(deger));
        }

        private static bool IsNil(XElement e)
        {
            XAttribute a = e.Attribute(XName.Get("nil", "http://www.w3.org/2001/XMLSchema-instance"));
            return a != null && a.Value == "true";
        }

        private static double Dbl(XElement e)
        {
            if (e == null || string.IsNullOrWhiteSpace(e.Value)) return 0;
            return XmlConvert.ToDouble(e.Value.Trim());
        }

        private static long Lng(XElement e)
        {
            if (e == null || string.IsNullOrWhiteSpace(e.Value)) return 0;
            return XmlConvert.ToInt64(e.Value.Trim());
        }

        private static async Task<WsSonuc> SonucCagir(string adres, string islem, XElement govde)
        {
            XElement sonuc = await Cagir(adres, islem, govde);
            if (sonuc == null)
                throw new Exception("Servis yanıtı okunamadı.");
            WsSonuc s = new WsSonuc();
            s.islemSonuc = string.Equals(((string)sonuc.Element(Tns + "islemSonuc") ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
            s.hataStr = (string)sonuc.Element(Tns + "hataStr") ?? "";
            return s;
        }

        /// <summary>İletiyi gönderir ve yanıttaki {islem}Result öğesini döndürür (yoksa null).</summary>
        private static async Task<XElement> Cagir(string adres, string islem, XElement govde)
        {
            XDocument ileti = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement(Soap + "Envelope", new XAttribute(XNamespace.Xmlns + "soap", Soap.NamespaceName),
                    new XElement(Soap + "Body", govde)));

            using (HttpRequestMessage istek = new HttpRequestMessage(HttpMethod.Post, adres))
            {
                istek.Content = new StringContent(ileti.Declaration + ileti.ToString(SaveOptions.DisableFormatting), Encoding.UTF8, "text/xml");
                istek.Headers.Add("SOAPAction", "\"http://tempuri.org/" + islem + "\"");
                using (HttpResponseMessage yanit = await istemci.SendAsync(istek))
                {
                    string metin = await yanit.Content.ReadAsStringAsync();
                    XDocument cevap;
                    try
                    {
                        cevap = XDocument.Parse(metin);
                    }
                    catch (XmlException)
                    {
                        throw new Exception("Servis yanıtı okunamadı (HTTP " + (int)yanit.StatusCode + ").");
                    }
                    XElement hata = cevap.Descendants(Soap + "Fault").FirstOrDefault();
                    if (hata != null)
                        throw new Exception((string)hata.Element("faultstring") ?? "Servis hata döndürdü.");
                    return cevap.Descendants(Tns + islem + "Result").FirstOrDefault();
                }
            }
        }
    }
}

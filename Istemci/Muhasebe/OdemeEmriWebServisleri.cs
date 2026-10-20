using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Istemci.Muhasebe
{
    /// <summary>OYBS işlem tipi (eski WebServisler.OybsMuhasebeServis.IslemTipi; sayı değerleri aynı).</summary>
    public enum OybsIslemTipi { FonGirisi = 0, FonCikisi = 1, FonNemalandirmasi = 2 }

    /// <summary>OYBS kayıt tipi (eski WebServisler.OybsMuhasebeServis.KayitTipi).</summary>
    public enum OybsKayitTipi { MuhasebelesmisKayitlar = 0, MuhasebelesecekKayitlar = 1, TumKayitlar = 2 }

    /// <summary>OYBS'nin muhasebeleşecek işlem kaydı (eski WebServisler.OybsMuhasebeServis.MuhasebelesecekIslem, alanlar aynı).</summary>
    public class OybsIslem
    {
        public string Tanim = "";
        public OybsIslemTipi islemTipi;
        public string GirisMiktari = "";
        public string GirisParaBirimi = "";
        public DateTime GirisTarihi;
        public string CikisMiktari = "";
        public string CikisBirimi = "";
        public DateTime CikisTarihi;
        public string NemalandirilanMiktar = "";
        public string VadeSonuToplamMiktar = "";
        public string NemaParaBirimi = "";
        public DateTime VadeBaslangicTarihi;
        public DateTime VadeBitisTarihi;
        public string IlgiliBankaHesabi = "";
        public string IlgiliProje = "";
        public string IlgiliBirim = "";
        public string OdemeYapan = "";
        public string OdemeYapilan = "";
        public long OybsId;
        public string MuhasebeIslemAciklamasi = "";
        public bool MuhasebelestiMi;
        public bool Aktif;
    }

    /// <summary>
    /// Ödeme Emri / MİF ekranının kullandığı iki eski SOAP web servisi, .NET'in kendi HttpClient ve System.Xml.Linq'iyle (eski "Web Reference"
    /// istemcilerinin yerine, aynı istek biçimiyle):
    ///   OYBS (eski WebServisler.OybsMuhasebeServis.OybsMuhasebe): adres OFMServisUrl, AuthenticationHeader OFMServisUser / OFMServisPass.
    ///   Evrak (eski WebServisler.Evrak.EvrakIslemServis): adres yazilimURL + "Servis/EvrakIslemServis.asmx", GidenEvrakKaydet.
    /// </summary>
    public static class OdemeEmriWebServisleri
    {
        private static readonly XNamespace Soap = "http://schemas.xmlsoap.org/soap/envelope/";
        private static readonly XNamespace Tns = "http://tempuri.org/";
        private static readonly XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";

        private static HttpClient istemci;
        private static readonly object kilit = new object();

        private static HttpClient Istemci()
        {
            lock (kilit)
            {
                if (istemci == null)
                {
                    HttpClientHandler h = new HttpClientHandler();
                    System.Net.WebProxy proxy = TNS.OrtakFonksiyonlar.ProxyAyarla();
                    if (proxy != null && proxy.Address != null)
                    {
                        h.Proxy = proxy;
                        h.UseProxy = true;
                    }
                    istemci = new HttpClient(h) { Timeout = TimeSpan.FromSeconds(300) };
                }
                return istemci;
            }
        }

        /// <summary>SOAP isteği gönderir, yanıt gövdesindeki "&lt;islem&gt;Result" öğesini döndürür (yoksa null). Hata (Fault) olursa istisna atar.</summary>
        private static XElement Cagir(string adres, string islem, XElement baslik, params object[] govdeAlanlari)
        {
            if (string.IsNullOrWhiteSpace(adres))
                throw new Exception("Servis adresi tanımlı değil.");

            XElement zarf = new XElement(Soap + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soap", Soap.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "xsi", Xsi.NamespaceName));
            if (baslik != null)
                zarf.Add(new XElement(Soap + "Header", baslik));
            zarf.Add(new XElement(Soap + "Body", new XElement(Tns + islem, govdeAlanlari)));

            XDocument ileti = new XDocument(new XDeclaration("1.0", "utf-8", null), zarf);

            using (HttpRequestMessage istek = new HttpRequestMessage(HttpMethod.Post, adres))
            {
                istek.Content = new StringContent(ileti.Declaration + ileti.ToString(SaveOptions.DisableFormatting), Encoding.UTF8, "text/xml");
                istek.Headers.Add("SOAPAction", "\"http://tempuri.org/" + islem + "\"");
                using (HttpResponseMessage yanit = Istemci().Send(istek))
                {
                    string metin = yanit.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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
                    return cevap.Descendants(Tns + (islem + "Result")).FirstOrDefault();
                }
            }
        }

        // ------------------------------------------------------------------ OYBS

        private static XElement OybsBaslik()
        {
            return new XElement(Tns + "AuthenticationHeader",
                new XElement(Tns + "UserName", Ayar.Oku("OFMServisUser")),
                new XElement(Tns + "Password", Ayar.Oku("OFMServisPass")));
        }

        private static string OybsAdres()
        {
            return Ayar.Oku("OFMServisUrl");
        }

        private static string Metin(XElement e, string ad)
        {
            XElement a = e.Element(Tns + ad);
            return a == null ? "" : a.Value;
        }

        private static DateTime Tarih(XElement e, string ad)
        {
            string s = Metin(e, ad);
            if (s == "") return DateTime.MinValue;
            try { return XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.Unspecified); }
            catch { return DateTime.MinValue; }
        }

        private static OybsIslem IslemOku(XElement e)
        {
            OybsIslem i = new OybsIslem();
            i.Tanim = Metin(e, "Tanim");
            OybsIslemTipi tip;
            i.islemTipi = Enum.TryParse(Metin(e, "islemTipi"), out tip) ? tip : OybsIslemTipi.FonGirisi;
            i.GirisMiktari = Metin(e, "GirisMiktari");
            i.GirisParaBirimi = Metin(e, "GirisParaBirimi");
            i.GirisTarihi = Tarih(e, "GirisTarihi");
            i.CikisMiktari = Metin(e, "CikisMiktari");
            i.CikisBirimi = Metin(e, "CikisBirimi");
            i.CikisTarihi = Tarih(e, "CikisTarihi");
            i.NemalandirilanMiktar = Metin(e, "NemalandirilanMiktar");
            i.VadeSonuToplamMiktar = Metin(e, "VadeSonuToplamMiktar");
            i.NemaParaBirimi = Metin(e, "NemaParaBirimi");
            i.VadeBaslangicTarihi = Tarih(e, "VadeBaslangicTarihi");
            i.VadeBitisTarihi = Tarih(e, "VadeBitisTarihi");
            i.IlgiliBankaHesabi = Metin(e, "IlgiliBankaHesabi");
            i.IlgiliProje = Metin(e, "IlgiliProje");
            i.IlgiliBirim = Metin(e, "IlgiliBirim");
            i.OdemeYapan = Metin(e, "OdemeYapan");
            i.OdemeYapilan = Metin(e, "OdemeYapilan");
            long id;
            i.OybsId = long.TryParse(Metin(e, "OybsId"), out id) ? id : 0;
            i.MuhasebeIslemAciklamasi = Metin(e, "MuhasebeIslemAciklamasi");
            i.MuhasebelestiMi = string.Equals(Metin(e, "MuhasebelestiMi"), "true", StringComparison.OrdinalIgnoreCase);
            i.Aktif = string.Equals(Metin(e, "Aktif"), "true", StringComparison.OrdinalIgnoreCase);
            return i;
        }

        private static string TarihYaz(DateTime t)
        {
            return XmlConvert.ToString(t, XmlDateTimeSerializationMode.Unspecified);
        }

        /// <summary>Eski OybsMuhasebe.KayitlariGetir.</summary>
        public static List<OybsIslem> KayitlariGetir(OybsIslemTipi islemTipi, OybsKayitTipi kayitTipi)
        {
            XElement sonuc = Cagir(OybsAdres(), "KayitlariGetir", OybsBaslik(),
                new XElement(Tns + "islemTipi", islemTipi.ToString()),
                new XElement(Tns + "kayitTipi", kayitTipi.ToString()));
            List<OybsIslem> liste = new List<OybsIslem>();
            if (sonuc != null)
                foreach (XElement e in sonuc.Elements(Tns + "MuhasebelesecekIslem"))
                    liste.Add(IslemOku(e));
            return liste;
        }

        /// <summary>Eski OybsMuhasebe.Getir.</summary>
        public static OybsIslem Getir(OybsIslemTipi islemTipi, long oybsId)
        {
            XElement sonuc = Cagir(OybsAdres(), "Getir", OybsBaslik(),
                new XElement(Tns + "islemTipi", islemTipi.ToString()),
                new XElement(Tns + "oybsId", oybsId));
            return sonuc == null ? null : IslemOku(sonuc);
        }

        /// <summary>Eski OybsMuhasebe.Kaydet (islem nesnesinin bütün alanları eski XmlSerializer sırasıyla gönderilir).</summary>
        public static long Kaydet(OybsIslem i)
        {
            XElement islem = new XElement(Tns + "islem",
                new XElement(Tns + "Tanim", i.Tanim ?? ""),
                new XElement(Tns + "islemTipi", i.islemTipi.ToString()),
                new XElement(Tns + "GirisMiktari", i.GirisMiktari ?? ""),
                new XElement(Tns + "GirisParaBirimi", i.GirisParaBirimi ?? ""),
                new XElement(Tns + "GirisTarihi", TarihYaz(i.GirisTarihi)),
                new XElement(Tns + "CikisMiktari", i.CikisMiktari ?? ""),
                new XElement(Tns + "CikisBirimi", i.CikisBirimi ?? ""),
                new XElement(Tns + "CikisTarihi", TarihYaz(i.CikisTarihi)),
                new XElement(Tns + "NemalandirilanMiktar", i.NemalandirilanMiktar ?? ""),
                new XElement(Tns + "VadeSonuToplamMiktar", i.VadeSonuToplamMiktar ?? ""),
                new XElement(Tns + "NemaParaBirimi", i.NemaParaBirimi ?? ""),
                new XElement(Tns + "VadeBaslangicTarihi", TarihYaz(i.VadeBaslangicTarihi)),
                new XElement(Tns + "VadeBitisTarihi", TarihYaz(i.VadeBitisTarihi)),
                new XElement(Tns + "IlgiliBankaHesabi", i.IlgiliBankaHesabi ?? ""),
                new XElement(Tns + "IlgiliProje", i.IlgiliProje ?? ""),
                new XElement(Tns + "IlgiliBirim", i.IlgiliBirim ?? ""),
                new XElement(Tns + "OdemeYapan", i.OdemeYapan ?? ""),
                new XElement(Tns + "OdemeYapilan", i.OdemeYapilan ?? ""),
                new XElement(Tns + "OybsId", i.OybsId),
                new XElement(Tns + "MuhasebeIslemAciklamasi", i.MuhasebeIslemAciklamasi ?? ""),
                new XElement(Tns + "MuhasebelestiMi", i.MuhasebelestiMi ? "true" : "false"),
                new XElement(Tns + "Aktif", i.Aktif ? "true" : "false"));
            XElement sonuc = Cagir(OybsAdres(), "Kaydet", OybsBaslik(), islem);
            long id;
            return sonuc != null && long.TryParse(sonuc.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out id) ? id : 0;
        }

        // ------------------------------------------------------------------ Evrak

        /// <summary>Eski WebServisler.Arac.EvrakServisiOlustur adresi: yazilimURL (giris.aspx atılır) + Servis/EvrakIslemServis.asmx.</summary>
        private static string EvrakAdres()
        {
            string adres = Ayar.Oku("yazilimURL");
            adres = adres.Replace("giris.aspx", "");
            if (!adres.Trim().EndsWith("/")) adres += "/";
            return adres + "Servis/EvrakIslemServis.asmx";
        }

        /// <summary>Eski EvrakIslemServis.GidenEvrakKaydet: kayıt sonucu (11 haneli ön evrak no ya da hata metni) döner.</summary>
        public static string GidenEvrakKaydet(string kullaniciKodu, string parola, string xmlEvrak)
        {
            XElement sonuc = Cagir(EvrakAdres(), "GidenEvrakKaydet", null,
                new XElement(Tns + "kullaniciKodu", kullaniciKodu ?? ""),
                new XElement(Tns + "parola", parola ?? ""),
                new XElement(Tns + "xmlEvrak", xmlEvrak ?? ""));
            return sonuc == null ? "" : sonuc.Value;
        }

        /// <summary>Eski GenelIslemlerIstemci.EvrakXMLOlustur (tek ekli).</summary>
        public static string EvrakXMLOlustur(string evrakBirimKodu, string prEvrakNo, string konu, int gelenGiden, string gidecegiYer, string dosyaKodu, string aciklama, string ekAd, string ekIcerik)
        {
            string xml =
               "<EVRAK>" +
               "<BILGI>" +
               "<PREVRAKNO>" + prEvrakNo + "</PREVRAKNO>" +
               "<EVRAKBIRIMKODU>" + evrakBirimKodu + "</EVRAKBIRIMKODU>" +
               "<KONU>" + konu + "</KONU>" +
               "<EVRAKTURU>" + gelenGiden + "</EVRAKTURU>" +
               "<GIDECEGIYER>" + gidecegiYer + "</GIDECEGIYER>" +
               "<DOSYAKODU>" + dosyaKodu + "</DOSYAKODU>" +
               "<IMZAYOLU></IMZAYOLU>" +
               "<GIZLILIK></GIZLILIK>" +
               "<YAZIDURUM></YAZIDURUM>" +
               "<ACIKLAMA>" + aciklama + "</ACIKLAMA>" +
               "<DOSYANO></DOSYANO>" +
               "<EK></EK>" +
               "<DAGITIMLAR><DAGITIM><KOD></KOD><AD></AD></DAGITIM></DAGITIMLAR>" +
               "<EVRAKILGILER><EVRAKILGI><TARIH></TARIH><AD></AD></EVRAKILGI></EVRAKILGILER>" +
               "<ANTET></ANTET>" +
               "<ICERIK></ICERIK>" +
               "<EKYAZIALAN></EKYAZIALAN>" +
               "<PARAF></PARAF>" +
               "<ALTBILGI></ALTBILGI>" +
               "<IMZA1></IMZA1>" +
               "<IMZA2></IMZA2>" +
               "<IMZA3></IMZA3>" +
               "<EKLER>" +
               "<EK AD='" + ekAd + "'>" + ekIcerik + "</EK>" +
               "</EKLER>" +
               "</BILGI>" +
               "</EVRAK>";
            return xml;
        }
    }
}

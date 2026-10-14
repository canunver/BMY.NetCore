using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TNS;
using TNS.BHF;
using TNS.KYM;

namespace Istemci.ButceForm
{
    /// <summary>
    /// Deprem harcamalarının SBB e-bütçe sistemine aktarımı (eski DepremHarcamaForm.aspx btnAktar_Click ve yardımcıları).
    /// Eski sayfa WebServisler projesindeki ButceHazirlikFormlari web başvurusunu (SOAP, .asmx) kullanıyordu; .NET 8'de o vekil
    /// sınıf olmadığından aynı "DepremHarcamaKaydet" çağrısı burada elle hazırlanan SOAP 1.1 iletisiyle yapılır.
    /// Servis adresi ayar dosyasındaki "ButceHazirlikFormlariUrl" anahtarından okunur; yoksa eski projedeki adres kullanılır.
    /// İl il gruplama, 0319-0325 -> 0318 ve 0617-0624 -> 0616 birleştirmesi ve rapor türü 1 + 2 gönderimi eskisinin aynısıdır.
    /// </summary>
    public static class DepremHarcamaAktarimi
    {
        private const string VarsayilanAdres = "http://programbutce.sbb.gov.tr/Services/ButceHazirlikFormlari.asmx";
        private static readonly XNamespace Soap = "http://schemas.xmlsoap.org/soap/envelope/";
        private static readonly XNamespace Tns = "http://tempuri.org/";
        private static readonly HttpClient istemci = new HttpClient() { Timeout = TimeSpan.FromSeconds(100) };

        /// <summary>Eski web başvurusundaki DepremHarcama sınıfının gönderilen alanları.</summary>
        private class DepremHarcama
        {
            public string EkoKod;
            public string HarcamaAciklama1;
            public string HarcamaAciklama2;
            public double[] Degerler;
        }

        /// <summary>Eski web başvurusundaki Sonuc sınıfının kullanılan alanları.</summary>
        private class WsSonuc
        {
            public bool islemSonuc;
            public string hataStr;
        }

        /// <summary>Aktarımı yapar; başarılıysa null, değilse hata metnini döndürür.</summary>
        public static async Task<string> Aktar(Kullanici kullanan, DepremHarcamaAna frm, string raporSablonYol)
        {
            Sonuc s = TNS.MUH.Arac.Tanimla().EbutceAktarimBilgiOku();
            string ebutceUser = s?.anahtar;
            string ebutcePassword = s?.bilgiStr;
            // Eski sayfa hata metninde parolayı da gösteriyordu; burada yalnızca kullanıcı kodu yazılır.
            if (string.IsNullOrWhiteSpace(ebutceUser) || string.IsNullOrWhiteSpace(ebutcePassword))
                return $"ebütçe kullanıcı bilgisi okunamadı({ebutceUser})";

            string kurumKodu = TNS.DEG.Arac.VarsayilanKurumBul() ?? "";
            string adres = Ayar.Oku("ButceHazirlikFormlariUrl", VarsayilanAdres);

            global::ButceForm.DepremRapor r = new global::ButceForm.DepremRapor(kullanan, raporSablonYol, frm);
            SortedList sl = r.GetirIlEko();
            if (sl == null)
                return "Bu birimle (muhasebe:" + frm.muhasebeKod + ", birim: " + frm.birimKod + ") ilgili verileri görme yetkiniz bulunmamaktadır.";

            List<DepremHarcama> harcamalar = new List<DepremHarcama>();
            int oncekiIl = -1;
            WsSonuc snc = null;
            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                int ilKod = alAnahtar.IntAl(0);
                string ekoKod = alAnahtar.StrAl(2);
                if (ekoKod.Length != 4) continue;

                if (ilKod != oncekiIl)
                {
                    snc = await Gonder(adres, ebutceUser, ebutcePassword, kurumKodu, frm, oncekiIl, harcamalar);
                    oncekiIl = ilKod;
                    if (snc != null && !snc.islemSonuc) break;
                    harcamalar.Clear();
                }

                DepremHarcama satir = new DepremHarcama();
                satir.Degerler = new double[6];
                satir.EkoKod = ekoKod;
                bool degerVar = false;
                for (int i = 0; i < 6; i++)
                {
                    satir.Degerler[i] = alVeri.DblAl(i);
                    if (satir.Degerler[i] > 0)
                        degerVar = true;
                }
                satir.HarcamaAciklama1 = "Muhtelif";
                satir.HarcamaAciklama2 = "Muhtelif";
                if (degerVar)
                    harcamalar.Add(satir);
            }
            if (snc == null || snc.islemSonuc)
            {
                snc = await Gonder(adres, ebutceUser, ebutcePassword, kurumKodu, frm, oncekiIl, harcamalar);
            }

            if (snc != null && !snc.islemSonuc)
                return $"Aktarım tamamlanamadı: (Url: {adres}/Kul: {ebutceUser}) " + snc.hataStr;
            return null;
        }

        private static async Task<WsSonuc> Gonder(string adres, string kullanici, string parola, string kurumKodu, DepremHarcamaAna frm, int ilKod, List<DepremHarcama> harcamalar)
        {
            WsSonuc snc = null;
            if (ilKod > 0 && harcamalar.Count > 0)
            {
                try
                {
                    KodlariDegistir(harcamalar);
                    snc = await DepremHarcamaKaydet(adres, kullanici, parola, frm.yil, frm.ay, kurumKodu, ilKod, 1, harcamalar);
                    if (snc.islemSonuc)
                    {
                        return await DepremHarcamaKaydet(adres, kullanici, parola, frm.yil, frm.ay, kurumKodu, ilKod, 2, harcamalar);
                    }
                    else
                        return snc;
                }
                catch (Exception ex)
                {
                    snc = new WsSonuc();
                    snc.islemSonuc = false;
                    snc.hataStr = ex.Message;
                }
            }
            return snc;
        }

        private static void KodlariDegistir(List<DepremHarcama> harcamalar)
        {
            DepremHarcama Eko0318 = EkoBul(harcamalar, "0318");
            DepremHarcama Eko0616 = EkoBul(harcamalar, "0616");
            foreach (var item in harcamalar)
            {
                string yeniKod = YeniKodBul(item.EkoKod);
                if (yeniKod == "0318") EkleSifirla(Eko0318, item);
                if (yeniKod == "0616") EkleSifirla(Eko0616, item);
            }

            for (int j = harcamalar.Count - 1; j >= 0; j--)
            {
                if (SifirMi(harcamalar[j]))
                {
                    harcamalar.RemoveAt(j);
                }
            }
        }

        private static bool SifirMi(DepremHarcama depremHarcama)
        {
            for (int i = 0; i < 6; i++)
            {
                if (depremHarcama.Degerler[i] > 0) return false;
            }
            return true;
        }

        private static void EkleSifirla(DepremHarcama ekoDiger, DepremHarcama item)
        {
            for (int i = 0; i < 6; i++)
            {
                ekoDiger.Degerler[i] += item.Degerler[i];
                item.Degerler[i] = 0;
            }
        }

        private static DepremHarcama EkoBul(List<DepremHarcama> harcamalar, string arananEkoKod)
        {
            foreach (var item in harcamalar)
            {
                if (item.EkoKod == arananEkoKod) return item;
            }
            DepremHarcama satir = new DepremHarcama();
            satir.Degerler = new double[6];
            satir.EkoKod = arananEkoKod;
            for (int i = 0; i < 6; i++)
            {
                satir.Degerler[i] = 0;
            }
            satir.HarcamaAciklama1 = "Muhtelif";
            satir.HarcamaAciklama2 = "Muhtelif";
            harcamalar.Add(satir);
            return satir;
        }

        private static string YeniKodBul(string ekoKod)
        {
            if (ekoKod == "0319") return "0318";
            if (ekoKod == "0320") return "0318";
            if (ekoKod == "0321") return "0318";
            if (ekoKod == "0322") return "0318";
            if (ekoKod == "0323") return "0318";
            if (ekoKod == "0324") return "0318";
            if (ekoKod == "0325") return "0318";

            if (ekoKod == "0617") return "0616";
            if (ekoKod == "0618") return "0616";
            if (ekoKod == "0619") return "0616";
            if (ekoKod == "0620") return "0616";
            if (ekoKod == "0621") return "0616";
            if (ekoKod == "0622") return "0616";
            if (ekoKod == "0623") return "0616";
            if (ekoKod == "0624") return "0616";

            return "";
        }

        // ------------------------------------------------------------------ SOAP çağrısı

        /// <summary>
        /// ButceHazirlikFormlari.asmx DepremHarcamaKaydet (SOAP 1.1, document/literal; eski vekil sınıfın gönderdiği iletinin aynısı).
        /// Sunucu hata (soap:Fault) dönerse ya da yanıt okunamazsa istisna atılır; Gonder bunu hata sonucuna çevirir.
        /// </summary>
        private static async Task<WsSonuc> DepremHarcamaKaydet(string adres, string kullaniciKodu, string parola, int yil, int ay, string kurumKod, int ilKod, int raporTur, List<DepremHarcama> harcamalar)
        {
            XElement liste = new XElement(Tns + "harcamalar");
            foreach (DepremHarcama h in harcamalar)
            {
                XElement degerler = new XElement(Tns + "Degerler", h.Degerler.Select(d => new XElement(Tns + "double", XmlConvert.ToString(d))));
                liste.Add(new XElement(Tns + "DepremHarcama",
                    new XElement(Tns + "TabloKod", 0),
                    new XElement(Tns + "EkoKod", h.EkoKod ?? ""),
                    new XElement(Tns + "CocukSay", 0),
                    new XElement(Tns + "IlKod", 0),
                    new XElement(Tns + "HarcamaAciklama1", h.HarcamaAciklama1 ?? ""),
                    new XElement(Tns + "HarcamaAciklama2", h.HarcamaAciklama2 ?? ""),
                    degerler));
            }

            XDocument ileti = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement(Soap + "Envelope", new XAttribute(XNamespace.Xmlns + "soap", Soap.NamespaceName),
                    new XElement(Soap + "Body",
                        new XElement(Tns + "DepremHarcamaKaydet",
                            new XElement(Tns + "kullaniciKodu", kullaniciKodu ?? ""),
                            new XElement(Tns + "parola", parola ?? ""),
                            new XElement(Tns + "yil", yil),
                            new XElement(Tns + "ay", ay),
                            new XElement(Tns + "kurumKod", kurumKod ?? ""),
                            new XElement(Tns + "ilKod", ilKod),
                            new XElement(Tns + "raporTur", raporTur),
                            liste))));

            using (HttpRequestMessage istek = new HttpRequestMessage(HttpMethod.Post, adres))
            {
                istek.Content = new StringContent(ileti.Declaration + ileti.ToString(SaveOptions.DisableFormatting), Encoding.UTF8, "text/xml");
                istek.Headers.Add("SOAPAction", "\"http://tempuri.org/DepremHarcamaKaydet\"");
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
                    XElement sonuc = cevap.Descendants(Tns + "DepremHarcamaKaydetResult").FirstOrDefault();
                    if (sonuc == null)
                        throw new Exception("Servis yanıtı okunamadı (HTTP " + (int)yanit.StatusCode + ").");
                    WsSonuc s = new WsSonuc();
                    s.islemSonuc = string.Equals(((string)sonuc.Element(Tns + "islemSonuc") ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
                    s.hataStr = (string)sonuc.Element(Tns + "hataStr") ?? "";
                    return s;
                }
            }
        }
    }
}

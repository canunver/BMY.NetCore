using System;
using System.Collections.Generic;
using System.Linq;
using OrtakClass;
using TNS;
using TNS.IKY;
using TNS.KYM;
using System.Data;
using TNS.UZY;
using Aspose.Words;
using System.IO;
using System.Collections;

namespace InsanKaynaklari
{
    /// <summary>
    /// Personel Raporları ekranının (eski InsanKaynaklari/RaporPersonel.aspx; RaporTanim.raporDosyasiNo 1 olan raporlar) rapor
    /// üreticisi: istatistikler (IK022-IK027, IK037-IK040, IK101, IK102), sırtlık (IK041), toplu tanıtma kartı (IK045),
    /// soruşturma (IK056). RaporAl dönüşü üretilen dosyanın tam yolu; bilinmeyen kodda eskisi gibi "" döner.
    /// Eski InsanKaynaklari/PersonelRaporlari2.cs'ten alındı; iş mantığı aynen korundu. Değişenler:
    ///   - System.Web / Ext.NET bağımlılığı ve TNS.Raporlama.IRapor arayüzü kaldırıldı; kullanılmayan TNS.ORG (TNS.IKY.Personel /
    ///     Unvan / Arac ile ad çakışması) ve TNS.Raporlama using'leri kaldırıldı.
    ///   - TanitmaKartiYazdir'ın üçüncü parametresi artık şablon klasörü değil şablon dosyasının tam yolu (eski kod
    ///     sablonYol + "\IKY\PersonelTanitmaKarti.docx" kuruyordu; Linux'ta "\" yol ayırıcı değil). IK045'te
    ///     istemciUzayi.GenelSayfa.RaporSablonYol() yerine RaporAl'a gelen sablonAd (IKY\PersonelTanitmaKarti.docx) verilir.
    ///   - Eski GenelIslemlerIstemci.AsposeAlanaResimEkle Core'da yok; sınıfa private olarak kopyalandı.
    ///   - Sırtlıktaki logo eski System.Web MapPath("~/app_themes/images/CBLogo.png") yerine SirtlikLogoYolu'ndan okunur; dosya
    ///     yoksa logo konmaz (eskisi dosya yoksa hata verirdi).
    ///   - RaporAl başında AsposeLisans.Yukle() çağrılır.
    ///   - RaporAl'dan çağrılmayan MaasIdasiHesaplamaCetveliYazdir ve yardımcıları alınmadı (PersonelOrtakRaporlar'da var).
    /// </summary>
    public class PersonelRaporlari2
    {
        private class IstatistikSinif
        {
            public string ad { get; set; }
            public double deger { get; set; }
            public IstatistikSinif(string ad, double deger)
            {
                this.ad = ad;
                this.deger = deger;
            }

            public override bool Equals(Object o)
            {
                if (o == null) return false;
                if (ad == null) return false;
                if (o is string)
                {
                    return ad.Equals((string)o);
                }
                else if (o is IstatistikSinif)
                {
                    return ad.Equals(((IstatistikSinif)o).ad);
                }
                return true;
            }
        }

        public static byte[] ResimGoster(TNS.IKY.IIKYServis servisIKY, int kod)
        {
            if (kod > 0)
            {
                return servisIKY.PersonelResimGetir(kod, 0, 0, 0);
                //ObjectArray liste = servisIKY.PersonelResimGetir(kod);
                //if (liste.sonuc.islemSonuc)
                //    foreach (PersonelResim resim in liste.objeler)
                //        return resim.resim;
            }
            return null;
        }

        /// <summary>sablonDosyaAd: PersonelTanitmaKarti.docx şablonunun tam yolu (eskisi şablon klasörünü alıyordu).</summary>
        public static string TanitmaKartiYazdir(Kullanici kullanan, int kod, string sablonDosyaAd, string ciktiTur)
        {
            TNS.IKY.IIKYServis servisIKY = TNS.IKY.Arac.Tanimla();
            if (kod == 0)
            {
                throw new Exception("Lütfen kişi seçiniz");
            }
            TNS.IKY.Personel personel = servisIKY.HizmetBelgesiRaporu(kullanan, kod);
            string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;
            string dosyaAd = sablonDosyaAd; // eski: sablonYol + "\\IKY\\PersonelTanitmaKarti.docx"

            Document doc = new Document(dosyaAd);
            DocumentBuilder docb = new DocumentBuilder(doc);

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "kurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "AD", personel.ad + " " + personel.soyad);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "TC", personel.tcKimlikNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Sicil", personel.sicilNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "DogumTarihi", personel.dogumTarihi.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "DogumYeri", personel.nufus.dogumYeri);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "MedeniHali", personel.nufus.medeniHaliAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "nufusil", personel.nufus.ilAd + " - " + personel.nufus.ilceAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "AnneAdi", personel.nufus.anaAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "BabaAdi", personel.nufus.babaAd);

            DataTable tab = new DataTable();
            tab.TableName = "Ogrenimler";
            tab.Columns.Add("ogrenimi");
            tab.Columns.Add("okulu");
            tab.Columns.Add("tarihi");

            if (personel.ogrenim.Length > 0)
            {
                int cnt = 1;
                foreach (TNS.IKY.PersonelOgrenim a in personel.ogrenim)
                {
                    DataRow row = tab.NewRow();
                    row["ogrenimi"] = a.ogrenimAdi;
                    row["okulu"] = a.okulAdi;
                    row["tarihi"] = a.belgeTarihi;
                    tab.Rows.Add(row);
                    cnt++;
                }
            }
            else
            {
                DataRow row = tab.NewRow();
                row["ogrenimi"] = "";
                row["okulu"] = "";
                row["tarihi"] = "";
                tab.Rows.Add(row);
            }

            string esAd = "", kurslar = "", esMeslek = "", anneMeslek = "", babaMeslek = "";
            foreach (TNS.IKY.PersonelAile a in personel.aile)
            {
                if (a.yakinlik == ENUMYakinlikDerecesi.Esi)
                {
                    esAd = a.ad + " " + a.soyad;
                    esMeslek = a.meslek;
                }
                else if (a.yakinlik == ENUMYakinlikDerecesi.Annesi)
                    anneMeslek = a.meslek;
                if (a.yakinlik == ENUMYakinlikDerecesi.Babasi)
                    babaMeslek = a.meslek;
            }

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "EsAd", esAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "EsMeslek", esMeslek);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "BabaMeslek", babaMeslek);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "AnneMeslek", anneMeslek);

            foreach (TNS.IKY.PersonelKurs k in personel.kurs)
                kurslar += k.kursAdi + " / " + k.tarih + "\r\n";

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "kurs", kurslar);

            string diller = "";
            foreach (TNS.IKY.PersonelDil d in personel.dil)
                diller += d.dilAdi + " / " + d.belgeAdi + " / " + d.duzey + " / " + d.puan + "\r\n";

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "dil", diller);

            //var hizmetGunleri = servisIKY.HizmetleriGetir(personel.kod.ToString(), "",0, new TNSDateTime(DateTime.Now));
            //Hizmet hizmet = servisIKY.HizmetSureBilgileriniGetir(personel.kod, DateTime.Now);

            Hizmet hizmetSuresi = new Hizmet();
            foreach (Hizmet h in personel.hizmetSureleri) //1 kişi 1 hizmet süresi bilgisi dönecek
                hizmetSuresi = h;

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "THizmetSuresi", hizmetSuresi.hizmetYili.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "IseBaslama", personel.ilkIseBaslamaTarihi.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "CIseBaslama", personel.kurumaBaslamaTarihi.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "CUnvan", hizmetSuresi.guncelHizmet.unvan);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "CStatu", personel.personelTipAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "GeldigiKurum", personel.geldigiKurum);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Birim", hizmetSuresi.guncelHizmet.birim);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "KurumdakiSure", Hizmet.YilDondur(hizmetSuresi.kurumdakiSure).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "KadrodakiSure", Hizmet.YilDondur(hizmetSuresi.kadrodakiSure).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "kadroyaGecmeTarihi", hizmetSuresi.guncelHizmet.kadroyaGectigiTarih.ToShortDateString());

            ObjectArray unvanlar = servisIKY.UnvanaGoreHizmetGrupla(personel.kod);
            string isler = "";
            int uCnt = 0;
            string yilAraligi = "";
            foreach (Unvan u in unvanlar.objeler)
            {
                if (isler != "") isler += " ";
                if (u.minYil > 0)
                {
                    if (u.maxYil > 0)
                        yilAraligi = " " + u.minYil + "-" + u.maxYil;
                    else
                        yilAraligi = " " + u.minYil + "-...";
                }
                else
                    yilAraligi = " " + u.hizmetGunu + " GÜN";
                isler += KucukHarf(uCnt) + ") " + u.ad + yilAraligi;
                uCnt++;
            }
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "GecmisHizmetler", isler);

            byte[] resim = servisIKY.PersonelResimGetir(personel.kod, 0, 0, 0);

            if (resim != null)
            {
                AsposeAlanaResimEkle(docb, "resim", resim, Aspose.Words.Drawing.RelativeHorizontalPosition.Default, 0, Aspose.Words.Drawing.RelativeVerticalPosition.Paragraph, 0, Aspose.Words.Drawing.WrapType.None, 150, 150);
            }
            doc.MailMerge.ExecuteWithRegions(tab);

            if (ciktiTur == "pdf")
                doc.Save(sonucDosyaad, SaveFormat.Pdf);
            else
                doc.Save(sonucDosyaad);
            return sonucDosyaad;
        }

        static char[] alfabeTurkce = { 'a', 'b', 'c', 'ç', 'd', 'e', 'f', 'g', 'ğ', 'h', 'ı', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'ö', 'p', 'r', 's', 'ş', 't', 'u', 'ü', 'v', 'y', 'z' };
        private static string KucukHarf(int harfNo)
        {
            if (harfNo < alfabeTurkce.Length)
                return alfabeTurkce[harfNo].ToString();

            return harfNo.ToString();
        }

        //public static void OzlukRaporYazdir(Kullanici kullanan, int raporTur, int pKod)
        //{
        //    TNS.IKY.IIKYServis servisIKY = TNS.IKY.Arac.Tanimla();
        //    string sablonAd = "";
        //    if (raporTur == 1)
        //        sablonAd = "PER-PersonelOzluk1.xlt";
        //    if (raporTur == 2)
        //        sablonAd = "PER-PersonelOzluk2.xlt";

        //    Tablo XLS = GenelIslemler.NewTablo();

        //    string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();

        //    XLS.DosyaAc(System.Web.HttpContext.Current.Server.MapPath("~") + "/RaporSablon/PERv2/" + sablonAd, sonucDosyaAd);

        //    TNS.IKY.PersonelKadro pKadro = new TNS.IKY.PersonelKadro();
        //    pKadro.personelKod = pKod;
        //    ObjectArray pobj = servisIKY.PersonelKadroListele(kullanan, pKadro, true);
        //    foreach (TNS.IKY.PersonelKadro f in pobj.objeler)
        //    {
        //        pKadro = f;
        //    }

        //    ObjectArray unvanlar = new ObjectArray();
        //    unvanlar = servisIKY.UnvanListele(new Unvan());
        //    foreach (Unvan unvan in unvanlar.objeler)
        //        if (unvan.kod == pKadro.unvan)
        //            XLS.HucreAdBulYaz("Unvan", unvan.ad);
        //    XLS.HucreAdBulYaz("kadroDerece", pKadro.kadroDerece);
        //    XLS.HucreAdBulYaz("kadroTarih", pKadro.kadroTarih.ToString());
        //    XLS.HucreAdBulYaz("odemeyeEsasDerece", pKadro.odemeyeEsasDerece);
        //    XLS.HucreAdBulYaz("odemeyeEsasKademe", pKadro.odemeyeEsasKademe);
        //    XLS.HucreAdBulYaz("odemeyeEsasTarih", pKadro.odemeyeEsasTarih.ToString());
        //    XLS.HucreAdBulYaz("odemeyeEsasGosterge", pKadro.odemeyeEsasGosterge);
        //    XLS.HucreAdBulYaz("odemeyeEsasEkGosterge", pKadro.odemeyeEsasEkGosterge);
        //    XLS.HucreAdBulYaz("kazanilmisHakDerece", pKadro.kazanilmisHakDerece);
        //    XLS.HucreAdBulYaz("kazanilmisHakKademe", pKadro.kazanilmisHakKademe);
        //    XLS.HucreAdBulYaz("kazanilmisHakTarih", pKadro.kazanilmisHakTarih.ToString());
        //    XLS.HucreAdBulYaz("kazanilmisEkGosterge", pKadro.kazanilmisEkGosterge);
        //    XLS.HucreAdBulYaz("kazanilmisGosterge", pKadro.kazanilmisGosterge);
        //    XLS.HucreAdBulYaz("emekliyeEsasDerece", pKadro.emekliyeEsasDerece);
        //    XLS.HucreAdBulYaz("emekliyeEsasKademe", pKadro.emekliyeEsasKademe);
        //    XLS.HucreAdBulYaz("emekliyeEsasTarih", pKadro.emekliyeEsasTarih.ToString());
        //    XLS.HucreAdBulYaz("emekliyeEsasGosterge", pKadro.emekliyeEsasGosterge);
        //    XLS.HucreAdBulYaz("emekliyeEsasEkGosterge", pKadro.emekliyeEsasEkGosterge);

        //    TNS.IKY.Personel personelbilgisi = new TNS.IKY.Personel();
        //    personelbilgisi.kod = pKod;
        //    ObjectArray pBil = servisIKY.PersonelListele(kullanan, personelbilgisi, true, false);
        //    foreach (TNS.IKY.Personel f in pBil.objeler)
        //        personelbilgisi = f;

        //    if (raporTur == 2)//Şart Silinebilir.
        //    {
        //        XLS.HucreAdBulYaz("Adi", personelbilgisi.ad);
        //        XLS.HucreAdBulYaz("Soyadi", personelbilgisi.soyad);
        //        //XLS.HucreAdBulYaz("muhasebeKodu", personelbilgisi.muhasebeKodu);
        //        //XLS.HucreAdBulYaz("muhasebeAdi", personelbilgisi.muhasebeAdi);//BARIS:Yapılacak
        //        //XLS.HucreAdBulYaz("birimKodu", personelbilgisi.birimKodu);
        //        //XLS.HucreAdBulYaz("birimAd", personelbilgisi.birimAd);
        //        XLS.HucreAdBulYaz("Cinsiyeti", Resources.InsanKaynaklari.ResourceManager.GetString("FRMPABC" + personelbilgisi.cinsiyet.ToString("d3")));
        //        //XLS.HucreAdBulYaz("vergiDairesi", personelbilgisi.vergiDairesi);
        //        //XLS.HucreAdBulYaz("saglikKarneNo", personelbilgisi.saglikKarneNo);
        //        //XLS.HucreAdBulYaz("esDurumuKodu", personelbilgisi.esDurumuKodu);//BARIS
        //        //XLS.HucreAdBulYaz("durum", personelbilgisi.durum);//BARIS
        //        Banka kriterBanka = new Banka();
        //        //kriterBanka.kod = personelbilgisi.bankaKodu;
        //        //ObjectArray banka = servisIKY.BankaListele(kriterBanka);
        //        //XLS.HucreAdBulYaz("bankaKodu", banka.objeler.Count > 0 ? ((Banka)(banka.objeler[0])).ad : "");
        //        //XLS.HucreAdBulYaz("bankaSubeKodu", personelbilgisi.bankaSubeKodu);
        //        BankaSube kriterBankaSube = new BankaSube();
        //        //kriterBanka.kod = personelbilgisi.bankaSubeKodu;
        //        //ObjectArray bankaSube = servisIKY.BankaSubeListele(kriterBankaSube);
        //        //XLS.HucreAdBulYaz("bankaSubeAdi", bankaSube.objeler.Count > 0 ? ((BankaSube)(bankaSube.objeler[0])).ad : "");
        //        //XLS.HucreAdBulYaz("bankaHesapNo", personelbilgisi.bankaHesapNo);
        //        Ogrenim kriterOgrenim = new Ogrenim();
        //        //kriterOgrenim.kod = personelbilgisi.ogrenimKodu;
        //        //ObjectArray ogrenimSozluk = servisIKY.OgrenimListele(kriterOgrenim);
        //        //XLS.HucreAdBulYaz("ogrenim", ogrenimSozluk.objeler.Count > 0 ? ((Ogrenim)(ogrenimSozluk.objeler[0])).ad : "");
        //    }

        //    XLS.HucreAdBulYaz("AdSoyad", personelbilgisi.ad + " " + personelbilgisi.soyad);
        //    XLS.HucreAdBulYaz("SicilNo", personelbilgisi.sicilNo);
        //    XLS.HucreAdBulYaz("EmekliSicilNo", personelbilgisi.emekliSicilNo);
        //    XLS.HucreAdBulYaz("BabaAdi", personelbilgisi.nufus.babaAd);
        //    XLS.HucreAdBulYaz("AnaAdi", personelbilgisi.nufus.anaAd);
        //    XLS.HucreAdBulYaz("DogumYeri", personelbilgisi.nufus.dogumYeri);
        //    XLS.HucreAdBulYaz("DogumTarihi", personelbilgisi.dogumTarihi.ToString());
        //    XLS.HucreAdBulYaz("MedeniHali", personelbilgisi.nufus.medeniHali == 1 ? Resources.InsanKaynaklari.FRMORT037 : Resources.InsanKaynaklari.FRMORT036); //BARIS
        //    XLS.HucreAdBulYaz("KanGrubu", personelbilgisi.nufus.kanGrubu);
        //    //XLS.HucreAdBulYaz("BakanlikKimlikKartNo", personelbilgisi.bakanlikKartNo);

        //    XLS.HucreAdBulYaz("NufusIl", GenelIslemlerIstemci.IlAdGetir(personelbilgisi.nufus.nufusIlKod));
        //    XLS.HucreAdBulYaz("NufusIlce", GenelIslemlerIstemci.IlceAdGetir(personelbilgisi.nufus.nufusIlceKod));
        //    XLS.HucreAdBulYaz("NufusMahalleKoy", personelbilgisi.nufus.nufusMahalleKoy);
        //    XLS.HucreAdBulYaz("CiltNo", personelbilgisi.nufus.nufusCiltNo);
        //    XLS.HucreAdBulYaz("SiraNo", personelbilgisi.nufus.nufusSiraNo);
        //    XLS.HucreAdBulYaz("AileSiraNo", personelbilgisi.nufus.nufusAileSiraNo);
        //    XLS.HucreAdBulYaz("NufusDairesi", personelbilgisi.nufus.cuzdanVerildigiYer);
        //    XLS.HucreAdBulYaz("VerilisTarihi", personelbilgisi.nufus.cuzdanVerilisTarihi.ToString());
        //    XLS.HucreAdBulYaz("VerilisNedeni", personelbilgisi.nufus.cuzdanVerilisNedeni);
        //    XLS.HucreAdBulYaz("CuzdanSeriNo", personelbilgisi.nufus.cuzdanSeriNo);
        //    XLS.HucreAdBulYaz("CuzdanKayitNo", personelbilgisi.nufus.cuzdanKayitNo);
        //    XLS.HucreAdBulYaz("TCNo", personelbilgisi.tcKimlikNo);
        //    //XLS.HucreAdBulYaz("AskerlikDurumu", Resources.InsanKaynaklari.ResourceManager.GetString("FRMPASD" + (personelbilgisi.askerlikKodu).ToString("d3")));

        //    //if (personelbilgisi.askerlikKodu == 1)
        //    //{
        //    //    XLS.HucreAdBulYaz("AskerlikBaslamaTarihi", personelbilgisi.askerlikBaslamaTarihi.ToString());
        //    //    XLS.HucreAdBulYaz("AskerlikTerhisTarihi", personelbilgisi.terhisTarihi.ToString());
        //    //}

        //    int cocukSayac = 0;
        //    int yardimAlanCocukSayac = 0;
        //    TNS.IKY.PersonelAile personelAileBilgisi = new TNS.IKY.PersonelAile();
        //    personelAileBilgisi.personelKod = pKod;
        //    ObjectArray pAileBireyleri = servisIKY.PersonelAileListele(kullanan, 0, pKod, null, null, null, 0);
        //    foreach (TNS.IKY.PersonelAile aileBireyi in pAileBireyleri.objeler)
        //    {
        //        if (aileBireyi.yakinlik == ENUMYakinlikDerecesi.Esi)
        //            XLS.HucreAdBulYaz("EsininAdi", aileBireyi.ad);
        //        if (aileBireyi.yakinlik == ENUMYakinlikDerecesi.Oglu || aileBireyi.yakinlik == ENUMYakinlikDerecesi.Kizi)
        //        {
        //            cocukSayac++;
        //            if (DateTime.Now.Year - aileBireyi.dogumTarihi.Yil < 6)
        //                yardimAlanCocukSayac++;
        //        }
        //    }
        //    XLS.HucreAdBulYaz("CocukSayisi", cocukSayac.ToString());
        //    XLS.HucreAdBulYaz("YardimAlanCocukSayisi", yardimAlanCocukSayac.ToString());

        //    TNS.IKY.PersonelAdres pAdres = new TNS.IKY.PersonelAdres();
        //    pAdres.kod = pKod;
        //    PersonelIletisim pAdresBilgileri = servisIKY.PersonelAdresListele(kullanan, pAdres.kod);

        //    XLS.HucreAdBulYaz("IkametgahIsTelefon", pAdres.isTel);
        //    XLS.HucreAdBulYaz("IkametgahFaks", pAdres.faks);
        //    XLS.HucreAdBulYaz("IkametgahEPosta", pAdres.eposta);

        //    XLS.HucreAdBulYaz("IkametgahEvTelefon", pAdres.evTel);
        //    XLS.HucreAdBulYaz("IkametgahCepNo", pAdres.cepTel);
        //    XLS.HucreAdBulYaz("IkametgahAdres", pAdres.adres);
        //    XLS.HucreAdBulYaz("IkametgahEvKimin", Resources.InsanKaynaklari.ResourceManager.GetString("FRMPIA" + (pAdres.mulkDurumu + 8).ToString("d3")));

        //    if (pAdres.mulkDurumu == 1)
        //    {
        //        XLS.HucreAdBulYaz("IkametgahLojmanTarihi", pAdres.tasinmaTarihi.ToString());
        //    }

        //    TNS.IKY.PersonelMalBildirimi pMalBildirimi = new TNS.IKY.PersonelMalBildirimi();
        //    pMalBildirimi.personelKod = pKod;
        //    ObjectArray pMalBildirimiBilgileri = servisIKY.PersonelMalBildirimiListele(kullanan, pMalBildirimi);
        //    if (pMalBildirimiBilgileri.objeler == null || pMalBildirimiBilgileri.objeler.Count == 0)
        //        XLS.HucreAdBulYaz("IkametgahMalBildirimi", "x");
        //    else
        //        XLS.HucreAdBulYaz("IkametgahMalBildirimi", "ü");

        //    TNS.IKY.PersonelOgrenim pOgrenim = new TNS.IKY.PersonelOgrenim();
        //    pOgrenim.personelKod = pKod;
        //    //ObjectArray pOgrenimBilgileri = servisIKY.PersonelOgrenimListele(kullanan, pOgrenim);
        //    //foreach (TNS.IKY.PersonelOgrenim f in pOgrenimBilgileri.objeler)
        //    //{
        //    //    Ogrenim og = new Ogrenim();
        //    //    og.kod = f.ogrenimKod;
        //    //    ObjectArray ogrenim = servisIKY.OgrenimListele(og);
        //    //    if (ogrenim.objeler != null && ogrenim.objeler.Count > 0)
        //    //        switch (((Ogrenim)ogrenim.objeler[0]).tip)
        //    //        {
        //    //            case 1:
        //    //                XLS.HucreAdBulYaz("IlkokulAd", f.okulAdi);
        //    //                XLS.HucreAdBulYaz("IlkokulTarih", f.bitirmeTarihi.ToString());
        //    //                break;
        //    //            case 2:
        //    //                XLS.HucreAdBulYaz("OrtaokulTarih", f.bitirmeTarihi.ToString());
        //    //                XLS.HucreAdBulYaz("OrtaokulAd", f.okulAdi);
        //    //                break;
        //    //            case 3:
        //    //                XLS.HucreAdBulYaz("LiseTarih", f.bitirmeTarihi.ToString());
        //    //                XLS.HucreAdBulYaz("LiseAd", f.okulAdi);
        //    //                break;
        //    //            case 4:
        //    //                XLS.HucreAdBulYaz("OnLisansTarih", f.bitirmeTarihi.ToString());
        //    //                XLS.HucreAdBulYaz("OnlisansAd", f.okulAdi);
        //    //                break;
        //    //            case 5:
        //    //                XLS.HucreAdBulYaz("LisansTarih", f.bitirmeTarihi.ToString());
        //    //                XLS.HucreAdBulYaz("LisansAd", f.okulAdi);
        //    //                break;
        //    //            case 6:
        //    //                XLS.HucreAdBulYaz("YuksekLisansTarih", f.bitirmeTarihi.ToString());
        //    //                XLS.HucreAdBulYaz("YuksekLisans", f.okulAdi);
        //    //                break;
        //    //        }
        //    //}

        //    XLS.HucreAdBulYaz("KadroSinifi", pKadro.sinifi);
        //    //XLS.HucreAdBulYaz("KurumGirisTarihi", personelbilgisi.sonBirimeBaslamaTarihi.ToString());
        //    //XLS.HucreAdBulYaz("sonGoreveBaslamaTarihi", personelbilgisi.sonGoreveBaslamaTarihi.ToString());
        //    XLS.HucreAdBulYaz("MemuriyetTarihi", personelbilgisi.ilkIseBaslamaTarihi.ToString());

        //    //if (!personelbilgisi.sonBirimeBaslamaTarihi.isNull)
        //    //{
        //    //    DateDifference d = new DateDifference(personelbilgisi.sonBirimeBaslamaTarihi.Oku(), DateTime.Now);
        //    //    XLS.HucreAdBulYaz("Hizmet", (d.Years > 0 ? d.Years.ToString() + "yıl " : "") + (d.Months > 0 ? d.Months.ToString() + "ay " : "") + (d.Days > 0 ? d.Days.ToString() + "gün" : ""));
        //    //}
        //    //XLS.HucreAdBulYaz("AlisTarihi", );
        //    //XLS.HucreAdBulYaz("KidemYeniTarih", pKadro.kidemTarihi.ToString());

        //    TimeSpan toplamMemuriyet = new TimeSpan();
        //    TNS.IKY.PersonelMemuriyet pMemur = new TNS.IKY.PersonelMemuriyet();
        //    pMemur.personelKod = pKod;
        //    int kaynakSatir = 0;
        //    int satir = 0;
        //    int sutun = 0;
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
        //    satir = kaynakSatir;
        //    //ObjectArray personelmemuriyetbilgileri = servisIKY.PersonelMemuriyetListele(kullanan, pMemur);
        //    //XLS.SatirKopyalaAc(kaynakSatir, personelmemuriyetbilgileri.objeler.Count - 1, kaynakSatir + 1);
        //    //foreach (TNS.IKY.PersonelMemuriyet f in personelmemuriyetbilgileri.objeler)
        //    //{
        //    //    if (!f.ayrilmaTarihi.isNull)
        //    //        toplamMemuriyet += f.ayrilmaTarihi.Oku() - f.baslamaTarihi.Oku();
        //    //    else
        //    //        toplamMemuriyet += DateTime.Now - f.baslamaTarihi.Oku();

        //    //    if (f.ayrilmaTarihi.isNull)
        //    //        XLS.HucreAdBulYaz("Gorev", f.gorev);

        //    //    satir++;
        //    //    if (f.birimAd.Contains("-"))
        //    //        XLS.HucreDegerYaz(satir, 0, f.birimAd.Substring(f.birimAd.IndexOf('-') + 1));
        //    //    XLS.HucreDegerYaz(satir, 2, f.gorev);
        //    //    XLS.HucreDegerYaz(satir, 5, f.baslamaTarihi.ToString());
        //    //    XLS.HucreDegerYaz(satir, 7, f.ayrilmaTarihi.ToString());
        //    //    //ObjectArray ayrilmaNedenler = servisIKY.NedenListele(new Neden());
        //    //    //if (ayrilmaNedenler.sonuc.islemSonuc)
        //    //    //    foreach (Neden ayrilmaNedeni in ayrilmaNedenler.objeler)
        //    //    //        if (f.ayrilmaNedeni == ayrilmaNedeni.kod)
        //    //    //            XLS.HucreDegerYaz(satir, 8, ayrilmaNedeni.ad);

        //    //    XLS.HucreBirlestir(satir, 0, satir, 1);
        //    //    XLS.HucreBirlestir(satir, 2, satir, 4);
        //    //    XLS.HucreBirlestir(satir, 5, satir, 6);
        //    //}
        //    if (!personelbilgisi.ilkIseBaslamaTarihi.isNull)
        //    {
        //        int m_yil = ((int)(toplamMemuriyet.TotalDays / 365));
        //        int m_ay = ((int)((toplamMemuriyet.TotalDays % 365) / 30));
        //        int m_gun = ((int)((toplamMemuriyet.TotalDays % 365) % 30));
        //        XLS.HucreAdBulYaz("HizmetSuresiYil", (m_yil > 0 ? m_yil.ToString() + "yıl " : "")
        //                                            + (m_ay > 0 ? m_ay.ToString() + "ay " : "")
        //                                            + (m_gun > 0 ? m_gun.ToString() + "gün" : ""));
        //    }

        //    int kursSay = 0;
        //    TNS.IKY.PersonelKurs pKurs = new TNS.IKY.PersonelKurs();
        //    pKurs.personelKod = pKod;
        //    ObjectArray personelkursbilgileri = servisIKY.PersonelKursListele(kullanan, pKurs);
        //    foreach (TNS.IKY.PersonelKurs kurs in personelkursbilgileri.objeler)
        //    {
        //        kursSay++;
        //        XLS.HucreAdBulYaz("Egitim" + kursSay.ToString() + "Icerik", kurs.kursAdi);
        //        XLS.HucreAdBulYaz("Egitim" + kursSay.ToString() + "Duzey", kurs.derece);
        //        XLS.HucreAdBulYaz("Egitim" + kursSay.ToString() + "BaslamaTarihi", kurs.baslamaTarihi.ToString());
        //        XLS.HucreAdBulYaz("Egitim" + kursSay.ToString() + "BitisTarihi", kurs.bitirmeTarihi.ToString());
        //        XLS.HucreAdBulYaz("Egitim" + kursSay.ToString() + "Yer", kurs.aldigiYer);
        //    }

        //    //int dilSay = 0;
        //    //TNS.IKY.PersonelDil pDil = new TNS.IKY.PersonelDil();
        //    //pDil.personelKod = OrtakFonksiyonlar.ConvertToInt(hdnKod.Value, 0);
        //    //ObjectArray pdiller = servisIKY.PersonelDilListele(kullanan, pDil);
        //    //foreach (TNS.IKY.PersonelDil dil in pdiller.objeler)
        //    //{
        //    //    dilSay = dilSay + 1;
        //    //    XLS.HucreAdBulYaz("Yabanci" + dilSay.ToString() + "Dil", dil.dilAd);
        //    //    XLS.HucreAdBulYaz("Yabanci" + dilSay.ToString() + "Duzey", dil.duzey);
        //    //    XLS.HucreAdBulYaz("Yabanci" + dilSay.ToString() + "Dayanak", Resources.InsanKaynaklari.ResourceManager.GetString("FRMPDLB" + dil.belgeTur.ToString("d3")));//f.dayanak);//BARIS:derece olabilir, daha önceden dayanak yazıyordu.
        //    //    XLS.HucreAdBulYaz("Yabanci" + dilSay.ToString() + "Tazminat", dil.belgeTarihi.ToString());//f.tazminattarihi.ToString());//BARIS:belge türü olabilir
        //    //}

        //    //Resim ekleme
        //    XLS.ResimEkle(8, 3, .80, 10, new MemoryStream(ResimGoster(servisIKY, pKod)));

        //    XLS.DosyaSaklaTamYol();
        //    OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "Personel" + pKod.ToString(), true, GenelIslemler.ExcelTur());
        //}

        private string IstatistikYazdir(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            string sqlSay = null;
            string istAd = null;
            string istCol1 = null;
            string istCol2 = null;

            if (ekBilgi == "IK022")
            {
                sqlSay = @"SELECT S.KAYITAD, COUNT(*) FROM Personel.dbo.PERPERSONEL P LEFT OUTER JOIN InsanKaynaklari.dbo.IKYSOZLUK S ON S.KOD = P.OGRENIMKODU 
                            where P.AKTIFMI=1 {0} {1} group by S.KAYITAD order BY 2 DESC";
                istAd = "Öğrenim Durumlarına Göre Personel Dağılımı";
                istCol1 = "Öğrenim";
                istCol2 = "Sayı";
            }
            else if (ekBilgi == "IK023")
            {
                sqlSay = "SELECT CASE when P.CINSIYET=" + ((int)ENUMCINSIYET.ERKEK).ToString() + " then 'Erkek' when P.CINSIYET=" + ((int)ENUMCINSIYET.KADIN).ToString() + " then 'Kadın' else 'Tanımsız' END , COUNT(*) FROM Personel.dbo.PERPERSONEL P "
                       + "where P.AKTIFMI=1 {0} {1} group by P.CINSIYET order BY 2 DESC";
                istAd = "Cinsiyetlerine Göre Personel Dağılımı";
                istCol1 = "Cinsiyet";
                istCol2 = "Sayı";
            }
            else if (ekBilgi == "IK037")
            {
                sqlSay = "SELECT CASE WHEN A.ESSAY IS NULL OR A.ESSAY = 0 THEN 'Evli Değil' ElSE 'Evli' END DURUM, COUNT(*) "
                         + " FROM Personel.dbo.PERPERSONEL P LEFT OUTER JOIN  "
                         + " (SELECT A.PKOD PKOD, COUNT(*) ESSAY FROM IKYPERSONELAILE A WHERE A.YAKINLIKDERECESI IN (" + ((int)ENUMYakinlikDerecesi.Esi).ToString() + ") GROUP BY A.PKOD) A "
                         + " ON P.KOD = A.PKOD where P.AKTIFMI=1 {0} {1} group BY A.ESSAY ORDER BY 2 DESC";
                istAd = "Evlilik Durumuna Göre Dağılım";
                istCol1 = "Evlilik Durumu";
                istCol2 = "Sayı";
            }
            else if (ekBilgi == "IK038")
            {
                sqlSay = "SELECT CASE WHEN A.COCUKSAY IS NULL OR A.COCUKSAY = 0 THEN 'Çocuksuz' ElSE CONVERT(varchar(2), A.COCUKSAY) +  ' Çocuklu' END DURUM, COUNT(*) "
                         + " FROM Personel.dbo.PERPERSONEL P LEFT OUTER JOIN  "
                         + " (SELECT A.PKOD PKOD, COUNT(*) COCUKSAY FROM IKYPERSONELAILE A WHERE A.YAKINLIKDERECESI IN (" + ((int)ENUMYakinlikDerecesi.Kizi).ToString() + ", " + ((int)ENUMYakinlikDerecesi.Oglu).ToString() + ") GROUP BY A.PKOD) A "
                         + " ON P.KOD = A.PKOD where P.AKTIFMI=1 {0} {1} group BY A.COCUKSAY ORDER BY 2 DESC";
                istAd = "Çocuk Sayısına Göre Dağılım";
                istCol1 = "Çocuk Sayısı";
                istCol2 = "Sayı";
            }
            else if (ekBilgi == "IK025")
            {
                sqlSay = @"SELECT DATEDIFF(YEAR, DOGUMTARIHI, GETDATE()), COUNT(*) FROM Personel.dbo.PERPERSONEL P where P.AKTIFMI=1 {0} {1} group by DATEDIFF(YEAR, DOGUMTARIHI, GETDATE()) order BY 1";
                istAd = "Yaş Gruplarına Göre Personel Dağılımı";
                istCol1 = "Yaş Grubu";
                istCol2 = "Sayı";
            }
            else if (ekBilgi == "IK026")
            {
                sqlSay = @"SELECT S.KAYITAD, COUNT(*) FROM Personel.dbo.PERPERSONEL P INNER JOIN IKYPERSONELDIL D ON P.KOD=D.PKOD INNER JOIN IKYSOZLUK S ON S.KOD=D.DILKOD where P.AKTIFMI=1 {0} {1} group BY S.KAYITAD ORDER BY 2 DESC";
                istAd = "Bildikleri Dillere Göre Personel Dağılımı";
                istCol1 = "Dil";
                istCol2 = "Sayı";
            }
            else if (ekBilgi == "IK027")
            {
                sqlSay = @"SELECT K.GUN/360, COUNT(*)
                            FROM Personel.dbo.PERPERSONEL P LEFT OUTER JOIN  (SELECT K.PKOD AS KOD, SUM(dbo.gunDondur(K.KADROTARIH, K.AYRILMATARIHI, 1, null, K.HizmetGunSayisi)) AS GUN
                            FROM IKYPERSONELKADRO K group by K.PKOD) K ON P.KOD = K.KOD where P.AKTIFMI=1 {0} {1} GROUP BY K.GUN/360 ORDER BY 1";
                istAd = "Kıdem Yılına Göre Personel Dağılımı";
                istCol1 = "Yıl Grubu";
                istCol2 = "Sayı";
            }
            else if (ekBilgi == "IK039")
            {
                sqlSay = @"SELECT dbo.gunDondur(ISEBASLAMATARIHI, getDate(), 1, null, 0) /360, count(*)
                            FROM Personel.dbo.PERPERSONEL P WHERE P.AKTIFMI=1  {0} {1}
                            group by dbo.gunDondur(ISEBASLAMATARIHI, getDate(), 1, null, 0) /360 ORDER BY 1";
                istAd = "Kurumda Çalıştığı Yıla Göre Personel Dağılımı";
                istCol1 = "Yıl Grubu";
                istCol2 = "Sayı";
            }
            else if (ekBilgi == "IK040")
            {
                sqlSay = @"SELECT dbo.gunDondur(MEMURBASLAMATARIHI, getDate(), 1, null, 0) /360, count(*)
                            FROM Personel.dbo.PERPERSONEL P WHERE P.AKTIFMI=1 {0} {1}
                            group by dbo.gunDondur(MEMURBASLAMATARIHI, getDate(), 1, null, 0) /360 ORDER BY 1";
                istAd = "Emekliliğe Sayılan Yıla Göre Personel Dağılımı";
                istCol1 = "Yıl Grubu";
                istCol2 = "Sayı";
            }
            else  //ekBilgi == IK024
            {
                sqlSay = @"SELECT S.AD, COUNT(*) FROM Personel.dbo.PERPERSONEL P LEFT OUTER JOIN InsanKaynaklari.dbo.IKYPERSONELTIPI S ON S.KOD = P.ISTIHDAMSEKLI 
                            where P.AKTIFMI=1 {0} {1} group by S.AD order BY 2 DESC";
                istAd = "İstihdam Şekillerine Göre Personel Dağılımı";
                istCol1 = "İstihdam Şekli";
                istCol2 = "Sayı";
            }
            sqlSay = String.Format(sqlSay, BirimKosulOl(orgKod), TurKosulOl("P.ISTIHDAMSEKLI"));
            VTSorgula vtSayilar = new VTSorgula(sqlSay, "BaglantiSatiriIKY", 0);
            ArrayList al = new ArrayList();
            IstatistikSinif yaslar = null;
            while (true)
            {
                DataRow dr = vtSayilar.satirOku();
                if (dr == null) break;
                if (ekBilgi == "IK025" || ekBilgi == "IK027" || ekBilgi == "IK039" || ekBilgi == "IK040")
                {
                    int yas = vtSayilar.IntAl(0);
                    int sayi = vtSayilar.IntAl(1);
                    int grup;
                    if (ekBilgi == "IK025")
                    {
                        if (yas < 20) grup = 1;
                        else if (yas < 30) grup = 2;
                        else if (yas < 40) grup = 3;
                        else if (yas < 50) grup = 4;
                        else if (yas < 60) grup = 5;
                        else grup = 6;
                    }
                    else
                    {
                        if (yas <= 5) grup = 1;
                        else if (yas <= 10) grup = 2;
                        else if (yas <= 15) grup = 3;
                        else if (yas <= 20) grup = 4;
                        else if (yas <= 25) grup = 5;
                        else grup = 6;
                    }

                    string grupAd = (ekBilgi == "IK025") ? YasGrupAdOl(grup) : YilGrupAdOl(grup);
                    int yer = al.IndexOf(grupAd);
                    if (yer == -1)
                    {
                        yaslar = new IstatistikSinif(grupAd, 0);
                        al.Add(yaslar);
                    }
                    else
                        yaslar = (IstatistikSinif)al[yer];
                    yaslar.deger += sayi;
                }
                else
                    al.Add(new IstatistikSinif(vtSayilar.StringAl(0), vtSayilar.IntAl(1)));
            }
            return IstatistikBilgiYazdir(sablonAd, sonucDosyaAd, ciktiYeri, istAd, istCol1, istCol2, al);
        }

        private string TurKosulOl(string alanAd)
        {
            return PersonelTurKosulOl("AND", alanAd, personelTurler);
        }

        private string BirimKosulOl(string kopruKod)
        {
            if (!string.IsNullOrWhiteSpace(kopruKod))
            {
                return " AND (SELECT KOPRUKOD FROM Kullanici.dbo.ORGBIRIM OB WHERE OB.KOD=P.ORGANIZASYONKOD) LIKE '" + kopruKod + "'";
            }
            return "";

        }

        public static string PersonelTurKosulOl(string baglac, string alan, string sPersonelTurler)
        {
            string donusDeger = "";
            if (!string.IsNullOrWhiteSpace(sPersonelTurler))
            {
                string[] personelTurler = sPersonelTurler.Split(',', ';');
                foreach (string item in personelTurler)
                {
                    int pturKod = OrtakFonksiyonlar.ConvertToInt(item, 0);
                    if (pturKod > 0)
                    {
                        if (donusDeger == "")
                            donusDeger = baglac + " " + alan + " IN (" + pturKod.ToString();
                        else
                            donusDeger += ", " + pturKod.ToString();
                    }
                }
                if (donusDeger != "")
                {
                    donusDeger += ")";
                }
            }
            return donusDeger;
        }

        private string YilGrupAdOl(int grup)
        {
            if (grup == 1) return "0-5 Yıl";
            else if (grup == 2) return "6-10 Yıl";
            else if (grup == 3) return "11-15 Yıl";
            else if (grup == 4) return "16-20 Yıl";
            else if (grup == 5) return "21-25 Yıl";
            else return "26 Yıl üstü";
        }

        private string YasGrupAdOl(int grup)
        {
            if (grup == 1) return "20 Yaş altı";
            else if (grup == 2) return "20-29 Yaş aralığı";
            else if (grup == 3) return "30-39 Yaş aralığı";
            else if (grup == 4) return "40-49 Yaş aralığı";
            else if (grup == 5) return "50-59 Yaş aralığı";
            else return "60 Yaş üstü";
        }

        private string IstatistikBilgiYazdir(string sablonAd, string sonucDosyaAd, string ciktiYeri, string raporAd, string sutun1ad, string sutun2ad, ArrayList al)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);
            int satirSay = al.Count;
            int satir = 0, sutun = 0, fsatir = 0, fsutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int baslaSatir = satir;
            int yukseklik = XLS.SatirYukseklikAl(baslaSatir);
            XLS.HucreAdAdresCoz("FormatSatir", ref fsatir, ref fsutun);

            XLS.HucreDegerYaz(satir - 2, sutun, raporAd);
            XLS.HucreDegerYaz(satir - 1, sutun, sutun1ad);
            XLS.HucreDegerYaz(satir - 1, sutun + 1, sutun2ad);
            if (satirSay > 2)
                XLS.SatirAc(satir + 1, satirSay - 2);

            double toplam = 0;
            IstatistikSinif iSinif = null;
            for (int i = 0; i < satirSay; i++)
            {
                if (i > 0 && i < satirSay - 1)
                    XLS.HucreKopyala(fsatir, fsutun, fsatir, fsutun + 2, satir + i, sutun);
                iSinif = (IstatistikSinif)al[i];
                toplam += iSinif.deger;
            }

            for (int i = 0; i < satirSay; i++)
            {
                iSinif = (IstatistikSinif)al[i];
                XLS.HucreDegerYaz(satir, sutun, iSinif.ad);
                XLS.HucreDegerYaz(satir, sutun + 1, iSinif.deger);
                XLS.HucreDegerYaz(satir, sutun + 2, iSinif.deger * 100 / toplam);
                satir++;
            }
            if (satirSay < 2)
            {
                satir += (2 - satirSay);
            }
            XLS.HucreDegerYaz(satir, sutun + 1, toplam);
            XLS.SatirYukseklikAyarla(satir, baslaSatir, yukseklik);
            if (satirSay == 1)
            {
                XLS.SatirSil(baslaSatir + 1, baslaSatir + 1);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string TanitmaKartiYazdirToplu(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            string[] personelKodlar = null;
            string personelKod = (string)raporParam["personelKod"];
            if (string.IsNullOrWhiteSpace(personelKod))
            {
                TNS.IKY.IIKYServis servisIKY = TNS.IKY.Arac.Tanimla();

                TNS.IKY.Personel personel = new TNS.IKY.Personel();
                personel.genelSorgu = (string)raporParam["sorgu"];
                personel.aktifDurumlar = (string)raporParam["aktifDurumlar"];
                personel.aktifDurum = (int)raporParam["aktiflik"];
                personel.personelTipleri = (string)raporParam["personelTur"];
                //string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
                TNS.ObjectArray liste = servisIKY.PersonelListele((Kullanici)kullanan, personel, false, false);
                personelKodlar = new string[liste.ObjeSayisi];
                for (int i = 0; i < personelKodlar.Length; i++)
                {
                    personelKodlar[i] = ((TNS.IKY.Personel)liste.objeler[i]).kod.ToString();
                }
            }
            else
                personelKodlar = personelKod.Split(',', ';');

            if (personelKodlar == null || personelKodlar.Length == 0)
                throw new Exception("Lütfen personel seçiniz!");

            Document doc = null;
            DocumentBuilder docb = null;

            for (int i = 0; i < personelKodlar.Length; i++)
            {
                int kod = OrtakFonksiyonlar.ConvertToInt(personelKodlar[i], 0);
                if (kod > 0)
                {
                    string ciktiAd = TanitmaKartiYazdir((Kullanici)kullanan, kod, sablonAd, "docx"); // eski: istemciUzayi.GenelSayfa.RaporSablonYol()
                    if (i == 0)
                    {
                        doc = new Document(ciktiAd);
                        docb = new DocumentBuilder(doc);
                    }
                    else
                    {
                        docb.MoveToDocumentEnd();
                        docb.InsertBreak(BreakType.PageBreak);
                        GenelIslemlerIstemci.AsposeBuilderaDocEkle(docb, ciktiAd);
                    }
                    try
                    {
                        System.IO.File.Delete(ciktiAd);
                    }
                    catch { }
                }
            }
            if (ciktiYeri != "pdf") ciktiYeri = "docx";
            doc.Save(sonucDosyaAd + "." + ciktiYeri);
            return sonucDosyaAd + "." + ciktiYeri;
        }

        private string SirtlikYazdir(object kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            TNS.IKY.Personel p = new TNS.IKY.Personel();
            p.ogrenimListesiOlsun = false;
            p.resimOlsun = true;
            p.kodlar = (string)raporParam["personelKod"];
            p.personelTipleri = (string)raporParam["personelTur"];
            p.kopruKod = (string)raporParam["ORGKOD"];
            p.aktifDurum = (int)raporParam["aktiflik"];
            p.genelSorgu = (string)raporParam["sorgu"];

            p.aktifTarih = new TNSDateTime((string)raporParam["tarihSorgu"]);
            p.aktifTarihTur = (int)raporParam["tarihSorguTur"];

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri; // GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);
            int satir = 0, sutun = 1, sutun2 = 0;
            int kaynakSatir = 0, boyutSatir = 0;
            ObjectArray arr = new ObjectArray();
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            int genislik = XLS.SutunGenislikAl(sutun2);
            int yukseklik = XLS.SatirYukseklikAl(boyutSatir);

            ObjectArray list = servisIKY.PersonelListele((TNS.KYM.Kullanici)kullanan, p, false, false);

            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);
            int toplamSay = list.objeler.Count;
            if (toplamSay > 500) toplamSay = 500;
            for (int i = 0; i < toplamSay; i++)
            {
                TNS.IKY.Personel d = (TNS.IKY.Personel)list.objeler[i];
                if (i < toplamSay - 1) //sonraki varsa halen doldurulmamış olanı kopyala
                {
                    XLS.HucreKopyala(satir, sutun, satir + 12, sutun + 6, satir, sutun + 7);
                }

                // Eski: new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/app_themes/images/CBLogo.png"), ...)
                if (File.Exists(SirtlikLogoYolu))
                    XLS.ResimEkle(satir + 3, sutun + 3, 4, 1.87, new FileStream(SirtlikLogoYolu, FileMode.Open, FileAccess.Read), 0, 0, 2, 3);
                XLS.HucreDegerYaz(satir + 4, sutun + 3, GenelIslemlerIstemci.VarsayilanKurumBulAd() + "\n\n" + "İNSAN KAYNAKLARI\nBAŞKANLIĞI");
                if (d.resim != null)
                {
                    XLS.ResimEkle(satir + 5, sutun + 3, 4, 4.50, new MemoryStream(d.resim), 0, 0, 2, 3);
                }
                XLS.HucreDegerYaz(satir + 6, sutun + 3, d.adSoyad);
                XLS.HucreDegerYaz(satir + 7, sutun + 3, "SİCİL NO\n" + d.sicilNo);
                XLS.HucreDegerYaz(satir + 8, sutun + 3, "T.C. KİMLİK NO\n" + d.tcKimlikNo);
                if (i % 5 == 0 && i > 0)
                {
                    XLS.SayfaSonuKoySutun(sutun);
                }
                sutun = sutun + 7;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            AsposeLisans.Yukle();
            this.sonucDosyaAd = sonucDosyaAd;
            this.ciktiYeri = ciktiYeri;
            personelTurler = (string)raporParam["personelTur"];
            orgKod = (string)raporParam["ORGKOD"];
            if (ekBilgi.Equals("IK022") || ekBilgi.Equals("IK023") || ekBilgi.Equals("IK024") || ekBilgi.Equals("IK025") || ekBilgi.Equals("IK026") || ekBilgi.Equals("IK027") || ekBilgi.Equals("IK037") || ekBilgi.Equals("IK038") || ekBilgi.Equals("IK039") || ekBilgi.Equals("IK040"))
                return IstatistikYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK101") || ekBilgi.Equals("IK102"))
                return IstatistikYazdir2((TNS.KYM.Kullanici)kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK041"))
                return SirtlikYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK045"))
                return TanitmaKartiYazdirToplu(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK056"))
                return SorusturmaYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else
                return "";
        }

        private string SorusturmaYazdir(object kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            //ParamEkle("tarihSorgu", tarihSorgu) + ParamEkle("tarihSorguTur", tarihSorguTur.ToString())

            IIKYServis servisIKY;
            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            TNS.IKY.Personel p = new TNS.IKY.Personel();

            p.cezalarOlsun = 2;
            //p.resimOlsun = true;
            //p.kodlar = (string)raporParam["personelKod"];
            //p.personelTipleri = (string)raporParam["personelTur"];
            p.kopruKod = (string)raporParam["ORGKOD"];
            p.aktifDurum = (int)raporParam["aktiflik"];
            p.genelSorgu = (string)raporParam["sorgu"];

            p.aktifTarih = new TNSDateTime((string)raporParam["tarihSorgu"]);
            p.aktifTarihTur = (int)raporParam["tarihSorguTur"];

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri; // GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);
            int satir = 0, sutun = 1, sutun2 = 0;
            int kaynakSatir = 0, boyutSatir = 0;
            ObjectArray arr = new ObjectArray();
            XLS.HucreAdAdresCoz("FormatSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            ObjectArray list = servisIKY.PersonelListele((TNS.KYM.Kullanici)kullanan, p, false, false);

            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);
            int toplamSay = list.objeler.Count;
            for (int i = 0; i < toplamSay; i++)
            {
                TNS.IKY.Personel d = (TNS.IKY.Personel)list.objeler[i];
                if (d.cezaOduller != null && d.cezaOduller.ObjeSayisi > 0)
                {
                    for (int j = 0; j < d.cezaOduller.ObjeSayisi; j++)
                    {
                        TNS.IKY.PersonelCezaOdul pco = (TNS.IKY.PersonelCezaOdul)d.cezaOduller[j];
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                        XLS.HucreDegerYaz(satir, sutun + 0, i + 1);
                        XLS.HucreDegerYaz(satir, sutun + 1, d.sicilNo);
                        XLS.HucreDegerYaz(satir, sutun + 2, d.unvanAd);
                        XLS.HucreDegerYaz(satir, sutun + 3, d.adSoyad);
                        XLS.HucreDegerYaz(satir, sutun + 4, d.birimAd);
                        XLS.HucreDegerYaz(satir, sutun + 5, d.kadro.gorevBirimAd);
                        XLS.HucreDegerYaz(satir, sutun + 6, pco.tarih.Oku());
                        XLS.HucreDegerYaz(satir, sutun + 7, pco.cezaOdulTipiAdi);
                        XLS.HucreDegerYaz(satir, sutun + 8, pco.sebebi);
                        satir++;
                    }
                }
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string IstatistikYazdir2(Kullanici kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            IkiBoyutluTablo ib = new IkiBoyutluTablo();
            ib.ustBaslik1 = GenelIslemlerIstemci.VarsayilanKurumBulAd();
            ib.ustBaslik2 = new TNSDateTime(DateTime.Now).ToString() + " TARİHİ İTİBARİYLE " + RaporAdBul(ekBilgi);
            ib.kosegenYazi = "UNVANLAR";
            ib.sutunlarBaslik = "Personel Türleri";
            ib.hucreFormat = "#,##0;#,##0;#";

            ib.soldaAlanSayisi = 1;
            ib.ustteAlanSayisi = 2;
            int istKod = 1;
            if (ekBilgi == "IK101")
            {
                ib.soldaAlanSayisi = 1;
                ib.ustteAlanSayisi = 2;
                istKod = 1;
            }
            else if (ekBilgi == "IK102")
            {
                ib.soldaAlanSayisi = 1;
                ib.ustteAlanSayisi = 1;
                istKod = 2;
            }
            ObjectArray oa = TNS.IKY.Arac.Tanimla().PersonelIstatistikListele(kullanan, PersonelRaporlari.PersonelOl(raporParam), istKod);
            if (!oa.sonuc.islemSonuc)
            {
                throw new Exception(oa.sonuc.hataStr);
            }
            ib.veriler = oa.objeler;

            string uz = ciktiYeri;
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            string tut = OrtakClass.IkiBoyutluTablo.TabloUret(ib, uz, sonucDosyaAd);
            return tut;
        }

        private string RaporAdBul(string ekBilgi)
        {
            return "PERSONELİN UNVANA GÖRE SAYISAL DAĞILIMI";
        }

        public string personelTurler { get; set; }

        public string orgKod { get; set; }

        public string sonucDosyaAd { get; set; }

        public string ciktiYeri { get; set; }

        // Eski MaasIdasiHesaplamaCetveliYazdir / MaasIadesiBilgiYaz / AyGunBul / KesintiBilgiBul RaporAl'dan çağrılmadığı ve
        // Core'da cikti/kisisel/Istemci/InsanKaynaklari/PersonelOrtakRaporlar.cs içine zaten taşındığı için buraya alınmadı.

        /// <summary>
        /// Sırtlık listesindeki (IK041) logo dosyası (eski ~/app_themes/images/CBLogo.png). Uç katmanı açılışta
        /// Path.Combine(ortam.WebRootPath, "app_themes", "images", "CBLogo.png") gibi bir değer atayabilir. Dosya yoksa logo konmaz.
        /// </summary>
        public static string SirtlikLogoYolu { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "app_themes", "images", "CBLogo.png");

        /// <summary>Eski GenelIslemlerIstemci.AsposeAlanaResimEkle (byte[] ve Stream aşırı yüklemeleri birleştirildi; Core OrtakClass'ta yok).</summary>
        private static void AsposeAlanaResimEkle(Aspose.Words.DocumentBuilder builder, string alanAd, byte[] resim, Aspose.Words.Drawing.RelativeHorizontalPosition horPos, double sol, Aspose.Words.Drawing.RelativeVerticalPosition verPos, double ust, Aspose.Words.Drawing.WrapType bolunme, double w, double h)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(resim);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            double aspectRatioW = w / image.Width;
            double aspectRatioH = h / image.Height;
            double aspectRatio = Math.Min(aspectRatioW, aspectRatioH);
            if (aspectRatio > 0)
            {
                w = image.Width * aspectRatio;
                h = image.Height * aspectRatio;
            }

            while (builder.MoveToMergeField(alanAd))
            {
                Aspose.Words.Drawing.Shape eklenenResim = builder.InsertImage(image, horPos, sol, verPos, ust, w, h, bolunme);
            }
        }
    }
}
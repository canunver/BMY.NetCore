using System;
using System.Collections;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır hesap planı bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMHesapPlani
    {
        /// <summary>
        /// Verilen taşınır hesap planlarını teker teker varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen taşınır hesap planlarından
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesapPlani">Taşınır hesap planlarını tutan nesne</param>
        /// <param name="muhasebat">İşlemin muhasebat formatında excel dosyasından gelip gelmediği bilgisi</param>
        /// <param name="kaydetZorla">Hata olsa da kayıt işlemi yapılsın mı bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray hesapPlani, bool muhasebat, bool kaydetZorla)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            bool yetkili = TMMYetkiKontrol.Yoneticimi(kullanan);

            string yetkiTipiAdi = "Taşınır Hesap Kodu Değiştirme Yetkisi";
            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            if (!yetkili) yetkili = yetkiTipiDegeri == "1";

            if (!yetkili)
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            sonuc = KontrolEt(ve, kullanan, hesapPlani, "Yazma");

            if (!muhasebat)
            {
                //Üst kod kullanıldı uyarısı
                string hesapKod = ((HesapPlaniSatir)hesapPlani[0]).hesapKod;
                hesapKod.Replace(".", "");
                hesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, hesapKod);
                //int sonNokta = hesapKod.LastIndexOf(".");
                //if (sonNokta > 0)
                //    hesapKod = hesapKod.Remove(sonNokta);
                //hesapKod = hesapKod.Replace(".", "");
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.hesapKod = hesapKod;
                Sonuc sonucKullan = KullanilmisMi(hs);
                if (!sonucKullan.islemSonuc)
                {
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM120, OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, hesapKod.Replace(".", "")));
                    sonuc.hataStr += sonucKullan.bilgiStr + "<br>";
                    sonuc.islemSonuc = false;
                }
            }

            if (kaydetZorla)
            {
                sonuc.bilgiStr = "";
                sonuc.islemSonuc = true;
            }

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                TABHesapPlani to = new TABHesapPlani(ve);
                if (muhasebat)
                    sonuc = to.HesapKodKaydetMuhasebat(hesapPlani);
                else
                    sonuc = to.HesapKodKaydet(hesapPlani);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    if (!muhasebat)
                    {
                        //Üst kod kullanıldı uyarısı
                        string hesapKod = ((HesapPlaniSatir)hesapPlani[0]).hesapKod;
                        hesapKod.Replace(".", "");
                        hesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, hesapKod);
                        int sonNokta = hesapKod.LastIndexOf(".");
                        if (sonNokta > 0)
                            hesapKod = hesapKod.Remove(sonNokta);
                        hesapKod = hesapKod.Replace(".", "");
                        HesapPlaniSatir hs = new HesapPlaniSatir();
                        hs.hesapKod = hesapKod;
                        Sonuc sonucKullan = KullanilmisMi(hs);
                        if (!sonucKullan.islemSonuc)
                        {
                            sonuc.bilgiStr += string.Format(TasinirMalResI.TMM121, OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, hesapKod));
                            sonuc.bilgiStr += sonucKullan.bilgiStr + "<br>";
                        }
                    }

                    sonuc.bilgiStr += TasinirMalResI.TMM017;
                }
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Taşınır hesap planlarını teker teker silen yordam
        /// Eğer, taşınır işlem fişlerinde kullanılmışsa ve/veya detayında başka taşınır hesap
        /// planı varsa ya da kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesapPlani">Taşınır hesap planlarını tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Kullanici kullanan, ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM071, "");
            bool yetkili = TMMYetkiKontrol.Yoneticimi(kullanan);

            string yetkiTipiAdi = "Taşınır Hesap Kodu Değiştirme Yetkisi";
            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            if (!yetkili) yetkili = yetkiTipiDegeri == "1";

            if (!yetkili)
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            sonuc = KontrolEt(ve, kullanan, hesapPlani, "Silme");

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                TABHesapPlani to = new TABHesapPlani(ve);
                sonuc = to.HesapKodSil(hesapPlani);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = TasinirMalResI.TMM122;
                }
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap planlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesKod">Taşınır hesap planı bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap planları listesi döndürülür.</returns>
        public ObjectArray Listele(Kullanici kullanan, HesapPlaniSatir hesKod, Sayfalama sayfalama)
        {
            ObjectArray hVAN = new ObjectArray();
            string hata = "";

            if (hata != "")
            {
                hVAN.sonuc.hataStr = hata;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABHesapPlani to = new TABHesapPlani(ve);
            IDataReader r = to.HesapKodListele(hesKod, sayfalama);

            if (to.Hata() == "")
            {
                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABTASHesapPlani);

                int sonSira = (int)TABHesapPlani.ENUMAlanlar.VURGULA;
                while (r.Read())
                {
                    HesapPlaniSatir hKod = new HesapPlaniSatir();

                    hKod.hesapKod = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.KOD, "");
                    hKod.aciklama = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.ACIKLAMA, "");
                    hKod.seviye = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.SEVIYE, 0);
                    hKod.detay = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.DETAY, 0) == 0;
                    hKod.olcuBirim = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.OLCUBIRIMKOD, 0);
                    hKod.kdv = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.KDV, 0);
                    hKod.kullanilmiyor = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.KULLANILMIYOR, 0);
                    hKod.guncelleme = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.GUNCELLEME, 0);
                    hKod.numara = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.NUMARA, "");
                    hKod.amortiYil = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.AMORTIYIL, 0);
                    hKod.rfidEtiketKod = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.RFIDETIKETKOD, 0);
                    hKod.markaKod = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.MARKAKOD, 0);
                    hKod.modelKod = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.MODELKOD, 0);
                    hKod.kistAmortisman = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.KISTAMORTISMAN, 0);
                    hKod.vurgula = VeriErisim.DegerAl(r, (int)TABHesapPlani.ENUMAlanlar.VURGULA, 0);
                    hKod.olcuBirimAd = VeriErisim.DegerAl(r, sonSira + 1, "");

                    hKod.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hKod.hesapKod);

                    if (sayfalama.sayfaNo > 0)
                        hKod.toplamKayitSayisi = VeriErisim.DegerAl(r, sonSira + 2, 0);

                    hVAN.objeler.Add(hKod);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return hVAN;
        }

        /// <summary>
        /// Parametre olarak verilen taşınır hesap planlarını yetkiTipi parametresindeki
        /// değere göre bir takım kontrollerden geçiren yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesapPlani">Taşınır hesap planlarını tutan nesne</param>
        /// <param name="yetkiTipi">Yapılan işlemin türü</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private Sonuc KontrolEt(VeriErisim ve, Kullanici kullanan, ObjectArray hesapPlani, string yetkiTipi)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM123, "");
            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABTASHesapPlani);

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            if (hesapPlani != null && yetkiTipi == "Yazma")
            {
                foreach (HesapPlaniSatir hs in hesapPlani.objeler)
                {
                    if (hs.hesapKod == "")
                        hata += TasinirMalResI.TMM124 + "<br>";
                    if (hs.aciklama == "")
                        hata += hs.hesapKod + " - " + TasinirMalResI.TMM125 + "<br>";

                    //Excel den liste halinde kayıt yapıldığında anasıcocugundan dolayı kayıt yapılamıyor
                    //Kayıt esnasında bakmak lazım

                    if (!OrtakFonksiyonlar.KodYapiKontrol(t.Kirilimlar(0), hs.hesapKod.Replace(".", ""), false).islemSonuc)
                        hata += string.Format(TasinirMalResI.TMM126, t.Kirilimlar(0)) + "<br>";
                }
            }
            else if (hesapPlani != null && yetkiTipi == "Silme")
            {
                TABHesapPlani th = new TABHesapPlani(ve);
                foreach (HesapPlaniSatir hs in hesapPlani.objeler)
                {
                    if (th.HesapKodAnasiCocuguVarMi(hs.hesapKod, true))
                        hata += "<br>" + string.Format(TasinirMalResI.TMM127, hs.hesapKod);

                    Sonuc sonucKullanilmisMi = KullanilmisMi(hs);
                    if (!sonucKullanilmisMi.islemSonuc)
                        hata += "<br>" + sonucKullanilmisMi.hataStr;
                }
            }

            if (hata == "")
                sonuc.islemSonuc = true;

            sonuc.hataStr = hata;

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Taşınır hesap planlarının detay bilgilerini düzanleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        public void HesapPlaniDetaySayilariDuzenle(VeriErisim veriErisim)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));

            //ve.BeginTransaction();
            TABHesapPlani to = new TABHesapPlani(ve);
            to.HesapPlaniDetaySayilariDuzenle("");

            //ve.Commit();
            ve.VeriErisimKapat();
        }

        /// <summary>
        /// Parametre olarak verilen taşınır hesap planının taşınır işlem
        /// fişlerinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="hKod">Taşınır hesap planı bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private Sonuc KullanilmisMi(HesapPlaniSatir hKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));

            TABHesapPlani tabHesapPlani = new TABHesapPlani(ve);

            IDataReader dr = tabHesapPlani.KullanilmisMi(hKod);

            Sonuc sonuc = new Sonuc();

            bool kullanilmis = false;

            while (dr.Read())
            {
                sonuc.islemSonuc = false;
                if (sonuc.hataStr == "")
                    sonuc.hataStr = TasinirMalResI.TMM128 + "<br>";
                sonuc.hataStr += string.Format(TasinirMalResI.TMM129, VeriErisim.DegerAl(dr, 0, 0), VeriErisim.DegerAl(dr, 1, ""), VeriErisim.DegerAl(dr, 2, ""), VeriErisim.DegerAl(dr, 3, "")) + "<br>";
                sonuc.bilgiStr += string.Format(TasinirMalResI.TMM129, VeriErisim.DegerAl(dr, 0, 0), VeriErisim.DegerAl(dr, 1, ""), VeriErisim.DegerAl(dr, 2, ""), VeriErisim.DegerAl(dr, 3, "")) + "<br>";
                kullanilmis = true;
            }
            dr.Close();

            if (!kullanilmis)
            {
                sonuc.islemSonuc = true;
                sonuc.hataStr = "";
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KullanilmisMi(Kullanici kullanan, HesapPlaniSatir hKod)
        {
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABHesapPlani tabHesapPlani = new TABHesapPlani(ve);

            IDataReader dr = tabHesapPlani.KullanilmisMi(hKod);

            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = false;
            //bool kullanilmis = false;

            int sayi = 0;
            while (dr.Read())
            {
                if (sonuc.hataStr == "")
                    sonuc.hataStr = TasinirMalResI.TMM128 + "<br>";
                sonuc.hataStr += string.Format(TasinirMalResI.TMM129, VeriErisim.DegerAl(dr, 0, 0), VeriErisim.DegerAl(dr, 1, ""), VeriErisim.DegerAl(dr, 2, ""), VeriErisim.DegerAl(dr, 3, "")) + "<br>";
                sonuc.bilgiStr += string.Format(TasinirMalResI.TMM121, OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, hKod.hesapKod));
                sonuc.bilgiStr += string.Format(TasinirMalResI.TMM129, VeriErisim.DegerAl(dr, 0, 0), VeriErisim.DegerAl(dr, 1, ""), VeriErisim.DegerAl(dr, 2, ""), VeriErisim.DegerAl(dr, 3, "")) + "<br>";

                //kullanilmis = true;
                sonuc.islemSonuc = true;
                sayi++;
                if (sayi == 5)
                    break;
            }

            dr.Close();
            ve.VeriErisimKapat();

            return sonuc;
        }

        //public HesapPlaniSatir OzellikAl(HesapPlaniSatir hKod)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABHesapPlani to = new TABHesapPlani(ve);

        //    hKod = to.HesapKodOzellikAl(hKod);

        //    ve.VeriErisimKapat();

        //    return hKod;
        //}

        internal static Sonuc AmortismanSureKaydet(VeriErisim veriErisim, Kullanici kullanan, HesapPlaniSatir sure)
        {
            if (string.IsNullOrWhiteSpace(sure.hesapKod.Replace(".", "")))
                return new Sonuc("Lütfen hesap planı seçiniz");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));

            TABHesapPlani tabHesapPlani = new TABHesapPlani(ve);

            Sonuc s = tabHesapPlani.AmortismanSureKaydet(sure);
            ve.VeriErisimKapat();
            return s;
        }
    }
}

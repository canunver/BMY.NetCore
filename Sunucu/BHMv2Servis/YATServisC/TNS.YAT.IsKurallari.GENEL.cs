using System.Collections;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;
using TNS.DEG;
using System.Data;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Yatırım modülü iş kuralları sınıflarının ortak kullandığı yordamları tutan sınıf
    /// </summary>
    public class Genel
    {
        /// <summary>
        /// Gönderilen yıl ve bütçe kaynak türüne bakarak bütçe kaynağın bütçe dışı mı
        /// yoksa bütçede gösterilen mi olduğunu kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kaynakTur">Bütçe kaynağın türü</param>
        /// <returns>Bütçe kaynak bütçe dışı ise true, değilse false döndürülür.</returns>
        public static bool KaynakButceDisiMi(int yil, int kaynakTur)
        {
            if (yil >= YatirimEntegreYilBul() &&
                (kaynakTur == (int)ENUMButceKaynakTur.BUTCE || kaynakTur == (int)ENUMButceKaynakTur.KREDIBUTCEICI))
                return false;
            return true;
        }

        /// <summary>
        /// Yatırım ve bütçe hazırlık modüllerinin entegre edildiği yılı döndüren yordam
        /// Entegre yılı, Degisken.xml yapılandırma dosyasındaki YATIRIMENTEGREYIL değişkeninde tutuluyor.
        /// </summary>
        /// <returns>Entegre yılı döndürülür.</returns>
        public static int YatirimEntegreYilBul()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/YATIRIMENTEGREYIL"), 2011);
        }

        /// <summary>
        /// Bütçe hazırlık yılını döndüren yordam
        /// Bütçe hazırlık yılı, Degisken.xml yapılandırma dosyasındaki BUTCEHAZIRLIK değişkeninde tutuluyor.
        /// </summary>
        /// <returns>Bütçe hazırlık yılı döndürülür.</returns>
        public static int HazirlikYilBul()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/BUTCEHAZIRLIK"), System.DateTime.Now.Year + 1);
        }

        /// <summary>
        /// Sözlük tanımlamaların silme işlemleri sırasında çağırılan,
        /// gönderilen sözlük kodunun doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="kod">Sözlük kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SozlukSilKontrol(int kod)
        {
            Sonuc donusDeger = new Sonuc(true, "", "");

            if (kod == 0)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT038;
            }

            return donusDeger;
        }

        /// <summary>
        /// Sözlük tanımlamaların kaydet işlemleri sırasında çağırılan,
        /// gönderilen sözlük nesnesinin doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="sozluk">Sözlük bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SozlukKaydetKontrol(Sozluk sozluk)
        {
            Sonuc donusDeger = SozlukSilKontrol(sozluk.kod);

            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (sozluk.ad.Trim() == "")
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT010;
            }

            return donusDeger;
        }

        /// <summary>
        /// Kodu gönderilen sektör sözlük bilgisinin sistemde tanımlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="sektorKodu">Sektör sözlüğünün kodu</param>
        /// <returns>Sektör varsa true, yoksa false döndürülür.</returns>
        public static bool SektorVarMi(VeriErisim ve, int yil, string sektorKodu)
        {
            TABYATSektor tabSektor = new TABYATSektor(ve);
            return tabSektor.SektorVarMi(yil, sektorKodu);
        }

        /// <summary>
        /// Gönderilen sektör sözlük bilgisinin alt sektörünün olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="sektor">Sektör sözlük bilgisini tutan nesne</param>
        /// <returns>Sektörün, alt sektörü varsa true, yoksa false döndürülür.</returns>
        public static bool AltSektorVarMi(VeriErisim ve, int yil, SektorSozluk sektor)
        {
            TABYATSektor tabSektor = new TABYATSektor(ve);
            return tabSektor.AltSektorVarMi(yil, sektor);
        }

        /// <summary>
        /// Kodu gönderilen sektör sözlük bilgisinin yatırım projelerinde
        /// kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="sektorKodu">Sektör sözlüğünün kodu</param>
        /// <returns>Sektör; yatırım projelerinde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public static bool SektorKullanilmisMi(VeriErisim ve, string sektorKodu)
        {
            TABYATProje tabProje = new TABYATProje(ve);
            return tabProje.SektorKullanilmisMi(sektorKodu);
        }

        /// <summary>
        /// Kodu gönderilen proje tür sözlük bilgisinin sistemde tanımlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="turKodu">Proje türün kodu</param>
        /// <returns>Proje tür varsa true, yoksa false döndürülür.</returns>
        public static bool ProjeTurVarMi(VeriErisim ve, int turKodu)
        {
            TABYATProjeTur tabProjeTur = new TABYATProjeTur(ve);
            return tabProjeTur.ProjeTurVarMi(turKodu);
        }

        /// <summary>
        /// Kodu gönderilen proje tür sözlük bilgisinin yatırım projelerinde
        /// kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="projeTurKodu">Proje türün kodu</param>
        /// <returns>Proje tür; yatırım projelerinde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public static bool ProjeTurProjedeKullanilmisMi(VeriErisim ve, int projeTurKodu)
        {
            TABYATProje tabProje = new TABYATProje(ve);
            return tabProje.ProjeTurKullanilmisMi(projeTurKodu);
        }

        /// <summary>
        /// Kodu gönderilen proje tür sözlük bilgisinin yatırım alt projelerinde
        /// kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="projeTurKodu">Proje türün kodu</param>
        /// <returns>Proje tür; yatırım alt projelerinde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public static bool ProjeTurAltProjedeKullanilmisMi(VeriErisim ve, int projeTurKodu)
        {
            TABYATAltProje tabAltProje = new TABYATAltProje(ve);
            return tabAltProje.ProjeTurKullanilmisMi(projeTurKodu);
        }

        /// <summary>
        /// Kodu gönderilen projenin sistemde tanımlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <returns>Yatırım projesi varsa true, yoksa false döndürülür.</returns>
        public static bool ProjeVarMi(VeriErisim ve, int yil, string senaryoKod, int projeKodu)
        {
            TABYATProje tabProje = new TABYATProje(ve);
            return tabProje.ProjeVarMi(yil, senaryoKod, projeKodu);
        }

        /// <summary>
        /// Kodu gönderilen yatırım projesinin alt projelerde
        /// kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="projeKod">Yatırım projesinin programatik kodu</param>
        /// <returns>Yatırım projesi; alt projelerde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public static bool ProjeKullanilmisMi(VeriErisim ve, int yil, string senaryoKod, int projeKod)
        {
            TABYATAltProje tabAltProje = new TABYATAltProje(ve);
            return tabAltProje.ProjeKullanilmisMi(yil, senaryoKod, projeKod);
        }

        /// <summary>
        /// Kodu gönderilen yatırım alt projesinin sistemde tanımlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <returns>Yatırım alt projesi varsa true, yoksa false döndürülür.</returns>
        public static bool AltProjeVarMi(VeriErisim ve, int yil, string senaryoKod, int altProjeKodu)
        {
            TABYATAltProje tabAltProje = new TABYATAltProje(ve);
            return tabAltProje.AltProjeVarMi(yil, senaryoKod, altProjeKodu);
        }

        /// <summary>
        /// Kodu gönderilen kredi talebin sistemde tanımlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="krediTalepNo">Kredi talebin programatik kodu</param>
        /// <returns>Kredi talep varsa true, yoksa false döndürülür.</returns>
        public static bool KrediTalepVarMi(VeriErisim ve, int krediTalepNo)
        {
            TABYATKrediTalep tabKrediTalep = new TABYATKrediTalep(ve);
            return tabKrediTalep.KrediTalepVarMi(krediTalepNo);
        }

        /// <summary>
        /// Kodu gönderilen dokümanın sistemde tanımlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="siraNo">Dokümanın programatik kodu</param>
        /// <returns>Doküman varsa true, yoksa false döndürülür.</returns>
        public static bool DokumanVarMi(VeriErisim ve, string siraNo)
        {
            TABYATDokuman tabDokuman = new TABYATDokuman(ve);
            return tabDokuman.DokumanVarMi(siraNo);
        }

        /// <summary>
        /// Kodu gönderilen doküman tür sözlük bilgisinin yatırım alt projelerine
        /// eklenmiş doküman bilgilerinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="dokumanTurKod">Doküman türünün kodu</param>
        /// <returns>Doküman türü; dokümanlarda kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public static bool DokumanTurKullanilmisMi(VeriErisim ve, int dokumanTurKod)
        {
            TABYATDokuman tabDokuman = new TABYATDokuman(ve);
            return tabDokuman.DokumanTurKullanilmisMi(dokumanTurKod);
        }

        /// <summary>
        /// Kodu gönderilen bütçe kaynağın sistemde tanımlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kaynakNo">Bütçe kaynağın programatik kodu</param>
        /// <returns>Bütçe kaynak varsa true, yoksa false döndürülür.</returns>
        public static bool ButceKaynakVarMi(VeriErisim ve, int yil, string senaryoKod, int kaynakNo)
        {
            TABYATButceKaynak tabBK = new TABYATButceKaynak(ve);
            return tabBK.KaynakVarMi(yil, senaryoKod, kaynakNo);
        }

        /// <summary>
        /// Kodu gönderilen il sözlük bilgisinin ilçelerde
        /// kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="ilKodu">İl sözlüğünün kodu</param>
        /// <returns>İl; ilçelerde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public static bool IlKullanilmisMi(VeriErisim ve, int ilKodu)
        {
            TABYATIlce tabIlce = new TABYATIlce(ve);
            return tabIlce.IlKullanilmisMi(ilKodu);
        }

        /// <summary>
        /// Gönderilen ilçe sözlük bilgisinin yatırım projelerinde ve/veya
        /// yatırım alt projelerinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="ilce">İlçe sözlük bilgilerini tutan nesne</param>
        /// <returns>İlçe; yatırım projelerinde ve/veya yatırım alt projelerinde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public static bool IlceKullanilmisMi(VeriErisim ve, IlceSozluk ilce)
        {
            return true;
            //TABYATProje tabProje = new TABYATProje(ve);

            //if (tabProje.IlceKullanilmisMi(ilce))
            //    return true;

            //TABYATAltProje tabAltProje = new TABYATAltProje(ve);
            //return tabAltProje.IlceKullanilmisMi(ilce);
        }

        /// <summary>
        /// Sistem yılını döndüren yordam
        /// </summary>
        /// <returns>Sistem yılı</returns>
        public static int AktifYilDondur()
        {
            return System.DateTime.Today.Year;
        }

        //public static bool ButceKaynakTeklifVarMi(VeriErisim ve, ButceKaynakTeklif teklif)
        //{
        //    TABYATButceKaynakTeklif tabKT = new TABYATButceKaynakTeklif(ve);
        //    return tabKT.ButceKaynakTeklifVarMi(teklif);
        //}

        //public static bool ButceKaynakOdenekVarMi(VeriErisim ve, ButceKaynakOdenek odenek)
        //{
        //    TABYATButceKaynakOdenek tabKO = new TABYATButceKaynakOdenek(ve);
        //    return tabKO.ButceKaynakOdenekVarMi(odenek);
        //}

        //public static bool IlceVarMi(VeriErisim ve, IlceSozluk ilce)
        //{
        //    TABYATIlce tabIlce = new TABYATIlce(ve);
        //    return tabIlce.IlceVarMi(ilce);
        //}

        internal static ObjectArray TanimSozlukListele(VeriErisim veriErisim, Kullanici kullanan, int sozlukTur, int yil, string senaryoKod)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGenel tab = new TABGenel(ve);

            IDataReader dr = tab.TanimSozlukListele(sozlukTur, yil, senaryoKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    Sozluk s = new Sozluk();
                    s.kod = VeriErisim.DegerAl(dr, 0, 0);
                    s.ad = VeriErisim.DegerAl(dr, 1, "");
                    s.say = VeriErisim.DegerAl(dr, 2, 0);
                    oa.Ekle(s);
                }
                dr.Close();
            }
            else
                oa.sonuc.DegerAta(ve.hata);
            //if (oa.sonuc.islemSonuc)
            //{
            //    dr = tab.TanimSozlukDetaySayisListele(sozlukTur);
            //    if (dr != null)
            //    {
            //        while (dr.Read())
            //        {
            //            Sozluk s = new Sozluk();

            //            s.kod = VeriErisim.DegerAl(dr, 0, 0);
            //            int yer = oa.objeler.IndexOf(s);
            //            if (yer >= 0)
            //            {
            //                s = (Sozluk)oa.objeler[yer];
            //                s.say = VeriErisim.DegerAl(dr, 1, 0);
            //            }
            //        }
            //        dr.Close();
            //    }
            //    else
            //        oa.sonuc.DegerAta(ve.hata);
            //}
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc TanimSozlukEkle(VeriErisim veriErisim, Kullanici kullanan, int sozlukTur, Sozluk sozluk, bool kontrol)
        {
            Sonuc s = new Sonuc();
            if (kontrol)
            {
                if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                {
                    s.DegerAta("Yetkiniz yok");
                    return s;
                }
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGenel tab = new TABGenel(ve);

            ve.BeginTransaction();
            sozluk.kod = tab.SozlukSayacBul(sozlukTur);
            if (sozluk.kod > 0)
                s = tab.SozlukEkle(sozlukTur, sozluk);
            else
                s.DegerAta("Kod okunamadı!" + ve.hata);
            if (s.islemSonuc)
            {
                s.anahtar = sozluk.kod.ToString();
                ve.Commit();
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        internal static decimal DeflatorBul(VeriErisim veriErisim, Kullanici kullanan, int butceYil, string senaryoKod, string sektorKod)
        {
            if (string.IsNullOrWhiteSpace(sektorKod) || sektorKod.Length < 2) return 0;
            sektorKod = sektorKod.Substring(0, 2);
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGenel tab = new TABGenel(ve);
            decimal d = tab.DeflatorBul(butceYil, senaryoKod, sektorKod);
            ve.VeriErisimKapat();
            return d;
        }
    }
}
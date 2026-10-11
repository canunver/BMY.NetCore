using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Stratejik plan çalışmalarında yer alacak personel için
    /// personel-görev atamaları ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma, silme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class STRGorevlendirme
    {
        /// <summary>
        /// Personel-Görev ilişkisi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Gorevlendirme nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gorevler">Görevlendirme bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Gorevlendirme[] gorevler)
        {
            Sonuc sonuc = new Sonuc();

            if (gorevler == null || gorevler.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR035;
                return sonuc;
            }

            foreach (Gorevlendirme g in gorevler)
            {
                string hata = string.Empty;
                if (g.donem == null || g.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (g.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (g.ekipKod <= 0)
                    hata += PerformansButceResI.STR034 + "<br>";
                //if (string.IsNullOrEmpty(g.tcKimlikNo) || g.tcKimlikNo.Length != 11)
                //    hata += PerformansButceResI.STR036 + "<br>";
                if (string.IsNullOrEmpty(g.personelAd))
                    hata += PerformansButceResI.STR084 + "<br>";
                //if (string.IsNullOrEmpty(g.gorevAciklama))
                //    hata += PerformansButceResI.STR037;
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, gorevler[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGorevlendirme tGorevlendirme = new TABGorevlendirme(ve);
            ve.BeginTransaction();

            tGorevlendirme.Sil(gorevler[0]);
            int ekipKod = 0;
            int sayac = 0;
            foreach (Gorevlendirme g in gorevler)
            {
                if (ekipKod != g.ekipKod)
                {
                    ekipKod = g.ekipKod;
                    sayac = 0;
                }

                sayac++;
                g.siraNo = sayac;

                sonuc = tGorevlendirme.Kaydet(g);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Personel-Görev ilişkisini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Gorevlendirme nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gorev">Görevlendirme bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Gorevlendirme gorev)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (gorev.donem == null || gorev.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (gorev.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, gorev.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGorevlendirme tGorev = new TABGorevlendirme(ve);
            ve.BeginTransaction();

            sonuc = tGorev.Sil(gorev);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.STR014;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.STR015 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan görevlendirme ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gorev">Görevlendirme bilgisini tutan nesne</param>
        /// <returns>Bir görevlendirme için görevlendirme ayrıntılarını döndürür</returns>
        public static Gorevlendirme[] Listele(VeriErisim veriErisim, Kullanici kullanan, Gorevlendirme gorev, bool sorgu)
        {
            TNSCollection gorevler = new TNSCollection();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGorevlendirme tGorev = new TABGorevlendirme(ve);

            IDataReader dr =  tGorev.Listele(gorev, sorgu);
            if (dr != null)
            {
                while (dr.Read())
                {
                    Gorevlendirme gelen = new Gorevlendirme();
                    gelen.donem = new Donem();
                    gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABGorevlendirme.ENUMAlanlar.DONEMNO, 0);
                    gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABGorevlendirme.ENUMAlanlar.ORGBIRIMKOD, 0);
                    gelen.ekipKod = VeriErisim.DegerAl(dr, (int)TABGorevlendirme.ENUMAlanlar.EKIPKOD, 0);
                    gelen.siraNo = VeriErisim.DegerAl(dr, (int)TABGorevlendirme.ENUMAlanlar.SIRANO, 0);
                    gelen.personelAd = VeriErisim.DegerAl(dr, (int)TABGorevlendirme.ENUMAlanlar.PERSONELAD, string.Empty);
                    gelen.gorevAciklama = VeriErisim.DegerAl(dr, (int)TABGorevlendirme.ENUMAlanlar.GOREVACIKLAMA, string.Empty);
                    gelen.gorevYer = VeriErisim.DegerAl(dr, (int)TABGorevlendirme.ENUMAlanlar.GOREVYER, string.Empty);
                    gelen.tel = VeriErisim.DegerAl(dr, (int)TABGorevlendirme.ENUMAlanlar.TEL, string.Empty);
                    gelen.eMail = VeriErisim.DegerAl(dr, (int)TABGorevlendirme.ENUMAlanlar.EMAIL, string.Empty);

                    gorevler.Add(gelen);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return (Gorevlendirme[])gorevler.ToArray(typeof(Gorevlendirme));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.YAT;
using TNS.YAT.IsKurallari.Veri;
using System.Data;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Hizmet alımı suretiyle kullanılan/kullanılacak taşıtlara ilişkin ödenek teklif ve
    /// taşıt talep bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class TasitBilgi
    {
        /// <summary>
        /// Verilen yıla uygun olan Hizmet alımı suretiyle kullanılan/kullanılacak
        /// taşıtlara ilişkin ödenek teklif bilgilerini döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasitAlimBedeliBilgisi nesneleri döndürülür.</returns>
        public static ObjectArray TasitAlimBedeliListele(VeriErisim veriErisim, int yil)
        {
            ObjectArray donusDeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(null));
            TABYATTasitBedel tabTasitBedel = new TABYATTasitBedel(ve);
            IDataReader dr = tabTasitBedel.Listele(yil);
            if (dr != null)
            {
                TasitAlimBedeliBilgisi tabb = null;
                while (dr.Read())
                {
                    string tasitSiraNo = VeriErisim.DegerAl(dr, 1, "");
                    if (tabb == null || tabb.tasitSiraNo != tasitSiraNo)
                    {
                        if (tabb != null)
                            donusDeger.Ekle(tabb);
                        tabb = new TasitAlimBedeliBilgisi();
                        tabb.tasitSiraNo = tasitSiraNo;
                        tabb.yillar.Add((double)0);
                        tabb.yillar.Add((double)0);
                        tabb.yillar.Add((double)0);
                    }
                    int teklifYil = VeriErisim.DegerAl(dr, 2, 0);
                    int indis = teklifYil - yil;
                    if (indis >= 0 && indis <= 2)
                        tabb.yillar[indis] = VeriErisim.DegerAl(dr, 3, (double)0);
                }
                if (tabb != null)
                    donusDeger.Ekle(tabb);
            }
            else
                donusDeger.sonuc.DegerAta(string.Format(YatirimResI.YAT063, ve.hata));
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Hizmet alımı suretiyle kullanılan/kullanılacak
        /// taşıtlara ilişkin ödenek teklif bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl bilgisi</param>
        /// <param name="tasitAlimBedelleri">TasitAlimBedeliBilgisi nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc TasitAlimBedeliKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, TNSCollection tasitAlimBedelleri)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
                TABYATTasitBedel tabTasitBedel = new TABYATTasitBedel(ve);

                ve.BeginTransaction();
                for (int i = 0; sonuc.islemSonuc && i < tasitAlimBedelleri.Count; i++)
                {
                    TasitAlimBedeliBilgisi tabb = (TasitAlimBedeliBilgisi)tasitAlimBedelleri[i];
                    sonuc = tabTasitBedel.Sil(yil, tabb.tasitSiraNo);

                    for (int j = 0; sonuc.islemSonuc && j < tabb.yillar.Count && j < 3; j++)
                    {
                        double bedel = (double)tabb.yillar[j];
                        if (bedel != 0)
                            sonuc = tabTasitBedel.Kaydet(yil, tabb.tasitSiraNo, yil + j, bedel);
                    }
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }
            else
                sonuc.DegerAta(YatirimResI.YAT064);
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşıt talep bilgilerini döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurumKod">Kurum kriteri</param>
        /// <param name="sektorKod">Sektör kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasitTalepBilgi nesneleri döndürülür.</returns>
        public static ObjectArray TasitListesiBul(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumKod, string sektorKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATTasitListesi tabTasitListesi = new TABYATTasitListesi(ve);
            IDataReader dr = tabTasitListesi.Listele(yil, kurumKod, sektorKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TasitTalepBilgi tb = new TasitTalepBilgi();
                    tb.tasitSiraNo = VeriErisim.DegerAl(dr, 4, "");
                    tb.mevcutAdet = VeriErisim.DegerAl(dr, 5, 0);
                    tb.mevcutModel = VeriErisim.DegerAl(dr, 6, "");
                    tb.talepAdet = VeriErisim.DegerAl(dr, 7, 0);
                    tb.talepAciklama = VeriErisim.DegerAl(dr, 8, "");
                    donusDeger.Ekle(tb);
                }
            }
            else
                donusDeger.sonuc.DegerAta(string.Format(YatirimResI.YAT063, ve.hata));
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Taşıt talep bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl bilgisi</param>
        /// <param name="kurumKod">Kurum bilgisi</param>
        /// <param name="sektorKod">Sektör bilgisi</param>
        /// <param name="tasitListesi">TasitTalepBilgi nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc TasitListesiKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumKod, string sektorKod, TNSCollection tasitListesi)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATTasitListesi tabTasitListesi = new TABYATTasitListesi(ve);

            ve.BeginTransaction();
            sonuc = tabTasitListesi.Sil(yil, kurumKod, sektorKod);
            for (int i = 0; sonuc.islemSonuc && i < tasitListesi.Count; i++)
            {
                TasitTalepBilgi tb = (TasitTalepBilgi)tasitListesi[i];
                sonuc = tabTasitListesi.Kaydet(yil, kurumKod, sektorKod, i, tb.tasitSiraNo, tb.mevcutAdet, tb.mevcutModel, tb.talepAdet, tb.talepAciklama);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Kredi talep bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class KrediTalepClass
    {
        /// <summary>
        /// Kredi talep varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen kredi talep nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="krediTalep">Kredi talep bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, KrediTalep krediTalep)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            if (krediTalep.altProjeKodu <= 0)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT035;
                return donusDeger;
            }

            if (krediTalep.yil <= 0)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT032;
                return donusDeger;
            }

            if (string.IsNullOrEmpty(krediTalep.talepAd))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT045;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATKrediTalep tabKrediTalep = new TABYATKrediTalep(ve);
            ve.BeginTransaction();

            if (!Genel.AltProjeVarMi(ve, yil, senaryoKod, krediTalep.altProjeKodu))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT022;
            }

            if (krediTalep.krediNo != 0)
            {
                if (!Genel.KrediTalepVarMi(ve, krediTalep.krediNo))
                {
                    donusDeger.islemSonuc = false;
                    donusDeger.hataStr = YatirimResI.YAT046;
                }
            }

            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            try
            {
                if (krediTalep.krediNo == 0)
                    donusDeger = tabKrediTalep.Kaydet(krediTalep);
                else
                    donusDeger = tabKrediTalep.Guncelle(krediTalep);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kredi talepleri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="krediNo">Kredi talebin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kredi talepler döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int altProjeKodu, int krediNo)
        {
            return Listele(veriErisim, kullanan, altProjeKodu, krediNo, 0);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kredi talepleri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kredi talepler döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim veriErisim, int altProjeKodu, Kullanici kullanan, int yil)
        {
            return Listele(veriErisim, kullanan, altProjeKodu, 0, yil);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kredi talepleri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="krediNo">Kredi talebin programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kredi talepler döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int altProjeKodu, int krediNo, int yil)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATKrediTalep tabKrediTalep = new TABYATKrediTalep(ve);
            IDataReader dr = null;

            try
            {
                dr = tabKrediTalep.Listele(altProjeKodu, krediNo, yil);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                KrediTalep gelen = new KrediTalep();
                gelen.altProjeKodu = VeriErisim.DegerAl(dr, 0, -1);
                gelen.krediNo = VeriErisim.DegerAl(dr, 1, -1);
                gelen.yil = VeriErisim.DegerAl(dr, 2, 0);
                gelen.amac= VeriErisim.DegerAl(dr, 3, "");
                gelen.krediIhtiyaci = VeriErisim.DegerAl(dr, 4, 0.0);
                gelen.teminEdilen = VeriErisim.DegerAl(dr, 5, 0.0);
                gelen.talepEdilen = VeriErisim.DegerAl(dr, 6, 0.0);
                gelen.alternatif = VeriErisim.DegerAl(dr, 7, "");
                gelen.dusunceler = VeriErisim.DegerAl(dr, 8, "");
                gelen.talepAd = VeriErisim.DegerAl(dr, 9, "");

                donusDeger.liste.Add(gelen);
            }

            if (donusDeger.liste.Count == 0)
                donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kredi talebin yatırım alt projesinin programatik kodunu bulan yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="krediTalepNo">Kredi talebin programatik kodu</param>
        /// <returns>Yatırım alt projesinin programatik kodu</returns>
        public static int AltProjeKoduBul(VeriErisim veriErisim, int krediTalepNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(null));
            TABYATKrediTalep tabKrediTalep = new TABYATKrediTalep(ve);
            return tabKrediTalep.AltProjeKoduBul(krediTalepNo);
        }
    }
}
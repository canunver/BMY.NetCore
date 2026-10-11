using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.STR.IsKurallari.Veri;

namespace TNS.STR.IsKurallari
{
    /// <summary>
    /// Vizyonla ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class STRVizyon
    {
        /// <summary>
        /// Vizyon varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Vizyon nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Vizyon vizyon)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (vizyon.donemBYil <= 0)
                hata = StratejikPlanResI.STR003 + "<br>";
            if (vizyon.dil.kod <= 0)
                hata += StratejikPlanResI.STR004 + "<br>";
            if (string.IsNullOrEmpty(vizyon.vizyon))
                hata += StratejikPlanResI.STR030 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABVizyon tVizyon = new TABVizyon(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();

            Misyon misyon = new Misyon(vizyon.donemBYil, vizyon.dil, "", "", "");
            if (!STRGenel.MisyonVarMi(ve, misyon))
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = StratejikPlanResI.STR031 + "<br>";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (!tVizyon.VizyonVarMi(vizyon.donemBYil))
                sonuc = tVizyon.Kaydet(vizyon);

            if (sonuc.islemSonuc)
            {
                StratejikYazi yazi = new StratejikYazi(vizyon);
                if (tYazi.YaziVarMi(yazi))
                    sonuc = tYazi.Guncelle(yazi);
                else
                    sonuc = tYazi.Kaydet(yazi);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = StratejikPlanResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = StratejikPlanResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan vizyon bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında vizyon nesnesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Vizyon vizyon)
        {
            ObjectArray vizyonVAN = new ObjectArray();
            vizyonVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!vizyonVAN.sonuc.islemSonuc)
                return vizyonVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABVizyon tVizyon = new TABVizyon(ve);

            IDataReader dr = tVizyon.Listele(vizyon);
            while (dr.Read())
            {
                Vizyon gelen = new Vizyon();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.dil.kod = VeriErisim.DegerAl(dr, 3, 0);
                gelen.dil.ad = STRServis.UzayYaziDegerGetir("STRDIL", gelen.dil.kod.ToString(), true, "");
                gelen.vizyon = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.dosya = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);

                vizyonVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (vizyonVAN.objeler.Count == 0)
                vizyonVAN.sonuc.bilgiStr = StratejikPlanResI.STR012;

            ve.VeriErisimKapat();
            return vizyonVAN;
        }
    }
}
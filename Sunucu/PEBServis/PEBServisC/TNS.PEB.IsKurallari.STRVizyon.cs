using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
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
            Sonuc sonuc = YetkiSTR.OGYHakkiVarMi(kullanan, vizyon.kurumKod, BHMv2.ENUMOGY.Yazma);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (vizyon.donemBYil <= 0)
                hata = PerformansButceResI.STR003 + "<br>";
            if (vizyon.dil.kod <= 0)
                hata += PerformansButceResI.STR004 + "<br>";
            if (string.IsNullOrEmpty(vizyon.vizyon))
                hata += PerformansButceResI.STR030 + "<br>";
            if (string.IsNullOrEmpty(vizyon.kurumKod))
                hata += "Kurumsal kod girilmelidir!" + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVizyon tVizyon = new TABVizyon(ve);
            TABTema tTema = new TABTema(ve);
            TABYazi tYazi = new TABYazi(ve);
            if (!STRGenel.MisyonVarMi(ve, vizyon.donemBYil, vizyon.dil.kod, vizyon.kurumKod))
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = PerformansButceResI.STR031 + "<br>";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve.BeginTransaction();
            if (!tVizyon.VizyonVarMi(vizyon.kurumKod, vizyon.donemBYil))
                sonuc = tVizyon.Kaydet(vizyon);
            Tema tema = new Tema(vizyon.donemBYil, 1, "Tema", "Tema", "Tema", "", vizyon.dil, vizyon.kurumKod);
            if (!tTema.TemaVarMi(vizyon.kurumKod, vizyon.donemBYil, 1))
                sonuc = tTema.Kaydet(tema);

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
                StratejikYazi yazi = new StratejikYazi(tema);
                if (!tYazi.YaziVarMi(yazi))
                    sonuc = tYazi.Kaydet(yazi);
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
            vizyonVAN.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!vizyonVAN.sonuc.islemSonuc)
                return vizyonVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVizyon tVizyon = new TABVizyon(ve);

            IDataReader dr = tVizyon.Listele(vizyon);
            while (dr.Read())
            {
                Vizyon gelen = new Vizyon();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.dil.kod = VeriErisim.DegerAl(dr, 3, 0);
                gelen.dil.ad = PEBServis.UzayYaziDegerGetir("STRDIL", gelen.dil.kod.ToString(), true, "");
                gelen.vizyon = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.dosya = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.kurumKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, 8, string.Empty));

                vizyonVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (vizyonVAN.objeler.Count == 0)
                vizyonVAN.sonuc.bilgiStr = PerformansButceResI.STR012;

            ve.VeriErisimKapat();
            return vizyonVAN;
        }
    }
}
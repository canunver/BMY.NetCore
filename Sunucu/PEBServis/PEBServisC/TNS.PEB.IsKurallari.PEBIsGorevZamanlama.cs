using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// İş/göreve ait zaman planı bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBIsGorevZamanlama
    {
        /// <summary>
        /// İş/göreve ait zaman planı bilgilerini kaydeden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig)
        {
            Sonuc sonuc = new Sonuc();
            Sonuc sonucTarihce = new Sonuc();
            ENUMTARIHCEISLEMTIP islemTip = new ENUMTARIHCEISLEMTIP();
            object eski = null, yeni = null;
            Sonuc silSonuc = new Sonuc();


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsGorevZamanlama tZamanlama = new TABIsGorevZamanlama(ve);
            ve.BeginTransaction();
            ObjectArray igz = null;
            igz =Listele(ve,kullanan, ig);
            silSonuc= tZamanlama.Sil(ig);
            if(silSonuc.islemSonuc)
            {
                foreach (IsGorevZamanlama zamanlama in igz.objeler)
                {
                    islemTip = ENUMTARIHCEISLEMTIP.SIL;
                    if (yeni == null)
                        yeni = new IsGorevZamanlama();
                    eski = zamanlama;
                    sonucTarihce = STROperasyonelPlanTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.ISGOREVZAMANLAMA, islemTip,ig);
                }
            }
            int sayac = 0;
            foreach (IsGorevZamanlama zamanlama in ig.zamanlamalar)
            {
                if (!(zamanlama.basTarih.isNull
                    && zamanlama.bitTarih.isNull
                    && string.IsNullOrEmpty(zamanlama.basAciklama)
                    && string.IsNullOrEmpty(zamanlama.bitAciklama)
                    && string.IsNullOrEmpty(zamanlama.sure)
                    && string.IsNullOrEmpty(zamanlama.yer)))
                {
                    sayac++;
                    zamanlama.siraNo = sayac;
                    sonuc = tZamanlama.Kaydet(ig, zamanlama);
                    if (!sonuc.islemSonuc)
                        break;
                    else
                    {
                        if (eski == null)
                            eski = new IsGorevZamanlama();
                        yeni = zamanlama;
                        islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                        sonucTarihce = STROperasyonelPlanTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.ISGOREVZAMANLAMA, islemTip,ig);
                    }
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen iş/görevin zaman planı bilgilerini listeleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında IsGorevZamanlama nesneleri listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig)
        {
            ObjectArray zamanlamaVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsGorevZamanlama tZamanlama = new TABIsGorevZamanlama(ve);

            IDataReader dr = tZamanlama.Listele(ig);
            while (dr.Read())
            {
                IsGorevZamanlama gelen = new IsGorevZamanlama();
                gelen.siraNo = VeriErisim.DegerAl(dr, 3, 0);
                gelen.basTarih = VeriErisim.DegerAl(dr, 4);
                gelen.bitTarih = VeriErisim.DegerAl(dr, 5);
                gelen.basAciklama = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.bitAciklama = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.sure = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.yer = VeriErisim.DegerAl(dr, 9, string.Empty);
                zamanlamaVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (zamanlamaVAN.objeler.Count == 0)
                zamanlamaVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return zamanlamaVAN;
        }
    }
}
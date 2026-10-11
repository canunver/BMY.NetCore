using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// İş/göreve ait performans göstergeleri, hedeflenen ve gerçekleşen değerleri
    /// bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBIsGorevGosterge
    {
        /// <summary>
        /// İş/göreve ait performans göstergelerini, hedeflenen ve gerçekleşen değerlerini kaydeden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig)
        {
            Sonuc sonuc = new Sonuc();
            Sonuc sonucTarihce = new Sonuc();
            object eski = null, yeni = null;
            ENUMTARIHCEISLEMTIP islemTip = new ENUMTARIHCEISLEMTIP();
            ObjectArray igs = null;
            Sonuc silSonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsGorevGosterge tGosterge = new TABIsGorevGosterge(ve);
            ve.BeginTransaction();
            igs = Listele(ve, kullanan, ig);
            silSonuc=tGosterge.Sil(ig);
            if(silSonuc.islemSonuc)
            {
                islemTip = ENUMTARIHCEISLEMTIP.SIL;
                foreach (IsGorevGosterge sorumlu in igs.objeler)
                {
                    eski = sorumlu;
                    if (yeni == null)
                        yeni = new IsGorevGosterge();
                    sonucTarihce = STROperasyonelPlanTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.ISGOREVPERFORMANSGOSTERGE, islemTip, ig);
                }
            }
            int sayac = 0;
            foreach (IsGorevGosterge gosterge in ig.gostergeler)
            {
                if (!PEBGenel.GostergeVarMi(ve, gosterge.gosterge.gostergeKod, (int)ENUMGostergeKullanimSekli.OperasyonelPlan))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(PerformansButceResI.PEB082, gosterge.gosterge.gostergeKod.ToString());
                    break;
                }

                //Gerçekleşen girilmiş mi kontrolü
                bool gercekVarmi = false;
                for (int i = 0; i < gosterge.gerceklesen.Length; i++)
                {
                    if (gosterge.gerceklesen[i] > 0)
                    {
                        gercekVarmi = true;
                        break;
                    }
                }

                if (gosterge.hedeflenen > 0 || gercekVarmi)
                {
                    sayac++;
                    gosterge.siraNo = sayac;
                    sonuc = tGosterge.Kaydet(ig, gosterge);
                    if (!sonuc.islemSonuc)
                        break;
                    else
                    {

                        islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                        if (eski == null)
                            eski = new IsGorevGosterge();
                        yeni = gosterge;
                        sonucTarihce = STROperasyonelPlanTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.ISGOREVPERFORMANSGOSTERGE, islemTip, ig);
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
        /// Verilen iş/görevin performans göstergelerini, hedeflenen ve gerçekleşen değerlerini listeleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında IsGorevGosterge nesneleri listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig)
        {
            ObjectArray gostergeVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsGorevGosterge tGosterge = new TABIsGorevGosterge(ve);

            IDataReader dr = tGosterge.Listele(ig);
            while (dr.Read())
            {
                IsGorevGosterge gelen = new IsGorevGosterge();
                gelen.siraNo = VeriErisim.DegerAl(dr, 3, 0);
                gelen.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 4, 0);
                gelen.hedeflenen = VeriErisim.DegerAl(dr, 5, (double)0);
                for (int i = 0; i < gelen.gerceklesen.Length; i++)
                    gelen.gerceklesen[i] = VeriErisim.DegerAl(dr, 6 + i, (double)0);
                gelen.aciklama = VeriErisim.DegerAl(dr, 6 + gelen.gerceklesen.Length, string.Empty);
                gelen.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 7 + gelen.gerceklesen.Length, string.Empty);
                gelen.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 8 + gelen.gerceklesen.Length, string.Empty);

                gostergeVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (gostergeVAN.objeler.Count == 0)
                gostergeVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return gostergeVAN;
        }
    }
}
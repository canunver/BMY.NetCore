using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// İş/göreve ait sorumlu personel bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBIsGorevSorumlu
    {
        /// <summary>
        /// İş/göreve ait sorumlu personel bilgilerini kaydeden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig)
        {
            Sonuc sonuc = new Sonuc();
            Sonuc silSonuc = new Sonuc();
            Sonuc sonucTarihce = new Sonuc();
            ENUMTARIHCEISLEMTIP islemTip = new ENUMTARIHCEISLEMTIP();
            ObjectArray igs = null;
            object eski = null, yeni = null;
            
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsGorevSorumlu tSorumlu = new TABIsGorevSorumlu(ve);
            ve.BeginTransaction();
            igs = Listele(ve, kullanan, ig);
            silSonuc=tSorumlu.Sil(ig);
            if(silSonuc.islemSonuc)
            {
                islemTip = ENUMTARIHCEISLEMTIP.SIL;
                foreach(IsGorevSorumlu sorumlu in igs.objeler)
                {
                    eski = sorumlu;
                    if (yeni == null)
                        yeni = new IsGorevSorumlu();
                    sonucTarihce = STROperasyonelPlanTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.ISGOREVSORUMLULAR, islemTip, ig);
                }
            }
            int sayac = 0;
            foreach (IsGorevSorumlu sorumlu in ig.sorumlular)
            {
                //if (!PEBGenel.PersonelVarMiView(ve, sorumlu.tcKimlikNo))
                //{
                //    sonuc.islemSonuc = false;
                //    sonuc.hataStr = string.Format(PerformansButceResI.PEB083, sorumlu.tcKimlikNo);
                //    break;
                //}

                sayac++;
                sorumlu.siraNo = sayac;
                sonuc = tSorumlu.Kaydet(ig, sorumlu);
                if (!sonuc.islemSonuc)
                    break;
                else
                {
                    islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                    if (eski == null)
                        eski = new IsGorevSorumlu();
                    yeni = sorumlu;
                    sonucTarihce = STROperasyonelPlanTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.ISGOREVSORUMLULAR, islemTip, ig);

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
        /// Verilen iş/görevin sorumlu personel bilgilerini listeleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında IsGorevSorumlu nesneleri listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig)
        {
            ObjectArray sorumluVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsGorevSorumlu tSorumlu = new TABIsGorevSorumlu(ve);

            IDataReader dr = tSorumlu.Listele(ig);
            while (dr.Read())
            {
                IsGorevSorumlu gelen = new IsGorevSorumlu();
                gelen.siraNo = VeriErisim.DegerAl(dr, 3, 0);
                gelen.tcKimlikNo = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.gorev = VeriErisim.DegerAl(dr, 5, string.Empty);

                if (VeriErisim.DegerAl(dr, 6, 0) == 1)
                    gelen.sorumlu = true;
                else
                    gelen.sorumlu = false;
                
                gelen.ad = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.soyad = VeriErisim.DegerAl(dr, 8, string.Empty);

                sorumluVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (sorumluVAN.objeler.Count == 0)
                sorumluVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return sorumluVAN;
        }
    }
}
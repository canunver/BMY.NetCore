using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{

    /// <summary>
    /// Sözleşme sonucunda ödenmesi gereken ücret bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleSozlesmeSonuUcret
    {
        /// <summary>
        /// Arguman olarak verilen, sözleşme sonucunda ödenmesi gereken ücret bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (liste.objeler.Count == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                IhaleSozlesmeSonuUcret ucret = (IhaleSozlesmeSonuUcret)liste.objeler[0];
                if (ucret.yil == 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00172 + "<br>";
                if (ucret.muhasebe == 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00173 + "<br>";
                if (ucret.birim.Trim() == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00174 + "<br>";
                if (ucret.surecNo == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00175 + "<br>";

                if (hata != "")
                {
                    
                }
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            IhaleSozlesmeSonuUcret u = ((IhaleSozlesmeSonuUcret)liste.objeler[0]);

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, u.yil, u.surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleSozlesmeSonuUcret to = new TABIhaleSozlesmeSonuUcret(ve);
            ve.BeginTransaction();


            sonuc = to.Sil((IhaleSozlesmeSonuUcret)liste.objeler[0]);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşme sonucunda ödnemesi gereken ücret bilgisini veritabanından okur 
        /// ve IhaleSozlesmeSonuUcret nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşme sonucunda ödnemesi gereken ücret bilgisi listesini IhaleSozlesmeSonuUcret nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, IhaleSozlesmeSonuUcret kriter)
        {
            ObjectArray donusDegeri = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleSozlesmeSonuUcret to = new TABIhaleSozlesmeSonuUcret(ve);
            IDataReader r = to.Listele(kriter);
            IhaleSozlesmeSonuUcret ekleObj;
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ekleObj = new IhaleSozlesmeSonuUcret();
                    ekleObj.yil = VeriErisim.DegerAl(r, 0, 0);
                    ekleObj.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    ekleObj.birim = VeriErisim.DegerAl(r, 2, "");
                    ekleObj.surecNo = VeriErisim.DegerAl(r, 3, "");
                    ekleObj.siraNo = VeriErisim.DegerAl(r, 4, 0);
                    ekleObj.tutar = VeriErisim.DegerAl(r, 5, 0.0);
                    ekleObj.belgeTarihi = VeriErisim.DegerAl(r, 6);
                    ekleObj.belgeNo = VeriErisim.DegerAl(r, 7, "");
                    donusDegeri.objeler.Add(ekleObj);
                }
            }
            r.Close();


            ve.VeriErisimKapat();

            return donusDegeri;
        }
    }

}

using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Dönem teklif bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifDonem
    {
        /// <summary>
        /// Arguman olarak verilen, dönem teklif bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifDonem nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
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
            int satir = 0;
            foreach (TeklifDonem d in liste.objeler)
            {
                satir++;
                if (d.tarih.isNull)
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00275, satir) + "<br>";
            }

            if (liste.objeler.Count == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00007;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = ((TeklifDonem)liste.objeler[0]).yil;
            string surecNo = ((TeklifDonem)liste.objeler[0]).surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = true;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifDonem to = new TABTeklifDonem(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(liste);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

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
        /// Arguman olarak verilen kriterlere bağlı olarak, dönem teklif bilgisini veritabanından okur 
        /// ve TeklifDonem nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, dönem teklif listesini TeklifDonem nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TeklifDonem kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifDonem to = new TABTeklifDonem(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TeklifDonem d = new TeklifDonem();

                    d.yil = VeriErisim.DegerAl(r, 0, 0);
                    d.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    d.birim = VeriErisim.DegerAl(r, 2, "");
                    d.surecNo = VeriErisim.DegerAl(r, 3, "");
                    d.donemNo = VeriErisim.DegerAl(r, 4, 0);
                    d.tarih = VeriErisim.DegerAl(r, 5);

                    bilgi.objeler.Add(d);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}

using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Kiralama bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCKiralama
    {
        /// <summary>
        /// Arguman olarak verilen, kiralama bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Kiralama form, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan, form.muhasebe, form.birim))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hataStr = "";
            if (form.yil == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (form.surecNo == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00067 + "<br>";

            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = form.yil;
            string surecNo = form.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKiralama to = new TABKiralama(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
            {
                //Süreç devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().SurecDurumKaydet(null, kullanan, yil, surecNo, ENUMSurecDurum.DEVAMEDIYOR);
                ///////////////////////////////////////////////////////////////////////////

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
        /// Arguman olarak verilen kriterlere bağlı olarak, kiralama bilgisini veritabanından okur 
        /// ve Kiralama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kiralama listesini Kiralama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Kiralama kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABKiralama to = new TABKiralama(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Kiralama k = new Kiralama();

                    k.yil = VeriErisim.DegerAl(r, 0, 0);
                    k.surecNo = VeriErisim.DegerAl(r, 1, "");
                    k.siraNo = VeriErisim.DegerAl(r, 2, 0);
                    k.tur = VeriErisim.DegerAl(r, 3, 0);

                    //Araç Kiralama
                    k.Arac.marka = VeriErisim.DegerAl(r, 4, "");
                    k.Arac.tip = VeriErisim.DegerAl(r, 5, "");
                    k.Arac.model = VeriErisim.DegerAl(r, 6, "");
                    k.Arac.modelYili = VeriErisim.DegerAl(r, 7, 0);
                    k.Arac.cinsi = VeriErisim.DegerAl(r, 8, "");
                    k.Arac.plaka = VeriErisim.DegerAl(r, 9, "");
                    k.Arac.mensei = VeriErisim.DegerAl(r, 10, "");
                    k.Arac.silindirHacmi = VeriErisim.DegerAl(r, 11, "");
                    k.Arac.motorGucu = VeriErisim.DegerAl(r, 12, "");
                    k.Arac.agirligi = VeriErisim.DegerAl(r, 13, "");
                    k.Arac.koltukSayisi = VeriErisim.DegerAl(r, 14, "");
                    k.Arac.kullanisAmaci = VeriErisim.DegerAl(r, 15, "");

                    //Taşınır Kiralama
                    k.Tasinir.adi = VeriErisim.DegerAl(r, 16, "");
                    k.Tasinir.markasi = VeriErisim.DegerAl(r, 17, "");
                    k.Tasinir.model = VeriErisim.DegerAl(r, 18, "");
                    k.Tasinir.modelYili = VeriErisim.DegerAl(r, 19, 0);
                    k.Tasinir.serisi = VeriErisim.DegerAl(r, 20, "");

                    //Taşınmaz Kiralama
                    k.Tasinmaz.cinsi = VeriErisim.DegerAl(r, 21, 0);
                    k.Tasinmaz.adi = VeriErisim.DegerAl(r, 22, "");
                    k.Tasinmaz.paftaAdaParsel = VeriErisim.DegerAl(r, 23, "");
                    k.Tasinmaz.yuzolcumu = VeriErisim.DegerAl(r, 24, "");
                    k.Tasinmaz.hisseOrani = VeriErisim.DegerAl(r, 25, "");
                    k.Tasinmaz.katSayisi = VeriErisim.DegerAl(r, 26, "");
                    k.Tasinmaz.yapimYili = VeriErisim.DegerAl(r, 27, 0);

                    if (r.FieldCount > 28)
                    {
                        k.muhasebe = VeriErisim.DegerAl(r, 28, 0);
                        k.birim = VeriErisim.DegerAl(r, 29, "");
                    }

                    bilgi.objeler.Add(k);
                }

            }
            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

    }
}

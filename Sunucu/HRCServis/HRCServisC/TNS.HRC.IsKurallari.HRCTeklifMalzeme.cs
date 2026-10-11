using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Teklife ait malzeme bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifMalzeme
    {
        /// <summary>
        /// Arguman olarak verilen, teklife ait malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifMalzeme nesne koleksiyonu olarak tutan nesne</param>
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
            foreach (TeklifMalzeme m in liste.objeler)
            {
                satir++;
                if (m.adi == "" && !m.sil)
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00279, satir)+"<br>";
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
            int yil = ((TeklifMalzeme)liste.objeler[0]).yil;
            string surecNo = ((TeklifMalzeme)liste.objeler[0]).surecNo;
            int tip = ((TeklifMalzeme)liste.objeler[0]).tip;
            if (tip != (int)ENUMTeklifTuru.KISMITEKLIF)
            {
                tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
                if (tamamlandi)
                {
                    sonuc.islemSonuc = true;
                    return sonuc;
                }
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifMalzeme to = new TABTeklifMalzeme(ve);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, teklife ait malzeme bilgisini veritabanından okur 
        /// ve Teklif Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki teklife ait malzeme listesini TeklifMalzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TeklifMalzeme kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifMalzeme to = new TABTeklifMalzeme(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TeklifMalzeme m = new TeklifMalzeme();

                    m.yil = VeriErisim.DegerAl(r, 0, 0);
                    m.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    m.birim = VeriErisim.DegerAl(r, 2, "");
                    m.surecNo = VeriErisim.DegerAl(r, 3, "");
                    m.malzemeNo = VeriErisim.DegerAl(r, 4, 0);
                    m.tip = VeriErisim.DegerAl(r, 5, 0);
                    m.adi = VeriErisim.DegerAl(r, 6, "");
                    m.miktar = VeriErisim.DegerAl(r, 7, 0.0);
                    m.olcuBirim.kod = VeriErisim.DegerAl(r, 8, 0);
                    m.malzemeKod = VeriErisim.DegerAl(r, 9, "");
                    m.olcuBirim.ad = VeriErisim.DegerAl(r, 10, "");
                    bilgi.objeler.Add(m);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firmanın ihalesini kazandığı malzeme bilgisini veritabanından okur 
        /// ve TeklifMalzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <param name="firmaNo">İşlem yapılacak firma no bilgisi</param>
        /// <returns>İstenilen kriterdeki, firmanın ihalesini kazandığı malzeme bilgisi listesini TeklifMalzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleMalzemeKazananFirma(Kullanici kullanan, int yil, string surecNo, int firmaNo)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifMalzeme to = new TABTeklifMalzeme(ve);
            IDataReader r = to.ListeleMalzemeKazananFirma(yil, surecNo, firmaNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TeklifMalzeme m = new TeklifMalzeme();

                    m.yil = VeriErisim.DegerAl(r, 0, 0);
                    m.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    m.birim = VeriErisim.DegerAl(r, 2, "");
                    m.surecNo = VeriErisim.DegerAl(r, 3, "");
                    m.malzemeNo = VeriErisim.DegerAl(r, 4, 0);
                    m.tip = VeriErisim.DegerAl(r, 5, 0);
                    m.adi = VeriErisim.DegerAl(r, 6, "");
                    m.miktar = VeriErisim.DegerAl(r, 7, 0.0);
                    m.olcuBirim.kod = VeriErisim.DegerAl(r, 8, 0);
                    m.malzemeKod = VeriErisim.DegerAl(r, 9, "");
                    m.olcuBirim.ad = VeriErisim.DegerAl(r, 10, "");
                    m.birimFiyat = VeriErisim.DegerAl(r, 11, 0.0);

                    bilgi.objeler.Add(m);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return bilgi;
        }
    }
}

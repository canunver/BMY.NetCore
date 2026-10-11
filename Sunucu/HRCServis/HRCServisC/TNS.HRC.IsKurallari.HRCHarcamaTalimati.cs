using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Harcama talimatı bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCHarcamaTalimati
    {
        /// <summary>
        /// Arguman olarak verilen, harcama talimatı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, HarcamaTalimati form, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hataStr = "";
            if (form.yil == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (form.muhasebe == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (form.birim.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
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
            TABHarcamaTalimati to = new TABHarcamaTalimati(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(form);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, harcama talimatı bilgisini veritabanından okur 
        /// ve HarcamaTalimatı nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, harcama talimatı bilgisi listesini HarcamaTalimati nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, HarcamaTalimati kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABHarcamaTalimati to = new TABHarcamaTalimati(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {

                    HarcamaTalimati bilgi = new HarcamaTalimati();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.harcamaMakami = VeriErisim.DegerAl(r, 4, "");
                    bilgi.harcamaBirimi = VeriErisim.DegerAl(r, 5, "");
                    bilgi.gerekcesi = VeriErisim.DegerAl(r, 6, "");
                    bilgi.konusu = VeriErisim.DegerAl(r, 7, "");
                    bilgi.miktarAciklama = VeriErisim.DegerAl(r, 8, "");
                    bilgi.gerceklestirmeSuresi = VeriErisim.DegerAl(r, 9, "");
                    bilgi.gerceklestirmeUsulu = VeriErisim.DegerAl(r, 10, "");
                    bilgi.yaklasikBedel = VeriErisim.DegerAl(r, 11, 0.0);
                    bilgi.butceTertip = VeriErisim.DegerAl(r, 12, "");
                    bilgi.odenekTutar = VeriErisim.DegerAl(r, 13, 0.0);
                    bilgi.evrakTarihi = VeriErisim.DegerAl(r, 14);
                    bilgi.evrakSeri = VeriErisim.DegerAl(r, 15, "");
                    bilgi.evrakSayi = VeriErisim.DegerAl(r, 16, "");
                    bilgi.gerceklestirmeGorevli = VeriErisim.DegerAl(r, 17, "");
                    bilgi.aciklama = VeriErisim.DegerAl(r, 18, "");
                    bilgi.teklifEdenYetkili = VeriErisim.DegerAl(r, 19, "");
                    bilgi.teklifEdenYetkiliUnvan = VeriErisim.DegerAl(r, 20, "");
                    bilgi.harcamaYetkilisi = VeriErisim.DegerAl(r, 21, "");
                    bilgi.harcamaYetkilisiUnvan = VeriErisim.DegerAl(r, 22, "");
                    bilgi.onayTarihi = VeriErisim.DegerAl(r, 23);
                    bilgi.avansVarmi = VeriErisim.DegerAl(r, 24, 0);
                    bilgi.mihmandarlikVarmi = VeriErisim.DegerAl(r, 25, 0);
                    bilgi.perTcNo = VeriErisim.DegerAl(r, 26, "");
                    bilgi.perAdSoyad = VeriErisim.DegerAl(r, 27, "");
                    bilgi.perBankaAd = VeriErisim.DegerAl(r, 28, "");
                    bilgi.perHesapNo = VeriErisim.DegerAl(r, 29, "");
                    bilgi.perVergiDairesi = VeriErisim.DegerAl(r, 30, "");
                    bilgi.perUnvan = VeriErisim.DegerAl(r, 31, "");

                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }
    }
}

using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhaleye firma daveti bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleFirmaTeklifDaveti
    {
        /// <summary>
        /// Arguman olarak verilen, ihaleye firma daveti bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="list">Kaydedilecek bilgileri FirmaTeklifDaveti nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray list, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(true, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hataStr = "";
            int satir = 0;
            ObjectArray silinecekler = new ObjectArray();
            ObjectArray kaydedilecekler = new ObjectArray();
            if (list.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                TNSDateTime bugunTarihi = new TNSDateTime(DateTime.Now.ToString());
                foreach (FirmaTeklifDaveti ida in list.objeler)
                {
                    satir++;
                    if (ida.sil == true)
                    {
                        FirmaTeklifDaveti silinecek = new FirmaTeklifDaveti();
                        silinecek.yil = ida.yil;
                        silinecek.muhasebe = ida.muhasebe;
                        silinecek.birim = ida.birim;
                        silinecek.surecNo = ida.surecNo;
                        silinecek.firmaKodu = ida.firmaKodu;
                        silinecekler.objeler.Add(silinecek);
                    }
                    else
                    {
                        if (ida.yil == 0)
                        {
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00153, satir) + "<br>";
                            break;
                        }
                        if (ida.muhasebe == 0)
                        {
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00154, satir) + "<br>";
                            break;
                        }
                        if (ida.birim.Trim() == "")
                        {
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00155, satir) + "<br>";
                            break;
                        }
                        if (ida.surecNo == "")
                        {
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00156, satir) + "<br>";
                            break;
                        }

                        if (ida.firmaKodu == 0)
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00157, satir) + "<br>";

                        kaydedilecekler.objeler.Add(ida);
                    }
                }//end foreach
            }

            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            FirmaTeklifDaveti dosya = (FirmaTeklifDaveti)list.objeler[0];
            int yil = dosya.yil;
            string surecNo = dosya.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleFirmaTeklifDaveti to = new TABIhaleFirmaTeklifDaveti(ve);
            ve.BeginTransaction();

            if (silinecekler.objeler.Count > 0)
                sonuc = to.Sil(silinecekler);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(kaydedilecekler);


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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleye firma daveti bilgisini veritabanından okur 
        /// ve FirmaTeklifDaveti nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleye firma daveti bilgisi listesini FirmaTeklifDaveti nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, FirmaTeklifDaveti kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleFirmaTeklifDaveti to = new TABIhaleFirmaTeklifDaveti(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    FirmaTeklifDaveti bilgi = new FirmaTeklifDaveti();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.firmaKodu = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.belgeGondermeTarihi = VeriErisim.DegerAl(r, 5);
                    bilgi.gidisSekli = VeriErisim.DegerAl(r, 6, 0);
                    bilgi.firmaAdi = VeriErisim.DegerAl(r, 7, "");
                    bilgi.firmaAdres = VeriErisim.DegerAl(r, 8, "");
                    bilgi.firmaTelefon = VeriErisim.DegerAl(r, 9, "");
                    bilgi.firmaFaks = VeriErisim.DegerAl(r, 10, "");
                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }
    }

}

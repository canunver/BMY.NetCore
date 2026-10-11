 using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Birime gelen birimden giden personel bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCGelenGidenPersonel
    {
        /// <summary>
        /// Arguman olarak verilen, birime gelen birimden giden personel bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="list">Kaydedilecek bilgileri PSLGelenGidenPersonel nesne koleksiyonu olarak tutan nesne</param>
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

            if (list.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                foreach (TNS.HRC.PSLGelenGidenPersonel ida in list.objeler)
                {
                    satir++;

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
                    //if (ida.tCKimlikNo == "")
                    //    hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00214, satir) + "<br>";
                    //if (ida.maasNo == 0)
                    //    hataStr += satir + ". satırda maaş no boş gelmiş.<br>";
                    //if (ida.say2000No == 0)
                    //    hataStr += satir + ". satırda sayi2000 no  boş gelmis.<br>";


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
            TNS.HRC.PSLGelenGidenPersonel izin = (TNS.HRC.PSLGelenGidenPersonel)list.objeler[0];
            int yil = izin.yil;
            string surecNo = izin.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGelenPersonel to = new TABGelenPersonel(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(izin);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(list);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, birime gelen birimden giden personel bilgisini veritabanından okur 
        /// ve PSLGelenGidenPersonel nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, birime gelen birimden giden personel bilgisi listesini PSLGelenGidenPersonel nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TNS.HRC.PSLGelenGidenPersonel kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGelenPersonel to = new TABGelenPersonel(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TNS.HRC.PSLGelenGidenPersonel bilgi = new TNS.HRC.PSLGelenGidenPersonel();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.tip = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.tCKimlikNo = VeriErisim.DegerAl(r, 5, "");
                    bilgi.maasNo = VeriErisim.DegerAl(r, 6, 0);
                    bilgi.say2000No = VeriErisim.DegerAl(r, 7, 0); 
                    bilgi.baslamaTarihi = VeriErisim.DegerAl(r, 8);
                    bilgi.gorevi = VeriErisim.DegerAl(r, 9, "");
                    bilgi.kadro = VeriErisim.DegerAl(r, 10, "");
                    bilgi.adSoyad = VeriErisim.DegerAl(r, 11, "");

                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }
    }

}

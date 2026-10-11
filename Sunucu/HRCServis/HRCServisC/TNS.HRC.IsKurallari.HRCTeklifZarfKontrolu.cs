using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Teklif veren firmaların, teklif zarf kontrol bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifZarfKontrolu
    {
        /// <summary>
        /// Arguman olarak verilen, teklif veren firmaların, teklif zarf kontrol bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, TeklifZarfKontroluBaslik liste, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan, liste.muhasebe, liste.birim))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";


            if (liste.satirlar.objeler.Count == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                TNSDateTime bugunTarihi = new TNSDateTime(DateTime.Now.ToString());
                if (liste.tarih.isNull)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00193 + "<br>";
                if ((liste.tarih.Yil > bugunTarihi.Yil) || ((liste.tarih.Yil >= bugunTarihi.Yil) && (liste.tarih.YilinGunu > bugunTarihi.YilinGunu)))
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00194;
                if (liste.saat == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00195 + "<br>";
                try
                {
                    string gunSaat = liste.saat;
                    int koordinat = gunSaat.IndexOf(":");
                    int saat = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(0, koordinat), 0);
                    int dakika = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(koordinat + 1, gunSaat.Length - koordinat - 1), 0);

                    /*if ((liste.tarih.Yil == bugunTarihi.Yil) && (bugunTarihi.YilinGunu == liste.tarih.YilinGunu))
                    {
                        if (saat > bugunTarihi.Saat)
                            hata += "Belge Saat bilgisi anlık saatten ileri bir saat girilmiştir.<br>";
                        else if ((saat == bugunTarihi.Saat) && (dakika > bugunTarihi.Dakika))
                            hata += "Belge Saat bilgisi anlık saatten ileri bir saat girilmiştir.<br>";
                    }*/
                }
                catch
                {
                    hata = TNS.HRC.HarcamaSurecleriResI.HRC00183 + "<br>";
                }


                int satir = 0;

                foreach (TeklifZarfKontroluSatir k in liste.satirlar.objeler)
                {
                    satir++;

                    if (k.firmaAdi.Trim() == "")
                        hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00280, satir) + "<br>";
                    if ((k.uygunmu == 1) && (k.degerlendirme != ""))
                        hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00281, satir) + "<br>";
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
            int yil = liste.yil;
            string surecNo = liste.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifZarfKontrolu to = new TABTeklifZarfKontrolu(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(liste);


            if (sonuc.islemSonuc)
            {
                sonuc = to.Kaydet(liste);
                if (sonuc.islemSonuc)
                {

                    int satir = 0;
                    ObjectArray firmalar = new ObjectArray();
                    foreach (TeklifZarfKontroluSatir k in liste.satirlar.objeler)
                    {
                        satir++;

                        IhaleTeklifDosyasi teklifVerenFirma = new IhaleTeklifDosyasi();
                        teklifVerenFirma.yil = liste.yil;
                        teklifVerenFirma.muhasebe = liste.muhasebe;
                        teklifVerenFirma.surecNo = liste.surecNo;
                        teklifVerenFirma.birim = liste.birim;
                        teklifVerenFirma.ihaleAsamasi = liste.ihaleAsamasi;
                        teklifVerenFirma.firmaKodu = k.firmaKodu;

                        //Firmanın verdiği zarf uygunluk kontrolünde geriye dönük firma devam bilgisinin güncellenmesi için
                        if (k.uygunmu == (int)ENUMUygunMu.UYGUN)
                        {
                            teklifVerenFirma.firmaDevam = (int)ENUMEvetHayir.EVET;
                            teklifVerenFirma.firmaYeterli = (int)ENUMEvetHayir.EVET;
                        }
                        else
                        {
                            teklifVerenFirma.firmaDevam = (int)ENUMEvetHayir.HAYIR;
                            teklifVerenFirma.firmaYeterli = (int)ENUMEvetHayir.HAYIR;
                        }
                        firmalar.objeler.Add(teklifVerenFirma);
                    }

                    TABTeklifDosyasiVeren tabTeklif = new TABTeklifDosyasiVeren(ve);
                    sonuc = tabTeklif.GuncelleFirmaSurecBilgisi(firmalar);

                    if (sonuc.islemSonuc)
                    {
                        TABTeklifEdilenFiyat tabTeklifFiyat = new TABTeklifEdilenFiyat(ve);
                        sonuc = tabTeklifFiyat.SilZarfKontroluGecemeyen(liste);
                    }
                }
            }
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
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif veren firmanın teklif zarfı kontrol bilgisini veritabanından okur 
        /// ve TeklifZarfKontroluBaslik nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklif zarf kontrol bilgisini TeklifZarfKontroluBaslik nesnesi olarak tutan nesne</returns>
        public TeklifZarfKontroluBaslik Listele(Kullanici kullanan, TeklifZarfKontroluBaslik kriter)
        {
            TeklifZarfKontroluBaslik bilgi = new TeklifZarfKontroluBaslik();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifZarfKontrolu to = new TABTeklifZarfKontrolu(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    if (bilgi.yil == 0)
                    {
                        bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                        bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                        bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                        bilgi.tarih = VeriErisim.DegerAl(r, 4);
                        bilgi.saat = VeriErisim.DegerAl(r, 5, "");
                        bilgi.ihaleAsamasi = VeriErisim.DegerAl(r, 9, 0);
                    }

                    TeklifZarfKontroluSatir f = new TeklifZarfKontroluSatir();

                    f.firmaKodu = VeriErisim.DegerAl(r, 6, 0);
                    f.uygunmu = VeriErisim.DegerAl(r, 7, 0);
                    f.degerlendirme = VeriErisim.DegerAl(r, 8, "");
                    f.firmaAdi = VeriErisim.DegerAl(r, 10, "");

                    bilgi.satirlar.objeler.Add(f);
                }
            }
            r.Close();


            TABTeklifZarfKontrolu to2 = new TABTeklifZarfKontrolu(ve);
            IDataReader r1 = to2.ListeleTeklifDosyasıKontrolEdilmeyenler(kriter);

            if (to2.Hata() == "")
            {
                while (r1.Read())
                {

                    TeklifZarfKontroluSatir f = new TeklifZarfKontroluSatir();

                    f.firmaKodu = VeriErisim.DegerAl(r1, 0, 0);
                    f.firmaAdi = VeriErisim.DegerAl(r1, 1, "");

                    bilgi.satirlar.objeler.Add(f);
                }

            }

            r1.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}

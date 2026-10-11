

using TNS.Veri;
using TNS.KYM;
using TNS.EVR.IsKurallari.Veri;
using EVRServisI;
using System.Data;
using System.Collections;
using System;
namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Saklama bilgileriyle ilgili, tanımlama, silme, listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class EVRSaklama
    {
        /// <summary>
        /// Parametre olarak verilen EvrakSaklama nesnesine göre, Saklama bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">The liste.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakSaklama[] liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            sonuc = KontrolEt(ve, kullanan, liste, "Yazma");

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABSaklama to = new TABSaklama(ve);


                    TABGenel tabGenel = new TABGenel(ve);
                    if (liste != null)
                        foreach (EvrakSaklama bilgi in liste)
                        {
                            if (string.IsNullOrEmpty(bilgi.kod))
                            {
                                string sayacKod = "EVRSAKLAMAKOD" + DateTime.Now.Year;
                                bilgi.kod = DateTime.Now.Year + tabGenel.BelgeNoAl(sayacKod, "0000000");
                            }
                            sonuc = to.Kaydet(bilgi);

                            if (!sonuc.islemSonuc)
                                break;
                        }

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();

                        sonuc.bilgiStr += EvrakI.EVRI011;
                    }
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{BEEB2DC7-032F-4A2E-9DE8-5369D9A1B632} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Saklama birimlerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Saklama bilgilerini içeren nesne koleksiyonu</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, string kod)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            //sonuc = KontrolEt(ve, kullanan, liste, "Silme");

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABSaklama to = new TABSaklama(ve);

                    if (sonuc.hataStr == "")
                        sonuc = to.Sil(kod);
                    else
                        sonuc.islemSonuc = false;

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = EvrakI.EVRI013;
                    }
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{F9269A56-3F3F-42AD-A172-FC7BF2ECE0D8} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ArrayList Listele(VeriErisim veriErisim, string prEvrakNo)
        {
            ArrayList hVAN = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABSaklama to = new TABSaklama(ve);
            IDataReader r = to.Listele(prEvrakNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    EvrakSaklama bilgi = new EvrakSaklama();
                    bilgi.kod = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.KOD, "");
                    bilgi.prEvrakNo = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.PREVRAKNO, "");
                    bilgi.kriterKod = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.KRITERKOD, 0);
                    bilgi.baslangicKod = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.BASLANGICKOD, 0);
                    bilgi.sure = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.SURE, 0);
                    bilgi.sureBirim = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.SUREBIRIM, "");
                    bilgi.bitisTarih = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.BITISTARIH);
                    bilgi.islem = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.ISLEM, 0);
                    bilgi.tasfiyePlani = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.TASFIYEPLANI, 0);
                    bilgi.raporTarih = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.RAPORTARIH);
                    bilgi.islemTarih = VeriErisim.DegerAl(r, (int)TABSaklama.ENUMAlanlar.ISLEMTARIH);

                    hVAN.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        public static Sonuc KontrolEt(VeriErisim veriErisim, Kullanici kullanan, EvrakSaklama[] liste, string yetkiTipi)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc(false, "", "");

            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri); 

            if (liste != null && yetkiTipi == "Yazma")
            {
                if (liste != null)
                    foreach (EvrakSaklama bilgi in liste)
                    {
                        if (bilgi.sure == 0)
                        {
                            //hata = EvrakI.EVRI172 + "<br>"; //Dosyaya kaldırma işlemi yapıldığında EVRSaklama.Kaydet işlemide çağrılıyor ve saklama süresi boş olduğu için işlem yapılsa bile uyarı çıkıyor. Bu nedenle kaldırıldı. Melih 16.05.2014 
                            break;
                        }
                        if (bilgi.kriterKod == 0)
                        {
                            hata = EvrakI.EVRI173 + "<br>";
                            break;
                        }
                        if (bilgi.baslangicKod == 0)
                        {
                            hata = EvrakI.EVRI174 + "<br>";
                            break;
                        }
                    }
            }

            if (hata == "")
                sonuc.islemSonuc = true;

            sonuc.hataStr = hata;

            //ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc SaklamaSuresiHesaplaveKaydet(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tabEvrak = new TABEvrak(ve);

            EvrakSaklama evrakSaklama = SaklamaSuresiHesapla(ve, kullanan, prEvrakNo);

            //Tasfiye bilgisini Kaydet
            if (evrakSaklama.tasfiyePlani > 0)
            {
                ObjectArray listeTasfiye = new ObjectArray();
                listeTasfiye.objeler.Add(new EvrakTasfiye() { prEvrakNo = prEvrakNo, tasfiyeKod = evrakSaklama.tasfiyePlani });
                sonuc = EVRTasfiye.Kaydet(ve, kullanan, listeTasfiye);
            }
            else
                sonuc.islemSonuc = true;

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    //Not: geçici olarak bu yöntem seçildi. Daha sonra parametre olarak geçirilecek. Hüseyin. [GÇ]
                    string evrEvrakNo = tabEvrak.EvrEvrakNoGetir(prEvrakNo);

                    EvrakSinif form = new EvrakSinif() { evrEvrakNo = evrEvrakNo, imhaTarihi = null, transferTarihi = null };

                    if (evrakSaklama.tasfiyePlani == (int)ENUMTanimlamaTasfiyePlani.IMHA)
                        form.imhaTarihi = evrakSaklama.bitisTarih;
                    else if (evrakSaklama.tasfiyePlani == (int)ENUMTanimlamaTasfiyePlani.TRANSFER)
                        form.transferTarihi = evrakSaklama.bitisTarih;

                    if (form.imhaTarihi != null || form.transferTarihi != null)
                        sonuc = tabEvrak.Guncelle(form);

                    if (sonuc.islemSonuc)
                        sonuc = Kaydet(ve, kullanan, new EvrakSaklama[] { evrakSaklama });

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();

                        sonuc.bilgiStr += EvrakI.EVRI011;
                    }
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{099861D6-D769-41D6-89D8-C01C5CA68920} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }
            }

            ve.VeriErisimKapat();


            return sonuc;
        }

        public static EvrakSaklama SaklamaSuresiHesapla(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo)
        {
            EvrakSaklama evrakSaklama = new EvrakSaklama();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            DosyaPlani[] dosyaPlanlari = EVREvrakDosyaPlani.Listele(ve, kullanan, prEvrakNo);
            if (dosyaPlanlari != null && dosyaPlanlari.Length > 0)
            {
                DosyaPlani dosyaPlanSuresiUzunOlan = new DosyaPlani();
                foreach (DosyaPlani item in dosyaPlanlari)
                    if (item.saklamaSuresi > dosyaPlanSuresiUzunOlan.saklamaSuresi)
                        dosyaPlanSuresiUzunOlan = item;

                if (dosyaPlanSuresiUzunOlan.saklamaSuresi > 0)
                {
                    ObjectArray evraklar = EVREvrak.Listele(ve, kullanan, new EvrakSinif() { prEvrakNo = prEvrakNo }, 0, 0);
                    if (evraklar.objeler.Count > 0)
                    {
                        EvrakSinif evrak = (EvrakSinif)evraklar[0];

                        //Saklama Süresi Hesapla
                        TNSDateTime bitisTarihi = new TNSDateTime();
                        switch (dosyaPlanSuresiUzunOlan.sureBaslangicTipi)
                        {
                            case (int)ENUMTanimlamaSureBaslangici.BELGEURETIMTARIHI:
                            case (int)ENUMTanimlamaSureBaslangici.KLASORACILISTARIHI:
                            case (int)ENUMTanimlamaSureBaslangici.KLASORKAPANISTARIHI:
                            case (int)ENUMTanimlamaSureBaslangici.KLASORDEKIILKDOSYATARIHI:
                            case (int)ENUMTanimlamaSureBaslangici.KLASORDEKISONDOSYATARIHI:
                                if (!evrak.uretimTarihi.isNull)
                                {
                                    evrak.uretimTarihi.AddYears(dosyaPlanSuresiUzunOlan.saklamaSuresi);
                                    bitisTarihi = evrak.uretimTarihi;
                                }
                                else
                                {
                                    evrak.evrakOlusturmaTarihi.AddYears(dosyaPlanSuresiUzunOlan.saklamaSuresi);
                                    bitisTarihi = evrak.evrakOlusturmaTarihi;
                                }
                                break;
                        }
                        //***********************************************
                        evrakSaklama.prEvrakNo = prEvrakNo;
                        evrakSaklama.sure = dosyaPlanSuresiUzunOlan.saklamaSuresi;
                        evrakSaklama.kriterKod = dosyaPlanSuresiUzunOlan.saklamaKriteri;
                        evrakSaklama.baslangicKod = dosyaPlanSuresiUzunOlan.sureBaslangicTipi;
                        evrakSaklama.tasfiyePlani = dosyaPlanSuresiUzunOlan.tasfiyePlanTipi;
                        evrakSaklama.sureBirim = "YIL";
                        evrakSaklama.bitisTarih = bitisTarihi;
                        evrakSaklama.islem = (int)ENUMEvrakSaklamaIslem.SUREHESAPLANDI;
                        evrakSaklama.islemTarih = new TNSDateTime(DateTime.Now);

                    }
                }
            }


            ve.VeriErisimKapat();

            return evrakSaklama;
        }

        public static Sonuc IslemGuncelle(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNoListe, int islem)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABSaklama to = new TABSaklama(ve);

            ve.BeginTransaction();
            try
            {
                foreach (var prEvrakNo in prEvrakNoListe.TrimEnd(';').Split(';'))
                {
                    sonuc = to.IslemGuncelle(prEvrakNo, islem, new TNSDateTime(DateTime.Now));
                    if (!sonuc.islemSonuc)
                        break;
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();

                    sonuc.bilgiStr += EvrakI.EVRI011;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{14D646C2-50F7-4172-B404-3C9CC5424516} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

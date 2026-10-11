using System;
using System.Data;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak hareket bilgileriyle ilgili, kaydetme, listeleme, havale notu listeleme, okundu bilgisi kaydetme ve son hareket bilgisi getirme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVREvrakHareket
    {
        /// <summary>
        /// Parametre olarak verilen EvrakHareket nesnesine göre, Evrak hareket bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EvrakHareket bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (form.prEvrakNo == "")
                sonuc.hataStr += " " + EvrakI.EVRI083 + "<br>";

            if (sonuc.hataStr != "")
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            VeriErisim veKYM = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM");


            //*******************************
            form.islemTarih = new TNSDateTime(DateTime.Now);
            if (string.IsNullOrEmpty(form.islemYapan))
                form.islemYapan = kullanan.mernis;
            if (string.IsNullOrEmpty(form.islemYapanAd))
                form.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
            if (form.kisi1 == form.islemYapan)
                form.kisi1Ad = form.islemYapanAd;
            if (form.kisi2 == form.islemYapan)
                form.kisi2Ad = form.islemYapanAd;

            if (form.kisi1 != "" && form.kisi1Ad == "")
                form.kisi1Ad = EVRGenel.KullaniciAdGetir(veKYM, form.kisi1);// EVRServis.UzayYaziDegerGetir("KULLANICIAD", form.kisi1, true, "");
            if (form.kisi2 != "" && form.kisi2Ad == "")
                form.kisi2Ad = EVRGenel.KullaniciAdGetir(veKYM, form.kisi2);//EVRServis.UzayYaziDegerGetir("KULLANICIAD", form.kisi2, true, "");

            if (form.vekil != "" && form.vekilAd == "")
                form.vekilAd = EVRGenel.KullaniciAdGetir(veKYM, form.vekil);
            veKYM.VeriErisimKapat();
            //*******************************

            ve.BeginTransaction();
            try
            {
                TABEvrakHareket to = new TABEvrakHareket(ve);

                form.siraNo = to.MaksSiraNo(form);

                sonuc = to.Kaydet(form);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{67E9ED41-894B-4BB1-B067-C779F57341E8} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen EvrakHareket nesnesine göre, evrak'ın hareket bilgisini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">EvrakHareket bilgisini içeren nesne</param>
        /// <returns>EvrakHareket nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, EvrakHareket form)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrakHareket to = new TABEvrakHareket(ve);
            IDataReader dr = to.Listele(form);

            if (dr != null)
            {
                while (dr.Read())
                {
                    EvrakHareket bilgi = new EvrakHareket();
                    bilgi.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.PREVRAKNO, "");
                    bilgi.siraNo = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.SIRANO, 0);
                    bilgi.birimKod1 = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.BIRIMKOD1, "");
                    bilgi.birimKod2 = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.BIRIMKOD2, "");
                    bilgi.kisi1 = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.KISI1, "");
                    bilgi.kisi2 = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.KISI2, "");
                    bilgi.islemTarih = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.ISLEMTARIH);
                    bilgi.islemYapan = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.ISLEMYAPAN, "");
                    bilgi.islem = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.ISLEM, 0);
                    bilgi.havaleNotu = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.HAVALENOTU, "");
                    bilgi.okundu = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.OKUNDU, 0);
                    bilgi.kisi1Ad = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.OKUNDU + 1, "");
                    bilgi.kisi2Ad = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.OKUNDU + 2, "");
                    bilgi.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.OKUNDU + 3, "");
                    bilgi.vekil = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.OKUNDU + 4, "");
                    bilgi.vekilAd = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.OKUNDU + 5, "");
                    bilgi.erisemez = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.OKUNDU + 6, 0);

                    hVAN.objeler.Add(bilgi);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            return hVAN;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesnesine göre, evrak sayısını hesaplayan yordam.
        /// </summary>
        /// <param name="form">EvrakSinif bilgisini içeren nesne</param>
        /// <returns>Evrak sayısı double nesene türünde döndürülür.</returns>
        public static double[] EvrakSayisiVer(ref VeriErisim ve, Kullanici kullanan, EvrakSinif[] form)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            TABEvrakHareket to = new TABEvrakHareket(ve);
            double[] deger = new double[form.Length];

            for (int i = 0; i < form.Length; i++)
            {
                if (form[i].hareket.islem == (int)ENUMEvrakTarihceDurum.DEVAMEDENISLER || form[i].hareket.islem == (int)ENUMEvrakTarihceDurum.DEVAMEDENISLERVEMIADLIISLER)
                    deger[i] = to.EvrakSayisiVerDevamEdenIsler(form[i]);
                else
                    deger[i] = to.EvrakSayisiVer(form[i]);
            }
            ve.VeriErisimKapat();

            return deger;
        }

        /// <summary>
        /// Evrak'ın havale sırasında kaydedilen notlarını okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakHareket nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray HavaleNotuListele(VeriErisim veriErisim, string prEvrakNo)
        {
            EvrakHareket form = new EvrakHareket();
            form.prEvrakNo = prEvrakNo;
            form.havaleNotu = "X";//null veya boş olmayanları listelem için
            //form.islem = (int)ENUMEvrakHareketDurum.HAVALEEDILDI;

            return Listele(veriErisim, form);
        }

        /// <summary>
        /// Evrak'ın hareket bilgisine okundu bilgisini kaydeden yordam.
        /// </summary>
        /// <param name="form">EvrakHareket bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc OkunduOlarakIsaretle(VeriErisim veriErisim, EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (form.prEvrakNo == "")
                sonuc.hataStr += " " + EvrakI.EVRI084 + "<br>";

            if (sonuc.hataStr != "")
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakHareket to = new TABEvrakHareket(ve);

                sonuc = to.OkunduOlarakIsaretle(form);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{8FECC729-E11A-4A07-BDFA-E8573EADD03C} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Evrak'ın son hareket durumunu okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakHareket nesnesi olarak döndürülür.</returns>
        public static EvrakHareket SonHaraketGetir(VeriErisim veriErisim, string prEvrakNo)
        {
            EvrakHareket bilgi = new EvrakHareket();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrakHareket to = new TABEvrakHareket(ve);
            IDataReader dr = to.SonHaraketGetir(prEvrakNo);

            if (dr != null)
            {
                if (dr.Read())
                {
                    bilgi.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.PREVRAKNO, "");
                    bilgi.siraNo = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.SIRANO, 0);
                    bilgi.birimKod1 = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.BIRIMKOD1, "");
                    bilgi.birimKod2 = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.BIRIMKOD2, "");
                    bilgi.kisi1 = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.KISI1, "");
                    bilgi.kisi2 = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.KISI2, "");
                    bilgi.islemTarih = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.ISLEMTARIH);
                    bilgi.islemYapan = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.ISLEMYAPAN, "");
                    bilgi.islem = VeriErisim.DegerAl(dr, (int)TABEvrakHareket.ENUMAlanlar.ISLEM, 0);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static Sonuc ErisimYetkisiDegistir(VeriErisim veriErisim, Kullanici kullanan, EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (form.prEvrakNo == "")
                sonuc.hataStr += " " + EvrakI.EVRI084 + "<br>";

            if (sonuc.hataStr != "")
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakHareket to = new TABEvrakHareket(ve);
                TABEvrak tabEvrak = new TABEvrak(ve);

                sonuc = to.ErisimYetkisiDegistir(form);

                if (sonuc.islemSonuc)
                {
                    //Not: geçici olarak bu yöntem seçildi. Daha sonra parametre olarak geçirilecek. Hüseyi. [GÇ]
                    string evrEvrakNo = tabEvrak.EvrEvrakNoGetir(form.prEvrakNo);

                    EvrakTarihce tarihce = new EvrakTarihce();
                    tarihce.evrEvrakNo = evrEvrakNo;
                    if (form.erisemez == 1)
                        tarihce.islem = string.Format("tarafından {0} isimli kullanıcının erişim yetkisi kaldırıldı.", form.islemYapanAd);
                    else
                        tarihce.islem = string.Format("tarafından {0} isimli kullanıcıya erişim yetkisi verildi.", form.islemYapanAd);

                    sonuc = EVREvrakTarihce.Kaydet(ve, kullanan, tarihce);
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{94020BE0-0A35-46C2-BE58-ED4C360F4226} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc SahiplikBilgisiDegistir(VeriErisim veriErisim, Kullanici kullanan, EvrakHareket form)
        {
            return Kaydet(veriErisim, kullanan, form);
        }

        public static Sonuc HareketDegistir(VeriErisim veriErisim, Kullanici kullanan, EvrakHareket[] formListe)
        {
            Sonuc sonuc = new Sonuc(false, "Kaydedilecek evrak bilgisi bulunamadı.", "");

            string anahtar = "";

            if (formListe != null && formListe.Length > 0)
            {
                foreach (EvrakHareket form in formListe)
                {
                    sonuc = Kaydet(veriErisim, kullanan, form);
                    if (!sonuc.islemSonuc)
                        break;
                    else
                        anahtar += form.prEvrakNo + ";";
                }
            }

            sonuc.anahtar = anahtar.TrimEnd(';');

            return sonuc;
        }
    }
}

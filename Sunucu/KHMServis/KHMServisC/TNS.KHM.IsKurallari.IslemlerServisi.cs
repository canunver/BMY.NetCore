using System;
using System.Data;
using TNS.BKD;
using TNS.BUM;
using TNS.KHM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using System.Collections;

namespace TNS.KHM.IsKurallari
{
    /// <summary>
    /// Kesin hesap ait işlemleri yürüten sınıf
    /// </summary>
    public class KHMIslemlerServisi
    {
        /// <summary>
        /// Devir akreditif (Bütçe İşlemleri) listelesini hazırlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="durum">Durum bilgisi.</param>
        /// <param name="akreditifMi">Eğer <c>true</c> olursa akreditif işlemi yapılır.</param>
        /// <returns></returns>
        public TNSCollection BIDevirAkreditifListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string[] kodlar, ENUMFormDurumGrup durum, bool akreditifMi)
        {
            TNSCollection donusDeger = new TNSCollection();
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            if (kodlar.Length == 0 || (kodlar.Length > 0 && (string)kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                string kurum = (string)kodlar[0];
                Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

                kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kurum, tUz.Kirilimlar(0));

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {
                    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                    TABButceIslemleri islGider = new TABButceIslemleri(ve);
                    IDataReader dr = null;
                    string[] islemlerAkreditif = new string[4];

                    islemlerAkreditif[0] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.AKREDITIF);
                    islemlerAkreditif[1] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.TAAHHUTARTIGI);
                    islemlerAkreditif[2] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.DEVREDENDISPROJE);
                    islemlerAkreditif[3] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.DEVRENGELIRLI);
                    string[] islemlerDevir = new string[1];
                    islemlerDevir[0] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.DEVRENGELIRLI);
                    if (akreditifMi)
                        dr = islGider.DevirAkreditifListele(yil, islemlerAkreditif, kodlar, durum);
                    else
                        dr = islGider.DevirAkreditifListele(yil, islemlerDevir, kodlar, durum);

                    if (dr != null)
                        while (dr.Read())
                        {
                            TertipOdenekBilgi to = new TertipOdenekBilgi(yil);

                            to.tertip.kodlar[0] = VeriErisim.DegerAl(dr, 0, "");
                            to.tertip.kodlar[1] = VeriErisim.DegerAl(dr, 1, "");
                            to.tertip.kodlar[2] = VeriErisim.DegerAl(dr, 2, "");
                            to.tertip.kodlar[3] = VeriErisim.DegerAl(dr, 3, "");
                            to.islem = VeriErisim.DegerAl(dr, 4, "");
                            to.odenek = VeriErisim.DegerAl(dr, 5, 0.0);

                            donusDeger.Add(to);
                        }

                    ve.VeriErisimKapat();

                }
                else
                    donusDeger.Add(new Sonuc(false, "Kurum Bilgilerini görme hakkınız yok!", ""));

            }
            return donusDeger;
        }

        TNSCollection BIDevirListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string[] kodlar, ENUMFormDurumGrup durum, bool akreditifMi)
        {
            TNSCollection donusDeger = new TNSCollection();
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            if (kodlar.Length == 0 || (kodlar.Length > 0 && (string)kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                string kurum = (string)kodlar[0];
                Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

                kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kurum, tUz.Kirilimlar(0));

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {
                    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                    TABButceIslemleri islGider = new TABButceIslemleri(ve);
                    IDataReader dr = null;
                    string[] islemler = new string[1];
                    islemler[0] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.EKLEME);

                    dr = islGider.DevirAkreditifListele(yil, islemler, kodlar, durum);

                    if (dr != null)
                        while (dr.Read())
                        {
                            TertipOdenekBilgi to = new TertipOdenekBilgi(yil);
                            to.tertip.kodlar[0] = VeriErisim.DegerAl(dr, 0, "");
                            to.tertip.kodlar[1] = VeriErisim.DegerAl(dr, 1, "");
                            to.tertip.kodlar[2] = VeriErisim.DegerAl(dr, 2, "");
                            to.tertip.kodlar[3] = VeriErisim.DegerAl(dr, 3, "");
                            to.islem = VeriErisim.DegerAl(dr, 4, "");
                            to.odenek = VeriErisim.DegerAl(dr, 5, 0.0);
                            donusDeger.Add(to);
                        }
                    ve.VeriErisimKapat();

                }
                else
                    donusDeger.Add(new Sonuc(false, "Kurum Bilgilerini görme hakkınız yok!", ""));

            }
            return donusDeger;
        }

        /// <summary>
        /// Yıl ici iptal bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="detaylar">TertipOdenekBilgi tipinde işlem yapılacak bilgileri taşıyan nesne.</param>
        /// <returns></returns>
        //public Sonuc YilIciIptalKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, TNSCollection detaylar)
        //{
        //    Sonuc sonuc = new Sonuc(true, "", "");
        //    KHMGiderServisi ser = new KHMGiderServisi();
        //    VeriErisim veKesinHesap = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    Tertip t = new Tertip(yil, ENUMTabloKod.TABOdenek);
        //    string kurumson = "";
        //    if (!(yil > 0))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = "Yıl bilgisi boş olamaz.";
        //        veKesinHesap.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    if (sonuc.islemSonuc)
        //    {
        //        veKesinHesap.BeginTransaction();
        //        for (int i = 0; i < detaylar.Count; i++)
        //        {
        //            if (sonuc.islemSonuc)
        //            {
        //                TertipOdenekBilgi tertipOdenek = (TertipOdenekBilgi)detaylar[i];
        //                if (tertipOdenek.tertip.TamMi())
        //                {
        //                    if (tertipOdenek.tertip.kodlar.Length == 0 || (tertipOdenek.tertip.kodlar.Length > 0 && (string)tertipOdenek.tertip.kodlar[0] == ""))
        //                        sonuc.hataStr = "Kurum kodu boş olamaz!";
        //                    else
        //                    {
        //                        string kurum = (string)tertipOdenek.tertip.kodlar[0];
        //                        kurumson = OrtakFonksiyonlar.KurumsaldanKuruma(kurum, tertipOdenek.tertip.Kirilimlar(0)).Replace(".", "");
        //                        kurum = kurum.Replace(".", "");
        //                        if (KHMYetkiServisi.KesinHesapOlusturmaYetkisiVarMi(kullanan, yil, kurum))
        //                        {
        //                            tertipOdenek.tertip.NoktalariAt();
        //                            TABKHIslemler tabIslemler = new TABKHIslemler(veKesinHesap);
        //                            if (tabIslemler.Sil(yil, tertipOdenek.tertip.kodlar).islemSonuc)
        //                                sonuc = tabIslemler.Kaydet(yil, tertipOdenek.tertip.kodlar, tertipOdenek.odenek);

        //                        }

        //                    }
        //                }
        //                else
        //                {
        //                    sonuc.islemSonuc = false;
        //                    sonuc.hataStr = "Girilen tertip (" + tertipOdenek.tertip.TertipStrOl() + ") hatalı!";
        //                    break;
        //                }
        //            }
        //            else
        //                break;
        //        }
        //    }
        //    if (sonuc.islemSonuc)
        //    {
        //        veKesinHesap.Commit();
        //    }
        //    else
        //        veKesinHesap.Rollback();

        //    if (sonuc.islemSonuc && kurumson != "")
        //    {
        //        t.kodlar[0] = kurumson;
        //        sonuc = DevirAkteditifGuncelle(veKesinHesap, kullanan, yil, 12, detaylar);
        //        if (!sonuc.islemSonuc)
        //            sonuc.hataStr = "Bilgiler kaydedilmiş ama güncelleme işlemi yapılamamıştır.";

        //        //sonuc = ser.KesinHesapGiderBilgileriHesapla(veKesinHesap, kullanan, yil, 12,t);
        //    }
        //    veKesinHesap.VeriErisimKapat();
        //    return sonuc;
        //}

        /// <summary>
        /// Yil içi iptal listelesi raporunu hazırlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public TNSCollection YilIciIptalListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string[] kodlar)
        {
            TNSCollection donusDeger = new TNSCollection();
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            if (kodlar.Length == 0 || (kodlar.Length > 0 && (string)kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                string kurum = (string)kodlar[0];
                kurum = kurum.Replace(".", "");
                //kurum = Tertip.KurumsaldanKuruma(kurum).Replace(".", "");

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {
                    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
                    TABKHIslemler islKH = new TABKHIslemler(ve);
                    IDataReader dr = null;


                    dr = islKH.Listele(yil, kodlar);

                    if (dr != null)
                        while (dr.Read())
                        {
                            TertipOdenekBilgi to = new TertipOdenekBilgi(yil);
                            to.tertip.kodlar[0] = VeriErisim.DegerAl(dr, 1, "");
                            to.tertip.kodlar[1] = VeriErisim.DegerAl(dr, 2, "");
                            to.tertip.kodlar[2] = VeriErisim.DegerAl(dr, 3, "");
                            to.tertip.kodlar[3] = VeriErisim.DegerAl(dr, 4, "");
                            to.odenek = VeriErisim.DegerAl(dr, 5, 0.0);
                            donusDeger.Add(to);
                        }
                    ve.VeriErisimKapat();

                }
                else
                    donusDeger.Add(new Sonuc(false, "Kurum Bilgilerini görme hakkınız yok!", ""));

            }
            return donusDeger;
        }

        //private Sonuc DevirAkteditifGuncelle(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, TNSCollection detaylar)
        //{
        //    Sonuc sonuc = new Sonuc(true, "", "");

        //    KHMGiderServisi ser = new KHMGiderServisi();
        //    //VeriErisim veKesinHesap = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM");           


        //    for (int i = 0; i < detaylar.Count; i++)
        //    {
        //        TertipOdenekBilgi tertipOdenek = (TertipOdenekBilgi)detaylar[i];
        //        sonuc = ser.KesinHesapGiderBilgileriHesapla(veriErisim, kullanan, yil, ay, tertipOdenek.tertip);
        //    }

        //    //veKesinHesap.VeriErisimKapat();
        //    return sonuc;
        //}

        /// <summary>
        /// Devir akreditif (Kesin hesap) listesini hazırlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="islem">İşlem bilgisi.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public TNSCollection KHDevirAkreditifListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string[] islem, string[] kodlar)
        {
            TNSCollection donusDeger = new TNSCollection();
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            if (kodlar.Length == 0 || (kodlar.Length > 0 && (string)kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                string kurum = (string)kodlar[0];
                Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

                kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kurum, tUz.Kirilimlar(0));

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {
                    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
                    TABKHDevir islKH = new TABKHDevir(ve);
                    IDataReader dr = null;


                    dr = islKH.Listele(yil, kurum, islem, kodlar);

                    if (dr != null)
                        while (dr.Read())
                        {
                            TertipOdenekBilgi to = new TertipOdenekBilgi(yil);
                            to.islem = VeriErisim.DegerAl(dr, 2, "");
                            to.tertip.kodlar[0] = VeriErisim.DegerAl(dr, 3, "");
                            to.tertip.kodlar[1] = VeriErisim.DegerAl(dr, 4, "");
                            to.tertip.kodlar[2] = VeriErisim.DegerAl(dr, 5, "");
                            to.tertip.kodlar[3] = VeriErisim.DegerAl(dr, 6, "");
                            to.odenek = VeriErisim.DegerAl(dr, 7, 0.0);
                            donusDeger.Add(to);
                        }
                    ve.VeriErisimKapat();

                }
                else
                    donusDeger.Add(new Sonuc(false, "Kurum Bilgilerini görme hakkınız yok!", ""));

            }
            return donusDeger;
        }

        /// <summary>
        /// Devir akreditif (Kesin Hesap) kaydetme işlemini yapan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="detaylar">TertipOdenekBilgi tipinde işlem yapılacak bilgileri taşıyan nesne.</param>
        /// <returns></returns>
        //public Sonuc KHDevirAkreditifKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, TNSCollection detaylar)
        //{
        //    Sonuc sonuc = new Sonuc(true, "", "");
        //    KHMGiderServisi ser = new KHMGiderServisi();
        //    VeriErisim veKesinHesap = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);
        //    string kurumson = "";
        //    if (!(yil > 0))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = "Yıl bilgisi boş olamaz.";
        //        return sonuc;
        //    }
        //    else if (!(KHMYetkiServisi.KesinHesapOlusturmaYetkisiVarMi(kullanan, yil, kurum)))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = "Bu işlemi yapmaya yetkiniz bulunammaktadır.";
        //        return sonuc;
        //    }

        //    if (sonuc.islemSonuc)
        //    {
        //        TABKHDevir tabDevir = new TABKHDevir(veKesinHesap);
        //        veKesinHesap.BeginTransaction();
        //        sonuc = tabDevir.Sil(yil, kurum.Replace(".", ""), "");
        //        if (sonuc.islemSonuc)
        //        {
        //            for (int i = 0; i < detaylar.Count; i++)
        //            {
        //                if (sonuc.islemSonuc)
        //                {
        //                    TertipOdenekBilgi tertipOdenek = (TertipOdenekBilgi)detaylar[i];
        //                    if (tertipOdenek.tertip.TamMi())
        //                    {
        //                        if (tertipOdenek.tertip.kodlar.Length == 0 || (tertipOdenek.tertip.kodlar.Length > 0 && (string)tertipOdenek.tertip.kodlar[0] == ""))
        //                            sonuc.hataStr = "Kurum kodu boş olamaz!";
        //                        else
        //                        {
        //                            //string kurum = (string)tertipOdenek.tertip.kodlar[0];
        //                            kurumson = OrtakFonksiyonlar.KurumsaldanKuruma(kurum, tertipOdenek.tertip.Kirilimlar(0)).Replace(".", "");

        //                            tertipOdenek.tertip.NoktalariAt();
        //                            sonuc = tabDevir.Kaydet(yil, kurumson, tertipOdenek.islem.Replace(".", ""), tertipOdenek.tertip.kodlar, tertipOdenek.odenek);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        sonuc.islemSonuc = false;
        //                        sonuc.hataStr = "Girilen tertip (" + tertipOdenek.tertip.TertipStrOl() + ") hatalı!";
        //                        break;
        //                    }
        //                }
        //                else
        //                    break;
        //            }
        //        }
        //    }
        //    if (sonuc.islemSonuc)
        //    {
        //        veKesinHesap.Commit();
        //    }
        //    else
        //        veKesinHesap.Rollback();

        //    if (sonuc.islemSonuc && kurumson != "")
        //    {
        //        t.kodlar[0] = (kurumson);
        //        sonuc = DevirAkteditifGuncelle(veKesinHesap, kullanan, yil, 12, detaylar);
        //        if (!sonuc.islemSonuc)
        //            sonuc.hataStr = "Bilgiler kaydedilmiş ama güncelleme işlemi yapılamamıştır.";

        //        //sonuc = ser.KesinHesapGiderBilgileriHesapla(veKesinHesap, kullanan, yil, 12,t);
        //    }
        //    veKesinHesap.VeriErisimKapat();
        //    return sonuc;
        //}

        /// <summary>
        /// Gider gerekçesi kayıt eden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="gerekce">İşlem yapılacak bilgi.</param>
        /// <returns></returns>
        public Sonuc GiderGerekceKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, GiderGerekce gerekce)
        {
            string bilgiStr = "";
            Sonuc sonuc = new Sonuc(false, "Yetkiniz yok", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));


            if (KHMYetkiServisi.YazmaYetkisiVarMi(kullanan, (string)gerekce.tertip.kodlar[0]))
            {
                ve.BeginTransaction();
                gerekce.tertip.NoktalariAt();
                TABKHIslemler to = new TABKHIslemler(ve);

                sonuc = to.GiderGerekceSil(yil, gerekce.tertip.kodlar);

                if (sonuc.islemSonuc)
                {
                    int yazilan = 0;
                    string parca = "";
                    int satir = 0;
                    int kalan = gerekce.gerekce.Length;
                    int parcaBoy;
                    while (kalan > 0)
                    {
                        if (kalan > 250)
                            parcaBoy = 250;
                        else
                            parcaBoy = kalan;

                        parca = gerekce.gerekce.Substring(yazilan, parcaBoy);
                        yazilan = yazilan + parcaBoy;
                        kalan = kalan - parcaBoy;
                        sonuc = to.GiderGerekceKaydet(yil, gerekce.tertip.kodlar, ++satir, parca);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                sonuc.bilgiStr = bilgiStr;
            }
            else
                sonuc.hataStr = "Bu tertibi değiştirmeye yetkiniz yok.";

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Gider gerekçesi kayıt eden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="gerekce">İşlem yapılacak bilgi.</param>
        /// <returns></returns>
        public Sonuc GiderGerekceDetayKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, GiderGerekceDetay gerekce)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));

            TNS.BHMv2.IBHMv2Servis servisBHM = TNS.BHMv2.Arac.Tanimla();

            if (!servisBHM.OGYYetkisiVarMi(kullanan, null, gerekce.tertip, TNS.BHMv2.ENUMOGY.Yazma))
            {
                sonuc.DegerAta(string.Format("Verdiğiniz tertibe {0} açıklama girme yetkiniz yok!", gerekce.tertip.TertipStrOl()));
                ve.VeriErisimKapat();
                return sonuc;
            }
            TABKHIslemler to = new TABKHIslemler(ve);

            if (OnayliMi(to, yil, gerekce))
            {
                sonuc.DegerAta(string.Format("Verdiğiniz tertip onaylandığı için değiştirilemez!", gerekce.tertip.TertipStrOl()));
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();
            gerekce.tertip.NoktalariAt();

            sonuc = to.GiderGerekceDetaySil(yil, gerekce.tertip.kodlar);

            if (sonuc.islemSonuc)
            {
                int yazilan = 0;
                string parca = "";
                int satir = 0;
                int kalan = gerekce.gerekce.Length;
                int parcaBoy;
                while (kalan > 0)
                {
                    if (kalan > 250)
                        parcaBoy = 250;
                    else
                        parcaBoy = kalan;

                    parca = gerekce.gerekce.Substring(yazilan, parcaBoy);
                    yazilan = yazilan + parcaBoy;
                    kalan = kalan - parcaBoy;
                    sonuc = to.GiderGerekceDetayKaydet(yil, gerekce.tertip.kodlar, ++satir, parca);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private bool OnayliMi(TABKHIslemler to, int yil, GiderGerekceDetay gerekce)
        {
            int onayKod = to.OnayKodBul(yil, gerekce.tertip.kodlar);
            if (onayKod > 2)
                return true;
            return false;
        }

        internal Sonuc GiderGerekceDetayOnayla(VeriErisim veriErisim, Kullanici kullanan, int yil, GiderGerekceDetay gerekce)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));

            TNS.BHMv2.IBHMv2Servis servisBHM = TNS.BHMv2.Arac.Tanimla();

            if (!servisBHM.OGYYetkisiVarMi(kullanan, null, gerekce.tertip, TNS.BHMv2.ENUMOGY.Yazma))
            {
                sonuc.DegerAta(string.Format("Verdiğiniz tertibe {0} yazma yetkiniz yok!", gerekce.tertip.TertipStrOl()));
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();
            gerekce.tertip.NoktalariAt();
            TABKHIslemler to = new TABKHIslemler(ve);

            sonuc = to.GiderGerekceDetayOnaySil(yil, gerekce.tertip.kodlar);

            if (sonuc.islemSonuc)
            {
                sonuc = to.GiderGerekceDetayOnayla(yil, gerekce.tertip.kodlar, 3);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Gelir gerekçesi kayıt eden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="gerekce">İşlem yapılacak bilgi.</param>
        /// <returns></returns>
        public Sonuc GelirGerekceKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, GelirGerekce gerekce)
        {
            Sonuc sonuc = new Sonuc(false, "Yetkiniz yok", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));


            if (KHMYetkiServisi.YazmaYetkisiVarMi(kullanan, (string)gerekce.tertip.kodlar[0]))
            {
                ve.BeginTransaction();
                gerekce.tertip.NoktalariAt();
                TABKHIslemler tg = new TABKHIslemler(ve);

                sonuc = tg.GelirGerekceSil(yil, gerekce.tertip.kodlar);

                if (sonuc.islemSonuc && gerekce.gerekce != null && gerekce.gerekce != "")
                {
                    int yazilan = 0;
                    string parca = "";
                    int satir = 0;
                    int kalan = gerekce.gerekce.Length;
                    int parcaBoy;
                    while (kalan > 0)
                    {
                        if (kalan > 250)
                            parcaBoy = 250;
                        else
                            parcaBoy = kalan;

                        parca = gerekce.gerekce.Substring(yazilan, parcaBoy);
                        yazilan = yazilan + parcaBoy;
                        kalan = kalan - parcaBoy;
                        sonuc = tg.GelirGerekceKaydet(yil, gerekce.tertip.kodlar, ++satir, parca);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            else
                sonuc.hataStr = "Bu tertibi değiştirmeye yetkiniz yok.";

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Gider gerekcesi getiren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public GiderGerekceVAN GiderGerekceGetir(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip tertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            GiderGerekceVAN gv = new GiderGerekceVAN();
            gv.sonuc.islemSonuc = true;
            int satir;

            string yazi;

            if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, (string)tertip.kodlar[0]))
            {
                TABKHIslemler to = new TABKHIslemler(ve);
                tertip.DiziKontrol();
                tertip.NoktalariAt();
                gv.gerekce.gerekce = "";
                IDataReader r = to.GiderGerekceGetir(yil, tertip.kodlar);
                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        satir = VeriErisim.DegerAl(r, 0, 0);
                        yazi = VeriErisim.DegerAl(r, 1, "");
                        gv.gerekce.gerekce = gv.gerekce.gerekce + yazi;
                    }
                    r.Close();
                }
                else
                    gv.sonuc.hataStr = to.Hata();
            }
            else
            {
                gv.sonuc.hataStr = "Bu tertipleri görmeye yetkiniz yok.";
            }
            ve.VeriErisimKapat();
            return gv;
        }

        /// <summary>
        /// Gider gerekcesi getiren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public GiderGerekceDetayVAN GiderGerekceDetayGetir(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip tertip)
        {
            GiderGerekceDetayVAN gv = new GiderGerekceDetayVAN(yil);
            TNS.BHMv2.IBHMv2Servis servisBHM = TNS.BHMv2.Arac.Tanimla();

            if (!servisBHM.OGYYetkisiVarMi(kullanan, null, tertip, TNS.BHMv2.ENUMOGY.Gorme))
            {
                gv.sonuc.DegerAta(string.Format("Verdiğiniz kodu {0} görme yetkiniz yok!", tertip.TertipStrOl()), "");
                return gv;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            gv.sonuc.islemSonuc = true;
            int satir;
            string yazi;

            TABKHIslemler to = new TABKHIslemler(ve);
            tertip.DiziKontrol();
            tertip.NoktalariAt();
            gv.gerekce.gerekce = "";
            IDataReader r = to.GiderGerekceDetayGetir(yil, tertip.kodlar);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    satir = VeriErisim.DegerAl(r, 0, 0);
                    yazi = VeriErisim.DegerAl(r, 1, "");
                    gv.gerekce.gerekce = gv.gerekce.gerekce + yazi;
                }
                r.Close();
            }
            else
                gv.sonuc.hataStr = to.Hata();
            ve.VeriErisimKapat();
            return gv;
        }

        /// <summary>
        /// Gider gerekcesi listeleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public ObjectArray GiderGerekceListele(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip tertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray gv = new ObjectArray();
            gv.sonuc.islemSonuc = true;

            if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, (string)tertip.kodlar[0]))
            {
                TABKHIslemler to = new TABKHIslemler(ve);
                tertip.DiziKontrol();
                tertip.NoktalariAt();
                IDataReader r = to.GiderGerekceListele(yil, tertip.kodlar);
                yil = 0;
                string kur = "";
                string fon = "";
                string fin = "";
                string eko = "";
                int satir = 0;
                if (to.Hata() == "")
                {
                    GiderGerekce gg = new GiderGerekce();

                    while (r.Read())
                    {
                        satir = VeriErisim.DegerAl(r, 5, 0);
                        if (satir == 1 && gg.yil > 0)//İlk satırdan sonra gelen ilk 1 nolu satır için
                        {
                            gv.objeler.Add(gg);
                            gg = new GiderGerekce();
                        }
                        gg.yil = VeriErisim.DegerAl(r, 0, 0);
                        kur = VeriErisim.DegerAl(r, 1, "");
                        fon = VeriErisim.DegerAl(r, 2, "");
                        fin = VeriErisim.DegerAl(r, 3, "");
                        eko = VeriErisim.DegerAl(r, 4, "");
                        gg.tertip.TertipAta(kur + "-" + fon + "-" + fin + "-" + eko);
                        gg.gerekce += VeriErisim.DegerAl(r, 6, "");
                    }
                    gv.objeler.Add(gg);

                    r.Close();
                }
                else
                    gv.sonuc.hataStr = to.Hata();
            }
            else
                gv.sonuc.hataStr = "Bu tertipleri görmeye yetkiniz yok.";

            ve.VeriErisimKapat();
            return gv;
        }

        internal string FaaliyetAciklamaBul(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip tertip)
        {
            if (tertip.kodlar.Length < 1)
                return "";
            if ((tertip.kodlar[0]).Length != ((string)tertip.Kirilimlar(0)).Length)
                return "";

            if (!KHMYetkiServisi.YazmaYetkisiVarMi(kullanan, tertip.kodlar[0])) return "";
            tertip.DiziKontrol();
            tertip.NoktalariAt();
            int prgKod = OrtakFonksiyonlar.ConvertToInt(tertip.kodlar[1], 0);
            tertip.kodlar[1] = "";

            string pebKodlar = PebkodBul(kullanan, yil, prgKod);
            if (string.IsNullOrEmpty(pebKodlar))
                return "";
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKHIslemler to = new TABKHIslemler(ve);

            IDataReader r = to.GiderGerekceListeleDetay(yil, tertip.kodlar, pebKodlar);

            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            System.Text.StringBuilder donDeger = new System.Text.StringBuilder();
            if (r != null)
            {
                while (r.Read())
                {
                    yil = VeriErisim.DegerAl(r, 0, 0);
                    string pebKod = VeriErisim.DegerAl(r, 5, "");
                    int satir = VeriErisim.DegerAl(r, 6, 0);
                    if (satir == 1)
                    {
                        donDeger.Append("\r\n\r\n" + uzyServis.UzayDegeriStr(kullanan, "ABSPEBKOD", yil + "-" + pebKod, true) + "\r\n");
                    }
                    donDeger.Append(VeriErisim.DegerAl(r, 7, ""));
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return donDeger.ToString();
        }

        private string PebkodBul(Kullanici kullanan, int yil, int prgKod)
        {
            BKD.PrgPebIliski kriter = new BKD.PrgPebIliski();
            kriter.yil = yil;
            kriter.prgKod = prgKod;
            string tut = "";
            ObjectArray listeAF = TNS.BKD.Arac.Tanimla().PrgPebIliskiListele(kullanan, kriter);
            foreach (BKD.PrgPebIliski item in listeAF.objeler)
            {
                if (string.IsNullOrWhiteSpace(item.pebKod)) continue;
                if (tut != "") tut += ",";
                tut += item.pebKod;
            }
            return tut;
        }

        /// <summary>
        /// Gider gerekcesi listeleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public ObjectArray GiderGerekceListeleDetay(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip tertip)
        {
            ObjectArray gv = new ObjectArray();
            gv.sonuc.islemSonuc = true;

            TNS.BHMv2.IBHMv2Servis servisBHM = TNS.BHMv2.Arac.Tanimla();

            if (!servisBHM.OGYYetkisiVarMi(kullanan, null, tertip, TNS.BHMv2.ENUMOGY.Gorme))
            {
                gv.sonuc.DegerAta(string.Format("Verdiğiniz kodu {0} görme yetkiniz yok!", tertip.TertipStrOl()), "");
                return gv;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABKHIslemler to = new TABKHIslemler(ve);
            tertip.DiziKontrol();
            tertip.NoktalariAt();
            IDataReader r = to.GiderGerekceListeleDetay(yil, tertip.kodlar);
            yil = 0;
            string kur = "";
            string fon = "";
            string fin = "";
            string eko = "";
            string peb = "";
            int satir = 0;
            if (to.Hata() == "")
            {
                GiderGerekceDetay gg = null;

                while (r.Read())
                {
                    satir = VeriErisim.DegerAl(r, 6, 0);
                    if (satir == 1 || gg == null)//İlk satırdan sonra gelen ilk 1 nolu satır için
                    {
                        if (gg != null)
                            gv.objeler.Add(gg);
                        gg = new GiderGerekceDetay(yil);
                        kur = VeriErisim.DegerAl(r, 1, "");
                        fon = VeriErisim.DegerAl(r, 2, "");
                        fin = VeriErisim.DegerAl(r, 3, "");
                        eko = VeriErisim.DegerAl(r, 4, "");
                        peb = VeriErisim.DegerAl(r, 5, "");
                        gg.tertip.TertipAta(kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb);
                    }
                    gg.gerekce += VeriErisim.DegerAl(r, 7, "");
                }
                if (gg != null)
                    gv.objeler.Add(gg);

                r.Close();
            }
            else
                gv.sonuc.hataStr = to.Hata();

            ve.VeriErisimKapat();
            return gv;
        }

        internal ObjectArray FaaliyetAciklamaDamgaSayBul(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip)
        {
            ObjectArray gv = new ObjectArray();
            int yil = tertip.yil;

            TNS.BHMv2.IBHMv2Servis servisBHM = TNS.BHMv2.Arac.Tanimla();

            if (!servisBHM.OGYYetkisiVarMi(kullanan, null, tertip, TNS.BHMv2.ENUMOGY.Gorme))
            {
                gv.sonuc.DegerAta(string.Format("Verdiğiniz kodu {0} görme yetkiniz yok!", tertip.TertipStrOl()), "");
                return gv;
            }
            string[] sorgular = new string[2];
            string[] baglantilar = new string[2];
            tertip.DiziKontrol();
            tertip.NoktalariAt();
            sorgular[0] = TABKHIslemler.GiderGerekceDamgaSayBulDetaySql(yil, tertip.kodlar);
            sorgular[1] = TABKHIslemler.GiderGerekceOnayBulDetaySql(yil, tertip.kodlar);
            baglantilar[0] = "BaglantiSatiriKHM";
            baglantilar[1] = "BaglantiSatiriKHM";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();
            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4 });

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                GiderGerekceDetay gg = new GiderGerekceDetay(yil);
                string kur = alAnahtar.StrAl(0);
                string fon = alAnahtar.StrAl(1);
                string fin = alAnahtar.StrAl(2);
                string eko = alAnahtar.StrAl(3);
                string peb = alAnahtar.StrAl(4);
                gg.damgaSay = alVeri.IntAl(0);
                gg.onayKod = alVeri.IntAl(1);

                gg.tertip.TertipAta(kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb);
                gg.tertip.Noktala();
                gv.Ekle(gg);
            }

            return gv;
        }

        /// <summary>
        /// Gelir gerekcesi getiren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public GelirGerekceVAN GelirGerekceGetir(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip tertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            GelirGerekceVAN gv = new GelirGerekceVAN();
            gv.sonuc.islemSonuc = true;
            int satir;

            string yazi;
            if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, (string)tertip.kodlar[0]))
            {
                TABKHIslemler tg = new TABKHIslemler(ve);
                tertip.DiziKontrol();
                tertip.NoktalariAt();
                gv.gerekce.gerekce = "";
                IDataReader r = tg.GelirGerekceGetir(yil, tertip.kodlar);
                if (tg.Hata() == "")
                {
                    while (r.Read())
                    {
                        satir = VeriErisim.DegerAl(r, 0, 0);
                        yazi = VeriErisim.DegerAl(r, 1, "");
                        gv.gerekce.gerekce = gv.gerekce.gerekce + yazi;
                    }
                    r.Close();
                }
                else
                    gv.sonuc.hataStr = tg.Hata();
            }
            else
            {
                gv.sonuc.hataStr = "Bu tertipleri görmeye yetkiniz yok.";
            }
            ve.VeriErisimKapat();
            return gv;
        }

        /// <summary>
        /// Gider gerekçesi silen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public Sonuc GiderGerekceSil(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip tertip)
        {
            if (tertip.kodlar.Length < 1)
                return new Sonuc(false, "Hatalı tertip geldi", "");
            if (((string)tertip.kodlar[0]).Length != ((string)tertip.Kirilimlar(0)).Length)
            {
                return new Sonuc(false, "Bütçe fişi silme işleminde birim girilmelidir. Hatalı birim kodu: " + ((string)tertip.kodlar[0]), "");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM", TNS.KYM.Arac.KimlikBul(kullanan));

            if (KHMYetkiServisi.YazmaYetkisiVarMi(kullanan, (string)tertip.kodlar[0]))
            {
                tertip.NoktalariAt();
                ve.BeginTransaction();
                TABKHIslemler to = new TABKHIslemler(ve);
                Sonuc sonuc = to.GiderGerekceSil(yil, tertip.kodlar);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                return sonuc;
            }
            else
                return new Sonuc(false, "Bu kurumsal kodla(" + (string)tertip.kodlar[0] + ") ilgili işlem yapma yetkiniz .", "");
        }

        /// <summary>
        /// Gider gerekçesi silen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public Sonuc GiderGerekceSilDetay(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip tertip)
        {
            if (tertip.kodlar.Length < 1)
                return new Sonuc(false, "Hatalı tertip geldi", "");
            if ((tertip.kodlar[0]).Length != ((string)tertip.Kirilimlar(0)).Length || (tertip.kodlar[4]).Length != ((string)tertip.Kirilimlar(4)).Length)
            {
                return new Sonuc(false, string.Format("Açıklama silme işleminde birim ve faaliyet girilmelidir. Hatalı birim kodu:{0} faaliyet kodu:{1}", tertip.kodlar[0], tertip.kodlar[4]), "");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM", TNS.KYM.Arac.KimlikBul(kullanan));

            if (KHMYetkiServisi.YazmaYetkisiVarMi(kullanan, tertip.kodlar[0], tertip.kodlar[4]))
            {
                tertip.NoktalariAt();
                ve.BeginTransaction();
                TABKHIslemler to = new TABKHIslemler(ve);
                Sonuc sonuc = to.GiderGerekceDetaySil(yil, tertip.kodlar);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                return sonuc;
            }
            else
                return new Sonuc(false, "Bu kurumsal kodla veya faaliyetle (" + (string)tertip.kodlar[0] + ", " + (string)tertip.kodlar[4] + ") ilgili işlem yapma yetkiniz .", "");
        }

        /// <summary>
        /// Gelir gerekçesi silen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public Sonuc GelirGerekceSil(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip tertip)
        {
            if (tertip.kodlar.Length < 1)
                return new Sonuc(false, "Hatalı tertip geldi", "");
            if (((string)tertip.kodlar[0]).Length != ((string)tertip.Kirilimlar(0)).Length)
            {
                return new Sonuc(false, "Bütçe fişi silme işleminde birim girilmelidir. Hatalı birim kodu: " + ((string)tertip.kodlar[0]), "");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));

            if (KHMYetkiServisi.YazmaYetkisiVarMi(kullanan, (string)tertip.kodlar[0]))
            {
                tertip.NoktalariAt();
                ve.BeginTransaction();
                TABKHIslemler to = new TABKHIslemler(ve);
                Sonuc sonuc = to.GelirGerekceSil(yil, tertip.kodlar);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                return sonuc;
            }
            else
                return new Sonuc(false, "Bu kurumsal kodla(" + (string)tertip.kodlar[0] + ") ilgili işlem yapma yetkiniz .", "");
        }

        /// <summary>
        /// İta amirlerini listeleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="derece">Derece bilgisi.</param>
        /// <returns></returns>
        public TNSCollection ItaAmirleriListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, int derece)
        {
            TNSCollection donusDeger = new TNSCollection();
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            if (kurum == "")
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                kurum = kurum.Replace(".", "");
                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {
                    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
                    TABKHIslemler islKH = new TABKHIslemler(ve);
                    IDataReader dr = null;

                    dr = islKH.ItaAmirleriListele(yil, kurum, derece);

                    if (dr != null)
                        while (dr.Read())
                        {
                            KHItaAmiri ita = new KHItaAmiri();
                            ita.yil = VeriErisim.DegerAl(dr, 0, 0);
                            ita.kurum = VeriErisim.DegerAl(dr, 1, "");
                            ita.derece = VeriErisim.DegerAl(dr, 3, 0);
                            ita.bilgiler = new KHItaBilgileri();
                            ita.bilgiler.birim = VeriErisim.DegerAl(dr, 2, "");
                            ita.bilgiler.adSoyad = VeriErisim.DegerAl(dr, 4, "");
                            if (!dr.IsDBNull(4)) ita.bilgiler.baslamaTarihi = new TNSDateTime(dr.GetDateTime(5));
                            if (!dr.IsDBNull(5)) ita.bilgiler.ayrilmaTarihi = new TNSDateTime(dr.GetDateTime(6));
                            donusDeger.Add(ita);
                        }
                    ve.VeriErisimKapat();

                }
                else
                    donusDeger.Add(new Sonuc(false, "Kurum Bilgilerini görme hakkınız yok!", ""));

            }
            return donusDeger;

        }

        /// <summary>
        /// İta amirlerini kayıt eden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="derece">Derece bilgisi.</param>
        /// <param name="detaylar">KHItaBilgileri tipinde işlem yapılacak bilgileri taşıyan nesne.</param>
        /// <returns></returns>
        public Sonuc ItaAmirleriKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, int derece, TNSCollection detaylar)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            KHMIslemlerServisi ser = new KHMIslemlerServisi();
            VeriErisim veKesinHesap = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));


            if (!(yil > 0))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Yıl bilgisi boş olamaz.";
                return sonuc;
            }

            if (sonuc.islemSonuc)
            {
                veKesinHesap.BeginTransaction();
                TABKHIslemler tabIslemler = new TABKHIslemler(veKesinHesap);
                if (KHMYetkiServisi.YazmaYetkisiVarMi(kullanan, kurum))
                {
                    if (tabIslemler.ItaAmirleriSil(yil, kurum, derece).islemSonuc)
                    {
                        for (int i = 0; i < detaylar.Count; i++)
                        {
                            if (sonuc.islemSonuc)
                            {
                                KHItaBilgileri itaAmiri = (KHItaBilgileri)detaylar[i];
                                sonuc = tabIslemler.ItaAmirleriKaydet(yil, kurum, itaAmiri.birim, derece, itaAmiri.adSoyad, itaAmiri.baslamaTarihi, itaAmiri.ayrilmaTarihi);
                            }

                            else
                                break;
                        }
                    }
                }
            }
            if (sonuc.islemSonuc)
            {
                veKesinHesap.Commit();
            }
            else
                veKesinHesap.Rollback();

            veKesinHesap.VeriErisimKapat();
            return sonuc;
        }

        internal Sonuc GiderGerekceDetayOnayKaldir(VeriErisim veriErisim, Kullanici kullanan, Tertip kistasTertip)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));

            TNS.BHMv2.IBHMv2Servis servisBHM = TNS.BHMv2.Arac.Tanimla();

            if (!servisBHM.OGYYetkisiVarMi(kullanan, null, kistasTertip, TNS.BHMv2.ENUMOGY.Yazma))
            {
                sonuc.DegerAta(string.Format("Verdiğiniz tertibe {0} yazma yetkiniz yok!", kistasTertip.TertipStrOl()));
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();
            kistasTertip.NoktalariAt();
            TABKHIslemler to = new TABKHIslemler(ve);

            sonuc = to.GiderGerekceDetayOnaySil(kistasTertip.yil, kistasTertip.kodlar);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}

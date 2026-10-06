using System;
using System.Data;
using TNS.BKD;
using TNS.BUM;
using TNS.KHM.IsKurallari.Veri;
using TNS.KYM;
using TNS.MUH;
using TNS.UZY;
using TNS.Veri;
using TNS.BHMv2;

namespace TNS.KHM.IsKurallari
{
    /// <summary>
    /// Gider ile ilgili fonksiyonları yöneten sınıf
    /// </summary>
    public class KHMGiderServisi
    {
        //static void KonsolideHesapla(TNSCollection dr, int kırılım)
        //{
        //    int sayi = dr.Count;
        //    double[,] Miktarlar = new double[sayi, kırılım];
        //    for (int i = dr.Count - 1; i >= 0; i--)
        //    {
        //        UcgenSatir us = (UcgenSatir)dr[i];

        //        for (int j = kırılım; j >= 0; j--)
        //        {
        //            if (j != us.yer)
        //            {
        //                for (int k = 0; k <= i; k++)
        //                {
        //                    UcgenSatir aranan = (UcgenSatir)dr[i - k];

        //                    if (aranan.yer == j)
        //                    {
        //                        Miktarlar[i - k, j] = Miktarlar[i - k, j] + Miktarlar[i, j];
        //                        k = i;
        //                    }
        //                }
        //            }
        //            else
        //                Miktarlar[i, j] = (double)us.ekDegerler[0];
        //        }


        //    }
        //}

        //private double OdenekGondermeHesapla(int yil, string[] kodlar)
        //{
        //    IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        //    OdenekDurumKriter kriter = new OdenekDurumKriter();
        //    kriter.yil = yil;
        //    kriter.tertip.kodlar = kodlar;
        //    kriter.harcamaGoster = false;
        //    kriter.odenekGondermeGoster = true;
        //    kriter.odemeEmriGoster = false;
        //    kriter.odenekGoster = false;
        //    kriter.konsolide = true;
        //    kriter.detay = ENUMOdenekDurumDetay.KONSOLIDE;

        //    ObjectArray oVAN = servisMUH.OdenekDurum(kriter);
        //    if (oVAN.objeler.Count > 0)
        //    {
        //        TNS.MUH.OdenekDurum o = (TNS.MUH.OdenekDurum)oVAN.objeler[0];
        //        return o.onayliOdenekGonderme - o.onayliTenkis;
        //    }
        //    else
        //        return 0;
        //}

        //private void YılsonuDevirIptalHesapla(int yil, string[] kodlar, double ysot, double kbo, double harcama, double devir, double akreditif, out double odenekUstu, out double yilSonuIptal)
        //{
        //    odenekUstu = 0;
        //    yilSonuIptal = 0;
        //    if (ysot < harcama)
        //    {
        //        odenekUstu = harcama - ysot;
        //        yilSonuIptal = 0;
        //        devir = 0;

        //    }
        //    else
        //    {
        //        if (akreditif > 0)
        //            yilSonuIptal = ysot - harcama - devir + akreditif;
        //        else
        //            yilSonuIptal = ysot - harcama - devir;

        //        if (yilSonuIptal < 0)
        //        {
        //            odenekUstu = yilSonuIptal * -1;
        //            yilSonuIptal = 0;
        //        }

        //    }
        //}

        //double DevirKonsolideGetir(VeriErisim veriErisim, int yil, string[] kodlar, ENUMFormDurumGrup durum)
        //{
        //    double donusDeger = 0.0;
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    VeriErisim veKesHes = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(null));
        //    TABButceIslemleri islGider = new TABButceIslemleri(ve);
        //    TABKHIslemler tabKHIslemler = new TABKHIslemler(veKesHes);
        //    string[] islemler = new string[4];
        //    islemler[0] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.AKREDITIF);
        //    islemler[1] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.TAAHHUTARTIGI);
        //    islemler[2] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.DEVREDENDISPROJE);
        //    islemler[3] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.DEVRENGELIRLI);
        //    donusDeger = islGider.DevirRakamiGetir(yil + 1, islemler, kodlar, durum);
        //    //donusDeger = islGider.DevirRakamiGetir(yil+1, "020103;020104;020105;020106", kodlar, durum);  

        //    donusDeger = donusDeger + tabKHIslemler.RakamGetir(yil, kodlar);

        //    ve.VeriErisimKapat();
        //    veKesHes.VeriErisimKapat();
        //    return donusDeger;
        //}

        //double AkreditifKonsolideGetir(VeriErisim veriErisim, int yil, string[] kodlar, ENUMFormDurumGrup durum)
        //{
        //    double donusDeger = 0.0;

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    VeriErisim veKesHes = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(null));
        //    TABButceIslemleri islGider = new TABButceIslemleri(ve);
        //    TABKHDevir tabKHDevir = new TABKHDevir(veKesHes);
        //    //donusDeger = islGider.DevirRakamiGetir(yil+1, "020103;020105;020106", kodlar, durum);         
        //    string[] islemlerAkreditif = new string[4];
        //    islemlerAkreditif[0] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.AKREDITIF);
        //    islemlerAkreditif[1] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.TAAHHUTARTIGI);
        //    islemlerAkreditif[2] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.DEVREDENDISPROJE);
        //    islemlerAkreditif[3] = BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.BAGISDEVRI);


        //    donusDeger = donusDeger + tabKHDevir.RakamGetir(yil, islemlerAkreditif, kodlar);
        //    // donusDeger = donusDeger + tabKHIslemler.RakamGetir(yil, (int)ENUMIslemKod.AKREDITIF, kodlar);

        //    ve.VeriErisimKapat();
        //    veKesHes.VeriErisimKapat();

        //    return donusDeger;
        //}

        //KHTertipBilgi ButceIslemleriSonuclariBulAylik(VeriErisim veriErisim, int yil, int ay, string[] kodlar, ENUMFormDurumGrup durum)
        //{
        //    KHTertipBilgi tBilgi = new KHTertipBilgi();
        //    int isaret;
        //    double deger;
        //    string islem = "";
        //    string kurum = "";

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    TABButceIslemleri islGider = new TABButceIslemleri(ve);
        //    IDataReader dr = islGider.ButceIslemleriSonuclariBulAylik(yil, ay, "02", kodlar, durum, "");
        //    if (dr != null)
        //    {
        //        while (dr.Read())
        //        {
        //            islem = VeriErisim.DegerAl(dr, 0, "");
        //            isaret = VeriErisim.DegerAl(dr, 1, 0);
        //            deger = VeriErisim.DegerAl(dr, 2, 0.0);
        //            kurum = VeriErisim.DegerAl(dr, 3, "");

        //            if (islem.StartsWith(BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.EKLEME).Replace(".", "")))
        //            {
        //                if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.AKREDITIF).Replace(".", ""))
        //                {
        //                    tBilgi.akreditifdevir = tBilgi.akreditifdevir + deger;
        //                }
        //                else if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.TAAHHUTARTIGI).Replace(".", ""))
        //                {
        //                    tBilgi.taahhutdevir = tBilgi.taahhutdevir + deger;
        //                }
        //                else if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.DEVREDENDISPROJE).Replace(".", ""))
        //                {
        //                    tBilgi.disprojedevir = tBilgi.disprojedevir + deger;
        //                }
        //                else if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.BAGISDEVRI).Replace(".", ""))
        //                {
        //                    if ((string)kodlar[2] == "8")//Bağış devri ise 8 olmalıdır.
        //                        tBilgi.bagisdevir = tBilgi.bagisdevir + deger;
        //                    else
        //                    {
        //                        OrtakFonksiyonlar.HataDosyaYaz("KesinHesap.txt", "Anlaşılmayan işlem kodu (" + islem + "), Ödenek Ekleme işlemi kabul edildi!", false);
        //                        tBilgi.odenekEkle = tBilgi.odenekEkle + deger;
        //                    }
        //                }
        //                else if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.DEVRENGELIRLI).Replace(".", ""))
        //                {
        //                    if ((string)kodlar[2] == "8")
        //                        tBilgi.bagisdevir = tBilgi.bagisdevir + deger;
        //                    else if ((string)kodlar[2] == "6")
        //                        tBilgi.ozeldevir = tBilgi.ozeldevir + deger;
        //                    else
        //                        tBilgi.digerdevir = tBilgi.digerdevir + deger;
        //                }
        //                else
        //                {
        //                    if ((string)kodlar[2] == "8" || (string)kodlar[2] == "6")
        //                        tBilgi.ozelOdenek = tBilgi.ozelOdenek + deger;
        //                    else
        //                        tBilgi.odenekEkle = tBilgi.odenekEkle + deger;
        //                }
        //            }
        //            else
        //            {
        //                if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.KURUMLARARASIAKTARMA).Replace(".", "") || islem == "020207")
        //                {
        //                    if (isaret > 0)
        //                        tBilgi.karaEk = tBilgi.karaEk + deger;
        //                    else
        //                        tBilgi.karaDus = tBilgi.karaDus - deger;
        //                }
        //                else if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.YEDEKTENAKTARMA).Replace(".", ""))
        //                {
        //                    if (isaret > 0)
        //                        tBilgi.yedekEk = tBilgi.yedekEk + deger;
        //                    else
        //                        tBilgi.yedekDus = tBilgi.yedekDus - deger;
        //                }
        //                else if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.YEDEGEAKTARMA).Replace(".", ""))
        //                {
        //                    if (isaret > 0)
        //                        tBilgi.karaEk = tBilgi.karaEk + deger;
        //                    else
        //                        tBilgi.karaDus = tBilgi.karaDus - deger;
        //                }
        //                else
        //                {
        //                    if (islem != BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.KURUMICIAKTARMA).Replace(".", ""))
        //                        OrtakFonksiyonlar.HataDosyaYaz("KesinHesap.txt", "Anlaşılmayan işlem kodu (" + islem + "), Kurum İçi Aktarma işlemi kabul edildi!", false);

        //                    if (isaret > 0)
        //                        tBilgi.kiciEk = tBilgi.kiciEk + deger;
        //                    else
        //                        tBilgi.kiciDus = tBilgi.kiciDus - deger;
        //                }
        //            }
        //        }
        //        dr.Close();
        //    }

        //    ve.VeriErisimKapat();

        //    return tBilgi;
        //}

        KHTertipBilgi ErtesiYilaDevirBul(VeriErisim veriErisim, int yil, int ay, string[] kodlar)
        {
            KHTertipBilgi tBilgi = new KHTertipBilgi();
            int isaret;
            double deger;
            string islem = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(null));
            TABKHDevir tabDevir = new TABKHDevir(ve);
            IDataReader dr = tabDevir.DevirIslemleriBul(yil, "02", kodlar, "");
            if (dr != null)
            {
                while (dr.Read())
                {
                    islem = (string)VeriErisim.DegerAl(dr, 0, "");
                    isaret = (int)VeriErisim.DegerAl(dr, 1, (double)0);
                    deger = (double)VeriErisim.DegerAl(dr, 2, (double)0);

                    if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.AKREDITIF).Replace(".", ""))
                    {
                        tBilgi.akreditifdevir = tBilgi.akreditifdevir + deger;

                    }
                    else if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.TAAHHUTARTIGI).Replace(".", ""))
                    {
                        tBilgi.taahhutdevir = tBilgi.taahhutdevir + deger;

                    }
                    else if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.DEVREDENDISPROJE).Replace(".", ""))
                    {
                        tBilgi.disprojedevir = tBilgi.disprojedevir + deger;

                    }
                    else if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.DEVRENGELIRLI).Replace(".", ""))
                    {
                        if ((string)kodlar[2] == "8")
                            tBilgi.bagisdevir = tBilgi.bagisdevir + deger;
                        else if ((string)kodlar[2] == "6")
                            tBilgi.ozeldevir = tBilgi.ozeldevir + deger;
                        else
                            tBilgi.digerdevir = tBilgi.digerdevir + deger;

                    }

                    //else if (islem == BISabitler.IslemKod(TNS.BUM.ENUMIslemKod.BAGISDEVRI).Replace(".", ""))
                    //{
                    //    if ((string)kodlar[2] == "8")
                    //        tBilgi.bagisdevir = tBilgi.bagisdevir + deger;                       
                    //}

                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            return tBilgi;
        }

        KHTertipBilgi DegerBul(VeriErisim veriErisim, int yil, int ay, string[] kodlar, string ekKosul)
        {
            KHTertipBilgi tBilgi = new KHTertipBilgi();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(null));
            TABGider islGider = new TABGider(ve);
            IDataReader dr = islGider.DegerBul(yil, ay, kodlar, ekKosul, false);
            if (dr != null)
            {
                while (dr.Read())
                {
                    tBilgi.bo = (double)VeriErisim.DegerAl(dr, 0, 0.0);
                    tBilgi.kbo = (double)VeriErisim.DegerAl(dr, 1, (double)0);
                    tBilgi.kiciEk = (double)VeriErisim.DegerAl(dr, 2, (double)0);
                    tBilgi.kiciDus = (double)VeriErisim.DegerAl(dr, 3, (double)0);
                    tBilgi.karaEk = (double)VeriErisim.DegerAl(dr, 4, (double)0);
                    tBilgi.karaDus = (double)VeriErisim.DegerAl(dr, 5, (double)0);
                    tBilgi.yedekEk = (double)VeriErisim.DegerAl(dr, 6, (double)0);
                    tBilgi.yedekDus = (double)VeriErisim.DegerAl(dr, 7, (double)0);
                    tBilgi.odenekEkle = (double)VeriErisim.DegerAl(dr, 8, (double)0);
                    tBilgi.ozelOdenek = (double)VeriErisim.DegerAl(dr, 9, (double)0);
                    tBilgi.yiliciIptal = (double)VeriErisim.DegerAl(dr, 10, (double)0);
                    tBilgi.harcama = (double)VeriErisim.DegerAl(dr, 11, (double)0);
                    tBilgi.odenekUstu = (double)VeriErisim.DegerAl(dr, 12, (double)0);
                    tBilgi.yilSonuIptal = (double)VeriErisim.DegerAl(dr, 13, (double)0);
                    tBilgi.devir = (double)VeriErisim.DegerAl(dr, 14, (double)0);
                    tBilgi.akreditif = (double)VeriErisim.DegerAl(dr, 15, (double)0);

                    tBilgi.akreditifdevir = (double)VeriErisim.DegerAl(dr, 16, (double)0);
                    tBilgi.taahhutdevir = (double)VeriErisim.DegerAl(dr, 17, (double)0);
                    tBilgi.disprojedevir = (double)VeriErisim.DegerAl(dr, 18, (double)0);
                    tBilgi.bagisdevir = (double)VeriErisim.DegerAl(dr, 19, (double)0);
                    tBilgi.ozeldevir = (double)VeriErisim.DegerAl(dr, 20, (double)0);
                    tBilgi.digerdevir = (double)VeriErisim.DegerAl(dr, 21, (double)0);
                    tBilgi.gonderilenodenek = tBilgi.harcama;// (double)VeriErisim.DegerAl(dr, 22, (double)0);Muhasebat yıl sonunda artan ödeneği otomatik tenkis yapıyor
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return tBilgi;
        }

        static string UcgenAciklamaBul(IUZYServis uzS, System.Collections.Hashtable hashTable, Kullanici kullanan, int yil, int kodNo, string uzayAd, string aciklamaKod)
        {

            string acik = "";
            if (kodNo != 0)
            {
                string anahtar = kodNo.ToString() + "-" + aciklamaKod + "-" + yil.ToString();
                acik = (string)hashTable[anahtar];
                if (acik == null)
                {
                    acik = uzS.UzayDegeriStr(kullanan, uzayAd, yil.ToString() + "-" + aciklamaKod, true);
                    if (acik == "")
                        acik = uzS.UzayDegeriStr(kullanan, uzayAd, (yil - 1).ToString() + "-" + aciklamaKod, true);
                    if (acik == "")
                        acik = uzS.UzayDegeriStr(kullanan, uzayAd, (yil - 2).ToString() + "-" + aciklamaKod, true);
                    hashTable.Add(anahtar, acik);
                }
            }
            else
            {
                acik = uzS.UzayDegeriStr(kullanan, uzayAd, yil.ToString() + "-" + aciklamaKod, true);
                if (acik == "")
                    acik = uzS.UzayDegeriStr(kullanan, uzayAd, (yil - 1).ToString() + "-" + aciklamaKod, true);
                if (acik == "")
                    acik = uzS.UzayDegeriStr(kullanan, uzayAd, (yil - 2).ToString() + "-" + aciklamaKod, true);
            }
            return acik;
        }

        private KHOdenekBilgi OdenekHarcamaBul(VeriErisim veriErisim, int yil, int ay, string[] kodlar, string ekKosul)
        {
            KHOdenekBilgi donusdeger = new KHOdenekBilgi();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            //VeriErisim veHarcama = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM");
            TABButce tabButce = new TABButce(ve);
            TABHarcama tabHarcama = new TABHarcama(ve);
            donusdeger.ysot = tabButce.YSOTBul(yil, kodlar, false, ekKosul);
            donusdeger.kbo = tabButce.kboBul(yil, kodlar, ekKosul);
            donusdeger.harcama = tabHarcama.HarcamaAylikOdenek(yil, ay, ekKosul, kodlar, false);
            if (yil < 2005)
                donusdeger.harcama = donusdeger.harcama / 1000;
            ve.VeriErisimKapat();
            //veHarcama.VeriErisimKapat();
            return donusdeger;
        }

        /// <summary>
        /// Devir cetveli raporunu hazırlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapDevirCetveliHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip)
        {
            TNSCollection donusDeger = new TNSCollection();
            VeriErisim veListe = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));

            IUZYServis uzS = TNS.Api.ServisIstemci.Olustur<IUZYServis>(System.Configuration.ConfigurationManager.AppSettings.Get("UZYServisURL"));
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            int kirilimSay = 11;
            tertip.NoktalariAt();

            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && (string)tertip.kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                if (!(ay > 0))
                    ay = 12;

                string kurum = (string)tertip.kodlar[0];
                //kurum = Tertip.KurumsaldanKuruma(kurum);

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {

                    TABKHDevir tabDevir = new TABKHDevir(veListe);
                    //TABGider tabGiderDeger = new TABGider(ve);                  
                    IDataReader dr = tabDevir.TertipListele(yil, -1, tertip.kodlar, "");

                    if (dr != null)
                    {
                        System.Collections.Hashtable hashTable = new System.Collections.Hashtable();
                        string[] kistaslar = new string[4];
                        string[] eskiDegerler = new string[kirilimSay];
                        string[] yeniDegerler = new string[kirilimSay];
                        string[] degerTablo = new string[4];
                        degerTablo[0] = "ABSKUR";
                        degerTablo[1] = "ABSFON";
                        degerTablo[2] = "ABSFIN";
                        degerTablo[3] = "ABSEKO";
                        string ekKosul = "";
                        ekKosul = GenelKosul.EkKosulOlustur(tertip.kodlar);

                        for (int i = 0; i < kirilimSay; i++) eskiDegerler[i] = "x";
                        string tut;

                        while (dr.Read())
                        {
                            int yer = 0;
                            Tertip tertipListe = new Tertip(yil, ENUMTabloKod.TABButce);
                            tertipListe.kodlar[0] = (VeriErisim.DegerAl(dr, 0, ""));
                            tertipListe.kodlar[1] = (VeriErisim.DegerAl(dr, 1, ""));
                            tertipListe.kodlar[2] = (VeriErisim.DegerAl(dr, 2, ""));
                            tertipListe.kodlar[3] = (VeriErisim.DegerAl(dr, 3, ""));

                            for (int i = 0; i < tertip.kodlar.Length; i++)
                                kistaslar[i] = "";
                            for (int i = 0; i < tertipListe.kodlar.Length; i++)
                            {
                                tut = OrtakFonksiyonlar.KodNoktaKoy((string)tertipListe.Kirilimlar(i), (string)tertipListe.kodlar[i]);
                                kistaslar[0] = "";
                                ObjectArray d = new ObjectArray();
                                OrtakFonksiyonlar.StringiParcala(tut, ".", d);
                                for (int j = 0; j < d.objeler.Count; j++)
                                {
                                    yeniDegerler[yer++] = (string)d.objeler[j];

                                }
                            }
                            int kodyer = 0;
                            string aciklamaKod = "";
                            for (int j = 0; j < yer; j++)
                            {
                                if (tertipListe.kodlar[kodyer].ToString() != kistaslar[kodyer].ToString())
                                    kistaslar[kodyer] = (string)kistaslar[kodyer] + (string)yeniDegerler[j];
                                else
                                {
                                    kistaslar[kodyer + 1] = (string)kistaslar[kodyer + 1] + (string)yeniDegerler[j];
                                    kodyer++;
                                }
                                if (eskiDegerler[j] != yeniDegerler[j])
                                {
                                    UcgenSatir us = new UcgenSatir();
                                    us.yer = j;
                                    aciklamaKod = (string)kistaslar[kodyer];
                                    us.aciklama = UcgenAciklamaBul(uzS, hashTable, kullanan, yil, kodyer, degerTablo[kodyer], aciklamaKod);
                                    us.kodParca = yeniDegerler[j];
                                    us.ekDegerler.Add(ErtesiYilaDevirBul(ve, yil, ay, kistaslar));
                                    donusDeger.Add(us);

                                    for (int k = j + 1; k < yer; k++)
                                        eskiDegerler[k] = "";
                                }
                            }
                            for (int j = 0; j < yer; j++)
                            {
                                eskiDegerler[j] = yeniDegerler[j];
                            }


                        }

                        dr.Close();
                    }


                }
                else
                    donusDeger.Add(new Sonuc(false, "Bu bilgileri görme yetkiniz yok", ""));

            }
            veListe.VeriErisimKapat();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap gider bilgilerini hesaplayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="khBilgiler">kaydedilecek TertipBilgi nesnelerini içeren koleksiyon.</param>
        /// <returns></returns>
        public static Sonuc KesinHesapGiderBilgileriKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, TNSCollection khBilgiler)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (yil < 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Yıl bilgisi boş olamaz.";
                return sonuc;
            }
            VeriErisim veKesinHesap = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            string kurum =  TNS.DEG.Arac.VarsayilanKurumBul();
            int ay = 12;
            if (KHMYetkiServisi.KesinHesapOlusturmaYetkisiVarMi(kullanan, yil, kurum))
            {
                Hesapla(khBilgiler);
                TABGider tabGider = new TABGider(veKesinHesap);

                veKesinHesap.BeginTransaction();
                foreach (TertipBilgi tertipBilgi in khBilgiler)
                {
                    sonuc = tabGider.KesinHesapBilgileriSil(yil, ay, tertipBilgi.tertip.kodlar, "");
                    if (sonuc.islemSonuc)
                    {
                        sonuc = tabGider.KesinHesapBilgileriKaydet(yil, ay, tertipBilgi.tertip.kodlar, tertipBilgi.tertipBilgi.bo, tertipBilgi.tertipBilgi.kbo, tertipBilgi.tertipBilgi.kiciEk, tertipBilgi.tertipBilgi.kiciDus, tertipBilgi.tertipBilgi.karaEk, tertipBilgi.tertipBilgi.karaDus, tertipBilgi.tertipBilgi.yedekEk, tertipBilgi.tertipBilgi.yedekDus, tertipBilgi.tertipBilgi.odenekEkle, tertipBilgi.tertipBilgi.ozelOdenek, tertipBilgi.tertipBilgi.yiliciIptal, tertipBilgi.tertipBilgi.harcama, tertipBilgi.tertipBilgi.odenekUstu, tertipBilgi.tertipBilgi.yilSonuIptal, tertipBilgi.tertipBilgi.devir, tertipBilgi.tertipBilgi.akreditif, tertipBilgi.tertipBilgi.akreditifdevir, tertipBilgi.tertipBilgi.taahhutdevir, tertipBilgi.tertipBilgi.disprojedevir, tertipBilgi.tertipBilgi.bagisdevir, tertipBilgi.tertipBilgi.ozeldevir, tertipBilgi.tertipBilgi.digerdevir, tertipBilgi.tertipBilgi.gonderilenodenek);

                        if (!sonuc.islemSonuc)
                            break;
                    }
                }

                if (sonuc.islemSonuc)
                    veKesinHesap.Commit();
                else
                    veKesinHesap.Rollback();
            }
            else
                sonuc.hataStr = "Bu işlemi yapmaya yetkiniz bulunmamaktadır.";

            veKesinHesap.VeriErisimKapat();
            return sonuc;
        }

        private static void Hesapla(TNSCollection khBilgiler)
        {
            foreach(TertipBilgi tertipBilgi in khBilgiler)
            {
                double devir1 = tertipBilgi.tertipBilgi.akreditifdevir + tertipBilgi.tertipBilgi.taahhutdevir + tertipBilgi.tertipBilgi.disprojedevir + tertipBilgi.tertipBilgi.digerdevir;
                double devir2 = tertipBilgi.tertipBilgi.bagisdevir + tertipBilgi.tertipBilgi.ozeldevir;
                double ysot = tertipBilgi.tertipBilgi.kbo + tertipBilgi.tertipBilgi.kiciEk + tertipBilgi.tertipBilgi.karaEk + tertipBilgi.tertipBilgi.odenekEkle + tertipBilgi.tertipBilgi.ozelOdenek + tertipBilgi.tertipBilgi.yedekEk - tertipBilgi.tertipBilgi.kiciDus - tertipBilgi.tertipBilgi.karaDus - tertipBilgi.tertipBilgi.yiliciIptal - tertipBilgi.tertipBilgi.yedekDus + devir1 + devir2;

                YilSonuDevirIptalHesapla(ysot, tertipBilgi.tertipBilgi.kbo, tertipBilgi.tertipBilgi.harcama, tertipBilgi.tertipBilgi.devir, tertipBilgi.tertipBilgi.akreditif, out tertipBilgi.tertipBilgi.odenekUstu, out tertipBilgi.tertipBilgi.yilSonuIptal);
                //KHTertipBilgi tertipBilgiDevir = new KHTertipBilgi();
                //tertipBilgiDevir = ErtesiYilaDevirBul(ve, yil, ay, tertipBilgi.tertip.kodlar);
                //tertipBilgi.tertipBilgi.akreditifdevir = tertipBilgiDevir.akreditifdevir;
                //tertipBilgi.tertipBilgi.bagisdevir = tertipBilgiDevir.bagisdevir;
                //tertipBilgi.tertipBilgi.disprojedevir = tertipBilgiDevir.disprojedevir;
                //tertipBilgi.tertipBilgi.taahhutdevir = tertipBilgiDevir.taahhutdevir;
                //tertipBilgi.tertipBilgi.digerdevir = tertipBilgiDevir.digerdevir;
            }
        }

        private static void YilSonuDevirIptalHesapla(double ysot, double kbo, double harcama, double devir, double akreditif, out double odenekUstu, out double yilSonuIptal)
        {
            odenekUstu = 0;
            if (ysot < harcama)
            {
                odenekUstu = harcama - ysot;
                yilSonuIptal = 0;
            }
            else
            {
                if (akreditif > 0)
                    yilSonuIptal = ysot - harcama - devir + akreditif;
                else
                    yilSonuIptal = ysot - harcama - devir;

                if (yilSonuIptal < 0)
                {
                    odenekUstu = yilSonuIptal * -1;
                    yilSonuIptal = 0;
                }
            }
        }

        /// <summary>
        /// Kesin hesap gider cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapGiderCetveliHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip)
        {
            return KesinHesapGiderCetveliHazirla(veriErisim, kullanan, yil, ay, tertip, false, ENUMIslemKod.TANIMSIZ);
        }

        /// <summary>
        /// Kesin hesap gider cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="kurumsal">Eğer <c>true</c> kurumsal bazda liste alınır.</param>
        /// <param name="raporTur">Rapor tür bilgisi.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapGiderCetveliHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip, bool kurumsal, ENUMIslemKod raporTur)
        {
            TNSCollection donusDeger = new TNSCollection();
            VeriErisim veListe = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));

            IUZYServis uzS = TNS.Api.ServisIstemci.Olustur<IUZYServis>(System.Configuration.ConfigurationManager.AppSettings.Get("UZYServisURL"));

            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }
            string kosul = "";
            int kirilimSay = 11;
            if (kurumsal)
                kirilimSay = 4;
            tertip.NoktalariAt();

            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && (string)tertip.kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                if (!(ay > 0))
                    ay = 12;

                string kurum = (string)tertip.kodlar[0];
                //kurum = Tertip.KurumsaldanKuruma(kurum);

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {

                    TABGider tabGider = new TABGider(veListe);
                    //TABGider tabGiderDeger = new TABGider(ve);
                    kosul = tabGider.ekKosulOl((int)raporTur);
                    IDataReader dr = tabGider.TertipListele(yil, ay, tertip.kodlar, kosul);

                    if (dr != null)
                    {
                        System.Collections.Hashtable hashTable = new System.Collections.Hashtable();
                        string[] kistaslar = new string[4];
                        string[] eskiDegerler = new string[kirilimSay];
                        string[] yeniDegerler = new string[kirilimSay];
                        string[] degerTablo = new string[4];
                        degerTablo[0] = "ABSKUR";
                        degerTablo[1] = "ABSFON";
                        degerTablo[2] = "ABSFIN";
                        degerTablo[3] = "ABSEKO";
                        string ekKosul = "";
                        ekKosul = GenelKosul.EkKosulOlustur(tertip.kodlar);

                        for (int i = 0; i < kirilimSay; i++) eskiDegerler[i] = "x";
                        string tut;

                        while (dr.Read())
                        {
                            int yer = 0;
                            Tertip tertipListe = new Tertip(yil, ENUMTabloKod.TABButce);
                            tertipListe.kodlar[0] = VeriErisim.DegerAl(dr, 0, "");
                            if (!kurumsal)
                            {
                                tertipListe.kodlar[1] = VeriErisim.DegerAl(dr, 1, "");
                                tertipListe.kodlar[2] = VeriErisim.DegerAl(dr, 2, "");
                                tertipListe.kodlar[3] = VeriErisim.DegerAl(dr, 3, "");
                            }
                            if (tertip != null)
                            {

                                for (int i = 0; i < tertip.kodlar.Length; i++)
                                    kistaslar[i] = "";
                            }
                            for (int i = 0; i < tertipListe.kodlar.Length; i++)
                            {
                                tut = OrtakFonksiyonlar.KodNoktaKoy((string)tertipListe.Kirilimlar(i), (string)tertipListe.kodlar[i]);
                                kistaslar[0] = "";
                                ObjectArray d = new ObjectArray();
                                OrtakFonksiyonlar.StringiParcala(tut, ".", d);
                                for (int j = 0; j < d.objeler.Count; j++)
                                {
                                    yeniDegerler[yer++] = (string)d.objeler[j];
                                }
                            }
                            int kodyer = 0;
                            string aciklamaKod = "";
                            for (int j = 0; j < yer; j++)
                            {
                                if (tertipListe.kodlar[kodyer].ToString() != kistaslar[kodyer].ToString())
                                    kistaslar[kodyer] = (string)kistaslar[kodyer] + (string)yeniDegerler[j];
                                else
                                {
                                    kistaslar[kodyer + 1] = (string)kistaslar[kodyer + 1] + (string)yeniDegerler[j];
                                    kodyer++;
                                }
                                if (eskiDegerler[j] != yeniDegerler[j])
                                {
                                    UcgenSatir us = new UcgenSatir();
                                    us.yer = j;
                                    aciklamaKod = (string)kistaslar[kodyer];
                                    us.aciklama = UcgenAciklamaBul(uzS, hashTable, kullanan, yil, kodyer, degerTablo[kodyer], aciklamaKod);
                                    us.kodParca = yeniDegerler[j];
                                    us.ekDegerler.Add(DegerBul(ve, yil, ay, kistaslar, ekKosul));
                                    donusDeger.Add(us);

                                    for (int k = j + 1; k < yer; k++)
                                        eskiDegerler[k] = "";
                                }
                            }
                            for (int j = 0; j < yer; j++)
                            {
                                eskiDegerler[j] = yeniDegerler[j];
                            }
                        }

                        UcgenSatir usT = new UcgenSatir();
                        usT.yer = 0;
                        usT.aciklama = "                                         GENEL TOPLAM";
                        usT.kodParca = "";
                        usT.ekDegerler.Add(DegerBul(ve, yil, ay, new string[0], ekKosul));
                        donusDeger.Add(usT);

                        dr.Close();
                    }
                }
                else
                    donusDeger.Add(new Sonuc(false, "Bu bilgileri görme yetkiniz yok", ""));
            }
            veListe.VeriErisimKapat();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap fonksiyonel cetvelibi hazirlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="tur">Tür bilgisi.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapFonksiyonelCetveliHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip, int tur)
        {
            TNSCollection donusDeger = new TNSCollection();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                ve.VeriErisimKapat();
                return donusDeger;
            }


            tertip.NoktalariAt();

            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && (string)tertip.kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                if (!(ay > 0))
                    ay = 12;

                string kurum = (string)tertip.kodlar[0];
                //kurum = Tertip.KurumsaldanKuruma(kurum);

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {
                    string[] kodlar = new string[4];
                    string ekKosul = "";
                    kodlar[0] = "";
                    kodlar[1] = "";
                    kodlar[2] = "";
                    kodlar[3] = "";


                    for (int i = 0; i <= 12; i++)
                    {
                        if (tur == 2)
                            kodlar[tur] = i.ToString();
                        else
                            kodlar[tur] = i.ToString("00");
                        ekKosul = GenelKosul.EkKosulOlustur(kodlar);
                        donusDeger.Add(DegerBul(ve, yil, ay, tertip.kodlar, ekKosul));
                    }
                }
            }
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap kod cetvelini hazırlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="tur">Tür bilgisi.</param>
        /// <param name="seviye">seviye bilgisi.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapKodCetvelHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip, string tur, int seviye)
        {
            TNSCollection donusDeger = new TNSCollection();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            tertip.NoktalariAt();

            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && (string)tertip.kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                if (!(ay > 0))
                    ay = 12;

                string kurum = (string)tertip.kodlar[0];
                //kurum = Tertip.KurumsaldanKuruma(kurum);

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {
                    //TNSCollection kodlar = new TNSCollection();
                    IBKDServis servisBKD = TNS.BKD.Arac.Tanimla();
                    ABSKodlarVAN absv = servisBKD.KodListeleSeviye(yil, tur, "", seviye);
                    string ekKosul = "";

                    int yer = Convert.ToInt32(tur);
                    foreach (ABSKod kod in absv.absKodlar)
                    {
                        TertipBilgi tb = new TertipBilgi();
                        tb.tertip = new Tertip(yil, ENUMTabloKod.TABButce);
                        for (int k = 0; k < 4; k++)
                            tb.tertip.kodlar[k] = "";
                        tb.tertip.kodlar[yer - 1] = kod.kod;
                        ekKosul = GenelKosul.EkKosulOlustur(tb.tertip.kodlar);
                        tb.tertipBilgi = DegerBul(ve, yil, ay, tertip.kodlar, ekKosul);
                        tb.aciklama = kod.Ad();
                        donusDeger.Add(tb);
                    }
                    TertipBilgi tbg = new TertipBilgi();
                    tbg.tertip = new Tertip(yil, ENUMTabloKod.TABButce);
                    tbg.tertipBilgi = DegerBul(ve, yil, ay, tertip.kodlar, "");
                    for (int k = 0; k < 4; k++)
                        tbg.tertip.kodlar[k] = "";
                    tbg.aciklama = "GENEL TOPLAM";
                    donusDeger.Add(tbg);

                }
            }
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap Çapraz cetvel hazirlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapCaprazCetvelHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip)
        {
            TNSCollection donusDeger = new TNSCollection();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            tertip.NoktalariAt();

            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && (string)tertip.kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                if (!(ay > 0))
                    ay = 12;

                string kurum = (string)tertip.kodlar[0];
                //kurum = Tertip.KurumsaldanKuruma(kurum);

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {

                    IBKDServis servisBKD = TNS.BKD.Arac.Tanimla();
                    ABSKodlarVAN absvfon = servisBKD.KodListeleSeviye(yil, "2", "", 1);
                    ABSKodlarVAN absveko = servisBKD.KodListeleSeviye(yil, "4", "", 1);
                    string ekKosul = "";


                    foreach (ABSKod kodfon in absvfon.absKodlar)
                    {
                        foreach (ABSKod kodeko in absveko.absKodlar)
                        {
                            TertipBilgi tb = new TertipBilgi();
                            tb.tertip = new Tertip(yil, ENUMTabloKod.TABOdenek);
                            for (int k = 0; k < 4; k++)
                                tb.tertip.kodlar[k] = "";
                            tb.tertip.kodlar[1] = kodfon.kod;
                            tb.tertip.kodlar[3] = kodeko.kod;
                            ekKosul = GenelKosul.EkKosulOlustur(tb.tertip.kodlar);
                            tb.tertipBilgi = DegerBul(ve, yil, ay, tertip.kodlar, ekKosul);
                            donusDeger.Add(tb);
                        }
                    }


                }
            }
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap mahsup cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="tur">Tür bilgisi.</param>
        /// <param name="seviye">Seviye bilgisi.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapMahsupCetveliHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip, string tur, int seviye)
        {
            TNSCollection donusDeger = new TNSCollection();
            VeriErisim veharcama = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim veMahsup = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            tertip.NoktalariAt();

            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && (string)tertip.kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                if (!(ay > 0))
                    ay = 12;

                string kurum = (string)tertip.kodlar[0];
                //kurum = Tertip.KurumsaldanKuruma(kurum);

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {
                    //TNSCollection kodlar = new TNSCollection();
                    TABHarcama tabHarcama = new TABHarcama(veharcama);
                    TABHarcamaMahsup tabMahsup = new TABHarcamaMahsup(veMahsup);
                    IBKDServis servisBKD = TNS.BKD.Arac.Tanimla();
                    ABSKodlarVAN absv = servisBKD.KodListeleSeviye(yil, tur, "", seviye);
                    string ekKosul = "";

                    int yer = Convert.ToInt32(tur);
                    foreach (ABSKod kod in absv.absKodlar)
                    {
                        TertipHarcamaBilgi tb = new TertipHarcamaBilgi(yil);

                        for (int k = 0; k < 4; k++)
                            tb.tertip.kodlar[k] = "";
                        tb.tertip.kodlar[yer - 1] = kod.kod;
                        ekKosul = GenelKosul.EkKosulOlustur(tb.tertip.kodlar);
                        tb.harcama = tabHarcama.HarcamaAylikOdenek(yil, ay, ekKosul, tb.tertip.kodlar, false);
                        tb.mahsup = tabMahsup.HarcamaAylikOdenek(yil, ay, ekKosul, tertip.kodlar, true);
                        tb.aciklama = kod.Ad();
                        donusDeger.Add(tb);
                    }
                    /*
                    TertipHarcamaBilgi tbg = new TertipHarcamaBilgi();
                    tbg.tertipBilgi = tabHarcama.HarcamaAylikOdenek(yil, ay, tertipBilgi.tertip.kodlar);
                    for (int k = 0; k < 4; k++)
                        tbg.tertip.kodlar.Add("");
                    tbg.aciklama = "GENEL TOPLAM";
                    donusDeger.Add(tbg);
                    */
                }
            }
            veMahsup.VeriErisimKapat();
            veharcama.VeriErisimKapat();


            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap Gider Çok yıllı raporu hazırlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="tur">Tür.</param>
        /// <param name="seviye">Seviye bilgisi.</param>
        /// <param name="kacYil">Kaç yıllık işlem yapılacağı bilgisi.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapCokYilliCetvelHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip, string tur, int seviye, int kacYil)
        {
            TNSCollection donusDeger = new TNSCollection();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            tertip.NoktalariAt();

            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && (string)tertip.kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                if (!(ay > 0))
                    ay = 12;

                string kurum = (string)tertip.kodlar[0];
                //kurum = Tertip.KurumsaldanKuruma(kurum);

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {
                    //TNSCollection kodlar = new TNSCollection();
                    IBKDServis servisBKD = TNS.BKD.Arac.Tanimla();
                    ABSKodlarVAN absv = servisBKD.KodListeleSeviye(yil, tur, "", seviye);
                    string ekKosul = "";

                    int yer = Convert.ToInt32(tur);
                    foreach (ABSKod kod in absv.absKodlar)
                    {
                        TertipBilgiler tb = new TertipBilgiler();
                        tb.tertip = new Tertip(yil, ENUMTabloKod.TABButce);
                        for (int k = 0; k < 4; k++)
                            tb.tertip.kodlar[k] = "";
                        tb.tertip.kodlar[yer - 1] = kod.kod;
                        ekKosul = GenelKosul.EkKosulOlustur(tb.tertip.kodlar);
                        for (int i = 0; i <= kacYil; i++)
                            tb.degerler.Add(OdenekHarcamaBul(ve, yil - kacYil + i, ay, tertip.kodlar, ekKosul));
                        tb.aciklama = kod.Ad();
                        donusDeger.Add(tb);
                    }
                    TertipBilgiler tbg = new TertipBilgiler();
                    tbg.tertip = new Tertip(yil, ENUMTabloKod.TABButce);
                    for (int i = 0; i <= kacYil; i++)
                        tbg.degerler.Add(OdenekHarcamaBul(ve, yil - kacYil + i, ay, tertip.kodlar, ""));
                    for (int k = 0; k < 4; k++)
                        tbg.tertip.kodlar[k] = "";
                    tbg.aciklama = "GENEL TOPLAM";
                    donusDeger.Add(tbg);

                }
            }
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Gider Tertip deger bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public ObjectArray GiderTertipDegerListele(Kullanici kullanan, int yil, Tertip tertip)
        {
            ObjectArray donenDeger = new ObjectArray();
            tertip.NoktalariAt();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGider tabGider = new TABGider(ve);
            IDataReader dr = tabGider.DegerBul(yil, 12, tertip.kodlar, "", true);

            if (dr != null)
            {
                while (dr.Read())
                {
                    TertipBilgi tBilgi = new TertipBilgi();

                    tBilgi.tertipBilgi.bo = VeriErisim.DegerAl(dr, 0, 0.0);
                    tBilgi.tertipBilgi.kbo = VeriErisim.DegerAl(dr, 1, 0.0);
                    tBilgi.tertipBilgi.kiciEk = VeriErisim.DegerAl(dr, 2, 0.0);
                    tBilgi.tertipBilgi.kiciDus = VeriErisim.DegerAl(dr, 3, 0.0);
                    tBilgi.tertipBilgi.karaEk = VeriErisim.DegerAl(dr, 4, 0.0);
                    tBilgi.tertipBilgi.karaDus = VeriErisim.DegerAl(dr, 5, 0.0);
                    tBilgi.tertipBilgi.yedekEk = VeriErisim.DegerAl(dr, 6, 0.0);
                    tBilgi.tertipBilgi.yedekDus = VeriErisim.DegerAl(dr, 7, 0.0);
                    tBilgi.tertipBilgi.odenekEkle = VeriErisim.DegerAl(dr, 8, 0.0);
                    tBilgi.tertipBilgi.ozelOdenek = VeriErisim.DegerAl(dr, 9, 0.0);
                    tBilgi.tertipBilgi.yiliciIptal = VeriErisim.DegerAl(dr, 10, 0.0);
                    tBilgi.tertipBilgi.harcama = VeriErisim.DegerAl(dr, 11, 0.0);
                    tBilgi.tertipBilgi.odenekUstu = VeriErisim.DegerAl(dr, 12, 0.0);
                    tBilgi.tertipBilgi.yilSonuIptal = VeriErisim.DegerAl(dr, 13, 0.0);
                    tBilgi.tertipBilgi.devir = VeriErisim.DegerAl(dr, 14, 0.0);
                    tBilgi.tertipBilgi.akreditif = VeriErisim.DegerAl(dr, 15, 0.0);
                    tBilgi.tertipBilgi.akreditifdevir = VeriErisim.DegerAl(dr, 16, 0.0);
                    tBilgi.tertipBilgi.taahhutdevir = VeriErisim.DegerAl(dr, 17, 0.0);
                    tBilgi.tertipBilgi.disprojedevir = VeriErisim.DegerAl(dr, 18, 0.0);
                    tBilgi.tertipBilgi.bagisdevir = VeriErisim.DegerAl(dr, 19, 0.0);
                    tBilgi.tertipBilgi.ozeldevir = VeriErisim.DegerAl(dr, 20, 0.0);
                    tBilgi.tertipBilgi.digerdevir = VeriErisim.DegerAl(dr, 21, 0.0);
                    tBilgi.tertipBilgi.gonderilenodenek = VeriErisim.DegerAl(dr, 22, 0.0);
                    string kur = VeriErisim.DegerAl(dr, 23, "");
                    string fon = VeriErisim.DegerAl(dr, 24, "");
                    string fin = VeriErisim.DegerAl(dr, 25, "");
                    string eko = VeriErisim.DegerAl(dr, 26, "");
                    tBilgi.tertip = new Tertip(yil, ENUMTabloKod.TABButce);
                    tBilgi.tertip.TertipAta(kur + "-" + fon + "-" + fin + "-" + eko);
                    donenDeger.objeler.Add(tBilgi);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return donenDeger;
        }

        /// <summary>
        /// Gider tertiplerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public ObjectArray GiderTertipListele(Kullanici kullanan, int yil, Tertip tertip)
        {
            ObjectArray tertipler = new ObjectArray();
            tertip.NoktalariAt();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGider tabGider = new TABGider(ve);
            IDataReader dr = tabGider.TertipListele(yil, -1, tertip.kodlar, "");

            if (dr != null)
            {
                while (dr.Read())
                {
                    Tertip tertipListe = new Tertip(yil, ENUMTabloKod.TABButce);
                    tertipListe.kodlar[0] = (VeriErisim.DegerAl(dr, 0, ""));
                    tertipListe.kodlar[1] = (VeriErisim.DegerAl(dr, 1, ""));
                    tertipListe.kodlar[2] = (VeriErisim.DegerAl(dr, 2, ""));
                    tertipListe.kodlar[3] = (VeriErisim.DegerAl(dr, 3, ""));

                    tertipListe.Noktala();

                    if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, tertipListe.kodlar[0]))
                        tertipler.objeler.Add(tertipListe);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return tertipler;
        }
    }
}

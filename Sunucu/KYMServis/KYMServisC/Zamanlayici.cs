using System;
using System.Collections.Generic;
using System.Timers;

namespace TNS.KYM.IsKurallari
{
    public static class Zamanlayici
    {
        static Timer yatirimEPostaKontrolTimer;
        static Timer tcmbTasinirMuhasebeIslemKontrolTimer;

        public static void Start()
        {
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                if (tcmbTasinirMuhasebeIslemKontrolTimer != null && tcmbTasinirMuhasebeIslemKontrolTimer.Enabled) return;

                tcmbTasinirMuhasebeIslemKontrolTimer = new Timer(1 * 10 * 1 * 1000);//10sn
                tcmbTasinirMuhasebeIslemKontrolTimer.Elapsed += new ElapsedEventHandler(TCMBTasinirMuhasebeIslemKontrolTimer_Elapsed);
                tcmbTasinirMuhasebeIslemKontrolTimer.Enabled = true;
            }

            //if (TNS.KYM.Arac.ModulKullaniliyorMu("TRM"))
            //{
            //    if (yatirimEPostaKontrolTimer != null && yatirimEPostaKontrolTimer.Enabled) return;

            //    yatirimEPostaKontrolTimer = new Timer(1 * 10 * 1 * 1000);//10sn
            //    yatirimEPostaKontrolTimer.Elapsed += new ElapsedEventHandler(YatirimEPostaKontrolTimer_Elapsed);
            //    yatirimEPostaKontrolTimer.Enabled = true;
            //}
        }

        #region TCMBTasinirMuhasebeIslemKontrolTimer_Elapsed
        static void TCMBTasinirMuhasebeIslemKontrolTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            tcmbTasinirMuhasebeIslemKontrolTimer.Interval = 1 * 60 * 60 * 1000;//1 saat
            tcmbTasinirMuhasebeIslemKontrolTimer.Enabled = false;
            string islemAdi = "TCMBTasinirMuhasebeIslemKontrol";
            lock (islemAdi)
            {
                string kontrol = Kontrol(islemAdi, ENUMZamanlayiciTur.DAKIKALIK.ToString(), DateTime.Now.DayOfWeek, 0);
                if (string.IsNullOrEmpty(kontrol))
                {
                    TNS.TMM.ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

                    Kullanici kullanan = new Kullanici();
                    kullanan.kullaniciKodu = "SISTEM";
                    kullanan.mernis = "SISTEM";
                    kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.SISTEMYONETICISI);

                    //servisTMM.MuhasebeBilgiDurumSorgulaVeGuncelle(kullanan, null);
                    servisTMM.MuhasebeServisineGonder(kullanan, null);
                }
            }
            tcmbTasinirMuhasebeIslemKontrolTimer.Enabled = true;
        }
        #endregion

        #region YatirimEPostaKontrolTimer_Elapsed
        static void YatirimEPostaKontrolTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            yatirimEPostaKontrolTimer.Enabled = false;

            //yatirimEPostaKontrolTimer.Interval = 1 * 60 * 60 * 1000;//Saat başı
            //yatirimEPostaKontrolTimer.Enabled = false;
            //string islemAdi = "YatirimEPostaKontrol";
            //lock (islemAdi)
            //{
            //    string kontrol = Kontrol(islemAdi, ENUMZamanlayiciTur.SAATLIK.ToString(), DayOfWeek.Monday, 0);
            //    if (string.IsNullOrEmpty(kontrol))
            //        new TRMEPostaIslemleri().IMAPMailleriListele();
            //}
            //yatirimEPostaKontrolTimer.Enabled = true;
        }
        #endregion

        static string Kontrol(string islemAdi, string zamanlamaTuru, DayOfWeek gunAdi, int calismaSaati)
        {
            //Bu metod birden fazla worker processin aynı işlemi birden fazla yapmaması için yazılmıştır.
            //İşlemin zamanlanası (2 saatte bir veya 5 dakikada bir) timer nesnesi üzerinden yapılacaktır.
            //Bu metod ayrıca haftalık veya günlük yapılacak işlemlerinde çalışmasını sağlar. Sadece pazar günleri saat 17 de çalış gibi

            //OrtakFonksiyonlar.HataStrYaz(islemAdi);

            //Haftalık
            if (zamanlamaTuru == ENUMZamanlayiciTur.HAFTALIK.ToString() && gunAdi != DateTime.Now.DayOfWeek) return "1";
            if (zamanlamaTuru == ENUMZamanlayiciTur.HAFTALIK.ToString() && calismaSaati != DateTime.Now.Hour) return "2";

            //Günlük
            if (zamanlamaTuru == ENUMZamanlayiciTur.GUNLUK.ToString() && calismaSaati != DateTime.Now.Hour) return "3";

            ZamanlayiciTip z = new ZamanlayiciTip();
            z.islemAdi = islemAdi;
            z.tur = zamanlamaTuru;
            ObjectArray bilgiler = TNS.KYM.IsKurallari.KYMZamanlayici.Listele(null, z);

            if (bilgiler.sonuc.islemSonuc && bilgiler.objeler.Count > 0)
            {
                List<object> liste = new List<object>();

                foreach (ZamanlayiciTip item in bilgiler.objeler)
                {
                    if (item.tur == ENUMZamanlayiciTur.HAFTALIK.ToString())
                    {
                        if (item.zaman.Oku().DayOfYear == DateTime.Now.DayOfYear) return "4";//Bugün çalıştımı kontrolü
                    }
                    else if (item.tur == ENUMZamanlayiciTur.GUNLUK.ToString())
                    {
                        if (item.zaman.Oku().DayOfYear == DateTime.Now.DayOfYear) return "5";//Bugün çalıştımı kontrolü
                    }
                    else if (item.tur == ENUMZamanlayiciTur.SAATLIK.ToString())
                    {
                        if (item.zaman.Oku().Hour == DateTime.Now.Hour) return "6";//Bu saat çalıştımı kontrolü
                    }
                    else if (item.tur == ENUMZamanlayiciTur.DAKIKALIK.ToString())
                    {
                        if (item.zaman.Oku().Hour == DateTime.Now.Hour && item.zaman.Oku().Minute == DateTime.Now.Minute) return "7";//Bu dakikada çalıştımı kontrolü
                    }
                }
            }

            z.tur = zamanlamaTuru;
            z.islemAdi = islemAdi;
            z.zaman = new TNSDateTime(DateTime.Now);

            Sonuc sonuc = TNS.KYM.IsKurallari.KYMZamanlayici.Kaydet(null, z);

            return "";
        }
    }
}

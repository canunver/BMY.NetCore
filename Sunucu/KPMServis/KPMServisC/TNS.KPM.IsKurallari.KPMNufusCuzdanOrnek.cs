using System;
using System.Net;
using System.Text;
using Mernis.KPSv2.Types.BilesikKutukSorgulaKimlikNoServis;
using Mernis.KPSv2.Types.KimlikNoSorgulaAdresServis;
using Newtonsoft.Json;
using TNS.KYM;

namespace TNS.KPM.IsKurallari
{
    public class KPMNufusKayitOrnek
    {
        public static ObjectArray NufusKayitOrnekRaporla(Kullanici kullanan, Istek istek)
        {
            ObjectArray nkoVAN = new ObjectArray();
            istek.GecerliMi(nkoVAN.sonuc);

            if (!istek.entegrasyon && !Yetki.OkumaHakkiVarMi(kullanan))
                nkoVAN.sonuc.DegerAta("Okuma hakkı yok");

            if (nkoVAN.sonuc.islemSonuc)
            {
                int say = KPMIstek.Say(null, kullanan);
                if (say > 300)//Melih yaptı 13.12.2022 tarım hibe nedeniyle
                    nkoVAN.sonuc.DegerAta("aylık sorgulama limitini aştınız!");
            }

            if (!nkoVAN.sonuc.islemSonuc)
                return nkoVAN;

            if (TNS.DEG.Arac.ManasKullaniyor())
                nkoVAN = NufusOkuManas(kullanan, istek);
            else
            {
                string mernisKurumServisi = System.Configuration.ConfigurationManager.AppSettings.Get("MernisKurumServisi") + "";

                if (mernisKurumServisi == "")
                    nkoVAN = NufusOku(kullanan, istek);
                else if (mernisKurumServisi == "Tarim")
                    nkoVAN = NufusOkuTarim(kullanan, istek);
                else if (mernisKurumServisi == "TCCB")
                    nkoVAN = NufusOkuTCCB(kullanan, istek);
            }

            return nkoVAN;
        }

        private static ObjectArray NufusOkuTarim(Kullanici kullanan, Istek istek)
        {
            ObjectArray nkoVAN = new ObjectArray();
            if (istek.dogumTarih.isNull)
            {
                nkoVAN.HataAta("Doğum tarihi boş olamaz!");
                return nkoVAN;
            }
            TarimMernis.BirlesikKutukSorgulamaService mernis = new TarimMernis.BirlesikKutukSorgulamaService();
            //Tarımda hibedestek kullanıyorsa her zaman ikamet bilgisi ile çalışsın
            string MernisKurumServisiTipi = System.Configuration.ConfigurationManager.AppSettings.Get("MernisKurumServisiTipi") + "";
            if (MernisKurumServisiTipi == "HibeDestek")
                return KPMIkametgah.IkametgahRaporla(kullanan, istek);
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            string mernisKullanici = System.Configuration.ConfigurationManager.AppSettings.Get("Mernisv2User");
            string mernisParola = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(System.Configuration.ConfigurationManager.AppSettings.Get("Mernisv2Password"));

            if (string.IsNullOrWhiteSpace(kullanan.mernis) || OrtakFonksiyonlar.ConvertToLong(kullanan.mernis) == 0)
                return nkoVAN;

            try
            {
                long tt = long.Parse(kullanan.mernis);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("TRM_Mernis parse hata-->" + e.Message);
                nkoVAN.sonuc.DegerAta("Hatalı kod:" + kullanan.mernis);
                return nkoVAN;
            }

            var mernisModelKullanici = new TarimMernis.Kullanici();

            try
            {
                mernisModelKullanici = new TarimMernis.Kullanici
                {
                    KullaniciAdi = mernisKullanici,
                    Sifre = mernisParola,
                    SorgulayanTcNo = long.Parse(kullanan.mernis),
                    SorgulayanIp = kullanan.telefon//TODO: sorgulayan kişinin local ip si
                };
                DebugYaz($"Mernis Sorgulanan:{istek.tcKimlikNo}, Sorgulayan: {kullanan.mernis}, SorgulayanIp: {mernisModelKullanici.SorgulayanIp}", false);
            }
            catch (Exception e)
            {
                nkoVAN.sonuc.DegerAta(e.Message);
                OrtakFonksiyonlar.HataStrYaz("TRM_Mernis Model hata (SorgulayanTcNo:" + kullanan.mernis + ", Sorgulanan:" + istek.tcKimlikNo + ", IP:" + kullanan.telefon + ")-->" + e.Message);
                return nkoVAN;
            }

            TarimMernis.BilesikKutukSorgulaKimlikNoSorguKriteri[] nkoSorgulist = new TarimMernis.BilesikKutukSorgulaKimlikNoSorguKriteri[] { new TarimMernis.BilesikKutukSorgulaKimlikNoSorguKriteri() { DogumAy = istek.dogumTarih.Ay, DogumAySpecified = true, DogumGun = istek.dogumTarih.Gun, DogumGunSpecified = true, DogumYil = istek.dogumTarih.Yil, DogumYilSpecified = true, KimlikNo = long.Parse(istek.tcKimlikNo), KimlikNoSpecified = true } };
            TarimMernis.BilesikKutukBilgileriSonucu kisiSonuc = new TarimMernis.BilesikKutukBilgileriSonucu();
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                kisiSonuc = mernis.BilesikKutukSorgulaKimlikNoServis(nkoSorgulist, mernisModelKullanici);
                DebugYaz($"Mernis Sorgulanan:{istek.tcKimlikNo}, Sorgulayan: {kullanan.mernis}, SorgulayanIp: {mernisModelKullanici.SorgulayanIp}", false);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("TRM_Mernis sorgulama hata (SorgulayanTcNo:" + kullanan.mernis + ", Sorgulanan:" + istek.tcKimlikNo + ", IP:" + kullanan.telefon + ")-->" + e.Message);
                nkoVAN.sonuc.DegerAta("Veri alınamadı:" + e.Message);
                return nkoVAN;
            }
            if (kisiSonuc == null)
            {
                DebugYaz($"Sunucudan kisiSonuc nesnesi alınamadı", false);
                nkoVAN.sonuc.DegerAta("Sunucudan kisiSonuc nesnesi alınamadı!");
                return nkoVAN;
            }
            else if (kisiSonuc.HataBilgisi == null)
            {
                var kisiListe = kisiSonuc.SorguSonucu;

                if (kisiListe == null || kisiListe.Length == 0)
                {
                    nkoVAN.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı. BilesikKutukSorgulaKimlikNoServis sorgusu sonucunda BilesikKutukBilgileriSonucu.SorguSonucu " + (kisiListe == null ? "null" : "0 kayıt") + " döndürdü!";
                    return nkoVAN;
                }

                if (kisiListe[0].HataBilgisi != null && !string.IsNullOrEmpty(kisiListe[0].HataBilgisi.Aciklama))
                {
                    nkoVAN.sonuc.islemSonuc = false;
                    nkoVAN.sonuc.hataStr = kisiListe[0].HataBilgisi.Aciklama;
                    return nkoVAN;
                }

                TarimMernis.BilesikKutukBilgileri kisi = kisiListe[0];

                //Kisi k = new Kisi(kisi, kab);
                Kisi k = new Kisi();
                if (kisi != null)
                {
                    k.tcKimlikNo = kisi.KimlikNo.ToString();
                    k.ad = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Ad;
                    k.soyad = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Soyad;
                    k.babaAd = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.BabaAd;
                    k.anaAd = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.AnneAd;
                    k.dogumYeri = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumYer;

                    KPMIkametgah.TarihYaz(kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumTarih, k.dogumTarihi);
                    k.cinsiyet = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet != null ? (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.HasValue ? kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.Value : 0) : 0;
                    k.medeniHal = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal != null ? (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.HasValue ? kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.Value : 0) : 0;
                    k.din = 0;

                    try
                    {
                        KPMIkametgah.TarihYaz(kisi.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi.VerilmeTarih, k.nufusCuzdani.verilisTarihi);

                        k.nufusCuzdani.nufusCuzdanSeri = kisi.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi.Seri != null ? kisi.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi.Seri : "";
                        k.nufusCuzdani.nufusCuzdanNo = kisi.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi.No.ToString();
                        k.nufusCuzdani.verildigiYer = kisi.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi.VerildigiIlce.Aciklama;
                        k.nufusCuzdani.verilisNedeniAciklama = kisi.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi.CuzdanVerilmeNeden.Aciklama;
                        k.nufusCuzdani.kayitNo = kisi.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi.CuzdanKayitNo != null ? kisi.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi.CuzdanKayitNo.ToString() : "";
                    }
                    catch { }

                    //if (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Il != null && kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Il.Kod.HasValue)
                    //{
                    //    k.kayitliOlduguYer.ilce.il.kod = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Il.Kod.Value;
                    //    k.kayitliOlduguYer.ilce.il.ad = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Il.Aciklama;
                    //}
                    //if (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Ilce != null && kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Ilce.Kod.HasValue)
                    //{
                    //    k.kayitliOlduguYer.ilce.kod = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Ilce.Kod.Value;
                    //    k.kayitliOlduguYer.ilce.ad = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Ilce.Aciklama;
                    //}
                    //if (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Cilt != null && kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Cilt.Kod.HasValue)
                    //{
                    //    k.kayitliOlduguYer.mahalleKoy = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Cilt.Aciklama;
                    //    k.kayitliOlduguYer.ciltNo = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.Cilt.Kod.Value;
                    //}
                    //if (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.AileSiraNo != null && kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.AileSiraNo.HasValue)
                    //    k.kayitliOlduguYer.aileSiraNo = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.AileSiraNo.Value;
                    //if (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.BireySiraNo != null && kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.BireySiraNo.HasValue)
                    //    k.kayitliOlduguYer.bireySiraNo = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi.BireySiraNo.Value;

                    if (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet != null && kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.HasValue)
                        k.cinsiyetAciklama = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Aciklama;
                    if (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal != null && kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.HasValue)
                        k.medeniHalAciklama = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Aciklama;
                    //if (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.Din != null && kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.Din.Kod.HasValue)
                    //    k.dinAciklama = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.Din.Aciklama;

                    nkoVAN.objeler.Add(k);
                }

                try
                {
                    istek.adSoyad = k.ad + " " + k.soyad;
                    nkoVAN.sonuc = KPMIstek.Kaydet(null, kullanan, istek);
                }
                catch (Exception e)
                {
                    OrtakFonksiyonlar.HataStrYaz("TRM_Mernis Istek Kayıt hata-->" + e.Message);
                    nkoVAN.sonuc.DegerAta("İşlem hatası:" + e.Message);
                    return nkoVAN;
                }
            }
            else
            {
                nkoVAN.sonuc.DegerAta($"Sunucudan hata döndü: {kisiSonuc.HataBilgisi.Aciklama}, {kisiSonuc.HataBilgisi.Kod}");
                DebugYaz(nkoVAN.sonuc.hataStr, false);
                return nkoVAN;
            }

            return nkoVAN;
        }

        private static ObjectArray NufusOkuTCCB(Kullanici kullanan, Istek istek)
        {
            ObjectArray nkoVAN = new ObjectArray();

            string tokenAlinan = GetTokenTCCB();
            if (string.IsNullOrWhiteSpace(tokenAlinan))
            {
                nkoVAN.sonuc.hataStr = "Mernis entegrasyonu için Token bilgisi alınamadı";
                nkoVAN.sonuc.islemSonuc = false;
                OrtakFonksiyonlar.HataStrYaz("TC Sorgulanan Hata1:" + nkoVAN.sonuc.hataStr);
                return nkoVAN;
            }

            var client = new WebClient();
            string url = "https://api.tccb.gov.tr/kps/kisi";
            string dataRequest = @"{'ProjeId':<<PROJESIRANO>>,'SorgulayanTcNo':<<SORGULAYAN>>,'SorgulanacakTcNo':<<SORGULANACAK>>}";
            string response = "";

            dataRequest = dataRequest.Replace("<<PROJESIRANO>>", "1");
            dataRequest = dataRequest.Replace("<<SORGULAYAN>>", kullanan.mernis);
            dataRequest = dataRequest.Replace("<<SORGULANACAK>>", istek.tcKimlikNo);
            dataRequest = dataRequest.Replace("'", "\"");

            try
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + tokenAlinan);
                response = client.UploadString(new Uri(url), "POST", dataRequest);

                var gelenSonuc = Newtonsoft.Json.Linq.JObject.Parse(response);
                string hata = "";
                hata = NesnedenDegerVer(gelenSonuc, "hataMesaji");
                if (hata == "başarılı") hata = "";

                if (!string.IsNullOrWhiteSpace(hata))
                {
                    nkoVAN.sonuc.hataStr = hata;
                    nkoVAN.sonuc.islemSonuc = false;
                    OrtakFonksiyonlar.HataStrYaz("TC Sorgulanan Hata2:" + istek.tcKimlikNo + " " + nkoVAN.sonuc.hataStr);
                    return nkoVAN;
                }
                string tcKimlikNo = NesnedenDegerVer(gelenSonuc, "tcKimlikNo");

                if (string.IsNullOrWhiteSpace(tcKimlikNo))
                {
                    nkoVAN.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";
                    OrtakFonksiyonlar.HataStrYaz("TC Sorgulanan bulamadı:" + nkoVAN.sonuc.bilgiStr);
                    return nkoVAN;
                }

                Kisi k = new Kisi();
                k.tcKimlikNo = tcKimlikNo;
                k.ad = NesnedenDegerVer(gelenSonuc, "kisiTemelBilgisi.ad");
                k.soyad = NesnedenDegerVer(gelenSonuc, "kisiTemelBilgisi.soyad");
                k.babaAd = NesnedenDegerVer(gelenSonuc, "kisiTemelBilgisi.babaAd");
                k.anaAd = NesnedenDegerVer(gelenSonuc, "kisiTemelBilgisi.anneAd");
                k.dogumYeri = NesnedenDegerVer(gelenSonuc, "kisiTemelBilgisi.dogumYer");

                k.dogumTarihi = new TNSDateTime(NesnedenDegerVer(gelenSonuc, "kisiTemelBilgisi.dogumTarih"));
                k.cinsiyetAciklama = NesnedenDegerVer(gelenSonuc, "kisiTemelBilgisi.dogumYer");
                k.medeniHalAciklama = NesnedenDegerVer(gelenSonuc, "kisiTemelBilgisi.cinsiyet");
                k.dinAciklama = "";

                try
                {
                    k.nufusCuzdani.verilisTarihi = new TNSDateTime(NesnedenDegerVer(gelenSonuc, "kisiCuzdanBilgisi.verilmeTarihi"));

                    k.nufusCuzdani.nufusCuzdanSeri = NesnedenDegerVer(gelenSonuc, "kisiCuzdanBilgisi.cüzdanSeri");
                    k.nufusCuzdani.nufusCuzdanNo = NesnedenDegerVer(gelenSonuc, "kisiCuzdanBilgisi.cüzdanNo");
                    k.nufusCuzdani.verildigiYer = NesnedenDegerVer(gelenSonuc, "kisiCuzdanBilgisi.verildigiYerAdi");
                    k.nufusCuzdani.verilisNedeniAciklama = NesnedenDegerVer(gelenSonuc, "kisiCuzdanBilgisi.verilmeNedeni");
                    k.nufusCuzdani.kayitNo = NesnedenDegerVer(gelenSonuc, "kisiCuzdanBilgisi.cüzdanKayitNo");
                }
                catch { }

                k.kayitliOlduguYer.ilce.il.kod = OrtakFonksiyonlar.ConvertToInt(NesnedenDegerVer(gelenSonuc, "kisiKayitYeriBilgisi.nufusuBagliOlduguIlKod"), 0);
                k.kayitliOlduguYer.ilce.il.ad = NesnedenDegerVer(gelenSonuc, "kisiKayitYeriBilgisi.nufusuBagliOlduguIl");
                k.kayitliOlduguYer.ilce.kod = OrtakFonksiyonlar.ConvertToInt(NesnedenDegerVer(gelenSonuc, "kisiKayitYeriBilgisi.nufusuBagliOlduguIlceKod"), 0);
                k.kayitliOlduguYer.ilce.ad = NesnedenDegerVer(gelenSonuc, "kisiKayitYeriBilgisi.nufusuBagliOlduguIlce");
                k.kayitliOlduguYer.mahalleKoy = NesnedenDegerVer(gelenSonuc, "kisiKayitYeriBilgisi.nufusuBagliOlduguMahalleKoy");
                k.kayitliOlduguYer.ciltNo = OrtakFonksiyonlar.ConvertToInt(NesnedenDegerVer(gelenSonuc, "kisiKayitYeriBilgisi.nufusuBagliOlduguMahalleKoyCiltKod"), 0);
                k.kayitliOlduguYer.aileSiraNo = OrtakFonksiyonlar.ConvertToInt(NesnedenDegerVer(gelenSonuc, "kisiKayitYeriBilgisi.aileSiraNo"), 0);
                k.kayitliOlduguYer.bireySiraNo = OrtakFonksiyonlar.ConvertToInt(NesnedenDegerVer(gelenSonuc, "kisiKayitYeriBilgisi.bireySiraNo"), 0);


                nkoVAN.objeler.Add(k);


                try
                {
                    istek.adSoyad = k.ad + " " + k.soyad;
                    nkoVAN.sonuc = KPMIstek.Kaydet(null, kullanan, istek);
                }
                catch (Exception e)
                {
                    OrtakFonksiyonlar.HataStrYaz("TCCB_Mernis Adres hata-->" + e.Message);
                }

            }
            catch { }

            return nkoVAN;
        }

        private static void DebugYaz(string yazi, bool yeni = false)
        {
            TNSDateTime tarih = new TNSDateTime(DateTime.Now);
            OrtakFonksiyonlar.HataDosyaYaz("KPMDebugMernis.txt", tarih.ToLongString() + " - " + yazi, yeni);
        }

        private static ObjectArray NufusOku(Kullanici kullanan, Istek istek)
        {
            ObjectArray nkoVAN = new ObjectArray();

            BilesikKutukSorgulaKimlikNoServis nkoServis = Mernis.KPSv2.Types.KPSv2Servis.BilesikKutukSorgulaTCKimlikNoServisOlustur();
            BilesikKutukSorgulaKimlikNoSorguKriteri[] nkoSorgulist = { new BilesikKutukSorgulaKimlikNoSorguKriteri { KimlikNo = long.Parse(istek.tcKimlikNo) } };
            BilesikKutukBilgileriSonucu kisiSonuc = nkoServis.Sorgula(nkoSorgulist);

            if (kisiSonuc.HataBilgisi == null)
            {
                var kisiListe = kisiSonuc.SorguSonucu;

                if (kisiListe == null || kisiListe.Length == 0)
                {
                    nkoVAN.sonuc.DegerAta("Verilen kriterlere uygun kayıt bulunamadı!");
                    return nkoVAN;
                }
                if (kisiListe[0].HataBilgisi != null && !string.IsNullOrEmpty(kisiListe[0].HataBilgisi.Aciklama))
                {
                    nkoVAN.sonuc.DegerAta(kisiListe[0].HataBilgisi.Aciklama);
                    return nkoVAN;
                }

                Kisi k = new Kisi(kisiListe[0]);
                istek.adSoyad = k.ad + " " + k.soyad;
                nkoVAN.objeler.Add(k);
                //Kim, kaç defa bu raporu almış logları tutuluyor
                nkoVAN.sonuc = KPMIstek.Kaydet(null, kullanan, istek);
            }
            else
            {
                nkoVAN.sonuc.islemSonuc = false;
                nkoVAN.sonuc.hataStr = kisiSonuc.HataBilgisi.Aciklama + "(" + KPMServis.BaglantiBilgi() + ")";
                return nkoVAN;
            }

            return nkoVAN;
        }

        private static ObjectArray NufusOkuManas(Kullanici kullanan, Istek istek)
        {
            ObjectArray nkoVAN = new ObjectArray();


            IKY.IIKYServis servisIKY = IKY.Arac.Tanimla();

            IKY.Personel kriter = new IKY.Personel();
            kriter.kod = OrtakFonksiyonlar.ConvertToInt(istek.tcKimlikNo, 0); //TCKimlikNo parametresi için personel kod gönderiliyor.
            kriter.tarihce = false;

            ObjectArray perListe = servisIKY.PersonelListele(kullanan, kriter, true, false);
            if (perListe.objeler.Count > 0)
            {
                TNS.IKY.Personel per = (TNS.IKY.Personel)perListe.objeler[0];

                TNS.KPM.Kisi nufusOrnek = new TNS.KPM.Kisi();

                nufusOrnek.nufusCuzdani.nufusCuzdanSeri = per.nufus.cuzdanSeriNo;
                nufusOrnek.nufusCuzdani.nufusCuzdanNo = per.nufus.cuzdanKayitNo;
                nufusOrnek.cinsiyetAciklama = per.nufus.cinsiyetAd;
                nufusOrnek.soyad = per.soyad;
                nufusOrnek.ad = per.ad;
                nufusOrnek.babaAd = per.nufus.babaAd;
                nufusOrnek.anaAd = per.nufus.anaAd;
                nufusOrnek.dogumYeri = per.nufus.dogumYeri;
                nufusOrnek.dogumTarihi = per.nufus.dogumTarihi;
                nufusOrnek.medeniHalAciklama = per.nufus.medeniHaliAd;
                nufusOrnek.dinAciklama = per.nufus.dini;
                nufusOrnek.kanGrubu = per.nufus.kanGrubu;
                nufusOrnek.kizlikSoyad = per.nufus.ilkSoyad;
                nufusOrnek.kayitliOlduguYer.ilce.il.ad = per.nufus.ilAd;
                nufusOrnek.kayitliOlduguYer.ilce.ad = per.nufus.ilceAd;
                nufusOrnek.kayitliOlduguYer.mahalleKoy = per.nufus.nufusMahalleKoy;
                nufusOrnek.kayitliOlduguYer.ciltNo = OrtakFonksiyonlar.ConvertToInt(per.nufus.nufusCiltNo, 0);
                nufusOrnek.kayitliOlduguYer.aileSiraNo = OrtakFonksiyonlar.ConvertToInt(per.nufus.nufusAileSiraNo, 0);
                nufusOrnek.kayitliOlduguYer.bireySiraNo = OrtakFonksiyonlar.ConvertToInt(per.nufus.nufusSiraNo, 0);
                nufusOrnek.nufusCuzdani.verildigiYer = per.nufus.cuzdanVerildigiYer;
                nufusOrnek.nufusCuzdani.verilisNedeniAciklama = per.nufus.cuzdanVerilisNedeni;
                nufusOrnek.nufusCuzdani.kayitNo = per.nufus.cuzdanKayitNo;
                nufusOrnek.nufusCuzdani.verilisTarihi = per.nufus.cuzdanVerilisTarihi;

                nkoVAN.objeler.Add(nufusOrnek);
            }

            return nkoVAN;
        }

        private static string NesnedenDegerVer(Newtonsoft.Json.Linq.JObject gelenSonuc, string v)
        {
            try
            {
                string[] parca = v.Split('.');
                if (parca.Length == 1)
                    return gelenSonuc[v] + "";
                else if (parca.Length == 2)
                    return gelenSonuc[parca[0]][parca[1]] + "";
            }
            catch
            {

                return "";
            }

            return "";
        }

        static string GetTokenTCCB()
        {
            string url = "https://api.tccb.gov.tr/token/authenticate";
            string response = "";

            string dataRequest = @"{'username':'<<USERNAME>>','password':'<<PASSWORD>>'}";

            string username = OrtakFonksiyonlar.WebConfigOku("Mernisv2User", "");
            string password = OrtakFonksiyonlar.WebConfigOku("Mernisv2Password", "");

            dataRequest = dataRequest.Replace("<<USERNAME>>", username);
            dataRequest = dataRequest.Replace("<<PASSWORD>>", password);
            dataRequest = dataRequest.Replace("'", "\"");

            var client = new WebClient();

            try
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                response = client.UploadString(new Uri(url), "POST", dataRequest);

                var obj = Newtonsoft.Json.Linq.JObject.Parse(response); //JsonConvert.DeserializeObject<dynamic>(response);
                return GelenNesnedenTokenAlTCCB(obj);
            }
            catch (WebException we)
            {
                OrtakFonksiyonlar.HataStrYaz("Hata aldık (TCCB Get Token):" + we.Message + "*" + we.StackTrace + "*" + we.InnerException + "*" + we.Source);
            }

            return "";
        }

        static string GelenNesnedenTokenAlTCCB(Newtonsoft.Json.Linq.JObject obj)
        {
            string tokenStr = NesnedenDegerVer(obj, "token");

            if (!string.IsNullOrWhiteSpace(tokenStr))
                return tokenStr;
            else
                return "";
        }

        //private static void MernisLog(string v1, string v2 = "")
        //{
        //    OrtakFonksiyonlar.HataDosyaYaz("MernisLog.txt", v1 + " " + v2, false);
        //}


    }
}

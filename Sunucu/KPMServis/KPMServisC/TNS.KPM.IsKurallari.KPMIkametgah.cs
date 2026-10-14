using System;
using System.Net;
using System.Text;
//using TarimMernis;
using Mernis.KPSv2.Types.BilesikKutukSorgulaKimlikNoServis;
using Mernis.KPSv2.Types.KimlikNoSorgulaAdresServis;
using TNS.IKY;
using TNS.KYM;

namespace TNS.KPM.IsKurallari
{
    public class KPMIkametgah
    {
        public static ObjectArray IkametgahRaporla(Kullanici kullanan, Istek istek)
        {
            ObjectArray ikametgahVAN = new ObjectArray();
            istek.GecerliMi(ikametgahVAN.sonuc);

            if (!Yetki.OkumaHakkiVarMi(kullanan) && istek.istekTur != (int)ENUMIstekTur.SADECEADRES)
                ikametgahVAN.sonuc.DegerAta("Okuma hakkı yok");

            if (ikametgahVAN.sonuc.islemSonuc)
            {
                int say = KPMIstek.Say(null, kullanan);
                if (say > 300)//Melih yaptı 13.12.2022 tarım hibe nedeniyle
                    ikametgahVAN.sonuc.DegerAta("aylık sorgulama limitini aştınız!");
            }

            if (!ikametgahVAN.sonuc.islemSonuc)
                return ikametgahVAN;


            if (TNS.DEG.Arac.ManasKullaniyor())
                ikametgahVAN = IkametgahOkuManas(kullanan, istek);
            else
            {
                string mernisKurumServisi = System.Configuration.ConfigurationManager.AppSettings.Get("MernisKurumServisi") + "";

                if (mernisKurumServisi == "")
                    ikametgahVAN = IkametgahOku(kullanan, istek);
                else if (mernisKurumServisi == "Tarim")
                    ikametgahVAN = IkametgahOkuTarim(kullanan, istek);
                else if (mernisKurumServisi == "TCCB")
                    ikametgahVAN = IkametgahOkuTCCB(kullanan, istek);
            }

            return ikametgahVAN;
        }


        private static ObjectArray IkametgahOkuTarim(Kullanici kullanan, Istek istek)
        {
            ObjectArray ikametgahVAN = new ObjectArray();
            if (istek.dogumTarih.isNull)
            {
                ikametgahVAN.sonuc.DegerAta("Doğum tarihi boş olamaz!");
            }
            if (string.IsNullOrWhiteSpace(kullanan.mernis) || OrtakFonksiyonlar.ConvertToLong(kullanan.mernis) == 0)
            {
                ikametgahVAN.sonuc.DegerAta("Hatalı mernis no: " + kullanan.mernis);
            }

            try
            {
                long tt = long.Parse(kullanan.mernis);
            }
            catch (Exception e)
            {
                ikametgahVAN.sonuc.DegerAta("Hatalı mernis no: " + kullanan.mernis);
            }
            if(!ikametgahVAN.sonuc.islemSonuc) return ikametgahVAN;
            TarimMernis.BirlesikKutukSorgulamaService mernis = new TarimMernis.BirlesikKutukSorgulamaService();

            string mernisKullanici = System.Configuration.ConfigurationManager.AppSettings.Get("Mernisv2User");
            string mernisParola = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(System.Configuration.ConfigurationManager.AppSettings.Get("Mernisv2Password"));


            //İlk önce veri tabanında daha önce sorgulanan kayıt varsa onu oku ve geri döndür
            Kisi sk = KPMIstek.GetirKimlik(null, kullanan, istek.tcKimlikNo);
            if (sk != null && sk.tcKimlikNo != "")
            {
                ikametgahVAN.objeler.Add(sk);
                return ikametgahVAN;
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
                DebugYaz($"Adres Sorgulanan:{istek.tcKimlikNo}, Sorgulayan: {kullanan.mernis}, SorgulayanIp: {mernisModelKullanici.SorgulayanIp}", false);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("TRM_Ikamet Model hata (SorgulayanTcNo:" + kullanan.mernis + ", Sorgulanan:" + istek.tcKimlikNo + ", IP:" + kullanan.telefon + ")-->" + e.Message);
                return ikametgahVAN;
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
                ikametgahVAN.sonuc.DegerAta("Sorgulamadı: " + e.Message);
                OrtakFonksiyonlar.HataStrYaz("TRM_Ikamet sorgulama hata (SorgulayanTcNo:" + kullanan.mernis + ", Sorgulanan:" + istek.tcKimlikNo + ", IP:" + kullanan.telefon + ")-->" + e.Message);
                return ikametgahVAN;
            }

            if (kisiSonuc.HataBilgisi == null)
            {
                var kisiListe = kisiSonuc.SorguSonucu;

                if (kisiListe == null || kisiListe.Length == 0)
                {
                    ikametgahVAN.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";
                    return ikametgahVAN;
                }

                if (kisiListe[0].HataBilgisi != null && !string.IsNullOrEmpty(kisiListe[0].HataBilgisi.Aciklama))
                {
                    ikametgahVAN.sonuc.islemSonuc = false;
                    ikametgahVAN.sonuc.hataStr = kisiListe[0].HataBilgisi.Aciklama;
                    return ikametgahVAN;
                }

                TarimMernis.BilesikKutukBilgileri kisi = kisiListe[0];

                //Adres bilgileri
                TarimMernis.AdresBilgileriSonucu adresSonuc = new TarimMernis.AdresBilgileriSonucu();
                try
                {
                    adresSonuc = mernis.AdresBilgisiSorgula(nkoSorgulist[0].KimlikNo.Value, true, istek.dogumTarih.Yil, true, istek.dogumTarih.Ay, true, istek.dogumTarih.Gun, true, mernisModelKullanici);
                }
                catch (Exception e)
                {
                    ikametgahVAN.sonuc.DegerAta("Sorgulamadı: " + e.Message);

                    OrtakFonksiyonlar.HataStrYaz("TRM_Ikamet kimlik hata-->" + e.Message + " Kullanici:" + mernisKullanici + " Parola:" + mernisParola);
                    return ikametgahVAN;
                }

                var kab = adresSonuc.YerlesimYeriAdresi;
                //TarimMernis.AdresBilgileri kab = new TarimMernis.AdresBilgileri();
                //if (adresList.Length > 0)
                //{
                //    kab = adresList[0];
                //}

                //Kisi k = new Kisi(kisi, kab);
                Kisi k = new Kisi();
                if (kisi != null)
                {
                    try
                    {
                        if (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Ad != null)
                            KimlikBilgisiAlTarim(kisi, ref k);
                        else
                            KimlikBilgisiAlMaviTarim(kisi, ref k);
                    }
                    catch { }
                }

                if (kab != null)
                {
                    k.tcKimlikNo = kab.TcKimlikNo.ToString();

                    if (kab != null)
                    {
                        k.adres.il = kab.Il;
                        k.adres.ilce = kab.Ilce;
                        k.adres.bucak = "";// adres.Bucak;
                        k.adres.mahalle = kab.Mahalle;
                        k.adres.sokak = kab.Cadde;
                        k.adres.numara = kab.DisKapiNo + "/" + kab.IcKapiNo;
                    }
                    else
                    {
                        k.adres.bucak = "Hata! Kişinin, Adres Kayıt Sisteminde adres bilgisi bulunmamaktadır";
                    }
                }

                ikametgahVAN.objeler.Add(k);

                try
                {
                    istek.adSoyad = k.ad + " " + k.soyad;
                    ikametgahVAN.sonuc = KPMIstek.Kaydet(null, kullanan, istek);

                    //Kimlik bilgisini veri tabanına kaydet
                    if (!string.IsNullOrWhiteSpace(k.ad))
                    {
                        if (!string.IsNullOrWhiteSpace(k.adres.il))//adresi yoksa kayıt olmasın
                            KPMIstek.KaydetKimlik(null, kullanan, k);
                    }
                }
                catch (Exception e)
                {
                    OrtakFonksiyonlar.HataStrYaz("TRM_Ikamet istek kaydet hata-->" + e.Message);
                    ikametgahVAN.sonuc.DegerAta("Kaydedilemedi: " + e.Message);
                    return ikametgahVAN;
                }
            }
            else {
                ikametgahVAN.sonuc.DegerAta("Sorgulanamadı:" + kisiSonuc.HataBilgisi);
            }
            return ikametgahVAN;
        }

        private static void KimlikBilgisiAlTarim(TarimMernis.BilesikKutukBilgileri kisi, ref Kisi k)
        {
            k.tcKimlikNo = kisi.KimlikNo.ToString();
            k.ad = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Ad;
            k.soyad = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Soyad;
            k.babaAd = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.BabaAd;
            k.anaAd = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.AnneAd;
            k.dogumYeri = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumYer;

            TarihYaz(kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumTarih, k.dogumTarihi);

            //k.dogumTarihiMer.Yaz(kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumTarih.Yil, kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumTarih.Ay, kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumTarih.Gun);
            k.cinsiyet = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet != null ? (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.HasValue ? kisi.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.Value : 0) : 0;
            k.medeniHal = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal != null ? (kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.HasValue ? kisi.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.Value : 0) : 0;
            k.din = 0;

            try
            {
                TarihYaz(kisi.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi.VerilmeTarih, k.nufusCuzdani.verilisTarihi);

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

            //KayitliOlduguYer ataması
            //TarimMernis.TCKisiKayitYeriBilgisi kkyb = kisi.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi;

            //if (kkyb.Il != null && kkyb.Il.Kod.HasValue)
            //{
            //    k.kayitliOlduguYer.ilce.il.kod = kkyb.Il.Kod.Value;
            //    k.kayitliOlduguYer.ilce.il.ad = kkyb.Il.Aciklama;
            //}
            //if (kkyb.Ilce != null && kkyb.Ilce.Kod.HasValue)
            //{
            //    k.kayitliOlduguYer.ilce.kod = kkyb.Ilce.Kod.Value;
            //    k.kayitliOlduguYer.ilce.ad = kkyb.Ilce.Aciklama;
            //}
            //if (kkyb.Cilt != null && kkyb.Cilt.Kod.HasValue)
            //{
            //    k.kayitliOlduguYer.mahalleKoy = kkyb.Cilt.Aciklama;
            //    k.kayitliOlduguYer.ciltNo = kkyb.Cilt.Kod.Value;
            //}
            //if (kkyb.AileSiraNo != null && kkyb.AileSiraNo.HasValue)
            //    k.kayitliOlduguYer.aileSiraNo = kkyb.AileSiraNo.Value;
            //if (kkyb.BireySiraNo != null && kkyb.BireySiraNo.HasValue)
            //    k.kayitliOlduguYer.bireySiraNo = kkyb.BireySiraNo.Value;
        }

        private static void KimlikBilgisiAlMaviTarim(TarimMernis.BilesikKutukBilgileri kisi, ref Kisi k)
        {
            k.tcKimlikNo = kisi.KimlikNo.ToString();
            k.ad = kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.Ad;
            k.soyad = kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.Soyad;
            k.babaAd = kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.BabaAd;
            k.anaAd = kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.AnneAd;
            k.dogumYeri = kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumYer;

            TarihYaz(kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumTarih, k.dogumTarihi);

            //k.dogumTarihiMer.Yaz(kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumTarih.Yil, kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumTarih.Ay, kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.DogumTarih.Gun);
            k.cinsiyet = kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet != null ? (kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.HasValue ? kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.Value : 0) : 0;
            k.medeniHal = kisi.MaviKartliKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal != null ? (kisi.MaviKartliKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.HasValue ? kisi.MaviKartliKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.Value : 0) : 0;

            if (kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet != null && kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.HasValue)
                k.cinsiyetAciklama = kisi.MaviKartliKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Aciklama;
            if (kisi.MaviKartliKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal != null && kisi.MaviKartliKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.HasValue)
                k.medeniHalAciklama = kisi.MaviKartliKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Aciklama;
        }

        private static ObjectArray IkametgahOkuTCCB(Kullanici kullanan, Istek istek)
        {
            ObjectArray nkoVAN = new ObjectArray();

            string tokenAlinan = GetTokenTCCB();
            if (string.IsNullOrWhiteSpace(tokenAlinan))
            {
                nkoVAN.sonuc.hataStr = "Mernis Adres entegrasyonu için Token bilgisi alınamadı";
                nkoVAN.sonuc.islemSonuc = false;
                OrtakFonksiyonlar.HataStrYaz("İkametgah Sorgulanan Hata1:" + nkoVAN.sonuc.hataStr);
                return nkoVAN;
            }

            var client = new WebClient();
            string url = "https://api.tccb.gov.tr/kps/adres";
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

                var gelenSonuc = TNS.JsonIslem.NesneAyristir(response);
                string hata = "";
                hata = NesnedenDegerVer(gelenSonuc, "hataMesaji");
                if (hata == "başarılı") hata = "";

                if (!string.IsNullOrWhiteSpace(hata) && hata != "başarılı")
                {
                    nkoVAN.sonuc.hataStr = hata;
                    nkoVAN.sonuc.islemSonuc = false;
                    OrtakFonksiyonlar.HataStrYaz("İkamet Sorgulanan Hata2:" + istek.tcKimlikNo + " " + nkoVAN.sonuc.hataStr);
                    return nkoVAN;
                }

                Kisi k = new Kisi();

                ObjectArray olar = KPMNufusKayitOrnek.NufusKayitOrnekRaporla(kullanan, istek);
                foreach (Kisi item in olar.objeler)
                {
                    k = item;
                }

                string tcKimlikNo = k.tcKimlikNo;

                if (string.IsNullOrWhiteSpace(tcKimlikNo))
                {
                    nkoVAN.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";
                    OrtakFonksiyonlar.HataStrYaz("TC Sorgulanan bulamadı:" + istek.tcKimlikNo + " " + nkoVAN.sonuc.bilgiStr);
                    return nkoVAN;
                }

                k.adres.il = NesnedenDegerVer(gelenSonuc, "il");
                k.adres.ilce = NesnedenDegerVer(gelenSonuc, "ilce");
                k.adres.bucak = "";// adres.Bucak;
                k.adres.mahalle = NesnedenDegerVer(gelenSonuc, "mahalle");
                k.adres.sokak = NesnedenDegerVer(gelenSonuc, "csbm");
                k.adres.numara = NesnedenDegerVer(gelenSonuc, "disKapiNo") + " / " + NesnedenDegerVer(gelenSonuc, "icKapiNo");
                k.adres.koy = NesnedenDegerVer(gelenSonuc, "koy");

                nkoVAN.objeler.Add(k);

                try
                {
                    istek.adSoyad = k.ad + " " + k.soyad;
                    nkoVAN.sonuc = KPMIstek.Kaydet(null, kullanan, istek);
                }
                catch (Exception e)
                {
                    OrtakFonksiyonlar.HataStrYaz("TCCB_Ikamet Adres hata-->" + e.Message);
                }

            }
            catch { }

            return nkoVAN;
        }

        public static void TarihYaz(TarimMernis.TarihBilgisi dogumTarih, TNSDateTime dogumTarihi)
        {
            if (dogumTarih != null && dogumTarih.Yil.HasValue && dogumTarih.Ay.HasValue && dogumTarih.Gun.HasValue)
                dogumTarihi.Yaz(new DateTime(dogumTarih.Yil.Value, dogumTarih.Ay.Value, dogumTarih.Gun.Value));
        }

        private static ObjectArray IkametgahOku(Kullanici kullanan, Istek istek)
        {
            ObjectArray ikametgahVAN = new ObjectArray();

            BilesikKutukSorgulaKimlikNoServis nkoServis = Mernis.KPSv2.Types.KPSv2Servis.BilesikKutukSorgulaTCKimlikNoServisOlustur();
            BilesikKutukSorgulaKimlikNoSorguKriteri[] nkoSorgulist = { new BilesikKutukSorgulaKimlikNoSorguKriteri { KimlikNo = long.Parse(istek.tcKimlikNo) } };
            Mernis.KPSv2.Types.BilesikKutukSorgulaKimlikNoServis.BilesikKutukBilgileriSonucu kisiSonuc = nkoServis.Sorgula(nkoSorgulist);

            if (kisiSonuc.HataBilgisi == null)
            {
                var kisiListe = kisiSonuc.SorguSonucu;

                if (kisiListe == null || kisiListe.Length == 0)
                {
                    ikametgahVAN.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";
                    return ikametgahVAN;
                }

                if (kisiListe[0].HataBilgisi != null && !string.IsNullOrEmpty(kisiListe[0].HataBilgisi.Aciklama))
                {
                    ikametgahVAN.sonuc.islemSonuc = false;
                    ikametgahVAN.sonuc.hataStr = kisiListe[0].HataBilgisi.Aciklama;
                    return ikametgahVAN;
                }

                Mernis.KPSv2.Types.BilesikKutukSorgulaKimlikNoServis.BilesikKutukBilgileri kisi = kisiListe[0];

                KimlikNoSorgulaAdresServis adresServis = Mernis.KPSv2.Types.KPSv2Servis.KimlikNoSorgulaAdresServisOlustur();
                KimlikNoileAdresSorguKriteri[] adresSorguList = { new KimlikNoileAdresSorguKriteri { KimlikNo = long.Parse(istek.tcKimlikNo) } };
                Mernis.KPSv2.Types.KimlikNoSorgulaAdresServis.KimlikNoileKisiAdresBilgileriSonucu adresSonuc = adresServis.Sorgula(adresSorguList);

                var adresList = adresSonuc.SorguSonucu;
                Mernis.KPSv2.Types.KimlikNoSorgulaAdresServis.KimlikNoileKisiAdresBilgileri kab = new Mernis.KPSv2.Types.KimlikNoSorgulaAdresServis.KimlikNoileKisiAdresBilgileri();
                if (adresList.Length > 0)
                {
                    kab = adresList[0];
                }

                Kisi k = new Kisi(kisi, kab);
                ikametgahVAN.objeler.Add(k);

                istek.adSoyad = k.ad + " " + k.soyad;
                ikametgahVAN.sonuc = KPMIstek.Kaydet(null, kullanan, istek);
            }
            else
            {
                ikametgahVAN.sonuc.islemSonuc = false;
                ikametgahVAN.sonuc.hataStr = kisiSonuc.HataBilgisi.Aciklama;
                return ikametgahVAN;
            }

            return ikametgahVAN;
        }

        private static ObjectArray IkametgahOkuManas(Kullanici kullanan, Istek istek)
        {
            ObjectArray ikametgahVAN = new ObjectArray();

            IKY.IIKYServis servisIKY = IKY.Arac.Tanimla();

            IKY.Personel kriter = new IKY.Personel();
            kriter.kod = OrtakFonksiyonlar.ConvertToInt(istek.tcKimlikNo, 0); //TCKimlikNo parametresi için personel kod gönderiliyor.
            kriter.tarihce = false;

            ObjectArray perListe = servisIKY.PersonelListele(kullanan, kriter, true, false);
            if (perListe.objeler.Count > 0)
            {
                TNS.IKY.Personel per = (TNS.IKY.Personel)perListe.objeler[0];

                per.iletisim = servisIKY.PersonelAdresListele(kullanan, per.kod);

                TNS.KPM.Kisi k = new TNS.KPM.Kisi();
                k.ad = per.ad;
                k.soyad = per.soyad;
                k.babaAd = per.nufus.babaAd;
                k.anaAd = per.nufus.anaAd;
                k.dogumYeri = per.nufus.dogumYeri;
                k.dogumTarihi = per.nufus.dogumTarihi;
                k.medeniHalAciklama = per.nufus.medeniHaliAd;
                k.adres.sokak = per.iletisim.adres;

                ikametgahVAN.objeler.Add(k);
            }

            return ikametgahVAN;
        }

        private static string NesnedenDegerVer(System.Text.Json.Nodes.JsonObject gelenSonuc, string v)
        {
            try
            {
                string[] parca = v.Split('.');
                if (parca.Length == 1)
                    return TNS.JsonIslem.Metin(gelenSonuc[v]);
                else if (parca.Length == 2)
                    return TNS.JsonIslem.Metin(gelenSonuc[parca[0]][parca[1]]);
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

                var obj = TNS.JsonIslem.NesneAyristir(response); //JsonConvert.DeserializeObject<dynamic>(response);
                return GelenNesnedenTokenAlTCCB(obj);
            }
            catch (WebException we)
            {
                OrtakFonksiyonlar.HataStrYaz("Hata aldık (TCCB Get Token):" + we.Message + "*" + we.StackTrace + "*" + we.InnerException + "*" + we.Source);
            }

            return "";
        }

        private static void DebugYaz(string yazi, bool yeni = false)
        {
            TNSDateTime tarih = new TNSDateTime(DateTime.Now);
            OrtakFonksiyonlar.HataDosyaYaz("KPMDebugIkamet.txt", tarih.ToLongString() + " - " + yazi, yeni);
        }

        static string GelenNesnedenTokenAlTCCB(System.Text.Json.Nodes.JsonObject obj)
        {
            string tokenStr = NesnedenDegerVer(obj, "token");

            if (!string.IsNullOrWhiteSpace(tokenStr))
                return tokenStr;
            else
                return "";
        }

    }
}

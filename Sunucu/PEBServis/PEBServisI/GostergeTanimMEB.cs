using System;
using System.Collections.Generic;
using System.Text;
using TNS.BKD;
using System.Collections;
using TNS.ORG;
using TNS.KYM;

namespace TNS.PEB
{

    public class PerformansHamVeriTur
    {
        public int kod;
        public string ad;
        public ENUMGostergeOran oranMi;

        public PerformansHamVeriTur(int kod, string ad, ENUMGostergeOran oranMi)
        {
            this.kod = kod;
            this.ad = ad;
            this.oranMi = oranMi;
        }
    }

    public class PerformansAnahtar
    {
        public string kod;
        public string ad;
        public ENUMGostergeOran oranMi;

        public PerformansAnahtar(string kod, string ad, ENUMGostergeOran oranMi)
        {
            this.kod = kod;
            this.ad = ad;
            this.oranMi = oranMi;
        }
    }

    public enum ENUMAnahtarVeriGirisTur
    {
        yok, liste, metin
    }

    public enum ENUMGostergeOran
    {
        oran, oranDegil, belirsiz
    }

    public interface IHamveriTurTanim
    {
        PerformansHamVeriTur[] VeriTurleriDondur(string birimTur);

        ENUMAnahtarVeriGirisTur Anahtar1VeriGirisTur(int hamVeriKod, string birimTur);
        PerformansAnahtar[] Anahtar1ListeDondur(int hamVeriKod, string birimTur);

        ENUMAnahtarVeriGirisTur Anahtar2VeriGirisTur(int hamVeriKod, string anahtar1, string birimTur);
        PerformansAnahtar[] Anahtar2ListeDondur(int hamVeriKod, string anahtar1, string birimTur);

        ENUMAnahtarVeriGirisTur Anahtar3VeriGirisTur(int hamVeriKod, string anahtar1, string anahtar2, string birimTur);
        PerformansAnahtar[] Anahtar3ListeDondur(int hamVeriKod, string anahtar1, string anahtar2, string birimTur);

        ENUMAnahtarVeriGirisTur Anahtar4VeriGirisTur(int hamVeriKod, string anahtar1, string anahtar2, string anahtar3, string birimTur);
        PerformansAnahtar[] Anahtar4ListeDondur(int hamVeriKod, string anahtar1, string anahtar2, string anahtar3, string birimTur);
    }

    /*
     * 
                       Bilgi 
Açıklama               Türü  Anahtar1         Anahtar2               Anahtar3                 Anahtar4   Açıklama
---------------------  ----  ---------------  ---------------------  -----------------------  ---------  ---------------------------------------
Sınıf Bilgileri         1    Sınıf Kodları    Sınıf Bilgi Kodu       Şube Kodu                -          Not: Sınıf Bilgi kodu 1 (şube sayısı) 
                                                                                                              için şube kodu alanı kullanılmıyor
---------------------  ----  ---------------  ---------------------  -----------------------  ---------  ----------------------------------
Okul Bilgileri          2    Okul Bilgi Kodu  -                      -                        -         
---------------------  ----  ---------------  ---------------------  -----------------------  ---------  ----------------------------------
Ders Başarı Durumu      3    Sınıf Kodları    Ders Kodları           Başarı Durumu (1,2,3,4)  Şube Kodu 
---------------------  ----  ---------------  ---------------------  -----------------------  ---------  ---------------------------------
Yıl Sonu Karne Ders 
Notları                 4    Sınıf Kodları    Ders Kodları           Şube Kodu                -         
---------------------  ----  ---------------  ---------------------  -----------------------  ---------  ----------------------------------
SBS konuları bazında
%5, %50 ve %100 giren 
öğrenci sayısı          5    SBSB             Ders Kodları           Başarı Durumu (1,2,3)    -         	
---------------------  ----  ---------------  ---------------------  -----------------------  ---------  ----------------------------------
SBS konuları bazında
alınan puanlar          5    SBSN             Ders Kodları           -                        -         	
---------------------  ----  ---------------  ---------------------  -----------------------  ---------  ----------------------------------
YGS'de  %5, %50 ve 
%100 giren öğrenci 
sayısı                  5    YGSB             Başarı Durumu (1,2,3)  -                        -         
---------------------  ----  ---------------  ---------------------  -----------------------  ---------  ----------------------------------
     
     * 
     * 
     * 
     * */



    public class MEBHamveriTurTanim : IHamveriTurTanim
    {
        public static PerformansHamVeriTur[] performansHamVeriTurleri ={
            new PerformansHamVeriTur(1, "Sınıf Sayısal Bilgiler", ENUMGostergeOran.belirsiz),
            new PerformansHamVeriTur(2, "Okul Sayısal Bilgiler", ENUMGostergeOran.belirsiz),
            new PerformansHamVeriTur(3, "Ögrenci Ders Başari (Not)", ENUMGostergeOran.belirsiz),
            new PerformansHamVeriTur(4, "Öğrenci Ders Basari (Geçis)", ENUMGostergeOran.belirsiz),
            new PerformansHamVeriTur(5, "Üst Eğitim Kurumuna Geçiş", ENUMGostergeOran.belirsiz)
            };

        public static PerformansAnahtar[] sinifSayisalBilgiler = {  
            new PerformansAnahtar("01", "Şube Sayısı", ENUMGostergeOran.oranDegil),
            new PerformansAnahtar("02", "Kız Öğrenci Sayısı", ENUMGostergeOran.oranDegil),
            new PerformansAnahtar("03", "Erkek Öğrenci Sayısı", ENUMGostergeOran.oranDegil),
            new PerformansAnahtar("04", "Dezavantajlı Öğrenci Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("05", "Özel Öğrenci Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("06", "Yatılı Öğrenci Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("07", "Başarı Belgesi Alan Öğrenci Sayısı", ENUMGostergeOran.oran)};

        public static PerformansAnahtar[] okulSayisalBilgiler = {  
            new PerformansAnahtar("01", "Ögretmen Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("02", "Bilgisayar Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("03", "Uygulama Sınıfı Sayısı", ENUMGostergeOran.oranDegil),
            new PerformansAnahtar("04", "Toplam Okul Geliri", ENUMGostergeOran.oranDegil),
            new PerformansAnahtar("05", "Lisanslı Sporcu Sayısı", ENUMGostergeOran.oranDegil),
            new PerformansAnahtar("06", "Katılınan Beyin Olimpiyatları Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("07", "Ulusal Etkinlik Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("08", "Uluslararası Etkinlik Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("09", "Ulusal Ödül Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("10", "Uluslararası Ödül Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("11", "Devam Oranı", ENUMGostergeOran.oran),
            new PerformansAnahtar("12", "Mezuniyet Oranı", ENUMGostergeOran.oran),
            new PerformansAnahtar("13", "Disiplin Cezası Alan Öğrenci Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("14", "Terk Etmeme Oranı", ENUMGostergeOran.oran),
            new PerformansAnahtar("15", "Sınıf Geçme Oranı", ENUMGostergeOran.oran),
            new PerformansAnahtar("16", "On Günden Fazla Devamsız Öğrenci Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("17", "Branşında ders veren öğretmenin girdiği ders oranı", ENUMGostergeOran.oran),
            new PerformansAnahtar("18", "İlçe okul sıralaması", ENUMGostergeOran.oranDegil),
            new PerformansAnahtar("19", "İl okul sıralaması", ENUMGostergeOran.oranDegil),
            new PerformansAnahtar("20", "Ülke okul sıralaması", ENUMGostergeOran.oranDegil),
            new PerformansAnahtar("21", "Okulda Meydana Gelen Kaza Sayısı", ENUMGostergeOran.oran),
            new PerformansAnahtar("22", "Güvenlik Görevlisi Sayısı", ENUMGostergeOran.oran),
            //,
            //new PerformansAnahtar("18", "Alan Dal Sayısı", ENUMGostergeOran.oran),
            //new PerformansAnahtar("19", "İşletme Uygulama Öğrenci Sayısı", ENUMGostergeOran.oranDegil),
            //new PerformansAnahtar("20", "Mezun Çalışan Sayısı", ENUMGostergeOran.oranDegil),
            //new PerformansAnahtar("21", "Akredite Dal Sayısı", ENUMGostergeOran.oranDegil),
            //new PerformansAnahtar("22", "Açılan Kurs Sayısı", ENUMGostergeOran.oranDegil),
            //new PerformansAnahtar("23", "Kurs Çeşidi Sayısı", ENUMGostergeOran.oranDegil),
            //new PerformansAnahtar("24", "Kursa Katılımcı Sayısı", ENUMGostergeOran.oranDegil),
            //new PerformansAnahtar("25", "Rehber Öğretmen Sayısı", ENUMGostergeOran.oran),
            //new PerformansAnahtar("26", "Yapılan Rehberlik Sayısı", ENUMGostergeOran.oran),
            //new PerformansAnahtar("28", "Branş Dersine Girme", ENUMGostergeOran.oran),
            //new PerformansAnahtar("29", "Lisans Üstü Dereceli Ögretmen Sayısı", ENUMGostergeOran.oran)
        };

        public static PerformansAnahtar[] IlkOgretimSiniflar = {
            new PerformansAnahtar("01", "1. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("02", "2. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("03", "3. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("04", "4. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("05", "5. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("06", "6. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("07", "7. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("08", "8. Sınıf", ENUMGostergeOran.oran)};

        public static PerformansAnahtar[] TumSiniflar = {
            new PerformansAnahtar("01", "1. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("02", "2. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("03", "3. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("04", "4. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("05", "5. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("06", "6. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("07", "7. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("08", "8. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("09", "9. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("10", "10. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("11", "11. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("12", "12. Sınıf", ENUMGostergeOran.oran)};

        public static PerformansAnahtar[] OrtaOgretimSiniflar = {
            new PerformansAnahtar("09", "9. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("10", "10. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("11", "11. Sınıf", ENUMGostergeOran.oran),
            new PerformansAnahtar("12", "12. Sınıf", ENUMGostergeOran.oran)};

        public PerformansHamVeriTur[] VeriTurleriDondur(string birimTur)
        {
            return performansHamVeriTurleri;
        }

        public ENUMAnahtarVeriGirisTur Anahtar1VeriGirisTur(int hamVeriKod, string birimTur)
        {
            return ENUMAnahtarVeriGirisTur.liste;
        }

        public PerformansAnahtar[] Anahtar1ListeDondur(int hamVeriKod, string birimTur)
        {
            if (hamVeriKod == 1)
                return SiniflariDondur(birimTur);
            else if (hamVeriKod == 2)
                return okulSayisalBilgiler;
            else if (hamVeriKod == 3 || hamVeriKod == 4)
                return SiniflariDondur(birimTur);
            else if (hamVeriKod == 5)
                return SinavTurleriDondur(birimTur);

            return null;
        }

        private PerformansAnahtar[] SinavTurleriDondur(string birimTur)
        {
            if (string.IsNullOrEmpty(birimTur))
            {
                return new PerformansAnahtar[] { 
                    new PerformansAnahtar("SBSN", "Orta Öğrenime Sınavla Geçiş", ENUMGostergeOran.oran),
                    new PerformansAnahtar("SBSB", "Orta Öğrenime Sınavla Geçiş", ENUMGostergeOran.oran),
                    new PerformansAnahtar("YGSB", "Yüksek Öğrenime Sınavla Geçiş", ENUMGostergeOran.oran)
                    };
            }
            else
            {
                string noktasiz = birimTur.Replace(".", "");
                if (noktasiz == "0203")
                    return new PerformansAnahtar[] { 
                    new PerformansAnahtar("SBSN", "Orta Öğrenime Sınavla Geçiş", ENUMGostergeOran.oran),
                    new PerformansAnahtar("SBSB", "Orta Öğrenime Sınavla Geçiş", ENUMGostergeOran.oran)
                };
                else
                    return new PerformansAnahtar[] { new PerformansAnahtar("YGSB", "Yüksek Öğrenime Sınavla Geçiş", ENUMGostergeOran.oran) };
            }
        }

        private PerformansAnahtar[] SiniflariDondur(string birimTur)
        {
            if (string.IsNullOrEmpty(birimTur))
                return TumSiniflar;
            string noktasiz = birimTur.Replace(".", "");
            if (noktasiz == "0203")
                return IlkOgretimSiniflar;
            else
                return OrtaOgretimSiniflar;
        }

        public ENUMAnahtarVeriGirisTur Anahtar2VeriGirisTur(int hamVeriKod, string anahtar1, string birimTur)
        {
            if (hamVeriKod == 2) return ENUMAnahtarVeriGirisTur.yok;

            return ENUMAnahtarVeriGirisTur.liste;
        }

        public PerformansAnahtar[] Anahtar2ListeDondur(int hamVeriKod, string anahtar1, string birimTur)
        {
            if (hamVeriKod == 5)
                if (anahtar1 == "SBSB" || anahtar1 == "SBSN")
                    return new PerformansAnahtar[] { 
                        new PerformansAnahtar("TRK", "Türkçe", ENUMGostergeOran.oran),
                        new PerformansAnahtar("MAT", "Matematik", ENUMGostergeOran.oran),
                        new PerformansAnahtar("FEN", "Fen Bilimleri", ENUMGostergeOran.oran)
                    };
                else
                    return new PerformansAnahtar[] { 
                        new PerformansAnahtar("1", "%5 dilim", ENUMGostergeOran.oran),
                        new PerformansAnahtar("2", "%6-50 dilim", ENUMGostergeOran.oran),
                        new PerformansAnahtar("3", "%50-100 dilim", ENUMGostergeOran.oran)
                    };
            else if (hamVeriKod == 3 || hamVeriKod == 4)
                return DersleriDondur(anahtar1, birimTur);
            else if (hamVeriKod == 1)
                return sinifSayisalBilgiler;
            return null;

        }

        private PerformansAnahtar[] DersleriDondur(string sinifKodstr, string birimTur)
        {

            string noktasiz = "";
            if (birimTur != null) noktasiz = birimTur.Replace(".", "");
            string sinifKod = sinifKodstr;
            ArrayList al = new ArrayList();
            for (int i = 0; i < TurKodDersAd.DersListesi.Length; i++)
            {
                if (TurKodDersAd.DersListesi[i].turKod.StartsWith(noktasiz) && TurKodDersAd.DersListesi[i].sinif == sinifKod)
                    al.Add(new PerformansAnahtar(TurKodDersAd.DersListesi[i].dersKod, TurKodDersAd.DersListesi[i].dersAd, ENUMGostergeOran.oran));
            }
            return (PerformansAnahtar[])al.ToArray(typeof(PerformansAnahtar));
        }

        public ENUMAnahtarVeriGirisTur Anahtar3VeriGirisTur(int hamVeriKod, string anahtar1, string anahtar2, string birimTur)
        {
            if (hamVeriKod == 3)
            {
                return ENUMAnahtarVeriGirisTur.liste;
            }
            else if (hamVeriKod == 5 && anahtar1 == "SBSB")
            {
                return ENUMAnahtarVeriGirisTur.liste;
            }

            return ENUMAnahtarVeriGirisTur.yok;
        }

        public PerformansAnahtar[] Anahtar3ListeDondur(int hamVeriKod, string anahtar1, string anahtar2, string birimTur)
        {
            if (hamVeriKod == 3)
            {
                return new PerformansAnahtar[] { 
                    new PerformansAnahtar("1", "Not 25 in altında", ENUMGostergeOran.oran),
                    new PerformansAnahtar("2", "25-50 arası", ENUMGostergeOran.oran),
                    new PerformansAnahtar("3", "50-75 arası", ENUMGostergeOran.oran),
                    new PerformansAnahtar("4", "75-100 arası", ENUMGostergeOran.oran)
                };
            }
            else if (hamVeriKod == 5 && anahtar1 == "SBSB")
            {
                return new PerformansAnahtar[] { 
                    new PerformansAnahtar("1", "%5 dilim", ENUMGostergeOran.oran),
                    new PerformansAnahtar("2", "%6-50 dilim", ENUMGostergeOran.oran),
                    new PerformansAnahtar("3", "%50-100 dilim", ENUMGostergeOran.oran)
                };
            }

            return null;
        }

        public ENUMAnahtarVeriGirisTur Anahtar4VeriGirisTur(int hamVeriKod, string anahtar1, string anahtar2, string anahtar3, string birimTur)
        {
            return ENUMAnahtarVeriGirisTur.yok;
        }

        public PerformansAnahtar[] Anahtar4ListeDondur(int hamVeriKod, string anahtar1, string anahtar2, string anahtar3, string birimTur)
        {
            return null;
        }

        public static Sonuc OkulVeriUretKaydet(IPEBServis servisPEB, Kullanici kullanan, OrganizasyonBirim orgBirim, int yil1, int yil2)
        {
            Random rastgele = new Random();
            Sonuc s2 = new Sonuc(true, "", "");
            PerformansAnahtar[] siniflar = TNS.PEB.TurKodDersAd.SiniflariBul(orgBirim.turKod);
            if (siniflar == null) return s2;
            int sinifSay = siniflar.Length;
            if (sinifSay <= 0) return s2;
            int subeSay = rastgele.Next(2, 5);
            double ogrenciSayisi = 0;
            int seviye = rastgele.Next(0, 3);
            TNSCollection dersNotliste = new TNSCollection();
            TNSCollection dersOrtListe = new TNSCollection();
            TNSCollection uekListe = new TNSCollection();
            PerformansHamVeriDetay[] sinifBilgi = SinifSayisalBilgileriUret(orgBirim, rastgele, seviye, subeSay, siniflar, out ogrenciSayisi, dersNotliste, dersOrtListe);
            PerformansHamVeriDetay[] okulBilgi = OkulSayisalBilgileriUret(orgBirim, rastgele, seviye, subeSay, ogrenciSayisi, uekListe);
            PerformansHamVeriDetay[] dersNotlar = (PerformansHamVeriDetay[])dersNotliste.ToArray(typeof(PerformansHamVeriDetay));
            PerformansHamVeriDetay[] dersOrtlar = (PerformansHamVeriDetay[])dersOrtListe.ToArray(typeof(PerformansHamVeriDetay));
            PerformansHamVeriDetay[] uekler = (PerformansHamVeriDetay[])uekListe.ToArray(typeof(PerformansHamVeriDetay));

            Sonuc s = new Sonuc();
            for (int yil = yil2; yil >= yil1; yil--)
            {
                s = Kaydet(servisPEB, kullanan, yil, orgBirim, 1, sinifBilgi);
                if (!s.islemSonuc) return s;

                s = Kaydet(servisPEB, kullanan, yil, orgBirim, 2, okulBilgi);
                if (!s.islemSonuc) return s;

                s = Kaydet(servisPEB, kullanan, yil, orgBirim, 3, dersNotlar);
                if (!s.islemSonuc) return s;

                s = Kaydet(servisPEB, kullanan, yil, orgBirim, 4, dersOrtlar);
                if (!s.islemSonuc) return s;

                s = Kaydet(servisPEB, kullanan, yil, orgBirim, 5, uekler);
                if (!s.islemSonuc) return s;

                DetaylariGuncelle(1, ref sinifBilgi, rastgele);
                DetaylariGuncelle(2, ref okulBilgi, rastgele);
                DetaylariGuncelle(3, ref dersNotlar, rastgele);
                DetaylariGuncelle(4, ref dersOrtlar, rastgele);
                DetaylariGuncelle(5, ref uekler, rastgele);
            }
            s.kayitSay = (sinifBilgi.Length + okulBilgi.Length + dersNotlar.Length + dersOrtlar.Length + uekler.Length) * (yil2 - yil1 + 1);
            return s;
        }

        public static Sonuc Kaydet(IPEBServis servisPEB, Kullanici kullanan, int yil, OrganizasyonBirim orgBirim, int eNUMPerformansHamVeriTur, PerformansHamVeriDetay[] performansHamVeriDetay)
        {
            if (performansHamVeriDetay == null || performansHamVeriDetay.Length <= 0)
            {
                return new Sonuc(true, "", "");
            }
            TNS.PEB.PerformansHamVeri phVeri = new PerformansHamVeri();
            phVeri.yil = yil;
            phVeri.bilgiTuru = eNUMPerformansHamVeriTur;
            phVeri.birim = orgBirim;
            phVeri.detay = performansHamVeriDetay;
            return servisPEB.PerformansHamVeriKaydet(kullanan, phVeri);
        }

        private static void DetaylariGuncelle(int bilgiTuru, ref PerformansHamVeriDetay[] phVeriDetaylar, Random rastgele)
        {
            for (int i = 0; i < phVeriDetaylar.Length; i++)
            {
                int isaret = rastgele.Next(0, 2);
                if (isaret < 1) isaret = -1;
                else isaret = 1;
                int yuzdelikDegisimOrani = rastgele.Next(20, 30);
                int yuvarla = 0;

                double degisim = Math.Round(phVeriDetaylar[i].bilgi * yuzdelikDegisimOrani / 1000, yuvarla);
                if (degisim == 0) degisim = 1;
                if (bilgiTuru == 1 && phVeriDetaylar[i].anahtar2 == "01") degisim = 0;
                phVeriDetaylar[i].bilgi += isaret * degisim;
                if (phVeriDetaylar[i].bilgi < 0) phVeriDetaylar[i].bilgi = 0;
            }
        }

        private static PerformansHamVeriDetay[] SinifSayisalBilgileriUret(OrganizasyonBirim orgBirim, Random rastgele, int seviye, int subeSay, PerformansAnahtar[] siniflar, out double ogrenciSayisi, TNSCollection dersNotliste, TNSCollection dersOrtListe)
        {
            ogrenciSayisi = 0;
            TNSCollection liste = new TNSCollection();
            foreach (PerformansAnahtar sinif in siniflar)
            {
                string sinifKod = sinif.kod;
                liste.Add(SinifBilgiUret(sinifKod, "01", "-", subeSay, 1));
                for (int subeNo = 0; subeNo < subeSay; subeNo++)
                {
                    int sinifOgrenciSay = rastgele.Next(15, 60);
                    int kOgrenciSay = (sinifOgrenciSay * rastgele.Next(40, 60)) / 100;
                    int eOgrenciSay = sinifOgrenciSay - kOgrenciSay;
                    ogrenciSayisi += sinifOgrenciSay;
                    int dezSay = (sinifOgrenciSay * rastgele.Next(3, 6)) / 100;
                    int ozelSay = (sinifOgrenciSay * rastgele.Next(3, 6)) / 100;
                    int YatSay = (sinifOgrenciSay * rastgele.Next(3, 6)) / 100;
                    int BasariliOgr = (sinifOgrenciSay * RastGeleSeviyeliBul(rastgele, 10, 25, seviye, true)) / 100;

                    string subeKod = SubeKodBul(subeNo);
                    DersNotGrupUret(orgBirim, rastgele, seviye, sinifKod, subeKod, sinifOgrenciSay, dersNotliste, dersOrtListe);

                    liste.Add(SinifBilgiUret(sinifKod, "02", subeKod, kOgrenciSay, sinifOgrenciSay));
                    liste.Add(SinifBilgiUret(sinifKod, "03", subeKod, eOgrenciSay, sinifOgrenciSay));
                    liste.Add(SinifBilgiUret(sinifKod, "04", subeKod, dezSay, sinifOgrenciSay));
                    liste.Add(SinifBilgiUret(sinifKod, "05", subeKod, ozelSay, sinifOgrenciSay));
                    liste.Add(SinifBilgiUret(sinifKod, "06", subeKod, YatSay, sinifOgrenciSay));
                    liste.Add(SinifBilgiUret(sinifKod, "07", subeKod, BasariliOgr, sinifOgrenciSay));

                }
            }
            PerformansHamVeriDetay[] phVeriDetaylar = (PerformansHamVeriDetay[])liste.ToArray(typeof(PerformansHamVeriDetay));
            return phVeriDetaylar;
        }

        private static int RastGeleSeviyeliBul(Random rastgele, int alt, int ustHaric, int seviye, bool artanIyi)
        {
            if (seviye < 0 || seviye > 2) return rastgele.Next(alt, ustHaric);
            int fark = (ustHaric - alt) / 3;
            if (!artanIyi)
                seviye = 2 - seviye;
            return rastgele.Next(alt + fark * seviye, alt + fark * (seviye + 1) + 1);
        }

        private static PerformansHamVeriDetay SinifBilgiUret(string sinifKod, string eNUMSinifSayisalBilgiler, string subeKod, int bilgiDeger, int sinifOgrenciSay)
        {
            PerformansHamVeriDetay phVeriDetay = new PerformansHamVeriDetay();
            phVeriDetay.anahtar1 = sinifKod;
            phVeriDetay.anahtar2 = eNUMSinifSayisalBilgiler;
            phVeriDetay.anahtar3 = subeKod;
            phVeriDetay.bilgi = bilgiDeger;
            phVeriDetay.bolen = sinifOgrenciSay;

            return phVeriDetay;
        }

        private static string SubeKodBul(int subeNo)
        {
            return new string((char)(65 + subeNo), 1);
        }

        public static PerformansHamVeriDetay[] OkulSayisalBilgileriUret(OrganizasyonBirim orgBirim, Random rastgele, int seviye, int subeSay, double ogrenciSayisi, TNSCollection uekListe)
        {
            TNSCollection liste = new TNSCollection();

             RastGeleSeviyeliBul(rastgele, 20, 40, seviye, false);

             liste.Add(OkulSayisalBilgiVerisiUret("01", (int)Math.Round(ogrenciSayisi / RastGeleSeviyeliBul(rastgele, 20, 40, seviye, false)), (int)ogrenciSayisi));
             liste.Add(OkulSayisalBilgiVerisiUret("02", (int)Math.Round(ogrenciSayisi / RastGeleSeviyeliBul(rastgele, 10, 50, seviye, false)), (int)ogrenciSayisi));
             liste.Add(OkulSayisalBilgiVerisiUret("03", RastGeleSeviyeliBul(rastgele, 0, 10, seviye, false), (int)ogrenciSayisi));
             liste.Add(OkulSayisalBilgiVerisiUret("04", (int)(RastGeleSeviyeliBul(rastgele, 2500, 3500, seviye, false) * ogrenciSayisi), (int)ogrenciSayisi));

            double egitimOgrenciGunSayisi = ogrenciSayisi * 180;
            liste.Add(OkulSayisalBilgiVerisiUret("11", (int)Math.Round(egitimOgrenciGunSayisi * RastGeleSeviyeliBul(rastgele, 900, 990, seviye, true) / 1000), (int)egitimOgrenciGunSayisi));
            liste.Add(OkulSayisalBilgiVerisiUret("12", (int)Math.Round(ogrenciSayisi * RastGeleSeviyeliBul(rastgele, 900, 990, seviye, true) / 1000), (int)ogrenciSayisi));

            liste.Add(OkulSayisalBilgiVerisiUret("13", (int)Math.Round(ogrenciSayisi * RastGeleSeviyeliBul(rastgele, 50, 300, seviye, false) / 1000), (int)ogrenciSayisi));
            liste.Add(OkulSayisalBilgiVerisiUret("14", (int)Math.Round(ogrenciSayisi * RastGeleSeviyeliBul(rastgele, 800, 950, seviye, true) / 1000), (int)ogrenciSayisi));
            liste.Add(OkulSayisalBilgiVerisiUret("15", (int)Math.Round(ogrenciSayisi * RastGeleSeviyeliBul(rastgele, 900, 990, seviye, true) / 1000), (int)ogrenciSayisi));
            liste.Add(OkulSayisalBilgiVerisiUret("16", (int)Math.Round(ogrenciSayisi * RastGeleSeviyeliBul(rastgele, 100, 300, seviye, false) / 1000), (int)ogrenciSayisi));
            liste.Add(OkulSayisalBilgiVerisiUret("17", (int)Math.Round(400.0 * rastgele.Next(750, 950) / 1000), 400));

            int ulusEtk = RastGeleSeviyeliBul(rastgele, 0, 20, seviye, true);
            int ulusAraEtk = RastGeleSeviyeliBul(rastgele, 0, 5, seviye, true);
            liste.Add(OkulSayisalBilgiVerisiUret("05", (int)Math.Round(ogrenciSayisi * RastGeleSeviyeliBul(rastgele, 50, 150, seviye, true) / 1000), (int)ogrenciSayisi));
            liste.Add(OkulSayisalBilgiVerisiUret("06", RastGeleSeviyeliBul(rastgele, 5, 20, seviye, true), 1));
            liste.Add(OkulSayisalBilgiVerisiUret("07", ulusEtk, 1));
            liste.Add(OkulSayisalBilgiVerisiUret("08", ulusAraEtk, 1));
            liste.Add(OkulSayisalBilgiVerisiUret("09", ulusEtk * rastgele.Next(1, 5), 1));
            liste.Add(OkulSayisalBilgiVerisiUret("10", ulusAraEtk * rastgele.Next(0, 3), 1));
            UEKUret(orgBirim, ogrenciSayisi, rastgele, uekListe);

            return (PerformansHamVeriDetay[])liste.ToArray(typeof(PerformansHamVeriDetay));
        }

        private static void UEKUret(OrganizasyonBirim orgBirim, double anaSay, Random rastgele, TNSCollection liste)
        {
            if (anaSay == 0) return;
            IHamveriTurTanim hamVeri = new MEBHamveriTurTanim();
            PerformansHamVeriDetay phVeriDetay = null;
            if (orgBirim.turKod.StartsWith("02"))
            {
                PerformansAnahtar[] sbsKonular = hamVeri.Anahtar2ListeDondur(5, "SBSB", null);
                foreach (PerformansAnahtar ders in sbsKonular)
                {
                    double say = anaSay;

                    for (int i = 0; i < 3; i++)
                    {
                        phVeriDetay = new PerformansHamVeriDetay();
                        phVeriDetay.anahtar1 = "SBSB";
                        phVeriDetay.anahtar2 = ders.kod;
                        phVeriDetay.anahtar3 = (i + 1).ToString();

                        if (i == 2)
                            phVeriDetay.bilgi = say;
                        else
                        {
                            int min = 0;
                            int max = 25;
                            if (i == 0) { min = 0; max = 12; }
                            else if (i == 1) { min = 15; max = 65; }
                            int oran = rastgele.Next(min, max);
                            int grupSay = (int)(say * oran / 100);
                            phVeriDetay.bilgi = grupSay;
                            say -= grupSay;
                        }
                        phVeriDetay.bolen = anaSay;
                        liste.Add(phVeriDetay);
                    }
                    phVeriDetay = new PerformansHamVeriDetay();
                    phVeriDetay.anahtar1 = "SBSN";
                    phVeriDetay.anahtar2 = ders.kod;

                    phVeriDetay.bolen = anaSay;
                    phVeriDetay.bilgi = phVeriDetay.bolen * rastgele.Next(350, 510);
                    liste.Add(phVeriDetay);
                }
            }
            else
            {
                double say = anaSay;

                for (int i = 0; i < 3; i++)
                {
                    phVeriDetay = new PerformansHamVeriDetay();
                    phVeriDetay.anahtar1 = "YGSB";
                    phVeriDetay.anahtar2 = (i + 1).ToString();

                    if (i == 2)
                        phVeriDetay.bilgi = say;
                    else
                    {
                        int min = 0;
                        int max = 25;
                        if (i == 0) { min = 0; max = 12; }
                        else if (i == 1) { min = 15; max = 65; }
                        int oran = rastgele.Next(min, max);
                        int grupSay = (int)(say * oran / 100);
                        phVeriDetay.bilgi = grupSay;
                        say -= grupSay;
                    }
                    phVeriDetay.bolen = anaSay;
                    liste.Add(phVeriDetay);
                }
            }
        }

        private static PerformansHamVeriDetay OkulSayisalBilgiVerisiUret(string eNUMOkulSayisalBilgiler, int bilgi, int bolen)
        {
            PerformansHamVeriDetay phVeriDetay = new PerformansHamVeriDetay();
            phVeriDetay.anahtar1 = eNUMOkulSayisalBilgiler;
            phVeriDetay.bilgi = bilgi;
            phVeriDetay.bolen = bolen;
            return phVeriDetay;
        }

        public static void DersNotGrupUret(OrganizasyonBirim orgBirim, Random rastgele, int seviye, string sinifKod, string subeKod, int anaSay, TNSCollection liste, TNSCollection notOrtalamaListe)
        {
            ArrayList dersler = new ArrayList();
            TurKodDersAd.DersleriBul(orgBirim.turKod, sinifKod, dersler);

            for (int dersNo = 0; dersNo < dersler.Count; dersNo++)
            {
                object[] tut2 = (object[])dersler[dersNo];
                string dersKod = (string)tut2[0];
                int say = anaSay;
                double[] notlar = new double[4];
                for (int i = 0; i < 4; i++)
                {
                    notlar[i] = 0;
                    PerformansHamVeriDetay phVeriDetay = new PerformansHamVeriDetay();
                    phVeriDetay.anahtar1 = sinifKod;
                    phVeriDetay.anahtar2 = dersKod;
                    phVeriDetay.anahtar3 = (i + 1).ToString();
                    phVeriDetay.anahtar4 = subeKod;
                    if (i == 3)
                        phVeriDetay.bilgi = say;
                    else
                    {
                        int min = 0;
                        int max = 25;
                        if (i == 0) { min = 5; max = 30; }
                        else if (i == 1) { min = 20; max = 40; }
                        else if (i == 2) { min = 35; max = 70; }

                        int oran = RastGeleSeviyeliBul(rastgele, min, max, seviye, true);

                        int grupSay = say * oran / 100;
                        phVeriDetay.bilgi = grupSay;
                        say -= grupSay;
                    }
                    phVeriDetay.bolen = anaSay;
                    notlar[i] = phVeriDetay.bilgi;
                    liste.Add(phVeriDetay);
                }

                double notOrtalama = (rastgele.Next(12, 22) * notlar[0] + rastgele.Next(35, 45) * notlar[1] + rastgele.Next(58, 72) * notlar[2] + rastgele.Next(80, 96) * notlar[3]);
                PerformansHamVeriDetay phNotDetay = new PerformansHamVeriDetay();
                phNotDetay.anahtar1 = sinifKod;
                phNotDetay.anahtar2 = dersKod;
                phNotDetay.anahtar3 = subeKod;
                phNotDetay.anahtar4 = "-";
                phNotDetay.bilgi = notOrtalama;
                phNotDetay.bolen = anaSay;
                notOrtalamaListe.Add(phNotDetay);
            }
        }
    }


    [Serializable()]
    public class TurKodDersAd
    {
        public string turKod = "";
        public string sinif = "";
        public string dersKod = "";
        public string dersAd = "";

        public static TurKodDersAd[] DersListesi =
        {
            new TurKodDersAd("0203", "01", "TRK", "Türkçe"),
            new TurKodDersAd("0203", "01", "HB", "Hayat Bilgisi"),
            new TurKodDersAd("0203", "01", "MAT", "Matematik"),
            new TurKodDersAd("0203", "02", "TRK", "Türkçe"),
            new TurKodDersAd("0203", "02", "HB", "Hayat Bilgisi"),
            new TurKodDersAd("0203", "02", "MAT", "Matematik"),
            new TurKodDersAd("0203", "03", "TRK", "Türkçe"),
            new TurKodDersAd("0203", "03", "HB", "Hayat Bilgisi"),
            new TurKodDersAd("0203", "03", "MAT", "Matematik"),
            new TurKodDersAd("0203", "04", "TRK", "Türkçe"),
            new TurKodDersAd("0203", "04", "HB", "Hayat Bilgisi"),
            new TurKodDersAd("0203", "04", "MAT", "Matematik"),
            new TurKodDersAd("0203", "05", "TRK", "Türkçe"),
            new TurKodDersAd("0203", "05", "HB", "Hayat Bilgisi"),
            new TurKodDersAd("0203", "05", "MAT", "Matematik"),
            new TurKodDersAd("0203", "06", "TRK", "Türkçe"),
            new TurKodDersAd("0203", "06", "FEN", "Fen Bilimleri"),
            new TurKodDersAd("0203", "06", "SOS", "Sosyal Bilgiler"),
            new TurKodDersAd("0203", "06", "MAT", "Matematik"),
            new TurKodDersAd("0203", "07", "TRK", "Türkçe"),
            new TurKodDersAd("0203", "07", "FEN", "Fen Bilimleri"),
            new TurKodDersAd("0203", "07", "SOS", "Sosyal Bilgiler"),
            new TurKodDersAd("0203", "07", "MAT", "Matematik"),
            new TurKodDersAd("0203", "08", "TRK", "Türkçe"),
            new TurKodDersAd("0203", "08", "FEN", "Fen Bilimleri"),
            new TurKodDersAd("0203", "08", "SOS", "Sosyal Bilgiler"),
            new TurKodDersAd("0203", "08", "MAT", "Matematik"),

            new TurKodDersAd("0301", "09", "TRK", "Türkçe"),
            new TurKodDersAd("0301", "09", "FIZ", "Fizik"),
            new TurKodDersAd("0301", "09", "KIM", "Kimya"),
            new TurKodDersAd("0301", "09", "BIO", "Biyoloji"),
            new TurKodDersAd("0301", "09", "MAT", "Matematik"),
            new TurKodDersAd("0301", "10", "TRK", "Türkçe"),
            new TurKodDersAd("0301", "10", "FIZ", "Fizik"),
            new TurKodDersAd("0301", "10", "KIM", "Kimya"),
            new TurKodDersAd("0301", "10", "BIO", "Biyoloji"),
            new TurKodDersAd("0301", "10", "MAT", "Matematik"),
            new TurKodDersAd("0301", "11", "TRK", "Türkçe"),
            new TurKodDersAd("0301", "11", "FIZ", "Fizik"),
            new TurKodDersAd("0301", "11", "KIM", "Kimya"),
            new TurKodDersAd("0301", "11", "BIO", "Biyoloji"),
            new TurKodDersAd("0301", "11", "MAT", "Matematik"),
            new TurKodDersAd("0301", "12", "TRK", "Türkçe"),
            new TurKodDersAd("0301", "12", "FIZ", "Fizik"),
            new TurKodDersAd("0301", "12", "KIM", "Kimya"),
            new TurKodDersAd("0301", "12", "BIO", "Biyoloji"),
            new TurKodDersAd("0301", "12", "MAT", "Matematik"),
        };

        public static bool ArrayListteVarMi(ArrayList al, string kod)
        {
            foreach (object[] item in al)
            {
                string itemKod = (string)item[0];
                if (itemKod.Equals(kod)) return true;
            }
            return false;
        }

        public static string SinifAd(string sinif)
        {
            string tut = sinif;
            if (tut.StartsWith("0")) tut = tut.Substring(1);
            return tut + ". sınıf";
        }

        public static void SiniflariDersleriBul(string ob_turKod, ArrayList siniflar, ArrayList dersler)
        {
            ob_turKod = ob_turKod.Replace(".", "");
            for (int i = 0; i < TNS.PEB.TurKodDersAd.DersListesi.Length; i++)
            {
                TurKodDersAd ders = TurKodDersAd.DersListesi[i];
                if (ders.turKod == ob_turKod)
                {
                    if (dersler != null)
                    {
                        if (!ArrayListteVarMi(dersler, ders.dersKod))
                        {
                            dersler.Add(new object[] { ders.dersKod, ders.dersAd });
                        }
                    }
                    if (siniflar != null)
                    {
                        if (!ArrayListteVarMi(siniflar, ders.sinifToString()))
                        {
                            siniflar.Add(new object[] { ders.sinifToString(), SinifAd(ders.sinif) });
                        }
                    }
                }
            }

        }

        public TurKodDersAd(string turKod, string sinif, string dersKod, string dersAd)
        {
            this.turKod = turKod;
            this.sinif = sinif;
            this.dersKod = dersKod;
            this.dersAd = dersAd;
        }


        public static string AdBul(TurKodDersAd[] liste, string turKod, string sinif, string dersKod)
        {
            turKod = turKod.Replace(".", "");
            for (int i = 0; i < liste.Length; i++)
            {
                if (liste[i].turKod == turKod && liste[i].sinif == sinif && liste[i].dersKod == dersKod)
                    return liste[i].dersAd;
            }
            return "";
        }

        public static string DersAdBul(string turKod, string sinif, string dersKod)
        {
            return AdBul(DersListesi, turKod, sinif, dersKod);
        }

        public static string KonuAdBul(string turKod, int sinif, string dersKod)
        {
            return dersKod;
        }

        public string sinifToString()
        {
            return sinif;
        }

        internal static void DersleriBul(string ob_turKod, string sinifKod, ArrayList dersler)
        {
            ob_turKod = ob_turKod.Replace(".", "");
            for (int i = 0; i < TNS.PEB.TurKodDersAd.DersListesi.Length; i++)
            {
                TurKodDersAd ders = TurKodDersAd.DersListesi[i];
                if (ders.turKod == ob_turKod && ders.sinif == sinifKod)
                {
                    if (!ArrayListteVarMi(dersler, ders.dersKod))
                    {
                        dersler.Add(new object[] { ders.dersKod, ders.dersAd });
                    }
                }
            }
        }

        internal static PerformansAnahtar[] SiniflariBul(string orgKod)
        {
            if (orgKod.StartsWith("02")) return TNS.PEB.MEBHamveriTurTanim.IlkOgretimSiniflar;
            else if (orgKod.StartsWith("03")) return TNS.PEB.MEBHamveriTurTanim.OrtaOgretimSiniflar;
            return null;
        }
    }

}
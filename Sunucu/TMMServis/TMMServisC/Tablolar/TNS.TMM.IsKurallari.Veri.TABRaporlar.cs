using System;
using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır raporları ile ilgili fonksiyonları tutan sınıf
    /// </summary>
    public class TABRaporlar : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABRaporlar(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADSTOKHAREKETVIEW = "TASSTOKHAREKETVIEW";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSTOKHAREKETVIEW : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Muhasebe adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEAD = 2,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 3,
            /// <summary>
            /// Harcama birim adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMAD = 4,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 5,
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 6,
            /// <summary>
            /// Fiş tarih alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISTARIH = 7,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 8,
            /// <summary>
            /// Ambar adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARAD = 9,
            /// <summary>
            /// İşlem tipi kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTIPIKOD = 10,
            /// <summary>
            /// İşlem tipi adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTIPIAD = 11,
            /// <summary>
            /// İşlem tipi türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTIPITUR = 12,
            /// <summary>
            /// Nereden geldi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDENGELDI = 13,
            /// <summary>
            /// Nereye gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREYEGITTI = 14,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GMUHASEBEKOD = 15,
            /// <summary>
            /// Harcama kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GHARCAMAKOD = 16,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GAMBARKOD = 17,
            /// <summary>
            /// Durum alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 18,
            /// <summary>
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 19,
            /// <summary>
            /// Hesap plan adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANAD = 20,
            /// <summary>
            /// Ölçü birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMKOD = 21,
            /// <summary>
            /// Ölçü birim adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMAD = 22,
            /// <summary>
            /// Miltar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 23,
            /// <summary>
            /// Kdva oran alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 24,
            /// <summary>
            /// Birim fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 25,
            /// <summary>
            /// Birim fiyat kdv alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYATKDVLI = 26,
            /// <summary>
            /// İl kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILKOD = 27,
            /// <summary>
            /// İl Adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILAD = 28,
            /// <summary>
            /// İlçe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILCEKOD = 29,
            /// <summary>
            /// İlçe adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILCEAD = 30,
            /// <summary>
            /// Kime gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KIMEGITTI = 31,
            /// <summary>
            /// Bölge kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BOLGEKOD = 32,
            /// <summary>
            /// Bölge adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BOLGEAD = 33
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSTOKHAREKETVIEW = { "YIL", "MUHASEBEKOD", "MUHASEBEAD", "HARCAMABIRIMKOD",
                                                   "HARCAMABIRIMAD", "FISNO", "SIRANO", "FISTARIH",
                                                   "AMBARKOD", "AMBARAD", "ISLEMTIPIKOD", "ISLEMTIPIAD", "ISLEMTIPITUR",
                                                   "NEREDENGELDI", "NEREYEGITTI", "GMUHASEBEKOD", "GHARCAMAKOD", "GAMBARKOD",
                                                   "DURUM", "HESAPPLANKOD", "HESAPPLANAD", "OLCUBIRIMKOD", "OLCUBIRIMAD",
                                                   "MIKTAR", "KDVORAN", "BIRIMFIYAT", "BIRIMFIYATKDVLI",
                                                   "ILKOD", "ILAD", "ILCEKOD", "ILCEAD", "KIMEGITTI", "BOLGEKOD", "BOLGEAD"};

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADSICILHAREKETVIEW = "TASSICILHAREKETVIEW";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSICILHAREKETVIEW : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Muhasebe adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEAD = 2,
            /// <summary>
            /// Harcama birimi kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 3,
            /// <summary>
            /// Harcama birimi adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMAD = 4,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 5,
            /// <summary>
            /// Ambar adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARAD = 6,
            /// <summary>
            /// Fiş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISTARIH = 7,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 8,
            /// <summary>
            /// Nereden geldi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDENGELDI = 9,
            /// <summary>
            /// Nereye gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREYEGITTI = 10,
            /// <summary>
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 11,
            /// <summary>
            /// Hesap plan adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANAD = 12,
            /// <summary>
            /// Kdvli birim fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVLIBIRIMFIYAT = 13,
            /// <summary>
            /// Sicil no hareket personel sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILNOHAREKETPRSICILNO = 14,
            /// <summary>
            /// Sicil no hareket sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILNOHAREKETSIRANO = 15,
            /// <summary>
            /// Sicil no hareket sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILNOHAREKETSICILNO = 16,
            /// <summary>
            /// Ölçü birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMKOD = 17,
            /// <summary>
            /// Ölçü birim adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMAD = 18,
            /// <summary>
            /// İl kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILKOD = 19,
            /// <summary>
            /// İl adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILAD = 20,
            /// <summary>
            /// İlçe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILCEKOD = 21,
            /// <summary>
            /// İlçe adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILCEAD = 22,
            /// <summary>
            /// İşlem tipi kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTIPKOD = 23,
            /// <summary>
            /// İşlem tipi adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTIPAD = 24,
            /// <summary>
            /// İşlem tipi türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTIPTUR = 25,
            /// <summary>
            /// Kime gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KIMEGITTI = 26,
            /// <summary>
            /// Zimmet özellik alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ZIMMETOZELLIK = 27,
            /// <summary>
            /// Gönderen muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GONMUHASEBEKOD = 28,
            /// <summary>
            /// Gönderen harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GONHARCAMABIRIMKOD = 29,
            /// <summary>
            /// Gönderen ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GONAMBARKOD = 30,
            /// <summary>
            /// Marka kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MARKAKOD = 31,
            /// <summary>
            /// Model kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODELKOD = 32,
            /// <summary>
            /// Şase no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SASENO = 33,
            /// <summary>
            /// Motor no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MOTORNO = 34,
            /// <summary>
            /// Plaka alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PLAKA = 35,
            /// <summary>
            /// Cilt no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CILTNO = 36,
            /// <summary>
            /// Dil alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DIL = 37,
            /// <summary>
            /// Yazar adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAZARADI = 38,
            /// <summary>
            /// Adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADI = 39,
            /// <summary>
            /// Yayın yeri alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAYINYERI = 40,
            /// <summary>
            /// Yayın tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAYINTARIHI = 41,
            /// <summary>
            /// Özellik nereden geldi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELLIKNEREDENGELDI = 42,
            /// <summary>
            /// Boyutları alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BOYUTLARI = 43,
            /// <summary>
            /// Satır sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATIRSAYISI = 44,
            /// <summary>
            /// Yaprak sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAPRAKSAYISI = 45,
            /// <summary>
            /// Sayfa sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYFASAYISI = 46,
            /// <summary>
            /// Cilt türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CILTTURU = 47,
            /// <summary>
            /// Çeşit alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CESIDI = 48,
            /// <summary>
            /// Yeri konusu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YERIKONUSU = 49,
            /// <summary>
            /// Geliş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GELISTARIHI = 50,
            /// <summary>
            /// Nerede bulundu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDEBULUNDU = 51,
            /// <summary>
            /// Çağı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CAGI = 52,
            /// <summary>
            /// Durum maddesi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUMUMADDESI = 53,
            /// <summary>
            /// Önyüz alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONYUZ = 54,
            /// <summary>
            /// Arka yüz alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ARKAYUZ = 55,
            /// <summary>
            /// Fotoğraf alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FOTOGRAF = 56,
            /// <summary>
            /// Puan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PUAN = 57,
            /// <summary>
            /// Dış sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DISSICILNO = 58,
            /// <summary>
            /// Değer artışı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGERARTISI = 59,
            /// <summary>
            /// Marka Adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MARKAAD = 60,
            /// <summary>
            /// Model Adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODELAD = 61
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSICILHAREKETVIEW = { "YIL", "MUHASEBEKOD", "MUHASEBEAD", "HARCAMABIRIMKOD",
                                                   "HARCAMABIRIMAD", "AMBARKOD", "AMBARAD", "FISTARIH",
                                                   "FISNO", "NEREDENGELDI", "NEREYEGITTI", "HESAPPLANKOD",
                                                   "HESAPPLANAD", "KDVLIBIRIMFIYAT", "SICILNOHAREKETPRSICILNO", "SICILNOHAREKETSIRANO",
                                                   "SICILNOHAREKETSICILNO", "OLCUBIRIMKOD", "OLCUBIRIMAD", "ILKOD", "ILAD", "ILCEKOD", "ILCEAD",
                                                   "ISLEMTIPKOD", "ISLEMTIPAD", "ISLEMTIPTUR", "KIMEGITTI", "ZIMMETOZELLIK",
                                                   "GONMUHASEBEKOD", "GONHARCAMABIRIMKOD", "GONAMBARKOD", "MARKAKOD",
                                                   "MODELKOD", "SASENO", "MOTORNO", "PLAKA", "CILTNO", "DIL", "YAZARADI", "ADI", "YAYINYERI",
                                                   "YAYINTARIHI", "OZELLIKNEREDENGELDI", "BOYUTLARI", "SATIRSAYISI", "YAPRAKSAYISI",
                                                   "SAYFASAYISI", "CILTTURU", "CESIDI", "YERIKONUSU", "GELISTARIHI",
                                                   "NEREDEBULUNDU", "CAGI", "DURUMUMADDESI", "ONYUZ", "ARKAYUZ", "FOTOGRAF",
                                                   "PUAN", "DISSICILNO", "DEGERARTISI","MARKAAD","MODELAD" };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADIL = "TASIL";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADISTEKKARSILANANVIEW = "TASISTEKKARSILANAN";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarIL
        {
            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARIL = { "KOD", "AD" };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADILCE = "TASILCE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarILCE
        {
            /// <summary>
            /// İl kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILKOD = 0,
            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 1,
            /// <summary>
            /// Ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 2
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARILCE = { "ILKOD", "KOD", "AD" };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMUHASEBE = "TASMUHASEBE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarMUHASEBE
        {
            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// İl kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILKOD = 2,
            /// <summary>
            /// İlçe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILCEKOD = 3,
            /// <summary>
            /// Kapalı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAPALI = 4,
            /// <summary>
            /// Bölge kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BOLGEKOD = 5,
            /// <summary>
            /// Kdv mükellefi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVMUKELLEFI = 6,
            /// <summary>
            /// İcmal muaf alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ICMALMUAF = 7
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMUHASEBE = { "KOD", "AD", "ILKOD", "ILCEKOD", "KAPALI", "BOLGEKOD", "KDVMUKELLEFI", "ICMALMUAF" };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADHARCAMA = "TASHARCAMABIRIM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarHARCAMA : int
        {
            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARHARCAMA = { "KOD", "AD", "MUHASEBEKOD" };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTIF = "TASISLEMFORM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTIF : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 3,
            /// <summary>
            /// Fiş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISTARIH = 4,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 5,
            /// <summary>
            /// İşlem tipi kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTIPKOD = 6,
            /// <summary>
            /// Muayene no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUAYENENO = 7,
            /// <summary>
            /// Muayene tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUAYENETARIH = 8,
            /// <summary>
            /// Dayanak no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAYANAKNO = 9,
            /// <summary>
            /// Dayanak tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAYANAKTARIH = 10,
            /// <summary>
            /// Nereden geldi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDENGELDI = 11,
            /// <summary>
            /// Nereye gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREYEGITTI = 12,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GMUHASEBEKOD = 13,
            /// <summary>
            /// Harcama kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GHARCAMAKOD = 14,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GAMBARKOD = 15,
            /// <summary>
            /// Durum alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 16,
            /// <summary>
            /// İşlem tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 17,
            /// <summary>
            /// İşlem yapan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 18,
            /// <summary>
            /// Fatura no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURANO = 19,
            /// <summary>
            /// Fatura tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURATARIH = 20,
            /// <summary>
            /// Kime gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KIMEGITTI = 21
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTIF = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO","FISTARIH", "AMBARKOD", "ISLEMTIPIKOD",
                                    "MUAYENENO","MUAYENETARIH","DAYANAKNO","DAYANAKTARIH","NEREDENGELDI",
                                    "NEREYEGITTI","GMUHASEBEKOD","GHARCAMAKOD","GAMBARKOD","DURUM","ISLEMTARIH","ISLEMYAPAN","FATURANO","FATURATARIH","KIMEGITTI"};

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTIFDETAY = "TASISLEMDETAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTIFDETAY : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 3,
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
            /// <summary>
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 5,
            /// <summary>
            /// Sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILNO = 6,
            /// <summary>
            /// Miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 7,
            /// <summary>
            /// Kdv oran alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 8,
            /// <summary>
            /// Birim fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 9,
            /// <summary>
            /// Birim Fiyat KDVli alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYATKDVLI = 10,
            /// <summary>
            /// Sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ESICILNO = 11,
            /// <summary>
            /// Alım tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EALIMTARIHI = 12,
            /// <summary>
            /// Tedarik şekli alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ETEDARIKSEKLI = 13,
            /// <summary>
            /// Açıklama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EACIKLAMA = 14,
            /// <summary>
            /// Son tarih alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SONTARIH = 15
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTIFDETAY = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "SIRANO", "HESAPPLANKOD", "SICILNO", "MIKTAR", "KDVORAN", "BIRIMFIYAT", "BIRIMFIYATKDVLI", "ESICILNO", "EALIMTARIHI", "ETEDARIKSEKLI", "EACIKLAMA", "SONTARIH" };

        /// <summary>
        /// Parametre olarak verilen Yıl, Harcama Birim Kodu ve Hesap kodunu kıstas olarak kullanarak,
        /// kıstasa uygun Hesap Planı Ortalama Fiyat ve Miktarlarını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan,  Int türünde nesne</param>
        /// <param name="harcamaBirimKod">Harcama Birim kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="hesapKod">Hesap kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Hesap Plani Ortalama Fiyat ve Miktarlarını tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader HesapPlaniOrtalamaFiyatMiktarListele(int yil, string harcamaBirimKod, string hesapKod)
        {
            string sql = "SELECT " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR];
            sql += " * " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BIRIMFIYAT] + ")";
            sql += " / " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + ")";
            sql += " FROM " + TABLOADSTOKHAREKETVIEW;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", yil));
            if (!string.IsNullOrEmpty(harcamaBirimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "LIKE", harcamaBirimKod.Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(hesapKod))
            {
                string[] yler = hesapKod.Split(',');
                string kosul = "";
                for (int i = 0; i < yler.Length; i++)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", yler[i].Replace(".", "") + "%"));
                }
                if (kosul != "")
                    sql += " AND (" + kosul + ")";
            }
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "=", (int)ENUMIslemTipi.SATINALMAGIRIS));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            sql += " GROUP BY " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];

            sql += " ORDER BY " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Yetki Değerlerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Form bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerlerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Form bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader HarcamaBirimiSonTIFListele(int yil, TNSDateTime islemTarihIlk, TNSDateTime islemTarihSon, string[] yetkiDeger)
        {
            string sql = "SELECT ";
            sql += TABLOADIL + "." + ALANLARIL[(int)ENUMAlanlarIL.KOD];
            sql += ", " + TABLOADIL + "." + ALANLARIL[(int)ENUMAlanlarIL.AD];
            sql += ", " + TABLOADILCE + "." + ALANLARILCE[(int)ENUMAlanlarILCE.KOD];
            sql += ", " + TABLOADILCE + "." + ALANLARILCE[(int)ENUMAlanlarILCE.AD];
            sql += ", " + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.KOD];
            sql += ", " + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.AD];
            sql += ", " + TABLOADHARCAMA + "." + ALANLARHARCAMA[(int)ENUMAlanlarHARCAMA.KOD];
            sql += ", " + TABLOADHARCAMA + "." + ALANLARHARCAMA[(int)ENUMAlanlarHARCAMA.AD];
            sql += ", " + VeriErisim.IslecBul("MAX") + "(" + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.FISNO] + ")";
            sql += " FROM " + TABLOADIL + ", " + TABLOADILCE + ", " + TABLOADMUHASEBE + ", " + TABLOADHARCAMA + ", " + TABLOADTIF;
            sql += " WHERE " + TABLOADIL + "." + ALANLARIL[(int)ENUMAlanlarIL.KOD] + "=" + TABLOADILCE + "." + ALANLARILCE[(int)ENUMAlanlarILCE.ILKOD];
            sql += " AND " + TABLOADILCE + "." + ALANLARILCE[(int)ENUMAlanlarILCE.ILKOD] + "=" + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.ILKOD];
            sql += " AND " + TABLOADILCE + "." + ALANLARILCE[(int)ENUMAlanlarILCE.KOD] + "=" + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.ILCEKOD];
            sql += " AND " + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.KOD] + "=" + TABLOADHARCAMA + "." + ALANLARHARCAMA[(int)ENUMAlanlarHARCAMA.MUHASEBEKOD];
            sql += " AND " + TABLOADHARCAMA + "." + ALANLARHARCAMA[(int)ENUMAlanlarHARCAMA.MUHASEBEKOD] + "=" + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.MUHASEBEKOD];
            sql += " AND " + TABLOADHARCAMA + "." + ALANLARHARCAMA[(int)ENUMAlanlarHARCAMA.KOD] + "=" + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.HARCAMABIRIMKOD];

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.YIL], "=", yil));
            if (!islemTarihIlk.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], ">=", islemTarihIlk));
            if (!islemTarihSon.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], "<=", islemTarihSon));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADTIF);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " GROUP BY ";
            sql += TABLOADIL + "." + ALANLARIL[(int)ENUMAlanlarIL.KOD];
            sql += ", " + TABLOADIL + "." + ALANLARIL[(int)ENUMAlanlarIL.AD];
            sql += ", " + TABLOADILCE + "." + ALANLARILCE[(int)ENUMAlanlarILCE.KOD];
            sql += ", " + TABLOADILCE + "." + ALANLARILCE[(int)ENUMAlanlarILCE.AD];
            sql += ", " + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.KOD];
            sql += ", " + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.AD];
            sql += ", " + TABLOADHARCAMA + "." + ALANLARHARCAMA[(int)ENUMAlanlarHARCAMA.KOD];
            sql += ", " + TABLOADHARCAMA + "." + ALANLARHARCAMA[(int)ENUMAlanlarHARCAMA.AD];

            sql += " ORDER BY ";
            sql += TABLOADIL + "." + ALANLARIL[(int)ENUMAlanlarIL.KOD];
            sql += ", " + TABLOADIL + "." + ALANLARIL[(int)ENUMAlanlarIL.AD];
            sql += ", " + TABLOADILCE + "." + ALANLARILCE[(int)ENUMAlanlarILCE.KOD];
            sql += ", " + TABLOADILCE + "." + ALANLARILCE[(int)ENUMAlanlarILCE.AD];
            sql += ", " + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.KOD];
            sql += ", " + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.AD];
            sql += ", " + TABLOADHARCAMA + "." + ALANLARHARCAMA[(int)ENUMAlanlarHARCAMA.KOD];
            sql += ", " + TABLOADHARCAMA + "." + ALANLARHARCAMA[(int)ENUMAlanlarHARCAMA.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Defter Kriter bilgisi ve Yetki Değerlerini kıstas olarak kullanarak,
        /// kıstasa uygun Dayanıklı Taşınırların bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Defter Kriter bilgisini tutan, DefterKriter türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değerlerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Dayanıklı Taşınır malzeme bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader DayanikliTasinirlarDefteriOlustur(DefterKriter kriter, string[] yetkiDeger)
        {
            string sql = "SELECT ";
            sql += TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ILKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ILAD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ILCEKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ILCEAD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.MUHASEBEKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.MUHASEBEAD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HARCAMABIRIMKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HARCAMABIRIMAD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.AMBARKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.AMBARAD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HESAPPLANKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HESAPPLANAD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.OLCUBIRIMAD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.FISTARIH];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.FISNO];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.NEREDENGELDI];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.NEREYEGITTI];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.KDVLIBIRIMFIYAT];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.SICILNOHAREKETSICILNO];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ISLEMTIPKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ISLEMTIPAD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ISLEMTIPTUR];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.KIMEGITTI];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ZIMMETOZELLIK];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.GONMUHASEBEKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.GONHARCAMABIRIMKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.GONAMBARKOD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.DEGERARTISI];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.PLAKA];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.SASENO];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.MOTORNO];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.SICILNOHAREKETPRSICILNO];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.MARKAAD];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.MODELAD];
            sql += ", TASSICILHAREKETVIEW.GIAI+' '+TASSICILHAREKETVIEW.EKNO AS SERINO";
            sql += " FROM " + TABLOADSICILHAREKETVIEW;

            //ERDAL
            //if (kriter.yil > 0)
            //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.YIL], "=", kriter.yil));

            if (!string.IsNullOrEmpty(kriter.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.MUHASEBEKOD], "=", kriter.muhasebeKod));

            if (!string.IsNullOrEmpty(kriter.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HARCAMABIRIMKOD], "=", kriter.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kriter.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.AMBARKOD], "=", kriter.ambarKod));

            if (!string.IsNullOrEmpty(kriter.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HESAPPLANKOD], "LIKE", kriter.hesapPlanKod.Replace(".", "") + "%"));

            sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HESAPPLANKOD], "NOT LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));//demirbaşlar için
            sql += ")";

            if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, "SICILNOHAREKETSICILNO", "NOT LIKE", "255070201??13%"));

            //MELİHsql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.TIFDURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADSICILHAREKETVIEW);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " ORDER BY ";
            sql += TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HESAPPLANKOD];

            //Sıralamada 2007 sicilleri 2008' den sonra gelmesin diye eklendi
            //***************************************************************
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.SICILNOHAREKETSICILNO];
            //***************************************************************

            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.SICILNOHAREKETPRSICILNO];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.SICILNOHAREKETSIRANO];
            sql += ", " + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ISLEMTIPTUR];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Defter Kriter bilgisi ve Yetki Değerlerini kıstas olarak kullanarak,
        /// kısatasa uygun Tüketim Malzemesi olan Taşınırların bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Defter Kriter bilgisini tutan, DefterKriter türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Tüketim malzemesi olan Taşınır malzeme bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TuketimMalzemeleriDefteriOlustur(DefterKriter kriter, string[] yetkiDeger)
        {
            string sql = "SELECT ";
            sql += TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISTARIH];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISNO];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.NEREYEGITTI];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPIKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPIAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BIRIMFIYATKDVLI];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.NEREDENGELDI];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.GMUHASEBEKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.GHARCAMAKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.GAMBARKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.KIMEGITTI];
            sql += " FROM " + TABLOADSTOKHAREKETVIEW;

            string kosul = string.Empty;

            if (kriter.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", kriter.yil));

            if (!string.IsNullOrEmpty(kriter.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "=", kriter.muhasebeKod));

            if (!string.IsNullOrEmpty(kriter.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "=", kriter.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kriter.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARKOD], "=", kriter.ambarKod));

            if (!string.IsNullOrEmpty(kriter.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", kriter.hesapPlanKod.Replace(".", "") + "%"));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADSTOKHAREKETVIEW);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " ORDER BY ";
            sql += TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD];
            sql += "," + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISNO];
            sql += "," + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.SIRANO];
            //sql += "," + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Defter Kriter bilgisi ve Yetki Değerlerine göre,
        /// Müzede bulunan Taşınırların bilgisini MuzeVeyaKutuphaneDefteriOlustur rutinini çalıştırarak döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Defter Kriter bilgisini tutan, DefterKriter türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Müzede kullanılan Taşınırların bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader MuzeDefteriOlustur(DefterKriter kriter, string[] yetkiDeger)
        {
            return MuzeVeyaKutuphaneDefteriOlustur(kriter, ((int)ENUMTasinirHesapKodu.MUZE).ToString(), yetkiDeger);
        }

        /// <summary>
        /// Parametre olarak verilen Defter Kriter bilgisi ve Yetki Değerlerine göre
        /// Kütüphane bulunan Taşınırların bilgisini MuzeVeyaKutuphaneDefteriOlustur rutinini çalıştırarak döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Defter Kriter bilgisini tutan, DefterKriter türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Müzede kullanılan Taşınırların bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader KutuphaneDefteriOlustur(DefterKriter kriter, string[] yetkiDeger)
        {
            return MuzeVeyaKutuphaneDefteriOlustur(kriter, ((int)ENUMTasinirHesapKodu.KUTUPHANE).ToString(), yetkiDeger);
        }

        /// <summary>
        /// Parametre olarak verilen Defter Kriter bilgisi, Hesap Plan Kodu ve Yetki Değerlerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınırların bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Defter Kriter bilgisini tutan, DefterKriter türünde nesne</param>
        /// <param name="hesapPlanKod">Hesap Plan Kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınırların bilgisini tutan IDataReader türünde nesne döndürür</returns>
        private IDataReader MuzeVeyaKutuphaneDefteriOlustur(DefterKriter kriter, string hesapPlanKod, string[] yetkiDeger)
        {
            string sql = "SELECT ";
            sql += VeriErisim.TumAlanlar(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW);
            sql += " FROM " + TABLOADSICILHAREKETVIEW;

            //ERDAL
            //if (kriter.yil > 0)
            //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.YIL], "=", kriter.yil));

            if (!string.IsNullOrEmpty(kriter.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.MUHASEBEKOD], "=", kriter.muhasebeKod));

            if (!string.IsNullOrEmpty(kriter.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HARCAMABIRIMKOD], "=", kriter.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kriter.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.AMBARKOD], "=", kriter.ambarKod));

            if (!string.IsNullOrEmpty(kriter.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HESAPPLANKOD], "LIKE", kriter.hesapPlanKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(kriter.kitapAd))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ADI], "LIKE", "%" + kriter.kitapAd + "%"));

            if (!string.IsNullOrEmpty(kriter.yazarAd))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.YAZARADI], "LIKE", "%" + kriter.yazarAd + "%"));

            if (!string.IsNullOrEmpty(kriter.yer))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.YERIKONUSU], "LIKE", "%" + kriter.yer + "%"));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSICILHAREKETVIEW, ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HESAPPLANKOD], "LIKE", hesapPlanKod.Replace(".", "") + "%"));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADSICILHAREKETVIEW);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " ORDER BY ";
            sql += TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.HESAPPLANKOD];
            sql += "," + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.SICILNOHAREKETPRSICILNO];
            sql += "," + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.SICILNOHAREKETSIRANO];
            sql += "," + TABLOADSICILHAREKETVIEW + "." + ALANLARSICILHAREKETVIEW[(int)ENUMAlanlarSICILHAREKETVIEW.ISLEMTIPTUR];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır cetveli ile ilgili SQL cümlesine,
        /// From kısmını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, From kısmı eklenmiş Taşınır Cetvel sql cümlesini tutan String türünde nesne döndürür</returns>
        private string CetvelSqlFromKismi(string sql)
        {
            sql += " FROM " + TABLOADSTOKHAREKETVIEW;
            sql += " INNER JOIN " + TABLOADMUHASEBE;
            sql += " ON " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD] + "=" + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.KOD];
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır Cetveli ile ilgili SQL cümlesine,
        /// Koşul kısmını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <param name="cetvel">Taşınır Cetvel bilgisini tutan, Cetvel türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerleri tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Koşul kısmı eklenmiş Taşınır Cetvel sql cümlesini tutan String türünde nesne döndürür</returns>
        private string CetvelSqlKosulKismi(string sql, Cetvel cetvel, string[] yetkiDeger, ENUMCetvelRakamTipi cetvelRakamTipi, string kurumKod)
        {
            string kosul = "(" + VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.ICMALMUAF], "<>", 1);
            kosul = VeriErisim.KosulEkle(kosul, "OR", TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMUHASEBE.ICMALMUAF] + " IS NULL)");

            //Maliye KHK kapsamındakiler cetvellerde gösterilmesin Melih 13.12.2016
            if (kurumKod == "12")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPIKOD], "<>", 26));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPIKOD], "<>", 27));
            }

            if (!string.IsNullOrEmpty(cetvel.muhasebeKod) && cetvel.muhasebeKod != "%")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "=", cetvel.muhasebeKod));

            if (!string.IsNullOrEmpty(cetvel.harcamaKod))
            {
                //Harcama birim koduna ; ayıracı ile birden fazla hb yazılıp cetvelin ona göre alınması için
                cetvel.harcamaKod = cetvel.harcamaKod.Replace(".", "");
                cetvel.harcamaKod = cetvel.harcamaKod.Replace(" ", "");

                if (cetvel.harcamaKod.Contains(";"))
                {
                    string[] hbler = cetvel.harcamaKod.Split(';');
                    string hbKosul = "";
                    for (int i = 0; i < hbler.Length; i++)
                        if (hbler[i] != "")
                            hbKosul = VeriErisim.KosulEkle(hbKosul, "OR", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "=", hbler[i]));

                    if (hbKosul != "")
                        kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + hbKosul + ")");
                }
                else if (cetvel.harcamaKod.Length < 11)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "LIKE", cetvel.harcamaKod + "%"));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "=", cetvel.harcamaKod));
            }

            if (!string.IsNullOrEmpty(cetvel.ambarKod) && cetvel.ambarKod != "%")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARKOD], "=", cetvel.ambarKod));

            if (!string.IsNullOrEmpty(cetvel.ilKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD], "=", cetvel.ilKod));

            if (!string.IsNullOrEmpty(cetvel.ilceKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEKOD], "=", cetvel.ilceKod));

            if (!string.IsNullOrEmpty(cetvel.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", cetvel.hesapPlanKod.Replace(".", "") + "%"));

            if (cetvel.bolgeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BOLGEKOD], "=", cetvel.bolgeKod));

            if (!cetvel.tarih1.isNull)
            {
                if (!(cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS || cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISTARIH], ">=", cetvel.tarih1));
            }
            if (!cetvel.tarih2.isNull)
            {
                if (!(cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS || cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISTARIH], "<=", cetvel.tarih2));
            }

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            kosul += " AND ((" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "<>" + (int)ENUMIslemTipi.DEVIRGIRIS;
            kosul += " AND " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "<>" + (int)ENUMIslemTipi.DAGITIMIADEGIRIS;
            kosul += " AND " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "<>" + (int)ENUMIslemTipi.DEVIRCIKIS;
            kosul += " AND " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "<>" + (int)ENUMIslemTipi.DAGITIMIADECIKIS;
            kosul += ") OR " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD] + "<>" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.GMUHASEBEKOD];
            kosul += " OR " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD] + "<>" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.GHARCAMAKOD];
            kosul += ")";

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADSTOKHAREKETVIEW);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Cetvel Rakam tipine göre,
        /// Verilen Sql cümlesine Geçen Yıldan Devredenleri göstermesi ile ilgili koşulu ekleyen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <param name="cetvelRakamTipi">Cetvel Rakam Tipini tutan, ENUMCetvelRakamTipi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Geçen Yoldan Devretmeyle ilgili koşulu eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        private string GecenYildanDevredenKosulu(int yil, string sql, ENUMCetvelRakamTipi cetvelRakamTipi)
        {
            if (yil > 0)
            {
                if (cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS)
                {
                    //Tüketim için
                    sql = VeriErisim.KosulEkle(sql, " AND ((", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", yil));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "=", (int)ENUMIslemTipi.YILDEVIRGIRIS));
                    sql += ")";

                    //Demirbaş için
                    sql = VeriErisim.KosulEkle(sql, "OR (", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "<", yil));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "NOT LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<", (int)ENUMIslemTipi.TUKETIMCIKIS));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.DEGERARTTIR));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.ENFLASYONARTISI));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.ZFDUSME));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.DTLDUSME));
                    sql += "))";
                }
                else
                {
                    //Demirbaş için
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "<", yil));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "NOT LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], ">=", (int)ENUMIslemTipi.TUKETIMCIKIS));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.ZFVERME));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.DTLVERME));
                }
            }

            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl bilgisine göre
        /// Verilen sql cümlesine Yıl İçinde giren koşulunu ekleyen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Yıl İçinde Giren koşulu eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        private string YilIcindeGirenKosulu(int yil, string sql)
        {
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND ((", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], ">=", (int)ENUMIslemTipi.SATINALMAGIRIS));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<", (int)ENUMIslemTipi.ZFDUSME));
            sql += ")";
            sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "=", (int)ENUMIslemTipi.ACILIS));
            sql += ")";
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl bilgisine göre
        /// Verilen sql cümlesine Yıl İçinde çıkan koşulunu ekleyen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Yıl İçinde Çıkan koşulu eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        private string YilIcindeCikanKosulu(int yil, string sql)
        {
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND ((", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], ">=", (int)ENUMIslemTipi.TUKETIMCIKIS));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.ZFVERME));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.DTLVERME));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.YILDEVIRCIKIS));
            sql = VeriErisim.KosulEkle(sql, ") OR (", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "=", (int)ENUMIslemTipi.DEGERARTTIR));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));
            sql += "))";
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen SQL cümlesine,
        /// Taşınır Sayım Cetveli Alanlarını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Taşınır Sayım Cetveli alanları eklenmiş sql cümlesini döndüren string türünde nesne döndürür</returns>
        private string TasinirSayimCetveliAlanlari(string sql)
        {
            //sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL];
            sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMAD];
            sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD] + ",1, 7)";
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Sql cümlesine
        /// Toplamla ilgili alanları ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Toplam Alanları eklenmiş sql cümlesini tutan string türünde nesne döndürür</returns>
        private string ToplamAlanlari(string sql)
        {
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + "*" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BIRIMFIYATKDVLI] + ")";
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Cetvel, Cetvel Rakam tipi ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Sayım Cetveli bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="cetvel">Cetvel bilgisini tutan,  Cetvel türünde nesne</param>
        /// <param name="cetvelRakamTipi">Rakam tipini tutan, ENUMCetvelRakamtipi türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değerini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır Sayım Cetveli bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TasinirSayimCetveli(Cetvel cetvel, ENUMCetvelRakamTipi cetvelRakamTipi, string[] yetkiDeger, string kurumKod)
        {
            string sql = "SELECT";
            sql = TasinirSayimCetveliAlanlari(sql);
            sql = ToplamAlanlari(sql);
            sql = CetvelSqlFromKismi(sql);
            sql = CetvelSqlKosulKismi(sql, cetvel, yetkiDeger, cetvelRakamTipi, kurumKod);

            if (cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS || cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                sql = GecenYildanDevredenKosulu(cetvel.yil, sql, cetvelRakamTipi);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDEGIREN)
                sql = YilIcindeGirenKosulu(cetvel.yil, sql);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDECIKAN)
                sql = YilIcindeCikanKosulu(cetvel.yil, sql);

            sql += " GROUP BY";
            sql = TasinirSayimCetveliAlanlari(sql);

            sql += " ORDER BY";
            sql = TasinirSayimCetveliAlanlari(sql);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sql cümlesine,
        /// Taşınır Hesap Cetveli alanlarını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Taşınır Hesap Cetveli alanları eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        private string TasinirHesapCetveliAlanlari(string sql)
        {
            //sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL];
            sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEAD];
            sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD] + ",1, 7)";
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Cetvel, Cetvel Rakam tipi ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Hesap Cetveli bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="cetvel">Cetvel bilgisini tutan, Cetvel türünde nesne</param>
        /// <param name="cetvelRakamTipi">Cetvel Rakam Tipini tutan, ENUMCetvelRakamTipi türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değeri bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır Hesap Cetveli bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TasinirHesapCetveli(Cetvel cetvel, ENUMCetvelRakamTipi cetvelRakamTipi, string[] yetkiDeger, string kurumKod)
        {
            string sql = "SELECT";
            sql = TasinirHesapCetveliAlanlari(sql);
            sql = ToplamAlanlari(sql);
            sql = CetvelSqlFromKismi(sql);
            sql = CetvelSqlKosulKismi(sql, cetvel, yetkiDeger, cetvelRakamTipi, kurumKod);

            if (cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS || cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                sql = GecenYildanDevredenKosulu(cetvel.yil, sql, cetvelRakamTipi);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDEGIREN)
                sql = YilIcindeGirenKosulu(cetvel.yil, sql);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDECIKAN)
                sql = YilIcindeCikanKosulu(cetvel.yil, sql);

            sql += " GROUP BY";
            sql = TasinirHesapCetveliAlanlari(sql);

            sql += " ORDER BY";
            sql = TasinirHesapCetveliAlanlari(sql);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen sql cümlesine,
        /// Taşınır Hesap Cetveli Bölge alanlarını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Taşınır Hesap Cetveli Bölge alanları eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        private string TasinirHesapCetveliBolgeAlanlari(string sql)
        {
            //sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL];
            sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BOLGEKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BOLGEAD];
            sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD] + ",1, 7)";
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Cetvel, Cetvel Rakam tipi ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Hesap Cetveli Bölge bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="cetvel">Cetvel bilgisini tutan, Cetvel türünde nesne</param>
        /// <param name="cetvelRakamTipi">Cetvel rakam tipi bilgisini tutan, ENUMCetvelRakamTipi türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kritere uygun Taşınır Hesap Cetveli Bölge bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TasinirHesapCetveliBolge(Cetvel cetvel, ENUMCetvelRakamTipi cetvelRakamTipi, string[] yetkiDeger, string kurumKod)
        {
            string sql = "SELECT";
            sql = TasinirHesapCetveliBolgeAlanlari(sql);
            sql = ToplamAlanlari(sql);
            sql = CetvelSqlFromKismi(sql);
            sql = CetvelSqlKosulKismi(sql, cetvel, yetkiDeger, cetvelRakamTipi, kurumKod);

            //Bölge bilgisi kullanılmayan yerler var
            //Bu yüzden bölge kodu sıfır ise gelmesin
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BOLGEKOD], "<>", 0));

            if (cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS || cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                sql = GecenYildanDevredenKosulu(cetvel.yil, sql, cetvelRakamTipi);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDEGIREN)
                sql = YilIcindeGirenKosulu(cetvel.yil, sql);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDECIKAN)
                sql = YilIcindeCikanKosulu(cetvel.yil, sql);

            sql += " GROUP BY";
            sql = TasinirHesapCetveliBolgeAlanlari(sql);

            sql += " ORDER BY";
            sql = TasinirHesapCetveliBolgeAlanlari(sql);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen sql cümlesine,
        /// Müze Kütüphane Cetvel alanlarını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Müze Kütüphane Cetvel alanları eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        private string MuzeKutuphaneCetveliAlanlari(string sql)
        {
            //sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL];
            sql += " " + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD] + ",1, 7)";
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Cetvel, Cetvel Rakam tipi ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Müze Kütüphane Cetveli bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="cetvel">Cetvel bilgisini tutan, Cetvel türünde nesne</param>
        /// <param name="cetvelRakamTipi">Cetvel rakam tipi bilgisini tutan, ENUMCetvelRakamTipi türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerlerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Müze Kütüphane Cetveli bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader MuzeKutuphaneCetveli(Cetvel cetvel, ENUMCetvelRakamTipi cetvelRakamTipi, string[] yetkiDeger, string kurumKod)
        {
            string sql = "SELECT";
            sql = MuzeKutuphaneCetveliAlanlari(sql);
            sql = ToplamAlanlari(sql);
            sql = CetvelSqlFromKismi(sql);
            sql = CetvelSqlKosulKismi(sql, cetvel, yetkiDeger, cetvelRakamTipi, kurumKod);

            if (cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS || cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                sql = GecenYildanDevredenKosulu(cetvel.yil, sql, cetvelRakamTipi);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDEGIREN)
                sql = YilIcindeGirenKosulu(cetvel.yil, sql);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDECIKAN)
                sql = YilIcindeCikanKosulu(cetvel.yil, sql);

            string kosul = VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", ((int)ENUMTasinirHesapKodu.MUZE).ToString() + "%");
            kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", ((int)ENUMTasinirHesapKodu.KUTUPHANE).ToString() + "%"));
            kosul = "(" + kosul + ")";

            if (sql.Contains("WHERE"))
                sql += " AND " + kosul;
            else
                sql += " WHERE " + kosul;

            sql += " GROUP BY";
            sql = MuzeKutuphaneCetveliAlanlari(sql);

            sql += " ORDER BY";
            sql = MuzeKutuphaneCetveliAlanlari(sql);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen sql cümlesine,
        /// Taşınır Hesap Cetveli İl alanlarını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Taşınır Hesap Cetveli Il Alanları eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        private string TasinirHesapCetveliIlAlanlari(string sql)
        {
            //sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL];
            sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILAD];
            //sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD];
            //sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEAD];

            /*
             * Erdal harcama bazında dönmesin diye
             sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD];
             * Erdal harcama bazında dönmesin diye
             */

            //sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMAD];
            sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD] + ",1, 7)";
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Cetvel, Cetvel Rakam tipi ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Hesap Cetveli İl bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="cetvel">Cetvel bilgisini tutan, Cetvel türünde nesne</param>
        /// <param name="cetvelRakamTipi">Cetvel rakam tipi bilgisini tutan, ENUMCetvelRakamTipi türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerlerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kritere uygun Taşınır Hesap Cetveli İl bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TasinirHesapCetveliIl(Cetvel cetvel, ENUMCetvelRakamTipi cetvelRakamTipi, string[] yetkiDeger, string kurumKod)
        {
            string sql = "SELECT";
            sql = TasinirHesapCetveliIlAlanlari(sql);
            sql = ToplamAlanlari(sql);
            sql = CetvelSqlFromKismi(sql);
            sql = CetvelSqlKosulKismi(sql, cetvel, yetkiDeger, cetvelRakamTipi, kurumKod);

            if (cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS || cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                sql = GecenYildanDevredenKosulu(cetvel.yil, sql, cetvelRakamTipi);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDEGIREN)
                sql = YilIcindeGirenKosulu(cetvel.yil, sql);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDECIKAN)
                sql = YilIcindeCikanKosulu(cetvel.yil, sql);

            sql += " GROUP BY";
            sql = TasinirHesapCetveliIlAlanlari(sql);

            sql += " ORDER BY";
            sql = TasinirHesapCetveliIlAlanlari(sql);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Cetvel, Cetvel Rakam tipi ve Yetki değerine göre,
        /// Taşınır Kesin Hesap Cetveli bilgisini TasinirKesinHesapCetveliVEIcmal rutinini çalıştırarak döndüren fonksiyon
        /// </summary>
        /// <param name="cetvel">Cetvel bilgisini tutan, Cetvel türünde nesne</param>
        /// <param name="cetvelRakamTipi">Cetvel Rakam Tipi bilgisini tutan, ENUMCetvelRakamTipi türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Taşınır Kesin Hesap Cetveli bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TasinirKesinHesapCetveli(Cetvel cetvel, ENUMCetvelRakamTipi cetvelRakamTipi, string[] yetkiDeger, bool muhasebeEkle, string kurumKod)
        {
            return TasinirKesinHesapCetveliVEIcmal(cetvel, cetvelRakamTipi, 7, yetkiDeger, muhasebeEkle, kurumKod, false);
        }

        /// <summary>
        /// Parametre olarak verilen Cetvel, Cetvel Rakam tipi ve Yetki değerine göre,
        /// Taşınır Kesin Hesap İcmal Cetveli bilgisini TasinirKesinHesapCetveliVEIcmal rutinini çalıştırarak döndüren fonksiyon
        /// </summary>
        /// <param name="cetvel">Cetvel bilgisini tutan, Cetvel türünde nesne</param>
        /// <param name="cetvelRakamTipi">Cetvel Rakam Tipi bilgisini tutan, ENUMCetvelRakamTipi türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Taşınır Kesin Hesap İcmal Cetveli bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TasinirKesinHesapIcmalCetveli(Cetvel cetvel, ENUMCetvelRakamTipi cetvelRakamTipi, string[] yetkiDeger, bool muhasebeEkle, string kurumKod, bool icmal)
        {
            int hesapKodUzunluk = 5;
            if (cetvel.seviye == 1) hesapKodUzunluk = 3;
            else if (cetvel.seviye == 2) hesapKodUzunluk = 5;
            else if (cetvel.seviye == 3) hesapKodUzunluk = 7;

            return TasinirKesinHesapCetveliVEIcmal(cetvel, cetvelRakamTipi, hesapKodUzunluk, yetkiDeger, muhasebeEkle, kurumKod, icmal);
        }

        /// <summary>
        /// Parametre olarak verilen Sql cümlesine, Hesap Kod uzunluk alanına göre,
        /// Taşınır Kesin Hesap Cetveli ve İcmal Alanlarını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini oluşturan, String türünde nesne</param>
        /// <param name="hesapKodUzunluk">Hesap Kodu Uzunluk bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Taşınır Kesin Hesap Cetveli ve İcmal Alanlarını eklenmiş Sql cümlesini tutan String türünde nesne döndürür</returns>
        private string TasinirKesinHesapCetveliVEIcmalAlanlari(string sql, int hesapKodUzunluk, bool muhasebeEkle, bool icmal)
        {
            //sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL];
            sql += " " + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD] + ",1, " + hesapKodUzunluk.ToString() + ")";
            if (hesapKodUzunluk == 7 && !icmal)
            {
                sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
                sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            }
            if (muhasebeEkle)
            {
                sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD];
                sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEAD];
            }

            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Cetvel, Cetvel Rakam tipi, Hesap kodu uzunluk  ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Kesin Hesap Cetveli ve İcmal bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="cetvel">Cetvel bilgisini tutan, Cetvel türünde nesne</param>
        /// <param name="cetvelRakamTipi">Cetvel Rakam Tipi bilgisini tutan, ENUMCetvelRakamTipi türünde nesne</param>
        /// <param name="hesapKodUzunluk">Hesap Kodu Uzunluk bilgisini tutan, Int türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerini tutan, String dizi türünde nesne </param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır Kesin Hesap Cetveli ve İcmal bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TasinirKesinHesapCetveliVEIcmal(Cetvel cetvel, ENUMCetvelRakamTipi cetvelRakamTipi, int hesapKodUzunluk, string[] yetkiDeger, bool muhasebeEkle, string kurumKod, bool icmal)
        {
            string sql = "SELECT";
            sql = TasinirKesinHesapCetveliVEIcmalAlanlari(sql, hesapKodUzunluk, muhasebeEkle, icmal);
            sql = ToplamAlanlari(sql);
            sql = CetvelSqlFromKismi(sql);
            sql = CetvelSqlKosulKismi(sql, cetvel, yetkiDeger, cetvelRakamTipi, kurumKod);

            if (cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENGIRIS || cetvelRakamTipi == ENUMCetvelRakamTipi.GECENYILDANDEVREDENCIKIS)
                sql = GecenYildanDevredenKosulu(cetvel.yil, sql, cetvelRakamTipi);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDEGIREN)
                sql = YilIcindeGirenKosulu(cetvel.yil, sql);
            else if (cetvelRakamTipi == ENUMCetvelRakamTipi.YILICINDECIKAN)
                sql = YilIcindeCikanKosulu(cetvel.yil, sql);

            sql += " GROUP BY";
            sql = TasinirKesinHesapCetveliVEIcmalAlanlari(sql, hesapKodUzunluk, muhasebeEkle, icmal);

            sql += " ORDER BY";
            sql = TasinirKesinHesapCetveliVEIcmalAlanlari(sql, hesapKodUzunluk, muhasebeEkle, icmal);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Depo Durumu, Depo Durum İşlemi ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Depo Durumu bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="dp">Depo Durum bilgisini tutan, DepoDurum türünde nesne</param>
        /// <param name="depoDurumIslem">Depo Durum İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değeri bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır Depo Durum bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TasinirDepoDurumu(DepoDurum dp, string depoDurumIslem, string[] yetkiDeger)
        {
            string tablo = string.Empty;
            if (depoDurumIslem == "giren" || depoDurumIslem == "cikan")
                tablo = TABLOADSTOKHAREKETVIEW;
            else if (depoDurumIslem.StartsWith("zimmet"))
                tablo = "TASAMBARZIMMETVIEW";

            string sql = "SELECT";
            sql = TasinirDepoDurumuAlanlari(sql, tablo, dp.raporTur, dp.hesapPlanKod.Replace(".", "").Length);

            if (depoDurumIslem == "giren" || depoDurumIslem == "cikan")
            {
                sql += ", " + VeriErisim.IslecBul("SUM") + "(" + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + ")";
                sql += ", " + VeriErisim.IslecBul("SUM") + "(" + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + "*" + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BIRIMFIYATKDVLI] + ")";
            }
            else if (depoDurumIslem.StartsWith("zimmet"))
            {
                sql += ", " + VeriErisim.IslecBul("COUNT") + "(*)";
                sql += ", " + VeriErisim.IslecBul("SUM") + "(" + tablo + "." + "KDVLIBIRIMFIYAT" + ")";
            }
            sql += " FROM " + tablo;

            string kosul = string.Empty;
            if (dp.yilIci && dp.yil > 0)
                kosul = VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", dp.yil);
            else if (dp.yil > 0)
            {
                kosul = "((";
                kosul += VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", dp.yil);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));
                kosul = VeriErisim.KosulEkle(kosul, ") OR (", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "<=", dp.yil));
                kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "NOT LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));//Demirbaşlar için
                kosul += ")))";
            }

            if (!string.IsNullOrEmpty(dp.muhasebeKod))
            {
                if (dp.raporTur == (int)ENUMDepoDurumRaporTur.IL)
                {
                    if (dp.muhasebeKod.Length == 5)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "=", dp.muhasebeKod));
                    else
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "LIKE", dp.muhasebeKod + "%"));
                }
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "=", dp.muhasebeKod));
            }

            if (!string.IsNullOrEmpty(dp.harcamaKod))
            {
                if (dp.harcamaKod.Replace(".", "").Length == 11)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "=", dp.harcamaKod.Replace(".", "")));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "LIKE", dp.harcamaKod.Replace(".", "") + "%"));
            }

            if (!string.IsNullOrEmpty(dp.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARKOD], "=", dp.ambarKod));

            if (!string.IsNullOrEmpty(dp.hesapPlanKod))
            {
                string[] hp = dp.hesapPlanKod.Split(';');
                string hesapKosul = "";
                for (int i = 0; i < hp.Length; i++)
                    hesapKosul = VeriErisim.KosulEkle(hesapKosul, "OR", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", hp[i].Replace(".", "") + "%"));

                if (hesapKosul != "")
                    kosul += " AND (" + hesapKosul + ")";
            }
            if (!string.IsNullOrEmpty(dp.ilKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD], "=", dp.ilKod));

            if (!string.IsNullOrEmpty(dp.ilceKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEKOD], "=", dp.ilceKod));

            if (depoDurumIslem == "giren" || depoDurumIslem == "cikan")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            if (depoDurumIslem == "giren")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], ">=", (int)ENUMIslemTipi.SATINALMAGIRIS));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<", (int)ENUMIslemTipi.TUKETIMCIKIS));

                if (dp.khkDurumu == 1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPIKOD], "<>", "26"));
                else if (dp.khkDurumu == 2)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPIKOD], "=", "26"));

                //Erdal "TASSTOKHAREKETVIEW" tablosunda zimmetler yok
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.ZFDUSME));
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.DTLDUSME));

                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.YILDEVIRGIRIS));
            }
            else if (depoDurumIslem == "cikan")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND ((", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], ">=", (int)ENUMIslemTipi.TUKETIMCIKIS));

                //Erdal "TASSTOKHAREKETVIEW" tablosunda zimmetler yok
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.ZFVERME));
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.DTLVERME));

                if (!dp.yilDevri)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.YILDEVIRCIKIS));

                kosul += ")";
                kosul = VeriErisim.KosulEkle(kosul, "OR (", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "=", (int)ENUMIslemTipi.DEGERARTTIR));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", (int)ENUMTasinirHesapKodu.TUKETIM + "%"));
                kosul += "))";

                if (dp.khkDurumu == 1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPIKOD], "<>", "27"));
                else if (dp.khkDurumu == 2)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPIKOD], "=", "27"));
            }
            else if (depoDurumIslem == "zimmetKisi")
                kosul = VeriErisim.KosulEkle(kosul, "AND ", VeriErisim.KosulOl(tablo, "ISLEMTURUKOD", "=", (int)ENUMIslemTipi.ZFVERME));
            else if (depoDurumIslem == "zimmetOrtak")
                kosul = VeriErisim.KosulEkle(kosul, "AND ", VeriErisim.KosulOl(tablo, "ISLEMTURUKOD", "=", (int)ENUMIslemTipi.DTLVERME));

            //if (dp.raporTur != (int)ENUMDepoDurumRaporTur.AMBAR && (depoDurumIslem == "giren" || depoDurumIslem == "cikan"))
            if (dp.ambarKod == "" && (depoDurumIslem == "giren" || depoDurumIslem == "cikan"))
            {
                kosul += " AND ((" + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "<>" + (int)ENUMIslemTipi.DEVIRGIRIS;
                kosul += " AND " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "<>" + (int)ENUMIslemTipi.DAGITIMIADEGIRIS;
                kosul += " AND " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "<>" + (int)ENUMIslemTipi.DEVIRCIKIS;
                kosul += " AND " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "<>" + (int)ENUMIslemTipi.DAGITIMIADECIKIS;
                kosul += ") OR " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD] + "<>" + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.GMUHASEBEKOD];
                kosul += " OR " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD] + "<>" + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.GHARCAMAKOD];
                kosul += ")";
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, tablo);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " GROUP BY";
            sql = TasinirDepoDurumuAlanlari(sql, tablo, dp.raporTur, dp.hesapPlanKod.Replace(".", "").Length);

            //melih performans için bazı yerlerde timeout hatası alıyordu. Oracleda gereksinim duyulabilir.
            //sql += " ORDER BY";
            //sql = TasinirDepoDurumuAlanlari(sql, tablo, dp.raporTur, dp.hesapPlanKod.Replace(".", "").Length);

            if (dp.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
                sql += " ORDER BY " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD];
            else if (dp.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                sql += " ORDER BY " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD];
            else if (dp.raporTur == (int)ENUMDepoDurumRaporTur.IL)
                sql += " ORDER BY " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD];
            else if (dp.raporTur == (int)ENUMDepoDurumRaporTur.AMBAR || dp.raporTur == (int)ENUMDepoDurumRaporTur.KURUM)
                sql += " ORDER BY " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen sql cümlesine,
        /// Taşınır Depo Durum Alanlarını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <param name="tablo">Tablo adını tutan, String türünde nesne</param>
        /// <param name="raporTur">Rapor Türü bilgisini tutan, Int türünde nesne</param>
        /// <param name="hesapKodUzunluk">Hesap Kodu Uzunluk bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Taşınır Depo Durum alanları eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        private string TasinirDepoDurumuAlanlari(string sql, string tablo, int raporTur, int hesapKodUzunluk)
        {
            sql += " " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL];
            if (raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA || raporTur == (int)ENUMDepoDurumRaporTur.IL)
            {
                sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(" + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD] + ", 1, " + hesapKodUzunluk.ToString() + ")";
                //sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANAD];
                sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
                sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];

                if (raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
                {
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEAD];
                }
                else if (raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                {
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD];
                    //sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMAD];
                }
                else if (raporTur == (int)ENUMDepoDurumRaporTur.IL)
                {
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILAD];
                }
            }
            else if (raporTur == (int)ENUMDepoDurumRaporTur.AMBAR || raporTur == (int)ENUMDepoDurumRaporTur.KURUM)
            {
                if (raporTur == (int)ENUMDepoDurumRaporTur.AMBAR)
                {
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILAD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEKOD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEAD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEAD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMAD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARKOD];
                    sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARAD];
                }

                sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD];
                sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANAD];
                sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
                sql += ", " + tablo + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            }
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Tüketim Çıkış ve Yetki Değerini kıstas olarak kullanarak,
        /// kıstasa uygun Tüketim Çıkış bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tk">Tüketim Çıkış bilgisini tutan, TuketimCikis türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="fark">Farka göre hesaplama yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Tüketim Çıkış bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TuketimCikis(TuketimCikis tk, string[] yetkiDeger, bool fark)
        {
            string sql = "SELECT";
            sql = TuketimCikisAlanlari(sql);
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + "*" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BIRIMFIYATKDVLI] + ")";
            sql += " FROM " + TABLOADSTOKHAREKETVIEW;

            string kosul = string.Empty;

            if (tk.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", tk.yil));

            if (!string.IsNullOrEmpty(tk.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "=", tk.muhasebeKod));

            if (!string.IsNullOrEmpty(tk.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "=", tk.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(tk.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARKOD], "=", tk.ambarKod));

            if (!string.IsNullOrEmpty(tk.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", tk.hesapPlanKod.Replace(".", "") + "%"));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "=", (int)ENUMIslemTipi.TUKETIMCIKIS));

            if (!tk.tarih1.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISTARIH], ">=", tk.tarih1));

            if (!tk.tarih2.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISTARIH], "<=", tk.tarih2));

            //Erdal
            //kosul = VeriErisim.KosulEkle(kosul, "AND", "((" + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "=" + (int)ENUMIslemTipi.DEVIRCIKIS + " AND " + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD] + "<>" + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.GMUHASEBEKOD] + ") OR " + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "<>" + (int)ENUMIslemTipi.DEVIRCIKIS + ")");
            //kosul = VeriErisim.KosulEkle(kosul, "AND", "((" + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "=" + (int)ENUMIslemTipi.DAGITIMIADECIKIS + " AND " + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD] + "<>" + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.GMUHASEBEKOD] + ") OR " + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR] + "<>" + (int)ENUMIslemTipi.DAGITIMIADECIKIS + ")");

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADSTOKHAREKETVIEW);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " GROUP BY";
            sql = TuketimCikisAlanlari(sql);

            if (fark)
            {
                sql += " HAVING " + VeriErisim.IslecBul("ROUND");
                sql += "(" + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + " * " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BIRIMFIYATKDVLI] + "), 2)";
                sql += " - " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + " * " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BIRIMFIYATKDVLI] + ") <> 0";
            }

            sql += " ORDER BY";
            sql = TuketimCikisAlanlari(sql);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen sql cümlesine,
        /// Tüketim Çıkış alanlarını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Tüketim Çıkış alanları eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        private string TuketimCikisAlanlari(string sql)
        {
            sql += " " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL];
            sql += "," + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMAD];
            //sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARKOD];
            //sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARAD];
            sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD] + ",1, 7)";
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Zimmet Ortak Alan ve Yetki Değerini kıstas olarak kullanarak,
        /// kıstasa uygun Zimmet Ortak Alan bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Zimmet Ortak Alan bilgisini tutan, ZimmetOrtakAlanVeKisi türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Zimmet Ortak Alan bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader ZimmetOrtakAlanVeKisi(ZimmetOrtakAlanVeKisi kriter, string[] yetkiDeger)
        {
            string sql = "SELECT ";
            sql += "TASAMBARZIMMETVIEW.YIL";
            sql += ", TASAMBARZIMMETVIEW.MUHASEBEKOD";
            sql += ", TASAMBARZIMMETVIEW.MUHASEBEAD";
            sql += ", TASAMBARZIMMETVIEW.HARCAMABIRIMKOD";
            sql += ", TASAMBARZIMMETVIEW.HARCAMABIRIMAD";
            sql += ", TASAMBARZIMMETVIEW.NEREYEGITTI";
            sql += ", TASAMBARZIMMETVIEW.ODAAD";
            sql += ", TASAMBARZIMMETVIEW.PRSICILNO";
            sql += ", TASAMBARZIMMETVIEW.SICILNO";
            sql += ", TASAMBARZIMMETVIEW.HESAPPLANAD";
            sql += ", TASAMBARZIMMETVIEW.FISNO";
            sql += ", TASAMBARZIMMETVIEW.FISTARIH";
            sql += ", TASAMBARZIMMETVIEW.KIMEGITTI";
            sql += ", TASAMBARZIMMETVIEW.KISIAD";
            sql += ", TASAMBARZIMMETVIEW.ZIMMETOZELLIK";
            sql += ", TASAMBARZIMMETVIEW.AMBARKOD";
            sql += ", TASAMBARZIMMETVIEW.AMBARAD";
            sql += ", TASAMBARZIMMETVIEW.SASENO";
            sql += " FROM " + "TASAMBARZIMMETVIEW";

            string kosul = string.Empty;
            //if (kriter.yil > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "YIL", "=", kriter.yil));

            if (!string.IsNullOrEmpty(kriter.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "MUHASEBEKOD", "=", kriter.muhasebeKod));

            /*Erdal
            //if (!string.IsNullOrEmpty(kriter.harcamaKod))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "HARCAMABIRIMKOD", "=", kriter.harcamaKod.Replace(".", "")));

            //if (!string.IsNullOrEmpty(kriter.odaKod))
            //{
            //    string odaKosul = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "NEREYEGITTI", "=", kriter.odaKod));
            //    if (kriter.kisiDahilEt > 0)
            //    {
            //        odaKosul = VeriErisim.KosulEkle(odaKosul, "OR", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "ODA", "=", kriter.odaKod));
            //    }
            //    odaKosul = "(" + odaKosul + ")";
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", odaKosul);
            //}

            //if (!string.IsNullOrEmpty(kriter.tcKimlikNo))
            //{
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "KIMEGITTI", "=", kriter.tcKimlikNo));
            //    //melih kişi üstünde ortak alan zimmet varsa gelsin diye
            //    //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "NEREYEGITTI", "=", ""));
            //}
            */

            //Erdal oda + kişi zimmetleri gelsin diye değişti
            if (!string.IsNullOrEmpty(kriter.odaKod))
            {
                string odaKosul = "(" + VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "NEREYEGITTI", "=", kriter.odaKod);
                if (!string.IsNullOrEmpty(kriter.harcamaKod))
                    odaKosul = VeriErisim.KosulEkle(odaKosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "HARCAMABIRIMKOD", "=", kriter.harcamaKod.Replace(".", "")));
                odaKosul += ")";
                if (kriter.kisiDahilEt > 0)
                {
                    string kisiKosul = "TASAMBARZIMMETVIEW.KIMEGITTI IN (SELECT KOD FROM TASPERSONEL";
                    kisiKosul = VeriErisim.KosulEkle(kisiKosul, "WHERE", VeriErisim.KosulOl("TASPERSONEL", "MUHASEBEKOD", "=", kriter.muhasebeKod));
                    kisiKosul = VeriErisim.KosulEkle(kisiKosul, "AND", VeriErisim.KosulOl("TASPERSONEL", "HARCAMABIRIMKOD", "=", kriter.harcamaKod.Replace(".", "")));
                    kisiKosul = VeriErisim.KosulEkle(kisiKosul, "AND", VeriErisim.KosulOl("TASPERSONEL", "ODA", "=", kriter.odaKod));
                    kisiKosul += ")";
                    odaKosul = VeriErisim.KosulEkle(odaKosul, "OR", kisiKosul);
                }
                odaKosul = "(" + odaKosul + ")";
                kosul = VeriErisim.KosulEkle(kosul, "AND", odaKosul);
            }

            if (!string.IsNullOrEmpty(kriter.tcKimlikNo))
            {
                if (!string.IsNullOrEmpty(kriter.harcamaKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "HARCAMABIRIMKOD", "=", kriter.harcamaKod.Replace(".", "")));

                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "KIMEGITTI", "=", kriter.tcKimlikNo));
                //melih kişi üstünde ortak alan zimmet varsa gelsin diye
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "NEREYEGITTI", "=", ""));
            }

            if (!string.IsNullOrEmpty(kriter.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "HESAPPLANKOD", "=", kriter.hesapPlanKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kriter.sicilNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "SICILNO", "=", kriter.sicilNo));

            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, "TASAMBARZIMMETVIEW");
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " ORDER BY ";
            sql += "TASAMBARZIMMETVIEW.HARCAMABIRIMKOD";
            sql += ", TASAMBARZIMMETVIEW.AMBARKOD";
            sql += ", TASAMBARZIMMETVIEW.SICILNO";
            sql += ", TASAMBARZIMMETVIEW.FISNO";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Formu, Taşınır Form Kriteri ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Malzeme Tarihçe bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="tfKriter">Taşınır Form Kriter bilgisini tutan, TasinirFormKriter türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Malzeme Tarihçe bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TasinirIslemMalzemeTarihceRaporu(TasinirIslemForm tf, TasinirFormKriter tfKriter, string[] yetkiDeger)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTIF, ALANLARTIF);
            sql += ", " + VeriErisim.TumAlanlar(TABLOADTIFDETAY, ALANLARTIFDETAY) + ", TASHESAPPLAN.ACIKLAMA, TASISLEMTIP.TUR, TASISLEMTIP.AD, TASPERSONEL.AD";
            sql += ", TASISLEMFORM.ACIKLAMA";
            sql += " FROM " + TABLOADTIF;// + ", " + TABLOADTIFDETAY + ", TASHESAPPLAN, TASISLEMTIP";
                                         //sql += " LEFT OUTER JOIN TASPERSONEL ON TASPERSONEL.KOD=TASISLEMFORM.KIMEGITTI";
                                         //sql += " WHERE " + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.YIL] + "=" + TABLOADTIFDETAY + "." + ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.YIL];
                                         //sql += " AND " + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.MUHASEBEKOD] + "=" + TABLOADTIFDETAY + "." + ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.MUHASEBEKOD];
                                         //sql += " AND " + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.HARCAMABIRIMKOD] + "=" + TABLOADTIFDETAY + "." + ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.HARCAMABIRIMKOD];
                                         //sql += " AND " + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.FISNO] + "=" + TABLOADTIFDETAY + "." + ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.FISNO];
                                         //sql += " AND TASHESAPPLAN.KOD=TASISLEMDETAY.HESAPPLANKOD";
                                         //sql += " AND TASISLEMTIP.KOD=TASISLEMFORM.ISLEMTIPIKOD";
            sql += " LEFT OUTER JOIN TASISLEMDETAY ON TASISLEMFORM.YIL = TASISLEMDETAY.YIL AND TASISLEMFORM.MUHASEBEKOD = TASISLEMDETAY.MUHASEBEKOD AND TASISLEMFORM.HARCAMABIRIMKOD = TASISLEMDETAY.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO = TASISLEMDETAY.FISNO";
            sql += " LEFT OUTER JOIN TASHESAPPLAN ON TASHESAPPLAN.KOD = TASISLEMDETAY.HESAPPLANKOD";
            sql += " LEFT OUTER JOIN TASISLEMTIP ON TASISLEMTIP.KOD = TASISLEMFORM.ISLEMTIPIKOD";
            sql += " LEFT OUTER JOIN TASPERSONEL ON TASPERSONEL.KOD = TASISLEMFORM.KIMEGITTI";
            sql += " WHERE TASISLEMFORM.YIL>0";

            if (tf.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.YIL], "=", tf.yil));

            if (!string.IsNullOrEmpty(tf.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.MUHASEBEKOD], "LIKE", tf.muhasebeKod));

            if (!string.IsNullOrEmpty(tf.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.HARCAMABIRIMKOD], "LIKE", tf.harcamaKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tf.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.AMBARKOD], "=", tf.ambarKod));

            if (!tfKriter.belgeTarihBasla.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], ">=", tfKriter.belgeTarihBasla));

            if (!tfKriter.belgeTarihBit.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], "<=", tfKriter.belgeTarihBit));

            if (tf.islemTipKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.ISLEMTIPKOD], "=", tf.islemTipKod));
            if (tfKriter.islemTipleri != "")
            {
                string[] islemTipleri = tfKriter.islemTipleri.Split(';');
                string islemTipKosul = "";
                for (int i = 0; i < islemTipleri.Length; i++)
                    islemTipKosul = VeriErisim.KosulEkle(islemTipKosul, "OR", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.ISLEMTIPKOD], "=", OrtakFonksiyonlar.ConvertToInt(islemTipleri[i], 0)));

                if (islemTipKosul != "")
                    sql += " AND (" + islemTipKosul + ")";
            }

            if (!string.IsNullOrEmpty(tf.gMuhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GMUHASEBEKOD], "=", tf.gMuhasebeKod));

            if (!string.IsNullOrEmpty(tf.gHarcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GHARCAMAKOD], "=", tf.gHarcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(tf.gAmbarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GAMBARKOD], "=", tf.gAmbarKod));

            if (!string.IsNullOrEmpty(tf.neredenGeldi))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.NEREDENGELDI], "=", tf.neredenGeldi));

            if (!string.IsNullOrEmpty(tf.nereyeGitti))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.NEREYEGITTI], "=", tf.nereyeGitti));

            if (!string.IsNullOrEmpty(tf.kimeGitti))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.KIMEGITTI], "=", tf.kimeGitti));

            if (!string.IsNullOrEmpty(tfKriter.hesapKodu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIFDETAY, ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.HESAPPLANKOD], "LIKE", tfKriter.hesapKodu.Replace(".", "") + "%"));

            if (tf.durum > 0)
            {
                if (tf.durum == 9)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.DURUM], "=", (int)ENUMBelgeDurumu.IPTAL));
                else if (tf.durum < 5)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.DURUM], "<", (int)ENUMBelgeDurumu.ONAYLI));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.DURUM], "<", (int)ENUMBelgeDurumu.IPTAL));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADTIF);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " ORDER BY " + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.YIL];
            sql += ", " + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.MUHASEBEKOD];
            sql += ", " + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.HARCAMABIRIMKOD];
            sql += ", " + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.AMBARKOD];
            sql += ", " + TABLOADTIF + "." + ALANLARTIF[(int)ENUMAlanlarTIF.FISNO];

            if (tf.adaNo == "SIRANO")
                sql += ", " + TABLOADTIFDETAY + "." + ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.SIRANO];
            else
                sql += ", " + TABLOADTIFDETAY + "." + ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.HESAPPLANKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil No Hareket ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Sicil Raporu bilgisini dönen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Sicil No Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Sicil Raporu bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader SicilRaporu(SicilNoHareket kriter, string[] yetkiDeger)
        {
            string sql = "SELECT *,TASHESAPPLAN.AMORTIYIL FROM ";
            sql += "(SELECT ";
            sql += "TASAMBARSICILVIEW.PRSICILNO";
            sql += ", TASAMBARSICILVIEW.SICILNO";
            sql += ", TASAMBARSICILVIEW.HESAPPLANAD";
            sql += ", TASAMBARSICILVIEW.KDVORAN";
            sql += ", TASAMBARSICILVIEW.KDVLIBIRIMFIYAT";
            sql += ", TASAMBARSICILVIEW.FIYAT";
            sql += ", TASAMBARSICILVIEW.ISLEMTIPIKOD";
            sql += ", TASAMBARSICILVIEW.ISLEMTURUKOD";
            sql += ", TASAMBARSICILVIEW.ISLEMTARIH";
            sql += ", TASAMBARSICILVIEW.MUHASEBEKOD";
            sql += ", TASAMBARSICILVIEW.MUHASEBEAD";
            sql += ", TASAMBARSICILVIEW.HARCAMABIRIMKOD";
            sql += ", TASAMBARSICILVIEW.HARCAMABIRIMAD";
            sql += ", TASAMBARSICILVIEW.AMBARKOD";
            sql += ", TASAMBARSICILVIEW.AMBARAD";
            sql += ", TASAMBARSICILVIEW.KIMEGITTI";
            sql += ", '' KISIAD";//UNION için gerekli
            sql += ", TASAMBARSICILVIEW.NEREYEGITTI";
            sql += ", '' ODAAD";//UNION için gerekli
            sql += ", TASAMBARSICILVIEW.ZIMMETOZELLIK";
            sql += ", TASAMBARSICILVIEW.MARKAAD";
            sql += ", TASAMBARSICILVIEW.MODELAD";
            sql += ", TASAMBARSICILVIEW.SASENO";
            sql += ", TASAMBARSICILVIEW.MOTORNO";
            sql += ", TASAMBARSICILVIEW.DISSICILNO";
            sql += ", TASAMBARSICILVIEW.HESAPPLANKOD";
            sql += ", TASAMBARSICILVIEW.PLAKA";
            sql += ", TASAMBARSICILVIEW.YERIKONUSU";
            sql += " FROM " + "TASAMBARSICILVIEW";

            string kosul = string.Empty;

            if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SICILNO", "NOT LIKE", "255070201??13%"));
            else
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SICILNO", "<>", ""));

            if (!string.IsNullOrEmpty(kriter.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD", "=", kriter.muhasebeKod));

            if (!string.IsNullOrEmpty(kriter.harcamaBirimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "HARCAMABIRIMKOD", "=", kriter.harcamaBirimKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kriter.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "AMBARKOD", "=", kriter.ambarKod));

            if (!string.IsNullOrEmpty(kriter.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "HESAPPLANKOD", "LIKE", kriter.hesapPlanKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(kriter.kimeGitti))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "KIMEGITTI", "=", kriter.kimeGitti));

            if (!string.IsNullOrEmpty(kriter.nereyeGitti))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "NEREYEGITTI", "=", kriter.nereyeGitti));

            //İşlem tarihine bakıyorum çünkü onaylanma tarihi önemli sicil açısından
            //Küçük tarih kriterini fisTarih değişkeni ile gönderiyorum istemciden
            //Çünkü class'ı değiştirmek istemedim.
            if (!kriter.fisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "ISLEMTARIH", ">=", kriter.fisTarih));

            //Büyük tarih kriterini islemTarih değişkeni ile gönderiyorum istemciden
            //Çünkü class'ı değiştirmek istemedim.
            if (!kriter.islemTarih.isNull)
            {
                kriter.islemTarih.AddDays(1);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "ISLEMTARIH", "<=", kriter.islemTarih));
            }

            if (!string.IsNullOrEmpty(kriter.fisNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "FISNO", "=", kriter.fisNo));

            if (!string.IsNullOrEmpty(kriter.sicilNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SICILNO", "=", kriter.sicilNo));

            if (kriter.fiyat > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "FIYAT", ">=", kriter.fiyat));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, "");
            if (orluYetkiler != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", orluYetkiler);

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " UNION ";
            sql += "SELECT ";
            sql += "TASAMBARZIMMETVIEW.PRSICILNO";
            sql += ", TASAMBARZIMMETVIEW.SICILNO";
            sql += ", TASAMBARZIMMETVIEW.HESAPPLANAD";
            sql += ", TASAMBARZIMMETVIEW.KDVORAN";
            sql += ", TASAMBARZIMMETVIEW.KDVLIBIRIMFIYAT";
            sql += ", TASAMBARZIMMETVIEW.FIYAT";
            sql += ", TASAMBARZIMMETVIEW.ISLEMTIPIKOD";
            sql += ", TASAMBARZIMMETVIEW.ISLEMTURUKOD";
            sql += ", TASAMBARZIMMETVIEW.ISLEMTARIH";
            sql += ", TASAMBARZIMMETVIEW.MUHASEBEKOD";
            sql += ", TASAMBARZIMMETVIEW.MUHASEBEAD";
            sql += ", TASAMBARZIMMETVIEW.HARCAMABIRIMKOD";
            sql += ", TASAMBARZIMMETVIEW.HARCAMABIRIMAD";
            sql += ", TASAMBARZIMMETVIEW.AMBARKOD";
            sql += ", TASAMBARZIMMETVIEW.AMBARAD";
            sql += ", TASAMBARZIMMETVIEW.KIMEGITTI";
            sql += ", TASAMBARZIMMETVIEW.KISIAD";
            sql += ", TASAMBARZIMMETVIEW.NEREYEGITTI";
            sql += ", TASAMBARZIMMETVIEW.ODAAD";
            sql += ", TASAMBARZIMMETVIEW.ZIMMETOZELLIK";
            sql += ", TASAMBARZIMMETVIEW.MARKAAD";
            sql += ", TASAMBARZIMMETVIEW.MODELAD";
            sql += ", TASAMBARZIMMETVIEW.SASENO";
            sql += ", TASAMBARZIMMETVIEW.MOTORNO";
            sql += ", TASAMBARZIMMETVIEW.DISSICILNO";
            sql += ", TASAMBARZIMMETVIEW.HESAPPLANKOD";
            sql += ", TASAMBARZIMMETVIEW.PLAKA";
            sql += ", TASAMBARZIMMETVIEW.YERIKONUSU";
            sql += " FROM " + "TASAMBARZIMMETVIEW";

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += ") A, TASSICILNOHAREKET B LEFT OUTER JOIN TASHESAPPLAN ON B.HESAPPLANKOD=TASHESAPPLAN.KOD ";

            sql += "WHERE A.PRSICILNO=B.PRSICILNO ";
            sql += "AND B.SIRANO=1 ";

            if (kriter.islemTipKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "ISLEMTIPIKOD", "=", kriter.islemTipKod));
            else if (kriter.islemTipKod < 0)
            {
                if (kriter.islemTipKod == -1)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "ISLEMTIPIKOD", "<", 50));
                if (kriter.islemTipKod == -2)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "ISLEMTIPIKOD", ">", 50));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "ISLEMTIPIKOD", "<", 1000));
                }
            }
            sql += "ORDER BY A.SICILNO";

            veriErisim.sureAsimi = 120;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil No Hareket ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Sicil Raporu bilgisini dönen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Sicil No Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Sicil Raporu bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader SicilRaporuRFID(SicilNoHareket kriter, string[] yetkiDeger)
        {
            string sql = "SELECT A.*,B.*,RF.RFIDNO,TIP1.AD,TIP2.AD FROM ";
            sql += "(SELECT ";
            sql += "TASAMBARSICILVIEW.PRSICILNO";
            sql += ", TASAMBARSICILVIEW.SICILNO";
            sql += ", TASAMBARSICILVIEW.HESAPPLANAD";
            sql += ", TASAMBARSICILVIEW.KDVORAN";
            sql += ", TASAMBARSICILVIEW.KDVLIBIRIMFIYAT";
            sql += ", TASAMBARSICILVIEW.FIYAT";
            sql += ", TASAMBARSICILVIEW.ISLEMTIPIKOD";
            sql += ", TASAMBARSICILVIEW.ISLEMTURUKOD";
            sql += ", TASAMBARSICILVIEW.ISLEMTARIH";
            sql += ", TASAMBARSICILVIEW.MUHASEBEKOD";
            sql += ", TASAMBARSICILVIEW.MUHASEBEAD";
            sql += ", TASAMBARSICILVIEW.HARCAMABIRIMKOD";
            sql += ", TASAMBARSICILVIEW.HARCAMABIRIMAD";
            sql += ", TASAMBARSICILVIEW.AMBARKOD";
            sql += ", TASAMBARSICILVIEW.AMBARAD";
            sql += ", TASAMBARSICILVIEW.KIMEGITTI";
            sql += ", '' KISIAD";//UNION için gerekli
            sql += ", TASAMBARSICILVIEW.NEREYEGITTI";
            sql += ", '' ODAAD";//UNION için gerekli
            sql += ", TASAMBARSICILVIEW.ZIMMETOZELLIK";
            sql += ", TASAMBARSICILVIEW.MARKAAD";
            sql += ", TASAMBARSICILVIEW.MODELAD";
            sql += ", TASAMBARSICILVIEW.SASENO";
            sql += ", TASAMBARSICILVIEW.MOTORNO";
            sql += ", TASAMBARSICILVIEW.DISSICILNO";
            sql += ", TASAMBARSICILVIEW.HESAPPLANKOD";
            sql += ", TASAMBARSICILVIEW.PLAKA";
            sql += ", TASAMBARSICILVIEW.YERIKONUSU";
            sql += " FROM " + "TASAMBARSICILVIEW";

            string kosul = string.Empty;

            if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SICILNO", "NOT LIKE", "255070201??13%"));

            if (!string.IsNullOrEmpty(kriter.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD", "=", kriter.muhasebeKod));

            if (!string.IsNullOrEmpty(kriter.harcamaBirimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "HARCAMABIRIMKOD", "=", kriter.harcamaBirimKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kriter.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "AMBARKOD", "=", kriter.ambarKod));

            if (!string.IsNullOrEmpty(kriter.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "HESAPPLANKOD", "LIKE", kriter.hesapPlanKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(kriter.kimeGitti))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "KIMEGITTI", "=", kriter.kimeGitti));

            if (!string.IsNullOrEmpty(kriter.nereyeGitti))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "NEREYEGITTI", "=", kriter.nereyeGitti));

            //İşlem tarihine bakıyorum çünkü onaylanma tarihi önemli sicil açısından
            //Küçük tarih kriterini fisTarih değişkeni ile gönderiyorum istemciden
            //Çünkü class'ı değiştirmek istemedim.
            if (!kriter.fisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "ISLEMTARIH", ">=", kriter.fisTarih));

            //Büyük tarih kriterini islemTarih değişkeni ile gönderiyorum istemciden
            //Çünkü class'ı değiştirmek istemedim.
            if (!kriter.islemTarih.isNull)
            {
                kriter.islemTarih.AddDays(1);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "ISLEMTARIH", "<=", kriter.islemTarih));
            }

            if (!string.IsNullOrEmpty(kriter.fisNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "FISNO", "=", kriter.fisNo));

            if (kriter.fiyat > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "FIYAT", ">=", kriter.fiyat));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, "");
            if (orluYetkiler != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", orluYetkiler);

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " UNION ";
            sql += "SELECT ";
            sql += "TASAMBARZIMMETVIEW.PRSICILNO";
            sql += ", TASAMBARZIMMETVIEW.SICILNO";
            sql += ", TASAMBARZIMMETVIEW.HESAPPLANAD";
            sql += ", TASAMBARZIMMETVIEW.KDVORAN";
            sql += ", TASAMBARZIMMETVIEW.KDVLIBIRIMFIYAT";
            sql += ", TASAMBARZIMMETVIEW.FIYAT";
            sql += ", TASAMBARZIMMETVIEW.ISLEMTIPIKOD";
            sql += ", TASAMBARZIMMETVIEW.ISLEMTURUKOD";
            sql += ", TASAMBARZIMMETVIEW.ISLEMTARIH";
            sql += ", TASAMBARZIMMETVIEW.MUHASEBEKOD";
            sql += ", TASAMBARZIMMETVIEW.MUHASEBEAD";
            sql += ", TASAMBARZIMMETVIEW.HARCAMABIRIMKOD";
            sql += ", TASAMBARZIMMETVIEW.HARCAMABIRIMAD";
            sql += ", TASAMBARZIMMETVIEW.AMBARKOD";
            sql += ", TASAMBARZIMMETVIEW.AMBARAD";
            sql += ", TASAMBARZIMMETVIEW.KIMEGITTI";
            sql += ", TASAMBARZIMMETVIEW.KISIAD";
            sql += ", TASAMBARZIMMETVIEW.NEREYEGITTI";
            sql += ", TASAMBARZIMMETVIEW.ODAAD";
            sql += ", TASAMBARZIMMETVIEW.ZIMMETOZELLIK";
            sql += ", TASAMBARZIMMETVIEW.MARKAAD";
            sql += ", TASAMBARZIMMETVIEW.MODELAD";
            sql += ", TASAMBARZIMMETVIEW.SASENO";
            sql += ", TASAMBARZIMMETVIEW.MOTORNO";
            sql += ", TASAMBARZIMMETVIEW.DISSICILNO";
            sql += ", TASAMBARZIMMETVIEW.HESAPPLANKOD";
            sql += ", TASAMBARZIMMETVIEW.PLAKA";
            sql += ", TASAMBARZIMMETVIEW.YERIKONUSU";
            sql += " FROM " + "TASAMBARZIMMETVIEW";

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += ") A, TASSICILNOHAREKET B ";
            sql += " , TASSICILNORFID RF ";
            sql += " , TASISLEMTIP TIP1 ";
            sql += " , TASISLEMTIP TIP2 ";

            sql += "WHERE A.PRSICILNO=B.PRSICILNO ";
            sql += " AND A.PRSICILNO=RF.PRSICILNO ";
            sql += " AND TIP1.KOD=A.ISLEMTIPIKOD ";
            sql += " AND TIP2.KOD=B.ISLEMTIPIKOD ";

            sql += "AND B.SIRANO=1 ";
            sql += "ORDER BY A.SICILNO";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen sql cümlesine
        /// Cetvel Değer Artış alanlarını ekleyen fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <param name="tip">Tip bilgisini tutan, ENUMCetvelTipi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Cetvel Değer Artış Alanları eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        public string CetvelDegerArtislariAlanlari(string sql, ENUMCetvelTipi tip, int hesapKodUzunluk)
        {
            sql += " TASSICILHAREKETVIEW.YIL";
            if (tip == ENUMCetvelTipi.SAYIMCETVELI || tip == ENUMCetvelTipi.YONETIMHESABICETVELI)
            {
                sql += ", TASMUHASEBE.ILKOD";
                sql += ", TASMUHASEBE.ILCEKOD";
                sql += ", TASSICILHAREKETVIEW.MUHASEBEKOD";
                sql += ", TASSICILHAREKETVIEW.HARCAMABIRIMKOD";
                sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(TASSICILHAREKETVIEW.HESAPPLANKOD,1, 7)";
                sql += ", TASSICILHAREKETVIEW.OLCUBIRIMKOD";
            }
            else if (tip == ENUMCetvelTipi.HESAPCETVELI)
            {
                sql += ", TASMUHASEBE.ILKOD";
                sql += ", TASMUHASEBE.ILCEKOD";
                sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(TASSICILHAREKETVIEW.HESAPPLANKOD,1, 7)";
                sql += ", TASSICILHAREKETVIEW.OLCUBIRIMKOD";
            }
            else if (tip == ENUMCetvelTipi.HESAPCETVELIIL)
            {
                sql += ", TASMUHASEBE.ILKOD";
                sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(TASSICILHAREKETVIEW.HESAPPLANKOD,1, 7)";
                sql += ", TASSICILHAREKETVIEW.OLCUBIRIMKOD";
            }
            else if (tip == ENUMCetvelTipi.KESINHESAPCETVELI)
            {
                sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(TASSICILHAREKETVIEW.HESAPPLANKOD,1, 7)";
                sql += ", TASSICILHAREKETVIEW.OLCUBIRIMKOD";
                sql += ", TASSICILHAREKETVIEW.MUHASEBEKOD";
            }
            else if (tip == ENUMCetvelTipi.KESINHESAPCETVELIICMAL)
            {
                sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(TASSICILHAREKETVIEW.HESAPPLANKOD,1, " + hesapKodUzunluk + ")";
                sql += ", TASSICILHAREKETVIEW.MUHASEBEKOD";
            }
            else if (tip == ENUMCetvelTipi.MUZEKUTUPHANECETVELI)
            {
                sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(TASSICILHAREKETVIEW.HESAPPLANKOD,1, 7)";
                sql += ", TASSICILHAREKETVIEW.OLCUBIRIMKOD";
            }
            else if (tip == ENUMCetvelTipi.HESAPCETVELIBOLGE)
            {
                sql += ", TASMUHASEBE.BOLGEKOD";
                sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(TASSICILHAREKETVIEW.HESAPPLANKOD,1, 7)";
                sql += ", TASSICILHAREKETVIEW.OLCUBIRIMKOD";
            }
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen Cetvel ve Yetki Değerini kıstas olarak kullanarak,
        /// kıstasa uygun Cetvel Değer Artışları bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="cetvel">Cetvel bilgisini tutan, Cetvel türünde nesne</param>
        /// <param name="tip">Tip bilgisini tutan, ENUMCetvelTipi türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değeri bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Cetvel Değer Artışları bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader CetvelDegerArtislari(Cetvel cetvel, ENUMCetvelTipi tip, string[] yetkiDeger, int hesapKodUzunluk)
        {
            string sql = "SELECT";
            sql = CetvelDegerArtislariAlanlari(sql, tip, hesapKodUzunluk);
            sql += ", " + VeriErisim.IslecBul("SUM") + "(TASSICILHAREKETVIEW.DEGERARTISI)";
            sql += " FROM TASSICILHAREKETVIEW, TASMUHASEBE";
            sql += " WHERE TASSICILHAREKETVIEW.MUHASEBEKOD = TASMUHASEBE.KOD";

            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL));

            //if (cetvel.yil > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "YIL", "=", cetvel.yil));

            if (!string.IsNullOrEmpty(cetvel.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "MUHASEBEKOD", "=", cetvel.muhasebeKod));

            if (!string.IsNullOrEmpty(cetvel.harcamaKod))
            {
                //Harcama birim koduna ; ayıracı ile birden fazla hb yazılıp cetvelin ona göre alınması için
                cetvel.harcamaKod = cetvel.harcamaKod.Replace(".", "");
                cetvel.harcamaKod = cetvel.harcamaKod.Replace(" ", "");

                if (cetvel.harcamaKod.Contains(";"))
                {
                    string[] hbler = cetvel.harcamaKod.Split(';');
                    string hbKosul = "";
                    for (int i = 0; i < hbler.Length; i++)
                        if (hbler[i] != "")
                            hbKosul = VeriErisim.KosulEkle(hbKosul, "OR", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "HARCAMABIRIMKOD", "=", hbler[i]));

                    if (hbKosul != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", "(" + hbKosul + ")");
                }
                else if (cetvel.harcamaKod.Length < 11)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "HARCAMABIRIMKOD", "LIKE", cetvel.harcamaKod + "%"));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "HARCAMABIRIMKOD", "=", cetvel.harcamaKod));
            }

            if (!string.IsNullOrEmpty(cetvel.ilKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ILKOD", "=", cetvel.ilKod));

            if (!string.IsNullOrEmpty(cetvel.ilceKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ILCEKOD", "=", cetvel.ilceKod));

            if (!string.IsNullOrEmpty(cetvel.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "HESAPPLANKOD", "LIKE", cetvel.hesapPlanKod.Replace(".", "") + "%"));

            if (cetvel.bolgeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASMUHASEBE", "BOLGEKOD", "=", cetvel.bolgeKod));


            if (tip != ENUMCetvelTipi.SAYIMCETVELI && tip != ENUMCetvelTipi.YONETIMHESABICETVELI && tip != ENUMCetvelTipi.KESINHESAPCETVELIICMAL && tip != ENUMCetvelTipi.KESINHESAPCETVELI)//Cetveller tarih aralığı alındığında 14 nolu cetvel muhasebe rapor formatında uyumsuzluk yaratıyor
            {
                if (!cetvel.tarih1.isNull)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "FISTARIH", ">=", cetvel.tarih1));

                if (!cetvel.tarih2.isNull)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "FISTARIH", "<=", cetvel.tarih2));
            }

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "DEGERARTISI", ">", 0));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, "TASSICILHAREKETVIEW");
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " GROUP BY";
            sql = CetvelDegerArtislariAlanlari(sql, tip, hesapKodUzunluk);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form ve Yetki Değerini kıstas olarak kullanarak,
        /// kıstasa uygun Girişi Yapılmamış Devir Çıkışları bilgisini dönen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değeri bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Girişi Yapılmamış Devir Çıkış bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader GirisiYapilmamisDevirCikislari(TasinirIslemForm kriter, string[] yetkiDeger)
        {
            string sql = "SELECT ";
            sql += VeriErisim.TumAlanlar("A", ALANLARTIF);
            sql += ", " + "TASISLEMTIP.AD";
            sql += ", " + "TASISLEMTIP.TUR";
            sql += ", " + "A.ACIKLAMA";
            sql += " FROM " + TABLOADTIF + " A, TASISLEMTIP";
            sql += " WHERE " + "A.ISLEMTIPIKOD=TASISLEMTIP.KOD";
            if (kriter.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMTIP", "TUR", "=", (int)ENUMIslemTipi.DEVIRCIKIS));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMTIP", "TUR", "=", (int)ENUMIslemTipi.DAGITIMIADECIKIS));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "DURUM", "=", (int)ENUMBelgeDurumu.ONAYLI));

            //Devir girişi olmuş mu olmamış mı için ek sql
            string sql2 = "SELECT *";
            sql2 += " FROM " + TABLOADTIF + " B, TASISLEMTIP";
            sql2 += " WHERE " + "B.ISLEMTIPIKOD=TASISLEMTIP.KOD";
            if (kriter.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl("TASISLEMTIP", "TUR", "=", (int)ENUMIslemTipi.DEVIRGIRIS));
            else
                sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl("TASISLEMTIP", "TUR", "=", (int)ENUMIslemTipi.DAGITIMIADEGIRIS));

            sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl("B", "DURUM", "=", (int)ENUMBelgeDurumu.ONAYLI));
            sql2 += " AND " + "A.YIL=B.GYIL";
            sql2 += " AND " + "A.MUHASEBEKOD=B.GMUHASEBEKOD";
            sql2 += " AND " + "A.HARCAMABIRIMKOD=B.GHARCAMAKOD";
            sql2 += " AND " + "A.FISNO=B.GFISNO";

            sql += " AND NOT EXISTS (" + sql2 + ")";

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "YIL", "=", kriter.yil));

            if (!string.IsNullOrEmpty(kriter.muhasebeKod)
                && !string.IsNullOrEmpty(kriter.harcamaKod)
                && !string.IsNullOrEmpty(kriter.ambarKod))
            {
                sql = VeriErisim.KosulEkle(sql, "AND ((", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", kriter.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HARCAMABIRIMKOD", "=", kriter.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "AMBARKOD", "=", kriter.ambarKod));
                sql += ")";
                sql = VeriErisim.KosulEkle(sql, "OR (", VeriErisim.KosulOl("A", "GMUHASEBEKOD", "=", kriter.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "GHARCAMAKOD", "=", kriter.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "GAMBARKOD", "=", kriter.ambarKod));
                sql += "))";
            }
            else if (!string.IsNullOrEmpty(kriter.muhasebeKod)
                && !string.IsNullOrEmpty(kriter.harcamaKod))
            {
                sql = VeriErisim.KosulEkle(sql, "AND ((", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", kriter.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HARCAMABIRIMKOD", "=", kriter.harcamaKod.Replace(".", "")));
                sql += ")";
                sql = VeriErisim.KosulEkle(sql, "OR (", VeriErisim.KosulOl("A", "GMUHASEBEKOD", "=", kriter.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "GHARCAMAKOD", "=", kriter.harcamaKod.Replace(".", "")));
                sql += "))";
            }
            else if (!string.IsNullOrEmpty(kriter.muhasebeKod))
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", kriter.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("A", "GMUHASEBEKOD", "=", kriter.muhasebeKod));
                sql += ")";
            }

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, "A");
            if (orluYetkiler != "")
            {
                string orluYetkilerG = orluYetkiler.Replace("MUHASEBEKOD", "GMUHASEBEKOD");
                orluYetkilerG = orluYetkilerG.Replace("HARCAMABIRIMKOD", "GHARCAMAKOD");
                orluYetkilerG = orluYetkilerG.Replace("AMBARKOD", "GAMBARKOD");

                sql = VeriErisim.KosulEkle(sql, "AND", "((" + orluYetkiler + ") OR (" + orluYetkilerG + "))");
            }

            sql += " ORDER BY YIL, MUHASEBEKOD, HARCAMABIRIMKOD, AMBARKOD, FISNO, GMUHASEBEKOD, GHARCAMAKOD, GAMBARKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Kuruş Farkı ve Yetki Değeri bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Kuruş Farkları bilgisini dönen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kf">Kuruş Farkı bilgisini tutan, KurusFarki türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Kuruş Farkları bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader KurusFarklariRaporu(KurusFarki kf, string[] yetkiDeger)
        {
            string sql = "SELECT" + " YIL";
            sql += ", " + "ILKOD";
            sql += ", " + "ILAD";
            sql += ", " + "ILCEKOD";
            sql += ", " + "ILCEAD";
            sql += ", " + "MUHASEBEKOD";
            sql += ", " + "MUHASEBEAD";
            sql += ", " + "HARCAMABIRIMKOD";
            sql += ", " + "HARCAMABIRIMAD";
            sql += ", " + "FISNO";
            sql += ", " + "ISLEMTIPITUR";
            sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPPLANKOD, 1, 7)";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(MIKTAR * BIRIMFIYATKDVLI)";
            sql += " FROM " + TABLOADSTOKHAREKETVIEW;

            string kosul = string.Empty;
            if (kf.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", kf.yil));

            if (!string.IsNullOrEmpty(kf.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "=", kf.muhasebeKod));

            if (!string.IsNullOrEmpty(kf.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "=", kf.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kf.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARKOD], "=", kf.ambarKod));

            if (!string.IsNullOrEmpty(kf.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", kf.hesapPlanKod.Replace(".", "") + "%"));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.TUKETIMCIKIS));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.YILDEVIRGIRIS));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.YILDEVIRCIKIS));

            if (!kf.tarih1.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISTARIH], ">=", kf.tarih1));

            if (!kf.tarih2.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISTARIH], "<=", kf.tarih2));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADSTOKHAREKETVIEW);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " GROUP BY" + " YIL";
            sql += ", " + "ILKOD";
            sql += ", " + "ILAD";
            sql += ", " + "ILCEKOD";
            sql += ", " + "ILCEAD";
            sql += ", " + "MUHASEBEKOD";
            sql += ", " + "MUHASEBEAD";
            sql += ", " + "HARCAMABIRIMKOD";
            sql += ", " + "HARCAMABIRIMAD";
            sql += ", " + "FISNO";
            sql += ", " + "ISLEMTIPITUR";
            sql += ", " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPPLANKOD, 1, 7)";

            sql += " HAVING " + VeriErisim.IslecBul("ROUND");
            sql += "(" + VeriErisim.IslecBul("SUM") + "(MIKTAR * BIRIMFIYATKDVLI), 2)";
            sql += " - " + VeriErisim.IslecBul("SUM") + "(MIKTAR * BIRIMFIYATKDVLI) <> 0";
            sql += " OR " + VeriErisim.IslecBul("ROUND");
            sql += "(" + VeriErisim.IslecBul("SUM") + "(MIKTAR * BIRIMFIYATKDVLI), 2)";
            sql += " - " + VeriErisim.IslecBul("SUM") + "(" + VeriErisim.IslecBul("ROUND") + "(MIKTAR * BIRIMFIYATKDVLI, 2)) <> 0";

            sql += " ORDER BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPPLANKOD, 1, 7)";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Depo Durumu, Depo Durum İşlem ve Yetki Değerini kıstas olarak kullanarak,
        /// kıstasa uygun En Çok Kullanılan Taşınırlar bilgisini dönen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="dp">Depo Durumu bilgisini tutan, DepoDurum türünde nesne</param>
        /// <param name="depoDurumIslem">Depo Durum İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değeri bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun En Çok Kullanılan Taşınır bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader EnCokKullanilanTasinirlar(DepoDurum dp, string depoDurumIslem, string[] yetkiDeger)
        {
            string sql = "SELECT ";
            sql += TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + "*" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BIRIMFIYATKDVLI] + ")";
            sql += " FROM " + TABLOADSTOKHAREKETVIEW;

            string kosul = string.Empty;
            if (depoDurumIslem == "gecenYilTuketim" && dp.yil > 0)
            {
                kosul += VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", dp.yil - 1);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));
            }
            else if (depoDurumIslem == "donemselTuketim" && dp.yil > 0)
            {
                kosul += VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", dp.yil);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));
            }
            else if (dp.yil > 0)
            {
                kosul = "((";
                kosul += VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", dp.yil);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));
                kosul += ")";
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], " NOT LIKE", ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() + "%"));//demirbaşlar için
                kosul += ")";
            }

            if (!string.IsNullOrEmpty(dp.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "=", dp.muhasebeKod));

            if (!string.IsNullOrEmpty(dp.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "LIKE", dp.harcamaKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(dp.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.AMBARKOD], "=", dp.ambarKod));

            if (!string.IsNullOrEmpty(dp.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", dp.hesapPlanKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(dp.ilKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILKOD], "=", dp.ilKod));

            if (!string.IsNullOrEmpty(dp.ilceKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ILCEKOD], "=", dp.ilceKod));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            if (depoDurumIslem == "giren")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], ">=", (int)ENUMIslemTipi.SATINALMAGIRIS));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<", (int)ENUMIslemTipi.TUKETIMCIKIS));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.ZFDUSME));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.DTLDUSME));
            }
            else if (depoDurumIslem == "cikan")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND ((", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], ">=", (int)ENUMIslemTipi.TUKETIMCIKIS));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.ZFVERME));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.DTLVERME));
                kosul += ")";
                kosul = VeriErisim.KosulEkle(kosul, "OR (", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "=", (int)ENUMIslemTipi.DEGERARTTIR));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", (int)ENUMTasinirHesapKodu.TUKETIM + "%"));
                kosul += "))";
            }
            else if (depoDurumIslem == "gecenYilTuketim" || depoDurumIslem == "donemselTuketim")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "=", (int)ENUMIslemTipi.TUKETIMCIKIS));
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "=", (int)ENUMIslemTipi.DEGERARTTIR));
                kosul += ")";

                if (depoDurumIslem == "donemselTuketim")
                {
                    TNSDateTime tarih1 = null;
                    TNSDateTime tarih2 = null;
                    if (dp.donem == 1)
                    {
                        tarih1 = new TNSDateTime("01.01." + DateTime.Today.Year.ToString());
                        tarih2 = new TNSDateTime("31.03." + DateTime.Today.Year.ToString());
                    }
                    else if (dp.donem == 2)
                    {
                        tarih1 = new TNSDateTime("01.04." + DateTime.Today.Year.ToString());
                        tarih2 = new TNSDateTime("30.06." + DateTime.Today.Year.ToString());
                    }
                    else if (dp.donem == 3)
                    {
                        tarih1 = new TNSDateTime("01.07." + DateTime.Today.Year.ToString());
                        tarih2 = new TNSDateTime("30.09." + DateTime.Today.Year.ToString());
                    }
                    else if (dp.donem == 4)
                    {
                        tarih1 = new TNSDateTime("01.10." + DateTime.Today.Year.ToString());
                        tarih2 = new TNSDateTime("31.12." + DateTime.Today.Year.ToString());
                    }
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISTARIH], ">=", tarih1));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.FISTARIH], "<=", tarih2));
                }
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADSTOKHAREKETVIEW);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " GROUP BY ";
            sql += TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANAD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMKOD];
            sql += ", " + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.OLCUBIRIMAD];

            sql += " ORDER BY ";
            sql += VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD] + ", 1, 3)";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + "*" + TABLOADSTOKHAREKETVIEW + "." + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BIRIMFIYATKDVLI] + ") DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen İstek Form ve Yetki Değerini kıstas olarak kullanarak,
        /// kıstasa uygun İştek Karşılama bilgisini dönen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">İstek Form bilgisini tutan, IstekForm türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun İstek Karşılama bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader IstekKarsilananRaporu(IstekForm kriter, string[] yetkiDeger)
        {
            string sql = "SELECT * FROM " + TABLOADISTEKKARSILANANVIEW;

            string kosul = string.Empty;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADISTEKKARSILANANVIEW, "YIL", "=", kriter.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADISTEKKARSILANANVIEW, "MUHASEBEKOD", "=", kriter.muhasebeKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADISTEKKARSILANANVIEW, "HARCAMABIRIMKOD", "=", kriter.harcamaKod));

            if (!string.IsNullOrEmpty(kriter.istekYapanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADISTEKKARSILANANVIEW, "ISTEKYAPAN", "=", kriter.istekYapanKod));

            if (kriter.baslaTarih.Yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADISTEKKARSILANANVIEW, "ISTEKTARIHI", ">=", kriter.baslaTarih));
            if (kriter.bitTarih.Yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADISTEKKARSILANANVIEW, "ISTEKTARIHI", "<=", kriter.bitTarih));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, "TASISTEKFORM");
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Muhasabat Kontrol ve Yetki Değerini kıstas olarak kullanarak,
        /// kıstasa uygun Muhasabat Kontrol bilgilerini dönen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Muhasabat Kontrol bilgisini tutan, MuhasabatKontrol türünde nesne</param>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Muhasabat Kontrol bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader MuhasebatKontrolRaporu(MuhasebatKontrol kriter, string[] yetkiDeger)
        {
            string alanlar = VeriErisim.IslecBul("LEFT") + "(" + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD] + ", " + kriter.hesapKodUzunluk + ")";
            alanlar += ", " + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR];

            string ekAlanlar = string.Empty;
            if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE_HARCAMA)
                ekAlanlar = ", " + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD];
            if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA || kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE_HARCAMA)
                ekAlanlar += ", " + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD];

            string sql = "SELECT ";
            sql += alanlar;
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MIKTAR] + " * " + ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.BIRIMFIYATKDVLI] + ")";
            sql += ekAlanlar;
            sql += " FROM " + TABLOADSTOKHAREKETVIEW;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.ISLEMTIPITUR], "<>", (int)ENUMIslemTipi.YILDEVIRCIKIS));

            sql = VeriErisim.KosulEkle(sql, "AND ((", VeriErisim.KosulOl(string.Empty, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "=", kriter.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", (int)ENUMTasinirHesapKodu.TUKETIM + "%"));
            sql = VeriErisim.KosulEkle(sql, ") OR (", VeriErisim.KosulOl(string.Empty, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.YIL], "<=", kriter.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "NOT LIKE", (int)ENUMTasinirHesapKodu.TUKETIM + "%"));
            sql += "))";

            if (!string.IsNullOrEmpty(kriter.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "LIKE", kriter.muhasebeKod + "%"));

            if (!string.IsNullOrEmpty(kriter.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "LIKE", kriter.harcamaKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(kriter.hesapKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", kriter.hesapKod.Replace(".", "") + "%"));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, string.Empty);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " GROUP BY " + alanlar + ekAlanlar;
            sql += " ORDER BY " + ekAlanlar.Substring(2) + ", " + alanlar;

            return veriErisim.DataReaderDondur(sql);
        }

        ///// <summary>
        ///// Parametre olarak verilen Birim Fiyat Ortalaması bilgisini kıstas olarak kullanarak,
        ///// kıstasa uyugn Birim Fiyat Ortalamaları bilgisini dönen,
        ///// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        ///// </summary>
        ///// <param name="ort">Birim Fiyat Ortalaması bilgisini tutan, BirimFiyatOrtalamasi türünde nesne</param>
        ///// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Birim Fiyat Ortalaması bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        //public IDataReader BirimFiyatOrtalamalari(BirimFiyatOrtalamasi ort)
        //{
        //    string sql = "SELECT ";
        //    sql += TABLOADTIFDETAY + ".HESAPPLANKOD";
        //    sql += ", " + VeriErisim.IslecBul("AVG") + "(" + TABLOADTIFDETAY + ".BIRIMFIYAT)";
        //    sql += ", " + TABHesapPlani.TABLOAD + ".ACIKLAMA ";

        //    sql += " FROM " + TABLOADTIF + " INNER JOIN " + TABLOADTIFDETAY + " ON ";
        //    sql += TABLOADTIF + ".YIL=" + TABLOADTIFDETAY + ".YIL";
        //    sql += " AND " + TABLOADTIF + ".MUHASEBEKOD=" + TABLOADTIFDETAY + ".MUHASEBEKOD";
        //    sql += " AND " + TABLOADTIF + ".HARCAMABIRIMKOD=" + TABLOADTIFDETAY + ".HARCAMABIRIMKOD";
        //    sql += " AND " + TABLOADTIF + ".FISNO =" + TABLOADTIFDETAY + ".FISNO ";
        //    sql += " AND " + TABLOADTIFDETAY + ".HESAPPLANKOD LIKE '" + ort.hesapKod.Replace(".", "") + "%' ";

        //    sql += " INNER JOIN " + TABHesapPlani.TABLOAD + " ON ";
        //    sql += TABLOADTIFDETAY + ".HESAPPLANKOD =" + TABHesapPlani.TABLOAD + ".KOD";

        //    sql += " GROUP BY " + TABLOADTIF + ".ISLEMTIPIKOD," + TABLOADTIF + ".DURUM," + TABLOADTIFDETAY + ".HESAPPLANKOD," + TABLOADTIF + ".YIL, " + TABHesapPlani.TABLOAD + ".ACIKLAMA "; ;

        //    sql = VeriErisim.KosulEkle(sql, " HAVING ", VeriErisim.KosulOl(TABLOADTIF, "ISLEMTIPIKOD", "=", (int)ENUMIslemTipi.SATINALMAGIRIS));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, "DURUM", "=", (int)ENUMBelgeDurumu.ONAYLI));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, "YIL", "=", ort.yil));


        //    return veriErisim.DataReaderDondur(sql);
        //}

        public IDataReader DevirCikis(DevirCikis tk, int islemTur)
        {
            tk.tarih2.AddDays(1);

            string sql = "SELECT TASISLEMFORM.FISNO, TASISLEMFORM.MUHASEBEKOD, TASISLEMFORM.HARCAMABIRIMKOD, TASISLEMFORM.GMUHASEBEKOD, TASISLEMFORM.GHARCAMAKOD, SUBSTRING(TASISLEMDETAY.HESAPPLANKOD,1,7), sum(TASISLEMDETAY.MIKTAR*TASISLEMDETAY.BIRIMFIYAT), sum(TASISLEMDETAY.MIKTAR*TASISLEMDETAY.BIRIMFIYATKDVLI), TASISLEMFORM.YIL";
            sql += " FROM TASISLEMFORM INNER JOIN";
            sql += " TASISLEMDETAY ON TASISLEMFORM.YIL = TASISLEMDETAY.YIL AND TASISLEMFORM.MUHASEBEKOD = TASISLEMDETAY.MUHASEBEKOD AND";
            sql += " TASISLEMFORM.HARCAMABIRIMKOD = TASISLEMDETAY.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO = TASISLEMDETAY.FISNO";
            sql += " INNER JOIN TASISLEMTIP ON TASISLEMFORM.ISLEMTIPIKOD = TASISLEMTIP.KOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TASISLEMFORM", "DURUM", "=", (int)ENUMBelgeDurumu.ONAYLI));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMTIP", "TUR", "=", islemTur));
            sql = VeriErisim.KosulEkle(sql, "AND", "(TASISLEMFORM.MUHASEBEKOD<>TASISLEMFORM.GMUHASEBEKOD OR TASISLEMFORM.HARCAMABIRIMKOD<>TASISLEMFORM.GHARCAMAKOD)");

            if (tk.hesapPlanKod != "-999")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMDETAY", "HESAPPLANKOD", "LIKE", "150%"));

            if (tk.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMFORM", "YIL", "=", tk.yil));

            if (!string.IsNullOrEmpty(tk.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMFORM", "MUHASEBEKOD", "=", tk.muhasebeKod));

            if (!string.IsNullOrEmpty(tk.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMFORM", "HARCAMABIRIMKOD", "=", tk.harcamaKod.Replace(".", "")));

            if (!tk.tarih1.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMFORM", "FISTARIH", ">=", tk.tarih1));

            if (!tk.tarih2.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMFORM", "FISTARIH", "<", tk.tarih2));

            sql += " GROUP BY TASISLEMFORM.FISNO, TASISLEMFORM.MUHASEBEKOD, TASISLEMFORM.HARCAMABIRIMKOD, TASISLEMFORM.GMUHASEBEKOD, TASISLEMFORM.GHARCAMAKOD, SUBSTRING(TASISLEMDETAY.HESAPPLANKOD,1,7),TASISLEMFORM.YIL";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Depo Durumu, Depo Durum İşlemi ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Depo Durumu bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="dp">Depo Durum bilgisini tutan, DepoDurum türünde nesne</param>
        /// <param name="depoDurumIslem">Depo Durum İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değeri bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır Depo Durum bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader KHKRaporu(DepoDurum dp, string depoDurumIslem, string[] yetkiDeger)
        {
            string sql = @" select [NEREDENGELDI],[HESAPPLANKOD],count(*) miktar,
sum(KDVLIBIRIMFIYAT) tutar
from [dbo].[TASSICILNOHAREKET]
INNER JOIN dbo.TASHESAPPLAN ON dbo.[TASSICILNOHAREKET].HESAPPLANKOD = dbo.TASHESAPPLAN.KOD
INNER JOIN TASMUHASEBE ON TASMUHASEBE.KOD = [TASSICILNOHAREKET].MUHASEBEKOD
INNER JOIN TASIL ON TASMUHASEBE.ILKOD = TASIL.KOD ";

            if (!string.IsNullOrEmpty(dp.TcVNo))
                sql += @" LEFT OUTER JOIN TASFIRMA ON TASFIRMA.AD = [TASSICILNOHAREKET].NEREDENGELDI ";

            sql += @" WHERE dbo.[TASSICILNOHAREKET].ISLEMTIPIKOD = " + (int)ENUMIslemTipi.KHKGIRIS;

            if (depoDurumIslem == "cikan")
            {
                sql += @" and PRSICILNO in (select PRSICILNO
from [dbo].[TASSICILNOHAREKET] 
where ISLEMTIPIKOD = " + (int)ENUMIslemTipi.KHKCIKIS + ")";
            }

            if (dp.yil > 0)
                sql += " and dbo.TASSICILNOHAREKET.YIL = '" + dp.yil + "' ";

            if (!string.IsNullOrEmpty(dp.muhasebeKod))
                sql += " and dbo.TASSICILNOHAREKET.MUHASEBEKOD = '" + dp.muhasebeKod + "' ";

            if (!string.IsNullOrEmpty(dp.harcamaKod))
                sql += " and dbo.TASSICILNOHAREKET.HARCAMABIRIMKOD = '" + dp.harcamaKod.Replace(".", "") + "' ";

            if (!string.IsNullOrEmpty(dp.hesapPlanKod))
                sql += " and dbo.TASSICILNOHAREKET.HESAPPLANKOD LIKE '" + dp.hesapPlanKod.Replace(".", "") + "%' ";

            if (!string.IsNullOrEmpty(dp.ilKod))
                sql += " and dbo.TASIL.KOD = '" + dp.ilKod + "' ";

            if (!string.IsNullOrEmpty(dp.firma))
                sql += " and dbo.TASSICILNOHAREKET.NEREDENGELDI LIKE '" + dp.firma + "' ";

            if (!string.IsNullOrEmpty(dp.TcVNo))
                sql += " and dbo.TASFIRMA.VNO = '" + dp.TcVNo + "' ";

            if (!dp.baslamaTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("dbo.TASSICILNOHAREKET", "FISTARIH", ">=", dp.baslamaTarih));

            if (!dp.bitisTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("dbo.TASSICILNOHAREKET", "FISTARIH", "<=", dp.bitisTarih));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADTIF);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += @" group by [NEREDENGELDI],[HESAPPLANKOD] ";

            sql += " order by 1,2";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Formu, Taşınır Form Kriteri ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Malzeme Tarihçe bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="tfKriter">Taşınır Form Kriter bilgisini tutan, TasinirFormKriter türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Malzeme Tarihçe bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TasinirKodGrupluTasinirHurdaRaporu(TasinirIslemForm tf, TasinirFormKriter tfKriter, string[] yetkiDeger)
        {
            string sql = @"SELECT 
                            TASISLEMFORM.MUHASEBEKOD, 
                            TASISLEMFORM.HARCAMABIRIMKOD,
                            TASISLEMFORM.AMBARKOD, 
                            TASISLEMFORM.FISNO belgeno,
                            TASISLEMFORM.FISTARIH belgetarihi, 
                            TASISLEMTIP.AD islemtipi, 
                            TASISLEMFORM.NEREYEGITTI, 
--TASISLEMDETAY.HESAPPLANKOD,
                            substring(TASISLEMDETAY.HESAPPLANKOD,0,4) hesapplankod,
                            --TASHESAPPLAN.ACIKLAMA tasiniradi,  
                            --TASHESAPPLAN.OLCUBIRIMKOD , 
                            TASOLCUBIRIM.AD,
                            sum(TASISLEMDETAY.MIKTAR) miktar, 
                            sum(TASISLEMDETAY.MIKTAR*TASISLEMDETAY.BIRIMFIYATKDVLI) tutar
                            FROM TASISLEMFORM, TASISLEMDETAY, TASHESAPPLAN, TASISLEMTIP, TASOLCUBIRIM 
                            WHERE TASISLEMFORM.YIL=TASISLEMDETAY.YIL AND TASISLEMFORM.MUHASEBEKOD=TASISLEMDETAY.MUHASEBEKOD 
                            AND TASISLEMFORM.HARCAMABIRIMKOD=TASISLEMDETAY.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO=TASISLEMDETAY.FISNO 
                            and TASOLCUBIRIM.KOD = TASHESAPPLAN.OLCUBIRIMKOD
                            AND TASHESAPPLAN.KOD=TASISLEMDETAY.HESAPPLANKOD AND TASISLEMTIP.KOD=TASISLEMFORM.ISLEMTIPIKOD ";

            //AND (TASISLEMFORM.ISLEMTIPIKOD=11 OR TASISLEMFORM.ISLEMTIPIKOD=12 
            //     OR TASISLEMFORM.ISLEMTIPIKOD=22)  ";
            //AND TASISLEMFORM.YIL=2016 
            //                            and (TASISLEMDETAY.HESAPPLANKOD like '150%' or TASISLEMDETAY.HESAPPLANKOD like '253%' 
            //                            or TASISLEMDETAY.HESAPPLANKOD like '254%' or TASISLEMDETAY.HESAPPLANKOD like '255%') ";

            if (tf.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.YIL], "=", tf.yil));

            if (!string.IsNullOrEmpty(tf.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.MUHASEBEKOD], "LIKE", tf.muhasebeKod));

            if (!string.IsNullOrEmpty(tf.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.HARCAMABIRIMKOD], "LIKE", tf.harcamaKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tf.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.AMBARKOD], "=", tf.ambarKod));

            if (!tfKriter.belgeTarihBasla.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], ">=", tfKriter.belgeTarihBasla));

            if (!tfKriter.belgeTarihBit.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], "<=", tfKriter.belgeTarihBit));

            if (tf.islemTipKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.ISLEMTIPKOD], "=", tf.islemTipKod));
            if (tfKriter.islemTipleri != "")
            {
                string[] islemTipleri = tfKriter.islemTipleri.Split(';');
                string islemTipKosul = "";
                for (int i = 0; i < islemTipleri.Length; i++)
                    islemTipKosul = VeriErisim.KosulEkle(islemTipKosul, "OR", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.ISLEMTIPKOD], "=", OrtakFonksiyonlar.ConvertToInt(islemTipleri[i], 0)));

                if (islemTipKosul != "")
                    sql += " AND (" + islemTipKosul + ")";
            }

            if (!string.IsNullOrEmpty(tf.gMuhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GMUHASEBEKOD], "=", tf.gMuhasebeKod));

            if (!string.IsNullOrEmpty(tf.gHarcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GHARCAMAKOD], "=", tf.gHarcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(tf.gAmbarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GAMBARKOD], "=", tf.gAmbarKod));

            if (!string.IsNullOrEmpty(tf.neredenGeldi))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.NEREDENGELDI], "=", tf.neredenGeldi));

            if (!string.IsNullOrEmpty(tf.nereyeGitti))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.NEREYEGITTI], "=", tf.nereyeGitti));

            if (!string.IsNullOrEmpty(tf.kimeGitti))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.KIMEGITTI], "=", tf.kimeGitti));

            if (!string.IsNullOrEmpty(tfKriter.hesapKodu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIFDETAY, ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.HESAPPLANKOD], "LIKE", tfKriter.hesapKodu.Replace(".", "") + "%"));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADTIF);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += @"   group by TASISLEMFORM.MUHASEBEKOD, 
                        TASISLEMFORM.HARCAMABIRIMKOD,
                        TASISLEMFORM.AMBARKOD, 
                        TASISLEMFORM.FISNO ,
                        TASISLEMFORM.FISTARIH , 
                        TASISLEMTIP.AD , 
                        TASISLEMFORM.NEREYEGITTI, 
--TASISLEMDETAY.HESAPPLANKOD
                        substring(TASISLEMDETAY.HESAPPLANKOD,0,4) ,
                        TASOLCUBIRIM.AD
                        --TASHESAPPLAN.ACIKLAMA ,
                        --TASHESAPPLAN.OLCUBIRIMKOD 
                        ORDER BY TASISLEMFORM.MUHASEBEKOD, 
                        TASISLEMFORM.HARCAMABIRIMKOD,
                        TASISLEMFORM.AMBARKOD, 
                        TASISLEMFORM.FISNO ,
                        TASISLEMFORM.FISTARIH , 
                        TASISLEMTIP.AD , 
                        TASISLEMFORM.NEREYEGITTI, 
--TASISLEMDETAY.HESAPPLANKOD
                        substring(TASISLEMDETAY.HESAPPLANKOD,0,4) ,
                        TASOLCUBIRIM.AD
                        --TASHESAPPLAN.ACIKLAMA,
                        --TASHESAPPLAN.OLCUBIRIMKOD ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Formu, Taşınır Form Kriteri ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Malzeme Tarihçe bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="tfKriter">Taşınır Form Kriter bilgisini tutan, TasinirFormKriter türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Malzeme Tarihçe bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader IlBazliTasinirHurdaRaporu(TasinirIslemForm tf, TasinirFormKriter tfKriter, string[] yetkiDeger)
        {
            string kosul = " ";
            string sql = @"SELECT TASIL.KOD
                            , TASIL.AD
                            , substring(TASISLEMDETAY.HESAPPLANKOD,0,4) hesapplankod
                            , sum(TASISLEMDETAY.MIKTAR) miktar
                            , sum(TASISLEMDETAY.MIKTAR*TASISLEMDETAY.BIRIMFIYATKDVLI) tutar
                            FROM TASISLEMFORM, TASISLEMDETAY, TASHESAPPLAN, TASISLEMTIP, TASMUHASEBE, TASIL
                            WHERE TASISLEMFORM.YIL=TASISLEMDETAY.YIL 
                            AND TASISLEMFORM.MUHASEBEKOD=TASISLEMDETAY.MUHASEBEKOD 
                            AND TASISLEMFORM.HARCAMABIRIMKOD=TASISLEMDETAY.HARCAMABIRIMKOD 
                            AND TASISLEMFORM.FISNO=TASISLEMDETAY.FISNO 
                            AND TASHESAPPLAN.KOD=TASISLEMDETAY.HESAPPLANKOD 
                            AND TASISLEMTIP.KOD=TASISLEMFORM.ISLEMTIPIKOD 
                            and TASMUHASEBE.KOD = TASISLEMFORM.MUHASEBEKOD
                            and TASMUHASEBE.ILKOD = TASIL.KOD ";

            if (tf.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.YIL], "=", tf.yil));

            if (!string.IsNullOrEmpty(tf.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.MUHASEBEKOD], "LIKE", tf.muhasebeKod));

            if (!string.IsNullOrEmpty(tf.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.HARCAMABIRIMKOD], "LIKE", tf.harcamaKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tf.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.AMBARKOD], "=", tf.ambarKod));

            if (!tfKriter.belgeTarihBasla.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], ">=", tfKriter.belgeTarihBasla));

            if (!tfKriter.belgeTarihBit.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], "<=", tfKriter.belgeTarihBit));

            if (tf.islemTipKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.ISLEMTIPKOD], "=", tf.islemTipKod));
            if (tfKriter.islemTipleri != "")
            {
                string[] islemTipleri = tfKriter.islemTipleri.Split(';');
                string islemTipKosul = "";
                for (int i = 0; i < islemTipleri.Length; i++)
                    islemTipKosul = VeriErisim.KosulEkle(islemTipKosul, "OR", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.ISLEMTIPKOD], "=", OrtakFonksiyonlar.ConvertToInt(islemTipleri[i], 0)));

                if (islemTipKosul != "")
                    kosul += " AND (" + islemTipKosul + ")";
            }

            if (!string.IsNullOrEmpty(tf.gMuhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GMUHASEBEKOD], "=", tf.gMuhasebeKod));

            if (!string.IsNullOrEmpty(tf.gHarcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GHARCAMAKOD], "=", tf.gHarcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(tf.gAmbarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GAMBARKOD], "=", tf.gAmbarKod));

            if (!string.IsNullOrEmpty(tf.neredenGeldi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.NEREDENGELDI], "=", tf.neredenGeldi));

            if (!string.IsNullOrEmpty(tf.nereyeGitti))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.NEREYEGITTI], "=", tf.nereyeGitti));

            if (!string.IsNullOrEmpty(tf.kimeGitti))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.KIMEGITTI], "=", tf.kimeGitti));

            if (!string.IsNullOrEmpty(tfKriter.hesapKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIFDETAY, ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.HESAPPLANKOD], "LIKE", tfKriter.hesapKodu.Replace(".", "") + "%"));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADTIF);
            if (orluYetkiler != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", orluYetkiler);

            sql += kosul;

            sql += @"  group by TASIL.KOD, TASIL.AD, substring(TASISLEMDETAY.HESAPPLANKOD,0,4) ";
            //                       ORDER BY TASIL.KOD, TASIL.AD, substring(TASISLEMDETAY.HESAPPLANKOD,0,4)";

            sql += @"union
SELECT '-1', '-1',
 substring(TASISLEMDETAY.HESAPPLANKOD,0,4) hesapplankod
, sum(TASISLEMDETAY.MIKTAR) miktar
, sum(TASISLEMDETAY.MIKTAR*TASISLEMDETAY.BIRIMFIYATKDVLI) tutar
FROM TASISLEMFORM, TASISLEMDETAY, TASHESAPPLAN, TASISLEMTIP, TASMUHASEBE
WHERE TASISLEMFORM.YIL=TASISLEMDETAY.YIL 
AND TASISLEMFORM.MUHASEBEKOD=TASISLEMDETAY.MUHASEBEKOD 
AND TASISLEMFORM.HARCAMABIRIMKOD=TASISLEMDETAY.HARCAMABIRIMKOD 
AND TASISLEMFORM.FISNO=TASISLEMDETAY.FISNO 
AND TASHESAPPLAN.KOD=TASISLEMDETAY.HESAPPLANKOD 
AND TASISLEMTIP.KOD=TASISLEMFORM.ISLEMTIPIKOD 
and TASMUHASEBE.KOD = TASISLEMFORM.MUHASEBEKOD ";

            sql += kosul;

            sql += @" group by  substring(TASISLEMDETAY.HESAPPLANKOD,0,4)
                      ORDER BY  1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Formu, Taşınır Form Kriteri ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Malzeme Tarihçe bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="tfKriter">Taşınır Form Kriter bilgisini tutan, TasinirFormKriter türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Malzeme Tarihçe bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader BirimItibariylaTasinirHurdaRaporu(TasinirIslemForm tf, TasinirFormKriter tfKriter, string[] yetkiDeger)
        {
            string kosul = " ";
            string sql = @"SELECT substring(TASISLEMFORM.HARCAMABIRIMKOD,0,9)
, substring(TASISLEMDETAY.HESAPPLANKOD,0,4) hesapplankod
, sum(TASISLEMDETAY.MIKTAR) miktar
, sum(TASISLEMDETAY.MIKTAR*TASISLEMDETAY.BIRIMFIYATKDVLI) tutar
FROM TASISLEMFORM, TASISLEMDETAY, TASHESAPPLAN, TASISLEMTIP, TASMUHASEBE, TASHARCAMABIRIM
WHERE TASISLEMFORM.YIL=TASISLEMDETAY.YIL AND TASISLEMFORM.MUHASEBEKOD=TASISLEMDETAY.MUHASEBEKOD 
AND TASISLEMFORM.HARCAMABIRIMKOD=TASISLEMDETAY.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO=TASISLEMDETAY.FISNO 
AND TASHESAPPLAN.KOD=TASISLEMDETAY.HESAPPLANKOD AND TASISLEMTIP.KOD=TASISLEMFORM.ISLEMTIPIKOD 
and TASMUHASEBE.KOD = TASISLEMFORM.MUHASEBEKOD
and TASHARCAMABIRIM.KOD = TASISLEMFORM.HARCAMABIRIMKOD AND TASISLEMFORM.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD
and TASHARCAMABIRIM.KAPALI=0 ";

            if (tf.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.YIL], "=", tf.yil));

            if (!string.IsNullOrEmpty(tf.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.MUHASEBEKOD], "LIKE", tf.muhasebeKod));

            if (!string.IsNullOrEmpty(tf.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.HARCAMABIRIMKOD], "LIKE", tf.harcamaKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tf.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.AMBARKOD], "=", tf.ambarKod));

            if (!tfKriter.belgeTarihBasla.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], ">=", tfKriter.belgeTarihBasla));

            if (!tfKriter.belgeTarihBit.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], "<=", tfKriter.belgeTarihBit));

            if (tf.islemTipKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.ISLEMTIPKOD], "=", tf.islemTipKod));
            if (tfKriter.islemTipleri != "")
            {
                string[] islemTipleri = tfKriter.islemTipleri.Split(';');
                string islemTipKosul = "";
                for (int i = 0; i < islemTipleri.Length; i++)
                    islemTipKosul = VeriErisim.KosulEkle(islemTipKosul, "OR", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.ISLEMTIPKOD], "=", OrtakFonksiyonlar.ConvertToInt(islemTipleri[i], 0)));

                if (islemTipKosul != "")
                    kosul += " AND (" + islemTipKosul + ")";
            }

            if (!string.IsNullOrEmpty(tf.gMuhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GMUHASEBEKOD], "=", tf.gMuhasebeKod));

            if (!string.IsNullOrEmpty(tf.gHarcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GHARCAMAKOD], "=", tf.gHarcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(tf.gAmbarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GAMBARKOD], "=", tf.gAmbarKod));

            if (!string.IsNullOrEmpty(tf.neredenGeldi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.NEREDENGELDI], "=", tf.neredenGeldi));

            if (!string.IsNullOrEmpty(tf.nereyeGitti))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.NEREYEGITTI], "=", tf.nereyeGitti));

            if (!string.IsNullOrEmpty(tf.kimeGitti))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.KIMEGITTI], "=", tf.kimeGitti));

            if (!string.IsNullOrEmpty(tfKriter.hesapKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIFDETAY, ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.HESAPPLANKOD], "LIKE", tfKriter.hesapKodu.Replace(".", "") + "%"));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADTIF);
            if (orluYetkiler != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", orluYetkiler);

            sql += kosul;

            sql += @"  group by 
substring(TASISLEMFORM.HARCAMABIRIMKOD,0,9),
substring(TASISLEMDETAY.HESAPPLANKOD,0,4) ";
            //                       ORDER BY TASIL.KOD, TASIL.AD, substring(TASISLEMDETAY.HESAPPLANKOD,0,4)";

            sql += @" union SELECT substring(TASISLEMFORM.HARCAMABIRIMKOD,0,7) birim
, substring(TASISLEMDETAY.HESAPPLANKOD,0,4) hesapplankod
, sum(TASISLEMDETAY.MIKTAR) miktar
, sum(TASISLEMDETAY.MIKTAR*TASISLEMDETAY.BIRIMFIYATKDVLI) tutar
FROM TASISLEMFORM, TASISLEMDETAY, TASHESAPPLAN, TASISLEMTIP, TASMUHASEBE, TASHARCAMABIRIM
WHERE TASISLEMFORM.YIL=TASISLEMDETAY.YIL AND TASISLEMFORM.MUHASEBEKOD=TASISLEMDETAY.MUHASEBEKOD 
AND TASISLEMFORM.HARCAMABIRIMKOD=TASISLEMDETAY.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO=TASISLEMDETAY.FISNO 
AND TASHESAPPLAN.KOD=TASISLEMDETAY.HESAPPLANKOD AND TASISLEMTIP.KOD=TASISLEMFORM.ISLEMTIPIKOD 
and TASMUHASEBE.KOD = TASISLEMFORM.MUHASEBEKOD
and TASHARCAMABIRIM.KOD = TASISLEMFORM.HARCAMABIRIMKOD AND TASISLEMFORM.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD
and TASHARCAMABIRIM.KAPALI=0 ";

            sql += kosul;

            sql += @"  group by 
substring(TASISLEMFORM.HARCAMABIRIMKOD,0,7),
substring(TASISLEMDETAY.HESAPPLANKOD,0,4) ";

            sql += @" union
SELECT '-1',
 substring(TASISLEMDETAY.HESAPPLANKOD,0,4) hesapplankod
, sum(TASISLEMDETAY.MIKTAR) miktar
, sum(TASISLEMDETAY.MIKTAR*TASISLEMDETAY.BIRIMFIYATKDVLI) tutar
FROM TASISLEMFORM, TASISLEMDETAY, TASHESAPPLAN, TASISLEMTIP, TASMUHASEBE
WHERE TASISLEMFORM.YIL=TASISLEMDETAY.YIL 
AND TASISLEMFORM.MUHASEBEKOD=TASISLEMDETAY.MUHASEBEKOD 
AND TASISLEMFORM.HARCAMABIRIMKOD=TASISLEMDETAY.HARCAMABIRIMKOD 
AND TASISLEMFORM.FISNO=TASISLEMDETAY.FISNO 
AND TASHESAPPLAN.KOD=TASISLEMDETAY.HESAPPLANKOD 
AND TASISLEMTIP.KOD=TASISLEMFORM.ISLEMTIPIKOD 
and TASMUHASEBE.KOD = TASISLEMFORM.MUHASEBEKOD ";
            sql += kosul;

            sql += @" group by  substring(TASISLEMDETAY.HESAPPLANKOD,0,4)
                      ORDER BY  1";

            return veriErisim.DataReaderDondur(sql);
        }


        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Formu, Taşınır Form Kriteri ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Malzeme Tarihçe bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="tfKriter">Taşınır Form Kriter bilgisini tutan, TasinirFormKriter türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Malzeme Tarihçe bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader BakanlikDuzeyiTasinirHurdaRaporu(TasinirIslemForm tf, TasinirFormKriter tfKriter, string[] yetkiDeger)
        {
            string kosul = " ";
            string sql = @"SELECT substring(TASISLEMDETAY.HESAPPLANKOD,0,8) hesapplankod
                           ,0
                           , sum(TASISLEMDETAY.MIKTAR) miktar
                           , sum(TASISLEMDETAY.MIKTAR*TASISLEMDETAY.BIRIMFIYATKDVLI) tutar
                           FROM TASISLEMFORM, TASISLEMDETAY, TASHESAPPLAN, TASISLEMTIP, TASMUHASEBE
                           WHERE TASISLEMFORM.YIL=TASISLEMDETAY.YIL AND TASISLEMFORM.MUHASEBEKOD=TASISLEMDETAY.MUHASEBEKOD 
                           AND TASISLEMFORM.HARCAMABIRIMKOD=TASISLEMDETAY.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO=TASISLEMDETAY.FISNO 
                           AND TASHESAPPLAN.KOD=TASISLEMDETAY.HESAPPLANKOD AND TASISLEMTIP.KOD=TASISLEMFORM.ISLEMTIPIKOD 
                           and TASMUHASEBE.KOD = TASISLEMFORM.MUHASEBEKOD ";

            if (tf.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.YIL], "=", tf.yil));

            if (!string.IsNullOrEmpty(tf.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.MUHASEBEKOD], "LIKE", tf.muhasebeKod));

            if (!string.IsNullOrEmpty(tf.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.HARCAMABIRIMKOD], "LIKE", tf.harcamaKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tf.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.AMBARKOD], "=", tf.ambarKod));

            if (!tfKriter.belgeTarihBasla.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], ">=", tfKriter.belgeTarihBasla));

            if (!tfKriter.belgeTarihBit.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.FISTARIH], "<=", tfKriter.belgeTarihBit));

            if (tf.islemTipKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.ISLEMTIPKOD], "=", tf.islemTipKod));
            if (tfKriter.islemTipleri != "")
            {
                string[] islemTipleri = tfKriter.islemTipleri.Split(';');
                string islemTipKosul = "";
                for (int i = 0; i < islemTipleri.Length; i++)
                    islemTipKosul = VeriErisim.KosulEkle(islemTipKosul, "OR", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.ISLEMTIPKOD], "=", OrtakFonksiyonlar.ConvertToInt(islemTipleri[i], 0)));

                if (islemTipKosul != "")
                    kosul += " AND (" + islemTipKosul + ")";
            }

            if (!string.IsNullOrEmpty(tf.gMuhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GMUHASEBEKOD], "=", tf.gMuhasebeKod));

            if (!string.IsNullOrEmpty(tf.gHarcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GHARCAMAKOD], "=", tf.gHarcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(tf.gAmbarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.GAMBARKOD], "=", tf.gAmbarKod));

            if (!string.IsNullOrEmpty(tf.neredenGeldi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.NEREDENGELDI], "=", tf.neredenGeldi));

            if (!string.IsNullOrEmpty(tf.nereyeGitti))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.NEREYEGITTI], "=", tf.nereyeGitti));

            if (!string.IsNullOrEmpty(tf.kimeGitti))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.KIMEGITTI], "=", tf.kimeGitti));

            if (!string.IsNullOrEmpty(tfKriter.hesapKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIFDETAY, ALANLARTIFDETAY[(int)ENUMAlanlarTIFDETAY.HESAPPLANKOD], "LIKE", tfKriter.hesapKodu.Replace(".", "") + "%"));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTIF, ALANLARTIF[(int)ENUMAlanlarTIF.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADTIF);
            if (orluYetkiler != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", orluYetkiler);

            sql += kosul;

            sql += @"  group by substring(TASISLEMDETAY.HESAPPLANKOD,0,8)";

            sql += @" UNION 
                        SELECT 
                        substring(TASISLEMDETAY.HESAPPLANKOD,0,4) hesapplankod
                        , 1
                        , sum(TASISLEMDETAY.MIKTAR) miktar
                        , sum(TASISLEMDETAY.MIKTAR*TASISLEMDETAY.BIRIMFIYATKDVLI) tutar
                            FROM TASISLEMFORM, TASISLEMDETAY, TASHESAPPLAN, TASISLEMTIP, TASMUHASEBE
                            WHERE 
                        TASISLEMFORM.YIL=TASISLEMDETAY.YIL AND TASISLEMFORM.MUHASEBEKOD=TASISLEMDETAY.MUHASEBEKOD
                            AND TASISLEMFORM.HARCAMABIRIMKOD=TASISLEMDETAY.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO=TASISLEMDETAY.FISNO 
                        AND TASHESAPPLAN.KOD=TASISLEMDETAY.HESAPPLANKOD AND TASISLEMTIP.KOD=TASISLEMFORM.ISLEMTIPIKOD 
                        and TASMUHASEBE.KOD = TASISLEMFORM.MUHASEBEKOD 
                            AND TASISLEMFORM.YIL=TASISLEMDETAY.YIL ";

            sql += kosul;

            sql += @"  group by substring(TASISLEMDETAY.HESAPPLANKOD,0,4)
                       order by 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Formu, Taşınır Form Kriteri ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Malzeme Tarihçe bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Malzeme Tarihçe bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader HesapPlaniBazindaYillikTuketimRaporu(DepoDurum tf)
        {
            string kosul = " ";
            string hesapPlanKod = tf.hesapPlanKod.Replace(".", "");
            string grupla = @"  GROUP BY TASSTOKHAREKETVIEW.YIL,
TASSTOKHAREKETVIEW.MUHASEBEKOD, TASSTOKHAREKETVIEW.MUHASEBEAD, 
TASSTOKHAREKETVIEW.HARCAMABIRIMKOD, TASSTOKHAREKETVIEW.HARCAMABIRIMAD, " +
" substring(TASSTOKHAREKETVIEW.HESAPPLANKOD,0," + (hesapPlanKod.Length - 1).ToString() + ")";

            string sql = @"select TASSTOKHAREKETVIEW.YIL,1,
TASSTOKHAREKETVIEW.MUHASEBEKOD, TASSTOKHAREKETVIEW.MUHASEBEAD, 
TASSTOKHAREKETVIEW.HARCAMABIRIMKOD, TASSTOKHAREKETVIEW.HARCAMABIRIMAD, " +
" substring(TASSTOKHAREKETVIEW.HESAPPLANKOD,0," + (hesapPlanKod.Length - 1).ToString() +
@"), SUM(TASSTOKHAREKETVIEW.MIKTAR), 
SUM(TASSTOKHAREKETVIEW.MIKTAR*TASSTOKHAREKETVIEW.BIRIMFIYATKDVLI) 
from TASSTOKHAREKETVIEW
INNER JOIN TASMUHASEBE ON TASSTOKHAREKETVIEW.MUHASEBEKOD=TASMUHASEBE.KOD 
where TASSTOKHAREKETVIEW.YIL  >= YEAR(getdate())-3
and TASSTOKHAREKETVIEW.ISLEMTIPITUR<50 ";

            if (!string.IsNullOrEmpty(tf.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "LIKE", tf.muhasebeKod));

            if (!string.IsNullOrEmpty(tf.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "LIKE", tf.harcamaKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tf.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", hesapPlanKod + "%"));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            //string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADSTOKHAREKETVIEW);
            //if (orluYetkiler != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", orluYetkiler);

            sql += kosul + grupla;

            sql += @" UNION 
                        select TASSTOKHAREKETVIEW.YIL,2,
TASSTOKHAREKETVIEW.MUHASEBEKOD, TASSTOKHAREKETVIEW.MUHASEBEAD, 
TASSTOKHAREKETVIEW.HARCAMABIRIMKOD, TASSTOKHAREKETVIEW.HARCAMABIRIMAD, " +
" substring(TASSTOKHAREKETVIEW.HESAPPLANKOD,0," + (hesapPlanKod.Length - 1).ToString() +
@"), SUM(TASSTOKHAREKETVIEW.MIKTAR), 
SUM(TASSTOKHAREKETVIEW.MIKTAR*TASSTOKHAREKETVIEW.BIRIMFIYATKDVLI) 
from TASSTOKHAREKETVIEW
INNER JOIN TASMUHASEBE ON TASSTOKHAREKETVIEW.MUHASEBEKOD=TASMUHASEBE.KOD 
where TASSTOKHAREKETVIEW.YIL  >= YEAR(getdate())-3
and TASSTOKHAREKETVIEW.ISLEMTIPITUR>=50 and TASSTOKHAREKETVIEW.ISLEMTIPIKOD<>18 ";

            sql += kosul + grupla;

            sql += " order by 1,2,3,4,5,6";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Formu, Taşınır Form Kriteri ve Yetki değerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Malzeme Tarihçe bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>       
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Malzeme Tarihçe bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader ToplamYillikTuketimRaporu(DepoDurum tf)
        {
            string kosul = " ";
            string grupla = @"  GROUP BY TASSTOKHAREKETVIEW.YIL,
TASSTOKHAREKETVIEW.MUHASEBEKOD, TASSTOKHAREKETVIEW.MUHASEBEAD 
,TASSTOKHAREKETVIEW.HARCAMABIRIMKOD, TASSTOKHAREKETVIEW.HARCAMABIRIMAD ";

            string sql = @"select TASSTOKHAREKETVIEW.YIL,1,
TASSTOKHAREKETVIEW.MUHASEBEKOD, TASSTOKHAREKETVIEW.MUHASEBEAD, 
TASSTOKHAREKETVIEW.HARCAMABIRIMKOD, TASSTOKHAREKETVIEW.HARCAMABIRIMAD, 
SUM(TASSTOKHAREKETVIEW.MIKTAR), 
SUM(TASSTOKHAREKETVIEW.MIKTAR*TASSTOKHAREKETVIEW.BIRIMFIYATKDVLI) 
from TASSTOKHAREKETVIEW
INNER JOIN TASMUHASEBE ON TASSTOKHAREKETVIEW.MUHASEBEKOD=TASMUHASEBE.KOD 
where TASSTOKHAREKETVIEW.YIL  >= YEAR(getdate())-3
and TASSTOKHAREKETVIEW.ISLEMTIPIKOD=15 ";

            if (!string.IsNullOrEmpty(tf.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.MUHASEBEKOD], "LIKE", tf.muhasebeKod));

            if (!string.IsNullOrEmpty(tf.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HARCAMABIRIMKOD], "LIKE", tf.harcamaKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tf.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.HESAPPLANKOD], "LIKE", tf.hesapPlanKod.Replace(".", "") + "%"));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSTOKHAREKETVIEW, ALANLARSTOKHAREKETVIEW[(int)ENUMAlanlarSTOKHAREKETVIEW.DURUM], "=", (int)ENUMBelgeDurumu.ONAYLI));

            //string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOADSTOKHAREKETVIEW);
            //if (orluYetkiler != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", orluYetkiler);

            sql += kosul + grupla;

            sql += @" UNION 
                        select TASSTOKHAREKETVIEW.YIL,2,
TASSTOKHAREKETVIEW.MUHASEBEKOD, TASSTOKHAREKETVIEW.MUHASEBEAD, 
TASSTOKHAREKETVIEW.HARCAMABIRIMKOD, TASSTOKHAREKETVIEW.HARCAMABIRIMAD, 
SUM(TASSTOKHAREKETVIEW.MIKTAR), 
SUM(TASSTOKHAREKETVIEW.MIKTAR*TASSTOKHAREKETVIEW.BIRIMFIYATKDVLI) 
from TASSTOKHAREKETVIEW
INNER JOIN TASMUHASEBE ON TASSTOKHAREKETVIEW.MUHASEBEKOD=TASMUHASEBE.KOD 
where TASSTOKHAREKETVIEW.YIL  >= YEAR(getdate())-3
and TASSTOKHAREKETVIEW.ISLEMTIPITUR<50 
and TASSTOKHAREKETVIEW.ISLEMTIPIKOD<>15 ";

            sql += kosul + grupla;

            sql += @" UNION 
                        select TASSTOKHAREKETVIEW.YIL,3,
TASSTOKHAREKETVIEW.MUHASEBEKOD, TASSTOKHAREKETVIEW.MUHASEBEAD, 
TASSTOKHAREKETVIEW.HARCAMABIRIMKOD, TASSTOKHAREKETVIEW.HARCAMABIRIMAD, 
SUM(TASSTOKHAREKETVIEW.MIKTAR), 
SUM(TASSTOKHAREKETVIEW.MIKTAR*TASSTOKHAREKETVIEW.BIRIMFIYATKDVLI) 
from TASSTOKHAREKETVIEW
INNER JOIN TASMUHASEBE ON TASSTOKHAREKETVIEW.MUHASEBEKOD=TASMUHASEBE.KOD 
where TASSTOKHAREKETVIEW.YIL  >= YEAR(getdate())-3
and TASSTOKHAREKETVIEW.ISLEMTIPITUR<50 ";

            sql += kosul + grupla;

            sql += @" UNION 
                        select TASSTOKHAREKETVIEW.YIL,4,
TASSTOKHAREKETVIEW.MUHASEBEKOD, TASSTOKHAREKETVIEW.MUHASEBEAD, 
TASSTOKHAREKETVIEW.HARCAMABIRIMKOD, TASSTOKHAREKETVIEW.HARCAMABIRIMAD, 
SUM(TASSTOKHAREKETVIEW.MIKTAR), 
SUM(TASSTOKHAREKETVIEW.MIKTAR*TASSTOKHAREKETVIEW.BIRIMFIYATKDVLI) 
from TASSTOKHAREKETVIEW
INNER JOIN TASMUHASEBE ON TASSTOKHAREKETVIEW.MUHASEBEKOD=TASMUHASEBE.KOD 
where TASSTOKHAREKETVIEW.YIL  >= YEAR(getdate())-3
and TASSTOKHAREKETVIEW.ISLEMTIPITUR>=50 and TASSTOKHAREKETVIEW.ISLEMTIPIKOD<>18 ";

            sql += kosul + grupla;

            sql += " order by 1,2,3,4,5";

            return veriErisim.DataReaderDondur(sql);
        }


        public IDataReader BirimlereGoreDemirbasRaporu(TasinirIslemDetay form)
        {
            string kosul = "";
            string sql = @"SELECT HARCAMABIRIMKOD, HARCAMABIRIMAD, HESAPPLANKOD, HESAPPLANAD, COUNT(*) AS ADET , SUM(KDVLIBIRIMFIYAT) AS TUTAR FROM TASDEMIRBASVIEW ";

            if (!string.IsNullOrEmpty(form.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD", "=", form.muhasebeKod));

            if (!string.IsNullOrEmpty(form.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "HARCAMABIRIMKOD", "=", form.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(form.hesapPlanKod))
            {
                string ekKosul = "";
                string[] kodlar = form.hesapPlanKod.Split(';');
                foreach (var hesapPlanKod in kodlar)
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("", "HESAPPLANKOD", "LIKE", hesapPlanKod + "%"));

                kosul += " AND (" + ekKosul + ")";
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY HARCAMABIRIMKOD, HARCAMABIRIMAD, HESAPPLANKOD, HESAPPLANAD";
            sql += " ORDER BY HARCAMABIRIMKOD, HESAPPLANKOD, HESAPPLANAD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KontrolRaporu01(KontrolRaporu form)
        {
            string kosul = "";
            string sql = @"
                    SELECT H1.PRSICILNO, D1.BIRIMFIYATKDVLI AS KDVLIBIRIMFIYAT_TIF, H1.KDVLIBIRIMFIYAT AS KDVLIBIRIMFIYAT_ILK, H2.KDVLIBIRIMFIYAT AS KDVLIBIRIMFIYAT_SON, 
                    H1.YIL, H1.MUHASEBEKOD,TASMUHASEBE.AD AS MUHASEBEAD, H1.HARCAMABIRIMKOD,TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, H1.FISNO, H1.SICILNO, TASHESAPPLAN.ACIKLAMA AS SICILNOAD FROM TASSICILNOHAREKET AS H1 
                    INNER JOIN TASSICILNOHAREKET AS H2 ON H1.PRSICILNO = H2.PRSICILNO AND H2.SIRANO = (SELECT MAX(B.SIRANO) FROM TASSICILNOHAREKET B WHERE H2.PRSICILNO = B.PRSICILNO AND B.ISLEMTURUKOD < 50)
                    INNER JOIN (SELECT H3.PRSICILNO FROM TASSICILNOHAREKET AS H3 WHERE (SELECT 1 FROM TASSICILNOHAREKET B WHERE H3.PRSICILNO = B.PRSICILNO AND (B.ISLEMTURUKOD = -1 OR B.ISLEMTURUKOD = -2)) IS NULL GROUP BY H3.PRSICILNO) H3 ON H1.PRSICILNO = H3.PRSICILNO

                    LEFT OUTER JOIN TASISLEMDETAY AS D1 ON D1.YIL = H1.YIL AND D1.MUHASEBEKOD = H1.MUHASEBEKOD AND D1.HARCAMABIRIMKOD = H1.HARCAMABIRIMKOD AND D1.FISNO = H1.FISNO AND D1.SIRANO = H1.FISSIRANO AND D1.HESAPPLANKOD = H1.HESAPPLANKOD
                    LEFT OUTER JOIN TASISLEMFORM AS F1 ON F1.YIL = D1.YIL AND F1.MUHASEBEKOD = D1.MUHASEBEKOD AND F1.HARCAMABIRIMKOD = D1.HARCAMABIRIMKOD AND F1.FISNO = D1.FISNO AND F1.DURUM <> 9
                    LEFT OUTER JOIN TASMUHASEBE ON TASMUHASEBE.KOD = H1.MUHASEBEKOD
                    LEFT OUTER JOIN TASHARCAMABIRIM ON TASHARCAMABIRIM.MUHASEBEKOD = H1.MUHASEBEKOD AND TASHARCAMABIRIM.KOD = H1.HARCAMABIRIMKOD
                    LEFT OUTER JOIN TASHESAPPLAN ON TASHESAPPLAN.KOD = H1.HESAPPLANKOD 

                    WHERE  H1.SIRANO = 1 AND H1.DURUM = 1
                    AND (H1.KDVLIBIRIMFIYAT <> H2.KDVLIBIRIMFIYAT OR D1.BIRIMFIYATKDVLI <> H2.KDVLIBIRIMFIYAT)
                    ";

            sql += " ORDER BY H1.PRSICILNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KontrolRaporu02(KontrolRaporu form)
        {
            string kosul = "";
            string sql = @"
                    SELECT H1.PRSICILNO, D1.BIRIMFIYATKDVLI AS KDVLIBIRIMFIYAT_TIF, H1.KDVLIBIRIMFIYAT AS KDVLIBIRIMFIYAT_ILK, H2.KDVLIBIRIMFIYAT AS KDVLIBIRIMFIYAT_SON, 
                    H1.YIL, H1.MUHASEBEKOD,TASMUHASEBE.AD AS MUHASEBEAD, H1.HARCAMABIRIMKOD,TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, H1.FISNO, H1.SICILNO, TASHESAPPLAN.ACIKLAMA AS SICILNOAD FROM TASSICILNOHAREKET AS H1 
                    INNER JOIN TASSICILNOHAREKET AS H2 ON H1.PRSICILNO = H2.PRSICILNO AND H2.SIRANO = (SELECT MAX(B.SIRANO) FROM TASSICILNOHAREKET B WHERE H2.PRSICILNO = B.PRSICILNO AND B.ISLEMTURUKOD < 50)
                    INNER JOIN (SELECT H3.PRSICILNO FROM TASSICILNOHAREKET AS H3 WHERE NOT (SELECT 1 FROM TASSICILNOHAREKET B WHERE H3.PRSICILNO = B.PRSICILNO AND (B.ISLEMTURUKOD = -1 OR B.ISLEMTURUKOD = -2)) IS NULL GROUP BY H3.PRSICILNO) H3 ON H1.PRSICILNO = H3.PRSICILNO

                    LEFT OUTER JOIN TASISLEMDETAY AS D1 ON D1.YIL = H1.YIL AND D1.MUHASEBEKOD = H1.MUHASEBEKOD AND D1.HARCAMABIRIMKOD = H1.HARCAMABIRIMKOD AND D1.FISNO = H1.FISNO AND D1.SIRANO = H1.FISSIRANO AND D1.HESAPPLANKOD = H1.HESAPPLANKOD
                    LEFT OUTER JOIN TASISLEMFORM AS F1 ON F1.YIL = D1.YIL AND F1.MUHASEBEKOD = D1.MUHASEBEKOD AND F1.HARCAMABIRIMKOD = D1.HARCAMABIRIMKOD AND F1.FISNO = D1.FISNO AND F1.DURUM <> 9
                    LEFT OUTER JOIN TASMUHASEBE ON TASMUHASEBE.KOD = H1.MUHASEBEKOD
                    LEFT OUTER JOIN TASHARCAMABIRIM ON TASHARCAMABIRIM.MUHASEBEKOD = H1.MUHASEBEKOD AND TASHARCAMABIRIM.KOD = H1.HARCAMABIRIMKOD
                    LEFT OUTER JOIN TASHESAPPLAN ON TASHESAPPLAN.KOD = H1.HESAPPLANKOD 

                    WHERE  H1.SIRANO = 1 AND H1.DURUM = 1
                    AND (H1.KDVLIBIRIMFIYAT <> H2.KDVLIBIRIMFIYAT OR D1.BIRIMFIYATKDVLI <> H2.KDVLIBIRIMFIYAT)
                    ";

            sql += " ORDER BY H1.PRSICILNO";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
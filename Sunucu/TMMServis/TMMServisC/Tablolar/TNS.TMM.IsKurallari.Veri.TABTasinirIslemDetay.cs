using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır İşlem Detay bilgilerini tutan TASISLEMDETAY veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABTasinirIslemDetay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTasinirIslemDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASISLEMDETAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
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
            /// Kdv oranı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 8,
            /// <summary>
            /// Birim fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 9,
            /// <summary>
            /// Birim fiyat kdvli alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
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
            SONTARIH = 15,

            YERLESKEYERI = 16,
            SATISBEDELI = 17
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "SIRANO", "HESAPPLANKOD", "SICILNO", "MIKTAR", "KDVORAN", "BIRIMFIYAT", "BIRIMFIYATKDVLI", "ESICILNO", "EALIMTARIHI", "ETEDARIKSEKLI", "EACIKLAMA", "SONTARIH", "YERLESKEYERI", "SATISBEDELI" };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADOZELLIK = "TASISLEMOZELLIK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarOzellik
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
            /// Marka kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MARKAKOD = 5,
            /// <summary>
            /// Model kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODELKOD = 6,
            /// <summary>
            /// Şase no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SASENO = 7,
            /// <summary>
            /// Motor no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MOTORNO = 8,
            /// <summary>
            /// Plaka alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PLAKA = 9,
            /// <summary>
            /// Cilt no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CILTNO = 10,
            /// <summary>
            /// Dil alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DIL = 11,
            /// <summary>
            /// Yazar adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAZARADI = 12,
            /// <summary>
            /// Adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADI = 13,
            /// <summary>
            /// Yayın yeri alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAYINYERI = 14,
            /// <summary>
            /// Yayın tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAYINTARIHI = 15,
            /// <summary>
            /// Nereden geldi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDENGELDI = 16,
            /// <summary>
            /// Boyutları alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BOYUTLARI = 17,
            /// <summary>
            /// Satır sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATIRSAYISI = 18,
            /// <summary>
            /// Yaprak sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAPRAKSAYISI = 19,
            /// <summary>
            /// Sayfa sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYFASAYISI = 20,
            /// <summary>
            /// Cilt türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CILTTURU = 21,
            /// <summary>
            /// Çeşidi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CESIDI = 22,
            /// <summary>
            /// Yeri konusu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YERIKONUSU = 23,
            /// <summary>
            /// Geliş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GELISTARIHI = 24,
            /// <summary>
            /// Nerede bulundu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDEBULUNDU = 25,
            /// <summary>
            /// Çağı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CAGI = 26,
            /// <summary>
            /// Durum maddesi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUMUMADDESI = 27,
            /// <summary>
            /// Ön yüz alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONYUZ = 28,
            /// <summary>
            /// Arka yüz alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ARKAYUZ = 29,
            /// <summary>
            /// Fotoğraf alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FOTOGRAF = 30,
            /// <summary>
            /// Puan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PUAN = 31,
            /// <summary>
            /// Dış sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DISSICILNO = 32,

            DISSICILNO2 = 33,
            BISNO = 34,
            ESKIBISNO1 = 35,
            ESKIBISNO2 = 36,
            RFIDETIKETTURKOD = 37,
            GARANTIBITISTARIHI = 38,
            GIAI = 39,

            EKNO = 40,
            HESAPTARIHI = 41,
            BUTCEKODU = 42,
            BLOKAJKODU = 43,
            GELDIGISUBE = 44,

            AMORTISMANYIL = 45,
            AMORTISMANBITTI = 46,
            BULUNDUGUYER = 47,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAROZELLIK = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "SIRANO", "MARKAKOD", "MODELKOD", "SASENO", "MOTORNO", "PLAKA", "CILTNO", "DIL", "YAZARADI", "ADI", "YAYINYERI", "YAYINTARIHI", "NEREDENGELDI", "BOYUTLARI", "SATIRSAYISI", "YAPRAKSAYISI", "SAYFASAYISI", "CILTTURU", "CESIDI", "YERIKONUSU", "GELISTARIHI", "NEREDEBULUNDU", "CAGI", "DURUMUMADDESI", "ONYUZ", "ARKAYUZ", "FOTOGRAF", "PUAN",
            "DISSICILNO", "DISSICILNO2", "BISNO", "ESKIBISNO1", "ESKIBISNO2", "RFIDETIKETTURKOD", "GARANTIBITISTARIHI", "GIAI", "EKNO", "HESAPTARIHI", "BUTCEKODU", "BLOKAJKODU", "GELDIGISUBE", "AMORTISMANYIL", "AMORTISMANBITTI", "BULUNDUGUYER" };

        /// <summary>
        /// Taşınır İşlem Detay bilgisini veri tabanına kaydeden,
        /// sql cümlesini oluşturup çalıştıran, ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim bilgisini tutan, VeriErisim türünde nesne</param>
        /// <param name="td">Taşınır İşlem Detay bilgisini tutan, TasinirIslemDetay türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(VeriErisim ve, TasinirIslemDetay td)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(td.yil) + ",";
            sql += VeriErisim.DegerYap(td.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(td.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(td.fisNo) + ",";
            sql += VeriErisim.DegerYap(td.siraNo) + ",";
            sql += VeriErisim.DegerYap(td.hesapPlanKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(td.sicilNo) + ",";
            sql += VeriErisim.DegerYap(td.miktar) + ",";
            sql += VeriErisim.DegerYap(td.kdvOran) + ",";
            sql += VeriErisim.DegerYap(td.birimFiyat) + ",";
            sql += VeriErisim.DegerYap(td.birimFiyatKDVLi) + ",";
            sql += VeriErisim.DegerYap(td.eSicilNo) + ",";
            sql += VeriErisim.DegerYap(td.eAlimTarihi) + ",";
            sql += VeriErisim.DegerYap(td.eTedarikSekli) + ",";
            sql += VeriErisim.DegerYap(td.eAciklama) + ",";
            sql += VeriErisim.DegerYap(td.sonTarih) + ",";
            sql += VeriErisim.DegerYap(td.yerleskeYeri) + ",";
            sql += VeriErisim.DegerYap(td.satisBedeli) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Taşınır İşlem Form özellik bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim bilgisini tutan, VeriErisim türünde nesne</param>
        /// <param name="td">Taşınır İşlem Detay bilgisini tutan, TasinirIslemDetay türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc OzellikKaydet(VeriErisim ve, TasinirIslemDetay td)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOADOZELLIK + " (" + VeriErisim.TumAlanlar("", ALANLAROZELLIK) + ") VALUES (";
            sql += VeriErisim.DegerYap(td.yil) + ",";
            sql += VeriErisim.DegerYap(td.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(td.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(td.fisNo) + ",";
            sql += VeriErisim.DegerYap(td.siraNo) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.markaKod) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.modelKod) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.saseNo) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.motorNo) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.plaka) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.ciltNo) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.dil) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.yazarAdi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.adi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.yayinYeri) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.yayinTarihi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.neredenGeldi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.boyutlari) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.satirSayisi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.yaprakSayisi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.sayfaSayisi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.ciltTuru) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.cesidi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.yeriKonusu) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.gelisTarihi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.neredeBulundu) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.cagi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.durumuMaddesi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.onYuz) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.arkaYuz) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.fotograf) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.puan) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.disSicilNo) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.disSicilNo2) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.bisNo) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.eskiBisNo1) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.eskiBisNo2) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.rfidEtiketTurKod) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.garantiBitisTarihi) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.giai) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.ekNo) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.hesapTarih) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.butceKodu) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.blokajKodu) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.geldigiSube) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.amortismanYil) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.amortismanBitti) + ",";
            sql += VeriErisim.DegerYap(td.ozellik.bulunduguYer) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Paremetre olarak verilen Taşınır İşlem Form bilgilerini kullanarak 
        /// İlgili Taşınır İşlem Detay bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();

            string sql = "DELETE FROM " + TABLOAD;

            //Fişe ait bütün satırlar
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));

            sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOADOZELLIK;

                //Özelliğe ait bütün satırlar
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROZELLIK[(int)ENUMAlanlar.YIL], "=", tf.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROZELLIK[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROZELLIK[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROZELLIK[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));

                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Form Detay bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="siraNoSirali">Sıra numarasına göre sıralamanın yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Taşınır İşlem Detay bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(TasinirIslemForm tf, bool siraNoSirali, int sayfaNo, int kayitSayisi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADOZELLIK, ALANLAROZELLIK) + ",TASHESAPPLAN.ACIKLAMA AS TASHESAPPLANACIKLAMA, TASOLCUBIRIM.AD AS TASOLCUBIRIMAD, TASMARKA.AD AS TASMARKAAD, TASMODEL.AD AS TASMODELAD, TASRFIDETIKETTURU.AD AS TASRFIDETIKETTURUAD, TASODA.AD AS YERLESKEAD FROM ";
            sql = sql.Replace("TASISLEMDETAY.YIL", "TASISLEMDETAY.YIL AS YIL1");
            sql = sql.Replace("TASISLEMDETAY.MUHASEBEKOD", "TASISLEMDETAY.MUHASEBEKOD AS MUHASEBEKOD1");
            sql = sql.Replace("TASISLEMDETAY.HARCAMABIRIMKOD", "TASISLEMDETAY.HARCAMABIRIMKOD AS HARCAMABIRIMKOD1");
            sql = sql.Replace("TASISLEMDETAY.FISNO", "TASISLEMDETAY.FISNO AS FISNO1");
            sql = sql.Replace("TASISLEMDETAY.SIRANO", "TASISLEMDETAY.SIRANO AS SIRANO1");

            sql += "TASISLEMDETAY LEFT OUTER JOIN ";
            sql += "TASISLEMOZELLIK LEFT OUTER JOIN TASMARKA ON ";
            sql += "TASISLEMOZELLIK.MARKAKOD = TASMARKA.KOD ";
            sql += "LEFT OUTER JOIN TASMODEL ON ";
            sql += "TASISLEMOZELLIK.MARKAKOD = TASMODEL.MARKAKOD AND ";
            sql += "TASISLEMOZELLIK.MODELKOD = TASMODEL.KOD ON ";
            sql += "TASISLEMDETAY.YIL = TASISLEMOZELLIK.YIL AND ";
            sql += "TASISLEMDETAY.MUHASEBEKOD = TASISLEMOZELLIK.MUHASEBEKOD AND ";
            sql += "TASISLEMDETAY.HARCAMABIRIMKOD = TASISLEMOZELLIK.HARCAMABIRIMKOD AND ";
            sql += "TASISLEMDETAY.FISNO = TASISLEMOZELLIK.FISNO AND ";
            sql += "TASISLEMDETAY.SIRANO = TASISLEMOZELLIK.SIRANO LEFT OUTER JOIN ";
            sql += "TASHESAPPLAN LEFT OUTER JOIN ";
            sql += "TASOLCUBIRIM ON TASHESAPPLAN.OLCUBIRIMKOD = TASOLCUBIRIM.KOD ON ";
            sql += "TASISLEMDETAY.HESAPPLANKOD = TASHESAPPLAN.KOD ";
            sql += "LEFT OUTER JOIN TASRFIDETIKETTURU ON ";
            sql += "TASISLEMOZELLIK.RFIDETIKETTURKOD = TASRFIDETIKETTURU.KOD ";
            sql += "LEFT OUTER JOIN TASODA ON TASISLEMDETAY.YERLESKEYERI = TASODA.KOD ";

            //Fişe ait bütün satırlar
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));

            string siralama = "";
            if (siraNoSirali)
                siralama = " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];
            else
                siralama = " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KDVORAN];

            sql = TABGenel.GetPaginatedSQL(sayfaNo, kayitSayisi, sql, siralama);

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

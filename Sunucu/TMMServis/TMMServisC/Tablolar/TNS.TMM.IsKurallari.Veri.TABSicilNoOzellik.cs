using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{

    /// <summary>
    /// Sicil no özellik bilgilerini tutan TASSICILNOOZELLIK veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABSicilNoOzellik : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSicilNoOzellik(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASSICILNOOZELLIK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Programatik sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSICILNO = 0,
            /// <summary>
            /// Marka kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MARKAKOD = 1,
            /// <summary>
            /// Model kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODELKOD = 2,
            /// <summary>
            /// Şase no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SASENO = 3,
            /// <summary>
            /// Motor no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MOTORNO = 4,
            /// <summary>
            /// Plaka alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PLAKA = 5,
            /// <summary>
            /// Cilt no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CILTNO = 6,
            /// <summary>
            /// Dil alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DIL = 7,
            /// <summary>
            /// Yazar adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAZARADI = 8,
            /// <summary>
            /// Adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADI = 9,
            /// <summary>
            /// Yayın yeri alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAYINYERI = 10,
            /// <summary>
            /// Yayın tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAYINTARIHI = 11,
            /// <summary>
            /// Nereden geldi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDENGELDI = 12,
            /// <summary>
            /// Boyutları alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BOYUTLARI = 13,
            /// <summary>
            /// Satır sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATIRSAYISI = 14,
            /// <summary>
            /// Yaprak sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAPRAKSAYISI = 15,
            /// <summary>
            /// Sayfa sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYFASAYISI = 16,
            /// <summary>
            /// Cilt türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CILTTURU = 17,
            /// <summary>
            /// Çeşidi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CESIDI = 18,
            /// <summary>
            /// Yeri konusu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YERIKONUSU = 19,
            /// <summary>
            /// Geliş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GELISTARIHI = 20,
            /// <summary>
            /// Nerede bulundu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDEBULUNDU = 21,
            /// <summary>
            /// Çağı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CAGI = 22,
            /// <summary>
            /// Durum maddesi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUMUMADDESI = 23,
            /// <summary>
            /// Ön yüz alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONYUZ = 24,
            /// <summary>
            /// Arka yüz alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ARKAYUZ = 25,
            /// <summary>
            /// Fotoğraf alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FOTOGRAF = 26,
            /// <summary>
            /// Puan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PUAN = 27,
            /// <summary>
            /// Dış sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DISSICILNO = 28,

            DISSICILNO2 = 29,
            BISNO = 30,
            ESKIBISNO1 = 31,
            ESKIBISNO2 = 32,
            RFIDETIKETTURKOD = 33,
            GARANTIBITISTARIHI = 34,
            GIAI = 35,

            EKNO = 36,
            HESAPTARIHI = 37,
            BUTCEKODU = 38,
            BLOKAJKODU = 39,
            GELDIGISUBE = 40,

            AMORTISMANYIL = 41,
            AMORTISMANBITTI = 42,
            BULUNDUGUYER = 43,
            RFIDETIKETTARIHI = 44,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "PRSICILNO", "MARKAKOD", "MODELKOD", "SASENO", "MOTORNO", "PLAKA", "CILTNO", "DIL", "YAZARADI", "ADI", "YAYINYERI", "YAYINTARIHI", "NEREDENGELDI", "BOYUTLARI", "SATIRSAYISI", "YAPRAKSAYISI", "SAYFASAYISI", "CILTTURU", "CESIDI", "YERIKONUSU", "GELISTARIHI", "NEREDEBULUNDU", "CAGI", "DURUMUMADDESI", "ONYUZ", "ARKAYUZ", "FOTOGRAF", "PUAN", "DISSICILNO",
            "DISSICILNO2", "BISNO", "ESKIBISNO1", "ESKIBISNO2", "RFIDETIKETTURKOD", "GARANTIBITISTARIHI", "GIAI", "EKNO", "HESAPTARIHI", "BUTCEKODU", "BLOKAJKODU", "GELDIGISUBE", "AMORTISMANYIL", "AMORTISMANBITTI", "BULUNDUGUYER", "RFIDETIKETTARIHI" };

        /// <summary>
        /// Sicil No Özellik bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="deger">Sicil No Özellik bilgilerini tutan, SicilNoOzellik türünde nesne</param>
        /// <returns>Ekleme, Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(SicilNoOzellik deger)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            if (deger.yeriKonusu == deger.saseNo) deger.yeriKonusu = "";//yeri alanına şase no bilgisi yazılıyor.(Tekrarlı bilgi oluyor)

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(deger.prSicilno) + ",";
            sql += VeriErisim.DegerYap(deger.markaKod) + ",";
            sql += VeriErisim.DegerYap(deger.modelKod) + ",";
            sql += VeriErisim.DegerYap(deger.saseNo) + ",";
            sql += VeriErisim.DegerYap(deger.motorNo) + ",";
            sql += VeriErisim.DegerYap(deger.plaka) + ",";
            sql += VeriErisim.DegerYap(deger.ciltNo) + ",";
            sql += VeriErisim.DegerYap(deger.dil) + ",";
            sql += VeriErisim.DegerYap(deger.yazarAdi) + ",";
            sql += VeriErisim.DegerYap(deger.adi) + ",";
            sql += VeriErisim.DegerYap(deger.yayinYeri) + ",";
            sql += VeriErisim.DegerYap(deger.yayinTarihi) + ",";
            sql += VeriErisim.DegerYap(deger.neredenGeldi) + ",";
            sql += VeriErisim.DegerYap(deger.boyutlari) + ",";
            sql += VeriErisim.DegerYap(deger.satirSayisi) + ",";
            sql += VeriErisim.DegerYap(deger.yaprakSayisi) + ",";
            sql += VeriErisim.DegerYap(deger.sayfaSayisi) + ",";
            sql += VeriErisim.DegerYap(deger.ciltTuru) + ",";
            sql += VeriErisim.DegerYap(deger.cesidi) + ",";
            sql += VeriErisim.DegerYap(deger.yeriKonusu) + ",";
            sql += VeriErisim.DegerYap(deger.gelisTarihi) + ",";
            sql += VeriErisim.DegerYap(deger.neredeBulundu) + ",";
            sql += VeriErisim.DegerYap(deger.cagi) + ",";
            sql += VeriErisim.DegerYap(deger.durumuMaddesi) + ",";
            sql += VeriErisim.DegerYap(deger.onYuz) + ",";
            sql += VeriErisim.DegerYap(deger.arkaYuz) + ",";
            sql += VeriErisim.DegerYap(deger.fotograf) + ",";
            sql += VeriErisim.DegerYap(deger.puan) + ",";
            sql += VeriErisim.DegerYap(deger.disSicilNo) + ",";
            sql += VeriErisim.DegerYap(deger.disSicilNo2) + ",";
            sql += VeriErisim.DegerYap(deger.bisNo) + ",";
            sql += VeriErisim.DegerYap(deger.eskiBisNo1) + ",";
            sql += VeriErisim.DegerYap(deger.eskiBisNo2) + ",";
            sql += VeriErisim.DegerYap(deger.rfidEtiketTurKod) + ",";
            sql += VeriErisim.DegerYap(deger.garantiBitisTarihi) + ",";
            sql += VeriErisim.DegerYap(deger.giai) + ",";
            sql += VeriErisim.DegerYap(deger.ekNo) + ",";
            sql += VeriErisim.DegerYap(deger.hesapTarih) + ",";
            sql += VeriErisim.DegerYap(deger.butceKodu) + ",";
            sql += VeriErisim.DegerYap(deger.blokajKodu) + ",";
            sql += VeriErisim.DegerYap(deger.geldigiSube) + ",";
            sql += VeriErisim.DegerYap(deger.amortismanYil) + ",";
            sql += VeriErisim.DegerYap(deger.amortismanBitti) + ",";
            sql += VeriErisim.DegerYap(deger.bulunduguYer) + ",";
            sql += VeriErisim.DegerYap(deger.rfidEtiketTarihi) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";

                if (deger.kayitTur == 1) //Yapıştır işlemi ile sadece ilgili kaydın güncellemesi için bu kontrol eklendi.
                {
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SASENO], deger.saseNo);
                }
                else
                {
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MARKAKOD], deger.markaKod) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MODELKOD], deger.modelKod) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SASENO], deger.saseNo) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MOTORNO], deger.motorNo) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PLAKA], deger.plaka) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CILTNO], deger.ciltNo) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIL], deger.dil) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAZARADI], deger.yazarAdi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], deger.adi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAYINYERI], deger.yayinYeri) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAYINTARIHI], deger.yayinTarihi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREDENGELDI], deger.neredenGeldi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BOYUTLARI], deger.boyutlari) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SATIRSAYISI], deger.satirSayisi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAPRAKSAYISI], deger.yaprakSayisi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYFASAYISI], deger.sayfaSayisi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CILTTURU], deger.ciltTuru) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CESIDI], deger.cesidi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YERIKONUSU], deger.yeriKonusu) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELISTARIHI], deger.gelisTarihi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREDEBULUNDU], deger.neredeBulundu) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CAGI], deger.cagi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUMUMADDESI], deger.durumuMaddesi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONYUZ], deger.onYuz) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARKAYUZ], deger.arkaYuz) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FOTOGRAF], deger.fotograf) + ",";

                    if (deger.disSicilNo != "KAYDETME")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DISSICILNO], deger.disSicilNo) + ",";

                    if (deger.disSicilNo2 != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DISSICILNO2], deger.disSicilNo2) + ",";
                    if (deger.bisNo != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BISNO], deger.bisNo) + ",";
                    if (deger.eskiBisNo1 != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ESKIBISNO1], deger.eskiBisNo1) + ",";
                    if (deger.eskiBisNo2 != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ESKIBISNO2], deger.eskiBisNo2) + ",";
                    if (deger.rfidEtiketTurKod > 0)
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RFIDETIKETTURKOD], deger.rfidEtiketTurKod) + ",";
                    if (!deger.garantiBitisTarihi.isNull)
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GARANTIBITISTARIHI], deger.garantiBitisTarihi) + ",";
                    if (deger.giai != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIAI], deger.giai) + ",";
                    if (deger.ekNo != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKNO], deger.ekNo) + ",";
                    if (deger.hesapTarih != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPTARIHI], deger.hesapTarih) + ",";
                    if (deger.butceKodu != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUTCEKODU], deger.butceKodu) + ",";
                    if (deger.blokajKodu != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BLOKAJKODU], deger.blokajKodu) + ",";
                    if (deger.geldigiSube != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELDIGISUBE], deger.geldigiSube) + ",";
                    if (deger.amortismanYil != 0)
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMORTISMANYIL], deger.amortismanYil < 0 ? 0 : deger.amortismanYil) + ",";
                    if (deger.amortismanBitti != 0)
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMORTISMANBITTI], deger.amortismanBitti < 0 ? 0 : deger.amortismanBitti) + ",";
                    if (deger.bulunduguYer != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULUNDUGUYER], deger.bulunduguYer) + ",";
                    if (!deger.rfidEtiketTarihi.isNull)
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RFIDETIKETTARIHI], deger.rfidEtiketTarihi) + ",";

                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PUAN], deger.puan);
                }

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", deger.prSicilno));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Programatik Sicil No bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Sicil No Özellik bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="prSicilNo">Programatik Sicil no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Sicil No Özellik bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(int prSicilNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (prSicilNo > 0)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo);

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.PRSICILNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Programatik Sicil no bilgisini kullanarak
        /// Sicil No Özellik bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="prSicilNo">Programatik Sicil no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(int prSicilNo)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc BisKaydet(SicilNoOzellik deger)
        {
            string sql = "";
            veriErisim.hata = "";
            sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BISNO], deger.bisNo) + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", deger.prSicilno));

            Sonuc sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sonuc = Kaydet(deger);
            }

            return sonuc;
        }

        public string BisNoGetir(int prSicilNo)
        {
            string sql = "SELECT BISNO FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));

            return veriErisim.TekKolonSQLStr(sql);
        }

        public Sonuc BisSil(SicilNoOzellik deger)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            veriErisim.hata = "";
            sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BISNO], "") + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BISNO], "=", deger.bisNo));

            if (deger.prSicilno > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", deger.prSicilno));

            return KomutCalistir(sql, 0);
        }

        public string SicilNoGetir(string prSicilNo)
        {
            string sql = "SELECT SICILNO FROM TASSICILNORFIDVIEW ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));

            return veriErisim.TekKolonSQLStr(sql);
        }

        public int PrSicilNoGetir(string sicilNo)
        {
            string sql = "SELECT PRSICILNO FROM TASSICILNORFIDVIEW ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SICILNO", "=", sicilNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public IDataReader PrSicilNoListele(SicilNoHareket form)
        {
            string sql = "SELECT OZ.PRSICILNO, H1.SICILNO, OZ.DISSICILNO, OZ.BISNO, OZ.ESKIBISNO1 FROM TASSICILNOOZELLIK OZ ";
            sql += "INNER JOIN TASSICILNOHAREKET H1 ON H1.PRSICILNO = OZ.PRSICILNO AND H1.SIRANO = 1 ";

            string kosul = string.Empty;
            if (!string.IsNullOrWhiteSpace(form.ambarKod))
                kosul = VeriErisim.KosulOl("H1", "AMBARKOD", " = ", form.ambarKod);
            if (!string.IsNullOrWhiteSpace(form.sicilNo))
                kosul = VeriErisim.KosulOl("H1", "SICILNO", " = ", form.sicilNo);
            if (!string.IsNullOrWhiteSpace(form.ozellik.disSicilNo))
                kosul = VeriErisim.KosulOl("OZ", "DISSICILNO", " = ", form.ozellik.disSicilNo);

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY OZ.PRSICILNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc BisDus(int[] prSicilNoListe)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            veriErisim.hata = "";
            sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BISNO], "") + " ";

            string kosul = "";

            foreach (var prSicilNo in prSicilNoListe)
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BISNO], "=", prSicilNo));
            kosul = "(" + kosul + ") ";

            kosul += " AND (NOT (PRSICILNO IN (" + string.Join(",", prSicilNoListe) + ")))";

            if (kosul != "")
                sql += " WHERE " + kosul + "";

            return KomutCalistir(sql, 0);
        }

        public int AmortsimanSureGetir(int prSicilNo)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.AMORTISMANYIL] + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc AmortsimanSureKaydet(int prSicilNo, int sure)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMORTISMANYIL], sure) + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));

            return KomutCalistir(sql, 1);
        }


        public Sonuc HesapPlanKodDegistir(int prSicilNo, string yeniHesapKod, string islemYapan)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            veriErisim.komutTuru = CommandType.StoredProcedure;
            veriErisim.param.Add(new VeriPara("@PRSICILNO", DbType.Int32, prSicilNo));
            veriErisim.param.Add(new VeriPara("@HESAPPLANKODYENI", DbType.String, yeniHesapKod.Replace(".", "").Trim()));
            veriErisim.param.Add(new VeriPara("@ISLEMYAPAN", DbType.String, islemYapan));

            int i = veriErisim.ExecuteNonquery("SPHESAPPLANDEGISTIR");
            if (i > 0)
            {
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = "Hesap plan kodu başarıyla değiştirildi.";
            }
            else
                sonuc.DegerAta(veriErisim.hata);

            return sonuc;
        }

        public Sonuc RFIDEtiketTarihiGuncelle(int prSicilNo, TNSDateTime tarih)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RFIDETIKETTARIHI], tarih) + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));

            return KomutCalistir(sql, 1);
        }

    }
}
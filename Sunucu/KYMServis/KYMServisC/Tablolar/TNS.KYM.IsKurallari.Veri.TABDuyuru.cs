using System;
using TNS;
using TNS.Veri;

namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// Kullanıcılara sisteme girdiklerinde, giriş bilgi ekranında gösterilen duyuruları tutan "DUYURU" veri tabanı tablosu ve kullanıcılara gönderilecek elektronik postaları tutan KULLANICI_EPOSTA veri tabanı tablosu ve postaların kimlere gönderileceğini tutan KULLANICI_EPOSTA_GONDER veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABDuyuru : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABDuyuru(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Kullanıcılara gösterilen duyuruları tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOADDUYURU = "DUYURU";

        /// <summary>
        /// Kullanıcılara gösterilen duyuruları tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarDUYURU : int
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// ONEM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONEM = 1,

            /// <summary>
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 2,

            /// <summary>
            /// DUYURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DUYURU = 3,

            /// <summary>
            /// GIZLE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIZLE = 4,

            /// <summary>
            /// KTKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KTKOD = 5,

            /// <summary>
            /// PENCEREDE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PENCEREDE = 6,

            /// <summary>
            /// KONU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONU = 7
        }

        /// <summary>
        /// Kullanıcılara gösterilen duyuruları tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLARDUYURU = { "KOD", "ONEM", "TARIH", "DUYURU", "GIZLE", "KTKOD", "PENCEREDE", "KONU" };

        /// <summary>
        /// Kullanıcılara gönderilecek elektronik postaları tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOADEPOSTA = "KULLANICI_EPOSTA";

        /// <summary>
        /// Kullanıcılara gönderilecek elektronik postaları tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarEPOSTA : int
        {
            /// <summary>
            /// MESAJID alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MESAJID = 0,

            /// <summary>
            /// KONU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONU = 1,

            /// <summary>
            /// MESAJ alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MESAJ = 2,

            /// <summary>
            /// KTKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KTKOD = 3,

            /// <summary>
            /// GONDEREN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GONDEREN = 4,

            /// <summary>
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 5
        }

        /// <summary>
        /// Kullanıcılara gönderilecek elektronik postaları tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAREPOSTA = { "MESAJID", "KONU", "MESAJ", "KTKOD", "GONDEREN", "TARIH" };

        /// <summary>
        /// Kullanıcılara gönderilecek elektronik postaların kimlere gönderileceğini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOADEPOSTAGONDER = "KULLANICI_EPOSTA_GONDER";

        /// <summary>
        /// Kullanıcılara gönderilecek elektronik postaların kimlere gönderileceğini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        enum ENUMAlanlarEPOSTAGONDER : int
        {
            /// <summary>
            /// MESAJID alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MESAJID = 0,

            /// <summary>
            /// KULLANICIKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 1,

            /// <summary>
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 2,

            /// <summary>
            /// EMAIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EMAIL = 3,

            /// <summary>
            /// HATAMESAJI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HATAMESAJI = 4,

            /// <summary>
            /// GONDERILDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GONDERILDIMI = 5,
        }

        /// <summary>
        /// Kullanıcılara gönderilecek elektronik postaların kimlere gönderileceğini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAREPOSTAGONDER = { "MESAJID", "KULLANICIKODU", "TARIH", "EMAIL", "HATAMESAJI", "GONDERILDIMI" };

        /// <summary>
        /// Veri tabanında kayıtlı ve verilen sql koşuluna uyan duyuru listesini döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kosul">Duyuru kıstası olan sql koşulu.</param>
        /// <returns>Duyuruları bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        private System.Data.IDataReader Listele(string kosul)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDUYURU, ALANLARDUYURU);
            sql = sql + " FROM DUYURU ";
            if (kosul != "")
                sql = sql + " Where " + kosul;

            sql += " ORDER BY " + ENUMAlanlarDUYURU.ONEM + ", " + ENUMAlanlarDUYURU.TARIH + " DESC" + ", " + ENUMAlanlarDUYURU.KOD + " DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Veri tabanında kayıtlı tüm duyuru listesini döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <returns>Duyuruları bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader Listele()
        {
            return Listele("");
        }

        /// <summary>
        /// Veri tabanında kayıtlı ve verilen kıstasa uyan duyuru listesini döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="duyuru">Seçilecek duyurularla ilgili kıstasları tutan nesne.</param>
        /// <returns>Duyuruları bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader Ara(Duyuru duyuru)
        {
            string kosul = "";

            if (duyuru.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.KOD], "=", duyuru.kod));
            else
            {
                if (!string.IsNullOrWhiteSpace(duyuru.konu))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.KONU], "LIKE", "%" + duyuru.konu + "%"));
                }
                if (duyuru.gizle != ENUMGizle.TANIMSIZ)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.GIZLE], "=", (int)duyuru.gizle));

                if (duyuru.pencerede != ENUMPencerede.TANIMSIZ)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.PENCEREDE], "=", (int)duyuru.pencerede));

                if (!TNSDateTime.IsNull(duyuru.baslaTarih))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.TARIH], ">=", duyuru.baslaTarih));

                if (!TNSDateTime.IsNull(duyuru.bitisTarih))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.TARIH], "<=", duyuru.bitisTarih));

                if (duyuru.kullaniciTipleri != null)
                {
                    string ktKosul = "";
                    for (int i = 0; i < duyuru.kullaniciTipleri.Length; i++)
                    {
                        if (duyuru.kullaniciTipleri[i] == ENUMKullaniciTipi.SISTEMYONETICISI)
                        {
                            ktKosul = "";
                            break;
                        }
                        if (duyuru.kullaniciTipleri[i] != ENUMKullaniciTipi.TANIMSIZ)
                            ktKosul = VeriErisim.KosulEkle(ktKosul, "OR", VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.KTKOD], "LIKE", "%" + ((int)duyuru.kullaniciTipleri[i]).ToString() + ";%"));
                    }

                    if (ktKosul != "")
                    {
                        ktKosul = VeriErisim.KosulEkle(ktKosul, "OR", VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.KTKOD], "LIKE", "0;%"));
                        ktKosul = VeriErisim.KosulEkle(ktKosul, "OR", VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.KTKOD], "LIKE", "%;0;%"));
                        kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + ktKosul + ")");
                    }
                }
                if (duyuru.onem > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.ONEM], "=", duyuru.onem));
            }
            return Listele(kosul);
        }

        /// <summary>
        /// Veri tabanınından kodu  verilen duyuruyu döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kod">Okunacak duyurunun anhtarı olan kod.</param>
        /// <returns>Duyuruyu bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader Oku(int kod)
        {
            return Listele(VeriErisim.KosulOl(TABLOADDUYURU, ALANLARDUYURU[(int)ENUMAlanlarDUYURU.KOD], "=", kod));
        }

        /// <summary>
        /// Sitemde kayıtlı en büyük duyuru kodunun bir fazlasını, eğer tanımlı duyuru yoksa 1 döndüren yordam. (Select Max SQL'i üretir)
        /// </summary>
        /// <returns>Yeni duyuru kaydetmek için oluşturulan kodu döndürür.</returns>
        public int DuyuruKoduUret()
        {
            int maxKod = 0;
            string sql = "SELECT MAX(KOD) FROM " + TABLOADDUYURU;
            System.Data.IDataReader r = veriErisim.DataReaderDondur(sql);
            if (r.Read())
            {
                if (!r.IsDBNull(0))
                    maxKod = VeriErisim.DegerAl(r, 0, 0);
                else
                    maxKod = 0;
            }
            r.Close();
            return maxKod + 1;
        }

        /// <summary>
        /// Veri tabanına kaydedilecek duyuru için sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="duyuru">Eklenecek duyurunun bilgilerini tutan nesne.</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc Ekle(Duyuru duyuru)
        {
            string sql = "insert into " + TABLOADDUYURU + "(" + VeriErisim.TumAlanlar("", ALANLARDUYURU) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(duyuru.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(duyuru.onem) + ", ";
            sql = sql + VeriErisim.DegerYap(duyuru.tarih) + ", ";
            sql = sql + VeriErisim.DegerYap(duyuru.duyuru) + ", ";
            sql = sql + VeriErisim.DegerYap((int)duyuru.gizle) + ", ";
            sql = sql + VeriErisim.DegerYap(duyuru.KullaniciTipleriStr) + ", ";
            sql = sql + VeriErisim.DegerYap((int)duyuru.pencerede) + ", ";
            sql = sql + VeriErisim.DegerYap(duyuru.konu) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında varolan duyuru bilgilerini güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="duyuru">Güncellenecek duyuru kaydının yeni bilgilerini tutan nesne.</param>
        /// <returns>Oluşturulan update komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc Guncelle(Duyuru duyuru)
        {
            string sql = "UPDATE " + TABLOADDUYURU + " SET ";
            sql = sql + VeriErisim.AtamaOl("", ALANLARDUYURU[(int)ENUMAlanlarDUYURU.ONEM], duyuru.onem) + ", "
                      + VeriErisim.AtamaOl("", ALANLARDUYURU[(int)ENUMAlanlarDUYURU.TARIH], duyuru.tarih) + ", "
                      + VeriErisim.AtamaOl("", ALANLARDUYURU[(int)ENUMAlanlarDUYURU.DUYURU], duyuru.duyuru) + ", "
                      + VeriErisim.AtamaOl("", ALANLARDUYURU[(int)ENUMAlanlarDUYURU.GIZLE], (int)duyuru.gizle) + ", "
                      + VeriErisim.AtamaOl("", ALANLARDUYURU[(int)ENUMAlanlarDUYURU.PENCEREDE], (int)duyuru.pencerede) + ", "
                      + VeriErisim.AtamaOl("", ALANLARDUYURU[(int)ENUMAlanlarDUYURU.KONU], duyuru.konu) + ", "
                      + VeriErisim.AtamaOl("", ALANLARDUYURU[(int)ENUMAlanlarDUYURU.KTKOD], duyuru.KullaniciTipleriStr);
            sql = sql + " WHERE ";
            sql = sql + VeriErisim.KosulOl("", ALANLARDUYURU[(int)ENUMAlanlarDUYURU.KOD], "=", duyuru.kod);

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kodu verilen duyuruyu veri tabanından silmek için 
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kod">Silinecek duyurunun kodu.</param>
        /// <returns>Tertiple ilişkili malzemeleri silmek için oluşturulan delete komutu çalıştırılır ve sonucu döndürülür.</returns>
        public Sonuc Sil(int kod)
        {
            string sql = "delete from " + TABLOADDUYURU + " WHERE ";
            sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl("", ALANLARDUYURU[(int)ENUMAlanlarDUYURU.KOD], "=", kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Sitemde kayıtlı en büyük posta kodunun bir fazlasını, eğer tanımlı posta yoksa 1 döndüren yordam. (Select Max SQL'i üretir)
        /// </summary>
        /// <returns>Yeni posta kaydetmek için oluşturulan kodu döndürür.</returns>
        public int EPostaKoduUret()
        {
            int maxKod = 0;
            string sql = "SELECT MAX(" + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.MESAJID] + ") FROM " + TABLOADEPOSTA;
            System.Data.IDataReader r = veriErisim.DataReaderDondur(sql);
            if (r.Read())
            {
                if (!r.IsDBNull(0))
                    maxKod = VeriErisim.DegerAl(r, 0, 0);
                else
                    maxKod = 0;
            }
            r.Close();
            return maxKod + 1;
        }

        /// <summary>
        /// Veri tabanına kaydedilecek posta kaydı için sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kod">Eklenecek postanın anahtarı.</param>
        /// <param name="gonderen">Postanın gönderen kişinin kullanıcı kodu.</param>
        /// <param name="konu">Postanın konusu.</param>
        /// <param name="mesaj">Postanın metni.</param>
        /// <param name="ktKod">Postanın gönderileceği kullanıcı tipi.</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc EPostaKaydet(int kod, string gonderen, string konu, string mesaj, int ktKod)
        {
            string sql = "INSERT INTO " + TABLOADEPOSTA + " (" + VeriErisim.TumAlanlar("", ALANLAREPOSTA) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(kod) + ", ";
            sql = sql + VeriErisim.DegerYap(konu) + ", ";
            sql = sql + VeriErisim.DegerYap(mesaj) + ", ";
            sql = sql + VeriErisim.DegerYap(ktKod) + ", ";
            sql = sql + VeriErisim.DegerYap(gonderen) + ",";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now.ToString())) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Postanın gönderileceği kişileri veri tabanına kaydedilecek veya kayıtlı ise güncelleyecek sql cümlelerini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="mesajId">Eklenecek kaydın ilişkili olduğu postanın anahtarı.</param>
        /// <param name="kullaniciKodu">Eklenecek kaydın ilişkili olduğu postayı gönderen kullanıcı kodu.</param>
        /// <param name="tarih">Kaydın eklendiği/güncellendiği tarih.</param>
        /// <param name="email">Postanın gönderileceği kişinin elektronik posta adresi.</param>
        /// <param name="hataMesaj">Posta gönderme işlemi sırasında hata oluşursa, oluşan hatanın metni.</param>
        /// <param name="gonderildimi">Posta gönderildisi ise 1, gönderilemedi ise veya gönderilecek ise 0.</param>
        /// <returns>Oluşturulan update/insert komutlarının çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc EPostaGonderKaydet(int mesajId, string kullaniciKodu, TNSDateTime tarih, string email, string hataMesaj, int gonderildimi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADEPOSTAGONDER + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.MESAJID], mesajId) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.KULLANICIKODU], kullaniciKodu) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.TARIH], tarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.EMAIL], email) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.HATAMESAJI], hataMesaj) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.GONDERILDIMI], gonderildimi) + " ";
            sql = VeriErisim.KosulEkle(sql, " WHERE ", VeriErisim.KosulOl("", ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.MESAJID], "=", mesajId));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.KULLANICIKODU], "=", kullaniciKodu));

            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOADEPOSTAGONDER + " (" + VeriErisim.TumAlanlar("", ALANLAREPOSTAGONDER) + ") VALUES (";
                sql += VeriErisim.DegerYap(mesajId) + ",";
                sql += VeriErisim.DegerYap(kullaniciKodu) + ",";
                sql += VeriErisim.DegerYap(tarih) + ",";
                sql += VeriErisim.DegerYap(email) + ",";
                sql += VeriErisim.DegerYap(hataMesaj) + ",";
                sql += VeriErisim.DegerYap(gonderildimi) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Elektronik posta gönderilenler listesinden verilen "gonderildiMi" koduna göre gönderilenleri yada gönderilmeyenleri, gönderilecek posta bilgileri ile listeleyen 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="mesajId">Gönderilecek postanın anahtar kodu.</param>
        /// <param name="gonderildiMi">0 ise gönderilmeyenler, 1 ise gönderilenler listelenir.</param>
        /// <returns>Posta bilgilerini ve gönderilecek kişileri bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader EPostaGonderListele(int mesajId, int gonderildiMi)
        {
            string sql = "SELECT ";
            sql += TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.MESAJID] + ", ";
            sql += TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.KONU] + ", ";
            sql += TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.MESAJ] + ", ";
            sql += TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.KTKOD] + ", ";
            sql += TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.GONDEREN] + ", ";
            sql += TABLOADEPOSTAGONDER + "." + ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.KULLANICIKODU] + ", ";
            sql += TABLOADEPOSTAGONDER + "." + ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.EMAIL];
            sql += " FROM " + TABLOADEPOSTA + " INNER JOIN " + TABLOADEPOSTAGONDER + " ON ";
            sql += TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.MESAJID] + " = " + TABLOADEPOSTAGONDER + "." + ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.MESAJID];
            if (mesajId > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADEPOSTAGONDER, ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.MESAJID], "=", mesajId));
            if (gonderildiMi > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADEPOSTAGONDER, ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.GONDERILDIMI], "=", gonderildiMi));

            sql += " ORDER BY 1,2";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Elektronik posta gönderilenler listesinden, toplam gönderilecekler ile gönderilenlerin posta başına sayılarını bulan
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <returns>Toplam gönderilecekler ile gönderilenlerin posta başına sayılarını bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader EPostaGonderOncekiGonderiler()
        {
            string sql = "SELECT ";
            sql += TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.MESAJID] + ", ";
            sql += TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.KONU] + ", ";
            sql += TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.GONDEREN] + ", ";
            sql += TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.TARIH] + ", ";
            sql += "COUNT(*)" + ", ";
            sql += "SUM(" + TABLOADEPOSTAGONDER + "." + ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.GONDERILDIMI] + ")";
            sql += " FROM " + TABLOADEPOSTAGONDER + " INNER JOIN " + TABLOADEPOSTA + " ON " + TABLOADEPOSTAGONDER + "." + ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.MESAJID] + " = " + TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.MESAJID];
            sql += " GROUP BY " + TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.MESAJID] + ", " + TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.KONU] + ", " + TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.GONDEREN] + ", " + TABLOADEPOSTA + "." + ALANLAREPOSTA[(int)ENUMAlanlarEPOSTA.TARIH];
            sql += " ORDER BY 1 DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Posta numarası verilen, postayla ilgili kullanıcıları verilen "gonderildiMi" parametresine göre döndüren
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="mesajId">Kullanıcıları döndürülecek elektronik postanın numarası.</param>
        /// <param name="gonderildiMi">-1 ise tüm kullanıcılar, 1 ise gönderilenler, 0 ise gönderilmeyenler listenir.</param>
        /// <returns>Bir postayla ilgili gönderileceklerin listesini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader EPostaGonderilenlerListele(int mesajId, int gonderildiMi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAREPOSTAGONDER) + " FROM " + TABLOADEPOSTAGONDER;
            string kosul = string.Empty;
            if (mesajId > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.MESAJID], "=", mesajId));
            if (gonderildiMi > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAREPOSTAGONDER[(int)ENUMAlanlarEPOSTAGONDER.GONDERILDIMI], "=", gonderildiMi));
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;
            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        public System.Data.IDataReader GorevleriOku(string kosul)
        {
            string sql = "SELECT * FROM GRVGOREVLER WHERE ATANANKISIKOD='" + kosul + "'";
            sql += " ORDER BY ATAMATARIHI DESC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

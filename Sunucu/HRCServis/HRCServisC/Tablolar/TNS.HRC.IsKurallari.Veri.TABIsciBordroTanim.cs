using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İşçi bordro tanım bilgisini tutan PSLISCIBORDROTANIM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIsciBordroTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIsciBordroTanim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İşçi bordro tanım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLISCIBORDROTANIM";

        /// <summary>
        /// İşçi bordro tanım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar : int
        {
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 1,
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 2,
            /// <summary>
            /// AY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AY = 3,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,
            /// <summary>
            /// YEVMIYE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YEVMIYE = 5,
            /// <summary>
            /// ASGARIGECIMINDIRIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ASGARIGECIMINDIRIM = 6,
            /// <summary>
            /// OZELSIGORTA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OZELSIGORTA = 7,
            /// <summary>
            /// SAKATLIKINDIRIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAKATLIKINDIRIM = 8,
            /// <summary>
            /// PRIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRIM = 9,
            /// <summary>
            /// SOSYALYARDIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOSYALYARDIM = 10,
            /// <summary>
            /// TASITYARDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASITYARDIMI = 11,
            /// <summary>
            /// YEMEKYARDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YEMEKYARDIMI = 12,
            /// <summary>
            /// SENDIKAAIDAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SENDIKAAIDAT = 13,
        }

        /// <summary>
        /// İşçi bordro tanım tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "BIRIMKOD", "YIL", "AY", "TCKIMLIKNO", "YEVMIYE", "ASGARIGECIMINDIRIM", "OZELSIGORTA", "SAKATLIKINDIRIM", "PRIM", "SOSYALYARDIM", "TASITYARDIMI", "YEMEKYARDIMI", "SENDIKAAIDAT" };

        /// <summary>
        /// İşçi bordro tanım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek işçi bordro tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (IsciBordroTanim f in bilgi.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.muhasebe) + ",";
                sql += VeriErisim.DegerYap(f.birim) + ",";
                sql += VeriErisim.DegerYap(f.yil) + ",";
                sql += VeriErisim.DegerYap(f.ay) + ",";
                sql += VeriErisim.DegerYap(f.tcKimlikNo) + ",";
                sql += VeriErisim.DegerYap(f.yevmiye) + ",";
                sql += VeriErisim.DegerYap(f.asgariGecimIndirim) + ",";
                sql += VeriErisim.DegerYap(f.ozelSigorta) + ",";
                sql += VeriErisim.DegerYap(f.sakatlikIndirimi) + ",";
                sql += VeriErisim.DegerYap(f.prim) + ",";
                sql += VeriErisim.DegerYap(f.sosyalYardim) + ",";
                sql += VeriErisim.DegerYap(f.tasitYardimi) + ",";
                sql += VeriErisim.DegerYap(f.yemekYardimi) + ",";
                sql += VeriErisim.DegerYap(f.sendikaAidati) + ")";


                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";

                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASGARIGECIMINDIRIM], f.asgariGecimIndirim) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YEVMIYE], f.yevmiye) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OZELSIGORTA], f.ozelSigorta) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAKATLIKINDIRIM], f.sakatlikIndirimi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PRIM], f.prim) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOSYALYARDIM], f.sosyalYardim) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASITYARDIMI], f.tasitYardimi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SENDIKAAIDAT], f.sendikaAidati) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YEMEKYARDIMI], f.yemekYardimi);

                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", f.tcKimlikNo));

                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", f.ay));


                    sonuc = KomutCalistir(sql, 1);

                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// İşçi bordro tanım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak işçi bordro tanım bilgisi kıstas parametresi</param>
        /// <returns>
        /// İşçi bordro tanım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IsciBordroTanim bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + DBDegiskeni.TABLOADKUMILATIFVERGI + ".KUMULATIFVERGIMATRAHI";
            sql += " FROM " + TABLOAD + " LEFT OUTER JOIN " + DBDegiskeni.TABLOADKUMILATIFVERGI + " ON "
               + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + DBDegiskeni.TABLOADKUMILATIFVERGI + ".TCKIMLIKNO "
               + " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + DBDegiskeni.TABLOADKUMILATIFVERGI + ".YIL "
                + " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AY] + " = " + DBDegiskeni.TABLOADKUMILATIFVERGI + ".AY ";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " ASC";

            return veriErisim.DataReaderDondur(sql);
        }
    }

}
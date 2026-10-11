using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Sözleşmeli bordro tanım bilgisini tutan PSLSOZBORDROTANIM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSozlesmeliBordroTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSozlesmeliBordroTanim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Sözleşmeli bordro tanım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLSOZBORDROTANIM";

        /// <summary>
        /// Sözleşmeli bordro tanım bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// SOZLESMEUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOZLESMEUCRET = 5,
            /// <summary>
            /// EKTAZMINATORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKTAZMINATORAN = 6,
            /// <summary>
            /// ASGARIGECIMINDIRIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ASGARIGECIMINDIRIM = 7,
            /// <summary>
            /// OZELSIGORTA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OZELSIGORTA = 8,
            /// <summary>
            /// SAKATLIKINDIRIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAKATLIKINDIRIM = 9,
            /// <summary>
            /// SENDIKADEVLETYARDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SENDIKADEVLETYARDIMI = 10,
            /// <summary>
            /// SENDIKAAIDATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SENDIKAAIDATI = 11,
            /// <summary>
            /// LOJMAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            LOJMAN = 12,
            /// <summary>
            /// ICRA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ICRA = 13,
            /// <summary>
            /// NAFAKA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            NAFAKA = 14,
            /// <summary>
            /// DIGERKESINTI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DIGERKESINTI = 15,
            /// <summary>
            /// BORDROTUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BORDROTUR = 16,
            /// <summary>
            /// SSKISVERENPAYIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISVERENPAYIORAN = 17,
        }

        /// <summary>
        /// Sözleşmeli bordro tanım tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "BIRIMKOD", "YIL", "AY", "TCKIMLIKNO", "SOZLESMEUCRET", "EKTAZMINATORAN", "ASGARIGECIMINDIRIM", "OZELSIGORTA", "SAKATLIKINDIRIM", "SENDIKADEVLETYARDIMI", "SENDIKAAIDATI", "LOJMAN", "ICRA", "NAFAKA", "DIGERKESINTI", "BORDROTUR", "SSKISVERENPAYIORAN" };

        /// <summary>
        /// Personel bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONEL = "HRCPERSONEL";

        /// <summary>
        /// Personel bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPersonel : int
        {
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// SOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOYAD = 2,
            /// <summary>
            /// UNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVAN = 3,
        }

        /// <summary>
        /// Personel tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONEL = { "TCKIMLIKNO", "AD", "SOYAD", "UNVAN" };

        /// <summary>
        /// Sözleşmeli bordro tanım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek sözleşmeli bordro tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (TNS.HRC.SozlesmeliBordroTanim f in bilgi.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.muhasebe) + ",";
                sql += VeriErisim.DegerYap(f.birim) + ",";
                sql += VeriErisim.DegerYap(f.yil) + ",";
                sql += VeriErisim.DegerYap(f.ay) + ",";
                sql += VeriErisim.DegerYap(f.tcKimlikNo) + ",";
                sql += VeriErisim.DegerYap(f.sozlesmeUcret) + ",";
                sql += VeriErisim.DegerYap(f.ekTazminatOrani) + ",";
                sql += VeriErisim.DegerYap(f.asgariGecimIndirim) + ",";
                sql += VeriErisim.DegerYap(f.ozelSigorta) + ",";
                sql += VeriErisim.DegerYap(f.sakatlikIndirimi) + ",";
                sql += VeriErisim.DegerYap(f.sendikaDevletYardimi) + ",";
                sql += VeriErisim.DegerYap(f.sendikaAidati) + ",";
                sql += VeriErisim.DegerYap(f.lojman) + ",";
                sql += VeriErisim.DegerYap(f.icra) + ",";
                sql += VeriErisim.DegerYap(f.nafaka) + ",";
                sql += VeriErisim.DegerYap(f.digerKesinti) + ",";
                sql += VeriErisim.DegerYap(f.bordroTur) + ",";
                sql += VeriErisim.DegerYap(f.sskIsverenPayiOrani) + ")";


                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";

                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASGARIGECIMINDIRIM], f.asgariGecimIndirim) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKTAZMINATORAN], f.ekTazminatOrani) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OZELSIGORTA], f.ozelSigorta) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAKATLIKINDIRIM], f.sakatlikIndirimi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEUCRET], f.sozlesmeUcret) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SENDIKADEVLETYARDIMI], f.sendikaDevletYardimi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SENDIKAAIDATI], f.sendikaAidati) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.LOJMAN], f.lojman) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICRA], f.icra) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NAFAKA], f.nafaka) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIGERKESINTI], f.digerKesinti) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SSKISVERENPAYIORAN], f.sskIsverenPayiOrani);

                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", f.tcKimlikNo));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", f.ay));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BORDROTUR], "=", f.bordroTur));

                    sonuc = KomutCalistir(sql, 1);

                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Sözleşmeli bordro tanım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak sözleşmeli bordro tanım bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sözleşmeli bordro tanım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TNS.HRC.SozlesmeliBordroTanim bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + DBDegiskeni.TABLOADKUMILATIFVERGI + ".KUMULATIFVERGIMATRAHI" +
                    "," + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.AD] +
                    "," + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.SOYAD] +
                    "," + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.UNVAN] +
                    "," + DBDegiskeni.TABLOADKUMILATIFVERGI + ".YIL" +
                    "," + DBDegiskeni.TABLOADKUMILATIFVERGI + ".AY";
            sql += " FROM " + TABLOAD + " LEFT OUTER JOIN " + TABLOADPERSONEL + " ON " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO] +
                    " LEFT OUTER JOIN " + DBDegiskeni.TABLOADKUMILATIFVERGI + " ON "
                    + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + DBDegiskeni.TABLOADKUMILATIFVERGI + ".TCKIMLIKNO ";
            //+" AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + DBDegiskeni.TABLOADKUMILATIFVERGI + ".YIL "
            //+ " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AY] + " = " + DBDegiskeni.TABLOADKUMILATIFVERGI + ".AY ";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay));
            if (bilgi.tcKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", bilgi.tcKimlikNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " ASC";

            return veriErisim.DataReaderDondur(sql);
        }


    }

}
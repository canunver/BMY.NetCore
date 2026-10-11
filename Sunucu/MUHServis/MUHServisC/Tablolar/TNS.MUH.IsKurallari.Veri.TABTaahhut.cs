using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHTAAHHUT veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTaahhut : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhut(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHTAAHHUT";

        public enum ENUMAlanlar
        {
            KOD = 0,
            TAAHHUTKOD = 1,
            YIL = 2,
            AY = 3,
            ILKTAAHHUTTUTARI = 4,
            FIYATFARKI = 5,
            ILAVETAAHHUTLER = 6,
            DIGERTAAHHUTLER = 7,
            ISLEMTARIHI = 8,
            ISLEMYAPANKOD = 9,
            ISLEMYAPANAD = 10,
            TAAHHUTDEGERARTISIKOD = 11,
            TERTIPKOD = 12,
            SONRAKIYILTUTAR = 13,
        }

        static string[] ALANLAR ={ "KOD", "TAAHHUTKOD", "YIL", "AY", "ILKTAAHHUTTUTARI", "FIYATFARKLI", "ILAVETAAHHUTLER","DIGERTAAHHUTLER",
                                     "ISLEMTARIHI", "ISLEMYAPANKOD", "ISLEMYAPANAD", "TAAHHUTDEGERARTISIKOD", "TERTIPKOD", "SONRAKIYILTUTAR"};

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="taahhut">The taahhut.</param>
        /// <returns></returns>
        public Sonuc Kaydet(TaahhutBilgi taahhut, bool ekle)
        {
            Sonuc sonuc;
            string sql;

            if (ekle)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(taahhut.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.taahhutKod) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.yil) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.ay) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.ilkTaahhutTutari) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.fiyatFarki) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.ilaveTaahhutler) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.digerTaahhutler) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.islemTarihi) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.islemYapanKod) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.islemYapanAd) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.degerArtisKod) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.tertipKod) + ", ";
                sql = sql + VeriErisim.DegerYap(0) + ") ";
                sonuc = KomutCalistir(sql, 1);
                sonuc.anahtar = taahhut.kod.ToString();
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], taahhut.yil) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AY], taahhut.ay) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKTAAHHUTTUTARI], taahhut.ilkTaahhutTutari) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIYATFARKI], taahhut.fiyatFarki) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILAVETAAHHUTLER], taahhut.ilaveTaahhutler) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIGERTAAHHUTLER], taahhut.digerTaahhutler) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], taahhut.islemTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], taahhut.islemYapanKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], taahhut.islemYapanAd) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TERTIPKOD], taahhut.tertipKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONRAKIYILTUTAR], 0);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", taahhut.kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", taahhut.taahhutKod));
                if (taahhut.degerArtisKod == null)
                    sql = VeriErisim.KosulEkle(sql, "AND", $"{ALANLAR[(int)ENUMAlanlar.TAAHHUTDEGERARTISIKOD]} IS NULL");
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTDEGERARTISIKOD], "=", taahhut.degerArtisKod));
                sonuc = KomutCalistir(sql, 1);
                sonuc.anahtar = taahhut.kod.ToString();
            }
            return sonuc;
        }

        public Sonuc SozlesmeTutarGuncelle(int taahhutKod)
        {
            string sql = $"UPDATE {TABTaahhutKarti.TABLOAD} SET IHALEBEDELI= (SELECT SUM(ILKTAAHHUTTUTARI+FIYATFARKLI+ILAVETAAHHUTLER+DIGERTAAHHUTLER) FROM {TABLOAD} WHERE TAAHHUTKOD={taahhutKod}) WHERE KOD={taahhutKod}";
            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kod">kod.</param>
        /// <returns></returns>
        public Sonuc Sil(int kod, int taahhutKod, string taahhutKodDegerArtisKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            string kosul = "";

            if (kod != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod + " "));
            if (taahhutKod != 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", taahhutKod + " "));

                if (taahhutKodDegerArtisKod == null)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"{ALANLAR[(int)ENUMAlanlar.TAAHHUTDEGERARTISIKOD]} IS NULL");
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTDEGERARTISIKOD], "=", taahhutKodDegerArtisKod));
            }
            if (!string.IsNullOrEmpty(kosul))
            {
                sql += " WHERE " + kosul;
                return KomutCalistir(sql, 0);
            }
            else
                return new Sonuc("Taahhüt bilgi silme parametresi yok!");
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(TaahhutBilgi bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = "";
            if (bilgi.taahhutKod != -1)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod));
                if (bilgi.degerArtisKod == null)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"{ALANLAR[(int)ENUMAlanlar.TAAHHUTDEGERARTISIKOD]} IS NULL");
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTDEGERARTISIKOD], "=", bilgi.degerArtisKod));
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 3, 4, 13";

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        public IDataReader MevcutOnayliToplamBul(int taahhutKod, bool degerArtisiMi)
        {
            string sql = "SELECT T.TERTIPKOD, T.YIL, T.AY, SUM(ILKTAAHHUTTUTARI), SUM(FIYATFARKLI), SUM(ILAVETAAHHUTLER), SUM(DIGERTAAHHUTLER) FROM " + TABLOAD + " T";
            sql += $" INNER JOIN MUHTAAHHUTKARTI TK ON TK.KOD = T.TAAHHUTKOD";
            string oeBag;
            if (degerArtisiMi)
            {
                sql += $" INNER JOIN MUHTAAHHUTKARTIDEGERARTISI DA ON DA.KOD = T.TAAHHUTDEGERARTISIKOD";
                oeBag = "DA";
            }
            else
            {
                oeBag = "TK";
            }

            sql += $" LEFT OUTER JOIN MUHMIFFORM MIF ON MIF.YIL = {oeBag}.ODEMEEMRIYIL AND MIF.BELGENO = {oeBag}.ODEMEEMRINO";

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", "MIF.FORMDURUM = 6");
            kosul = VeriErisim.KosulEkle(kosul, "AND", $"T.TAAHHUTDEGERARTISIKOD IS {(degerArtisiMi ? "NOT" : "")} NULL");
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TK", "KOD", "=", taahhutKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TK", "KAPANDI", "=", 0));
            sql += " WHERE " + kosul;
            sql += " GROUP BY T.TERTIPKOD, T.YIL, T.AY ";

            sql += " ORDER BY T.TERTIPKOD, T.YIL, T.AY ";

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        public IDataReader OEBekleyenListele(int yil, int muhasebeKod, string birimKod, bool degerArtisiMi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("T", ALANLAR) + ", TK.MUTEAHHITKIMLIKNO, MIF.YIL, MIF.BELGENO, MIF.FORMDURUM FROM " + TABLOAD + " T";
            sql += $" INNER JOIN MUHTAAHHUTKARTI TK ON TK.KOD = T.TAAHHUTKOD";
            string oeBag;
            if (degerArtisiMi)
            {
                sql += $" INNER JOIN MUHTAAHHUTKARTIDEGERARTISI DA ON DA.KOD = T.TAAHHUTDEGERARTISIKOD";
                oeBag = "DA";
            }
            else
            {
                oeBag = "TK";
            }

            sql += $" LEFT OUTER JOIN MUHMIFFORM MIF ON MIF.YIL = {oeBag}.ODEMEEMRIYIL AND MIF.BELGENO = {oeBag}.ODEMEEMRINO";

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TK", "YIL", "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", $"T.TAAHHUTDEGERARTISIKOD IS {(degerArtisiMi ? "NOT" : "")} NULL");
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TK", "MUHASEBEKOD", "=", muhasebeKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TK", "BIRIMKOD", "=", birimKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TK", "KAPANDI", "=", 0));
            sql += " WHERE " + kosul;

            sql += " ORDER BY TK.MUTEAHHITKIMLIKNO, T.YIL, T.AY, T.TERTIPKOD";

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }


        /// <summary>
        /// Siras the no bul.
        /// </summary>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public int SiraNoBul()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            return (int)veriErisim.TekKolonSQLDbl(sql) + 1;
        }
    }
}

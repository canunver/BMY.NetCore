using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{

    public class TABSicilNoDegerArtis : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSicilNoDegerArtis(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASSICILNODEGERARTIS";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            MUHASEBEKOD = 1,
            HARCAMABIRIMKOD = 2,
            BELGENO = 3,
            BELGETARIHI = 4,
            PRSICILNO = 5,
            GORSICILNO = 6,
            TUTAR = 7,
            GEREKCE = 8,
            DURUM = 9,
            TUR = 10,
            HESAPPLANKOD = 11,
            ESKITUTAR = 12,
            TIFNO = 13,
            AMBARKOD = 14,
            KDVORAN = 15,
            KATSAYI = 16,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "MUHASEBEKOD", "HARCAMABIRIMKOD", "BELGENO", "BELGETARIHI", "PRSICILNO", "GORSICILNO", "TUTAR", "GEREKCE", "DURUM", "TUR", "HESAPPLANKOD", "ESKITUTAR", "TIFNO", "AMBARKOD", "KDVORAN", "KATSAYI" };

        public Sonuc Kaydet(SicilNoDegerArtis bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.harcamaKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.belgeNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.gorSicilNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.gerekce) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.tur) + ",";
            sql += VeriErisim.DegerYap(bilgi.hesapPlanKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.eskiTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.tifNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ambarKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.kdvOran) + ",";
            sql += VeriErisim.DegerYap(bilgi.katsayi) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc Guncelle(SicilNoDegerArtis bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOD], bilgi.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], bilgi.muhasebeKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], bilgi.harcamaKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], bilgi.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], bilgi.prSicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GORSICILNO], bilgi.gorSicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTAR], bilgi.tutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], bilgi.tur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], bilgi.gerekce) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], bilgi.hesapPlanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ESKITUTAR], bilgi.eskiTutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIFNO], bilgi.tifNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], bilgi.ambarKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVORAN], bilgi.kdvOran) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KATSAYI], bilgi.katsayi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc GuncelleTifNo(SicilNoDegerArtis bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIFNO], bilgi.tifNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public IDataReader Listele(SicilNoDegerArtis bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TASMUHASEBE.AD, TASHARCAMABIRIM.AD, TASHESAPPLAN.KOD, TASHESAPPLAN.ACIKLAMA";
            sql += ", (SELECT SUM(AMORTISMANMIKTAR) FROM TASAMORTISMAN IC WHERE TASSICILNODEGERARTIS.KOD = IC.DEGERARTISKODU) AS AMORTISMANMIKTAR";
            sql += " FROM " + TABLOAD;
            sql += " INNER JOIN TASMUHASEBE ON TASSICILNODEGERARTIS.MUHASEBEKOD = TASMUHASEBE.KOD";
            sql += " INNER JOIN TASHARCAMABIRIM ON TASSICILNODEGERARTIS.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD AND TASMUHASEBE.KOD = TASHARCAMABIRIM.MUHASEBEKOD";
            sql += " INNER JOIN TASSICILNOHAREKET ON TASSICILNODEGERARTIS.PRSICILNO = TASSICILNOHAREKET.PRSICILNO AND TASSICILNOHAREKET.SIRANO=1";
            sql += " INNER JOIN TASHESAPPLAN ON TASSICILNOHAREKET.HESAPPLANKOD = TASHESAPPLAN.KOD";
            sql += " INNER JOIN TASSICILNOOZELLIK ON TASSICILNODEGERARTIS.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO";

            string kosul = string.Empty;
            if (bilgi.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (bilgi.muhasebeKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            if (bilgi.harcamaKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", bilgi.harcamaKod));
            if (bilgi.belgeNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "=", bilgi.belgeNo));
            if (bilgi.sorguBelgeNoBas != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], ">=", bilgi.sorguBelgeNoBas));
            if (bilgi.sorguBelgeNoBit != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "<=", bilgi.sorguBelgeNoBit));
            if (!bilgi.belgeTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETARIHI], "=", bilgi.belgeTarihi));
            if (!bilgi.sorguBelgeBasTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETARIHI], ">=", bilgi.sorguBelgeBasTarihi));
            if (!bilgi.sorguBelgeBitTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETARIHI], "<=", bilgi.sorguBelgeBitTarihi));
            if (bilgi.prSicilNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", bilgi.prSicilNo));
            if (bilgi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));
            if (bilgi.gorSicilNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GORSICILNO], "=", bilgi.gorSicilNo));
            if (bilgi.sorguEskiSicilNo != "")
            {
                string icKosulES = "";
                icKosulES = VeriErisim.KosulEkle(icKosulES, "AND", VeriErisim.KosulOl("TASSICILNOOZELLIK", "DISSICILNO", "=", bilgi.sorguEskiSicilNo));
                icKosulES = VeriErisim.KosulEkle(icKosulES, "OR", VeriErisim.KosulOl("TASSICILNOOZELLIK", "DISSICILNO2", "=", bilgi.sorguEskiSicilNo));
                kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosulES + ")");
            }
            if (bilgi.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));
            else if (bilgi.durum == 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL)); //Bütün Belgeler (İptaller Hariç)


            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BELGENO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleToplamTutar(SicilNoDegerArtis bilgi)
        {
            string sql = "SELECT TASSICILNODEGERARTIS.PRSICILNO, SUM(TASSICILNODEGERARTIS.TUTAR) AS TOPLAMTUTAR";
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (bilgi.prSicilNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", bilgi.prSicilNo));

            if (bilgi.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));

            if (bilgi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " GROUP BY TASSICILNODEGERARTIS.PRSICILNO";

            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Sil(SicilNoDegerArtis bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc GuncelleDurum(SicilNoDegerArtis bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public bool EnflasyonDuzeltmesiYapildiMi(SicilNoDegerArtis bilgi)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", bilgi.prSicilNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], "=", bilgi.belgeTarihi));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        public bool EnflasyonDuzeltmesiYapildiMiMB(SicilNoDegerArtis bilgi)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", bilgi.prSicilNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", 1));
            if (bilgi.belgeTarihi.Yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], "=", bilgi.belgeTarihi));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }
    }
}
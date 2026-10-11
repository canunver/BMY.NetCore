using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABVeriToplama : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABVeriToplama(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Koy bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMVERITOPLAMA";

        /// <summary>
        /// Koy bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            ACIKLAMA = 1,
            ILKOD = 2,
            YENIYATIRIM = 3,
            MODTEKYEN = 4,
            TAMAMLAMA = 5,
            FAAL = 6,
            FAALDEGIL = 7,
            GERCEKKISIE = 8,
            GERCEKKISIK = 9,
            SIRKET = 10,
            KOOPERATIF = 11,
            TOPLAMYATIRIMTUTARI = 12,
            TOPLAMHIBETUTARI = 13,
            ISLENENHAMMADDEMIKTARI = 14,
            BUYUKBAS = 15,
            KUCUKBAS = 16,
            SAGLANANISTIHDAM = 17,
            IHRACATYAPANTESISSAYISI = 18,
            SONYILCIRO = 19
        }


        public static string[] ALANLAR = { "KOD", "ACIKLAMA", "ILKOD", "YENIYATIRIM", "MODTEKYEN", "TAMAMLAMA", "FAAL", "FAALDEGIL", "GERCEKKISIE", "GERCEKKISIK", "SIRKET", "KOOPERATIF", "TOPLAMYATIRIMTUTARI", "TOPLAMHIBETUTARI", "ISLENENHAMMADDEMIKTARI", "BUYUKBAS", "KUCUKBAS", "SAGLANANISTIHDAM", "IHRACATYAPANTESISSAYISI", "SONYILCIRO" };

        public Sonuc Kaydet(VeriToplama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YENIYATIRIM], bilgi.yeniYatirim) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MODTEKYEN], bilgi.modTekYen) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAMAMLAMA], bilgi.tamamlama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAAL], bilgi.faal) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAALDEGIL], bilgi.faalDegil) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GERCEKKISIE], bilgi.gercekKisiE) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GERCEKKISIK], bilgi.gercekKisiK) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRKET], bilgi.sirket) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOOPERATIF], bilgi.kooperatif) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPLAMYATIRIMTUTARI], bilgi.toplamYatirimTutari) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPLAMHIBETUTARI], bilgi.toplamHibeTutari) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLENENHAMMADDEMIKTARI], bilgi.islenenHammaddeMiktari) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUYUKBAS], bilgi.buyukBas) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KUCUKBAS], bilgi.kucukBas) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAGLANANISTIHDAM], bilgi.saglananIstihdam) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHRACATYAPANTESISSAYISI], bilgi.ihracatYapanTesisSayisi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONYILCIRO], bilgi.sonYilCiro);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
                sql += VeriErisim.DegerYap(bilgi.ilKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.yeniYatirim) + ",";
                sql += VeriErisim.DegerYap(bilgi.modTekYen) + ",";
                sql += VeriErisim.DegerYap(bilgi.tamamlama) + ",";
                sql += VeriErisim.DegerYap(bilgi.faal) + ",";
                sql += VeriErisim.DegerYap(bilgi.faalDegil) + ",";
                sql += VeriErisim.DegerYap(bilgi.gercekKisiE) + ",";
                sql += VeriErisim.DegerYap(bilgi.gercekKisiK) + ",";
                sql += VeriErisim.DegerYap(bilgi.sirket) + ",";
                sql += VeriErisim.DegerYap(bilgi.kooperatif) + ","; ;
                sql += VeriErisim.DegerYap(bilgi.toplamYatirimTutari) + ","; ;
                sql += VeriErisim.DegerYap(bilgi.toplamHibeTutari) + ","; ;
                sql += VeriErisim.DegerYap(bilgi.islenenHammaddeMiktari) + ","; ;
                sql += VeriErisim.DegerYap(bilgi.buyukBas) + ","; ;
                sql += VeriErisim.DegerYap(bilgi.kucukBas) + ","; ;
                sql += VeriErisim.DegerYap(bilgi.saglananIstihdam) + ","; ;
                sql += VeriErisim.DegerYap(bilgi.ihracatYapanTesisSayisi) + ","; ;
                sql += VeriErisim.DegerYap(bilgi.sonYilCiro) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc Sil(VeriToplama bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(VeriToplama bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.ilKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod));

            if (bilgi.aciklama != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "=", bilgi.aciklama));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tabloda kullanılan en büyük kod numarasını döndürür
        /// </summary>
        /// <returns>Verilen kriter bilgisine uygun kayıt bulunursa true, yoksa false döndürür</returns>
        public int EnBuyukKod()
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
            sql += " FROM " + TABLOAD;

            int deger = veriErisim.TekKolonSQLInt(sql);

            return deger + 1;
        }
    }
}

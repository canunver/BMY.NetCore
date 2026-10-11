using System.Data;
using TNS.Veri;
using System;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABHibeMakineReferansFiyat : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeMakineReferansFiyat(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "TRMHIBEMAKINAREFFIYAT";
        enum ENUMAlanlar
        {
            ETAPNO = 0,
            ILKOD = 1,
            MALZEMEKOD = 2,
            FIYAT = 3
        }
        static string[] ALANLAR = { "ETAPNO", "ILKOD", "MALZEMEKOD", "FIYAT" };

        static string TABLOADTARIHCE = "TRMHIBEMAKINAREFFIYATTARIHCE";
        enum ENUMAlanlarTarihce
        {
            ETAPNO = 0,
            ILKOD = 1,
            MALZEMEKOD = 2,
            FIYAT = 3,
            ISLEMYAPAN = 4,
            ISLEMTARIHI = 5
        }
        static string[] ALANLARTARIHCE = { "ETAPNO", "ILKOD", "MALZEMEKOD", "FIYAT", "ISLEMYAPAN", "ISLEMTARIHI" };

        /// <summary>
        /// HibeMakineRefFiyat bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(HibeMakineRefFiyat bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIYAT], bilgi.fiyat);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MALZEMEKOD], "=", bilgi.malzemeKod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.ilKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.malzemeKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.fiyat) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc KaydetTarihce(HibeMakineRefFiyat bilgi)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar(TABLOADTARIHCE, ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.malzemeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.fiyat) + ",";
            sql += VeriErisim.DegerYap(bilgi.islemYapan) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(System.DateTime.Now)) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc BasvuruRefFiyatlariGuncelle(HibeMakineRefFiyat bilgi)
        {
            string sql = "UPDATE TRMHIBEMAKINADETAY SET ";
            sql += VeriErisim.AtamaOl("", "REFERANSTUTAR", bilgi.fiyat);
            sql += " FROM TRMHIBEMAKINA INNER JOIN";
            sql += " TRMHIBEMAKINADETAY ON TRMHIBEMAKINA.KOD = TRMHIBEMAKINADETAY.HIBEKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "TRMHIBEMAKINA.ETAPNO", "=", bilgi.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "TRMHIBEMAKINA.ILKOD", "=", bilgi.ilKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "TRMHIBEMAKINADETAY.MALZEMEKOD", "=", bilgi.malzemeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "TRMHIBEMAKINA.DURUM", "<", (int)ENUMHibeMakineBasvuruDurum.FATURAGELDI));

            return KomutCalistir(sql, 0);
        }

        public Sonuc BasvuruFiyatlariGuncelle(HibeMakineDetay bilgi)
        {
            string sql = "UPDATE TRMHIBEMAKINADETAY SET ";
            sql += VeriErisim.AtamaOl("", "REFERANSTUTAR", bilgi.referansTutar) + ",";
            sql += VeriErisim.AtamaOl("", "TOPLAMTUTAR", bilgi.toplamTutar) + ",";
            sql += VeriErisim.AtamaOl("", "HIBEESASTUTAR", bilgi.hibeEsasTutar) + ",";
            sql += VeriErisim.AtamaOl("", "AYNIKATKI", bilgi.ayniKatki) + ",";
            sql += VeriErisim.AtamaOl("", "HIBETUTAR", bilgi.hibeTutar);
            sql += " FROM TRMHIBEMAKINADETAY";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "HIBEKOD", "=", bilgi.hibeKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// BankaSube bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// BankaSube bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(HibeMakineRefFiyat bilgi, bool sadeceAktifKayitlar)
        {
            string kosul = "";
            string sql = " SELECT TRMMALZEMEGRUBU.KOD MGKOD, TRMMALZEMEGRUBU.AD MGAD, TRMMALZEME.KOD MKOD, TRMMALZEME.AD MAD,";
            sql += " (SELECT FIYAT FROM TRMHIBEMAKINAREFFIYAT WHERE TRMMALZEME.KOD = TRMHIBEMAKINAREFFIYAT.MALZEMEKOD";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod));
            sql += " ) FROM TRMMALZEMEGRUBU INNER JOIN";
            sql += " TRMMALZEME ON TRMMALZEMEGRUBU.KOD = TRMMALZEME.MALZEMEGRUBUKOD";
            if (bilgi.malzemeGrupKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TRMMALZEMEGRUBU.KOD", "=", bilgi.malzemeGrupKod));
            if (bilgi.malzemeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TRMMALZEME.KOD", "=", bilgi.malzemeKod));
            if (sadeceAktifKayitlar)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TRMMALZEME.KULLANIMDA", "=", 1));

            if (bilgi.sadeceBasvuruMalzemeleri > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", "TRMMALZEME.KOD IN");
                kosul += " (SELECT TRMHIBEMAKINADETAY.MALZEMEKOD FROM TRMHIBEMAKINADETAY INNER JOIN TRMHIBEMAKINA ON TRMHIBEMAKINADETAY.HIBEKOD = TRMHIBEMAKINA.KOD ";
                kosul = VeriErisim.KosulEkle(kosul, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod));
                kosul += ")";
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY MGAD, MAD";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleTarihce(HibeMakineRefFiyat bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTARIHCE, ALANLARTARIHCE) + " FROM " + TABLOADTARIHCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ETAPNO], "=", bilgi.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ILKOD], "=", bilgi.ilKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTARIHCE[(int)ENUMAlanlarTarihce.MALZEMEKOD], "=", bilgi.malzemeKod));
            sql += " ORDER BY 1,2,3,6";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(int etapNo, string ilKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", ilKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilTarihce(int etapNo, string ilKod)
        {
            string sql = "DELETE FROM " + TABLOADTARIHCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ETAPNO], "=", etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ILKOD], "=", ilKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SonrakiYilaAktar(HibeMakineRefFiyat bilgi, double ufeOran, string kullaniciAd)
        {
            string sql = "INSERT INTO " + TABLOAD + "(ETAPNO, ILKOD, MALZEMEKOD, FIYAT)";
            sql += " SELECT " + (bilgi.etapNo + 1) + ", ILKOD, MALZEMEKOD, FIYAT * (1+(" + ufeOran.ToString().Replace(',', '.') + "/100)) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIYAT], ">", 0));

            Sonuc sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sql = "INSERT INTO " + TABLOADTARIHCE + "(ETAPNO, ILKOD, MALZEMEKOD, FIYAT, ISLEMYAPAN, ISLEMTARIHI)";
                sql += " SELECT ETAPNO, ILKOD, MALZEMEKOD, FIYAT, '" + kullaniciAd + "', " + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now.ToString())) + " FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo + 1));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIYAT], ">", 0));

                sonuc = KomutCalistir(sql, 0);
            }
            return sonuc;

        }

    }
}

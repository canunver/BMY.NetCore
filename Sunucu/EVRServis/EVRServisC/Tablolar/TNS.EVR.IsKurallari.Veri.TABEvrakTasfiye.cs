using System;
using TNS.Veri;
using System.Data;

namespace TNS.EVR.IsKurallari.Veri
{

    /// <summary>
    /// Birim bilgilerini içeren EVREVRAKTASFIYE veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABTasfiye : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABTasfiye sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABTasfiye(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tasfiye bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVREVRAKTASFIYE";

        /// <summary>
        /// Tasfiye bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlar
        {
            PREVRAKNO = 0,
            TASFIYEKOD = 1,
            TASFIYETARIHI = 2,
        }

        /// <summary>
        /// Tasfiye bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "PREVRAKNO", "TASFIYEKOD", "TASFIYETARIHI" };

        /// <summary>
        /// Parametre olarak verilen Tasfiye nesne koleksiyonuna göre; Saklama bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Tasfiye nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            foreach (EvrakTasfiye bilgi in liste.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(bilgi.prEvrakNo) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.tasfiyeKod) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.tasfiyeTarihi) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASFIYEKOD], bilgi.tasfiyeKod) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASFIYETARIHI], bilgi.tasfiyeTarihi) + " ";
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", bilgi.prEvrakNo);

                    sonuc = KomutCalistir(sql, 1);
                }


                if (!sonuc.islemSonuc)
                    return sonuc;
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Tasfiye nesne koleksiyonuna göre; Tasfiye bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Tasfiye nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(string prEvrakNo)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo);

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
                return sonuc;

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen Tasfiye nesnesine göre; Tasfiye bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Tasfiye bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(EvrakSinif form)
        {
            string hareketTablo = "";
            if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.YENI || form.hareket.islem == (int)ENUMEvrakTarihceDurum.HAVALEGELEN || form.hareket.islem == (int)ENUMEvrakTarihceDurum.BUTUNBELGELER)
                hareketTablo = TABGenel.VIEWHAREKETSONDURUM;//Evrak havale edildikten sonra bilgilerinde değişiklik yapılırsa bile izi olan birimlerde listelenmeli
            else
                hareketTablo = TABGenel.VIEWHAREKETSONDURUMBIRIM;

            string sql = "SELECT ";

            sql += "EVREVRAK.EVREVRAKNO, EVREVRAK.YIL, EVREVRAK.EVRAKTIPI, EVREVRAK.EVRAKBIRIMKODU, EVREVRAK.EVRAKNO, EVREVRAK.EVRAKTARIH, EVREVRAK.GELDIGIYER, EVREVRAK.GELENEVRAKNO, EVREVRAK.GELENEVRAKTARIH, EVREVRAK.KONU, EVREVRAK.GIZLILIK, EVREVRAK.YAZIDURUM, EVREVRAK.SURECAD ";
            sql += ",EVRYAZIDURUMU.AD, EVRGIZLILIK.AD ";
            sql += "," + hareketTablo + ".ISLEM ";
            sql += ", TS1.PREVRAKNO, TS1.ASIL, '' DOSYAPLANKODLARI";
            sql += ",EVRKTSFIY.TASFIYEKOD,EVREVRAK.OLUSTURMATARIHI,EVREVRAK.EVRAKBIRIMKODU";
            sql += " FROM EVREVRAK";
            sql += " INNER JOIN EVREVRAKSICIL TS1 ON EVREVRAK.EVREVRAKNO = TS1.EVREVRAKNO ";
            sql += " INNER JOIN " + hareketTablo + " ON TS1.PREVRAKNO = " + hareketTablo + ".PREVRAKNO ";
            sql += " LEFT OUTER JOIN EVRYAZIDURUMU ON EVREVRAK.YAZIDURUM = EVRYAZIDURUMU.KOD ";
            sql += " LEFT OUTER JOIN EVRGIZLILIK ON EVREVRAK.GIZLILIK = EVRGIZLILIK.KOD ";
            //sql += "  LEFT OUTER  JOIN EVRBIRIM EVRBKD0 ON EVREVRAK.HAVALEBIRIMKODU = EVRBKD0.KOD ";
            //sql += "  LEFT OUTER  JOIN EVRBIRIM EVRBKD2 ON " + hareketTablo + ".BIRIMKOD2 = EVRBKD2.KOD ";
            //sql += "  LEFT OUTER  JOIN EVRBIRIM EVRBKD1 ON " + hareketTablo + ".BIRIMKOD1 = EVRBKD1.KOD ";
            sql += " INNER JOIN EVREVRAKTASFIYE EVRKTSFIY  ON EVRKTSFIY.PREVRAKNO=TS1.PREVRAKNO ";

            string kosul = "";

            if (!string.IsNullOrEmpty(form.evrakBirimKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKBIRIMKODU", "LIKE", form.evrakBirimKodu));
            if (form.sicil.asil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TS1", "ASIL", "=", form.sicil.asil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "<>", (int)ENUMEvrakHareketDurum.IPTALEDILDI));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "YIL", "<=", DateTime.Now.Year));
            if (kosul != "")
            {
                sql += " WHERE " + kosul;
                sql += " ORDER BY EVREVRAK.EVRAKNO DESC";
            }

            if (!string.IsNullOrEmpty(kosul))
                return veriErisim.DataReaderDondur(sql);
            else
                return null;

        }
    }
}

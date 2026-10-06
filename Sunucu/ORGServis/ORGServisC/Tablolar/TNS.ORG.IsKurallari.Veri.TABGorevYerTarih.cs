using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.ORG.IsKurallari.Veri
{
    /// <summary>
    /// Görev Yer Tarih veri tabanı tablosu ile ilgili olarak listeleme, kaydetme ve silme işlemini yapan sınıf
    /// </summary>
    public class TABGorevYerTarih : TABOrtak
    {
        /// <summary>
        ///  Sınıfın oluşturucu(Constructer) fonksiyonu
        /// </summary>
        /// <param name="veriErisim">Veri Erişim tipinde sınıf</param>
        public TABGorevYerTarih(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adı
        /// </summary>
        static string TABLOAD = "PERGOREVYERTARIH";

        /// <summary>
        /// Tablo alanları indisi
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Görev kodu
            /// </summary>
            GOREVKOD = 0,
            /// <summary>
            /// Sıra numarası
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// Bulunduğu ülke kodu
            /// </summary>
            BULUNDUGUULKEKOD = 2,
            /// <summary>
            /// Bulunduğu il kodu
            /// </summary>
            BULUNDUGUILKOD = 3,
            /// <summary>
            /// Bulunduğu ilçe kodu
            /// </summary>
            BULUNDUGUILCEKOD = 4,
            /// <summary>
            /// Gideceği ülke kodu
            /// </summary>
            GIDECEGIULKEKOD = 5,
            /// <summary>
            /// Gideceği il kodu
            /// </summary>
            GIDECEGIILKOD = 6,
            /// <summary>
            /// Gideceği ilçe kodu
            /// </summary>
            GIDECEGIILCEKOD = 7,
            /// <summary>
            /// Başlangıç tarihi
            /// </summary>
            BASLANGICTARIH = 8,
            /// <summary>
            /// Bitiş tarihi
            /// </summary>
            BITISTARIH = 9,
            /// <summary>
            /// Vasıta
            /// </summary>
            VASITA = 10,
            /// <summary>
            /// Yol dahil mi
            /// </summary>
            YOLDAHILMI = 11,
        }

        /// <summary>
        /// Tablo alanları
        /// </summary>
        static string[] ALANLAR = { "GOREVKOD", 
                                    "SIRANO", 
                                    "BULUNDUGUULKEKOD", 
                                    "BULUNDUGUILKOD", 
                                    "BULUNDUGUILCEKOD", 
                                    "GIDECEGIULKEKOD",
                                    "GIDECEGIILKOD", 
                                    "GIDECEGIILCEKOD", 
                                    "BASLANGICTARIH", 
                                    "BITISTARIH",
                                    "VASITA",
                                    "YOLDAHILMI"};

        /// <summary>
        /// Listeleme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="p">Görev Yer Tarih tipinde sınıf</param>
        /// <returns>IDataReader tipinde sınıf döndürür</returns>
        public IDataReader Listele(GorevYerTarih p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," +
                      " PERULKE_1.AD AS BULUNDUGUULKE,  " +
                      " PERIL_1.AD AS BULUNDUGUIL, " +
                      " PERILCE_1.AD AS BULUNDUGUILCE, " +
                      " PERULKE.AD AS GIDECEGIULKE, " +
                      " PERIL.AD AS GIDECEGIIL, " +
                      " PERILCE.AD AS GIDECEGIILCE " +
                  " FROM " + TABLOAD + " LEFT OUTER JOIN " +
                      " PERILCE ON PERGOREVYERTARIH.GIDECEGIULKEKOD = PERILCE.ULKEKOD AND PERGOREVYERTARIH.GIDECEGIILKOD = PERILCE.ILKOD AND  " +
                      " PERGOREVYERTARIH.GIDECEGIILCEKOD = PERILCE.KOD LEFT OUTER JOIN " +
                      " PERIL ON PERGOREVYERTARIH.GIDECEGIULKEKOD = PERIL.ULKEKOD AND PERGOREVYERTARIH.GIDECEGIILKOD = PERIL.KOD LEFT OUTER JOIN " +
                      " PERULKE ON PERGOREVYERTARIH.GIDECEGIULKEKOD = PERULKE.KOD LEFT OUTER JOIN " +

                      " PERILCE AS PERILCE_1 ON PERGOREVYERTARIH.BULUNDUGUULKEKOD = PERILCE_1.ULKEKOD AND  " +
                      " PERGOREVYERTARIH.BULUNDUGUILKOD = PERILCE_1.ILKOD AND PERGOREVYERTARIH.BULUNDUGUILCEKOD = PERILCE_1.KOD LEFT OUTER JOIN " +
                      " PERIL AS PERIL_1 ON PERGOREVYERTARIH.BULUNDUGUULKEKOD = PERIL_1.ULKEKOD AND  " +
                      " PERGOREVYERTARIH.BULUNDUGUILKOD = PERIL_1.KOD LEFT OUTER JOIN " +
                      " PERULKE AS PERULKE_1 ON PERGOREVYERTARIH.BULUNDUGUULKEKOD = PERULKE_1.KOD";

            if (p.gorevKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREVKOD], "=", p.gorevKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kaydetme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="liste">Object Array tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(ObjectArray liste, int gorevKod)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            int siraNo = 1;
            foreach (GorevYerTarih p in liste.objeler)
            {
                sql = string.Empty;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(gorevKod) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(p.bulunduguUlkeKod) + ",";
                sql += VeriErisim.DegerYap(p.bulunduguIlKod) + ",";
                sql += VeriErisim.DegerYap(p.bulunduguIlceKod) + ",";
                sql += VeriErisim.DegerYap(p.gidecegiUlkeKod) + ",";
                sql += VeriErisim.DegerYap(p.gidecegiIlKod) + ",";
                sql += VeriErisim.DegerYap(p.gidecegiIlceKod) + ",";
                sql += VeriErisim.DegerYap(p.baslangicTarihi) + ",";
                sql += VeriErisim.DegerYap(p.bitisTarihi) + ",";
                sql += VeriErisim.DegerYap(p.vasita) + ",";
                sql += VeriErisim.DegerYap(p.yolDahilMi) + ")";

                sonuc = KomutCalistir(sql, 1);
                siraNo++;
                if (!sonuc.islemSonuc)
                {
                    break;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Silme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="gorevKod">Görev kodu</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Sil(int gorevKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOREVKOD], "=", gorevKod));

            return KomutCalistir(sql, 0);
        }
    }
}

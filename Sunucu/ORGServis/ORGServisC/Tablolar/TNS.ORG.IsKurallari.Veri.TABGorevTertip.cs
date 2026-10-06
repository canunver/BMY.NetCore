using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.ORG.IsKurallari.Veri
{
    /// <summary>
    /// Görev Tertip veri tabanı tablosu ile ilgili olarak listeleme, kaydetme ve silme işlemini yapan sınıf
    /// </summary>
    public class TABGorevTertip : TABOrtak
    {
        /// <summary>
        ///  Sınıfın oluşturucu(Constructer) fonksiyonu
        /// </summary>
        /// <param name="veriErisim">Veri Erişim tipinde sınıf</param>
        public TABGorevTertip(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adı
        /// </summary>
        static string TABLOAD = "PERGOREVTERTIP";

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
            /// Tertip
            /// </summary>
            TERTIP = 2,
            /// <summary>
            /// Tutar
            /// </summary>
            TUTAR = 3,
        }

        /// <summary>
        /// Tablo alanları
        /// </summary>
        static string[] ALANLAR = { "GOREVKOD", 
                                    "SIRANO", 
                                    "TERTIP",
                                    "TUTAR"};

        /// <summary>
        /// Listeleme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="p">Görev Tertip tipinde sınıf</param>
        /// <returns>IDataReader tipinde sınıf döndürür</returns>
        public IDataReader Listele(GorevTertip p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) +
                  " FROM " + TABLOAD;

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
            foreach (GorevTertip p in liste.objeler)
            {
                sql = string.Empty;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(gorevKod) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(p.tertip) + ",";
                sql += VeriErisim.DegerYap(p.kullanilacakTutar) + ")";

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
        /// Silme işlemini yapan fonksiyon
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

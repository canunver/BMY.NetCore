using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABSistem : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABSistem sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABSistem(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRSISTEM";

        public enum ENUMAlanlar : int { DIGERUSTVERILER = 0 }

        public static string[] ALANLAR = { "DIGERUSTVERILER" };


        public Sonuc Kaydet(string digerUstVeriler)
        {
            Sonuc sonuc = Sil();

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(digerUstVeriler) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil()
        {
            return KomutCalistir("DELETE FROM " + TABLOAD, 0);
        }

        public string Oku()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            return veriErisim.TekKolonSQLStr(sql);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.ORG.IsKurallari.Veri
{
    /// <summary>
    /// Görev Personel veri tabanı tablosu ile ilgili olarak listeleme, kaydetme ve silme işlemini yapan sınıf
    /// </summary>
    public class TABGorevPersonel : TABOrtak
    {
        /// <summary>
        ///  Sınıfın oluşturucu(Constructer) fonksiyonu
        /// </summary>
        /// <param name="veriErisim">Veri Erişim tipinde sınıf</param>
        public TABGorevPersonel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adı
        /// </summary>
        static string TABLOAD = "PERGOREVPERSONEL";

        /// <summary>
        /// Tablo alanları indisi
        /// </summary>
        enum ENUMAlanlar : int
        {
            /// <summary>
            /// Görev Kodu
            /// </summary>
            GOREVKOD = 0,
            /// <summary>
            /// T.C. Kimlik Numarası
            /// </summary>
            TCKIMLIKNO = 1,
            /// <summary>
            /// Adı
            /// </summary>
            AD = 2,
            /// <summary>
            /// Soyadı
            /// </summary>
            SOYAD = 3,
            /// <summary>
            /// Ünvan Adı
            /// </summary>
            UNVANAD = 4,
            /// <summary>
            /// Ünvan Kodu
            /// </summary>
            UNVANKOD = 5,
            /// <summary>
            /// Kadro Derecesi
            /// </summary>
            KADRODERECE = 6,
            /// <summary>
            /// Ek Gösterge
            /// </summary>
            EKGOSTERGE = 7,
            /// <summary>
            /// Avans Tutarı
            /// </summary>
            AVANSTUTARI = 8,
            /// <summary>
            /// Avans Para Birimi Kodu
            /// </summary>
            AVANSPARABIRIMKOD = 9,
            /// <summary>
            /// Süreç Yıl
            /// </summary>
            SURECYIL = 10,
            /// <summary>
            /// Süreç Numarası
            /// </summary>
            SURECNO = 11
        }

        /// <summary>
        /// Tablo alanları
        /// </summary>
        static string[] ALANLAR = { "GOREVKOD", 
                                    "TCKIMLIKNO", 
                                    "AD", 
                                    "SOYAD", 
                                    "UNVANAD", 
                                    "UNVANKOD", 
                                    "KADRODERECE", 
                                    "EKGOSTERGE",
                                    "AVANSTUTARI",
                                    "AVANSPARABIRIMKOD",
                                    "SURECYIL",
                                    "SURECNO" };

        /// <summary>
        /// Listeleme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="p">Görev Personel tipinde sınıf</param>
        /// <returns>IDataReader tipinde sınıf döndürür</returns>
        public IDataReader Listele(GorevPersonel p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.gorevKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREVKOD], "=", p.gorevKod));

            if (p.tckimlikno != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tckimlikno));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kaydetme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="p">Görev Personel tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(GorevPersonel p)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.gorevKod) + ",";
            sql += VeriErisim.DegerYap(p.tckimlikno) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ",";
            sql += VeriErisim.DegerYap(p.soyad) + ",";
            sql += VeriErisim.DegerYap(p.unvan) + ",";
            sql += VeriErisim.DegerYap(p.unvanKod) + ",";
            sql += VeriErisim.DegerYap(p.kadroDerece) + ",";
            sql += VeriErisim.DegerYap(p.ekGosterge) + ",";
            sql += VeriErisim.DegerYap(p.avansTutari) + ",";
            sql += VeriErisim.DegerYap(p.avansParaBirimKod) + ",";
            sql += VeriErisim.DegerYap(p.surecYil) + ",";
            sql += VeriErisim.DegerYap(p.surecNo) + ")";

            sonuc = KomutCalistir(sql, 1);


            return sonuc;
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

            foreach (GorevPersonel p in liste.objeler)
            {
                sql = string.Empty;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(gorevKod) + ",";
                sql += VeriErisim.DegerYap(p.tckimlikno) + ",";
                sql += VeriErisim.DegerYap(p.ad) + ",";
                sql += VeriErisim.DegerYap(p.soyad) + ",";
                sql += VeriErisim.DegerYap(p.unvan) + ",";
                sql += VeriErisim.DegerYap(p.unvanKod) + ",";
                sql += VeriErisim.DegerYap(p.kadroDerece) + ",";
                sql += VeriErisim.DegerYap(p.ekGosterge) + ",";
                sql += VeriErisim.DegerYap(p.avansTutari) + ",";
                sql += VeriErisim.DegerYap(p.avansParaBirimKod) + ",";
                sql += VeriErisim.DegerYap(p.surecYil) + ",";
                sql += VeriErisim.DegerYap(p.surecNo) + ")";

                sonuc = KomutCalistir(sql, 1);

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

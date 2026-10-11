using System;
using System.Data;
using TNS.BKD;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Hesap planı bilgilerini tutan TASHESAPPLAN veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABHesapPlani : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHesapPlani(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TASHESAPPLAN";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Açıklama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 1,
            /// <summary>
            /// Seviye alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SEVIYE = 2,
            /// <summary>
            /// Detay alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DETAY = 3,
            /// <summary>
            /// Ölçü birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMKOD = 4,
            /// <summary>
            /// Kdv alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDV = 5,
            /// <summary>
            /// Kullanılmıyor alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANILMIYOR = 6,
            /// <summary>
            /// Güncelleme alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GUNCELLEME = 7,
            /// <summary>
            /// Numara alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NUMARA = 8,

            /// <summary>
            /// Numara alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMORTIYIL = 9,

            RFIDETIKETKOD = 10,
            MARKAKOD = 11,
            MODELKOD = 12,
            KISTAMORTISMAN = 13,
            VURGULA = 14,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "ACIKLAMA", "SEVIYE", "DETAY", "OLCUBIRIMKOD", "KDV", "KULLANILMIYOR", "GUNCELLEME", "NUMARA", "AMORTIYIL", "RFIDETIKETKOD", "MARKAKOD", "MODELKOD", "KISTAMORTISMAN", "VURGULA" };

        /// <summary>
        /// Verilen taşınır hesap planı bilgilerini
        /// veri tabanına kaydeden veya güncelleyen,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapPlani">Hesap Planı bilgisini tutan, ObjectArray türünde nesne</param>
        /// <returns>Ekleme,Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc HesapKodKaydet(ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABTASHesapPlani);

            foreach (HesapPlaniSatir hs in hesapPlani.objeler)
            {
                int seviye = 0;

                hs.hesapKod = hs.hesapKod.Replace(".", "");
                hs.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hs.hesapKod);
                string[] st = hs.hesapKod.Split('.');
                seviye = st.Length;
                hs.hesapKod = hs.hesapKod.Replace(".", "");

                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(hs.hesapKod) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.aciklama) + ", ";
                sql = sql + VeriErisim.DegerYap(seviye) + ", ";
                sql = sql + VeriErisim.DegerYap(0) + ", ";
                if (hs.olcuBirim > -1)
                    sql = sql + VeriErisim.DegerYap(hs.olcuBirim) + ",";
                else
                    sql = sql + VeriErisim.DegerYap(0) + ",";
                if (hs.kdv > -1)
                    sql = sql + VeriErisim.DegerYap(hs.kdv) + ",";
                else
                    sql = sql + VeriErisim.DegerYap(0) + ",";
                sql = sql + VeriErisim.DegerYap(hs.kullanilmiyor) + ",";
                sql = sql + VeriErisim.DegerYap(hs.guncelleme) + ",";
                sql = sql + VeriErisim.DegerYap(hs.numara) + ",";
                sql = sql + VeriErisim.DegerYap(hs.amortiYil) + ",";

                if (hs.rfidEtiketKod > -1)
                    sql = sql + VeriErisim.DegerYap(hs.rfidEtiketKod) + ",";
                else
                    sql = sql + VeriErisim.DegerYap(0) + ",";

                if (hs.markaKod > -1)
                    sql = sql + VeriErisim.DegerYap(hs.markaKod) + ",";
                else
                    sql = sql + VeriErisim.DegerYap(0) + ",";

                if (hs.modelKod > -1)
                    sql = sql + VeriErisim.DegerYap(hs.modelKod) + ",";
                else
                    sql = sql + VeriErisim.DegerYap(0) + ",";

                sql = sql + VeriErisim.DegerYap(hs.kistAmortisman) + ",";
                sql = sql + VeriErisim.DegerYap(hs.vurgula) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + VeriErisim.DegerYap(hs.hesapKod);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + "=" + VeriErisim.DegerYap(hs.aciklama);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD] + "=" + hs.olcuBirim;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.KDV] + "=" + hs.kdv;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.GUNCELLEME] + "=" + hs.guncelleme;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.KULLANILMIYOR] + "=" + hs.kullanilmiyor;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.NUMARA] + "=" + VeriErisim.DegerYap(hs.numara);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.AMORTIYIL] + "=" + hs.amortiYil + "";
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.RFIDETIKETKOD] + "=" + hs.rfidEtiketKod + "";
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.MARKAKOD] + "=" + hs.markaKod + "";
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.MODELKOD] + "=" + hs.modelKod + "";
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.KISTAMORTISMAN] + "=" + hs.kistAmortisman + "";
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.VURGULA] + "=" + hs.vurgula + "";
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hs.hesapKod);

                    sonuc = KomutCalistir(sql, 1);
                }
                else//Sadece yeni kayıtta işlemi yap
                    HesapPlaniDetaySayilariDuzenle(hs.hesapKod);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Hesap Plani bilgisini kullanarak
        /// Verilen taşınır hesap planı bilgilerini varsa güncelleyen yoksa kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapPlani">Hesap Planı bilgilerini tutan, ObjectArray türünde nesne</param>
        /// <returns>Ekleme,Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc HesapKodKaydetMuhasebat(ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABTASHesapPlani);

            foreach (HesapPlaniSatir hs in hesapPlani.objeler)
            {
                int seviye = 0;

                hs.hesapKod = hs.hesapKod.Replace(".", "");
                hs.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hs.hesapKod);
                string[] st = hs.hesapKod.Split('.');
                seviye = st.Length;
                hs.hesapKod = hs.hesapKod.Replace(".", "");

                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(hs.hesapKod) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.aciklama) + ", ";
                sql = sql + VeriErisim.DegerYap(seviye) + ", ";
                sql = sql + VeriErisim.DegerYap(0) + ", ";
                if (hs.olcuBirim > -1)
                    sql = sql + VeriErisim.DegerYap(hs.olcuBirim) + ",";
                else
                    sql = sql + VeriErisim.DegerYap(0) + ",";
                if (hs.kdv > -1)
                    sql = sql + VeriErisim.DegerYap(hs.kdv) + ",";
                else
                    sql = sql + VeriErisim.DegerYap(0) + ",";
                sql = sql + VeriErisim.DegerYap(hs.kullanilmiyor) + ",";
                sql = sql + VeriErisim.DegerYap(hs.guncelleme) + ",";
                sql = sql + VeriErisim.DegerYap("") + ",";
                sql = sql + VeriErisim.DegerYap("0") + ",";

                if (hs.rfidEtiketKod > -1)
                    sql = sql + VeriErisim.DegerYap(hs.rfidEtiketKod) + ",";
                else
                    sql = sql + VeriErisim.DegerYap(0) + ",";

                if (hs.markaKod > -1)
                    sql = sql + VeriErisim.DegerYap(hs.markaKod) + ",";
                else
                    sql = sql + VeriErisim.DegerYap(0) + ",";

                if (hs.modelKod > -1)
                    sql = sql + VeriErisim.DegerYap(hs.modelKod) + ",";
                else
                    sql = sql + VeriErisim.DegerYap(0) + ",";

                sql = sql + VeriErisim.DegerYap(0) + ",";
                sql = sql + VeriErisim.DegerYap(0) + ")";


                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + VeriErisim.DegerYap(hs.hesapKod);
                    if (hs.aciklama.Trim() != "")
                        sql += ", " + ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + "=" + VeriErisim.DegerYap(hs.aciklama);
                    if (hs.olcuBirim > -1)
                        sql += ", " + ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD] + "=" + hs.olcuBirim;
                    if (hs.kdv > -1)
                        sql += ", " + ALANLAR[(int)ENUMAlanlar.KDV] + "=" + hs.kdv;
                    if (hs.kullanilmiyor > -1)
                        sql += ", " + ALANLAR[(int)ENUMAlanlar.KULLANILMIYOR] + "=" + hs.kullanilmiyor;
                    if (hs.guncelleme > -1)
                        sql += ", " + ALANLAR[(int)ENUMAlanlar.GUNCELLEME] + "=" + hs.guncelleme;
                    if (hs.vurgula > -1)
                        sql += ", " + ALANLAR[(int)ENUMAlanlar.VURGULA] + "=" + hs.vurgula;
                    if (hs.rfidEtiketKod > -1)
                        sql += ", " + ALANLAR[(int)ENUMAlanlar.RFIDETIKETKOD] + "=" + hs.rfidEtiketKod;
                    if (hs.markaKod > -1)
                        sql += ", " + ALANLAR[(int)ENUMAlanlar.MARKAKOD] + "=" + hs.markaKod;
                    if (hs.modelKod > -1)
                        sql += ", " + ALANLAR[(int)ENUMAlanlar.MODELKOD] + "=" + hs.modelKod;

                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hs.hesapKod);
                    sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUNCELLEME], "=", 0);

                    sonuc = KomutCalistir(sql, 0);
                }

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = string.Format(TasinirMalResI.TMM001, hs.hesapKod);
                    return sonuc;
                }
            }

            HesapPlaniDetaySayilariDuzenle("");

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Hesap Planı bilgilerini kullanarak
        /// ilgili Hesap Plan bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapPlani">Hesap Planı bilgilerini tutan, ObjectArray türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc HesapKodSil(ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (HesapPlaniSatir hs in hesapPlani.objeler)
            {
                string sql = "";

                sql = "DELETE FROM " + TABLOAD + " WHERE ";
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hs.hesapKod);

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            HesapPlaniDetaySayilariDuzenle("");

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Hesap Planı Satır bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Hesap Plan bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesKod">Hesap Planı Satır bilgisini tutan, HesapPlaniSatir türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Hesap Plan bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader HesapKodListele(HesapPlaniSatir hesKod, Sayfalama sayfalama)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT TASHESAPPLAN.*, TASOLCUBIRIM.AD OLCUBIRIMAD ";
            sql += " FROM TASHESAPPLAN LEFT OUTER JOIN ";
            sql += " TASOLCUBIRIM ON TASHESAPPLAN.OLCUBIRIMKOD = TASOLCUBIRIM.KOD ";
            if (hesKod.hesapKoduBarkod != null && !string.IsNullOrEmpty(hesKod.hesapKoduBarkod.barkod))
                sql += " LEFT OUTER JOIN TASHESAPPLANBARKOD ON TASHESAPPLANBARKOD.TASHESAPNO = TASHESAPPLAN.KOD ";

            if (hesKod.hesapKodAciklama == "" || hesKod.hesapKod == "094")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], " LIKE ", hesKod.hesapKod.Replace(".", "").Trim() + "%"));

            if (hesKod.aciklama != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], " LIKE ", "%" + hesKod.aciklama + "%"));
            if (hesKod.seviye != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SEVIYE], "=", hesKod.seviye));
            if (hesKod.detay == true)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DETAY], "=", 0));
            if (hesKod.olcuBirim > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD], "=", hesKod.olcuBirim));
            if (hesKod.numara != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.NUMARA], "=", hesKod.numara));
            if (hesKod.guncelleme > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GUNCELLEME], "=", hesKod.guncelleme));
            if (hesKod.vurgula > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VURGULA], "=", hesKod.vurgula));
            if (hesKod.rfidEtiketKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.RFIDETIKETKOD], "=", hesKod.rfidEtiketKod));
            if (hesKod.markaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MARKAKOD], "=", hesKod.markaKod));
            if (hesKod.modelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MODELKOD], "=", hesKod.modelKod));
            if (hesKod.modelKod > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KISTAMORTISMAN], "=", hesKod.kistAmortisman));

            if (hesKod.kullanilmiyor != 2)//Sayımda kapalı hesap kodu yazılmış ise gelsin
            {
                if (hesKod.kullanilmiyor > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANILMIYOR], "=", hesKod.kullanilmiyor));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANILMIYOR], "=", 0));
            }

            if (hesKod.hesapKoduBarkod != null && !string.IsNullOrEmpty(hesKod.hesapKoduBarkod.barkod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASHESAPPLANBARKOD", "BARKOD", "=", hesKod.hesapKoduBarkod.barkod));

            if (hesKod.hesapKodAciklama != "")
            {
                if (hesKod.hesapKodAciklama.StartsWith("@"))
                {
                    hesKod.hesapKodAciklama = hesKod.hesapKodAciklama.Replace("@", "");
                    string[] aranan = hesKod.hesapKodAciklama.Split(' ');
                    string ekKosul = "";
                    for (int i = 0; i < aranan.Length; i++)
                    {
                        if (aranan[i] == "") continue;
                        ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", aranan[i].Replace(".", "") + "%"));
                    }
                    if (ekKosul != "")
                        kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + ekKosul + ")");
                }
                else if (hesKod.hesapKodAciklama.StartsWith("!"))
                {
                    hesKod.hesapKodAciklama = hesKod.hesapKodAciklama.Replace("!", "");
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", hesKod.hesapKodAciklama));
                }
                else
                {
                    string ekKosul = string.Empty;
                    string[] aranan = hesKod.hesapKodAciklama.Split(' ');

                    foreach (string ara in aranan)
                    {
                        if (string.IsNullOrEmpty(ara.Trim()))
                            continue;

                        //ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], " LIKE ", ara.Replace(".", "").Trim() + "%"));
                        //ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], " LIKE ", "%" + ara + "%"));

                        //if (!string.IsNullOrWhiteSpace(ekKosul))
                        //    sql += " AND (" + ekKosul + ")";

                        kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], " LIKE ", ara.Replace(".", "").Trim() + "%"));
                        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], " LIKE ", "%" + ara + "%"));
                        kosul += ")";
                    }
                }
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            if (string.IsNullOrEmpty(sayfalama.siralamaAlani))
                sayfalama.siralamaAlani = "KOD";
            string siralama = " ORDER BY TASHESAPPLAN." + sayfalama.siralamaAlani + " " + sayfalama.siralamaYon;

            sql = TABGenel.GetPaginatedSQL(sayfalama.sayfaNo, sayfalama.kayitSayisi, sql, siralama);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Hesap Planı Satır bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun hesap planının bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hKod">Hesap Planı Satır bilgilerini tutan, HesapPlaniSatir türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Hesap Planı bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        //public HesapPlaniSatir HesapKodOzellikAl(HesapPlaniSatir hKod)
        //{
        //    string sql = "";
        //    Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABTASHesapPlani);

        //    hKod.hesapKod = hKod.hesapKod.Replace(".", "");

        //    sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hKod.hesapKod));

        //    IDataReader r = veriErisim.DataReaderDondur(sql);

        //    hKod.hesapKod = "";
        //    if (veriErisim.hata == "")
        //    {
        //        if (r.Read())
        //        {
        //            hKod.hesapKod = VeriErisim.DegerAl(r, 0, "");
        //            hKod.aciklama = VeriErisim.DegerAl(r, 1, "");
        //            hKod.seviye = VeriErisim.DegerAl(r, 2, 0);
        //            hKod.detay = VeriErisim.DegerAl(r, 3, 0) == 0;
        //        }
        //    }

        //    hKod.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hKod.hesapKod.Replace(".", ""));

        //    r.Close();

        //    return hKod;
        //}

        /// <summary>
        /// Parametre olarak verilen Hesap Kodu bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun verilen hesap kodunun detay bilgileri olup olmadığını döndüren veya detay ara parametresine göre kendisini olup olmadığını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapKod">Hesap kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="detayAra">Detay ara yapılıp yapılmayacağı bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, detay bilginin olup olmadığını tutan, var ise true yok ise false bilgisini tutan Bool türünde nesne döndürür</returns>
        public bool HesapKodAnasiCocuguVarMi(string hesapKod, bool detayAra)
        {
            string sql = "";
            bool varMi = false;

            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            if (detayAra)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", hesapKod.Replace(".", "") + "%"));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hesapKod.Replace(".", "")));

            IDataReader r = veriErisim.DataReaderDondur(sql);

            if (veriErisim.hata == "")
                if (r.Read())
                    if (detayAra)
                        varMi = VeriErisim.DegerAl(r, 0, 0) >= 2;
                    else
                        varMi = VeriErisim.DegerAl(r, 0, 0) >= 1;

            r.Close();

            return varMi;
        }

        /// <summary>
        /// Parametre olarak verilen Hesap Kodu bilgisini kullanarak,
        /// verilen taşınır hesap planı kodunun detay alanını, detay hesap kodu sayısını bularak güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hKod">Hesap Kodu bilgisini tutan, String türünde nesne</param>
        public void HesapPlaniDetaySayilariDuzenle(string hKod)
        {
            string sql;
            string bulHKod = "";
            Sonuc sonuc = new Sonuc(false, "", "");
            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABTASHesapPlani);

            hKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hKod);
            bulHKod = hKod;
            if (hKod.LastIndexOf('.') > 0)
                bulHKod = hKod.Substring(0, hKod.LastIndexOf('.'));
            hKod = hKod.Replace(".", "");
            bulHKod = bulHKod.Replace(".", "");

            if (bulHKod.Trim() != "")
            {
                sql = "UPDATE " + TABLOAD;
                sql += " SET DETAY=(SELECT COUNT(*) FROM " + TABLOAD + " B";
                sql += " WHERE B.KOD LIKE " + TABLOAD + ".KOD" + VeriErisim.IslecBul("+") + "'%' AND ";
                sql += " B.SEVIYE>" + TABLOAD + ".SEVIYE) WHERE KOD LIKE '" + bulHKod + "%'";
                sonuc = KomutCalistir(sql, 0);
            }
            else if (hKod.Trim() == "")
            {
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=2 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=5 AND SEVIYE<>2";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=3 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=7 AND SEVIYE<>3";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=4 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=9 AND SEVIYE<>4";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=5 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=11 AND SEVIYE<>5";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=6 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=13 AND SEVIYE<>6";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=7 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=15 AND SEVIYE<>7";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=8 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=17 AND SEVIYE<>8";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=9 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=19 AND SEVIYE<>9";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=10 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=21 AND SEVIYE<>10";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=11 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=23 AND SEVIYE<>11";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=12 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=25 AND SEVIYE<>12";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=13 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)=27 AND SEVIYE<>13";
                sonuc = KomutCalistir(sql, 0);

                sql = "UPDATE " + TABLOAD + " SET SEVIYE=0 WHERE KOD IS NULL";
                sonuc = KomutCalistir(sql, 0);
                sql = "UPDATE " + TABLOAD + " SET SEVIYE=0 WHERE " + VeriErisim.IslecBul("LEN") + "(KOD)<3";
                sonuc = KomutCalistir(sql, 0);

                string[] hk = { "150", "253", "254", "255" };
                for (int i = 0; i < hk.Length; i++)
                {
                    if (!hKod.StartsWith(hk[i])) continue;

                    sql = "UPDATE " + TABLOAD;
                    sql += " SET DETAY=(SELECT COUNT(*) FROM " + TABLOAD + " B";
                    sql += " WHERE B.KOD LIKE " + TABLOAD + ".KOD" + VeriErisim.IslecBul("+") + "'%' AND ";
                    sql += " B.SEVIYE>" + TABLOAD + ".SEVIYE) WHERE KOD LIKE '" + hk[i] + "%'";
                    sonuc = KomutCalistir(sql, 0);
                }

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    sql = "UPDATE " + TABLOAD;
                    sql += " SET DETAY=(SELECT COUNT(*) FROM " + TABLOAD + " B";
                    sql += " WHERE B.KOD LIKE " + TABLOAD + ".KOD" + VeriErisim.IslecBul("+") + "'%' AND ";
                    sql += " B.SEVIYE>" + TABLOAD + ".SEVIYE)";
                    sonuc = KomutCalistir(sql, 0);
                }
            }
            return;
        }

        /// <summary>
        /// Parametre olarak verilen Hesap Plani Satir bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Verilen taşınır hesap planının taşınır işlem fişlerinde kullanılıp kullanılmadığını, kullandıysa ilgili taşınır işlem fişlerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hKod">Hesap Plani Satır bilgisini tutan, HesapPlaniSatir türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Fişleri bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader KullanilmisMi(HesapPlaniSatir hKod)
        {
            string sql = string.Empty;

            sql = "SELECT TASISLEMDETAY.YIL, TASISLEMDETAY.MUHASEBEKOD, TASISLEMDETAY.HARCAMABIRIMKOD,";
            sql += " TASISLEMDETAY.FISNO, SIRANO FROM TASISLEMDETAY JOIN TASISLEMFORM ON ";
            sql += " TASISLEMFORM.YIL = TASISLEMDETAY.YIL AND TASISLEMFORM.MUHASEBEKOD = TASISLEMDETAY.MUHASEBEKOD AND ";
            sql += " TASISLEMFORM.HARCAMABIRIMKOD  = TASISLEMDETAY.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO = TASISLEMDETAY.FISNO ";
            sql += " WHERE HESAPPLANKOD = '" + hKod.hesapKod.Replace(".", "") + "'";
            sql += " AND TASISLEMFORM.DURUM <9";
            sql += " ORDER BY TASISLEMDETAY.YIL,TASISLEMDETAY.MUHASEBEKOD,TASISLEMDETAY.HARCAMABIRIMKOD,TASISLEMDETAY.FISNO DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc AmortismanSureKaydet(HesapPlaniSatir sure)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET AMORTIYIL= " + VeriErisim.DegerYap(sure.amortiYil);
            sql += " WHERE KOD LIKE " + VeriErisim.DegerYap(sure.hesapKod.Replace(".", "") + "%");
            return KomutCalistir(sql, 0);
        }
    }
}
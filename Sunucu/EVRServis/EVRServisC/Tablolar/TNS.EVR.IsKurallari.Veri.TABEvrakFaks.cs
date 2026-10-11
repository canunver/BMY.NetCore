using System;
using TNS.Veri;
using System.Data;

namespace TNS.EVR.IsKurallari.Veri
{

    /// <summary>
    /// Birim bilgilerini içeren EVRFAKS veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABEvrakFaks : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABTasfiye sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakFaks(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tasfiye bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRFAKS";

        /// <summary>
        /// Tasfiye bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlar
        {
            KODU = 0,
            BIRIMKOD = 1,
            KIMDEN = 2,
            KIME = 3,
            ACIKLAMA = 4,
            FAKSNO = 5,
            ONCELIK = 6,
            KAYITEDEN = 7,
            KAYITZAMANI = 8,
            TEYIT = 9,
            GONDERILMEZAMANI = 10,
            DURUMU = 11,
            TEKRARSAYISI = 12,
            KAPAKSAYFASI = 13,
            SAYFASAYISI = 14,
            KONU = 15,

        }

        /// <summary>
        /// faks bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KODU", "BIRIMKOD", "KIMDEN", "KIME", "ACIKLAMA", "FAKSNO", "ONCELIK", "KAYITEDEN", "KAYITZAMANI", "TEYIT", "GONDERILMEZAMANI", "DURUMU", "TEKRARSAYISI", "KAPAKSAYFASI", "SAYFASAYISI", "KONU" };

        /// <summary>
        /// Parametre olarak verilen faks nesne koleksiyonuna göre; faks bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Tasfiye nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            foreach (EvrakFaks bilgi in liste.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(bilgi.kodu) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.birimKod) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kimden) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kime) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.aciklama) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.faksNo) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.oncelik) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kayitEden) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kayitZamani) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.teyit) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.gonderilmeZamani) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.durumu) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.tekrarSayisi) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kapakSayfasi) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.sayfaSayisi) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.konu) + ")";

                sonuc = KomutCalistir(sql, 1);
                sonuc.anahtar = bilgi.kodu;
            }
            if (!sonuc.islemSonuc)
                return sonuc;
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen faks nesne koleksiyonuna göre; faks bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Tasfiye nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc GuncelleDurum(ObjectArray liste, string islem)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            foreach (EvrakFaks bilgi in liste.objeler)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                if (bilgi.durumu != (int)(ENUMEvrakFaksDurumu.TEKRARGONDER))
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUMU], bilgi.durumu) + " ";

                else if (bilgi.durumu == (int)(ENUMEvrakFaksDurumu.TEKRARGONDER)) //tekrar gönder
                {
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUMU], (int)(ENUMEvrakFaksDurumu.GONDERILDI)) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKRARSAYISI], bilgi.tekrarSayisi) + " ";
                }

                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODU], "=", bilgi.kodu);

                sonuc = KomutCalistir(sql, 1);
            }
            if (!sonuc.islemSonuc)
                return sonuc;
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen faks nesne koleksiyonuna göre; faks bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Tasfiye nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc GuncelleOncelik(ObjectArray liste, string islem)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            foreach (EvrakFaks bilgi in liste.objeler)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONCELIK], bilgi.oncelik) + " ";
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODU], "=", bilgi.kodu);

                sonuc = KomutCalistir(sql, 1);
            }
            if (!sonuc.islemSonuc)
                return sonuc;
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen faks nesne koleksiyonuna göre; faks bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Tasfiye nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc GuncelleDurumWebServis(ObjectArray liste, string islem)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            foreach (EvrakFaks bilgi in liste.objeler)
            {
                sql = "UPDATE " + TABLOAD + " SET ";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GONDERILMEZAMANI], bilgi.gonderilmeZamani) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKRARSAYISI], bilgi.tekrarSayisi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUMU], (int)(ENUMEvrakFaksDurumu.GONDERILIYOR)) + " ";
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODU], "=", bilgi.kodu);

                sonuc = KomutCalistir(sql, 1);
            }
            if (!sonuc.islemSonuc)
                return sonuc;
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Tasfiye nesne koleksiyonuna göre; Tasfiye bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Tasfiye nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(string kodu)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODU], "=", kodu);

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
        public IDataReader Listele(EvrakFaks bilgi)
        {
            string sql = "";
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.birimKod != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birimKod));
            if (bilgi.kimden != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMDEN], "=", bilgi.kimden));
            if (bilgi.kime != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIME], "=", bilgi.kime));
            if (bilgi.aciklama != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "=", bilgi.aciklama));
            if (bilgi.faksNo != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FAKSNO], "=", bilgi.faksNo));
            if (bilgi.oncelik != 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONCELIK], "=", bilgi.oncelik));
            if (bilgi.kayitEden != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITEDEN], "=", bilgi.kayitEden));
            if (bilgi.teyit != 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TEYIT], "=", bilgi.teyit));
            if (!bilgi.gonderilmeZamani.isNull)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GONDERILMEZAMANI], "=", bilgi.gonderilmeZamani));
            if (bilgi.durumu != 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUMU], "=", bilgi.durumu));
            if (bilgi.tekrarSayisi != 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TEKRARSAYISI], "=", bilgi.tekrarSayisi));
            if (bilgi.kapakSayfasi != 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAPAKSAYFASI], "=", bilgi.kapakSayfasi));
            if (bilgi.sayfaSayisi != 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYFASAYISI], "=", bilgi.sayfaSayisi));
            if (bilgi.konu != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KONU], "=", bilgi.konu));
            if (kriter != "")
                sql += " WHERE " + kriter;
            sql += " ORDER BY ONCELIK ASC,TEKRARSAYISI ASC,GONDERILMEZAMANI DESC";

            return veriErisim.DataReaderDondur(sql);

        }
    }
}

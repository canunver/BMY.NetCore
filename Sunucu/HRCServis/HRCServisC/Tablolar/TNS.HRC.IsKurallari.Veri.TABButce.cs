using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe işlemlerine ait bilgileri tutan 
    /// HRCBUTCEGIDER, HRCBUTCEHARCAMA, HRCBUTCEODENEKGONDERME, HRCMUHASEBEBIRIMTERTIP veri tabanı
    /// tabloları ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABButce : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABButce(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Bütçe Gider tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADBUTCEGIDER = "HRCBUTCEGIDER";

        /// <summary>
        /// Bütçe Gider tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarBUTCEGIDER
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,
            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 2,
            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 3,
            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 4,
            /// <summary>
            /// KANUN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KANUN = 5,
            /// <summary>
            /// KBO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KBO = 6,
            /// <summary>
            /// EKLENEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKLENEN = 7,
            /// <summary>
            /// DUSULEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DUSULEN = 8,
            /// <summary>
            /// SERBEST alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SERBEST = 9,
        }

        /// <summary>
        /// Bütçe Gider tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBUTCEGIDER = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "KANUN", "KBO", "EKLENEN", "DUSULEN", "SERBEST" };

        /// <summary>
        /// Bütçe Harcama tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADBUTCEHARCAMA = "HRCBUTCEHARCAMA";

        /// <summary>
        /// Bütçe Harcama tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarBUTCEHARCAMA
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// AY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AY = 1,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 4,
            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 5,
            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 6,
            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 7,
            /// <summary>
            /// HARCAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HARCAMA = 8,
        }

        /// <summary>
        /// Bütçe Harcama tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBUTCEHARCAMA = { "YIL", "AY", "MUHASEBEKOD", "BIRIMKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "HARCAMA" };

        /// <summary>
        /// Bütçe Ödenek Gönderme tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADBUTCEODENEKGONDERME = "HRCBUTCEODENEKGONDERME";

        /// <summary>
        /// Bütçe Ödenek Gönderme tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarBUTCEODENEKGONDERME
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// BELGETUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 1,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 4,
            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 5,
            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 6,
            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 7,
            /// <summary>
            /// MIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 8,
            /// <summary>
            /// FORMDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FORMDURUM = 9,
            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 10,
        }

        /// <summary>
        /// Bütçe Ödenek Gönderme tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBUTCEODENEKGONDERME = { "YIL", "BELGETUR", "MUHASEBEKOD", "BIRIMKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "MIKTAR", "FORMDURUM", "PEBKOD" };

        /// <summary>
        /// Muhasebe Birim view tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMUHASEBEBIRIM = "HRCMUHASEBEBIRIMVIEW";

        /// <summary>
        /// Muhasebe Birim view tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarMUHASEBEBIRIM
        {
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,
            /// <summary>
            /// BIRIMAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMAD = 3,
        }

        /// <summary>
        /// Muhasebe Birim view tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMUHASEBEBIRIM = { "BIRIMKOD", "MUHASEBEKOD", "KURKOD", "BIRIMAD" };

        /// <summary>
        /// Muhasebe Birim Tertip tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMUHASEBEBIRIMTERTIP = "HRCMUHASEBEBIRIMTERTIP";

        /// <summary>
        /// Muhasebe Birim Tertip tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarMUHASEBEBIRIMTERTIP
        {
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 1,
            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,
            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 3,
            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 4,
            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 5,
            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 6,
        }

        /// <summary>
        /// Muhasebe Birim Tertip tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMUHASEBEBIRIMTERTIP = { "MUHASEBEKOD", "BIRIMKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD" };

        /// <summary>
        /// Muhasebe view tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMUHASEBE = "HRCMUHASEBEVIEW";

        /// <summary>
        /// Muhasebe view tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarMUHASEBE
        {
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// MUHASEBEAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEAD = 1,
        }

        /// <summary>
        /// Muhasebe view tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMUHASEBE = { "MUHASEBEKOD", "MUHASEBEAD" };

        /// <summary>
        /// Muhasebe Birim bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <returns>
        /// Muhasebe Birim bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MuhasebeBirimListele(int muhasebe, string birim)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMUHASEBE, ALANLARMUHASEBE) + "," + VeriErisim.TumAlanlar(TABLOADMUHASEBEBIRIM, ALANLARMUHASEBEBIRIM) + " FROM " + TABLOADMUHASEBE + "," + TABLOADMUHASEBEBIRIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", "HRCMUHASEBEVIEW.MUHASEBEKOD=HRCMUHASEBEBIRIMVIEW.MUHASEBEKOD");

            if (muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCMUHASEBEBIRIMVIEW", ALANLARMUHASEBEBIRIM[(int)ENUMAlanlarMUHASEBEBIRIM.MUHASEBEKOD], "=", muhasebe));
            if (birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCMUHASEBEBIRIMVIEW", ALANLARMUHASEBEBIRIM[(int)ENUMAlanlarMUHASEBEBIRIM.BIRIMKOD], "=", birim));


            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Muhasebe Birim Grup bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="kurKod">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <returns>
        /// Muhasebe Birim Grup bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MuhasebeBirimGrupListele(int muhasebe, string birim, string kurKod)
        {
            string sql = "";

            sql = "SELECT HRCMUHASEBEVIEW.MUHASEBEKOD,BIRIMKOD,MUHASEBEAD FROM " + TABLOADMUHASEBE + "," + TABLOADMUHASEBEBIRIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", "HRCMUHASEBEVIEW.MUHASEBEKOD=HRCMUHASEBEBIRIMVIEW.MUHASEBEKOD");

            if (muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBEBIRIM[(int)ENUMAlanlarMUHASEBEBIRIM.MUHASEBEKOD], "=", muhasebe));
            if (birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMUHASEBEBIRIM, ALANLARMUHASEBEBIRIM[(int)ENUMAlanlarMUHASEBEBIRIM.BIRIMKOD], "LIKE", birim + "%"));
            if (kurKod != "")
            {
                string[] dizi = kurKod.Split(';');
                string kosul = "";
                for (int i = 0; i < dizi.Length; i++)
                    if (dizi[i].Trim() != "")
                        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOADMUHASEBEBIRIM, ALANLARMUHASEBEBIRIM[(int)ENUMAlanlarMUHASEBEBIRIM.KURKOD], "LIKE", dizi[i].Replace(".", "") + "%"));

                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            }

            sql += " GROUP BY HRCMUHASEBEVIEW.MUHASEBEKOD,BIRIMKOD,MUHASEBEAD";
            sql += " ORDER BY HRCMUHASEBEVIEW.MUHASEBEKOD,BIRIMKOD,MUHASEBEAD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Muhasebe Birim Tertip bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        /// <returns>
        /// Muhasebe Birim Tertip bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MuhasebeBirimTertipListele(int muhasebe, string birim, string eko)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMUHASEBEBIRIMTERTIP) + " FROM " + TABLOADMUHASEBEBIRIMTERTIP;

            if (muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMUHASEBEBIRIMTERTIP, ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.MUHASEBEKOD], "=", muhasebe));
            if (birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMUHASEBEBIRIMTERTIP, ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.BIRIMKOD], "LIKE", birim + "%"));
            if (eko != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMUHASEBEBIRIMTERTIP, ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.EKOKOD], "LIKE", eko.Replace(".", "") + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 1,2,3,4,5";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Muhasebe Birim Tertip bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek Muhasebe Birim Tertip bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc MuhasebeBirimTertipKaydet(MuhasebeBirimTertip bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADMUHASEBEBIRIMTERTIP + " (" + VeriErisim.TumAlanlar("", ALANLARMUHASEBEBIRIMTERTIP) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.birimKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.fonKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.finKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.ekoKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.pebEkoKod.Replace(".", "")) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOADMUHASEBEBIRIMTERTIP + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.KURKOD], bilgi.kurKod.Replace(".", "")) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.FONKOD], bilgi.fonKod.Replace(".", "")) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.FINKOD], bilgi.finKod.Replace(".", "")) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.EKOKOD], bilgi.ekoKod.Replace(".", "")) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.PEBKOD], bilgi.pebEkoKod.Replace(".", ""));
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.MUHASEBEKOD], "=", bilgi.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.BIRIMKOD], "=", bilgi.birimKod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Muhasebe Birim Tertip bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek Muhasebe Birim Tertip bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc MuhasebeBirimTertipSil(MuhasebeBirimTertip bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADMUHASEBEBIRIMTERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.BIRIMKOD], "=", bilgi.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.KURKOD], "=", bilgi.kurKod));//.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.FONKOD], "=", bilgi.fonKod));//.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.FINKOD], "=", bilgi.finKod));//.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEBIRIMTERTIP[(int)ENUMAlanlarMUHASEBEBIRIMTERTIP.EKOKOD], "=", bilgi.ekoKod));//.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Tertip bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="kur">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <param name="fon">Listeleme için kullanılacak fonksiyonel kod kıstas parametresi</param>
        /// <param name="fin">Listeleme için kullanılacak finansman kod kıstas parametresi</param>
        /// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        /// <returns>
        /// Tertip bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader TertipListele(int yil, string kur, string fon, string fin, string eko)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARBUTCEGIDER) + " FROM " + TABLOADBUTCEGIDER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBUTCEGIDER[(int)ENUMAlanlarBUTCEGIDER.YIL], "=", yil));

            if (kur != "")
            {
                string[] dizi = kur.Split(';');
                string kosul = "";
                for (int i = 0; i < dizi.Length; i++)
                    if (dizi[i].Trim() != "")
                        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARBUTCEGIDER[(int)ENUMAlanlarBUTCEGIDER.KURKOD], "LIKE", dizi[i].Replace(".", "") + "%"));

                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            }
            if (fon != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEGIDER[(int)ENUMAlanlarBUTCEGIDER.FONKOD], "=", fon.Replace(".", "")));
            if (fin != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEGIDER[(int)ENUMAlanlarBUTCEGIDER.FINKOD], "=", fin.Replace(".", "")));
            if (eko != "")
            {
                string[] dizi = eko.Split(';');
                string kosul = "";
                for (int i = 0; i < dizi.Length; i++)
                {
                    //gönderilen eko kod 2 kırılımlı olmalıdır. 
                    if (dizi[i].Replace(".", "").Length > 3)
                        dizi[i] = dizi[i].Replace(".", "").Remove(3);
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARBUTCEGIDER[(int)ENUMAlanlarBUTCEGIDER.EKOKOD], "LIKE", dizi[i].Replace(".", "").Trim() + "%"));
                }
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            }
            sql += " ORDER BY " + ALANLARBUTCEGIDER[(int)ENUMAlanlarBUTCEGIDER.YIL] + "," + ALANLARBUTCEGIDER[(int)ENUMAlanlarBUTCEGIDER.KURKOD] + "," + ALANLARBUTCEGIDER[(int)ENUMAlanlarBUTCEGIDER.FONKOD] + "," + ALANLARBUTCEGIDER[(int)ENUMAlanlarBUTCEGIDER.FINKOD] + "," + ALANLARBUTCEGIDER[(int)ENUMAlanlarBUTCEGIDER.EKOKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Harcama bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="ay">Listeleme için kullanılacak ay kıstas parametresi</param>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="kur">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <param name="fon">Listeleme için kullanılacak fonksiyonel kod kıstas parametresi</param>
        /// <param name="fin">Listeleme için kullanılacak finansman kod kıstas parametresi</param>
        /// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        /// <param name="ekoSeviye">Listeleme için kullanılacak ekonomik kod seviyesi kıstas parametresi</param>
        /// <returns>
        /// Harcama bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader HarcamaListele(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, int ekoSeviye)
        {
            string muhasebatTur = System.Configuration.ConfigurationManager.AppSettings.Get("MuhasebatTur") + "";
            string sql = "";

            sql = "SELECT YIL, MUHASEBEKOD, BIRIMKOD, KURKOD, FONKOD, FINKOD," + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + ekoSeviye + "),SUM(HARCAMA) FROM " + TABLOADBUTCEHARCAMA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.YIL], "=", yil));

            if (OrtakFonksiyonlar.ConvertToInt(muhasebatTur, 0) == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.AY], "=", ay));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.AY], "<=", ay));

            if (muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.MUHASEBEKOD], "=", muhasebe));
            if (birim != "")
            {
                if (birim.IndexOf(',') > 0)
                {
                    string[] birimler = birim.Split(',');
                    string birimKosul = "";
                    for (int i = 0; i < birimler.Length; i++)
                    {
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.BIRIMKOD], "=", birimler[i].Replace(".", "")));
                    }
                    if (birimKosul != "")
                        sql += " AND (" + birimKosul + ")";
                }
                else
                    if (birim.Length < 3)
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.BIRIMKOD], "LIKE", birim + "%"));
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.BIRIMKOD], "=", birim));
            }

            if (kur != "")
            {
                string[] dizi = kur.Split(';');
                string kosul = "";
                for (int i = 0; i < dizi.Length; i++)
                    if (dizi[i].Trim() != "")
                        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.KURKOD], "LIKE", dizi[i].Replace(".", "") + "%"));

                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            }

            if (fon != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.FONKOD], "=", fon.Replace(".", "")));
            if (fin != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.FINKOD], "=", fin.Replace(".", "")));
            if (eko != "")
            {
                string[] dizi = eko.Split(';');
                string kosul = "";
                for (int i = 0; i < dizi.Length; i++)
                    if (dizi[i].Trim() != "")
                        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.EKOKOD], "LIKE", dizi[i].Replace(".", "").Trim() + "%"));

                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            }

            sql += " GROUP BY " + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.YIL] + "," + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.MUHASEBEKOD] + "," + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.BIRIMKOD] + "," + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.KURKOD] + "," + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.FONKOD] + "," + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.FINKOD] + "," + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + ekoSeviye + ")";
            sql += " ORDER BY " + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.YIL] + "," + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.MUHASEBEKOD] + "," + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.BIRIMKOD] + "," + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.KURKOD] + "," + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.FONKOD] + "," + ALANLARBUTCEHARCAMA[(int)ENUMAlanlarBUTCEHARCAMA.FINKOD] + "," + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + ekoSeviye + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ödenek Gönderme bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="kur">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <param name="fon">Listeleme için kullanılacak fonksiyonel kod kıstas parametresi</param>
        /// <param name="fin">Listeleme için kullanılacak finansman kod kıstas parametresi</param>
        /// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        /// <param name="durum">Listeleme için kullanılacak belge durum kıstas parametresi</param>
        /// <returns>
        /// Ödenek Gönderme bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader OdenekGondermeListele(int yil, int muhasebe, string birim, string kur, string fon, string fin, string eko, int durum)
        {
            string sql = "";

            sql = "SELECT YIL, MUHASEBEKOD, BIRIMKOD, KURKOD, FONKOD, FINKOD, EKOKOD, SUM(MIKTAR), PEBKOD FROM " + TABLOADBUTCEODENEKGONDERME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.YIL], "=", yil));

            if (muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.MUHASEBEKOD], "=", muhasebe));
            if (birim != "")
            {
                if (birim.IndexOf(',') > 0)
                {
                    string[] birimler = birim.Split(',');
                    string birimKosul = "";
                    for (int i = 0; i < birimler.Length; i++)
                    {
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.BIRIMKOD], "=", birimler[i].Replace(".", "")));
                    }
                    if (birimKosul != "")
                        sql += " AND (" + birimKosul + ")";
                }
                else
                    if (birim.Length < 3)
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.BIRIMKOD], "LIKE", birim + "%"));
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.BIRIMKOD], "=", birim));
            }
            if (kur != "")
            {
                string[] dizi = kur.Split(';');
                string kosul = "";
                for (int i = 0; i < dizi.Length; i++)
                    if (dizi[i].Trim() != "")
                        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.KURKOD], "LIKE", dizi[i].Replace(".", "") + "%"));

                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            }

            if (fon != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FONKOD], "=", fon.Replace(".", "")));
            if (fin != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FINKOD], "=", fin.Replace(".", "")));
            if (eko != "")
            {
                string[] dizi = eko.Split(';');
                string kosul = "";
                for (int i = 0; i < dizi.Length; i++)
                {
                    //gönderilen eko kod 2 kırılımlı olmalıdır.
                    dizi[i] = dizi[i].Replace(".", "");
                    if (TNS.MUH.Arac.OdenekGondermeTertipYapi(yil) == MUH.ENUMOdenekGondermeTur.eko2)//Eğer MasrafPlanı değilse
                    {
                        if (dizi[i].Length > 3)
                            dizi[i] = dizi[i].Remove(3);
                    }
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.EKOKOD], "LIKE", dizi[i].Replace(".", "").Trim() + "%"));
                }
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            }
            if (durum > 0)
            {
                if (durum == (int)TNS.MUH.ENUMBelgeDurumu.TAHSISVEONAYLI)
                {
                    string kosul = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FORMDURUM], "=", (int)TNS.MUH.ENUMBelgeDurumu.TAHSIS));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FORMDURUM], "=", (int)TNS.MUH.ENUMBelgeDurumu.ONAYLI));
                    sql += " AND (" + kosul + ")";
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FORMDURUM], "=", durum));
            }
            else
            {
                string kosul = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FORMDURUM], "<>", (int)TNS.MUH.ENUMBelgeDurumu.IPTAL));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FORMDURUM], "<>", (int)TNS.MUH.ENUMBelgeDurumu.YENITALEP));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FORMDURUM], "<>", (int)TNS.MUH.ENUMBelgeDurumu.TALEP));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FORMDURUM], "<>", (int)TNS.MUH.ENUMBelgeDurumu.MUHASEBATIADE));
                sql += " AND (" + kosul + ")";
            }
            sql += " GROUP BY " + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.YIL] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.MUHASEBEKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.BIRIMKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.KURKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FONKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FINKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.EKOKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.PEBKOD];
            sql += " ORDER BY " + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.YIL] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.MUHASEBEKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.BIRIMKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.KURKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FONKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.FINKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.EKOKOD] + "," + ALANLARBUTCEODENEKGONDERME[(int)ENUMAlanlarBUTCEODENEKGONDERME.PEBKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        ///// <summary>
        ///// Doğrudan Temin ihale usulü ile yapılmış harcama miktarı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        ///// üretip, çalıştırıp, datareader döndüren yordam.
        ///// </summary>
        ///// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        ///// <param name="kur">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        ///// <param name="fon">Listeleme için kullanılacak fonksiyonel kod kıstas parametresi</param>
        ///// <param name="fin">Listeleme için kullanılacak finansman kod kıstas parametresi</param>
        ///// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        ///// <returns>
        ///// Doğrudan Temin ihale usulü ile yapılmış harcama miktarı bilgisini bulmak için oluşturulan 
        ///// select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür
        ///// </returns>
        //public IDataReader DogrudanTeminMiktariBul(int yil, string kur, string fon, string fin, string eko)
        //{
        //    string sql = "";

        //    sql = "SELECT SUM(HRCODEMEEMRIDETAY.BORC - HRCODEMEEMRIDETAY.ALACAK)";
        //    sql += " FROM HRCIHALEONAYFORM INNER JOIN";
        //    sql += " HRCODEMEEMRIDETAY ON HRCIHALEONAYFORM.YIL = HRCODEMEEMRIDETAY.YIL AND ";
        //    sql += " HRCIHALEONAYFORM.SURECNO = HRCODEMEEMRIDETAY.SURECNO RIGHT OUTER JOIN";
        //    sql += " HRCSUREC ON HRCIHALEONAYFORM.YIL = HRCSUREC.YIL AND HRCIHALEONAYFORM.SURECNO = HRCSUREC.SURECNO";
        //    sql += " WHERE HRCIHALEONAYFORM.IHALEUSULU = 4 AND HRCIHALEONAYFORM.IHALEMADDESI = 4 AND HRCSUREC.SURECDURUM < 9 AND ";
        //    sql += " HRCODEMEEMRIDETAY.YANSITMAHESAPKOD LIKE '830%' AND ";
        //    sql += " HRCIHALEONAYFORM.YIL = " + yil + " AND ";
        //    if (kur != "" && fon != "" && fin != "" && eko != "")
        //        sql += " HRCODEMEEMRIDETAY.TERTIP = '" + kur + "-" + fon + "-" + fin + "-" + eko + "'";

        //    return veriErisim.DataReaderDondur(sql);
        //}
    }
}
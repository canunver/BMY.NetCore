using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Personel bilgisini tutan HRCPARCA veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABPersonel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPersonel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Personel bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONEL = "HRCPERSONEL";

        /// <summary>
        /// Personel bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPersonel : int
        {
            TCKIMLIKNO = 0,
            ADSOYAD = 1,
            UNVAN = 2,
            MUHASEBEKODU = 3,
            BIRIMKODU = 4,
            BANKA = 5,
            SUBE = 6,
            UNVANKODU = 7,
            HESAPNO = 8,
            VERGIDAIRESI = 9,
            KADRODERECE = 10,
            ODEMEESASDERECE = 11,
            ODEMEESASGOSTERGE = 12,
            ODEMEESASEKGOSTERGE = 13,
        }

        /// <summary>
        /// Personel tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONEL = { "TCKIMLIKNO", "ADSOYAD", "UNVAN", "MUHASEBEKODU", "BIRIMKODU", "BANKA", "SUBE", "UNVANKODU", "HESAPNO", "VERGIDAIRESI", "KADRODERECE", "ODEMEESASDERECE", "ODEMEESASGOSTERGE", "ODEMEESASEKGOSTERGE" };

        /// <summary>
        /// Personel Aile bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONELAILE = "HRCPERPERSONELAILEVIEW";

        /// <summary>
        /// Personel Aile bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPersonelAile : int
        {
            /// <summary>
            /// PERSONELTCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERSONELTCKIMLIKNO = 0,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 1,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 2,
            /// <summary>
            /// SOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOYAD = 3,
            /// <summary>
            /// DOGUMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOGUMTARIHI = 4,
            /// <summary>
            /// CINSIYET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CINSIYET = 5,
            /// <summary>
            /// YAKINLIKDERECESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKINLIKDERECESI = 6,
            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 7,
            /// <summary>
            /// SAGLIKKARNENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAGLIKKARNENO = 8,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 9,
            /// <summary>
            /// EVLENMETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVLENMETARIHI = 10,
            /// <summary>
            /// AILECUZDANISERINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AILECUZDANISERINO = 11,
            /// <summary>
            /// COCUKYAKINLIGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            COCUKYAKINLIGI = 12,
            /// <summary>
            /// ANNEADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ANNEADI = 13,
            /// <summary>
            /// BABAADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BABAADI = 14,
        }

        /// <summary>
        /// Personel Aiile tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONELAILE = { "PERSONELTCKIMLIKNO", "TCKIMLIKNO", "AD", "SOYAD", "DOGUMTARIHI", "CINSIYET", "YAKINLIKDERECESI", "DURUM", "SAGLIKKARNENO", "SIRANO", "EVLENMETARIHI", "AILECUZDANISERINO", "COCUKYAKINLIGI", "ANNEADI", "BABAADI" };

        /// <summary>
        /// Personel bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="tcKimlikNo">Listeleme için kullanılacak TC Kimlik Numarası bilgisi kıstas parametresi</param>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod bilgisi kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod bilgisi kıstas parametresi</param>
        /// <returns>
        /// Personel bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(string tcKimlikNo, int muhasebe, string birim)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADPERSONEL, ALANLARPERSONEL) + " FROM " + TABLOADPERSONEL;

            string kosul = "";
            if (muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.MUHASEBEKODU], "=", muhasebe));
            if (birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BIRIMKODU], "=", birim));
            if (tcKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO], "=", tcKimlikNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        public IDataReader Listele(FirmaBilgisi bilgi)
        {
            string sql = "SELECT TCKIMLIKNO, SICILNO, ADSOYAD, UNVAN, CALISMATURU, BANKA, SUBE, HESAPNO, MUHASEBEKODU, BIRIMKODU, BANKAKODU, BANKASUBEKODU, UNVANKODU, SIRANO, ISTIHDAMSEKLI, VERGIDAIRESI, KADRODERECE, AD, SOYAD, DURUM, ODEMEESASDERECE, ODEMEESASGOSTERGE, ODEMEESASEKGOSTERGE FROM HRCPERSONEL";
            string kosul = "";
           
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "MUHASEBEKODU", "=", bilgi.muhasebe));
            else
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "MUHASEBEKODU", ">", 0));

            if (bilgi.ad != "")
            {
                string[] parcalar = bilgi.ad.Split();

                foreach (string parca in parcalar)
                {
                    if (parca.Trim() == "") continue;
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("", "ADSOYAD", "LIKE", "%" + parca + "%"));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", "TCKIMLIKNO", "LIKE", parca + "%"));
                    kosul += ")";
                }
            }

            if (bilgi.vergiNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TCKIMLIKNO", "=", bilgi.vergiNo));

            if (bilgi.vergiDairesi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "VERGIDAIRESI", "LIKE", "%" + bilgi.vergiDairesi + "%"));

            if (bilgi.banka != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "BANKA", "LIKE", "%" + bilgi.banka + "%"));

            if (bilgi.hesapNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "HESAPNO", "LIKE", "%" + bilgi.hesapNo + "%"));
              
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 3";

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Öğrenci Eğitmen bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="tcKimlikNo">Listeleme için kullanılacak TC Kimlik Numarası bilgisi kıstas parametresi</param>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod bilgisi kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod bilgisi kıstas parametresi</param>
        /// <returns>
        /// Öğretmen Eğitmen bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader PersonelOgrenciEgitmenListele(string tcKimlikNo, int muhasebe, string birim)
        {
            string sql = "";

            sql = "SELECT     MUHASEBEKOD, BIRIMKOD , TCKIMLIKNO, ADISOYADI AS ADSOYAD, BANKAKODU, BANKASUBEKODU, " +
                    "BANKAHESAPNO AS HESAPNO " +
                    "FROM         HRCOGRENCIEGITMEN " +
                    "WHERE     (TCKIMLIKNO = " + VeriErisim.DegerYap(tcKimlikNo) + ") ";

            if (muhasebe > 0)
                sql += " AND (MUHASEBEKOD = " + VeriErisim.DegerYap(muhasebe) + ") ";
            if (birim != "")
                sql += " AND (BIRIMKOD IN (" + VeriErisim.DegerYap(birim) + ", '000')) ";

            sql += " UNION ";

            sql += "SELECT     MUHASEBEKODU, BIRIMKODU, TCKIMLIKNO, ADSOYAD, BANKAKODU, BANKASUBEKODU, HESAPNO " +
            "FROM         HRCPERSONEL " +
            "WHERE     (TCKIMLIKNO = " + VeriErisim.DegerYap(tcKimlikNo) + ") ";

            if (muhasebe > 0)
                sql += " AND (MUHASEBEKODU = " + VeriErisim.DegerYap(muhasebe) + ") ";
            if (birim != "")
                sql += " AND (BIRIMKODU=" + VeriErisim.DegerYap(birim) + ") ";

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Personel sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod bilgisi kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod bilgisi kıstas parametresi</param>
        /// <returns>
        /// Personel sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader PersonelSayisi(int muhasebe, string birim)
        {
            string sql = "";

            sql = "SELECT MUHASEBEKODU,BIRIMKODU,SAYI FROM PERBIRIMDAGILIMIVIEW";

            string kosul = "";
            if (muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARPERSONEL[(int)ENUMAlanlarPersonel.MUHASEBEKODU], "=", muhasebe));
            if (birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BIRIMKODU], "LIKE", birim + "%"));
            //if (birim != "")
            //{
            //    if (birim.IndexOf(',') > 0)
            //    {
            //        string[] birimler = birim.Split(',');
            //        string birimKosul = "";
            //        for (int i = 0; i < birimler.Length; i++)
            //        {
            //            birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BIRIMKODU], "LIKE", birimler[i].Replace(".", "") + "%"));
            //        }
            //        if (birimKosul != "")
            //            sql += " AND (" + birimKosul + ")";
            //    }
            //    else
            //        if (birim.Length < 3)
            //            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BIRIMKODU], "LIKE", birim + "%"));
            //        else
            //            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BIRIMKODU], "=", birim));
            //}
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY MUHASEBEKODU,BIRIMKODU";

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Personel Aile bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak personel aile bilgisi kıstas parametresi</param>
        /// <returns>
        /// Personel Aile bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AileListele(PersonelAileBilgi kriter)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADPERSONELAILE, ALANLARPERSONELAILE) + " FROM " + TABLOADPERSONELAILE;

            string kosul = "";
            if (kriter.personelTcKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONELAILE, ALANLARPERSONELAILE[(int)ENUMAlanlarPersonelAile.PERSONELTCKIMLIKNO], "=", kriter.personelTcKimlikNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADPERSONELAILE + "." + ALANLARPERSONELAILE[(int)ENUMAlanlarPersonel.ADSOYAD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
    }
}

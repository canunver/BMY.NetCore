using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCABONEFATURA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABAboneFatura : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAboneFatura(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCABONEFATURA";


        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// FATURAYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURAYIL = 4,
            /// <summary>
            /// FATURAAY alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURAAY = 5,
            /// <summary>
            /// ABONENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ABONENO = 6,
            /// <summary>
            /// ABONETIP alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ABONETIP = 7,
            /// <summary>
            /// FATURATUTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURATUTAR = 8,
            /// <summary>
            /// FAIZMIKTARI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAIZMIKTARI = 9,
            /// <summary>
            /// OZELKULLANIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELKULLANIM = 10,
            /// <summary>
            /// ODEMETARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMETARIHI = 11,
            /// <summary>
            /// ODEMEEMRINO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 12,
            /// <summary>
            /// FATURANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURANO = 13,
            /// <summary>
            /// ISLETMENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLETMENO = 14,
            /// <summary>
            /// ODEMEEMRIYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRIYIL = 15,
            DAMGAVERGISI = 16,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FATURAYIL", "FATURAAY", "ABONENO", "ABONETIP", "FATURATUTAR", "FAIZMIKTARI", "OZELKULLANIM", "ODEMETARIHI", "ODEMEEMRINO", "FATURANO", "ISLETMENO", "ODEMEEMRIYIL", "DAMGAVERGISI" };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADODENEKMAIL = "HRCABONEODENEKGONDERMEMAIL";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarODENEKMAIL
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// EPOSTA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EPOSTA = 1,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 2,
            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARODENEKMAIL = { "KOD", "EPOSTA", "ADSOYAD", "KURKOD" };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSURECMUHMIFBAG = "HRCSURECMUHMIFBAG";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSurecMuhMifBag
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// FORMDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FORMDURUM = 4,
            /// <summary>
            /// SURECDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECDURUM = 5,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSURECMUHMIFBAG = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "FORMDURUM", "SURECDURUM", "ODEMEEMRINO" };

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan, ObjectArray yapısında nesne</param>
        /// <returns>
        /// Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).yil) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).muhasebe) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).birim) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).surecNo) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).faturaYil) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).faturaAy) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).aboneNo) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).aboneTip) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).faturaTutar) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).faizMiktari) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).ozelKullanim) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).odemeTarihi) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).odemeEmriNo) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).faturaNo) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).isletmeKodu) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).odemeEmriYil) + ",";
                sql += VeriErisim.DegerYap(((AboneFatura)bilgi.objeler[i]).damgaVergisi) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Anahtar değişkenleri tutan, AboneFatura yapısında nesne</param>
        /// <returns>
        /// Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(AboneFatura bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ABONETIP], "=", (bilgi.aboneTip)));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Select edilecek kayıtlara ait kıstas bilgisini tutan, AboneFatura yapısında nesne</param>
        /// <returns>
        /// Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(AboneFatura bilgi, bool esitle)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", HRCSUREC.SURECTARIH, MIF.YEVMIYETARIH, MIF.YEVMIYENO, MIF.FORMDURUM FROM " + TABLOAD + " INNER JOIN HRCSUREC ON HRCSUREC.YIL=HRCABONEFATURA.YIL AND HRCSUREC.SURECNO=HRCABONEFATURA.SURECNO";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMIFFORM AS MIF ON MIF.YIL= HRCABONEFATURA.YIL AND MIF.BELGENO = HRCABONEFATURA.ODEMEEMRINO";

            if (string.IsNullOrEmpty(bilgi.surecNo))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "HRCSUREC.SURECDURUM", "<>", 9));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (!string.IsNullOrEmpty(bilgi.birim))
            {
                if (bilgi.birim.Length < 3)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "LIKE", bilgi.birim + "%"));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            }
            if (bilgi.faturaYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURAYIL], "=", bilgi.faturaYil));
            if (bilgi.faturaAy > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURAAY], "=", bilgi.faturaAy));
            if (!string.IsNullOrEmpty(bilgi.aboneNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONENO], "=", bilgi.aboneNo));
            if (bilgi.aboneTip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONETIP], "=", bilgi.aboneTip));
            if (!string.IsNullOrEmpty(bilgi.faturaNo))
            {
                if (esitle)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURANO], "=", bilgi.faturaNo));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURANO], "LIKE", "%" + bilgi.faturaNo + "%"));
            }
            if (bilgi.odemeEmriNo == "HAZIRLANMAMIŞ")//telefon süreçlerine Otomatik ödeme emri kesilmesi sırasında kullanılıyır
            {
                string kkosul = "";
                kkosul = VeriErisim.KosulEkle(kkosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "=", ""));
                kkosul += " OR " + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO] + " IS NULL";//Oracle da null oluyor
                sql += " AND (" + kkosul + ")";
            }
            if (bilgi.mifDurum == -100)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCSUREC", "SURECDURUM", "<", 9));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ABONENO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Abone Faturanın önceden kayıtlı olup olmadığı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="liste">Listeleme için kullanılacak abone fatura kıstas parametresi</param>
        /// <returns>
        /// Abone Faturanın önceden kayıtlı olup olmadığı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader OncedenKayitliMi(ObjectArray liste)
        {
            string sql = "";
            string kosul = "";
            string aboneNolar = "";
            AboneFatura bilgi = (AboneFatura)liste.objeler[0];

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD + ",HRCSUREC";
            kosul = TABLOAD + ".YIL=HRCSUREC.YIL AND " + TABLOAD + ".SURECNO=HRCSUREC.SURECNO AND HRCSUREC.SURECDURUM <> 9";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURATUTAR], ">", 0));

            //if (bilgi.yil > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));

            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            //if (bilgi.birim != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.faturaYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURAYIL], "=", bilgi.faturaYil));
            if (bilgi.faturaAy > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURAAY], "=", bilgi.faturaAy));
            if (bilgi.aboneTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONETIP], "=", bilgi.aboneTip));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "<>", bilgi.surecNo));

            foreach (AboneFatura fatura in liste.objeler)
            {
                //if (fatura.aboneNo != "")
                if (fatura.aboneNo != "" && fatura.faturaTutar > 0)
                    aboneNolar = VeriErisim.KosulEkle(aboneNolar, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONENO], "=", fatura.aboneNo));
            }

            if (kosul != "")
                sql += " WHERE " + kosul + " AND HRCSUREC.SURECDURUM <> 9";
            if (aboneNolar != "")
                sql += " AND (" + aboneNolar + ")";

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ABONENO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Abone fatura ödeme emri belge numarası bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek abone fatura bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdemeEmriNoKaydet(AboneFatura bilgi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRIYIL], bilgi.odemeEmriYil);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAYIL], "=", bilgi.faturaYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAAY], "=", bilgi.faturaAy));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ABONENO], "=", bilgi.aboneNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Abone fatura gsm ödeme bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek abone fatura bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc GsmBilgiKaydet(AboneFatura bilgi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLETMENO], bilgi.isletmeKodu);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ABONETIP], "=", bilgi.aboneTip));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Ödenek gönderme mail bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="mail">Listeleme için kullanılacak abone ödenek gönderme mail bilgisi kıstas parametresi</param>
        /// <returns>
        /// Ödenek gönderme mail bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader OdenekGondermeMailListele(AboneOdenekGondermeMail mail)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARODENEKMAIL) + " FROM " + TABLOADODENEKMAIL;

            if (!string.IsNullOrEmpty(mail.ePosta))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARODENEKMAIL[(int)ENUMAlanlarODENEKMAIL.EPOSTA], "LIKE", "%" + mail.ePosta + "%"));
            if (!string.IsNullOrEmpty(mail.adSoyad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARODENEKMAIL[(int)ENUMAlanlarODENEKMAIL.EPOSTA], "LIKE", "%" + mail.adSoyad + "%"));
            if (!string.IsNullOrEmpty(mail.kurKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARODENEKMAIL[(int)ENUMAlanlarODENEKMAIL.KURKOD], "=", mail.kurKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY KURKOD, EPOSTA";//mail ataılırken kurumsal kod sıralı olması önemli

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ödenek gönderme mail bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="mail">Silinecek ödenek gönderme mail bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdenekGondermeMailSil(AboneOdenekGondermeMail mail)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADODENEKMAIL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARODENEKMAIL[(int)ENUMAlanlarODENEKMAIL.KOD], "=", mail.kod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Ödenek gönderme mail bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="mail">Eklenecek ödenek gönderme mail bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdenekGondermeMailKaydet(AboneOdenekGondermeMail mail)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOADODENEKMAIL + " (" + VeriErisim.TumAlanlar("", ALANLARODENEKMAIL) + ") VALUES (";
            sql += VeriErisim.DegerYap(mail.kod) + ",";
            sql += VeriErisim.DegerYap(mail.ePosta) + ",";
            sql += VeriErisim.DegerYap(mail.adSoyad) + ",";
            sql += VeriErisim.DegerYap(mail.kurKod) + ")";

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Ödenek gönderme mail bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="mail">Güncellenecek ödenek gönderme mail bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdenekGondermeMailGuncelle(AboneOdenekGondermeMail mail)
        {
            string sql = "UPDATE " + TABLOADODENEKMAIL + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARODENEKMAIL[(int)ENUMAlanlarODENEKMAIL.EPOSTA], mail.ePosta) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARODENEKMAIL[(int)ENUMAlanlarODENEKMAIL.ADSOYAD], mail.adSoyad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARODENEKMAIL[(int)ENUMAlanlarODENEKMAIL.KURKOD], mail.kurKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARODENEKMAIL[(int)ENUMAlanlarODENEKMAIL.KOD], "=", mail.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Abone fatura istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak Abone fatura istatistik bilgisi kıstas parametresi</param>
        /// <returns>
        /// Abone fatura istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleIstatistik(AboneFaturaIstatistikSinif kriter)
        {
            string sql = "";
            string kosul = "";
            string groupBy = "";
            string orderBy = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ABONETIP] + "," +
                              "SUM(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FATURATUTAR] + ")," +
                              "COUNT(DISTINCT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FATURAAY] + ")," +
                              "(SUM(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FATURATUTAR] + ") / " +
                              "COUNT(DISTINCT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FATURAAY] + ")) " +

                  "FROM " + TABLOAD + " LEFT OUTER JOIN " + TABLOADSURECMUHMIFBAG + " ON " +
                            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSURECMUHMIFBAG + "." + ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.YIL] + " AND " +
                            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADSURECMUHMIFBAG + "." + ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.MUHASEBEKOD] + " AND " +
                            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADSURECMUHMIFBAG + "." + ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.BIRIMKOD] + " AND " +
                            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSURECMUHMIFBAG + "." + ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.SURECNO];

            if (kriter.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
            if (kriter.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebe));
            if (kriter.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kriter.birim));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSURECMUHMIFBAG, ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.SURECDURUM], "<>", 9));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSURECMUHMIFBAG, ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.FORMDURUM], "<>", 9));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSURECMUHMIFBAG, ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.ODEMEEMRINO], "<>", ""));

            groupBy = "GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ABONETIP];
            orderBy = "ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ABONETIP];

            if (kosul != "")
                sql += " WHERE " + kosul;

            if (groupBy != "")
                sql += groupBy + orderBy;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Abone fatura ödeme emri belge numarası bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="surecNo">Listeleme için kullanılacak süreç numarası bilgisi kıstas parametresi</param>
        /// <returns>
        /// Abone fatura ödeme emri belge numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleOdemeEmri(int yil, string surecNo)
        {
            string sql = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "," +
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO] + "," +
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRIYIL] +
                  " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public string BaskaSurecteFaturaVarMi(string faturaNo, string surecNo, int muhasebe)
        {
            string sql = "SELECT MIN(" + ALANLAR[(int)ENUMAlanlar.SURECNO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURANO], "=", faturaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "<>", surecNo));
            return this.veriErisim.TekKolonSQLStr(sql);
        }
    }
}
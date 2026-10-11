using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BHMV2IOIPROJE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHMV2IOIPROJE : TABOrtak
    {
        /// <summary>
        /// TABBHMV2IOIPROJE sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMV2IOIPROJE(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını (BHMV2IOIPROJE) tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMV2IOIPROJE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 0,

            /// <summary>
            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,

            /// <summary>
            /// PROJEADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IOIPROJEADI = 3,

            /// <summary>
            /// BASLAYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASLAYIL = 4,

            /// <summary>
            /// BITISYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITISYIL = 5,

            /// <summary>
            /// TOPLAMMALIYET alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PROJETUTARI = 6,

            /// <summary>
            /// ILKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILKOD = 7,

            /// <summary>
            /// ISTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISTUR = 8,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 9,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "SIRANO", "MUHASEBEKOD", "BIRIMKOD", "IOIPROJEADI", "BASLAYIL", "BITISYIL", "PROJETUTARI", "ILKOD", "ISTUR", "ACIKLAMA" };



        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADPEB = "BHMV2IOIPROJEPEBvw";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarPEB : int
        {
            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 0,

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 1,

            /// <summary>
            /// PEBKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 2,

        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPEB = { "SIRANO", "YIL", "PEBKOD" };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, IOIProje yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(IOIProje form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IOIPROJEADI], form.ioiProjeAdi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLAYIL], form.baslaYil);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISYIL], form.bitisYil);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJETUTARI], form.projeTutari);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], form.aciklama);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], form.ilKod);

            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISTUR], form.isTur);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], form.muhasebeKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], form.birimKod);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 1);
        }

        //public Sonuc EklePebKod(int siraNo, int yil, string pebKod)
        //{
        //    string sql = "INSERT INTO " + TABLOADPEB + "(" + VeriErisim.TumAlanlar("", ALANLARPEB) + ") VALUES";
        //    sql += "(" + VeriErisim.DegerYap(siraNo);
        //    sql += "," + VeriErisim.DegerYap(yil);
        //    sql += "," + VeriErisim.DegerYap(pebKod.Replace(".", ""));
        //    sql += ")";
        //    return KomutCalistir(sql, 1);
        //}

        //public Sonuc SilPebKodlariSil(int siraNo)
        //{
        //    string sql = "DELETE FROM " + TABLOADPEB;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARPEB[(int)ENUMAlanlarPEB.SIRANO], "=", siraNo));
        //    return KomutCalistir(sql, 0);
        //}

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, IOIProje yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(IOIProje form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, IOIProje yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(IOIProje form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + ")  OUTPUT Inserted.SIRANO VALUES";
            sql += "(" + VeriErisim.DegerYap(form.muhasebeKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(form.ioiProjeAdi);
            sql += "," + VeriErisim.DegerYap(form.baslaYil);
            sql += "," + VeriErisim.DegerYap(form.bitisYil);
            sql += "," + VeriErisim.DegerYap(form.projeTutari);
            sql += "," + VeriErisim.DegerYap(form.ilKod);
            sql += "," + VeriErisim.DegerYap(form.isTur);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += ")";
            int eklenenDeger = veriErisim.TekKolonSQLInt(sql);
            Sonuc s = new Sonuc();
            s.anahtar = eklenenDeger.ToString();
            if (!string.IsNullOrWhiteSpace(veriErisim.hata))
                s.DegerAta(veriErisim.hata);
            return s;
        }

        private string Virgulle(System.Collections.Generic.List<int> liste)
        {
            string donusDeger = "";
            for (int i = 0; i < liste.Count; i++)
            {
                if (i > 0) donusDeger += ",";
                donusDeger += VeriErisim.DegerYap(liste[i]);
            }
            return donusDeger;
        }


        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, IOIProje yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(IOIProje form, System.Collections.Generic.List<int> isKodListesi, int butceYil)
        {
            string pebkodAlan = "(" + PebKodlariBulSql() + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            string alanlar = VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", M.MUHASEBEAD, B.BIRIMAD, IL.ILADI ";
            string ekAlanlar;
            if (butceYil == -1)
            {
                ekAlanlar = ", NULL , NULL, NULL, NULL";
            }
            else if (butceYil <= 2000)
            {
                ekAlanlar = ", (SELECT SUM(AKTARILANTUTAR) FROM BHMV2IOIPROJEAKTARMA WHERE BHMV2IOIPROJEAKTARMA.SIRANO=BHMV2IOIPROJE.SIRANO)";
                ekAlanlar += ", (SELECT SUM(ODENEK) FROM ButceIslemleri.dbo.MPGMASRAFPLANITERTIP MT WHERE MT.IOIISKOD=BHMV2IOIPROJE.SIRANO AND MT.DURUM = 5)";
                ekAlanlar += ", (SELECT SUM(HARCAMATUTARI) FROM BHMV2IOIPROJEHARCAMA WHERE BHMV2IOIPROJEHARCAMA.SIRANO=BHMV2IOIPROJE.SIRANO)";
                ekAlanlar += ", (SELECT SUM(D.MIKTAR) AS MIKTAR FROM ButceIslemleri.dbo.MUHGONDERMETENKISFORM F";
                ekAlanlar += "     INNER JOIN ButceIslemleri.dbo.MUHGONDERMETENKIS D ON F.YIL = D.YIL AND F.BELGETUR = D.BELGETUR AND F.BELGENO = D.BELGENO WHERE F.FORMDURUM <= 5 AND F.IOIISKOD=BHMV2IOIPROJE.SIRANO)";
            }
            else
            {
                ekAlanlar = $", (SELECT SUM(ODENEK) FROM ButceIslemleri.dbo.MPGMASRAFPLANITERTIP MT WHERE MT.YIL={butceYil} AND MT.ASAMA=5 AND MT.IOIISKOD=BHMV2IOIPROJE.SIRANO AND MT.DURUM = 5)";
                ekAlanlar += $", (SELECT SUM(ODENEK) FROM ButceIslemleri.dbo.MPGMASRAFPLANITERTIP MT WHERE MT.YIL={butceYil} AND MT.ASAMA=5 AND MT.IOIISKOD=BHMV2IOIPROJE.SIRANO AND MT.DURUM < 5)";

                ekAlanlar += ", (SELECT SUM(D.MIKTAR) AS MIKTAR FROM ButceIslemleri.dbo.MUHGONDERMETENKISFORM F";
                ekAlanlar += $"     INNER JOIN ButceIslemleri.dbo.MUHGONDERMETENKIS D ON F.YIL = D.YIL AND F.BELGETUR = D.BELGETUR AND F.BELGENO = D.BELGENO AND F.YIL ={butceYil} AND F.FORMDURUM = 5 AND F.IOIISKOD=BHMV2IOIPROJE.SIRANO)";
                ekAlanlar += ", (SELECT SUM(D.MIKTAR) AS MIKTAR FROM ButceIslemleri.dbo.MUHGONDERMETENKISFORM F";
                ekAlanlar += $"     INNER JOIN ButceIslemleri.dbo.MUHGONDERMETENKIS D ON F.YIL = D.YIL AND F.BELGETUR = D.BELGETUR AND F.BELGENO = D.BELGENO AND F.YIL ={butceYil} AND F.FORMDURUM < 5 AND F.IOIISKOD=BHMV2IOIPROJE.SIRANO)";
            }

            string sql = "SELECT " + alanlar + ekAlanlar + ", " + pebkodAlan;
            sql += " FROM BHMV2IOIPROJE LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE AS M ON BHMV2IOIPROJE.MUHASEBEKOD = M.MUHASEBEKOD";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM AS B ON BHMV2IOIPROJE.MUHASEBEKOD = B.MUHASEBEKOD AND BHMV2IOIPROJE.BIRIMKOD = B.BIRIMKOD ";
            sql += " LEFT OUTER JOIN Yatirim.dbo.YATIL IL ON IL.ILKODU = BHMV2IOIPROJE.ILKOD ";

            string kosul = "";
            if (isKodListesi != null && isKodListesi.Count > 0)
            {
                kosul = TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO] + " IN (" + Virgulle(isKodListesi) + ")";
            }
            else if (form.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            else
            {
                if (form.isTur > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTUR], "=", form.isTur));
                }
                if (form.muhasebeKod > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
                }
                if (!string.IsNullOrWhiteSpace(form.birimKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "LIKE", form.birimKod + "%"));
                if (form.ilKod > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", form.ilKod));
                }
                if (!string.IsNullOrWhiteSpace(form.ioiProjeAdi))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IOIPROJEADI], "LIKE", "%" + form.ioiProjeAdi + "%"));
                }
                if (form.baslaYil > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLAYIL], ">=", form.baslaYil));
                }
                if (form.bitisYil > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLAYIL], "<=", form.bitisYil));
                }
                if (form.projeTutari > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJETUTARI], ">=", form.projeTutari));
                }
                if (form.iOIHarc > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJETUTARI], "<=", form.iOIHarc));
                }
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " ORDER BY 2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        private string PebKodlariBulSql()
        {
            return "SELECT PK.YIL, PK.PEBKOD, PK.PEBAD, PK.PEBAD2, P.PROJENO, P.DPTPROJENO, P.AD, AP.ALTPROJENO, AP.AD " +
                " FROM BHMV2IOIPROJEPEBvw IOI" +
                " INNER JOIN ButceHazirlik.dbo.ABSPEBKOD PK ON PK.YIL=IOI.YIL AND PK.PEBKOD=IOI.PEBKOD" +
                " INNER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD KP ON KP.YIL=PK.YIL AND KP.PEBKOD=PK.PEBKOD AND KP.SENARYOKOD='0600'" +
                " INNER JOIN Yatirim.dbo.YATBUTCEKAYNAK KAY ON KAY.YIL=KP.YIL AND KAY.SENARYOKOD=KP.SENARYOKOD AND KAY.KAYNAKNO=KP.KAYNAKNO " +
                " INNER JOIN Yatirim.dbo.YATALTPROJE AS AP ON AP.YIL=KAY.YIL AND AP.SENARYOKOD=KAY.SENARYOKOD AND AP.ALTPROJENO=KAY.ALTPROJENO" +
                " INNER JOIN Yatirim.dbo.YATPROJE AS P ON P.YIL=AP.YIL AND P.SENARYOKOD = AP.SENARYOKOD AND P.PROJENO = AP.PROJENO " +
                " where IOI.SIRANO = BHMV2IOIPROJE.SIRANO";
        }

        public IDataReader ListelePeb(int yil, string pebKod)
        {
            string sql = "SELECT PK.YIL, PK.PEBKOD, PK.PEBAD, PK.PEBAD2, P.PROJENO, P.DPTPROJENO, P.AD, AP.ALTPROJENO, AP.AD " +
                " FROM ButceHazirlik.dbo.ABSPEBKOD PK " +
                " INNER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD KP ON KP.YIL=PK.YIL AND KP.PEBKOD=PK.PEBKOD AND KP.SENARYOKOD='0600'" +
                " INNER JOIN Yatirim.dbo.YATBUTCEKAYNAK KAY ON KAY.YIL=KP.YIL AND KAY.SENARYOKOD=KP.SENARYOKOD AND KAY.KAYNAKNO=KP.KAYNAKNO " +
                " INNER JOIN Yatirim.dbo.YATALTPROJE AS AP ON AP.YIL=KAY.YIL AND AP.SENARYOKOD=KAY.SENARYOKOD AND AP.ALTPROJENO=KAY.ALTPROJENO" +
                " INNER JOIN Yatirim.dbo.YATPROJE AS P ON P.YIL=AP.YIL AND P.SENARYOKOD = AP.SENARYOKOD AND P.PROJENO = AP.PROJENO " +
                $" where PK.YIL={VeriErisim.DegerYap(yil)} AND PK.PEBKOD={VeriErisim.DegerYap(pebKod.Replace(".", ""))}";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleHarcama(int ioiProjeNo)
        {
            string sql = $"SELECT HARCAMANO, HARCAMATARIHI, HARCAMATUTARI, ACIKLAMA FROM BHMV2IOIPROJEHARCAMA WHERE SIRANO={VeriErisim.DegerYap(ioiProjeNo)} ORDER BY 2";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleAktarilan(int ioiProjeNo)
        {
            string sql = $"SELECT AKTARMANO, AKTARMATARIHI, AKTARILANTUTAR, ACIKLAMA, TUR FROM BHMV2IOIPROJEAKTARMA WHERE SIRANO={VeriErisim.DegerYap(ioiProjeNo)} ORDER BY 2";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc HarcamaEkle(int ioiProjeNo, IOIProjeHarcama item)
        {
            string sql = $"INSERT INTO BHMV2IOIPROJEHARCAMA (SIRANO, HARCAMATARIHI, HARCAMATUTARI, ACIKLAMA) VALUES ({VeriErisim.DegerYap(ioiProjeNo)}, {VeriErisim.DegerYap(item.harcamaTarihi)}, {VeriErisim.DegerYap(item.harcamaTutari)}, {VeriErisim.DegerYap(item.aciklama)})";
            return KomutCalistir(sql, 1);
        }

        public Sonuc HarcamaGuncelle(IOIProjeHarcama item)
        {
            string sql = $"UPDATE BHMV2IOIPROJEHARCAMA SET HARCAMATARIHI={VeriErisim.DegerYap(item.harcamaTarihi)}, HARCAMATUTARI={VeriErisim.DegerYap(item.harcamaTutari)}, ACIKLAMA={VeriErisim.DegerYap(item.aciklama)} WHERE HARCAMANO={VeriErisim.DegerYap(item.harcamaNo)}";
            return KomutCalistir(sql, 1);
        }

        public Sonuc HarcamaSil(IOIProjeHarcama item)
        {
            string sql = $"DELETE FROM BHMV2IOIPROJEHARCAMA WHERE HARCAMANO={VeriErisim.DegerYap(item.harcamaNo)}";
            return KomutCalistir(sql, 1);
        }

        public Sonuc AktarmaEkle(int ioiProjeNo, IOIProjeAktarilan item)
        {
            string sql = $"INSERT INTO BHMV2IOIPROJEAKTARMA (SIRANO, AKTARMATARIHI, AKTARILANTUTAR, ACIKLAMA, TUR) VALUES ({VeriErisim.DegerYap(ioiProjeNo)}, {VeriErisim.DegerYap(item.aktarmaTarihi)}, {VeriErisim.DegerYap(item.aktarilanTutar)}, {VeriErisim.DegerYap(item.aciklama)}, {VeriErisim.DegerYap(item.tur)})";
            return KomutCalistir(sql, 1);
        }

        public Sonuc AktarmaGuncelle(IOIProjeAktarilan item)
        {
            string sql = $"UPDATE BHMV2IOIPROJEAKTARMA SET AKTARMATARIHI={VeriErisim.DegerYap(item.aktarmaTarihi)}, AKTARILANTUTAR={VeriErisim.DegerYap(item.aktarilanTutar)}, ACIKLAMA={VeriErisim.DegerYap(item.aciklama)}, TUR={VeriErisim.DegerYap(item.tur)} WHERE AKTARMANO={VeriErisim.DegerYap(item.aktarmaNo)}";
            return KomutCalistir(sql, 1);
        }

        public Sonuc AktarmaSil(IOIProjeAktarilan item)
        {
            string sql = $"DELETE FROM BHMV2IOIPROJEAKTARMA WHERE AKTARMANO={VeriErisim.DegerYap(item.aktarmaNo)}";
            return KomutCalistir(sql, 1);
        }
    }
}

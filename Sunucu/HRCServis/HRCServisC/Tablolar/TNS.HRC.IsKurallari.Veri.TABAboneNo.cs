using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Abone numarası bilgisini tutan HRCABONENO veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABAboneNo : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAboneNo(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Abone Numarası tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCABONENO";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// ABONENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ABONENO = 3,
            /// <summary>
            /// ABONEADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ABONEADI = 4,
            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 5,
            /// <summary>
            /// ILKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILKODU = 6,
            /// <summary>
            /// ABONETIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ABONETIP = 7,
            /// <summary>
            /// FIRMANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMANO = 8,
            /// <summary>
            /// KAYITTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYITTARIHI = 9,
            /// <summary>
            /// KAYITEDEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYITEDEN = 10,
            /// <summary>
            /// ISLETMEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLETMEKODU = 11,
        }

        /// <summary>
        /// Abone Numarası tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "MUHASEBEKOD", "BIRIMKOD", "ABONENO", "ABONEADI", "DURUM", "ILKODU", "ABONETIP", "FIRMANO", "KAYITTARIHI", "KAYITEDEN", "ISLETMEKODU" };

        /// <summary>
        /// Muhasebe view tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADMUHASEBEVIEW = "HRCMUHASEBEVIEW";

        /// <summary>
        /// Muhasebe view tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarMUHASEBEVIEW
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
        public static string[] ALANLARMUHASEBEVIEW = { "MUHASEBEKOD", "MUHASEBEAD" };

        /// <summary>
        /// Firma bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADFIRMABILGISI = "HRCFIRMABILGISI";

        /// <summary>
        /// Firma Bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarFIRMABILGISI
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
        }

        /// <summary>
        /// Firma Bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARFIRMABILGISI = { "KOD", "AD", "MUHASEBEKOD" };

        /// <summary>
        /// Abone numarası bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek abone numarası bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(AboneNo bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.aboneNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.aboneAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.aboneTip) + ",";
            sql += VeriErisim.DegerYap(bilgi.firma.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.kayitTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.kayitEden) + ",";
            sql += VeriErisim.DegerYap(bilgi.isletmeKodu) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], bilgi.muhasebe) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], bilgi.birim) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ABONENO], bilgi.aboneNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ABONEADI], bilgi.aboneAdi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKODU], bilgi.ilKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ABONETIP], bilgi.aboneTip) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMANO], bilgi.firma.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITTARIHI], bilgi.kayitTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITEDEN], bilgi.kayitEden) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLETMEKODU], bilgi.isletmeKodu);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Abone numarası bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek abone numarası için kıstas parametresi</param>
        /// <returns>
        /// Abone numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(AboneNo bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR)
                    + "," + TABLOADMUHASEBEVIEW + "." + ALANLARMUHASEBEVIEW[(int)ENUMAlanlarMUHASEBEVIEW.MUHASEBEAD]
                    + "," + TABLOADFIRMABILGISI + "." + ALANLARFIRMABILGISI[(int)ENUMAlanlarFIRMABILGISI.AD];
            sql += " FROM " + TABLOAD;

            sql += " LEFT OUTER JOIN " + TABLOADFIRMABILGISI + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMANO] + "=" + TABLOADFIRMABILGISI + "." + ALANLARFIRMABILGISI[(int)ENUMAlanlarFIRMABILGISI.KOD] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADFIRMABILGISI + "." + ALANLARFIRMABILGISI[(int)ENUMAlanlarFIRMABILGISI.MUHASEBEKOD];

            sql += " LEFT OUTER JOIN " + TABLOADMUHASEBEVIEW + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADMUHASEBEVIEW + "." + ALANLARMUHASEBEVIEW[(int)ENUMAlanlarMUHASEBEVIEW.MUHASEBEKOD];

            if (!bilgi.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITTARIHI], ">=", bilgi.baslamaTarih));

            if (!bilgi.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITTARIHI], "<=", bilgi.bitisTarih));

            if (bilgi.ilKodu != "" && bilgi.ilKodu != "0")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKODU], "=", bilgi.ilKodu));

            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));

            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));

            if (bilgi.aboneNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONENO], "=", bilgi.aboneNo));

            if (bilgi.isletmeKodu != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLETMEKODU], "=", bilgi.isletmeKodu));

            if (bilgi.durum == -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", 1));//sadece aktif olanları listele
            else
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));

            if (bilgi.aboneTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONETIP], "=", bilgi.aboneTip));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Abone numarası bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek abone numarası için kıstas parametresi</param>
        /// <returns>
        /// Abone numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AboneNoKayitliMi(AboneNo bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);

            sql += " FROM " + TABLOAD;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            //if (bilgi.muhasebe > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));

            //if (bilgi.birim != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));

            if (bilgi.aboneNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONENO], "=", bilgi.aboneNo));

            if (bilgi.aboneTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONETIP], "=", bilgi.aboneTip));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", 1));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleIlgili(AboneNo bilgi)
        {
            string icSql = "SELECT " + ALANLAR[(int)ENUMAlanlar.FIRMANO] + " FROM " + TABLOAD;
            icSql = VeriErisim.KosulEkle(icSql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));

            if (bilgi.aboneTip > 0)
                icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONETIP], "=", bilgi.aboneTip));

            if (bilgi.aboneNo != "")
                icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONENO], "=", bilgi.aboneNo));

            if (bilgi.birim != "")
                icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));

            if (bilgi.aboneNo != "")
                icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ABONENO], "=", bilgi.aboneNo));

            if (bilgi.isletmeKodu != "")
                icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLETMEKODU], "=", bilgi.isletmeKodu));

            if (bilgi.durum == -1)
                icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", 1));//sadece aktif olanları listele
            else
                icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));

            string sql = "SELECT " + VeriErisim.TumAlanlar("", TABFirmaBilgisi.ALANLAR) + " FROM " + TABFirmaBilgisi.TABLOAD;
            sql += " WHERE " + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD] + " IN (" + icSql + ")";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

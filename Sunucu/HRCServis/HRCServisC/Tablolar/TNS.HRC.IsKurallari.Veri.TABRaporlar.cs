using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Harcama Yönetimi raporlar sınıfı
    /// </summary>
    public class TABRaporlar : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABRaporlar(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Aşama durum süreç sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Aşama durum süreç sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AsamaDurumSurecSayisi(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";
            string innerKosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.SURECNO] + ")) FROM " + TABSurec.TABLOADDURUM;
            if (kriter.muhasebeKod > 0 || kriter.birimKod != "" || !kriter.baslamaTarih.isNull || !kriter.bitisTarih.isNull)
            {

                sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";

                if (kriter.muhasebeKod > 0)
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebeKod));
                if (kriter.birimKod != "")
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD], "=", kriter.birimKod));
                if (!kriter.baslamaTarih.isNull)
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
                if (!kriter.bitisTarih.isNull)
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

                sql += innerKosul;
                sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.SURECNO];
            }

            if (kriter.parcaKod1 > 0 && kriter.parcaKod2 > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.PARCAKOD], ">", kriter.parcaKod1));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.PARCAKOD], "<", kriter.parcaKod2));
            }
            else
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.PARCAKOD], "=", kriter.parcaKod1));
            }
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.DURUM], "=", kriter.parcaDurum1));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.ISLEMKOD], "=", kriter.islemKodu));

            if (kosul != "")
                sql += " WHERE " + kosul;


            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Onay aşama durum süreç sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Onay aşama durum süreç sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader OnayAsamaDurumSurecSayisi(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";
            string innerKosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.SURECNO] + ")) FROM " + TABSurec.TABLOADDURUM;
            if (kriter.muhasebeKod > 0 || kriter.birimKod != "" || !kriter.baslamaTarih.isNull || !kriter.bitisTarih.isNull)
            {

                sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";

                if (kriter.muhasebeKod > 0)
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebeKod));
                if (kriter.birimKod != "")
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD], "=", kriter.birimKod));
                if (!kriter.baslamaTarih.isNull)
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
                if (!kriter.bitisTarih.isNull)
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

                sql += innerKosul;
                sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.SURECNO];
                sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + "=" + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.YIL];

                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));

            }
            else
            {
                sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";
                sql += TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.SURECNO];
                sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + "=" + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.YIL];

                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));

            }

            //Burda bir parca kodlarda aralık verilirse calısıyormu ıncele

            if (kriter.kurumsalKod != "" && kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND ((";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + ")";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }
            else if (kriter.kurumsalKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + "))";
            }
            else if (kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }

            if (kriter.parcaKod1 > 0 && kriter.parcaKod2 > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.PARCAKOD], ">", kriter.parcaKod1));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.PARCAKOD], "<", kriter.parcaKod2));
            }
            else if (kriter.parcaKod1 > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.PARCAKOD], "=", kriter.parcaKod1));
            }
            if (kriter.parcaDurum1 > 0 && kriter.parcaDurum2 > 0)
            {
                kosul += " AND (";
                kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.DURUM], "=", kriter.parcaDurum1));
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.DURUM], "=", kriter.parcaDurum2)) + ") ";
            }
            else if (kriter.parcaDurum1 > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADDURUM, TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.DURUM], "=", kriter.parcaDurum1));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));

            if (kosul != "")
                sql += " WHERE " + kosul;


            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Mal hizmet yapım özet toplam tutar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Mal hizmet yapım özet toplam tutar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MHYOzetToplamTutarHesapla(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + TABOnayForm.TABLOAD + "." + TABOnayForm.ALANLAR[(int)TABOnayForm.ENUMAlanlar.YAKLASIKMALIYET] + ")" +
                "," + VeriErisim.IslecBul("SUM") + "(" + DBDegiskeni.VIEWMHYALIM + ".TOPLAM)" +
                "," + VeriErisim.IslecBul("SUM") + "(" + DBDegiskeni.GRUPLUFATURADETAYVIEW + ".TOPLAM)" +
                 "," + VeriErisim.IslecBul("SUM") + "(" + DBDegiskeni.GRUPLUODEMEEMRIVIEW + ".TOPLAM)" +
                " FROM " + TABSurec.TABLOAD;
            //Burda bir parca kodlarda aralık verilirse calısıyormu ıncele

            if (kriter.kurumsalKod != "" && kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND ((";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + ")";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }
            else if (kriter.kurumsalKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + "))";
            }
            else if (kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }


            sql += " LEFT OUTER JOIN " + TABOnayForm.TABLOAD + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + TABOnayForm.TABLOAD + "." + TABOnayForm.ALANLAR[(int)TABOnayForm.ENUMAlanlar.SURECNO];
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + TABOnayForm.TABLOAD + "." + TABOnayForm.ALANLAR[(int)TABOnayForm.ENUMAlanlar.YIL];

            sql += " LEFT OUTER JOIN " + DBDegiskeni.GRUPLUFATURADETAYVIEW + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.GRUPLUFATURADETAYVIEW + ".SURECNO";
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + DBDegiskeni.GRUPLUFATURADETAYVIEW + ".YIL";

            sql += " LEFT OUTER JOIN " + DBDegiskeni.GRUPLUODEMEEMRIVIEW + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.GRUPLUODEMEEMRIVIEW + ".SURECNO";
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + DBDegiskeni.GRUPLUODEMEEMRIVIEW + ".YIL";

            sql += " LEFT OUTER JOIN " + DBDegiskeni.VIEWMHYALIM + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.VIEWMHYALIM + ".SURECNO";
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + DBDegiskeni.VIEWMHYALIM + ".YIL";

            if (!kriter.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));

            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD], "=", kriter.birimKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Mal hizmet yapım detay toplam tutar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Mal hizmet yapım detay toplam tutar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MHYDetayToplamTutarHesapla(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";

            sql += "SELECT ";

            sql += TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO];
            sql += "," + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD];
            sql += "," + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD];

            sql += ",ONAYVIEW2.ONAYLAMATARIHI";
            sql += ",ONAYVIEW2.YAKLASIKMALIYET";
            sql += "," + DBDegiskeni.VIEWMHYALIM + ".TOPLAM";
            sql += "," + DBDegiskeni.VIEWMHYALIM + ".FIRMANO";
            sql += "," + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD];
            sql += "," + DBDegiskeni.TABLOADFATURADETAYSURECTOPLAMVIEW + ".TOPLAM";
            sql += "," + DBDegiskeni.TABLOADFATURADETAYSURECTOPLAMVIEW + "." + TABFaturaForm.ALANLAR[(int)TABFaturaForm.ENUMAlanlar.FATURATARIH];
            sql += "," + DBDegiskeni.TABLOADODEMEEMRITAHAKKUKTOPLAMVIEW + ".TOPLAM";
            sql += "," + DBDegiskeni.TABLOADODEMEEMRITAHAKKUKTOPLAMVIEW + "." + TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH];
            sql += "," + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEAD ";
            sql += "," + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.PARCAKOD];
            sql += "," + DBDegiskeni.TABLOADFATURADETAYSURECTOPLAMVIEW + ".TOPLAM";
            sql += ",ONAYVIEW2.EKONOMIKKOD1";
            sql += ",ONAYVIEW2.EKONOMIKKOD2";
            sql += "," + DBDegiskeni.TABLOADMUHASEBEBIRIM + ".BIRIMAD ";

            if (kriter.odemeEmriGoster)
            {
                sql += "," + DBDegiskeni.VIEWSURECMUHMIFBAG + ".EKOKOD";
                sql += "," + DBDegiskeni.VIEWSURECMUHMIFBAG + ".ODEMEEMRINO";
            }

            sql += " FROM " + TABFirmaBilgisi.TABLOAD + " RIGHT OUTER JOIN " + TABSurec.TABLOAD;

            if (kriter.kurumsalKod != "" && kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW +
               "AS ONAYVIEW1 ON (" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = ONAYVIEW1.SURECNO AND " +
                TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = ONAYVIEW1.YIL";
                sql += " AND ((";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl("ONAYVIEW1", "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("ONAYVIEW1", "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + ")";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl("ONAYVIEW1", "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl("ONAYVIEW1", "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }
            else if (kriter.kurumsalKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO AND " +
                TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".YIL";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + "))";
            }
            else if (kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO AND ";
                sql += TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".YIL ";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }

            sql += " INNER JOIN " + DBDegiskeni.TABLOADMUHASEBE + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD] + " = " + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEKOD ";
            //sql += " INNER JOIN " + DBDegiskeni.TABLOADMUHASEBEBIRIM + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD] + " = " + DBDegiskeni.TABLOADMUHASEBEBIRIM + ".BIRIMKOD"; 

            sql += " LEFT OUTER JOIN HRCMUHASEBEBIRIMVIEW ON HRCMUHASEBEBIRIMVIEW.MUHASEBEKOD = " + TABSurec.TABLOAD + ".MUHASEBEKOD AND HRCMUHASEBEBIRIMVIEW.BIRIMKOD = " + TABSurec.TABLOAD + ".BIRIMKOD ";


            sql += " LEFT OUTER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ONAYVIEW2 ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = ONAYVIEW2.SURECNO";
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " =ONAYVIEW2.YIL";


            sql += " LEFT OUTER JOIN " + DBDegiskeni.VIEWMHYALIM + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.VIEWMHYALIM + ".SURECNO";
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + DBDegiskeni.VIEWMHYALIM + ".YIL";

            sql += " LEFT OUTER JOIN " + DBDegiskeni.TABLOADFATURADETAYSURECTOPLAMVIEW + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADFATURADETAYSURECTOPLAMVIEW + ".SURECNO";
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + DBDegiskeni.TABLOADFATURADETAYSURECTOPLAMVIEW + ".YIL";

            sql += " LEFT OUTER JOIN " + DBDegiskeni.TABLOADODEMEEMRITAHAKKUKTOPLAMVIEW + " ON (" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADODEMEEMRITAHAKKUKTOPLAMVIEW + ".SURECNO";
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + DBDegiskeni.TABLOADODEMEEMRITAHAKKUKTOPLAMVIEW + ".YIL";
            sql += " AND " + DBDegiskeni.TABLOADFATURADETAYSURECTOPLAMVIEW + ".PRFATURANO = " + DBDegiskeni.TABLOADODEMEEMRITAHAKKUKTOPLAMVIEW + ".PRFATURANO) ";

            sql += " ON " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD] + "=" + DBDegiskeni.VIEWMHYALIM + ".FIRMANO";

            sql += " INNER JOIN " + TABSurec.TABLOADDURUM + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.SURECNO];
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.YIL];
            sql += " AND " + TABSurec.TABLOADDURUM + "." + TABSurec.ALANLARDURUM[(int)TABSurec.ENUMAlanlarDurum.PARCAKOD] + "=  (SELECT MAX(PARCAKOD) FROM HRCSURECDURUM WHERE  HRCSUREC.SURECNO = HRCSURECDURUM.SURECNO AND HRCSUREC.YIL = HRCSURECDURUM.YIL AND HRCSURECDURUM.DURUM>0)";

            if (kriter.odemeEmriGoster)
            {
                sql += " LEFT OUTER JOIN " + DBDegiskeni.VIEWSURECMUHMIFBAG + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.VIEWSURECMUHMIFBAG + ".SURECNO";
                sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + " = " + DBDegiskeni.VIEWSURECMUHMIFBAG + ".YIL";
                sql += " AND " + DBDegiskeni.TABLOADFATURADETAYSURECTOPLAMVIEW + ".ODEMEEMRINO = " + DBDegiskeni.VIEWSURECMUHMIFBAG + ".ODEMEEMRINO";
            }
            if (!kriter.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));
            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD], "=", kriter.birimKod));

            if (kosul != "")
                sql += " WHERE " + kosul;


            sql += " ORDER BY HRCSUREC.YIL, HRCSUREC.SURECNO ";

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Mal hizmet yapım ihale tür toplam bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Mal hizmet yapım ihale tür toplam bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MHYIhaleTurToplamlariRaporu(RaporKriter kriter)
        {
            string sql = "";

            sql += " SELECT   HRCSUREC.ISLEMKOD, SUM(HRCIHALEONAYFORM.YAKLASIKMALIYET) AS Expr1," +
                    " COUNT(*) AS Expr2, HRCIHALEONAYFORM.IHALEUSULU, " +
                    "  HRCIHALEONAYFORM.IHALEMADDESI, HRCIHALETURMADDE.ADI " +
                    " FROM HRCIHALEONAYFORM INNER JOIN  " +
                    " HRCIHALETURMADDE ON HRCIHALEONAYFORM.IHALEUSULU = HRCIHALETURMADDE.IHALEKOD AND " +
                    " HRCIHALEONAYFORM.IHALEMADDESI = HRCIHALETURMADDE.KOD LEFT OUTER JOIN " +
                    " HRCSUREC ON HRCIHALEONAYFORM.YIL = HRCSUREC.YIL AND " +
                    " HRCIHALEONAYFORM.SURECNO = HRCSUREC.SURECNO " +
                    " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (HRCIHALEONAYFORM.YIL = " + kriter.yil + ")" +
                    " GROUP BY HRCSUREC.ISLEMKOD, HRCIHALEONAYFORM.IHALEUSULU, HRCIHALEONAYFORM.IHALEMADDESI, HRCIHALETURMADDE.ADI" +
                    " HAVING      (HRCSUREC.ISLEMKOD <= 3) ORDER BY HRCSUREC.ISLEMKOD," +
                    " HRCIHALEONAYFORM.IHALEUSULU, HRCIHALEONAYFORM.IHALEMADDESI ";

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Mal hizmet yapım il toplam bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Mal hizmet yapım il toplam bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MHYIlToplamlariRaporu(RaporKriter kriter)
        {
            string sql = "";

            sql += "SELECT  HRCSUREC.ISLEMKOD, SUM(HRCIHALEONAYFORM.YAKLASIKMALIYET) AS Expr1, COUNT(*) AS Expr2, HRCIHALEONAYFORM.MUHASEBEKOD / 1000 AS Expr3, HRCIL.AD " +
                    " FROM HRCIHALEONAYFORM LEFT OUTER JOIN " +
                    "  HRCIL ON HRCIHALEONAYFORM.MUHASEBEKOD / 1000 = HRCIL.KOD RIGHT OUTER JOIN " +
                    "  HRCSUREC ON HRCIHALEONAYFORM.YIL = HRCSUREC.YIL AND HRCIHALEONAYFORM.SURECNO = HRCSUREC.SURECNO " +
                    " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (HRCIHALEONAYFORM.YIL =" + kriter.yil + ")" +
                    " GROUP BY HRCSUREC.ISLEMKOD, HRCIHALEONAYFORM.MUHASEBEKOD / 1000, HRCIL.AD " +
                    " HAVING      (HRCSUREC.ISLEMKOD <= 3) " +
                    " ORDER BY HRCSUREC.ISLEMKOD, Expr3 ";

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Abonelik özet toplam tutar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Abonelik özet toplam tutar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AbonelikOzetToplamTutarHesapla(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";
            string grup = "";

            sql = "SELECT " + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.ABONETIP] + ","
                + VeriErisim.IslecBul("SUM") + "(" + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.FATURATUTAR] + ")" +
                "," + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ")" +

                " FROM " + TABAboneFatura.TABLOAD;

            sql += " INNER JOIN " + TABSurec.TABLOAD + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + TABAboneFatura.TABLOAD + ".SURECNO";

            sql += " LEFT OUTER JOIN " + TABOdemeEmriForm.TABLOADDETAY + " ON " + TABOdemeEmriForm.TABLOADDETAY + ".SURECNO" + " = " + TABAboneFatura.TABLOAD + ".SURECNO";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".MUHASEBEKOD" + " = " + TABAboneFatura.TABLOAD + ".MUHASEBEKOD";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".BIRIMKOD" + " = " + TABAboneFatura.TABLOAD + ".BIRIMKOD";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".YIL" + " = " + TABAboneFatura.TABLOAD + ".YIL";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".PRODEMEEMRINO" + " = " + TABAboneFatura.TABLOAD + ".ODEMEEMRINO";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));
            if (!kriter.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD], "=", kriter.birimKod));


            grup = TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.ABONETIP];
            if (kosul != "")
                sql += " WHERE " + kosul;

            if (grup != "")
                sql += " GROUP BY  " + grup;

            sql += " ORDER BY " + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.ABONETIP];



            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Abonelik detay toplam tutar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Abonelik detay toplam tutar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AbonelikDetayToplamTutarHesapla(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";
            string grup = "";

            sql = "SELECT " + TABAboneFatura.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO];
            sql += "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.MUHASEBEKOD];
            sql += "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.BIRIMKOD];
            sql += "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.ABONETIP];
            sql += "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.ODEMETARIHI];
            sql += "," + VeriErisim.IslecBul("SUM") + "(" + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.FATURATUTAR] + ")";
            sql += "," + TABOdemeEmriForm.TABLOAD + "." + TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH];
            sql += "," + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ")";

            sql += " FROM " + TABOdemeEmriForm.TABLOADDETAY;

            sql += " INNER JOIN " + TABOdemeEmriForm.TABLOAD + " ON " + TABOdemeEmriForm.TABLOADDETAY + ".SURECNO" + " = " + TABOdemeEmriForm.TABLOAD + ".SURECNO";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".MUHASEBEKOD" + " = " + TABOdemeEmriForm.TABLOAD + ".MUHASEBEKOD";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".BIRIMKOD" + " = " + TABOdemeEmriForm.TABLOAD + ".BIRIMKOD";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".YIL" + " = " + TABOdemeEmriForm.TABLOAD + ".YIL";


            sql += " INNER JOIN " + TABSurec.TABLOAD + " ON (" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + TABOdemeEmriForm.TABLOADDETAY + ".SURECNO";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));

            if (kriter.muhasebeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD], "=", kriter.birimKod));


            if (!kriter.baslamaTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            sql += ")";

            sql += " INNER JOIN " + DBDegiskeni.TABLOADMUHASEBE + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD] + " = " + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEKOD ";

            sql += " RIGHT OUTER JOIN " + TABAboneFatura.TABLOAD + " ON ";
            sql += TABOdemeEmriForm.TABLOADDETAY + ".SURECNO" + " = " + TABAboneFatura.TABLOAD + ".SURECNO";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".MUHASEBEKOD" + " = " + TABAboneFatura.TABLOAD + ".MUHASEBEKOD";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".BIRIMKOD" + " = " + TABAboneFatura.TABLOAD + ".BIRIMKOD";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".YIL" + " = " + TABAboneFatura.TABLOAD + ".YIL";
            sql += " AND " + TABOdemeEmriForm.TABLOADDETAY + ".PRODEMEEMRINO" + " = " + TABAboneFatura.TABLOAD + ".ODEMEEMRINO";


            grup = TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.SURECNO];
            grup += "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.MUHASEBEKOD];
            grup += "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.BIRIMKOD];
            grup += "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.ABONETIP];
            grup += "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.ODEMETARIHI];
            grup += "," + TABOdemeEmriForm.TABLOAD + "." + TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH];

            if (kosul != "")
                sql += " WHERE " + kosul;

            if (grup != "")
                sql += " GROUP BY  " + grup;
            sql += " ORDER BY " + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.SURECNO]
                + "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.MUHASEBEKOD]
                + "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.ABONETIP]
                + "," + TABAboneFatura.TABLOAD + "." + TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.ODEMETARIHI]
                + "," + TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH];

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Onay belgesi düzenleyen farklı birim sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Onay belgesi düzenleyen farklı birim sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader OnayFarkliBirimSayisi(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD] + "+" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD] + ")) FROM " + TABSurec.TABLOAD;

            if (kriter.kurumsalKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + ")";

            }
            if (kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN  " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + ")";

            }


            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));

            if (!kriter.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "MUHASEBEKOD", "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "BIRIMKOD", "=", kriter.birimKod));
            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Onay belgesi düzenleyen farklı muhasebe sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Onay belgesi düzenleyen farklı muhasebe sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader OnayFarkliMuhasebeSayisi(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD] + ")) FROM " + TABSurec.TABLOAD;


            if (kriter.kurumsalKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + ")";

            }
            if (kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN  " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + ")";

            }


            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));

            if (!kriter.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "MUHASEBEKOD", "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "BIRIMKOD", "=", kriter.birimKod));
            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Süreç düzenleyen farklı birim sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        ///  bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader SurecFarkliBirimSayisi(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD] + "+" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD] + ")) FROM " + TABSurec.TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));

            if (!kriter.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "MUHASEBEKOD", "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "BIRIMKOD", "=", kriter.birimKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Süreç düzenleyen farklı muhasebe sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        ///  bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader SurecFarkliMuhasebeSayisi(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD] + ")) FROM " + TABSurec.TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "=", (int)ENUMSurecDurum.IPTAL));

            if (!kriter.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "MUHASEBEKOD", "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "BIRIMKOD", "=", kriter.birimKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// İşlem türüne ait süreç sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// İşlem türüne ait süreç sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader IslemTuruSurecSayisi(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + ") FROM " + TABSurec.TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));

            if (!kriter.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "MUHASEBEKOD", "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "BIRIMKOD", "=", kriter.birimKod));
            /*
                 if (kriter.altRaporTipi != 0)
                 {
                     kosul += " AND (" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + TABAboneFatura.TABLOAD+ ".SURECNO";
                     kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABAboneFatura.TABLOAD, TABAboneFatura.ALANLAR[(int)TABAboneFatura.ENUMAlanlar.ABONETIP], "=", kriter.altRaporTipi))+")";
                 }*/



            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Abone süreç fatura aşama sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Abone süreç fatura aşama sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AboneSurecFaturaAsamaSayisi(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + ")) FROM " + TABAboneFatura.TABLOAD + "," + TABSurec.TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));
            kosul += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + TABAboneFatura.TABLOAD + ".SURECNO";

            if (!kriter.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "MUHASEBEKOD", "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "BIRIMKOD", "=", kriter.birimKod));



            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Abone süreç ödeme aşama sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Abone süreç ödeme aşama sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AboneSurecOdemeAsamaSayisi(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + ")) FROM " + TABOdemeEmriForm.TABLOAD + "," + TABSurec.TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));
            kosul += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + " = " + TABOdemeEmriForm.TABLOAD + ".SURECNO";

            if (!kriter.baslamaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "MUHASEBEKOD", "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, "BIRIMKOD", "=", kriter.birimKod));



            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Alınmış ürün mukayese özet toplam tutar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Alınmış ürün mukayese özet toplam tutar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AlinmisUrunMukayeseOzetToplamTutarHesapla(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";
            string innerKosul = "";
            string grup = "";

            sql = "SELECT " + TABTasinirForm.TABLOADDETAY + ".HESAPPLANKOD" +
                "," + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABTasinirForm.TABLOADDETAY + ".MUHASEBEKOD)) MUHASEBESAYI" +
                "," + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABTasinirForm.TABLOADDETAY + ".BIRIMKOD)) BIRIMSAYI" +
                "," + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABTasinirForm.TABLOADDETAY + ".SURECNO)) ISLEMSAYI" +
                "," + VeriErisim.IslecBul("SUM") + "(" + TABTasinirForm.TABLOADDETAY + ".MIKTAR" + "*" + TABTasinirForm.TABLOADDETAY + ".BIRIMFIYAT" + ") TOPLAMTUTAR" +
                "," + VeriErisim.IslecBul("SUM") + "(" + TABTasinirForm.TABLOADDETAY + ".MIKTAR) TOPLAMMIKTAR" +
                ",(" + VeriErisim.IslecBul("SUM") + "(" + TABTasinirForm.TABLOADDETAY + ".MIKTAR" + "*" + TABTasinirForm.TABLOADDETAY + ".BIRIMFIYAT" + ")" +
                "/" + VeriErisim.IslecBul("SUM") + "(" + TABTasinirForm.TABLOADDETAY + ".MIKTAR)) ORTALAMABIRIM" +
                " FROM " + TABTasinirForm.TABLOADDETAY;
            //Burda bir parca kodlarda aralık verilirse calısıyormu ıncele

            if (kriter.muhasebeKod > 0 || kriter.birimKod != "" || !kriter.baslamaTarih.isNull || !kriter.bitisTarih.isNull)
            {

                sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";

                if (kriter.muhasebeKod > 0)
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebeKod));
                if (kriter.birimKod != "")
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD], "=", kriter.birimKod));
                if (!kriter.baslamaTarih.isNull)
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
                if (!kriter.bitisTarih.isNull)
                    innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

                innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
                innerKosul = VeriErisim.KosulEkle(innerKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));

                sql += innerKosul;
                sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABTasinirForm.TABLOADDETAY + ".SURECNO";
            }

            if (kriter.kurumsalKod != "" && kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABTasinirForm.TABLOADDETAY + ".SURECNO" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND ((";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + ")";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }
            else if (kriter.kurumsalKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABTasinirForm.TABLOADDETAY + ".SURECNO" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + "))";
            }
            else if (kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABTasinirForm.TABLOADDETAY + ".SURECNO" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTasinirForm.TABLOADDETAY, TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.HESAPPLANKOD], "LIKE", kriter.tasinirHesapKod + "%"));


            grup = TABTasinirForm.TABLOADDETAY + "." + TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.HESAPPLANKOD];

            if (kosul != "")
                sql += " WHERE " + kosul;

            if (grup != "")
                sql += " GROUP BY  " + grup;

            sql += " ORDER BY " + TABTasinirForm.TABLOADDETAY + "." + TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.HESAPPLANKOD];

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Taşınır hesap adı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kod">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Taşınır hesap adı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader TasinirHesapAdiniAl(string kod)
        {
            string sql = "";
            sql = "SELECT " + DBDegiskeni.TABLOADTASINIRHESAPKODVIEW + ".ACIKLAMA" +
           " FROM " + DBDegiskeni.TABLOADTASINIRHESAPKODVIEW +
           " WHERE " + DBDegiskeni.TABLOADTASINIRHESAPKODVIEW + ".KOD=" + kod;
            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Alınmış ürün mukayese detay toplam tutar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Alınmış ürün mukayese detay toplam tutar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AlinmisUrunMukayeseDetayToplamTutarHesapla(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";
            string icKosul = "";
            string grup = "";

            sql = "SELECT " + TABTasinirForm.TABLOADDETAY + ".MUHASEBEKOD" +
                "," + TABTasinirForm.TABLOADDETAY + ".BIRIMKOD" +
                "," + TABTasinirForm.TABLOADDETAY + ".SURECNO" +
                "," + VeriErisim.IslecBul("SUM") + "(" + TABTasinirForm.TABLOADDETAY + ".MIKTAR" + "*" + TABTasinirForm.TABLOADDETAY + ".BIRIMFIYAT" + ") TOPLAMTUTAR" +
                "," + VeriErisim.IslecBul("SUM") + "(" + TABTasinirForm.TABLOADDETAY + ".MIKTAR) TOPLAMMIKTAR" +
                ",(" + VeriErisim.IslecBul("SUM") + "(" + TABTasinirForm.TABLOADDETAY + ".MIKTAR" + "*" + TABTasinirForm.TABLOADDETAY + ".BIRIMFIYAT" + ")" +
                "/" + VeriErisim.IslecBul("SUM") + "(" + TABTasinirForm.TABLOADDETAY + ".MIKTAR)) ORTALAMABIRIM" +
                "," + TABTasinirForm.TABLOAD + ".TASINIRTARIH" +
                "," + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEAD" +
                "," + DBDegiskeni.TABLOADMUHASEBEBIRIM + ".BIRIMAD" +


                " FROM " + TABTasinirForm.TABLOADDETAY;
            //Burda bir parca kodlarda aralık verilirse calısıyormu ıncele


            sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";

            if (kriter.muhasebeKod > 0)
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD], "=", kriter.birimKod));
            if (!kriter.baslamaTarih.isNull)
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
            if (!kriter.bitisTarih.isNull)
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

            icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
            icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));


            sql += icKosul;
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + "=" + TABTasinirForm.TABLOADDETAY + ".YIL";
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABTasinirForm.TABLOADDETAY + ".SURECNO";


            if (kriter.kurumsalKod != "" && kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABTasinirForm.TABLOADDETAY + ".SURECNO" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND " + TABTasinirForm.TABLOADDETAY + ".YIL" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".YIL";
                sql += " AND ((";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + ")";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }
            else if (kriter.kurumsalKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABTasinirForm.TABLOADDETAY + ".SURECNO" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND " + TABTasinirForm.TABLOADDETAY + ".YIL" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".YIL";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + "))";
            }
            else if (kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABTasinirForm.TABLOADDETAY + ".SURECNO" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND " + TABTasinirForm.TABLOADDETAY + ".YIL" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".YIL";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }

            sql += " INNER JOIN " + DBDegiskeni.TABLOADMUHASEBE + " ON " + TABTasinirForm.TABLOADDETAY + ".MUHASEBEKOD" + " = " + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEKOD ";
            //sql += " INNER JOIN " + DBDegiskeni.TABLOADMUHASEBEBIRIM + " ON " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD] + " = " + DBDegiskeni.TABLOADMUHASEBEBIRIM + ".BIRIMKOD"; 
            sql += " INNER JOIN " + TABTasinirForm.TABLOAD + " ON " + TABTasinirForm.TABLOADDETAY + ".SURECNO" + " = " + TABTasinirForm.TABLOAD + ".SURECNO" + " AND  " +
                                                                        TABTasinirForm.TABLOADDETAY + ".YIL" + " = " + TABTasinirForm.TABLOAD + ".YIL";


            sql += " LEFT OUTER JOIN HRCMUHASEBEBIRIMVIEW ON HRCMUHASEBEBIRIMVIEW.MUHASEBEKOD = HRCTASINIRDETAY.MUHASEBEKOD AND HRCMUHASEBEBIRIMVIEW.BIRIMKOD = HRCTASINIRDETAY.BIRIMKOD ";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTasinirForm.TABLOADDETAY, TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.HESAPPLANKOD], "LIKE", kriter.tasinirHesapKod.Replace(".", "") + "%"));


            grup += TABTasinirForm.TABLOADDETAY + "." + TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.MUHASEBEKOD];
            grup += "," + TABTasinirForm.TABLOADDETAY + "." + TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.BIRIMKOD];
            grup += "," + TABTasinirForm.TABLOADDETAY + "." + TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.SURECNO];
            grup += "," + TABTasinirForm.TABLOAD + "." + TABTasinirForm.ALANLAR[(int)TABTasinirForm.ENUMAlanlar.TASINIRTARIH];
            grup += "," + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEAD";
            grup += "," + DBDegiskeni.TABLOADMUHASEBEBIRIM + ".BIRIMAD";

            if (kosul != "")
                sql += " WHERE " + kosul;

            if (grup != "")
                sql += " GROUP BY  " + grup;

            sql += " ORDER BY ";
            sql += TABTasinirForm.TABLOADDETAY + "." + TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.MUHASEBEKOD];
            sql += "," + TABTasinirForm.TABLOADDETAY + "." + TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.BIRIMKOD];
            sql += "," + TABTasinirForm.TABLOADDETAY + "." + TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.SURECNO];
            sql += "," + TABTasinirForm.TABLOAD + "." + TABTasinirForm.ALANLAR[(int)TABTasinirForm.ENUMAlanlar.TASINIRTARIH];
            sql += "," + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEAD";
            sql += "," + DBDegiskeni.TABLOADMUHASEBEBIRIM + ".BIRIMAD";

            veriErisim.sureAsimi = 120;
            return veriErisim.DataReaderDondur(sql);

        }
        /// <summary>
        /// Alınmış ürün mukayese özet sıralı birim fiyat bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <param name="artan">Listeleme için kullanılacak sıralama şekli parametresi</param>
        /// <returns>
        /// Alınmış ürün mukayese özet sıralı birim fiyat bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AlinmisUrunMukayeseOzetSiraliBirimFiyatHesapla(RaporKriter kriter, bool artan)
        {
            string sql = "";
            string kosul = "";
            string icKosul = "";

            sql = "SELECT " + TABTasinirForm.TABLOADDETAY + ".MIKTAR" + "," + TABTasinirForm.TABLOADDETAY + ".BIRIMFIYAT" +
                " FROM " + TABTasinirForm.TABLOADDETAY;
            //Burda bir parca kodlarda aralık verilirse calısıyormu ıncele

            if (kriter.muhasebeKod > 0 || kriter.birimKod != "" || !kriter.baslamaTarih.isNull || !kriter.bitisTarih.isNull)
            {

                sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";

                if (kriter.muhasebeKod > 0)
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebeKod));
                if (kriter.birimKod != "")
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.BIRIMKOD], "=", kriter.birimKod));
                if (!kriter.baslamaTarih.isNull)
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], ">=", kriter.baslamaTarih));
                if (!kriter.bitisTarih.isNull)
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECTARIH], "<=", kriter.bitisTarih));

                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.ISLEMKOD], "=", kriter.islemKodu));
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "=", (int)ENUMSurecDurum.IPTAL));

                sql += icKosul;
                sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABTasinirForm.TABLOADDETAY + ".SURECNO";
            }

            if (kriter.kurumsalKod != "" && kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABTasinirForm.TABLOADDETAY + ".SURECNO" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND ((";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + ")";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }
            else if (kriter.kurumsalKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABTasinirForm.TABLOADDETAY + ".SURECNO" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD1", "LIKE", kriter.kurumsalKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "KURUMSALKOD2", "LIKE", kriter.kurumsalKod + "%")) + "))";
            }
            else if (kriter.ekonomikKod != "")
            {
                sql += " INNER JOIN " + DBDegiskeni.TABLOADONAYFORMVIEW + " ON (" + TABTasinirForm.TABLOADDETAY + ".SURECNO" + " = " + DBDegiskeni.TABLOADONAYFORMVIEW + ".SURECNO";
                sql += " AND (";
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD1", "LIKE", kriter.ekonomikKod + "%"));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(DBDegiskeni.TABLOADONAYFORMVIEW, "EKONOMIKKOD2", "LIKE", kriter.ekonomikKod + "%")) + "))";
            }

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTasinirForm.TABLOADDETAY, TABTasinirForm.ALANLARDETAY[(int)TABTasinirForm.ENUMAlanlarDetay.HESAPPLANKOD], "LIKE", kriter.tasinirHesapKod + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            if (artan)
                sql += " ORDER BY " + TABTasinirForm.TABLOADDETAY + ".BIRIMFIYAT  ASC";
            else
                sql += " ORDER BY " + TABTasinirForm.TABLOADDETAY + ".BIRIMFIYAT  DESC";

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// İl karşılaştırma özet tüm personel sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// İl karşılaştırma özet tüm personel sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader IlKarsilastirmaOzetTumPersonelSayisiHesapla(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + DBDegiskeni.TABLOADPERSONEL + ".TCKIMLIKNO)" +

            " FROM " + DBDegiskeni.TABLOADPERSONEL;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(DBDegiskeni.TABLOADPERSONEL, "MUHASEBEKODU", "!=", 0));
            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// İl karşılaştırma özet toplam tutar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// İl karşılaştırma özet toplam tutar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader IlKarsilastirmaOzetToplamTutarHesapla(RaporKriter kriter)
        {
            string sql = "";
            //string kosul = "";
            //string icKosul = "";
            //string grup = "";

            //sql = "SELECT " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000" +
            //     "," + DBDegiskeni.TABLOADHRCIL + ".AD" +
            //     "," + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ")" +
            //     "," + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI" +
            //     "," + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ")/" +
            //     DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI" +
            //    " FROM " + DBDegiskeni.TABLOADHRCIL + "," + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + "," + TABOdemeEmriForm.TABLOADDETAY;
            ////Burda bir parca kodlarda aralık verilirse calısıyormu ıncele

            //if (!kriter.baslamaTarih.isNull || !kriter.bitisTarih.isNull)
            //{
            //    sql += " INNER JOIN " + TABOdemeEmriForm.TABLOAD + " ON ";

            //    if (!kriter.baslamaTarih.isNull)
            //        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOAD, TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH], ">=", kriter.baslamaTarih));
            //    if (!kriter.bitisTarih.isNull)
            //        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOAD, TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH], "<=", kriter.bitisTarih));
            //    sql += icKosul;
            //    sql += " AND " + TABOdemeEmriForm.TABLOAD + "." + TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.SURECNO] + "=" + TABOdemeEmriForm.TABLOADDETAY + ".SURECNO";
            //}
            //if (kriter.muhasebeKod > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD], "=", kriter.muhasebeKod));
            //if (kriter.birimKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD], "=", kriter.birimKod));
            //if (kriter.ekonomikKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.YANSITMAHESAPKOD], "LIKE", "830" + kriter.ekonomikKod.Replace(".", "") + "%"));
            //if (kriter.kurumsalKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.TERTIP], "LIKE", kriter.kurumsalKod.Replace(".", "") + "%"));
            //if (kosul != "")
            //    kosul += " AND ";
            //kosul += TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000" + "=" +
            //    DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".ILKODU";
            //kosul += " AND " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000" + "=" + DBDegiskeni.TABLOADHRCIL + ".KOD";

            //grup += TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000" +
            //"," + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI" +
            //   "," + DBDegiskeni.TABLOADHRCIL + ".AD";
            //if (kosul != "")
            //    sql += " WHERE " + kosul;

            //if (grup != "")
            //    sql += " GROUP BY  " + grup;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// İl karşılaştırma özet Türkiye toplam bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// İl karşılaştırma özet Türkiye toplam bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader IlKarsilastirmaOzetTurkiyeToplamiHesapla(RaporKriter kriter)
        {
            string sql = "";
            string kosul = "";
            string icKosul = "";

            sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ")" +
                " FROM " + TABOdemeEmriForm.TABLOADDETAY;
            //Burda bir parca kodlarda aralık verilirse calısıyormu ıncele

            if (!kriter.baslamaTarih.isNull || !kriter.bitisTarih.isNull)
            {
                sql += " INNER JOIN " + TABOdemeEmriForm.TABLOAD + " ON ";

                if (!kriter.baslamaTarih.isNull)
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOAD, TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH], ">=", kriter.baslamaTarih));
                if (!kriter.bitisTarih.isNull)
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOAD, TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH], "<=", kriter.bitisTarih));
                sql += icKosul;
                sql += " AND " + TABOdemeEmriForm.TABLOAD + "." + TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.SURECNO] + "=" + TABOdemeEmriForm.TABLOADDETAY + ".SURECNO";
            }
            if (kriter.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD], "=", kriter.muhasebeKod));
            if (kriter.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD], "=", kriter.birimKod));
            if (kriter.ekonomikKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.YANSITMAHESAPKOD], "LIKE", "830" + kriter.ekonomikKod.Replace(".", "") + "%"));
            if (kriter.kurumsalKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.TERTIP], "LIKE", kriter.kurumsalKod.Replace(".", "") + "%"));


            if (kosul != "")
                sql += " WHERE " + kosul;


            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// İl karşılaştırma detay toplam bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// İl karşılaştırma detay toplam bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader IlKarsilastirmaDetayToplamTutarHesapla(RaporKriter kriter)
        {
            string sql = "";
            //string kosul = "";
            //string icKosul = "";
            //string grup = "";

            //sql = "SELECT " + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEAD" +
            //    "," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000 ILKODU" +
            //    "," + DBDegiskeni.TABLOADHRCIL + ".AD" +
            //     "," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] +
            //     "," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD] +
            //     "," + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ")" +
            //     "," + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".SAYI" +
            //     "," + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ")/" +
            //     DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".SAYI" +//Birim e gore gruplu personel view ini oluştur
            //    " FROM " + DBDegiskeni.TABLOADHRCIL + "," + DBDegiskeni.TABLOADMUHASEBE + "," + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + "," + TABOdemeEmriForm.TABLOADDETAY;
            ////Burda bir parca kodlarda aralık verilirse calısıyormu ıncele

            //if (!kriter.baslamaTarih.isNull || !kriter.bitisTarih.isNull)
            //{
            //    sql += " INNER JOIN " + TABOdemeEmriForm.TABLOAD + " ON ";

            //    if (!kriter.baslamaTarih.isNull)
            //        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOAD, TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH], ">=", kriter.baslamaTarih));
            //    if (!kriter.bitisTarih.isNull)
            //        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOAD, TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH], "<=", kriter.bitisTarih));
            //    sql += icKosul;
            //    sql += " AND " + TABOdemeEmriForm.TABLOAD + "." + TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.SURECNO] + "=" + TABOdemeEmriForm.TABLOADDETAY + ".SURECNO";
            //}
            //if (kriter.muhasebeKod > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD], "=", kriter.muhasebeKod));
            //if (kriter.birimKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD], "=", kriter.birimKod));
            //if (kriter.ekonomikKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.YANSITMAHESAPKOD], "LIKE", "830" + kriter.ekonomikKod.Replace(".", "") + "%"));
            //if (kriter.kurumsalKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.TERTIP], "LIKE", kriter.kurumsalKod.Replace(".", "") + "%"));

            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD], ">", 1000));

            //kosul += " AND " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "=" +
            //    DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".MUHASEBEKODU";
            //kosul += " AND " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD] + "=" +
            //   DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".BIRIMKODU";

            //kosul += " AND " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "=" +
            //   DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEKOD";
            //kosul += " AND " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000" + "=" +
            // DBDegiskeni.TABLOADHRCIL + ".KOD";

            //grup += TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD];
            //grup += "," + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEAD";
            //grup += "," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000";
            //grup += "," + DBDegiskeni.TABLOADHRCIL + ".AD";
            //grup += "," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD];
            //grup += "," + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".SAYI";

            //if (kosul != "")
            //    sql += " WHERE " + kosul;

            //if (grup != "")
            //    sql += " GROUP BY  " + grup;

            //sql += " ORDER BY " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] +
            //"," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD];
            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// İl karşılaştırma özet merkez toplam tutar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// İl karşılaştırma özet merkez toplam tutar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader IlKarsilastirmaOzetMerkezToplamTutarHesapla(RaporKriter kriter)
        {
            string sql = "";
            //string kosul = "";
            //string icKosul = "";
            //string grup = "";

            //sql = "SELECT " + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEAD" +
            //    "," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000 ILKODU" +
            //    "," + DBDegiskeni.TABLOADHRCIL + ".AD" +
            //     "," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] +
            //     "," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD] +
            //     "," + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ")" +
            //     "," + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".SAYI" +
            //     "," + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ")/" +
            //     DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".SAYI" +//Birim e gore gruplu personel view ini oluştur
            //    " FROM " + DBDegiskeni.TABLOADHRCIL + "," + DBDegiskeni.TABLOADMUHASEBE + "," + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + "," + TABOdemeEmriForm.TABLOADDETAY;
            ////Burda bir parca kodlarda aralık verilirse calısıyormu ıncele

            //if (!kriter.baslamaTarih.isNull || !kriter.bitisTarih.isNull)
            //{
            //    sql += " INNER JOIN " + TABOdemeEmriForm.TABLOAD + " ON ";

            //    if (!kriter.baslamaTarih.isNull)
            //        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOAD, TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH], ">=", kriter.baslamaTarih));
            //    if (!kriter.bitisTarih.isNull)
            //        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOAD, TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH], "<=", kriter.bitisTarih));
            //    sql += icKosul;
            //    sql += " AND " + TABOdemeEmriForm.TABLOAD + "." + TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.SURECNO] + "=" + TABOdemeEmriForm.TABLOADDETAY + ".SURECNO";
            //}
            //if (kriter.muhasebeKod > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD], "=", kriter.muhasebeKod));
            //if (kriter.birimKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD], "=", kriter.birimKod));
            //if (kriter.ekonomikKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.YANSITMAHESAPKOD], "LIKE", "830" + kriter.ekonomikKod.Replace(".", "") + "%"));
            //if (kriter.kurumsalKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.TERTIP], "LIKE", kriter.kurumsalKod.Replace(".", "") + "%"));

            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD], "<", 1000));

            //kosul += " AND " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "=" +
            //    DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".MUHASEBEKODU";
            //kosul += " AND " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD] + "=" +
            //   DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".BIRIMKODU";

            //kosul += " AND " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "=" +
            //   DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEKOD";
            //kosul += " AND " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000" + "=" +
            // DBDegiskeni.TABLOADHRCIL + ".KOD";

            //grup += TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD];
            //grup += "," + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEAD";
            //grup += "," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000";
            //grup += "," + DBDegiskeni.TABLOADHRCIL + ".AD";
            //grup += "," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD];
            //grup += "," + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".SAYI";



            //if (kosul != "")
            //    sql += " WHERE " + kosul;

            //if (grup != "")
            //    sql += " GROUP BY  " + grup;

            //sql += " ORDER BY " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] +
            //"," + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD];

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// İl karşılaştırma özet toplam harcama bazlı sıralama bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <param name="artan">Listeleme için kullanılacak sıralama şekli parametresi</param>
        /// <param name="kisiBasiHarcama">Eğer <c>true</c> verilirse kişi başı harcamayı listeler</param>
        /// <returns>
        /// İl karşılaştırma özet toplam harcama bazlı sıralama bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader IlKarsilastirmaOzetToplamHarcamaBazlıSiralama(RaporKriter kriter, bool artan, bool kisiBasiHarcama)
        {
            string sql = "";
            //string kosul = "";
            //string icKosul = "";
            //string grup = "";

            //sql = "SELECT " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000" +
            //     "," + DBDegiskeni.TABLOADHRCIL + ".AD" +
            //     "," + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ") TOPLAMHARCAMA" +
            //     "," + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI" +
            //     "," + VeriErisim.IslecBul("SUM") + "(" + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BORC] + ")/" +
            //     DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI KISIBASIHARCAMA" +
            //    " FROM " + DBDegiskeni.TABLOADHRCIL + "," + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + "," + TABOdemeEmriForm.TABLOADDETAY;
            ////Burda bir parca kodlarda aralık verilirse calısıyormu ıncele

            //if (!kriter.baslamaTarih.isNull || !kriter.bitisTarih.isNull)
            //{
            //    sql += " INNER JOIN " + TABOdemeEmriForm.TABLOAD + " ON ";

            //    if (!kriter.baslamaTarih.isNull)
            //        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOAD, TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH], ">=", kriter.baslamaTarih));
            //    if (!kriter.bitisTarih.isNull)
            //        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOAD, TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.ODEMEEMRITARIH], "<=", kriter.bitisTarih));
            //    sql += icKosul;
            //    sql += " AND " + TABOdemeEmriForm.TABLOAD + "." + TABOdemeEmriForm.ALANLAR[(int)TABOdemeEmriForm.ENUMAlanlar.SURECNO] + "=" + TABOdemeEmriForm.TABLOADDETAY + ".SURECNO";
            //}
            //if (kriter.muhasebeKod > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD], "=", kriter.muhasebeKod));
            //if (kriter.birimKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.BIRIMKOD], "=", kriter.birimKod));
            //if (kriter.ekonomikKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.YANSITMAHESAPKOD], "LIKE", "830" + kriter.ekonomikKod.Replace(".", "") + "%"));
            //if (kriter.kurumsalKod != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABOdemeEmriForm.TABLOADDETAY, TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.TERTIP], "LIKE", kriter.kurumsalKod.Replace(".", "") + "%"));
            //if (kosul != "")
            //    kosul += " AND ";
            //kosul += TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000" + "=" +
            //    DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".ILKODU";
            //kosul += " AND " + TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000" + "=" + DBDegiskeni.TABLOADHRCIL + ".KOD";

            //grup += TABOdemeEmriForm.TABLOADDETAY + "." + TABOdemeEmriForm.ALANLARDETAY[(int)TABOdemeEmriForm.ENUMAlanlarDetay.MUHASEBEKOD] + "/1000" +
            //"," + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI" +
            //   "," + DBDegiskeni.TABLOADHRCIL + ".AD";
            //if (kosul != "")
            //    sql += " WHERE " + kosul;

            //if (grup != "")
            //    sql += " GROUP BY  " + grup;
            //if (kisiBasiHarcama)
            //{
            //    if (artan)
            //        sql += " ORDER BY KISIBASIHARCAMA ASC";
            //    else
            //        sql += " ORDER BY KISIBASIHARCAMA DESC";
            //}
            //else
            //{
            //    if (artan)
            //        sql += " ORDER BY TOPLAMHARCAMA ASC";
            //    else
            //        sql += " ORDER BY TOPLAMHARCAMA DESC";
            //}


            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Abonelik istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Abonelik istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AbonelikIstatistik(RaporKriter kriter)
        {
            string sql = "";
            string turkiyeAlan = "";
            string merkezAlan = "HRCABONEFATURA.MUHASEBEKOD, HRCABONEFATURA.BIRIMKOD";
            string tasraAlan = "HRCABONEFATURA.MUHASEBEKOD/1000";
            string ilAlan = "HRCABONEFATURA.MUHASEBEKOD";
            string ilceAlan = "HRCABONEFATURA.MUHASEBEKOD, HRCABONEFATURA.BIRIMKOD";

            string alanlar = "";
            switch (kriter.raporSekli)
            {
                case 1:
                    alanlar = turkiyeAlan;
                    break;
                case 2:
                    alanlar = merkezAlan;
                    break;
                case 3:
                    alanlar = tasraAlan;
                    break;
                case 4:
                    alanlar = ilAlan;
                    break;
                case 5:
                    alanlar = ilceAlan;
                    break;
                default:
                    break;
            }

            sql = "SELECT SUM(HRCABONEFATURA.FATURATUTAR),COUNT(*)";
            if (alanlar != "") sql += "," + alanlar;
            sql += " FROM HRCABONEFATURA INNER JOIN";
            sql += " HRCSUREC ON HRCABONEFATURA.YIL = HRCSUREC.YIL AND HRCABONEFATURA.SURECNO = HRCSUREC.SURECNO AND ";
            sql += " HRCABONEFATURA.MUHASEBEKOD = HRCSUREC.MUHASEBEKOD AND HRCABONEFATURA.BIRIMKOD = HRCSUREC.BIRIMKOD";
            sql += " WHERE HRCSUREC.SURECDURUM < 9 ";
            sql += " AND HRCABONEFATURA.ABONETIP = " + kriter.aboneTip;
            if (kriter.yil > 0)
                sql += " AND HRCABONEFATURA.FATURAYIL = " + kriter.yil;
            if (kriter.ay > 0)
                sql += " AND HRCABONEFATURA.FATURAAY = " + kriter.ay;
            if (kriter.muhasebeKod > 0)
                sql += " AND HRCABONEFATURA.MUHASEBEKOD = " + kriter.muhasebeKod;
            if (kriter.birimKod != "")
                sql += " AND HRCABONEFATURA.BIRIMKOD = '" + kriter.birimKod + "'";
            if (kriter.il == 99)
                sql += " AND HRCABONEFATURA.MUHASEBEKOD/1000 > 0 ";
            else if (kriter.il > -1)
                sql += " AND HRCABONEFATURA.MUHASEBEKOD/1000 = " + kriter.il;

            if (alanlar != "")
            {
                sql += " GROUP BY " + alanlar;
                sql += " ORDER BY " + alanlar;
            }

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Personel istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Personel istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader PersonelIstatistik(RaporKriter kriter)
        {
            string sql = "";
            string turkiyeAlan = "";
            string merkezAlan = "MUHASEBEKODU, BIRIMKODU";
            string tasraAlan = "ILKODU";
            string ilAlan = "MUHASEBEKODU";
            string ilceAlan = "MUHASEBEKODU, BIRIMKODU";


            string alanlar = "";
            switch (kriter.raporSekli)
            {
                case 1:
                    alanlar = turkiyeAlan;
                    break;
                case 2:
                    alanlar = merkezAlan;
                    break;
                case 3:
                    alanlar = tasraAlan;
                    break;
                case 4:
                    alanlar = ilAlan;
                    break;
                case 5:
                    alanlar = ilceAlan;
                    break;
                default:
                    break;
            }

            sql = "SELECT SUM(SAYI)";
            if (alanlar != "") sql += "," + alanlar;
            sql += " FROM PERBIRIMDAGILIMIVIEW";
            sql += " WHERE MUHASEBEKODU >0 ";
            if (kriter.muhasebeKod > 0)
                sql += " AND MUHASEBEKODU = " + kriter.muhasebeKod;
            if (kriter.birimKod != "")
                sql += " AND BIRIMKODU = '" + kriter.birimKod + "'";
            if (kriter.il == 99)
                sql += " AND ILKODU > 0 ";
            else if (kriter.il > -1)
                sql += " AND ILKODU = " + kriter.il;

            if (alanlar != "")
            {
                sql += " GROUP BY " + alanlar;
                sql += " ORDER BY " + alanlar;
            }

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Abonelik ortalama istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Abonelik ortalama istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AbonelikOrtalamaIstatistik(RaporKriter kriter)
        {
            string sql = "";

            sql = "SELECT AVG(HRCABONEFATURA.FATURATUTAR)";
            sql += " FROM HRCABONEFATURA INNER JOIN";
            sql += " HRCSUREC ON HRCABONEFATURA.YIL = HRCSUREC.YIL AND HRCABONEFATURA.SURECNO = HRCSUREC.SURECNO AND ";
            sql += " HRCABONEFATURA.MUHASEBEKOD = HRCSUREC.MUHASEBEKOD AND HRCABONEFATURA.BIRIMKOD = HRCSUREC.BIRIMKOD";
            sql += " WHERE HRCSUREC.SURECDURUM <> 9 ";
            sql += " AND HRCABONEFATURA.ABONETIP = " + kriter.aboneTip;
            if (kriter.yil > 0)
                sql += " AND HRCABONEFATURA.FATURAYIL = " + kriter.yil;
            if (kriter.ay > 0)
                sql += " AND HRCABONEFATURA.FATURAAY = " + kriter.ay;
            if (kriter.muhasebeKod > 0)
                sql += " AND HRCABONEFATURA.MUHASEBEKOD = " + kriter.muhasebeKod;
            if (kriter.birimKod != "")
                sql += " AND HRCABONEFATURA.BIRIMKOD = '" + kriter.birimKod + "'";
            else if (kriter.il > -1)
                sql += " AND HRCABONEFATURA.MUHASEBEKOD/1000 = " + kriter.il;

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Sürekli görev yolluk özet emekli bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sürekli görev yolluk özet emekli bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader SGYollukOzetEmekli(RaporKriter kriter)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sürekli görev yolluk özet atanmış bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sürekli görev yolluk özet atanmış bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader SGYollukOzetAtanmis(RaporKriter kriter)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sürekli görev yolluk özet il liste bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak rapor kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sürekli görev yolluk özet il liste bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader SGYollukOzetIlListesi(RaporKriter kriter)
        {
            throw new NotImplementedException();
        }

        public static string BirimListesiKYKosulOl(string baglac, string tabloAd, string alanAd, TNSCollection birimKodlar)
        {
            string inKosul = "";
            if (birimKodlar != null)
            {
                foreach (string bir in birimKodlar)
                {
                    if (inKosul != "") inKosul += " OR ";
                    inKosul += VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", bir.Replace(".", "") + "%");
                }
            }

            if (inKosul != "")
            {
                inKosul = " " + baglac + "(" + inKosul + ")";
            }
            return inKosul;
        }

        public static string BirimListeleStrKYSql(TNSCollection birimKodlar)
        {
            string sql = "SELECT KY.KOD, KY.AD, KY.SAYMANLIKKOD, KY.BIRIMKOD, KY.KURKOD FROM BHMKURUMSALYAPI KY";

            string kosul = "";

            //if (ilKod > 0)
            //{
            //    string ilKosulSql;
            //    if (ilKod == 99)
            //    {
            //        ilKosulSql = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl("KY", "KOD", "LIKE", "900%"));
            //    }
            //    else
            //    {
            //        ilKosulSql = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl("KY", "KOD", "LIKE", "_." + ilKod.ToString()));
            //        if (ilKod == 6)
            //        {
            //            ilKosulSql = VeriErisim.KosulEkle(ilKosulSql, "OR", VeriErisim.KosulOl("KY", "KOD", "LIKE", "100%"));
            //            ilKosulSql = VeriErisim.KosulEkle(ilKosulSql, "OR", VeriErisim.KosulOl("KY", "KOD", "LIKE", "300%"));
            //        }
            //    }
            //    ilKosulSql = "(" + ilKosulSql + ")";
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", ilKosulSql);
            //}

            kosul = VeriErisim.KosulEkle(kosul, "AND", "LEN(KY.KOD) = 11");
            kosul = VeriErisim.KosulEkle(kosul, "AND", BirimListesiKYKosulOl("", "KY", "KOD", birimKodlar));
            kosul = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            kosul += " ORDER BY 1";
            return kosul;
        }


        //public static string BirimListeleStrMuhSql(MuhasebeBirim muhasebeBirim, bool kurumsalYapili, TNSCollection birimKodlar, bool kyOnde = false)
        //{
        //    string alanAd = "RIGHT('00000' + CONVERT(VARCHAR, O.MUHASEBEKOD),5)";
        //    string join;
        //    if (kurumsalYapili)
        //        join = "INNER";
        //    else
        //        join = "LEFT OUTER";
        //    string on = "";
        //    string son = "";
        //    if (kyOnde)
        //        on = "KY.KOD, ";
        //    else
        //        son = "KY.KOD, ";

        //    string sql = "SELECT " + on + alanAd + ", O.BIRIMKOD, O.KURKOD, M.MUHASEBEAD, O.BIRIMAD, " + son + " KY.AD FROM HRCMUHASEBEVIEW M INNER JOIN HRCMUHASEBEBIRIMVIEW O ON M.MUHASEBEKOD = O.MUHASEBEKOD " + join + " JOIN BHMKURUMSALYAPI KY ON KY.SAYMANLIKKOD = O.MUHASEBEKOD AND KY.BIRIMKOD=O.BIRIMKOD AND KY.KURKOD=O.KURKOD";

        //    string kosul = "";
        //    if (muhasebeBirim != null)
        //    {
        //        if (muhasebeBirim.plakaKod > 0)
        //        {
        //            string ilKosulSql = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl("O", "MUHASEBEKOD", ">", muhasebeBirim.plakaKod * 1000));
        //            ilKosulSql = VeriErisim.KosulEkle(ilKosulSql, "AND", VeriErisim.KosulOl("O", "MUHASEBEKOD", "<", muhasebeBirim.plakaKod * 1000 + 999));
        //            if (muhasebeBirim.plakaKod == 6)
        //            {
        //                ilKosulSql = VeriErisim.KosulEkle(ilKosulSql, "OR", VeriErisim.KosulOl("O", "MUHASEBEKOD", ">", 0));
        //                ilKosulSql = VeriErisim.KosulEkle(ilKosulSql, "AND", VeriErisim.KosulOl("O", "MUHASEBEKOD", "<", 999));
        //            }
        //            ilKosulSql = "(" + ilKosulSql + ")";
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", ilKosulSql);
        //        }

        //        if (!string.IsNullOrWhiteSpace(muhasebeBirim.birimKod))
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", "O.BIRIMKOD = " + VeriErisim.DegerYap(muhasebeBirim.birimKod));

        //        if (!string.IsNullOrWhiteSpace(muhasebeBirim.kurKod))
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", "O.KURKOD = " + VeriErisim.DegerYap(muhasebeBirim.kurKod.Replace(".", "")));
        //    }
        //    kosul = VeriErisim.KosulEkle(kosul, "AND", BirimListesiKosulOl("", "O", birimKodlar));
        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("O", "BIRIMKOD", "<>", "000"));
        //    kosul = VeriErisim.KosulEkle(sql, "WHERE", kosul);
        //    kosul += " ORDER BY 1,2,3";
        //    return kosul;
        //}

        public static string BirimListesiKosulOl(string baglac, string tabloAd, TNSCollection birimKodlar, string sayAlanAd = "MUHASEBEKOD")
        {
            string inKosul = "";
            if (birimKodlar != null)
            {
                foreach (TNS.HRC.MuhasebeBirim bir in birimKodlar)
                {
                    if (inKosul != "") inKosul += ",";
                    inKosul += VeriErisim.DegerYap(bir.muhasebeKod + "." + bir.birimKod + "." + bir.kurKod.Replace(".", ""));
                }
            }
            if (inKosul != "")
            {
                inKosul = " " + baglac + string.Format(" (CONVERT(VARCHAR, {0}{1})+'.'+{0}BIRIMKOD+'.'+{0}KURKOD IN (", tabloAd + ".", sayAlanAd) + inKosul + "))";
            }
            return inKosul;
        }

        //public IDataReader HarcamaBirimiListele(MuhasebeBirim muhasebeBirim, bool kurumsalYapili)
        //{
        //    string sql = BirimListeleStrMuhSql(muhasebeBirim, kurumsalYapili, null);
        //    return veriErisim.DataReaderDondur(sql);
        //}
    }
}
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Performans bütçe modülü raporları ile ilgili select sql sorgularını tutan sınıf
    /// </summary>
    public class TABRaporlar : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABRaporlar(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Personel giderlerinin ekonomik kodu
        /// </summary>
        private static string personelGider = "01";

        /// <summary>
        /// Parametre olarak verilen faaliyet/proje türünü kullanarak faaliyet/projenin
        /// bilgilerini tutan tablo alanlarını virgüllerle ayırarak döndüren yordam
        /// </summary>
        /// <param name="fpTur">ENUMFaaliyetProjeTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Virgüllerle ayrılmış faaliyet/proje tablo alanları döndürülür.</returns>
        private string FPTAlanlari(int fpTur)
        {
            string sql = "PEBFPODENEKTOPLAMVIEW" + "." + "YIL";
            sql += ", PEBFPODENEKTOPLAMVIEW" + "." + "KURKOD";
            sql += ", PEBFPODENEKTOPLAMVIEW" + "." + "SENARYOKOD";
            sql += ", PEBFPODENEKTOPLAMVIEW" + "." + "FPKOD";
            sql += ", PEBFPODENEKTOPLAMVIEW" + "." + "FPAD";
            sql += ", PEBFPODENEKTOPLAMVIEW" + "." + "PROJEMI";
            sql += ", PEBFPODENEKTOPLAMVIEW" + "." + "TUR";
            if (fpTur != (int)ENUMFaaliyetProjeTur.DigerKurumIci && fpTur != (int)ENUMFaaliyetProjeTur.DigerKurumDisi)
            {
                sql += ", " + "PEBFAALIYETPROJEAGIRLIK.PERHEDEFKOD";
                sql += ", " + "PEBFAALIYETPROJEAGIRLIK.AGIRLIK";
            }
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// faaliyet/proje toplam sql cümlesinin koşul kısmını oluşturan ve döndüren yordam
        /// Dönen stringin başına WHERE mi, yoksa AND mi geleceğine çağıran fonksiyon karar vermelidir.
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="phkurKod">Birim kodu</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="phKod">Performans hedefin programatik kodu</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <param name="fpTur">ENUMFaaliyetProjeTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Sql cümlesinin koşul kısmı döndürülür.</returns>
        private string FPTKriterleri(int yil, string phkurKod, string senaryoKod, int phKod, int fpKod, int fpTur, int yansiyan, bool agirlikVar)
        {
            string sql = VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "YIL", "=", yil);
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));

            if (fpKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "FPKOD", "=", fpKod));

            if (agirlikVar)
            {
                if (phKod > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFAALIYETPROJEAGIRLIK", "PERHEDEFKOD", "=", phKod));
                if (!string.IsNullOrEmpty(phkurKod))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFAALIYETPROJEAGIRLIK", "PHKURKOD", "LIKE", phkurKod.Replace(".", "") + "%"));
            }
            else
            {
                //if (!string.IsNullOrEmpty(phkurKod) && fpKod > 0)
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "KURKOD", "LIKE", phkurKod.Replace(".", "") + "%"));
            }

            if (fpTur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "TUR", "=", fpTur));

            if (yansiyan > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "YANSIYAN", "=", yansiyan));

            return sql;
        }

        /// <summary>
        /// Faaliyet/projenin anahtar bilgilerini tutan tablo alanlarını virgüllerle ayırarak döndüren yordam
        /// </summary>
        /// <returns>Virgüllerle ayrılmış faaliyet/projenin anahtar tablo alanları döndürülür.</returns>
        private string FPTSiralama()
        {
            string sql = "PEBFPODENEKTOPLAMVIEW" + "." + "YIL";
            sql += ", PEBFPODENEKTOPLAMVIEW" + "." + "KURKOD";
            sql += ", PEBFPODENEKTOPLAMVIEW" + "." + "SENARYOKOD";
            sql += ", PEBFPODENEKTOPLAMVIEW" + "." + "FPKOD";
            return sql;
        }

        /// <summary>
        /// PEBFPODENEKTOPLAMVIEW ve PEBFAALIYETPROJEAGIRLIK tablolarının
        /// join edilmesi sonucu oluşan sql cümleciğini döndüren yordam
        /// </summary>
        /// <returns>Join edilmiş sql cümleciği döndürülür.</returns>
        private string FPTJoin()
        {
            string sql = "PEBFPODENEKTOPLAMVIEW" + "." + "YIL" + "=" + "PEBFAALIYETPROJEAGIRLIK" + "." + "YIL";
            sql += " AND " + "PEBFPODENEKTOPLAMVIEW" + "." + "KURKOD" + "=" + "PEBFAALIYETPROJEAGIRLIK" + "." + "FPKURKOD";
            sql += " AND " + "PEBFPODENEKTOPLAMVIEW" + "." + "FPKOD" + "=" + "PEBFAALIYETPROJEAGIRLIK" + "." + "FPKOD";
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve faaliyet/projelere ait kimlik,
        /// ağırlık ve ödenek bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="phkurKod">Birim kodu</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="phKod">Performans hedefin programatik kodu</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <param name="fpTur">ENUMFaaliyetProjeTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Verilen kriterlere uygun olan faaliyet/projelere ait kimlik, ağırlık ve ödenek bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader FaaliyetProjeToplam(int yil, string phkurKod, string senaryoKod, int phKod, int fpKod, int fpTur, int yansiyan)
        {
            string sql = "SELECT " + FPTAlanlari(fpTur);
            sql += ", " + VeriErisim.IslecBul("SUM") + "(PEBFPODENEKTOPLAMVIEW.ODENEK)";
            sql += " FROM " + "PEBFPODENEKTOPLAMVIEW";
            if (fpTur != (int)ENUMFaaliyetProjeTur.DigerKurumIci && fpTur != (int)ENUMFaaliyetProjeTur.DigerKurumDisi)
            {
                sql += ", PEBFAALIYETPROJEAGIRLIK";
                sql += " WHERE " + FPTJoin();
                sql = VeriErisim.KosulEkle(sql, "AND", FPTKriterleri(yil, phkurKod, senaryoKod, phKod, fpKod, fpTur, yansiyan, true));
            }
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", FPTKriterleri(yil, phkurKod, senaryoKod, phKod, fpKod, fpTur, yansiyan, false));

            sql += " GROUP BY " + FPTAlanlari(fpTur);
            sql += " ORDER BY " + FPTSiralama();

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve DigerKurumIci veya
        /// DigerKurumDisi türündeki faaliyet/projelerin ödenek toplamını döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="phKod">Performans hedefin programatik kodu</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <param name="fpTur">ENUMFaaliyetProjeTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Verilen kriterlere uygun olan faaliyet/projelerin ödenek toplamı döndürülür.</returns>
        public double DigerFaaliyetProjeToplam(int yil, string senaryoKod, int fpTur)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(PEBFPODENEKTOPLAMVIEW.ODENEK) FROM  PEBFPODENEKTOPLAMVIEW";
            sql = VeriErisim.KosulEkle(sql, "WHERE", FPTKriterleri(yil, "", senaryoKod, 0, 0, fpTur, -1, false));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Performans hedefin bilgilerini tutan tablo alanlarını virgüllerle ayırarak döndüren yordam
        /// </summary>
        /// <returns>Virgüllerle ayrılmış performans hedef tablo alanları döndürülür.</returns>
        private string PHTAlanlari()
        {
            string sql = "PEBPERHEDEF" + "." + "YIL";
            sql += ", PEBPERHEDEF" + "." + "KURKOD";
            sql += ", PEBPERHEDEF" + "." + "HEDEFKOD";
            sql += ", PEBPERHEDEF" + "." + "HEDEF";
            sql += ", " + "PEBPERHEDEFAGIRLIK.DONEMBYIL";
            sql += ", " + "PEBPERHEDEFAGIRLIK.STRHEDEFKOD";
            sql += ", " + "PEBPERHEDEFAGIRLIK.AGIRLIK";
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// performans hedef toplam sql cümlesinin koşul kısmını oluşturan ve döndüren yordam
        /// Dönen stringin başına WHERE mi, yoksa AND mi geleceğine çağıran fonksiyon karar vermelidir.
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedefin programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Sql cümlesinin koşul kısmı döndürülür.</returns>
        private string PHTKriterleri(int donemBYil, int strHedefKod, int yil, string kurKod)
        {
            string sql = VeriErisim.KosulOl("PEBPERHEDEFAGIRLIK", "DONEMBYIL", "=", donemBYil);
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEFAGIRLIK", "STRHEDEFKOD", "=", strHedefKod));

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEFAGIRLIK", "YIL", "=", yil));

            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEFAGIRLIK", "KURKOD", "LIKE", kurKod.Replace(".", "") + "%"));

            return sql;
        }

        /// <summary>
        /// Performans hedefin anahtar bilgilerini tutan tablo alanlarını virgüllerle ayırarak döndüren yordam
        /// </summary>
        /// <returns>Virgüllerle ayrılmış performans hedefin anahtar tablo alanları döndürülür.</returns>
        private string PHTSiralama()
        {
            string sql = "PEBPERHEDEF" + "." + "YIL";
            sql += ", PEBPERHEDEF" + "." + "KURKOD";
            sql += ", PEBPERHEDEF" + "." + "HEDEFKOD";
            return sql;
        }

        /// <summary>
        /// view parametresinde gönderilen view ile PEBFAALIYETPROJEAGIRLIK, PEBPERHEDEF
        /// ve PEBPERHEDEFAGIRLIK tablolarının join edilmesi sonucu oluşan sql cümleciğini döndüren yordam
        /// </summary>
        /// <param name="view">Join edilecek view</param>
        /// <returns>Join edilmiş sql cümleciği döndürülür.</returns>
        private string PHJoin(string view)
        {
            string kurumAlan = string.Empty;
            if (view == "PEBFPODENEKTOPLAMVIEW")
                kurumAlan = "KURKOD";
            else if (view == "PEBFPODENEKVIEW")
                kurumAlan = "BIRIM";
            else if (view == "PEBFPODENEKYANSIYANVIEW")
                kurumAlan = "FPKURKOD";

            string sql = view + "." + "YIL" + "=" + "PEBFAALIYETPROJEAGIRLIK" + "." + "YIL";
            sql += " AND " + view + "." + kurumAlan + "=" + "PEBFAALIYETPROJEAGIRLIK" + "." + "FPKURKOD";
            sql += " AND " + view + "." + "FPKOD" + "=" + "PEBFAALIYETPROJEAGIRLIK" + "." + "FPKOD";
            sql += " AND " + "PEBFAALIYETPROJEAGIRLIK" + "." + "YIL" + "=" + "PEBPERHEDEF" + "." + "YIL";
            sql += " AND " + "PEBFAALIYETPROJEAGIRLIK" + "." + "PHKURKOD" + "=" + "PEBPERHEDEF" + "." + "KURKOD";
            sql += " AND " + "PEBFAALIYETPROJEAGIRLIK" + "." + "PERHEDEFKOD" + "=" + "PEBPERHEDEF" + "." + "HEDEFKOD";
            sql += " AND " + "PEBPERHEDEF" + "." + "YIL" + "=" + "PEBPERHEDEFAGIRLIK" + "." + "YIL";
            sql += " AND " + "PEBPERHEDEF" + "." + "KURKOD" + "=" + "PEBPERHEDEFAGIRLIK" + "." + "KURKOD";
            sql += " AND " + "PEBPERHEDEF" + "." + "HEDEFKOD" + "=" + "PEBPERHEDEFAGIRLIK" + "." + "PERHEDEFKOD";
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve performans hedeflere ait kimlik,
        /// ağırlık ve ödenek bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedefin programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Verilen kriterlere uygun olan performans hedeflere ait kimlik, ağırlık ve ödenek bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader PerformansHedefToplam(int donemBYil, int strHedefKod, int yil, string senaryoKod, string kurKod)
        {
            string sql = "SELECT " + PHTAlanlari();
            sql += ", " + VeriErisim.IslecBul("SUM") + "(PEBFAALIYETPROJEAGIRLIK.AGIRLIK * PEBFPODENEKTOPLAMVIEW.ODENEK / 100)";
            sql += " FROM " + "PEBFPODENEKTOPLAMVIEW" + ", " + "PEBFAALIYETPROJEAGIRLIK";
            sql += ", " + "PEBPERHEDEF" + ", " + "PEBPERHEDEFAGIRLIK";
            sql += " WHERE " + PHJoin("PEBFPODENEKTOPLAMVIEW");

            sql = VeriErisim.KosulEkle(sql, "AND", PHTKriterleri(donemBYil, strHedefKod, yil, kurKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            sql += " GROUP BY " + PHTAlanlari();
            sql += " ORDER BY " + PHTSiralama();

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve performans göstergelerini
        /// ve ödeneklerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Verilen kriterlere uygun olan performans göstergelerini ve ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader PerformansGostergeOdenekToplam(int yil, string senaryoKod, string kurKod)
        {
            string sql = "SELECT PEBGOSTERGE.GOSTERGEKOD, PEBGOSTERGE.GOSTERGEAD, PEBGOSTERGE.OLCUBIRIMKOD, PEBGOSTERGE.TUR, PEBGOSTERGE.KULLANIMSEKLI, PEBGOSTERGE.SEVIYE";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(PEBFAALIYETPROJEAGIRLIK.AGIRLIK * PEBFPODENEKTOPLAMVIEW.ODENEK / 100)";
            sql += " FROM " + "PEBFPODENEKTOPLAMVIEW" + ", " + "PEBFAALIYETPROJEAGIRLIK";
            sql += ", " + "PEBGOSTERGEHEDEF" + ", " + "PEBGOSTERGE";
            sql += " WHERE " + "PEBFPODENEKTOPLAMVIEW" + "." + "YIL" + "=" + "PEBFAALIYETPROJEAGIRLIK" + "." + "YIL";
            sql += " AND " + "PEBFPODENEKTOPLAMVIEW" + "." + "KURKOD" + "=" + "PEBFAALIYETPROJEAGIRLIK" + "." + "FPKURKOD";
            sql += " AND " + "PEBFPODENEKTOPLAMVIEW" + "." + "FPKOD" + "=" + "PEBFAALIYETPROJEAGIRLIK" + "." + "FPKOD";
            sql += " AND " + "PEBFAALIYETPROJEAGIRLIK" + "." + "YIL" + "=" + "PEBGOSTERGEHEDEF" + "." + "YIL";
            sql += " AND " + "PEBFAALIYETPROJEAGIRLIK" + "." + "FPKURKOD" + "=" + "PEBGOSTERGEHEDEF" + "." + "GHKURKOD";
            sql += " AND " + "PEBFAALIYETPROJEAGIRLIK" + "." + "PHKURKOD" + "=" + "PEBGOSTERGEHEDEF" + "." + "PHKURKOD";
            sql += " AND " + "PEBFAALIYETPROJEAGIRLIK" + "." + "PERHEDEFKOD" + "=" + "PEBGOSTERGEHEDEF" + "." + "PERHEDEFKOD";
            sql += " AND " + "PEBGOSTERGEHEDEF" + "." + "GOSTERGEKOD" + "=" + "PEBGOSTERGE" + "." + "GOSTERGEKOD";

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "YIL", "=", yil));

            if (!string.IsNullOrEmpty(senaryoKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "KURKOD", "LIKE", kurKod.Replace(".", "") + "%"));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "SIRANO", "=", 1));

            sql += " GROUP BY PEBGOSTERGE.GOSTERGEKOD, PEBGOSTERGE.GOSTERGEAD, PEBGOSTERGE.OLCUBIRIMKOD, PEBGOSTERGE.TUR, PEBGOSTERGE.KULLANIMSEKLI, PEBGOSTERGE.SEVIYE";
            sql += " ORDER BY PEBGOSTERGE.GOSTERGEAD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen performans hedefi kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve performans hedefin bilgilerini
        /// ve bağlı olduğu stratejik hedefleri ve ağırlıklarını DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="hedef">Performans hedefi bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan performans hedefin bilgilerini ve ağırlıklarını tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader PerformansHedefKartiListele(PerformansHedef hedef)
        {
            string sql = "SELECT " + "PEBPERHEDEF.YIL";
            sql += ", " + "PEBPERHEDEF.KURKOD";
            sql += ", " + "PEBPERHEDEF.HEDEFKOD";
            sql += ", " + "PEBPERHEDEF.HEDEF";
            sql += ", " + "PEBPERHEDEF.ACIKLAMA";
            sql += ", " + "PEBPERHEDEFAGIRLIK.DONEMBYIL";
            sql += ", " + "PEBPERHEDEFAGIRLIK.STRHEDEFKOD";
            sql += ", " + "PEBPERHEDEFAGIRLIK.AGIRLIK";
            sql += ", " + "PEBPERHEDEF.HEDEFGORKOD";
            sql += " FROM " + "PEBPERHEDEF" + ", PEBPERHEDEFAGIRLIK";

            //JOIN
            sql += " WHERE " + "PEBPERHEDEF" + "." + "YIL" + "=" + "PEBPERHEDEFAGIRLIK" + "." + "YIL";
            sql += " AND " + "PEBPERHEDEF" + "." + "KURKOD" + "=" + "PEBPERHEDEFAGIRLIK" + "." + "KURKOD";
            sql += " AND " + "PEBPERHEDEF" + "." + "HEDEFKOD" + "=" + "PEBPERHEDEFAGIRLIK" + "." + "PERHEDEFKOD";

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEF", "YIL", "=", hedef.yil));
            if (!string.IsNullOrEmpty(hedef.kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEF", "KURKOD", "=", hedef.kurKod.Replace(".", "")));
            if (hedef.perHedefKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEF", "HEDEFKOD", "=", hedef.perHedefKod));

            //if (donemBYil > 0 && strHedefKod > 0)
            //{
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEFAGIRLIK", "DONEMBYIL", "=", donemBYil));
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEFAGIRLIK", "STRHEDEFKOD", "=", strHedefKod));
            //}

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve ilgili stratejik hedefe ait
        /// performans hedefleri ve ödeneklerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedefin programatik kodu</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Verilen kriterlere uygun olan performans hedefleri ve ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader StrHedefRaporla(int donemBYil, int strHedefKod, string senaryoKod)
        {
            return PerformansHedefToplam(donemBYil, strHedefKod, 0, senaryoKod, "");
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve ilgili stratejik amaca ait
        /// stratejik hedefleri ve ödeneklerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedefin programatik kodu</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Verilen kriterlere uygun olan stratejik hedefleri ve ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader StrAmacRaporla(int donemBYil, int strHedefKod, string senaryoKod)
        {
            string sql = "SELECT " + "PEBPERHEDEF" + "." + "KURKOD";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(PEBPERHEDEFAGIRLIK.AGIRLIK * PEBFAALIYETPROJEAGIRLIK.AGIRLIK * PEBFPODENEKTOPLAMVIEW.ODENEK / 10000)";
            sql += " FROM " + "PEBFPODENEKTOPLAMVIEW" + ", " + "PEBFAALIYETPROJEAGIRLIK";
            sql += ", " + "PEBPERHEDEF" + ", " + "PEBPERHEDEFAGIRLIK";
            sql += " WHERE " + PHJoin("PEBFPODENEKTOPLAMVIEW");

            sql = VeriErisim.KosulEkle(sql, "AND", PHTKriterleri(donemBYil, strHedefKod, 0, ""));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));

            sql += " GROUP BY " + "PEBPERHEDEF" + "." + "KURKOD";
            sql += " ORDER BY " + "PEBPERHEDEF" + "." + "KURKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kullanarak analitik bütçe
        /// sınıflandırmasının alanlarını virgüllerle ayırarak döndüren yordam
        /// absKodSinif parametresi istenen kod alanlarını, uzunluk parametresi de
        /// kodun soldan ne kadarının istendiğini ifade eder. uzunluk parametresi 0
        /// gönderilirse kodun tamamı isteniyor demektir.
        /// </summary>
        /// <param name="tabloAd">Kod alanlarının istendiği tablonun adı</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki kodlardan biri olmalıdır.</param>
        /// <param name="uzunluk">Kodun soldan istenen uzunluğu</param>
        /// <returns>Virgüllerle ayrılmış ABS kod alanları döndürülür.</returns>
        private string ABSKodSinifAlanlari(string tabloAd, int absKodSinif, int uzunluk)
        {
            string sql = string.Empty;

            if (absKodSinif == (int)ENUMAbsKodSinif.TERTIP)
            {
                sql += ", " + tabloAd + "." + "KURKOD";
                sql += ", " + tabloAd + "." + "FONKOD";
                sql += ", " + tabloAd + "." + "FINKOD";
                if (uzunluk == 0)
                    sql += ", " + tabloAd + "." + "EKOKOD";
                else
                    sql += ", " + VeriErisim.IslecBul("LEFT") + "(" + tabloAd + "." + "EKOKOD, " + uzunluk + ")";
            }
            else if (absKodSinif == (int)ENUMAbsKodSinif.KURKOD)
            {
                if (uzunluk == 0)
                    sql += ", " + tabloAd + "." + "KURKOD";
                else
                    sql += ", " + VeriErisim.IslecBul("LEFT") + "(" + tabloAd + "." + "KURKOD, " + uzunluk + ")";
            }
            else if (absKodSinif == (int)ENUMAbsKodSinif.FONKOD)
            {
                if (uzunluk == 0)
                    sql += ", " + tabloAd + "." + "FONKOD";
                else
                    sql += ", " + VeriErisim.IslecBul("LEFT") + "(" + tabloAd + "." + "FONKOD, " + uzunluk + ")";
            }
            else if (absKodSinif == (int)ENUMAbsKodSinif.FINKOD)
            {
                if (uzunluk == 0)
                    sql += ", " + tabloAd + "." + "FINKOD";
                else
                    sql += ", " + VeriErisim.IslecBul("LEFT") + "(" + tabloAd + "." + "FINKOD, " + uzunluk + ")";
            }
            else if (absKodSinif == (int)ENUMAbsKodSinif.EKOKOD)
            {
                if (uzunluk == 0)
                    sql += ", " + tabloAd + "." + "EKOKOD";
                else
                    sql += ", " + VeriErisim.IslecBul("LEFT") + "(" + tabloAd + "." + "EKOKOD, " + uzunluk + ")";
            }

            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kullanarak faaliyet/projenin
        /// bilgilerini tutan tablo alanlarını virgüllerle ayırarak döndüren yordam
        /// </summary>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki kodlardan biri olmalıdır.</param>
        /// <param name="uzunluk">Kodun soldan istenen uzunluğu</param>
        /// <returns>Virgüllerle ayrılmış faaliyet/proje tablo alanları döndürülür.</returns>
        private string FPOAlanlari(string kurKod, int absKodSinif, int uzunluk)
        {
            string sql = "BIRIM";
            sql += ", FPKOD";
            sql += ", FPAD";
            sql += ", PROJEMI";
            sql += ", TUR";

            if (!string.IsNullOrEmpty(kurKod))
                sql += ABSKodSinifAlanlari("PEBFPODENEKVIEW", absKodSinif, uzunluk);

            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// faaliyet/proje ödenek sql cümlesinin koşul kısmını oluşturan ve döndüren yordam
        /// Dönen stringin başına WHERE mi, yoksa AND mi geleceğine çağıran fonksiyon karar vermelidir.
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Senaryo kriteri</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <param name="personelGideriMi">İstenen ödeneğin personel gideri kaleminden mi istendiği bilgisi</param>
        /// <param name="fpKartMi">Faaliyet/proje kartı için mi çağırıldığı bilgisi</param>
        /// <returns>Sql cümlesinin koşul kısmı döndürülür.</returns>
        private string FPOKriterleri(int yil, string kurKod, string odenekSenaryo, int fpKod, bool personelGideriMi, bool fpKartMi)
        {
            string sql = VeriErisim.KosulOl("", "YIL", "=", yil);
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", odenekSenaryo.Replace(".", "")));

            //Diğer türündeki FPlerin gelmesi istenmiyor
            //FP Kartında gelmesi için fpKartMi eklendi
            if (!fpKartMi)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "TUR", "<>", (int)ENUMFaaliyetProjeTur.DigerKurumIci));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "TUR", "<>", (int)ENUMFaaliyetProjeTur.DigerKurumDisi));
            }

            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BIRIM", "LIKE", kurKod.Replace(".", "") + "%"));

            if (fpKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "FPKOD", "=", fpKod));

            if (personelGideriMi)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "EKOKOD", "LIKE", personelGider + "%"));

            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kullanarak, faaliyet/projenin anahtar
        /// alanlarını ve ABS kod alanlarını virgüllerle ayırarak döndüren yordam
        /// </summary>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki kodlardan biri olmalıdır.</param>
        /// <param name="uzunluk">Kodun soldan istenen uzunluğu</param>
        /// <returns>Virgüllerle ayrılmış faaliyet/projenin anahtar alanları ve ABS kod alanları döndürülür.</returns>
        private string FPOSiralama(string kurKod, int absKodSinif, int uzunluk)
        {
            string sql = string.Empty;
            if (!string.IsNullOrEmpty(kurKod))
            {
                sql += "FPKOD";
                sql += ABSKodSinifAlanlari("PEBFPODENEKVIEW", absKodSinif, uzunluk);
            }
            else
            {
                sql += "BIRIM";
                sql += ", FPKOD";
            }
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve faaliyet/projeleri
        /// ve ödeneklerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="odenekSenaryo">Senaryo kriteri</param>
        /// <param name="personelGideriMi">İstenen ödeneğin personel gideri kaleminden mi istendiği bilgisi</param>
        /// <returns>Verilen kriterlere uygun olan faaliyet/projeleri ve ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader FaaliyetProjeOdenekleri(int yil, string odenekSenaryo, bool personelGideriMi)
        {
            return FaaliyetProjeOdenekleri(yil, "", odenekSenaryo, 0, personelGideriMi, (int)ENUMAbsKodSinif.TERTIP, 0, false);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve faaliyet/projeleri
        /// ve ödeneklerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Senaryo kriteri</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki kodlardan biri olmalıdır.</param>
        /// <param name="uzunluk">Kodun soldan istenen uzunluğu</param>
        /// <returns>Verilen kriterlere uygun olan faaliyet/projeleri ve ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader FaaliyetProjeOdenekleri(int yil, string kurKod, string odenekSenaryo, int absKodSinif, int uzunluk)
        {
            return FaaliyetProjeOdenekleri(yil, kurKod, odenekSenaryo, 0, false, absKodSinif, uzunluk, false);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve faaliyet/projeleri
        /// ve ödeneklerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Senaryo kriteri</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki kodlardan biri olmalıdır.</param>
        /// <param name="uzunluk">Kodun soldan istenen uzunluğu</param>
        /// <returns>Verilen kriterlere uygun olan faaliyet/projeleri ve ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader FaaliyetProjeOdenekleri(int yil, string kurKod, string odenekSenaryo, int fpKod, int absKodSinif, int uzunluk)
        {
            return FaaliyetProjeOdenekleri(yil, kurKod, odenekSenaryo, fpKod, false, absKodSinif, uzunluk, true);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve faaliyet/projeleri
        /// ve ödeneklerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Senaryo kriteri</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <param name="personelGideriMi">İstenen ödeneğin personel gideri kaleminden mi istendiği bilgisi</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki kodlardan biri olmalıdır.</param>
        /// <param name="uzunluk">Kodun soldan istenen uzunluğu</param>
        /// <param name="fpKartMi">Faaliyet/proje kartı için mi çağırıldığı bilgisi</param>
        /// <returns>Verilen kriterlere uygun olan faaliyet/projeleri ve ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        private IDataReader FaaliyetProjeOdenekleri(int yil, string kurKod, string odenekSenaryo, int fpKod, bool personelGideriMi, int absKodSinif, int uzunluk, bool fpKartMi)
        {
            string sql = "SELECT " + FPOAlanlari(kurKod, absKodSinif, uzunluk);
            sql += ", " + VeriErisim.IslecBul("SUM") + "(ODENEK)";
            sql += " FROM " + "PEBFPODENEKVIEW";

            sql = VeriErisim.KosulEkle(sql, "WHERE", FPOKriterleri(yil, kurKod, odenekSenaryo, fpKod, personelGideriMi, fpKartMi));
            sql += " GROUP BY " + FPOAlanlari(kurKod, absKodSinif, uzunluk);
            sql += " ORDER BY " + FPOSiralama(kurKod, absKodSinif, uzunluk);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kullanarak sabit maliyetin
        /// bilgilerini tutan tablo alanlarını virgüllerle ayırarak döndüren yordam
        /// </summary>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki kodlardan biri olmalıdır.</param>
        /// <param name="uzunluk">Kodun soldan istenen uzunluğu</param>
        /// <returns>Virgüllerle ayrılmış sabit maliyet tablo alanları döndürülür.</returns>
        private string SMOAlanlari(int absKodSinif, int uzunluk)
        {
            string sql = "PEBSABITMALIYET.SMKOD";
            sql += ", PEBSABITMALIYET.SMAD";
            sql += ", PEBSABITMALIYET.TUR";

            sql += ABSKodSinifAlanlari("PEBODENEK", absKodSinif, uzunluk);

            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// sabit maliyet ödenek sql cümlesinin koşul kısmını oluşturan ve döndüren yordam
        /// Dönen stringin başına WHERE mi, yoksa AND mi geleceğine çağıran fonksiyon karar vermelidir.
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Senaryo kriteri</param>
        /// <param name="smKod">Sabit maliyetin programatik kodu</param>
        /// <param name="tur">ENUMSabitMaliyetTur listesindeki türlerden biri olmalıdır.</param>
        /// <returns>Sql cümlesinin koşul kısmı döndürülür.</returns>
        private string SMOKriterleri(int yil, string kurKod, string odenekSenaryo, int smKod, int tur)
        {
            string sql = VeriErisim.KosulOl("PEBODENEK", "SABITMI", "=", (int)ENUMOdenekTur.SabitMaliyet);
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBSABITMALIYET", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBSABITMALIYET", "SMKURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBODENEK", "SENARYOKOD", "=", odenekSenaryo.Replace(".", "")));

            if (smKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBSABITMALIYET", "SMKOD", "=", smKod));

            if (tur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBSABITMALIYET", "TUR", "=", tur));

            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kullanarak, sabit maliyetin anahtar
        /// alanlarını ve ABS kod alanlarını virgüllerle ayırarak döndüren yordam
        /// </summary>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki kodlardan biri olmalıdır.</param>
        /// <param name="uzunluk">Kodun soldan istenen uzunluğu</param>
        /// <returns>Virgüllerle ayrılmış sabit maliyetin anahtar alanları ve ABS kod alanları döndürülür.</returns>
        private string SMOSiralama(int absKodSinif, int uzunluk)
        {
            string sql = "PEBSABITMALIYET.SMKOD";
            sql += ABSKodSinifAlanlari("PEBODENEK", absKodSinif, uzunluk);

            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sabit maliyetleri
        /// ve ödeneklerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Senaryo kriteri</param>
        /// <param name="smKod">Sabit maliyetin programatik kodu</param>
        /// <param name="tur">ENUMSabitMaliyetTur listesindeki türlerden biri olmalıdır.</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki kodlardan biri olmalıdır.</param>
        /// <param name="uzunluk">Kodun soldan istenen uzunluğu</param>
        /// <returns>Verilen kriterlere uygun olan sabit maliyetleri ve ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader SabitMaliyetOdenekleri(int yil, string kurKod, string odenekSenaryo, int smKod, int tur, int absKodSinif, int uzunluk)
        {
            string sql = "SELECT " + SMOAlanlari(absKodSinif, uzunluk);
            sql += ", " + VeriErisim.IslecBul("SUM") + "(PEBODENEK.ODENEK)";
            sql += " FROM " + "PEBSABITMALIYET, PEBODENEK";
            sql += " WHERE " + "PEBSABITMALIYET.YIL" + "=" + "PEBODENEK.YIL";
            sql += " AND " + "PEBSABITMALIYET.SMKURKOD" + "=" + "PEBODENEK.MALIYETKURKOD";
            sql += " AND " + "PEBSABITMALIYET.SMKOD" + "=" + "PEBODENEK.MALIYETKOD";

            sql = VeriErisim.KosulEkle(sql, "AND", SMOKriterleri(yil, kurKod, odenekSenaryo, smKod, tur));
            sql += " GROUP BY " + SMOAlanlari(absKodSinif, uzunluk);
            sql += " ORDER BY " + SMOSiralama(absKodSinif, uzunluk);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Faaliyet projenin bilgilerini ve sabit maliyet türünü tutan tablo alanlarını virgüllerle ayırarak döndüren yordam
        /// </summary>
        /// <returns>Virgüllerle ayrılmış faaliyet/proje ve sabit maliyet türü tablo alanları döndürülür.</returns>
        private string FYAlanlari()
        {
            string sql = "FPKURKOD";
            sql += ", FPKOD";
            sql += ", FPAD";
            sql += ", FPTUR";
            sql += ", SMTUR";
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// faaliyet/proje yansıyan ödenek sql cümlesinin koşul kısmını oluşturan ve döndüren yordam
        /// Dönen stringin başına WHERE mi, yoksa AND mi geleceğine çağıran fonksiyon karar vermelidir.
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Senaryo kriteri</param>
        /// <returns>Sql cümlesinin koşul kısmı döndürülür.</returns>
        private string FYKriterleri(int yil, string kurKod, string odenekSenaryo)
        {
            string sql = VeriErisim.KosulOl("", "YIL", "=", yil);
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", odenekSenaryo.Replace(".", "")));

            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "FPKURKOD", "LIKE", kurKod.Replace(".", "") + "%"));

            return sql;
        }

        /// <summary>
        /// Faaliyet/projenin anahtar bilgilerini ve sabit maliyet türünü tutan
        /// tablo alanlarını virgüllerle ayırarak döndüren yordam
        /// </summary>
        /// <returns>Virgüllerle ayrılmış faaliyet/projenin anahtar ve sabit maliyet türünün tablo alanları döndürülür.</returns>
        private string FYSiralama()
        {
            string sql = "FPKURKOD";
            sql += ", FPKOD";
            sql += ", SMTUR";
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve faaliyet/projeleri
        /// ve yansıyan ödeneklerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="odenekSenaryo">Senaryo kriteri</param>
        /// <returns>Verilen kriterlere uygun olan faaliyet/projeleri ve yansıyan ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader FaaliyetYansitilanlar(int yil, string odenekSenaryo)
        {
            return FaaliyetYansitilanlar(yil, "", odenekSenaryo);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve faaliyet/projeleri
        /// ve yansıyan ödeneklerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Senaryo kriteri</param>
        /// <returns>Verilen kriterlere uygun olan faaliyet/projeleri ve yansıyan ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader FaaliyetYansitilanlar(int yil, string kurKod, string odenekSenaryo)
        {
            string sql = "SELECT " + FYAlanlari();
            sql += ", " + VeriErisim.IslecBul("SUM") + "(ODENEK)";
            sql += " FROM " + "PEBFPODENEKYANSIYANVIEW";

            sql = VeriErisim.KosulEkle(sql, "WHERE", FYKriterleri(yil, kurKod, odenekSenaryo));
            sql += " GROUP BY " + FYAlanlari();
            sql += " ORDER BY " + FYSiralama();

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve
        /// stratejik hedefin ödenek toplamını döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedefin programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Verilen stratejik hedefin ödenek toplamı döndürülür.</returns>
        public double PEBOzetRaporla(int donemBYil, int strHedefKod, int yil, string senaryoKod)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(PEBPERHEDEFAGIRLIK.AGIRLIK * PEBFAALIYETPROJEAGIRLIK.AGIRLIK * PEBFPODENEKTOPLAMVIEW.ODENEK / 10000)";
            sql += " FROM " + "PEBFPODENEKTOPLAMVIEW" + ", " + "PEBFAALIYETPROJEAGIRLIK";
            sql += ", " + "PEBPERHEDEF" + ", " + "PEBPERHEDEFAGIRLIK";
            sql += " WHERE " + PHJoin("PEBFPODENEKTOPLAMVIEW");

            sql = VeriErisim.KosulEkle(sql, "AND", PHTKriterleri(donemBYil, strHedefKod, yil, ""));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFPODENEKTOPLAMVIEW", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve
        /// stratejik hedefleri ve ödeneklerini döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="yansitilan">Yansıtılan ödenekler mi isteniyor bilgisi</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki kodlardan biri olmalıdır.</param>
        /// <param name="uzunluk">Kodun soldan istenen uzunluğu</param>
        /// <returns>Verilen kriterlere uygun olan stratejik hedefleri ve ödeneklerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader StrHedefOdenekDagilimRaporla(int yil, string senaryoKod, bool yansitilan, int absKodSinif, int uzunluk)
        {
            string tablo = string.Empty;
            if (yansitilan)
                tablo = "PEBFPODENEKYANSIYANVIEW";
            else
                tablo = "PEBFPODENEKVIEW";

            string alanlar = "PEBPERHEDEFAGIRLIK" + "." + "DONEMBYIL";
            alanlar += ", " + "PEBPERHEDEFAGIRLIK" + "." + "STRHEDEFKOD";
            if (!yansitilan)
                alanlar += ABSKodSinifAlanlari(tablo, absKodSinif, uzunluk);

            string sql = "SELECT " + alanlar;
            sql += ", " + VeriErisim.IslecBul("SUM") + "(PEBPERHEDEFAGIRLIK.AGIRLIK * PEBFAALIYETPROJEAGIRLIK.AGIRLIK * " + tablo + ".ODENEK / 10000)";
            sql += " FROM " + tablo + ", " + "PEBFAALIYETPROJEAGIRLIK";
            sql += ", " + "PEBPERHEDEF" + ", " + "PEBPERHEDEFAGIRLIK";
            sql += " WHERE " + PHJoin(tablo);

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEFAGIRLIK", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tablo, "SENARYOKOD", "=", senaryoKod.Replace(".", "")));

            sql += " GROUP BY " + alanlar;
            sql += " ORDER BY " + alanlar;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve performans göstergelerini, hedeflenen ve gerçekleşen değerlerini, bağlı olduğu
        /// performans hedeflerini ve stratejik hedefleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Verilen kriterlere uygun olan performans göstergelerini, hedeflenen ve gerçekleşen değerleri,
        /// bağlı olduğu performans hedeflerini ve stratejik hedefleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader BirimGostergeleriRaporla(int yil, string kurKod, int[] gostergeSeviye)
        {
            string sql = "SELECT PEBPERHEDEFAGIRLIK.DONEMBYIL";
            sql += ", " + "PEBPERHEDEFAGIRLIK.STRHEDEFKOD";
            sql += ", " + "PEBPERHEDEF.HEDEFKOD";
            sql += ", " + "PEBPERHEDEF.HEDEF";
            sql += ", " + "PEBPERHEDEF.ACIKLAMA";
            sql += ", " + "PEBGOSTERGE.GOSTERGEKOD";
            sql += ", " + "PEBGOSTERGE.GOSTERGEAD";
            sql += ", " + "PEBOLCUBIRIM.OLCUBIRIMKOD";
            sql += ", " + "PEBOLCUBIRIM.OLCUBIRIMAD";
            sql += ", " + "PEBGOSTERGEHEDEF.MIKTAR";
            sql += ", " + "PEBGOSTERGEHEDEF.SIRANO";
            sql += ", " + "PEBGOSTERGEGERCEK.AY";
            sql += ", " + "PEBGOSTERGEGERCEK.MIKTAR";

            sql += " FROM " + "PEBPERHEDEFAGIRLIK";
            sql += " INNER JOIN " + "PEBPERHEDEF";
            sql += " ON " + "PEBPERHEDEFAGIRLIK.YIL" + "=" + "PEBPERHEDEF.YIL";
            sql += " AND " + "PEBPERHEDEFAGIRLIK.KURKOD" + "=" + "PEBPERHEDEF.KURKOD";
            sql += " AND " + "PEBPERHEDEFAGIRLIK.PERHEDEFKOD" + "=" + "PEBPERHEDEF.HEDEFKOD";

            sql += " INNER JOIN " + "PEBGOSTERGEHEDEF";
            sql += " ON " + "PEBPERHEDEF.YIL" + "=" + "PEBGOSTERGEHEDEF.YIL";
            sql += " AND " + "PEBPERHEDEF.KURKOD" + "=" + "PEBGOSTERGEHEDEF.PHKURKOD";
            sql += " AND " + "PEBPERHEDEF.HEDEFKOD" + "=" + "PEBGOSTERGEHEDEF.PERHEDEFKOD";

            sql += " INNER JOIN " + "PEBGOSTERGE";
            sql += " ON " + "PEBGOSTERGEHEDEF.GOSTERGEKOD" + "=" + "PEBGOSTERGE.GOSTERGEKOD";

            sql += " INNER JOIN " + "PEBOLCUBIRIM";
            sql += " ON " + "PEBGOSTERGE.OLCUBIRIMKOD" + "=" + "PEBOLCUBIRIM.OLCUBIRIMKOD";

            sql += " LEFT OUTER JOIN " + "PEBGOSTERGEGERCEK";
            sql += " ON " + "PEBGOSTERGEHEDEF.YIL" + "=" + "PEBGOSTERGEGERCEK.YIL";
            sql += " AND " + "PEBGOSTERGEHEDEF.PHKURKOD" + "=" + "PEBGOSTERGEGERCEK.PHKURKOD";
            sql += " AND " + "PEBGOSTERGEHEDEF.PERHEDEFKOD" + "=" + "PEBGOSTERGEGERCEK.PERHEDEFKOD";
            sql += " AND " + "PEBGOSTERGEHEDEF.GOSTERGEKOD" + "=" + "PEBGOSTERGEGERCEK.GOSTERGEKOD";
            sql += " AND " + "PEBGOSTERGEHEDEF.GHKURKOD" + "=" + "PEBGOSTERGEGERCEK.GGKURKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("PEBPERHEDEF", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "ASAMA", "=", 6));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "GHKURKOD", "=", kurKod.Replace(".", "")));

            bool parantezKoy = false;
            for (int i = 0; i < gostergeSeviye.Length; i++)
            {
                if (gostergeSeviye[i] > 0)
                {
                    parantezKoy = true;

                    if (i == 0)
                        sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("PEBGOSTERGE", "SEVIYE", "=", gostergeSeviye[i]));
                    else
                        sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("PEBGOSTERGE", "SEVIYE", "=", gostergeSeviye[i]));
                }

                if (parantezKoy && i + 1 == gostergeSeviye.Length)
                    sql += ")";
            }

            sql += " ORDER BY PEBPERHEDEFAGIRLIK.DONEMBYIL";
            sql += ", " + "PEBPERHEDEFAGIRLIK.STRHEDEFKOD";
            sql += ", " + "PEBPERHEDEF.HEDEFKOD";
            sql += ", " + "PEBGOSTERGE.GOSTERGEKOD";
            sql += ", " + "PEBGOSTERGEHEDEF.SIRANO";
            sql += ", " + "PEBGOSTERGEGERCEK.AY";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve performans göstergelerini, hedeflenen ve gerçekleşen değerlerini, bağlı olduğu
        /// performans hedeflerini ve stratejik hedefleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="pHedef">Performans hedef bilgilerini tutan nesne</param>
        /// <param name="pGosterge">Performans gösterge bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan performans göstergelerini, hedeflenen ve gerçekleşen değerleri,
        /// bağlı olduğu performans hedeflerini ve stratejik hedefleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader PerformansGostergeDetayRaporla(PerformansHedef pHedef, PerformansGosterge pGosterge)
        {
            string sql = "SELECT PEBPERHEDEFAGIRLIK.DONEMBYIL";
            sql += ", " + "PEBPERHEDEFAGIRLIK.STRHEDEFKOD";
            sql += ", " + "PEBPERHEDEFAGIRLIK.AGIRLIK";
            sql += ", " + "PEBPERHEDEF.HEDEFKOD";
            sql += ", " + "PEBPERHEDEF.HEDEF";
            sql += ", " + "PEBPERHEDEF.ACIKLAMA";
            sql += ", " + "PEBGOSTERGE.GOSTERGEKOD";
            sql += ", " + "PEBGOSTERGE.GOSTERGEAD";
            sql += ", " + "PEBOLCUBIRIM.OLCUBIRIMKOD";
            sql += ", " + "PEBOLCUBIRIM.OLCUBIRIMAD";
            sql += ", " + "PEBGOSTERGEHEDEF.MIKTAR";
            sql += ", " + "PEBGOSTERGEHEDEF.SIRANO";
            sql += ", " + "PEBGOSTERGEHEDEF.ACIKLAMA";
            sql += ", " + "PEBGOSTERGEGERCEK.AY";
            sql += ", " + "PEBGOSTERGEGERCEK.MIKTAR";
            sql += ", " + "PEBGOSTERGEGERCEK.KAYNAK";
            sql += ", " + "PEBGOSTERGEGERCEK.SONUC";
            sql += ", " + "PEBGOSTERGEGERCEK.SAPMA";
            sql += ", " + "PEBGOSTERGEGERCEK.ONLEM";
            sql += ", " + "PEBGOSTERGE.ILERLEMEYONU";

            sql += " FROM " + "PEBPERHEDEFAGIRLIK";
            sql += " INNER JOIN " + "PEBPERHEDEF";
            sql += " ON " + "PEBPERHEDEFAGIRLIK.YIL" + "=" + "PEBPERHEDEF.YIL";
            sql += " AND " + "PEBPERHEDEFAGIRLIK.KURKOD" + "=" + "PEBPERHEDEF.KURKOD";
            sql += " AND " + "PEBPERHEDEFAGIRLIK.PERHEDEFKOD" + "=" + "PEBPERHEDEF.HEDEFKOD";

            sql += " INNER JOIN " + "PEBGOSTERGEHEDEF";
            sql += " ON " + "PEBPERHEDEF.YIL" + "=" + "PEBGOSTERGEHEDEF.YIL";
            sql += " AND " + "PEBPERHEDEF.KURKOD" + "=" + "PEBGOSTERGEHEDEF.PHKURKOD";
            sql += " AND " + "PEBPERHEDEF.HEDEFKOD" + "=" + "PEBGOSTERGEHEDEF.PERHEDEFKOD";

            sql += " INNER JOIN " + "PEBGOSTERGE";
            sql += " ON " + "PEBGOSTERGEHEDEF.GOSTERGEKOD" + "=" + "PEBGOSTERGE.GOSTERGEKOD";

            sql += " INNER JOIN " + "PEBOLCUBIRIM";
            sql += " ON " + "PEBGOSTERGE.OLCUBIRIMKOD" + "=" + "PEBOLCUBIRIM.OLCUBIRIMKOD";

            sql += " LEFT OUTER JOIN " + "PEBGOSTERGEGERCEK";
            sql += " ON " + "PEBGOSTERGEHEDEF.YIL" + "=" + "PEBGOSTERGEGERCEK.YIL";
            sql += " AND " + "PEBGOSTERGEHEDEF.PHKURKOD" + "=" + "PEBGOSTERGEGERCEK.PHKURKOD";
            sql += " AND " + "PEBGOSTERGEHEDEF.PERHEDEFKOD" + "=" + "PEBGOSTERGEGERCEK.PERHEDEFKOD";
            sql += " AND " + "PEBGOSTERGEHEDEF.GOSTERGEKOD" + "=" + "PEBGOSTERGEGERCEK.GOSTERGEKOD";
            sql += " AND " + "PEBGOSTERGEHEDEF.GHKURKOD" + "=" + "PEBGOSTERGEGERCEK.GGKURKOD";

            string[] parcaliKurKod = pHedef.kurKod.Split(';');
            if (parcaliKurKod.Length < 2)
                return null;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "YIL", "=", pHedef.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "PHKURKOD", "=", parcaliKurKod[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "ASAMA", "=", 6));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "PERHEDEFKOD", "=", pHedef.perHedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "GOSTERGEKOD", "=", pGosterge.gostergeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "GHKURKOD", "=", parcaliKurKod[1].Replace(".", "")));

            sql += " ORDER BY PEBPERHEDEFAGIRLIK.DONEMBYIL";
            sql += ", " + "PEBPERHEDEFAGIRLIK.STRHEDEFKOD";
            sql += ", " + "PEBGOSTERGEGERCEK.AY";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve performans göstergelerini, hedeflenen ve gerçekleşen değerlerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="perHedef">Performans hedef bilgilerini tutan nesne</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Verilen kriterlere uygun olan performans göstergelerini, hedeflenen
        /// ve gerçekleşen değerleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader PerformansGostergeRaporla(PerformansHedef perHedef, int[] gostergeSeviye)
        {
            string sql = "SELECT PEBGOSTERGE.GOSTERGEKOD";
            sql += ", " + "PEBGOSTERGE.GOSTERGEAD";
            sql += ", " + "PEBOLCUBIRIM.OLCUBIRIMKOD";
            sql += ", " + "PEBOLCUBIRIM.OLCUBIRIMAD";
            sql += ", " + "PEBGOSTERGEHEDEF.PHKURKOD";
            sql += ", " + "PEBGOSTERGEHEDEF.PERHEDEFKOD";
            sql += ", " + "PEBGOSTERGEHEDEF.MIKTAR";
            sql += ", " + "PEBGOSTERGEHEDEF.SIRANO";
            sql += ", " + "PEBGOSTERGEHEDEF.ACIKLAMA";
            sql += ", " + "PEBGOSTERGEGERCEK.AY";
            sql += ", " + "PEBGOSTERGEGERCEK.MIKTAR";
            sql += ", " + "PEBGOSTERGEGERCEK.KAYNAK";
            sql += ", " + "PEBGOSTERGEGERCEK.SONUC";
            sql += ", " + "PEBGOSTERGEGERCEK.SAPMA";
            sql += ", " + "PEBGOSTERGEGERCEK.ONLEM";
            sql += ", " + "PEBGOSTERGE.ILERLEMEYONU";
            sql += ", " + "PEBGOSTERGEHEDEF.GHKURKOD";

            sql += " FROM " + "PEBGOSTERGEHEDEF";
            sql += " INNER JOIN " + "PEBGOSTERGE";
            sql += " ON " + "PEBGOSTERGEHEDEF.GOSTERGEKOD" + "=" + "PEBGOSTERGE.GOSTERGEKOD";

            sql += " INNER JOIN " + "PEBOLCUBIRIM";
            sql += " ON " + "PEBGOSTERGE.OLCUBIRIMKOD" + "=" + "PEBOLCUBIRIM.OLCUBIRIMKOD";

            sql += " LEFT OUTER JOIN " + "PEBGOSTERGEGERCEK";
            sql += " ON " + "PEBGOSTERGEHEDEF.YIL" + "=" + "PEBGOSTERGEGERCEK.YIL";
            sql += " AND " + "PEBGOSTERGEHEDEF.PHKURKOD" + "=" + "PEBGOSTERGEGERCEK.PHKURKOD";
            sql += " AND " + "PEBGOSTERGEHEDEF.PERHEDEFKOD" + "=" + "PEBGOSTERGEGERCEK.PERHEDEFKOD";
            sql += " AND " + "PEBGOSTERGEHEDEF.GOSTERGEKOD" + "=" + "PEBGOSTERGEGERCEK.GOSTERGEKOD";
            sql += " AND " + "PEBGOSTERGEHEDEF.GHKURKOD" + "=" + "PEBGOSTERGEGERCEK.GGKURKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "YIL", "=", perHedef.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "ASAMA", "=", 6));
            if (!string.IsNullOrEmpty(perHedef.kurKod))
            {
                string[] parcaliKurKod = perHedef.kurKod.Split(';');
                if (!string.IsNullOrEmpty(parcaliKurKod[0]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "GHKURKOD", "=", parcaliKurKod[0].Replace(".", "")));
                if (parcaliKurKod.Length == 2 && !string.IsNullOrEmpty(parcaliKurKod[1]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "PHKURKOD", "=", parcaliKurKod[1].Replace(".", "")));
            }
            if (perHedef.perHedefKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "PERHEDEFKOD", "=", perHedef.perHedefKod));

            bool parantezKoy = false;
            for (int i = 0; i < gostergeSeviye.Length; i++)
            {
                if (gostergeSeviye[i] > 0)
                {
                    parantezKoy = true;

                    if (i == 0)
                        sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("PEBGOSTERGE", "SEVIYE", "=", gostergeSeviye[i]));
                    else
                        sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("PEBGOSTERGE", "SEVIYE", "=", gostergeSeviye[i]));
                }

                if (parantezKoy && i + 1 == gostergeSeviye.Length)
                    sql += ")";
            }

            sql += " ORDER BY PEBGOSTERGE.GOSTERGEKOD";
            sql += ", " + "PEBGOSTERGEHEDEF.PHKURKOD";
            sql += ", " + "PEBGOSTERGEHEDEF.GHKURKOD";
            sql += ", " + "PEBGOSTERGEHEDEF.PERHEDEFKOD";
            sql += ", " + "PEBGOSTERGEGERCEK.AY";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        /// stratejik hedefleri ve bağlı olan I. ve II. seviye performans göstergelerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="asama">Aşama kriteri</param>
        /// <returns>Verilen kriterlere uygun olan stratejik hedefleri ve bağlı olan I. ve II.
        /// seviye performans göstergelerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader StratejikGostergeRaporla(int donemBYil, int yil, int asama)
        {
            string sql = "SELECT PEBPERHEDEFAGIRLIK.DONEMBYIL";
            sql += ", " + "PEBPERHEDEFAGIRLIK.STRHEDEFKOD";
            sql += ", " + "PEBGOSTERGE.GOSTERGEKOD";
            sql += ", " + "PEBGOSTERGE.GOSTERGEAD";
            sql += ", " + "PEBOLCUBIRIM.OLCUBIRIMKOD";
            sql += ", " + "PEBOLCUBIRIM.OLCUBIRIMAD";

            sql += " FROM " + "PEBPERHEDEFAGIRLIK";
            sql += " INNER JOIN " + "PEBPERHEDEF";
            sql += " ON " + "PEBPERHEDEFAGIRLIK.YIL" + "=" + "PEBPERHEDEF.YIL";
            sql += " AND " + "PEBPERHEDEFAGIRLIK.KURKOD" + "=" + "PEBPERHEDEF.KURKOD";
            sql += " AND " + "PEBPERHEDEFAGIRLIK.PERHEDEFKOD" + "=" + "PEBPERHEDEF.HEDEFKOD";

            sql += " INNER JOIN " + "PEBGOSTERGEHEDEF";
            sql += " ON " + "PEBPERHEDEF.YIL" + "=" + "PEBGOSTERGEHEDEF.YIL";
            sql += " AND " + "PEBPERHEDEF.KURKOD" + "=" + "PEBGOSTERGEHEDEF.PHKURKOD";
            sql += " AND " + "PEBPERHEDEF.HEDEFKOD" + "=" + "PEBGOSTERGEHEDEF.PERHEDEFKOD";

            sql += " INNER JOIN " + "PEBGOSTERGE";
            sql += " ON " + "PEBGOSTERGEHEDEF.GOSTERGEKOD" + "=" + "PEBGOSTERGE.GOSTERGEKOD";

            sql += " INNER JOIN " + "PEBOLCUBIRIM";
            sql += " ON " + "PEBGOSTERGE.OLCUBIRIMKOD" + "=" + "PEBOLCUBIRIM.OLCUBIRIMKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("PEBPERHEDEFAGIRLIK", "DONEMBYIL", "=", donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEF", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBGOSTERGEHEDEF", "ASAMA", "=", asama));
            sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("PEBGOSTERGE", "SEVIYE", "=", (int)ENUMGostergeSeviye.SeviyeI));
            sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("PEBGOSTERGE", "SEVIYE", "=", (int)ENUMGostergeSeviye.SeviyeII));
            sql += ")";

            sql += " ORDER BY PEBPERHEDEFAGIRLIK.DONEMBYIL";
            sql += ", " + "PEBPERHEDEFAGIRLIK.STRHEDEFKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader GostergeGerceklesenHedefGosterge(int yil, string kurKod, int ay)
        {
            string sql = "SELECT PEBGOSTERGEHEDEF.PHKURKOD, PEBGOSTERGEHEDEF.PERHEDEFKOD, PEBPERHEDEF.HEDEF, PEBPERHEDEF.ACIKLAMA,";
            sql += "PEBGOSTERGEHEDEF.GOSTERGEKOD, PEBGOSTERGEHEDEF.GHKURKOD, PEBGOSTERGEHEDEF.MIKTAR, PEBGOSTERGEHEDEF.SIRANO,";
            sql += "PEBGOSTERGEHEDEF.ACIKLAMA, PEBGOSTERGE.GOSTERGEAD, PEBOLCUBIRIM.OLCUBIRIMAD, PEBGOSTERGE.ILERLEMEYONU,";
            sql += "PEBGOSTERGEGERCEK.MIKTAR, PEBGOSTERGEGERCEK.KAYNAK, PEBGOSTERGEGERCEK.SONUC, PEBGOSTERGEGERCEK.SAPMA,";
            sql += "PEBGOSTERGEGERCEK.ONLEM";
            sql += " FROM PEBGOSTERGEHEDEF INNER JOIN";
            sql += " PEBPERHEDEF ON PEBGOSTERGEHEDEF.YIL = PEBPERHEDEF.YIL AND PEBGOSTERGEHEDEF.PHKURKOD = PEBPERHEDEF.KURKOD AND";
            sql += " PEBGOSTERGEHEDEF.PERHEDEFKOD = PEBPERHEDEF.HEDEFKOD INNER JOIN";
            sql += " PEBGOSTERGE ON PEBGOSTERGEHEDEF.GOSTERGEKOD = PEBGOSTERGE.GOSTERGEKOD INNER JOIN";
            sql += " PEBOLCUBIRIM ON PEBGOSTERGE.OLCUBIRIMKOD = PEBOLCUBIRIM.OLCUBIRIMKOD LEFT OUTER JOIN";
            sql += " PEBGOSTERGEGERCEK ON PEBGOSTERGEHEDEF.YIL = PEBGOSTERGEGERCEK.YIL AND";
            sql += " PEBGOSTERGEHEDEF.PHKURKOD = PEBGOSTERGEGERCEK.PHKURKOD AND";
            sql += " PEBGOSTERGEHEDEF.PERHEDEFKOD = PEBGOSTERGEGERCEK.PERHEDEFKOD AND";
            sql += " PEBGOSTERGEHEDEF.GOSTERGEKOD = PEBGOSTERGEGERCEK.GOSTERGEKOD AND";
            sql += " PEBGOSTERGEHEDEF.GHKURKOD = PEBGOSTERGEGERCEK.GGKURKOD AND";
            sql += " PEBGOSTERGEGERCEK.AY = " + VeriErisim.DegerYap(ay);
            sql += " WHERE (PEBGOSTERGEHEDEF.YIL = " + VeriErisim.DegerYap(yil) + ") AND (PEBGOSTERGEHEDEF.ASAMA = 6)";
            if (!string.IsNullOrEmpty(kurKod))
                if (kurKod.Length > 5)
                    sql += " AND (PEBGOSTERGEHEDEF.GHKURKOD = " + VeriErisim.DegerYap(kurKod.Replace(".", "")) + ")";
                else
                    sql += " AND (PEBGOSTERGEHEDEF.GHKURKOD LIKE " + VeriErisim.DegerYap(kurKod.Replace(".", "") + "%") + ")";
            sql += " order by PEBGOSTERGEHEDEF.PHKURKOD, PEBGOSTERGEHEDEF.PERHEDEFKOD, PEBGOSTERGEHEDEF.GOSTERGEKOD, PEBGOSTERGEHEDEF.GHKURKOD";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
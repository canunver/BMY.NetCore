using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Yatırım modülü raporları ile ilgili select sql sorgularını tutan sınıf
    /// </summary>
    public class TABYATRaporlar : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATRaporlar(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Verilen parametreleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve yatırım projelerinin ve alt projelerinin kimlik bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="sektorNo">Sektör kriteri</param>
        /// <param name="etudMu">Değeri; "E" ise etüd projeleri, "H" ise etüd olmayan projeleri, boş ise tamamını ifade eder.</param>
        /// <param name="projeNo">Yatırım projesinin programatik kodu</param>
        /// <param name="birimKodlari">Kullanıcının yetkili olduğu birim listesi</param>
        /// <returns>Yatırım projelerinin ve alt projelerinin kimlik bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Tablo2Listele(int yil, string senaryoKod, string sektorNo, string etudMu, int projeNo, int projeTur, ENUMButceKaynakTur[] bkTur, string[] birimKodlari, int altProjeNo, string araString)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABYATProje.TABLOAD, TABYATProje.ALANLAR);
            sql += ", " + VeriErisim.TumAlanlar(TABYATAltProje.TABLOAD, TABYATAltProje.ALANLAR) + "," + TABYATSektor.TABLOAD + ".AD, " + TABYATProjeTur.TABLOAD + ".TURADI";
            sql += " FROM " + TABYATProje.TABLOAD;
            sql += " INNER JOIN " + TABYATAltProje.TABLOAD + " ON " + VeriErisim.Join(TABYATProje.TABLOAD, TABYATAltProje.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO] });
            sql += " LEFT OUTER JOIN " + TABYATSektor.TABLOAD + " ON " + VeriErisim.Join(TABYATSektor.TABLOAD, TABYATProje.TABLOAD, new string[] { "YIL", "SEKTORNO" });
            sql += " LEFT OUTER JOIN " + TABYATProjeTur.TABLOAD + " ON " + VeriErisim.Join(TABYATProjeTur.TABLOAD, TABYATProje.TABLOAD, new string[] { "TURKODU" });
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.AKTIF], "=", 1));
            if (!string.IsNullOrEmpty(sektorNo))                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.SEKTORNO], "LIKE", sektorNo.Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(etudMu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.ETUDPROJEMI], "=", etudMu));
            if (projeNo > 0)                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO], "=", projeNo));
            if (altProjeNo > 0)                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATAltProje.TABLOAD, TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.ALTPROJENO], "=", altProjeNo));
            if (projeTur > 0)                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.TURKODU], "=", projeTur));

            if (!string.IsNullOrWhiteSpace(araString))
            {
                string icKosul = VeriErisim.KosulEkle("", "OR", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.AD], "LIKE", "%" + araString + "%"));
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.DPTPROJENO], "LIKE", "%" + araString + "%"));
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABYATAltProje.TABLOAD, TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.AD], "LIKE", "%" + araString + "%"));
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + icKosul + ")");
            }

            if (bkTur != null && bkTur.Length > 0)
            {
                string kume = "";
                for (int i = 0; i < bkTur.Length; i++)
                {
                    if (kume != "") kume += ",";
                    kume += ((int)bkTur[i]);
                }
                kume = "(" + kume + ")";
                sql = VeriErisim.KosulEkle(sql, "AND", "(1 IN (select 1 from YATBUTCEKAYNAK K where K.ALTPROJENO=YATALTPROJE.ALTPROJENO AND K.KAYNAKTUR IN " + kume + "))");
            }

            string orluBirimler = TABGenel.DegerleriOrla(birimKodlari, TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM], 0);
            if (!string.IsNullOrEmpty(orluBirimler))
                sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

            sql += " ORDER BY " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO] + ", " + TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.ALTPROJENO];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yatırım projesi programatik kodu ve degerler dizisindeki kullanıcının yetkili olduğu birimleri
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        /// ilgili yatırım projesinin teklif bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        /// <returns>Yatırım projesinin teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Tablo5TeklifBilgileriListele(int projeKodu, string[] degerler, string senaryoKod)
        {
            return TeklifBilgisiListele(projeKodu, 0, degerler, senaryoKod);
        }

        ///// <summary>
        ///// Yatırım projesi programatik kodu, senaryo kodu ve degerler dizisindeki kullanıcının yetkili olduğu
        ///// birimleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        ///// ilgili yatırım projesinin ödenek teklif bilgilerini DataReader yapısında döndüren yordam
        ///// </summary>
        ///// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        ///// <param name="senaryoKod">Senaryo kodu</param>
        ///// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        ///// <returns>Yatırım projesinin ödenek teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        //public IDataReader Tablo5OdenekBilgileriListele(int projeKodu, string senaryoKod, string[] degerler)
        //{
        //    return OdenekBilgisiListele(projeKodu, 0, senaryoKod, degerler, 0);
        //}

        /// <summary>
        /// Verilen yatırım alt projesi programatik kodunu kıstas olarak kullanarak, select sql cümlesini oluşturup,
        /// çalıştıran ve yatırım projesinin programatik kodunu döndüren yordam
        /// </summary>
        /// <param name="altProjeKodu">Yatırım alt projesi programatik kodu</param>
        /// <returns>Yatırım projesinin programatik kodu döndürülür.</returns>
        public int ProjeKoduAlFromAltProje(int yil, string senaryoKod, int altProjeKodu)
        {
            string sql = "SELECT " + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.PROJENO];
            sql += " FROM " + TABYATAltProje.TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.ALTPROJENO], "=", altProjeKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Yatırım projesi programatik kodu ve degerler dizisindeki kullanıcının yetkili olduğu birimleri
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        /// ilgili yatırım projesinin kimlik ve teklif bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        /// <returns>Yatırım projesinin kimlik ve teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Tablo6ProjeVeTeklifBilgisiListele(int yil, string senaryoKod, int projeKodu, string[] degerler)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABYATProje.TABLOAD, TABYATProje.ALANLAR);
            sql += ", " + VeriErisim.IslecBul("SUM");
            sql += "(" + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.PROJEBUTCE] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM");
            sql += "(" + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.PROJEBUTCEDISAGIDEN] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM");
            sql += "(" + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YILBUTCEDIS] + ")";
            sql += " FROM " + TABYATProje.TABLOAD + ", " + TABYATAltProje.TABLOAD + ", " + TABYATButceKaynak.TABLOAD + ", " + TABYATButceKaynakTeklif.TABLOAD;
            sql += " WHERE " + VeriErisim.Join(TABYATProje.TABLOAD, TABYATAltProje.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO] });
            sql += " AND " + VeriErisim.Join(TABYATAltProje.TABLOAD, TABYATButceKaynak.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.ALTPROJENO] });
            sql += " AND " + VeriErisim.Join(TABYATButceKaynak.TABLOAD, TABYATButceKaynakTeklif.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.KAYNAKNO] });

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            if (projeKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO], "=", projeKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.TEKLIFYIL], "=", 0));

            string orluBirimler = TABGenel.DegerleriOrla(degerler, TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM], 0);
            if (!string.IsNullOrEmpty(orluBirimler))
                sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

            sql += " GROUP BY " + VeriErisim.TumAlanlar(TABYATProje.TABLOAD, TABYATProje.ALANLAR);
            sql += " ORDER BY " + VeriErisim.TumAlanlar(TABYATProje.TABLOAD, TABYATProje.ALANLAR);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yatırım projesi programatik kodu, yıl ve degerler dizisindeki kullanıcının yetkili olduğu birimleri
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        /// ilgili yatırım projesinin teklif bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        /// <returns>Yatırım projesinin teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Tablo7TeklifBilgisiListele(int projeKodu, int yil, string[] degerler, string senaryoKod)
        {
            return TeklifBilgisiListele(projeKodu, yil, degerler, senaryoKod);
        }

        ///// <summary>
        ///// Yatırım projesi programatik kodu, yıl, senaryo kodu ve degerler dizisindeki kullanıcının yetkili olduğu
        ///// birimleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        ///// ilgili yatırım projesinin ödenek teklif bilgilerini DataReader yapısında döndüren yordam
        ///// </summary>
        ///// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Senaryo kodu</param>
        ///// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        ///// <returns>Yatırım projesinin ödenek teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        //public IDataReader Tablo7OdenekBilgisiListele(int projeKodu, int yil, string senaryoKod, string[] degerler)
        //{
        //    return OdenekBilgisiListele(projeKodu, yil, senaryoKod, degerler, -1);
        //}

        /// <summary>
        /// Yatırım projesi programatik kodu, yıl, teklif yıl (Çok yıllı bütçe) ve degerler dizisindeki kullanıcının
        /// yetkili olduğu birimleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        /// ilgili yatırım projesinin teklif bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        /// <param name="teklifYil">Teklif yılı (Çok yıllı bütçe)</param>
        /// <returns>Yatırım projesinin teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        private IDataReader TeklifBilgisiListele(int projeKodu, int yil, string[] degerler, string senaryoKod)
        {
            string sql = "SELECT " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL];
            sql += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.TEKLIFYIL];
            sql += ", " + VeriErisim.IslecBul("SUM");
            sql += "(" + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YILBUTCEDIS] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM");
            sql += "(" + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.PROJEBUTCE] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM");
            sql += "(" + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.PROJEBUTCEDISAGIDEN] + ")";
            sql += " FROM " + TABYATProje.TABLOAD + ", " + TABYATAltProje.TABLOAD + ", " + TABYATButceKaynak.TABLOAD + ", " + TABYATButceKaynakTeklif.TABLOAD;

            sql += " WHERE " + VeriErisim.Join(TABYATProje.TABLOAD, TABYATAltProje.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO] });
            sql += " AND " + VeriErisim.Join(TABYATAltProje.TABLOAD, TABYATButceKaynak.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.ALTPROJENO] });
            sql += " AND " + VeriErisim.Join(TABYATButceKaynak.TABLOAD, TABYATButceKaynakTeklif.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.KAYNAKNO] });


            if (projeKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO], "=", projeKodu));
            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            string orluBirimler = TABGenel.DegerleriOrla(degerler, TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM], 0);
            if (!string.IsNullOrEmpty(orluBirimler))
                sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

            sql += " GROUP BY " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL];
            sql += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.TEKLIFYIL];
            sql += " ORDER BY " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL];
            sql += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.TEKLIFYIL];

            return veriErisim.DataReaderDondur(sql);
        }

        ///// <summary>
        ///// Yatırım projesi programatik kodu, yıl, senaryo kodu, teklif yıl (Çok yıllı bütçe) ve degerler dizisindeki
        ///// kullanıcının yetkili olduğu birimleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        ///// ilgili yatırım projesinin ödenek teklif bilgilerini DataReader yapısında döndüren yordam
        ///// </summary>
        ///// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Senaryo kodu</param>
        ///// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        ///// <param name="teklifYil">Teklif yılı (Çok yıllı bütçe)</param>
        ///// <returns>Yatırım projesinin ödenek teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        //private IDataReader OdenekBilgisiListele(int projeKodu, int yil, string senaryoKod, string[] degerler, int teklifYil)
        //{
        //    string sql = "";
        //    //string sql = "SELECT " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.YIL];
        //    //sql += ", " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.TEKLIFYIL];
        //    //sql += ", " + VeriErisim.IslecBul("SUM");
        //    //sql += "(" + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.ODENEK] + ")";
        //    //sql += " FROM " + TABYATProje.TABLOAD + ", " + TABYATAltProje.TABLOAD + ", " + TABYATButceKaynak.TABLOAD + ", " + TABYATButceKaynakOdenek.TABLOAD;
        //    //sql += " WHERE " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO];
        //    //sql += " = " + TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.PROJENO];
        //    //sql += " AND " + TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.ALTPROJENO];
        //    //sql += " = " + TABYATButceKaynak.TABLOAD + "." + TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.ALTPROJENO];
        //    //sql += " AND " + TABYATButceKaynak.TABLOAD + "." + TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.KAYNAKNO];
        //    //sql += " = " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.KAYNAKNO];

        //    //if (projeKodu > 0)
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO], "=", projeKodu));
        //    //if (yil > 0)
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakOdenek.TABLOAD, TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.YIL], "=", yil));
        //    //if (!string.IsNullOrEmpty(senaryoKod))
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakOdenek.TABLOAD, TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
        //    //if (teklifYil >= 0)
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakOdenek.TABLOAD, TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.TEKLIFYIL], "=", teklifYil));

        //    //string orluBirimler = TABGenel.DegerleriOrla(degerler, TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM]);
        //    //if (!string.IsNullOrEmpty(orluBirimler))
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

        //    //sql += " GROUP BY " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.YIL];
        //    //sql += ", " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.TEKLIFYIL];
        //    //sql += " ORDER BY " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.YIL];
        //    //sql += ", " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.TEKLIFYIL];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        /// <summary>
        /// Verilen parametreleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve yatırım projesi bazında proje bütçe teklif bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kodu</param>
        /// <param name="sektorKod">Sektör kriteri</param>
        /// <param name="model">ENUMModel modellerinden biri</param>
        /// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        /// <returns>Yatırım projesi bazında proje bütçe teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Tablo1213Listele(int yil, string senaryoKod, string sektorKod, int model, string[] degerler)
        {
            string sql = "SELECT " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.AD];
            sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.KARAKTERISTIKOZET];
            sql += ", " + VeriErisim.IslecBul("SUM");
            sql += "(" + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.PROJEBUTCE] + ")";
            sql += " FROM " + TABYATProje.TABLOAD + ", " + TABYATAltProje.TABLOAD + ", " + TABYATButceKaynak.TABLOAD + ", " + TABYATButceKaynakTeklif.TABLOAD;

            sql += " WHERE " + VeriErisim.Join(TABYATProje.TABLOAD, TABYATAltProje.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO] });
            sql += " AND " + VeriErisim.Join(TABYATAltProje.TABLOAD, TABYATButceKaynak.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.ALTPROJENO] });
            sql += " AND " + VeriErisim.Join(TABYATButceKaynak.TABLOAD, TABYATButceKaynakTeklif.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.KAYNAKNO] });

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (!string.IsNullOrEmpty(sektorKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.SEKTORNO], "=", sektorKod.Replace(".", "")));
            if (model > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.MODEL], "=", model));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.TEKLIFYIL], "=", 0));

            string orluBirimler = TABGenel.DegerleriOrla(degerler, TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM], 0);
            if (!string.IsNullOrEmpty(orluBirimler))
                sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

            sql += " GROUP BY " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.AD];
            sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.KARAKTERISTIKOZET];
            sql += " ORDER BY " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen parametreleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve birim ve
        /// sektör bazında verilen yılın bir ve iki önceki yılının harcama bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        /// <returns>Birim ve sektör bazında verilen yılın bir ve iki önceki yılının harcama bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader SektorDagilimYilBilgileriListele(int yil, string[] degerler, string senaryoKod)
        {
            string alanlar = TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM];
            alanlar += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.SEKTORNO];
            alanlar += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL];
            alanlar += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.TEKLIFYIL];

            string sql = "SELECT " + alanlar + ", " + VeriErisim.IslecBul("SUM") + "(" + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.PROJEBUTCE] + ")";
            sql += " FROM " + TABYATProje.TABLOAD + ", " + TABYATAltProje.TABLOAD + ", " + TABYATButceKaynak.TABLOAD + ", " + TABYATButceKaynakTeklif.TABLOAD;

            sql += " WHERE " + VeriErisim.Join(TABYATProje.TABLOAD, TABYATAltProje.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO] });
            sql += " AND " + VeriErisim.Join(TABYATAltProje.TABLOAD, TABYATButceKaynak.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.ALTPROJENO] });
            sql += " AND " + VeriErisim.Join(TABYATButceKaynak.TABLOAD, TABYATButceKaynakTeklif.TABLOAD, new string[] { "YIL", "SENARYOKOD", TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.KAYNAKNO] });

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL], ">=", yil - 2));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            string orluBirimler = TABGenel.DegerleriOrla(degerler, TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM], 0);
            if (!string.IsNullOrEmpty(orluBirimler))
                sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

            sql += " GROUP BY " + alanlar;
            sql += " ORDER BY " + alanlar;

            return veriErisim.DataReaderDondur(sql);
        }

        ///// <summary>
        ///// Verilen parametreleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        ///// birim ve sektör bazında ödenek teklif bilgilerini DataReader yapısında döndüren yordam
        ///// </summary>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Senaryo kodu</param>
        ///// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        ///// <returns>Birim ve sektör bazında ödenek teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        //public IDataReader SektorDagilimTeklifBilgileriListele(int yil, string senaryoKod, string[] degerler)
        //{
        //    string sql = "";
        //    //string sql = "SELECT " + TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM];
        //    //sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.SEKTORNO];
        //    //sql += ", " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.YIL];
        //    //sql += ", " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.TEKLIFYIL];
        //    //sql += ", " + VeriErisim.IslecBul("SUM");
        //    //sql += "(" + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.ODENEK] + ")";
        //    //sql += " FROM " + TABYATProje.TABLOAD + ", " + TABYATAltProje.TABLOAD + ", " + TABYATButceKaynak.TABLOAD + ", " + TABYATButceKaynakOdenek.TABLOAD;
        //    //sql += " WHERE " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO];
        //    //sql += " = " + TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.PROJENO];
        //    //sql += " AND " + TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.ALTPROJENO];
        //    //sql += " = " + TABYATButceKaynak.TABLOAD + "." + TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.ALTPROJENO];
        //    //sql += " AND " + TABYATButceKaynak.TABLOAD + "." + TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.KAYNAKNO];
        //    //sql += " = " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.KAYNAKNO];

        //    //if (yil > 0)
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakOdenek.TABLOAD, TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.YIL], "=", yil));
        //    //if (!string.IsNullOrEmpty(senaryoKod))
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakOdenek.TABLOAD, TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

        //    //string orluBirimler = TABGenel.DegerleriOrla(degerler, TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM]);
        //    //if (!string.IsNullOrEmpty(orluBirimler))
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

        //    //sql += " GROUP BY " + TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM];
        //    //sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.SEKTORNO];
        //    //sql += ", " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.YIL];
        //    //sql += ", " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.TEKLIFYIL];
        //    //sql += " ORDER BY " + TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM];
        //    //sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.SEKTORNO];
        //    //sql += ", " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.YIL];
        //    //sql += ", " + TABYATButceKaynakOdenek.TABLOAD + "." + TABYATButceKaynakOdenek.ALANLAR[(int)TABYATButceKaynakOdenek.ENUMAlanlar.TEKLIFYIL];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        ///// <summary>
        ///// Verilen parametreleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        ///// yatırım projesi bazında proje bütçe teklif bilgilerini DataReader yapısında döndüren yordam
        ///// </summary>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="birimKodu">Birim kodu</param>
        ///// <param name="degerler">Kullanıcının yetkili olduğu birim listesi</param>
        ///// <returns>Yatırım projesi bazında proje bütçe teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        //public IDataReader BirimProjeleriTeklifBilgileriListele(int yil, string birimKodu, string[] degerler)
        //{
        //    string sql = "SELECT " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO];
        //    sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.AD];
        //    sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.DPTPROJENO];
        //    sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.GEREKCE];
        //    sql += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL];
        //    sql += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.TEKLIFYIL];
        //    sql += ", " + VeriErisim.IslecBul("SUM");
        //    sql += "(" + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.PROJEBUTCE] + ")";
        //    sql += " FROM " + TABYATProje.TABLOAD + ", " + TABYATAltProje.TABLOAD + ", " + TABYATButceKaynak.TABLOAD + ", " + TABYATButceKaynakTeklif.TABLOAD;
        //    sql += " WHERE " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO];
        //    sql += " = " + TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.PROJENO];
        //    sql += " AND " + TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.ALTPROJENO];
        //    sql += " = " + TABYATButceKaynak.TABLOAD + "." + TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.ALTPROJENO];
        //    sql += " AND " + TABYATButceKaynak.TABLOAD + "." + TABYATButceKaynak.ALANLAR[(int)TABYATButceKaynak.ENUMAlanlar.KAYNAKNO];
        //    sql += " = " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.KAYNAKNO];

        //    if (yil > 0)
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATButceKaynakTeklif.TABLOAD, TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL], "=", yil));
        //    if (!string.IsNullOrEmpty(birimKodu))
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATAltProje.TABLOAD, TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM], "LIKE", birimKodu.Replace(".", "") + "%"));

        //    string orluBirimler = TABGenel.DegerleriOrla(degerler, TABYATAltProje.TABLOAD + "." + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.BIRIM]);
        //    if (!string.IsNullOrEmpty(orluBirimler))
        //        sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

        //    sql += " GROUP BY " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO];
        //    sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.AD];
        //    sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.DPTPROJENO];
        //    sql += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.GEREKCE];
        //    sql += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL];
        //    sql += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.TEKLIFYIL];

        //    sql += " ORDER BY " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO];
        //    sql += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL];
        //    sql += ", " + TABYATButceKaynakTeklif.TABLOAD + "." + TABYATButceKaynakTeklif.ALANLAR[(int)TABYATButceKaynakTeklif.ENUMAlanlar.TEKLIFYIL];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        public IDataReader VizeCetveliRaporla(int yil, string senaryoKod, string birimKodu, string sektorKod)
        {
            string alanlar = "P.SEKTORNO, SEK.SEKTORAD, P.PROJENO, P.DPTPROJENO, P.AD, AP.ALTPROJENO, AP.BIRIM, AP.AD, O.PEBKOD, YATBUTCEKAYNAK.TOPLAMADAHIL, APK.PEBAD, SUBSTRING(O.EKOKOD, 1, 2), O.TEKLIFYIL, O.FINKOD";
            string sql = "SELECT " + alanlar + ", SUM(O.ODENEK)";
            sql += " FROM YATBUTCEKAYNAKPEBKOD AS K INNER JOIN YATBUTCEKAYNAK AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += " INNER JOIN YATALTPROJE AS AP ON " + VeriErisim.Join("BK", "AP", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " INNER JOIN YATPROJE AS P ON " + VeriErisim.Join("AP", "P", new string[] { "YIL", "SENARYOKOD", "PROJENO" });
            sql += " INNER JOIN ButceHazirlikV2.dbo.BHMODENEK AS O ON " + VeriErisim.Join("K", "O", new string[] { "YIL", "SENARYOKOD", "PEBKOD" });
            sql += " INNER JOIN ButceHazirlik.dbo.ABSSEKTOR AS SEK ON " + VeriErisim.Join("P", "SEK", new string[] { "YIL", "SEKTORNO" });
            sql += " INNER JOIN ButceHazirlik.dbo.ABSPEBKOD AS APK ON " + VeriErisim.Join("APK", "K", new string[] { "YIL", "PEBKOD" });
            sql += " WHERE (K.YIL = " + VeriErisim.DegerYap(yil) + ") AND (O.SENARYOKOD = " + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ")";
            if (!string.IsNullOrWhiteSpace(birimKodu))
                sql += " AND " + VeriErisim.KosulOl("AP", "BIRIM", "LIKE", birimKodu.Replace(".", "") + "%");
            if (!string.IsNullOrWhiteSpace(sektorKod))
                sql += " AND " + VeriErisim.KosulOl("P", "SEKTORNO", "=", sektorKod.Replace(".", ""));
            sql += " group by " + alanlar;
            sql += " order by 1,4 DESC,7,9,12";

            return this.veriErisim.DataReaderDondur(sql);
        }
    }
}
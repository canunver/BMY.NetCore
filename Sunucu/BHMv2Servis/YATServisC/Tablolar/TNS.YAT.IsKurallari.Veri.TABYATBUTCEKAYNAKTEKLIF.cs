using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe kaynağın bütçede gösterilmeyen teklif bilgilerini tutan YATBUTCEKAYNAKTEKLIF
    /// veri tabanı tablosu ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATButceKaynakTeklif : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATButceKaynakTeklif(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATBUTCEKAYNAKTEKLIF";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// KAYNAKNO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KAYNAKNO = 0,

            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 1,

            /// <summary>
            /// TEKLIFYIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFYIL = 2,

            /// <summary>
            /// YILBUTCEDIS alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YILBUTCEDIS = 3,

            /// <summary>
            /// PROJEBUTCE alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PROJEBUTCE = 4,

            /// <summary>
            /// PROJEBUTCEDIS alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PROJEBUTCEDISAGIDEN = 5,

            /// <summary>
            /// SENARYOKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 6
        }

        /// <summary>
        /// Bütçe kaynağın teklif bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KAYNAKNO", "YIL", "TEKLIFYIL", "YILBUTCEDIS", "PROJEBUTCE", "PROJEBUTCEDIS", "SENARYOKOD" };

        /// <summary>
        /// Bütçe kaynağın teklif bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="butceYil">Bütçe kaynağın teklif bilgilerini tutan nesne</param>
        /// <param name="teklifYil">Teklif yılı (Çok yıllı bütçe)</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int butceYil, string senaryoKod, int kaynakNo, ENUMProjeMaliYapiKod teklifYil, ProjeMaliYapi pmy)
        {
            if (pmy.DisOzkaynak != 0 || pmy.Toplam != 0 || pmy.FinKod7 != 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(kaynakNo) + ",";
                sql += VeriErisim.DegerYap(butceYil) + ",";
                sql += VeriErisim.DegerYap((int)teklifYil) + ",";
                sql += VeriErisim.DegerYap(pmy.FinKod7) + ",";
                sql += VeriErisim.DegerYap(pmy.Toplam) + ",";
                sql += VeriErisim.DegerYap(pmy.DisOzkaynak) + ",";
                sql += VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ")";

                return KomutCalistir(sql, 1);
            }
            return new Sonuc();
        }

        /// <summary>
        /// Verilen bütçe kaynak teklif nesnesini kullanarak,
        /// ilgili teklif bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="teklif">Bütçe kaynağın teklif bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KaynakSil(int butceYil, string senaryoKod, int kaynakNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Verilen bütçe kaynak teklif nesnesini kullanarak,
        /// ilgili teklif bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="butceYil">Silinecek kaynağın yılı</param>
        /// <param name="kaynakNo">Silinecek kaynağın numarası</param>
        /// <param name="senaryoKod">Silinecek kaynağın senaryo kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int butceYil, int kaynakNo, string senaryoKod, int YilOnckIkiOnceki, ENUMProjeMaliYapiKod teklifYil = ENUMProjeMaliYapiKod.TANIMSIZ)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (teklifYil == ENUMProjeMaliYapiKod.TANIMSIZ)
            {
                if (YilOnckIkiOnceki == 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", ALANLAR[(int)ENUMAlanlar.TEKLIFYIL] + " IN (-1, 0, 1, 2, 6)");
                else if (YilOnckIkiOnceki == 1)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", ALANLAR[(int)ENUMAlanlar.TEKLIFYIL] + " IN (-7, -3, -4, -2)");
                }
                else if (YilOnckIkiOnceki == 2)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", ALANLAR[(int)ENUMAlanlar.TEKLIFYIL] + " IN (-2)");
                }
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFYIL], "=", (int)teklifYil));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen değişkenleri kıstas olarak kullanarak, select sql cümlesini oluşturup,
        /// çalıştıran ve bütçe kaynak teklif bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="kaynakNo">Bütçe kaynağın programatik kodu</param>
        /// <param name="butceYil">Yıl kriteri</param>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <param name="senaryoKod">Butce yıla ait rakamların hangi yıla ait olduğu</param>
        /// <param name="birimKodlar">Kullanıcının yetkili olduğu birim listesi</param>
        /// <param name="tekYil">Sadece verilen yılın rakamını getirir.</param>
        /// <returns>Verilen kriterlere uygun olan bütçe kaynak teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int kaynakNo, int butceYil, int projeKodu, string senaryoKod, string[] birimKodlar, bool tekYil, bool eskiYillarKBO) //, bool tumYillar, int ilkYil,
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD + ", YATBUTCEKAYNAK, YATALTPROJE";
            sql += " WHERE " + VeriErisim.Join(TABLOAD, "YATBUTCEKAYNAK", new string[] { ALANLAR[(int)ENUMAlanlar.KAYNAKNO] });
            sql += " AND " + VeriErisim.Join("YATBUTCEKAYNAK", "YATALTPROJE", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAK", "YIL", "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAK", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));

            if (kaynakNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));

            if (projeKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATALTPROJE", "PROJENO", "=", projeKodu));

            string orluBirimler = TABGenel.DegerleriOrla(birimKodlar, "YATALTPROJE.BIRIM", projeKodu);
            if (orluBirimler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

            if (!eskiYillarKBO)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            else
            {
                int grupSay = 2;
                string garipKosul = "";
                for (int grup = 0; grup < grupSay; grup++)
                {
                    string icKosul;
                    if (grup == 0) icKosul = "(TEKLIFYIL >= -1)";
                    else icKosul = "(TEKLIFYIL < -1)";
                    string k1 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil - grup);
                    k1 = VeriErisim.KosulEkle(k1, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

                    string k2 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "<>", butceYil - grup);
                    k2 = VeriErisim.KosulEkle(k2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", "0600"));

                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", "((" + k1 + ") OR (" + k2 + "))");

                    if (garipKosul != "") garipKosul += " OR ";
                    garipKosul += "(" + icKosul + ")";
                }
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + garipKosul + ")");
            }

            if (tekYil)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil));
            }

            sql += " ORDER BY YATALTPROJE.ALTPROJENO";
            sql += "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO];
            sql += "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " DESC";
            sql += "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen değişkenleri kıstas olarak kullanarak, select sql cümlesini oluşturup,
        /// çalıştıran ve bütçe kaynak teklif bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="kaynakNo">Bütçe kaynağın programatik kodu</param>
        /// <param name="butceYil">Yıl kriteri</param>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <param name="senaryoKod">Butce yıla ait rakamların hangi yıla ait olduğu</param>
        /// <param name="birimKodlar">Kullanıcının yetkili olduğu birim listesi</param>
        /// <param name="tekYil">Sadece verilen yılın rakamını getirir.</param>
        /// <returns>Verilen kriterlere uygun olan bütçe kaynak teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public static string Listele(string kaynakNoUstTabloBag, int butceYil, string senaryoKod, bool tekYil, bool eskiYillarKBO) //, bool tumYillar, int ilkYil,
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO] + "=" + kaynakNoUstTabloBag);

            string eskiSenaryo = senaryoKod.Replace(".", "");
            if (eskiYillarKBO)
                eskiSenaryo = "0600";
            string eskiYilKosul = "<=";
            if (tekYil)
                eskiYilKosul = "=";
            int grupSay = 2;
            string garipKosul = "";
            for (int grup = 0; grup < grupSay; grup++)
            {
                string icKosul;
                if (grup == 0) icKosul = "(TEKLIFYIL >= -1)";
                else icKosul = "(TEKLIFYIL < -1)";
                string k1 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil - grup);
                k1 = VeriErisim.KosulEkle(k1, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

                string k2 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], eskiYilKosul, butceYil - grup - 1);
                k2 = VeriErisim.KosulEkle(k2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", eskiSenaryo));

                icKosul = VeriErisim.KosulEkle(icKosul, "AND", "((" + k1 + ") OR (" + k2 + "))");

                if (garipKosul != "") garipKosul += " OR ";
                garipKosul += "(" + icKosul + ")";
            }
            sql = VeriErisim.KosulEkle(sql, "AND", "(" + garipKosul + ")");

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " DESC";
            sql += "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD];

            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen değişkenleri kıstas olarak kullanarak, select sql cümlesini oluşturup,
        /// çalıştıran ve bütçe kaynak teklif bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="kaynakNo">Bütçe kaynağın programatik kodu</param>
        /// <param name="butceYil">Yıl kriteri</param>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <param name="senaryoKod">Butce yıla ait rakamların hangi yıla ait olduğu</param>
        /// <param name="birimKodlar">Kullanıcının yetkili olduğu birim listesi</param>
        /// <param name="tumYillar">Projenin geçmiş yıl rakamlarını da getirir.</param>
        /// <param name="ilkYil">Eğer verilmişse projenşn verilen yıllardaki rakamları getirilir.</param>
        /// <returns>Verilen kriterlere uygun olan bütçe kaynak teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader ListeleToplam(int kaynakNo, int butceYil, int projeKodu, string senaryoKod, string[] birimKodlar, bool tumYillar, int ilkYil, string kurKod, string sektorKod, bool sektorVar, bool birimVar)
        {
            string alanlar = "";
            if (birimVar) alanlar += "YATALTPROJE.BIRIM,";
            if (sektorVar) alanlar += "YATPROJE.SEKTORNO,";
            alanlar += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TEKLIFYIL];
            string sumAlanlar = "SUM" + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJEBUTCE] + "), ";
            sumAlanlar += "SUM" + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YILBUTCEDIS] + "), ";
            sumAlanlar += "SUM" + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJEBUTCEDISAGIDEN] + ")";


            string sql = "SELECT " + alanlar + "," + sumAlanlar;
            sql += " FROM " + TABLOAD + ", YATBUTCEKAYNAK, YATALTPROJE, YATPROJE";
            sql += " WHERE " + VeriErisim.Join(TABLOAD, "YATBUTCEKAYNAK", new string[] { ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "YIL", "SENARYOKOD" });
            sql += " AND " + VeriErisim.Join("YATBUTCEKAYNAK", "YATALTPROJE", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " AND " + VeriErisim.Join("YATALTPROJE", "YATPROJE", new string[] { "YIL", "SENARYOKOD", "PROJENO" });

            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATALTPROJE", "BIRIM", "LIKE", kurKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(sektorKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATPROJE", "SEKTORNO", "LIKE", sektorKod.Replace(".", "") + "%"));

            if (kaynakNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));

            if (projeKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATALTPROJE", "PROJENO", "=", projeKodu));

            string orluBirimler = TABGenel.DegerleriOrla(birimKodlar, "YATALTPROJE.BIRIM", 0);
            if (orluBirimler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

            string garipKosul = "";
            for (int grup = 0; grup < 2; grup++)
            {
                string icKosul;
                if (grup == 0) icKosul = "(TEKLIFYIL >= -1)";
                else icKosul = "(TEKLIFYIL < -1)";
                if (ilkYil > 0)
                {
                    string k1 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil - grup);
                    k1 = VeriErisim.KosulEkle(k1, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

                    string k2 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "<", butceYil - grup);
                    k2 = VeriErisim.KosulEkle(k2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], ">=", ilkYil - grup));
                    k2 = VeriErisim.KosulEkle(k2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", "0600"));

                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", "((" + k1 + ") OR (" + k2 + "))");
                }
                else if (tumYillar)
                {
                    string k1 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil - grup);
                    k1 = VeriErisim.KosulEkle(k1, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

                    string k2 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "<", butceYil - grup);
                    k2 = VeriErisim.KosulEkle(k2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", "0600"));

                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", "((" + k1 + ") OR (" + k2 + "))");
                }
                else
                {
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil - grup));
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
                }
                if (garipKosul != "") garipKosul += " OR ";
                garipKosul += "(" + icKosul + ")";
            }
            sql = VeriErisim.KosulEkle(sql, "AND", "(" + garipKosul + ")");

            sql += " GROUP BY " + alanlar;
            sql += " ORDER BY " + alanlar;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen değişkenleri kıstas olarak kullanarak, select sql cümlesini oluşturup,
        /// çalıştıran ve bütçe kaynak teklif bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="kaynakNo">Bütçe kaynağın programatik kodu</param>
        /// <param name="butceYil">Yıl kriteri</param>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <param name="senaryoKod">Butce yıla ait rakamların hangi yıla ait olduğu</param>
        /// <param name="birimKodlar">Kullanıcının yetkili olduğu birim listesi</param>
        /// <param name="tumYillar">Projenin geçmiş yıl rakamlarını da getirir.</param>
        /// <param name="ilkYil">Eğer verilmişse projenşn verilen yıllardaki rakamları getirilir.</param>
        /// <returns>Verilen kriterlere uygun olan bütçe kaynak teklif bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader ListeleProjeToplam(int kaynakNo, int butceYil, int projeKodu, string senaryoKod, string[] birimKodlar, bool tumYillar, int ilkYil, string kurKod, string sektorKod, bool altProjeVar)
        {
            string alanlar = TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.PROJENO];
            alanlar += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.AD];
            alanlar += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.DPTPROJENO];
            alanlar += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.GEREKCE];
            alanlar += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.SEKTORNO];
            alanlar += ", " + TABYATProje.TABLOAD + "." + TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.KARAKTERISTIK] + ",";

            if (altProjeVar)
            {
                alanlar += "YATALTPROJE.ALTPROJENO,YATALTPROJE.BIRIM,YATALTPROJE.AD,";
            }
            alanlar += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TEKLIFYIL];
            string sumAlanlar = "SUM" + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJEBUTCE] + "), ";
            sumAlanlar += "SUM" + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YILBUTCEDIS] + "), ";
            sumAlanlar += "SUM" + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJEBUTCEDISAGIDEN] + ")";

            string sql = "SELECT " + alanlar + "," + sumAlanlar;
            sql += " FROM " + TABLOAD + ", YATBUTCEKAYNAK, YATALTPROJE, YATPROJE";
            sql += " WHERE " + VeriErisim.Join(TABLOAD, "YATBUTCEKAYNAK", new string[] { ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "YIL", "SENARYOKOD" });
            sql += " AND " + VeriErisim.Join("YATBUTCEKAYNAK", "YATALTPROJE", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " AND " + VeriErisim.Join("YATALTPROJE", "YATPROJE", new string[] { "YIL", "SENARYOKOD", "PROJENO" });

            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATALTPROJE", "BIRIM", "LIKE", kurKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(sektorKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATPROJE", "SEKTORNO", "LIKE", sektorKod.Replace(".", "") + "%"));

            if (kaynakNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));

            if (projeKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATALTPROJE", "PROJENO", "=", projeKodu));

            string orluBirimler = TABGenel.DegerleriOrla(birimKodlar, "YATALTPROJE.BIRIM", 0);
            if (orluBirimler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

            string garipKosul = "";
            for (int grup = 0; grup < 2; grup++)
            {
                string icKosul;
                if (grup == 0) icKosul = "(TEKLIFYIL >= -1)";
                else icKosul = "(TEKLIFYIL < -1)";
                if (ilkYil > 0)
                {
                    string k1 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil - grup);
                    k1 = VeriErisim.KosulEkle(k1, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

                    string k2 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "<", butceYil - grup);
                    k2 = VeriErisim.KosulEkle(k2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], ">=", ilkYil - grup));
                    k2 = VeriErisim.KosulEkle(k2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", "0600"));

                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", "((" + k1 + ") OR (" + k2 + "))");
                }
                else if (tumYillar)
                {
                    string k1 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil - grup);
                    k1 = VeriErisim.KosulEkle(k1, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

                    string k2 = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "<", butceYil - grup);
                    k2 = VeriErisim.KosulEkle(k2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", "0600"));

                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", "((" + k1 + ") OR (" + k2 + "))");
                }
                else
                {
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", butceYil - grup));
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
                }
                if (garipKosul != "") garipKosul += " OR ";
                garipKosul += "(" + icKosul + ")";
            }
            sql = VeriErisim.KosulEkle(sql, "AND", "(" + garipKosul + ")");

            sql += " GROUP BY " + alanlar;
            sql += " ORDER BY " + alanlar;

            return veriErisim.DataReaderDondur(sql);
        }

        //public bool ButceKaynakTeklifVarMi(ButceKaynakTeklif teklif)
        //{
        //    string[] intAlan = { ALANLAR[(int)ENUMAlanlar.KAYNAKNO], ALANLAR[(int)ENUMAlanlar.YIL] };
        //    int[] intDeger = { teklif.kaynakNo, teklif.yil };

        //    string[] strAlan = new string[0];
        //    string[] strDeger = new string[0];

        //    return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        //}

        public Sonuc EksikSatirlariDoldur(int butceYil, int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int kaynakNo)
        {
            senaryoKod = senaryoKod.Replace(".", "");
            string sql = "INSERT INTO YATBUTCEKAYNAKTEKLIF";
            sql += " SELECT KAYNAKNO, " + VeriErisim.DegerYap(yil) + " YIL, SENARYOKOD, " + VeriErisim.DegerYap((int)teklifYil) + " TEKLIFYIL, 0 YILBUTCEDIS, 0 PROJEBUTCE, 0 PROJEBUTCEDIS";
            sql += " FROM YATBUTCEKAYNAK K";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            sql += " and 1 NOT IN (";
            sql += " select 1 from  YATBUTCEKAYNAKTEKLIF T";
            sql += " where T.KAYNAKNO = K.KAYNAKNO";
            sql += " and T.TEKLIFYIL = " + VeriErisim.DegerYap((int)teklifYil) + " AND T.YIL=" + VeriErisim.DegerYap(yil) + " AND T.SENARYOKOD=K.SENARYOKOD ";
            sql += ")";
            if (kaynakNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("K", "KAYNAKNO", "=", kaynakNo));
            return KomutCalistir(sql, 0);
        }

        public Sonuc OdenekKopyala(int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int kaynakNo)
        {
            string sql = "UPDATE YATBUTCEKAYNAKTEKLIF SET YILBUTCEDIS = ({0}), PROJEBUTCE= ({1}) WHERE {2}";
            string toplamSql = "SELECT SUM(O.ODENEK) FROM YATBUTCEKAYNAKPEBKOD AS P INNER JOIN ButceHazirlikV2.dbo.BHMODENEK AS O ON P.YIL = O.YIL AND P.PEBKOD = O.PEBKOD AND P.SENARYOKOD = O.SENARYOKOD";
            toplamSql += " WHERE (O.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (O.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (O.TEKLIFYIL = YATBUTCEKAYNAKTEKLIF.TEKLIFYIL) AND (P.KAYNAKNO = YATBUTCEKAYNAKTEKLIF.KAYNAKNO)";
            string fin7sql = toplamSql + " AND (O.FINKOD = 7)";

            string kosul = VeriErisim.KosulOl("", "YIL", "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TEKLIFYIL", "=", (int)teklifYil));
            if (kaynakNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKTEKLIF", "KAYNAKNO", "=", kaynakNo));
            kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE " + VeriErisim.Join("K", "YATBUTCEKAYNAKTEKLIF", new string[] { "KAYNAKNO", "YIL", "SENARYOKOD" }) + " AND K.KAYNAKTUR<=2)";
            sql = String.Format(sql, fin7sql, toplamSql, kosul);
            return KomutCalistir(sql, 0, 55000);
        }

        public Sonuc EklenenKopyala(int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int kaynakNo)
        {
            string sql = "UPDATE YATBUTCEKAYNAKTEKLIF SET YILBUTCEDIS = ({0}), PROJEBUTCE= ({1}) WHERE {2}";
            string toplamSql = "SELECT SUM(O.MIKTAR) FROM MUHPEBEKLENEN O WHERE (O.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (O.KAYNAKNO = YATBUTCEKAYNAKTEKLIF.KAYNAKNO) AND (O.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD)";
            string fin7sql = toplamSql + " AND (O.FINKOD = 7)";

            string kosul = VeriErisim.KosulOl("", "YIL", "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TEKLIFYIL", "=", (int)teklifYil));
            if (kaynakNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKTEKLIF", "KAYNAKNO", "=", kaynakNo));

            kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE " + VeriErisim.Join("K", "YATBUTCEKAYNAKTEKLIF", new string[] { "KAYNAKNO", "YIL", "SENARYOKOD" }) + " AND K.KAYNAKTUR<=2)";
            sql = String.Format(sql, fin7sql, toplamSql, kosul);
            return KomutCalistir(sql, 0, 55000);
        }

        public Sonuc DusulenKopyala(int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int kaynakNo)
        {
            string sql = "UPDATE YATBUTCEKAYNAKTEKLIF SET YILBUTCEDIS = ({0}), PROJEBUTCE= ({1}) WHERE {2}";
            string toplamSql = "SELECT SUM(O.MIKTAR) FROM MUHPEBDUSULEN O WHERE (O.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (O.KAYNAKNO = YATBUTCEKAYNAKTEKLIF.KAYNAKNO) AND (O.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD)";
            string fin7sql = toplamSql + " AND (O.FINKOD = 7)";

            string kosul = VeriErisim.KosulOl("", "YIL", "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TEKLIFYIL", "=", (int)teklifYil));
            if (kaynakNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKTEKLIF", "KAYNAKNO", "=", kaynakNo));

            kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE " + VeriErisim.Join("K", "YATBUTCEKAYNAKTEKLIF", new string[] { "KAYNAKNO", "YIL", "SENARYOKOD" }) + " AND K.KAYNAKTUR<=2)";
            sql = String.Format(sql, fin7sql, toplamSql, kosul);
            return KomutCalistir(sql, 0, 55000);
        }

        public Sonuc OdenekKopyalaEk(int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int kaynakNo)
        {
            string ek = "";
            if (teklifYil == ENUMProjeMaliYapiKod.ekteklif1)
                ek = "1";
            else if (teklifYil == ENUMProjeMaliYapiKod.ekteklif2)
                ek = "2";
            string sql = "UPDATE YATBUTCEKAYNAKTEKLIF SET YILBUTCEDIS = ({0}), PROJEBUTCE= ({1}) WHERE {2}";
            string toplamSql = "SELECT SUM(O.ODENEK" + ek + ") FROM YATBUTCEKAYNAKPEBKOD AS P INNER JOIN ButceHazirlikV2.dbo.BHMTAVANUSTU AS O ON P.YIL = O.YIL AND P.PEBKOD = O.PEBKOD AND P.SENARYOKOD = O.SENARYOKOD";
            toplamSql += " WHERE (O.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (O.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (P.KAYNAKNO = YATBUTCEKAYNAKTEKLIF.KAYNAKNO)";
            string fin7sql = toplamSql + " AND (O.FINKOD = 7)";

            string kosul = VeriErisim.KosulOl("", "YIL", "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TEKLIFYIL", "=", (int)teklifYil));
            if (kaynakNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKTEKLIF", "KAYNAKNO", "=", kaynakNo));

            kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE " + VeriErisim.Join("K", "YATBUTCEKAYNAKTEKLIF", new string[] { "KAYNAKNO", "YIL", "SENARYOKOD" }) + " AND K.KAYNAKTUR<=2)";
            sql = String.Format(sql, fin7sql, toplamSql, kosul);
            return KomutCalistir(sql, 0, 55000);
        }

        public Sonuc HarcamaKopyala(int butceYil, int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int ay, int kaynakNo)
        {
            string sql = "UPDATE YATBUTCEKAYNAKTEKLIF SET YILBUTCEDIS = ({0}), PROJEBUTCE= ({1}) WHERE {2}";

            string toplamSql = "SELECT SUM(O.BORC - O.ALACAK) FROM ButceIslemleri.dbo.MUHMIF O INNER JOIN";
            toplamSql += " ButceIslemleri.dbo.MUHMIFFORM ON O.YIL = ButceIslemleri.dbo.MUHMIFFORM.YIL AND ";
            toplamSql += " O.BELGENO = ButceIslemleri.dbo.MUHMIFFORM.BELGENO INNER JOIN";
            toplamSql += " dbo.YATBUTCEKAYNAKPEBKOD AS PK ON PK.YIL = " + VeriErisim.DegerYap(butceYil) + " AND PK.SENARYOKOD = " + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND PK.PEBKOD = O.PEBKOD";
            toplamSql += " WHERE (O.HESAPKOD LIKE '830%') AND (ButceIslemleri.dbo.MUHMIFFORM.FORMDURUM <= 6) AND (O.KURKOD <> '')";
            toplamSql += " AND (O.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (PK.KAYNAKNO = YATBUTCEKAYNAKTEKLIF.KAYNAKNO) AND (MONTH(ButceIslemleri.dbo.MUHMIFFORM.ISLEMTARIH) <= " + ay.ToString() + ")";
            string fin7sql = toplamSql + " AND (O.FINKOD = '07')";

            string kosul = VeriErisim.KosulOl("", "YIL", "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TEKLIFYIL", "=", (int)teklifYil));
            if (kaynakNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKTEKLIF", "KAYNAKNO", "=", kaynakNo));
            kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE K.KAYNAKNO=YATBUTCEKAYNAKTEKLIF.KAYNAKNO AND K.YIL=" + VeriErisim.DegerYap(butceYil) + " AND K.SENARYOKOD=YATBUTCEKAYNAKTEKLIF.SENARYOKOD AND K.KAYNAKTUR<=2)";
            sql = String.Format(sql, fin7sql, toplamSql, kosul);
            return KomutCalistir(sql, 0, 55000);
        }

        public Sonuc HarcamaTahminKopyala(int butceYil, int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int kaynakNo)
        {
            string sql = "UPDATE YATBUTCEKAYNAKTEKLIF SET YILBUTCEDIS = ({0}), PROJEBUTCE= ({1}) WHERE {2}";
            string toplamSql = "SELECT SUM(O.ODENEK) FROM YATBUTCEKAYNAKPEBKOD AS P INNER JOIN ButceHazirlikV2.dbo.BHMTAHMINGIDER AS O ON P.YIL = " + VeriErisim.DegerYap(butceYil) + " AND P.PEBKOD = O.PEBKOD AND P.SENARYOKOD = O.SENARYOKOD";
            toplamSql += " WHERE (O.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (O.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (O.TEKLIFYIL = 0) AND (P.KAYNAKNO = YATBUTCEKAYNAKTEKLIF.KAYNAKNO)";
            string fin7sql = toplamSql + " AND (O.FINKOD = 7)";

            string kosul = VeriErisim.KosulOl("", "YIL", "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TEKLIFYIL", "=", (int)teklifYil));
            if (kaynakNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKTEKLIF", "KAYNAKNO", "=", kaynakNo));
            //kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE " + VeriErisim.Join("K", "YATBUTCEKAYNAKTEKLIF", new string[] { "KAYNAKNO", "YIL", "SENARYOKOD" }) + " AND K.KAYNAKTUR<=2)";
            kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE K.KAYNAKNO=YATBUTCEKAYNAKTEKLIF.KAYNAKNO AND K.YIL=" + VeriErisim.DegerYap(butceYil) + " AND K.SENARYOKOD=YATBUTCEKAYNAKTEKLIF.SENARYOKOD AND K.KAYNAKTUR<=2)";
            sql = String.Format(sql, fin7sql, toplamSql, kosul);
            return KomutCalistir(sql, 0, 55000);
        }

        public Sonuc KumulatifHarcamaTahminiOl(int butceYil, int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int kaynakNo)
        {
            string sql = "UPDATE YATBUTCEKAYNAKTEKLIF SET YILBUTCEDIS = ({0}), PROJEBUTCE= ({1}) WHERE {2}";

            string toplamSql = "SELECT SUM(IC.{0} * D.DEFLATOR) FROM YATBUTCEKAYNAKTEKLIF IC INNER JOIN YATBUTCEKAYNAK AS K ON " + VeriErisim.DegerYap(butceYil) + "=K.YIL AND IC.SENARYOKOD=K.SENARYOKOD AND IC.KAYNAKNO=K.KAYNAKNO";
            toplamSql += " INNER JOIN YATALTPROJE AS AP ON " + VeriErisim.Join("K", "AP", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            toplamSql += " INNER JOIN YATPROJE AS P ON " + VeriErisim.Join("AP", "P", new string[] { "YIL", "SENARYOKOD", "PROJENO" });
            toplamSql += " INNER JOIN YATDEFLATOR AS D ON " + VeriErisim.DegerYap(butceYil) + "=D.YIL AND D.SENARYOKOD= " + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND D.SEKTORKOD=SUBSTRING(P.SEKTORNO, 1, 2)";

            toplamSql += " WHERE (IC.KAYNAKNO = YATBUTCEKAYNAKTEKLIF.KAYNAKNO) AND ";
            toplamSql += " (((IC.YIL = YATBUTCEKAYNAKTEKLIF.YIL - 1) AND (IC.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (IC.TEKLIFYIL = " + VeriErisim.DegerYap((int)ENUMProjeMaliYapiKod.kumulatifHarcama) + ")) OR";
            toplamSql += " ((IC.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (IC.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (IC.TEKLIFYIL = " + VeriErisim.DegerYap((int)ENUMProjeMaliYapiKod.harcamaTahmin) + ")))";

            string kosul = VeriErisim.KosulOl("", "YIL", "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TEKLIFYIL", "=", (int)teklifYil));
            if (kaynakNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKTEKLIF", "KAYNAKNO", "=", kaynakNo));
            //kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE " + VeriErisim.Join("K", "YATBUTCEKAYNAKTEKLIF", new string[] { "KAYNAKNO", "YIL", "SENARYOKOD" }) + " AND K.KAYNAKTUR<=2)";
            kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE K.KAYNAKNO=YATBUTCEKAYNAKTEKLIF.KAYNAKNO AND K.YIL=" + VeriErisim.DegerYap(butceYil) + " AND K.SENARYOKOD=YATBUTCEKAYNAKTEKLIF.SENARYOKOD)";
            sql = String.Format(sql, string.Format(toplamSql, "YILBUTCEDIS"), string.Format(toplamSql, "PROJEBUTCE"), kosul);
            return KomutCalistir(sql, 0, 55000);

            /*
             * 
                         string sql = "UPDATE YATBUTCEKAYNAKTEKLIF SET YILBUTCEDIS = ({0}), PROJEBUTCE= ({1}) WHERE {2}";

            string toplamSql = "SELECT SUM(IC.{0} * D.DEFLATOR) FROM YATBUTCEKAYNAKTEKLIF IC INNER JOIN YATBUTCEKAYNAK AS K ON " + VeriErisim.DegerYap(butceYil) + "=K.YIL AND IC.SENARYOKOD=K.SENARYOKOD AND IC.KAYNAKNO=K.KAYNAKNO";
            toplamSql += " INNER JOIN YATALTPROJE AS AP ON " + VeriErisim.Join("K", "AP", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            toplamSql += " INNER JOIN YATPROJE AS P ON " + VeriErisim.Join("AP", "P", new string[] { "YIL", "SENARYOKOD", "PROJENO" });
            toplamSql += " INNER JOIN YATDEFLATOR AS D ON " + VeriErisim.DegerYap(butceYil) + "=D.YIL AND D.SENARYOKOD= " + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND D.SEKTORKOD=SUBSTRING(P.SEKTORNO, 1, 2)";

            toplamSql += " WHERE (IC.KAYNAKNO = YATBUTCEKAYNAKTEKLIF.KAYNAKNO) AND ";
            toplamSql += " (((IC.YIL = YATBUTCEKAYNAKTEKLIF.YIL - 1) AND (IC.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (IC.TEKLIFYIL = " + VeriErisim.DegerYap((int)ENUMProjeMaliYapiKod.kumulatifHarcama) + ")) OR";
            toplamSql += " ((IC.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (IC.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (IC.TEKLIFYIL = " + VeriErisim.DegerYap((int)ENUMProjeMaliYapiKod.harcamaTahmin) + ")))";

            string kosul = VeriErisim.KosulOl("", "YIL", "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TEKLIFYIL", "=", (int)teklifYil));
            if (kaynakNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKTEKLIF", "KAYNAKNO", "=", kaynakNo));
            //kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE " + VeriErisim.Join("K", "YATBUTCEKAYNAKTEKLIF", new string[] { "KAYNAKNO", "YIL", "SENARYOKOD" }) + " AND K.KAYNAKTUR<=2)";
            kosul += " AND 1 IN (SELECT 1 FROM YATBUTCEKAYNAK K WHERE K.KAYNAKNO=YATBUTCEKAYNAKTEKLIF.KAYNAKNO AND K.YIL=" + VeriErisim.DegerYap(butceYil) + " AND K.SENARYOKOD=YATBUTCEKAYNAKTEKLIF.SENARYOKOD)";
            sql = String.Format(sql, string.Format(toplamSql, "YILBUTCEDIS"), string.Format(toplamSql, "PROJEBUTCE"), kosul);
            return KomutCalistir(sql, 0, 55000);

             * 
             * */
        }

        public Sonuc ProjeBedeliOlDevamEden(int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int kaynakNo)
        {
            string sql = "UPDATE YATBUTCEKAYNAKTEKLIF SET YILBUTCEDIS = ({0}), PROJEBUTCE= ({1}) WHERE {2}";

            string toplamSql = "SELECT SUM(IC.{0}) FROM YATBUTCEKAYNAKTEKLIF IC ";

            toplamSql += " WHERE (IC.KAYNAKNO = YATBUTCEKAYNAKTEKLIF.KAYNAKNO) AND ";
            toplamSql += " (((IC.YIL = YATBUTCEKAYNAKTEKLIF.YIL - 1) AND (IC.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (IC.TEKLIFYIL = " + VeriErisim.DegerYap((int)ENUMProjeMaliYapiKod.kumulatifHarcama) + ")) OR";
            toplamSql += " ((IC.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (IC.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (IC.TEKLIFYIL = " + VeriErisim.DegerYap((int)ENUMProjeMaliYapiKod.teklif0) + ")) OR";
            toplamSql += " ((IC.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (IC.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (IC.TEKLIFYIL = " + VeriErisim.DegerYap((int)ENUMProjeMaliYapiKod.teklif1) + ")) OR";
            toplamSql += " ((IC.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (IC.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (IC.TEKLIFYIL = " + VeriErisim.DegerYap((int)ENUMProjeMaliYapiKod.teklif2) + ")) OR";
            toplamSql += " ((IC.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (IC.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (IC.TEKLIFYIL = " + VeriErisim.DegerYap((int)ENUMProjeMaliYapiKod.sonrakiYillar) + ")))";

            string kosul = VeriErisim.KosulOl("", "YIL", "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TEKLIFYIL", "=", (int)teklifYil));
            if (kaynakNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKTEKLIF", "KAYNAKNO", "=", kaynakNo));

            kosul += " AND 1 IN (";
            kosul += "SELECT 1 FROM YATBUTCEKAYNAK K INNER JOIN YATALTPROJE AP ON " + VeriErisim.Join("K", "AP", new string[] { "ALTPROJENO", "YIL", "SENARYOKOD" }) + " INNER JOIN YATPROJE P ON " + VeriErisim.Join("AP", "P", new string[] { "PROJENO", "YIL", "SENARYOKOD" }) + " WHERE " + VeriErisim.Join("K", "YATBUTCEKAYNAKTEKLIF", new string[] { "KAYNAKNO", "YIL", "SENARYOKOD" }) + " AND P.ESKIYENI=0 ";
            kosul += ")";
            sql = String.Format(sql, string.Format(toplamSql, "YILBUTCEDIS"), string.Format(toplamSql, "PROJEBUTCE"), kosul);
            return KomutCalistir(sql, 0, 55000);
        }

        public Sonuc ProjeBedeliOlEskiYeni(int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int kaynakNo)
        {
            string sql = "UPDATE YATBUTCEKAYNAKTEKLIF SET YILBUTCEDIS = ({0}), PROJEBUTCE= ({1}) WHERE {2}";

            string toplamSql = "SELECT SUM(IC.{0}) FROM YATBUTCEKAYNAKTEKLIF IC WHERE (IC.KAYNAKNO = YATBUTCEKAYNAKTEKLIF.KAYNAKNO) AND (IC.YIL = YATBUTCEKAYNAKTEKLIF.YIL) AND (IC.SENARYOKOD = YATBUTCEKAYNAKTEKLIF.SENARYOKOD) AND (IC.TEKLIFYIL = " + VeriErisim.DegerYap((int)ENUMProjeMaliYapiKod.teklif0) + ")";

            string kosul = VeriErisim.KosulOl("", "YIL", "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TEKLIFYIL", "=", (int)teklifYil));
            if (kaynakNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKTEKLIF", "KAYNAKNO", "=", kaynakNo));

            kosul += " AND 1 IN (";
            kosul += "SELECT 1 FROM YATBUTCEKAYNAK K INNER JOIN YATALTPROJE AP ON " + VeriErisim.Join("K", "AP", new string[] { "ALTPROJENO", "YIL", "SENARYOKOD" }) + " INNER JOIN YATPROJE P ON " + VeriErisim.Join("AP", "P", new string[] { "PROJENO", "YIL", "SENARYOKOD" }) + " WHERE " + VeriErisim.Join("K", "YATBUTCEKAYNAKTEKLIF", new string[] { "KAYNAKNO", "YIL", "SENARYOKOD" }) + " AND P.ESKIYENI=1 ";
            kosul += ")";
            sql = String.Format(sql, string.Format(toplamSql, "YILBUTCEDIS"), string.Format(toplamSql, "PROJEBUTCE"), kosul);
            return KomutCalistir(sql, 0, 55000);
        }

        public Sonuc AsamaSil(int kaynakYil, string kaynakAsama, string senaryoKod, string projeKodListe)
        {
            string sql = "DELETE FROM " + TABLOAD;
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (projeKodListe != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "(1 in (" + TABYATButceKaynak.KaynakProjeyeBagla(projeKodListe, TABLOAD, kaynakYil, kaynakAsama) + "))");
            }

            return KomutCalistir(sql, 0);
        }

        public Sonuc AsamaKopyala(int kaynakYil, string kaynakAsama, string hedefAsama, string projeKodListe)
        {
            string sql = "INSERT INTO " + TABLOAD + "(KAYNAKNO, YIL, TEKLIFYIL, YILBUTCEDIS, PROJEBUTCE, PROJEBUTCEDIS, SENARYOKOD) ";
            sql += "SELECT KAYNAKNO, YIL, TEKLIFYIL, YILBUTCEDIS, PROJEBUTCE, PROJEBUTCEDIS, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD FROM " + TABLOAD;
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            if (projeKodListe != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "(1 in (" + TABYATButceKaynak.KaynakProjeyeBagla(projeKodListe, TABLOAD, kaynakYil, kaynakAsama) + "))");
            }

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeMaliYapi(int raporYil, string raporSenaryoKod, int yil, string senaryoKod, ENUMProjeMaliYapiKod teklifYil, int projeNo, int altProjeNo, bool kucukEsit, bool detayAltProje)
        {
            string alanlar = "AP.PROJENO, AP.ALTPROJENO, K.KAYNAKTUR";
            string gruplar = "AP.PROJENO, AP.ALTPROJENO, K.KAYNAKTUR";

            if (!detayAltProje)
            {
                alanlar = "AP.PROJENO, 0, K.KAYNAKTUR";
                gruplar = "AP.PROJENO, K.KAYNAKTUR";
            }
            string sql = "SELECT " + alanlar + ", SUM(KT.YILBUTCEDIS), SUM(KT.PROJEBUTCE), SUM(KT.PROJEBUTCEDIS) " +
                         " FROM YATBUTCEKAYNAKTEKLIF AS KT INNER JOIN YATBUTCEKAYNAK AS K ON KT.KAYNAKNO=K.KAYNAKNO AND K.YIL=" + VeriErisim.DegerYap(raporYil) + " AND K.SENARYOKOD=" + VeriErisim.DegerYap(raporSenaryoKod.Replace(".", "")) +
                         " INNER JOIN  YATALTPROJE AS AP ON " + VeriErisim.Join("K", "AP", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("KT", "YIL", kucukEsit ? "<=" : "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KT", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KT", "TEKLIFYIL", "=", (int)teklifYil));
            if (altProjeNo == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", "K.TOPLAMADAHIL IN (1,2)");

            if (projeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "PROJENO", "=", projeNo));

            if (altProjeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "ALTPROJENO", "=", altProjeNo));

            sql += " GROUP BY " + gruplar;
            sql += " ORDER BY " + gruplar;
            return this.veriErisim.DataReaderDondur(sql);
        }

        public Sonuc YeniProjeTeklifSil(int kaynakYil, string kaynakAsama, string hedefAsama, string projeKodListe)
        {
            string sql = "DELETE FROM YATYENIPROJEBUTCE";
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", hedefAsama.Replace(".", "")));
            if (projeKodListe != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "(1 in (" + TABYATButceKaynak.KaynakProjeyeBagla(projeKodListe, "YATYENIPROJEBUTCE", kaynakYil, kaynakAsama) + "))");
            }

            return KomutCalistir(sql, 0);
        }

        public Sonuc AsamaKopyalaYeniProje(int kaynakYil, string kaynakAsama, string hedefAsama, string projeKodListe)
        {
            string sql = "INSERT INTO YATYENIPROJEBUTCE(KAYNAKNO, YIL, SENARYOKOD, EKOKOD, PROJETUTARIDIS, PROJETUTARIIC, DISKREDI0, ICKAYNAK0, DISKREDI1, ICKAYNAK1, DISKREDI2, ICKAYNAK2) ";
            sql += "SELECT KAYNAKNO, YIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, EKOKOD, PROJETUTARIDIS, PROJETUTARIIC, DISKREDI0, ICKAYNAK0, DISKREDI1, ICKAYNAK1, DISKREDI2, ICKAYNAK2 FROM " + "YATYENIPROJEBUTCE";
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            if (projeKodListe != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "(1 in (" + TABYATButceKaynak.KaynakProjeyeBagla(projeKodListe, "YATYENIPROJEBUTCE", kaynakYil, kaynakAsama) + "))");
            }

            return KomutCalistir(sql, 0);
        }
    }
}
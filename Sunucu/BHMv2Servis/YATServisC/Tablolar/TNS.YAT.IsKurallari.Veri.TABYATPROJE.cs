using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Yatırım proje bilgilerini tutan YATPROJE veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATProje : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATProje(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATPROJE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// PROJENO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PROJENO = 0,

            /// <summary>
            /// DPTPROJENO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DPTPROJENO = 1,

            /// <summary>
            /// SEKTORNO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SEKTORNO = 2,

            /// <summary>
            /// TURKODU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TURKODU = 3,

            /// <summary>
            /// AD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AD = 4,

            /// <summary>
            /// KARAKTERISTIK alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KARAKTERISTIK = 5,

            /// <summary>
            /// KARAKTERISTIKOZET alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KARAKTERISTIKOZET = 6,

            /// <summary>
            /// ETUDPROJEMI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ETUDPROJEMI = 7,

            /// <summary>
            /// BASLANGICYILI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BASLANGICYILI = 8,

            /// <summary>
            /// BITISYILI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BITISYILI = 9,

            /// <summary>
            /// MODEL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MODEL = 10,

            /// <summary>
            /// YURUTECEKKURUM alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YURUTECEKKURUM = 11,

            /// <summary>
            /// GEREKCE alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 12, AKTIF = 13, ESKIYENI = 14, CARI = 15, YIL = 16, SENARYOKOD = 17
        }

        /// <summary>
        /// Yatırım projesinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "PROJENO", "DPTPROJENO", "SEKTORNO", "TURKODU", "AD", "KARAKTERISTIK", "KARAKTERISTIKOZET",
                                   "ETUDPROJEMI", "BASLANGICYILI", "BITISYILI", "MODEL", "YURUTECEKKURUM",
                                   "GEREKCE", "AKTIF", "ESKIYENI", "CARI", "YIL", "SENARYOKOD"};

        /// <summary>
        /// Yatırım proje bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Yatırım projesinin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="proje">Yatırım proje bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string senaryoKod, Proje proje)
        {
            //int kod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJENO]);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(proje.kod) + ",";
            sql += VeriErisim.DegerYap(proje.DPTProjeNo) + ",";
            sql += VeriErisim.DegerYap(proje.sektor.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(proje.tur.kod) + ",";
            sql += VeriErisim.DegerYap(proje.ad) + ",";
            sql += VeriErisim.DegerYap(proje.karakteristik) + ",";
            sql += VeriErisim.DegerYap(proje.karakteristikOzet) + ",";
            string etudMu = "H";
            if (proje.etudProjeMi)
                etudMu = "E";
            sql += VeriErisim.DegerYap(etudMu) + ",";
            sql += VeriErisim.DegerYap(proje.baslangicYili) + ",";
            sql += VeriErisim.DegerYap(proje.bitisYili) + ",";
            sql += VeriErisim.DegerYap(proje.model) + ",";
            sql += VeriErisim.DegerYap(proje.yurutecekKurum.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(proje.gerekce) + ",";
            sql += VeriErisim.DegerYap(proje.aktifMi ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(proje.eskiYeni ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(proje.cari ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan yatırım proje bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="proje">Yatırım proje bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(int yil, string senaryoKod, Proje proje)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DPTPROJENO], proje.DPTProjeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEKTORNO], proje.sektor.kod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TURKODU], proje.tur.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], proje.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARAKTERISTIK], proje.karakteristik) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARAKTERISTIKOZET], proje.karakteristikOzet) + ",";
            string etudMu = "H";
            if (proje.etudProjeMi)
                etudMu = "E";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ETUDPROJEMI], etudMu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICYILI], proje.baslangicYili) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISYILI], proje.bitisYili) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MODEL], proje.model) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YURUTECEKKURUM], proje.yurutecekKurum.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], proje.gerekce) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], proje.aktifMi ? 1 : 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ESKIYENI], proje.eskiYeni ? 1 : 0);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], "=", proje.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = proje.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Yıl ve degerler dizisindeki kullanıcının yetkili olduğu birimleri
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve yatırım projelerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Kullanıcının yetkili olduğu birim listesi</param>
        /// <param name="kurKod">Projenin kurum kodu</param>
        /// <param name="kriter">Projenin kurum kodu</param>
        /// <returns>Verilen kriterlere uygun olan yatırım projelerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string senaryoKod, string kurKod, string kriter)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            if (!string.IsNullOrWhiteSpace(kriter))
            {
                string icSql = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + kriter + "%"));
                icSql = VeriErisim.KosulEkle(icSql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DPTPROJENO], "LIKE", "%" + kriter + "%"));
                icSql = VeriErisim.KosulEkle(icSql, "OR", $"(1 IN (SELECT 1 FROM YATALTPROJE WHERE YATALTPROJE.YIL=YATPROJE.YIL AND YATALTPROJE.SENARYOKOD=YATPROJE.SENARYOKOD AND YATALTPROJE.PROJENO=YATPROJE.PROJENO AND AD LIKE {VeriErisim.DegerYap("%" + kriter + "%")}))");
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + icSql + ")");
            }
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.DPTPROJENO] + " , " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yıl ve degerler dizisindeki kullanıcının yetkili olduğu birimleri
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve yatırım projelerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Kullanıcının yetkili olduğu birim listesi</param>
        /// <param name="kurKod">Projenin kurum kodu</param>
        /// <param name="kriter">Projenin kurum kodu</param>
        /// <returns>Verilen kriterlere uygun olan yatırım projelerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader ListeleAltProje(int yil, string senaryoKod, string kurKod, int projeKod, string kriter)
        {
            string sql = "SELECT ALTPROJENO, AD FROM YATALTPROJE";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "PROJENO", "=", projeKod));

            if (!string.IsNullOrWhiteSpace(kriter))
            {
                sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl("", "AD", "LIKE", "%" + kriter + "%"));
            }
            sql += " ORDER BY AD";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yıl ve degerler dizisindeki kullanıcının yetkili olduğu birimleri
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve yatırım projelerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="birimListesi">Kullanıcının yetkili olduğu birim listesi</param>
        /// <returns>Verilen kriterlere uygun olan yatırım projelerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string senaryoKod, string[] birimListesi, Proje proje, bool altProjelerle)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", YATSEKTOR.AD, YATPROJETUR.TURADI";
            if (altProjelerle)
                sql += "," + VeriErisim.TumAlanlar(TABYATAltProje.TABLOAD, TABYATAltProje.ALANLAR);
            sql += " FROM " + TABLOAD;
            sql += " LEFT JOIN YATSEKTOR ON " + VeriErisim.Join(TABLOAD, "YATSEKTOR", new string[] { "YIL", "SEKTORNO" }); ;
            sql += " LEFT JOIN YATPROJETUR ON " + VeriErisim.Join(TABLOAD, "YATPROJETUR", new string[] { "TURKODU" }); ;

            if (altProjelerle)
                sql += " LEFT JOIN YATALTPROJE ON " + VeriErisim.Join(TABLOAD, "YATALTPROJE", new string[] { "YIL", "SENARYOKOD", "PROJENO" });

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            if (proje.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJENO], "=", proje.kod));
            if (!string.IsNullOrWhiteSpace(proje.ad))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + proje.ad + "%"));
            if (!string.IsNullOrWhiteSpace(proje.DPTProjeNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DPTPROJENO], "LIKE", proje.DPTProjeNo + "%"));
            if (!string.IsNullOrWhiteSpace(proje.sektor.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SEKTORNO], "LIKE", proje.sektor.kod.Replace(".", "") + "%"));

            if (proje.aktiflik != 2)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AKTIF], "=", proje.aktiflik));

            if (altProjelerle)
            {
                string orluBirimler1 = TABGenel.DegerleriOrla(birimListesi, "YATALTPROJE.BIRIM", proje.kod);
                string orluBirimler2 = TABGenel.DegerleriOrla(birimListesi, TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YURUTECEKKURUM], proje.kod);
                string orluBirimler = orluBirimler1;
                if (string.IsNullOrWhiteSpace(orluBirimler1)) orluBirimler = orluBirimler2;
                else if (string.IsNullOrWhiteSpace(orluBirimler2)) orluBirimler = orluBirimler1;
                else orluBirimler = "(" + orluBirimler1 + " OR " + orluBirimler2 + ")";
                if (!string.IsNullOrWhiteSpace(orluBirimler))
                    sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);
                if (proje.iliskiliAltProje > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", "YATALTPROJE.ALTPROJENO =" + VeriErisim.DegerYap(proje.iliskiliAltProje));
            }
            else
            {
                string icSql = "";
                string orluBirimler = TABGenel.DegerleriOrla(birimListesi, "IC.BIRIM", proje.kod);
                if (!string.IsNullOrWhiteSpace(orluBirimler) || proje.iliskiliAltProje > 0)
                {
                    icSql = "SELECT 1 FROM YATALTPROJE IC WHERE " + VeriErisim.Join("IC", "YATPROJE", new string[] { "YIL", "SENARYOKOD", "PROJENO" });

                    if (proje.iliskiliAltProje > 0)
                        icSql = VeriErisim.KosulEkle(icSql, "AND", "IC.ALTPROJENO =" + VeriErisim.DegerYap(proje.iliskiliAltProje));
                    else
                        icSql = VeriErisim.KosulEkle(icSql, "AND", orluBirimler);
                    icSql = " 1 IN (" + icSql + ")";
                }
                if (proje.iliskiliAltProje == 0)
                {
                    string orluBirimler2 = TABGenel.DegerleriOrla(birimListesi, TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YURUTECEKKURUM], proje.kod);
                    if (!string.IsNullOrWhiteSpace(orluBirimler2))
                    {
                        if (!string.IsNullOrWhiteSpace(icSql))
                        {
                            icSql = "((" + icSql + ") OR (" + orluBirimler2 + "))";
                        }
                        else
                            icSql = orluBirimler2;
                    }
                }

                if (!string.IsNullOrWhiteSpace(icSql))
                    sql = VeriErisim.KosulEkle(sql, "AND", icSql);
            }
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.DPTPROJENO] + " , " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD];
            if (altProjelerle)
                sql += "," + TABYATAltProje.ALANLAR[(int)TABYATAltProje.ENUMAlanlar.ALTPROJENO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen yatırım projesi programatik kodunu kullanarak,
        /// ilgili yatırım projesini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="projeKod">Yatırım projesi programatik kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string senaryoKod, int projeKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], "=", projeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen yatırım projesi programatik kodunu kullanarak
        /// yatırım projesinin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="projeKodu">Yatırım projesi programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool ProjeVarMi(int yil, string senaryoKod, int projeKodu)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.PROJENO] };
            int[] intDeger = { yil, projeKodu };

            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.SENARYOKOD] };
            string[] strDeger = { senaryoKod.Replace(".", "") };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Verilen proje türün, yatırım projelerinden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="projeTurKodu">Proje tür kodu</param>
        /// <returns>Proje tür; yatırım projelerinde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool ProjeTurKullanilmisMi(int projeTurKodu)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.TURKODU] };
            int[] intDeger = { projeTurKodu };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Verilen sektörün, yatırım projelerinden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="sektorKodu">Sektör kodu</param>
        /// <returns>Sektör; yatırım projelerinde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool SektorKullanilmisMi(string sektorKodu)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.SEKTORNO] };
            string[] strDeger = { sektorKodu.Replace(".", "") };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, new string[0], new int[0]);
        }

        public IDataReader ListeleFaalIle(int yil, string senaryoKod, string altFaalKod)
        {
            string sql = "SELECT DISTINCT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql += " LEFT JOIN YATALTPROJE ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJENO] + "=YATALTPROJE.PROJENO AND YATPROJE.YIL=YATALTPROJE.YIL AND YATPROJE.SENARYOKOD=YATALTPROJE.SENARYOKOD";
            sql += " LEFT JOIN YATBUTCEKAYNAK ON  YATBUTCEKAYNAK.ALTPROJENO=YATALTPROJE.ALTPROJENO AND YATBUTCEKAYNAK.YIL=YATALTPROJE.YIL AND YATBUTCEKAYNAK.SENARYOKOD=YATALTPROJE.SENARYOKOD";
            sql += " LEFT JOIN YATBUTCEKAYNAKPEBKOD ON YATBUTCEKAYNAKPEBKOD.YIL=" + VeriErisim.DegerYap(yil) + " AND YATBUTCEKAYNAKPEBKOD.SENARYOKOD= " + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND YATBUTCEKAYNAKPEBKOD.KAYNAKNO=YATBUTCEKAYNAK.KAYNAKNO AND YATBUTCEKAYNAKPEBKOD.YIL=YATBUTCEKAYNAK.YIL AND YATBUTCEKAYNAKPEBKOD.SENARYOKOD=YATBUTCEKAYNAK.SENARYOKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("YATBUTCEKAYNAKPEBKOD", "PEBKOD", "LIKE", altFaalKod.Replace(".", "") + "%"));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.DPTPROJENO] + ", " + ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc ProjeKopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, string projeKodListe)
        {
            string sql = "INSERT INTO " + TABLOAD + "(YIL, SENARYOKOD, PROJENO, DPTPROJENO, SEKTORNO, TURKODU, AD, KARAKTERISTIK, KARAKTERISTIKOZET, ETUDPROJEMI, BASLANGICYILI, BITISYILI, MODEL, YURUTECEKKURUM, GEREKCE, AKTIF, ESKIYENI, CARI) ";
            sql += "SELECT " + VeriErisim.DegerYap(hedefYil) + " YIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, PROJENO, DPTPROJENO, SEKTORNO, TURKODU, AD, KARAKTERISTIK, KARAKTERISTIKOZET, ETUDPROJEMI, BASLANGICYILI, BITISYILI, MODEL, YURUTECEKKURUM, GEREKCE, AKTIF, ESKIYENI, CARI FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", "(1 NOT in (SELECT 1 FROM " + TABLOAD + $" IC WHERE IC.YIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.PROJENO ={TABLOAD}.PROJENO))");

            if (projeKodListe != null)
                sql = VeriErisim.KosulEkle(sql, "AND", "(PROJENO in (" + projeKodListe + "))");

            return KomutCalistir(sql, 0);
        }

        public static string OdenekListesiSql(int projeNo, int altProjeNo, int butceYil, string senaryoKod, int teklifYil, string finKod, int eko2Uz, bool ekOdenek = false, bool finkodluListe = false)
        {
            string alanAd = "O.ODENEK";
            string tabloAd;
            if (ekOdenek && teklifYil != 0)
                alanAd += teklifYil.ToString();
            if (ekOdenek)
                tabloAd = "ButceHazirlikV2.dbo.BHMTAVANUSTU";
            else
                tabloAd = "ButceHazirlikV2.dbo.BHMODENEK";

            string finAlan = (finkodluListe ? "O.FINKOD, " : "");
            string alanlar = $"LEFT(O.EKOKOD, {eko2Uz}), {finAlan} E.EKOAD";

            string sql = $"SELECT {alanlar}, SUM({alanAd}) FROM Yatirim.dbo.YATALTPROJE AS AP";
            sql += " INNER JOIN Yatirim.dbo.YATBUTCEKAYNAK AS BK ON " + VeriErisim.Join("AP", "BK", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " INNER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS KP ON " + VeriErisim.Join("BK", "KP", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += $" INNER JOIN {tabloAd} AS O ON " + VeriErisim.Join("KP", "O", new string[] { "YIL", "SENARYOKOD", "PEBKOD" });
            sql += $" INNER JOIN ButceHazirlik.dbo.ABSEKO AS E ON O.YIL = E.YIL AND LEFT(O.EKOKOD, {eko2Uz}) = E.EKOKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("AP", "YIL", "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            if (projeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "PROJENO", "=", projeNo));
            if (altProjeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "ALTPROJENO", "=", altProjeNo));

            if (!ekOdenek)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "TEKLIFYIL", "=", teklifYil));

            if (!string.IsNullOrEmpty(finKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "FINKOD", "=", finKod));
            sql += $" GROUP BY {alanlar}";
            sql += " ORDER BY 1";
            if (finkodluListe)
                sql += ", 2";
            return sql;
        }

        public static string OdenekListesiSqlTam(int projeNo, int altProjeNo, int butceYil, string senaryoKod, int teklifYil)
        {
            string sql = "SELECT O.KURKOD, O.FONKOD, O.FINKOD, O.EKOKOD, E.EKOAD, SUM(O.ODENEK) FROM Yatirim.dbo.YATALTPROJE AS AP";
            sql += " INNER JOIN Yatirim.dbo.YATBUTCEKAYNAK AS BK ON " + VeriErisim.Join("AP", "BK", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " INNER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS KP ON " + VeriErisim.Join("BK", "KP", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += " INNER JOIN ButceHazirlikV2.dbo.BHMODENEK AS O ON " + VeriErisim.Join("KP", "O", new string[] { "YIL", "SENARYOKOD", "PEBKOD" });
            sql += " INNER JOIN ButceHazirlik.dbo.ABSEKO AS E ON O.YIL = E.YIL AND O.EKOKOD = E.EKOKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("AP", "YIL", "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            if (projeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "PROJENO", "=", projeNo));
            if (altProjeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "ALTPROJENO", "=", altProjeNo));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "TEKLIFYIL", "=", teklifYil));

            sql += " GROUP BY O.KURKOD, O.FONKOD, O.FINKOD, O.EKOKOD, E.EKOAD";
            sql += " ORDER BY 1,2,3,4";
            return sql;
        }

        public static string OdenekListesiYeniSql(int projeNo, int altProjeNo, int butceYil, string senaryoKod, int teklifYil, int eko2Uz)
        {
            string sql = $"SELECT LEFT(O.EKOKOD, {eko2Uz}), E.EKOAD, SUM(O.DISKREDI" + teklifYil.ToString() + "), SUM(O.ICKAYNAK" + teklifYil.ToString() + ") FROM Yatirim.dbo.YATALTPROJE AS AP";
            sql += " INNER JOIN Yatirim.dbo.YATBUTCEKAYNAK AS BK ON " + VeriErisim.Join("AP", "BK", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " INNER JOIN Yatirim.dbo.YATYENIPROJEBUTCE AS O ON " + VeriErisim.Join("BK", "O", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += $" INNER JOIN ButceHazirlik.dbo.ABSEKO AS E ON O.YIL = E.YIL AND LEFT(O.EKOKOD, {eko2Uz}) = E.EKOKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("AP", "YIL", "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            if (projeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "PROJENO", "=", projeNo));
            if (altProjeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "ALTPROJENO", "=", altProjeNo));
            sql += $" GROUP BY LEFT(O.EKOKOD, {eko2Uz}), E.EKOAD";
            sql += " ORDER BY 1";
            return sql;
        }

        public static string OdenekListesiYeniSqlTam(int projeNo, int altProjeNo, int butceYil, string senaryoKod, string finKod)
        {
            string alan = "";
            if (finKod == "01") alan = "ICKAYNAK";
            else alan = "DISKREDI";

            string sql = "SELECT AP.BIRIM, '000000' FONKOD, " + VeriErisim.DegerYap(finKod) + " FINKOD, O.EKOKOD, E.EKOAD, SUM(O." + alan + "0), SUM(O." + alan + "1), SUM(O." + alan + "2) FROM Yatirim.dbo.YATALTPROJE AS AP";
            sql += " INNER JOIN Yatirim.dbo.YATBUTCEKAYNAK AS BK ON " + VeriErisim.Join("AP", "BK", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " INNER JOIN Yatirim.dbo.YATYENIPROJEBUTCE AS O ON " + VeriErisim.Join("BK", "O", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += " INNER JOIN ButceHazirlik.dbo.ABSEKO AS E ON O.YIL = E.YIL AND  O.EKOKOD = E.EKOKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("AP", "YIL", "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            if (projeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "PROJENO", "=", projeNo));
            if (altProjeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "ALTPROJENO", "=", altProjeNo));
            sql += " GROUP BY AP.BIRIM, O.EKOKOD, E.EKOAD";
            sql += " ORDER BY 1,2,3,4";
            return sql;
        }

        public static string SektorBulSql(int yil, string senaryoKod, string faaliyetKod)
        {
            string sorgu = "SELECT SEKTORNO FROM Yatirim.dbo.YATPROJE AS P";
            sorgu += " LEFT OUTER JOIN Yatirim.dbo.YATALTPROJE AS AP ON " + VeriErisim.Join("P", "AP", new string[] { "YIL", "SENARYOKOD", "PROJENO" });
            sorgu += " LEFT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("AP", "K", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sorgu += " RIGHT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sorgu = VeriErisim.KosulEkle(sorgu, "WHERE", VeriErisim.KosulOl("BK", "YIL", "=", yil));
            sorgu = VeriErisim.KosulEkle(sorgu, "AND", VeriErisim.KosulOl("BK", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            sorgu = VeriErisim.KosulEkle(sorgu, "AND", VeriErisim.KosulOl("BK", "PEBKOD", "=", faaliyetKod.Replace(".", "")));
            return sorgu;
        }

        /// <summary>
        /// Yıl ve degerler dizisindeki kullanıcının yetkili olduğu birimleri
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve yatırım projelerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="birimListesi">Kullanıcının yetkili olduğu birim listesi</param>
        /// <returns>Verilen kriterlere uygun olan yatırım projelerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader ListeleTum(int yil, string senaryoKod, string[] birimListesi, Proje proje, bool tekYil, bool eskiYillarKBO, int ekoUz, out string sqlDonus)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", YATSEKTOR.AD, YATPROJETUR.TURADI, ABSKUR.KURAD";
            sql += "," + IcSorguOl(TABYATAltProje.Listele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJENO], yil, senaryoKod, tekYil, eskiYillarKBO, ekoUz)) + " XML_ALTPROJELER";
            sql += "," + IcSorguOl(TABYATALTPROJEBILGI.Listele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJENO], null)) + " XML_BILGILER";
            sql += "," + IcSorguOl(TABYATIlIlce.Listele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJENO], null)) + " XML_ILCELER";
            sql += "," + IcSorguOl(TABYATPROJEKALKINMABOLGE.Listele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJENO], null)) + " XML_BOLGELER";
            sql += " FROM " + TABLOAD;
            sql += " LEFT JOIN YATSEKTOR ON " + VeriErisim.Join(TABLOAD, "YATSEKTOR", new string[] { "YIL", "SEKTORNO" }); ;
            sql += " LEFT JOIN YATPROJETUR ON " + VeriErisim.Join(TABLOAD, "YATPROJETUR", new string[] { "TURKODU" }); ;
            sql += " LEFT JOIN ButceHazirlik.dbo.ABSKUR ABSKUR ON " + VeriErisim.Join(TABLOAD, "ABSKUR", new string[] { "YIL", "YURUTECEKKURUM" }, new string[] { "YIL", "KURKOD" });

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", Math.Abs(yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            if (proje.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJENO], "=", proje.kod));
            if (!string.IsNullOrWhiteSpace(proje.ad))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + proje.ad + "%"));
            if (!string.IsNullOrWhiteSpace(proje.DPTProjeNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DPTPROJENO], "LIKE", proje.DPTProjeNo + "%"));
            if (!string.IsNullOrWhiteSpace(proje.sektor.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SEKTORNO], "LIKE", proje.sektor.kod.Replace(".", "") + "%"));
            if (proje.aktiflik != 2)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABYATProje.TABLOAD, TABYATProje.ALANLAR[(int)TABYATProje.ENUMAlanlar.AKTIF], "=", proje.aktiflik));

            string icSql = "";
            string orluBirimler = TABGenel.DegerleriOrla(birimListesi, "IC.BIRIM", proje.kod);
            if (!string.IsNullOrWhiteSpace(orluBirimler) || proje.iliskiliAltProje > 0)
            {
                icSql = "SELECT 1 FROM YATALTPROJE IC WHERE " + VeriErisim.Join("IC", "YATPROJE", new string[] { "YIL", "SENARYOKOD", "PROJENO" });

                if (proje.iliskiliAltProje > 0)
                    icSql = VeriErisim.KosulEkle(icSql, "AND", "IC.ALTPROJENO =" + VeriErisim.DegerYap(proje.iliskiliAltProje));
                else
                    icSql = VeriErisim.KosulEkle(icSql, "AND", orluBirimler);
                icSql = " 1 IN (" + icSql + ")";
            }
            if (proje.iliskiliAltProje == 0)
            {
                string orluBirimler2 = TABGenel.DegerleriOrla(birimListesi, TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YURUTECEKKURUM], proje.kod);
                if (!string.IsNullOrWhiteSpace(orluBirimler2))
                {
                    if (!string.IsNullOrWhiteSpace(icSql))
                    {
                        icSql = "((" + icSql + ") OR (" + orluBirimler2 + "))";
                    }
                    else
                        icSql = orluBirimler2;
                }
            }

            if (!string.IsNullOrWhiteSpace(icSql))
                sql = VeriErisim.KosulEkle(sql, "AND", icSql);
            sql += " ORDER BY ";
            if (proje.siralama == 2)
                sql += ALANLAR[(int)ENUMAlanlar.SEKTORNO] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.DPTPROJENO] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD];
            sqlDonus = sql;
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
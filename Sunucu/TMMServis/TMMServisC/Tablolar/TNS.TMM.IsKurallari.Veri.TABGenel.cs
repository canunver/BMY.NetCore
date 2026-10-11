using System;
using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır tabloları ile ilgili genel kullanılan (Sayaç Değer Al, Kayıt Var Mı, Belge No al) fonksiyonları tutan sınıf
    /// </summary>
    public class TABGenel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGenel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Parametre olarak Verilen Veri Erişim ve Amortisman Kriter bilgisini kullanarak
        /// Amortisman Başlama yılını döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisini tutan, VeriErisim türünde nesne</param>
        /// <param name="ak">Amortisman Kriter bilgisini tutan, AmortismanKriter türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Amortisman Başlangıç yılını tutan Int türünde nesne döndürür</returns>
        public static int AmortismanBaslamaYilDondur(VeriErisim veriErisim, AmortismanKriter ak)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", null);
            TABAmortisman ta = new TABAmortisman(ve);

            int yil = ta.AmortismanBaslamaYilDondur(ak);

            ve.VeriErisimKapat();
            return yil;
        }

        /// <summary>
        /// Parametre olarak verilen Yetki Değeri ve Tablo Adına göre,
        /// Yetki Değerine göre kontrolü sağlayacak Sql koşulunu oluşturan fonksiyon
        /// </summary>
        /// <param name="yetkiDeger">Yetki Değer bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="tabloAd">Tablo adı bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Yetki değerlerine göre oluşturulan sql koşul ifadesini tutan String türünde nesne döndürür</returns>
        public static string YetkiDegerOrla(string[] yetkiDeger, string tabloAd)
        {
            string donusDeger = string.Empty;
            bool orlandiMi = false;
            foreach (string s in yetkiDeger)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    string[] parcaliYetki = s.Split('-');
                    if (parcaliYetki.Length == 3)
                    {
                        string ekleme = string.Empty;
                        if (parcaliYetki[0].Replace(".", "") != "*")
                        {
                            if (parcaliYetki[0].Contains("*"))
                            {
                                if (!string.IsNullOrEmpty(tabloAd))
                                    ekleme += tabloAd + "." + "MUHASEBEKOD" + " LIKE '" + parcaliYetki[0].Replace(".", "").Substring(0, parcaliYetki[0].Length - 1) + "%'";
                                else
                                    ekleme += "MUHASEBEKOD" + " LIKE '" + parcaliYetki[0].Replace(".", "").Substring(0, parcaliYetki[0].Length - 1) + "%'";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(tabloAd))
                                    ekleme += tabloAd + "." + "MUHASEBEKOD" + "='" + parcaliYetki[0].Replace(".", "") + "'";
                                else
                                    ekleme += "MUHASEBEKOD" + "='" + parcaliYetki[0].Replace(".", "") + "'";
                            }
                        }

                        if (parcaliYetki[1].Replace(".", "") != "*")
                        {
                            if (!string.IsNullOrEmpty(ekleme))
                                ekleme += " AND ";

                            if (!string.IsNullOrEmpty(tabloAd))
                                ekleme += tabloAd + "." + "HARCAMABIRIMKOD" + "='" + parcaliYetki[1].Replace(".", "") + "'";
                            else
                                ekleme += "HARCAMABIRIMKOD" + "='" + parcaliYetki[1].Replace(".", "") + "'";
                        }

                        if (parcaliYetki[2].Replace(".", "") != "*")
                        {
                            if (!string.IsNullOrEmpty(ekleme))
                                ekleme += " AND ";

                            if (!string.IsNullOrEmpty(tabloAd))
                                ekleme += tabloAd + "." + "AMBARKOD" + "='" + parcaliYetki[2].Replace(".", "") + "'";
                            else
                                ekleme += "AMBARKOD" + "='" + parcaliYetki[2].Replace(".", "") + "'";
                        }

                        if (!string.IsNullOrEmpty(donusDeger) && !string.IsNullOrEmpty(ekleme))
                        {
                            orlandiMi = true;
                            donusDeger += " OR ";
                        }

                        if (!string.IsNullOrEmpty(ekleme))
                            donusDeger += "(" + ekleme + ")";
                    }
                }
            }

            if (orlandiMi)
                donusDeger = "(" + donusDeger + ")";

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Tablo adı ve Alan bilgisine göre,
        /// Sayaç değerini SayacDegerAl rutinini çalıştırarak döndüren fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim bilgisini tutan, VeriErisim türünde nesne</param>
        /// <param name="tablo">Tablo adı bilgisini tutan, String türünde nesne</param>
        /// <param name="alan">Tablo alanının adını tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Sayaç değerini tutan Int türünde nesne döndürür</returns>
        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan)
        {
            return SayacDegerAl(ve, tablo, alan, new string[0], new string[0], new string[0], new int[0]);
        }

        /// <summary>
        /// Parametre olarak verilen Tablo adı, Alan Adı ve Koşullara göre, 
        /// İlgili alanın en yüksek değerini(Sayaç bilgisi) döndüren,
        /// sql komutunun oluşturup çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim bilgisini tutan, VeriErisim türünde nesne</param>
        /// <param name="tablo">Tablo adı bilgisini tutan, String türünde nesne</param>
        /// <param name="alan">Alan adı bilgisini tutan, String türünde nesne</param>
        /// <param name="whereStrAlan">Koşulun uygulanacağı string türünde alan bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="whereStrDeger">Koşulda kullanılacak string türünde değer bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="whereIntAlan">Koşulun uygulanacağı int türünde alan bilgisini tutan, Int dizi türünde nesne</param>
        /// <param name="whereIntDeger">Koşulda kullanılacak int türünde değer bilgisini tutan, Int dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun en yüksek değeri tutan (Sayaç) Int türünde nesne döndürür</returns>
        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT MAX(CAST(" + alan + " AS INT)) FROM " + tablo;

            for (int i = 0; i < whereStrAlan.Length; i++)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereStrAlan[i], "=", whereStrDeger[i]));

            for (int i = 0; i < whereIntAlan.Length; i++)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereIntAlan[i], "=", whereIntDeger[i]));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return OrtakFonksiyonlar.ConvertToInt(ve.TekKolonSQLDbl(sql).ToString(), 0) + 1;
        }

        /// <summary>
        /// Parametre olarak gönderilen tabloda verilen kriterlere uygun kayıt var mı kontrolü yapan fonksiyon
        /// </summary>
        /// <param name="ve">Veri tabanı erişiminde kullanılacak nesne</param>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="whereStrAlan">String tipinde veri tutan alan adları</param>
        /// <param name="whereStrDeger">String tipinde veri tutan alanlara eşitlenecek değerler</param>
        /// <param name="whereIntAlan">Sayı tipinde veri tutan alan adları</param>
        /// <param name="whereIntDeger">Sayı tipinde veri tutan alanlara eşitlenecek değerler</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public static bool KayitVarmi(VeriErisim ve, string tablo, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT COUNT(*) FROM " + tablo;
            for (int i = 0; i < whereStrAlan.Length; i++)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereStrAlan[i], "=", whereStrDeger[i]));

            for (int i = 0; i < whereIntAlan.Length; i++)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereIntAlan[i], "=", whereIntDeger[i]));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return ve.TekKolonSQLDbl(sql) != 0;
        }

        /// <summary>
        /// Parametre olarak verilen Sayaç Kodu ve Sayaç Formatına göre,
        /// Belge Numarasını döndüren fonksiyon
        /// </summary>
        /// <param name="sayacKod">Sayaç kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="sayacFormat">Sayaç format bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Belge numarası bilgisini tutan String türünde nesne döndürür</returns>
        public string BelgeNoAl(string sayacKod, string sayacFormat, string ilkDeger = "0")
        {
            string sayacDeger = "";
            string sql = "";

            veriErisim.BeginTransaction();
            sql = "SELECT SAYAC FROM SAYAC ";
            if (!(veriErisim.baglantiTip == "ORACLE"))
                sql += " WITH(XLOCK,ROWLOCK) ";
            sql += " WHERE " + VeriErisim.KosulOl("", "KOD", "=", sayacKod);

            IDataReader r = veriErisim.DataReaderDondur(sql);

            if (veriErisim.hata == "")
            {
                if (r.Read())
                    sayacDeger = VeriErisim.DegerAl(r, 0, "");
                r.Close();

                bool ilkKayit = false;
                if (sayacDeger == "")
                {
                    ilkKayit = true;
                    if (ilkDeger != "0")
                        sayacDeger = ilkDeger;
                    else
                        sayacDeger = "0";
                }

                sayacDeger = (Convert.ToInt32(sayacDeger) + 1).ToString();
                sayacDeger = sayacDeger.PadLeft(sayacFormat.Length, sayacFormat[0]);

                if (ilkKayit)
                    sql = "INSERT INTO SAYAC (KOD,SAYAC) VALUES (" + VeriErisim.DegerYap(sayacKod) + "," + VeriErisim.DegerYap(sayacDeger) + ")";
                else
                    sql = "UPDATE SAYAC SET SAYAC=" + VeriErisim.DegerYap(sayacDeger) + " WHERE KOD=" + VeriErisim.DegerYap(sayacKod);

                if (KomutCalistir(sql, 0).islemSonuc)
                {
                    veriErisim.Commit();
                    return sayacDeger;
                }
            }

            veriErisim.Rollback();
            return "";
        }

        /// <summary>
        /// Parametre olarak verilen Sayaç koduna göre
        /// ilgili sayaç dosyasını sistemde oluşturan fonksiyon
        /// </summary>
        /// <param name="sayacKod">Sayaç kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer sayaç dosyası oluşturulmuşsa true oluşturulmamışsa false değerini tutan Bool türünde nesne döndürür</returns>
        bool SayacDosyaYarat(string sayacKod)
        {
            if (System.IO.File.Exists(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod))
                return true;

            try
            {
                System.IO.FileStream fs = System.IO.File.Create(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod);
                fs.Close();
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Parametre olarak verilen Sayaç koduna göre
        /// ilgili sayaç dosyasını sistemden silen fonksiyon
        /// </summary>
        /// <param name="sayacKod">Sayaç kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer sayaç dosyası silinmişse true silinememişse false değerini tutan Bool türünde nesne döndürür</returns>
        bool SayacDosyaSil(string sayacKod)
        {
            try
            {
                System.IO.File.Delete(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parametre olarak verilen Form durumunun,
        /// Durum numarasını döndüren fonksiyon
        /// </summary>
        /// <param name="yeniDurum">Form durumunu tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, form durum numarasını tutan Int türünde nesne döndürür</returns>
        public static int FormDurumNoVer(string yeniDurum)
        {
            return yeniDurum == "Onay" ? (int)ENUMBelgeDurumu.ONAYLI :
            yeniDurum == "İptal" ? (int)ENUMBelgeDurumu.IPTAL :
            yeniDurum == "OnayKaldir" ? (int)ENUMBelgeDurumu.ONAYKALDIR :
            (int)ENUMBelgeDurumu.TANIMSIZ;
        }

        /// <summary>
        /// Parametre olarak verilen SQL cümlesini çalıştırıp,
        /// çalışmanın sonucunda elde edilen değerleri döndüren fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda, sql komutunun sonuçlarını tutan IDataReader türünde nesne</returns>
        public IDataReader SQLCalistir(string sql)
        {
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen SQL cümlesini çalıştırıp,
        /// çalışmanın sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sql">Sql cümlesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc SQLCalistirKomut(string sql)
        {
            return KomutCalistir(sql, 0, 0);
        }

        public static string GetPaginatedSQL(int sayfaNo, int kayitSayisi, string sql, string siralamaCumlesi)
        {
            // siralamaCumlesi is zorunlu!
            if (string.IsNullOrEmpty(siralamaCumlesi))
                throw new ArgumentNullException("Sıralama cümlesi boş bırakılmış. Ör: ORDER BY TABLOADI.KOLONADI ASC şeklinde belirtilmeli");

            // kayıtSayisi <=0 ise sayfalama yapılmayacak
            if (kayitSayisi <= 0)
            {
                return string.Format("{0} {1}", sql, siralamaCumlesi);
            }

            // sql içinde gelen SELECT cümlesi atılıyor.
            //String partialSQL = sql.Remove(0, "SELECT ".Length);

            //ilk from terimini bul ve öncesine ROWID yerleştir
            int fromYer = sql.LastIndexOf(" FROM ");
            string partialSQL = sql.Substring(0, fromYer) + ", COUNT(*) OVER() AS SatirSayisi, ROW_NUMBER() OVER (" + siralamaCumlesi + ") AS rowID " + sql.Substring(fromYer + 1);


            // Build the limited query...
            return String.Format(
                "SELECT * FROM ( {0} ) AS SUB WHERE rowID > {1} AND rowID <= {2}",
                partialSQL,
                ((sayfaNo - 1) * kayitSayisi).ToString(),
                (sayfaNo * kayitSayisi).ToString()
            );
        }

    }
}

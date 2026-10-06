using TNS.BUM;
using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// Modül içerisinde bulunan yordamlarında kullanılan sdql cümlelerinde kullanılacak ortak koşullar için koşul satırları üreten nesne
    /// </summary>
    public class GenelKosul
    {
        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="tabloAd">Tablo adı.</param>
        /// <param name="alanAd">Alan adı.</param>
        /// <param name="islec">İşleç.</param>
        /// <param name="kurumKod">Kurum kodu.</param>
        /// <returns></returns>
        public static string KurumKosulOl(string tabloAd, string alanAd, string islec, string kurumKod)
        {
            string kosul; ;

            if (kurumKod.Length == 2)
            {
                kosul = VeriErisim.KosulOl(tabloAd, alanAd, ">=", kurumKod);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, alanAd, "<", kurumKod + TNS.BKD.Arac.BagimsizKurumBaslangic()));
                kosul = "(" + kosul + ")";
            }
            else if (kurumKod.IndexOf("/") >= 0)
            {
                kosul = "";
                string parca1 = "", parca2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(kurumKod, "/", ref parca1, ref parca2);

                kosul = VeriErisim.KosulOl(tabloAd, alanAd, ">=", parca1.Replace(".", ""));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, alanAd, "<", parca2.Replace(".", "")));
                kosul = "(" + kosul + ")";
            }
            else
                kosul = VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", kurumKod.Replace(".", "") + "%");
            return kosul;
        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="tabloAd">Tablo adı.</param>
        /// <param name="alanAd">Alan adı.</param>
        /// <param name="islec">İşleç.</param>
        /// <param name="kod">Bütçe kodu.</param>
        /// <returns></returns>
        public static string KodKosulOl(string tabloAd, string alanAd, string islec, string kod)
        {
            string kosul = "";

            if (kod.IndexOf("/") >= 0)
            {
                string parca1 = "", parca2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(kod, "/", ref parca1, ref parca2);

                kosul = VeriErisim.KosulOl(tabloAd, alanAd, ">=", parca1.Replace(".", ""));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, alanAd, "<", parca2.Replace(".", "")));
                kosul = "(" + kosul + ")";
            }
            else if (kod.IndexOf(";") >= 0)
            {
                string[] kodlar = kod.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", kodlar[i].Replace(".", "") + "%"));

                kosul = "(" + kosul + ")";
            }
            else
                kosul = VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", kod.Replace(".", "") + "%");
            return kosul;
        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="tabloAd">Tablo adı.</param>
        /// <param name="alanAd">Alan adı.</param>
        /// <returns></returns>
        public static string IslemAlanOl(string tabloAd, string alanAd)
        {
            string donusDeger;
            if (tabloAd == null || tabloAd == "")
                donusDeger = alanAd;
            else
                donusDeger = tabloAd + "." + alanAd;

            return VeriErisim.IslecBul("SUBSTRING") + "(" + donusDeger + ",1,4)";
        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="tabloAd">Tablo adı.</param>
        /// <param name="alanAd">Alan adı.</param>
        /// <param name="kosul">Koşul.</param>
        /// <returns></returns>
        public static string IslemKosulOl(string tabloAd, string alanAd, string kosul)
        {
            if (kosul.Length == 2)
                return VeriErisim.KosulOl("", IslemAlanOl(tabloAd, alanAd), "LIKE", kosul.Replace(".", "") + "%");
            else
                return VeriErisim.KosulOl("", IslemAlanOl(tabloAd, alanAd), "=", kosul.Replace(".", "").Substring(0, 4));
            //islemKod.Replace(".","").Substring(0,4) + "%"
            //            return VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.ISLEM],"=",islem.Replace(".",""))
            //            string donusDeger;
            //            if(tabloAd==null || tabloAd=="")
            //                donusDeger=alanAd;
            //            else
            //                donusDeger=tabloAd+"."+alanAd;
            //            return donusDeger;
        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="asama">Aşama.</param>
        /// <param name="ekKosul">Ek koşul.</param>
        /// <param name="kistasKodlar">Kıstas kodları.</param>
        /// <param name="cokYil">Eğer <c>true</c> ise çok yıllı işlem yapılsın.</param>
        /// <returns></returns>
        public static string KistasKosulOl(int yil, int asama, string ekKosul, string[] kistasKodlar, bool cokYil)
        {
            string kosul = "";

            if (yil != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "YIL", "=", yil));

            if (asama != -1)
            {
                if (cokYil)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("", "ASAMA", "=", asama));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", "ASAMA", "=", asama + 10));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", "ASAMA", "=", asama + 20)) + ")";
                }
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "ASAMA", "=", asama));
            }

            if (kistasKodlar.Length >= 1 && (string)kistasKodlar[0] != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", KurumKosulOl("", "KURKOD", "=", (string)kistasKodlar[0]));
            }

            if (kistasKodlar.Length >= 2 && (string)kistasKodlar[1] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "FONKOD", "LIKE", ((string)kistasKodlar[1]).Replace(".", "")));
            if (kistasKodlar.Length >= 3 && (string)kistasKodlar[2] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "FINKOD", "LIKE", ((string)kistasKodlar[2]).Replace(".", "")));
            if (kistasKodlar.Length >= 4 && (string)kistasKodlar[3] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "EKOKOD", "LIKE", ((string)kistasKodlar[3]).Replace(".", "")));
            if (ekKosul != "")
            {
                if (kosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", ekKosul);
                else
                    kosul = ekKosul;
            }

            return kosul;
        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="kistasKodlar">Kıstas kodları.</param>
        /// <returns></returns>
        public static string EkKosulOlustur(string[] kistasKodlar)
        {
            string kosul = "";
            if (kistasKodlar.Length >= 1 && (string)kistasKodlar[0] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KurumKosulOl("", "KURKOD", "=", (string)kistasKodlar[0]));
            if (kistasKodlar.Length >= 2 && (string)kistasKodlar[1] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "FONKOD", "LIKE", ((string)kistasKodlar[1]).Replace(".", "")));
            if (kistasKodlar.Length >= 3 && (string)kistasKodlar[2] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "FINKOD", "LIKE", ((string)kistasKodlar[2]).Replace(".", "")));
            if (kistasKodlar.Length >= 4 && (string)kistasKodlar[3] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "EKOKOD", "LIKE", ((string)kistasKodlar[3]).Replace(".", "")));
            return kosul;

        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="kistasKodlar">Kıstas kodları.</param>
        /// <returns></returns>
        public static string EkKosulOlusturGelir(string[] kistasKodlar)
        {
            string kosul = "";
            if (kistasKodlar.Length >= 1 && (string)kistasKodlar[0] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KurumKosulOl("", "KURKOD", "=", (string)kistasKodlar[0]));
            if (kistasKodlar.Length >= 2 && (string)kistasKodlar[1] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "EKOKOD", "LIKE", ((string)kistasKodlar[1]).Replace(".", "")));
            return kosul;

        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="asama">Aşama.</param>
        /// <param name="ekKosul">Ek koşul.</param>
        /// <param name="kistasKodlar">Kıstas kodları.</param>
        /// <param name="asamami">Eğer <c>false</c> ise asama olarak gönderilen değer ay olarak kullanılsın.</param>
        /// <param name="tabload">Tablo adı.</param>
        /// <returns></returns>
        public static string KistasKosulOl(int yil, int asama, string ekKosul, string[] kistasKodlar, bool asamami, string tabload)
        {
            string kosul = "";

            if (yil != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabload, "YIL", "=", yil));

            if (asama != -1)
            {
                if (asamami)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabload, "ASAMA", "=", asama));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabload, "AY", "=", asama));

            }

            if (kistasKodlar.Length >= 1 && !string.IsNullOrWhiteSpace((string)kistasKodlar[0]))
            {
                string[] t = ((string)kistasKodlar[0]).Split(';');
                string tKosul = "";
                for (int i = 0; i < t.Length; i++)
                {
                    tKosul = VeriErisim.KosulEkle(tKosul, "OR", KurumKosulOl(tabload, "KURKOD", "LIKE", t[i].Replace(".", "")));
                }
                kosul += " AND (" + tKosul + ")";
            }
            if (kistasKodlar.Length >= 2 && !string.IsNullOrEmpty(kistasKodlar[1]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl(tabload, "FONKOD", "LIKE", ((string)kistasKodlar[1]).Replace(".", "")));
            if (kistasKodlar.Length >= 3 && !string.IsNullOrEmpty(kistasKodlar[2]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl(tabload, "FINKOD", "LIKE", ((string)kistasKodlar[2]).Replace(".", "")));
            if (kistasKodlar.Length >= 4 && !string.IsNullOrEmpty(kistasKodlar[3]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl(tabload, "EKOKOD", "LIKE", ((string)kistasKodlar[3]).Replace(".", "")));
            if (kistasKodlar.Length >= 5 && !string.IsNullOrEmpty(kistasKodlar[4]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl(tabload, "PEBKOD", "LIKE", ((string)kistasKodlar[4]).Replace(".", "")));
            if (ekKosul != "")
            {
                if (kosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", ekKosul);
                else
                    kosul = ekKosul;
            }

            return kosul;
        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="onKosul">Ön koşul.</param>
        /// <param name="baglac">Baglaç.</param>
        /// <param name="formDurumGrup">Form durumu.</param>
        /// <param name="tabloAd">Tablo adı.</param>
        /// <param name="alanAd">Alan adı.</param>
        /// <returns></returns>
        public static string FormDurumKosulOl(string onKosul, string baglac, ENUMFormDurumGrup formDurumGrup, string tabloAd, string alanAd)
        {
            if (formDurumGrup == ENUMFormDurumGrup.YENI)
                onKosul = VeriErisim.KosulEkle(onKosul, baglac, VeriErisim.KosulOl(tabloAd, alanAd, "<=", (int)ENUMFormDurum.YENIKAYIT));
            else if (formDurumGrup == ENUMFormDurumGrup.YENI_ONAYLI)
                onKosul = VeriErisim.KosulEkle(onKosul, baglac, VeriErisim.KosulOl(tabloAd, alanAd, "<=", (int)ENUMFormDurum.ONAYLI));
            else if (formDurumGrup == ENUMFormDurumGrup.ONAYLI)
            {
                onKosul = VeriErisim.KosulEkle(onKosul, baglac, VeriErisim.KosulOl(tabloAd, alanAd, ">=", (int)ENUMFormDurum.DYDOGRU));
                onKosul = VeriErisim.KosulEkle(onKosul, baglac, VeriErisim.KosulOl(tabloAd, alanAd, "<=", (int)ENUMFormDurum.ONAYLI));
            }
            else if (formDurumGrup == ENUMFormDurumGrup.ONAYLI)
            {
                onKosul = VeriErisim.KosulEkle(onKosul, baglac, VeriErisim.KosulOl(tabloAd, alanAd, ">=", (int)ENUMFormDurum.SAYISTAYRED));
            }
            return onKosul;
        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="ekKosul">Ek koşul.</param>
        /// <param name="kistasKodlar">Kıstas kodları.</param>
        /// <returns></returns>
        public static string KistasKosulOlGelir(int yil, int ay, string ekKosul, string[] kistasKodlar)
        {
            return KistasKosulOlGelir(yil, ay, ekKosul, kistasKodlar, false);
        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="ekKosul">Ek koşul.</param>
        /// <param name="kistasKodlar">Kistas kodları.</param>
        /// <param name="asamaMi">Eğer <c>true</c> ise ay parametresini aşama değeri olarak kullan.</param>
        /// <returns></returns>
        public static string KistasKosulOlGelir(int yil, int ay, string ekKosul, string[] kistasKodlar, bool asamaMi)
        {
            string kosul = "";

            if (yil != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "YIL", "=", yil));
            if (ay > 0)
                if (!asamaMi)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "AY", "=", ay));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "ASAMA", "=", ay));

            if (kistasKodlar.Length >= 1 && (string)kistasKodlar[0] != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", KurumKosulOl("", "KURKOD", "=", (string)kistasKodlar[0]));
            }

            if (kistasKodlar.Length >= 2 && (string)kistasKodlar[1] != "")
                if (kistasKodlar.Length >= 3 && (string)kistasKodlar[2] != "")
                {
                    kosul = kosul + "AND(";
                    kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl("", "EKOKOD", "LIKE", ((string)kistasKodlar[1]).Replace(".", "") + "%"));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", "EKOKOD", "LIKE", ((string)kistasKodlar[2]).Replace(".", "") + "%"));
                    kosul = kosul + ")";
                }
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "EKOKOD", "LIKE", ((string)kistasKodlar[1]).Replace(".", "") + "%"));


            if (ekKosul != "")
            {
                if (kosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", ekKosul);
                else
                    kosul = ekKosul;
            }

            return kosul;
        }

        /// <summary>
        /// Verilen parametre değerini inceleyip sql cümlesinde kullanılacak koşulu oluşturur
        /// </summary>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="alanAd">Alan adı.</param>
        /// <returns></returns>
        public static string HesapKoduEkKosulOlustur(string kodlar, string alanAd)
        {
            string kosul = "";
            ObjectArray d = new ObjectArray();
            int parca = OrtakFonksiyonlar.StringiParcala(kodlar, ";", d);

            foreach (string yetkiTipiDegeriParca in d.objeler)
            {
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", alanAd, "LIKE", yetkiTipiDegeriParca.Replace(".", "") + "%"));
            }
            kosul = "(" + kosul + ")";
            return kosul;
        }
    }
}

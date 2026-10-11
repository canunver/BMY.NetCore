using System;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;
using TNS.KYM;
using TNS.BKD;
using TNS.BKD.IsKurallari.Veri;
using System.Data;
using System.Collections;

namespace TNS.BKD.IsKurallari
{
    class ButceKodlariListesi
    {
        private static TNSCollection butceKodlari = new TNSCollection();
        private static TNSCollection yillar = new TNSCollection();

        private static void TumKodlariOku(int yil)
        {
            int kodNo = 1;
            while (true)
            {
                string yol = "/KODYAPI/KOD" + kodNo;
                string kodAd = TNS.DEG.Arac.DegiskenDegerBul(yil, yol);
                if (!String.IsNullOrEmpty(kodAd))
                {
                    string kodYapiIc = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/KODYAPI");
                    string vtTabloAdiIc = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/VTTABLOADI");
                    string vtAlanAdiIc = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/VTALANADI");
                    string vtAlanKodu = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/VTALANKODU");

                    string vtBaglantiAdi = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/VTBAGLANTIADI");
                    string yilYok = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/YILYOK");
                    string ekSayfa = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/EKSAYFA");
                    string ekSayfaSeviye = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/EKSAYFASEVIYE");
                    string ekAlanlar = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/EKALANLAR");

                    ButceKodYapi bky = new ButceKodYapi(yil, kodNo, kodAd, kodYapiIc, vtTabloAdiIc, vtAlanAdiIc, vtAlanKodu, vtBaglantiAdi, yilYok, ekSayfa, ekSayfaSeviye, ekAlanlar);
                    if (bky.ekAlanlar != null)
                    {
                        foreach (EkAlan ekAlan in bky.ekAlanlar)
                        {
                            ekAlan.SeviyeBelirle(TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/" + ekAlan.ad + "/SEVIYE"));
                            ekAlan.Maske = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/" + ekAlan.ad + "/MASKE");
                            ekAlan.ListeUzayAd = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/" + ekAlan.ad + "/LISTE");
                            ekAlan.OrtakAlan = TNS.DEG.Arac.DegiskenDegerBul(yil, yol + "/" + ekAlan.ad + "/ORTAKALAN");
                        }
                    }

                    butceKodlari.Add(bky);
                    kodNo++;
                }
                else
                    break;
            }
            yillar.Add(yil);
        }

        private static void ButceKoduOkuYil(int yil)
        {
            System.Threading.Monitor.Enter(yillar);
            if (yillar.IndexOf(yil) < 0)
            {
                TumKodlariOku(yil);
            }
            System.Threading.Monitor.Exit(yillar);
        }

        private static void YilYoksaOku(int yil)
        {
            if (yillar.IndexOf(yil) < 0)
            {
                ButceKoduOkuYil(yil);
            }
        }

        public static ButceKodYapi ButceKodlariAl(int yil, int kodNo, string kodAd)
        {
            YilYoksaOku(yil);

            for (int i = 0; i < butceKodlari.Count; i++)
            {
                ButceKodYapi bky = (ButceKodYapi)butceKodlari[i];
                if (bky.yil == yil && (bky.kodNo == kodNo || bky.butceKodAd == kodAd))
                    return bky;
            }
            return null;
        }

        public static ButceKodYapi ButceKodlariAl(int yil, int kodNo)
        {
            return ButceKodlariAl(yil, kodNo, "");
        }

        public static ButceKodYapi ButceKodlariAl(int yil, string kodAd)
        {
            return ButceKodlariAl(yil, -1, kodAd);
        }

        public static ButceKodYapi ButceKodlariAlKodYadaAd(int yil, string kodYadaAd)
        {
            int kodNo = OrtakFonksiyonlar.ConvertToInt(kodYadaAd, -1);
            if (kodNo != -1)
            {
                return ButceKodlariAl(yil, kodNo);
            }
            else
            {
                return ButceKodlariAl(yil, kodYadaAd);
            }
        }
    }

    /// <summary>
    /// Bütçe kod tanım bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class KodServisi
    {
        /// <summary>
        /// Veri tabanı erişiminde kullanılacak bilgilere erişmek için, konfigurasyon dosyasındaki değişken ismi.
        /// </summary>
        static string BAGLANTISATIR = "BaglantiSatiriBKD";

        private static string BaglantiSatiriVer(string baglantiSatiri)
        {
            if (!string.IsNullOrEmpty(baglantiSatiri))
                return baglantiSatiri;
            return BAGLANTISATIR;
        }

        private static EkAlanDeger[] EkAlanDegerleriAl(IDataReader r, int baslaKolon, EkAlan[] ekAlanlar)
        {
            if (ekAlanlar == null)
                return null;
            EkAlanDeger[] donusDeger = new EkAlanDeger[ekAlanlar.Length];
            for (int i = 0; i < ekAlanlar.Length; i++)
            {
                donusDeger[i] = new EkAlanDeger(ekAlanlar[i], null);
                EkAlan ekAlan = ekAlanlar[i];
                if (ekAlan.enumMaske == ENUMMaskeEkAlan.String)
                    donusDeger[i].Deger = VeriErisim.DegerAl(r, baslaKolon + i, string.Empty);
                else if (ekAlan.enumMaske == ENUMMaskeEkAlan.Number)
                    donusDeger[i].Deger = VeriErisim.DegerAl(r, baslaKolon + i, 0);
                else if (ekAlan.enumMaske == ENUMMaskeEkAlan.TNSDateTime)
                    donusDeger[i].Deger = VeriErisim.DegerAl(r, baslaKolon + i);
            }
            return donusDeger;
        }

        /// <summary>
        /// Veri tabanına parçalı yazılan kod açıklama bilgilerini verilen dil için okuyan ve döndüren yordam.
        /// Veri tabanından verilen kriterlerle okunan kayıtlardan parçalar birleştirilerek döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="yil">Açıklaması okunacak kodun yıl bilgisi.</param>
        /// <param name="kodTur">Açıklaması okunacak kodun değişken.xml'deki kod numarası.</param>
        /// <param name="kod">Açıklaması okunacak kodun kod değeri.</param>
        /// <param name="dilKod">Okunacak açıklamanın dili.</param>
        /// <returns>Veri tabanına parçalı yazılan kod açıklama bilgileri döndürülür.</returns>
        public static string AciklamaBul(VeriErisim veriErisim, int yil, int kodTur, string kod, string dilKod)
        {
            ButceKodYapi bky = ButceKodlariListesi.ButceKodlariAl(yil, kodTur);
            if (bky == null)
                throw new Exception(ButceKodResI.BKDKS001);

            if (bky.yilYok)
                yil = 0;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BAGLANTISATIR, TNS.KYM.Arac.KimlikBul(null));
            TABABSKODACIKLAMA tab = new TABABSKODACIKLAMA(ve);
            IDataReader r = null;

            try
            {
                r = tab.Listele(yil, kodTur, kod, dilKod);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                return e.Message;
            }

            string aciklama = string.Empty;
            while (r.Read())
                aciklama += VeriErisim.DegerAl(r, 1, "");
            r.Close();

            ve.VeriErisimKapat();
            return aciklama;
        }

        /// <summary>
        /// Verilen arama kıstası kullanılarak, kod ve addan arama yaparak, uyan kodları listeleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="dilKod">Adların aranacağı ve listeleneceği dil.</param>
        /// <param name="yil">Kodların listeleneceği yıl.</param>
        /// <param name="kodTuru">Listelenecek kodun tür kodu (Değişken.xml'den kodların adları ve numaraları bulunabilir.)</param>
        /// <param name="aramaKistas">Listelenecek kodların uyacağı arama dizgisi. Arama dizgisi boşluklardan bölünür. İlk parça hem kod için hem de ad için kullanılır. Diğer parçalar sadece ad için arama da kullanılır.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi döndürülür.</returns>
        internal static ABSKod[] KodAra(VeriErisim veriErisim, string dilKod, int yil, string kodTuru, string aramaKistas)
        {
            return KodAra(veriErisim, dilKod, yil, kodTuru, aramaKistas, false);
        }

        internal static ABSKod[] KodAra(VeriErisim veriErisim, string dilKod, int yil, string kodTuru, string aramaKistas, bool adaGoreSirala)
        {
            if (string.IsNullOrEmpty(aramaKistas))
                throw new Exception(ButceKodResI.BKDKS001);

            ButceKodYapi bky = ButceKodlariListesi.ButceKodlariAlKodYadaAd(yil, kodTuru);
            if (bky == null)
            {
                throw new Exception(ButceKodResI.BKDKS001);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BaglantiSatiriVer(bky.vtBaglantiAd), TNS.KYM.Arac.KimlikBul(null));
            ArrayList al = new ArrayList();
            TABButceKod tbk = new TABButceKod(ve, yil, bky.kodNo, bky.butceKodAd, bky.kodYapi, bky.tabloAd, bky.vtAlanKod, bky.vtAlanAd, bky.yilYok);
            IDataReader r = tbk.Ara(dilKod, yil, aramaKistas, bky.ekAlanlar, adaGoreSirala);

            if (tbk.Hata() == "")
            {
                while (r.Read())
                {
                    ABSKod kod = new ABSKod(OrtakFonksiyonlar.KodNoktaKoy(bky.kodYapi, r.GetString(0)));
                    kod.DilEkle(dilKod, VeriErisim.DegerAl(r, 1, ""), "");
                    kod.ekAlanlar = EkAlanDegerleriAl(r, 2, bky.ekAlanlar);
                    al.Add(kod);
                }
                r.Close();
            }
            return (ABSKod[])al.ToArray(typeof(ABSKod));
        }

        /// <summary>
        /// Kendisine parametre olarak gecirilen kodTuru'ne ve yil'a ait, ve verilen kod'un 
        ///     - seviye 0 ise kodun bir alt kırılımındaki kodları
        ///     - seviye 0 değilse, 
        ///         - sadeceDetay ise verilen seviyedeki kodları
        ///         - sadeceDetay değil ise, kod ile seviye arasındaki tüm kodları
        /// listeleyen yordam. ustKod parametresi boş ise listeleme ilk seviyeden başlar.
        /// Veri tabanından verilen kodların listesi alınır ve kodla birlikte gelen her dil için kod yapısına ad eklenir.
        /// Daha sonra her bir dil için açıklama bulunur ve kod yapısına eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="yil">Kod değerlerinin listeleneceği yıl.</param>
        /// <param name="kodTuru">Listelenecek kodun tür kodu (Değişken.xml'den kodların adları ve numaraları bulunabilir.)</param>
        /// <param name="ustKod">Listelenecek kodun başlayacağı seviye ve kıstas parametresi.</param>
        /// <param name="seviye">Listelenecek kodun biteceği seviyeyi gösteren parametre.</param>
        /// <param name="sadeceDetay">Bu paramtre doğru ise verilen koda ve üst koda uyan, en alt seviye kodlar listelenir ve seviye parametresi kullanımaz. Bu parametre yanlış ise seviye parametresi kullanılır.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi ve sonuc bilgisi döndürülür.</returns>
        public static ABSKodlarVAN KodListeleSeviye(VeriErisim veriErisim, int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay, bool adaGoreSirala, string filtreAd, string filtreKod, bool hiyerarsik, bool ikiYil, string baslangicFiltreKodAd, bool aciklamaEkle)
        {
            ButceKodYapi bky = ButceKodlariListesi.ButceKodlariAlKodYadaAd(yil, kodTuru);
            filtreAd = filtreAd.Trim();
            if (bky == null)
            {
                ABSKodlarVAN akv = new ABSKodlarVAN();
                akv.sonuc.islemSonuc = false;
                akv.sonuc.hataStr = ButceKodResI.BKDKS001;
                return akv;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BaglantiSatiriVer(bky.vtBaglantiAd), null);
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, BaglantiSatiriVer(bky.vtBaglantiAd), null);
            ABSKodlarVAN bkv = new ABSKodlarVAN();
            TABButceKod tbk = new TABButceKod(ve, yil, bky.kodNo, bky.butceKodAd, bky.kodYapi, bky.tabloAd, bky.vtAlanKod, bky.vtAlanAd, bky.yilYok);
            TABButceKod tbk2 = new TABButceKod(ve2, yil, bky.kodNo, bky.butceKodAd, bky.kodYapi, bky.tabloAd, bky.vtAlanKod, bky.vtAlanAd, bky.yilYok);
            int enUzunKod = bky.kodYapi.Length;
            int bolumsay = bky.kodBolumler.Length;
            if (seviye == -1 || seviye > bolumsay)
                seviye = bolumsay;
            IDataReader r = tbk.Listele(ustKod.Replace(".", ""), seviye, sadeceDetay, bky.ekAlanlar, ikiYil, adaGoreSirala, filtreKod, baslangicFiltreKodAd);

            if (tbk.Hata() == "")
            {
                ABSKod kodtut = new ABSKod();
                if (r != null)
                {
                    while (r.Read())
                    {
                        string okunanKod;
                        if (bky.kodYapi == "N")
                            okunanKod = r.GetInt32(0).ToString();
                        else
                        {
                            okunanKod = r.GetString(0);
                            if (bky.kodYapi != "S")
                                okunanKod = OrtakFonksiyonlar.KodNoktaKoy(bky.kodYapi, okunanKod);
                        }
                        ABSKod kod = new ABSKod(okunanKod);
                        bool ekle = true;
                        if (!string.IsNullOrEmpty(filtreAd))
                        {
                            double tut = tbk2.FiltreliKayitSay(kod.kod, filtreAd);
                            if (tut < 1) ekle = false;
                        }
                        if (ekle && sadeceDetay && !hiyerarsik)
                        {
                            if (kod.kod.Length < enUzunKod)
                            {
                                double tut = tbk2.FiltreliKayitSay(kod.kod, "");
                                if (tut > 1) ekle = false;
                            }
                        }
                        if (ekle)
                        {
                            for (int i = 0; i < BKD.Arac.DillerLength; i++)
                            {
                                kod.DilEkle(BKD.Arac.Diller(i), VeriErisim.DegerAl(r, 1 + i, ""), "");
                            }
                            kod.ekAlanlar = EkAlanDegerleriAl(r, 1 + BKD.Arac.DillerLength, bky.ekAlanlar);
                            bkv.absKodlar.Add(kod);
                        }
                    }
                    r.Close();

                    if (kodTuru != "20" && aciklamaEkle)//COGRAFYA tablosundan çekerken kod açıklama tablosuna bakmasın
                    {
                        foreach (ABSKod kod in bkv.absKodlar)
                        {
                            for (int i = 0; i < BKD.Arac.DillerLength; i++)
                            {
                                kod.AciklamaAta(BKD.Arac.Diller(i), AciklamaBul(ve, yil, bky.kodNo, kod.kod, BKD.Arac.Diller(i)));
                            }
                        }
                    }
                }
                bkv.sonuc.islemSonuc = true;
            }
            else
            {
                bkv.sonuc.islemSonuc = false;
                bkv.sonuc.hataStr = tbk.Hata();
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();//ht:connection pool size dolu hatasına düşüyordu.bu nedenle eklendi
            return bkv;
        }

        public static string KodAdBul(VeriErisim veriErisim, int yil, string kodTuru, string kod)
        {
            ButceKodYapi bky = ButceKodlariListesi.ButceKodlariAlKodYadaAd(yil, kodTuru);
            if (bky == null)
            {
                return ButceKodResI.BKDKS001;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BaglantiSatiriVer(bky.vtBaglantiAd), TNS.KYM.Arac.KimlikBul(null));
            ABSKodlarVAN bkv = new ABSKodlarVAN();
            TABButceKod tbk = new TABButceKod(ve, yil, bky.kodNo, bky.butceKodAd, bky.kodYapi, bky.tabloAd, bky.vtAlanKod, bky.vtAlanAd, bky.yilYok);

            IDataReader r = tbk.Oku(kod.Replace(".", ""));
            string ad = "";
            if (r != null)
            {
                if (r.Read())
                {
                    ad = VeriErisim.DegerAl(r, 0, "");
                }
                r.Close();
            }
            else
            {
                ad = tbk.Hata();
            }

            ve.VeriErisimKapat();

            return ad;
        }

        /// <summary>
        /// Verilen bir kuruma ait birimleri listeleyen yordam. Bu yordam verilen kullanıcının görebileceği birimleri listeler.
        /// Kurum ve alt düzey seçimine göre ek koşul oluşturulur ve birim listesi ile, birim listesinin tüm dillerdeki adları bulunur. Birim açıklamaları okunmaz.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Görebileceği birimleri listelenecek kullanıcı bilgilerini tutan sınıf.</param>
        /// <param name="yil">Birimlerin listeleneceği yıl.</param>
        /// <param name="kurum">Birimlerin listeleneceği kurum.</param>
        /// <param name="altDuzey">En alt düzeydeki birimler listelenir.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi ve sonuc bilgisi döndürülür.</returns>
        public static ABSKodlarVAN BirimListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, bool altDuzey, bool ikiYil)
        {
            return BirimListele(veriErisim, kullanan, yil, kurum, altDuzey, ikiYil, false);
        }

        public static ABSKodlarVAN BirimListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, bool altDuzey, bool ikiYil, bool adaGoreSirala)
        {
            ButceKodYapi bky = ButceKodlariListesi.ButceKodlariAlKodYadaAd(yil, "1");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BaglantiSatiriVer(bky.vtBaglantiAd), TNS.KYM.Arac.KimlikBul(kullanan));
            ABSKodlarVAN kv = new ABSKodlarVAN();
            TABButceKod tb = new TABButceKod(ve, yil, bky.kodNo, bky.butceKodAd, bky.kodYapi, bky.tabloAd, bky.vtAlanKod, bky.vtAlanAd, bky.yilYok);

            int seviye;
            if (altDuzey)
                seviye = bky.kodYapi.Split('.').Length;
            else
                seviye = -1;//Yetkilerde tüm kurumsal kodların listelenmesinde,  seviye parametre olarak gelmediği için -1 verildi. Eski değer 0 dı. 

            IDataReader r = tb.Listele(kurum, seviye, true, bky.ekAlanlar, ikiYil, adaGoreSirala, "", "");
            if (tb.Hata() == "")
            {
                while (r.Read())
                {
                    ABSKod k = new ABSKod(OrtakFonksiyonlar.KodNoktaKoy(bky.kodYapi, r.GetString(0)));
                    for (int i = 0; i < BKD.Arac.DillerLength; i++)
                    {
                        string ad = "";
                        try
                        {
                            ad = VeriErisim.DegerAl(r, i + 1, "");
                        }
                        catch { }
                        k.DilEkle(BKD.Arac.Diller(i), ad, "");
                    }
                    k.ekAlanlar = EkAlanDegerleriAl(r, 1 + BKD.Arac.DillerLength, bky.ekAlanlar);
                    kv.absKodlar.Add(k);
                }
                kv.sonuc.islemSonuc = true;
            }
            else
            {
                kv.sonuc.islemSonuc = false;
                kv.sonuc.hataStr = tb.Hata();

            }
            ve.VeriErisimKapat();

            return kv;
        }

        /// <summary>
        /// Verilen listedeki (TNSCollection) kodları veri tabanına kaydeden yordam.
        /// Listedeki her bir kod için kontrol yapılır. Kontrolden çıkılırsa, her bir kod için silme, ekleme veya güncelleme işlemi yapılır.
        /// Kontroller:
        ///     Kod boş olamaz.
        ///     Kod silinecekse kod hiyerarşisinde, kodun alt kodu olamaz.
        /// Kod açıklamasının parçaları önce veri tabanından silinir ve veri tabanına parçalı olarak kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Kodların kaydedileceği yıl.</param>
        /// <param name="kodTuru">Kodların ait olduğu kod türü. Bu bilgi kullanılarak veri tabanı bilgileri bulunur.</param>
        /// <param name="kodlar">Kaydedilecek kod değerlerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KodKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kodTuru, TNSCollection kodlar)
        {
            if (yil < 0)
                return new Sonuc(false, ButceKodResI.BKDKS007, "");

            ButceKodYapi bky = ButceKodlariListesi.ButceKodlariAlKodYadaAd(yil, kodTuru);
            if (bky == null) return new Sonuc(false, string.Format(ButceKodResI.BKDKS002, kodTuru), "");

            if (bky.yilYok)
                yil = 0;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BaglantiSatiriVer(bky.vtBaglantiAd), TNS.KYM.Arac.KimlikBul(kullanan));
            TABButceKod tbk = new TABButceKod(ve, yil, bky.kodNo, bky.butceKodAd, bky.kodYapi, bky.tabloAd, bky.vtAlanKod, bky.vtAlanAd, bky.yilYok);

            //2021 Cumhur veritabanına ulaşılamadığı için yapıldı. Silinecek Melih 09.01.2021
            //if (kullanan.kullaniciKodu == "stratek")
            //{
            //    tbk.EkoKodYarat2021();
            //    ve.VeriErisimKapat();
            //    return new Sonuc();
            //}

            Sonuc sonuc = KodDegistirmeYetkisiVarMi(kullanan);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = string.Format(ButceKodResI.BKDKS003, bky.butceKodAd);
                ve.VeriErisimKapat();
                return sonuc;
            }

            foreach (ABSKod kodd in kodlar)
            {
                string kodTut = kodd.kod.Replace(".", "");
                if (kodTut == "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceKodResI.BKDKS004;
                    break;
                }

                if (kodd.kayitDurumu == ENUMKayitDurumu.SIL)
                {
                    if (kodTut.Length != bky.kodYapi.Replace(".", "").Length)// son kırılımda değil ise
                    {
                        sonuc = tbk.VeriVarMi(kodTut);
                        if (sonuc.islemSonuc)//alt kod var 
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = string.Format(ButceKodResI.BKDKS005, kodd.kod);
                            break;
                        }
                        else
                            sonuc.islemSonuc = true;
                    }
                }
            }

            if (sonuc.islemSonuc) // Hepsi bitti devam et.
            {
                ve.BeginTransaction();

                VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, BAGLANTISATIR, TNS.KYM.Arac.KimlikBul(kullanan));
                TABABSKODACIKLAMA tabKodAciklama = new TABABSKODACIKLAMA(ve2);

                ve2.BeginTransaction();

                foreach (ABSKod kodd in kodlar)
                {
                    string kodTut = kodd.kod.Replace(".", "");
                    if (kodd.kayitDurumu == ENUMKayitDurumu.YENI || kodd.kayitDurumu == ENUMKayitDurumu.GUNCELLE)
                    {
                        kodd.UygunEkAlanlariBul();
                        sonuc = tbk.EkleVeyaGuncelle(kodTut, kodd.diller, kodd.ekAlanlar);
                        if (sonuc.islemSonuc)
                        {
                            for (int i = 0; i < kodd.diller.Length; i++)
                            {
                                string ulkeKod = (string)kodd.diller.dilKod[i];
                                if (!string.IsNullOrEmpty(ulkeKod))
                                {
                                    ABSKodDil kd = (ABSKodDil)kodd.diller.absKodDil[i];
                                    sonuc = tabKodAciklama.Sil(yil, ulkeKod, bky.kodNo, kodd.kod);
                                    if (!sonuc.islemSonuc) break;
                                    string giris = kd.aciklama;
                                    int yazilan = 0;
                                    string parca = "";
                                    int satir = 0;
                                    int kalan = giris.Length;
                                    int parcaBoy;
                                    while (kalan > 0)
                                    {
                                        if (kalan > 250)
                                            parcaBoy = 250;
                                        else
                                            parcaBoy = kalan;

                                        parca = giris.Substring(yazilan, parcaBoy);
                                        yazilan = yazilan + parcaBoy;
                                        kalan = kalan - parcaBoy;
                                        satir++;
                                        sonuc = tabKodAciklama.Kaydet(yil, ulkeKod, bky.kodNo, kodd.kod, satir, parca);
                                        if (!sonuc.islemSonuc) break;
                                    }
                                    if (!sonuc.islemSonuc) break;
                                }
                            }
                        }
                    }
                    else if (kodd.kayitDurumu == ENUMKayitDurumu.SIL)
                    {
                        sonuc = tabKodAciklama.Sil(yil, "", bky.kodNo, kodd.kod);
                        if (sonuc.islemSonuc)
                            sonuc = tbk.KodSil(kodTut);
                    }

                    if (!sonuc.islemSonuc)
                    {
                        sonuc.hataStr = string.Format(ButceKodResI.BKDKS006, kodd.kod, sonuc.hataStr);
                        break;
                    }
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    ve2.Commit();
                }
                else
                {
                    ve.Rollback();
                    ve2.Rollback();
                }

                ve2.VeriErisimKapat();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Kullanıcının kod değiştirme yetkisi olup olmadığını bulan yordam.
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>Kod değiştirme yetkisi yoksa, Sonuc içinde yanlış; varsa doğru döndürülür.</returns>
        public static Sonuc KodDegistirmeYetkisiVarMi(Kullanici kullanan)
        {
            Sonuc sonuc = new Sonuc();
            if (kullanan.KullaniciTipiIceriyorMu((int)KYM.ENUMKullaniciTipi.SISTEMYONETICISI))
                sonuc.islemSonuc = true;
            else
                sonuc.islemSonuc = false;
            return sonuc;

        }

        /// <summary>
        /// Verilen kod türü bir önceki yıldaki değerleri bir sonraki yıla kopyalar.
        /// İşlem sonucu Sonuc yapısında döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Kodların kaydedileceği yıl.</param>
        /// <param name="kodTuru">Kodların ait olduğu kod türü. Bu bilgi kullanılarak veri tabanı bilgileri bulunur.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        internal static Sonuc OncekiYildanKopyala(VeriErisim veriErisim, Kullanici kullanan, int yil, string kodTuru)
        {
            Sonuc sonuc = KodDegistirmeYetkisiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;
            ABSKodlarVAN kodlar = KodListeleSeviye(veriErisim, yil - 1, kodTuru, "", -1, false, false, "", "", true, false, "", true);
            if (!kodlar.sonuc.islemSonuc)
            {
                return kodlar.sonuc;
            }

            TNSCollection kod = new TNSCollection();

            foreach (ABSKod k in kodlar.absKodlar)
            {
                k.kayitDurumu = ENUMKayitDurumu.YENI;
            }

            return KodKaydet(veriErisim, kullanan, yil, kodTuru, kodlar.absKodlar);
        }

        internal static int ButceKodSatirSayisiBul(VeriErisim veriErisim, int yil, string kodTuru)
        {
            ButceKodYapi bky = ButceKodlariListesi.ButceKodlariAlKodYadaAd(yil, kodTuru);
            if (bky == null)
            {
                throw new Exception(ButceKodResI.BKDKS001);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BaglantiSatiriVer(bky.vtBaglantiAd), TNS.KYM.Arac.KimlikBul(null));
            ArrayList al = new ArrayList();
            TABButceKod tbk = new TABButceKod(ve, yil, bky.kodNo, bky.butceKodAd, bky.kodYapi, bky.tabloAd, bky.vtAlanKod, bky.vtAlanAd, bky.yilYok);
            if (bky.yilYok)
                yil = -1;

            int say = tbk.KayitSayiBul(yil);
            ve.VeriErisimKapat();
            return say;
        }

        public static ABSKodlarVAN KurumListesiKullanici(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            ButceKodYapi bky = ButceKodlariListesi.ButceKodlariAlKodYadaAd(yil, "1");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BaglantiSatiriVer(bky.vtBaglantiAd), TNS.KYM.Arac.KimlikBul(kullanan));
            ABSKodlarVAN kv = new ABSKodlarVAN();
            TABButceKod tb = new TABButceKod(ve, yil, bky.kodNo, bky.butceKodAd, bky.kodYapi, bky.tabloAd, bky.vtAlanKod, bky.vtAlanAd, bky.yilYok);
            string ekKosul;
            ekKosul = "";

            IDataReader r = tb.KurumListesiKosul(ekKosul, bky.bolumSay == 4);
            if (r != null)
            {
                while (r.Read())
                {
                    ABSKod k = new ABSKod(OrtakFonksiyonlar.KodNoktaKoy(bky.kodYapi, r.GetString(0)));

                    for (int i = 0; i < BKD.Arac.DillerLength; i++)
                    {
                        k.DilEkle(BKD.Arac.Diller(i), VeriErisim.DegerAl(r, 1 + i, ""), "");
                    }
                    kv.absKodlar.Add(k);
                }
                kv.sonuc.islemSonuc = true;
            }
            else
            {
                kv.sonuc.islemSonuc = false;
                kv.sonuc.hataStr = tb.Hata();
            }
            ve.VeriErisimKapat();
            return kv;
        }

        //public static ABSKodlarVAN KurumListesi(VeriErisim veriErisim, int yil, ENUMButceTur butceTur)
        //{
        //    ButceKodYapi bky = ButceKodlariListesi.ButceKodlariAlKodYadaAd(yil, "1");
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BAGLANTISATIR);
        //    ABSKodlarVAN kv = new ABSKodlarVAN();
        //    TABButceKod tb = new TABButceKod(ve, yil, bky.kodNo, bky.butceKodAd, bky.kodYapi, bky.tabloAd, bky.vtAlanKod, bky.vtAlanAd);

        //    IDataReader r = tb.KurumListesiKosul(butceTur);
        //    if (tb.Hata() == "")
        //    {
        //        while (r.Read())
        //        {
        //            ABSKod k = new ABSKod(OrtakFonksiyonlar.KodNoktaKoy("99.99.99", r.GetString(0)));
        //            for (int i = 0; i < TNS.BKD.Arac.Diller.Length; i++)
        //            {
        //                k.DilEkle(TNS.BKD.Arac.Diller[i], r.GetString(i + 1), "");
        //            }
        //            kv.absKodlar.Add(k);
        //        }
        //        kv.sonuc.islemSonuc = true;
        //    }
        //    else
        //    {
        //        kv.sonuc.islemSonuc = false;
        //        kv.sonuc.hataStr = tb.Hata();

        //    }
        //    ve.VeriErisimKapat();

        //    return kv;
        //}
    }
}


/*
        //private static Sonuc YetkiKontrolKurumsal(Kullanici kullanan, string kod)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    if (kullanan.KullaniciTipiIceriyorMu((int)KYM.ENUMKullaniciTipi.SISTEMYONETICISI))
        //        sonuc.islemSonuc = true;
        //    else
        //        sonuc.islemSonuc = false;

        //    return sonuc;
        //}

        //private static Sonuc YetkiKontrolFonksiyonel(Kullanici kullanan, string kod, string kodYapi)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    if (kullanan.KullaniciTipiIceriyorMu((int)KYM.ENUMKullaniciTipi.SISTEMYONETICISI))
        //        sonuc.islemSonuc = true;
        //    else
        //        sonuc.islemSonuc = false;
        //    return sonuc;
        //}
*/

using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.UZY;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Bütçe tasnifi yapıldıktan sonra, her bir tasnif hiyerarşik satırına veri girilmesi sağlanır.
    /// Girilen verilerle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class ButceTasnifOdenekGiris
    {
        /// <summary>
        /// Kullanıcının tasnif ödeneği yazma yetkisinin var olup olmadığına bakan yordam.
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="tasnifOdenekleriFormu">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, ButceTasnifOdenekleriFormu tasnifOdenekleriFormu)
        {
            return true;
        }

        /// <summary>
        /// Verilen tasnif ödenekleri formundaki alanların tutarlılığını kontrol eden yordam.
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="tasnifOdenekleriFormu">Alan tutarlılığı kontrol edilecek tasnif ödenekleri formu nesnesi</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, ButceTasnifOdenekleriFormu tasnifOdenekleriFormu)
        {
            return new Sonuc(true, string.Empty, string.Empty);
        }

        /// <summary>
        /// Form içerisinde verilen tasnif ödenek değerlerini, tasnif sıra numarası, yıl ve senaryo bilgisini kullanarak kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifOdenekleriFormu">Form parametresinin; tasnif sıra numarası, yıl ve senaryo alanları ve verilen ödenekler kullanılarak kayıt yapılır.</param>
        /// <param name="onceSilme">Bu parametre doğru ise silme işlemi yapılmaz, sadece ekleme işlemi yapılır.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ButceTasnifOdenekleriFormu tasnifOdenekleriFormu, bool onceSilme)
        {
            Sonuc s = new Sonuc(true, string.Empty, string.Empty);
            if (!YazmaYetkisiVarMi(kullanan, tasnifOdenekleriFormu))
            {
                s.DegerAta(ButceHazirlikResI.BHMBTOG001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, tasnifOdenekleriFormu);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCETASNIFODENEK tabButceTasnifOdenek = new TABBHMBUTCETASNIFODENEK(ve);

            ve.BeginTransaction();

            if (tasnifOdenekleriFormu.tasnif.senaryolar != null)
            {
                for (int i = 0; i < tasnifOdenekleriFormu.tasnif.senaryolar.Length && s.islemSonuc; i++)
                {
                    for (int j = 0; j < tasnifOdenekleriFormu.butceTasnifSatirOdenekleri.Length && s.islemSonuc; j++)
                    {
                        if (!onceSilme)
                            s = tabButceTasnifOdenek.Sil(tasnifOdenekleriFormu, j, tasnifOdenekleriFormu.tasnif.senaryolar[i].kod);
                        if (tasnifOdenekleriFormu.butceTasnifSatirOdenekleri[j].ekDegerler[i] > 0)
                            s = tabButceTasnifOdenek.Ekle(tasnifOdenekleriFormu, j, tasnifOdenekleriFormu.tasnif.senaryolar[i].kod, i);
                    }
                }
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Kullanıcının, tasnif ödenekleri formunu görme yetkisinin var olup olmadığına bulan yordam.
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="tasnifOdenekleriFormu">Kullanıcının görmeye çalıştığı tasnif ödenekleri formu nesnesi.</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, ButceTasnifOdenekleriFormu tasnifOdenekleriFormu)
        {
            return true;
        }

        /// <summary>
        /// Tasnif ödenekleri formunun tutarlılığını kontrol eden yordam.
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="tasnifOdenekleriFormu">Anahtar alan tutarlılığı kontrol edilecek tasnif ödenekleri formu nesnesi.</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, ButceTasnifOdenekleriFormu tasnifOdenekleriFormu)
        {
            return new Sonuc(true, string.Empty, string.Empty);
        }

        /// <summary>
        /// Verilen tasnif sıra numarası, yıl ve senaryo bilgisine göre, 
        /// kullanıcının girmiş olduğu tasnif değerlerini döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifOdenekleriFormu">Form parametresinin; tasnif sıra numarası, yıl ve senaryo alanları kullanılarak değerler bulunacaktır.</param>
        /// <param name="ekDegerParamlar">Okunan her bir satırla birlikte döndürülecek olan ek bilgilerin tanımlarını tutan dizi.</param>
        /// <param name="senaryoKod">Eğer verilmişse, sadece o senaryo, verilmemişse, tasnif ile ilişkilendirilmiş tüm senaryolar okunur.</param>
        /// <param name="hiyerarsiKod">Eğer verilmişse, sadece o satır, verilmemişse, tüm satırlar okunur.</param>
        /// <param name="kistasTertip">Okunan her bir satırdaki tertip şablonlarının uyması gereken kistas tertip.</param>
        /// <returns>Verilen tasnif sıra numarası, yıl ve senaryo bilgisine göre girilen değerler ve istenilen ek bilgiler form yapısı içindeki dizide döndürülür.</returns>
        public static ButceTasnifOdenekleriFormu Oku(VeriErisim veriErisim, Kullanici kullanan, ButceTasnifOdenekleriFormu tasnifOdenekleriFormu, EkDegerParam[] ekDegerParamlar, string senaryoKod, string hiyerarsiKod, string kistasTertip)
        {
            if (!GormeYetkisiVarMi(kullanan, tasnifOdenekleriFormu))
            {
                throw new Exception(ButceHazirlikResI.BHMBTOG002);
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, tasnifOdenekleriFormu);
            if (!s.islemSonuc)
            {
                throw new Exception(s.hataStr);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCETASNIFODENEK tabButceTasnifOdenek = new TABBHMBUTCETASNIFODENEK(ve);

            IUZYServis uzyServis = UZY.Arac.Tanimla();
            ButceTasnifOdenekleriFormu formIc = new ButceTasnifOdenekleriFormu();
            if (!string.IsNullOrEmpty(senaryoKod))
            {
                formIc.tasnif.senaryolar = new ButceSenaryo[1];
                formIc.tasnif.senaryolar[0] = new ButceSenaryo();
                formIc.tasnif.senaryolar[0].kod = senaryoKod;
                formIc.tasnif.senaryolar[0].ad = TNS.BHMv2.IsKurallari.BHMv2ButceSenaryo.AdBul(ve, senaryoKod);
            }
            else
            {
                formIc.tasnif.senaryolar = TNS.BHMv2.IsKurallari.BHMv2ButceSenaryo.SenaryolariBul(ve, tasnifOdenekleriFormu.tasnif.siraNo);
            }
            ButceTasnifHiyerarsikSatir[] bthsler = TNS.BHMv2.IsKurallari.BHMv2ButceTasnifHiyerarsi.Listele(ve, kullanan, tasnifOdenekleriFormu.tasnif.siraNo, tasnifOdenekleriFormu.yil, hiyerarsiKod, kistasTertip);

            ArrayList al = new ArrayList();
            int senSay = 0;
            if (formIc.tasnif.senaryolar != null)
            {
                senSay = formIc.tasnif.senaryolar.Length;
            }
            if (string.IsNullOrEmpty(hiyerarsiKod))
            {
                ButceTasnifSatirOdenegi toplamSat;
                //if (!string.IsNullOrEmpty(senaryoKod))
                //    toplamSat = new ButceTasnifSatirOdenegi(1);
                //else
                toplamSat = new ButceTasnifSatirOdenegi(senSay * 2 + EkKolonSay(ekDegerParamlar));
                toplamSat.hiyerarsi.ad = ButceHazirlikResI.BHMBTOG003;
                toplamSat.hiyerarsi.kod = string.Empty;
                toplamSat.hiyerarsi.altKirilimVar = true;
                al.Add(toplamSat);
            }
            for (int i = 0; i < bthsler.Length; i++)
            {
                ButceTasnifSatirOdenegi detay;
                //if (!string.IsNullOrEmpty(senaryoKod))
                //    detay = new ButceTasnifSatirOdenegi(1);
                //else
                detay = new ButceTasnifSatirOdenegi(senSay * 2 + EkKolonSay(ekDegerParamlar));
                detay.hiyerarsi = bthsler[i];
                if (!detay.hiyerarsi.altKirilimVar)
                {
                    //if (!string.IsNullOrEmpty(senaryoKod))
                    //    detay.ekDegerler[0] = ButceTasnifOdenekBul(tabButceTasnifOdenek, tasnifOdenekleriFormu.tasnif.siraNo, tasnifOdenekleriFormu.yil, formIc.tasnif.senaryolar[0].kod, detay.hiyerarsi.kod);
                    //else
                    {
                        for (int senNo = 0; senNo < senSay; senNo++)
                        {
                            detay.ekDegerler[senNo] = ButceTasnifOdenekBul(tabButceTasnifOdenek, tasnifOdenekleriFormu.tasnif.siraNo, tasnifOdenekleriFormu.yil, formIc.tasnif.senaryolar[senNo].kod, detay.hiyerarsi.kod);
                        }
                        for (int senNo = 0; senNo < senSay; senNo++)
                        {
                            detay.ekDegerler[senSay + senNo] = EkDegerHesapla(kullanan, uzyServis, tasnifOdenekleriFormu.yil, detay.hiyerarsi.tertipSablonlar, new EkDegerParam(BKD.ENUMTabloKod.TABPEBOdenek, 0, 0, formIc.tasnif.senaryolar[senNo].kod));
                        }
                        for (int ekSay = 0; ekSay < EkKolonSay(ekDegerParamlar); ekSay++)
                        {
                            detay.ekDegerler[senSay * 2 + ekSay] = EkDegerHesapla(kullanan, uzyServis, tasnifOdenekleriFormu.yil, detay.hiyerarsi.tertipSablonlar, ekDegerParamlar[ekSay]);
                        }
                        if (string.IsNullOrEmpty(hiyerarsiKod))
                            Ekle(al, al.Count, detay.hiyerarsi.kod, detay.ekDegerler);
                    }
                }
                else if (detay.ekDegerler != null)
                    for (int j = 0; j < detay.ekDegerler.Length; j++)
                    {
                        detay.ekDegerler[j] = 0;
                    }
                al.Add(detay);
            }
            formIc.butceTasnifSatirOdenekleri = (ButceTasnifSatirOdenegi[])al.ToArray(typeof(ButceTasnifSatirOdenegi));
            return formIc;
        }

        private static int EkKolonSay(EkDegerParam[] ekDegerParamlar)
        {
            if (ekDegerParamlar == null) return 0;
            else return ekDegerParamlar.Length;
        }

        private static double ButceTasnifOdenekBul(TABBHMBUTCETASNIFODENEK tabButceTasnifOdenek, int tasnifNo, int yil, string senaryoKod, string hiyerarsikSatirKod)
        {
            return tabButceTasnifOdenek.OdenekBul(tasnifNo, yil, senaryoKod, hiyerarsikSatirKod);
        }

        /// <summary>
        /// Tasnif formunun bir satırının ödeneğini, satıra girilmiş olan şablon tertipleri kullanarak bulan ve 
        /// aynı şablonları kullanarak ek değerleri hesaplayan yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="uzyServis">Ek değerler hesaplanırken uzay servisi kullanılır. Kullanılacak uzay servisi sunucu erişim nesnesi.</param>
        /// <param name="yil">Hesaplanacak ek değerin yılı.</param>
        /// <param name="tertipSablonlar">Dizinin her bir şablonu için ek değer hesaplanır, ekleme ve çıkarmalara göre sonuç bulunur.</param>
        /// <param name="ekDegerParam">Kullanıcının girdiği değerlerle birlikte döndürülecek ek değerler dizisi</param>
        /// <returns>Verilen tertip şablonlarına göre hesaplanan ek değer bilgisi döndürülür.</returns>
        public static double EkDegerHesapla(Kullanici kullanan, IUZYServis uzyServis, int yil, ButceTasnifTertipSablon[] tertipSablonlar, EkDegerParam ekDegerParam)
        {
            if (ekDegerParam == null)
                return 0;

            double tut = 0;
            for (int j = 0; j < tertipSablonlar.Length; j++)
            {
                ButceTasnifTertipSablon ts = tertipSablonlar[j];
                object donusDeger = ekDegerParam.EkDegerHesaplaDbl(uzyServis, kullanan, yil, ts.tertipSablon);
                if (donusDeger.GetType() == typeof(double))
                {
                    double hesaplanan = (double)donusDeger;
                    if (ts.ekleCikar == TertipEkleCikar.Ekle)
                        tut += hesaplanan;
                    else
                        tut -= hesaplanan;
                }
            }
            return tut;
        }

        /// <summary>
        /// Bulunan ödeneği ve ekdeğerleri, hiyerarşik satırlarda, üst hiyerarşi satırlarının değerlerine ekleyen özyinelemeli yordam.
        /// Verilen hiyerarşik kodun bir üst hiyerarşik kodu 3 karakter eksiltilerek (.99) bulunur. 
        /// Bu koda sahip satır dizide bulunur ve bu satıra ödenek ve ek değerler eklenir.
        /// Ekleme yordamı, yeni bulunan satırın üst satırlara eklenmesi için tekrar çağrılır. (özyineleme)
        /// </summary>
        /// <param name="butceTasnifDegerler">Bütçe tasnif satırlarını tutan dizi.</param>
        /// <param name="baslaYer">Dizide eklemenin kaynağı olacak satır</param>
        /// <param name="tamKod">Dizide eklemenin kaynağı hiyerarşik satır kodu</param>
        /// <param name="ekDegerler">Satır için hesaplanmış ek değerler dizisi.</param>
        public static void Ekle(ArrayList butceTasnifDegerler, int baslaYer, string tamKod, double[] ekDegerler)
        {
            if (string.IsNullOrEmpty(tamKod))
                return;
            string araKod;
            if (tamKod.Length <= 3) araKod = string.Empty;
            else araKod = tamKod.Substring(0, tamKod.Length - 3);

            ButceTasnifSatirOdenegi detay = DizideBul(butceTasnifDegerler, araKod, ref baslaYer);
            if (detay != null)
            {
                if (ekDegerler != null && detay.ekDegerler != null)
                {
                    if (detay.ekDegerler.Length != ekDegerler.Length)
                    {
                        throw new Exception(ButceHazirlikResI.BHMBTOG005);
                    }
                    for (int i = 0; i < ekDegerler.Length; i++)
                    {
                        detay.ekDegerler[i] = detay.ekDegerler[i] + ekDegerler[i];
                    }
                }
                Ekle(butceTasnifDegerler, baslaYer, araKod, ekDegerler);
            }
            else
                throw new Exception(string.Format(ButceHazirlikResI.BHMBTOG006, araKod));
        }

        /// <summary>
        /// Verilen satır indisinden başlanarak, yukarı doğrı verilen kodu arayan ve bulduğu satırı döndüren yordam.
        /// Satır bulunamazsa null döndürülür.
        /// </summary>
        /// <param name="butceTasnifDegerler">Arama yapılacak dizi</param>
        /// <param name="aranan">Dizide aranan değer</param>
        /// <param name="baslaYer">Arama işleminin başlayacağı satır.</param>
        /// <returns>Arama sonucu bulunana dizi elemanı döndürülür.</returns>
        public static ButceTasnifSatirOdenegi DizideBul(ArrayList butceTasnifDegerler, string aranan, ref int baslaYer)
        {
            for (int i = baslaYer - 1; i >= 0; i--)
            {
                ButceTasnifSatirOdenegi btd = (ButceTasnifSatirOdenegi)butceTasnifDegerler[i];
                if (btd.hiyerarsi.kod.Equals(aranan))
                {
                    baslaYer = i;
                    return btd;
                }
            }
            return null;
        }
    }
}

using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Butçe tasnifinde tutulan hiyerarşik satırlarla ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2ButceTasnifHiyerarsi
    {
        /// <summary>
        /// Kullanıcının bütçe tasnif hiyerarşi satırlarını veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen bütçe tasnifi hiyerarşik satırındaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="hiyerarsikSatir">Alan tutarlılığı kontrol edilecek bütçe tasnifi hiyerarşik satırı nesnesi</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, ButceTasnifHiyerarsikSatir hiyerarsikSatir)
        {
            string hata = string.Empty;
            if (string.IsNullOrEmpty(hiyerarsikSatir.ad))
                hata += ButceHazirlikResI.BHMBTH002 + "<br>";

            if (string.IsNullOrEmpty(hiyerarsikSatir.kod))
                hata += ButceHazirlikResI.BHMBTH003 + "<br>";

            if (hiyerarsikSatir.kod.Replace(".", string.Empty).Length % 2 == 1)
                hata += ButceHazirlikResI.BHMBTH004 + "<br>";

            return new Sonuc(string.IsNullOrEmpty(hata), hata, string.Empty);
        }


        /// <summary>
        /// Bir bütçe tasnifine ait, tanımlı tüm satırları kaydeden yordam.
        /// Hata olursa Exception oluşturur
        /// Öncelikle hiyerarşi dizisindeki, satırlara, satırın içerleklik yerine göre yeni bir hiyerarşik kod verilir.
        /// Her bir satırın, alt kırılımının olup olmadığına, alt kırılımların düzgün yerleştirilip yerleştirilmediğine ve alt kırılımı olmayanlara tertip şablonu girildiğine bakılır.
        /// Sonra:
        ///    1. Eskiden kodu olup da şimdi olmayan satırlar silinir. (Bu satırların verileri de silinir.)
        ///    2. Eskiden kodu olup da şimdi de olanlar güncellenir. (Bu satırların verileri de güncellenir.)
        ///    3. Eskiden kodu olmayıp şimdi olanları ekler.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifSiraNo">Kaydedilecek hiyerarşinin ait olduğu tasnifin sıra numarası</param>
        /// <param name="hiyerarsi">Verilen tasnif numarasına ait hiyerarşik satırları içeren dizi</param>
        /// <param name="yil">Her bir satırdaki tertip şablonunun yıldan yıla değişebileceği tasarlanmıştır. Verilen satırlardaki tertip şablonlarının hangi yıla ait olduğu bilgisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int tasnifSiraNo, ButceTasnifHiyerarsikSatir[] hiyerarsi, int yil)
        {
            Sonuc s = new Sonuc(true, string.Empty, string.Empty);
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta(ButceHazirlikResI.BHMBTH001);
                return s;
            }
            ArrayList intArrayList = new ArrayList();

            // Öncelikle hiyerarşi dizisindeki, satırlara, satırın içerleklik yerine göre yeni bir hiyerarşik kod verilir.
            for (int i = 0; i < hiyerarsi.Length; i++)
            {
                hiyerarsi[i].yeniKod = HiyerarsiKodBul(hiyerarsi[i], intArrayList);
            }
            int sat = SonrakiSatBul(hiyerarsi, -1);
            string hata = string.Empty;

            // Her bir satırın, alt kırılımının olup olmadığına, alt kırılımların düzgün yerleştirilip yerleştirilmediğine ve alt kırılımı olmayanlara tertip şablonu girildiğine bakılır.
            if (sat == -1)
                hata = ButceHazirlikResI.BHMBTH005 + "<br>";
            else
            {
                while (sat != -1)
                {
                    int sonrakiSat = SonrakiSatBul(hiyerarsi, sat);
                    if (sonrakiSat != -1)
                    {
                        if (hiyerarsi[sat].seviye == hiyerarsi[sonrakiSat].seviye - 1)
                            hiyerarsi[sat].altKirilimVar = true;
                        else if (hiyerarsi[sat].seviye >= hiyerarsi[sonrakiSat].seviye)
                            hiyerarsi[sat].altKirilimVar = false;
                        else
                            hata += string.Format(ButceHazirlikResI.BHMBTH006, sat.ToString(), sonrakiSat.ToString()) + "<br>";
                    }
                    else
                        hiyerarsi[sat].altKirilimVar = false;
                    if (!hiyerarsi[sat].altKirilimVar)
                    {
                        if (hiyerarsi[sat].tertipSablonlar_Length() == 0)
                            hata += string.Format(ButceHazirlikResI.BHMBTH007, sat.ToString()) + "<br>";
                    }
                    sat = sonrakiSat;
                }
            }

            if (!string.IsNullOrEmpty(hata))
            {
                s.DegerAta(hata);
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCETASNIFHIYERARSI tabTasHiyer = new TABBHMBUTCETASNIFHIYERARSI(ve);
            TABBHMBUTCETASNIFTERTIPSABLON tabTasHiyerTertip = new TABBHMBUTCETASNIFTERTIPSABLON(ve);
            TABBHMBUTCETASNIFODENEK tabTasHiyerOdenek = new TABBHMBUTCETASNIFODENEK(ve);

            ve.BeginTransaction();

            // Eskiden kodu olup da şimdi olmayanları sil
            for (int i = 0; i < hiyerarsi.Length && s.islemSonuc; i++)
            {
                if (string.IsNullOrEmpty(hiyerarsi[i].yeniKod) && !string.IsNullOrEmpty(hiyerarsi[i].kod))
                {
                    s = tabTasHiyerTertip.Sil(tasnifSiraNo, hiyerarsi[i].kod);
                    if (s.islemSonuc)
                        s = tabTasHiyerOdenek.Sil(tasnifSiraNo, hiyerarsi[i].kod);
                    if (s.islemSonuc)
                        s = tabTasHiyer.Sil(tasnifSiraNo, hiyerarsi[i].kod);
                    if (s.islemSonuc)
                        hiyerarsi[i].kod = "@" + hiyerarsi[i].kod;
                }
            }

            // Eskiden kodu olup da şimdi de olanları güncelle
            while (true)
            {
                int i = DegisecekSatirBul(hiyerarsi);
                if (i >= 0)
                {
                    s = tabTasHiyerTertip.Sil(tasnifSiraNo, hiyerarsi[i].kod);
                    if (s.islemSonuc)
                        s = tabTasHiyerTertip.Guncelle(tasnifSiraNo, hiyerarsi[i]);
                    for (int j = 0; j < hiyerarsi[i].tertipSablonlar_Length() && s.islemSonuc; j++)
                    {
                        s = tabTasHiyerTertip.Ekle(tasnifSiraNo, hiyerarsi[i], j);
                    }

                    if (s.islemSonuc)
                        s = tabTasHiyerOdenek.Guncelle(tasnifSiraNo, hiyerarsi[i]);
                    if (s.islemSonuc)
                        s = tabTasHiyer.Guncelle(tasnifSiraNo, hiyerarsi[i]);
                    if (s.islemSonuc)
                        hiyerarsi[i].kod = "@" + hiyerarsi[i].kod;
                    else
                        break;
                }
                else
                    break;
            }

            // Eskiden kodu olmayıp şimdi olanları ekle
            for (int i = 0; i < hiyerarsi.Length && s.islemSonuc; i++)
            {
                if (!string.IsNullOrEmpty(hiyerarsi[i].yeniKod) && string.IsNullOrEmpty(hiyerarsi[i].kod))
                {
                    s = tabTasHiyer.Ekle(tasnifSiraNo, hiyerarsi[i]);
                    for (int j = 0; j < hiyerarsi[i].tertipSablonlar_Length() && s.islemSonuc; j++)
                    {
                        s = tabTasHiyerTertip.Ekle(tasnifSiraNo, hiyerarsi[i], j);
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
        /// Bir satırın eski kodu ve yeni kodu varsa ve bu ikisi birbirinden farklı ise güncelleme işlemi yapılır.
        /// (-1 döndürme durumu hatalı algoritma olduğunda oluşur. Bu noktaya gelmek için bir satırın eski kodunun ve yeni kodunun aynı olmaması gerekir.)
        /// Satırlara yeni kodlar verildiğinde, doğal olara veri tabanında halen kayıtlı verilerle çakışmalar olacaktır.
        /// Bu durumda güncelleme yapılırken, öncelikle veri tabanında kayıtlı olmayan kodlar güncellenmeli ve böylece update komutu sırasında hata alınmamalıdır.
        /// Örnek
        ///    Eski kodlar    Yeni Kodlar
        ///        01            01  
        ///        02            03
        ///        03            04
        ///        04            05
        /// Yukarıdaki örnekte işlem sırasında önce 02'leri 03 yaparsanız, veri tabanında eski 03'lerde olduğu için çakışma olur.
        /// Bu yüzden dizi gezilir ve yeni değer 03 için, eski değeri 03 ola satır var mı diye bakılır. Varsa Öncelik o satıra geçer.
        /// O satırında yeni kodu 04'ün eski kodlu satırı olduğu için, öncelik eski kodu 04 olan satıra geçer. 
        /// Bu satırda yeni kod 05'dir ve bu kodun eski kodu olmadığından tüm eski 04'ler 05 yapılır. 
        /// Artık veri tabanında 04'lü eski kayıt olmadığından, 03'ler 04 yapılabilinir. (İşlem yapılan eski kodların başına @ konularak veri tabanında olmadığı işareti konulmuş olunur.)
        /// </summary>
        /// <param name="hiyerarsi">İlişkili satırını bulunacağı hiyerarşik satır dizisi</param>
        /// <param name="kaynakSatir">İlişkili satırın bulunacağı ve güncelleme yapılması planlanan satır</param>
        /// <returns>Güncelleme yapılması planlanan satırın ilişkili satırı varsa ilişkili satırın numarası, yoksa -1 döndürülür.</returns>
        public static int IliskiliSatirBul(ButceTasnifHiyerarsikSatir[] hiyerarsi, int kaynakSatir)
        {
            string kaynakKod = hiyerarsi[kaynakSatir].yeniKod;
            if (kaynakKod == hiyerarsi[kaynakSatir].kod) return -1;
            for (int i = 0; i < hiyerarsi.Length; i++)
            {
                if (kaynakKod == hiyerarsi[i].kod)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Değişecek satır bulma yordamı, eski kodu da, yeni kodu da dolu olan ve birbirinden farklı olan, 
        /// ilişkili satırı bulunmayan bir satırı değişmek üzere döndürür. 
        /// Döndürülen satırın, eski kodunun başına @ işareti konularak bu satırın değiştiği bilgisi tutulur. (Daha sonraki ilişki satır aranırken bulunmaması için)
        /// </summary>
        /// <param name="hiyerarsi">Değiştirilecek satırın bulunacağı hiyerarşik satır dizisi.</param>
        /// <returns>Değiştirilecek satır bulunmuşsa, dizideki indisi döndürülür, bulunmamışsa -1 döndürülür.</returns>
        public static int DegisecekSatirBul(ButceTasnifHiyerarsikSatir[] hiyerarsi)
        {
            for (int i = 0; i < hiyerarsi.Length; i++)
            {
                if (!string.IsNullOrEmpty(hiyerarsi[i].yeniKod) && !string.IsNullOrEmpty(hiyerarsi[i].kod))
                {
                    if (!hiyerarsi[i].kod.StartsWith("@"))
                    {
                        int degisecekSatir = i;
                        while (true)
                        {
                            int yeniSatir = IliskiliSatirBul(hiyerarsi, degisecekSatir);
                            if (yeniSatir == -1)
                            {
                                return degisecekSatir;
                            }
                            degisecekSatir = yeniSatir;
                        }
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Verilen indis satırından başlayarak, hiyerarşik dizideki bir sonraki yeni kodu dolu olan satırı bulan yordam.
        /// </summary>
        /// <param name="hiyerarsi">Bir sonraki yeni kodu dolu olan satırın bulunacağı dizi.</param>
        /// <param name="sat">Kendisinden sonraki yeni kodu dolu olan satırın bulunacağı satırın indisi.</param>
        /// <returns>Yeni kodu dolu olan ve verilen satırdan sonra gelen satırın indisi döndürülür. Bulunamazsa -1 döndürülür.</returns>
        private static int SonrakiSatBul(ButceTasnifHiyerarsikSatir[] hiyerarsi, int sat)
        {
            for (int i = sat + 1; i < hiyerarsi.Length; i++)
            {
                if (!string.IsNullOrEmpty(hiyerarsi[i].yeniKod))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Hiyerarşik seviyelerin son numaralarını tutan seviye dizisindeki bu numaralar, iki basamaklı dizgiye çevrilerek birleştirilir ve döndürülür.
        /// </summary>
        /// <param name="seviye">Verilen seviye dizisindeki son numaralar, verilen seviyeye kadar iki basamaklı dizgiye çevrilerek birleştirilir.</param>
        /// <param name="seviyeDizisi">Her bir seviye için o seviyenin son numarasını tutan int dizi.</param>
        /// <returns>Hiyerarşik seviyele dizisindeki numaralar, iki basamaklı dizgiye çevrilerek birleştirilir ve döndürülür.</returns>
        public static string StrKodaCevir(int seviye, ArrayList seviyeDizisi)
        {
            string donDeger = string.Empty;
            for (int i = 0; i < seviye; i++)
            {
                if (donDeger != string.Empty) donDeger += ".";
                int tut = (int)seviyeDizisi[i];
                string tutS = "";
                if (tut < 100)
                    tutS = tut.ToString("00");
                else
                {
                    int kalan = tut % 10;
                    tut = ((tut - kalan) / 10) - 10;
                    char c = (char)(65 + tut);
                    tutS = c + kalan.ToString("0");
                }
                donDeger += tutS;
            }
            return donDeger;
        }

        /// <summary>
        /// Seviye dizisindeki, verilen seviyedeki numara bir arttırılır. Bu seviyenin numarası bir arttığı için kendisinden daha aşağıdaki seviyelerin numaraları sıfırlanır.
        /// Arttırılan numara döndürülür.
        /// </summary>
        /// <param name="seviye">Numarası bir arttırılacak seviye.(Seviyeler 1'den başladığı, diziler 0'dan başladığı için 1 eksiği ile diziye erişilir.)</param>
        /// <param name="seviyeDizisi">Tüm seviyelerin son numaralarını tutan dizi.</param>
        /// <returns>Numarası arttırılan seviyenin, yeni numarası döndürülür.</returns>
        public static int KodArttir(int seviye, ArrayList seviyeDizisi)
        {
            for (int i = seviye; i < seviyeDizisi.Count; i++)
            {
                seviyeDizisi[i] = (int)0;
            }
            seviyeDizisi[seviye - 1] = ((int)seviyeDizisi[seviye - 1]) + 1;
            return (int)seviyeDizisi[seviye - 1];
        }

        /// <summary>
        /// Hşyerarşik dizide, verilen dizi satırının içerleklik seviyesine göre, satıra yeni kod verilir.
        /// Yeni kod verilirken var olan seviyenin numarası bir arttırılır ve tüm dizi dizgiye çevrilerek yeni numara verilir.
        /// Örnek
        ///           Seviyeler    Seviye Dizisi   Kodlar    
        ///              1               1           01
        ///              1               2           02
        ///              2               2,1         02.01
        ///              2               2,2         02.02
        ///              3               2,2,1       02.02.01
        ///              2               2,3,0       02.03
        ///              1               3,0,0       03
        /// İlk çağrı yapıldığında (seviye: 1) seviye dizisi boştur, 1. seviye için diziye bir eleman eklenir, bir arttırılır dizgiye çevrilir.
        /// İkinci çağrı yapıldığında (seviye: 1) seviye dizisi doludur, 1. seviye bir arttırılır dizgiye çevrilir.
        /// Üçüncü çağrı yapıldığında (seviye: 2) seviye dizisinin ikinci boyutu boştur, ikinci seviye eklenir, bir arttırılır, dizgiye çevrilir.
        /// Dördüncü çağrı yapıldığında (seviye: 2) seviye dizisinin ikinci boyutu doludur, bir arttırılır, dizgiye çevrilir.
        /// Beşinci çağrı yapıldığında (seviye: 3) seviye dizisinin üçüncü boyutu boştur, üçüncü seviye eklenir, bir arttırılır, dizgiye çevrilir.
        /// Altıncı çağrı yapıldığında (seviye: 2) seviye dizisinin ikinci boyutu doludur, fakat artık sonraki boyutlar olduğundan sonraki boyutlar sıfırlanır. İkinci seviye bir arttırılır, dizgiye çevrilir.
        /// Yedinci çağrı yapıldığında (seviye: 1) seviye dizisinin birinci boyutu doludur, fakat artık sonraki boyutlar olduğundan sonraki boyutlar sıfırlanır. Birinci seviye bir arttırılır, dizgiye çevrilir.
        /// </summary>
        /// <param name="butceTasnifHiyerarsi">Seviye bilgisi tutulan hiyerarşik dizi satırı</param>
        /// <param name="seviyeDizisi"></param>
        /// <returns></returns>
        public static string HiyerarsiKodBul(ButceTasnifHiyerarsikSatir butceTasnifHiyerarsi, ArrayList seviyeDizisi)
        {
            if (string.IsNullOrEmpty(butceTasnifHiyerarsi.ad))
                return string.Empty;
            while (butceTasnifHiyerarsi.seviye > seviyeDizisi.Count)
            {
                seviyeDizisi.Add((int)0);
            }

            KodArttir(butceTasnifHiyerarsi.seviye, seviyeDizisi);

            return StrKodaCevir(butceTasnifHiyerarsi.seviye, seviyeDizisi);
        }

        /// <summary>
        /// Sıra numarası verilen tasnife ait, hiyerarşik satır bilgilerini ve her bir satırın verilen yıla ait tertip şablonlarını döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifSiraNo">Hiyerarşi satırları döndürülecek tasnifin sıra numarası</param>
        /// <param name="yil">Her bir satırdaki tertip şablonunun yıldan yıla değişebileceği tasarlanmıştır. Döndürülecek satırlardaki tertip şablonlarının hangi yıla ait olduğu bilgisi.</param>
        /// <param name="hiyerarsiKod">Dolu ise sadece ilgili satır okunur.</param>
        /// <returns>Sıra numarası verilen tasnife ait hiyerarşik satır bilgileri dizi olarak döndürülür.</returns>
        public static ButceTasnifHiyerarsikSatir[] Listele(VeriErisim veriErisim, Kullanici kullanan, int tasnifSiraNo, int yil, string hiyerarsiKod, string kistasTertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            VeriErisim veIc = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCETASNIFHIYERARSI tabTasnifHiyerarsi = new TABBHMBUTCETASNIFHIYERARSI(ve);
            IDataReader dr;
            dr = tabTasnifHiyerarsi.Listele(tasnifSiraNo, hiyerarsiKod);

            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                ButceTasnifHiyerarsikSatir formIc = new ButceTasnifHiyerarsikSatir();
                formIc.kod = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.KOD, string.Empty);
                formIc.kod = TNS.OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99.99.99.99", formIc.kod);
                formIc.ad = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.AD, string.Empty);
                formIc.altKirilimVar = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.ALTKIRILIMVAR, 0) == 1;
                formIc.tertipSablonlar = TNS.BHMv2.IsKurallari.BHMv2ButceTasnifTertip.Oku(veIc, kullanan, tasnifSiraNo, formIc.kod, yil);
                if (KistasTertipleUyumluMu(formIc, kistasTertip))
                {
                    al.Add(formIc);
                }
            }
            dr.Close();
            ve.VeriErisimKapat();
            veIc.VeriErisimKapat();
            if (string.IsNullOrEmpty(hiyerarsiKod))
            {
                int yeni;
                CocuksuzlariListedenCikart(al, 0, out yeni);
                for (int i = al.Count - 1; i >= 0; i--)
                {
                    if (al[i] == null)
                        al.RemoveAt(i);
                }
            }
            return (ButceTasnifHiyerarsikSatir[])al.ToArray(typeof(ButceTasnifHiyerarsikSatir));
        }

        private static int CocuksuzlariListedenCikart(ArrayList al, int yer, out int yeni)
        {
            yeni = -1;
            if (yer < 0) return 0;
            if (yer >= al.Count) return 0;
            int cocukSay = 0;
            ButceTasnifHiyerarsikSatir gelenSatir = (ButceTasnifHiyerarsikSatir)al[yer];
            while (yer < al.Count)
            {
                ButceTasnifHiyerarsikSatir islemSatir = (ButceTasnifHiyerarsikSatir)al[yer];
                if (gelenSatir.kod.Length > islemSatir.kod.Length)
                {
                    yeni = yer;
                    return cocukSay;
                }
                if (islemSatir.altKirilimVar)
                {
                    int sonraki = yer + 1;
                    int altCocukSay = 0;
                    if (sonraki < al.Count)
                    {
                        ButceTasnifHiyerarsikSatir sonrakiSatir = (ButceTasnifHiyerarsikSatir)al[sonraki];
                        if (sonrakiSatir.kod.Length > islemSatir.kod.Length)
                            altCocukSay += CocuksuzlariListedenCikart(al, sonraki, out sonraki);
                    }
                    if (altCocukSay == 0)
                    {
                        al[yer] = null;
                    }
                    yer = sonraki;
                    cocukSay += altCocukSay;
                }
                else
                {
                    cocukSay++;
                    yer++;
                }
            }
            yeni = yer;
            return cocukSay;
        }

        private static bool KistasTertipleUyumluMu(ButceTasnifHiyerarsikSatir formIc, string kistasTertip)
        {
            if (string.IsNullOrEmpty(kistasTertip))
                return true;
            for (int i = 0; i < formIc.tertipSablonlar_Length(); i++)
            {
                ButceTasnifTertipSablon btts = formIc.tertipSablonlar[i];
                if (btts.ekleCikar == TertipEkleCikar.Ekle)
                {
                    string[] sablonBolum = btts.tertipSablon.Split('-');
                    string[] kistasTertipBolum = kistasTertip.Split('-');
                    if (kistasTertipBolum.Length > sablonBolum.Length) return false;
                    for (int j = 0; j < Math.Max(kistasTertipBolum.Length, sablonBolum.Length); j++)
                    {
                        if (!TertipBolumUyumluMu(kistasTertipBolum, sablonBolum, j)) return false;
                    }
                }
            }
            return true;
        }

        private static bool TertipBolumUyumluMu(string[] kistasTertipBolumler, string[] sablonBolumler, int yer)
        {
            string kistasTertipBolum = null;
            string sablonBolum = null;

            if (yer < kistasTertipBolumler.Length) kistasTertipBolum = kistasTertipBolumler[yer];
            if (yer < sablonBolumler.Length) sablonBolum = sablonBolumler[yer];

            if (kistasTertipBolum == null) return true;
            if (string.IsNullOrEmpty(sablonBolum)) return true;
            if (kistasTertipBolum == sablonBolum) return true;
            if (sablonBolum.StartsWith(kistasTertipBolum)) return true;

            string[] kistasTertipAltBolumler = kistasTertipBolum.Split(',');
            string[] sablonBolumAltBolumler = sablonBolum.Split(',');
            for (int i = 0; i < kistasTertipAltBolumler.Length; i++)
            {
                for (int j = 0; j < sablonBolumAltBolumler.Length; j++)
                {
                    if (sablonBolumAltBolumler[j].StartsWith(kistasTertipAltBolumler[i])) return true;
                }
            }
            return false;
        }
    }
}


/*

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="kullanan"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        //internal static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ButceTasnifHiyerarsi form)
        //{
        //    Sonuc s = new Sonuc(true, string.Empty, string.Empty);
        //    if (!YazmaYetkisiVarMi(kullanan))
        //    {
        //        s.DegerAta("Yazma yetkiniz yok!");
        //        return s;
        //    }

        //    s = AlanlarGecerliMi(kullanan, form);
        //    if (!s.islemSonuc)
        //    {
        //        return s;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
        //    TABBHMBUTCETASNIFHIYERARSI tabButceTasnifHiyerarsi = new TABBHMBUTCETASNIFHIYERARSI(ve);
        //    TABBHMBUTCETASNIFTERTIP tabButceTasnifTertip = new TABBHMBUTCETASNIFTERTIP(ve);

        //    ve.BeginTransaction();

        //    if (s.islemSonuc)
        //        s = tabButceTasnifTertip.Sil(form);

        //    if (s.islemSonuc)
        //        s = tabButceTasnifHiyerarsi.Sil(form);

        //    if (s.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return s;
        //}


        /// <summary>
        /// ButceTasnifHiyerarsi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ButceTasnifHiyerarsi yapısında ButceTasnifHiyerarsi: bilgilerini tutan değişken</param>
        //public static ButceTasnifHiyerarsi Oku(VeriErisim veriErisim, Kullanici kullanan, ButceTasnifHiyerarsi form)
        //{
        //    if (!GormeYetkisiVarMi(kullanan, form))
        //    {
        //        throw new Exception("Görme yetkiniz yok!");
        //    }

        //    Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
        //    if (!s.islemSonuc)
        //    {
        //        throw new Exception(s.hataStr);
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
        //    TABBHMBUTCETASNIFHIYERARSI tabBHMBUTCETASNIFHIYERARSI = new TABBHMBUTCETASNIFHIYERARSI(ve);

        //    ButceTasnifHiyerarsi formIc = new ButceTasnifHiyerarsi();
        //    IDataReader dr;
        //    dr = tabBHMBUTCETASNIFHIYERARSI.Oku(form);
        //    if (dr == null)
        //    {
        //        ve.VeriErisimKapat();
        //        throw new Exception(ve.hata);
        //    }
        //    if (dr.Read())
        //    {
        //        formIc.butceTasnif.siraNo = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.TASNIFNO, 0);
        //        formIc.kod = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.KOD, string.Empty);
        //        formIc.kod = TNS.OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99.99", formIc.kod);

        //        formIc.ad = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.AD, string.Empty);
        //        formIc.altKirilimVar = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.ALTKIRILIMVAR, 0);
        //    }
        //    else
        //    {
        //        dr.Close();
        //        ve.VeriErisimKapat();
        //        throw new Exception("Aranan Kayıt bulunamadı!");
        //    }
        //    dr.Close();

        //    try
        //    {
        //        formIc.butceTasnif = TNS.BHMv2.IsKurallari.ButceTasnifIslemler.Oku(ve, kullanan, formIc.butceTasnif);
        //    }
        //    catch { };

        //    ve.VeriErisimKapat();
        //    return formIc;
        //}


        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, ButceTasnifHiyerarsikSatir form)
        {
            return new Sonuc(true, string.Empty, string.Empty);
        }


        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, ButceTasnifHiyerarsikSatir form)
        {
            return true;
        }



*/
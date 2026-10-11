using System;
using System.Collections.Generic;
using System.Text;
using TNS.KYM;
using TNS.Veri;
using System.Data;
using TNS.BHF.IsKurallari.Veri;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Birimin hizmet gerekçesi metni ile ilgili işlemleri tutan sınıf.
    /// </summary>
    public class HizmetGerekcesiServis
    {
        /// <summary>
        /// Birime ait hizmet gerekçesini döndüren yordam.
        /// Veri tabanına parçalı olarak yazılmış gerekçeler okunur ve birleştirilerek döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Hizmet gerekçesi metni ve sonuç yapısı döndürülür.</returns>
        public static FaaliyetGerekce FaaliyetGerekceOku(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int faaliyetKod)
        {
            ObjectArray oa = FaaliyetGerekceListele(veriErisim, kullanan, yil, kurumsalKod, faaliyetKod, false);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
            {
                return (FaaliyetGerekce)oa.objeler[0];
            }

            FaaliyetGerekce faalGer = new FaaliyetGerekce();
            faalGer.sonuc = oa.sonuc;
            return faalGer;
        }

        public static Sonuc FaaliyetGerekceBirlestir(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            ObjectArray oa = FaaliyetGerekceListele(veriErisim, kullanan, yil, kurumsalKod, 0, true);
            if (!oa.sonuc.islemSonuc) return oa.sonuc;
            foreach (FaaliyetGerekce item in oa.objeler)
            {
                FaaliyetGerekce okunan = FaaliyetGerekceOku(veriErisim, kullanan, yil, item.kurKod, item.faalKod);
                if (!okunan.sonuc.islemSonuc) return okunan.sonuc;
                if (string.IsNullOrWhiteSpace(okunan.gerekce))
                {
                    Sonuc s = FaaliyetGerekcesiKaydet(veriErisim, kullanan, yil, item.kurKod, item.faalKod, item);
                    if (!s.islemSonuc) return s;
                }
            }
            return oa.sonuc;
        }

        public static ObjectArray FaaliyetGerekceListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int faaliyetKod, bool faaliyeteGoreGrupla)
        {
            ObjectArray donusDeger = new ObjectArray();
            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                IKYMServis kymServis = TNS.KYM.Arac.Tanimla();
                ObjectArray oa = kymServis.BuyukMetinListele(kullanan, "BHF", GerekceAnahtarOl(yil, kurumsalKod, faaliyetKod, faaliyeteGoreGrupla), "tr");
                if (oa.sonuc.islemSonuc)
                {
                    FaaliyetGerekce faalGer = null;
                    foreach (StrKodStrAd item in oa.objeler)
                    {
                        string kurKod;
                        int faalKod;
                        int alanKod;
                        MetinKodParcala(item.kod, out kurKod, out faalKod, out alanKod);
                        if (faalGer == null || faalGer.kurKod != kurKod || faalGer.faalKod != faalKod)
                        {
                            faalGer = new FaaliyetGerekce();
                            faalGer.kurKod = kurKod;
                            faalGer.faalKod = faalKod;
                            donusDeger.Ekle(faalGer);
                        }

                        if (alanKod == 1)
                            faalGer.gerekce = OrtakFonksiyonlar.RemoveHTML(item.ad);
                        else if (alanKod == 2)
                            faalGer.dayanak = OrtakFonksiyonlar.RemoveHTML(item.ad);
                        else if (alanKod == 3)
                            faalGer.maliyet = OrtakFonksiyonlar.RemoveHTML(item.ad);
                    }
                }
                else
                    donusDeger.sonuc = oa.sonuc;
                if (faaliyeteGoreGrupla)
                {
                    donusDeger.objeler.Sort(new FaaliyetGerekce.Sirala(2));

                    ObjectArray yeniListe = new ObjectArray();
                    FaaliyetGerekce fgGit = null;
                    foreach (FaaliyetGerekce fg in donusDeger.objeler)
                    {
                        if (fg.kurKod == kurumsalKod.Replace(".", "")) continue;
                        if (fgGit == null || fgGit.faalKod != fg.faalKod)
                        {
                            if (fgGit != null)
                                yeniListe.Ekle(fgGit);
                            fgGit = new FaaliyetGerekce();
                            fgGit.kurKod = kurumsalKod;
                            fgGit.faalKod = fg.faalKod;
                        }
                        Birlestir(fgGit, fg);
                    }
                    if (fgGit != null)
                        yeniListe.Ekle(fgGit);
                    donusDeger = yeniListe;
                }
            }
            else
            {
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFHGS001);
            }
            return donusDeger;
        }
        private static void Birlestir(FaaliyetGerekce fgGit, FaaliyetGerekce fg)
        {
            fgGit.gerekce = AciklamaBirlestir(fgGit.gerekce, fg.gerekce);
            fgGit.maliyet = AciklamaBirlestir(fgGit.maliyet, fg.maliyet);
            fgGit.dayanak = AciklamaBirlestir(fgGit.dayanak, fg.dayanak);
        }

        private static string AciklamaBirlestir(string metin1, string metin2)
        {
            if (string.IsNullOrWhiteSpace(metin1)) return metin2;
            if (string.IsNullOrWhiteSpace(metin2)) return metin1;
            return metin1 + "\r\n" + metin2;
        }

        private static void MetinKodParcala(string kod, out string kurKod, out int faalKod, out int alanKod)
        {
            string[] alanlar = kod.Split('_');
            kurKod = OrtakFonksiyonlar.DiziElemanAl(alanlar, 2, "");
            faalKod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(alanlar, 3, ""), 0);
            alanKod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(alanlar, 4, ""), 0);
        }

        private static string GerekceAnahtarOl(int yil, string kurumsalKod, int faaliyetKod, string alanKod)
        {
            return $"FAALGER_{yil}_{kurumsalKod.Replace(".", "")}_{faaliyetKod}_{alanKod}";
        }

        private static string GerekceAnahtarOl(int yil, string kurumsalKod, int faaliyetKod, bool faaliyeteGoreGrupla)
        {
            string tut = $"FAALGER_{yil}_{kurumsalKod.Replace(".", "")}";
            if (kurumsalKod.Length < 6) tut = "%";
            if (!faaliyeteGoreGrupla)
            {
                tut += "_";
                if (faaliyetKod > 0)
                {
                    tut += $"{faaliyetKod}_";
                }
            }
            return tut;
        }
        /// <summary>
        /// Birime ait gerekçe metnini kaydeden yordam.
        /// Yetki kontrolü yapıldıktan sonra, eski gerekçe metni paraçaları silinir. Sonra yeni gerekçe metni 250 karekterlik parçalara ayrılır ve her parça kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="gerekce">Kaydedilecek hizmet gerekçesi metni.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc FaaliyetGerekcesiKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int faaliyetKod, FaaliyetGerekce gerekce)
        {
            if (yil <= 0 || kurumsalKod == "" || faaliyetKod <= 0)
            {
                return new Sonuc(false, ButceFormResI.BHFHGS002, "");
            }
            else
            {
                if (Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                {
                    IKYMServis kymServis = TNS.KYM.Arac.Tanimla();
                    TNSCollection liste = new TNSCollection();
                    liste.Add(new StrKodStrAd(GerekceAnahtarOl(yil, kurumsalKod, faaliyetKod, "1"), gerekce.gerekce));
                    liste.Add(new StrKodStrAd(GerekceAnahtarOl(yil, kurumsalKod, faaliyetKod, "2"), gerekce.dayanak));
                    liste.Add(new StrKodStrAd(GerekceAnahtarOl(yil, kurumsalKod, faaliyetKod, "3"), gerekce.maliyet));
                    return kymServis.BuyukMetinListeSakla(kullanan, "BHF", "tr", liste);
                }
                else
                    return new Sonuc(false, ButceFormResI.BHFHGS003, "");
            }
        }

        /// <summary>
        /// Hizmet gerekçesi girmiş olan birim sayısını bulup döndüren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="yil">Gerekçe sayısı bulunacak yıl.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesi sayısının bulunacağı birimin kurumsal kodu.</param>
        /// <returns>Hizmet gerekçesi sayısı döndürülür.</returns>
        internal static int HizmetGerekcesiSayisiBul(VeriErisim veriErisim, int yil, string kurumsalKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));

            TABHizmetGerekcesi thg = new TABHizmetGerekcesi(ve);

            int sonuc = thg.HizmetGerekcesiSayisiBul(yil, kurumsalKod.Replace(".", ""));

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Hizmet gerekçelerini bir önceki yıldan, alıp bu yıla kopyalayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin kopyalanacağı hedef yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin kopyalanacağı birimin kurumsal kodu.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        internal static Sonuc HizmetGerekcesiKopyalaOncYil(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            if (yil <= 0 || kurumsalKod == "")
            {
                return new Sonuc(false, ButceFormResI.BHFHGS002, "");
            }
            else
            {
                if (Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                {
                    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

                    TABHizmetGerekcesi thg = new TABHizmetGerekcesi(ve);
                    ve.BeginTransaction();
                    Sonuc sonuc = thg.HizmetGerekcesiKopyalaOncYil(yil, kurumsalKod.Replace(".", ""));
                    if (sonuc.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
                else
                    return new Sonuc(false, ButceFormResI.BHFHGS003, "");
            }
        }
    }
}

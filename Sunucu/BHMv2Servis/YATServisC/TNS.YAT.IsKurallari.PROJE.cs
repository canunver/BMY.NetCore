using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;
using System.Collections;
using TNS.BHMv2;
using TNS.BKD;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Yatırım proje bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class ProjeClass
    {
        public static bool KapaliMi(BHMv2.IBHMv2Servis servis, Kullanici kullanan, int yil, string senaryoKod)
        {
            BKD.TavanGenel tg = new BKD.TavanGenel();
            tg.kod1 = "";
            tg.kod2 = "";
            ObjectArray oaTanim = servis.BHMTavanGenelOnayListele(kullanan, yil, senaryoKod, TNS.BKD.ENUMTavanTur.YATProjeTanim, tg);
            if (oaTanim.sonuc.islemSonuc && oaTanim.ObjeSayisi == 1)
            {
                tg = (TNS.BKD.TavanGenel)oaTanim.objeler[0];
                if (tg.durum == 1) return false;
                if (tg.durum == 4 && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ProjeGormeHakkiVarMi(VeriErisim ve, Kullanici kullanan, Proje p)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            TABYATALTPROJEBILGI tab = new TABYATALTPROJEBILGI(ve);
            int say = tab.YetkiliKullaniciSay(kullanan.KullaniciKodu, p.kod, 0);
            if (say != 1) return false;
            return true;
        }

        public static bool ProjeYazmaHakkiVarMi(VeriErisim ve, Kullanici kullanan, Proje p, int yil, string senaryoKod)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            TABYATALTPROJEBILGI tab = new TABYATALTPROJEBILGI(ve);
            int say = tab.YetkiliKullaniciSay(kullanan.KullaniciKodu, p.kod, 0);
            if (say != 1) return false;
            return true;
        }

        public static Sonuc ProjeAltProjeYazmaHakkiVarMi(VeriErisim ve, Kullanici kullanan, int projeKodu, int altProjeKodu, int yil, string senaryoKod)
        {
            return ProjeAltProjeYazmaGormeHakkiVarMi(ve, kullanan, projeKodu, altProjeKodu, yil, senaryoKod, BHMv2.ENUMOGY.Yazma);
        }

        public static Sonuc ProjeAltProjeYazmaGormeHakkiVarMi(VeriErisim ve, Kullanici kullanan, int projeKodu, int altProjeKodu, int yil, string senaryoKod, TNS.BHMv2.ENUMOGY ogy, bool hazirlik = true)
        {
            if (ogy != BHMv2.ENUMOGY.Gorme && hazirlik)
            {
                TNS.BHMv2.IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                if (KapaliMi(servis, kullanan, yil, senaryoKod)) return new Sonuc("Veri giriş dönemi kapatıldığından kayıt işlemi yapamazsınız!");
            }
            if (!AsamaYetkisiVarMi(kullanan, senaryoKod, ogy))
                return new Sonuc("Verilen aşamayla ilgili işlem yapma yetkiniz bulunmamaktadır!");

            if (altProjeKodu <= 0)
            {
                Proje p = new Proje();
                p.kod = projeKodu;
                BilgiVAN bv = ProjeClass.Listele(ve, kullanan, yil, senaryoKod, p, null, false);
                if (!bv.sonuc.islemSonuc)
                    return bv.sonuc;
                if (bv.liste.Count != 1)
                    return new Sonuc("Verilen proje okunamadı!");
                p = (Proje)bv.liste[0];

                Sonuc donusDeger = Yetki.ProjeGormeYazmaHakkiVarMi(kullanan, p.yurutecekKurum, ogy);
                if (donusDeger.islemSonuc)
                    return null;
                bool sonuc = ProjeClass.ProjeYazmaHakkiVarMi(ve, kullanan, p, yil, senaryoKod);
                if (!sonuc)
                    return new Sonuc("Yazma hakkınız yok!");
            }
            else
            {
                AltProje ap = new AltProje();
                ap.proje.kod = projeKodu;
                ap.kod = altProjeKodu;
                BilgiVAN bv = AltProjeClass.Listele(ve, kullanan, yil, senaryoKod, ap, null, false, false);
                if (!bv.sonuc.islemSonuc)
                    return bv.sonuc;
                if (bv.liste.Count != 1)
                    return new Sonuc("Verilen alt proje okunamadı!");
                ap = (AltProje)bv.liste[0];
                Sonuc donusDeger = Yetki.ProjeYazmaHakkiVarMi(kullanan, ap.birim.kod);
                if (donusDeger.islemSonuc)
                    return null;

                bool sonuc = AltProjeClass.AltProjeYazmaHakkiVarMi(ve, kullanan, ap, yil, senaryoKod);
                if (!sonuc)
                    return new Sonuc("Yazma hakkınız yok!");
            }
            return null;
        }

        private static bool AsamaYetkisiVarMi(Kullanici kullanan, string senaryoKod, ENUMOGY ogy)
        {
            return true;
            //"Bütçe Hazırlığı Görebileceği Aşamalar", "Bütçe Hazırlığı Kaydedebileceği Aşamalar", "Bütçe Hazırlığı Onaylayabileceği Aşamalar"
            //if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;

            //string asama;
            //if (ogy == ENUMOGY.Yazma)
            //    asama = kullanan.YetkiTipiDegeriBul("Bütçe Hazırlığı Kaydedebileceği Aşamalar");
            //else if (ogy == ENUMOGY.Gorme)
            //    asama = kullanan.YetkiTipiDegeriBul("Bütçe Hazırlığı Görebileceği Aşamalar");
            //else
            //    asama = kullanan.YetkiTipiDegeriBul("Bütçe Hazırlığı Onaylayabileceği Aşamalar");

            //string[] asamadegerler = asama.Split(';');
            //foreach (string asamalar in asamadegerler)
            //{
            //    if ((asamalar != string.Empty) && (OrtakFonksiyonlar.ConvertToInt(senaryoKod.Substring(0, 2), 0) == OrtakFonksiyonlar.ConvertToInt(asamalar, 0))) return true;
            //}
            //return false;
        }

        /// <summary>
        /// Yatırım projesi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen proje nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="proje">Yatırım projesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, Proje proje, bool yeni)
        {
            if (string.IsNullOrWhiteSpace(proje.yurutecekKurum))
            {
                return new Sonuc("Lütfen sorumlu birimi seçiniz!");
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc donusDeger = Yetki.ProjeYazmaHakkiVarMi(kullanan, proje.yurutecekKurum);
            if (!donusDeger.islemSonuc)
            {
                if (proje.kod > 0)
                {
                    donusDeger = ProjeAltProjeYazmaHakkiVarMi(ve, kullanan, proje.kod, 0, yil, senaryoKod);
                    if (donusDeger == null) donusDeger = new Sonuc();
                }
            }
            else
            {
                TNS.BHMv2.IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                if (KapaliMi(servis, kullanan, yil, senaryoKod)) donusDeger.DegerAta("Veri giriş dönemi kapatıldığından kayıt işlemi yapamazsınız!");
            }
            if (string.IsNullOrWhiteSpace(proje.DPTProjeNo))
            {
                donusDeger.DegerAta(YatirimResI.BHMPD047 + "<br>");
            }
            else if (proje.DPTProjeNo.Length < 5)
                donusDeger.DegerAta(YatirimResI.BHMPD048 + "<br>");
            else
            {
                int dptYil = OrtakFonksiyonlar.ConvertToInt(proje.DPTProjeNo.Substring(0, 4), 0);
                if (!(dptYil > 0))
                    donusDeger.DegerAta(YatirimResI.BHMPD049 + "<br>");
            }

            if (proje.model == 0)
                donusDeger.DegerAta(YatirimResI.BHMPD044 + "<br>");

            if (proje.tur.kod == 0)
                donusDeger.DegerAta(YatirimResI.BHMPD045 + "<br>");

            if (string.IsNullOrWhiteSpace(proje.sektor.kod))
                donusDeger.DegerAta(YatirimResI.BHMPD046 + "<br>");


            if (proje.ad.Trim() == "")
                donusDeger.DegerAta(YatirimResI.YAT010 + "<br>");

            if (proje.baslangicYili <= 0)
                donusDeger.DegerAta(YatirimResI.YAT011 + "<br>");

            if (proje.bitisYili <= 0)
                donusDeger.DegerAta(YatirimResI.YAT009 + "<br>");

            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }
            TABYATProje tabProje = new TABYATProje(ve);

            if (!Genel.ProjeTurVarMi(ve, proje.tur.kod))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT014;
            }

            if (!Genel.SektorVarMi(ve, yil, proje.sektor.kod))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT047;
            }

            if (Genel.AltSektorVarMi(ve, yil, proje.sektor))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT048;
            }

            //if (!Genel.IlceVarMi(ve, proje.ilce))
            //{
            //    donusDeger.islemSonuc = false;
            //    donusDeger.hataStr = "İlçe bilgisi yanlış!";
            //}

            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }
            ve.BeginTransaction();
            try
            {
                if (yeni)
                {
                    Sozluk s = new Sozluk(0, proje.ad);
                    donusDeger = IsKurallari.Genel.TanimSozlukEkle(ve, kullanan, 1, s, false);
                    if (donusDeger.islemSonuc)
                    {
                        proje.kod = OrtakFonksiyonlar.ConvertToInt(donusDeger.anahtar, 0);
                        donusDeger = tabProje.Kaydet(yil, senaryoKod, proje);
                    }
                }
                else
                    donusDeger = tabProje.Guncelle(yil, senaryoKod, proje);

                if (donusDeger.islemSonuc)
                {
                    if (proje.ilIlceler != null)
                        donusDeger = IlIlce.Kaydet(ve, yil, senaryoKod, proje.kod, 0, proje.ilIlceler);
                }
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
            {
                donusDeger.anahtar = proje.kod.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        ///// <summary>
        ///// Verilen yıla uygun olan yatırım projelerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        //public static BilgiVAN Listele(Kullanici kullanan, string senaryoKod, int yil)
        //{
        //    Proje p = new Proje();
        //    return Listele(null, kullanan, yil, senaryoKod, p, null);
        //}

        /// <summary>
        /// Verilen kriterlere uygun olan yatırım projelerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="proje">Yatırım projesi kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, Proje proje, string kurKod, bool altProjelerle)
        {
            if (proje == null) proje = new Proje();
            BilgiVAN donusDeger = new BilgiVAN();
            if (proje.kod == 0)
            {
                donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, kurKod);
                if (!donusDeger.sonuc.islemSonuc)
                    return donusDeger;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATProje tabProje = new TABYATProje(ve);
            IDataReader dr = null;

            dr = tabProje.Listele(yil, senaryoKod, Yetki.BirimKodlariDondur(kullanan, kurKod), proje, altProjelerle);

            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve2.VeriErisimKapat();
                ve.VeriErisimKapat();
                return donusDeger;
            }
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            Hashtable ht = new Hashtable();
            int eskiGelen = -1;
            Proje aktifProje = null;
            while (dr.Read())
            {
                Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

                int gelen_kod = VeriErisim.DegerAl(dr, 0, -1);
                if (aktifProje == null || gelen_kod != eskiGelen)
                {
                    eskiGelen = gelen_kod;
                    Proje gelen = new Proje();
                    gelen.kod = eskiGelen;
                    gelen.DPTProjeNo = VeriErisim.DegerAl(dr, 1, "");

                    gelen.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99", VeriErisim.DegerAl(dr, 2, ""));
                    gelen.tur.kod = VeriErisim.DegerAl(dr, 3, -1);
                    gelen.ad = VeriErisim.DegerAl(dr, 4, "");
                    gelen.karakteristik = VeriErisim.DegerAl(dr, 5, "");
                    gelen.karakteristikOzet = VeriErisim.DegerAl(dr, 6, "");

                    gelen.etudProjeMi = VeriErisim.DegerAl(dr, 7, "") == "E";
                    gelen.baslangicYili = VeriErisim.DegerAl(dr, 8, 0);
                    gelen.bitisYili = VeriErisim.DegerAl(dr, 9, 0);
                    gelen.model = VeriErisim.DegerAl(dr, 10, -1);
                    string kurAd = "";
                    gelen.yurutecekKurum = GecerliDegilseBosalt(kullanan, uzyServis, VeriErisim.DegerAl(dr, 11, ""), tUz.Kirilimlar(0), yil, ht, out kurAd);
                    if (!string.IsNullOrWhiteSpace(gelen.yurutecekKurum))
                        kurAd = YATServis.UzayYaziDegerGetir("ABSKUR", yil.ToString() + "-" + gelen.yurutecekKurum, true, "");

                    gelen.yurutecekKurumAd = kurAd;
                    gelen.gerekce = VeriErisim.DegerAl(dr, 12, "");
                    gelen.aktifMi = VeriErisim.DegerAl(dr, 13, 1) == 1;
                    gelen.eskiYeni = VeriErisim.DegerAl(dr, 14, 0) == 1;
                    gelen.cari = VeriErisim.DegerAl(dr, 15, 0) == 1;

                    gelen.sektor.ad = VeriErisim.DegerAl(dr, 18, "");
                    gelen.tur.ad = VeriErisim.DegerAl(dr, 19, "");
                    gelen.ilIlceler = IlIlce.Listele(ve2, kullanan, yil, senaryoKod, gelen.kod, 0);

                    donusDeger.liste.Add(gelen);
                    aktifProje = gelen;
                }
                if (altProjelerle)
                {
                    aktifProje.altProjeler.Add(AltProjeOl(dr, 20));
                }
            }
            dr.Close();
            if (proje.kod != 0 && donusDeger.liste.Count == 1)
            {
                Proje p = ((Proje)donusDeger.liste[0]);
                donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, p.yurutecekKurum);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    if (ProjeClass.ProjeGormeHakkiVarMi(ve, kullanan, p))
                    {
                        donusDeger.sonuc.DegerAta("");
                    }
                }
            }
            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan yatırım projelerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Yatırım projesi kriteri</param>
        /// <param name="kurKod">Yatırım projesi birimi</param>
        /// <param name="kriter">Yatırım projesi kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN ListeleProjeTanim(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string kurKod, string kriter)
        {
            BilgiVAN donusDeger = new BilgiVAN();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATProje tabProje = new TABYATProje(ve);
            IDataReader dr = tabProje.Listele(yil, senaryoKod, kurKod, kriter);

            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return donusDeger;
            }
            while (dr.Read())
            {
                Proje gelen = new Proje();
                gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
                gelen.DPTProjeNo = VeriErisim.DegerAl(dr, 1, "");

                gelen.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99", VeriErisim.DegerAl(dr, 2, ""));
                gelen.tur.kod = VeriErisim.DegerAl(dr, 3, -1);
                gelen.ad = VeriErisim.DegerAl(dr, 4, "");
                gelen.karakteristik = VeriErisim.DegerAl(dr, 5, "");
                gelen.karakteristikOzet = VeriErisim.DegerAl(dr, 6, "");

                gelen.etudProjeMi = VeriErisim.DegerAl(dr, 7, "") == "E";
                gelen.baslangicYili = VeriErisim.DegerAl(dr, 8, 0);
                gelen.bitisYili = VeriErisim.DegerAl(dr, 9, 0);
                gelen.model = VeriErisim.DegerAl(dr, 10, -1);
                gelen.gerekce = VeriErisim.DegerAl(dr, 12, "");
                gelen.aktifMi = VeriErisim.DegerAl(dr, 13, 1) == 1;
                gelen.eskiYeni = VeriErisim.DegerAl(dr, 14, 0) == 1;
                gelen.cari = VeriErisim.DegerAl(dr, 15, 0) == 1;

                donusDeger.liste.Add(gelen);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan yatırım projelerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Yatırım projesi kriteri</param>
        /// <param name="kurKod">Yatırım projesi birimi</param>
        /// <param name="kriter">Yatırım projesi kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN ListeleAltProjeTanim(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string kurKod, int projeKod, string kriter)
        {
            BilgiVAN donusDeger = new BilgiVAN();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATProje tabProje = new TABYATProje(ve);
            IDataReader dr = tabProje.ListeleAltProje(yil, senaryoKod, kurKod, projeKod, kriter);

            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return donusDeger;
            }
            while (dr.Read())
            {
                AltProje gelen = new AltProje();
                gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
                gelen.ad = VeriErisim.DegerAl(dr, 1, "");
                donusDeger.liste.Add(gelen);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static string GecerliDegilseBosalt(Kullanici kullanan, TNS.UZY.IUZYServis uzyServis, string okunanKurkod, string maske, int yil, Hashtable ht, out string kurAd)
        {
            kurAd = "";
            okunanKurkod = OrtakFonksiyonlar.KodNoktaKoy(maske, okunanKurkod.Replace(".", ""));
            if (okunanKurkod.Length > maske.Length)
                okunanKurkod = "";
            if (string.IsNullOrWhiteSpace(okunanKurkod))
            {
                try
                {
                    kurAd = (string)ht[okunanKurkod];
                }
                catch
                {
                    kurAd = null;
                }
                if (kurAd == null)
                {
                    kurAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + okunanKurkod, true);
                    if (string.IsNullOrWhiteSpace(kurAd))
                    {
                        okunanKurkod = "";
                    }
                    else
                        ht[okunanKurkod] = kurAd;
                }
            }
            return okunanKurkod;
        }

        private static AltProje AltProjeOl(IDataReader dr, int baslaKolon)
        {
            AltProje ap = new AltProje();
            ap.kod = VeriErisim.DegerAl(dr, baslaKolon, 0);
            ap.ad = VeriErisim.DegerAl(dr, baslaKolon + 4, "");
            ap.DPTDeVar = VeriErisim.DegerAl(dr, baslaKolon + 26, 0) == 1;

            return ap;
        }

        /// <summary>
        /// Yatırım projesi, yatırım alt projelerinde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// proje nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeler">Proje nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, TNSCollection projeler)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATProje tabProje = new TABYATProje(ve);
            TABYATIlIlce tabIlIlce = new TABYATIlIlce(ve);
            TABYATALTPROJEBILGI tabBilgi = new TABYATALTPROJEBILGI(ve);
            ve.BeginTransaction();

            try
            {
                foreach (Proje p in projeler)
                {
                    if (p.kod <= 0)
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = YatirimResI.YAT049;
                        break;
                    }
                    if (Genel.ProjeKullanilmisMi(ve, yil, senaryoKod, p.kod))
                    {
                        donusDeger.islemSonuc = false;
                        p.ad = YATServis.UzayYaziDegerGetir("YATPROJE", yil + "-" + senaryoKod + "-" + p.kod.ToString(), true, string.Empty);
                        donusDeger.hataStr = string.Format(YatirimResI.YAT050, p.ad);
                        break;
                    }
                    tabBilgi.Sil(p.kod, 0);
                    if (!string.IsNullOrEmpty(ve.hata))
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = ve.hata;
                        break;
                    }

                    tabIlIlce.Sil(yil, senaryoKod, p.kod, 0);
                    if (!string.IsNullOrEmpty(ve.hata))
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = ve.hata;
                        break;
                    }

                    tabProje.Sil(yil, senaryoKod, p.kod);
                    if (!string.IsNullOrEmpty(ve.hata))
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = ve.hata;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
            {
                ve.Commit();
                donusDeger.bilgiStr = YatirimResI.YAT020;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriYAT";

        internal static ObjectArray ProjeListeleFaal(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string altFaalKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATProje tabProje = new TABYATProje(ve);
            IDataReader dr = tabProje.ListeleFaalIle(yil, senaryoKod, altFaalKod);
            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return donusDeger;
            }
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            Hashtable ht = new Hashtable();
            while (dr.Read())
            {
                Proje gelen = new Proje();
                gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
                gelen.DPTProjeNo = VeriErisim.DegerAl(dr, 1, "");

                gelen.sektor.kod = VeriErisim.DegerAl(dr, 2, "");
                if (gelen.sektor.kod != "")
                {
                    gelen.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99", gelen.sektor.kod);
                    gelen.sektor.ad = YATServis.UzayYaziDegerGetir("YATSEKTOR", gelen.sektor.kod, true, "");
                }

                gelen.tur.kod = VeriErisim.DegerAl(dr, 3, -1);
                if (gelen.tur.kod != -1)
                    gelen.tur.ad = YATServis.UzayYaziDegerGetir("YATPROJETUR", gelen.tur.kod.ToString(), true, "");

                gelen.ad = VeriErisim.DegerAl(dr, 4, "");
                gelen.karakteristik = VeriErisim.DegerAl(dr, 5, "");
                gelen.karakteristikOzet = VeriErisim.DegerAl(dr, 6, "");

                string etudMu = VeriErisim.DegerAl(dr, 7, "");
                if (etudMu == "E")
                    gelen.etudProjeMi = true;
                else
                    gelen.etudProjeMi = false;

                gelen.baslangicYili = VeriErisim.DegerAl(dr, 8, 0);
                gelen.bitisYili = VeriErisim.DegerAl(dr, 9, 0);

                gelen.model = VeriErisim.DegerAl(dr, 10, -1);
                string kurAd = "";
                gelen.yurutecekKurum = GecerliDegilseBosalt(kullanan, uzyServis, VeriErisim.DegerAl(dr, 11, ""), tUz.Kirilimlar(0), yil, ht, out kurAd);
                gelen.yurutecekKurumAd = kurAd;


                gelen.gerekce = VeriErisim.DegerAl(dr, 12, "");
                gelen.aktifMi = VeriErisim.DegerAl(dr, 13, 1) == 1;
                gelen.eskiYeni = VeriErisim.DegerAl(dr, 14, 0) == 1;

                gelen.ilIlceler = IlIlce.Listele(ve2, kullanan, yil, senaryoKod, gelen.kod, 0);

                donusDeger.Ekle(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// KarakteristikAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KarakteristikAna yapısında KarakteristikAna: Karakteristik Ana bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KarekteristikKaydet(VeriErisim veriErisim, Kullanici kullanan, KarakteristikGiris[] form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta("Bu işlemi yapmaya yetkiniz yoktur!");
                return s;
            }
            if (form == null || form.Length == 0)
            {
                s.DegerAta("Boş liste kaydedilemez!");
                return s;
            }
            for (int i = 0; i < form.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(form[i].deger))
                {
                    s.DegerAta("Adı boş olan satır var!");
                    return s;
                }
            }
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATKARAKTERISTIKTANIM tabYATKARAKTERISTIKTANIM = new TABYATKARAKTERISTIKTANIM(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabYATKARAKTERISTIKTANIM.Sil();

            for (int i = 0; i < form.Length && s.islemSonuc; i++)
            {
                s = tabYATKARAKTERISTIKTANIM.Ekle(form[i]);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// KarakteristikAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        public static KarakteristikGiris[] KarekteristikListele(VeriErisim veriErisim, Kullanici kullanan)
        {
            List<KarakteristikGiris> al = new List<KarakteristikGiris>();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATKARAKTERISTIKTANIM tabYATKARAKTERISTIKTANIM = new TABYATKARAKTERISTIKTANIM(ve);

            IDataReader dr = tabYATKARAKTERISTIKTANIM.Listele();
            if (dr == null)
            {
                ve.VeriErisimKapat();
                return al.ToArray();
            }
            while (dr.Read())
            {
                KarakteristikGiris detay = new KarakteristikGiris();
                detay.deger = VeriErisim.DegerAl(dr, (int)TABYATKARAKTERISTIKTANIM.ENUMAlanlar.DEGER, "");
                detay.degerKisa = VeriErisim.DegerAl(dr, (int)TABYATKARAKTERISTIKTANIM.ENUMAlanlar.DEGERKISA, "");
                detay.ekTur = (ENUMKarakteristikTur)VeriErisim.DegerAl(dr, (int)TABYATKARAKTERISTIKTANIM.ENUMAlanlar.EKTUR, 0);
                detay.sira = VeriErisim.DegerAl(dr, (int)TABYATKARAKTERISTIKTANIM.ENUMAlanlar.SIRA, "");
                al.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return al.ToArray();
        }

        private static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Verilen kriterlere uygun olan yatırım projelerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="proje">Yatırım projesi kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static ObjectArray ListeleTum(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, Proje proje, bool tekYil, bool eskiYillarKBO, int ekoUz)
        {
            if (proje == null) proje = new Proje();
            ObjectArray donusDeger = new ObjectArray();
            if (proje.kod == 0)
            {
                donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, proje.birim.kod);
                if (!donusDeger.sonuc.islemSonuc)
                    return donusDeger;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATProje tabProje = new TABYATProje(ve);
            string sql;
            IDataReader dr = tabProje.ListeleTum(yil, senaryoKod, Yetki.BirimKodlariDondur(kullanan, proje.birim.kod), proje, tekYil, eskiYillarKBO, ekoUz, out sql);
            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return donusDeger;
            }
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            Hashtable ht = new Hashtable();
            while (dr.Read())
            {
                Proje gelen = new Proje();
                gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
                gelen.DPTProjeNo = VeriErisim.DegerAl(dr, 1, "");
                gelen.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99", VeriErisim.DegerAl(dr, 2, ""));
                gelen.tur.kod = VeriErisim.DegerAl(dr, 3, -1);
                gelen.ad = VeriErisim.DegerAl(dr, 4, "");
                gelen.karakteristik = VeriErisim.DegerAl(dr, 5, "");
                gelen.karakteristikOzet = VeriErisim.DegerAl(dr, 6, "");

                gelen.etudProjeMi = VeriErisim.DegerAl(dr, 7, "") == "E";
                gelen.baslangicYili = VeriErisim.DegerAl(dr, 8, 0);
                gelen.bitisYili = VeriErisim.DegerAl(dr, 9, 0);
                gelen.model = VeriErisim.DegerAl(dr, 10, -1);
                gelen.yurutecekKurum = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(0), VeriErisim.DegerAl(dr, 11, ""));
                gelen.gerekce = VeriErisim.DegerAl(dr, 12, "");
                gelen.aktifMi = VeriErisim.DegerAl(dr, 13, 1) == 1;
                gelen.eskiYeni = VeriErisim.DegerAl(dr, 14, 0) == 1;
                gelen.cari = VeriErisim.DegerAl(dr, 15, 0) == 1;

                gelen.sektor.ad = VeriErisim.DegerAl(dr, 18, "");
                gelen.tur.ad = VeriErisim.DegerAl(dr, 19, "");
                gelen.yurutecekKurumAd = VeriErisim.DegerAl(dr, 20, "");

                gelen.altProjeler = AltProjeClass.ListeleTum(yil, VeriErisim.DegerAl(dr, 21, ""), gelen.ad);

                gelen.bilgiler = AltProjeClass.BilgiOku(VeriErisim.DegerAl(dr, 22, ""));
                gelen.ilIlceler = IlIlce.Listele(VeriErisim.DegerAl(dr, 23, ""));
                gelen.kalkinmaBolgeler = AltProjeClass.BolgeListele(VeriErisim.DegerAl(dr, 24, ""));
                donusDeger.Ekle(gelen);
            }
            dr.Close();

            //DosyayaYaz(sql);
            //foreach (Proje p in donusDeger.objeler)
            //{
            //    DosyayaYaz(String.Format("Proje: {0} {1} {2}", p.DPTProjeNo, p.ad, p.bitisYili));
            //    foreach (AltProje altP in p.altProjeler)
            //    {
            //        DosyayaYaz(String.Format("\tAlt Proje: {0} {1} {2}", altP.kod, altP.ad, altP.bitisYil));
            //        foreach (ButceKaynak bk in altP.butceKaynaklari)
            //        {
            //            DosyayaYaz(String.Format("\t\tKaynak: {0} {1} {2}", bk.kaynakNo, bk.kaynakAd, bk.kaynakTur));
            //            foreach (ButceKaynakTeklif bkt in bk.teklifler)
            //            {
            //                DosyayaYaz(String.Format("\t\t\tTeklif: {0} {1} {2}", bkt.butceYil, bkt.HarcamaYilSonuToplam, bkt.Odenek0Toplam));
            //            }
            //        }
            //    }
            //}
            //if (proje.kod != 0 && donusDeger.ObjeSayisi == 1)
            //{
            //    Proje p = (Proje)donusDeger.Al(0);
            //    donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, p.yurutecekKurum);
            //    if (!donusDeger.sonuc.islemSonuc)
            //    {
            //        if (ProjeClass.ProjeGormeHakkiVarMi(ve, kullanan, p))
            //        {
            //            donusDeger.sonuc.DegerAta("");
            //        }
            //    }
            //}
            ve.VeriErisimKapat();
            return donusDeger;
        }

        private static void DosyayaYaz(string v)
        {
            OrtakFonksiyonlar.HataDosyaYaz("ProjeSonuc.txt", v, false);
        }
    }
}
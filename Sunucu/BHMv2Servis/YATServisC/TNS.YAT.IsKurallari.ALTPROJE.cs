using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;
using TNS.BKD;
using System.Collections;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Yatırım alt proje bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class AltProjeClass
    {
        public static bool AltProjeYazmaHakkiVarMi(VeriErisim ve, Kullanici kullanan, AltProje ap, int yil, string senaryoKod)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            TABYATALTPROJEBILGI tab = new TABYATALTPROJEBILGI(ve);
            int say = tab.YetkiliKullaniciSay(kullanan.KullaniciKodu, ap.proje.kod, ap.kod);
            if (say != 1) return false;
            return true;

        }

        /// <summary>
        /// Yatırım alt projesi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen alt proje nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProje">Yatırım alt projesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, AltProje altProje, bool yeni)
        {
            if (string.IsNullOrWhiteSpace(altProje.birim.kod))
            {
                return new Sonuc("Lütfen sorumlu birimi seçiniz!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc donusDeger = Yetki.ProjeYazmaHakkiVarMi(kullanan, altProje.birim.kod);
            if (!donusDeger.islemSonuc)
            {
                if (altProje.kod > 0)
                {
                    donusDeger = ProjeClass.ProjeAltProjeYazmaHakkiVarMi(ve, kullanan, altProje.proje.kod, altProje.kod, yil, senaryoKod);
                    if (donusDeger == null) donusDeger = new Sonuc();
                }
            }
            else
            {
                TNS.BHMv2.IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                if (ProjeClass.KapaliMi(servis, kullanan, yil, senaryoKod)) donusDeger.DegerAta("Veri giriş dönemi kapatıldığından kayıt işlemi yapamazsınız!");
            }

            if (altProje.ad.Trim() == "")
            {
                donusDeger.DegerAta(YatirimResI.YAT010);
            }

            if (altProje.baslangicYil <= 0)
            {
                donusDeger.DegerAta(YatirimResI.YAT011);
            }

            if (altProje.bitisYil <= 0)
            {
                donusDeger.DegerAta(YatirimResI.YAT009);
            }

            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            TABYATAltProje tabAltProje = new TABYATAltProje(ve);
            ve.BeginTransaction();

            int aYil = yil;
            if (altProje.birim.kod != "")
                altProje.birim.AdAta(YATServis.UzayYaziDegerGetir("ABSKUR", aYil.ToString() + "-" + altProje.birim.kod, true, ""));

            Tertip t = new Tertip(aYil, ENUMTabloKod.TABOdenek);

            if (altProje.birim.kod == "" || altProje.birim.Ad() == "" || altProje.birim.kod.Length != t.Kirilimlar(0).Length)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT012;
            }

            if (!Genel.ProjeVarMi(ve, yil, senaryoKod, altProje.proje.kod))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT013;
            }

            if (!Genel.ProjeTurVarMi(ve, altProje.tur.kod))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT014;
            }

            string hataStrYilKontrol = AltProjeYilKontrol(kullanan, yil, senaryoKod, altProje);
            if (!string.IsNullOrEmpty(hataStrYilKontrol))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = hataStrYilKontrol;
            }

            //if (!Genel.IlceVarMi(ve, altProje.ilce))
            //{
            //    donusDeger.islemSonuc = false;
            //    donusDeger.hataStr = "İlçe bilgisi yanlış!";
            //}

            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            try
            {
                //Proje proje = new Proje();
                //proje.kod = altProje.proje.kod;
                //BilgiVAN ustProje = ProjeClass.Listele(kullanan, proje);

                //if (!ustProje.sonuc.islemSonuc)
                //{
                //    donusDeger.islemSonuc = false;
                //    ve.VeriErisimKapat();
                //    return donusDeger;
                //}

                //if (altProje.baslangicYil < ((Proje)ustProje.liste[0]).baslangicYili || altProje.bitisYil > ((Proje)ustProje.liste[0]).bitisYili)
                //{
                //    if (altProje.baslangicYil < ((Proje)ustProje.liste[0]).baslangicYili)
                //        ((Proje)ustProje.liste[0]).baslangicYili = altProje.baslangicYil;
                //    if (altProje.bitisYil > ((Proje)ustProje.liste[0]).bitisYili)
                //        ((Proje)ustProje.liste[0]).bitisYili = altProje.bitisYil;
                //    donusDeger = ProjeClass.Kaydet(ve, kullanan, (Proje)ustProje.liste[0]);
                //}

                //if (!donusDeger.islemSonuc)
                //{
                //    donusDeger.islemSonuc = false;
                //    ve.VeriErisimKapat();
                //    return donusDeger;
                //}

                if (yeni)
                {
                    Sozluk s = new Sozluk(0, altProje.ad);
                    donusDeger = IsKurallari.Genel.TanimSozlukEkle(ve, kullanan, 2, s, false);
                    if (donusDeger.islemSonuc)
                    {
                        altProje.kod = OrtakFonksiyonlar.ConvertToInt(donusDeger.anahtar, 0);
                        donusDeger = tabAltProje.Kaydet(yil, senaryoKod, altProje);
                    }
                }
                else
                    donusDeger = tabAltProje.Guncelle(yil, senaryoKod, altProje);

                if (donusDeger.islemSonuc)
                {
                    donusDeger = IlIlce.Kaydet(ve, yil, senaryoKod, altProje.proje.kod, altProje.kod, altProje.ilIlceler);
                }
                if (donusDeger.islemSonuc)
                {
                    donusDeger = BolgeKaydet(ve, yil, senaryoKod, altProje.proje.kod, altProje.kod, altProje.bolgeler);
                }
                if (donusDeger.islemSonuc && yeni)
                {
                    if ((senaryoKod == "0000" || senaryoKod == "00.00"))
                    {
                        ButceKaynak kaynak = new ButceKaynak();
                        kaynak.altProjeKodu = altProje.kod;
                        kaynak.kaynakAd = "Proje Ödenek Bilgileri";
                        kaynak.kaynakTur = ENUMButceKaynakTur.YENIPROJE;
                        kaynak.toplamaDahil = ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleHaric;
                        kaynak.DPTRapordaGor = false;
                        donusDeger = ButceKaynakClass.Kaydet(ve, kullanan, yil, senaryoKod, kaynak, true, false);
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
                donusDeger.anahtar = altProje.kod.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        private static Sonuc BolgeKaydet(VeriErisim ve, int yil, string senaryoKod, int projeNo, int altProjeNo, TNSCollection bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            TABYATPROJEKALKINMABOLGE tabIlIlce = new TABYATPROJEKALKINMABOLGE(ve);
            donusDeger = tabIlIlce.Sil(yil, senaryoKod, projeNo, altProjeNo);
            if (bilgi != null)
            {
                foreach (KalkinmaBolge il in bilgi)
                {
                    if (donusDeger.islemSonuc)
                        donusDeger = tabIlIlce.Ekle(yil, senaryoKod, projeNo, altProjeNo, il.bolgeKod);
                    else
                        break;
                }
            }
            return donusDeger;
        }

        /// <summary>
        /// Yatırım alt projesi ile projesinin başlangıç ve bitiş yıllarının
        /// birbiriyle tutarlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProje">Yatırım alt projesi bilgisini tutan nesne</param>
        /// <returns>Hata varsa hata, yoksa boş string döndürülür.</returns>
        private static string AltProjeYilKontrol(Kullanici kullanan, int yil, string senaryoKod, AltProje altProje)
        {
            string hataStr = string.Empty;
            Proje proje = new Proje();
            proje.kod = altProje.proje.kod;
            BilgiVAN projeListe = ProjeClass.Listele(null, kullanan, yil, senaryoKod, proje, null, false);
            if (projeListe.liste.Count > 0)
            {
                Proje p = (Proje)projeListe.liste[0];
                if (altProje.baslangicYil < p.baslangicYili || altProje.baslangicYil > p.bitisYili)
                {
                    hataStr += string.Format(YatirimResI.YAT015, p.baslangicYili, p.bitisYili) + "<br>";
                }
                if (altProje.bitisYil < p.baslangicYili || altProje.bitisYil > p.bitisYili)
                {
                    hataStr += string.Format(YatirimResI.YAT016, p.baslangicYili, p.bitisYili) + "<br>";
                }
            }
            return hataStr;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan yatırım alt projelerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="altProje">Yatırım alt projesi kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım alt projeleri döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, AltProje altProje, string sektorKod, bool detayOku, bool detayRakamOku)
        {
            BilgiVAN donusDeger = new BilgiVAN();
            if (altProje == null) altProje = new AltProje();
            if (altProje.kod == 0)
            {
                donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, null);
                if (!donusDeger.sonuc.islemSonuc)
                    return donusDeger;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATAltProje tabAltProje = new TABYATAltProje(ve);
            IDataReader dr = null;

            try
            {
                string[] kurkodlar = Yetki.BirimKodlariDondur(kullanan, altProje.birim.kod);
                dr = tabAltProje.Listele(yil, senaryoKod, altProje.proje.kod, kurkodlar, sektorKod, altProje.kod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                AltProje gelen = new AltProje();
                gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
                gelen.aktifYil = yil;
                if (gelen.aktifYil == -1)
                    gelen.aktifYil = Genel.AktifYilDondur();
                int aYil = gelen.aktifYil;
                Tertip t = new Tertip(aYil, ENUMTabloKod.TABOdenek);
                gelen.birim.kod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), VeriErisim.DegerAl(dr, 1, ""));
                if (gelen.birim.kod != "")
                    gelen.birim.AdAta(YATServis.UzayYaziDegerGetir("ABSKUR", aYil.ToString() + "-" + gelen.birim.kod, true, ""));

                gelen.proje.kod = VeriErisim.DegerAl(dr, 2, -1);
                if (gelen.proje.kod != -1)
                    gelen.proje.ad = YATServis.UzayYaziDegerGetir("YATPROJE", yil + "-" + senaryoKod + "-" + gelen.proje.kod.ToString(), true, "");

                gelen.tur.kod = VeriErisim.DegerAl(dr, 3, -1);
                if (gelen.tur.kod != -1)
                    gelen.tur.ad = YATServis.UzayYaziDegerGetir("YATPROJETUR", gelen.tur.kod.ToString(), true, "");

                gelen.ad = VeriErisim.DegerAl(dr, 4, "");
                gelen.karakteristik = VeriErisim.DegerAl(dr, 5, "");
                gelen.karakteristikOzet = VeriErisim.DegerAl(dr, 6, "");

                gelen.sorumluAd = VeriErisim.DegerAl(dr, 7, "");
                gelen.sorumluGorev = VeriErisim.DegerAl(dr, 8, "");
                gelen.sorumluTelefon = VeriErisim.DegerAl(dr, 9, "");
                gelen.sorumluEPosta = VeriErisim.DegerAl(dr, 10, "");
                gelen.onerenAd = VeriErisim.DegerAl(dr, 11, "");
                gelen.onerenAdres = VeriErisim.DegerAl(dr, 12, "");
                gelen.onerenTelefon = VeriErisim.DegerAl(dr, 13, "");
                gelen.onerenFaks = VeriErisim.DegerAl(dr, 14, "");
                gelen.gerekce = VeriErisim.DegerAl(dr, 15, "");
                gelen.amac = VeriErisim.DegerAl(dr, 16, "");
                gelen.hedef = VeriErisim.DegerAl(dr, 17, "");
                gelen.maliyet = VeriErisim.DegerAl(dr, 18, (double)0);
                gelen.finansmanKaynak = VeriErisim.DegerAl(dr, 19, "");
                gelen.baslangicYil = VeriErisim.DegerAl(dr, 20, 0);
                gelen.bitisYil = VeriErisim.DegerAl(dr, 21, 0);
                gelen.model = VeriErisim.DegerAl(dr, 22, 0);
                gelen.ihaleTarihi = VeriErisim.DegerAl(dr, 23);
                gelen.ihaleBedeli = VeriErisim.DegerAl(dr, 24, (double)0);
                gelen.ihaleYasasi = VeriErisim.DegerAl(dr, 25, "");
                gelen.DPTDeVar = VeriErisim.DegerAl(dr, 26, 0) == 1;

                gelen.ilIlceler = IlIlce.Listele(ve2, kullanan, yil, senaryoKod, gelen.proje.kod, gelen.kod);
                gelen.bolgeler = BolgeListele(ve2, kullanan, yil, senaryoKod, gelen.proje.kod, gelen.kod);

                donusDeger.liste.Add(gelen);
            }
            dr.Close();

            if (altProje.kod != 0 && donusDeger.liste.Count == 1)
            {
                AltProje gelen = (AltProje)donusDeger.liste[0];

                donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, gelen.birim_kod);
                if (!donusDeger.sonuc.islemSonuc)
                {
                    if (AltProjeYazmaHakkiVarMi(ve, kullanan, gelen, 0, ""))
                    {
                        donusDeger.sonuc.DegerAta("");
                    }
                }
            }

            if (detayOku && donusDeger.sonuc.islemSonuc)
            {
                foreach (AltProje gelen in donusDeger.liste)
                {
                    BilgiVAN bv = ButceKaynakClass.BilgiOku(ve, kullanan, 0, gelen.kod, 0, yil, senaryoKod, string.Empty, null, detayRakamOku, false, gelen.birim_kod, false, null, false);
                    gelen.butceKaynaklari = bv.liste;
                }
            }

            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static TNSCollection ListeleTum(int aYil, string okunanXML, string projeAd)
        {
            TNSCollection donusDeger = new TNSCollection();
            IDataReader dr = new XMLStringDataReader(okunanXML);
            while (dr.Read())
            {
                AltProje gelen = new AltProje();
                gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
                Tertip t = new Tertip(aYil, ENUMTabloKod.TABOdenek);
                gelen.birim.kod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), VeriErisim.DegerAl(dr, 1, ""));

                gelen.proje.kod = VeriErisim.DegerAl(dr, 2, -1);
                gelen.tur.kod = VeriErisim.DegerAl(dr, 3, -1);
                gelen.ad = VeriErisim.DegerAl(dr, 4, "");
                gelen.karakteristik = VeriErisim.DegerAl(dr, 5, "");
                gelen.karakteristikOzet = VeriErisim.DegerAl(dr, 6, "");

                gelen.sorumluAd = VeriErisim.DegerAl(dr, 7, "");
                gelen.sorumluGorev = VeriErisim.DegerAl(dr, 8, "");
                gelen.sorumluTelefon = VeriErisim.DegerAl(dr, 9, "");
                gelen.sorumluEPosta = VeriErisim.DegerAl(dr, 10, "");
                gelen.onerenAd = VeriErisim.DegerAl(dr, 11, "");
                gelen.onerenAdres = VeriErisim.DegerAl(dr, 12, "");
                gelen.onerenTelefon = VeriErisim.DegerAl(dr, 13, "");
                gelen.onerenFaks = VeriErisim.DegerAl(dr, 14, "");
                gelen.gerekce = VeriErisim.DegerAl(dr, 15, "");
                gelen.amac = VeriErisim.DegerAl(dr, 16, "");
                gelen.hedef = VeriErisim.DegerAl(dr, 17, "");
                gelen.maliyet = VeriErisim.DegerAl(dr, 18, (double)0);
                gelen.finansmanKaynak = VeriErisim.DegerAl(dr, 19, "");
                gelen.baslangicYil = VeriErisim.DegerAl(dr, 20, 0);
                gelen.bitisYil = VeriErisim.DegerAl(dr, 21, 0);
                gelen.model = VeriErisim.DegerAl(dr, 22, 0);
                gelen.ihaleTarihi = VeriErisim.DegerAl(dr, 23);
                gelen.ihaleBedeli = VeriErisim.DegerAl(dr, 24, (double)0);
                gelen.ihaleYasasi = VeriErisim.DegerAl(dr, 25, "");
                gelen.DPTDeVar = VeriErisim.DegerAl(dr, 26, 0) == 1;

                gelen.birim.AdAta(VeriErisim.DegerAl(dr, 29, ""));
                gelen.tur.ad = VeriErisim.DegerAl(dr, 30, "");
                gelen.proje.ad = projeAd;

                gelen.bilgiler = AltProjeClass.BilgiOku(VeriErisim.DegerAl(dr, 31, "") + "");
                gelen.ilIlceler = IlIlce.Listele(VeriErisim.DegerAl(dr, 32, ""));
                gelen.bolgeler = AltProjeClass.BolgeListele(VeriErisim.DegerAl(dr, 33, ""));
                gelen.butceKaynaklari = ButceKaynakClass.BilgiOku(aYil, VeriErisim.DegerAl(dr, 34, ""));
                donusDeger.Add(gelen);
            }
            dr.Close();
            return donusDeger;
        }

        /// <summary>
        /// Verilen yatırım projesine veya alt projesine ait olan il, ilçe bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeNo">Yatırım projesinin programatik kodu</param>
        /// <param name="altProjeNo">Yatırım alt projesinin programatik kodu</param>
        /// <returns>ProjeIlIlce nesnelerinden oluşan liste döndürülür.</returns>
        public static TNSCollection BolgeListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int projeNo, int altProjeNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATPROJEKALKINMABOLGE tab = new TABYATPROJEKALKINMABOLGE(ve);
            IDataReader dr = tab.Oku(yil, senaryoKod, projeNo, altProjeNo);
            TNSCollection donusDeger = DonusDegerOl(dr);
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen yatırım projesine veya alt projesine ait olan il, ilçe bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeNo">Yatırım projesinin programatik kodu</param>
        /// <param name="altProjeNo">Yatırım alt projesinin programatik kodu</param>
        /// <returns>ProjeIlIlce nesnelerinden oluşan liste döndürülür.</returns>
        public static TNSCollection BolgeListele(string okunanXML)
        {
            XMLStringDataReader dr = new XMLStringDataReader(okunanXML);
            return DonusDegerOl(dr);
        }

        private static TNSCollection DonusDegerOl(IDataReader dr)
        {
            TNSCollection donusDeger = new TNSCollection();
            if (dr != null)
            {
                while (dr.Read())
                {
                    KalkinmaBolge kb = new KalkinmaBolge();
                    kb.bolgeKod = VeriErisim.DegerAl(dr, (int)TABYATPROJEKALKINMABOLGE.ENUMAlanlar.BOLGEKOD, "");
                    donusDeger.Add(kb);
                }
                dr.Close();
            }
            return donusDeger;
        }

        /// <summary>
        /// Yatırım alt projesi; bütçe kaynaklarda, dokümanlarda ve kredi taleplerde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// alt proje nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeler">Alt proje nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, TNSCollection altProjeler)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATAltProje tabAltProje = new TABYATAltProje(ve);
            TABYATIlIlce tabIlIlce = new TABYATIlIlce(ve);
            TABYATPROJEKALKINMABOLGE tabBolge = new TABYATPROJEKALKINMABOLGE(ve);
            TABYATALTPROJEDETAYPROGRAMI tabDetayProgram = new TABYATALTPROJEDETAYPROGRAMI(ve);
            ve.BeginTransaction();

            try
            {
                foreach (AltProje ap in altProjeler)
                {
                    if (ap.kod <= 0)
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = YatirimResI.YAT018;
                        break;
                    }
                    if (tabAltProje.AltProjeKullanilmisMi(yil, senaryoKod, ap.kod))
                    {
                        donusDeger.islemSonuc = false;
                        ap.ad = YATServis.UzayYaziDegerGetir("YATALTPROJE", yil + "-" + senaryoKod + "-" + ap.kod.ToString(), true, string.Empty);
                        donusDeger.hataStr = string.Format(YatirimResI.YAT019, ap.ad);
                        break;
                    }

                    tabAltProje.Sil(yil, senaryoKod, ap.kod);
                    if (!string.IsNullOrEmpty(ve.hata))
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = ve.hata;
                        break;
                    }
                    tabIlIlce.Sil(yil, senaryoKod, 0, ap.kod);
                    if (!string.IsNullOrEmpty(ve.hata))
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = ve.hata;
                        break;
                    }

                    tabBolge.Sil(yil, senaryoKod, 0, ap.kod);
                    if (!string.IsNullOrEmpty(ve.hata))
                    {
                        donusDeger.islemSonuc = false;
                        donusDeger.hataStr = ve.hata;
                        break;
                    }

                    tabDetayProgram.Sil(yil, senaryoKod, ap.kod);
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

        //public static Sonuc BitisYiliGuncelle(VeriErisim veriErisim, int altProjeKodu, int bitisYili)
        //{
        //    Sonuc donusDeger = new Sonuc();

        //    if (bitisYili <= 0)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = YatirimResI.YAT009;
        //        return donusDeger;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABYATAltProje tabAltProje = new TABYATAltProje(ve);
        //    ve.BeginTransaction();

        //    try
        //    {
        //        donusDeger = tabAltProje.BitisYiliGuncelle(altProjeKodu, bitisYili);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.Message;
        //    }

        //    if (donusDeger.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return donusDeger;
        //}

        internal static Sonuc GerekceKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, AltProje[] gerekceler)
        {
            Sonuc s = new Sonuc(true, "", "");
            TNS.BHMv2.IBHMv2Servis bhmv2Servis = TNS.BHMv2.Arac.Tanimla();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                int durum = bhmv2Servis.OnayKodOkuTasra(kullanan, yil, 0, "000", "0000", BHMv2.ENUMBirimOnayTur.PROJEGEREKCE);
                if (durum >= 4)
                {
                    s.DegerAta("Proje gerekçeleri veri girişine kapalı olduğundan değişiklik yapamazsınız!");
                    return s;
                }
            }

            foreach (AltProje item in gerekceler)
            {
                Sonuc donusDeger = Yetki.ProjeYazmaHakkiVarMi(kullanan, item.birim.kod);
                if (!donusDeger.islemSonuc)
                    return donusDeger;
                if (item.gerekce.Length > 300)
                {
                    s.DegerAta("Gerekçe 300 karakterden uzun olamaz!");
                    return s;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATAltProje tabAltProje = new TABYATAltProje(ve);
            ve.BeginTransaction();

            try
            {
                foreach (AltProje item in gerekceler)
                {
                    s = tabAltProje.GerekceGuncelle(yil, senaryoKod, item);
                    if (!s.islemSonuc)
                        break;
                }
            }
            catch (Exception e)
            {
                s.DegerAta(e.Message);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc AltProjeKaydetYeni(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, string sektorKod, AltProjeTeklif[] projeGerekceler)
        {
            TNS.BHMv2.IBHMv2Servis bhmv2Servis = TNS.BHMv2.Arac.Tanimla();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                int durum = bhmv2Servis.OnayKodOkuTasra(kullanan, yil, 0, "000", "0000", BHMv2.ENUMBirimOnayTur.PROJEGEREKCE);
                if (durum >= 4)
                {
                    return new Sonuc(false, "Proje gerekçeleri veri girişine kapalı olduğundan değişiklik yapamazsınız!", "");
                }
            }

            Sonuc donusDeger = Yetki.ProjeYazmaHakkiVarMi(kullanan, kurKod);
            if (!donusDeger.islemSonuc)
                return donusDeger;
            if (yil <= DateTime.Now.Year)
                donusDeger.DegerAta("Yıl, içinde bulunulan yıldan büyük olmalıdır!");
            if (string.IsNullOrWhiteSpace(kurKod) || kurKod.Replace(".", "").Length != 8)
                donusDeger.DegerAta("Kaydetmek için kurumsal kodu tam seçiniz!");
            if (string.IsNullOrWhiteSpace(sektorKod))
                donusDeger.DegerAta("Kaydetmek için sektör seçiniz!");
            int projeNo = 0;
            foreach (var item in projeGerekceler)
            {
                projeNo++;
                item.baslangicYil = yil;
                if (item.gerekce.Length > 1000)
                    donusDeger.DegerAta(string.Format("{0}. satırda, Gerekçe 1000 karakterden uzun olamaz!", projeNo));
                if (string.IsNullOrWhiteSpace(item.ad))
                    donusDeger.DegerAta(string.Format("{0}. satırda, Proje adını giriniz!", projeNo));
                if (item.yiliTeklifi0 <= 0)
                    donusDeger.DegerAta(string.Format("{0}. satırda, Projenin birinci yil ödeneğini giriniz!", projeNo));
                if (item.yiliTeklifi1 < 0)
                    donusDeger.DegerAta(string.Format("{0}. satırda, Projenin ikinci yil ödeneğini hatalı!", projeNo));
                if (item.yiliTeklifi2 < 0)
                    donusDeger.DegerAta(string.Format("{0}. satırda, Projenin üçüncü yil ödeneğini hatalı!", projeNo));
                if (item.projeTutari < item.yiliTeklifi0 + item.yiliTeklifi1 + item.yiliTeklifi2)
                    donusDeger.DegerAta(string.Format("{0}. satırda, Proje tutarı, yıl ödeneklerinin toplamından küçük olamaz!", projeNo));
                if (item.baslangicYil > item.bitisYil)
                    donusDeger.DegerAta(string.Format("{0}. satırda, Projenin başlangıç yılı, bitiş yılından büyük olamaz!", projeNo));
                if (!donusDeger.islemSonuc)
                    return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATALTPROJETEKLIF tabAltProje = new TABYATALTPROJETEKLIF(ve);
            ve.BeginTransaction();

            try
            {
                AltProjeTeklif form = new AltProjeTeklif(yil, kurKod, sektorKod);
                donusDeger = tabAltProje.Sil(form);
                if (donusDeger.islemSonuc)
                {
                    projeNo = 0;
                    foreach (AltProjeTeklif item in projeGerekceler)
                    {
                        donusDeger = tabAltProje.Ekle(yil, kurKod, sektorKod, ++projeNo, item);
                        if (!donusDeger.islemSonuc)
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                donusDeger.DegerAta(e.Message);
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        internal static ObjectArray AltProjeListeleYeni(VeriErisim veriErisim, Kullanici kullanan, AltProjeTeklif form)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, form.birim);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATALTPROJETEKLIF tabAltProje = new TABYATALTPROJETEKLIF(ve);
            IDataReader dr = tabAltProje.Oku(form);

            if (dr != null)
            {
                while (dr.Read())
                {
                    AltProjeTeklif gelen = new AltProjeTeklif();
                    gelen.yil = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.YIL, -1);
                    gelen.birim = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.BIRIM, "");
                    gelen.sektorNo = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.SEKTORNO, "");
                    int altProjeNo = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.ALTPROJENO, 0);
                    gelen.ad = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.AD, "");
                    gelen.baslangicYil = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.BASLANGICYILI, 0);
                    gelen.bitisYil = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.BITISYILI, 0);

                    gelen.projeTutari = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.PROJETUTARI, (double)0.0);
                    gelen.yiliTeklifi0 = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.YILITEKLIFI0, (double)0.0);
                    gelen.yiliTeklifi1 = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.YILITEKLIFI1, (double)0.0);
                    gelen.yiliTeklifi2 = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.YILITEKLIFI2, (double)0.0);

                    gelen.gerekce = VeriErisim.DegerAl(dr, (int)TABYATALTPROJETEKLIF.ENUMAlanlar.GEREKCE, "");

                    donusDeger.Ekle(gelen);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return donusDeger;
        }

        internal static Hashtable AltProjeBilgiListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int projeKodu, int altProjeKodu, string[] alanlar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATALTPROJEBILGI tab = new TABYATALTPROJEBILGI(ve);
            IDataReader dr = tab.Listele(yil, senaryoKod, projeKodu, altProjeKodu, alanlar);
            System.Collections.Hashtable ht = DROku(dr, false);
            ve.VeriErisimKapat();
            return ht;
        }

        internal static Hashtable AltProjeBilgiListeleTum(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string[] alanlar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATALTPROJEBILGI tab = new TABYATALTPROJEBILGI(ve);
            IDataReader dr = tab.Listele(yil, senaryoKod, -1, -1, alanlar);
            System.Collections.Hashtable ht = DROku(dr, true);
            ve.VeriErisimKapat();
            return ht;
        }

        internal static Hashtable BilgiOku(string okunanXML)
        {
            ObjectArray pVAN = new ObjectArray();

            XMLStringDataReader dr = new XMLStringDataReader(okunanXML);
            System.Collections.Hashtable ht = DROku(dr, false);
            return ht;
        }

        private static Hashtable DROku(IDataReader dr, bool projeAltProjeEKle)
        {
            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            if (dr != null)
            {
                while (dr.Read())
                {
                    int projeNo = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEBILGI.ENUMAlanlar.PROJENO, 0);
                    int altProjeNo = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEBILGI.ENUMAlanlar.ALTPROJENO, 0);
                    string alanAd = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEBILGI.ENUMAlanlar.ALANAD, "");
                    string deger = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEBILGI.ENUMAlanlar.METIN, "");
                    if (projeAltProjeEKle)
                        alanAd = alanAd + "_" + projeNo + "_" + altProjeNo;
                    string eski;
                    try
                    {
                        eski = (string)ht[alanAd];
                    }
                    catch { eski = ""; }
                    ht[alanAd] = eski + deger;
                }
                dr.Close();
            }
            return ht;
        }

        internal static Sonuc AltProjeBilgiKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int projeKodu, int altProjeKodu, System.Collections.Hashtable degerler)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc s = ProjeClass.ProjeAltProjeYazmaHakkiVarMi(ve, kullanan, projeKodu, altProjeKodu, yil, senaryoKod);
            if (s != null)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABYATALTPROJEBILGI tab = new TABYATALTPROJEBILGI(ve);

            string[] alanlar = new string[degerler.Count];
            int say = 0;
            foreach (DictionaryEntry item in degerler)
            {
                alanlar[say++] = (string)item.Key;
            }

            ve.BeginTransaction();

            s = tab.Sil(yil, senaryoKod, projeKodu, altProjeKodu, alanlar);

            if (s.islemSonuc)
            {
                foreach (DictionaryEntry item in degerler)
                {
                    string alan = (string)item.Key;
                    string deger = (string)item.Value;

                    for (int i = 0; i < SatirSay(deger, 250); i++)
                    {
                        s = tab.Ekle(yil, senaryoKod, projeKodu, altProjeKodu, alan, i, SatirAl(deger, i, 250));
                        if (!s.islemSonuc)
                            break;
                    }
                    if (!s.islemSonuc)
                        break;
                }
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static string SatirAl(string deger, int satirNo, int satirUzunluk)
        {
            string donen = deger.Substring(satirNo * satirUzunluk);
            if (donen.Length > satirUzunluk)
                donen = donen.Substring(0, satirUzunluk);
            return donen;
        }

        private static int SatirSay(string deger, int satirUzunluk)
        {
            double l = deger.Length;
            return (int)Math.Ceiling(l / satirUzunluk);
        }

        public static string VERIERISIMAD = "BaglantiSatiriYAT";

        /// <summary>
        /// ALTPROJEYETKI listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ALTPROJEYETKI yapısında ALTPROJEYETKI:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleYetki(VeriErisim veriErisim, Kullanici kullanan, int projeKod, int altProjeKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATALTPROJEBILGI tab = new TABYATALTPROJEBILGI(ve);

            ObjectArray alAna = new ObjectArray();
            IDataReader dr = tab.Oku(projeKod, altProjeKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                alAna.sonuc.DegerAta(ve.hata);
                return alAna;
            }
            while (dr.Read())
            {
                Kullanici kul = new Kullanici();
                kul.kullaniciKodu = VeriErisim.DegerAl(dr, 2, "");
                kul.ad = VeriErisim.DegerAl(dr, 3, "");
                kul.soyad = VeriErisim.DegerAl(dr, 4, "");
                kul.EPOSTA = VeriErisim.DegerAl(dr, 5, "");
                kul.Telefon = VeriErisim.DegerAl(dr, 6, "");
                alAna.Ekle(kul);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return alAna;
        }

        /// <summary>
        /// ALTPROJEYETKI nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ALTPROJEYETKI yapısında ALTPROJEYETKI:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetYetki(VeriErisim veriErisim, Kullanici kullanan, int projeKod, int altProjeKod, TNSCollection kullanicilar, int yil, string senaryoKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc s = ProjeClass.ProjeAltProjeYazmaHakkiVarMi(ve, kullanan, projeKod, altProjeKod, yil, senaryoKod);

            if (s == null)
            {
                for (int i = 0; i < kullanicilar.Count - 1; i++)
                {
                    Kullanici ana = (Kullanici)kullanicilar[i];
                    for (int j = i + 1; j < kullanicilar.Count; j++)
                    {
                        Kullanici alt = (Kullanici)kullanicilar[j];
                        if (alt.kullaniciKodu == ana.kullaniciKodu)
                        {
                            if (s == null) s = new Sonuc();
                            s.DegerAta(alt.kullaniciKodu + " kullanıcı kodu birden fazla kullanılmış");
                            break;
                        }
                    }
                    if (s != null) break;
                }
            }
            if (s != null)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABYATALTPROJEBILGI tab = new TABYATALTPROJEBILGI(ve);

            ve.BeginTransaction();
            s = tab.Sil(projeKod, altProjeKod);
            if (s.islemSonuc)
            {
                foreach (Kullanici kul in kullanicilar)
                {
                    if (kul != null && !string.IsNullOrWhiteSpace(kul.KullaniciKodu))
                    {
                        s = tab.Ekle(kul.KullaniciKodu, projeKod, altProjeKod);
                        if (!s.islemSonuc) break;
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

        internal static Sonuc AltProjeTasi(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int apKod, int pKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATAltProje tabAltProje = new TABYATAltProje(ve);
            ve.BeginTransaction();
            Sonuc donusDeger = tabAltProje.Tasi(yil, senaryoKod, apKod, pKod);
            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}
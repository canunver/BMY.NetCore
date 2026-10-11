using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using System.Collections;
using System.Collections.Generic;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Birimlere ait daire bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBDaire
    {
        /// <summary>
        /// Daire varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Daire nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="daire">Daire bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Daire daire)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan, daire.kurKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(daire.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            else if (!PEBGenel.KurKodVarMi(daire.yil, daire.kurKod))
                hata += PerformansButceResI.PEB006 + "<br>";
            if (string.IsNullOrEmpty(daire.daireAd))
                hata += PerformansButceResI.PEB007 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABDaire tDaire = new TABDaire(ve);
            ve.BeginTransaction();

            if (tDaire.DaireVarMi(daire))
                sonuc = tDaire.Guncelle(daire);
            else if (daire.daireKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.PEB032 + "<br>";
            }
            else
                sonuc = tDaire.Kaydet(daire);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.PEB015;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.PEB016 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan daire bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="daire">Daire bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında daire listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Daire daire)
        {
            ObjectArray daireVAN = new ObjectArray();
            daireVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, daire.kurKod);
            if (!daireVAN.sonuc.islemSonuc)
                return daireVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABDaire tDaire = new TABDaire(ve);

            IDataReader dr = tDaire.Listele(daire);
            while (dr.Read())
            {
                Daire gelen = new Daire();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                gelen.daireKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.daireAd = VeriErisim.DegerAl(dr, 3, string.Empty);

                daireVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (daireVAN.objeler.Count == 0)
                daireVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return daireVAN;
        }

        /// <summary>
        /// Daire, operasyonel plan alt faaliyetlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Daire nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="daire">Daire bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Daire daire)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan, daire.kurKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(daire.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            if (daire.daireKod <= 0)
                hata += PerformansButceResI.PEB007 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABDaire tDaire = new TABDaire(ve);
            ve.BeginTransaction();

            if (tDaire.DaireKullanilmisMi(daire))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(PerformansButceResI.PEB033, daire.kurKod, daire.daireAd);
            }

            if (sonuc.islemSonuc)
                sonuc = tDaire.Sil(daire);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.PEB020;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.PEB021 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static Daire[] KullanilanDaireListele(VeriErisim veriErisim, bool sadeceEskiKayitlar)
        {
            List<Daire> al = new List<Daire>();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(null));
            TABDaire tDaire = new TABDaire(ve);

            IDataReader dr = tDaire.KullanilanDaireListeleEski();

            if (dr != null)
            {
                while (dr.Read())
                {
                    Daire d = new Daire();
                    d.yil = VeriErisim.DegerAl(dr, 0, 0);
                    d.kurKod = VeriErisim.DegerAl(dr, 1, "");
                    d.daireKod = VeriErisim.DegerAl(dr, 2, 0);
                    d.daireAd = VeriErisim.DegerAl(dr, 3, "");
                    al.Add(d);
                }
                dr.Close();
            }

            if (!sadeceEskiKayitlar)
            {
                dr = tDaire.KullanilanDaireListeleYeni();

                if (dr != null)
                {
                    while (dr.Read())
                    {
                        Daire d = new Daire();
                        d.yil = VeriErisim.DegerAl(dr, 0, 0);
                        d.kurKod = VeriErisim.DegerAl(dr, 1, "");
                        d.daireKod = VeriErisim.DegerAl(dr, 2, 0);
                        d.daireAd = VeriErisim.DegerAl(dr, 3, "");
                        al.Add(d);
                    }
                    dr.Close();
                }
            }

            ve.VeriErisimKapat();
            if (al.Count == 0) return null;
            return al.ToArray();
        }

        internal static Sonuc DaireyiOrgaCevir(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, int daireKod, int orgKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABDaire tDaire = new TABDaire(ve);

            ve.BeginTransaction();
            Sonuc s = tDaire.DaireEkle(yil, kurKod, daireKod, orgKod);
            if (s.islemSonuc)
                s = tDaire.AltFaaliyetEkle(yil, kurKod, daireKod, orgKod);
            if (s.islemSonuc)
                s = tDaire.IsGorevEkle(yil, kurKod, daireKod, orgKod);
            if (s.islemSonuc)
                s = tDaire.PEBISGOREVGOSTERGEDegis(yil, kurKod, daireKod, orgKod);
            if (s.islemSonuc)
                s = tDaire.PEBISGOREVSORUMLUDegis(yil, kurKod, daireKod, orgKod);
            if (s.islemSonuc)
                s = tDaire.PEBISGOREVZAMANLAMADegis(yil, kurKod, daireKod, orgKod);
            if (s.islemSonuc)
                s = tDaire.IsGorevSil(yil, kurKod, daireKod);
            if (s.islemSonuc)
                s = tDaire.AltFaaliyetSil(yil, kurKod, daireKod);
            if (s.islemSonuc)
                s = tDaire.DaireSil(yil, kurKod, daireKod);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return s;
        }

        internal static string AyniNumaraliFPSMListele(VeriErisim veriErisim)
        {
            string donusDeger = null;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(null));

            if (donusDeger == null)
            {
                TABSabitMaliyet tSM = new TABSabitMaliyet(ve);
                IDataReader dr = tSM.AyniNumaraliBul();
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    return ve.hata;
                }
                if (dr.Read())
                {
                    donusDeger = "S-" + VeriErisim.DegerAl(dr, 0, "") + "-" + VeriErisim.DegerAl(dr, 1, 0);
                }
                dr.Close();
            }
            if (donusDeger == null)
            {
                TABFaaliyetProje tFP = new TABFaaliyetProje(ve);
                IDataReader dr = tFP.AyniNumaraliBul();
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    return ve.hata;
                }
                if (dr.Read())
                {
                    donusDeger = "F-" + VeriErisim.DegerAl(dr, 0, "") + "-" + VeriErisim.DegerAl(dr, 1, 0);
                }
                dr.Close();

            }
            

            ve.VeriErisimKapat();
            return donusDeger;
        }

        internal static void AyniNumaraliFPSMDuzelt(VeriErisim veriErisim, Kullanici kullanan, string faalSmKod)
        {
            if (string.IsNullOrEmpty(faalSmKod)) return;
            string[] faalSmParcalar = faalSmKod.Split('-');
            if (faalSmParcalar.Length != 3) return;
            string kurKod = faalSmParcalar[1];
            if (string.IsNullOrEmpty(kurKod)) return;
            kurKod = kurKod.Replace(".", "");
            int fpSmNo = OrtakFonksiyonlar.ConvertToInt(faalSmParcalar[2], 0);
            if (fpSmNo <= 0) return;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc s = new Sonuc();
            TABDaire tFPSM = new TABDaire(ve);
            ve.BeginTransaction();
            if (faalSmParcalar[0] == "F")
            {
                TABFaaliyetProje tFP = new TABFaaliyetProje(ve);
                int enBuyukNo = tFP.FPKodAl();
                s.DegerAta(enBuyukNo.ToString());
                s = tFPSM.FPEkle(kurKod, fpSmNo, enBuyukNo);
                if (s.islemSonuc)
                    s = tFPSM.FPAgirlikDegistir(kurKod, fpSmNo, enBuyukNo);
                if (s.islemSonuc)
                    s = tFPSM.FPSMAgirlikDegistir(kurKod, fpSmNo, enBuyukNo, 0);
                if (s.islemSonuc)
                    s = tFPSM.FPSMAltFaaliyetDegistir(kurKod, fpSmNo, enBuyukNo, 0);
                if (s.islemSonuc)
                    s = tFPSM.FPSMPEBFonDegistir(kurKod, fpSmNo, enBuyukNo, 0);
                if (s.islemSonuc)
                    s = tFPSM.FPSMHarcamaYansiyanDegistir(kurKod, fpSmNo, enBuyukNo, 0);
                if (s.islemSonuc)
                    s = tFPSM.FPSMHarcamaDegistir(kurKod, fpSmNo, enBuyukNo, 0);
                if (s.islemSonuc)
                    s = tFPSM.FPSil(kurKod, fpSmNo);
            }
            else if (faalSmParcalar[0] == "S")
            {
                TABSabitMaliyet tSM = new TABSabitMaliyet(ve);
                int enBuyukNo = tSM.SMKodAl();
                s.DegerAta(enBuyukNo.ToString());
                s = tFPSM.SMEkle(kurKod, fpSmNo, enBuyukNo);
                if (s.islemSonuc)
                    s = tFPSM.FPSMAgirlikDegistir(kurKod, fpSmNo, enBuyukNo, 1);
                if (s.islemSonuc)
                    s = tFPSM.FPSMAltFaaliyetDegistir(kurKod, fpSmNo, enBuyukNo, 1);
                if (s.islemSonuc)
                    s = tFPSM.FPSMPEBFonDegistir(kurKod, fpSmNo, enBuyukNo, 1);
                if (s.islemSonuc)
                    s = tFPSM.FPSMHarcamaYansiyanDegistir(kurKod, fpSmNo, enBuyukNo, 1);
                if (s.islemSonuc)
                    s = tFPSM.FPSMHarcamaDegistir(kurKod, fpSmNo, enBuyukNo, 1);
                if (s.islemSonuc)
                    s = tFPSM.SMSil(kurKod, fpSmNo);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            if (!s.islemSonuc)
                throw new Exception(s.hataStr);
        }
    }
}
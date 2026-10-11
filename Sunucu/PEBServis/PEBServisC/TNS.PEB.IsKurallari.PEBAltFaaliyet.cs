using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Operasyonel plan alt faaliyet bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBAltFaaliyet
    {
        /// <summary>
        /// Operasyonel plan alt faaliyeti varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AltFaaliyet nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="af">Alt faaliyet bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AltFaaliyet af)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan, af.orgKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (af.orgKod < 0)
                hata += PerformansButceResI.PEB005 + "<br>";
            //else if (!PEBGenel.KurKodVarMi(af.yil, af.orgKod))
            //    hata += PerformansButceResI.PEB006 + "<br>";
            //if (af.daire.daireKod <= 0)
            //    hata += PerformansButceResI.PEB007 + "<br>";
            if (af.fp.fpKod <= 0 && af.sm.smKod <= 0)
                hata += PerformansButceResI.PEB008 + "<br>";
            if (af.fp.fpKod > 0 && af.sm.smKod > 0)
                hata += PerformansButceResI.PEB009 + "<br>";
            if (string.IsNullOrEmpty(af.afAd))
                hata += PerformansButceResI.PEB010 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));

            //af.daire.yil = af.yil;
            //af.daire.kurKod = af.orgKod;
            //if (!PEBGenel.DaireVarMi(ve, af.daire))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PerformansButceResI.PEB011 + "<br>";
            //    ve.VeriErisimKapat();
            //    return sonuc;
            //}

            if (af.fp.fpKod > 0)
            {
                if (!PEBGenel.FaaliyetProjeVarMi(ve, af.yil, af.fp.fpKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = PerformansButceResI.PEB012 + "<br>";
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            if (af.sm.smKod > 0)
            {
                if (!PEBGenel.SabitMaliyetVarMi(ve, af.yil, af.sm.smKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = PerformansButceResI.PEB013 + "<br>";
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            TABAltFaaliyet tAF = new TABAltFaaliyet(ve);
            ve.BeginTransaction();

            if (tAF.AltFaaliyetVarMi(af))
                sonuc = tAF.Guncelle(af);
            else if (af.afKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.PEB014 + "<br>";
            }
            else
                sonuc = tAF.Kaydet(af);

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = af.afKod.ToString();
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
        /// Verilen kriterlere uygun olan operasyonel plan alt faaliyeti bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="af">Alt faaliyet bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında operasyonel plan alt faaliyeti listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, AltFaaliyet af)
        {
            ObjectArray afVAN = new ObjectArray();
            //afVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, af.orgKod);
            //if (!afVAN.sonuc.islemSonuc)
            //    return afVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAltFaaliyet tAF = new TABAltFaaliyet(ve);

            IDataReader dr = tAF.Listele(af);
            while (dr.Read())
            {
                AltFaaliyet gelen = new AltFaaliyet();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.orgKod = OrtakFonksiyonlar.ConvertToInt(VeriErisim.DegerAl(dr, 1, string.Empty), 0);
                //gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                //gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                gelen.afKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.fp.fpKod = VeriErisim.DegerAl(dr, 3, 0);
                //gelen.daire.daireKod = VeriErisim.DegerAl(dr, 4, 0);
                gelen.afAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.sm.smKod = VeriErisim.DegerAl(dr, 6, 0);
                //gelen.fp.fpAd = VeriErisim.DegerAl(dr, 7, string.Empty);
                //gelen.daire.daireAd = VeriErisim.DegerAl(dr, 8, string.Empty);
                //gelen.sm.smAd = VeriErisim.DegerAl(dr, 9, string.Empty);

                afVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (afVAN.objeler.Count == 0)
                afVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return afVAN;
        }

        /// <summary>
        /// Operasyonel plan alt faaliyeti, iş/görevde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AltFaaliyet nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="af">Alt faaliyet bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, AltFaaliyet af)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan, af.orgKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (af.orgKod <= 0)
                hata += PerformansButceResI.PEB005 + "<br>";
            if (af.afKod <= 0)
                hata += PerformansButceResI.PEB018 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAltFaaliyet tAF = new TABAltFaaliyet(ve);
            ve.BeginTransaction();

            if (tAF.AltFaaliyetKullanilmisMi(af))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(PerformansButceResI.PEB019, af.orgKod, af.afAd);
            }

            if (sonuc.islemSonuc)
                sonuc = tAF.Sil(af);

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
    }
}
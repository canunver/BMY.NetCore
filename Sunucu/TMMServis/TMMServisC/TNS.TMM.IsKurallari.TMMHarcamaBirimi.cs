using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Harcama birimi bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMHarcamaBirimi
    {
        /// <summary>
        /// Harcama birimi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen HarcamaBirimi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, HarcamaBirimi harcamaBirimi)
        {
            Sonuc sonuc = new Sonuc();

            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (harcamaBirimi.kod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";

            if (harcamaBirimi.muhasebeKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";

            if (harcamaBirimi.ad == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM115 + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            Sonuc kodSonuc = OrtakFonksiyonlar.KodYapiKontrol(kodYapisi, harcamaBirimi.kod, true);

            if (!kodSonuc.islemSonuc)
            {
                sonuc.hataStr = kodSonuc.hataStr;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHarcamaBirimi tabHarcamaBirimi = new TABHarcamaBirimi(ve);
            TABMuhasebe tabMuhasebe = new TABMuhasebe(ve);

            Muhasebe m = new Muhasebe();
            m.kod = harcamaBirimi.muhasebeKod;
            IDataReader dr = tabMuhasebe.Listele(m);
            if (!dr.Read())
            {
                dr.Close();
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM116;
                return sonuc;
            }
            dr.Close();
            ve.BeginTransaction();

            sonuc = tabHarcamaBirimi.Kaydet(harcamaBirimi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM017;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM018 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Harcama birimi, ambarlarda kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen HarcamaBirimi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, HarcamaBirimi harcamaBirimi)
        {
            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (harcamaBirimi.kod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";

            if (harcamaBirimi.muhasebeKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHarcamaBirimi tabHarcamaBirimi = new TABHarcamaBirimi(ve);
            TABAmbar tabAmbar = new TABAmbar(ve);

            //Altında tanımlı ambar var mı?
            Ambar amb = new Ambar();
            amb.muhasebeKod = harcamaBirimi.muhasebeKod;
            amb.harcamaBirimKod = harcamaBirimi.kod;
            IDataReader drAmbar = tabAmbar.VarMi(amb);

            if (drAmbar.Read()) //Var...
            {
                drAmbar.Close();
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM117;
                return sonuc;
            }

            drAmbar.Close();
            ve.BeginTransaction();

            sonuc = tabHarcamaBirimi.Sil(harcamaBirimi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM025;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TasinirMalResI.TMM026 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan harcama birimi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi listesi döndürülür.</returns>
        public static ObjectArray Listele(Kullanici kullanan, HarcamaBirimi harcamaBirimi)
        {
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray bilgi = new ObjectArray();

            string[] hbler = new string[500];
            int hbSayac = 0;

            if (kullanan != null)
            {
                string yetkiTipiAdi = "";
                yetkiTipiAdi = "Taşınır Muhasebe Harcama Birim Yetkisi";
                string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

                yetkiTipiAdi = "Taşınır Kullanıcı Birimi Yetkisi";
                yetkiTipiDegeri += ";" + OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

                if (yetkiTipiDegeri.Trim() != "")
                {
                    string[] d = yetkiTipiDegeri.Split(';');

                    for (int i = 0; i < d.Length; i++)
                    {
                        string[] muhasebeBirim = d[i].Split('-');
                        if (muhasebeBirim.Length != 3 || muhasebeBirim[0] == "*")
                            continue;

                        hbler[hbSayac] = muhasebeBirim[1];
                        hbSayac++;
                    }
                }
            }

            TABHarcamaBirimi tabHarcamaBirimi = new TABHarcamaBirimi(ve);
            IDataReader dr = tabHarcamaBirimi.Listele(harcamaBirimi);

            while (dr.Read())
            {
                HarcamaBirimi hb = new HarcamaBirimi();
                hb.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                hb.ilceKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                hb.muhasebeKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                hb.kod = VeriErisim.DegerAl(dr, 3, string.Empty);
                hb.kod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, hb.kod);
                hb.ad = VeriErisim.DegerAl(dr, 4, string.Empty);
                hb.muhasebeAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                hb.il = VeriErisim.DegerAl(dr, 6, string.Empty);
                hb.ilce = VeriErisim.DegerAl(dr, 7, string.Empty);
                hb.kapali = VeriErisim.DegerAl(dr, 8, 0);

                //Yetkisi olan hb ler listelendiğinde devir için yapılan hb listelemesinde diğer hbler çıkmıyor
                //bool vardir = false;
                //if (hbSayac > 0)
                //{
                //    for (int i = 0; i < hbSayac; i++)
                //    {
                //        if (hb.kod.Replace(".", "") == hbler[i].Replace(".", ""))
                //        {
                //            vardir = true;
                //            break;
                //        }
                //    }
                //}
                //else
                //    vardir = true;

                //if (vardir)

                bool yetki = true;
                if (harcamaBirimi.kapali == -100)
                    yetki = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, hb.muhasebeKod, hb.kod);

                if (yetki)
                    bilgi.objeler.Add(hb);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan harcama birimi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi listesi döndürülür.</returns>
        public static ObjectArray ABSKuraGoreHarcamaBirimleriListele(Kullanici kullanan, int yil)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray bilgi = new ObjectArray();

            //string[] hbler = new string[500];
            //int hbSayac = 0;

            //if (kullanan != null)
            //{
            //    string yetkiTipiAdi = "";
            //    yetkiTipiAdi = "Taşınır Muhasebe Harcama Birim Yetkisi";
            //    string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

            //    yetkiTipiAdi = "Taşınır Kullanıcı Birimi Yetkisi";
            //    yetkiTipiDegeri += ";" + OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

            //    if (yetkiTipiDegeri.Trim() != "")
            //    {
            //        string[] d = yetkiTipiDegeri.Split(';');

            //        for (int i = 0; i < d.Length; i++)
            //        {
            //            string[] muhasebeBirim = d[i].Split('-');
            //            if (muhasebeBirim.Length != 3 || muhasebeBirim[0] == "*")
            //                continue;

            //            hbler[hbSayac] = muhasebeBirim[1];
            //            hbSayac++;
            //        }
            //    }
            //}

            TABHarcamaBirimi tabHarcamaBirimi = new TABHarcamaBirimi(ve);
            IDataReader dr = tabHarcamaBirimi.ABSKuraGoreHarcamaBirimleriListele(yil);

            while (dr.Read())
            {
                HarcamaBirimi hb = new HarcamaBirimi();
                hb.kod = VeriErisim.DegerAl(dr, 0, string.Empty);
                //hb.kod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, hb.kod);
                hb.ad = VeriErisim.DegerAl(dr, 1, string.Empty);

                bilgi.objeler.Add(hb);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
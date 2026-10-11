using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;
using TNS.BHMv2;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Faaliyet/proje bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBFaaliyetProje
    {
        /// <summary>
        /// Faaliyet/proje varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen FaaliyetProje nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin de ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// Aynı zamanda bütçe hazırlık alt faaliyetlerle bağının da ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, FaaliyetProje fp)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan, fp.kurKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(fp.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            else if (!PEBGenel.KurKodVarMi(fp.yil, fp.kurKod))
                hata += PerformansButceResI.PEB006 + "<br>";
            if (string.IsNullOrEmpty(fp.fpAd))
                hata += PerformansButceResI.PEB034 + "<br>";
            if (fp.fpAgirlik.Count <= 0 && fp.tur != (int)ENUMFaaliyetProjeTur.DigerKurumIci && fp.tur != (int)ENUMFaaliyetProjeTur.DigerKurumDisi)
                hata += PerformansButceResI.PEB035 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABFaaliyetProje tFP = new TABFaaliyetProje(ve);
            ve.BeginTransaction();

            if (tFP.FPVarMi(fp.yil, fp.fpKod))
                sonuc = tFP.Guncelle(fp);
            else if (fp.fpKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.PEB036 + "<br>";
            }
            else
            {
                sonuc = tFP.Kaydet(fp);
                fp.fpKod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar.Trim(), 0);
            }

            if (sonuc.islemSonuc && fp.tur != (int)ENUMFaaliyetProjeTur.DigerKurumIci && fp.tur != (int)ENUMFaaliyetProjeTur.DigerKurumDisi)
            {
                foreach (FaaliyetProjeAgirlik fps in fp.fpAgirlik)
                {
                    fps.yil = fp.yil;
                    fps.fpKurKod = fp.kurKod;
                    fps.fpKod = fp.fpKod;
                }
                sonuc = PEBFaaliyetProjeAgirlik.Kaydet(ve, kullanan, fp.fpAgirlik);
            }

            //if (sonuc.islemSonuc)
            //    sonuc = PEBFonKodlariSilKaydet(tFP, fp.yil, fp.kurKod, 0, fp.fpKod, fp.fpPEBFonKodlar);

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = fp.fpKod.ToString();
                sonuc.bilgiStr = PerformansButceResI.PEB015;
                ve.Commit();
                TNS.KYM.IKYMServis kymServis = TNS.KYM.Arac.Tanimla();
                kymServis.BuyukMetinSakla(kullanan, "PEB", "fpa" + fp.yil + fp.kurKod + fp.fpKod, kullanan.aktifKultur, fp.fpAciklama);
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
        /// Faaliyet/projenin veya sabit maliyetin bütçe alt faaliyetlerini önce silip sonra kaydeden yordam
        /// </summary>
        /// <param name="tFP">Bütçe alt faaliyet bilgilerini tutan veri tabanı tablosuyla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="sabitMi">Faaliyet/proje mi (0 değeri) sabit maliyet mi (1 değeri) bilgisi</param>
        /// <param name="maliyetKod">Faaliyet/proje veya sabit maliyet programatik kodu</param>
        /// <param name="pebKodlar">Bütçe alt faaliyet listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc PEBFonKodlariSilKaydet(TABFaaliyetProje tFP, int yil, string kurKod, int sabitMi, int maliyetKod, TNSCollection pebKodlar)
        {
            Sonuc sonuc = tFP.PEBFonKodlariSil(yil, kurKod, sabitMi, maliyetKod);
            foreach (string pebFonKod in pebKodlar)
            {
                if (sonuc.islemSonuc)
                    sonuc = tFP.PEBFonKodKaydet(yil, kurKod, sabitMi, maliyetKod, pebFonKod);
                else
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="aktarEbutce">e-butce sistemine aktarım için mi çağrılıyor yoksa sistem kullanımı için mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, FaaliyetProje fp, bool detayAgirlik, bool aktarEbutce)
        {
            return Listele(veriErisim, kullanan, fp, detayAgirlik, 0, aktarEbutce);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="perHedefKod">Performans hedef programatik kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, FaaliyetProje fp, bool detayAgirlik, int perHedefKod)
        {
            return Listele(veriErisim, kullanan, fp, detayAgirlik, perHedefKod, false);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="perHedefKod">Performans hedef programatik kodu</param>
        /// <param name="aktarEbutce">e-butce sistemine aktarım için mi çağrılıyor yoksa sistem kullanımı için mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, FaaliyetProje fp, bool detayAgirlik, int perHedefKod, bool aktarEbutce)
        {
            ObjectArray fpVAN = new ObjectArray();
            //fpVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, fp.kurKod);
            //if (!fpVAN.sonuc.islemSonuc)
            //    return fpVAN;

            if (detayAgirlik && !aktarEbutce)
            {
                string hata = string.Empty;
                if (string.IsNullOrEmpty(fp.kurKod))
                    hata += PerformansButceResI.PEB005 + "<br>";
                if (fp.fpKod <= 0)
                    hata += PerformansButceResI.PEB037 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    fpVAN.sonuc.islemSonuc = false;
                    fpVAN.sonuc.hataStr = hata;
                    return fpVAN;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFaaliyetProje tFP = new TABFaaliyetProje(ve);

            IDataReader dr = tFP.Listele(fp, perHedefKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    FaaliyetProje gelen = new FaaliyetProje();
                    gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                    gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                    gelen.fpKod = VeriErisim.DegerAl(dr, 2, 0);
                    gelen.fpAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                    gelen.projeMi = VeriErisim.DegerAl(dr, 4, 0);
                    gelen.sorumlu = VeriErisim.DegerAl(dr, 5, string.Empty);
                    gelen.seviye = VeriErisim.DegerAl(dr, 6, string.Empty);
                    gelen.tur = VeriErisim.DegerAl(dr, 7, 0);
                    gelen.ebutceKod = VeriErisim.DegerAl(dr, 8, string.Empty);

                    fpVAN.objeler.Add(gelen);
                }
                dr.Close();
            }
            else
                fpVAN.sonuc.DegerAta(string.Format(PerformansButceResI.PEB038, ve.hata));

            if (detayAgirlik && fpVAN.sonuc.islemSonuc)
            {
                foreach (FaaliyetProje gelen in fpVAN.objeler)
                {
                    ObjectArray fpAgirlik = PEBFaaliyetProjeAgirlik.Listele(ve, kullanan, gelen.yil, gelen.kurKod, gelen.fpKod);
                    if (!fpAgirlik.sonuc.islemSonuc)
                    {
                        fpVAN.sonuc = fpAgirlik.sonuc;
                        break;
                    }
                    else
                        gelen.fpAgirlik = fpAgirlik.objeler;

                    gelen.fpPEBFonKodlar = PEBFonlariBul(ve, gelen.yil, gelen.kurKod, 0, gelen.fpKod);
                }
            }

            if (fpVAN.objeler.Count == 0)
                fpVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            if (fpVAN.objeler.Count == 1)
            {
                FaaliyetProje gelen = (FaaliyetProje)fpVAN.objeler[0];
                TNS.KYM.IKYMServis kymServis = TNS.KYM.Arac.Tanimla();
                gelen.fpAciklama = kymServis.BuyukMetinOku(kullanan, "PEB", "fpa" + gelen.yil + gelen.kurKod + gelen.fpKod, kullanan.aktifKultur);
            }

            ve.VeriErisimKapat();
            return fpVAN;
        }

        /// <summary>
        /// Faaliyet/projenin ilgili birimlerini alt faaliyet bağı üzerinden bütçe hazırlıkta
        /// ödenek girişleri sırasında kullanılmış tertiplerden bulup döndüren yordam
        /// Faaliyet/projeler e-butce sistemine aktarılırken ilgili birimleri de istendiği için yazıldı.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/projeye ait ilgili birimler string olarak liste şeklinde döndürülür.</returns>
        public static ObjectArray IlgiliBirimleriBul(VeriErisim veriErisim, Kullanici kullanan, FaaliyetProje fp, string senaryoKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, fp.kurKod);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            string hata = string.Empty;
            if (fp.yil <= 0)
                hata = PerformansButceResI.PEB039 + "<br>";
            if (string.IsNullOrEmpty(fp.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            if (fp.fpKod <= 0)
                hata += PerformansButceResI.PEB037;
            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));

            Tertip tertip = new Tertip(fp.yil, ENUMTabloKod.TABPEBOdenek);
            tertip.kodlar[0] = fp.kurKod;
            tertip.kodlar[4] = PEBDiziToStr(PEBFonlariBul(ve, fp.yil, fp.kurKod, 0, fp.fpKod));
            if (string.IsNullOrEmpty(tertip.kodlar[4]))
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            UcgenKodBilgileri kodBilgi = new UcgenKodBilgileri();
            kodBilgi.Add(new UcgenKodBilgi(1, tertip.Kirilimlar(0).Split('.').Length));

            EkDegerParam[] ekBilgi = new EkDegerParam[1];
            ekBilgi[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);

            IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            OdenekBilgi[] odnler = servisBHMv2.ListeTertipOl(kullanan, tertip, kodBilgi, ekBilgi, true);
            if (odnler.Length <= 0)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            foreach (OdenekBilgi odn in odnler)
                if ((double)odn.ekRakamlar[0] > 0)
                    donusDeger.objeler.Add(odn.tertip.kodlar[0]);

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bütçe alt faaliyet kodlarını virgül
        /// ayracı ile birleştirerek string olarak döndüren yordam
        /// </summary>
        /// <param name="pebKodlar">Bütçe alt faaliyet kodları listesini tutan nesne</param>
        /// <returns>Virgüllerle ayrılmış bütçe alt faaliyet kodları döndürülür.</returns>
        private static string PEBDiziToStr(TNSCollection pebKodlar)
        {
            if (pebKodlar.Count <= 0)
                return string.Empty;

            string kodlar = string.Empty;
            foreach (string peb in pebKodlar)
            {
                if (!string.IsNullOrEmpty(kodlar))
                    kodlar += ",";
                kodlar += peb;
            }
            return kodlar;
        }

        /// <summary>
        /// Faaliyet/projenin veya sabit maliyetin bütçe alt faaliyetlerini bulup döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="sabitMi">Faaliyet/proje mi (0 değeri) sabit maliyet mi (1 değeri) bilgisi</param>
        /// <param name="maliyetKod">Faaliyet/proje veya sabit maliyet programatik kodu</param>
        /// <returns>Bütçe alt faaliyet kodları listesi döndürülür.</returns>
        public static TNSCollection PEBFonlariBul(VeriErisim veriErisim, int yil, string kurKod, int sabitMi, int maliyetKod)
        {
            TNSCollection kodlar = new TNSCollection();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(null));

            TABFaaliyetProje tFP = new TABFaaliyetProje(ve);

            IDataReader dr = tFP.ListelePenFonlar(yil, kurKod, sabitMi, maliyetKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    string peb = PEBGenel.PebKodNoktala(VeriErisim.DegerAl(dr, 0, ""));
                    kodlar.Add(peb);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return kodlar;
        }

        /// <summary>
        /// Faaliyet/proje, sabit maliyetlerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fpler">FaaliyetProje nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection fpler)
        {
            Sonuc sonuc = new Sonuc(false, PerformansButceResI.PEB040, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABFaaliyetProje tFP = new TABFaaliyetProje(ve);
            TABFaaliyetProjeAgirlik tFPAgirlik = new TABFaaliyetProjeAgirlik(ve);
            //TABOdenek tOdenek = new TABOdenek(ve);
            ve.BeginTransaction();

            foreach (FaaliyetProje fp in fpler)
            {
                sonuc = Yetki.YazmaHakkiVarMi(kullanan, fp.kurKod);
                if (!sonuc.islemSonuc)
                    break;

                if (tFP.FPKullanilmisMi(fp.yil, fp.kurKod, fp.fpKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(PerformansButceResI.PEB041, fp.kurKod, fp.fpKod.ToString());
                    break;
                }

                //Faaliyet/Projenin ödenekleri siliniyor.
                //sonuc = tOdenek.Sil(fp.yil, fp.kurKod, fp.asama, fp.fpKod, (int)ENUMOdenekTur.FaaliyetProje);
                //if (!sonuc.islemSonuc)
                //    break;

                //Faaliyet/Projenin PEB Kodları siliniyor.
                sonuc = tFP.PEBFonKodlariSil(fp.yil, fp.kurKod, 0, fp.fpKod);
                if (!sonuc.islemSonuc)
                    break;

                //Faaliyet/Projenin ağırlıkları siliniyor.
                FaaliyetProjeAgirlik agirlik = new FaaliyetProjeAgirlik();
                agirlik.yil = fp.yil;
                agirlik.fpKurKod = fp.kurKod;
                agirlik.fpKod = fp.fpKod;
                sonuc = tFPAgirlik.Sil(agirlik);
                if (!sonuc.islemSonuc)
                    break;

                //Faaliyet/Projenin kendisi siliniyor.
                sonuc = tFP.Sil(fp);
                if (!sonuc.islemSonuc)
                    break;
            }

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
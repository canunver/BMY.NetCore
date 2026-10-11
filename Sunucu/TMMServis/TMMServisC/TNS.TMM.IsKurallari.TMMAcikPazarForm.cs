using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// İhtiyaç fazlası taşınır bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMAcikPazarForm
    {
        /// <summary>
        /// İhtiyaç fazlası taşınır form ve detay bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AcikPazarForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KaydetAcikPazar(VeriErisim veriErisim, Kullanici kullanan, AcikPazarForm apf)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(apf.muhasebeKod))
                hata += TasinirMalResI.TMM013 + "<br>";
            if (string.IsNullOrEmpty(apf.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (apf.detaylar.Count <= 0)
                hata += TasinirMalResI.TMM015 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, apf.muhasebeKod, apf.harcamaKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM016;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            sonuc = KaydetForm(ve, apf);
            if (sonuc.islemSonuc)
                sonuc = KaydetDetay(ve, apf.detaylar);

            //Kayıt işlemi tamamlandıktan sonra
            //Detayı olmayan istek varsa siliniyor.
            if (sonuc.islemSonuc)
            {
                ((AcikPazarDetay)apf.detaylar[apf.detaylar.Count - 1]).prSicilNo = 0;
                sonuc = SilIstekGiris(ve, (AcikPazarDetay)apf.detaylar[apf.detaylar.Count - 1]);
            }

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
        /// İhtiyaç fazlası taşınır formunun üst bilgilerini kaydeden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="apf">İhtiyaç fazlası taşınır formunun üst bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc KaydetForm(VeriErisim ve, AcikPazarForm apf)
        {
            Sonuc sonuc = new Sonuc();
            TABAcikPazarForm tAPF = new TABAcikPazarForm(ve);

            //Belki ilerde güncelleme de gerekebilir
            //Ama şu anda güncellenecek alan yok
            if (!tAPF.AcikPazarFormVarMi(apf))
                sonuc = tAPF.Kaydet(apf);

            return sonuc;
        }

        /// <summary>
        /// İhtiyaç fazlası taşınır formunun detay bilgilerini kaydeden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="detaylar">İhtiyaç fazlası taşınır formunun detay bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc KaydetDetay(VeriErisim ve, TNSCollection detaylar)
        {
            TABAcikPazarDetay tAPD = new TABAcikPazarDetay(ve);

            AcikPazarDetay detay = (AcikPazarDetay)detaylar[0];
            Sonuc sonuc = tAPD.Sil(detay);
            if (!sonuc.islemSonuc)
                return sonuc;

            foreach (AcikPazarDetay apd in detaylar)
            {
                apd.prSicilNo = (int)TMMServis.UzaySayiDegerGetir("TASPRSICILGETIR", apd.muhasebeKod + "-" + apd.harcamaKod + "-" + apd.gorSicilNo, true, "");

                if (apd.prSicilNo <= 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(TasinirMalResI.TMM019, apd.gorSicilNo) + "<br>";
                    break;
                }

                if (apd.eklenisTarihi.isNull)
                    apd.eklenisTarihi.Yaz(DateTime.Now.Date);

                sonuc = tAPD.Kaydet(apd);

                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// İhtiyaç fazlası taşınır malzemelerine yapılan istek bilgilerini kaydeden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="istekler">İhtiyaç fazlası taşınır malzemelerine yapılan istek listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc KaydetIstek(VeriErisim ve, TNSCollection istekler)
        {
            Sonuc sonuc = new Sonuc();
            TABAcikPazarIstek tAPI = new TABAcikPazarIstek(ve);
            TABAcikPazarDetay tAPD = new TABAcikPazarDetay(ve);
            foreach (AcikPazarIstek api in istekler)
            {
                string hata = string.Empty;
                if (string.IsNullOrEmpty(api.muhasebeKod))
                    hata += TasinirMalResI.TMM013 + "<br>";
                if (string.IsNullOrEmpty(api.harcamaKod))
                    hata += TasinirMalResI.TMM014 + "<br>";
                if (string.IsNullOrEmpty(api.gorSicilNo))
                    hata += TasinirMalResI.TMM020 + "<br>";

                if (string.IsNullOrEmpty(hata))
                {
                    AcikPazarDetay apd = new AcikPazarDetay();
                    apd.muhasebeKod = api.muhasebeKod;
                    apd.harcamaKod = api.harcamaKod;
                    apd.prSicilNo = api.prSicilNo;
                    if (!tAPD.AcikPazardaMi(apd))
                        hata += string.Format(TasinirMalResI.TMM021, api.gorSicilNo) + "<br>";
                }

                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }

                if (api.istekTarihi.isNull)
                    api.istekTarihi.Yaz(DateTime.Now.Date);

                sonuc = tAPI.Kaydet(api);

                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// İhtiyaç fazlası taşınır malzemelerine yapılan istek bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bilgiler eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <param name="istekler">İhtiyaç fazlası taşınır malzemelerine yapılan istek listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KaydetAcikPazarIstek(VeriErisim veriErisim, Kullanici kullanan, AcikPazarForm apf, TNSCollection istekler)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(apf.istekYapanMuhasebeKod))
                hata += TasinirMalResI.TMM022 + "<br>";
            if (string.IsNullOrEmpty(apf.istekYapanHarcamaKod))
                hata += TasinirMalResI.TMM023 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, apf.istekYapanMuhasebeKod, apf.istekYapanHarcamaKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM016;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            sonuc = SilIstekSorgu(ve, apf);
            if (sonuc.islemSonuc)
                sonuc = KaydetIstek(ve, istekler);

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
        /// İhtiyaç fazlası taşınır form, detay ve istek bilgilerini silen yordam
        /// tif parametresi true ise sadece taşınır işlem fişi kesilmiş malzemeler formun detayından ve
        /// istek listesinden silinir, false ise forma ait tüm bilgiler (form, detayı, istekler) silinir.
        /// Kullanıcı işleme yetkili değilse veya gönderilen AcikPazarForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <param name="tif">Silme isteğinin taşınır işlem fişi oluşturulmasından kaynaklı olup olmadığı bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SilAcikPazar(VeriErisim veriErisim, Kullanici kullanan, AcikPazarForm apf, bool tif)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(apf.muhasebeKod))
                hata += TasinirMalResI.TMM013 + "<br>";
            if (string.IsNullOrEmpty(apf.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, apf.muhasebeKod, apf.harcamaKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM024;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            if (tif)
            {
                foreach (AcikPazarDetay detay in apf.detaylar)
                {
                    if (detay.prSicilNo <= 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = TasinirMalResI.TMM020 + "<br>";
                        break;
                    }

                    sonuc = SilDetay(ve, detay);
                    if (sonuc.islemSonuc)
                        sonuc = SilIstekGiris(ve, detay);

                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            else
            {
                //Bu forma ait tüm istekler, detaylar ve formun kendisi silinecek
                AcikPazarDetay detay = new AcikPazarDetay();
                detay.muhasebeKod = apf.muhasebeKod;
                detay.harcamaKod = apf.harcamaKod;

                sonuc = SilDetay(ve, detay);
                if (sonuc.islemSonuc)
                    sonuc = SilIstekGiris(ve, detay);
                if (sonuc.islemSonuc)
                    sonuc = SilForm(ve, apf);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM025;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM026 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// İhtiyaç fazlası taşınır formunun üst bilgilerini silen yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="apf">İhtiyaç fazlası taşınır formunun üst bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc SilForm(VeriErisim ve, AcikPazarForm apf)
        {
            TABAcikPazarForm tAPF = new TABAcikPazarForm(ve);
            return tAPF.Sil(apf);
        }

        /// <summary>
        /// İhtiyaç fazlası taşınır formunun detay bilgilerini silen yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="apd">İhtiyaç fazlası taşınır formunun detay bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc SilDetay(VeriErisim ve, AcikPazarDetay apd)
        {
            TABAcikPazarDetay tAPD = new TABAcikPazarDetay(ve);
            return tAPD.Sil(apd);
        }

        /// <summary>
        /// İhtiyaç fazlası taşınır malzemelerine yapılan istek bilgilerini silen yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="apd">İhtiyaç fazlası taşınır formunun detay bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc SilIstekGiris(VeriErisim ve, AcikPazarDetay apd)
        {
            TABAcikPazarIstek tAPI = new TABAcikPazarIstek(ve);
            return tAPI.SilGiris(apd);
        }

        /// <summary>
        /// İhtiyaç fazlası taşınır malzemelerine yapılan istek bilgilerini silen yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="apf">İhtiyaç fazlası taşınır formunun üst bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc SilIstekSorgu(VeriErisim ve, AcikPazarForm apf)
        {
            TABAcikPazarIstek tAPI = new TABAcikPazarIstek(ve);
            return tAPI.SilSorgu(apf);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ihtiyaç fazlası taşınır bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <param name="sorgudan">İsteğin sorgu ekranından mı yoksa form ekranından mı yapıldığı bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ihtiyaç fazlası taşınır bilgileri listesi döndürülür.</returns>
        public static ObjectArray ListeleAcikPazar(VeriErisim veriErisim, Kullanici kullanan, AcikPazarForm apf, bool sorgudan)
        {
            ObjectArray apfVAN = new ObjectArray();

            string hata = string.Empty;
            if (sorgudan)
            {
                if (string.IsNullOrEmpty(apf.istekYapanMuhasebeKod))
                    hata += TasinirMalResI.TMM022 + "<br>";
                if (string.IsNullOrEmpty(apf.istekYapanHarcamaKod))
                    hata += TasinirMalResI.TMM023 + "<br>";
                if (string.IsNullOrEmpty(apf.hesapKod))
                    hata += TasinirMalResI.TMM027 + "<br>";
            }
            else
            {
                if (string.IsNullOrEmpty(apf.muhasebeKod))
                    hata += TasinirMalResI.TMM013 + "<br>";
                if (string.IsNullOrEmpty(apf.harcamaKod))
                    hata += TasinirMalResI.TMM014 + "<br>";
            }
            if (!string.IsNullOrEmpty(hata))
            {
                apfVAN.sonuc.islemSonuc = false;
                apfVAN.sonuc.hataStr = hata;
                return apfVAN;
            }

            if (sorgudan)
                apfVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, apf.istekYapanMuhasebeKod, apf.istekYapanHarcamaKod);
            else
                apfVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, apf.muhasebeKod, apf.harcamaKod);

            if (!apfVAN.sonuc.islemSonuc)
            {
                apfVAN.sonuc.hataStr = TasinirMalResI.TMM028;
                return apfVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAcikPazarDetay tAPD = new TABAcikPazarDetay(ve);

            IDataReader dr = tAPD.Listele(apf, sorgudan);
            while (dr.Read())
            {
                AcikPazarDetay gelen = new AcikPazarDetay();
                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.muhasebeAd = VeriErisim.DegerAl(dr, 6, string.Empty);

                gelen.harcamaKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                gelen.harcamaAd = VeriErisim.DegerAl(dr, 7, string.Empty);

                gelen.prSicilNo = VeriErisim.DegerAl(dr, 2, 0);
                gelen.siraNo = VeriErisim.DegerAl(dr, 3, 0);
                gelen.gorSicilNo = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.hesapKod = VeriErisim.DegerAl(dr, 9, string.Empty);
                gelen.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.hesapKod);
                gelen.hesapAd = VeriErisim.DegerAl(dr, 10, string.Empty);
                gelen.kdvOran = VeriErisim.DegerAl(dr, 11, 0);
                gelen.birimFiyat = VeriErisim.DegerAl(dr, 12, (decimal)0);
                gelen.aciklama = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.eklenisTarihi = VeriErisim.DegerAl(dr, 5);

                apf.detaylar.Add(gelen);
            }
            dr.Close();

            //İstek olanlar bulunup işaretlenecek
            TABAcikPazarIstek tAPI = new TABAcikPazarIstek(ve);

            AcikPazarIstek istek = new AcikPazarIstek();
            istek.muhasebeKod = apf.muhasebeKod;
            istek.harcamaKod = apf.harcamaKod;
            istek.istekYapanMuhasebeKod = apf.istekYapanMuhasebeKod;
            istek.istekYapanHarcamaKod = apf.istekYapanHarcamaKod;

            dr = tAPI.Listele(istek);
            while (dr.Read())
            {
                foreach (AcikPazarDetay detay in apf.detaylar)
                {
                    string muhasebeKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    string harcamaKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), harcamaKod);
                    int sicilNo = VeriErisim.DegerAl(dr, 2, 0);
                    if (detay.muhasebeKod == muhasebeKod && detay.harcamaKod == harcamaKod && detay.prSicilNo == sicilNo)
                        detay.istekVar = true;
                }
            }
            dr.Close();

            if (apf.detaylar.Count > 0)
                apfVAN.objeler.Add(apf);

            if (apfVAN.objeler.Count == 0)
                apfVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return apfVAN;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ihtiyaç fazlası taşınır istek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="istek">İhtiyaç fazlası taşınır istek bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ihtiyaç fazlası taşınır istek bilgileri listesi döndürülür.</returns>
        public static ObjectArray ListeleAcikPazarIstek(VeriErisim veriErisim, Kullanici kullanan, AcikPazarIstek istek)
        {
            ObjectArray istekVAN = new ObjectArray();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(istek.muhasebeKod))
                hata += TasinirMalResI.TMM013 + "<br>";
            if (string.IsNullOrEmpty(istek.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (istek.prSicilNo <= 0)
                hata += TasinirMalResI.TMM020 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                istekVAN.sonuc.islemSonuc = false;
                istekVAN.sonuc.hataStr = hata;
                return istekVAN;
            }

            istekVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, istek.muhasebeKod, istek.harcamaKod);
            if (!istekVAN.sonuc.islemSonuc)
            {
                istekVAN.sonuc.hataStr = TasinirMalResI.TMM028;
                return istekVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAcikPazarIstek tAPI = new TABAcikPazarIstek(ve);

            IDataReader dr = tAPI.Listele(istek);
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                AcikPazarIstek gelen = new AcikPazarIstek();
                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.muhasebeAd = Genel.AdGetir(isimler, gelen.muhasebeKod, "TASMUHASEBE");

                gelen.harcamaKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                gelen.harcamaAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod, "TASHARCAMABIRIM");

                gelen.prSicilNo = VeriErisim.DegerAl(dr, 2, 0);

                gelen.istekYapanMuhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.istekYapanMuhasebeAd = Genel.AdGetir(isimler, gelen.istekYapanMuhasebeKod, "TASMUHASEBE");

                gelen.istekYapanHarcamaKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.istekYapanHarcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.istekYapanHarcamaKod);
                gelen.istekYapanHarcamaAd = Genel.AdGetir(isimler, gelen.istekYapanMuhasebeKod + "-" + gelen.istekYapanHarcamaKod, "TASHARCAMABIRIM");

                gelen.istekTarihi = VeriErisim.DegerAl(dr, 5);

                istekVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (istekVAN.objeler.Count == 0)
                istekVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            return istekVAN;
        }
    }
}
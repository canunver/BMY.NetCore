using System;
using System.Collections;
using System.Text.RegularExpressions;
using TNS.KYM;
using TNS.Veri;

namespace TNS.TRM
{
    /// <summary>
    /// Tarim sınıf için istemci ve sunucuda ortak kullanılacak statik fonksiyonlar tutan sınıf.
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        public static string anketModul = "TRM";
        public static string[] anketAnahtar = { "EKO_07_001", "EKO_07_002", "EKO_07_003", "EKO_07_004" };

        /// <summary>
        /// Tarim sunucu nesnesi uyandıran yordam.
        /// </summary>
        /// <returns>Tarim sunucu nesnesi döndürülür.</returns>
        public static ITRMServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("TRMServisURL");
            ITRMServis servis = TNS.Api.ServisIstemci.Olustur<ITRMServis>(sUrl);
            return servis;
        }

        public static string EtapAdiVer(ENUMHibeTur hibeTur, int etapNo)
        {
            if (hibeTur == ENUMHibeTur.SULAMA || hibeTur == ENUMHibeTur.MAKINE)
                return etapNo.ToString();
            else if (etapNo >= 17)
            {
                return (etapNo - 2).ToString();
            }
            else if (etapNo >= 14)
            {
                int surum = (etapNo - 14) + 1;
                return "14." + surum;
            }
            else
                return etapNo.ToString();
        }
        public static string EtapAdiVer(int etapNo)
        {
            return EtapAdiVer(ENUMHibeTur.EKONOMIKYATIRIM, etapNo);
        }

        public static int EtapKoduVer(ENUMHibeTur hibeTur, string etapAd)
        {
            etapAd = etapAd.Trim();
            int etapKod = OrtakFonksiyonlar.ConvertToInt(etapAd, 0);
            if (hibeTur == ENUMHibeTur.SULAMA || hibeTur == ENUMHibeTur.MAKINE)
                return etapKod;
            if (etapAd == "14.1") return 14;
            if (etapAd == "14.2") return 15;
            if (etapAd == "14.3") return 16;

            if (etapKod > 14) return etapKod + 2;
            else return etapKod;
        }

        public static string KimlikNoGizle(string kimlikNo)
        {
            kimlikNo = kimlikNo.Substring(0, 2) + "*******" + kimlikNo.Substring(kimlikNo.Length - 2, 2);

            return kimlikNo;
        }

        public static double HashtableDegerVerDbl(Hashtable ht, string anahtar)
        {
            if (ht[anahtar] == null)
                return 0;
            else
                return OrtakFonksiyonlar.ConvertToDbl(ht[anahtar]);
        }
        public static int HashtableDegerVerInt(Hashtable ht, string anahtar)
        {
            if (ht[anahtar] == null)
                return 0;
            else
                return OrtakFonksiyonlar.ConvertToInt(ht[anahtar], 0);
        }
        public static void HashtableDegerToplaInt(Hashtable ht, string anahtar, int deger)
        {
            int sakliDeger = HashtableDegerVerInt(ht, anahtar);
            sakliDeger += deger;
            ht[anahtar] = sakliDeger;
        }
        public static void HashtableDegerToplaDbl(Hashtable ht, string anahtar, double deger)
        {
            double sakliDeger = HashtableDegerVerDbl(ht, anahtar);
            sakliDeger += deger;
            ht[anahtar] = sakliDeger;
        }
        public static string MaskeleTC(string alan)
        {
            //12345678901 : 12*******01
            if (string.IsNullOrWhiteSpace(alan))
                return alan;

            string donenDeger = alan.Replace(" ", "");
            int uzunluk = donenDeger.Length;

            if (uzunluk > 4)
            {
                try
                {
                    var mask = new string('*', uzunluk - 4);
                    var bas = donenDeger.Substring(0, 2);
                    var bit = donenDeger.Substring(uzunluk - 2, 2);

                    donenDeger = string.Concat(bas, mask, bit).ToString();
                }
                catch
                {
                    donenDeger = alan;
                }
            }
            return donenDeger;
        }
        public static string MaskeleTel(string alan)
        {
            //05321113344   : 053******44
            //5321113344    : 53******44
            //0(532)1113344 : 0(53*******44
            if (string.IsNullOrWhiteSpace(alan))
                return alan;

            string donenDeger = alan.Replace(" ", "");

            try
            {
                int baslangic = 2;
                int bitis = 2;

                if (donenDeger.StartsWith("0"))
                    baslangic++;

                if (donenDeger.Contains("("))
                    baslangic++;

                var mask = new string('*', donenDeger.Length - baslangic - bitis);
                var bas = donenDeger.Substring(0, baslangic);
                var bit = donenDeger.Substring(donenDeger.Length - 2, 2);

                donenDeger = string.Concat(bas, mask, bit).ToString();
            }
            catch
            {
                donenDeger = alan;
            }

            return donenDeger;
        }
    }

    /// <summary>
    /// Yetki kontrollerini düzenleyen fonksiyonları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Yetki
    {
        /// <summary>
        /// Tarim Servis Tanımı
        /// </summary>
        private static ITRMServis servis = Arac.Tanimla();

        public static bool YoneticiMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            return false;
        }

        public static bool YetkisiVarMiDestekTurune(Kullanici kullanan, ENUMHibeTur hibeTur)
        {
            return kullanan.YetkiTipiDegeriIceriyorMu("Destek Basvuru Islem Yetkisi", ((int)hibeTur).ToString(), false);
        }

        public static bool YetkisiVarMiBirime(Kullanici kullanan, string ilKod)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUUZMAN))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUMERKEZ))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURURAPORLAMA))
                return true;

            if (string.IsNullOrEmpty(ilKod))
                return false;

            string[] ytDegerler = YetkiliOlduguIlleriGetir(kullanan);
            string[] iller = ilKod.Split(';');

            for (int i = 0; i < ytDegerler.Length; i++)
            {
                foreach (string il in iller)
                {
                    if (il.Trim() == "") continue;
                    if (ytDegerler[i].Replace(".", "") == il.Trim())
                        return true;
                }
            }

            return false;
        }

        public static bool YetkiVarMiYazmayaKisa(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUUZMAN))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUMERKEZ))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURURAPORLAMA))
                return true;

            //DebugYaz(kullanan, $"Aranan tiplerden {kullanan.YetkiTipiDegeriBul("Destek Basvuru Yetkilisi")}");
            if (!kullanan.YetkiTipiDegeriIceriyorMu("Destek Basvuru Yetkilisi", "2", false))
                return false;

            //DebugYaz(kullanan, "Aranan tiplerden sınıfı geçti");
            return true;
        }

        public static bool YetkiVarMiYazmaya(Kullanici kullanan, string ilKod, int etapNo, Sonuc s, string islemTip = "", ENUMHibeTur hibeTur = ENUMHibeTur.MAKINE, int kod = 0, bool online = false)
        {
            if (!YetkiVarMiYazmayaKisa(kullanan))
            {
                s.DegerAta("Yazma yetkiniz bulunmamaktadır");
                return false;
            }
            Yonetim y;
            //DebugYaz(kullanan, $"Yazma Kontrolü {hibeTur}");
            bool basvuruyaGoreKontrol = ilKod.IndexOf(".") > -1;
            bool eskiBasvuru = false;

            switch (hibeTur)
            {
                case ENUMHibeTur.MAKINE:
                    if (etapNo == -1)
                        etapNo = servis.HibeMakineAktifEtapNo();
                    y = servis.HibeMakineYonetimOku(kullanan, new Yonetim() { etapNo = etapNo });
                    break;
                case ENUMHibeTur.SULAMA:
                    if (etapNo == -1)
                        etapNo = servis.HibeSulamaAktifEtapNo();
                    y = servis.HibeSulamaYonetimOku(kullanan, new Yonetim() { etapNo = etapNo });
                    break;
                case ENUMHibeTur.EKONOMIKYATIRIM:
                    if (etapNo == -1)
                        etapNo = servis.HibeYatirimAktifEtapNo();
                    else
                    {
                        if (etapNo <= 11 && etapNo != 0 && !online)
                            eskiBasvuru = true;
                    }
                    if (!eskiBasvuru)
                        y = servis.HibeYatirimYonetimOku(kullanan, new Yonetim() { etapNo = etapNo });
                    else
                        y = null;
                    break;
                default:
                    if (etapNo == -1)
                        etapNo = servis.HibeMakineAktifEtapNo();
                    y = servis.HibeMakineYonetimOku(kullanan, new Yonetim() { etapNo = etapNo });
                    break;
            }

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            //DebugYaz(kullanan, $"DESTEKBASVURUYONETICI rolü var mıdır");

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))
            {
                bool deger = YetkisiVarMiDestekTurune(kullanan, hibeTur);
                if (!deger)
                    s.DegerAta("Destek türü için yetkiniz bulunmamaktadır!");
                return deger;
            }
            //DebugYaz(kullanan, $"DESTEKBASVURUUZMAN {islemTip}");
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUUZMAN) && (islemTip == "OdemeIslem" || islemTip == "GrupIslem"))
            {
                bool deger = YetkisiVarMiDestekTurune(kullanan, hibeTur);
                if (!deger)
                    s.DegerAta("Destek türü için yetkiniz bulunmamaktadır!");
                return deger;
            }
            //DebugYaz(kullanan, $"DESTEKBASVURUTASRA {islemTip}");
            //////  İl Müdürlüklerine giriş kapatıldıktan sonra Ödeme İşlem ve Grup Oluşturma yetkisi vermek için eklenmiştir. ///
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUTASRA) && (islemTip == "OdemeIslem" || islemTip == "GrupIslem"))
            {
                bool ilyetki = false;
                if (ilKod != "")
                {
                    ilyetki = YetkisiVarMiBirime(kullanan, ilKod);
                    if (!ilyetki)
                    {
                        s.DegerAta("Verilen ille ilgili işlem yapma yetkiniz bulunmamaktadır!");
                        return false;
                    }
                }
                if (ilyetki)
                {
                    bool deger = YetkisiVarMiDestekTurune(kullanan, hibeTur);
                    if (!deger)
                        s.DegerAta("Destek türü için yetkiniz bulunmamaktadır!");
                    return deger;
                }
            }

            //DebugYaz(kullanan, $"İl Müdürlüklerine giriş kapatıldıktan sonra Ödeme İşlem ve Grup Oluşturma yetkisi vermek!");
            /////   İl Müdürlüklerine giriş kapatıldıktan sonra Ödeme İşlem ve Grup Oluşturma yetkisi vermek  SONU  ////
            if (!eskiBasvuru)
            {
                if (y == null)
                {
                    s.DegerAta("Yönetim bilgisi okunamadı!");
                    return false;
                }
                //if (y.etapNo != etapNo) return false;

                //Online başvuru kayıt düzeltmesi yapılıyor ise
                if (online)
                {
                    if (kod > 0)
                    {
                        if (hibeTur == ENUMHibeTur.EKONOMIKYATIRIM)
                        {
                            bool asamaKontrol = servis.HibeYatirimMerkezKabulAsamasiGectimi(etapNo);
                            if (asamaKontrol)
                            {
                                int durum = servis.HibeYatirimBasvuruDurumBilgisi(kod.ToString());
                                if (durum == (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI)
                                    return true;
                            }
                        }

                        if (hibeTur == ENUMHibeTur.SULAMA)
                        {
                            return true;
                        }
                    }
                }

                if (y.giris == 0)
                {
                    if (y.girisler == null || y.girisler.Length == 0)
                    {
                        s.DegerAta("Yönetim bilgisi boş geldi!");
                        return false;
                    }
                }
            }

            //DebugYaz(kullanan, $"girisYapabilir = false!");
            bool girisYapabilir = false;
            if (!eskiBasvuru)
            {
                TNSDateTime bugun = new TNSDateTime(DateTime.Now.ToShortDateString());

                if (y.giris == 1 && !basvuruyaGoreKontrol)//başvuruya göre kontrol edilmiyorsa 
                    girisYapabilir = true;
                else if (y.girisler != null)
                {
                    foreach (YonetimGiris item in y.girisler)
                    {
                        if (item.ilKod == ilKod)
                        {
                            if (bugun.Oku() <= item.tarih.Oku())
                            {
                                girisYapabilir = true;
                                break;
                            }
                        }
                    }
                }
            }
            else
                girisYapabilir = true;

            //DebugYaz(kullanan, $"girisYapabilir: {yetki}!");
            if (girisYapabilir)
            {
                //DebugYaz(kullanan, $"Destek Basvuru Yetkilisi: {yetki}!");
                if (!kullanan.YetkiTipiDegeriIceriyorMu("Destek Basvuru Yetkilisi", "2", false))
                {
                    s.DegerAta("Yazma yetkiniz bulunmamaktadır!");
                    return false;
                }

                //DebugYaz(kullanan, $"ilkod: {ilKod}!");
                if (ilKod != "" && !basvuruyaGoreKontrol)
                {
                    bool yetki = YetkisiVarMiBirime(kullanan, ilKod);
                    if (!yetki)
                    {
                        s.DegerAta("Verilen ille ilgili işlem yapma yetkiniz bulunmamaktadır!");
                        return false;
                    }
                }
            }

            //DebugYaz(kullanan, $"Son: {yetki}!");
            if (!YetkisiVarMiDestekTurune(kullanan, hibeTur))
            {
                s.DegerAta("Destek türü için yetkiniz bulunmamaktadır!");
                return false;
            }
            return true;
        }

        public static void DebugYaz(Kullanici kullanan, string metin, bool yeni = false)
        {
            if (kullanan.TCKIMLIKNO == "15926351518")
                OrtakFonksiyonlar.HataDosyaYaz("YetkiKontrol.txt", metin, yeni);
        }

        public static bool YetkiVarMiOkumaya(Kullanici kullanan, string ilKod, ENUMHibeTur hibeTur = ENUMHibeTur.MAKINE)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))
                return YetkisiVarMiDestekTurune(kullanan, hibeTur);

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUUZMAN))
                return YetkisiVarMiDestekTurune(kullanan, hibeTur);

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUMERKEZ))
                return YetkisiVarMiDestekTurune(kullanan, hibeTur);

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURURAPORLAMA))
                return YetkisiVarMiDestekTurune(kullanan, hibeTur);

            bool yetki = false;
            if (kullanan.YetkiTipiDegeriIceriyorMu("Destek Basvuru Yetkilisi", "1", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Destek Basvuru Yetkilisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Destek Basvuru Yetkilisi", "3", false))
                yetki = true;

            if (yetki && ilKod != "")
                yetki = YetkisiVarMiBirime(kullanan, ilKod);

            return yetki && YetkisiVarMiDestekTurune(kullanan, hibeTur);
        }

        public static string[] YetkiliOlduguIlleriGetir(Kullanici kullanan)
        {
            string yetkiTipiYetkiDegeri = "";
            yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Destek Basvuru Yetkili Oldugu Iller");

            string[] ytDegerler;
            ytDegerler = yetkiTipiYetkiDegeri.Replace(".", "").Split(';');

            return ytDegerler;
        }
    }

    /// <summary>
    /// Sunucu tarafından sunulan fonksiyonları belirten arayüz
    /// </summary>
    public interface ITRMServis
    {
        Sonuc ButceKaydet(Kullanici kullanan, ObjectArray bilgi);
        ObjectArray ButceListele(Kullanici kullanan, Butce kriter);

        /**********Makine/Ekipman****************************/
        /// <summary>
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc MalzemeGrubuKaydet(Kullanici kullanan, MalzemeGrubu bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme grubu bilgisini veritabanından okur 
        /// ve MalzemeGrubu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme grubu listesini MalzemeGrubu nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray MalzemeGrubuListele(Kullanici kullanan, MalzemeGrubu kriter);
        /// <summary>
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc MalzemeGrubuSil(Kullanici kullanan, MalzemeGrubu bilgi);
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //Sonuc MalzemeKaydet(Kullanici kullanan, Malzeme bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        //ObjectArray MalzemeListele(Kullanici kullanan, Malzeme kriter);
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //Sonuc MalzemeSil(Kullanici kullanan, Malzeme bilgi);
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc MakinaOzelligiKaydet(Kullanici kullanan, Malzeme bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray MakinaOzelligiListele(Kullanici kullanan, Malzeme kriter);
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc MakinaOzelligiSil(Kullanici kullanan, Malzeme bilgi);
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc MakinaTanimKaydet(Kullanici kullanan, Malzeme bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray MakinaTanimListele(Kullanici kullanan, Malzeme kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="malzemeGrupKod">The malzeme grup kod.</param>
        /// <param name="malzemeKod">The malzeme kod.</param>
        /// <returns>
        /// İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne
        /// </returns>
        ObjectArray MakinaTanimOzellikListele(Kullanici kullanan, int malzemeGrupKod, int malzemeKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="malzemeGrupKod">The malzeme grup kod.</param>
        /// <param name="malzemeKod">The malzeme kod.</param>
        /// <returns>
        /// İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne
        /// </returns>
        ObjectArray MakinaTanimVeOzellikleriniListele(Kullanici kullanan, Malzeme m);
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc MakinaTanimSil(Kullanici kullanan, Malzeme bilgi);
        bool AnaMalzemeKodVarMi(Kullanici kullanan, int anaMalzemeKod);
        Sonuc MalzemeAnaKodGuncelle(Kullanici kullanan, int malzemeKod, int anaMalzemeKod, int kendiAnaKodu);
        /// <summary>
        /// Arguman olarak verilen, hibe makine bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeMakineKaydet(Kullanici kullanan, HibeMakine bilgi);
        /// <summary>
        /// Arguman olarak verilen, hibe makine proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeMakineKaydetProjeNo(Kullanici kullanan, HibeMakine bilgi);
        /// <summary>
        /// Arguman olarak verilen, hibe makine proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeMakinaKaydetEksikHibeMiktar(Kullanici kullanan, HibeMakineDetay bilgi);
        /// <summary>
        /// Arguman olarak verilen, hibe makine proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeSulamaKaydetEksikHibeMiktar(Kullanici kullanan, HibeSulamaDetay bilgi);
        /// <summary>
        /// Arguman olarak verilen, hibe red bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        Sonuc HibeMakineKaydetReddedildi(Kullanici kullanan, HibeMakine bilgi);
        Sonuc HibeMakineKaydetBasvuruYapildi(Kullanici kullanan, int hibeKod);
        Sonuc HibeMakineYatirimciDegistir(Kullanici kullanan, HibeMakine bilgi);

        Sonuc HibeYatirimYatirimciDegistir(Kullanici kullanan, HibeYatirim bilgi);
        Sonuc HibeYatirimBasvuruNoDegistir(Kullanici kullanan, HibeYatirim bilgi);

        Sonuc HibeSulamaYatirimciDegistir(Kullanici kullanan, HibeSulama bilgi);

        /// <summary>
        /// Arguman olarak verilen, hibe makine sozlesme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeMakineKaydetSozlesme(Kullanici kullanan, HibeMakine bilgi);
        /// <summary>
        /// Arguman olarak verilen, hibe makine fatura bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeMakineKaydetFatura(Kullanici kullanan, HibeMakine bilgi);
        /// <summary>
        /// Arguman olarak verilen, hibe makine yerinde tespit bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeMakineKaydetYerindeTespit(Kullanici kullanan, HibeMakine bilgi);
        Sonuc HibeMakineBasvuruyuIslemeAl(Kullanici kullanan, int kod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe makine bilgisini veritabanından okur 
        /// ve HibeMakine nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini HibeMakine nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray HibeMakineListele(Kullanici kullanan, HibeMakine kriter);
        ObjectArray HibeMakineSorgula(Kullanici kullanan, HibeMakine kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc);
        int HibeMakineBasvuruSayiDondur(Kullanici kullanan, HibeMakine kriter);
        ObjectArray HibeMakineTarihceListele(Kullanici kullanan, HibeMakine kriter);
        ObjectArray HibeMakineRefFiyatTarihceListele(Kullanici kullanan, HibeMakineRefFiyat kriter);
        Sonuc HibeMakineRefSonrakiYilaAktar(Kullanici kullanan, HibeMakineRefFiyat kriter, double ufeOran);

        //ObjectArray HibeYatirimRefFiyatTarihceListele(Kullanici kullanan, HibeYatirimRefFiyat kriter);
        Sonuc HibeYatirimRefSonrakiYilaAktar(Kullanici kullanan, HibeYatirimRefFiyat kriter, double ufeOran);

        ObjectArray HibeMakineOdemeListele(Kullanici kullanan, HibeMakineSulamaOdeme kriter);
        Sonuc HibeMakineOdemeSil(Kullanici kullanan, HibeMakineSulamaOdeme kriter);
        Sonuc HibeMakineOdemeKaydet(Kullanici kullanan, HibeMakineSulamaOdeme bilgi);
        Sonuc HibeMakineOdemeKaydetOdemeEmriNo(Kullanici kullanan, HibeMakineSulamaOdeme bilgi);
        Sonuc HibeMakineOdemeKaydetOdendi(Kullanici kullanan, int kod);
        Sonuc HibeMakineOdemeKaydetBankaGonder(Kullanici kullanan, int kod);
        double HibeMakineOdemeMiktarGetir(Kullanici kullanan, int tur);

        int[] OncekiEtapdaKimlikNoVarmi(string kimlikNo, int tip);
        int[] AktifEtapdaKimlikNoVarmi(string kimlikNo, int tip);
        Sonuc HibeMakineMalzemeDegistir(Kullanici kullanan, HibeMakineDetay bilgi);
        ObjectArray HibeMakineIlBazindaIcmal(Kullanici kullanan, HibeMakine kriter, bool ilListele);
        Sonuc HibeMakineYonetimKaydet(Kullanici kullanan, Yonetim form);
        Sonuc HibeMakineYonetimSil(Kullanici kullanan, Yonetim form);
        Yonetim[] HibeMakineYonetimListele(Kullanici kullanan, Yonetim form);
        Yonetim HibeMakineYonetimOku(Kullanici kullanan, Yonetim form);
        int HibeMakineAktifEtapNo();
        double HibeMakineAktifUstLimit(int hukukiStatu);
        string HibeMakineOnlineKapandiMi();
        Sonuc HibeMakineRefFiyatKaydet(Kullanici kullanan, ObjectArray liste);
        ObjectArray HibeMakineRefFiyatListele(Kullanici kullanan, HibeMakineRefFiyat kriter);
        Sonuc HibeYatirimRefFiyatKaydet(Kullanici kullanan, ObjectArray liste);
        ObjectArray HibeYatirimRefFiyatListele(Kullanici kullanan, HibeYatirimRefFiyat kriter);

        void HibeMakineHibeTutarHesapla(int hukukiStatu, decimal referansTutar, decimal proformaTutar, decimal malzemeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, decimal hibeOran, int adet);

        Sonuc AnketSorulariniAktar(string onEkTur, string eskiEtap, string yeniEtap);

        //Grup
        Sonuc HibeMakineGrupKaydet(Kullanici kullanan, HibeMakineSulamaGrup bilgi);
        Sonuc HibeMakineGrupCikart(Kullanici kullanan, HibeMakineSulamaGrup bilgi);
        Sonuc HibeMakineGrupOdemeyeGonder(Kullanici kullanan, HibeMakineSulamaGrup bilgi);
        Sonuc HibeMakineGrupOdemeBaslat(Kullanici kullanan, HibeMakineSulamaGrup[] bilgi);
        Sonuc HibeMakineGrupOdendi(Kullanici kullanan, HibeMakineSulamaGrup bilgi);
        Sonuc HibeMakineGrupBankaGonder(Kullanici kullanan, HibeMakineSulamaGrup bilgi);
        Sonuc HibeMakineGrupGeriAl(Kullanici kullanan, HibeMakineSulamaGrup bilgi);
        //Grup

        Sonuc HibeMakineSulamaGrupKaydet(Kullanici kullanan, HibeMakineSulamaGrup g);
        Sonuc HibeMakineSulamaGrupKontrol(Kullanici kullanan, HibeMakineSulamaGrup g);
        HibeMakineSulamaGrup[] HibeMakineSulamaGrupListele(Kullanici kullanan, HibeMakineSulamaGrup g, params int[] ekDurumlar);
        HibeMakineSulamaGrup[] HibeMakineSulamaListeleGrupBazliToplam(Kullanici kullanan, int etapNo, int durum, int tipi, int tur = -1, int odemeKod = -1, bool kontrolEdilen = false, TNSDateTime baslamaTarih = null, TNSDateTime bitisTarihi = null);
        /**********Makine/Ekipman****************************/

        /**********Ekonomik Yatırım****************************/
        /// <summary>
        /// Arguman olarak verilen, hibe Yatirim bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeYatirimKaydet(Kullanici kullanan, HibeYatirim bilgi);
        Sonuc HibeTutarlariKaydet(Kullanici kullanan, HibeYatirim bilgi);
        Sonuc HibeYatirimKaydetSozlesmeSonrasi(Kullanici kullanan, HibeYatirim bilgi);
        Sonuc HibeYatirimKaydetKabul(Kullanici kullanan, int etapNo, string ip, int basvuruTur);
        Sonuc HibeYatirimKaydetGeriAl(Kullanici kullanan, int etapNo, string ip, int geriAlmaTuru, int basvuruTur, string altYapiGrup);
        Sonuc HibeYatirimKaydetKabulCalismasi(Kullanici kullanan, int etapNo, string ilKod, HibeYatirim[] bilgiler, int kabulCalismasi);
        /// <summary>
        /// Arguman olarak verilen, hibe red bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        Sonuc HibeYatirimKaydetReddedildi(Kullanici kullanan, HibeYatirim bilgi);
        Sonuc HibeYatirimKaydetOnline(Kullanici kullanan, HibeYatirim bilgi);
        Sonuc HibeYatirimSilSozlesme(Kullanici kullanan, HibeYatirimSozlesme sozlesme);
        Sonuc HibeYatirimKaydetSozlesme(Kullanici kullanan, HibeYatirimSozlesme sozlesme, HibeYatirimMaliyet maliyet, string ip);
        Sonuc HibeYatirimMaliyetGuncelle(Kullanici kullanan, HibeYatirimMaliyet maliyet);
        HibeYatirim HibeYatirimSozlesmeListele(Kullanici kullanan, int hibeKod);
        HibeYatirim HibeYatirimListeleYatirimVeSozlesme(Kullanici kullanan, int hibeKod);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe Yatirim bilgisini veritabanından okur 
        /// ve HibeYatirim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini HibeYatirim nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray HibeYatirimListele(Kullanici kullanan, HibeYatirim kriter);
        ObjectArray HibeYatirimListeleMerkezdeKabulHali(Kullanici kullanan, int kod);
        ObjectArray HibeYatirimSorgula(Kullanici kullanan, HibeYatirim kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc);
        ObjectArray HibeYatirimMalzemeSorgula(Kullanici kullanan, HibeYatirim kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc);
        int HibeYatirimBasvuruSayiDondur(Kullanici kullanan, HibeYatirim kriter);
        bool HibeYatirimKabulSonrasiVarMi(Kullanici kullanan, HibeYatirim kriter);
        ObjectArray HibeYatirimListeleOncelikliForKabul(Kullanici kullanan, HibeYatirim kriter);
        ObjectArray HibeYatirimEksikBelgeListele(Kullanici kullanan, HibeYatirim kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe Yatirim tarihçe bilgisini veritabanından okur 
        /// ve HibeYatirimTarihce nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, listesiyi HibeYatirimTarihce nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray HibeYatirimTarihceListele(Kullanici kullanan, HibeYatirim kriter);
        ObjectArray HibeYatirimIlBazindaIcmal(Kullanici kullanan, HibeYatirim kriter, bool ilListele);
        Sonuc HibeYatirimYonetimKaydet(Kullanici kullanan, Yonetim form);
        Sonuc HibeYatirimYonetimSil(Kullanici kullanan, Yonetim form);
        Yonetim[] HibeYatirimYonetimListele(Kullanici kullanan, Yonetim form);
        Yonetim HibeYatirimYonetimOku(Kullanici kullanan, Yonetim form);
        int HibeYatirimAktifEtapNo();
        string HibeYatirimTebligNoBul(int etapNo, string konuKod);
        double HibeYatirimAktifUstLimit(int hukukiStatu);
        double HibeYatirimAktifUstLimitKonu(int etapNo, int projeNitelik, string konuKod, int hukukiStatu);
        //void HibeYatirimHibeTutarHesapla(int hukukiStatu, decimal referansTutar, decimal proformaTutar, decimal malzemeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, decimal hibeOran);
        void HibeYatirimHibeTutarHesapla(int projeNitelik, string konuKod, decimal referansTutar, decimal proformaTutar, decimal malzemeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, int hukukiStatu, decimal hibeOran, int etapNo);
        Sonuc YatirimNitelikKaydet(Kullanici kullanan, YatirimNitelik bilgi);
        ObjectArray YatirimNitelikListele(Kullanici kullanan, YatirimNitelik kriter);
        Sonuc YatirimNitelikSil(Kullanici kullanan, YatirimNitelik bilgi);
        Sonuc YatirimMalzemeTurKaydet(Kullanici kullanan, YatirimMalzemeTur bilgi);
        ObjectArray YatirimMalzemeTurListele(Kullanici kullanan, YatirimMalzemeTur kriter);
        Sonuc YatirimMalzemeTurSil(Kullanici kullanan, YatirimMalzemeTur bilgi);
        Sonuc YatirimMalzemeTanimKaydet(Kullanici kullanan, YatirimMalzemeTanim bilgi);
        Sonuc YatirimKonuNitelikKisitKaydet(Kullanici kullanan, YatirimKonuNitelikKisit bilgi);
        Sonuc YatirimKonuNitelikKisitSil(Kullanici kullanan, YatirimKonuNitelikKisit bilgi);
        ObjectArray YatirimKonuNitelikKisitListele(Kullanici kullanan, YatirimKonuNitelikKisit kriter);
        ObjectArray YatirimMalzemeTanimListele(Kullanici kullanan, YatirimMalzemeTanim kriter);
        Sonuc YatirimMalzemeTanimSil(Kullanici kullanan, YatirimMalzemeTanim bilgi);

        Sonuc YatirimKonuKaydet(Kullanici kullanan, YatirimKonu bilgi);
        ObjectArray YatirimKonuListele(Kullanici kullanan, YatirimKonu kriter);
        Sonuc YatirimKonuSil(Kullanici kullanan, YatirimKonu bilgi);
        Sonuc YatirimKonuIlKaydet(Kullanici kullanan, ObjectArray liste);
        ObjectArray YatirimKonuIlListele(Kullanici kullanan, YatirimKonuIl kriter);
        Sonuc YatirimKonuIlAktar(Kullanici kullanan, int eskiEtap, int yeniEtap);

        Sonuc YatirimAltKonuIlKaydet(Kullanici kullanan, ObjectArray liste);
        ObjectArray YatirimAltKonuIlListele(Kullanici kullanan, YatirimAltKonuIl kriter);
        Sonuc YatirimAltKonuIlAktar(Kullanici kullanan, int eskiEtap, int yeniEtap);

        Sonuc YatirimKonuUstSinirKaydet(Kullanici kullanan, ObjectArray liste);
        ObjectArray YatirimKonuUstSinirListele(Kullanici kullanan, YatirimKonuUstSinir kriter);
        Sonuc YatirimKonuUstSinirAktar(Kullanici kullanan, int eskiEtap, int yeniEtap);


        Sonuc YatirimAltKonuDetayKaydet(Kullanici kullanan, YatirimAltKonuDetay bilgi);
        ObjectArray YatirimAltKonuDetayListele(Kullanici kullanan, YatirimAltKonuDetay kriter);
        Sonuc YatirimAltKonuDetaySil(Kullanici kullanan, YatirimAltKonuDetay bilgi);

        Sonuc YatirimOncelikKaydet(Kullanici kullanan, ObjectArray bilgi);
        ObjectArray YatirimOncelikListele(Kullanici kullanan, HibeYatirimOncelik kriter);
        Sonuc HibeYatirimUygulamaSozlesmeKaydet(Kullanici kullanan, HibeYatirimUygulamaSozlesme us);
        Sonuc HibeYatirimEskiEtapUygSozlesmeKaydet(Kullanici kullanan, HibeYatirimUygulamaSozlesme[] uygSozlesme);
        Sonuc HibeYatirimUygulamaSozlesmeSil(Kullanici kullanan, HibeYatirimUygulamaSozlesme us);
        HibeYatirimUygulamaSozlesme[] HibeYatirimUygulamaSozlesmeListele(Kullanici kullanan, HibeYatirimUygulamaSozlesme us, bool iptalVeHataliHaric);
        Sonuc HibeYatirimEskiEtapHakedisleriKaydet(Kullanici kullanan, HibeYatirimHakedis[] hakedisler);
        Sonuc HibeYatirimHakedisKaydet(Kullanici kullanan, HibeYatirimHakedis hakedis);
        Sonuc HibeYatirimHakedisSil(Kullanici kullanan, HibeYatirimHakedis hakedis);
        Sonuc HibeYatirimHakedisOdemeGrupNoGuncelle(Kullanici kullanan, HibeYatirimHakedis hakedis);
        Sonuc HibeYatirimHakedisOdemeKontrolGuncelle(Kullanici kullanan, HibeYatirimHakedis hakedis);
        HibeYatirimHakedis[] HibeYatirimHakedisListele(Kullanici kullanan, HibeYatirimHakedis hakedis);
        HibeYatirim[] HibeYatirimHakedisListeleOdemeGonder(Kullanici kullanan, HibeYatirim kriter);
        Sonuc HibeYatirimHakedisOdemeyeGonder(Kullanici kullanan, string odemeGrupNo);
        Sonuc HibeYatirimHakedisOdemeIslemYap(Kullanici kullanan, HibeYatirimHakedis[] hakedisler, ENUMHibeYatirimHakedisDurum yeniHakedisDurum);
        Sonuc HibeYatirimKaydetDegerlendirme(Kullanici kullanan, HibeYatirim[] bilgiler);
        HibeYatirimUygulamaSozlesme[] HibeYatirimListeleUygulamaSozlesmesiVeHakedisler(Kullanici kullanan, int hibeKod);
        ObjectArray YatirimKapasiteBirimListele();
        Sonuc HibeYatirimBasvuruyuIslemeAl(Kullanici kullanan, int kod, string ilKod, string basvuruNo, string ip);
        YatirimIsletmePlani HibeYatirimIsletmePlaniGetir(Kullanici kullanan, int hibeKod);
        Sonuc HibeYatirimIsletmePlaniKaydet(Kullanici kullanan, YatirimIsletmePlani bilgi);
        string HibeYatirimOnlineKapandiMi(Kullanici kullanan, string basvuruNo);
        string HibeYatirimDegerlendirmeKapandiMi(string basvuruNo, string projeKonu, string ilKodu);
        Sonuc OdenenHakedislerinUygulamaSozlesmesiDurumGuncellemesi(Kullanici kullanan, int odemeKod);
        Sonuc HibeMakineDegerlendirmeKaydet(Kullanici kullanan, int hibeKod, string anahtar, double artisPuan);
        HibeMakineServis[] HibeMakineListForTKDK(Kullanici kullanan, string kimlikNo);

        /**********Ekonomik Yatırım****************************/

        /******Sulama**********************/
        /// <summary>
        /// Arguman olarak verilen, Sulama grubu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SulamaGrubuKaydet(Kullanici kullanan, SulamaGrubu bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Sulama grubu bilgisini veritabanından okur 
        /// ve SulamaGrubu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Sulama grubu listesini SulamaGrubu nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray SulamaGrubuListele(Kullanici kullanan, SulamaGrubu kriter);
        /// <summary>
        /// Arguman olarak verilen, Sulama grubu bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SulamaGrubuSil(Kullanici kullanan, SulamaGrubu bilgi);
        Sonuc SulamaMalzemeKaydet(Kullanici kullanan, SulamaMalzeme m);
        Sonuc SulamaMalzemeSil(Kullanici kullanan, SulamaMalzeme m);
        SulamaMalzeme[] SulamaMalzemeListele(Kullanici kullanan, SulamaMalzeme m, bool detayVer);
        Sonuc HibeSulamaMalzemeKaydet(Kullanici kullanan, HibeSulamaMalzemeFiyat[] mfler, HibeSulama hs);
        Sonuc HibeSulamaMalzemeSil(Kullanici kullanan, int hibeKod, ENUMHibeSulamaMalzemeTip tip, string hibeIlKod, int sulamaGrupKod, string parselNo);
        HibeSulamaMalzemeFiyat[] HibeSulamaMalzemeListele(Kullanici kullanan, int hibeKod, ENUMHibeSulamaMalzemeTip tip, string hibeIlKod, int sulamaGrupKod, string parselNo);
        HibeSulamaMalzemeFiyat[] ListeleSulamaGrupTutar(Kullanici kullanan, int hibeKod, int tip);

        Sonuc KontrolSulamaOncedenReddilenDestek(Kullanici kullanan, HibeSulama bilgi);

        /// <summary>
        /// Arguman olarak verilen, hibe Sulama bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeSulamaKaydet(Kullanici kullanan, HibeSulama bilgi);
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeSulamaKaydetProjeNo(Kullanici kullanan, HibeSulama bilgi);
        /// <summary>
        /// Arguman olarak verilen, hibe red bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        Sonuc HibeSulamaKaydetReddedildi(Kullanici kullanan, HibeSulama bilgi);
        Sonuc HibeSulamaKaydetAsilYedek(Kullanici kullanan, int[] hibeKodlar, string asilYedek);
        Sonuc HibeSulamaKaydetKabul(Kullanici kullanan, int[] hibeKodlar, string kabul);
        Sonuc HibeSulamaKaydetBasvuruYapildi(Kullanici kullanan, int hibeKod);
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama sozlesme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeSulamaKaydetSozlesme(Kullanici kullanan, HibeSulama bilgi);
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama fatura bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeSulamaKaydetFatura(Kullanici kullanan, HibeSulama bilgi);
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama yerinde tespit bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HibeSulamaKaydetYerindeTespit(Kullanici kullanan, HibeSulama bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe Sulama bilgisini veritabanından okur 
        /// ve HibeSulama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini HibeSulama nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray HibeSulamaListele(Kullanici kullanan, HibeSulama kriter);
        ObjectArray HibeSulamaSorgula(Kullanici kullanan, HibeSulama kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc);
        int HibeSulamaBasvuruSayiDondur(Kullanici kullanan, HibeSulama kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe Sulama tarihçe bilgisini veritabanından okur 
        /// ve HibeSulamaTarihce nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, listesiyi HibeSulamaTarihce nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray HibeSulamaTarihceListele(Kullanici kullanan, HibeSulama kriter);
        ObjectArray HibeSulamaIlBazindaIcmal(Kullanici kullanan, HibeSulama kriter, bool ilListele);
        Sonuc HibeSulamaYonetimKaydet(Kullanici kullanan, Yonetim form);
        Sonuc HibeSulamaYonetimSil(Kullanici kullanan, Yonetim form);
        Yonetim[] HibeSulamaYonetimListele(Kullanici kullanan, Yonetim form);
        Yonetim HibeSulamaYonetimOku(Kullanici kullanan, Yonetim form);
        int HibeSulamaAktifEtapNo();
        double HibeSulamaAktifUstLimit(int hukukiStatu);
        string HibeSulamaOnlineKapandiMi();
        string HibeSulamaSozlesmeKapandiMi();
        void HibeSulamaHibeTutarHesapla(int hukukiStatu, ref decimal hibeEsasTutarProje, ref decimal hibeEsasTutarUygulama, decimal proformaTutar, decimal malzemeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, decimal hibeOran, decimal basvuruAyniKatki);

        Sonuc HibeSulamaDegerlendirmeKaydet(Kullanici kullanan, int hibeKod, string anahtar, double artisPuan);
        string HibeSulamaBirimRefFarklimi(Kullanici kullanan, int hibeKod);

        ObjectArray HibeSulamaBankaKrediListele(Kullanici kullanan, HibeSulamaBankaKredi kriter);
        Sonuc HibeSulamaBankaKrediKaydet(Kullanici kullanan, HibeSulamaBankaKredi form);
        Sonuc HibeSulamaBankaKrediSil(Kullanici kullanan, HibeSulamaBankaKredi form);

        /******Sulama**********************/

        string PlakaGetir(string ilKod);
        /// <summary>
        /// Arguman olarak verilen, BankaSube bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc BankaSubeKaydet(Kullanici kullanan, BankaSube bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, BankaSube bilgisini veritabanından okur 
        /// ve BankaSube nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, BankaSube listesini BankaSube nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray BankaSubeListele(Kullanici kullanan, BankaSube kriter);
        /// <summary>
        /// Arguman olarak verilen, BankaSube bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc BankaSubeSil(Kullanici kullanan, BankaSube bilgi);
        /// <summary>
        /// Arguman olarak verilen, Koy bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KoyKaydet(Kullanici kullanan, Koy bilgi);
        Sonuc KoyleriKapat(Kullanici kullanan, Koy bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Koy bilgisini veritabanından okur 
        /// ve Koy nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Koy listesini Koy nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray KoyListele(Kullanici kullanan, Koy kriter);
        /// <summary>
        /// Arguman olarak verilen, Koy bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KoySil(Kullanici kullanan, Koy bilgi);

        ObjectArray CografyaListele(string kod, bool sadeceDetay);

        HibeMakine[] HibeMakineKimlikBilgisiEksikBasvurular(Kullanici kullanan);
        Sonuc HibeMakineKimlikBilgisiEksikGuncelle(Kullanici kullanan, HibeMakine hm);

        HibeSulama[] HibeSulamaKimlikBilgisiEksikBasvurular(Kullanici kullanan);
        Sonuc HibeSulamaKimlikBilgisiEksikGuncelle(Kullanici kullanan, HibeSulama hs);

        ObjectArray MalzemeFiyatAnalizi(Kullanici kullanan, MalzemeFiyatAnalizi mfa);

        Sonuc HibeYatirimAnket1Kaydet(Kullanici kullanan, int hibeKod, string anahtar, string ip);
        Sonuc HibeYatirimAnket2Kaydet(Kullanici kullanan, int hibeKod, string anahtar, string ip);
        Sonuc HibeYatirimAnket3Kaydet(Kullanici kullanan, int hibeKod, string anahtar, string ip);
        Sonuc HibeYatirimAnket4Kaydet(Kullanici kullanan, int hibeKod, string anahtar, string ip);

        YatirimIzlemeCetveli[] YatirimIzlemeCetveliListesi(Kullanici kullanan, YatirimIzlemeCetveli[] izlemeCetveli, int etapNo, int ilTur, int konuTur);
        ArrayList IlListesi(Kullanici kullanan, int ilTur);
        ObjectArray MakinaEkipmanDesteklemeleri(Kullanici kullanan, int etapNo, int odemeIlTur);
        ObjectArray KKYDPOdeme(Kullanici kullanan, int etapNo, int odemeIlTur);
        ObjectArray EYHibeTutariGetir(Kullanici kullanan, int etapNo, int odemeIlTur);

        Sonuc EklenenDosyayiKaydet(Kullanici kullanan, EklenenDosya bilgi);
        Sonuc EklenenDosyayiSil(Kullanici kullanan, string kod);
        ObjectArray EklenenDosyalariListele(Kullanici kullanan, EklenenDosya kriter);

        Sonuc CezaKaydet(Kullanici kullanan, Ceza bilgi);
        Sonuc CezaSil(Kullanici kullanan, Ceza bilgi);
        Sonuc CezaliKisiSil(Kullanici kullanan, Ceza bilgi);
        ObjectArray CezalariListele(Kullanici kullanan, Ceza kriter);
        ObjectArray CezaliKisileriListele(Kullanici kullanan, int cezaKod);
        ObjectArray ListeleCezaVeCezaliKisiler(Kullanici kullanan, int cezaKod);

        Sonuc BasvuruBelgeTanimiKaydet(Kullanici kullanan, BasvuruBelgeTanim bilgi);
        Sonuc BasvuruBelgeTanimiSil(Kullanici kullanan, BasvuruBelgeTanim bilgi);
        ObjectArray BasvuruBelgeTanimlariniListele(Kullanici kullanan, BasvuruBelgeTanim kriter);
        ObjectArray BasvuruBelgeAdlariniListele(Kullanici kullanan, int belgeKod);
        ObjectArray BasvuruBelgeVeAdlariniListele(Kullanici kullanan, BasvuruBelgeTanim kriter);
        Sonuc BasvuruBelgeTanimiAktar(Kullanici kullanan, int eskiEtap, int yeniEtap);

        Sonuc BasvurudaYuklenenBelgeleriKaydet(Kullanici kullanan, BasvurudaYuklenenBelgeler bilgi);
        Sonuc BasvurudaYuklenenBelgeleriSil(Kullanici kullanan, BasvurudaYuklenenBelgeler bilgi);
        ObjectArray BasvurudaYuklenenBelgeleriListele(Kullanici kullanan, BasvurudaYuklenenBelgeler kriter, bool eksikBelgeler = false);

        bool HibeTuruYilMalzemeGrubunaGoreBelgeTanimKaydiVarmi(Kullanici kullanan, BasvuruBelgeTanim bilgi);

        Sonuc SorulanSoruyuKaydet(Kullanici kullanan, SorulanSorular bilgi);
        ObjectArray SorulanSorulariListele(Kullanici kullanan, SorulanSorular kriter);
        Sonuc SorulanSoruyuSil(Kullanici kullanan, SorulanSorular bilgi);

        Sonuc BasvuruKaydet(Kullanici kullanan, Basvuru bilgi);
        Sonuc BasvuruSil(Kullanici kullanan, Basvuru bilgi);
        Basvuru[] BasvuruListele(Kullanici kullanan, Basvuru kriter);
        Sonuc BasvuruSablonKaydet(Kullanici kullanan, BasvuruSablon bilgi);
        Sonuc BasvuruSablonSil(Kullanici kullanan, BasvuruSablon bilgi);
        BasvuruSablon[] BasvuruSablonListele(Kullanici kullanan, BasvuruSablon kriter);
        Sonuc BasvuruSablonAktar(Kullanici kullanan, int turKod, int eskiEtap, int yeniEtap);

        Sonuc OnlineBasvuruKaydet(OnlineBasvuruKaydi bilgi);
        Sonuc OnlineBasvuruAktivasyon(string kod, int aktif);
        Sonuc OnlineBasvuruSil(OnlineBasvuruKaydi bilgi);
        Sonuc OnlineBasvuruGuncelleParola(Kullanici kullanan, OnlineBasvuruKaydi bilgi);
        Sonuc OnlineBasvuruParola(OnlineBasvuruKaydi bilgi);
        string OnlineBasvuruAktivasyon(OnlineBasvuruKaydi bilgi);
        OnlineBasvuruKaydi[] OnlineBasvuruListele(OnlineBasvuruKaydi kriter);
        ObjectArray KirsalKalkinmaYatirimTesisMakineSulamaOdenek(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2);
        ArrayList MakineSulamaOdenen(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2);

        Sonuc HibeYatirimOrtaklarKaydet(Kullanici kullanan, Ortaklar[] ortaklar);
        Sonuc HibeYatirimYonetimKuruluKaydet(Kullanici kullanan, YonetimKurulu[] yonetimKurulu);
        Sonuc HibeYatirimFonKaynakKaydet(Kullanici kullanan, YonetimFonKaynak[] YonetimFonKaynak);
        ObjectArray HibeYatirimOrtaklarListele(Kullanici kullanan, int hibeKod);
        ObjectArray HibeYatirimYonetimKuruluListele(Kullanici kullanan, int hibeKod);
        ObjectArray HibeYatirimFonKaynakListele(Kullanici kullanan, int hibeKod, int fonTip);
        Sonuc HibeYatirimGiderleriKaydet(Kullanici kullanan, YatirimGiderlerTablosu[] giderTablosu);
        ObjectArray HibeYatirimGiderleriListele(Kullanici kullanan, YatirimGiderlerTablosu giderTablosu);
        Sonuc HibeYatirimSureEylemPlaniKaydet(Kullanici kullanan, YatirimSureEylemPlani[] plan);
        ObjectArray HibeYatirimSureEylemPlaniListele(Kullanici kullanan, YatirimSureEylemPlani plan);
        Sonuc HibeYatirimIsTakvimiKaydet(Kullanici kullanan, YatirimIsTakvimi[] plan);
        ObjectArray HibeYatirimIsTakvimiListele(Kullanici kullanan, YatirimIsTakvimi plan);
        Sonuc HibeYatirimProjeButcesiKaydet(Kullanici kullanan, YatirimProjeButcesi[] butce);
        ObjectArray HibeYatirimProjeButcesiListele(Kullanici kullanan, YatirimProjeButcesi butce);
        Sonuc HibeYatirimFinansmanKaynagiKaydet(Kullanici kullanan, YatirimFinansmanKaynagi[] kaynak);
        ObjectArray HibeYatirimFinansmanKaynagiListele(Kullanici kullanan, YatirimFinansmanKaynagi kaynak);
        int HibeYatirimBasvuruDurumBilgisi(string basvuruNo);
        bool HibeYatirimMerkezKabulAsamasiGectimi(int etapNo);

        Sonuc HibeYatirimSozlesmeDurumunuGuncelle(Kullanici kullanan, HibeYatirimSozlesme bilgi);
        Sonuc HibeYatirimSozlesmeNihaiRaporTarihGuncelle(Kullanici kullanan, HibeYatirimSozlesme bilgi);
        Sonuc HibeYatirimSozlesmeYapiKimlikNoGuncelle(Kullanici kullanan, HibeYatirimSozlesme bilgi);

        ObjectArray ProjeNiteligineGoreHibeTutarDagilimi(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2);
        ArrayList HibeYatirimGenelDurum(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2);
        ArrayList HibeYatirimTBasincliSulamaGenelDurum(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2);
        ObjectArray IlDegerlendirmeKomisyonuListele(Kullanici kullanan, IlDegerlendirmeKomisyonu komisyon);
        Sonuc IlDegerlendirmeKomisyonuKaydet(Kullanici kullanan, IlDegerlendirmeKomisyonu[] komisyon);
        Sonuc IlDegerlendirmeKomisyonuSil(Kullanici kullanan, IlDegerlendirmeKomisyonu[] komisyon);

        Sonuc IlDegerlendirmeKomisyonunuKilitle(Kullanici kullanan, int kod, int durum);
        ObjectArray KKYDPIlTurBazliProjeler(Kullanici kullanan, int etapNo, int ilTur);
        ObjectArray KKYDPProjelerMandaAyri(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2);
        ObjectArray KKYDPIlTurBazliBitenProjeler(Kullanici kullanan, int etapNo, int ilTur);
        Sonuc HibeYatirimBasvuruyuGeriCek(Kullanici kullanan, HibeYatirim bilgi);
        Sonuc HibeYatirimKaydetUygunlukCalismasi(Kullanici kullanan, HibeYatirim[] bilgiler);

        Sonuc HibeYatirimKaydetBasvuruYapildi(Kullanici kullanan, HibeYatirim bilgiler);
        Sonuc HibeYatirimBasvuruyuOnlineYap(Kullanici kullanan, int kod, string ip);
        bool HibeYatirimListeleBasvuruYapildi(Kullanici kullanan, int kod);

        ObjectArray TUIKIlceListele(Kullanici kullanan, string ilAd, string ilceAd);
        double TUIKIlceNormListele(Kullanici kullanan, string ilAd, string ilceAd, int tip);
        ObjectArray TUIKIlceBilgiListele(Kullanici kullanan, int tip, string ilAd, string ilce);
        ObjectArray TUIKIlIlceGuncelle(Kullanici kullanan, string ilAd, string ilce, double sulunorm, double suluyeninorm, double kurunorm, double kuruyeninorm);
        ObjectArray TUIKOzetListele(Kullanici kullanan);
        ObjectArray TUIKIlListele(Kullanici kullanan);

        Sonuc AnketSonucSifirla(Kullanici kullanan, int hibeKod, int etapNo, string ilKod, string ip);
        ArrayList HibeYatirimDegerlendirmeSayilariniGetir(Kullanici kullanan, int etapNo, string ilKod, int yatirimOncelikTur, string altYapiGrup);
        HibeYatirimServis[] HibeYatirimListForTKDK(Kullanici kullanan, string kimlikNo);

        ObjectArray ListeleKabulCalismasiIlToplami(Kullanici kullanan, Butce kriter);
        ObjectArray ListeleSulamaKabulCalismasiIlToplami(Kullanici kullanan, HibeSulama kriter);

        HazineRapor[] HibeMakineHazineRapor(int yil, int ay);
        HazineRapor[] HibeYatirimHazineRapor(int yil, int ay, int tur, int etapNo);
        HazineRapor[] HibeSulamaHazineRapor(int yil, int ay, int tur, int etapNo);

        Sonuc HibeYatirimKaydetEksikBelge(Kullanici kullanan, int hibeKod, ObjectArray eksikBelge);
        ObjectArray HibeYatirimListeleEksikBelge(Kullanici kullanan, int hibeKod);
        Sonuc HibeYatirimGoruntuBilgiKaydet(Kullanici kullanan, Goruntu bilgi);
        ObjectArray HibeYatirimGoruntuBilgiListele(Kullanici kullanan, Goruntu kriter);

        Sonuc HibeYatirimYatirimciyiOdemedenCikart(Kullanici kullanan, TNS.TRM.HibeYatirimHakedis hakedis);

        Sonuc HibeYatirimSozlesmeFesihKaydet(Kullanici kullanan, HibeYatirimSozlesmeFesih bilgi);
        ObjectArray HibeYatirimSozlesmeFesihListele(Kullanici kullanan, HibeYatirimSozlesmeFesih bilgi);
        Sonuc HibeYatirimSozlesmeFesihSil(Kullanici kullanan, HibeYatirimSozlesmeFesih bilgi);
        ObjectArray HibeYatirimSozlesmeFesihRaporListele(Kullanici kullanan, HibeYatirimSozlesmeFesih bilgi);
        ObjectArray HibeYatirimSozlesmeHakRaporListele(Kullanici kullanan, HibeYatirim bilgi);
        ObjectArray HibeYatirimIldeReddilenlerRaporListele(Kullanici kullanan, HibeYatirim bilgi);
        ObjectArray HibeSulamaKonusunaGoreRapor(Kullanici kullanan, HibeSulama bilgi);
        ObjectArray HibeSulamaBilgiNotuRapor(Kullanici kullanan, HibeSulama bilgi);
        ObjectArray HibeSulamaKonuIstatistik(Kullanici kullanan, HibeSulama bilgi);
        ObjectArray HibeSulamaIstatistik(Kullanici kullanan, HibeSulama bilgi);

        Sonuc HibeSulamaDigerSil(Kullanici kullanan, int hibeKod, string parselNo, string hibeIlKod, int sulamaGrupKod);
        Sonuc HibeSulamaParselReddet(Kullanici kullanan, int hibeKod, string parselNo, string hibeIlKod, int sulamaGrupKod);

        Sonuc VeriToplamaKaydet(Kullanici kullanan, ObjectArray bilgi);
        ObjectArray VeriToplamaListele(Kullanici kullanan, VeriToplama bilgi);
        Sonuc VeriToplamaSil(Kullanici kullanan, VeriToplama bilgi);

        Sonuc YatirimMalzemeKaydet(Kullanici kullanan, YatirimMalzeme bilgi);
        YatirimMalzeme[] YatirimMalzemeListele(Kullanici kullanan, YatirimMalzeme kriter);
        YatirimMalzeme[] YatirimMalzemeListeleIstatistik(Kullanici kullanan, YatirimMalzeme kriter);
        Sonuc YatirimMalzemeSil(Kullanici kullanan, YatirimMalzeme bilgi);

        Sonuc YatirimMalzemeGrubuKaydet(Kullanici kullanan, YatirimMalzemeTur bilgi);
        ObjectArray YatirimMalzemeGrubuListele(Kullanici kullanan, YatirimMalzemeTur kriter);
        Sonuc YatirimMalzemeGrubuSil(Kullanici kullanan, YatirimMalzemeTur bilgi);
        Sonuc YatirimAnaMalzemeGrubuKaydet(Kullanici kullanan, YatirimMalzemeTur bilgi);
        ObjectArray YatirimAnaMalzemeGrubuListele(Kullanici kullanan, YatirimMalzemeTur kriter);
        Sonuc YatirimAnaMalzemeGrubuSil(Kullanici kullanan, YatirimMalzemeTur bilgi);

        Sonuc YatirimMakinaOzelligiKaydet(Kullanici kullanan, Malzeme bilgi);
        ObjectArray YatirimMakinaOzelligiListele(Kullanici kullanan, Malzeme kriter);
        Sonuc YatirimMakinaOzelligiSil(Kullanici kullanan, Malzeme bilgi);
        Sonuc YatirimMakinaTanimKaydet(Kullanici kullanan, Malzeme bilgi);
        ObjectArray YatirimMakinaTanimListele(Kullanici kullanan, Malzeme kriter);
        ObjectArray YatirimMakinaTanimOzellikListele(Kullanici kullanan, int malzemeGrupKod, int malzemeKod);
        ObjectArray YatirimMakinaTanimVeOzellikleriniListele(Kullanici kullanan, Malzeme m);
        Sonuc YatirimMakinaTanimSil(Kullanici kullanan, Malzeme bilgi);
    }
}
using System.IO;
using System.Linq;
using TNS.Veri;
using System;
using TNS.KYM;
using TNS.EVR.IsKurallari.Veri;
using System.Collections.Generic;
using System.Data;
using EVRServisI;
using WebServisler.Evrak.SgbnetOCRServisi;
using System.Diagnostics;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// OCR bilgileriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVROCR
    {
        /// <summary>
        /// Paramete olarak verilen Altbilgi nesne koleksiyonunu, kullanıcının evrak altbilgileri olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public static Sonuc EvrakDosyaTopluKaydet(VeriErisim ve, Kullanici kullanan)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            VeriErisim ve2 = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            TABOCR to = new TABOCR(ve);

            //en son işlenen dosyanın CreationTime bilgisini veritabanından oku
            TNSDateTime enSonIslenenDosyaTarihi = new TNSDateTime();

            enSonIslenenDosyaTarihi = new TNSDateTime(kullanan.anaAdi);//tarih bilgisi taşınıyor

            if (enSonIslenenDosyaTarihi.isNull)
            {
                enSonIslenenDosyaTarihi = to.ListeleKontrol();
                if (enSonIslenenDosyaTarihi.isNull)
                    enSonIslenenDosyaTarihi = new TNSDateTime("01.01.2000");
            }

            string islemYapilacakKlasor = System.Configuration.ConfigurationManager.AppSettings.Get("EvrakDosyaYol") + "";

            OrtakFonksiyonlar.HataDosyaYaz("OCR.txt", DateTime.Now.ToString() + " : Klasör ve Dosya bilgileri alınacak", true);

            sonuc = to.KaydetKontrol(new TNSDateTime(DateTime.Now));

            ve.BeginTransaction();
            try
            {
                DirectoryInfo di = new DirectoryInfo(islemYapilacakKlasor);
                IEnumerable<FileInfo> dosyaListesi = KlasordekiDosyalariBul(di, "*.*");

                OrtakFonksiyonlar.HataDosyaYaz("OCR.txt", DateTime.Now.ToString() + " : Klasör ve Dosya bilgileri alındı", false);

                int sayac = 0;
                int kayitBasarili = 0;
                int kayitBasarisiz = 0;
                int kayitGecildi = 0;

                foreach (FileInfo file in dosyaListesi)
                {
                    sayac++;

                    if (sayac % 500 == 0)
                        OrtakFonksiyonlar.HataDosyaYaz("OCR.txt", DateTime.Now.ToString() + " : Dosyalar veri tabanına yazılıyor :" + sayac + "; Kayıt edilemeyen:" + kayitBasarisiz, false);

                    //Eğer ele alınan dosyanın CreationTime bilgisi, en son işlem yapılan dosya tarihinden küçük ise işlem yapılmasın
                    if (file.CreationTime.CompareTo(enSonIslenenDosyaTarihi.Oku()) < 0)
                    {
                        kayitGecildi++;
                        continue;
                    }

                    EvrakDosya eo = new EvrakDosya();
                    eo.islemTarih = new TNSDateTime(DateTime.Now);
                    eo.islemYapan = kullanan.mernis;
                    eo.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                    eo.dosyaAdi = file.Name;
                    eo.dosyaYolu = file.Directory.FullName;
                    eo.dosyaBoyutu = file.Length;
                    eo.dosyaTarihi = new TNSDateTime(file.CreationTime);

                    if (eo.dosyaYolu.IndexOf("\\ARSIV\\") > -1)
                    {
                        eo.arsivKod = OrtakFonksiyonlar.ConvertToInt(file.Directory.Name, 0);
                        if (file.Name.Length > 11)
                            eo.evrEvrakNo = file.Name.Substring(0, 11);
                        else
                            eo.evrEvrakNo = eo.arsivKod.ToString();
                    }
                    else
                    {
                        eo.arsivKod = 0;
                        eo.evrEvrakNo = file.Directory.Name;
                    }

                    //eo.eImzaDosyasi = 0;
                    //eo.eImzalananSiraNo = 0;
                    //eo.ocrYapildi = 0;
                    eo.ocrTarihi = new TNSDateTime();
                    if (eo.siraNo == 1)
                        eo.ustYaziDosyasi = 1;
                    else
                        eo.ustYaziDosyasi = 0;
                    //eo.iptal = 0;
                    eo.aciklama = "";

                    //Dosya daha önce kayıt edilmiş mi?
                    EvrakDosya kriter = new EvrakDosya();
                    kriter.dosyaAdi = eo.dosyaAdi;
                    kriter.dosyaYolu = eo.dosyaYolu;
                    //kriter.evrEvrakNo = eo.evrEvrakNo;
                    EvrakDosya[] eDosyalar = EvrakDosyaListele(ve2, kullanan, kriter, 1);
                    if (eDosyalar.Length > 0)
                    {
                        kayitGecildi++;
                        continue;
                    }

                    eo.siraNo = to.KodAl(eo.evrEvrakNo);

                    sonuc = to.Kaydet(eo);

                    if (sonuc.islemSonuc)
                        kayitBasarili++;
                    else
                    {
                        kayitBasarisiz++;
                        ve.hata = "";
                    }
                }

                OrtakFonksiyonlar.HataDosyaYaz("OCR.txt", DateTime.Now.ToString() + " : Dosyalar veri tabanına yazıldı", false);
                OrtakFonksiyonlar.HataDosyaYaz("OCR.txt", DateTime.Now.ToString() + " : Toplam dosya sayısı  :" + sayac, false);
                OrtakFonksiyonlar.HataDosyaYaz("OCR.txt", DateTime.Now.ToString() + " : Kayıt edilen         :" + kayitBasarili, false);
                OrtakFonksiyonlar.HataDosyaYaz("OCR.txt", DateTime.Now.ToString() + " : Kayıt edilemeyen     :" + kayitBasarisiz, false);
                OrtakFonksiyonlar.HataDosyaYaz("OCR.txt", DateTime.Now.ToString() + " : Önceden kayıt edilen :" + kayitGecildi, false);

                ve.Commit();

                OrtakFonksiyonlar.HataDosyaYaz("OCR.txt", DateTime.Now.ToString() + " : Commit edildi", false);
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{157FAA9E-84FD-40BA-9A8A-84F98615776F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();

            return sonuc;
        }

        public static IEnumerable<FileInfo> KlasordekiDosyalariBul(DirectoryInfo dirInfo, string searchPattern)
        {
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
            {
                if (di.Name.ToLower() == "antet" || di.Name.ToLower() == "temp")
                    continue;

                foreach (FileInfo fi in KlasordekiDosyalariBul(di, searchPattern))
                    yield return fi;
            }
            foreach (FileInfo fi in dirInfo.GetFiles(searchPattern))
                yield return fi;
        }

        public static EvrakDosya[] EvrakDosyaListele(VeriErisim veriErisim, Kullanici kullanan, EvrakDosya form, int top, bool dagitimDosyalariniDahilEt = false)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOCR tab = new TABOCR(ve);

            try
            {
                IDataReader dr = tab.Listele(form, top);

                if (dr != null)
                {
                    while (dr.Read())
                    {
                        form = new EvrakDosya();

                        form.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.EVREVRAKNO, "");
                        form.siraNo = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.SIRANO, 0);
                        form.dosyaYolu = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.DOSYAYOLU, "");
                        form.dosyaAdi = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.DOSYAADI, "");
                        form.dosyaTarihi = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.DOSYATARIHI);
                        form.dosyaBoyutu = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.DOSYABOYUTU, 0.0);
                        form.arsivKod = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.ARSIVKOD, 0);
                        form.ustYaziDosyasi = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.USTYAZIDOSYASI, 0);
                        form.eImzaDosyasi = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.EIMZADOSYASI, 0);
                        form.eImzalananSiraNo = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.EIMZALANANSIRANO, 0);
                        form.ocrYapildi = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.OCRYAPILDI, 0);
                        form.ocrTarihi = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.OCRTARIHI);
                        form.ocrGuid = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.OCRGUID, "");
                        form.islemYapan = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.ISLEMYAPAN, "");
                        form.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.ISLEMYAPANAD, "");
                        form.islemTarih = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.ISLEMTARIH);
                        form.iptal = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.IPTAL, 0);
                        form.aciklama = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.ACIKLAMA, "");
                        form.kriptoYapildi = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.KRIPTOYAPILDI, 0);
                        form.eImzaGuid = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.EIMZAGUID, "");
                        form.kriptoTarihi = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.KRIPTOTARIHI);
                        form.icerikYapildi = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.ICERIKYAPILDI, 0);
                        form.icerikGuid = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.ICERIKGUID, "");
                        form.icerikTarihi = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlar.ICERIKTARIHI);

                        bool ekle = true;
                        //Dagitim dosyaları istenmediği takdirde gösterilmesin
                        if (form.dosyaYolu.EndsWith(@"\Dagitim") && !dagitimDosyalariniDahilEt)
                            ekle = false;

                        if (ekle)
                            liste.objeler.Add(form);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712AX} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return (EvrakDosya[])liste.objeler.ToArray(typeof(EvrakDosya));
        }

        public static Sonuc EvrakDosyaKaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakDosya dosya, bool imzaKaldir = true)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOCR tab = new TABOCR(ve);

            //E-İmza sırasında dağıtım dosyaları kayıt edilirken evrak üzerindeki imzalar kaldırılmasın. H.Ö - 11:39 15.04.2014
            if (imzaKaldir)
            {
                bool evrakImzaliMi = EVREvrak.EvrakImzaliMi(ve, kullanan, dosya.evrEvrakNo);
                if (evrakImzaliMi)
                {
                    sonuc.DegerAta("İmzalı / Paraflı evrak üzerinde değişiklik yapılamaz!");

                    if (!sonuc.islemSonuc)
                    {
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }
            }

            ve.BeginTransaction();
            try
            {
                if (dosya.siraNo <= 0)
                    dosya.siraNo = tab.KodAl(dosya.evrEvrakNo);

                dosya.islemTarih = new TNSDateTime(DateTime.Now);

                sonuc = tab.Kaydet(dosya);
                if (!sonuc.islemSonuc)
                {
                    ve.hata = "";
                    sonuc = tab.Guncelle(dosya);
                }

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    //E-İmza sırasında dağıtım dosyaları kayıt edilirken tarihçeye yazılmasın. H.Ö - 11:39 15.04.2014
                    if (imzaKaldir)
                    {
                        if (dosya.eImzaDosyasi > 0)
                        {
                            EvrakTarihce tarihce = new EvrakTarihce();
                            tarihce.evrEvrakNo = dosya.evrEvrakNo;
                            tarihce.islem = ENUMEvrakTarihceDurum.EIMZALANDI.ToString();
                            tarihce.islem += "(" + dosya.dosyaAdi + ")";
                            sonuc = EVREvrakTarihce.Kaydet(ve, kullanan, tarihce);
                        }
                        else
                        {
                            EvrakTarihce tarihce = new EvrakTarihce();
                            tarihce.evrEvrakNo = dosya.evrEvrakNo;
                            tarihce.islem = (dosya.iptal != 1 ? "Dosya eklendi." : "Dosya Silindi");
                            tarihce.islem += "(" + dosya.dosyaAdi + ")";
                            sonuc = EVREvrakTarihce.Kaydet(ve, kullanan, tarihce);
                        }
                    }

                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712A9} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc EvrakDosyaGuncelle(VeriErisim veriErisim, Kullanici kullanan, EvrakDosya form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.evrEvrakNo))
                hata += "EvrEvrakNo boş bırakılamaz" + "<br>";
            if (form.siraNo == 0)
                hata += "SıraNo boş bırakılamaz" + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOCR tab = new TABOCR(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Guncelle(form);
                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{65BEA194-FAB4-489B-B1CB-48F7D221BD98} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc EvrakDosyaDagitimDosyalariniSil(VeriErisim veriErisim, Kullanici kullanan, string evrEvrakNo, bool imzaKaldir = true)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOCR tab = new TABOCR(ve);

            ve.BeginTransaction();
            try
            {
                EvrakDosya dosya = new EvrakDosya();
                dosya.evrEvrakNo = evrEvrakNo;
                dosya.dosyaYolu = @"%\Dagitim";
                sonuc = tab.Sil(dosya);

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI092 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{BFAC9C30-3C91-454A-A891-D0093465FF35} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc EvrakDosyaIcerikKaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakDosya dosya)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiriDosyaIcerik);
            TABOCR tab = new TABOCR(ve);

            ve.BeginTransaction();
            try
            {
                bool yeniKayit = false;
                if (string.IsNullOrEmpty(dosya.icerikGuid))
                {
                    dosya.icerikGuid = Guid.NewGuid().ToString();
                    yeniKayit = true;
                }

                if (!yeniKayit)
                    sonuc = tab.SilIcerik(dosya);

                sonuc = tab.KaydetIcerik(dosya);

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    sonuc.anahtar = dosya.icerikGuid;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712A9} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray EvrakDosyaIcerikListele(VeriErisim veriErisim, string kelime, int yil)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiriDosyaIcerik);
            TABOCR tab = new TABOCR(ve);

            IDataReader dr = tab.ListeleIcerik(kelime, yil);
            if (dr != null)
            {
                while (dr.Read())
                {
                    EvrakIcerikArama arama = new EvrakIcerikArama();
                    arama.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlarIcerik.EVREVRAKNO, "");
                    arama.dosyaSiraNo = VeriErisim.DegerAl(dr, (int)TABOCR.ENUMAlanlarIcerik.DOSYASIRANO, 0);
                    arama.klasor = VeriErisim.DegerAl(dr, 3, "");
                    arama.dosyaAd = VeriErisim.DegerAl(dr, 4, "");
                    liste.objeler.Add(arama);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return liste;
        }


        /// <summary>
        /// OCR Servisi için Servis bilgisini taşımak için.
        /// </summary>
        /// <value>
        /// The servis.
        /// </value>
        public OCR Servis { get; set; }

        /// <summary>
        /// OCR işlemi için servis bilgilerini alır
        /// </summary>
        public EVROCR()
        {
            this.Servis = new OCR();
            this.Servis.Url = System.Configuration.ConfigurationManager.AppSettings.Get("SgbnetOCRServisi") + "";
        }

        /// <summary>
        /// OCR işleminden geçirilecek olan dosyayı karşı tarafa gönderir
        /// </summary>
        /// <param name="bytes">byte[] değerinde dosya içeriği</param>
        /// <returns>işlenecek dosya için string değerinde GUID anahtarı</returns>
        public static string IslenecekDosyayiGonderAsByte(byte[] bytes)
        {
            EVROCR ocr = new EVROCR();
            return ocr.Servis.IslenecekDosyayiGonderAsByte(bytes);
        }

        /// <summary>
        /// OCR işleminden geçirilen dosyayıyı karşı taraftan alır
        /// </summary>
        /// <param name="guid">işlencek dosya için verilen GUID anahtarı</param>
        /// <returns>byte[] değerinde dosya içeriği</returns>
        public static byte[] IslenenDosyayiAlAsByte(string guid)
        {
            EVROCR ocr = new EVROCR();
            return ocr.Servis.IslenenDosyayiAlAsByte(guid);
        }

        /// <summary>
        /// OCR işleminden geçirilen dosyayıyı karşı taraftan siler
        /// </summary>
        /// <param name="guid">silinecek dosya için verilen GUID anahtarı</param>
        /// <returns>string değerinde başarılı ise "1", diğer durumda "0"</returns>
        public static string IslenenDosyayiSil(string guid)
        {
            EVROCR ocr = new EVROCR();
            return ocr.Servis.IslenenDosyayiSil(guid);
        }

        /// <summary>
        /// OCR işlemini karşı tarafta başlatır
        /// </summary>
        public static void OCRIsleminiBaslat()
        {
            EVROCR ocr = new EVROCR();
            ocr.Servis.OCRIsleminiBaslat();
        }

        /// <summary>
        /// OCR işleminden geçecek dosyaları bul ve işlem yerine gönder
        /// </summary>
        /// <returns>string değerinde sonuc bilgisi</returns>
        public static string OCRIslenecekDosyalariGonder()
        {
            string hataMsg = "";
            //OrtakFonksiyonlar.HataStrYaz("Dosya OCR için gönderilecek");
            EvrakDosya form = new EvrakDosya();
            form.ocrYapildi = (int)ENUMEvrakDosyaOCRIslem.YAPILACAK;

            IEVRServis servisEVR = TNS.EVR.Arac.Tanimla();
            //OrtakFonksiyonlar.HataStrYaz("Dosya OCR için listeleniyor");
            EvrakDosya[] dosyaListe = EvrakDosyaListele(null, new Kullanici(), form, 5000, true);
            if (dosyaListe != null && dosyaListe.Length > 0)
            {
                //OrtakFonksiyonlar.HataStrYaz("Dosya OCR için listeleme başarılı. Satır sayısı:" + dosyaListe.Length.ToString());
                //15 dk çalıştıktan sonra dursun
                Stopwatch watch = new Stopwatch();
                watch.Start();

                foreach (EvrakDosya dosya in dosyaListe)
                {
                    if (watch.Elapsed.Minutes >= 15)
                        break;

                    form = new EvrakDosya();

                    // pdf, tif veya jpeg ise işlem yap. 
                    string extension = Path.GetExtension(dosya.dosyaAdi);
                    if (extension == ".pdf" || extension == ".tif" || extension == ".jpeg")
                    {
                        try
                        {
                            //OrtakFonksiyonlar.HataStrYaz("Dosya OCR için listeleme dosya bilgisi alıyor");
                            FileInfo fi = new FileInfo(Path.Combine(dosya.dosyaYolu, dosya.dosyaAdi));
                            if (fi.Exists)
                            {
                                //OrtakFonksiyonlar.HataStrYaz("Dosya OCR için listeleme dosya bilgisi gönderiliyor");

                                //Dosyayı OCR a çevirmek üzere iş emri olarak kayıt et.
                                string guid = EVROCR.IslenecekDosyayiGonderAsByte(File.ReadAllBytes(fi.FullName));

                                if (!string.IsNullOrEmpty(guid) && guid.Length == 36)
                                {
                                    //OrtakFonksiyonlar.HataStrYaz("Dosya OCR için dosya bilgisi gönderildi. gelen guid:" + guid);

                                    form.ocrGuid = guid;
                                    form.ocrYapildi = (int)ENUMEvrakDosyaOCRIslem.YAPILIYOR;
                                }
                                else
                                    OrtakFonksiyonlar.HataStrYaz("Dosya OCR için dosya gönderilemedi. guid:" + guid);
                            }
                            else
                            {
                                form.ocrYapildi = (int)ENUMEvrakDosyaOCRIslem.DOSYAYOK;
                                //OrtakFonksiyonlar.HataStrYaz("Dosya OCR için dosya bulunamadı." + Path.Combine(dosya.dosyaYolu, dosya.dosyaAdi));
                            }
                        }
                        catch (Exception e)
                        {
                            OrtakFonksiyonlar.HataStrYaz("OCR dosya gönderme sırasında bir hata oluştu. Dosya:" + Path.Combine(dosya.dosyaYolu, dosya.dosyaAdi) + " Hata:" + e.Message + ";" + e.StackTrace);
                            throw;
                        }
                    }
                    else
                    {
                        form.ocrYapildi = (int)ENUMEvrakDosyaOCRIslem.YAPILMAYACAK;
                        //OrtakFonksiyonlar.HataStrYaz("Dosya OCR için listeleme OCR yapılmayacak karar verildi. extension:" + extension);
                    }

                    if (form.ocrYapildi > 0)
                    {
                        form.evrEvrakNo = dosya.evrEvrakNo;
                        form.siraNo = dosya.siraNo;
                        form.ocrTarihi = new TNSDateTime(DateTime.Now);

                        //OrtakFonksiyonlar.HataStrYaz("Dosya OCR için listeleme OCR bilgisi güncellenecek.");
                        Sonuc sonuc = servisEVR.EvrakDosyaOCRGuncelle(new Kullanici(), form);
                        if (!sonuc.islemSonuc)
                        {
                            OrtakFonksiyonlar.HataStrYaz("Dosya OCR için listeleme OCR bilgisi güncellenecek hata:" + sonuc.hataStr);
                            hataMsg = sonuc.hataStr;
                            break;
                        }
                        //else
                        //    OrtakFonksiyonlar.HataStrYaz("Dosya OCR için listeleme OCR bilgisi güncelleme başarılı.");
                    }
                }

                //OrtakFonksiyonlar.HataStrYaz("Dosya OCR için listeleme çıkıyor");
                watch.Stop();
            }
            else
                OrtakFonksiyonlar.HataStrYaz("Dosya OCR için listeleme !!!!başarısız!!!!");

            return hataMsg;
        }

        /// <summary>
        /// OCR işleminden geçmekte olan dosyaları karşı taraftan al ve orjinal dosya üzerine kopyala
        /// </summary>
        /// <returns>string değerinde sonuc bilgisi</returns>
        public static string OCRIslenenDosyalariAl()
        {
            string hataMsg = "";

            EvrakDosya form = new EvrakDosya();
            form.ocrYapildi = (int)ENUMEvrakDosyaOCRIslem.YAPILIYOR;

            IEVRServis servisEVR = TNS.EVR.Arac.Tanimla();

            EvrakDosya[] dosyaListe = EvrakDosyaListele(null, new Kullanici(), form, 0, false);
            if (dosyaListe != null && dosyaListe.Length > 0)
            {
                //15 dk çalıştıktan sonra dursun
                Stopwatch watch = new Stopwatch();
                watch.Start();

                foreach (EvrakDosya dosya in dosyaListe)
                {
                    if (watch.Elapsed.Minutes >= 100)
                        break;

                    form = new EvrakDosya();

                    //OCR a çevrilmiş olan dosyayı almak için.
                    byte[] dosyaIcerik = new byte[0];
                    try { dosyaIcerik = EVROCR.IslenenDosyayiAlAsByte(dosya.ocrGuid); }
                    catch (Exception e)
                    {
                        OrtakFonksiyonlar.HataStrYaz("{381B5426-B487-4F57-AF0B-6E11127F6B08} OCR Dosya Alma İşlemi Hata:" + dosya.ocrGuid + " * " + e.Message + e.StackTrace);
                        continue;
                    }

                    if (dosyaIcerik != null && dosyaIcerik.Length > 0)
                    {
                        string dosyaAdi = dosya.dosyaYolu + "\\" + dosya.dosyaAdi;
                        string yedekDosyaAdi = dosyaAdi + ".orj";


                        if (File.Exists(dosyaAdi))
                        {
                            try
                            {
                                if (File.Exists(yedekDosyaAdi)) File.Delete(yedekDosyaAdi);
                                File.Move(dosyaAdi, yedekDosyaAdi);
                            }
                            catch (Exception e)
                            {
                                OrtakFonksiyonlar.HataStrYaz("{EFDA8D64-5098-4946-83DD-860E230D0A10} OCR Dosya Alma İşlemi Hata:" + dosyaAdi + " * " + yedekDosyaAdi + " * " + e.Message + e.StackTrace);
                                continue;
                            }
                        }

                        if (!File.Exists(dosyaAdi))
                        {
                            File.WriteAllBytes(dosyaAdi, dosyaIcerik);
                            form.ocrYapildi = (int)ENUMEvrakDosyaOCRIslem.YAPILDI;
                        }
                    }

                    if (form.ocrYapildi > 0)
                    {
                        form.evrEvrakNo = dosya.evrEvrakNo;
                        form.siraNo = dosya.siraNo;
                        form.ocrTarihi = new TNSDateTime(DateTime.Now);

                        Sonuc sonuc = servisEVR.EvrakDosyaOCRGuncelle(new Kullanici(), form);

                        if (sonuc.islemSonuc)
                        {
                            string durum = EVROCR.IslenenDosyayiSil(dosya.ocrGuid);
                            if (durum != "1")
                                hataMsg = "silinemedi";
                        }
                        else
                        {
                            hataMsg = sonuc.hataStr;
                            OrtakFonksiyonlar.HataStrYaz("{DA6E9C09-D714-455B-883A-CBBA93AB07E6} OCR Dosya Alma İşlemi Hata:" + dosya.ocrGuid + " * " + hataMsg);
                            break;
                        }
                    }
                }
                watch.Stop();
            }

            return hataMsg;
        }

        public static byte[] EvrakinDosyasiniGetir(string evrEvrakNo, int siraNo)
        {
            byte[] dosyaIcerik = new byte[0];
            if (string.IsNullOrEmpty(evrEvrakNo.Trim()) || siraNo == 0)
                return dosyaIcerik;

            EvrakDosya form = new EvrakDosya();
            form.evrEvrakNo = evrEvrakNo;
            form.siraNo = siraNo;

            IEVRServis servisEVR = TNS.EVR.Arac.Tanimla();

            EvrakDosya[] dosyaListe = EvrakDosyaListele(null, new Kullanici(), form, 1, false);

            foreach (EvrakDosya dosya in dosyaListe)
            {
                string dosyaYol = Path.Combine(dosya.dosyaYolu, dosya.dosyaAdi);
                FileInfo imzaliDosya = new FileInfo(dosyaYol);

                if (imzaliDosya.Exists)
                {
                    string tmpFileName = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();

                    StratekCrypto.Facade f = new StratekCrypto.Facade();
                    f.Password = "28bdb479-f804-4ea7-b0bc-531d4ece6c85";
                    f.DecryptFileByInternalPrivateKey(imzaliDosya.FullName, tmpFileName);

                    dosyaIcerik = File.ReadAllBytes(tmpFileName);
                    File.Delete(tmpFileName);
                }
            }

            return dosyaIcerik;
        }
    }
}

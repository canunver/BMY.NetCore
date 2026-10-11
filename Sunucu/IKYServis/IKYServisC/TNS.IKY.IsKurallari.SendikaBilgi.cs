using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.IKY.IsKurallari
{
    /// <summary>
    /// IKYSendikaBilgi formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class IKYSendikaBilgi
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriIKY";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, SendikaBilgi form)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static void AlanlarGecerliMi(Kullanici kullanan, SendikaBilgi form, Sonuc sonuc)
        {
            if (string.IsNullOrWhiteSpace(form.sendikaAd))
                sonuc.DegerAta("Sendika adı boş olamaz!");
            if (string.IsNullOrWhiteSpace(form.iBAN))
                sonuc.DegerAta("IBAN boş olamaz!");
            if (string.IsNullOrWhiteSpace(form.hesapNo))
                sonuc.DegerAta("Hesap no boş olamaz!");
            if (form.topluGorusmePuani <= 0)
                sonuc.DegerAta("Toplu görüşme puanı giriniz!");
        }


        /// <summary>
        /// SendikaBilgi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SendikaBilgi yapısında SendikaBilgi:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, SendikaBilgi form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("İşlem yapmaya yetkiniz bulunmamaktadır!");
                return s;
            }

            AlanlarGecerliMi(kullanan, form, s);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSENDIKABILGI tabIKYSENDIKABILGI = new TABIKYSENDIKABILGI(ve);
            string degismis;
            try
            {
                degismis = "$" + OrtakFonksiyonlar.SifreleOrtakAnahtar(form.sendikaAd);
            }
            catch (Exception e)
            {
                degismis = form.sendikaAd;
            }
            form.sendikaAd = degismis;
            ve.BeginTransaction();

            if (form.sendikaNo == 0)
            {
                int siraNo = tabIKYSENDIKABILGI.SendikaNoBul(form);
                if (!string.IsNullOrWhiteSpace(ve.hata))
                    s.DegerAta("Sıra no alınamadı!");
                if (s.islemSonuc)
                {
                    form.sendikaNo = siraNo;
                    s = tabIKYSENDIKABILGI.Ekle(form);
                    s.anahtar = siraNo.ToString();
                }
            }
            else
                s = tabIKYSENDIKABILGI.Guncelle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// SendikaBilgi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SendikaBilgi yapısında SendikaBilgi:  bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, SendikaBilgi form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form, true);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, SendikaBilgi form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, SendikaBilgi form)
        {
            return new Sonuc(true, "", "");
        }

        internal static TNSCollection Listele(string okunanXML)
        {
            if (string.IsNullOrWhiteSpace(okunanXML)) return null;
            TNSCollection pVAN = new TNSCollection();

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                while (r.Read())
                {
                    pVAN.Add(ReaderdanOku(r, true));
                }
                r.Close();
            }
            return pVAN;
        }


        /// <summary>
        /// SendikaBilgi listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SendikaBilgi yapısında SendikaBilgi:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, SendikaBilgi form, bool persOku)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan == null ? "Sistem" : TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSENDIKABILGI tabIKYSENDIKABILGI = new TABIKYSENDIKABILGI(ve);
            //TABIKYSENDIKAPERSONEL tabIKYSENDIKAPERSONEL = new TABIKYSENDIKAPERSONEL(ve);

            IDataReader dr;
            dr = tabIKYSENDIKABILGI.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                oa.Ekle(ReaderdanOku(dr, false));
            }
            dr.Close();
            if (persOku)
            {
                TNSCollection al;
                foreach (SendikaBilgi formIc in oa.objeler)
                {
                    dr = tabIKYSENDIKABILGI.ListeleSendPers(formIc.sendikaNo, null);
                    if (dr == null)
                    {
                        ve.VeriErisimKapat();
                        oa.sonuc.DegerAta(ve.hata);
                        return oa;
                    }
                    al = new TNSCollection();
                    while (dr.Read())
                    {
                        PersonelSendika detay = new PersonelSendika();
                        detay.kod = VeriErisim.DegerAl(dr, 0, 0);
                        detay.tcKimlikNo = VeriErisim.DegerAl(dr, 1, "");
                        detay.ad = VeriErisim.DegerAl(dr, 2, "");
                        detay.soyad = VeriErisim.DegerAl(dr, 3, "");
                        detay.sicilNo = VeriErisim.DegerAl(dr, 4, "");
                        detay.kesintiOrani = VeriErisim.DegerAl(dr, 5, 0.0);
                        al.Add(detay);
                    }
                    formIc.sendikaPersonel = al;
                    dr.Close();
                }
            }
            ve.VeriErisimKapat();
            return oa;
        }

        private static SendikaBilgi ReaderdanOku(IDataReader dr, bool kisiKesintiOraniEkle)
        {
            SendikaBilgi detay = new SendikaBilgi();
            detay.sendikaNo = VeriErisim.DegerAl(dr, (int)TABIKYSENDIKABILGI.ENUMAlanlarSB.SENDIKANO, 0);
            detay.sendikaAd = VeriErisim.DegerAl(dr, (int)TABIKYSENDIKABILGI.ENUMAlanlarSB.SENDIKAAD, "");
            if (detay.sendikaAd.StartsWith("$"))
            {
                detay.sendikaAd = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(detay.sendikaAd.Substring(1));
            }
            detay.adres = VeriErisim.DegerAl(dr, (int)TABIKYSENDIKABILGI.ENUMAlanlarSB.ADRES, "");
            detay.iBAN = VeriErisim.DegerAl(dr, (int)TABIKYSENDIKABILGI.ENUMAlanlarSB.IBAN, "");
            detay.hesapNo = VeriErisim.DegerAl(dr, (int)TABIKYSENDIKABILGI.ENUMAlanlarSB.HESAPNO, "");
            detay.topluGorusmePuani = VeriErisim.DegerAl(dr, (int)TABIKYSENDIKABILGI.ENUMAlanlarSB.TOPLUGORUSMEPUANI, 0);
            detay.kesintiOrani = VeriErisim.DegerAl(dr, (int)TABIKYSENDIKABILGI.ENUMAlanlarSB.KESINTIORANI, 0.0);
            if (kisiKesintiOraniEkle)
                detay.kesintiOrani = VeriErisim.DegerAl(dr, (int)TABIKYSENDIKABILGI.ENUMAlanlarSB.KESINTIORANI + 1, 0.0);
            return detay;
        }

        internal static Sonuc PersonelSendikadanCikart(VeriErisim veriErisim, Kullanici kullanan, int pKod)
        {
            Sonuc s = new Sonuc();
            if (pKod <= 0)
                s.DegerAta("Lütfen personel seçiniz");
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                s.DegerAta("Bu işlemi sistem yöneticileri yapabilir");
            if (s.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
                TABIKYSENDIKABILGI tab = new TABIKYSENDIKABILGI(ve);

                s = tab.PersonelSendikadanCikart(pKod);
                ve.VeriErisimKapat();
            }
            return s;
        }

        internal static Sonuc PersonelSendikaOranDegistir(VeriErisim veriErisim, Kullanici kullanan, int pKod, double kesintiOrani)
        {
            Sonuc s = new Sonuc();
            if (pKod <= 0)
                s.DegerAta("Lütfen personel seçiniz");
            if (kesintiOrani <= 0)
                s.DegerAta("Lütfen kesinti oranını giriniz!");
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                s.DegerAta("Bu işlemi sistem yöneticileri yapabilir");
            if (s.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
                TABIKYSENDIKABILGI tab = new TABIKYSENDIKABILGI(ve);

                s = tab.PersonelSendikaOranDegistir(pKod, kesintiOrani);
                ve.VeriErisimKapat();
            }
            return s;
        }


        internal static Sonuc PersonelSendikayaEkle(VeriErisim veriErisim, Kullanici kullanan, int sendikaKod, TNSCollection pKodList, double kesintiOrani, bool yetkiKontroluYap)
        {
            Sonuc s = new Sonuc();

            if (sendikaKod <= 0 && kesintiOrani > 0)
                s.DegerAta("Lütfen sendika seçiniz!");
            if (kesintiOrani <= 0 && sendikaKod > 0)
                s.DegerAta("Lütfen kesinti oranını giriniz!");
            if (yetkiKontroluYap)
            {
                if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                    s.DegerAta("Bu işlemi sistem yöneticileri yapabilir!");
            }
            if (s.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
                TABIKYSENDIKABILGI tab = new TABIKYSENDIKABILGI(ve);

                foreach (int pKod in pKodList)
                {
                    bool sendikasiVar = false;
                    if (tab.SendikadaKoduBul(pKod) > 0)
                        sendikasiVar = true;
                    ve.BeginTransaction();
                    if (sendikasiVar)
                        s = tab.PersonelSendikadanCikart(pKod);
                    if (s.islemSonuc && sendikaKod > 0 && kesintiOrani > 0)
                        s = tab.Ekle(sendikaKod, pKod, kesintiOrani);
                    if (s.islemSonuc)
                        ve.Commit();
                    else
                    {
                        ve.Rollback();
                        break;
                    }
                }
                ve.VeriErisimKapat();
            }
            return s;
        }

        internal static ObjectVAN OkuKisiSendikaBilgi(VeriErisim veriErisim, Kullanici kullanan, string personelTCKimlikNo)
        {
            ObjectVAN ov = new ObjectVAN();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan == null ? "Sistem" : TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSENDIKABILGI tabIKYSENDIKABILGI = new TABIKYSENDIKABILGI(ve);

            IDataReader dr = tabIKYSENDIKABILGI.ListeleSendPers(-1, personelTCKimlikNo);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                ov.sonuc.DegerAta(ve.hata);
                return ov;
            }
            if (dr.Read())
            {
                SendikaBilgi detay = new SendikaBilgi();
                detay.kesintiOrani = VeriErisim.DegerAl(dr, 5, 0.0);
                detay.sendikaNo = VeriErisim.DegerAl(dr, 6, 0);
                ov.o = detay;
            }
            dr.Close();
            ve.VeriErisimKapat();
            return ov;
        }
    }
}

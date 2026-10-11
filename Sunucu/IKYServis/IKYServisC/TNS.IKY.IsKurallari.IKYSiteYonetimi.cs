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
    /// IKYLojmanSite formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class IKYSiteYonetimi
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, SozlukGider form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, SozlukGider form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// SozlukGider nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SozlukGider yapısında SozlukGider:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetSozlukGider(VeriErisim veriErisim, Kullanici kullanan, SozlukGider form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(PersonelResI.LojmanSite001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSiteYonetimi TAB = new TABIKYSiteYonetimi(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.kod > 0)
                    s = TAB.GuncelleSozlukGider(form);
                else
                {
                    form.kod = TABGenel.SayacDegerAlInt(ve, TABIKYSiteYonetimi.TABLOSOZLUKGIDER);
                    s = TAB.KaydetSozlukGider(form);
                }
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// SozlukGider nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SozlukGider yapısında SozlukGider:  bilgilerini tutan değişken</param>
        public static ObjectVAN OkuSozlukGider(VeriErisim veriErisim, Kullanici kullanan, SozlukGider form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleSozlukGider(veriErisim, kullanan, form);
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
        public static bool GormeYetkisiVarMi(Kullanici kullanan, SozlukGider form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, SozlukGider form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// SozlukGider listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SozlukGider yapısında SozlukGider:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleSozlukGider(VeriErisim veriErisim, Kullanici kullanan, SozlukGider form)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSiteYonetimi TAB = new TABIKYSiteYonetimi(ve);

            IDataReader dr = TAB.OkuSozlukGider(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            int AlanlarSonIndex = (Enum.GetNames(typeof(TABIKYSiteYonetimi.ENUMAlanlarSOZLUKGIDER))).Length;
            while (dr.Read())
            {
                SozlukGider detay = new SozlukGider();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMAlanlarSOZLUKGIDER.KOD, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMAlanlarSOZLUKGIDER.AD, "");
                detay.eldenGor = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMAlanlarSOZLUKGIDER.ELDENGOR, 0);
                detay.endexGor = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMAlanlarSOZLUKGIDER.ENDEXGOR, 0);
                detay.aciklamaGor = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMAlanlarSOZLUKGIDER.ACIKLAMAGOR, 0);
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static Sonuc SilSozlukGider(Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc();
            //object eski = null, yeni = null; //Tarihçe için

            //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";

            if (kod < 0)
                hata += "Sozluk gider kodu boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
            {
                TABIKYSiteYonetimi tab = new TABIKYSiteYonetimi(ve);
                sonuc = tab.SilSozlukGider(kod);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";

                //    //Tarihçe Kaydet ********************
                //    yeni = p;
                //    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, ENUMTARIHCEISLEMTIP.SIL);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// SiteGider nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SiteGider yapısında SiteGider:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetSiteGider(VeriErisim veriErisim, Kullanici kullanan, int siteKod, int yil, int ay, TNSCollection giderBilgiler)
        {
            Sonuc s = new Sonuc(true, "", "");
            //if (!YazmaYetkisiVarMi(kullanan, form))
            //{
            //    s.DegerAta(PersonelResI.LojmanSite001);
            //    return s;
            //}

            //s = AlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    return s;
            //}
            string hata = "";

            if (!TNS.IKY.Arac.DonemAcikMi(yil, ay))
                hata = "Bulunduğunuz maaş yıl-ay dönemi içerisinde olmadığınızdan kayıt gerçekleştirilemedi.";

            if (siteKod <= 0)
                hata += " Gider gireceğiniz siteyi seçmelisiniz.";

            if (!string.IsNullOrWhiteSpace(hata))
            {
                s.islemSonuc = false;
                s.hataStr = hata;
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            SiteGider sg = new SiteGider();
            sg.yil = yil;
            sg.ay = ay;
            sg.siteKod = siteKod;
            //VarMi v = SiteGideriVarMi(ve, kullanan, sg);
            //if (!v.s.islemSonuc || v.varMi)
            //{
            //    if (v.s.islemSonuc)
            //        s.DegerAta("Daha önce aynı site, yıl,ay ve gider türünden kayıt girilmiş.");
            //    else
            //        s.DegerAta(v.s.hataStr);
            //}

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABIKYSiteYonetimi TAB = new TABIKYSiteYonetimi(ve);
            ve.BeginTransaction();

            foreach (SiteGider form in giderBilgiler)
            {
                if (!s.islemSonuc) break;
                form.yil = yil;
                form.ay = ay;
                form.siteKod = siteKod;
                if (form.kod > 0)
                    s = TAB.GuncelleSiteGider(form);
                else
                {
                    form.kod = TABGenel.SayacDegerAlInt(ve, TABIKYSiteYonetimi.TABLOSITEGIDER);
                    s = TAB.KaydetSiteGider(form);
                }
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// SozlukGider listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SozlukGider yapısında SozlukGider:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleSiteGider(VeriErisim veriErisim, Kullanici kullanan, SiteGider form)
        {
            ObjectArray oa = new ObjectArray();
            //if (!GormeYetkisiVarMi(kullanan, form))
            //{
            //    oa.sonuc.DegerAta(PersonelResI.LojmanSite002);
            //    return oa;
            //}

            //Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    oa.sonuc.DegerAta(s.hataStr);
            //    return oa;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSiteYonetimi TAB = new TABIKYSiteYonetimi(ve);

            IDataReader dr = TAB.ListeleSiteGider(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            int AlanlarSonIndex = (Enum.GetNames(typeof(TABIKYSiteYonetimi.ENUMALANLARSITEGIDER))).Length;
            while (dr.Read())
            {
                SiteGider detay = new SiteGider();

                detay.giderKod = VeriErisim.DegerAl(dr, AlanlarSonIndex + 1, 0);
                detay.giderTurAd = VeriErisim.DegerAl(dr, AlanlarSonIndex, "");
                detay.kod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEGIDER.KOD, 0);
                detay.otomatik = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEGIDER.OTOMATIK, 0) == 1;
                detay.miktar = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEGIDER.MIKTAR, 0.0);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEGIDER.ACIKLAMA, "");
                detay.eskiKod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEGIDER.ESKIKOD, 0);

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static VarMi SiteGideriVarMi(VeriErisim veriErisim, Kullanici kullanan, SiteGider sg)
        {
            VarMi varMi = new VarMi();
            varMi.s = new Sonuc();
            varMi.varMi = false;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSiteYonetimi TAB = new TABIKYSiteYonetimi(ve);
            sg.varMi = true;

            IDataReader dr = TAB.OkuSiteGider(sg);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                varMi.s.DegerAta(ve.hata);
                return varMi;
            }

            if (dr.Read())
            {
                if (sg.kod != 0)
                {
                    int kod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEGIDER.KOD, 0);
                    if (kod != sg.kod)
                        varMi.varMi = true;
                }
                else
                    varMi.varMi = true;
            }

            dr.Close();
            ve.VeriErisimKapat();
            return varMi;
        }

        /// <summary>
        /// SiteGider nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SiteGider yapısında SiteGider:  bilgilerini tutan değişken</param>
        public static ObjectVAN OkuSiteGider(VeriErisim veriErisim, Kullanici kullanan, SiteGider form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleSiteGider(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        public static Sonuc SilSiteGider(Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc();
            //object eski = null, yeni = null; //Tarihçe için

            //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";

            if (kod < 0)
                hata += "Site gider kodu boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
            {
                TABIKYSiteYonetimi tab = new TABIKYSiteYonetimi(ve);
                sonuc = tab.SilSiteGider(kod);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";

                //    //Tarihçe Kaydet ********************
                //    yeni = p;
                //    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, ENUMTARIHCEISLEMTIP.SIL);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// SiteGider nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SiteGider yapısında SiteGider:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetSiteKesintisi(VeriErisim veriErisim, Kullanici kullanan, PersonelSiteKesinti form)
        {
            Sonuc s = new Sonuc(true, "", "");
            //if (!YazmaYetkisiVarMi(kullanan, form))
            //{
            //    s.DegerAta(PersonelResI.LojmanSite001);
            //    return s;
            //}

            //s = AlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    return s;
            //}
            string hata = "";

            if (!TNS.IKY.Arac.DonemAcikMi(form.yil, form.ay))
                hata = "Bulunduğunuz maaş yıl-ay dönemi içerisinde olmadığınızdan kayıt gerçekleştirilemedi.";

            if (form.pLojmanKod <= 0)
                hata += " Gider gireceğiniz personel dairesini seçmelisiniz.";

            if (form.giderKod <= 0)
                hata += " Gider gireceğiniz gider türünü seçmelisiniz.";

            if (form.yil <= 0)
                hata += " Gider gireceğiniz yılı seçmelisiniz.";

            if (form.ay <= 0)
                hata += " Gider gireceğiniz ayı seçmelisiniz.";

            if (!string.IsNullOrWhiteSpace(hata))
            {
                s.islemSonuc = false;
                s.hataStr = hata;
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSiteYonetimi TAB = new TABIKYSiteYonetimi(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.kod > 0)
                    s = TAB.GuncellePersonelSiteKesintisi(form);
                else
                {
                    VarMi v = PersonelSiteKesintisiVarMi(null, kullanan, form);
                    if (v.s.islemSonuc && !v.varMi)
                    {
                        form.kod = TABGenel.SayacDegerAlInt(ve, TABIKYSiteYonetimi.TABLOSITEKESINTISI);
                        s = TAB.KaydetPersonelSiteKesintisi(form);
                    }
                    else
                    {
                        s.DegerAta("Daha önce aynı personel-daire, yıl,ay ve gider türünden kayıt girilmiş.");
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

        /// <summary>
        /// PersonelSiteKesinti listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelSiteKesinti yapısında PersonelSiteKesinti:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleSiteKesintisi(VeriErisim veriErisim, Kullanici kullanan, PersonelSiteKesinti form)
        {
            ObjectArray oa = new ObjectArray();
            //if (!GormeYetkisiVarMi(kullanan, form))
            //{
            //    oa.sonuc.DegerAta(PersonelResI.LojmanSite002);
            //    return oa;
            //}

            //Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    oa.sonuc.DegerAta(s.hataStr);
            //    return oa;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSiteYonetimi TAB = new TABIKYSiteYonetimi(ve);

            IDataReader dr;
            dr = TAB.OkuPersonelSiteKesintisi(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            int AlanlarSonIndex = (Enum.GetNames(typeof(TABIKYSiteYonetimi.ENUMALANLARSITEKESINTISI))).Length;
            while (dr.Read())
            {
                PersonelSiteKesinti detay = new PersonelSiteKesinti();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEKESINTISI.KOD, 0);
                detay.siteKesTopKod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEKESINTISI.SKTKOD, 0);
                detay.giderKod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEKESINTISI.GIDERKOD, 0);
                detay.aktif = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEKESINTISI.AKTIF, 0);
                detay.endex = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEKESINTISI.ENDEX, 0.0);
                detay.tutar = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEKESINTISI.TUTAR, 0.0);
                detay.elden = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEKESINTISI.ELDEN, 0.0);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARSITEKESINTISI.ACIKLAMA, "");
                detay.siteKesTopKod = VeriErisim.DegerAl(dr, AlanlarSonIndex, 0);
                detay.pLojmanKod = VeriErisim.DegerAl(dr, AlanlarSonIndex + 1, 0);
                detay.yil = VeriErisim.DegerAl(dr, AlanlarSonIndex + 2, 0);
                detay.ay = VeriErisim.DegerAl(dr, AlanlarSonIndex + 3, 0);
                detay.toplamTutar = VeriErisim.DegerAl(dr, AlanlarSonIndex + 4, 0.0);
                //detay.siteAdi = VeriErisim.DegerAl(dr, AlanlarSonIndex + 1, "");
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        /// <summary>
        /// PersonelSiteKesinti nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelSiteKesinti yapısında PersonelSiteKesinti:  bilgilerini tutan değişken</param>
        public static ObjectVAN OkuSiteKesintisi(VeriErisim veriErisim, Kullanici kullanan, PersonelSiteKesinti form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleSiteKesintisi(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        public static Sonuc SilSiteKesintisi(Kullanici kullanan, PersonelSiteKesinti sk)
        {
            Sonuc sonuc = new Sonuc();
            //object eski = null, yeni = null; //Tarihçe için

            //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";

            if (sk.kod < 0)
                hata += "Personel site kesintisi kodu boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
            {
                TABIKYSiteYonetimi tab = new TABIKYSiteYonetimi(ve);
                sonuc = tab.SilPersonelSiteKesintisi(sk);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";

                //    //Tarihçe Kaydet ********************
                //    yeni = p;
                //    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, ENUMTARIHCEISLEMTIP.SIL);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc SilSiteKesintisiToplam(Kullanici kullanan, int toplamKod)
        {
            Sonuc sonuc = new Sonuc();
            //object eski = null, yeni = null; //Tarihçe için

            //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";

            if (toplamKod < 0)
                hata += "Personel site kesintisi kodu boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();


            TABIKYSiteYonetimi tab = new TABIKYSiteYonetimi(ve);
            sonuc = tab.SilPersonelSiteKesintisi(new PersonelSiteKesinti { siteKesTopKod = toplamKod });
            if (sonuc.islemSonuc)
                sonuc = tab.SilPersonelSiteKesintiToplam(toplamKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";

                //    //Tarihçe Kaydet ********************
                //    yeni = p;
                //    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, ENUMTARIHCEISLEMTIP.SIL);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static VarMi PersonelSiteKesintisiVarMi(VeriErisim ve, Kullanici kullanan, PersonelSiteKesinti sg)
        {
            VarMi varMi = new VarMi();
            varMi.s = new Sonuc();
            varMi.varMi = false;
            sg.varMi = true;

            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSiteYonetimi TAB = new TABIKYSiteYonetimi(ve);

            IDataReader dr;
            dr = TAB.ListelePersonelKesinti(sg);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                varMi.s.DegerAta(ve.hata);
                return varMi;
            }

            if (dr.Read())
                varMi.varMi = true;

            dr.Dispose();
            //ve.VeriErisimKapat();
            return varMi;
        }

        public static Sonuc SiteKesintileriTopluKaydet(VeriErisim veriErisim, Kullanici kullanan, PersonelSiteKesinti[] kesintiler)
        {
            Sonuc sonuc = new Sonuc();
            PersonelSiteKesinti k = kesintiler[0];

            if (!TNS.IKY.Arac.DonemAcikMi(k.yil, k.ay))
                sonuc.hataStr = "Bulunduğunuz maaş yıl-ay dönemi içerisinde olmadığınızdan kayıt gerçekleştirilemedi.";

            if (k.pLojmanKod <= 0)
                sonuc.hataStr += " Gider gireceğiniz personel dairesini seçmelisiniz.";

            if (k.giderKod <= 0)
                sonuc.hataStr += " Gider gireceğiniz gider türünü seçmelisiniz.";

            if (k.yil <= 0)
                sonuc.hataStr += " Gider gireceğiniz yılı seçmelisiniz.";

            if (k.ay <= 0)
                sonuc.hataStr += " Gider gireceğiniz ayı seçmelisiniz.";

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            double toplamTutar = 0;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            //VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSiteYonetimi tabKesinti = new TABIKYSiteYonetimi(ve);
            ve.BeginTransaction();

            if (kesintiler[0].siteKesTopKod > 0)
                sonuc = tabKesinti.SilPersonelSiteKesintisi(kesintiler[0]);

            if (sonuc.islemSonuc)
            {
                PersonelSiteKesintiToplam t = new PersonelSiteKesintiToplam();
                t.kod = kesintiler[0].siteKesTopKod;
                t.pLojmanKod = kesintiler[0].pLojmanKod;
                t.yil = kesintiler[0].yil;
                t.ay = kesintiler[0].ay;
                t.toplamTutar = kesintiler[0].toplamTutar;

                if (kesintiler[0].siteKesTopKod > 0)
                    sonuc = tabKesinti.GuncellePersonelSiteKesintiToplam(t);
                else
                {
                    t.kod = TABGenel.SayacDegerAlInt(ve, TABIKYSiteYonetimi.TABLOSITEKESINTITOPLAM);
                    sonuc = tabKesinti.KaydetPersonelSiteKesintiToplam(t);
                }

                if (sonuc.islemSonuc)
                {
                    IDataReader r;
                    double miktar = 0;

                    foreach (PersonelSiteKesinti sk in kesintiler)
                    {
                        VarMi v = PersonelSiteKesintisiVarMi(ve, kullanan, new PersonelSiteKesinti { siteKesTopKod = t.kod, giderKod = sk.giderKod });
                        if (v.s.islemSonuc && !v.varMi)
                        {
                            miktar = 0;
                            if (sk.aktif == 1)
                            {
                                r = tabKesinti.OkuSiteGider(new SiteGider { siteKod = sk.siteKod, yil = sk.yil, ay = sk.ay, giderKod = sk.giderKod });
                                if (r.Read())
                                    miktar = VeriErisim.DegerAl(r, 6, 0.0);
                                if (sk.endex > 0)
                                    sk.tutar = miktar * sk.endex + sk.elden;
                                else
                                    sk.tutar = miktar + sk.elden;
                                r.Dispose();
                            }
                            else
                                sk.tutar = 0;

                            toplamTutar += sk.tutar;
                            sk.siteKesTopKod = t.kod;
                            sk.kod = TABGenel.SayacDegerAlInt(ve, TABIKYSiteYonetimi.TABLOSITEKESINTISI);
                            sonuc = tabKesinti.KaydetPersonelSiteKesintisi(sk);
                        }
                        else
                        {
                            if (v.s.islemSonuc)
                                sonuc.DegerAta("Daha önce aynı personel-daire, yıl,ay ve gider türünden kayıt girilmiş.");
                            else
                                sonuc.DegerAta(v.s.hataStr);
                        }

                        if (!sonuc.islemSonuc)
                            break;
                    }
                }

                if (sonuc.islemSonuc && toplamTutar > 0)
                    sonuc = tabKesinti.GuncellePersonelSiteKesintiToplamTutar(new PersonelSiteKesintiToplam { kod = t.kod, toplamTutar = toplamTutar });
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// PersonelSiteKesinti listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelSiteKesinti yapısında PersonelSiteKesinti:  bilgilerini tutan değişken</param>
        public static ObjectArray ListelePersonelKesinti(VeriErisim veriErisim, Kullanici kullanan, PersonelSiteKesinti form)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSiteYonetimi TAB = new TABIKYSiteYonetimi(ve);

            IDataReader dr;
            dr = TAB.ListelePersonelKesinti(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            int AlanlarSonIndex = (Enum.GetNames(typeof(TABIKYSiteYonetimi.ENUMALANLARKESINTI))).Length;
            while (dr.Read())
            {
                PersonelSiteKesinti detay = new PersonelSiteKesinti();
                detay.personelKod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.PKOD, 0);
                detay.yil = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.YIL, 0);
                detay.ay = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.AY, 0);
                detay.pLojmanKod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.PLOJMANKOD, 0);
                detay.giderKod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.GIDERKOD, 0);
                detay.aktif = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.AKTIF, 0);
                detay.endex = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.ENDEX, 0.0);
                detay.tutar = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.TUTAR, 0.0);
                detay.elden = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.ELDEN, 0.0);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.ACIKLAMA, "");

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static Sonuc SilPersonelKesinti(Kullanici kullanan, PersonelSiteKesinti sk)
        {
            Sonuc sonuc = new Sonuc();
            //object eski = null, yeni = null; //Tarihçe için

            //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";

            if (sk.pLojmanKod < 0 && sk.personelKod < 0)
                hata += "Personel kodu ya da personel daire kodu boş bırakılamaz.<br>";

            if (sk.yil < 0)
                hata += "Kesinti yılı boş bırakılamaz.<br>";

            if (sk.ay < 0)
                hata += "Kesinti ayı boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
            {
                TABIKYSiteYonetimi tab = new TABIKYSiteYonetimi(ve);
                sonuc = tab.SilPersonelKesinti(sk);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";

                //    //Tarihçe Kaydet ********************
                //    yeni = p;
                //    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, ENUMTARIHCEISLEMTIP.SIL);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        private static SiteGider SiteGiderAl(ObjectArray oaSg, int giderKod)
        {
            if (oaSg != null)
            {
                foreach (SiteGider item in oaSg.objeler)
                {
                    if (item.giderKod == giderKod)
                        return item;
                }
            }
            return new SiteGider();
        }

        public static Sonuc PersonelKesintileriKaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection kisiler)
        {
            Sonuc sonuc = new Sonuc();
            if (kisiler.Count == 0)
            {
                sonuc.DegerAta("Lütfen veri giriniz");
                return sonuc;
            }
            PersonelLojmani k = (PersonelLojmani)kisiler[0];
            if (!TNS.IKY.Arac.DonemAcikMi(k.yil, k.ay))
                sonuc.hataStr = "Bulunduğunuz maaş yıl-ay dönemi içerisinde olmadığınızdan kayıt gerçekleştirilemedi.";

            if (k.kod <= 0)
                sonuc.hataStr += " Gider gireceğiniz personel dairesini seçmelisiniz.";

            //if (k.giderKod <= 0)
            //    sonuc.hataStr += " Gider gireceğiniz gider türünü seçmelisiniz.";

            if (k.yil <= 0)
                sonuc.hataStr += " Gider gireceğiniz yılı seçmelisiniz.";

            if (k.ay <= 0)
                sonuc.hataStr += " Gider gireceğiniz ayı seçmelisiniz.";

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYSiteYonetimi tabKesinti = new TABIKYSiteYonetimi(ve);
            ve.BeginTransaction();

            ObjectArray oa2 = IKYSiteYonetimi.ListeleSiteGider(ve, kullanan, new SiteGider { siteKod = k.siteKod, yil = k.yil, ay = k.ay });
            int say = 0;
            foreach (PersonelLojmani kes in kisiler)
            {
                say++;
                if (!sonuc.islemSonuc) break;
                sonuc = tabKesinti.SilPersonelKesinti(new PersonelSiteKesinti { pLojmanKod = kes.kod, yil = kes.yil, ay = kes.ay });
                if (!sonuc.islemSonuc) break;
                foreach (PersonelSiteKesinti sk in kes.pkesintiler)
                {
                    double oncEndeks = 0;
                    double oncOdeme = 0;
                    SiteGider sgIc = SiteGiderAl(oa2, sk.giderKod);
                    if (sk.endex > 0)
                    {
                        if (sk.elden > 0)
                        {
                            sk.tutar = sk.elden;
                        }
                        else
                        {
                            OncekiEndeksveOdemeBul(tabKesinti, kes, sk.giderKod, out oncEndeks, out oncOdeme);
                            sk.tutar = sgIc.miktar * (sk.endex - oncEndeks);
                        }
                    }
                    else if (sk.aktif == 1)
                    {
                        sk.tutar = sgIc.miktar;
                        sk.tutar += sk.elden;
                    }
                    if (sk.tutar < 0)
                    {
                        sonuc.DegerAta(string.Format("{0} kodlu gider kaydedilirken, {1}. satırda negatif ({2}) değer oluştu (Önc endeks:{3} Şimd endeks:{4}).", sk.giderKod, say, sk.tutar, oncEndeks, sk.endex));
                    }
                    else
                    {
                        sk.kod = TABGenel.SayacDegerAlInt(ve, TABIKYSiteYonetimi.TABLOKESINTI);
                        sonuc = tabKesinti.KaydetPersonelKesinti(sk);
                    }
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
            {
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static void OncekiEndeksveOdemeBul(TABIKYSiteYonetimi tabKesinti, PersonelLojmani kes, int giderKod, out double oncEndeks, out double oncOdeme)
        {
            int yil = kes.yil;
            int ay = kes.ay;
            ay--;
            if (ay < 1)
            {
                ay = 12;
                yil--;
            }
            oncEndeks = 0;
            oncOdeme = 0;
            IDataReader dr = tabKesinti.ListelePersonelKesinti(new PersonelSiteKesinti { pLojmanKod = kes.kod, yil = yil, ay = ay, giderKod = giderKod });
            if (dr != null)
            {
                while (dr.Read())
                {
                    double icEndeks = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.ENDEX, 0.0);
                    double icTutar = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.TUTAR, 0.0);
                    if (icEndeks > oncEndeks)
                    {
                        oncEndeks = icEndeks;
                        oncOdeme = icTutar;
                    }
                }
                dr.Close();
            }
        }
    }
}

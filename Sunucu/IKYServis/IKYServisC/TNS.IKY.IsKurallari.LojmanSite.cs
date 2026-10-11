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
    public class IKYLojmanSite
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, LojmanSite form)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            return false;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, LojmanSite form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// LojmanSite nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSite yapısında LojmanSite:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, LojmanSite form)
        {
            Sonuc s = new Sonuc(true, "", "");
            string kopruKod = IsKurallari.IKYServis.MinimumKopruKodGetir();

            IKYYetki.YetkiVarMiOGY(kullanan, kopruKod, 0, s, MUH.ENUMOGY.Yazma, ENUMYetkiIslem.LojmanTanim, false, null);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYLOJMANSITE TABIKYLOJMANSITE = new TABIKYLOJMANSITE(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.kod > 0)
                    s = TABIKYLOJMANSITE.Guncelle(form);
                else
                {
                    form.kod = TABGenel.SayacDegerAlInt(ve, TABIKYLOJMANSITE.TABLOAD);
                    s = TABIKYLOJMANSITE.Kaydet(form);
                }
            }

            //if (s.islemSonuc)
            //    s = TABIKYLOJMANSITE.Sil(form);

            //if (s.islemSonuc)
            //    s = TABIKYLOJMANSITE.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// IKYLOJMANSITE nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSite yapısında LojmanSite:  bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, LojmanSite form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form);
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
        public static bool GormeYetkisiVarMi(Kullanici kullanan, LojmanSite form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, LojmanSite form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// LojmanSite listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSite yapısında LojmanSite:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, LojmanSite form)
        {
            ObjectArray oa = new ObjectArray();

            string kopruKod = IsKurallari.IKYServis.MinimumKopruKodGetir();

            //oa.sonuc = IKYYetki.YetkiVarMiOGY(kullanan, kopruKod, 0, null, MUH.ENUMOGY.Gorme, ENUMYetkiIslem.LojmanTanim, false, null);
            //if (!oa.sonuc.islemSonuc)
            //{
            //    return oa;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYLOJMANSITE TABIKYLOJMANSITE = new TABIKYLOJMANSITE(ve);

            IDataReader dr;
            dr = TABIKYLOJMANSITE.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                LojmanSite detay = new LojmanSite();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.KOD, 0);
                detay.siteAdi = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.SITEADI, "");
                detay.mahalle = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.MAHALLE, "");
                detay.cadde = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.CADDE, "");
                detay.sokak = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.SOKAK, "");
                detay.semt = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.SEMT, "");
                detay.ilce = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.ILCE, "");
                detay.il = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.IL, "");
                detay.yakitKesilecek = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.YAKITKESILECEK, 0);
                detay.kesintiTipi = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.KESINTITIPI, "");
                detay.kesintiCevreGideri = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.KESINTICEVREGIDERI, 0);
                detay.kesintiSuGideri = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.KESINTISUGIDERI, 0);
                detay.kesintiDiger = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.KESINTIDIGER, 0);
                detay.kesintiDigerAciklama = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.KESINTIDIGERACIKLAMA, "");
                detay.kesintiOrtakGider = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITE.ENUMAlanlar.KESINTIORTAKGIDER, 0);
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static Sonuc Sil(Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc();
            //object eski = null, yeni = null; //Tarihçe için

            string kopruKod = IsKurallari.IKYServis.MinimumKopruKodGetir();

            IKYYetki.YetkiVarMiOGY(kullanan, kopruKod, 0, sonuc, MUH.ENUMOGY.Yazma, ENUMYetkiIslem.LojmanTanim, false, null);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            string hata = "";
            //if (p.kursKodu == 0)
            //    hata = "Kurs bilgisi boş bırakılamaz.<br>";
            if (kod < 0)
                hata += "Site kod boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
            {
                TABIKYLOJMANSITE tab = new TABIKYLOJMANSITE(ve);
                sonuc = tab.Sil(kod);
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

        public static ObjectArray SiteYetkilileriListele(VeriErisim veriErisim, Kullanici kullanan, IKYSiteYetki form)
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
            // string kopruKod = IsKurallari.IKYServis.MinimumKopruKodGetir();

            //oa.sonuc = IKYYetki.YetkiVarMiOGY(kullanan, kopruKod, null, ENUMYetkiIslem.Lojman);
            //pVAN.sonuc = IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, p.personelKod, null, ENUMYetkiIslem.Lojman);
            //if (!oa.sonuc.islemSonuc)
            //{
            //    return oa;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYLOJMANSITE TABIKYLOJMANSITE = new TABIKYLOJMANSITE(ve);

            IDataReader dr;
            dr = TABIKYLOJMANSITE.SiteYetkilileriListele(form.yetkiliOlduguSiteKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                IKYSiteYetki detay = new IKYSiteYetki();
                detay.kod = VeriErisim.DegerAl(dr, 0, 0);
                detay.pkod = VeriErisim.DegerAl(dr, 1, 0);
                detay.personelAd = VeriErisim.DegerAl(dr, 2, "");
                detay.oturduguSite = VeriErisim.DegerAl(dr, 3, "");
                detay.oturduguBlok = VeriErisim.DegerAl(dr, 4, "");
                detay.oturduguDaire = VeriErisim.DegerAl(dr, 5, 0);
                detay.oturduguPLKod = VeriErisim.DegerAl(dr, 6, 0);
                detay.yetkiliOlduguSiteKod = VeriErisim.DegerAl(dr, 7, 0);
                detay.yetkiliOlduguSiteAd = VeriErisim.DegerAl(dr, 8, "");
                detay.tip = VeriErisim.DegerAl(dr, 9, 0);
                detay.aktif = VeriErisim.DegerAl(dr, 10, 0);
                detay.tipAd = VeriErisim.DegerAl(dr, 11, "");
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static Sonuc SiteYetkiKaydet(Kullanici kullanan, IKYSiteYetki[] pList)
        {
            Sonuc sonuc = new Sonuc();

            //if (!YazmaYetkisiVarMi(kullanan, new PersonelIzin()))
            //{
            //    sonuc.DegerAta(PersonelResI.LojmanSite001);
            //    return sonuc;
            //}

            //sonuc = IKYYetki.YetkiVarMiYazmayaTC(kullanan, kullanan.mernis, sonuc, ENUMYetkiIslem.IzinDevir);
            ////sonuc = IKYYetki.YetkiVarMiYazmaya(kullanan, null, sonuc, ENUMYetkiIslem.IzinDevir);
            //if (!sonuc.islemSonuc)
            //{
            //    return sonuc;
            //}
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            string hata = "", hataK = "";
            //foreach (IKYSiteYetki p in pList)
            //{
            //    if (p.aktif == 1 && p.tip == 0)
            //        hata = "Yetkili olan site yöneticisinin yetki tipini girmelisiniz.<br>";               
            //}

            ve.BeginTransaction();
            TABIKYLOJMANSITE tab = new TABIKYLOJMANSITE(ve);

            foreach (IKYSiteYetki p in pList)
            {
                hataK = "";
                VarMi v = YetkiliKisiVarmi(ve, kullanan, p);
                if (!v.s.islemSonuc || v.varMi)
                {
                    if (v.s.islemSonuc)
                        hataK = p.pkod + " kodlu kişi aynı sitede zaten yetkili.</br>";
                    else
                        hataK = v.s.hataStr;
                }

                if (!string.IsNullOrWhiteSpace(hataK))
                {
                    hata += hataK;
                    continue;
                }

                if (p.kod > 0)
                    sonuc = tab.SiteYetkiGuncelle(p);
                else
                {
                    hataK = p.pkod + " kodlu kişinin yetki kaydı başarılı.</br>";
                    p.kod = TABGenel.SayacDegerAlInt(ve, TABIKYLOJMANSITE.TABLOADSITEYETKI);
                    sonuc = tab.SiteYetkiKaydet(p);
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = PersonelResI.PER007;
                }
                else
                    ve.Rollback();

                hata += hataK;
            }

            ve.VeriErisimKapat();

            if (hata != "")
            {
                sonuc.hataStr = hata;
                return sonuc;
            }

            return sonuc;
        }

        public static VarMi YetkiliKisiVarmi(VeriErisim veriErisim, Kullanici kullanan, IKYSiteYetki y)
        {
            VarMi varMi = new VarMi();
            varMi.s = new Sonuc();
            varMi.varMi = false;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYLOJMANSITE TAB = new TABIKYLOJMANSITE(ve);

            IDataReader dr;
            dr = TAB.YetkiliKisiVarmi(y);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                varMi.s.DegerAta(ve.hata);
                return varMi;
            }

            if (dr.Read())
            {
                varMi.varMi = true;
            }

            dr.Close();
            ve.VeriErisimKapat();
            return varMi;
        }

        public static Sonuc SiteYetkiAktifPasifYap(Kullanici kullanan, IKYSiteYetki p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!YazmaYetkisiVarMi(kullanan, new PersonelIzin()))
            //{
            //    sonuc.DegerAta(PersonelResI.LojmanSite001);
            //    return sonuc;
            //}

            //sonuc = IKYYetki.YetkiVarMiYazmayaTC(kullanan, kullanan.mernis, sonuc, ENUMYetkiIslem.IzinDevir);
            ////sonuc = IKYYetki.YetkiVarMiYazmaya(kullanan, null, sonuc, ENUMYetkiIslem.IzinDevir);
            //if (!sonuc.islemSonuc)
            //{
            //    return sonuc;
            //}


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABIKYLOJMANSITE tab = new TABIKYLOJMANSITE(ve);

            sonuc = tab.SiteYetkiAktifPasifYap(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

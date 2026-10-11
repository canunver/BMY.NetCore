using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
using TNS.MUH.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// MUHIstatistikKodGiris formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class MUHIstatistikKodGiris
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBUM";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, IstatistikKod form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, IstatistikKod form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// IstatistikKod nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IstatistikKod form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(ButceMuhasebeResI.IstatistikKodGiris001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHISTATISTIKKOD tabMUHISTATISTIKKOD = new TABMUHISTATISTIKKOD(ve);
            TABMUHISTATISTIKKODDEGER tabMUHISTATISTIKKODDEGER = new TABMUHISTATISTIKKODDEGER(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabMUHISTATISTIKKODDEGER.Sil(form);

            for (int i = 0; i < form.istatistikKodDeger.Count && s.islemSonuc; i++)
            {
                s = tabMUHISTATISTIKKODDEGER.Ekle(form, (IstatistikKodDeger)form.istatistikKodDeger[i]);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// IstatistikKod nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc TanimKaydet(VeriErisim veriErisim, Kullanici kullanan, IstatistikKod form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(ButceMuhasebeResI.IstatistikKodGiris001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHISTATISTIKKOD tabMUHISTATISTIKKOD = new TABMUHISTATISTIKKOD(ve);

            ve.BeginTransaction();

            if (form.kod == 0)
            {
                form.kod = tabMUHISTATISTIKKOD.KodBul();
                if (string.IsNullOrWhiteSpace(ve.hata))
                    s = tabMUHISTATISTIKKOD.Ekle(form);
                else
                    s.DegerAta(ve.hata);
            }
            else
                s = tabMUHISTATISTIKKOD.Guncelle(form);

            if (s.islemSonuc)
            {
                s.anahtar = form.kod.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// IstatistikKod nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        public static IstatistikKod Oku(VeriErisim veriErisim, Kullanici kullanan, IstatistikKod form, bool sifirEkle, bool eksibirEkle)
        {
            ObjectArray s = Listele(veriErisim, kullanan, form, sifirEkle, eksibirEkle, true);
            if (s == null || !s.sonuc.islemSonuc || s.ObjeSayisi == 0) return null;
            return (IstatistikKod)s.objeler[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, IstatistikKod form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, IstatistikKod form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// IstatistikKod listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IstatistikKod form, bool sifirEkle, bool eksibirEkle, bool detaylarOlsun)
        {
            ObjectArray alAna = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception(ButceMuhasebeResI.IstatistikKodGiris002);
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                throw new Exception(s.hataStr);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHISTATISTIKKOD tabMUHISTATISTIKKOD = new TABMUHISTATISTIKKOD(ve);
            TABMUHISTATISTIKKODDEGER tabMUHISTATISTIKKODDEGER = new TABMUHISTATISTIKKODDEGER(ve);

            IDataReader dr = tabMUHISTATISTIKKOD.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            while (dr.Read())
            {
                IstatistikKod detay = new IstatistikKod();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKOD.ENUMAlanlar.KOD, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKOD.ENUMAlanlar.AD, "");
                detay.zorunlu = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKOD.ENUMAlanlar.ZORUNLU, 0);
                detay.ekAlan1Ad = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKOD.ENUMAlanlar.EKALAN1AD, "");
                detay.ekAlan2Ad = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKOD.ENUMAlanlar.EKALAN2AD, "");
                detay.ekAlan3Ad = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKOD.ENUMAlanlar.EKALAN3AD, "");
                alAna.Ekle(detay);
            }
            dr.Close();
            if (detaylarOlsun)
            {
                foreach (IstatistikKod formIc in alAna.objeler)
                {
                    dr = tabMUHISTATISTIKKODDEGER.Oku(formIc);
                    if (dr == null)
                    {
                        ve.VeriErisimKapat();
                        throw new Exception(ve.hata);
                    }
                    if (eksibirEkle)
                        formIc.istatistikKodDeger.Add(new IstatistikKodDeger(-1, "Boş olanlar", "", "", ""));

                    if (sifirEkle)
                        formIc.istatistikKodDeger.Add(new IstatistikKodDeger());

                    while (dr.Read())
                    {
                        IstatistikKodDeger detay = new IstatistikKodDeger();
                        detay.degerKod = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKODDEGER.ENUMAlanlar.DEGERKOD, 0);
                        detay.aciklama = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKODDEGER.ENUMAlanlar.ACIKLAMA, "");
                        detay.ekAlan1 = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKODDEGER.ENUMAlanlar.EKALAN1, "");
                        detay.ekAlan2 = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKODDEGER.ENUMAlanlar.EKALAN2, "");
                        detay.ekAlan3 = VeriErisim.DegerAl(dr, (int)TABMUHISTATISTIKKODDEGER.ENUMAlanlar.EKALAN3, "");
                        formIc.istatistikKodDeger.Add(detay);
                    }
                    dr.Close();
                }
            }
            ve.VeriErisimKapat();
            return alAna;
        }

        /// <summary>
        /// IstatistikKod listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        public static IstatistikKodDeger DegerAdBul(VeriErisim veriErisim, Kullanici kullanan, int istKod, int degerKod, Hashtable ht)
        {
            if (ht != null)
            {
                try
                {
                    IstatistikKodDeger deger = (IstatistikKodDeger)ht[istKod.ToString() + "_" + degerKod.ToString()];
                    if (deger != null)
                        return deger;
                }
                catch
                {
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHISTATISTIKKODDEGER tab = new TABMUHISTATISTIKKODDEGER(ve);
            IstatistikKodDeger cevap = null;
            IDataReader dr = tab.DegerAdBul(istKod, degerKod);
            if (dr != null)
            {
                if (dr.Read())
                {
                    try
                    {
                        cevap = new IstatistikKodDeger();
                        cevap.degerKod = istKod;
                        cevap.aciklama = VeriErisim.DegerAl(dr, 0, "");
                        cevap.ekAlan1 = VeriErisim.DegerAl(dr, 1, "");
                        cevap.ekAlan2 = VeriErisim.DegerAl(dr, 2, "");
                        cevap.ekAlan3 = VeriErisim.DegerAl(dr, 3, "");
                    }
                    catch {}
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            if (ht != null)
            {
                ht[istKod.ToString() + "_" + degerKod.ToString()] = cevap;
            }
            return cevap;
        }

        internal static Sonuc ODEMIFIstKodAta(VeriErisim veriErisim, Kullanici kullanan, TNSCollection formlar, int ist1, int ist2)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            if (ist1 < 0 || ist2 < 0)
                hata = "<br>" + "Lütfen geçerli bir kod seçiniz!";

            foreach (OdemeEmriMIFForm form in formlar)
            {
                bool yetki = MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, form.kurum);
                if (!yetki)
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH054, form.belgeNo);
            }

            ve.BeginTransaction();
            if (hata == "")
            {
                TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
                foreach (OdemeEmriMIFForm form in formlar)
                {
                    Sonuc s = to.IstatistikKodAta(form, ist1, ist2);

                    if (!s.islemSonuc)
                    {
                        hata = s.hataStr;
                        break;
                    }
                }
            }

            if (hata != "")
            {
                ve.Rollback();
                sonuc.DegerAta(hata);
            }
            else
            {
                ve.Commit();
                sonuc.bilgiStr = "<br>" + ButceMuhasebeResI.MUH065;
                sonuc.islemSonuc = true;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}

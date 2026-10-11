using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;

namespace TNS.HRC.IsKurallari
{
    public class HRCGundelik
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        public static Sonuc Kaydet(Kullanici kullanan, Gundelik bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (bilgi.yil <= 1900)
                hata = "Yıl alanı boş bırakılamaz.<br>";

            //if (bilgi.unvanKod <= -1)
            //    hata = "Unvan alanı boş bırakılamaz.<br>";

            if (string.IsNullOrWhiteSpace(bilgi.ulkeKod))
                hata = "Ülke alanı boş bırakılamaz.<br>";

            if (bilgi.gundelikTutar <= 0)
                hata = "Gündelik tutar sıfıdan büyük olmalıdır.<br>";

            if (bilgi.paraBirimi.kod <= 0)
                hata = "Para birimi alanı boş bırakılamaz.<br>";

            if (bilgi.tur == 0)
                hata = "Tür alanı boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABGundelik to = new TABGundelik(ve);

            bool guncelle = false;

            if (bilgi.kod > 0)
            {
                ObjectArray eskiKayitlar = Listele(ve, kullanan, new Gundelik() { kod = bilgi.kod });

                if (eskiKayitlar.objeler.Count == 1)
                {
                    Gundelik eskiKayit = (Gundelik)eskiKayitlar[0];

                    if (eskiKayit.yil == bilgi.yil && eskiKayit.unvanKod == bilgi.unvanKod && eskiKayit.ulkeKod == bilgi.ulkeKod && (eskiKayit.tur == bilgi.tur || eskiKayit.tur == 0))
                        guncelle = true;
                }
            }

            ve.BeginTransaction();

            if (guncelle)
                sonuc = to.Guncelle(bilgi);
            else
            {
                bilgi.kod = OrtakFonksiyonlar.ConvertToInt(new TABGenel(ve).BelgeNoAl("HRCGUNDELIK", "0"), 0);
                sonuc = to.Kaydet(bilgi);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = bilgi.kod + "";
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(Kullanici kullanan, Gundelik bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (bilgi.kod == 0)
                sonuc.hataStr = "Silinecek kaydı seçiniz.<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABGundelik to = new TABGundelik(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Seçilen kayıt silinmiştir.<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Gundelik kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABGundelik to = new TABGundelik(ve);
            IDataReader r = to.Listele(kriter);

            if (r != null)
            {
                while (r.Read())
                {
                    int sonSira = (int)TABGundelik.ENUMAlanlar.TUR + 1;

                    Gundelik f = new Gundelik();

                    f.kod = VeriErisim.DegerAl(r, (int)TABGundelik.ENUMAlanlar.KOD, 0);
                    f.yil = VeriErisim.DegerAl(r, (int)TABGundelik.ENUMAlanlar.YIL, 0);
                    f.unvanKod = VeriErisim.DegerAl(r, (int)TABGundelik.ENUMAlanlar.UNVANKOD, 0);
                    f.ulkeKod = VeriErisim.DegerAl(r, (int)TABGundelik.ENUMAlanlar.COGRAFYAKOD, "");
                    f.gundelikTutar = VeriErisim.DegerAl(r, (int)TABGundelik.ENUMAlanlar.GUNDELIK, 0.0);
                    f.paraBirimi.kod = VeriErisim.DegerAl(r, (int)TABGundelik.ENUMAlanlar.PARABIRIMKOD, 0);
                    f.tur = (ENUMGundelikTur)VeriErisim.DegerAl(r, (int)TABGundelik.ENUMAlanlar.TUR, 0);

                    f.unvanAd = VeriErisim.DegerAl(r, sonSira++, "");
                    f.ulkeAd = VeriErisim.DegerAl(r, sonSira++, "");
                    f.paraBirimi.kisaltma = VeriErisim.DegerAl(r, sonSira++, "");

                    bilgi.objeler.Add(f);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return bilgi;
        }

        public static Sonuc SonrakiYilaKopyala(Kullanici kullanan, Gundelik bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (bilgi.yil <= 1900)
                hata = "Yıl alanı boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGundelik to = new TABGundelik(ve);
            ve.BeginTransaction();

            sonuc = to.SonrakiYilaKopyala(bilgi.yil);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        internal static ObjectVAN GeciciGorevYollukManasOku(VeriErisim veriErisim, Kullanici kullanan, GeciciGorevYollukManas form)
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
        public static bool GormeYetkisiVarMi(Kullanici kullanan, GeciciGorevYollukManas form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, GeciciGorevYollukManas form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// GeciciGorevYollukManas listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">GeciciGorevYollukManas yapısında GeciciGorevYollukManas:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, GeciciGorevYollukManas form)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            Surec surec =  TNS.HRC.IsKurallari.HRCSurec.SurecOku(veriErisim, kullanan, form.yil, form.surecNo);
            if(surec == null)
            {
                oa.sonuc.DegerAta("Süreç okunamadı!");
                ve.VeriErisimKapat();
                return oa;
            }

            form.muhasebe = surec.muhasebe;
            form.birim = surec.birim;
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                oa.sonuc.DegerAta("Bu formu görme yetkiniz yok!");
                ve.VeriErisimKapat();
                return oa;
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                oa.sonuc.DegerAta(s.hataStr);
                ve.VeriErisimKapat();
                return oa;
            }

            TABGundelik tab = new TABGundelik(ve);

            IDataReader dr = tab.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                GeciciGorevYollukManas detay = new GeciciGorevYollukManas();
                detay.yil = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.YIL, 0);
                detay.surecNo = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.SURECNO, "");
                detay.yi_yd = (ENUMYollukGundelikTuru)VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.YI_YD, 0);
                detay.kimlikNo = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.KIMLIKNO, "");
                detay.unvanKodu = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.UNVANKODU, 0);
                detay.gerekce = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.GEREKCE, "");
                detay.baslangicTarih = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.BASLANGICTARIH);
                detay.bitisTarih = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.BITISTARIH);
                detay.butceTertibi = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.BUTCETERTIBI, "");
                detay.avansTutari = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.AVANSTUTARI, 0.0);
                detay.avansOdemeEmri = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.AVANSODEMEEMRI, "");
                detay.odemeEmri = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.ODEMEEMRI, "");

                detay.unvan = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.DURUM + 1, "");
                detay.soyad = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.DURUM + 2, "");
                detay.ad = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.DURUM + 3, "");
                detay.durum = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.DURUM + 4, 0);
                detay.istihdamSekli = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.DURUM + 5, 0);

                detay.firmaAd = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.DURUM + 6, "");
                detay.firmaUyruk = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUK.DURUM + 7, 0);

                oa.Ekle(detay);
            }
            dr.Close();
            TNSCollection al;
            string kodyapi = DEG.Arac.KodYapiBul(form.yil, 20);
            foreach (GeciciGorevYollukManas formIc in oa.objeler)
            {
                dr = tab.ListeleDetay(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    GeciciGorevYollukManasDetay detay = new GeciciGorevYollukManasDetay();
                    detay.tur = (ENUMYollukIslemTur)VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUKDETAY.TUR, 0);
                    detay.gun = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUKDETAY.GUN, 0);
                    detay.gecelikKonaklamaUcreti = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUKDETAY.GECELIKKONAKLAMAUCRETI, 0.0);
                    detay.yerKod = OrtakFonksiyonlar.KodNoktaKoy(kodyapi, VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUKDETAY.YERKOD, ""));
                    detay.gundelik = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUKDETAY.GUNDELIK, 0.0);
                    detay.odenecek = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUKDETAY.ODENECEK, 0.0);
                    detay.aciklama = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUKDETAY.ACIKLAMA, "");
                    detay.yerAdSade = VeriErisim.DegerAl(dr, (int)TABGundelik.ENUMAlanlarYOLLUKDETAY.ACIKLAMA + 1, "");
                    al.Add(detay);
                }
                formIc.detay = al;
                dr.Close();
            }
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc GeciciGorevYollukManasOEIliskilendir(VeriErisim veriErisim, Kullanici kullanan, GeciciGorevYollukManas gelenForm, bool avans)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, gelenForm))
            {
                s.DegerAta("Yazma hakkınız yok!");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, gelenForm);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABGundelik tab = new TABGundelik(ve);
            GeciciGorevYollukManasHesaplaIc(tab, kullanan, gelenForm);

            ObjectVAN ov = GeciciGorevYollukManasOku(veriErisim, kullanan, gelenForm);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }
            GeciciGorevYollukManas okunan = null;
            if (ov.o == null)
            {
                ve.VeriErisimKapat();
                return new Sonuc("Kaydedilecek form okunamadı!");
            }

            okunan = (GeciciGorevYollukManas)ov.o;
            if (!YazmaYetkisiVarMi(kullanan, okunan))
            {
                s.DegerAta("Yazma hakkınız yok!");
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                s = tab.GuncelleOE(gelenForm, avans);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, GeciciGorevYollukManas form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, GeciciGorevYollukManas form)
        {
            return new Sonuc(true, "", "");
        }

        internal static Sonuc GeciciGorevYollukManasKaydet(VeriErisim veriErisim, Kullanici kullanan, GeciciGorevYollukManas gelenForm)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, gelenForm))
            {
                s.DegerAta("Yazma hakkınız yok!");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, gelenForm);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABGundelik tab = new TABGundelik(ve);
            GeciciGorevYollukManasHesaplaIc(tab, kullanan, gelenForm);

            if (!gelenForm.Kontrol(s))
            {
                ve.VeriErisimKapat();
                return s;
            }

            ObjectVAN ov = GeciciGorevYollukManasOku(veriErisim, kullanan, gelenForm);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }
            GeciciGorevYollukManas okunan = null;
            if (ov.o != null)
            {
                okunan = (GeciciGorevYollukManas)ov.o;
                if (!YazmaYetkisiVarMi(kullanan, okunan))
                {
                    s.DegerAta("Yazma hakkınız yok!");
                }
                if (okunan.durum == (int)TNS.MUH.ENUMBelgeDurumu.ONAYLI || okunan.durum == (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                {
                    s.DegerAta("İlişkili belge onaylı/yevmiyeli olduğundan formu değiştiremezsiniz!");
                }
            }
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();
            if (s.islemSonuc)
                s = tab.SilDetay(gelenForm);

            if (s.islemSonuc)
            {
                if (okunan == null)
                    s = tab.Ekle(gelenForm);
                else
                    s = tab.Guncelle(gelenForm);
            }
            for (int i = 0; i < gelenForm.detay.Count && s.islemSonuc; i++)
            {
                s = tab.Ekle(gelenForm, i);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static void GeciciGorevYollukManasHesapla(VeriErisim veriErisim, Kullanici kullanan, GeciciGorevYollukManas form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABGundelik tab = new TABGundelik(ve);
            GeciciGorevYollukManasHesaplaIc(tab, kullanan, form);
            ve.VeriErisimKapat();
        }

        internal static void GeciciGorevYollukManasHesaplaIc(TABGundelik tab, Kullanici kullanan, GeciciGorevYollukManas form)
        {
            for (int i = 0; i < form.detay.Count; i++)
            {
                GeciciGorevYollukManasDetay det = (GeciciGorevYollukManasDetay)form.detay[i];
                if (det.tur == ENUMYollukIslemTur.YOLLUK || det.tur == ENUMYollukIslemTur.KONAKLAMA || det.tur == ENUMYollukIslemTur.ZORUNLUMASRAF)
                {
                    if (det.tur == ENUMYollukIslemTur.ZORUNLUMASRAF)
                        det.gun = 1;
                    if (det.gundelik == 0 && !string.IsNullOrWhiteSpace(det.yerKod))
                        det.gundelik = tab.GundelikBul(form.yil, form.unvanKodu, det.yerKod.Split('.')[0], form.YollukTuru);
                }
            }
            form.OdenecekHesapla();
        }
    }
}

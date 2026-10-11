using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
using TNS.PEB.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// STREylemPlaniForm formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class STREylemPlaniForm
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriSTR";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, EylemPlani form)
        {
            return TNS.PEB.Arac.YetkinMi(kullanan);
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, EylemPlani form, Sonuc s)
        {
            if (form.eylemTuru == 1)
            {
                if (form.donemBYil == 0) s.DegerAta("Lütfen SP dönem yılını seçiniz!");
                if (form.amacKod == 0 || form.hedefKod == 0 || form.gostergeKod == 0 || string.IsNullOrWhiteSpace(form.kurumKod)) s.DegerAta("Lütfen SP göstergesi seçiniz!");
            }
            if (string.IsNullOrWhiteSpace(form.eylemAdi)) s.DegerAta("Lütfen eylem adı giriniz!");
            if (string.IsNullOrWhiteSpace(form.yurutenBirim)) s.DegerAta("Lütfen birim giriniz!");
            return s;
        }


        /// <summary>
        /// EylemPlani nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EylemPlani yapısında EylemPlani:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EylemPlani form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                return s;
            }

            AlanlarGecerliMi(kullanan, form, s);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tabSTREYLEMPLANI = new TABSTREYLEMPLANI(ve);

            ve.BeginTransaction();
            int anahtar = -1;
            if (form.eylemID <= 0)
            {
                anahtar = tabSTREYLEMPLANI.AnahtarBul();
                s.DegerAta(tabSTREYLEMPLANI.Hata());
            }
            if (s.islemSonuc)
            {
                if (anahtar > 0)
                {
                    form.eylemID = anahtar;
                    s = tabSTREYLEMPLANI.Ekle(form);
                }
                else
                {
                    s = tabSTREYLEMPLANI.Guncelle(form);
                    if (s.islemSonuc)
                        s = tabSTREYLEMPLANI.SilIlgiliBirimler(form);
                }
                if (s.islemSonuc)
                {
                    foreach (StrKodStrAd item in form.ilgiliBirimler)
                    {
                        s = tabSTREYLEMPLANI.EkleIlgiliBirim(form, item.kod);
                        if (!s.islemSonuc) break;
                    }
                }
            }
            s.anahtar = form.eylemID.ToString();
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            TarihceyeIsle(form.eylemID, kullanan, $"{form.eylemAdi} adlı eylem {(anahtar > 0 ? "eklendi" : "güncellendi")}");
            return s;
        }

        internal static Sonuc SilEylemPlaniForm(VeriErisim veriErisim, Kullanici kullanan, EylemPlani form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                return s;
            }

            if (form.eylemID <= 0)
            {
                s.DegerAta("Silinmesi için bir eylem vermelisiniz!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tabSTREYLEMPLANI = new TABSTREYLEMPLANI(ve);
            int hareketSay = tabSTREYLEMPLANI.HareketSayisiBul(form);
            if (hareketSay > 0)
            {
                s.DegerAta("Dönem verisi girilen eylemler silinemez!");
            }
            else
            {
                ve.BeginTransaction();
                s = tabSTREYLEMPLANI.SilIlgiliBirimler(form);
                if (s.islemSonuc)
                    s = tabSTREYLEMPLANI.SilYilBilgi(form);
                if (s.islemSonuc)
                    s = tabSTREYLEMPLANI.SilYetkiler(form);
                if (s.islemSonuc)
                    s = tabSTREYLEMPLANI.SilEylemDonem(form);
                if (s.islemSonuc)
                    s = tabSTREYLEMPLANI.SilForm(form);

                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            TarihceyeIsle(form.eylemID, kullanan, $"{form.eylemID} kodlu eylem silindi");
            return s;
        }

        private async static void TarihceyeIsle(int eylemID, Kullanici kullanan, string aciklama)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tabSTREYLEMPLANI = new TABSTREYLEMPLANI(ve);
            tabSTREYLEMPLANI.TarihceEkle(eylemID, aciklama, kullanan.kullaniciKodu);
            ve.VeriErisimKapat();
        }

        internal static Sonuc AgirlikGuncelle(VeriErisim veriErisim, Kullanici kullanan, int yil, int eylemID, int agirlik)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, null))
            {
                s.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                return s;
            }

            if (yil <= 0)
                s.DegerAta("Yıl bilgisi hatalı!");
            if (eylemID <= 0)
                s.DegerAta("Eylem kodu hatalı!");
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tabSTREYLEMPLANI = new TABSTREYLEMPLANI(ve);

            ve.BeginTransaction();
            s = tabSTREYLEMPLANI.SilYilAgirlik(eylemID, yil);
            if (s.islemSonuc)
            {
                s = tabSTREYLEMPLANI.EkleYilAgirlik(eylemID, yil, agirlik);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc KaydetEylemPlaniYetki(VeriErisim veriErisim, Kullanici kullanan, int eylemID, TNSCollection kisiler)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, null))
            {
                s.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                return s;
            }

            if (eylemID <= 0)
                s.DegerAta("Eylem kodu hatalı!");
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tabSTREYLEMPLANI = new TABSTREYLEMPLANI(ve);

            ve.BeginTransaction();
            s = tabSTREYLEMPLANI.SilYetki(eylemID);
            if (s.islemSonuc)
            {
                foreach (Kullanici kul in kisiler)
                {
                    s = tabSTREYLEMPLANI.EkleYetki(eylemID, kul.kullaniciKodu);
                    if (!s.islemSonuc) break;
                }
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectArray ListeleEylemPlaniYetki(VeriErisim veriErisim, Kullanici kullanan, int eylemID)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, null))
            {
                oa.sonuc.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                return oa;
            }

            if (eylemID <= 0)
                oa.sonuc.DegerAta("Eylem kodu hatalı!");
            if (!oa.sonuc.islemSonuc)
            {
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tab = new TABSTREYLEMPLANI(ve);

            IDataReader dr = tab.ListeleYetki(eylemID);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                Kullanici kul = new Kullanici();
                kul.kullaniciKodu = VeriErisim.DegerAl(dr, 1, "");
                kul.ad = VeriErisim.DegerAl(dr, 2, "");
                kul.soyad = VeriErisim.DegerAl(dr, 3, "");
                kul.EPOSTA = VeriErisim.DegerAl(dr, 4, "");
                kul.Telefon = VeriErisim.DegerAl(dr, 5, "");
                oa.Ekle(kul);
            }

            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray ListeleEylemPlaniDonem(VeriErisim veriErisim, Kullanici kullanan, int eylemTuru, int yil, int ay)
        {
            string KurKodYapi = KurKodYapiBul(yil);

            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, null))
            {
                oa.sonuc.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tab = new TABSTREYLEMPLANI(ve);

            IDataReader dr = tab.ListeleDonem(eylemTuru, yil, ay);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                EylemPlani detay = new EylemPlani();
                detay.eylemID = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.EYLEMID, 0);
                detay.eylemAdi = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.EYLEMADI, "");
                detay.eylemTuru = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.EYLEMTURU, 0);
                detay.yurutenBirim = OrtakFonksiyonlar.KodNoktaKoy(KurKodYapi, VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.YURUTENBIRIM, ""));
                detay.baslamaTarihi = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.BASLAMATARIHI);
                detay.bitisTarihi = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.BITISTARIHI);
                detay.donemBYil = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.DONEMBYIL, 0);
                detay.hedefKod = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.HEDEFKOD, 0);
                detay.kurumKod = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.KURUMKOD, "");
                detay.gostergeKod = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.GOSTERGEKOD, 0);
                detay.durum = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.DURUM, 0);
                int kolNo = (int)TABSTREYLEMPLANI.ENUMAlanlar.EYLEMAMAC + 1;
                detay.yurutenBirimKisaAd = VeriErisim.DegerAl(dr, kolNo++, "");
                detay.donemDurum = VeriErisim.DegerAl(dr, kolNo++, 0);
                oa.Ekle(detay);
            }

            ve.VeriErisimKapat();
            return oa;
        }

        private static string KurKodYapiBul(int yil)
        {
            return TNS.BKD.TertipYapi.KodYapiBul(yil, 1, false);
        }

        internal static Sonuc AyarlaEylemPlaniDonem(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, int eylemID, int donemDurum)
        {
            Sonuc sonuc = new Sonuc();
            if (!YazmaYetkisiVarMi(kullanan, null))
            {
                sonuc.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tab = new TABSTREYLEMPLANI(ve);

            sonuc = tab.DonemDurumSil(eylemID, yil, ay);
            if (donemDurum != 5 && sonuc.islemSonuc)
                sonuc = tab.DonemDurumEkle(eylemID, yil, ay, donemDurum);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            if (sonuc.islemSonuc)
                TarihceyeIsle(eylemID, kullanan, $"eylem durumu {yil}-{ay} için {donemDurum} yapıldı");
            ve.VeriErisimKapat();
            return sonuc;
        }
        /// <summary>
        /// EylemPlani nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="eylemID">Gerçekleşmesi okunacak eylemin IDsi</param>
        /// <param name="yil">Gerçekleşmesi okunacak dönemim yılı</param>
        /// <param name="ay">Gerçekleşmesi okunacak dönemim ayı</param>
        public static ObjectVAN OkuGerceklesme(VeriErisim veriErisim, Kullanici kullanan, int eylemID, int yil, int ay)
        {
            ObjectVAN ov = new ObjectVAN();

            if (eylemID == 0)
                ov.sonuc.DegerAta("Lüfen eylem seçiniz");
            if (yil == 0 || ay == 0)
                ov.sonuc.DegerAta("Lüfen yıl ve ay seçiniz");
            if (!ov.sonuc.islemSonuc) return ov;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tabSTREYLEMPLANI = new TABSTREYLEMPLANI(ve);

            IDataReader dr = tabSTREYLEMPLANI.EylemGerceklesmeOku(eylemID, yil, ay, kullanan.kullaniciKodu);
            ov.o = null;
            string okunanKullanici = "";
            if (dr == null)
            {
                ve.VeriErisimKapat();
                ov.sonuc.DegerAta(ve.hata);
                return ov;
            }
            if (dr.Read())
            {
                EylemPlani o = new EylemPlani();
                EylemGerceklesme g = new EylemGerceklesme();
                int kolNo = 0;
                o.eylemID = VeriErisim.DegerAl(dr, kolNo++, 0);
                o.eylemAdi = VeriErisim.DegerAl(dr, kolNo++, "");
                o.durum = VeriErisim.DegerAl(dr, kolNo++, 0);

                g.ay = ay;
                g.yil = VeriErisim.DegerAl(dr, kolNo++, 0);
                g.gerceklesme = VeriErisim.DegerAl(dr, kolNo++, 0);
                g.miktar = VeriErisim.DegerAl(dr, kolNo++, 0.0);
                g.planlanan = VeriErisim.DegerAl(dr, kolNo++, "");
                g.gerceklesen = VeriErisim.DegerAl(dr, kolNo++, "");
                g.sorunlar = VeriErisim.DegerAl(dr, kolNo++, "");
                g.aciklamalar = VeriErisim.DegerAl(dr, kolNo++, "");
                o.donemDurum = VeriErisim.DegerAl(dr, kolNo++, 0);
                okunanKullanici = VeriErisim.DegerAl(dr, kolNo++, "");
                o.eylemGerceklesme.Add(g);
                ov.o = o;
            }
            else
                ov.sonuc.DegerAta("Aranan eylem bulunamadı!");
            dr.Close();
            if (ov.o != null)
            {
                if (!TNS.PEB.Arac.YetkinMi(kullanan))
                {
                    if (kullanan.kullaniciKodu != okunanKullanici)
                    {
                        ov.o = null;
                        ov.sonuc.DegerAta("İstenilen eylemle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                    }
                    else
                    {
                        if (((EylemPlani)ov.o).donemDurum != 1)
                            ((EylemPlani)ov.o).donemDurum = 0;
                    }
                }
            }
            ve.VeriErisimKapat();
            return ov;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, EylemPlani form)
        {
            return TNS.PEB.Arac.YetkinMi(kullanan);
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, EylemPlani form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// EylemPlani listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EylemPlani yapısında EylemPlani:  bilgilerini tutan değişken</param>
        /// <param name="yil">Verisi listelenecek yıl</param>
        /// <param name="donemBasYil">Stratejik Plan Dönemi</param>
        /// <param name="ay">Verisi listelenecek ay</param>
        /// <param name="detayli">Eylemlere ait gerçekleşme bilgileri ve ilgili birimler de okunur</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EylemPlani form, int yil, int ay, int donemBasYil, bool detayli)
        {
            ObjectArray oa = new ObjectArray();

            if (yil < donemBasYil || yil > donemBasYil + 4)
            {
                oa.sonuc.DegerAta($"Görmek istediğiniz yıl ({yil}), stratejik plan({donemBasYil}-{donemBasYil + 4}) dönemi dışında!");
                return oa;
            }

            string KurKodYapi = KurKodYapiBul(yil);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tabSTREYLEMPLANI = new TABSTREYLEMPLANI(ve);

            IDataReader dr = tabSTREYLEMPLANI.Listele(form, yil, ay, donemBasYil, (TNS.PEB.Arac.YetkinMi(kullanan) ? null : kullanan.kullaniciKodu));
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                EylemPlani detay = new EylemPlani();
                detay.eylemID = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.EYLEMID, 0);
                detay.eylemAdi = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.EYLEMADI, "");
                detay.eylemTuru = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.EYLEMTURU, 0);
                detay.yurutenBirim = OrtakFonksiyonlar.KodNoktaKoy(KurKodYapi, VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.YURUTENBIRIM, ""));
                detay.baslamaTarihi = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.BASLAMATARIHI);
                detay.bitisTarihi = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.BITISTARIHI);
                detay.donemBYil = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.DONEMBYIL, 0);
                detay.hedefKod = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.HEDEFKOD, 0);
                detay.kurumKod = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.KURUMKOD, "");
                detay.gostergeKod = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.GOSTERGEKOD, 0);
                detay.durum = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.DURUM, 0);
                detay.eylemAmac = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlar.EYLEMAMAC, "");
                int kolNo = (int)TABSTREYLEMPLANI.ENUMAlanlar.EYLEMAMAC + 1;
                detay.yurutenBirimAd = VeriErisim.DegerAl(dr, kolNo++, "");
                detay.yurutenBirimKisaAd = VeriErisim.DegerAl(dr, kolNo++, "");
                detay.agirlik = VeriErisim.DegerAl(dr, kolNo++, 0);
                detay.gerceklesme = VeriErisim.DegerAl(dr, kolNo++, 0);
                detay.miktar = VeriErisim.DegerAl(dr, kolNo++, 0.0);

                detay.hedefKisa = VeriErisim.DegerAl(dr, kolNo++, "");
                detay.hedefAd = VeriErisim.DegerAl(dr, kolNo++, "");

                detay.amacKod = VeriErisim.DegerAl(dr, kolNo++, 0);
                detay.amacKisa = VeriErisim.DegerAl(dr, kolNo++, "");
                detay.amacAd = VeriErisim.DegerAl(dr, kolNo++, "");

                detay.gostergeKisa = VeriErisim.DegerAl(dr, kolNo++, "");
                detay.gostergeAd = VeriErisim.DegerAl(dr, kolNo++, "");

                detay.hedefBirimAd = VeriErisim.DegerAl(dr, kolNo++, "");
                detay.hedefBirimKisaAd = VeriErisim.DegerAl(dr, kolNo++, "");
                detay.gostergeBaslangic = VeriErisim.DegerAl(dr, kolNo++, 0.0);
                detay.gostergeTur = VeriErisim.DegerAl(dr, kolNo++, 0);
                detay.yilTahmin = VeriErisim.DegerAl(dr, kolNo++, 0.0);
                detay.olcuBirimAd = VeriErisim.DegerAl(dr, kolNo++, "");
                oa.Ekle(detay);
            }
            dr.Close();
            if (detayli && oa.sonuc.islemSonuc && oa.objeler.Count > 0)
            {
                dr = tabSTREYLEMPLANI.ListeleGerceklesme(donemBasYil, oa.objeler);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                EylemPlani simdikiEylem = null;
                while (dr.Read())
                {
                    int eylemID = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarGERCEKLESME.EYLEMID, 0);
                    if (simdikiEylem == null || simdikiEylem.eylemID != eylemID) simdikiEylem = EylemBul(oa.objeler, eylemID);
                    if (simdikiEylem != null)
                    {
                        EylemGerceklesme detay = new EylemGerceklesme();
                        detay.yil = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarGERCEKLESME.YIL, 0);
                        detay.ay = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarGERCEKLESME.AY, 0);
                        detay.olcuBirimAd = simdikiEylem.olcuBirimAd;
                        detay.gerceklesme = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarGERCEKLESME.GERCEKLESME, 0.0);
                        detay.miktar = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarGERCEKLESME.MIKTAR, 0);
                        detay.planlanan = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarGERCEKLESME.PLANLANAN, "");
                        detay.gerceklesen = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarGERCEKLESME.GERCEKLESEN, "");
                        detay.sorunlar = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarGERCEKLESME.SORUNLAR, "");
                        detay.aciklamalar = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarGERCEKLESME.ACIKLAMALAR, "");
                        simdikiEylem.eylemGerceklesme.Add(detay);
                    }
                }
                dr.Close();

                dr = tabSTREYLEMPLANI.ListeleIlgiliBirimler(yil, oa.objeler);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                simdikiEylem = null;
                while (dr.Read())
                {
                    int eylemID = VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarILGILIBIRIMLER.EYLEMID, 0);
                    if (simdikiEylem == null || simdikiEylem.eylemID != eylemID) simdikiEylem = EylemBul(oa.objeler, eylemID);
                    if (simdikiEylem != null)
                    {
                        StrKodStrAd detay = new StrKodStrAd();
                        detay.kod = OrtakFonksiyonlar.KodNoktaKoy(KurKodYapi, VeriErisim.DegerAl(dr, (int)TABSTREYLEMPLANI.ENUMAlanlarILGILIBIRIMLER.KURKOD, ""));
                        detay.ad = VeriErisim.DegerAl(dr, 2, "");
                        detay.kisaAd = VeriErisim.DegerAl(dr, 3, "");
                        simdikiEylem.ilgiliBirimler.Add(detay);
                    }
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return oa;
        }

        private static EylemPlani EylemBul(TNSCollection eylemler, int eylemID)
        {
            foreach (EylemPlani item in eylemler)
            {
                if (item.eylemID == eylemID) return item;
            }
            return null;
        }

        internal static Sonuc KaydetEylemPlaniGerceklesme(VeriErisim veriErisim, Kullanici kullanan, int eylemID, EylemGerceklesme form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ov = OkuGerceklesme(ve, kullanan, eylemID, form.yil, form.ay);
            EylemPlani ep = (EylemPlani)ov.o;

            if (ov.sonuc.islemSonuc)
            {
                if (ep.durum != 1)
                {
                    ov.sonuc.DegerAta("Eylem kartı kapalı olduğundan, dönem bilgisi kaydedilemez!");
                }

                if (ep.donemDurum != 1)
                {
                    ov.sonuc.DegerAta("Dönem kapalı olduğundan, dönem bilgisi kaydedilemez!");
                }
            }

            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }

            TABSTREYLEMPLANI tabSTREYLEMPLANI = new TABSTREYLEMPLANI(ve);
            ve.BeginTransaction();
            try
            {
                EylemGerceklesme eg = (EylemGerceklesme)ep.eylemGerceklesme[0];
                if (eg.yil != 0)
                    ov.sonuc = tabSTREYLEMPLANI.GuncelleGerceklesme(eylemID, form);
                else
                    ov.sonuc = tabSTREYLEMPLANI.EkleGerceklesme(eylemID, form);
            }
            catch (Exception ex)
            {
                ov.sonuc.DegerAta("Kayıt işleminde beklenmedi hata oluştu:" + ex.Message);
            }
            if (ov.sonuc.islemSonuc)
            {
                ve.Commit();
                TarihceyeIsle(eylemID, kullanan, $"{form.yil}/{form.ay}-{form.ayAd} dönemi gerçekleşme bilgisi kaydedildi.");
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return ov.sonuc;
        }

        internal static ObjectArray ListeleEylemPlaniTarihce(VeriErisim veriErisim, Kullanici kullanan, int eylemID)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, null))
            {
                oa.sonuc.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTREYLEMPLANI tab = new TABSTREYLEMPLANI(ve);

            IDataReader dr = tab.ListeleTarihce(eylemID);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                EylemTarihce detay = new EylemTarihce();
                detay.eylemID = VeriErisim.DegerAl(dr, 0, 0);
                detay.tarih = VeriErisim.DegerAl(dr, 1);
                detay.aciklama = VeriErisim.DegerAl(dr, 2, "");
                detay.kullaniciKodu = VeriErisim.DegerAl(dr, 3, "");
                detay.ad = VeriErisim.DegerAl(dr, 4, "");
                detay.soyad = VeriErisim.DegerAl(dr, 5, "");
                detay.telefon = VeriErisim.DegerAl(dr, 6, "");
                oa.Ekle(detay);
            }

            ve.VeriErisimKapat();
            return oa;
        }
    }
}

using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
using TNS.MUH.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.DEG;
using TNS.HRC;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// MUHMUHBANKAODELISTE formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BankaOdemeListe
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBUM";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, MUHBankaOdeme form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, MUHBankaOdeme form)
        {
            string hataStr = "";
            if (form.gun == null || form.gun.isNull)
                hataStr = "Gün alan boş olamaz!";
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrEmpty(hataStr))
                s.DegerAta(hataStr);
            return s;
        }

        /// <summary>
        /// MUHBankaOdeme nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MUHBankaOdeme yapısında MUHBankaOdeme: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            form.gun = new TNSDateTime(form.gun.ToString());

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBANKAODEMEDETAY tab = new TABMUHBANKAODEMEDETAY(ve);

            ve.BeginTransaction();
            string formAnahtar;
            if (form.siraNo == 0)
            {
                s = Ekle(ve, kullanan, form.saymanlik, form.birim, form.gun, form.aciklama, form.muhYetkili, form.muhYetUnvan, form.onaylayan, form.onayUnvan);
                formAnahtar = s.anahtar;
                form.siraNo = OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0);
            }
            else
            {
                formAnahtar = form.siraNo.ToString();
                s = Guncelle(ve, kullanan, form);
            }

            if (s.islemSonuc)
                s = tab.Sil(form);

            for (int i = 0; i < form.muhBankaOdemeDetay.Length && s.islemSonuc; i++)
                s = tab.Ekle(form, i + 1, form.muhBankaOdemeDetay[i]);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            s.anahtar = formAnahtar;
            return s;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, MUHBankaOdeme form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, MUHBankaOdeme form)
        {
            return new Sonuc(true, "", "");
        }

        public static void DRdenForma(IDataReader dr, MUHBankaOdeme form)
        {
            form.saymanlik = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.SAYMANLIK, 0);
            form.birim = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.BIRIM, "");
            form.gun = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.GUN);
            form.siraNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.SIRANO, 0);
            form.aciklama = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ACIKLAMA, "");
            form.kullaniciKod = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.KULKOD, "");
            form.olusmaZamani = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.OLUSTARIH);
            form.degistirenKod = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.DEGKOD, "");
            form.degismeZamani = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.DEGTARIH);
            form.onaylayanKod = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ONAYKOD, "");
            form.onayZamani = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ONAYTARIH);
            form.hazirlayan = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.HAZIRLAYAN, "");
            form.odemeTarih = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ODEMETAR);
            form.muhYetkili = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.MUHYETKILI, "");
            form.muhYetUnvan = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.MUHYETUNV, "");
            form.onaylayan = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ONAYYETKILI, "");
            form.onayUnvan = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ONAYYETUNV, "");
            form.tur = (ENUMBankaListesiTur)VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.TUR, 0);
            form.durum = (ENUMBankaListesiDurum)VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.DURUM, 0);

            form.bankaDosyaAd = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.BANKADOSYAAD, "");
            form.cekEmriNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.CEKEMRINO, "");
            form.donemNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.DONEMNO, 0);
        }


        /// <summary>
        /// MUHBankaOdeme nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MUHBankaOdeme yapısında MUHBankaOdeme: bilgilerini tutan değişken</param>
        public static MUHBankaOdeme Oku(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme form)
        {
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception("Görme yetkiniz yok!");
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                throw new Exception(s.hataStr);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBANKAODEMEANA tabMUHBANKAODEMEANA = new TABMUHBANKAODEMEANA(ve);
            TABMUHBANKAODEMEDETAY tabMUHBANKAODEMEDETAY = new TABMUHBANKAODEMEDETAY(ve);

            MUHBankaOdeme formIc = new MUHBankaOdeme();
            IDataReader dr;
            dr = tabMUHBANKAODEMEANA.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            if (dr.Read())
            {
                DRdenForma(dr, formIc);
            }
            else
            {
                dr.Close();
                ve.VeriErisimKapat();
                throw new Exception("Aranan Kayıt bulunamadı!");
            }
            dr.Close();
            ArrayList al;
            dr = tabMUHBANKAODEMEDETAY.Oku(form.saymanlik, form.birim, form.gun, form.siraNo, 0);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            al = new ArrayList();
            while (dr.Read())
            {
                MUHBankaOdemeDetay detay = new MUHBankaOdemeDetay();
                DetayDoldur(detay, dr);
                al.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            if (form.grupOku == "1")
            {
                for (int i = 0; i < al.Count; i++)
                {
                    MUHBankaOdemeDetay ana = (MUHBankaOdemeDetay)al[i];
                    for (int j = i + 1; j < al.Count; j++)
                    {
                        MUHBankaOdemeDetay ilgiliAyni = (MUHBankaOdemeDetay)al[j];
                        if (ana.IlgiliAyniMi(ilgiliAyni))
                        {
                            ana.Birlestir(ilgiliAyni);
                            al.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            formIc.muhBankaOdemeDetay = (MUHBankaOdemeDetay[])al.ToArray(typeof(MUHBankaOdemeDetay));
            return formIc;
        }

        private static void DetayDoldur(MUHBankaOdemeDetay detay, IDataReader dr)
        {
            detay.sira = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.SIRA, 0);
            detay.mifYil = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.MIFYIL, 0);
            detay.mifBelgeNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.MIFBELGENO, "");
            detay.mifSira = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.MIFSIRA, 0);
            detay.muhHesapKodu = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.MUHHESAPKOD, "");
            detay.sozlesmeNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.SOZLESMENO, "");
            detay.ilgili = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.ILGILI, "");
            detay.tc_Vno = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.TC_VNO, "");
            detay.ilgBanka = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.ILGILIBANKA, "");
            detay.ilgIBAN = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.ILGILIIBAN, "");
            detay.tutar = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.TUTAR, 0.0);
            detay.doviz = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.DOVIZ, "");
            detay.aciklama = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.ACIKLAMA, "");
            detay.durum = (ENUMBankaListesiDetayDurum)VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.DURUM, 0);
            detay.dekontNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.DEKONTNO, "");
            detay.odemeEmriNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.ODEMEEMRINO, "");
            detay.cekNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.CEKNO, "");
            detay.mifYevmiyeNo = VeriErisim.DegerAl(dr, ((int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.CEKNO) + 1, 0);
            detay.odemeEmriIslemTarih = VeriErisim.DegerAl(dr, ((int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.CEKNO) + 2);
        }

        internal static Sonuc Ekle(VeriErisim veriErisim, Kullanici kullanan, int saymanlik, string birim, TNSDateTime dt, string aciklama, string muhYetkili, string muhYetUnvan, string onaylayan, string onayUnvan)
        {
            if (dt == null || dt.isNull || dt.Oku() == DateTime.MinValue) return new Sonuc(false, "Geçerli bir tarih giriniz!", "");
            if (string.IsNullOrWhiteSpace(aciklama)) return new Sonuc(false, "Geçerli bir açıklama giriniz!", "");
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);

            TNSDateTime d = dt;

            int siraNo = tab.SiraNoBul(saymanlik, birim, d) + 1;
            ve.BeginTransaction();

            s = tab.Ekle(kullanan, saymanlik, birim, d, siraNo, aciklama, muhYetkili, muhYetUnvan, onaylayan, onayUnvan);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            s.anahtar = siraNo.ToString();
            return s;
        }

        internal static Sonuc Guncelle(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme bilgi)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            MUHBankaOdeme eski = Oku(ve, kullanan, bilgi);
            if (eski == null)
            {
                s.DegerAta("Güncellenecek form bulunamadı!");
                ve.VeriErisimKapat();
                return s;
            }

            if (eski.durum == ENUMBankaListesiDurum.Onayli)
            {
                s.DegerAta("Belge onaylı olduğu için değiştirilemez!");
                ve.VeriErisimKapat();
                return s;
            }

            if (eski.tur == ENUMBankaListesiTur.HarcamaSurecUretti)
            {
                s.DegerAta("Form süreçten üretildiği için değiştirilemez!");
                ve.VeriErisimKapat();
                return s;
            }

            TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);


            ve.BeginTransaction();

            s = tab.Guncelle(kullanan, bilgi);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme bilgi)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            MUHBankaOdeme eski = Oku(ve, kullanan, bilgi);
            if (eski == null)
            {
                s.DegerAta("Silinecek form bulunamadı!");
                ve.VeriErisimKapat();
                return s;
            }

            if (eski.durum == ENUMBankaListesiDurum.Onayli)
            {
                s.DegerAta("Belge onaylı olduğu için silinemez!");
                ve.VeriErisimKapat();
                return s;
            }

            if (eski.tur == ENUMBankaListesiTur.HarcamaSurecUretti)
            {
                s.DegerAta("Form süreçten üretildiği için silinemez!");
                ve.VeriErisimKapat();
                return s;
            }

            if (eski.muhBankaOdemeDetay.Length > 0)
            {
                s.DegerAta("Form içinde kayıtlı ödeme emirleri olduğu için için silinemez!");
                ve.VeriErisimKapat();
                return s;
            }

            TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);

            ve.BeginTransaction();

            s = tab.Sil(kullanan, bilgi);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc OEEkle(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay detay)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            MUHBankaOdeme eski = Oku(ve, kullanan, bilgi);
            if (eski == null)
            {
                s.DegerAta("Güncellenecek form bulunamadı!");
            }

            if (eski.durum == ENUMBankaListesiDurum.Onayli)
            {
                s.DegerAta("Belge onaylı olduğu için değiştirilemez!");
            }

            if (eski.tur == ENUMBankaListesiTur.HarcamaSurecUretti)
            {
                s.DegerAta("Form süreçten üretildiği için değiştirilemez!");
            }

            if (s.islemSonuc)
            {
                OdemeEmriMIFForm eklenecekMIF = new MUHOdemeEmriMIFServis().BelgeKimlikOku(ve, detay.mifYil, detay.mifBelgeNo);

                //DebugYaz($"detay.mifYil:{detay.mifYil}, detay.mifBelgeNo:{detay.mifBelgeNo}, eklenecekMIF.muhasebe:{eklenecekMIF.muhasebe}, eski.saymanlik:{eski.saymanlik}, eklenecekMIF.birim:{eklenecekMIF.birim}, eski.birim:{eski.birim}");

                if (eklenecekMIF.formDurum != (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                {
                    s.DegerAta("Banka listesine eklenenen belgeler yevmiyeli olmalıdır!");
                }

                if (eklenecekMIF.muhasebe != eski.saymanlik)
                {
                    s.DegerAta("Banka listesine eklenenen belgeler, banka listesi ile aynı saymanlığa ait olmalıdır!");
                }

                if (eklenecekMIF.birim != eski.birim && eski.birim != "000")
                {
                    s.DegerAta("Banka listesine eklenenen belgeler, banka listesi ile aynı birime ait olmalıdır!");
                }
            }

            if (s.islemSonuc)
            {
                TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);
                TABMUHBANKAODEMEDETAY tabDetay = new TABMUHBANKAODEMEDETAY(ve);

                ve.BeginTransaction();

                s = tab.Degisti(kullanan, bilgi);
                int sira = 0;
                if (s.islemSonuc)
                {
                    sira = tabDetay.SiraNoBul(bilgi) + 1;
                    s = tabDetay.Ekle(bilgi, sira, detay);
                }

                if (s.islemSonuc)
                {
                    ve.Commit();
                    s.anahtar = sira.ToString();
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc BasilanCekNoAta(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay detay)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            MUHBankaOdeme eski = Oku(ve, kullanan, bilgi);
            if (eski == null)
            {
                s.DegerAta("Güncellenecek form bulunamadı!");
                eski = new MUHBankaOdeme();
            }

            if (eski.durum == ENUMBankaListesiDurum.Onayli)
            {
                s.DegerAta("Belge onaylı olduğu için değiştirilemez!");
            }

            if (!string.IsNullOrWhiteSpace(detay.cekNo))
            {
                long cekNo = OrtakFonksiyonlar.ConvertToLong(detay.cekNo);
                if (cekNo == 0)
                    s.DegerAta("Çek numarası sayılardan oluşmalı");
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABGenel tg = new TABGenel(ve);
            TABMUHBANKAODEMEDETAY tabDetay = new TABMUHBANKAODEMEDETAY(ve);

            ve.BeginTransaction();

            if (string.IsNullOrWhiteSpace(detay.cekNo))
            {
                detay.cekNo = tg.BelgeNoAl(detay.muhHesapKodu, "0000000000");
            }
            else
            {
                s = tg.SayacNoGuncelle(detay.muhHesapKodu, detay.cekNo, "0000000000");
                detay.cekNo = s.anahtar;
            }
            if (s.islemSonuc)
                s = tabDetay.CekNoGuncelle(bilgi, detay);
            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = detay.cekNo;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc OESil(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme bilgi)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);
            TABMUHBANKAODEMEDETAY tabDetay = new TABMUHBANKAODEMEDETAY(ve);

            ve.BeginTransaction();

            s = tab.Degisti(kullanan, bilgi);
            if (s.islemSonuc)
            {
                for (int i = 0; i < bilgi.muhBankaOdemeDetay.Length; i++)
                {
                    s = tabDetay.SilOE(bilgi, bilgi.muhBankaOdemeDetay[i]);
                }
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc Onayla(VeriErisim veriErisim, Kullanici kullanan, int saymanlik, string birim, TNSDateTime gun, int siraNo, ENUMBankaListesiDurum onayKod, int donemNo)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);

            ve.BeginTransaction();

            s = tab.Onayla(kullanan, saymanlik, birim, gun, siraNo, onayKod);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            if (s.islemSonuc && donemNo == 0 && onayKod == ENUMBankaListesiDurum.Onayli)
            {
                TABGenel tg = new TABGenel(ve);
                int sayac = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("BANKALISTE"+gun.Yil.ToString("0000") + gun.Ay.ToString("00"), "000000"), 0);
                Sonuc sayacSonuc = tab.DonemNoGuncelle(saymanlik, birim, gun, siraNo, sayac);
                if (!sayacSonuc.islemSonuc)
                {
                    s.DegerAta("Belge onaylandı ama sayaç sayaç yazılamadı!");
                    sayac = 0;
                }
                donemNo = sayac;

            }
            s.anahtar = donemNo.ToString();
            ve.VeriErisimKapat()
                ;
            return s;
        }

        /// <summary>
        /// MUHBankaOdeme nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MUHBankaOdeme yapısında MUHBankaOdeme: bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int saymanlikKod, string birim, TNSDateTime basla, TNSDateTime bitis, string araMetin)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);
            ObjectArray oa = new ObjectArray();
            IDataReader dr = tab.Listele(saymanlikKod, birim, basla, bitis, araMetin);
            if (dr == null)
            {
                oa.sonuc = new Sonuc(false, ve.hata, "");
                ve.VeriErisimKapat();
                return oa;
            }
            while (dr.Read())
            {
                MUHBankaOdeme formIc = new MUHBankaOdeme();
                formIc.saymanlik = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.SAYMANLIK, 0);
                formIc.birim = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.BIRIM, "");

                formIc.gun = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.GUN);
                formIc.siraNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.SIRANO, 0);
                formIc.aciklama = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ACIKLAMA, "");
                formIc.kullaniciKod = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.KULKOD, "");
                formIc.olusmaZamani = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.OLUSTARIH);
                formIc.degistirenKod = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.DEGKOD, "");
                formIc.degismeZamani = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.DEGTARIH);
                formIc.onaylayanKod = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ONAYKOD, "");
                formIc.onayZamani = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ONAYTARIH);
                formIc.hazirlayan = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.HAZIRLAYAN, "");
                formIc.odemeTarih = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ODEMETAR);
                formIc.muhYetkili = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.MUHYETKILI, "");
                formIc.muhYetUnvan = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.MUHYETUNV, "");
                formIc.onaylayan = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ONAYYETKILI, "");
                formIc.onayUnvan = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.ONAYYETUNV, "");
                formIc.tur = (ENUMBankaListesiTur)VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.TUR, 0);
                formIc.durum = (ENUMBankaListesiDurum)VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.DURUM, 0);

                formIc.bankaDosyaAd = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.BANKADOSYAAD, "");
                formIc.cekEmriNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.CEKEMRINO, "");
                formIc.donemNo = VeriErisim.DegerAl(dr, (int)TABMUHBANKAODEMEANA.ENUMAlanlar.DONEMNO, 0);
                oa.Ekle(formIc);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static ObjectArray OdenecekEmirListele(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlikKod, string birim, TNSDateTime yevBasTar, TNSDateTime yevBitTar, string cekNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBANKAODEMEDETAY tab = new TABMUHBANKAODEMEDETAY(ve);
            ObjectArray oa = new ObjectArray();
            //DebugYaz($"yil:{yil}, saymanlikKod:{saymanlikKod}, birim:{birim}");
            IDataReader dr = tab.OdenecekGondermeListele(saymanlikKod, birim, yil, true, yevBasTar, yevBitTar, cekNo);
            if (dr == null)
            {
                oa.sonuc = new Sonuc(false, ve.hata, "");
                ve.VeriErisimKapat();
                return oa;
            }
            string bankasizVNolar = "";
            string kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();

            while (dr.Read())
            {
                MUHBankaOdemeDetay formIc = new MUHBankaOdemeDetay();
                formIc.mifYil = VeriErisim.DegerAl(dr, 0, 0);
                formIc.mifBelgeNo = VeriErisim.DegerAl(dr, 1, "");
                formIc.mifSira = VeriErisim.DegerAl(dr, 2, 0);
                formIc.muhHesapKodu = VeriErisim.DegerAl(dr, 3, "");
                if (formIc.muhHesapKodu.StartsWith("103"))
                {
                    formIc.tutar = VeriErisim.DegerAl(dr, 5, (double)0);
                }
                else
                {
                    formIc.doviz = VeriErisim.DegerAl(dr, 4, "");
                    formIc.tutar = VeriErisim.DegerAl(dr, 6, (double)0);
                    if (string.IsNullOrWhiteSpace(formIc.doviz)) formIc.doviz = "HATA";
                }
                formIc.ilgili = VeriErisim.DegerAl(dr, 7, "");
                formIc.tc_Vno = VeriErisim.DegerAl(dr, 8, "");
                formIc.ilgBanka = VeriErisim.DegerAl(dr, 9, "");
                formIc.ilgIBAN = VeriErisim.DegerAl(dr, 10, "");
                formIc.aciklama = VeriErisim.DegerAl(dr, 11, "");
                string altHesapKod = VeriErisim.DegerAl(dr, 13, "");
                string yevmiyeAciklama = VeriErisim.DegerAl(dr, 14, "");
                formIc.birimKod = VeriErisim.DegerAl(dr, 15, "");
                if (!string.IsNullOrWhiteSpace(yevmiyeAciklama)) formIc.aciklama = yevmiyeAciklama;

                if (formIc.tc_Vno != altHesapKod && !string.IsNullOrEmpty(altHesapKod))
                {
                    formIc.tc_Vno = altHesapKod;
                    formIc.ilgili = "";
                    formIc.ilgBanka = "";
                    formIc.ilgIBAN = "";
                }
                if (string.IsNullOrEmpty(formIc.ilgIBAN) || string.IsNullOrEmpty(formIc.ilgili))
                {
                    if (bankasizVNolar != "") bankasizVNolar += ",";
                    bankasizVNolar += formIc.tc_Vno;
                }

                TNSDateTime oTarih = VeriErisim.DegerAl(dr, 16);
                string oNo = VeriErisim.DegerAl(dr, 17, "");

                if (TNS.HRC.Arac.CumhurbaskanligiMi())//Cumhurbaşkanlığı açıklama yerine fatura no ve tarih görülmesini istedi
                {
                    if (oNo.Trim() != "")
                    {
                        formIc.aciklama = "Fatura:" + oTarih.ToString() + " / " + oNo;
                    }
                }

                oa.Ekle(formIc);
            }
            dr.Close();

            if (bankasizVNolar != "")
            {
                TNS.HRC.IHRCServis hrcServis = TNS.HRC.Arac.Tanimla();
                HRC.FirmaBilgisi fb = new HRC.FirmaBilgisi();
                fb.muhasebe = saymanlikKod;
                fb.ad = bankasizVNolar; //.Replace(",", " ");
                ObjectArray fbl = hrcServis.FirmaBilgisiListele(kullanan, fb);

                TNS.PER.IPERServis perServis = TNS.PER.Arac.Tanimla();
                PER.Personel personel = new PER.Personel();
                personel.muhasebeKodu = 0;
                personel.tcList = new TNSCollection(bankasizVNolar.Split(','));
                ObjectArray pl = perServis.PersonelListele(kullanan, personel);
                Hashtable bankaListe = new Hashtable();
                foreach (MUHBankaOdemeDetay bod in oa.objeler)
                {
                    if (string.IsNullOrWhiteSpace(bod.ilgili) || string.IsNullOrWhiteSpace(bod.ilgIBAN))
                    {
                        IlgiliBilgi ib = IlgiliBilgiBul(perServis, kullanan, fbl, pl, bod.tc_Vno, bankaListe);
                        if (ib != null)
                        {
                            bod.ilgili = ib.ilgili;
                            bod.ilgBanka = ib.ilgBanka;
                            if (!string.IsNullOrWhiteSpace(ib.bicSwift)) bod.ilgBanka = bod.ilgBanka + " [" + ib.bicSwift + "]";
                            bod.ilgIBAN = ib.ilgIBAN;
                        }
                    }
                }
            }
            ve.VeriErisimKapat();
            return oa;
        }

        //private static void DebugYaz(string dbgStr)
        //{
        //    OrtakFonksiyonlar.DebugStrYaz("000BankaListesi.txt", dbgStr);
        //}

        private static IlgiliBilgi IlgiliBilgiBul(TNS.PER.IPERServis perServis, Kullanici kullanan, ObjectArray fbl, ObjectArray pl, string tcvergiNo, Hashtable bankaListe)
        {
            foreach (HRC.FirmaBilgisi item in fbl.objeler)
            {
                if (item.vergiNo == tcvergiNo)
                    return IlgiliOlFirmadan(item);
            }
            foreach (PER.Personel item in pl.objeler)
            {
                if (item.tcKimlikNo == tcvergiNo)
                    return IlgiliOlPersonelden(perServis, kullanan, item, bankaListe);
            }

            return null;
        }

        private static IlgiliBilgi IlgiliOlPersonelden(TNS.PER.IPERServis perServis, Kullanici kullanan, PER.Personel personel, Hashtable bankaListe)
        {
            IlgiliBilgi ib = new IlgiliBilgi();
            ib.ilgili = personel.ad + " " + personel.soyad;
            ib.ilgBanka = BankaBilgiBul(perServis, kullanan, personel.bankaKodu, personel.bankaSubeKodu, bankaListe);
            ib.bicSwift = "";
            ib.ilgIBAN = personel.bankaHesapNo;
            ib.vergiDairesi = personel.vergiDairesi;
            return ib;
        }

        private static IlgiliBilgi IlgiliOlFirmadan(HRC.FirmaBilgisi fb)
        {
            IlgiliBilgi ib = new IlgiliBilgi();
            ib.ilgili = fb.ad;
            ib.ilgBanka = fb.banka;
            ib.bicSwift = fb.bicSwift;
            ib.ilgIBAN = fb.hesapNo;
            ib.vergiDairesi = fb.vergiDairesi;
            return ib;
        }

        public static IlgiliBilgi IlgiliBilgiBul(VeriErisim veriErisim, Kullanici kullanan, string tc_Vno, int saymanlikKod)
        {
            TNS.PER.IPERServis p = TNS.PER.Arac.Tanimla();
            TNS.HRC.IHRCServis h = TNS.HRC.Arac.Tanimla();

            return IlgiliBilgiBulIc(kullanan, p, h, tc_Vno, saymanlikKod);
        }

        private static IlgiliBilgi IlgiliBilgiBulIc(Kullanici kullanan, TNS.PER.IPERServis perServis, TNS.HRC.IHRCServis hrcServis, string tc_Vno, int saymanlikKod)
        {
            IlgiliBilgi ib = null;

            if (string.IsNullOrEmpty(tc_Vno)) return ib;
            tc_Vno = tc_Vno.Trim();
            //Firma Ara
            HRC.FirmaBilgisi fb = new HRC.FirmaBilgisi();
            fb.muhasebe = saymanlikKod;
            fb.vergiNo = tc_Vno;
            fb.personeldeDeAra = true;
            fb.aktifDurum = 1;
            ObjectArray fbl = hrcServis.FirmaBilgisiListele(kullanan, fb);
            if (fbl != null && fbl.objeler.Count > 0 && fbl.sonuc.islemSonuc)
            {
                fb = (HRC.FirmaBilgisi)fbl.objeler[0];
                return IlgiliOlFirmadan(fb);
            }

            //Personel Ara
            //if (tc_Vno.Length == 11)
            //{
            //    PER.Personel personel = new PER.Personel();
            //    personel.muhasebeKodu = 0;
            //    personel.tcKimlikNo = tc_Vno;
            //    ObjectArray pl = perServis.PersonelListele(kullanan, personel);
            //    if (pl != null && pl.objeler.Count > 0 && pl.sonuc.islemSonuc)
            //    {
            //        return IlgiliOlPersonelden(perServis, kullanan, (PER.Personel)pl.objeler[0], null);
            //    }
            //}

            HRC.OgrenciEgitmen oe = new HRC.OgrenciEgitmen();
            oe.muhasebe = saymanlikKod;
            oe.tcKimlikNo = tc_Vno;

            fbl = hrcServis.OgrenciEgitmenListele(kullanan, oe);
            if (fbl != null && fbl.objeler.Count > 0 && fbl.sonuc.islemSonuc)
            {
                oe = (HRC.OgrenciEgitmen)fbl.objeler[0];
                ib = new IlgiliBilgi();
                ib.ilgili = oe.adiSoyadi;
                ib.ilgBanka = oe.bankaAdi;
                ib.bicSwift = "";
                ib.ilgIBAN = oe.bankaHesapNo;
                ib.vergiDairesi = "";
                return ib;
            }
            return ib;
        }

        private static string BankaBilgiBul(PER.IPERServis perServis, Kullanici kullanan, string bankaKod, string subeKod, Hashtable liste)
        {
            if (!string.IsNullOrWhiteSpace(bankaKod) && !string.IsNullOrWhiteSpace(subeKod))
            {
                string bankaAd = null;
                try
                {
                    bankaAd = (string)liste[bankaKod + "-" + subeKod];
                }
                catch
                {
                    bankaAd = null;
                }
                if (string.IsNullOrWhiteSpace(bankaAd))
                {
                    TNS.PER.BankaSube bankaSube = new PER.BankaSube();
                    bankaSube.bankaKodu = bankaKod;
                    bankaSube.kod = subeKod;
                    ObjectArray oa = perServis.BankaSubeListele(bankaSube);
                    if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
                    {
                        bankaSube = (TNS.PER.BankaSube)oa.objeler[0];
                        bankaAd = bankaSube.bankaAd + " - " + bankaSube.ad;
                    }
                    else
                        bankaAd = bankaKod + "-" + subeKod;
                }
                if (liste != null)
                {
                    liste[bankaKod + "-" + subeKod] = bankaAd;
                }
                return bankaAd;
            }
            return "tanımsız";
        }

        internal static Sonuc DetayOdendi(VeriErisim veriErisim, Kullanici kullanan, int saymanlik, string birim, TNSDateTime gun, int siraNo, int sira, string dekontNo)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBANKAODEMEDETAY tab = new TABMUHBANKAODEMEDETAY(ve);

            IDataReader dr = tab.Oku(saymanlik, birim, gun, siraNo, sira);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                s.DegerAta(ve.hata);
                return s;
            }
            MUHBankaOdemeDetay detay = null;
            if (dr.Read())
            {
                detay = new MUHBankaOdemeDetay();
                DetayDoldur(detay, dr);
            }
            dr.Close();
            if (detay == null)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Kayıt bulunamadı!");
                return s;
            }

            if (!string.IsNullOrEmpty(detay.dekontNo))
            {
                ve.VeriErisimKapat();
                s.DegerAta("Bu kayıt daha önce kapatılmış!");
                return s;
            }

            OdemeEmriMIFForm eskiForm = new OdemeEmriMIFForm();
            eskiForm.yil = detay.mifYil;
            eskiForm.belgeNo = detay.mifBelgeNo;
            TNS.ObjectArray eskiFormOA = (new MUHOdemeEmriMIFServis()).Listele(kullanan, eskiForm, true, false, false);
            if (!eskiFormOA.sonuc.islemSonuc || eskiFormOA.ObjeSayisi != 1)
            {
                ve.VeriErisimKapat();
                s.DegerAta("İlgili ödeme emri belgesi okunamadı!");
                return s;
            }
            string dekTarih = "";
            string dekNo = "";
            OrtakFonksiyonlar.StringiIkiyeBol(dekontNo, "/", ref dekTarih, ref dekNo);

            eskiForm = (OdemeEmriMIFForm)eskiFormOA.objeler[0];
            eskiForm.belgeNo = "";

            OdemeEmriMIFForm yeni = new OdemeEmriMIFForm();
            yeni.belgeNo = "";
            yeni.islemTarih = new TNSDateTime(dekTarih);
            yeni.yil = yeni.islemTarih.Yil;
            yeni.ilgiliAd = eskiForm.ilgiliAd;
            yeni.ilgiliBankaAd = eskiForm.ilgiliBankaAd;
            yeni.ilgiliBankaNo = eskiForm.ilgiliBankaNo;
            yeni.ilgiliBelgeNo = detay.mifBelgeNo;
            yeni.ilgiliNo = eskiForm.ilgiliNo;
            yeni.ilgiliVD = eskiForm.ilgiliVD;

            yeni.islemYapan = kullanan.kullaniciKodu;
            yeni.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
            yeni.OEBNo = dekNo;
            yeni.kurum = eskiForm.kurum;
            yeni.kurKod = eskiForm.kurKod;
            yeni.kurumAd = eskiForm.kurumAd;
            yeni.muhasebe = eskiForm.muhasebe;
            yeni.birim = eskiForm.birim;
            yeni.aciklama = eskiForm.aciklama + " (Banka Transferi)";

            OdemeEmriMIFDetay okunan = (new MUHOdemeEmriMIFServis()).SatirOku(ve, detay.mifYil, detay.mifBelgeNo, detay.mifSira);
            if (okunan == null)
            {
                ve.VeriErisimKapat();
                s.DegerAta("İlgili ödeme emri detay kaydı okunamadı!");
                return s;
            }

            string hesapKod = detay.muhHesapKodu;
            OdemeEmriMIFDetay d1 = SatirDegistir(okunan);

            string yeniHesapKod = YeniHesapBul(ve, yeni.yil, hesapKod);
            okunan.hesapKod = yeniHesapKod;
            d1.mahsupBelgeNo = detay.mifBelgeNo;
            yeni.detay.Ekle(d1);
            d1.satirNo = 1;
            yeni.detay.Ekle(okunan);
            okunan.satirNo = 2;

            s = (new MUHOdemeEmriMIFServis()).Kaydet(kullanan, yeni);

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            string odemeEmriNo = s.anahtar;
            ve.BeginTransaction();

            s = tab.DetayOdendi(kullanan, saymanlik, birim, gun, siraNo, sira, dekontNo, odemeEmriNo);
            if (s.islemSonuc)
            {
                s.anahtar = odemeEmriNo;
                ve.Commit();
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc DetayOdendiToplu(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay[] mbod)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (mbod == null || mbod.Length == 0)
            {
                s.DegerAta("Kapatılacak kayıtları seçiniz!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBANKAODEMEDETAY tab = new TABMUHBANKAODEMEDETAY(ve);

            OdemeEmriMIFForm yeni = new OdemeEmriMIFForm();
            yeni.belgeNo = "";
            yeni.islemTarih = bilgi.kapatmaTarihi;
            yeni.yil = yeni.islemTarih.Yil;
            yeni.ilgiliNo = TNS.DEG.Arac.VarsayilanKurumBul();
            yeni.ilgiliAd = "";
            yeni.ilgiliBankaAd = "";
            yeni.ilgiliBankaNo = "";
            yeni.ilgiliBelgeNo = "";
            yeni.ilgiliVD = "";

            yeni.islemYapan = kullanan.kullaniciKodu;
            yeni.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
            yeni.kurum = yeni.ilgiliNo;
            yeni.muhasebe = bilgi.saymanlik;
            yeni.birim = bilgi.birim;
            yeni.aciklama = "Kapatma Belgesi";
            Hashtable ht = new Hashtable();

            TNS.HRC.IHRCServis hrcServis = TNS.HRC.Arac.Tanimla();
            HRC.MerkezBankasiKurSinif kurForm = null;
            Hashtable htHesapPlan = new Hashtable();
            for (int i = 0; i < mbod.Length; i++)
            {
                string hesapKod = mbod[i].muhHesapKodu;
                string yeniHesapKod = YeniHesapBul(ve, yeni.yil, hesapKod);
                double anaParaTutar = mbod[i].tutar;
                double dovizTutar = 0;
                decimal dovizKur = 0;
                string kurKisaltma = "";
                string kurTur = "";
                if (!hesapKod.StartsWith("103"))
                {
                    if (kurForm == null)
                    {
                        kurForm = new HRC.MerkezBankasiKurSinif();
                        kurForm.kurTarih = yeni.islemTarih;
                        kurForm = hrcServis.MerkezBankKurOku(kullanan, kurForm);
                        if (kurForm == null)
                        {
                            s.DegerAta("Kur bilgisi okunamadı");
                            break;
                        }
                        else if (!kurForm.sonuc.islemSonuc)
                        {
                            s = kurForm.sonuc;
                            break;
                        }
                    }

                    HesapPlaniSatir hs = HesapPlaniSatirBul(htHesapPlan, yeni.yil, yeniHesapKod);
                    if (!string.IsNullOrWhiteSpace(hs.dovizKurKod))
                    {
                        kurTur = hs.dovizKurTurAlis;
                        kurKisaltma = hs.dovizKurKod;
                        MerkezBankasiKurDetaySinif kurdetay = DovizBul(kurForm, hs.dovizKurKod);
                        if (kurdetay == null || kurdetay.dovizSatis == 0)
                        {
                            s.DegerAta(hs.dovizKurKod + " para biriminin karşılığı bulunamadı!");
                            break;
                        }
                        dovizTutar = anaParaTutar;
                        dovizKur = kurdetay.DegerBul(kurTur, 0);
                        anaParaTutar = Math.Round(dovizTutar * Convert.ToDouble(dovizKur), 2);
                    }
                    else
                    {
                        s.DegerAta(yeniHesapKod + " hesabının döviz türü bulunamadı!");
                        break;
                    }
                }
                HTEkle(ht, yeniHesapKod, anaParaTutar, dovizTutar);

                OdemeEmriMIFDetay okunan = (new MUHOdemeEmriMIFServis()).SatirOku(ve, mbod[i].mifYil, mbod[i].mifBelgeNo, mbod[i].mifSira);
                if (okunan == null)
                {
                    ve.VeriErisimKapat();
                    s.DegerAta("İlgili ödeme emri detay kaydı okunamadı!");
                    return s;
                }
                OdemeEmriMIFDetay yeniSatir = new OdemeEmriMIFDetay(yeni.yil);
                yeniSatir.detayBirim = okunan.detayBirim;
                yeniSatir.hesapKod = hesapKod;
                yeniSatir.satirNo = 0;
                yeniSatir.borc = anaParaTutar;
                yeniSatir.alacak = 0;
                yeniSatir.dovizBorc = dovizTutar;
                yeniSatir.dovizAlacak = 0;
                yeniSatir.doviz = kurKisaltma;
                yeniSatir.dovizKurTur = kurTur;
                yeniSatir.dovizKur = dovizKur;

                yeniSatir.altHesapKod = mbod[i].tc_Vno;
                yeniSatir.mahsupBelgeNo = mbod[i].mifBelgeNo;
                yeni.detay.Ekle(yeniSatir);
            }

            int satirNo = mbod.Length + 1;
            foreach (DictionaryEntry de in ht)
            {
                string hesapKod = (string)de.Key;
                double[] miktarlar = (double[])de.Value;
                HesapPlaniSatir hs = HesapPlaniSatirBul(htHesapPlan, yeni.yil, hesapKod);

                OdemeEmriMIFDetay yeniSatir = new OdemeEmriMIFDetay(yeni.yil);
                yeniSatir.hesapKod = hesapKod;
                yeniSatir.satirNo = satirNo++;
                yeniSatir.borc = 0;
                yeniSatir.alacak = miktarlar[0];
                yeniSatir.dovizBorc = 0;
                yeniSatir.dovizAlacak = miktarlar[1];
                yeniSatir.altHesapKod = "";
                yeniSatir.doviz = hs.dovizKurKod;
                if (!string.IsNullOrWhiteSpace(yeniSatir.doviz))
                {
                    yeniSatir.dovizKurTur = hs.dovizKurTurAlis;
                    yeniSatir.dovizKur = OrtakFonksiyonlar.ConvertToDecimal(miktarlar[0] / miktarlar[1]);
                }
                yeni.detay.Ekle(yeniSatir);
            }

            if (s.islemSonuc)
                s = (new MUHOdemeEmriMIFServis()).Kaydet(kullanan, yeni);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();
            string odemeEmriNo = s.anahtar;
            for (int i = 0; i < mbod.Length; i++)
            {
                s = tab.DetayOdendi(kullanan, bilgi.saymanlik, bilgi.birim, bilgi.gun, bilgi.siraNo, mbod[i].sira, "-", odemeEmriNo);
                if (!s.islemSonuc) break;
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static string YeniHesapBul(VeriErisim ve, int yil, string hesapKod)
        {
            HesapPlaniYansitma pYansitma = new HesapPlaniYansitma();
            pYansitma.yil = yil;
            pYansitma.hesapKod = hesapKod;
            ObjectArray oa = TNS.MUH.IsKurallari.MUHYansitmaServis.Listele(ve, pYansitma, true);
            string yeniHesapKod = hesapKod.Substring(5);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
            {
                yeniHesapKod = ((HesapPlaniYansitma)(oa.objeler[0])).yansitmaHesapKod;
            }
            else
            {
                if (hesapKod.StartsWith("103"))
                    yeniHesapKod = "10201" + yeniHesapKod;
                else
                    yeniHesapKod = "10501" + yeniHesapKod;
            }
            return yeniHesapKod;
        }

        private static MerkezBankasiKurDetaySinif DovizBul(MerkezBankasiKurSinif kurForm, string paraKisaltma)
        {
            foreach (MerkezBankasiKurDetaySinif item in kurForm.satirlar)
            {
                if (item.paraBirim.kisaltma == paraKisaltma)
                    return item;
            }
            return null;
        }

        private static HesapPlaniSatir HesapPlaniSatirBul(Hashtable htHesapPlan, int yil, string hesapKod)
        {
            HesapPlaniSatir hs = (HesapPlaniSatir)htHesapPlan[hesapKod];
            if (hs == null)
            {
                hs = new HesapPlaniSatir();
                hs.yil = yil;
                hs.hesapKod = hesapKod;
                hs = (new MUHHesapPlaniServis()).OzellikAl(null, hs);
                htHesapPlan[hesapKod] = hs;
            }
            return hs;
        }

        private static void HTEkle(Hashtable ht, string hesapKod, double miktar, double doviz)
        {
            double[] eski = null;
            try
            {
                eski = (double[])ht[hesapKod];
            }
            catch { }
            if (eski == null) eski = new double[] { 0, 0 };
            eski[0] += miktar;
            eski[1] += doviz;
            ht[hesapKod] = eski;
        }

        private static OdemeEmriMIFDetay SatirDegistir(OdemeEmriMIFDetay eski)
        {
            OdemeEmriMIFDetay yeni = new OdemeEmriMIFDetay(eski);
            DegerDegistir(ref yeni.borc, ref yeni.alacak);
            DegerDegistir(ref yeni.dovizBorc, ref yeni.dovizAlacak);
            return yeni;
        }

        private static void DegerDegistir(ref double p1, ref double p2)
        {
            double sakla = p1;
            p1 = p2;
            p2 = sakla;
        }

        internal static Sonuc BankayaGonderildi(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme mbO, string sonucDosyaAd)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);

            ve.BeginTransaction();

            s = tab.BankayaGonderildi(kullanan, mbO, sonucDosyaAd);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        //internal static Sonuc CekNoYaz(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme bilgi)
        //{
        //    Sonuc s = new Sonuc(true, "", "");
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

        //    TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);
        //    int iCekNo = 0;
        //    if (!string.IsNullOrWhiteSpace(bilgi.cekEmriNo))
        //    {
        //        iCekNo = OrtakFonksiyonlar.ConvertToInt(bilgi.cekEmriNo, -1);
        //        if (iCekNo <= 0)
        //            s.DegerAta("Geçerli bir çek numarasını giriniz! Hatalı çek no:" + bilgi.cekEmriNo);
        //        bilgi.cekEmriNo = CekEmriYap(iCekNo);
        //    }

        //    MUHBankaOdeme okunan = Oku(ve, kullanan, bilgi);

        //    if (okunan.odemeTarih == null || okunan.odemeTarih.isNull)
        //    {
        //        s.DegerAta("Ödeme listesinin ödeme tarihini giriniz!");
        //    }
        //    else
        //    {
        //        if (string.IsNullOrWhiteSpace(bilgi.cekEmriNo))
        //        {
        //            string tut = tab.MaxCekEmriNoBul(okunan.odemeTarih, bilgi.saymanlik);
        //            iCekNo = OrtakFonksiyonlar.ConvertToInt(tut, 0) + 1;
        //            bilgi.cekEmriNo = CekEmriYap(iCekNo);
        //        }

        //        if (string.IsNullOrWhiteSpace(okunan.cekEmriNo))
        //        {
        //            int say = tab.CekEmriNoSayiBul(okunan.odemeTarih, okunan.saymanlik, bilgi.cekEmriNo);
        //            if (say > 0)
        //                s.DegerAta("Bu çek no daha önce kullanılmış!");
        //        }
        //        else if (okunan.cekEmriNo != bilgi.cekEmriNo)
        //        {
        //            s.DegerAta("Bu ödeme listesine daha önce verdiğiniz numara ile bu numara aynı değil");
        //        }
        //    }

        //    if (!s.islemSonuc)
        //    {
        //        ve.VeriErisimKapat();
        //        return s;
        //    }

        //    ve.BeginTransaction();

        //    s = tab.CekNoGuncelle(bilgi);

        //    if (s.islemSonuc)
        //        s = tab.OdemeEmriCekNoGuncelle(bilgi, okunan.odemeTarih.Yil);

        //    if (s.islemSonuc)
        //    {
        //        s.anahtar = bilgi.cekEmriNo;
        //        ve.Commit();
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return s;
        //}

        private static string CekEmriYap(int iCekNo)
        {
            return iCekNo.ToString("00000000");
        }

        internal static int BankaDosyaSiraNoBul(VeriErisim veriErisim, Kullanici kullanan, string onAd)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);
            string maxDosyaAd = tab.MaksBankaDosyaAdBul(onAd);
            if (string.IsNullOrEmpty(maxDosyaAd)) return 1;
            if (maxDosyaAd == onAd) return 2;
            int adUz = maxDosyaAd.Length;
            if (adUz < 2) return 1;
            adUz = OrtakFonksiyonlar.ConvertToInt(maxDosyaAd.Substring(adUz - 2), -1);
            if (adUz <= 0) return 1;
            return adUz + 1;
        }

        internal static Sonuc CekNoYaz(VeriErisim veriErisim, Kullanici kullanan, MUHBankaOdeme bilgi, string cekNo)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrEmpty(cekNo))
            {
                s.DegerAta("Çek numarası girilmelidir.");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            MUHBankaOdeme eski = Oku(ve, kullanan, bilgi);
            if (eski == null)
            {
                s.DegerAta("Güncellenecek form bulunamadı!");
                ve.VeriErisimKapat();
                return s;
            }

            if (eski.durum == ENUMBankaListesiDurum.Onayli)
            {
                s.DegerAta("Belge onaylı olduğu için değiştirilemez!");
                ve.VeriErisimKapat();
                return s;
            }

            TABMUHBANKAODEMEANA tab = new TABMUHBANKAODEMEANA(ve);
            TABMUHBANKAODEMEDETAY tabDetay = new TABMUHBANKAODEMEDETAY(ve);

            if (!string.IsNullOrWhiteSpace(eski.cekEmriNo))
            {
                if (eski.cekEmriNo != cekNo)
                {
                    s.DegerAta("Bu banka listesi daha önce " + eski.cekEmriNo + " numaralı çek emri numarası ile ilişkilendirilmiş!");
                    ve.VeriErisimKapat();
                    return s;
                }
            }
            else
            {
                int cekSay = tab.CekEmriNoSayiBul(eski.odemeTarih.Yil, eski.saymanlik, eski.birim, cekNo);
                if (cekSay > 0)
                {
                    s.DegerAta("Bu " + cekNo + " numaralı çek emri numarası, daha önce başka bir banka listesi ile ilişkilendirilmiş!");
                    ve.VeriErisimKapat();
                    return s;
                }
            }

            if (eski.tur == ENUMBankaListesiTur.HarcamaSurecUretti)
            {
                s.DegerAta("Form süreçten üretildiği için değiştirilemez!");
                ve.VeriErisimKapat();
                return s;
            }

            ObjectArray oa = OdenecekEmirListele(ve, kullanan, bilgi.gun.Yil, bilgi.saymanlik, bilgi.birim, null, null, cekNo);
            if (!oa.sonuc.islemSonuc || oa.ObjeSayisi == 0)
            {
                ve.VeriErisimKapat();
                if (!oa.sonuc.islemSonuc)
                    return oa.sonuc;
                else
                {
                    s.DegerAta("Eklenecek kayıt bulunamadı!");
                    return s;
                }
            }

            ve.BeginTransaction();
            s = tab.Degisti(kullanan, bilgi);
            if (s.islemSonuc)
            {
                bilgi.cekEmriNo = cekNo;
                s = tab.CekNoGuncelle(bilgi);
            }
            int sira = 0;
            if (s.islemSonuc)
                sira = tabDetay.SiraNoBul(bilgi) + 1;

            for (int i = 0; i < oa.ObjeSayisi && s.islemSonuc; i++)
            {
                MUHBankaOdemeDetay formIc = (MUHBankaOdemeDetay)oa.objeler[i];
                s = tabDetay.Ekle(bilgi, sira++, formIc);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static string ParcaAl(string satir, ref int yer, int uzunluk)
        {
            string tut = "";
            try
            {
                tut = satir.Substring(yer, uzunluk);
            }
            catch { }
            yer += uzunluk;
            return tut;
        }

        internal static Sonuc BankaAkibetDosyasiIsle(VeriErisim veriErisim, Kullanici kullanan, TNSDateTime islenecekGun)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            islenecekGun = new TNSDateTime(new DateTime(2014, 4, 17));
            string onAd = "qq"; // TNS.MUH.Arac.BankaDosyaOnAdOl("musteriNo", islenecekGun, "OD");
            string anaYol = OrtakFonksiyonlar.WebConfigOku("MuhasebeDosyaYol", "");
            FTPIslem di = new FTPIslem("halkbankftp");
            string mifBilgiler = "";
            for (int i = 0; true; i++)
            {
                string gelenDosyaAd = onAd + OrtakFonksiyonlar.MetinYap(i, 2);
                Sonuc s = di.FTPdenDosyaya(gelenDosyaAd, anaYol + "/" + gelenDosyaAd);
                if (!s.islemSonuc)
                    break;

                System.IO.StreamReader sw = new System.IO.StreamReader(anaYol + "/" + gelenDosyaAd, System.Text.Encoding.Default);
                try
                {
                    while (!sw.EndOfStream)
                    {
                        string satir = sw.ReadLine();
                        if (satir[0] == 'D')
                        {
                            int yer = 1;
                            string tarihStr = ParcaAl(satir, ref yer, 8);
                            string mbSubeNo = ParcaAl(satir, ref yer, 5);
                            string bankaIBAN = ParcaAl(satir, ref yer, 26);
                            string temp1 = ParcaAl(satir, ref yer, 3);
                            temp1 = ParcaAl(satir, ref yer, 4);
                            temp1 = ParcaAl(satir, ref yer, 5);
                            string ilgIBAN = ParcaAl(satir, ref yer, 26);
                            string tutar = ParcaAl(satir, ref yer, 15);
                            string aciklama = ParcaAl(satir, ref yer, 30);
                            string mifBilgi = ParcaAl(satir, ref yer, 36);
                            mifBilgiler += mifBilgi + "<br />";
                            temp1 = ParcaAl(satir, ref yer, 14);
                            string ilgili = ParcaAl(satir, ref yer, 30);
                            temp1 = ParcaAl(satir, ref yer, 60);
                            string durum = ParcaAl(satir, ref yer, 2);
                            if (durum == "01")
                            {
                            }
                            /*
                                01	İŞLEM TAMAM-ÖDEME YAPILDI                             
                                02	BAKİYE YETERSİZ
                                03	ALACAKLI HESAP HATALI (HAVALE İÇİN)
                                04	HEDEF BANKA-ŞUBE KODU HATALI
                                05	NUMERİK/ALFABETİK ALANDA HATALI KARAKTER
                                06	BORÇLU HESAP HATALI
                                07	TARİH HATALI (İŞGÜNÜ DEĞİL YADA FORMATI HATALI)
                                08	ALACAK HESAP KAPALI/BLOKELİ (HAVALE İÇİN)
                                09	YP HESAPTAN EFT YAPILAMAZ
                                10	İŞLEM DEVAM EDİYOR
                                11	KAYIT SİLİNMİŞ
                                12	İŞLEM TUTARI KOMİSYON İÇİN YETERSİZ
                                13	ADET/TOPLAM TUTAR HATALI
                            * */
                            string eftHavaleRef = ParcaAl(satir, ref yer, 30);
                        }
                    }
                }
                catch (Exception ex)
                {
                    sonuc.DegerAta(ex.Message);
                }
                sw.Close();
                if (!sonuc.islemSonuc)
                    break;
            }
            if (sonuc.islemSonuc)
                sonuc.DegerAta(mifBilgiler);
            return sonuc;
        }
    }
}

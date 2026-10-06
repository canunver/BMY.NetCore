using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
using TNS.KYM.IsKurallari.Veri;
using System.Collections;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// KYMAnketSoru formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class AnketServis
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriKYM";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, AnketSoru form)
        {

            Sonuc s = new Sonuc(true, "", "");
            form.AlanlarTutarliMi(s);
            return s;
        }

        /// <summary>
        /// AnketSoru nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="anketSoru">AnketSoru yapısında AnketSoru: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AnketSoru anketSoru)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, anketSoru);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            SoruAnketteVarMi(tabAnket, s);

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();

            if (anketSoru.prSiraNo != 0)
            {
                if (s.islemSonuc)
                    s = tabAnket.SecenekleriSil(anketSoru);
                if (s.islemSonuc)
                    s = tabAnket.SoruSil(anketSoru);
            }
            else
            {
                anketSoru.prSiraNo = tabAnket.MaxSoruNoBul();
                anketSoru.prSiraNo++;
            }
            if (s.islemSonuc)
                s = tabAnket.SoruEkle(anketSoru);

            for (int i = 0; i < anketSoru.cevapSecenekler.Length && s.islemSonuc; i++)
            {
                s = tabAnket.SecenekEkle(anketSoru, i);
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = anketSoru.prSiraNo.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, AnketSoru form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, AnketSoru form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// AnketSoru nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AnketSoru yapısında AnketSoru: bilgilerini tutan değişken</param>
        public static AnketSoru[] Listele(VeriErisim veriErisim, Kullanici kullanan, AnketSoru form)
        {
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception("Görme yetkiniz yok!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            IDataReader dr = tabAnket.SorulariListele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                AnketSoru formIc = new AnketSoru();
                formIc.prSiraNo = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.PRSIRANO, 0);
                formIc.soruMetin = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.METIN, "");
                formIc.secenekTur = (ENUMSecenekTur)VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.SECENEKTUR, 0);
                formIc.anahtar1 = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.ANAHTAR1, "");
                formIc.anahtar2 = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.ANAHTAR2, "");
                formIc.dogruCevapSecenek = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.CEVAPSECENEK, 0);
                formIc.durum = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.DURUM, 0);
                al.Add(formIc);
            }
            dr.Close();

            SecenekleriOku(ve, tabAnket, al, false, null);

            ve.VeriErisimKapat();
            return (AnketSoru[])al.ToArray(typeof(AnketSoru));
        }

        private static void SecenekleriOku(VeriErisim ve, TABANKET tabAnket, ArrayList al, bool cevaplarDahil, AnketBilgi anketBilgi)
        {
            ve = VeriErisim.VeriErisimBul(ve, VERIERISIMAD, null);

            IDataReader dr = null;
            foreach (AnketSoru soru in al)
            {
                dr = tabAnket.SecenekleriOku(soru);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }
                ArrayList alIc = new ArrayList();
                while (dr.Read())
                {
                    CevapSecenekler detay = new CevapSecenekler();
                    detay.secenekSiraNo = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSECENEKLER.SECENEKSIRANO, 0);
                    detay.secenek = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSECENEKLER.SECENEK, "");
                    detay.secenekMetin = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSECENEKLER.METIN, "");
                    detay.puan = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSECENEKLER.PUAN, 0.0);
                    alIc.Add(detay);
                }
                soru.cevapSecenekler = (CevapSecenekler[])alIc.ToArray(typeof(CevapSecenekler));
                dr.Close();
            }
            if (cevaplarDahil)
            {
                dr = tabAnket.CevaplariOku(anketBilgi);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        int soruSiraNo = VeriErisim.DegerAl(dr, 0, 0);
                        int secenekSiraNo = VeriErisim.DegerAl(dr, 1, 0);
                        string secenekMetin = VeriErisim.DegerAl(dr, 2, "");
                        double puan = VeriErisim.DegerAl(dr, 3, 0.0);
                        AnketSoru soru = SoruBul(al, soruSiraNo);
                        if (soru != null)
                        {
                            soru.CevapEkle(secenekSiraNo, secenekMetin, puan);
                        }
                    }
                    dr.Close();
                }
            }

            ve.VeriErisimKapat();
        }

        private static AnketSoru SoruBul(ArrayList al, int prSiraNo)
        {
            foreach (AnketSoru soru in al)
            {
                if (soru.prSiraNo == prSiraNo)
                    return soru;
            }
            return null;
        }

        internal static string[] AnahtarKelimeler(VeriErisim veriErisim, int anahtarNo, string kistas)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, null);
            TABANKET tabAnket = new TABANKET(ve);

            IDataReader dr = tabAnket.AnahtarKelimeleriListele(anahtarNo, kistas);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList al = new ArrayList();
            int eklenenSay = 20;
            while (dr.Read())
            {
                string metin = VeriErisim.DegerAl(dr, 0, "");
                al.Add(metin);
                if (eklenenSay-- <= 0)
                    break;
            }
            dr.Close();
            ve.VeriErisimKapat();
            return (string[])al.ToArray(typeof(string));
        }

        /// <summary>
        /// Anket sorusunun, anketlerde kullanılıp kullanılamayacağı durumuna değer atayan yordam
        /// </summary>
        /// <param name="kullanan">Yordamı çağıran kullanıcı bilgilerini tutan nesne</param>
        /// <param name="anketSoru">Soru numarasını ve durumunu tutan nesne</param>
        /// <returns>İşlem sonucu döndürülür.</returns>
        internal static Sonuc SoruDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, AnketSoru anketSoru)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            ve.BeginTransaction();

            if (anketSoru.prSiraNo > 0)
            {
                s = tabAnket.SoruDurumDegistir(anketSoru);
            }
            else
            {
                s.DegerAta("Yeni sorunun durumu değiştirilemez!");
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = anketSoru.prSiraNo.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc AnketBilgiKaydet(VeriErisim veriErisim, Kullanici kullanan, AnketBilgi anketBilgi)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            anketBilgi.AlanlarGecerliMi(s);
            if (!s.islemSonuc) return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            if (AnketCevaplanmisMi(tabAnket, anketBilgi))
            {
                s.DegerAta("Bu ankete cevap verildiği için değiştiremezsiniz!");
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabAnket.AnketSorulariSil(anketBilgi);

            if (s.islemSonuc)
                s = tabAnket.AnketBilgiSil(anketBilgi);

            if (s.islemSonuc)
                s = tabAnket.AnketBilgiKaydet(anketBilgi);

            for (int i = 0; s.islemSonuc && i < anketBilgi.sorular.Length; i++)
                s = tabAnket.AnketeSoruKaydet(anketBilgi, i);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static AnketBilgi AnketBilgiOku(VeriErisim veriErisim, Kullanici kullanan, AnketBilgi anketKistasBilgi, bool seceneklerDahil, bool cevaplarDahil)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            IDataReader dr = tabAnket.AnketBilgiOku(anketKistasBilgi);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            AnketBilgi anketBilgi = null;
            if (dr.Read())
            {
                anketBilgi = new AnketBilgi();
                anketBilgi.modul = anketKistasBilgi.modul;
                anketBilgi.anketAnahtar = anketKistasBilgi.anketAnahtar;
                anketBilgi.anketAd = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.ANKETAD, "");
                anketBilgi.baslaTarih = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.BASLATARIH);
                anketBilgi.bitisTarih = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.BITISTARIH);
                anketBilgi.tekGirislik = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.TEKGIRISLIK, 0) == 1 ? true : false;
            }
            dr.Close();

            if (anketBilgi == null)
            {
                ve.VeriErisimKapat();
                return null;
            }

            ArrayList al = new ArrayList();

            dr = tabAnket.AnketSorulariListele(anketKistasBilgi);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            while (dr.Read())
            {
                AnketSoru anketSoru = new AnketSoru();
                anketSoru.prSiraNo = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.PRSIRANO, 0);
                anketSoru.soruMetin = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.METIN, "");
                anketSoru.secenekTur = (ENUMSecenekTur)VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.SECENEKTUR, 0);
                anketSoru.anahtar1 = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.ANAHTAR1, "");
                anketSoru.anahtar2 = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.ANAHTAR2, "");
                anketSoru.dogruCevapSecenek = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.CEVAPSECENEK, 0);
                anketSoru.durum = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.DURUM, 0);
                anketSoru.soruSira = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.DURUM + 1, 0);
                anketSoru.soruSeviye = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.DURUM + 2, 0);
                al.Add(anketSoru);
            }
            dr.Close();

            if (seceneklerDahil)
                SecenekleriOku(ve, tabAnket, al, cevaplarDahil, anketKistasBilgi);

            anketBilgi.sorular = (AnketSoru[])al.ToArray(typeof(AnketSoru));

            ve.VeriErisimKapat();
            return anketBilgi;
        }

        internal static AnketBilgi[] AnketBilgiListele(VeriErisim veriErisim, Kullanici kullanan, AnketBilgi anketKistasBilgi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            IDataReader dr = tabAnket.AnketBilgiOku(anketKistasBilgi);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            ArrayList liste = new ArrayList();
            AnketBilgi anketBilgi = null;
            while (dr.Read())
            {
                anketBilgi = new AnketBilgi();
                anketBilgi.modul = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.MODUL, ""); ;
                anketBilgi.anketAnahtar = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.ANAHTAR, "");
                anketBilgi.anketAd = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.ANKETAD, "");
                anketBilgi.baslaTarih = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.BASLATARIH);
                anketBilgi.bitisTarih = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.BITISTARIH);
                anketBilgi.tekGirislik = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.TEKGIRISLIK, 0) == 1 ? true : false;

                liste.Add(anketBilgi);
            }
            dr.Close();

            if (anketBilgi == null)
            {
                ve.VeriErisimKapat();
                return null;
            }

            ve.VeriErisimKapat();
            return (AnketBilgi[])liste.ToArray(typeof(AnketBilgi)); ;
        }

        internal static Sonuc AnketBilgisineSoruKaydet(VeriErisim veriErisim, Kullanici kullanan, AnketBilgi anketBilgi)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            if (AnketCevaplanmisMi(tabAnket, anketBilgi))
            {
                s.DegerAta("Bu ankete cevap verildiği için değiştiremezsiniz!");
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();

            for (int i = 0; s.islemSonuc && i < anketBilgi.sorular.Length; i++)
                s = tabAnket.AnketeSoruKaydet(anketBilgi, i);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        internal static Sonuc AnketCevaplariKaydet(VeriErisim veriErisim, Kullanici kullanan, AnketBilgi anketBilgi)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!CevapYetkisiVarMi(kullanan, anketBilgi))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            if (!string.IsNullOrEmpty(anketBilgi.mailAdres))
                anketBilgi.kullaniciKod = anketBilgi.mailAdres;
            else if (kullanan != null)
                anketBilgi.kullaniciKod = kullanan.kullaniciKodu;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            if (anketBilgi.tekGirislik && tabAnket.AnketCevapAnaVarMi(anketBilgi))
            {
                ve.VeriErisimKapat();
                s.DegerAta("Bu anketi daha önce doldurmuşsunuz ve değişiklik yapamazsınız!");
                return s;
            }

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabAnket.AnketCevaplariSil(anketBilgi);

            if (s.islemSonuc)
                s = tabAnket.AnketCevapAnaSil(anketBilgi);

            if (s.islemSonuc)
                s = tabAnket.AnketCevapAnaKaydet(anketBilgi);

            for (int i = 0; s.islemSonuc && i < anketBilgi.sorular.Length; i++)
            {
                AnketSoru soru = anketBilgi.sorular[i];
                for (int j = 0; s.islemSonuc && j < soru.CevapSay(); j++)
                    s = tabAnket.AnketCevapDetayKaydet(anketBilgi, soru, soru.Cevap(j));
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        public static Sonuc AnketCevaplariSifirla(VeriErisim veriErisim, Kullanici kullanan, int hibeKod)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI) && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEBASKAN))
            {
                s.islemSonuc = false;
                s.hataStr = "Bu işlemi yapma yetkiniz yok.";
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabAnket.AnketCevaplariSil(new AnketBilgi() { cevapAnahtar = hibeKod.ToString() });

            if (s.islemSonuc)
                s = tabAnket.AnketCevapAnaSil(new AnketBilgi() { cevapAnahtar = hibeKod.ToString() });

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static bool CevapYetkisiVarMi(Kullanici kullanan, AnketBilgi anketBilgi)
        {
            if (!string.IsNullOrEmpty(anketBilgi.mailAdres) && !string.IsNullOrEmpty(anketBilgi.onayKod))
            {
                Sonuc s = AnketDavetiGecerliMi(anketBilgi.modul, anketBilgi.anketAnahtar, anketBilgi.orgBirimKod, anketBilgi.cevapAnahtar, anketBilgi.mailAdres, anketBilgi.onayKod);
                return s.islemSonuc;
            }
            if (kullanan == null) return false;
            return true;
        }

        internal static Sonuc AnketDavetiGonder(VeriErisim veriErisim, Kullanici kullanan, string modul, string anketAnahtar, int orgBirimKod, string cevapAnahtar, string mailAdres)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (OrtakFonksiyonlar.MailAdresiDogrumu(mailAdres))
            {
                string onayKod = OnayKodBul(modul, anketAnahtar, orgBirimKod, cevapAnahtar, mailAdres);

                string yazilimMailAdresi = OrtakFonksiyonlar.WebConfigOku("yazilimMailAdresi", "cevapyok@sgb.gov.tr");
                string yazilimURL = OrtakFonksiyonlar.WebConfigOku("yazilimURL", "http://localhost/SGB.Netv2");
                string adres = string.Format(yazilimURL + "/KYM/AnketCevap.aspx?modul={0}&anahtar={1}&orgBirimKod={2}&cevapAnahtar={3}&mailAdres={4}&onayKod={5}", modul, anketAnahtar, orgBirimKod, cevapAnahtar, mailAdres, onayKod);
                string bilgi = "İdaremizin daha iyi hizmet vermesi amacıyla, paydaşlarımızın fikrini almak için düzenlediğimiz anketi doldurmamızını talep etmekteyiz. <a href='" + adres + "'>Anketi doldurmak için tıklayınız</a>.";
                string tut = OrtakFonksiyonlar.MailAt(yazilimMailAdresi, mailAdres, "Anket girişi daveti", bilgi, true, false, null);
                if (tut == "1")
                {
                    tut = mailAdres + " e-Posta adresine anket daveti gönderilmiştir.";
                }
                s.DegerAta(tut);
            }
            else
                s.DegerAta("Hatalı mail adresi:" + mailAdres);
            return s;
        }

        private static string OnayKodBul(string modul, string anketAnahtar, int orgBirimKod, string cevapAnahtar, string mailAdres)
        {
            return TNS.KYM.Yetki.Sifrele(modul + "_" + anketAnahtar + "_" + orgBirimKod + "_" + cevapAnahtar + "_" + mailAdres + "_Anket");
        }

        internal static Sonuc AnketDavetiGecerliMi(string modul, string anketAnahtar, int orgBirimKod, string cevapAnahtar, string mailAdres, string onayKod)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (OnayKodBul(modul, anketAnahtar, orgBirimKod, cevapAnahtar, mailAdres) != onayKod)
                s.DegerAta("Hatalı parametre");
            return s;
        }

        private static bool AnketCevaplanmisMi(TABANKET tabAnket, AnketBilgi anketBilgi)
        {
            return false; //???
        }

        private static void SoruAnketteVarMi(TABANKET tabAnket, Sonuc s)
        {
            //???
        }

        internal static AnketBilgi AnketIstatistikBul(VeriErisim veriErisim, Kullanici kullanan, AnketBilgi anketKistasBilgi)
        {
            string kopruKod = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            IDataReader dr = tabAnket.AnketBilgiOku(anketKistasBilgi);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            AnketBilgi anketBilgi = null;
            if (dr.Read())
            {
                anketBilgi = new AnketBilgi();
                anketBilgi.modul = anketKistasBilgi.modul;
                anketBilgi.anketAnahtar = anketKistasBilgi.anketAnahtar;
                anketBilgi.anketAd = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.ANKETAD, "");
                anketBilgi.baslaTarih = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.BASLATARIH);
                anketBilgi.bitisTarih = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.BITISTARIH);
                anketBilgi.tekGirislik = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.TEKGIRISLIK, 0) == 1 ? true : false;
            }
            dr.Close();

            if (anketBilgi == null)
            {
                ve.VeriErisimKapat();
                return null;
            }

            ArrayList al = new ArrayList();

            AnketSoru kistasSoru = null;
            if (anketKistasBilgi.sorular != null && anketKistasBilgi.sorular.Length == 1)
                kistasSoru = anketKistasBilgi.sorular[0];

            dr = tabAnket.AnketSorulariListele(anketKistasBilgi, kistasSoru);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            while (dr.Read())
            {
                AnketSoru anketSoru = new AnketSoru();
                anketSoru.prSiraNo = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.PRSIRANO, 0);
                anketSoru.soruMetin = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.METIN, "");
                anketSoru.secenekTur = (ENUMSecenekTur)VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.SECENEKTUR, 0);
                anketSoru.anahtar1 = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.ANAHTAR1, "");
                anketSoru.anahtar2 = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.ANAHTAR2, "");
                anketSoru.dogruCevapSecenek = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.CEVAPSECENEK, 0);
                anketSoru.durum = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.DURUM, 0);
                anketSoru.soruSira = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.DURUM + 1, 0);
                anketSoru.soruSeviye = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.DURUM + 2, 0);
                if (anketSoru.secenekTur != ENUMSecenekTur.METINCEVAP)
                    al.Add(anketSoru);
            }
            dr.Close();

            foreach (AnketSoru soru in al)
            {
                dr = tabAnket.AnketIstatistikOku(anketKistasBilgi, soru, kopruKod);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }

                ArrayList alIc = new ArrayList();
                while (dr.Read())
                {
                    CevapSecenekler detay = new CevapSecenekler();
                    detay.secenekMetin = VeriErisim.DegerAl(dr, 0, "");
                    detay.cevapSay = VeriErisim.DegerAl(dr, 1, 0);
                    alIc.Add(detay);
                }
                soru.cevapSecenekler = (CevapSecenekler[])alIc.ToArray(typeof(CevapSecenekler));
                dr.Close();
            }

            anketBilgi.sorular = (AnketSoru[])al.ToArray(typeof(AnketSoru));

            ve.VeriErisimKapat();
            return anketBilgi;
        }

        internal static AnketCaprazBilgi AnketCaprazIstatistikBul(VeriErisim veriErisim, Kullanici kullanan, AnketBilgi anketKistasBilgi)
        {
            string kopruKod = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABANKET tabAnket = new TABANKET(ve);

            if (anketKistasBilgi.sorular == null || anketKistasBilgi.sorular.Length != 2)
                throw new Exception("Çapraz tablo almak için iki adet soru seçiniz!");

            IDataReader dr = tabAnket.AnketBilgiOku(anketKistasBilgi);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            AnketBilgi anketBilgi = null;
            if (dr.Read())
            {
                anketBilgi = new AnketBilgi();
                anketBilgi.modul = anketKistasBilgi.modul;
                anketBilgi.anketAnahtar = anketKistasBilgi.anketAnahtar;
                anketBilgi.anketAd = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.ANKETAD, "");
                anketBilgi.baslaTarih = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.BASLATARIH);
                anketBilgi.bitisTarih = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.BITISTARIH);
                anketBilgi.tekGirislik = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarANKET.TEKGIRISLIK, 0) == 1 ? true : false;
            }
            dr.Close();

            if (anketBilgi == null)
            {
                ve.VeriErisimKapat();
                return null;
            }

            AnketSoru kistasSoru = null;
            if (anketKistasBilgi.sorular != null && anketKistasBilgi.sorular.Length == 1)
                kistasSoru = anketKistasBilgi.sorular[0];

            AnketCaprazBilgi acb = new AnketCaprazBilgi();
            acb.anketAd = anketBilgi.anketAd;

            for (int i = 0; i < 2; i++)
            {
                acb.sorular[i] = null;
                dr = tabAnket.AnketSorulariListele(anketKistasBilgi, anketKistasBilgi.sorular[i]);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }
                if (dr.Read())
                {
                    AnketSoru anketSoru = new AnketSoru();
                    anketSoru.prSiraNo = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.PRSIRANO, 0);
                    anketSoru.soruMetin = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.METIN, "");
                    anketSoru.secenekTur = (ENUMSecenekTur)VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.SECENEKTUR, 0);
                    anketSoru.anahtar1 = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.ANAHTAR1, "");
                    anketSoru.anahtar2 = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.ANAHTAR2, "");
                    anketSoru.dogruCevapSecenek = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.CEVAPSECENEK, 0);
                    anketSoru.durum = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.DURUM, 0);
                    anketSoru.soruSira = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.DURUM + 1, 0);
                    anketSoru.soruSeviye = VeriErisim.DegerAl(dr, (int)TABANKET.ENUMAlanlarSORU.DURUM + 2, 0);
                    acb.sorular[i] = anketSoru;
                }
                dr.Close();
            }

            for (int i = 0; i < 2; i++)
            {
                if (acb.sorular[i] == null)
                {
                    throw new Exception("Soru bilgisi okunamadı!");
                }
                dr = tabAnket.AnketIstatistikOku(anketKistasBilgi, acb.sorular[i], kopruKod);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }

                ArrayList alIc = new ArrayList();
                while (dr.Read())
                {
                    CevapSecenekler detay = new CevapSecenekler();
                    detay.secenekMetin = VeriErisim.DegerAl(dr, 0, "");
                    detay.cevapSay = VeriErisim.DegerAl(dr, 1, 0);
                    alIc.Add(detay);
                }
                acb.sorular[i].cevapSecenekler = (CevapSecenekler[])alIc.ToArray(typeof(CevapSecenekler));
                dr.Close();
            }

            if (acb.sorular[0].cevapSecenekler == null || acb.sorular[0].cevapSecenekler.Length == 0)
            {
                ve.VeriErisimKapat();
                throw new Exception("Soru bilgisi okunamadı!");
            }
            if (acb.sorular[1].cevapSecenekler == null || acb.sorular[1].cevapSecenekler.Length == 0)
            {
                ve.VeriErisimKapat();
                throw new Exception("Soru bilgisi okunamadı!");
            }

            bool tasti = false;
            acb.toplamlar = new int[acb.sorular[0].cevapSecenekler.Length, acb.sorular[1].cevapSecenekler.Length + 1];
            for (int i = 0; i < acb.sorular[0].cevapSecenekler.Length; i++)
            {
                for (int j = 0; j < acb.sorular[1].cevapSecenekler.Length + 1; j++)
                {
                    acb.toplamlar[i, j] = 0;
                }

                dr = tabAnket.AnketCaprazIstatistikOku(anketKistasBilgi, acb.sorular[1], kopruKod, acb.sorular[0], acb.sorular[0].cevapSecenekler[i]);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        int yer = CevapMetinyerBul(acb.sorular[1].cevapSecenekler, VeriErisim.DegerAl(dr, 0, ""));
                        if (yer < 0)
                        {
                            tasti = true;
                            yer = acb.sorular[1].cevapSecenekler.Length + 1;
                        }
                        acb.toplamlar[i, yer] = VeriErisim.DegerAl(dr, 1, 0);
                    }
                    dr.Close();
                }
            }

            acb.tasti = tasti;

            ve.VeriErisimKapat();

            return acb;
        }

        private static int CevapMetinyerBul(CevapSecenekler[] cevapSecenekler, string cevapMetin)
        {
            if (cevapSecenekler == null) return -1;
            for (int i = 0; i < cevapSecenekler.Length; i++)
            {
                if (cevapSecenekler[i].secenekMetin == cevapMetin)
                    return i;
            }
            return -1;
        }
    }
}

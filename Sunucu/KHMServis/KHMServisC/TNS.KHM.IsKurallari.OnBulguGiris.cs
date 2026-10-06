using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KHM;
using TNS.KHM.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BHMv2;
using System.Data.SqlTypes;
using System.Collections.Generic;

namespace TNS.KHM.IsKurallari
{
    /// <summary>
    /// KHMOnBulguGiris formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class KHMOnBulguGiris
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriKHM";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int saymanlik, string muhKod, Sonuc sonuc)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, SayBulgu form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// OnBulgu nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBulgu yapısında OnBulgu:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, SayBulgu form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YetkisiVarMi(kullanan, "", "", ENUMOGY.Yazma, s))
            {
                return s;
            }
            form.KontrolEt(s);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKHMONBULGU tabKHMONBULGU = new TABKHMONBULGU(ve);

            ve.BeginTransaction();

            if (form.bulguID > 0)
            {
                s = tabKHMONBULGU.Guncelle(form);
                s.anahtar = form.bulguID.ToString();
            }
            else
                s = tabKHMONBULGU.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// OnBulgu nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBulgu yapısında OnBulgu:  bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, SayBulgu form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, SayBulgu form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// OnBulgu listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBulgu yapısında OnBulgu:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, SayBulgu form)
        {
            ObjectArray oa = new ObjectArray();
            if (!YetkisiVarMi(kullanan, "", "", ENUMOGY.Gorme, oa.sonuc))
            {
                oa.sonuc.DegerAta("Yetkiniz yok");
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKHMONBULGU tabKHMONBULGU = new TABKHMONBULGU(ve);

            IDataReader dr;
            dr = tabKHMONBULGU.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                SayBulgu detay = new SayBulgu();
                detay.bulguID = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.BULGUID, 0);
                detay.yil = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.YIL, 0);
                detay.bitisYil = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.BITISYIL, 0);
                detay.butceTurKod = (enumBulguButceTur)VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.BUTCETURKOD, 0);
                detay.bulguTurKod = (enumBulguTur)VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.BULGUTURKOD, 0);
                detay.bulguNo = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.BULGUNO, "");
                detay.bulguBasligi = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.BULGU, "");
                detay.bulguOzeti = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.BULGUOZETI, "");
                detay.bulguDayanagi = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.BULGUDAYANAGI, "");
                detay.idareCevabi = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.IDARECEVABI, "");
                detay.idareCevapOzeti = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.IDARECEVAPOZETI, "");
                detay.sayistayOnerisi = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.SAYISTAYONERISI, "");
                detay.durumKod = (enumBulguDurum)VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.DURUMKOD, 0);

                detay.yerineGetirildi = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.YERINEGETIRILDI, 0) == 1;
                detay.yapilacakFaaliyetler = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.YAPILACAKFAALIYETLER, "");
                detay.ilgiliBirimKurum = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.ILGILIBIRIMKURUM, "");
                detay.gorevOrtagi = VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.GOREVORTAGI, "");
                detay.ongorulenTamamlanmaTarihi.Yaz(VeriErisim.DegerAl(dr, (int)TABKHMONBULGU.ENUMAlanlar.ONGORULENTAMAMLANMATARIHI));

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        private static bool YetkisiVarMi(Kullanici kullanan, string saymanlikKod, string birimKod, ENUMOGY ogy, Sonuc sonuc)
        {
            return true;
        }

        internal static Sonuc BulguBirimBagKur(VeriErisim veriErisim, Kullanici kullanan, int onBulguId, TNS.MUH.MuhasebeBirim muhasebeBirim)
        {
            Sonuc s = new Sonuc();
            if (!YetkisiVarMi(kullanan, "", "", ENUMOGY.Yazma, s))
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKHMONBULGU tabKHMONBULGU = new TABKHMONBULGU(ve);

            s = tabKHMONBULGU.EkleBirim(onBulguId, muhasebeBirim);
            if (!s.islemSonuc)
                s.hataStr = "Kayıt var olduğundan eklenemedi!";
            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// OnBulgu listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBulgu yapısında OnBulgu:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleBulguBirim(VeriErisim veriErisim, Kullanici kullanan, int bulguId)
        {
            ObjectArray oa = new ObjectArray();
            if (!YetkisiVarMi(kullanan, "", "", ENUMOGY.Gorme, oa.sonuc))
            {
                oa.sonuc.DegerAta("Yetkiniz yok");
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKHMONBULGU tabKHMONBULGU = new TABKHMONBULGU(ve);

            IDataReader dr;
            dr = tabKHMONBULGU.ListeleBirim(bulguId);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                TNS.MUH.MuhasebeBirim detay = new TNS.MUH.MuhasebeBirim();
                int kol = 0;
                int icBulguID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.muhasebe = VeriErisim.DegerAl(dr, kol++, 0);
                detay.birimKod = VeriErisim.DegerAl(dr, kol++, "");
                detay.kurum = OrtakFonksiyonlar.KodNoktaKoy("9999.9999", VeriErisim.DegerAl(dr, kol++, ""));
                detay.muhasebeAd = VeriErisim.DegerAl(dr, kol++, "");
                detay.birimAd = VeriErisim.DegerAl(dr, kol++, "");
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc BulguBirimBagSil(VeriErisim veriErisim, Kullanici kullanan, int onBulguId, MUH.MuhasebeBirim muhasebeBirim)
        {
            Sonuc s = new Sonuc();
            if (!YetkisiVarMi(kullanan, "", "", ENUMOGY.Yazma, s))
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKHMONBULGU tabKHMONBULGU = new TABKHMONBULGU(ve);

            s = tabKHMONBULGU.SilBirim(onBulguId, muhasebeBirim);
            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectArray OnBulguCevapListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string birimKod)
        {
            ObjectArray oa = new ObjectArray();
            TNS.MUH.MuhasebeBirim muhBirim = new TNS.MUH.MuhasebeBirim();
            muhBirim.MuhasebeBirimKurum = birimKod;
            if (!YetkisiVarMi(kullanan, muhBirim.muhasebe.ToString(), muhBirim.birimKod, ENUMOGY.Yazma, oa.sonuc))
            {
                return oa;
            }

            if (birimKod == null)
            {
                return ListeleSadeceCevap(veriErisim, kullanan, yil);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKHMONBULGU tabKHMONBULGU = new TABKHMONBULGU(ve);

            IDataReader dr;
            dr = tabKHMONBULGU.ListeleOnBulguCevap(yil, muhBirim.muhasebe, muhBirim.birimKod, muhBirim.kurum);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                SayBulgu detay = new SayBulgu();
                int kol = 0;
                detay.bulguID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.yil = VeriErisim.DegerAl(dr, kol++, 0);
                detay.butceTurKod = (enumBulguButceTur)VeriErisim.DegerAl(dr, kol++, 0);
                detay.bulguTurKod = (enumBulguTur)VeriErisim.DegerAl(dr, kol++, 0);
                detay.bulguNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.bulguBasligi = VeriErisim.DegerAl(dr, kol++, "");
                detay.bulguOzeti = VeriErisim.DegerAl(dr, kol++, "");
                detay.bulguDayanagi = VeriErisim.DegerAl(dr, kol++, "");
                detay.idareCevabi = VeriErisim.DegerAl(dr, kol++, "");
                detay.idareCevapOzeti = VeriErisim.DegerAl(dr, kol++, "");
                detay.sayistayOnerisi = VeriErisim.DegerAl(dr, kol++, "");
                detay.durumKod = (enumBulguDurum)VeriErisim.DegerAl(dr, kol++, 0);

                detay.birimKod = VeriErisim.DegerAl(dr, kol++, "");
                detay.muhasebeKod = VeriErisim.DegerAl(dr, kol++, 0);
                detay.kurKod = VeriErisim.DegerAl(dr, kol++, "");
                detay.birimCevap = VeriErisim.DegerAl(dr, kol++, "");
                detay.birimIsler = VeriErisim.DegerAl(dr, kol++, "");
                detay.yerineGetirildiCevap = VeriErisim.DegerAl(dr, kol++, 0) == 1;
                detay.ongorulenTamamlanmaTarihiCevap.Yaz(VeriErisim.DegerAl(dr, kol++));

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        private static ObjectArray ListeleSadeceCevap(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKHMONBULGU tabKHMONBULGU = new TABKHMONBULGU(ve);

            IDataReader dr;
            dr = tabKHMONBULGU.ListeleOnBulguSadeceCevap(yil);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                SayBulgu detay = new SayBulgu();
                int kol = 0;
                detay.bulguID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.yil = VeriErisim.DegerAl(dr, kol++, 0);
                detay.birimCevap = VeriErisim.DegerAl(dr, kol++, "");
                detay.birimIsler = VeriErisim.DegerAl(dr, kol++, "");

                detay.yerineGetirildiCevap = VeriErisim.DegerAl(dr, kol++, 0) == 1;
                detay.ongorulenTamamlanmaTarihiCevap.Yaz(VeriErisim.DegerAl(dr, kol++));

                detay.birimKod = VeriErisim.DegerAl(dr, kol++, "");
                detay.muhasebeKod = VeriErisim.DegerAl(dr, kol++, 0);
                detay.kurKod = VeriErisim.DegerAl(dr, kol++, "");
                string muhasebeAd = VeriErisim.DegerAl(dr, kol++, "");
                string birimAd = VeriErisim.DegerAl(dr, kol++, "");
                detay.muhasebeBirimAd = TNS.MUH.Arac.BirimAdOl(muhasebeAd, birimAd);

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc OnBulguCevapKaydet(VeriErisim veriErisim, Kullanici kullanan, SayBulgu onBulgu)
        {
            Sonuc s = new Sonuc();
            if (string.IsNullOrWhiteSpace(onBulgu.birimCevap)) s.DegerAta("Birim cevabı girilmelidir!");
            if (string.IsNullOrWhiteSpace(onBulgu.birimIsler)) s.DegerAta("Birimin yapacağı işler girilmelidir!");
            if (onBulgu.bulguID <= 0) s.DegerAta("İlgili ön bulgu seçilmelidir!");
            if (onBulgu.muhasebeKod <= 0) s.DegerAta("Bulgu biriminin muhasebe kodu seçilmelidir!");
            if (onBulgu.birimKod.Length != 3) s.DegerAta("Bulgu biriminin kodu seçilmelidir!");
            if (string.IsNullOrWhiteSpace(onBulgu.kurKod)) s.DegerAta("Bulgu biriminin kurum kodu seçilmelidir!");
            if (!s.islemSonuc) return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectVAN ov = Oku(ve, kullanan, onBulgu);
            SayBulgu okunan = null;
            if (ov.sonuc.islemSonuc && ov.o != null)
                okunan = (SayBulgu)ov.o;
            else
            {
                if (!ov.sonuc.islemSonuc)
                    s = ov.sonuc;
                else
                    s.DegerAta("Ön bulgu bulunamadı!");
                ve.VeriErisimKapat();
                return s;
            }

            YazmaYetkisiVarMi(kullanan, onBulgu.muhasebeKod, onBulgu.birimKod, s);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            s = BirimAcikMi(ve, kullanan, okunan.yil, 1, onBulgu.muhasebeKod, onBulgu.birimKod, onBulgu.kurKod);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABKHMONBULGU tabKHMONBULGU = new TABKHMONBULGU(ve);

            try
            {
                ve.BeginTransaction();
                s = tabKHMONBULGU.CevapKaydet(onBulgu);
            }
            catch (Exception e)
            {
                s.DegerAta(e.Message);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        private static Sonuc BirimAcikMi(VeriErisim veriErisim, Kullanici kullanan, int yil, int tur, int muhasebeKod, string birimKod, string kurKod)
        {

            ObjectArray oa = BirimGirisIzniListele(veriErisim, kullanan, yil, tur, muhasebeKod, birimKod, kurKod);
            if (!oa.sonuc.islemSonuc)
                return oa.sonuc;
            Sonuc sonuc = new Sonuc();
            bool izin = true;
            if (oa.ObjeSayisi != 1)
                izin = false;
            else
            {
                BirimGirisIzni bi = (BirimGirisIzni)oa.objeler[0];
                izin = bi.girisIzni;
            }
            if (!izin)
                sonuc.DegerAta($"Cevap girmek istediğiniz birim {yil} yılı, {((tur == 1) ? "ön bulgu" : "bulgu")} girişine kapalı!");
            return sonuc;
        }

        internal static ObjectArray BirimGirisIzniListele(VeriErisim veriErisim, Kullanici kullanan, int yil, int tur, int muhasebeKod = 0, string birimKod = "", string kurKod = "")
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKHMONBULGU tabKHMONBULGU = new TABKHMONBULGU(ve);

            IDataReader dr;
            dr = tabKHMONBULGU.ListeleBirimVeriGirisDurum(yil, tur, muhasebeKod, birimKod, kurKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                BirimGirisIzni bi = new BirimGirisIzni();
                int kol = 0;
                bi.birim.muhasebe = VeriErisim.DegerAl(dr, kol++, 0);
                bi.birim.birimKod = VeriErisim.DegerAl(dr, kol++, "");
                bi.birim.kurum = VeriErisim.DegerAl(dr, kol++, "");
                bi.birim.muhasebeAd = VeriErisim.DegerAl(dr, kol++, "");
                bi.birim.birimAd = VeriErisim.DegerAl(dr, kol++, "");
                bi.girisIzni = VeriErisim.DegerAl(dr, kol++, 0) == 1;
                oa.Ekle(bi);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc BirimGirisIzniKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, int tur, bool izin, TNSCollection birimler)
        {
            Sonuc s = new Sonuc();
            YazmaYetkisiVarMi(kullanan, 0, "", s);
            if (birimler == null || birimler.Count == 0)
                s.DegerAta("Birim seçmelisiniz!");

            if (!s.islemSonuc)
            {
                return s;
            }

            int sat = 1;
            foreach (MUH.MuhasebeBirim item in birimler)
            {
                if (item.muhasebe <= 0) s.DegerAta($"{sat}. satırda birimin muhasebe kodu seçilmelidir!");
                if (item.birimKod.Length != 3) s.DegerAta($"{sat}. satırda birimin kodu seçilmelidir!");
                if (string.IsNullOrWhiteSpace(item.kurum)) s.DegerAta($"{sat}. satırda birimin kurum kodu seçilmelidir!");
                sat++;
            }
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKHMONBULGU tabKHMONBULGU = new TABKHMONBULGU(ve);

            try
            {
                ve.BeginTransaction();
                s = tabKHMONBULGU.IzinSil(yil, tur, birimler);
                if (s.islemSonuc && izin)
                {
                    s = tabKHMONBULGU.IzinEkle(yil, tur, birimler);
                }
            }
            catch (Exception e)
            {
                s.DegerAta(e.Message);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }
    }
}

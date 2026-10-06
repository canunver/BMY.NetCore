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
    /// KYMBirimMesajlari formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class KYMBirimMesajlari
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriKYM";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, BirimMesaj form)
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
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, BirimMesaj form)
        {
            TNS.UZY.IUZYServis servisUzy = TNS.UZY.Arac.Tanimla();
            Sonuc s = new Sonuc(true, "", "");
            if (form.saymanlikKod <= 0)
                s.DegerAta("Lütfen saymanlık giriniz!");
            else if (string.IsNullOrWhiteSpace(form.birimKod) || string.IsNullOrWhiteSpace(form.kurKod))
                s.DegerAta("Lütfen birim giriniz!");
            else
            {
                form.birimAd = servisUzy.UzayDegeriStr(kullanan, "MUHMUHASEBEBIRIM", form.birimKod + "-" + form.saymanlikKod, true);
                if (string.IsNullOrWhiteSpace(form.birimAd))
                {
                    s.DegerAta("Lütfen geçerli bir saymanlık ve birim giriniz!");
                }
            }
            if (string.IsNullOrWhiteSpace(form.sayfa))
                s.DegerAta("Lütfen ilgili sayfayı seçiniz!");
            if (string.IsNullOrWhiteSpace(form.mesaj.Trim()))
                s.DegerAta("Lütfen mesaj giriniz!");
            return s;
        }

        /// <summary>
        /// BirimMesajAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimMesajAna yapısında BirimMesajAna: Birim Mesaj Ana bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimMesaj form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma yetkiniz bulunmamaktadır!");
                return s;
            }

            form.kurKod = TNS.DEG.Arac.VarsayilanKurumBul();
            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKYMBIRIMMESAJ tabKYMBIRIMMESAJ = new TABKYMBIRIMMESAJ(ve);

            ve.BeginTransaction();
            form.kayitTarihi = new TNSDateTime(DateTime.Now);
            form.kaydeden = kullanan.KullaniciKodu;
            form.karsilamaTarihi = null;
            form.karsilayan = null;
            if (form.durum != BirimMesaj.MesajDurum.Yayinda)
                form.durum = BirimMesaj.MesajDurum.Yeni;

            if (form.siraNo == 0)
            {
                form.siraNo = tabKYMBIRIMMESAJ.SiraNoBul(form);
                if (string.IsNullOrWhiteSpace(ve.hata))
                    s.DegerAta(ve.hata);
                if (s.islemSonuc)
                    s = tabKYMBIRIMMESAJ.Ekle(form);
            }
            else
                s = tabKYMBIRIMMESAJ.Guncelle(form);


            if (s.islemSonuc)
            {
                s.anahtar = form.saymanlikKod + ";" + form.birimKod + ";" + form.kurKod + ";" + form.siraNo;
                ve.Commit();
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
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, BirimMesaj form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static bool AnahtarAlanlarGecerliMi(Kullanici kullanan, BirimMesaj form)
        {
            if (form.saymanlikKod <= 0) return false;
            if (string.IsNullOrWhiteSpace(form.birimKod)) return false;
            if (string.IsNullOrWhiteSpace(form.kurKod)) return false;
            if (form.siraNo <= 0) return false;
            return true;
        }

        internal static int MesajSayisiBul(VeriErisim veriErisim, Kullanici kullanan, BirimMesaj form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKYMBIRIMMESAJ tabKYMBIRIMMESAJ = new TABKYMBIRIMMESAJ(ve);
            int a = tabKYMBIRIMMESAJ.MesajSayisi(form);
            ve.VeriErisimKapat();
            return a;
        }

        /// <summary>
        /// BirimMesajAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimMesajAna yapısında BirimMesajAna: Birim Mesaj Ana bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, BirimMesaj form)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                oa.sonuc.DegerAta("Görme yetkiniz yok!");
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKYMBIRIMMESAJ tabKYMBIRIMMESAJ = new TABKYMBIRIMMESAJ(ve);

            IDataReader dr = tabKYMBIRIMMESAJ.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                BirimMesaj detay = new BirimMesaj();
                detay.saymanlikKod = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.SAYMANLIKKOD, 0);
                detay.birimKod = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.BIRIMKOD, "");
                detay.kurKod = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.KURUMKOD, "");
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.SIRANO, 0);
                detay.mesaj = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.MESAJ, "");
                detay.kaydeden = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.KAYDEDEN, "");
                detay.kayitTarihi = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.KAYITTARIHI);
                detay.karsilayan = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.KARSILAYAN, "");
                detay.karsilamaTarihi = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.KARSILAMATARIHI);
                detay.sayfa = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.SAYFA, "");
                detay.durum = (BirimMesaj.MesajDurum)VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.DURUM, 0);

                detay.saymanlikAd = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.DURUM + 1, "");
                detay.birimAd = VeriErisim.DegerAl(dr, (int)TABKYMBIRIMMESAJ.ENUMAlanlar.DURUM + 2, "");
                oa.Ekle(detay);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc DurumDegistir(VeriErisim veriErisim, Kullanici kullanan, TNSCollection mesajlar)
        {
            Sonuc s = new Sonuc(true, "", "");

            int oncSay = 0;
            string oncBir = "";
            foreach (BirimMesaj item in mesajlar)
            {
                if (!AnahtarAlanlarGecerliMi(kullanan, item))
                {
                    s.DegerAta("Hatalı kayıt geldi!");
                    return s;
                }
                if (item.saymanlikKod != oncSay && item.birimKod != oncBir)
                {
                    if (!DurumYetkisiVarMi(kullanan, item))
                    {
                        s.DegerAta("Yetkiniz bulunmamaktadır!");
                        return s;
                    }
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKYMBIRIMMESAJ tabKYMBIRIMMESAJ = new TABKYMBIRIMMESAJ(ve);

            ve.BeginTransaction();
            foreach (BirimMesaj form in mesajlar)
            {
                form.karsilamaTarihi = new TNSDateTime(DateTime.Now);
                form.karsilayan = kullanan.kullaniciKodu;
                s = tabKYMBIRIMMESAJ.DurumAta(form);
                if (!s.islemSonuc) break;
            }            

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static bool DurumYetkisiVarMi(Kullanici kullanan, BirimMesaj item)
        {
            return true;
        }
    }
}

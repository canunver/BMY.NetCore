using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
using TNS.BHF.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// BHFTasitMarkaModel formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHFTasitMarkaModel
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHF";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, TasitMarkaModel form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, TasitMarkaModel form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrWhiteSpace(form.model))
                s.DegerAta("Model alanını girmelisiniz");
            if (string.IsNullOrWhiteSpace(form.marka))
                s.DegerAta("Marka alanını girmelisiniz");
            return s;
        }

        /// <summary>
        /// TasitMarkaModel nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitMarkaModel yapısında TasitMarkaModel: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasitMarkaModel form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(ButceFormResI.TasitMarkaModel001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITMARKAMODEL tabBHFTASITMARKAMODEL = new TABBHFTASITMARKAMODEL(ve);

            ve.BeginTransaction();

            if (form.kod == 0)
            {
                form.kod = tabBHFTASITMARKAMODEL.EnBuyukNo();
            }
            else
            {
                if (s.islemSonuc)
                    s = tabBHFTASITMARKAMODEL.Sil(form);
            }
            if (s.islemSonuc)
                s = tabBHFTASITMARKAMODEL.Ekle(form);

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.kod.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// TasitMarkaModel nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitMarkaModel yapısında TasitMarkaModel: İstatistik Kodu bilgilerini tutan değişken</param>
        public static TasitMarkaModel Oku(VeriErisim veriErisim, Kullanici kullanan, TasitMarkaModel form)
        {
            ObjectArray s = Listele(veriErisim, kullanan, form);
            if (s == null || s.ObjeSayisi == 0) return null;
            return (TasitMarkaModel)s.objeler[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, TasitMarkaModel form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, TasitMarkaModel form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// TasitMarkaModel listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitMarkaModel yapısında TasitMarkaModel: İstatistik Kodu bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TasitMarkaModel form)
        {
            ObjectArray alAna = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                alAna.sonuc.DegerAta(ButceFormResI.TasitMarkaModel002);
                return alAna;
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                alAna.sonuc.DegerAta(s.hataStr);
                return alAna;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITMARKAMODEL tabBHFTASITMARKAMODEL = new TABBHFTASITMARKAMODEL(ve);

            IDataReader dr = tabBHFTASITMARKAMODEL.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                alAna.sonuc.DegerAta(ve.hata);
                return alAna;
            }
            while (dr.Read())
            {
                TasitMarkaModel detay = new TasitMarkaModel();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABBHFTASITMARKAMODEL.ENUMAlanlar.KOD, 0);
                detay.marka = VeriErisim.DegerAl(dr, (int)TABBHFTASITMARKAMODEL.ENUMAlanlar.MARKA, "");
                detay.model = VeriErisim.DegerAl(dr, (int)TABBHFTASITMARKAMODEL.ENUMAlanlar.MODEL, "");
                detay.tipKodu = VeriErisim.DegerAl(dr, (int)TABBHFTASITMARKAMODEL.ENUMAlanlar.TIPKODU, 0);
                detay.kesin = VeriErisim.DegerAl(dr, (int)TABBHFTASITMARKAMODEL.ENUMAlanlar.KESIN, 0);
                detay.tipAdi = VeriErisim.DegerAl(dr, 5, "");
                alAna.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return alAna;
        }

        internal static ObjectArray ListeleMarkaModel(VeriErisim veriErisim, Kullanici kullanan, TasitMarkaModel form)
        {
            ObjectArray alAna = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                alAna.sonuc.DegerAta(ButceFormResI.TasitMarkaModel002);
                return alAna;
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                alAna.sonuc.DegerAta(s.hataStr);
                return alAna;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITMARKAMODEL tabBHFTASITMARKAMODEL = new TABBHFTASITMARKAMODEL(ve);

            IDataReader dr = tabBHFTASITMARKAMODEL.ListeleMarkaModel(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                alAna.sonuc.DegerAta(ve.hata);
                return alAna;
            }
            while (dr.Read())
            {
                TasitMarkaModel detay = new TasitMarkaModel();
                if (!string.IsNullOrWhiteSpace(form.marka))
                {
                    detay.kod = VeriErisim.DegerAl(dr, 0, 0);
                    detay.model = VeriErisim.DegerAl(dr, 1, "");
                    detay.kesin = VeriErisim.DegerAl(dr, 2, 0);
                }
                else
                    detay.marka = VeriErisim.DegerAl(dr, 0, "");

                alAna.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return alAna;
        }

        internal static Sonuc TipGuncelle(VeriErisim veriErisim, Kullanici kullanan, string tipAd, string marka, string modelBasla)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta(ButceFormResI.TasitMarkaModel001);
            }

            if(string.IsNullOrWhiteSpace(marka) || string.IsNullOrWhiteSpace(tipAd) || string.IsNullOrWhiteSpace(modelBasla))
            {
                s.DegerAta("tip adı, marka ve modelin başladığı harfler dolu olmalı!");
            }

            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITMARKAMODEL tabBHFTASITMARKAMODEL = new TABBHFTASITMARKAMODEL(ve);

            TasitTip tipForm = new TasitTip();
            tipForm.ad=tipAd;
            ObjectArray oa = BHFTasitTip.Listele(null, kullanan, tipForm);
            if (oa.sonuc.islemSonuc)
            {
                if (oa.ObjeSayisi == 1)
                {
                    tipForm = (TasitTip)oa.objeler[0];
                    if (s.islemSonuc)
                        s = tabBHFTASITMARKAMODEL.TipGuncelle(tipForm.kod, marka, modelBasla);
                }
                else
                    s.DegerAta("Araç tipi bulunamadı!");
            }
            else
                s = oa.sonuc;

            ve.VeriErisimKapat();
            return s;
        }
    }
}

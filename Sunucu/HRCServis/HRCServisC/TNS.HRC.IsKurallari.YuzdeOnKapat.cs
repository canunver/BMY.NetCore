using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// HRCYuzdeOnKapat formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCYuzdeOnKapat
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
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
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, YuzdeOnKapat form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrWhiteSpace(form.malHizmetYapim)) s.DegerAta("Mal/Hizmet/Yapım seçiniz.");
            if (string.IsNullOrWhiteSpace(form.aciklama)) s.DegerAta("Açıklama giriniz.");
            if (form.kapali == 0 && form.talepNo != 0) s.DegerAta("Veri girişine açıldığında talep numarasını sıfırlayınız!");
            if (form.kapali == 1 && (form.talepNo < -1 || form.talepNo > 3)) s.DegerAta("Veri girişine kapatıldığında talep numarasını -1 ile 3 arasında giriniz!");

            return s;
        }

        /// <summary>
        /// YuzdeOnKapatAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YuzdeOnKapatAna yapısında YuzdeOnKapatAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, YuzdeOnKapat form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta(HarcamaSurecleriResI.YuzdeOnKapat001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCYUZDEONKAPAT tabHRCYUZDEONKAPAT = new TABHRCYUZDEONKAPAT(ve);

            ve.BeginTransaction();

            s = tabHRCYUZDEONKAPAT.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// YuzdeOnKapatAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YuzdeOnKapatAna yapısında YuzdeOnKapatAna:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, YuzdeOnKapat form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCYUZDEONKAPAT tabHRCYUZDEONKAPAT = new TABHRCYUZDEONKAPAT(ve);

            ObjectArray al = new ObjectArray();
            IDataReader dr = tabHRCYUZDEONKAPAT.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    YuzdeOnKapat detay = new YuzdeOnKapat();
                    detay.yili = VeriErisim.DegerAl(dr, (int)TABHRCYUZDEONKAPAT.ENUMAlanlar.YILI, 0);
                    detay.malHizmetYapim = VeriErisim.DegerAl(dr, (int)TABHRCYUZDEONKAPAT.ENUMAlanlar.MALHIZMETYAPIM, "");
                    detay.tarih = VeriErisim.DegerAl(dr, (int)TABHRCYUZDEONKAPAT.ENUMAlanlar.TARIH);
                    detay.aciklama = VeriErisim.DegerAl(dr, (int)TABHRCYUZDEONKAPAT.ENUMAlanlar.ACIKLAMA, "");
                    detay.kapali = VeriErisim.DegerAl(dr, (int)TABHRCYUZDEONKAPAT.ENUMAlanlar.KAPALI, 0);
                    detay.talepNo = VeriErisim.DegerAl(dr, (int)TABHRCYUZDEONKAPAT.ENUMAlanlar.TALEPNO, 0);
                    al.Ekle(detay);
                }
                dr.Close();
            }
            else
                al.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return al;
        }

        public static Sonuc YuzdeOnAcikMi(VeriErisim veriErisim, Kullanici kullanan, int yil, string ihaleMaddeAdi, string malHizmetYapim, TNSCollection tertipler, int saymanlikKodu, string birimKodu)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (OnayForm.DogrudanTeminMi(ihaleMaddeAdi) && !ARGEMi(tertipler))
            {
                ObjectArray oa = Listele(veriErisim, kullanan, new YuzdeOnKapat(yil, malHizmetYapim));
                if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
                {
                    YuzdeOnKapat yok = (YuzdeOnKapat)oa[0];
                    bool kapanmis = false;
                    string hata = null;
                    if (yok.kapali == 1)
                    {
                        if (yok.talepNo == 0)
                            kapanmis = true;
                        else
                        {
                            DogrudanTeminPlan form = new DogrudanTeminPlan();
                            form.yil = yil;
                            form.talepNo = yok.talepNo;
                            if (form.talepNo > 0) form.talepNo--;
                            form.harcamaTuru = malHizmetYapim;
                            form.saymanlik = saymanlikKodu;
                            form.birim = birimKodu;
                            ObjectArray talepler = HRCDogrudanTeminPlan.Listele(veriErisim, kullanan, form);
                            if (talepler.ObjeSayisi == 0)
                            {
                                kapanmis = true;
                                hata = yok.aciklama;
                            }
                        }
                    }
                    if (kapanmis)
                    {
                        if (string.IsNullOrEmpty(hata))
                            hata = string.Format("Bakanlığımızın {0} alımı %10 limiti dolmuş olduğundan, {1} maddesine göre işlem yapamazsınız!", malHizmetYapim, ihaleMaddeAdi);
                        s.DegerAta(hata);
                    }
                }
            }
            return s;
        }

        private static bool ARGEMi(TNSCollection tertipler)
        {
            if (tertipler == null || tertipler.Count == 0) return false;
            foreach (object tob in tertipler)
            {
                string tertip = null;
                if (tob.GetType() == typeof(TertipOdenekBilgi))
                {
                    tertip = ((TertipOdenekBilgi)tob).tertip;
                }
                else if (tob.GetType() == typeof(Tertip))
                {
                    tertip = ((Tertip)tob).TertipStrOl();
                }
                if (!ARGEMi(tertip)) return false;
            }
            return true;
        }

        private static bool ARGEMi(string tertip)
        {
            if (tertip == null) return false;
            string[] tertip_kodlar = tertip.Split('-');
            string fon = OrtakFonksiyonlar.DiziElemanAl(tertip_kodlar, 1, "");
            if (string.IsNullOrWhiteSpace(fon)) return false;

            return fon.Replace(".", "").StartsWith("0482");
        }
    }
}

using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// EVREvrakFizikselYeri formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class EVREvrakFizikselYeri
    {
        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, EvrakFizikselYeri form)
        {
            EvrakSinif evrakForm = new EvrakSinif();
            evrakForm.prEvrakNo = form.prEvrakNo;
            ObjectArray liste = TNS.EVR.IsKurallari.EVREvrak.Listele(null, kullanan, evrakForm);
            foreach (EvrakSinif ef in liste.objeler)
            {
                ef.bulunduguYer = form.kullaniciAktifBirimi;
                return TNS.EVR.Yetki.YazmaYetkisiVarMi(kullanan, ef);
            }

            return false;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, EvrakFizikselYeri form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// EvrakFizikselYeri nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EvrakFizikselYeri yapısında EvrakFizikselYeri:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakFizikselYeri form)
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

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakFizikselYeri to = new TABEvrakFizikselYeri(ve);

            ve.BeginTransaction();
            try
            {
                if (s.islemSonuc)
                    s = to.Sil(form);

                if (s.islemSonuc)
                    s = to.Ekle(form);

                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{4DA52F84-4043-44AE-BBE6-ED7AE12E7B91} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// EvrakFizikselYeri nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EvrakFizikselYeri yapısında EvrakFizikselYeri:  bilgilerini tutan değişken</param>
        public static EvrakFizikselYeri Oku(VeriErisim veriErisim, Kullanici kullanan, EvrakFizikselYeri form)
        {
            EvrakFizikselYeri[] s = Listele(veriErisim, kullanan, form);
            if (s == null || s.Length == 0) return null;
            return s[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, EvrakFizikselYeri form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, EvrakFizikselYeri form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// EvrakFizikselYeri listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EvrakFizikselYeri yapısında EvrakFizikselYeri:  bilgilerini tutan değişken</param>
        public static EvrakFizikselYeri[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakFizikselYeri form)
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

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakFizikselYeri to = new TABEvrakFizikselYeri(ve);

            ArrayList alAna;
            IDataReader dr;
            dr = to.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            alAna = new ArrayList();
            while (dr.Read())
            {
                EvrakFizikselYeri detay = new EvrakFizikselYeri();
                detay.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakFizikselYeri.ENUMAlanlar.PREVRAKNO, "");
                detay.bina = VeriErisim.DegerAl(dr, (int)TABEvrakFizikselYeri.ENUMAlanlar.BINA, "");
                detay.kat = VeriErisim.DegerAl(dr, (int)TABEvrakFizikselYeri.ENUMAlanlar.KAT, "");
                detay.oda = VeriErisim.DegerAl(dr, (int)TABEvrakFizikselYeri.ENUMAlanlar.ODA, "");
                detay.bolum = VeriErisim.DegerAl(dr, (int)TABEvrakFizikselYeri.ENUMAlanlar.BOLUM, "");
                detay.raf = VeriErisim.DegerAl(dr, (int)TABEvrakFizikselYeri.ENUMAlanlar.RAF, "");
                detay.klasor = VeriErisim.DegerAl(dr, (int)TABEvrakFizikselYeri.ENUMAlanlar.KLASOR, "");
                detay.RFID = VeriErisim.DegerAl(dr, (int)TABEvrakFizikselYeri.ENUMAlanlar.RFID, "");
                alAna.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return (EvrakFizikselYeri[])alAna.ToArray(typeof(EvrakFizikselYeri));
        }

    }
}

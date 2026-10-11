using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// HRCCalisanBordroGelirKesinti formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCCalisanBordroGelirKesinti
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, CalisanBordroGelirKesinti form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, CalisanBordroGelirKesinti form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// CalisanBordroGelirKesinti nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroGelirKesinti yapısında CalisanBordroGelirKesinti: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, CalisanBordroGelirKesinti form)
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

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABPSLCALISANBORDROGELKES tabPSLCALISANBORDROGELKES = new TABPSLCALISANBORDROGELKES(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabPSLCALISANBORDROGELKES.Sil(form);

            if (s.islemSonuc)
                s = tabPSLCALISANBORDROGELKES.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
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
        public static bool GormeYetkisiVarMi(Kullanici kullanan, CalisanBordroGelirKesinti form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, CalisanBordroGelirKesinti form)
        {
            return new Sonuc(true, "", "");
        }

        public static CalisanBordroGelirKesinti KesintiBul(CalisanBordroGelirKesinti[] dizi, int gelKesKod)
        {
            for (int i = 0; i < dizi.Length; i++)
            {
                if (dizi[i].gelirKesintiKod == gelKesKod)
                {
                    return dizi[i];
                }
            }
            return null;
        }
        /// <summary>
        /// CalisanBordroGelirKesinti nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroGelirKesinti yapısında CalisanBordroGelirKesinti: bilgilerini tutan değişken</param>
        public static CalisanBordroGelirKesinti[] Listele(VeriErisim veriErisim, Kullanici kullanan)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABPSLCALISANBORDROGELKES tabPSLCALISANBORDROGELKES = new TABPSLCALISANBORDROGELKES(ve);


            IDataReader dr;
            dr = tabPSLCALISANBORDROGELKES.Oku(null);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                CalisanBordroGelirKesinti formIc = new CalisanBordroGelirKesinti();
                formIc.gelirKesintiKod = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROGELKES.ENUMAlanlar.GELKESKOD, 0);
                formIc.gelirKesintiTur = (ENUMGelirKesintiTur)VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROGELKES.ENUMAlanlar.GELKESTUR, 0);
                formIc.hesapKodu = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROGELKES.ENUMAlanlar.HESAPKODU, "");
                formIc.adi = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROGELKES.ENUMAlanlar.ADI, "");
                formIc.siraNo = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROGELKES.ENUMAlanlar.SIRANO, "");
                formIc.kisiSira = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROGELKES.ENUMAlanlar.KISISIRA, "");
                formIc.formul = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROGELKES.ENUMAlanlar.FORMUL, "");
                formIc.formulSira = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROGELKES.ENUMAlanlar.FORMULSIRA, "");
                al.Add(formIc);
            }

            dr.Close();
            ve.VeriErisimKapat();
            return (CalisanBordroGelirKesinti[])al.ToArray(typeof(CalisanBordroGelirKesinti));
        }

    }
}

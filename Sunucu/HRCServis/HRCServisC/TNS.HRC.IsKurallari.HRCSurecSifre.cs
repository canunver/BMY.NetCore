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
    /// HRCSurecSifre formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCSurecSifre
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD="BaglantiSatiriHRC";

       
        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, SurecSifreSinif form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// SurecSifreSinif nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SurecSifreSinif yapısında SurecSifreSinif: bilgilerini tutan değişken</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, SurecSifreSinif form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                s.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                s.islemSonuc = false;
                return s;
            }
            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCSURECSIFRE tabHRCSURECSIFRE = new TABHRCSURECSIFRE(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabHRCSURECSIFRE.Sil(form);

            form.sifre = OrtakFonksiyonlar.Sifrele(form.sifre);
            for (int i = 0; i < form.satirlar.Length && s.islemSonuc; i++)
            {
                s = tabHRCSURECSIFRE.Ekle(form, i);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// SurecSifreSinif nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SurecSifreSinif yapısında SurecSifreSinif: bilgilerini tutan değişken</param>
        /// <returns></returns>
        public static SurecSifreSinif Oku(VeriErisim veriErisim, Kullanici kullanan, SurecSifreSinif form)
        {

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCSURECSIFRE tabHRCSURECSIFRE = new TABHRCSURECSIFRE(ve);

            ObjectArray donenList = new ObjectArray();
            SurecSifreSinif formIc = new SurecSifreSinif();
            IDataReader dr;
            ArrayList al;
            dr = tabHRCSURECSIFRE.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            al = new ArrayList();
            while (dr.Read())
            {
                if (string.IsNullOrEmpty(formIc.surecNo))
                { 
                    formIc.yil = VeriErisim.DegerAl(dr, (int)TABHRCSURECSIFRE.ENUMAlanlar.YIL, 0);
                    formIc.surecNo = VeriErisim.DegerAl(dr, (int)TABHRCSURECSIFRE.ENUMAlanlar.SURECNO, "");
                    formIc.sifre = VeriErisim.DegerAl(dr, (int)TABHRCSURECSIFRE.ENUMAlanlar.SIFRE, "");
                    formIc.sifre = OrtakFonksiyonlar.SifreCoz(formIc.sifre);
                    formIc.TCKimlik = VeriErisim.DegerAl(dr, (int)TABHRCSURECSIFRE.ENUMAlanlar.TCKIMLIK, "");
                    formIc.tarih = VeriErisim.DegerAl(dr, (int)TABHRCSURECSIFRE.ENUMAlanlar.TARIH); 
                }
                Parca detay = new Parca();
              
                detay.kod = VeriErisim.DegerAl(dr, (int)TABHRCSURECSIFRE.ENUMAlanlar.PARCAKOD, 0);
                al.Add(detay);
            }
            formIc.satirlar = (Parca[])al.ToArray(typeof(Parca));
            dr.Close();
            ve.VeriErisimKapat();
            return formIc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç şifre bilgisini veritabanından okur
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kriterSinif">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public static Sonuc SifreDogrula(SurecSifreSinif kriterSinif)
        {
            Sonuc sonuc = new Sonuc(false,"","");
            try
            {
                SurecSifreSinif surSifre = Oku(null, null, kriterSinif);

                if (surSifre.sifre == kriterSinif.sifre)
                    sonuc.islemSonuc = true;

            }
            catch (Exception exc)
            {

                sonuc.hataStr = exc.Message;
            }

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç şifre bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">Silinecek bilgiyi tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, SurecSifreSinif form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCSURECSIFRE tabHRCSURECSIFRE = new TABHRCSURECSIFRE(ve);
            
            return tabHRCSURECSIFRE.Sil(form);
          
        }
    }
}

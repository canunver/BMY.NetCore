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
    /// HRCDovizOdeme formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCDovizOdeme
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
        public static bool HasWriteAccess(Kullanici kullanan, DovizOdeme form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc IsFieldsOK(Kullanici kullanan, DovizOdeme form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// DovizOdeme nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DovizOdeme yapısında DovizOdeme:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Save(VeriErisim veriErisim, Kullanici kullanan, DovizOdeme form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!HasWriteAccess(kullanan, form))
            {
                s.DegerAta(HarcamaSurecleriResI.DovizOdeme001);
                return s;
            }

            s = IsFieldsOK(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCDOVIZODEME tabHRCDOVIZODEME = new TABHRCDOVIZODEME(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabHRCDOVIZODEME.Delete(form);

            if (s.islemSonuc)
                s = tabHRCDOVIZODEME.Insert(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// DovizOdeme nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DovizOdeme yapısında DovizOdeme:  bilgilerini tutan değişken</param>
        public static DovizOdeme Get(VeriErisim veriErisim, Kullanici kullanan, DovizOdeme form)
        {
            DovizOdeme[] s = List(veriErisim, kullanan, form);
            if (s == null || s.Length == 0) return null;
            return s[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool HasReadAccess(Kullanici kullanan, DovizOdeme form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AreKeyFieldsOK(Kullanici kullanan, DovizOdeme form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// DovizOdeme listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DovizOdeme yapısında DovizOdeme:  bilgilerini tutan değişken</param>
        public static DovizOdeme[] List(VeriErisim veriErisim, Kullanici kullanan, DovizOdeme form)
        {
            if (!HasReadAccess(kullanan, form))
            {
                throw new Exception(HarcamaSurecleriResI.DovizOdeme002);
            }

            Sonuc s = AreKeyFieldsOK(kullanan, form);
            if (!s.islemSonuc)
            {
                throw new Exception(s.hataStr);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCDOVIZODEME tabHRCDOVIZODEME = new TABHRCDOVIZODEME(ve);

            ArrayList alAna;
            IDataReader dr;
            dr = tabHRCDOVIZODEME.Get(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            alAna = new ArrayList();
            while (dr.Read())
            {
                DovizOdeme detay = new DovizOdeme();
                detay.surecYili = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.SURECYILI, 0);
                detay.surecNo = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.SURECNO, "");
                detay.tcKimlikveyaVergiNo = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.TCKIMLIKVEYAVERGINO, "");
                detay.odemeemriNo = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.ODEMEEMRINO, "");
                detay.odemeemriyil = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.ODEMEEMRIYIL, 0);
                detay.gerekce = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.GEREKCE, "");
                detay.tutar = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.TUTAR, 0.0);
                detay.tertipler = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.TERTIPLER, "");
                detay.firma.vergiNo = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.TCKIMLIKVEYAVERGINO, "");
                detay.odemeHesapKodu = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.ODEMEHESAPKODU, "");
                detay.paraBirimi = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.PARABIRIMI, "");
                alAna.Add(detay);
            }
            dr.Close();

            foreach (DovizOdeme formIc in alAna)
            {
                FirmaBilgisi fbkriter = new FirmaBilgisi();
                fbkriter.muhasebe = form.firma.muhasebe;
                fbkriter.vergiNo = formIc.firma.vergiNo;
                ObjectArray oa = new HRCFirmaBilgisi().Listele(kullanan, ve, fbkriter);
                if (!oa.sonuc.islemSonuc || oa.ObjeSayisi != 1)
                {
                    ve.VeriErisimKapat();
                    string hata;
                    if (!oa.sonuc.islemSonuc) hata = oa.sonuc.hataStr;
                    else if (oa.ObjeSayisi == 0) hata = formIc.firma.vergiNo + " vergi numaralı firma bulunamadı!";
                    else hata = formIc.firma.vergiNo + " vergi numaralı birden fazla firma bulundu!";
                    throw new Exception(hata);
                }
                formIc.firma = (FirmaBilgisi)oa.objeler[0];
            }

            ve.VeriErisimKapat();
            return (DovizOdeme[])alAna.ToArray(typeof(DovizOdeme));
        }

        public static Sonuc DovizOdemeOdemeEmriNoKaydet(VeriErisim veriErisim, Kullanici kullanan, DovizOdeme form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!HasWriteAccess(kullanan, form))
            {
                s.DegerAta(HarcamaSurecleriResI.DovizOdeme001);
                return s;
            }

            s = IsFieldsOK(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCDOVIZODEME tabHRCDOVIZODEME = new TABHRCDOVIZODEME(ve);

            ve.BeginTransaction();

            DovizOdeme dovizOdemeler = Get(ve, kullanan, form);

            DovizOdeme doviz = new DovizOdeme();
            doviz.odemeemriNo = form.odemeemriNo;
            doviz.odemeemriyil = form.odemeemriyil;
            doviz.surecNo = form.surecNo;
            doviz.surecYili = form.surecYili;
            s = tabHRCDOVIZODEME.DovizOdemeOdemeEmriNoKaydet(doviz);

            if (s.islemSonuc)
            {
                s.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                s.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + s.hataStr;
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }
    }
}

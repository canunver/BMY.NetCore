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
    /// HRCAvukatOdemesi formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCAvukatOdemesi
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, AvukatOdemesi form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, AvukatOdemesi form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// AvukatOdemesi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AvukatOdemesi yapısında AvukatOdemesi:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AvukatOdemesi form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma Yetkiniz Yok");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCAVUKATODEMESI tabHRCAVUKATODEMESI = new TABHRCAVUKATODEMESI(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabHRCAVUKATODEMESI.Sil(form);

            if (s.islemSonuc)
                s = tabHRCAVUKATODEMESI.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// AvukatOdemesi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AvukatOdemesi yapısında AvukatOdemesi:  bilgilerini tutan değişken</param>
        public static AvukatOdemesi Oku(VeriErisim veriErisim, Kullanici kullanan, AvukatOdemesi form)
        {
            AvukatOdemesi[] s = Listele(veriErisim, kullanan, form);
            if (s == null || s.Length == 0) return null;
            return s[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, AvukatOdemesi form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, AvukatOdemesi form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// AvukatOdemesi listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AvukatOdemesi yapısında AvukatOdemesi:  bilgilerini tutan değişken</param>
        public static AvukatOdemesi[] Listele(VeriErisim veriErisim, Kullanici kullanan, AvukatOdemesi form)
        {
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception("Görme Yetkiniz Yok");
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                throw new Exception(s.hataStr);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCAVUKATODEMESI tabHRCAVUKATODEMESI = new TABHRCAVUKATODEMESI(ve);

            ArrayList alAna;
            IDataReader dr;
            dr = tabHRCAVUKATODEMESI.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            alAna = new ArrayList();
            while (dr.Read())
            {
                AvukatOdemesi detay = new AvukatOdemesi();
                detay.surecYili = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.SURECYILI, 0);
                detay.surecNo = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.SURECNO, "");
                detay.tcKimlikveyaVergiNo = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.TCKIMLIKVEYAVERGINO, "");
                detay.kisiHesapKodu = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.KISIHESAPKODU, "");
                detay.odemeEmriNo = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.ODEMEEMRINO, "");
                detay.odemeEmriYil = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.ODEMEEMRIYIL, 0);
                detay.gerekce = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.GEREKCE, "");
                detay.brutUcret = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.BRUTUCRET, 0.0);
                detay.odenecekTutar = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.ODENECEKTUTAR, 0.0);
                detay.tertipler = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.TERTIPLER, "");
                detay.odemeHesapKodu = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.ODEMEHESAPKODU, "");
                detay.paraBirimi = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.PARABIRIMI, "");
                detay.faturaTarihi = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.FATURATARIHI);
                detay.faturaNumarasi = VeriErisim.DegerAl(dr, (int)TABHRCAVUKATODEMESI.ENUMAlanlar.FATURANUMARASI, "");
                detay.firma.vergiNo = VeriErisim.DegerAl(dr, (int)TABHRCDOVIZODEME.ENUMFields.TCKIMLIKVEYAVERGINO, "");
                alAna.Add(detay);
            }
            dr.Close();
            foreach (AvukatOdemesi formIc in alAna)
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
            return (AvukatOdemesi[])alAna.ToArray(typeof(AvukatOdemesi));
        }

        public static Sonuc AvukatOdemesiOdemeEmriNoKaydet(VeriErisim veriErisim, Kullanici kullanan, AvukatOdemesi form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(HarcamaSurecleriResI.DovizOdeme001);
                return s;
            }

            s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCAVUKATODEMESI tabHRCAVUKATODEMESI = new TABHRCAVUKATODEMESI(ve);

            ve.BeginTransaction();

            AvukatOdemesi avukatOdemesi = Oku(ve, kullanan, form);

            AvukatOdemesi avukat = new AvukatOdemesi();
            avukat.odemeEmriNo = form.odemeEmriNo;
            avukat.odemeEmriYil = form.odemeEmriYil;
            avukat.surecNo = form.surecNo;
            avukat.surecYili = form.surecYili;
            s = tabHRCAVUKATODEMESI.AvukatOdemesiOdemeEmriNoKaydet(avukat);

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

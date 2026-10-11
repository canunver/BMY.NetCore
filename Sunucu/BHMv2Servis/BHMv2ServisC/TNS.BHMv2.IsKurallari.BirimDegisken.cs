using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// BHMv2BirimDegisken formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2BirimDegisken
    {
        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int saymanlik, string birim, string kurumKodu)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, BirimDegisken form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// BirimDegisken nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimDegisken yapısında BirimDegisken:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlik, string birim, string kurumKodu, BirimDegisken[] degiskenler)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, saymanlik, birim, kurumKodu))
            {
                s.DegerAta("Resources.ButceHazirlikv2.BirimDegisken001");
                return s;
            }

            //s = AlanlarGecerliMi(kullanan, saymanlik,  birim,  kurumKodu);
            //if (!s.islemSonuc)
            //{
            //    return s;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMBIRIMDEGISKEN tabBHMV2BIRIMDEGISKEN = new TABBHMBIRIMDEGISKEN(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabBHMV2BIRIMDEGISKEN.Sil(yil, saymanlik, birim, kurumKodu);

            for (int i = 0; i < degiskenler.Length && s.islemSonuc; i++)
            {
                s = tabBHMV2BIRIMDEGISKEN.Ekle(yil, saymanlik, birim, kurumKodu, degiskenler[i]);
            }

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
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, int saymanlik, string birim, string kurumKodu)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, BirimDegisken form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// BirimDegisken listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimDegisken yapısında BirimDegisken:  bilgilerini tutan değişken</param>
        public static BirimDegisken[] Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlik, string birim, string kurumKodu)
        {
            if (!GormeYetkisiVarMi(kullanan, saymanlik, birim, kurumKodu))
            {
                throw new Exception("Resources.ButceHazirlikv2.BirimDegisken002");
            }

            //Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    throw new Exception(s.hataStr);
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMBIRIMDEGISKEN tabBHMV2BIRIMDEGISKEN = new TABBHMBIRIMDEGISKEN(ve);

            IDataReader dr = tabBHMV2BIRIMDEGISKEN.Listele(yil, saymanlik, birim, kurumKodu);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList alAna = new ArrayList();
            while (dr.Read())
            {
                BirimDegisken detay = new BirimDegisken();
                detay.degiskenAdi = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlar.DEGISKENADI, "");
                detay.degerInt = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlar.DEGERINT, 0);
                detay.degerDbl = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlar.DEGERDBL, 0.0);
                detay.degerStr = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlar.DEGERSTR, "");
                alAna.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return (BirimDegisken[])alAna.ToArray(typeof(BirimDegisken));
        }

        internal static BirimDegiskenYapi BirimDegiskenYapiOku(VeriErisim veriErisim, Kullanici kullanan, string kurKod)
        {
            int yil = DateTime.Now.Year;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMBIRIMDEGISKEN tab = new TABBHMBIRIMDEGISKEN(ve);

            IDataReader dr = tab.YapiListele(kurKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            BirimDegiskenYapi by = new BirimDegiskenYapi();
            ArrayList alDeg = new ArrayList();
            ArrayList alFor = new ArrayList();
            BirimDegiskenTanim detay = null;
            while (dr.Read())
            {
                detay = new BirimDegiskenTanim();
                detay.degiskenAdi = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlarTANIM.DEGISKENADI, "");
                int tipIc = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlarTANIM.TIP, 0);
                detay.tip = (VariableType)tipIc;
                int ortakIc = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlarTANIM.ORTAK, 0);
                detay.ortak = ortakIc == 1;
                detay.formula = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlarTANIM.FORMUL, "");
                detay.kisaEtiket = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlarTANIM.KISAETIKET, "");
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlarTANIM.UZUNETIKET, "");
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMDEGISKEN.ENUMAlanlarTANIM.SIRANO, "");
                if (string.IsNullOrWhiteSpace(detay.formula))
                    alDeg.Add(detay);
                else
                    alFor.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            if (detay == null) return null;
            by.degiskenler = (BirimDegiskenTanim[])alDeg.ToArray(typeof(BirimDegiskenTanim));
            by.formuller = (BirimDegiskenTanim[])alFor.ToArray(typeof(BirimDegiskenTanim));
            return by;
        }

        internal static Sonuc BirimDegiskenYapiSakla(VeriErisim veriErisim, Kullanici kullanan, string kurKod, BirimDegiskenYapi by)
        {
            int yil = DateTime.Now.Year;
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMBIRIMDEGISKEN tab = new TABBHMBIRIMDEGISKEN(ve);

            s = tab.YapiSil(kurKod);
            for (int i = 0; i < by.degiskenler.Length && s.islemSonuc; i++)
            {
                s = tab.YapiEkle(kurKod, by.degiskenler[i]);
            }
            for (int i = 0; i < by.formuller.Length && s.islemSonuc; i++)
            {
                s = tab.YapiEkle(kurKod, by.formuller[i]);
            }

            if (s.islemSonuc)
            {
                ve.Commit();
            }
            else
            {
                ve.Rollback();
            }
            return s;
        }
    }
}

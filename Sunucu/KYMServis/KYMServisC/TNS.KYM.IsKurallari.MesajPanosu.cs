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
    /// KYMMesajPanosu formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class KYMMesajPanosu
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriKYM";

        /// <summary>
        /// KullaniciMesaj nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="mesajlar">KullaniciMesaj yapısında KullaniciMesaj:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KullaniciMesaj[] mesajlar)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABKYMKULLANICIMESAJ tabKYMKULLANICIMESAJ = new TABKYMKULLANICIMESAJ(ve);

            TNSDateTime dt = new TNSDateTime(DateTime.Now);
            for (int i = 0; i < mesajlar.Length; i++)
            {
                ve.BeginTransaction();
                mesajlar[i].tarih = dt;
                mesajlar[i].mesajNo = tabKYMKULLANICIMESAJ.MaxMesajNo(mesajlar[i].kullaniciKodu) + 1;
                if (ve.hata != "") s.DegerAta(ve.hata);
                if (s.islemSonuc)
                    s = tabKYMKULLANICIMESAJ.Ekle(mesajlar[i]);
                if (s.islemSonuc)
                    ve.Commit();
                else
                {
                    ve.Rollback();
                    break;
                }
                ve.hata = "";
            }

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// KullaniciMesaj listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KullaniciMesaj yapısında KullaniciMesaj:  bilgilerini tutan değişken</param>
        public static KullaniciMesaj[] Listele(VeriErisim veriErisim, Kullanici kullanan, KullaniciMesaj form)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABKYMKULLANICIMESAJ tabKYMKULLANICIMESAJ = new TABKYMKULLANICIMESAJ(ve);

            ArrayList alAna;
            IDataReader dr;
            dr = tabKYMKULLANICIMESAJ.Oku(form);
            alAna = new ArrayList();

            if (dr == null)
            {
                ve.VeriErisimKapat();
                //throw new Exception(ve.hata);
                return (KullaniciMesaj[])alAna.ToArray(typeof(KullaniciMesaj));
            }
            while (dr.Read())
            {
                KullaniciMesaj detay = new KullaniciMesaj();
                detay.kullaniciKodu = VeriErisim.DegerAl(dr, (int)TABKYMKULLANICIMESAJ.ENUMAlanlar.KULLANICIKODU, "");
                detay.mesajNo = VeriErisim.DegerAl(dr, (int)TABKYMKULLANICIMESAJ.ENUMAlanlar.MESAJNO, 0);
                detay.tarih = VeriErisim.DegerAl(dr, (int)TABKYMKULLANICIMESAJ.ENUMAlanlar.TARIH);
                detay.mesaj = VeriErisim.DegerAl(dr, (int)TABKYMKULLANICIMESAJ.ENUMAlanlar.MESAJ, "");
                alAna.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return (KullaniciMesaj[])alAna.ToArray(typeof(KullaniciMesaj));
        }
    }
}

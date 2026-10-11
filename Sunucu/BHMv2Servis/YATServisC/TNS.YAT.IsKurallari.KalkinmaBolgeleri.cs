using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
using TNS.YAT.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// YATKalkinmaBolgeleriTamim formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class YATKalkinmaBolgeleri
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD="BaglantiSatiriYAT";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            return (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI));

        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, KalkinmaBolge form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrWhiteSpace(form.bolgeKod))
                s.DegerAta("Bölge kodu boş olamaz!");
            if (string.IsNullOrWhiteSpace(form.bolgeAd))
                s.DegerAta("Bölge Adı boş olamaz!");

            if (form.kalkinmaBolgeIl.Count == 0)
                s.DegerAta("Bölge illeri boş olamaz!");
            else
            {
                foreach (Cografya item in form.kalkinmaBolgeIl)
                {
                    if (string.IsNullOrWhiteSpace(item.kod))
                    {
                        s.DegerAta("İl kodu boş olamaz!");
                        break;
                    }
                }
            }
            return s;
        }

        /// <summary>
        /// KalkinmaBolge nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KalkinmaBolge yapısında KalkinmaBolge: Kalkinma Bölgelesi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KalkinmaBolge form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta("Bölge bilgisi kaydetme yetkiniz bulunmamaktadır");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATKALKINMABOLGE tabYATKALKINMABOLGE = new TABYATKALKINMABOLGE(ve);
            TABYATKALKINMABOLGEIL tabYATKALKINMABOLGEIL = new TABYATKALKINMABOLGEIL(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabYATKALKINMABOLGEIL.Sil(form);
            if (s.islemSonuc)
                s = tabYATKALKINMABOLGE.Sil(form);

            if (s.islemSonuc)
                s = tabYATKALKINMABOLGE.Ekle(form);

            for (int i = 0; i < form.kalkinmaBolgeIl.Count && s.islemSonuc; i++)
            {
                Cografya sk = (Cografya)form.kalkinmaBolgeIl[i];
                s = tabYATKALKINMABOLGEIL.Ekle(form.bolgeKod, sk.kod);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// KalkinmaBolge nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KalkinmaBolge yapısında KalkinmaBolge: Kalkinma Bölgelesi bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, KalkinmaBolge form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, KalkinmaBolge form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// KalkinmaBolge listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KalkinmaBolge yapısında KalkinmaBolge: Kalkinma Bölgelesi bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, KalkinmaBolge form)
        {
            ObjectArray oa = new ObjectArray();

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                oa.sonuc.DegerAta(s.hataStr);
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATKALKINMABOLGE tabYATKALKINMABOLGE = new TABYATKALKINMABOLGE(ve);
            TABYATKALKINMABOLGEIL tabYATKALKINMABOLGEIL = new TABYATKALKINMABOLGEIL(ve);

            IDataReader dr = tabYATKALKINMABOLGE.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                KalkinmaBolge detay = new KalkinmaBolge();
                detay.bolgeKod = VeriErisim.DegerAl(dr, (int)TABYATKALKINMABOLGE.ENUMAlanlar.BOLGEKOD, "");
                detay.bolgeAd = VeriErisim.DegerAl(dr, (int)TABYATKALKINMABOLGE.ENUMAlanlar.BOLGEAD, "");
                oa.Ekle(detay);
            }
            dr.Close();
            TNSCollection al;
            foreach(KalkinmaBolge formIc in oa.objeler)
            {
                dr = tabYATKALKINMABOLGEIL.Oku(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    Cografya detay = new Cografya();
                    detay.kod = VeriErisim.DegerAl(dr, 0, "");
                    detay.ad = VeriErisim.DegerAl(dr, 1, "");
                    detay.plakaKod = VeriErisim.DegerAl(dr, 2, "");
                    detay.muhasebe = VeriErisim.DegerAl(dr, 3, 0);
                    al.Add(detay);
                }
                formIc.kalkinmaBolgeIl = al;
                dr.Close();
            }
            ve.VeriErisimKapat();
            return oa;
        }
    }
}

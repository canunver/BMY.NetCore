using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
using TNS.PEB.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// PEBKurumGrup formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class PEBKurumGrup
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriSTR";

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, KurumGruplari form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrWhiteSpace(form.ad))
                s.DegerAta("Lütfen grup adını giriniz!");
            return s;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, KurumGruplari form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrWhiteSpace(form.kullaniciKodu))
                s.DegerAta("Lütfen kullanıcıyı seçiniz!");
            if (form.siraNo <= 0)
                s.DegerAta("Lütfen sırano seçiniz!");
            return s;
        }

        /// <summary>
        /// KurumGruplari nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KurumGruplari yapısında KurumGruplari: Kurum Grupları bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KurumGruplari form)
        {
            Sonuc s = new Sonuc(true, "", "");
            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRKURUMGRUPLARI tabPEBKURUMGRUPLARI = new TABSTRKURUMGRUPLARI(ve);
            TABSTRKURUMGRUPDETAY tabPEBKURUMGRUPDETAY = new TABSTRKURUMGRUPDETAY(ve);

            ve.BeginTransaction();

            form.siraNo = tabPEBKURUMGRUPLARI.SiraNoBul(form);
            if (s.islemSonuc)
                s = tabPEBKURUMGRUPLARI.Ekle(form);

            for (int i = 0; i < form.kurumGrupDetay.Count && s.islemSonuc; i++)
            {
                s = tabPEBKURUMGRUPDETAY.Ekle(form, ((KurumGrupDetay)form.kurumGrupDetay[i]).kurumKod);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// KurumGruplari nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KurumGruplari yapısında KurumGruplari: Kurum Grupları bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, KurumGruplari form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// KurumGruplari listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KurumGruplari yapısında KurumGruplari: Kurum Grupları bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, KurumGruplari form)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRKURUMGRUPLARI tabPEBKURUMGRUPLARI = new TABSTRKURUMGRUPLARI(ve);
            TABSTRKURUMGRUPDETAY tabPEBKURUMGRUPDETAY = new TABSTRKURUMGRUPDETAY(ve);

            IDataReader dr;
            dr = tabPEBKURUMGRUPLARI.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                KurumGruplari detay = new KurumGruplari();
                detay.kullaniciKodu = VeriErisim.DegerAl(dr, (int)TABSTRKURUMGRUPLARI.ENUMAlanlar.KULLANICIKODU, "");
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABSTRKURUMGRUPLARI.ENUMAlanlar.SIRANO, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABSTRKURUMGRUPLARI.ENUMAlanlar.AD, "");
                detay.kurumGrupDetay = null;
                oa.Ekle(detay);
            }
            dr.Close();
            if (form.detayOku)
            {
                TNSCollection al;
                TNS.BKD.Tertip trtp = new BKD.Tertip(form.yil, BKD.ENUMTabloKod.TABPEBOdenek);
                foreach (KurumGruplari formIc in oa.objeler)
                {
                    dr = tabPEBKURUMGRUPDETAY.Listele(form.yil, formIc);
                    if (dr == null)
                    {
                        ve.VeriErisimKapat();
                        oa.sonuc.DegerAta(ve.hata);
                        return oa;
                    }
                    al = new TNSCollection();
                    while (dr.Read())
                    {
                        KurumGrupDetay detay = new KurumGrupDetay();
                        detay.kurumKod = OrtakFonksiyonlar.KodNoktaKoy(trtp.Kirilimlar(0), VeriErisim.DegerAl(dr, (int)TABSTRKURUMGRUPDETAY.ENUMAlanlar.KURUMKOD, ""));
                        detay.kurumAd = VeriErisim.DegerAl(dr, (int)TABSTRKURUMGRUPDETAY.ENUMAlanlar.KURUMKOD + 1, "");
                        al.Add(detay);
                    }
                    formIc.kurumGrupDetay = al;
                    dr.Close();
                }
            }
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KurumGruplari form)
        {
            Sonuc s = new Sonuc(true, "", "");
            s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRKURUMGRUPLARI tabPEBKURUMGRUPLARI = new TABSTRKURUMGRUPLARI(ve);
            TABSTRKURUMGRUPDETAY tabPEBKURUMGRUPDETAY = new TABSTRKURUMGRUPDETAY(ve);

            ve.BeginTransaction();

            s = tabPEBKURUMGRUPLARI.Sil(form);
            if (s.islemSonuc)
                s = tabPEBKURUMGRUPDETAY.Sil(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }
    }
}

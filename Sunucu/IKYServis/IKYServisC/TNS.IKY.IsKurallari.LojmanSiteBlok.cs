using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.IKY.IsKurallari
{
    /// <summary>
    /// IKYLojmanSite formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class IKYLojmanSiteBlok
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriIKY";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, LojmanSiteBlok form)
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
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, LojmanSiteBlok form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// LojmanSiteBlok nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSiteBlok yapısında LojmanSiteBlok:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, LojmanSiteBlok form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(PersonelResI.LojmanSite001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYLOJMANSITEBLOK TAB = new TABIKYLOJMANSITEBLOK(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.kod > 0)
                    s = TAB.Guncelle(form);
                else
                {
                    form.kod = TABGenel.SayacDegerAlInt(ve, TABIKYLOJMANSITEBLOK.TABLOAD);
                    s = TAB.Kaydet(form);
                }
            }

            //if (s.islemSonuc)
            //    s = TABIKYLOJMANSITE.Sil(form);

            //if (s.islemSonuc)
            //    s = TABIKYLOJMANSITE.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// LojmanSiteBlok nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSiteBlok yapısında LojmanSiteBlok:  bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, LojmanSiteBlok form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, LojmanSiteBlok form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, LojmanSiteBlok form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// LojmanSiteBlok listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSiteBlok yapısında LojmanSiteBlok:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, LojmanSiteBlok form)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                oa.sonuc.DegerAta(PersonelResI.LojmanSite002);
                return oa;
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                oa.sonuc.DegerAta(s.hataStr);
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYLOJMANSITEBLOK TAB = new TABIKYLOJMANSITEBLOK(ve);

            IDataReader dr;
            dr = TAB.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            int AlanlarSonIndex = (Enum.GetNames(typeof(TABIKYLOJMANSITEBLOK.ENUMAlanlar))).Length;
            while (dr.Read())
            {
                LojmanSiteBlok detay = new LojmanSiteBlok();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOK.ENUMAlanlar.KOD, 0);
                detay.siteKod = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOK.ENUMAlanlar.SKOD, 0);
                detay.blokAdi = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOK.ENUMAlanlar.BlokAdi, "");
                detay.blokNo = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOK.ENUMAlanlar.BlokNo, "");
                detay.daireSayisi = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOK.ENUMAlanlar.BlokDaireSayisi, 0);
                detay.siteGideri = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOK.ENUMAlanlar.BlokSiteGideriKesilsinmi, 0);
                detay.siteAdi = VeriErisim.DegerAl(dr, AlanlarSonIndex, "");
                detay.daireSayisi = VeriErisim.DegerAl(dr, AlanlarSonIndex + 1, 0);
                detay.bosDaireAdeti = VeriErisim.DegerAl(dr, AlanlarSonIndex + 2, 0);
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static Sonuc Sil(Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc();
            //object eski = null, yeni = null; //Tarihçe için

            if (!YazmaYetkisiVarMi(kullanan, new LojmanSiteBlok ()))
            {
                sonuc.DegerAta(PersonelResI.LojmanSite001);
                return sonuc;
            }

            string hata = "";
            //if (p.kursKodu == 0)
            //    hata = "Kurs bilgisi boş bırakılamaz.<br>";
            if (kod < 0)
                hata += "Blok kod boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
            {
                TABIKYLOJMANSITEBLOK tab = new TABIKYLOJMANSITEBLOK(ve);
                sonuc = tab.Sil(kod);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";

                //    //Tarihçe Kaydet ********************
                //    yeni = p;
                //    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, ENUMTARIHCEISLEMTIP.SIL);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

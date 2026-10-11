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
    public class IKYLojmanSiteBlokDaire
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, LojmanSiteBlokDaire form)
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
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, LojmanSiteBlokDaire form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// LojmanSiteBlokDaire nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSiteBlokDaire yapısında LojmanSiteBlokDaire:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, LojmanSiteBlokDaire form)
        {
            string hata = "";
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

            if (form.metrekare <= 0)
                hata = "Metrekare alanı boş bırakılamaz.";
            if (form.daireNo <= 0)
                hata += "Daire no alanı boş bırakılamaz.";
            if (form.daireTuru <= 0)
                hata += "Daire türü alanı boş bırakılamaz.";

            if (!string.IsNullOrWhiteSpace(hata))
            {
                s.DegerAta(hata);
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYLOJMANSITEBLOKDAIRE TAB = new TABIKYLOJMANSITEBLOKDAIRE(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.kod > 0)
                    s = TAB.Guncelle(form);
                else
                {
                    form.kod = TABGenel.SayacDegerAlInt(ve, TABIKYLOJMANSITEBLOKDAIRE.TABLOAD);
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
        /// LojmanSiteBlokDaire nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSiteBlokDaire yapısında LojmanSiteBlokDaire:  bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, LojmanSiteBlokDaire form)
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
        public static bool GormeYetkisiVarMi(Kullanici kullanan, LojmanSiteBlokDaire form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, LojmanSiteBlokDaire form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// LojmanSiteBlokDaire listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSiteBlokDaire yapısında LojmanSiteBlokDaire:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, LojmanSiteBlokDaire form)
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
            TABIKYLOJMANSITEBLOKDAIRE TAB = new TABIKYLOJMANSITEBLOKDAIRE(ve);

            IDataReader dr;
            dr = TAB.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            int AlanlarSonIndex = (Enum.GetNames(typeof(TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar))).Length;
            while (dr.Read())
            {
                LojmanSiteBlokDaire detay = new LojmanSiteBlokDaire();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.KOD, 0);
                detay.blokKod = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.BKOD, 0);
                detay.daireNo = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireNo, 0);
                detay.daireTipi = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireTipi, 0);
                detay.metrekare = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireMetreKare, 0);
                detay.elektrikNo = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireElektrikNo, 0);
                detay.suNo = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireSuNo, 0);
                detay.dogalgazNo = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireDogalGazNo, 0);
                detay.cevtemNo = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireCevreTemizlik, 0);
                detay.dahiliTel = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireTelefon, "");
                detay.daireTuru = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireTuru, 0);
                detay.daireKatNo = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireKatNo, 0);
                detay.daireAciklama = VeriErisim.DegerAl(dr, (int)TABIKYLOJMANSITEBLOKDAIRE.ENUMAlanlar.DaireAciklama, "");
                detay.siteAdi = VeriErisim.DegerAl(dr, AlanlarSonIndex, "");
                detay.blokNo = VeriErisim.DegerAl(dr, AlanlarSonIndex + 1, "");
                detay.daireTuruAd = VeriErisim.DegerAl(dr, AlanlarSonIndex + 2, "");
                detay.doluMu = VeriErisim.DegerAl(dr, AlanlarSonIndex + 3, 0);
                detay.rezervMi = VeriErisim.DegerAl(dr, AlanlarSonIndex + 4, 0);

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

            //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            if (!YazmaYetkisiVarMi(kullanan, new LojmanSiteBlokDaire()))
            {
                sonuc.DegerAta(PersonelResI.LojmanSite001);
                return sonuc;
            }

            string hata = "";
            //if (p.kursKodu == 0)
            //    hata = "Kurs bilgisi boş bırakılamaz.<br>";
            if (kod < 0)
                hata += "Daire kod boş bırakılamaz.<br>";

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
                TABIKYLOJMANSITEBLOKDAIRE tab = new TABIKYLOJMANSITEBLOKDAIRE(ve);
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

        public static ObjectArray BosDaireListesi(VeriErisim veriErisim, Kullanici kullanan, int blokKod)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYLOJMANSITEBLOKDAIRE TAB = new TABIKYLOJMANSITEBLOKDAIRE(ve);

            IDataReader dr;
            dr = TAB.BosDaireListesi(blokKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                LojmanSiteBlokDaire detay = new LojmanSiteBlokDaire();
                detay.kod = VeriErisim.DegerAl(dr, 0, 0);
                detay.daireNo = VeriErisim.DegerAl(dr, 1, 0);
                detay.daireTuruAd = VeriErisim.DegerAl(dr, 2, "");
                detay.metrekare = VeriErisim.DegerAl(dr, 3, 0);

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static VarMi DaireDoluMu(VeriErisim ve, Kullanici kullanan, int daireKod, int plojmanKod)
        {
            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYLOJMANSITEBLOKDAIRE TAB = new TABIKYLOJMANSITEBLOKDAIRE(ve);
            VarMi varMi = new VarMi();
            varMi.s = new Sonuc();
            varMi.varMi = false;

            IDataReader dr;
            dr = TAB.DaireDoluMu(daireKod, plojmanKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                varMi.s.DegerAta(ve.hata);
                return varMi;
            }

            if (dr.Read())
            {
                varMi.varMi = true;
                int tip = VeriErisim.DegerAl(dr, 0, 0);
                string unvan = VeriErisim.DegerAl(dr, 2, "");
                if (!string.IsNullOrWhiteSpace(unvan))
                    unvan = "/" + unvan;

                string kisi = VeriErisim.DegerAl(dr, 1, "")+unvan; 
                string hataStr = "";
                if (tip == (int)ENUMTahsisTipi.Rezervasyon)
                    hataStr = "Daire rezervlidir. Rezerve eden kişi: " + kisi;
                else
                    hataStr = "Daire tahsislidir. Dairede oturan kişi: " + kisi;
                varMi.s.DegerAta(hataStr);
            }

            dr.Dispose();
            //ve.VeriErisimKapat();

            return varMi;
        }
    }
}

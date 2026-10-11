using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.DEG;
using TNS.KYM;
using TNS.Veri;
using TNS.BHMv2.IsKurallari.Veri;
using System.Collections;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Kurumsal harcama birimi hiyerarşisini tutan kurumsal yapı bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2KurumsalYapi
    {
        /// <summary>
        /// Konfigürasyon dosyasından okunan ve değişkende saklanan kurumsal kod (kod no:13) yapısı döndürülür.
        /// </summary>
        /// <returns>Kurumsal kod yapısı (99.99 gibi) dizgi olarak döndürülür.</returns>
        public static string KurumsalYapiDondur()
        {
            string kodYapisi = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD15/KODYAPI");
            return kodYapisi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ve hiyerarşideki bir alt kırılımı ve bütün alt kırılımları döndüren yordam.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Yordamı çağıran kullanıcı</param>
        /// <param name="kurumsalYapi">Kurumsal yapı kriteri. Bu yapı içindeki değerlere göre kriter oluşturulur.</param>
        /// <param name="sadeceBirinciSeviye">Kurumsal yapı sıradüzeni içinde, birinci seviyedeki kodlar döndürülür.</param>
        /// <param name="enAltSeviye">Sadece en alt seviyedeki kurumsal kod bilgileri döndürülür.</param>
        /// <param name="birAltSeviye">True ise Kurumsal yapı kriterinde verilen kodun bir alt seviyesindeki kodlar listelenir. False ise o kod ile başlayan tüm kodlar listelenir</param>
        /// <returns>Verilen kriterlere uyan kurumsal kodlar dizisi döndürülür</returns>
        public static KurumsalYapi[] Listele(VeriErisim veriErisim, Kullanici kullanan, KurumsalYapi kurumsalYapi, bool sadeceBirinciSeviye, bool enAltSeviye, bool birAltSeviye)
        {
            string hata = string.Empty;
            string kodYapisi = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD15/KODYAPI");
            string kurKodYapisi = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD1/KODYAPI");

            if (!(string.IsNullOrEmpty(kurumsalYapi.kod) || OrtakFonksiyonlar.KodYapiKontrol(kodYapisi, kurumsalYapi.kod.Replace(".", string.Empty), false).islemSonuc))
                hata = string.Format(ButceHazirlikResI.BHMKY001, kodYapisi);
            if (!string.IsNullOrEmpty(hata))
            {
                throw new Exception(hata);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", kullanan.KimlikBul());
            TABBHMKURUMSALYAPI tabKY = new TABBHMKURUMSALYAPI(ve);
            IDataReader dr = null;

            try
            {
                dr = tabKY.Listele(kurumsalYapi, kodYapisi, sadeceBirinciSeviye, enAltSeviye, birAltSeviye);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }
            ArrayList al = new ArrayList();
            if (dr != null)
            {
                while (dr.Read())
                {
                    KurumsalYapi gelen = new KurumsalYapi();
                    gelen.kod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.KOD, string.Empty));
                    gelen.ad = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.AD, string.Empty);
                    gelen.saymanlikKod = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.SAYMANLIKKOD, 0);
                    gelen.birimKod = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.BIRIMKOD, string.Empty);
                    gelen.kurKod = OrtakFonksiyonlar.KodNoktaKoy(kurKodYapisi, VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.KURKOD, string.Empty));

                    al.Add(gelen);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return (KurumsalYapi[])al.ToArray(typeof(KurumsalYapi));
        }

        /// <summary>
        /// Verilen saymanlık, birim ve kurumsal kod kriterlerine uygun olan en alt kırılımdaki harcama birimlerini döndüren yordam.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="saymanlik">Harcama birimi verilen saymanlık olan harcama birimleri listelenir.</param>
        /// <param name="birim">Harcama birimi verilen birim kodu olan harcama birimleri listelenir.</param>
        /// <param name="kurKod">Harcama birimi verilen kurumsal kod harcama birimleri listelenir.</param>
        /// <returns>Verilen kriterlere uyan kurumsal kodlar dizisi döndürülür.</returns>
        public static string[] Listele(VeriErisim veriErisim, string saymanlik, string birim, string kurKod)
        {
            ArrayList al = new ArrayList();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", null);
            TABBHMKURUMSALYAPI tab = new TABBHMKURUMSALYAPI(ve);

            IDataReader dr = tab.Listele(saymanlik, birim, kurKod);

            if (dr != null)
            {
                while (dr.Read())
                {
                    al.Add(VeriErisim.DegerAl(dr, 0, "-"));
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return (string[])al.ToArray(typeof(string));
        }


        /// <summary>
        /// Verilen kurumsal yapı koduna ait bilgiler döndürülür.
        /// Kurumsal yapı 9 ile başlıyorsa dağıtılmayan tertiplerin tutulduğu kuurmsal yapı olduğundan
        /// nesne bilgileri veri tabanından okunmaz, elle oluşturulur.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kod">Kurumsal yapı bilgisi döndürülecek kod</param>
        /// <returns>Verilen koda ait kurumsal yapı bilgisi</returns>
        public static KurumsalYapi KurumsalYapiDondur(VeriErisim veriErisim, string kod)
        {
            string kodYapisi = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD15/KODYAPI");
            string kurKodYapisi = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD1/KODYAPI");

            KurumsalYapi donusDeger = new KurumsalYapi();
            if (kod.StartsWith("900"))
            {
                donusDeger.birimKod = string.Empty;
                donusDeger.saymanlikKod = 0;
                donusDeger.kod = kod;
                donusDeger.kurKod = kod.Substring(3, 8);
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", null);
            TABBHMKURUMSALYAPI tabKY = new TABBHMKURUMSALYAPI(ve);
            IDataReader dr = null;

            try
            {
                dr = tabKY.KurumsalKodOku(kod);

                if (dr.Read())
                {
                    donusDeger.kod = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.KOD, string.Empty);
                    donusDeger.kod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, donusDeger.kod);
                    donusDeger.ad = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.AD, string.Empty);
                    donusDeger.saymanlikKod = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.SAYMANLIKKOD, 0);
                    donusDeger.birimKod = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.BIRIMKOD, string.Empty);
                    donusDeger.kurKod = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.KURKOD, string.Empty);
                    donusDeger.kurKod = OrtakFonksiyonlar.KodNoktaKoy(kurKodYapisi, donusDeger.kurKod);
                }
                else
                {

                }

                dr.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                ve.VeriErisimKapat();
            }
            return donusDeger;
        }

        /*
                public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, KurumsalYapi ky, bool altKirilim, bool sadeceBirinciSeviye)
                {
                    return Listele(veriErisim, kullanan, ky, altKirilim, sadeceBirinciSeviye, false);
                } */

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KurumsalYapi ky)
        {
            Sonuc donusDeger = new Sonuc();

            string kodYapisi = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD15/KODYAPI");

            //Yetki kontrolü eklenecek

            string hata = string.Empty;
            if (string.IsNullOrEmpty(ky.kod.Trim()))
                hata = "Kod alanı boş bırakılamaz.<br>";
            else if (!OrtakFonksiyonlar.KodYapiKontrol(kodYapisi, ky.kod.Replace(".", string.Empty), false).islemSonuc)
                hata = "Kod yapısı " + kodYapisi + " şeklinde olmalıdır.<br>";
            if (string.IsNullOrEmpty(ky.ad.Trim()))
                hata += "Ad alanı boş bırakılamaz.<br>";
            //if (ky.saymanlikKod < 0)
            //    hata += "Muhasebe alanı boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", kullanan.KimlikBul());
            TABBHMKURUMSALYAPI tabKY = new TABBHMKURUMSALYAPI(ve);

            try
            {
                string ustKod = string.Empty;
                if (ky.kod.Contains("."))
                    ustKod = ky.kod.Substring(0, ky.kod.LastIndexOf("."));

                if (!string.IsNullOrEmpty(ustKod))
                {
                    donusDeger.islemSonuc = tabKY.VarMi(ustKod);
                    if (!donusDeger.islemSonuc)
                    {
                        ve.VeriErisimKapat();
                        donusDeger.hataStr = "Önce üst kodu kaydedin.<br>";
                        return donusDeger;
                    }
                }

                ve.BeginTransaction();

                donusDeger = tabKY.Guncelle(ky);
                if (!donusDeger.islemSonuc)
                    donusDeger = tabKY.Kaydet(ky);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KurumsalYapi ky)
        {
            Sonuc donusDeger = new Sonuc();

            string kodYapisi = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD15/KODYAPI");

            //Yetki kontrolü eklenecek

            string hata = string.Empty;
            if (string.IsNullOrEmpty(ky.kod.Trim()))
                hata = "Kod alanı boş bırakılamaz.<br>";
            else if (!OrtakFonksiyonlar.KodYapiKontrol(kodYapisi, ky.kod.Replace(".", string.Empty), false).islemSonuc)
                hata = "Kod yapısı " + kodYapisi + " şeklinde olmalıdır.<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMKURUMSALYAPI tabKY = new TABBHMKURUMSALYAPI(ve);
            ve.BeginTransaction();

            try
            {
                donusDeger.islemSonuc = tabKY.DetayVarMi(ky.kod);
                if (donusDeger.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    donusDeger.islemSonuc = false;
                    donusDeger.hataStr = "Önce alt kodu silin.<br>";
                    return donusDeger;
                }

                //Kod kullanılmış mı kontrolü olmalı
                //Hangi tablolarda kullanılacağını bilmediğim için eklemedim şimdilik
                donusDeger = tabKY.Sil(ky);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }


    }
}
using System;
using System.Runtime.CompilerServices;
using TNS.BKD;
using TNS.DEG;
using TNS.KYM;
using TNS.UZY;
using System.Data;
using System.Collections.Generic;
using TNS.Veri;

[assembly: SuppressIldasmAttribute()]
namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Muhasebeye ait işlemleri yürüten tüm fonksiyonları yöneten sınıf
    /// </summary>
    [Serializable()]
    public class MUHServis : MarshalByRefObject, TNS.MUH.IMUHServis
    {
        public static IUZYServis servisUZY = null;
        public static bool tabloDegisimYap = true;

        /// <summary>
        /// <see cref="MUHServis"/> varsayılan nesnesini üretir
        /// </summary>
        static MUHServis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.MUH.IsKurallari.MUHTabloDegisiklikleri.Kaydet();
                }
            }
        }

        internal static void AktarimLogYaz(string bilgi)
        {
            OrtakFonksiyonlar.HataDosyaYaz(TNS.MUH.IsKurallari.MUHServis.DosyaAdBul(false), ZamanBul() + ": " + bilgi, false);
        }

        public static string ZamanBul()
        {
            DateTime t = DateTime.Now;
            return $"{t.Hour.ToString("00")}:{t.Minute.ToString("00")}:{t.Second.ToString("00")}.{t.Millisecond.ToString("00")}";
        }

        public void DetayBirimDoldur()
        {
            TNS.Veri.VeriErisim ve = TNS.Veri.VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            ve.ExecuteNonquery("UPDATE MUHMIF SET DETAYBIRIM = (select BIRIMKOD FROM MUHMIFFORM F WHERE F.YIL=MUHMIF.YIL AND F.BELGENO=MUHMIF.BELGENO) WHERE (DETAYBIRIM='' OR DETAYBIRIM IS NULL)", 60000);
            ve.hata = "";
            ve.ExecuteNonquery("UPDATE MUHMIF SET KURKOD = (select KURKOD FROM MUHMIFFORM F WHERE F.YIL=MUHMIF.YIL AND F.BELGENO=MUHMIF.BELGENO) WHERE (KURKOD='' OR KURKOD IS NULL)", 60000);
            ve.VeriErisimKapat();
        }

        /// <summary>
        /// Verilen kriterlere bağlı olarak uzay servisi ile bilgi döner
        /// </summary>
        /// <param name="uzayAd">Sorgulama yapılacak uzay adı</param>
        /// <param name="kosul">Sorgulama yapılacak koşul</param>
        /// <param name="esitle">Eğer <c>true</c> olarak verilirse koşul bilgisini "=" olarak listeler eğer <c>false</c> verilirse "%LIKE%" olarak listeler</param>
        /// <param name="uzayAlan">Sorgulama sonucu döndürülecek uzaya ait alan adı</param>
        /// <returns>Verilen kriterlere bağlı olarak uzay servisi ile string yapısında bilgi döner</returns>
        internal static string UzayYaziDegerGetir(TNS.Veri.VeriErisim ve, Kullanici kullanan, string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            string sql = servisUZY.UzayKosulStr(kullanan == null ? new TNS.KYM.Kullanici() : kullanan, uzayAd, kosul, esitle, uzayAlan);
            if (string.IsNullOrWhiteSpace(sql)) return "";
            string ad = ve.TekKolonSQLStr(sql);
            if (!string.IsNullOrWhiteSpace(ve.hata))
            {
                ad = "";
                ve.hata = "";
            }
            return ad;
        }

        /// <summary>
        /// Verilen kriterlere bağlı olarak uzay servisi ile bilgi döner
        /// </summary>
        /// <param name="uzayAd">Sorgulama yapılacak uzay adı</param>
        /// <param name="kosul">Sorgulama yapılacak koşul</param>
        /// <param name="esitle">Eğer <c>true</c> olarak verilirse koşul bilgisini "=" olarak listeler eğer <c>false</c> verilirse "%LIKE%" olarak listeler</param>
        /// <param name="uzayAlan">Sorgulama sonucu döndürülecek uzaya ait alan adı</param>
        /// <returns>Verilen kriterlere bağlı olarak uzay servisi ile double yapısında bilgi döner</returns>
        internal static double UzaySayiDegerGetir(TNS.Veri.VeriErisim ve, Kullanici kullanan, string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            string sql = servisUZY.UzayKosulStr(kullanan == null ? new TNS.KYM.Kullanici() : kullanan, uzayAd, kosul, esitle, uzayAlan);
            if (string.IsNullOrWhiteSpace(sql)) return 0;

            double deger = ve.TekKolonSQLDbl(sql);
            if (!string.IsNullOrWhiteSpace(ve.hata))
            {
                deger = 0;
                ve.hata = "";
            }
            return deger;
        }

        /// <summary>
        /// Ödeme emri / MIF kaydetme işlemi
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc OdemeEmriMIFKaydet(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().Kaydet(kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message + " --> " + e.StackTrace, "");
            }
        }

        /// <summary>
        /// Ödeme emri / MIF kaydetme vergino güncelleme
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc OdemeEmriMIFAltHesapKodGuncelle(Kullanici kullanan, string yeniVergiNo, string eskiVergiNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TNS.MUH.IsKurallari.MUHOdemeEmriMIFServis.VergiNoGuncelle(null, kullanan, yeniVergiNo, eskiVergiNo);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc HRCFirmaBilgisiSilme(Kullanici kullanan, string vergiNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TNS.MUH.IsKurallari.MUHOdemeEmriMIFServis.FirmaBilgisiSil(null, kullanan, vergiNo);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public EksikOdenekler OdemeEmriMIFKaydetEksikOdenek(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().KaydetEksikOdenek(kullanan, form);
            }
            catch (Exception e)
            {
                EksikOdenekler eo = new EksikOdenekler();
                eo.sonuc = new Sonuc(false, e.ToString(), "");
                return eo;
            }
        }

        public OdemeEmriMIFDetay KDVBilgisiBul(Kullanici kullanan, int yil, string belgeNo, int satirNo, string hesapKod)
        {
            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(yil);
            try
            {
                return new MUHOdemeEmriMIFServis().KDVBilgisiBul(kullanan, yil, belgeNo, satirNo, hesapKod);
            }
            catch { }

            return detay;
        }

        public OdemeEmriMIFDetay OdemeEmriMIFTekSatirOku(Kullanici kullanan, int yil, string belgeNo, int satirNo)
        {
            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(yil);
            try
            {
                return new MUHOdemeEmriMIFServis().SatirOku(null, yil, belgeNo, satirNo);
            }
            catch { }

            return detay;
        }


        /// <summary>
        /// Dokumans the kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        public Sonuc DokumanKaydet(Kullanici kullanan, Dokuman dokuman)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().KaydetDokuman(kullanan, dokuman);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        /// <summary>
        /// Dokumans the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        public ObjectArray DokumanListele(Kullanici kullanan, Dokuman dokuman)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().ListeleDokuman(kullanan, dokuman);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }
        /// <summary>
        /// Dokumans the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        public Sonuc DokumanSil(Kullanici kullanan, Dokuman dokuman)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().SilDokuman(kullanan, dokuman);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Oybses the kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="oybsStr">The oybs STR.</param>
        /// <returns></returns>
        public Sonuc OybsKaydet(Kullanici kullanan, int yil, string belgeNo, string oybsStr)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().KaydetOybs(kullanan, yil, belgeNo, oybsStr);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Oybses the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        public Sonuc OybsSil(Kullanici kullanan, int yil, string belgeNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().SilOybs(kullanan, yil, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Oybses the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        public ObjectArray OybsListele(Kullanici kullanan, int yil, string belgeNo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().ListeleOybs(kullanan, yil, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Tek satır olarak Ödeme emri / MIF kaydetme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc OdemeEmriMIFTekSatirKaydet(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().KaydetTekSatir(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödeme emri / MIF kaydetme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <param name="ilgiliBelgeNo">İlgili belge no</param>
        /// <returns></returns>
        public Sonuc OdemeEmriMIFIlgiliBelgeKaydet(Kullanici kullanan, int yil, string belgeNo, string ilgiliBelgeNo)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().KaydetIlgiliBelge(kullanan, yil, belgeNo, ilgiliBelgeNo);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Ödeme emri / MIF listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="detay">Eğer <c>true</c> ise detay bilgilerini de listele</param>
        /// <returns></returns>
        /// ***
        public ObjectArray OdemeEmriMIFListele(Kullanici kullanan, OdemeEmriMIFForm form, bool detay)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().Listele(kullanan, form, false, detay, false);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Ödeme emri / MIF listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="detay">Eğer <c>true</c> ise detay bilgilerini de listele</param>
        /// <returns></returns>
        public ObjectArray OdemeEmriMIFListeleTumDetay(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().ListeleTumDetay(kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Ödeme emri / MIF tarih öncesi toplam listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray OdemeEmriMIFTarihOncesiToplamGetir(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().TarihOncesiToplamGetir(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödeme emri / MIF listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="detay">Eğer <c>true</c> ise detay bilgilerini de listele</param>
        /// <param name="mahsup">Eğer <c>true</c> ise mahsup bilgilerini de listele</param>
        /// <returns></returns>
        /// ***
        public ObjectArray OdemeEmriMIFListele(Kullanici kullanan, OdemeEmriMIFForm form, bool detay, bool mahsup)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().Listele(kullanan, form, false, detay, mahsup);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Bir Ödeme emri / MIF listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        /// ***
        public ObjectArray OdemeEmriMIFTekBelgeListele(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().Listele(kullanan, form, true, false, false);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Belge durum bilgisi
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <returns></returns>
        public int BelgeDurumBilgisi(int yil, string belgeNo)
        {
            int donusDeger = 0;

            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().BelgeKimlikOku(null, yil, belgeNo).formDurum;
            }
            catch { donusDeger = 0; }

            return donusDeger;
        }

        /// <summary>
        /// Belge durum değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="formlar">İşlem yapılacak bilgileri içeren nesme</param>
        /// <returns></returns>
        public Sonuc BelgeDurumDegistirODEMIF(Kullanici kullanan, TNSCollection formlar, bool surecBlokeDuzenle)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().BelgeDurumDegistir(kullanan, formlar, surecBlokeDuzenle);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Belge tür değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="yeniBelgeTur">Yeni belge türü</param>
        /// <returns></returns>
        public Sonuc BelgeTurDegistir(Kullanici kullanan, OdemeEmriMIFForm form, int yeniBelgeTur)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().BelgeTurDegistir(kullanan, form, yeniBelgeTur);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödenek Gönderme/tenkis kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="MPRevizeDeYap">4 düzey Ödenek değişimi sırasında, MP revize de üretilecekse true, aksi halde false verilmelidir.</param>
        /// <returns></returns>
        public Sonuc GondermeTenkisKaydet(Kullanici kullanan, GondermeTenkisForm form, bool MPRevizeDeYap, bool donemOdenegi, bool sgbKbsKontrolYapma)
        {
            try
            {
                return new MUHGondermeTenkisServis().Kaydet(kullanan, form, false, MPRevizeDeYap, donemOdenegi, sgbKbsKontrolYapma);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.StackTrace + " " + e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Ödenek Gönderme/tenkis kontrolü
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc GondermeTenkisKontrol(Kullanici kullanan, GondermeTenkisForm form, bool donemOdenegi, bool sgbKbsKontrolYapma)
        {
            try
            {
                return new MUHGondermeTenkisServis().Kaydet(kullanan, form, true, false, donemOdenegi, sgbKbsKontrolYapma);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Ödenek Gönderme/tenkis listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="detay">if set to <c>true</c> [detay]</param>
        /// <returns></returns>
        public ObjectArray GondermeTenkisListele(Kullanici kullanan, GondermeTenkisForm form, int detay)
        {
            try
            {
                return new MUHGondermeTenkisServis().Listele(kullanan, form, false, detay);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Ödenek Gönderme/tenkis tek belge listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectVAN GondermeTenkisKimlikOku(Kullanici kullanan, GondermeTenkisForm form)
        {
            try
            {
                return new MUHGondermeTenkisServis().KimlikOku(kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN donusDeger = new ObjectVAN();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Ödenek Gönderme/tenkis tek belge listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray GondermeTenkisTekBelgeListele(Kullanici kullanan, GondermeTenkisForm form)
        {
            try
            {
                return new MUHGondermeTenkisServis().Listele(kullanan, form, true, 0);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Ödenek Gönderme/tenkis tek belge tek detay listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <param name="belgeTur">Belge turü</param>
        /// <param name="tertip">Tertip bilgisi</param>
        /// <returns></returns>
        public GondermeTenkisDetay GondermeTenkisTekBelgeTekDetayListele(Kullanici kullanan, int yil, string belgeNo, int belgeTur, string tertip)
        {
            GondermeTenkisDetay donusDeger = new GondermeTenkisDetay();
            try
            {
                donusDeger = new MUHGondermeTenkisServis().TekBelgeTekDetayGetir(yil, belgeNo, belgeTur, tertip);
            }
            catch
            {

            }

            return donusDeger;
        }

        /// <summary>
        /// Belge durum değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="formlar">İşlem yapılacak bilgileri içeren nesme</param>
        /// <returns></returns>
        public Sonuc BelgeDurumDegistirGonTen(Kullanici kullanan, TNSCollection formlar)
        {
            try
            {
                return new MUHGondermeTenkisServis().BelgeDurumDegistir(kullanan, formlar);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc TalepBelgeYapGonTen(Kullanici kullanan, TNSCollection formlar, bool sgbKbsKontrolYapma)
        {
            try
            {
                return new MUHGondermeTenkisServis().TalepBelgeYapGonTen(kullanan, formlar, sgbKbsKontrolYapma);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        ///// <summary>
        ///// Ödenek Gönderme/tenkis kontrol listesi
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        ///// <returns></returns>
        //public ObjectArray GondermeTenkisKontrolListesi(Kullanici kullanan, GondermeTenkisForm form)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new MUHGondermeTenkisServis().KontrolListesi(kullanan, form);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        /// <summary>
        /// Hesap planı kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlani">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        public Sonuc HesapPlaniKaydet(Kullanici kullanan, ObjectArray hesapPlani)
        {
            try
            {
                return new MUHHesapPlaniServis().Kaydet(kullanan, hesapPlani);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Hesap planı aktar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kaynakYil">Kaynak yıl</param>
        /// <param name="hedefYil">Hedef yıl</param>
        /// <returns></returns>
        public Sonuc HesapPlaniAktar(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHHesapPlaniServis().Aktar(kullanan, kaynakYil, hedefYil);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Hesaps plani sil
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlani">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        public Sonuc HesapPlaniSil(Kullanici kullanan, ObjectArray hesapPlani)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHHesapPlaniServis().Sil(kullanan, hesapPlani);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Hesap planı listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlaniSatir">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        public ObjectArray HesapPlaniListele(Kullanici kullanan, HesapPlaniSatir hesapPlaniSatir)
        {
            try
            {
                return new MUHHesapPlaniServis().Listele(kullanan, hesapPlaniSatir);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }

        }

        /// <summary>
        /// Hesap planı detay sayıları düzenle
        /// </summary>
        /// <param name="yil">Yıl</param>
        public void HesapPlaniDetaySayilariDuzenle(int yil)
        {
            new MUHHesapPlaniServis().HesapPlaniDetaySayilariDuzenle(null, yil);
        }

        /// <summary>
        /// Hesap kodunun özelliklerini al
        /// </summary>
        /// <param name="hKod">Hesap kodu</param>
        /// <returns></returns>
        public HesapPlaniSatir HesapKodOzellikAl(HesapPlaniSatir hKod)
        {
            HesapPlaniSatir donusDeger = new MUHHesapPlaniServis().OzellikAl(null, hKod);

            return donusDeger;
        }

        ///// <summary>
        ///// Hesap planı ebutceden aktar
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="yil">Yıl</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <param name="hesapKod">Hesap kodu</param>
        ///// <returns></returns>
        //public Sonuc HesapPlaniEbutcedenAktar(Kullanici kullanan, string yil, string kurum, string hesapKod)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new MUHHesapPlaniServis().EbutcedenAktar(kullanan, yil, kurum, hesapKod);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        /// <summary>
        /// Hesap plani nitelik değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlaniSatir">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        public Sonuc HesapPlaniNitelikDegistir(Kullanici kullanan, HesapPlaniSatir hesapPlaniSatir, int nitelikIslemi)
        {
            try
            {
                return new MUHHesapPlaniServis().NitelikDegistir(kullanan, hesapPlaniSatir, nitelikIslemi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Hesap planı yansıtma listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlani">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        public ObjectArray HesapPlaniYansitmaListele(Kullanici kullanan, ObjectArray hesapPlani)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHHesapPlaniServis().YansitmaListele(kullanan, hesapPlani);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Kullanıcının yetkili olduğu muhasebe birimleri
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <returns></returns>
        public ObjectArray KullaniciMuhasebeBirimleri(Kullanici kullanan, int muhasebe, string kurum, string kistasKodAd)
        {
            try
            {
                return new MUHMuhasebeBirimServis().KullaniciMuhasebeBirimleri(kullanan, muhasebe, kurum, kistasKodAd);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Kullanıcının yetkili olduğu muhasebe birimleri
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <returns></returns>
        public ObjectArray MuhasebeBirimListeleSablondan(Kullanici kullanan, string sablon)
        {
            try
            {
                return new MUHMuhasebeBirimServis().MuhasebeBirimListeleSablondan(kullanan, sablon);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Kullanıcının yetkili olduğu muhasebe birimleri
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <returns></returns>
        public ObjectArray KullaniciMuhasebeBirimleri(Kullanici kullanan, int muhasebe, string kurum)
        {
            return KullaniciMuhasebeBirimleri(kullanan, muhasebe, kurum, null);
        }


        /// <summary>
        /// Kullanıcıya ait Harcama birimlerini listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yetkiTipiAdi">yetki tipi adi</param>
        /// <returns></returns>
        public ObjectArray KullanicininHarcamaBirimleri(Kullanici kullanan, string yetkiTipiAdi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHMuhasebeBirimServis().KullanicininHarcamaBirimleri(kullanan, yetkiTipiAdi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Harcama birimine ait kurumsal kod listesi
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        public ABSKodlarVAN HarcamaBirimleriKurumsalKod(int yil, string kurum, int muhasebe, string birim)
        {
            ABSKodlarVAN donusDeger = new ABSKodlarVAN();
            try
            {
                donusDeger = new MUHMuhasebeBirimServis().HarcamaBirimleriKurumsalKod(null, yil, kurum, muhasebe, birim);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Imza kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="imzaBilgileri">İmza bilgileri</param>
        /// <returns></returns>
        public Sonuc ImzaKaydet(Kullanici kullanan, ObjectArray imzaBilgileri)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHImzaServis().Kaydet(kullanan, imzaBilgileri);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Imza listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belgeTur">Belge türü</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        public ObjectArray ImzaListele(Kullanici kullanan, ENUMBelgeTuru belgeTur, int muhasebe, string birim)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHImzaServis().Listele(kullanan, belgeTur, muhasebe, birim);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Nakit talebi kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">İşlem yapılacak bilgileri içeren nesne</param>
        /// <param name="kontrolSekli">Kontrol şekli</param>
        /// <returns></returns>
        public Sonuc NakitTalebiKaydet(Kullanici kullanan, ObjectArray bilgi, string kontrolSekli)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHNakitTalebi().Kaydet(kullanan, bilgi, kontrolSekli);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Nakit talebi listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">İşlem yapılacak bilgileri içeren nesne</param>
        /// <param name="kontrolSekli">Kontrol şekli</param>
        /// <returns></returns>
        public ObjectArray NakitTalebiListele(Kullanici kullanan, NakitTalebi bilgi, string kontrolSekli)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHNakitTalebi().Listele(kullanan, bilgi, kontrolSekli);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Tarihce listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeTur">Belge tür</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <returns></returns>
        public ObjectArray TarihceListele(Kullanici kullanan, int yil, ENUMBelgeTuru belgeTur, string belgeNo)
        {
            try
            {
                return new MUHTarihceServis().Listele(kullanan, yil, belgeTur, belgeNo);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Yansıtma bilgileri kaydet
        /// </summary>
        /// <param name="yansitmalar">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc YansitmaKaydet(ObjectArray yansitmalar)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHYansitmaServis().Kaydet(yansitmalar);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Yansitma listesi aktar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kaynakYil">Kaynak yil</param>
        /// <param name="hedefYil">Hedef yil</param>
        /// <returns></returns>
        public Sonuc YansitmaAktar(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHYansitmaServis().Aktar(kullanan, kaynakYil, hedefYil);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Yansıtma bilgileri sil
        /// </summary>
        /// <param name="yansitmalar">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc YansitmaSil(ObjectArray yansitmalar)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHYansitmaServis().Sil(yansitmalar);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Yansitma bilgilerini listele
        /// </summary>
        /// <param name="yansitma">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray YansitmaListele(HesapPlaniYansitma yansitma)
        {
            try
            {
                return MUHYansitmaServis.Listele(null, yansitma, false);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Ekonomik koddan hesap planı kodlarını getir
        /// </summary>
        /// <param name="ekoKod">eko kod</param>
        /// <returns></returns>
        public HesapPlaniYansitma[] EkonomikKoddanHesapKodlariGetir(string ekoKod)
        {
            HesapPlaniYansitma[] donusDeger = new HesapPlaniYansitma[0];
            try
            {
                donusDeger = new MUHYansitmaServis().EkonomikKoddanHesapKodlariGetir(ekoKod);
            }
            catch
            {
                donusDeger = new HesapPlaniYansitma[0];
            }

            return donusDeger;
        }

        /// <summary>
        /// Yansitma bilgilerini listele
        /// </summary>
        /// <param name="yansitma">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray YansitmaEkoKodListele(HesapPlaniYansitma yansitma)
        {
            try
            {
                return MUHYansitmaServis.ListeleEkoKod(null, yansitma);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        ///// <summary>
        ///// Yansitma listesini ebutceden aktar
        ///// </summary>
        ///// <param name="yil">Yıl</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <param name="hesapKod">Hesap kodu</param>
        ///// <param name="yansitmaHesapKod">Yansitma hesap kodu</param>
        ///// <returns></returns>
        //public Sonuc YansitmaEbutcedenAktar(string yil, string kurum, string hesapKod, string yansitmaHesapKod)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new MUHYansitmaServis().EbutcedenAktar(yil, kurum, hesapKod, yansitmaHesapKod);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        /// <summary>
        /// Muhasebe bilgilerini kaydet
        /// </summary>
        /// <param name="muhasebeBilgileri">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc MuhasebeKaydet(ObjectArray muhasebeBilgileri)
        {
            try
            {
                return MUHMuhasebeServis.Kaydet(muhasebeBilgileri);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Muhasebe bilgisini sil
        /// </summary>
        /// <param name="muhasebeBilgileri">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc MuhasebeSil(ObjectArray muhasebeBilgileri)
        {
            try
            {
                return MUHMuhasebeServis.Sil(muhasebeBilgileri);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Muhasebe bilgisi listele
        /// </summary>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <returns></returns>
        public ObjectArray MuhasebeListele(MuhasebeBilgisi muhasebe)
        {
            try
            {
                return MUHMuhasebeServis.Listele(null, muhasebe);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Muhasebe birimi kaydet.
        /// </summary>
        /// <param name="muhasebeBirimler">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc MuhasebeBirimKaydet(ObjectArray muhasebeBirimler)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHMuhasebeBirimServis().Kaydet(muhasebeBirimler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        ///// <summary>
        ///// Muhasebe birim bilgisini ebutceden aktar.
        ///// </summary>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <param name="muhasebe">Muhasebe kodu</param>
        ///// <param name="kod">Muhasebe birim kodu</param>
        ///// <param name="ad">Muhasebe birim adı</param>
        ///// <returns></returns>
        //public Sonuc MuhasebeBirimEbutcedenAktar(string kurum, string muhasebe, string kod, string ad)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new MUHMuhasebeBirimServis().EbutcedenAktar(kurum, muhasebe, kod, ad);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        /// <summary>
        /// Muhasebe birimi sil.
        /// </summary>
        /// <param name="muhasebeBirimler">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc MuhasebeBirimSil(ObjectArray muhasebeBirimler)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHMuhasebeBirimServis().Sil(muhasebeBirimler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Muhasebe birim listele.
        /// </summary>
        /// <param name="muhasebeBirim">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray MuhasebeBirimListele(MuhasebeBirim muhasebeBirim)
        {
            try
            {
                return new MUHMuhasebeBirimServis().Listele(muhasebeBirim);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Muhasebe birim listele.
        /// </summary>
        /// <param name="muhasebeBirim">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        internal static MuhasebeBirim VNOdanMuhasebeBirimListele(MuhasebeBirim muhasebeBirim)
        {
            try
            {
                return MUHMuhasebeBirimServis.ListeleMuhBirim(null, muhasebeBirim);
            }
            catch
            {
                return new MuhasebeBirim();
            }
        }

        public MuhasebeBirim[] MuhasebeBirimListele(Kullanici kullanan, int saymanlikKod, string birimTurler)
        {
            return MUHMuhasebeBirimServis.Listele(null, kullanan, saymanlikKod, birimTurler);
        }

        //public Sonuc FirmaKaydet(FirmaBilgileri firmaBilgileri)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new MUHFirmaServis().Kaydet(firmaBilgileri);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc FirmaSil(FirmaBilgileri firmaBilgileri)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new MUHFirmaServis().Sil(firmaBilgileri);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        public ObjectArray AltHesapListeHazirla(Kullanici kullanan, int muhasebe, string kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().AltHesapListeHazirla(kullanan, muhasebe, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Gider Bütçesi kaydet
        /// </summary>
        /// <param name="bilgi">İşlem yapılacak bilgileri içeren nesne</param>
        /// <returns></returns>
        public Sonuc ButceGiderKaydet(ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHButceGider().Kaydet(bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Gider Bütçesi listele
        /// </summary>
        /// <param name="kriter">Kriter</param>
        /// <returns></returns>
        public ObjectArray ButceGiderListele(ButceGider kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHButceGider().Listele(kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Gelir Bütçesi finansman kaydet
        /// </summary>
        /// <param name="bilgi">İşlem yapılacak bilgileri içeren nesne</param>
        /// <returns></returns>
        public Sonuc ButceGelirFinansmanKaydet(ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHButceGelirFinansman().Kaydet(bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Gelir Bütçesi finansman listele
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        public ObjectArray ButceGelirFinansmanListele(ButceGelirFinansman kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHButceGelirFinansman().Listele(kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Sablon icerik tür liste.
        /// </summary>
        /// <returns></returns>
        public ObjectArray SablonIcerikTurListe()
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHSablonServis().IcerikTurListe();
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Sablon kaydet.
        /// </summary>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc SablonKaydet(Sablon form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHSablonServis().Kaydet(form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Sablon sil.
        /// </summary>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc SablonSil(Sablon form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHSablonServis().Sil(form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Sablon listele.
        /// </summary>
        /// <param name="sablon">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray SablonListe(Sablon sablon)
        {
            try
            {
                return new MUHSablonServis().Liste(sablon);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }

        /// <summary>
        /// Bütçe gider rakam olustur.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="serbestMi">Eğer <c>true</c> ise serbest ödenek tutarını ödenek kadar oluştur</param>
        /// <returns></returns>
        public Sonuc ButceGiderRakamOlustur(Kullanici kullanan, int yil, bool serbestMi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdenekHarcamaServis().ButceGiderRakamOlustur(kullanan, yil, serbestMi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Gelir Bütçesi rakam olustur.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="serbestMi">Eğer <c>true</c> ise serbest ödenek tutarını ödenek kadar oluştur</param>
        /// <returns></returns>
        public Sonuc ButceGelirRakamOlustur(Kullanici kullanan, int yil, bool serbestMi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdenekHarcamaServis().ButceGelirRakamOlustur(kullanan, yil, serbestMi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Harcama listele.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        public ObjectArray OdenekDurum(OdenekDurumKriter kriter)
        {
            try
            {
                return new MUHOdenekHarcamaServis().OdenekDurum(kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Ödenek durum bilgisi listesi.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray OdenekDurumListesi(OdenekDurumKriter kriter)
        {
            try
            {
                return new MUHOdenekHarcamaServis().OdenekDurumListesi(kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Harcama durum bilgisi listele.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray HarcamaDurumListesi(Kullanici kullanan, OdenekDurumKriter kriter)
        {
            try
            {
                return new MUHOdenekHarcamaServis().HarcamaDurumListesi(kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Harcama listele.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        public ObjectArray HarcamaListele(OdenekDurumKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdenekHarcamaServis().HarcamaListele(kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Gelir durum listesi.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray GelirDurumListesi(OdenekDurumKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdenekHarcamaServis().GelirDurumListesi(kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Bütce durumu bilgilendirme.
        /// </summary>
        /// <param name="degerler">ButceDurumuVAN tipindeki collection</param>
        /// <returns>İşlem başarılı olmuşsa ButceDurumuVAN collectionunda degerler döner.Hata oluşmuşsa Sonuc yapısı döndürülür.</returns>
        public ObjectArray ButceDurumuBilgilendirme(ObjectArray degerler)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdenekHarcamaServis().ButceDurumuBilgilendirme(degerler);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödenek üstü harcama listesi.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray OdenekUstuHarcamaListesi(OdenekDurumKriter kriter)
        {
            try
            {
                return new MUHOdenekHarcamaServis().OdenekUstuHarcamaListesi(kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Muhasebattan gelen bilgilerden bütçe gideri fon eko liste.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray MuhasebatButceGideriFonEkoListe(OdenekDurumKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdenekHarcamaServis().MuhasebatButceGideriFonEkoListe(kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Tertip listele.
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="eko">Ekonomik kod</param>
        /// <returns></returns>
        public ObjectArray TertipListele(int yil, string kurum, int muhasebe, string birim, string eko)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().TertipListele(yil, kurum, muhasebe, birim, eko);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// 4 kırımlı olarak Tertip listele 
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="eko">Ekonomik Kod</param>
        /// <returns></returns>
        public ObjectArray TertipListeleEko4(int yil, string kurum, int muhasebe, string birim, string eko)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().TertipListeleEko4(yil, kurum, muhasebe, birim, eko);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Performans bütçe tertiplerini listele 
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="eko">Ekonomik Kod</param>
        /// <param name="peb">Peb kodu</param>
        /// <returns></returns>
        public ObjectArray TertipListeleEko4Peb(int yil, string kurum, int muhasebe, string birim, string eko, string peb)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().TertipListeleEko4Peb(yil, kurum, muhasebe, birim, eko, peb);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre bilanço raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="tarih1">The tarih1.</param>
        /// <param name="tarih2">The tarih2.</param>
        /// <returns></returns>
        public ObjectArray Bilanco(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, int tur, int fisTur, TNSDateTime tarih1, TNSDateTime tarih2)
        {
            try
            {
                return TNS.MUH.IsKurallari.MUHMizanBilanco.Bilanco(null, kullanan, yil, kurum, saymanlikKodu, birim, tur, fisTur, tarih1, tarih2);
            }
            catch (Exception exp)
            {
                ObjectArray bc = new ObjectArray();
                bc.objeler.Add(new Sonuc(false, exp.Message, ""));
                return bc;
            }
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre mizan raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <param name="seviye">hesap kodu Seviyesi</param>
        /// <param name="basHesap">Başlangıç hesap kodu</param>
        /// <param name="bitHesap">Bitiş hesap kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <returns></returns>
        public MizanBilgi Mizan(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string altHesap, bool ucgen = false)
        {
            try
            {
                return TNS.MUH.IsKurallari.MUHMizanBilanco.Mizan(null, kullanan, yil, kurum, saymanlikKodu, birim, tarih1, tarih2, seviye, basHesap, bitHesap, tur, fisTur, listeTur, altHesap, ucgen);
            }
            catch (Exception ex)
            {
                MizanBilgi bc = new MizanBilgi(0, 0);
                bc.sonuc.hataStr = ex.Message;
                bc.sonuc.islemSonuc = false;
                return bc;
            }
        }

        public MizanBilgi MizanToplamBul(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string altHesap)
        {
            try
            {
                return TNS.MUH.IsKurallari.MUHMizanBilanco.MizanToplamBul(null, kullanan, yil, kurum, saymanlikKodu, birim, tarih1, tarih2, seviye, basHesap, bitHesap, tur, fisTur, listeTur, altHesap);
            }
            catch (Exception ex)
            {
                MizanBilgi bc = new MizanBilgi(0, 0);
                bc.sonuc.hataStr = ex.Message;
                bc.sonuc.islemSonuc = false;
                return bc;
            }
        }

        public Sonuc MuhasebeSorgu(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string kurKod, string fonKod, string altHesap, int borcAlacak)
        {
            try
            {
                return TNS.MUH.IsKurallari.MUHMizanBilanco.MuhasebeSorgu(kullanan, yil, kurum, saymanlikKodu, birim, tarih1, tarih2, seviye, basHesap, bitHesap, tur, fisTur, listeTur, kurKod, fonKod, altHesap, borcAlacak);
            }
            catch (Exception ex)
            {
                return new Sonuc(ex.Message);
            }
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre birim bazında mizan raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <param name="seviye">hesap kodu Seviyesi</param>
        /// <param name="basHesap">Başlangıç hesap kodu</param>
        /// <param name="bitHesap">Bitiş hesap kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <returns></returns>
        public MizanBilgi MizanBirim(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string kurKod = "", string fonKod = "", string altHesap = "")
        {
            try
            {
                return TNS.MUH.IsKurallari.MUHMizanBilanco.MizanBirim(null, kullanan, yil, kurum, saymanlikKodu, birim, tarih1, tarih2, seviye, basHesap, bitHesap, tur, fisTur, listeTur, kurKod, fonKod, altHesap);
            }
            catch (Exception exp)
            {
                MizanBilgi bc = new MizanBilgi(0, 0);
                bc.sonuc.hataStr = exp.Message;
                bc.sonuc.islemSonuc = false;
                return bc;
            }
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre tertip bazında mizan raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <param name="seviye">Hesap kodu Seviyesi</param>
        /// <param name="basHesap">Başlangıç hesap kodu</param>
        /// <param name="bitHesap">Bitiş hesap kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="listeTur"></param>
        /// <returns></returns>
        public MizanBilgi MizanTertip(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string altHesap, bool gercekEkoKodVer, int yevmiyeNo1, int yevmiyeNo2)
        {
            try
            {
                return TNS.MUH.IsKurallari.MUHMizanBilanco.MizanTertip(null, kullanan, yil, kurum, saymanlikKodu, birim, tarih1, tarih2, seviye, basHesap, bitHesap, tur, fisTur, listeTur, altHesap, gercekEkoKodVer, yevmiyeNo1, yevmiyeNo2);
            }
            catch (Exception exp)
            {
                MizanBilgi bc = new MizanBilgi(0, 0);
                bc.sonuc.hataStr = exp.Message;
                bc.sonuc.islemSonuc = false;
                return bc;
            }
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre büyük defter raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public BuyukDefter[] BuyukDefterListele(Kullanici kullanan, OdemeEmriMIFForm form, int basla, int bitis, string raporTur)
        {
            try
            {
                return new MUHMizanBilanco().BuyukDefterListele(kullanan, form, basla, bitis, raporTur);
            }
            catch
            {
                return new BuyukDefter[0];
            }
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre büyük defter devreden rakamlar raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public double[] BuyukDefterDevreden(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            double[] donusDeger = new double[2];
            try
            {
                donusDeger = new MUHMizanBilanco().BuyukDefterDevreden(kullanan, form);
            }
            catch { }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gider tertip) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray KesinHesapGiderTertipListele(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHMizanBilanco().KesinHesapGiderTertipListele(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gider) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="kolonDeger">kolon deger</param>
        /// <returns></returns>
        public ObjectArray KesinHesapGiderRakamVer(Kullanici kullanan, OdemeEmriMIFForm form, string[] kolonDeger)
        {
            try
            {
                return new MUHMizanBilanco().KesinHesapGiderRakamVer(kullanan, form, kolonDeger);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gelir tertip) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray KesinHesapGelirTertipListele(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHMizanBilanco().KesinHesapGelirTertipListele(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gelir) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray KesinHesapGelirRakamVer(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHMizanBilanco().KesinHesapGelirRakamVer(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Bütçe) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="kodTur">kod tur</param>
        /// <returns></returns>
        public ObjectArray KesinHesapButce(Kullanici kullanan, OdemeEmriMIFForm form, int kodTur)
        {
            try
            {
                return new MUHMizanBilanco().KesinHesapButce(kullanan, form, kodTur);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }

        }

        /// <summary>
        /// Verilen kriter bilgilerine göre ödeme emri belgelerinde 830-905 hesap kodlarının kullanımını raporlar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        public OdemeEmriMIFForm Kontrol830905(TNS.KYM.Kullanici kullanan, int yil, int saymanlikKodu, string birim)
        {
            try
            {
                return TNS.MUH.IsKurallari.MUHMizanBilanco.Kontrol830905(null, kullanan, yil, saymanlikKodu, birim);
            }
            catch (Exception exp)
            {
                OdemeEmriMIFForm bc = new OdemeEmriMIFForm();
                bc.aciklama = exp.Message;
                return bc;
            }
        }

        /// <summary>
        /// Ödenek üstü raporunu hazırlar.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        public OdemeEmriMIFForm OdenekUstu(TNS.KYM.Kullanici kullanan, int yil, int saymanlikKodu, string birim)
        {
            try
            {
                return TNS.MUH.IsKurallari.MUHMizanBilanco.OdenekUstu(null, kullanan, yil, saymanlikKodu, birim);
            }
            catch (Exception exp)
            {
                OdemeEmriMIFForm bc = new OdemeEmriMIFForm();
                bc.aciklama = exp.Message;
                return bc;
            }
        }

        /// <summary>
        /// Hatalı hesap kodu kullanılan ödeme emri belgelerini raporlar.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        public OdemeEmriMIFForm UstHesapKodKullanilanlar(TNS.KYM.Kullanici kullanan, int yil, int saymanlikKodu, string birim)
        {
            try
            {
                return TNS.MUH.IsKurallari.MUHMizanBilanco.UstHesapKodKullanilanlar(null, kullanan, yil, saymanlikKodu, birim);
            }
            catch (Exception exp)
            {
                OdemeEmriMIFForm bc = new OdemeEmriMIFForm();
                bc.aciklama = exp.Message;
                return bc;
            }
        }

        /// <summary>
        /// Muhasebata ait harcama rakamlarını aktarma işlemi
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="ay">ay</param>
        /// <param name="kurum">Kurum kodu</param>
        //public Sonuc MuhasebatAktar(Kullanici kullanan, string yil, string ay, string kurum)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    try
        //    {
        //        if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //        {
        //            sonuc.hataStr = "Harcama bilgilerini çekmeye yetkili değilsiniz.";
        //            return sonuc;
        //        }
        //        TNS.MUH.IsKurallari.MUHebutceIslemleri a = new MUHebutceIslemleri();
        //        sonuc = a.ebutceMuhasebatKontrol(kullanan, yil, ay, kurum);
        //    }
        //    catch (Exception e) { sonuc.hataStr = e.Message; return sonuc; }

        //    return sonuc;
        //}

        /// <summary>
        /// e-bütçe sisteminden Muhasebata ait harcama rakamlarını aktarma işlemi
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="ay">ay</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <returns></returns>
        public Sonuc HarcamaAktar(Kullanici kullanan, string yil, string ay, string kurum)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            try
            {
                if (kullanan.ad != "aktarim")
                {
                    if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                    {
                        sonuc.hataStr = "Harcama bilgilerini çekmeye yetkili değilsiniz.";
                        return sonuc;
                    }
                }

                int iYil = OrtakFonksiyonlar.ConvertToInt(yil, 0);
                int iAy = OrtakFonksiyonlar.ConvertToInt(ay, 0);

                TNS.MUH.IsKurallari.MUHProgramButceIslemleri a = new MUHProgramButceIslemleri();

                if (iYil >= 2021)
                    sonuc = a.ebutceHarcamaKontrolPrg(kullanan, iYil, iAy, kurum, "");
                //else
                //    sonuc = a.ebutceHarcamaKontrolYeni2(kullanan, yil, ay, kurum, "");

            }
            catch (Exception e) { sonuc.hataStr = e.Message; return sonuc; }

            return sonuc;
        }

        /// <summary>
        /// e-bütçe sisteminden Ödenek gönderme belgelerinin durumlarını kontrol eder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="tur">tur</param>
        public void OdenekGondermeEButcedenKontrol(Kullanici kullanan, int yil, int tur)
        {
            try
            {
                if (kullanan.ad != "aktarim")
                {
                    if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                        return;
                }

                TNS.MUH.IsKurallari.MUHSBBIslemleri a = new MUHSBBIslemleri();
                a.ebutceOdenekGondermeKontrol(kullanan, yil, tur);

                //Eğer dosya var ise içeri girme daha önce giren işlem yapıyor
                //if (OrtakFonksiyonlar.HataDosyaOku("OdenekGondermeCalisiyor.txt").Length > 0)
                //    return;
                //İşlem yapan yok ise dosyayı yarat
                //OrtakFonksiyonlar.HataDosyaYaz("OdenekGondermeCalisiyor.txt", "Lütfen dosyayı silmeyin", true);

                //İşlem bitti dosyayı sil
                //OrtakFonksiyonlar.HataDosyaSil("OdenekGondermeCalisiyor.txt");
            }
            catch (Exception e)
            {
                //OrtakFonksiyonlar.HataDosyaSil("OdenekGondermeCalisiyor.txt");
                AktarimLogYaz("OdenekGondermeKontrol-->" + e.ToString());
            }
        }

        public Sonuc OdenekGondermeAktarimIslemleri(Kullanici kullanici, int yil)
        {
            Sonuc s = new Sonuc(); ;
            try
            {
                OdenekGondermeEbutceyeGonder(kullanici, yil);
                s.DegerAta("Ödenek Gönderme Bitti");
            }
            catch (Exception ex)
            {
                s.DegerAta((yil) + " / " + "Ödenek Gönderme Kaydet" + " Hata: " + ex.Message);
            }

            try
            {
                OdenekGondermeEButcedenKontrol(kullanici, yil, (int)TNS.MUH.ENUMBelgeDurumu.MUHASEBATALDI);
                s.DegerAta("MUHASEBATALDI Bitti");
            }
            catch (Exception ex)
            {
                s.DegerAta((yil) + " / " + TNS.MUH.ENUMBelgeDurumu.MUHASEBATALDI + " Hata: " + ex.Message);
            }

            //try
            //{
            //    OdenekGondermeEButcedenKontrol(kullanici, yil, (int)TNS.MUH.ENUMBelgeDurumu.MUHASEBATIADE);
            //    s.DegerAta("MUHASEBAT İade");
            //}
            //catch (Exception ex)
            //{
            //    s.DegerAta((yil) + " / " + TNS.MUH.ENUMBelgeDurumu.MUHASEBATIADE + " Hata: " + ex.Message);
            //}

            //try
            //{
            //    OdenekGondermeEButcedenKontrol(kullanici, yil, (int)TNS.MUH.ENUMBelgeDurumu.MUHASEBATAGONDERILDI);
            //    s.DegerAta("MUHASEBATAGONDERILDI Bitti");
            //}
            //catch (Exception ex)
            //{
            //    s.DegerAta((yil) + " / " + TNS.MUH.ENUMBelgeDurumu.MUHASEBATAGONDERILDI + " Hata: " + ex.Message);
            //}

            return s;
        }


        /// <summary>
        /// Ödenek gönderme belgelerini e-bütçe sistemine kayıt eder
        /// </summary>
        /// <param name="yil">Yıl</param>
        public void OdenekGondermeBelgeleriGonder(Kullanici kullanan, int yil, ENUMBelgeTuru belgeTur, string belgeNolar)
        {
            TNS.MUH.IsKurallari.MUHSBBIslemleri a = new MUHSBBIslemleri();

            try
            {
                a.programButceOdenekGondermeBulGonder(kullanan, yil, belgeTur, belgeNolar);
            }
            catch { }
        }


        /// <summary>
        /// Ödenek gönderme belgelerini e-bütçe sistemine kayıt eder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        public void OdenekGondermeEbutceyeGonder(Kullanici kullanan, int yil)
        {
            try
            {
                if (kullanan.ad != "aktarim")
                    if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                        return;

                TNS.MUH.IsKurallari.MUHSBBIslemleri a = new MUHSBBIslemleri();
                try
                {
                    a.programButceOdenekGondermeGonder(kullanan, yil);
                }
                catch { }
            }
            catch (Exception e)
            {
                AktarimLogYaz("OdenekGondermeKontrol-->" + e.ToString());
            }
        }

        /// <summary>
        /// e-bütçe sisteminde bulunan ödenek gönderme belgelerini listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="ay">ay</param>
        /// <param name="tur">tur</param>
        //public void OdenekGondermeEbutcedenListele(Kullanici kullanan, int yil, int ay, int tur)
        //{
        //    try
        //    {
        //        if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //            return;

        //        TNS.MUH.IsKurallari.MUHebutceIslemleri a = new MUHebutceIslemleri();
        //        try
        //        {
        //            a.ebutceOdenekGondermeListele(kullanan, yil, ay, tur);
        //        }
        //        catch { }
        //    }
        //    catch (Exception e)
        //    {
        //        AktarimLogYaz("OdenekGondermeListe-->" + e.ToString());
        //    }
        //}

        public static string DosyaAdBul(bool thread)
        {
            string ek;
            if (thread) ek = "Task";
            else ek = "";

            return "OdenekGondermeKontrol" + DateTime.Now.ToString("yyMMddHH") + ek + ".txt";
        }


        /// <summary>
        /// e-bütçe sisteminden Ödenek rakamlarını aktarır.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="asama">asama</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <returns></returns>
        //public Sonuc OdenekAktar(Kullanici kullanan, string yil, string asama, string kurum)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    try
        //    {
        //        if (kullanan.ad != "aktarim")
        //        {
        //            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //            {
        //                sonuc.hataStr = "Ödenek bilgilerini çekmeye yetkili değilsiniz.";
        //                return sonuc;
        //            }
        //        }
        //        TNS.MUH.IsKurallari.MUHebutceIslemleri a = new MUHebutceIslemleri();
        //        sonuc = a.ebutceOdenekKontrol(yil, asama, kurum);
        //    }
        //    catch (Exception e) { sonuc.hataStr = e.Message; return sonuc; }

        //    return sonuc;
        //}

        /// <summary>
        /// e-bütçe sisteminden bütçe kodlarını aktarır
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kodTur">kod tur</param>
        /// <param name="ustKod">ust kod</param>
        /// <param name="seviye">seviye</param>
        //public Sonuc ButceKodAktar(Kullanici kullanan, string yil, string kodTur, string ustKod, int seviye)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    try
        //    {
        //        if (kullanan.ad != "aktarim")
        //        {
        //            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //            {
        //                sonuc.hataStr = "Ödenek bilgilerini çekmeye yetkili değilsiniz.";
        //                return sonuc;
        //            }
        //        }
        //        TNS.MUH.IsKurallari.MUHebutceIslemleri a = new MUHebutceIslemleri();
        //        sonuc = a.ebutceKodKontrol(yil, kodTur, ustKod, seviye, true);
        //    }
        //    catch (Exception e) { sonuc.hataStr = e.Message; return sonuc; }

        //    return sonuc;
        //}

        /// <summary>
        /// e-bütçe sisteminden bütçe rakamlarını aktarır
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        //public Sonuc ButceAktar(Kullanici kullanan, string yil, string kurum)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    try
        //    {
        //        if (kullanan.ad != "aktarim")
        //        {
        //            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //            {
        //                sonuc.hataStr = "Ödenek bilgilerini çekmeye yetkili değilsiniz.";
        //                return sonuc;
        //            }
        //        }
        //        TNS.MUH.IsKurallari.MUHebutceIslemleri a = new MUHebutceIslemleri();
        //        sonuc = a.ebutceButceKontrol(yil, kurum);
        //    }
        //    catch (Exception e) { sonuc.hataStr = e.Message; return sonuc; }

        //    return sonuc;
        //}

        /// <summary>
        /// Ebutce kullanıcı bilgilerini ebütçe sistemine bağlantı kurarak test eder.
        /// </summary>
        /// <param name="kullanici">The kullanici.</param>
        /// <param name="parola">The parola.</param>
        /// <returns></returns>
        public Sonuc EbutceParolaKontrol(string kullanici, string parola)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHProgramButceIslemleri().EbutceParolaKontrol(kullanici, parola);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// e-bütçe sisteminden bütçe işlem rakamlarını aktarır
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        //public Sonuc ButceIslemAktar(Kullanici kullanan, int yil, string kurum)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    try
        //    {
        //        if (kullanan.ad != "aktarim")
        //        {
        //            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //            {
        //                sonuc.hataStr = "Ödenek bilgilerini çekmeye yetkili değilsiniz.";
        //                return sonuc;
        //            }
        //        }
        //        TNS.MUH.IsKurallari.MUHebutceIslemleri a = new MUHebutceIslemleri();
        //        sonuc = a.ebutceButceIslemKontrol(yil, kurum);
        //    }
        //    catch (Exception e) { sonuc.hataStr = e.Message; return sonuc; }

        //    return sonuc;
        //}

        /// <summary>
        /// e-bütçe sisteminden AHP rakamlarını aktarır
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        //public Sonuc AHPAktar(Kullanici kullanan, int yil, string kurum)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    try
        //    {
        //        if (kullanan.ad != "aktarim")
        //        {
        //            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //            {
        //                sonuc.hataStr = "Ödenek bilgilerini çekmeye yetkili değilsiniz.";
        //                return sonuc;
        //            }
        //        }
        //        TNS.MUH.IsKurallari.MUHebutceIslemleri a = new MUHebutceIslemleri();
        //        sonuc = a.ebutceAHPKontrol(yil, kurum);
        //    }
        //    catch (Exception e) { sonuc.hataStr = e.Message; return sonuc; }

        //    return sonuc;
        //}

        /// <summary>
        /// Makbuz tipi kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipleri">makbuz tipleri</param>
        /// <returns></returns>
        public Sonuc MakbuzTipiKaydet(Kullanici kullanan, ObjectArray makbuzTipleri)
        {
            try
            {
                return MUHMakbuzTipiServis.Kaydet(kullanan, makbuzTipleri);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }

        }

        /// <summary>
        /// Makbuz tipi sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipleri">makbuz tipleri</param>
        /// <returns></returns>
        public Sonuc MakbuzTipiSil(Kullanici kullanan, ObjectArray makbuzTipleri)
        {
            try
            {
                return MUHMakbuzTipiServis.Sil(kullanan, makbuzTipleri);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Makbuz tipi listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipi">makbuz tipi</param>
        /// <returns></returns>
        public ObjectArray MakbuzTipiListele(Kullanici kullanan, MakbuzTipi makbuzTipi)
        {
            try
            {
                return MUHMakbuzTipiServis.Listele(kullanan, makbuzTipi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public Sonuc MakbuzAciklamaKaydet(Kullanici kullanan, MakbuzAciklama bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHMakbuzServis.MakbuzAciklamaKaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Markas the listele.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray MakbuzAciklamaListele(MakbuzAciklama bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = MUHMakbuzServis.MakbuzAciklamaListele(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Markas the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc MakbuzAciklamaSil(Kullanici kullanan, MakbuzAciklama bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHMakbuzServis.MakbuzAciklamaSil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// Makbuz kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzlar">makbuzlar</param>
        /// <returns></returns>
        public Sonuc MakbuzKaydet(Kullanici kullanan, Makbuz makbuz)
        {
            try
            {
                return MUHMakbuzServis.Kaydet(null, kullanan, makbuz);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Makbuz durum guncelle.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzlar">makbuzlar</param>
        /// <returns></returns>
        public Sonuc MakbuzDurumGuncelle(Kullanici kullanan, TNSCollection makbuzlar)
        {
            try
            {
                return MUHMakbuzServis.DurumGuncelle(null, kullanan, makbuzlar);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Makbuz odeme emri guncelle.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzlar">makbuzlar</param>
        /// <returns></returns>
        public Sonuc MakbuzOdemeEmriGuncelle(Kullanici kullanan, ObjectArray makbuzlar)
        {
            try
            {
                return MUHMakbuzServis.OdemeEmriGuncelle(kullanan, makbuzlar);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc MakbuzOdendi(Kullanici kullanan, Makbuz makbuz)
        {
            try
            {
                return MUHMakbuzServis.MakbuzOdendi(kullanan, makbuz);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }

        }

        /// <summary>
        /// Makbuz listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuz">makbuz</param>
        /// <returns></returns>
        public ObjectArray MakbuzListele(Kullanici kullanan, Makbuz makbuz)
        {
            try
            {
                return MUHMakbuzServis.Listele(null, kullanan, makbuz);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public ObjectArray MakbuzBirimleriListele(Kullanici kullanan, Makbuz makbuz)
        {
            try
            {
                return MUHMakbuzServis.ListeleBirim(null, kullanan, makbuz);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public ObjectArray MakbuzKisileriListele(Kullanici kullanan, Makbuz makbuz)
        {
            try
            {
                return MUHMakbuzServis.ListeleKisi(null, kullanan, makbuz);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public ObjectArray MakbuzListeleMizan(Kullanici kullanan, Makbuz makbuz)
        {
            try
            {
                return MUHMakbuzServis.ListeleMizan(null, kullanan, makbuz);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Avans listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">avans</param>
        /// <returns></returns>
        public ObjectArray AvansListele(Kullanici kullanan, AvansBilgi bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().AvansListele(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödenek ihtiyaç kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihtiyac">ihtiyac</param>
        /// <returns></returns>
        public Sonuc OdenekIhtiyacKaydet(Kullanici kullanan, ObjectArray ihtiyac)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdenekIhtiyac().Kaydet(kullanan, ihtiyac);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödenek ihtiyaç listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihtiyac">ihtiyac</param>
        /// <returns></returns>
        public ObjectArray OdenekIhtiyacListele(Kullanici kullanan, OdenekIhtiyac ihtiyac)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdenekIhtiyac().Listele(kullanan, ihtiyac);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödenek ihtiyaç icmal olarak listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihtiyac">ihtiyac</param>
        /// <returns></returns>
        public ObjectArray OdenekIhtiyacListeleIcmal(Kullanici kullanan, OdenekIhtiyac ihtiyac)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdenekIhtiyac().ListeleIcmal(kullanan, ihtiyac);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödenek ihtiyaç onay kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="odenekIhtiyacOnay">odenek ihtiyac onay</param>
        /// <returns></returns>
        public Sonuc OdenekIhtiyacOnayKaydet(Kullanici kullanan, OdenekIhtiyacOnay odenekIhtiyacOnay)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdenekIhtiyacOnay().Kaydet(kullanan, odenekIhtiyacOnay);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödenek ihtiyaç onay listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="odenekIhtiyacOnay">odenek ihtiyac onay</param>
        /// <returns></returns>
        public ObjectArray OdenekIhtiyacOnayListele(Kullanici kullanan, OdenekIhtiyacOnay odenekIhtiyacOnay)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdenekIhtiyacOnay().Listele(kullanan, odenekIhtiyacOnay);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Muhasebe yetkilisi kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebeYetkilisi">muhasebe yetkilisi</param>
        /// <returns></returns>
        public Sonuc MuhasebeYetkilisiKaydet(Kullanici kullanan, MuhasebeYetkilisi muhasebeYetkilisi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHMuhasebeYetkilisi().Kaydet(kullanan, muhasebeYetkilisi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Muhasebe yetkilisi sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebeYetkilisi">muhasebe yetkilisi</param>
        /// <returns></returns>
        public Sonuc MuhasebeYetkilisiSil(Kullanici kullanan, MuhasebeYetkilisi muhasebeYetkilisi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHMuhasebeYetkilisi().Sil(kullanan, muhasebeYetkilisi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Muhasebe yetkilisi listele.
        /// </summary>
        /// <param name="muhasebeYetkilisi">muhasebe yetkilisi</param>
        /// <returns></returns>
        public ObjectArray MuhasebeYetkilisiListele(MuhasebeYetkilisi muhasebeYetkilisi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHMuhasebeYetkilisi().Listele(muhasebeYetkilisi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public Sonuc MIFEntegrasyonGonder(Kullanici kullanan, ObjectArray MIFListe)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHMuhasebatEntegrasyon().Gonder(kullanan, MIFListe);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Maaş bilgileri listele.(EGO için geliştirildi)
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray MaasEntegrasyonuListe(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().MaasEntegrasyouListe(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Maas bilgileri islendi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="fisNo">fis no</param>
        public void MaasEntegrasyonuIslendi(Kullanici kullanan, string fisNo)
        {
            try
            {
                new MUHOdemeEmriMIFServis().MaasEntegrasyouIslendi(kullanan, fisNo);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Harcamaların Aylık dağılımları.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        public ObjectArray AylikDagilimHarcama(AylikDagilim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdenekHarcamaServis().AylikDagilimHarcama(kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödenek gönderme  Aylık dağılımları.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        public ObjectArray AylikDagilimOdenekGonderme(AylikDagilim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdenekHarcamaServis().AylikDagilimOdenekGonderme(kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray YillikAylikDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().YillikAylikDagilimHesapKod(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray YillikAylikTarihselDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().YillikAylikTarihselDagilimHesapKod(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray BirimGelirGider(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().BirimGelirGider(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray GelirGiderListe(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().GelirGiderListe(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray EkoKod1Basamak(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().EkoKod1Basamak(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray AylikDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().AylikDagilimHesapKod(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray YillikDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().YillikDagilimHesapKod(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray BirimDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().BirimDagilimHesapKod(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray TasinirRaporu(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().TasinirRaporu(kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Talep kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">belge</param>
        /// <returns></returns>
        public Sonuc TalepKaydet(Kullanici kullanan, TalepBelge belge)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTalepServis().Kaydet(kullanan, belge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        ///// <summary>
        ///// Talep onay belgesi kaydet.
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="tertipListe">tertip liste</param>
        ///// <param name="talepYil">talep yil</param>
        ///// <param name="talepNo">talep no</param>
        ///// <param name="muhasebe">Muhasebe kodu</param>
        ///// <param name="birim">Birim kodu</param>
        ///// <param name="surecNo">Sürec no</param>
        ///// <returns></returns>
        //public Sonuc TalepKaydetOnayBelgesi(Kullanici kullanan, ObjectArray tertipListe, int talepYil, int talepNo, int muhasebe, string birim, string surecNo)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new MUHTalepServis().KaydetOnayBelgesiTalep(kullanan, tertipListe, talepYil, talepNo, muhasebe, birim, surecNo);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        /// <summary>
        /// Talep tarihçe kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tarihce">tarihce</param>
        /// <returns></returns>
        public Sonuc TalepTarihceKaydet(Kullanici kullanan, TalepTarihce tarihce)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTalepServis().KaydetTarihce(kullanan, tarihce);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Talep Ödenek gönderme belgesi kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="talepDetay">talep detay</param>
        /// <param name="yil">Yıl</param>
        /// <param name="talepNo">talep no</param>
        /// <returns></returns>
        public Sonuc TalepOdenekGondermeKaydet(Kullanici kullanan, ObjectArray talepDetay, int yil, int talepNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTalepServis().KaydetOdenekGonderme(kullanan, talepDetay, yil, talepNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Talep durum değiştir.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="talepNo">talep no</param>
        /// <param name="talepFormDurum">talep form durum</param>
        /// <returns></returns>
        public Sonuc TalepDurumDegistir(Kullanici kullanan, int yil, int talepNo, int talepFormDurum)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTalepServis().DurumDegistir(kullanan, yil, talepNo, talepFormDurum);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Talep listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">belge</param>
        /// <returns></returns>
        public ObjectArray TalepListele(Kullanici kullanan, TalepBelge belge)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHTalepServis().Listele(kullanan, belge);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Talep tek belge listele .
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="talepNo">talep no</param>
        /// <returns></returns>
        public TalepBelge TalepListeleTekBelge(Kullanici kullanan, int yil, int talepNo)
        {
            TalepBelge donusDeger = new TalepBelge();
            try
            {
                donusDeger = new MUHTalepServis().ListeleTekBelge(kullanan, yil, talepNo);
            }
            catch
            {
            }

            return donusDeger;
        }

        /// <summary>
        /// Kapanis bilgilerini kaydet
        /// </summary>
        /// <param name="kullanan">Kullanan.</param>
        /// <param name="liste">KapanisBilgileri türünde nesneleri içeren Liste.</param>
        /// <returns></returns>
        public Sonuc KapanisBilgisiKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().KapanisBilgisiKaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        /// <summary>
        /// Kapanis bilgilerini iptal eder
        /// </summary>
        /// <param name="kullanan">Kullanan.</param>
        /// <param name="liste">KapanisBilgileri türünde nesneleri içeren Liste.</param>
        /// <returns></returns>
        public Sonuc KapanisBilgisiIptalEt(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().KapanisBilgisiIptalEt(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KapanisBilgisiIptalEtHazirla(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().KapanisBilgisiIptalEtHazirla(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray KapanisBilgisiListele(KapanisBilgileri kriter)
        {
            ObjectArray sonuclar = new ObjectArray();
            try
            {
                sonuclar = MUHOdemeEmriMIFServis.KapanisBilgisiListele(kriter);
            }
            catch (Exception ex)
            {
                sonuclar.sonuc.islemSonuc = false;
                sonuclar.sonuc.hataStr = ex.ToString();
            }
            return sonuclar;
        }

        /// <summary>
        /// Kur farkı olan hesaplar için belge üretir
        /// </summary>
        /// <param name="kullanan">Kullanan.</param>
        /// <param name="belge">The belge.</param>
        /// <param name="muhasebeBirimineIsle"></param>
        /// <returns></returns>
        public Sonuc KurFarkiBelgeUret(Kullanici kullanan, OdemeEmriMIFForm belge, bool muhasebeBirimineIsle)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new MUHOdemeEmriMIFServis().KurFarkiBelgeUret(kullanan, belge, muhasebeBirimineIsle);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }

        }

        /// <summary>
        /// Yıl sonu hesap kapanış belgelerini ve verilen koşullara göre üretir
        /// </summary>
        /// <param name="kullanan">Kullanan.</param>
        /// <returns></returns>
        public ObjectArray YilSonuBelgeUret(Kullanici kullanan, OdemeEmriMIFForm form, int tur, string islem)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new MUHOdemeEmriMIFServis().YilSonuBelgeUret(kullanan, form, tur, islem);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }

        }

        /// <summary>
        /// ISLs the harcama listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray ISLHarcamaListele(Kullanici kullanan, ISLHarcama bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return MUHHarcama.Listele(null, kullanan, bilgi);
            }
            catch (Exception ex)
            {
                ObjectArray sonuclar = new ObjectArray();
                sonuclar.sonuc.islemSonuc = false;
                sonuclar.sonuc.hataStr = ex.ToString();
                return sonuclar;
            }
        }

        /// <summary>
        /// Butces the odenek kapat.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="belgeTarih">The belge tarih.</param>
        /// <returns></returns>
        public ObjectArray ButceOdenekKapat(Kullanici kullanan, int yil, int muhasebe, string birim, string belgeNo, TNSDateTime belgeTarih)
        {
            ObjectArray sonuclar = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                sonuclar = MUHOdemeEmriMIFServis.ButceOdenekKapat(kullanan, yil, muhasebe, birim, belgeNo, belgeTarih);
            }
            catch (Exception ex)
            {
                sonuclar.sonuc.islemSonuc = false;
                sonuclar.sonuc.hataStr = ex.ToString();
            }
            return sonuclar;
        }

        /// <summary>
        /// Odeneks the miktar getir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="kur">The kur.</param>
        /// <param name="fon">The fon.</param>
        /// <param name="fin">The fin.</param>
        /// <param name="eko">The eko.</param>
        /// <param name="peb">The peb.</param>
        /// <returns></returns>
        public double OdenekMiktarGetir(Kullanici kullanan, int muhasebe, string birim, int yil, string kur, string fon, string fin, string eko, string peb = "")
        {
            double donusDeger = 0;
            try
            {
                if (servisUZY == null)
                    servisUZY = TNS.UZY.Arac.Tanimla();

                TNS.KYM.Arac.KulturDegistir(kullanan);
                double odenek = 0.0;
                if (Arac.OdenekUstuKontrolBul() == ENUMOdenekUstuKontrol.ODENEK)
                    odenek = servisUZY.UzayDegeriDbl(kullanan, "ABSBUTCEGIDER", yil + "-" + kur + "-" + fon + "-" + fin + "-" + eko, false, "");
                else
                    odenek = servisUZY.UzayDegeriDbl(kullanan, "MUHGONDERMEBELGESITENKIS", yil + "----" + muhasebe + "-" + birim + "--5-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb, false, "");
                donusDeger = odenek;
            }
            catch { }

            return donusDeger;
        }

        /// <summary>
        /// Odeneks the talep yapilacak mi.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="tertip">The tertip.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="surecYil">The surec yil.</param>
        /// <param name="surecNo">The surec no.</param>
        /// <param name="odemeEmriBelgeNo">The odeme emri belge no.</param>
        /// <param name="borc">The borc.</param>
        /// <returns></returns>
        //public Sonuc OdenekTalepYapilacakMi(Kullanici kullanan, int muhasebe, string birim, string tertip, int yil, int surecYil, string surecNo, string odemeEmriBelgeNo, double borc)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new MUHOdemeEmriMIFServis().OdenekTalepYapilacakMi(kullanan, muhasebe, birim, tertip, yil, surecYil, surecNo, odemeEmriBelgeNo, borc);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        /// <summary>
        /// Odeneks the talep yapilacak mi.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="tertipler">The tertipler.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="surecYil">The surec yil.</param>
        /// <param name="surecNo">The surec no.</param>
        /// <param name="odemeEmriBelgeNo">The odeme emri belge no.</param>
        /// <returns></returns>
        public EksikOdenekler OdenekTalepYapilacakMi(Kullanici kullanan, int muhasebe, string birim, TertipOdenekBilgi[] tertipler, int yil, int surecYil, string surecNo, string odemeEmriBelgeNo, bool odemeEmriMi, bool dogrudanTeminMi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new MUHOdemeEmriMIFServis().OdenekTalepYapilacakMi(kullanan, muhasebe, birim, tertipler, yil, surecYil, surecNo, odemeEmriBelgeNo, odemeEmriMi, dogrudanTeminMi);
            }
            catch (Exception e)
            {
                EksikOdenekler eo = new EksikOdenekler();
                eo.sonuc = new Sonuc(false, e.ToString(), "");
                return eo;
            }
        }

        /// <summary>
        /// Makbuz listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuz">makbuz</param>
        /// <returns></returns>
        public ObjectArray YilSonuIslemleriListele(Kullanici kullanan, YilSonuIslem yilSonuIslem)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHYilSonuIslemleri().Listele(kullanan, yilSonuIslem, true);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Yevmiyes the defteri listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        //public DataTable YevmiyeDefteriListele(Kullanici kullanan, OdemeEmriMIFForm form)
        //{
        //    DataTable dt = new DataTable();

        //    try
        //    {
        //        OrtakFonksiyonlar.HataStrYaz("Yevmiye Listele Başla" + DateTime.Now.ToString());
        //        dt = new MUHOdemeEmriMIFServis().YevmiyeDefteriListele(kullanan, form);
        //        OrtakFonksiyonlar.HataStrYaz("Yevmiye Listele Bitti" + DateTime.Now.ToString());
        //    }
        //    catch { }

        //    return dt;
        //}

        public Sonuc BankaListesiDetayliKaydet(Kullanici kullanan, MUHBankaOdeme form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.Kaydet(null, kullanan, form);
        }

        public Sonuc BankaListesiEkle(Kullanici kullanan, int saymanlik, string birim, TNSDateTime dt, string aciklama, string muhYetkili, string muhYetUnvan, string onaylayan, string onayUnvan)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.Ekle(null, kullanan, saymanlik, birim, dt, aciklama, muhYetkili, muhYetUnvan, onaylayan, onayUnvan);
        }

        public Sonuc BankaListesiGuncelle(Kullanici kullanan, MUHBankaOdeme bilgi)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.Guncelle(null, kullanan, bilgi);
        }

        public Sonuc BankaListesiSil(Kullanici kullanan, MUHBankaOdeme bilgi)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.Sil(null, kullanan, bilgi);
        }

        public Sonuc BankaListesineOEEkle(Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay detay)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.OEEkle(null, kullanan, bilgi, detay);
        }

        public Sonuc BankaListesiBasilanCekNoAta(Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay detay)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.BasilanCekNoAta(null, kullanan, bilgi, detay);
        }

        public MUHBankaOdeme BankaListesiOku(Kullanici kullanan, MUHBankaOdeme bilgi)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.Oku(null, kullanan, bilgi);
        }

        public ObjectArray BankaListesiListele(Kullanici kullanan, int saymanlikKod, string birim, TNSDateTime basla, TNSDateTime bitis, string araMetin)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.Listele(null, kullanan, saymanlikKod, birim, basla, bitis, araMetin);
        }

        //public Sonuc BankaListesiDetayDurumGuncelle(Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay detay)
        //{
        //    TNS.KYM.Arac.KulturDegistir(kullanan);
        //    return BankaOdemeListe.DetayDurumGuncelle(null, kullanan, bilgi, detay);
        //}

        public ObjectArray BankaListesiOdenecekEmirListele(Kullanici kullanan, int yil, int saymanlikKod, string birim, TNSDateTime yevBasTar, TNSDateTime yevBitTar)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.OdenecekEmirListele(null, kullanan, yil, saymanlikKod, birim, yevBasTar, yevBitTar, null);
        }

        public Sonuc BankaListesindenOESil(Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay detay)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.OESil(null, kullanan, bilgi);
        }

        public Sonuc BankaListesiOnayla(Kullanici kullanan, int saymanlik, string birim, TNSDateTime gun, int siraNo, ENUMBankaListesiDurum onayKod, int donemNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.Onayla(null, kullanan, saymanlik, birim, gun, siraNo, onayKod, donemNo);
        }

        public Sonuc BankaListesiDetayOdendi(Kullanici kullanan, int saymanlik, string birim, TNSDateTime gun, int siraNo, int sira, string dekontNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.DetayOdendi(null, kullanan, saymanlik, birim, gun, siraNo, sira, dekontNo);
        }

        public Sonuc BankaListesiDetayOdendiToplu(Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay[] mbod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.DetayOdendiToplu(null, kullanan, bilgi, mbod);
        }

        public int BankaDosyaSiraNoBul(Kullanici kullanan, string onAd)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.BankaDosyaSiraNoBul(null, kullanan, onAd);
        }

        public Sonuc BankaListesiBankayaGonderildi(Kullanici kullanan, MUHBankaOdeme mbO, string sonucDosyaAd)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.BankayaGonderildi(null, kullanan, mbO, sonucDosyaAd);
        }

        //public Sonuc BankaListesiCekNoYaz(Kullanici kullanan, MUHBankaOdeme bilgi)
        //{
        //    TNS.KYM.Arac.KulturDegistir(kullanan);
        //    return BankaOdemeListe.CekNoYaz(null, kullanan, bilgi);
        //}

        public Sonuc BankaListesiCekNoOEEkle(Kullanici kullanan, MUHBankaOdeme bilgi, string cekNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.CekNoYaz(null, kullanan, bilgi, cekNo);
        }

        public IlgiliBilgi IlgiliBilgiBul(Kullanici kullanan, string tc_Vno, int saymanlikKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.IlgiliBilgiBul(null, kullanan, tc_Vno, saymanlikKod);
        }

        public Sonuc BankaAkibetDosyasiIsle(Kullanici kullanan, TNSDateTime islenecekGun)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BankaOdemeListe.BankaAkibetDosyasiIsle(null, kullanan, islenecekGun);
        }

        public YuzdeOnBilgi YuzdeOnBilgiBul(Kullanici kullanan, int yil, ENUMYuzdeOnTur MalHizmetYapim, int saymanlik, string birim, string haricOdemeEmriNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return MUHOdemeEmriMIFServis.YuzdeOnBilgiBul(null, kullanan, yil, MalHizmetYapim, saymanlik, birim, haricOdemeEmriNo);
        }

        public YuzdeOnBilgi YuzdeOnUstSinirBul(Kullanici kullanan, int yil, YuzdeOnBilgi yob)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return MUHOdemeEmriMIFServis.YuzdeOnUstSinirBul(null, kullanan, yil, yob);
        }

        public OdemeEmriMIFForm[] OdemeEmriMIFSurecBelgeNoListele(Kullanici kullanan, int surecYil, string surecNo, int mifYil, string mifBelgeNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return MUHOdemeEmriMIFServis.SurecBelgeNoListele(null, kullanan, surecYil, surecNo, mifYil, mifBelgeNo);
        }

        public Sonuc OdemeEmriMIFSurecBagKur(Kullanici kullanan, int yil, string belgeNo, int surecYil, string surecNo, int muhasebeKod, string birimKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return MUHOdemeEmriMIFServis.OdemeEmriMIFSurecBagKur(null, kullanan, yil, belgeNo, surecYil, surecNo, muhasebeKod, birimKod);
        }

        public Sonuc BirimHarcamaSayOE(Kullanici kullanan, int yil, int ay, int saymanlikKod, string birimKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return MUHOdemeEmriMIFServis.BirimHarcamaSayOE(null, kullanan, yil, ay, saymanlikKod, birimKod);
        }

        public ObjectArray IstisnaRaporlari(Kullanici kullanan, SGBKBSFark sgbKbsFark)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            if (sgbKbsFark.mode == 2)
                return MUHOdemeEmriMIFServis.OdenekUstuListele(null, kullanan, sgbKbsFark);
            else
                return MUHOdemeEmriMIFServis.SGBKBSFarkListele(null, kullanan, sgbKbsFark);

        }

        public Sonuc OdemeEmriEvrakNoKaydet(Kullanici kullanan, int yil, string evrakNo, string odemeEmriNo, int durum)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().OdemeEmriEvrakNoKaydet(kullanan, yil, evrakNo, odemeEmriNo, durum);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc OdemeEmriEvrakNoUret(Kullanici kullanan, int yil, string belgeNo)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().OdemeEmriEvrakNoUret(null, kullanan, yil, belgeNo);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public string OdemeEmriEvrakNoOku(int yil, string belgeNo)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().OdemeEmriEvrakNoOku(yil, belgeNo);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public ObjectArray YuzdeOnListele(Kullanici kullanan, int yil)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return new MUHOdemeEmriMIFServis().YuzdeOnListele(null, kullanan, yil);
        }

        public bool MuhasebeBirimYetkiVarMi(Kullanici kullanan, string muhasebe, string birim, TNS.MUH.ENUMOGY yetkiTur)
        {
            bool yetkiVar = TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)yetkiTur, (int)TNS.MUH.ENUMBelgeTuru.ODEMEEMRI);
            if (!yetkiVar) return false;

            return TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, muhasebe, birim, yetkiTur == ENUMOGY.Gorme);
        }

        public AvansHareket[] AvansSorgula(Kullanici kullanan, AvansSorguKriter form)
        {
            return MUHMizanBilanco.AvansSorgula(null, kullanan, form);
        }

        public EksikOdenekBilgi TertipDurumBul(Kullanici kullanan, int yil, int muhasebe, string birim, string tertip, string odemeEmriBelgeNo, int surecYil, string surecNo)
        {
            TNS.Veri.VeriErisim ve = TNS.Veri.VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            EksikOdenekBilgi eob = (new MUHOdemeEmriMIFServis()).TertipDurumBul(ve, kullanan, yil, muhasebe, birim, tertip, odemeEmriBelgeNo, surecYil, surecNo, true);
            ve.VeriErisimKapat();
            return eob;
        }

        public GondermeTenkisDetay SatirDurumBul(Kullanici kullanan, int form_yil, string muhasebe, string birim, ENUMBelgeTuru belgeTur, GondermeTenkisDetay sd, bool planlananOdenek, bool talep)
        {
            return (new MUHGondermeTenkisServis()).SatirDurumBul(kullanan, form_yil, muhasebe, birim, belgeTur, sd, planlananOdenek, talep);
        }

        public BagliEkoPeb[] ListeleBagliEkoPebAna(Kullanici kullanan, BagliEkoPeb form)
        {
            return MUHBagliEkoPebAna.Listele(null, kullanan, form);
        }

        /// <summary>
        /// BagliEkoPeb nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BagliEkoPeb yapısında BagliEkoPeb:  bilgilerini tutan değişken</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc KaydetBagliEkoPebAna(Kullanici kullanan, BagliEkoPeb form)
        {
            return MUHBagliEkoPebAna.Kaydet(null, kullanan, form);
        }

        /// <summary>
        /// Sils the bagli eko peb ana.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc SilBagliEkoPebAna(Kullanici kullanan, BagliEkoPeb form)
        {
            return MUHBagliEkoPebAna.Sil(null, kullanan, form);
        }

        /// <summary>
        /// Saymanliks the birim kurkod bul.
        /// </summary>
        /// <param name="saymanlik">The saymanlik.</param>
        /// <param name="birim">The birim.</param>
        /// <returns></returns>
        public string SaymanlikBirimKurkodBul(int saymanlik, string birim)
        {
            try
            {
                return new MUHMuhasebeBirimServis().SaymanlikBirimKurkodBul(saymanlik, birim);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Avanses the belgeleri listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">The kriter.</param>
        /// <param name="mahsuplarDahilVer">if set to <c>true</c> [mahsuplar dahil ver].</param>
        /// <returns></returns>
        public ObjectArray AvansBelgeleriListele(Kullanici kullanan, OdemeEmriMIFForm kriter, bool mahsuplarDahilVer)
        {
            return MUHOdemeEmriMIFServis.AvansBelgeleriListele(null, kullanan, kriter, mahsuplarDahilVer);
        }

        /// <summary>
        /// Mahsups the edilecek belge listele.
        /// </summary>
        /// <param name="oeForm">The oe form.</param>
        /// <param name="mahsupAlaniBosOlanlar">if set to <c>true</c> [mahsup alani bos olanlar].</param>
        /// <returns></returns>
        public ObjectArray MahsupEdilecekBelgeListele(OdemeEmriMIFForm oeForm, bool mahsupAlaniBosOlanlar)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().MahsupEdilecekBelgeListele(oeForm, mahsupAlaniBosOlanlar);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// GondermeTenkisIcmal listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">GondermeTenkisIcmal yapısında GondermeTenkisIcmal:  bilgilerini tutan değişken</param>
        /// <returns></returns>
        public ObjectArray ListeleOdenekGondermeIcmal(Kullanici kullanan, GondermeTenkisIcmal form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return MUHOdenekGondermeIcmal.Listele(null, kullanan, form);
        }

        /// <summary>
        /// GondermeTenkisIcmal nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">GondermeTenkisIcmal yapısında GondermeTenkisIcmal:  bilgilerini tutan değişken</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc KaydetOdenekGondermeIcmal(Kullanici kullanan, GondermeTenkisIcmal form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return MUHOdenekGondermeIcmal.Kaydet(null, kullanan, form, false);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }


        /// <summary>
        /// GondermeTenkisIcmal nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="formlar">GondermeTenkisIcmal yapısında GondermeTenkisIcmal:  bilgilerini tutan değişken</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc DurumAtaOdenekGondermeIcmal(Kullanici kullanan, TNSCollection formlar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return MUHOdenekGondermeIcmal.DurumAta(null, kullanan, formlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Tertiplers the uygun mu.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tertipler">The tertipler.</param>
        /// <returns></returns>
        public Sonuc TertiplerUygunMu(Kullanici kullanan, TNSCollection tertipler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return MUHBagliEkoPebAna.TertiplerUygunMu(null, kullanan, tertipler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Alts the hesap degistir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="sirano">The sirano.</param>
        /// <param name="altHesapKod">The alt hesap kod.</param>
        /// <returns></returns>
        public Sonuc AltHesapDegistir(Kullanici kullanan, int yil, string belgeNo, int sirano, string altHesapKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TNS.MUH.IsKurallari.MUHOdemeEmriMIFServis.AltHesapDegistir(null, kullanan, yil, belgeNo, sirano, altHesapKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ManasGunlukTakipGetir(Kullanici kullanan, ManasGunlukTakip kriter)
        {
            try
            {
                return MUHOdemeEmriMIFServis.ManasGunlukTakipGetir(kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Taahhut kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="taahhutListesi">Taahhüt Listesi</param>
        /// <returns></returns>
        public Sonuc TaahhutKartiKaydet(Kullanici kullanan, TaahhutKartiBilgi taahhutListesi, TNSCollection taahhutBilgiler)
        {
            try
            {
                return MUHTaahhutKarti.Kaydet(kullanan, taahhutListesi, taahhutBilgiler);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Taahhuts the karti odeme emri kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kartYil">The kart yil.</param>
        /// <param name="kartKod">The kart kod.</param>
        /// <param name="odemeEmriYil">The odeme emri yil.</param>
        /// <param name="odemeEmriNo">The odeme emri no.</param>
        /// <returns></returns>
        public Sonuc TaahhutKartiOdemeEmriKaydet(Kullanici kullanan, int kartYil, int kartKod, int odemeEmriYil, string odemeEmriNo)
        {
            try
            {
                return MUHTaahhutKarti.KaydetOdemeEmri(kullanan, kartYil, kartKod, odemeEmriYil, odemeEmriNo);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc TaahhutKartiOdemeEmriUret(Kullanici kullanan, TaahhutKartiBilgi bilgi)
        {
            try
            {
                return MUHTaahhutKarti.OdemeEmriUret(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc TaahhutKartiOdemeEmriUret(Kullanici kullanan, int sozKod, string degerArtisiKod)
        {
            try
            {
                return MUHTaahhutKarti.OdemeEmriUretDegerArtisi(null, kullanan, sozKod, degerArtisiKod);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Taahhuts the karti deger artisi odeme emri kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kod">The kod.</param>
        /// <param name="taahhukKod">The taahhuk kod.</param>
        /// <param name="odemeEmriYil">The odeme emri yil.</param>
        /// <param name="odemeEmriNo">The odeme emri no.</param>
        /// <returns></returns>
        public Sonuc TaahhutKartiDegerArtisiOdemeEmriKaydet(Kullanici kullanan, string kod, int taahhukKod, int odemeEmriYil, string odemeEmriNo)
        {
            try
            {
                return MUHTaahhutKarti.KaydetDegerArtisiOdemeEmri(kullanan, kod, taahhukKod, odemeEmriYil, odemeEmriNo);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc TaahhutKartiDegerArtisiKaydet(Kullanici kullanan, TaahhutKartiDegerArtisi da, TNSCollection taahhutBilgiler)
        {
            try
            {
                return MUHTaahhutKarti.KaydetDegerArtisiTek(null, kullanan, da, taahhutBilgiler);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectVAN TaahhutKartiDegerArtisiAcilabilirMi(Kullanici kullanan, TaahhutKartiDegerArtisi da)
        {
            try
            {
                return MUHTaahhutKarti.DegerArtisiAcilabilirMi(null, kullanan, da);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// Taahhuts the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="taahhut">The taahhut.</param>
        /// <returns></returns>
        public ObjectArray TaahhutKartiListele(Kullanici kullanan, TaahhutKartiBilgi taahhut, int personel1Nrml0Yok2Hepsi)
        {
            try
            {
                return MUHTaahhutKarti.Listele(null, kullanan, taahhut, personel1Nrml0Yok2Hepsi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Taahhüt the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public Sonuc TaahhutKartiSil(Kullanici kullanan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTaahhutKarti().Sil(kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Taahhüt the kapat.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public Sonuc TaahhutKartiKapat(Kullanici kullanan, TNSCollection kodlar, int durum)
        {
            try
            {
                return MUHTaahhutKarti.Kapat(kullanan, kodlar, durum);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// KBSs the aktarim listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public ObjectArray KBSAktarimListele(Kullanici kullanan, int yil)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().KBSAktarimListele(kullanan, yil);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// KBSs the aktarim kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">The liste.</param>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public Sonuc KBSAktarimKaydet(Kullanici kullanan, ObjectArray liste, int yil)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().KBSAktarimKaydet(kullanan, liste, yil);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// KBSs the aktarim detay kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">The liste.</param>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public Sonuc KBSAktarimDetayKaydet(Kullanici kullanan, ObjectArray liste, int yil)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().KBSAktarimDetayKaydet(kullanan, liste, yil);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// KBSs the aktarim detay listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public ObjectArray KBSAktarimDetayListele(Kullanici kullanan, int yil)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHOdemeEmriMIFServis().KBSAktarimDetayListele(kullanan, yil);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Taahhut kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="taahhutListesi">Taahhüt Listesi</param>
        /// <returns></returns>
        public Sonuc TaahhutKaydet(Kullanici kullanan, int taahhutKod, string degerArtisiKod, TNSCollection taahhutListesi)
        {
            try
            {
                return MUHTaahhut.Kaydet(kullanan, taahhutKod, degerArtisiKod, taahhutListesi);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }

        }

        /// <summary>
        /// Taahhuts the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="taahhut">The taahhut.</param>
        /// <returns></returns>
        public ObjectArray TaahhutListele(Kullanici kullanan, TaahhutBilgi taahhut)
        {
            try
            {
                return MUHTaahhut.Listele(null, kullanan, taahhut);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Taahhuts the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="taahhut">The taahhut.</param>
        /// <returns></returns>
        public ObjectArray TaahhutToplamListele(Kullanici kullanan, int taahhutKod)
        {
            try
            {
                return MUHTaahhut.TaahhutToplamListele(null, kullanan, taahhutKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Taahhüt the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public Sonuc TaahhutSil(Kullanici kullanan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTaahhut().Sil(kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// AlinanTeminat kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="teminatListesi">The teminat listesi.</param>
        /// <returns></returns>
        public Sonuc TaahhutTeminatlarKaydet(Kullanici kullanan, ObjectArray teminatListesi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTaahhutTeminatlar().Kaydet(kullanan, teminatListesi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// AlinanTeminats the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="teminat">The teminat.</param>
        /// <returns></returns>
        public ObjectArray TaahhutTeminatlarListele(Kullanici kullanan, TaahhutTeminatlar teminat)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHTaahhutTeminatlar().Listele(kullanan, teminat);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// AlinanTeminat the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="teminatListesi">The teminat listesi.</param>
        /// <returns></returns>
        public Sonuc TaahhutTeminatlarSil(Kullanici kullanan, ObjectArray teminatListesi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTaahhutTeminatlar().Sil(kullanan, teminatListesi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// VerilenAnavsVeMahsuplar kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">The liste.</param>
        /// <returns></returns>
        public Sonuc TaahhutAvanslarKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTaahhutAvanslar().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// VerilenAnavsVeMahsuplar the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public ObjectArray TaahhutAvanslarListele(Kullanici kullanan, TaahhutAvanslar form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHTaahhutAvanslar().Listele(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// MUHVerilenAvanslarVeMahsuplar the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="avansListesi">The avans listesi.</param>
        /// <returns></returns>
        public Sonuc TaahhutAvanslarSil(Kullanici kullanan, ObjectArray avansListesi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTaahhutAvanslar().Sil(kullanan, avansListesi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// BorclarVeTemlikler kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc TaahhutBorclarTanimKaydet(Kullanici kullanan, TaahhutBorclarTanim form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTaahhutBorclarTanim().Kaydet(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// BorclarVeTemlikler the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="borclar">The borclar.</param>
        /// <returns></returns>
        public ObjectArray TaahhutBorclarTanimListele(Kullanici kullanan, TaahhutBorclarTanim borclar)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHTaahhutBorclarTanim().Listele(kullanan, borclar);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// BorclarVeTemlikler the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="borcListe">The borc liste.</param>
        /// <returns></returns>
        public Sonuc TaahhutBorclarTanimSil(Kullanici kullanan, ObjectArray borcListe)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTaahhutBorclarTanim().Sil(kullanan, borcListe);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Ödeme emri kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="odemeEmriListesi">Odeme Emri Listesi</param>
        /// <returns></returns>
        public Sonuc TaahhutOdemeEmriKaydet(Kullanici kullanan, ObjectArray odemeEmriListesi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTaahhutOdemeEmri().Kaydet(kullanan, odemeEmriListesi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Odeme Emri  the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public ObjectArray TaahhutOdemeEmriListele(Kullanici kullanan, TaahhutOdeme form)
        {
            try
            {
                return MUHTaahhutOdemeEmri.Listele(kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        public double TaahhutOdemeEmriHazirlananToplamTutar(Kullanici kullanan, int taahhutKod)
        {
            return new MUHTaahhutOdemeEmri().HazirlananOdemeEmriTutari(kullanan, taahhutKod);
        }

        public double TaahhutOdemeEmriHazirlananKapatmaFarki(Kullanici kullanan, int taahhutKod)
        {
            return new MUHTaahhutOdemeEmri().HazirlananOdemeEmriTutariKapatmaFarki(kullanan, taahhutKod);
        }

        /// <summary>
        /// OdemeEmri the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public Sonuc TaahhutOdemeEmriSil(Kullanici kullanan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHTaahhutOdemeEmri().Sil(kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// mutemet kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc MutemetTanimKaydet(Kullanici kullanan, MutemetTanim form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHMutemetTanim().Kaydet(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// mutemet the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public ObjectArray MutemetTanimListele(Kullanici kullanan, MutemetTanim form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHMutemetTanim().Listele(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// mutemet the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="mutemetListesi">The mutemet listesi.</param>
        /// <returns></returns>
        public Sonuc MutemetTanimSil(Kullanici kullanan, ObjectArray mutemetListesi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHMutemetTanim().Sil(kullanan, mutemetListesi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc MutemetAktar(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHMutemetTanim().Aktar(kullanan, kaynakYil, hedefYil);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// hesapPlaniKisit kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc HesapPlaniKisitKaydet(Kullanici kullanan, HesapPlaniKisit form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHHesapPlaniKisit().Kaydet(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// HesapPlaniKisit the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public ObjectArray HesapPlaniKisitListele(Kullanici kullanan, HesapPlaniKisit form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHHesapPlaniKisit().Listele(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// HesapPlaniKisit the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">The liste.</param>
        /// <returns></returns>
        public Sonuc HesapPlaniKisitSil(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHHesapPlaniKisit().Sil(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc HesapPlaniKisitAktar(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHHesapPlaniKisit().Aktar(kullanan, kaynakYil, hedefYil);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }


        /// <summary>
        /// IstatistikKod nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <param name="sifirEkle">if set to <c>true</c> [sifir ekle].</param>
        /// <param name="eksibirEkle">if set to <c>true</c> [eksibir ekle].</param>
        /// <returns></returns>
        public ObjectArray ListeleIstatistikTanim(Kullanici kullanan, IstatistikKod form, bool sifirEkle, bool eksibirEkle, bool detaylarOlsun)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return MUHIstatistikKodGiris.Listele(null, kullanan, form, sifirEkle, eksibirEkle, detaylarOlsun);
        }

        /// <summary>
        /// IstatistikKod nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        public IstatistikKod OkuIstatistikKodGiris(Kullanici kullanan, IstatistikKod form, bool sifirEkle, bool eksibirEkle)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return MUHIstatistikKodGiris.Oku(null, kullanan, form, sifirEkle, eksibirEkle);
        }


        /// <summary>
        /// IstatistikKod nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc KaydetIstatistikKodGiris(Kullanici kullanan, IstatistikKod form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return MUHIstatistikKodGiris.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// IstatistikKod nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc KaydetIstatistikKodTanim(Kullanici kullanan, IstatistikKod form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return MUHIstatistikKodGiris.TanimKaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Belges the istatistik kod ata odemif.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="fler">The fler.</param>
        /// <param name="ist1">The ist1.</param>
        /// <param name="ist2">The ist2.</param>
        /// <returns></returns>
        public Sonuc BelgeIstatistikKodAtaODEMIF(Kullanici kullanan, TNSCollection fler, int ist1, int ist2)
        {
            return MUHIstatistikKodGiris.ODEMIFIstKodAta(null, kullanan, fler, ist1, ist2);
        }

        /// <summary>
        /// BorclarVeTemlikler kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="borclar">The borclar.</param>
        /// <returns></returns>
        public Sonuc TaahhutBorclarKaydet(Kullanici kullanan, ObjectArray borclar)
        {
            try
            {
                return MUHTaahhutBorclar.Kaydet(kullanan, borclar);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// BorclarVeTemlikler the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="borclar">The borclar.</param>
        /// <returns></returns>
        public ObjectArray TaahhutBorclarListele(Kullanici kullanan, TaahhutBorclar borclar)
        {
            try
            {
                return MUHTaahhutBorclar.Listele(kullanan, borclar);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// BorclarVeTemlikler the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="borcListe">The borc liste.</param>
        /// <returns></returns>
        public Sonuc TaahhutBorclarSil(Kullanici kullanan, ObjectArray borcListe)
        {
            try
            {
                return MUHTaahhutBorclar.Sil(kullanan, borcListe);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Damga vergisi aktarım tipi kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="aktarimTipleri">aktarim Tipleri</param>
        /// <returns></returns>
        public Sonuc DamgaVergisiAktarimTipiKaydet(Kullanici kullanan, ObjectArray aktarimTipleri)
        {
            try
            {
                return MUHVergiDairesiAktarimTurTanimServis.Kaydet(kullanan, aktarimTipleri);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }

        }

        /// <summary>
        /// Damga vergisi aktarım tipi sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="aktarimTipleri">aktarim Tipleri</param>
        /// <returns></returns>
        public Sonuc DamgaVergisiAktarimTipiSil(Kullanici kullanan, ObjectArray aktarimTipleri)
        {
            try
            {
                return MUHVergiDairesiAktarimTurTanimServis.Sil(kullanan, aktarimTipleri);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Damga vergisi aktarım tipi listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="aktarimTipi">aktarim tipi</param>
        /// <returns></returns>
        public ObjectArray DamgaVergisiAktarimTipiListele(Kullanici kullanan, VergiDairesiAktarimTurTanim aktarimTipi)
        {
            try
            {
                return MUHVergiDairesiAktarimTurTanimServis.Listele(kullanan, aktarimTipi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Mahsups the yapilmamis belge bul.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="hesapKod">The hesap kod.</param>
        /// <param name="altHesapKod">The alt hesap kod.</param>
        /// <param name="borclu">if set to <c>true</c> [borclu].</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="miktar">The miktar.</param>
        /// <returns></returns>
        public Sonuc MahsupYapilmamisBelgeBul(Kullanici kullanan, int yil, string hesapKod, string altHesapKod, bool borclu, string belgeNo, double miktar)
        {
            try
            {
                return MUHOdemeEmriMIFServis.MahsupYapilmamisBelgeBul(null, kullanan, yil, hesapKod, altHesapKod, borclu, belgeNo, miktar);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectVAN MahsupSatiriBul(Kullanici kullanan, int yil, string mahsupBelgeNo, string hesapKod, string altHesapKod, bool borclu, double miktar)
        {
            try
            {
                return MUHOdemeEmriMIFServis.MahsupSatiriBul(null, kullanan, yil, mahsupBelgeNo, hesapKod, altHesapKod, borclu, miktar);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// MuhasebeBirim nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MuhasebeBirim yapısında MuhasebeBirim:  bilgilerini tutan değişken</param>
        /// <returns></returns>
        public ObjectArray KapaliBirimListele(Kullanici kullanan, MuhasebeBirim form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return MUHMuhasebeBirimServis.KapaliBirimListele(null, kullanan, form);
        }

        /// <summary>
        /// MuhasebeBirim nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MuhasebeBirim yapısında MuhasebeBirim:  bilgilerini tutan değişken</param>
        /// <param name="kapat">if set to <c>true</c> [kapat].</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc BirimAcKapat(Kullanici kullanan, MuhasebeBirim form, bool kapat)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return MUHMuhasebeBirimServis.BirimAcKapat(null, kullanan, form, kapat);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Taahhuts the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public ObjectArray TaahhutListele(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            try
            {
                return MUHMizanBilanco.TaahhutListele(kullanan, form);
            }
            catch (Exception ex)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(ex.Message);
                return oa;
            }
        }

        /// <summary>
        /// Faturas the kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc FaturaKaydet(Kullanici kullanan, Fatura bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHFatura.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Faturas the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray FaturaListele(Kullanici kullanan, Fatura bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = MUHFatura.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Faturas the durum guncelle.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc FaturaDurumGuncelle(Kullanici kullanan, Fatura bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHFatura.DurumGuncelle(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Faturas the mif belge no guncelle.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc FaturaMIFBelgeNoGuncelle(Kullanici kullanan, Fatura bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHFatura.MIFBelgeNoGuncelle(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc FaturaMuhasebeAciklamaKaydet(Kullanici kullanan, Fatura bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHFatura.KaydetMuhasebeAciklama(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// Faturas the aciklama kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc FaturaAciklamaKaydet(Kullanici kullanan, FaturaAciklama bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHFatura.FaturaAciklamaKaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Faturas the aciklama listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray FaturaAciklamaListele(Kullanici kullanan, FaturaAciklama bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = MUHFatura.FaturaAciklamaListele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Faturas the aciklama sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc FaturaAciklamaSil(Kullanici kullanan, FaturaAciklama bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHFatura.FaturaAciklamaSil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Plakas the kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc PlakaKaydet(Kullanici kullanan, Plaka bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHPlaka.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Plakas the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray PlakaListele(Kullanici kullanan, Plaka bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = MUHPlaka.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Plakas the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc PlakaSil(Kullanici kullanan, Plaka bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHPlaka.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Mudurluks the kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc MudurlukKaydet(Kullanici kullanan, Mudurluk bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHMudurluk.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Mudurluks the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray MudurlukListele(Kullanici kullanan, Mudurluk bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = MUHMudurluk.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Mudurluks the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc MudurlukSil(Kullanici kullanan, Mudurluk bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHMudurluk.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc AciklamaKaydet(Kullanici kullanan, AciklamaBilgi bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHAciklama.Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray AciklamaListele(Kullanici kullanan, AciklamaBilgi bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = MUHAciklama.Listele(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc AciklamaSil(Kullanici kullanan, AciklamaBilgi bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = MUHAciklama.Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// Odeneks the gonderme sorgu.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="durumAralik">The durum aralik.</param>
        /// <param name="belgeTur">The belge tur.</param>
        /// <param name="faaliyetli">if set to <c>true</c> [faaliyetli].</param>
        /// <param name="kur">The kur.</param>
        /// <param name="fon">The fon.</param>
        /// <param name="fin">The fin.</param>
        /// <param name="eko">The eko.</param>
        /// <returns></returns>
        public string OdenekGondermeSorgu(int yil, int muhasebe, string birim, int durumAralik, ENUMBelgeTuru belgeTur, bool faaliyetli, string kur, string fon, string fin, string eko)
        {
            if (belgeTur == ENUMBelgeTuru.ODENEKGONDERME || belgeTur == ENUMBelgeTuru.TENKIS)
                return TNS.MUH.IsKurallari.Veri.TABGondermeTenkis.OdenekGondermeSorgu(yil, muhasebe, birim, durumAralik, belgeTur, faaliyetli, kur, fon, fin, eko);
            else if (durumAralik == 6)
                return TNS.MUH.IsKurallari.Veri.TABMuhasebat.HarcamaSorgu(yil, muhasebe, birim, durumAralik, belgeTur, faaliyetli, kur, fon, fin, eko);
            else
                return TNS.MUH.IsKurallari.Veri.TABOdemeEmriMIF.HarcamaSorgu(yil, muhasebe, birim, durumAralik, belgeTur, faaliyetli, kur, fon, fin, eko);
        }

        public bool BirimKapaliMi(Kullanici kullanan, int saymanlik, string birimKod, string kurKod)
        {
            return MUHMuhasebeBirimServis.BirimKapaliMi(null, kullanan, saymanlik, birimKod, kurKod);
        }

        public Sonuc ImzaTanimKaydet(Kullanici kullanan, ImzaTanim form)
        {
            try
            {
                return MUHImzaTanimServis.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public Sonuc ImzaTanimSil(Kullanici kullanan, ImzaTanim form)
        {
            try
            {
                return MUHImzaTanimServis.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public ImzaTanim[] ImzaTanimListele(Kullanici kullanan, ImzaTanim form)
        {
            try
            {
                return MUHImzaTanimServis.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ImzaTanim ImzaTanimOku(Kullanici kullanan, ImzaTanim form)
        {
            try
            {
                return MUHImzaTanimServis.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc ImzaTanimGecerliImzaYap(Kullanici kullanan, int imzaTanimKod, string birimKod)
        {
            try
            {
                return MUHImzaTanimServis.GecerliImzaYap(null, kullanan, imzaTanimKod, birimKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// CekKocani nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CekKocani yapısında CekKocani: Çek Koçanındaki Sayfalar bilgilerini tutan değişken</param>
        public ObjectArray ListeleCekKocani(Kullanici kullanan, CekKocani form, bool acik, bool bozdurulmus, bool iptalli, TNSDateTime baslaTarih, TNSDateTime bitisTarih)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return MUHCekKocani.Listele(null, kullanan, form, acik, bozdurulmus, iptalli, baslaTarih, bitisTarih);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// CekKocani nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CekKocani yapısında CekKocani: Çek Koçanındaki Sayfalar bilgilerini tutan değişken</param>
        //public ObjectVAN OkuCekKocani(Kullanici kullanan, CekKocani form)
        //{
        //    try
        //    {
        //        TNS.KYM.Arac.KulturDegistir(kullanan);
        //        return MUHCekKocani.Oku(null, kullanan, form);
        //    }
        //    catch (Exception e)
        //    {
        //        ObjectVAN ov = new ObjectVAN();
        //        ov.sonuc.DegerAta(e.ToString());
        //        return ov;
        //    }
        //}

        /// <summary>
        /// CekKocani nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CekKocani yapısında CekKocani: Çek Koçanındaki Sayfalar bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetCekKocani(Kullanici kullanan, CekKocani form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return MUHCekKocani.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// CekKocani nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CekKocani yapısında CekKocani: Çek Koçanındaki Sayfalar bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SilCekKocani(Kullanici kullanan, CekKocani form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return MUHCekKocani.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SayistayVeriKaydet(Kullanici kullanan, SayistayVeri form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHSayistayVeri().Kaydet(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray SayistayVeriListele(Kullanici kullanan, SayistayVeri form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new MUHSayistayVeri().Listele(kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public Sonuc SayistayVeriSil(Kullanici kullanan, SayistayVeri veri)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new MUHSayistayVeri().Sil(kullanan, veri);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray KapatmaListele(Kullanici kullanan, int yil, string belgeNo)
        {
            try
            {
                return new MUHOdemeEmriMIFServis().KapatmaListele(kullanan, yil, belgeNo);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        public Sonuc MuhasebatHarcamaKaydet(Kullanici kullanan, int yil, int ay, int muhasebeKod, string birimKod, string vergiNo, HesapBilgi[] tolar, bool prg)
        {
            return TNS.MUH.IsKurallari.MUHProgramButceIslemleri.MuhasebatHarcamaKaydet(kullanan, yil, ay, muhasebeKod, birimKod, vergiNo, tolar, prg);
        }

        public Sonuc IsKaydet(Kullanici kullanan, IsTanim form)
        {
            try
            {
                return MUHIs.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public Sonuc IsSil(Kullanici kullanan, IsTanim form)
        {
            try
            {
                return MUHIs.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray IsListele(Kullanici kullanan, IsTanim form)
        {
            try
            {
                return MUHIs.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }

        public Sonuc IsOdenekKaydet(Kullanici kullanan, IsOdenek[] liste)
        {
            try
            {
                return MUHIsOdenek.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public Sonuc IsOdenekSil(Kullanici kullanan, IsOdenek form)
        {
            try
            {
                return MUHIsOdenek.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray IsOdenekListele(Kullanici kullanan, IsOdenek form)
        {
            try
            {
                return MUHIsOdenek.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }

        public ObjectArray IsOdenekDurumListele(Kullanici kullanan, int yil, int muhasebe, string birim, int isKod)
        {
            try
            {
                return MUHIsOdenek.DurumListele(null, kullanan, yil, muhasebe, birim, isKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }

        public ObjectArray IsBlokeOdenekListele(Kullanici kullanan, IsTanim form)
        {
            try
            {
                return MUHIs.BlokeOdenekListele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }

        public Sonuc TaahhutKartiSozlesmeKaydet(Kullanici kullanan, TaahhutKartiSozlesme form)
        {
            try
            {
                return MUHTaahhutKartiSozlesme.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public Sonuc TaahhutKartiSozlesmeSil(Kullanici kullanan, TaahhutKartiSozlesme form)
        {
            try
            {
                return MUHTaahhutKartiSozlesme.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray TaahhutKartiSozlesmeListele(Kullanici kullanan, TaahhutKartiSozlesme form)
        {
            try
            {
                return MUHTaahhutKartiSozlesme.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }

        public Sonuc TaahhutKartiBakimTarihceKaydet(Kullanici kullanan, TaahhutKartiBakimTarihce form)
        {
            try
            {
                return MUHTaahhutKartiBakimTarihce.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public Sonuc TaahhutKartiBakimTarihceSil(Kullanici kullanan, TaahhutKartiBakimTarihce form)
        {
            try
            {
                return MUHTaahhutKartiBakimTarihce.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray TaahhutKartiBakimTarihceListele(Kullanici kullanan, TaahhutKartiBakimTarihce form)
        {
            try
            {
                return MUHTaahhutKartiBakimTarihce.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }

        public Sonuc OrtakBelgeKaydet(Kullanici islemiYapan, OrtakBelge form)
        {
            try
            {
                return MUHOratakBelge.Kaydet(islemiYapan, form);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public ObjectArray OrtakBelgeListele(Kullanici islemiYapan, OrtakBelge form)
        {

            try
            {
                return MUHOratakBelge.Listele(islemiYapan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public OrtakBelge OrtakBelgeOku(Kullanici islemiYapan, OrtakBelge form)
        {
            try
            {
                return MUHOratakBelge.Oku(islemiYapan, form);
            }
            catch (Exception e)
            {
                return new OrtakBelge();
            }

        }
        public Sonuc OrtakBelgeSil(Kullanici islemiYapan, OrtakBelge form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = MUHOratakBelge.Sil(islemiYapan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EntegrasyonMIFKaydet(Kullanici islemiYapan, OdemeEmriMIFForm bilgi)
        {
            try
            {
                return MUHEntegrasyonMIF.Kaydet(null, islemiYapan, bilgi);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public Sonuc EntegrasyonMIFGuncelleDurum(Kullanici islemiYapan, OdemeEmriMIFForm bilgi)
        {
            try
            {
                return MUHEntegrasyonMIF.GuncelleDurum(null, islemiYapan, bilgi);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public ObjectArray EntegrasyonMIFListele(Kullanici islemiYapan, OdemeEmriMIFForm bilgi, bool getirDetay)
        {

            try
            {
                return MUHEntegrasyonMIF.Listele(null, islemiYapan, bilgi, getirDetay);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public Sonuc EntegrasyonMIFSil(Kullanici islemiYapan, OdemeEmriMIFForm bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = MUHEntegrasyonMIF.Sil(null, islemiYapan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc CaprazTabloKaydet(Kullanici kullanan, CaprazTablo bilgi)
        {
            try
            {
                return MUHCaprazTablo.Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }

        }
        public Sonuc CaprazTabloSil(Kullanici kullanan, CaprazTablo bilgi)
        {
            try
            {
                return MUHCaprazTablo.Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }
        public ObjectArray CaprazTabloListele(Kullanici kullanan, CaprazTablo bilgi)
        {
            try
            {
                return MUHCaprazTablo.Listele(kullanan, bilgi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }
        public ObjectArray CaprazTabloRaporListele(Kullanici kullanan, CaprazTablo bilgi)
        {
            try
            {
                return MUHCaprazTablo.ListeleTablo(kullanan, bilgi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }
        public ObjectArray CaprazTabloRaporHesapKodListele(Kullanici kullanan, CaprazTablo bilgi)
        {
            try
            {
                return MUHCaprazTablo.ListeleHesapKodTablo(kullanan, bilgi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }
        public ObjectArray CaprazTabloRaporHesapKodListele2(Kullanici kullanan, CaprazTablo bilgi)
        {
            try
            {
                return MUHCaprazTablo.ListeleHesapKodTablo2(kullanan, bilgi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public Sonuc TalepTanimKaydet(Kullanici kullanan, TalepTanimForm bilgi)
        {
            try
            {
                return MUHTalepTanimForm.Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }

        }
        public Sonuc TalepTanimSil(Kullanici kullanan, TalepTanimForm bilgi)
        {
            try
            {
                return MUHTalepTanimForm.Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }
        public ObjectArray TalepTanimListele(Kullanici kullanan, TalepTanimForm bilgi)
        {
            try
            {
                return MUHTalepTanimForm.Listele(kullanan, bilgi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }
        public Sonuc TkdkTalepTertipKaydet(Kullanici kullanan, ObjectArray liste)
        {
            try
            {
                return MUHTKDKTalep.TertipKaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }

        }

        public Sonuc TkdkTalepKaydet(Kullanici kullanan, TkdkTalepForm bilgi)
        {
            try
            {
                return MUHTKDKTalep.Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }

        }
        public Sonuc TkdkTalepGuncelleDurum(Kullanici kullanan, ObjectArray bilgi)
        {
            try
            {
                return MUHTKDKTalep.GuncelleDurum(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }

        }
        public Sonuc TkdkTalepSil(Kullanici kullanan, TkdkTalepForm bilgi)
        {
            try
            {
                return MUHTKDKTalep.Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }
        public Sonuc TkdkTalepOdemeEmriGuncelle(Kullanici kullanan, TkdkTalepForm bilgi)
        {
            try
            {
                return MUHTKDKTalep.OdemeEmriNoGuncelle(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray TkdkTalepListele(Kullanici kullanan, TkdkTalepForm bilgi)
        {
            try
            {
                return MUHTKDKTalep.Listele(kullanan, bilgi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        //Burası hata veriyor TCCB TFS
        public Sonuc FaturaTakipKaydet(Kullanici kullanan, FaturaTakip form)
        {
            try
            {
                return MUHFaturaTakip.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray FaturaTakipListele(Kullanici kullanan, FaturaTakip form)
        {
            try
            {
                return MUHFaturaTakip.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public Sonuc BIBagKopart(Kullanici kullanan, int revizeYil, int revizeNo)
        {
            return MUHGondermeTenkisServis.BIBagKopart(kullanan, revizeYil, revizeNo);
        }

        public Sonuc BIDuzelt(Kullanici kullanan, BKD.MPRevizeForm f)
        {
            return MUHGondermeTenkisServis.BIDuzelt(kullanan, f);
        }

        public Sonuc MuhasebeBirimiTasi(Kullanici kullanan, int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod, TNSCollection islemListesi)
        {
            try
            {
                return MUHMuhasebeServis.MuhasebeBirimiTasi(null, kullanan, eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod, islemListesi);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc EbutceAktarimBilgiOku()
        {
            return MUHProgramButceIslemleri.EbutceAktarimBilgiOku();
        }
    }
}

namespace TNS
{
    /// <summary>
    /// Kullanıcı modülünde kullanılan yetki tipi listelerini döndüren yordamları tutan sınıf.
    /// </summary>
    public class KYMListe : YetkiTipiListesi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ListeSatir[] YetkiListesiOdemeEmriMIF()
        {
            ListeSatir[] lv = new ListeSatir[6];
            lv[0] = new ListeSatir("1", "Görme yetkisi var.");
            lv[1] = new ListeSatir("2", "Görme ve yazma yetkisi var.");
            lv[2] = new ListeSatir("3", "Görme ve onaylama yetkisi var.");
            lv[3] = new ListeSatir("4", "Görme, yazma ve onaylama yetkisi var.");
            lv[4] = new ListeSatir("5", "Belge onayı kaldırma.");
            lv[5] = new ListeSatir("6", "Yevmiye kaldırma.");

            return lv;
        }

        public static ListeSatir[] Veznedar()
        {
            ListeSatir[] lv = new ListeSatir[3];
            lv[0] = new ListeSatir("1", "Evet");
            lv[1] = new ListeSatir("0", "Hayır");
            lv[2] = new ListeSatir("100", "Makbuz hazırlayan Veznedar");

            return lv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim.Equals("6"))
                return YetkiListesiOdemeEmriMIF();
            if (ekParamTanim.Equals("7"))
                return Veznedar();
            throw new NotImplementedException("Bilinmeyen parametre!");
        }
    }
}
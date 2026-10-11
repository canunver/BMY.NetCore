using System;
using System.Collections;
using System.Text;
//using tr.com.ega.eimza.api;
using EVRServisI;
using TNS.DEG;
using TNS.EVR;
using TNS.EVR.IsKurallari;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak formu sunucu yordamlarının fabrika sınıfı. Gerçekleştirilen yordamlar, ilgili sınıfdaki işlem yapan yordamını çağırırlar.
    /// </summary>
    [Serializable()]
    public class EVRServis : MarshalByRefObject, IEVRServis, IDisposable
    {
        /// <summary>
        /// Kullanıcı sunucu yordamlarına erişimi sağlayan değişken.
        /// </summary>
        //public static IKYMServis servisKYM = null;

        static EVRServis()
        {
            //if (servisKYM == null)
            //{
            //servisKYM = TNS.KYM.Arac.Tanimla();
            //EVRDosyaPlani.DosyaPlaniKlasorBolumOlustur(null, null);
            //}
        }

        TNS.Veri.VeriErisim ve = null;
        public void Dispose()
        {
            if (ve != null)
                ve.VeriErisimKapat(false);
        }

        public bool YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu islem)
        {
            return EVREvrak.YapilabilirMi(islem);
        }
        public bool YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu islem, string birimKod)
        {
            return EVREvrak.YapilabilirMi(islem, birimKod);
        }


        /// <summary>
        /// OzelAlan bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">The form.</param>
        /// <returns>
        /// OzelAlan nesne koleksiyonunu olarak döndürülür.
        /// </returns>
        public ObjectArray KEPGonderiListele(Kullanici kullanan, KEPGonderi form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRKEPGonderi.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakOzelAlan nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">The form.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc KEPGonderiKaydet(Kullanici kullanan, KEPGonderi form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRKEPGonderi.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakOzelAlan nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">The form.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc KEPGonderiSMIMEKaydet(Kullanici kullanan, KEPGonderi form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRKEPGonderi.KaydetSMIME(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Keps the gonderi durum kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc KEPGonderiDurumKaydet(Kullanici kullanan, KEPGonderi form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRKEPGonderi.KaydetDurum(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakOzelAlan nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">EvrakOzelAlan nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakOzelAlanKaydet(Kullanici kullanan, ArrayList liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakOzelAlan.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// EvrakOzelAlan bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <param name="birimKod">Birim kod bilgisini içeren nesne </param>
        /// <param name="dosyaKodlari">EvrakOzelAlan nesne koleksiyonunu içeren nesne</param>
        /// <returns>EvrakOzelAlan nesne koleksiyonunu olarak döndürülür.</returns>
        public ArrayList EvrakOzelAlanListele(string prEvrakNo, string birimKod, ArrayList dosyaKodlari)
        {
            ArrayList donusDeger = new ArrayList();
            try
            {
                donusDeger = EVREvrakOzelAlan.Listele(null, prEvrakNo, birimKod, dosyaKodlari);
            }
            catch
            {
                //
            }
            return donusDeger;
        }

        /// <summary>
        /// OzelAlan bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">OzelAlan bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc OzelAlanKaydet(Kullanici kullanan, OzelAlan bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new EVROzelAlan().Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// OzelAlan bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">OzelAlan bilgisini içeren nesne</param>
        /// <returns>OzelAlan nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray OzelAlanListele(Kullanici kullanan, OzelAlan bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new EVROzelAlan().Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// OzelAlan bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">OzelAlan bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc OzelAlanSil(Kullanici kullanan, OzelAlan bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new EVROzelAlan().Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray DosyayaKaldirilmisEvrakListele(EvrakSinif bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVREvrak.ListeleDosyayaKaldirilmisEvraklar(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }
        /// <summary>
        /// Parametre olarak verilen evrak birimlerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">>Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak birimlerini içeren nesne koleksiyonu</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc BirimKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRBirim.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen evrak birimlerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak birimlerini içeren nesne koleksiyonu</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc BirimSil(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRBirim.Sil(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Birim nesnesine göre evrak birimlerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Birim bilgisini içeren nesne</param>
        /// <returns>Birim nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray BirimListele(Birim bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRBirim.Listele(null, bilgi, false);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Birim nesnesine göre evrak birimlerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Birim bilgisini içeren nesne</param>
        /// <returns>Birim nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray AnaBirimleriListele(Birim bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRBirim.Listele(null, bilgi, true);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Birim nesnesine göre evrak birimini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Birim bilgisini içeren nesne</param>
        /// <returns>Evrak birimi Birim nesnesi olarak döndürülür.</returns>
        public Birim BirimOku(Birim bilgi)
        {
            return EVRBirim.Oku(null, bilgi);
        }

        /// <summary>
        /// Parametre olarak verilen evrak birim kodunun evrak kök kodunu getiren yordam.
        /// </summary>
        /// <param name="evrakBirim">Evrek birim kodunu içeren nesne</param>
        /// <returns>Evrak kök kodu string nesesi olarak döndürülür.</returns>
        public string EvrakKokBirimGetir(string evrakBirim)
        {
            return EVRBirim.EvrakKokBirimGetir(null, evrakBirim);
        }
        public Birim EvrakKokBirimGetir2(string evrakBirim)
        {
            return EVRBirim.EvrakKokBirimGetir2(null, evrakBirim);
        }

        /// <summary>
        /// Parametre olarak verilen evrak birim kodunun evrak kurum kök kodunu getiren yordam.
        /// </summary>
        /// <param name="evrakBirim">Evrek birim kodunu içeren nesne</param>
        /// <returns>Evrak kurum kök kodu string nesesi olarak döndürülür.</returns>
        public string EvrakKurumKokBirimGetir(string evrakBirim)
        {
            string birim = EVRBirim.EvrakKokBirimGetir(null, evrakBirim);

            if (birim == evrakBirim)
            {
                string[] parcalar = birim.Split('.');

                birim = "";
                for (int i = 0; i < parcalar.Length - 1; i++)
                {
                    if (birim != "")
                        birim += ".";

                    birim += parcalar[i];
                }

                //Meclis de Kurum içi yazışmada birimlerin gelmesi için eklendi. 09.03.2012
                if (EVRBirim.EvrakKokBirimGetir(null, birim) == "")
                    birim = evrakBirim;
            }

            return birim;
        }

        /// <summary>
        /// Evrak birimlerinin seviye değerlerini düzenleyen yordam.
        /// </summary>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc BirimDetaySayilariDuzenle()
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            try
            {
                sonuc = EVRBirim.DetaySayilariDuzenle(null);
            }
            catch (Exception exc)
            {
                sonuc.hataStr = exc.Message;
                sonuc.islemSonuc = false;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <param name="aktarim">DosyaPlani nesne koleksiyonunun aktarım ile gelip gelmediğini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc DosyaPlaniKaydet(Kullanici kullanan, ObjectArray liste, bool aktarim, string seciliKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDosyaPlani.Kaydet(null, kullanan, liste, aktarim, seciliKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu içeren dosya planı bilgilerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc DosyaPlaniSil(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDosyaPlani.Sil(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesnesine göre, DosyaPlani nesne koleksiyonunu veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">DosyaPlani bilgisini içeren nesne</param>
        /// <returns>DosyaPlani nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray DosyaPlaniListele(DosyaPlani bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDosyaPlani.Listele(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesnesine göre, DosyaPlani nesne koleksiyonunu veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">DosyaPlani bilgisini içeren nesne</param>
        /// <returns>DosyaPlani nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray DosyaPlaniTarihceListele(DosyaPlaniTarihce bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDosyaPlani.ListeleTarihce(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray DosyaPlaniEvrakListele(EvrakSinif bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDosyaPlani.ListeleDosyaPlaniEvrak(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        //public ObjectArray DosyaPlaniEvrakSayisi(DosyaPlaniTarihce bilgi)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = EVRDosyaPlani.ListeleDosyaTarihceSayisi(null, bilgi);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        donusDeger.sonuc.islemSonuc = false;
        //    }

        //    return donusDeger;
        //}


        /// <summary>
        /// Dosya planlarının seviye değerlerini düzenleyen yordam.
        /// </summary>        
        public void DosyaPlaniDetaySayilariDuzenle()
        {
            EVRDosyaPlani.DetaySayilariDuzenle(null);
        }

        //public Sonuc GizlilikKaydet(Kullanici kullanan, ObjectArray liste)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = EVRGizlilik.Kaydet(kullanan, liste);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}
        //public Sonuc GizlilikSil(Kullanici kullanan, ObjectArray liste)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = EVRGizlilik.Sil(kullanan, liste);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}
        //public ObjectArray GizlilikListele(Tanimlama bilgi)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = EVRGizlilik.Listele(bilgi);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        donusDeger.sonuc.islemSonuc = false;
        //    }

        //    return donusDeger;
        //}

        /// <summary>
        /// Parametre olarak verilen DosyaPlaniBirim nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlaniBirim nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc DosyaPlaniBirimKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDosyaPlaniBirim.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlaniBirim nesnesine göre, DosyaPlaniBirim nesne koleksiyonunu veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">DosyaPlaniBirim bilgisini içeren nesne</param>
        /// <returns>DosyaPlaniBirim nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray DosyaPlaniBirimListele(Kullanici kullanan, DosyaPlaniBirim bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDosyaPlaniBirim.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc DosyaPlaniFavoriKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDosyaPlaniFavori.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Dosya planı favori bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ad">Dosya plan kodunun ad bilgisini içeren nesne</param>
        /// <param name="kod">Dosya plan kodunun kod bilgisini içeren nesne</param>
        /// <returns>DosyaPlani nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray DosyaPlaniFavoriListele(Kullanici kullanan, string ad, string kod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDosyaPlaniFavori.Listele(null, kullanan, ad, kod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu içeren dosya planı favori bilgilerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>        
        public Sonuc DosyaPlaniFavoriSil(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDosyaPlaniFavori.Sil(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        //public Sonuc YaziDurumuKaydet(Kullanici kullanan, ObjectArray liste)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = EVRYaziDurumu.Kaydet(kullanan, liste);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}
        //public Sonuc YaziDurumuSil(Kullanici kullanan, ObjectArray liste)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = EVRYaziDurumu.Sil(kullanan, liste);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}
        //public ObjectArray YaziDurumuListele(Tanimlama bilgi)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = EVRYaziDurumu.Listele(bilgi);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        donusDeger.sonuc.islemSonuc = false;
        //    }

        //    return donusDeger;
        //}

        //public Sonuc GelisSekliKaydet(Kullanici kullanan, ObjectArray liste)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = EVRGelisSekli.Kaydet(kullanan, liste);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}
        //public Sonuc GelisSekliSil(Kullanici kullanan, ObjectArray liste)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = EVRGelisSekli.Sil(kullanan, liste);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}
        //public ObjectArray GelisSekliListele(Tanimlama bilgi)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = EVRGelisSekli.Listele(bilgi);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        donusDeger.sonuc.islemSonuc = false;
        //    }

        //    return donusDeger;
        //}

        public Sonuc DosyaPlaniBolumTanimKaydet(Kullanici kullanan, DosyaPlaniBolumTanim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDosyaPlani.DosyaPlaniBolumTanimKaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray DosyaPlaniBolumTanimListele(DosyaPlaniBolumTanim bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDosyaPlani.DosyaPlaniBolumTanimListele(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray DosyaPlaniBolumListele(DosyaPlaniBolum bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDosyaPlani.DosyaPlaniBolumListele(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public Sonuc DosyaPlaniBolumKaydet(Kullanici kullanan, DosyaPlaniBolum bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDosyaPlani.DosyaPlaniBolumKaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc DosyaPlaniKlasorBolumOlustur(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDosyaPlani.DosyaPlaniKlasorBolumOlustur(null, kullanan);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray DosyaPlaniKlasorBolumListele(DosyaPlaniBolum bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDosyaPlani.ListeleKlasorBolum(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public Sonuc DosyaPlaniEvrakKaydet(Kullanici kullanan, string prEvrakNo, ArrayList liste, bool eskiKodlariSil, string gerekce)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakDosyaPlani.Kaydet(null, kullanan, prEvrakNo, liste, eskiKodlariSil, gerekce);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        /// <summary>
        /// Paramete olarak verilen evrEvrakNo nesnesine göre, evrak'ın tarihçesini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="evrEvrakNo">evrEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakTarihce nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray EvrakTarihceListele(string evrEvrakNo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVREvrakTarihce.Listele(null, evrEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Evrak'ın kullanıcının kendi biriminde olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <param name="teslimAlmaIslemi">Evrak teslim alma bilgisini içeren nesne.</param>
        /// <returns>Evrak kullanıcının kendi biriminde ise doğru, yoksa yanlış döndürülür.<</returns>
        public bool EvrakKendiBirimindeMi(Kullanici kullanan, EvrakSinif form, bool teslimAlmaIslemi)
        {
            bool donusDeger = false;
            try
            {
                donusDeger = EVREvrak.EvrakKendiBirimindeMi(null, kullanan, form, teslimAlmaIslemi);
            }
            catch
            {
                donusDeger = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen kullanıcının evrak üzerinde işlem yapma yetkisini kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <param name="islem">Evrak hareket durumunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc IslemYapmaYetkisiVarMi(Kullanici kullanan, EvrakSinif form, ENUMEvrakHareketDurum islem)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.IslemYapmaYetkisiVarMi(null, kullanan, form, islem);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak hareket işlemerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hareketTip">Evrak hareket durumunu içeren nesne</param>
        /// <param name="liste">EvrakSinif nesne koleksiyonu tipinde evrak bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakIslem(Kullanici kullanan, ENUMEvrakHareketDurum hareketTip, ObjectArray liste, bool yetkiKontrolEt = true)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                switch (hareketTip)
                {
                    case ENUMEvrakHareketDurum.TESLIMALDI:
                        sonuc = EVREvrak.TeslimAl(null, kullanan, liste);
                        break;
                    case ENUMEvrakHareketDurum.DOSYADANALINDI:
                        sonuc = EVREvrak.DosyadanAl(null, kullanan, liste);
                        break;
                    case ENUMEvrakHareketDurum.HAVALEGERIALINDI:
                        sonuc = EVREvrak.HavaleGeriAl(null, kullanan, liste);
                        break;
                    case ENUMEvrakHareketDurum.HAVALEEDILDI:
                    case ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN:
                        sonuc = EVREvrak.HavaleEt(null, kullanan, liste, yetkiKontrolEt);
                        break;
                    case ENUMEvrakHareketDurum.DOSYAYAKALDIR:
                        sonuc = EVREvrak.DosyayaKaldir(null, kullanan, liste);
                        break;
                    case ENUMEvrakHareketDurum.IPTALEDILDI:
                        sonuc = EVREvrak.IptalEt(null, kullanan, liste);
                        break;
                    case ENUMEvrakHareketDurum.IPTALGERIALINDI:
                        sonuc = EVREvrak.IptalGeriAl(null, kullanan, liste);
                        break;
                    case ENUMEvrakHareketDurum.NUMARAALINDI:
                        sonuc = EVREvrak.GidenEvrakNumaraVer(null, kullanan, liste, yetkiKontrolEt);
                        break;
                    case ENUMEvrakHareketDurum.IADEEDILDI:
                        sonuc = EVREvrak.HavaleGeriGonder(null, kullanan, liste);
                        break;
                    case ENUMEvrakHareketDurum.ARSIVEKALDIR:
                        sonuc = EVREvrak.ArsiveKaldir(null, kullanan, liste);
                        break;
                    //case ENUMEvrakHareketDurum.GERIGONDERILDI:
                    //case ENUMEvrakHareketDurum.IMZALANDI:
                    //case ENUMEvrakHareketDurum.PARAFLANDI:
                    //    sonuc = EVREvrak.Imzala(kullanan, liste, hareketTip);
                    //    break;
                    //case ENUMEvrakHareketDurum.EIMZALANDI:
                    //    sonuc = EVREvrak.EImzala(kullanan, liste, hareketTip);
                    //    break;
                    case ENUMEvrakHareketDurum.IMZADANGERIALINDI:
                        sonuc = EVREvrak.ImzadanGeriAl(null, kullanan, liste);
                        break;
                    default:
                        sonuc.hataStr = EvrakI.EVRI158;
                        sonuc.islemSonuc = false;
                        break;
                }

            }
            catch (Exception e)
            {
                sonuc.hataStr = e.ToString();
                sonuc.islemSonuc = false;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesnesine göre, Evrak bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne </param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakKaydet(Kullanici kullanan, EvrakSinif form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            try
            {
                Sonuc sonuc = EVREvrak.Kaydet(ve, kullanan, form);
                ve.VeriErisimKapat(false);
                return sonuc;
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat(false);
                OrtakFonksiyonlar.HataStrYaz("{F00A1DAF-C779-4ED1-978B-1E1B2BBD087XX} Message: " + e.Message);
                Sonuc sonuc = new Sonuc();
                sonuc.hataStr = e.ToString();
                sonuc.islemSonuc = false;
                return sonuc;
            }
        }

        public Sonuc EvrakGizlikSureUzatimiKaydet(Kullanici kullanan, string[] evrEvrakNolar, TNSDateTime sureSonuTarih)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = EVREvrak.EvrakGizlikSureUzatimiKaydet(null, kullanan, evrEvrakNolar, sureSonuTarih);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("{F00A1DAF-C779-4ED1-978B-1E1B2BBD087XX} Message: " + e.Message);
                sonuc.hataStr = e.ToString();
                sonuc.islemSonuc = false;
            }
            return sonuc;
        }

        public Sonuc EvrakGizlikDerecesiKaydet(Kullanici kullanan, string[] evrEvrakNolar, int gizlilikTip)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = EVREvrak.EvrakGizlikDerecesiKaydet(null, kullanan, evrEvrakNolar, gizlilikTip);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("{11381C48-9A32-4D53-820E-5BD655E9F188} Message: " + e.Message);
                sonuc.hataStr = e.ToString();
                sonuc.islemSonuc = false;
            }
            return sonuc;
        }

        /// <summary>
        /// Evrak'ın havale sırasında kaydedilen notlarını okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakHareket nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray EvrakHavaleNotuListele(string prEvrakNo)
        {
            try
            {
                return EVREvrakHareket.HavaleNotuListele(null, prEvrakNo);
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
        /// Paramete olarak verilen EvrakHareket nesnesine göre, evrak'ın hareket bilgisini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">EvrakHareket bilgisini içeren nesne</param>
        /// <returns>EvrakHareket nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray EvrakHareketListele(EvrakHareket form)
        {
            try
            {
                return EVREvrakHareket.Listele(null, form);
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
        /// Parametre olarak verilen EvrakHareket nesnesine göre, Evrak hareket bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EvrakHareket bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakHareketKaydet(Kullanici kullanan, EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = EVREvrakHareket.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                sonuc.hataStr = e.ToString();
                sonuc.islemSonuc = false;
            }

            return sonuc;
        }

        /// <summary>
        /// Evrak'ın son hareket durumunu okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakHareket nesnesi olarak döndürülür.</returns>
        public EvrakHareket EvrakSonHaraketGetir(string prEvrakNo)
        {
            EvrakHareket hareket = new EvrakHareket();
            try
            {
                hareket = EVREvrakHareket.SonHaraketGetir(null, prEvrakNo);
            }
            catch
            {
                hareket = null;
            }

            return hareket;
        }

        /// <summary>
        /// Evrak'ın hareket bilgisine okundu bilgisini kaydeden yordam.
        /// </summary>
        /// <param name="form">EvrakHareket bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc OkunduOlarakIsaretle(EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = EVREvrakHareket.OkunduOlarakIsaretle(null, form);
            }
            catch (Exception e)
            {
                sonuc.hataStr = e.ToString();
                sonuc.islemSonuc = false;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesnesine göre, evrak sayısını hesaplayan yordam.
        /// </summary>
        /// <param name="form">EvrakSinif bilgisini içeren nesne</param>
        /// <returns>Evrak sayısı double nesene türünde döndürülür.</returns>
        public double[] EvrakSayisiVer(Kullanici kullanan, EvrakSinif[] form)
        {
            double[] donusDeger = new double[form.Length];
            try
            {
                donusDeger = EVREvrakHareket.EvrakSayisiVer(ref ve, kullanan, form);
            }
            catch
            {
                //donusDeger = 0;

            }

            return donusDeger;
        }

        public ObjectArray EvrakListele(Kullanici kullanan, EvrakSinif bilgi, int start, int limit, bool sadeceSatirSayisi = false)
        {
            try
            {
                return EVREvrak.Listele(null, kullanan, bilgi, start, limit, sadeceSatirSayisi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        public string[] EvrakListelePrEvrakNo(EvrakSinif form)
        {
            try
            {
                return EVREvrak.ListelePrEvrakNo(null, form);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Paramete olarak verilen Antet nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Antet nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc AntetKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRAntet.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Antet nesne koleksiyonunu içeren Antet bilgilerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Antet nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc AntetSil(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRAntet.Sil(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Paramete olarak verilen Antet nesnesine göre, Antet bilgisini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Antet bilgisini içeren nesne</param>
        /// <returns>Antet nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray AntetListele(Kullanici kullanan, Antet bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRAntet.Listele(null, kullanan, bilgi);
                if (bilgi != null && donusDeger == null || donusDeger.objeler.Count > 0)
                {
                    if (string.IsNullOrEmpty(((Antet)donusDeger.objeler[0]).antet))
                    {
                        bilgi.evrakBirimKodu = this.EvrakKokBirimGetir(bilgi.evrakBirimKodu);
                        donusDeger = EVRAntet.Listele(null, kullanan, bilgi);
                    }
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Paramete olarak verilen Altbilgi nesne koleksiyonunu, kullanıcının evrak altbilgileri olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc AltbilgiKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRAltbilgi.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Altbilgi nesne koleksiyonunu içeren Altbilgileri veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc AltbilgiSil(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRAltbilgi.Sil(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Paramete olarak verilen Altbilgi nesnesine göre, kullanıcının evrak altbilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>Altbilgi nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray AltbilgiListele(Kullanici kullanan, Altbilgi bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRAltbilgi.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak konu bilgisini EvrakKonu nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="konu">Evrak konu biglisini içeren nesne</param>
        /// <param name="kullanimSayisi">Kullanım sayısını içeren nesne</param>
        /// <returns>EvrakKonu nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray EvrakKonuListele(string birim, string konu, int kullanimSayisi)
        {
            try
            {
                return EVREvrak.KonuListele(null, birim, konu, kullanimSayisi);
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
        /// Parametere olarak verilen bilgilere göre, evrak konu bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="konu">Evrak konu biglisini içeren nesne</param>
        /// <param name="kalici">Kalıcı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakKonuKaydet(string birim, string konu, int kalici)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.KonuKaydet(null, birim, konu, kalici);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakKonuSil(string birim, string konu)
        {
            try
            {
                return EVREvrak.KonuSil(null, birim, konu);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray MetaGrupListele(string birim, int kod, string adi)
        {
            try
            {
                return EVREvrak.MetaGrupListele(null, birim, kod, adi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        public Sonuc MetaGrupKaydet(string birim, int kod, string adi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.MetaGrupKaydet(null, birim, kod, adi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc MetaGrupSil(string birim, int kod)
        {
            try
            {
                return EVREvrak.MetaGrupSil(null, birim, kod);
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
        /// Parametere olarak verilen bilgilere göre, evrak yer bilgisini EvrakYer nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer biglisini içeren nesne</param>
        /// <param name="tur">Yer türü bilgisini içeren nesne</param>
        /// <param name="kullanimSayisi">Kullanım sayısını içeren nesne</param>
        /// <returns>EvrakYer nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray EvrakYerIsimleriListele(string birim, string yer, int tur, int kullanimSayisi)
        {
            try
            {
                return EVREvrak.YerIsimleriListele(null, birim, yer, tur, kullanimSayisi);
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
        /// Parametere olarak verilen bilgilere göre, evrak yer bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer biglisini içeren nesne</param>
        /// <param name="tur">Yer türü bilgisini içeren nesne</param>
        /// <param name="yerBirimKodu">Yer birim kodu bilgisini içeren nesne</param>
        /// <param name="kalici">Kalıcı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakYerIsimleriKaydet(string birim, string yer, int tur, string yerBirimKodu, int kalici)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.YerIsimleriKaydet(null, birim, yer, tur, yerBirimKodu, kalici);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak yer bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer biglisini içeren nesne</param>
        /// <param name="tur">Yer türü bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakYerIsimleriSil(string birim, string yer, int tur)
        {
            try
            {
                return EVREvrak.YerIsimleriSil(null, birim, yer, tur);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, paraf bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Evrak paraf biglisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc ParafKaydet(Kullanici kullanan, Paraf bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRParaf.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak paraf bilgisini Paraf nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kod">Paraf kod bilgisini içeren nesne</param>
        /// <param name="kullaniciKod">Kullanıcı kod bilgisini içeren nesne</param>
        /// <param name="detaylarYuklensin">Paraf detaylarının listelenme durumunu içeren nesne</param>
        /// <returns>Paraf nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray ParafListele(Kullanici kullanan, int kod, string kullaniciKod, bool detaylarYuklensin)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRParaf.Listele(null, kullanan, kod, kullaniciKod, detaylarYuklensin);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak paraf bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="parafKod">Paraf kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc ParafSil(Kullanici kullanan, int parafKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRParaf.Sil(null, kullanan, parafKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak dağıtım bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Dağıtım bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc DagitimKaydet(Kullanici kullanan, Dagitim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDagitim.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, dağıtım bilgisini Dagitim nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kodList">Dağıtım kod listesini içeren nesne</param>
        /// <param name="evrakBrimKod">Evrak birim kodunu içeren nesne</param>
        /// <param name="alanlarYuklensin">Dağıtım alanlarının listelenme durumunu içeren nesne</param>
        /// <returns>Dagitim nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray DagitimListele(Kullanici kullanan, string kodList, string evrakBrimKod, bool alanlarYuklensin)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDagitim.Listele(null, kullanan, kodList, evrakBrimKod, alanlarYuklensin);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, dağıtım bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="dagitimKod">Dağıtım kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc DagitimSil(Kullanici kullanan, int dagitimKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDagitim.Sil(null, kullanan, dagitimKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen prEvrakNo bilgisine göre, evrak'ın dağıtım bilgisini EvrakDagitim nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakDagitim nesne koleksiyonunu olarak döndürülür.</returns>
        public ArrayList EvrakDagitimListele(string prEvrakNo)
        {
            ArrayList donusDeger = new ArrayList();
            try
            {
                donusDeger = EVREvrakDagitim.Listele(null, prEvrakNo);
            }
            catch
            {
                //
            }

            return donusDeger;
        }

        //public Sonuc GidenTurKaydet(Kullanici kullanan, ObjectArray liste)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = EVRGidenTur.Kaydet(kullanan, liste);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}
        //public Sonuc GidenTurSil(Kullanici kullanan, ObjectArray liste)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = EVRGidenTur.Sil(kullanan, liste);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}
        //public ObjectArray GidenTurListele(Kullanici kullanan, Tanimlama bilgi)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = EVRGidenTur.Listele(kullanan, bilgi);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        donusDeger.sonuc.islemSonuc = false;
        //    }

        //    return donusDeger;
        //}

        /// <summary>
        /// Parametre olarak verilen prEvrakNo bilgisine göre, evrak dosya kod bilgisini DosyaPlani nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="prEvrakNo">Evrak'ın prEvrakNo bilgisini içeren nesne</param>
        /// <returns>DosyaPlani nesne koleksiyonunu olarak döndürülür.</returns>
        public ArrayList EvrakDosyaKodOku(Kullanici kullanan, string prEvrakNo)
        {
            return EVREvrakDosyaPlani.Oku(null, kullanan, prEvrakNo);
        }

        /// <summary>
        /// Paramete olarak verilen DosyaPlani nesne koleksiyonunu, kullanıcının evrak dosya kodu bilgileri olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>        
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakDosyaKodKaydet(Kullanici kullanan, string prEvrakNo, ArrayList liste)
        {
            try
            {
                return EVREvrakDosyaPlani.Kaydet(null, kullanan, prEvrakNo, liste, true, null);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc EvrakDosyaKodTopluDegistir(Kullanici kullanan, string eskiKod, string yeniKod, TNSDateTime tarih1, TNSDateTime tarih2, string aciklama, string bakimdakiler)
        {
            try
            {
                return EVREvrakDosyaPlani.TopluDegisiklik(null, kullanan, eskiKod, yeniKod, tarih1, tarih2, aciklama, bakimdakiler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, ilişkili evrakların evrEvrakNo ve prEvrakNo bilgileri metin dizisi olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="iliskiprEvrakNo">İlişki prEvrakNo bilgisini içeren nesne</param>
        /// <param name="iliskiTip">Evrak ilişki tipini içeren nese</param>
        /// <returns>Evrak ilişki bilgileri metin dizisi olarak döndürülür.</returns>
        public string[] EvrakIliskidenEvrVePrEvrakNoVer(string iliskiprEvrakNo, int iliskiTip)
        {
            try
            {
                return EVREvrak.EvrakIliskidenEvrVePrEvrakNoVer(null, iliskiprEvrakNo, iliskiTip);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Paramete olarak verilen prEvrakNo nesnesine göre, evrak'ın evrEvrakNo, evrakTarih ve evrakNo bilgisini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakSinifKucuk nesesi olarak döndürülür.</returns>
        public EvrakSinifKucuk EvrakNoVeTarihVer(string prEvrakNo)
        {
            try
            {
                return EVREvrak.EvrakNoVeTarihVer(null, prEvrakNo);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Paramete olarak verilen prEvrakNo nesnesine göre, evrak'ın ilişkili olduğu diğer evrakları EvrakSinif nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakSinif nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray IliskiliEvraklariListele(string prEvrakNo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVREvrak.ListeleIliskiliEvraklari(ref ve, prEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Paramete olarak verilen Sablon nesnesini, kullanıcı bilgileri ile birlikte evrak şablonunu olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Şablon bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc SablonKaydet(Kullanici kullanan, Sablon form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRSablon.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen şablon bilgisine göre, evrak'ın şablon bilgisini Sablon nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Şablon bilgilerini içeren nesne</param>
        /// <returns>Sablon nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray SablonListele(Kullanici kullanan, Sablon form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRSablon.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, şablon bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="sablonId">Şablon ID bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc SablonSil(Kullanici kullanan, int sablonId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRSablon.Sil(null, kullanan, sablonId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }


        public Sablon SablonOku(Kullanici kullanan, Sablon form)
        {
            try
            {
                return EVRSablon.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Mobil imza için, belgenin imza durumunu kontrol eden yordam.
        /// </summary>
        /// <param name="data">Kontrol edilecek veri dizisi nesnesi</param>
        /// <returns>İmzalı ise doğru, yoksa yanlış döndürülür.</returns>
        public bool ImzaliMi(byte[] data)
        {
            bool donusDeger = false;
            try
            {
                donusDeger = new EVRMobilImza().ImzaliMi(data);
            }
            catch
            {

            }

            return donusDeger;
        }

        /// <summary>
        /// Mobil imza için, sertfikanın gerçerliliğini kontrol eden yordam.
        /// </summary>
        /// <param name="data">Kontrol edilecek veri dizisi nesnesi</param>
        /// <param name="tarih">ontrol edilecek tarih nesnesi</param>
        /// <returns>İşlem sonucu metin olarak döndürülür.</returns>
        public string SertifikaGecerliMi(byte[] data, TNSDateTime tarih)
        {
            string donusDeger = string.Empty;
            try
            {
                donusDeger = new EVRMobilImza().SertifikaGecerliMi(data, tarih);
            }
            catch
            {
            }

            return donusDeger;
        }

        /// <summary>
        /// Mobil imza için, gönderilen veri dizisindeki imzaları döndüren yordam.
        /// </summary>
        /// <param name="data">Kontrol edilecek veri dizisi nesnesi</param>
        /// <returns>İşlem sonucu imza bilgileri nesne koleksiyonu türünde döndürülür.</returns>
        public ObjectArray BelgedekiImzalar(byte[] data)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new EVRMobilImza().BelgedekiImzalar(data);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Mobil imza için, veri dizisini imazaya gönderen yordam.
        /// </summary>
        /// <param name="data">Veri dizisi içeren nesne</param>
        /// <param name="gsmOperator">GSM operatörlerini içeren nesne</param>
        /// <param name="telNo">Telefon numarasını içeren nesne</param>
        /// <returns>İşlem sonucu ImzaSonuc nesnesi türünde döndürülür.</returns>
        public ImzaSonuc ImzayaGonder(byte[] data, ENUMGSMOperator gsmOperator, string telNo)
        {
            ImzaSonuc donusDeger = new ImzaSonuc();
            try
            {
                donusDeger = new EVRMobilImza().ImzayaGonder(data, gsmOperator, telNo);
            }
            catch (Exception ex)
            {
                donusDeger.sonuc.hataStr = ex.Message;
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Mobil imza için, imzalama işlemini yapan yordam. 
        /// </summary>
        /// <param name="hr">İmzalanacak bilgiyi içeren nesne</param>
        /// <param name="cepMesaj">İmzalamak için cep telefonuna gelen mesajı içeren nesne</param>
        /// <returns>İşlem sonucu ImzaSonuc nesnesi türünde döndürülür.</returns>
        public ImzaSonuc MobilImzala(ImzaSonuc hr, string cepMesaj)
        {
            ImzaSonuc donusDeger = new ImzaSonuc();
            try
            {
                donusDeger = new EVRMobilImza().Imzala(hr, cepMesaj);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen evrak nesne koleksiyonuna göre, giden evrak dağıtım listesini gönderilen evraklar bilgisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="EvrakListe">EvrakSinif nesne koleksiyonu tipinde evrak bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakGonderilenKaydet(Kullanici kullanan, ObjectArray EvrakListe)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakGonderilen.Kaydet(null, kullanan, EvrakListe);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakGonderilen nesne koleksiyonuna göre, gönderilen evraklar bilgisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">EvrakGonderilen nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakGonderilenKaydet(ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakGonderilen.Kaydet(null, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, Evrak gönderilen bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="gonderilenBirimKodu">Gönderilen birim kodunu içeren nesne</param>
        /// <param name="alindimi">Alındı bilgisini içeren nesne</param>
        /// <returns>EvrakGonderilen nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray EvrakGonderilenListele(string gonderilenBirimKodu, int alindimi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVREvrakGonderilen.Listele(null, gonderilenBirimKodu, alindimi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Paramete olarak verilen Haberci nesnesini, evrak habercisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Haberci bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc HaberciKaydet(Haberci form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHaberci.Kaydet(null, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgisine göre, evrak haberci listesini Haberci nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="tcKimlikNo">TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="kodList">Haberci kod listesini içeren nesne</param>
        /// <param name="evrakBirimKodu">Evrak birim kodunu içeren nesne</param>
        /// <param name="detaylariGetir">Detay bilgilerinin getirilmesini belirten nesne</param>
        /// <returns>Haberci nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray HaberciListele(string tcKimlikNo, string kodList, string evrakBirimKodu, bool detaylariGetir)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRHaberci.Listele(null, tcKimlikNo, kodList, evrakBirimKodu, detaylariGetir);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, haberci bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="haberciKod">Haberci kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc HaberciSil(int haberciKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHaberci.Sil(null, haberciKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen HavaleKural nesnesine göre, havale kuralı olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">HavaleKural bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc HavaleKuralKaydet(HavaleKural form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHavaleKural.Kaydet(null, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak havale kural listesini HavaleKural nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kodList">Havale kural kod listesini içeren nesne</param>
        /// <param name="tcKimlikNo">TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="evrakBirimKodu">Evrak birim kodunu içeren nesne</param>
        /// <returns>HavaleKural nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray HavaleKuralListele(string kodList, string tcKimlikNo, string evrakBirimKodu)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRHavaleKural.Listele(null, kodList, tcKimlikNo, evrakBirimKodu);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, havale kural bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="havaleKuralKodu">Havale kural kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc HavaleKuralSil(int havaleKuralKodu)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHavaleKural.Sil(null, havaleKuralKodu);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, havale kural bilgilerine bakarak evrak'ı havale eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <param name="kodList">Havale kural kod listesini içeren nesne</param>
        /// <param name="tcKimlikNo">TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="evrakBirimKodu">Evrak birim kodunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc HavaleKuralIleHavale(Kullanici kullanan, EvrakSinif form, string kodList, string tcKimlikNo, string evrakBirimKodu)
        {
            try
            {
                return EVREvrak.HavaleKuralIleHavale(null, kullanan, form, kodList, tcKimlikNo, evrakBirimKodu);
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
        /// Parametre olarak verilen Tanimlama nesne koleksiyonunu, posta tür bilgisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Tanimlama nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc PostaIslemTurKaydet(ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRPostaIslemTur.Kaydet(null, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Tanimlama bilgisine göre, posta tür listesini Tanimlama nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Tanimlama bilgisini içeren nesne</param></param>
        /// <returns>Tanimlama nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray PostaIslemTurListele(Tanimlama bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRPostaIslemTur.Listele(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, posta tür bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Tanimlama nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc PostaIslemTurSil(ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRPostaIslemTur.Sil(null, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen PostaIslem bilgisine göre, posta işlemini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">PostaIslem bilgisini içeren nesne</param></param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc PostaIslemFormKaydet(PostaIslem form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRPostaIslemForm.Kaydet(null, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen PostaIslem bilgisine göre, posta işlem listesini PostaIslem nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">PostaIslem bilgisini içeren nesne</param></param>
        /// <returns>PostaIslem nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray PostaIslemFormListele(PostaIslem form, bool detay)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRPostaIslemForm.Listele(null, form, detay);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, posta işlem bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">PostaIslem nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc PostaIslemFormSil(ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRPostaIslemForm.Sil(liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen PostaIslem nesnesine göre, evrak bilgilerini PostaIslemDetay nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">PostaIslem nesne koleksiyonunu içeren nesne</param>
        /// <returns>PostaIslemDetay nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray PostaIslemFormListeleEvrak(PostaIslem form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRPostaIslemForm.ListeleEvrak(null, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parammetre olarak verilen rapor türüne göre, evrak sayılarını içeren rapor bilgisini nesne dizisi olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <param name="raporTur">Rapor türünü içeren nesne</param>
        /// <returns>Nesne dizisi olarak döndürülür.</returns>
        public ObjectArray RaporListele(EvrakSinif form, ENUMEvrakRaporTuru raporTur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRRapor.Listele(null, form, raporTur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, personel bilgilerini personel nesne koleksiyonunu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Personel bilgilerini içeren nesne</param>
        /// <returns>Personel nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray PersonelListele(Personel bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRBirim.PersonelListele(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen tanımlama tür bilisine göre, tamımlama bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Tanımlama nesne koleksiyonunu içeren nesne</param>
        /// <param name="tanimlamFormTur">Tanımlama tür bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc TanimlamaKaydet(Kullanici kullanan, ObjectArray liste, ENUMTamimlamaFormTur tanimlamFormTur)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRTamimlama.Kaydet(null, kullanan, liste, tanimlamFormTur);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, tanımlama bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Tanımlama nesne koleksiyonunu içeren nesne</param>
        /// <param name="tanimlamFormTur">Tanımlama tür bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc TanimlamaSil(Kullanici kullanan, ObjectArray liste, ENUMTamimlamaFormTur tanimlamFormTur)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRTamimlama.Sil(null, kullanan, liste, tanimlamFormTur);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, tanımlama bilgilerini Tanımlama nesne koleksiyonunu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">>Tanımlama bilgisini içeren nesne</param>
        /// <param name="tanimlamFormTur">Tanımlama tür bilgisini içeren nesne</param>
        /// <returns>Tanımlama nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray TanimlamaListele(Kullanici kullanan, Tanimlama bilgi, ENUMTamimlamaFormTur tanimlamFormTur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRTamimlama.Listele(null, kullanan, bilgi, tanimlamFormTur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen kullanıcının yetkili olduğu evrak birim kodunu getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>Metin nesnesi olarak döndürülür.</returns>
        public string EvrakBirimKodListele(Kullanici kullanan)
        {
            string degerler = "";
            foreach (System.Collections.DictionaryEntry de in kullanan.yetkiTipiDegerleri)
            {

                string ytAd = (string)de.Key;
                if (ytAd.Equals(EvrakI.EVRI001_1))
                {
                    string d = (string)de.Value;
                    if (d.Trim() != "")
                        degerler = d;
                }
            }
            return degerler;
        }

        /// <summary>
        /// Parametre olarak verilen kaynak ve hedef evrak bilgilerine göre, evrak'ın dosyasını kopyalayan yordam.
        /// </summary>
        /// <param name="kaynakEvrak">Kopyalanacak evrak bilgilerini içeren nesne</param>
        /// <param name="hedefEvrak">Oluşturulacak evrak bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakEkDosyaKopyala(Kullanici kullalan, EvrakSinif kaynakEvrak, EvrakSinif hedefEvrak)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.EvrakEkDosyaKopyala(null, kullalan, kaynakEvrak, hedefEvrak);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgilerine göre, evrak'ın dosya yolununu metin nesnesi olrak getiren yordam.
        /// </summary>
        /// <param name="evrakBirimi">Evrak birim kodunu içeren nesne</param>
        /// <param name="evrakOlusturmaTarihi">Evrak oluşturma tarihini içeren nesne</param>
        /// <param name="evrEvrakNo">Evrak evrEvrakNo bilgisini içeren nesne</param>
        /// <returns></returns>
        public string EvrakEkDosyaYolu(string evrakBirimi, TNSDateTime evrakOlusturmaTarihi, string evrEvrakNo)
        {
            string donusDeger = "";
            try
            {
                donusDeger = EVREvrak.EvrakEkDosyaYolu(null, evrakBirimi, evrakOlusturmaTarihi, evrEvrakNo);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen evrak EvrakIliski nesne koleksiyonunu bilgilerini veritabanına kaydeden yordam.
        /// </summary>        
        /// <param name="evrakIliskiArr">Evrak ilişki bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakIliskiKaydet(Kullanici kullalan, ObjectArray evrakIliskiArr)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.EvrakIliskiKaydet(null, kullalan, evrakIliskiArr);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen evrak EvrakIliski nesne koleksiyonunu bilgilerini veritabanından silen yordam.
        /// </summary>        
        /// <param name="evrakIliskiArr">Evrak ilişki bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc EvrakIliskiSil(Kullanici kullalan, ObjectArray evrakIliskiArr)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.EvrakIliskiSil(null, kullalan, evrakIliskiArr);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }


        public Sonuc EvrakDegisiklikKaydet(EvrakDegisiklik form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakDegisiklik.Kaydet(null, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray EvrakDegisiklikListele(EvrakDegisiklik bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVREvrakDegisiklik.Listele(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        //public ObjectArray KisiUzerindekileriListele(Kullanici kullanan, EvrakSinif form)
        //{
        //    try
        //    {
        //        return EVREvrak.KisiUzerindekileriListele(null, kullanan, form);
        //    }
        //    catch (Exception e)
        //    {
        //        ObjectArray donusDeger = new ObjectArray();
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        donusDeger.sonuc.islemSonuc = false;

        //        return donusDeger;
        //    }
        //}

        public ObjectArray EvrakHareketleriniRaporla(Kullanici kullanan, EvrakSinif form)
        {
            try
            {
                return EVREvrak.EvrakHareketleriniRaporla(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;

                return donusDeger;
            }
        }

        public ObjectArray EvrakEkDosyalariniRaporla(Kullanici kullanan, EvrakSinif form)
        {
            try
            {
                return EVREvrak.EvrakEkDosyalariniRaporla(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;

                return donusDeger;
            }
        }

        public ObjectArray DosyaTasnifPlaniRaporla(Kullanici kullanan, EvrakSinif form)
        {
            try
            {
                return EVREvrak.DosyaTasnifPlaniRaporla(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;

                return donusDeger;
            }
        }

        public Sonuc EvrakVersiyonKaydet(EvrakVersiyon evrVersiyon)
        {
            try
            {
                return EVREvrakVersiyon.Kaydet(null, evrVersiyon);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public EvrakVersiyon[] EvrakVersiyonListele(Kullanici kullanan, EvrakVersiyon form)
        {
            return EVREvrakVersiyon.Listele(null, kullanan, form);
        }
        public EvrakVersiyon EvrakVersiyonOku(Kullanici kullanan, EvrakVersiyon form)
        {
            return EVREvrakVersiyon.Oku(null, kullanan, form);
        }
        public Sonuc Sil(Kullanici kullanan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakVersiyon.Sil(null, kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakVersiyon2Kaydet(EvrakVersiyon2 form)
        {
            try
            {
                return EVREvrakVersiyon2.Kaydet(null, form);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public EvrakVersiyon2[] EvrakVersiyon2Listele(Kullanici kullanan, EvrakVersiyon2 form)
        {
            return EVREvrakVersiyon2.Listele(null, kullanan, form);
        }
        public EvrakVersiyon2 EvrakVersiyon2Oku(Kullanici kullanan, EvrakVersiyon2 form)
        {
            return EVREvrakVersiyon2.Oku(null, kullanan, form);
        }

        public bool EvrakOlaySorgu(string prEvrakNo, string evrEvrakNo, ENUMOlaySorguTip olaySorgu)
        {
            return EVREvrak.EvrakOlaySorgu(null, prEvrakNo, evrEvrakNo, olaySorgu);
        }

        public Sonuc ImzaYoluKaydet(Kullanici kullanan, ImzaYolu form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRImzaYolu.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc ImzaYoluSil(Kullanici kullanan, ImzaYolu form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRImzaYolu.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ImzaYolu[] ImzaYoluListele(Kullanici kullanan, ImzaYolu form)
        {
            try
            {
                return EVRImzaYolu.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ImzaYolu ImzaYoluOku(Kullanici kullanan, ImzaYolu form)
        {
            try
            {
                return EVRImzaYolu.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc EvrakImzaYoluKaydet(Kullanici kullanan, EvrakImzaYolu form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakImzaYolu.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakImzaYoluKaydet(Kullanici kullanan, EvrakImzaYoluHareket form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakImzaYolu.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakImzaYoluSil(Kullanici kullanan, EvrakImzaYolu form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakImzaYolu.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EvrakImzaYoluHareket[] EvrakImzaYoluListele(Kullanici kullanan, string prEvrakNo)
        {
            try
            {
                return EVREvrakImzaYolu.Listele(null, kullanan, prEvrakNo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakImzaYolu EvrakImzaYoluOku(Kullanici kullanan, EvrakImzaYolu form)
        {
            try
            {
                return EVREvrakImzaYolu.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //public string EvrakImzalayacakKisi(Kullanici kullanan, EvrakSinif form)
        //{
        //    try
        //    {
        //        return EVRImzaYolu.EvrakImzalayacakKisi(null, kullanan, form);
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
        public Sonuc ImzaYolunaGoreImzaYetkisiVarMi(Kullanici kullanan, ObjectArray evraklar)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.ImzaYolunaGoreImzaYetkisiVarMi(null, kullanan, evraklar);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc ArsivKaydet(Kullanici kullanan, Arsiv form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRArsiv.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc ArsivSil(Kullanici kullanan, Arsiv form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRArsiv.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Arsiv[] ArsivListele(Kullanici kullanan, Arsiv form)
        {
            try
            {
                return EVRArsiv.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Arsiv ArsivOku(Kullanici kullanan, Arsiv form, bool metaDetay = false)
        {
            try
            {
                return EVRArsiv.Oku(null, kullanan, form, metaDetay);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc EvrakArsivKaydet(Kullanici kullanan, EvrakArsiv form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakArsiv.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakArsivKaydet(Kullanici kullanan, EvrakArsiv[] formList)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakArsiv.Kaydet(null, kullanan, formList);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakArsivGuncelle(Kullanici kullanan, EvrakArsiv form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakArsiv.Guncelle(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakArsivSil(Kullanici kullanan, EvrakArsiv form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakArsiv.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EvrakSinif[] EvrakArsivListele(Kullanici kullanan, Arsiv form)
        {
            try
            {
                return EVREvrakArsiv.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakArsiv[] EvrakArsivListele(Kullanici kullanan, EvrakArsiv form)
        {
            try
            {
                return EVREvrakArsiv.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakArsivMeta[] EvrakArsivListele(Kullanici kullanan, EvrakArsivMeta form)
        {
            try
            {
                return EVREvrakArsiv.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakArsivDosya[] EvrakArsivListele(Kullanici kullanan, EvrakArsivDosya form)
        {
            try
            {
                return EVREvrakArsiv.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakArsivDosya[] EvrakArsivListele(Kullanici kullanan, int arsivKod, EvrakArsivMeta[] formList)
        {
            try
            {
                return EVREvrakArsiv.Listele(null, kullanan, arsivKod, formList);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakArsiv[] EvrakArsivAra(Kullanici kullanan, int arsivKod, EvrakArsivMeta[] formList)
        {
            try
            {
                return EVREvrakArsiv.Ara(null, kullanan, arsivKod, formList);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] OneriListele(string kriter, ENUMOneriTip oneriTip)
        {
            try
            {
                return EVROneri.Listele(null, kriter, oneriTip);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc MetaKaydet(Kullanici kullanan, Meta form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRMeta.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc MetaSil(Kullanici kullanan, Meta form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRMeta.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Meta[] MetaListele(Kullanici kullanan, Meta form, bool detay = false)
        {
            try
            {
                return EVRMeta.Listele(null, kullanan, form, detay);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Meta MetaOku(Kullanici kullanan, Meta form)
        {
            try
            {
                return EVRMeta.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc EvrakBilgiKaydet(Kullanici kullanan, EvrakBilgi form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakBilgi.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakBilgiSil(Kullanici kullanan, EvrakBilgi form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakBilgi.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EvrakBilgi[] EvrakBilgiListele(Kullanici kullanan, EvrakBilgi form)
        {
            try
            {
                return EVREvrakBilgi.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakBilgi[] EvrakBilgiListele(Kullanici kullanan, string[] prEvrakNoListe)
        {
            try
            {
                return EVREvrakBilgi.Listele(null, kullanan, prEvrakNoListe);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakBilgi EvrakBilgiOku(Kullanici kullanan, EvrakBilgi form)
        {
            try
            {
                return EVREvrakBilgi.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc EvrakBilgiIliskiKaydet(Kullanici kullanan, EvrakBilgiIliski form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakBilgiIliski.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakBilgiIliskiKaydet(Kullanici kullanan, EvrakBilgiIliski[] formList)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakBilgiIliski.Kaydet(null, kullanan, formList);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakBilgiIliskiSil(Kullanici kullanan, EvrakBilgiIliski form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakBilgiIliski.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EvrakBilgiIliski[] EvrakBilgiIliskiListele(Kullanici kullanan, EvrakBilgiIliski form)
        {
            try
            {
                return EVREvrakBilgiIliski.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakBilgiIliski EvrakBilgiIliskiOku(Kullanici kullanan, EvrakBilgiIliski form)
        {
            try
            {
                return EVREvrakBilgiIliski.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakImha[] EvrakImhaListele(Kullanici kullanan, TNSDateTime baslangicTarihi, TNSDateTime bitisTarihi)
        {
            try
            {
                return EVREvrak.EvrakImhaListele(null, baslangicTarihi, bitisTarihi);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakTasfiyeImha[] EvrakTasfiyeImhaListele(Kullanici kullanan, TNSDateTime baslangicTarihi, TNSDateTime bitisTarihi, EvrakTasfiyeImha form)
        {
            try
            {
                return EVREvrak.EvrakTasfiyeImhaListele(null, baslangicTarihi, bitisTarihi, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int EvrakTasfiyeImhaZamaniGelenEvrakSayisi(string evrakBirimKod, TNSDateTime bitisTarihi)
        {
            try
            {
                return EVREvrak.EvrakTasfiyeImhaZamaniGelenEvrakSayisi(null, evrakBirimKod, bitisTarihi);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool OrgSemaEvrakKullaniyorMu()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/OrgSemaEvrak"), 0) > 0 ? true : false;
        }
        public Sonuc EvrakDosyaKayitEdilebilirMi(Kullanici kullanan, string prEvrakNo, string kullaniciEvrakBirimi)
        {
            Sonuc donusDeger = new Sonuc(true, "", "");
            try
            {
                if (!string.IsNullOrEmpty(prEvrakNo))
                {
                    EvrakSinif form = new EvrakSinif();
                    ObjectArray list = EVREvrak.Listele(null, kullanan, new EvrakSinif() { prEvrakNo = prEvrakNo });
                    if (list.sonuc.islemSonuc && list.objeler.Count == 1)
                    {
                        form = (EvrakSinif)list[0];
                        form.bulunduguYer = kullaniciEvrakBirimi;
                        donusDeger = EVREvrak.DosyaKayitEdilebilirMi(null, kullanan, form);
                    }
                }
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public bool EvrakGormeYetkisiVarMi(Kullanici kullanan, EvrakSinif form)
        {
            return EVREvrak.GormeYetkisiVarMi(null, kullanan, form);
        }

        public string EvrakBirimAdGetir(string evrakBirimKod)
        {
            return EVRGenel.EvrakBirimAdGetir(evrakBirimKod);
        }
        public string EvrakBirimAdGetir(int idariKimlikKod)
        {
            return EVRGenel.EvrakBirimAdGetir(idariKimlikKod);
        }
        public string EvrakBirimKisaAdGetir(string evrakBirimKod)
        {
            return EVRGenel.EvrakBirimKisaAdGetir(evrakBirimKod);
        }
        public string DosyaPlaniAdGetir(string dosyaKodu)
        {
            return EVRGenel.DosyaPlaniAdGetir(dosyaKodu);
        }
        public string KullaniciAdGetir(string tcKimlikNo)
        {
            return EVRGenel.KullaniciAdGetir(tcKimlikNo);
        }
        public string EvrakGelisAdGetir(string kod)
        {
            return EVRGenel.EvrakGelisAdGetir(kod);
        }
        public Sonuc HavaleFavoriKaydet(Kullanici kullanan, HavaleFavori form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHavaleFavori.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc HavaleFavoriKaydet(Kullanici kullanan, HavaleFavori[] list)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHavaleFavori.Kaydet(null, kullanan, list);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc HavaleFavoriSil(Kullanici kullanan, string[] kodList)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHavaleFavori.Sil(null, kullanan, kodList);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public HavaleFavori[] HavaleFavoriListele(Kullanici kullanan, HavaleFavori form)
        {
            try
            {
                return EVRHavaleFavori.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public HavaleFavori HavaleFavoriOku(Kullanici kullanan, HavaleFavori form)
        {
            try
            {
                return EVRHavaleFavori.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc VekaletKaydet(Kullanici kullanan, Vekalet form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRVekalet.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc VekaletSil(Kullanici kullanan, Vekalet form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRVekalet.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Vekalet[] VekaletListele(Kullanici kullanan, Vekalet form)
        {
            try
            {
                return EVRVekalet.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Vekalet VekaletOku(Kullanici kullanan, Vekalet form)
        {
            try
            {
                return EVRVekalet.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Vekalet[] AktifVekaletleriListele(Kullanici kullanan)
        {
            try
            {
                return EVRVekalet.AktifVekaletleriListele(null, kullanan);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Vekalet AktifVekaletOku(Kullanici kullanan, string vekaletVerenMernis)
        {
            try
            {
                return EVRVekalet.AktifVekaletOku(null, kullanan, vekaletVerenMernis);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc DTVTBirimKaydet(Kullanici kullanan, DTVTBirim form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDTVTBirim.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public string[] DTVTBirimListele(string query)
        {
            try
            {
                return EVRDTVTBirim.Listele(null, query);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DTVTBirim DTVTKurumOku(string idareKimlikKodu)
        {
            try
            {
                return EVRDTVTBirim.KurumOku(null, idareKimlikKodu);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DTVTBirim[] DTVTAltKurumlariGetir(string idareKimlikKodu)
        {
            try
            {
                return EVRDTVTBirim.AltKurumlariGetir(null, idareKimlikKodu);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc DTVTKepAdresGuncelle(Kullanici kullanan, DTVTBirim form)
        {
            try
            {
                return EVRDTVTBirim.DTVTKepAdresGuncelle(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public int DtvtDisYazismaYapanKurumSayisi(string idareKimlikKodu, TNSDateTime guncellemeZamani)
        {
            try
            {
                return EVRDTVTBirim.DisYazismaYapanKurumSayisi(null, idareKimlikKodu, guncellemeZamani);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DTVTBirim[] DtvtDisYazismaYapanKurumlariGetir(string idareKimlikKodu, int start, int limit, TNSDateTime guncellemeZamani)
        {
            try
            {
                return EVRDTVTBirim.DisYazismaYapanKurumlariGetir(null, idareKimlikKodu, start, limit, guncellemeZamani);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc DTVTGuncellemeBilgisiKaydet(Kullanici kullanan, DTVTBirimBilgi form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDTVTBirim.GuncellemeBilgisiKaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public DTVTBirimBilgi DTVTGuncellemeBilgisiGetir(string idareKimlikKodu)
        {
            try
            {
                return EVRDTVTBirim.GuncellemeBilgisiGetir(null, idareKimlikKodu);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc DTVTBirimBilgisiKaydet(Birim form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRBirim.DTVTBirimBilgisiKaydet(null, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;

        }
        public string IdareKimlikKodGetir(string evrakBirimKod)
        {
            try
            {
                return EVRBirim.IdareKimlikKodGetir(null, evrakBirimKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string UstBirimKodGetir(string evrakBirimKod)
        {
            try
            {
                return EVRBirim.UstBirimKodGetir(null, evrakBirimKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string DagitimBirimKodGetir(string evrakBirimKod)
        {
            try
            {
                return EVRBirim.DagitimBirimKodGetir(null, evrakBirimKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc EvrakAraKriterKaydet(Kullanici kullanan, EvrakAraKriter form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakAraKriter.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EvrakAraKriter[] EvrakAraKriterListele(Kullanici kullanan, EvrakAraKriter form)
        {
            try
            {
                return EVREvrakAraKriter.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakAraKriter EvrakAraKriterOku(Kullanici kullanan, EvrakAraKriter form)
        {
            try
            {
                return EVREvrakAraKriter.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc GelenEvrakNumaraVer(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.GelenEvrakNumaraVer(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ArrayList EvrakEmailAdresleriVer()
        {
            ArrayList al = new ArrayList();
            //al.Add(new POPAdres("pop.gmail.com", 995, "test@tns.com.tr", "test2012", true));
            //al.Add(new POPAdres("pop.gmail.com", 995, "test2@tns.com.tr", "test2012", true));
            return al;
        }
        public Sonuc MailKaydet(string kullanici, string Uidl, int size, DateTime almaZamani, TNSMailMessage mesaj)
        {
            OrtakFonksiyonlar.HataStrYaz(mesaj.Body);

            if (mesaj.Ekler != null)
            {
                foreach (TNSMailEk e in mesaj.Ekler)
                {
                    OrtakFonksiyonlar.HataStrYaz("Adı:" + e.Name);

                    if (e.Content != null)
                    {
                        OrtakFonksiyonlar.HataStrYaz("İçerik:" + Encoding.UTF8.GetString(e.Content));
                    }
                    else
                        OrtakFonksiyonlar.HataStrYaz("Ek Yok");
                }
            }

            return new Sonuc(false, "Henüz yazılmadı", "");

        }
        public string BirimKodGetir(string idareKimlikKod)
        {
            try
            {
                return EVRBirim.BirimKodGetir(null, idareKimlikKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool BirimAktifMi(string kod)
        {
            try
            {
                return EVRBirim.BirimAktifMi(null, kod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc EvrakDosyaBilgisiKaydet(Kullanici kullanan, string evrEvrakNo, bool dosyasiVar)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.DosyaBilgisiKaydet(null, kullanan, evrEvrakNo, dosyasiVar);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        //public bool BirimdenBagimsizSadeceKisiyeGoreIslemYapKontrol(Kullanici kullanan)
        //{
        //    try
        //    {
        //        return EVREvrak.BirimdenBagimsizSadeceKisiyeGoreIslemYapKontrol(kullanan);
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
        public string EvrEvrakNoGetir(string prEvrakNo)
        {
            try
            {
                return EVREvrak.EvrEvrakNoGetir(null, prEvrakNo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc EvrakImzaYoluYenile(Kullanici kullanan, EvrakSinif form)
        {
            try
            {
                return EVREvrak.EvrakImzaYoluYenile(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc EvrakDosyalariniListele(Kullanici kullanan)
        {
            try
            {
                return EVROCR.EvrakDosyaTopluKaydet(null, kullanan);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc AyniGidenEvrakMevcutMu(Kullanici kullanan, EvrakSinif form)
        {
            try
            {
                return EVREvrak.AyniGidenEvrakMevcutMu(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool EvrakNoElleVerilsinMi(Kullanici kullanan, string birimKod)
        {
            try
            {
                return EVREvrak.EvrakNoElleVerilsinMi(kullanan, birimKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakImzaBilgi EvrakImzaBilgiOkuEski(Kullanici kullanan, int imzaYoluKod)
        {
            try
            {
                return EVRImzaYolu.EvrakImzaBilgiOkuEski(null, kullanan, imzaYoluKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakImzaBilgi EvrakImzaBilgiOku2(Kullanici kullanan, int imzaYoluKod)
        {
            try
            {
                return EVRImzaYolu.EvrakImzaBilgiOku2(null, kullanan, imzaYoluKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc EvrakAyarlarKaydet(EvrakAyarlar form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakAyarlar.Kaydet(null, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EvrakAyarlar EvrakAyarlarOku(string anahtarSahibi, ENUMEvrakAyarlarAnahtar anahtar)
        {
            try
            {
                return EVREvrakAyarlar.Oku(null, anahtarSahibi, anahtar);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakAyarlar[] EvrakAyarlarListele(params EvrakAyarlar[] kriter)
        {
            try
            {
                return EVREvrakAyarlar.Listele(null, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public EvrakArsivRaporEvrakSayisi[] EvrakArsivRaporEvrakSayisi(Kullanici kullanan, TNSDateTime baslangicTarih, TNSDateTime bitisTarih, string islemYapan, int arsivKod)
        {
            try
            {
                return EVREvrakArsiv.EvrakArsivRaporEvrakSayisiListele(null, kullanan, baslangicTarih, bitisTarih, islemYapan, arsivKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EvrakDosya[] EvrakDosyaOCRListele(Kullanici kullanan, EvrakDosya form, int top)
        {
            try
            {
                return EVROCR.EvrakDosyaListele(null, kullanan, form, top);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712OA} BeginTransaction - Message: " + e.Message + " # StackTrace: " + e.StackTrace);
                throw e;
            }
        }
        public Sonuc EvrakDosyaOCRGuncelle(Kullanici kullanan, EvrakDosya form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROCR.EvrakDosyaGuncelle(null, kullanan, form);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712OF} BeginTransaction - Message: " + e.Message + " # StackTrace: " + e.StackTrace);
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public string OCRIslenecekDosyalariGonder()
        {
            string donusDeger = "";
            try
            {
                donusDeger = EVROCR.OCRIslenecekDosyalariGonder();
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712OY} BeginTransaction - Message: " + e.Message + " # StackTrace: " + e.StackTrace);
                donusDeger = e.ToString();
            }

            return donusDeger;
        }
        public string OCRIslenenDosyalariAl()
        {
            string donusDeger = "";
            try
            {
                donusDeger = EVROCR.OCRIslenenDosyalariAl();
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712OM} BeginTransaction - Message: " + e.Message + " # StackTrace: " + e.StackTrace);
                donusDeger = e.ToString();
            }

            return donusDeger;
        }
        public void OCRIsleminiBaslat()
        {
            try
            {
                EVROCR.OCRIsleminiBaslat();
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712LM} BeginTransaction - Message: " + e.Message + " # StackTrace: " + e.StackTrace);
            }
        }
        public string KriptoIsleminiBaslat()
        {
            string donusDeger = "";
            try
            {
                donusDeger = EVRKripto.Kriptola();
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712KM} BeginTransaction - Message: " + e.Message + " # StackTrace: " + e.StackTrace);
                donusDeger = e.ToString();
            }

            return donusDeger;
        }

        public string IcerikIsleminiBaslat()
        {
            string donusDeger = "";
            try
            {
                donusDeger = EVRKripto.DosyaIcerikKaydet();
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712XM} BeginTransaction - Message: " + e.Message + " # StackTrace: " + e.StackTrace);
                donusDeger = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakDosyaKaydet(Kullanici kullanan, EvrakDosya dosya, bool imzaKaldir = true)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROCR.EvrakDosyaKaydet(null, kullanan, dosya, imzaKaldir);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712EM} BeginTransaction - Message: " + e.Message + " # StackTrace: " + e.StackTrace);
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakDosyaDagitimDosyalariniSil(Kullanici kullanan, string evrEvrakNo, bool imzaKaldir = true)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROCR.EvrakDosyaDagitimDosyalariniSil(null, kullanan, evrEvrakNo, imzaKaldir);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EvrakDosya[] EvrakDosyaListele(Kullanici kullanan, EvrakDosya form, int top, bool dagitimDosyalariniDahilEt = false)
        {
            try
            {
                return EVROCR.EvrakDosyaListele(null, kullanan, form, top, dagitimDosyalariniDahilEt);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ObjectArray EvrakDosyaIcerikListele(string kelime, int yil)
        {
            try
            {
                return EVROCR.EvrakDosyaIcerikListele(null, kelime, yil);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc FormKaydet(Kullanici kullanan, Form form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRForm.KaydetForm(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Form FormOku(Kullanici kullanan, Form form)
        {
            try
            {
                return EVRForm.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Form[] FormListele(Kullanici kullanan, Form form)
        {
            try
            {
                return EVRForm.ListeleForm(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public FormKontrol[] FormKontrolListele(Kullanici kullanan, FormKontrol form)
        {
            try
            {
                return EVRForm.ListeleKontrol(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public FormOzellik[] FormOzellikListele(Kullanici kullanan, FormOzellik form)
        {
            try
            {
                return EVRForm.ListeleOzellik(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc EvrakFormKaydet(Kullanici kullanan, string sayfaKod, EvrakForm[] formList)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakForm.Kaydet(null, kullanan, sayfaKod, formList);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EvrakForm[] EvrakFormListele(Kullanici kullanan, EvrakForm form)
        {
            try
            {
                return EVREvrakForm.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Paramete olarak verilen Tasfiye nesnesini, kullanıcı bilgileri ile birlikte veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Tasfiye bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc TasfiyeKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRTasfiye.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc TasfiyeSil(Kullanici kullanan, string prEvrakNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRTasfiye.Sil(null, kullanan, prEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Tasfiye bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Tasfiye bilgisini içeren nesne</param>
        /// <returns>Tasfiye nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray TasfiyeListele(Kullanici kullanan, EvrakSinif bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRTasfiye.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// Paramete olarak verilen Saklama nesnesini, kullanıcı bilgileri ile birlikte veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Saklama bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc SaklamaPlanKaydet(Kullanici kullanan, EvrakSaklama[] liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRSaklama.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc SaklamaSil(Kullanici kullanan, string kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRSaklama.Sil(null, kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Saklama bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Saklama bilgisini içeren nesne</param>
        /// <returns>Saklama nesne koleksiyonunu olarak döndürülür.</returns>
        public ArrayList SaklamaListele(Kullanici kullanan, string prEvrakNo)
        {
            ArrayList donusDeger = new ArrayList();
            try
            {
                donusDeger = EVRSaklama.Listele(null, prEvrakNo);
            }
            catch (Exception e)
            {

            }
            return donusDeger;
        }

        public Sonuc EvrakFaksKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRFaks.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakFaksGuncelle(ObjectArray liste, string islem)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRFaks.Guncelle(null, liste, islem);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakFaksSil(Kullanici kullanan, string kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRFaks.Sil(null, kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray EvrakFaksListele(Kullanici kullanan, EvrakFaks bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRFaks.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {

            }
            return donusDeger;
        }

        public Sonuc EvrakFaksTarihceKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRFaksTarihce.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakFaksTarihceSil(Kullanici kullanan, string kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRFaksTarihce.Sil(null, kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray EvrakFaksTarihceListele(Kullanici kullanan, EvrakFaksTarihce bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRFaksTarihce.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {

            }
            return donusDeger;
        }

        public Sonuc EvrakFaksKimlikKaydet(EvrakFaksKimlik form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRFaksKimlik.Kaydet(null, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
            }

            return donusDeger;
        }

        public string EvrakFaksKimlikBul(string birimKod)
        {
            string donusDeger = "";
            try
            {
                donusDeger = EVRFaksKimlik.KimlikGetir(null, birimKod);
            }
            catch (Exception e)
            {

            }

            return donusDeger;
        }

        public string FaksBirimGetir(string kimlik)
        {
            string deger = "";
            try
            {
                deger = EVRFaksKimlik.BirimGetir(null, kimlik);
            }
            catch (Exception e)
            {
                throw e;
            }

            return deger;
        }

        public ObjectArray FaksDosyaKoduGetir(string birim)
        {
            ObjectArray kod = EVRFaksKimlik.FaksDosyaKoduGetir(null, birim);
            return kod;
        }

        public string EPostaGonder(Kullanici kullanan, string evrEvrakNo, string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, TNSCollection ekDosyaIsimleri)
        {
            try
            {
                return EVRPosta.EPostaGonder(kullanan, evrEvrakNo, from, to, cc, bcc, konu, bilgi, html, replyVar, ekDosyaIsimleri);
            }
            catch (Exception e)
            {
                return "HATA:" + e.Message;
            }
        }

        public Sonuc EvrakParafImzaIslem(Kullanici kullanan, EvrakSinif evrak, string gonderilecekBirim, string gonderilecekKisi, ENUMParafImzaIslemTip parafImzaIslemTip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.ParafImzaIslem(null, kullanan, evrak, gonderilecekBirim, gonderilecekKisi, parafImzaIslemTip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakParafImzaGonder(Kullanici kullanan, EvrakSinif evrak, string gonderilecekBirim, string gonderilecekKisi, ENUMParafImzaGonderTip parafImzaGonderTip, bool sonImzaMi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.ParafImzaGonder(null, kullanan, evrak, gonderilecekBirim, gonderilecekKisi, parafImzaGonderTip, sonImzaMi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public EvrakFizikselYeri[] ListeleEvrakFizikselYeri(Kullanici kullanan, EvrakFizikselYeri form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return EVREvrakFizikselYeri.Listele(null, kullanan, form);
        }

        public EvrakFizikselYeri OkuEvrakFizikselYeri(Kullanici kullanan, EvrakFizikselYeri form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return EVREvrakFizikselYeri.Oku(null, kullanan, form);
        }

        public Sonuc KaydetEvrakFizikselYeri(Kullanici kullanan, EvrakFizikselYeri form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return EVREvrakFizikselYeri.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc EvrakDagitimHareketKaydet(Kullanici kullanan, EvrakDagitimHareket[] formList)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakDagitimHareket.Kaydet(null, kullanan, formList);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakDagitimHareketSil(Kullanici kullanan, string prEvrakNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakDagitimHareket.Sil(null, kullanan, prEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EvrakDagitimHareket[] EvrakDagitimHareketListele(Kullanici kullanan, EvrakDagitimHareket form)
        {
            try
            {
                return EVREvrakDagitimHareket.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc EvrakDagitimaGonder(Kullanici kullanan, EvrakSinif[] evraklar, EvrakDagitimHareket[] evrakDagitimHareketListe, bool dosyayaKaldir)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.DagitimaGonder(null, kullanan, evraklar, evrakDagitimHareketListe, dosyayaKaldir);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EVREYazismaBilgiKaydet(Kullanici kullanan, EYazismaBilgi form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREYazismaBilgi.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EVREYazismaBilgiSil(Kullanici kullanan, int eYazismaServisTur)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREYazismaBilgi.Sil(null, kullanan, eYazismaServisTur);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EYazismaBilgi EVREYazismaBilgiOku(EYazismaBilgi form)
        {
            try
            {
                return EVREYazismaBilgi.Oku(null, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EYazismaBilgi[] EVREYazismaBilgiListele()
        {
            try
            {
                return EVREYazismaBilgi.Listele(null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc EvrakiHazirlayanaGonder(Kullanici kullanan, string prEvrakNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.EvrakiHazirlayanaGonder(null, kullanan, prEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public bool EvrakImzaliMi(Kullanici kullanan, string evrEvrakNo)
        {
            bool imzali = false;
            try
            {
                imzali = EVREvrak.EvrakImzaliMi(null, kullanan, evrEvrakNo);
            }
            catch (Exception ex)
            {
            }
            return imzali;
        }
        public Sonuc ImzalaParafla2(Kullanici kullanan, string evrEvrakNo, EvrakHareket islemHareket, bool eImzaMi, object surecResultDegeri)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.ImzalaParafla2(null, kullanan, evrEvrakNo, islemHareket, eImzaMi, surecResultDegeri);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc GeriGonder2(Kullanici kullanan, EvrakHareket islemHareket)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.GeriGonder2(null, kullanan, islemHareket);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray SayacListele(string kod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRBirim.SayacListele(null, kod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SayacKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRBirim.SayacKaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public byte[] EvrakinDosyasiniGetir(string evrEvrakNo, int siraNo)
        {
            return EVROCR.EvrakinDosyasiniGetir(evrEvrakNo, siraNo);
        }

        public Sonuc EvrakImzaKaldir(Kullanici kullanan, string evrEvrakNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.EvrakImzaKaldir(null, kullanan, evrEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        //****************************************************************************
        //Etiket
        //****************************************************************************
        public Sonuc EtiketKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.EtiketKaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EtiketUstKodDegistir(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.EtiketUstKodDegistir(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EtiketSil(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.EtiketSil(null, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray EtiketListele(Etiket bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVREvrak.EtiketListele(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }
        public Sonuc EvrakEtiketKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.EvrakEtiketKaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc EvrakEtiketSil(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.EvrakEtiketSil(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        //****************************************************************************
        //Etiket
        //****************************************************************************

        public Sonuc EvrakSaklamaSuresiHesaplaveKaydet(Kullanici kullanan, string prEvrakNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRSaklama.SaklamaSuresiHesaplaveKaydet(null, kullanan, prEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakSaklamaPlaniIslemGuncelle(Kullanici kullanan, string prEvrakNoListe, int islem)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRSaklama.IslemGuncelle(null, kullanan, prEvrakNoListe, islem);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakTarihceKaydet(Kullanici kullanan, EvrakTarihce form)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = EVREvrakTarihce.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                sonuc.hataStr = e.ToString();
                sonuc.islemSonuc = false;
            }

            return sonuc;
        }

        public Sonuc ErisimYetkisiDegistir(Kullanici kullanan, EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = EVREvrakHareket.ErisimYetkisiDegistir(null, kullanan, form);
            }
            catch (Exception e)
            {
                sonuc.hataStr = e.ToString();
                sonuc.islemSonuc = false;
            }

            return sonuc;
        }

        public Sonuc EvrakSahiplikBilgisiDegistir(Kullanici kullanan, EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = EVREvrakHareket.SahiplikBilgisiDegistir(null, kullanan, form);
            }
            catch (Exception e)
            {
                sonuc.hataStr = e.ToString();
                sonuc.islemSonuc = false;
            }

            return sonuc;
        }

        public Sonuc SistemBilgisiKaydet(string digerUstVeriler)
        {
            try
            {
                return EVRSistem.Kaydet(null, digerUstVeriler);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public string SistemBilgisiOku()
        {
            try
            {
                return EVRSistem.Oku(null);
            }
            catch (Exception)
            {
                return "";
            }
        }


        public string GizlilikTipiGetir(int gizlilikTip)
        {
            switch (gizlilikTip)
            {
                case (int)ENUMGizlilik.COKGIZLI: return "Çok Gizli";
                case (int)ENUMGizlilik.GIZLI: return "Gizli";
                case (int)ENUMGizlilik.HIZMETEOZEL: return "Hizmete Özel";
                case (int)ENUMGizlilik.KISIYEOZEL: return "Kişiye Özel";
                case (int)ENUMGizlilik.TASNIFDISI: return "Tasnif Dışı";
                default: return "";
            }
        }


        //******************************************
        public Sonuc OrtakDosyalarKaydet(Kullanici kullanan, OrtakDosyalar yk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.Kaydet(null, kullanan, yk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray OrtakDosyalarListele(Kullanici kullanan, OrtakDosyalar yk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVROrtakDosyalar.Listele(null, kullanan, yk, true);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }
        public Sonuc OrtakDosyalarBirimKlasoruKontrol(Kullanici kullanan, OrtakDosyalarBirim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.BirimKlasoruKontrol(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.hataStr = e.ToString();
                donusDeger.islemSonuc = false;
            }

            return donusDeger;
        }
        public ObjectArray OrtakDosyalarDosyaYoluVerUst(Kullanici kullanan, int kod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVROrtakDosyalar.DosyaYoluVerUst(null, kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }
        public ObjectArray OrtakDosyalarDosyaYoluVerAlt(Kullanici kullanan, int kod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVROrtakDosyalar.DosyaYoluVerAlt(null, kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public Sonuc OrtakDosyalarYapistir(Kullanici kullanan, string islemTur, int nereye, string[] islemYapilacakKayitKodlari, string klasorYolu)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.Yapistir(null, kullanan, islemTur, nereye, islemYapilacakKayitKodlari, klasorYolu);
            }
            catch (Exception e)
            {
                donusDeger.hataStr = e.ToString();
                donusDeger.islemSonuc = false;
            }

            return donusDeger;
        }

        public Sonuc OrtakDosyalarSil(Kullanici kullanan, int[] islemYapilacakKayitKodlari, string klasorYolu)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.Sil(null, kullanan, islemYapilacakKayitKodlari, klasorYolu);
            }
            catch (Exception e)
            {
                donusDeger.hataStr = e.ToString();
                donusDeger.islemSonuc = false;
            }

            return donusDeger;
        }
        public Sonuc OrtakDosyalarAdDegistir(Kullanici kullanan, OrtakDosyalar yk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.AdDegistir(null, kullanan, yk);
            }
            catch (Exception e)
            {
                donusDeger.hataStr = e.ToString();
                donusDeger.islemSonuc = false;
            }

            return donusDeger;
        }
        public Sonuc OrtakDosyalarTarihceKaydet(Kullanici kullanan, OrtakDosyalarTarihce yk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.KaydetTarihce(null, kullanan, yk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray OrtakDosyalarTarihceListele(Kullanici kullanan, OrtakDosyalarTarihce yk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVROrtakDosyalar.ListeleTarihce(null, kullanan, yk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }
        public Sonuc OrtakDosyalarYetkiKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.KaydetYetki(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray OrtakDosyalarYetkiListele(Kullanici kullanan, int[] kodlar)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVROrtakDosyalar.ListeleYetki(null, kullanan, kodlar);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public Sonuc OrtakDosyalarNotKaydet(Kullanici kullanan, OrtakDosyalarNot yk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.NotKaydet(null, kullanan, yk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray OrtakDosyalarNotListele(Kullanici kullanan, OrtakDosyalarNot yk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVROrtakDosyalar.NotListele(null, kullanan, yk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }
        public Sonuc OrtakDosyalarNotSil(Kullanici kullanan, OrtakDosyalarNot yk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.NotSil(null, kullanan, yk);
            }
            catch (Exception e)
            {
                donusDeger.hataStr = e.ToString();
                donusDeger.islemSonuc = false;
            }

            return donusDeger;
        }

        public Sonuc OrtakDosyalarSurumKaydet(Kullanici kullanan, OrtakDosyalarSurumler yk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.SurumKaydet(null, kullanan, yk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray OrtakDosyalarSurumListele(Kullanici kullanan, OrtakDosyalarSurumler yk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVROrtakDosyalar.SurumListele(null, kullanan, yk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }
        public Sonuc OrtakDosyalarSurumSil(Kullanici kullanan, OrtakDosyalarSurumler yk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.SurumSil(null, kullanan, yk);
            }
            catch (Exception e)
            {
                donusDeger.hataStr = e.ToString();
                donusDeger.islemSonuc = false;
            }

            return donusDeger;
        }
        public Sonuc OrtakDosyalardanSurumeKaydet(Kullanici kullanan, OrtakDosyalar bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVROrtakDosyalar.SurumuOrtakDosyalaraKaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray OrtakDosyalarSurumDosyaYoluVerAlt(Kullanici kullanan, int kod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVROrtakDosyalar.DosyaYoluVerAlt(null, kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public OrtakDosyalarKullanimAlani OrtakDosyalarKullanimBilgisi(Kullanici kullanan)
        {
            OrtakDosyalarKullanimAlani donusDeger = new OrtakDosyalarKullanimAlani();
            try
            {
                donusDeger = EVROrtakDosyalar.KullanimBilgisi(null, kullanan);
            }
            catch { }

            return donusDeger;
        }
        public bool OrtakDosyalarYetkisiVarmi(Kullanici kullanan, int[] dosyaKodlar, int yetkiTuru)
        {
            bool donusDeger = false;
            try
            {
                donusDeger = EVROrtakDosyalar.YetkisiVarmi(null, kullanan, dosyaKodlar, yetkiTuru);
            }
            catch { }

            return donusDeger;
        }

        public int OrtakDosyalarBaglantiAdresindenKodBul(string adres)
        {
            int donusDeger = 0;
            try
            {
                donusDeger = EVROrtakDosyalar.ListeleBaglantiAdresindenKodBul(null, adres);
            }
            catch { }

            return donusDeger;
        }

        //******************************************

        public Sonuc DuyuruGrubuKaydet(Kullanici kullanan, DuyuruGrubu form)
        {
            VeriErisim ve = new VeriErisim();
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDuyuruGrubu.Kaydet(ve, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc DuyuruGrubuSil(Kullanici kullanan, DuyuruGrubu form)
        {
            VeriErisim ve = new VeriErisim();
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRDuyuruGrubu.Sil(ve, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray DuyuruGrubuListele(Kullanici kullanan, DuyuruGrubu bilgi)
        {
            VeriErisim ve = new VeriErisim();
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDuyuruGrubu.Listele(ve, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }
        public ObjectArray DuyuruGrubuKisiListele(Kullanici kullanan, DuyuruGrubuKisiler bilgi)
        {
            VeriErisim ve = new VeriErisim();
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVRDuyuruGrubu.ListeleKisiler(ve, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public bool BilgiIcinGelenEvrakGeriGonderilebilsinMi(string birimKod)
        {
            bool donusDeger = false;
            try
            {
                donusDeger = EVREvrak.BilgiIcinGelenEvrakGeriGonderilebilsinMi(birimKod);
            }
            catch { }

            return donusDeger;
        }

        public EvrakSicil[] ListeleSicil(Kullanici kullanan, EvrakSicil form)
        {
            try
            {
                return EVREvrak.ListeleSicil(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc EvrakAdresKaydet(Kullanici kullanan, EvrakAdresDefteri form)
        {
            VeriErisim ve = new VeriErisim();
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakAdresDefteri.Kaydet(ve, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakAdresSil(Kullanici kullanan, int kod)
        {
            VeriErisim ve = new VeriErisim();
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakAdresDefteri.Sil(ve, kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray EvrakAdresListele(Kullanici kullanan, EvrakAdresDefteri form, int grupKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVREvrakAdresDefteri.Listele(null, kullanan, form, grupKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public Sonuc EvrakAdresGrubuKaydet(Kullanici kullanan, EvrakAdresGrubuAdi form, ObjectArray adresListesi)
        {
            VeriErisim ve = new VeriErisim();
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakAdresGrubu.Kaydet(ve, kullanan, form, adresListesi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakAdresGrubuSil(Kullanici kullanan, int grupKod)
        {
            VeriErisim ve = new VeriErisim();
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakAdresGrubu.Sil(ve, kullanan, grupKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray EvrakAdresGrubuListele(Kullanici kullanan, EvrakAdresGrubuAdi form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = EVREvrakAdresGrubu.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public EvrakAdresDefteri[] GruptakiAdresleriGetir(Kullanici kullanan, string[] gruplar)
        {
            EvrakAdresDefteri[] donusDeger = null;
            try
            {
                donusDeger = EVREvrakAdresGrubu.ListeleGrupAdresleri(null, kullanan, gruplar);
            }
            catch (Exception e)
            {

            }

            return donusDeger;
        }

        public Sonuc SonraHavaleEtKaydet(Kullanici kullanan, EvrakSonraHavaleEt form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakSonraHavaleEt.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc SonraHavaleEtGonderildiOlarakIsaretle(Kullanici kullanan, string prEvrakNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakSonraHavaleEt.GonderildiOlarakIsaretle(null, kullanan, prEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public EvrakSonraHavaleEt[] SonraHavaleEtListele(Kullanici kullanan, EvrakSonraHavaleEt form)
        {
            try
            {
                return EVREvrakSonraHavaleEt.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc SiraNoDegistir(string primaryKey, int siraNo1, int siraNo2, ENUMSiraNoDegistirTablo tabloAd)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrak.SiraNoDegistir(primaryKey, siraNo1, siraNo2, tabloAd);
                if (donusDeger.islemSonuc && tabloAd == ENUMSiraNoDegistirTablo.EVREVRAKDOSYA)
                {
                    Sonuc donusDeger2 = EVREvrak.SiraNoDegistir(primaryKey, siraNo1, siraNo2, ENUMSiraNoDegistirTablo.INFOCOMDOSYAIMZA);
                }
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Birim[] BirimListele(string birimKodListesi)
        {
            try
            {
                return EVRBirim.Listele(null, birimKodListesi);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Birim[] BirimListeleYetkili(Kullanici kullanan, params int[] kullaniciTipleri)
        {
            try
            {
                string kullaniciYetkiliBirimler = kullaniciTipleri == null || kullaniciTipleri.Length == 0 ? TNS.EVR.Yetki.KullaniciEvrakYetkiliBirimleri(kullanan, ENUMOkumaYazma.OKUMA) : TNS.EVR.Yetki.KullaniciEvrakYetkiliBirimleri(kullanan, kullaniciTipleri);
                return EVRBirim.Listele(null, kullaniciYetkiliBirimler);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc HavaleGrupKaydet(Kullanici kullanan, HavaleGrup form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHavaleGrup.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc HavaleGrupSil(Kullanici kullanan, HavaleGrup form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHavaleGrup.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public HavaleGrup[] HavaleGrupListele(Kullanici kullanan, HavaleGrup form)
        {
            try
            {
                return EVRHavaleGrup.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public HavaleGrup HavaleGrupOku(Kullanici kullanan, HavaleGrup form)
        {
            try
            {
                return EVRHavaleGrup.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc HavaleGrupVarsayilanYap(Kullanici kullanan, HavaleGrup form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHavaleGrup.VarsayilanYap(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc HavaleGrupSayacGuncelle(Kullanici kullanan, HavaleGrupDetay[] detaylar)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHavaleGrup.SayacGuncelle(null, kullanan, detaylar);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc HavaleGrupPaylasimDurumuBelirle(Kullanici kullanan, HavaleGrup form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRHavaleGrup.PaylasimDurumuBelirle(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc EvrakHareketDegistir(Kullanici kullanan, EvrakHareket[] form)
        {
            try
            {
                return EVREvrakHareket.HareketDegistir(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public EvrakMeta[] EvrakMetaListele(Kullanici kullanan, EvrakMeta form)
        {
            try
            {
                return EVREvrakMeta.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc EvrakMetaKaydet(Kullanici kullanan, EvrakSinif evrak, EvrakMeta[] formList)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVREvrakMeta.Kaydet(null, kullanan, evrak, formList);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }


        public Sonuc RolKaydet(Kullanici kullanan, Rol form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRRol.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc RolSil(Kullanici kullanan, Rol form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRRol.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Rol[] RolListele(Kullanici kullanan, Rol form)
        {
            try
            {
                return EVRRol.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Rol RolOku(Kullanici kullanan, Rol form)
        {
            try
            {
                return EVRRol.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int RolKullaniciYetkiDegerBul(ENUMYetkiler yetki, string tcKimlikNo)
        {
            try
            {
                return EVRRol.KullaniciYetkiDegerBul(null, yetki, tcKimlikNo);
            }
            catch (Exception e)
            {
                return 0;
            }
        }


        public SaklamaPlani[] SaklamaPlaniListele(Kullanici kullanan, SaklamaPlani form)
        {
            try
            {
                return EVRSaklamaPlani.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc SaklamaPlaniKaydet(Kullanici kullanan, SaklamaPlani form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRSaklamaPlani.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc SaklamaPlaniSil(Kullanici kullanan, SaklamaPlani form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRSaklamaPlani.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public GuncellemeBilgi[] GuncellemeBilgiListele(Kullanici kullanan, string kod)
        {
            try
            {
                return EVRVeritabani.Listele(null, kullanan, kod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc GuncellemeBilgiKaydet(Kullanici kullanan, string kod, string sql)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRVeritabani.Kaydet(null, kullanan, kod, sql);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc GuncellemeBilgiSil(Kullanici kullanan, string kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRVeritabani.Sil(null, kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc VeritabaniIslemleriSorgula(int islem)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                if (islem == 1)
                    donusDeger = EVRVeritabani.EvrakHareketSonDurumTabloMu(null);
                else if (islem == 2)
                    donusDeger = EVRVeritabani.DagitimHareketleriHavaleMi(null);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc VeritabaniIslemleriSorguCalistir(int islem)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                if (islem == 1)
                    donusDeger = EVRVeritabani.EvrakHareketSonDurumTabloYap(null);
                else if (islem == 2)
                    donusDeger = EVRVeritabani.DagitimHavaleHarekeleriniDegistir(null);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public VeritabaniIndex[] VeritabaniIndexListele(VeritabaniIndex form, string baglantiSatiri)
        {
            try
            {
                return EVRVeritabani.IndexListele(null, form, baglantiSatiri);
            }
            catch (Exception e)
            {
                return new VeritabaniIndex[1] { new VeritabaniIndex() { objectId = -2, tabloAd = e.Message } };
            }
        }
        public Sonuc VeritabaniIndexIslem(VeritabaniIndex[] liste, ENUMVeritabaniIndexIslem islem, string baglantiSatiri)
        {
            try
            {
                return EVRVeritabani.IndexIslem(null, liste, islem, baglantiSatiri);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public Hashtable VeritabaniBaglantiSatiriListele()
        {
            Hashtable list = new Hashtable();
            try
            {
                string p_sifrele = System.Configuration.ConfigurationManager.AppSettings.Get("BaglantiSatiri.Sifrele") + "";

                foreach (string item in System.Configuration.ConfigurationManager.AppSettings.AllKeys)
                {
                    if (item.ToLower().IndexOf("x") == -1 && item.IndexOf(".") == -1 && item.StartsWith("BaglantiSatiri"))
                    {
                        string key = "";
                        string obj = item;

                        string value = System.Configuration.ConfigurationManager.AppSettings.Get(item) + "";

                        if (OrtakFonksiyonlar.ConvertToInt(p_sifrele, 0) > 0 && !string.IsNullOrEmpty(value))
                        {
                            if (p_sifrele == "2")
                                value = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(value);
                            else
                                value = OrtakFonksiyonlar.SifreCoz(value);
                        }

                        key = value.Substring(value.IndexOf("Catalog=") + 8).Split(';')[0];

                        if (!list.ContainsKey(key))
                            list.Add(key, obj);
                    }
                }
            }
            catch (Exception ex)
            {
                list.Add("*", ex.Message);
            }
            return list;
        }

        public Sonuc KepHesapKaydet(Kullanici kullanan, KepHesap form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRKepHesap.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc KepHesapSil(Kullanici kullanan, KepHesap form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRKepHesap.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public KepHesap[] KepHesapListele(Kullanici kullanan, KepHesap form)
        {
            try
            {
                return EVRKepHesap.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public KepHesap KepHesapOku(Kullanici kullanan, KepHesap form)
        {
            try
            {
                return EVRKepHesap.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc KepHesapVarsayilanYap(Kullanici kullanan, KepHesap form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRKepHesap.VarsayilanYap(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }


        public Sonuc KepEvrakKaydet(Kullanici kullanan, KepEvrak form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRKepEvrak.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc KepEvrakSil(Kullanici kullanan, KepEvrak form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRKepEvrak.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public KepEvrak[] KepEvrakListele(Kullanici kullanan, KepEvrak form)
        {
            try
            {
                return EVRKepEvrak.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public KepEvrak KepEvrakOku(Kullanici kullanan, KepEvrak form)
        {
            try
            {
                return EVRKepEvrak.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc KepAdresKaydet(Kullanici kullanan, KepAdres form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRKepAdres.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc KepAdresSil(Kullanici kullanan, KepAdres form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = EVRKepAdres.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public KepAdres[] KepAdresListele(Kullanici kullanan, KepAdres form)
        {
            try
            {
                return EVRKepAdres.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int EvrakTipGetir(string evrEvrakNo)
        {
            return EVRGenel.EvrakTipGetir(evrEvrakNo);
        }

        public Sonuc BirimDagitimYetkiKaydet(Kullanici kullanan, BirimDagitimYetki[] formList, string evrakBirimKod)
        {
            try
            {
                return EVRBirimDagitimYetki.Kaydet(null, kullanan, formList, evrakBirimKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public BirimDagitimYetki[] BirimDagitimYetkiListele(Kullanici kullanan, BirimDagitimYetki form)
        {
            try
            {
                return EVRBirimDagitimYetki.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Sonuc BirimHavaleYetkiKaydet(Kullanici kullanan, BirimHavaleYetki[] formList, string evrakBirimKod)
        {
            try
            {
                return EVRBirimHavaleYetki.Kaydet(null, kullanan, formList, evrakBirimKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public BirimHavaleYetki[] BirimHavaleYetkiListele(Kullanici kullanan, BirimHavaleYetki form)
        {
            try
            {
                return EVRBirimHavaleYetki.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool EditorKullaniliyorMu(Kullanici kullanan, string evrEvrakNo)
        {
            try
            {
                return EVREvrak.EditorKullaniliyorMu(null, kullanan, evrEvrakNo);
            }
            catch { }
            return false;
        }

        public Sonuc EvrakInfocomDosyaImzala(Kullanici kullanan, InfocomDosyaImza form)
        {
            try
            {
                return EVRInfocomDosyaImza.Kaydet(ve, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc
                {
                    islemSonuc = false,
                    hataStr = e.ToString()
                };
            }
        }

        public ObjectArray EvrakInfocomDosyaListele(Kullanici kullanan, InfocomDosyaImza form)
        {
            try
            {
                return EVRInfocomDosyaImza.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        public Sonuc EvrakInfocomApiTarihce(InfocomApiTarihce form)
        {
            try
            {
                return EVRInfocomApiTarihce.Kaydet(null, form);
            }
            catch (Exception e)
            {
                return new Sonuc
                {
                    islemSonuc = false,
                    hataStr = e.ToString()
                };
            }
        }

    }
}

namespace TNS
{
    /// <summary>
    /// Yetki tipi bilgileriyle ilgili yordamları tutan sınıf.
    /// </summary>
    public class KYMListe : YetkiTipiListesi, YetkiTipiAgaci
    {
        /// <summary>
        /// Evrak birimlerini ListeSatir nesne dizisi olarak veritabanından getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tip">Tip bilgisini içeren nesne</param>
        /// <returns>ListeSatir nesne dizisi olarak döndürülür.</returns>
        public static ListeSatir[] EvrakListesi(Kullanici kullanan, string tip)
        {
            TNS.EVR.Birim bilgi = new TNS.EVR.Birim();

            ObjectArray kl = EVRBirim.Listele(null, bilgi, false);
            ListeSatir[] donusDeger = new ListeSatir[kl.ObjeSayisi];

            int i = 0;
            foreach (TNS.EVR.Birim y in kl.objeler)
            {
                donusDeger[i++] = new ListeSatir(y.kod, y.kod + " - " + y.ad);
            }
            return donusDeger;
        }

        /// <summary>
        /// Yetki listesini ListeSatir nesne dizisi olarak getiren yordam.
        /// </summary>
        /// <returns>ListeSatir nesne dizisi olarak döndürülür.</returns>
        public static ListeSatir[] YetkiListesiEvrak()
        {
            ListeSatir[] lv = new ListeSatir[3];
            lv[0] = new ListeSatir("1", "Evrak Yetkilisi");
            lv[1] = new ListeSatir("2", "Evrak Kullanıcısı");
            lv[2] = new ListeSatir("3", "Evrak Memuru");

            return lv;
        }

        /// <summary>
        /// Evrak yetki listesini ListeSatir nesne dizisi olarak getiren yordam.
        /// </summary>
        /// <returns>ListeSatir nesne dizisi olarak döndürülür.</returns>
        public static ListeSatir[] YetkiListesiEvrakYetki()
        {
            ListeSatir[] lv = new ListeSatir[1];
            lv[0] = new ListeSatir("1", "İmza yetkisi var.");

            return lv;
        }

        /// <summary>
        /// Yetki listesini ListeSatir nesne dizisi olarak getiren yordam.
        /// </summary>
        /// <returns>ListeSatir nesne dizisi olarak döndürülür.</returns>
        public static ListeSatir[] YetkiListesiDosyaTasnif()
        {
            ListeSatir[] lv = new ListeSatir[3];
            lv[0] = new ListeSatir("1", "Evrak Yetkilisi");
            lv[1] = new ListeSatir("2", "Evrak Kullanıcısı");
            lv[2] = new ListeSatir("3", "Evrak Memuru");

            return lv;
        }

        /// <summary>
        /// Yetki listesini ListeSatir nesne dizisi olarak getiren yordam.
        /// </summary>
        /// <returns>ListeSatir nesne dizisi olarak döndürülür.</returns>
        public static ListeSatir[] YetkiListesiGizlilik()
        {
            ENUMTamimlamaFormTur tanimlamaFormTur = ENUMTamimlamaFormTur.EVRGIZLILIK;

            ObjectArray list = EVRTamimlama.Listele(null, new Kullanici(), new Tanimlama(), tanimlamaFormTur);
            ListeSatir[] lv = new ListeSatir[list.objeler.Count];

            int sayac = 0;
            foreach (Tanimlama tanim in list.objeler)
            {
                if (tanim.derece == 0) continue;
                lv[sayac] = new ListeSatir(tanim.kod.ToString(), tanim.ad);
                sayac++;
            }

            return lv;
        }

        /// <summary>
        /// Parametre olark verilen bilgilere göre yetki tipi elemanlarını ListeSatir nesne dizisi olarak getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl bilgisini içeren nesne</param>
        /// <param name="ekParamCagri">Çağrı bilgisini içeren nesne</param>
        /// <param name="ekParamTanim">Tanim bilgisini içeren nesne</param>
        /// <returns>ListeSatir nesne dizisi olarak döndürülür.</returns>
        public ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim.Equals("9"))
                return YetkiListesiEvrak();
            else if (ekParamTanim.Equals("11"))
                return YetkiListesiEvrakYetki();
            else if (ekParamTanim.Equals("13"))
                return YetkiListesiGizlilik();
            throw new NotImplementedException("Bilinmeyen parametre!");
        }

        public ListeSatir[] YetkiTipiAgacElemanlari(Kullanici kullanan, int yil, string ustKod, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim == "10")
                return BirimAgacDoldur(kullanan, yil, ustKod, ekParamCagri, ekParamTanim);
            else if (ekParamTanim == "12")
                return DosyaTasnifAgacDoldur(kullanan, yil, ustKod, ekParamCagri, ekParamTanim);

            throw new NotImplementedException("Bilinmeyen Agaç Elemanı!");
        }

        public ListeSatir[] BirimAgacDoldur(Kullanici kullanan, int yil, string ustKod, string ekParamCagri, string ekParamTanim)
        {

            TNS.EVR.Birim bilgi = new TNS.EVR.Birim();

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) && ustKod == "")
                ustKod = "%";

            if (string.IsNullOrEmpty(ustKod))
            {
                foreach (System.Collections.DictionaryEntry de in kullanan.yetkiTipiDegerleri)
                {
                    string ytAd = (string)de.Key;
                    if (ytAd.Equals(EvrakI.EVRI001_1))
                    {
                        string d = TNS.HashTableArrayList.ArrayListToStr(de.Value);
                        if (d.Trim() != "")
                        {
                            string[] dler = d.Trim().Split(';');
                            foreach (string dd in dler)
                            {
                                string kokBirim = EVRBirim.EvrakKokBirimGetir(null, dd.Trim());
                                if (ustKod.IndexOf(kokBirim) == -1)
                                {
                                    if (ustKod != "") ustKod += ";";
                                    ustKod += kokBirim;
                                }
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(ustKod))
                    ustKod = "999";
            }
            else
            {
                string[] kirilimlar = ustKod.Split('.');
                ustKod += "%";
                bilgi.seviye = kirilimlar.Length + 1;
            }

            bilgi.kod = ustKod;

            ObjectArray kl = EVRBirim.Listele(null, bilgi, false);

            if (kl.ObjeSayisi == 0)
            {
                for (int index = 0; index < 4; index++)
                {
                    bilgi.seviye++;
                    kl = EVRBirim.Listele(null, bilgi, false);
                    if (kl.ObjeSayisi > 0)
                        break;
                }
            }

            ListeSatir[] donusDeger = new ListeSatir[kl.ObjeSayisi];

            int i = 0;
            foreach (TNS.EVR.Birim y in kl.objeler)
            {
                donusDeger[i] = new ListeSatir(y.kod, y.kod + " - " + y.ad);
                if (!y.detay) donusDeger[i].cocuguVar = true;
                i++;
            }
            return donusDeger;
        }

        public ListeSatir[] DosyaTasnifAgacDoldur(Kullanici kullanan, int yil, string ustKod, string ekParamCagri, string ekParamTanim)
        {
            TNS.EVR.DosyaPlani bilgi = new TNS.EVR.DosyaPlani();

            if (string.IsNullOrEmpty(ustKod))
            {
                foreach (System.Collections.DictionaryEntry de in kullanan.yetkiTipiDegerleri)
                {
                    string ytAd = (string)de.Key;
                    if (ytAd.Equals(EvrakI.EVRI001_5))
                    {
                        string d = TNS.HashTableArrayList.ArrayListToStr(de.Value);
                        if (d.Trim() != "")
                        {
                            string[] dler = d.Trim().Split(';');
                            foreach (string dd in dler)
                            {
                                string kokBirim = EVRBirim.EvrakKokBirimGetir(null, dd.Trim());
                                if (ustKod.IndexOf(kokBirim) == -1)
                                {
                                    if (ustKod != "") ustKod += ";";
                                    ustKod += kokBirim;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                string[] kirilimlar = ustKod.Split('.');
                ustKod += "%";
                bilgi.seviye = kirilimlar.Length + 1;
            }

            bilgi.kod = ustKod;
            if (bilgi.seviye == 0) bilgi.seviye = 1;

            ObjectArray kl = EVRDosyaPlani.Listele(null, bilgi);

            if (kl.ObjeSayisi == 0)
            {
                for (int index = 0; index < 4; index++)
                {
                    bilgi.seviye++;
                    kl = EVRDosyaPlani.Listele(null, bilgi);
                    if (kl.ObjeSayisi > 0)
                        break;
                }
            }

            ListeSatir[] donusDeger = new ListeSatir[kl.ObjeSayisi];

            int i = 0;
            foreach (TNS.EVR.DosyaPlani y in kl.objeler)
            {
                donusDeger[i] = new ListeSatir(y.kod, y.kod + " - " + y.ad);
                if (!y.detay) donusDeger[i].cocuguVar = true;
                i++;
            }
            return donusDeger;
        }

        public string YetkiTipiKodAd(Kullanici kullanan, int yil, string kod, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim == "10")
                return BirimAgacYetkiTipi(kullanan, yil, kod, ekParamCagri, ekParamTanim);
            else if (ekParamTanim == "12")
                return DosyaTasnifYetkiTipi(kullanan, yil, kod, ekParamCagri, ekParamTanim);

            throw new NotImplementedException("Bilinmeyen yetki tipi!");
        }

        public string BirimAgacYetkiTipi(Kullanici kullanan, int yil, string kod, string ekParamCagri, string ekParamTanim)
        {
            TNS.EVR.Birim bilgi = new TNS.EVR.Birim();
            bilgi.kod = kod;
            bilgi = EVRBirim.Oku(null, bilgi);
            return bilgi.kod + " - " + bilgi.ad;
        }
        public string DosyaTasnifYetkiTipi(Kullanici kullanan, int yil, string kod, string ekParamCagri, string ekParamTanim)
        {
            TNS.EVR.DosyaPlani bilgi = new TNS.EVR.DosyaPlani();
            bilgi.kod = kod;
            bilgi = EVRDosyaPlani.Oku(null, bilgi);
            return bilgi.kod + " - " + bilgi.ad;
        }
    }
}

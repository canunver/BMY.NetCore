using System;
using System.Collections;
using TNS.BKD;
using TNS.KYM;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    /// <summary>
    /// Tarim formu sunucu yordamlarının fabrika sınıfı. Gerçekleştirilen yordamlar, ilgili sınıfdaki işlem yapan yordamını çağırırlar.
    /// </summary>
    [Serializable()]
    public class TRMServis : MarshalByRefObject, ITRMServis, IDisposable
    {
        public static bool tabloDegisimYap = true;
        /// <summary>f
        /// Kullanıcı sunucu yordamlarına erişimi sağlayan değişken.
        /// </summary>
        //public static IKYMServis servisKYM = null;

        /// <summary>
        /// Kullanıcı yönetimi sunucu erişimi ve uzay erişimi nesneleri oluşturulur.
        /// </summary>
        public TRMServis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.TRM.IsKurallari.TRMTabloDegisiklikleri.Kaydet();
                }
            }



            //if (servisKYM == null)
            //{
            //servisKYM = TNS.KYM.Arac.Tanimla();

            //TKDK Servis testi
            //TRMServisC.tr.gov.tkdk.tkdkservice.TKDKBilgiService tkdkServis = new TRMServisC.tr.gov.tkdk.tkdkservice.TKDKBilgiService();
            //TRMServisC.tr.gov.tkdk.tkdkservice.ResultOfTRGMModelEZGc0uCv rst = tkdkServis.GetTKDKBilgiTest(Convert.ToInt64("64207235290"), true, "TKDK743-BB42-4A7D-81CD-4B5051489985");
            //string[] msg = rst.Messages;
            //TRMServisC.tr.gov.tkdk.tkdkservice.TRGMModel model = rst.Data;
            //bool durum = rst.Success;
            //bool durumSpc = rst.SuccessSpecified;
            //}
        }

        TNS.Veri.VeriErisim ve = null;
        public void Dispose()
        {
            if (ve != null)
                ve.VeriErisimKapat(false);
        }

        /// <summary>
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MalzemeGrubuKaydet(Kullanici kullanan, MalzemeGrubu bilgi)
        {
            try
            {
                return new TRMMalzemeGrubu().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme grubu bilgisini veritabanından okur 
        /// ve MalzemeGrubu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme grubu listesini MalzemeGrubu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray MalzemeGrubuListele(Kullanici kullanan, MalzemeGrubu kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMMalzemeGrubu().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MalzemeGrubuSil(Kullanici kullanan, MalzemeGrubu bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMMalzemeGrubu().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc ButceKaydet(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMButce().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray ButceListele(Kullanici kullanan, Butce kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMButce().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /**********Makine/Ekipman****************************/
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //public Sonuc MalzemeKaydet(Kullanici kullanan, Malzeme bilgi)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new TRMMalzeme().Kaydet(kullanan, bilgi);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray MalzemeListele(Kullanici kullanan, Malzeme kriter)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new TRMMalzeme().Listele(kullanan, kriter);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //public Sonuc MalzemeSil(Kullanici kullanan, Malzeme bilgi)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new TRMMalzeme().Sil(kullanan, bilgi);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        /**********Makine/Ekipman****************************/
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MakinaTanimKaydet(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakinaTanim().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MalzemeAnaKodGuncelle(Kullanici kullanan, int malzemeKod, int anaMalzemeKod, int kendiAnaKodu)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakinaTanim().MalzemeAnaKodGuncelle(kullanan, malzemeKod, anaMalzemeKod, kendiAnaKodu);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public bool AnaMalzemeKodVarMi(Kullanici kullanan, int anaMalzemeKod)
        {
            bool donusDeger = false;
            try
            {
                donusDeger = new TRMHibeMakinaTanim().AnaMalzemeKodVarMi(kullanan, anaMalzemeKod);
                return donusDeger;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool HibeTuruYilMalzemeGrubunaGoreBelgeTanimKaydiVarmi(Kullanici kullanan, BasvuruBelgeTanim bilgi)
        {
            bool donusDeger = false;
            try
            {
                donusDeger = new TRMBasvuruBelgeTanim().HibeTuruYilMalzemeGrubunaGoreBelgeTanimKaydiVarmi(kullanan, bilgi);
                return donusDeger;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray MakinaTanimListele(Kullanici kullanan, Malzeme kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeMakinaTanim().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="malzemeGrupKod">The malzeme grup kod.</param>
        /// <param name="malzemeKod">The malzeme kod.</param>
        /// <returns>
        /// İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne
        /// </returns>
        public ObjectArray MakinaTanimOzellikListele(Kullanici kullanan, int malzemeGrupKod, int malzemeKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeMakinaTanim().ListeleMakinaTanimOzellik(kullanan, malzemeGrupKod, malzemeKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="malzemeGrupKod">The malzeme grup kod.</param>
        /// <param name="malzemeKod">The malzeme kod.</param>
        /// <returns>
        /// İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne
        /// </returns>
        public ObjectArray MakinaTanimVeOzellikleriniListele(Kullanici kullanan, Malzeme m)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeMakinaTanim().ListeleMakinaTanimVeOzellikleri(kullanan, m);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MakinaTanimSil(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakinaTanim().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MakinaOzelligiKaydet(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakineOzelligi().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray MakinaOzelligiListele(Kullanici kullanan, Malzeme kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeMakineOzelligi().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MakinaOzelligiSil(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakineOzelligi().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, hibe makine bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeMakineKaydet(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakine().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, hibe makine sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeMakineKaydetSozlesme(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakine().KaydetSozlesme(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, hibe makine sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeMakineBasvuruyuIslemeAl(Kullanici kullanan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakine().BasvuruyuIslemeAl(kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, hibe makine proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeMakineKaydetProjeNo(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakine().KaydetProjeNo(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, hibe Sulama sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeMakinaKaydetEksikHibeMiktar(Kullanici kullanan, HibeMakineDetay bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakine().KaydetEksikHibeMiktar(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, hibe Sulama sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeSulamaKaydetEksikHibeMiktar(Kullanici kullanan, HibeSulamaDetay bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().KaydetEksikHibeMiktar(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, hibe makine red bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeMakineKaydetReddedildi(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakine().KaydetReddedildi(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeMakineYatirimciDegistir(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakine().YatirimciDegistir(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimYatirimciDegistir(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().YatirimciDegistir(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeSulamaYatirimciDegistir(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().YatirimciDegistir(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimBasvuruNoDegistir(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().BasvuruNoDegistir(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        //Grup
        public Sonuc HibeMakineGrupKaydet(Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                if (bilgi.tipi == (int)ENUMHibeTur.MAKINE)
                    donusDeger = new TRMHibeMakine().GrupKaydet(kullanan, bilgi);
                else
                    donusDeger = new TRMHibeSulama().GrupKaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeMakineGrupCikart(Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                if (bilgi.tipi == (int)ENUMHibeTur.MAKINE)
                    donusDeger = new TRMHibeMakine().GrupCikart(kullanan, bilgi);
                else
                    donusDeger = new TRMHibeSulama().GrupCikart(kullanan, bilgi);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeMakineGrupOdemeyeGonder(Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            try
            {
                if (bilgi.tipi == (int)ENUMHibeTur.MAKINE)
                    return TRMHibeMakine.GrupOdemeyeGonder(null, kullanan, bilgi);
                else
                    return TRMHibeSulama.GrupOdemeyeGonder(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeMakineGrupOdemeBaslat(Kullanici kullanan, HibeMakineSulamaGrup[] bilgi)
        {
            try
            {
                if (bilgi.Length == 0)
                    return null;
                HibeMakineSulamaGrup b = new HibeMakineSulamaGrup();
                b.odemeKod = bilgi[0].odemeKod;
                b.tipi = bilgi[0].tipi;
                if (b.tipi == (int)ENUMHibeTur.MAKINE)
                    return TRMHibeMakine.GrupOdemeBaslat(null, kullanan, bilgi, b.odemeKod);
                else if (b.tipi == (int)ENUMHibeTur.SULAMA)
                    return TRMHibeSulama.GrupOdemeBaslat(null, kullanan, bilgi, b.odemeKod);
                else//(int)ENUMHibeTur.EKONOMIKYATIRIM
                    return TRMHibeYatirimHakedis.GrupOdemeBaslat(null, kullanan, bilgi, b.odemeKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeMakineGrupOdendi(Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            try
            {
                if (bilgi.tipi == (int)ENUMHibeTur.MAKINE)
                    return TRMHibeMakine.GrupOdendi(null, kullanan, bilgi.odemeKod, (int)ENUMHibeOdemeTuru.ODENDI);
                else if (bilgi.tipi == (int)ENUMHibeTur.SULAMA)
                    return TRMHibeSulama.GrupOdendi(null, kullanan, bilgi.odemeKod, (int)ENUMHibeOdemeTuru.ODENDI);
                else//(int)ENUMHibeTur.EKONOMIKYATIRIM
                    return TRMHibeYatirimHakedis.GrupOdendi(null, kullanan, bilgi.odemeKod, (int)ENUMHibeYatirimHakedisDurum.ODENDI);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeMakineGrupBankaGonder(Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            try
            {
                if (bilgi.tipi == (int)ENUMHibeTur.MAKINE)
                    return TRMHibeMakine.GrupOdendi(null, kullanan, bilgi.odemeKod, (int)ENUMHibeOdemeTuru.BANKAGONDER);
                else if (bilgi.tipi == (int)ENUMHibeTur.SULAMA)
                    return TRMHibeSulama.GrupOdendi(null, kullanan, bilgi.odemeKod, (int)ENUMHibeOdemeTuru.BANKAGONDER);
                else//(int)ENUMHibeTur.EKONOMIKYATIRIM
                    return TRMHibeYatirimHakedis.GrupOdendi(null, kullanan, bilgi.odemeKod, (int)ENUMHibeYatirimHakedisDurum.BANKAYAGONDERILDI);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        public Sonuc HibeMakineGrupGeriAl(Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            try
            {
                if (bilgi.tipi == (int)ENUMHibeTur.MAKINE)
                    return TRMHibeMakine.GrupGeriAl(null, kullanan, bilgi);
                else
                    return TRMHibeSulama.GrupGeriAl(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        //Grup
        public Sonuc HibeMakineSulamaGrupKaydet(Kullanici kullanan, HibeMakineSulamaGrup g)
        {
            try
            {
                return TRMHibeMakineSulamaGrup.Kaydet(null, kullanan, g);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public Sonuc HibeMakineSulamaGrupKontrol(Kullanici kullanan, HibeMakineSulamaGrup g)
        {
            try
            {
                return TRMHibeMakineSulamaGrup.GuncelleKontrol(null, kullanan, g);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public HibeMakineSulamaGrup[] HibeMakineSulamaGrupListele(Kullanici kullanan, HibeMakineSulamaGrup g, params int[] ekDurumlar)
        {
            try
            {
                return TRMHibeMakineSulamaGrup.Listele(null, kullanan, g, ekDurumlar);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public HibeMakineSulamaGrup[] HibeMakineSulamaListeleGrupBazliToplam(Kullanici kullanan, int etapNo, int durum, int tipi, int tur = -1, int odemeKod = -1, bool kontrolEdilen = false, TNSDateTime baslamaTarih = null, TNSDateTime bitisTarihi = null)
        {
            try
            {
                if (tipi == (int)ENUMHibeTur.MAKINE)
                    return TRMHibeMakine.HibeMakineListeleGrupBazliToplam(null, kullanan, etapNo, durum, tur);
                else if (tipi == (int)ENUMHibeTur.SULAMA)
                    return TRMHibeSulama.HibeSulamaListeleGrupBazliToplam(null, kullanan, etapNo, durum, tur);
                else
                    return TRMHibeYatirimHakedis.HibeYatirimHakedisListeleOdeme(null, kullanan, etapNo, durum, tur, odemeKod, kontrolEdilen, baslamaTarih, bitisTarihi);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc HibeMakineMalzemeDegistir(Kullanici kullanan, HibeMakineDetay bilgi)
        {
            try
            {
                return new TRMHibeMakine().MalzemeDegistir(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeMakineKaydetBasvuruYapildi(Kullanici kullanan, int hibeKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakine().KaydetBasvuruYapildi(kullanan, hibeKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, hibe makine yerinde tespit bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeMakineKaydetYerindeTespit(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakine().KaydetYerindeTespit(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, hibe makine fatura bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeMakineKaydetFatura(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakine().KaydetFatura(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe makine bilgisini veritabanından okur 
        /// ve HibeMakine nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini HibeMakine nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HibeMakineListele(Kullanici kullanan, HibeMakine kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeMakine().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeMakineSorgula(Kullanici kullanan, HibeMakine kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc)
        {
            try
            {
                return TRMHibeMakine.Sorgula(kullanan, kriter, ilkKayit, kayitSay, sortKolon, asc);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public int HibeMakineBasvuruSayiDondur(Kullanici kullanan, HibeMakine kriter)
        {
            try
            {
                return TRMHibeMakine.HibeMakineBasvuruSayiDondur(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe makine tarihçe bilgisini veritabanından okur 
        /// ve HibeMakineTarihce nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, listeyi HibeMakineTarihce nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HibeMakineTarihceListele(Kullanici kullanan, HibeMakine kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeMakine().ListeleTarihce(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeMakineRefFiyatTarihceListele(Kullanici kullanan, HibeMakineRefFiyat kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeMakineRefFiyat().ListeleTarihce(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeMakineIlBazindaIcmal(Kullanici kullanan, HibeMakine kriter, bool ilListele)
        {
            try
            {
                return new TRMRapor().HibeMakineIlBazindaIcmal(kullanan, kriter, ilListele);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }
        public ArrayList IlListesi(Kullanici kullanan, int ilTur)
        {
            try
            {
                return new TRMRapor().IlListesi(kullanan, ilTur);
            }
            catch
            {
                return new ArrayList();
            }
        }
        public ObjectArray HibeMakineOdemeListele(Kullanici kullanan, HibeMakineSulamaOdeme kriter)
        {
            try
            {
                return new TRMHibeMakineOdeme().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public Sonuc HibeMakineOdemeSil(Kullanici kullanan, HibeMakineSulamaOdeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakineOdeme().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeMakineOdemeKaydet(Kullanici kullanan, HibeMakineSulamaOdeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakineOdeme().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeMakineOdemeKaydetOdemeEmriNo(Kullanici kullanan, HibeMakineSulamaOdeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakineOdeme().KaydetOdemeEmriNo(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeMakineOdemeKaydetOdendi(Kullanici kullanan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakineOdeme().KaydetOdendi(kullanan, kod, (int)ENUMHibeOdemeTuru.ODENDI);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeMakineOdemeKaydetBankaGonder(Kullanici kullanan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakineOdeme().KaydetOdendi(kullanan, kod, (int)ENUMHibeOdemeTuru.BANKAGONDER);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public double HibeMakineOdemeMiktarGetir(Kullanici kullanan, int tur)
        {
            try
            {
                return TRMHibeMakine.HibeMakineOdemeTutarGetir(kullanan, tur);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc HibeMakineYonetimKaydet(Kullanici kullanan, Yonetim form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeMakineYonetim.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc HibeMakineYonetimSil(Kullanici kullanan, Yonetim form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeMakineYonetim.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Yonetim[] HibeMakineYonetimListele(Kullanici kullanan, Yonetim form)
        {
            try
            {
                return TRMHibeMakineYonetim.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Yonetim HibeMakineYonetimOku(Kullanici kullanan, Yonetim form)
        {
            try
            {
                return TRMHibeMakineYonetim.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int HibeMakineAktifEtapNo()
        {
            try
            {
                return TRMHibeMakineYonetim.HibeMakineAktifEtapNo(null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public double HibeMakineAktifUstLimit(int hukukiStatu)
        {
            try
            {
                return TRMHibeMakineYonetim.HibeMakineAktifUstLimit(null, hukukiStatu);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string HibeMakineOnlineKapandiMi()
        {
            try
            {
                return TRMHibeMakineYonetim.HibeMakineOnlineKapandiMi(null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Arguman olarak verilen, HibeMakineRefFiyat bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeMakineRefFiyatKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakineRefFiyat().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, HibeMakineRefFiyat bilgisini veritabanından okur 
        /// ve HibeMakineRefFiyat nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, HibeMakineRefFiyat listesini HibeMakineRefFiyat nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HibeMakineRefFiyatListele(Kullanici kullanan, HibeMakineRefFiyat kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeMakineRefFiyat().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeMakineRefSonrakiYilaAktar(Kullanici kullanan, HibeMakineRefFiyat kriter, double ufeOran)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakineRefFiyat().SonrakiYilaAktar(kullanan, kriter, ufeOran);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimRefFiyatKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimRefFiyat().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, HibeYatirimRefFiyat bilgisini veritabanından okur 
        /// ve HibeYatirimRefFiyat nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, HibeYatirimRefFiyat listesini HibeYatirimRefFiyat nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HibeYatirimRefFiyatListele(Kullanici kullanan, HibeYatirimRefFiyat kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirimRefFiyat().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimRefSonrakiYilaAktar(Kullanici kullanan, HibeYatirimRefFiyat kriter, double ufeOran)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimRefFiyat().SonrakiYilaAktar(kullanan, kriter, ufeOran);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public void HibeMakineHibeTutarHesapla(int hukukiStatu, decimal referansTutar, decimal proformaTutar, decimal malzemeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, decimal hibeOran, int adet)
        {
            TRMHibeMakine.HibeTutarHesapla(hukukiStatu, referansTutar, proformaTutar, malzemeTutar, ref hibeEsasTutar, ref toplamTutar, ref ayniKatki, ref hibeTutar, hibeOran, adet);
        }
        public Sonuc HibeMakineDegerlendirmeKaydet(Kullanici kullanan, int hibeKod, string anahtar, double artisPuan)
        {
            try
            {
                return TRMHibeMakine.DegerlendirmeKaydet(null, kullanan, hibeKod, anahtar, artisPuan);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        public HibeMakineServis[] HibeMakineListForTKDK(Kullanici kullanan, string kimlikNo)
        {
            try
            {
                return new TRMHibeMakine().HibeMakineListForTKDK(kullanan, kimlikNo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public HazineRapor[] HibeMakineHazineRapor(int yil, int ay)
        {
            try
            {
                return TRMHibeMakine.HazineRapor(yil, ay);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public HazineRapor[] HibeYatirimHazineRapor(int yil, int ay, int tur, int etapNo)
        {
            try
            {
                return TRMHibeYatirim.HazineRapor(yil, ay, tur, etapNo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public HazineRapor[] HibeSulamaHazineRapor(int yil, int ay, int tur, int etapNo)
        {
            try
            {
                return TRMHibeSulama.HazineRapor(yil, ay, tur, etapNo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /**********Makine/Ekipman****************************/

        public Sonuc AnketSorulariniAktar(string onEkTur, string eskiEtap, string yeniEtap)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeMakineYonetim().AnketSorulariniAktar(null, onEkTur, eskiEtap, yeniEtap);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /**********Ekonomik Yatirim****************************/
        /// <summary>
        /// Arguman olarak verilen, hibe yatirim bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeYatirimKaydet(Kullanici kullanan, HibeYatirim bilgi)
        {
            try
            {
                return new TRMHibeYatirim().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public Sonuc HibeTutarlariKaydet(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().HibeTutarlariKaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimIsletmePlaniKaydet(Kullanici kullanan, YatirimIsletmePlani bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().KaydetIsletmePlani(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimKaydetSozlesmeSonrasi(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().KaydetSozlesmeSonrasi(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimKaydetKabul(Kullanici kullanan, int etapNo, string ip, int basvuruTur)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().KaydetKabul(kullanan, etapNo, ip, basvuruTur);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimKaydetGeriAl(Kullanici kullanan, int etapNo, string ip, int geriAlmaTuru, int basvuruTur, string altYapiGrup)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().KaydetKabulGeriAl(kullanan, etapNo, ip, geriAlmaTuru, basvuruTur, altYapiGrup);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimKaydetKabulCalismasi(Kullanici kullanan, int etapNo, string ilKod, HibeYatirim[] bilgiler, int kabulCalismasi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().KaydetKabulCalismasi(kullanan, etapNo, ilKod, bilgiler, kabulCalismasi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimKaydetUygunlukCalismasi(Kullanici kullanan, HibeYatirim[] bilgiler)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().KaydetUygunlukCalismasi(kullanan, bilgiler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimKaydetBasvuruYapildi(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().KaydetBasvuruYapildi(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimBasvuruyuOnlineYap(Kullanici kullanan, int kod, string ip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().BasvuruyuOnlineYap(kullanan, kod, ip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public bool HibeYatirimListeleBasvuruYapildi(Kullanici kullanan, int kod)
        {
            try
            {
                return TRMHibeYatirim.ListeleBasvuruYapildi(null, kullanan, kod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Arguman olarak verilen, hibe yatirim sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeYatirimKaydetSozlesme(Kullanici kullanan, HibeYatirimSozlesme sozlesme, HibeYatirimMaliyet maliyet, string ip)
        {
            try
            {
                return new TRMHibeYatirim().KaydetSozlesme(kullanan, sozlesme, maliyet, ip);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }
        public Sonuc HibeYatirimSilSozlesme(Kullanici kullanan, HibeYatirimSozlesme sozlesme)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().SilSozlesme(kullanan, sozlesme);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, hibe yatirim sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeYatirimMaliyetGuncelle(Kullanici kullanan, HibeYatirimMaliyet maliyet)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().MaliyetGuncelle(kullanan, maliyet);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public HibeYatirim HibeYatirimSozlesmeListele(Kullanici kullanan, int hibeKod)
        {
            try
            {
                return TRMHibeYatirim.ListeleSozlesme(ref ve, kullanan, hibeKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public HibeYatirim HibeYatirimListeleYatirimVeSozlesme(Kullanici kullanan, int hibeKod)
        {
            try
            {
                return TRMHibeYatirim.ListeleYatirimVeSozlesme(ref ve, kullanan, hibeKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Arguman olarak verilen, hibe yatirim red bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeYatirimKaydetReddedildi(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().KaydetReddedildi(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimKaydetOnline(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().KaydetOnline(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimBasvuruyuGeriCek(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().BasvuruyuGeriCek(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe yatirim bilgisini veritabanından okur 
        /// ve HibeYatirim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini HibeYatirim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HibeYatirimListele(Kullanici kullanan, HibeYatirim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirim().Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeYatirimListeleMerkezdeKabulHali(Kullanici kullanan, int kod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirim().ListeleMerkezdeKabulHali(kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public HibeYatirimServis[] HibeYatirimListForTKDK(Kullanici kullanan, string kimlikNo)
        {
            try
            {
                return new TRMHibeYatirim().HibeYatirimListForTKDK(kullanan, kimlikNo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public YatirimIsletmePlani HibeYatirimIsletmePlaniGetir(Kullanici kullanan, int hibeKod)
        {
            try
            {
                return TRMHibeYatirim.YatirimIsletmePlaniGetir(kullanan, hibeKod);
            }
            catch (Exception e)
            {
                YatirimIsletmePlani donusDeger = new YatirimIsletmePlani();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public ObjectArray HibeYatirimSorgula(Kullanici kullanan, HibeYatirim kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc)
        {
            try
            {
                return TRMHibeYatirim.Sorgula(null, kullanan, kriter, ilkKayit, kayitSay, sortKolon, asc);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray HibeYatirimMalzemeSorgula(Kullanici kullanan, HibeYatirim kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc)
        {
            try
            {
                return TRMHibeYatirim.ListeleMalzeme(null, kullanan, kriter, ilkKayit, kayitSay, sortKolon, asc);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public int HibeYatirimBasvuruSayiDondur(Kullanici kullanan, HibeYatirim kriter)
        {
            try
            {
                return TRMHibeYatirim.HibeYatirimBasvuruSayiDondur(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool HibeYatirimKabulSonrasiVarMi(Kullanici kullanan, HibeYatirim kriter)
        {
            try
            {
                return TRMHibeYatirim.KabulSonrasiVarMi(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ObjectArray HibeYatirimListeleOncelikliForKabul(Kullanici kullanan, HibeYatirim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TRMHibeYatirim.ListeleOncelikliForKabul(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HibeYatirimEksikBelgeListele(Kullanici kullanan, HibeYatirim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TRMHibeYatirim.ListeleEksikBelge(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimGoruntuBilgiKaydet(Kullanici kullanan, Goruntu bilgi)
        {
            try
            {
                return TRMHibeYatirim.KaydetGoruntu(ref ve, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        public ObjectArray HibeYatirimGoruntuBilgiListele(Kullanici kullanan, Goruntu kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TRMHibeYatirim.ListeleGoruntu(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray ListeleKabulCalismasiIlToplami(Kullanici kullanan, Butce kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TRMHibeYatirim.ListeleKabulCalismasiIlToplami(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray ListeleSulamaKabulCalismasiIlToplami(Kullanici kullanan, HibeSulama kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TRMHibeSulama.ListeleSulamaKabulCalismasiIlToplami(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe yatirim tarihçe bilgisini veritabanından okur 
        /// ve HibeYatirimTarihce nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, listeyi HibeYatirimTarihce nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HibeYatirimTarihceListele(Kullanici kullanan, HibeYatirim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirim().ListeleTarihce(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        //public ObjectArray HibeYatirimRefFiyatTarihceListele(Kullanici kullanan, HibeYatirimRefFiyat kriter)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new TRMHibeYatirimRefFiyat().ListeleTarihce(kullanan, kriter);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        public ObjectArray HibeYatirimIlBazindaIcmal(Kullanici kullanan, HibeYatirim kriter, bool ilListele)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().HibeYatirimIlBazindaIcmal(kullanan, kriter, ilListele);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimYonetimKaydet(Kullanici kullanan, Yonetim form)
        {
            try
            {
                return TRMHibeYatirimYonetim.Kaydet(ref ve, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc HibeYatirimYonetimSil(Kullanici kullanan, Yonetim form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeYatirimYonetim.Sil(ref ve, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Yonetim[] HibeYatirimYonetimListele(Kullanici kullanan, Yonetim form)
        {
            try
            {
                return TRMHibeYatirimYonetim.Listele(ref ve, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Yonetim HibeYatirimYonetimOku(Kullanici kullanan, Yonetim form)
        {
            try
            {
                return TRMHibeYatirimYonetim.Oku(ref ve, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int HibeYatirimAktifEtapNo()
        {
            try
            {
                return TRMHibeYatirimYonetim.HibeYatirimAktifEtapNo(ref ve);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string HibeYatirimTebligNoBul(int etapNo, string konuKod)
        {
            try
            {
                return TRMHibeYatirimYonetim.HibeYatirimTebligNoBul(ref ve, etapNo, OrtakFonksiyonlar.ConvertToInt(konuKod, 0));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public double HibeYatirimAktifUstLimit(int hukukiStatu)
        {
            try
            {
                return TRMHibeYatirimYonetim.HibeYatirimAktifUstLimit(ref ve, hukukiStatu);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public double HibeYatirimAktifUstLimitKonu(int etapNo, int projeNitelik, string konuKod, int hukukiStatu)
        {
            try
            {
                return TRMHibeYatirimYonetim.HibeYatirimAktifUstLimitKonu(ref ve, etapNo, projeNitelik, konuKod, hukukiStatu);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string HibeYatirimOnlineKapandiMi(Kullanici kullanan, string basvuruNo)
        {
            try
            {
                return TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, basvuruNo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string HibeYatirimDegerlendirmeKapandiMi(string basvuruNo, string projeKonu, string ilKodu)
        {
            try
            {
                return TRMHibeYatirimYonetim.HibeYatirimDegerlendirmeKapandiMi(basvuruNo, projeKonu, ilKodu);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //public void HibeYatirimHibeTutarHesapla(int hukukiStatu, decimal referansTutar, decimal proformaTutar, decimal malzemeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, decimal hibeOran)
        public void HibeYatirimHibeTutarHesapla(int projeNitelik, string konuKod, decimal referansTutar, decimal proformaTutar, decimal malzemeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, int hukukiStatu, decimal hibeOran, int etapNo)
        {
            TRMHibeYatirim.HibeTutarHesapla(projeNitelik, konuKod, referansTutar, proformaTutar, malzemeTutar, ref hibeEsasTutar, ref toplamTutar, ref ayniKatki, ref hibeTutar, hukukiStatu, hibeOran, etapNo);
        }

        /// <summary>
        /// Arguman olarak verilen, hibe yatirim sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeYatirimSozlesmeDurumunuGuncelle(Kullanici kullanan, HibeYatirimSozlesme sozlesme)
        {
            try
            {
                return new TRMHibeYatirim().SozlesmeDurumunuGuncelle(kullanan, sozlesme);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc HibeYatirimSozlesmeNihaiRaporTarihGuncelle(Kullanici kullanan, HibeYatirimSozlesme sozlesme)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().NihaiRaporTarihGuncelle(kullanan, sozlesme);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimSozlesmeYapiKimlikNoGuncelle(Kullanici kullanan, HibeYatirimSozlesme sozlesme)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().YapiKimlikNoGuncelle(kullanan, sozlesme);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, YatirimNitelik bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc YatirimNitelikKaydet(Kullanici kullanan, YatirimNitelik bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimNitelik().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, YatirimNitelik bilgisini veritabanından okur 
        /// ve YatirimNitelik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, YatirimNitelik listesini YatirimNitelik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray YatirimNitelikListele(Kullanici kullanan, YatirimNitelik kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimNitelik().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, YatirimNitelik bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc YatirimNitelikSil(Kullanici kullanan, YatirimNitelik bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimNitelik().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, YatirimKonu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc YatirimMalzemeTurKaydet(Kullanici kullanan, YatirimMalzemeTur bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMalzemeTur().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, YatirimMalzemeTur bilgisini veritabanından okur 
        /// ve YatirimMalzemeTur nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, YatirimMalzemeTur listesini YatirimMalzemeTur nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray YatirimMalzemeTurListele(Kullanici kullanan, YatirimMalzemeTur kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimMalzemeTur().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, YatirimMalzemeTur bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc YatirimMalzemeTurSil(Kullanici kullanan, YatirimMalzemeTur bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMalzemeTur().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimMalzemeTanimKaydet(Kullanici kullanan, YatirimMalzemeTanim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMalzemeTanim().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimKonuNitelikKisitKaydet(Kullanici kullanan, YatirimKonuNitelikKisit bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonuNitelikKisit().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimKonuNitelikKisitSil(Kullanici kullanan, YatirimKonuNitelikKisit bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonuNitelikKisit().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray YatirimKonuNitelikKisitListele(Kullanici kullanan, YatirimKonuNitelikKisit kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimKonuNitelikKisit().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, YatirimMalzemeTanim bilgisini veritabanından okur 
        /// ve YatirimMalzemeTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, YatirimMalzemeTanim listesini YatirimMalzemeTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray YatirimMalzemeTanimListele(Kullanici kullanan, YatirimMalzemeTanim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimMalzemeTanim().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, YatirimMalzemeTanim bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc YatirimMalzemeTanimSil(Kullanici kullanan, YatirimMalzemeTanim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMalzemeTanim().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimKonuKaydet(Kullanici kullanan, YatirimKonu bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonu().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, YatirimKonu bilgisini veritabanından okur 
        /// ve YatirimKonu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, YatirimKonu listesini YatirimKonu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray YatirimKonuListele(Kullanici kullanan, YatirimKonu kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimKonu().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, YatirimKonu bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc YatirimKonuSil(Kullanici kullanan, YatirimKonu bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonu().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimKonuIlKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonu().KaydetKonuIl(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray YatirimKonuIlListele(Kullanici kullanan, YatirimKonuIl kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimKonu().ListeleKonuIl(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimKonuIlAktar(Kullanici kullanan, int eskiEtap, int yeniEtap)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonu().AktarKonuIl(kullanan, eskiEtap, yeniEtap);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        public Sonuc YatirimAltKonuIlKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonu().KaydetAltKonuIl(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray YatirimAltKonuIlListele(Kullanici kullanan, YatirimAltKonuIl kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimKonu().ListeleAltKonuIl(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimAltKonuIlAktar(Kullanici kullanan, int eskiEtap, int yeniEtap)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonu().AktarAltKonuIl(kullanan, eskiEtap, yeniEtap);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimKonuUstSinirKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonu().KaydetKonuUstSinir(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray YatirimKonuUstSinirListele(Kullanici kullanan, YatirimKonuUstSinir kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimKonu().ListeleKonuUstSinir(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimKonuUstSinirAktar(Kullanici kullanan, int eskiEtap, int yeniEtap)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonu().AktarKonuUstSinir(kullanan, eskiEtap, yeniEtap);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimAltKonuDetayKaydet(Kullanici kullanan, YatirimAltKonuDetay bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimKonu().KaydetAltKonuDetay(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray YatirimAltKonuDetayListele(Kullanici kullanan, YatirimAltKonuDetay kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimKonu().ListeleAltKonuDetay(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimAltKonuDetaySil(Kullanici kullanan, YatirimAltKonuDetay bilgi)
        {
            try
            {
                return new TRMYatirimKonu().SilAltKonuDetay(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        public Sonuc YatirimOncelikKaydet(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimOncelik().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray YatirimOncelikListele(Kullanici kullanan, HibeYatirimOncelik kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirimOncelik().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimUygulamaSozlesmeKaydet(Kullanici kullanan, HibeYatirimUygulamaSozlesme us)
        {
            try
            {
                return TRMHibeYatirimUygulamaSozlesme.Kaydet(ref ve, kullanan, us);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        public Sonuc HibeYatirimKaydetEksikBelge(Kullanici kullanan, int hibeKod, ObjectArray eksikBelge)
        {
            try
            {
                return TRMHibeYatirim.KaydetEksikBelge(ref ve, kullanan, hibeKod, eksikBelge);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public ObjectArray HibeYatirimListeleEksikBelge(Kullanici kullanan, int hibeKod)
        {
            try
            {
                return new TRMHibeYatirim().ListeleEksikBelge(kullanan, hibeKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public Sonuc HibeYatirimEskiEtapUygSozlesmeKaydet(Kullanici kullanan, HibeYatirimUygulamaSozlesme[] uygSozlesme)
        {
            try
            {
                return TRMHibeYatirimUygulamaSozlesme.EskiEtapUygSozlesmeKaydet(ref ve, kullanan, uygSozlesme);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeYatirimUygulamaSozlesmeSil(Kullanici kullanan, HibeYatirimUygulamaSozlesme us)
        {
            try
            {
                return TRMHibeYatirimUygulamaSozlesme.Sil(ref ve, kullanan, us);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public HibeYatirimUygulamaSozlesme[] HibeYatirimUygulamaSozlesmeListele(Kullanici kullanan, HibeYatirimUygulamaSozlesme us, bool iptalVeHataliHaric)
        {
            try
            {
                return TRMHibeYatirimUygulamaSozlesme.Listele(ref ve, kullanan, us, iptalVeHataliHaric);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc HibeYatirimEskiEtapHakedisleriKaydet(Kullanici kullanan, HibeYatirimHakedis[] hakedisler)
        {
            try
            {
                return TRMHibeYatirimHakedis.EskiEtapHakedisleriKaydet(ref ve, kullanan, hakedisler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeYatirimHakedisKaydet(Kullanici kullanan, HibeYatirimHakedis hakedis)
        {
            try
            {
                return TRMHibeYatirimHakedis.Kaydet(ref ve, kullanan, hakedis);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc OdenenHakedislerinUygulamaSozlesmesiDurumGuncellemesi(Kullanici kullanan, int odemeKod)
        {
            try
            {
                return TRMHibeYatirimHakedis.OdenenHakedislerinUygulamaSozlesmesiDurumGuncellemesi(kullanan, odemeKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeYatirimHakedisSil(Kullanici kullanan, HibeYatirimHakedis hakedis)
        {
            try
            {
                return TRMHibeYatirimHakedis.Sil(ref ve, kullanan, hakedis);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeYatirimHakedisOdemeGrupNoGuncelle(Kullanici kullanan, HibeYatirimHakedis hakedis)
        {
            try
            {
                return TRMHibeYatirimHakedis.GuncelleOdemeGrupNo(ref ve, kullanan, hakedis);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeYatirimHakedisOdemeKontrolGuncelle(Kullanici kullanan, HibeYatirimHakedis hakedis)
        {
            try
            {
                return TRMHibeYatirimHakedis.GuncelleKontrol(ref ve, kullanan, hakedis);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public HibeYatirimHakedis[] HibeYatirimHakedisListele(Kullanici kullanan, HibeYatirimHakedis hakedis)
        {
            try
            {
                return TRMHibeYatirimHakedis.Listele(ref ve, kullanan, hakedis);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public HibeYatirim[] HibeYatirimHakedisListeleOdemeGonder(Kullanici kullanan, HibeYatirim kriter)
        {
            try
            {
                return TRMHibeYatirimHakedis.ListeleOdemeGonder(ref ve, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public HibeYatirimUygulamaSozlesme[] HibeYatirimListeleUygulamaSozlesmesiVeHakedisler(Kullanici kullanan, int hibeKod)
        {
            try
            {
                return TRMHibeYatirimHakedis.ListeleUygulamaSozlesmesiVeHakedisler(ref ve, kullanan, hibeKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc HibeYatirimHakedisOdemeyeGonder(Kullanici kullanan, string odemeGrupNo)
        {
            try
            {
                return TRMHibeYatirimHakedis.OdemeyeGonder(ref ve, kullanan, odemeGrupNo);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeYatirimHakedisOdemeIslemYap(Kullanici kullanan, HibeYatirimHakedis[] hakedisler, ENUMHibeYatirimHakedisDurum yeniHakedisDurum)
        {
            try
            {
                return TRMHibeYatirimHakedis.OdemeIslemYap(ref ve, kullanan, hakedisler, yeniHakedisDurum);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeYatirimKaydetDegerlendirme(Kullanici kullanan, HibeYatirim[] bilgiler)
        {
            try
            {
                return TRMHibeYatirim.KaydetDegerlendirme(ref ve, kullanan, bilgiler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public ObjectArray YatirimKapasiteBirimListele()
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirim().YatirimKapasiteBirimListele();
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, hibe yatirim sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeYatirimBasvuruyuIslemeAl(Kullanici kullanan, int kod, string ilKod, string basvuruNo, string ip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().BasvuruyuIslemeAl(kullanan, kod, ilKod, basvuruNo, ip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public int HibeYatirimBasvuruDurumBilgisi(string basvuruNo)
        {
            try
            {
                return new TRMHibeYatirim().BasvuruDurumBilgisi(basvuruNo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool HibeYatirimMerkezKabulAsamasiGectimi(int etapNo)
        {
            try
            {
                return new TRMHibeYatirim().MerkezKabulAsamasiGectimi(etapNo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /**********Ekonomik Yatirim****************************/

        /******Sulama**********************/
        /// <summary>
        /// Arguman olarak verilen, Sulama grubu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SulamaGrubuKaydet(Kullanici kullanan, SulamaGrubu bilgi)
        {
            try
            {
                return new TRMSulamaGrubu().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Sulama grubu bilgisini veritabanından okur 
        /// ve SulamaGrubu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Sulama grubu listesini SulamaGrubu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SulamaGrubuListele(Kullanici kullanan, SulamaGrubu kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMSulamaGrubu().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, Sulama grubu bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SulamaGrubuSil(Kullanici kullanan, SulamaGrubu bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMSulamaGrubu().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc SulamaMalzemeKaydet(Kullanici kullanan, SulamaMalzeme m)
        {
            try
            {
                return TRMSulamaMalzeme.Kaydet(null, kullanan, m);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc SulamaMalzemeSil(Kullanici kullanan, SulamaMalzeme m)
        {
            try
            {
                return TRMSulamaMalzeme.Sil(null, kullanan, m);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public SulamaMalzeme[] SulamaMalzemeListele(Kullanici kullanan, SulamaMalzeme m, bool detayVer)
        {
            try
            {
                return TRMSulamaMalzeme.Listele(null, kullanan, m, detayVer);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc HibeSulamaMalzemeKaydet(Kullanici kullanan, HibeSulamaMalzemeFiyat[] mfler, HibeSulama hs)
        {
            try
            {
                return TRMHibeSulamaMalzeme.Kaydet(null, kullanan, mfler, hs);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeSulamaMalzemeSil(Kullanici kullanan, int hibeKod, ENUMHibeSulamaMalzemeTip tip, string hibeIlKod, int sulamaGrupKod, string parselID)
        {
            try
            {
                return TRMHibeSulamaMalzeme.Sil(null, kullanan, hibeKod, tip, hibeIlKod, sulamaGrupKod, parselID, true);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public HibeSulamaMalzemeFiyat[] HibeSulamaMalzemeListele(Kullanici kullanan, int hibeKod, ENUMHibeSulamaMalzemeTip tip, string hibeIlKod, int sulamaGrupKod, string parselNo)
        {
            try
            {
                return TRMHibeSulamaMalzeme.Listele(null, kullanan, hibeKod, tip, hibeIlKod, sulamaGrupKod, parselNo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public HibeSulamaMalzemeFiyat[] ListeleSulamaGrupTutar(Kullanici kullanan, int hibeKod, int tip)
        {
            try
            {
                return TRMHibeSulamaMalzeme.ListeleSulamaGrupTutar(null, kullanan, hibeKod, tip);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc KontrolSulamaOncedenReddilenDestek(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().KontrolOncedenReddilenDestek(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, hibe Sulama bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeSulamaKaydet(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeSulamaKaydetSozlesme(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().KaydetSozlesme(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeSulamaKaydetProjeNo(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().KaydetProjeNo(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama red bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeSulamaKaydetReddedildi(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().KaydetReddedildi(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeSulamaKaydetAsilYedek(Kullanici kullanan, int[] hibeKodlar, string asilYedek)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().KaydetAsilYedek(kullanan, hibeKodlar, asilYedek);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeSulamaKaydetKabul(Kullanici kullanan, int[] hibeKodlar, string kabul)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().KaydetKabul(kullanan, hibeKodlar, kabul);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeSulamaKaydetBasvuruYapildi(Kullanici kullanan, int hibeKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().KaydetBasvuruYapildi(kullanan, hibeKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama yerinde tespit bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeSulamaKaydetYerindeTespit(Kullanici kullanan, HibeSulama bilgi)
        {
            try
            {
                return new TRMHibeSulama().KaydetYerindeTespit(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama fatura bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HibeSulamaKaydetFatura(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulama().KaydetFatura(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe Sulama bilgisini veritabanından okur 
        /// ve HibeSulama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini HibeSulama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HibeSulamaListele(Kullanici kullanan, HibeSulama kriter)
        {
            try
            {
                return new TRMHibeSulama().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public ObjectArray HibeSulamaSorgula(Kullanici kullanan, HibeSulama kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc)
        {
            try
            {
                return TRMHibeSulama.Sorgula(kullanan, kriter, ilkKayit, kayitSay, sortKolon, asc);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public int HibeSulamaBasvuruSayiDondur(Kullanici kullanan, HibeSulama kriter)
        {
            try
            {
                return TRMHibeSulama.HibeSulamaBasvuruSayiDondur(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe Sulama tarihçe bilgisini veritabanından okur 
        /// ve HibeSulamaTarihce nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, listeyi HibeSulamaTarihce nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HibeSulamaTarihceListele(Kullanici kullanan, HibeSulama kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeSulama().ListeleTarihce(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeSulamaIlBazindaIcmal(Kullanici kullanan, HibeSulama kriter, bool ilListele)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().HibeSulamaIlBazindaIcmal(kullanan, kriter, ilListele);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeSulamaKonusunaGoreRapor(Kullanici kullanan, HibeSulama kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TRMHibeSulama.KonusunaGoreDagilimRapor(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HibeSulamaBilgiNotuRapor(Kullanici kullanan, HibeSulama kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TRMHibeSulama.BilgiNotuRapor(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HibeSulamaKonuIstatistik(Kullanici kullanan, HibeSulama kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TRMHibeSulama.KonuIstatistik(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HibeSulamaIstatistik(Kullanici kullanan, HibeSulama kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TRMHibeSulama.Istatistik(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeSulamaYonetimKaydet(Kullanici kullanan, Yonetim form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeSulamaYonetim.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc HibeSulamaYonetimSil(Kullanici kullanan, Yonetim form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeSulamaYonetim.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Yonetim[] HibeSulamaYonetimListele(Kullanici kullanan, Yonetim form)
        {
            try
            {
                return TRMHibeSulamaYonetim.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Yonetim HibeSulamaYonetimOku(Kullanici kullanan, Yonetim form)
        {
            try
            {
                return TRMHibeSulamaYonetim.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int HibeSulamaAktifEtapNo()
        {
            try
            {
                return TRMHibeSulamaYonetim.HibeSulamaAktifEtapNo(null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public double HibeSulamaAktifUstLimit(int hukukiStatu)
        {
            try
            {
                return TRMHibeSulamaYonetim.HibeSulamaAktifUstLimit(null, hukukiStatu);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string HibeSulamaOnlineKapandiMi()
        {
            try
            {
                return TRMHibeSulamaYonetim.HibeSulamaOnlineKapandiMi(null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string HibeSulamaSozlesmeKapandiMi()
        {
            try
            {
                return TRMHibeSulamaYonetim.HibeSulamaSozlesmeKapandiMi(null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void HibeSulamaHibeTutarHesapla(int hukukiStatu, ref decimal hibeEsasTutarProje, ref decimal hibeEsasTutarUygulama, decimal proformaTutar, decimal malzemeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, decimal hibeOran, decimal basvuruAyniKatki)
        {
            TRMHibeSulama.HibeTutarHesapla(hukukiStatu, ref hibeEsasTutarProje, ref hibeEsasTutarUygulama, proformaTutar, malzemeTutar, ref hibeEsasTutar, ref toplamTutar, ref ayniKatki, ref hibeTutar, hibeOran, basvuruAyniKatki);
        }
        public string HibeSulamaBirimRefFarklimi(Kullanici kullanan, int hibeKod)
        {
            try
            {
                return TRMHibeSulamaYonetim.HibeSulamaBirimReferansFarklimi(null, kullanan, hibeKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public Sonuc HibeSulamaBankaKrediKaydet(Kullanici kullanan, HibeSulamaBankaKredi form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeSulamaBankaKredi.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray HibeSulamaBankaKrediListele(Kullanici kullanan, HibeSulamaBankaKredi kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TRMHibeSulamaBankaKredi.Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeSulamaBankaKrediSil(Kullanici kullanan, HibeSulamaBankaKredi form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeSulamaBankaKredi.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /******Sulama**********************/

        public string PlakaGetir(string ilKod)
        {
            string deger = "";
            try
            {
                deger = new TRMRapor().PlakaGetir(null, ilKod);
            }
            catch { }
            return deger;
        }

        public int[] AktifEtapdaKimlikNoVarmi(string kimlikNo, int tip)
        {
            int[] deger = new int[2];
            try
            {
                deger = new TRMHibeMakine().AktifEtapdaKimlikNoVarmi(null, kimlikNo, tip);
            }
            catch { }
            return deger;
        }

        public int[] OncekiEtapdaKimlikNoVarmi(string kimlikNo, int tip)
        {
            int[] deger = new int[2];
            try
            {
                deger = new TRMHibeMakine().OncekiEtapdaKimlikNoVarmi(null, kimlikNo, tip);
            }
            catch { }
            return deger;
        }

        /// <summary>
        /// Arguman olarak verilen, bankaSube bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BankaSubeKaydet(Kullanici kullanan, BankaSube bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMBankaSube().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, BankaSube bilgisini veritabanından okur 
        /// ve BankaSube nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, BankaSube listesini BankaSube nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BankaSubeListele(Kullanici kullanan, BankaSube kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMBankaSube().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, BankaSube bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BankaSubeSil(Kullanici kullanan, BankaSube bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMBankaSube().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, Koy bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KoyKaydet(Kullanici kullanan, Koy bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMKoy().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc KoyleriKapat(Kullanici kullanan, Koy bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMKoy().KoyleriKapat(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Koy bilgisini veritabanından okur 
        /// ve Koy nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Koy listesini Koy nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray KoyListele(Kullanici kullanan, Koy kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMKoy().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, Koy bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KoySil(Kullanici kullanan, Koy bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMKoy().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray CografyaListele(string kod, bool sadeceDetay)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMKoy().CografyaListele(kod, sadeceDetay);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public HibeMakine[] HibeMakineKimlikBilgisiEksikBasvurular(Kullanici kullanan)
        {
            try
            {
                return TRMHibeMakine.KimlikBilgisiEksikBasvurular(null, kullanan);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc HibeMakineKimlikBilgisiEksikGuncelle(Kullanici kullanan, HibeMakine hm)
        {
            try
            {
                return TRMHibeMakine.KimlikBilgisiEksikGuncelle(null, kullanan, hm);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public HibeSulama[] HibeSulamaKimlikBilgisiEksikBasvurular(Kullanici kullanan)
        {
            try
            {
                return TRMHibeSulama.KimlikBilgisiEksikBasvurular(null, kullanan);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc HibeSulamaKimlikBilgisiEksikGuncelle(Kullanici kullanan, HibeSulama hs)
        {
            try
            {
                return TRMHibeSulama.KimlikBilgisiEksikGuncelle(null, kullanan, hs);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc HibeSulamaDegerlendirmeKaydet(Kullanici kullanan, int hibeKod, string anahtar, double artisPuan)
        {
            try
            {
                return TRMHibeSulama.DegerlendirmeKaydet(null, kullanan, hibeKod, anahtar, artisPuan);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        public ObjectArray MalzemeFiyatAnalizi(Kullanici kullanan, MalzemeFiyatAnalizi mfa)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().MalzemeFiyatAnalizi(kullanan, mfa);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public YatirimIzlemeCetveli[] YatirimIzlemeCetveliListesi(Kullanici kullanan, YatirimIzlemeCetveli[] izlemeCetveli, int etapNo, int ilTur, int konuTur)
        {
            try
            {
                return new TRMRapor().YatirimIzlemeCetveliListesi(kullanan, izlemeCetveli, etapNo, ilTur, konuTur);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ObjectArray MakinaEkipmanDesteklemeleri(Kullanici kullanan, int etapNo, int odemeIlTur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().MakinaEkipmanDesteklemeleri(kullanan, etapNo, odemeIlTur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray KKYDPOdeme(Kullanici kullanan, int etapNo, int odemeIlTur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().KKYDPOdeme(kullanan, etapNo, odemeIlTur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray EYHibeTutariGetir(Kullanici kullanan, int etapNo, int odemeIlTur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().EYHibeTutariGetir(kullanan, etapNo, odemeIlTur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimAnket1Kaydet(Kullanici kullanan, int hibeKod, string anahtar, string ip)
        {
            try
            {
                return new TRMHibeYatirimAnket1().Kaydet(null, kullanan, hibeKod, anahtar, ip);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("Anket Hata:" + e.ToString());
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeYatirimAnket2Kaydet(Kullanici kullanan, int hibeKod, string anahtar, string ip)
        {
            try
            {
                return new TRMHibeYatirimAnket2().Kaydet(null, kullanan, hibeKod, anahtar, ip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeYatirimAnket3Kaydet(Kullanici kullanan, int hibeKod, string anahtar, string ip)
        {
            try
            {
                return new TRMHibeYatirimAnket3().Kaydet(null, kullanan, hibeKod, anahtar, ip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc HibeYatirimAnket4Kaydet(Kullanici kullanan, int hibeKod, string anahtar, string ip)
        {
            try
            {
                return new TRMHibeYatirimAnket4().Kaydet(null, kullanan, hibeKod, anahtar, ip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc AnketSonucSifirla(Kullanici kullanan, int hibeKod, int etapNo, string ilKod, string ip)
        {
            try
            {
                return new TRMHibeYatirim().AnketSonucSifirla(kullanan, hibeKod, etapNo, ilKod, ip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        /// <summary>
        /// Arguman olarak verilen, KaynakTuru bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EklenenDosyayiKaydet(Kullanici kullanan, EklenenDosya bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMDosya().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, KaynakTuru bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EklenenDosyayiSil(Kullanici kullanan, string kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMDosya().Sil(kullanan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, KaynakTuru bilgisini veritabanından okur 
        /// ve KaynakTuru nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, KaynakTuru listesini KaynakTuru nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray EklenenDosyalariListele(Kullanici kullanan, EklenenDosya kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMDosya().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, Ceza bilgisini veritabanından okur 
        ///// ve Ceza nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="kullanan">The kullanan.</param>
        ///// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        ///// <returns>İstenilen kriterdeki, KaynakTuru listesini Ceza nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray CezalariListele(Kullanici kullanan, Ceza kriter)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new TRMHibeCeza().Listele(kullanan, kriter);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}
        public Sonuc CezaKaydet(Kullanici kullanan, Ceza bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeCeza().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, Ceza bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc CezaSil(Kullanici kullanan, Ceza bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeCeza().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, Ceza bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc CezaliKisiSil(Kullanici kullanan, Ceza bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeCeza().SilKisi(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Ceza bilgisini veritabanından okur 
        /// ve Ceza nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, KaynakTuru listesini Ceza nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray CezalariListele(Kullanici kullanan, Ceza kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeCeza().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Ceza bilgisini veritabanından okur 
        /// ve Ceza nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, KaynakTuru listesini Ceza nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleCezaVeCezaliKisiler(Kullanici kullanan, int cezaKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeCeza().ListeleCezaVeCezaliKisiler(kullanan, cezaKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Ceza bilgisini veritabanından okur 
        /// ve Ceza nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, KaynakTuru listesini Ceza nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray CezaliKisileriListele(Kullanici kullanan, int cezaKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeCeza().CezaliKisileriListele(kullanan, cezaKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        //public bool CezaliMi(Kullanici kullanan)
        //{
        //    bool donuDeger = false;
        //    try
        //    {
        //        donuDeger = new TRMHibeCeza().CezaliMi(kullanan.mernis);
        //    }
        //    catch (Exception e)
        //    {
        //        donuDeger = false;
        //    }
        //    return donuDeger;
        //}

        /// <summary>
        /// Arguman olarak verilen, Başvuru belge tanim bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BasvuruBelgeTanimiKaydet(Kullanici kullanan, BasvuruBelgeTanim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMBasvuruBelgeTanim().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, Başvuru belge tanim bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BasvuruBelgeTanimiSil(Kullanici kullanan, BasvuruBelgeTanim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMBasvuruBelgeTanim().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc BasvuruBelgeTanimiAktar(Kullanici kullanan, int eskiEtap, int yeniEtap)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMBasvuruBelgeTanim().Aktar(kullanan, eskiEtap, yeniEtap);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, BasvuruBelgeTanim bilgisini veritabanından okur 
        /// ve BasvuruBelgeTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, BasvuruBelgeTanim listesini BasvuruBelgeTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BasvuruBelgeTanimlariniListele(Kullanici kullanan, BasvuruBelgeTanim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMBasvuruBelgeTanim().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, BasvuruBelgeTanim bilgisini veritabanından okur 
        /// ve BasvuruBelgeTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, BasvuruBelgeTanim listesini BasvuruBelgeTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BasvuruBelgeVeAdlariniListele(Kullanici kullanan, BasvuruBelgeTanim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMBasvuruBelgeTanim().ListeleBelgeler(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, BasvuruBelgeTanim bilgisini veritabanından okur 
        /// ve BasvuruBelgeTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, BasvuruBelgeTanim listesini BasvuruBelgeTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BasvuruBelgeAdlariniListele(Kullanici kullanan, int belgeKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMBasvuruBelgeTanim().BasvuruBelgeAdlariniListele(kullanan, belgeKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, Başvuru belge tanim bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BasvurudaYuklenenBelgeleriKaydet(Kullanici kullanan, BasvurudaYuklenenBelgeler bilgi)
        {
            try
            {
                return new TRMHibeBasvuruYuklenenBelgeler().Kaydet(kullanan, bilgi);
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
        /// Arguman olarak verilen, Başvuru belge tanim bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BasvurudaYuklenenBelgeleriSil(Kullanici kullanan, BasvurudaYuklenenBelgeler bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeBasvuruYuklenenBelgeler().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, BasvuruBelgeTanim bilgisini veritabanından okur 
        /// ve BasvuruBelgeTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, BasvuruBelgeTanim listesini BasvuruBelgeTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BasvurudaYuklenenBelgeleriListele(Kullanici kullanan, BasvurudaYuklenenBelgeler kriter, bool eksikBelgeler)
        {
            try
            {
                return new TRMHibeBasvuruYuklenenBelgeler().Listele(kullanan, kriter, eksikBelgeler);
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
        /// Arguman olarak verilen, SorulanSorular bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SorulanSoruyuKaydet(Kullanici kullanan, SorulanSorular bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMSorulanSorular().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, SorulanSorular bilgisini veritabanından okur 
        /// ve SorulanSorular nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, soruları listesini SorulanSorular nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SorulanSorulariListele(Kullanici kullanan, SorulanSorular kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMSorulanSorular().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, SorulanSorular bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SorulanSoruyuSil(Kullanici kullanan, SorulanSorular bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMSorulanSorular().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BasvuruKaydet(Kullanici kullanan, Basvuru bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeBasvuru.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Basvuru[] BasvuruListele(Kullanici kullanan, Basvuru kriter)
        {
            Basvuru[] donusDeger = new Basvuru[0];
            try
            {
                donusDeger = TRMHibeBasvuru.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
            }
            return donusDeger;
        }

        public Sonuc BasvuruSil(Kullanici kullanan, Basvuru bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeBasvuru.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BasvuruSablonKaydet(Kullanici kullanan, BasvuruSablon bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeBasvuru.KaydetSablon(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BasvuruSablon[] BasvuruSablonListele(Kullanici kullanan, BasvuruSablon kriter)
        {
            BasvuruSablon[] donusDeger = new BasvuruSablon[0];
            try
            {
                donusDeger = TRMHibeBasvuru.ListeleSablon(null, kullanan, kriter);
            }
            catch (Exception e)
            {
            }
            return donusDeger;
        }

        public Sonuc BasvuruSablonAktar(Kullanici kullanan, int turKod, int eskiEtap, int yeniEtap)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeBasvuru.AktarSablon(kullanan, turKod, eskiEtap, yeniEtap);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BasvuruSablonSil(Kullanici kullanan, BasvuruSablon bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeBasvuru.SilSablon(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc OnlineBasvuruKaydet(OnlineBasvuruKaydi bilgi)
        {
            try
            {
                return TRMHibeBasvuru.KaydetOnlineBasvuru(null, bilgi);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }



        public Sonuc OnlineBasvuruAktivasyon(string kod, int aktif)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeBasvuru.KaydetOnlineBasvuruAktivasyon(null, kod, aktif);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc OnlineBasvuruParola(OnlineBasvuruKaydi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeBasvuru.KaydetOnlineBasvuruParola(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public string OnlineBasvuruAktivasyon(OnlineBasvuruKaydi bilgi)
        {
            string donusDeger = "";
            try
            {
                donusDeger = TRMHibeBasvuru.OnlineBasvuruAktivasyonPostasiGonder(bilgi);
            }
            catch (Exception e)
            {
            }
            return donusDeger;
        }

        public Sonuc OnlineBasvuruSil(OnlineBasvuruKaydi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeBasvuru.SilOnlineBasvuru(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc OnlineBasvuruGuncelleParola(Kullanici kullanan, OnlineBasvuruKaydi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeBasvuru.GuncelleOnlineBasvuruParola(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public OnlineBasvuruKaydi[] OnlineBasvuruListele(OnlineBasvuruKaydi kriter)
        {
            try
            {
                return TRMHibeBasvuru.ListeleOnlineBasvuru(null, kriter);
            }
            catch
            {
                return new OnlineBasvuruKaydi[0];
            }
        }

        public ObjectArray KirsalKalkinmaYatirimTesisMakineSulamaOdenek(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().KirsalKalkinmaYatirimTesisMakineSulamaOdenek(kullanan, etapNo, ilKod, t1, t2);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray KKYDPProjelerMandaAyri(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().KKYDPProjelerMandaAyri(kullanan, etapNo, ilKod, t1, t2);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray KKYDPIlTurBazliProjeler(Kullanici kullanan, int etapNo, int ilTur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().KKYDPIlTurBazliProjeler(kullanan, etapNo, ilTur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray KKYDPIlTurBazliBitenProjeler(Kullanici kullanan, int etapNo, int ilTur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().KKYDPIlTurBazliBitenProjeler(kullanan, etapNo, ilTur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray ProjeNiteligineGoreHibeTutarDagilimi(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().ProjeNiteligineGoreHibeTutarDagilimi(kullanan, etapNo, ilKod, t1, t2);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ArrayList MakineSulamaOdenen(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            ArrayList donusDeger = new ArrayList();
            try
            {
                donusDeger = new TRMRapor().MakineSulamaOdenen(kullanan, etapNo, ilKod, t1, t2);
            }
            catch (Exception e)
            {
            }
            return donusDeger;
        }

        public ArrayList HibeYatirimDegerlendirmeSayilariniGetir(Kullanici kullanan, int etapNo, string ilKod, int yatirimOncelikTur, string altYapiGrup)
        {
            ArrayList donusDeger = new ArrayList();
            try
            {
                donusDeger = new TRMHibeYatirim().DegerlendirmeSayilariniGetir(kullanan, etapNo, ilKod, yatirimOncelikTur, altYapiGrup);
            }
            catch (Exception e)
            {
            }
            return donusDeger;
        }

        public ArrayList HibeYatirimGenelDurum(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            ArrayList donusDeger = new ArrayList();
            try
            {
                donusDeger = new TRMRapor().HibeYatirimGenelDurum(kullanan, etapNo, ilKod, t1, t2);
            }
            catch (Exception e)
            {
            }
            return donusDeger;
        }

        public ArrayList HibeYatirimTBasincliSulamaGenelDurum(Kullanici kullanan, int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            ArrayList donusDeger = new ArrayList();
            try
            {
                donusDeger = new TRMRapor().TBasincliSulamaGenelDurum(kullanan, etapNo, ilKod, t1, t2);
            }
            catch (Exception e)
            {
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimYonetimKuruluKaydet(Kullanici kullanan, YonetimKurulu[] yonetimKurulu)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().YonetimKuruluKaydet(kullanan, yonetimKurulu);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeYatirimYonetimKuruluListele(Kullanici kullanan, int hibeKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().YonetimKuruluListele(kullanan, hibeKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc HibeYatirimOrtaklarKaydet(Kullanici kullanan, Ortaklar[] ortaklar)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().OrtaklarKaydet(kullanan, ortaklar);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeYatirimOrtaklarListele(Kullanici kullanan, int hibeKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().OrtaklarListele(kullanan, hibeKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimFonKaynakKaydet(Kullanici kullanan, YonetimFonKaynak[] yonetimFonKaynak)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().FonKaynakKaydet(kullanan, yonetimFonKaynak);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeYatirimFonKaynakListele(Kullanici kullanan, int hibeKod, int fonTip)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().FonKaynakListele(kullanan, hibeKod, fonTip);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimGiderleriKaydet(Kullanici kullanan, YatirimGiderlerTablosu[] giderTablosu)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().GiderleriKaydet(kullanan, giderTablosu);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeYatirimGiderleriListele(Kullanici kullanan, YatirimGiderlerTablosu giderTablosu)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().GiderleriListele(kullanan, giderTablosu);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimSureEylemPlaniKaydet(Kullanici kullanan, YatirimSureEylemPlani[] plan)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().SureEylemPlaniKaydet(kullanan, plan);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeYatirimSureEylemPlaniListele(Kullanici kullanan, YatirimSureEylemPlani plan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().SureEylemPlaniListele(kullanan, plan);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimIsTakvimiKaydet(Kullanici kullanan, YatirimIsTakvimi[] plan)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().IsTakvimiKaydet(kullanan, plan);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeYatirimIsTakvimiListele(Kullanici kullanan, YatirimIsTakvimi plan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().IsTakvimiListele(kullanan, plan);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimProjeButcesiKaydet(Kullanici kullanan, YatirimProjeButcesi[] butce)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().ProjeButcesiKaydet(kullanan, butce);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeYatirimProjeButcesiListele(Kullanici kullanan, YatirimProjeButcesi butce)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().ProjeButcesiListele(kullanan, butce);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimFinansmanKaynagiKaydet(Kullanici kullanan, YatirimFinansmanKaynagi[] kaynak)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().FinansmanKaynagiKaydet(kullanan, kaynak);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeYatirimFinansmanKaynagiListele(Kullanici kullanan, YatirimFinansmanKaynagi kaynak)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirimBasvuruTablolari().FinansmanKaynagiListele(kullanan, kaynak);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimYatirimciyiOdemedenCikart(Kullanici kullanan, TNS.TRM.HibeYatirimHakedis hakedis)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeYatirimHakedis.YatirimciyiOdemedenCikart(kullanan, hakedis);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray IlDegerlendirmeKomisyonuListele(Kullanici kullanan, IlDegerlendirmeKomisyonu komisyon)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeDegerlendirmeKomisyonu().ListeleIlDegerlendirmeKomisyonu(kullanan, komisyon);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc IlDegerlendirmeKomisyonuKaydet(Kullanici kullanan, IlDegerlendirmeKomisyonu[] komisyon)
        {
            try
            {
                return new TRMHibeDegerlendirmeKomisyonu().IlDegerlendirmeKomisyonuKaydet(kullanan, komisyon);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc IlDegerlendirmeKomisyonuSil(Kullanici kullanan, IlDegerlendirmeKomisyonu[] komisyon)
        {
            try
            {
                return new TRMHibeDegerlendirmeKomisyonu().SilIlDegerlendirmeKomisyonu(kullanan, komisyon);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }


        public Sonuc IlDegerlendirmeKomisyonunuKilitle(Kullanici kullanan, int kod, int durum)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeDegerlendirmeKomisyonu().KilitleIlDegerlendirmeKomisyonu(kullanan, kod, durum);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TUIKIlceListele(Kullanici kullanan, string ilAd, string ilceAd)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().IlceListele(kullanan, ilAd, ilceAd);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public double TUIKIlceNormListele(Kullanici kullanan, string ilAd, string ilceAd, int tip)
        {
            double donusDeger = 0;
            try
            {
                donusDeger = new TRMRapor().IlceNormListele(kullanan, ilAd, ilceAd, tip);
            }
            catch (Exception e)
            {
                donusDeger = 0;
            }
            return donusDeger;
        }

        public ObjectArray TUIKIlListele(Kullanici kullanan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().IlListele(kullanan);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray TUIKIlceBilgiListele(Kullanici kullanan, int tip, string ilAd, string ilce)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().IlceBilgiListele(kullanan, tip, ilAd, ilce);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TUIKIlIlceGuncelle(Kullanici kullanan, string ilAd, string ilce, double sulunorm, double suluyeninorm, double kurunorm, double kuruyeninorm)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().lIlceGuncelle(kullanan, ilAd, ilce, sulunorm, suluyeninorm, kurunorm, kuruyeninorm);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TUIKOzetListele(Kullanici kullanan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMRapor().OzetListele(kullanan);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimSozlesmeFesihKaydet(Kullanici kullanan, HibeYatirimSozlesmeFesih bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().KaydetSozlesmeFesih(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HibeYatirimSozlesmeFesihListele(Kullanici kullanan, HibeYatirimSozlesmeFesih bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirim().ListeleSozlesmeFesih(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeYatirimSozlesmeFesihSil(Kullanici kullanan, HibeYatirimSozlesmeFesih bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeYatirim().SilSozlesmeFesih(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HibeYatirimSozlesmeFesihRaporListele(Kullanici kullanan, HibeYatirimSozlesmeFesih bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirim().ListeleSozlesmeFesihRapor(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HibeYatirimSozlesmeHakRaporListele(Kullanici kullanan, HibeYatirim bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirim().ListeleSozlesmeHakKazananlar(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray HibeYatirimIldeReddilenlerRaporListele(Kullanici kullanan, HibeYatirim bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMHibeYatirim().ListeleIldeReddilenler(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeSulamaDigerSil(Kullanici kullanan, int hibeKod, string parselNo, string hibeIlKod, int sulamaGrupKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulamaDiger().Sil(ve, kullanan, hibeKod, parselNo, hibeIlKod, sulamaGrupKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HibeSulamaParselReddet(Kullanici kullanan, int hibeKod, string parselNo, string hibeIlKod, int sulamaGrupKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMHibeSulamaDiger().ParselReddet(ve, kullanan, hibeKod, parselNo, hibeIlKod, sulamaGrupKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc VeriToplamaKaydet(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMVeriToplama().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray VeriToplamaListele(Kullanici kullanan, VeriToplama kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMVeriToplama().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc VeriToplamaSil(Kullanici kullanan, VeriToplama bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMVeriToplama().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimMalzemeKaydet(Kullanici kullanan, YatirimMalzeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeYatirimMalzeme.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public YatirimMalzeme[] YatirimMalzemeListele(Kullanici kullanan, YatirimMalzeme kriter)
        {
            YatirimMalzeme[] donusDeger = null;
            try
            {
                donusDeger = TRMHibeYatirimMalzeme.Listele(null, kullanan, kriter);
            }
            catch
            {
                OrtakFonksiyonlar.HataStrYaz("YatirimMalzemeListele hata");
            }

            return donusDeger;
        }

        public YatirimMalzeme[] YatirimMalzemeListeleIstatistik(Kullanici kullanan, YatirimMalzeme kriter)
        {
            YatirimMalzeme[] donusDeger = null;
            try
            {
                donusDeger = TRMHibeYatirimMalzeme.ListeleIstatistik(null, kullanan, kriter);
            }
            catch { }

            return donusDeger;
        }

        public Sonuc YatirimMalzemeSil(Kullanici kullanan, YatirimMalzeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TRMHibeYatirimMalzeme.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        //Yatırım Makine
        public ObjectArray YatirimMakinaTanimListele(Kullanici kullanan, Malzeme kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimMakinaTanim().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray YatirimMakinaTanimOzellikListele(Kullanici kullanan, int malzemeGrupKod, int malzemeKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimMakinaTanim().ListeleMakinaTanimOzellik(kullanan, malzemeGrupKod, malzemeKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray YatirimMakinaTanimVeOzellikleriniListele(Kullanici kullanan, Malzeme m)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimMakinaTanim().ListeleMakinaTanimVeOzellikleri(kullanan, m);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc YatirimMakinaTanimKaydet(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMakinaTanim().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc YatirimMakinaTanimSil(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMakinaTanim().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc YatirimMakinaOzelligiKaydet(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMakineOzelligi().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray YatirimMakinaOzelligiListele(Kullanici kullanan, Malzeme kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimMakineOzelligi().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc YatirimMakinaOzelligiSil(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMakineOzelligi().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc YatirimMalzemeGrubuKaydet(Kullanici kullanan, YatirimMalzemeTur bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMalzemeGrubu().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray YatirimMalzemeGrubuListele(Kullanici kullanan, YatirimMalzemeTur kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimMalzemeGrubu().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc YatirimMalzemeGrubuSil(Kullanici kullanan, YatirimMalzemeTur bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMalzemeGrubu().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc YatirimAnaMalzemeGrubuKaydet(Kullanici kullanan, YatirimMalzemeTur bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMalzemeGrubu().KaydetAna(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray YatirimAnaMalzemeGrubuListele(Kullanici kullanan, YatirimMalzemeTur kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TRMYatirimMalzemeGrubu().ListeleAna(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc YatirimAnaMalzemeGrubuSil(Kullanici kullanan, YatirimMalzemeTur bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TRMYatirimMalzemeGrubu().SilAna(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
    }

    public class TRMYetki : TNS.KYM.IYetki
    {
        public Sonuc YetkiKontrolu(Kullanici kullanan, ENUMIslemTuru islemTur, string oncekiKaydeden, int saymanlikKod, string birimKod, string kurumsalKod, string ekBilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI)))
                sonuc.DegerAta("Bu işlemi yapmak için yetkiniz yok...");
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }
    }
}



namespace TNS
{
    /// <summary>
    /// Yetki tipi bilgileriyle ilgili yordamları tutan sınıf.
    /// </summary>
    public class KYMListe : YetkiTipiListesi
    {
        /// <summary>
        /// Evrak birimlerini ListeSatir nesne dizisi olarak veritabanından getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tip">Tip bilgisini içeren nesne</param>
        /// <returns>ListeSatir nesne dizisi olarak döndürülür.</returns>
        public static ListeSatir[] IlListesi()
        {
            IBKDServis servisBKD = TNS.BKD.Arac.Tanimla();
            ABSKodlarVAN birimVan = servisBKD.KodListeleSeviye(0, "20", "01", 3, true, true);

            ListeSatir[] donusDeger = new ListeSatir[birimVan.absKodlar.Count];

            int i = 0;
            foreach (ABSKod kod in birimVan.absKodlar)
            {
                donusDeger[i++] = new ListeSatir(kod.kod, kod.Ad());
            }

            return donusDeger;
        }

        /// <summary>
        /// Yetki listesini ListeSatir nesne dizisi olarak getiren yordam.
        /// </summary>
        /// <returns>
        /// ListeSatir nesne dizisi olarak döndürülür.
        /// </returns>
        public static ListeSatir[] YetkiListesiDestekBasvuru()
        {
            ListeSatir[] lv = new ListeSatir[3];
            lv[0] = new ListeSatir("1", "Okuma");
            lv[1] = new ListeSatir("2", "Yazma");
            lv[2] = new ListeSatir("3", "Onay");

            return lv;
        }

        /// <summary>
        /// Islems the listesi destek basvuru.
        /// </summary>
        /// <returns></returns>
        public static ListeSatir[] IslemListesiDestekBasvuru(Kullanici kullanan, int kod)
        {
            string[] yetkiKod = { "1", "2", "3" };
            string[] yetkiAd = { "Makine Ekipman", "Sulama", "Ekonomik Yatırım" };

            ListeSatir[] lv = new ListeSatir[0];

            if (kod > 0)
            {
                lv = new ListeSatir[1];
                lv[0] = new ListeSatir(yetkiKod[kod - 1], yetkiAd[kod - 1]);
                return lv;
            }


            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                lv = new ListeSatir[yetkiKod.Length];
                for (int i = 0; i < yetkiKod.Length; i++)
                    lv[i] = new ListeSatir(yetkiKod[i], yetkiAd[i]);
            }
            else
            {
                string[] yetki = kullanan.YetkiTipiDegeriBul("Destek Basvuru Islem Yetkisi").Split(';');
                lv = new ListeSatir[yetki.Length];
                int sayac = 0;
                for (int i = 0; i < yetkiKod.Length; i++)
                {
                    if (TRM.Yetki.YetkisiVarMiDestekTurune(kullanan, ((TRM.ENUMHibeTur)OrtakFonksiyonlar.ConvertToInt(yetkiKod[i], 0))))
                    {
                        lv[sayac] = new ListeSatir(yetkiKod[i], yetkiAd[i]);
                        sayac++;
                    }
                }
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
        /// <returns>
        /// ListeSatir nesne dizisi olarak döndürülür.
        /// </returns>
        public ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim.Equals("12"))
                return IlListesi();
            else if (ekParamTanim.Equals("13"))
                return YetkiListesiDestekBasvuru();
            else if (ekParamTanim.Equals("14"))
                return IslemListesiDestekBasvuru(kullanan, OrtakFonksiyonlar.ConvertToInt(ekParamCagri, 0));
            throw new NotImplementedException("Bilinmeyen parametre!");
        }
    }
}
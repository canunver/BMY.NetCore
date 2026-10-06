using System;
using System.Collections.Generic;
using System.Threading;
using TNS.BKD;
using TNS.BUM;
using TNS.KYM;
using TNS.MUH;

namespace TNS.KHM.IsKurallari
{
    /// <summary>
    /// Kesin hesap ait işlemleri yürüten tüm fonksiyonları yöneten sınıf
    /// </summary>
    [Serializable()]
    public class KHMServis : MarshalByRefObject, TNS.KHM.IKHMServis
    {
        public static bool tabloDegisimYap = true;
        static KHMServis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.KHM.IsKurallari.KHMTabloDegisiklikleri.Kaydet();
                }
            }
        }

        /// <summary>
        /// İta amirlerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="derece">Derece bilgisi.</param>
        /// <returns></returns>
        public TNSCollection<TNS.KHM.KHItaAmiri> ItaAmirleriListele(Kullanici kullanan, int yil, string kurum, int derece)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMIslemlerServisi().ItaAmirleriListele(null, kullanan, yil, kurum, derece);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// İta amirlerini kayıt eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="derece">Derece bilgisi.</param>
        /// <param name="detaylar">KHItaBilgileri tipinde işlem yapılacak bilgileri taşıyan nesne.</param>
        /// <returns></returns>
        public Sonuc ItaAmirleriKaydet(Kullanici kullanan, int yil, string kurum, int derece, TNSCollection detaylar)
        {
            try
            {
                return new KHMIslemlerServisi().ItaAmirleriKaydet(null, kullanan, yil, kurum, derece, detaylar);

            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Kesin hesap gider bilgilerini hesaplayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="khBilgiler">kaydedilecek TertipBilgi nesnelerini içeren koleksiyon.</param>
        /// <returns></returns>
        public Sonuc KesinHesapGiderBilgileriKaydet(Kullanici kullanan, int yil, TNSCollection khBilgiler)
        {
            try
            {
                return KHMGiderServisi.KesinHesapGiderBilgileriKaydet(null, kullanan, yil, khBilgiler);

            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Gelir bilgilerini hesaplayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public Sonuc KesinHesapGelirBilgileriHesapla(Kullanici kullanan, int yil, int ay, Tertip tertip)
        {
            try
            {
                return new KHMGelirServisi().KesinHesapGelirBilgileriHesapla(null, kullanan, yil, ay, tertip);

            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Gelir cetveli hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="dinamikMi">Eğer <c>true</c> dinamik olarak üretilir.</param>
        /// <returns></returns>
        public TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapGelirCetveliHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip, bool dinamikMi)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGelirServisi().KesinHesapGelirCetveliHazirla(null, kullanan, yil, ay, tertip, dinamikMi, 0);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Gelir cetveli (çok yıllı) raporunu hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="dinamikMi">Eğer <c>true</c> dinamik olarak üretilir.</param>
        /// <param name="kacYil">Kaç yıllık işlem yapılacağı bilgisi.</param>
        /// <returns></returns>
        public TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapGelirCetveliHazirlaCokYilli(Kullanici kullanan, int yil, int ay, Tertip tertip, bool dinamikMi, int kacYil)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGelirServisi().KesinHesapCokYilliGelirCetveliHazirla(null, kullanan, yil, ay, tertip, dinamikMi, kacYil);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap mizan raporu hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="gecici">Eğer <c>true</c> geçici Mizan alınır.</param>
        /// <returns></returns>
        public TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapMizanHazirla(Kullanici kullanan, int yil, string kurum, bool gecici)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMMizanServisi().KesinHesapMizanHazirla(null, kullanan, yil, kurum, gecici);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Gider gerekcesi getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public GiderGerekceVAN GiderGerekceGetir(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return new KHMIslemlerServisi().GiderGerekceGetir(null, kullanan, yil, tertip);
            }
            catch (Exception e)
            {
                GiderGerekceVAN a = new GiderGerekceVAN();
                a.sonuc.islemSonuc = false;
                a.sonuc.hataStr = e.ToString();
                return a;
            }
        }

        /// <summary>
        /// Gider gerekcesi getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public GiderGerekceDetayVAN GiderGerekceDetayGetir(Kullanici kullanan, int yil, Tertip tertip)
        {
            VeriTabaniDegistir();
            try
            {
                return new KHMIslemlerServisi().GiderGerekceDetayGetir(null, kullanan, yil, tertip);
            }
            catch (Exception e)
            {
                GiderGerekceDetayVAN a = new GiderGerekceDetayVAN(yil);
                a.sonuc.islemSonuc = false;
                a.sonuc.hataStr = e.ToString();
                return a;
            }
        }

        /// <summary>
        /// Gider gerekcesi listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public ObjectArray<TNS.KHM.GiderGerekce> GiderGerekceListele(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return new KHMIslemlerServisi().GiderGerekceListele(null, kullanan, yil, tertip);
            }
            catch (Exception e)
            {
                ObjectArray a = new ObjectArray();
                a.sonuc.islemSonuc = false;
                a.sonuc.hataStr = e.ToString();
                return a;
            }
        }

        /// <summary>
        /// Gider gerekcesi listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public ObjectArray<TNS.KHM.GiderGerekceDetay> GiderGerekceListeleDetay(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return new KHMIslemlerServisi().GiderGerekceListeleDetay(null, kullanan, yil, tertip);
            }
            catch (Exception e)
            {
                ObjectArray a = new ObjectArray();
                a.sonuc.islemSonuc = false;
                a.sonuc.hataStr = e.ToString();
                return a;
            }
        }

        /// <summary>
        /// Gelir gerekcesi getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public GelirGerekceVAN GelirGerekceGetir(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return new KHMIslemlerServisi().GelirGerekceGetir(null, kullanan, yil, tertip);
            }
            catch (Exception e)
            {
                GelirGerekceVAN a = new GelirGerekceVAN();
                a.sonuc.islemSonuc = false;
                a.sonuc.hataStr = e.ToString();
                return a;
            }
        }

        /// <summary>
        /// Gider gerekçesi kayıt eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="gerekce">İşlem yapılacak bilgi.</param>
        /// <returns></returns>
        public Sonuc GiderGerekceKaydet(Kullanici kullanan, int yil, GiderGerekce gerekce)
        {
            try
            {
                return new KHMIslemlerServisi().GiderGerekceKaydet(null, kullanan, yil, gerekce);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Gider gerekçesi kayıt eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="gerekce">İşlem yapılacak bilgi.</param>
        /// <returns></returns>
        public Sonuc GiderGerekceDetayKaydet(Kullanici kullanan, int yil, GiderGerekceDetay gerekce)
        {
            try
            {
                return new KHMIslemlerServisi().GiderGerekceDetayKaydet(null, kullanan, yil, gerekce);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Gelir gerekçesi kayıt eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="gerekce">İşlem yapılacak bilgi.</param>
        /// <returns></returns>
        public Sonuc GelirGerekceKaydet(Kullanici kullanan, int yil, GelirGerekce gerekce)
        {
            try
            {
                return new KHMIslemlerServisi().GelirGerekceKaydet(null, kullanan, yil, gerekce);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Gider gerekçesi silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public Sonuc GiderGerekceSil(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return new KHMIslemlerServisi().GiderGerekceSil(null, kullanan, yil, tertip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Gider gerekçesi silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public Sonuc GiderGerekceSilDetay(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return new KHMIslemlerServisi().GiderGerekceSilDetay(null, kullanan, yil, tertip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Gelir gerekçesi silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public Sonuc GelirGerekceSil(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return new KHMIslemlerServisi().GelirGerekceSil(null, kullanan, yil, tertip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Yil içi iptal listelesi raporunu hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public TNSCollection<TNS.KHM.TertipOdenekBilgi> YilIciIptalListele(Kullanici kullanan, int yil, string[] kodlar)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMIslemlerServisi().YilIciIptalListele(null, kullanan, yil, kodlar);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Yıl ici iptal bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="detaylar">TertipOdenekBilgi tipinde işlem yapılacak bilgileri taşıyan nesne.</param>
        /// <returns></returns>
        //public Sonuc YilIciIptalKaydet(Kullanici kullanan, int yil, TNSCollection detaylar)
        //{
        //    try
        //    {
        //        return new KHMIslemlerServisi().YilIciIptalKaydet(null, kullanan, yil, detaylar);

        //    }
        //    catch (Exception e)
        //    {
        //        return new Sonuc(false, e.Message, "");
        //    }
        //}

        /// <summary>
        /// Devir akreditif (Kesin Hesap) kaydetme işlemini yapan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="detaylar">TertipOdenekBilgi tipinde işlem yapılacak bilgileri taşıyan nesne.</param>
        /// <returns></returns>
        //public Sonuc KHDevirAkreditifKaydet(Kullanici kullanan, int yil, string kurum, TNSCollection detaylar)
        //{
        //    try
        //    {
        //        return new KHMIslemlerServisi().KHDevirAkreditifKaydet(null, kullanan, yil, kurum, detaylar);

        //    }
        //    catch (Exception e)
        //    {
        //        return new Sonuc(false, e.Message, "");
        //    }
        //}

        /// <summary>
        /// Kesin hesap gider cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapGiderCetveliHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapGiderCetveliHazirla(null, kullanan, yil, ay, tertip);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Devir akreditif (Bütçe İşlemleri) listelesini hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="akreditifMi">Eğer <c>true</c> olursa akreditif işlemi yapılır.</param>
        /// <returns></returns>
        public TNSCollection<TNS.KHM.TertipOdenekBilgi> BIDevirAkreditifListele(Kullanici kullanan, int yil, string[] kodlar, bool akreditifMi)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMIslemlerServisi().BIDevirAkreditifListele(null, kullanan, yil, kodlar, ENUMFormDurumGrup.ONAYLI, akreditifMi);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Devir akreditif (Kesin hesap) listesini hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="islem">İşlem bilgisi.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public TNSCollection<TNS.KHM.TertipOdenekBilgi> KHDevirAkreditifListele(Kullanici kullanan, int yil, string[] islem, string[] kodlar)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMIslemlerServisi().KHDevirAkreditifListele(null, kullanan, yil, islem, kodlar);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap Çapraz cetvel hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public TNSCollection<TNS.KHM.TertipBilgi> KesinHesapCaprazCetvelHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapCaprazCetvelHazirla(null, kullanan, yil, ay, tertip);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap gider cetvelini (kurumsal) hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="kurumsal">Eğer <c>true</c> kurumsal bazda liste alınır.</param>
        /// <returns></returns>
        public TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapGiderCetveliHazirlaKurumsal(Kullanici kullanan, int yil, int ay, Tertip tertip, bool kurumsal)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapGiderCetveliHazirla(null, kullanan, yil, ay, tertip, kurumsal, ENUMIslemKod.TANIMSIZ);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap gider cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="kurumsal">Eğer <c>true</c> kurumsal bazda liste alınır.</param>
        /// <param name="raporTur">Tür bilgisi.</param>
        /// <returns></returns>
        public TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapGiderCetveliHazirlaRaporTur(Kullanici kullanan, int yil, int ay, Tertip tertip, bool kurumsal, ENUMIslemKod raporTur)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapGiderCetveliHazirla(null, kullanan, yil, ay, tertip, kurumsal, raporTur);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Devir cetveli raporunu hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapDevirCetveliHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapDevirCetveliHazirla(null, kullanan, yil, ay, tertip);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap Gider Çok yıllı raporu hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="tur">Tür.</param>
        /// <param name="seviye">Seviye bilgisi.</param>
        /// <param name="kacYil">Kaç yıllık işlem yapılacağı bilgisi.</param>
        /// <returns></returns>
        public TNSCollection<TNS.KHM.TertipBilgiler> KesinHesapGiderCokYilli(Kullanici kullanan, int yil, int ay, Tertip tertip, string tur, int seviye, int kacYil)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapCokYilliCetvelHazirla(null, kullanan, yil, ay, tertip, tur, seviye, kacYil);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap fonksiyonel cetvelibi hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapFonksiyonelCetvelHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapFonksiyonelCetveliHazirla(null, kullanan, yil, ay, tertip, 1);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap finansal cetvelibi hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapFinansalCetvelHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapFonksiyonelCetveliHazirla(null, kullanan, yil, ay, tertip, 2);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap ekonomik cetvelini hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapEkonomikCetvelHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapFonksiyonelCetveliHazirla(null, kullanan, yil, ay, tertip, 2);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap kod cetvelini hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="tur">Tür bilgisi.</param>
        /// <param name="seviye">seviye bilgisi.</param>
        /// <returns></returns>
        public TNSCollection<TNS.KHM.TertipBilgi> KesinHesapKodCetvelHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip, string tur, int seviye)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapKodCetvelHazirla(null, kullanan, yil, ay, tertip, tur, seviye);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Kesin hesap mahsup cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="tur">Tür bilgisi.</param>
        /// <param name="seviye">Seviye bilgisi.</param>
        /// <returns></returns>
        public TNSCollection<TNS.KHM.TertipHarcamaBilgi> KesinHesapMahsupCetveliHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip, string tur, int seviye)
        {
            TNSCollection donusDeger;
            try
            {
                return new KHMGiderServisi().KesinHesapMahsupCetveliHazirla(null, kullanan, yil, ay, tertip, tur, seviye);
            }
            catch (Exception e)
            {
                donusDeger = new TNSCollection();
                donusDeger.Add(new Sonuc(false, e.Message, ""));
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının görme yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <param name="kurum">Yetki kontrolü yapılacak kurum/birim bilgisi.</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public bool GormeYetkisiVarMi(Kullanici kullanan, string kurum)
        {
            try
            {
                return KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gider Tertip deger bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public ObjectArray<TNS.KHM.TertipBilgi> GiderTertipDegerListele(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return new KHMGiderServisi().GiderTertipDegerListele(kullanan, yil, tertip);

            }
            catch //(Exception e)
            {
                return new ObjectArray();
            }
        }

        /// <summary>
        /// Gider tertiplerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public ObjectArray<TNS.BKD.Tertip> GiderTertipListele(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return new KHMGiderServisi().GiderTertipListele(kullanan, yil, tertip);
            }
            catch //(Exception e)
            {
                return new ObjectArray();
            }
        }

        public string FaaliyetAciklamaBul(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return new KHMIslemlerServisi().FaaliyetAciklamaBul(null, kullanan, yil, tertip);
            }
            catch
            {
                return "";
            }
        }

        public ObjectArray<TNS.KHM.GiderGerekceDetay> FaaliyetAciklamaDamgaSayBul(Kullanici kullanan, Tertip tertip)
        {
            try
            {
                return new KHMIslemlerServisi().FaaliyetAciklamaDamgaSayBul(null, kullanan, tertip);
            }
            catch
            {
                return null;
            }
        }

        private static bool degisimYapildi = false;
        public void VeriTabaniDegistir()
        {
            if (degisimYapildi) return;
            degisimYapildi = true;
            //string sql = "CREATE TABLE KHGIDERGEREKCEDETAYONAY(";
            //sql += "YIL int NOT NULL,";
            //sql += "KURKOD varchar(20) NOT NULL,";
            //sql += "FONKOD varchar(20) NOT NULL,";
            //sql += "FINKOD varchar(20) NOT NULL,";
            //sql += "EKOKOD varchar(20) NOT NULL,";
            //sql += "PEBKOD varchar(20) NOT NULL,";
            //sql += "ONAY int NOT NULL,";
            //sql += "PRIMARY KEY CLUSTERED ( YIL ASC, KURKOD ASC, FONKOD ASC, FINKOD ASC, EKOKOD ASC, PEBKOD ASC))";
            //TNS.Veri.VeriErisim ve = TNS.Veri.VeriErisim.VeriErisimBul(null, "BaglantiSatiriKHM");
            //ve.ExecuteNonquery(sql);
            //ve.VeriErisimKapat();
        }

        public Sonuc GiderGerekceDetayOnayla(Kullanici kullanan, int yil, GiderGerekceDetay gerekce)
        {
            try
            {
                return new KHMIslemlerServisi().GiderGerekceDetayOnayla(null, kullanan, yil, gerekce);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc FaaliyetAciklamaOnayKaldir(Kullanici kullanan, Tertip kistasTertip)
        {
            try
            {
                return new KHMIslemlerServisi().GiderGerekceDetayOnayKaldir(null, kullanan, kistasTertip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// OnBulgu nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBulgu yapısında OnBulgu:  bilgilerini tutan değişken</param>
        public ObjectArray<TNS.KHM.SayBulgu> ListeleOnBulgu(Kullanici kullanan, SayBulgu form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KHMOnBulguGiris.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// OnBulgu nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBulgu yapısında OnBulgu:  bilgilerini tutan değişken</param>
        public ObjectVAN<TNS.KHM.SayBulgu> OkuOnBulgu(Kullanici kullanan, SayBulgu form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KHMOnBulguGiris.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// OnBulgu nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBulgu yapısında OnBulgu:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetOnBulgu(Kullanici kullanan, SayBulgu form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KHMOnBulguGiris.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc BulguBirimBagKur(Kullanici kullanan, int onBulguId, TNS.MUH.MuhasebeBirim muhasebeBirim)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KHMOnBulguGiris.BulguBirimBagKur(null, kullanan, onBulguId, muhasebeBirim);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray<TNS.MUH.MuhasebeBirim> BulguIliskiliBirimListele(Kullanici kullanan, int onBulguId)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KHMOnBulguGiris.ListeleBulguBirim(null, kullanan, onBulguId);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc BulguBirimBagSil(Kullanici kullanan, int onBulguId, MuhasebeBirim muhasebeBirim)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KHMOnBulguGiris.BulguBirimBagSil(null, kullanan, onBulguId, muhasebeBirim);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray<TNS.KHM.SayBulgu> OnBulguCevapListele(Kullanici kullanan, int yil, string birimKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KHMOnBulguGiris.OnBulguCevapListele(null, kullanan, yil, birimKod);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc OnBulguCevapKaydet(Kullanici kullanan, SayBulgu onBulgu)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KHMOnBulguGiris.OnBulguCevapKaydet(null, kullanan, onBulgu);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray<TNS.KHM.BirimGirisIzni> BirimGirisIzniListele(Kullanici kullanan, int yil, int tur)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KHMOnBulguGiris.BirimGirisIzniListele(null, kullanan, yil, tur);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc BirimGirisIzniKaydet(Kullanici kullanan, int yil, int tur, bool izin, TNSCollection birimler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KHMOnBulguGiris.BirimGirisIzniKaydet(null, kullanan, yil, tur, izin, birimler);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }
    }
}

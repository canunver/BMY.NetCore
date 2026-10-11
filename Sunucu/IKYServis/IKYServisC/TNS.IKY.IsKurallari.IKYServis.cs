using System;
using System.Runtime;
using TNS.KYM;
using TNS.IKY;
using TNS.UZY;
using TNS.ORG;
using TNS.Veri;

namespace TNS.IKY.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class IKYServis : MarshalByRefObject, TNS.IKY.IIKYServis, TNS.KYM.IDuyuruListesi
    {
        public static bool tabloDegisimYap = true;

        public IKYServis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.IKY.IsKurallari.IKYTabloDegisiklikleri.Kaydet();
                }
            }
        }

        public Sonuc PersonelKaydet(Kullanici islemiYapan, Personel p)
        {
            try
            {
                return new IKYPersonel().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PersonelTCKimlikNoDegistir(Kullanici islemiYapan, Personel p)
        {
            try
            {
                return new IKYPersonel().TCKimlikNoDegistir(islemiYapan, p);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray PersonelListele(Kullanici kullanan, Personel p, bool hizmetleriOku, bool ozet)
        {
            try
            {
                return IKYPersonel.Listele(null, kullanan, p, ozet, hizmetleriOku);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray PersonelIstatistikListele(Kullanici kullanan, Personel p, int istatistikKod)
        {
            try
            {
                return IKYPersonel.IstatistikListele(null, kullanan, p, istatistikKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray PersonelLojmanliListele(Kullanici kullanan, Personel p)
        {
            try
            {
                return IKYPersonel.PersonelLojmanliListele(null, kullanan, p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray PersonelBulOncekiSonraki(Kullanici kullanan, string sicilNo, bool onceki)
        {
            try
            {
                return new IKYPersonel().PersonelBulOncekiSonraki(null, kullanan, sicilNo, onceki);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        //public ObjectArray PersonelListelePersonelRapor(Kullanici kullanan, Personel p)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new IKYPersonel().ListelePersonelRapor(kullanan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}
        //public ObjectArray PersonelListeleEgitimRapor(Kullanici kullanan, Personel p, PersonelKurs pk)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new IKYPersonel().ListeleEgitimRapor(kullanan, p, pk);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        public ObjectArray PersonelListeleIzinDurum(Kullanici kullanan, PersonelIzinDurum pk)
        {
            try
            {
                return new IKYIzin().ListeleIzinDurum(kullanan, pk);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        //public ObjectArray PersonelListeleUnvanKadro(Kullanici kullanan, Personel p)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new IKYPersonel().ListeleUnvanKadro(kullanan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Personel PersonelMemuriyetTarihleriListele(string tcKimlikNo)
        //{
        //    return new IKYMemuriyet().ListeleMemuriyetTarihleri(tcKimlikNo);
        //}

        public Personel HizmetBelgesiRaporu(Kullanici kullanan, int personelKod)
        {
            return IKYPersonel.HizmetBelgesiRaporu(null, kullanan, personelKod);
        }

        public ObjectArray OgrenimDurumuListesi(Kullanici kullanan, string personelKod, string personelTuru, string organizasyonKopruKodu, int aktiflik)
        {
            return IKYRapor.OgrenimDurumuListesi(null, kullanan, personelKod, personelTuru, organizasyonKopruKodu, aktiflik);
        }

        //public Sonuc PersonelSil(Kullanici islemiYapan, Personel p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new IKYPersonel().Sil(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        /// <summary>
        /// Personel Adres Bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Personel Adres bilgisi, Personel Adres tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc PersonelAdresKaydet(Kullanici islemiYapan, PersonelIletisim p)
        {
            try
            {
                return new IKYPersonel().KaydetIletisim(islemiYapan, p);
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
        /// Personel Adres bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Personel Adres bilgisi, Personel Adres tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public PersonelIletisim PersonelAdresListele(Kullanici islemiYapan, int kod)
        {
            return IKYPersonel.ListeleIletisim(null, islemiYapan, kod);
        }

        public PersonelNufus PersonelNufusListele(Kullanici islemiYapan, int kod)
        {
            return new IKYPersonel().ListeleNufus(islemiYapan, kod);
        }

        /// <summary>
        /// Personel Askerlik Bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Personel Askerlik bilgisi, Personel Askerlik tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc PersonelAskerlikKaydet(Kullanici islemiYapan, PAskerlik p)
        {
            try
            {
                return new IKYAskerlik().Kaydet(islemiYapan, p);
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
        /// Personel Askerlik bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Personel Askerlik bilgisi, Personel Askerlik tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public PAskerlik PersonelAskerlikListele(Kullanici islemiYapan, int kod)
        {
            return new IKYAskerlik().Listele(islemiYapan, kod);
        }

        /// <summary>
        /// Personel resminin kayıt edilmesi işlemini gerçekleştirir
        /// </summary>
        /// <param name="bilgi">Resme ait bilgileri saklar</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc PersonelResimKaydet(PersonelResim bilgi)
        {

            try
            {
                return IKYPersonelResim.Kaydet(null, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Personele ait resmin silinmesi işlemini gerçekleştirir
        /// </summary>
        /// <param name="bilgi">Resme ait bilgileri saklar</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc PersonelResimSil(PersonelResim bilgi)
        {
            try
            {
                return IKYPersonelResim.Sil(null, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Personele ait resim bilgisinin alınması işlemini gerçekleştirir
        /// </summary>
        /// <param name="bilgi">Resme ait bilgileri içerir</param>
        /// <returns>ObjectArray yapısı içinde, uygun olan verileri döndürür.</returns>
        public byte[] PersonelResimGetir(int pKod, int buyukluk, int en, int boy)
        {
            //byte[] donusDeger = null;

            try
            {
                return IKYPersonelResim.Getir(null, pKod, buyukluk, en, boy);
            }
            catch
            {
                //donusDeger.sonuc.islemSonuc = false;
                //donusDeger.sonuc.hataStr = e.ToString();
            }
            return null;
        }

        public Sonuc PersonelBelgeKaydet(Kullanici islemiYapan, PersonelBelgeler belge)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYBelgeler().KaydetBelge(islemiYapan, belge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray PersonelBelgeListele(Kullanici islemiYapan, PersonelBelgeler belge)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = new IKYBelgeler().ListeleBelge(islemiYapan, belge);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc PersonelBelgeListesiGetir(Kullanici islemiYapan, PersonelBelgeler belge)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYBelgeler().ListeleBelgeGetir(islemiYapan, belge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc PersonelBelgeSil(Kullanici islemiYapan, PersonelBelgeler belge)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYBelgeler().SilBelge(islemiYapan, belge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelDilKaydet(Kullanici islemiYapan, PersonelDil p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYDil().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray PersonelDilListele(Kullanici kullanan, PersonelDil p)
        {
            try
            {
                return IKYDil.Listele(null, kullanan, p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PersonelDilSil(Kullanici islemiYapan, PersonelDil p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYDil().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelKursKaydet(Kullanici islemiYapan, PersonelKurs p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYKurs().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray PersonelKursListele(Kullanici kullanan, PersonelKurs p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new IKYKurs().Listele(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc PersonelKursSil(Kullanici islemiYapan, PersonelKurs p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYKurs().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelAileKaydet(Kullanici islemiYapan, PersonelAile p)
        {
            try
            {
                return new IKYAile().KaydetAile(islemiYapan, p);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray PersonelAileListele(Kullanici kullanan, int aileKod, int personelKod, string personelKodlar, string personelTur, string organizasyonKopruKodu, int yukumlu)
        {
            try
            {
                return new IKYAile().ListeleAile(null, kullanan, aileKod, personelKod, personelKodlar, personelTur, organizasyonKopruKodu, yukumlu);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PersonelAileSil(Kullanici islemiYapan, PersonelAile p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYAile().SilAile(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelMalBildirimiKaydet(Kullanici islemiYapan, PersonelMalBildirimi p)
        {
            try
            {
                return IKYPersonel.KaydetMalBildirimi(null, islemiYapan, p);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray PersonelMalBildirimiListele(Kullanici kullanan, PersonelMalBildirimi p)
        {
            try
            {
                return IKYPersonel.ListeleMalBildirimi(null, kullanan, p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectVAN PersonelMalBildirimiOku(Kullanici kullanan, int malBildirimiKod)
        {
            try
            {
                return IKYPersonel.OkuMalBildirimi(null, kullanan, malBildirimiKod);
            }
            catch (Exception e)
            {
                ObjectVAN donusDeger = new ObjectVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PersonelMalBildirimiSil(Kullanici islemiYapan, PersonelMalBildirimi p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYPersonel().SilMalBildirimi(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelCezaOdulKaydet(Kullanici islemiYapan, PersonelCezaOdul p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYCezaOdul().KaydetCezaOdul(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelCezaOdulListele(Kullanici kullanan, PersonelCezaOdul p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new IKYCezaOdul().ListeleCezaOdul(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelCezaOdulSil(Kullanici islemiYapan, PersonelCezaOdul p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYCezaOdul().SilCezaOdul(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelOgrenimKaydet(Kullanici islemiYapan, PersonelOgrenim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYOgrenim().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray PersonelOgrenimListele(Kullanici kullanan, PersonelOgrenim p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new IKYOgrenim().Listele(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc PersonelOgrenimSil(Kullanici islemiYapan, PersonelOgrenim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYOgrenim().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        //public Sonuc PersonelMemuriyetKaydet(Kullanici islemiYapan, PersonelMemuriyet p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new IKYMemuriyet().KaydetMemuriyet(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray PersonelMemuriyetListele(Kullanici kullanan, PersonelMemuriyet p)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new IKYMemuriyet().ListeleMemuriyet(kullanan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc PersonelMemuriyetSil(Kullanici islemiYapan, PersonelMemuriyet p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new IKYMemuriyet().SilMemuriyet(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        public Sonuc AtamaOnayBilgileriniGuncelle(Kullanici islemiYapan, TNSCollection atama, int onayIslem)
        {
            try
            {
                return IKYPersonelKadro.GuncelleOnayBilgileri(null, islemiYapan, atama, onayIslem, true, true);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        //public ObjectArray PersonelAtamaListele(Kullanici kullanan, PersonelKadro p)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new IKYPersonelAtama().ListeleAtama(kullanan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        public Sonuc PersonelKadroKaydet(Kullanici islemiYapan, PersonelKadro p)
        {
            try
            {
                return IKYPersonelKadro.KaydetKadro(null, islemiYapan, p, false);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray PersonelKadroListele(Kullanici kullanan, PersonelKadro p, bool tarihArtan)
        {
            try
            {
                return IKYPersonelKadro.ListeleKadro(null, kullanan, p, tarihArtan, false);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PersonelKadroAyarKaydet(Kullanici islemiYapan, KadroAyar p)
        {
            try
            {
                return IKYPersonelKadro.KaydetKadroAyar(null, islemiYapan, p);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray PersonelKadroAyarListele(Kullanici kullanan, KadroAyar p)
        {
            try
            {
                return IKYPersonelKadro.ListeleKadroAyar(null, kullanan, p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        //public Sonuc BankaKaydet(Kullanici islemiYapan, Banka p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new IKYBanka().Kaydet(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray BankaListele(Banka p)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new IKYBanka().Listele(p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc BankaSil(Kullanici islemiYapan, Banka p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new IKYBanka().Sil(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc BankaSubeKaydet(Kullanici islemiYapan, BankaSube p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new PERBankaSube().Kaydet(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray BankaSubeListele(BankaSube p)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new PERBankaSube().Listele(p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc BankaSubeSil(Kullanici islemiYapan, BankaSube p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new PERBankaSube().Sil(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        public Sonuc IzinTurKaydet(Kullanici islemiYapan, IzinTur p)
        {
            try
            {
                return new PERIzinTur().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray IzinTurListele(IzinTur p)
        {
            try
            {
                return new PERIzinTur().Listele(p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc IzinTurSil(Kullanici islemiYapan, IzinTur p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERIzinTur().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc UnvanKaydet(Kullanici islemiYapan, Unvan p)
        {
            try
            {
                return PERUnvan.Kaydet(null, islemiYapan, p);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray UnvanListele(Unvan p)
        {
            try
            {
                return new PERUnvan().Listele(p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc UnvanSil(Kullanici islemiYapan, Unvan p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERUnvan().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        //public Sonuc SinifKaydet(Kullanici islemiYapan, Sinif p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new PERSinif().Kaydet(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray SinifListele(Sinif p)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new PERSinif().Listele(p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc SinifSil(Kullanici islemiYapan, Sinif p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new PERSinif().Sil(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc NedenKaydet(Kullanici islemiYapan, Neden p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new PERNeden().Kaydet(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray NedenListele(Neden p)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new PERNeden().Listele(p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc NedenSil(Kullanici islemiYapan, Neden p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new PERNeden().Sil(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc UstBirimKaydet(Kullanici islemiYapan, UstBirim p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new PERUstBirim().Kaydet(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray UstBirimListele(UstBirim p)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new PERUstBirim().Listele(p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc UstBirimSil(Kullanici islemiYapan, UstBirim p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new PERUstBirim().Sil(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}


        //public Sonuc KadroKaydet(Kullanici islemiYapan, ObjectArray p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new PERKadro().Kaydet(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray KadroRapor(Kullanici islemiYapan, Kadro p)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new PERKadro().Rapor(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc KadroSil(Kullanici islemiYapan, TNSDateTime tarih)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new PERKadro().Sil(islemiYapan, tarih);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray KadroVizeListele()
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new PERKadro().VizeListele();
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}


        //public Sonuc SozlesmeliBordroTanimKaydet(Kullanici islemiYapan, SozlesmeliBordroTanim p)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new IKYPersonel().KaydetSozlesmeliBordroTanim(islemiYapan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public Sonuc SozlesmeliBordroTanimSil(Kullanici islemiYapan, SozlesmeliBordroTanim kriter)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = new IKYPersonel().SilSozlesmeliBordroTanim(islemiYapan, kriter);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray OgrenimBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = PERPersonelOgrenim.OgrenimBazliPersonelSayilariListele(muhasebeKod, birimKod);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray UnvanBazliPersonelSayilariListele(int organizasyonKod)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = PERUnvan.UnvanBazliPersonelSayilariListele(organizasyonKod);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray SinifBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = PERSinif.SinifBazliPersonelSayilariListele(muhasebeKod, birimKod);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        //public ObjectArray UnvanVeKadroDereceBazliPersonelSayilariListele(Kullanici kullanan, KadroKontenjan kk)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = PERUnvan.UnvanVeKadroDereceBazliPersonelSayilariListele(kullanan, kk);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        public Sonuc KadroKontenjanKaydet(Kullanici kullanan, KadroKontenjan kk)
        {
            try
            {
                return PERKadroKontenjan.Kaydet(null, kullanan, kk, false);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        //public Sonuc KadroKontenjanSil(Kullanici kullanan, KadroKontenjan kk)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = PERKadroKontenjan.Sil(null, kullanan, kk, false);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        public KadroKontenjan[] KadroKontenjanListele(Kullanici kullanan, KadroKontenjan kk, bool detay)
        {
            try
            {
                return PERKadroKontenjan.Listele(null, kullanan, kk, detay);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc PersonelIzinKaydet(Kullanici islemiYapan, PersonelIzin p)
        {
            try
            {
                return new IKYIzin().KaydetIzin(islemiYapan, p, false);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PersonelIzinImzaGuncelle(Kullanici islemiYapan, PersonelIzin p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYIzin().GuncelleIzinImza(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelIzinListele(Kullanici kullanan, PersonelIzin p)
        {
            try
            {
                return new IKYIzin().ListeleIzin(kullanan, p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PersonelIzinSil(Kullanici islemiYapan, PersonelIzin p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYIzin().SilIzin(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelIzinYetkiKontrol(Kullanici islemiYapan, PersonelIzin p, ENUMTARIHCEISLEMTIP islemTip)
        {
            try
            {
                return new IKYIzin().YetkiKontrol(islemiYapan, p, islemTip);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public PersonelIzinGunleri PersonelYilBazliYillikIzinBilgileri(Kullanici islemiYapan, int personelKod, DateTime tarih)
        {
            PersonelIzinGunleri donusDeger = new PersonelIzinGunleri();
            try
            {
                donusDeger = new IKYIzin().PersonelYilBazliYillikIzinBilgileri(islemiYapan, personelKod, tarih);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelIzinDevirKaydet(Kullanici kullanan, PersonelIzinDevir[] pid)
        {
            try
            {
                return new IKYIzin().KaydetIzinDevir(kullanan, pid, false);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray PersonelIzinDevirListele(Kullanici kullanan, PersonelIzinDevir p, int organizasyonKodu)
        {
            try
            {
                return new IKYIzin().ListeleIzinDevir(null, kullanan, p, organizasyonKodu);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PersonelIzinDevirSil(Kullanici islemiYapan, PersonelIzinDevir p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYIzin().SilIzinDevir(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelIzinDevirleriAktar(Kullanici kullanan, PersonelIzinDevir pid, int organizasyonKodu)
        {
            try
            {
                return new IKYIzin().DevirleriAktar(kullanan, pid, organizasyonKodu);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public IzinBilgi PersonelKullanilanIzinHesapla(Kullanici kullanan, int personelKod, int yil, int haricKod)
        {
            return new IKYIzin().KullanilanIzinHesapla(null, kullanan, personelKod, yil, haricKod);
        }

        public int PersonelKendindenKucukKullanilanIzinHesapla(int personelKod, TNSDateTime baslamaTarihi)
        {
            return new IKYIzin().KendindenKucukKullanilanIzinHesapla(personelKod, baslamaTarihi);
        }

        //public ObjectArray PersonelTarihceListele(Kullanici kullanan, PersonelTarihce p)
        //{
        //    try
        //    {
        //        return new IKYPersonel().ListeleTarihce(kullanan, p);
        //    }
        //    catch (Exception e)
        //    {
        //        ObjectArray donusDeger = new ObjectArray();
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        return donusDeger;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="personelTCKimlikNo"></param>
        /// <returns>TARIH,KATEGORI,DEGISENALAN,YENIDEGER,ESKIDEGER,DEGISIKLIKYAPAN</returns>
        public String[] PersonelTarihceListele(Kullanici kullanan, int personelKod)
        {
            String[] donusDeger = null;
            try
            {
                donusDeger = IKYPersonelTarihce.Listele(kullanan, personelKod);
            }
            catch { }

            return donusDeger;
        }

        public Sonuc PersonelTarihceKaydet(Kullanici kullanan, object yeni, object eski, ENUMTARIHCE alan, int pKod, ENUMTARIHCEISLEMTIP islemTipi)
        {
            try
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
                Sonuc donusDeger = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, alan, pKod, islemTipi);
                ve.VeriErisimKapat();
                return donusDeger;
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
        /// Personel nitelik bilgilerini varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Personelin sistemde kayıtlı olması gerekmektedir, eğer kayıtlı değilse hata döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PersonelBilgiBirikimiKaydet(Kullanici kullanan, PersonelBilgiBirikimi personel)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.IKY.IsKurallari.PERNitelik.Kaydet(null, kullanan, personel);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan personel nitelik bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel nitelik listesi döndürülür.</returns>
        public ObjectArray PersonelBilgiBirikimiListele(Kullanici kullanan, PersonelBilgiBirikimi personel)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.IKY.IsKurallari.PERNitelik.Listele(null, kullanan, personel);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SozlukListele(Sozluk p)
        {
            try
            {
                return IKYSozluk.Listele(p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public Sonuc SozlukKaydet(Kullanici islemiYapan, Sozluk p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYSozluk().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc SozlukSil(Kullanici islemiYapan, Sozluk p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYSozluk().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray DoluBosKadroListele(PersonelKadro pk)
        {
            try
            {
                return IKYRapor.DoluBosKadroListele(null, pk);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray KadroRaporlari(Kullanici kullanan, string personelTur, string tip, string personelKod, string organizasyonKopruKodu, int siralamaListesi)
        {
            try
            {
                return IKYRapor.KadroRaporlari(null, kullanan, personelTur, tip, personelKod, organizasyonKopruKodu, siralamaListesi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public ObjectArray BakmaklaYukumluListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int yukumlu, int aktiflik)
        {
            try
            {
                return IKYRapor.BakmaklaYukumluListesi(null, kullanan, personelKod, personelTur, organizasyonKopruKodu, yukumlu, aktiflik);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray IzinListesi(Kullanici kullanan, TNS.IKY.Personel personel)
        {
            try
            {
                return IKYRapor.IzinListesi(null, kullanan, personel);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray KartListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, string raporTuru)
        {
            try
            {
                return IKYRapor.KartListesi(null, kullanan, personelKod, personelTur, organizasyonKopruKodu, raporTuru);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray LojmanListesiRaporu(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, int siteKod, int tahsisTip, int siralamaListesi, int yil1, int yil2)
        {
            try
            {
                return IKYRapor.LojmanListesiRaporu(null, kullanan, personelKod, personelTur, organizasyonKopruKodu, aktiflik, siteKod, tahsisTip, siralamaListesi, yil1, yil2);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        //public ObjectArray LojmanDegisiklikListesiRaporu(Kullanici kullanan, int aktiflik, int siteKod)
        //{
        //    try
        //    {
        //        return IKYRapor.LojmanDegisiklikListesiRaporu(null, kullanan, aktiflik, siteKod);
        //    }
        //    catch (Exception e)
        //    {
        //        ObjectArray donusDeger = new ObjectArray();
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        return donusDeger;
        //    }
        //}

        //public ObjectArray YillarArasiLojmandaOturanlarListesiRaporu(Kullanici kullanan, int aktiflik, int siteKod, int yil1, int yil2)
        //{
        //    try
        //    {
        //        return IKYRapor.YillarArasiLojmandaOturanlarListesiRaporu(null, kullanan, aktiflik, siteKod, yil1, yil2);
        //    }
        //    catch (Exception e)
        //    {
        //        ObjectArray donusDeger = new ObjectArray();
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        return donusDeger;
        //    }
        //}

        public ObjectArray LojmanDaireBazliRaporu(Kullanici kullanan, int siteKod, int daireBosluk)
        {
            try
            {
                return IKYRapor.LojmanDaireBazliRaporu(null, kullanan, siteKod, daireBosluk);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray PersonelSiteKesintiRaporu(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, int yil, int ay, int siteKod)
        {
            try
            {
                return IKYRapor.PersonelSiteKesintiRaporu(null, kullanan, personelKod, personelTur, organizasyonKopruKodu, aktiflik, yil, ay, siteKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        //public ObjectArray PersonelDetayliBilgiGetir(Kullanici kullanan, int kod, string kimlikNo, int eskiKadroKod)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new IKYPersonel().PersonelDetayliBilgiGetir(kullanan, kod, kimlikNo, eskiKadroKod);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }

        //    return donusDeger;
        //}

        public Sonuc PersonelKartKaydet(Kullanici islemiYapan, PersonelKart p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYKart().KaydetKart(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelKartListele(Kullanici kullanan, PersonelKart p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new IKYKart().ListeleKart(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelKartSil(Kullanici islemiYapan, PersonelKart p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYKart().SilKart(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelTerfiKaydet(Kullanici islemiYapan, Terfi p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYPersonelTerfi().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelTerfiListele(Kullanici kullanan, Terfi p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new IKYPersonelTerfi().Listele(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelTerfiSil(Kullanici islemiYapan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYPersonelTerfi().Sil(islemiYapan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc CBVerileriniAktar(Kullanici kullanan, int tip)
        {
            try
            {
                if (tip == 21)
                    return IKYAktarim.TumVeriyiAktar(TNS.KYM.Arac.KimlikBul(kullanan));
                else if (tip == 22)
                    return IKYAktarim.LojmanTanimVerileriniAktar(TNS.KYM.Arac.KimlikBul(kullanan));
                else if (tip == 24)
                    return IKYAktarim.SiteGiderTablosuAktar(TNS.KYM.Arac.KimlikBul(kullanan));
                //else if (tip == 25)
                //    return Veri.TABGenel.PersonelSiteKesintisiAktar(TNS.KYM.Arac.KimlikBul(kullanan));
                else
                    return IKYAktarim.CBVeriAktar(tip, TNS.KYM.Arac.KimlikBul(kullanan));
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public ObjectArray HizmetSebebiListele(ENUMHizmetSebebiNevi sebepler)
        {
            try
            {
                return IKYPersonelKadro.ListeleHizmetSebebi(sebepler);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray EskiHizmetTipiListele()
        {
            try
            {
                return IKYPersonelKadro.ListeleEskiHizmetTipi(null);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray HizmetleriGetir(Kullanici kullanan, string organizasyonKopruKod, string personelKod, string personelTur, TNSDateTime sonTarih, int siralamaListesi, string raporKod, int aktiflik)
        {
            try
            {
                return IKYPersonelKadro.HizmetleriGetir(null, kullanan, organizasyonKopruKod, personelKod, personelTur, sonTarih, siralamaListesi, raporKod, aktiflik);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public ObjectArray TerfisiGelenler(Kullanici kullanan, string yilAy, int tip)
        {
            try
            {
                return new IKYPersonelTerfi().TerfisiGelenler(kullanan, yilAy, tip);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public Sonuc EKEKHATerfiEt(Kullanici kullanan, Terfi terfi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new IKYPersonelTerfi().EKEKHATerfiEt(null, kullanan, terfi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Personel NakilBildirimRaporu(int pkod)
        {
            return new IKYRapor().NakilBildirimRaporu(pkod);
        }

        public ObjectArray UnvanaGoreHizmetGrupla(int personelKod)
        {
            try
            {
                return IKYPersonelKadro.UnvanaGoreHizmetGrupla(null, personelKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public MailImzaRapor MailImzaRaporu(int personelKod)
        {
            return new IKYRapor().MailImzaRaporu(personelKod);
        }

        //public Hizmet HizmetSureBilgileriniGetir(int personelKod, DateTime sonTarih)
        //{
        //    return new IKYPersonelKadro().HizmetSureBilgileriniGetir(personelKod, sonTarih);
        //}

        //public int HizmetGunuDondur(DateTime baslangicT, DateTime bitisT, int kadroKod)
        //{
        //    try
        //    {
        //        return IKYPersonelKadro.HizmetGunuDondur(null, baslangicT, bitisT, kadroKod);
        //    }
        //    catch { }
        //    return 0;
        //}

        ///// <summary>
        ///// Personele ait resim bilgisinin alınması işlemini gerçekleştirir
        ///// </summary>
        ///// <param name="bilgi">Resme ait bilgileri içerir</param>
        ///// <returns>ObjectArray yapısı içinde, uygun olan verileri döndürür.</returns>
        //public ObjectArray SirtlikListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu)
        //{
        //    try
        //    {
        //        return IKYRapor.SirtlikListesi(null, kullanan, personelKod, personelTur, organizasyonKopruKodu);
        //    }
        //    catch (Exception e)
        //    {
        //        ObjectArray donusDeger = new ObjectArray();
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        return donusDeger;
        //    }
        //}

        //public ObjectArray DinamikRaporListesi(string personelKod, string personelTur, string organizasyonKopruKodu, string siralamalar, int aktiflik)
        //{
        //    try
        //    {
        //        return IKYRapor.DinamikRaporListesi(personelKod, personelTur, organizasyonKopruKodu, siralamalar, aktiflik);
        //    }
        //    catch (Exception e)
        //    {
        //        ObjectArray donusDeger = new ObjectArray();
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        return donusDeger;
        //    }
        //}


        /// <summary>
        /// PersonelTipi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelTipi yapısında PersonelTipi: Personel Tipi bilgilerini tutan değişken</param>
        public ObjectArray ListelePersonelTip(Kullanici kullanan, PersonelTipi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYPersonelTipForm.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// PersonelTipi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelTipi yapısında PersonelTipi: Personel Tipi bilgilerini tutan değişken</param>
        public ObjectArray ListelePersonelTipGrup(Kullanici kullanan, bool sadeceKullanilanlar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYPersonelTipForm.ListeleGrup((VeriErisim)null, kullanan, sadeceKullanilanlar);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }


        /// <summary>
        /// PersonelTipi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelTipi yapısında PersonelTipi: Personel Tipi bilgilerini tutan değişken</param>
        public ObjectVAN OkuPersonelTip(Kullanici kullanan, PersonelTipi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYPersonelTipForm.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// PersonelTipi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelTipi yapısında PersonelTipi: Personel Tipi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetPersonelTip(Kullanici kullanan, PersonelTipi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYPersonelTipForm.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public PersonelMaasParam MaasParamListele(Kullanici kullanan, string personelTCKimlikNo, int yil)
        {
            return PERBordroTurGiris.MaasParamListele(null, kullanan, personelTCKimlikNo, yil);
        }

        public Sonuc MaasParamKaydet(Kullanici kullanan, TNSCollection kisiler, TNSCollection degiskenler)
        {
            return PERBordroTurGiris.MaasParamKaydet(null, kullanan, kisiler, degiskenler);
        }

        public Sonuc MaasMatrahKaydet(Kullanici kullanan, int yil, string surecNo, string tabloAd, TNSCollection matrahlar)
        {
            return PERBordroTurGiris.MaasMatrahKaydet(null, kullanan, yil, surecNo, tabloAd, matrahlar);
        }

        public Sonuc MaasMatrahSil(Kullanici kullanan, int yil, string surecNo, string tabloAd, TNSCollection tcListe)
        {
            return PERBordroTurGiris.MaasMatrahSil((VeriErisim)null, kullanan, yil, surecNo, tabloAd, tcListe);
        }

        public ObjectArray MaasMatrahAktar(Kullanici kullanan, TNSDateTime matrahTarih, TNSCollection matrahlar, int yontem, int yukle)
        {
            return PERBordroTurGiris.MaasMatrahAktar(null, kullanan, matrahTarih, matrahlar, yontem, yukle);
        }

        /// <summary>
        /// IKYLOJMANSITE nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IKYLOJMANSITE yapısında IKYLOJMANSITE:  bilgilerini tutan değişken</param>
        public ObjectArray LojmanSiteListele(Kullanici kullanan, LojmanSite form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYLojmanSite.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public ObjectArray SiteYetkilileriListele(Kullanici kullanan, IKYSiteYetki form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYLojmanSite.SiteYetkilileriListele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }


        /// <summary>
        /// LojmanSite nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IKYLOJMANSITE yapısında IKYLOJMANSITE:  bilgilerini tutan değişken</param>
        public ObjectVAN LojmanSiteOku(Kullanici kullanan, LojmanSite form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TNS.IKY.IsKurallari.IKYLojmanSite.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// IKYLOJMANSITE nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IKYLOJMANSITE yapısında IKYLOJMANSITE:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc LojmanSiteKaydet(Kullanici kullanan, LojmanSite form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYLojmanSite.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc LojmanSiteSil(Kullanici islemiYapan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = IKYLojmanSite.Sil(islemiYapan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc LojmanSiteYetkiKaydet(Kullanici kullanan, IKYSiteYetki[] listler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYLojmanSite.SiteYetkiKaydet(kullanan, listler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc LojmanSiteYetkiAktifPasifYap(Kullanici kullanan, IKYSiteYetki yetki)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYLojmanSite.SiteYetkiAktifPasifYap(kullanan, yetki);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// LojmanSiteBlok nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IKYLojmanSiteBlok yapısında IKYLojmanSiteBlok:  bilgilerini tutan değişken</param>
        public ObjectArray LojmanSiteBlokListele(Kullanici kullanan, LojmanSiteBlok form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYLojmanSiteBlok.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// LojmanSiteBlok nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IKYLojmanSiteBlok yapısında IKYLojmanSiteBlok:  bilgilerini tutan değişken</param>
        public ObjectVAN LojmanSiteBlokOku(Kullanici kullanan, LojmanSiteBlok form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TNS.IKY.IsKurallari.IKYLojmanSiteBlok.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// LojmanSiteBlok nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IKYLojmanSiteBlok yapısında IKYLojmanSiteBlok:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc LojmanSiteBlokKaydet(Kullanici kullanan, LojmanSiteBlok form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYLojmanSiteBlok.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc LojmanSiteBlokSil(Kullanici islemiYapan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = IKYLojmanSiteBlok.Sil(islemiYapan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        /// <summary>
        /// LojmanSiteBlokDaire nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSiteBlokDaire yapısında LojmanSiteBlokDaire:  bilgilerini tutan değişken</param>
        public ObjectArray LojmanSiteBlokDaireListele(Kullanici kullanan, LojmanSiteBlokDaire form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYLojmanSiteBlokDaire.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// LojmanSiteBlokDaire nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSiteBlokDaire yapısında LojmanSiteBlokDaire:  bilgilerini tutan değişken</param>
        public ObjectVAN LojmanSiteBlokDaireOku(Kullanici kullanan, LojmanSiteBlokDaire form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TNS.IKY.IsKurallari.IKYLojmanSiteBlokDaire.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// LojmanSiteBlokDaire nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSiteBlokDaire yapısında LojmanSiteBlokDaire:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc LojmanSiteBlokDaireKaydet(Kullanici kullanan, LojmanSiteBlokDaire form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYLojmanSiteBlokDaire.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc LojmanSiteBlokDaireSil(Kullanici islemiYapan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = IKYLojmanSiteBlokDaire.Sil(islemiYapan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray BosDaireListesi(Kullanici kullanan, int blokKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYLojmanSiteBlokDaire.BosDaireListesi(null, kullanan, blokKod);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc PersonelLojmanKaydet(Kullanici islemiYapan, PersonelLojmani p)
        {
            try
            {
                return IKYPersonelLojman.Kaydet(null, islemiYapan, p);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray PersonelLojmanListele(Kullanici kullanan, PersonelLojmani p)
        {
            try
            {
                return IKYPersonelLojman.Listele(null, kullanan, p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PersonelLojmanSil(Kullanici islemiYapan, PersonelLojmani p)
        {
            try
            {
                return IKYPersonelLojman.Sil(null, islemiYapan, p);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray LojmanTurleriniListele(Kullanici kullanan, LojmanTuru p)
        {
            try
            {
                return IKYPersonelLojman.ListeleLojmanTuru(null, kullanan, p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PersonelLojmanRezervdenTahsisEt(Kullanici islemiYapan, PersonelLojmani p)
        {
            try
            {
                return IKYPersonelLojman.RezervdenTahsisEt(null, islemiYapan, p);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray PersonelDoluDaireListesi(Kullanici kullanan, PersonelLojmani p)
        {
            try
            {
                return IKYPersonelLojman.ListelePLojmanK(null, kullanan, p);
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
        /// SozlukGider nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SozlukGider yapısında SozlukGider:  bilgilerini tutan değişken</param>
        public ObjectArray SozlukGiderListele(Kullanici kullanan, SozlukGider form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSiteYonetimi.ListeleSozlukGider(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// SozlukGider nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SozlukGider yapısında SozlukGider:  bilgilerini tutan değişken</param>
        public ObjectVAN SozlukGiderOku(Kullanici kullanan, SozlukGider form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSiteYonetimi.OkuSozlukGider(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// SozlukGider nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SozlukGider yapısında SozlukGider:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SozlukGiderKaydet(Kullanici kullanan, SozlukGider form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSiteYonetimi.KaydetSozlukGider(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SozlukGiderSil(Kullanici islemiYapan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = IKYSiteYonetimi.SilSozlukGider(islemiYapan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// SiteGider nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SiteGider yapısında SiteGider:  bilgilerini tutan değişken</param>
        public ObjectArray SiteGiderListele(Kullanici kullanan, SiteGider form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSiteYonetimi.ListeleSiteGider(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// SiteGider nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SiteGider yapısında SiteGider:  bilgilerini tutan değişken</param>
        public ObjectVAN SiteGiderOku(Kullanici kullanan, SiteGider form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSiteYonetimi.OkuSiteGider(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// SiteGider nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SiteGider yapısında SiteGider:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SiteGiderKaydet(Kullanici kullanan, int siteKod, int yil, int ay, TNSCollection giderBilgiler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSiteYonetimi.KaydetSiteGider(null, kullanan, siteKod, yil, ay, giderBilgiler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SiteGiderSil(Kullanici islemiYapan, int kod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = IKYSiteYonetimi.SilSiteGider(islemiYapan, kod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// PersonelSiteKesinti nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelSiteKesinti yapısında PersonelSiteKesinti:  bilgilerini tutan değişken</param>
        public ObjectArray PersonelSiteKesintisiListele(Kullanici kullanan, PersonelSiteKesinti form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSiteYonetimi.ListeleSiteKesintisi(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// PersonelSiteKesinti nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelSiteKesinti yapısında PersonelSiteKesinti:  bilgilerini tutan değişken</param>
        public ObjectVAN PersonelSiteKesintisiOku(Kullanici kullanan, PersonelSiteKesinti form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSiteYonetimi.OkuSiteKesintisi(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// PersonelSiteKesinti nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelSiteKesinti yapısında PersonelSiteKesinti:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PersonelSiteKesintisiKaydet(Kullanici kullanan, PersonelSiteKesinti form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSiteYonetimi.KaydetSiteKesintisi(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc PersonelSiteKesintisiSil(Kullanici islemiYapan, PersonelSiteKesinti sk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = IKYSiteYonetimi.SilSiteKesintisi(islemiYapan, sk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelSiteKesintisiToplamSil(Kullanici islemiYapan, int toplamKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = IKYSiteYonetimi.SilSiteKesintisiToplam(islemiYapan, toplamKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelKesintileriKaydet(Kullanici islemiYapan, TNSCollection col)
        {
            try
            {
                return IKYSiteYonetimi.PersonelKesintileriKaydet(null, islemiYapan, col);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Komisyon nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Komisyon yapısında Komisyon:  bilgilerini tutan değişken</param>
        public ObjectArray KomisyonListele(Kullanici kullanan, Komisyon form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYKomisyon.ListeleKomisyon(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// Komisyon nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Komisyon yapısında Komisyon:  bilgilerini tutan değişken</param>
        public ObjectVAN KomisyonOku(Kullanici kullanan, Komisyon form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYKomisyon.OkuKomisyon(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// Komisyon nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Komisyon yapısında Komisyon:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KomisyonKaydet(Kullanici kullanan, Komisyon form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYKomisyon.KaydetKomisyon(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KomisyonSil(Kullanici kullanan, int kod)
        {
            try
            {
                return IKYKomisyon.SilKomisyon(kullanan, kod);
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
        /// PersonelSiteKesinti nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelSiteKesinti yapısında PersonelSiteKesinti:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PersonelSiteKesintileriTopluKaydet(Kullanici kullanan, PersonelSiteKesinti[] form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSiteYonetimi.SiteKesintileriTopluKaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public string KullanicininIslemleri(Kullanici kullanan, string kopruKod, MUH.ENUMOGY ogy)
        {
            return TNS.IKY.IsKurallari.IKYYetki.KullanicininIslemleri(kullanan, kopruKod, ogy);
        }

        //public Sonuc ZamanliIsler(Kullanici kullanan, DateTime zaman)
        //{
        //    if (zaman.Minute < 10)
        //        return CBVerileriniAktar(kullanan, 21);
        //    else
        //        return new Sonuc();
        //}

        public ObjectArray PersonelGiderListele(Kullanici kullanan, IKY.IKYPersonelGider gider)
        {
            return IKYPersonelGider.Listele(null, kullanan, gider);
        }

        public Sonuc PersonelGiderKaydet(Kullanici kullanan, IKY.IKYPersonelGider gider, bool yeni)
        {
            return IKYPersonelGider.Kaydet(null, kullanan, gider, yeni);
        }

        //public Sonuc PersonelYemekGuncelleMailGonderildi(Kullanici kullanan, IKY.IKYPersonelGider gider)
        //{
        //    return IKYPersonelGider.GuncelleMailGonderildi(null, kullanan, gider);
        //}

        public Sonuc PersonelGiderAktar(Kullanici kullanan, IKY.IKYPersonelGider gider)
        {
            return IKYPersonelGider.Aktar(null, kullanan, gider);
        }

        //public Sonuc PersonelGiderSil(Kullanici kullanan, IKY.IKYPersonelGider yemek)
        //{
        //    return IKYPersonelGider.Sil(null, kullanan, yemek);
        //}

        //public ObjectArray YerleskelereGoreGiderListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, string yemekhaneler, DateTime baslamaTarihi, DateTime bitisTarihi)
        //{
        //    try
        //    {
        //        return IKYRapor.YemekhanelereGoreYemekListesi(null, kullanan, personelKod, personelTur, organizasyonKopruKodu, aktiflik, yemekhaneler, baslamaTarihi, bitisTarihi);
        //    }
        //    catch (Exception e)
        //    {
        //        ObjectArray donusDeger = new ObjectArray();
        //        donusDeger.sonuc.DegerAta(e.ToString());
        //        return donusDeger;
        //    }
        //}

        //public ObjectArray PersonelHesaplanmisGiderListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, string yemekhaneler, int yil, int ay)
        //{
        //    try
        //    {
        //        return IKYRapor.PersonelHesaplanmisYemekListesi(null, kullanan, personelKod, personelTur, organizasyonKopruKodu, aktiflik, yemekhaneler, yil, ay);
        //    }
        //    catch (Exception e)
        //    {
        //        ObjectArray donusDeger = new ObjectArray();
        //        donusDeger.sonuc.DegerAta(e.ToString());
        //        return donusDeger;
        //    }
        //}

        internal static string MinimumKopruKodGetir()
        {
            OrganizasyonBirim ob = TNS.ORG.Arac.GirisOrgBirimBul();
            if (ob == null) return null;
            return ob.kopruKod; //return "303";
        }

        /// <summary>
        /// HizmetSebebi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HizmetSebebi yapısında HizmetSebebi: Hizmet Sebepleri bilgilerini tutan değişken</param>
        public ObjectArray ListeleHizmetSebebi(Kullanici kullanan, HizmetSebebi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYHizmetSebebi.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// HizmetSebebi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HizmetSebebi yapısında HizmetSebebi: Hizmet Sebepleri bilgilerini tutan değişken</param>
        public ObjectVAN OkuHizmetSebebi(Kullanici kullanan, HizmetSebebi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYHizmetSebebi.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// HizmetSebebi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HizmetSebebi yapısında HizmetSebebi: Hizmet Sebepleri bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetHizmetSebebi(Kullanici kullanan, HizmetSebebi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYHizmetSebebi.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// HizmetSebebi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HizmetSebebi yapısında HizmetSebebi: Hizmet Sebepleri bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SilHizmetSebebleri(Kullanici kullanan, TNSCollection formlar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYHizmetSebebi.Sil(null, kullanan, formlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Gorevlendirme nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Gorevlendirme yapısında Gorevlendirme: Görevlendirme bilgilerini tutan değişken</param>
        public ObjectArray ListeleGorevlendirmeler(Kullanici kullanan, Gorevler form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYGorevlendirmeler.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// Gorevlendirme nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Gorevlendirme yapısında Gorevlendirme: Görevlendirme bilgilerini tutan değişken</param>
        public ObjectVAN OkuGorevlendirmeler(Kullanici kullanan, Gorevler form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYGorevlendirmeler.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// Gorevlendirme nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Gorevlendirme yapısında Gorevlendirme: Görevlendirme bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetGorevlendirmeler(Kullanici kullanan, Gorevler form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYGorevlendirmeler.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SilGorevlendirmeler(Kullanici kullanan, Gorevler form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYGorevlendirmeler.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Verilen birimdeki (öncelikle organizasyon kod veya harcama koduna göre) onay bekleyen hizmet hareketlerini listeler.
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="orgKod"></param>
        /// <param name="muhasebeKod"></param>
        /// <param name="muhasebeBirimKod"></param>
        /// <returns></returns>
        public ObjectVAN OnaysizKayitlar(Kullanici kullanan, int orgKod, bool sadeceSayilar)
        {
            return IsKurallari.IKYPersonelKadro.OnaysizKayitlar(null, kullanan, orgKod, sadeceSayilar);
        }

        public int SebepKodBul(ENUMHizmetSebebiNevi sebepTur)
        {
            return IsKurallari.IKYPersonelKadro.NakilSebebiBul(sebepTur);
        }


        public ObjectArray PersonelSozlesmeListele(Kullanici kullanan, PersonelSozlesme form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYPersonelSozlesme.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray ov = new ObjectArray();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        public Sonuc PersonelSozlesmeKaydet(Kullanici kullanan, PersonelSozlesme[] liste)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYPersonelSozlesme.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc PersonelSozlesmeSil(Kullanici kullanan, PersonelSozlesme form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYPersonelSozlesme.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray DuyuruListesiAl(Kullanici kullanan, string kullaniciTipleri)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYGorevlendirmeler.DuyuruListesiAl(null, kullanan, kullaniciTipleri);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc = new Sonuc(e.Message);
                OrtakFonksiyonlar.HataDosyaYaz("Duyuru.txt", e.Message, false);

                return oa;
            }
        }

        public Sonuc TarimVerileriniAktar(string tcKimlikNo)
        {
            try
            {
                OrtakFonksiyonlar.HataStrYaz("TarimVerileriniAktar:" + tcKimlikNo);
                if (string.IsNullOrEmpty(tcKimlikNo))
                    return TarimVerileriAktar.SiraliVeriAktar();
                else
                    return TarimVerileriAktar.VeriAktar(null, null, tcKimlikNo, true);
            }
            catch (Exception e)
            {
                return new Sonuc(e.Message);
            }
        }

        public Sonuc PersonelGiderYukle(Kullanici kullanan, TNSCollection liste)
        {
            return IKYPersonelGider.Yukle(null, kullanan, liste);
        }
        /// <summary>
        /// SendikaBilgi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SendikaBilgi yapısında SendikaBilgi:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleSendikaBilgi(Kullanici kullanan, SendikaBilgi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSendikaBilgi.Listele(null, kullanan, form, false);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }


        /// <summary>
        /// SendikaBilgi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SendikaBilgi yapısında SendikaBilgi:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuSendikaBilgi(Kullanici kullanan, SendikaBilgi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSendikaBilgi.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// SendikaBilgi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SendikaBilgi yapısında SendikaBilgi:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetSendikaBilgi(Kullanici kullanan, SendikaBilgi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSendikaBilgi.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc PersonelSendikadanCikart(Kullanici kullanan, int pKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSendikaBilgi.PersonelSendikadanCikart(null, kullanan, pKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public Sonuc PersonelSendikaOranDegistir(Kullanici kullanan, int pKod, double kesintiOrani)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSendikaBilgi.PersonelSendikaOranDegistir(null, kullanan, pKod, kesintiOrani);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc PersonelSendikayaEkle(Kullanici kullanan, int sendikaKod, TNSCollection pKodList, double kesintiOran, bool yetkiKontroluYap)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSendikaBilgi.PersonelSendikayaEkle(null, kullanan, sendikaKod, pKodList, kesintiOran, yetkiKontroluYap);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectVAN OkuKisiSendikaBilgi(Kullanici kullanan, string personelTCKimlikNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return IKYSendikaBilgi.OkuKisiSendikaBilgi(null, kullanan, personelTCKimlikNo);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc = new Sonuc(false, e.ToString(), "");
                return ov;
            }
        }
    }
}

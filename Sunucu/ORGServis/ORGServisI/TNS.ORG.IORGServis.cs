using System;

using TNS.KYM;

using TNS.UZY;



namespace TNS.ORG

{

    /// <summary>

    /// Yetki bilgilerini tutan enumeration

    /// </summary>

    public enum ENUMOGY

    {

        /// <summary>

        /// Onaylama

        /// </summary>

        Onaylama = 1,

        

        /// <summary>

        /// Görme

        /// </summary>

        Gorme = 2,

    

        /// <summary>

        /// Yazma

        /// </summary>

        Yazma = 3,

    }



    /// <summary>

    /// Bu sunucu sınıfı için, istemci ve sunucuda ortak kullanılacak statik fonksiyonlar içeren sınıf

    /// </summary>

    [Serializable()]

    public static class Arac

    {

        public static string OrgBirimIlIlceAdBul(int orgKod)

        {

            if (orgKod == 0)

                return string.Empty;



            while (true)

            {

                OrganizasyonBirim ob = OrgBirimBul(orgKod);

                if (ob == null)

                    return string.Empty;



                string ilIlceAd = string.Empty;

                if (!string.IsNullOrEmpty(ob.il))

                {

                    IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

                    ilIlceAd = servisUZY.UzayDegeriStr(null, "COGRAFYA", ob.il, true);

                    if (!string.IsNullOrEmpty(ob.ilce))

                        ilIlceAd += " - " + servisUZY.UzayDegeriStr(null, "COGRAFYA", ob.ilce, true);

                    return ilIlceAd;

                }



                if (ob.anaBirimKod == 0)

                    return string.Empty;

                orgKod = ob.anaBirimKod;

            }

        }



        /// <summary>

        /// Orgs the evrak birim bul.

        /// </summary>

        /// <param name="orgKod">The org kod.</param>

        /// <returns></returns>

        public static string OrgEvrakBirimBul(int orgKod)

        {

            OrganizasyonBirim ob = OrgBirimBul(orgKod);

            return ob != null ? ob.evrakbirim : string.Empty;

        }



        /// <summary>

        /// Orgs the antet bul.

        /// </summary>

        /// <param name="kriter">The kriter.</param>

        /// <returns></returns>

        public static string OrgAntetBul(OrganizasyonBirim kriter)

        {

            IORGServis servisORG = Tanimla();

            ObjectArray obler = servisORG.OrganizasyonBirimListele(kriter, false, false);

            if (!obler.sonuc.islemSonuc || obler.objeler.Count <= 0)

                return null;

            else

            {

                string antet = ((OrganizasyonBirim)obler.objeler[0]).antet;

                antet = antet.Replace("\n", "\r\n");

                return antet;

            }

        }



        public static OrganizasyonBirim OrgBirimBul(string entegreKod)

        {

            OrganizasyonBirim kriter = new OrganizasyonBirim();

            kriter.entegreKod = entegreKod;

            kriter.sablonMu = false;



            IORGServis servisORG = Tanimla();

            ObjectArray obler = servisORG.OrganizasyonBirimListele(kriter, false, false);

            if (!obler.sonuc.islemSonuc || obler.objeler.Count <= 0)

                return null;

            return (OrganizasyonBirim)obler.objeler[0];

        }



        public static OrganizasyonBirim IlceBul(int ilceMernisKod)

        {

            IORGServis servisORG = Tanimla();

            ObjectArray obler = servisORG.Il_IlceBul(null, "", ilceMernisKod, false, false);

            if (!obler.sonuc.islemSonuc || obler.objeler.Count <= 0)

                return null;

            return (OrganizasyonBirim)obler.objeler[0];

        }



        public static OrganizasyonBirim IlBul(string plakaKod)

        {

            IORGServis servisORG = Tanimla();

            ObjectArray obler = servisORG.Il_IlceBul(null, plakaKod, 0, false, false);

            if (!obler.sonuc.islemSonuc || obler.objeler.Count <= 0)

                return null;

            return (OrganizasyonBirim)obler.objeler[0];

        }



        /// <summary>

        /// Verilen Organizasyon koduna ait organizasyon birimini bulur

        /// </summary>

        /// <param name="kod">The kod.</param>

        /// <returns></returns>

        public static OrganizasyonBirim OrgBirimBul(int kod)

        {

            OrganizasyonBirim kriter = new OrganizasyonBirim();

            kriter.kod = kod;

            kriter.sablonMu = false;



            IORGServis servisORG = Tanimla();

            ObjectArray obler = servisORG.OrganizasyonBirimListele(kriter, false, false);

            if (!obler.sonuc.islemSonuc || obler.objeler.Count <= 0)

                return null;

            return (OrganizasyonBirim)obler.objeler[0];

        }



        /// <summary>

        /// Verilen muhasebe-birim-kurumsal a ait organizasyon birimini bulur

        /// </summary>

        /// <param name="muhasebe">The muhasebe.</param>

        /// <param name="birim">The birim.</param>

        /// <param name="kurumsal">The kurumsal.</param>

        /// <returns></returns>

        public static OrganizasyonBirim OrgBirimBul(string muhasebe, string birim, string kurumsal)

        {

            OrganizasyonBirim kriter = new OrganizasyonBirim();

            kriter.muhasebekodu = muhasebe;

            kriter.muhasebebirim = birim;

            kriter.kurumsalkod = kurumsal;

            kriter.sablonMu = false;



            IORGServis servisORG = Tanimla();

            ObjectArray obler = servisORG.OrganizasyonBirimListele(kriter, false, false);

            if (!obler.sonuc.islemSonuc || obler.objeler.Count <= 0)

                return null;

            return (OrganizasyonBirim)obler.objeler[0];

        }



        /// <summary>

        /// Verilen muhasebe-birim-kurumsal a ait organizasyon birimini bulur

        /// </summary>

        /// <param name="muhasebe">The muhasebe.</param>

        /// <param name="birim">The birim.</param>

        /// <param name="kurumsal">The kurumsal.</param>

        /// <returns></returns>

        public static OrganizasyonBirim GirisOrgBirimBul()

        {

            OrganizasyonBirim kriter = new OrganizasyonBirim();

            kriter.sablonMu = false;

            kriter.anaBirimKod = -1;



            IORGServis servisORG = Tanimla();

            ObjectArray obler = servisORG.OrganizasyonBirimListele(kriter, false, false);

            if (!obler.sonuc.islemSonuc || obler.objeler.Count <= 0)

                return null;

            return (OrganizasyonBirim)obler.objeler[0];

        }



        /// <summary>

        /// Bu sunucu sınıfının nesnesini .net remoting ile oluşturup döndüren statik fonksiyon

        /// </summary>

        /// <returns>.net remoting ile oluşturulan nesne</returns>

        public static IORGServis Tanimla()

        {

            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("KYMServisURL");

            IORGServis servis = TNS.Api.ServisIstemci.Olustur<IORGServis>(sUrl);

            return servis;

        }

    }



    /// <summary>

    /// Bu sunucu sınıfı tarafından sunulan fonksiyonları belirten arayüz sınıfı

    /// </summary>

    public interface IORGServis

    {

        /// <summary>

        /// Görev bilgisi kaydeder

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc GorevKaydet(Kullanici islemiYapan, Gorev p);



        /// <summary>

        /// Görev bilgisi listeler

        /// </summary>

        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>

        /// <returns>Object Array tipinde sınıf döndürür</returns>

        ObjectArray<Gorev> GorevListele(Gorev p);



        /// <summary>

        /// Görev bilgisi sorgular

        /// </summary>

        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>

        /// <returns>Object Array tipinde sınıf döndürür</returns>

        ObjectArray<Gorev> GorevSorgula(Gorev p);



        /// <summary>

        /// Görev bilgisi siler

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc GorevSil(Kullanici islemiYapan, Gorev p);



        /// <summary>

        /// Görev Evrak bilgisi kaydeder

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı Bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>

        /// <param name="evrEvrakNo">Evrak numarası, String tipinde sınıf</param>

        /// <param name="prEvrakNo">Program evrak numarası, String tipinde sınıf</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc GorevEvrakBilgiKaydet(Kullanici islemiYapan, int gorevKod, string evrEvrakNo, string prEvrakNo);



        /// <summary>

        /// Görev Yer Tarih bilgisi kaydeder

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="liste">Liste bilgisi, Object Array tipinde sınıf</param>

        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc GorevYerTarihKaydet(Kullanici islemiYapan, ObjectArray liste, int gorevKod);



        /// <summary>

        /// Görev Yer Tarih bilgisi listeler

        /// </summary>

        /// <param name="p">Görev Yer Tarih bilgisi, Görev Yer Tarih tipinde sınıf</param>

        /// <returns>Object Array tipinde sınıf döndürür</returns>

        ObjectArray GorevYerTarihListele(GorevYerTarih p);



        /// <summary>

        /// Görev Yer Tarih bilgisi siler

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc GorevYerTarihSil(Kullanici islemiYapan, int gorevKod);



        /// <summary>

        /// Görev Tertip bilgisi kaydeder

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="liste">Liste bilgisi, Object Array tipinde sınıf</param>

        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc GorevTertipKaydet(Kullanici islemiYapan, ObjectArray liste, int gorevKod);



        /// <summary>

        /// Görev Tertip bilgisi listeler

        /// </summary>

        /// <param name="p">Görev Tertip bilgisi, Görev Tertip tipinde sınıf</param>

        /// <returns>Object Array tipinde sınıf döndürür</returns>

        ObjectArray<GorevTertip> GorevTertipListele(GorevTertip p);



        /// <summary>

        /// Görev Tertip bilgisi siler

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc GorevTertipSil(Kullanici islemiYapan, int gorevKod);



        /// <summary>

        /// Görev Personel bilgisi kaydeder

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="liste">Liste bilgisi, Object Array tipinde sınıf</param>

        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc GorevPersonelKaydet(Kullanici islemiYapan, ObjectArray liste, int gorevKod);



        /// <summary>

        /// Görev Personel bilgisi listeler

        /// </summary>

        /// <param name="p">Görev Personel bilgisi, Görev Personel tipinde sınıf</param>

        /// <returns>Object Array tipinde sınıf</returns>

        ObjectArray GorevPersonelListele(GorevPersonel p);



        /// <summary>

        /// Görev Personel bilgisi siler

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc GorevPersonelSil(Kullanici islemiYapan, int gorevKod);



        Sonuc OrganizasyonBirimKaydet(Kullanici kullanici, TNSCollection obler);



        /// <summary>

        /// Organizasyon birim bilgisi kaydeder

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="p">Organizasyon birim bilgisi</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc OrganizasyonBirimKaydet(Kullanici islemiYapan, OrganizasyonBirim p);



        //Sonuc OrganizasyonBirimGuncelle(Kullanici islemiYapan, int orgBirimKod, int yeniAnaBirimKod, int eskiAnaBirimKod, int yeniSira);



        /// <summary>

        /// Bir kullanıcının görebileceği birimleri listeler

        /// </summary>

        /// <param name="kullanici">Organizasyon birim bilgisi</param>

        /// <param name="detayVer">Birime ait harcama birimlerini ve bütçe tertiplerini de listeler.</param>

        /// <param name="adEkle">Organizasyon birime ait kodlarının açıklamasını da döndürür.</param>

        /// <returns>Object Array tipinde sınıf döndürür</returns>

        ObjectArray<OrganizasyonBirim> OrganizasyonBirimListeleKullanici(Kullanici kullanici, bool detayVer, bool adEkle);



        /// <summary>

        /// Organizasyon birim bilgisi listeler

        /// </summary>

        /// <param name="p">Organizasyon birim bilgisi</param>

        /// <param name="detayVer">Birime ait harcama birimlerini ve bütçe tertiplerini de listeler.</param>

        /// <param name="adEkle">Organizasyon birime ait kodlarının açıklamasını da döndürür.</param>

        /// <returns>Object Array tipinde sınıf döndürür</returns>

        ObjectArray<OrganizasyonBirim> OrganizasyonBirimListele(OrganizasyonBirim p, bool detayVer, bool adEkle);



        ObjectArray<OrganizasyonBirim> OrganizasyonBirimListele(OrganizasyonBirim p, bool detayVer, int ustVeAyniSeviye, bool adEkle);



        ObjectArray<OrganizasyonBirim> Il_IlceBul(Kullanici kullanan, string plakaKod, int ilceMernisKod, bool detayVer, bool adEkle);



        /// <summary>

        /// Organizasyon birim bilgisi siler

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="p">Organizasyon birim bilgisi</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc OrganizasyonBirimSil(Kullanici islemiYapan, OrganizasyonBirim p);



        /// <summary>

        /// Organizasyon birim ana birim degistirir.

        /// </summary>

        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>

        /// <param name="kod">Kod.</param>

        /// <param name="anaBirimKod">Ana birim kodu.</param>

        /// <returns>Sonuç tipinde sınıf döndürür</returns>

        Sonuc OrganizasyonBirimAnaBirimVeSiraDegistir(Kullanici islemYapan, OrganizasyonBirim p, int yeniAnaBirimKod, int yeniSiraNo);



        Sonuc OrganizasyonSemayaSablonUygula(Kullanici kullanan, int obAnaKod, int sablonKod);



        GrafikOrgDal GrafikAgacOlustur(Kullanici kullanan, int orgBirimKod);



        Sonuc OrganizasyonBirimAc(Kullanici kullanici, int orgBirimKod);



        Sonuc OrganizasyonBirimKapat(Kullanici kullanici, int orgBirimKod);



        BirimGenelGorev GenelGorevOku(Kullanici kullanan, OrganizasyonBirim orgBirim, int siraNo);



        BirimGenelGorev[] GenelGorevListele(Kullanici kullanan, OrganizasyonBirim orgBirim, bool aktifler);



        BirimGenelGorev YeniGorevEkle(Kullanici kullanan, OrganizasyonBirim orgBirim);



        Sonuc GenelGorevKaydet(Kullanici kullanan, BirimGenelGorev birimGenelGorev);



        GenelGorevYetki GenelGorevYetkiOku(Kullanici kullanan, BirimGenelGorev birimGenelGorev, int yazYetkiSiraNo);



        Sonuc GenelGorevYetkiKaydet(Kullanici kullanan, GenelGorevYetki genelGorevYetki);



        Sonuc GenelGorevKisileriEkle(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, Personel[] personeller);



        Personel[] GenelGorevKisileriListele(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev);



        KisiGorev[] KisiGenelGorevleriListele(Kullanici kullanan, KisiGorev bilgi);



        Sonuc GenelGorevKisileriSil(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, Personel[] personeller);



        Sonuc KisiGenelGorevleriSil(Kullanici kullanan, KisiGorev[] bilgiler);



        Kullanici PersonelGorevYetkiBul(string tcKimlikNo);



        Sonuc GenelGorevYetkiSil(Kullanici kullanan, int orgKod, int gorevSiraNo, int yazSiraNo);



        CografyaBirimSayisi[] BirimTurSayisiListele(Kullanici kullanan, int il1Ilce2Ulke3, string[] turKodlar, string[] ilKodlar);



        bool MuhasebeBirimYetkiVarMi(Kullanici kullanan, string muhasebe, string birim, ENUMOGY yetkiTur);



        bool OrganizasyonBirimYetkiVarMi(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, ENUMOGY yetkiTur);



        GorevKisitDeger[] GenelGorevKisiKisitListele(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, string tckimlikno);



        Sonuc GenelGorevKisiKisitEkle(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, string tckimlikno, GorevKisitDeger[] kisitlar);



        Sonuc GenelGorevKisiTemizle(Kullanici kullanan, string tcKimlikNo);



        Sonuc KisiBirimiGorebilirMiKopru(Kullanici kullanan, int muhasebeKod, string birimKod);



        Sonuc KopruKodGuncelle(Kullanici kullanan);

    }

}
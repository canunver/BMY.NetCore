using System;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;
using TNS.ORG.IsKurallari.Veri;
using TNS.Veri;
using TNS.DEG;
using System.Collections;

namespace TNS.ORG.IsKurallari
{
    /// <summary>
    /// Organizasyon Birim Sınıfı
    /// </summary>
    class ORGBirim
    {
        private static string muhasebeBirimKontrol = "MuhasebeBirimKontrol";

        internal static ObjectArray Il_IlceBul(VeriErisim veriErisim, Kullanici kullanan, string plakaKod, int ilceMernisKod, bool detayVer, bool adEkle)
        {
            ObjectArray liste = new ObjectArray();

            TNS.KYM.IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();
            Cografya form = new Cografya();

            if (ilceMernisKod > 0)
                form.mernisKod = ilceMernisKod;
            else if (!string.IsNullOrWhiteSpace(plakaKod))
                form.plakaKod = plakaKod;
            else
            {
                liste.sonuc.DegerAta("Plaka kod veya ilcemernis kod verilmelidir!");
                return liste;
            }

            Cografya[] cler = servisKYM.ListeleCografya(kullanan, form);

            if (cler == null || cler.Length == 0)
            {
                liste.sonuc.DegerAta("Coğrafya tablosunda verilen kod bulunamadı!");
                return liste;
            }

            string cilcekod = "";
            string cilkod = "";
            if (ilceMernisKod > 0)
                cilcekod = cler[0].kod;
            else if (!string.IsNullOrWhiteSpace(plakaKod))
                cilkod = cler[0].kod;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABORGBirim tab = new TABORGBirim(ve);

            IDataReader r = null;

            try
            {
                r = tab.Il_IlceBul(cilkod, cilcekod, DateTime.Now.Year);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;

                return liste;
            }

            liste.objeler = BirimTopla(r, adEkle, detayVer, ve);
            ve.VeriErisimKapat();
            return liste;
        }


        /// <summary>
        /// Bir kullanıcının görebileceği birimleri listeler
        /// </summary>
        /// <param name="veriErisim">Organizasyon birim bilgisi</param>
        /// <param name="kullanan">Organizasyon birim bilgisi</param>
        /// <param name="detayVer">Birime ait harcama birimlerini ve bütçe tertiplerini de listeler.</param>
        /// <param name="adEkle">Organizasyon birime ait kodlarının açıklamasını da döndürür.</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public static ObjectArray ListeleKullanici(VeriErisim veriErisim, Kullanici kullanan, bool detayVer, bool adEkle)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABORGBirim tab = new TABORGBirim(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleKullanici(kullanan.mernis, DateTime.Now.Year);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;

                return liste;
            }

            liste.objeler = BirimTopla(r, adEkle, detayVer, ve);
            ve.VeriErisimKapat();
            return liste;
        }

        /// <summary>
        /// Organizasyon birim listeleyen fonksiyon
        /// </summary>
        /// <param name="p">Organizasyon birim  Bilgisi, Organizasyon birim tipinde sınıf</param>
        /// <param name="adEkle">Sözlük bilgilerinin adlarını bulmak için kullanılır. True olursa yavaş olur.</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, OrganizasyonBirim p, bool detayVer, bool adEkle)
        {
            return Listele(veriErisim, p, false, adEkle, detayVer, 0);
        }

        public static ObjectArray Listele(VeriErisim veriErisim, OrganizasyonBirim p, bool detayVer, int ustVeAyniSeviye, bool adEkle)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));
            p.kopruKod = KopruKodBul(ve, p.kod);
            if (p.kopruKod.Length <= 3)
                p.kopruKod = string.Empty;
            else
                p.kod = 0;
            ObjectArray liste = Listele(ve, p, false, adEkle, detayVer, ustVeAyniSeviye);
            ve.VeriErisimKapat();
            return liste;
        }

        private static ObjectArray Listele(VeriErisim veriErisim, OrganizasyonBirim p, bool esitle, bool adEkle, bool detayVer, int ustVeAyniSeviye)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));

            TABORGBirim tab = new TABORGBirim(ve);

            IDataReader r = tab.Listele(p, esitle, ustVeAyniSeviye);

            if (r != null)
                liste.objeler = BirimTopla(r, adEkle, detayVer, ve);
            else
            {
                ve.VeriErisimKapat();
                liste.sonuc.DegerAta(ve.hata);
                return liste;
            }

            liste.sonuc.islemSonuc = true;
            if (liste.objeler.Count == 0)
                liste.sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER017;

            ve.VeriErisimKapat();
            return liste;
        }

        private static OrgHarcamaKod[] HarcamaKodlariListele(VeriErisim veriErisim, int orgBirimKod, string muhasebeKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));
            TABORGHarcamaKod tab = new TABORGHarcamaKod(ve);

            try
            {
                IDataReader r = tab.Listele(orgBirimKod);
                TNSCollection donusDeger = new TNSCollection();

                string kurKodYapi = KurKodYapiDondur();
                string fonKodYapi = FonKodYapiDondur();

                while (r.Read())
                {
                    OrgHarcamaKod ohk = new OrgHarcamaKod();

                    ohk.kod = VeriErisim.DegerAl(r, (int)TABORGHarcamaKod.ENUMAlanlar.KOD, 0);
                    ohk.muhasebeBirimKod = VeriErisim.DegerAl(r, (int)TABORGHarcamaKod.ENUMAlanlar.MUHASEBEBIRIM, string.Empty);
                    ohk.kurKod = OrtakFonksiyonlar.KodNoktaKoy(kurKodYapi, VeriErisim.DegerAl(r, (int)TABORGHarcamaKod.ENUMAlanlar.KURKOD, string.Empty));
                    ohk.fonKod = OrtakFonksiyonlar.KodNoktaKoy(fonKodYapi, VeriErisim.DegerAl(r, (int)TABORGHarcamaKod.ENUMAlanlar.FONKOD, string.Empty));

                    if (!(string.IsNullOrEmpty(ohk.muhasebeBirimKod) || string.IsNullOrEmpty(muhasebeKod) || string.IsNullOrEmpty(ohk.kurKod)))
                        ohk.muhasebeBirimAd = ORGServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", ohk.muhasebeBirimKod + "-" + muhasebeKod + "-" + ohk.kurKod, true, string.Empty);

                    if (!string.IsNullOrEmpty(ohk.kurKod))
                        ohk.kurAd = ORGServis.UzayYaziDegerGetir("ABSKUR", DateTime.Now.Year + "-" + ohk.kurKod, true, string.Empty);

                    if (!string.IsNullOrEmpty(ohk.fonKod))
                        ohk.fonAd = ORGServis.UzayYaziDegerGetir("ABSFON", DateTime.Now.Year + "-" + ohk.fonKod, true, string.Empty);

                    donusDeger.Add(ohk);
                }
                r.Close();

                ve.VeriErisimKapat();
                return (OrgHarcamaKod[])donusDeger.ToArray(typeof(OrgHarcamaKod));
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }
        }

        private static OrgButceKod[] ButceKodlariListele(VeriErisim veriErisim, int orgBirimKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));
            TABORGButceKod tab = new TABORGButceKod(ve);

            try
            {
                IDataReader r = tab.Listele(orgBirimKod);
                TNSCollection donusDeger = new TNSCollection();

                string kurKodYapi = KurKodYapiDondur();

                while (r.Read())
                {
                    OrgButceKod obk = new OrgButceKod();

                    obk.tur = VeriErisim.DegerAl(r, (int)TABORGButceKod.ENUMAlanlar.TUR, 0);
                    obk.kurKod = OrtakFonksiyonlar.KodNoktaKoy(kurKodYapi, VeriErisim.DegerAl(r, (int)TABORGButceKod.ENUMAlanlar.KURKOD, string.Empty));

                    if (!string.IsNullOrEmpty(obk.kurKod))
                        obk.kurAd = ORGServis.UzayYaziDegerGetir("ABSKUR", DateTime.Now.Year + "-" + obk.kurKod, true, string.Empty);

                    donusDeger.Add(obk);
                }
                r.Close();

                ve.VeriErisimKapat();
                return (OrgButceKod[])donusDeger.ToArray(typeof(OrgButceKod));
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }
        }

        private static TNSCollection BirimTopla(IDataReader r, bool adEkle, bool detayVer, VeriErisim ve)
        {
            TNSCollection donusDeger = new TNSCollection();

            string cografiKodYapi = CografiKodYapiDondur();
            string kurKodYapi = KurKodYapiDondur();
            string fonKodYapi = FonKodYapiDondur();
            string turKodYapi = TurKodYapiDondur();
            while (r.Read())
            {
                OrganizasyonBirim pp = new OrganizasyonBirim();

                pp.kod = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.KOD, 0);
                pp.adi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ADI, "");
                pp.kisaadi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.KISAADI, "");
                pp.anaBirimKod = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ANABIRIMKOD, 0);
                pp.muhasebekodu = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.MUHASEBEKODU, string.Empty);
                pp.muhasebebirim = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.MUHASEBEBIRIM, "");
                pp.evrakbirim = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.EVRAKBIRIM, "");

                pp.ulke = OrtakFonksiyonlar.KodNoktaKoy(cografiKodYapi, VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ULKE, ""));
                if (!string.IsNullOrEmpty(pp.ulke) && adEkle)
                    pp.ulkeAd = ORGServis.UzayYaziDegerGetir("COGRAFYA", pp.ulke, true, string.Empty);
                pp.bolge = OrtakFonksiyonlar.KodNoktaKoy(cografiKodYapi, VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.BOLGE, ""));
                if (!string.IsNullOrEmpty(pp.bolge) && adEkle)
                    pp.bolgeAd = ORGServis.UzayYaziDegerGetir("COGRAFYA", pp.bolge, true, string.Empty);
                pp.il = OrtakFonksiyonlar.KodNoktaKoy(cografiKodYapi, VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.IL, ""));
                if (!string.IsNullOrEmpty(pp.il) && adEkle)
                    pp.ilAd = ORGServis.UzayYaziDegerGetir("COGRAFYA", pp.il, true, string.Empty);
                pp.ilce = OrtakFonksiyonlar.KodNoktaKoy(cografiKodYapi, VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ILCE, ""));
                if (!string.IsNullOrEmpty(pp.ilce) && adEkle)
                    pp.ilceAd = ORGServis.UzayYaziDegerGetir("COGRAFYA", pp.ilce, true, string.Empty);

                pp.kurumsalkod = OrtakFonksiyonlar.KodNoktaKoy(kurKodYapi, VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.KURUMSALKOD, ""));
                pp.vergino = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.VERGINO, "");
                pp.vergidairesi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.VERGIDAIRESI, "");
                pp.adres = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ADRES, "");
                pp.telefon = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.TELEFON, "");
                pp.faks = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.FAKS, "");
                pp.eposta = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.EPOSTA, "");
                pp.url = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.URL, "");
                pp.antet = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ANTET, "");
                pp.yerMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.YERMI, 0);
                pp.harcamaMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.HARCAMAMI, 0);
                pp.butceMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.BUTCEMI, 0);
                pp.tasinirMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.TASINIRMI, 0);
                pp.personelMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.PERSONELMI, 0);
                pp.evrakMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.EVRAKMI, 0);
                pp.ulkeMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ULKEMI, 0);
                pp.bolgeMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.BOLGEMI, 0);
                pp.ilMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ILMI, 0);
                pp.ilceMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ILCEMI, 0);
                pp.iletisimMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ILETISIMMI, 0);
                pp.kopruKod = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.KOPRUKOD, string.Empty);
                pp.siraNo = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.SIRANO, 0);
                pp.sablonMu = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.SABLONMU, 0) == 1 ? true : false;
                pp.fonKod = OrtakFonksiyonlar.KodNoktaKoy(fonKodYapi, VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.FONKOD, string.Empty));
                pp.cizimCikisNoktasi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.CIZIMCIKISNOKTASI, 0) == 0 ? false : true;
                pp.cizimdeGorunme = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.CIZIMDEGORUNME, 0) == 0 ? false : true;
                pp.kurumMu = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.KURUMMU, 0);
                pp.kurumKod = OrtakFonksiyonlar.KodNoktaKoy(kurKodYapi, VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.KURUMKOD, string.Empty));
                pp.ambarMi = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.AMBARMI, 0);
                pp.ambarKod = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.AMBARKOD, string.Empty);

                pp.merkezKod = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.MERKEZKOD, 0);
                if (pp.merkezKod > 0 && adEkle)
                    pp.merkezAd = ORGServis.UzayYaziDegerGetir("ORGBIRIM", pp.merkezKod.ToString(), true, string.Empty);

                pp.yoneticiKod = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.YONETICI, 0);
                if (pp.yoneticiKod > 0 && adEkle)
                    pp.yoneticiAd = ORGServis.UzayYaziDegerGetir("PERUNVAN", pp.yoneticiKod.ToString(), true, string.Empty);

                pp.ustYoneticiKod1 = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.USTYONETICI1, 0);
                if (pp.ustYoneticiKod1 > 0 && adEkle)
                    pp.ustYoneticiAd1 = ORGServis.UzayYaziDegerGetir("PERUNVAN", pp.ustYoneticiKod1.ToString(), true, string.Empty);

                pp.ustYoneticiKod2 = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.USTYONETICI2, 0);
                if (pp.ustYoneticiKod2 > 0 && adEkle)
                    pp.ustYoneticiAd2 = ORGServis.UzayYaziDegerGetir("PERUNVAN", pp.ustYoneticiKod2.ToString(), true, string.Empty);

                pp.turKod = OrtakFonksiyonlar.KodNoktaKoy(turKodYapi, VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.TURKOD, string.Empty));
                if (!string.IsNullOrEmpty(pp.turKod) && adEkle)
                    pp.turAd = ORGServis.UzayYaziDegerGetir("ORGBIRIMTUR", pp.turKod, true, string.Empty);

                pp.yoneticiTCKimlik = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.YONETICITCKIMLIK, string.Empty);
                if (!string.IsNullOrEmpty(pp.yoneticiTCKimlik) && adEkle)
                    pp.yoneticiTCKimlikAd = ORGServis.UzayYaziDegerGetir("PERPERSONEL", pp.yoneticiTCKimlik, true, string.Empty);

                pp.ustYoneticiTCKimlik1 = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.USTYONETICITCKIMLIK1, string.Empty);
                if (!string.IsNullOrEmpty(pp.ustYoneticiTCKimlik1) && adEkle)
                    pp.ustYoneticiTCKimlikAd1 = ORGServis.UzayYaziDegerGetir("PERPERSONEL", pp.ustYoneticiTCKimlik1, true, string.Empty);

                pp.ustYoneticiTCKimlik2 = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.USTYONETICITCKIMLIK2, string.Empty);
                if (!string.IsNullOrEmpty(pp.ustYoneticiTCKimlik2) && adEkle)
                    pp.ustYoneticiTCKimlikAd2 = ORGServis.UzayYaziDegerGetir("PERPERSONEL", pp.ustYoneticiTCKimlik2, true, string.Empty);

                pp.entegreKod = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ENTEGREKOD, string.Empty);
                pp.altFaaliyetKodlar = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ALTFAALKODLAR, string.Empty);

                try
                {
                    pp.altDalVar = VeriErisim.DegerAl(r, (int)TABORGBirim.ENUMAlanlar.ALTFAALKODLAR + 1, 0) >= 1 ? true : false;
                }
                catch (Exception)
                {
                }

                donusDeger.Add(pp);
            }
            r.Close();
            if (detayVer)
            {
                try
                {
                    foreach (OrganizasyonBirim ob in donusDeger)
                    {
                        ob.orgHarcamaKod = HarcamaKodlariListele(ve, ob.kod, ob.muhasebekodu);
                        ob.orgButceKod = ButceKodlariListele(ve, ob.kod);
                    }
                }
                catch (Exception)
                {
                }
            }

            return donusDeger;
        }

        private static string KurKodYapiDondur()
        {
            return KodYapiDondur(1);
        }

        private static string FonKodYapiDondur()
        {
            return KodYapiDondur(2);
        }

        private static string TurKodYapiDondur()
        {
            return KodYapiDondur(21);
        }

        private static string CografiKodYapiDondur()
        {
            return KodYapiDondur(20);
        }

        private static string KodYapiDondur(int kodNo)
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD" + kodNo.ToString() + "/KODYAPI");
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection obler)
        {
            Sonuc sonuc = YazmaYetkisiVarMi(kullanan);
            if (!sonuc.islemSonuc) return sonuc;
            if (obler.Count <= 0)
            {
                sonuc = new Sonuc();
                sonuc.islemSonuc = false;
                sonuc.hataStr = OrganizasyonSemaResI.PER264;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABORGBirim tab = new TABORGBirim(ve);

            ve.BeginTransaction();

            for (int i = 0; i < obler.Count; i++)
            {
                OrganizasyonBirim ob = (OrganizasyonBirim)obler[i];
                ob.siraNo = tab.KayitVarmi(ob.kod, ob.anaBirimKod) ? 1 : 0;
            }

            TNSCollection oblerHiyerarsi = new TNSCollection();
            oblerHiyerarsi.AddRange(obler);

            for (int i = 0; i < obler.Count; i++)
            {
                OrganizasyonBirim ob = (OrganizasyonBirim)obler[i];

                int orgBirimKod = ob.kod;

                bool orgBirimVar = ob.siraNo == 1 ? true : false;
                ob.siraNo = 0;
                if (!orgBirimVar)
                    ob.kod = 0;

                sonuc = Kaydet(ve, kullanan, ob, false);
                if (!sonuc.islemSonuc)
                    break;

                if (!orgBirimVar)
                {
                    for (int j = 0; j < oblerHiyerarsi.Count; j++)
                    {
                        OrganizasyonBirim o = (OrganizasyonBirim)oblerHiyerarsi[j];
                        if (o.anaBirimKod != 0 && o.anaBirimKod == orgBirimKod)
                        {
                            o.anaBirimKod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);
                            oblerHiyerarsi.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER019;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = OrganizasyonSemaResI.PER253 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Nesne Özellik bilgisini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Nesne Özellik bilgisi, Nesne Özellik tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim p, bool sablon)
        {
            Sonuc sonuc = YazmaYetkisiVarMi(kullanan);
            if (!sonuc.islemSonuc) return sonuc;
            if (!p.sablonMu)
            {
                string hata = string.Empty;
                if (string.IsNullOrEmpty(p.adi))
                    hata = OrganizasyonSemaResI.PER241;
                if (!string.IsNullOrEmpty(p.muhasebebirim))
                {
                    if (string.IsNullOrEmpty(ORGServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", p.muhasebebirim + "-" + p.muhasebekodu + "-" + p.kurumsalkod, true, "")))
                    {
                        hata += string.Format(OrganizasyonSemaResI.PER245, p.muhasebekodu, p.muhasebebirim);
                        if (sablon)
                            sonuc.anahtar = muhasebeBirimKontrol;
                    }
                }
                else if (!string.IsNullOrEmpty(p.muhasebekodu))
                {
                    if (string.IsNullOrEmpty(ORGServis.UzayYaziDegerGetir("MUHMUHASEBE", p.muhasebekodu, true, "")))
                    {
                        hata += string.Format(OrganizasyonSemaResI.PER244, p.muhasebekodu);
                        if (sablon)
                            sonuc.anahtar = muhasebeBirimKontrol;
                    }
                }
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.hataStr = hata;
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABORGBirim tab = new TABORGBirim(ve);

            if (!KayitVarMi(ve, p))
            {
                p.kopruKod = KopruKodUret(ve, kullanan, p.kod, p.anaBirimKod);

                if (p.orgHarcamaKod != null && p.orgHarcamaKod.Length > 0)
                {
                    p.muhasebebirim = p.orgHarcamaKod[0].muhasebeBirimKod;
                    p.kurumsalkod = p.orgHarcamaKod[0].kurKod;
                    p.fonKod = p.orgHarcamaKod[0].fonKod;
                }

                sonuc = tab.Kaydet(p);

                if (sonuc.islemSonuc)
                {
                    //if ((p.orgHarcamaKod == null || p.orgHarcamaKod.Length <= 0) &&
                    //    (!string.IsNullOrEmpty(p.muhasebebirim) || !string.IsNullOrEmpty(p.kurumsalkod) || !string.IsNullOrEmpty(p.fonKod)))
                    //{
                    //    p.orgHarcamaKod = new OrgHarcamaKod[1];
                    //    p.orgHarcamaKod[0] = new OrgHarcamaKod();
                    //    p.orgHarcamaKod[0].muhasebeBirimKod = p.muhasebebirim;
                    //    p.orgHarcamaKod[0].kurKod = p.kurumsalkod;
                    //    p.orgHarcamaKod[0].fonKod = p.fonKod;
                    //}

                    sonuc = HarcamaKodlariKaydet(ve, kullanan, p.kod, p.orgHarcamaKod);
                    if (sonuc.islemSonuc)
                    {
                        //if ((p.orgButceKod == null || p.orgButceKod.Length <= 0) && !string.IsNullOrEmpty(p.kurumsalkod))
                        //{
                        //    p.orgButceKod = new OrgButceKod[1];
                        //    p.orgButceKod[0] = new OrgButceKod();
                        //    p.orgButceKod[0].tur = (int)ENUMORGButceKodTur.MERKEZ;
                        //    p.orgButceKod[0].kurKod = p.kurumsalkod;
                        //}

                        sonuc = ButceKodlariKaydet(ve, kullanan, p.kod, p.orgButceKod);
                    }
                    sonuc.anahtar = p.kod.ToString();
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER019;
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = OrganizasyonSemaResI.PER253 + "<br>" + sonuc.hataStr;
                }
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = OrganizasyonSemaResI.PER246;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc YazmaYetkisiVarMi(Kullanici kullanici)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (kullanici == null)
                s.DegerAta("Bu işlemi yapabilmek için sisteme giriş yapmalısınız!");
            if (!kullanici.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                s.DegerAta("Bu işlemi yapabilmek için sistem yöneticisi olmalısınız!");
            return s;
        }

        public static Sonuc Guncelle(VeriErisim veriErisim, Kullanici kullanici, int orgBirimKod, int yeniAnaBirimKod, int eskiAnaBirimKod, int yeniSira)
        {
            Sonuc sonuc = YazmaYetkisiVarMi(kullanici);
            if (!sonuc.islemSonuc) return sonuc;
            //BARIS:*orgBirimKod ile belirtilen birimin ana birim kodunu *yeniAnaBirimKod yap, sırasını *sira yap, diğerlerinin sırasını kaydır.
            //BARIS:anaBirimKod'u *eskiAnaBirimKod olanların sıralarını güncelle.
            //BARIS:kopruKod ne yapılacak hiçbir fikrim yok. kopruKod güncellenecekse alt dallar için de bu işlem yapılmalı.
            return sonuc;

            //BARIS:Bunlar yapıldıktan sonra AnaBirimDegistir ve SiraDegistir fonksiyonlarına gerek kalmayacak(Başka bir yer kullanmıyorsa).
        }

        public static Sonuc HarcamaKodlariKaydet(VeriErisim veriErisim, Kullanici kullanan, int orgBirimKod, OrgHarcamaKod[] ohk)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABORGHarcamaKod tab = new TABORGHarcamaKod(ve);

            int siraNo = 0;
            tab.Sil(orgBirimKod);

            if (!(ohk == null || ohk.Length <= 0))
            {
                foreach (OrgHarcamaKod o in ohk)
                {
                    if (string.IsNullOrEmpty(o.muhasebeBirimKod) && string.IsNullOrEmpty(o.kurKod) && string.IsNullOrEmpty(o.fonKod))
                        continue;

                    o.kod = ++siraNo;
                    sonuc = tab.Kaydet(orgBirimKod, o);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc ButceKodlariKaydet(VeriErisim veriErisim, Kullanici kullanan, int orgBirimKod, OrgButceKod[] obk)
        {
            Sonuc sonuc = new Sonuc();

            if (obk == null || obk.Length <= 0)
                return sonuc;

            for (int i = 0; i < obk.Length; i++)
            {
                OrgButceKod o1 = obk[i];
                for (int j = i + 1; j < obk.Length; j++)
                {
                    OrgButceKod o2 = obk[j];
                    if (!string.IsNullOrEmpty(o1.kurKod) && !string.IsNullOrEmpty(o2.kurKod) && o1.kurKod.Replace(".", "").Equals(o2.kurKod.Replace(".", "")))
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = OrganizasyonSemaResI.PER265;
                        return sonuc;
                    }
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABORGButceKod tab = new TABORGButceKod(ve);

            tab.Sil(orgBirimKod);
            foreach (OrgButceKod o in obk)
            {
                if (o.tur <= 0 || string.IsNullOrEmpty(o.kurKod))
                    continue;

                sonuc = tab.Kaydet(orgBirimKod, o);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static bool KayitVarMi(VeriErisim veriErisim, OrganizasyonBirim p)
        {
            if (p.sablonMu)
                return false;

            ENUMORGBirimTur tur = ENUMORGBirimTur.TANIMSIZ;
            if (p.harcamaMi == 1)
                tur = ENUMORGBirimTur.HARCAMA;
            else if (p.tasinirMi == 1)
                tur = ENUMORGBirimTur.TASINIR;
            else if (p.ulkeMi == 1)
                tur = ENUMORGBirimTur.ULKE;
            else if (p.bolgeMi == 1)
                tur = ENUMORGBirimTur.BOLGE;
            else if (p.ilMi == 1)
                tur = ENUMORGBirimTur.IL;
            else if (p.ilceMi == 1)
                tur = ENUMORGBirimTur.ILCE;
            else if (p.kurumMu == 1)
                tur = ENUMORGBirimTur.KURUM;
            else if (p.ambarMi == 1)
                tur = ENUMORGBirimTur.AMBAR;

            bool kayitVarMi = false;
            if (tur != ENUMORGBirimTur.TANIMSIZ)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));
                TABORGBirim tab = new TABORGBirim(ve);
                kayitVarMi = tab.KayitVarmi(tur, p);
                ve.VeriErisimKapat();
            }
            return kayitVarMi;
        }

        private static OrganizasyonBirim KayitVer(VeriErisim veriErisim, int obKod, bool detayVer)
        {
            OrganizasyonBirim ob = new OrganizasyonBirim();
            ob.kod = obKod;
            return KayitVer(veriErisim, ob, true, detayVer);
        }

        public static OrganizasyonBirim KayitVer(VeriErisim veriErisim, OrganizasyonBirim p, bool adEkle, bool detayVer)
        {
            ObjectArray liste = Listele(veriErisim, p, true, adEkle, detayVer, 0);
            if (!liste.sonuc.islemSonuc)
            {
                if (veriErisim != null) veriErisim.VeriErisimKapat();
                throw new Exception(liste.sonuc.hataStr);
            }
            if (liste.objeler.Count > 1)
            {
                if (veriErisim != null) veriErisim.VeriErisimKapat();
                throw new Exception(OrganizasyonSemaResI.PER257);
            }
            if (liste.objeler.Count <= 0)
                return null;
            return (OrganizasyonBirim)liste.objeler[0];
        }

        private static Sonuc KopruKodGuncelle(VeriErisim veriErisim, Kullanici kullanan, int anaBirimKod, string anaKopruKod)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABORGBirim tab = new TABORGBirim(ve);

            sonuc = tab.KopruKodGuncelle(anaBirimKod, anaKopruKod);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static string KopruKodBul(VeriErisim veriErisim, int kod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));
            TABORGBirim tab = new TABORGBirim(ve);
            string kopruKod = tab.KopruKodBul(kod);
            ve.VeriErisimKapat();
            return kopruKod;
        }

        private static string KopruKodUret(VeriErisim veriErisim, Kullanici kullanan, int kod, int anaBirimKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABORGBirim tab = new TABORGBirim(ve);

            string yeniKopruKod = string.Empty;
            if (kod > 0)
                yeniKopruKod = tab.KopruKodBul(kod);

            string anaKopruKod = string.Empty;
            if (anaBirimKod > 0)
                anaKopruKod = tab.KopruKodBul(anaBirimKod);

            if (!string.IsNullOrEmpty(yeniKopruKod))
            {
                if ((yeniKopruKod.Length > 3 && yeniKopruKod.Substring(0, yeniKopruKod.Length - 3) == anaKopruKod) ||
                    (yeniKopruKod.Length == 3 && string.IsNullOrEmpty(anaKopruKod)))
                {
                    ve.VeriErisimKapat();
                    return yeniKopruKod;
                }
            }

            yeniKopruKod = anaKopruKod;
            string maksKopruKod = tab.MaksKopruKodBulFromCocuk(kod, anaBirimKod);
            if (string.IsNullOrEmpty(maksKopruKod))
                yeniKopruKod += "1".PadLeft(3, '0');
            else
            {
                int artiBir = OrtakFonksiyonlar.ConvertToInt(maksKopruKod.Substring(maksKopruKod.Length - 3), 0) + 1;
                if (artiBir > 999)
                    throw new Exception(OrganizasyonSemaResI.PER247);
                yeniKopruKod += artiBir.ToString().PadLeft(3, '0');
            }

            ve.VeriErisimKapat();
            return yeniKopruKod;
        }

        /// <summary>
        /// Nesne Özellik bilgisini silen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Nesne Özellik bilgisi, Nesne Özellik tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim p)
        {
            Sonuc sonuc = YazmaYetkisiVarMi(kullanan);
            if (!sonuc.islemSonuc) return sonuc;

            if (p.kod <= 0)
            {
                sonuc.hataStr = TNS.ORG.OrganizasyonSemaResI.PER020;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABORGBirim tab = new TABORGBirim(ve);
            TABORGButceKod tabOB = new TABORGButceKod(ve);
            TABORGHarcamaKod tabOH = new TABORGHarcamaKod(ve);

            OrganizasyonBirim obKaynak = KayitVer(veriErisim, p.kod, false);

            sonuc = tabOB.Sil(p.kod);
            if (sonuc.islemSonuc)
                sonuc = tabOH.Sil(p.kod);
            if (sonuc.islemSonuc)
                sonuc = tab.Sil(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER021;

                VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
                ve2.BeginTransaction();
                TABORGBirim tab2 = new TABORGBirim(ve);
                sonuc = tab2.SiraNoBirDegistir(0, p.anaBirimKod, obKaynak.siraNo++, 0, false, false);
                if (sonuc.islemSonuc)
                    ve2.Commit();
                else
                    ve2.Rollback();
                ve2.VeriErisimKapat();
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.ORG.OrganizasyonSemaResI.PER249 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Organizasyon birim ana birim degistirir.
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="kod">Kod.</param>
        /// <param name="anaBirimKod">Ana birim kodu.</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public static Sonuc AnaBirimDegistir(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 0)
            {
                sonuc.hataStr = TNS.ORG.OrganizasyonSemaResI.PER263;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABORGBirim tab = new TABORGBirim(ve);

            string kopruKod = KopruKodUret(ve, kullanan, p.kod, p.anaBirimKod);
            int siraNo = tab.YeniSiraNoVer(p.anaBirimKod);
            sonuc = tab.AnaBirimDegistir(p.kod, p.anaBirimKod, kopruKod, siraNo);
            if (sonuc.islemSonuc)
                sonuc = KopruKodGuncelle(ve, kullanan, p.kod, kopruKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER262;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.ORG.OrganizasyonSemaResI.PER248 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc AnaBirimVeSiraDegistir(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim p, int yeniAnaBirimKod, int yeniSiraNo)
        {
            Sonuc sonuc = YazmaYetkisiVarMi(kullanan);
            if (!sonuc.islemSonuc) return sonuc;

            int eskiAnaBirimKod = -1;
            OrganizasyonBirim obKaynak = KayitVer(veriErisim, p.kod, false);

            if (p.anaBirimKod != yeniAnaBirimKod)
            {
                eskiAnaBirimKod = p.anaBirimKod;
                p.anaBirimKod = yeniAnaBirimKod;
                sonuc = AnaBirimDegistir(veriErisim, kullanan, p);
            }

            if (sonuc == null || sonuc.islemSonuc)
                sonuc = SiraDegistir(veriErisim, kullanan, p.kod, p.anaBirimKod, yeniSiraNo);

            if (eskiAnaBirimKod != -1 && sonuc.islemSonuc && obKaynak != null)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
                ve.BeginTransaction();
                TABORGBirim tab = new TABORGBirim(ve);
                sonuc = tab.SiraNoBirDegistir(0, eskiAnaBirimKod, obKaynak.siraNo++, 0, false, false);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        public static Sonuc SablonUygula(VeriErisim veriErisim, Kullanici kullanan, int obAnaKod, int sablonKod)
        {
            Sonuc sonuc = YazmaYetkisiVarMi(kullanan);
            if (!sonuc.islemSonuc) return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            OrganizasyonBirim kriterSablon = new OrganizasyonBirim();
            kriterSablon.kod = sablonKod;
            ObjectArray liste = Listele(ve, kriterSablon, false, false);
            if (!liste.sonuc.islemSonuc || liste.objeler.Count <= 0)
            {
                ve.VeriErisimKapat();
                return liste.sonuc;
            }
            OrganizasyonBirim sablon = (OrganizasyonBirim)liste.objeler[0];

            if (!sablon.sablonMu)
                sonuc.hataStr = OrganizasyonSemaResI.PER254 + "<br>";
            if (sablon.kod <= 0)
                sonuc.hataStr += OrganizasyonSemaResI.PER251 + "<br>";
            if (string.IsNullOrEmpty(sablon.adi))
                sonuc.hataStr += OrganizasyonSemaResI.PER252 + "<br>";
            if (sablon.ilMi <= 0 && sablon.ilceMi <= 0)
                sonuc.hataStr += OrganizasyonSemaResI.PER250;
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                return sonuc;
            }

            kriterSablon = new OrganizasyonBirim();
            kriterSablon.kopruKod = sablon.kopruKod;
            kriterSablon.sablonMu = true;
            liste = Listele(ve, kriterSablon, false, false);
            if (!liste.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return liste.sonuc;
            }
            TNSCollection sablonlar = liste.objeler;

            IBKDServis servisBKD = TNS.BKD.Arac.Tanimla();
            ABSKodlarVAN absKodVan = servisBKD.KodListeleSeviye(0, "20", sablon.ulke, 3, true);
            if (!absKodVan.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return absKodVan.sonuc;
            }
            TNSCollection iller = absKodVan.absKodlar;
            iller.Sort();

            absKodVan = servisBKD.KodListeleSeviye(0, "20", sablon.ulke, 4, true);
            if (!absKodVan.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return absKodVan.sonuc;
            }
            TNSCollection ilceler = absKodVan.absKodlar;
            ilceler.Sort();

            kriterSablon = new OrganizasyonBirim();
            kriterSablon.kod = obAnaKod;
            liste = Listele(ve, kriterSablon, false, false);
            if (!liste.sonuc.islemSonuc || liste.objeler.Count <= 0)
            {
                ve.VeriErisimKapat();
                return liste.sonuc;
            }
            OrganizasyonBirim obAna = (OrganizasyonBirim)liste.objeler[0];

            sonuc = SablonUygula(ve, kullanan, obAna, sablon.anaBirimKod, sablonlar, iller, ilceler);
            if (sonuc.islemSonuc)
            {
                if (!string.IsNullOrEmpty(sonuc.bilgiStr))
                    sonuc.bilgiStr = "<br><br>" + sonuc.bilgiStr;
                sonuc.bilgiStr = OrganizasyonSemaResI.PER256 + sonuc.bilgiStr;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = OrganizasyonSemaResI.PER255 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc SablonUygula(VeriErisim ve, Kullanici kullanan, OrganizasyonBirim obAna, int anaSablonId, TNSCollection sablonlar, TNSCollection iller, TNSCollection ilceler)
        {
            string bilgi = string.Empty;
            Sonuc sonuc = new Sonuc();

            for (int i = 0; i < sablonlar.Count; i++)
            {
                OrganizasyonBirim sablon = (OrganizasyonBirim)sablonlar[i];
                if (sablon.anaBirimKod != anaSablonId)
                    continue;

                if (sablon.ilMi == 1)
                {
                    foreach (ABSKod il in iller)
                    {
                        OrganizasyonBirim obIl = new OrganizasyonBirim();
                        obIl.anaBirimKod = obAna.kod;
                        obIl.ulke = il.kod.IndexOf(".") > -1 ? il.kod.Substring(0, il.kod.IndexOf(".")) : il.kod;
                        obIl.bolge = il.kod.LastIndexOf(".") > -1 ? il.kod.Substring(0, il.kod.LastIndexOf(".")) : il.kod;
                        obIl.il = il.kod;
                        obIl.ilMi = 1;

                        OrganizasyonBirim varOlanOB = KayitVer(ve, obIl, false, false);
                        if (varOlanOB != null)
                            obIl = varOlanOB;

                        obIl.adi = OrtakFonksiyonlar.IlkHarflerBuyuk(FormulDegerBul(sablon.adi, "İLADI", il.Ad(kullanan.aktifKultur)));
                        obIl.kurumsalkod = sablon.kurumsalkod;
                        obIl.merkezKod = sablon.merkezKod;
                        obIl.yoneticiKod = sablon.yoneticiKod;
                        obIl.ustYoneticiKod1 = sablon.ustYoneticiKod1;
                        obIl.ustYoneticiKod2 = sablon.ustYoneticiKod2;

                        sonuc = Kaydet(ve, kullanan, obIl, true);
                        if (!sonuc.islemSonuc)
                            return sonuc;

                        obIl.kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);
                        obIl.adi = il.Ad(kullanan.aktifKultur);

                        sonuc = SablonUygula(ve, kullanan, obIl, sablon.kod, sablonlar, iller, ilceler);
                        if (!sonuc.islemSonuc)
                            return sonuc;

                        if (!string.IsNullOrEmpty(sonuc.bilgiStr))
                            bilgi += sonuc.bilgiStr;
                    }
                }
                else if (sablon.ilceMi == 1)
                {
                    foreach (ABSKod ilce in ilceler)
                    {
                        OrganizasyonBirim obAnaYedek = null;

                        string ulke = ilce.kod.IndexOf(".") > -1 ? ilce.kod.Substring(0, ilce.kod.IndexOf(".")) : ilce.kod;
                        string il = ilce.kod.LastIndexOf(".") > -1 ? ilce.kod.Substring(0, ilce.kod.LastIndexOf(".")) : ilce.kod;
                        string bolge = il.LastIndexOf(".") > -1 ? il.Substring(0, il.LastIndexOf(".")) : il;

                        if (string.IsNullOrEmpty(obAna.il)) //string.IsNullOrEmpty(obAna.ulke) || 
                        {
                            OrganizasyonBirim kriterOB = new OrganizasyonBirim();
                            kriterOB.anaBirimKod = obAna.kod;
                            kriterOB.ulke = ulke;
                            kriterOB.bolge = bolge;
                            kriterOB.il = il;
                            kriterOB = KayitVer(ve, kriterOB, false, false);
                            if (kriterOB == null)
                                continue;

                            obAnaYedek = obAna;
                            obAna = kriterOB;
                        }
                        else if (il != obAna.il) //ulke != obAna.ulke || 
                            continue;

                        OrganizasyonBirim obIlce = new OrganizasyonBirim();
                        obIlce.anaBirimKod = obAna.kod;
                        obIlce.ulke = ulke;
                        obIlce.bolge = bolge;
                        obIlce.il = il;
                        obIlce.ilce = ilce.kod;
                        obIlce.ilceMi = 1;

                        OrganizasyonBirim varOlanOB = KayitVer(ve, obIlce, false, false);
                        if (varOlanOB != null)
                            obIlce = varOlanOB;

                        obIlce.adi = OrtakFonksiyonlar.IlkHarflerBuyuk(FormulDegerBul(sablon.adi, "İLÇEADI", ilce.Ad(kullanan.aktifKultur)));
                        obIlce.kurumsalkod = sablon.kurumsalkod;
                        obIlce.merkezKod = sablon.merkezKod;
                        obIlce.yoneticiKod = sablon.yoneticiKod;
                        obIlce.ustYoneticiKod1 = sablon.ustYoneticiKod1;
                        obIlce.ustYoneticiKod2 = sablon.ustYoneticiKod2;

                        sonuc = Kaydet(ve, kullanan, obIlce, true);
                        if (!sonuc.islemSonuc)
                            return sonuc;

                        obIlce.kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);
                        obIlce.adi = ilce.Ad(kullanan.aktifKultur);

                        sonuc = SablonUygula(ve, kullanan, obIlce, sablon.kod, sablonlar, iller, ilceler);
                        if (!sonuc.islemSonuc)
                            return sonuc;

                        if (obAnaYedek != null)
                            obAna = obAnaYedek;

                        if (!string.IsNullOrEmpty(sonuc.bilgiStr))
                            bilgi += sonuc.bilgiStr;
                    }
                }
                else if (sablon.yerMi == 1 || sablon.harcamaMi == 1 || sablon.butceMi == 1 ||
                    sablon.tasinirMi == 1 || sablon.personelMi == 1 || sablon.evrakMi == 1)
                {
                    string plakaKod = string.Empty;
                    if (!string.IsNullOrEmpty(obAna.il))
                        plakaKod = ORGServis.UzayYaziDegerGetir("COGRAFYAMUHASEBE", obAna.il, true, "PLAKAKOD");

                    ObjectArray mbler = null;
                    int donguSay = 0;
                    bool devamEt = true;
                    while (devamEt)
                    {
                        MuhasebeBirim mb = null;

                        OrganizasyonBirim ob = new OrganizasyonBirim();
                        ob.anaBirimKod = obAna.kod;

                        if (sablon.adi.Replace(" ", "").ToUpper() == "*BİRİMADI*")
                        {
                            if (donguSay == 0)
                            {
                                mb = new MuhasebeBirim();
                                mb.birimKod = sablon.muhasebebirim;
                                mb.plakaKod = OrtakFonksiyonlar.ConvertToInt(plakaKod, 0);
                                mb.kurum = sablon.kurumsalkod;

                                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                                mbler = servisMUH.MuhasebeBirimListele(mb);
                                if (!mbler.sonuc.islemSonuc || mbler.objeler.Count <= 0)
                                    break;
                            }

                            if (donguSay == mbler.objeler.Count - 1)
                                devamEt = false;

                            mb = (MuhasebeBirim)mbler.objeler[donguSay];
                            ob.kurumsalkod = mb.kurum;
                            ob.muhasebekodu = mb.muhasebe.ToString();
                            ob.muhasebebirim = mb.birimKod;

                            donguSay++;
                        }
                        else
                        {
                            devamEt = false;

                            ob.kurumsalkod = sablon.kurumsalkod;
                            if (sablon.harcamaMi == 1 || sablon.tasinirMi == 1)
                            {
                                if (string.IsNullOrEmpty(sablon.muhasebekodu))
                                {
                                    if (!string.IsNullOrEmpty(obAna.ilce))
                                        ob.muhasebekodu = ORGServis.UzaySayiDegerGetir("COGRAFYAMUHASEBE", obAna.ilce, true, string.Empty).ToString();
                                    else if (!string.IsNullOrEmpty(obAna.il))
                                        ob.muhasebekodu = ORGServis.UzaySayiDegerGetir("COGRAFYAMUHASEBE", obAna.il, true, string.Empty).ToString();
                                }
                                else
                                    ob.muhasebekodu = FormulDegerBul(sablon.muhasebekodu, "İLKODU", plakaKod);
                                ob.muhasebekodu = OrtakFonksiyonlar.ConvertToInt(ob.muhasebekodu, 0).ToString();
                                ob.muhasebebirim = sablon.muhasebebirim;
                            }
                        }

                        ob.yerMi = sablon.yerMi;

                        OrganizasyonBirim varOlanOB = KayitVer(ve, ob, false, false);
                        if (varOlanOB != null)
                            ob = varOlanOB;

                        if (mb == null)
                        {
                            ob.adi = FormulDegerBul(sablon.adi, "İLADI", obAna.adi);
                            if (ob.adi == sablon.adi)
                                ob.adi = FormulDegerBul(sablon.adi, "İLÇEADI", obAna.adi);
                        }
                        else
                            ob.adi = mb.birimAd;
                        ob.adi = OrtakFonksiyonlar.IlkHarflerBuyuk(ob.adi);

                        ob.evrakbirim = FormulDegerBul(sablon.evrakbirim, "İLKODU", plakaKod);
                        if (!string.IsNullOrEmpty(obAna.ilce))
                            ob.evrakbirim = FormulDegerBul(ob.evrakbirim, "İLÇEKODU",
                                ORGServis.UzayYaziDegerGetir("COGRAFYAMUHASEBE", obAna.ilce, true, "PLAKAKOD"));

                        ob.harcamaMi = sablon.harcamaMi;
                        ob.butceMi = sablon.butceMi;
                        ob.tasinirMi = sablon.tasinirMi;
                        ob.personelMi = sablon.personelMi;
                        ob.evrakMi = sablon.evrakMi;
                        ob.merkezKod = sablon.merkezKod;
                        ob.yoneticiKod = sablon.yoneticiKod;
                        ob.ustYoneticiKod1 = sablon.ustYoneticiKod1;
                        ob.ustYoneticiKod2 = sablon.ustYoneticiKod2;

                        sonuc = Kaydet(ve, kullanan, ob, true);
                        if (!sonuc.islemSonuc && sonuc.anahtar.Equals(muhasebeBirimKontrol))
                            bilgi += sonuc.hataStr + "<br>";
                        else if (!sonuc.islemSonuc)
                            return sonuc;

                        ob.kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);
                        ob.adi = obAna.adi;
                        ob.il = obAna.il;
                        ob.ilce = obAna.ilce;

                        sonuc = SablonUygula(ve, kullanan, ob, sablon.kod, sablonlar, iller, ilceler);
                        if (!sonuc.islemSonuc)
                            return sonuc;

                        if (!string.IsNullOrEmpty(sonuc.bilgiStr))
                            bilgi += sonuc.bilgiStr;
                    }
                }
            }

            if (!string.IsNullOrEmpty(bilgi))
                sonuc.bilgiStr = bilgi;

            return sonuc;
        }

        private static string FormulDegerBul(string formulluStr, string formul, string eklenecekStr)
        {
            string bulunan = string.Empty;
            int posBasYildiz = formulluStr.IndexOf("*");
            if (posBasYildiz != -1 && formulluStr.Length > posBasYildiz + 1)
            {
                int posBitYildiz = formulluStr.IndexOf("*", posBasYildiz + 1);
                if (posBitYildiz != -1 &&
                    formulluStr.Substring(posBasYildiz + 1, posBitYildiz - posBasYildiz - 1).Replace(" ", "").ToUpper() == formul.Replace(" ", "").ToUpper())
                {
                    if (posBasYildiz != 0)
                        bulunan = formulluStr.Substring(0, posBasYildiz);
                    bulunan += eklenecekStr;
                    if (formulluStr.Length > posBitYildiz + 1)
                        bulunan += formulluStr.Substring(posBitYildiz + 1);
                }
            }
            return string.IsNullOrEmpty(bulunan) ? formulluStr : bulunan;
        }

        private static Sonuc SiraDegistir(VeriErisim veriErisim, Kullanici kullanan, int kod, int anaBirimKod, int yeniSiraNo)
        {
            Sonuc sonuc = new Sonuc();

            if (kod <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.ORG.OrganizasyonSemaResI.PER258;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));

            OrganizasyonBirim obKaynak = KayitVer(ve, kod, false);
            if (obKaynak == null)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = OrganizasyonSemaResI.PER259;
                return sonuc;
            }

            ve.BeginTransaction();
            TABORGBirim tab = new TABORGBirim(ve);

            int basSira = yeniSiraNo;
            bool arttir = true;
            int bitSira = obKaynak.siraNo;
            if (basSira >= bitSira)
            {
                arttir = false;
                bitSira = basSira;
                basSira = obKaynak.siraNo;
            }

            sonuc = tab.SiraNoBirDegistir(obKaynak.kod, anaBirimKod, basSira, bitSira, false, arttir);
            if (sonuc.islemSonuc)
                sonuc = tab.SiraNoBirDegistir(obKaynak.kod, anaBirimKod, basSira, bitSira, true, arttir);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER262;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.ORG.OrganizasyonSemaResI.PER248 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static GrafikOrgDal GrafikAgacOlustur(VeriErisim veriErisim, Kullanici kullanan, int orgBirimKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABORGBirim tabBirim = new TABORGBirim(ve);

            IDataReader r = null;
            try
            {
                r = tabBirim.GrafikBirimleriniBul(tabBirim.KopruKodBul(orgBirimKod));
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            if (r == null)
                throw new Exception(OrganizasyonSemaResI.PER266);

            TNSCollection obler = BirimTopla(r, true, false, ve);
            ve.VeriErisimKapat();

            GrafikOrgDal anaDal = null;
            for (int i = 0; i < obler.Count; i++)
            {
                OrganizasyonBirim obAna = (OrganizasyonBirim)obler[i];
                if (orgBirimKod == obAna.kod)
                {
                    anaDal = new GrafikOrgDal(obAna);
                    obler.RemoveAt(i);
                    break;
                }
            }

            int donguSure = DateTime.Now.Minute;
            bool donguBitti = false;
            while (!donguBitti && donguSure + 1 > DateTime.Now.Minute)
            {
                donguBitti = true;
                foreach (OrganizasyonBirim ob in obler)
                {
                    if (ob.ustYoneticiKod2 > 0)
                    {
                        GrafikOrgDal dalUstYon2 = new GrafikOrgDal();
                        dalUstYon2.kod = ob.ustYoneticiKod2;
                        dalUstYon2.kopruKod = ob.kopruKod;
                        dalUstYon2.ad = ob.ustYoneticiAd2;
                        dalUstYon2.yoneticiAd = ob.ustYoneticiTCKimlikAd2;
                        dalUstYon2.tur = ENUMGrafikOrgDalTur.YONETICI;
                        if (!anaDal.DalEkle(dalUstYon2))
                            donguBitti = false;
                    }

                    if (ob.ustYoneticiKod1 > 0)
                    {
                        GrafikOrgDal dalUstYon1 = new GrafikOrgDal();
                        dalUstYon1.kod = ob.ustYoneticiKod1;
                        dalUstYon1.kopruKod = ob.kopruKod;
                        dalUstYon1.ad = ob.ustYoneticiAd1;
                        dalUstYon1.yoneticiAd = ob.ustYoneticiTCKimlikAd1;
                        dalUstYon1.tur = ENUMGrafikOrgDalTur.YONETICI;
                        dalUstYon1.baglanacakYoneticiKod = ob.ustYoneticiKod2;
                        if (!anaDal.DalEkle(dalUstYon1))
                            donguBitti = false;
                    }

                    GrafikOrgDal dal = new GrafikOrgDal(ob);
                    if (!anaDal.DalEkle(dal))
                        donguBitti = false;
                }
            }

            return anaDal;
        }

        public static Sonuc BirimAc(VeriErisim veriErisim, Kullanici kullanici, int orgBirimKod)
        {
            return BirimAcKapat(veriErisim, kullanici, orgBirimKod, true);
        }

        public static Sonuc BirimKapat(VeriErisim veriErisim, Kullanici kullanici, int orgBirimKod)
        {
            return BirimAcKapat(veriErisim, kullanici, orgBirimKod, false);
        }

        private static Sonuc BirimAcKapat(VeriErisim veriErisim, Kullanici kullanan, int orgBirimKod, bool ac)
        {
            Sonuc sonuc = YazmaYetkisiVarMi(kullanan);
            if (!sonuc.islemSonuc) return sonuc;

            if (orgBirimKod <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = OrganizasyonSemaResI.PER269;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            //string kopruKod = KopruKodBul(ve, orgBirimKod);

            TABORGBirim tabOrg = new TABORGBirim(ve);
            ve.BeginTransaction();

            if (ac)
                sonuc = tabOrg.BirimAc(orgBirimKod);//kopruKod
            else
                sonuc = tabOrg.BirimKapat(orgBirimKod);//kopruKod

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER262;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.ORG.OrganizasyonSemaResI.PER248 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static CografyaBirimSayisi[] BirimTurSayisiListele(VeriErisim veriErisim, Kullanici kullanan, int il1Ilce2Ulke3, string[] turKodlar, string[] ilKodlar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABORGBirim tabOrg = new TABORGBirim(ve);

            CografyaBirimSayisi[] donusDeger = null;

            IDataReader dr = null;
            if (il1Ilce2Ulke3 == 3)
            {
                dr = tabOrg.UlkedeBirimTurSayisiBul(turKodlar);
                if (dr != null)
                {
                    if (dr.Read())
                    {
                        donusDeger = new CografyaBirimSayisi[1];
                        donusDeger[0] = new CografyaBirimSayisi("01", "Türkiye", VeriErisim.DegerAl(dr, 0, (int)0), false);
                    }
                    dr.Close();
                }
            }
            else
            {
                if (il1Ilce2Ulke3 == 1)
                    dr = tabOrg.IllerdeBirimTurSayisiBul(turKodlar, ilKodlar);
                else
                    dr = tabOrg.IlcelerdeBirimTurSayisiBul(turKodlar, ilKodlar);
                if (dr != null)
                {
                    ArrayList al = new ArrayList();
                    while (dr.Read())
                    {
                        al.Add(new CografyaBirimSayisi(VeriErisim.DegerAl(dr, 0, ""), VeriErisim.DegerAl(dr, 2, ""), VeriErisim.DegerAl(dr, 3, (int)0), true));
                    }
                    dr.Close();
                    donusDeger = (CografyaBirimSayisi[])al.ToArray(typeof(CografyaBirimSayisi));
                }
            }
            return donusDeger;
        }

        internal static Sonuc KopruKodGuncelle(VeriErisim veriErisim, Kullanici kullanan)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABORGBirim tab = new TABORGBirim(ve);
            Sonuc s = new Sonuc();
            s = BirimDolas(ve, tab, -1, "", s);
            ve.VeriErisimKapat();
            return s;
        }

        private static Sonuc BirimDolas(VeriErisim ve, TABORGBirim tab, int ustKod, string ustKopruKod, Sonuc s)
        {
            OrganizasyonBirim p = new OrganizasyonBirim();
            p.anaBirimKod = ustKod;
            ObjectArray oa = Listele(ve, p, true, false, false, 0);
            if (!oa.sonuc.islemSonuc) return oa.sonuc;

            for (int i = 0; i < oa.objeler.Count; i++)
            {
                OrganizasyonBirim gelen = (OrganizasyonBirim)oa.objeler[i];
                string icKopruKod = ustKopruKod + (i + 1).ToString().PadLeft(3, '0');
                Sonuc sic = tab.KopruKodGuncelleBirim(gelen.kod, icKopruKod);
                if (!sic.islemSonuc)
                {
                    s.DegerAta(sic.hataStr);
                    return s;
                }
                s = BirimDolas(ve, tab, gelen.kod, icKopruKod, s);
                if (!s.islemSonuc)
                    return s;
            }
            return s;
        }
    }
}
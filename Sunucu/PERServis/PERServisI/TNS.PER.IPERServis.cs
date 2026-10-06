using System;
using TNS.KYM;
using TNS.DEG;

namespace TNS.PER
{

    /// <summary>
    /// Personelle ilgili görev tanımı, bilgi birikimi, beceri gibi bilgileri tutan sınıf
    /// </summary>
    [Serializable()]
    public class PersonelBilgiBirikimi
    {
        /// <summary>
        /// Personelin T.C. kimlik numarası
        /// </summary>
        public string tcKimlikNo;

        /// <summary>
        /// Personelin görev tanımı
        /// </summary>
        public string gorev;

        /// <summary>
        /// Personelin bilgi birikimi
        /// </summary>
        public string bilgi;

        /// <summary>
        /// Personelin sahip olduğu beceriler
        /// </summary>
        public string beceri;

        public string ad;
        public string soyad;
        public string resim;

        /// <summary>
        /// Parametresiz Personel nesnesi oluşturucu yordam
        /// </summary>
        public PersonelBilgiBirikimi()
        {
            tcKimlikNo = string.Empty;
            gorev = string.Empty;
            bilgi = string.Empty;
            beceri = string.Empty;
        }
    }

    [Serializable()]
    public static class Arac
    {
        public static int ISTIHDAMSEKLISAY = 5;

        public static string IstihdamSekliAd(ENUMPersonelIstihdamSekli istihdamSekli)
        {
            if (istihdamSekli == ENUMPersonelIstihdamSekli.MEMUR) return PersonelResI.FRMKAD023_1;
            if (istihdamSekli == ENUMPersonelIstihdamSekli.Sozlesmeli) return PersonelResI.FRMKAD023_2;
            if (istihdamSekli == ENUMPersonelIstihdamSekli.Sozlesmeli_4C) return PersonelResI.FRMKAD023_3;
            if (istihdamSekli == ENUMPersonelIstihdamSekli.ISCI) return PersonelResI.FRMKAD023_4;
            if (istihdamSekli == ENUMPersonelIstihdamSekli.Gecici_Isci) return PersonelResI.FRMKAD023_5;
            if (istihdamSekli == ENUMPersonelIstihdamSekli.SSKBagkur) return PersonelResI.FRMKAD023_20;
            if (istihdamSekli == ENUMPersonelIstihdamSekli.OzelSektor) return PersonelResI.FRMKAD023_21;
            return "";
        }

        public static IPERServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("PERServisURL");
            IPERServis servis = TNS.Api.ServisIstemci.Olustur<IPERServis>(sUrl);
            return servis;
        }

        public static bool OrgSemaPersonelKullaniyorMu()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/OrgSemaPersonel"), 0) > 0 ? true : false;
        }

        public static bool MerkezBankasiKullaniyor()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/MERKEZBANKASI"), 0) > 0;
        }

        public static bool PersonelKendiBilgileriniDegistiremesin()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/PersonelKendiBilgileriniDegistiremesin"), 0) > 0 ? true : false;
        }
    }

    [Serializable()]
    public static class Yetki
    {
        public static TNS.ORG.IORGServis _servisORG = null;

        public static TNS.ORG.IORGServis servisORG
        {
            get
            {
                if (_servisORG == null) _servisORG = TNS.ORG.Arac.Tanimla();
                return _servisORG;
            }
        }

        public static TNS.PER.IPERServis _servisPER = null;

        public static TNS.PER.IPERServis servisPER
        {
            get
            {
                if (_servisPER == null) _servisPER = TNS.PER.Arac.Tanimla();
                return _servisPER;
            }
        }
        public static bool YetkisiVarMiKopru(Kullanici kullanan, string saymanlik, string birim, int orgKod)
        {
            Sonuc s = servisORG.KisiBirimiGorebilirMiKopru(kullanan, OrtakFonksiyonlar.ConvertToInt(saymanlik, -1), birim);
            return s.islemSonuc;
        }

        public static bool YetkisiVarMiBirime(Kullanici kullanan, string saymanlik, string birim, int orgKod, bool gorme = false)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
            {
                if (OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0) == OrtakFonksiyonlar.ConvertToInt(saymanlik, 0))
                    return true;
            }

            bool orgKullaniliyor = TNS.PER.Arac.OrgSemaPersonelKullaniyorMu();
            if (orgKullaniliyor && orgKod > 0)
            {
                string[] ytDegerler = YetkiliOlduguBirimler(kullanan, gorme);

                foreach (string y in ytDegerler)
                {
                    if (orgKod == OrtakFonksiyonlar.ConvertToInt(y, 0))
                        return true;
                }
            }
            else
            {
                string[] ytDegerler = YetkiliOlduguBirimler(kullanan, gorme);

                for (int i = 0; i < ytDegerler.Length; i++)
                {
                    string[] birimler;
                    birimler = ytDegerler[i].Split('-');
                    if (birimler.Length > 1)//Yetki tipi saymanlık-birim şeklinde verilmesi lazım
                    {
                        if (birimler[0] == "*" && birimler[1] == "*")
                            return true;
                        else if (OrtakFonksiyonlar.ConvertToInt(birimler[0], 0) == OrtakFonksiyonlar.ConvertToInt(saymanlik, 0) && birimler[1] == "*")
                            return true;
                        else if (OrtakFonksiyonlar.ConvertToInt(birimler[0], 0) == OrtakFonksiyonlar.ConvertToInt(saymanlik, 0) && OrtakFonksiyonlar.ConvertToInt(birimler[1], 0) == OrtakFonksiyonlar.ConvertToInt(birim, 0))
                            return true;
                    }
                }
            }

            return false;
        }

        public static string[] YetkiliOlduguBirimler(Kullanici kullanan, bool gorme = false)
        {
            bool orgKullaniliyor = TNS.PER.Arac.OrgSemaPersonelKullaniyorMu();
            if (orgKullaniliyor)
            {
                string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Personel Organizasyon Yetkisi");
                yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri.TrimEnd(';');
                string[] yler = yetkiTipiYetkiDegeri.Split(';');
                string donenDeger = "";
                for (int i = 0; i < yler.Length; i++)
                {
                    TNS.ORG.OrganizasyonBirim kriter = new TNS.ORG.OrganizasyonBirim();
                    kriter.kod = OrtakFonksiyonlar.ConvertToInt(yler[i], 0);
                    ObjectArray dler = servisORG.OrganizasyonBirimListele(kriter, false, false);
                    kriter.kod = 0;
                    kriter.kopruKod = ((TNS.ORG.OrganizasyonBirim)dler.objeler[0]).kopruKod;
                    dler = servisORG.OrganizasyonBirimListele(kriter, false, false);

                    foreach (TNS.ORG.OrganizasyonBirim o in dler.objeler)
                    {
                        if (donenDeger != "") donenDeger += ";";
                        donenDeger += o.kod.ToString();
                    }
                }

                return donenDeger.Split(';');
            }
            else
            {
                string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Personel Saymanlık-Birim Yetkisi");
                if (gorme)
                {
                    string odemeEmriMifYetkisi = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Ödeme Emri/MİF Saymanlık-Birim Yetkisi");
                    if (!string.IsNullOrWhiteSpace(odemeEmriMifYetkisi))
                    {
                        if (string.IsNullOrWhiteSpace(yetkiTipiYetkiDegeri)) yetkiTipiYetkiDegeri = odemeEmriMifYetkisi;
                        else yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri + ";" + odemeEmriMifYetkisi;
                    }
                }
                yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri.TrimEnd(';');
                return yetkiTipiYetkiDegeri.Split(';');
            }
        }

        public static Personel PersonelOku(Kullanici kullanan, string tcKimlik)
        {
            Personel p = new Personel();
            p.tcKimlikNo = tcKimlik;
            ObjectArray oa = servisPER.PersonelListele(kullanan, p);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
                return (Personel)oa[0];
            return (Personel)null;
        }

        public static bool YetkiVarMiYazmaya(Kullanici kullanan, string kayitEdecegiMernis, bool kendisiIseKontroleDevam = false)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (string.IsNullOrWhiteSpace(kayitEdecegiMernis))
                return YetkiVarMiYazmaya(kullanan, (Personel)null);
            else
                return YetkiVarMiYazmaya(kullanan, PersonelOku(kullanan, kayitEdecegiMernis), kendisiIseKontroleDevam);
        }

        public static bool YetkiVarMiYazmaya(Kullanici kullanan, Personel p, bool kendisiIseKontroleDevam = false)
        {
            if (TNS.PER.Arac.PersonelKendiBilgileriniDegistiremesin())//Bazı kurumlar (Teftiş) Kullanıcıların kendi bilgilerini değiştirmesini istemiyor Melih 04.11.2014
                kendisiIseKontroleDevam = true;

            if (kullanan.mernis == p.tcKimlikNo)
                if (!kendisiIseKontroleDevam)
                    return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            Sonuc s = new Sonuc(true, "", "");
            if (!PER.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
                s = TNS.ORG.Arac.Tanimla().KisiBirimiGorebilirMiKopru(kullanan, p.muhasebeKodu, p.birimKodu);
            if (s.islemSonuc)
            {
                if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                    return true;

                if (kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "2", false) ||
                    kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "3", false) ||
                    kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "4", false)
                    )
                    return true;
            }
            return false;
        }

        public static bool YetkiVarMiOkumaya(Kullanici kullanan, string okuyacagiMernis = "")
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return true;

            if (kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "1", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "4", false)
                )
                return true;

            if (kullanan.mernis == okuyacagiMernis) //Kullanıcı kendi bilgisini görsün diye
                return true;

            return false;
        }
    }

    public interface IPERServis
    {
        //Görev
        Sonuc GorevKaydet(Kullanici islemiYapan, Gorev p);
        ObjectArray GorevListele(Kullanici islemiYapan, Gorev p);
        ObjectArray GorevSorgula(Kullanici islemiYapan, Gorev p);
        Sonuc GorevSil(Kullanici islemiYapan, Gorev p);
        Sonuc GorevDurumDegistir(Kullanici islemiYapan, int gorevYil, int gorevKod, int durum);
        Sonuc GorevDurumDegistir(Kullanici islemiYapan, string prEvrakNo, string evrEvrakNo, int durum);
        Sonuc GorevEvrakBilgiKaydet(Kullanici islemiYapan, int gorevYil, int gorevKod, string evrEvrakNo, string prEvrakNo);
        Sonuc GorevAlanBilgiKaydet(Kullanici islemiYapan, int gorevYil, int gorevKod, string alan1, string imza1, string imza2, string imza3);
        Sonuc GorevYerTarihKaydet(Kullanici islemiYapan, ObjectArray liste, int gorevYil, int gorevKod);
        ObjectArray GorevYerTarihListele(GorevYerTarih p);
        Sonuc GorevYerTarihSil(Kullanici islemiYapan, int gorevYil, int gorevKod);
        Sonuc GorevPersonelKaydet(Kullanici islemiYapan, ObjectArray liste, int gorevYil, int gorevKod);
        ObjectArray GorevPersonelListele(GorevPersonel p);
        Sonuc GorevPersonelSil(Kullanici islemiYapan, int gorevYil, int gorevKod);

        //Personel
        Sonuc PersonelKaydet(Kullanici islemiYapan, Personel p);
        Sonuc PersonelBirimKaydet(Kullanici islemiYapan, Personel p);
        ObjectArray PersonelListele(Kullanici islemiYapan, Personel p);
        ObjectArray PersonelListeleUnvanKadro(Kullanici islemiYapan, Personel p);
        ObjectArray PersonelListeleOzet(Kullanici islemiYapan, Personel p);
        ObjectArray PersonelListelePersonelRapor(Kullanici islemiYapan, Personel p);
        ObjectArray PersonelListeleEgitimRapor(Kullanici islemiYapan, Personel p, PersonelKurs pk);
        ObjectArray PersonelListeleIzinRapor(Kullanici islemiYapan, Personel p, PersonelIzin pk);
        ObjectArray PersonelListeleIzinDurum(Kullanici islemiYapan, PersonelIzinDurum pk);
        Personel PersonelMemuriyetTarihleriListele(string tcKimlikNo);
        ObjectArray PersonelListeleDaireRapor(Kullanici islemiYapan, Personel p, PersonelMemuriyet pk);
        ObjectArray DonemlikEgitimFaaliyetRaporu(Kullanici kullanan, Personel p, PersonelKurs pk);

        //Personel Belgeleri
        Sonuc PersonelBelgeKaydet(Kullanici islemiYapan, PersonelBelgeler belge);
        ObjectArray PersonelBelgeListele(Kullanici islemiYapan, PersonelBelgeler belge);
        Sonuc PersonelBelgeListesiGetir(Kullanici islemiYapan, PersonelBelgeler belge);
        Sonuc PersonelBelgeSil(Kullanici islemiYapan, PersonelBelgeler belge);

        //Personel Adres
        Sonuc PersonelAdresKaydet(Kullanici islemiYapan, PersonelAdres p);
        ObjectArray PersonelAdresListele(Kullanici islemiYapan, PersonelAdres p);

        //PersonelKurs Bilgisi
        Sonuc PersonelKursKaydet(Kullanici islemiYapan, PersonelKurs p);
        //PersonelKurs Bilgisi
        Sonuc PersonelEgitimTanimKaydet(Kullanici islemiYapan, PersonelEgitimTurleri p);
        ObjectArray PersonelKursListele(Kullanici islemiYapan, PersonelKurs p);
        Sonuc PersonelKursSil(Kullanici islemiYapan, PersonelKurs p);
        ObjectArray PersonelEgitimTurleriListele(Kullanici islemiYapan, PersonelEgitimTurleri p);
        ObjectArray ListeleEgitimTurBilgileriRecursive(Kullanici islemiYapan, int p);
        Sonuc PersonelEgitimTanimSil(Kullanici islemiYapan, PersonelEgitimTurleri p);
        //PersonelAile Bilgisi
        Sonuc PersonelAileKaydet(Kullanici islemiYapan, PersonelAile p);
        ObjectArray PersonelAileListele(Kullanici islemiYapan, PersonelAile p);
        Sonuc PersonelAileSil(Kullanici islemiYapan, PersonelAile p);

        //PersonelMalBildirimi Bilgisi
        Sonuc PersonelMalBildirimiKaydet(Kullanici islemiYapan, PersonelMalBildirimi p);
        ObjectArray PersonelMalBildirimiListele(Kullanici islemiYapan, PersonelMalBildirimi p);
        Sonuc PersonelMalBildirimiSil(Kullanici islemiYapan, PersonelMalBildirimi p);

        //Personel Bilgi Birikimi

        /// <summary>
        /// Personel nitelik bilgilerini varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Personelin sistemde kayıtlı olması gerekmektedir, eğer kayıtlı değilse hata döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc PersonelBilgiBirikimiKaydet(Kullanici kullanan, PersonelBilgiBirikimi personel);
        /// <summary>
        /// Verilen kriterlere uygun olan personel nitelik bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel nitelik listesi döndürülür.</returns>
        ObjectArray PersonelBilgiBirikimiListele(Kullanici kullanan, PersonelBilgiBirikimi personel);


        //PersonelCezaÖdül Bilgisi
        Sonuc PersonelCezaOdulKaydet(Kullanici islemiYapan, PersonelCezaOdul p);
        ObjectArray PersonelCezaOdulListele(Kullanici islemiYapan, PersonelCezaOdul p);
        Sonuc PersonelCezaOdulSil(Kullanici islemiYapan, PersonelCezaOdul p);

        //PersonelÖğrenim Bilgisi
        Sonuc PersonelOgrenimKaydet(Kullanici islemiYapan, PersonelOgrenim p);
        ObjectArray PersonelOgrenimListele(Kullanici islemiYapan, PersonelOgrenim p);
        Sonuc PersonelOgrenimSil(Kullanici islemiYapan, PersonelOgrenim p);

        //PersonelDil Bilgisi
        Sonuc PersonelDilKaydet(Kullanici islemiYapan, PersonelDil p);
        ObjectArray PersonelDilListele(Kullanici islemiYapan, PersonelDil p);
        Sonuc PersonelDilSil(Kullanici islemiYapan, PersonelDil p);

        //PersonelMemuriyet Bilgisi
        Sonuc PersonelMemuriyetKaydet(Kullanici islemiYapan, PersonelMemuriyet p);
        ObjectArray PersonelMemuriyetListele(Kullanici islemiYapan, PersonelMemuriyet p);
        Sonuc PersonelMemuriyetSil(Kullanici islemiYapan, PersonelMemuriyet p);

        //PersonelKadro Bilgisi
        Sonuc PersonelKadroKaydet(Kullanici islemiYapan, PersonelKadro p);
        ObjectArray PersonelKadroListele(Kullanici islemiYapan, PersonelKadro p);
        Sonuc PersonelKadroSil(Kullanici islemiYapan, PersonelKadro p);

        //Personel Resim Bilgisi
        Sonuc PersonelResimKaydet(PersonelResim bilgi);
        Sonuc PersonelResimSil(PersonelResim bilgi);
        ObjectArray PersonelResimGetir(string bilgi);

        //Dil Bilgisi
        Sonuc DilKaydet(Kullanici islemiYapan, Dil p);
        ObjectArray DilListele(Dil p);
        Sonuc DilSil(Kullanici islemiYapan, Dil p);

        //Kurs Bilgisi
        Sonuc KursKaydet(Kullanici islemiYapan, Kurs p);
        ObjectArray KursListele(Kurs p);
        Sonuc KursSil(Kullanici islemiYapan, Kurs p);

        //Banka Bilgisi
        Sonuc BankaKaydet(Kullanici islemiYapan, Banka p);
        ObjectArray BankaListele(Banka p);
        Sonuc BankaSil(Kullanici islemiYapan, Banka p);

        //Banka Şube Bilgisi
        Sonuc BankaSubeKaydet(Kullanici islemiYapan, BankaSube p);
        ObjectArray BankaSubeListele(BankaSube p);
        Sonuc BankaSubeSil(Kullanici islemiYapan, BankaSube p);

        //Öğrenim Bilgisi
        Sonuc OgrenimKaydet(Kullanici islemiYapan, Ogrenim p);
        ObjectArray OgrenimListele(Ogrenim p);
        Sonuc OgrenimSil(Kullanici islemiYapan, Ogrenim p);

        //İzin Tür Bilgisi
        Sonuc IzinTurKaydet(Kullanici islemiYapan, IzinTur p);
        ObjectArray IzinTurListele(IzinTur p);
        Sonuc IzinTurSil(Kullanici islemiYapan, IzinTur p);
        ObjectArray IzinTurIlkKaydaDon();

        // İzin Dönem Bilgileri
        ObjectArray IzinDonemListele(IzinDonem p);

        //Ünvan Bilgisi
        Sonuc UnvanKaydet(Kullanici islemiYapan, Unvan p);
        ObjectArray UnvanListele(Unvan p);
        Sonuc UnvanSil(Kullanici islemiYapan, Unvan p);

        //Sinif Bilgisi
        Sonuc SinifKaydet(Kullanici islemiYapan, Sinif p);
        ObjectArray SinifListele(Sinif p);
        Sonuc SinifSil(Kullanici islemiYapan, Sinif p);

        //Neden Bilgisi
        Sonuc NedenKaydet(Kullanici islemiYapan, Neden p);
        ObjectArray NedenListele(Neden p);
        Sonuc NedenSil(Kullanici islemiYapan, Neden p);


        //UstBirim Bilgisi
        Sonuc UstBirimKaydet(Kullanici islemiYapan, UstBirim p);
        ObjectArray UstBirimListele(UstBirim p);
        Sonuc UstBirimSil(Kullanici islemiYapan, UstBirim p);

        //Kadro Bilgisi
        Sonuc KadroKaydet(Kullanici islemiYapan, ObjectArray p);
        Sonuc KadroSil(Kullanici islemiYapan, TNSDateTime tarih);
        ObjectArray KadroRapor(Kullanici islemiYapan, Kadro p);
        ObjectArray KadroVizeListele();

        //Sözleşmeli Personel Maaş Tanım
        Sonuc SozlesmeliBordroTanimKaydet(Kullanici islemiYapan, SozlesmeliBordroTanim p);
        Sonuc SozlesmeliBordroTanimSil(Kullanici islemiYapan, SozlesmeliBordroTanim kriter);

        ObjectArray OgrenimBazliPersonelSayilariListele(int muhasebeKod, string birimKod);
        ObjectArray UnvanBazliPersonelSayilariListele(int muhasebeKod, string birimKod);
        ObjectArray SinifBazliPersonelSayilariListele(int muhasebeKod, string birimKod);
        ObjectArray UnvanVeKadroDereceBazliPersonelSayilariListele(Kullanici kullanan, KadroKontenjan kk);

        Sonuc KadroKontenjanKaydet(Kullanici kullanan, KadroKontenjan kk);
        Sonuc KadroKontenjanSil(Kullanici kullanan, KadroKontenjan kk);
        KadroKontenjan[] KadroKontenjanListele(Kullanici kullanan, KadroKontenjan kk, bool detay);

        Sonuc PersonelIzinKaydet(Kullanici islemiYapan, PersonelIzin p);
        ObjectArray PersonelIzinListele(Kullanici kullanan, PersonelIzin p);
        Sonuc PersonelIzinSil(Kullanici islemiYapan, PersonelIzin p);
        Sonuc PersonelIzinOnayla(Kullanici islemiYapan, PersonelIzin p);
        Sonuc PersonelIzinEvrakBilgiKaydet(Kullanici islemiYapan, PersonelIzin p);
        Sonuc PersonelIzinAlanKaydet(Kullanici islemiYapan, PersonelIzin p);

        Sonuc PersonelIzinDevirKaydet(Kullanici islemiYapan, PersonelIzinDevir[] p);
        ObjectArray PersonelIzinDevirListele(Kullanici kullanan, PersonelIzinDevir p);
        Sonuc PersonelIzinDevirSil(Kullanici islemiYapan, PersonelIzinDevir p);

        Sonuc PersonelIzinTalepKaydet(Kullanici islemiYapan, PersonelIzinTalep[] p);
        ObjectArray PersonelIzinTalepListele(Kullanici kullanan, PersonelIzinTalep p);

        int PersonelKullanilanIzinHesapla(string tcKimlikNo, int yil, TNSDateTime baslangicTarih, int izinTip);

        ObjectArray PersonelTarihceListele(Kullanici kullanan, PersonelTarihce p);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="personelTCKimlikNo"></param>
        /// <returns>TARIH,KATEGORI,DEGISENALAN,YENIDEGER,ESKIDEGER,DEGISIKLIKYAPAN</returns>
        String[] PersonelTarihceListele(Kullanici kullanan, string personelTCKimlikNo);

        Sonuc GorevKonuKaydet(Kullanici islemiYapan, GorevKonu p);
        ObjectArray GorevKonuListele(GorevKonu p);
        Sonuc GorevKonuSil(Kullanici islemiYapan, GorevKonu p);

        Sonuc IzinEvrakNoKaydet(Kullanici kullanan, string tcKimlikNo, int siraNo, string prEvrakNo);

        string IzinEvrakNoOku(Kullanici kullanan, string tcKimlikNo, int siraNo);

        Sonuc BirimKoduDegistir(Kullanici kullanan, int eskiSaymanlik, string eskiBirim, int yeniSaymanlik, string yeniBirim);

        ObjectArray BirimDaireBilgileriListele(BirimDaireBilgileri daireBilgileri);

        string ListeleEgitimTurleriAdlari(Kullanici kullanan, int kod);

        ObjectArray BirimBazliPersonelSonMemuriyetListele(Kullanici kullanan, Personel p);

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using TNS.KYM;

namespace TNS.ORG
{
    public enum BirimTip
    {
        ButceBirimi = 1,
        ButceKurumu = 2,
        EvrakBirimi = 3,
        HarcamaBirimi = 4,
        PersonelBirimi = 5,
        TasinirBirimi = 6,
        Ambar = 7,
        TANIMSIZ = 999
    }

    [Serializable()]
    public class GenelGorevYetki
    {
        public int yazYetkiSiraNo;
        public BirimGenelGorev birimGenelGorev = null;
        public string yetkiAd = null;
        public Hashtable yetkiDamgalar = null;
        public int yazYetkiIndis;

        public void DamgaEkle(string damgaAd, string damgaDeger)
        {
            if (string.IsNullOrEmpty(damgaDeger)) return;
            if (yetkiDamgalar == null) yetkiDamgalar = new Hashtable();
            yetkiDamgalar.Add(damgaAd, damgaDeger);
        }

        public void DamgaEkle(string damgaAd, bool damgaDeger)
        {
            string tut;
            if (damgaDeger)
                tut = "1";
            else
                tut = null;
            DamgaEkle(damgaAd, tut);
        }

        public void DamgaEkle(string damgaAd, int damgaDeger)
        {
            DamgaEkle(damgaAd, damgaDeger.ToString());
        }

        public int DegerAl(string damgaAd, int def)
        {
            if (yetkiDamgalar == null) return def;
            string tut = (string)yetkiDamgalar[damgaAd];
            return OrtakFonksiyonlar.ConvertToInt(tut, def);
        }

        public string DegerAl(string damgaAd)
        {
            if (yetkiDamgalar == null) return null;
            return (string)yetkiDamgalar[damgaAd];
        }

        public bool DegerAlBool(string damgaAd)
        {
            if (yetkiDamgalar == null) return false;
            string tut = (string)yetkiDamgalar[damgaAd];
            if (string.IsNullOrEmpty(tut)) return false;
            return true;
        }
    }

    [Serializable()]
    public class YazilimYetki
    {
        public int siraNo;
        public int birimGorevlerIndis;
        public string gorevAciklama;
    }

    [Serializable()]
    public class BirimGenelGorev
    {
        public static BirimGenelGorev GorevKodBicimindenGenelGoreve(string gorevKod)
        {
            string birimKodS = "";
            string gorevNoS = "";
            OrtakFonksiyonlar.StringiIkiyeBol(gorevKod, "_", ref birimKodS, ref gorevNoS);

            int birimKod = OrtakFonksiyonlar.ConvertToInt(birimKodS, 0);
            int gorevNo = OrtakFonksiyonlar.ConvertToInt(gorevNoS, 0);
            BirimGenelGorev birimGenelGorev = new BirimGenelGorev();
            birimGenelGorev.OrgKod = birimKod;
            birimGenelGorev.siraNo = gorevNo;
            return birimGenelGorev;
        }

        public OrganizasyonBirim orgBirim = null;
        public int siraNo { get; set; }
        public string ad { get; set; }
        public string aciklama = null;

        public BirimTip[] iliskiliBirimler;
        public YazilimYetki[] yazilimYetkiler;

        public BirimGenelGorev()
        {
            siraNo = 0;
        }

        public BirimGenelGorev(int siraNo, string ad)
        {
            this.siraNo = siraNo;
            this.ad = ad;
        }

        public int OrgKod
        {
            get
            {
                if (orgBirim == null) return 0;
                else return orgBirim.kod;
            }
            set
            {
                if (value == 0) orgBirim = null;
                else
                {
                    if (orgBirim == null) orgBirim = new OrganizasyonBirim();
                    orgBirim.kod = value;
                }
            }
        }

        public string Anahtar
        {
            get
            {
                return OrgKod.ToString() + "_" + siraNo.ToString();
            }
        }

        public int maxGorev = -1;
        public bool zorunlu = false;
    }

    [Serializable()]
    public class BirimGorev
    {
        string gorevKaynakAd = null;
        TNS.KYM.ENUMKullaniciTipi[] iliskiliKullaniciTipi;
        BirimTip[] iliskiliBirimler;
        string sayfaAd;
        public int genislik;
        public int yukseklik;

        private string StringEkle(string onStr, string ekStr, string ek)
        {
            if (string.IsNullOrEmpty(ekStr)) return onStr;
            if (string.IsNullOrEmpty(onStr)) return ekStr;
            return onStr + ek + ekStr;
        }

        public static OrganizasyonBirim OrganizasyonelBirimBul(IORGServis servisPer, int orgKod)
        {
            if (orgKod > 0)
            {
                OrganizasyonBirim ob = new OrganizasyonBirim();
                ob.kod = orgKod;
                //ObjectArray oa = servisPer.OrganizasyonBirimListele(ob);
                //if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
                //{
                //    return (OrganizasyonBirim)oa.objeler[0];
                //}
                //else
                //    return null;
                return ob;
            }
            return null;
        }

        public static OrganizasyonBirim OrganizasyonelBirimOl(int orgKod)
        {
            if (orgKod > 0)
            {
                OrganizasyonBirim ob = new OrganizasyonBirim();
                ob.kod = orgKod;
                return ob;
            }
            return null;
        }

        public BirimGorev(BirimTip[] iliskiliBirimler, TNS.KYM.ENUMKullaniciTipi[] iliskiliKullaniciTipi, string gorevKaynakAd, string sayfaAd, int genislik, int yukseklik)
        {
            this.iliskiliBirimler = iliskiliBirimler;
            this.iliskiliKullaniciTipi = iliskiliKullaniciTipi;
            this.gorevKaynakAd = gorevKaynakAd;
            this.sayfaAd = sayfaAd;
            this.genislik = genislik;
            this.yukseklik = yukseklik;
        }

        public string GorevAd { get { return OrganizasyonSemaResI.ResourceManager.GetString(gorevKaynakAd); } }

        public string SayfaAd { get { return sayfaAd; } }

        public bool BirimTipVarMi(BirimTip[] birimTipler)
        {
            if (birimTipler == null || birimTipler.Length == 0) return true;
            for (int i = 0; i < birimTipler.Length; i++)
            {
                for (int j = 0; j < iliskiliBirimler.Length; j++)
                {
                    if (birimTipler[i] == iliskiliBirimler[j]) return true;
                }
            }
            return false;
        }

        public virtual bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            throw new NotImplementedException();
        }

        protected string YetkiBulIntlisteKYM5(bool gor, bool yaz, bool onayla)
        {
            if (onayla) return "3";
            if (yaz) return "2";
            if (gor) return "1";
            return "";
        }

        protected string YetkiBulIntliste(bool gor, bool yaz, bool onayla, bool onayKaldir, bool yevmiyeKaldir)
        {
            string yetki;
            if (yevmiyeKaldir)
            {
                yetki = "6";
            }
            else if (onayKaldir)
            {
                yetki = "5";
            }
            else if (onayla)
            {
                if (yaz)
                {
                    yetki = "4";
                }
                else
                {
                    yetki = "3";
                }
            }
            else if (yaz)
            {
                yetki = "2";
            }
            else
            {
                yetki = "1";
            }
            return yetki;
        }

        protected string HarcamaKurumsalKodlarBul(OrganizasyonBirim orgBirim)
        {
            if (orgBirim.orgHarcamaKod == null) return null;
            string donusStr = "";
            for (int i = 0; i < orgBirim.orgHarcamaKod.Length; i++)
            {
                if (!string.IsNullOrEmpty(orgBirim.orgHarcamaKod[i].kurKod) || !string.IsNullOrEmpty(orgBirim.orgHarcamaKod[i].fonKod))
                {
                    string ekStr = "";
                    if (string.IsNullOrEmpty(orgBirim.orgHarcamaKod[i].fonKod))
                        ekStr = orgBirim.orgHarcamaKod[i].kurKod;
                    else
                        ekStr = orgBirim.orgHarcamaKod[i].kurKod + "-" + orgBirim.orgHarcamaKod[i].fonKod;
                    if (!donusStr.Contains(ekStr))
                        donusStr = StringEkle(donusStr, ekStr, ";");
                }
            }
            return donusStr;
        }

        protected string HarcamaBirimKodlariBul(OrganizasyonBirim orgBirim, out bool birimVar)
        {
            birimVar = false;
            if (orgBirim.orgHarcamaKod == null) return null;
            if (string.IsNullOrEmpty(orgBirim.muhasebekodu)) return null;
            string donusStr = "";
            string muhasebeKod = orgBirim.muhasebekodu.PadLeft(5, '0');
            for (int i = 0; i < orgBirim.orgHarcamaKod.Length; i++)
            {
                if (!string.IsNullOrEmpty(orgBirim.orgHarcamaKod[i].muhasebeBirimKod))
                {
                    if (orgBirim.orgHarcamaKod[i].muhasebeBirimKod != "000")
                    {
                        birimVar = true;
                    }
                    string ekStr = muhasebeKod + "-" + orgBirim.orgHarcamaKod[i].muhasebeBirimKod;
                    if (!donusStr.Contains(ekStr))
                        donusStr = StringEkle(donusStr, ekStr, ";");
                }
            }
            return donusStr;
        }

        protected string TasinirAmbarKodlariBul(OrganizasyonBirim orgBirim, GenelGorevYetki ggy)
        {
            if (string.IsNullOrEmpty(orgBirim.muhasebekodu)) return null;
            string muhasebeKod = orgBirim.muhasebekodu.PadLeft(5, '0');

            if (orgBirim.ambarMi == 1)
            {
                return muhasebeKod + "-" + orgBirim.kurumsalkod + "." + orgBirim.muhasebebirim + "-" + orgBirim.ambarKod;
            }

            if (orgBirim.orgHarcamaKod == null) return null;
            string donusStr = "";
            for (int i = 0; i < orgBirim.orgHarcamaKod.Length; i++)
            {
                if (!string.IsNullOrEmpty(orgBirim.orgHarcamaKod[i].muhasebeBirimKod))// && !string.IsNullOrEmpty(orgBirim.orgHarcamaKod[i].kurKod)) Merkez Bankasında Kurumsal kod yok
                {
                    string ekStr = "";
                    string ambarKodu = ggy.DegerAl("TasinirAmbarKodu");
                    if (!string.IsNullOrEmpty(ambarKodu))
                    {
                        string[] ambarlar = ambarKodu.Split(';', ',', ' ');
                        foreach (string item in ambarlar)
                        {
                            if (!string.IsNullOrEmpty(item.Trim()))
                            {
                                ekStr = muhasebeKod + "-" + orgBirim.orgHarcamaKod[i].kurKod + "." + orgBirim.orgHarcamaKod[i].muhasebeBirimKod + "-" + item.Trim();
                                if (!donusStr.Contains(ekStr))
                                    donusStr = StringEkle(donusStr, ekStr, ";");
                            }
                        }
                    }
                    else
                    {
                        ekStr = muhasebeKod + "-" + orgBirim.orgHarcamaKod[i].kurKod + "." + orgBirim.orgHarcamaKod[i].muhasebeBirimKod + "-*";

                        if (!donusStr.Contains(ekStr))
                            donusStr = StringEkle(donusStr, ekStr, ";");
                    }
                }
            }

            donusStr = donusStr.Replace(".", "");

            return donusStr;
        }

        internal string FaaliyetKoduBul(OrganizasyonBirim orgBirim, GorevKisitDeger[] kisitDegerler, GenelGorevYetki ggy)
        {
            string faaliyetler = "";

            if (kisitDegerler != null && kisitDegerler.Length >= 2)
            {
                faaliyetler = kisitDegerler[1].deger;
            }

            if (string.IsNullOrWhiteSpace(faaliyetler))
            {
                if (!string.IsNullOrWhiteSpace(orgBirim.altFaaliyetKodlar))
                    faaliyetler = orgBirim.altFaaliyetKodlar;
            }

            if (string.IsNullOrWhiteSpace(faaliyetler))
            {
                faaliyetler = ggy.DegerAl("Faaliyet");
            }
            if (string.IsNullOrEmpty(faaliyetler)) faaliyetler = "*";
            return faaliyetler;
        }
    }

    public class BG_OdenekGonderir : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string ekonomikKodlar = ggy.DegerAl("Ekonomik");
            if (string.IsNullOrEmpty(ekonomikKodlar)) ekonomikKodlar = "*";
            string birimKod;
            if (orgBirim.kurumMu == 1)
            {
                birimKod = orgBirim.kurumKod;
            }
            else
            {
                birimKod = orgBirim.BirimKodlariOl(ggy.DegerAlBool("Merkez"), ggy.DegerAlBool("Tasra"), ggy.DegerAlBool("Yurtdisi"));
            }

            if (string.IsNullOrEmpty(birimKod)) return false;
            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            kullanan.yetkiTipiDegerleri["Ödenek Gönderme Hazırlayacağı Birimler", grupSay] = birimKod;
            kullanan.yetkiTipiDegerleri["Ödenek Gönderme Yetkisi", grupSay] = yetki;
            kullanan.yetkiTipiDegerleri["Ödenek Gönderme Ekonomik Kod", grupSay] = ekonomikKodlar;
            kullanan.yetkiTipiDegerleri["Ödenek Gönderme Faaliyet Kod", grupSay] = FaaliyetKoduBul(orgBirim, kisitDegerler, ggy);
            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.ODENEKGONDERME);
            return true;
        }

        public BG_OdenekGonderir()
            : base(new BirimTip[] { BirimTip.ButceBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.ODENEKGONDERME }, "PERBRMGRV001", "YETODENEKGONDERME.aspx", 620, 300)
        {

        }
    }

    public class BG_ButceHazirlik : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string faaliyetler = FaaliyetKoduBul(orgBirim, kisitDegerler, ggy);

            string yazAsamalar;
            string gorAsamalar;
            string birimKod;

            if (orgBirim.kurumMu == 1)
            {
                gorAsamalar = "00;01;02;03;04;05;06;07;08;09";
                yazAsamalar = gorAsamalar;
                birimKod = orgBirim.kurumKod;
            }
            else if (orgBirim.butceMi == 1)
            {
                yazAsamalar = "00";
                gorAsamalar = "00;03,04;05;06";
                birimKod = orgBirim.BirimKodlariOl(ggy.DegerAlBool("Merkez"), ggy.DegerAlBool("Tasra"), ggy.DegerAlBool("Yurtdisi"));
            }
            else
                return false;

            if (string.IsNullOrEmpty(birimKod))
            {
                SabitGorevlerArac.DebugYaz($"{kullanan.ad} için {orgBirim.adi} birimine ait kurumsal kodlar bulunamadığından yetki ataması yapılmadı!");
                return false;
            }

            if (ggy.DegerAlBool("Oku") || ggy.DegerAlBool("Yaz") || ggy.DegerAlBool("Onayla"))
            {
                kullanan.yetkiTipiDegerleri["Bütçe Hazırlığı Görebileceği Birimler", grupSay] = birimKod;
                kullanan.yetkiTipiDegerleri["Bütçe Hazırlığı Görebileceği Aşamalar", grupSay] = gorAsamalar;
                kullanan.yetkiTipiDegerleri["Bütçe Hazırlığı Görebileceği Altfaaliyetler", grupSay] = faaliyetler;
            }
            if (ggy.DegerAlBool("Yaz"))
            {
                kullanan.yetkiTipiDegerleri["Bütçe Hazırlığı Kaydedebileceği Birimler", grupSay] = birimKod;
                kullanan.yetkiTipiDegerleri["Bütçe Hazırlığı Kaydedebileceği Aşamalar", grupSay] = yazAsamalar;
                kullanan.yetkiTipiDegerleri["Bütçe Hazırlığı Kaydedebileceği Altfaaliyetler", grupSay] = faaliyetler;
            }
            if (ggy.DegerAlBool("Onayla"))
            {
                kullanan.yetkiTipiDegerleri["Bütçe Hazırlığı Onaylayabileceği Birimler", grupSay] = birimKod;
                kullanan.yetkiTipiDegerleri["Bütçe Hazırlığı Onaylayabileceği Aşamalar", grupSay] = yazAsamalar;
                kullanan.yetkiTipiDegerleri["Bütçe Hazırlığı Onaylayabileceği Altfaaliyetler", grupSay] = faaliyetler;
            }
            if (ggy.DegerAlBool("Proje"))
            {
                kullanan.yetkiTipiDegerleri["Yatırım Hazırlığı Yapacağı Birimler", grupSay] = birimKod;
                kullanan.yetkiTipiDegerleri["Yatırım Hazırlığı Yetkisi", grupSay] = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.YATIRIMPROJELERI);
            }
            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.BUTCEHAZIRLIK);
            return true;
        }

        public BG_ButceHazirlik()
            : base(new BirimTip[] { BirimTip.ButceBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.BUTCEHAZIRLIK }, "PERBRMGRV002", "YETBUTCEHAZIRLIK.aspx", 620, 300)
        {

        }
    }

    public class BG_BütceIslemYapar : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;

            string yazAsamalar;
            string gorAsamalar;
            string birimKod;
            if (orgBirim.kurumMu == 1)
            {
                gorAsamalar = "0;1;2;3;4;5;6;7;8;9";
                yazAsamalar = gorAsamalar;
                birimKod = orgBirim.kurumKod;
            }
            else if (orgBirim.butceMi == 1)
            {
                yazAsamalar = "0";
                gorAsamalar = "0;1;2;3;4;5;6";
                birimKod = orgBirim.BirimKodlariOl(ggy.DegerAlBool("Merkez"), ggy.DegerAlBool("Tasra"), ggy.DegerAlBool("Yurtdisi"));
            }
            else
                return false;

            if (string.IsNullOrEmpty(birimKod)) return false;

            if (ggy.DegerAlBool("Oku") || ggy.DegerAlBool("Yaz") || ggy.DegerAlBool("Onayla"))
            {
                kullanan.yetkiTipiDegerleri["Bütçe Uygulama Görme Yetki Şablonu", grupSay] = birimKod;
                kullanan.yetkiTipiDegerleri["Bütçe Uygulama Göreceği Aşamalar", grupSay] = gorAsamalar;
            }
            if (ggy.DegerAlBool("Yaz"))
            {
                kullanan.yetkiTipiDegerleri["Bütçe Uygulama Kayıt Yetki Şablonu", grupSay] = birimKod;
                kullanan.yetkiTipiDegerleri["Bütçe Uygulama Kaydedeceği Aşamalar", grupSay] = yazAsamalar;
            }
            if (ggy.DegerAlBool("Onayla"))
            {
                kullanan.yetkiTipiDegerleri["Bütçe Uygulama Onay Yetki Şablonu", grupSay] = birimKod;
                kullanan.yetkiTipiDegerleri["Bütçe Uygulama Onaylayacağı Aşamalar", grupSay] = yazAsamalar;
            }
            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.BUTCEUYGULAMA);
            return true;
        }

        public BG_BütceIslemYapar()
            : base(new BirimTip[] { BirimTip.ButceBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.BUTCEUYGULAMA }, "PERBRMGRV003", "YETBUTCEUYGULAMA.aspx", 620, 300)
        {

        }
    }

    public class BG_YatirimProjesi : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod;
            if (orgBirim.kurumMu == 1)
            {
                birimKod = orgBirim.kurumKod;
            }
            else if (orgBirim.butceMi == 1)
            {
                birimKod = orgBirim.BirimKodlariOl(ggy.DegerAlBool("Merkez"), ggy.DegerAlBool("Tasra"), ggy.DegerAlBool("Yurtdisi"));
            }
            else
                return false;

            if (string.IsNullOrEmpty(birimKod)) return false;
            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            kullanan.yetkiTipiDegerleri["Yatırım Hazırlığı Yapacağı Birimler", grupSay] = birimKod;
            kullanan.yetkiTipiDegerleri["Yatırım Hazırlığı Yetkisi", grupSay] = yetki;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.YATIRIMPROJELERI);
            return true;
        }

        public BG_YatirimProjesi()
            : base(new BirimTip[] { BirimTip.ButceBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.YATIRIMPROJELERI }, "PERBRMGRV004", "YETYATIRIMPROJELERI.aspx", 620, 300)
        {

        }
    }

    public class BG_KesinHesap : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod;
            if (orgBirim.kurumMu == 1)
            {
                birimKod = orgBirim.kurumKod;
            }
            else if (orgBirim.butceMi == 1)
            {
                birimKod = orgBirim.BirimKodlariOl(ggy.DegerAlBool("Merkez"), ggy.DegerAlBool("Tasra"), ggy.DegerAlBool("Yurtdisi"));
            }
            else
                return false;

            if (string.IsNullOrEmpty(birimKod)) return false;
            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            kullanan.yetkiTipiDegerleri["Kesin Hesap Yapacağı Birimler", grupSay] = birimKod;
            kullanan.yetkiTipiDegerleri["Kesin Hesap Yetkisi", grupSay] = yetki;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.KESINHESAP);
            return true;
        }

        public BG_KesinHesap()
            : base(new BirimTip[] { BirimTip.ButceBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.KESINHESAP }, "PERBRMGRV005", "YETKESINHESAP.aspx", 620, 300)
        {

        }
    }

    public class BG_StratejikPlan : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod;
            string kopruKod = orgBirim.kopruKod;
            if (orgBirim.kurumMu == 1)
            {
                birimKod = orgBirim.kurumKod;
            }
            else if (orgBirim.butceMi == 1)
            {
                birimKod = orgBirim.BirimKodlariOl(ggy.DegerAlBool("Merkez"), ggy.DegerAlBool("Tasra"), ggy.DegerAlBool("Yurtdisi"));
            }
            else if (orgBirim.harcamaMi == 1)
            {
                birimKod = "";
            }
            else
                return false;

            if (string.IsNullOrEmpty(birimKod)) return false;
            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            kullanan.yetkiTipiDegerleri["Stratejik Plan Yapacağı Birimler", grupSay] = birimKod;
            kullanan.yetkiTipiDegerleri["Stratejik Plan Yetkisi", grupSay] = yetki;
            kullanan.yetkiTipiDegerleri["Stratejik Plan Kopru Kod", grupSay] = kopruKod;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.STRATEJIKPLAN);
            return true;
        }

        public BG_StratejikPlan()
            : base(new BirimTip[] { BirimTip.ButceBirimi, BirimTip.ButceKurumu, BirimTip.HarcamaBirimi }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.STRATEJIKPLAN }, "PERBRMGRV006", "YETSTRATEJIKPLAN.aspx", 620, 300)
        {

        }
    }

    public class BG_PerformansButce : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod;
            if (orgBirim.kurumMu == 1)
            {
                birimKod = orgBirim.kurumKod;
            }
            else if (orgBirim.butceMi == 1)
            {
                birimKod = orgBirim.BirimKodlariOl(ggy.DegerAlBool("Merkez"), ggy.DegerAlBool("Tasra"), ggy.DegerAlBool("Yurtdisi"));
            }
            else
                return false;

            if (string.IsNullOrEmpty(birimKod)) return false;
            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            kullanan.yetkiTipiDegerleri["Performans Bütçe Yapacağı Birimler", grupSay] = birimKod;
            kullanan.yetkiTipiDegerleri["Performans Bütçe Tedbir Yapacağı Birimler", grupSay] = birimKod;
            kullanan.yetkiTipiDegerleri["Performans Bütçe Yetkisi", grupSay] = yetki;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.PERFORMANSBUTCE);
            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.PERFORMANSBUTCETEDBIR);
            return true;
        }

        public BG_PerformansButce()
            : base(new BirimTip[] { BirimTip.ButceBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.PERFORMANSBUTCE, KYM.ENUMKullaniciTipi.PERFORMANSBUTCETEDBIR }, "PERBRMGRV007", "YETPERFORMANSBUTCE.aspx", 620, 300)
        {

        }
    }

    public class BG_EvrakIslemleri : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            //08.01.2017 Hüseyin - Manas için Evrak Belge Yöneticisi - Geçici Yöntem
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod = null;
            if (orgBirim.evrakMi == 1)
            {
                birimKod = orgBirim.evrakbirim;
            }
            if (string.IsNullOrEmpty(birimKod)) return false;


            int evrakGorev = ggy.DegerAl("Gorev", 0);
            if (evrakGorev >= 1 && evrakGorev <= 3)
            {

                kullanan.yetkiTipiDegerleri["Evrak Yetki Şablonu", grupSay] = evrakGorev.ToString();
                kullanan.yetkiTipiDegerleri["Yetkili Olduğu Evrak Birimleri", grupSay] = birimKod;

                if (ggy.DegerAlBool("Imza"))
                {
                    kullanan.yetkiTipiDegerleri["Evrak Onay Yetki Şablonu", grupSay] = "1";
                }
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.EVRAK);
            }
            else if (ggy.DegerAl("BelgeYoneticisi") == "53")
            {
                kullanan.yetkiTipiDegerleri["Evrak Belge Yöneticisi", grupSay] = "1";
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI);
            }
            else
                return false;

            return true;
        }

        public BG_EvrakIslemleri()
            : base(new BirimTip[] { BirimTip.EvrakBirimi }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.EVRAK, ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI }, "PERBRMGRV008", "YETEVRAK.aspx", 620, 500)
        {

        }
    }

    public class BG_OdemeEmri : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            bool birimVar = false;
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string ekonomikKodlar = ggy.DegerAl("Ekonomik");
            if (string.IsNullOrEmpty(ekonomikKodlar)) ekonomikKodlar = "*";
            string birimKod;
            string saymanlikBirim;

            if (orgBirim.kurumMu == 1)
            {
                birimKod = orgBirim.kurumKod;
                saymanlikBirim = "*-*";
            }
            else if (orgBirim.harcamaMi == 1)
            {
                birimKod = HarcamaKurumsalKodlarBul(orgBirim);
                saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
            }
            else
            {
                return false;
            }

            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), ggy.DegerAlBool("OnayKaldir"), ggy.DegerAlBool("YevmiyeIptal"));

            kullanan.yetkiTipiDegerleri["Ödeme Emri/MİF Hazırlayacağı Birimler", grupSay] = birimKod;
            kullanan.yetkiTipiDegerleri["Ödeme Emri/MİF Saymanlık-Birim Yetkisi", grupSay] = saymanlikBirim;
            kullanan.yetkiTipiDegerleri["Ödeme Emri/MİF Yetkisi", grupSay] = yetki;
            kullanan.yetkiTipiDegerleri["Ödeme Emri/MİF Ekonomik Kod", grupSay] = ekonomikKodlar;
            kullanan.yetkiTipiDegerleri["Ödeme Emri/MİF Faaliyet Kod", grupSay] = FaaliyetKoduBul(orgBirim, kisitDegerler, ggy);
            kullanan.yetkiTipiDegerleri["Yevmiye Veremez", grupSay] = (ggy.DegerAlBool("OnayKaldir") || ggy.DegerAlBool("YevmiyeIptal")) ? "" : "*";

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.ODEMEEMRI_MIF);
            //Melih tarafından kaldırıldı 11.09.2019 Muhasebe Yetkilisinin hesap planı tanımlama, yıl kapatma gibi geniş yetkisi var
            if (ggy.DegerAlBool("OnayKaldir"))
            {
                if (birimVar)
                    kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.MUHASEBE_BIRIM_YONETICISI);
                else
                    kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.MUHASEBEYONETICISI);
            }

            return true;
        }

        public BG_OdemeEmri()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.ODEMEEMRI_MIF }, "PERBRMGRV009", "YETODEMEEMRI_MIF.aspx", 620, 300)
        {

        }
    }

    public class BG_AlacakTakip : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod;
            string saymanlikBirim;

            if (orgBirim.harcamaMi == 1)
            {
                birimKod = HarcamaKurumsalKodlarBul(orgBirim);
                bool birimVar;
                saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
            }
            else if (orgBirim.kurumMu == 1)
            {
                birimKod = orgBirim.kurumKod;
                saymanlikBirim = "*-*";
            }
            else
            {
                return false;
            }

            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            kullanan.yetkiTipiDegerleri["Alacak Takibi Yapacağı Birimler", grupSay] = birimKod;
            kullanan.yetkiTipiDegerleri["Alacak Takibi Yapacağı Saymanlık-Birim", grupSay] = saymanlikBirim;
            kullanan.yetkiTipiDegerleri["Alacak Takibi Yetkisi", grupSay] = yetki;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.ALACAKTAKIP);
            return true;
        }

        public BG_AlacakTakip()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.ALACAKTAKIP }, "PERBRMGRV010", "YETALACAKTAKIP.aspx", 620, 300)
        {

        }
    }

    public class BG_TeminatMektubu : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod;
            string saymanlikBirim;

            if (orgBirim.harcamaMi == 1)
            {
                birimKod = HarcamaKurumsalKodlarBul(orgBirim);
                bool birimVar;
                saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
            }
            else
            {
                return false;
            }

            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Siler"), false, false);

            kullanan.yetkiTipiDegerleri["Teminat Mektubu Yetkisi", grupSay] = yetki;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.TEMINATMEKTUBU);
            return true;
        }

        public BG_TeminatMektubu()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.TEMINATMEKTUBU }, "PERBRMGRV020", "YETTEMINATMEKTUBU.aspx", 620, 300)
        {

        }
    }

    public class BG_TaahhutKarti : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod;
            string saymanlikBirim;

            if (orgBirim.harcamaMi == 1)
            {
                birimKod = HarcamaKurumsalKodlarBul(orgBirim);
                bool birimVar;
                saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
            }
            else
            {
                return false;
            }

            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Siler"), false, false);

            kullanan.yetkiTipiDegerleri["Taahhut Karti Yetkisi", grupSay] = yetki;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.TAAHHUTKARTI);
            return true;
        }

        public BG_TaahhutKarti()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.TAAHHUTKARTI }, "PERBRMGRV022", "YETTAAHHUTKARTI.aspx", 620, 300)
        {

        }
    }

    public class BG_KurumProjeleri : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string saymanlikBirim;

            if (orgBirim.harcamaMi == 1)
            {
                bool birimVar;
                saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
            }
            else
            {
                saymanlikBirim = "*-*";
            }

            YetkiAta(kullanan, grupSay, ggy.DegerAlBool("Oku"), "Proje Kaydedebileceği Birimler", saymanlikBirim);
            YetkiAta(kullanan, grupSay, ggy.DegerAlBool("Yaz"), "Proje Görebileceği Birimler", saymanlikBirim);
            YetkiAta(kullanan, grupSay, ggy.DegerAlBool("Onayla"), "Proje Onaylayabileceği Birimler", saymanlikBirim);

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.KURUMPROJELERI);
            return true;
        }

        private void YetkiAta(Kullanici kullanan, int grupSay, bool yetkiAta, string yetkiKod, string saymanlikBirim)
        {
            if (yetkiAta)
                kullanan.yetkiTipiDegerleri[yetkiKod, grupSay] = saymanlikBirim;
        }

        public BG_KurumProjeleri()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.KURUMPROJELERI }, "PERBRMGRV023", "YETKURUMPROJELERI.aspx", 620, 300)
        {

        }
    }

    public class BG_DunyaBankasiProjeleri : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;

            if (ggy.DegerAlBool("Yonetici"))
            {
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI);
            }
            else
            {
                string birimKod = orgBirim.BirimKodlariOl(true, false, false);
                if (string.IsNullOrEmpty(birimKod)) return false;
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.DUNYABANKASIBIRIMKULLANICISI);
                kullanan.yetkiTipiDegerleri["Dünya Bankası Proje Birimi", grupSay] = birimKod;
            }

            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            kullanan.yetkiTipiDegerleri["Dünya Bankası İşlem Yetkisi", grupSay] = yetki;
            return true;
        }

        private void YetkiAta(Kullanici kullanan, int grupSay, bool yetkiAta, string yetkiKod, string saymanlikBirim)
        {
            if (yetkiAta)
                kullanan.yetkiTipiDegerleri[yetkiKod, grupSay] = saymanlikBirim;
        }

        public BG_DunyaBankasiProjeleri()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI, TNS.KYM.ENUMKullaniciTipi.DUNYABANKASIBIRIMKULLANICISI }, "PERBRMGRV024", "YETDUNYABANKASIPROJELERI.aspx", 620, 300)
        {

        }
    }

    public class BG_SayistayBulguYonetimi: BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;

            if (ggy.DegerAlBool("Yonetici"))
            {
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.SAYISTAYBULGUYONETICISI);
            }
            else 
            {
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.SAYISTAYBULGUKULLANICISI);
                string saymanlikBirim;
                if (orgBirim.harcamaMi == 1)
                {
                    string birimKod = HarcamaKurumsalKodlarBul(orgBirim);
                    bool birimVar;
                    saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
                }
                else
                    saymanlikBirim = "99999-999";
                kullanan.yetkiTipiDegerleri["Sayıştay Bulgu Saymanlık-Birim Yetkisi", grupSay] = saymanlikBirim;
            }

            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            kullanan.yetkiTipiDegerleri["Sayıştay Bulgu İşlem Yetkisi", grupSay] = yetki;
            return true;
        }

        private void YetkiAta(Kullanici kullanan, int grupSay, bool yetkiAta, string yetkiKod, string saymanlikBirim)
        {
            if (yetkiAta)
                kullanan.yetkiTipiDegerleri[yetkiKod, grupSay] = saymanlikBirim;
        }

        public BG_SayistayBulguYonetimi()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi, BirimTip.ButceKurumu}, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.SAYISTAYBULGUYONETICISI, TNS.KYM.ENUMKullaniciTipi.SAYISTAYBULGUKULLANICISI}, "PERBRMGRV025", "YETSAYISTAYBULGUYONETIMI.aspx", 620, 300)
        {

        }
    }

    public class BG_PersonelIslemleri : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod;
            string saymanlikBirim;
            string yetki = YetkiBulIntlisteKYM5(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"));

            if (orgBirim.harcamaMi == 1)
            {
                birimKod = HarcamaKurumsalKodlarBul(orgBirim);
                bool birimVar;
                saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
                kullanan.yetkiTipiDegerleri["Personel Saymanlık-Birim Yetkisi", grupSay] = saymanlikBirim;
            }
            kullanan.yetkiTipiDegerleri["Personel Organizasyon Yetkisi", grupSay] = orgBirim.kod.ToString();
            kullanan.yetkiTipiDegerleri["KopruKod", grupSay] = orgBirim.kopruKod;
            kullanan.yetkiTipiDegerleri["Personel Yetkisi", grupSay] = yetki;

            kullanan.yetkiTipiDegerleri["OkuIs", grupSay] = ggy.DegerAl("OkuIs");
            kullanan.yetkiTipiDegerleri["YazIs", grupSay] = ggy.DegerAl("YazIs");
            kullanan.yetkiTipiDegerleri["OnayIs", grupSay] = ggy.DegerAl("OnayIs");

            kullanan.yetkiTipiDegerleri["GORPERTIP", grupSay] = ggy.DegerAl("GORPERTIP");
            kullanan.yetkiTipiDegerleri["YAZPERTIP", grupSay] = ggy.DegerAl("YAZPERTIP");
            kullanan.yetkiTipiDegerleri["ONAPERTIP", grupSay] = ggy.DegerAl("ONAPERTIP");

            kullanan.yetkiTipiDegerleri["AltBirim", grupSay] = ggy.DegerAlBool("AltBirim") ? "1" : "0";

            kullanan.yetkiTipiDegerleri["Personel Kullanıcı Yönetir", grupSay] = ggy.DegerAlBool("KY") ? "1" : "";
            kullanan.yetkiTipiDegerleri["Personel Kullanıcı Görev Listesi", grupSay] = ggy.DegerAl("KY_Liste");
            if (ggy.DegerAlBool("Onayla"))
            {
                kullanan.yetkiTipiDegerleri["İzin-Devir Bilgisi Girebilir", grupSay] = "1";
                if (orgBirim.kopruKod.Length <= 3 && ggy.DegerAlBool("AltBirim"))
                    kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.PERSONEL_YONETICISI);
            }

            string sayfa = ggy.DegerAl("AcilisMasa");
            if (!string.IsNullOrWhiteSpace(sayfa))
                kullanan.yetkiTipiDegerleri["AcilisMasa", grupSay] = sayfa;
            sayfa = ggy.DegerAl("AcilisTab");
            if (!string.IsNullOrWhiteSpace(sayfa))
                kullanan.yetkiTipiDegerleri["AcilisTab", grupSay] = sayfa;
            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.PERSONEL);
            return true;
        }

        public BG_PersonelIslemleri()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi, BirimTip.PersonelBirimi }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.PERSONEL }, "PERBRMGRV011", "YETPERSONEL.aspx", 620, 600)
        {

        }
    }

    public class BG_KadroIslemleri : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod;

            if (orgBirim.kurumMu == 1)
            {
                birimKod = orgBirim.kurumKod;
            }
            else
            {
                return false;
            }

            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            kullanan.yetkiTipiDegerleri["Kadro Yetkisi", grupSay] = yetki;
            kullanan.yetkiTipiDegerleri["Kadro Yapacağı Kurum", grupSay] = birimKod;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.KADRO);
            return true;
        }

        public BG_KadroIslemleri()
            : base(new BirimTip[] { BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.KADRO }, "PERBRMGRV012", "YETKADRO.aspx", 620, 300)
        {

        }
    }

    public class BG_KullanicilariYonetir : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string saymanlikBirim;

            if (orgBirim.harcamaMi == 1)
            {
                bool birimVar;
                saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
            }
            else
            {
                return false;
            }

            kullanan.yetkiTipiDegerleri["Kurum", grupSay] = saymanlikBirim;
            kullanan.yetkiTipiDegerleri["KopruKod", grupSay] = orgBirim.kopruKod;


            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.KULLANICIYONETICISI);
            return true;
        }

        public BG_KullanicilariYonetir()
            : base(new BirimTip[] { BirimTip.ButceKurumu, BirimTip.HarcamaBirimi }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.KULLANICIYONETICISI }, "PERBRMGRV013", "YETKULLANICIYONETICISI.aspx", 620, 300)
        {

        }
    }

    public class BG_TasinirIslemleri : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string saymanlikBirim;

            if (orgBirim.kurumMu == 1)
            {
                saymanlikBirim = "*-*-*";
            }
            else if (orgBirim.harcamaMi == 1 || orgBirim.tasinirMi == 1 || orgBirim.ambarMi == 1)
            {
                saymanlikBirim = TasinirAmbarKodlariBul(orgBirim, ggy);
            }
            else
            {
                return false;
            }

            int ekle = 0;
            if (ggy.DegerAlBool("MalKaydi"))
            {
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.TASINIRMALKAYDI);
                ekle++;
            }
            if (ggy.DegerAlBool("Konsolide"))
            {
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI);
                ekle++;
            }
            if (ggy.DegerAlBool("MalYoneticisi"))
            {
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.TASINIRMALYONETICISI);
                ekle++;
            }
            if (ggy.DegerAlBool("KullaniciBirim"))
            {
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.TASINIRKULLANICIBIRIM);
                ekle++;
            }
            if (ggy.DegerAlBool("TuketimYilDevri"))
            {
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.TASINIR_MAL_YIL_DEVRI_MUDAHALE_YETKISI);
                ekle++;
            }
            if (ggy.DegerAl("TasinirIslemTuru") == "4")
            {
                kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.PERSONEL);
                ekle++;
            }

            if (ekle == 0)
                return false;

            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            string birimYetki = ggy.DegerAl("MuhasebeHarcamaYetki");

            if (ggy.DegerAlBool("KullaniciBirim"))
            {
                if (string.IsNullOrWhiteSpace(birimYetki))
                    kullanan.yetkiTipiDegerleri["Taşınır Kullanıcı Birimi Yetkisi", grupSay] = saymanlikBirim;
                else
                    kullanan.yetkiTipiDegerleri["Taşınır Kullanıcı Birimi Yetkisi", grupSay] = birimYetki;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(birimYetki))
                    kullanan.yetkiTipiDegerleri["Taşınır Muhasebe Harcama Birim Yetkisi", grupSay] = saymanlikBirim;
                else
                    kullanan.yetkiTipiDegerleri["Taşınır Muhasebe Harcama Birim Yetkisi", grupSay] = birimYetki;
            }

            //TasinirIslemTuru Demirbaş, Gayrimenkul, Yazılım, Personel
            if (ggy.DegerAlBool("TasinirIslemTuru"))
                kullanan.yetkiTipiDegerleri["TasinirIslemTuru", grupSay] = ggy.DegerAl("TasinirIslemTuru");
            else
                kullanan.yetkiTipiDegerleri["TasinirIslemTuru", grupSay] = "0";

            if (ggy.DegerAl("TasinirIslemTuru") == "4")//Personel
            {
                kullanan.yetkiTipiDegerleri["Personel Yetkisi", grupSay] = "2";//Yazma yetkisi

                if (string.IsNullOrWhiteSpace(birimYetki))
                    kullanan.yetkiTipiDegerleri["Personel Saymanlık-Birim Yetkisi", grupSay] = saymanlikBirim;
                else
                    kullanan.yetkiTipiDegerleri["Personel Saymanlık-Birim Yetkisi", grupSay] = birimYetki;
            }

            //Onaylama B/A Onayı gibi
            if (ggy.DegerAlBool("Onayla"))
                kullanan.yetkiTipiDegerleri["TasinirIslemOnay", grupSay] = ggy.DegerAl("Onayla");
            else
                kullanan.yetkiTipiDegerleri["TasinirIslemOnay", grupSay] = "";

            //Amortisman Yapabilir
            if (ggy.DegerAlBool("AmortismanYapar"))
                kullanan.yetkiTipiDegerleri["AmortismanYapar", grupSay] = ggy.DegerAl("AmortismanYapar");
            else
                kullanan.yetkiTipiDegerleri["AmortismanYapar", grupSay] = "";

            kullanan.yetkiTipiDegerleri["Taşınır Yetki Şablonu", grupSay] = yetki;
            return true;
        }

        public BG_TasinirIslemleri()
            : base(new BirimTip[] { BirimTip.TasinirBirimi, BirimTip.HarcamaBirimi, BirimTip.Ambar }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.TASINIRMALKAYDI }, "PERBRMGRV014", "YETTASINIRMALKAYDI.aspx", 800, 400)
        {

        }
    }

    public class BG_SistemYoneticisi : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;

            if (orgBirim.kurumMu != 1)
                return false;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.SISTEMYONETICISI);
            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.TASINIR_MAL_YIL_DEVRI_MUDAHALE_YETKISI);
            return true;
        }

        public BG_SistemYoneticisi()
            : base(new BirimTip[] { BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.SISTEMYONETICISI }, "PERBRMGRV015", "YETSISTEMYONETICISI.aspx", 620, 300)
        {

        }
    }

    public class BG_KutuphaneIslemleri : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string Kutuphaneler;

            if (orgBirim.kurumMu != 1)
            {
                return false;
            }

            Kutuphaneler = ggy.DegerAl("Kutuphaneler");
            string yetki = YetkiBulIntliste(ggy.DegerAlBool("Oku"), ggy.DegerAlBool("Yaz"), ggy.DegerAlBool("Onayla"), false, false);

            kullanan.yetkiTipiDegerleri["Kütüphane Listesi", grupSay] = Kutuphaneler;
            kullanan.yetkiTipiDegerleri["Kütüphane Yetkisi", grupSay] = yetki;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.KUTUPHANE);
            return true;
        }

        public BG_KutuphaneIslemleri()
            : base(new BirimTip[] { BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.KUTUPHANE }, "PERBRMGRV016", "YETKUTUPHANE.aspx", 620, 300)
        {

        }
    }

    public class BG_KimlikPaylasimi : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string saymanlikBirim;
            if (orgBirim.harcamaMi == 1)
            {
                bool birimVar;
                saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
            }
            else
            {
                return false;
            }

            kullanan.yetkiTipiDegerleri["Kimlik Paylaşım Saymanlık-Birim Yetkisi", grupSay] = saymanlikBirim;
            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.KIMLIKPAYLASIMI);
            return true;
        }

        public BG_KimlikPaylasimi()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.KIMLIKPAYLASIMI }, "PERBRMGRV017", "YETKIMLIKPAYLASIMI.aspx", 620, 300)
        {

        }
    }

    public class BG_GorevYonetimSistemi : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;

            string birimKod = orgBirim.kod.ToString();
            string yetki = "";

            kullanan.yetkiTipiDegerleri["Görev Yönetimi Sistemi Birimi", grupSay] = birimKod;
            kullanan.yetkiTipiDegerleri["Görev Yönetimi Sistemi Rolü", grupSay] = yetki;

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.GOREVYONETIMIKULLANICISI);
            return true;
        }

        public BG_GorevYonetimSistemi()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi, BirimTip.ButceKurumu }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.GOREVYONETIMIKULLANICISI }, "PERBRMGRV018", "YETGOREVYONETIMSISTEMI.aspx", 620, 300)
        {

        }
    }

    public class BG_SatinAlmaIslemiYapar : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string saymanlikBirim;

            if (orgBirim.harcamaMi == 1)
            {
                bool birimVar;
                saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
            }
            else
            {
                return false;
            }

            kullanan.yetkiTipiDegerleri["Kurum", grupSay] = saymanlikBirim;
            kullanan.yetkiTipiDegerleri["KopruKod", grupSay] = orgBirim.kopruKod;


            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.SATINALMAGOREVLISI_MANAS);
            return true;
        }

        public BG_SatinAlmaIslemiYapar()
            : base(new BirimTip[] { BirimTip.ButceKurumu, BirimTip.HarcamaBirimi }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.SATINALMAGOREVLISI_MANAS }, "PERBRMGRV019", "YETSATINALMA.aspx", 620, 300)
        {

        }
    }

    public class BG_HarcamaIslemleri : BirimGorev
    {
        public override bool KullaniciYetkiOl(int grupSay, Hashtable damgalar, OrganizasyonBirim orgBirim, Kullanici kullanan, GorevKisitDeger[] kisitDegerler)
        {
            GenelGorevYetki ggy = new GenelGorevYetki();
            ggy.yetkiDamgalar = damgalar;
            string birimKod;
            string saymanlikBirim;

            if (orgBirim.kurumMu == 1)
            {
                saymanlikBirim = "*-*";
                kullanan.yetkiTipiDegerleri["Ödeme Emri/MİF Saymanlık-Birim Yetkisi", grupSay] = saymanlikBirim;
            }
            else if (orgBirim.harcamaMi == 1)
            {
                birimKod = HarcamaKurumsalKodlarBul(orgBirim);
                bool birimVar;
                saymanlikBirim = HarcamaBirimKodlariBul(orgBirim, out birimVar);
                kullanan.yetkiTipiDegerleri["Ödeme Emri/MİF Saymanlık-Birim Yetkisi", grupSay] = saymanlikBirim;
            }

            kullanan.yetkiTipiDegerleri["Görebileceği Adımlar", grupSay] = ggy.DegerAl("Görebileceği Adımlar");
            kullanan.yetkiTipiDegerleri["Yapabileceği Adımlar", grupSay] = ggy.DegerAl("Yapabileceği Adımlar");
            kullanan.yetkiTipiDegerleri["Onaylayabileceği Adımlar", grupSay] = ggy.DegerAl("Onaylayabileceği Adımlar");

            kullanan.KullaniciTipiEkle(ENUMKullaniciTipi.HARCAMAISLEMLERI);
            return true;
        }

        public BG_HarcamaIslemleri()
            : base(new BirimTip[] { BirimTip.HarcamaBirimi }, new TNS.KYM.ENUMKullaniciTipi[] { TNS.KYM.ENUMKullaniciTipi.PERSONEL }, "PERBRMGRV021", "YETHARCAMA.aspx", 620, 600)
        {

        }
    }

    [Serializable]
    public class GorevKisitKod
    {
        public int kod;
        public string ad;
        public string erisimKod;

        public GorevKisitKod(int kod, string ad, string erisimKod)
        {
            this.kod = kod;
            this.ad = ad;
            this.erisimKod = erisimKod;
        }
    }

    [Serializable]
    public class GorevKisitDeger
    {
        public GorevKisitKod gorevKisit { get { if (gorevKisitKod == null) gorevKisitKod = new GorevKisitKod(-1, "", ""); return gorevKisitKod; } }
        public GorevKisitKod gorevKisitKod;
        public string deger { get; set; }
        public int kod { get { return gorevKisit.kod; } set { gorevKisit.kod = value; } }
        public string ad { get { return gorevKisit.ad; } set { gorevKisit.ad = value; } }

        public GorevKisitDeger()
        {
            gorevKisitKod = null;
        }
    }

    public static class SabitGorevlerArac
    {
        public static void DebugYaz(string veri)
        {
            OrtakFonksiyonlar.HataDosyaYaz("Yetki.txt", veri, false);
        }

        public static GorevKisitKod[] GorevKisitAlanlar = { new GorevKisitKod(1, "Ekonomik Kod", ""), new GorevKisitKod(2, "Faaliyet Kod", ""), new GorevKisitKod(3, "Ambar Kod", "") };

        //Bu dizinin sırası değişmeyecek, bu sıra veri tabanında tutuluyor. Çıkartılan yerine null konulacak. Yeni eklenenler sona eklenecek.
        public static BirimGorev[] birimGorevler = {
            new BG_OdenekGonderir(), //0
            new BG_ButceHazirlik(), //1
            new BG_BütceIslemYapar(), //2
            new BG_YatirimProjesi(), //3
            new BG_KesinHesap(), //4
            new BG_StratejikPlan(), //5
            new BG_PerformansButce(), //6
            new BG_EvrakIslemleri(), //7
            new BG_OdemeEmri(), //8
            new BG_AlacakTakip(), //9
            new BG_PersonelIslemleri(), //10
            new BG_KadroIslemleri(), //11
            new BG_KullanicilariYonetir(), //12
            new BG_TasinirIslemleri(), //13
            new BG_SistemYoneticisi(), //14
            new BG_KutuphaneIslemleri(), //15
            new BG_KimlikPaylasimi(), //16
            new BG_GorevYonetimSistemi(), //17
            new BG_SatinAlmaIslemiYapar(), //18
            new BG_TeminatMektubu(), //19
            new BG_HarcamaIslemleri(), //20
            new BG_TaahhutKarti(), //21
            new BG_KurumProjeleri(), //22
            new BG_DunyaBankasiProjeleri(), //23
            new BG_SayistayBulguYonetimi() //24
        };

        public static string YazilimYetkiAd(int yazYetkiSiraNo)
        {
            return birimGorevler[yazYetkiSiraNo].GorevAd;
        }

        public static int GorevKisitAlanlarYerBul(int kisitKod)
        {
            for (int i = 0; i < GorevKisitAlanlar.Length; i++)
            {
                if (GorevKisitAlanlar[i].kod == kisitKod) return i;
            }
            return -1;
        }
    }
}

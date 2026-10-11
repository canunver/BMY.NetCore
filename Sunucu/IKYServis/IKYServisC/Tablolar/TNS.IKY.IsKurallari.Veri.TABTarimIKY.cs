using System;
using System.Collections.Generic;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABTarimIKY : TABOrtak
    {
        public TABTarimIKY(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADPER = "PersonelBilgi_SGB";
        public static string TABLOADPEROGR = "vPersonelOgrenim";
        public static string TABLOADPERHAR = "vPersonelHareket";

        public string POTcKimlikNo;
        public string POOgrenimDurumu;
        public int POOkulId;
        public string POOkulAdi;
        public int POFakulteId;
        public string POFakulteAdi;
        public int POBolumId;
        public string POBolumAdi;
        public string POOgrenimAdi;
        public int POSure;
        public TNSDateTime POMezuniyetTarihi = new TNSDateTime();
        public TNSDateTime PODiplomaTarihi = new TNSDateTime();
        public string PODiplomaBelgeNo;


        public int POOgrenimKod;
        public int POBolumKod;
        public int POFakulteKod;
        public int POUniversiteKod;

        public string PHTcKimlikNo;
        public int PHBirimId;
        public string PHBirimAdi;
        public int PHUnvanId;
        public string PHUnvanAdi;
        public int PHKazanilanDerece;
        public int PHKazanilanKademe;
        public int PHKazanilanGosterge;
        public int PHKazanilanEkGosterge;
        public int PHEmeklilikDerece;
        public int PHEmeklilikKademe;
        public int PHEmeklilikGosterge;
        public int PHEmeklilikEkGosterge;
        public int PHOdenecekDerece;
        public int PHOdenecekKademe;
        public int PHOdenecekGosterge;
        public int PHOdenecekEkGosterge;
        public TNSDateTime PHBaslamaTarihi = new TNSDateTime();
        public TNSDateTime PHAyrilmaTarihi = new TNSDateTime();
        public TNSDateTime PHHareketTarihi = new TNSDateTime();
        public string PHHareketTipAdi;
        public string PHKadroTip;
        public int PHDetsisNo;
        public int PHHizmetSebebiKod;
        public string PHBirimBirimKodu;
        public int PHBirimMuhasebeKodu;
        public string PHBirimKurKod;
        public int PHBirimOrgKod;
        public int PHPersonelTipi;


        public string PBTcKimlikNo;
        public string PBAdi;
        public string PBSoyadi;
        public string PBAnneAdi;
        public string PBBabaAdi;

        public string PBDogumYeri;
        public TNSDateTime PBDogumTarihi = new TNSDateTime();
        public int PBCinsiyet;
        public int PBMedeniHal;
        public string PBNufusIl;
        public string PBNufusIlce;

        public string PBNufusMahalleKoy;
        public string PBAskerlikDurumu;
        public string PBEvAdresi;
        public string PBCepTelefonu;
        public string PBIsTelefonu;
        public string PBEPosta;
        public string PBKurumSicilNo;
        public string PBEmekliSicilNo;
        public string PBKadroUnvanAdi;
        public int PBKadroUnvanId;
        public string PBKadroBirimAdi;
        public int PBKadroBirimId;
        public int PBKadroBirimDetsisNo;
        public string PBKadroAltBirimAdi;
        public int PBKadroAltBirimId;
        public int PBKadroAltBirimDetsisNo;
        public string PBGeciciGorevUnvanAdi;
        public int PBGeciciGorevUnvanId;
        public string PBGeciciGorevBirimAdi;
        public int PBGeciciGorevBirimId;
        public int PBGeciciGorevBirimDetsisNo;
        public TNSDateTime PBSonIslemTarihi = new TNSDateTime();
        public string PBSonIslemAciklama;
        public int PBSonIslemId;
        public int PBOzlukDurumId;
        public string PBOzlukDurumAdi;
        public string PBSinif;
        public string PBKadroTipi;
        public int PBPersonelTipi;
        public TNSDateTime PBIlkMemuriyeteBaslamaTarihi = new TNSDateTime();
        public int PBSonPersonelHareketKadroDerece;
        public int PBSonPersonelHareketKadroKademe;
        public int PBSonPersonelHareketKadroGosterge;
        public int PBSonPersonelHareketKadroEkGosterge;
        public int PBSonPersonelHareketEmeklilikDerece;
        public int PBSonPersonelHareketEmeklilikKademe;
        public int PBSonPersonelHareketEmeklilikGosterge;
        public int PBSonPersonelHareketEmeklilikEkGosterge;
        public int PBSonPersonelHareketOdenecekDerece;
        public int PBSonPersonelHareketOdenecekKademe;
        public int PBSonPersonelHareketOdenecekGosterge;
        public int PBSonPersonelHareketOdenecekEkGosterge;

        public string OGRKayitBul()
        {
            if (OGRKayitKod.StartsWith("08"))
                return OGRKayitAd;
            else
                return OGRKayitKod;
        }

        //public decimal GBBIRIM_GRUP_BIRIM_ID;
        //public decimal GBBIRIM_ID;
        //public decimal GBBIRIM_GRUP_ID;
        //public decimal GBSILINDI;
        //public decimal YBBIRIM_ID;
        //public decimal YBUST_BIRIM_ID;
        //public string YBBIRIM_ADI;
        //public string YBANA_KOD;
        //public string YBDESIMAL_KODU;
        //public string YBTELEFON_NO;
        //public string YBTELEFON_NO2;
        //public string YBDAGITIM_ADI;
        //public decimal YBSIRA_NO;
        //public decimal YBAKTIF;
        //public string YBYOL;
        //public decimal YBSEHIR_ID;
        //public string YBADRES;
        //public string YBEPOSTA;
        //public string YBFAKS_NO;
        //public string YBWEB_ADRESI;
        //public string YBOLUR_ONAY_UNVAN;
        //public string YBEKOD;
        //public string YBPARENTEKOD;
        //public TNSDateTime YBSONGUNCELLEME = new TNSDateTime();
        //public decimal YBALTKURUMVARMI;
        //public decimal YBBIRIM_TURU;
        //public decimal YBBIRIM_GRUP_ID;
        //public decimal YBBELGENET_ID;
        //public decimal YBPOSTA_BIRIM_ID;
        //public string YBANTET_BILGISI;
        //public decimal YBBELGENET_KULLANIM;
        //public decimal YBBELGENET_GORUNURLUK_TIPI;
        //public int YBDurumId;
        //public string YBAdi;
        //public int YBTeskilatId;

        public string DTBirimKodu;
        public int DTMuhasebeKodu;
        public string DTKurKod;
        public int DTOrgKod;

        public string PBKadroBirimBirimKodu;
        public int PBKadroBirimMuhasebeKodu;
        public string PBKadroBirimKurKod;
        public int PBKadroBirimOrgKod;

        public string PBGeciciGorevBirimKodu;
        public int PBGeciciGorevMuhasebeKodu;
        public string PBGeciciGorevKurKod;
        public int PBGeciciGorevOrgKod;



        IDataReader aktifDr = null;
        static string[] ALANLARPER = { "TcKimlikNo", "Adi", "Soyadi", "AnneAdi", "BabaAdi", "DogumYeri", "DogumTarihi",
            "Cinsiyet", "MedeniHal", "NufusIl", "NufusIlce", "NufusMahalleKoy", "AskerlikDurumu", "EvAdresi", "CepTelefonu", "IsTelefonu", "EPosta",
            "KurumSicilNo", "EmekliSicilNo", "KadroUnvanAdi", "KadroUnvanId", "KadroBirimAdi", "KadroBirimId", "KadroBirimDetsisNo", "KadroAltBirimAdi",
            "KadroAltBirimId", "KadroAltBirimDetsisNo", "GeciciGorevUnvanAdi", "GeciciGorevUnvanId", "GeciciGorevBirimAdi", "GeciciGorevBirimId",
            "GeciciGorevBirimDetsisNo", "SonIslemTarihi", "SonIslemAciklama", "SonIslemId", "OzlukDurumId", "OzlukDurumAdi", "Sinif", "KadroTipi",
            "IlkMemuriyeteBaslamaTarihi", "SonPersonelHareketKadroDerece", "SonPersonelHareketKadroKademe", "SonPersonelHareketKadroGosterge", "SonPersonelHareketKadroEkGosterge",
            "SonPersonelHareketEmeklilikDerece", "SonPersonelHareketEmeklilikKademe", "SonPersonelHareketEmeklilikGosterge", "SonPersonelHareketEmeklilikEkGosterge",
            "SonPersonelHareketOdenecekDerece", "SonPersonelHareketOdenecekKademe", "SonPersonelHareketOdenecekGosterge", "SonPersonelHareketOdenecekEkGosterge" };

        static string[] ALANLARPEROGR = { "TcKimlikNo", "OgrenimDurumu", "OkulId",
                                       "OkulAdi", "FakulteId", "FakulteAdi", "BolumId", "BolumAdi",
                                       "OgrenimAdi", "Sure", "MezuniyetTarihi", "DiplomaTarihi", "DiplomaBelgeNo"};

        static string[] ALANLARPERHAR = {"TcKimlikNo", "BirimId", "BirimAdi", "UnvanId", "UnvanAdi", "KazanilanDerece", "KazanilanKademe", "KazanilanGosterge",
                                         "KazanilanEkGosterge", "EmeklilikDerece", "EmeklilikKademe", "EmeklilikGosterge", "EmeklilikEkGosterge", "OdenecekDerece",
                                         "OdenecekKademe", "OdenecekGosterge", "OdenecekEkGosterge", "BaslamaTarihi", "AyrilmaTarihi",
                                         "HareketTarihi", "HareketTipAdi", "KadroTip", "DetsisNo" };

        public int OGRKod;
        public string OGRKayitKod;
        public string OGRKayitAd;

        public bool PerOku(bool drKapat, Dictionary<string, string[]> ilListesi, Dictionary<int, object[]> detsisListesi)
        {
            if (aktifDr == null) return false;
            if (aktifDr.Read())
            {
                int cl = 0;
                PBTcKimlikNo = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBSoyadi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBAnneAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBBabaAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBDogumYeri = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBDogumTarihi.Yaz(VeriErisim.DegerAl(aktifDr, cl++));

                /*Cinsiyet
                    Erkek = 2
                    Kadın = 3
                    = 0*/
                string cinsStr = VeriErisim.DegerAl(aktifDr, cl++, "");
                if (cinsStr == "Erkek") PBCinsiyet = 2;
                else if (cinsStr == "Kadın") PBCinsiyet = 3;
                else PBCinsiyet = 0;

                /*MedeniHal
                Bekar = 14
                Evli = 15
                Boşanmış = 765
                Dul = 766
                Evliliğin İptali = 767
                = 0  */

                string medeniHalStr = VeriErisim.DegerAl(aktifDr, cl++, "");
                if (medeniHalStr == "Bekar") PBMedeniHal = 14;
                else if (medeniHalStr == "Evli") PBMedeniHal = 15;
                else if (medeniHalStr == "Boşanmış") PBMedeniHal = 765;
                else if (medeniHalStr == "Dul") PBMedeniHal = 766;
                else if (medeniHalStr == "Evliliğin İptali") PBMedeniHal = 767;
                else PBMedeniHal = 0;

                string ilAd = VeriErisim.DegerAl(aktifDr, cl++, ""); ;
                string ilceAd = VeriErisim.DegerAl(aktifDr, cl++, "");

                string[] ilIlce = null;
                ilListesi.TryGetValue(ilAd + " " + ilceAd, out ilIlce);
                if (ilIlce != null)
                {
                    PBNufusIl = ilIlce[0];
                    PBNufusIlce = ilIlce[1];
                }
                else
                {
                    OrtakFonksiyonlar.HataStrYaz($"{ilAd}-{ilceAd} coğrafya listesinde bulunamadı");
                    PBNufusIl = "";
                    PBNufusIlce = "";
                }
                PBNufusMahalleKoy = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBAskerlikDurumu = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBEvAdresi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBCepTelefonu = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBIsTelefonu = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBEPosta = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBKurumSicilNo = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBEmekliSicilNo = VeriErisim.DegerAl(aktifDr, cl++, "");

                PBKadroUnvanAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBKadroUnvanId = VeriErisim.DegerAl(aktifDr, cl++, (int)0);

                PBKadroBirimAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBKadroBirimId = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBKadroBirimDetsisNo = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                BirimEslestir(detsisListesi, PBKadroBirimDetsisNo, out PBKadroBirimBirimKodu, out PBKadroBirimMuhasebeKodu, out PBKadroBirimKurKod, out PBKadroBirimOrgKod);

                PBKadroAltBirimAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBKadroAltBirimId = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBKadroAltBirimDetsisNo = VeriErisim.DegerAl(aktifDr, cl++, (int)0);

                PBGeciciGorevUnvanAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBGeciciGorevUnvanId = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBGeciciGorevBirimAdi = VeriErisim.DegerAl(aktifDr, cl++, "");

                PBGeciciGorevBirimId = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBGeciciGorevBirimDetsisNo = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                BirimEslestir(detsisListesi, PBGeciciGorevBirimDetsisNo, out PBGeciciGorevBirimKodu, out PBGeciciGorevMuhasebeKodu, out PBGeciciGorevKurKod, out PBGeciciGorevOrgKod);

                PBSonIslemTarihi.Yaz(VeriErisim.DegerAl(aktifDr, cl++));
                PBSonIslemAciklama = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBSonIslemId = VeriErisim.DegerAl(aktifDr, cl++, (int)0);

                /*Gayri Faal
                Faal*/
                PBOzlukDurumId = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBOzlukDurumAdi = VeriErisim.DegerAl(aktifDr, cl++, "");

                /*
                    Genel İdare Hizmetleri
                    Yardımcı Hizmetler
                    Avukatlık Hizmetleri
                    4/B
                    Teknik Hizmetler
                    Sağlık Hizmetleri
                 * */
                PBSinif = VeriErisim.DegerAl(aktifDr, cl++, "");

                /*
                 * 
                 Tarım:
                    İşçi
                    4/B Sözleşmeli
                    Memur 

                SGB:
                    MEMUR = 1,
                    Sozlesmeli = 2,
                    Sozlesmeli_4C = 3,
                    ISCI = 4,
                    Gecici_Isci = 5,
                 * */

                PBKadroTipi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PBPersonelTipi = PersonelTipiBul(PBKadroTipi, PBKadroUnvanAdi);

                PBIlkMemuriyeteBaslamaTarihi.Yaz(VeriErisim.DegerAl(aktifDr, cl++));

                PBSonPersonelHareketKadroDerece = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketKadroKademe = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketKadroGosterge = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketKadroEkGosterge = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketEmeklilikDerece = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketEmeklilikKademe = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketEmeklilikGosterge = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketEmeklilikEkGosterge = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketOdenecekDerece = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketOdenecekKademe = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketOdenecekGosterge = VeriErisim.DegerAl(aktifDr, cl++, (int)0);
                PBSonPersonelHareketOdenecekEkGosterge = VeriErisim.DegerAl(aktifDr, cl++, (int)0);

                if (drKapat)
                {
                    aktifDr.Close();
                    aktifDr = null;
                }
                return true;
            }
            else
            {
                aktifDr.Close();
                aktifDr = null;
                return false;
            }
        }

        private static int PersonelTipiBul(string kadroTipi, string kadroUnvanAdi)
        {
            int PBPersonelTipi;
            if (kadroTipi == "Memur")
                PBPersonelTipi = 1;
            else if (kadroTipi == "4/B Sözleşmeli")
                PBPersonelTipi = 2;
            else if (kadroTipi == "İşçi")
            {
                if (kadroUnvanAdi == "Geçici İşçi (Ş)" || kadroUnvanAdi == "Geçici İşçi (696)" || kadroUnvanAdi == "Geçici İşçi")
                    PBPersonelTipi = 5;
                else
                    PBPersonelTipi = 4;
            }
            else
                PBPersonelTipi = 100;
            return PBPersonelTipi;
        }

        public bool ListelePer(string tcKimlikNo, bool kendisi)
        {
            string sql = $"SELECT {VeriErisim.TumAlanlar("", ALANLARPER)} FROM {TABLOADPER}";
            if (!string.IsNullOrWhiteSpace(tcKimlikNo) && kendisi)
                sql += $" WHERE TcKimlikNo = {VeriErisim.DegerYap(tcKimlikNo)}";
            else
                sql += $" WHERE TcKimlikNo = (SELECT MIN(TcKimlikNo) FROM {TABLOADPER} WHERE SonIslemTarihi > '2019-01-01' and SonIslemTarihi<'3300-01-01' and TcKimlikNo>{VeriErisim.DegerYap(tcKimlikNo)})";

            aktifDr = this.veriErisim.DataReaderDondur(sql);
            return aktifDr != null;
        }

        public bool PerOgrenimOku(Dictionary<string, int> OgrenimListesi)
        {
            if (aktifDr == null) return false;
            if (aktifDr.Read())
            {
                int cl = 0;

                POTcKimlikNo = VeriErisim.DegerAl(aktifDr, cl++, "");
                POOgrenimDurumu = VeriErisim.DegerAl(aktifDr, cl++, "");
                POOkulId = VeriErisim.DegerAl(aktifDr, cl++, 0);
                POOkulAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                POFakulteId = VeriErisim.DegerAl(aktifDr, cl++, 0);
                POFakulteAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                POBolumId = VeriErisim.DegerAl(aktifDr, cl++, 0);
                POBolumAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                POOgrenimAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                POSure = VeriErisim.DegerAl(aktifDr, cl++, 0);
                POMezuniyetTarihi.Yaz(VeriErisim.DegerAl(aktifDr, cl++));
                PODiplomaTarihi.Yaz(VeriErisim.DegerAl(aktifDr, cl++));
                PODiplomaBelgeNo = VeriErisim.DegerAl(aktifDr, cl++, "");

                POOgrenimKod = 0;
                OgrenimListesi.TryGetValue(POOgrenimDurumu, out POOgrenimKod);
                POBolumKod = OgrenimKodBul(OgrenimListesi, "11", POBolumId);
                POFakulteKod = OgrenimKodBul(OgrenimListesi, "10", POFakulteId);
                POUniversiteKod = OgrenimKodBul(OgrenimListesi, "09", POOkulId);

                return true;
            }
            else
            {
                aktifDr.Close();
                aktifDr = null;
                return false;
            }
        }

        private int OgrenimKodBul(Dictionary<string, int> OgrenimListesi, string sozlukAnaKod, int okunanId)
        {
            int kod = 0;
            string kayitKod = sozlukAnaKod + "." + okunanId.ToString("00000");
            OgrenimListesi.TryGetValue(kayitKod, out kod);
            return kod;
        }

        public bool ListelePerOgrenim(string tcKimlikNo)
        {
            string sql = $"SELECT {VeriErisim.TumAlanlar("", ALANLARPEROGR)} FROM {TABLOADPEROGR} WHERE TcKimlikNo = {VeriErisim.DegerYap(tcKimlikNo)}";

            aktifDr = this.veriErisim.DataReaderDondur(sql);
            return aktifDr != null;
        }

        public bool PerHareketOku(Dictionary<int, object[]> detsisListesi, Dictionary<string, int> hizmetSebebiListesi)
        {
            if (aktifDr == null) return false;
            if (aktifDr.Read())
            {
                int cl = 0;

                PHTcKimlikNo = VeriErisim.DegerAl(aktifDr, cl++, "");
                PHBirimId = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHBirimAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PHUnvanId = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHUnvanAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PHKazanilanDerece = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHKazanilanKademe = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHKazanilanGosterge = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHKazanilanEkGosterge = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHEmeklilikDerece = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHEmeklilikKademe = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHEmeklilikGosterge = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHEmeklilikEkGosterge = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHOdenecekDerece = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHOdenecekKademe = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHOdenecekGosterge = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHOdenecekEkGosterge = VeriErisim.DegerAl(aktifDr, cl++, 0);
                PHBaslamaTarihi.Yaz(VeriErisim.DegerAl(aktifDr, cl++));
                PHAyrilmaTarihi.Yaz(VeriErisim.DegerAl(aktifDr, cl++));
                PHHareketTarihi.Yaz(VeriErisim.DegerAl(aktifDr, cl++));
                PHHareketTipAdi = VeriErisim.DegerAl(aktifDr, cl++, "");
                PHKadroTip = VeriErisim.DegerAl(aktifDr, cl++, "");
                PHDetsisNo = VeriErisim.DegerAl(aktifDr, cl++, 0);

                BirimEslestir(detsisListesi, PHDetsisNo, out PHBirimBirimKodu, out PHBirimMuhasebeKodu, out PHBirimKurKod, out PHBirimOrgKod);
                PHPersonelTipi = PersonelTipiBul(PHKadroTip, PHUnvanAdi);
                PHHizmetSebebiKod = 0;
                hizmetSebebiListesi.TryGetValue(PHHareketTipAdi, out PHHizmetSebebiKod);
                return true;
            }
            else
            {
                aktifDr.Close();
                aktifDr = null;
                return false;
            }
        }

        private static void BirimEslestir(Dictionary<int, object[]> detsisListesi, int detsisNo, out string birimKodu, out int muhasebeKodu, out string birimKurKod, out int birimOrgKod)
        {
            object[] o;
            if (detsisNo > 0)
            {
                detsisListesi.TryGetValue(detsisNo, out o);
                if (o == null)
                    OrtakFonksiyonlar.HataStrYaz($"{detsisNo} detsis listesinde bulunamadı");
            }
            else
                o = null;
            if (o != null)
            {
                birimKodu = (string)o[0];
                muhasebeKodu = (int)o[1];
                birimKurKod = (string)o[2];
                birimOrgKod = (int)o[3];
            }
            else
            {
                birimKodu = "";
                muhasebeKodu = 0;
                birimKurKod = "";
                birimOrgKod = 0;
            }

        }

        public bool ListelePerHareket(string tcKimlikNo)
        {
            string sql = $"SELECT {VeriErisim.TumAlanlar("", ALANLARPERHAR)} FROM {TABLOADPERHAR} WHERE TcKimlikNo = {VeriErisim.DegerYap(tcKimlikNo)} ORDER BY AyrilmaTarihi,  BaslamaTarihi";

            aktifDr = this.veriErisim.DataReaderDondur(sql);
            return aktifDr != null;
        }

        public bool DetsisOku()
        {
            if (aktifDr == null) return false;
            if (aktifDr.Read())
            {
                int cl = 0;
                PBKadroBirimDetsisNo = VeriErisim.DegerAl(aktifDr, cl++, 0);
                DTBirimKodu = VeriErisim.DegerAl(aktifDr, cl++, "");
                DTMuhasebeKodu = VeriErisim.DegerAl(aktifDr, cl++, 0);
                DTKurKod = VeriErisim.DegerAl(aktifDr, cl++, "");
                DTOrgKod = VeriErisim.DegerAl(aktifDr, cl++, 0);

                return true;
            }
            else
            {
                aktifDr.Close();
                aktifDr = null;
                return false;
            }
        }

        public bool DetsisListele()
        {
            string sql = "select DETSISNO, d.BIRIMKODU, d.MUHASEBEKODU, d.KURKOD, b.KOD from DETSISCEVRIM d INNER JOIN Kullanici.Dbo.ORGBIRIM b on d.BIRIMKODU=b.MUHASEBEBIRIM and d.MUHASEBEKODU=b.MUHASEBEKODU and  d.KURKOD=b.KURUMSALKOD";

            aktifDr = this.veriErisim.DataReaderDondur(sql);
            return aktifDr != null;
        }

        public bool OgrenimOku()
        {
            if (aktifDr == null) return false;
            if (aktifDr.Read())
            {
                int cl = 0;
                OGRKod = VeriErisim.DegerAl(aktifDr, cl++, 0);
                OGRKayitKod = VeriErisim.DegerAl(aktifDr, cl++, "");
                OGRKayitAd = VeriErisim.DegerAl(aktifDr, cl++, "");

                return true;
            }
            else
            {
                aktifDr.Close();
                aktifDr = null;
                return false;
            }
        }

        public bool OgrenimListele()
        {
            string sql = "select KOD, KAYITKOD, KAYITAD from  IKYSOZLUK where LEN(KAYITKOD)>2 AND KAYITKOD > '08' AND KAYITKOD < '12'";

            aktifDr = this.veriErisim.DataReaderDondur(sql);
            return aktifDr != null;
        }
    }
}

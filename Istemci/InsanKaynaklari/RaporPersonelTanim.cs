using System.Collections.Generic;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// Personel Raporları (eski InsanKaynaklari/RaporPersonel.aspx) rapor tanımları: raporların kodu, adı, şablonu, hangi rapor
    /// sınıfıyla (PersonelRaporlari / PersonelRaporlari2) üretildiği ve ekranda hangi kriter alanlarının açılacağı.
    /// Eski sayfadaki raporGrubu / raporTanimlar dizileri aynen alındı. Agac: eski sayfadaki rapor ağacının (treListe) düğümleri.
    /// </summary>
    public static class RaporPersonelTanim
    {
        public enum ENUMRaporTur
        {
            //Crystal,
            Metot,
        }

        public class RaporGrubu
        {
            public RaporGrubu() { }
            public string KOD { get; set; }
            public string AD { get; set; }
            public string ATAKOD { get; set; }
            public int pZorunlu { get; set; }

            public RaporGrubu(string KOD, string AD, string ATAKOD, int pZorunlu)
            {
                this.KOD = KOD;
                this.AD = AD;
                this.ATAKOD = ATAKOD;
                this.pZorunlu = pZorunlu;
            }
        }

        public static RaporGrubu[] raporGrubu = {
             new RaporGrubu("GR002", "Birim Raporları", null,0),
             new RaporGrubu("GR003", "Kişisel Raporlar", null,1),
             new RaporGrubu("GR004", "Kadro Raporları", null,0),
             new RaporGrubu("GR005", "İzin Raporları", null,1),
             new RaporGrubu("GR006", "Hizmet Raporları", null,1),
             //new RaporGrubu("GR007", "Terfi Raporları", null),
             new RaporGrubu("GR008", "Unvan Raporları", null,0),
             //new RaporGrubu("GR009", "Dinamik Rapor Listesi", null),
             new RaporGrubu("GR010", "İstatistiki Raporlar", null,0),
             new RaporGrubu("GR011", "Kart Raporları", null,0),
             new RaporGrubu("GR012", "Lojman Raporları", null,0),
             new RaporGrubu("GR013", "Site Yönetim Raporları", null,0),
             new RaporGrubu("GR014", "Yemek Raporları", null,0),
        };

        public class RaporAlanTanim
        {
            public bool cmpBirim = true;
            public bool cmpKadroDerece = false;
            public bool cmpIslemSira = false;
            public bool cmpCiktiTur = false;
            public bool cmpPersonelTurCoklu = true;
            public bool cmpCiktiTurDoc = false;
            public bool cmpYukumlu = false;
            public bool cmpKisiArama = true;
            public bool cmpSonTarih = false;
            public bool cmpDinamik = false;
            public string siralamaListe = null;
            public bool cmpAktiflik = true;
            public bool cmpSiteKod = false;
            public bool cmpDaireBosluk = false;
            public bool cmpYil = false;
            public bool cmpAy = false;
            public bool cmpYilArasi = false;
            public bool cmpYemekhane = false;
            public bool cmpKisiAramaGrid = false;
            public bool tarihSorgu = false;
            public bool izinTur = false;
        }

        public class RaporTanim
        {
            public RaporTanim() { }
            public string KOD { get; set; }
            public string AD { get; set; }
            public string DOSYAAD { get; set; }
            public ENUMRaporTur raporTur { get; set; }
            public string raporGrubu { get; set; }
            public int raporDosyasiNo { get; set; }

            public bool cmpBirim { get; set; }
            public bool cmpKadroDerece { get; set; }
            public bool cmpIslemSira { get; set; }
            public bool cmpCiktiTur { get; set; }
            public bool cmpCiktiTurDoc { get; set; }
            public bool cmpPersonelTurCoklu { get; set; }
            public bool cmpYukumlu { get; set; }
            public bool cmpSonTarih { get; set; }
            public bool cmpKisiArama { get; set; }
            public bool cmpDinamik { get; set; }
            public bool cmpAktiflik { get; set; }
            public bool cmpSiteKod { get; set; }
            public bool cmpDaireBosluk { get; set; }
            public bool cmpYil { get; set; }
            public bool cmpAy { get; set; }
            public bool cmpYilArasi { get; set; }
            public bool cmpYemekhane { get; set; }
            public string siralamaListe { get; set; }
            public bool cmpKisiAramaGrid { get; set; }
            public bool tarihSorgu { get; set; }
            public bool izinTur { get; set; }

            public RaporTanim(string KOD, string AD, string DOSYAAD, ENUMRaporTur raporTur,
                              bool cmpBirim, bool cmpKadroDerece, bool cmpIslemSira, bool kullanma3,
                              bool cmpCiktiTur, bool cmpPersonelTurCoklu, bool cmpCiktiTurDoc,
                              bool cmpYukumlu, bool kullanma1, bool cmpKisiArama, bool kullanma2, bool cmpSonTarih,
                              int raporDosyasiNo, bool cmpDinamik, string raporGrubu, string siralamaListe, bool cmpAktiflik, bool cmpSiteKod,
                              bool cmpDaireBosluk, bool cmpYil, bool cmpAy, bool cmpYilArasi, bool cmpYemekhane, bool cmpKisiAramaGrid, bool tarihSorgu, bool izinTur)
            {
                this.KOD = KOD;
                this.AD = AD;
                this.DOSYAAD = DOSYAAD;
                this.raporTur = raporTur;
                this.raporGrubu = raporGrubu;
                this.raporDosyasiNo = raporDosyasiNo;

                this.cmpBirim = cmpBirim;
                this.cmpKadroDerece = cmpKadroDerece;
                this.cmpIslemSira = cmpIslemSira;
                this.cmpCiktiTur = cmpCiktiTur;
                this.cmpPersonelTurCoklu = cmpPersonelTurCoklu;
                this.cmpCiktiTurDoc = cmpCiktiTurDoc;
                this.cmpYukumlu = cmpYukumlu;
                this.cmpKisiArama = cmpKisiArama;
                this.cmpSonTarih = cmpSonTarih;
                this.cmpDinamik = cmpDinamik;
                this.siralamaListe = siralamaListe;
                this.cmpAktiflik = cmpAktiflik;
                this.cmpSiteKod = cmpSiteKod;
                this.cmpDaireBosluk = cmpDaireBosluk;
                this.cmpYil = cmpYil;
                this.cmpAy = cmpAy;
                this.cmpYilArasi = cmpYilArasi;
                this.cmpYemekhane = cmpYemekhane;
                this.cmpKisiAramaGrid = cmpKisiAramaGrid;
                this.tarihSorgu = tarihSorgu;
                this.izinTur = izinTur;
            }

            public RaporTanim(string KOD, string AD, string DOSYAAD, ENUMRaporTur raporTur, string raporGrubu, int raporDosyasiNo, RaporAlanTanim rat)
            {
                this.KOD = KOD;
                this.AD = AD;
                this.DOSYAAD = DOSYAAD;
                this.raporTur = raporTur;
                this.raporGrubu = raporGrubu;
                this.raporDosyasiNo = raporDosyasiNo;

                this.cmpBirim = rat.cmpBirim;
                this.cmpKadroDerece = rat.cmpKadroDerece;
                this.cmpIslemSira = rat.cmpIslemSira;
                this.cmpCiktiTur = rat.cmpCiktiTur;
                this.cmpPersonelTurCoklu = rat.cmpPersonelTurCoklu;
                this.cmpCiktiTurDoc = rat.cmpCiktiTurDoc;
                this.cmpYukumlu = rat.cmpYukumlu;
                this.cmpKisiArama = rat.cmpKisiArama;
                this.cmpSonTarih = rat.cmpSonTarih;
                this.cmpDinamik = rat.cmpDinamik;
                this.siralamaListe = rat.siralamaListe;
                this.cmpAktiflik = rat.cmpAktiflik;
                this.cmpSiteKod = rat.cmpSiteKod;
                this.cmpDaireBosluk = rat.cmpDaireBosluk;
                this.cmpYil = rat.cmpYil;
                this.cmpAy = rat.cmpAy;
                this.cmpYilArasi = rat.cmpYilArasi;
                this.cmpYemekhane = rat.cmpYemekhane;
                this.cmpKisiAramaGrid = rat.cmpKisiAramaGrid;
                this.tarihSorgu = rat.tarihSorgu;
                this.izinTur = rat.izinTur;
            }
        }
        public static string sira = "Öncelik Derecesine Göre,Unvana Göre,Birime Göre,İsme Göre";

        public static RaporTanim[] raporTanimlar = {
             new RaporTanim("IK009", "Dolu-Boş Kadro Listesi",            "PersonelKadroKontenjanMemur.xlt",       ENUMRaporTur.Metot,   true, false, false, true, true, false, false, false, false, false, false, false, 0, false, "GR004", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK012", "Organizasyon Şeması",               "OrganizasyonPersonel.xltx",             ENUMRaporTur.Metot,   false, false, false, false, true, true, false, false , false, false, false, false, 0, false, "GR002", null,false,false,false,false,false,false,false, false, false, false),

             new RaporTanim("IK013", "İsimli Kadro Listesi",              "IsimliKadroListesi.xltx",               ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, false, 0, false, "GR004", "Unvana Göre,Birime Göre,İsme Göre",false,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK014", "İsimsiz Kadro Listesi",             "IsimsizKadroListesi.xltx",              ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, false, 0, false, "GR004", null,false,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK015", "Kadro Derece Durum Listesi",        "KadroDereceDurumListesi.xltx",          ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, false, 0, false, "GR004", "Unvana Göre,Birime Göre,İsme Göre",false,false,false,false,false,false,false, true, false, false),

             new RaporTanim("IK016", "Aile Listesi",                      "AileListesi.xltx",                      ENUMRaporTur.Metot,   true, false, false, false, false, true, false, true , false, true, false, false, 0, false, "GR003", null,true,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK017", "Öğrenim Durum Listesi",             "OgrenimDurumListesi.xltx",              ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, false, 0, false, "GR003", null,true,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK021", "İzin/Görevlendirme (Herkes)",       "IzinListesi.xltx",                      ENUMRaporTur.Metot,   "GR005", 0, new RaporAlanTanim{izinTur=true, siralamaListe="Unvana Göre,Birime Göre,İsme Göre"}),
             new RaporTanim("IK021b","İzin/Görevlendirme (Kullanan)",     "IzinListesi2.xltx",                     ENUMRaporTur.Metot,   "GR005", 0, new RaporAlanTanim{izinTur=true, siralamaListe="Unvana Göre,Birime Göre,İsme Göre"}),
             new RaporTanim("IK021a","İdari Görev Bilgileri",             "IdariGorevListesi.xltx",                ENUMRaporTur.Metot,   "GR005", 0, new RaporAlanTanim{cmpSonTarih=true}),
             new RaporTanim("IK022", "Öğrenim Genel Durumu Dağılımı",     "IKYIstatistik.xltx",                    ENUMRaporTur.Metot,   true, false, false, true, true, true, false, false, false, false, false, false, 1, false, "GR010", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK023", "Cinsiyete Göre Dağılım",            "IKYIstatistik.xltx",                    ENUMRaporTur.Metot,   true, false, false, true, true, true, false, false, false, false, false, false, 1, false, "GR010", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK024", "Personel Türlerine Göre Dağılım",   "IKYIstatistik.xltx",                    ENUMRaporTur.Metot,   true, false, false, true, true, true, false, false, false, false, false, false, 1, false, "GR010", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK025", "Yaş Genel Durum Dağılımı",          "IKYIstatistik.xltx",                    ENUMRaporTur.Metot,   true, false, false, true, true, true, false, false, false, false, false, false, 1, false, "GR010", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK026", "Dil Durumuna Göre Dağılım",         "IKYIstatistik.xltx",                    ENUMRaporTur.Metot,   true, false, false, true, true, true, false, false, false, false, false, false, 1, false, "GR010", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK027", "Kıdem Yılına Göre Personel Dağılımı", "IKYIstatistik.xltx",                  ENUMRaporTur.Metot,   true, false, false, true, true, true, false, false, false, false, false, false, 1, false, "GR010", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK028", "Personel Kart Listesi",             "KartListesi.xltx",                      ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, false, 0, false, "GR011", null,false,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK029", "Teslim Edilmeyen Kart Listesi",     "KartListesi.xltx",                      ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, false, 0, false, "GR011", null,false,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK030", "Kayıp Kart Listesi",                "KartListesi.xltx",                      ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, false, 0, false, "GR011", null,false,false,false,false,false,false,false, true, false, false),

             new RaporTanim("IK031", "Kurum Hizmeti Listesi",             "HizmetBelgesiListesi.xltx",             ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, true, true, 0, false, "GR006",  "Unvana Göre,Birime Göre,İsme Göre,Süreye Göre",true,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK032", "Kıdem Hizmeti Listesi",             "HizmetBelgesiListesi.xltx",             ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, true, 0, false, "GR006", "Unvana Göre,Birime Göre,İsme Göre,Süreye Göre",true,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK033", "Emekli Keseneği Hizmet Belgesi Listesi", "HizmetBelgesiListesi.xltx",        ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, true, 0, false, "GR006", "Unvana Göre,Birime Göre,İsme Göre,Süreye Göre",true,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK034", "İzin Hizmet Belgesi Listesi",       "HizmetBelgesiListesi.xltx",             ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, true, 0, false, "GR006", "Unvana Göre,Birime Göre,İsme Göre,Süreye Göre",true,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK035", "Toplam Hizmet Belgesi Listesi",     "HizmetBelgesiListesi.xltx",             ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, true, 0, false, "GR006", "Unvana Göre,Birime Göre,İsme Göre,Süreye Göre",true,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK036", "En Son Hizmet Belgesi Listesi",     "KadroDereceDurumListesi.xltx",          ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, false, 0, false,"GR006", "Unvana Göre,Birime Göre,İsme Göre,Süreye Göre",false,false,false,false,false,false,false,true, false, false),
             new RaporTanim("IK037", "Evlilik Durumuna Göre Dağılım",     "IKYIstatistik.xltx",                    ENUMRaporTur.Metot,   true, false, false, true, true, true, false, false, false, false, false, false, 1, false, "GR010", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK038", "Çocuk Sayısına Göre Dağılım",       "IKYIstatistik.xltx",                    ENUMRaporTur.Metot,   true, false, false, true, true, true, false, false, false, false, false, false, 1, false, "GR010", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK039", "Kurumda Çalıştığı Yıla Göre Personel Dağılımı", "IKYIstatistik.xltx",        ENUMRaporTur.Metot,   true, false, false, true, true, true, false, false, false, false, false, false, 1, false, "GR010", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK040", "Emekliliğe Sayılan Yıla Göre Personel Dağılımı", "IKYIstatistik.xltx",       ENUMRaporTur.Metot,   true, false, false, true, true, true, false, false, false, false, false, false, 1, false, "GR010", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK041", "Sırtlık Listesi",                   "Sirtlik.xltx",                          ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, false, 1, false, "GR003", null,false,false,false,false,false,false,false, true, true, false),
             new RaporTanim("IK042", "Unvan İsim Listesi",                "UnvanIsimListesi.xltx",                 ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, false, 0, false, "GR008", null,false,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK043", "Unvan Kadro Dağılımları",           "PersonelKadroKontenjanMemurT.xlt",      ENUMRaporTur.Metot,   true, false, false, true, true, false,false,false, false, false, false, false, 0, false, "GR008", null,false,false,false,false,false,false,false, false,false, false),
             new RaporTanim("IK044", "Unvan Derece Dağılımı",             "PersonelKadroKontenjanMemur.xlt",       ENUMRaporTur.Metot,   true, false, false, true, true, false,false,false, false, false, false, false, 0, false, "GR008", null,false,false,false,false,false,false,false, false, false, false),
             new RaporTanim("IK045", "Tanıtma Kartı",                     "PersonelTanitmaKarti.docx",             ENUMRaporTur.Metot,   true, false, false, false, false, true, true, false , false, true, false, false, 1, false, "GR003", null,true,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK046", "Kurum Kadrolu Hizmeti Listesi",     "HizmetBelgesiListesi.xltx",             ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true, false, true, 0, false, "GR006", "Unvana Göre,Birime Göre,İsme Göre,Süreye Göre",true,false,false,false,false,false,false, true, false, false),
             new RaporTanim("IK047", "Lojmanda Oturan Personel Listesi",  "LojmanListesi.xltx",                    ENUMRaporTur.Metot, false, false, false, false, false, false, false, false, false, false, false, false, 0, false, "GR011", null,false,true,false,false,false,false,false, false, false, false),
             new RaporTanim("IK048", "Lojman Dolu-Boş Daire Listesi",  "LojmanDaireListesi.xltx",                  ENUMRaporTur.Metot, false, false, false, false, false, false, false, false, false, false, false, false, 0, false, "GR011", null,false,true,true ,false,false,false,false, false, false, false),
             new RaporTanim("IK049", "Site Kesinti (Site)",  "PersonelSiteKesintiListesi.xltx",                    ENUMRaporTur.Metot, true, false, false, false, false, true, false, false, false, true, false, false, 0, false, "GR011", null,true,true,false, true,true,false,false, true, false, false),
             new RaporTanim("IK049a","Site Kesinti (Blok)", "PersonelSiteKesintiListesi.xltx",                     ENUMRaporTur.Metot, true, false, false, false, false, true, false, false, false, true, false, false, 0, false, "GR011", null,true,true,false, true,true,false,false, true, false, false),
             new RaporTanim("IK050", "Lojman Değişiklik Listesi",  "LojmanDegisiklikListesi.xltx",                 ENUMRaporTur.Metot, false, false, false, false, false, false, false, false, false, false, false, false, 0, false, "GR011", null,false,true,false,false,false,false,false, false, false, false),
             new RaporTanim("IK051", "Lojmanda Oturanlar Listesi (Süre Kısıtlı)",  "LojmanListesi.xltx",           ENUMRaporTur.Metot, false, false, false, false, false, false, false, false, false, false, false, false, 0, false, "GR011", null,false,true,false,false,false,true ,false, false, false, false),
             new RaporTanim("IK052", "Lojman Değişiklik Listesi Gruplu",  "LojmanDegisiklikListesiGrup.xltx",      ENUMRaporTur.Metot, false, false, false, false, false, false, false, false, false, false, false, false, 0, false, "GR011", null,false,true,false,false,false,false,false, false, false, false),
             new RaporTanim("IK053", "Lojmanda Rezervasyon Listesi",  "LojmanRezervasyonListesi.xltx",             ENUMRaporTur.Metot, false, false, false, false, false, false, false, false, false, false, false, false, 0, false, "GR011", sira,false,true,false,false,false,false,false, false, false, false),
             new RaporTanim("IK054", "Gider Listesi",                 "PersonelGiderListesi.xltx",                 ENUMRaporTur.Metot, "GR011", 2, new RaporAlanTanim{cmpAktiflik=false, cmpKisiArama=false, cmpPersonelTurCoklu=false, cmpYil=true, cmpAy=true } ),
             //new RaporTanim("IK055", "Personel Hesaplanmış Yemek Listesi", "PersonelHesaplanmisYemekListesi.xltx", ENUMRaporTur.Metot,   true, false, false, false, false, true, false, false, false, true,false,false, 0, false, "GR014", null,true,false,false,true,true,false,true, true, false, false),
             new RaporTanim("IK056", "Soruşturma Bilgisi",            "SorusturmaListesi.xltx",                    ENUMRaporTur.Metot, "GR003", 1, new RaporAlanTanim{cmpAktiflik=false, cmpKisiArama=false, cmpPersonelTurCoklu=true, cmpYil=false, cmpAy=false, tarihSorgu=true } ),

             new RaporTanim("IK101", "Unvan ve Personel Tipine Göre Dağılım", "",                                  ENUMRaporTur.Metot,   "GR010", 1, new RaporAlanTanim{}),
             new RaporTanim("IK102", "Hrc. Birimi ve Personel Tipine Göre Dağılım", "",                            ENUMRaporTur.Metot,   "GR010", 1, new RaporAlanTanim{}),

             new RaporTanim("GR009", "Dinamik Liste Seçimi Listesi",      "DinamikRapor.xltx",                     ENUMRaporTur.Metot, true, false, false, true, true, true,false,false, false, true, false, false, 0, true, "GR009", null,true,false,false,false,false,false,false, false, true, false),
        };

        public static RaporTanim RaporBul(string kod)
        {
            foreach (RaporTanim item in raporTanimlar)
            {
                if (item.KOD == kod) return item;
            }
            return null;
        }

        /// <summary>Lojman raporları (Core'da Lojman sekmesi yok). Göster false iken ağaçta görünmez ve alınamaz.</summary>
        public static readonly string[] LojmanRaporlari = { "IK047", "IK048", "IK049", "IK049a", "IK050", "IK051", "IK052", "IK053" };
        public static bool LojmanRaporlariGoster = false;

        public static bool Gizli(string kod)
        {
            return !LojmanRaporlariGoster && System.Array.IndexOf(LojmanRaporlari, kod) >= 0;
        }

        /// <summary>Ağaç düğümü: kod (rapor ya da grup kodu), metin, alt düğümler. Rapor koduyla eşleşen düğüm tıklanınca o rapor seçilir.</summary>
        public class Dugum
        {
            public string kod { get; set; }
            public string ad { get; set; }
            public List<Dugum> alt { get; set; }
            public Dugum(string kod, string ad, params Dugum[] alt)
            {
                this.kod = kod;
                this.ad = ad;
                this.alt = alt == null || alt.Length == 0 ? null : new List<Dugum>(alt);
            }
        }

        private static Dugum D(string kod, string ad, params Dugum[] alt) { return new Dugum(kod, ad, alt); }

        /// <summary>Eski treListe (gizli "Rapor Programı" ve "Terfi Raporları" grupları hariç).</summary>
        private static readonly Dugum[] TumAgac = {
            D("GR002", "Birim Raporları",
                D("IK012", "Organizasyon Şeması")),
            D("GR003", "Kişisel Raporlar",
                D("IK045", "Tanıtma Kartı"),
                D("IK016", "Aile Listesi"),
                D("IK017", "Öğrenim Durum Listesi"),
                D("IK041", "Sırtlık Listesi"),
                D("IK056", "Soruşturma Bilgisi")),
            D("GR004", "Kadro Raporları",
                D("IK009", "Dolu-Boş Kadro Listesi"),
                D("IK013", "İsimli Kadro Listesi"),
                D("IK014", "İsimsiz Kadro Listesi"),
                D("IK015", "Kadro Derece Durum Listesi")),
            D("GR005", "İzin/Görev Raporları",
                D("IK021", "İzin/Görevlendirme (Herkes)"),
                D("IK021b", "İzin/Görevlendirme (Kullanan)"),
                D("IK021a", "İdari Görev")),
            D("GR006", "Hizmet Raporları",
                D("IK031", "Kurum Hizmeti Listesi"),
                D("IK046", "Kadro Hizmeti Listesi"),
                D("IK032", "Kıdem Hizmeti Listesi"),
                D("IK033", "Emekli Keseneği Hizmet Belgesi Listesi"),
                D("IK034", "İzin Hizmet Belgesi Listesi"),
                D("IK035", "Toplam Hizmet Belgesi Listesi"),
                D("IK036", "En Son Hizmet Belgesi Listesi")),
            D("GR008", "Unvan Raporları",
                D("IK042", "Unvan İsim Listesi"),
                D("IK043", "Unvan Kadro Dağılımları"),
                D("IK044", "Unvan Derece Dağılımı")),
            D("GR009", "Dinamik Rapor Listesi"),
            D("GR010", "İstatistiki Raporlar",
                D("IK101", "Unvan ve Personel Grubuna Göre Dağılım"),
                D("IK102", "Hrc. Birimi ve Personel Tipine Göre Dağılım"),
                D("IK022", "Öğrenim Genel Durumu Dağılımı"),
                D("GR0101", "Personel Genel Durumu Dağılımı",
                    D("IK023", "Cinsiyete Göre Dağılım"),
                    D("IK037", "Evlilik Durumuna Göre Dağılım"),
                    D("IK038", "Çocuk Sayısına Göre Dağılım")),
                D("IK024", "Personel Türlerine Göre Dağılım"),
                D("IK025", "Yaş Genel Durum Dağılımı"),
                D("IK026", "Dil Durumuna Göre Dağılım"),
                D("GR0102", "Hizmet Genel Durumu Dağılımı",
                    D("IK027", "Kıdeme Göre"),
                    D("IK039", "Kurum Hizmetine Göre"),
                    D("IK040", "Emeklilik Hizmetine Göre"))),
            D("GR011", "Diğer Raporlar",
                D("IK054", "Gider Listesi"),
                D("IK028", "Personel Kart Listesi"),
                D("IK029", "Teslim Edilmeyen Kart Listesi"),
                D("IK030", "Kayıp Kart Listesi"),
                D("IK047", "Lojmanda Oturanlar Listesi"),
                D("IK051", "Lojmanda Oturanlar Listesi (Süreli)"),
                D("IK048", "Lojman Dolu Boş Daire Listesi"),
                D("IK050", "Lojman Değişiklik Listesi"),
                D("IK052", "Lojman Değişiklik Listesi Gruplu"),
                D("IK053", "Lojman Rezervasyon Listesi")),
        };

        /// <summary>Görünür ağaç (gizli raporlar çıkarılmış).</summary>
        public static List<Dugum> Agac()
        {
            List<Dugum> l = new List<Dugum>();
            foreach (Dugum d in TumAgac)
            {
                Dugum s = Suz(d);
                if (s != null) l.Add(s);
            }
            return l;
        }

        private static Dugum Suz(Dugum d)
        {
            if (Gizli(d.kod)) return null;
            if (d.alt == null) return new Dugum(d.kod, d.ad);
            List<Dugum> alt = new List<Dugum>();
            foreach (Dugum a in d.alt)
            {
                Dugum s = Suz(a);
                if (s != null) alt.Add(s);
            }
            return new Dugum(d.kod, d.ad, alt.ToArray());
        }
    }
}

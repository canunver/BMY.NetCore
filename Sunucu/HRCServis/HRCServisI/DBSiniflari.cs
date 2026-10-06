using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TNS;
using TNS.BKD;
using static TNS.HRC.DBHakedis;
using static TNS.HRC.KodAdYonetim;

namespace TNS.HRC
{
    public static class KodAdYonetim
    {

        public enum enumKesintiTuru : int
        {
            YukleniciyeOdenecekTutar = 1,
            GelirKurumlarVergisi = 2,
            DamgaVergisi = 3,
            UcretDamgaVergisi = 12,
            KDVTevkifati = 4,
            SGKKesintisi = 5,
            IdareMakinesiKiralari = 6,
            GecikmeCezasi = 7,
            FiyatFarkiTeminatKesintisi = 8,
            VergiBorcu = 9,
            Icra = 10,
            Nafaka = 11,
            Diger = 20
        }

        public static StrKodStrAd[] KesintiTuru = {
            new StrKodStrAd("1", "Yükleniciye Ödenecek Tutar"),
            new StrKodStrAd("2", "Gelir/Kurumlar Vergisi (%20)"),
            new StrKodStrAd("3", "Damga Vergisi (%0,948)"),
            new StrKodStrAd("12", "Ücret Damga Vergisi (%0,759)"),
            new StrKodStrAd("4", "KDV Tevkifatı"),
            new StrKodStrAd("5", "SGK Kesintisi"),
            new StrKodStrAd("6", "İdare Makinesi Kiraları"),
            new StrKodStrAd("7", "Gecikme Cezası"),
            new StrKodStrAd("8", "Fiyat Farkı Teminat Kesintisi"),
            new StrKodStrAd("9", "Vergi Borcu"),
            new StrKodStrAd("10", "İcra"),
            new StrKodStrAd("11", "Nafaka"),
            new StrKodStrAd("20", "Diğer")
        };

        public static StrKodStrAd[] HakedisTuru = {
            new StrKodStrAd("0", "Hakediş Türü Seçiniz"),
            new StrKodStrAd("2", "Avans Hak Edişi"),
            new StrKodStrAd("3", "1-F Yurt İçi Yolluk Ödemeleri"),
            new StrKodStrAd("4", "1-F Yurt Dışı Yolluk Ödemeleri"),
            new StrKodStrAd("5", "Sözleşme Hak Edişi–1A– Mal Alımları Hak Edişi"),
            new StrKodStrAd("6", "Sözleşme Hak Edişi–1B– Yapım İşleri Hak Edişi"),
            new StrKodStrAd("7", "Sözleşme Hak Edişi–1C– Danışmanlık Hizmetleri Hak Edişi"),
            new StrKodStrAd("8", "Sözleşme Hak Edişi–1D– Danışmanlık Dışı Hizmetlerin Hak Edişi"),
            new StrKodStrAd("9", "Sözleşme Hak Edişi–1E– Eğitim Hak Edişi"),
            new StrKodStrAd("10", "1F– Operasyonel Giderleri Hak Edişi"),
            new StrKodStrAd("11", "2–Kredi Ödemeleri"),
            new StrKodStrAd("12", "3–Hibe Ödemeleri"),
            new StrKodStrAd("1", "......"),
        };

        public static StrKodStrAd[] HakedisTurEkliDosya = {
            new StrKodStrAd("2", "DBHK"),
            new StrKodStrAd("3", "DBHG"),
            new StrKodStrAd("4", "DBHH"),
            new StrKodStrAd("5", "DBHA"),
            new StrKodStrAd("6", "DBHB"),
            new StrKodStrAd("7", "DBHC"),
            new StrKodStrAd("8", "DBHD"),
            new StrKodStrAd("9", "DBHE"),
            new StrKodStrAd("10", "DBHF"),
            new StrKodStrAd("11", "DBHI"),
            new StrKodStrAd("12", "DBHJ"),
            new StrKodStrAd("13", "HKS"),
            new StrKodStrAd("1", "DBH"),
        };

        public static StrKodStrAd[] ModulKodlar = {
            new StrKodStrAd("DBİ", "İkraz Anlaşması"),
            new StrKodStrAd("DBB", "Bütçe Referansları"),
            new StrKodStrAd("DBS", "Sözleşmeler") ,

            new StrKodStrAd("DBHA", "Sözleşme Hak Edişi–1A – Mal Alımları Hak Edişi"),
            new StrKodStrAd("DBHB", "Sözleşme Hak Edişi–1B – Yapım İşleri Hak Edişi"),
            new StrKodStrAd("DBHC", "Sözleşme Hak Edişi–1C – Danışmanlık Hizmetleri Hak Edişi"),
            new StrKodStrAd("DBHD", "Sözleşme Hak Edişi–1D – Danışmanlık Dışı Hizmetlerin Hak Edişi"),
            new StrKodStrAd("DBHE", "Sözleşme Hak Edişi–1E – Eğitim Hak Edişi"),
            new StrKodStrAd("DBHF", "Sözleşme Hak Edişi–1F – Operasyonel Giderleri Hak Edişi"),
            new StrKodStrAd("DBHG", "1-F Yurt İçi Yolluk Ödemeleri"),
            new StrKodStrAd("DBHH", "1-F Yurt Dışı Yolluk Ödemeleri"),
            new StrKodStrAd("DBHI", "2–Kredi Ödemeleri"),
            new StrKodStrAd("DBHJ", "3–Hibe Ödemeleri"),
            new StrKodStrAd("DBHK", "Avans Hak Edişi"),
            new StrKodStrAd("HKS", "Hakediş Ödeme Sonrası"),
            new StrKodStrAd("DBH", "Hakedişler"),
        };

        public static StrKodStrAd[] BelgeTuru = {
            new StrKodStrAd("1", "Fatura"),
            new StrKodStrAd("2", "Sözleşme"),
            new StrKodStrAd("3", "Makbuz"),
            new StrKodStrAd("5", "Yolluk"),
            new StrKodStrAd("6", "İlan"),
            new StrKodStrAd("4", "Diğer") };

        public static StrKodStrAd[] ParaBirimi = {
            new StrKodStrAd("TRY", "TL"),
            new StrKodStrAd("USD", "USD"),
            new StrKodStrAd("EUR", "Euro") };

        public static StrKodStrAd[] Durum = {
            new StrKodStrAd("90", "İptal Hariç"), // 9 Hariç
            new StrKodStrAd("91", "İşlemde"), // 1,2
            new StrKodStrAd("3", "Onaya Gönderildi"), //3
            new StrKodStrAd("92", "Numara Bekleyen"), //4
            new StrKodStrAd("1", "Yeni"),
            new StrKodStrAd("2", "Değiştirildi"),
            new StrKodStrAd("4", "Onaylı"),
            new StrKodStrAd("5", "Numara Verildi"),
            new StrKodStrAd("6", "Ödeme Tamamlandı"),
            new StrKodStrAd("9", "İptal") };

        public static StrKodStrAd[] SozlesmeDurum = {
            new StrKodStrAd("1", "İşlemde"), // 1,2
            new StrKodStrAd("5", "Tamamlandı"),
            new StrKodStrAd("6", "Kapatıldı"),
            new StrKodStrAd("7", "Sözleşme İptali"),
            new StrKodStrAd("9", "İptal Edildi") };

        public static StrKodStrAd[] ButceKategori = {
            new StrKodStrAd("1A", "Mallar (GO)"),
            new StrKodStrAd("1B", "Yapım İşler (W)"),
            new StrKodStrAd("1C", "Danışmanlık Hizmetleri (CS)"),
            new StrKodStrAd("1D", "Danışmanlık Dışı Hizmetler (NCS)"),
            new StrKodStrAd("1E", "Eğitim Giderleri (E)"),
            new StrKodStrAd("1F", "Operasyon Giderleri (OC)"),
            new StrKodStrAd("2", "Kredi"),
            new StrKodStrAd("3", "Eşleştirme Hibesi")
        };

        public static StrKodStrAd[] KDVGirisTuru = {
            new StrKodStrAd("-1", "Elle"),
            new StrKodStrAd("0", "KDVSiz"),
            new StrKodStrAd("1", "%1"),
            new StrKodStrAd("5", "%5"),
            new StrKodStrAd("7", "%7"),
            new StrKodStrAd("10", "%10"),
            new StrKodStrAd("17", "%17"),
            new StrKodStrAd("20", "%20") };

        public static StrKodStrAd[] OnceSonraKodlar = {
            new StrKodStrAd("1", "Önce"),
            new StrKodStrAd("2", "Sonra"),
        };
        //public static string[] adlarEn = { "Prior", "Post" };


        public enum DBListeTuru
        {
            KesintiTuru,
            HakedisTuru,
            BelgeTuru,
            ParaBirimi,
            Durum,
            ButceKategori,
            KDVGirisTuru,
            ModulKodlar,
            SozlesmeDurum,
            OnceSonra,
            HakedisTurEkliDosya
        }

        public static StrKodStrAd[] ListeBulArr(DBListeTuru kodTuru)
        {
            switch (kodTuru)
            {
                case DBListeTuru.KesintiTuru: return KesintiTuru;
                case DBListeTuru.HakedisTuru: return HakedisTuru;
                case DBListeTuru.BelgeTuru: return BelgeTuru;
                case DBListeTuru.ParaBirimi: return ParaBirimi;
                case DBListeTuru.Durum: return Durum;
                case DBListeTuru.ButceKategori: return ButceKategori;
                case DBListeTuru.KDVGirisTuru: return KDVGirisTuru;
                case DBListeTuru.ModulKodlar: return ModulKodlar;
                case DBListeTuru.SozlesmeDurum: return SozlesmeDurum;
                case DBListeTuru.OnceSonra: return OnceSonraKodlar;
                case DBListeTuru.HakedisTurEkliDosya: return HakedisTurEkliDosya;
            }
            return null;
        }

        public static TNSCollection ListeBul(DBListeTuru kodTuru)
        {
            TNSCollection liste = new TNSCollection();
            StrKodStrAd[] listeArr = ListeBulArr(kodTuru);
            if (liste != null)
            {
                liste.AddRange(listeArr);
            }

            return liste;
        }

        public static List<StrKodStrAd> ListeBulList(DBListeTuru kodTuru)
        {
            List<StrKodStrAd> liste = new List<StrKodStrAd>();
            StrKodStrAd[] listeArr = ListeBulArr(kodTuru);
            if (liste != null)
            {
                liste.AddRange(listeArr);
            }

            return liste;
        }

        public static string AdBul(DBListeTuru kodTuru, string kod)
        {
            StrKodStrAd[] listeArr = ListeBulArr(kodTuru);
            if (listeArr != null)
            {
                foreach (StrKodStrAd item in listeArr)
                {
                    if (item.kod == kod) return item.ad;
                }
            }
            return "";
        }

        public static string AdBulInt(DBListeTuru kodTuru, int kod)
        {
            StrKodStrAd[] listeArr = ListeBulArr(kodTuru);
            if (listeArr != null)
            {
                foreach (StrKodStrAd item in listeArr)
                {
                    if (OrtakFonksiyonlar.ConvertToInt(item.kod, -1) == kod) return item.ad;
                }
            }
            return "";
        }
    }


    public static class DBButceKategori
    {
        //    public static string[] kodlar = { "1A", "1B", "1C", "1D", "1E", "1F", "2A" };
        //    public static string[] adlar = { "Mallar", "İşler", "Danışmanlık Hizmetleri", "Danışmanlık Dışı Hizmetler", "Eğitim Giderleri", "Operasyon Giderleri", "Eşleştirme Hibesi" };
        //    public static string[] adlarEn = { "Goods", "Works", "Consulting Services", "Non-Consulting Services", "Eğitim Giderleri", "Operasyon Giderleri", "Matching Grants" };

        public static string AdBul(string kod)
        {
            return KodAdYonetim.AdBul(DBListeTuru.ButceKategori, kod);
        }

        public static List<StrKodStrAd> ListeOl()
        {
            return KodAdYonetim.ListeBulList(DBListeTuru.ButceKategori);
        }
    }

    public static class DBSatinAlmaYontemi
    {
        public static string[] kodlar = { "1", "2", "3", "4", "5" };
        public static string[] adlar = { "Sınırlı - Ulusal", "Açık - Uluslararası", "Açık - Ulusal", "Açık - Ulusal/Uluslararası", "Açık/Sınırlı - Ulusal" };
        public static string[] adlarEn = { "Limited - National", "Open - International", "Open - National", "Open - National/International", "Open/Limited - National" };

        public static string AdBul(string kod)
        {
            for (int i = 0; i < kodlar.Length; i++)
            {
                if (kodlar[i] == kod) return adlar[i];
            }
            return "";
        }

        public static List<StrKodStrAd> ListeOl()
        {
            List<StrKodStrAd> l = new List<StrKodStrAd>();
            l.Add(new StrKodStrAd("", "Seçiniz"));
            for (int i = 0; i < kodlar.Length; i++)
            {
                l.Add(new StrKodStrAd(kodlar[i], adlar[i]));
            }
            return l;
        }
    }

    public static class DBSecimYontemi
    {
        public static string[] kodlar = { "CQS", "CS", "FFBP", "INDV", "QCBS", "RFB", "RFBG", "RFBIS", "RFBQ", "RFBSW", "RFQ" };
        public static string[] adlar = { "Danışmanın Niteliklere Dayalı Seçimi (CQS)",
                                        "Ticari Uygulama (CS)",
                                        "Teklif Talebi - Tesis (Tasarım, Tedarik ve Kurulum - Ön Yeterlilik Olmadan Tek Aşamalı)",
                                        "Bireysel Danışmanlar (INDV)",
                                        "Kalite ve Maliyet Esaslı Seçim (QCBS)",
                                        "Teklif Talebi (RFB)",
                                        "Teklif Talebi (RFB) - Mal Alımı",
                                        "Teklif Talebi (RFB) - Bilgi Sistemi",
                                        "Teklif Talebi (RFB) / Fiyat Teklifi Talebi (RFQ)",
                                        "Teklif Talebi (RFB) - Küçük Ölçekli İşler",
                                        "Fiyat Teklifi Talebi (RFQ)",
                                         };
        public static string[] adlarEn = {
                                        "Consultant’s Qualification-based Selection (CQS)",
                                        "Commercial Practice (CS)",
                                        "Request for Bids Plant (Design, Supply and Installation Single Stage without Prequalification)",
                                        "Individual Consultants (INDV)",
                                        "Quality and Cost-based Selection (QCBS)",
                                        "Request for Bids (RFB)",
                                        "Request for Bids (RFB) Goods",
                                        "Request for Bids (RFB) (Information System)",
                                        "Request for Bids (RFB) / Requests for Quotations (RFQ)",
                                        "Request for Bids (RFB) Small Works",
                                        "Request for Quotations (RFQ)",
        };

        public static string AdBul(string kod)
        {
            for (int i = 0; i < kodlar.Length; i++)
            {
                if (kodlar[i] == kod) return adlar[i];
            }
            return "";
        }

        public static List<StrKodStrAd> ListeOl()
        {
            List<StrKodStrAd> l = new List<StrKodStrAd>();
            l.Add(new StrKodStrAd("", "Seçiniz"));
            for (int i = 0; i < kodlar.Length; i++)
            {
                l.Add(new StrKodStrAd(kodlar[i], adlar[i]));
            }
            return l;
        }
    }

    public static class DBOnceSonra
    {
        public static int[] kodlar = { 1, 2 };
        public static string[] adlar = { "Önce", "Sonra" };
        public static string[] adlarEn = { "Prior", "Post" };

        public static string AdBul(int kod)
        {
            for (int i = 0; i < kodlar.Length; i++)
            {
                if (kodlar[i] == kod) return adlar[i];
            }
            return "";
        }

        public static List<StrKodStrAd> ListeOl()
        {
            List<StrKodStrAd> l = new List<StrKodStrAd>();
            l.Add(new StrKodStrAd("", "Seçiniz"));
            for (int i = 0; i < kodlar.Length; i++)
            {
                l.Add(new StrKodStrAd(kodlar[i].ToString(), adlar[i]));
            }
            return l;
        }
    }

    [Serializable()]
    public class DBIkraz
    {
        public TNSCollection bilesenler { get; set; } = new TNSCollection();
        public TNSCollection altBilesenler { get; set; } = new TNSCollection();

        /// <summary>
        /// İkraz ID: 
        /// </summary>
        public int ikrazID { get; set; }

        /// <summary>
        /// Anlaşma Adı Tr: 
        /// </summary>
        public string anlasmaAdiTr { get; set; }

        /// <summary>
        /// Anlaşma Adı En: 
        /// </summary>
        public string anlasmaAdiEn { get; set; }

        /// <summary>
        /// anlasmaKodu: 
        /// </summary>
        public string anlasmaKodu { get; set; }

        /// <summary>
        /// baslangicYili: 
        /// </summary>
        public int baslangicYili { get; set; }

        /// <summary>
        /// bitisYili: 
        /// </summary>
        public int bitisYili { get; set; }

        /// <summary>
        /// paraBirimi: 
        /// </summary>
        public string paraBirimi { get; set; }

        /// <summary>
        /// donusumKuru: 
        /// </summary>
        public double donusumKuru { get; set; }
        public string mbHesapNo { get; set; }
        public string ikrazNo { get; set; }

        public DBAltBilesen AltBilesenBul()
        {
            DBBilesen bil = BilesenBul();
            if (bil != null)
                return bil.AltBilseneBul();
            else return null;
        }

        private DBBilesen BilesenBul()
        {
            if (bilesenler != null && bilesenler.Count >= 1)
                return (DBBilesen)bilesenler[0];
            return null;
        }

        public void Kontrol(Sonuc s)
        {
            if (string.IsNullOrWhiteSpace(this.anlasmaAdiTr)) s.DegerAta("Lütfen anlaşmanın Türkçe adını giriniz!");
            if (string.IsNullOrWhiteSpace(this.anlasmaAdiEn)) s.DegerAta("Lütfen anlaşmanın İngilizce adını giriniz!");
            if (string.IsNullOrWhiteSpace(this.anlasmaKodu)) s.DegerAta("Lütfen anlaşmanın kodunu giriniz!");
            if (string.IsNullOrWhiteSpace(this.paraBirimi)) s.DegerAta("Lütfen anlaşmanın para birimini giriniz!");
            if (this.baslangicYili <= 0) s.DegerAta("Lütfen başlangıç yılını giriniz!");
            if (this.bitisYili <= 0) s.DegerAta("Lütfen bitiş yılını giriniz!");
            if (this.baslangicYili >= this.bitisYili) s.DegerAta("Başlangıç yılı, bitiş yılından küçük olmalıdır!");
            if (this.donusumKuru == 0) s.DegerAta("Lütfen anlaşmanın dönüşüm kurunu giriniz!");
        }
    }

    [System.Serializable()]
    public class DBBilesen
    {
        public DBIkraz ikraz = new DBIkraz();

        /// <summary>
        /// Bileşen ID: 
        /// </summary>
        public int bilesenID { get; set; }

        /// <summary>
        /// Bileşen No: 
        /// </summary>
        public string bilesenNo { get; set; }

        /// <summary>
        /// Bileşen Ad: 
        /// </summary>
        public string bilesenAd { get; set; }
        public int ikrazID { get { return ikraz.ikrazID; } set { ikraz.ikrazID = value; } }

        public TNSCollection altBilesenler { get; set; } = new TNSCollection();

        public void Kontrol(Sonuc s)
        {
            if (ikrazID <= 0) s.DegerAta("İkraz kodunu giriniz!");
            if (string.IsNullOrWhiteSpace(bilesenNo)) s.DegerAta("Bileşen numarası giriniz!");
            if (string.IsNullOrWhiteSpace(bilesenAd)) s.DegerAta("Bileşen adını giriniz!");
        }

        public double toplamButce { get; set; }

        internal DBAltBilesen AltBilseneBul()
        {
            if (altBilesenler != null && altBilesenler.Count >= 1)
                return (DBAltBilesen)altBilesenler[0];
            return null;
        }
    }

    /// <summary>
    /// DBAltBilesen: 
    /// </summary>
    [System.Serializable()]
    public class DBAltBilesen
    {
        public DBBilesen bilesen { get; set; } = new DBBilesen();

        /// <summary>
        /// Alt Bileşen ID: 
        /// </summary>
        public int altBilesenID { get; set; }

        /// <summary>
        /// Bilesen ID: 
        /// </summary>
        public int bilesenID { get { return bilesen.bilesenID; } set { bilesen.bilesenID = value; } }

        /// <summary>
        /// Alt Bileşen No: 
        /// </summary>
        public string altBilesenNo { get; set; }

        /// <summary>
        /// Alt Bileşen Ad: 
        /// </summary>
        public string altBilesenAd { get; set; }

        /// <summary>
        /// İlgili Birim No: 
        /// </summary>
        public string ilgiliBirimNo { get; set; }

        /// <summary>
        /// İlgili Birim Adı: 
        /// </summary>
        public string ilgiliBirimAdi { get; set; }

        /// <summary>
        /// İlgili Birim Bütçe Kodu: 
        /// </summary>
        public string ilgiliBirimButceKodu { get; set; }

        /// <summary>
        /// İlgili Birim Bütçe Kodu: 
        /// </summary>
        public string ilgiliBirimButceAdi { get; set; }

        public int bagYil { get; set; }

        /// <summary>
        /// İlgili PEB Kodu: 
        /// </summary>
        public string ilgiliPEBKod { get; set; }

        /// <summary>
        /// İlgili PEB Ad: 
        /// </summary>
        public string ilgiliPEBAd { get; set; }

        /// <summary>
        /// Toplam Bütçe: 
        /// </summary>
        public double toplamButce { get; set; }
        public int ikrazID { get { return bilesen.ikrazID; } set { bilesen.ikrazID = value; } }

        public TNSCollection satinAlmaPlani { get; set; } = new TNSCollection();

        public DBButceRef ButceRefBul()
        {
            if (satinAlmaPlani != null && satinAlmaPlani.Count >= 1)
                return ((DBSatinAlma)satinAlmaPlani[0]).ButceRefBul();
            return null;
        }

        public void Kontrol(Sonuc s)
        {
            if (bilesenID <= 0) s.DegerAta("Bileşen ID giriniz!");
            if (string.IsNullOrWhiteSpace(altBilesenNo)) s.DegerAta("Alt bileşen no giriniz!");
            if (string.IsNullOrWhiteSpace(altBilesenAd)) s.DegerAta("Alt bileşen adı giriniz!");
            if (string.IsNullOrWhiteSpace(ilgiliBirimNo)) s.DegerAta("Birimin anlaşmadaki kodunu giriniz!");
            if (string.IsNullOrWhiteSpace(ilgiliBirimAdi)) s.DegerAta("Birim adını giriniz!");
            if (string.IsNullOrWhiteSpace(ilgiliBirimButceKodu)) s.DegerAta("Birimin kurumsal kodunu giriniz!");
            if (bagYil < 2020) s.DegerAta("Lütfen kod isimlerini bulmak için kullanılan, bağlantı yılını giriniz!");
            if (toplamButce <= 0) s.DegerAta("Alt bileşen bütçesini giriniz!");
        }
    }

    /// <summary>
    /// DBButceRef: 
    /// </summary>
    [System.Serializable()]
    public class DBButceRef
    {
        /// <summary>
        /// Bütçe Ref ID: 
        /// </summary>
        public int butceRefID { get; set; }

        /// <summary>
        /// Bütçe Ref ID: 
        /// </summary>
        public int ikrazID { get; set; }
        public string ilgiliBirimAdi { get; set; }

        /// <summary>
        /// Alt Bileşen ID: 
        /// </summary>
        public int satinAlmaID { get; set; }

        /// <summary>
        /// Referans No: 
        /// </summary>
        public string referansNo { get; set; }

        /// <summary>
        /// Açıklama Tr: 
        /// </summary>
        public string aciklamaTr { get; set; }

        /// <summary>
        /// Açıklama En: 
        /// </summary>
        public string aciklamaEn { get; set; }


        /// <summary>
        /// Bütçe Kategori: 
        /// </summary>
        public string butceKategori { get; set; }

        /// <summary>
        /// Ödenek: 
        /// </summary>
        public double odenek { get; set; }

        public double odenekAlt { get; set; }

        public double odenekUst { get; set; }

        public int durum { get; set; }

        public string durumAd
        {
            get
            {
                return "Yeni";
            }
        }

        public int altBilesenID { get; set; }
        public string aramaMetni { get; set; }
        public TNSCollection lotlar { get; set; } = new TNSCollection();
        public DBButceRefLot[] lotlarArr { get { return null; } set { lotlar.AddRange(value); } }
    }


    /// <summary>
    /// DBButceRef: 
    /// </summary>
    [System.Serializable()]
    public class DBButceRefLot
    {
        /// <summary>
        /// Bütçe Ref Lot ID: 
        /// </summary>
        public int butceRefLotID { get; set; }

        /// <summary>
        /// Bütçe Ref ID: 
        /// </summary>
        public int butceRefID { get; set; }

        /// <summary>
        /// lotNo: 
        /// </summary>
        public int lotNo { get; set; }

        /// <summary>
        /// aciklama: 
        /// </summary>
        public string aciklama { get; set; }
    }

    /// <summary>
    /// DBSatinAlma: 
    /// </summary>
    [System.Serializable()]
    public class DBSatinAlmaGerceklesmePlan
    {
        public int satinAlmaID { get; set; } = 0;
        public int dagilimID { get; set; } = 0;
        public TNSDateTime planTarihi { get; set; } = new TNSDateTime();
        public DateTime planTarihiDT { get { return planTarihi.Oku(); } set { planTarihi.Yaz(value); } }

        public int durum { get; set; }
        public string aciklama { get; set; }
        public double toplam { get; set; } = 0;

        public TNSCollection detaylar = new TNSCollection();
        public List<DBOdemePlaniDetay> detaylarTanimli
        {
            get { return detaylar?.Cast<DBOdemePlaniDetay>().ToList(); }
            set { detaylar.AddRange(value); }
        }

        public string MetneCevir()
        {
            string s = string.Empty;

            foreach (DBOdemePlaniDetay item in detaylar)
            {
                s += item.MetneCevir() + "\r\n";
            }
            return s;
        }
    }

    /// <summary>
    /// DBSatinAlma: 
    /// </summary>
    [System.Serializable()]
    public class DBSatinAlma
    {
        public TNSCollection butceRefler = new TNSCollection();
        public TNSCollection gerceklesmePlanlar = new TNSCollection();

        /// <summary>
        /// Bütçe Ref ID: 
        /// </summary>
        public int satinAlmaID { get; set; }

        /// <summary>
        /// Bütçe Ref ID: 
        /// </summary>
        public int ikrazID { get; set; }
        public string ilgiliBirimAdi { get; set; }

        /// <summary>
        /// Alt Bileşen ID: 
        /// </summary>
        public int altBilesenID { get; set; }

        /// <summary>
        /// Satın Alma No 
        /// </summary>
        public int satinAlmaNo { get; set; }

        /// <summary>
        /// Satın Alma No 
        /// </summary>
        public int satinAlmaGrupNo { get; set; }

        /// <summary>
        /// Açıklama Tr: 
        /// </summary>
        public string aciklamaTr { get; set; }

        /// <summary>
        /// Açıklama En: 
        /// </summary>
        public string aciklamaEn { get; set; }

        /// <summary>
        /// Referans No: 
        /// </summary>
        public string referansNo { get; set; }

        /// <summary>
        /// Referans Grup Ek: 
        /// </summary>
        public string referansGrupEk { get; set; }

        /// <summary>
        /// Referans Ek: 
        /// </summary>
        public string referansEk { get; set; }

        /// <summary>
        /// Bütçe Kategori: 
        /// </summary>
        public string butceKategori { get; set; }
        public string butceKategoriAd { get { return KodAdYonetim.AdBul(DBListeTuru.ButceKategori, butceKategori); } }

        /// <summary>
        /// Satın Alma Yöntemi : 
        /// </summary>
        public string satinAlmaYontemi { get; set; }

        /// <summary>
        /// Seçim Yöntemi: 
        /// </summary>
        public string secimYontemi { get; set; }

        /// <summary>
        /// Önce Sonra: 
        /// </summary>
        public int onceSonra { get; set; }

        /// <summary>
        /// Durum sorgusu:
        /// </summary>
        public int sorDurum { get; set; }

        /// <summary>
        /// Ödenek: 
        /// </summary>
        public double odenek { get; set; }

        public double odenekEUR { get; set; }

        /// <summary>
        /// Tahmini Davet Tarihi : 
        /// </summary>
        public string tahminiDavetTarihi { get; set; }

        /// <summary>
        /// Planlanan İmza Tarihi : 
        /// </summary>
        public string planlananImzaTarihi { get; set; }

        /// <summary>
        /// Planlanan Bitiş Tarihi : 
        /// </summary>
        public string planlananBitisTarihi { get; set; }

        /// <summary>
        /// Alt birim kodu: 
        /// </summary>
        public string altBirimAd { get; set; }

        public double odenekAlt { get; set; }

        public double odenekUst { get; set; }

        internal DBButceRef ButceRefBul()
        {
            if (butceRefler != null && butceRefler.Count >= 1)
                return (DBButceRef)butceRefler[0];
            else return null;
        }

        public DBSatinAlmaGerceklesmePlan PlanBul()
        {
            if (this.gerceklesmePlanlar == null || this.gerceklesmePlanlar.Count == 0) return null;
            return (DBSatinAlmaGerceklesmePlan)this.gerceklesmePlanlar[this.gerceklesmePlanlar.Count - 1];
        }

        public DBSatinAlmaGerceklesmePlan GerceklesmePlanBul(int dagilimID)
        {
            foreach (DBSatinAlmaGerceklesmePlan gp in gerceklesmePlanlar)
            {
                if (gp.dagilimID == dagilimID)
                {
                    return gp;
                }
            }
            return null;
        }

        public TNSDateTime kayitTarih { get; set; } = new TNSDateTime();
        public TNSDateTime odemeSonTarih { get; set; } = new TNSDateTime();
    }

    [Serializable()]
    public class DBYuklenebilecekDosyalar
    {
        public int kod { get; set; }
        public int siraNo { get; set; }
        public string dosyaAd { get; set; }
        //public string acıklama { get; set; } 
        public string yukluDosyaAd { get; set; } = "";
        public string modulKod { get; set; }
        public string modulKodAd { get { return KodAdYonetim.AdBul(DBListeTuru.ModulKodlar, modulKod); } }

        public void Kontrol(Sonuc s)
        {
            if (string.IsNullOrWhiteSpace(this.modulKod)) s.DegerAta("Lütfen Modul Kod seçin!");
            if (string.IsNullOrWhiteSpace(this.dosyaAd)) s.DegerAta("Lütfen İçerik İsmini girin!");
            if (this.kod < 0) s.DegerAta("Dosyanın ID sinde sorun var!");
            if (this.siraNo <= 0) s.DegerAta("Lütfen Sıra No Giriniz");
        }
    }

    [Serializable()]
    public class DBSozlesme
    {
        public string aramaMetni { get; set; }
        public int sozlesmeID { get; set; } = 0;
        public int butceRefID { get; set; } = 0;
        public int butceRefLotID { get; set; } = 0;
        public int ilgiliKod { get; set; } = 0;
        public int sozlesmeNo { get; set; } = 0;
        public string konu { get; set; }
        public string aciklama { get; set; }
        public string firmaVD { get; set; }
        public string firmaVNo { get; set; }
        public string firmaAdi { get; set; }
        public string firmaIBAN { get; set; }
        public TNSDateTime sozlesmeTarihi { get; set; } = new TNSDateTime();
        public DateTime sozlesmeTarihiDT { get { return sozlesmeTarihi.Oku(); } set { sozlesmeTarihi.Yaz(value); } }

        public TNSDateTime zeyilTarihi { get; set; } = new TNSDateTime();
        public DateTime zeyilTarihiDT { get { return zeyilTarihi.Oku(); } set { zeyilTarihi.Yaz(value); } }

        public string paraBirimi { get; set; }
        public double kurUSD { get; set; } = 0;
        public double kurEUR { get; set; } = 0;
        public int avansHukmu { get; set; } = 0;
        public double avansOrani { get; set; } = 0;
        public int avansMahsubu { get; set; } = 0;
        public double avansMahsubuOrani { get; set; } = 0;
        public int durum { get; set; } = 1;
        public string durumAd { get { return KodAdYonetim.AdBulInt(DBListeTuru.SozlesmeDurum, durum); } }

        public int ikrazID { get; set; } = 0;
        public string anlasmaKodu { get; set; }

        public int bilesenID { get; set; } = 0;
        public string bilesenNo { get; set; }
        public string bilesenAdi { get; set; }

        public int altBilesenID { get; set; } = 0;
        public string altBilesenNo { get; set; }
        public string altBilesenAdi { get; set; }

        public string satKategori { get; set; }
        public int satID { get; set; } = 0;
        public string satReferansNo { get; set; }

        public string satAcikTR { get; set; }
        public string satAcikEN { get; set; }
        public string satGrupEk { get; set; }

        public string butceReferansNo { get; set; }
        public string butceReferansAciklama { get; set; }
        public int butceRefLotNo { get; set; } = 0;
        public string butceRefLotAciklama { get; set; }

        public double sozlesmeTutari { get; set; } = 0;
        public double sozlesmeTutariSon { get; set; } = 0;

        public double sozlesmeTutariSonTL
        {
            get
            {
                return DBHakedisOzeti.DegerCevir(sozlesmeTutariSon, paraBirimi, "TRY", kurEUR, kurUSD);
            }
        }

        public double sozlesmeTutariSonEUR
        {
            get
            {
                return DBHakedisOzeti.DegerCevir(sozlesmeTutariSon, paraBirimi, "EUR", kurEUR, kurUSD);
            }
        }
        public double sozlesmeTutariSonUSD
        {
            get
            {
                return DBHakedisOzeti.DegerCevir(sozlesmeTutariSon, paraBirimi, "USD", kurEUR, kurUSD);
            }
        }

        public double kalanSon { get; set; } = 0;
        public double kalanSonEUR { get; set; } = 0;

        public double odenen { get; set; } = 0;
        public double odenenTL { get; set; } = 0;
        public double odenenEUR { get; set; } = 0;
        public double odenenUSD { get; set; } = 0;
        public double odenmeyecekEUR { get; set; } = 0;

        public TNSCollection odemePlani = null;
        public TNSCollection hakedisler = null;
        public string secimAciklama
        {
            get
            {
                return $"{firmaAdi}-({sozlesmeTarihi}-{sozlesmeTutariSon.ToString("#,##0.00")} {paraBirimi}) {(butceRefLotNo > 0 ? "(LOT=" + butceRefLotNo + ")" : "")} {aciklama}";
            }
        }

        public int onceSonra { get; set; } = 0;

        public int sorDurum { get; set; } = 0;

        public void SozlesmeTutarHesapla(double kurUSD, double kurEUR, bool sozlesmesiz = false)
        {
            if (!sozlesmesiz)
            {
                sozlesmeTutari = ToplamBul(OdemePlaniBul(true, true));

                DBOdemePlani op = OdemePlaniBul(false, true);
                sozlesmeTutariSon = ToplamBul(op);

                if (op != null && op.odemePlaniTipi != 0)
                    zeyilTarihi.Yaz(op.odemePlaniTarihi);
                else
                    zeyilTarihi.Yaz(DateTime.MinValue);
            }
            else
            {
                sozlesmeTutari = 0;
                zeyilTarihi.Yaz(DateTime.MinValue);
            }
            odenenTL = 0;
            odenenEUR = 0;
            odenenUSD = 0;
            foreach (DBHakedisOzeti item in hakedisler)
            {
                odenenTL += item.odenenTL;
                odenenEUR += item.odenenEUR;
                odenenUSD += item.odenenUSD;
            }
            if (paraBirimi == "EUR") odenen = odenenEUR;
            else if (paraBirimi == "USD") odenen = odenenUSD;
            else odenen = odenenTL;
            if (sozlesmesiz)
            {
                if (paraBirimi == "EUR") sozlesmeTutari = odenenEUR;
                else if (paraBirimi == "USD") sozlesmeTutari = odenenUSD;
                else sozlesmeTutari = odenenTL;
                sozlesmeTutariSon = sozlesmeTutari;
                kalanSon = 0;
                kalanSonEUR = 0;
            }
            else
            {
                kalanSon = (sozlesmeTutariSon == 0 ? sozlesmeTutari : sozlesmeTutariSon) - odenen;
                kalanSonEUR = DBHakedisOzeti.DegerCevir(kalanSon, paraBirimi, "EUR", kurEUR, kurUSD);
            }
        }

        private double ToplamBul(DBOdemePlani odemePlani)
        {
            double top = 0;
            if (odemePlani != null && odemePlani.detaylar != null)
            {
                foreach (DBOdemePlaniDetay item in odemePlani.detaylar)
                {
                    if (item != null)
                        top += item.odenecekTutar;
                }
            }
            return top;
        }

        public DBOdemePlani OdemePlaniBul(bool ilk, bool sonZeyil)
        {
            if ((odemePlani == null || odemePlani.Count == 0)) return null;
            if (ilk) return (DBOdemePlani)odemePlani[0];

            for (int i = odemePlani.Count - 1; i >= 0; i--)
            {
                DBOdemePlani dbOdemePlani = (DBOdemePlani)odemePlani[i];
                if (dbOdemePlani.durum != 5) continue;
                if (!sonZeyil) return dbOdemePlani;
                if (dbOdemePlani.odemePlaniTipi != 2) return dbOdemePlani;
            }
            return null;
        }

        public TNSDateTime tahminiBaslamaTarihi { get; set; } = new TNSDateTime();
        public DateTime tahminiBaslamaTarihiDT { get { return tahminiBaslamaTarihi.Oku(); } set { tahminiBaslamaTarihi.Yaz(value); } }

        public string avansParaBirimi { get; set; }
        public string avansTeminatMektubu { get; set; }
        public string kesinTeminatMektubu { get; set; }
        public bool ihtiyatKesintisiIceriyorMu { get; set; }
        public bool sarfMalzemesiVarMi { get; set; }
        public bool demirbasVarMi { get; set; }
        public TNSDateTime sozlesmeNoAlmaTarihi { get; set; } = new TNSDateTime();
        public DateTime sozlesmeNoAlmaTarihiDT { get { return sozlesmeNoAlmaTarihi.Oku(); } set { sozlesmeNoAlmaTarihi.Yaz(value); } }

        public string gerekce { get; set; }
        public TNSDateTime odemeSonTarih { get; set; }
        public TNSDateTime kayitTarih { get; set; }
        public bool mbOdemelerden { get; set; } = false;

        public TNSDateTime sozTarihiBasla { get; set; } = new TNSDateTime();
        public DateTime sozTarihiBaslaDT { get { return sozTarihiBasla.Oku(); } set { sozTarihiBasla.Yaz(value); } }
        public TNSDateTime sozTarihiBit { get; set; } = new TNSDateTime();
        public DateTime sozTarihiBitDT { get { return sozTarihiBit.Oku(); } set { sozTarihiBit.Yaz(value); } }
        public double sozTutarBasla { get; set; } = 0;
        public double sozTutarBit { get; set; } = 0;
        public string paraBirimiSor { get; set; }
    }

    [Serializable()]
    public class DBHakedisTarihce
    {
        public TNSDateTime islemTarihi { get; set; } = new TNSDateTime();
        public DateTime islemTarihiDT { get { return islemTarihi.Oku(); } set { islemTarihi.Yaz(value); } }
        public string islemTarihiStr { get { return islemTarihiDT.ToString("dd.MM.yyyy HH:mm"); } }
        public string islem { get; set; }
        public string islemiYapan { get; set; }
        public string ekBilgi { get; set; }
    }


    [Serializable()]
    public class DBHakedisMalzeme
    {
        public string malzemeAdi { get; set; }
        public int miktar { get; set; }
        public double birimFiyat { get; set; }
        public int siraNo { get; set; }
    }


    [Serializable()]
    public class DBYollukBilgisi
    {
        public string yer { get; set; }
        public string aciklama { get; set; }
        public double tutar { get; set; }
        public int siraNo { get; set; }
    }

    [Serializable()]
    public class DBHakedisSahibi
    {
        public int hakedisID { get; set; }
        public int siraNo { get; set; }
        public int ilgiliKod { get; set; }
        public int numarator { get; set; }
        public int merkezNumaratorID { get; set; }
        public int odendi { get; set; } //0 bekliyor, 5 ödendi, 9 Ödenmedi
        public string ilgiliAd { get; set; }
        public string ilgiliVNO { get; set; }
        public string ilgiliIBAN { get; set; }
        public enumKesintiTuru kesintiTurKod { get; set; }
        public string kesintiTurAd { get { return KodAdYonetim.AdBulInt(DBListeTuru.KesintiTuru, (int)kesintiTurKod); } }
        public string aciklama { get; set; }
        public string paraBirimi { get; set; }
        public double kesintiTutari { get; set; }
        public double kesintiTutariTL
        {
            get
            {
                if (paraBirimi == "EUR") return TNS.HRC.Arac.ParaCevir(kesintiTutari, kurEUR, true);
                else if (paraBirimi == "USD") return TNS.HRC.Arac.ParaCevir(kesintiTutari, kurUSD, true);
                else return kesintiTutari;
            }
        }
        public double kesintiTutariEUR { get { return TNS.HRC.Arac.ParaCevir(kesintiTutariTL, kurEUR, false); } }
        public double kesintiTutariUSD { get { return TNS.HRC.Arac.ParaCevir(kesintiTutariTL, kurUSD, false); } }
        public double kurUSD { get; set; } // TL/USD Kuru
        public double kurEUR { get; set; } // TL/EUR Kuru
        public string bankaSube { get; set; }
        public string hakedisGerekce { get; set; }
        public string butceKategori { get; set; }
        public string butceKategoriAd { get { return KodAdYonetim.AdBul(DBListeTuru.ButceKategori, butceKategori); } }

        public string odendiAd
        {
            get
            {
                if (odendi == 0)
                {
                    if (numarator == 0) return "No Bekliyor";
                    else if (merkezNumaratorID == 0) return "Merkez No Bekliyor";
                    else return "Ödeme Bekleniyor";
                }
                else if (odendi == 1 || odendi == 5) return "Ödendi";
                else return "İptal";
            }
        }

        public int altBilesenID { get; set; }
        public TNSDateTime odemeTarihi { get; set; } = new TNSDateTime();

        public string odemeTarihiStr
        {
            get
            {
                if (TNSDateTime.IsNull(odemeTarihi)) return "";
                return odemeTarihi.ToString();
            }

            set
            {
                odemeTarihi.Yaz(value);
            }
        }

        public double aktarilanTutarTL { get { if (TNSDateTime.IsNull(odemeTarihi)) return 0; return kesintiTutariTL; } }
        public double aktarilanTutarUSD { get { return TNS.HRC.Arac.ParaCevir(aktarilanTutarTL, aktarimKurUSD, false); } }
        public double aktarilanTutarEUR { get { return TNS.HRC.Arac.ParaCevir(aktarilanTutarTL, aktarimKurEUR, false); } }
        public double aktarimKurUSD { get; set; } = 0;
        public double aktarimKurEUR { get; set; } = 0;
    }

    [Serializable()]
    public class DBHakedis
    {
        public int ikrazID { get; set; }
        public int altBilesenID { get; set; }
        public int hakedisID { get; set; }
        public int ilgiliKod { get; set; }
        public string ilgiliAd { get; set; }
        public string hakedisTurDosyaEk
        {
            get
            {
                return AdBul(DBListeTuru.HakedisTurEkliDosya, hakedisTurKod.ToString());
            }
        }

        public int hakedisTurKod { get; set; } // Hakediş Türü
        public string hakedisTurAd { get { return KodAdYonetim.AdBulInt(DBListeTuru.HakedisTuru, hakedisTurKod); } } // Hakediş Türü
        public int sozlesmeID { get; set; } // Sözleşme Seçimi
        public string sozlesmeTanim { get; set; } // Sözleşme no, Sözleşm Tarih ve Sözleşme ad alanları toplamı
        public int butceRefID { get; set; } // Bütçe Referansı (select2)
        public string butceRefAd { get; set; } // Bütçe Referansı (select2)
        public int belgeTurKod { get; set; } // Belge Türü
        public string belgeTurAd { get { return KodAdYonetim.AdBulInt(DBListeTuru.BelgeTuru, belgeTurKod); } } // Belge Türü
        public DateTime belgeTarihiDT { get { return belgeTarihi.Oku(); } set { belgeTarihi.Yaz(value); } } // Belge Tarihi
        public TNSDateTime belgeTarihi { get; set; } = new TNSDateTime(); // Belge Tarihi
        public TNSDateTime arastirmaTarihi { get; set; } = new TNSDateTime();
        public DateTime arastirmaTarihiDT
        {
            get { return arastirmaTarihi.Oku(); }
            set { arastirmaTarihi.Yaz(value); }
        }
        public string belgeNo { get; set; }
        public string paraBirimi { get; set; } // Para Birimi
        public DateTime kurTarihiDT { get { return kurTarihi.Oku(); } set { kurTarihi.Yaz(value); } } // Belge Tarihi
        public TNSDateTime kurTarihi { get; set; } = new TNSDateTime(); // Kur Tarihi
        public double kurUSD { get; set; } // TL/USD Kuru
        public double kurEUR { get; set; } // TL/EUR Kuru
        public double kdvHaricBelgeToplami { get; set; } // KDV Hariç Belge Toplamı
        public int kdvGirisTurKod { get; set; } // KDV Giriş Türü ("elle" veya "oranla")
        public string kdvGirisTurAd { get { return KodAdYonetim.AdBulInt(DBListeTuru.KDVGirisTuru, kdvGirisTurKod); } } // KDV Giriş Türü ("elle" veya "oranla")
        public double kdvOrani { get; set; } // KDV Oranı
        public double kdvTutari { get; set; } // KDV Tutarı (readonly)
        public double avansKesintisi { get; set; } // Avans Kesintisi
        public double odemeToplami { get { return kdvHaricBelgeToplami + kdvTutari - avansKesintisi; } } // Ödeme Toplamı (hesaplanır)

        public double odemeToplamiTL
        {
            get
            {
                if (paraBirimi == "EUR") return TNS.HRC.Arac.ParaCevir(odemeToplami, kurEUR, true);
                else if (paraBirimi == "USD") return TNS.HRC.Arac.ParaCevir(odemeToplami, kurUSD, true);
                else return odemeToplami;
            }
        }

        public double odemeToplamiEUR
        {
            get
            {
                return TNS.HRC.Arac.ParaCevir(odemeToplamiTL, kurEUR, false);
            }
        }

        public int durumKod { get; set; } // Durum (readonly)
        public string durumAd { get { return KodAdYonetim.AdBul(DBListeTuru.Durum, durumKod.ToString()); } } // Durum (readonly)
        public string butceKategori { get; set; }
        public string butceKategoriAd { get { return KodAdYonetim.AdBul(DBListeTuru.ButceKategori, butceKategori); } } // Durum (readonly)
        public string referansNo { get; set; }
        public string hakedisGerekce { get; set; }

        public TNSCollection hakedisSahipleri { get; set; } = new TNSCollection(); // Hakediş sahipleri listesi
        public DBHakedisSahibi[] hakedisSahipleriArr { get { return null; } set { hakedisSahipleri.AddRange(value); } }

        public TNSCollection yollukBilgileri { get; set; } = new TNSCollection(); // Hakediş sahipleri listesi
        public DBYollukBilgisi[] yollukBilgileriArr { get { return null; } set { yollukBilgileri.AddRange(value); } }

        public TNSCollection malzemeListesi { get; set; } = new TNSCollection(); // Hakediş sahipleri listesi
        public DBHakedisMalzeme[] malzemeListesiArr { get { return null; } set { malzemeListesi.AddRange(value); } }

        public TNSCollection tarihceListesi { get; set; } = new TNSCollection(); // Hakediş sahipleri listesi
        public string altBilesenNo { get; set; }
        public string altBilesenAd { get; set; }
        public string ikrazAnlasamaAdi { get; set; }
        public double ikrazDonusumKuru { get; set; }
        public string ikrazParaBirimi { get; set; }
        public string ilgiliVNO { get; set; }
        public string ilgiliIBAN { get; set; }
        public string altBilesenKurKod { get; set; }
        public double HakSahipleriToplami
        {
            get
            {
                double toplam = 0;
                foreach (DBHakedisSahibi item in hakedisSahipleri)
                {
                    toplam = OrtakFonksiyonlar.KurusYap(toplam + item.kesintiTutari);
                }
                return toplam;
            }
        }

        public bool HakedisToplamlariDogruMu()
        {
            return OrtakFonksiyonlar.KurusYap(HakSahipleriToplami) == OrtakFonksiyonlar.KurusYap(this.odemeToplami);
        }

        public double KuraCevir(double paraMiktari, string yeniParaBirimi)
        {
            return DBHakedisOzeti.DegerCevir(paraMiktari, paraBirimi, yeniParaBirimi, kurEUR, kurUSD);
        }

        public DBHakedisOdenek[] odenekListesiArr { get { return null; } set { odenekListesi.AddRange(value); } }
        public TNSCollection odenekListesi { get; set; } = new TNSCollection();

        public int lotNo { get; set; }
        public string lotAciklama { get; set; }
        public int hakedisNo { get; set; }
        public string ilgiliBirimAdi { get; set; }
        public int onceSonra { get; set; }
        public TNSDateTime numaratorAlmaTarihi { get; set; } = new TNSDateTime();
        public DateTime numaratorAlmaTarihiDT { get { return numaratorAlmaTarihi.Oku(); } set { numaratorAlmaTarihi.Yaz(value); } }

        public string butceRefRefNo { get; set; }
        public string butceSPRefNo { get; set; }

        public int numaratorAlt { get; set; } = 0;
        public int numaratorUst { get; set; } = 0;
        public bool yetkiliKullanici { get; set; } = false;
    }

    [Serializable()]
    public class DBMerkezBankasiNumarator
    {
        public int altBilesenID { get; set; }
        public int merkezNumaratorID { get; set; }
        public int numarator { get; set; }
        public TNSDateTime islemTarihi { get; set; } = new TNSDateTime();
        public string islemiYapan { get; set; }
        public int durum { get; set; }
        public string durumAd { get { return KodAdYonetim.AdBulInt(DBListeTuru.Durum, durum); } }
        public TNSCollection hakedisSahipleri { get; set; } = new TNSCollection();
        public string detsisNo { get; set; }
        public string ikrazNo { get; set; }
        public string birimKurKod { get; set; }
        public string mbHesapNo { get; set; }
        public string birimAd { get; set; }
        public string ikrazParaBirimi { get; set; }
        public double donusumKuru { get; set; }

        public DBImza imzaSol { get; set; } = new DBImza();
        public DBImza imzaSag { get; set; } = new DBImza();
    }
    /// <summary>
    /// DBImza: 
    /// </summary>
    [System.Serializable()]
    public class DBImza
    {

        /// <summary>
        /// İmza ID: 
        /// </summary>
        public int imzaID { get; set; }

        /// <summary>
        /// Alt Bileşen ID: 
        /// </summary>
        public int altBilesenID { get; set; }

        /// <summary>
        /// Ad Soyad: 
        /// </summary>
        public string adSoyad { get; set; }

        /// <summary>
        /// Satir1: 
        /// </summary>
        public string satir1 { get; set; }

        /// <summary>
        /// Satir2: 
        /// </summary>
        public string satir2 { get; set; }

        /// <summary>
        /// Pasif: 
        /// </summary>
        public bool pasif { get; set; }
    }

    [System.Serializable()]
    public class DBHakedisOzeti
    {
        public int hakedisID { get; set; }
        public int belgeTurKod { get; set; }
        public TNSDateTime belgeTarihi { get; set; } = new TNSDateTime();
        public DateTime belgeTarihiDT { get { return belgeTarihi.Oku(); } }
        public string belgeNo { get; set; }
        public string paraBirimi { get; set; }
        public TNSDateTime kurTarihi { get; set; } = new TNSDateTime();
        public double odenen { get; set; }

        public double kurEUR { get; set; }
        public double kurUSD { get; set; }

        public double trKarsilik { get; set; }
        public double eurKarsilik { get; set; }


        public double odenenTL
        {
            get
            {
                if (trKarsilik > 0) return trKarsilik;
                return DegerCevir(odenen, paraBirimi, "TRY", kurEUR, kurUSD);
            }
        }

        public double odenenEUR
        {
            get
            {
                if (eurKarsilik > 0) return eurKarsilik;
                return DegerCevir(odenen, paraBirimi, "EUR", kurEUR, kurUSD);
            }
        }

        public double odenenUSD
        {
            get
            {
                return DegerCevir(odenen, paraBirimi, "USD", kurEUR, kurUSD);
            }
        }

        public static double DegerCevir(double cevrilecekTutar, string cevrilecekParaBirimi, string hedefParaPirimi, double kurEUR, double kurUSD)
        {
            if (cevrilecekParaBirimi == hedefParaPirimi) return cevrilecekTutar;
            if (hedefParaPirimi == "EUR")
            {
                if (cevrilecekParaBirimi == "USD") return TNS.HRC.Arac.ParaCevir(TNS.HRC.Arac.ParaCevir(cevrilecekTutar, kurUSD, true), kurEUR, false);
                else return TNS.HRC.Arac.ParaCevir(cevrilecekTutar, kurEUR, false);
            }
            else if (hedefParaPirimi == "USD")
            {
                if (cevrilecekParaBirimi == "EUR") return TNS.HRC.Arac.ParaCevir(TNS.HRC.Arac.ParaCevir(cevrilecekTutar, kurEUR, true), kurUSD, false);
                else return TNS.HRC.Arac.ParaCevir(cevrilecekTutar, kurUSD, false);
            }
            else
            {
                if (cevrilecekParaBirimi == "EUR") return TNS.HRC.Arac.ParaCevir(cevrilecekTutar, kurEUR, true);
                else if (cevrilecekParaBirimi == "USD") return TNS.HRC.Arac.ParaCevir(cevrilecekTutar, kurUSD, true);
                else return cevrilecekTutar;
            }
        }

        public double OdenenBul(string paraBirimi)
        {
            if (paraBirimi == "USD") return odenenUSD;
            else if (paraBirimi == "EUR") return odenenEUR;
            else return odenenTL;
        }

        public bool OnceMi(int hakedisID, TNSDateTime belgeTarihi)
        {
            int kiyas = this.belgeTarihi.Oku().CompareTo(belgeTarihi.Oku());
            if (kiyas < 0) return true;
            if (kiyas == 0)
            {
                return this.hakedisID < hakedisID;
            }
            return false;
        }
    }

    [System.Serializable()]
    public class DBOdemePlani
    {
        public int odemePlanID { get; set; }
        public int sozlesmeID { get; set; }

        public TNSDateTime tahminiSozlesmeBitisi { get; set; } = new TNSDateTime();
        public DateTime tahminiSozlesmeBitisiDT { get { return tahminiSozlesmeBitisi.Oku(); } set { tahminiSozlesmeBitisi.Yaz(value); } }

        public TNSDateTime odemePlaniTarihi { get; set; } = new TNSDateTime();
        public DateTime odemePlaniTarihiDT { get { return odemePlaniTarihi.Oku(); } set { odemePlaniTarihi.Yaz(value); } }
        public int durum { get; set; }
        public string aciklama { get; set; } = "";
        public string paraBirimi { get; set; } = "";
        public int odemePlaniTipi { get; set; }
        public List<DBOdemePlaniDetay> detaylarTanimli
        {
            get { return detaylar?.Cast<DBOdemePlaniDetay>().ToList(); }
            set { detaylar.AddRange(value); }
        }
        public TNSCollection detaylar { get; set; } = new TNSCollection();
        public double kurUSD { get; set; }
        public double kurEUR { get; set; }
        public double odenecekTutar { get; set; } = 0;
        public double odenecekTutarTRY { get; set; } = 0;
        public double odenecekTutarEUR { get; set; } = 0;
        public double odenecekTutarUSD { get; set; } = 0;

        public string MetneCevir()
        {
            string s = string.Empty;

            foreach (DBOdemePlaniDetay item in detaylar)
            {
                s += item.MetneCevir() + "\r\n";
            }
            return s;
        }

        public void RakamlariHesapla()
        {
            odenecekTutar = 0;
            odenecekTutarEUR = 0;
            odenecekTutarUSD = 0;
            odenecekTutarTRY = 0;
            foreach (DBOdemePlaniDetay item in detaylar)
            {
                item.RakamlariHesapla(paraBirimi, kurUSD, kurEUR);
                odenecekTutar += item.odenecekTutar;
                odenecekTutarEUR += item.odenecekTutarEUR;
                odenecekTutarUSD += item.odenecekTutarUSD;
                odenecekTutarTRY += item.odenecekTutarTRY;
            }
        }
    }

    [System.Serializable()]
    public class DBOdemePlaniDetay : IComparable
    {
        public DBOdemePlaniDetay(int odemePlanID, int yil, int ay, double odenecekTutar)
        {
            this.odemePlanID = odemePlanID;
            this.yil = yil;
            this.ay = ay;
            this.odenecekTutar = odenecekTutar;
            this.odenecekTutarEUR = 0;
            this.odenecekTutarUSD = 0;
            this.odenecekTutarTRY = 0;
        }
        public DBOdemePlaniDetay()
        {
            this.odemePlanID = 0;
            this.yil = 0;
            this.ay = 0;
            this.odenecekTutar = 0.0;
            this.odenecekTutarEUR = 0;
            this.odenecekTutarUSD = 0;
            this.odenecekTutarTRY = 0;
        }

        public DBOdemePlaniDetay(DBOdemePlaniDetay kaynak)
        {
            this.odemePlanID = kaynak.odemePlanID;
            this.yil = kaynak.yil;
            this.ay = kaynak.ay;
            this.odenecekTutar = kaynak.odenecekTutar;
            this.odenecekTutarEUR = kaynak.odenecekTutarEUR;
            this.odenecekTutarUSD = kaynak.odenecekTutarUSD;
            this.odenecekTutarTRY = kaynak.odenecekTutarTRY;
        }

        public int odemePlanID { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public double odenecekTutar { get; set; }
        public double odenecekTutarEUR { get; set; }
        public double odenecekTutarUSD { get; set; }
        public double odenecekTutarTRY { get; set; }

        public bool BosMu()
        {
            return (yil <= 0 && ay <= 0 && odenecekTutar < 0.001);
        }

        public void RakamlariHesapla(string paraBirimi, double kurUSD, double kurEUR)
        {
            odenecekTutarEUR = DBHakedisOzeti.DegerCevir(odenecekTutar, paraBirimi, "EUR", kurEUR, kurUSD);
            odenecekTutarUSD = DBHakedisOzeti.DegerCevir(odenecekTutar, paraBirimi, "USD", kurEUR, kurUSD);
            odenecekTutarTRY = DBHakedisOzeti.DegerCevir(odenecekTutar, paraBirimi, "TRY", kurEUR, kurUSD);
        }

        public void Ekle(DBOdemePlaniDetay item)
        {
            this.odenecekTutar += item.odenecekTutar;
            this.odenecekTutarEUR += item.odenecekTutarEUR;
            this.odenecekTutarUSD += item.odenecekTutarUSD;
            this.odenecekTutarTRY += item.odenecekTutarTRY;
        }

        public void Sifirla()
        {
            this.odenecekTutar = 0;
            this.odenecekTutarEUR = 0;
            this.odenecekTutarUSD = 0;
            this.odenecekTutarTRY = 0;
        }

        public void Dus(DBOdemePlaniDetay item)
        {
            this.odenecekTutar -= item.odenecekTutar;
            this.odenecekTutarEUR -= item.odenecekTutarEUR;
            this.odenecekTutarUSD -= item.odenecekTutarUSD;
            this.odenecekTutarTRY -= item.odenecekTutarTRY;
        }

        public double DegerAl(int pb)
        {
            if (pb == 0) return this.odenecekTutar;
            if (pb == 1) return this.odenecekTutarTRY;
            if (pb == 2) return this.odenecekTutarEUR;
            if (pb == 3) return this.odenecekTutarUSD;
            return 0;
        }

        public void DegerAta(int pb, double deger)
        {
            if (pb == 0) this.odenecekTutar = deger;
            if (pb == 1) this.odenecekTutarTRY = deger;
            if (pb == 2) this.odenecekTutarEUR = deger;
            if (pb == 3) this.odenecekTutarUSD = deger;
        }

        public string MetneCevir()
        {
            return $"{yil}-{ay}- ORJ: {odenecekTutar} TRY:{odenecekTutarTRY} EUR:{odenecekTutarEUR} USD:{odenecekTutarUSD}";
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (!(obj is DBOdemePlaniDetay))
                return -1;

            DBOdemePlaniDetay other = (DBOdemePlaniDetay)obj;
            int kiyas = this.yil.CompareTo(other.yil);

            if (kiyas == 0)
                kiyas = this.ay.CompareTo(other.ay);

            return kiyas;
        }
    }

    [System.Serializable]
    public class DBHakedisOdenek
    {
        public int hakedisID { get; set; }
        public string ekoKod { get; set; }
        public double odenek { get; set; }
        public double harcama { get; set; }
        public double miktar { get; set; }
        public string ekoAd { get; set; }
    }

    [System.Serializable]
    public class DBEkstre
    {
        public string Numarator { get; set; }
        public int IkrazID { get; set; }
        public TNSDateTime EksDate { get; set; } = new TNSDateTime();
        public DateTime EksDateDT { get { return EksDate.Oku(); } set { EksDate.Yaz(value); } }
        public int EksDateSiraNo { get; set; }
        public string CatNo { get; set; }
        public string Explanation { get; set; }
        public string Cur { get; set; }
        public double AmountPaid { get; set; }
        public double ExchRate { get; set; }
        public double TryEquiv { get; set; }
        public double EurEquiv { get; set; }
        public double DepositEur { get; set; }
        public double DepositTry { get; set; }
        public double BalanceEur { get; set; }
        public double HakedisTutari { get; set; }
        public bool HakedisVar { get; set; }
        public int SGBNumarator { get; set; }
        public string EksDateSTR { get; set; }

        public void Kontrol(Sonuc s)
        {
            if (!s.islemSonuc)
                return;

            if (Numarator == null || Numarator == "")
            {
                s.DegerAta("Numaratör boş veya geçersiz!");
                return;
            }

            if (IkrazID <= 0)
            {
                s.DegerAta("İkraz bilgisi boş veya geçersiz!");
                return;
            }

            if (EksDate == null || EksDateDT == DateTime.MinValue)
            {
                s.DegerAta("Ekstre tarihi boş!");
                return;
            }
        }
    }

    public class DinamikAnahtarYapisi
    {
        public int AltBilesenID { get; set; }
        public string Kategori { get; set; }

        public override int GetHashCode()
        {
            return (AltBilesenID.ToString() + Kategori).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as DinamikAnahtarYapisi;

            if (other == null)
                return false;

            return other.AltBilesenID == AltBilesenID &&
                   other.Kategori == Kategori;
        }
    }

    public class DinamikVeriYapisi
    {
        public double Butce { get; set; } = 0;
        public double Harcama { get; set; } = 0;
    }

    [Serializable()]
    public class HaritaDTO
    {
        public string Ad { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public int Veri { get; set; }
        public int Level { get; set; }
    }
}
using System;
using System.Collections;
using System.Data;
using System.IO;
using OrtakClass;
using TNS;
using TNS.KPM;
using TNS.KYM;

namespace KimlikPaylasim
{
    /// <summary>
    /// Nüfus / kimlik paylaşım raporları (Rapor.xml grup 500, NESNE "KimlikPaylasim.NufusYazdir"; EKBILGI: 1 İkametgah İlmühaberi,
    /// 2 Nüfus Cüzdanı Örneği, 3 Vukuatlı Nüfus Kayıt Örneği). Eski KimlikPaylasim/NufusYazdir.cs'ten alındı; iş kuralı çağrıları
    /// (TNS.KPM: IkametgahRaporla, NufusKayitOrnekRaporla, VukuatliNufusKayitOrnekRaporla) ve alan değerleri aynıdır.
    /// Değişenler:
    ///   - EkRtf (.rtf şablon, .doc çıktı) yerine Aspose.Words adres-mektup birleştirmesi: şablonlar KPM\*.dotx, çıktı .docx.
    ///     Rapor.xml grup 500'deki SABLON adları .dotx olmalıdır (bkz. NOTLAR.md).
    ///   - İkametgah şablonundaki \IF (X&lt;&gt;'')\...\ENDIF\ yapıları .dotx'te alan çiftine indirildi: «X» değeri, «XEK» ise X boş
    ///     değilse eki (" İLİ", " İLÇESİ" ...). Son cümledeki köy / mahalle ve sokak seçimleri «YER» ve «SOKAKMETNI» alanlarıdır.
    ///   - İkametgah bilgisi alınamazsa eski kod ekrana mesaj basıp boş "ServisYok" şablonunu yazdırıyordu; mesaj kutusu
    ///     olmadığından yalnızca belge üretilir (eski çıktının aynısı).
    ///   - Şablon kurumun RaporSablonYol klasöründe yoksa uygulamayla gelen RaporSablon\KPM klasörüne bakılır.
    /// </summary>
    public class NufusYazdir
    {
        public string RaporAl(object o_kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            servisKPM = TNS.KPM.Arac.Tanimla();
            servisKYM = TNS.KYM.Arac.Tanimla();
            this.sablonAd = SablonBul(sablonAd);
            this.sonucDosyaAd = sonucDosyaAd + ".docx";
            kullanan = (Kullanici)o_kullanan;
            TNS.KPM.Istek istek = new TNS.KPM.Istek();
            istek.istekTur = OrtakFonksiyonlar.ConvertToInt(ekBilgi, 1);
            istek.tcKimlikNo = (string)raporParam["TCKimlikNo"];
            istek.islemYapan = kullanan.kullaniciKodu;
            istek.islemTarih.Yaz(DateTime.Now);

            txtDuzenleyenAdSoyad = (string)raporParam["DUZENLEYENAD"] ?? "";
            txtDuzenleyenUnvan = (string)raporParam["DUZENLEYENUNVAN"] ?? "";
            txtOnaylayanAdSoyad = (string)raporParam["ONAYLAYANAD"] ?? "";
            txtOnaylayanUnvan = (string)raporParam["ONAYLAYANUNVAN"] ?? "";

            AsposeLisans.Yukle();
            if (istek.istekTur == (int)ENUMIstekTur.IKAMETGAH)
                return IkametgahIlmuhaberiYazdir(istek);
            else if (istek.istekTur == (int)ENUMIstekTur.NUFUSCUZDANORNEK)
                return NufusKayitYazdir(istek);
            else if (istek.istekTur == (int)ENUMIstekTur.VUKUATLINUFUSKAYITORNEK)
                return VukuatliNufusKayitYazdir(istek);

            throw new Exception("Hatalı Parametre Geldi!");
        }

        //Nufus Kayit Ornegi Yazdirma
        private string NufusKayitYazdir(TNS.KPM.Istek istek)
        {
            ObjectArray nufusList = servisKPM.NufusKayitOrnekRaporla(kullanan, istek);

            TNS.KPM.Kisi nufusOrnek = new TNS.KPM.Kisi();

            if (!nufusList.sonuc.islemSonuc && nufusList.ObjeSayisi == 0)
            {
                throw new Exception(nufusList.sonuc.hataStr);
            }
            else if (nufusList.ObjeSayisi > 0)
                nufusOrnek = (TNS.KPM.Kisi)nufusList.objeler[0];
            else
            {
                throw new Exception(nufusList.sonuc.bilgiStr);
            }

            Alanlar a = new Alanlar();
            a.Ekle("SERI", nufusOrnek.nufusCuzdani.nufusCuzdanSeri);
            a.Ekle("NUMARA", nufusOrnek.nufusCuzdani.nufusCuzdanNo);
            a.Ekle("TCKIMLIKNO", istek.tcKimlikNo);
            a.Ekle("CINSIYET", nufusOrnek.cinsiyetAciklama);
            a.Ekle("SOYADI", nufusOrnek.soyad);
            a.Ekle("ADI", nufusOrnek.ad);
            a.Ekle("BABAADI", nufusOrnek.babaAd);
            a.Ekle("ANAADI", nufusOrnek.anaAd);
            a.Ekle("DOGUMYERI", nufusOrnek.dogumYeri);
            a.Ekle("DOGUMTARIHI", TarihYaz(nufusOrnek.dogumTarihi.Oku()));
            a.Ekle("MEDENIHALI", nufusOrnek.medeniHalAciklama);
            a.Ekle("DINI", "");
            a.Ekle("KANGRUBU", nufusOrnek.kanGrubu);
            a.Ekle("KIZLIKSOYADI", nufusOrnek.kizlikSoyad);
            a.Ekle("IL", nufusOrnek.kayitliOlduguYer.ilce.il.ad);
            a.Ekle("ILCE", nufusOrnek.kayitliOlduguYer.ilce.ad);
            a.Ekle("MAHALLEKOY", nufusOrnek.kayitliOlduguYer.mahalleKoy);
            a.Ekle("CILTNO", nufusOrnek.kayitliOlduguYer.ciltNo.ToString().PadLeft(4, '0'));
            a.Ekle("AILESIRA", nufusOrnek.kayitliOlduguYer.aileSiraNo.ToString().PadLeft(5, '0'));
            a.Ekle("BIREYSIRA", nufusOrnek.kayitliOlduguYer.bireySiraNo.ToString().PadLeft(4, '0'));
            a.Ekle("VERILDIGIYER", nufusOrnek.nufusCuzdani.verildigiYer);
            a.Ekle("VERILISNEDENI", nufusOrnek.nufusCuzdani.verilisNedeniAciklama);
            a.Ekle("KAYITNO", nufusOrnek.nufusCuzdani.kayitNo);
            a.Ekle("VERILISTARIHI", TarihYaz(nufusOrnek.nufusCuzdani.verilisTarihi.Oku()));

            a.Ekle("DUZENLEYENAD", txtDuzenleyenAdSoyad.Trim());
            a.Ekle("DUZENLEYENUNVAN", txtDuzenleyenUnvan.Trim());
            a.Ekle("ONAYLAYANAD", txtOnaylayanAdSoyad.Trim());
            a.Ekle("ONAYLAYANUNVAN", txtOnaylayanUnvan.Trim());

            return Yaz(this.sablonAd, a, null);
        }

        //İkametgah İl Muhaberi Yazdirma
        private string IkametgahIlmuhaberiYazdir(TNS.KPM.Istek istek)
        {
            ObjectArray ikametgahList = servisKPM.IkametgahRaporla(kullanan, istek);

            TNS.KPM.Kisi ikametgah = new TNS.KPM.Kisi();

            // Eski kod burada "İkametgah bilgisi alınamadı" mesajını basıp boş belgeyle devam ediyordu.
            if (ikametgahList.ObjeSayisi > 0)
                ikametgah = (TNS.KPM.Kisi)ikametgahList.objeler[0];

            bool servisVar = false;
            string sablon = this.sablonAd;
            if (ikametgah.adres != null && !string.IsNullOrWhiteSpace(ikametgah.adres.il))
                servisVar = true;
            else
                sablon = SablonBul(this.sablonAd.Replace("IkametgahIlmuhaberi.dotx", "IkametgahIlmuhaberiServisYok.dotx"));

            Alanlar a = new Alanlar();
            if (servisVar)
            {
                string bucak = "";
                try { bucak = ikametgah.adres.bucak; } catch { }

                SatirEkle(a, "ILI", ikametgah.adres.il, " İLİ");
                SatirEkle(a, "ILCESI", ikametgah.adres.ilce, " İLÇESİ");
                SatirEkle(a, "BUCAGI", bucak, " BUCAĞI");
                SatirEkle(a, "KOYU", ikametgah.adres.koy, " KÖYÜ");
                SatirEkle(a, "MAHALLESI", ikametgah.adres.mahalle, " MAHALLESİ");
                // \IF (KOYU<>'')\\KOYU\ köyü \ELSE\\MAHALLESI\ mahallesi \ENDIF\
                a.Ekle("YER", !string.IsNullOrEmpty(ikametgah.adres.koy) ? ikametgah.adres.koy + " köyü " : ikametgah.adres.mahalle + " mahallesi ");
                // \IF(SOKAK<>'')\\SOKAK\ sokağı/caddesi \ELSE\\ENDIF\
                a.Ekle("SOKAKMETNI", !string.IsNullOrEmpty(ikametgah.adres.sokak) ? ikametgah.adres.sokak + " sokağı/caddesi " : "");
                a.Ekle("SOKAK", ikametgah.adres.sokak);
                a.Ekle("NO", ikametgah.adres.numara);
                a.Ekle("DUZENLEYENAD", txtDuzenleyenAdSoyad.Trim());
                a.Ekle("DUZENLEYENUNVAN", txtDuzenleyenUnvan.Trim());
                a.Ekle("ONAYLAYANAD", txtOnaylayanAdSoyad.Trim());
                a.Ekle("ONAYLAYANUNVAN", txtOnaylayanUnvan.Trim());
            }
            else
            {
                a.Ekle("ADRES", ikametgah.adres != null ? ikametgah.adres.sokak : "");
            }

            a.Ekle("ADISOYADI", ikametgah.ad + " " + ikametgah.soyad);
            a.Ekle("BABAADI", ikametgah.babaAd);
            a.Ekle("ANNEADI", ikametgah.anaAd);
            a.Ekle("DOGUMYERI", ikametgah.dogumYeri);
            a.Ekle("DOGUMYILI", ikametgah.dogumTarihi.Yil.ToString());
            a.Ekle("MEDENIHALI", ikametgah.medeniHalAciklama);
            a.Ekle("ISI", "");

            return Yaz(sablon, a, null);
        }

        //Vukuatlı Nufus Kayit Ornegi Yazdirma
        private string VukuatliNufusKayitYazdir(TNS.KPM.Istek istek)
        {
            ObjectArray nufusList = servisKPM.VukuatliNufusKayitOrnekRaporla(kullanan, istek);

            TNS.KPM.Kisi nufusKisi = new TNS.KPM.Kisi();

            if (!nufusList.sonuc.islemSonuc)
            {
                throw new Exception(nufusList.sonuc.hataStr);
            }
            else if (nufusList.ObjeSayisi > 0)
                nufusKisi = (TNS.KPM.Kisi)nufusList.objeler[0];
            else
            {
                throw new Exception(nufusList.sonuc.bilgiStr);
            }

            Alanlar a = new Alanlar();
            a.Ekle("ILI", nufusKisi.kayitliOlduguYer.ilce.il.ad);
            a.Ekle("ILCESI", nufusKisi.kayitliOlduguYer.ilce.ad);
            a.Ekle("MAHALLE_KOY", nufusKisi.kayitliOlduguYer.mahalleKoy);
            a.Ekle("CILTNO", nufusKisi.kayitliOlduguYer.ciltNo.ToString());
            a.Ekle("ASIRANO", nufusKisi.kayitliOlduguYer.aileSiraNo.ToString());
            a.Ekle("SIRANO", nufusKisi.kayitliOlduguYer.bireySiraNo.ToString());

            // Eski kod aile fertlerini "vnko" tablosu olarak veriyordu; eski şablonda bu bölge yoktu (tablo boş basılıyordu).
            DataTable dtKisiBilgi = new DataTable("vnko");
            dtKisiBilgi.Columns.Add("sira");
            dtKisiBilgi.Columns.Add("yakinlikDerecesi");
            dtKisiBilgi.Columns.Add("tCKimlikNo");
            dtKisiBilgi.Columns.Add("adi");
            dtKisiBilgi.Columns.Add("soyadi");
            dtKisiBilgi.Columns.Add("babaAdi");
            dtKisiBilgi.Columns.Add("anaAdi");

            int index = 1;
            foreach (AileFerdi af in nufusKisi.aileFertleri.objeler)
            {
                dtKisiBilgi.Rows.Add(index.ToString(), "", af.tcKimlikNo, af.ad, af.soyad, af.babaAd, af.anaAd);
                index++;
            }

            return Yaz(this.sablonAd, a, dtKisiBilgi);
        }

        // ------------------------------------------------------------------ Aspose.Words

        /// <summary>Adres-mektup alan adları ve değerleri (eski EkRtf.CreateVar sırası).</summary>
        private class Alanlar
        {
            public readonly System.Collections.Generic.List<string> adlar = new System.Collections.Generic.List<string>();
            public readonly System.Collections.Generic.List<object> degerler = new System.Collections.Generic.List<object>();

            public void Ekle(string ad, string deger)
            {
                adlar.Add(ad);
                degerler.Add(deger ?? "");
            }
        }

        private static void SatirEkle(Alanlar a, string ad, string deger, string ek)
        {
            a.Ekle(ad, deger);
            a.Ekle(ad + "EK", string.IsNullOrEmpty(deger) ? "" : ek);
        }

        private string Yaz(string sablon, Alanlar a, DataTable bolge)
        {
            Aspose.Words.Document doc = new Aspose.Words.Document(sablon);
            doc.MailMerge.Execute(a.adlar.ToArray(), a.degerler.ToArray());
            if (bolge != null)
                doc.MailMerge.ExecuteWithRegions(bolge);
            doc.UpdateFields(); // belgedeki TIME (tarih) alanları
            doc.Save(sonucDosyaAd, Aspose.Words.SaveFormat.Docx);
            return sonucDosyaAd;
        }

        /// <summary>EkRtf CreateVar(ad, DateTime, true) karşılığı: gün.ay.yıl; boş tarih boş.</summary>
        private static string TarihYaz(DateTime t)
        {
            return t.Year < 1900 ? "" : t.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// RaporlamaServis'in verdiği şablon yolu (kurum RaporSablonYol + "KPM\ad") yoksa uygulamayla gelen RaporSablon\KPM\ad
        /// (çalışma klasörü = içerik kökü, sonra uygulama klasörü) denenir; hiçbiri yoksa gelen yol döner (Aspose hata verir).
        /// </summary>
        private static string SablonBul(string sablonAd)
        {
            if (string.IsNullOrWhiteSpace(sablonAd) || File.Exists(sablonAd)) return sablonAd;
            string ad = sablonAd.Replace('\\', '/');
            int i = ad.LastIndexOf("/KPM/", StringComparison.OrdinalIgnoreCase);
            string goreli = i >= 0 ? ad.Substring(i + 1) : "KPM/" + Path.GetFileName(ad);
            foreach (string kok in new string[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                string s = Path.Combine(kok, "RaporSablon", goreli.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(s)) return s;
            }
            return sablonAd;
        }

        public IKYMServis servisKYM { get; set; }
        public IKPMServis servisKPM { get; set; }
        public Kullanici kullanan { get; set; }
        public string sablonAd { get; set; }
        public string txtDuzenleyenAdSoyad { get; set; }
        public string txtDuzenleyenUnvan { get; set; }
        public string txtOnaylayanAdSoyad { get; set; }
        public string txtOnaylayanUnvan { get; set; }
        public string sonucDosyaAd { get; set; }
    }
}

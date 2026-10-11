// Static Model
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BUM.IsKurallari.Veri;
//using TNS.BHM;
using TNS.KYM;
using TNS.BKD;
using TNS.UZY;
using TNS.BHMv2;
using System;
using TNS.DEG;

namespace TNS.BUM.IsKurallari
{
    /// <summary>
    /// Bütçe İşlem Servisi Sınıfı
    /// </summary>
    public class ButceIslemServisi
    {
        /// <summary>
        /// Varsayılan Kurum Değişken tanımı
        /// </summary>
        private static string VarsayilanKurum;

        /// <summary>
        /// Bütçe İşlem Yılı değişken tanımı
        /// </summary>
        private static int ButceIslemYili;

        /// <summary>
        /// Parametre olarak verilen sayıyı, virgülle hale getiren fonksiyon
        /// </summary>
        /// <param name="rakam">Sayı bilgisini tutan, Double türünde nesne</param>
        /// <param name="virguldenSonra">Virgülden sonraki hane sayısını tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Virgüllenmiş sayı bilgisini tutan String türünde nesne döndürür</returns>
        private static string Virgulle(double rakam, int virguldenSonra)
        {
            string format = "#,##0";
            if (virguldenSonra > 0)
            {
                format += ".";
                format.PadRight(6 + virguldenSonra, '#');
            }
            return rakam.ToString(format);
        }

        /// <summary>
        /// Parametre olarak verilen Kurum kodu ve Ekonomik kod bilgilerine göre,
        /// Ayrıntılı Harcama Bilgisini döndüren fonksiyon
        /// </summary>
        /// <param name="kurumKod">Kurum kod bilgisini tutan, String türünde nesne</param>
        /// <param name="ekoKod">Ekonomik kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonrasında, Ayrıntılı Harcama bilgisini tutan SerbestBilgiSatir türünde nesne döndürür</returns>
        private static SerbestBilgiSatir AHPListedeBul(string kurumKod, string ekoKod)
        {
            for (int i = 0; i < AHPSerbestBilgi.Count; i++)
            {
                SerbestBilgiSatir sbs = (SerbestBilgiSatir)AHPSerbestBilgi[i];
                if (sbs.kurumKod.Equals(kurumKod) && sbs.ekoKod.Equals(ekoKod))
                    return sbs;
            }
            return null;
        }

        /// <summary>
        /// Parametre olarak verilen kod bilgisine göre,
        /// kodun son kırılımını silen fonksiyon
        /// </summary>
        /// <param name="kod">Kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, son kırılımı silinmiş kod bilgisini tutan String türünde nesne döndürür</returns>
        private static string SonKirilimSil(string kod)
        {
            while (kod.Length > 0)
            {
                int uzunluk = kod.Length - 1;
                if (kod[uzunluk] == '.')
                {
                    kod = kod.Remove(uzunluk);
                    return kod;
                }
                else
                    kod = kod.Remove(uzunluk);
            }
            return "";
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum Kodu ve Ekonomik Kod bilgilerine göre,
        /// Ayrıntılı Harcama Planının Durumunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurKod">Kurum kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="ekonomikKod">Ekonomik kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, AHP Durum bilgisini tutan Int türünde nesne döndürür</returns>
        private static int AHPDurum(int yil, string kurKod, string ekonomikKod)
        {
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            string kurumKod = TNS.BKD.Arac.KurumsaldanKuruma(kurKod, tUzBul.Kirilimlar(0));
            if (!TNS.BKD.Arac.KurumMu(kurumKod))
                throw new System.Exception(ButceUygulamaResI.BUM052 + " (" + kurKod + ") " + ButceUygulamaResI.BUM053);

            if (yil < ButceIslemYili)
                return 12;
            else if (yil > ButceIslemYili)
                return 0;

            string tut = ekonomikKod;
            while (true)
            {
                SerbestBilgiSatir sbs = AHPListedeBul(kurumKod, tut);
                if (sbs != null)
                    return sbs.ay;
                if (string.IsNullOrEmpty(tut))
                    break;
                tut = SonKirilimSil(tut);
            }

            tut = ekonomikKod;
            while (true)
            {
                SerbestBilgiSatir sbs = AHPListedeBul("", tut);
                if (sbs != null)
                    return sbs.ay;
                if (string.IsNullOrEmpty(tut))
                    break;
                tut = SonKirilimSil(tut);
            }
            return 0;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu ve Sıra No Aşama bilgilerine göre,
        /// Bütçe İşleminin onaylı olup olmadığı onaylı ise hangi form grubu tarafından onaylandığını dönen fonksiyon.
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo_asama">Sıra No Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşleminin Onaylı olup olmadığı onaylı ise hangi form rubu tarafından onaylandığı bilgisini tutan ENUMFormDurumGrup nesnesini döndürür</returns>
        public static ENUMFormDurumGrup ButceIslemiOnayliMi(VeriErisim veriErisim, int yil, string kurum, string islemKod, int siraNo_asama)
        {
            ENUMFormDurumGrup formDurumGrup;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            TABISLForm tabForm = new TABISLForm(ve);
            int onayKod = tabForm.ButceIslemiOnayKoduBul(yil, kurum, islemKod, siraNo_asama);
            if (onayKod == (int)ENUMFormDurum.YENIKAYIT)
                formDurumGrup = ENUMFormDurumGrup.YENI;
            else if (onayKod >= (int)ENUMFormDurum.DYDOGRU && onayKod <= (int)ENUMFormDurum.ONAYLI)
                formDurumGrup = ENUMFormDurumGrup.ONAYLI;
            else if (onayKod >= (int)ENUMFormDurum.SAYISTAYRED && onayKod <= (int)ENUMFormDurum.SILINDI)
                formDurumGrup = ENUMFormDurumGrup.IPTALLI;
            else
                formDurumGrup = ENUMFormDurumGrup.HATALI;
            return formDurumGrup;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, İşlem Türü, Kodlar bilgilerini kullanarak,
        /// Eklenen ve Düşülen bilgilerini döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemTur">İşlem türü bilgisini tutan, String türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal ve Ekonomik Kod bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="durum">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Eklenen ve Düşülen bilgilerini tutan BITertipBilgi türünde nesne döndürür</returns>
        public static BITertipBilgi EklenenDusulenBul(int yil, string islemTur, string[] kodlar, ENUMFormDurumGrup durum)
        {
            BITertipBilgi tBilgi = new BITertipBilgi();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            TABISLGider islGider = new TABISLGider(ve);
            islGider.EklenenDusulenBul(yil, islemTur, kodlar, durum, out tBilgi.eklenen, out tBilgi.dusulen);
            ve.VeriErisimKapat();

            return tBilgi;
        }

        /// <summary>
        /// Parametre olarak verilen Reader'dan, Bilgileri okuyarak,
        /// ABSKodlar dizisine ekleyen fonksiyon
        /// </summary>
        /// <param name="p_yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="r">Reader bilgisini tutan, IDataReader türünde nesne</param>
        /// <param name="absKodlar">ABSKodlar dizisini tutan, ObjectArray türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="uzyServis">Uzay Servisi bilgisini tutan, IUZYServis türünde nesne</param>
        private static void ReaderDoldur(int p_yil, IDataReader r, ObjectArray absKodlar, Tertip tertip, TNS.UZY.IUZYServis uzyServis)
        {
            while (r.Read())
            {
                Tertip icTertip = new Tertip(p_yil, tertip.tabloKod);
                icTertip.TertipAta(r.GetString(0));
                icTertip.Noktala();
                icTertip.AdlariBul(uzyServis);
                absKodlar.Ekle(icTertip);
            }
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Tertip bilgilerine göre,
        /// Tertip listesini döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="hiyerarsik">Hiyerarşiye göre Tertip listesinin gelip gelmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Tertip listesini tutan ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray TertipListesiBul(VeriErisim veriErisim, int yil, Tertip tertip, bool hiyerarsik)
        {

            ObjectArray akv = new ObjectArray();
            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && string.IsNullOrEmpty(tertip.kodlar[0])))
            {
                akv.sonuc.islemSonuc = false;
                akv.sonuc.hataStr = ButceUygulamaResI.BUM054;
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
                TABButce to = GenelKosul.TabButceOl(tertip.tabloKod, ve);

                IDataReader r = to.TertipListesiBul(yil, tertip.kodlar, "", (string)tertip.Kirilimlar(0), hiyerarsik);
                if (ve.hata == "")
                {
                    ReaderDoldur(yil, r, akv, tertip, BUMServis.TanimlaUZY());
                    r.Close();
                    akv.sonuc.islemSonuc = true;
                }
                else
                {
                    akv.sonuc.islemSonuc = false;
                    akv.sonuc.hataStr = ve.hata;
                }
                ve.VeriErisimKapat();
            }
            return akv;
        }

        /// <summary>
        /// Parametre olarak verilen DataReader'dan verileri okuyarak,
        /// Bütçe İşlem Form nesnesine dolduran fonksiyon
        /// </summary>
        /// <param name="dr">DataReader nesnesini tutan, IDataReader türünde nesne</param>
        /// <param name="biForm">Bütçe İşlem Form bilgilerini tutan, BIForm türünde nesne</param>
        private static void BIFormOl(IDataReader dr, BIForm biForm)
        {
            biForm.yil = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.YIL, 0);
            biForm.kurum = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.KURUMKOD, ""));
            biForm.islemKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.ISLEM, ""));
            biForm.siraNo = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.SIRANO, 0);
            biForm.evrakTarih = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.TARIH);
            biForm.evrakNo = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.YAZINO, "");
            biForm.ilgi = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.ILGI, "");
            biForm.notlar = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.NOTLAR, "");
            biForm.formDurum = new BIFormDurum(VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.FORMDURUM, 0));
            biForm.kullaniciTipKod = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.ISLEMYAPANKTKOD, 0);
            biForm.kullaniciKod = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.ISLEMYAPANKULKOD, "");
            biForm.degisiklikTarih = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.DEGISIKLIKTARIH);
            biForm.onayTarih = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.ONAYTARIH);

            biForm.ilgiliIslemKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.ILGILIISLEM, ""));
            biForm.ilgiliSiraNo = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.ILGILISIRANO, 0);

            biForm.bumkoKurum = OrtakFonksiyonlar.KodNoktaKoy("99.99", VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.BUMKOKURUMKOD, ""));
            biForm.bumkoSiraNo = VeriErisim.DegerAl(dr, (int)TABISLForm.ENUMAlanlar.BUMKOSIRANO, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No ve Dayanak Listesi bilgilerine göre,
        /// Eğer Dayanak bilgileri var ise Dayanak Listesi bilgilerini DataReader nesnesine dolduran ve true dönen,
        /// yok ise false döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="dayanakListesi">Dayanak listesi bilgilerini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Dayanak listesi bilgileri var ise true yoksa false değerini tutan Bool türünde nesne döndürür</returns>
        private static bool DayanaklariBul(VeriErisim veriErisim, int yil, string kurum, string islemKod, int siraNo, TNSCollection dayanakListesi)
        {
            TABISLFormDayanak formDayanak = new TABISLFormDayanak(veriErisim);
            IDataReader dr = formDayanak.Listele(yil, kurum, islemKod, siraNo);
            if (dr != null)
            {
                while (dr.Read())
                {
                    BIDayanak d = new BIDayanak();
                    d.dayanakKod = VeriErisim.DegerAl(dr, 1, 0);
                    d.dayanakAd = VeriErisim.DegerAl(dr, 2, "");
                    dayanakListesi.Add(d);
                }
                dr.Close();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kodlar bilgisini kullanarak,
        /// Bütçe Tablosunda BilgiBul rutinini çalıştırarak değerleri döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kodlar bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="onayli">Onaylı miktarını tutan, Double türünde nesne</param>
        /// <param name="onaysiz">Onaysız miktarını tutan, Double türünde nesne</param>
        private static void SerbestBul(VeriErisim veriErisim, int yil, string[] kodlar, out double onayli, out double onaysiz)
        {
            double gecici;
            if (kodlar.Length < 5)
            {
                TABButce butce = new TABButceGider(veriErisim);
                butce.BilgiBul(yil, kodlar, out gecici, out gecici, out gecici, out gecici, out onayli, out gecici, out gecici, out onaysiz);
            }
            else
            {
                onayli = 0; onaysiz = 0;//???
            }
        }

        /// <summary>
        /// Muhasebe servisi değişken tanımı
        /// </summary>
        private static TNS.MUH.IMUHServis muhServis;

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// Muhasebe servisine bağlanan ve kayıtlara ulaşılmışsa Onaylı, Onaysız ve Onaylı Tenkis ve Onaysız Tenkis miktarlarını döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal ve Ekonomik Kod bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="onayli">Onaylı bilgisini tutan, Double türünde nesne</param>
        /// <param name="onaysiz">Onaysız bilgisini tutan, Double türünde nesne</param>
        /// <param name="onayliT">Onaylı tenkis bilgisini tutan, Double türünde nesne</param>
        /// <param name="onaysizT">Onaysız tenkis bilgisini tutan, Double türünde nesne</param>
        private static void GonderilenBul(VeriErisim veriErisim, int yil, string[] kodlar, out double onayli, out double onaysiz, out double onayliT, out double onaysizT)
        {
            onayli = 0;
            onaysiz = 0;
            onayliT = 0;
            onaysizT = 0;
            TNS.MUH.OdenekDurumKriter odk = new TNS.MUH.OdenekDurumKriter();
            odk.ay = 0;
            odk.yil = yil;
            odk.birim = "";
            odk.ekoKodKirilim = 2;
            odk.konsolide = true;
            odk.tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kodlar[0], kodlar[1], kodlar[2], kodlar[3], kodlar[4]);
            odk.odenekGoster = false;
            odk.harcamaGoster = false;
            odk.mifGoster = false;
            odk.odemeEmriGoster = false;
            odk.odenekGondermeGoster = false;
            odk.tenkisGoster = false;

            odk.odenekGondermeGoster = true;
            ObjectArray odv;
            try
            {
                odv = muhServis.OdenekDurum(odk);
            }
            catch (Exception ex)
            {
                odv = new ObjectArray();
                odv.sonuc = new Sonuc(false, ex.Message, "");
            }

            if (odv.sonuc.islemSonuc)
            {
                TNS.MUH.OdenekDurum o = (TNS.MUH.OdenekDurum)odv.objeler[0];
                onayli = o.onayliOdenekGonderme;
                onaysiz = o.onaysizOdenekGonderme;
                onayliT = o.onayliTenkis;
                onaysizT = o.onaysizTenkis;
            }
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// Muhasebe servisine bağlanan ve kayıtlara ulaşılmışsa Onaylı ve Onaysız miktarları döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal ve Ekonomik Kod bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="onayli">Onaylı miktar bilgisini tutan, Double türünde nesne</param>
        /// <param name="onaysiz">Onaysız miktar bilgisini tutan, Double türünde nesne</param>
        private static void HarcananBul(VeriErisim veriErisim, int yil, string[] kodlar, out double onayli, out double onaysiz)
        {
            onayli = 0;
            onaysiz = 0;
            TNS.MUH.OdenekDurumKriter odk = new TNS.MUH.OdenekDurumKriter();
            odk.ay = 0;
            odk.yil = yil;
            odk.birim = "";
            odk.ekoKodKirilim = 2;
            odk.konsolide = true;
            odk.tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kodlar[0], kodlar[1], kodlar[2], kodlar[3], kodlar[4]);

            odk.odenekGoster = false;
            odk.harcamaGoster = false;
            odk.mifGoster = false;
            odk.odemeEmriGoster = false;
            odk.odenekGondermeGoster = false;
            odk.tenkisGoster = false;

            odk.harcamaGoster = true;

            ObjectArray odv;
            try
            {
                odv = muhServis.OdenekDurum(odk);
            }
            catch (Exception ex)
            {
                odv = new ObjectArray();
                odv.sonuc = new Sonuc(false, ex.Message, "");
            }

            if (odv.sonuc.islemSonuc)
            {
                TNS.MUH.OdenekDurum o = (TNS.MUH.OdenekDurum)odv.objeler[0];
                onayli = o.harcama;
                onaysiz = 0;
            }
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgilerine göre,
        /// Onaylı ve Onaysız tertip bilgisini EklenenDusulenBul, SerbestBul, GonderilenBul, HarcananBul rutinlerini çalıştırarak döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal ve Ekonomik Kod bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="tertipBilgiOnayli">Onaylı tertip bilgisini tutan, BITertipBilgi türünde nesne</param>
        /// <param name="tertipBilgiOnaysiz">Onaysız tertip bilgisini tutan, BITertipBilgi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, true değerini tutan Bool türünde nesne döndürür</returns>
        private static bool GiderTertipBilgiBul(VeriErisim veriErisim, int yil, string[] kodlar, BITertipBilgi tertipBilgiOnayli, BITertipBilgi tertipBilgiOnaysiz)
        {
            TABISLAPGider islGider = new TABISLAPGider(veriErisim);
            islGider.EklenenDusulenBul(yil, "02", kodlar, ENUMFormDurumGrup.ONAYLI, out tertipBilgiOnayli.eklenen, out tertipBilgiOnayli.dusulen);
            islGider.EklenenDusulenBul(yil, "02", kodlar, ENUMFormDurumGrup.YENI, out tertipBilgiOnaysiz.eklenen, out tertipBilgiOnaysiz.dusulen);

            SerbestBul(veriErisim, yil, kodlar, out tertipBilgiOnayli.serbest, out tertipBilgiOnaysiz.serbest);
            GonderilenBul(veriErisim, yil, kodlar, out tertipBilgiOnayli.gonderilen, out tertipBilgiOnaysiz.gonderilen, out tertipBilgiOnayli.tenkisEdilen, out tertipBilgiOnaysiz.tenkisEdilen);
            HarcananBul(veriErisim, yil, kodlar, out tertipBilgiOnayli.harcanan, out tertipBilgiOnaysiz.harcanan);
            return true;
        }

        private static bool FormDetayGiderIcmalBulAP(VeriErisim veriErisim, int yil, string kurum, string islemKod, int siraNo, TNSCollection tertipListesi, bool detayli)
        {
            Tertip tertip = new Tertip(yil, ENUMTabloKod.TABGiderIcmal);
            TABISLIHP islDetayIcmalAP = new TABISLIHP(veriErisim);
            IDataReader dr = islDetayIcmalAP.Listele(yil, kurum, islemKod, siraNo);
            if (dr != null)
            {
                string oncekiTertip = "", simdTertip;
                BISatir formDetay = null;
                while (dr.Read())
                {
                    string ekoKod = OrtakFonksiyonlar.KodNoktaKoy(tertip.Kirilimlar(1), VeriErisim.DegerAl(dr, 0, ""));
                    simdTertip = ekoKod;
                    if (simdTertip != oncekiTertip || formDetay == null)
                    {
                        if (detayli)
                            formDetay = new BISatirDetayli();
                        else
                            formDetay = new BISatir();
                        formDetay.tertip = new Tertip(yil, ENUMTabloKod.TABGiderIcmal, kurum, ekoKod);
                        formDetay.miktar = 0;
                        formDetay.yildizYer = 0;
                        formDetay.aylikProgram = new double[12];
                        for (int ay = 0; ay < 12; ay++)
                            formDetay.aylikProgram[ay] = 0;
                        tertipListesi.Add(formDetay);
                        oncekiTertip = simdTertip;
                    }
                    int ay1 = VeriErisim.DegerAl(dr, 1, 0);
                    if (ay1 >= 1 && ay1 <= 12)
                        formDetay.aylikProgram[ay1 - 1] = VeriErisim.DegerAl(dr, 2, (double)0);
                }
                dr.Close();
                if (detayli)
                {
                    TABButce butce = new TABButceGider(veriErisim);
                    for (int i = 0; i < tertipListesi.Count; i++)
                    {
                        BISatirDetayli formSatirDetay = (BISatirDetayli)tertipListesi[i];

                        string[] kodlar = new string[4];
                        kodlar[0] = formSatirDetay.tertip.kodlar[0];
                        kodlar[1] = "";
                        kodlar[2] = "";
                        kodlar[3] = formSatirDetay.tertip.kodlar[1];

                        formSatirDetay.kbo = butce.KBOBul(yil, kodlar);
                        formSatirDetay.tertipBilgiOnayli = new BITertipBilgi();
                        formSatirDetay.tertipBilgiOnaysiz = new BITertipBilgi();
                        if (!GiderTertipBilgiBul(veriErisim, yil, kodlar, formSatirDetay.tertipBilgiOnayli, formSatirDetay.tertipBilgiOnaysiz))
                            return false;
                    }
                }
                return true;
            }
            else
                return false;
        }

        private static bool FormDetayGiderBulAP(VeriErisim veriErisim, int yil, string kurum, string islemKod, int siraNo, TNSCollection tertipListesi, bool detayli)
        {
            TABISLAPGider islDetayAP = new TABISLAPGider(veriErisim);
            IDataReader dr = islDetayAP.Listele(yil, kurum, islemKod, siraNo);
            if (dr != null)
            {
                string oncekiTertipStr = "", simdTertipStr;
                BISatir formDetay = null;
                int detayBasla = tertipListesi.Count;
                while (dr.Read())
                {
                    string kurKod = VeriErisim.DegerAl(dr, 0, "");
                    string fonKod = VeriErisim.DegerAl(dr, 1, "");
                    string finKod = VeriErisim.DegerAl(dr, 2, "");
                    string ekoKod = VeriErisim.DegerAl(dr, 3, "");
                    string pebKod = VeriErisim.DegerAl(dr, 4, "");
                    simdTertipStr = kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod;
                    if (simdTertipStr != oncekiTertipStr || formDetay == null)
                    {
                        if (detayli)
                            formDetay = new BISatirDetayli();
                        else
                            formDetay = new BISatir();
                        formDetay.tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurKod, fonKod, finKod, ekoKod, pebKod);
                        formDetay.tertip.Noktala();
                        formDetay.miktar = 0;
                        formDetay.yildizYer = 0;
                        formDetay.aylikProgram = new double[12];
                        /*                        for (int ay = 0; ay < 12; ay++)
                                                    formDetay.aylikProgram.Add((double)0);*/
                        tertipListesi.Add(formDetay);
                        oncekiTertipStr = simdTertipStr;
                    }
                    int ay1 = VeriErisim.DegerAl(dr, 5, 0);
                    if (ay1 >= 1 && ay1 <= 12)
                        formDetay.aylikProgram[ay1 - 1] = VeriErisim.DegerAl(dr, 6, (double)0);
                }
                dr.Close();
                if (detayli)
                {
                    TABButce butce = new TABButceGider(veriErisim);
                    for (int i = detayBasla; i < tertipListesi.Count; i++)
                    {
                        BISatirDetayli formSatirDetay = (BISatirDetayli)tertipListesi[i];
                        formSatirDetay.kbo = butce.KBOBul(yil, formSatirDetay.tertip.kodlar);
                        formSatirDetay.tertipBilgiOnayli = new BITertipBilgi();
                        formSatirDetay.tertipBilgiOnaysiz = new BITertipBilgi();
                        if (!GiderTertipBilgiBul(veriErisim, yil, formSatirDetay.tertip.kodlar, formSatirDetay.tertipBilgiOnayli, formSatirDetay.tertipBilgiOnaysiz))
                            return false;
                    }
                }
                return true;
            }
            else
                return false;
        }

        //private static bool GelirTertipBilgiBul(VeriErisim veriErisim, int yil, string[] kodlar, BITertipBilgi tertipBilgiOnayli, BITertipBilgi tertipBilgiOnaysiz)
        //{
        //    TABISLGelir islGelir = new TABISLGelir(veriErisim);
        //    islGelir.EklenenDusulenBul(yil, "02", kodlar, ENUMFormDurumGrup.ONAYLI, out tertipBilgiOnayli.eklenen, out tertipBilgiOnayli.dusulen);
        //    islGelir.EklenenDusulenBul(yil, "02", kodlar, ENUMFormDurumGrup.YENI, out tertipBilgiOnaysiz.eklenen, out tertipBilgiOnaysiz.dusulen);

        //    tertipBilgiOnayli.serbest = 0;
        //    tertipBilgiOnaysiz.serbest = 0;
        //    tertipBilgiOnayli.gonderilen = 0;
        //    tertipBilgiOnaysiz.gonderilen = 0;
        //    tertipBilgiOnayli.tenkisEdilen = 0;
        //    tertipBilgiOnaysiz.tenkisEdilen = 0;
        //    tertipBilgiOnayli.harcanan = 0;
        //    tertipBilgiOnaysiz.harcanan = 0;

        //    return true;
        //}

        //private static bool FormDetayGelirBulAP(VeriErisim veriErisim, int yil, string kurum, string islemKod, int siraNo, TNSCollection tertipListesi, bool detayli)
        //{
        //    TABISLAPGelir ap = new TABISLAPGelir(veriErisim);
        //    IDataReader dr = ap.Listele(yil, kurum, islemKod, siraNo);
        //    if (dr != null)
        //    {
        //        string oncekiTertipStr = "", simdTertipStr;
        //        BISatir formDetay = null;
        //        while (dr.Read())
        //        {
        //            string kurKod = VeriErisim.DegerAl(dr, 0, "");
        //            string ekoKod = VeriErisim.DegerAl(dr, 1, "");
        //            simdTertipStr = kurKod + "-" + ekoKod;
        //            if (simdTertipStr != oncekiTertipStr || formDetay == null)
        //            {
        //                if (detayli)
        //                    formDetay = new BISatirDetayli();
        //                else
        //                    formDetay = new BISatir();
        //                formDetay.tertip = new Tertip(yil, ENUMTabloKod.TABGelir, kurKod, ekoKod);
        //                formDetay.tertip.Noktala();
        //                formDetay.miktar = 0;
        //                formDetay.yildizYer = 0;
        //                formDetay.aylikProgram = new double[12];
        //                tertipListesi.Add(formDetay);
        //                oncekiTertipStr = simdTertipStr;
        //            }
        //            int ay1 = VeriErisim.DegerAl(dr, 2, 0);
        //            if (ay1 >= 1 && ay1 <= 12)
        //                formDetay.aylikProgram[ay1 - 1] = VeriErisim.DegerAl(dr, 3, (double)0);
        //        }
        //        dr.Close();
        //        if (detayli)
        //        {
        //            TABButceGelir butce = new TABButceGelir(veriErisim);
        //            for (int i = 0; i < tertipListesi.Count; i++)
        //            {
        //                BISatirDetayli formSatirDetay = (BISatirDetayli)tertipListesi[i];
        //                formSatirDetay.kbo = butce.KBOBul(yil, formSatirDetay.tertip.kodlar);
        //                formSatirDetay.tertipBilgiOnayli = new BITertipBilgi();
        //                formSatirDetay.tertipBilgiOnaysiz = new BITertipBilgi();
        //                if (!GelirTertipBilgiBul(veriErisim, yil, formSatirDetay.tertip.kodlar, formSatirDetay.tertipBilgiOnayli, formSatirDetay.tertipBilgiOnaysiz))
        //                    return false;
        //            }
        //        }
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        //private static bool FinansmanTertipBilgiBul(VeriErisim veriErisim, int yil, string[] kodlar, BITertipBilgi tertipBilgiOnayli, BITertipBilgi tertipBilgiOnaysiz)
        //{
        //    TABISLFinansman islFinansman = new TABISLFinansman(veriErisim);
        //    islFinansman.EklenenDusulenBul(yil, "02", kodlar, ENUMFormDurumGrup.ONAYLI, out tertipBilgiOnayli.eklenen, out tertipBilgiOnayli.dusulen);
        //    islFinansman.EklenenDusulenBul(yil, "02", kodlar, ENUMFormDurumGrup.YENI, out tertipBilgiOnaysiz.eklenen, out tertipBilgiOnaysiz.dusulen);

        //    tertipBilgiOnayli.serbest = 0;
        //    tertipBilgiOnaysiz.serbest = 0;
        //    tertipBilgiOnayli.gonderilen = 0;
        //    tertipBilgiOnaysiz.gonderilen = 0;
        //    tertipBilgiOnayli.tenkisEdilen = 0;
        //    tertipBilgiOnaysiz.tenkisEdilen = 0;
        //    tertipBilgiOnayli.harcanan = 0;
        //    tertipBilgiOnaysiz.harcanan = 0;

        //    return true;
        //}

        //private static bool FormDetayFinansBulAP(VeriErisim veriErisim, int yil, string kurum, string islemKod, int siraNo, TNSCollection tertipListesi, bool detayli)
        //{
        //    Tertip tertip = new Tertip(yil, ENUMTabloKod.TABFinansman);
        //    TABISLAPFinansman ap = new TABISLAPFinansman(veriErisim);
        //    IDataReader dr = ap.Listele(yil, kurum, islemKod, siraNo);
        //    if (dr != null)
        //    {
        //        string oncekiTertipStr = "", simdTertipStr;
        //        BISatir formDetay = null;
        //        while (dr.Read())
        //        {
        //            string kurKod = VeriErisim.DegerAl(dr, 0, "");
        //            string ekoKod = VeriErisim.DegerAl(dr, 1, "");
        //            simdTertipStr = kurKod + "-" + ekoKod;
        //            if (simdTertipStr != oncekiTertipStr || formDetay == null)
        //            {
        //                if (detayli)
        //                    formDetay = new BISatirDetayli();
        //                else
        //                    formDetay = new BISatir();
        //                formDetay.tertip = new Tertip(yil, ENUMTabloKod.TABFinansman, kurKod, ekoKod);
        //                formDetay.tertip.Noktala();
        //                formDetay.miktar = 0;
        //                formDetay.yildizYer = 0;
        //                formDetay.aylikProgram = new double[12];
        //                tertipListesi.Add(formDetay);
        //                oncekiTertipStr = simdTertipStr;
        //            }
        //            int ay1 = VeriErisim.DegerAl(dr, 2, 0);
        //            if (ay1 >= 1 && ay1 <= 12)
        //                formDetay.aylikProgram[ay1 - 1] = VeriErisim.DegerAl(dr, 3, (double)0);
        //        }
        //        dr.Close();
        //        if (detayli)
        //        {
        //            TABButceFinansman butce = new TABButceFinansman(veriErisim);
        //            for (int i = 0; i < tertipListesi.Count; i++)
        //            {
        //                BISatirDetayli formSatirDetay = (BISatirDetayli)tertipListesi[i];
        //                formSatirDetay.kbo = butce.KBOBul(yil, formSatirDetay.tertip.kodlar);
        //                formSatirDetay.tertipBilgiOnayli = new BITertipBilgi();
        //                formSatirDetay.tertipBilgiOnaysiz = new BITertipBilgi();
        //                if (!FinansmanTertipBilgiBul(veriErisim, yil, formSatirDetay.tertip.kodlar, formSatirDetay.tertipBilgiOnayli, formSatirDetay.tertipBilgiOnaysiz))
        //                    return false;
        //            }
        //        }
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        private static bool FormDetayGiderBul(VeriErisim veriErisim, int yil, string kurum, string islemKod, int siraNo, TNSCollection tertipListesi, bool detayli)
        {
            TABISLGider islDetay = new TABISLGider(veriErisim);
            IDataReader dr = islDetay.Listele(yil, kurum, islemKod, siraNo);
            if (dr != null)
            {
                while (dr.Read())
                {
                    BISatir formDetay;
                    if (detayli)
                        formDetay = new BISatirDetayli();
                    else
                        formDetay = new BISatir();
                    formDetay.tertip = new Tertip(yil, ENUMTabloKod.TABButce, VeriErisim.DegerAl(dr, 1, ""), VeriErisim.DegerAl(dr, 2, ""), VeriErisim.DegerAl(dr, 3, ""), VeriErisim.DegerAl(dr, 4, ""));
                    formDetay.tertip.Noktala();
                    formDetay.miktar = VeriErisim.DegerAl(dr, 5, (double)0);
                    formDetay.yildizYer = VeriErisim.DegerAl(dr, 6, (int)0);
                    tertipListesi.Add(formDetay);
                }
                dr.Close();

                if (BIForm.DevirIslemiMi(islemKod))
                {
                    TABISLDevir devir = new TABISLDevir(veriErisim);
                    for (int i = 0; i < tertipListesi.Count; i++)
                    {
                        BISatir formDetay = (BISatir)tertipListesi[i];
                        string devrolan = devir.Listele(yil, kurum, islemKod, siraNo, formDetay.tertip.kodlar);
                        string[] devirKirilim = devrolan.Split('-');
                        formDetay.devredenTertip = new Tertip(yil, ENUMTabloKod.TABButce, devirKirilim);
                    }
                }

                if (detayli)
                {
                    TABButce butce = new TABButceGider(veriErisim);
                    for (int i = 0; i < tertipListesi.Count; i++)
                    {
                        BISatirDetayli formDetay = (BISatirDetayli)tertipListesi[i];
                        formDetay.kbo = butce.KBOBul(yil, formDetay.tertip.kodlar);
                        formDetay.tertipBilgiOnayli = new BITertipBilgi();
                        formDetay.tertipBilgiOnaysiz = new BITertipBilgi();
                        if (!GiderTertipBilgiBul(veriErisim, yil, formDetay.tertip.kodlar, formDetay.tertipBilgiOnayli, formDetay.tertipBilgiOnaysiz))
                            return false;
                    }
                }
                return true;
            }
            else
                return false;
        }

        //private static bool FormDetayGelirBul(VeriErisim veriErisim, int yil, string kurum, string islemKod, int siraNo, TNSCollection tertipListesi, bool detayli)
        //{
        //    TABISLGelir islDetay = new TABISLGelir(veriErisim);
        //    IDataReader dr = islDetay.Listele(yil, kurum, islemKod, siraNo);
        //    if (dr != null)
        //    {
        //        while (dr.Read())
        //        {
        //            BISatir formDetay;
        //            if (detayli)
        //                formDetay = new BISatirDetayli();
        //            else
        //                formDetay = new BISatir();
        //            formDetay.tertip = new Tertip(yil, ENUMTabloKod.TABGelir, VeriErisim.DegerAl(dr, 1, ""), VeriErisim.DegerAl(dr, 2, ""));
        //            formDetay.tertip.Noktala();
        //            formDetay.miktar = VeriErisim.DegerAl(dr, 3, (double)0);
        //            formDetay.yildizYer = VeriErisim.DegerAl(dr, 4, (int)0);
        //            tertipListesi.Add(formDetay);
        //        }
        //        dr.Close();
        //        if (detayli)
        //        {
        //            TABButce butce = new TABButceGelir(veriErisim);
        //            for (int i = 0; i < tertipListesi.Count; i++)
        //            {
        //                BISatirDetayli formDetay = (BISatirDetayli)tertipListesi[i];
        //                formDetay.kbo = butce.KBOBul(yil, formDetay.tertip.kodlar);
        //                formDetay.tertipBilgiOnayli = new BITertipBilgi();
        //                formDetay.tertipBilgiOnaysiz = new BITertipBilgi();
        //                if (!GelirTertipBilgiBul(veriErisim, yil, formDetay.tertip.kodlar, formDetay.tertipBilgiOnayli, formDetay.tertipBilgiOnaysiz))
        //                    return false;
        //            }
        //        }
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        //private static bool FormDetayFinansBul(VeriErisim veriErisim, int yil, string kurum, string islemKod, int siraNo, TNSCollection tertipListesi, bool detayli)
        //{
        //    TABISLFinansman islDetay = new TABISLFinansman(veriErisim);
        //    IDataReader dr = islDetay.Listele(yil, kurum, islemKod, siraNo);
        //    if (dr != null)
        //    {
        //        while (dr.Read())
        //        {
        //            BISatir formDetay;
        //            if (detayli)
        //                formDetay = new BISatirDetayli();
        //            else
        //                formDetay = new BISatir();
        //            formDetay.tertip = new Tertip(yil, ENUMTabloKod.TABFinansman, VeriErisim.DegerAl(dr, 1, ""), VeriErisim.DegerAl(dr, 2, ""));
        //            formDetay.tertip.Noktala();

        //            formDetay.miktar = VeriErisim.DegerAl(dr, 3, (double)0);
        //            formDetay.yildizYer = VeriErisim.DegerAl(dr, 4, (int)0);
        //            tertipListesi.Add(formDetay);
        //        }
        //        dr.Close();
        //        if (detayli)
        //        {
        //            TABButce butce = new TABButceFinansman(veriErisim);
        //            for (int i = 0; i < tertipListesi.Count; i++)
        //            {
        //                BISatirDetayli formDetay = (BISatirDetayli)tertipListesi[i];
        //                formDetay.kbo = butce.KBOBul(yil, formDetay.tertip.kodlar);
        //                formDetay.tertipBilgiOnayli = new BITertipBilgi();
        //                formDetay.tertipBilgiOnaysiz = new BITertipBilgi();
        //                if (!FinansmanTertipBilgiBul(veriErisim, yil, formDetay.tertip.kodlar, formDetay.tertipBilgiOnayli, formDetay.tertipBilgiOnaysiz))
        //                    return false;
        //            }
        //        }
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Bütçe İşlem Formu bilgilerine göre,
        /// Bütçe İşlem bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="form">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="sadeceOzet">Özet parametresine göre listeleme yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray BIListele(VeriErisim veriErisim, Kullanici kullanan, BIForm form, bool sadeceOzet)
        {
            muhServis = BUMServis.TanimlaMUH();
            ObjectArray donusDeger = new ObjectArray();
            int yil = form.yil;
            string kurum = form.kurum;
            /*
                        if (!sorgulama && !BUMYetkiServisi.GormeYetkisiVarMi(kullanan, yil, kurum))
                        {
                            donusDeger.sonuc.islemSonuc = false;
                            donusDeger.sonuc.hataStr = "Verilen kriterlere ile sonucu görme yetkiniz yok!\r\n";
                            return donusDeger;
                        }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABISLForm tabForm = new TABISLForm(ve);

            IDataReader dr = tabForm.Sorgula(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    BIForm biForm = new BIForm(yil, kurum, "", 0);
                    BIFormOl(dr, biForm);
                    if (BUMYetkiServisi.IslemiGormeYetkisiVarMi(kullanan, biForm.formDurum.durumKod, biForm.kullaniciTipKod))
                        donusDeger.Ekle(biForm);
                }
                dr.Close();
                TABISLGider islGider = new TABISLGider(ve);
                TABISLAPGider islGiderAP = new TABISLAPGider(ve);
                TABISLIHP islGiderIcmalAP = new TABISLIHP(ve);
                for (int i = 0; i < donusDeger.ObjeSayisi; i++)
                {
                    BIForm biForm = (BIForm)donusDeger[i];
                    if (biForm.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE))
                        || biForm.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHP)))
                        biForm.miktar = islGiderAP.ToplamBul(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);
                    else if (biForm.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE))
                        || biForm.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHP))
                        )
                        biForm.miktar = islGiderIcmalAP.ToplamBul(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);
                    else
                        biForm.miktar = islGider.ToplamBul(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);
                    biForm.dayanaklar = new TNSCollection();
                    if (!DayanaklariBul(ve, biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo, biForm.dayanaklar))
                    {
                        donusDeger.sonuc.islemSonuc = false;
                        donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM055 + "\r\n" + ve.hata;
                        ve.VeriErisimKapat();
                        return donusDeger;
                    }
                    if (!sadeceOzet)
                    {
                        biForm.giderler = new TNSCollection();
                        //biForm.gelirler = new TNSCollection();
                        //biForm.finanslar = new TNSCollection();
                        if (biForm.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE))
                            || biForm.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHP))
                            || biForm.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHP))
                            || biForm.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE))
                            )
                        {
                            if (!FormDetayGiderBulAP(ve, biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo, biForm.giderler, false))
                            {
                                donusDeger.sonuc.islemSonuc = false;
                                donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM056 + "\r\n" + ve.hata;
                                ve.VeriErisimKapat();
                                return donusDeger;
                            }

                            //if (!FormDetayGelirBulAP(ve, biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo, biForm.gelirler, false))
                            //{
                            //    donusDeger.sonuc.islemSonuc = false;
                            //    donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM057 + "\r\n" + ve.hata;
                            //    ve.VeriErisimKapat();
                            //    return donusDeger;
                            //}

                            //if (!FormDetayFinansBulAP(ve, biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo, biForm.finanslar, false))
                            //{
                            //    donusDeger.sonuc.islemSonuc = false;
                            //    donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM058 + "\r\n" + ve.hata;
                            //    ve.VeriErisimKapat();
                            //    return donusDeger;
                            //}
                        }
                        else
                        {
                            if (!FormDetayGiderBul(ve, biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo, biForm.giderler, false))
                            {
                                donusDeger.sonuc.islemSonuc = false;
                                donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM059 + "\r\n" + ve.hata;
                                ve.VeriErisimKapat();
                                return donusDeger;
                            }

                            //if (!FormDetayGelirBul(ve, biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo, biForm.gelirler, false))
                            //{
                            //    donusDeger.sonuc.islemSonuc = false;
                            //    donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM060 + "\r\n" + ve.hata;
                            //    ve.VeriErisimKapat();
                            //    return donusDeger;
                            //}

                            //if (!FormDetayFinansBul(ve, biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo, biForm.finanslar, false))
                            //{
                            //    donusDeger.sonuc.islemSonuc = false;
                            //    donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM061 + "\r\n" + ve.hata;
                            //    ve.VeriErisimKapat();
                            //    return donusDeger;
                            //}
                        }
                    }
                }
            }
            else
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM062 + "\r\n" + ve.hata;
            }
            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Üst Koda göre,
        /// Bütçe İşlemlerinin listesini döndüren fonksiyon
        /// </summary>
        /// <param name="ustKod">Üst kod bilgisini tutan, String türünde nesne</param>
        /// <param name="hiyerarsik">Hiyerarşik olarak işlem yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <param name="anaKodlar">Ana kodlara göre listeleme yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray ButceIslemListesi(string ustKod, bool hiyerarsik, bool anaKodlar)
        {
            ObjectArray donusDeger = new ObjectArray();
            foreach (IslemBilgi ib in BISabitler.islemListesi)
            {
                if (ib.islemKod.StartsWith(ustKod) && ustKod.Length < ib.islemKod.Length)
                {
                    bool ekle = false;
                    if (anaKodlar)
                    {
                        if (ib.islemKod.Length == 5 && ib.butceIslemiMi)
                            ekle = true;
                    }
                    else
                    {
                        if (hiyerarsik && !(ib.sonKirilim && !ib.butceIslemiMi))
                            ekle = true;
                        else if (!hiyerarsik && ib.sonKirilim && ib.butceIslemiMi)
                            ekle = true;
                    }
                    if (ekle)
                    {
                        donusDeger.Ekle(new ListeSatir(ib.islemKod, ib.islemAd));
                    }
                }
            }
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve İşlem kodu bilgilerine göre,
        /// Dayanak bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgilerini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Dayanak bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray BIDayanakListesi(VeriErisim veriErisim, int yil, int islemKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            TABISLDayanaklar islDayanaklar = new TABISLDayanaklar(ve);
            IDataReader dr = islDayanaklar.Listele(yil, islemKod, 3);
            if (dr != null)
            {
                while (dr.Read())
                {
                    BIDayanak d = new BIDayanak();
                    d.yil = VeriErisim.DegerAl(dr, 0, 0);
                    d.dayanakKod = VeriErisim.DegerAl(dr, 1, 0);
                    d.dayanakAd = VeriErisim.DegerAl(dr, 2, "");
                    d.aciklama = VeriErisim.DegerAl(dr, 3, "");
                    donusDeger.Ekle(d);
                }
                dr.Close();
            }
            else
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM063 + "\r\n" + ve.hata;
            }
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı ve Dayanak bilgilerine göre,
        /// Dayanak bilgisini kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="form">Dayanak bilgisini tutan, BIDayanak türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc DayanakKaydet(VeriErisim veriErisim, Kullanici kullanan, BIDayanak form)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(form.yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABISLDayanaklar tbDayanak = new TABISLDayanaklar(ve);
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                ve.BeginTransaction();
                if (form.dayanakKod == 0)
                {
                    sonuc = tbDayanak.Kaydet(form.yil, tbDayanak.EnBuyukKodBul(form.yil), form.dayanakAd, form.aciklama);
                }
                else if (form.dayanakAd != "")
                {
                    sonuc = tbDayanak.Degistir(form.yil, form.dayanakKod, form.dayanakAd, form.aciklama);
                }
                else
                {
                    sonuc = tbDayanak.Sil(form.yil, form.dayanakKod);
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceUygulamaResI.BUM064;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Bütçe İşlem Dayanak bilgilerine göre,
        /// Dayanak bilgisini okuyan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="form">Dayanak bilgisini tutan, BIDayanak türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Dayanak bilgisini tutan BIDayanak türünde nesne döndürür</returns>
        internal static BIDayanak DayanakOku(VeriErisim veriErisim, BIDayanak form)
        {
            BIDayanak d = null;
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(form.yil), TNS.KYM.Arac.KimlikBul(null));
            TABISLDayanaklar tbDayanak = new TABISLDayanaklar(ve);

            IDataReader dr = tbDayanak.Oku(form.yil, form.dayanakKod);
            if (dr != null)
            {
                if (dr.Read())
                {
                    d = new BIDayanak();
                    d.yil = VeriErisim.DegerAl(dr, 0, 0);
                    d.dayanakKod = VeriErisim.DegerAl(dr, 1, 0);
                    d.dayanakAd = VeriErisim.DegerAl(dr, 2, "");
                    d.aciklama = VeriErisim.DegerAl(dr, 3, "");
                    d.islemKodlari = new int[5];
                    d.islemKodlari[0] = 1;
                    d.islemKodlari[1] = 2;
                    d.islemKodlari[2] = 3;
                    d.islemKodlari[3] = 4;
                    d.islemKodlari[4] = 5;
                }
                dr.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ButceUygulamaResI.BUM065 + "\r\n" + ve.hata);
            }
            ve.VeriErisimKapat();
            return d;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı bilgisine göre,
        /// Kaynak yılda bulunan Dayanak bilgilerini Hedef Yıla kopyalan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="kaynakYil">Kaynak Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="hedefYil">Hedef Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kopyalama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        internal static Sonuc DayanakKopyala(VeriErisim veriErisim, Kullanici kullanan, int kaynakYil, int hedefYil)
        {

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return new Sonuc(false, ButceUygulamaResI.BUM066, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(hedefYil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABISLDayanaklar tbDayanak = new TABISLDayanaklar(ve);
            ve.BeginTransaction();

            Sonuc sonuc = tbDayanak.Kopyala(kaynakYil, hedefYil);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl bilgisine göre,
        /// Yedek tertip listesini dönen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Yedek Tertip listesini tutan ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray YedekTertipListesi(int yil)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.Ekle(new ListeSatir("12.01.31.00-01.1.2.00-1-09.1", ButceUygulamaResI.BUM067));
            donusDeger.Ekle(new ListeSatir("12.01.31.00-01.1.2.00-1-09.3", ButceUygulamaResI.BUM068));
            donusDeger.Ekle(new ListeSatir("12.01.31.00-01.1.2.00-1-09.5", ButceUygulamaResI.BUM069));
            donusDeger.Ekle(new ListeSatir("12.01.31.00-01.1.2.00-1-09.6", ButceUygulamaResI.BUM070));
            donusDeger.Ekle(new ListeSatir("12.01.31.00-01.1.2.00-1-09.7", ButceUygulamaResI.BUM071));
            donusDeger.Ekle(new ListeSatir("12.01.31.00-01.1.2.65-1-09.9", ButceUygulamaResI.BUM072));
            donusDeger.Ekle(new ListeSatir("12.01.31.00-01.1.2.66-1-09.9", ButceUygulamaResI.BUM073));
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu ve Sıra no bilgilerini kullanarak,
        /// Kurumlar listesini döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kurumlar bilgisini tutan ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray BIKurumlarListesi(VeriErisim veriErisim, int yil, string kurum, string islemKod, int siraNo)
        {
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            ObjectArray donusDeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            TABISLGider islGider = new TABISLGider(ve);
            IDataReader dr = islGider.KurumlarListesi(yil, kurum, islemKod, siraNo);
            if (dr != null)
            {
                TNSCollection kurumKodlari = new TNSCollection();
                while (dr.Read())
                {
                    string kurumKod = TNS.BKD.Arac.KurumsaldanKuruma(VeriErisim.DegerAl(dr, 0, ""), tUzBul.Kirilimlar(0));
                    if (!kurumKodlari.Contains(kurumKod))
                        kurumKodlari.Add(kurumKod);
                }
                dr.Close();
                IUZYServis uzyServis = BUMServis.TanimlaUZY();
                for (int i = 0; i < kurumKodlari.Count; i++)
                {
                    donusDeger.Ekle(new ABSKodDil((string)kurumKodlari[i], uzyServis.UzayDegeriStr(null, "ABSKUR", yil.ToString() + "-" + ((string)kurumKodlari[i]), true, "") + " " + ButceUygulamaResI.BUM074));
                }
            }
            else
            {
                donusDeger.sonuc = new Sonuc(false, ButceUygulamaResI.BUM075 + "\r\n" + ve.hata, "");
            }
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen  Kullanıcı, Yıl, Kurum, İşlem Kodu, Sıra No bilgilerine göre,
        /// Bütçe İşlem bilgilerini okuyan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra numarası bilgisini tutan, Int türünde nesne</param>
        /// <param name="detayli">Detay bilgilerinde getirilip getirilmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgisini tutan Object türünde nesne döndürür</returns>
        public static object BIOku(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, string islemKod, int siraNo, bool detayli)
        {
            if (islemKod == null) islemKod = "";
            if (islemKod.Length < 5)
                return new Sonuc(false, ButceUygulamaResI.BUM076 + " '" + islemKod + "' " + ButceUygulamaResI.BUM077, "");

            if (BUMYetkiServisi.GormeYetkisiVarMi(kullanan, yil, kurum, -1))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABISLForm tabForm = new TABISLForm(ve);
                IDataReader dr = tabForm.Oku(yil, kurum, islemKod, siraNo);
                if (dr != null)
                {
                    if (dr.Read())
                    {
                        BIForm biForm = new BIForm(yil, kurum, islemKod, siraNo);
                        BIFormOl(dr, biForm);
                        kurum = biForm.kurum;
                        dr.Close();
                        if (!BUMYetkiServisi.IslemiGormeYetkisiVarMi(kullanan, biForm.formDurum.durumKod, biForm.kullaniciTipKod))
                        {
                            ve.VeriErisimKapat();
                            return new Sonuc(false, ButceUygulamaResI.BUM078, "");
                        }
                        biForm.dayanaklar = new TNSCollection();
                        if (!DayanaklariBul(ve, yil, kurum, islemKod, biForm.siraNo, biForm.dayanaklar))
                        {
                            ve.VeriErisimKapat();
                            return new Sonuc(false, ButceUygulamaResI.BUM079 + "\r\n" + ve.hata, "");
                        }
                        if (!islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IPTAL)))
                        {
                            biForm.giderler = new TNSCollection();
                            //biForm.gelirler = new TNSCollection();
                            //biForm.finanslar = new TNSCollection();

                            TABISLAPGider islGiderAP = new TABISLAPGider(ve);
                            biForm.miktar = islGiderAP.ToplamBul(yil, kurum, islemKod, biForm.siraNo);

                            if (!FormDetayGiderBulAP(ve, yil, kurum, islemKod, biForm.siraNo, biForm.giderler, detayli))
                            {
                                ve.VeriErisimKapat();
                                return new Sonuc(false, ButceUygulamaResI.BUM080 + "\r\n" + ve.hata, "");
                            }
                            //if (!FormDetayGelirBulAP(ve, yil, kurum, islemKod, biForm.siraNo, biForm.gelirler, detayli))
                            //{
                            //    ve.VeriErisimKapat();
                            //    return new Sonuc(false, ButceUygulamaResI.BUM081 + "\r\n" + ve.hata, "");
                            //}
                            //if (!FormDetayFinansBulAP(ve, yil, kurum, islemKod, biForm.siraNo, biForm.finanslar, detayli))
                            //{
                            //    ve.VeriErisimKapat();
                            //    return new Sonuc(false, ButceUygulamaResI.BUM082 + "\r\n" + ve.hata, "");
                            //}
                        }
                        ve.VeriErisimKapat();
                        return biForm;
                    }
                    else
                    {
                        ve.VeriErisimKapat();
                        return new Sonuc(false, ButceUygulamaResI.BUM084, "");
                    }
                }
                else
                {
                    ve.VeriErisimKapat();
                    return new Sonuc(false, ButceUygulamaResI.BUM084, "");
                }
            }
            else
            {
                return new Sonuc(false, ButceUygulamaResI.BUM085, "");
            }
        }

        private static void FormKontrol(BIForm form, Sonuc sonuc)
        {
            //Kurum Sayısı Kontrolü Yap
            if (!form.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IPTAL)))
            {
                ObjectArray lv = YedekTertipListesi(form.yil);

                IUZYServis servis = BUMServis.TanimlaUZY();
                if (!form.KurumSayisiDogruMu(servis, sonuc, lv.objeler))
                    return;
                //if (form.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.ANAEKLEMEAKTARMA)))
                //    if (!form.KurumToplamlariDogruMu(sonuc))
                //        return;

                //if (form.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))
                //    if (!form.AFPToplamlariDogruMu(sonuc))
                //        return;
            }
        }

        private static Sonuc TABButceyeYansit(VeriErisim veriErisim, int yil, string islemKod, TNSCollection giderler, int dusurEkle, bool taslakMi)
        {
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            TABButce butce = new TABButceGider(veriErisim);
            double serbest, eklenen, dusulen;
            Sonuc sonuc = new Sonuc(true, "", "");
            if (giderler != null)
            {
                for (int i = 0; i < giderler.Count && sonuc.islemSonuc; i++)
                {
                    BISatir detay = (BISatir)giderler[i];
                    if (TNS.BKD.Arac.KurumsaldanKuruma(detay.tertip.kodlar[0], tUzBul.Kirilimlar(0)) == VarsayilanKurum)
                    {
                        if (detay.tertip.tabloKod == ENUMTabloKod.TABButce)
                        {
                            int ahpSerbestAy = AHPDurum(yil, (string)detay.tertip.kodlar[0], (string)detay.tertip.kodlar[3]);
                            if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.SERBEST)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))
                            {
                                eklenen = 0;
                                dusulen = 0;
                                serbest = 0;
                                if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.SERBEST)) && ahpSerbestAy == 0)
                                    serbest = detay.eklenen_serbest;
                                else if ((islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE))) && ahpSerbestAy > 0)
                                {
                                    for (int ay = 0; ay < detay.aylikProgram.Length; ay++)
                                    {
                                        if (ay < ahpSerbestAy)
                                            serbest += detay.aylikProgram[ay];
                                    }
                                }
                            }
                            else
                            {
                                eklenen = detay.eklenen_serbest;
                                dusulen = detay.dusulen;
                                serbest = eklenen - dusulen;
                            }
                            sonuc = butce.TaslakDegistir(yil, detay.tertip.kodlar, dusurEkle * eklenen, dusurEkle * dusulen, dusurEkle * serbest, taslakMi);
                            if (!sonuc.islemSonuc)
                            {
                                sonuc.hataStr = (i + 1).ToString() + ButceUygulamaResI.BUM086 + " '" + detay.tertip.TertipStrOl() + "' " + ButceUygulamaResI.BUM087 + sonuc.hataStr + "-" + veriErisim.hata;
                            }
                        }
                    }
                }
            }

            return sonuc;
        }

        //private static Sonuc TABGelireYansit(VeriErisim veriErisim, int yil, string islemKod, TNSCollection gelirler, int dusurEkle, bool taslakMi)
        //{
        //    TABButce butceGelir = new TABButceGelir(veriErisim);
        //    double serbest, eklenen, dusulen;
        //    Sonuc sonuc = new Sonuc(true, "", "");
        //    if (gelirler != null)
        //    {
        //        for (int i = 0; i < gelirler.Count && sonuc.islemSonuc; i++)
        //        {
        //            BISatir detay = (BISatir)gelirler[i];
        //            if (TNS.BKD.Arac.KurumsaldanKuruma(detay.tertip.kodlar[0]) == VarsayilanKurum)
        //            {
        //                if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.ANAEKLEMEAKTARMA)))
        //                {
        //                    eklenen = detay.eklenen_serbest;
        //                    dusulen = detay.dusulen;
        //                    serbest = 0;
        //                    sonuc = butceGelir.TaslakDegistir(yil, detay.tertip.kodlar, dusurEkle * eklenen, dusurEkle * dusulen, dusurEkle * serbest, taslakMi);
        //                    if (!sonuc.islemSonuc)
        //                    {
        //                        sonuc.hataStr = (i + 1).ToString() + ButceUygulamaResI.BUM086 + " '" + detay.tertip.TertipStrOl() + "' " + ButceUygulamaResI.BUM087 + sonuc.hataStr + "-" + veriErisim.hata;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return sonuc;
        //}

        //private static Sonuc TABFinansmanaYansit(VeriErisim veriErisim, int yil, string islemKod, TNSCollection finanslar, int dusurEkle, bool taslakMi)
        //{
        //    TABButce butceFinansman = new TABButceFinansman(veriErisim);
        //    double serbest, eklenen, dusulen;
        //    Sonuc sonuc = new Sonuc(true, "", "");
        //    if (finanslar != null)
        //    {
        //        for (int i = 0; i < finanslar.Count && sonuc.islemSonuc; i++)
        //        {
        //            BISatir detay = (BISatir)finanslar[i];
        //            if (TNS.BKD.Arac.KurumsaldanKuruma(detay.tertip.kodlar[0]) == VarsayilanKurum)
        //            {
        //                if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.ANAEKLEMEAKTARMA)))
        //                {
        //                    eklenen = detay.eklenen_serbest;
        //                    dusulen = detay.dusulen;
        //                    serbest = 0;
        //                    sonuc = butceFinansman.TaslakDegistir(yil, detay.tertip.kodlar, dusurEkle * eklenen, dusurEkle * dusulen, dusurEkle * serbest, taslakMi);
        //                    if (!sonuc.islemSonuc)
        //                    {
        //                        sonuc.hataStr = (i + 1).ToString() + ButceUygulamaResI.BUM086 + " '" + detay.tertip.TertipStrOl() + "' " + ButceUygulamaResI.BUM087 + sonuc.hataStr + "-" + veriErisim.hata;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return sonuc;
        //}

        /// <summary>
        /// Parametre olarak verilen Bütçe İşlem Form bilgilerine göre,
        /// Bütçe İşlemini silen fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="biForm">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, silme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc BISil(VeriErisim ve, BIForm biForm)
        {
            TABISLForm islForm = new TABISLForm(ve);
            TABISLFormDayanak islFormDayanak = new TABISLFormDayanak(ve);
            if (!biForm.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IPTAL)))
            {
                TABISLGider islGider = new TABISLGider(ve);
                TABISLAPGider islGiderAP = new TABISLAPGider(ve);
                TABISLGelir islGelir = new TABISLGelir(ve);
                TABISLAPGelir islGelirAP = new TABISLAPGelir(ve);
                TABISLFinansman islFinans = new TABISLFinansman(ve);
                TABISLAPFinansman islFinansAP = new TABISLAPFinansman(ve);
                TABISLIHP islGiderIcmalAP = new TABISLIHP(ve);
                TABISLDevir devir = new TABISLDevir(ve);

                if (ve.hata == "")
                    islGider.Sil(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);
                if (ve.hata == "")
                    islGiderAP.Sil(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);
                if (ve.hata == "")
                    islGiderIcmalAP.Sil(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);
                if (ve.hata == "")
                    devir.Sil(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);

                if (ve.hata == "")
                    islGelir.Sil(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);
                if (ve.hata == "")
                    islGelirAP.Sil(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);

                if (ve.hata == "")
                    islFinans.Sil(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);
                if (ve.hata == "")
                    islFinansAP.Sil(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);
            }
            if (ve.hata == "")
                islFormDayanak.Sil(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);
            if (ve.hata == "")
                islForm.Sil(biForm.yil, biForm.kurum, biForm.islemKod, biForm.siraNo);

            Sonuc s = new Sonuc(ve.hata == "", ve.hata, "");

            if (!biForm.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IPTAL)))
            {
                if (s.islemSonuc)
                    s = TABButceyeYansit(ve, biForm.yil, biForm.islemKod, biForm.giderler, -1, true);
                //if (s.islemSonuc)
                //    s = TABGelireYansit(ve, biForm.yil, biForm.islemKod, biForm.gelirler, -1, true);
                //if (s.islemSonuc)
                //    s = TABFinansmanaYansit(ve, biForm.yil, biForm.islemKod, biForm.finanslar, -1, true);
            }
            return s;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum, İşlem Kodu ve Sıra Numarası bilgilerine göre,
        /// Bütçe işlemini silen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra numarası bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, silme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BISil(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, string islemKod, int siraNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc sonuc;
            object s = BIOku(ve, kullanan, yil, kurum, islemKod, siraNo, false);
            if (s.GetType() == typeof(BIForm))
            {
                ve.BeginTransaction();
                sonuc = BISil(ve, (BIForm)s);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            else
                sonuc = (Sonuc)s;
            ve.VeriErisimKapat();
            return sonuc;
        }


        /// <summary>
        /// Yıl içinde gider bütçe işlemleriyle açılan tertiplerin hangi kırılımdan açıldığını gösteren Yıldızı kontrol eden fonksiyon.
        /// Tertip varsa ve yıldız yoksa devam eder, varsa hata verir.
        /// Tertip yoksa, Başka bir işlemde yıldız var mı? kontrolü yapar, varsa hata verir.
        /// Başka işlemde yıldız varsa, ve bunda da yıldız varsa hata verir.
        /// Başka işlemde yıldız yoksa, ve bunda da yıldız yoksa hata verir.
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="sonuc">Sonucu tutan, Sonuc türünde nesne</param>
        /// <param name="uzyServis">Uzay servisini tutan, IUZYServis türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="detay">Detay bilgisini tutan, BISatir türünde nesne</param>
        /// <param name="devirKontrol">Devir kontrolünün yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        public static void YildizKontrolGider(VeriErisim veriErisim, Sonuc sonuc, IUZYServis uzyServis, int yil, BISatir detay, bool devirKontrol)
        {
            string kurum, islemKod;
            int siraNo;
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            TABButce butce = new TABButceGider(veriErisim);
            TABISLGider islGider = new TABISLGider(veriErisim);
            BITertipBilgi tertipBilgiOnayli = new BITertipBilgi();
            BITertipBilgi tertipBilgiOnaysiz = new BITertipBilgi();

            if (devirKontrol)
            {
                double kboDevir;
                kboDevir = butce.YSOTBul(yil - 1, detay.devredenTertip.kodlar, false);
                if (kboDevir <= 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceUygulamaResI.BUM088 + " (" + detay.devredenTertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM089;
                    return;
                }
            }
            if (TNS.BKD.Arac.KurumsaldanKuruma(detay.tertip.kodlar[0], tUzBul.Kirilimlar(0)) == VarsayilanKurum)
            {
                double kbo = butce.KBOBul(yil, detay.tertip.kodlar);
                if (kbo > 0)
                {
                    if (detay.yildizYer != 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ButceUygulamaResI.BUM090 + "(" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM091;
                    }
                    return;
                }
                if (islGider.YildizVarMi(yil, detay.tertip.kodlar, out kurum, out islemKod, out siraNo))
                {
                    if (detay.yildizYer != 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ButceUygulamaResI.BUM092 + islemKod + " " + ButceUygulamaResI.BUM093 + siraNo.ToString() + ButceUygulamaResI.BUM094 + detay.tertip.TertipStrOl() + ButceUygulamaResI.BUM095;
                    }
                }
                else
                {
                    if (detay.yildizYer == 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ButceUygulamaResI.BUM096 + detay.tertip.TertipStrOl() + ButceUygulamaResI.BUM097;
                    }
                    else
                    {
                        if (detay.tertip.TamMi())
                        {
                            string kurAd = uzyServis.UzayDegeriStr(null, "ABSKUR", yil + "-" + (string)detay.tertip.kodlar[0], true, null);
                            string fonAd = uzyServis.UzayDegeriStr(null, "ABSFON", yil + "-" + (string)detay.tertip.kodlar[1], true, null);
                            string finAd = uzyServis.UzayDegeriStr(null, "ABSFIN", yil + "-" + (string)detay.tertip.kodlar[2], true, null);
                            string ekoAd = uzyServis.UzayDegeriStr(null, "ABSEKO", yil + "-" + (string)detay.tertip.kodlar[3], true, null);
                            if (kurAd == "" || fonAd == "" || finAd == "" || ekoAd == "")
                            {
                                sonuc.islemSonuc = false;
                                sonuc.hataStr = ButceUygulamaResI.BUM098 + (string)detay.tertip.kodlar[0] + "/" + kurAd + ButceUygulamaResI.BUM099 + (string)detay.tertip.kodlar[1] + "/" + fonAd + ButceUygulamaResI.BUM100 + (string)detay.tertip.kodlar[2] + "/" + finAd + ButceUygulamaResI.BUM101 + (string)detay.tertip.kodlar[3] + "/" + ekoAd + ButceUygulamaResI.BUM102;
                            }
                            else
                            {
                                butce.Kaydet(yil, detay.tertip.kodlar, 0, 0);
                                veriErisim.hata = "";
                            }
                        }
                        else
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = ButceUygulamaResI.BUM103 + " (" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM104;
                        }
                    }
                }
            }
            else //Bizim olmayan tertip yalnızca tertip kontrolü yap
            {
                if (detay.tertip.TamMi())
                {
                    //string kurAd = uzyServis.UzayDegeriStr(null, "ABSKUR", yil + "-" + (string)detay.tertip.kodlar[0], true, null);
                    string fonAd = uzyServis.UzayDegeriStr(null, "ABSFON", yil + "-" + (string)detay.tertip.kodlar[1], true, null);
                    string finAd = uzyServis.UzayDegeriStr(null, "ABSFIN", yil + "-" + (string)detay.tertip.kodlar[2], true, null);
                    string ekoAd = uzyServis.UzayDegeriStr(null, "ABSEKO", yil + "-" + (string)detay.tertip.kodlar[3], true, null);
                    //if (kurAd == "" || fonAd == "" || finAd == "" || ekoAd == "")
                    if (fonAd == "" || finAd == "" || ekoAd == "")
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ButceUygulamaResI.BUM105 + "(" + (string)detay.tertip.kodlar[1] + "/" + fonAd + ") " + ButceUygulamaResI.BUM106 + "(" + (string)detay.tertip.kodlar[2] + "/" + finAd + ") " + ButceUygulamaResI.BUM107 + "(" + (string)detay.tertip.kodlar[3] + "/" + ekoAd + ") " + ButceUygulamaResI.BUM108;
                    }
                }
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceUygulamaResI.BUM103 + " (" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM108;
                }
            }
        }

        /// <summary>
        /// Parametre olarak verilen İşlem koduna göre,
        /// Harcama Programının Otomatik olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, işlem kodu 02 ile başlıyorsa true, başlamıyorsa false değerini tutan Bool türünde nesne döndürür</returns>
        private static bool HarcamaProgramiOtomatikMi(string islemKod)
        {
            return islemKod.StartsWith("02");
        }

        /// <summary>
        /// Parametre olarak verilen İşlem koduna göre,
        /// Harcama Programının Zorunlu olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, işlem kodu 01.03 ile başlamıyorsa true başlıyorsa false değerini tutan Bool türünde nesne döndürür</returns>
        private static bool HarcamaProgramiZorunluMu(string islemKod)
        {
            return !islemKod.StartsWith("01.03");
        }

        private static void HarcamaProgramiKontrol(Sonuc sonuc, string islemKod, BISatir detay)
        {
            if (!HarcamaProgramiOtomatikMi(islemKod))
            {
                if (HarcamaProgramiZorunluMu(islemKod))
                {
                    if (detay.aylikProgram == null)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ButceUygulamaResI.BUM109;
                    }
                }
                else if (detay.aylikProgram != null)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceUygulamaResI.BUM109;
                }
            }
        }

        private static void HarcamaProgramiSonDurumBul(VeriErisim ve, Sonuc sonuc, int yil, string islemKod, string[] kodlar, double[] bc, bool harcananDahil, ENUMFormDurumGrup onayDurum, bool sadeceRevizeler)
        {
            TABISLAPGider ahp = new TABISLAPGider(ve);
            for (int i = 0; i < 12; i++)
                bc[i] = 0;

            IDataReader dr = ahp.TertipDurum(yil, kodlar, onayDurum, sadeceRevizeler);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int ay = VeriErisim.DegerAl(dr, 0, 0);
                    if (ay >= 1 && ay <= 12)
                        bc[ay - 1] = VeriErisim.DegerAl(dr, 1, 0.0);
                }
                dr.Close();
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
            }

            if (sonuc.islemSonuc && harcananDahil)
            {
                double onayliGond, onaysizGond, gond;
                double onayliHarc, onaysizHarc;
                double onayliTenkis, onaysizTenkis;
                double maks;
                GonderilenBul(ve, yil, kodlar, out onayliGond, out onaysizGond, out onayliTenkis, out onaysizTenkis);
                //TenkisEdilenBul(ve, yil, kodlar);
                HarcananBul(ve, yil, kodlar, out onayliHarc, out onaysizHarc);
                gond = onayliGond + onaysizGond - onayliTenkis;
                if (gond > onayliHarc + onaysizHarc)
                    maks = gond;
                else
                    maks = onayliHarc + onaysizHarc;
                for (int i = 0; i < 12 && maks > 0; i++)
                {
                    if (maks > (double)bc[i])
                    {
                        maks -= (double)bc[i];
                        bc[i] = (double)0;
                    }
                    else
                    {
                        bc[i] = (double)bc[i] - maks;
                        maks = 0;
                    }
                }
            }
        }

        private static void HarcamaProgramiGider(VeriErisim ve, Sonuc sonuc, int yil, string islemKod, BISatir detay, int ay, bool aktarim)
        {
            if (sonuc.islemSonuc)
                HarcamaProgramiKontrol(sonuc, islemKod, detay);
            if (sonuc.islemSonuc && HarcamaProgramiOtomatikMi(islemKod))
            {
                if (detay.aylikProgram == null)
                    detay.aylikProgram = new double[12];
                else return;
                for (int i = 0; i < 12; i++)
                    detay.aylikProgram[i] = 0;
                int ahpSerbestAy = AHPDurum(yil, (string)detay.tertip.kodlar[0], (string)detay.tertip.kodlar[3]);
                if (ahpSerbestAy > 0 && detay.miktar < 0)
                {
                    double[] bc = new double[12];
                    HarcamaProgramiSonDurumBul(ve, sonuc, yil, islemKod, detay.tertip.kodlar, bc, true, ENUMFormDurumGrup.ONAYLI, false);

                    double maks = 0 - detay.miktar;
                    string aylikDurum = "";
                    for (int i = 0; i < 12 && maks > 0; i++)
                    {
                        aylikDurum = aylikDurum + ((double)bc[i]).ToString("#,##0") + ", ";
                        if (maks > bc[i])
                        {
                            detay.aylikProgram[i] = 0 - bc[i];
                            maks -= bc[i];
                        }
                        else
                        {
                            detay.aylikProgram[i] = 0 - maks;
                            maks = 0;
                        }
                    }
                    if (maks > 0)
                    {
                        if (!aktarim)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = detay.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM110 + maks.ToString("#.###,##") + " % " + aylikDurum;
                        }
                        detay.aylikProgram[ay - 1] = detay.aylikProgram[System.DateTime.Now.Month] - maks;
                    }
                }
                else if (detay.miktar != 0)
                {
                    if (ahpSerbestAy <= 0)
                        ahpSerbestAy = 1;

                    detay.aylikProgram[ahpSerbestAy - 1] = detay.miktar;
                }
            }
        }

        private static void YildizKontrolGiderIcmal(VeriErisim veriErisim, Sonuc sonuc, IUZYServis uzyServis, int yil, BISatir detay)
        {
            TABButce butce = new TABButceGider(veriErisim);
            string[] kodlar = new string[4];
            kodlar[0] = detay.tertip.kodlar[0];
            kodlar[1] = "";
            kodlar[2] = "";
            kodlar[3] = detay.tertip.kodlar[1];
            double kbo = butce.KBOBul(yil, kodlar);
            if (kbo <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceUygulamaResI.BUM111 + "(" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM112;
            }
        }

        private static void IcmalProgramiSonDurumBul(VeriErisim ve, Sonuc sonuc, int yil, string islemKod, string[] kodlar, double[] bc, bool harcananDahil, ENUMFormDurumGrup onayDurum, bool sadeceRevizeler)
        {
            TABISLIHP ihp = new TABISLIHP(ve);
            for (int i = 0; i < 12; i++)
                bc[i] = 0;

            IDataReader dr = ihp.TertipDurum(yil, kodlar, onayDurum, sadeceRevizeler);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int ay = VeriErisim.DegerAl(dr, 0, 0);
                    if (ay >= 1 && ay <= 12)
                        bc[ay - 1] = VeriErisim.DegerAl(dr, 1, 0.0);
                }
                dr.Close();
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
            }

            if (sonuc.islemSonuc && harcananDahil)
            {
                double onayliGond, onaysizGond, gond;
                double onayliHarc, onaysizHarc;
                double onayliTenkis, onaysizTenkis;
                double maks;
                string[] icKodlar = { kodlar[0], "", "", kodlar[1] };
                GonderilenBul(ve, yil, icKodlar, out onayliGond, out onaysizGond, out onayliTenkis, out onaysizTenkis);
                //TenkisEdilenBul(ve, yil, icKodlar);
                HarcananBul(ve, yil, icKodlar, out onayliHarc, out onaysizHarc);
                gond = onayliGond + onaysizGond - onayliTenkis;
                if (gond > onayliHarc + onaysizHarc)
                    maks = gond;
                else
                    maks = onayliHarc + onaysizHarc;
                for (int i = 0; i < 12 && maks > 0; i++)
                {
                    if (maks > (double)bc[i])
                    {
                        maks -= (double)bc[i];
                        bc[i] = 0;
                    }
                    else
                    {
                        bc[i] = (double)bc[i] - maks;
                        maks = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Parametre olarak verilen Yıl bilgisine göre,
        /// Değişken Servisi kullanarak, Aktarma Üst Oranı bulan  ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Aktarma Üst Oranını tutan Int türünde nesne döndürür</returns>
        private static int YuzdeYirmiKontrolOraniBul(int yil)
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(yil, "AKTARMAUSTORAN"), -1);
        }

        private static Sonuc GiderTertipleriTutarliMi(VeriErisim veriErisim, BIForm islem, bool icmalKontrol, bool islemiEkle, bool yuzdeYirmiKontroluYap)
        {
            Tertip tUzBul = new Tertip(islem.yil, ENUMTabloKod.TABPEBOdenek);

            int yuzdeYirmi = -1;
            if (yuzdeYirmiKontroluYap)
                yuzdeYirmi = YuzdeYirmiKontrolOraniBul(islem.yil);

            Sonuc donusDeger = new Sonuc(true, "", "");
            TABButce butce = new TABButceGider(veriErisim);
            if (islem.giderler != null)
            {
                TNSCollection ekoKodlar = new TNSCollection();
                for (int i = 0; i < islem.giderler.Count; i++)
                {
                    BISatir satir = (BISatir)islem.giderler[i];
                    if (satir.tertip.tabloKod == ENUMTabloKod.TABButce)
                    {
                        string icKurum = TNS.BKD.Arac.KurumsaldanKuruma(satir.tertip.kodlar[0], tUzBul.Kirilimlar(0));
                        if (ekoKodlar.IndexOf(icKurum + "-" + satir.tertip.kodlar[3]) < 0)
                            ekoKodlar.Add(icKurum + "-" + satir.tertip.kodlar[3]);

                        double kbo = butce.KBOBul(islem.yil, satir.tertip.kodlar);
                        BITertipBilgi tertipBilgiOnayli = new BITertipBilgi();

                        BITertipBilgi tertipBilgiOnaysiz = new BITertipBilgi();
                        GiderTertipBilgiBul(veriErisim, islem.yil, satir.tertip.kodlar, tertipBilgiOnayli, tertipBilgiOnaysiz);
                        double sonucKbo = kbo + tertipBilgiOnayli.eklenen - (tertipBilgiOnayli.dusulen);

                        if (islemiEkle)
                            if (islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AKTARMA)) || islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.EKLEME)))
                            {
                                sonucKbo += satir.miktar;
                            }


                        if (islem.islemKod == BISabitler.IslemKod(ENUMIslemKod.SERBEST))
                        {
                            if (islemiEkle)
                                tertipBilgiOnayli.serbest += satir.miktar;
                            if (tertipBilgiOnayli.serbest > sonucKbo)
                            {
                                donusDeger.DegerAta(ButceUygulamaResI.BUM113 + " " + satir.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM114 + " (" + Virgulle(tertipBilgiOnayli.serbest, 2) + "), " + ButceUygulamaResI.BUM115 + " (" + Virgulle(sonucKbo, 2) + "=" + Virgulle(kbo, 2) + "+" + Virgulle(tertipBilgiOnayli.eklenen, 2) + "-" + Virgulle(tertipBilgiOnayli.dusulen, 2) + ") " + ButceUygulamaResI.BUM116, "");
                                return donusDeger;
                            }
                        }
                        else if (islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AKTARMA)))
                        {
                            if (satir.miktar < 0)
                            {
                                double kullanilan, gonderilen, harcanan;
                                gonderilen = tertipBilgiOnayli.gonderilen + tertipBilgiOnaysiz.gonderilen - tertipBilgiOnayli.tenkisEdilen;
                                harcanan = tertipBilgiOnayli.harcanan;
                                if (gonderilen > harcanan) kullanilan = gonderilen;
                                else kullanilan = harcanan;
                                if (islemiEkle)
                                    tertipBilgiOnayli.serbest += satir.miktar;

                                if ((tertipBilgiOnayli.serbest - kullanilan) < 0)
                                {
                                    donusDeger.DegerAta(satir.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM117 + " (" + Virgulle(satir.dusulen, 2) + "), " + ButceUygulamaResI.BUM118 + "(" + Virgulle(tertipBilgiOnayli.serbest - kullanilan, 2) + "=" + Virgulle(tertipBilgiOnayli.serbest, 2) + "-" + Virgulle(kullanilan, 2) + ") " + ButceUygulamaResI.BUM119, "");
                                    return donusDeger;
                                }
                            }
                            else if (satir.miktar > 0 && yuzdeYirmiKontroluYap & yuzdeYirmi >= 0)
                            {
                                if (tertipBilgiOnayli.eklenen * 100 / kbo > yuzdeYirmi)
                                {
                                    donusDeger.DegerAta(satir.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM120 + " (" + Virgulle(tertipBilgiOnayli.eklenen, 2) + "), " + ButceUygulamaResI.BUM121 + "(" + Virgulle(kbo, 2) + " " + ButceUygulamaResI.BUM122 + " " + yuzdeYirmi.ToString() + " " + ButceUygulamaResI.BUM123, "");
                                    return donusDeger;
                                }
                            }
                        }
                        else if (islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE)))
                        {
                            TABISLAPGider ahp = new TABISLAPGider(veriErisim);
                            double icmalToplam = ahp.ToplamBulRevize(islem.yil, satir.tertip.kodlar, true, -1, ENUMFormDurumGrup.ONAYLI);
                            if (islemiEkle)
                                for (int ay = 0; ay < 12; ay++)
                                    icmalToplam += (double)satir.aylikProgram[ay];
                            if (icmalToplam > sonucKbo)
                            {
                                donusDeger.DegerAta(ButceUygulamaResI.BUM124 + " " + satir.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM125 + " (" + Virgulle(icmalToplam, 0) + "), " + ButceUygulamaResI.BUM126 + " (" + Virgulle(sonucKbo, 0) + ") " + ButceUygulamaResI.BUM127, "");
                                return donusDeger;
                            }
                            double[] ahpDurum = new double[12];
                            HarcamaProgramiSonDurumBul(veriErisim, donusDeger, islem.yil, islem.islemKod, satir.tertip.kodlar, ahpDurum, false, ENUMFormDurumGrup.ONAYLI, false);
                            if (!donusDeger.islemSonuc)
                                return donusDeger;

                            for (int ay = 0; ay < 12; ay++)
                            {
                                if (islemiEkle)
                                    ahpDurum[ay] = (double)ahpDurum[ay] + (double)satir.aylikProgram[ay];
                                if (((double)ahpDurum[ay]) < 0)
                                {
                                    donusDeger.DegerAta(ButceUygulamaResI.BUM128 + " " + satir.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM129 + ", " + (ay + 1).ToString() + " " + ButceUygulamaResI.BUM130 + " (" + Virgulle((double)ahpDurum[i], 0) + "), " + ButceUygulamaResI.BUM131, "");
                                    return donusDeger;
                                }
                            }
                        }
                    }
                    else if (satir.tertip.tabloKod == ENUMTabloKod.TABGiderIcmal)
                    {
                        TABISLIHP icmalRevize = new TABISLIHP(veriErisim);
                        string[] kodlar = { satir.tertip.kodlar[0], "", "", satir.tertip.kodlar[1] };
                        double kbo = butce.KBOBul(islem.yil, kodlar);
                        double icmalToplam = icmalRevize.ToplamBulRevize(islem.yil, satir.tertip.kodlar, ENUMFormDurumGrup.ONAYLI);
                        if (islemiEkle)
                            for (int ay = 0; ay < 12; ay++)
                                icmalToplam += (double)satir.aylikProgram[ay];
                        if (icmalToplam > kbo)
                        {
                            donusDeger.DegerAta(ButceUygulamaResI.BUM132 + " " + satir.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM133 + " (" + Virgulle(icmalToplam, 0) + "), " + ButceUygulamaResI.BUM134 + " (" + Virgulle(kbo, 0) + ") " + ButceUygulamaResI.BUM135, "");
                            return donusDeger;
                        }
                    }
                }
                if (icmalKontrol && islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE)))
                {
                    for (int i = 0; i < ekoKodlar.Count; i++)
                    {
                        double[] ahpDurum = new double[12];
                        double[] ihpDurum = new double[12];
                        string[] kodlarS = ((string)ekoKodlar[i]).Split('-');
                        string[] kodlar = { kodlarS[0], "", "", kodlarS[1] };
                        HarcamaProgramiSonDurumBul(veriErisim, donusDeger, islem.yil, islem.islemKod, kodlar, ahpDurum, false, ENUMFormDurumGrup.ONAYLI, true);
                        if (!donusDeger.islemSonuc)
                            return donusDeger;

                        IcmalProgramiSonDurumBul(veriErisim, donusDeger, islem.yil, islem.islemKod, kodlarS, ihpDurum, false, ENUMFormDurumGrup.ONAYLI, true);
                        if (!donusDeger.islemSonuc)
                            return donusDeger;
                        double toplamAhp = 0;
                        double toplamIhp = 0;
                        for (int ay = 0; ay < 12; ay++)
                        {
                            toplamAhp = toplamAhp + (double)ahpDurum[ay];
                            toplamIhp = toplamIhp + (double)ihpDurum[ay];
                            if (toplamAhp > toplamIhp)
                            {
                                donusDeger.DegerAta(ButceUygulamaResI.BUM136 + " " + (string)ekoKodlar[i] + " " + ButceUygulamaResI.BUM137 + ", " + (ay + 1).ToString() + ButceUygulamaResI.BUM138 + " (" + ButceUygulamaResI.BUM139 + ", " + Virgulle(toplamIhp, 0) + "), " + ButceUygulamaResI.BUM140 + " (" + ButceUygulamaResI.BUM141 + ", " + Virgulle(toplamAhp, 0) + ") " + ButceUygulamaResI.BUM142, "");
                                return donusDeger;
                            }
                        }

                    }
                }
            }
            donusDeger.islemSonuc = true;
            return donusDeger;
        }

        //private static void YildizKontrolGelir(VeriErisim veriErisim, Sonuc sonuc, IUZYServis uzyServis, int yil, BISatir detay)
        //{
        //    string kurum, islemKod;
        //    int siraNo;

        //    TABButce butceGelir = new TABButceGelir(veriErisim);
        //    TABISLGelir islGelir = new TABISLGelir(veriErisim);
        //    BITertipBilgi tertipBilgiOnayli = new BITertipBilgi();
        //    BITertipBilgi tertipBilgiOnaysiz = new BITertipBilgi();
        //    if (detay.tertip.kodlar[0] == VarsayilanKurum)
        //    {
        //        double kbo = butceGelir.KBOBul(yil, detay.tertip.kodlar);
        //        if (kbo > 0)
        //        {
        //            if (detay.yildizYer != 0)
        //            {
        //                sonuc.islemSonuc = false;
        //                sonuc.hataStr = ButceUygulamaResI.BUM143 + "(" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM144 + "!(" + ButceUygulamaResI.BUM145 + ")";
        //            }
        //            return;
        //        }
        //        if (islGelir.YildizVarMi(yil, detay.tertip.kodlar, out kurum, out islemKod, out siraNo))
        //        {
        //            if (detay.yildizYer != 0)
        //            {
        //                sonuc.islemSonuc = false;
        //                sonuc.hataStr = ButceUygulamaResI.BUM146 + "(" + ButceUygulamaResI.BUM147 + islemKod + " " + ButceUygulamaResI.BUM148 + siraNo.ToString() + ") " + ButceUygulamaResI.BUM149 + "(" + detay.tertip.TertipStrOl() + "), " + ButceUygulamaResI.BUM150 + "! (" + ButceUygulamaResI.BUM151 + ")";
        //            }
        //        }
        //        else
        //        {
        //            if (detay.yildizYer == 0)
        //            {
        //                sonuc.islemSonuc = false;
        //                sonuc.hataStr = ButceUygulamaResI.BUM152 + " (" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM153;
        //            }
        //            else
        //            {
        //                if (detay.tertip.TamMi())
        //                {
        //                    string kurAd = uzyServis.UzayDegeriStr(null, "ABSKUR", yil + "-" + (string)detay.tertip.kodlar[0], true, null);
        //                    string ekoAd = uzyServis.UzayDegeriStr(null, "ABSGELIRKODUYG", yil + "-" + (string)detay.tertip.kodlar[1], true, null);
        //                    if (kurAd == "" || ekoAd == "")
        //                    {
        //                        sonuc.islemSonuc = false;
        //                        sonuc.hataStr = ButceUygulamaResI.BUM154 + " (" + (string)detay.tertip.kodlar[0] + "/" + kurAd + " " + ButceUygulamaResI.BUM155 + "(" + (string)detay.tertip.kodlar[1] + "/" + ekoAd + ") " + ButceUygulamaResI.BUM156;
        //                    }
        //                    else
        //                    {
        //                        butceGelir.Kaydet(yil, detay.tertip.kodlar, 0, 0);
        //                        veriErisim.hata = "";
        //                    }
        //                }
        //                else
        //                {
        //                    sonuc.islemSonuc = false;
        //                    sonuc.hataStr = ButceUygulamaResI.BUM157 + " (" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM158;
        //                }
        //            }
        //        }
        //    }
        //    else // Bizim olmayan tertip
        //    {
        //        if (detay.tertip.TamMi())
        //        {
        //            string kurAd = uzyServis.UzayDegeriStr(null, "ABSKUR", yil + "-" + (string)detay.tertip.kodlar[0], true, null);
        //            string ekoAd = uzyServis.UzayDegeriStr(null, "ABSGELIRKODUYG", yil + "-" + (string)detay.tertip.kodlar[1], true, null);
        //            if (kurAd == "" || ekoAd == "")
        //            {
        //                sonuc.islemSonuc = false;
        //                sonuc.hataStr = ButceUygulamaResI.BUM154 + " (" + (string)detay.tertip.kodlar[0] + "/" + kurAd + " " + ButceUygulamaResI.BUM159 + "(" + (string)detay.tertip.kodlar[1] + "/" + ekoAd + ") " + ButceUygulamaResI.BUM160;
        //            }
        //        }
        //        else
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = ButceUygulamaResI.BUM161 + " (" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM162;
        //        }
        //    }
        //}

        //private static void HarcamaProgramiGelir(VeriErisim ve, Sonuc sonuc, int yil, string islemKod, BISatir detay, int ay, bool aktarim)
        //{
        //    if (sonuc.islemSonuc && !aktarim)
        //        HarcamaProgramiKontrol(sonuc, islemKod, detay);
        //    if (sonuc.islemSonuc && HarcamaProgramiOtomatikMi(islemKod))
        //    {
        //        if (detay.aylikProgram == null)
        //            detay.aylikProgram = new double[12];
        //        for (int i = 0; i < 12; i++)
        //            detay.aylikProgram[i] = 0;

        //        detay.aylikProgram[ay - 1] = detay.miktar;
        //    }
        //}

        //private static Sonuc GelirTertipleriTutarliMi(VeriErisim veriErisim, BIForm islem)
        //{
        //    Sonuc donusDeger = new Sonuc(false, "", "");
        //    TABButce butceGelir = new TABButceGelir(veriErisim);
        //    if (islem.gelirler != null)
        //    {
        //        for (int i = 0; i < islem.gelirler.Count; i++)
        //        {
        //            BISatir satir = (BISatir)islem.gelirler[i];
        //            double kbo = butceGelir.KBOBul(islem.yil, satir.tertip.kodlar);
        //            BITertipBilgi tertipBilgiOnayli = new BITertipBilgi();
        //            BITertipBilgi tertipBilgiOnaysiz = new BITertipBilgi();
        //            GelirTertipBilgiBul(veriErisim, islem.yil, satir.tertip.kodlar, tertipBilgiOnayli, tertipBilgiOnaysiz);
        //            double sonucKbo = kbo + tertipBilgiOnayli.eklenen - (tertipBilgiOnayli.dusulen + tertipBilgiOnaysiz.dusulen);
        //            if (sonucKbo < 0)
        //            {
        //                donusDeger.hataStr = satir.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM163 + " (" + Virgulle(satir.miktar, 2) + "), " + ButceUygulamaResI.BUM164 + "(" + Virgulle(sonucKbo, 2) + " " + ButceUygulamaResI.BUM165;
        //                return donusDeger;
        //            }

        //            if (islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))
        //            {
        //                TABISLAPGelir icmalRevize = new TABISLAPGelir(veriErisim);
        //                double icmalToplam = icmalRevize.ToplamBulRevize(islem.yil, satir.tertip.kodlar, false);
        //                if (icmalToplam > kbo)
        //                {
        //                    donusDeger.hataStr = ButceUygulamaResI.BUM166 + " " + satir.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM167 + " (" + Virgulle(icmalToplam, 0) + "), " + ButceUygulamaResI.BUM168 + " (" + Virgulle(kbo, 0) + ") " + ButceUygulamaResI.BUM169;
        //                    return donusDeger;
        //                }
        //            }


        //        }
        //    }
        //    donusDeger.islemSonuc = true;
        //    return donusDeger;
        //}

        //private static void YildizKontrolFinans(VeriErisim veriErisim, Sonuc sonuc, IUZYServis uzyServis, int yil, BISatir detay)
        //{
        //    string kurum, islemKod;
        //    int siraNo;

        //    TABButce butceFinans = new TABButceFinansman(veriErisim);
        //    TABISLFinansman islFinans = new TABISLFinansman(veriErisim);
        //    BITertipBilgi tertipBilgiOnayli = new BITertipBilgi();
        //    BITertipBilgi tertipBilgiOnaysiz = new BITertipBilgi();

        //    if (detay.tertip.kodlar[0] == VarsayilanKurum)
        //    {
        //        double kbo = butceFinans.KBOBul(yil, detay.tertip.kodlar);
        //        if (kbo > 0)
        //        {
        //            if (detay.yildizYer != 0)
        //            {
        //                sonuc.islemSonuc = false;
        //                sonuc.hataStr = ButceUygulamaResI.BUM170 + "(" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM171;
        //            }
        //            return;
        //        }
        //        if (islFinans.YildizVarMi(yil, detay.tertip.kodlar, out kurum, out islemKod, out siraNo))
        //        {
        //            if (detay.yildizYer != 0)
        //            {
        //                sonuc.islemSonuc = false;
        //                sonuc.hataStr = ButceUygulamaResI.BUM172 + "(" + ButceUygulamaResI.BUM173 + islemKod + " " + ButceUygulamaResI.BUM174 + siraNo.ToString() + ") ile açılmış bir finansman tertibini(" + detay.tertip.TertipStrOl() + "), " + ButceUygulamaResI.BUM175;
        //            }
        //        }
        //        else
        //        {
        //            if (detay.yildizYer == 0)
        //            {
        //                sonuc.islemSonuc = false;
        //                sonuc.hataStr = ButceUygulamaResI.BUM176 + " (" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM177;
        //            }
        //            else
        //            {
        //                if (detay.tertip.TamMi())
        //                {
        //                    string kurAd = uzyServis.UzayDegeriStr(null, "ABSKUR", yil + "-" + (string)detay.tertip.kodlar[0], true, null);
        //                    string ekoAd = uzyServis.UzayDegeriStr(null, "ABSFINANSMANKOD", yil + "-" + (string)detay.tertip.kodlar[1], true, null);
        //                    if (kurAd == "" || ekoAd == "")
        //                    {
        //                        sonuc.islemSonuc = false;
        //                        sonuc.hataStr = ButceUygulamaResI.BUM178 + " (" + (string)detay.tertip.kodlar[0] + "/" + kurAd + " " + ButceUygulamaResI.BUM179 + "(" + (string)detay.tertip.kodlar[1] + "/" + ekoAd + ") " + ButceUygulamaResI.BUM180;
        //                    }
        //                    else
        //                    {
        //                        butceFinans.Kaydet(yil, detay.tertip.kodlar, 0, 0);
        //                        veriErisim.hata = "";
        //                    }
        //                }
        //                else
        //                {
        //                    sonuc.islemSonuc = false;
        //                    sonuc.hataStr = ButceUygulamaResI.BUM181 + " (" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM182;
        //                }
        //            }
        //        }
        //    }
        //    else // Bizim olmayan tertip
        //    {
        //        if (detay.tertip.TamMi())
        //        {
        //            string kurAd = uzyServis.UzayDegeriStr(null, "ABSKUR", yil + "-" + (string)detay.tertip.kodlar[0], true, null);
        //            string ekoAd = uzyServis.UzayDegeriStr(null, "ABSFINANSMANKOD", yil + "-" + (string)detay.tertip.kodlar[1], true, null);
        //            if (kurAd == "" || ekoAd == "")
        //            {
        //                sonuc.islemSonuc = false;
        //                sonuc.hataStr = ButceUygulamaResI.BUM183 + " (" + (string)detay.tertip.kodlar[0] + "/" + kurAd + " " + ButceUygulamaResI.BUM184 + "(" + (string)detay.tertip.kodlar[1] + "/" + ekoAd + ") " + ButceUygulamaResI.BUM185;
        //            }
        //        }
        //        else
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = ButceUygulamaResI.BUM186 + " (" + detay.tertip.TertipStrOl() + ") " + ButceUygulamaResI.BUM187;
        //        }
        //    }
        //}

        //private static void HarcamaProgramiFinansman(VeriErisim ve, Sonuc sonuc, int yil, string islemKod, BISatir detay, int ay, bool aktarim)
        //{
        //    if (sonuc.islemSonuc && !aktarim)
        //        HarcamaProgramiKontrol(sonuc, islemKod, detay);
        //    if (sonuc.islemSonuc && HarcamaProgramiOtomatikMi(islemKod))
        //    {
        //        if (detay.aylikProgram == null)
        //            detay.aylikProgram = new double[12];
        //        for (int i = 0; i < 12; i++)
        //            detay.aylikProgram[i] = 0;

        //        detay.aylikProgram[ay - 1] = detay.miktar;
        //    }
        //}

        //private static Sonuc FinansmanTertipleriTutarliMi(VeriErisim veriErisim, BIForm islem)
        //{
        //    Sonuc donusDeger = new Sonuc(false, "", "");
        //    TABButce butceFinansman = new TABButceFinansman(veriErisim);
        //    if (islem.finanslar != null)
        //    {
        //        for (int i = 0; i < islem.finanslar.Count; i++)
        //        {
        //            BISatir satir = (BISatir)islem.finanslar[i];
        //            double kbo = butceFinansman.KBOBul(islem.yil, satir.tertip.kodlar);
        //            BITertipBilgi tertipBilgiOnayli = new BITertipBilgi();
        //            BITertipBilgi tertipBilgiOnaysiz = new BITertipBilgi();
        //            FinansmanTertipBilgiBul(veriErisim, islem.yil, satir.tertip.kodlar, tertipBilgiOnayli, tertipBilgiOnaysiz);
        //            double sonucKbo = kbo + tertipBilgiOnayli.eklenen - (tertipBilgiOnayli.dusulen + tertipBilgiOnaysiz.dusulen);
        //            if (sonucKbo < 0)
        //            {
        //                donusDeger.hataStr = satir.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM188 + " (" + Virgulle(satir.miktar, 2) + "), " + ButceUygulamaResI.BUM189 + "(" + Virgulle(sonucKbo, 2) + " " + ButceUygulamaResI.BUM190;
        //                return donusDeger;
        //            }

        //            if (islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))
        //            {
        //                TABISLAPFinansman icmalRevize = new TABISLAPFinansman(veriErisim);
        //                double icmalToplam = icmalRevize.ToplamBulRevize(islem.yil, satir.tertip.kodlar, false);
        //                if (icmalToplam > kbo)
        //                {
        //                    donusDeger.hataStr = ButceUygulamaResI.BUM191 + " " + satir.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM192 + " (" + Virgulle(icmalToplam, 0) + "), " + ButceUygulamaResI.BUM193 + " (" + Virgulle(kbo, 0) + ") " + ButceUygulamaResI.BUM194;
        //                    return donusDeger;
        //                }
        //            }

        //        }
        //    }
        //    donusDeger.islemSonuc = true;
        //    return donusDeger;
        //}

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Bütçe İşlemi bilgilerine göre,
        /// Bütçe İşlemini kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="aktarim">Aktarım yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc BIKaydet(VeriErisim veriErisim, Kullanici kullanan, BIForm islem, bool aktarim)
        {
            string hataMsg;
            IUZYServis uzyServis = BUMServis.TanimlaUZY();
            //aktarim = true;
            //islem.IcmalRevizeGelirYansit();

            if ((hataMsg = islem.HataliMi()) != "")
            {
                return new Sonuc(false, hataMsg, "");
            }
            if (!BUMYetkiServisi.YazmaYetkisiVarMi(kullanan, islem.yil, islem.kurum, -1))
            {
                return new Sonuc(false, ButceUygulamaResI.BUM195, "");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(islem.yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABISLForm islForm = new TABISLForm(ve);
            TABISLFormDayanak islFormDayanak = new TABISLFormDayanak(ve);
            islem.islemKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", islem.islemKod);

            if (islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE)))
            {
                int onayKod = islForm.ButceIslemiOnayKoduBul(islem.yil, islem.kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), 3);
                if (onayKod <= 2)
                {
                    ve.VeriErisimKapat();
                    return new Sonuc(false, ButceUygulamaResI.BUM196, "");
                }
            }
            else if (islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))
            {
                int onayKod = islForm.ButceIslemiOnayKoduBul(islem.yil, islem.kurum, BISabitler.IslemKod(ENUMIslemKod.IHP), 3);
                if (onayKod <= 2)
                {
                    ve.VeriErisimKapat();
                    return new Sonuc(false, ButceUygulamaResI.BUM197, "");
                }
            }

            object cevap;
            if (islem.siraNo != 0)
            {
                cevap = BIOku(ve, kullanan, islem.yil, islem.kurum, islem.islemKod, islem.siraNo, false);
                if (cevap.GetType() == typeof(Sonuc))
                {
                    ve.VeriErisimKapat();
                    return (Sonuc)cevap;
                }
                if (!((BIForm)cevap).YeniMi())
                {
                    ve.VeriErisimKapat();
                    return new Sonuc(false, ButceUygulamaResI.BUM198, "");
                }
                if (!BUMYetkiServisi.IslemeYazmaYetkisiVarMi(kullanan, ((BIForm)cevap).kullaniciKod))
                {
                    ve.VeriErisimKapat();
                    return new Sonuc(false, ButceUygulamaResI.BUM199, "");
                }
            }
            else
            {
                islem.siraNo = islForm.SiraNoBul(islem.yil, islem.kurum, islem.islemKod);
                cevap = null;
            }
            Sonuc sonuc = new Sonuc(true, "", "");
            if (!aktarim)
                FormKontrol(islem, sonuc);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }
            ve.BeginTransaction();
            if (cevap != null)
                sonuc = BISil(ve, (BIForm)cevap);
            if (sonuc.islemSonuc)
                sonuc = islForm.Kaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, islem.evrakTarih, islem.evrakNo, islem.ilgi, islem.notlar, (int)ENUMFormDurum.YENIKAYIT, islem.ilgiliIslemKod, islem.ilgiliSiraNo, -1, kullanan.kullaniciKodu, new TNSDateTime(System.DateTime.Now), islem.bumkoKurum, islem.bumkoSiraNo);
            if (sonuc.islemSonuc && islem.dayanaklar != null)
            {
                for (int i = 0; i < islem.dayanaklar.Count && sonuc.islemSonuc; i++)
                    sonuc = islFormDayanak.Kaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, ((BIDayanak)islem.dayanaklar[i]).dayanakKod, i + 1);
            }

            if (sonuc.islemSonuc)
            {
                if (!islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IPTAL)))
                {
                    if (sonuc.islemSonuc && islem.giderler != null)
                    {
                        TABISLGider islGider = new TABISLGider(ve);
                        TABISLAPGider islGiderAP = new TABISLAPGider(ve);
                        TABISLIHP islGiderIcmalAP = new TABISLIHP(ve);
                        for (int i = 0; i < islem.giderler.Count && sonuc.islemSonuc; i++)
                        {
                            BISatir detay = (BISatir)islem.giderler[i];
                            if (detay.tertip.tabloKod == ENUMTabloKod.TABButce || detay.tertip.tabloKod == ENUMTabloKod.TABPEBOdenek)
                            {
                                //if (!aktarim)
                                //    YildizKontrolGider(ve, sonuc, uzyServis, islem.yil, detay, BIForm.DevirIslemiMi(islem.islemKod));
                                //if (sonuc.islemSonuc)
                                //    sonuc = islGider.Kaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, detay.tertip.kodlar, detay.miktar, (int)ENUMFormDurum.YENIKAYIT, detay.yildizYer);
                                HarcamaProgramiGider(ve, sonuc, islem.yil, islem.islemKod, detay, System.Convert.ToDateTime(islem.evrakTarih.ToString()).Month, aktarim);
                                if (sonuc.islemSonuc && detay.aylikProgram != null)
                                {
                                    for (int ay = 0; ay < 12 && sonuc.islemSonuc; ay++)
                                    {
                                        double ayDeger = (double)detay.aylikProgram[ay];
                                        if (ayDeger != 0)
                                        {
                                            sonuc = islGiderAP.Kaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, detay.tertip.kodlar, ay + 1, Math.Floor(ayDeger), ENUMFormDurum.YENIKAYIT);
                                        }
                                    }
                                }
                            }
                            else if (detay.tertip.tabloKod == ENUMTabloKod.TABGiderIcmal)
                            {
                                if (!aktarim)
                                    YildizKontrolGiderIcmal(ve, sonuc, uzyServis, islem.yil, detay);
                                if (sonuc.islemSonuc && detay.aylikProgram != null)
                                {
                                    for (int ay = 0; ay < 12 && sonuc.islemSonuc; ay++)
                                    {
                                        double ayDeger = (double)detay.aylikProgram[ay];
                                        if (ayDeger != 0)
                                        {
                                            sonuc = islGiderIcmalAP.IHPKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, (string)detay.tertip.kodlar[1], ay + 1, ayDeger, (int)ENUMFormDurum.YENIKAYIT);
                                        }
                                    }
                                }

                            }

                            if (sonuc.islemSonuc && BIForm.DevirIslemiMi(islem.islemKod))
                            {
                                TABISLDevir devir = new TABISLDevir(ve);
                                sonuc = devir.Kaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, detay.tertip.kodlar, detay.devredenTertip.kodlar, detay.miktar, (int)ENUMFormDurum.YENIKAYIT);
                            }
                        }

                        if (sonuc.islemSonuc && !aktarim)
                        {
                            sonuc = TABButceyeYansit(ve, islem.yil, islem.islemKod, islem.giderler, +1, true);
                        }

                        if (sonuc.islemSonuc && !aktarim)
                        {
                            bool icmalKontrol = false;
                            //if (kullanan.kullaniciTipi.kod == (int)ENUMKullaniciTipi.DEVLETBUTCEUZMANI || kullanan.kullaniciTipi.kod == (int)ENUMKullaniciTipi.TNSUSTDUZEY)
                            //    icmalKontrol = false;
                            sonuc = GiderTertipleriTutarliMi(ve, islem, icmalKontrol, true, false);
                        }
                    }

                    //if (sonuc.islemSonuc && islem.gelirler != null)
                    //{
                    //    TABISLGelir islGelir = new TABISLGelir(ve);
                    //    TABISLAPGelir islGelirAP = new TABISLAPGelir(ve);
                    //    for (int i = 0; i < islem.gelirler.Count && sonuc.islemSonuc; i++)
                    //    {
                    //        BISatir detay = (BISatir)islem.gelirler[i];
                    //        if (!aktarim)
                    //            YildizKontrolGelir(ve, sonuc, uzyServis, islem.yil, detay);
                    //        if (sonuc.islemSonuc)
                    //            sonuc = islGelir.Kaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, detay.tertip.kodlar, detay.miktar, (int)ENUMFormDurum.YENIKAYIT, detay.yildizYer);
                    //        HarcamaProgramiGelir(ve, sonuc, islem.yil, islem.islemKod, detay, System.Convert.ToDateTime(islem.evrakTarih.ToString()).Month, aktarim);
                    //        if (sonuc.islemSonuc && detay.aylikProgram != null)
                    //        {
                    //            for (int ay = 0; ay < 12 && sonuc.islemSonuc; ay++)
                    //            {
                    //                double ayDeger = (double)detay.aylikProgram[ay];
                    //                if (ayDeger != 0)
                    //                {
                    //                    sonuc = islGelirAP.Kaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, detay.tertip.kodlar, ay + 1, Math.Floor(ayDeger), ENUMFormDurum.YENIKAYIT);
                    //                }
                    //            }
                    //        }
                    //    }

                    //    if (sonuc.islemSonuc && !aktarim)
                    //    {
                    //        sonuc = TABGelireYansit(ve, islem.yil, islem.islemKod, islem.gelirler, +1, true);
                    //    }

                    //    if (sonuc.islemSonuc && !aktarim)
                    //    {
                    //        sonuc = GelirTertipleriTutarliMi(ve, islem);
                    //    }
                    //}

                    //if (sonuc.islemSonuc && islem.finanslar != null)
                    //{
                    //    TABISLFinansman islFinans = new TABISLFinansman(ve);
                    //    TABISLAPFinansman islFinansAP = new TABISLAPFinansman(ve);
                    //    for (int i = 0; i < islem.finanslar.Count && sonuc.islemSonuc; i++)
                    //    {
                    //        BISatir detay = (BISatir)islem.finanslar[i];
                    //        if (!aktarim)
                    //            YildizKontrolFinans(ve, sonuc, uzyServis, islem.yil, detay);
                    //        if (sonuc.islemSonuc)
                    //            sonuc = islFinans.Kaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, detay.tertip.kodlar, detay.miktar, (int)ENUMFormDurum.YENIKAYIT, detay.yildizYer);
                    //        HarcamaProgramiFinansman(ve, sonuc, islem.yil, islem.islemKod, detay, System.Convert.ToDateTime(islem.evrakTarih.ToString()).Month, aktarim);
                    //        if (sonuc.islemSonuc && detay.aylikProgram != null)
                    //        {
                    //            for (int ay = 0; ay < 12 && sonuc.islemSonuc; ay++)
                    //            {
                    //                double ayDeger = (double)detay.aylikProgram[ay];
                    //                if (ayDeger != 0)
                    //                {
                    //                    sonuc = islFinansAP.Kaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, detay.tertip.kodlar, ay + 1, Math.Floor(ayDeger), ENUMFormDurum.YENIKAYIT);
                    //                }
                    //            }
                    //        }
                    //    }

                    //    if (sonuc.islemSonuc && !aktarim)
                    //    {
                    //        sonuc = TABFinansmanaYansit(ve, islem.yil, islem.islemKod, islem.finanslar, +1, true);
                    //    }

                    //    if (sonuc.islemSonuc && !aktarim)
                    //    {
                    //        sonuc = FinansmanTertipleriTutarliMi(ve, islem);
                    //    }
                    //}
                }
                else
                {
                    object ilgili = BIOku(ve, kullanan, islem.yil, islem.kurum, islem.ilgiliIslemKod, islem.ilgiliSiraNo, false);
                    if (ilgili.GetType() == typeof(Sonuc))
                        sonuc = (Sonuc)ilgili;
                    else
                    {
                        if (!((BIForm)ilgili).OnayliMi())
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = ButceUygulamaResI.BUM200;
                        }
                    }
                }
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = islem.siraNo.ToString();
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı ve Bütçe İşlem bilgilerine göre,
        /// Kullanıcının Bütçe İşlemini onaylayıp, onaylayamacağı bilgisini döndüren fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="bumkoOnayiMi">BÜMKO işlemlerini de onaylayıp onaylamayacağı bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onaylayabilecekse true onaylamayacak ise false bilgisini tutan Bool türünde nesne döndürür</returns>
        private static bool KullaniciIslemiOnaylayaBilirMi(Kullanici kullanan, BIForm islem, bool bumkoOnayiMi)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (bumkoOnayiMi)
                return true;
            if (islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AKTARMA)))
                return true;
            if (islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE)))
                return true;
            return false;
        }

        private static Sonuc TABButceyeYansitTaslaktan(VeriErisim veriErisim, int yil, string islemKod, TNSCollection giderler)
        {
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            TABButce butce = new TABButceGider(veriErisim);
            double serbest, eklenen, dusulen;
            Sonuc sonuc = new Sonuc(true, "", "");
            if (giderler != null)
            {
                for (int i = 0; i < giderler.Count && sonuc.islemSonuc; i++)
                {
                    BISatir detay = (BISatir)giderler[i];
                    if (TNS.BKD.Arac.KurumsaldanKuruma(detay.tertip.kodlar[0], tUzBul.Kirilimlar(0)) == VarsayilanKurum)
                    {
                        if (detay.tertip.tabloKod == ENUMTabloKod.TABButce)
                        {
                            int ahpSerbestAy = AHPDurum(yil, detay.tertip.kodlar[0], detay.tertip.kodlar[3]);
                            if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.SERBEST)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))
                            {
                                eklenen = 0;
                                dusulen = 0;
                                serbest = 0;
                                if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.SERBEST)) && ahpSerbestAy == 0)
                                    serbest = detay.eklenen_serbest;
                                else if (
                                    (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE))) && ahpSerbestAy > 0)
                                {
                                    for (int ay = 0; ay < detay.aylikProgram.Length; ay++)
                                    {
                                        if (ay < ahpSerbestAy)
                                            serbest += (double)detay.aylikProgram[ay];
                                    }
                                }
                            }
                            else
                            {
                                eklenen = detay.eklenen_serbest;
                                dusulen = detay.dusulen;
                                serbest = eklenen - dusulen;
                            }
                            sonuc = butce.TaslaktanOnaya(yil, detay.tertip.kodlar, eklenen, dusulen, serbest);
                        }
                    }
                }
            }
            return sonuc;
        }

        //private static Sonuc TABGelireYansitTaslaktan(VeriErisim veriErisim, int yil, string islemKod, TNSCollection gelirler)
        //{
        //    TABButce butceGelir = new TABButceGelir(veriErisim);
        //    double serbest, eklenen, dusulen;
        //    Sonuc sonuc = new Sonuc(true, "", "");
        //    if (gelirler != null)
        //    {
        //        for (int i = 0; i < gelirler.Count && sonuc.islemSonuc; i++)
        //        {
        //            BISatir detay = (BISatir)gelirler[i];
        //            if (TNS.BKD.Arac.KurumsaldanKuruma(detay.tertip.kodlar[0]) == VarsayilanKurum)
        //            {
        //                if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.ANAEKLEMEAKTARMA)))
        //                {
        //                    eklenen = detay.eklenen_serbest;
        //                    dusulen = detay.dusulen;
        //                    serbest = 0;
        //                    sonuc = butceGelir.TaslaktanOnaya(yil, detay.tertip.kodlar, eklenen, dusulen, serbest);
        //                }
        //            }
        //        }
        //    }

        //    return sonuc;
        //}

        //private static Sonuc TABFinansmanaYansitTaslaktan(VeriErisim veriErisim, int yil, string islemKod, TNSCollection finanslar)
        //{
        //    TABButce butceFinansman = new TABButceFinansman(veriErisim);
        //    double serbest, eklenen, dusulen;
        //    Sonuc sonuc = new Sonuc(true, "", "");
        //    if (finanslar != null)
        //    {
        //        for (int i = 0; i < finanslar.Count && sonuc.islemSonuc; i++)
        //        {
        //            BISatir detay = (BISatir)finanslar[i];
        //            if (TNS.BKD.Arac.KurumsaldanKuruma(detay.tertip.kodlar[0]) == VarsayilanKurum)
        //            {
        //                if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.ANAEKLEMEAKTARMA)))
        //                {
        //                    eklenen = detay.eklenen_serbest;
        //                    dusulen = detay.dusulen;
        //                    serbest = 0;
        //                    sonuc = butceFinansman.TaslaktanOnaya(yil, detay.tertip.kodlar, eklenen, dusulen, serbest);
        //                }
        //            }
        //        }
        //    }
        //    return sonuc;
        //}

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Bütçe İşlem Form bilgilerine göre,
        /// Kod atama işlemi yapan fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="yeniKod">Yeni kod bilgisini tutan, ENUMFormDurum türünde nesne</param>
        /// <param name="kontrolEtme">Kontrol edilip edilmeyeceği bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kod atama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc BIFormKodAta(VeriErisim ve, Kullanici kullanan, BIForm islem, ENUMFormDurum yeniKod, bool kontrolEtme)
        {
            Sonuc sonuc;
            kontrolEtme = true;
            TABISLGider islGider = new TABISLGider(ve);
            TABISLAPGider islGiderAP = new TABISLAPGider(ve);
            TABISLIHP islGiderIcmalAP = new TABISLIHP(ve);
            TABISLForm islForm = new TABISLForm(ve);
            TABISLDevir devir = new TABISLDevir(ve);

            sonuc = islForm.OnayKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, (int)yeniKod, -1, kullanan.kullaniciKodu, new TNSDateTime(System.DateTime.Now));
            if (!islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IPTAL)))
            {
                if (sonuc.islemSonuc)
                {
                    sonuc = islGider.OnayKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, (int)yeniKod);
                }

                if (sonuc.islemSonuc)
                {
                    sonuc = islGiderAP.OnayKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, (int)yeniKod);
                }

                if (sonuc.islemSonuc)
                {
                    sonuc = islGiderIcmalAP.OnayKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, (int)yeniKod);
                }
                if (sonuc.islemSonuc)
                {
                    sonuc = devir.OnayKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, (int)yeniKod);
                }

                TABISLGelir islGelir = new TABISLGelir(ve);
                TABISLAPGelir islGelirAP = new TABISLAPGelir(ve);

                if (sonuc.islemSonuc)
                {
                    sonuc = islGelir.OnayKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, (int)yeniKod);
                }

                if (sonuc.islemSonuc)
                {
                    sonuc = islGelirAP.OnayKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, (int)yeniKod);
                }

                TABISLFinansman islFinans = new TABISLFinansman(ve);
                TABISLAPFinansman islFinansAP = new TABISLAPFinansman(ve);

                if (sonuc.islemSonuc)
                {
                    sonuc = islFinans.OnayKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, (int)yeniKod);
                }

                if (sonuc.islemSonuc)
                {
                    sonuc = islFinansAP.OnayKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, (int)yeniKod);
                }

                if (kontrolEtme)
                {
                    bool kontrolEt = false;
                    if (islem.YeniMi())
                    {
                        if (yeniKod == ENUMFormDurum.ONAYLI)
                        {
                            kontrolEt = true;
                            if (sonuc.islemSonuc)
                            {
                                sonuc = TABButceyeYansitTaslaktan(ve, islem.yil, islem.islemKod, islem.giderler);
                            }

                            //if (sonuc.islemSonuc)
                            //{
                            //    sonuc = TABGelireYansitTaslaktan(ve, islem.yil, islem.islemKod, islem.gelirler);
                            //}

                            //if (sonuc.islemSonuc)
                            //{
                            //    sonuc = TABFinansmanaYansitTaslaktan(ve, islem.yil, islem.islemKod, islem.finanslar);
                            //}
                        }
                        else if (yeniKod == ENUMFormDurum.SILINDI || yeniKod == ENUMFormDurum.BUTCEISLEMIPTAL)
                        {
                            kontrolEt = true;
                            if (sonuc.islemSonuc)
                            {
                                sonuc = TABButceyeYansit(ve, islem.yil, islem.islemKod, islem.giderler, -1, true);
                            }

                            //if (sonuc.islemSonuc)
                            //{
                            //    sonuc = TABGelireYansit(ve, islem.yil, islem.islemKod, islem.gelirler, -1, true);
                            //}

                            //if (sonuc.islemSonuc)
                            //{
                            //    sonuc = TABFinansmanaYansit(ve, islem.yil, islem.islemKod, islem.finanslar, -1, true);
                            //}
                        }
                    }
                    else if (islem.OnayliMi())
                    {
                        if (yeniKod == ENUMFormDurum.SILINDI || yeniKod == ENUMFormDurum.BUTCEISLEMIPTAL)
                        {
                            kontrolEt = true;
                            if (sonuc.islemSonuc)
                            {
                                sonuc = TABButceyeYansit(ve, islem.yil, islem.islemKod, islem.giderler, -1, false);
                            }

                            //if (sonuc.islemSonuc)
                            //{
                            //    sonuc = TABGelireYansit(ve, islem.yil, islem.islemKod, islem.gelirler, -1, false);
                            //}

                            //if (sonuc.islemSonuc)
                            //{
                            //    sonuc = TABFinansmanaYansit(ve, islem.yil, islem.islemKod, islem.finanslar, -1, false);
                            //}
                        }
                    }
                    if (kontrolEt && !kontrolEtme)
                    {
                        if (sonuc.islemSonuc)
                        {
                            bool icmalKontrol = true;
                            //if (kullanan.kullaniciTipi.kod == (int)ENUMKullaniciTipi.DEVLETBUTCEUZMANI || kullanan.kullaniciTipi.kod == (int)ENUMKullaniciTipi.TNSUSTDUZEY)
                            //    icmalKontrol = false;
                            bool yuzdeYirmiKontroluYap = !kontrolEtme;
                            sonuc = GiderTertipleriTutarliMi(ve, islem, icmalKontrol, false, yuzdeYirmiKontroluYap);
                        }

                        //if (sonuc.islemSonuc)
                        //{
                        //    sonuc = GelirTertipleriTutarliMi(ve, islem);
                        //}

                        //if (sonuc.islemSonuc)
                        //{
                        //    sonuc = FinansmanTertipleriTutarliMi(ve, (BIForm)islem);
                        //}
                    }
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı ve İşlem bilgilerine göre,
        /// Bütçe İşlemini Onaylayan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="bumkoOnayiMi">Onaylamanın BUMKO'ya göre yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, onaylama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc BIOnayla(VeriErisim veriErisim, Kullanici kullanan, BIForm islem, bool bumkoOnayiMi)
        {
            if (!KullaniciIslemiOnaylayaBilirMi(kullanan, islem, bumkoOnayiMi))
                return new Sonuc(false, ButceUygulamaResI.BUM201, "");

            if (!BUMYetkiServisi.OnaylamaYetkisiVarMi(kullanan, islem.yil, islem.kurum, -1))
            {
                return new Sonuc(false, ButceUygulamaResI.BUM202, "");
            }

            if (islem.siraNo <= 0)
                return new Sonuc(false, ButceUygulamaResI.BUM203, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(islem.yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABISLForm islForm = new TABISLForm(ve);
            object cevap;

            cevap = BIOku(ve, kullanan, islem.yil, islem.kurum, islem.islemKod, islem.siraNo, false);
            if (cevap.GetType() == typeof(Sonuc))
            {
                ve.VeriErisimKapat();
                return (Sonuc)cevap;
            }
            if (!((BIForm)cevap).YeniMi())
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, ButceUygulamaResI.BUM204, "");
            }
            if (!BUMYetkiServisi.IslemiOnaylamaYetkisiVarMi(kullanan, ((BIForm)cevap).kullaniciKod, bumkoOnayiMi))
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, ButceUygulamaResI.BUM205, "");
            }
            ve.BeginTransaction();
            Sonuc sonuc = BIFormKodAta(ve, kullanan, (BIForm)cevap, ENUMFormDurum.ONAYLI, bumkoOnayiMi);
            if (islem.islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IPTAL)))
            {
                object ilgili = BIOku(ve, kullanan, islem.yil, islem.kurum, islem.ilgiliIslemKod, islem.ilgiliSiraNo, false);
                if (ilgili.GetType() == typeof(Sonuc))
                    sonuc = (Sonuc)ilgili;
                else
                {
                    if (((BIForm)ilgili).kullaniciKod == kullanan.kullaniciKodu || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) //???KullaniciTipi.BuyukEsitMi(kullanan.kullaniciTipi.kod, ((BIForm)ilgili).kullaniciTipKod))
                        sonuc = BIFormKodAta(ve, kullanan, (BIForm)ilgili, ENUMFormDurum.BUTCEISLEMIPTAL, bumkoOnayiMi);
                    else
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ((BIForm)ilgili).kullaniciKod + " " + ButceUygulamaResI.BUM206;
                    }
                }
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                BIFormDurum fd = new BIFormDurum((int)ENUMFormDurum.ONAYLI);
                sonuc.anahtar = fd.durumAd;
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı ve Bütçe İşlem Form bilgilerini göre,
        /// Bütçe İşlemini iptal eden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, iptal işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc BIIptal(VeriErisim veriErisim, Kullanici kullanan, BIForm islem)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(islem.yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABISLForm islForm = new TABISLForm(ve);
            object cevap;
            if (!BUMYetkiServisi.OnaylamaYetkisiVarMi(kullanan, islem.yil, islem.kurum, -1))
                return new Sonuc(false, ButceUygulamaResI.BUM207, "");

            if (islem.siraNo == 0)
                return new Sonuc(false, ButceUygulamaResI.BUM208, "");

            cevap = BIOku(ve, kullanan, islem.yil, islem.kurum, islem.islemKod, islem.siraNo, false);

            if (cevap.GetType() == typeof(Sonuc))
            {
                ve.VeriErisimKapat();
                return (Sonuc)cevap;
            }

            if (!((BIForm)cevap).YeniMi())
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, ButceUygulamaResI.BUM209, "");
            }

            if (!BUMYetkiServisi.IslemiOnaylamaYetkisiVarMi(kullanan, ((BIForm)cevap).kullaniciKod))
            {
                return new Sonuc(false, ButceUygulamaResI.BUM210, "");
            }

            ve.BeginTransaction();
            Sonuc sonuc = BIFormKodAta(ve, kullanan, (BIForm)cevap, ENUMFormDurum.SILINDI, true);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                BIFormDurum fd = new BIFormDurum((int)ENUMFormDurum.SILINDI);
                sonuc.anahtar = fd.durumAd;
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı ve Bütçe İşlem bilgisini kullanarak,
        /// BUMKO Kodu atayan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, atama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc BIBUMKOKodAta(VeriErisim veriErisim, Kullanici kullanan, BIForm islem)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(islem.yil), TNS.KYM.Arac.KimlikBul(kullanan));

            TABISLForm form = new TABISLForm(ve);

            ve.BeginTransaction();
            Sonuc sonuc = form.BUMKOKodAta(islem);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Tertip bilgisini parçalayarak,
        /// Dizi haline çeviren fonksiyon
        /// </summary>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="dizi">Dizi bilgisini tutan, TNSCollection türünde nesne</param>
        private static void DiziyeCevir(Tertip tertip, TNSCollection dizi)
        {
            dizi.Clear();
            for (int j = 0; j < tertip.kodlar.Length; j++)
            {
                string tertipStr = (string)tertip.kodlar[j];
                string[] strDizi = tertipStr.Split('.');
                for (int i = 0; i < strDizi.Length; i++)
                {
                    dizi.Add(strDizi[i]);
                }
                if (tertipStr.Length < ((string)tertip.Kirilimlar(j)).Length)
                {
                    string[] kirDizi = tertip.Kirilimlar(j).Split('.');
                    for (int i = 0; i < kirDizi.Length - strDizi.Length; i++)
                    {
                        dizi.Add("");
                    }
                }
            }
            //            string tertipStr=tertip.TertipStrOl().Replace("-",".");
            //            StringArray strDizi=new StringArray();
            //            OrtakFonksiyonlar.StringiParcala(tertipStr, ".", strDizi);
            //            dizi.Clear();
            //            for(int i=0; i<strDizi.str.Count; i++)
            //            {
            //                dizi.Add(strDizi.str[i]);
            //            }
        }

        private static void KodBolumOlustur(int[] sinifBoy, int kirilimYer, string kodParca, ref string kodBolum, ref int kodYer)
        {
            if (kirilimYer == 0)
                kodBolum = "";
            else
            {
                for (int j = 0; j < sinifBoy.Length - 1; j++)
                    if (kirilimYer == sinifBoy[j])
                    {
                        kodBolum = "";
                        break;
                    }
            }
            for (int j = 0; j < sinifBoy.Length; j++)
                if (kirilimYer < sinifBoy[j])
                {
                    kodYer = j;
                    break;
                }
            if (kodBolum != "") kodBolum += ".";
            kodBolum += kodParca;
        }

        private static void UcgenSatirEkle(VeriErisim ve, IUZYServis uzyServis, int yil, TNSCollection hedef, int diziBoy, TNSCollection diziP, int kodYer, double miktar, string kodBolum, int butceKodYer, int yildizYer)
        {
            UcgenSatir us = new UcgenSatir();
            us.yer = kodYer;
            us.kodParca = (string)diziP[kodYer];
            string uzay = "";
            if (butceKodYer == 1)
                uzay = "ABSKUR";
            else if (butceKodYer == 2)
                uzay = "ABSFON";
            else if (butceKodYer == 3)
                uzay = "ABSFIN";
            else if (butceKodYer == 4)
                uzay = "ABSEKO";
            else if (butceKodYer == 5)
                uzay = "ABSGELIRKODUYG";
            else if (butceKodYer == 6)
                uzay = "ABSFINANSMANKOD";
            if (uzay != "")
                try
                {
                    us.aciklama = uzyServis.UzayDegeriStr(null, uzay, yil + "-" + kodBolum, true);
                }
                catch
                {
                    us.aciklama = "";
                }
            else
                us.aciklama = "";
            if (kodYer >= diziBoy - 1)
            {
                us.ekDegerler.Add(yildizYer);
                if (miktar > 0)
                {
                    us.ekDegerler.Add((double)0);
                    us.ekDegerler.Add(miktar);
                }
                else
                {
                    us.ekDegerler.Add((double)0 - miktar);
                    us.ekDegerler.Add((double)0);
                }
            }
            hedef.Add(us);
        }

        /// <summary>
        /// Parametre olarak verilen Uzay Servisi, Yıl, Kaynak, Hedef ve Tertip tür bilgilerini kullanarak,
        /// Kaynak dizide bulunan tertipleri hiyerarşik bazda kırılımlarını sağlayarak Hedef diziye atayan fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="uzyServis">Uzay servisini tutan, IUZYServis türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutna, Int Türünde nesne</param>
        /// <param name="kaynak">Kaynak dizi bilgisini tutan, TNSCollection türünde nesne</param>
        /// <param name="hedef">Hedef dizi bilgisin tutan, TNSCollection türünde nesne</param>
        /// <param name="tertipTur">Tertip tür bilgisini tutan, Tertip türünde nesne</param>
        private static void UcgeneCevir(VeriErisim ve, IUZYServis uzyServis, int yil, TNSCollection kaynak, TNSCollection hedef, Tertip tertipTur)
        {
            if (kaynak.Count > 0)
            {
                string kurKod = ((string)((BISatir)kaynak[0]).tertip.kodlar[0]);

                int[] sinifBoy = new int[tertipTur.kodlar.Length];
                for (int i = 0; i < sinifBoy.Length; i++)
                {
                    int onceki;
                    if (i == 0) onceki = 0;
                    else onceki = sinifBoy[i - 1];
                    sinifBoy[i] = onceki + tertipTur.Kirilimlar(i).Split('.').Length;
                }

                TNSCollection eskiDizi = new TNSCollection();
                TNSCollection yeniDizi = new TNSCollection();
                TNSCollection eskiDiziP, yeniDiziP;
                BISatir ozetDetay = (BISatir)kaynak[0];

                DiziyeCevir(ozetDetay.tertip, eskiDizi);
                string kodBolum = "";
                int kodYer = 0;
                for (int i = 0; i < eskiDizi.Count; i++)
                {
                    KodBolumOlustur(sinifBoy, i, (string)eskiDizi[i], ref kodBolum, ref kodYer);
                    UcgenSatirEkle(ve, uzyServis, yil, hedef, eskiDizi.Count, eskiDizi, i, ozetDetay.miktar, kodBolum, (int)tertipTur.KodNolar(kodYer), ozetDetay.yildizYer);
                }
                eskiDiziP = eskiDizi;
                yeniDiziP = yeniDizi;
                for (int satir = 1; satir < kaynak.Count; satir++)
                {
                    ozetDetay = (BISatir)kaynak[satir];
                    DiziyeCevir(ozetDetay.tertip, yeniDiziP);
                    int yer;
                    kodBolum = "";
                    for (yer = 0; yer < yeniDiziP.Count && (string)yeniDiziP[yer] == (string)eskiDiziP[yer]; yer++)
                        KodBolumOlustur(sinifBoy, yer, (string)yeniDiziP[yer], ref kodBolum, ref kodYer);
                    for (; yer < yeniDiziP.Count; yer++)
                    {
                        KodBolumOlustur(sinifBoy, yer, (string)yeniDiziP[yer], ref kodBolum, ref kodYer);
                        UcgenSatirEkle(ve, uzyServis, yil, hedef, yeniDiziP.Count, yeniDiziP, yer, ozetDetay.miktar, kodBolum, (int)tertipTur.KodNolar(kodYer), ozetDetay.yildizYer);
                    }

                    if (yeniDiziP == yeniDizi)
                    {
                        eskiDiziP = yeniDizi;
                        yeniDiziP = eskiDizi;
                    }
                    else
                    {
                        eskiDiziP = eskiDizi;
                        yeniDiziP = yeniDizi;
                    }
                }

            }
        }

        ///// <summary>
        ///// Parametre olarak verilen Kullanıcı, Yıl, Kurum, İşlem kodu ve Sıra numarası bilgilerine göre,
        ///// Bütçe İşlem bilgilerini tertip bazında hiyerarşik olarak döndüren fonksiyon
        ///// </summary>
        ///// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        ///// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        ///// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        ///// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        ///// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        ///// <param name="siraNo">Sıra numarası bilgisini tutan, Int türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgisini tutan Object türünde nesne döndürür</returns>
        //public static object BIOkuUcgen(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, string islemKod, int siraNo)
        //{
        //    if (BUMYetkiServisi.GormeYetkisiVarMi(kullanan, yil, kurum, -1))
        //    {
        //        IUZYServis uzyServis = BUMServis.TanimlaUZY();
        //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
        //        TABISLForm tabForm = new TABISLForm(ve);
        //        TABISLGider islGider = new TABISLGider(ve);
        //        IDataReader dr = tabForm.Oku(yil, kurum, islemKod, siraNo);
        //        if (dr != null)
        //        {
        //            if (dr.Read())
        //            {
        //                BIForm biForm = new BIForm(yil, kurum, islemKod, siraNo);
        //                BIFormOl(dr, biForm);
        //                if (!BUMYetkiServisi.IslemiGormeYetkisiVarMi(kullanan, biForm.formDurum.durumKod, biForm.kullaniciTipKod))
        //                    return new Sonuc(false, ButceUygulamaResI.BUM211, "");
        //                dr.Close();
        //                biForm.miktar = islGider.ToplamBul(yil, kurum, islemKod, biForm.siraNo);
        //                biForm.dayanaklar = new TNSCollection();
        //                if (!DayanaklariBul(ve, yil, kurum, islemKod, biForm.siraNo, biForm.dayanaklar))
        //                    return new Sonuc(false, ButceUygulamaResI.BUM212 + "\r\n" + ve.hata, "");

        //                TNSCollection giderler = new TNSCollection();
        //                if (!FormDetayGiderBul(ve, yil, kurum, islemKod, biForm.siraNo, giderler, false))
        //                    return new Sonuc(false, ButceUygulamaResI.BUM213 + "\r\n" + ve.hata, "");
        //                //TNSCollection gelirler = new TNSCollection();
        //                //if (!FormDetayGelirBul(ve, yil, kurum, islemKod, biForm.siraNo, gelirler, false))
        //                //    return new Sonuc(false, ButceUygulamaResI.BUM213 + "\r\n" + ve.hata, "");
        //                //TNSCollection finanslar = new TNSCollection();
        //                //if (!FormDetayFinansBul(ve, yil, kurum, islemKod, biForm.siraNo, finanslar, false))
        //                //    return new Sonuc(false, ButceUygulamaResI.BUM213 + "\r\n" + ve.hata, "");
        //                biForm.giderler = new TNSCollection();
        //                //biForm.gelirler = new TNSCollection();
        //                //biForm.finanslar = new TNSCollection();
        //                UcgeneCevir(ve, uzyServis, yil, giderler, biForm.giderler, new Tertip(yil, ENUMTabloKod.TABButce));
        //                //UcgeneCevir(ve, uzyServis, yil, gelirler, biForm.gelirler, new Tertip(yil, ENUMTabloKod.TABGelir));
        //                //UcgeneCevir(ve, uzyServis, yil, finanslar, biForm.finanslar, new Tertip(yil, ENUMTabloKod.TABFinansman));
        //                return biForm;
        //            }
        //            else
        //                return new Sonuc(false, ButceUygulamaResI.BUM214, "");
        //        }
        //        else
        //            return new Sonuc(false, ButceUygulamaResI.BUM214, "");
        //    }
        //    else
        //        return new Sonuc(false, ButceUygulamaResI.BUM215, "");
        //}

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum, Tablo kodu bilgilerine göre,
        /// Serbestleri hesaplayıp sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanici">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="tabloKod">Tablo kodu bilgisini tutan, ENUMTabloKod türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, serbest hesaplama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc SerbestHesapla(VeriErisim veriErisim, Kullanici kullanici, int yil, string kurum, ENUMTabloKod tabloKod)
        {
            if (!TNS.BKD.Arac.KurumMu(kurum))
                return new Sonuc(false, ButceUygulamaResI.BUM216, "");

            string[] ekoDizi = { "01", "02", "03.1", "03.2", "03.3", "03.4", "03.5", "03.6", "03.7", "03.8", "03.9", "04", "05", "06", "07", "08", "09" };
            if (kullanici != null)
            {
                if (!kullanici.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                    return new Sonuc(false, ButceUygulamaResI.BUM217, "");
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            TABButce tabButce = GenelKosul.TabButceOl(tabloKod, ve);

            Sonuc s = new Sonuc(true, "", "");
            string[] kodlar = { kurum, "", "", "" };
            if (ve.baglantiTip.Equals("OLEDB"))
            {
                Tertip tertip = new Tertip(yil, tabloKod, kurum);
                ObjectArray oa = TertipListesiBul(ve, yil, tertip, false);
                if (oa.sonuc.islemSonuc)
                {
                    ve.BeginTransaction();
                    int serbestAy;
                    double serbest;
                    double serbestTaslak;
                    TABAyrintiliProgram tabAP = GenelKosul.TabISLAPOl(tabloKod, ve);
                    for (int j = 0; j < oa.ObjeSayisi && s.islemSonuc; j++)
                    {
                        Tertip t = (Tertip)oa[j];
                        serbestAy = AHPDurum(yil, kurum, t.kodlar[3]);
                        serbest = tabAP.ToplamBulRevize(yil, t.kodlar, true, serbestAy, ENUMFormDurumGrup.ONAYLI);
                        serbestTaslak = tabAP.ToplamBulRevize(yil, t.kodlar, true, serbestAy, ENUMFormDurumGrup.YENI);
                        s = tabButce.SerbestleriYaz(yil, t.kodlar, serbest, serbestTaslak);
                    }
                    if (s.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                }
                else
                    s = oa.sonuc;
            }
            else
            {

                ve.BeginTransaction();
                for (int i = 0; i < ekoDizi.Length; i++)
                {
                    kodlar[3] = ekoDizi[i];
                    s = tabButce.SerbestleriHesapla(AHPDurum(yil, kurum, ekoDizi[i]), yil, kodlar);
                    if (!s.islemSonuc)
                        break;
                }
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Bütçe İşlem Form Durumu Grup listesini değerleri ve isimleriyle birlikte TNSCollection türünde döndüren fonksiyon.
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem Form Durumu Grupların bilgilerini tutan TNSCollection türünde nesne döndürür</returns>
        public TNSCollection BIFormDurumGrupListesi()
        {
            TNSCollection donusDeger = new TNSCollection();
            donusDeger.Add(new ABSKodDil(((int)ENUMFormDurumGrup.YENI).ToString(), ButceUygulamaResI.BUM218));
            donusDeger.Add(new ABSKodDil(((int)ENUMFormDurumGrup.ONAYLI).ToString(), ButceUygulamaResI.BUM219));
            donusDeger.Add(new ABSKodDil(((int)ENUMFormDurumGrup.YENI_ONAYLI).ToString(), ButceUygulamaResI.BUM220));
            donusDeger.Add(new ABSKodDil(((int)ENUMFormDurumGrup.IPTALLI).ToString(), ButceUygulamaResI.BUM221));
            donusDeger.Add(new ABSKodDil(((int)ENUMFormDurumGrup.HEPSI).ToString(), ButceUygulamaResI.BUM222));
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Tablo kodu bilgilerine göre,
        /// Bütçeleri hazırlayan ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="tabloKod">Tablo kodu bilgisini tutan, ENUMTablokod türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, bütçe hazırlama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc ButceleriHazirla(VeriErisim veriErisim, TNS.KYM.Kullanici kullanan, int yil, string kurum, ENUMTabloKod tabloKod)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                sonuc.DegerAta(ButceUygulamaResI.BUM223, "");
                return sonuc;
            }
            IBHMv2Servis bhmServis = TNS.BHMv2.Arac.Tanimla();
            Tertip kistasTertip = null;
            ENUMTabloKod icTabloKod = tabloKod;
            UcgenKodBilgileri ukb = new UcgenKodBilgileri();
            EkDegerParam[] ekBilgiler = new EkDegerParam[2];
            Tertip butceTertip = new Tertip(yil, tabloKod);
            if (tabloKod == ENUMTabloKod.TABButce)
            {
                icTabloKod = ENUMTabloKod.TABPEBOdenek;
            }
            else if (tabloKod == ENUMTabloKod.TABGelir)
            {
                icTabloKod = ENUMTabloKod.TABPEBGelir;
            }
            else if (tabloKod == ENUMTabloKod.TABFinansman)
            {
                icTabloKod = ENUMTabloKod.TABFinansman;
            }
            else
                sonuc.DegerAta(ButceUygulamaResI.BUM224, "");

            OdenekBilgi[] tertipler = null;
            if (sonuc.islemSonuc)
            {
                kistasTertip = new Tertip(yil, icTabloKod, kurum);
                for (int i = 0; i < butceTertip.KodNoSay(); i++)
                {
                    ukb.Add(new UcgenKodBilgi(butceTertip.KodNolar(i), butceTertip.KodNoKirilimSay(i)));
                }
                ekBilgiler[0] = new EkDegerParam(icTabloKod, 0, 0, "05.00");
                ekBilgiler[1] = new EkDegerParam(icTabloKod, 0, 0, "06.00");
                tertipler = bhmServis.ListeTertipOl(kullanan, kistasTertip, ukb, ekBilgiler, true);
                if (tertipler.Length == 0)
                    sonuc.DegerAta(ButceUygulamaResI.BUM225, "");
            }
            if (!sonuc.islemSonuc)
                return sonuc;

            TABButce butce = null;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            if (tabloKod == ENUMTabloKod.TABButce)
                butce = new TABButceGider(ve);
            else if (tabloKod == ENUMTabloKod.TABGelir)
                butce = new TABButceGelir(ve);
            else
                butce = new TABButceFinansman(ve);

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                sonuc = butce.Sil(yil, kistasTertip.kodlar);
                foreach (OdenekBilgi ob in tertipler)
                {
                    sonuc = butce.Kaydet(yil, ob.tertip.kodlar, (double)ob.ekRakamlar[0], (double)ob.ekRakamlar[1]);
                    if (!sonuc.islemSonuc) break;
                }
                //if (sonuc.islemSonuc)
                //{
                //    sonuc = butce.IslemdekiYenileriEkle(yil, kistasTertip.kodlar);
                //}
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        static TNSCollection AHPSerbestBilgi = null;
        private static void AHPSerbestBilgisiOku()
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, BUMServis.BaglantiSatiriBul(ButceIslemYili), TNS.KYM.Arac.KimlikBul(null));
            TABAHPSerbestAy tabAHPSerbestAy = new TABAHPSerbestAy(ve);

            if (AHPSerbestBilgi == null)
                AHPSerbestBilgi = new TNSCollection();
            else
                AHPSerbestBilgi.Clear();

            IDataReader dr = tabAHPSerbestAy.Oku(ButceIslemYili);
            if (dr != null)
            {
                while (dr.Read())
                {
                    SerbestBilgiSatir sbs = new SerbestBilgiSatir();
                    sbs.kurumKod = VeriErisim.DegerAl(dr, 1, "");
                    sbs.ekoKod = VeriErisim.DegerAl(dr, 2, "");
                    sbs.ay = VeriErisim.DegerAl(dr, 3, 0);
                    AHPSerbestBilgi.Add(sbs);
                }
                dr.Close();
            }
        }

        static ButceIslemServisi()
        {
            VarsayilanKurum = TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD");
            if (string.IsNullOrEmpty(VarsayilanKurum))
                throw new Exception(ButceUygulamaResI.BUM226);
            ButceIslemYili = DateTime.Now.Year;// OrtakFonksiyonlar.ConvertToInt(degServis.DegiskenDegerBul(0, "/BUTCEISLEM"), 0);
            AHPSerbestBilgisiOku();
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Serbest bilgilerine göre,
        /// Serbestleri hesaplayan ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="ahpSerbestBilgi">Serbest bilgilerini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, hesaplama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc SerbestleriHesapla(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, TNSCollection ahpSerbestBilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                ve.BeginTransaction();

                TABAHPSerbestAy tabAHPSerbestAy = new TABAHPSerbestAy(ve);

                sonuc = tabAHPSerbestAy.Sil(yil, kurum);

                for (int i = 0; sonuc.islemSonuc && i < ahpSerbestBilgi.Count; i++)
                {
                    SerbestBilgiSatir sbs = (SerbestBilgiSatir)ahpSerbestBilgi[i];
                    sonuc = tabAHPSerbestAy.Kaydet(yil, sbs.kurumKod, sbs.ekoKod, sbs.ay);
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    AHPSerbestBilgisiOku();
                    sonuc = SerbestHesapla(null, kullanan, yil, kurum, ENUMTabloKod.TABButce);
                }
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }
            else
                sonuc.DegerAta(ButceUygulamaResI.BUM227);
            return sonuc;
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="yil"></param>
        /// <param name="kodlar"></param>
        /// <param name="onayli"></param>
        /// <param name="onaysiz"></param>

        // Verilen yil, tertipler için onaylı ve onaysız serbest toplamını bulur.
        private void TenkisEdilenBul(VeriErisim veriErisim, int yil, TNSCollection kodlar, out double onayli, out double onaysiz)
        {
            onayli = 0;
            onaysiz = 0;
            TNS.MUH.OdenekDurumKriter odk = new TNS.MUH.OdenekDurumKriter();
            odk.ay = 0;
            odk.yil = yil;
            odk.birim = "";
            odk.ekoKodKirilim = 2;
            odk.konsolide = true;
            odk.tertip = new Tertip(ENUMTabloKod.TABButce);
            for (int i = 0; i < kodlar.Count; i++)
                odk.tertip.kodlar.Add(kodlar[i]);

            odk.odenekGoster = false;
            odk.harcamaGoster = false;
            odk.mifGoster = false;
            odk.odemeEmriGoster = false;
            odk.odenekGondermeGoster = false;
            odk.tenkisGoster = false;

            odk.tenkisGoster = true;

            TNS.MUH.OdenekDurumVAN odv = muhServis.OdenekDurum(odk);

            TNS.MUH.OdenekDurum o = (TNS.MUH.OdenekDurum)odv.odenekDurum[0];
            onayli = o.onayliTenkis;
            onaysiz = o.onaysizTenkis;
        }

        public string AklamaEklemeHPHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumKod)
        {
            string hata = "";
            //VeriErisim ve=VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));


            TNSCollection islemler = BIListele(null, kullanan, yil, kurumKod, "02", ENUMFormDurumGrup.HEPSI, true, null, false);
            for (int j = 0; j < islemler.Count; j++)
            {
                VeriErisim veYaz = VeriErisim.VeriErisimBul(null);
                BIForm kimlik = (BIForm)islemler[j];
                object o = BIOku(veYaz, kullanan, kimlik.yil, kimlik.kurum, kimlik.islemKod, kimlik.siraNo, false);
                if (o.GetType() == typeof(BIForm))
                {
                    Sonuc sonuc = new Sonuc(true, "", "");
                    TABISLGIDERAHP islGiderAP = new TABISLGIDERAHP(veYaz);

                    BIForm islem = (BIForm)o;
                    veYaz.BeginTransaction();
                    sonuc = islGiderAP.Sil(islem.yil, islem.kurum, islem.islemKod, islem.siraNo);
                    for (int i = 0; i < islem.giderler.Count && sonuc.islemSonuc; i++)
                    {
                        BISatir detay = (BISatir)islem.giderler[i];
                        HarcamaProgramiGider(veYaz, sonuc, islem.yil, islem.islemKod, detay, System.Convert.ToDateTime(islem.evrakTarih.ToString()).Month);
                        if (sonuc.islemSonuc && detay.aylikProgram != null)
                        {
                            for (int ay = 0; ay < 12 && sonuc.islemSonuc; ay++)
                            {
                                double ayDeger = (double)detay.aylikProgram[ay];
                                if (ayDeger != 0)
                                {
                                    sonuc = islGiderAP.AHPKaydet(islem.yil, islem.kurum, islem.islemKod, islem.siraNo, detay.tertip.kodlar, ay + 1, ayDeger, (int)islem.formDurum.durumKod);
                                }
                            }
                        }
                    }
                    if (sonuc.islemSonuc)
                        veYaz.Commit();
                    else
                    {
                        veYaz.Rollback();
                        hata += islem.kurum + "-" + islem.islemKod + "-" + islem.siraNo + " " + sonuc.hataStr + "\r\n";
                    }
                }
                veYaz.VeriErisimKapat();
            }
            //            ve.VeriErisimKapat();
            return hata;
        }
        /// <summary>
        /// Kullanıcının verdiği yıla göre dayanakları listelemesi sağlanır.
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="yil"></param>
        /// <returns></returns>
        public TNSCollectionVAN DayanakListele(VeriErisim veriErisim, int yil)
        {
            TNSCollectionVAN donusDeger = new TNSCollectionVAN();
            donusDeger.dizi = new TNSCollection();
            donusDeger.sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABISLDayanaklar tbDayanaklar = new TABISLDayanaklar(ve);
            IDataReader dr = tbDayanaklar.Listele(yil, 2);
            if (dr != null)
            {
                while (dr.Read())
                {
                    BIDayanak deger = new BIDayanak();
                    deger.dayanakKod = VeriErisim.DegerAl(dr, 1, 0);
                    deger.dayanakAd = VeriErisim.DegerAl(dr, 2, "");
                    deger.aciklama = VeriErisim.DegerAl(dr, 3, "");

                    donusDeger.dizi.Add(deger);
                }
                dr.Close();
            }
            else
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = "Veritabanından okuma yapılamadı!\r\n";
            }

            ve.VeriErisimKapat();
            return donusDeger;

        }

        public void devirKontrol(int yil, TNSCollection kodlar)
        {
            double kboDevir;
            if (yil == 2006)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BYES");
                TABEskiButce eskiButce = new TABEskiButce(ve);

                kboDevir = eskiButce.kboBul(yil - 1, (string)kodlar[0], (string)kodlar[1], (string)kodlar[2], (string)kodlar[3], true);
                if (kboDevir <= 0)
                    kboDevir = eskiButce.OdenekBul(yil - 1, (string)kodlar[0], (string)kodlar[1], (string)kodlar[2], (string)kodlar[3], false);
                ve.VeriErisimKapat();

            }
        }
*/


    }

}

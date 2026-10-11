using System;
using System.Data;
using System.Xml;
using System.Configuration;
using System.IO;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Telefon abone fatura bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCAboneFaturaTelekom
    {
        /// <summary>
        /// Oluşan hataların yazılacağı dosya adını tutan değişken
        /// </summary>
        public string hataDosyaAd = "AboneDosyaAktarimiTelefon" + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + ".txt";
        int faturaYil = 0;
        int faturaAy = 0;

        /// <summary>
        /// Arguman olarak verilen, fatura yıl ve fatura ay bilgisini kullanarak telefon abone fatura bilgisini Telekom'un gönderdiği xml dosyasından okur
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="baslik">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Aktar(Kullanici kullanan, AboneFaturaTelefon baslik)
        {
            Sonuc sonuc = new Sonuc();

            string filePath = ConfigurationManager.AppSettings.Get("HarcamaTelekomDosyaYol") + "";

            string yil = baslik.faturaYil.ToString();
            string ay = baslik.faturaAy.ToString();

            if (ay.Length == 1)
                ay = "0" + ay;
            if (!filePath.Trim().EndsWith("\\"))
                filePath += "\\";
            filePath += yil + ay + "\\" + yil + ay + ".XML";

            if (!System.IO.File.Exists(filePath))
            {
                OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + filePath + " adlı telekom dosyası bulunamadı. Lütfen ilgili telekom dosyasını hazırlayıp, belirtilen adrese kopyalayın.", false);
                sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00039, filePath);
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ObjectArray liste = readXmlDocumentToList(filePath);

            int kontrolAy = ((AboneFaturaTelefon)(liste.objeler[0])).faturaAy - 1;
            int kontrolYil = ((AboneFaturaTelefon)(liste.objeler[0])).faturaYil;
            if (kontrolAy == 0)
            {
                kontrolAy = 12;
                kontrolYil--;
            }

            if (kontrolAy == baslik.faturaAy && kontrolYil == baslik.faturaYil)
                sonuc = Kaydet(liste);
            else
            {
                OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + "XML dosyası seçilen Ay ve Yıla ait değildir. Lütfen işleminizi kontrol ediniz.", false);
                sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00040, kontrolAy, kontrolYil, baslik.faturaAy, baslik.faturaYil);
                sonuc.islemSonuc = false;
                return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura bilgisini veritabanından okur 
        /// ve fatura bilgisi bulunan kullanıcıları listeler ve bu kullanıcılara fatura bilgisini içeren 
        /// mail gönderir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="baslik">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        public void MailGonder(Kullanici kullanan, AboneFaturaTelefon baslik)
        {
            faturaYil = baslik.faturaYil;
            faturaAy = baslik.faturaAy;

            MailAt();
        }

        //clienttan gelen ; ayıraçlı liste içinde, Telekomdan gelen pdf dosyaları var ise ; ayıraçlı dosya listesini geri döndür
        /// <summary>
        /// Arguman olarak verilen kriterlere göre fatura dosya ismi üretir ve string olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yıl bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="faturaNolar">İşlem yapılacak fatura numaraları. ";" ile birbirinden ayrılır</param>
        /// <returns>Kriterlere uygun fatura dosya isimlerini ";" ile ayrılmış string olarak döner</returns>
        public string FaturaDosyaIsimVer(Kullanici kullanan, int faturaYil, int faturaAy, string faturaNolar)
        {
            string donenDeger = "";
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaTelekomDosyaYol") + "";
            string yil = faturaYil.ToString();
            string ay = faturaAy.ToString("00");
            if (!filePath.EndsWith("\\")) filePath += "\\";
            filePath += yil + ay + "\\";

            string[] nolar = faturaNolar.Split(';');
            string[] dosyalar = Directory.GetFiles(filePath);

            for (int i = 0; i < nolar.Length; i++)
            {
                string aboneNo = nolar[i];
                aboneNo = aboneNo.Insert(3, "-");

                for (int j = 0; j < dosyalar.Length; j++)
                {
                    if (dosyalar[j].IndexOf(aboneNo) > -1)
                    {
                        if (donenDeger != "") donenDeger += ";";
                        donenDeger += Path.GetFileName(dosyalar[j]);
                        break;
                    }
                }
            }

            return donenDeger;
        }

        //Client tan gelen adlı dosyayı oku ve clientta gönder
        /// <summary>
        /// Arguman olarak verilen dosyayı okur ve byte dizisi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yil bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="dosyaAd">İşlem yapılacak dosya adı bilgisi.</param>
        /// <returns>Okunan dosyayı byte dizisi olarak döner</returns>
        public byte[] FaturaDosyaVer(Kullanici kullanan, int faturaYil, int faturaAy, string dosyaAd)
        {
            byte[] donenDeger = { };
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaTelekomDosyaYol") + "";
            string yil = faturaYil.ToString();
            string ay = faturaAy.ToString("00");
            if (!filePath.EndsWith("\\")) filePath += "\\";
            filePath += yil + ay + "\\";

            string pdfAd = filePath + dosyaAd;

            if (System.IO.File.Exists(pdfAd))
            {
                FileInfo fInfo = new FileInfo(pdfAd);
                long numBytes = fInfo.Length;
                double dLen = Convert.ToDouble(fInfo.Length / 1000000);
                if (dLen < 4)
                {
                    FileStream fStream = new FileStream(pdfAd, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fStream);

                    donenDeger = br.ReadBytes((int)numBytes);
                    br.Close();
                    fStream.Close();
                    fStream.Dispose();
                }
            }

            return donenDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura bilgisini veritabanından okur 
        /// ve AboneFaturaTelefon nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="faturaKriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> verilirse süreç oluşturulmamış telekom fatura bilgisi listelenir.</param>
        /// <returns>İstenilen kriterdeki, telekom fatura bilgisi listesini AboneFaturaTelefon nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(AboneFaturaTelefon faturaKriter, bool sadeceYeniler)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
            TABAboneFaturaTelekom tab = new TABAboneFaturaTelekom(ve);

            IDataReader dr = tab.Listele(faturaKriter, sadeceYeniler);
            AboneFaturaTelefon fatura = new AboneFaturaTelefon();
            while (dr.Read())
            {
                fatura = new AboneFaturaTelefon();

                fatura.faturaYil = VeriErisim.DegerAl(dr, 0, 0);
                fatura.faturaAy = VeriErisim.DegerAl(dr, 1, 0);
                fatura.hizmetNo = VeriErisim.DegerAl(dr, 2, "");
                fatura.faturaNo = VeriErisim.DegerAl(dr, 3, "");
                fatura.siraNo = VeriErisim.DegerAl(dr, 4, 0);
                fatura.tcKimlikNo = VeriErisim.DegerAl(dr, 5, "");
                fatura.vkn = VeriErisim.DegerAl(dr, 6, "");
                fatura.adSoyadUnvan = VeriErisim.DegerAl(dr, 7, "");
                fatura.adres = VeriErisim.DegerAl(dr, 8, "");
                fatura.docNo = VeriErisim.DegerAl(dr, 9, "");
                fatura.tarih = VeriErisim.DegerAl(dr, 10);
                fatura.sonOdemeTarih = VeriErisim.DegerAl(dr, 11);
                fatura.aciklama = VeriErisim.DegerAl(dr, 12, "");
                fatura.paraBirimKod = VeriErisim.DegerAl(dr, 13, "");
                fatura.malHizTopTutar = (decimal)VeriErisim.DegerAl(dr, 14, 0.0);
                fatura.topIskonto = (decimal)VeriErisim.DegerAl(dr, 15, 0.0);
                fatura.kdvOran = (decimal)VeriErisim.DegerAl(dr, 16, 0.0);
                fatura.kdvTutar = (decimal)VeriErisim.DegerAl(dr, 17, 0.0);
                fatura.kdvTopTutar = (decimal)VeriErisim.DegerAl(dr, 18, 0.0);
                fatura.digVerFonKod = VeriErisim.DegerAl(dr, 19, "");
                fatura.digVerFonOran = (decimal)VeriErisim.DegerAl(dr, 20, 0.0);
                fatura.digVerFonTutar = (decimal)VeriErisim.DegerAl(dr, 21, 0.0);
                fatura.dvfTopTutar = (decimal)VeriErisim.DegerAl(dr, 22, 0.0);
                fatura.fatTopTutar = (decimal)VeriErisim.DegerAl(dr, 23, 0.0);
                fatura.odnTutar = (decimal)VeriErisim.DegerAl(dr, 24, 0.0);
                fatura.aylikUcret = (decimal)VeriErisim.DegerAl(dr, 25, 0.0);
                fatura.gorusme = (decimal)VeriErisim.DegerAl(dr, 26, 0.0);
                fatura.kayitli = (decimal)VeriErisim.DegerAl(dr, 27, 0.0);
                fatura.ayrinti = (decimal)VeriErisim.DegerAl(dr, 28, 0.0);
                fatura.fonotel = (decimal)VeriErisim.DegerAl(dr, 29, 0.0);
                fatura.gecikme = (decimal)VeriErisim.DegerAl(dr, 30, 0.0);
                fatura.kapatilma = (decimal)VeriErisim.DegerAl(dr, 31, 0.0);
                fatura.baglanti = (decimal)VeriErisim.DegerAl(dr, 32, 0.0);
                fatura.faturaEdilmemis = (decimal)VeriErisim.DegerAl(dr, 33, 0.0);
                fatura.cesitli = (decimal)VeriErisim.DegerAl(dr, 34, 0.0);
                fatura.damga = (decimal)VeriErisim.DegerAl(dr, 35, 0.0);
                fatura.sehirIci = (decimal)VeriErisim.DegerAl(dr, 36, 0.0);
                fatura.sehirDisi = (decimal)VeriErisim.DegerAl(dr, 37, 0.0);
                fatura.milletlerArasi = (decimal)VeriErisim.DegerAl(dr, 38, 0.0);
                fatura.ucretliServisler = (decimal)VeriErisim.DegerAl(dr, 39, 0.0);
                fatura.internet = (decimal)VeriErisim.DegerAl(dr, 40, 0.0);
                fatura.gsm = (decimal)VeriErisim.DegerAl(dr, 41, 0.0);
                fatura.sesliMesajAylikUcret = (decimal)VeriErisim.DegerAl(dr, 42, 0.0);
                fatura.smsUcret = (decimal)VeriErisim.DegerAl(dr, 43, 0.0);
                fatura.sesliMesajUcreti = (decimal)VeriErisim.DegerAl(dr, 44, 0.0);
                fatura.aileKartUcreti = (decimal)VeriErisim.DegerAl(dr, 45, 0.0);
                fatura.cihazUcreti = (decimal)VeriErisim.DegerAl(dr, 46, 0.0);
                fatura.muhasebe = VeriErisim.DegerAl(dr, 47, 0);
                fatura.birim = VeriErisim.DegerAl(dr, 48, "");
                fatura.mail = VeriErisim.DegerAl(dr, 49, "");

                bilgi.Ekle(fatura);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Telekom fatura bilgisi varsa Sonuc nesnesinin islemSonuc özelliği true olarak gelir.
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KayitVarMi(AboneFaturaTelefon kriter)
        {
            Sonuc sonuc = new Sonuc();
            ObjectArray bilgi = Listele(kriter, false);
            if (bilgi.objeler.Count > 0)
                sonuc.islemSonuc = true;
            else
                sonuc.islemSonuc = false;

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura istatistik bilgisini veritabanından okur 
        /// ve TelekomIstatistik nesnesi olarak döner
        /// </summary>
        /// <param name="aboneTip">İşlem yapılacak abone tip bilgisi.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, telekom fatura istatistik bilgisini TelekomIstatistik nesnesi olarak tutan nesne</returns>
        public TelekomIstatistik ListeleTelekomIstatistik(ENUMAboneTip aboneTip, int yil, int muhasebe, string birim)
        {
            TelekomIstatistik bilgi = new TelekomIstatistik();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABAboneFaturaTelekom to = new TABAboneFaturaTelekom(ve);
            IDataReader r = to.ListeleTelekomIstatistik(aboneTip, yil, muhasebe, birim);

            if (to.Hata() == "")
            {
                decimal toplamTutar = 0;
                int toplamFaturaSayi = 0;
                int sayi = 0;
                while (r.Read())
                {
                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.ay = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.muhasebeKod = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.birimKod = VeriErisim.DegerAl(r, 3, "");
                    toplamTutar += VeriErisim.DegerAl(r, 4, 0);
                    toplamFaturaSayi += VeriErisim.DegerAl(r, 5, 0);
                    sayi++;
                }

                bilgi.toplamTutar = toplamTutar;
                bilgi.toplamFaturaSayi = toplamFaturaSayi;
                decimal ortAboneSayi = toplamFaturaSayi / sayi;
                bilgi.ortAboneSayi = OrtakFonksiyonlar.ConvertToInt(ortAboneSayi, 0);
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }


        private Sonuc Kaydet(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
            TABAboneFaturaTelekom tab = new TABAboneFaturaTelekom(ve);

            ve.BeginTransaction();

            int faturaSira = 0;
            foreach (AboneFaturaTelefon fatura in list.objeler)
            {
                faturaSira++;
                fatura.siraNo = faturaSira;
                fatura.faturaAy = fatura.faturaAy - 1;//TELEKOM FATURASI GELEN AYDAN BİR ÖNCEKİ AY OLMALI
                if (fatura.faturaAy == 0)
                {
                    fatura.faturaAy = 12;
                    fatura.faturaYil = fatura.faturaYil - 1;
                }
                sonuc = tab.Kaydet(fatura);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + sonuc.hataStr, false);
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private ObjectArray MailListele(AboneFaturaTelefon faturaKriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
            TABAboneFaturaTelekom tab = new TABAboneFaturaTelekom(ve);

            IDataReader dr = tab.MailIcinListe(faturaKriter);
            AboneFaturaTelefon fatura = new AboneFaturaTelefon();
            while (dr.Read())
            {
                fatura = new AboneFaturaTelefon();

                fatura.muhasebe = VeriErisim.DegerAl(dr, 0, 0);
                fatura.birim = VeriErisim.DegerAl(dr, 1, "");
                fatura.mail = VeriErisim.DegerAl(dr, 2, "");
                fatura.odemeEmriNo = VeriErisim.DegerAl(dr, 3, "");
                fatura.surecNo = VeriErisim.DegerAl(dr, 4, "");
                fatura.hizmetNo = VeriErisim.DegerAl(dr, 5, "");
                fatura.sonOdemeTarih = VeriErisim.DegerAl(dr, 6);
                fatura.odnTutar = (decimal)VeriErisim.DegerAl(dr, 7, 0.0);

                bilgi.Ekle(fatura);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        private void DosyaEkle(AboneFaturaTelefon fatura, TNSCollection pdfler, string filePath)
        {
            string pdfAd = "";
            string aboneNo = fatura.hizmetNo.Insert(3, "-");
            string faturaYil = fatura.faturaYil.ToString();
            string faturaAy = fatura.faturaAy.ToString();
            if (faturaAy.Length == 1) faturaAy = "0" + faturaAy;
            pdfAd = filePath + "\\" + aboneNo + "_" + faturaAy + "_" + faturaYil + ".pdf";
            if (System.IO.File.Exists(pdfAd))
                pdfler.Add(pdfAd);
        }

        private ObjectArray readXmlDocumentToList(string filePath)
        {
            XmlDocument myXmlDocument = new XmlDocument();
            myXmlDocument.Load(filePath);

            XmlNode node;
            node = myXmlDocument.DocumentElement;

            ObjectArray faturaList = new ObjectArray();
            AboneFaturaTelefon fatura = new AboneFaturaTelefon();
            TelekomFaturaDetay faturaDetay = new TelekomFaturaDetay();

            int sira = 0;
            foreach (XmlNode node1 in node.ChildNodes)
            {
                fatura = new AboneFaturaTelefon();
                fatura.siraNo = sira++;
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    if (node2.Name == "Alici")
                    {
                        foreach (XmlNode node3 in node2.ChildNodes)
                        {
                            if (node3.Name == "VKN")
                                fatura.vkn = node3.InnerText;
                            if (node3.Name == "TCKN")
                                fatura.tcKimlikNo = node3.InnerText;
                            if (node3.Name == "AdSoyadUn")
                                fatura.adSoyadUnvan = node3.InnerText;
                            if (node3.Name == "Adres")
                                fatura.adres = node3.InnerText;
                            if (node3.Name == "HizmetNo")
                                fatura.hizmetNo = node3.InnerText.Replace("-", "");
                            if (node3.Name == "DocNo")
                                fatura.docNo = node3.InnerText;
                        }
                    }
                    if (node2.Name == "Bilgiler")
                    {
                        foreach (XmlNode node3 in node2.ChildNodes)
                        {
                            if (node3.Name == "Tarih")
                            {
                                fatura.tarih = new TNSDateTime(node3.InnerText);
                                //Telekom faturalarının okunduğu XML dosyasından istemciden gelen ay ve yıl bilgilerinin 1 ay eksiği olduğu için
                                TNSDateTime dt = new TNSDateTime("01." + fatura.tarih.Ay + "." + fatura.tarih.Yil);
                                fatura.faturaAy = dt.Oku().AddMonths(1).Month;
                                fatura.faturaYil = dt.Oku().AddMonths(1).Year;
                            }
                            if (node3.Name == "SonOdemeTarih")
                                fatura.sonOdemeTarih = new TNSDateTime(node3.InnerText);
                            if (node3.Name == "Id")
                                fatura.faturaNo = node3.InnerText;
                            if (node3.Name == "Aciklama")
                                fatura.aciklama = node3.InnerText;
                        }
                    }

                    if (node2.Name == "Tutarlar")
                    {
                        foreach (XmlNode node3 in node2.ChildNodes)
                        {
                            if (node3.Name == "ParaBirimKod")
                                fatura.paraBirimKod = node3.InnerText;
                            if (node3.Name == "MalHizTopTutar")
                                fatura.malHizTopTutar = stringToDecimal(node3.InnerText);
                            if (node3.Name == "TopIskonto")
                                fatura.topIskonto = stringToDecimal(node3.InnerText);
                            if (node3.Name == "KDV")
                            {
                                foreach (XmlNode node4 in node3.ChildNodes)
                                {
                                    if (node4.Name == "Oran")
                                        fatura.kdvOran = stringToDecimal(node4.InnerText);
                                    if (node4.Name == "Tutar")
                                        fatura.kdvTutar = stringToDecimal(node4.InnerText);
                                }
                            }
                            if (node3.Name == "KDVTopTutar")
                                fatura.kdvTopTutar = stringToDecimal(node3.InnerText);
                            if (node3.Name == "DigVerFonlar")
                            {
                                foreach (XmlNode node4 in node3.ChildNodes)
                                {
                                    if (node4.Name == "DigVerFon")
                                    {
                                        foreach (XmlNode node5 in node4.ChildNodes)
                                        {
                                            if (node5.Name == "Kod")
                                                fatura.digVerFonKod = node5.InnerText;
                                            if (node5.Name == "Oran")
                                                fatura.digVerFonOran = stringToDecimal(node5.InnerText);
                                            if (node5.Name == "Tutar")
                                                fatura.digVerFonTutar = stringToDecimal(node5.InnerText);
                                        }
                                    }
                                }
                            }

                            if (node3.Name == "DVFTopTutar")
                                fatura.dvfTopTutar = stringToDecimal(node3.InnerText);
                            if (node3.Name == "FatTopTutar")
                                fatura.fatTopTutar = stringToDecimal(node3.InnerText);
                            if (node3.Name == "OdnTutar")
                                fatura.odnTutar = stringToDecimal(node3.InnerText);
                            if (node3.Name == "AylikUcret")
                                fatura.aylikUcret = stringToDecimal(node3.InnerText);
                            if (node3.Name == "Gorusme")
                                fatura.gorusme = stringToDecimal(node3.InnerText);
                            if (node3.Name == "Kayitli")
                                fatura.kayitli = stringToDecimal(node3.InnerText);
                            if (node3.Name == "Ayrinti")
                                fatura.ayrinti = stringToDecimal(node3.InnerText);
                            if (node3.Name == "Fonotel")
                                fatura.fonotel = stringToDecimal(node3.InnerText);
                            if (node3.Name == "Gecikme")
                                fatura.gecikme = stringToDecimal(node3.InnerText);
                            if (node3.Name == "Kapatilma")
                                fatura.kapatilma = stringToDecimal(node3.InnerText);
                            if (node3.Name == "Baglanti")
                                fatura.baglanti = stringToDecimal(node3.InnerText);
                            if (node3.Name == "FaturaEdilmemis")
                                fatura.faturaEdilmemis = stringToDecimal(node3.InnerText);
                            if (node3.Name == "Cesitli")
                                fatura.cesitli = stringToDecimal(node3.InnerText);
                            if (node3.Name == "Damga")
                                fatura.damga = stringToDecimal(node3.InnerText);
                            if (node3.Name == "SehirIci")
                                fatura.sehirIci = stringToDecimal(node3.InnerText);
                            if (node3.Name == "SehirDisi")
                                fatura.sehirDisi = stringToDecimal(node3.InnerText);
                            if (node3.Name == "MilletlerArasi")
                                fatura.milletlerArasi = stringToDecimal(node3.InnerText);
                            if (node3.Name == "UcretliServisler")
                                fatura.ucretliServisler = stringToDecimal(node3.InnerText);
                            if (node3.Name == "Internet")
                                fatura.internet = stringToDecimal(node3.InnerText);
                            if (node3.Name == "GSM")
                                fatura.gsm = stringToDecimal(node3.InnerText);
                            if (node3.Name == "SesliMesajAylikUcret")
                                fatura.sesliMesajAylikUcret = stringToDecimal(node3.InnerText);
                            if (node3.Name == "SmsUcret")
                                fatura.smsUcret = stringToDecimal(node3.InnerText);
                            if (node3.Name == "SesliMesajUcreti")
                                fatura.sesliMesajUcreti = stringToDecimal(node3.InnerText);
                            if (node3.Name == "AileKartUcreti")
                                fatura.aileKartUcreti = stringToDecimal(node3.InnerText);
                            if (node3.Name == "CihazUcreti")
                                fatura.cihazUcreti = stringToDecimal(node3.InnerText);
                        }
                    }
                }
                faturaList.Ekle(fatura);
            }

            return faturaList;
        }

        private decimal stringToDecimal(string degerStr)
        {
            decimal retValue = 0;
            if (degerStr != "#error#")
            {
                try
                {
                    retValue = decimal.Parse(degerStr.Replace('.', ','));
                }
                catch { }

            }

            return retValue;
        }

        private void MailAt()
        {
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaTelekomDosyaYol") + "";
            string kimden = ConfigurationManager.AppSettings.Get("SMTPKullanici").Trim();
            string yazilimURL = ConfigurationManager.AppSettings.Get("yazilimURL").Trim();
            string yil = faturaYil.ToString();
            string ay = faturaAy.ToString("00");
            string konu = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00041, ay, yil);

            AboneFaturaTelefon baslik = new AboneFaturaTelefon();
            baslik.faturaYil = faturaYil;
            baslik.faturaAy = faturaAy;
            ObjectArray liste = MailListele(baslik);

            if (!filePath.EndsWith("\\")) filePath += "\\";
            filePath += yil + ay + "\\";

            int muhasebe = 0;
            string birim = "";

            ObjectArray mailListe = new ObjectArray();
            TelekomEmailListe tListe = new TelekomEmailListe();
            foreach (AboneFaturaTelefon fatura in liste.objeler)
            {
                if (muhasebe != fatura.muhasebe || birim != fatura.birim)
                {
                    if (tListe.muhasebe > 0)
                        mailListe.objeler.Add(tListe);

                    tListe = new TelekomEmailListe();
                    tListe.muhasebe = fatura.muhasebe;
                    tListe.birim = fatura.birim;
                    tListe.mail = fatura.mail;
                    tListe.odemeEmriNo = fatura.odemeEmriNo;
                    tListe.surecNo = fatura.surecNo;
                }

                AboneFaturaTelefon f = new AboneFaturaTelefon();
                f.faturaYil = faturaYil;
                f.faturaAy = faturaAy;
                f.hizmetNo = fatura.hizmetNo;
                f.sonOdemeTarih = fatura.sonOdemeTarih;
                f.odnTutar = fatura.odnTutar;
                tListe.fatura.objeler.Add(f);

                muhasebe = fatura.muhasebe;
                birim = fatura.birim;
            }
            if (tListe.muhasebe > 0)
                mailListe.objeler.Add(tListe);

            foreach (TelekomEmailListe fatura in mailListe.objeler)
            {
                double toplamFatura = 0;
                string telefonNo = "";
                TNSCollection pdfler = new TNSCollection();

                foreach (AboneFaturaTelefon ab in fatura.fatura.objeler)
                {
                    toplamFatura += OrtakFonksiyonlar.ConvertToDbl(ab.odnTutar);
                    if (telefonNo != "") telefonNo += ", ";
                    telefonNo += ab.hizmetNo;
                    DosyaEkle(ab, pdfler, filePath);
                }

                string odemeEmriLink = "<a href='" + yazilimURL + "/giris.aspx?modul=ButceMuhasebe&sayfa=OdemeEmriMifFrame.aspx&parametre=yil-" + DateTime.Now.Year + "-belgeNo-" + fatura.odemeEmriNo + "'>" + TNS.HRC.HarcamaSurecleriResI.HRC00035 + "</a>";
                string teyitLink = "<a href='" + yazilimURL + "/giris.aspx?modul=HarcamaSurecleri&sayfa=TanimEFaturaTeyit.aspx&parametre='>" + TNS.HRC.HarcamaSurecleriResI.HRC00042 + "</a>";
                string bilgi = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00043, faturaAy.ToString("00"), faturaYil, telefonNo, toplamFatura.ToString("#,###.00"), fatura.surecNo);

                if (fatura.odemeEmriNo != string.Empty)
                    bilgi += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00044, fatura.odemeEmriNo, odemeEmriLink);
                else
                    bilgi += TNS.HRC.HarcamaSurecleriResI.HRC00045;

                bilgi += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00046, teyitLink);

                string mail = fatura.mail + ";sgbnetBilgi@sgb.gov.tr";

                if (kimden.Trim() != "" && mail.Trim() != "")
                {
                    if (mail.IndexOf('@') != -1)
                    {
                        try
                        {
                            OrtakFonksiyonlar.MailAt(kimden, mail, konu, bilgi, true, false, pdfler);
                        }
                        catch (Exception e)
                        {
                            string dosyaAdi = "mailGonder" + DateTime.Now.Year.ToString() + "" + DateTime.Now.Month.ToString("00") + "" + DateTime.Now.Day.ToString("00") + "Hata.txt";
                            OrtakFonksiyonlar.HataDosyaYaz(dosyaAdi, DateTime.Now.ToShortDateString() + "<=Gönderilemedi=> <-" + mail + "-> -Hata Mesaji->" + e.Message, false);
                        }
                    }
                }
            }
        }
    }
}

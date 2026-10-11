using System;
using System.Data;
using System.Xml;
using System.Configuration;
using System.IO;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using HRCServisC.tr.gov.sgb.uygulama.su;

namespace TNS.HRC.IsKurallari
{
    //Yeni su servisi eklemek için
    //Aktar metodu içinde iller string arrayine kodu ekle
    //AktarIc metodu içinde yeni yazılacak su aktarma metodu çağrılacak
    //AktarISKI,AktarASKI benzeri metod yazılacak
    /// <summary>
    /// Su fatura bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCAboneFaturaSu
    {
        /// <summary>
        /// Oluşan hataların yazılacağı dosya adını tutan değişken
        /// </summary>
        public string hataDosyaAd = "AboneDosyaAktarimiSu" + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + ".txt";
        int faturaYil = 0;
        int faturaAy = 0;

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura bilgisini ilgili web servisden okur
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Aktar(Kullanici kullanan, int yil, int ay)
        {
            string[] iller = { "06", "16", "34" };
            for (int i = 0; i < iller.Length; i++)
            {
                AktarIc(kullanan, yil, ay, iller[i]);
            }

            return null;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve ilgili kullanıcılara fatura bilgisini mail olarak gönderir
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="baslik">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        public void MailGonder(Kullanici kullanan, AboneFaturaSu baslik)
        {
            faturaYil = baslik.faturaYil;
            faturaAy = baslik.faturaAy;

            MailAt();
        }

        //clienttan gelen ; ayıraçlı liste içinde, Sudan gelen pdf dosyaları var ise ; ayıraçlı dosya listesini geri döndür
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
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaSu3DosyaYol") + "";
            string yil = faturaYil.ToString();
            string ay = faturaAy.ToString("00");
            if (!filePath.EndsWith("\\")) filePath += "\\";
            filePath += yil + ay + "\\";

            string[] nolar = faturaNolar.Split(';');
            string[] dosyalar = Directory.GetFiles(filePath);

            for (int i = 0; i < nolar.Length; i++)
            {
                string aboneNo = nolar[i];

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
        /// <param name="faturaYil">İşlem yapılacak fatura yıl bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="dosyaAd">İşlem yapılacak dosya adı bilgisi.</param>
        /// <returns>Okunan dosyayı byte dizisi olarak döner</returns>
        public byte[] FaturaDosyaVer(Kullanici kullanan, int faturaYil, int faturaAy, string dosyaAd)
        {
            byte[] donenDeger = { };
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaSuDosyaYol") + "";
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
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura bilgisini veritabanından okur 
        /// ve AboneFaturaSu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="faturaKriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> süreç oluşturulmayan faturalar gelir</param>
        /// <returns>İstenilen kriterdeki, su fatura bilgisi listesini AboneFaturaSu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(AboneFaturaSu faturaKriter, bool sadeceYeniler)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
            TABAboneFaturaSu tab = new TABAboneFaturaSu(ve);

            IDataReader dr = tab.Listele(faturaKriter, sadeceYeniler);
            AboneFaturaSu fatura = new AboneFaturaSu();
            while (dr.Read())
            {
                fatura = new AboneFaturaSu();

                fatura.faturaYil = VeriErisim.DegerAl(dr, 0, 0);
                fatura.faturaAy = VeriErisim.DegerAl(dr, 1, 0);
                fatura.isletmeKodu = VeriErisim.DegerAl(dr, 2, "");
                fatura.hizmetNo = VeriErisim.DegerAl(dr, 3, "");
                fatura.aboneAd = VeriErisim.DegerAl(dr, 4, "");
                fatura.siraNo = VeriErisim.DegerAl(dr, 5, 0);
                fatura.faturaNo = VeriErisim.DegerAl(dr, 6, "");
                fatura.adresi = VeriErisim.DegerAl(dr, 7, "");
                fatura.tebligTarih = VeriErisim.DegerAl(dr, 8);
                fatura.sonOdemeTarih = VeriErisim.DegerAl(dr, 9);
                fatura.sayacNo = VeriErisim.DegerAl(dr, 10, "");
                fatura.ilkOkuma = VeriErisim.DegerAl(dr, 11);
                fatura.sonOkuma = VeriErisim.DegerAl(dr, 12);
                fatura.ilkEndex = VeriErisim.DegerAl(dr, 13, 0.0);
                fatura.sonEndex = VeriErisim.DegerAl(dr, 14, 0.0);
                fatura.tuketim = VeriErisim.DegerAl(dr, 15, 0.0);
                fatura.kiyas = VeriErisim.DegerAl(dr, 16, 0.0);
                fatura.ctvTutari = VeriErisim.DegerAl(dr, 17, 0.0);
                fatura.kdv = VeriErisim.DegerAl(dr, 18, 0.0);
                fatura.suBedeli = VeriErisim.DegerAl(dr, 19, 0.0);
                fatura.atikSu = VeriErisim.DegerAl(dr, 20, 0.0);
                fatura.subeYolu = VeriErisim.DegerAl(dr, 21, 0.0);
                fatura.fatTopTutar = VeriErisim.DegerAl(dr, 22, 0.0);
                fatura.tahsil = VeriErisim.DegerAl(dr, 23, 0.0);
                fatura.tenzil = VeriErisim.DegerAl(dr, 24, 0.0);
                fatura.ceza = VeriErisim.DegerAl(dr, 25, 0.0);
                fatura.kalan = VeriErisim.DegerAl(dr, 26, 0.0);
                fatura.muhasebe = VeriErisim.DegerAl(dr, 27, 0);
                fatura.birim = VeriErisim.DegerAl(dr, 28, "");
                fatura.mail = VeriErisim.DegerAl(dr, 29, "");

                bilgi.Ekle(fatura);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Su fatura bilgisi varsa Sonuc nesnesinin islemSonuc özelliği true olarak gelir.
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KayitVarMi(AboneFaturaSu kriter)
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
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura istatistik bilgisini veritabanından okur 
        /// ve SuIstatistik nesnesi olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, su fatura istatistik bilgisini SuIstatistik nesnesi olarak tutan nesne</returns>
        public SuIstatistik ListeleSuIstatistik(int yil, int muhasebe, string birim)
        {
            SuIstatistik bilgi = new SuIstatistik();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABAboneFaturaSu to = new TABAboneFaturaSu(ve);
            IDataReader r = to.ListeleSuIstatistik(yil, muhasebe, birim);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebeKod = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birimKod = VeriErisim.DegerAl(r, 2, "");
                    bilgi.toplamTutar = VeriErisim.DegerAl(r, 3, 0);
                    bilgi.toplamTuketim = VeriErisim.DegerAl(r, 4, 0);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }



        private void FaturaCiktisiOlustur(Kullanici kullanan, AboneFaturaSu fatura)
        {
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaSuDosyaYol") + "";
            string yil = fatura.faturaYil.ToString();
            string ay = fatura.faturaAy.ToString("00");

            if (!filePath.EndsWith("\\")) filePath += "\\";
            filePath += yil + ay + "\\";
            filePath += fatura.isletmeKodu.Replace(".", "") + "_" + fatura.hizmetNo + ".html";

            string faturaCiktiIcerik = FaturaCiktiIcerikOlustur(kullanan, fatura);

            OrtakFonksiyonlar.DosyaYaz(filePath, faturaCiktiIcerik, true);
        }

        private string FaturaFirmaAdGetir(Kullanici kullanan, string isletmeKod, string hizmetNo)
        {
            AboneNo a = new AboneNo();
            a.isletmeKodu = isletmeKod;
            a.aboneNo = hizmetNo;
            a.aboneTip = 1;
            ObjectArray firmaListe = new HRCAboneNo().Listele(kullanan, a);
            if (firmaListe.objeler.Count > 0)
                return ((AboneNo)firmaListe.objeler[0]).firma.ad;
            return "Sistemde Abone Numarasına ait Ödeme Yapılacak Kurum bilgisi bulunmamaktadır.";
        }

        private string FaturaCiktiIcerikOlustur(Kullanici kullanan, AboneFaturaSu fatura)
        {
            string donusDeger = "";
            donusDeger = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
            "<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
            "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\">" +
            "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-9\" />" +
            "<meta http-equiv=\"Content-Language\" content=\"tr\" />" +
            "<head><title>Su Tüketim Belgesi</title>" +
            "<style type=\"text/css\">.body{font-family:Arial; font-size:small;font-weight:bold;}.baslik{background-color: #FFFFCC;}</style></head>" +
            "<body class=\"body\">" +
            "<table border=\"1\" style=\"border-width:thin; border-color:Black;\">" +
                "<tr><td class=\"baslik\">ÖDEME YAPILACAK KURUM</td><td colspan=\"7\">" + fatura.firmaAd + "</td></tr>" +
                "<tr><td colspan=\"8\" class=\"baslik\"><div align=\"center\">SU FATURASI TÜKETİM BELGESİ</div></td></tr>" +
                "<tr><td class=\"baslik\">Fatura Tarihi</td><td colspan=\"7\">" + fatura.tebligTarih + "</td></tr>" +
                 "<tr><td class=\"baslik\">Son Ödeme Tarihi</td><td colspan=\"7\">" + fatura.sonOdemeTarih + "</td></tr>" +
                "<tr><td class=\"baslik\">Fatura No</td><td colspan=\"7\">" + fatura.faturaNo + "</td></tr>" +
                "<tr><td class=\"baslik\">Abone No</td><td colspan=\"7\">" + fatura.hizmetNo + "</td></tr>" +
                "<tr><td class=\"baslik\">Adı Soyadı</td><td colspan=\"7\">" + fatura.aboneAd + "</td></tr>" +
                "<tr><td class=\"baslik\">Adresi</td><td colspan=\"7\">" + fatura.adresi + "</td></tr>" +
                "<tr><td class=\"baslik\">Muhasebe Kodu / Birim Kodu</td><td colspan=\"7\">" + fatura.muhasebe + "/" + fatura.birim + "</td></tr>" +
                "<tr><td class=\"baslik\">Seri No</td><td colspan=\"7\"> </td></tr>" +
                "<tr><td class=\"baslik\">Sayaç No</td><td colspan=\"7\">" + fatura.sayacNo + "</td></tr>" +
                "<tr><td class=\"baslik\">İlk Endeks</td><td colspan=\"7\">" + fatura.ilkEndex + "</td></tr>" +
                "<tr><td class=\"baslik\">Son Endeks</td><td colspan=\"7\">" + fatura.sonEndex + "</td></tr>" +
                "<tr><td class=\"baslik\">Su Bedeli</td><td colspan=\"7\">" + fatura.suBedeli.ToString("#,###.00") + "</td></tr>" +
                "<tr><td class=\"baslik\">Atık Su Bedeli</td><td colspan=\"7\">" + fatura.atikSu.ToString("#,###.00") + "</td></tr>" +
                "<tr><td class=\"baslik\">Şube yolu bakım Bedeli</td><td colspan=\"7\">" + fatura.subeYolu.ToString("#,###.00") + "</td></tr>" +
                "<tr><td class=\"baslik\">Kdv Bedeli</td><td colspan=\"7\">" + fatura.kdv.ToString("#,###.00") + "</td></tr>" +
                "<tr><td class=\"baslik\">Ctv Bedeli</td><td colspan=\"7\">" + fatura.ctvTutari.ToString("#,###.00") + "</td></tr>" +
                "<tr><td class=\"baslik\"></td><td colspan=\"7\"></td></tr>" +
                "<tr><td class=\"baslik\">Genel Toplam :</td><td colspan=\"7\">" + fatura.fatTopTutar.ToString("#,###.00") + "</td></tr>" +
                "<tr><td class=\"baslik\"></td><td colspan=\"7\"></td></tr>" +
                "<tr><td class=\"baslik\">Ceza Bedeli</td><td colspan=\"7\">" + fatura.ceza.ToString("#,###.00") + "</td></tr>" +
                "</table><br /><br /></body></html>";
            return donusDeger;
        }

        //private Sonuc Kaydet(ObjectArray list)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABAboneFaturaSu tab = new TABAboneFaturaSu(ve);

        //    ve.BeginTransaction();

        //    int faturaSira = 0;
        //    foreach (AboneFaturaSu fatura in list.objeler)
        //    {
        //        faturaSira++;
        //        fatura.siraNo = faturaSira;
        //        sonuc = tab.Kaydet(fatura);
        //        if (!sonuc.islemSonuc)
        //            break;
        //    }

        //    if (sonuc.islemSonuc)
        //    {
        //        sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
        //        ve.Commit();
        //    }
        //    else
        //    {
        //        OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + sonuc.hataStr, false);
        //        sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011;
        //        ve.Rollback();
        //    }

        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        private Sonuc AktarIc(Kullanici kullanan, int yil, int ay, string ilKod)
        {
            Sonuc sonuc = new Sonuc();

            AboneNo abone = new AboneNo();
            abone.aboneTip = (int)ENUMAboneTip.SU;
            abone.durum = (int)ENUMAktiflik.AKTIF;
            abone.ilKodu = ilKod;

            //Aboneleri listele
            HRCAboneNo aboneler = new HRCAboneNo();
            ObjectArray aboneListe = aboneler.Listele(kullanan, abone);

            //Kayıt edilmiş olanları listele
            AboneFaturaSu kriter = new AboneFaturaSu();
            ObjectArray listeFatura = new ObjectArray();
            kriter.faturaYil = yil;
            kriter.faturaAy = ay;
            listeFatura = new HRCAboneFaturaSu().Listele(kriter, false);

            //Kayıt edilmemiş olanları yeni liste yap
            ObjectArray kontrolListe = new ObjectArray();
            foreach (AboneNo a in aboneListe.objeler)
            {
                bool bulundu = false;
                foreach (AboneFaturaSu abf in listeFatura.objeler)
                {
                    if (abf.isletmeKodu == a.isletmeKodu && abf.hizmetNo == a.aboneNo)
                        bulundu = true;
                }
                if (!bulundu)
                    kontrolListe.objeler.Add(a);
            }
            //******************************************

            if (ilKod == "06")
            {
                try { AktarASKI(kullanan, yil, ay, kontrolListe); }
                catch { };
            }
            else if (ilKod == "16")
            {
                try { AktarBUSKI(kullanan, yil, ay, kontrolListe); }
                catch { };
            }
            else if (ilKod == "34")
            {
                try { AktarISKI(kullanan, yil, ay, kontrolListe); }
                catch { };
            }
            return null;
        }

        private void AktarASKI(Kullanici kullanan, int yil, int ay, ObjectArray kontrolListe)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAboneFaturaSu tab = new TABAboneFaturaSu(ve);

            string donem = yil.ToString() + "" + ay.ToString("00");
            foreach (AboneNo a in kontrolListe.objeler)
            {
                AboneFaturaSu afe = new AboneFaturaSu();
                afe.firmaAd = "ANKARA SU VE KANALİZASYON İDARESİ GENEL MÜDÜRLÜĞÜ";
                HRCServisC.tr.gov.sgb.uygulama.su.SuFatura suServis = new HRCServisC.tr.gov.sgb.uygulama.su.SuFatura();
                suServis.Proxy = OrtakFonksiyonlar.ProxyAyarla();

                DataTable dt = new DataTable();
                try
                {
                    dt = suServis.AskiFaturaSorgula(a.aboneNo, donem);
                }
                catch { }

                foreach (DataRow row in dt.Rows)
                {
                    string faturaDonem = row.ItemArray[3].ToString();
                    if (donem == faturaDonem)
                    {
                        afe.isletmeKodu = "06";
                        afe.faturaYil = yil;
                        afe.faturaAy = ay;
                        afe.faturaNo = row.ItemArray[0].ToString();
                        afe.tebligTarih = new TNSDateTime((DateTime)row.ItemArray[1]);
                        afe.sonOdemeTarih = new TNSDateTime((DateTime)row.ItemArray[2]);
                        afe.hizmetNo = a.aboneNo;
                        afe.aboneAd = row.ItemArray[7].ToString();
                        afe.adresi = row.ItemArray[8].ToString();
                        afe.sayacNo = row.ItemArray[9].ToString();
                        afe.ilkOkuma = new TNSDateTime((DateTime)row.ItemArray[11]);
                        afe.sonOkuma = new TNSDateTime((DateTime)row.ItemArray[12]);
                        afe.ilkEndex = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[13].ToString());
                        afe.sonEndex = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[14].ToString());
                        afe.tuketim = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[15].ToString());
                        afe.kiyas = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[16].ToString());
                        afe.ctvTutari = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[17].ToString());
                        afe.kdv = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[18].ToString());
                        afe.suBedeli = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[19].ToString());
                        afe.atikSu = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[20].ToString());
                        afe.subeYolu = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[21].ToString());
                        afe.fatTopTutar = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[22].ToString());
                        afe.tahsil = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[23].ToString());
                        afe.tenzil = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[24].ToString());
                        afe.ceza = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[25].ToString());

                        break;
                    }
                }
                //********************************************

                if (afe.fatTopTutar > 0)
                {
                    string donenDeger = tab.KayitVarMi(afe);
                    if (donenDeger == "")
                    {
                        Sonuc sonuc = tab.Kaydet(afe);
                        if (!sonuc.islemSonuc)
                        {
                            OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + sonuc.hataStr, false);
                            break;
                        }
                        else
                            FaturaCiktisiOlustur(kullanan, afe);
                    }
                }
            }

            ve.VeriErisimKapat();
        }

        private void AktarISKI(Kullanici kullanan, int yil, int ay, ObjectArray kontrolListe)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAboneFaturaSu tab = new TABAboneFaturaSu(ve);

            string donem = yil.ToString() + "" + ay.ToString().PadLeft(2, '0');
            string oncekiDonem = yil.ToString() + "" + (ay - 1).ToString().PadLeft(2, '0');
            foreach (AboneNo a in kontrolListe.objeler)
            {
                AboneFaturaSu afe = new AboneFaturaSu();
                afe.firmaAd = "İSTANBUL SU VE KANALİZASYON İDARESİ GENEL MÜDÜRLÜĞÜ";
                HRCServisC.tr.gov.sgb.uygulama.su.SuFatura suServis = new HRCServisC.tr.gov.sgb.uygulama.su.SuFatura();
                suServis.Proxy = OrtakFonksiyonlar.ProxyAyarla();

                sonFatura sonFatura = suServis.IskiFaturaSorgula(a.aboneNo, donem);

                if (sonFatura.Donem == donem || sonFatura.Donem == oncekiDonem)
                {

                    afe.isletmeKodu = "34";
                    afe.faturaYil = yil;
                    afe.faturaAy = ay;
                    afe.faturaNo = sonFatura.FaturaNo;
                    afe.tebligTarih = new TNSDateTime(sonFatura.FaturaTarihi);
                    afe.sonOdemeTarih = new TNSDateTime(sonFatura.SonOdemeTarihi);
                    afe.hizmetNo = a.aboneNo;
                    afe.aboneAd = sonFatura.AdSoyad;
                    afe.adresi = sonFatura.Adres;
                    //afe.sayacNo = 
                    afe.ilkOkuma = new TNSDateTime(sonFatura.IlkOkumaTarihi);
                    afe.sonOkuma = new TNSDateTime(sonFatura.SonOkumaTarihi);
                    //afe.ilkEndex = OrtakFonksiyonlar.ConvertToDouble();
                    //afe.sonEndex = OrtakFonksiyonlar.ConvertToDouble();
                    //afe.tuketim = OrtakFonksiyonlar.ConvertToDouble();
                    //afe.kiyas = OrtakFonksiyonlar.ConvertToDouble();
                    afe.ctvTutari = OrtakFonksiyonlar.ConvertToDouble(sonFatura.CevreTv.Replace('.', ','));
                    afe.kdv = OrtakFonksiyonlar.ConvertToDouble(sonFatura.Kdv.Replace('.', ','));
                    afe.suBedeli = OrtakFonksiyonlar.ConvertToDouble(sonFatura.SuBedeli.Replace('.', ','));
                    //afe.atikSu = OrtakFonksiyonlar.ConvertToDouble();
                    //afe.subeYolu = OrtakFonksiyonlar.ConvertToDouble();
                    afe.fatTopTutar = OrtakFonksiyonlar.ConvertToDouble(sonFatura.DonemTutari.Replace('.', ','));
                    //afe.tahsil = OrtakFonksiyonlar.ConvertToDouble();
                    //afe.tenzil = OrtakFonksiyonlar.ConvertToDouble();
                    afe.ceza = OrtakFonksiyonlar.ConvertToDouble(sonFatura.GecmisDonemGecikmeCezasi.Replace('.', ','));
                }
                //********************************************

                if (afe.fatTopTutar > 0)
                {
                    string donenDeger = tab.KayitVarMi(afe);
                    if (donenDeger == "")
                    {
                        Sonuc sonuc = tab.Kaydet(afe);
                        if (!sonuc.islemSonuc)
                        {
                            OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + sonuc.hataStr, false);
                            break;
                        }
                        else
                            FaturaCiktisiOlustur(kullanan, afe);
                    }
                }
            }

            ve.VeriErisimKapat();
        }

        private void AktarBUSKI(Kullanici kullanan, int yil, int ay, ObjectArray kontrolListe)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAboneFaturaSu tab = new TABAboneFaturaSu(ve);

            foreach (AboneNo a in kontrolListe.objeler)
            {
                AboneFaturaSu afe = new AboneFaturaSu();
                afe.firmaAd = "BURSA SU VE KANALİZASYON İDARESİ GENEL MÜDÜRLÜĞÜ";
                HRCServisC.tr.gov.sgb.uygulama.su.SuFatura suServis = new HRCServisC.tr.gov.sgb.uygulama.su.SuFatura();
                suServis.Proxy = OrtakFonksiyonlar.ProxyAyarla();

                DataTable dt = new DataTable();
                try
                {
                    dt = suServis.BuskiFaturaSorgula(a.aboneNo, yil.ToString() + "/" + ay.ToString("00"));
                }
                catch { }

                foreach (DataRow row in dt.Rows)
                {
                    TNSDateTime faturaDonem = new TNSDateTime((DateTime)row.ItemArray[3]);
                    if (faturaDonem.Ay == ay && faturaDonem.Yil == yil)
                    {
                        afe.isletmeKodu = "16";
                        afe.faturaYil = yil;
                        afe.faturaAy = ay;
                        afe.faturaNo = row.ItemArray[0].ToString();
                        afe.tebligTarih = new TNSDateTime((DateTime)row.ItemArray[1]);
                        afe.sonOdemeTarih = new TNSDateTime((DateTime)row.ItemArray[2]);
                        afe.hizmetNo = a.aboneNo;
                        afe.aboneAd = row.ItemArray[7].ToString();
                        afe.adresi = row.ItemArray[8].ToString();
                        afe.sayacNo = row.ItemArray[9].ToString();
                        afe.ilkOkuma = new TNSDateTime((DateTime)row.ItemArray[11]);
                        afe.sonOkuma = new TNSDateTime((DateTime)row.ItemArray[12]);
                        afe.ilkEndex = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[13].ToString());
                        afe.sonEndex = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[14].ToString());
                        afe.tuketim = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[15].ToString());
                        afe.kiyas = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[16].ToString());
                        //afe.ctvTutari = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[].ToString());
                        afe.kdv = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[17].ToString());
                        //afe.suBedeli = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[].ToString());
                        //afe.atikSu = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[].ToString());
                        afe.subeYolu = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[18].ToString());
                        afe.fatTopTutar = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[19].ToString());
                        afe.tahsil = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[20].ToString());
                        afe.tenzil = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[21].ToString());
                        afe.ceza = OrtakFonksiyonlar.ConvertToDouble(row.ItemArray[22].ToString());

                        break;
                    }
                }
                //********************************************

                if (afe.fatTopTutar > 0)
                {
                    string donenDeger = tab.KayitVarMi(afe);
                    if (donenDeger == "")
                    {
                        Sonuc sonuc = tab.Kaydet(afe);
                        if (!sonuc.islemSonuc)
                        {
                            OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + sonuc.hataStr, false);
                            break;
                        }
                        else
                            FaturaCiktisiOlustur(kullanan, afe);
                    }
                }
            }

            ve.VeriErisimKapat();
        }

        private void MailAt()
        {
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaSuDosyaYol") + "";
            string kimden = ConfigurationManager.AppSettings.Get("SMTPKullanici").Trim();
            string yazilimURL = ConfigurationManager.AppSettings.Get("yazilimURL").Trim();
            string yil = faturaYil.ToString();
            string ay = faturaAy.ToString("00");
            string konu = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00037, ay, yil);

            AboneFaturaSu baslik = new AboneFaturaSu();
            baslik.faturaYil = faturaYil;
            baslik.faturaAy = faturaAy;
            ObjectArray liste = MailListele(baslik);

            if (!filePath.EndsWith("\\")) filePath += "\\";
            filePath += yil + ay + "\\";

            int muhasebe = 0;
            string birim = "";

            ObjectArray mailListe = new ObjectArray();
            SuEmailListe tListe = new SuEmailListe();
            foreach (AboneFaturaSu fatura in liste.objeler)
            {
                if (muhasebe != fatura.muhasebe || birim != fatura.birim)
                {
                    if (tListe.muhasebe > 0)
                        mailListe.objeler.Add(tListe);

                    tListe = new SuEmailListe();
                    tListe.muhasebe = fatura.muhasebe;
                    tListe.birim = fatura.birim;
                    tListe.mail = fatura.mail;
                    tListe.odemeEmriNo = fatura.odemeEmriNo;
                    tListe.surecNo = fatura.surecNo;
                }

                AboneFaturaSu f = new AboneFaturaSu();
                f.faturaYil = faturaYil;
                f.faturaAy = faturaAy;
                f.hizmetNo = fatura.hizmetNo;
                f.isletmeKodu = fatura.isletmeKodu;
                f.sonOdemeTarih = fatura.sonOdemeTarih;
                f.fatTopTutar = fatura.fatTopTutar;
                tListe.fatura.objeler.Add(f);

                muhasebe = fatura.muhasebe;
                birim = fatura.birim;
            }
            if (tListe.muhasebe > 0)
                mailListe.objeler.Add(tListe);

            foreach (SuEmailListe fatura in mailListe.objeler)
            {
                double toplamFatura = 0;
                string suNo = "";
                TNSCollection pdfler = new TNSCollection();

                foreach (AboneFaturaSu ab in fatura.fatura.objeler)
                {
                    toplamFatura += OrtakFonksiyonlar.ConvertToDbl(ab.fatTopTutar);
                    if (suNo != "") suNo += ", ";
                    suNo += ab.hizmetNo;
                    DosyaEkle(ab, pdfler, filePath);
                }

                string odemeEmriLink = "<a href='" + yazilimURL + "/giris.aspx?modul=ButceMuhasebe&sayfa=OdemeEmriMifFrame.aspx&parametre=yil-" + DateTime.Now.Year + "-belgeNo-" + fatura.odemeEmriNo + "'>" + TNS.HRC.HarcamaSurecleriResI.HRC00035 + "</a>";
                string bilgi = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00038, faturaAy.ToString("00"), faturaYil, suNo, toplamFatura.ToString("#,###.00"), fatura.surecNo, fatura.odemeEmriNo, odemeEmriLink);
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

        private ObjectArray MailListele(AboneFaturaSu faturaKriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
            TABAboneFaturaSu tab = new TABAboneFaturaSu(ve);

            IDataReader dr = tab.MailIcinListe(faturaKriter);
            AboneFaturaSu fatura = new AboneFaturaSu();
            while (dr.Read())
            {
                fatura = new AboneFaturaSu();

                fatura.muhasebe = VeriErisim.DegerAl(dr, 0, 0);
                fatura.birim = VeriErisim.DegerAl(dr, 1, "");
                fatura.mail = VeriErisim.DegerAl(dr, 2, "");
                fatura.odemeEmriNo = VeriErisim.DegerAl(dr, 3, "");
                fatura.surecNo = VeriErisim.DegerAl(dr, 4, "");
                fatura.hizmetNo = VeriErisim.DegerAl(dr, 5, "");
                fatura.sonOdemeTarih = VeriErisim.DegerAl(dr, 6);
                fatura.fatTopTutar = VeriErisim.DegerAl(dr, 7, 0.0);
                fatura.isletmeKodu = VeriErisim.DegerAl(dr, 8, "");
                TNSDateTime surecTarih = VeriErisim.DegerAl(dr, 9);

                if (surecTarih.ToString() == DateTime.Now.ToShortDateString())
                    bilgi.Ekle(fatura);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        private void DosyaEkle(AboneFaturaSu fatura, TNSCollection pdfler, string filePath)
        {
            string pdfAd = "";
            string aboneNo = fatura.isletmeKodu.Replace(".", "") + "_" + fatura.hizmetNo;
            string faturaYil = fatura.faturaYil.ToString();
            string faturaAy = fatura.faturaAy.ToString();
            if (faturaAy.Length == 1) faturaAy = "0" + faturaAy;
            pdfAd = filePath + "\\" + aboneNo + ".html";
            if (System.IO.File.Exists(pdfAd))
                pdfler.Add(pdfAd);
        }
    }
}

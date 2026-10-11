using System;
using System.Data;
using System.Xml;
using System.Configuration;
using System.IO;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using HRCServisC.tr.gov.sgb.uygulama.tedas;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Elektrik fatura bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCAboneFaturaElektrik
    {
        /// <summary>
        /// Oluşan hataların yazılacağı dosya adını tutan değişken
        /// </summary>
        public string hataDosyaAd = "AboneDosyaAktarimiElektrik" + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + ".txt";
        int faturaYil = 0;
        int faturaAy = 0;

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve ilgili dosyaya fatura çıktısını kaydeder
        /// ve sonucu string olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yil bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <returns>Fatura çıktısı oluşturma işlem sonucu string olarak döner</returns>
        public string EskiFaturalaraCiktiOlustur(Kullanici kullanan, int yil, int ay)
        {
            AboneFaturaElektrik elk = new AboneFaturaElektrik();
            elk.faturaYil = yil;
            elk.faturaAy = ay;
            ObjectArray liste = Listele(elk, false);

            try
            {
                foreach (AboneFaturaElektrik fat in liste.objeler)
                {
                    FaturaCiktisiOlustur(kullanan, fat);
                }

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini ilgili web servisden okur
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Aktar(Kullanici kullanan, int yil, int ay)
        {
            Sonuc sonuc = new Sonuc();

            XmlNode node;

            AboneNo abone = new AboneNo();
            abone.aboneTip = (int)ENUMAboneTip.ELEKTRIK;
            abone.durum = (int)ENUMAktiflik.AKTIF;

            //Aboneleri listele
            HRCAboneNo aboneler = new HRCAboneNo();
            ObjectArray aboneListe = aboneler.Listele(kullanan, abone);

            //Kayıt edilmiş olanları listele
            AboneFaturaElektrik kriter = new AboneFaturaElektrik();
            ObjectArray listeFatura = new ObjectArray();
            kriter.faturaYil = yil;
            kriter.faturaAy = ay;
            listeFatura = new HRCAboneFaturaElektrik().Listele(kriter, false);

            //Kayıt edilmemiş olanları yeni liste yap
            ObjectArray kontrolListe = new ObjectArray();
            foreach (AboneNo a in aboneListe.objeler)
            {
                bool bulundu = false;
                foreach (AboneFaturaElektrik abf in listeFatura.objeler)
                {
                    if (abf.isletmeKodu == a.isletmeKodu && abf.hizmetNo == a.aboneNo)
                        bulundu = true;
                }
                if (!bulundu)
                    kontrolListe.objeler.Add(a);
            }
            //******************************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAboneFaturaElektrik tab = new TABAboneFaturaElektrik(ve);

            string donem = yil.ToString() + "/" + ay.ToString().PadLeft(2, '0');
            foreach (AboneNo a in kontrolListe.objeler)
            {
                int ilKodu = 0;
                int ilBolgeKodu = 0;
                int ilceKodu = 0;
                int kasabaKodu = 0;
                int koyKodu = 0;
                string[] parca = a.isletmeKodu.Split('.');

                if (string.IsNullOrEmpty(a.isletmeKodu) || parca.Length < 5)
                {
                    OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + "İşletmeKodu:" + a.isletmeKodu + "/AboneNo:" + a.aboneNo + "/Dönem:" + donem + "/İşletme kodu boş bırakılmış", false);
                    continue;
                }
                try
                {
                    ilKodu = OrtakFonksiyonlar.ConvertToInt(parca[0], 0);
                    ilBolgeKodu = OrtakFonksiyonlar.ConvertToInt(parca[1], 0);
                    ilceKodu = OrtakFonksiyonlar.ConvertToInt(parca[2], 0);
                    kasabaKodu = OrtakFonksiyonlar.ConvertToInt(parca[3], 0);
                    koyKodu = OrtakFonksiyonlar.ConvertToInt(parca[4], 0);
                }
                catch
                { }


                string il = a.muhasebe.ToString().PadLeft(5, '0').Substring(0, 2);
                //Aydın - Denizli - Manisa
                if (il == "09" || il == "22" || il == "45")
                {

                }
                else
                {
                    HRCServisC.tr.gov.sgb.uygulama.tedas.TedasFatura faturaSorgu = new HRCServisC.tr.gov.sgb.uygulama.tedas.TedasFatura();
                    faturaSorgu.Proxy = OrtakFonksiyonlar.ProxyAyarla();

                    try
                    {
                        node = faturaSorgu.FaturaSorgula(ilKodu, ilBolgeKodu, ilceKodu, kasabaKodu, koyKodu, a.aboneNo, donem);
                        if (node.InnerText.StartsWith("HATA"))
                        {
                            OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + "İşletmeKodu:" + a.isletmeKodu + "/AboneNo:" + a.aboneNo + "/Dönem:" + donem + "/" + node.InnerText, false);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + "İşletmeKodu:" + a.isletmeKodu + "/AboneNo:" + a.aboneNo + "/Dönem:" + donem + "/" + ex.Message, false);
                        continue;
                    }



                    ObjectArray liste = new ObjectArray();
                    AboneFaturaElektrik afe = new AboneFaturaElektrik();
                    foreach (XmlNode node1 in node.ChildNodes)
                    {
                        if (node1.Name == "IsletmeKodu")
                        {
                            afe.isletmeKodu = node1.InnerText.Trim();
                            string[] isletme = afe.isletmeKodu.Split('.');
                            afe.isletmeKodu = isletme[0].PadLeft(3, '0') + "." + isletme[1].PadLeft(2, '0') + "." + isletme[2].PadLeft(2, '0') + "." + isletme[3].PadLeft(2, '0') + "." + isletme[4].PadLeft(2, '0');
                        }
                        else if (node1.Name == "AboneNo")
                            afe.hizmetNo = node1.InnerText.Trim();
                        else if (node1.Name == "Adi_SirketAdi")
                            afe.adSoyadUnvan += node1.InnerText;
                        else if (node1.Name == "Soyad")
                            afe.adSoyadUnvan += " " + node1.InnerText;
                        else if (node1.Name == "DosyaNo")
                            afe.dosyaNo = node1.InnerText;
                        else if (node1.Name == "SiraNo")
                            afe.siraNo = OrtakFonksiyonlar.ConvertToDbl(node1.InnerText);
                        else if (node1.Name == "Adres")
                            afe.adres = node1.InnerText;
                        else if (node1.Name == "Tarife")
                            afe.tarife = node1.InnerText;
                        else if (node1.Name == "Dönem")
                        {
                            afe.faturaYil = OrtakFonksiyonlar.ConvertToInt(node1.InnerText.Split('/')[0], 0);
                            afe.faturaAy = OrtakFonksiyonlar.ConvertToInt(node1.InnerText.Split('/')[1], 0);
                        }
                        else if (node1.Name == "FaturaMiktar")
                        {
                            afe.fatTopTutar = OrtakFonksiyonlar.ConvertToDecimal(node1.InnerText);
                            if (afe.fatTopTutar == 0)
                                break;
                        }
                        else if (node1.Name == "OdemeDurum")
                            afe.odemeDurum = node1.InnerText;
                        else if (node1.Name == "TuketimAktifKwh")
                            afe.tuketimAktifKwh = OrtakFonksiyonlar.ConvertToDecimal(node1.InnerText);
                        else if (node1.Name == "TuketimReaktifKwh")
                            afe.tuketimReaktifKwh = OrtakFonksiyonlar.ConvertToDecimal(node1.InnerText);

                        else if (node1.Name == "Sayaclar")
                        {
                            foreach (XmlNode node2 in node1.ChildNodes)
                            {
                                if (node2.Name == "Sayac")
                                {
                                    AboneFaturaElektrikSayac afec = new AboneFaturaElektrikSayac();
                                    afec.faturaAy = ay;
                                    afec.faturaYil = yil;
                                    afec.hizmetNo = afe.hizmetNo;
                                    afec.isletmeKodu = afe.isletmeKodu;
                                    foreach (XmlNode node3 in node2.ChildNodes)
                                    {
                                        if (node3.Name == "SayacSeriNo")
                                        { afec.seriNo = node3.InnerText; }
                                        else if (node3.Name == "SayacMarka")
                                        { afec.marka = node3.InnerText; }
                                        else if (node3.Name == "EnerjiOlcumCins")
                                        { afec.enerjiOlcumCins = node3.InnerText; }
                                        else if (node3.Name == "HaneTam")
                                        { afec.haneTam = node3.InnerText; }
                                        else if (node3.Name == "HaneKesir")
                                        { afec.haneKesir = node3.InnerText; }
                                        else if (node3.Name == "SonOkumaTarih")
                                        { afec.sonOkumaTarih = new TNSDateTime(node3.InnerText); }
                                    }
                                    afe.sayaclar.objeler.Add(afec);
                                }
                            }
                        }
                    }
                    if (afe.fatTopTutar != 0)
                    {
                        string donenDeger = tab.KayitVarMi(afe);
                        if (donenDeger == "")
                        {
                            sonuc = tab.Kaydet(afe);
                            if (!sonuc.islemSonuc)
                            {
                                OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - " + sonuc.hataStr, false);
                                break;
                            }
                            else
                            {
                                foreach (AboneFaturaElektrikSayac sayac in afe.sayaclar.objeler)
                                {
                                    if (sonuc.islemSonuc)
                                        sonuc = tab.KaydetSayacBilgi(sayac);
                                }
                                FaturaCiktisiOlustur(kullanan, afe);
                            }
                        }
                    }
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve ilgili kullanıcılara fatura bilgisini mail olarak gönderir
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="baslik">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        public void MailGonder(Kullanici kullanan, AboneFaturaElektrik baslik)
        {
            faturaYil = baslik.faturaYil;
            faturaAy = baslik.faturaAy;

            MailAt();
        }

        //clienttan gelen ; ayıraçlı liste içinde, Elektrikden gelen pdf dosyaları var ise ; ayıraçlı dosya listesini geri döndür
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
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaElektrikDosyaYol") + "";
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
        /// <param name="faturaYil">İşlem yapılacak fatura yil bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="dosyaAd">İşlem yapılacak dosya adı bilgisi.</param>
        /// <returns>Okunan dosyayı byte dizisi olarak döner</returns>
        public byte[] FaturaDosyaVer(Kullanici kullanan, int faturaYil, int faturaAy, string dosyaAd)
        {
            byte[] donenDeger = { };
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaElektrikDosyaYol") + "";
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
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve AboneFaturaElektrik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="faturaKriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> süreç oluşturulmayan faturalar gelir</param>
        /// <returns>İstenilen kriterdeki, elektrik fatura bilgisi listesini AboneFaturaElektrik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(AboneFaturaElektrik faturaKriter, bool sadeceYeniler)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
            TABAboneFaturaElektrik tab = new TABAboneFaturaElektrik(ve);

            IDataReader dr = tab.Listele(faturaKriter, sadeceYeniler);
            AboneFaturaElektrik fatura = new AboneFaturaElektrik();
            while (dr.Read())
            {
                fatura = new AboneFaturaElektrik();

                fatura.faturaYil = VeriErisim.DegerAl(dr, 0, 0);
                fatura.faturaAy = VeriErisim.DegerAl(dr, 1, 0);
                fatura.isletmeKodu = VeriErisim.DegerAl(dr, 2, "");
                fatura.hizmetNo = VeriErisim.DegerAl(dr, 3, "");
                fatura.adSoyadUnvan = VeriErisim.DegerAl(dr, 4, "");
                fatura.dosyaNo = VeriErisim.DegerAl(dr, 5, "");
                fatura.siraNo = VeriErisim.DegerAl(dr, 6, 0);
                fatura.adres = VeriErisim.DegerAl(dr, 7, "");
                fatura.docNo = VeriErisim.DegerAl(dr, 8, "");
                fatura.tarife = VeriErisim.DegerAl(dr, 9, "");
                fatura.fatTopTutar = (decimal)VeriErisim.DegerAl(dr, 10, 0.0);
                fatura.odemeDurum = VeriErisim.DegerAl(dr, 11, "");
                fatura.tuketimAktifKwh = (decimal)VeriErisim.DegerAl(dr, 12, 0.0);
                fatura.tuketimReaktifKwh = (decimal)VeriErisim.DegerAl(dr, 13, 0.0);
                fatura.sonOdemeTarih = VeriErisim.DegerAl(dr, 14);
                fatura.muhasebe = VeriErisim.DegerAl(dr, 15, 0);
                fatura.birim = VeriErisim.DegerAl(dr, 16, "");
                fatura.mail = VeriErisim.DegerAl(dr, 17, "");

                bilgi.Ekle(fatura);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Elektrik fatura bilgisi varsa Sonuc nesnesinin islemSonuc özelliği true olarak gelir.
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KayitVarMi(AboneFaturaElektrik kriter)
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
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura istatistik bilgisini veritabanından okur 
        /// ve ElektrikIstatistik nesnesi olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, elektrik fatura istatistik bilgisini ElektrikIstatistik nesnesi olarak tutan nesne</returns>
        public ElektrikIstatistik ListeleElektrikIstatistik(int yil, int muhasebe, string birim)
        {
            ElektrikIstatistik bilgi = new ElektrikIstatistik();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABAboneFaturaElektrik to = new TABAboneFaturaElektrik(ve);
            IDataReader r = to.ListeleElektrikIstatistik(yil, muhasebe, birim);

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


        //private Sonuc Kaydet(ObjectArray list)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABAboneFaturaElektrik tab = new TABAboneFaturaElektrik(ve);

        //    ve.BeginTransaction();

        //    double faturaSira = 0;
        //    foreach (AboneFaturaElektrik fatura in list.objeler)
        //    {
        //        faturaSira++;
        //        fatura.siraNo = faturaSira;
        //        fatura.faturaAy = fatura.faturaAy - 1;//TEDAŞ FATURASI GELEN AYDAN BİR ÖNCEKİ AY OLMALI
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

        private void MailAt()
        {
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaElektrikDosyaYol") + "";
            string kimden = ConfigurationManager.AppSettings.Get("SMTPKullanici").Trim();
            string yazilimURL = ConfigurationManager.AppSettings.Get("yazilimURL").Trim();
            string yil = faturaYil.ToString();
            string ay = faturaAy.ToString("00");
            string konu = ay + " / " + yil + " dönemine ait Elektrik fatura bilgileri";

            AboneFaturaElektrik baslik = new AboneFaturaElektrik();
            baslik.faturaYil = faturaYil;
            baslik.faturaAy = faturaAy;
            ObjectArray liste = MailListele(baslik);

            if (!filePath.EndsWith("\\")) filePath += "\\";
            filePath += yil + ay + "\\";

            int muhasebe = 0;
            string birim = "";

            ObjectArray mailListe = new ObjectArray();
            ElektrikEmailListe tListe = new ElektrikEmailListe();
            foreach (AboneFaturaElektrik fatura in liste.objeler)
            {
                if (muhasebe != fatura.muhasebe || birim != fatura.birim)
                {
                    if (tListe.muhasebe > 0)
                        mailListe.objeler.Add(tListe);

                    tListe = new ElektrikEmailListe();
                    tListe.muhasebe = fatura.muhasebe;
                    tListe.birim = fatura.birim;
                    tListe.mail = fatura.mail;
                    tListe.odemeEmriNo = fatura.odemeEmriNo;
                    tListe.surecNo = fatura.surecNo;
                }

                AboneFaturaElektrik f = new AboneFaturaElektrik();
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

            foreach (ElektrikEmailListe fatura in mailListe.objeler)
            {
                double toplamFatura = 0;
                string elektrikNo = "";
                TNSCollection pdfler = new TNSCollection();

                foreach (AboneFaturaElektrik ab in fatura.fatura.objeler)
                {
                    toplamFatura += OrtakFonksiyonlar.ConvertToDbl(ab.fatTopTutar);
                    if (elektrikNo != "") elektrikNo += ", ";
                    elektrikNo += ab.hizmetNo;
                    DosyaEkle(ab, pdfler, filePath);
                }

                string odemeEmriLink = "<a href='" + yazilimURL + "/giris.aspx?modul=ButceMuhasebe&sayfa=OdemeEmriMifFrame.aspx&parametre=yil-" + DateTime.Now.Year + "-belgeNo-" + fatura.odemeEmriNo + "'>" + TNS.HRC.HarcamaSurecleriResI.HRC00035 + "</a>";
                string bilgi = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00036, faturaAy.ToString("00"), faturaYil, elektrikNo, toplamFatura.ToString("#,###.00"), fatura.surecNo, fatura.odemeEmriNo, odemeEmriLink);
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

        private ObjectArray MailListele(AboneFaturaElektrik faturaKriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
            TABAboneFaturaElektrik tab = new TABAboneFaturaElektrik(ve);

            IDataReader dr = tab.MailIcinListe(faturaKriter);
            AboneFaturaElektrik fatura = new AboneFaturaElektrik();
            while (dr.Read())
            {
                fatura = new AboneFaturaElektrik();

                fatura.muhasebe = VeriErisim.DegerAl(dr, 0, 0);
                fatura.birim = VeriErisim.DegerAl(dr, 1, "");
                fatura.mail = VeriErisim.DegerAl(dr, 2, "");
                fatura.odemeEmriNo = VeriErisim.DegerAl(dr, 3, "");
                fatura.surecNo = VeriErisim.DegerAl(dr, 4, "");
                fatura.hizmetNo = VeriErisim.DegerAl(dr, 5, "");
                fatura.sonOdemeTarih = VeriErisim.DegerAl(dr, 6);
                fatura.fatTopTutar = (decimal)VeriErisim.DegerAl(dr, 7, 0.0);
                fatura.isletmeKodu = VeriErisim.DegerAl(dr, 8, "");
                TNSDateTime surecTarih = VeriErisim.DegerAl(dr, 9);

                if (surecTarih.ToString() == DateTime.Now.ToShortDateString())
                    bilgi.Ekle(fatura);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        private void DosyaEkle(AboneFaturaElektrik fatura, TNSCollection pdfler, string filePath)
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

        private void FaturaCiktisiOlustur(Kullanici kullanan, AboneFaturaElektrik fatura)
        {
            string filePath = ConfigurationManager.AppSettings.Get("HarcamaElektrikDosyaYol") + "";
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
            return TNS.HRC.HarcamaSurecleriResI.HRC00034;
        }

        private string FaturaCiktiIcerikOlustur(Kullanici kullanan, AboneFaturaElektrik fatura)
        {
            string firmaAd = FaturaFirmaAdGetir(kullanan, fatura.isletmeKodu, fatura.hizmetNo);
            AboneFaturaElektrikSayac sayac = new AboneFaturaElektrikSayac();
            if (fatura.sayaclar.objeler.Count > 0)
            {
                sayac = (AboneFaturaElektrikSayac)fatura.sayaclar.objeler[0];
            }

            string sayacSonOkumaTarih = !sayac.sonOkumaTarih.isNull ? sayac.sonOkumaTarih.ToString() : "";
            string sayacSeriNo = sayac.seriNo != "" ? sayac.seriNo : "";
            string sayacMarka = sayac.marka != "" ? sayac.marka : "";

            string donusDeger = "";
            donusDeger = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
            "<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
            "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\">" +
            "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-9\" />" +
            "<meta http-equiv=\"Content-Language\" content=\"tr\" />" +
            "<head><title>Elektrik Tüketim Belgesi</title>" +
            "<style type=\"text/css\">.body{font-family:Arial; font-size:small;font-weight:bold;}.baslik{background-color: #FFFFCC;}</style></head>" +
            "<body class=\"body\">" +
            "<table border=\"1\" style=\"border-width:thin; border-color:Black;\">" +
                "<tr><td class=\"baslik\">ÖDEME YAPILACAK KURUM</td><td colspan=\"7\">" + firmaAd.ToUpper() + "</td></tr>" +
                "<tr><td colspan=\"8\" class=\"baslik\"><div align=\"center\">ELEKTRİK TÜKETİM BELGESİ</div></td></tr>" +
                "<tr><td class=\"baslik\">MÜŞTERİNİN ADI ADRESİ</td><td colspan=\"5\">" + fatura.adSoyadUnvan + " " + fatura.adres + "</td><td class=\"baslik\">VERGİ DAİRESİ VERGİ NO</td><td></td></tr>" +
                "<tr><td class=\"baslik\">İŞLETME ADI</td><td colspan=\"3\"></td><td class=\"baslik\">İŞLETME KODU</td><td>" + fatura.isletmeKodu + "</td><td class=\"baslik\">MÜŞTERİ NO</td><td>" + fatura.hizmetNo + "</td></tr>" +
                "<tr><td class=\"baslik\">FATURA NO</td><td colspan=\"7\"></td></tr>" +
                "<tr><td class=\"baslik\">MÜŞTERİNİN GRUBU</td><td colspan=\"7\">" + fatura.tarife + "</td></tr>" +
                "<tr><td class=\"baslik\">TARİFE KODU</td><td></td><td colspan=\"2\" class=\"baslik\">FATURA TARİHİ</td><td></td><td rowspan=\"2\" class=\"baslik\">SAYAÇ BİLGİ</td><td rowspan=\"2\" class=\"baslik\">SERİ NO</td><td rowspan=\"2\" class=\"baslik\">MARKA / TİP</td></tr>" +
                "<tr><td class=\"baslik\">DOSYA NO</td><td>" + fatura.dosyaNo + "</td><td colspan=\"2\" class=\"baslik\">FATURA DÖNEMİ</td><td>" + fatura.faturaYil + "/" + fatura.faturaAy + "</td></tr>" +
                "<tr><td class=\"baslik\">SIRA NO</td><td>" + fatura.siraNo + "</td><td colspan=\"2\" class=\"baslik\">İLK OKUMA TARİHİ</td><td></td><td class=\"baslik\">AKTİF S.</td><td>" + sayacSeriNo + "</td><td>" + sayacMarka + "</td></tr>" +
                "<tr><td class=\"baslik\">KURGÜÇ (kVA)</td><td></td><td colspan=\"2\" class=\"baslik\">SON OKUMA TARİHİ</td><td>" + sayacSonOkumaTarih + "</td><td class=\"baslik\">REAKTİF S.</td><td></td><td></td></tr>" +
                "<tr><td class=\"baslik\">A. TRF. ORANI</td><td></td><td colspan=\"2\" class=\"baslik\">SONRAKİ OK. DÖNEMİ</td><td></td><td class=\"baslik\">KAPASİTİF S.</td><td></td><td></td></tr>" +
                "<tr><td class=\"baslik\">G. TRF. ORANI</td><td></td><td colspan=\"2\" class=\"baslik\">GÜN. ORT. TÜK. (kWh)</td><td></td><td class=\"baslik\">DEMAND S.</td><td></td><td></td></tr>" +
                "<tr class=\"baslik\"><td colspan=\"8\"><div align=\"center\">PERAKENDE SATIŞ</div></td></tr>" +
                "<tr class=\"baslik\"><td>AKTİF</td><td>SON ENDEKS</td><td>İLK ENDEKS</td><td>ÇARPAN</td><td>TÜK. (kWh)</td><td>İLAVE TÜK.</td><td>BİRİM FİYAT</td><td>TOPLAM</td></tr>" +
                "<tr class=\"baslik\"><td colspan=\"8\"><div align=\"center\">ÇOK ZAMANLI</div></td></tr>" +
                "<tr><td class=\"baslik\">GÜNDÜZ</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr><td class=\"baslik\">PUANT</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr><td class=\"baslik\">GECE</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr class=\"baslik\"><td colspan=\"8\"><div align=\"center\">TEK ZAMANLI</div></td></tr>" +
                "<tr><td class=\"baslik\">AKTİF</td><td></td><td></td><td></td><td>" + fatura.tuketimAktifKwh + "</td><td></td><td></td><td></td></tr>" +
                "<tr><td class=\"baslik\">T.KAYBI (Fe/Cu)</td><td colspan=\"3\"></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr><td class=\"baslik\">KAÇAK</td><td colspan=\"3\"></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr><td class=\"baslik\">TÜK. TOPLAMI</td><td colspan=\"3\"></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr><td colspan=\"6\"></td><td class=\"baslik\">TOPLAM 1</td><td></td></tr>" +
                "<tr><td colspan=\"6\" class=\"baslik\">PERAKENDE SATIŞ HİZMET BEDELİ</td><td></td><td></td></tr>" +
                "<tr><td colspan=\"6\"></td><td class=\"baslik\">TOPLAM 2</td><td></td></tr>" +
                "<tr><td colspan=\"6\" class=\"baslik\">İLETİM BEDELİ</td><td></td><td></td></tr>" +
                "<tr><td colspan=\"6\"></td><td class=\"baslik\">TOPLAM 3</td><td></td></tr>" +
                "<tr class=\"baslik\"><td colspan=\"6\">DAĞITIM SİSTEM KULLANIM BEDELİ</td><td>BİRİM FİYAT</td><td>TOPLAM 4.1</td></tr>" +
                "<tr><td colspan=\"2\" class=\"baslik\">DAĞITIM BEDELİ</td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr class=\"baslik\"><td>GÜÇ</td><td>kW</td><td>MAX DEMANT</td><td>ÇARPAN</td><td>TÜK (kW)</td><td>&nbsp;</td><td>BİRİM FİYAT</td><td>TOPLAM 4.2</td></tr>" +
                "<tr><td class=\"baslik\">SÖZ GÜCÜ</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr><td class=\"baslik\">FİİLİ GÜÇ</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr><td class=\"baslik\">GÜÇ AŞIMI</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr class=\"baslik\"><td>REAKTİF</td><td>SON ENDEKS</td><td>İLK ENDEKS</td><td>ÇARPAN</td><td>TÜK.(kVARh)</td><td>İLAVE TÜK.</td><td>BİRİM FİYAT</td><td>TOPLAM 4.3</td></tr>" +
                "<tr><td class=\"baslik\">ENDÜKTİF</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr><td class=\"baslik\">KAPASİTİF</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr><td colspan=\"6\"></td><td class=\"baslik\">TOPLAM 4</td><td></td></tr>" +
                "<tr class=\"baslik\"><td colspan=\"5\">EMREAMADE KAPASİTE BEDELİ</td><td>kW</td><td>BİRİM FİYAT</td><td>TOPLAM</td></tr>" +
                "<tr><td rowspan=\"2\" class=\"baslik\">EMREAMADE KAPASİTE BEDELİ</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
                "<tr><td colspan=\"5\"></td><td class=\"baslik\">TOPLAM 5</td><td></td></tr>" +
                "<tr><td colspan=\"4\" class=\"baslik\">ÖDEME YERLERİ</td><td colspan=\"3\" class=\"baslik\">TOPLAM (1+2+3+4+5)</td><td></td></tr>" +
                "<tr><td rowspan=\"8\" align=\"left\" valign=\"top\"></td><td colspan=\"3\" rowspan=\"8\" align=\"left\" valign=\"top\"></td><td colspan=\"3\" class=\"baslik\">ENERJİ FONU (TOP.X %1)</td><td></td></tr>" +
                "<tr><td colspan=\"3\" class=\"baslik\">TRT PAYI (TOP. X %2)</td><td></td></tr>" +
                "<tr><td colspan=\"3\" class=\"baslik\">ELEKTRİK TÜKETİM VERGİSİ(TOP. X %1-%5)</td><td></td></tr>" +
                "<tr><td colspan=\"3\" class=\"baslik\">MUHTELİF İLAVE / GECİKME ZAMMI / vb.</td><td></td></tr>" +
                "<tr><td colspan=\"3\"></td><td></td></tr>" +
                "<tr><td colspan=\"3\" class=\"baslik\">KESME BAĞLAMA / USULSÜZ KULLANIM BEDELİ</td><td></td></tr>" +
                "<tr><td colspan=\"3\" class=\"baslik\">BAĞLANTI BEDELİ</td><td></td></tr>" +
                "<tr><td colspan=\"3\" class=\"baslik\">KATMA DEĞER VERGİSİ</td><td></td></tr>" +
                "<tr><td class=\"baslik\">SON ÖDEME TARİHİ</td><td colspan=\"3\"></td><td colspan=\"3\" class=\"baslik\">P. FATURA TUTARI</td><td></td></tr>" +
                "<tr><td colspan=\"4\">" + OrtakFonksiyonlar.Yaziyla(OrtakFonksiyonlar.ConvertToDbl(fatura.fatTopTutar)) + "</td><td colspan=\"3\" class=\"baslik\">MUHTELİF TENZİL</td><td></td></tr>" +
                "<tr><td colspan=\"2\" class=\"baslik\">ESKİ BORÇ (GEC.ZAM.HARİÇ)</td><td colspan=\"2\"></td><td colspan=\"3\" class=\"baslik\">ÖDENECEK TUTAR (TL)</td><td>" + fatura.fatTopTutar.ToString("#,###.00") + "</td></tr>" +
            "</table><br /><br /></body></html>";
            return donusDeger;
        }
    }
}

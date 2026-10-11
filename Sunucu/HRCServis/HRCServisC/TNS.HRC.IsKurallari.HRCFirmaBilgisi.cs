using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Firma bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCFirmaBilgisi
    {
        /// <summary>
        /// Arguman olarak verilen, firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, FirmaBilgisi kriter)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            string hata = "";

            if (kriter.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004;
            //if (kriter.birim == string.Empty)
            //    hata += "Birim alanı boş bırakılamaz.<br>";
            if (kriter.ad == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00106 + "<br>";
            if (kriter.vergiNo == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00107 + "<br>";

            if (kriter.fatura > 0 && string.IsNullOrEmpty(kriter.eMail))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00109 + "<br>";
            if (kriter.fatura > 0 && (!string.IsNullOrEmpty(kriter.eMail)))
            {
                if (!OrtakFonksiyonlar.MailAdresiDogrumu(kriter.eMail))
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00110 + "<br>";
            }

            if (kriter.hesapNo.Trim() != "")
            {
                Sonuc IBANKontrol = OrtakFonksiyonlar.IBANKontrol(kriter.hesapNo.Trim(), true);
                if (!IBANKontrol.islemSonuc)
                    hata += IBANKontrol.hataStr + "<br>";
            }

            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFirmaBilgisi to = new TABFirmaBilgisi(ve);
            string donenDeger = to.KayitVarMi(kriter);

            if (donenDeger != "" && kriter.kod == 0)
            {
                sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00108, kriter.vergiNo);
                return sonuc;
            }

            if (kriter.kod == 0)
            {
                //Firma kod alınıyor
                //**********************************************************
                TABGenel tg = new TABGenel(ve);

                string sayacKod = "FIRMABILGI";

                kriter.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0"), 0);
                //**********************************************************
            }

            ve.BeginTransaction();

            sonuc = to.Kaydet(kriter);

            //firmanın fatura girisi yapması için firma girişi yapılır.
            if (sonuc.islemSonuc && kriter.fatura > 0)
            {
                sonuc = FirmaGirisKaydet(kullanan, ve, kriter, false);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, firmanın sistemi giriş bilgisini veritabanına kaydeder,
        /// eğer firma sisteme kayıtlı ve parola bilgisi yoksa ya da yeni parola talebi varsa
        /// firma yetkilisine mail olarak parola gönderir
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ve">The ve.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="yeniParolaGonder">Eğer <c>true</c> verilirse firma yetkilisine mail yoluyla parola gönderilir</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc FirmaGirisKaydet(Kullanici kullanan, VeriErisim ve, FirmaBilgisi kriter, bool yeniParolaGonder)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim veIc = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFirmaBilgisi to = new TABFirmaBilgisi(veIc);

            double kayitSayisi = to.FirmaGirisKayitlimi(kriter);

            if (yeniParolaGonder && kayitSayisi == 0)
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00111;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (kayitSayisi == 0 || yeniParolaGonder)
            {
                ObjectArray oList = Listele(kullanan, ve, kriter);

                if (oList.ObjeSayisi == 1)
                {
                    FirmaBilgisi firma = (FirmaBilgisi)oList[0];

                    IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

                    Sonuc parolaSonuc = servisKYM.ParolaUret();

                    if (parolaSonuc.islemSonuc)
                    {
                        kriter.parola = Sifrele(parolaSonuc.anahtar);

                        sonuc = to.FirmaGirisKaydet(kriter);
                        if (sonuc.islemSonuc)
                        {
                            if (!string.IsNullOrEmpty(firma.eMail))
                                MailAt(kullanan, firma, parolaSonuc.anahtar, true);
                            else
                            {
                                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00112;
                                sonuc.islemSonuc = false;
                            }
                        }
                    }
                }
                else
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00113;
                    sonuc.islemSonuc = false;
                }
            }
            else if (kayitSayisi == 0)
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00114;
                sonuc.islemSonuc = false;

            }


            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, firma parola bilgisinin geçerliliğini kontrol eder, geçerliyse veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// Firma Kullanıcının şifresi,Firma kullanıcının eski şifresi doğruysa (giriş yaparak doğrulanır) ve yeni şifresinin ikisi de aynı ise değiştirilir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="eskiParola">İşlem yapılacak firmaya ait eski parola</param>
        /// <param name="yeniParola1">İşlem yapılacak firmaya ait yeni parola</param>
        /// <param name="yeniParola2">İşlem yapılacak firmaya ait yeni parola tekrarı</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ParolaDegistir(Kullanici kullanan, string eskiParola, string yeniParola1, string yeniParola2)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc sonuc = new Sonuc(false, "", "");
            yeniParola1 = yeniParola1.Trim();
            yeniParola2 = yeniParola2.Trim();

            FirmaBilgisi kriter = new FirmaBilgisi();
            kriter.vergiNo = kullanan.kullaniciKodu;

            FirmaBilgisi firmaGirisKaydi = FirmaGirisListele(kullanan, ve, kriter);
            if (firmaGirisKaydi.vergiNo != "")
            {
                if (yeniParola1.Length > 4)
                {
                    if (yeniParola1 == yeniParola2)
                    {
                        if (firmaGirisKaydi.parola != Sifrele(yeniParola1))
                        {
                            if (firmaGirisKaydi.parola == Sifrele(eskiParola))
                            {
                                ve.BeginTransaction();
                                TABFirmaBilgisi to = new TABFirmaBilgisi(ve);
                                firmaGirisKaydi.parola = Sifrele(yeniParola1);

                                sonuc = to.FirmaGirisKaydet(firmaGirisKaydi);

                                if (sonuc.hataStr != "")
                                {
                                    ve.Rollback();
                                }
                                else
                                    ve.Commit();
                            }
                            else
                            {
                                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00115;
                            }
                        }
                        else
                        {
                            sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00116;
                        }
                    }
                    else
                    {
                        sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00117;
                    }
                }
                else
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00118;
                }
            }
            else
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00119;

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firma bilgisini veritabanından okur
        /// ve FirmaBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="veG">The ve G.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>
        /// İstenilen kriterdeki, firma bilgisi listesini FirmaBilgisi nesne koleksiyonu olarak tutan nesne
        /// </returns>
        public ObjectArray Listele(Kullanici kullanan, VeriErisim veG, FirmaBilgisi kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veG, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFirmaBilgisi to = new TABFirmaBilgisi(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    FirmaBilgisi firmaBilgisi = new FirmaBilgisi();

                    firmaBilgisi.kod = VeriErisim.DegerAl(r, 0, 0);
                    firmaBilgisi.ad = VeriErisim.DegerAl(r, 1, "");
                    firmaBilgisi.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    firmaBilgisi.birim = VeriErisim.DegerAl(r, 3, "");
                    firmaBilgisi.vergiNo = VeriErisim.DegerAl(r, 4, "");
                    firmaBilgisi.vergiDairesi = VeriErisim.DegerAl(r, 5, "");
                    firmaBilgisi.banka = VeriErisim.DegerAl(r, 6, "");
                    firmaBilgisi.hesapNo = VeriErisim.DegerAl(r, 7, "");
                    firmaBilgisi.eMail = VeriErisim.DegerAl(r, 8, "");
                    firmaBilgisi.firmaYetkilisi = VeriErisim.DegerAl(r, 9, "");
                    firmaBilgisi.telefon = VeriErisim.DegerAl(r, 10, "");
                    firmaBilgisi.faks = VeriErisim.DegerAl(r, 11, "");
                    firmaBilgisi.adres = VeriErisim.DegerAl(r, 12, "");
                    firmaBilgisi.uyruk = VeriErisim.DegerAl(r, 13, 0);
                    firmaBilgisi.gorulmesin = VeriErisim.DegerAl(r, 14, 0);
                    firmaBilgisi.yasakli = VeriErisim.DegerAl(r, 15, 0);
                    firmaBilgisi.cepTel = VeriErisim.DegerAl(r, 16, "");
                    firmaBilgisi.web = VeriErisim.DegerAl(r, 17, "");
                    firmaBilgisi.fatura = VeriErisim.DegerAl(r, 18, 0);
                    firmaBilgisi.bicSwift = VeriErisim.DegerAl(r, 19, "");
                    firmaBilgisi.kisiUnvan = VeriErisim.DegerAl(r, 20, "");
                    firmaBilgisi.derece = VeriErisim.DegerAl(r, 21, 0);
                    firmaBilgisi.ekGosterge = VeriErisim.DegerAl(r, 22, 0);
                    firmaBilgisi.aciklama = VeriErisim.DegerAl(r, 23, "");
                    firmaBilgisi.karaListe = VeriErisim.DegerAl(r, 24, 0);
                    firmaBilgisi.adresNo = VeriErisim.DegerAl(r, 25, "");

                    bilgi.objeler.Add(firmaBilgisi);
                }
                r.Close();
            }

            if (kriter.personeldeDeAra)
            {
                //     string sql = "SELECT , SICILNO, , UNVAN, CALISMATURU, BANKA, SUBE, HESAPNO, MUHASEBEKODU, BIRIMKODU, BANKAKODU, BANKASUBEKODU, UNVANKODU, SIRANO, ISTIHDAMSEKLI, VERGIDAIRESI, KADRODERECE, AD, SOYAD, DURUM, ODEMEESASDERECE, ODEMEESASGOSTERGE, ODEMEESASEKGOSTERGE FROM HRCPERSONEL";
                //         string sql = "SELECT 1SICILNO, , 4CALISMATURU, , 6SUBE, , , 10BANKAKODU, 11BANKASUBEKODU, 12UNVANKODU, 13SIRANO, 14ISTIHDAMSEKLI, , 16KADRODERECE, 17AD, 18SOYAD, 19DURUM, , 21ODEMEESASGOSTERGE, FROM HRCPERSONEL";

                TABPersonel per = new TABPersonel(ve);
                r = per.Listele(kriter);

                if (per.Hata() == "")
                {
                    while (r.Read())
                    {
                        FirmaBilgisi firmaBilgisi = new FirmaBilgisi();

                        firmaBilgisi.kod = 0;
                        firmaBilgisi.vergiNo = VeriErisim.DegerAl(r, 0, ""); //0TCKIMLIKNO
                        firmaBilgisi.ad = VeriErisim.DegerAl(r, 2, ""); //2ADSOYAD
                        firmaBilgisi.muhasebe = VeriErisim.DegerAl(r, 8, 0);//8MUHASEBEKODU, 
                        firmaBilgisi.birim = VeriErisim.DegerAl(r, 9, "");//9BIRIMKODU
                        firmaBilgisi.vergiDairesi = VeriErisim.DegerAl(r, 15, "");//15VERGIDAIRESI
                        firmaBilgisi.banka = VeriErisim.DegerAl(r, 5, "");//5BANKA
                        firmaBilgisi.hesapNo = VeriErisim.DegerAl(r, 7, "");//7HESAPNO
                        //firmaBilgisi.eMail = VeriErisim.DegerAl(r, 8, "");
                        //firmaBilgisi.firmaYetkilisi = VeriErisim.DegerAl(r, 9, "");
                        //firmaBilgisi.telefon = VeriErisim.DegerAl(r, 10, "");
                        //firmaBilgisi.faks = VeriErisim.DegerAl(r, 11, "");
                        //firmaBilgisi.adres = VeriErisim.DegerAl(r, 12, "");
                        //firmaBilgisi.uyruk = VeriErisim.DegerAl(r, 13, 0);
                        //firmaBilgisi.gorulmesin = VeriErisim.DegerAl(r, 14, 0);
                        //firmaBilgisi.yasakli = VeriErisim.DegerAl(r, 15, 0);
                        //firmaBilgisi.cepTel = VeriErisim.DegerAl(r, 16, "");
                        //firmaBilgisi.web = VeriErisim.DegerAl(r, 17, "");
                        //firmaBilgisi.fatura = VeriErisim.DegerAl(r, 18, 0);
                        //   firmaBilgisi.bicSwift = VeriErisim.DegerAl(r, 19, "");
                        firmaBilgisi.kisiUnvan = VeriErisim.DegerAl(r, 3, "");//3UNVAN
                        firmaBilgisi.derece = VeriErisim.DegerAl(r, 20, 0);//20ODEMEESASDERECE
                        firmaBilgisi.ekGosterge = VeriErisim.DegerAl(r, 22, 0);//22ODEMEESASEKGOSTERGE 
                        bilgi.objeler.Add(firmaBilgisi);
                    }
                    r.Close();
                }
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firma bilgisini veritabanından okur
        /// ve FirmaBilgisi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="veG">The ve G.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>
        /// İstenilen kriterdeki, firma bilgisini FirmaBilgisi nesnesi olarak tutan nesne
        /// </returns>
        public FirmaBilgisi FirmaGirisListele(Kullanici kullanan, VeriErisim veG, FirmaBilgisi kriter)
        {
            FirmaBilgisi firmaBilgisi = new FirmaBilgisi();

            VeriErisim ve = VeriErisim.VeriErisimBul(veG, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFirmaBilgisi to = new TABFirmaBilgisi(ve);

            IDataReader r = to.FirmaGirisListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    firmaBilgisi.vergiNo = VeriErisim.DegerAl(r, 0, "");
                    firmaBilgisi.ad = VeriErisim.DegerAl(r, 1, "");
                    firmaBilgisi.parola = VeriErisim.DegerAl(r, 2, "");
                    firmaBilgisi.sonGirisTarih = VeriErisim.DegerAl(r, 3);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return firmaBilgisi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firmanın yaptığı işleri veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="vNo">İşlem yapılacak firmaya ait vergi numarası</param>
        /// <param name="sadeceDevamEdenler">Eğer <c>true</c> verilirse firmaya ait devam işler listelenir</param>
        /// <returns>İstenilen kriterdeki, firma işleri listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray FirmaninIsleri(Kullanici kullanan, int yil, string vNo, bool sadeceDevamEdenler)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFirmaBilgisi to = new TABFirmaBilgisi(ve);
            IDataReader r = to.FirmaninIsleri(yil, vNo, sadeceDevamEdenler);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Teklif teklif = new Teklif();

                    teklif.yil = VeriErisim.DegerAl(r, 0, 0);
                    teklif.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    teklif.birim = VeriErisim.DegerAl(r, 2, "");
                    teklif.surecNo = VeriErisim.DegerAl(r, 3, "");
                    teklif.firma.firmaNo = VeriErisim.DegerAl(r, 4, 0);
                    teklif.birimFiyat = VeriErisim.DegerAl(r, 5, 0.0);
                    teklif.firma.firmaBilgisi.vergiNo = VeriErisim.DegerAl(r, 6, "");
                    teklif.firma.firmaBilgisi.ad = VeriErisim.DegerAl(r, 7, "");
                    teklif.aciklama = VeriErisim.DegerAl(r, 8, "");

                    bilgi.objeler.Add(teklif);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, kullanıcı koduna ait firmanın sistem son giriş tarihi bilgisini veritabanına kaydeder
        /// </summary>
        /// <param name="kullaniciKodu">İşlem yapılacak firmaya ait kullanıcı kodu</param>
        public void FirmaGirisTarihKaydet(string kullaniciKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", kullaniciKodu);

            TABFirmaBilgisi to = new TABFirmaBilgisi(ve);
            to.FirmaGirisTarihKaydet(kullaniciKodu);
            ve.VeriErisimKapat();
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ilgili firmanın sisteme giriş bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="vergiNo">İşlem yapılacak firmaya ait vergi numarası</param>
        /// <param name="parola">İşlem yapılacak firmaya ait parola</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FirmaGirisYap(string vergiNo, string parola)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            FirmaBilgisi firma = new FirmaBilgisi();
            firma.vergiNo = vergiNo;

            ObjectArray firmaList = Listele(null, null, firma);
            if (firmaList.ObjeSayisi == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00121;
                return sonuc;
            }

            firma = FirmaGirisListele(null, null, firma);

            if (string.IsNullOrEmpty(firma.vergiNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00122;

                return sonuc;
            }

            if (Sifrele(parola) == firma.parola)
                sonuc.islemSonuc = true;
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00123;
                sonuc.islemSonuc = false;
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen dizginin sha1 yöntemi ile şifrelenmiş halini döndüren yordam.
        /// </summary>
        /// <param name="bilgi">sha1 yöntemi ile şifrelenecek dizgi.</param>
        /// <returns>Verilen dizginin sha1 yöntemi ile şifrelenmiş hali döndürülür.</returns>
        private static string Sifrele(string bilgi)
        {
            return Convert.ToHexString(System.Security.Cryptography.SHA1.HashData(System.Text.Encoding.UTF8.GetBytes(bilgi))); /* .NET 8: FormsAuthentication.HashPasswordForStoringInConfigFile(bilgi, "sha1") karsiligi (buyuk harf onaltilik SHA1) */
        }


        private void MailAt(Kullanici kullanan, FirmaBilgisi firma, string parola, bool yeniMi)
        {
            string yazilimAd = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimAd");
            string yazilimMailAdresi = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimMailAdresi");
            string yazilimURL = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimURL");

            string mesaj = "";
            mesaj = "Sayın " + firma.ad + ",\r\n";
            mesaj = mesaj + yazilimAd + " sisteminde, ";

            if (yeniMi)
                mesaj = mesaj + "firmanıza ait yeni kullanıcı tanımlaması yapılmıştır.";
            else
                mesaj = mesaj + "kullanmakta olduğunuz parolanız değiştirilmiştir.\r\n\r\n";
            mesaj = mesaj + "Kullanıcı bilgileriniz;\r\n\r\n";
            mesaj = mesaj + "Vergi Numarası : " + firma.vergiNo + "\r\n";
            mesaj = mesaj + "Parola : " + parola;
            mesaj = mesaj + "\r\n\r\n";
            mesaj = mesaj + "Yukarıda verilen bilgileri kullanarak " + yazilimURL + " adresinden, " + yazilimAd + " sistemine giriş yapabilirsiniz.\r\n\r\n";
            mesaj = mesaj + "\r\n\r\n";
            mesaj = mesaj + "Not : Lütfen parolanızı kimseyle paylaşmayınız ve e-posta gönderirken kullanmayınız. e-posta adresimiz " + yazilimMailAdresi + "\r\n\r\n";
            mesaj = mesaj + "\r\n\r\n\r\n";
            mesaj = mesaj + "İyi çalışmalar.\r\n";
            mesaj = mesaj + yazilimAd + " Çalışma Ekibi";

            try
            {
                OrtakFonksiyonlar.MailAt(yazilimMailAdresi, firma.eMail, yazilimAd + " Kullanıcı Bilgisi", mesaj);
            }
            catch
            {
            }
        }
    }
}

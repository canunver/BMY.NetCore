using System;
using System.Collections.Generic;
using System.Linq;
using OrtakClass;
using TNS.KYM;
using TNS.PER;
using TNS;

namespace Personelv2
{
    /// <summary>
    /// Personel raporları (Rapor.xml grup 4000, NESNE "Personelv2.PersonelRaporlari"; EKBILGI: Egitim, Personel, Faaliyet;
    /// kıstaslar: Adi, Yil, Donem, Kurulus, Yer).
    /// Eski Personelv2/PersonelRaporlari.cs'ten alındı (yalnızca bu üç dalın ulaştığı RaporPersonel, RaporPersonelEgitim ve
    /// RaporPersonelEgitim3Aylik). Değişenler: isim alanı eski dosyadaki "Personel" yerine Rapor.xml'deki NESNE adına uygun
    /// "Personelv2"; System.Web, IRapor ve kullanılmayan TNS.ORG / TNS.MUH using'leri kaldırıldı; Resources.Personel metinleri ve
    /// Genel.DonemAdGetir aşağıdaki PersonelKaynak sınıfına kopyalandı; "o_servis.GetType() == typeof(IPERServis)" (her zaman
    /// false idi, servis null gelirse patlıyordu) "o_servis is IPERServis" yapıldı; KurKod verilmezse birim süzgeci uygulanmaz;
    /// RaporAl genel rapor ekranının EKBILGI ve kıstas adlarını eski sayfanın (RaporPersonel.aspx) değerlerine çevirir.
    /// </summary>
    public class PersonelRaporlari
    {
        public string RaporPersonel(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;
            IPERServis servisPER;

            if (o_servis is IPERServis)
                servisPER = (IPERServis)o_servis;
            else
                servisPER = TNS.PER.Arac.Tanimla();

            TNS.PER.Personel kriter = new TNS.PER.Personel();
            string birim = (string)raporParam["KurKod"];
            if (string.IsNullOrEmpty(birim))
            {
                // Genel rapor ekranında (Rapor.xml grup 4000) birim kıstası yok: birim süzgeci uygulanmaz.
            }
            else if (GenelIslemlerIstemci.OrgSemaPersonelKullaniyorMu())
            {
                kriter.organizasyonKodu = OrtakFonksiyonlar.ConvertToInt(birim, 0);
                //if (kriter.organizasyonKodu > 0)
                //{
                //    OrganizasyonBirim orgBirim = TNS.ORG.Arac.OrgBirimBul(kriter.organizasyonKodu);
                //    if (orgBirim != null)
                //    {
                //        kriter.muhasebeKodu = OrtakFonksiyonlar.ConvertToInt(orgBirim.muhasebekodu, 0);
                //        kriter.birimKodu = orgBirim.muhasebebirim;
                //        kriter.kurum = orgBirim.kurumsalkod;
                //    }
                //}
            }
            else
            {
                string[] parcaliKod = birim.Split('-');
                kriter.kurum = parcaliKod[0];
                kriter.muhasebeKodu = OrtakFonksiyonlar.ConvertToInt(parcaliKod[1], 0);
                kriter.birimKodu = parcaliKod[2];
            }

            ObjectArray perPersonelRaporBilgileri = servisPER.PersonelListelePersonelRapor(kullanan, kriter);
            if (perPersonelRaporBilgileri == null || !perPersonelRaporBilgileri.sonuc.islemSonuc)
            {
                throw new Exception(perPersonelRaporBilgileri.sonuc.hataStr);
            }
            if (!string.IsNullOrEmpty(perPersonelRaporBilgileri.sonuc.bilgiStr))
            {
                throw new Exception(perPersonelRaporBilgileri.sonuc.bilgiStr);
            }

            if (perPersonelRaporBilgileri.objeler.Count <= 0)
            {
                throw new Exception(PersonelKaynak.FRMPLH007);
            }

            int satirSayisi = 0;
            List<PersonelPersonelRapor> liste = new List<PersonelPersonelRapor>();
            PersonelPersonelRapor prTemp;
            foreach (PersonelPersonelRapor pr in perPersonelRaporBilgileri.objeler)
            {
                prTemp = new PersonelPersonelRapor();  
                prTemp = pr;
                //if (prTemp.tcKimlikNo != pr.tcKimlikNo)
                //{
                //    if (!string.IsNullOrEmpty(prTemp.tcKimlikNo))
                //        liste.Add(prTemp);
              
                //    prTemp.seviyesi = 0;
                //    prTemp.LUSeviyesi = 0;
                //    prTemp.dil = "";
                //    prTemp.notuDil = 0;
                //    prTemp.duzeyDil = "";
                //    prTemp.yilDil = 0;
                //    prTemp.ayDil = 0;
                //    prTemp.belgeTurDil = 0;

                    satirSayisi++;
                //}

                //if (pr.seviyesi < 6)
                //{
                //    if (pr.gecerliOkul != 1)
                //        continue;
                //}

                switch (pr.seviyesi)
                {
                    case 0:
                        //if (prTemp.seviyesi < 1)
                        //{
                            //prTemp.seviyesi = 0;
                            prTemp.seviyesiText = "Belirsiz";
                            //prTemp.okulAdi = pr.okulAdi;
                            //prTemp.bitirmeTarihi = pr.bitirmeTarihi;
                        //}
                        break;
                    case 1:
                        //if (prTemp.seviyesi < 1)
                        //{
                            //prTemp.seviyesi = 1;
                            prTemp.seviyesiText = "İlk Öğretim";
                            //prTemp.okulAdi = pr.okulAdi;
                            //prTemp.bitirmeTarihi = pr.bitirmeTarihi;
                        //}
                        break;
                    case 2:
                        //if (prTemp.seviyesi < 2)
                        //{
                            //prTemp.seviyesi = 2;
                            prTemp.seviyesiText = "Orta Okul";
                            //prTemp.okulAdi = pr.okulAdi;
                            //prTemp.bitirmeTarihi = pr.bitirmeTarihi;
                        //}
                        break;
                    case 3:
                        //if (prTemp.seviyesi < 4)
                        //{
                            //prTemp.seviyesi = 3;
                            prTemp.seviyesiText = "Lise";
                            //prTemp.okulAdi = pr.okulAdi;
                            //prTemp.bitirmeTarihi = pr.bitirmeTarihi;
                        //}
                        break;
                    case 4:
                        //if (prTemp.seviyesi < 5)
                        //{
                            //prTemp.seviyesi = 4;
                            prTemp.seviyesiText = "Ön Lisans";
                        //prTemp.okulAdi = pr.okulAdi;
                        //prTemp.bitirmeTarihi = pr.bitirmeTarihi;
                        //}
                        prTemp.okulAdi = prTemp.okulAdi + " / " + prTemp.fakulteBolum;
                        break;
                    case 5:
                        //if (prTemp.seviyesi < 5)
                        //{
                            //prTemp.seviyesi = 5;
                            prTemp.seviyesiText = "Lisans";
                        //prTemp.okulAdi = pr.okulAdi;
                        //prTemp.bitirmeTarihi = pr.bitirmeTarihi;
                        //}
                        prTemp.okulAdi = prTemp.okulAdi + " / " + prTemp.fakulteBolum;
                        break;
                    case 6:
                        //if (prTemp.LUSeviyesi < 6)
                        //{
                            //prTemp.LUSeviyesi = 6;
                        prTemp.seviyesiText = "Yüksek Lisans";
                        //prTemp.LUSeviyesiText = "Yüksek Lisans";
                        //prTemp.LUOkulAdi = pr.okulAdi;
                        //prTemp.okulAdi = "";
                        //prTemp.bitirmeTarihi = new TNSDateTime();
                        //prTemp.LUBitirmeTarihi = pr.bitirmeTarihi;
                        //}
                        break;
                    default:
                        //if (prTemp.LUSeviyesi < 7)
                        //{
                            //prTemp.LUSeviyesi = 7;
                        prTemp.seviyesiText = "Doktora";
                            //prTemp.LUSeviyesiText = "Doktora";
                            //prTemp.LUOkulAdi = pr.okulAdi;
                            //prTemp.okulAdi = "";
                            //prTemp.bitirmeTarihi = new TNSDateTime();
                            //prTemp.LUBitirmeTarihi = pr.bitirmeTarihi;
                        //}
                        break;                   
                }

                prTemp.okulAdi = pr.okulAdi;
                prTemp.bitirmeTarihi = pr.bitirmeTarihi;
                
                if (pr.belgeTurDil == 3)
                    if (pr.yilDil > prTemp.yilDil || (pr.yilDil == prTemp.yilDil && pr.ayDil > prTemp.ayDil))
                    {
                        prTemp.dil = string.IsNullOrEmpty(pr.dil) ? "" : pr.dil;
                        prTemp.notuDil = pr.notuDil > 0 ? pr.notuDil : 0;
                        prTemp.duzeyDil = string.IsNullOrEmpty(pr.duzeyDil) ? "" : pr.duzeyDil;
                        prTemp.yilDil = pr.yilDil;
                        prTemp.ayDil = pr.ayDil;
                        prTemp.belgeTurDil = pr.belgeTurDil;
                    }
                liste.Add(prTemp);
            }
            //if (!string.IsNullOrEmpty(prTemp.tcKimlikNo))
            //    liste.Add(prTemp);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int kaynakSatir = 0;
            int satir = 0;
            int sutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            XLS.SatirKopyalaAc(kaynakSatir+1, satirSayisi, kaynakSatir + 1);
            string tc = "";
            int kisiSayisi = 0;
            foreach (PersonelPersonelRapor pr in liste.ToArray<PersonelPersonelRapor>())
            {               
                string tc1 = pr.tcKimlikNo;
             
                int bYas = DateTime.Now.Year - pr.dogumTarihi.Yil;
                string bYasAraligi = (bYas > 19 && bYas < 60) ? (bYas.ToString()).Substring(0, 1) + "0 - " + (bYas.ToString()).Substring(0, 1) + "9" : bYas >= 60 ? "60 ve üzeri" : "TANIMSIZ";

                satir++;
                
                if (tc != tc1)
                {
                    kisiSayisi++;
                    XLS.HucreDegerYaz(satir, 0, kisiSayisi);
                    XLS.HucreDegerYaz(satir, 1, pr.ad + " " + pr.soyad);
                    XLS.HucreDegerYaz(satir, 2, pr.unvan);
                    XLS.HucreDegerYaz(satir, 3, pr.sinif);
                    XLS.HucreDegerYaz(satir, 4, pr.cinsiyet == 1 ? PersonelKaynak.FRMPKS007 : PersonelKaynak.FRMPKS008);
                    XLS.HucreDegerYaz(satir, 5, pr.dogumTarihi.ToString());
                    XLS.HucreDegerYaz(satir, 6, bYas);
                    XLS.HucreDegerYaz(satir, 7, bYasAraligi);

                    //XLS.HucreDegerYaz(satir, 10, pr.LUOkulAdi + (pr.LUBitirmeTarihi.Yil > 0 ? (" (" + pr.LUBitirmeTarihi.Yil + ")") : ""));
                    //XLS.HucreDegerYaz(satir, 11, pr.LUSeviyesiText);
                    XLS.HucreDegerYaz(satir, 10, pr.dil);
                    XLS.HucreDegerYaz(satir, 11, pr.notuDil > 0 ? pr.notuDil.ToString() : "");
                    XLS.HucreDegerYaz(satir, 12, pr.duzeyDil);
                    XLS.HucreDegerYaz(satir, 13, pr.yilDil > 0 ? pr.yilDil.ToString() : "");
                    XLS.HucreDegerYaz(satir, 14, pr.belgeTurDil > 0 ? PersonelKaynak.DilBelgeTurAd(pr.belgeTurDil) : "");
                }
                XLS.HucreDegerYaz(satir, 8, pr.okulAdi + (pr.bitirmeTarihi.Yil > 0 ? (" (" + pr.bitirmeTarihi.Yil + ")") : ""));
                XLS.HucreDegerYaz(satir, 9, pr.seviyesiText);
                tc = tc1;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string RaporPersonelEgitim(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;
            IPERServis servisPER;

            if (o_servis is IPERServis)
                servisPER = (IPERServis)o_servis;
            else
                servisPER = TNS.PER.Arac.Tanimla();

            TNS.PER.Personel kriterPersonel = new TNS.PER.Personel();
            string birim = (string)raporParam["KurKod"];
            if (string.IsNullOrEmpty(birim))
            {
                // Genel rapor ekranında (Rapor.xml grup 4000) birim kıstası yok: birim süzgeci uygulanmaz.
            }
            else if (GenelIslemlerIstemci.OrgSemaPersonelKullaniyorMu())
            {
                kriterPersonel.organizasyonKodu = OrtakFonksiyonlar.ConvertToInt(birim, 0);
                //if (kriterPersonel.organizasyonKodu > 0)
                //{
                //    OrganizasyonBirim orgBirim = TNS.ORG.Arac.OrgBirimBul(kriterPersonel.organizasyonKodu);
                //    if (orgBirim != null)
                //    {
                //        kriterPersonel.muhasebeKodu = OrtakFonksiyonlar.ConvertToInt(orgBirim.muhasebekodu, 0);
                //        kriterPersonel.birimKodu = orgBirim.muhasebebirim;
                //        kriterPersonel.kurum = orgBirim.kurumsalkod;
                //    }
                //}
            }
            else
            {
                string[] parcaliKod = birim.Split('-');
                kriterPersonel.kurum = parcaliKod[0];
                kriterPersonel.muhasebeKodu = OrtakFonksiyonlar.ConvertToInt(parcaliKod[1], 0);
                kriterPersonel.birimKodu = parcaliKod[2];
            }

            TNS.PER.PersonelKurs kriterKurs = new TNS.PER.PersonelKurs();

            if (!string.IsNullOrEmpty((string)raporParam["EgitiminAdi"]))
                kriterKurs.kursAdi = (string)raporParam["EgitiminAdi"];

            if (OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0) > 0)
                kriterKurs.yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
        
       
            if (OrtakFonksiyonlar.ConvertToInt((string)raporParam["Donem"], 0) > 0)
                kriterKurs.donem = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Donem"], 0);
            

            if (!string.IsNullOrEmpty((string)raporParam["EgitimiVerenKurulus"]))
                kriterKurs.aldigiYer = (string)raporParam["EgitimiVerenKurulus"];

            if (!string.IsNullOrEmpty((string)raporParam["AlindigiYer"]))
                kriterKurs.egitimYeri = (string)raporParam["AlindigiYer"];

            if (OrtakFonksiyonlar.ConvertToInt((string)raporParam["KatilimTipi"], 0) > 0)
                kriterKurs.katilimTipi = OrtakFonksiyonlar.ConvertToInt((string)raporParam["KatilimTipi"], 0);

            ObjectArray personelEgitimBilgileri = servisPER.PersonelListeleEgitimRapor(kullanan, kriterPersonel, kriterKurs);
            if (personelEgitimBilgileri == null || !personelEgitimBilgileri.sonuc.islemSonuc)
            {
                throw new Exception(personelEgitimBilgileri.sonuc.hataStr);
            }
            if (!string.IsNullOrEmpty(personelEgitimBilgileri.sonuc.bilgiStr))
            {
                throw new Exception(personelEgitimBilgileri.sonuc.bilgiStr);
            }

            if (personelEgitimBilgileri.objeler.Count <= 0)
            {
                throw new Exception(PersonelKaynak.FRMPLH007);
            }

            Tablo XLS = GenelIslemler.NewTablo();

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int kaynakSatir = 0;
            int satir = 0;
            int sutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            kaynakSatir++;
            satir = kaynakSatir;
            XLS.SatirKopyalaAc(kaynakSatir-1, personelEgitimBilgileri.objeler.Count+1 , kaynakSatir );
            string egitimAdi = "";



            foreach (PersonelEgitimRapor per in personelEgitimBilgileri.objeler)
            {
                satir++;
                egitimAdi = servisPER.ListeleEgitimTurleriAdlari(kullanan, per.egitimTurKodu);
               
                if (string.IsNullOrEmpty(egitimAdi))
                    egitimAdi = per.egitiminAdi; 

                //egitimAdi += per.egitiminAdi;                

                XLS.HucreDegerYaz(satir, 0, satir - kaynakSatir);
                XLS.HucreDegerYaz(satir, 1, per.ad + " " + per.soyad);
                XLS.HucreDegerYaz(satir, 2, per.unvan);
                XLS.HucreDegerYaz(satir, 3, egitimAdi);
                //XLS.HucreDegerYaz(satir, 4, per.egitiminTipi);
                ENUMKatilimTipi katilimTipi = (ENUMKatilimTipi)per.katilimTipi;
                XLS.HucreDegerYaz(satir, 4, katilimTipi.ToString());
                XLS.HucreDegerYaz(satir, 5, per.baslangicTarihi.ToString());
                XLS.HucreDegerYaz(satir, 6, per.bitisTarihi.ToString());
                XLS.HucreDegerYaz(satir, 7, per.dersSaati);
                XLS.HucreDegerYaz(satir, 8, per.egitimiVerenKurulus);
                XLS.HucreDegerYaz(satir, 9, per.egitimYeri);
                XLS.HucreDegerYaz(satir, 10, per.aciklama);
                XLS.HucreDegerYaz(satir, 11, per.donemi);
                XLS.HucreDegerYaz(satir, 12, per.yili > 0 ? per.yili.ToString() : "");
                XLS.HucreDegerYaz(satir, 13, per.dosyaNo);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string RaporPersonelEgitim3Aylik(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;
            IPERServis servisPER;

            if (o_servis is IPERServis)
                servisPER = (IPERServis)o_servis;
            else
                servisPER = TNS.PER.Arac.Tanimla();

            TNS.PER.Personel kriterPersonel = new TNS.PER.Personel();
            string birim = (string)raporParam["KurKod"];
            if (string.IsNullOrEmpty(birim))
            {
                // Genel rapor ekranında (Rapor.xml grup 4000) birim kıstası yok: birim süzgeci uygulanmaz.
            }
            else if (GenelIslemlerIstemci.OrgSemaPersonelKullaniyorMu())
            {
                kriterPersonel.organizasyonKodu = OrtakFonksiyonlar.ConvertToInt(birim, 0);
                //if (kriterPersonel.organizasyonKodu > 0)
                //{
                //    OrganizasyonBirim orgBirim = TNS.ORG.Arac.OrgBirimBul(kriterPersonel.organizasyonKodu);
                //    if (orgBirim != null)
                //    {
                //        kriterPersonel.muhasebeKodu = OrtakFonksiyonlar.ConvertToInt(orgBirim.muhasebekodu, 0);
                //        kriterPersonel.birimKodu = orgBirim.muhasebebirim;
                //        kriterPersonel.kurum = orgBirim.kurumsalkod;
                //    }
                //}
            }
            else
            {
                string[] parcaliKod = birim.Split('-');
                kriterPersonel.kurum = parcaliKod[0];
                kriterPersonel.muhasebeKodu = OrtakFonksiyonlar.ConvertToInt(parcaliKod[1], 0);
                kriterPersonel.birimKodu = parcaliKod[2];
            }

            TNS.PER.PersonelKurs kriterKurs = new TNS.PER.PersonelKurs();

            if (!string.IsNullOrEmpty((string)raporParam["EgitiminAdi"]))
                kriterKurs.kursAdi = (string)raporParam["EgitiminAdi"];

            if (OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0) > 0)
                kriterKurs.yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);


            if (OrtakFonksiyonlar.ConvertToInt((string)raporParam["Donem"], 0) > 0)
                kriterKurs.donem = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Donem"], 0);


            //if (!string.IsNullOrEmpty((string)raporParam["EgitimiVerenKurulus"]))
            //    kriterKurs.aldigiYer = (string)raporParam["EgitimiVerenKurulus"];

            if (!string.IsNullOrEmpty((string)raporParam["AlindigiYer"]))
                kriterKurs.aldigiYer = (string)raporParam["AlindigiYer"];

            ObjectArray personelEgitimBilgileri = servisPER.DonemlikEgitimFaaliyetRaporu(kullanan, kriterPersonel, kriterKurs);
            if (personelEgitimBilgileri == null || !personelEgitimBilgileri.sonuc.islemSonuc)
            {
                throw new Exception(personelEgitimBilgileri.sonuc.hataStr);
            }
            if (!string.IsNullOrEmpty(personelEgitimBilgileri.sonuc.bilgiStr))
            {
                throw new Exception(personelEgitimBilgileri.sonuc.bilgiStr);
            }

            if (personelEgitimBilgileri.objeler.Count <= 0)
            {
                throw new Exception(PersonelKaynak.FRMPLH007);
            }

            Tablo XLS = GenelIslemler.NewTablo();
       
            // Eski kod şablonu her zaman /RaporSablon/PERv2/3AylikEgitimFaaliyetRaporu.xlt'den alıyordu (Server.MapPath).
            string eskiSablon = Istemci.Ayar.Oku("RaporSablonYol") + "/PERv2/3AylikEgitimFaaliyetRaporu.xlt";
            if (System.IO.File.Exists(eskiSablon))
                sablonAd = eskiSablon;
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int kaynakSatir = 0;
            int satir = 0;
            int sutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            kaynakSatir++;
            satir = kaynakSatir;
            XLS.SatirKopyalaAc(kaynakSatir - 1, personelEgitimBilgileri.objeler.Count + 1, kaynakSatir);
            XLS.HucreAdBulYaz("Yil", kriterKurs.yil > 0 ? kriterKurs.yil.ToString() : "");
            XLS.HucreAdBulYaz("donem", PersonelKaynak.DonemAdGetir(kriterKurs.donem));
            XLS.HucreAdBulYaz("BirimAdi", raporParam["BirimAd"] + "");
            XLS.HucreAdBulYaz("Baslik", kriterKurs.donem + "." + " 3 AYLIK EĞİTİM FAALİYET RAPORU");
            string egitimAdi = "";

            foreach (PersonelEgitimRapor per in personelEgitimBilgileri.objeler)
            {
                satir++;
                egitimAdi = servisPER.ListeleEgitimTurleriAdlari(kullanan, per.egitimTurKodu);

                if (string.IsNullOrEmpty(egitimAdi))
                    egitimAdi = per.egitiminAdi;
                //    egitimAdi += "-";

                //egitimAdi += per.egitiminAdi;  

                XLS.HucreDegerYaz(satir, 0, "");
                XLS.HucreDegerYaz(satir, 1, "");
                XLS.HucreDegerYaz(satir, 2, per.egitimiVerenKurulus);
                XLS.HucreDegerYaz(satir, 3, egitimAdi);
                XLS.HucreDegerYaz(satir, 4, per.baslangicTarihi.ToString() + " - " + per.bitisTarihi.ToString());
                XLS.HucreDegerYaz(satir, 5, "");
                XLS.HucreDegerYaz(satir, 6, per.gun);
                XLS.HucreDegerYaz(satir, 7, per.dersSaati);
                XLS.HucreDegerYaz(satir, 8, per.unvan);
                XLS.HucreDegerYaz(satir, 9, per.katilimciSayisi);
                XLS.HucreDegerYaz(satir, 10, per.egitimYeri);
                XLS.HucreDegerYaz(satir, 11, per.aciklama);
            }
          
            XLS.HucreDegerYaz(satir+3, 0, "*Eğitimin izleyen üç aylık dönem(ler)de de devam etmesi durumunda sadece ilgili döneme isabet eden kısmı hesaplanıp yazılacaktır.");

            XLS.KoyuYap(satir + 3, 0, true);
            XLS.HucreBirlestir(satir + 3, 0, satir + 3,11);
           
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>
        /// Rapor motoru giriş noktası. EKBILGI: Egitim (eski FRMPRPT002 "Eğitim"), Personel (FRMPRPT001 "Personel Genel"),
        /// Faaliyet (FRMPRPT013 "Eğitim Faaliyet Bilgileri"); eski sayfanın gönderdiği bu Türkçe değerler de kabul edilir.
        /// Kıstaslar eski sayfanın parametre adlarına çevrilir: Adi -> EgitiminAdi, Kurulus -> EgitimiVerenKurulus, Yer -> AlindigiYer.
        /// </summary>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            if (raporParam == null)
                raporParam = new System.Collections.Hashtable();
            KistasAktar(raporParam, "Adi", "EgitiminAdi");
            KistasAktar(raporParam, "Kurulus", "EgitimiVerenKurulus");
            KistasAktar(raporParam, "Yer", "AlindigiYer");

            if (ekBilgi == "Personel" || ekBilgi == "Personel Genel")
                return RaporPersonel(kullanan, servis, SablonBul(sablonAd, "PER-Personel.xlt"), sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi == "Egitim" || ekBilgi == "Eğitim")
                return RaporPersonelEgitim(kullanan, servis, SablonBul(sablonAd, "PER-PersonelEgitim.xlt"), sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi == "Faaliyet" || ekBilgi == "Eğitim Faaliyet Bilgileri")
            {
                raporParam["AlindigiYer"] += ",SGB,STRATEJİ";
                return RaporPersonelEgitim3Aylik(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            }
            else
                return "HATA: Tanınmayan ek bilgi: " + ekBilgi;
        }

        /// <summary>Genel rapor ekranının kıstas adını (kaynak) eski sayfanın parametre adına (hedef) taşır; hedef doluysa dokunmaz.</summary>
        private static void KistasAktar(System.Collections.Hashtable raporParam, string kaynak, string hedef)
        {
            if (raporParam[hedef] == null && raporParam[kaynak] != null)
                raporParam[hedef] = raporParam[kaynak] + "";
        }

        /// <summary>Rapor.xml'deki şablon yoksa eski sayfanın (RaporPersonel.aspx, Resources.Personel.FRMPRPSxxx) şablonunu dener.</summary>
        private static string SablonBul(string sablonAd, string eskiSablonAd)
        {
            if (!string.IsNullOrEmpty(sablonAd) && System.IO.File.Exists(sablonAd))
                return sablonAd;
            string eskiSablon = Istemci.Ayar.Oku("RaporSablonYol") + "/PERv2/" + eskiSablonAd;
            if (System.IO.File.Exists(eskiSablon))
                return eskiSablon;
            return sablonAd;
        }
    }

    /// <summary>
    /// Eski App_GlobalResources/Personel.resx'ten (varsayılan kültür) bu raporların kullandığı metinler ve eski
    /// Personelv2/Genel.cs'teki DonemAdGetir; Core'da karşılıkları yok.
    /// </summary>
    internal static class PersonelKaynak
    {
        internal const string FRMPLH007 = "Verilen kriterlere uygun kayıt bulunamadı.";
        internal const string FRMPKS007 = "Erkek";
        internal const string FRMPKS008 = "Kadın";

        /// <summary>Eski Resources.Personel.ResourceManager.GetString("FRMPDLB" + belgeTur.ToString("d3")).</summary>
        internal static string DilBelgeTurAd(int belgeTur)
        {
            switch (belgeTur)
            {
                case 1: return "Sertifika";
                case 2: return "Katılım Belgesi";
                case 3: return "KPDS";
                case 4: return "TOEFL";
                case 5: return "IELTS";
                case 6: return "ÜDS";
                case 7: return "YDS";
                case 99: return "Beyan";
                default: return null;
            }
        }

        /// <summary>Eski Personel.Genel.DonemAdGetir.</summary>
        internal static string DonemAdGetir(int donem)
        {
            switch (donem)
            {
                case 1: return "1. ÜÇ AYLIK DÖNEM(OCAK-MART)";
                case 2: return "2. ÜÇ AYLIK DÖNEM(NİSAN-HAZİRAN)";
                case 3: return "3. ÜÇ AYLIK DÖNEM(TEMMUZ-EYLÜL)";
                case 4: return "4. ÜÇ AYLIK DÖNEM(EKİM-ARALIK)";
                default: return "";
            }
        }
    }
}

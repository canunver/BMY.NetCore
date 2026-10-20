using System;
using System.Collections.Generic;
using System.Linq;
using OrtakClass;
using TNS;
using TNS.IKY;
using TNS.KYM;
using System.Data;
using TNS.UZY;
using System.IO;
using System.Collections;

namespace InsanKaynaklari
{
    /// <summary>
    /// Personel Raporları ekranının (eski InsanKaynaklari/RaporPersonel.aspx; RaporTanim.raporDosyasiNo 0 ve 2 olan raporlar)
    /// rapor üreticisi. RaporAl(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi): ekBilgi rapor kodu
    /// (IK009, IK010, IK012-IK021b, IK028-IK036, IK042-IK054, GR009), ciktiYeri "xlsx" / "pdf" (Tablo raporlarında "docx" de
    /// Excel biçiminde saklanır, eskisi gibi); dönüş üretilen dosyanın tam yolu (sonucDosyaAd + "." + uzantı).
    /// Eski InsanKaynaklari/PersonelRaporlari.cs'ten alındı; iş mantığı (servis çağrıları, SQL, hücre yerleşimi) aynen korundu.
    /// Değişenler:
    ///   - System.Web / Ext.NET bağımlılığı ve TNS.Raporlama.IRapor arayüzü kaldırıldı (Core'da IRapor yok); kullanılmayan
    ///     TNS.ORG (TNS.IKY.Personel / Arac ile ad çakışması), TNS.Raporlama ve Aspose.Words using'leri kaldırıldı.
    ///   - IK010'da eski GenelIslemler.MesajKutusu("Hata", FRMKAD016) + return "" yerine throw new Exception(metin); eski
    ///     Resources.InsanKaynaklari.FRMPRP012 / FRMKAD016 metinleri sabit olarak yazıldı.
    ///   - RaporAl başında AsposeLisans.Yukle() çağrılır (eski Global.asax lisansı yüklüyordu; çağrı tekrar edilebilir).
    ///   - Eski dosyadaki yorum satırına alınmış (ölü) metotlar olduğu gibi bırakıldı; derlemeye girmezler.
    ///   - servis parametresi eskisi gibi "o_servis.GetType() == typeof(IIKYServis)" ile denetlenir (her zaman false; servis
    ///     her seferinde TNS.IKY.Arac.Tanimla() ile kurulur). servis null geçilirse NullReferenceException olur; null geçmeyin.
    /// </summary>
    public class PersonelRaporlari
    {
        protected string DoluBosKadroYazdir(object kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            TNS.IKY.PersonelKadro pk = new TNS.IKY.PersonelKadro();
            pk.personelTipCoklu = (string)raporParam["personelTur"];
            pk.organizasyonKopruKodu = (string)raporParam["ORGKOD"];

            var liste = servisIKY.DoluBosKadroListele(pk);
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);
            if (liste.sonuc.islemSonuc && liste.objeler.Count > 0)
            {
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                ArrayList list = new ArrayList();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                XLS.HucreAdBulYaz("BirimAd", (string)raporParam["BirimAd"]);
                satir = kaynakSatir;
                bool sabit;
                int ayniSatirda = 0, yazilanSatir = 0, siraNo = 1;

                foreach (KadroDoluBos k in liste.objeler)
                {
                    sabit = false;
                    ayniSatirda = 0;
                    foreach (object[] o in list)
                    {
                        if (k.unvanKod == OrtakFonksiyonlar.ConvertToInt(o[0], 0))
                        {
                            ayniSatirda = OrtakFonksiyonlar.ConvertToInt(o[1], 0);
                            sutun = 3 * k.kadroDerece;// -2;
                            sabit = true;
                            break;
                        }
                    }

                    if (!sabit)
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, sutun + 51, satir, 0);
                        XLS.HucreDegerYaz(satir, 0, siraNo);
                        XLS.HucreDegerYaz(satir, 1, k.sinifAd);
                        XLS.HucreDegerYaz(satir, 2, k.unvanAd); //k.unvanKod + "/" + 
                        siraNo++;
                    }
                    if (ayniSatirda > 0)
                        yazilanSatir = ayniSatirda;
                    else
                        yazilanSatir = satir;
                    sutun = 3 * k.kadroDerece;// -2;
                    XLS.HucreDegerYaz(yazilanSatir, sutun, k.kontenjan);
                    XLS.HucreDegerYaz(yazilanSatir, sutun + 1, k.dolu);
                    XLS.HucreDegerYaz(yazilanSatir, sutun + 2, k.bos);

                    list.Add(new object[] { k.unvanKod, yazilanSatir });
                }
                int yukseklik = 360;
                if (ekBilgi == "IK043")
                    yukseklik = 600;

                XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(yukseklik));
            }
            else if (!liste.sonuc.islemSonuc)
            {
                XLS.HucreDegerYaz(2, 2, liste.sonuc.hataStr);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //public string RaporPersonel(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    Kullanici kullanan = (Kullanici)o_kullanan;
        //    IIKYServis servisIKY;

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();


        //    TNS.IKY.Personel kriter = new TNS.IKY.Personel();
        //    string birim = (string)raporParam["KurKod"];

        //    ObjectArray perPersonelRaporBilgileri = servisIKY.PersonelListelePersonelRapor(kullanan, kriter);
        //    if (perPersonelRaporBilgileri == null || !perPersonelRaporBilgileri.sonuc.islemSonuc)
        //    {
        //        throw new Exception(perPersonelRaporBilgileri.sonuc.hataStr);
        //    }
        //    if (!string.IsNullOrEmpty(perPersonelRaporBilgileri.sonuc.bilgiStr))
        //    {
        //        throw new Exception(perPersonelRaporBilgileri.sonuc.bilgiStr);
        //    }

        //    if (perPersonelRaporBilgileri.objeler.Count <= 0)
        //    {
        //        throw new Exception(Resources.InsanKaynaklari.FRMPLH007);
        //    }

        //    int satirSayisi = 0;
        //    List<PersonelPersonelRapor> liste = new List<PersonelPersonelRapor>();
        //    PersonelPersonelRapor prTemp = new PersonelPersonelRapor();
        //    foreach (PersonelPersonelRapor pr in perPersonelRaporBilgileri.objeler)
        //    {
        //        if (prTemp.tcKimlikNo != pr.tcKimlikNo)
        //        {
        //            if (!string.IsNullOrEmpty(prTemp.tcKimlikNo))
        //                liste.Add(prTemp);
        //            prTemp = pr;
        //            prTemp.seviyesi = 0;
        //            prTemp.LUSeviyesi = 0;
        //            prTemp.dil = "";
        //            prTemp.notuDil = 0;
        //            prTemp.duzeyDil = "";
        //            prTemp.yilDil = 0;
        //            prTemp.ayDil = 0;
        //            prTemp.belgeTurDil = 0;

        //            satirSayisi++;
        //        }

        //        switch (pr.seviyesi)
        //        {
        //            case 0:
        //                if (prTemp.seviyesi < 1)
        //                {
        //                    prTemp.seviyesi = 0;
        //                    prTemp.seviyesiText = "Belirsiz";
        //                    prTemp.okulAdi = pr.okulAdi;
        //                    prTemp.bitirmeTarihi = pr.bitirmeTarihi;
        //                }
        //                break;
        //            case 1:
        //                if (prTemp.seviyesi < 1)
        //                {
        //                    prTemp.seviyesi = 1;
        //                    prTemp.seviyesiText = "İlk Öğretim";
        //                    prTemp.okulAdi = pr.okulAdi;
        //                    prTemp.bitirmeTarihi = pr.bitirmeTarihi;
        //                }
        //                break;
        //            case 2:
        //                if (prTemp.seviyesi < 2)
        //                {
        //                    prTemp.seviyesi = 2;
        //                    prTemp.seviyesiText = "Orta Okul";
        //                    prTemp.okulAdi = pr.okulAdi;
        //                    prTemp.bitirmeTarihi = pr.bitirmeTarihi;
        //                }
        //                break;
        //            case 3:
        //                if (prTemp.seviyesi < 4)
        //                {
        //                    prTemp.seviyesi = 3;
        //                    prTemp.seviyesiText = "Lise";
        //                    prTemp.okulAdi = pr.okulAdi;
        //                    prTemp.bitirmeTarihi = pr.bitirmeTarihi;
        //                }
        //                break;
        //            case 4:
        //                if (prTemp.seviyesi < 5)
        //                {
        //                    prTemp.seviyesi = 4;
        //                    prTemp.seviyesiText = "Ön Lisans";
        //                    prTemp.okulAdi = pr.okulAdi;
        //                    prTemp.bitirmeTarihi = pr.bitirmeTarihi;
        //                }
        //                break;
        //            case 5:
        //                if (prTemp.seviyesi < 5)
        //                {
        //                    prTemp.seviyesi = 5;
        //                    prTemp.seviyesiText = "Lisans";
        //                    prTemp.okulAdi = pr.okulAdi;
        //                    prTemp.bitirmeTarihi = pr.bitirmeTarihi;
        //                }
        //                break;
        //            case 6:
        //                if (prTemp.LUSeviyesi < 6)
        //                {
        //                    prTemp.LUSeviyesi = 6;
        //                    prTemp.LUSeviyesiText = "Yüksek Lisans";
        //                    prTemp.LUOkulAdi = pr.okulAdi;
        //                    prTemp.LUBitirmeTarihi = pr.bitirmeTarihi;
        //                }
        //                break;
        //            default:
        //                if (prTemp.LUSeviyesi < 7)
        //                {
        //                    prTemp.LUSeviyesi = 7;
        //                    prTemp.LUSeviyesiText = "Doktora";
        //                    prTemp.LUOkulAdi = pr.okulAdi;
        //                    prTemp.LUBitirmeTarihi = pr.bitirmeTarihi;
        //                }
        //                break;
        //        }

        //        if (pr.belgeTurDil == 3)
        //            if (pr.yilDil > prTemp.yilDil || (pr.yilDil == prTemp.yilDil && pr.ayDil > prTemp.ayDil))
        //            {
        //                prTemp.dil = string.IsNullOrEmpty(pr.dil) ? "" : pr.dil;
        //                prTemp.notuDil = pr.notuDil > 0 ? pr.notuDil : 0;
        //                prTemp.duzeyDil = string.IsNullOrEmpty(pr.duzeyDil) ? "" : pr.duzeyDil;
        //                prTemp.yilDil = pr.yilDil;
        //                prTemp.ayDil = pr.ayDil;
        //                prTemp.belgeTurDil = pr.belgeTurDil;
        //            }
        //    }
        //    if (!string.IsNullOrEmpty(prTemp.tcKimlikNo))
        //        liste.Add(prTemp);

        //    Tablo XLS = GenelIslemler.NewTablo();
        //    sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);

        //    int kaynakSatir = 0;
        //    int satir = 0;
        //    int sutun = 0;
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
        //    satir = kaynakSatir;
        //    XLS.SatirKopyalaAc(kaynakSatir, satirSayisi - 1, kaynakSatir + 1);

        //    foreach (PersonelPersonelRapor pr in liste.ToArray<PersonelPersonelRapor>())
        //    {
        //        int bYas = DateTime.Now.Year - pr.dogumTarihi.Yil;
        //        string bYasAraligi = (bYas > 19 && bYas < 60) ? (bYas.ToString()).Substring(0, 1) + "0 - " + (bYas.ToString()).Substring(0, 1) + "9" : bYas >= 60 ? "60 ve üzeri" : "TANIMSIZ";

        //        satir++;
        //        XLS.HucreDegerYaz(satir, 0, satir - kaynakSatir);
        //        XLS.HucreDegerYaz(satir, 1, pr.ad + " " + pr.soyad);
        //        XLS.HucreDegerYaz(satir, 2, pr.unvan);
        //        XLS.HucreDegerYaz(satir, 3, pr.cinsiyet == 1 ? Resources.InsanKaynaklari.FRMPKS007 : Resources.InsanKaynaklari.FRMPKS008);
        //        XLS.HucreDegerYaz(satir, 4, pr.dogumTarihi.ToString());
        //        XLS.HucreDegerYaz(satir, 5, bYas);
        //        XLS.HucreDegerYaz(satir, 6, bYasAraligi);
        //        XLS.HucreDegerYaz(satir, 7, pr.okulAdi + (pr.bitirmeTarihi.Yil > 0 ? (" (" + pr.bitirmeTarihi.Yil + ")") : ""));
        //        XLS.HucreDegerYaz(satir, 8, pr.seviyesiText);
        //        XLS.HucreDegerYaz(satir, 9, pr.LUOkulAdi + (pr.LUBitirmeTarihi.Yil > 0 ? (" (" + pr.LUBitirmeTarihi.Yil + ")") : ""));
        //        XLS.HucreDegerYaz(satir, 10, pr.LUSeviyesiText);
        //        XLS.HucreDegerYaz(satir, 11, pr.dil);
        //        XLS.HucreDegerYaz(satir, 12, pr.notuDil > 0 ? pr.notuDil.ToString() : "");
        //        XLS.HucreDegerYaz(satir, 13, pr.duzeyDil);
        //        XLS.HucreDegerYaz(satir, 14, pr.yilDil > 0 ? pr.yilDil.ToString() : "");
        //        XLS.HucreDegerYaz(satir, 15, pr.belgeTurDil > 0 ? Resources.InsanKaynaklari.ResourceManager.GetString("FRMPDLB" + pr.belgeTurDil.ToString("d3")) : "");
        //    }

        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        //public string RaporPersonelEgitim(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    Kullanici kullanan = (Kullanici)o_kullanan;
        //    IIKYServis servisIKY;

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    TNS.IKY.Personel kriterPersonel = new TNS.IKY.Personel();
        //    string birim = (string)raporParam["KurKod"];

        //    TNS.IKY.PersonelKurs kriterKurs = new TNS.IKY.PersonelKurs();

        //    if (!string.IsNullOrEmpty((string)raporParam["EgitiminAdi"]))
        //        kriterKurs.kursAdi = (string)raporParam["EgitiminAdi"];

        //    if (OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0) > 0)
        //        kriterKurs.yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);

        //    if (!string.IsNullOrEmpty((string)raporParam["Donem"]))
        //        kriterKurs.donem = (string)raporParam["Donem"];

        //    if (!string.IsNullOrEmpty((string)raporParam["EgitimiVerenKurulus"]))
        //        kriterKurs.aldigiYer = (string)raporParam["EgitimiVerenKurulus"];

        //    if (!string.IsNullOrEmpty((string)raporParam["AlindigiYer"]))
        //        kriterKurs.egitimYeri = (int)raporParam["AlindigiYer"];

        //    ObjectArray personelEgitimBilgileri = servisIKY.PersonelListeleEgitimRapor(kullanan, kriterPersonel, kriterKurs);
        //    if (personelEgitimBilgileri == null || !personelEgitimBilgileri.sonuc.islemSonuc)
        //    {
        //        throw new Exception(personelEgitimBilgileri.sonuc.hataStr);
        //    }
        //    if (!string.IsNullOrEmpty(personelEgitimBilgileri.sonuc.bilgiStr))
        //    {
        //        throw new Exception(personelEgitimBilgileri.sonuc.bilgiStr);
        //    }

        //    if (personelEgitimBilgileri.objeler.Count <= 0)
        //    {
        //        throw new Exception(Resources.InsanKaynaklari.FRMPLH007);
        //    }

        //    Tablo XLS = GenelIslemler.NewTablo();

        //    sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);

        //    int kaynakSatir = 0;
        //    int satir = 0;
        //    int sutun = 0;
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
        //    satir = kaynakSatir;
        //    XLS.SatirKopyalaAc(kaynakSatir, personelEgitimBilgileri.objeler.Count - 1, kaynakSatir + 1);


        //    foreach (PersonelEgitimRapor per in personelEgitimBilgileri.objeler)
        //    {
        //        satir++;
        //        XLS.HucreDegerYaz(satir, 0, satir - kaynakSatir);
        //        XLS.HucreDegerYaz(satir, 1, per.ad + " " + per.soyad);
        //        XLS.HucreDegerYaz(satir, 2, per.unvan);
        //        XLS.HucreDegerYaz(satir, 3, per.egitiminAdi);
        //        XLS.HucreDegerYaz(satir, 4, per.egitiminTipi);
        //        XLS.HucreDegerYaz(satir, 5, per.baslangicTarihi.ToString());
        //        XLS.HucreDegerYaz(satir, 6, per.bitisTarihi.ToString());
        //        XLS.HucreDegerYaz(satir, 7, per.dersSaati);
        //        XLS.HucreDegerYaz(satir, 8, per.egitimiVerenKurulus);
        //        XLS.HucreDegerYaz(satir, 9, per.egitimYeri);
        //        XLS.HucreDegerYaz(satir, 10, per.aciklama);
        //        XLS.HucreDegerYaz(satir, 11, per.donemi);
        //        XLS.HucreDegerYaz(satir, 12, per.yili > 0 ? per.yili.ToString() : "");
        //        XLS.HucreDegerYaz(satir, 13, per.dosyaNo);
        //    }
        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        //public string RaporPersonelUnvanCinsiyet(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    Kullanici kullanan = (Kullanici)o_kullanan;
        //    IIKYServis servisIKY;

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();



        //    TNS.IKY.Personel kriter = new TNS.IKY.Personel();
        //    string birim = (string)raporParam["KurKod"];

        //    ObjectArray perPersoneller = servisIKY.PersonelListeleUnvanKadro(kullanan, kriter);
        //    if (perPersoneller == null || !perPersoneller.sonuc.islemSonuc)
        //    {
        //        throw new Exception(perPersoneller.sonuc.hataStr);
        //    }
        //    if (!string.IsNullOrEmpty(perPersoneller.sonuc.bilgiStr))
        //    {
        //        throw new Exception(perPersoneller.sonuc.bilgiStr);
        //    }

        //    int satirSayisiTut = 0;
        //    string unvanYedek = "";
        //    foreach (TNS.IKY.PersonelUnvanKadro unvanKadro in perPersoneller.objeler)
        //        if (unvanKadro.unvanAd != unvanYedek)
        //        {
        //            unvanYedek = unvanKadro.unvanAd;
        //            satirSayisiTut++;
        //        }

        //    if (satirSayisiTut <= 0)
        //    {
        //        throw new Exception(Resources.InsanKaynaklari.FRMPLH007);
        //    }

        //    Tablo XLS = GenelIslemler.NewTablo();

        //    sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);

        //    int kaynakSatir = 0;
        //    int satir = 0;
        //    int sutun = 0;
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
        //    satir = kaynakSatir;
        //    XLS.SatirKopyalaAc(kaynakSatir, satirSayisiTut - 1, kaynakSatir + 1);

        //    unvanYedek = "";
        //    foreach (TNS.IKY.PersonelUnvanKadro unvanKadro in perPersoneller.objeler)
        //    {
        //        if (unvanKadro.unvanAd != unvanYedek)
        //        {
        //            unvanYedek = unvanKadro.unvanAd;
        //            satir++;
        //        }
        //        XLS.HucreDegerYaz(satir, 0, unvanKadro.unvanAd);
        //        if (unvanKadro.cinsiyet == 1)
        //            XLS.HucreDegerYaz(satir, 1, unvanKadro.cinsiyetAdet);
        //        if (unvanKadro.cinsiyet == 2)
        //            XLS.HucreDegerYaz(satir, 2, unvanKadro.cinsiyetAdet);
        //    }

        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        //public string RaporPersonelKadroDoluBos(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    Kullanici kullanan = (Kullanici)o_kullanan;
        //    IIKYServis servisIKY;

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    TNS.IKY.KadroKontenjan d = new TNS.IKY.KadroKontenjan();

        //    d.organizasyonKopruKod = (string)raporParam["ORGKOD"];
        //    ObjectArray bilgi = servisIKY.UnvanVeKadroDereceBazliPersonelSayilariListele(kullanan, d);
        //    if (!bilgi.sonuc.islemSonuc)
        //    {
        //        throw new Exception(bilgi.sonuc.hataStr);
        //    }
        //    if (bilgi.objeler.Count <= 0)
        //    {
        //        throw new Exception(bilgi.sonuc.bilgiStr);
        //    }

        //    Tablo XLS = GenelIslemler.NewTablo();

        //    sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);
        //    int satir = 0;
        //    int sutun = 0;
        //    int kaynakSatir = 0;
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

        //    XLS.HucreAdBulYaz("BirimAd", (string)raporParam["BirimAd"]);

        //    satir = kaynakSatir;

        //    int kadroDereceSayi = 15;

        //    for (int i = 0; i < bilgi.objeler.Count; i++)
        //    {
        //        UnvanVeKadroDereceBazliPersonelSayi iPerSayi = (UnvanVeKadroDereceBazliPersonelSayi)bilgi.objeler[i];
        //        if (iPerSayi.unvan.kod <= 0 || iPerSayi.kadroDerece < 1 || iPerSayi.kadroDerece > 15)
        //        {
        //            bilgi.objeler.RemoveAt(i);
        //            i--;
        //            continue;
        //        }

        //        int[,] personelSayilar = new int[kadroDereceSayi, 2];
        //        personelSayilar[iPerSayi.kadroDerece - 1, 0] = iPerSayi.personelSayi;
        //        personelSayilar[iPerSayi.kadroDerece - 1, 1] = iPerSayi.kontenjan;
        //        bilgi.objeler.RemoveAt(i);
        //        i--;

        //        for (int j = i + 1; j < bilgi.objeler.Count; j++)
        //        {
        //            UnvanVeKadroDereceBazliPersonelSayi jPerSayi = (UnvanVeKadroDereceBazliPersonelSayi)bilgi.objeler[j];
        //            if (jPerSayi.unvan.kod <= 0 || jPerSayi.kadroDerece < 1 || jPerSayi.kadroDerece > 15)
        //            {
        //                bilgi.objeler.RemoveAt(j);
        //                j--;
        //                continue;
        //            }

        //            if (iPerSayi.unvan.kod == jPerSayi.unvan.kod)
        //            {
        //                personelSayilar[jPerSayi.kadroDerece - 1, 0] = jPerSayi.personelSayi;
        //                personelSayilar[jPerSayi.kadroDerece - 1, 1] = jPerSayi.kontenjan;
        //                bilgi.objeler.RemoveAt(j);
        //                j--;
        //            }
        //        }

        //        satir++;
        //        XLS.SatirAc(satir, 1);
        //        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 33, satir, sutun);

        //        XLS.HucreDegerYaz(satir, sutun, iPerSayi.unvan.ad + " (" + iPerSayi.unvan.kod.ToString() + ")");
        //        int[] toplam = new int[2];
        //        for (int k = 0; k < personelSayilar.Length / 2; k++)
        //        {
        //            if (personelSayilar[k, 0] > 0)
        //                XLS.HucreDegerYaz(satir, sutun + 1 + k * 2, personelSayilar[k, 0]);
        //            if (personelSayilar[k, 1] - personelSayilar[k, 0] != 0)
        //                XLS.HucreDegerYaz(satir, sutun + 2 + k * 2, personelSayilar[k, 1] - personelSayilar[k, 0]);

        //            toplam[0] += personelSayilar[k, 0];
        //            toplam[1] += personelSayilar[k, 1];
        //        }

        //        XLS.HucreDegerYaz(satir, sutun + 1 + personelSayilar.Length, toplam[0]);
        //        XLS.HucreDegerYaz(satir, sutun + 2 + personelSayilar.Length, toplam[1] - toplam[0]);
        //        XLS.HucreDegerYaz(satir, sutun + 3 + personelSayilar.Length, toplam[1]);
        //    }

        //    XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        public string RaporPersonelKadroDoluBosIslem(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;
            IIKYServis servisIKY;

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            TNS.IKY.KadroKontenjan kriter = new TNS.IKY.KadroKontenjan();
            kriter.kod = OrtakFonksiyonlar.ConvertToInt((string)raporParam["IslemSiraNo"], 0);
            if (kriter.kod <= 0)
                throw new Exception("İşlem numarası boş bırakılamaz."); // eski Resources.InsanKaynaklari.FRMPRP012

            kriter.organizasyonKadroKod = (string)raporParam["ORGKOD"];
            KadroKontenjanDetay[] detaylar = new KadroKontenjanDetay[0];
            TNS.IKY.KadroKontenjan[] kklar = servisIKY.KadroKontenjanListele(kullanan, kriter, true);
            try
            {
                if (kklar == null || kklar.Length <= 0)
                {
                    // Eski: GenelIslemler.MesajKutusu("Hata", Resources.InsanKaynaklari.FRMKAD016); return "";
                    throw new Exception("Aradığınız kriterlere uygun kayıt bulunamadı.");
                    //throw new Exception(Resources.InsanKaynaklari.FRMKAD016);
                }
                detaylar = kklar[0].detaylar;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            int satirSayisi = 0;
            int tempUnvan = 0;
            foreach (KadroKontenjanDetay kkd in detaylar)
            {
                if (kkd.unvanKod != tempUnvan)
                {
                    satirSayisi++;
                    tempUnvan = kkd.unvanKod;
                }
            }

            Tablo XLS = GenelIslemler.NewTablo();

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("BirimAd", (string)raporParam["BirimAd"]);
            XLS.HucreAdBulYaz("IslemNo", kklar[0].kod.ToString("d6"));
            XLS.HucreAdBulYaz("IslemTuru", kklar[0].islemTur.ToString());
            satir = kaynakSatir;
            XLS.SatirKopyalaAc(kaynakSatir, satirSayisi, kaynakSatir + 1);

            tempUnvan = -1;
            int satirToplam = 0;
            int[] sutunToplam = new int[15];
            for (int i = 0; i < sutunToplam.Length; i++)
                sutunToplam[i] = 0;
            for (int i = 0; i < detaylar.Length; i++)
            {
                if (detaylar[i].unvanKod != tempUnvan)
                {
                    if (i > 0)
                        XLS.HucreDegerYaz(satir, sutun + 17, satirToplam);
                    satir++;
                    tempUnvan = detaylar[i].unvanKod;
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);
                    XLS.HucreDegerYaz(satir, sutun, detaylar[i].unvanAd + " (" + detaylar[i].unvanKod.ToString() + ")");
                    satirToplam = 0;
                }

                if (detaylar[i].unvanKod == tempUnvan && detaylar[i].kontenjan != 0)
                {
                    XLS.HucreDegerYaz(satir, sutun + 1 + detaylar[i].kadroDerece, detaylar[i].kontenjan);
                    sutunToplam[detaylar[i].kadroDerece - 1] += detaylar[i].kontenjan;
                    satirToplam += detaylar[i].kontenjan;
                }

                if (i == detaylar.Length - 1)
                {
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);
                    XLS.HucreDegerYaz(satir, sutun + 17, satirToplam);
                    satir++;
                    //XLS.KoyuYap(satir, sutun, true);
                    XLS.HucreDegerYaz(satir, sutun, "Toplam");
                    for (int j = 0; j < sutunToplam.Length; j++)
                    {
                        //XLS.KoyuYap(satir, sutun + 2 + j, true);
                        XLS.HucreDegerYaz(satir, sutun + 2 + j, sutunToplam[j]);
                    }

                    int genelToplam = 0;
                    for (int j = 0; j < sutunToplam.Length; j++)
                        genelToplam += sutunToplam[j];
                    //XLS.KoyuYap(satir, sutun + 17, true);
                    XLS.HucreDegerYaz(satir, sutun + 17, genelToplam);
                }
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //public string RaporPersonelDil(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    Kullanici kullanan = (Kullanici)o_kullanan;
        //    IIKYServis servisIKY;

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();


        //    TNS.IKY.Personel kriter = new TNS.IKY.Personel();
        //    string birim = (string)raporParam["KurKod"];

        //    ObjectArray perPersoneller = servisIKY.PersonelListele(kullanan, kriter, false, false);
        //    if (perPersoneller == null || !perPersoneller.sonuc.islemSonuc)
        //    {
        //        throw new Exception(perPersoneller.sonuc.hataStr);
        //    }
        //    if (!string.IsNullOrEmpty(perPersoneller.sonuc.bilgiStr))
        //    {
        //        throw new Exception(perPersoneller.sonuc.bilgiStr);
        //    }

        //    int satirSayisiTut = 0;
        //    foreach (TNS.IKY.Personel personel in perPersoneller.objeler)
        //    {
        //        TNS.IKY.PersonelDil pDil = new TNS.IKY.PersonelDil();
        //        pDil.tcKimlikNo = personel.tcKimlikNo;
        //        satirSayisiTut += servisIKY.PersonelDilListele(kullanan, pDil).objeler.Count;
        //    }

        //    if (satirSayisiTut <= 0)
        //    {
        //        throw new Exception(Resources.InsanKaynaklari.FRMPLH007);
        //    }

        //    Tablo XLS = GenelIslemler.NewTablo();
        //    sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);

        //    int kaynakSatir = 0;
        //    int satir = 0;
        //    int sutun = 0;
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
        //    satir = kaynakSatir;
        //    XLS.SatirKopyalaAc(kaynakSatir, satirSayisiTut - 1, kaynakSatir + 1);

        //    int personelSayac = 0;
        //    foreach (TNS.IKY.Personel personel in perPersoneller.objeler)
        //    {
        //        TNS.IKY.PersonelDil pDil = new TNS.IKY.PersonelDil();
        //        pDil.tcKimlikNo = personel.tcKimlikNo;
        //        ObjectArray pdiller = servisIKY.PersonelDilListele(kullanan, pDil);
        //        if (pdiller.objeler.Count > 0)
        //            personelSayac++;
        //        foreach (TNS.IKY.PersonelDil dil in pdiller.objeler)
        //        {
        //            satir++;
        //            XLS.HucreDegerYaz(satir, 0, personelSayac);
        //            XLS.HucreDegerYaz(satir, 1, personel.ad + " " + personel.soyad);
        //            //XLS.HucreDegerYaz(satir, 2, personel.unvanAd);
        //            XLS.HucreDegerYaz(satir, 3, string.IsNullOrEmpty(dil.kod.ToString()) ? "" : dil.kod.ToString());
        //            XLS.HucreDegerYaz(satir, 4, dil.puan > 0 ? dil.puan + "" : "");
        //            XLS.HucreDegerYaz(satir, 5, string.IsNullOrEmpty(dil.duzey) ? "" : dil.duzey);
        //            XLS.HucreDegerYaz(satir, 6, (dil.donemYil > 0 && dil.donemAy > 0) ? dil.donemYil + "/" + dil.donemAy : "");
        //            XLS.HucreDegerYaz(satir, 7, dil.belgeTur > 0 ? Resources.InsanKaynaklari.ResourceManager.GetString("FRMPDLB" + dil.belgeTur.ToString("d3")) : "");
        //        }
        //    }

        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        //public string RaporPersonelKadroBilgisi(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    Kullanici kullanan = (Kullanici)o_kullanan;
        //    IIKYServis servisIKY;

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    TNS.IKY.Personel kriterPersonel = new TNS.IKY.Personel();
        //    string birim = (string)raporParam["KurKod"];

        //    Tablo XLS = GenelIslemler.NewTablo();

        //    sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);

        //    int kaynakSatir = 0;
        //    int satir = 0;
        //    int sutun = 0;
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
        //    satir = kaynakSatir;
        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        //public string RaporPersonelKanGrubu(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    Kullanici kullanan = (Kullanici)o_kullanan;
        //    IIKYServis servisIKY;

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();


        //    TNS.IKY.Personel kriter = new TNS.IKY.Personel();
        //    string birim = (string)raporParam["KurKod"];

        //    kriter.nufus.kanGrubu = (string)raporParam["KanGrubu"];
        //    ObjectArray perPersoneller = servisIKY.PersonelListele(kullanan, kriter, false, false);
        //    if (perPersoneller == null || !perPersoneller.sonuc.islemSonuc)
        //    {
        //        throw new Exception(perPersoneller.sonuc.hataStr);
        //    }
        //    if (!string.IsNullOrEmpty(perPersoneller.sonuc.bilgiStr))
        //    {
        //        throw new Exception(perPersoneller.sonuc.bilgiStr);
        //    }

        //    Tablo XLS = GenelIslemler.NewTablo();
        //    sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);

        //    int kaynakSatir = 0;
        //    int satir = 0;
        //    int sutun = 0;
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
        //    satir = kaynakSatir;
        //    XLS.SatirKopyalaAc(kaynakSatir, perPersoneller.objeler.Count - 1, kaynakSatir + 1);

        //    int personelSayac = 0;
        //    foreach (TNS.IKY.Personel personel in perPersoneller.objeler)
        //    {
        //        personelSayac++;
        //        satir++;
        //        XLS.HucreDegerYaz(satir, 0, personelSayac);
        //        XLS.HucreDegerYaz(satir, 1, personel.ad + " " + personel.soyad);
        //        XLS.HucreDegerYaz(satir, 2, personel.nufus.kanGrubu);
        //    }

        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        //public string RaporPersonelIzinDurumu(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    Kullanici kullanan = (Kullanici)o_kullanan;
        //    IIKYServis servisIKY;

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    TNS.IKY.Personel kriterPersonel = new TNS.IKY.Personel();
        //    //string birim = (string)raporParam["KurKod"];

        //    TNS.IKY.PersonelIzin kriterIzin = new TNS.IKY.PersonelIzin();
        //    //if (!string.IsNullOrEmpty((string)raporParam["Donem"]))
        //    //    kriterIzin.donem = (string)raporParam["Donem"];
        //    string tarih = "01.01." + ((string.IsNullOrEmpty((string)raporParam["yil"])) ? "0000" : (string)raporParam["yil"]);
        //    kriterIzin.baslangicTarihi = new TNSDateTime(tarih);
        //    //kriterIzin.tcKimlikNo = (string)raporParam["TC"];
        //    kriterIzin.izinTurKod = OrtakFonksiyonlar.ConvertToInt((string)raporParam["izinTipi"], 0);
        //    ObjectArray personelIzinBilgileri = servisIKY.PersonelListeleIzinRapor(kullanan, kriterPersonel, kriterIzin);
        //    if (personelIzinBilgileri == null || !personelIzinBilgileri.sonuc.islemSonuc)
        //    {
        //        throw new Exception(personelIzinBilgileri.sonuc.hataStr);
        //    }
        //    if (!string.IsNullOrEmpty(personelIzinBilgileri.sonuc.bilgiStr))
        //    {
        //        throw new Exception(personelIzinBilgileri.sonuc.bilgiStr);
        //    }

        //    if (personelIzinBilgileri.objeler.Count <= 0)
        //    {
        //        throw new Exception(Resources.InsanKaynaklari.FRMPLH007);
        //    }

        //    Tablo XLS = GenelIslemler.NewTablo();

        //    sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);

        //    int kaynakSatir = 0;
        //    int satir = 0;
        //    int sutun = 0;
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
        //    satir = kaynakSatir;
        //    XLS.SatirKopyalaAc(kaynakSatir, personelIzinBilgileri.objeler.Count - 1, kaynakSatir + 1);

        //    int sira = 0;
        //    int tutSatirNo = 0;
        //    string anlikTC = "";

        //    //İzin türlerinin isimlerini al
        //    ObjectArray dler = servisIKY.IzinTurListele(new IzinTur());
        //    Dictionary<int, string> tipler = new Dictionary<int, string>();
        //    foreach (TNS.IKY.IzinTur d in dler.objeler)
        //        tipler.Add(d.kod, d.ad);

        //    foreach (PersonelIzinRapor izinBilgisi in personelIzinBilgileri.objeler)
        //    {
        //        satir++;

        //        if (anlikTC != izinBilgisi.tcKimlikNo)
        //            sira++;
        //        XLS.HucreDegerYaz(satir, 0, sira);
        //        //XLS.HucreDegerYaz(satir, 1, izinBilgisi.ad + " " + izinBilgisi.soyad);
        //        XLS.HucreDegerYaz(satir, 2, izinBilgisi.unvan);
        //        //XLS.HucreDegerYaz(satir, 3, izinBilgisi.sicilNumarasi);
        //        XLS.HucreDegerYaz(satir, 4, izinBilgisi.baslangicTarihi.ToString());
        //        XLS.HucreDegerYaz(satir, 5, izinBilgisi.bitisTarihi.ToString());

        //        XLS.HucreDegerYaz(satir, 6, tipler[izinBilgisi.tip]);
        //        XLS.HucreDegerYaz(satir, 7, izinBilgisi.yillikIzin);

        //        if (anlikTC != izinBilgisi.tcKimlikNo)
        //        {
        //            if (satir - 1 > tutSatirNo)
        //            {
        //                XLS.HucreBirlestir(tutSatirNo, 0, satir - 1, 0);
        //                XLS.HucreBirlestir(tutSatirNo, 1, satir - 1, 1);
        //                XLS.HucreBirlestir(tutSatirNo, 2, satir - 1, 2);
        //                XLS.HucreBirlestir(tutSatirNo, 3, satir - 1, 3);
        //            }

        //            tutSatirNo = satir;
        //            anlikTC = izinBilgisi.tcKimlikNo;
        //        }
        //    }
        //    if (satir - 1 > tutSatirNo)
        //    {
        //        XLS.HucreBirlestir(tutSatirNo, 0, satir, 0);
        //        XLS.HucreBirlestir(tutSatirNo, 1, satir, 1);
        //        XLS.HucreBirlestir(tutSatirNo, 2, satir, 2);
        //        XLS.HucreBirlestir(tutSatirNo, 3, satir, 3);
        //    }
        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        private class IstihdamSekli : IEquatable<IstihdamSekli>
        {
            public IstihdamSekli(int kod, string ad)
            {
                this.kod = kod;
                this.ad = ad;
            }
            public int kod { get; set; }
            public string ad { get; set; }

            public bool Equals(IstihdamSekli x)
            {
                if (x == null) return false;
                return this.kod.Equals(x.kod);
            }
        }

        private string OrganizasyonSemaPersonelYaz(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            string sqlIstihdamlar = @"SELECT S.KOD, S.AD, S.SIRA, COUNT(*)
                            FROM Kullanici.dbo.ORGBIRIM B INNER JOIN Personel.dbo.PERPERSONEL P ON B.KOD=P.ORGANIZASYONKOD
                            INNER JOIN InsanKaynaklari.dbo.IKYPERSONELTIPI S ON S.KOD = P.ISTIHDAMSEKLI 
                            where P.AKTIFMI=1 AND KOPRUKOD IS NOT null {0}
                            group by S.KOD, S.AD, S.SIRA
                            order BY S.SIRA";

            string personelTurler = (string)raporParam["personelTur"];
            VTSorgula vtIstihdamlar = new VTSorgula(String.Format(sqlIstihdamlar, PersonelTurKosulOl("AND", "S.KOD", personelTurler)), "BaglantiSatiriIKY", 0);
            if (!string.IsNullOrWhiteSpace(vtIstihdamlar.hata))
                throw new Exception(vtIstihdamlar.hata);
            List<IstihdamSekli> istihdamlar = new List<IstihdamSekli>();
            while (true)
            {
                DataRow dr = vtIstihdamlar.satirOku();
                if (dr == null) break;
                istihdamlar.Add(new IstihdamSekli(vtIstihdamlar.IntAl(0), vtIstihdamlar.StringAl(1)));
            }
            int satir = 0, sutun = 0, fsatir = 0, fsutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref fsatir, ref fsutun);
            int yukseklik = XLS.SatirYukseklikAl(satir);
            int baslaSatir = satir;
            int genislik = XLS.SutunGenislikAl(21);
            for (int i = 0; i < istihdamlar.Count + 1; i++)
            {
                if (i > 0)
                {
                    XLS.HucreKopyala(fsatir, 21, fsatir + 20, 21, fsatir, 21 + i);
                    XLS.HucreKopyala(satir - 1, 21, satir - 1, 21, satir - 1, 21 + i);
                }
                string deger;
                if (i < istihdamlar.Count)
                    deger = istihdamlar[i].ad;
                else
                    deger = "Toplam";
                XLS.HucreDegerYaz(satir - 1, 21 + i, deger);
            }
            XLS.SutunGenislikAyarla(21 + 1, 21 + istihdamlar.Count, genislik);
            ToplamAl toplamAl = new ToplamAl(2, istihdamlar.Count);

            string eskiKopru = null;
            int[] dizi = new int[20];
            DiziDegerAta(dizi, 0, 0);

            string sqlSay = @"SELECT ADI, KOPRUKOD, P.ISTIHDAMSEKLI, S.AD, COUNT(*)
                            FROM Kullanici.dbo.ORGBIRIM B LEFT OUTER JOIN Personel.dbo.PERPERSONEL P ON B.KOD=P.ORGANIZASYONKOD {0}
                            LEFT OUTER JOIN InsanKaynaklari.dbo.IKYPERSONELTIPI S ON S.KOD = P.ISTIHDAMSEKLI 
                            where P.AKTIFMI=1 AND KOPRUKOD IS NOT null
                            group by ADI, BASLANGICTARIH, BITISTARIH, KOPRUKOD,  P.ISTIHDAMSEKLI, S.AD
                            order BY KOPRUKOD";
            VTSorgula vtSayilar = new VTSorgula(String.Format(sqlSay, PersonelTurKosulOl("AND", "P.ISTIHDAMSEKLI", personelTurler)), "BaglantiSatiriIKY", 0);
            int maksKir = -1;
            while (true)
            {
                DataRow dr = vtSayilar.satirOku();
                if (dr == null) break;
                string yeniKopru = vtSayilar.StringAl(1);
                if (eskiKopru != yeniKopru)
                {
                    if (eskiKopru != null)
                    {
                        SayilariYaz(XLS, satir, toplamAl, 1);
                        toplamAl.SatirSifirla(1);
                        satir++;
                    }
                    int kirilim = (yeniKopru.Length / 3) - 1;
                    if (kirilim > maksKir) maksKir = kirilim;
                    dizi[kirilim]++;
                    DiziDegerAta(dizi, kirilim + 1, 0);
                    XLS.HucreKopyala(fsatir + kirilim, 0, fsatir + kirilim, 22 + istihdamlar.Count, satir, 0);
                    XLS.HucreDegerYaz(satir, 20, vtSayilar.StringAl(0));
                    XLS.HucreDegerYaz(satir, 0 + kirilim, FormatliRakam(dizi[kirilim], kirilim) + ".");
                    eskiKopru = yeniKopru;
                }
                int isKod = vtSayilar.IntAl(2);
                int isSay = vtSayilar.IntAl(4);
                int yer = istihdamlar.IndexOf(new IstihdamSekli(isKod, ""));
                if (yer >= 0)
                {
                    toplamAl.DegerAta(1, yer, isSay);
                    toplamAl.DegerArttir(0, yer, isSay);
                }
            }
            if (eskiKopru != null)
            {
                SayilariYaz(XLS, satir, toplamAl, 1);
                satir++;
            }
            XLS.HucreKopyala(fsatir + 20, 0, fsatir + 20, 22 + istihdamlar.Count, satir, 0);
            SayilariYaz(XLS, satir, toplamAl, 0);
            satir++;
            XLS.SatirYukseklikAyarla(baslaSatir, satir, yukseklik);
            XLS.SutunGenislikAyarla(maksKir + 1, 19, 0);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string PersonelTurKosulOl(string baglac, string alan, string sPersonelTurler)
        {
            string donusDeger = "";
            if (!string.IsNullOrWhiteSpace(sPersonelTurler))
            {
                string[] personelTurler = sPersonelTurler.Split(',', ';');
                foreach (string item in personelTurler)
                {
                    int pturKod = OrtakFonksiyonlar.ConvertToInt(item, 0);
                    if (pturKod > 0)
                    {
                        if (donusDeger == "")
                            donusDeger = baglac + " " + alan + " IN (" + pturKod.ToString();
                        else
                            donusDeger += ", " + pturKod.ToString();
                    }
                }
                if (donusDeger != "")
                {
                    donusDeger += ")";
                }
            }
            return donusDeger;
        }

        //protected string IsimliKadroListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    string personelTuru = (string)raporParam["personelTur"];
        //    string personelKod = (string)raporParam["personelKod"];
        //    string organizasyonKodu = (string)raporParam["ORGKOD"];
        //    int siralamaListesi = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["siralamaListesi"]), 0);

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    var list = servisIKY.KadroRaporlari(kullanan, personelTuru, raporKod, personelKod, organizasyonKodu, siralamaListesi);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Kadrolar";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("Birim");
        //    tab.Columns.Add("Isim");
        //    tab.Columns.Add("KadroDerece");
        //    tab.Columns.Add("EkGosterge");
        //    tab.Columns.Add("PersonelTur");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (PersonelKadroRapor k in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = k.siraNo;
        //            row["Birim"] = k.birim;
        //            row["Isim"] = k.isim;
        //            row["KadroDerece"] = k.kadroDerece;
        //            row["EkGosterge"] = k.kEkGosterge;
        //            row["PersonelTur"] = k.statu;
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["Birim"] = "";
        //        row["Isim"] = "";
        //        row["KadroDerece"] = "";
        //        row["EkGosterge"] = "";
        //        row["PersonelTur"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    Document doc = new Document(sablonAd);

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        protected string IsimliKadroListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            string personelKod = (string)raporParam["personelKod"];
            string organizasyonKodu = (string)raporParam["ORGKOD"];
            int siralamaListesi = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["siralamaListesi"]), 0);

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            var list = servisIKY.KadroRaporlari(kullanan, personelTuru, ekBilgi, personelKod, organizasyonKodu, siralamaListesi);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            if (list.ObjeSayisi > 0)
            {
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                string baslik = "İSİMLİ KADRO LİSTESİ";

                XLS.HucreAdBulYaz("baslik", baslik);

                satir = kaynakSatir;

                foreach (PersonelKadroRapor pa in list.objeler)
                {
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, pa.siraNo);
                    XLS.HucreDegerYaz(satir, 1, pa.birim);
                    XLS.HucreDegerYaz(satir, 2, pa.isim);
                    XLS.HucreDegerYaz(satir, 3, pa.kadroDerece);
                    XLS.HucreDegerYaz(satir, 4, pa.kEkGosterge);
                    XLS.HucreDegerYaz(satir, 5, "");
                    XLS.HucreDegerYaz(satir, 6, pa.statu);
                    XLS.HucreDegerYaz(satir, 7, "");

                    satir++;
                }

                XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(600));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //protected string IsimsizKadroListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    string personelTuru = (string)raporParam["personelTur"];
        //    string personelKod = (string)raporParam["personelKod"];
        //    string organizasyonKodu = (string)raporParam["ORGKOD"];

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    var list = servisIKY.KadroRaporlari(kullanan, personelTuru, raporKod, personelKod, organizasyonKodu, 0);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Kadrolar";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("Sinif");
        //    tab.Columns.Add("Unvan");
        //    tab.Columns.Add("Adet");
        //    tab.Columns.Add("KadroDerece");
        //    tab.Columns.Add("EkGosterge");
        //    tab.Columns.Add("PersonelTur");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (PersonelKadroRapor k in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = k.siraNo;
        //            row["Sinif"] = k.sinif;
        //            row["Unvan"] = k.unvan;
        //            row["Adet"] = k.adet;
        //            row["KadroDerece"] = k.kadroDerece;
        //            row["EkGosterge"] = k.kEkGosterge;
        //            row["PersonelTur"] = k.statu;
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["Sinif"] = "";
        //        row["Unvan"] = "";
        //        row["Adet"] = "";
        //        row["KadroDerece"] = "";
        //        row["EkGosterge"] = "";
        //        row["PersonelTur"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    Document doc = new Document(sablonAd);

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        protected string IsimsizKadroListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            string personelKod = (string)raporParam["personelKod"];
            string organizasyonKodu = (string)raporParam["ORGKOD"];

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            var list = servisIKY.KadroRaporlari(kullanan, personelTuru, ekBilgi, personelKod, organizasyonKodu, 0);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            if (list.ObjeSayisi > 0)
            {
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                string baslik = "İSİMSİZ KADRO LİSTESİ";

                XLS.HucreAdBulYaz("baslik", baslik);

                satir = kaynakSatir;

                foreach (PersonelKadroRapor pa in list.objeler)
                {
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, pa.siraNo);
                    XLS.HucreDegerYaz(satir, 1, pa.sinif);
                    XLS.HucreDegerYaz(satir, 2, pa.unvan);
                    XLS.HucreDegerYaz(satir, 3, pa.adet);
                    XLS.HucreDegerYaz(satir, 4, pa.kadroDerece);
                    XLS.HucreDegerYaz(satir, 5, pa.kEkGosterge);
                    XLS.HucreDegerYaz(satir, 6, pa.statu);

                    satir++;
                }

                XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(600));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //protected string KadroDereceDurumListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    string personelTuru = (string)raporParam["personelTur"];
        //    string personelKod = (string)raporParam["personelKod"];
        //    string organizasyonKodu = (string)raporParam["ORGKOD"];
        //    int siralamaListesi = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["siralamaListesi"]), 0);

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    var list = servisIKY.KadroRaporlari(kullanan, personelTuru, raporKod, personelKod, organizasyonKodu, siralamaListesi);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Kadrolar";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("Ad");
        //    tab.Columns.Add("Unvan");
        //    tab.Columns.Add("PersonelTur");
        //    tab.Columns.Add("KadroDerece");
        //    tab.Columns.Add("KDerece");
        //    tab.Columns.Add("KKademe");
        //    tab.Columns.Add("KEkGosterge");
        //    tab.Columns.Add("EDerece");
        //    tab.Columns.Add("EKademe");
        //    tab.Columns.Add("EEkGosterge");
        //    tab.Columns.Add("Ogrenim");
        //    tab.Columns.Add("YDerece");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (PersonelKadroRapor k in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = k.siraNo;
        //            row["Ad"] = k.isim;
        //            row["Unvan"] = k.unvan;
        //            row["PersonelTur"] = k.statu;
        //            row["KadroDerece"] = k.kadroDerece;
        //            row["KDerece"] = k.kDerece;
        //            row["KKademe"] = k.kKademe;
        //            row["KEkGosterge"] = k.kEkGosterge;
        //            row["EDerece"] = k.eDerece;
        //            row["EKademe"] = k.eKademe;
        //            row["EEkGosterge"] = k.eEkGosterge;
        //            row["Ogrenim"] = k.derece.ogrenim;
        //            row["YDerece"] = k.derece.derece;
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["Ad"] = "";
        //        row["Unvan"] = "";
        //        row["PersonelTur"] = "";
        //        row["KadroDerece"] = "";
        //        row["KDerece"] = "";
        //        row["KKademe"] = "";
        //        row["KEkGosterge"] = "";
        //        row["EDerece"] = "";
        //        row["EKademe"] = "";
        //        row["EEkGosterge"] = "";
        //        row["Ogrenim"] = "";
        //        row["YDerece"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    Document doc = new Document(sablonAd);
        //    DocumentBuilder docb = new DocumentBuilder(doc);
        //    if (raporKod == "IK036")
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "RaporAd", "PERSONEL EN SON HİZMET BELGESİ DURUM LİSTESİ");
        //    else
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "RaporAd", "PERSONEL KADRO DERECE LİSTESİ");

        //    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Toplam", "Toplam Kayıt: " + list.ObjeSayisi.ToString());

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        protected string KadroDereceDurumListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            string personelKod = (string)raporParam["personelKod"];
            string organizasyonKodu = (string)raporParam["ORGKOD"];
            int siralamaListesi = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["siralamaListesi"]), 0);

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            var list = servisIKY.KadroRaporlari(kullanan, personelTuru, ekBilgi, personelKod, organizasyonKodu, siralamaListesi);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            if (list.ObjeSayisi > 0)
            {
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                string baslik = "";

                if (ekBilgi == "IK036")
                    baslik = "PERSONEL EN SON HİZMET BELGESİ DURUM LİSTESİ";
                else
                    baslik = "PERSONEL KADRO DERECE LİSTESİ";

                XLS.HucreAdBulYaz("baslik", baslik);

                satir = kaynakSatir;

                foreach (PersonelKadroRapor pa in list.objeler)
                {
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, pa.siraNo);
                    XLS.HucreDegerYaz(satir, 1, pa.isim);
                    XLS.HucreDegerYaz(satir, 2, pa.unvan);
                    XLS.HucreDegerYaz(satir, 3, pa.statu);
                    XLS.HucreDegerYaz(satir, 4, pa.kadroDerece);
                    XLS.HucreDegerYaz(satir, 5, pa.kDerece);
                    XLS.HucreDegerYaz(satir, 6, pa.kKademe);
                    XLS.HucreDegerYaz(satir, 7, pa.kEkGosterge);
                    XLS.HucreDegerYaz(satir, 8, pa.eDerece);
                    XLS.HucreDegerYaz(satir, 9, pa.eKademe);
                    XLS.HucreDegerYaz(satir, 10, pa.eEkGosterge);
                    XLS.HucreDegerYaz(satir, 11, pa.derece.ogrenim);
                    XLS.HucreDegerYaz(satir, 12, pa.derece.derece);

                    satir++;
                }

                XLS.HucreDegerYaz(satir, 10, "Toplam Kayıt: " + list.ObjeSayisi.ToString());
                XLS.HucreBirlestir(satir, 10, satir, 12);
                XLS.KoyuYap(satir, 10, true);
                XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(900));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //protected string BakmaklaYukumluListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    string personelTuru = (string)raporParam["personelTur"];
        //    string personelKod = (string)raporParam["personelKod"];
        //    string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
        //    int yukumlu = (int)raporParam["yukumlu"];
        //    int aktiflik = (int)raporParam["aktiflik"];

        //    Document doc = new Document(sablonAd);
        //    DocumentBuilder docb = new DocumentBuilder(doc);

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    var list = servisIKY.BakmaklaYukumluListesi(kullanan, personelKod, personelTuru, organizasyonKopruKodu, yukumlu, aktiflik);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    if (yukumlu <= 0)
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "tur", "AİLE");
        //    else
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "tur", "BAKMAKLA YÜKÜMLÜ");

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Yukumlu";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("PersonelAd");
        //    tab.Columns.Add("Unvan");

        //    DataTable tabIc = new DataTable();
        //    tabIc.TableName = "IcListe";
        //    tabIc.Columns.Add("IcSiraNo");
        //    tabIc.Columns.Add("Isim");
        //    tabIc.Columns.Add("Yakinlik");
        //    tabIc.Columns.Add("DogumTarihi");
        //    tabIc.Columns.Add("Ogrenim");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        int cnt = 1;
        //        int pKod = 0;
        //        foreach (TNS.IKY.PersonelAile a in list.objeler)
        //        {
        //            int cntIc = 1;
        //            DataRow row = tabIc.NewRow();
        //            row["IcSiraNo"] = cntIc;
        //            row["Isim"] = a.ad;
        //            row["Yakinlik"] = a.yakinlikDerecesiAd;
        //            row["DogumTarihi"] = a.dogumTarihi.ToString();
        //            row["Ogrenim"] = a.ogrenimAd;
        //            tabIc.Rows.Add(row);
        //            if (pKod != a.personelKod)
        //            {
        //                DataRow rowDıs = tab.NewRow();
        //                rowDıs["SiraNo"] = cnt;
        //                rowDıs["PersonelAd"] = a.personelAdi;
        //                rowDıs["Unvan"] = a.unvan;
        //                tab.Rows.Add(rowDıs);
        //                cnt++;
        //            }
        //            pKod = a.personelKod;
        //            cntIc++;
        //        }
        //    }
        //    else
        //    {
        //        //DataRow row = tab.NewRow();
        //        //row["SiraNo"] = "";
        //        //row["PersonelAd"] = "";
        //        //row["TC"] = "";
        //        //row["Isim"] = "";
        //        //row["Cinsiyet"] = "";
        //        //row["Yakinlik"] = "";
        //        //row["DogumTarihi"] = "";
        //        //row["Ogrenim"] = "";
        //        //row["Meslek"] = "";
        //        //row["YBitisTarihi"] = "";
        //        //tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    doc.MailMerge.ExecuteWithRegions(tab);
        //    doc.MailMerge.ExecuteWithRegions(tabIc);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        protected string AileListesiYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            string personelKod = (string)raporParam["personelKod"];
            string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
            int yukumlu = (int)raporParam["yukumlu"];
            int aktiflik = (int)raporParam["aktiflik"];

            Kullanici kullanan = (Kullanici)o_kullanan;
            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            var list = servisIKY.BakmaklaYukumluListesi(kullanan, personelKod, personelTuru, organizasyonKopruKodu, yukumlu, aktiflik);
            if (list.ObjeSayisi > 0)
            {
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                if (yukumlu <= 0)
                    XLS.HucreAdBulYaz("baslik", "PERSONELİN AİLE LİSTESİ");
                else
                    XLS.HucreAdBulYaz("baslik", "PERSONELİN BAKMAKLA YÜKÜMLÜ AİLE LİSTESİ");

                satir = kaynakSatir - 1;
                int cnt = 1;
                int cntIc = 0;
                int Tcocuk = 0, kucukCocuk = 0;
                string esiCalisiyor = "";
                int oncPer = -1;
                foreach (TNS.IKY.PersonelAile pa in list.objeler)
                {
                    if (oncPer != pa.personelKod)
                    {
                        if (oncPer != -1)
                        {
                            satir++;
                            XLS.HucreDegerYaz(satir, 1, "Eşi Çalışıyor: " + esiCalisiyor);
                            XLS.HucreDegerYaz(satir, 2, "6 Yaş Altı Çocuk Sayısı: " + kucukCocuk);
                            XLS.HucreDegerYaz(satir, 3, "6 Yaş Üstü Çocuk Sayısı: " + (Tcocuk - kucukCocuk));
                            XLS.HucreDegerYaz(satir, 4, "Toplam Aile Yardım Puanı: ");
                        }

                        oncPer = pa.personelKod;
                        satir++;
                        XLS.HucreDegerYaz(satir, 0, cnt);
                        XLS.HucreDegerYaz(satir, 1, pa.personelAdi);
                        XLS.HucreDegerYaz(satir, 2, pa.unvan);
                        XLS.ArkaPlanRenk(satir, 0, satir, 5, TabloRenk.GRAY_25);
                        cnt++;
                        Tcocuk = 0; kucukCocuk = 0; esiCalisiyor = "";
                        cntIc = 1;
                    }

                    satir++;
                    XLS.HucreDegerYaz(satir, 1, cntIc);
                    XLS.HucreDegerYaz(satir, 2, pa.ad);
                    XLS.HucreDegerYaz(satir, 3, pa.yakinlikDerecesiAd);
                    XLS.HucreDegerYaz(satir, 4, pa.ogrenimAd);
                    XLS.HucreDegerYaz(satir, 5, pa.dogumTarihi.ToString());
                    cntIc++;
                    if (pa.yakinlik == ENUMYakinlikDerecesi.Kizi || pa.yakinlik == ENUMYakinlikDerecesi.Oglu)
                    {
                        Tcocuk++;
                        if (pa.yas < 6)
                            kucukCocuk++;
                    }
                    if (pa.yakinlik == ENUMYakinlikDerecesi.Esi && pa.durumKayitKod == ((int)ENUMSozlukUnsurlari.YAKININDURUMU).ToString().PadLeft(2, '0') + "." + ((int)ENUMYakininDurumu.Calisiyor).ToString().PadLeft(5, '0'))
                        esiCalisiyor = "√";
                }
                if (oncPer != -1)
                {
                    satir++;
                    XLS.HucreDegerYaz(satir, 1, "Eşi Çalışıyor: " + esiCalisiyor);
                    XLS.HucreDegerYaz(satir, 2, "6 Yaş Altı Çocuk Sayısı: " + kucukCocuk);
                    XLS.HucreDegerYaz(satir, 3, "6 Yaş Üstü Çocuk Sayısı: " + (Tcocuk - kucukCocuk));
                    XLS.HucreDegerYaz(satir, 4, "Toplam Aile Yardım Puanı: ");
                }

                XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //protected string OgrenimDurumListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    string personelTuru = (string)raporParam["personelTur"];
        //    string personelKod = (string)raporParam["personelKod"];
        //    int aktiflik = (int)raporParam["aktiflik"];
        //    string organizasyonKopruKodu = (string)raporParam["ORGKOD"];

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    ObjectArray list = servisIKY.OgrenimDurumuListesi(kullanan, personelKod, personelTuru, organizasyonKopruKodu, aktiflik);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Ogrenimler";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("PersonelAdi");
        //    tab.Columns.Add("OkulAdi");
        //    tab.Columns.Add("OgrenimTipi");
        //    tab.Columns.Add("Unvan");
        //    tab.Columns.Add("Birim");
        //    tab.Columns.Add("Yas");
        //    tab.Columns.Add("Statu");
        //    tab.Columns.Add("kurumdakiSure");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (OgrenimRaporu a in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = a.siraNo;
        //            row["PersonelAdi"] = a.personelAd;
        //            row["OkulAdi"] = a.okulAdi;
        //            row["OgrenimTipi"] = a.okulTipi;
        //            row["Unvan"] = a.unvan;
        //            row["Birim"] = a.birim;
        //            row["Yas"] = a.yasGunu;
        //            row["Statu"] = a.statu;
        //            row["kurumdakiSure"] = Hizmet.YilAyGunDondur(a.kurumHizmetGunu);
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["PersonelAdi"] = "";
        //        row["OkulAdi"] = "";
        //        row["OgrenimTipi"] = "";
        //        row["Unvan"] = "";
        //        row["Birim"] = "";
        //        row["Yas"] = "";
        //        row["Statu"] = "";
        //        row["kurumdakiSure"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    Document doc = new Document(sablonAd);

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        protected string OgrenimDurumListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            string personelKod = (string)raporParam["personelKod"];
            string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
            int aktiflik = (int)raporParam["aktiflik"];

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            ObjectArray list = servisIKY.OgrenimDurumuListesi(kullanan, personelKod, personelTuru, organizasyonKopruKodu, aktiflik);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            if (list.ObjeSayisi > 0)
            {
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                string baslik = "EN SON ÖĞRENİM DURUMU";

                XLS.HucreAdBulYaz("baslik", baslik);

                satir = kaynakSatir;

                foreach (OgrenimRaporu pa in list.objeler)
                {
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, pa.siraNo);
                    XLS.HucreDegerYaz(satir, 1, pa.personelAd);
                    XLS.HucreDegerYaz(satir, 2, pa.unvan);
                    XLS.HucreDegerYaz(satir, 3, pa.birim);
                    XLS.HucreDegerYaz(satir, 4, pa.okulTipi);
                    XLS.HucreDegerYaz(satir, 5, pa.okulAdi);
                    XLS.HucreDegerYaz(satir, 6, pa.yasGunu);
                    XLS.HucreDegerYaz(satir, 7, pa.statu);
                    XLS.HucreDegerYaz(satir, 8, Hizmet.YilAyGunDondur(pa.kurumHizmetGunu));

                    satir++;
                }

                XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(900));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //protected string IzinListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    string personelTuru = (string)raporParam["personelTur"];
        //    string personelKod = (string)raporParam["personelKod"];
        //    string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
        //    int izinTuru = (int)raporParam["izinTuru"];
        //    int aktiflik = (int)raporParam["aktiflik"];
        //    int siralamaListesi = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["siralamaListesi"]), 0);

        //    TNSDateTime kucukTarih = new TNSDateTime((string)raporParam["kucukTarih"]);
        //    TNSDateTime buyukTarih = new TNSDateTime((string)raporParam["buyukTarih"]);
        //    Document doc = new Document(sablonAd);
        //    DocumentBuilder docb = new DocumentBuilder(doc);

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    if (izinTuru == (int)ENUMIZINTIP.YILLIK)
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "IzinTuru", "YILLIK İZİN LİSTESİ");
        //    else if (izinTuru == (int)ENUMIZINTIP.MAZERET)
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "IzinTuru", "MAZERET İZİN LİSTESİ");
        //    else if (izinTuru == (int)ENUMIZINTIP.SIHHI)
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "IzinTuru", "SAĞLIK İZİN LİSTESİ");
        //    else
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "IzinTuru", "İKİ TARİH ARASINDAKİ İZİNLER");

        //    if (izinTuru == -1)
        //        izinTuru = 1;
        //    ObjectArray list = servisIKY.IzinListesi(kullanan, personelKod, personelTuru, organizasyonKopruKodu, izinTuru, kucukTarih, buyukTarih, siralamaListesi, aktiflik);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);
        //    DataTable tab = new DataTable();
        //    tab.TableName = "Izinler";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("Ad");
        //    tab.Columns.Add("Unvan");
        //    tab.Columns.Add("KadroDerece");
        //    tab.Columns.Add("BaslangicTarihi");
        //    tab.Columns.Add("BitisTarihi");
        //    tab.Columns.Add("izinTuru");
        //    tab.Columns.Add("Sebep");
        //    tab.Columns.Add("GunSayisi");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (PersonelIzinRapor i in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = i.siraNo;
        //            row["Ad"] = i.ad;
        //            row["Unvan"] = i.unvan;
        //            row["KadroDerece"] = i.kadroDerece;
        //            row["BaslangicTarihi"] = i.baslangicTarihi.ToString();
        //            row["BitisTarihi"] = i.bitisTarihi.ToString();
        //            row["izinTuru"] = i.izinTuru;
        //            row["Sebep"] = i.aciklama;
        //            row["GunSayisi"] = i.gunSayisi;
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["Ad"] = "";
        //        row["Unvan"] = "";
        //        row["KadroDerece"] = "";
        //        row["BaslangicTarihi"] = "";
        //        row["BitisTarihi"] = "";
        //        row["izinTuru"] = "";
        //        row["Sebep"] = "";
        //        row["GunSayisi"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        //protected string KartListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    string personelTuru = (string)raporParam["personelTur"];
        //    string personelKod = (string)raporParam["personelKod"];
        //    string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
        //    Document doc = new Document(sablonAd);
        //    DocumentBuilder docb = new DocumentBuilder(doc);

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    if (raporKod == "IK028")
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "KartTuru", "PERSONEL");
        //    else if (raporKod == "IK029")
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "KartTuru", "TESLİM EDİLMEYEN");
        //    else if (raporKod == "IK030")
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "KartTuru", "KAYIP");

        //    ObjectArray list = servisIKY.KartListesi(kullanan, personelKod, personelTuru, organizasyonKopruKodu, raporKod);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Kartlar";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("KartTipi");
        //    tab.Columns.Add("Ad");
        //    tab.Columns.Add("Unvan");
        //    tab.Columns.Add("Yakinligi");
        //    tab.Columns.Add("KartSahibi");
        //    tab.Columns.Add("KartNo");
        //    tab.Columns.Add("plaka");
        //    tab.Columns.Add("TagCNo");
        //    tab.Columns.Add("ZimmetTarihi");
        //    tab.Columns.Add("IadeTarihi");
        //    tab.Columns.Add("KayipTarihi");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (TNS.IKY.PersonelKart k in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = k.siraNo;
        //            row["KartTipi"] = k.kartTipiAd;
        //            row["Ad"] = k.personelAd;
        //            row["Unvan"] = k.unvan;
        //            row["Yakinligi"] = k.yakinligi;
        //            row["KartSahibi"] = k.adiSoyadi;
        //            row["KartNo"] = k.kartNo;
        //            row["plaka"] = k.plaka;
        //            row["TagCNo"] = k.tagCihazNo;
        //            row["ZimmetTarihi"] = k.zimmetTarihi.ToString();
        //            row["IadeTarihi"] = k.iadeTarihi.ToString();
        //            row["KayipTarihi"] = k.kayipTarihi.ToString();
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["KartTipi"] = "";
        //        row["Ad"] = "";
        //        row["Unvan"] = "";
        //        row["Yakinligi"] = "";
        //        row["KartSahibi"] = "";
        //        row["KartNo"] = "";
        //        row["plaka"] = "";
        //        row["TagCNo"] = "";
        //        row["ZimmetTarihi"] = "";
        //        row["IadeTarihi"] = "";
        //        row["KayipTarihi"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        protected string KartListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            string personelKod = (string)raporParam["personelKod"];
            string organizasyonKopruKodu = (string)raporParam["ORGKOD"];

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            ObjectArray list = servisIKY.KartListesi(kullanan, personelKod, personelTuru, organizasyonKopruKodu, ekBilgi);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            if (list.ObjeSayisi > 0)
            {
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                string baslik = "";

                if (ekBilgi == "IK028")
                    baslik = "PERSONEL KART LİSTESİ";
                else if (ekBilgi == "IK029")
                    baslik = "TESLİM EDİLMEYEN KART LİSTESİ";
                else if (ekBilgi == "IK030")
                    baslik = "KAYIP KART LİSTESİ";

                XLS.HucreAdBulYaz("baslik", baslik);

                satir = kaynakSatir;

                foreach (TNS.IKY.PersonelKart pa in list.objeler)
                {
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, pa.siraNo);
                    XLS.HucreDegerYaz(satir, 1, pa.kartTipiAd);
                    XLS.HucreDegerYaz(satir, 2, pa.personelAd);
                    XLS.HucreDegerYaz(satir, 3, pa.unvan);
                    XLS.HucreDegerYaz(satir, 4, pa.yakinlikAd);
                    XLS.HucreDegerYaz(satir, 5, pa.adiSoyadi);
                    XLS.HucreDegerYaz(satir, 6, pa.kartNo);
                    XLS.HucreDegerYaz(satir, 7, pa.plaka);
                    XLS.HucreDegerYaz(satir, 8, pa.tagCihazNo);
                    XLS.HucreDegerYaz(satir, 9, pa.zimmetTarihi.ToString());
                    XLS.HucreDegerYaz(satir, 10, pa.iadeTarihi.ToString());
                    XLS.HucreDegerYaz(satir, 11, pa.kayipTarihi.ToString());

                    satir++;
                }

                XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(900));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //protected string LojmanListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    string personelTuru = (string)raporParam["personelTur"];
        //    string personelKod = (string)raporParam["personelKod"];
        //    string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
        //    int aktiflik = (int)raporParam["aktiflik"];
        //    int siteKod = (int)raporParam["site"];
        //    int siralamaListesi = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["siralamaListesi"]), 0);

        //    Document doc = new Document(sablonAd);
        //    DocumentBuilder docb = new DocumentBuilder(doc);
        //    int tahsisTip = 0;

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    if (raporKod == "IK053")
        //        tahsisTip = (int)ENUMTahsisTipi.Rezervasyon;
        //    else
        //        tahsisTip = (int)ENUMTahsisTipi.Tahsis;

        //    ObjectArray list = servisIKY.LojmanListesiRaporu(kullanan, personelKod, personelTuru, organizasyonKopruKodu, aktiflik, siteKod, tahsisTip, siralamaListesi);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Lojman";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("Ad");
        //    tab.Columns.Add("telefon");
        //    tab.Columns.Add("mail");
        //    tab.Columns.Add("BaslangicTarihi");
        //    tab.Columns.Add("site");
        //    tab.Columns.Add("blok");
        //    tab.Columns.Add("daire");
        //    tab.Columns.Add("daireTur");
        //    tab.Columns.Add("oturmaSuresi");
        //    //tab.Columns.Add("oturduguLojmanSayisi");

        //    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "RaporTuru", "Lojmanda Oturanların Listesi");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (PersonelLojmani k in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = k.siraNo;
        //            row["Ad"] = k.personelAd;
        //            row["telefon"] = k.personelTel;
        //            row["mail"] = k.personelMail;
        //            row["BaslangicTarihi"] = k.basTarihi;
        //            row["site"] = k.siteAdi;
        //            row["blok"] = k.blokNo;
        //            row["daire"] = k.daireNo;
        //            row["daireTur"] = k.daireTipiAd;
        //            row["oturmaSuresi"] = Hizmet.YilAyGunDondur(k.oturmaGunu);
        //            //row["oturduguLojmanSayisi"] = k.oturduguLojmanSayisi;
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["Ad"] = "";
        //        row["telefon"] = "";
        //        row["mail"] = "";
        //        row["BaslangicTarihi"] = "";
        //        row["site"] = "";
        //        row["blok"] = "";
        //        row["daire"] = "";
        //        row["daireTur"] = "";
        //        row["oturmaSuresi"] = "";
        //        //row["oturduguLojmanSayisi"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        protected string LojmanListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            string personelKod = (string)raporParam["personelKod"];
            string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
            int aktiflik = (int)raporParam["aktiflik"];
            int siteKod = (int)raporParam["site"];
            int yil1 = (int)raporParam["yil1"];
            int yil2 = (int)raporParam["yil2"];
            if (ekBilgi == "IK053")
            {
                yil1 = -1;
                yil2 = -1;
            }

            int tahsisTip = 0;
            int siralamaListesi = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["siralamaListesi"]), 0);

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            if (ekBilgi == "IK053")
                tahsisTip = (int)ENUMTahsisTipi.Rezervasyon;
            else
                tahsisTip = (int)ENUMTahsisTipi.Tahsis;

            PersonelLojmani pKriter = new PersonelLojmani();

            ObjectArray list = servisIKY.LojmanListesiRaporu(kullanan, personelKod, personelTuru, organizasyonKopruKodu, aktiflik, siteKod, tahsisTip, siralamaListesi, yil1, yil2);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            if (list.ObjeSayisi > 0)
            {
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

                //XLS.HucreAdBulYaz("baslik", "LOJMANDA OTURANLARIN LİSTESİ");

                satir = kaynakSatir;
                int cnt = 1;
                string oncelik = "";
                foreach (PersonelLojmani pa in list.objeler)
                {
                    sutun = 0; oncelik = "";
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun, cnt);
                    XLS.HucreDegerYaz(satir, sutun + 1, pa.personelAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, pa.unvanAd);
                    if (ekBilgi == "IK053")
                    {
                        if (pa.oncelikDerecesi == 1)
                            oncelik = "1- Çok Önemli";
                        else if (pa.oncelikDerecesi == 2)
                            oncelik = "2- Önemli";
                        else if (pa.oncelikDerecesi == 13)
                            oncelik = "3- Normal";
                        XLS.HucreDegerYaz(satir, sutun + 3, oncelik);
                    }
                    else
                        XLS.HucreDegerYaz(satir, sutun + 3, pa.lojmanTipiEtiketi);
                    XLS.HucreDegerYaz(satir, sutun + 4, pa.personelTel);
                    XLS.HucreDegerYaz(satir, sutun + 5, pa.personelMail);
                    if (ekBilgi == "IK053")
                        XLS.HucreDegerYaz(satir, sutun + 6, pa.rezervTarih.Oku());
                    else
                        XLS.HucreDegerYaz(satir, sutun + 6, pa.basTarihi.Oku());
                    XLS.HucreDegerYaz(satir, sutun + 7, pa.siteAdi);
                    XLS.HucreDegerYaz(satir, sutun + 8, pa.blokNo);
                    XLS.HucreDegerYaz(satir, sutun + 9, pa.daireNo);
                    XLS.HucreDegerYaz(satir, sutun + 10, pa.daireTipiAd);
                    XLS.HucreDegerYaz(satir, sutun + 11, Hizmet.YilAyGunDondur(pa.oturmaGunu));
                    satir++;
                    cnt++;
                }
                XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(900));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        protected string IzinListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            string personelKod = (string)raporParam["personelKod"];
            string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
            int aktiflik = (int)raporParam["aktiflik"];
            int siralamaKod = (int)raporParam["siralamaKod"];

            TNS.IKY.Personel per = new TNS.IKY.Personel();
            per.izinDonemBas = new TNSDateTime(IzinArac.DonemBasiBul((int)raporParam["yil1"]));
            per.izinDonemSon = new TNSDateTime(IzinArac.DonemSonuBul((int)raporParam["yil2"]));
            per.izinTurler = (string)raporParam["izinTurler"];
            per.kodlar = personelKod;
            per.personelTipleri = personelTuru;
            per.kopruKod = organizasyonKopruKodu;
            per.aktifDurum = aktiflik;
            per.genelSorgu = (string)raporParam["sorgu"];

            if (siralamaKod == 0)
                per.listeSiralama = ENUMPersonelSira.Unvan;
            else if (siralamaKod == 1)
                per.listeSiralama = ENUMPersonelSira.Birim;
            else
                per.listeSiralama = ENUMPersonelSira.AdSoyad;

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            ObjectArray list = servisIKY.IzinListesi(kullanan, per);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            if (list.ObjeSayisi > 0)
            {
                int kaynakSatir = 0;
                int formatSayfa = 0;
                int formatSatir = 0;
                int formatSutun = 0;
                int formatSatir2 = 0;
                int formatSutun2 = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("FormatSatir", ref formatSayfa, ref formatSatir, ref formatSutun, ref formatSatir2, ref formatSutun2);
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                int yuks = XLS.SatirYukseklikAl(kaynakSatir);
                string baslik = "YILLIK İZİN/GÖREVLENDİRMELER";

                XLS.HucreAdBulYaz("baslik", baslik);

                satir = kaynakSatir;
                int cnt = 1;

                foreach (TNS.IKY.Personel pa in list.objeler)
                {
                    int toplamGun = 0;
                    int kisiSatir = satir;
                    if (ekBilgi != "IK021b")
                    {
                        XLS.HucreKopyala(formatSatir, formatSutun, formatSatir, formatSutun2, satir, sutun);
                        XLS.HucreDegerYaz(satir, sutun + 0, cnt);
                        XLS.HucreDegerYaz(satir, sutun + 2, pa.adSoyad + " (" + pa.unvanAd + ")");
                        cnt++;
                        satir++;
                    }
                    int cntIc = 1;
                    foreach (TNS.IKY.PersonelIzin pi in pa.izin)
                    {
                        if (ekBilgi != "IK021b")
                        {
                            XLS.HucreKopyala(formatSatir + 1, formatSutun, formatSatir + 1, formatSutun2, satir, sutun);
                            XLS.HucreDegerYaz(satir, sutun + 0, cntIc);
                            XLS.HucreDegerYaz(satir, sutun + 2, pi.izinTurAd);
                            XLS.HucreDegerYaz(satir, sutun + 3, pi.baslangicTarihi.Oku());
                            XLS.HucreDegerYaz(satir, sutun + 4, pi.bitisTarihi.Oku());
                            XLS.HucreDegerYaz(satir, sutun + 5, pi.izinYeri);
                            XLS.HucreDegerYaz(satir, sutun + 6, pi.aciklama);
                            XLS.HucreDegerYaz(satir, sutun + 7, pi.kullanilanGun);
                            toplamGun += pi.kullanilanGun;
                            cntIc++;
                        }
                        else
                        {
                            XLS.HucreKopyala(formatSatir, formatSutun, formatSatir, formatSutun2, satir, sutun);
                            XLS.HucreDegerYaz(satir, sutun + 0, cnt);
                            XLS.HucreDegerYaz(satir, sutun + 1, pa.sicilNo);
                            XLS.HucreDegerYaz(satir, sutun + 2, pa.adSoyad);
                            XLS.HucreDegerYaz(satir, sutun + 3, pa.unvanAd);
                            XLS.HucreDegerYaz(satir, sutun + 4, pa.kadro.muhasebeBirimiAd);
                            XLS.HucreDegerYaz(satir, sutun + 5, pi.izinTurAd);
                            XLS.HucreDegerYaz(satir, sutun + 6, pi.ulkeAd);
                            XLS.HucreDegerYaz(satir, sutun + 7, pi.yerAd);
                            XLS.HucreDegerYaz(satir, sutun + 8, pi.baslangicTarihiDT);
                            XLS.HucreDegerYaz(satir, sutun + 9, pi.bitisTarihiDT);
                            XLS.HucreDegerYaz(satir, sutun + 10, pi.kullanilanGun);
                            XLS.HucreDegerYaz(satir, sutun + 11, pi.aciklama);
                            cnt++;
                        }
                        XLS.SatirYukseklikAyarla(satir, satir, -1, 7, yuks);
                        satir++;
                    }
                    if (ekBilgi != "IK021b")
                    {
                        XLS.HucreDegerYaz(kisiSatir, 7, toplamGun);
                        XLS.SatirYukseklikAyarla(kisiSatir, satir, yuks);
                    }
                }
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //protected string LojmanDegisiklikListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    int aktiflik = (int)raporParam["aktiflik"];
        //    int siteKod = (int)raporParam["site"];
        //    Document doc = new Document(sablonAd);
        //    DocumentBuilder docb = new DocumentBuilder(doc);

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    ObjectArray list = servisIKY.LojmanDegisiklikListesiRaporu(kullanan, aktiflik, siteKod);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Lojman";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("Ad");
        //    tab.Columns.Add("telefon");
        //    tab.Columns.Add("mail");
        //    tab.Columns.Add("BaslangicTarihi");
        //    tab.Columns.Add("site");
        //    tab.Columns.Add("blok");
        //    tab.Columns.Add("daire");
        //    tab.Columns.Add("daireTur");
        //    tab.Columns.Add("AyrilisTarihi");

        //    //GenelIslemlerIstemci.AsposeAlanaYaz(docb, "RaporTuru", "Lojmanda Oturanların Listesi");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (PersonelLojmani k in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = k.siraNo;
        //            row["Ad"] = k.personelAd;
        //            row["telefon"] = k.personelTel;
        //            row["mail"] = k.personelMail;
        //            row["BaslangicTarihi"] = k.basTarihi;
        //            row["site"] = k.siteAdi;
        //            row["blok"] = k.blokNo;
        //            row["daire"] = k.daireNo;
        //            row["daireTur"] = k.daireTipiAd;
        //            row["AyrilisTarihi"] = k.bitTarihi;
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["Ad"] = "";
        //        row["telefon"] = "";
        //        row["mail"] = "";
        //        row["BaslangicTarihi"] = "";
        //        row["site"] = "";
        //        row["blok"] = "";
        //        row["daire"] = "";
        //        row["daireTur"] = "";
        //        row["AyrilisTarihi"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        //protected string LojmanDaireListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    //string personelTuru = (string)raporParam["personelTur"];
        //    //string personelKod = (string)raporParam["personelKod"];
        //    //string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
        //    //int aktiflik = (int)raporParam["aktiflik"];
        //    int siteKod = (int)raporParam["site"];
        //    int daireBosluk = (int)raporParam["daireBosluk"];
        //    Document doc = new Document(sablonAd);
        //    DocumentBuilder docb = new DocumentBuilder(doc);

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    ObjectArray list = servisIKY.LojmanDaireBazliRaporu(kullanan, siteKod, daireBosluk);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Lojman";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("Ad");
        //    tab.Columns.Add("site");
        //    tab.Columns.Add("blok");
        //    tab.Columns.Add("daire");
        //    tab.Columns.Add("daireTur");

        //    if (daireBosluk == 1)
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "RaporTuru", "Boş Daire Listesi");
        //    else if (daireBosluk == 2)
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "RaporTuru", "Dolu Daire Listesi");
        //    else
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "RaporTuru", "Tüm Dairelerin Listesi");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (LojmanSiteBlokDaire k in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = k.siraNo;
        //            row["Ad"] = k.personelAd;
        //            row["site"] = k.siteAdi;
        //            row["blok"] = k.blokNo;
        //            row["daire"] = k.daireNo;
        //            row["daireTur"] = k.daireTuruAd;
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["Ad"] = "";
        //        row["site"] = "";
        //        row["blok"] = "";
        //        row["daire"] = "";
        //        row["daireTur"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        protected string LojmanDaireListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            int siteKod = (int)raporParam["site"];
            int daireBosluk = (int)raporParam["daireBosluk"];

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            ObjectArray list = servisIKY.LojmanDaireBazliRaporu(kullanan, siteKod, daireBosluk);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            if (list.ObjeSayisi > 0)
            {
                string baslik = "";
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

                if (daireBosluk == 1)
                    baslik = "BOŞ DAİRE LİSTESİ";
                else if (daireBosluk == 2)
                    baslik = "DOLU DAİRE LİSTESİ";
                else
                    baslik = "DAİRE LİSTESİ";

                XLS.HucreAdBulYaz("baslik", baslik);

                satir = kaynakSatir;
                int cnt = 1;

                foreach (PersonelLojmani pa in list.objeler)
                {
                    if (daireBosluk == 1 && !pa.Bos) continue;
                    if (daireBosluk == 2 && pa.Bos) continue;

                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, cnt);
                    XLS.HucreDegerYaz(satir, 1, pa.siteAdi);
                    XLS.HucreDegerYaz(satir, 2, pa.blokNo);
                    XLS.HucreDegerYaz(satir, 3, pa.daireNo);
                    XLS.HucreDegerYaz(satir, 4, pa.daireTipiAd);
                    if (!pa.Bos)
                    {
                        XLS.HucreDegerYaz(satir, 5, pa.personelAd);
                        XLS.HucreDegerYaz(satir, 6, pa.unvanAd);
                        XLS.HucreDegerYaz(satir, 7, pa.lojmanTipiEtiketi);
                        XLS.HucreDegerYaz(satir, 8, Hizmet.YilAyGunDondur(pa.oturmaGunu));
                    }
                    satir++;
                    cnt++;
                }

                XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //protected string HizmetListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    string personelTuru = (string)raporParam["personelTur"];
        //    int aktiflik = (int)raporParam["aktiflik"];
        //    string personelKod = (string)raporParam["personelKod"];
        //    TNSDateTime sonTarih = new TNSDateTime((string)raporParam["sonTarih"]);
        //    string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
        //    int siralamaListesi = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["siralamaListesi"]), 0);

        //    Document doc = new Document(sablonAd);
        //    DocumentBuilder docb = new DocumentBuilder(doc);

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    if (raporKod == "IK031")
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "hizmetTuru", "KURUM HİZMETİ LİSTESİ");
        //    else if (raporKod == "IK046")
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "hizmetTuru", "KURUM KADROLU HİZMET LİSTESİ");
        //    else if (raporKod == "IK032")
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "hizmetTuru", "KIDEM HİZMETİ LİSTESİ");
        //    else if (raporKod == "IK033")
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "hizmetTuru", "EMEKLİ KESENEĞİ HİZMET BELGESİ LİSTESİ");
        //    else if (raporKod == "IK034")
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "hizmetTuru", "İZİN HİZMET BELGESİ LİSTESİ");
        //    else if (raporKod == "IK035")
        //        GenelIslemlerIstemci.AsposeAlanaYaz(docb, "hizmetTuru", "TOPLAM HİZMET BELGESİ LİSTESİ");

        //    if (sonTarih.Yil == 1)
        //        sonTarih = new TNSDateTime(DateTime.Now);

        //    var list = servisIKY.HizmetleriGetir(kullanan, organizasyonKopruKodu, personelKod, personelTuru, sonTarih, siralamaListesi, raporKod, aktiflik);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Hizmetler";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("Ad");
        //    tab.Columns.Add("Unvan");
        //    tab.Columns.Add("gun");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        int cnt = 1;
        //        foreach (Hizmet h in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = cnt;
        //            row["Ad"] = h.personelAd;
        //            row["Unvan"] = h.guncelHizmet.unvan;

        //            if (raporKod == "IK031")
        //                row["gun"] = Hizmet.YilAyGunDondur(h.kurumdakiSure);
        //            else if (raporKod == "IK046")
        //                row["gun"] = Hizmet.YilAyGunDondur(h.kadrodakiSure);
        //            else if (raporKod == "IK032")
        //                row["gun"] = h.kidemYilAyGun;
        //            else if (raporKod == "IK033")
        //                row["gun"] = h.EKEAYilAyGun;
        //            else if (raporKod == "IK034")
        //                row["gun"] = Hizmet.YilAyGunDondur(h.izinHizmetGunu);
        //            else if (raporKod == "IK035")
        //                row["gun"] = h.hizmetYilAyGun;

        //            tab.Rows.Add(row);
        //            cnt++;
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["Ad"] = "";
        //        row["Unvan"] = "";
        //        row["gun"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    doc.MailMerge.ExecuteWithRegions(tab);
        //    doc.Save(sonucDosyaad);
        //    return sonucDosyaad;
        //}

        protected string HizmetListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            int aktiflik = (int)raporParam["aktiflik"];
            string personelKod = (string)raporParam["personelKod"];
            TNSDateTime sonTarih = new TNSDateTime((string)raporParam["sonTarih"]);
            string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
            int siralamaListesi = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["siralamaListesi"]), 0);

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            if (sonTarih.Yil == 1)
                sonTarih = new TNSDateTime(DateTime.Now);

            var list = servisIKY.HizmetleriGetir(kullanan, organizasyonKopruKodu, personelKod, personelTuru, sonTarih, siralamaListesi, ekBilgi, aktiflik);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            if (list.ObjeSayisi > 0)
            {
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                string baslik = "";

                if (ekBilgi == "IK031")
                    baslik = "KURUM HİZMETİ LİSTESİ";
                else if (ekBilgi == "IK046")
                    baslik = "KURUM KADROLU HİZMET LİSTESİ";
                else if (ekBilgi == "IK032")
                    baslik = "KIDEM HİZMETİ LİSTESİ";
                else if (ekBilgi == "IK033")
                    baslik = "EMEKLİ KESENEĞİ HİZMET BELGESİ LİSTESİ";
                else if (ekBilgi == "IK034")
                    baslik = "İZİN HİZMET BELGESİ LİSTESİ";
                else if (ekBilgi == "IK035")
                    baslik = "TOPLAM HİZMET BELGESİ LİSTESİ";

                XLS.HucreAdBulYaz("baslik", baslik);

                satir = kaynakSatir;
                int cnt = 1;
                string gun = "";

                foreach (Hizmet h in list.objeler)
                {
                    if (ekBilgi == "IK031")
                        gun = Hizmet.YilAyGunDondur(h.kurumdakiSure);
                    else if (ekBilgi == "IK046")
                        gun = Hizmet.YilAyGunDondur(h.kadrodakiSure);
                    else if (ekBilgi == "IK032")
                        gun = h.kidemYilAyGun;
                    else if (ekBilgi == "IK033")
                        gun = h.EKEAYilAyGun;
                    else if (ekBilgi == "IK034")
                        gun = Hizmet.YilAyGunDondur(h.izinHizmetGunu);
                    else if (ekBilgi == "IK035")
                        gun = h.hizmetYilAyGun;

                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 4, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, cnt);
                    XLS.HucreDegerYaz(satir, 1, h.personelAd);
                    XLS.HucreDegerYaz(satir, 2, h.guncelHizmet.unvan);
                    XLS.HucreDegerYaz(satir, 3, gun);

                    satir++;
                    cnt++;
                }

                XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //protected string IsimliUnvanListesiYazdir(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    string personelTuru = (string)raporParam["personelTur"];
        //    string personelKod = (string)raporParam["personelKod"];
        //    string organizasyonKodu = (string)raporParam["ORGKOD"];

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    ObjectArray list = servisIKY.KadroRaporlari(kullanan, personelTuru, raporKod, personelKod, organizasyonKodu, 0);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Unvanlar";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("HizmetTipi");
        //    tab.Columns.Add("Unvan");
        //    tab.Columns.Add("AdSoyad");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (PersonelKadroRapor k in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = k.siraNo;
        //            row["HizmetTipi"] = k.sinif;
        //            row["Unvan"] = k.unvan;
        //            row["AdSoyad"] = k.isim;
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["HizmetTipi"] = "";
        //        row["Unvan"] = "";
        //        row["AdSoyad"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    Document doc = new Document(sablonAd);
        //    DocumentBuilder docb = new DocumentBuilder(doc);

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        protected string IsimliUnvanListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            string personelKod = (string)raporParam["personelKod"];
            string organizasyonKodu = (string)raporParam["ORGKOD"];

            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            ObjectArray list = servisIKY.KadroRaporlari(kullanan, personelTuru, ekBilgi, personelKod, organizasyonKodu, 0);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            if (list.ObjeSayisi > 0)
            {
                int kaynakSatir = 0;
                int sutun = 0;
                int satir = 0;
                ObjectArray arr = new ObjectArray();
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                string baslik = "UNVAN İSİM LİSTESİ";

                XLS.HucreAdBulYaz("baslik", baslik);

                satir = kaynakSatir;

                foreach (PersonelKadroRapor k in list.objeler)
                {
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 4, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, k.siraNo);
                    XLS.HucreDegerYaz(satir, 1, k.sinif);
                    XLS.HucreDegerYaz(satir, 2, k.unvan);
                    XLS.HucreDegerYaz(satir, 3, k.isim);

                    satir++;
                }

                XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(600));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public static string[] dinamikKolonAdlar = {
            "Resim", "Kod", "Ad", "Soyad", "TC Kimlik No", "Sicil No", "Unvan", "Birim", "Personel Türü", "Başlama Tarihi",
            "Ayrılış Tarihi", "Cep Tel", "Dahili Tel", "Baba Ad", "Kadro Derece", "Ek Gösterge", "Doğum Yeri", "Doğum Tarihi", "Nüfus İlçesi", "Öğrenim Durumu",
            "Geldiği Kurum", "Okul Adı", "Görevli O. Birim", "Cinsiyet", "E. Posta", "Adres", "Birim Tür", "Görev Birim Tür", "Harc. Birimi Kod", "Saymanlık Birim Ad",
            "Harc. Birimi Ad", "Ayrılış Gerekçesi" };

        private string DinamikRaporListesiYazdir(object kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri; // GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);
            int satir = 0, sutun = 0;
            int kaynakSatir = 0, baslaSatir = 0;
            ObjectArray arr = new ObjectArray();
            XLS.HucreAdAdresCoz("FormatSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            int yuks = XLS.SatirYukseklikAl(baslaSatir);

            satir = baslaSatir - 1;
            string[] liste = null;

            string siralamalar = (string)raporParam["siralamaListesi"];
            if (!string.IsNullOrWhiteSpace(siralamalar))
                liste = siralamalar.Split(',', ';');

            TNS.IKY.Personel p = PersonelOl(raporParam);

            for (int i = 0; i < liste.Length; i++)
            {
                if (i > 0)
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun, kaynakSatir, i);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun, satir, i);
                XLS.KoyuYap(satir, i, true);
                XLS.DuseyHizala(satir, i, 2);
                int siraKod = OrtakFonksiyonlar.ConvertToInt(liste[i], 0);
                if (siraKod == 0)
                {
                    XLS.SutunGenislikAyarla(i, i, 2000);
                    yuks = yuks * 5;
                    p.resimOlsun = true;//Resim seçilmişse olsun.
                }

                XLS.HucreDegerYaz(satir, i, OrtakFonksiyonlar.DiziElemanAl(dinamikKolonAdlar, siraKod, ""));
            }
            XLS.HucreBirlestir(satir - 1, sutun, satir - 1, sutun + liste.Length - 1);

            ObjectArray personelListesi = servisIKY.PersonelListele((TNS.KYM.Kullanici)kullanan, p, false, false);
            //int toplamSay = list.objeler.Count;
            for (int i = 0; i < personelListesi.ObjeSayisi; i++)
            {
                satir++;
                TNS.IKY.Personel d = (TNS.IKY.Personel)personelListesi.objeler[i];
                if (d == null)
                    d = new TNS.IKY.Personel();
                if (d.kadro == null)
                    d.kadro = new TNS.IKY.PersonelKadro();
                if (d.iletisim == null)
                    d.iletisim = new PersonelIletisim();
                if (d.nufus == null)
                    d.nufus = new TNS.IKY.PersonelNufus();

                TNS.IKY.PersonelOgrenim og = d.SonOkulBul();
                if (og == null) og = new TNS.IKY.PersonelOgrenim();
                if (liste.Length > 0)
                {
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, liste.Length, satir, sutun);
                    int cnt = 0;
                    foreach (string item in liste)
                    {
                        int siraKod = OrtakFonksiyonlar.ConvertToInt(item, 0);
                        switch (siraKod)
                        {
                            case 0:
                                if (d.resim != null)
                                    XLS.ResimEkle(satir, cnt, 2.5, 3.50, new MemoryStream(TNS.IKY.Arac.ResimAyarla(d.resim, 4, 0, 0)), 0, 0, 2, 3);
                                break;
                            case 1:
                                XLS.HucreDegerYaz(satir, cnt, d.kod);
                                break;
                            case 2:
                                XLS.HucreDegerYaz(satir, cnt, d.ad);
                                break;
                            case 3:
                                XLS.HucreDegerYaz(satir, cnt, d.soyad);
                                break;
                            case 4:
                                XLS.HucreDegerYaz(satir, cnt, d.TCKIMLIKNO);
                                break;
                            case 5:
                                XLS.HucreDegerYaz(satir, cnt, d.sicilNo);
                                break;
                            case 6:
                                XLS.HucreDegerYaz(satir, cnt, d.unvanAd);
                                break;
                            case 7:
                                XLS.HucreDegerYaz(satir, cnt, d.kadro.birimAd);
                                break;
                            case 22:
                                XLS.HucreDegerYaz(satir, cnt, d.kadro.gorevBirimAd);
                                break;
                            case 8:
                                XLS.HucreDegerYaz(satir, cnt, d.kadro.personelTipAd);
                                break;
                            case 9:
                                XLS.HucreDegerYaz(satir, cnt, d.kurumaBaslamaTarihi.ToString());
                                break;
                            case 10:
                                XLS.HucreDegerYaz(satir, cnt, d.kadro.ayrilmaTarihi.ToString());
                                break;
                            case 11:
                                XLS.HucreDegerYaz(satir, cnt, d.iletisim.cepTelefon);
                                break;
                            case 12:
                                XLS.HucreDegerYaz(satir, cnt, d.iletisim.isTelefon);
                                break;
                            case 13:
                                XLS.HucreDegerYaz(satir, cnt, d.nufus.babaAd);
                                break;
                            case 14:
                                XLS.HucreDegerYaz(satir, cnt, d.KADRODERECE);
                                break;
                            case 15:
                                XLS.HucreDegerYaz(satir, cnt, d.EKGOSTERGE);
                                break;
                            case 16:
                                XLS.HucreDegerYaz(satir, cnt, d.nufus.dogumYeri);
                                break;
                            case 17:
                                XLS.HucreDegerYaz(satir, cnt, d.dogumTarihi.ToString());
                                break;
                            case 18:
                                XLS.HucreDegerYaz(satir, cnt, d.nufus.ilceAd);
                                break;
                            case 19:
                                XLS.HucreDegerYaz(satir, cnt, og.ogrenimAdi);
                                break;
                            case 20:
                                XLS.HucreDegerYaz(satir, cnt, d.geldigiKurum);
                                break;
                            case 21:
                                XLS.HucreDegerYaz(satir, cnt, og.okulAdi);
                                break;
                            case 23:
                                XLS.HucreDegerYaz(satir, cnt, d.nufus.cinsiyetAd);
                                break;
                            case 24:
                                XLS.HucreDegerYaz(satir, cnt, d.iletisim.eposta);
                                break;
                            case 25:
                                XLS.HucreDegerYaz(satir, cnt, d.iletisim.adres);
                                break;
                            case 26:
                                XLS.HucreDegerYaz(satir, cnt, d.birimTuru);
                                break;
                            case 27:
                                XLS.HucreDegerYaz(satir, cnt, d.calistigiBirimTuru);
                                break;
                            case 28:
                                XLS.HucreDegerYaz(satir, cnt, d.kadro.muhasebeKodu.ToString("00000") + "-" + d.kadro.muhasebeBirimi);
                                break;
                            case 29:
                                XLS.HucreDegerYaz(satir, cnt, TNS.MUH.Arac.BirimAdOl(d.kadro.muhasebeAd, d.kadro.muhasebeBirimiAd));
                                break;
                            case 30:
                                XLS.HucreDegerYaz(satir, cnt, d.kadro.muhasebeBirimiAd);
                                break;
                            case 31:
                                XLS.HucreDegerYaz(satir, cnt, d.kadro.ayrilmaGerekcesiAd);
                                break;
                        }
                        cnt++;
                    }
                }
            }
            XLS.SatirYukseklikAyarla(baslaSatir - 1, satir, -1, 3, yuks);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public static TNS.IKY.Personel PersonelOl(Hashtable raporParam)
        {
            TNS.IKY.Personel p = new TNS.IKY.Personel();
            p.ogrenimListesiOlsun = true;
            p.kodlar = (string)raporParam["personelKod"];
            p.personelTipleri = (string)raporParam["personelTur"];
            p.kopruKod = (string)raporParam["ORGKOD"];
            p.aktifDurum = (int)raporParam["aktiflik"];
            p.genelSorgu = (string)raporParam["sorgu"];

            p.aktifTarih = new TNSDateTime((string)raporParam["tarihSorgu"]);
            p.aktifTarihTur = (int)raporParam["tarihSorguTur"];
            return p;
        }

        protected string PersonelKesintiListesiYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi, int grup)
        {
            IIKYServis servisIKY;
            string personelTuru = (string)raporParam["personelTur"];
            string personelKod = (string)raporParam["personelKod"];
            string organizasyonKopruKodu = (string)raporParam["ORGKOD"];
            int yil = (int)raporParam["yil"];
            int ay = (int)raporParam["ay"];
            int siteKod = (int)raporParam["site"];
            int aktiflik = (int)raporParam["aktiflik"];

            Kullanici kullanan = (Kullanici)o_kullanan;
            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            int ilkKol = 6;
            int kaynakSatir = 0;
            int sutun = 0;
            int fSatir = 0;
            int fsutun = 0;
            int basSatir = 0;
            int basSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref fSatir, ref fsutun);
            XLS.HucreAdAdresCoz("baslik", ref basSatir, ref basSutun);
            int bSatir = kaynakSatir - 1;
            int yuks = XLS.SatirYukseklikAl(kaynakSatir);
            TNSCollection col = servisIKY.SozlukGiderListele(kullanan, new SozlukGider()).objeler;

            int kesintiSay = col.Count;
            ToplamAl toplamAl = new ToplamAl(4, kesintiSay);

            for (int i = 0; i <= kesintiSay; i++)
            {
                string metin;
                if (i > 0)
                {
                    XLS.HucreKopyala(fSatir, fsutun + ilkKol, fSatir + 1, fsutun + ilkKol, fSatir, fsutun + ilkKol + i);
                    XLS.HucreKopyala(bSatir, sutun + ilkKol, bSatir, sutun + ilkKol, bSatir, sutun + ilkKol + i);
                }
                if (i == kesintiSay)
                    metin = "Toplam";
                else
                    metin = ((SozlukGider)col[i]).ad;
                XLS.HucreDegerYaz(bSatir, sutun + ilkKol + i, metin);
            }

            ObjectArray list = servisIKY.PersonelSiteKesintiRaporu(kullanan, personelKod, personelTuru, organizasyonKopruKodu, aktiflik, yil, ay, siteKod);
            if (list.ObjeSayisi > 0)
            {
                ObjectArray arr = new ObjectArray();
                XLS.HucreDegerYaz(basSatir, basSutun, "PERSONEL KESİNTİ LİSTESİ");
                XLS.HucreBirlestir(basSatir, basSutun, basSatir, basSutun + ilkKol + kesintiSay);
                int satir = kaynakSatir;
                int perSay = 1;
                //int cntIc = 0;
                string oncSiteKod = "";
                string yilAy = "";
                string oncTC = "";

                int oncKisiSatir = -1;
                PersonelSiteKesinti sonPa = null;
                foreach (PersonelSiteKesinti pa in list.objeler)
                {
                    if (oncSiteKod != OncekiSiteGrupBul(pa, grup) || yilAy != pa.yil.ToString() + pa.ay.ToString())
                    {
                        if (sonPa != null)
                        {
                            satir++;
                            XLS.HucreKopyala(fSatir + 1, fsutun, fSatir + 1, fsutun + ilkKol + kesintiSay + 1, satir, sutun);
                            XLS.HucreDegerYaz(satir, sutun, MetinYap(sonPa, grup));
                            KesintiListesiToplamYaz(XLS, satir, sutun + ilkKol, toplamAl, 1);
                            toplamAl.SatirSifirla(1);
                            XLS.SayfaSonuKoy(satir + 1);
                            perSay = 1;
                        }
                        oncSiteKod = OncekiSiteGrupBul(pa, grup);
                        yilAy = pa.yil.ToString() + pa.ay.ToString();
                    }

                    if (oncTC != pa.TC)
                    {
                        if (oncKisiSatir != -1)
                            KesintiListesiToplamYaz(XLS, oncKisiSatir, sutun + ilkKol, toplamAl, 0);
                        if (oncTC != "")
                            satir++;
                        XLS.HucreKopyala(fSatir, fsutun, fSatir, fsutun + ilkKol + kesintiSay + 1, satir, sutun);
                        XLS.HucreDegerYaz(satir, 0, perSay++);
                        XLS.HucreDegerYaz(satir, 1, pa.TC);
                        XLS.HucreDegerYaz(satir, 2, pa.personelAd);
                        XLS.HucreDegerYaz(satir, 3, pa.siteAdi + " / " + pa.blokNo + " / " + pa.daireNo);
                        XLS.HucreDegerYaz(satir, 4, pa.personelMail);
                        XLS.HucreDegerYaz(satir, 5, pa.unvanAd);
                        sonPa = pa;

                        oncTC = pa.TC;
                        toplamAl.SatirSifirla(0);
                        oncKisiSatir = satir;
                    }
                    toplamAl.DegerAta(0, pa.giderKod - 1, pa.tutar);
                    toplamAl.DegerArttir(1, pa.giderKod - 1, pa.tutar);
                    toplamAl.DegerArttir(2, pa.giderKod - 1, pa.tutar);
                    toplamAl.DegerArttir(3, pa.giderKod - 1, pa.tutar);
                }

                if (oncKisiSatir != -1)
                {
                    KesintiListesiToplamYaz(XLS, oncKisiSatir, sutun + ilkKol, toplamAl, 0);
                    satir++;
                    XLS.HucreKopyala(fSatir + 1, fsutun, fSatir + 1, fsutun + ilkKol + kesintiSay + 1, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun, MetinYap(sonPa, grup));
                    KesintiListesiToplamYaz(XLS, satir, sutun + ilkKol, toplamAl, 1);
                    toplamAl.SatirSifirla(1);
                }
                XLS.SatirYukseklikAyarla(kaynakSatir, satir, yuks);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string OncekiSiteGrupBul(PersonelSiteKesinti pa, int grup)
        {
            if (grup == 1) return pa.siteKod.ToString();
            else return pa.siteKod.ToString() + "_" + pa.blokNo.ToString();
        }

        private string MetinYap(PersonelSiteKesinti sonPa, int grup)
        {
            string ad;
            if (grup == 1)
                ad = sonPa.siteAdi;
            else
                ad = sonPa.siteAdi + " (" + sonPa.blokNo + ")";
            return string.Format("Toplam ({1} Yılı {2} Ayı {0})", ad, sonPa.yil, OrtakFonksiyonlar.AyAdiGetir(sonPa.ay));
        }

        private void KesintiListesiToplamYaz(Tablo XLS, int satir, int sutun, ToplamAl toplamAl, int toplamSatir)
        {
            for (int i = 0; i < toplamAl.SutunSay(); i++)
            {
                XLS.HucreDegerYaz(satir, sutun + i, toplamAl.toplam[toplamSatir, i]);
            }
            XLS.HucreDegerYaz(satir, sutun + toplamAl.SutunSay(), toplamAl.SatToplamAl(toplamSatir, 0, toplamAl.SutunSay() - 1));
        }

        //protected string LojmanDegisiklikListesiGrupExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    IIKYServis servisIKY;
        //    int aktiflik = (int)raporParam["aktiflik"];
        //    int siteKod = (int)raporParam["site"];

        //    Kullanici kullanan = (Kullanici)o_kullanan;
        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    Tablo XLS = GenelIslemler.NewTablo();
        //    sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);
        //    XLS.DosyaSaklamaFormatAta(ciktiYeri);

        //    ObjectArray list = servisIKY.LojmanDegisiklikListesiRaporu(kullanan, aktiflik, siteKod);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    if (list.ObjeSayisi > 0)
        //    {
        //        int kaynakSatir = 0;
        //        int sutun = 0;
        //        int satir = 0;
        //        ObjectArray arr = new ObjectArray();
        //        XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

        //        XLS.HucreAdBulYaz("baslik", "LOJMAN DEĞİŞİKLİK LİSTESİ");

        //        satir = kaynakSatir - 1;
        //        int cnt = 1;
        //        int cntIc = 0;
        //        int oncPer = -1;

        //        foreach (PersonelLojmani pa in list.objeler)
        //        {
        //            if (oncPer != pa.personelKod)
        //            {
        //                oncPer = pa.personelKod;
        //                satir++;
        //                XLS.HucreDegerYaz(satir, 0, cnt);
        //                XLS.HucreDegerYaz(satir, 1, pa.personelAd);
        //                XLS.HucreDegerYaz(satir, 2, pa.personelTel);
        //                XLS.HucreDegerYaz(satir, 3, pa.personelMail);
        //                XLS.ArkaPlanRenk(satir, 0, satir, 7, TabloRenk.LIGHT_TURQUOISE2);
        //                cnt++;
        //                cntIc = 1;
        //            }

        //            satir++;
        //            XLS.HucreDegerYaz(satir, 1, cntIc);
        //            XLS.HucreDegerYaz(satir, 2, pa.basTarihi.ToString());
        //            XLS.HucreDegerYaz(satir, 3, pa.bitTarihi.ToString());
        //            XLS.HucreDegerYaz(satir, 4, pa.siteAdi);
        //            XLS.HucreDegerYaz(satir, 5, pa.blokNo);
        //            XLS.HucreDegerYaz(satir, 6, pa.daireNo);
        //            XLS.HucreDegerYaz(satir, 7, pa.daireTipiAd);

        //            cntIc++;
        //        }

        //        XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
        //    }

        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        protected string LojmanDegisiklikListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi, bool grup)
        {
            IIKYServis servisIKY;
            Kullanici kullanan = (Kullanici)o_kullanan;
            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);

            TNS.IKY.Personel pKriter = new TNS.IKY.Personel();
            pKriter.personelSiteKod = (int)raporParam["site"];
            if (pKriter.personelSiteKod == 0) pKriter.personelSiteKod = -1;
            pKriter.lojmanListesiOlsun = true;
            ObjectArray list = servisIKY.PersonelListele(kullanan, pKriter, false, false);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            if (list.ObjeSayisi > 0)
            {
                int sil1 = 0, fSatir1 = 0, fSutun1 = 0, fSatir2 = 0, fSutun2 = 0;
                int baslaSatir = 0;
                int sutun = 0;
                int satir = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
                int yuks = XLS.SatirYukseklikAl(baslaSatir);
                XLS.HucreAdAdresCoz("FormatSatir", ref sil1, ref fSatir1, ref fSutun1, ref fSatir2, ref fSutun2);

                XLS.HucreAdBulYaz("baslik", "LOJMAN DEĞİŞİKLİK LİSTESİ");

                satir = baslaSatir;
                int cnt = 0;
                int cntLoj = 0;

                foreach (TNS.IKY.Personel pa in list.objeler)
                {
                    if (grup)
                    {
                        XLS.HucreKopyala(fSatir1, fSutun1, fSatir2, fSutun2, satir, sutun);
                        XLS.HucreDegerYaz(satir, 0, ++cnt);
                        XLS.HucreDegerYaz(satir, 1, pa.ad + " " + pa.soyad);
                        XLS.HucreDegerYaz(satir, 2, pa.isTelefon);
                        XLS.HucreDegerYaz(satir, 3, pa.iletisim.eposta);
                        satir++;
                        cntLoj = 0;
                    }
                    foreach (TNS.IKY.PersonelLojmani pl in pa.lojmanlar.objeler)
                    {
                        int icSut;
                        if (!grup)
                        {
                            XLS.HucreKopyala(fSatir1, fSutun1, fSatir2, fSutun2, satir, sutun);
                            XLS.HucreDegerYaz(satir, 0, ++cnt);
                            XLS.HucreDegerYaz(satir, 1, pa.ad + " " + pa.soyad);
                            XLS.HucreDegerYaz(satir, 2, pa.isTelefon);
                            XLS.HucreDegerYaz(satir, 3, pa.iletisim.eposta);
                            icSut = 4;
                        }
                        else
                        {
                            XLS.HucreKopyala(fSatir1 + 1, fSutun1, fSatir2 + 1, fSutun2, satir, sutun);
                            XLS.HucreDegerYaz(satir, 1, ++cntLoj);
                            icSut = 2;
                        }
                        XLS.HucreDegerYaz(satir, icSut + 0, pl.basTarihi.Oku());
                        if (!pl.bitTarihi.isNull)
                            XLS.HucreDegerYaz(satir, icSut + 1, pl.bitTarihi.Oku());
                        XLS.HucreDegerYaz(satir, icSut + 2, pl.siteAdi);
                        XLS.HucreDegerYaz(satir, icSut + 3, pl.blokNo);
                        XLS.HucreDegerYaz(satir, icSut + 4, pl.daireNo);
                        XLS.HucreDegerYaz(satir, icSut + 5, pl.lojmanTipiEtiketi);
                        satir++;
                    }
                }
                XLS.SatirYukseklikAyarla(baslaSatir, satir, yuks);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //protected string YillarArasiLojmandaOturanlarListesiRaporu(object o_servis, string raporKod, string sablonAd, System.Collections.Hashtable raporParam, string ciktiTur)
        //{
        //    IIKYServis servisIKY;
        //    int aktiflik = (int)raporParam["aktiflik"];
        //    int siteKod = (int)raporParam["site"];
        //    int yil1 = (int)raporParam["yil1"];
        //    int yil2 = (int)raporParam["yil2"];
        //    Document doc = new Document(sablonAd);
        //    DocumentBuilder docb = new DocumentBuilder(doc);

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    ObjectArray list = servisIKY.YillarArasiLojmandaOturanlarListesiRaporu(kullanan, aktiflik, siteKod, yil1, yil2);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Yillar", yil1 + " - " + yil2);

        //    DataTable tab = new DataTable();
        //    tab.TableName = "Lojman";
        //    tab.Columns.Add("SiraNo");
        //    tab.Columns.Add("Ad");
        //    tab.Columns.Add("telefon");
        //    tab.Columns.Add("mail");
        //    tab.Columns.Add("BaslangicTarihi");
        //    tab.Columns.Add("site");
        //    tab.Columns.Add("blok");
        //    tab.Columns.Add("daire");
        //    tab.Columns.Add("oturmaGunu");
        //    tab.Columns.Add("AyrilisTarihi");

        //    //GenelIslemlerIstemci.AsposeAlanaYaz(docb, "RaporTuru", "Lojmanda Oturanların Listesi");

        //    if (list.ObjeSayisi > 0)
        //    {
        //        foreach (PersonelLojmani k in list.objeler)
        //        {
        //            DataRow row = tab.NewRow();
        //            row["SiraNo"] = k.siraNo;
        //            row["Ad"] = k.personelAd;
        //            row["telefon"] = k.personelTel;
        //            row["mail"] = k.personelMail;
        //            row["BaslangicTarihi"] = k.basTarihi;
        //            row["site"] = k.siteAdi;
        //            row["blok"] = k.blokNo;
        //            row["daire"] = k.daireNo;
        //            row["oturmaGunu"] = Hizmet.YilAyGunDondur(k.oturmaGunu);
        //            row["AyrilisTarihi"] = k.bitTarihi;
        //            tab.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        DataRow row = tab.NewRow();
        //        row["SiraNo"] = "";
        //        row["Ad"] = "";
        //        row["telefon"] = "";
        //        row["mail"] = "";
        //        row["BaslangicTarihi"] = "";
        //        row["site"] = "";
        //        row["blok"] = "";
        //        row["daire"] = "";
        //        row["oturmaGunu"] = "";
        //        row["AyrilisTarihi"] = "";
        //        tab.Rows.Add(row);
        //    }

        //    string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + "." + ciktiTur;

        //    doc.MailMerge.ExecuteWithRegions(tab);

        //    doc.Save(sonucDosyaad);

        //    return sonucDosyaad;
        //}

        //protected string YillarArasiLojmandaOturanlarListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    IIKYServis servisIKY;
        //    int aktiflik = (int)raporParam["aktiflik"];
        //    int siteKod = (int)raporParam["site"];
        //    int yil1 = (int)raporParam["yil1"];
        //    int yil2 = (int)raporParam["yil2"];

        //    if (o_servis.GetType() == typeof(IIKYServis))
        //        servisIKY = (IIKYServis)o_servis;
        //    else
        //        servisIKY = TNS.IKY.Arac.Tanimla();

        //    ObjectArray list = servisIKY.YillarArasiLojmandaOturanlarListesiRaporu(kullanan, aktiflik, siteKod, yil1, yil2);
        //    if (!list.sonuc.islemSonuc)
        //        throw new Exception(list.sonuc.hataStr);

        //    Tablo XLS = GenelIslemler.NewTablo();
        //    sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);
        //    XLS.DosyaSaklamaFormatAta(ciktiYeri);

        //    if (list.ObjeSayisi > 0)
        //    {
        //        int kaynakSatir = 0;
        //        int sutun = 0;
        //        int satir = 0;
        //        ObjectArray arr = new ObjectArray();
        //        XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

        //        XLS.HucreAdBulYaz("baslik", yil1 + " - " + yil2 + " YILLAR ARASI LOJMANDA OTURANLAR LİSTESİ");

        //        satir = kaynakSatir;
        //        int cnt = 1;

        //        foreach (PersonelLojmani pa in list.objeler)
        //        {
        //            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);
        //            XLS.HucreDegerYaz(satir, 0, cnt);
        //            XLS.HucreDegerYaz(satir, 1, pa.personelAd);
        //            XLS.HucreDegerYaz(satir, 2, pa.personelTel);
        //            XLS.HucreDegerYaz(satir, 3, pa.personelMail);
        //            XLS.HucreDegerYaz(satir, 4, pa.basTarihi.ToString());
        //            XLS.HucreDegerYaz(satir, 5, pa.bitTarihi.ToString());
        //            XLS.HucreDegerYaz(satir, 6, pa.siteAdi);
        //            XLS.HucreDegerYaz(satir, 7, pa.blokNo);
        //            XLS.HucreDegerYaz(satir, 8, pa.daireNo);
        //            XLS.HucreDegerYaz(satir, 9, Hizmet.YilAyGunDondur(pa.oturmaGunu));

        //            satir++;
        //            cnt++;
        //        }

        //        XLS.SatirYukseklikAyarla(kaynakSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
        //    }

        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        protected string PersonelGiderListesiExceleYazdir(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IIKYServis servisIKY;
            if (o_servis.GetType() == typeof(IIKYServis))
                servisIKY = (IIKYServis)o_servis;
            else
                servisIKY = TNS.IKY.Arac.Tanimla();

            //int aktiflik = OrtakFonksiyonlar.ConvertToInt(raporParam["aktiflik"], 0);
            IKYPersonelGider krGider = new IKYPersonelGider();
            krGider.pKod = 0;
            krGider.kriterOrgKopruKod = (string)raporParam["ORGKOD"];
            krGider.kriterYil = OrtakFonksiyonlar.ConvertToInt(raporParam["yil"], DateTime.Now.Year);
            krGider.kriterAy = OrtakFonksiyonlar.ConvertToInt(raporParam["ay"], DateTime.Now.Month);

            ObjectArray list = servisIKY.PersonelGiderListele(kullanan, krGider);
            if (!list.sonuc.islemSonuc)
                throw new Exception(list.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);
            int baslaSatir = 0;
            int sutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);

            int fSayfa = 0, fsat1 = 0, fsut1 = 0, fsat2 = 0, fsut2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref fSayfa, ref fsat1, ref fsut1, ref fsat2, ref fsut2);

            string baslik = krGider.kriterYil + " YILI " + krGider.kriterAy + ". AY GİDER LİSTESİDİR";
            XLS.HucreAdBulYaz("baslik", baslik);
            int satir = baslaSatir;

            int satirYukseklik = XLS.SatirYukseklikAl(satir);

            int satirSay = 1;
            foreach (IKYPersonelGider pa in list.objeler)
            {
                XLS.HucreKopyala(fsat1, fsut1, fsat2, fsut2, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun + 0, satirSay);
                XLS.HucreDegerYaz(satir, sutun + 1, pa.tckimlikNo);
                XLS.HucreDegerYaz(satir, sutun + 2, pa.ad + " " + pa.soyad);
                XLS.HucreDegerYaz(satir, sutun + 3, pa.saymanlikKod + "-" + pa.birimKod);
                XLS.HucreDegerYaz(satir, sutun + 4, pa.tarihDT);
                XLS.HucreDegerYaz(satir, sutun + 5, pa.harcamaTurAdi);
                XLS.HucreDegerYaz(satir, sutun + 6, pa.tutar);
                satirSay++;
                satir++;
            }
            XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 3, satirYukseklik);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string FormatliRakam(int sayi, int kirilim)
        {
            string donusDeger;
            if (kirilim == 0 || kirilim == 2)
                donusDeger = OrtakFonksiyonlar.RomaRakamiyla(sayi);
            else if (kirilim == 1 || kirilim == 3)
            {
                donusDeger = "" + (char)(64 + sayi);
            }
            else
                donusDeger = sayi.ToString();
            if (kirilim == 2 || kirilim == 3)
                donusDeger = donusDeger.ToLower();
            return donusDeger;
        }

        private void DiziDegerAta(int[] dizi, int basla, int deger)
        {
            for (int i = basla; i < dizi.Length; i++)
            {
                dizi[i] = deger;
            }
        }

        private void SayilariYaz(Tablo XLS, int satir, ToplamAl toplamAl, int toplamDiziSatirNo)
        {
            for (int i = 0; i < toplamAl.SutunSay(); i++)
            {
                XLS.HucreDegerYaz(satir, 21 + i, toplamAl.toplam[toplamDiziSatirNo, i]);
            }
            XLS.HucreDegerYaz(satir, 21 + toplamAl.SutunSay(), toplamAl.SatToplamAl(toplamDiziSatirNo, 0, toplamAl.SutunSay() - 1));
        }

        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            AsposeLisans.Yukle();
            this.kullanan = (Kullanici)kullanan;
            //else if (ekBilgi.Equals("Personel"))
            //    return RaporPersonel(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            //else if (ekBilgi.Equals("Eğitim"))
            //    return RaporPersonelEgitim(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            //else if (ekBilgi.Equals("Unvan-Kadro"))
            //    return RaporPersonelUnvanCinsiyet(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            if (ekBilgi.Equals("IK009") || ekBilgi.Equals("IK043") || ekBilgi.Equals("IK044"))
                return DoluBosKadroYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK010"))
                return RaporPersonelKadroDoluBosIslem(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            //else if (ekBilgi.Equals("Yabancı Dil"))
            //    return RaporPersonelDil(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            //else if (ekBilgi.Equals("Personel Kadro Bilgisi"))
            //    return RaporPersonelKadroBilgisi(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            //else if (ekBilgi.Equals("Kan Grubu"))
            //    return RaporPersonelKanGrubu(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            //else if (ekBilgi.Equals("İzin Durum"))
            //    return RaporPersonelIzinDurumu(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK018") || ekBilgi.Equals("IK019") || ekBilgi.Equals("IK020") || ekBilgi.Equals("IK021") || ekBilgi.Equals("IK021b"))
                return IzinListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK021a"))
                return IdariGorevYazdir(this.kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK012"))
                return OrganizasyonSemaPersonelYaz(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK013"))
                return IsimliKadroListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK014"))
                return IsimsizKadroListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK015") || ekBilgi.Equals("IK036"))
                return KadroDereceDurumListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK016"))
                return AileListesiYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK017"))
                return OgrenimDurumListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK028") || ekBilgi.Equals("IK029") || ekBilgi.Equals("IK030"))
                return KartListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK031") || ekBilgi.Equals("IK032") || ekBilgi.Equals("IK033") || ekBilgi.Equals("IK034") || ekBilgi.Equals("IK035") || ekBilgi.Equals("IK046"))
                return HizmetListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK042"))
                return IsimliUnvanListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("GR009"))
                return DinamikRaporListesiYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK047") || ekBilgi.Equals("IK053") || ekBilgi.Equals("IK051"))
                return LojmanListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK048"))
                return LojmanDaireListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("IK050"))
                return LojmanDegisiklikListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi, false);
            else if (ekBilgi.Equals("IK052"))
                return LojmanDegisiklikListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi, true);
            else if (ekBilgi.Equals("IK049"))
                return PersonelKesintiListesiYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi, 1);
            else if (ekBilgi.Equals("IK049a"))
                return PersonelKesintiListesiYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi, 2);
            else if (ekBilgi.Equals("IK054"))
                return PersonelGiderListesiExceleYazdir(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else
                throw new Exception(ekBilgi + " kodlu rapor henüz hazırlanmamış!");
        }

        private string IdariGorevYazdir(Kullanici kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            TNS.IKY.Personel p = PersonelRaporlari.PersonelOl(raporParam);
            TNSDateTime gorevTar = new TNSDateTime((string)raporParam["sonTarih"]);
            if (!TNSDateTime.IsNull(gorevTar) && gorevTar.Yil == 1) gorevTar.isNull = true;
            if (TNSDateTime.IsNull(gorevTar))
                p.gorevYili = -1;
            else
                p.gorevYili = gorevTar.Yil;
            ObjectArray oa = TNS.IKY.Arac.Tanimla().PersonelListele(kullanan, p, false, true);
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.DosyaSaklamaFormatAta(ciktiYeri);
            int baslaSatir = 0;
            int sutun = 0;
            int fSayfa = 0, fSatir1 = 0, fSutun1 = 0, fSatir2 = 0, fSutun2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref fSayfa, ref fSatir1, ref fSutun1, ref fSatir2, ref fSutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            int satir = baslaSatir;
            XLS.HucreAdBulYaz("kurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            int yuks = XLS.SatirYukseklikAl(baslaSatir);
            foreach (TNS.IKY.Personel item in oa.objeler)
            {
                if (TarihVarMi(item.gorevler, gorevTar))
                {
                    foreach (Gorevler g in item.gorevler.objeler)
                    {
                        if (TNSDateTime.IsNull(gorevTar) || TNS.ResmiTatil.TarihIcindeMi(gorevTar.Oku(), g.baslamaTarihiDT, g.bitisTarihiDT))
                        {
                            XLS.HucreKopyala(fSatir1, fSutun1, fSatir2, fSutun2, satir, sutun);
                            XLS.HucreDegerYaz(satir, sutun, satir - baslaSatir + 1);
                            XLS.HucreDegerYaz(satir, sutun + 1, item.sicilNo);
                            XLS.HucreDegerYaz(satir, sutun + 2, item.unvanAd);
                            XLS.HucreDegerYaz(satir, sutun + 3, item.adSoyad);
                            XLS.HucreDegerYaz(satir, sutun + 5, g.birimiAd);
                            XLS.HucreDegerYaz(satir, sutun + 6, g.unvanAd);
                            if (!TNSDateTime.IsNull(g.baslamaTarihi))
                                XLS.HucreDegerYaz(satir, sutun + 7, g.baslamaTarihiDT);
                            if (!TNSDateTime.IsNull(g.bitisTarihi))
                                XLS.HucreDegerYaz(satir, sutun + 8, g.bitisTarihiDT);
                            XLS.HucreDegerYaz(satir, sutun + 9, g.gorevDurumuAd);
                            XLS.HucreDegerYaz(satir, sutun + 10, g.aciklama);

                            satir++;
                        }
                    }
                }
            }
            XLS.SatirYukseklikAyarla(baslaSatir, satir - 1, -1, 7, yuks);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private bool TarihVarMi(ObjectArray gorevler, TNSDateTime gorevTar)
        {
            if (gorevler == null || gorevler.ObjeSayisi == 0) return false;
            if (TNSDateTime.IsNull(gorevTar)) return true;
            foreach (Gorevler item in gorevler.objeler)
            {
                if (TNS.ResmiTatil.TarihIcindeMi(gorevTar.Oku(), item.baslamaTarihiDT, item.bitisTarihiDT))
                    return true;
            }
            return false;
        }

        public Kullanici kullanan { get; set; }
    }
}
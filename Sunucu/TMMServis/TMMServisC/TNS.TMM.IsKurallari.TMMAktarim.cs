using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class TMMAktarim
    {
        private static Sonuc KaydetKontroluYap(Kullanici kullanan, Aktarim tk)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(tk.muhasebe.kod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (string.IsNullOrEmpty(tk.harcamaBirimi.kod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (string.IsNullOrEmpty(tk.ambar.kod))
                hata += TasinirMalResI.TMM031 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, tk.muhasebe.kod, tk.harcamaBirimi.kod, tk.ambar.kod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            hata = Genel.KontrolMuhasebeHarcamaAmbar(tk.muhasebe.kod, tk.harcamaBirimi.kod, tk.ambar.kod);
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Aktarim tk)
        {
            Sonuc sonuc = KaydetKontroluYap(kullanan, tk);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAktarim tabTK = new TABAktarim(ve);
            ve.BeginTransaction();

            try
            {
                if (tk.belgeTarihi == null || tk.belgeTarihi.isNull)
                    tk.belgeTarihi = new TNSDateTime(DateTime.Today);

                tk.durum = (int)ENUMTesisBilesenDurum.KAYITEDILDI;
                tk.islemTarih = new TNSDateTime(DateTime.Today);
                tk.islemYapanAd = kullanan.ADSOYAD;
                tk.islemYapanKod = kullanan.TCKIMLIKNO;

                if (string.IsNullOrEmpty(tk.kod))
                {
                    tk.kod = Guid.NewGuid().ToString();
                    sonuc = tabTK.Kaydet(tk);
                }
                else
                {
                    sonuc = tabTK.Guncelle(tk);
                    sonuc = tabTK.SilDetay(tk.kod);
                }

                //Yuklenen dosyanın yerine konması
                string kayitEdilecekDosyaYol = Path.Combine(AktarimDosyaYoluVer(), tk.kod);
                File.Copy(tk.yuklenenDosyaAdi, kayitEdilecekDosyaYol, true);
                //********************

                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = tk.kod;

                    //Tarihçeye gönderilen dosya işelenecek kaydı atılsın
                    tabTK.SilTarihce(tk.kod);
                    tabTK.KaydetTarihce(tk.kod, "Gönderilen dosyadaki verilerin kaydı yapılacak");

                    ParameterizedThreadStart paraThread = new ParameterizedThreadStart(AktarimDosyaIcerikKaydetThread);
                    Thread th = new Thread(paraThread);
                    th.Name = "AktarimDosyaIcerikKaydetThread";
                    Object obj = new object[] { kullanan, tk.kod };
                    th.Start(obj);
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static string AktarimDosyaYoluVer()
        {
            string tasinirYol = OrtakFonksiyonlar.WebConfigOku("TasinirDosyaYol", "");
            string kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "Aktarim");
            if (!Directory.Exists(kayitEdilecekDosyaYol))
                Directory.CreateDirectory(kayitEdilecekDosyaYol);

            return kayitEdilecekDosyaYol;
        }

        public static void AktarimDosyaIcerikKaydetThread(object obj)
        {
            Object[] vListe = (Object[])obj;
            Kullanici kullanan = (Kullanici)vListe[0];
            string aktarimKod = (string)vListe[1];

            if (string.IsNullOrWhiteSpace(aktarimKod))
                return;

            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAktarim tabTK2 = new TABAktarim(ve2);

            //Tarihçeye gönderilen dosya işlenmeye başladı  kaydı atılsın
            tabTK2.KaydetTarihce(aktarimKod, "Gönderilen dosyadaki veriler okunmaya başlandı");

            string okunacakDosyaYol = Path.Combine(AktarimDosyaYoluVer(), aktarimKod);

            TMMExcelOkuyucu XLS = new TMMExcelOkuyucu(); /* Core: eski kod istemci kitapligindaki OrtakClass.Tablo sinifini kullaniyordu */
            XLS.DosyaAc(okunacakDosyaYol);

            int sayac = -1;
            int siraNo = 1;
            ObjectArray liste = new ObjectArray();

            List<string> hesapKodlari = new List<string>();
            while (true)
            {
                sayac++;
                if (sayac % 2000 == 0 && sayac > 0)
                    tabTK2.KaydetTarihce(aktarimKod, sayac.ToString("#,##0") + " adet satır okundu");

                string hesapKod = XLS.HucreDegerAl(sayac, 0);

                if (string.IsNullOrWhiteSpace(hesapKod) && sayac > 10)
                    break;

                if (!hesapKod.StartsWith("150") && !hesapKod.StartsWith("25")) continue;

                string[] hkler = hesapKod.Split('.');
                hesapKod = "";

                for (int i = 0; i < hkler.Length; i++)
                {
                    string h = hkler[i];
                    if (string.IsNullOrWhiteSpace(h)) continue;
                    if (h.Length == 1) h = "0" + h;

                    hesapKod += h;
                }

                AktarimDetay tkd = new AktarimDetay();
                tkd.kod = Guid.NewGuid().ToString();
                tkd.kodAna = aktarimKod;
                tkd.siraNo = siraNo++;
                tkd.hesapPlanKod = hesapKod.Replace(".", "");
                tkd.miktar = OrtakFonksiyonlar.ConvertToInt(XLS.HucreDegerAl(sayac, 1), 0);
                if (tkd.miktar == 0) tkd.miktar = 1;

                tkd.kdv = OrtakFonksiyonlar.ConvertToInt(XLS.HucreDegerAl(sayac, 2), 0);
                tkd.birimFiyat = XLS.HucreDegerAlDbl(sayac, 3);
                tkd.aciklama = XLS.HucreDegerAl(sayac, 4);
                tkd.eserAdi = XLS.HucreDegerAl(sayac, 5);
                tkd.yazarAdi = XLS.HucreDegerAl(sayac, 6);
                tkd.sicilNo = XLS.HucreDegerAl(sayac, 7);

                if (tkd.aciklama.Length > 255) tkd.aciklama = tkd.aciklama.Substring(0, 255);
                if (tkd.eserAdi.Length > 255) tkd.eserAdi = tkd.eserAdi.Substring(0, 255);
                if (tkd.yazarAdi.Length > 255) tkd.yazarAdi = tkd.yazarAdi.Substring(0, 255);
                if (tkd.sicilNo.Length > 50) tkd.sicilNo = tkd.sicilNo.Substring(0, 50);

                liste.objeler.Add(tkd);

                bool bulundu = false;
                foreach (string hp in hesapKodlari)
                {
                    if (hp == tkd.hesapPlanKod)
                    {
                        bulundu = true;
                        break;
                    }
                }
                if (!bulundu)
                    hesapKodlari.Add(tkd.hesapPlanKod);
            }

            tabTK2.KaydetTarihce(aktarimKod, "Gönderilen dosyaki tüm veriler (" + (siraNo - 1).ToString("#,##0") + ") okundu. Hesap kodu kontrolü yapılacak");

            string hesapKodKontrol = "";
            foreach (string hk in hesapKodlari)
            {
                hesapKodKontrol += Genel.KontrolHesapPlan(kullanan, hk, 0, true, false);
            }
            if (hesapKodKontrol != "")
            {
                tabTK2.KaydetTarihce(aktarimKod, "Gönderilen dosyaki hesap kodlarında hata var. Hata:" + hesapKodKontrol);
                ve2.VeriErisimKapat();
                return;
            }


            //******************************************************
            //Kontrol yapılacak
            //Veritabanındaki kayıtlardan fiş üretilecek
            //******************************************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAktarim tabTK = new TABAktarim(ve);
            ve.BeginTransaction();

            Sonuc sonuc = tabTK.SilDetay(aktarimKod);
            sayac = 0;//yukarıdan farklı olarak 0 dan başlamalı

            tabTK2.KaydetTarihce(aktarimKod, "Okunan veriler kayıt ediliyor");

            foreach (AktarimDetay tkd in liste.objeler)
            {
                sayac++;

                if (sayac % 2000 == 0)
                    tabTK2.KaydetTarihce(aktarimKod, sayac.ToString("#,##0") + " adet satır kayıt edildi");

                sonuc = tabTK.KaydetDetay(tkd);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                Aktarim tks = new Aktarim();
                tks.kod = aktarimKod;
                tks.toplamKayit = sayac;
                tks.islenenKayit = 0;
                tks.olusanBelgeNo = "";
                tabTK2.GuncelleSayilar(tks);
                tabTK2.KaydetTarihce(aktarimKod, "Gönderilen dosyanın tüm satırları (" + sayac.ToString("#,##0") + ")kayıt edildi");
                ve.Commit();
            }
            else
            {
                tabTK2.KaydetTarihce(aktarimKod, "Gönderilen dosyadaki veriler kayıt edilirken hata oluştu. Son işlenen satır:" + sayac.ToString("#,##0") + " Hata:" + sonuc.hataStr + " " + sonuc.vtHatasi);

                ve.Rollback();
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();

            Object objYeni = new object[] { kullanan, aktarimKod };
            AktarimTIFOlustur(objYeni);

            return;
        }

        public static void AktarimTIFOlustur(object obj)
        {
            Sonuc sonuc = new Sonuc();

            Object[] vListe = (Object[])obj;
            Kullanici kullanan = (Kullanici)vListe[0];
            string aktarimKod = (string)vListe[1];


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAktarim tabTK2 = new TABAktarim(ve2);

            Aktarim ak = new Aktarim();
            ak.kod = aktarimKod;
            ObjectArray listeAna = Listele(ve, kullanan, ak);
            if (listeAna.objeler.Count > 0)
                ak = (Aktarim)listeAna[0];
            else
                return;

            tabTK2.KaydetTarihce(aktarimKod, "Belge üretimi aşamasına geçildi");

            AktarimDetay tk = new AktarimDetay { kodAna = aktarimKod, durum = 2 };//işlem görmemiş olanları listele
            ObjectArray listeDetay = ListeleDetay(ve, kullanan, tk);

            TasinirIslemForm tf = new TasinirIslemForm();
            tf.yil = DateTime.Now.Year;
            tf.muhasebeKod = ak.muhasebe.kod;
            tf.harcamaKod = ak.harcamaBirimi.kod;
            tf.ambarKod = ak.ambar.kod;
            tf.islemTipKod = ak.islemTipiKod;
            tf.islemTipTur = 0;
            tf.neredenGeldi = ak.neredenGeldi;
            tf.aciklama = ak.aciklama;
            tf.fisTarih = ak.belgeTarihi;
            tf.islemTarih = new TNSDateTime(DateTime.Now);
            tf.islemYapan = kullanan.kullaniciKodu;
            tf.kayittanSonraOnayla = true;

            int sayac = 0;
            string[] sonucAlanlar = null;
            foreach (AktarimDetay akd in listeDetay.objeler)
            {
                TasinirIslemDetay td = new TasinirIslemDetay();
                td.yil = tf.yil;
                td.muhasebeKod = tf.muhasebeKod;
                td.harcamaKod = tf.harcamaKod;
                td.ambarKod = tf.ambarKod;
                td.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(akd.birimFiyat);
                td.birimFiyatKDVLi = td.birimFiyat;
                td.hesapPlanKod = akd.hesapPlanKod;
                td.ozellik.yazarAdi = akd.yazarAdi;
                td.ozellik.adi = akd.eserAdi;
                td.ozellik.disSicilNo = akd.sicilNo;
                td.ozellik.saseNo= akd.aciklama;
                td.eSicilNo = akd.sicilNo;
                td.kdvOran = akd.kdv;
                td.miktar = OrtakFonksiyonlar.ConvertToDecimal(akd.miktar);
                td.siraNo = sayac + 1;
                td.eAciklama = akd.aciklama;
                td.yerleskeYeri = akd.kod;


                tf.detay.objeler.Add(td);

                if (sayac % 10000 == 0 && sayac > 0)
                {
                    sonuc = TMMTasinirIslemForm.Kaydet(ve, kullanan, tf);
                    if (!sonuc.islemSonuc)
                    {
                        tabTK2.KaydetTarihce(aktarimKod, "Belge üretilirken hata oluştu. Son işlenen satır:" + sayac.ToString("#,##0") + " Hata:" + sonuc.hataStr + " " + sonuc.vtHatasi);
                        break;
                    }

                    foreach (TasinirIslemDetay td2 in tf.detay.objeler)
                    {
                        tabTK2.DurumGuncelleDetay(td2.yerleskeYeri, 1);
                    }

                    sonucAlanlar = sonuc.anahtar.Split('-');
                    sonuc.anahtar = sonucAlanlar[0];

                    Aktarim tks = new Aktarim();
                    tks.kod = aktarimKod;
                    tks.toplamKayit = -1;
                    tks.islenenKayit = sayac;
                    tks.olusanBelgeNo = "";
                    tabTK2.GuncelleSayilar(tks);

                    tabTK2.KaydetTarihce(aktarimKod, "Belge Üretildi. Belge No:" + sonuc.anahtar + ". İşlenen satır sayısı:" + sayac.ToString("#,##0"));
                    tf.detay = new ObjectArray();
                    tf.fisNo = "";
                    tf.kod = "";
                }

                sayac++;
            }

            if (tf.detay.objeler.Count > 0)
            {
                sonuc = TMMTasinirIslemForm.Kaydet(ve, kullanan, tf);
                if (!sonuc.islemSonuc)
                {
                    tabTK2.KaydetTarihce(aktarimKod, "Belge üretilirken hata oluştu. Son işlenen satır:" + sayac.ToString("#,##0") + " Hata:" + sonuc.hataStr + " " + sonuc.vtHatasi);
                    ve.VeriErisimKapat();
                    return;
                }

                sonucAlanlar = sonuc.anahtar.Split('-');
                sonuc.anahtar = sonucAlanlar[0];

                Aktarim tks = new Aktarim();
                tks.kod = aktarimKod;
                tks.toplamKayit = -1;
                tks.islenenKayit = sayac;
                tks.olusanBelgeNo = "";
                tabTK2.GuncelleSayilar(tks);

                foreach (TasinirIslemDetay td2 in tf.detay.objeler)
                {
                    tabTK2.DurumGuncelleDetay(td2.yerleskeYeri, 1);
                }

                tabTK2.KaydetTarihce(aktarimKod, "Son Belge Üretildi. Belge No:" + sonuc.anahtar + ". İşlenen satır sayısı:" + sayac.ToString("#,##0"));
                tf.detay = new ObjectArray();
                tf.fisNo = "";
                tf.kod = "";
            }

            tabTK2.KaydetTarihce(aktarimKod, "Bütün satırlar için Belge üretimi tamamlandı");

            ve.VeriErisimKapat();
        }

        public static Sonuc AktarimDevamEt(VeriErisim veriErisim, Kullanici kullanan, Aktarim tk)
        {
            ParameterizedThreadStart paraThread = new ParameterizedThreadStart(AktarimTIFOlustur);
            Thread th = new Thread(paraThread);
            th.Name = "AktarimDosyaIcerikKaydetThread";
            Object obj = new object[] { kullanan, tk.kod };
            th.Start(obj);

            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = true;

            return sonuc;
        }

        public static Sonuc DurumGuncelle(VeriErisim veriErisim, Kullanici kullanan, string kod, int durumKod)
        {
            Sonuc sonuc = new Sonuc();
            if (kod == "") return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAktarim tabTK = new TABAktarim(ve);
            ve.BeginTransaction();

            sonuc = tabTK.DurumGuncelle(kod, durumKod);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Aktarim tk)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAktarim tabTK = new TABAktarim(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTK.Listele(tk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                Aktarim gelen = new Aktarim();
                gelen.kod = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.KOD, string.Empty);
                gelen.muhasebe.kod = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                gelen.muhasebe.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod, "TASMUHASEBE");

                gelen.harcamaBirimi.kod = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty);
                gelen.harcamaBirimi.kod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaBirimi.kod);
                gelen.harcamaBirimi.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod + "-" + gelen.harcamaBirimi.kod, "TASHARCAMABIRIM");

                gelen.ambar.kod = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.AMBARKOD, string.Empty);
                gelen.ambar.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod + "-" + gelen.harcamaBirimi.kod + "-" + gelen.ambar.kod, "TASAMBAR");

                gelen.belgeTarihi = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.BELGETARIH);
                gelen.islemTipiKod = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.ISLEMTIPIKOD, 0);
                gelen.belgeTur = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.BELGETUR, 0);
                gelen.neredenGeldi = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.NEREDENGELDI, "");
                gelen.aciklama = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.ACIKLAMA, "");
                gelen.islemTarih = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.ISLEMTARIH);
                gelen.islemYapanKod = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.ISLEMYAPANKOD, "");
                gelen.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.ISLEMYAPANAD, "");
                gelen.durum = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.DURUM, 0);

                gelen.toplamKayit = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.TOPLAMKAYIT, 0);
                gelen.islenenKayit = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.ISLENENKAYIT, 0);
                gelen.olusanBelgeNo = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlar.BELGENO, "").Trim();
                if (gelen.olusanBelgeNo == ",") gelen.olusanBelgeNo = "";

                donusDeger.objeler.Add(gelen);
            }
            dr.Close();

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();

            return donusDeger;
        }

        public static ObjectArray ListeleTarihce(VeriErisim veriErisim, Kullanici kullanan, string ilgilKod)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAktarim tabTK = new TABAktarim(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTK.ListeleTarihce(ilgilKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }


            while (dr.Read())
            {
                Aktarim gelen = new Aktarim();
                gelen.islemTarih = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarTarihce.ISLEMTARIH);
                gelen.aciklama = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarTarihce.ACIKLAMA, "");

                donusDeger.objeler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();

            return donusDeger;
        }

        public static ObjectArray ListeleDetay(VeriErisim veriErisim, Kullanici kullanan, AktarimDetay tk)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAktarim tabTK = new TABAktarim(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTK.ListeleDetay(tk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                AktarimDetay gelen = new AktarimDetay();
                gelen.kod = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.KOD, string.Empty);
                gelen.kodAna = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.KODANA, string.Empty);
                gelen.siraNo = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.SIRANO, 0);
                gelen.hesapPlanKod = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.HESAPPLANKOD, "");
                gelen.miktar = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.MIKTAR, 0.0);
                gelen.kdv = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.KDV, 0);
                gelen.birimFiyat = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.BIRIMFIYAT, 0.0);
                gelen.aciklama = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.ACIKLAMA, "");
                gelen.eserAdi = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.ESERADI, "");
                gelen.yazarAdi = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.YAZARADI, "");
                gelen.sicilNo = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.SICILNO, "");
                gelen.durum = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.DURUM, 0);
                gelen.belgeNo = VeriErisim.DegerAl(dr, (int)TABAktarim.ENUMAlanlarDetay.BELGENO, "");

                donusDeger.objeler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}
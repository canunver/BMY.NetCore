using System;
using System.Collections;
using System.Collections.Generic;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Makbuzlardan ödeme emri oluşturma; eski ButceMuhasebe/Makbuz.aspx.cs btnOdemeEmri_Click, OdemeEmriOlustur, HesapEkle,
    /// SatirNoBelirle ve Sirala kodunun aynısıdır. Ödeme emri belgeleri tarih aralığındaki her gün için birim bazında (web.config
    /// MakbuzOdemeEmriOlusturmaSekli = KISIBAZINDA ise kişi bazında) oluşturulur; makbuz durumu MakbuzOdemeEmriOlusturDurum ayarından
    /// (yoksa ödendi) gelir.
    /// </summary>
    public static class MakbuzOdemeEmri
    {
        /// <summary>Oluşan belgeler için mesaj satırları döndürür; listeleme hatasında hata doludur.</summary>
        public static List<string> Olustur(Kullanici kullanan, IMUHServis servisMUH, DateTime basTarih, DateTime bitTarih, int muhasebe, out string hata)
        {
            hata = null;
            List<string> mesajlar = new List<string>();

            // Odeme emri belgeleri birim bazında oluşturulacak bu nedenle
            // Verilen koşullarda makbuzların hangi birimlerde üretildiklerinin bulunması gerekli
            Makbuz makbuz = new Makbuz();
            makbuz.yil = basTarih.Year;
            makbuz.araBasTarih.Yaz(basTarih);
            makbuz.araBitTarih.Yaz(bitTarih);
            makbuz.muhasebe = muhasebe;
            makbuz.durum = (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI; // Ödendi

            int durumKod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.WebConfigOku("MakbuzOdemeEmriOlusturDurum", ""), 0);
            if (durumKod > 0)
                makbuz.durum = durumKod;
            durumKod = makbuz.durum;

            ObjectArray mmler = new ObjectArray();
            string belgeOlusturmeSekli = OrtakFonksiyonlar.WebConfigOku("MakbuzOdemeEmriOlusturmaSekli", "");

            if (belgeOlusturmeSekli == "")
                mmler = servisMUH.MakbuzBirimleriListele(kullanan, makbuz);
            else if (belgeOlusturmeSekli == "KISIBAZINDA")
                mmler = servisMUH.MakbuzKisileriListele(kullanan, makbuz);

            if (!mmler.sonuc.islemSonuc)
            {
                hata = mmler.sonuc.hataStr;
                return mesajlar;
            }

            TNSDateTime bitis = new TNSDateTime(bitTarih);
            TNSDateTime islemTarih = new TNSDateTime(basTarih);

            while (islemTarih.Oku() <= bitis.Oku())
            {
                foreach (Makbuz birim in mmler.objeler)
                {
                    Makbuz mk = new Makbuz();
                    mk.yil = basTarih.Year;
                    mk.araBasTarih.Yaz(islemTarih.Oku());
                    mk.araBitTarih.Yaz(islemTarih.Oku());
                    mk.muhasebe = muhasebe;
                    mk.birim = birim.birim;
                    mk.tcKimlikNoVergiNo = birim.tcKimlikNoVergiNo;
                    mk.kisiAd = birim.kisiAd;
                    mk.durum = durumKod; // (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI; // Ödendi
                    mk.kisiBilgisiTamBul = true;
                    ObjectArray mler = servisMUH.MakbuzListele(kullanan, mk);
                    if (!mler.sonuc.islemSonuc)
                    {
                        hata = mler.sonuc.hataStr;
                        return mesajlar;
                    }

                    if (mler.objeler.Count > 0)
                    {
                        string odemeEmriNo = OdemeEmriOlustur(kullanan, servisMUH, mler, belgeOlusturmeSekli);

                        string belgesiOlusanGrup = birim.birim + " birimi";
                        if (belgeOlusturmeSekli == "KISIBAZINDA")
                            belgesiOlusanGrup = birim.kisiAd;

                        if (OrtakFonksiyonlar.ConvertToDbl(odemeEmriNo) > 0)
                            mesajlar.Add(belgesiOlusanGrup + " için " + odemeEmriNo + " nolu ödeme emri belgesi oluşturuldu.");
                        else
                            mesajlar.Add(belgesiOlusanGrup + " için ödeme emri belgesi oluşturulurken hata oluştu. Hata: " + odemeEmriNo);
                    }
                }

                islemTarih.AddDays(1);
            }

            return mesajlar;
        }

        private static string OdemeEmriOlustur(Kullanici kullanan, IMUHServis servisMUH, ObjectArray makbuzlar, string olusturmaSekli)
        {
            Hashtable hesapOzellik = new Hashtable();

            string oncekiGelirHesabi = "";
            Makbuz ilkMakbuz = (Makbuz)makbuzlar.objeler[0];
            string birimKod = ilkMakbuz.birim;
            string topluAciklama = "";
            double yansitTutar = 0;
            OdemeEmriMIFForm o = new OdemeEmriMIFForm();

            // Birime ait kurumsal kodu bulmak için.
            string kurKod = servisMUH.SaymanlikBirimKurkodBul(ilkMakbuz.muhasebe, ilkMakbuz.birim);
            if (kurKod != "")
            {
                string[] kurkodlar = kurKod.Split(',');
                kurKod = kurkodlar[0].Trim();
            }

            string odemeEmriNo = "";

            Hashtable htKur = new Hashtable();

            foreach (Makbuz m in makbuzlar.objeler)
            {
                double kurDegeri = 1;
                if (!string.IsNullOrWhiteSpace(m.paraBirimiKod) && m.paraBirimiKod != "TRY")
                {
                    kurDegeri = OrtakFonksiyonlar.ConvertToDbl(htKur[m.paraBirimiKod + m.islemTarih.ToString()]);
                    if (kurDegeri == 0)
                    {
                        TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                        TNS.HRC.MerkezBankasiKurSinif kriter = new TNS.HRC.MerkezBankasiKurSinif();
                        kriter.kurTarih = m.islemTarih;
                        TNS.HRC.MerkezBankasiKurSinif kurListe = servisHRC.MerkezBankKurOku(kullanan, kriter);
                        foreach (TNS.HRC.MerkezBankasiKurDetaySinif kur in kurListe.satirlar)
                        {
                            if (kur.paraBirim.kisaltma == m.paraBirimiKod)
                            {
                                kurDegeri = OrtakFonksiyonlar.ConvertToDbl(kur.DegerBul("ALIS", 1));
                                htKur[m.paraBirimiKod + m.islemTarih.ToString()] = kurDegeri;
                                break;
                            }
                        }
                        if (kurDegeri == 0) kurDegeri = 1; // Kur bulunmadıysa
                    }
                }

                double kartlaOdenen = m.krediKartiIleOdenen;
                double tutarKDVli = m.makbuzTipiTutar;
                double kdvTutar = Math.Round(tutarKDVli * m.makbuzTipi.kdv / (100 + m.makbuzTipi.kdv), 2);
                double tutarKDVsiz = m.makbuzTipiTutar - kdvTutar;

                if (m.odemeEmriNo != "") odemeEmriNo = m.odemeEmriNo;

                string kdvAltHesap = "";
                string onceki600Hesap = "";

                if (topluAciklama != "") topluAciklama += ", ";
                topluAciklama += m.aciklama;

                foreach (MakbuzHesap mh in m.hesaplar)
                {
                    if (mh.hesaplananAlan > 0) continue;

                    if (!(mh.hesapKod.StartsWith("191") || mh.hesapKod.StartsWith("391")))
                    {
                        HesapEkle(kullanan, servisMUH, o, mh.hesapKod, 0, mh.tutar, mh.altHesap, mh.tertip, birimKod, ref hesapOzellik, m.paraBirimiKod, kurDegeri, kurKod);
                    }
                    tutarKDVli += mh.tutar;

                    string yansitHesap = "";
                    string kdvHesap = "";

                    if (mh.hesapKod.StartsWith("191") || mh.hesapKod.StartsWith("391"))
                    {
                        kdvHesap = mh.hesapKod;
                        yansitHesap = oncekiGelirHesabi;
                        oncekiGelirHesabi = "";
                        kdvAltHesap = onceki600Hesap;
                    }

                    if (mh.hesapKod.StartsWith("140") || mh.hesapKod.StartsWith("120"))
                    {
                        string y = YansitmaHesabi(kullanan, servisMUH, m.yil, mh.hesapKod);
                        if (y != null) yansitHesap = y;
                    }

                    if (mh.hesapKod.StartsWith("600"))
                    {
                        yansitHesap = "800" + mh.hesapKod.Substring(3, mh.hesapKod.Length - 3);
                        oncekiGelirHesabi = yansitHesap;
                        onceki600Hesap = mh.hesapKod;
                    }

                    if (!string.IsNullOrWhiteSpace(yansitHesap))
                    {
                        HesapEkle(kullanan, servisMUH, o, yansitHesap, 0, mh.tutar, "", mh.tertip, birimKod, ref hesapOzellik, m.paraBirimiKod, kurDegeri, kurKod);
                        yansitTutar += mh.tutar * kurDegeri;
                    }

                    if (!string.IsNullOrWhiteSpace(kdvHesap))
                    {
                        HesapEkle(kullanan, servisMUH, o, kdvHesap, 0, mh.tutar, kdvAltHesap, "", birimKod, ref hesapOzellik, m.paraBirimiKod, kurDegeri, kurKod);
                    }
                }

                // Hesaplanacak alanlar (Makbuzda makbuz tipi olarak seçilip kayıt edilenler (ayrıca elle satır girilmiş ise yukarıda işleniyor)
                HesapEkle(kullanan, servisMUH, o, m.makbuzTipi.hesapKod, 0, tutarKDVsiz, m.tcKimlikNoVergiNo, "", birimKod, ref hesapOzellik, m.paraBirimiKod, kurDegeri, kurKod);
                HesapEkle(kullanan, servisMUH, o, m.makbuzTipi.kdvHesapKod, 0, kdvTutar, m.makbuzTipi.hesapKod, "", birimKod, ref hesapOzellik, m.paraBirimiKod, kurDegeri, kurKod); // alt hesap olarak makbuz hesap kodu gönderiliyor ki kdv ayrı ayrı hesaplanabilsin

                HesapEkle(kullanan, servisMUH, o, m.makbuzTipi.kasaHesapKod, (tutarKDVli - kartlaOdenen), 0, "", "", birimKod, ref hesapOzellik, m.paraBirimiKod, kurDegeri, kurKod);
                HesapEkle(kullanan, servisMUH, o, m.makbuzTipi.krediHesapKod, kartlaOdenen, 0, "", "", birimKod, ref hesapOzellik, m.paraBirimiKod, kurDegeri, kurKod);

                HesapEkle(kullanan, servisMUH, o, m.makbuzTipi.yansitmaHesapKod, 0, m.makbuzTipiTutar, "", "", birimKod, ref hesapOzellik, m.paraBirimiKod, kurDegeri, kurKod);
                if (m.makbuzTipi.yansitmaHesapKod != "")
                    yansitTutar += m.makbuzTipiTutar * kurDegeri;
            }

            if (yansitTutar > 0)
                HesapEkle(kullanan, servisMUH, o, "805", yansitTutar, 0, "", "", birimKod, ref hesapOzellik, "", 1, kurKod);

            o.yil = ilkMakbuz.yil;
            o.muhasebe = ilkMakbuz.muhasebe;
            o.birim = ilkMakbuz.birim;
            o.aciklama = "Günlük makbuz kayıtları";
            o.formDurum = 1;
            o.islemTarih = ilkMakbuz.islemTarih;
            o.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            o.belgeNo = odemeEmriNo;
            o.kurKod = kurKod;

            if (olusturmaSekli == "KISIBAZINDA")
            {
                o.ilgiliAd = ilkMakbuz.kisiAd;
                o.ilgiliNo = ilkMakbuz.tcKimlikNoVergiNo;
                o.aciklama = topluAciklama;
            }

            Sonuc sonuc = new Sonuc();
            o.detay.objeler.Sort(new Sirala());

            TNSCollection detayTemp = o.detay.objeler;
            o.detay.objeler = new TNSCollection();

            foreach (OdemeEmriMIFDetay kd in detayTemp)
            {
                if (kd.kesinti == 0)
                {
                    o.detay.objeler.Add(kd);

                    foreach (OdemeEmriMIFDetay kd2 in detayTemp)
                    {
                        if (kd2.altHesapKod == kd.hesapKod)
                        {
                            kd2.satirAciklama = kd2.altHesapKod;
                            kd2.altHesapKod = "";
                            o.detay.objeler.Add(kd2);
                            kd2.kesinti = -99;
                        }
                    }
                }
            }

            sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, o);

            if (!sonuc.islemSonuc)
            {
                return sonuc.hataStr;
            }
            else
            {
                foreach (Makbuz m in makbuzlar.objeler)
                {
                    m.odemeEmriNo = sonuc.anahtar;
                }

                servisMUH.MakbuzOdemeEmriGuncelle(kullanan, makbuzlar);
                return sonuc.anahtar;
            }
        }

        /// <summary>
        /// Eski koddaki 140 ve 120 hesapları için aynı blok: hesap yansıtma niteliğindeyse hesap planındaki yansıtma hesabı (sonuncusu).
        /// Yansıtma niteliği yoksa null (yansıtma hesabı değişmez).
        /// </summary>
        private static string YansitmaHesabi(Kullanici kullanan, IMUHServis servisMUH, int yil, string hesapKod)
        {
            bool yansitmaListele = false;
            HesapPlaniSatir hps = new HesapPlaniSatir();
            hps.hesapKod = hesapKod;
            hps.yil = yil;
            ObjectArray hpsl = servisMUH.HesapPlaniListele(kullanan, hps);
            foreach (HesapPlaniSatir hpss in hpsl.objeler)
            {
                if (TNS.MUH.Arac.NitelikVarmi(hpss.nitelik, (int)ENUMHesapKodNitelik.YANSITMAYAPILACAK)) yansitmaListele = true;
                else if (TNS.MUH.Arac.NitelikVarmi(hpss.nitelik, (int)ENUMHesapKodNitelik.YANSITMAZORUNLUBORC)) yansitmaListele = true;
                else if (TNS.MUH.Arac.NitelikVarmi(hpss.nitelik, (int)ENUMHesapKodNitelik.YANSITMAZORUNLUALACAK)) yansitmaListele = true;
            }

            if (!yansitmaListele) return null;

            string yansitHesap = null;
            HesapPlaniYansitma hpy = new HesapPlaniYansitma();
            hpy.yil = yil;
            hpy.hesapKod = hesapKod;
            ObjectArray hpyl = servisMUH.YansitmaListele(hpy);
            foreach (HesapPlaniYansitma hpyy in hpyl.objeler)
            {
                yansitHesap = hpyy.yansitmaHesapKod;
            }
            return yansitHesap;
        }

        private static void HesapEkle(Kullanici kullanan, IMUHServis servisMUH, OdemeEmriMIFForm o, string hesapKod, double borc, double alacak, string altHesapKod,
            string tertip, string detayBirim, ref Hashtable hesapOzellik, string paraBirimiKod, double kurDegeri, string kurKod)
        {
            if (hesapKod.Trim() == "") return;

            HesapPlaniSatir hesapNitelik = new HesapPlaniSatir();

            if (borc > 0 || alacak > 0)
            {
                if (hesapOzellik[hesapKod.Replace(".", "")] == null)
                {
                    HesapPlaniSatir hs = new HesapPlaniSatir();
                    hs.yil = DateTime.Now.Year;
                    hs.hesapKod = hesapKod;
                    ObjectArray hsListe = servisMUH.HesapPlaniListele(kullanan, hs);

                    foreach (HesapPlaniSatir h in hsListe.objeler)
                    {
                        hesapOzellik[hesapKod.Replace(".", "")] = h;
                    }
                }

                hesapNitelik = (HesapPlaniSatir)hesapOzellik[hesapKod.Replace(".", "")];

                // Eğer Makbuz tanımlamada belirtilen hesap kodu yok ise ödeme emrine eklensin ki ödeme emri
                // hazırlanması sırasında uyarı vererek durumu kullanıcıya bildirelim
                if (hesapNitelik != null)
                    if (hesapNitelik.altHesapKod < 1 && !hesapKod.StartsWith("391")) altHesapKod = "";

                OdemeEmriMIFDetay sat;
                int tut = -1;
                for (int i = 0; i < o.detay.objeler.Count; i++)
                {
                    OdemeEmriMIFDetay oed = (OdemeEmriMIFDetay)o.detay.objeler[i];
                    if (oed.hesapKod == hesapKod && oed.altHesapKod == altHesapKod && oed.TertipStr().Replace(".", "").Replace("-", "") == tertip.Replace(".", "").Replace("-", ""))
                    {
                        tut = i;
                        break;
                    }
                }

                if (tut < 0)
                {
                    sat = new OdemeEmriMIFDetay(o.yil);
                    sat.hesapKod = hesapKod;
                    sat.borc = 0;
                    sat.alacak = 0;
                    sat.altHesapKod = altHesapKod;
                    sat.detayBirim = detayBirim + "." + kurKod;

                    TNS.MUH.Arac.TertipAta(ref sat, tertip);

                    sat.satirNo = SatirNoBelirle(hesapKod);
                    o.detay.Ekle(sat);
                }
                else
                    sat = (OdemeEmriMIFDetay)o.detay.objeler[tut];

                sat.borc += borc * kurDegeri;
                sat.alacak += alacak * kurDegeri;

                if (kurDegeri > 1 && hesapNitelik != null && hesapNitelik.dovizKurKod != "")
                {
                    sat.doviz = paraBirimiKod;
                    sat.dovizBorc += borc;
                    sat.dovizAlacak += alacak;
                    sat.dovizKurTur = "ALIS";
                    sat.dovizKur = OrtakFonksiyonlar.ConvertToDecimal(kurDegeri);
                }
            }
        }

        private static int SatirNoBelirle(string hesapKod)
        {
            if (hesapKod.StartsWith("333")) return 100;
            else if (hesapKod.StartsWith("140")) return 150;
            else if (hesapKod.StartsWith("600")) return 200;
            else if (hesapKod.StartsWith("391")) return 300;
            else if (hesapKod.StartsWith("330")) return 400;
            else if (hesapKod.StartsWith("100")) return 500;
            else if (hesapKod.StartsWith("109")) return 600;
            else if (hesapKod.StartsWith("800")) return 700;
            else if (hesapKod.StartsWith("805")) return 800;
            else return 10000;
        }

        private class Sirala : IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 == o2) return 0;
                OdemeEmriMIFDetay i_o1 = (OdemeEmriMIFDetay)o1;
                OdemeEmriMIFDetay i_o2 = (OdemeEmriMIFDetay)o2;
                return (i_o1.satirNo + i_o1.hesapKod).CompareTo((i_o2.satirNo + i_o2.hesapKod));
            }
        }
    }
}

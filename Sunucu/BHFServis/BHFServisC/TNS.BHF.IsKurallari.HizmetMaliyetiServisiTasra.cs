using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.BHF.IsKurallari.Veri;
using System.Data;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Summary description for HizmetMaliyetiServisi.
    /// </summary>
    public class HizmetMaliyetiServisiTasra
    {
        //public static ObjectArray SozlukListele(VeriErisim veriErisim)
        //{
        //    ObjectArray donusDeger = new ObjectArray();

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul());
        //    //TABHizmetMaliyetTasra thm = new TABHizmetMaliyetTasra(ve);

        //    //IDataReader r = thm.KodlariAl();
        //    //if (!string.IsNullOrEmpty(thm.Hata()))
        //    //{
        //    //    donusDeger.sonuc.islemSonuc = false;
        //    //    donusDeger.sonuc.hataStr = thm.Hata();
        //    //    ve.VeriErisimKapat();
        //    //    return donusDeger;
        //    //}

        //    //while (r.Read())
        //    //{
        //    //    HizmetMaliyetiSatir hms = new HizmetMaliyetiSatir();
        //    //    hms.kod = VeriErisim.DegerAl(r, 0, string.Empty);
        //    //    hms.aciklama = VeriErisim.DegerAl(r, 1, string.Empty);
        //    //    hms.degerGirilsinMi = VeriErisim.DegerAl(r, 2, 0) == 1;

        //    //    donusDeger.Ekle(hms);
        //    //}
        //    //r.Close();

        //    //if (donusDeger.objeler.Count <= 0)
        //    //    donusDeger.sonuc.bilgiStr = "Hizmet Maliyeti Tespiti sözlük bilgisi bulunamadı!";

        //    //ve.VeriErisimKapat();
        //    return donusDeger;
        //}

        //public static Sonuc SozlukKaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection hmsler)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    //if (!Servis.Yoneticimi(kullanan))
        //    //{
        //    //    sonuc.islemSonuc = false;
        //    //    sonuc.hataStr = "Bu bölümü kullanmaya yetkili değilsiniz!";
        //    //    return sonuc;
        //    //}

        //    //foreach (HizmetMaliyetiSatir hms in hmsler)
        //    //{
        //    //    if (string.IsNullOrEmpty(hms.kod))
        //    //        sonuc.hataStr = "Kod alanı boş geliyor.<br>";
        //    //    if (string.IsNullOrEmpty(hms.aciklama))
        //    //        sonuc.hataStr += "Ad alanı boş geliyor.<br>";

        //    //    if (!string.IsNullOrEmpty(sonuc.hataStr))
        //    //    {
        //    //        sonuc.islemSonuc = false;
        //    //        return sonuc;
        //    //    }
        //    //}

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul());
        //    //TABHizmetMaliyetTasra thm = new TABHizmetMaliyetTasra(ve);
        //    //ve.BeginTransaction();

        //    //thm.KodSil();
        //    //foreach (HizmetMaliyetiSatir hms in hmsler)
        //    //{
        //    //    sonuc = thm.KodKaydet(hms);
        //    //    if (!sonuc.islemSonuc)
        //    //        break;
        //    //}

        //    //if (sonuc.islemSonuc)
        //    //{
        //    //    sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
        //    //    ve.Commit();
        //    //}
        //    //else
        //    //{
        //    //    sonuc.hataStr = "Kayıt işlemi başarısız oldu!<br>" + sonuc.hataStr;
        //    //    ve.Rollback();
        //    //}

        //    //ve.VeriErisimKapat();
        //    return sonuc;
        //}

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurumsalKod, bool kurumsalToplam)
        {
            HizmetMaliyetiTesbitiSatir hms;
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc.islemSonuc = false;
            string ustKod = "";
            int say = 0;

            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            if (!muhServis.MuhasebeBirimYetkiVarMi(kullanan, muhasebeKod.ToString(), birimKod, MUH.ENUMOGY.Gorme))
            {
                donusDeger.sonuc.DegerAta("Bu birimle (muhasebe:" + muhasebeKod + ", birim: " + birimKod + ") ilgili verileri görme yetkiniz bulunmamaktadır.");
                return donusDeger;
            }

            if (!string.IsNullOrWhiteSpace(kurumsalKod) && !string.IsNullOrWhiteSpace(birimKod) && muhasebeKod > 0)
            {
                MUH.MuhasebeBirim mb = new MUH.MuhasebeBirim();
                mb.birimKod = birimKod;
                mb.kurum = kurumsalKod;
                mb.muhasebe = muhasebeKod;

                ObjectArray oa = muhServis.MuhasebeBirimListele(mb);
                if (!oa.sonuc.islemSonuc)
                    return oa;
                if (oa.ObjeSayisi != 1)
                {
                    donusDeger.sonuc.DegerAta(string.Format("Lütfen geçerli saymanlık ({0}), birim({1}) ve kurumsal kod({2}) seçiniz", muhasebeKod, birimKod, kurumsalKod));
                    return donusDeger;
                }
            }
            kurumsalKod = kurumsalKod.Replace(".", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABHizmetMaliyet thm = new TABHizmetMaliyet(ve);
            TABHizmetMaliyetTasra thmt = new TABHizmetMaliyetTasra(ve);
            IDataReader r = thm.KodlariAl();
            if (thm.Hata() == "")
            {
                while (r.Read())
                {
                    hms = new HizmetMaliyetiTesbitiSatir();
                    hms.kod = r.GetString(0);
                    ustKod = hms.kod;
                    hms.aciklama = r.GetString(1);
                    hms.degerGirilsinMi = r.GetInt32(2) == 1;
                    donusDeger.Ekle(hms);
                }
                r.Close();
                donusDeger.sonuc.islemSonuc = true;
                for (int i = 0; i < donusDeger.ObjeSayisi; i++)
                {
                    hms = (HizmetMaliyetiTesbitiSatir)donusDeger[i];
                    r = thmt.DegerleriAl(yil, muhasebeKod, birimKod, kurumsalKod, hms.kod);
                    hms.kod = TNS.OrtakFonksiyonlar.KodNoktaKoy("9.99.99", hms.kod);
                    if (thm.Hata() == "")
                    {
                        if (r.Read())
                        {
                            hms.gercek = VeriErisim.DegerAl(r, 0, (double)0);
                            hms.haziran = VeriErisim.DegerAl(r, 1, (double)0);
                            hms.tahmin = VeriErisim.DegerAl(r, 2, (double)0);
                            hms.tahmin1 = VeriErisim.DegerAl(r, 3, (double)0);
                            hms.tahmin2 = VeriErisim.DegerAl(r, 4, (double)0);
                        }
                        else
                        {
                            hms.gercek = 0;
                            hms.haziran = 0;
                            hms.tahmin = 0;
                            hms.tahmin1 = 0;
                            hms.tahmin2 = 0;
                        }
                        r.Close();
                        if (hms.tahmin > 0)
                            say++;
                    }
                    else
                    {
                        donusDeger.sonuc.hataStr = thm.Hata();
                        donusDeger.sonuc.islemSonuc = false;
                        break;
                    }
                }
                if (donusDeger.sonuc.islemSonuc && ustKod != "")
                {
                    r = thmt.DegerleriAlDiger(yil, muhasebeKod, birimKod, kurumsalKod, ustKod);
                    if (thm.Hata() == "")
                    {
                        int i = 1;
                        while (r.Read())
                        {
                            hms = new HizmetMaliyetiTesbitiSatir();
                            hms.kod = ustKod + "." + i.ToString().PadLeft(2, '0');
                            hms.gercek = VeriErisim.DegerAl(r, 2, (double)0);
                            hms.haziran = VeriErisim.DegerAl(r, 3, (double)0);
                            hms.tahmin = VeriErisim.DegerAl(r, 4, (double)0);
                            hms.tahmin1 = VeriErisim.DegerAl(r, 5, (double)0);
                            hms.tahmin2 = VeriErisim.DegerAl(r, 6, (double)0);
                            hms.aciklama = VeriErisim.DegerAl(r, 7, "");
                            hms.degerGirilsinMi = true;
                            donusDeger.Ekle(hms);
                            i++;
                        }
                        r.Close();
                    }
                    else
                    {
                        donusDeger.sonuc.hataStr = thm.Hata();
                        donusDeger.sonuc.islemSonuc = false;
                    }
                }
            }
            else
                donusDeger.sonuc.hataStr = thm.Hata();
            if (say > 0)
                donusDeger.sonuc.anahtar = say.ToString();
            else
                donusDeger.sonuc.anahtar = "";
            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurumsalKod, TNSCollection hizmetMaliyeti)
        {
            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            Sonuc sonuc = new Sonuc(false, "", "");
            string digerBilgi = "", kod = "";

            TNS.BHMv2.IBHMv2Servis bhmv2Servis = TNS.BHMv2.Arac.Tanimla();
            //if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            //{
                int durum = bhmv2Servis.OnayKodOkuTasra(kullanan, yil, 0, "000", "0000", BHMv2.ENUMBirimOnayTur.HIZMETMALIYETITASRA);
                if (durum >= 4)
                {
                    sonuc.DegerAta("Hizmet Maliyeti Tespiti Formu girilen yıl için veri girişine kapalı olduğundan değişiklik yapamazsınız!");
                    return sonuc;
                }
            //}
            MUH.MuhasebeBirim muhasebeBirim = new MUH.MuhasebeBirim();
            muhasebeBirim.birimKod = birimKod;
            muhasebeBirim.muhasebe = muhasebeKod;
            muhasebeBirim.kurum = kurumsalKod;

            ObjectArray oa = muhServis.MuhasebeBirimListele(muhasebeBirim);

            if (oa.ObjeSayisi < 1 || kurumsalKod.Length < 8)
            {
                sonuc.DegerAta("Girilen saymanlık, birim veya kurumsal kod hatalı, lütfen düzeltiniz!");
                return sonuc;
            }

            if (!muhServis.MuhasebeBirimYetkiVarMi(kullanan, muhasebeKod.ToString(), birimKod, MUH.ENUMOGY.Yazma))
            {
                sonuc.DegerAta("Bu birimle (muhasebe:" + muhasebeKod + ", birim: " + birimKod + ") ilgili veri kaydetme yetkiniz bulunmamaktadır.");
                return sonuc;
            }
            kurumsalKod = kurumsalKod.Replace(".", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABHizmetMaliyetTasra thm = new TABHizmetMaliyetTasra(ve);
            ve.BeginTransaction();
            sonuc = thm.Sil(yil, muhasebeKod, birimKod, kurumsalKod);
            if (sonuc.islemSonuc)
            {
                int kod6Sayac = 0;
                foreach (HizmetMaliyetiTesbitiSatir hms in hizmetMaliyeti)
                {
                    if (hms.degerGirilsinMi)
                    {
                        kod = hms.kod.Replace(".", "");
                        if (kod.StartsWith("6") || kod == "")
                        {
                            kod6Sayac++;
                            kod = "6" + kod6Sayac.ToString().PadLeft(2, '0');
                            digerBilgi = hms.aciklama;
                        }
                        else
                            digerBilgi = "";
                        sonuc = thm.Ekle(yil, muhasebeKod, birimKod, kurumsalKod, kod, hms.gercek, hms.haziran, hms.tahmin, hms.tahmin1, hms.tahmin2, digerBilgi);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
using System;
using System.Collections;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections.Generic;
using TNS.DEG;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class MUHFatura
    {
        /// <summary>
        /// Kaydets the specified ve.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Fatura bilgi) 
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sonuc = Kontrol("KAYDET", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            bilgi.islemYapan = kullanan.ad + " " + kullanan.soyad;
            bilgi.islemTarihi = new TNSDateTime(DateTime.Now);

            if (bilgi.tahakkukBelgeNo != "") bilgi.durum = (int)ENUMFaturaDurum.TAHAKKUK;
            if (bilgi.odemeBelgeNo != "") bilgi.durum = (int)ENUMFaturaDurum.ODENDI;
            if (bilgi.tahakkukBelgeNo == "" && bilgi.odemeBelgeNo == "") bilgi.durum = (int)ENUMFaturaDurum.YENI;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABFatura to = new TABFatura(ve);
            TABGenel tg = new TABGenel(ve);

            if (bilgi.kod > 0)
                sonuc = to.Guncelle(bilgi);
            else
            {
                bool otoNumara = OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEFATURAOTONUMARA"), 0) > 0 ? true : false;

                if (otoNumara)
                {
                    bilgi.faturaNo = tg.BelgeNoAl("FATURANOSAYAC" + bilgi.faturaTarihi.Yil, "00000");
                    bilgi.faturaNo = bilgi.faturaTarihi.Yil + bilgi.faturaNo;
                }

                bilgi.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("FATURAKODSAYAC", "00000"), 0);

                sonuc = to.Kaydet(bilgi);
            }

            if (sonuc.islemSonuc)
            {
                sonuc = to.FaturaDetaySil(bilgi.kod);
                foreach (FaturaDetay fd in bilgi.detay.objeler)
                {
                    fd.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("FATURAKODSAYAC", "00000"), 0);
                    fd.faturaKod = bilgi.kod;
                    sonuc = to.FaturaDetayKaydet(fd);
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = bilgi.kod.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Listeles the specified ve.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Fatura bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABFatura to = new TABFatura(ve);
            IDataReader dr = null;

            try
            {
                dr = to.Listele(bilgi);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            Fatura b = new Fatura();
            int islenenFaturaKod = 0;
            int detayIndex = (int)TABFatura.ENUMAlanlar.TEVKIFATTUTAR;
            while (dr.Read())
            {
                int kod = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.KOD, 0);
                if (islenenFaturaKod != kod)
                {
                    b = new Fatura();
                    b.kod = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.KOD, 0);
                    b.musteriKod = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.MUSTERIKOD, "");
                    b.faturaNo = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.FATURANO, "");
                    b.faturaTarihi = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.FATURATARIHI);
                    b.irsaliyeNo = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.IRSALIYENO, "");
                    b.irsaliyeTarihi = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.IRSALIYETARIHI);
                    b.aciklama = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.ACIKLAMA, "");
                    b.toplam = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.TOPLAM, 0.0);
                    b.kdvToplam = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.KDVTOPLAM, 0.0);
                    b.genelToplam = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.GENELTOPLAM, 0.0);
                    b.odemeTarihi = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.ODEMETARIHI);
                    b.odenenToplam = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.ODENENTOPLAM, 0.0);
                    b.odenenTutar = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.ODENENTUTAR, 0.0);
                    b.kesintiTutar = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.KESINTITUTAR, 0.0);
                    b.odemeParaBirimi = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.ODEMEPARABIRIMI, "");
                    b.faturaDili = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.FATURADILI, 0);
                    b.faturaParaBirimi = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.FATURAPARABIRIMI, "");
                    b.durum = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.DURUM, 0);
                    b.musteriAdi = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.MUSTERIADI, "");
                    b.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.MUHASEBEKOD, 0);
                    b.birimKod = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.BIRIMKOD, "");
                    b.tahakkukBelgeNo = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.TAHAKKUKBELGENO, "");
                    b.odemeBelgeNo = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.ODEMEBELGENO, "");
                    b.muhBelgeAciklama = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.MUHBELGEACIKLAMA, "");
                    b.tahakkukHesapKod = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.TAHAKKUKHESAPKOD, "");
                    b.odemeHesapKod = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.ODEMEHESAPKOD, "");
                    b.islemYapan = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.ISLEMYAPAN, "");
                    b.islemTarihi = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.ISLEMTARIHI);
                    b.musteriAdiEk = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.MUSTERIADIEK, "");
                    b.mudurlukKod = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.MUDURLUKKOD, "");
                    b.tevkifatOran = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.TEVKIFATORAN, 0);
                    b.tevkifatTutar = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlar.TEVKIFATTUTAR, 0.0);

                    liste.Ekle(b);
                    islenenFaturaKod = b.kod;
                }

                if (islenenFaturaKod == b.kod)
                {
                    FaturaDetay fd = new FaturaDetay();
                    fd.aciklama = VeriErisim.DegerAl(dr, detayIndex + 1, "");
                    fd.miktar = VeriErisim.DegerAl(dr, detayIndex + 2, 0.0);
                    fd.kdvOran = VeriErisim.DegerAl(dr, detayIndex + 3, 0.0);
                    fd.birimFiyat = VeriErisim.DegerAl(dr, detayIndex + 4, 0.0);
                    fd.kdvTutar = VeriErisim.DegerAl(dr, detayIndex + 5, 0.0);
                    fd.tutar = VeriErisim.DegerAl(dr, detayIndex + 6, 0.0);
                    fd.hesapKod = VeriErisim.DegerAl(dr, detayIndex + 7, "");

                    b.detay.objeler.Add(fd);
                }

            }

            dr.Close();
            ve.VeriErisimKapat();
            return liste;
        }

        /// <summary>
        /// Faturas the durum guncelle.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static Sonuc DurumGuncelle(VeriErisim veriErisim, Kullanici kullanan, Fatura bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sonuc = Kontrol("IPTAL", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABFatura to = new TABFatura(ve);

            if (bilgi.kod > 0)
            {
                bilgi.durum = (int)ENUMFaturaDurum.IPTAL;
                bilgi.islemYapan = kullanan.ad + " " + kullanan.soyad;
                bilgi.islemTarihi = new TNSDateTime(DateTime.Now);
                sonuc = to.DurumGuncelle(bilgi);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Faturas the durum guncelle.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static Sonuc MIFBelgeNoGuncelle(VeriErisim veriErisim, Kullanici kullanan, Fatura bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (bilgi.kod == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Fatura kod bilgisi boş bırakılamaz";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABFatura to = new TABFatura(ve);

            bilgi.islemYapan = kullanan.ad + " " + kullanan.soyad;
            bilgi.islemTarihi = new TNSDateTime(DateTime.Now);

            sonuc = to.MIFBelgeNoGuncelle(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Kontrols the specified tip.
        /// </summary>
        /// <param name="tip">The tip.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        private static Sonuc Kontrol(string tip, Kullanici kullanan, Fatura bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (bilgi.kod == 0)
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }

            if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (bilgi.muhasebeKod == 0)
                    sonuc.hataStr += "Muhasebe kod bilgisi boş bırakılamaz." + "<br>";
                if (string.IsNullOrEmpty(bilgi.birimKod))
                    sonuc.hataStr += "Birim Kod bilgisi boş bırakılamaz." + "<br>";
                if (string.IsNullOrEmpty(bilgi.mudurlukKod))
                    sonuc.hataStr += "Müdürlük Kod bilgisi boş bırakılamaz." + "<br>";
                if (string.IsNullOrEmpty(bilgi.musteriKod))
                    sonuc.hataStr += "Müşteri bilgisi boş bırakılamaz." + "<br>";
                if (bilgi.faturaTarihi.isNull)
                    sonuc.hataStr += "Fatura Tarihi boş bırakılamaz." + "<br>";

                int satir = 1;
                foreach (FaturaDetay fd in bilgi.detay.objeler)
                {
                    if (fd.aciklama == "")
                        sonuc.hataStr += satir + ". satırda Açıklama alanı boş bırakılmış." + "<br>";
                    if (fd.hesapKod == "")
                        sonuc.hataStr += satir + ". satırda Gelir Hesap Kodu alanı boş bırakılmış." + "<br>";
                    if (fd.tutar == 0)
                        sonuc.hataStr += satir + ". satırda Satır Tutarı alanı boş bırakılmış." + "<br>";
                    //if (fd.kdvOran == 0)
                    //    sonuc.hataStr += satir + ". satırda KDV oranı alanı boş bırakılmış." + "<br>";
                }
            }

            if (tip == "IPTAL")
            {
                if (bilgi.kod == 0)
                    sonuc.hataStr = "İlk önce kayıtlı fatura seçiniz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

        /// <summary>
        /// Faturas the aciklama kaydet.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static Sonuc FaturaAciklamaKaydet(VeriErisim veriErisim, Kullanici kullanan, FaturaAciklama bilgi)
        {
            Sonuc sonuc = new Sonuc();

            sonuc = FaturaAciklamaKontrol("", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABFatura to = new TABFatura(ve);

            if (bilgi.kod > 0)
                sonuc = to.FaturaAciklamaGuncelle(bilgi);
            else
            {
                if (bilgi.kod == 0)
                    bilgi.kod = to.EnBuyukAciklamaKod();

                sonuc = to.FaturaAciklamaKaydet(bilgi);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return sonuc;
        }

        /// <summary>
        /// Faturas the aciklama listele.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray FaturaAciklamaListele(VeriErisim veriErisim, Kullanici kullanan, FaturaAciklama bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABFatura to = new TABFatura(ve);
            IDataReader dr = null;

            try
            {
                dr = to.FaturaAciklamaListele(bilgi);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            while (dr.Read())
            {
                FaturaAciklama t = new FaturaAciklama();
                t.kod = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlarAciklama.KOD, 0);
                t.tur = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlarAciklama.TUR, 0);
                t.aciklama = VeriErisim.DegerAl(dr, (int)TABFatura.ENUMAlanlarAciklama.ACIKLAMA, "");
                liste.Ekle(t);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return liste;
        }

        /// <summary>
        /// Faturas the aciklama sil.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static Sonuc FaturaAciklamaSil(VeriErisim veriErisim, Kullanici kullanan, FaturaAciklama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sonuc = FaturaAciklamaKontrol("SIL", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABFatura to = new TABFatura(ve);

            sonuc = to.FaturaAciklamaSil(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc FaturaAciklamaKontrol(string tip, Kullanici kullanan, FaturaAciklama bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (bilgi.kod == 0)
                    sonuc.hataStr = "Silmek istediğiniz kaydın kod bilgisini giriniz.";
            }

            if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (string.IsNullOrEmpty(bilgi.aciklama))
                    sonuc.hataStr = "Fatura Açıklama Boş bırakılamaz." + "<br<";

            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

        public static Sonuc KaydetMuhasebeAciklama(VeriErisim veriErisim, Kullanici kullanan, Fatura bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABFatura to = new TABFatura(ve);

            sonuc = to.GuncelleMuhasebeAciklama(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}

using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;
using TNS.DEG;
using TNS.BKD;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class TMMUretimFormu
    {
        private static Sonuc KaydetKontroluYap(Kullanici kullanan, UretimFormu tk)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(tk.girenMuhasebe.kod))
                hata += "Malzemenin Gireceği muhasebe kodu boş olamaz<br>";
            if (string.IsNullOrEmpty(tk.girenHarcamaBirimi.kod))
                hata += "Malzemenin Gireceği harcama birim kodu boş olamaz<br>";
            if (string.IsNullOrEmpty(tk.girenAmbar.kod))
                hata += "Malzemenin Gireceği ambar kodu boş olamaz<br>";
            if (string.IsNullOrEmpty(tk.cikanMuhasebe.kod))
                hata += "Malzemenin Çıktığı muhasebe kodu boş olamaz<br>";
            if (string.IsNullOrEmpty(tk.cikanHarcamaBirimi.kod))
                hata += "Malzemenin Çıktığı harcama birim kodu boş olamaz<br>";
            if (string.IsNullOrEmpty(tk.cikanAmbar.kod))
                hata += "Malzemenin Çıktığı ambar kodu boş olamaz<br>";
            if (string.IsNullOrEmpty(tk.anaUrun.hesapKod))
                hata += "Üretilecek malzeme bilgisi boş olamaz<br>";
            if (tk.miktar <= 0)
                hata += "Miktar alanı 0 dan büyük olmalıdır" + "<br>";
            if (tk.fisTarihi.isNull)
                hata += "Fiş tarihi boş bırakılamaz" + "<br>";

            hata += Genel.KontrolMuhasebeHarcamaAmbar(tk.girenMuhasebe.kod, tk.girenHarcamaBirimi.kod, tk.girenAmbar.kod);
            hata += Genel.KontrolMuhasebeHarcamaAmbar(tk.cikanMuhasebe.kod, tk.cikanHarcamaBirimi.kod, tk.cikanAmbar.kod);

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, tk.girenMuhasebe.kod, tk.girenHarcamaBirimi.kod, tk.girenAmbar.kod);
            if (!sonuc.islemSonuc)
                hata += TasinirMalResI.TMM050 + "<br>";

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, tk.cikanMuhasebe.kod, tk.cikanHarcamaBirimi.kod, tk.cikanAmbar.kod);
            if (!sonuc.islemSonuc)
                hata += TasinirMalResI.TMM050 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, UretimFormu tk)
        {
            Sonuc sonuc = KaydetKontroluYap(kullanan, tk);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABUretimFormu tabTK = new TABUretimFormu(ve);
            ve.BeginTransaction();

            tk.anaUrun.hesapKod = tk.anaUrun.hesapKod.Replace(".", "");
            tk.girenHarcamaBirimi.kod = tk.girenHarcamaBirimi.kod.Replace(".", "");
            tk.cikanHarcamaBirimi.kod = tk.cikanHarcamaBirimi.kod.Replace(".", "");
            tk.islemYapan = kullanan.ADSOYAD;
            if (tk.fisTarihi == null || tk.fisTarihi.isNull)
                tk.fisTarihi = new TNSDateTime(DateTime.Today);

            tk.durum = (int)ENUMTesisBilesenDurum.KAYITEDILDI;

            try
            {
                sonuc = UretimCikisGirisFisiUret(kullanan, tk);
                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }

                tk.cikanFisNo = sonuc.anahtar;
                tk.girenFisNo = sonuc.bilgiStr;

                if (string.IsNullOrEmpty(tk.kod))
                {
                    tk.kod = Guid.NewGuid().ToString();
                    string belgeNo = new TABGenel(ve).BelgeNoAl("URETIMFISISAYAC" + tk.fisTarihi.Yil, "0000");
                    tk.fisNo = OrtakFonksiyonlar.ConvertToInt(tk.fisTarihi.Yil.ToString() + belgeNo, 0);

                    sonuc = tabTK.Kaydet(tk);
                }
                else
                    sonuc = tabTK.Guncelle(tk);

                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = tk.kod;
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

        public static Sonuc DurumGuncelle(VeriErisim veriErisim, Kullanici kullanan, string kod, int durumKod)
        {
            Sonuc sonuc = new Sonuc();
            if (kod == "") return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABUretimFormu tabTK = new TABUretimFormu(ve);
            ve.BeginTransaction();

            sonuc = tabTK.DurumGuncelle(kod, durumKod);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, UretimFormu tk)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABUretimFormu tabTK = new TABUretimFormu(ve);
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

            int sonKolon = (int)TABUretimFormu.ENUMAlanlar.CIKANTIFFISNO;
            while (dr.Read())
            {
                UretimFormu gelen = new UretimFormu();
                gelen.kod = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.KOD, string.Empty);

                gelen.girenMuhasebe.kod = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.GIRENMUHASEBEKOD, string.Empty);
                gelen.girenHarcamaBirimi.kod = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.GIRENHARCAMABIRIMKOD, string.Empty);
                gelen.girenAmbar.kod = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.GIRENAMBARKOD, string.Empty);
                gelen.girenHarcamaBirimi.kod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.girenHarcamaBirimi.kod);

                gelen.cikanMuhasebe.kod = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.CIKANMUHASEBEKOD, string.Empty);
                gelen.cikanHarcamaBirimi.kod = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.CIKANHARCAMABIRIMKOD, string.Empty);
                gelen.cikanAmbar.kod = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.CIKANAMBARKOD, string.Empty);
                gelen.cikanHarcamaBirimi.kod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.cikanHarcamaBirimi.kod);

                gelen.fisNo = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.FISNO, 0);
                gelen.fisTarihi = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.FISTARIH);
                gelen.durum = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.DURUM, 0);

                gelen.anaUrun.hesapKod = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.ANAURUNHESAPPLANKOD, "");
                gelen.anaUrun.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.anaUrun.hesapKod);
                gelen.miktar = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.MIKTAR, 0);
                gelen.islemYapan = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.ISLEMYAPAN, "");

                gelen.girenFisNo = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.GIRENTIFFISNO, "");
                gelen.cikanFisNo = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.CIKANTIFFISNO, "");

                gelen.girenMuhasebe.ad = VeriErisim.DegerAl(dr, sonKolon + 1, "");
                gelen.cikanMuhasebe.ad = VeriErisim.DegerAl(dr, sonKolon + 2, "");
                gelen.girenHarcamaBirimi.ad = VeriErisim.DegerAl(dr, sonKolon + 3, "");
                gelen.cikanHarcamaBirimi.ad = VeriErisim.DegerAl(dr, sonKolon + 4, "");
                gelen.girenAmbar.ad = VeriErisim.DegerAl(dr, sonKolon + 5, "");
                gelen.cikanAmbar.ad = VeriErisim.DegerAl(dr, sonKolon + 6, "");
                gelen.anaUrun.aciklama = VeriErisim.DegerAl(dr, sonKolon + 7, "");

                bool yetki = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, gelen.girenMuhasebe.kod, gelen.girenHarcamaBirimi.kod, gelen.girenAmbar.kod);

                if (yetki)
                    donusDeger.objeler.Add(gelen);
            }
            dr.Close();

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();

            return donusDeger;
        }


        public static Sonuc UretimCikisGirisFisiUret(Kullanici kullanan, UretimFormu tk)
        {
            IslemTip it = new IslemTip();
            it.tur = (int)ENUMIslemTipi.URETIMCIKIS;
            ObjectArray itListe = new TMMIslemTip().Listele(null, kullanan, it);
            int cikisIslemTipiKod = 0;
            foreach (IslemTip item in itListe.objeler)
            {
                cikisIslemTipiKod = item.kod;
            }

            it = new IslemTip();
            it.tur = (int)ENUMIslemTipi.URETILENGIRIS;
            itListe = new TMMIslemTip().Listele(null, kullanan, it);
            int girisIslemTipiKod = 0;
            foreach (IslemTip item in itListe.objeler)
            {
                girisIslemTipiKod = item.kod;
            }

            UretimRecetesi tr = new UretimRecetesi();
            tr.anaUrun.hesapKod = tk.anaUrun.hesapKod;
            ObjectArray receteListe = TMMUretimRecetesi.Listele(null, kullanan, tr);

            TasinirIslemForm tf = new TasinirIslemForm();
            tf.yil = tk.fisTarihi.Yil;
            tf.fisNo = tk.cikanFisNo;
            tf.fisTarih = tk.fisTarihi;
            tf.muhasebeKod = tk.cikanMuhasebe.kod;
            tf.harcamaKod = tk.cikanHarcamaBirimi.kod;
            tf.ambarKod = tk.cikanAmbar.kod;
            tf.islemTipTur = (int)ENUMIslemTipi.URETIMCIKIS;
            tf.islemTipKod = cikisIslemTipiKod;
            tf.aciklama = "Üretim Fişinden çıkış malzemeleri için oluşturulmuştur";

            decimal maliyetFiyati = 0;
            foreach (UretimRecetesi item in receteListe.objeler)
            {
                TuketimMalzemesiSatirOlustur(kullanan, item, tk, ref tf);
            }

            foreach (TasinirIslemDetay item in tf.detay.objeler)
            {
                maliyetFiyati += item.birimFiyat * item.miktar;
            }

            string cikisFisiNo = "";
            Sonuc sonuc = TMMTasinirIslemForm.Kaydet(null, kullanan, tf);

            if (sonuc.islemSonuc)
            {
                string[] tmp = sonuc.anahtar.Split('-');
                cikisFisiNo = tmp[0];
            }
            else
                return sonuc;


            //Giriş Fişi
            //*********************************************************
            tf = new TasinirIslemForm();
            tf.yil = tk.fisTarihi.Yil;
            tf.fisNo = tk.girenFisNo;
            tf.fisTarih = tk.fisTarihi;
            tf.muhasebeKod = tk.girenMuhasebe.kod;
            tf.harcamaKod = tk.girenHarcamaBirimi.kod;
            tf.ambarKod = tk.girenAmbar.kod;
            tf.islemTipTur = (int)ENUMIslemTipi.URETILENGIRIS;
            tf.islemTipKod = girisIslemTipiKod;
            tf.aciklama = "Üretim Fişinden üretilen malzeme için oluşturulmuştur";

            TasinirIslemDetay tdGiren = new TasinirIslemDetay();

            tdGiren.siraNo = 1;
            tdGiren.yil = tf.yil;
            tdGiren.muhasebeKod = tf.muhasebeKod;
            tdGiren.ambarKod = tf.ambarKod;
            tdGiren.harcamaKod = tf.harcamaKod;
            tdGiren.hesapPlanKod = tk.anaUrun.hesapKod;
            tdGiren.birimFiyat = maliyetFiyati;
            tdGiren.kdvOran = 20;
            tdGiren.miktar = tk.miktar;

            tf.detay.objeler.Add(tdGiren);

            string girisFisiNo = "";
            sonuc = TMMTasinirIslemForm.Kaydet(null, kullanan, tf);

            if (sonuc.islemSonuc)
            {
                string[] tmp = sonuc.anahtar.Split('-');
                girisFisiNo = tmp[0];
            }
            else
                return sonuc;

            sonuc.anahtar = cikisFisiNo;
            sonuc.bilgiStr = girisFisiNo;

            return sonuc;
        }

        private static void TuketimMalzemesiSatirOlustur(Kullanici kullanan, UretimRecetesi recete, UretimFormu tk, ref TasinirIslemForm tf)
        {
            int siraNo = tf.detay.objeler.Count + 1;

            recete.miktar = recete.miktar * tk.miktar;

            StokHareketBilgi shBilgi = new StokHareketBilgi();
            shBilgi.yil = tf.yil;
            shBilgi.muhasebeKod = tf.muhasebeKod;
            shBilgi.harcamaKod = tf.harcamaKod;
            shBilgi.ambarKod = tf.ambarKod;
            shBilgi.hesapPlanKod = recete.altUrun.hesapKod.Replace(".", "");

            ObjectArray bilgi = TMMStokHareket.AmbardakiStoklariGetir(null, kullanan, shBilgi);

            foreach (StokHareketBilgi stoktaki in bilgi.objeler)
            {
                TasinirIslemDetay td = new TasinirIslemDetay();

                td.siraNo = siraNo++;
                td.yil = tf.yil;
                td.muhasebeKod = tf.muhasebeKod;
                td.ambarKod = tf.ambarKod;
                td.harcamaKod = tf.harcamaKod;
                td.hesapPlanKod = recete.altUrun.hesapKod;

                decimal miktar = 0;
                if (OrtakFonksiyonlar.ConvertToDecimal(recete.miktar) >= stoktaki.miktar)
                    miktar = stoktaki.miktar;
                else
                    miktar = OrtakFonksiyonlar.ConvertToDecimal(recete.miktar);

                td.miktar = miktar;
                td.birimFiyat = stoktaki.birimFiyat;
                td.kdvOran = stoktaki.kdvOran;
                tf.detay.objeler.Add(td);

                recete.miktar = recete.miktar - OrtakFonksiyonlar.ConvertToDbl(td.miktar);

                if (Math.Round(recete.miktar, 10) <= 0)//bazı durumlarda 0 olamıyor
                    break;
            }

            if (Math.Round(recete.miktar, 10) > 0)
            {
                TasinirIslemDetay td = new TasinirIslemDetay();

                td.siraNo = 1;
                td.yil = tf.yil;
                td.muhasebeKod = tf.muhasebeKod;
                td.ambarKod = tf.ambarKod;
                td.harcamaKod = tf.harcamaKod;
                td.hesapPlanKod = recete.altUrun.hesapKod;

                td.miktar = OrtakFonksiyonlar.ConvertToDecimal(recete.miktar);
                td.birimFiyat = 0;
                td.kdvOran = 0;
                tf.detay.objeler.Add(td);
            }

        }
    }
}
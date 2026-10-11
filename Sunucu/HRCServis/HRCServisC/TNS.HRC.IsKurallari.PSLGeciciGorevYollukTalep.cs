using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Para birimi bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class PSLGeciciGorevYollukTalep
    {
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, GeciciGorevYollukTalepBaslik bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            // Tüm kullanıcılar satın alma talep formu üretebileceğinden, sadece muhasebe birim yetkisi kontrolü yapılıyor.
            if (!Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = Kontrol(kullanan, bilgi);

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYollukTalep to = new TABGeciciGorevYollukTalep(ve);
            ve.BeginTransaction();

            try
            {
                sonuc = to.SilAna(bilgi);
                if (sonuc.islemSonuc)
                    sonuc = to.SilDetay(bilgi.yil, bilgi.formKod);
                if (sonuc.islemSonuc)
                    sonuc = to.SilBeraberindekiler(bilgi.yil, bilgi.formKod);

                if (sonuc.islemSonuc)
                    sonuc = to.Kaydet(bilgi);
                int say = 1;
                if (sonuc.islemSonuc)
                    foreach (GeciciGorevYollukTalepDetay detay in bilgi.detay)
                    {
                        sonuc = to.KaydetDetay(bilgi.yil, bilgi.formKod, say++, detay);
                        if (!sonuc.islemSonuc)
                        {
                            sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                            break;
                        }
                    }
                say = 1;
                if (sonuc.islemSonuc)
                    foreach (GeciciGorevYollukBeraberler detay in bilgi.beraberindekiler)
                    {
                        sonuc = to.KaydetBeraber(bilgi.yil, bilgi.formKod, say++, detay.tcKimlik);
                        if (!sonuc.islemSonuc)
                        {
                            sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                            break;
                        }
                    }
                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = bilgi.formKod;
                    sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }
            catch (Exception e)
            {
                sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + e.Message);
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, GeciciGorevYollukTalepBaslik kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGeciciGorevYollukTalep to = new TABGeciciGorevYollukTalep(ve);
            IDataReader r = to.Oku(kriter.yil, kriter.formKod);

            if (r != null)
            {
                GeciciGorevYollukTalepBaslik f = null;
                if (r.Read())
                {
                    f = new GeciciGorevYollukTalepBaslik();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.formKod = VeriErisim.DegerAl(r, 1, "");
                    f.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    f.birim = VeriErisim.DegerAl(r, 3, "");
                    f.kurKod = VeriErisim.DegerAl(r, 4, "");
                    f.durum = VeriErisim.DegerAl(r, 5, 0);
                    f.tarih = VeriErisim.DegerAl(r, 6);
                    f.kimlikNo = VeriErisim.DegerAl(r, 7, "");
                    f.ad = VeriErisim.DegerAl(r, 8, "");
                    f.soyad = VeriErisim.DegerAl(r, 9, "");
                    f.unvan = VeriErisim.DegerAl(r, 10, "");
                    f.gundelikYil = VeriErisim.DegerAl(r, 11, 0);
                    f.gundelikKod = VeriErisim.DegerAl(r, 12, "");
                    f.gundelikTutar = VeriErisim.DegerAl(r, 13, (decimal)0);
                    f.gundelikDovizKod = VeriErisim.DegerAl(r, 14, (int)0);
                    f.nereden = VeriErisim.DegerAl(r, 15, "");
                    f.nereye = VeriErisim.DegerAl(r, 16, "");
                    f.baslangicTarih = VeriErisim.DegerAl(r, 17);
                    f.bitisTarih = VeriErisim.DegerAl(r, 18);
                    f.surecYil = VeriErisim.DegerAl(r, 19, 0);
                    f.surecNo = VeriErisim.DegerAl(r, 20, "");
                    f.tur = VeriErisim.DegerAl(r, 21, 0);
                    //f.gorevSuresi = VeriErisim.DegerAl(r, 22, 0);
                    f.gerekce = VeriErisim.DegerAl(r, 23, "");
                    f.aciklamalar = VeriErisim.DegerAl(r, 24, "");
                    f.butceTertibi = VeriErisim.DegerAl(r, 25, "");
                    f.avansTutari = VeriErisim.DegerAl(r, 26, (double)0);

                    f.sunanAd = VeriErisim.DegerAl(r, 27, "");
                    f.sunanUnvan = VeriErisim.DegerAl(r, 28, "");
                    f.hazirAd = VeriErisim.DegerAl(r, 29, "");
                    f.hazirUnvan = VeriErisim.DegerAl(r, 30, "");

                    f.onayAd = VeriErisim.DegerAl(r, 31, "");
                    f.onayUnvan = VeriErisim.DegerAl(r, 32, "");
                    f.onayMakam = VeriErisim.DegerAl(r, 33, "");
                    f.avansOdemeEmri = VeriErisim.DegerAl(r, 34, "");

                    bilgi.objeler.Add(f);
                }
                r.Close();
                HRCPersonel hrcP = new HRCPersonel();
                if (f != null)
                {
                    PersonelBilgi p = hrcP.Listele(ve, kullanan, f.kimlikNo, 0, "");
                    if (p != null)
                    {
                        p.AdSoyadBol(out f.ad, out f.soyad);
                    }
                    f.detay = ListeleDetay(ve, kullanan, f.yil, f.formKod);
                    f.beraberindekiler = ListeleBeraber(ve, kullanan, f.yil, f.formKod);
                }
            }
            else
                bilgi.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return bilgi;
        }

        public GeciciGorevYollukBeraberler[] ListeleBeraber(VeriErisim veriErisim, Kullanici kullanan, int yil, string formKod)
        {
            System.Collections.Generic.List<GeciciGorevYollukBeraberler> l = new System.Collections.Generic.List<GeciciGorevYollukBeraberler>();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGeciciGorevYollukTalep to = new TABGeciciGorevYollukTalep(ve);
            HRCPersonel hrcP = new HRCPersonel();

            IDataReader r = to.ListeleBeraber(yil, formKod);
            if (r != null)
            {
                while (r.Read())
                {
                    GeciciGorevYollukBeraberler t = new GeciciGorevYollukBeraberler();
                    t.tcKimlik = VeriErisim.DegerAl(r, 3, "");
                    l.Add(t);
                }
                r.Close();
            }
            else
                OrtakFonksiyonlar.HataStrYaz("PSLGeciciYollukTalep, Listele Beraber:" + ve.hata);

            foreach (GeciciGorevYollukBeraberler t in l)
            {
                PersonelBilgi p = hrcP.Listele(ve, kullanan, t.tcKimlik, 0, "");
                if (p != null)
                {
                    t.adSoyad = p.adSoyad;
                }
            }

            ve.VeriErisimKapat();

            return l.ToArray();
        }

        public TNSCollection ListeleDetay(VeriErisim veriErisim, Kullanici kullanan, int yil, string formKod)
        {
            TNSCollection bilgi = new TNSCollection();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGeciciGorevYollukTalep to = new TABGeciciGorevYollukTalep(ve);
            IDataReader r = to.ListeleDetay(yil, formKod);

            if (r != null)
            {
                while (r.Read())
                {
                    GeciciGorevYollukTalepDetay t = new GeciciGorevYollukTalepDetay();

                    t.tur = VeriErisim.DegerAl(r, 3, 0);
                    t.tarih = VeriErisim.DegerAl(r, 4);
                    t.gun = VeriErisim.DegerAl(r, 5, "");

                    bilgi.Add(t);
                }
                r.Close();
            }
            else
                OrtakFonksiyonlar.HataStrYaz("PSLGeciciYollukTalep, Listele Detay:" + ve.hata);

            ve.VeriErisimKapat();

            return bilgi;
        }

        public Sonuc Onaylandi(VeriErisim veriErisim, Kullanici kullanan, GeciciGorevYollukTalepBaslik form)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            //if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            //string hata = Kontrol(bilgi); ;


            //if (hata != "")
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = hata;
            //    return sonuc;
            //}

            GeciciGorevYollukTalepBaslik kriter = new GeciciGorevYollukTalepBaslik();
            kriter.yil = form.yil;
            kriter.formKod = form.formKod;

            ObjectArray l = Listele(null, kullanan, kriter);
            if (!l.sonuc.islemSonuc)
                return l.sonuc;
            if (l.ObjeSayisi == 1)
            {
                kriter = (GeciciGorevYollukTalepBaslik)l.objeler[0];

                if (kriter.durum == 3)
                {
                    sonuc.DegerAta("Onaylanmış belgeler değiştirilemez!" + "<br>");
                    return sonuc;
                }
            }
            else
            {
                sonuc.DegerAta("Onaylanmış belge bulunamadı!" + "<br>");
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYollukTalep to = new TABGeciciGorevYollukTalep(ve);
            ve.BeginTransaction();

            sonuc = to.DurumGuncelle(form, ENUMGeciciGorevYollukTalepDurum.MUHASEBEYEILETILDI);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = form.formKod + " nolu Yolluk Talep Formu durumu başarıyla güncellendi.";
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

        private string Kontrol(Kullanici kullanan, GeciciGorevYollukTalepBaslik bilgi)
        {
            string hata = string.Empty;
            if (string.IsNullOrEmpty(bilgi.formKod))
                hata += "Kayıt için form numarası vermelisiniz!" + "<br>";
            else
                bilgi.surecNo = bilgi.formKod;

            if (bilgi.yil < 2000)
                hata += "Kayıt için yıl değeri verilmelidir!" + "<br>";
            else
                bilgi.surecYil = bilgi.yil;

            if (string.IsNullOrEmpty(bilgi.gerekce))
                hata += "Kayıt için gerekçe alanı girilmelidir!" + "<br>";

            if (string.IsNullOrEmpty(bilgi.nereden))
                hata += "Kayıt için nereden alanı girilmelidir!" + "<br>";

            if (string.IsNullOrEmpty(bilgi.nereye))
                hata += "Kayıt için nereye alanı girilmelidir!" + "<br>";

            if (string.IsNullOrEmpty(bilgi.gundelikKod))
                hata += "Kayıt için gündelik miktarı seçilmelidir!" + "<br>";

            if (bilgi.baslangicTarih == null || bilgi.baslangicTarih.isNull)
                hata += "Kayıt için başlangıç tarihi girilmelidir!" + "<br>";

            if (bilgi.bitisTarih == null || bilgi.bitisTarih.isNull)
                hata += "Kayıt için bitiş tarihi girilmelidir!" + "<br>";

            if (bilgi.baslangicTarih.Oku() > bilgi.bitisTarih.Oku())
                hata = "Başlangıç bitiş tarihinden küçük olamaz!" + "<br>";

            //TimeSpan farkGun = bilgi.bitisTarih.Oku() - bilgi.baslangicTarih.Oku();
            //if (bilgi.gorevSuresi > farkGun.Days)
            //    hata += string.Format("Kayıt için görev süresi {0}'dan fazla olamaz!", farkGun.Days) + "<br>";

            //if (bilgi.gorevSuresi < 0)
            //    hata += "Kayıt için görev süresi negatif olamaz!" + "<br>";

            if (string.IsNullOrEmpty(bilgi.butceTertibi))
                hata += "Kayıt için bütçe tertibi girilmelidir!" + "<br>";

            if (bilgi.avansTutari < 0)
                hata += "Avans tutarı negatif olamaz!" + "<br>";

            //if (bilgi.adi == string.Empty)
            //    hata += TNS.HRC.HarcamaSurecleriResI.HRC00255 + "<br>";

            if (string.IsNullOrEmpty(hata))
            {
                GeciciGorevYollukTalepBaslik kriter = new GeciciGorevYollukTalepBaslik();
                kriter.yil = bilgi.yil;
                kriter.surecNo = bilgi.surecNo;

                ObjectArray l = Listele(null, kullanan, kriter);
                if (l.sonuc.islemSonuc && l.ObjeSayisi == 1)
                {
                    kriter = (GeciciGorevYollukTalepBaslik)l.objeler[0];
                    bilgi.avansOdemeEmri = kriter.avansOdemeEmri;

                    if (kriter.durum == 3)
                    {
                        hata += "Onaylanmış belgeler değiştirilemez!" + "<br>";
                    }
                }
            }
            return hata;
        }
    }
}

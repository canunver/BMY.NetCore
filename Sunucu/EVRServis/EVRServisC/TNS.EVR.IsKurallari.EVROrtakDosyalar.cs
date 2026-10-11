using System;
using System.Data;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System.IO;
using System.Collections.Generic;

namespace TNS.EVR.IsKurallari
{
    public class EVROrtakDosyalar
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalar yk, bool ayniIsimKontrolu = true)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            bool yetkisiVarmi = true;

            if (!yk.birimKlasoruMu)
                YetkisiVarmi(ve, kullanan, new int[] { yk.anaKod }, (int)ENUMOrtakDosyalarIzinler.EKLE);

            if (!yetkisiVarmi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Bu klasöre yeni klasör/dosya yaratma yetkiniz yok.<br/>";
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            try
            {
                TABOrtakDosyalar to = new TABOrtakDosyalar(ve);
                bool mevcut = false;

                if (yk.kod > 0) mevcut = to.KayitliMi(yk);

                if (ayniIsimKontrolu && !yk.birimKlasoruMu && (yk.kod > 0 || yk.anaKod > 0))
                {
                    int adVarmi = to.ListeleAdVarMi(yk.adi, yk.anaKod, yk.kod);
                    if (adVarmi > 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = "Aynı isimli (" + yk.adi + ") klasör/dosya olamaz.<br/>";
                        mevcut = true;
                    }
                }

                bool yeni = false;
                bool kullanimAlaniDoldu = false;
                OrtakDosyalarKullanimAlani ok = new OrtakDosyalarKullanimAlani();
                if (!mevcut)
                {
                    if (yk.boyut > 0)
                    {
                        ok = KullanimBilgisi(ve, kullanan);
                        float dosyaBoyutuMB = (float)(yk.boyut / 1000000);
                        if (ok.kullanilanAlan + dosyaBoyutuMB > ok.ayrilanAlan)
                        {
                            kullanimAlaniDoldu = true;
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = "Kullanımız için ayrılan " + ok.ayrilanAlan.ToString("#,###") + " MB alan dolduğu için kayıt yapılamıyor.<br/>";
                        }
                    }

                    if (!kullanimAlaniDoldu)
                    {
                        if (yk.kod == 0)
                        {
                            yk.baglantiAdresi = Guid.NewGuid().ToString().ToUpper();
                            if (yk.birimKlasoruMu)
                                yk.kod = yk.anaKod;
                            else
                                yk.kod = SayacNoAl(ve, "ORTAKDOSYAKOD");

                            sonuc = to.Kaydet(yk);
                            yeni = true;
                        }
                        else
                            sonuc = to.Guncelle(yk);
                    }

                    if (sonuc.islemSonuc)
                    {
                        int islemKod = yeni ? (int)ENUMEvrakOrtakAlanTarihceDurum.YENI : (int)ENUMEvrakOrtakAlanTarihceDurum.GUNCELLEME;
                        OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = yk.kod, islemKod = islemKod };
                        KaydetTarihce(ve, kullanan, tr);

                        //Üst klasör bilgilerini al ve kaydet
                        KaydetUstKlasorBilgileri(ve, to, kullanan, yk);
                        //********************************************************

                        ve.Commit();
                        sonuc.bilgiStr += "Kayıt işlemi başarıyla gerçekleşti";
                    }
                    else
                    {
                        if (!kullanimAlaniDoldu)
                            sonuc.hataStr += "Kayıt işlemi başarısız oldu.";

                        ve.Rollback();
                    }
                }
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();

            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc KaydetUstKlasorBilgileri(VeriErisim ve, TABOrtakDosyalar to, Kullanici kullanan, OrtakDosyalar yk)
        {
            ObjectArray yollar = DosyaYoluVerUst(ve, kullanan, yk.kod);
            foreach (OrtakDosyalar yol in yollar.objeler)
            {
                if (yk.seviye <= yol.seviye)
                {
                    yk.seviye = yol.seviye;
                    yk.yolAdi = yol.yolAdi;
                    yk.yolKod = yol.yolKod;
                }
            }
            return to.GuncelleAnaKod(yk);
        }

        public static Sonuc KaydetYetki(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();

            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            foreach (OrtakDosyalar yk in liste.objeler)
            {
                sonuc = to.SilYetki(yk.kod);
                if (sonuc.islemSonuc)
                {
                    foreach (OrtakDosyalarYetki ykYetki in yk.yetkiler)
                    {
                        ykYetki.kod = SayacNoAl(ve, "ORTAKDOSYAKODYETKI");
                        ykYetki.dosyaKod = yk.kod;
                        ykYetki.gorme = true;
                        sonuc = to.KaydetYetki(ykYetki);
                        if (!sonuc.islemSonuc) break;
                    }
                }

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                foreach (OrtakDosyalar yk in liste.objeler)
                {
                    foreach (OrtakDosyalarYetki ykYetki in yk.yetkiler)
                    {
                        OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = yk.kod, islemKod = (int)ENUMEvrakOrtakAlanTarihceDurum.YETKILENDIRME };
                        tr.aciklama = ykYetki.yetkiVerilenAd + ". ";
                        tr.aciklama += "Yetkiler; ";
                        tr.aciklama += "Görme:" + ykYetki.gorme + "; ";
                        tr.aciklama += "Klasör/Dosya Ekle:" + ykYetki.ekle + "; ";
                        tr.aciklama += "İndirme:" + ykYetki.indir + "; ";
                        tr.aciklama += "Kopyalama:" + ykYetki.kopyala + "; ";
                        tr.aciklama += "Taşıma:" + ykYetki.tasima + "; ";
                        tr.aciklama += "Silme:" + ykYetki.silme + "; ";
                        tr.aciklama += "e-posta Gönder:" + ykYetki.epostaGonder + "; ";
                        tr.aciklama += "Sürüm Yükleme:" + ykYetki.surum + "; ";
                        tr.aciklama += "Yeniden Yetkilendirme:" + ykYetki.yetki + "; ";
                        tr.aciklama += "Ad Değiştirme:" + ykYetki.adDegistir + "; ";
                        KaydetTarihce(ve, kullanan, tr);
                    }
                }
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalar bilgi, bool yetkiKontrolu)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            bool yetkisiVarmi = true;

            if (yetkiKontrolu && (bilgi.kod > 0 || bilgi.anaKod > 0))
            {
                int kod = bilgi.kod > 0 ? bilgi.kod : bilgi.anaKod;
                yetkisiVarmi = YetkisiVarmi(ve, kullanan, new int[] { kod }, (int)ENUMOrtakDosyalarIzinler.GORME);
            }

            if (!yetkisiVarmi)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = "Bu dosyayı görme yetkiniz yok.<br/>";
                ve.VeriErisimKapat();
                return liste;
            }

            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            IDataReader r = null;

            try
            {
                r = to.Listele(bilgi);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            if (r != null)
            {
                while (r.Read())
                {
                    OrtakDosyalar d = new OrtakDosyalar();

                    d.kod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.KOD, 0);
                    d.anaKod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.ANAKOD, 0);
                    d.adi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.ADI, "");
                    d.turu = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.TURU, 0);
                    d.boyut = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.BOYUT, 0.0);
                    d.fizikselDosyaID = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.FIZIKSELDOSYAID, "");
                    d.olusturan = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.OLUSTURAN, "");
                    d.olusturanAd = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.OLUSTURANAD, "");
                    d.olusturmaTarihi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.OLUSTURMATARIHI);
                    d.degistiren = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.DEGISTIREN, "");
                    d.degistirenAd = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.DEGISTIRENAD, "");
                    d.degistirmeTarihi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.DEGISTIRMETARIHI);
                    d.baglantiAdresi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.BAGLANTIADRESI, "");
                    d.yolKod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.YOLKOD, "");
                    d.yolAdi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.YOLADI, "");
                    d.dizin = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.YOLADI, "");
                    d.paylasimVarMi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.YOLADI + 1, 0) > 0;
                    d.notVarMi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlar.YOLADI + 2, 0) > 0;

                    liste.Ekle(d);
                }
            }

            r.Close();

            //Eğer paylaşılanları görecek ise 
            //dizin bilgisinde o kaydın üst klasör bilgileri yer alıyor (kendi kodu ile birlikte)
            //eğer üst klasör bilgisi listelecek ise kendinin listelenmemesi gerekiyor (paylaşım üst klasöre ve daha sonrada dosyaya paylaşım verildi ise)
            //client tarafında da dizin boş ise o satır gösterilmiyor
            //********************************************
            if (bilgi.gosterilecekDosyaTuru == (int)ENUMOrtakDosyalarGosterilecekDosyalar.PAYLASILAN && bilgi.olusturan != "")
            {
                foreach (OrtakDosyalar od2 in liste.objeler)
                {
                    string[] yollar = od2.yolKod.Split('/');
                    for (int i = 0; i < yollar.Length - 1; i++)//-1 dizin bilgisinde kendi koduda var. Onu almamak için
                    {
                        foreach (OrtakDosyalar od1 in liste.objeler)
                        {
                            if (od1.kod == OrtakFonksiyonlar.ConvertToInt(yollar[i], 0))
                            {
                                od2.dizin = "";
                                break;
                            }
                        }
                        if (od2.dizin == "") break;
                    }
                }
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc KaydetTarihce(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalarTarihce tr)
        {
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.YENI) tr.islemAd = "Yeni";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.GUNCELLEME) tr.islemAd = "Güncelleme";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.KOPYALA) tr.islemAd = "Kopyalama";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.TASI) tr.islemAd = "Taşıma";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.SIL) tr.islemAd = "Silme";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.ISIMDEGISTIR) tr.islemAd = "İsim Değiştirme";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.INDIRME) tr.islemAd = "İndirme";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.ONIZLEME) tr.islemAd = "Önizleme";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.NOTEKLEME) tr.islemAd = "Not Eklendi";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.NOTGUNCELLE) tr.islemAd = "Not Güncellendi";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.NOTSIL) tr.islemAd = "Not Silindi";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.SURUMYUKLE) tr.islemAd = "Sürüm Yükleme";
            if (tr.islemKod == (int)ENUMEvrakOrtakAlanTarihceDurum.YETKILENDIRME) tr.islemAd = "Yetkilendirme";

            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();

            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);
            tr.islemiYapan = kullanan.mernis;
            tr.islemiYapanAd = kullanan.ad + " " + kullanan.soyad;
            tr.islemTarihi = new TNSDateTime(DateTime.Now);
            sonuc = to.KaydetTarihce(tr);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray ListeleTarihce(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalarTarihce bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            IDataReader r = null;

            try
            {
                r = to.ListeleTarihce(bilgi.dosyaKod);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            if (r != null)
            {
                while (r.Read())
                {
                    OrtakDosyalarTarihce d = new OrtakDosyalarTarihce();

                    d.dosyaKod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarTarihce.DOSYAKOD, 0);
                    d.islemKod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarTarihce.ISLEMKOD, 0);
                    d.islemAd = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarTarihce.ISLEMAD, "");
                    d.islemTarihi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarTarihce.ISLEMTARIHI);
                    d.islemiYapan = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarTarihce.ISLEMIYAPAN, "");
                    d.islemiYapanAd = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarTarihce.ISLEMIYAPANAD, "");
                    d.aciklama = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarTarihce.ACIKLAMA, "");
                    liste.Ekle(d);
                }
            }

            r.Close();
            ve.VeriErisimKapat();
            return liste;
        }

        public static int ListeleBaglantiAdresindenKodBul(VeriErisim veriErisim, string adres)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            int sonuc = to.ListeleBaglantiAdresindenKodBul(adres);

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray ListeleYetki(VeriErisim veriErisim, Kullanici kullanan, int[] kodlar)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            IDataReader r = null;

            string ustKodlar = "";
            foreach (int kod in kodlar)
            {
                ObjectArray yollar = DosyaYoluVerUst(ve, kullanan, kod);
                foreach (OrtakDosyalar odd in yollar.objeler)
                {
                    //if (odd.kod == kod) continue;
                    if (ustKodlar != "") ustKodlar += ";";
                    ustKodlar += odd.kod.ToString();
                }
            }
            string[] kk = ustKodlar.Split(';');
            int[] kint = Array.ConvertAll(kk, s => int.Parse(s));

            //Sahibi bilgileri (Kendimin açtığı klasöre başkası dosya yüklerse onunda sahibi (yükleyen le birlikte) ben olmayım. Bu nedenle üst klasörleri ben yarattıysam onlara bakarım)
            //*********************************************************
            OrtakDosyalar od = new OrtakDosyalar();
            od.kodlar = kint;
            ObjectArray odlar = Listele(ve, kullanan, od, false);
            foreach (OrtakDosyalar odd in odlar.objeler)
            {
                OrtakDosyalarYetki d = new OrtakDosyalarYetki();
                d.dosyaKod = odd.kod;
                d.yetkiTur = (int)ENUMOrtakDosyalarYetkiTur.SAHIBI;
                d.yetkiVerilen = odd.olusturan;
                d.yetkiVeren = odd.olusturan;
                d.yetkiVerilenAd = odd.olusturanAd;
                d.yetkiVerenAd = odd.olusturanAd;
                d.gorme = true;
                d.ekle = true;
                d.epostaGonder = true;
                d.adDegistir = true;
                d.indir = true;
                d.kopyala = true;
                d.tasima = true;
                d.silme = true;
                d.surum = true;
                d.yetki = true;

                bool ekle = true;
                foreach (OrtakDosyalarYetki tt in liste.objeler)
                {
                    //Aynı kişi birden fazla kez eklenmesin
                    if (d.yetkiVeren == tt.yetkiVeren) { ekle = false; break; }
                }

                if (!ekle) continue;
                liste.Ekle(d);
            }
            //*********************************************************

            //Üst Klasörlerden gelen yetkiler
            //*********************************************************
            if (ustKodlar != "")
            {
                r = to.ListeleYetki(kint);

                if (r != null)
                {
                    while (r.Read())
                    {
                        OrtakDosyalarYetki d = new OrtakDosyalarYetki();

                        d.dosyaKod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.DOSYAKOD, 0);
                        d.yetkiTur = (int)ENUMOrtakDosyalarYetkiTur.DEVIR;
                        d.yetkiVerilen = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.YETKIVERILEN, "");
                        d.yetkiVeren = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.YETKIVEREN, "");
                        d.yetkiVerilenAd = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.YETKIVERILENAD, "");
                        d.yetkiVerenAd = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.YETKIVERENAD, "");
                        d.gorme = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.GORME, 0) == 1;
                        d.ekle = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.EKLE, 0) == 1;
                        d.epostaGonder = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.EPOSTAGONDER, 0) == 1;
                        d.adDegistir = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.ADDEGISTIR, 0) == 1;
                        d.indir = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.INDIR, 0) == 1;
                        d.kopyala = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.KOPYALA, 0) == 1;
                        d.tasima = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.TASIMA, 0) == 1;
                        d.silme = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.SILME, 0) == 1;
                        d.surum = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.SURUM, 0) == 1;
                        d.yetki = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.YETKI, 0) == 1;

                        bool ekle = true;
                        foreach (OrtakDosyalarYetki tt in liste.objeler)
                        {
                            //Aynı kişi birden fazla kez eklenmesin (sahibi ise)
                            if (d.yetkiVerilen == tt.yetkiVeren && tt.yetkiTur == (int)ENUMOrtakDosyalarYetkiTur.SAHIBI) { ekle = false; break; }
                        }

                        //kint içinde üst klasörler ile birlikte dosyaların kendiside mevcut. Burası sadece üst klasörleri kontrol etmeli
                        foreach (int kkk in kodlar)
                        {
                            if (kkk == d.dosyaKod)
                            {
                                ekle = false; break;
                            }
                        }

                        if (!ekle) continue;
                        liste.Ekle(d);
                    }
                }
                r.Close();
            }

            //*********************************************************


            //Paylaşılan bilgileri
            //*********************************************************
            r = to.ListeleYetki(kodlar);

            if (r != null)
            {
                while (r.Read())
                {
                    OrtakDosyalarYetki d = new OrtakDosyalarYetki();
                    d.dosyaKod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.DOSYAKOD, 0);
                    d.yetkiTur = (int)ENUMOrtakDosyalarYetkiTur.PAYLASILAN;
                    d.yetkiVerilen = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.YETKIVERILEN, "");
                    d.yetkiVeren = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.YETKIVEREN, "");
                    d.yetkiVerilenAd = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.YETKIVERILENAD, "");
                    d.yetkiVerenAd = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.YETKIVERENAD, "");
                    d.gorme = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.GORME, 0) == 1;
                    d.ekle = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.EKLE, 0) == 1;
                    d.epostaGonder = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.EPOSTAGONDER, 0) == 1;
                    d.adDegistir = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.ADDEGISTIR, 0) == 1;
                    d.indir = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.INDIR, 0) == 1;
                    d.kopyala = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.KOPYALA, 0) == 1;
                    d.tasima = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.TASIMA, 0) == 1;
                    d.silme = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.SILME, 0) == 1;
                    d.surum = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.SURUM, 0) == 1;
                    d.yetki = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarYetki.YETKI, 0) == 1;

                    bool ekle = true;
                    foreach (OrtakDosyalarYetki tt in liste.objeler)
                    {
                        //Aynı kişi birden fazla kez eklenmesin (sahibi ise)
                        if (d.yetkiVerilen == tt.yetkiVeren && tt.yetkiTur == (int)ENUMOrtakDosyalarYetkiTur.SAHIBI) { ekle = false; break; }
                        if (d.yetkiVerilen == tt.yetkiVeren && tt.yetkiTur == (int)ENUMOrtakDosyalarYetkiTur.DEVIR) { ekle = false; break; }
                    }

                    if (!ekle) continue;
                    liste.Ekle(d);
                }
            }
            r.Close();
            //*********************************************************

            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc BirimKlasoruKontrol(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalarBirim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            lock (bilgi.birimKod)
            {
                TABOrtakDosyalar to = new TABOrtakDosyalar(ve);
                IDataReader r = to.BirimKayitliMi(bilgi);

                if (r != null)
                {
                    while (r.Read())
                    {
                        int kod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarBirim.ANAKOD, 0);
                        sonuc.anahtar = kod.ToString();
                    }
                }
                r.Close();

                if (sonuc.anahtar == "")
                {
                    ve.BeginTransaction();

                    bilgi.kod = SayacNoAl(ve, "ORTAKDOSYAKOD");
                    sonuc = to.BirimKaydet(bilgi);

                    if (sonuc.islemSonuc)
                    {
                        OrtakDosyalar od = new OrtakDosyalar();
                        od.kod = 0;
                        od.anaKod = bilgi.kod;
                        od.adi = "Ana Klasör";
                        od.boyut = 0;
                        od.turu = (int)ENUMOrtakDosyalarDosyaTipleri.KLASOR;
                        od.olusturan = kullanan.mernis;
                        od.olusturanAd = kullanan.ad + " " + kullanan.soyad;
                        od.olusturmaTarihi = new TNSDateTime(DateTime.Now);
                        od.degistiren = kullanan.mernis;
                        od.degistirenAd = kullanan.ad + " " + kullanan.soyad;
                        od.degistirmeTarihi = od.olusturmaTarihi;
                        od.birimKlasoruMu = true;

                        sonuc = Kaydet(ve, kullanan, od);
                        if (sonuc.islemSonuc)
                            sonuc.anahtar = bilgi.kod.ToString();
                    }

                    if (sonuc.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                }

                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        public static ObjectArray DosyaYoluVerUst(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            IDataReader r = to.DosyaYoluVerUst(kod);

            if (r != null)
            {
                while (r.Read())
                {
                    OrtakDosyalar d = new OrtakDosyalar();
                    d.kod = VeriErisim.DegerAl(r, 0, 0);
                    d.adi = VeriErisim.DegerAl(r, 1, "");
                    d.anaKod = VeriErisim.DegerAl(r, 2, 0);
                    d.turu = VeriErisim.DegerAl(r, 3, 0);
                    d.boyut = VeriErisim.DegerAl(r, 4, 0);
                    d.fizikselDosyaID = VeriErisim.DegerAl(r, 5, "");
                    d.olusturan = VeriErisim.DegerAl(r, 6, "");
                    d.olusturanAd = VeriErisim.DegerAl(r, 7, "");
                    d.olusturmaTarihi = VeriErisim.DegerAl(r, 8);
                    d.degistiren = VeriErisim.DegerAl(r, 9, "");
                    d.degistirenAd = VeriErisim.DegerAl(r, 10, "");
                    d.degistirmeTarihi = VeriErisim.DegerAl(r, 11);
                    //d.dizin = VeriErisim.DegerAl(r, 13, "");
                    d.yolAdi = VeriErisim.DegerAl(r, 12, "");
                    d.seviye = VeriErisim.DegerAl(r, 13, 0);
                    d.yolKod = VeriErisim.DegerAl(r, 14, "");
                    liste.Ekle(d);
                }
            }

            r.Close();
            ve.VeriErisimKapat();
            return liste;
        }

        public static ObjectArray DosyaYoluVerAlt(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            IDataReader r = to.DosyaYoluVerAlt(kod);

            if (r != null)
            {
                while (r.Read())
                {
                    OrtakDosyalar d = new OrtakDosyalar();
                    d.kod = VeriErisim.DegerAl(r, 0, 0);
                    d.adi = VeriErisim.DegerAl(r, 1, "");
                    d.anaKod = VeriErisim.DegerAl(r, 2, 0);
                    d.turu = VeriErisim.DegerAl(r, 3, 0);
                    d.boyut = VeriErisim.DegerAl(r, 4, 0);
                    d.fizikselDosyaID = VeriErisim.DegerAl(r, 5, "");
                    d.olusturan = VeriErisim.DegerAl(r, 6, "");
                    d.olusturanAd = VeriErisim.DegerAl(r, 7, "");
                    d.olusturmaTarihi = VeriErisim.DegerAl(r, 8);
                    d.degistiren = VeriErisim.DegerAl(r, 9, "");
                    d.degistirenAd = VeriErisim.DegerAl(r, 10, "");
                    d.degistirmeTarihi = VeriErisim.DegerAl(r, 11);
                    //d.dizin = VeriErisim.DegerAl(r, 13, "");
                    d.yolAdi = VeriErisim.DegerAl(r, 12, "");
                    d.seviye = VeriErisim.DegerAl(r, 13, 0);
                    d.yolKod = VeriErisim.DegerAl(r, 14, "");
                    liste.Ekle(d);
                }
            }

            r.Close();
            ve.VeriErisimKapat();
            return liste;
        }

        public static ObjectArray DosyaYoluVerSurumAlt(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            IDataReader r = to.DosyaYoluVerAlt(kod);

            if (r != null)
            {
                while (r.Read())
                {
                    OrtakDosyalar d = new OrtakDosyalar();
                    d.kod = VeriErisim.DegerAl(r, 0, 0);
                    d.adi = VeriErisim.DegerAl(r, 1, "");
                    d.anaKod = VeriErisim.DegerAl(r, 2, 0);
                    d.turu = VeriErisim.DegerAl(r, 3, 0);
                    d.boyut = VeriErisim.DegerAl(r, 4, 0);
                    d.fizikselDosyaID = VeriErisim.DegerAl(r, 5, "");
                    d.olusturan = VeriErisim.DegerAl(r, 6, "");
                    d.olusturanAd = VeriErisim.DegerAl(r, 7, "");
                    d.olusturmaTarihi = VeriErisim.DegerAl(r, 8);
                    d.degistiren = VeriErisim.DegerAl(r, 9, "");
                    d.degistirenAd = VeriErisim.DegerAl(r, 10, "");
                    d.degistirmeTarihi = VeriErisim.DegerAl(r, 11);
                    //d.dizin = VeriErisim.DegerAl(r, 13, "");
                    d.yolAdi = VeriErisim.DegerAl(r, 12, "");
                    d.seviye = VeriErisim.DegerAl(r, 13, 0);
                    d.yolKod = VeriErisim.DegerAl(r, 14, "");
                    liste.Ekle(d);
                }
            }

            r.Close();
            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc Yapistir(VeriErisim veriErisim, Kullanici kullanan, string islemTur, int nereye, string[] islemYapilacakKayitKodlari, string klasorYolu)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            int islemKod = 0;
            string islemAdi = "";
            ve.BeginTransaction();

            ObjectArray liste = new ObjectArray();
            OrtakDosyalar yk = new OrtakDosyalar();
            ObjectArray kopyalanacakFizikselDosyalar = new ObjectArray();

            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            foreach (string islemYapilacakKayitKod in islemYapilacakKayitKodlari)
            {
                string olusturan = "";
                liste = Listele(ve, kullanan, new OrtakDosyalar() { kod = OrtakFonksiyonlar.ConvertToInt(islemYapilacakKayitKod, 0) }, true);
                foreach (OrtakDosyalar od in liste.objeler)
                {
                    yk.kod = od.kod;
                    yk.anaKod = od.anaKod;
                    yk.adi = od.adi;
                    yk.turu = od.turu;
                    yk.boyut = od.boyut;
                    yk.fizikselDosyaID = od.fizikselDosyaID;

                    olusturan = od.olusturan;
                }

                if (islemTur == "TASI")
                {
                    islemKod = (int)ENUMEvrakOrtakAlanTarihceDurum.TASI;
                    islemAdi = "Taşıma";

                    //Klasör alt klasörüne taşınamaz
                    bool altKlasoreTasiniyor = false;
                    if (yk.turu == (int)ENUMOrtakDosyalarDosyaTipleri.KLASOR)
                    {
                        liste = DosyaYoluVerAlt(ve, kullanan, OrtakFonksiyonlar.ConvertToInt(islemYapilacakKayitKod, 0));

                        foreach (OrtakDosyalar od in liste.objeler)
                        {
                            if (od.kod == nereye)
                            {
                                altKlasoreTasiniyor = true;
                                break;
                            }
                        }

                        if (altKlasoreTasiniyor)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr += "Taşınmak istenilen klasör, alt klasörüne taşınamaz.<br>";
                            break;
                        }
                    }

                    OrtakDosyalar yeni = new OrtakDosyalar();
                    yeni.kod = yk.kod;
                    yeni.adi = yk.adi;
                    yeni.anaKod = nereye;

                    if (yeni.anaKod > 0)
                    {
                        int adVarmi = to.ListeleAdVarMi(yeni.adi, yeni.anaKod, yeni.kod);
                        if (adVarmi > 0)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = "Aynı isimli (" + yeni.adi + ") klasör/dosya olamaz.<br/>";
                            break;
                        }
                    }

                    sonuc = to.GuncelleAnaKod(yeni);


                    ObjectArray altliste = DosyaYoluVerAlt(ve, kullanan, yeni.kod);
                    foreach (OrtakDosyalar al in altliste.objeler)
                    {
                        //Üst klasör bilgilerini al ve güncelle
                        KaydetUstKlasorBilgileri(ve, to, kullanan, al);
                        //********************************************************
                    }

                    if (sonuc.islemSonuc)
                    {
                        //Tarihçe Kaydet
                        //******************************************
                        OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = yk.kod, islemKod = islemKod };
                        KaydetTarihce(ve, kullanan, tr);
                    }
                }
                else if (islemTur == "KOPYALA")
                {
                    islemKod = (int)ENUMEvrakOrtakAlanTarihceDurum.KOPYALA;
                    islemAdi = "Kopyalama";

                    liste = DosyaYoluVerAlt(ve, kullanan, OrtakFonksiyonlar.ConvertToInt(islemYapilacakKayitKod, 0));
                    ObjectArray yeniListe = new ObjectArray();
                    foreach (OrtakDosyalar od in liste.objeler)
                    {
                        OrtakDosyalar yeni = new OrtakDosyalar();

                        yeni.anaKod = nereye;
                        yeni.adi = od.adi;
                        yeni.turu = od.turu;
                        yeni.boyut = od.boyut;
                        yeni.olusturan = kullanan.mernis;
                        yeni.olusturanAd = kullanan.ad + " " + kullanan.soyad;
                        yeni.olusturmaTarihi = new TNSDateTime(DateTime.Now);
                        yeni.degistiren = kullanan.mernis;
                        yeni.degistirenAd = kullanan.ad + " " + kullanan.soyad;
                        yeni.degistirmeTarihi = new TNSDateTime(DateTime.Now);
                        if (od.turu == (int)ENUMOrtakDosyalarDosyaTipleri.DOSYA) yeni.fizikselDosyaID = Guid.NewGuid().ToString();

                        yeni.eskiKod = od.kod;
                        yeni.eskiAnaKod = od.anaKod;
                        yeni.eskiFizikselDosyaID = od.fizikselDosyaID;

                        yeniListe.objeler.Add(yeni);
                    }

                    //Satırları kayıt edelim ki kod alanları doldurulsun. Sonra anakodları update edeceğiz.
                    foreach (OrtakDosyalar yeni in yeniListe.objeler)
                    {
                        sonuc = Kaydet(ve, kullanan, yeni, false);
                    }

                    //Klasör kopyalanıyorsa liste karışık sırada geldiği için anakodlarının set edilmesi için
                    foreach (OrtakDosyalar yeni1 in yeniListe.objeler)
                    {
                        int eskiAnaKod = yeni1.eskiAnaKod;
                        foreach (OrtakDosyalar yeni2 in yeniListe.objeler)
                        {
                            if (eskiAnaKod == yeni2.eskiKod)
                                yeni1.anaKod = yeni2.kod;
                        }
                        sonuc = to.GuncelleAnaKod(yeni1);
                    }

                    foreach (OrtakDosyalar yeni in yeniListe.objeler)
                    {
                        if (yeni.anaKod > 0)
                        {
                            int adVarmi = to.ListeleAdVarMi(yeni.adi, yeni.anaKod, yeni.kod);
                            if (adVarmi > 0)
                            {
                                sonuc.islemSonuc = false;
                                sonuc.hataStr = "Aynı isimli (" + yeni.adi + ") klasör/dosya olamaz.<br/>";
                                ve.Rollback();
                                ve.VeriErisimKapat();
                                return sonuc;
                            }
                        }
                    }

                    foreach (OrtakDosyalar yeni in yeniListe.objeler)
                    {
                        if (sonuc.islemSonuc)
                        {
                            if (!string.IsNullOrEmpty(yeni.eskiFizikselDosyaID))
                            {
                                OrtakDosyalar oo = new OrtakDosyalar();
                                oo.fizikselDosyaID = yeni.fizikselDosyaID;//yeni fiziksel dosya ID
                                oo.adi = yeni.eskiFizikselDosyaID;//eski fiziksel dosya ID
                                kopyalanacakFizikselDosyalar.objeler.Add(oo);
                            }

                            if (sonuc.islemSonuc)
                            {
                                //Tarihçe Kaydet
                                //******************************************
                                OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = yeni.eskiKod, islemKod = islemKod };
                                KaydetTarihce(ve, kullanan, tr);
                                tr = new OrtakDosyalarTarihce() { dosyaKod = yeni.kod, islemKod = islemKod };
                                KaydetTarihce(ve, kullanan, tr);
                            }
                        }
                        else
                            break;
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                try
                {
                    foreach (OrtakDosyalar oo in kopyalanacakFizikselDosyalar.objeler)
                    {
                        File.Copy(Path.Combine(klasorYolu, oo.adi), Path.Combine(klasorYolu, oo.fizikselDosyaID));
                    }
                }
                catch { }
            }

            if (sonuc.islemSonuc)
            {

                ve.Commit();
                sonuc.bilgiStr += islemAdi + " işlemi başarıyla gerçekleşti";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr += islemAdi + " işlemi başarısız oldu.";
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int[] islemYapilacakKayitKodlari, string klasorYolu)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            bool yetkisiVarmi = YetkisiVarmi(ve, kullanan, islemYapilacakKayitKodlari, (int)ENUMOrtakDosyalarIzinler.SILME);
            if (!yetkisiVarmi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "İşaretli klasör/dosyaları silme yetkiniz yok.<br/>";
                ve.VeriErisimKapat();
                return sonuc;
            }

            ObjectArray liste = new ObjectArray();
            string silinecekFizikselDosyalar = "";

            ve.BeginTransaction();

            try
            {
                TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

                foreach (int islemYapilacakKayitKod in islemYapilacakKayitKodlari)
                {
                    if (islemYapilacakKayitKod == 0) continue;

                    //Varsa Alt Klasör ve dosyalarını silelim, yoksa sadece dosya silinecek
                    liste = DosyaYoluVerAlt(ve, kullanan, OrtakFonksiyonlar.ConvertToInt(islemYapilacakKayitKod, 0));
                    foreach (OrtakDosyalar od in liste.objeler)
                    {
                        sonuc = to.Sil(od.kod);
                        if (sonuc.islemSonuc)
                            sonuc = to.SilYetki(od.kod);

                        if (sonuc.islemSonuc && od.turu == (int)ENUMOrtakDosyalarDosyaTipleri.DOSYA)//Dosya ise fiziksel dosyayı sil
                        {
                            if (silinecekFizikselDosyalar != "") silinecekFizikselDosyalar += ";";
                            silinecekFizikselDosyalar += od.fizikselDosyaID;
                        }

                        if (sonuc.islemSonuc)
                        {
                            OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = od.kod, islemKod = (int)ENUMEvrakOrtakAlanTarihceDurum.SIL };
                            KaydetTarihce(ve, kullanan, tr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }

            if (sonuc.islemSonuc)
            {
                //Eğer herhangi bir hata yok ise fiziksel dosyalar silinebilir
                string[] dosyalar = silinecekFizikselDosyalar.Split(';');
                foreach (string sd in dosyalar)
                {
                    try
                    {
                        if (sd.Trim() == "") continue;
                        File.Delete(Path.Combine(klasorYolu, sd));
                    }
                    catch { }
                }

                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarılı oldu.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme işlemi başarısız oldu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc AdDegistir(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalar bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            bool yetkisiVarmi = YetkisiVarmi(ve, kullanan, new int[] { bilgi.kod }, (int)ENUMOrtakDosyalarIzinler.ADDEGISTIR);
            if (!yetkisiVarmi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Bu klasör/dosya için ad değiştirme yetkiniz yok.<br/>";
                ve.VeriErisimKapat();
                return sonuc;
            }

            string eskiAd = "";

            ve.BeginTransaction();

            ObjectArray liste = new ObjectArray();
            OrtakDosyalar yk = new OrtakDosyalar();

            try
            {
                TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

                liste = Listele(null, kullanan, bilgi, true);
                foreach (OrtakDosyalar od in liste.objeler)
                {
                    yk.kod = od.kod;
                    yk.anaKod = od.anaKod;
                    yk.adi = od.adi;
                }
                eskiAd = yk.adi;
                yk.adi = bilgi.adi;

                if (yk.kod > 0)
                {
                    int adVarmi = to.ListeleAdVarMi(yk.adi, yk.anaKod, yk.kod);
                    if (adVarmi > 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = "Aynı isimli (" + yk.adi + ") klasör/dosya olamaz.<br/>";
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }

                if (yk.kod > 0)
                    sonuc = to.GuncelleAd(yk);
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Ad değiştirilmek istenilen dosya bulunamadı";
                }

                if (sonuc.islemSonuc)
                {
                    OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = yk.kod, islemKod = (int)ENUMEvrakOrtakAlanTarihceDurum.ISIMDEGISTIR };
                    tr.aciklama = "Eski Adı:" + eskiAd;
                    KaydetTarihce(ve, kullanan, tr);

                    //Üst klasör bilgilerini al ve kaydet
                    KaydetUstKlasorBilgileri(ve, to, kullanan, yk);
                    //********************************************************

                    ve.Commit();
                    sonuc.bilgiStr += "İsim değiştirme işlemi başarıyla gerçekleşti";
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr += "İsim değiştirme işlemi başarısız oldu.";
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        #region "NOTBILGISI"
        public static Sonuc NotKaydet(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalarNot bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            //Notu daha önce kayıt eden kişimi değiştiriyor kontrolü
            if (bilgi.kod > 0)
            {
                ObjectArray notlar = NotListele(ve, kullanan, new OrtakDosyalarNot() { kod = bilgi.kod, dosyaKod = bilgi.dosyaKod });
                foreach (OrtakDosyalarNot n in notlar.objeler)
                {
                    if (n.ekleyenKisi != kullanan.mernis)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = "Bu notu kayıt eden kişi (" + n.ekleyenKisiAd + ") güncellemebilir.";
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }
            }

            ve.BeginTransaction();

            try
            {
                TABOrtakDosyalar to = new TABOrtakDosyalar(ve);
                bool yeni = false;

                bilgi.ekleyenKisi = kullanan.mernis;
                bilgi.ekleyenKisiAd = kullanan.ad + " " + kullanan.soyad;
                bilgi.eklemeTarihi = new TNSDateTime(DateTime.Now);

                if (bilgi.kod == 0)
                {
                    bilgi.kod = SayacNoAl(ve, "ORTAKDOSYANOTKOD");
                    sonuc = to.KaydetNot(bilgi);
                    yeni = true;
                }
                else
                    sonuc = to.GuncelleNot(bilgi);

                if (sonuc.islemSonuc)
                {
                    int islemKod = yeni ? (int)ENUMEvrakOrtakAlanTarihceDurum.NOTEKLEME : (int)ENUMEvrakOrtakAlanTarihceDurum.NOTGUNCELLE;
                    OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = bilgi.dosyaKod, islemKod = islemKod };
                    KaydetTarihce(ve, kullanan, tr);

                    ve.Commit();
                    sonuc.bilgiStr += "Kayıt işlemi başarıyla gerçekleşti";
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr += "Kayıt işlemi başarısız oldu.";
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();

            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray NotListele(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalarNot bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            IDataReader r = null;

            try
            {
                r = to.ListeleNot(bilgi.dosyaKod, bilgi.kod);
            }

            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            if (r != null)
            {
                while (r.Read())
                {
                    OrtakDosyalarNot n = new OrtakDosyalarNot();
                    n.kod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarNot.KOD, 0);
                    n.dosyaKod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarNot.DOSYAKOD, 0);
                    n.ekleyenKisi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarNot.EKLEYENKISI, "");
                    n.ekleyenKisiAd = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarNot.EKLEYENKISIAD, "");
                    n.eklemeTarihi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarNot.EKLEMETARIHI);
                    n.notBilgisi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarNot.NOTBILGISI, "");
                    liste.Ekle(n);
                }
            }

            r.Close();

            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc NotSil(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalarNot bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            //Notu daha önce kayıt eden kişimi değiştiriyor kontrolü
            if (bilgi.kod > 0)
            {
                ObjectArray notlar = NotListele(ve, kullanan, new OrtakDosyalarNot() { kod = bilgi.kod, dosyaKod = bilgi.dosyaKod });
                foreach (OrtakDosyalarNot n in notlar.objeler)
                {
                    if (n.ekleyenKisi != kullanan.mernis)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = "Bu notu kayıt eden kişi (" + n.ekleyenKisiAd + ") silebilir.";
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }
            }

            ve.BeginTransaction();

            try
            {
                TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

                sonuc = to.SilNot(bilgi.kod);

                if (sonuc.islemSonuc)
                {
                    if (sonuc.islemSonuc)
                    {
                        OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = bilgi.dosyaKod, islemKod = (int)ENUMEvrakOrtakAlanTarihceDurum.NOTSIL };
                        KaydetTarihce(ve, kullanan, tr);
                    }
                    ve.Commit();
                    sonuc.bilgiStr = "Silme işlemi başarılı oldu..";
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = "Silme işlemi başarısız oldu.." + "<br>" + sonuc.hataStr;
                }
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        #endregion

        #region "SURUMLER"
        public static Sonuc SurumKaydet(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalarSurumler bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();

            try
            {
                TABOrtakDosyalar to = new TABOrtakDosyalar(ve);
                bool yeni = false;

                if (bilgi.kod == 0)
                {
                    bilgi.kod = SayacNoAl(ve, "ORTAKDOSYASURUMKOD");
                    sonuc = to.KaydetSurum(bilgi);
                    yeni = true;
                }
                //else
                //    sonuc = to.GuncelleSurum(bilgi);

                if (sonuc.islemSonuc)
                {
                    OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = bilgi.dosyaKod, islemKod = (int)ENUMEvrakOrtakAlanTarihceDurum.SURUMYUKLE };
                    KaydetTarihce(ve, kullanan, tr);

                    ve.Commit();
                    sonuc.bilgiStr += "Kayıt işlemi başarıyla gerçekleşti";
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr += "Kayıt işlemi başarısız oldu.";
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();

            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray SurumListele(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalarSurumler bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            IDataReader r = null;

            try
            {
                r = to.ListeleSurum(bilgi.dosyaKod, bilgi.kod);
            }

            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            if (r != null)
            {
                while (r.Read())
                {
                    OrtakDosyalarSurumler n = new OrtakDosyalarSurumler();
                    n.kod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarSurum.KOD, 0);
                    n.dosyaKod = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarSurum.DOSYAKOD, 0);
                    n.ekleyenKisi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarSurum.EKLEYENKISI, "");
                    n.ekleyenKisiAd = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarSurum.EKLEYENKISIAD, "");
                    n.eklemeTarihi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarSurum.EKLEMETARIHI);
                    n.boyut = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarSurum.BOYUT, 0);
                    n.fizikselDosyaID = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarSurum.FIZIKSELDOSYAID, "");
                    n.adi = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarSurum.ADI, "");
                    n.aciklama = VeriErisim.DegerAl(r, (int)TABOrtakDosyalar.ENUMAlanlarSurum.ACIKLAMA, "");
                    liste.Ekle(n);
                }
            }

            r.Close();

            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc SurumSil(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalarSurumler bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ////Notu daha önce kayıt eden kişimi değiştiriyor kontrolü
            //if (bilgi.kod > 0)
            //{
            //    ObjectArray notlar = NotListele(ve, kullanan, new OrtakDosyalarNot() { kod = bilgi.kod, dosyaKod = bilgi.dosyaKod });
            //    foreach (OrtakDosyalarNot n in notlar.objeler)
            //    {
            //        if (n.ekleyenKisi != kullanan.mernis)
            //        {
            //            sonuc.islemSonuc = false;
            //            sonuc.hataStr = "Bu notu kayıt eden kişi (" + n.ekleyenKisiAd + ") silebilir.";
            //            ve.VeriErisimKapat();
            //            return sonuc;
            //        }
            //    }
            //}

            ve.BeginTransaction();

            try
            {
                //TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

                //sonuc = to.SilNot(bilgi.kod);

                //if (sonuc.islemSonuc)
                //{
                //    if (sonuc.islemSonuc)
                //    {
                //        OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = bilgi.dosyaKod, islemKod = (int)ENUMEvrakOrtakAlanTarihceDurum.NOTSIL };
                //        KaydetTarihce(ve, kullanan, tr);
                //    }
                //    ve.Commit();
                //    sonuc.bilgiStr = "Silme işlemi başarılı oldu..";
                //}
                //else
                //{
                //    ve.Rollback();
                //    sonuc.hataStr = "Silme işlemi başarısız oldu.." + "<br>" + sonuc.hataStr;
                //}
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc SurumuOrtakDosyalaraKaydet(VeriErisim veriErisim, Kullanici kullanan, OrtakDosyalar bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            //bool yetkisiVarmi = YetkisiVarmi(ve, kullanan, new int[] { bilgi.kod }, (int)ENUMOrtakDosyalarIzinler.ADDEGISTIR);
            //if (!yetkisiVarmi)
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = "Bu klasör/dosya için sürüm ekleme yetkiniz yok.<br/>";
            //    ve.VeriErisimKapat();
            //    return sonuc;
            //}

            string eskiAd = "";

            ve.BeginTransaction();

            ObjectArray liste = new ObjectArray();
            OrtakDosyalar yk = new OrtakDosyalar();

            try
            {
                TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

                liste = Listele(null, kullanan, bilgi, true);
                foreach (OrtakDosyalar od in liste.objeler)
                {
                    yk.kod = od.kod;
                    yk.anaKod = od.anaKod;
                    yk.adi = od.adi;
                }
                eskiAd = yk.adi;
                yk.adi = bilgi.adi;

                if (yk.kod > 0)
                {
                    int adVarmi = to.ListeleAdVarMi(bilgi.adi, bilgi.anaKod, bilgi.kod);
                    if (adVarmi > 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = "Aynı isimli (" + yk.adi + ") klasör/dosya olamaz.<br/>";
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }

                if (bilgi.kod > 0)
                    sonuc = to.Guncelle(bilgi);
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Sürüm yüklenmek istenilen dosya bulunamadı";
                }

                if (sonuc.islemSonuc)
                {
                    OrtakDosyalarTarihce tr = new OrtakDosyalarTarihce() { dosyaKod = bilgi.kod, islemKod = (int)ENUMEvrakOrtakAlanTarihceDurum.SURUMYUKLE };
                    tr.aciklama = eskiAd + " için sürüm yüklendi";
                    KaydetTarihce(ve, kullanan, tr);

                    //Üst klasör bilgilerini al ve kaydet
                    KaydetUstKlasorBilgileri(ve, to, kullanan, yk);
                    //********************************************************

                    ve.Commit();
                    sonuc.bilgiStr += "Sürüm yükleme işlemi başarıyla gerçekleşti";
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr += "Sürüm yükleme işlemi başarısız oldu.";
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        #endregion

        public static OrtakDosyalarKullanimAlani KullanimBilgisi(VeriErisim ve, Kullanici kullanan)
        {
            OrtakDosyalarKullanimAlani ok = new OrtakDosyalarKullanimAlani();
            ok.ayrilanAlan = 5000f;//5GB

            //Kullanılan alanı bul
            //********************************
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABOrtakDosyalar to = new TABOrtakDosyalar(ve);

            double boyut = to.ListeleKullanimBilgisi(kullanan.mernis);
            boyut = boyut / 1000000;//gelen byte değeri MB ye çevir

            try
            {
                ok.kullanilanAlan = (float)boyut;
                ok.kullanimYuzdesi = ok.kullanilanAlan / ok.ayrilanAlan;
            }
            catch { ok.kullanimYuzdesi = 0f; }
            //********************************
            ve.VeriErisimKapat();

            return ok;
        }

        public static bool YetkisiVarmi(VeriErisim veriErisim, Kullanici kullanan, int[] dosyaKodlar, int yetkiTuru)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            bool yetkisiVarmi = true;
            foreach (int kod in dosyaKodlar)
            {
                yetkisiVarmi = YetkisiVarmi(ve, kullanan, kod, yetkiTuru);
                //Birden fazla dosya gönderilmiş ise ilk yetki olmayan durumda yetki yok döndüğrülecek. 
                //Toplu dosya işlemlerinde bir dosyanın yetkisinin olmaması diğerlerininde (yetkisinin olsa bile) olmayacağı anlamını taşır. Melih
                if (!yetkisiVarmi) break;
            }

            ve.VeriErisimKapat();

            return yetkisiVarmi;
        }

        public static bool YetkisiVarmi(VeriErisim veriErisim, Kullanici kullanan, int dosyaKod, int yetkiTuru)
        {
            bool sonuc = false;
            ObjectArray yetkiler = EVROrtakDosyalar.ListeleYetki(veriErisim, kullanan, new int[] { dosyaKod });

            string[] yetkiliOlduguBirimler = TNS.EVR.Yetki.KullaniciEvrakYetkiliBirimleri(kullanan, ENUMOkumaYazma.OKUMA).Split(';');

            foreach (OrtakDosyalarYetki y in yetkiler.objeler)
            {
                bool kontrolEt = false;
                if (y.yetkiVerilen == kullanan.mernis) kontrolEt = true;
                foreach (string yy in yetkiliOlduguBirimler)
                {
                    if (string.IsNullOrEmpty(yy)) continue;
                    if (y.yetkiVerilen == yy) kontrolEt = true;
                }

                if (kontrolEt && yetkiTuru == (int)ENUMOrtakDosyalarIzinler.GORME && y.gorme) { sonuc = true; break; }
                if (kontrolEt && yetkiTuru == (int)ENUMOrtakDosyalarIzinler.EPOSTAGONDER && y.epostaGonder) { sonuc = true; break; }
                if (kontrolEt && yetkiTuru == (int)ENUMOrtakDosyalarIzinler.INDIR && y.indir) { sonuc = true; break; }
                if (kontrolEt && yetkiTuru == (int)ENUMOrtakDosyalarIzinler.EKLE && y.ekle) { sonuc = true; break; }
                if (kontrolEt && yetkiTuru == (int)ENUMOrtakDosyalarIzinler.ADDEGISTIR && y.adDegistir) { sonuc = true; break; }
                if (kontrolEt && yetkiTuru == (int)ENUMOrtakDosyalarIzinler.KOPYALA && y.kopyala) { sonuc = true; break; }
                if (kontrolEt && yetkiTuru == (int)ENUMOrtakDosyalarIzinler.TASIMA && y.tasima) { sonuc = true; break; }
                if (kontrolEt && yetkiTuru == (int)ENUMOrtakDosyalarIzinler.SILME && y.silme) { sonuc = true; break; }
                if (kontrolEt && yetkiTuru == (int)ENUMOrtakDosyalarIzinler.SURUMYUKLE && y.surum) { sonuc = true; break; }
                if (kontrolEt && yetkiTuru == (int)ENUMOrtakDosyalarIzinler.YETKILENDIRME && y.yetki) { sonuc = true; break; }
            }

            return sonuc;
        }

        public static int SayacNoAl(VeriErisim ve, string sayacKod)
        {
            TABGenel tabGenel = new TABGenel(ve);
            return OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "000000000"), 0);
        }
    }
}

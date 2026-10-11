using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.HRC.IsKurallari.Veri;
using System.Data;
using TNS.MUH;

namespace TNS.HRC.IsKurallari
{
    public class HRCTeminatMektubu
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TeminatMektubu bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (!HRCYetkiKontrol.TeminatMektubuYetkisiVarmi(kullanan, 1))
            {
                sonuc.DegerAta("Yazma yetkiniz bulunmamaktadır!" + "<br/>");
                return sonuc;
            }
            sonuc.hataStr = VeriKontrol(bilgi, string.IsNullOrEmpty(bilgi.dosyaNo) ? ENUMIslemTuru.KAYIT : ENUMIslemTuru.GUNCELLEME, kullanan);
            if (sonuc.hataStr.Trim() != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                ve.BeginTransaction();
                TABTeminatMektubu TABMektup = new TABTeminatMektubu(ve);

                if (!string.IsNullOrEmpty(bilgi.dosyaNo))
                {
                    TABMektup.Sil(bilgi);
                    if (bilgi.cikisBilgileri != null)
                        TABMektup.SilCikis(bilgi.yil, bilgi.dosyaNo);
                }
                else
                {
                    string sayacKod = bilgi.yil.ToString();
                    sayacKod = "TASINMAZ" + sayacKod;//TEMINATMEKTUBU OLMALI AMA ÇALIŞAN KURUMLAR BOZULMASIN DİYE YAPILMADI MELİH 02.06.2015
                    TABGenel tg = new TABGenel(ve);
                    bilgi.dosyaNo = tg.BelgeNoAl(sayacKod, "000000");
                    // bilgi.dosyaNo = DateTime.Now.Year + "-" + TABMektup.getMaxDosyaNo(bilgi);
                }
                if (ve.hata.Trim() == "")
                {
                    sonuc = TABMektup.Kaydet(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        foreach (TeminatMektubuCikisBilgileri cikis in bilgi.cikisBilgileri)
                        {
                            cikis.kod = TABMektup.getCikisMaxKod(cikis);
                            cikis.dosyaNo = bilgi.dosyaNo;
                            sonuc = TABMektup.KaydetCikis(cikis);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.anahtar = bilgi.dosyaNo.ToString();
                        sonuc.bilgiStr = HarcamaSurecleriResI.HRC00301;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                else
                {
                    sonuc.DegerAta(ve.hata);
                }
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.Message);
            }
            finally
            {
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TeminatMektubu bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (!HRCYetkiKontrol.TeminatMektubuYetkisiVarmi(kullanan, 1))
            {
                sonuc.DegerAta("Yazma yetkiniz bulunmamaktadır!" + "<br/>");
                return sonuc;
            }

            sonuc.hataStr += TarihKapaliMi(kullanan, bilgi.kayitTarihi);
            sonuc.hataStr += VeriKontrol(bilgi, ENUMIslemTuru.SILME, kullanan);
            if (sonuc.hataStr.Trim() != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                ve.BeginTransaction();
                TABTeminatMektubu TABMektup = new TABTeminatMektubu(ve);

                if (!string.IsNullOrEmpty(bilgi.dosyaNo))
                {
                    sonuc = TABMektup.Sil(bilgi);

                    if (sonuc.islemSonuc)
                        sonuc = TABMektup.SilCikis(bilgi.yil, bilgi.dosyaNo);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.anahtar = bilgi.dosyaNo.ToString();
                        sonuc.bilgiStr = HarcamaSurecleriResI.HRC00301;
                    }
                    else
                    {
                        ve.Rollback();
                        sonuc.DegerAta(ve.hata);
                    }
                }
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.Message);
            }
            finally
            {
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TeminatMektubu bilgi, bool uyarilacaklar)
        {
            ObjectArray sonuclar = new ObjectArray();
            //string hata = VeriKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            //if (hata.Trim() != "")
            //{
            //    sonuclar.sonuc.DegerAta(hata);
            //    return sonuclar;
            //}

            //Taahhüt ten listeleme yapıldığında yetki kontrolü olmamalı Melih

            //if (!HRCYetkiKontrol.TeminatMektubuYetkisiVarmi(kullanan, 2))
            //{
            //    sonuclar.sonuc.DegerAta("Listeleme Yetkiniz Bulunmamaktadır!" + "<br/>");
            //    return sonuclar;
            //}

            //if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebeKod.ToString(), bilgi.birimKod))
            //{
            //    sonuclar.sonuc.hataStr = ButceMuhasebeResI.MUH081;
            //    sonuclar.sonuc.islemSonuc = false;
            //    return sonuclar;
            //}

            if (bilgi.birimKod != "") bilgi.birimKod += ";";
            bilgi.birimKod += TNS.MUH.Yetki.KullaniciMuhasebeBirimiGetir(kullanan);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            IDataReader dr = null;
            try
            {
                TABTeminatMektubu TABMektup = new TABTeminatMektubu(ve);
                dr = TABMektup.Listele(bilgi, uyarilacaklar);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        TeminatMektubu tm = new TeminatMektubu();
                        tm.yil = VeriErisim.DegerAl(dr, 0, 0);
                        tm.dosyaNo = VeriErisim.DegerAl(dr, 1, "");
                        tm.muhasebeKod = VeriErisim.DegerAl(dr, 2, 0);
                        tm.birimKod = VeriErisim.DegerAl(dr, 3, "");
                        tm.kayitTarihi = VeriErisim.DegerAl(dr, 4);
                        tm.giris.firma.kod = VeriErisim.DegerAl(dr, 5, 0);
                        tm.giris.dosyaTarih = VeriErisim.DegerAl(dr, 6);
                        tm.giris.numara = VeriErisim.DegerAl(dr, 7, "");
                        tm.giris.banka.kod = VeriErisim.DegerAl(dr, 8, "");
                        tm.giris.alinisSebebi = VeriErisim.DegerAl(dr, 9, "");
                        tm.giris.vadeTarih = VeriErisim.DegerAl(dr, 10);
                        tm.giris.nevi = VeriErisim.DegerAl(dr, 11, 0);
                        tm.giris.tutar = VeriErisim.DegerAl(dr, 12, (double)0);
                        tm.giris.muhasebeIslem.yil = VeriErisim.DegerAl(dr, 13, 0);
                        tm.giris.muhasebeIslem.belgeNo = VeriErisim.DegerAl(dr, 14, "");
                        tm.giris.muhasebeIslem.belgeTarihi = VeriErisim.DegerAl(dr, 15);
                        tm.cikis.tarih = VeriErisim.DegerAl(dr, 16);
                        tm.cikis.iadeNeden = VeriErisim.DegerAl(dr, 17, "");
                        tm.cikis.muhasebeIslem.yil = VeriErisim.DegerAl(dr, 18, 0);
                        tm.cikis.muhasebeIslem.belgeNo = VeriErisim.DegerAl(dr, 19, "");
                        tm.cikis.muhasebeIslem.belgeTarihi = VeriErisim.DegerAl(dr, 20);
                        tm.giris.bankaSube.kod = VeriErisim.DegerAl(dr, 21, "");
                        tm.suresiz = VeriErisim.DegerAl(dr, 22, 0);
                        tm.sozlesmeKod = VeriErisim.DegerAl(dr, 23, 0);

                        tm.giris.firma.ad = VeriErisim.DegerAl(dr, 24, "");
                        tm.giris.firma.vergiNo = VeriErisim.DegerAl(dr, 25, "");
                        tm.giris.banka.ad = VeriErisim.DegerAl(dr, 26, "");
                        tm.giris.bankaSube.ad = VeriErisim.DegerAl(dr, 27, "");
                        tm.giris.neviAdi = VeriErisim.DegerAl(dr, 28, "");

                        tm.cikis.tutar = VeriErisim.DegerAl(dr, 29, (double)0);
                        tm.cikis.tarih = VeriErisim.DegerAl(dr, 30);

                        sonuclar.objeler.Add(tm);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                sonuclar.sonuc.DegerAta(ex.Message);
            }
            finally
            {
                if (dr != null && !dr.IsClosed) dr.Close();
                ve.VeriErisimKapat();
            }
            return sonuclar;
        }

        public static ObjectArray ListeleCikis(VeriErisim veriErisim, Kullanici kullanan, TeminatMektubuCikisBilgileri bilgi)
        {
            ObjectArray sonuclar = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            IDataReader dr = null;
            try
            {
                ve.BeginTransaction();
                TABTeminatMektubu TABMektup = new TABTeminatMektubu(ve);
                dr = TABMektup.ListeleCikis(bilgi);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        TeminatMektubuCikisBilgileri tm = new TeminatMektubuCikisBilgileri();
                        tm.kod = VeriErisim.DegerAl(dr, 0, 0);
                        tm.yil = VeriErisim.DegerAl(dr, 1, 0);
                        tm.dosyaNo = VeriErisim.DegerAl(dr, 2, "");
                        tm.tutar = VeriErisim.DegerAl(dr, 3, 0.0);
                        tm.cikisTarih = VeriErisim.DegerAl(dr, 4);
                        tm.iadeNeden = VeriErisim.DegerAl(dr, 5, "");
                        tm.muhasebeIslem.yil = VeriErisim.DegerAl(dr, 6, 0);
                        tm.muhasebeIslem.belgeNo = VeriErisim.DegerAl(dr, 7, "");
                        tm.muhasebeIslem.belgeTarihi = VeriErisim.DegerAl(dr, 8);
                        tm.durum = VeriErisim.DegerAl(dr, 9, 0);

                        sonuclar.objeler.Add(tm);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                sonuclar.sonuc.DegerAta(ex.Message);
            }
            finally
            {
                if (dr != null && !dr.IsClosed) dr.Close();
                ve.VeriErisimKapat();
            }
            return sonuclar;
        }

        private static string VeriKontrol(TeminatMektubu bilgi, ENUMIslemTuru islemturu, Kullanici kullanan)
        {
            StringBuilder hatalar = new StringBuilder();

            if (islemturu == ENUMIslemTuru.KAYIT || islemturu == ENUMIslemTuru.GUNCELLEME)
            {
                if (bilgi.muhasebeKod <= 0)
                    hatalar.Append("Muhasebe kodu boş olmamalıdır<br/>");
                if (string.IsNullOrEmpty(bilgi.birimKod))
                    hatalar.Append("Birim kodu boş olmamalıdır<br/>");

                if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebeKod.ToString(), bilgi.birimKod))
                {
                    hatalar.Append(ButceMuhasebeResI.MUH081);
                }

                if (islemturu == ENUMIslemTuru.GUNCELLEME)
                {
                    if (bilgi.yil <= 0)
                        hatalar.Append("Yıl boş olmamalıdır<br/>");

                    if (string.IsNullOrEmpty(bilgi.dosyaNo))
                        hatalar.Append("Dosya numarası boş olmamalıdır<br/>");
                }

                if (string.IsNullOrEmpty(bilgi.giris.alinisSebebi))
                    hatalar.Append("Alınış sebebi boş olmamalıdır<br/>");
                if (string.IsNullOrEmpty(bilgi.giris.numara))
                    hatalar.Append("Teminat belgesinin numarası boş olmamalıdır<br/>");
                if (bilgi.giris.dosyaTarih.isNull)
                    hatalar.Append("Giriş tarihi boş bırakılamaz" + "<br/>");
                if (bilgi.giris.nevi <= 0)
                    hatalar.Append("Nevi bilgisi belirtiniz<br/>");
                if (bilgi.giris.tutar <= 0)
                    hatalar.Append("Tutar bilgisi sıfırdan büyük olmamalıdır<br/>");
                if (bilgi.giris.vadeTarih != null && bilgi.giris.vadeTarih.Oku() < bilgi.giris.dosyaTarih.Oku() && bilgi.suresiz == 0)
                    hatalar.Append("Vade tarihi günün tarihinden küçük olamaz" + "<br/>");
                if (string.IsNullOrEmpty(bilgi.giris.banka.kod))
                    hatalar.Append("Banka Adı boş olmamalıdır<br/>");
                if (string.IsNullOrEmpty(bilgi.giris.bankaSube.kod))
                    hatalar.Append("Banka Şube Adı boş olmamalıdır<br/>");

                double girisTutar = bilgi.giris.tutar;
                double cikisTutar = 0.0;
                foreach (TeminatMektubuCikisBilgileri cikis in bilgi.cikisBilgileri)
                {
                    cikisTutar += cikis.tutar;
                }
                if (girisTutar < cikisTutar)
                {
                    hatalar.Append("Çıkış tutarı giriş tutarından yüksek olamaz" + "<br/>");
                }

                if (bilgi.giris.firma.kod > 0)
                {
                    FirmaBilgisi fSorgu = new FirmaBilgisi();
                    fSorgu.kod = OrtakFonksiyonlar.ConvertToInt(bilgi.giris.firma.kod, 0);
                    IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                    ObjectArray liste = servisHRC.FirmaBilgisiListele(kullanan, fSorgu);

                    if (liste.objeler.Count == 0 || bilgi.giris.firma.kod == 0)
                        hatalar.Append("Girilen firma koduna ait bilgi bulunmamaktadır. Lütfen firma kaydı yaptıktan sonra teminat mektubu kayıt işlemine devam ediniz" + "<br/>");
                }
                else
                    hatalar.Append("Girilen firma koduna ait bilgi bulunmamaktadır. Lütfen firma kaydı yaptıktan sonra teminat mektubu kayıt işlemine devam ediniz" + "<br/>");

            }
            else if (islemturu == ENUMIslemTuru.SILME)
            {
                if (bilgi.muhasebeKod <= 0)
                    hatalar.Append("Muhasebe kodu boş olmamalıdır<br/>");
                if (string.IsNullOrEmpty(bilgi.birimKod))
                    hatalar.Append("Birim kodu boş olmamalıdır<br/>");

                if (bilgi.yil <= 0)
                    hatalar.Append("Yıl boş olmamalıdır<br/>");

                if (string.IsNullOrEmpty(bilgi.dosyaNo))
                    hatalar.Append("Dosya numarası boş olmamalıdır<br/>");

                //Ödeme emri belgeleri düzenlenmiş mi?
                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                TNS.MUH.OdemeEmriMIFForm oeForm = new TNS.MUH.OdemeEmriMIFForm();

                //Giriş Muhasebe belgesi var mı?
                if (bilgi.giris.muhasebeIslem.belgeNo != "")
                {
                    oeForm.yil = bilgi.giris.muhasebeIslem.yil;
                    oeForm.belgeNo = bilgi.giris.muhasebeIslem.belgeNo;

                    ObjectArray oeGirisBelge = servisMUH.OdemeEmriMIFTekBelgeListele(kullanan, oeForm);
                    if (oeGirisBelge.objeler.Count > 0)
                    {
                        foreach (TNS.MUH.OdemeEmriMIFForm oe in oeGirisBelge.objeler)
                        {
                            if (oe.formDurum != (int)TNS.MUH.ENUMBelgeDurumu.IPTAL)
                            {
                                hatalar.Append(oeForm.yil + "/" + oeForm.belgeNo + " nolu ödeme emri belgesi iptal edilmeden silme işlemi yapılamaz.<br>");
                                break;
                            }
                        }
                    }
                }

                //Çıkış Muhasebe belgesi var mı?
                if (bilgi.cikis.muhasebeIslem.belgeNo != "")
                {
                    oeForm.yil = bilgi.cikis.muhasebeIslem.yil;
                    oeForm.belgeNo = bilgi.cikis.muhasebeIslem.belgeNo;

                    ObjectArray oeCikisBelge = servisMUH.OdemeEmriMIFTekBelgeListele(kullanan, oeForm);
                    if (oeCikisBelge.objeler.Count > 0)
                    {
                        foreach (TNS.MUH.OdemeEmriMIFForm oe in oeCikisBelge.objeler)
                        {
                            if (oe.formDurum != (int)TNS.MUH.ENUMBelgeDurumu.IPTAL)
                            {
                                hatalar.Append(oeForm.yil + "/" + oeForm.belgeNo + " nolu ödeme emri belgesi iptal edilmeden silme işlemi yapılamaz.<br>");
                                break;
                            }
                        }
                    }
                }
            }
            return hatalar.ToString();
        }

        public static Sonuc GuncelleGirisOdemeEmri(VeriErisim veriErisim, Kullanici kullanan, int dosyaYil, string dosyaNo, int belgeYil, string belgeNo, TNSDateTime belgeTarihi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTeminatMektubu TABMektup = new TABTeminatMektubu(ve);

            Sonuc sonuc = TABMektup.GuncelleGirisOdemeEmri(dosyaYil, dosyaNo, belgeYil, belgeNo, belgeTarihi);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc GuncelleCikisOdemeEmri(VeriErisim veriErisim, Kullanici kullanan, int cikisKod, int belgeYil, string belgeNo, TNSDateTime belgeTarihi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTeminatMektubu TABMektup = new TABTeminatMektubu(ve);

            Sonuc sonuc = TABMektup.GuncelleCikisOdemeEmri(cikisKod, belgeYil, belgeNo, belgeTarihi);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static Sonuc UyariGonder(VeriErisim veriErisim, Kullanici kullanan)
        {
            Sonuc sonuc = new Sonuc();
            string tut = TNS.DEG.Arac.DegiskenDegerBul(0, "TeminatUyariEPosta");
            if (string.IsNullOrWhiteSpace(tut))
                return sonuc;

            int kontrolGun = 30;
            if (TNS.HRC.Arac.CumhurbaskanligiMi()) kontrolGun = 20;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TeminatMektubu mektup = new TeminatMektubu();
            mektup.giris.vadeTarih2 = new TNSDateTime(DateTime.Now.Date.AddDays(kontrolGun));
            mektup.cikis.durum = (int)ENUMCikisDurum.CIKISYAPILMAYAN;
            ObjectArray oa = Listele(ve, kullanan, mektup, true);
            if (!oa.sonuc.islemSonuc || oa.ObjeSayisi == 0)
            {
                ve.VeriErisimKapat();
                return oa.sonuc;
            }

            string html = "<html><head><style>td {padding:5px;} p {padding:5px;}</style></head><body><table border='1' cellspacing='0' cellpadding='0'><thead><tr style='font-weight:bold; text-align:center'>" + td(kolStil1, "#", "Vade Durumu", "Dosya No", "Numarası", "Nevi", "Firma Adı", "Kayıt Tarihi", "Vade Tarihi") + "</tr></thead><tbody>";

            int i = 1;
            foreach (TeminatMektubu item in oa.objeler)
            {
                html += "<tr>" + td(kolStil2, i.ToString(), item.vadeAciklama, item.yilDosyaNo, item.giris.numara, item.giris.neviAdi, item.giris.firma.ad, item.kayitTarihi.ToString(), item.giris.vadeTarih.ToString()) + "</tr>";
                i++;
            }
            html += "</tbody></table></body></html>";

            string sonucStr = OrtakFonksiyonlar.MailAt(null, tut, "Teminat Mektubu Bilgilendirme", html, true, false, null);
            if (sonucStr == "1")
            {
                TABTeminatMektubu TABMektup = new TABTeminatMektubu(ve);
                ve.BeginTransaction();
                TNSDateTime buSaat = new TNSDateTime(DateTime.Now);
                foreach (TeminatMektubu item in oa.objeler)
                {
                    sonuc = TABMektup.UyariGondermeTarihiYaz(item, buSaat);
                    if (!sonuc.islemSonuc) break;
                }
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        private static string[] kolStil1 = { "width: 25px;", "width: 150px;", "width: 100px;", "width: 120px;", "width: 200px;", "width: 300px;", "width: 80px;", "width: 80px;" };
        private static string[] kolStil2 = { "width: 25px;text-align:center;", "width: 150px;text-align:center;", "width: 100px;", "width: 120px;", "width: 200px;", "width: 300px;", "width: 80px;text-align:center;", "width: 80px;text-align:center;" };
        private static string td(string[] stil, params string[] icerik)
        {
            string html = "";
            for (int i = 0; i < icerik.Length; i++)
            {
                html += "<td style='" + stil[i] + "'>" + icerik[i] + "</td>";
            }
            return html;
        }

        internal static string TarihKapaliMi(Kullanici kullanan, TNSDateTime islemTarih)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return "";

            IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

            string degiskenAd = "MIFTARIHKAPAT";
            string donenDeger = "";
            TNSCollection kDeg = servisKYM.DegiskenleriListele("stratek", degiskenAd);
            if (kDeg != null && kDeg.Count > 0)
            {
                TNSDateTime kapaliTarih = new TNSDateTime(kDeg[0].ToString());
                if (islemTarih.Oku() < kapaliTarih.Oku())
                    donenDeger = "<br>" + string.Format(islemTarih.Yil + " yılı işlem yapmaya kapatılmıştır.<br>");
            }
            return donenDeger;
        }

    }
}

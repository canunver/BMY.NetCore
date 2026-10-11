using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TNS.BHMv2;
using TNS.BKD;
using TNS.DEG;
using TNS.HRC;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.PER;
using TNS.Veri;
namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// ödeme emri / MİF işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHOdemeEmriMIFServis
    {
        static IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
        static IPERServis servisPER = TNS.PER.Arac.Tanimla();
        EksikOdenekler KontrolEt(Kullanici kullanan, OdemeEmriMIFForm form, EksikOdenekler eoKontrolsuz)
        {
            Tertip tUzBul = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);
            int eko3Uz = tUzBul.KodUzunlukBul(3, 3, true);
            bool detayBirimKullaniyor = TNS.MUH.Arac.DetayBirimVarMi();
            if (string.IsNullOrWhiteSpace(form.islemYapan) || string.IsNullOrWhiteSpace(form.islemYapanAd))
            {
                form.islemYapan = kullanan.mernis;
                form.islemYapanAd = kullanan.ADSOYAD;
            }
            bool ego = false;
            if (string.IsNullOrWhiteSpace(form.kurum)) form.kurum = TNS.DEG.Arac.VarsayilanKurumBul();
            if (form.kurum.Replace(".", "") == "460604") ego = true;
            string hata = "";
            List<string> ekoKodlar = new List<string>();
            EksikOdenekler eo = new EksikOdenekler();
            eo.sonuc = new Sonuc(true, "", "");
            if (form.aktarim) return eo;
            //Muhasebe birim Listesi
            MuhasebeBirim mbirim = new MuhasebeBirim();
            mbirim.muhasebe = form.muhasebe;
            ObjectArray mbirimListesi = new MUHMuhasebeBirimServis().Listele(mbirim);
            Hashtable mBirimListe = new Hashtable();
            foreach (MuhasebeBirim item in mbirimListesi.objeler)
            {
                //if (item.pasif > 0) continue;
                //if (item.durum == 2) continue;//PASİF
                mBirimListe[item.muhasebe.ToString() + item.birimKod + item.kurum.Replace(".", "")] = item;
            }
            //if (MUHMuhasebeBirimServis.BirimKapaliMi(kullanan, form.muhasebe, form.birim, form.kurum))
            //{
            //    eo.sonuc.DegerAta("Birim kapalı olduğundan ödeme emri işlemi yapılamaz!");
            //    return eo;
            //}
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim veBkd = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBKD", TNS.KYM.Arac.KimlikBul(kullanan));
            //MükerrerlikKontrolu
            //İstemci tarafından kontrol yapılma isteği gelirse ve kurumda bu işlemi talep etmişse kontrol yapılsın. YENİ KAYIT İÇİN
            bool mukerrerlikKontroluYap = OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUKERRERLIKKONTROLUYAP") + "", 0) > 0;
            if (form.mukerrerlikKontrolu && string.IsNullOrEmpty(form.belgeNo.Trim()) && mukerrerlikKontroluYap)
            {
                hata = MukerrerlikKontrolu(ve, kullanan, form);
                if (hata != "")
                {
                    hata = "!MKK!" + hata;
                    eo.sonuc.DegerAta(hata);
                    ve.VeriErisimKapat();
                    veBkd.VeriErisimKapat();
                    return eo;
                }
            }
            //Yetki Kontrolü
            MUHMuhasebeBirimServis mb = new MUHMuhasebeBirimServis();
            ABSKodlarVAN kv = mb.HarcamaBirimleriKurumsalKod(ve, form.yil, form.kurum, form.muhasebe, form.birim);
            if (kv.sonuc.islemSonuc)
            {
                if (kv.absKodlar.Count == 0)
                {
                    ve.VeriErisimKapat();
                    veBkd.VeriErisimKapat();
                    eo.sonuc.DegerAta(ButceMuhasebeResI.MUH142 + " (" + form.yil + ", " + form.kurum + ", " + form.muhasebe + ", " + form.birim + ")");
                    return eo;
                }
                if (string.IsNullOrWhiteSpace(form.kurKod))
                {
                    ABSKod ilkKod = (ABSKod)kv.absKodlar[0];
                    form.kurKod = ilkKod.kod;
                }
                bool yetkiVar = false;
                string bakilanKodlar = "";
                foreach (ABSKod k in kv.absKodlar)
                {
                    if (MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMi(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, k.kod))
                    {
                        yetkiVar = true;
                        if (bakilanKodlar != "") bakilanKodlar += ",";
                        bakilanKodlar += k.kod;
                        break;
                    }
                }
                if (!yetkiVar)
                {
                    ve.VeriErisimKapat();
                    veBkd.VeriErisimKapat();
                    eo.sonuc.DegerAta(ButceMuhasebeResI.MUH081 + "(" + bakilanKodlar + ")");
                    return eo;
                }
            }
            else
            {
                ve.VeriErisimKapat();
                veBkd.VeriErisimKapat();
                eo.sonuc.DegerAta(kv.sonuc.hataStr + " * " + ButceMuhasebeResI.MUH143);
                return eo;
            }
            if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebe.ToString(), form.birim))
            {
                ve.VeriErisimKapat();
                veBkd.VeriErisimKapat();
                eo.sonuc.DegerAta(ButceMuhasebeResI.MUH081 + "(" + form.muhasebe.ToString() + "-" + form.birim + ")");
                return eo;
            }
            if (form.yil > DateTime.Now.Year)
                hata += "<br>" + string.Format(ButceMuhasebeResI.MUH144, DateTime.Now.Year);
            if (form.islemTarih.isNull)
                hata += "<br>" + ButceMuhasebeResI.MUH013;
            else
            {
                if (form.islemTarih.Oku().Year != form.yil)
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH145, form.yil, form.islemTarih.ToString());
            }
            //İşlem yapılan tarih kapatılmış mı kontrolü
            hata += TarihKapaliMi(kullanan, form.islemTarih);
            if (form.kurum.Replace(".", "").Trim() == "")
                hata += "<br>" + ButceMuhasebeResI.MUH014;
            else
            {
                string kKurum = MUHServis.UzayYaziDegerGetir(veBkd, kullanan, "ABSKUR", form.yil.ToString() + "-" + form.kurum, true, "KURKOD");
                if (kKurum.Replace(".", "") != form.kurum.Replace(".", ""))
                    hata += "<br><b>" + form.kurum + "</b> " + ButceMuhasebeResI.MUH015;
            }
            if (form.muhasebe == 0)
                hata += "<br>" + ButceMuhasebeResI.MUH016;
            else
            {
                double kMuhasebe = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHMUHASEBE", form.muhasebe.ToString(), true, "MUHASEBEKOD");
                if (Convert.ToInt32(kMuhasebe) != form.muhasebe)
                    hata += "<br><b>" + form.muhasebe + "</b> " + ButceMuhasebeResI.MUH017;
            }
            //if (form.belgeTur == ENUMBelgeTuru.ODEMEEMRI && (form.birim.Trim() == "" || form.birim.Trim() == "000"))//ego yıl sonu kapatmasında sorun
            //    hata += "<br>" + ButceMuhasebeResI.MUH147;
            if (!string.IsNullOrWhiteSpace(form.birim))
            {
                if (form.birim.Trim() != "000" && detayBirimKullaniyor)
                {
                    MuhasebeBirim kBirim = (MuhasebeBirim)mBirimListe[form.muhasebe.ToString() + form.birim + form.kurKod.Replace(".", "")];
                    //Eğer kurumsal kod ile birim yoksa birim, kurum kodu ile tanımlanmış diye bakılsın
                    if (kBirim == null)
                    {
                        if (form.kurKod.Replace(".", "").StartsWith(form.kurum.Replace(".", "")))
                            kBirim = (MuhasebeBirim)mBirimListe[form.muhasebe.ToString() + form.birim + form.kurum.Replace(".", "")];
                    }
                    if (kBirim != null)
                    {
                        //MuhasebeBirim kkbirim = (MuhasebeBirim)mBirimListe[form.muhasebe.ToString() + form.birim + form.kurKod.Replace(".", "")];
                        if (kBirim.pasif > 0)
                            hata += "<br>" + kBirim.birimKod + " - " + kBirim.birimAd + " birimi kapalı olduğu için kullanılmaz";
                    }
                    else
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH089, form.birim);
                }
            }
            if (!string.IsNullOrWhiteSpace(form.ilgiliBankaNo))
            {
                if (form.ilgiliBankaNo != "Muhtelif")
                {
                    Sonuc IBANKontrol = OrtakFonksiyonlar.IBANKontrol(form.ilgiliBankaNo.Trim(), true);
                    if (!IBANKontrol.islemSonuc)
                        hata += form.ilgiliBankaNo + " bilgisi hatalı: " + IBANKontrol.hataStr + "<br>";
                }
            }
            if (!string.IsNullOrWhiteSpace(form.belgeNo))
            {
                //Eğer belge değiştirilmek üzere gönderilmiş ise 
                //Kayıtlımı
                //Kayıtlı ise veri tabanındaki belge 
                //belgeyi gönderen kurumamı ait,
                //durumu değişikliğie uygunmu kontrolü
                if (form.belgeNo.Length < 9)
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH018, form.belgeNo);
                if (!form.belgeNo.StartsWith(form.kurum.Replace(".", "")))
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH019, form.belgeNo);
                else
                {
                    string kurum = form.kurum.Replace(".", "");
                    string bNo = form.belgeNo;
                    bNo = form.belgeNo.Substring(kurum.Length, bNo.Length - kurum.Length);
                    if (bNo.Length < 6)
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH205, form.belgeNo);
                }
                if (OrtakFonksiyonlar.ConvertToLong(form.belgeNo) == 0)
                {
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH206, form.belgeNo);
                }
                OdemeEmriMIFForm oS = TekBelgeGetir(ve, kullanan, form, false);
                if (oS.kurum != "")
                {
                    ABSKodlarVAN kkv = mb.HarcamaBirimleriKurumsalKod(ve, oS.yil, oS.kurum, oS.muhasebe, oS.birim);
                    if (kkv.sonuc.islemSonuc)
                    {
                        if (kkv.absKodlar.Count == 0)
                        {
                            ve.VeriErisimKapat();
                            veBkd.VeriErisimKapat();
                            eo.sonuc.DegerAta(ButceMuhasebeResI.MUH142 + " (" + oS.yil + ", " + oS.kurum + ", " + oS.muhasebe + ", " + oS.birim + ")");
                            return eo;
                        }
                        bool yetkiVar = false;
                        foreach (ABSKod k in kkv.absKodlar)
                        {
                            if (MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMi(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, k.kod))
                            {
                                yetkiVar = true;
                                break;
                            }
                        }
                        if (!yetkiVar)
                        {
                            ve.VeriErisimKapat();
                            veBkd.VeriErisimKapat();
                            eo.sonuc.DegerAta(string.Format(ButceMuhasebeResI.MUH148, oS.birim));
                            return eo;
                        }
                    }
                    if (oS.kurum.Replace(".", "") != form.kurum.Replace(".", ""))
                    {
                        ve.VeriErisimKapat();
                        veBkd.VeriErisimKapat();
                        eo.sonuc.DegerAta(string.Format(ButceMuhasebeResI.MUH020, form.belgeNo.ToString()));
                        return eo;
                    }
                    if (oS.formDurum != (int)ENUMBelgeDurumu.YENI && oS.formDurum != (int)ENUMBelgeDurumu.DEGISTIRILDI)
                    {
                        if (oS.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                        {
                            if (form.formDurum != (int)ENUMBelgeDurumu.ONAYKALDIR || form.formDurum != (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                                hata = "<br>" + string.Format(ButceMuhasebeResI.MUH149, form.belgeNo.ToString());
                        }
                        else if (oS.formDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                        {
                            if (form.formDurum != (int)ENUMBelgeDurumu.YEVMIYEKALDIR)
                            {
                                if (!form.aktarim)
                                    hata = "<br>" + string.Format(ButceMuhasebeResI.MUH150, form.belgeNo.ToString());
                            }
                        }
                        if (hata != "")
                        {
                            ve.VeriErisimKapat();
                            veBkd.VeriErisimKapat();
                            eo.sonuc.DegerAta(hata);
                            return eo;
                        }
                    }
                }
                else
                    if (!form.aktarim)
                    hata += "<br>" + ButceMuhasebeResI.MUH027;
            }
            //TODO İşlem tarihi kayıtlı diğer evraklardan öncesi varmı yokmu yeni ve değiştirilmek üzere gönderilmiş olsada
            //eğer değiştirilmek üzere gönderilmiş ise nasıl davranılmalı
            /*
            //Hesap no, tertip, borç , alacak bilgileri diğer satırlarda da tekrar edilmişmi kontolü
            //**********************************************************
            string[,] detayTekrar = new string[500, 1];
            int sayac = 0;
            foreach (OdemeEmriMIFDetay detay in form.detay)
            {
                string hesapKod = detay.hesapKod;
                string kur = (string)detay.tertip.kodlar[0];
                string fon = (string)detay.tertip.kodlar[1];
                string fin = (string)detay.tertip.kodlar[2];
                string eko = (string)detay.tertip.kodlar[3];
                bool borcKontrol = detay.borc > 0;
                bool alacakKontrol = detay.alacak > 0;
                detayTekrar[sayac, 0] = hesapKod + kur + fon + fin + eko + borcKontrol.ToString() + alacakKontrol.ToString();
                for (int i = 0; i < sayac; i++)
                {
                    if (detayTekrar[i, 0] == hesapKod + kur + fon + fin + eko + borcKontrol.ToString() + alacakKontrol.ToString())
                    {
                        hata += "<br>" + (i + 1).ToString() + ". ile " + (sayac + 1).ToString() + ". satırlarda içerik aynı. Bir belgede, aynı Hesap No ve Tertip içeren satırı birden fazla kullanamazsınız.";
                        break;
                    }
                }
                sayac++;
            }
            //**********************************************************
            */
            int satir = 0;
            decimal toplamBorc = 0;
            decimal toplamAlacak = 0;
            TABHesapPlani tg = new TABHesapPlani(ve);
            TABOdemeEmriMIF tabMIF = new TABOdemeEmriMIF(ve);
            bool veriVar = false;
            for (int i = form.detay.ObjeSayisi - 1; i >= 0; i--)
            {
                OdemeEmriMIFDetay detay = (OdemeEmriMIFDetay)form.detay.objeler[i];
                if (!(detay.borc == 0 && detay.alacak == 0 && detay.dovizBorc == 0 && detay.dovizAlacak == 0))
                {
                    veriVar = true;
                    break;
                }
            }
            if (veriVar)
            {
                for (int i = form.detay.ObjeSayisi - 1; i >= 0; i--)
                {
                    OdemeEmriMIFDetay detay = (OdemeEmriMIFDetay)form.detay.objeler[i];
                    if (detay.borc == 0 && detay.alacak == 0 && detay.dovizBorc == 0 && detay.dovizAlacak == 0)
                    {
                        form.detay.objeler.RemoveAt(i);
                    }
                }
            }
            if (form.detay.objeler.Count == 0 && !form.sadeceUstVeri)
                hata += "Belgenin detayında kayıt edilecek herhangi bir bilgi bulunamadı.";
            ENUMOdenekKontrolTur kt = TNS.MUH.Arac.OdenekKontrolTurBul(form.yil);
            //TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            string kodAd;
            string kod;
            List<OdemeEmriMIFDetay> zorunluYansitmalar = new List<OdemeEmriMIFDetay>();
            string ortak000 = TNS.MUH.Arac.DegiskenDegerBul(form.yil, "/Ortak000");
            List<OdemeEmriMIFDetay> bakiyeKontrolListesi = new List<OdemeEmriMIFDetay>();
            TNSCollection tertiplerDizisi = new TNSCollection();
            Hashtable hHesapNitelik = new Hashtable();
            Hashtable hMutemet = new Hashtable();
            bool avansKontroluYap = OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/AVANSKONTROLUYAP") + "", 0) > 0;
            if (form.projeNo == "KURFARKI")
                avansKontroluYap = false;

            string gelirBirimZorunlu = System.Configuration.ConfigurationManager.AppSettings.Get("GelirBirimZorunlu") + "";
            int satirSayac = 0;
            bool oebKontrol = false;
            Hashtable htHesapKod = new Hashtable();
            foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
            {
                satirSayac++;
                string hesapKod = detay.hesapKod;
                detay.borc = Math.Round(detay.borc, 2);
                detay.alacak = Math.Round(detay.alacak, 2);
                decimal borc = Math.Round(OrtakFonksiyonlar.ConvertToDecimal(detay.borc), 2);
                decimal alacak = Math.Round(OrtakFonksiyonlar.ConvertToDecimal(detay.alacak), 2);
                int kesinti = detay.kesinti;
                toplamBorc += borc;
                toplamAlacak += alacak;
                satir++;
                if (borc > 0 && alacak > 0)
                    hata += "<br>" + satir.ToString() + " nolu satırda borç ve alacak kısmının her ikisine de miktar girilmiş.";
                if (borc < 0 || alacak < 0)
                    hata += "<br>" + satir.ToString() + " nolu satırda borç veya alacak kısmına negatif miktar girilmiş.";
                //EGO gelir hesapları için birim yazılması zorunlu olsun istedi Melih 05.09.2018
                if (gelirBirimZorunlu == "1" && (detay.hesapKod.StartsWith("600") || detay.hesapKod.StartsWith("800")))
                {
                    if (detay.detayBirim == "000")
                    {
                        hata += "<br>" + satir.ToString() + " nolu satırda birim girilmesi zorunludur.";
                    }
                }
                //Döviz kuralları
                if (!string.IsNullOrWhiteSpace(detay.doviz) && form.projeNo != "KURFARKI")
                {
                    if (string.IsNullOrWhiteSpace(detay.dovizKurTur))
                        hata += "<br>" + detay.satirNo + " nolu satırda " + detay.hesapKod + " hesap kodu için döviz para biriminin Kur türü belirtilmemiş.";
                    else if (detay.dovizKur == 0 && form.fisTur[0] != ENUMFISTUR.SONFIS)
                        hata += "<br>" + detay.satirNo + " nolu satırda " + detay.hesapKod + " hesap kodu için döviz para biriminin Kur bilgisi atanamamış.";
                    if (detay.dovizBorc == 0 && detay.dovizAlacak == 0)
                        hata += "<br>" + detay.satirNo + " nolu satırda " + detay.hesapKod + " hesap kodu için döviz borç/alacak bilgisi girilmemiş.";
                }
                if (detay.dovizBorc + detay.dovizAlacak > 0 && detay.borc + detay.alacak == 0)//Manas 17.11.2021 döviz tutarı varsa borç/alacak boş olmasın Melih
                {
                    hata += "<br>" + detay.satirNo + " nolu satırda Döviz tutarları girilmesine rağmen borç/alacak bilgisi girilmemiş.";
                }
                //Hesap Kodu geçerliliği, tertip ve diğer ilgili zorunluluklar kontrolü
                //**********************************************************
                if (hesapKod.Trim() == "")
                {
                    hata += "<br>" + satir.ToString() + " nolu satırda hesap no alanı boş bırakılmış.";
                    continue;
                }

                if (htHesapKod[detay.hesapKod] == null)
                {
                    detay.hKod = HesapPlaniBul(ve, form.yil, detay, hHesapNitelik);
                    htHesapKod[detay.hesapKod] = detay.hKod;
                }
                else
                    detay.hKod = (HesapPlaniSatir)htHesapKod[detay.hesapKod];
                //Satıra hesap kodu döviz olmadığı halde döviz girilmiş ise
                if (!string.IsNullOrWhiteSpace(detay.doviz) && form.projeNo != "KURFARKI")
                {
                    if (string.IsNullOrWhiteSpace(detay.hKod.dovizKurKod))
                        hata += "<br>" + satir.ToString() + " nolu satırda " + detay.hKod.hesapKod + " hesap kodu döviz ile çalışmadığı halde döviz bilgisi girilmiş.";
                }
                if (!string.IsNullOrWhiteSpace(detay.hKod.dovizKurKod) && form.fisTur[0] == ENUMFISTUR.DONEMICI)
                {
                    if (detay.doviz != detay.hKod.dovizKurKod)
                        hata += "<br>" + satir.ToString() + " nolu satırda " + detay.hKod.hesapKod + " hesap kodu (" + detay.hKod.dovizKurKod + ") ile çalışmaktadır. Düzeltip tekrar deneyin";
                }
                //Avans Kontrolü
                //***********************************************************
                if (avansKontroluYap && TNS.MUH.Arac.NitelikVarmi(detay.hKod.nitelik, (int)ENUMHesapKodNitelik.AVANS) && form.fisTur[0] != ENUMFISTUR.ACILIS)
                {
                    if (!TNS.MUH.Arac.NitelikVarmi(detay.hKod.nitelik, (int)ENUMHesapKodNitelik.AVANSKONTROLUYAPMA))
                    {
                        hata += AvansKontrolu(ve, kullanan, form, detay);
                        if (hata != "")
                        {
                            eo.sonuc.DegerAta(hata);
                            ve.VeriErisimKapat();
                            veBkd.VeriErisimKapat();
                            return eo;
                        }
                    }
                }
                //***********************************************************
                if (detay.hKod.hesapKod == "")
                {
                    hata += "<br>" + satir.ToString() + " nolu satırda verilen hesap no (" + detay.hesapKod + ") hesap planında yok.";
                    continue;
                }
                else if (TNS.MUH.Arac.NitelikVarmi(detay.hKod.nitelik, (int)ENUMHesapKodNitelik.PASIF))
                {
                    hata += "<br>" + satir.ToString() + " nolu satırda verilen hesap no (" + detay.hesapKod + ") pasif yapıldığından kullanılamaz.";
                    continue;
                }
                if (detay.hKod.altHesapKod <= 0 && detay.altHesapKod.Trim() != "")
                {
                    if (form.altHesapZorunsuzsaHata)
                        hata += "<br>" + satir.ToString() + " nolu satırda verilen hesap no (" + detay.hesapKod + "), alt hesap kodu kabul etmemektedir. Alt hesabı temizleyip tekrar kayıt işlemi yapın"; //EGO talebi alt hesap olarak işaretlenmeyenlere alt hesap kodu girilemesin
                    else
                        detay.altHesapKod = "";
                }
                else if (detay.hKod.altHesapKod > 0 && form.fisTur[0] == ENUMFISTUR.DONEMICI)//Sadece dönem içi fişlerde yapılsın
                {
                    //Mutemet listesinde hesapkod kayıtlı ise althesabı kontrol et
                    //Kayıt var ise liste dışında başka birisi hesabı kullanamaz
                    //Lİmit üstünde tutar kullanılamaz
                    ObjectArray mliste = MutemetBul(kullanan, form.yil, form.muhasebe, form.birim, detay.hesapKod, hMutemet);
                    bool bulundu = false;
                    foreach (MutemetTanim mt in mliste.objeler)
                    {
                        if (detay.altHesapKod.Trim() == mt.tcKimlikNo.Trim())
                        {
                            if ((mt.tutar > 0) && (detay.borc > mt.tutar || detay.alacak > mt.tutar))
                            {
                                hata += "<br>" + satir.ToString() + " nolu satırda verilen hesap no (" + detay.hesapKod + ") ve kişi (" + detay.altHesapKod + ") için tanımlanan limit üzerinde belge oluşturulmak isteniyor. Limit tutarı (" + mt.tutar.ToString("#,###.00") + "). Muhasebe yöneticisine başvurarak limitin değiştirilmesini sağlayabilirsiniz.";
                            }
                            bulundu = true;
                            break;
                        }
                    }
                    if (mliste.objeler.Count > 0 && !bulundu)
                        hata += "<br>" + satir.ToString() + " nolu satırda verilen hesap no (" + detay.hesapKod + ") için belirtilen kişinin (" + detay.altHesapKod + ") mutemet tanımı yapılmamış. Muhasebe yöneticisine başvurarak tanımın yapılmasını sağlayabilirsiniz.";
                    if (string.IsNullOrWhiteSpace(detay.altHesapKod))
                    {
                        if (!string.IsNullOrWhiteSpace(form.ilgiliNo))
                            detay.altHesapKod = form.ilgiliNo;
                    }
                    if (string.IsNullOrWhiteSpace(detay.altHesapKod))
                    {
                        hata += "<br>" + satir.ToString() + " nolu satırda verilen hesap no (" + detay.hesapKod + ") için İlgili bilgisi yada alt hesap giriniz (Vergi/TC Kimlik)!";
                    }
                    else
                    {
                        if (!(form.fisTur[0] == ENUMFISTUR.ACILIS || form.fisTur[0] == ENUMFISTUR.KAPANIS || form.projeNo == "KURFARKI"))
                        {
                            //!!!Kayıt edilen vno/tcno doğrumu kontrol et.
                            ObjectArray aListe = AltHesapListeHazirla(kullanan, form.muhasebe, detay.altHesapKod.Trim());
                            bulundu = false;
                            foreach (TNS.HRC.FirmaBilgisi fb in aListe.objeler)
                            {
                                if (fb.vergiNo.Trim() == detay.altHesapKod.Trim()) { bulundu = true; break; }
                            }
                            if (!bulundu)
                                hata += "<br>" + satir.ToString() + " nolu satırda verilen alt hesap kodu (" + detay.altHesapKod + ") tanımlı değil. Firma veya Personel bilgi girişinin yapılması gereklidir";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(detay.detayBirim))
                {
                    string icBirimKod = "";
                    string icKurKod = "";
                    //Eğer detaybirim 033 gibi 3 hane geldi ise kurumsal kodu sonuna ekle
                    //Bu düzeltme otomatik ödeme emri oluşturma metodlarının tek tek elden geçmemesi için yapıldı.
                    //Ama kullanıcı manual kurkod yazıyorsa ve birim kurumsal kod eklenmemiş ise aşağıdaki kurala aykırı bir durum oluşturuyor. 20.10.2020 Melih
                    if (detay.detayBirim.Length == 3 && detay.detayBirim != "000")
                        detay.detayBirim = detay.detayBirim + "." + detay.kur;
                    OrtakFonksiyonlar.StringiIkiyeBol(detay.detayBirim, ".", ref icBirimKod, ref icKurKod);
                    icKurKod = icKurKod.Replace(".", "");

                    if (icBirimKod != "000" & (icBirimKod.Length != 3 || (icBirimKod + icKurKod).Length != 11))
                    {
                        MuhasebeBirim kBirim = (MuhasebeBirim)mBirimListe[form.muhasebe.ToString() + icBirimKod + icKurKod.Replace(".", "")];
                        //Eğer kurumsal kod ile birim yoksa birim, kurum kodu ile tanımlanmış diye bakılsın
                        if (kBirim == null)
                        {
                            hata += "<br>" + satir.ToString() + " nolu satırda verilen <b>" + detay.detayBirim + "</b> birim kodu hatalı.";
                        }
                        else
                            detay.detayBirim = icBirimKod;
                    }
                    else
                    {
                        detay.detayBirim = icBirimKod;
                        if (form.birim != "000")
                        {
                            if (!TNS.MUH.Arac.DetayBirimVarMi())
                                detay.detayBirim = form.birim;
                            if (form.birim != detay.detayBirim)
                                hata += "<br>" + satir.ToString() + " nolu satırda detay birim kodu ile belgenin birim kodu farklı. Eğer belgede farklı birimler kullanılacaksa belgenin birim kodunu 000 yapın.";
                        }

                        if (!string.IsNullOrWhiteSpace(icKurKod))
                        {
                            if (!string.IsNullOrWhiteSpace(detay.kur))
                            {
                                if (!detay.kur.Replace(".", "").StartsWith(icKurKod))
                                {
                                    hata += "<br>" + satir.ToString() + " nolu satırda verilen <b>" + detay.detayBirim + "</b> birimin kurumsal kodu (" + icKurKod + ") ile tertibin kurumsal kodu (" + detay.kur + ") uyumsuz.";
                                }
                            }
                            else
                                detay.kur = icKurKod;
                        }
                    }
                }
                if (string.IsNullOrEmpty(detay.detayBirim))
                {
                    detay.detayBirim = form.birim;
                }
                //Birim zorunluluğu var mı?
                if (TNS.MUH.Arac.NitelikVarmi(detay.hKod.nitelik, (int)ENUMHesapKodNitelik.BIRIMZORUNLU))
                {
                    if (!(form.fisTur[0] == ENUMFISTUR.ACILIS || form.fisTur[0] == ENUMFISTUR.KAPANIS || form.fisTur[0] == ENUMFISTUR.SONFIS))
                    {
                        if (string.IsNullOrEmpty(detay.detayBirim) || detay.detayBirim == "000")
                            hata += "<br>" + detay.satirNo + " nolu satırda " + detay.hesapKod + " hesap kodu için harcama birimini seçiniz.";
                    }
                }
                //Hesap kodu 630, 830, 835 ise birim 000 olmasın EGO Melih
                if (ego && detay.detayBirim == "000" &&
(!(form.fisTur[0] == ENUMFISTUR.ACILIS || form.fisTur[0] == ENUMFISTUR.KAPANIS || form.fisTur[0] == ENUMFISTUR.SONFIS)))
                {
                    if (detay.hesapKod.Trim().StartsWith("630") ||
                        detay.hesapKod.Trim().StartsWith("830") ||
                        detay.hesapKod.Trim().StartsWith("835"))
                    {
                        hata += "<br>" + string.Format("{0} hesabı için birim seçiniz.<br>", detay.hKod.hesapKod);
                    }
                }
                if (ego &&
                    form.fisTur[0] == ENUMFISTUR.DONEMICI &&
                    (detay.hesapKod.Trim().StartsWith("630") ||
                    detay.hesapKod.Trim().StartsWith("830") ||
                    detay.hesapKod.Trim().StartsWith("835")))
                    oebKontrol = true;
                if (string.IsNullOrEmpty(detay.kur))
                {
                    detay.kur = form.kurKod;
                }
                if ((detay.hKod.BorcCalisirMi() || detay.hKod.AlacakCalisirMi()) && hata == "")
                {
                    if (form.fisTur[0] != ENUMFISTUR.KAPANIS && form.fisTur[0] != ENUMFISTUR.SONFIS && form.projeNo != "KURFARKI")
                    {
                        BakiyeKontrolEkle(bakiyeKontrolListesi, form, detay);
                        if (detay.hKod.MahsupYapilacakMi() && string.IsNullOrEmpty(detay.mahsupBelgeNo.Trim()))//Eğer mahsup yapılabilir ise ilgilibelgeno girilsin
                        {
                            if (detay.hKod.BorcCalisirMi() && detay.alacak > 0) //Borç bakiye ise (bakiye=borc-alacak) küçük sıfırsa
                            {
                                hata += string.Format("{0} hesabı borç çalışan bir hesaptır. Alacak alanına rakam girdiğiniz için mahsup yapılacak. Fakat mahsup yapılacak belge numarası Mahsup Belge no alanına yazılmamış<br>", detay.hKod.hesapKod);
                            }
                            else if (detay.hKod.AlacakCalisirMi() && detay.borc > 0) //Alacak bakiye ise (bakiye=borc-alacak) büyük sıfırsa
                            {
                                hata += string.Format("{0} hesabı alacak çalışan bir hesaptır. Borç alanına rakam girdiğiniz için mahsup yapılacak. Fakat mahsup yapılacak belge numarası Mahsup Belge no alanına yazılmamış<br>", detay.hKod.hesapKod);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(detay.mahsupBelgeNo))
                        {
                            double digerBelgeToplam = 0;
                            if (detay.doviz == "")
                                digerBelgeToplam = tabMIF.SatirToplamBul(form.yil, detay.mahsupBelgeNo, detay.hesapKod, detay.altHesapKod);
                            else
                                digerBelgeToplam = tabMIF.SatirToplamBulDoviz(form.yil, detay.mahsupBelgeNo, detay.hesapKod, detay.altHesapKod);

                            if (digerBelgeToplam == 0)
                            {
                                hata += string.Format("{0} hesabı {1} belgesinde bulunamamıştır. Mahsup çalışan belgelerde ilgili belgede de aynı hesap olmalıdır.<br>", detay.hesapKod, detay.mahsupBelgeNo);
                            }
                            else if (detay.alacakYPB > 0 && Math.Round(detay.alacakYPB, 2) > Math.Round(digerBelgeToplam, 2))
                            {
                                hata += string.Format("{0} hesabı {1} belgesinde {2} olarak kayıt edilmiş olup, kapatılmak istenilen tutar bundan büyüktür. ( {3} ).<br>", detay.hesapKod, detay.mahsupBelgeNo, digerBelgeToplam.ToString("#,###.00"), detay.alacak.ToString("#,###.00"));
                            }
                        }
                    }
                }
                detay.kur = detay.kur.Replace(".", "");
                detay.fon = detay.fon.Replace(".", "");
                detay.fin = detay.fin.Replace(".", "");
                detay.eko = detay.eko.Replace(".", "");
                detay.peb = detay.peb.Replace(".", "");
                Tertip detayTertip = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
                detayTertip.TertipAta(detay.kur + "-" + detay.fon + "-" + detay.fin + "-" + detay.eko + "-" + detay.peb);
                string kur = OrtakFonksiyonlar.KodNoktaKoy((string)detayTertip.Kirilimlar(0), detay.kur.Replace(".", ""));
                string fon = OrtakFonksiyonlar.KodNoktaKoy((string)detayTertip.Kirilimlar(1), detay.fon.Replace(".", ""));
                string fin = OrtakFonksiyonlar.KodNoktaKoy((string)detayTertip.Kirilimlar(2), detay.fin.Replace(".", ""));
                string eko = OrtakFonksiyonlar.KodNoktaKoy((string)detayTertip.Kirilimlar(3), detay.eko.Replace(".", ""));
                string peb = OrtakFonksiyonlar.KodNoktaKoy((string)detayTertip.Kirilimlar(4), detay.peb.Replace(".", ""));
                bool tertipKontrolEt = false;
                int ekoUz = TNS.MUH.Arac.EkonomikIkiDuzeyKontrol(ego, hesapKod);
                if (detay.hKod.tertip != 0)
                    tertipKontrolEt = true;
                if (detay.hKod.ekoBasla != 0 && tertipKontrolEt)
                {
                    string ekoFormat = (string)detayTertip.Kirilimlar(3);
                    string hesEkoKod = TNS.MUH.Arac.HesapkoddanEkonomikKodBul(form.yil, detay.hKod, ekoFormat);
                    if (ekoUz == 2)
                    {
                        int kodStrUz;
                        if (TNS.BKD.Arac.Eko23Mu(form.yil, hesEkoKod))
                        {
                            kodStrUz = detayTertip.KodUzunlukBul(3, 3, true);
                        }
                        else
                            kodStrUz = detayTertip.KodUzunlukBul(3, 2, true);
                        if (hesEkoKod.Replace(".", "").Length >= kodStrUz)//830.30.06.06.07 gibi eksik ise (3 basamak girilmiş ise) 2 basamağa indir
                            hesEkoKod = hesEkoKod.Replace(".", "").Substring(0, kodStrUz);
                    }
                    if (!string.IsNullOrWhiteSpace(hesEkoKod))
                    {
                        if (!eko.Replace(".", "").StartsWith(hesEkoKod.Replace(".", "")))
                            hata += "<br>" + satir.ToString() + " nolu satırda verilen <b>" + detay.hKod.hesapKod + "</b> hesap kodu ile ekonomik kod (" + eko + ") eşit değil.";
                    }
                }
                if (tertipKontrolEt)
                {
                    bool tertipTam = TertipTamMi(veBkd, kullanan, detay, form.yil, kt, out kodAd, out kod, ekoUz);
                    if (!tertipTam)
                    {
                        if (kod == "")
                            hata += "<br>" + satir.ToString() + " nolu satırda, " + detay.hKod.hesapKod + " hesabı için tertip girilmesi zorunluluğu vardır. Tertibi eksiksiz giriniz!";
                        else
                            hata += "<br>" + satir.ToString() + " nolu satırda, " + kodAd + " " + kod + " tanımlı değil. Geçerli bir kod giriniz!";
                    }
                }
                string kontrolBirimi = form.birim;
                if (Arac.DetayBirimVarMi())
                {
                    kontrolBirimi = detay.detayBirim;
                }
                tertiplerDizisi.Add(new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek, detay.kur, detay.fon, detay.fin, detay.eko, detay.peb));
                if (OrtakFonksiyonlar.ConvertToInt(kontrolBirimi, 0) > 0 && !form.yilSonuBelgesiMi && detayBirimKullaniyor)
                {
                    MuhasebeBirim kBirim = (MuhasebeBirim)mBirimListe[form.muhasebe.ToString() + kontrolBirimi + detay.kur.Replace(".", "")];
                    if (kBirim == null)
                    {
                        if (detay.kur.Replace(".", "").StartsWith(form.kurum.Replace(".", "")))
                            kBirim = (MuhasebeBirim)mBirimListe[form.muhasebe.ToString() + kontrolBirimi + form.kurum.Replace(".", "")];
                    }
                    if (kBirim != null)
                    {
                        if (kBirim.pasif > 0)
                            hata += "<br>" + detay.kur + " kurumsal koduna sahip " + kBirim.birimKod + " - " + kBirim.birimAd + " birimi kapalı olduğu için kullanılmaz";
                        if (!kur.Replace(".", "").StartsWith(kBirim.kurum.Replace(".", "")))
                            hata += "<br>" + satir.ToString() + " nolu satırda verilen <b>" + kur + " kurumsal kodu " + kontrolBirimi + " harcama birimine</b> ait olmadığı için kullanılamaz.";
                    }
                    else
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH089, form.muhasebe.ToString() + "-" + kontrolBirimi + "-" + detay.kur);
                }
                if (!MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMi(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, kur))
                {
                    ve.VeriErisimKapat();
                    veBkd.VeriErisimKapat();
                    eo.sonuc.DegerAta("Belgede kullandığınız kurumsal kod ile ilgili işlem yapma yetkiniz yok");
                    return eo;
                }
                if (eko.StartsWith("03") || eko.StartsWith("06"))
                {
                    EkoKoduListeyeEkle(eko, ekoKodlar);
                }
                if (!MUH.IsKurallari.MUHYetkiServis.EkonomikYetkiVarMi(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, eko))
                {
                    ve.VeriErisimKapat();
                    veBkd.VeriErisimKapat();
                    eo.sonuc.DegerAta(string.Format("Belgede kullandığınız ekonomik kod ({0}) ile ilgili işlem yapma yetkiniz yok", eko));
                    return eo;
                }
                if (!MUH.IsKurallari.MUHYetkiServis.AltFaaliyetYetkiVarMi(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, peb))
                {
                    ve.VeriErisimKapat();
                    veBkd.VeriErisimKapat();
                    eo.sonuc.DegerAta(string.Format("Belgede kullandığınız Faaliyet ({0}) ile ilgili işlem yapma yetkiniz yok", peb));
                    return eo;
                }
                if (form.yil > 2013)
                {
                    string uygunlukHata = MUHBagliEkoPebAna.UygunMu(ve, kullanan, detay);
                    if (!string.IsNullOrEmpty(uygunlukHata))
                    {
                        ve.VeriErisimKapat();
                        veBkd.VeriErisimKapat();
                        eo.sonuc.DegerAta(uygunlukHata);
                        return eo;
                    }
                }
                //**********************************************************
                if (!detay.hKod.detay)
                    hata += "<br>" + satir.ToString() + " nolu satırda verilen hesap numarası (" + detay.hKod.hesapKod + ") detay hesap numarası olmadığı için kullanılamaz. Lütfen detay hesap numaraları kullanın.";
                if (TNS.MUH.Arac.NitelikVarmi(detay.hKod.nitelik, (int)ENUMHesapKodNitelik.YANSITMAZORUNLUBORC) && detay.borc > 0 || TNS.MUH.Arac.NitelikVarmi(detay.hKod.nitelik, (int)ENUMHesapKodNitelik.YANSITMAZORUNLUALACAK) && detay.alacak > 0)
                {
                    if (form.projeNo != "KURFARKI" &&
                        form.fisTur[0] != ENUMFISTUR.KAPANIS &&
                        form.fisTur[0] != ENUMFISTUR.ACILIS &&
                        form.fisTur[0] != ENUMFISTUR.SONFIS)//kur farkı yapılıyorsa zorunlu yansıtmalara bakılmasın Melih 08.01.2017 Manas
                    {
                        ZorunluYansitmalaraEkle(zorunluYansitmalar, detay);
                    }
                }
            }
            if (form.fisTur[0] != ENUMFISTUR.KAPANIS)
                hata += YansitmaTutarKontrolu(form.detay);
            //if (oebKontrol)
            //{
            //    if (string.IsNullOrWhiteSpace(form.ilgiliBankaNo.Trim()))
            //        hata += "<br>" + string.Format("İlgilinin IBAN bilgisini yazınız.");
            //    if (string.IsNullOrWhiteSpace(form.OEBNo.Trim()))
            //        hata += "<br>" + string.Format("Ödemeye esas belge no bilgisini giriniz.");
            //    if (string.IsNullOrWhiteSpace(form.OEBTur.Trim()))
            //        hata += "<br>" + string.Format("Ödemeye esas belge türü bilgisini giriniz.");
            //    if (form.OEBTutar == 0)
            //        hata += "<br>" + string.Format("Ödemeye esas belge tutar bilgisini giriniz.");
            //    if (form.OEBTarih.isNull)
            //        hata += "<br>" + string.Format("Ödemeye esas belge tarih bilgisini giriniz.");
            //}
            hata += MalAlimiIhaleKoduIsaretiKontrol(ve, kullanan, form, ekoKodlar, tertiplerDizisi);
            if (hata == "")
            {
                foreach (OdemeEmriMIFDetay detay in bakiyeKontrolListesi)
                {
                    hata += BorcAlacakKontrol(ve, kullanan, form, detay, ortak000);
                }
                if (zorunluYansitmalar.Count > 0)
                {
                    HesapPlaniYansitma yansitma = new HesapPlaniYansitma();
                    yansitma.yil = form.yil;
                    yansitma.hesapKod = "";
                    foreach (OdemeEmriMIFDetay item in zorunluYansitmalar)
                    {
                        if (yansitma.hesapKod != "") yansitma.hesapKod += ",";
                        yansitma.hesapKod += item.hesapKod;
                    }
                    ObjectArray oa = MUHYansitmaServis.Listele(ve, yansitma, true);
                    //Birinci kontrol
                    string zhata = ZorunluYansitmaKontrolu(form, oa, zorunluYansitmalar);
                    if (zhata != "")
                    {
                        //Zorunlu yansıtmalardan yapabildiklerini otomatik ekle
                        List<OdemeEmriMIFDetay> yeniEklenecek = new List<OdemeEmriMIFDetay>();
                        foreach (HesapPlaniYansitma gelenYansitma in oa.objeler)
                        {
                            foreach (OdemeEmriMIFDetay od in form.detay.objeler)
                            {
                                if (od.hesapKod.Replace(".", "") == gelenYansitma.hesapKod.Replace(".", ""))
                                {
                                    OdemeEmriMIFDetay ody = new OdemeEmriMIFDetay(od);
                                    ody.hesapKod = gelenYansitma.yansitmaHesapKod;
                                    //EGO zorunlu yansıtmada (140 hesabı) alt hesap bilgisi de yansıtmada gözüküyordu. 18.10.2019
                                    ody.kesinti = 0;
                                    ody.altHesapKod = "";
                                    if (gelenYansitma.yansitmaHesapKod.StartsWith("800"))
                                        ody.TertipAta("");
                                    //************************************
                                    yeniEklenecek.Add(ody);
                                    ody = new OdemeEmriMIFDetay(od.yil);
                                    if (gelenYansitma.yansitmaHesapKod.StartsWith("800"))
                                        ody.hesapKod = "805";
                                    else
                                        ody.hesapKod = "835";
                                    ody.borc = od.alacak;
                                    ody.alacak = od.borc;
                                    yeniEklenecek.Add(ody);
                                }
                            }
                        }
                        foreach (OdemeEmriMIFDetay ody in yeniEklenecek)
                        {
                            form.detay.objeler.Add(ody);
                        }
                        //İkinci kontrol
                        zhata = ZorunluYansitmaKontrolu(form, oa, zorunluYansitmalar);
                        if (zhata != "")
                            hata += zhata;
                    }
                }
            }
            //*****************************************************************************************
            //Eğer döviz ile çalışılıyorsa kur farkı olabilir. Onu otomatik yapmak için
            //BelgeyiMetinDosyasinaYaz(form);
            bool dovizliIslem = false;
            foreach (OdemeEmriMIFDetay d in form.detay.objeler)
            {
                if (d.doviz != "") { dovizliIslem = true; break; }
            }
            decimal fark = OrtakFonksiyonlar.KurusYap(Math.Round(toplamBorc, 4) - Math.Round(toplamAlacak, 4));
            decimal farkAtama = Math.Round(Math.Abs(fark), 2);
            if (dovizliIslem && farkAtama > 0 && farkAtama < otomatikKurFarki.deger)
            {
                TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new TNS.MUH.OdemeEmriMIFDetay(form.yil);
                oEmriDetay.detayBirim = form.birim;
                oEmriDetay.kur = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIKURUMSALKOD") + "";
                oEmriDetay.kur = oEmriDetay.kur.Replace(".", "");
                if (fark > 0)
                {
                    oEmriDetay.hesapKod = otomatikKurFarki.alacakHesap; // "600.11.01.02.02";
                    oEmriDetay.alacak = OrtakFonksiyonlar.ConvertToDbl(farkAtama);
                    toplamAlacak += farkAtama;
                }
                else if (fark < 0)
                {
                    oEmriDetay.hesapKod = otomatikKurFarki.borcHesap; // "630.11.01.05.02";
                    oEmriDetay.borc = OrtakFonksiyonlar.ConvertToDbl(farkAtama);
                    toplamBorc += farkAtama;
                }
                if (!string.IsNullOrEmpty(oEmriDetay.hesapKod))
                {
                    form.detay.objeler.Add(oEmriDetay);
                }
            }
            //BelgeyiMetinDosyasinaYaz(form);
            if (decimal.Compare(Math.Round(toplamBorc, 4), Math.Round(toplamAlacak, 4)) != 0)
                hata += "<br>Servis Kontrol Borç :" + Math.Round(toplamBorc, 4) + " ve Alacak " + Math.Round(toplamAlacak, 4) + " toplam tutarı eşit değil.";
            if (!string.IsNullOrWhiteSpace(form.OEBNo) && !TNSDateTime.IsNull(form.OEBTarih))
            {
                string faturaKayitli = FaturaKayitlimi(ve, form.yil, form.OEBNo.Trim(), form.OEBTarih, form.muhasebe, form.belgeNo, (int)form.belgeTur, form.ilgiliNo);
                if (faturaKayitli != "")
                    hata += "<br>Yazmış olduğunuz fatura bilgileri " + faturaKayitli + " nolu belge ile daha önce kayıt edilmiştir. Lütfen kontrol edin.";
            }
            if (form.odenekKontroluYapma != 9 && form.fisTur[0] != ENUMFISTUR.ACILIS)
            {
                ENUMOdenekUstuKontrol ouKontrol = Arac.OdenekUstuKontrolBul();
                System.Collections.Generic.List<OdemeEmriMIFDetay> avanslar = form.AvansKapatmaTertipleriBul(eko2Uz, eko3Uz);
                if (hata != "")
                {
                    eo.sonuc.DegerAta(hata);
                    ve.VeriErisimKapat();
                    veBkd.VeriErisimKapat();
                    return eo;
                }
                ENUMOdenekKontrolTur ogKontrolTur = Arac.OdenekKontrolTurBul(form.yil);
                bool izBedelGerekli = Arac.IzBedelGerekliBul(form.yil);
                System.Collections.Generic.List<OdemeEmriMIFDetay> tertipler = new List<OdemeEmriMIFDetay>();
                System.Collections.Generic.List<OdemeEmriMIFDetay> kontrolsuzTertipler = new List<OdemeEmriMIFDetay>();
                form.OdenekKontroluTertipleriBul(ogKontrolTur, true, tertipler, kontrolsuzTertipler);
                OdenekKontrol(ve, kullanan, form, tertipler, ouKontrol, eo, true, false, false);
                if (ouKontrol == ENUMOdenekUstuKontrol.ODENEKGONDERME)
                {
                    OdenekKontrol(ve, kullanan, form, kontrolsuzTertipler, ouKontrol, eoKontrolsuz, true, true, false);
                    if (izBedelGerekli)
                    {
                        foreach (EksikOdenekBilgi item in eoKontrolsuz.eksikOdenekler)
                        {
                            if (item.ODENEK == 0 && (item.eko.StartsWith("01") || item.eko.StartsWith("02")))
                            {
                                eo.sonuc.DegerAta(item.TertipStr() + " tertibi ile ilgili biriminize hiç ödenek gönderilmediğinden kullanamazsınız. Bu tertipden ödenek üstü harcama yapabilmeniz için en az bir kez ödenek gönderilmesi gerekmektedir.");
                            }
                        }
                    }
                }
            }
            Sonuc sonuc = new MUHHesapPlaniKisit().MuhasebeBelgesiKontrol(ve, kullanan, form);
            if (!sonuc.islemSonuc)
            {
                eo.sonuc.DegerAta(sonuc.hataStr);
                ve.VeriErisimKapat();
                veBkd.VeriErisimKapat();
                return eo;
            }
            ve.VeriErisimKapat();
            veBkd.VeriErisimKapat();
            return eo;
        }
        private void DebugYaz(string v)
        {
            OrtakFonksiyonlar.HataDosyaYaz("MIFTertip.txt", v, false);
        }
        private string ZorunluYansitmaKontrolu(OdemeEmriMIFForm form, ObjectArray oa, List<OdemeEmriMIFDetay> zorunluYansitmalar)
        {
            if (form.fisTur[0] == ENUMFISTUR.AMORTISMANKAPANISI)
            {
                return "";
            }
            string hata = "";
            foreach (OdemeEmriMIFDetay item in zorunluYansitmalar)
            {
                foreach (HesapPlaniYansitma gelenYansitma in oa.objeler)
                {
                    gelenYansitma.yansitmaHesapKod = gelenYansitma.yansitmaHesapKod.Replace(".", "");
                    if (item.hesapKod.Replace(".", "") != gelenYansitma.hesapKod.Replace(".", "")) continue;
                    int detaySatirNo = -1;
                    while (item.BorcAlacakNet != 0)
                    {
                        OdemeEmriMIFDetay yansitilan = form.SatirBul(gelenYansitma.yansitmaHesapKod, item.detayBirim, item.TertipStr(), ref detaySatirNo);
                        if (yansitilan == null) break;
                        if (item.borc > 0)
                        {
                            if (yansitilan.kalanBorc > 0)
                            {
                                double tut = 0;
                                if (item.borc >= yansitilan.kalanBorc)
                                {
                                    tut = yansitilan.kalanBorc;
                                }
                                else
                                {
                                    tut = item.borc;
                                }
                                item.borc -= tut;
                                yansitilan.dengeBorc += tut;
                            }
                            else
                                break;
                        }
                        else
                        {
                            if (yansitilan.kalanAlacak > 0)
                            {
                                double tut = 0;
                                if (item.alacak >= yansitilan.kalanAlacak)
                                {
                                    tut = yansitilan.kalanAlacak;
                                }
                                else
                                {
                                    tut = item.alacak;
                                }
                                item.alacak -= tut;
                                yansitilan.dengeAlacak += tut;
                            }
                            else
                                break;
                        }
                    }
                }
                if (item.BorcAlacakNet != 0)
                {
                    hata = String.Format("<br>Yansıtma hatası: Zorunlu olarak yansıtılması gereken {0} hesabının yansıtılmamış olan tutarı= {1}!", item.hesapKod, Math.Abs(item.BorcAlacakNet));
                }
            }
            return hata;
        }
        private void DetayDebugYaz(OdemeEmriMIFForm form)
        {
            int sayac = 0;
            foreach (OdemeEmriMIFDetay item in form.detay.objeler)
            {
                sayac++;
                OrtakFonksiyonlar.HataDosyaYaz($"Kontrol{DateTime.Now.Year}{DateTime.Now.Month.ToString("00")}.txt", sayac + ". hesapKod" + item.hesapKod + "*Tertip:" + item.TertipStr() + "*borc:" + item.borc + "*alacak:" + item.alacak + "*borcYPB:" + item.borcYPB + "*alacakYPB:" + item.alacakYPB, false);
            }
        }
        static OtomatikKurFarki _otomatikKurFarki = null;
        static OtomatikKurFarki otomatikKurFarki
        {
            get
            {
                if (_otomatikKurFarki == null)
                {
                    _otomatikKurFarki = new OtomatikKurFarki(OrtakFonksiyonlar.ConvertToDecimal(TNS.MUH.Arac.DegiskenDegerBul(0, "OtomatikKurFarkDeger")), TNS.MUH.Arac.DegiskenDegerBul(0, "OtomatikKurBorcHesap"), TNS.MUH.Arac.DegiskenDegerBul(0, "OtomatikKurAlacakHesap"));
                }
                return _otomatikKurFarki;
            }
        }
        class OtomatikKurFarki
        {
            public decimal deger = 0;
            public string borcHesap = null;
            public string alacakHesap = null;
            public OtomatikKurFarki(decimal deger, string borcHesap, string alacakHesap)
            {
                this.deger = deger / 100;
                this.borcHesap = borcHesap;
                this.alacakHesap = alacakHesap;
            }
        }
        public bool TertipTamMi(VeriErisim veriErisim, Kullanici kullanan, OdemeEmriMIFDetay detay, int yil, ENUMOdenekKontrolTur kt, out string kodAd, out string kod, int ekoUz = 0)
        {
            kodAd = "";
            kod = "";
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, detay.kur, detay.fon, detay.fin, detay.eko, detay.peb);
            if (!t.KodTamMi(0) || !t.KodTamMi(1) || !t.KodTamMi(2)) return false;
            if (ekoUz == 0)
            {
                if (!t.KodTamMi(3)) return false;
            }
            else
            {
                if (!t.KodTamMi(3, ekoUz)) return false;
            }
            if (kt != ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
                if (!t.KodTamMi(4)) return false;
            for (int i = 0; i < t.kodlar.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(t.kodlar[i]))
                {
                    t.adlar[i] = MUHServis.UzayYaziDegerGetir(veriErisim, kullanan, t.UzayAdBul(i), yil.ToString() + "-" + t.kodlar[i], true, "");
                }
            }
            if (!t.IsimlerTamMi(out kodAd, out kod))
            {
                if (string.IsNullOrWhiteSpace(kod))
                    return true;
                return false;
            }
            return true;
        }
        private string YansitmaTutarKontrolu(ObjectArray detay)
        {
            Hashtable yansitma800Toplam = new Hashtable();
            Hashtable yansitma805Toplam = new Hashtable();
            Hashtable yansitma830Toplam = new Hashtable();
            Hashtable yansitma835Toplam = new Hashtable();
            string hata = "";
            double toplam800 = 0.0;
            double toplam805 = 0.0;
            double toplam830 = 0.0;
            double toplam835 = 0.0;
            foreach (OdemeEmriMIFDetay mifDetay in detay.objeler)
            {
                //Yansıtma tutar kontrolü
                if (mifDetay.hesapKod.StartsWith("800") || mifDetay.hesapKod.StartsWith("810"))
                {
                    toplam800 += OrtakFonksiyonlar.ConvertToDouble(mifDetay.alacak - mifDetay.borc);//a-b
                    double yansitma800ToplamDBL = OrtakFonksiyonlar.ConvertToDouble(yansitma800Toplam[mifDetay.detayBirim]);
                    yansitma800Toplam[mifDetay.detayBirim] = yansitma800ToplamDBL + OrtakFonksiyonlar.ConvertToDouble(mifDetay.alacak - mifDetay.borc);
                }
                if (mifDetay.hesapKod.StartsWith("830") || mifDetay.hesapKod.StartsWith("831"))
                {
                    toplam830 += OrtakFonksiyonlar.ConvertToDouble(mifDetay.borc - mifDetay.alacak);//b-a
                    double yansitma830ToplamDBL = OrtakFonksiyonlar.ConvertToDouble(yansitma830Toplam[mifDetay.detayBirim]);
                    yansitma830Toplam[mifDetay.detayBirim] = yansitma830ToplamDBL + OrtakFonksiyonlar.ConvertToDouble(mifDetay.borc - mifDetay.alacak);
                }
                if (mifDetay.hesapKod.StartsWith("805") || mifDetay.hesapKod.StartsWith("895"))
                {
                    toplam805 += OrtakFonksiyonlar.ConvertToDouble(mifDetay.borc - mifDetay.alacak);//b-a
                    double yansitma805ToplamDBL = OrtakFonksiyonlar.ConvertToDouble(yansitma805Toplam[mifDetay.detayBirim]);
                    yansitma805Toplam[mifDetay.detayBirim] = yansitma805ToplamDBL + OrtakFonksiyonlar.ConvertToDouble(mifDetay.borc - mifDetay.alacak);
                }
                if (mifDetay.hesapKod.StartsWith("835"))
                {
                    toplam835 += OrtakFonksiyonlar.ConvertToDouble(mifDetay.alacak - mifDetay.borc);//a-b
                    double yansitma835ToplamDBL = OrtakFonksiyonlar.ConvertToDouble(yansitma835Toplam[mifDetay.detayBirim]);
                    yansitma835Toplam[mifDetay.detayBirim] = yansitma835ToplamDBL + OrtakFonksiyonlar.ConvertToDouble(mifDetay.alacak - mifDetay.borc);
                }
            }
            toplam800 = Math.Round(toplam800, 2);
            toplam805 = Math.Round(toplam805, 2);
            toplam830 = Math.Round(toplam830, 2);
            toplam835 = Math.Round(toplam835, 2);

            foreach (string anahtar in yansitma800Toplam.Keys)
            {
                double tutar800 = Math.Round(OrtakFonksiyonlar.ConvertToDouble(yansitma800Toplam[anahtar]), 2);
                double tutar805 = Math.Round(OrtakFonksiyonlar.ConvertToDouble(yansitma805Toplam[anahtar]), 2);
                if (tutar800 != tutar805)
                    hata += "<br>" + anahtar + " birimine ait <b>800</b> (" + tutar800.ToString("#,###.00") + ") ile <b>805</b> (" + tutar805.ToString("#,###.00") + ") toplamları eşit değil";
            }
            foreach (string anahtar in yansitma830Toplam.Keys)
            {
                double tutar830 = Math.Round(OrtakFonksiyonlar.ConvertToDouble(yansitma830Toplam[anahtar]), 2);
                double tutar835 = Math.Round(OrtakFonksiyonlar.ConvertToDouble(yansitma835Toplam[anahtar]), 2);
                if (tutar830 != tutar835)
                    hata += "<br>" + anahtar + " birimine ait <b>830/831</b> (" + tutar830.ToString("#,###.00") + ") ile <b>835</b> (" + tutar835.ToString("#,###.00") + ") toplamları eşit değil";
            }
            if (toplam800 != toplam805)
                hata += "<br>" + "<b>800</b> (" + toplam800.ToString("#,###.00") + ") ile <b>805</b> (" + toplam805.ToString("#,###.00") + ") genel toplamları eşit değil";
            if (toplam830 != toplam835)
                hata += "<br>" + "<b>830/831</b> (" + toplam830.ToString("#,###.00") + ") ile <b>835</b> (" + toplam835.ToString("#,###.00") + ") genel toplamları eşit değil";
            return hata;
        }
        private string MalAlimiIhaleKoduIsaretiKontrol(VeriErisim veriErisim, Kullanici kullanan, OdemeEmriMIFForm form, List<string> ekoKodlar, TNSCollection tertiplerDizisi)
        {
            try//Özelleştirme İdaresinde ButceHazirlik Vtsi yok. Hata verdiği için Melih 18.09.2014
            {
                string kod = TNS.MUH.Arac.IhaleKodBul(form.ihale);
                string tur = TNS.MUH.Arac.IhaleAlimturBul(form.ihale);
                Sonuc s = servisHRC.YuzdeOnAcikMi(kullanan, form.yil, kod, tur, tertiplerDizisi, form.muhasebe, form.birim);
                if (!s.islemSonuc)
                    return s.hataStr;
                if (!string.IsNullOrWhiteSpace(form.surecNo)) return "";
                string tirnakli = Tirnakla(ekoKodlar);
                if (!string.IsNullOrWhiteSpace(tirnakli))
                {
                    string sql = string.Format("SELECT COUNT(*) FROM " + TABOrtak.DatabaseAdBul("ButceHazirlikV2.dbo.") + "EKO_MHY where EKOKOD IN ({0}) AND IHALEZORUNLU=1", tirnakli);
                    double say = veriErisim.TekKolonSQLDbl(sql);
                    if (say >= 0.5)
                    {
                        if (string.IsNullOrWhiteSpace(kod) || string.IsNullOrWhiteSpace(tur))
                            return "Ekonomik kod olarak alım tertipleri (03, 06) girildiği için alım yöntemini de girmelisiniz. Giriş ekranında Proje bölümüne geçip, İhale maddesi ve alım türü alanlarını doldurunuz!";
                    }
                }
            }
            catch { }
            veriErisim.hata = "";
            return "";
        }
        private string Tirnakla(List<string> ekoKodlar)
        {
            string retVal = "";
            for (int i = 0; i < ekoKodlar.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(ekoKodlar[i]))
                {
                    if (retVal != "") retVal += ",";
                    retVal += "'" + ekoKodlar[i].Replace(".", "") + "'";
                }
            }
            return retVal;
            //"'061305','061390'"
            //throw new NotImplementedException();
        }
        private void EkoKoduListeyeEkle(string eko, List<string> ekoKodlar)
        {
            if (!ekoKodlar.Contains(eko))
                ekoKodlar.Add(eko);
        }
        private void BakiyeKontrolEkle(List<OdemeEmriMIFDetay> bakiyeKontrolListesi, OdemeEmriMIFForm form, OdemeEmriMIFDetay gelen)
        {
            string kur = "";
            string fon = "";
            string fin = "";
            string eko = "";
            string peb = "";
            if (gelen.hKod.tertip > 0)
            {
                kur = gelen.kur;
                fon = gelen.fon;
                fin = gelen.fin;
                eko = gelen.eko;
                peb = gelen.peb;
            }
            string altHesapKod = "";
            if (gelen.hKod.altHesapKod > 0)
                altHesapKod = gelen.altHesapKod;
            string detayBirim = gelen.detayBirim;
            if (detayBirim.Length > 3) detayBirim = detayBirim.Substring(0, 3);
            if (TNS.MUH.Arac.NitelikVarmi(gelen.hKod.nitelik, (int)ENUMHesapKodNitelik.SAYMANLIKBAZDABAKIYE))
                detayBirim = "";
            foreach (OdemeEmriMIFDetay eski in bakiyeKontrolListesi)
            {
                if (eski.hesapKod == gelen.hesapKod && eski.altHesapKod == altHesapKod && eski.kur == kur && eski.fon == fon && eski.fin == fin && eski.eko == eko && eski.peb == peb && eski.detayBirim == detayBirim)//eski.mahsupBelgeNo == mahsupBelgeNo && 
                {
                    eski.borc += gelen.borc;
                    eski.alacak += gelen.alacak;
                    return;
                }
            }
            OdemeEmriMIFDetay eklenecek = new OdemeEmriMIFDetay(gelen);
            eklenecek.altHesapKod = altHesapKod;
            eklenecek.kur = kur;
            eklenecek.fon = fon;
            eklenecek.fin = fin;
            eklenecek.eko = eko;
            eklenecek.peb = peb;
            eklenecek.detayBirim = detayBirim;
            bakiyeKontrolListesi.Add(eklenecek);
        }
        private void ZorunluYansitmalaraEkle(List<OdemeEmriMIFDetay> zorunluYansitmalar, OdemeEmriMIFDetay gelen)
        {
            foreach (OdemeEmriMIFDetay eski in zorunluYansitmalar)
            {
                if (eski.hesapKod == gelen.hesapKod && eski.TertipStr() == gelen.TertipStr() && eski.detayBirim == gelen.detayBirim)
                {
                    eski.borc += gelen.borc;
                    eski.alacak += gelen.alacak;
                    return;
                }
            }
            OdemeEmriMIFDetay d = new OdemeEmriMIFDetay(gelen.yil, gelen.hesapKod, gelen.borc, gelen.alacak);
            d.TertipAta(gelen.TertipStr());
            d.detayBirim = gelen.detayBirim;
            zorunluYansitmalar.Add(d);
        }
        private string BorcAlacakKontrol(VeriErisim ve, Kullanici kullanan, OdemeEmriMIFForm form, OdemeEmriMIFDetay satir, string ortak000)
        {
            satir.BorcAlacakAyarla();
            if (satir.hKod.bakiye == 1 && satir.alacak > 0 || satir.hKod.bakiye == 2 && satir.borc > 0)
            { //Borç Bakiye ise alacak büyük sıfırsa,      || Alacak   bakiye ise borç büyük sıfırsa
                double bakiyeGenel = HesapKodBakiyeBul(ve, kullanan, form, satir, ortak000);
                double bakiyeSatir = Math.Round(satir.borcYPB - satir.alacakYPB, 2);//döviz ise döviz rakamı gelir değilse kullanılan para biriminden gelir
                bakiyeGenel += bakiyeSatir;
                bakiyeGenel = Math.Round(bakiyeGenel, 2);
                if (satir.hKod.bakiye == 1 && bakiyeSatir < 0) //Borç bakiye ise (bakiye=borc-alacak) küçük sıfırsa
                {
                    if (bakiyeGenel < 0)
                        return string.Format("{0}-{1}-{2}-{3} hesabı borç çalışırken, yapılan işlemle alacak bakiye verecek hale gelmektedir. Bakiye Genel:{4} * Satir Bakiye:{5}<br>", satir.detayBirim, satir.hKod.hesapKod, satir.altHesapKod + "-" + satir.TertipStr(), satir.hKod.aciklama, bakiyeGenel.ToString("#,###.00"), bakiyeSatir.ToString("#,###.00"));
                }
                else if (satir.hKod.bakiye == 2 && bakiyeSatir > 0) //Alacak bakiye ise (bakiye=borc-alacak) büyük sıfırsa
                {
                    if (bakiyeGenel > 0)
                        return string.Format("{0}-{1}-{2}-{3} hesabı alacak çalışırken, yapılan işlemle borç bakiye verecek hale gelmektedir. Bakiye Genel:{4} * Satir Bakiye:{5}<br>", satir.detayBirim, satir.hKod.hesapKod, satir.altHesapKod + "-" + satir.TertipStr(), satir.hKod.aciklama, bakiyeGenel.ToString("#,###.00"), bakiyeSatir.ToString("#,###.00"));
                }
            }
            return "";
        }
        private void OdenekKontrol(VeriErisim ve, Kullanici kullanan, OdemeEmriMIFForm form, System.Collections.Generic.List<OdemeEmriMIFDetay> tertipler, ENUMOdenekUstuKontrol ouKontrol, EksikOdenekler eo, bool odemeEmrimi, bool kontrolsuzTertipler, bool dogrudanTeminMi)
        {
            List<EksikOdenekBilgi> l = new List<EksikOdenekBilgi>();
            string birim = form.birim;
            string ortak000 = TNS.MUH.Arac.DegiskenDegerBul(form.yil, "/Ortak000");
            if (ortak000 == "1")
                birim = "";
            try
            {
                IBHMv2Servis bhmServis = TNS.BHMv2.Arac.Tanimla();
                int mpDurum = bhmServis.MasrafPlaniGenelOnayDurumBul(kullanan, form.yil, "", BHMv2.ENUMMasrafPlanAsama.AHP);
                bool MPKontrol = mpDurum == 5;
                foreach (OdemeEmriMIFDetay item in tertipler)
                {
                    string surecNo = form.surecNo;
                    EksikOdenekBilgi eob = OdenekTalepYapilacakMi2(ve, kullanan, form.yil, form.muhasebe, birim, item.kur, item.fon, item.fin, item.eko, item.borc, item.peb, form.belgeNo, form.surecYil, surecNo, ouKontrol, MPKontrol, kontrolsuzTertipler, odemeEmrimi, dogrudanTeminMi);
                    if (eob.IHTIYAC > 0)
                    {
                        //if (kontrolsuzTertipler && (item.eko.StartsWith("01") || item.eko.StartsWith("02"))) continue;
                        eob.kurKont = item.kur;
                        eob.fonKont = item.fon;
                        eob.finKont = item.fin;
                        eob.ekoKont = item.eko;
                        eob.pebKont = item.peb;
                        string[] parcala = item.aciklama.Split('-');
                        eob.kur = TNS.MUH.Arac.DiziElemanAl(parcala, 0, "");
                        eob.fon = TNS.MUH.Arac.DiziElemanAl(parcala, 1, "");
                        eob.fin = TNS.MUH.Arac.DiziElemanAl(parcala, 2, "");
                        eob.eko = TNS.MUH.Arac.DiziElemanAl(parcala, 3, "");
                        eob.peb = TNS.MUH.Arac.DiziElemanAl(parcala, 4, "");
                        l.Add(eob);
                    }
                }
            }
            catch { }
            eo.eksikOdenekler = l.ToArray();
            EksikOdenekSonucIsle(eo);
        }
        private void EksikOdenekSonucIsle(EksikOdenekler eo)
        {
            eo.sonuc = new Sonuc(true, "", "");
            if (eo.eksikOdenekler.Length > 0)
            {
                string odenekUstuIzni = System.Configuration.ConfigurationManager.AppSettings.Get("OdenekUstuHarcamaIzni");
                if (OrtakFonksiyonlar.ConvertToInt(odenekUstuIzni, 0) == 0)
                {
                    eo.sonuc.DegerAta("<br>Ödenek üstü harcama yapılacağından işlem iptal edildi.<br>" + eo.BilgiStr());
                    eo.sonuc.bilgiStr = "";
                }
                else
                {
                    eo.sonuc.bilgiStr = "Ödenek üstü harcama yapılıyor, sistem kayıt yapılmasına izin verecek. Fakat aşağıdaki tertipleri düzeltmeniz gerekmektedir!" + eo.BilgiStr();
                }
            }
        }
        /// <summary>
        /// Hesaps the kod bakiye bul.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <param name="detay">The detay.</param>
        /// <param name="ortak000">The ortak000.</param>
        /// <returns></returns>
        public static double HesapKodBakiyeBul(VeriErisim ve, Kullanici kullanan, OdemeEmriMIFForm form, OdemeEmriMIFDetay detay, string ortak000)
        {
            //İŞARET İÇİN KONULDU
            string birim = detay.detayBirim;//Detay bazda bakılması gerekiyor. Melih 12.05.2014 TKDK ile yapılan çalışma sonucunda
            //if (ortak000 == "1")
            //    birim = "";
            string kur = detay.kur;
            string fon = detay.fon;
            string fin = detay.fin;
            string eko = detay.eko;
            string peb = detay.peb;
            string altHesapKod = detay.altHesapKod;
            //EGO ise althesap varsa hem birime hemde genele baksın Melih 04.03.2021
            if (form.kurum.Replace(".", "") == "460604" && altHesapKod != "" && birim != "000")
            {
                if (birim != "") birim += ",000";
            }
            if (TNS.MUH.Arac.NitelikVarmi(detay.hKod.nitelik, (int)ENUMHesapKodNitelik.SAYMANLIKBAZDABAKIYE))
                birim = "";
            string uzayAdi = "MUHODEMEEMRIMIF";
            if (detay.hKod.dovizKurKod != "")
                uzayAdi = "MUHODEMEEMRIMIFDOVIZ";
            string uzayKosul = form.yil.ToString() + "-" + form.muhasebe + "-" + birim + "-" + detay.hesapKod + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-1/8-" + peb + "-----" + altHesapKod;
            double bakiye = MUHServis.UzaySayiDegerGetir(ve, kullanan, uzayAdi, uzayKosul, false, "");
            //2018-1-008-3300702-----1/8------150109003 Sonuc Bakiye:2500
            //OrtakFonksiyonlar.HataDosyaYaz("DovizMIF.txt", uzayKosul + " Sonuc Bakiye:" + bakiye, false);
            //Şu anda kayıt edilmek istenilen belge daha önce kayıt edildi ise
            if (!string.IsNullOrWhiteSpace(form.belgeNo))
            {
                uzayKosul = form.yil.ToString() + "-" + "" + "-" + (TABOdemeEmriMIF.ANABIRIM ? "" : birim) + "-" + detay.hesapKod + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-1/8-" + peb + "----" + form.belgeNo + "-" + altHesapKod;
                double belgeBakiye = MUHServis.UzaySayiDegerGetir(ve, kullanan, uzayAdi, uzayKosul, false, "");
                bakiye -= belgeBakiye;
                //2018--008-3300702-----1/8-----1399000042-150109003 Sonuc Bakiye:0 Belge Bakiye:2500
                //OrtakFonksiyonlar.HataDosyaYaz("DovizMIF.txt", uzayKosul + " Sonuc Bakiye:" + bakiye + " Belge Bakiye:" + belgeBakiye, false);
            }
            //OrtakFonksiyonlar.HataDosyaYaz("DovizMIF.txt", "Donen Bakiye:" + bakiye, false);
            return bakiye;
        }
        /// <summary>
        /// Tertips the durum bul.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="tertip">The tertip.</param>
        /// <param name="odemeEmriBelgeNo">The odeme emri belge no.</param>
        /// <param name="surecYil">The surec yil.</param>
        /// <param name="surecNo">The surec no.</param>
        /// <param name="talep">if set to <c>true</c> [talep].</param>
        /// <returns></returns>
        public EksikOdenekBilgi TertipDurumBul(VeriErisim ve, Kullanici kullanan, int yil, int muhasebe, string birim, string tertip, string odemeEmriBelgeNo, int surecYil, string surecNo, bool talep)
        {
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);
            int eko3Uz = tUzBul.KodUzunlukBul(3, 3, true);
            ENUMOdenekUstuKontrol ouKontrol = Arac.OdenekUstuKontrolBul();
            ENUMOdenekKontrolTur ogKontrolTur = Arac.OdenekKontrolTurBul(yil);
            System.Collections.Generic.List<OdemeEmriMIFDetay> liste = new System.Collections.Generic.List<OdemeEmriMIFDetay>();
            OdemeEmriMIFDetay item = new OdemeEmriMIFDetay(yil);
            string[] tertipParca = tertip.Replace(".", "").Split('-');
            item.kur = Arac.DiziElemanAl(tertipParca, 0, "");
            item.fon = Arac.DiziElemanAl(tertipParca, 1, "");
            item.fin = Arac.DiziElemanAl(tertipParca, 2, "");
            item.eko = Arac.DiziElemanAl(tertipParca, 3, "");
            item.peb = Arac.DiziElemanAl(tertipParca, 4, "");
            OdemeEmriMIFForm.KontrolListesineEkle(yil, item, liste, ogKontrolTur, talep, eko2Uz, eko3Uz);
            item = liste[0];
            EksikOdenekBilgi eob = OdenekTalepYapilacakMi2(ve, kullanan, yil, muhasebe, birim, item.kur, item.fon, item.fin, item.eko, 0, item.peb, odemeEmriBelgeNo, surecYil, surecNo, ouKontrol, false, false, false, false);
            return eob;
        }
        /// <summary>
        /// Odeneks the talep yapilacak mi2.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="kur">The kur.</param>
        /// <param name="fon">The fon.</param>
        /// <param name="fin">The fin.</param>
        /// <param name="eko">The eko.</param>
        /// <param name="borc">The borc.</param>
        /// <param name="peb">The peb.</param>
        /// <param name="oeNo">The oe no.</param>
        /// <param name="surecYil">The surec yil.</param>
        /// <param name="surecNo">The surec no.</param>
        /// <param name="ouKontrol">The ou kontrol.</param>
        /// <param name="MPKontrol">if set to <c>true</c> [mp kontrol].</param>
        /// <param name="kontrolsuzTertipler">if set to <c>true</c> [kontrolsuz tertipler].</param>
        /// <param name="odemeEmrimi">if set to <c>true</c> [odeme emrimi].</param>
        /// <param name="dogrudanTeminMi">if set to <c>true</c> [dogrudan temin mi].</param>
        /// <returns></returns>
        public static EksikOdenekBilgi OdenekTalepYapilacakMi2(VeriErisim ve, Kullanici kullanan, int yil, int muhasebeInt, string birim, string kur, string fon, string fin, string eko, double borc, string peb, string oeNo, int surecYil, string surecNo, ENUMOdenekUstuKontrol ouKontrol, bool MPKontrol, bool kontrolsuzTertipler, bool odemeEmrimi, bool dogrudanTeminMi)
        {
            EksikOdenekBilgi eob = new EksikOdenekBilgi();
            eob.kur = kur;
            eob.fon = fon;
            eob.fin = fin;
            eob.eko = eko;
            eob.peb = peb;
            eob.borc = borc;
            string muhasebe = muhasebeInt.ToString();
            if (MPKontrol && !odemeEmrimi && !dogrudanTeminMi)
                eob.ODENEK = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MPGMASRAFPLANIBIRIM", yil.ToString() + "-5-" + muhasebe + "-" + birim + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb + "---5", false, "");
            else if (ouKontrol == ENUMOdenekUstuKontrol.ODENEK)// veya ODENEKGONDERME
            {
                eob.ODENEK = MUHServis.UzaySayiDegerGetir(ve, kullanan, "ABSBUTCEGIDER", yil.ToString() + "-" + kur + "-" + fon + "-" + fin + "-" + eko, false, "");
                peb = "";
                muhasebe = "";
                birim = "";
            }
            else
            {
                double tenkis = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", yil.ToString() + "-4---" + muhasebe + "-" + birim + "--0/6-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb, false, "");
                string durum = "5";
                if (kontrolsuzTertipler)
                {
                    durum = "0/6,99";
                }
                //string sql = MUHServis.servisUZY.UzayKosulStr(kullanan, "MUHGONDERMEBELGESITENKISKOSULSUZ", yil.ToString() + "-3---" + muhasebe + "-" + birim + "--" + durum + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb, false, "");
                //OrtakFonksiyonlar.HataDosyaYaz("OdenekSorgu.txt", sql, false);
                double gonderilen = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKISKOSULSUZ", yil.ToString() + "-3---" + muhasebe + "-" + birim + "--" + durum + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb, false, "");
                eob.ODENEK = gonderilen + tenkis;
            }
            eob.HARCAMA = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHODEMEEMRIMIF", yil.ToString() + "-" + muhasebe + "-" + birim + "-830-" + kur + "-" + fon + "-" + fin + "-" + eko + "-1/8-" + peb + "-", false, "");
            eob.AVANS = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHODEMEEMRIMIF", yil.ToString() + "-" + muhasebe + "-" + birim + "-160,161,164,165,166,167-" + kur + "-" + fon + "-" + fin + "-" + eko + "-1/8-" + peb + "-", false, "");
            eob.BLOKE = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHBLOKEODENEK", yil.ToString() + "-" + muhasebe + "-" + birim + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb, false, "");
            double surecBlokeEdilen = 0;
            if (!string.IsNullOrEmpty(surecNo))
            {
                surecBlokeEdilen = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHBLOKEODENEK", yil + "-" + muhasebe + "-" + birim + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb + "-" + surecYil.ToString() + "-" + surecNo, false, "");
                if (odemeEmrimi)
                    surecNo = "";
            }
            eob.BLOKE -= surecBlokeEdilen;
            string sYil;
            string sNo;
            string belgeNo;
            if (!odemeEmrimi)
            {
                sYil = surecYil.ToString();
                sNo = surecNo;
                belgeNo = "";
            }
            else
            {
                sYil = "";
                sNo = "";
                belgeNo = oeNo;
            }
            if (!string.IsNullOrWhiteSpace(sNo) || !string.IsNullOrWhiteSpace(belgeNo))
            {
                string uzayKosul = yil.ToString() + "-" + muhasebe + "-" + birim + "-830-" + kur + "-" + fon + "-" + fin + "-" + eko + "-1/8-" + peb + "--" + sYil + "-" + sNo + "-" + belgeNo;
                double belgeHarcama = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHODEMEEMRIMIF", uzayKosul, false, "");
                uzayKosul = yil.ToString() + "-" + muhasebe + "-" + birim + "-160,161,164,165,166,167-" + kur + "-" + fon + "-" + fin + "-" + eko + "-1/8-" + peb + "--" + sYil + "-" + sNo + "-" + belgeNo;
                double belgeAvans = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHODEMEEMRIMIF", uzayKosul, false, "");
                //string sql = MUHServis.servisUZY.UzayKosulStr(kullanan, "MUHODEMEEMRIMIF", uzayKosul, false, "");
                eob.HARCAMA -= belgeHarcama;
                eob.AVANS -= belgeAvans;
            }
            DebugYaz(yil, muhasebe, birim, ouKontrol, oeNo, eob, kullanan);
            //Eğer verilen tertipte ödenek 0 veya (-) ise bu tertip ile işlem yapılamasın. (Tertip listesinde listeleniyor). Melih 24.04.2015
            //Kontrol edilecek Tutar borc veya alacak 0 dan büyük ise işlem yapılsın Melih 24.04.2015
            if (borc > 0)
            {
                double kalanOdenek = Math.Round(eob.ODENEK - (eob.HARCAMA + eob.AVANS + eob.BLOKE), 2);
                //if (kalanOdenek < 0)
                //    kalanOdenek = 0;
                double kalanBorc = Math.Round(borc, 2);//EGO 30.01.2020 yılsonu kapatma işleminde ödeneğin tamamı harcandığında sorun oldu. Ne yapılabilir?
                if (kalanBorc > kalanOdenek) eob.IHTIYAC = kalanBorc - kalanOdenek;
                //else if (kalanOdenek <= 0) eob.IHTIYAC = Math.Abs(kalanBorc);////Melih 09.01.2015 TKDK da yıl sonu kapatması sırasında ödenek 150 harcama 150 olduğu halde kapatma (bütçe gider 9 nolu kapatma işi) uyarı verdi
                else eob.IHTIYAC = 0;
            }
            DebugYaz(yil, muhasebe, birim, ouKontrol, oeNo, eob, kullanan);
            return eob;
        }
        private static void DebugYaz(int yil, string muhasebe, string birim, ENUMOdenekUstuKontrol ouKontrol, string oeNo, EksikOdenekBilgi eob, Kullanici kullanan)
        {
            if (kullanan.kullaniciKodu == "stratek")
            {
                string yazi = $" yil:{yil},  muhasebe:{muhasebe},  birim:{birim},  ouKontrol:{(int)ouKontrol}, Tertip: {eob.TertipStr()} ==> ODENEK:{eob.ODENEK}, HARCAMA:{eob.HARCAMA}, BLOKE:{eob.BLOKE}, AVANS:{eob.AVANS}, IHTIYAC:{eob.IHTIYAC} ";
                OrtakFonksiyonlar.HataDosyaYaz("OdenekTalep.txt", yazi, false);
            }
        }
        /// <summary>
        /// Odeneks the talep yapilacak mi.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="tertipler">The tertipler.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="surecYil">The surec yil.</param>
        /// <param name="surecNo">The surec no.</param>
        /// <param name="odemeEmriBelgeNo">The odeme emri belge no.</param>
        /// <returns></returns>
        public EksikOdenekler OdenekTalepYapilacakMi(Kullanici kullanan, int muhasebe, string birim, TertipOdenekBilgi[] tertipler, int yil, int surecYil, string surecNo, string odemeEmriBelgeNo, bool odemeEmriMi, bool dogrudanTeminMi)
        {
            ENUMOdenekUstuKontrol ouKontrol = Arac.OdenekUstuKontrolBul();
            ENUMOdenekKontrolTur ogKontrolTur = Arac.OdenekKontrolTurBul(yil);
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);
            int eko3Uz = tUzBul.KodUzunlukBul(3, 3, true);
            string ortak000 = TNS.MUH.Arac.DegiskenDegerBul(yil, "/Ortak000");
            if (ortak000 == "1")
                birim = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            EksikOdenekler eo = new EksikOdenekler();
            eo.sonuc = new Sonuc();
            System.Collections.Generic.List<OdemeEmriMIFDetay> kontListe = new List<OdemeEmriMIFDetay>();
            if (tertipler != null && tertipler.Length > 0)
            {
                string tertip = string.Empty;
                for (int i = 0; i < tertipler.Length; i++)
                {
                    OdemeEmriMIFDetay item = new OdemeEmriMIFDetay(yil);
                    tertip = tertipler[i].tertip;
                    if (tertipler[i].odenekler != null && tertipler[i].odenekler.Length > 0)
                        item.borc = tertipler[i].odenekler[0];
                    string[] tertipParca = tertip.Replace(".", "").Split('-');
                    item.kur = Arac.DiziElemanAl(tertipParca, 0, "");
                    item.fon = Arac.DiziElemanAl(tertipParca, 1, "");
                    item.fin = Arac.DiziElemanAl(tertipParca, 2, "");
                    item.eko = Arac.DiziElemanAl(tertipParca, 3, "");
                    item.peb = Arac.DiziElemanAl(tertipParca, 4, "");
                    string uygunlukHata = MUHBagliEkoPebAna.UygunMu(ve, kullanan, item);
                    if (!string.IsNullOrEmpty(uygunlukHata))
                    {
                        ve.VeriErisimKapat();
                        eo.sonuc.DegerAta(uygunlukHata);
                        return eo;
                    }
                    OdemeEmriMIFForm.KontrolListesineEkle(yil, item, kontListe, ogKontrolTur, true, eko2Uz, eko3Uz);
                }
            }
            OdemeEmriMIFForm form = new OdemeEmriMIFForm();
            form.yil = yil;
            form.surecYil = surecYil;
            form.surecNo = surecNo;
            form.muhasebe = muhasebe;
            form.birim = birim;
            form.belgeNo = odemeEmriBelgeNo;
            OdenekKontrol(ve, kullanan, form, kontListe, ouKontrol, eo, odemeEmriMi, false, dogrudanTeminMi);
            ve.VeriErisimKapat();
            EksikOdenekSonucIsle(eo);
            return eo;
        }
        private string FaturaKayitlimi(VeriErisim ve, int fYil, string fNo, TNSDateTime fTarih, int muhasebe, string fBelgeNo, int fBelgeTur, string fkimlikNo)
        {
            string bulundu = "";
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader r = to.FaturaKayitlimi(fYil, fNo, fTarih, muhasebe, fBelgeNo, fBelgeTur, fkimlikNo);
            if (r != null)
            {
                if (r.Read())
                    bulundu = VeriErisim.DegerAl(r, 0, "");//BelgeNo
                r.Close();
            }
            return bulundu;
        }
        private void Satir90x905Olustur(OdemeEmriMIFForm form, string islemHesap)
        {
            Tertip tUzBul = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);
            if (form.fisTur[0] == ENUMFISTUR.AMORTISMANKAPANISI)
            {
                return;
            }
            string[,] a = new string[100, 5];
            int ekle90x = 0;
            foreach (OdemeEmriMIFDetay dd in form.detay.objeler)
            {
                //Önceden kayıt edilmiş olan 90x ve 905ler temizlensin
                if (dd.hesapKod.StartsWith(islemHesap) || dd.hesapKod.StartsWith("905"))
                    dd.hesapKod = "";
                //90x ve 905 satırları için bilgi topla
                if (dd.hesapKod.StartsWith("830"))
                {
                    string hesap2li = dd.hesapKod.Replace(".", "").Substring(3, 4);
                    if (dd.kur == "" || dd.eko.Replace(".", "").Length < 3)
                        continue;
                    string eko = dd.eko.Replace(".", "").Substring(0, eko2Uz);
                    string tertip = TNS.MUH.Arac.TertipStrOl(dd.kur, dd.fon, dd.fin, eko, dd.peb).Replace(".", "");
                    string detayBirim = dd.detayBirim;
                    for (int i = 0; i < a.GetUpperBound(0); i++)
                    {
                        ekle90x = i;
                        if (a[i, 0] == hesap2li && a[i, 1] == tertip)
                            break;
                        else if (a[i, 0] == null || a[i, 0] == "")
                            break;
                    }
                    a[ekle90x, 0] = hesap2li;
                    a[ekle90x, 1] = tertip;
                    a[ekle90x, 2] = (OrtakFonksiyonlar.ConvertToDbl(a[ekle90x, 2]) + dd.borc).ToString();
                    a[ekle90x, 3] = (OrtakFonksiyonlar.ConvertToDbl(a[ekle90x, 3]) + dd.alacak).ToString();
                    a[ekle90x, 4] = detayBirim;
                }
            }
            //90x ve 905 satırlarını ekle
            for (int i = 0; i < a.GetUpperBound(0); i++)
            {
                if (a[i, 0] != null && a[i, 0] != "")
                {
                    OdemeEmriMIFDetay d90x = new OdemeEmriMIFDetay(form.yil);
                    d90x.hesapKod = "905" + a[i, 0];
                    TNS.MUH.Arac.TertipAta(ref d90x, a[i, 1]);
                    d90x.detayBirim = a[i, 4];
                    d90x.borc = OrtakFonksiyonlar.ConvertToDbl(a[i, 2]);
                    d90x.alacak = OrtakFonksiyonlar.ConvertToDbl(a[i, 3]);
                    d90x.eko = d90x.eko.Replace(".", "");
                    if (d90x.eko.Length > eko2Uz)
                        d90x.eko = d90x.eko.Substring(0, eko2Uz);
                    d90x.BorcAlacakAyarla();
                    //OrtakFonksiyonlar.HataDosyaYaz("Kontrol.txt", "90xekle", false);
                    form.detay.objeler.Add(d90x);
                    //DetayDebugYaz(form);
                    d90x = new OdemeEmriMIFDetay(form.yil);
                    d90x.hesapKod = islemHesap + a[i, 0];
                    TNS.MUH.Arac.TertipAta(ref d90x, a[i, 1]);
                    d90x.detayBirim = a[i, 4];
                    d90x.borc = OrtakFonksiyonlar.ConvertToDbl(a[i, 3]);
                    d90x.alacak = OrtakFonksiyonlar.ConvertToDbl(a[i, 2]);
                    d90x.eko = d90x.eko.Replace(".", "");
                    if (d90x.eko.Length > eko2Uz)
                        d90x.eko = d90x.eko.Substring(0, eko2Uz);
                    d90x.BorcAlacakAyarla();
                    //OrtakFonksiyonlar.HataDosyaYaz("Kontrol.txt", "90x2ekle", false);
                    form.detay.objeler.Add(d90x);
                    //DetayDebugYaz(form);
                }
            }
            //Eski 90x ve 905 satırlarını sil
            for (int i = 0; i < form.detay.objeler.Count; i++)
            {
                if (((OdemeEmriMIFDetay)form.detay[i]).hesapKod.Trim() == "")
                {
                    form.detay.objeler.RemoveAt(i);
                    i = 0;
                }
            }
        }
        /// <summary>
        /// Bir Ödeme emri / MIF listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="tekBelge">Tek belge listelensin</param>
        /// <param name="detay">Eğer <c>true</c> ise detay bilgilerini de listele</param>
        /// <param name="mahsup">Eğer <c>true</c> ise mahsup bilgilerini de listele</param>
        /// <returns></returns>
        /// ***
        public ObjectArray Listele(Kullanici kullanan, OdemeEmriMIFForm form, bool tekBelge, bool detay, bool mahsup)
        {
            Tertip tUz = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH047, "");
            ObjectArray fV = new ObjectArray();
            string kurum = "";
            if (form.kurum == "")
                kurum = TNS.MUH.Yetki.VarsayilanKurumBul();
            else
                kurum = form.kurum;
            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, kurum))
            {
                fV.sonuc.hataStr = ButceMuhasebeResI.MUH048;
                fV.sonuc.islemSonuc = false;
                return fV;
            }
            if (tekBelge)
            {
                OdemeEmriMIFForm f = TekBelgeGetir(null, kullanan, form, true);
                if (f != null)
                {
                    fV.objeler.Add(f);
                    fV.sonuc.islemSonuc = true;
                }
                else
                {
                    fV.sonuc.islemSonuc = false;
                    fV.sonuc.hataStr = string.Format(ButceMuhasebeResI.MUH172, form.belgeNo);
                }
            }
            else
            {
                fV = CokBelgeGetir(kullanan, form, detay, mahsup);
                if (fV.objeler.Count == 0)
                {
                    fV.sonuc.islemSonuc = false;
                    fV.sonuc.hataStr = ButceMuhasebeResI.MUH051;
                }
                else
                    fV.sonuc.islemSonuc = true;
            }
            return fV;
        }
        /// <summary>
        /// Bir Ödeme emri / MIF listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="tekBelge">Tek belge listelensin</param>
        /// <param name="detay">Eğer <c>true</c> ise detay bilgilerini de listele</param>
        /// <param name="mahsup">Eğer <c>true</c> ise mahsup bilgilerini de listele</param>
        /// <returns></returns>
        /// ***
        public ObjectArray ListeleTumDetay(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            Tertip tUz = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH047, "");
            ObjectArray oslar = new ObjectArray();
            string kurum = "";
            if (form.kurum == "")
                kurum = OrtakFonksiyonlar.KurumsaldanKuruma(form.belgeNo.Substring(0, 4), tUz.Kirilimlar(0));
            else
                kurum = form.kurum;
            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, kurum))
            {
                oslar.sonuc.hataStr = ButceMuhasebeResI.MUH048;
                oslar.sonuc.islemSonuc = false;
                return oslar;
            }
            Tertip th = new Tertip(form.yil, ENUMTabloKod.TABOdemeEmri);
            Tertip tt = new Tertip(form.yil, ENUMTabloKod.TABOdenek);
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            //OrtakFonksiyonlar.HataStrYaz("SorguY Listele Başla" + DateTime.Now.ToString());
            IDataReader r = to.FormListe(form, false, false, false);
            if (to.Hata() == "")
            {
                OdemeEmriMIFForm os = null;
                while (r.Read())
                {
                    os = new OdemeEmriMIFForm();
                    DRDenFormOl(os, r);
                    os.kurum = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(0), os.kurum);
                    if (os.YEVCekNo != "")
                        os.YEVCekTarih = os.IOETarih;
                    os.kurumAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KURAD, "");
                    os.muhasebeAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.MUHASEBEAD, "");
                    oslar.objeler.Add(os);
                }
                r.Close();
            }
            foreach (OdemeEmriMIFForm item in oslar.objeler)
            {
                r = to.TekBelgeOkuDetay(item.yil, item.belgeNo, "", 0);
                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        OdemeEmriMIFDetay sd = DataReaderdanDetayOl(r, item.yil, th, tt);
                        item.detay.objeler.Add(sd);
                    }
                    r.Close();
                }
            }
            ve.VeriErisimKapat();
            return oslar;
        }
        private void DRDenFormOl(OdemeEmriMIFForm os, IDataReader r)
        {
            os.yil = VeriErisim.DegerAl(r, 0, 0);
            os.belgeTur = (ENUMBelgeTuru)VeriErisim.DegerAl(r, 1, 0);
            os.belgeNo = VeriErisim.DegerAl(r, 2, "");
            os.kurum = VeriErisim.DegerAl(r, 3, "");
            os.muhasebe = VeriErisim.DegerAl(r, 4, 0);
            os.birim = VeriErisim.DegerAl(r, 5, "");
            os.ilgiliAd = VeriErisim.DegerAl(r, 6, "");
            os.ilgiliNo = VeriErisim.DegerAl(r, 7, "");
            os.ilgiliBankaAd = VeriErisim.DegerAl(r, 8, "");
            os.ilgiliBankaNo = VeriErisim.DegerAl(r, 9, "");
            os.ilgiliVD = VeriErisim.DegerAl(r, 10, "");
            os.aciklama = VeriErisim.DegerAl(r, 11, "");
            os.islemTarih = VeriErisim.DegerAl(r, 12);
            os.formDurumTarih = VeriErisim.DegerAl(r, 13);
            os.formDurum = VeriErisim.DegerAl(r, 14, 0);
            os.iptalNeden = VeriErisim.DegerAl(r, 15, "");
            os.YEVYevTarih = VeriErisim.DegerAl(r, 16);
            os.YEVYevNo = VeriErisim.DegerAl(r, 17, 0);
            os.IOETarih = VeriErisim.DegerAl(r, 18);


            os.OEBTur = VeriErisim.DegerAl(r, 24, "");
            os.OEBTarih = VeriErisim.DegerAl(r, 25);
            os.OEBNo = VeriErisim.DegerAl(r, 26, "");

            os.YEVCekNo = VeriErisim.DegerAl(r, 29, "");
            os.islemYapan = VeriErisim.DegerAl(r, 30, "");
            os.projeNo = VeriErisim.DegerAl(r, 31, "");
            os.ihale = VeriErisim.DegerAl(r, 32, "");
            os.fisTurYap((ENUMFISTUR)VeriErisim.DegerAl(r, 33, 0));
            os.ilgiliBelgeNo = VeriErisim.DegerAl(r, 34, "");
            os.surecYil = VeriErisim.DegerAl(r, 35, 0);
            os.surecNo = VeriErisim.DegerAl(r, 36, "");
            os.kurKod = VeriErisim.DegerAl(r, 37, "");
            os.organizasyonKodu = VeriErisim.DegerAl(r, 38, 0);
            os.yevmiyeAciklama = VeriErisim.DegerAl(r, 39, "");
            os.duzeltilenOdemeEmriNo = VeriErisim.DegerAl(r, 40, "");
            os.istKod1.degerKod = VeriErisim.DegerAl(r, 41, 0);
            os.istKod2.degerKod = VeriErisim.DegerAl(r, 42, 0);
        }
        OdemeEmriMIFForm TekBelgeGetir(VeriErisim veGelen, Kullanici kullanan, OdemeEmriMIFForm form, bool detayVer)
        {
            bool bulundu = false;
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH047, "");
            if (form.belgeNo.Trim() == "" && form.YEVYevNo == 0)
                return new OdemeEmriMIFForm();
            VeriErisim ve = null;
            if (veGelen == null)
                ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            else
                ve = veGelen;
            VeriErisim veSor = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            OdemeEmriMIFForm os = new OdemeEmriMIFForm();
            OdemeEmriMIFForm sorguForm = new OdemeEmriMIFForm();
            sorguForm.yil = form.yil;
            sorguForm.belgeNo = form.belgeNo;
            if (string.IsNullOrEmpty(sorguForm.belgeNo))
                sorguForm.YEVYevNo = form.YEVYevNo;
            sorguForm.fisTur = null;
            IDataReader r = to.FormListe(sorguForm, false, false, false);
            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    bulundu = true;
                    Tertip t = new Tertip(form.yil, ENUMTabloKod.TABOdenek);
                    os.yil = VeriErisim.DegerAl(r, 0, 0);
                    os.belgeTur = (ENUMBelgeTuru)VeriErisim.DegerAl(r, 1, 0);
                    os.belgeNo = VeriErisim.DegerAl(r, 2, "");
                    os.kurum = VeriErisim.DegerAl(r, 3, "");
                    os.muhasebe = VeriErisim.DegerAl(r, 4, 0);
                    os.birim = VeriErisim.DegerAl(r, 5, "");
                    os.ilgiliAd = VeriErisim.DegerAl(r, 6, "");
                    os.ilgiliNo = VeriErisim.DegerAl(r, 7, "");
                    os.ilgiliBankaAd = VeriErisim.DegerAl(r, 8, "");
                    os.ilgiliBankaNo = VeriErisim.DegerAl(r, 9, "");
                    os.ilgiliVD = VeriErisim.DegerAl(r, 10, "");
                    os.aciklama = VeriErisim.DegerAl(r, 11, "");
                    os.islemTarih = VeriErisim.DegerAl(r, 12);
                    os.formDurumTarih = VeriErisim.DegerAl(r, 13);
                    os.formDurum = VeriErisim.DegerAl(r, 14, 0);
                    os.iptalNeden = VeriErisim.DegerAl(r, 15, "");
                    os.YEVYevTarih = VeriErisim.DegerAl(r, 16);
                    os.YEVYevNo = VeriErisim.DegerAl(r, 17, 0);
                    os.IOETarih = VeriErisim.DegerAl(r, 18);
                    os.IOEYevNo = VeriErisim.DegerAl(r, 19, "");
                    os.IOEKim = VeriErisim.DegerAl(r, 20, "");
                    os.AVKTarih = VeriErisim.DegerAl(r, 21);
                    os.AVKYevNo = VeriErisim.DegerAl(r, 22, "");
                    os.AVKTutar = VeriErisim.DegerAl(r, 23, 0.0);
                    os.OEBTur = VeriErisim.DegerAl(r, 24, "");
                    os.OEBTarih = VeriErisim.DegerAl(r, 25);
                    os.OEBNo = VeriErisim.DegerAl(r, 26, "");
                    os.OEBTutar = VeriErisim.DegerAl(r, 27, 0.0);
                    os.YEVVerNo = VeriErisim.DegerAl(r, 28, "");
                    os.YEVCekNo = VeriErisim.DegerAl(r, 29, "");
                    if (os.YEVCekNo != "")
                        os.YEVCekTarih = os.IOETarih;
                    os.islemYapan = VeriErisim.DegerAl(r, 30, "");
                    os.projeNo = VeriErisim.DegerAl(r, 31, "");
                    os.ihale = VeriErisim.DegerAl(r, 32, "");
                    os.fisTurYap((ENUMFISTUR)VeriErisim.DegerAl(r, 33, 0));
                    os.ilgiliBelgeNo = VeriErisim.DegerAl(r, 34, "");
                    os.surecYil = VeriErisim.DegerAl(r, 35, 0);
                    os.surecNo = VeriErisim.DegerAl(r, 36, "");
                    os.kurKod = VeriErisim.DegerAl(r, 37, "");
                    os.organizasyonKodu = VeriErisim.DegerAl(r, 38, 0);
                    os.yevmiyeAciklama = VeriErisim.DegerAl(r, 39, "");
                    os.duzeltilenOdemeEmriNo = VeriErisim.DegerAl(r, 40, "");
                    os.istKod1.degerKod = VeriErisim.DegerAl(r, 41, 0);
                    os.istKod2.degerKod = VeriErisim.DegerAl(r, 42, 0);
                    os.kurumAd = TNS.MUH.Yetki.VarsayilanKurumBulAd();
                    os.muhasebeAd = MUHServis.UzayYaziDegerGetir(veSor, kullanan, "MUHMUHASEBE", os.muhasebe.ToString(), true, "MUHASEBEAD");
                    os.birimAd = MUHServis.UzayYaziDegerGetir(veSor, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString() + "-" + os.kurKod, true, "BIRIMAD");
                    os.kurum = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), os.kurum);
                }
                r.Close();
            }
            else
            {
                if (veGelen == null)
                    ve.VeriErisimKapat();
                veSor.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            if (bulundu)
            {
                if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, os.muhasebe.ToString(), os.birim))
                {
                    return new OdemeEmriMIFForm();
                }
            }
            if (bulundu && detayVer && !form.sadeceUstVeri)
            {
                string detayHesapKod = "";
                if (form.detay.objeler.Count > 0)
                {
                    OdemeEmriMIFDetay d = new OdemeEmriMIFDetay(form.yil);
                    d = (OdemeEmriMIFDetay)form.detay[0];
                    detayHesapKod = d.hesapKod;
                }
                r = to.TekBelgeOkuDetay(os.yil, os.belgeNo, detayHesapKod, 0);
                if (to.Hata() == "")
                {
                    Tertip th = new Tertip(form.yil, ENUMTabloKod.TABOdemeEmri);
                    Tertip tt = new Tertip(form.yil, ENUMTabloKod.TABOdenek);
                    while (r.Read())
                    {
                        OdemeEmriMIFDetay sd = DataReaderdanDetayOl(r, form.yil, th, tt);
                        os.detay.objeler.Add(sd);
                    }
                    r.Close();
                }
                else
                {
                    if (veGelen == null)
                        ve.VeriErisimKapat();
                    veSor.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }
            }
            if (veGelen == null)
                ve.VeriErisimKapat();
            veSor.VeriErisimKapat();
            return os;
        }
        private OdemeEmriMIFDetay DataReaderdanDetayOl(IDataReader r, int yil, Tertip th, Tertip tt)
        {
            OdemeEmriMIFDetay sd = new OdemeEmriMIFDetay(yil);
            Tertip sdTertip = null;
            sd.satirNo = VeriErisim.DegerAl(r, 3, 0);
            sd.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), VeriErisim.DegerAl(r, 4, ""));
            string kur = VeriErisim.DegerAl(r, 5, "");
            sdTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            sdTertip.kodlar[0] = VeriErisim.DegerAl(r, 5, "");
            sdTertip.kodlar[1] = VeriErisim.DegerAl(r, 6, "");
            sdTertip.kodlar[2] = VeriErisim.DegerAl(r, 7, "");
            sdTertip.kodlar[3] = VeriErisim.DegerAl(r, 8, "");
            sdTertip.kodlar[4] = VeriErisim.DegerAl(r, 12, "");
            sdTertip.Noktala();
            sd.kur = sdTertip.kodlar[0];
            sd.fon = sdTertip.kodlar[1];
            sd.fin = sdTertip.kodlar[2];
            sd.eko = sdTertip.kodlar[3];
            sd.peb = sdTertip.kodlar[4];
            sd.prgKod = VeriErisim.DegerAl(r, 27, 0);
            sd.aPrgKod = VeriErisim.DegerAl(r, 28, 0);
            sd.faalKod = VeriErisim.DegerAl(r, 29, 0);
            sd.aFaalKod = VeriErisim.DegerAl(r, 30, 0);
            sd.borc = VeriErisim.DegerAl(r, 9, 0.0);
            sd.alacak = VeriErisim.DegerAl(r, 10, 0.0);
            sd.kesinti = VeriErisim.DegerAl(r, 11, 0);
            sd.altHesapKod = VeriErisim.DegerAl(r, 13, "");
            sd.doviz = VeriErisim.DegerAl(r, 14, "");
            sd.dovizKurTur = VeriErisim.DegerAl(r, 15, "");
            sd.dovizBorc = VeriErisim.DegerAl(r, 16, 0.0);
            sd.dovizAlacak = VeriErisim.DegerAl(r, 17, 0.0);
            sd.dovizKur = VeriErisim.DegerAl(r, 18, (decimal)0.0);
            sd.satirAciklama = VeriErisim.DegerAl(r, 19, "");
            sd.detayBirim = VeriErisim.DegerAl(r, 20, "");
            sd.isKod = VeriErisim.DegerAl(r, 21, 0);
            sd.isAdi = VeriErisim.DegerAl(r, 22, "");
            if (!string.IsNullOrWhiteSpace(sd.detayBirim))
                sd.detayBirim = sd.detayBirim + "." + sd.kur;
            if (string.IsNullOrWhiteSpace(sd.fon)) sd.kur = "";
            sd.aciklama = VeriErisim.DegerAl(r, 23, "");
            sd.mahsupBelgeNo = VeriErisim.DegerAl(r, 24, "");
            sd.altHesapAd = VeriErisim.DegerAl(r, 25, "");
            if (string.IsNullOrEmpty(sd.altHesapAd))
                sd.altHesapAd = VeriErisim.DegerAl(r, 26, "");
            return sd;
        }
        /// <summary>
        /// Verilen kritere göre ödeme emri belgelerinin listeleme işlemini yapan metod
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgileri taşıyan nesne</param>
        /// <param name="detay">Eğer <c>true</c> ise detay bilgilerini de listele</param>
        /// <param name="mahsup">Eğer <c>true</c> ise mahsup bilgilerini de listele</param>
        /// <returns></returns>
        public ObjectArray CokBelgeGetir(Kullanici kullanan, OdemeEmriMIFForm form, bool detay, bool mahsup)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim veSorgu = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray oslar = new ObjectArray();
            Hashtable birimAd = new Hashtable();
            //Listeleme sırasında kurum kullanıcıları kendilerine ait evrakları görsün
            MUHMuhasebeBirimServis.KullanicininYetkiliOlduguBirimler(kullanan, form);
            if (form.aciklama == "YetkisiYok")
                return oslar;
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            //OrtakFonksiyonlar.HataStrYaz("SorguY Listele Başla" + DateTime.Now.ToString());
            IDataReader r = to.FormListe(form, detay, mahsup, false);
            //OrtakFonksiyonlar.HataStrYaz("SorguY Listele Bitti" + DateTime.Now.ToString());
            if (to.Hata() == "")
            {
                string belgeNo = "";
                string eskiBelgeNo = "";
                OdemeEmriMIFForm os = null;
                Tertip th = new Tertip(form.yil, ENUMTabloKod.TABOdemeEmri);
                Tertip t = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
                Hashtable htIst = new Hashtable();
                while (r.Read())
                {
                    belgeNo = VeriErisim.DegerAl(r, 2, "");
                    if (eskiBelgeNo != belgeNo)
                    {
                        if (os != null)
                        {
                            //Eğer belgede kurumsal kod yok ise birim adı hatalı doldurulabilir
                            if (string.IsNullOrEmpty(os.birimAd))
                            {
                                if (birimAd[os.muhasebe.ToString() + os.birim + os.kurKod] == null)
                                {
                                    os.birimAd = MUHServis.UzayYaziDegerGetir(veSorgu, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString() + "-" + os.kurKod, false, "BIRIMAD");
                                    if (os.birimAd == "")
                                        os.birimAd = MUHServis.UzayYaziDegerGetir(veSorgu, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString() + "-" + MUH.Yetki.VarsayilanKurumBul(), false, "BIRIMAD");
                                    birimAd[os.muhasebe.ToString() + os.birim + os.kurKod] = os.birimAd;
                                }
                                else
                                    os.birimAd = (string)birimAd[os.muhasebe.ToString() + os.birim + os.kurKod];
                            }
                            oslar.objeler.Add(os);
                        }
                        os = new OdemeEmriMIFForm();
                        os.yil = VeriErisim.DegerAl(r, 0, 0);
                        os.belgeTur = (ENUMBelgeTuru)VeriErisim.DegerAl(r, 1, 0);
                        os.belgeNo = VeriErisim.DegerAl(r, 2, "");
                        os.kurum = VeriErisim.DegerAl(r, 3, "");
                        os.muhasebe = VeriErisim.DegerAl(r, 4, 0);
                        os.birim = VeriErisim.DegerAl(r, 5, "");
                        os.ilgiliAd = VeriErisim.DegerAl(r, 6, "");
                        os.ilgiliNo = VeriErisim.DegerAl(r, 7, "");
                        os.ilgiliBankaAd = VeriErisim.DegerAl(r, 8, "");
                        os.ilgiliBankaNo = VeriErisim.DegerAl(r, 9, "");
                        os.ilgiliVD = VeriErisim.DegerAl(r, 10, "");
                        os.aciklama = VeriErisim.DegerAl(r, 11, "");
                        os.islemTarih = VeriErisim.DegerAl(r, 12);
                        os.formDurumTarih = VeriErisim.DegerAl(r, 13);
                        os.formDurum = VeriErisim.DegerAl(r, 14, 0);
                        os.iptalNeden = VeriErisim.DegerAl(r, 15, "");
                        os.YEVYevTarih = VeriErisim.DegerAl(r, 16);
                        os.YEVYevNo = VeriErisim.DegerAl(r, 17, 0);
                        os.IOETarih = VeriErisim.DegerAl(r, 18);
                        os.OEBTur = VeriErisim.DegerAl(r, 24, "");
                        os.OEBTarih = VeriErisim.DegerAl(r, 25);
                        os.OEBNo = VeriErisim.DegerAl(r, 26, "");
                        os.YEVCekNo = VeriErisim.DegerAl(r, 29, "");
                        if (os.YEVCekNo != "")
                            os.YEVCekTarih = os.IOETarih;
                        os.kurum = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), os.kurum);
                        os.islemYapan = VeriErisim.DegerAl(r, 30, "");
                        os.projeNo = VeriErisim.DegerAl(r, 31, "");
                        os.ihale = VeriErisim.DegerAl(r, 32, "");
                        os.fisTurYap((ENUMFISTUR)VeriErisim.DegerAl(r, 33, 0));
                        os.ilgiliBelgeNo = VeriErisim.DegerAl(r, 34, "");
                        os.surecYil = VeriErisim.DegerAl(r, 35, 0);
                        os.surecNo = VeriErisim.DegerAl(r, 36, "");
                        os.kurKod = VeriErisim.DegerAl(r, 37, "");
                        os.organizasyonKodu = VeriErisim.DegerAl(r, 38, 0);
                        os.yevmiyeAciklama = VeriErisim.DegerAl(r, 39, "");
                        os.duzeltilenOdemeEmriNo = VeriErisim.DegerAl(r, 40, "");
                        os.istKod1.degerKod = VeriErisim.DegerAl(r, 41, 0);
                        os.istKod2.degerKod = VeriErisim.DegerAl(r, 42, 0);
                        os.istKod1.aciklama = IstatistikKodDeger.AciklamaBul(MUHIstatistikKodGiris.DegerAdBul(veSorgu, kullanan, 1, os.istKod1.degerKod, htIst));
                        os.istKod2.aciklama = IstatistikKodDeger.AciklamaBul(MUHIstatistikKodGiris.DegerAdBul(veSorgu, kullanan, 2, os.istKod2.degerKod, htIst));
                        os.kurumAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler1.KURAD, "");
                        os.muhasebeAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler1.MUHASEBEAD, "");
                        if (!detay)
                        {
                            os.borcToplam = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler1.BORC, 0.0);
                            os.alacakToplam = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler1.ALACAK, 0.0);
                            //Bu satır 17.07.2018 tarihinde eklendi. Tuğçe hanım listeleme ekranında ödeme emrine eklenmiş dosya var mı görmek istiyor. Melih
                            os.ekliDosyaVarmi = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler1.ALACAK + 1, 0);
                        }
                    }
                    //detay kayıtlar işleniyor
                    if (detay)
                    {
                        OdemeEmriMIFDetay d = new OdemeEmriMIFDetay(form.yil);
                        if (mahsup)
                            d.kapatma = new ObjectArray();
                        int satirNo = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.SIRA, 0);
                        string hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.HESAPKOD, ""));
                        bool oncekiKayitDevam = false;
                        foreach (OdemeEmriMIFDetay onceki in os.detay.objeler)
                        {
                            if (onceki.satirNo == satirNo && onceki.hesapKod == hesapKod)
                            {
                                d = onceki;
                                oncekiKayitDevam = true;
                                break;
                            }
                        }
                        if (!oncekiKayitDevam)
                        {
                            d.satirNo = satirNo;
                            d.hesapKod = hesapKod;
                            string kur = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KURKOD, "");
                            string fon = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FONKOD, "");
                            string fin = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FINKOD, "");
                            string eko = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.EKOKOD, "");
                            string peb = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.PEBKOD, "");
                            if (kur != "")
                            {
                                d.kur = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), kur);
                                d.fon = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(1), fon);
                                d.fin = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(2), fin);
                                d.eko = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(3), eko);
                                d.peb = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(4), peb);
                                d.prgKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.PRGKOD, 0);
                                d.aPrgKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.APRGKOD, 0);
                                d.faalKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FAALKOD, 0);
                                d.aFaalKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.AFAALKOD, 0);
                            }
                            d.borc = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.BORC, 0.0);
                            d.alacak = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALACAK, 0.0);
                            d.kesinti = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KESINTI, 0);
                            d.altHesapKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALTHESAPKOD, "");
                            d.doviz = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZ, "");
                            d.dovizKurTur = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZKURTUR, "");
                            d.dovizBorc = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZBORC, 0.0);
                            d.dovizAlacak = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZALACAK, 0.0);
                            d.dovizKur = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZKUR, (decimal)0.0);
                            d.satirAciklama = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ACIKLAMA, "");
                            d.detayBirim = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DETAYBIRIM, "");
                            d.hesapAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.HESAPPLANI_ACIKLAMA, "");
                            d.altHesapAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALTHESAPAD1, "");
                            if (string.IsNullOrEmpty(d.altHesapAd))
                                d.altHesapAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALTHESAPAD2, "");
                            os.borcToplam += d.borc;
                            os.alacakToplam += d.alacak;
                        }
                        if (mahsup)
                        {
                            string kapatan = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPATAN, "");
                            string kapatilan = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPATILAN, "");
                            string[] satirlar = kapatilan.Split('|');
                            for (int i = 0; i < satirlar.Length; i++)
                            {
                                string[] kapatanB = kapatan.Split('^');
                                string[] kapatilanB = satirlar[i].Split('^');
                                if (kapatanB.Length > 2)
                                {
                                    d.mahsupBelgeNo = kapatanB[2];
                                    d.sorguKapatan = 1;
                                }
                                if (string.IsNullOrEmpty(satirlar[i])) continue;
                                if (kapatanB.Length < 6)
                                {
                                    kapatanB = new string[7];
                                    for (int j = 0; j < kapatanB.Length; j++)
                                    {
                                        kapatanB[j] = "";
                                    }
                                }
                                if (kapatilanB.Length < 6)
                                {
                                    kapatilanB = new string[7];
                                    for (int j = 0; j < kapatanB.Length; j++)
                                    {
                                        kapatilanB[j] = "";
                                    }
                                }

                                KapanisBilgileri kapatma = new KapanisBilgileri();
                                kapatma.yil = OrtakFonksiyonlar.ConvertToInt(kapatilanB[0], 0);// VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPANISYIL, 0);
                                kapatma.belgeNo = kapatilanB[1];// VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPANISBELGENO, "");
                                kapatma.kTutar = OrtakFonksiyonlar.ConvertToDbl(kapatilanB[3].Replace(".", ","));//VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPANISTUTAR, 0.0);
                                kapatma.borcAlacak = OrtakFonksiyonlar.ConvertToInt(kapatilanB[4], 0);//VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.BORCALACAK, 0);
                                kapatma.kIlgiliVno = kapatilanB[5];//VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPANISILGILI, "");
                                kapatma.kBirimKod = kapatilanB[6];//VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPANISILGILI, "");
                                if (string.IsNullOrEmpty(kapatma.belgeNo))
                                {
                                    kapatma.yil = OrtakFonksiyonlar.ConvertToInt(kapatanB[0], 0);//VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPANISYIL2, 0);
                                    kapatma.belgeNo = kapatanB[2];//VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPANISBELGENO2, "");
                                    kapatma.kTutar = OrtakFonksiyonlar.ConvertToDbl(kapatanB[3].Replace(".", ","));//VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPANISTUTAR2, 0.0);
                                }
                                d.mahsupBelgeNo = kapatma.belgeNo;
                                if (d.altHesapKod == "" || kapatma.kIlgiliVno == d.altHesapKod)
                                    d.kapatma.objeler.Add(kapatma);
                            }
                        }
                        if (!oncekiKayitDevam)
                            os.detay.objeler.Add(d);
                    }
                    eskiBelgeNo = belgeNo;
                }
                if (os != null)
                {
                    //Eğer belgede kurumsal kod yok ise birim adı doldurulamıyor
                    if (string.IsNullOrEmpty(os.birimAd))
                    {
                        if (birimAd[os.muhasebe.ToString() + os.birim + os.kurKod] == null)
                        {
                            os.birimAd = MUHServis.UzayYaziDegerGetir(veSorgu, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString() + "-" + os.kurKod, false, "BIRIMAD");
                            if (os.birimAd == "")
                                os.birimAd = MUHServis.UzayYaziDegerGetir(veSorgu, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString() + "-" + MUH.Yetki.VarsayilanKurumBul(), false, "BIRIMAD");
                        }
                        else
                            os.birimAd = (string)birimAd[os.muhasebe.ToString() + os.birim + os.kurKod];
                    }
                    oslar.objeler.Add(os);
                }
                try
                {
                    r.Close();
                }
                catch { }
            }
            else
            {
                //OdemeEmriMIFForm os = new OdemeEmriMIFForm();
                //os.aciklama = "Listeleme sırasında bir hata oluştu. Lütfen kriter alanlarını kontrol ederek tekrar listeleme işlemi yapınız.<br>" + to.Hata();
                //oslar.formDizi.Add(os);
            }
            ve.VeriErisimKapat();
            veSorgu.VeriErisimKapat();
            return oslar;
        }
        /// <summary>
        /// Tarihes the oncesi toplam getir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public ObjectArray TarihOncesiToplamGetir(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            Tertip t = new Tertip(form.yil, ENUMTabloKod.TABOdenek);
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH047, "");
            ObjectArray fV = new ObjectArray();
            string kurum = "";
            if (form.kurum == "")
            {
                kurum = OrtakFonksiyonlar.KurumsaldanKuruma(form.belgeNo.Substring(0, 4), t.Kirilimlar(0));
            }
            else
                kurum = form.kurum;
            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, kurum))
            {
                fV.sonuc.hataStr = ButceMuhasebeResI.MUH048;
                fV.sonuc.islemSonuc = false;
                return fV;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray oslar = new ObjectArray();
            Hashtable birimAd = new Hashtable();
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader r = to.FormListe(form, true, false, false);
            if (to.Hata() == "")
            {
                Tertip th = new Tertip(form.yil, ENUMTabloKod.TABOdemeEmri);
                OdemeEmriMIFDetay d = new OdemeEmriMIFDetay(form.yil);
                while (r.Read())
                {
                    //detay kayıtlar işleniyor
                    string hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.HESAPKOD, ""));
                    d.hesapKod = hesapKod;
                    string kur = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KURKOD, "");
                    string fon = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FONKOD, "");
                    string fin = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FINKOD, "");
                    string eko = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.EKOKOD, "");
                    string peb = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.PEBKOD, "");
                    if (kur != "")
                    {
                        if (t.kodlar.Length > 0)
                            d.kur = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), kur);
                        if (t.kodlar.Length > 1)
                            d.fon = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(1), fon);
                        if (t.kodlar.Length > 2)
                            d.fin = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(2), fin);
                        if (t.kodlar.Length > 3)
                            d.eko = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(3), eko);
                        if (t.kodlar.Length > 4)
                            d.peb = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(4), peb);
                        d.prgKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.PRGKOD, 0);
                        d.aPrgKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.APRGKOD, 0);
                        d.faalKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FAALKOD, 0);
                        d.aFaalKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.AFAALKOD, 0);
                    }
                    d.borc += VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.BORC, 0.0);
                    d.alacak += VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALACAK, 0.0);
                    d.dovizBorc += VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZBORC, 0.0);
                    d.dovizAlacak += VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZALACAK, 0.0);
                    d.hesapAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.HESAPPLANI_ACIKLAMA, "");
                }
                try
                {
                    r.Close();
                }
                catch { }
                fV.objeler.Add(d);
            }
            else
            {
                fV.sonuc.hataStr = to.Hata();
                fV.sonuc.islemSonuc = false;
                return fV;
            }
            ve.VeriErisimKapat();
            return fV;
        }
        /// <summary>
        /// Belge durum bilgisi
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <returns></returns>
        public OdemeEmriMIFForm BelgeKimlikOku(VeriErisim veriErisim, int yil, string belgeNo)
        {
            OdemeEmriMIFForm form = new OdemeEmriMIFForm();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader dr = to.MIFKimlikOku(yil, belgeNo);
            if (dr != null && dr.Read())
            {
                DRDenFormOl(form, dr);
                dr.Close();
            }
            ve.VeriErisimKapat();
            return form;
        }
        /// <summary>
        /// Belge durum değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="formlar">İşlem yapılacak bilgileri içeren nesne</param>
        /// <param name="surecBlokeDuzenle">if set to <c>true</c> [surec bloke duzenle].</param>
        /// <returns></returns>
        public Sonuc BelgeDurumDegistir(Kullanici kullanan, TNSCollection formlar, bool surecBlokeDuzenle)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH052, "");
            string hata = "";
            int yevmiyeNo = 0;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            foreach (OdemeEmriMIFForm form in formlar)
            {
                bool yetki = false;
                if (form.formDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER ||
                    form.formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER ||
                    form.formDurum == (int)ENUMBelgeDurumu.ONAYLI ||
                    form.formDurum == (int)ENUMBelgeDurumu.ONAYKALDIR ||
                    form.formDurum == (int)ENUMBelgeDurumu.YEVMIYEKALDIR ||
                    form.formDurum == (int)ENUMBelgeDurumu.CEKNOVER)
                {
                    OdemeEmriMIFForm f = TekBelgeGetir(null, kullanan, form, false);
                    yetki = MUH.IsKurallari.MUHYetkiServis.OnaylamaYetkisiVarMiOdemeMIF(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, f.muhasebe.ToString(), f.birim, form.formDurum);
                }
                else
                    yetki = MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, form.kurum);
                if (!yetki)
                {
                    if (form.formDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER ||
                        form.formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER ||
                        form.formDurum == (int)ENUMBelgeDurumu.ONAYLI ||
                        form.formDurum == (int)ENUMBelgeDurumu.ONAYKALDIR ||
                        form.formDurum == (int)ENUMBelgeDurumu.YEVMIYEKALDIR ||
                        form.formDurum == (int)ENUMBelgeDurumu.CEKNOVER)
                        hata = "<br>" + string.Format(ButceMuhasebeResI.MUH054, form.belgeNo);//MUH173
                    else
                        hata = "<br>" + string.Format(ButceMuhasebeResI.MUH054, form.belgeNo);
                    break;
                }
                OdemeEmriMIFForm oS = TekBelgeGetir(ve, kullanan, form, false);
                //İptal edilmiş belgenin duyrumu ancak kayıt edilerek yeni konumuna getirilebilsin
                if (oS.formDurum == (int)ENUMBelgeDurumu.IPTAL)
                    hata += "<br>İptal edilmiş belge üzerinde işlem yapılamaz";
                //********************************************************************************
                form.surecNo = oS.surecNo;
                form.surecYil = oS.surecYil;
                form.muhasebe = oS.muhasebe;
                form.islemTarih = oS.islemTarih;
                if (oS.kurum != "")
                {
                    if (form.formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
                    {
                        if (form.YEVYevTarih.isNull)
                            form.YEVYevTarih = oS.islemTarih;
                        else if (form.YEVYevTarih.Yil != oS.islemTarih.Yil)
                        {
                            hata += "<br>" + "Yevmiye tarihi belgenin ait olduğu yıl içindeki bir tarih olmalıdır.";
                        }
                        if (form.YEVYevTarih.Oku() > DateTime.Now)
                        {
                            hata += "<br>" + "İçinde bulunduğumuz günden ileri tarihli yevmiye numarası verilemez!";
                        }
                    }
                    if (oS.kurum.Replace(".", "") != form.kurum.Replace(".", ""))
                        hata += "<br>" + ButceMuhasebeResI.MUH055;
                    if (oS.formDurum != (int)ENUMBelgeDurumu.YENI && oS.formDurum != (int)ENUMBelgeDurumu.DEGISTIRILDI)
                    {
                        if (oS.formDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                        {
                            if (form.formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER || form.formDurum != (int)ENUMBelgeDurumu.YEVMIYEKALDIR)
                                hata += "<br>" + string.Format(ButceMuhasebeResI.MUH174, form.belgeNo.ToString());
                        }
                        else if (oS.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                        {
                            if (form.formDurum != (int)ENUMBelgeDurumu.ONAYKALDIR && form.formDurum != (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER && form.formDurum != (int)ENUMBelgeDurumu.CEKNOVER)
                                hata = "<br>" + string.Format(ButceMuhasebeResI.MUH149, form.belgeNo.ToString());
                        }
                        else if (oS.formDurum != (int)ENUMBelgeDurumu.IPTAL)
                            hata += "<br>" + string.Format(ButceMuhasebeResI.MUH175, form.belgeNo.ToString());
                    }
                }
                else
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH063, form.belgeNo);
            }
            if (hata == "")
            {
                TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
                foreach (OdemeEmriMIFForm form in formlar)
                {
                    Sonuc s = null;
                    //İşlem yapılan tarih kapatılmış mı kontrolü
                    hata = TarihKapaliMi(kullanan, form.islemTarih);
                    if (hata != "")
                    {
                        hata = form.belgeNo + " nolu belge için işlem yapılamadı. Tarih kapatılmış.";
                        s = new Sonuc();
                        s.islemSonuc = false;
                        s.hataStr = hata;
                        break;
                    }
                    if (form.formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
                    {
                        lock (this)
                        {
                            if (yevmiyeNo == 0)
                            {
                                yevmiyeNo = to.EnBuyukYevmiyeNo(form.yil, form.muhasebe);
                                if (form.YEVYevNo > 0)
                                {
                                    yevmiyeNo = form.YEVYevNo;
                                    bool varmi = to.YevmiyeNoVarmi(form.yil, form.muhasebe, yevmiyeNo);
                                    if (varmi)
                                    {
                                        hata = form.YEVYevNo + " nolu yevmiye mükerrer olarak verilmek istenildiği için işlem yapılamadı.";
                                        s = new Sonuc();
                                        s.islemSonuc = false;
                                        s.hataStr = hata;
                                        break;
                                    }
                                }
                            }
                            else
                                yevmiyeNo++;
                            form.YEVYevNo = yevmiyeNo;
                            s = to.YevmiyeNoVer(form, true);
                        }
                    }
                    else if (form.formDurum == (int)ENUMBelgeDurumu.YEVMIYEKALDIR)
                    {
                        s = to.YevmiyeNoVer(form, false);
                    }
                    else if (form.formDurum == (int)ENUMBelgeDurumu.CEKNOVER)
                    {
                        s = to.CekNoVer(form, true);
                    }
                    else
                    {
                        bool onayKaldir = false;
                        if (form.formDurum == (int)ENUMBelgeDurumu.ONAYKALDIR)
                        {
                            form.formDurum = (int)ENUMBelgeDurumu.DEGISTIRILDI;
                            onayKaldir = true;
                        }
                        s = to.BelgeDurumDegistir(form, onayKaldir);
                        if (form.formDurum == (int)ENUMBelgeDurumu.IPTAL && s.islemSonuc)
                        {
                            ObjectArray liste = new ObjectArray();
                            KapanisBilgileri kapatma = new KapanisBilgileri();
                            kapatma.yil = form.yil;
                            kapatma.belgeNo = form.belgeNo;
                            liste.objeler.Add(kapatma);
                            Sonuc sKapat = new MUHOdemeEmriMIFServis().KapanisBilgisiIptalEt(kullanan, liste);
                        }
                    }
                    if (!s.islemSonuc)
                    {
                        hata = s.hataStr;
                        break;
                    }
                }
            }
            if (hata != "")
            {
                ve.Rollback();
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
            }
            else
            {
                ve.Commit();
                if (surecBlokeDuzenle)
                    FormlarinBlokleriniAyarla(kullanan, formlar);
                sonuc.bilgiStr = "<br>" + ButceMuhasebeResI.MUH065;
                sonuc.anahtar = yevmiyeNo.ToString();
                sonuc.islemSonuc = true;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
        private void FormlarinBlokleriniAyarla(Kullanici kullanan, TNSCollection formlar)
        {
            foreach (OdemeEmriMIFForm form in formlar)
            {
                if (string.IsNullOrEmpty(form.surecNo) || form.surecYil == 0) continue;
                servisHRC.BlokeTertipleriOtomatikGuncelle(kullanan, form.surecYil, form.surecNo, form.yil, ENUMKontrolDurum.KontrolEtme);
            }
        }
        /// <summary>
        /// Belge tür değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="yeniBelgeTur">Yeni belge türü</param>
        /// <returns></returns>
        public Sonuc BelgeTurDegistir(Kullanici kullanan, OdemeEmriMIFForm form, int yeniBelgeTur)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH176, "");
            string hata = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            bool yetki = false;
            yetki = MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMiKurum(kullanan, yeniBelgeTur, form.kurum);
            if (!yetki)
                hata = "<br>" + string.Format(ButceMuhasebeResI.MUH054, form.belgeNo);
            OdemeEmriMIFForm oS = TekBelgeGetir(ve, kullanan, form, false);
            if (oS.kurum == "")
                hata += "<br>" + string.Format(ButceMuhasebeResI.MUH063, form.belgeNo);
            if (hata == "")
            {
                TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
                sonuc = to.BelgeTurDegistir(form, yeniBelgeTur);
                if (!sonuc.islemSonuc)
                    hata = sonuc.hataStr;
            }
            if (hata != "")
            {
                ve.Rollback();
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
            }
            else
            {
                ve.Commit();
                sonuc.bilgiStr = "<br>" + ButceMuhasebeResI.MUH065;
                sonuc.islemSonuc = true;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
        /// <summary>
        /// Tertip listele.
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="eko">Ekonomik kod</param>
        /// <returns></returns>
        public ObjectArray TertipListele(int yil, string kurum, int muhasebe, string birim, string eko)
        {
            string kur = "";
            string fon = "";
            string fin = "";
            ObjectArray tertipVAN = new ObjectArray();
            if (muhasebe > 0 && birim != "" && TNS.BKD.Arac.KurumMu(kurum))
            {
                MUHMuhasebeBirimServis mb = new MUHMuhasebeBirimServis();
                ABSKodlarVAN kv = mb.HarcamaBirimleriKurumsalKod(null, yil, kurum, muhasebe, birim);
                if (kv.sonuc.islemSonuc)
                {
                    foreach (ABSKod k in kv.absKodlar)
                    {
                        if (kur != "") kur += ",";
                        kur += k.kod.Replace(".", "");
                    }
                }
            }
            else
                kur = kurum.Replace(".", "");
            eko = eko.Replace(".", "");
            if (!MUHGondermeTenkisServis.PEBTertipMi(yil))
            {
                Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                int eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
                TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
                if (eko.Length > eko2Uz)
                    eko = eko.Substring(0, eko2Uz);
                IDataReader r = to.TertipListele(yil, kur, "", "", eko);
                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        Tertip t = new Tertip(yil, ENUMTabloKod.TABOdenek);
                        kur = VeriErisim.DegerAl(r, 1, "");
                        fon = VeriErisim.DegerAl(r, 2, "");
                        fin = VeriErisim.DegerAl(r, 3, "");
                        eko = VeriErisim.DegerAl(r, 4, "");
                        t.TertipAta(kur, fon, fin, eko);
                        t.Noktala();
                        tertipVAN.Ekle(t);
                    }
                    r.Close();
                }
                ve.VeriErisimKapat();
            }
            else
            {
                IBHMv2Servis s = TNS.BHMv2.Arac.Tanimla();
                string[] kurlar = kur.Split(',');
                for (int i = 0; i < kurlar.Length; i++)
                {
                    if (kurlar[i].Trim() == "")
                        continue;
                    Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurlar[i], "", "", eko, "");
                    TertipOdenek[] tos = s.MPBirimTertipListele(kistas, muhasebe, birim);
                    foreach (TertipOdenek item in tos)
                    {
                        tertipVAN.Ekle(item.tertip);
                    }
                }
            }
            return tertipVAN;
        }
        /// <summary>
        /// 4 kırımlı olarak Tertip listele 
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="eko">Ekonomik Kod</param>
        /// <returns></returns>
        public ObjectArray TertipListeleEko4(int yil, string kurum, int muhasebe, string birim, string eko)
        {
            string kur = "";
            string fon = "";
            string fin = "";
            string ekoAd = "";
            ObjectArray tertipVAN = new ObjectArray();
            if (muhasebe > 0 && birim != "")
            {
                MUHMuhasebeBirimServis mb = new MUHMuhasebeBirimServis();
                string[] birimler = birim.Split(',');
                for (int i = 0; i < birimler.Length; i++)
                {
                    ABSKodlarVAN kv = mb.HarcamaBirimleriKurumsalKod(null, yil, kurum, muhasebe, birimler[i]);
                    if (kv.sonuc.islemSonuc)
                    {
                        foreach (ABSKod k in kv.absKodlar)
                        {
                            if (kur != "") kur += ",";
                            kur += k.kod.Replace(".", "");
                        }
                    }
                }
            }
            else
                kur = kurum.Replace(".", "");
            if (kur.Trim() == "") kur = kurum.Replace(".", "");
            eko = eko.Replace(".", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            kur = kur.Replace(";", ",");//Bazı yerlerde ayıraç olrak ; kullanuılmış fakat işlem , ile yapıldığı için
            IDataReader r = to.TertipListeleEko4(yil, kur, "", "", eko);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Tertip t = new Tertip(yil, ENUMTabloKod.TABOdenek);
                    kur = VeriErisim.DegerAl(r, 1, "");
                    fon = VeriErisim.DegerAl(r, 2, "");
                    fin = VeriErisim.DegerAl(r, 3, "");
                    eko = VeriErisim.DegerAl(r, 4, "");
                    ekoAd = VeriErisim.DegerAl(r, 5, "");
                    t.TertipAta(kur, fon, fin, eko);
                    t.adlar[3] = ekoAd;
                    t.Noktala();
                    tertipVAN.Ekle(t);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return tertipVAN;
        }
        /// <summary>
        /// Performans bütçe tertiplerini listele 
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="eko">Ekonomik Kod</param>
        /// <param name="peb">Peb kodu</param>
        /// <returns></returns>
        public ObjectArray TertipListeleEko4Peb(int yil, string kurum, int muhasebe, string birim, string eko, string peb)
        {
            string kur = "";
            string fon = "";
            string fin = "";
            string ekoAd = "";
            ObjectArray tertipVAN = new ObjectArray();
            if (muhasebe > 0 && birim != "")
            {
                MUHMuhasebeBirimServis mb = new MUHMuhasebeBirimServis();
                string[] birimler = birim.Split(',');
                for (int i = 0; i < birimler.Length; i++)
                {
                    ABSKodlarVAN kv = mb.HarcamaBirimleriKurumsalKod(null, yil, kurum, muhasebe, birimler[i]);
                    if (kv.sonuc.islemSonuc)
                    {
                        foreach (ABSKod k in kv.absKodlar)
                        {
                            if (kur != "") kur += ",";
                            kur += k.kod.Replace(".", "");
                        }
                    }
                }
            }
            else
                kur = kurum.Replace(".", "");
            eko = eko.Replace(".", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader r = to.TertipListeleEko4Peb(yil, kur, "", "", eko, "");
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                    kur = VeriErisim.DegerAl(r, 1, "");
                    fon = VeriErisim.DegerAl(r, 2, "");
                    fin = VeriErisim.DegerAl(r, 3, "");
                    eko = VeriErisim.DegerAl(r, 4, "");
                    ekoAd = VeriErisim.DegerAl(r, 5, "");
                    peb = VeriErisim.DegerAl(r, 6, "");

                    t.TertipAta(kur, fon, fin, eko, peb);
                    t.adlar[3] = ekoAd;
                    t.Noktala();
                    tertipVAN.Ekle(t);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return tertipVAN;
        }
        /// <summary>
        /// Avans listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="avans">avans</param>
        /// <returns></returns>
        public ObjectArray AvansListele(Kullanici kullanan, AvansBilgi avans)
        {
            ObjectArray avlar = new ObjectArray();
            if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, avans.muhasebe.ToString(), avans.birim))
            {
                avlar.sonuc.hataStr = ButceMuhasebeResI.MUH081;
                avlar.sonuc.islemSonuc = false;
                return avlar;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader r = to.AvansListele(avans.yil, avans.muhasebe, avans.birim, avans.kimlikNo, avans.adSoyad);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AvansBilgi av = new AvansBilgi();
                    av.yil = VeriErisim.DegerAl(r, 0, 0);
                    av.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    av.birim = VeriErisim.DegerAl(r, 2, "");
                    av.kimlikNo = VeriErisim.DegerAl(r, 3, "");
                    av.adSoyad = VeriErisim.DegerAl(r, 4, "");
                    av.borc = VeriErisim.DegerAl(r, 5, 0.0);
                    av.alacak = VeriErisim.DegerAl(r, 6, 0.0);
                    av.muhasebeAd = VeriErisim.DegerAl(r, 7, "");
                    //av.birimAd = VeriErisim.DegerAl(r, 8, "");
                    avlar.objeler.Add(av);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return avlar;
        }
        /// <summary>
        /// Maaş bilgileri listele.(EGO için geliştirildi)
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray MaasEntegrasyouListe(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriMAAS", TNS.KYM.Arac.KimlikBul(kullanan));//Maas entegrasyonu ego da(SQL Server) ve oracle da çalıştığı için özel bağlantı satırı gekeli oldu
            VeriErisim veBUM = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));//Maas entegrasyonu ego da(SQL Server) ve oracle da çalıştığı için özel bağlantı satırı gekeli oldu
            ObjectArray oslar = new ObjectArray();
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader r = to.MaasEntegrasyouListe(form);
            if (to.Hata() == "")
            {
                string belgeNo = "";
                string eskiBelgeNo = "";
                OdemeEmriMIFForm os = null;
                Tertip th = new Tertip(form.yil, ENUMTabloKod.TABOdemeEmri);
                Tertip t = new Tertip(form.yil, ENUMTabloKod.TABOdenek);
                while (r.Read())
                {
                    belgeNo = OrtakFonksiyonlar.ConvertToStr(VeriErisim.DegerAl(r, 0, 0));
                    if (eskiBelgeNo != belgeNo)
                    {
                        if (os != null)
                        {
                            //Eğer belgede kurumsal kod yok ise birim adı doldurulamıyor
                            if (string.IsNullOrEmpty(os.birimAd))
                                os.birimAd = MUHServis.UzayYaziDegerGetir(veBUM, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString(), false, "BIRIMAD");
                            oslar.objeler.Add(os);
                        }
                        os = new OdemeEmriMIFForm();
                        os.belgeNo = OrtakFonksiyonlar.ConvertToStr(VeriErisim.DegerAl(r, 0, 0));
                        os.yil = VeriErisim.DegerAl(r, 1, 0);
                        os.ay = VeriErisim.DegerAl(r, 2, 0);
                        os.muhasebe = VeriErisim.DegerAl(r, 3, 0);
                        os.birim = VeriErisim.DegerAl(r, 4, "");
                        os.bordroTur = VeriErisim.DegerAl(r, 5, "");
                        os.perTur = VeriErisim.DegerAl(r, 6, "");
                        os.islendi = VeriErisim.DegerAl(r, 7, "");
                    }
                    //detay kayıtlar işleniyor
                    string birimKurumsal = "";
                    OdemeEmriMIFDetay d = new OdemeEmriMIFDetay(form.yil);
                    d.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), VeriErisim.DegerAl(r, 10, ""));
                    string tertip = VeriErisim.DegerAl(r, 11, "");
                    string[] tt = tertip.Split('-');

                    string kur = "";
                    string fon = "";
                    string fin = "";
                    string eko = "";
                    if (tt.Length > 3)
                    {
                        kur = tt[0];
                        fon = tt[1];
                        fin = tt[2];
                        eko = tt[3];
                    }
                    if (kur != "")
                    {
                        d.kur = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), kur);
                        d.fon = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(1), fon);
                        d.fin = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(2), fin);
                        d.eko = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(3), eko);
                        birimKurumsal = kur;
                    }
                    d.borc = VeriErisim.DegerAl(r, 12, 0.0);
                    d.alacak = VeriErisim.DegerAl(r, 13, 0.0);
                    d.aciklama = VeriErisim.DegerAl(r, 14, "");
                    os.detay.objeler.Add(d);
                    os.borcToplam += d.borc;
                    os.alacakToplam += d.alacak;
                    //****************************************
                    if (eskiBelgeNo != belgeNo)
                    {
                        if (os.birim != "" && birimKurumsal != "")
                            os.birimAd = MUHServis.UzayYaziDegerGetir(veBUM, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString() + "-" + birimKurumsal, true, "BIRIMAD");
                    }
                    eskiBelgeNo = belgeNo;
                }
                if (os != null)
                {
                    //Eğer belgede kurumsal kod yok ise birim adı doldurulamıyor
                    if (string.IsNullOrEmpty(os.birimAd))
                        os.birimAd = MUHServis.UzayYaziDegerGetir(veBUM, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString(), false, "BIRIMAD");
                    oslar.objeler.Add(os);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            veBUM.VeriErisimKapat();
            return oslar;
        }
        /// <summary>
        /// Maas bilgileri islendi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="fisNo">fis no</param>
        public void MaasEntegrasyouIslendi(Kullanici kullanan, string fisNo)
        {
            if (fisNo != "")
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriMAAS", TNS.KYM.Arac.KimlikBul(kullanan));
                TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
                to.MaasEntegrasyonuIslendi(fisNo);
                ve.VeriErisimKapat();
            }
        }
        /// <summary>
        /// Kapanis bilgilerini kaydet
        /// </summary>
        /// <param name="kullanan">Kullanan.</param>
        /// <param name="liste">KapanisBilgileri türünde nesneleri içeren Liste.</param>
        /// <returns></returns>
        public Sonuc KapanisBilgisiKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc();
            KapanisBilgileri kapatmaKaydi = (KapanisBilgileri)liste.objeler[0];
            if (kapatmaKaydi.yil == 0 || kapatmaKaydi.belgeNo == "")
            {
                sonuc.DegerAta("Kapatma Yılı veya belge numarası boş geldi");
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            //Ödeme emri ekranından kayıt ediliyorsa, kapatma belgesi kaldırılabilir ve hic kapatma bilgisi gelmeyebilir. Bu nedenle önceden kayıt edilenlerin silinmesi için
            if (kapatmaKaydi.belgeNo != "-1")
                sonuc = to.KapanisBilgisiIptalEt(kapatmaKaydi);
            foreach (KapanisBilgileri kapatma in liste.objeler)
            {
                if (kapatma.kBelgeNo == "" || kapatma.kBelgeNo == "0") continue;
                if (kapatma.yil != kapatma.kYil)
                    sonuc.DegerAta("Kapatma belgesi ile açılış belgesi aynı yıla ait olmalıdır!");
                else
                    sonuc = to.KapanisBilgisiKaydet(kapatma);
                if (!sonuc.islemSonuc)
                    break;
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
        /// <summary>
        /// Kapanis bilgilerini iptal eder
        /// </summary>
        /// <param name="kullanan">Kullanan.</param>
        /// <param name="liste">KapanisBilgileri türünde nesneleri içeren Liste.</param>
        /// <returns></returns>
        public Sonuc KapanisBilgisiIptalEtHazirla(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc();
            TNSCollection oeformlar = new TNSCollection();
            foreach (KapanisBilgileri kapatma in liste.objeler)
            {
                OdemeEmriMIFForm f = new OdemeEmriMIFForm();
                f.yil = kapatma.yil;
                f.belgeNo = kapatma.belgeNo;
                f.kurum = DEG.Arac.VarsayilanKurumBul();
                f.islemYapan = kullanan.kullaniciKodu;
                f.formDurum = (int)ENUMBelgeDurumu.IPTAL;
                bool bulundu = false;
                foreach (OdemeEmriMIFForm ioe in oeformlar)
                {
                    if (ioe.belgeNo == kapatma.belgeNo && kapatma.belgeNo != "-1")
                        bulundu = true;
                }
                if (!bulundu)
                    oeformlar.Add(f);
            }
            if (oeformlar.Count > 0)
                sonuc = new MUHOdemeEmriMIFServis().BelgeDurumDegistir(kullanan, oeformlar, true);
            return sonuc;
        }
        /// <summary>
        /// Kapanises the bilgisi iptal et.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">The liste.</param>
        /// <returns></returns>
        public Sonuc KapanisBilgisiIptalEt(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            foreach (KapanisBilgileri kapatma in liste.objeler)
            {
                sonuc = to.KapanisBilgisiIptalEt(kapatma);
                if (!sonuc.islemSonuc)
                    break;
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
        /// <summary>
        /// Kapanises the bilgisi listele.
        /// </summary>
        /// <param name="kriter">The kriter.</param>
        /// <returns></returns>
        public static ObjectArray KapanisBilgisiListele(KapanisBilgileri kriter)
        {
            ObjectArray pVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
            IDataReader r = null;
            try
            {
                r = tab.KapanisBilgisiListele(kriter);
                if (r == null)
                    throw new Exception(tab.Hata());
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;
                return pVAN;
            }
            while (r.Read())
            {
                KapanisBilgileri k = new KapanisBilgileri();
                k.yil = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.YIL, 0);
                k.belgeNo = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.BELGENO, "");
                k.kYil = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KYIL, 0);
                k.kBelgeNo = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KBELGENO, "");
                k.kHesapKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KHESAPKOD, "");
                k.kIlgiliVno = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KILGILIVNO, "");
                k.kTertip = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KTERTIP, "");
                k.kTutar = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KTUTAR, 0.0);
                k.siraNo = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.SIRANO, 0);
                k.kBirimKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KBIRIMKOD, "");
                pVAN.objeler.Add(k);
            }
            r.Close();
            pVAN.sonuc.islemSonuc = true;
            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            ve.VeriErisimKapat();
            return pVAN;
        }
        /// <summary>
        /// Yenis the alan olustur.
        /// </summary>
        public void YeniAlanOlustur()
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            to.YeniAlanOlustur();
            ve.VeriErisimKapat();
        }
        /// <summary>
        /// Ödeme emri / MIF kaydetme işlemi
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            EksikOdenekler eo = KaydetEksikOdenek(kullanan, form);
            if (!eo.sonuc.islemSonuc)
                DetayDebugYaz(form);
            return eo.sonuc;
        }
        /// <summary>
        /// Ödeme emri / MIF kaydetme işlemi
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public EksikOdenekler KaydetEksikOdenek(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            string yansit900 = System.Configuration.ConfigurationManager.AppSettings.Get("Yansit900905") + "";
            string yansit903 = System.Configuration.ConfigurationManager.AppSettings.Get("Yansit903905") + "";
            if (form.kurum == "")
                form.kurum = DEG.Arac.VarsayilanKurumBul();
            form.belgeTur = ENUMBelgeTuru.MIF;
            foreach (OdemeEmriMIFDetay d in form.detay.objeler)
            {
                if (d.hesapKod.StartsWith("835"))
                {
                    if (d.alacak > 0)
                        form.belgeTur = ENUMBelgeTuru.ODEMEEMRI;
                    if (OrtakFonksiyonlar.ConvertToInt(yansit900, 0) > 0 || OrtakFonksiyonlar.ConvertToInt(yansit903, 0) > 0)
                    {
                        string islemHesap = "";
                        if (OrtakFonksiyonlar.ConvertToInt(yansit900, 0) > 0) islemHesap = "900";
                        if (OrtakFonksiyonlar.ConvertToInt(yansit903, 0) > 0) islemHesap = "903";
                        Satir90x905Olustur(form, islemHesap);
                    }
                    break;
                }
            }
            EksikOdenekler eoKontrolsuz = new EksikOdenekler();
            EksikOdenekler eo = KontrolEt(kullanan, form, eoKontrolsuz);
            if (!eo.sonuc.islemSonuc)
            {
                return eo;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            if (string.IsNullOrWhiteSpace(form.belgeNo))
            {
                if (!form.aktarim || form.formDurum == 0)
                    form.formDurum = 1;
                //Belgeye numara alınıyor
                //**********************************************************
                TABGenel tg = new TABGenel(ve);
                string sayacKod = form.yil + form.kurum.Replace(".", "");
                sayacKod = "ODE" + sayacKod;
                form.belgeNo = tg.BelgeNoAl(sayacKod, "000000");
                //**********************************************************
                //Eğer belgeno alındı ise aşağıda başına kurum kodu konuluyor
                //sonuc.Çıkışta Sonuç Anahtar alanına konuluyor ki client tarafı yeni kayıt edilen belge numarasını alabilsin
                //**********************************************************
                if (OrtakFonksiyonlar.ConvertToInt(form.belgeNo, 0) == 0)
                {
                    ve.VeriErisimKapat();
                    eo.sonuc.DegerAta(ButceMuhasebeResI.MUH007);
                    return eo;
                }
                else
                    form.belgeNo = form.kurum.Replace(".", "").PadRight(4, '0') + form.belgeNo;
                //**********************************************************
            }
            else
            {
                if (!form.aktarim || form.formDurum == 0)
                    form.formDurum = 2;
            }
            if (OrtakFonksiyonlar.ConvertToDouble(form.belgeNo, 0) > 0)
            {
                ve.BeginTransaction();
                TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
                if (form.formDurum != 1)
                    eo.sonuc = to.Sil(form.yil, form.belgeNo, form.sadeceUstVeri);
                if (eo.sonuc.islemSonuc)
                {
                    //Kesintilerin otomatik olarak işaretlenmesi
                    //**********************************************************
                    TABHesapPlani tg = new TABHesapPlani(ve);
                    foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
                    {
                        HesapPlaniSatir hKod = new HesapPlaniSatir();
                        hKod.yil = form.yil;
                        hKod.hesapKod = detay.hesapKod;
                        tg.HesapKodOzellikAl(hKod);
                        if ((hKod.kesinti != 0 && detay.alacak > 0) || hKod.kesinti != 0)
                            detay.kesinti = 1;
                        //Belgenin ait olduğu kurumsal kodu set eder.
                        if (!string.IsNullOrEmpty(detay.kur) && string.IsNullOrEmpty(form.kurKod.Trim()))
                            form.kurKod = detay.kur;
                    }
                    //**********************************************************
                    try
                    {
                        string mifSirala = TNS.MUH.Arac.DegiskenDegerBul(form.yil, "/MIFSirala");
                        if (!string.IsNullOrEmpty(mifSirala))
                            form.detay.objeler.Sort(new DetayKiyasla(mifSirala));
                    }
                    catch
                    {
                    }
                    eo.sonuc = to.Kaydet(form);
                }
                //İlgili belgeno alanına yazılan belgelerin ilgili belgeno alanlarına bu belgeyi kaydetmek için
                if (eo.sonuc.islemSonuc)
                {
                    string[] ilgiliBelge = form.ilgiliBelgeNo.Trim().Split(';');
                    for (int i = 0; i < ilgiliBelge.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(ilgiliBelge[i]))
                        {
                            eo.sonuc = to.KaydetODEMIFIlgiliBelge(form.yil, ilgiliBelge[i], form.belgeNo);
                            if (!eo.sonuc.islemSonuc)
                            {
                                eo.sonuc.hataStr = "<br>" + string.Format(ButceMuhasebeResI.MUH140, ilgiliBelge[i]);
                                break;
                            }
                        }
                    }
                }
                if (eo.sonuc.islemSonuc)
                {
                    ve.Commit();
                    if (form.formDurum == 1)
                    {
                        eo.sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH008, form.belgeNo) + "<br>" + eo.BilgiStr();
                        eo.sonuc.anahtar = form.belgeNo;
                    }
                    else
                    {
                        eo.sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH141, form.belgeNo) + "<br>" + eo.BilgiStr();
                        eo.sonuc.anahtar = form.belgeNo;
                    }
                    //Mahsup belge no kayıt etme işlemi Melih 05.07.2023
                    KapanisKayitlariniYaz(kullanan, form.yil, form.belgeNo, form.ilgiliNo, form.detay);

                    if (!string.IsNullOrEmpty(form.surecNo) && form.surecYil > 0)
                        servisHRC.BlokeTertipleriOtomatikGuncelle(kullanan, form.surecYil, form.surecNo, form.yil, ENUMKontrolDurum.KontrolEtme);
                    Sonuc s = DortDuzeyDegisimIslemleri(ve, kullanan, form);
                    if (!s.islemSonuc)
                    {
                        eo.sonuc.bilgiStr += " (" + s.hataStr + ")";
                    }
                    s = OdenekTalepIslemleri(kullanan, form, eoKontrolsuz);
                    if (!s.islemSonuc)
                    {
                        eo.sonuc.bilgiStr += " (" + s.hataStr + ")";
                    }
                    else if (!string.IsNullOrWhiteSpace(s.anahtar))
                    {
                        eo.sonuc.bilgiStr += " (" + s.bilgiStr + ")";
                    }
                }
                else
                    ve.Rollback();
            }
            else
                eo.sonuc.DegerAta(ButceMuhasebeResI.MUH007);
            ve.VeriErisimKapat();
            return eo;
        }

        private void DebugYazOE(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            if (kullanan.kullaniciKodu == "stratek")
            {
                foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
                {
                    DebugYaz($"{detay.TertipStr()}:{detay.BorcAlacakNet}");
                }
            }
        }
        public static Sonuc MahsupYapilmamisBelgeBul(VeriErisim veriErisim, Kullanici kullanan, int yil, string hesapKod, string altHesapKod, bool borclu, string belgeNo, double miktar)
        {
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            //OrtakFonksiyonlar.HataDosyaYaz("MahsupYapilmamisBelgeBul.txt", $"yil:{yil},hesapKod:{hesapKod}, altHesapKod:{altHesapKod}, borclu:{borclu}, belgeNo:{belgeNo}, miktar:{miktar}", true);
            TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
            s.anahtar = tab.MahsupYapilmamisBelgeBul(yil, hesapKod, altHesapKod, borclu, belgeNo, miktar);
            //OrtakFonksiyonlar.HataDosyaYaz("MahsupYapilmamisBelgeBul.txt", $"s.anahtar:{s.anahtar},ve.hata:{ve.hata}", false);
            if (!string.IsNullOrWhiteSpace(ve.hata))
                s.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectVAN MahsupSatiriBul(VeriErisim veriErisim, Kullanici kullanan, int yil, string mahsupBelgeNo, string hesapKod, string altHesapKod, bool borclu, double miktar)
        {
            ObjectVAN ov = new ObjectVAN();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
            IDataReader dr = tab.MahsupSatiriBul(yil, mahsupBelgeNo, hesapKod, altHesapKod, borclu, miktar);
            if (dr != null)
            {
                if (dr.Read())
                {
                    OdemeEmriMIFDetay d = new OdemeEmriMIFDetay(yil);
                    d.yil = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.YIL, 0);
                    d.belgeTur = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.BELGETUR, 0);
                    d.belgeNo = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.BELGENO, "");
                    d.satirNo = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.SIRA, 0);
                    d.hesapKod = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.HESAPKOD, "");
                    d.kur = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.KURKOD, "");
                    d.fon = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.FONKOD, "");
                    d.fin = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.FINKOD, "");
                    d.eko = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.EKOKOD, "");
                    d.borc = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.BORC, 0.0);
                    d.alacak = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.ALACAK, 0.0);
                    d.kesinti = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.KESINTI, 0);
                    d.peb = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.PEBKOD, "");
                    d.altHesapKod = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.ALTHESAPKOD, "");
                    d.doviz = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.DOVIZ, "");
                    d.dovizKurTur = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.DOVIZKURTUR, "");
                    d.dovizBorc = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.DOVIZBORC, 0.0);
                    d.dovizAlacak = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.DOVIZALACAK, 0.0);
                    d.dovizKur = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.DOVIZKUR, (decimal)0);
                    d.satirAciklama = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.ACIKLAMA, "");
                    d.detayBirim = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.DETAYBIRIM, "");
                    d.isKod = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarMIFD.ISKOD, 0);
                    ov.o = d;
                }
            }
            else
                ov.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return ov;
        }
        private Sonuc KapanisKayitlariniYaz(Kullanici kullanan, int yil, string belgeNo, string ilgiliVNo, ObjectArray liste)
        {
            ObjectArray kapatmalar = new ObjectArray();
            int siraNo = 1;
            foreach (OdemeEmriMIFDetay detay in liste.objeler)
            {
                KapanisBilgileri kapatma = new KapanisBilgileri();
                kapatma.yil = yil;
                kapatma.belgeNo = belgeNo;
                kapatma.kYil = yil;
                kapatma.kBelgeNo = detay.mahsupBelgeNo;
                kapatma.kHesapKod = detay.hesapKod;
                kapatma.kIlgiliVno = detay.altHesapKod;
                kapatma.kBirimKod = detay.detayBirim;
                //if (kapatma.kIlgiliVno.Trim() == "")//Sadece altHesapKod'da yazan olacak kapatma ekranından yapılıyprsa ??? Melih 12.06.2017
                //    kapatma.kIlgiliVno = ilgiliVNo;
                kapatma.kTertip = detay.TertipStr();//Cumhur tertip olması gerekiyor 05.07.2023 Melih. (Öncesinden hesap kodu gönderiliyordu???? Neden?)
                kapatma.kTutar = Math.Abs(detay.borcYPB - detay.alacakYPB);
                kapatma.siraNo = detay.satirNo;
                kapatma.borcAlacak = 1;
                if (detay.alacak > 0) kapatma.borcAlacak = 2;
                if (kapatma.siraNo == 0) kapatma.siraNo = siraNo;
                siraNo++;
                if (!string.IsNullOrEmpty(detay.mahsupBelgeNo))
                    kapatmalar.objeler.Add(kapatma);
            }
            if (kapatmalar.objeler.Count == 0)
            {
                KapanisBilgileri kapatma = new KapanisBilgileri();
                kapatma.yil = yil;
                kapatma.belgeNo = belgeNo;
                kapatmalar.objeler.Add(kapatma);
            }
            return KapanisBilgisiKaydet(kullanan, kapatmalar);//Daha önce kayıt edilipde silinmek istenen kayıtların silinebilmesi için boş olsada gönderiliyor. 07.07.2014 Melih
        }
        private Sonuc OdenekTalepIslemleri(Kullanici kullanan, OdemeEmriMIFForm formOE, EksikOdenekler eoKontrolsuz)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (eoKontrolsuz == null || eoKontrolsuz.eksikOdenekler == null || eoKontrolsuz.eksikOdenekler.Length == 0)
                return sonuc;
            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            string odenekAciklama = "(" + formOE.yil + "/" + formOE.belgeNo + " ödeme emri için) " + formOE.aciklama;
            GondermeTenkisForm form = new GondermeTenkisForm();
            form.belgeTur = ENUMBelgeTuru.ODENEKGONDERME;
            form.yil = formOE.yil;
            form.kurum = formOE.kurum;
            form.muhasebe = formOE.muhasebe;
            form.birim = formOE.birim;
            form.surecYil = formOE.surecYil;
            form.surecNo = formOE.surecNo;
            form.DortDuzeyDegisim = 0;
            form.talep = "1";
            form.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YENITALEP;
            //---------------
            form.islemTarih = new TNSDateTime(DateTime.Now.ToShortDateString());
            form.aciklama = odenekAciklama;
            form.islemYapan = kullanan.kullaniciKodu;
            form.yaziNo = "";
            form.yaziTarih = new TNSDateTime();
            form.istekYaziNo = "";
            form.istekYaziTarih = new TNSDateTime();
            if (form.belgeNo == "")
                form.icmalNo = 0;
            double toplam = 0;
            foreach (EksikOdenekBilgi to in eoKontrolsuz.eksikOdenekler)
            {
                if (to.eko.StartsWith("01") || to.eko.StartsWith("02")) continue;
                GondermeTenkisDetay detay = new GondermeTenkisDetay();
                detay.tertip = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
                detay.tertip.kodlar[0] = to.kur;
                detay.tertip.kodlar[1] = to.fon;
                detay.tertip.kodlar[2] = to.fin;
                detay.tertip.kodlar[3] = to.eko;
                detay.tertip.kodlar[4] = to.peb;
                detay.tertip.Noktala();
                detay.miktar = to.IHTIYAC;
                form.detay.objeler.Add(detay);
                toplam += detay.miktar;
            }
            try
            {
                if (form.detay.ObjeSayisi > 0)
                {
                    sonuc = (new MUHGondermeTenkisServis()).Kaydet(kullanan, form, false, true, false, false);
                    if (sonuc.islemSonuc)
                    {
                        sonuc.bilgiStr = string.Format("Ödeme emri kaydedilirken <b>{0}</b> numaralı, <b>{1}</b> liralık ödenek talebi üretildi!", sonuc.anahtar, toplam);
                    }
                }
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.StackTrace + " " + ex.Message);
            }
            return sonuc;
        }
        private Sonuc DortDuzeyDegisimIslemleri(VeriErisim ve, Kullanici kullanan, OdemeEmriMIFForm form)
        {
            Sonuc s = new Sonuc(true, "", "");
            ENUMOdenekKontrolTur ogKontrolTertipTur = Arac.OdenekKontrolTurBul(form.yil);
            if (!(Arac.DortDuzeyDegisimOlacakMi(ogKontrolTertipTur))) return s;
            //string hataStr = "";
            ENUMOdenekUstuKontrol ouKontrol = Arac.OdenekUstuKontrolBul();
            string birim = form.birim;
            string ortak000 = TNS.MUH.Arac.DegiskenDegerBul(form.yil, "/Ortak000");
            if (ortak000 == "1")
                birim = "";
            List<OdemeEmriMIFDetay> l = new List<OdemeEmriMIFDetay>();
            form.OdenekKontroluTertipleriBul(ENUMOdenekKontrolTur.E4POG_E4POE_E4PK, true, l, null);
            List<EksikOdenekBilgi> eobler = new List<EksikOdenekBilgi>();
            foreach (OdemeEmriMIFDetay item in l)
            {
                EksikOdenekBilgi eob = OdenekTalepYapilacakMi2(ve, kullanan, form.yil, form.muhasebe, birim, item.kur, item.fon, item.fin, item.eko, item.borc, item.peb, form.belgeNo, form.surecYil, form.surecNo, ouKontrol, false, false, false, false);
                if (eob.IHTIYAC > 0)
                {
                    eobler.Add(eob);
                }
            }
            if (eobler.Count == 0) return s;
            TertipOdenek[] uyumluListe = RevizeTertipListele(ve, kullanan, form, birim, eobler, ouKontrol);
            try
            {
                s = OdenekGondermeDortDuzeyDegisimOlustur(kullanan, eobler, uyumluListe, form, birim);
            }
            catch (Exception ex)
            {
                s.DegerAta(ex.Message);
            }
            return s;
        }
        private static Sonuc OdenekGondermeDortDuzeyDegisimOlustur(Kullanici kullanan, List<EksikOdenekBilgi> tertipListe, TertipOdenek[] uyumluListe, OdemeEmriMIFForm formOE, string birim)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            string odenekAciklama = formOE.yil + "/" + formOE.belgeNo + " ödeme emri için üretilmiş 4 düzey değişim formu";
            GondermeTenkisForm form = new GondermeTenkisForm();
            form.belgeTur = ENUMBelgeTuru.ODENEKGONDERME;
            form.yil = formOE.yil;
            form.kurum = formOE.kurum;
            form.muhasebe = formOE.muhasebe;
            form.birim = formOE.birim;
            form.surecYil = formOE.surecYil;
            form.surecNo = formOE.surecNo;
            form.DortDuzeyDegisim = 1;
            form.talep = "harcama";
            form.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.ONAYLI;
            //---------------
            form.islemTarih = new TNSDateTime(DateTime.Now.ToShortDateString());
            form.aciklama = odenekAciklama;
            form.islemYapan = kullanan.kullaniciKodu;
            form.yaziNo = "";
            form.yaziTarih = new TNSDateTime();
            form.istekYaziNo = "";
            form.istekYaziTarih = new TNSDateTime();
            if (form.belgeNo == "")
                form.icmalNo = 0;
            foreach (EksikOdenekBilgi to in tertipListe)
            {
                GondermeTenkisDetay detay = new GondermeTenkisDetay();
                detay.tertip = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
                detay.tertip.kodlar[0] = to.kur;
                detay.tertip.kodlar[1] = to.fon;
                detay.tertip.kodlar[2] = to.fin;
                detay.tertip.kodlar[3] = to.eko;
                detay.tertip.kodlar[4] = to.peb;
                detay.tertip.Noktala();
                detay.miktar = to.IHTIYAC;
                form.detay.objeler.Add(detay);
                double kalanIhtiyac = detay.miktar;
                foreach (TertipOdenek uyumluTertip in uyumluListe)
                {
                    double kalan = uyumluTertip.odenek[0];
                    if (kalan > 0 && TertipUyumluMu(uyumluTertip.tertip, detay.tertip))
                    {
                        GondermeTenkisDetay detayUy = new GondermeTenkisDetay();
                        detayUy.tertip = new Tertip(formOE.yil, ENUMTabloKod.TABPEBOdenek);
                        detayUy.tertip.TertipAta(uyumluTertip.tertip);
                        double dus;
                        if (kalan >= kalanIhtiyac)
                        {
                            dus = kalanIhtiyac;
                            kalanIhtiyac = 0;
                        }
                        else
                        {
                            dus = kalan;
                            kalanIhtiyac -= dus;
                        }
                        uyumluTertip.odenek[0] -= dus;
                        detayUy.miktar = dus * -1;
                        DetayaEkle(form.detay, detayUy);
                    }
                    if (kalanIhtiyac <= 0) break;
                }
            }
            try
            {
                sonuc = (new MUHGondermeTenkisServis()).Kaydet(kullanan, form, false, true, false, true);
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.StackTrace + " " + ex.Message);
            }
            return sonuc;
        }
        private static void DetayaEkle(ObjectArray detay, GondermeTenkisDetay detayUy)
        {
            for (int i = 0; i < detay.ObjeSayisi; i++)
            {
                GondermeTenkisDetay eskiDetay = (GondermeTenkisDetay)detay.objeler[i];
                if (eskiDetay.tertip.TertipStrOl() == detayUy.tertip.TertipStrOl())
                {
                    eskiDetay.miktar += detayUy.miktar;
                    return;
                }
            }
            detay.objeler.Add(detayUy);
        }
        private static bool TertipUyumluMu(Tertip tertip1, Tertip tertip2)
        {
            if (tertip1.kodlar.Length != 5 || tertip2.kodlar.Length != 5) return false;
            for (int i = 0; i < 5; i++)
            {
                if (i == 3)
                {
                    if (tertip1.kodlar[i] == tertip2.kodlar[i]) return false;
                    int eko2uznoktali = tertip1.KodUzunlukBul(3, 2, false);
                    if (tertip1.kodlar[i].Substring(0, eko2uznoktali) != tertip2.kodlar[i].Substring(0, eko2uznoktali)) return false;
                }
                else if (tertip1.kodlar[i] != tertip2.kodlar[i]) return false;
            }
            return true;
        }
        private TertipOdenek[] RevizeTertipListele(VeriErisim ve, Kullanici kullanan, OdemeEmriMIFForm form, string birim, List<EksikOdenekBilgi> tertipListe, ENUMOdenekUstuKontrol ouKontrol)
        {
            Tertip tUzBul = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);
            IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            TNS.UZY.IUZYServis servisUZY = MUHServis.servisUZY;
            ArrayList al = new ArrayList();
            foreach (EksikOdenekBilgi eob in tertipListe)
            {
                Tertip t = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek, eob.kur, eob.fon, eob.fin, eob.eko.Replace(".", "").Substring(0, eko2Uz), eob.peb);
                TertipOdenek[] liste = servisBHMv2.MPBirimTertipListele(t, form.muhasebe, birim);
                foreach (TertipOdenek tod in liste)
                {
                    bool bulundu = false;
                    TertipOdenek todenek = new TertipOdenek(3);
                    todenek.tertip = tod.tertip;
                    for (int i = 0; i < al.Count; i++)
                    {
                        if (((TertipOdenek)al[i]).tertip.TertipStrOl() == todenek.tertip.TertipStrOl())
                        {
                            bulundu = true;
                            break;
                        }
                    }
                    if (!bulundu)
                    {
                        EksikOdenekBilgi eob2 = OdenekTalepYapilacakMi2(ve, kullanan, form.yil, form.muhasebe, birim, todenek.tertip.kodlar[0], todenek.tertip.kodlar[1], todenek.tertip.kodlar[2], todenek.tertip.kodlar[3], 0, todenek.tertip.kodlar[4], "", 0, "", ouKontrol, false, false, false, false);
                        todenek.odenek[0] = eob2.ODENEK - eob2.HARCAMA - eob2.BLOKE - eob2.AVANS;
                        if (todenek.odenek[0] > 0)
                            al.Add(todenek);
                    }
                }
            }
            return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
        }
        /// <summary>
        /// Sils the oybs.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        public Sonuc SilOybs(Kullanici kullanan, int yil, string belgeNo)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";
            if (yil <= 0)
                hata += "Yıl bilgisi boş bırakılamaz.<br>";
            if (string.IsNullOrEmpty(belgeNo))
                hata += "Belge numarası bilgisi boş bırakılamaz.";
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
            sonuc = tab.SilOybs(yil, belgeNo);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
        /// <summary>
        /// Kaydets the oybs.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="oybsStr">The oybs STR.</param>
        /// <returns></returns>
        public Sonuc KaydetOybs(Kullanici kullanan, int yil, string belgeNo, string oybsStr)
        {
            Sonuc sonuc = new Sonuc();
            if (!string.IsNullOrEmpty(oybsStr))
            {
                SilOybs(kullanan, yil, belgeNo);
                //sonuc = SilOybs(kullanan, yil, belgeNo);
                //if (!sonuc.islemSonuc)
                //    return sonuc;
                //islemtip1-oybsId1;islemtip2-oybsId2
                string[] liste = oybsStr.Split(';');
                int islemTip = 0;
                long oybsId = 0;
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                ve.BeginTransaction();
                TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
                for (int i = 0; i < liste.Length; i++)
                {
                    if (!string.IsNullOrEmpty(liste[i]))
                    {
                        string[] deger = liste[i].Split('-');
                        islemTip = !string.IsNullOrEmpty(deger[0]) ? int.Parse(deger[0]) : -1;
                        oybsId = !string.IsNullOrEmpty(deger[1]) ? long.Parse(deger[1]) : -1;
                        string hata = "";
                        if (islemTip < 0)
                            hata += "İşlem Tipi hatalı<br>";
                        if (oybsId <= 0)
                            hata += "OYBS belgesi geçersiz.";
                        if (hata != "")
                        {
                            sonuc.hataStr = hata;
                            sonuc.islemSonuc = false;
                            return sonuc;
                        }
                        sonuc = tab.KaydetOybs(yil, belgeNo, islemTip, oybsId);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "Kaydetme işlemi başarıyla gerçekleştirildi.";
                }
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }
            else
            {
            }
            return sonuc;
        }
        /// <summary>
        /// Listeles the oybs.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        public ObjectArray ListeleOybs(Kullanici kullanan, int yil, string belgeNo)
        {
            ObjectArray pVAN = new ObjectArray();
            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";
            //    return pVAN;
            //}
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
            IDataReader r = null;
            try
            {
                r = tab.ListeleOybs(yil, belgeNo);
                if (r == null) throw new Exception(tab.Hata());
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;
                return pVAN;
            }
            int islemTip = 0;
            string oybsId = string.Empty;
            while (r.Read())
            {
                islemTip = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarOybs.ISLEMTIP, 0);
                oybsId = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarOybs.OYBSID, string.Empty);
                pVAN.objeler.Add(oybsId.ToString());
            }
            r.Close();
            pVAN.sonuc.anahtar = islemTip.ToString();
            pVAN.sonuc.islemSonuc = true;
            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            ve.VeriErisimKapat();
            return pVAN;
        }
        /// <summary>
        /// Kaydets the dokuman.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        public Sonuc KaydetDokuman(Kullanici kullanan, Dokuman dokuman)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";
            if (dokuman.yil <= 0)
                hata += "Yıl bilgisi boş bırakılamaz.<br>";
            if (string.IsNullOrEmpty(dokuman.belgeNo))
                hata += "Belge numarası bilgisi boş bırakılamaz.";
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string muhasebeBelgeSaklamaYeri = OrtakFonksiyonlar.WebConfigOku("MuhasebeBelgeSaklamaYeri", "");
            byte[] icerik = null;
            if (muhasebeBelgeSaklamaYeri == "Dosya")
            {
                icerik = dokuman.dokumanIcerik;
                dokuman.dokumanIcerik = null;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
            dokuman.siraNo = tab.BelgeSiraNoAl(dokuman.yil, dokuman.belgeNo);
            sonuc = tab.KaydetBelgeler(dokuman);
            if (sonuc.islemSonuc)
            {
                if (muhasebeBelgeSaklamaYeri == "Dosya")
                {
                    string dosya = Path.Combine(dokuman.kayitEdilecekDosyaYol, dokuman.yil.ToString());
                    if (!Directory.Exists(dosya)) Directory.CreateDirectory(dosya);
                    dosya = Path.Combine(dosya, dokuman.yil + "_" + dokuman.belgeNo + "_" + dokuman.siraNo);
                    try
                    {
                        if (File.Exists(dosya)) File.Delete(dosya);
                    }
                    catch { }
                    File.WriteAllBytes(dosya, icerik);
                }
                ve.Commit();
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
        /// <summary>
        /// Listeles the dokuman.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        public ObjectArray ListeleDokuman(Kullanici kullanan, Dokuman dokuman)
        {
            ObjectArray pVAN = new ObjectArray();
            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";
            //    return pVAN;
            //}
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
            IDataReader r = null;
            try
            {
                r = tab.ListeleBelgeler(dokuman);
                if (r == null) throw new Exception(tab.Hata());
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;
                return pVAN;
            }
            while (r.Read())
            {
                Dokuman d = new Dokuman();
                d.yil = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarBelgeler.YIL, 0);
                d.belgeNo = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarBelgeler.BELGENO, "");
                d.siraNo = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarBelgeler.SIRANO, 0);
                d.dokumanAdi = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMAlanlarBelgeler.BELGEADI, "");
                if (dokuman.siraNo != 0)
                    try
                    {
                        d.dokumanIcerik = (byte[])(r.GetValue((int)TABOdemeEmriMIF.ENUMAlanlarBelgeler.BELGE));
                    }
                    catch { }
                pVAN.objeler.Add(d);
            }
            r.Close();
            pVAN.sonuc.islemSonuc = true;
            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            ve.VeriErisimKapat();
            return pVAN;
        }
        /// <summary>
        /// Sils the dokuman.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        public Sonuc SilDokuman(Kullanici kullanan, Dokuman dokuman)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";
            if (dokuman.yil <= 0)
                hata += "Yıl bilgisi boş bırakılamaz.<br>";
            if (string.IsNullOrEmpty(dokuman.belgeNo))
                hata += "Belge numarası bilgisi boş bırakılamaz.";
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
            sonuc = tab.SilBelgeler(dokuman);
            if (sonuc.islemSonuc)
            {
                string dosya = Path.Combine(dokuman.kayitEdilecekDosyaYol, dokuman.yil.ToString());
                dosya = Path.Combine(dosya, dokuman.yil + "_" + dokuman.belgeNo + "_" + dokuman.siraNo);
                try
                {
                    if (File.Exists(dosya)) File.Delete(dosya);
                }
                catch { }
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
        /// <summary>
        /// Tek satır olarak Ödeme emri / MIF kaydetme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc KaydetTekSatir(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            if (form.belgeNo != "")
            {
                ve.BeginTransaction();
                TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
                sonuc = to.KaydetODEMIFTekSatir(form);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH141, form.belgeNo) + "<br>";
                    sonuc.anahtar = form.belgeNo;
                }
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }
            return sonuc;
        }
        /// <summary>
        /// Ödeme emri / MIF kaydetme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <param name="ilgiliBelgeNo">İlgili belge no</param>
        /// <returns></returns>
        public Sonuc KaydetIlgiliBelge(Kullanici kullanan, int yil, string belgeNo, string ilgiliBelgeNo)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            if (belgeNo != "")
            {
                ve.BeginTransaction();
                TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
                sonuc = to.KaydetODEMIFIlgiliBelge(yil, belgeNo, ilgiliBelgeNo);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH141, belgeNo) + "<br>";
                    sonuc.anahtar = belgeNo;
                }
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }
            return sonuc;
        }
        /// <summary>
        /// Yıl sonu hesap kapanış belgelerini ve verilen koşullara göre üretir
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public ObjectArray YilSonuBelgeUret(Kullanici kullanan, OdemeEmriMIFForm form, int tur, string islem)
        {
            MUHOdemeEmriMIFServis oemServis = new MUHOdemeEmriMIFServis();
            string islemYapilacakHesapKod = string.Empty;
            string borcHesapKod = string.Empty;
            string alacakHesapKod = string.Empty;
            ObjectArray belgeler = new ObjectArray();
            Hashtable hesapKodHT = new Hashtable();
            ObjectArray yeniDetay = new ObjectArray();
            ObjectArray sonuc = new ObjectArray();
            form.fisTurYap(ENUMFISTUR.KAPANIS);
            bool odenekGondermeYok = TNS.MUH.Arac.OdenekGondermeYokMu(form.yil);
            string belgeAciklama = "";
            if (tur == 1) belgeAciklama = "ÖDENEKLERİN GÖNDERİLMESİ-1";
            if (tur == 2) belgeAciklama = "ÖDENEKLERİ GÖNDERİLMESİ-2";
            if (tur == 3) belgeAciklama = "TENKİS İŞLEMLERİ-1";
            if (tur == 4) belgeAciklama = "TENKİS İŞLEMLERİ-2";
            if (tur == 5) belgeAciklama = "ÖDENEK İPTALİ";
            if (tur == 6) belgeAciklama = "ÖDENEKLİ GİDERLERİN KAPATILMASI";
            if (tur == 7) belgeAciklama = "BÜTÇE ÖDENEKLERİNİN KAPATILMASI";
            if (tur == 8) belgeAciklama = "AMORTİSMAN HESAPLARI";
            if (tur == 9) belgeAciklama = "BÜTÇE GİDERLERİN KAPATILMASI";
            if (tur == 10) belgeAciklama = "RED VE İADELERİN KAPATILMASI";
            if (tur == 11) belgeAciklama = "GİDER YANSITMANIN KAPATILMASI";
            if (tur == 12) belgeAciklama = "BÜTÇE GELİRLERİNİN KAPATILMASI";
            if (tur == 13) belgeAciklama = "GELİR YANSITMANIN KAPATILMASI";
            if (tur == 14) belgeAciklama = "FAALİYET GİDERLERİNİN KAPATILMASI";
            if (tur == 15) belgeAciklama = "FAALİYET GELİRLERİNİN KAPATILMASI";
            if (tur == 16) belgeAciklama = "FAALİYET HESAPLARININ KAPATILMASI";
            if (tur == 18) belgeAciklama = "AKTİF VE PASİF HESAPLARIN KAPANIŞ KAYDI";
            if (tur == 19) belgeAciklama = "AKTİF VE PASİF HESAPLARIN AÇILIŞ KAYDI";
            if (tur == 20) belgeAciklama = "NAKİT HAREKETLERİ";
            if (tur == 21) belgeAciklama = "DEĞER HAREKETLERİ";
            if (tur == 22) belgeAciklama = "FAALİYET SONUÇLARI";
            if (tur == 23) belgeAciklama = "120 HESABI KAPATILMASI";
            if (tur == 24) belgeAciklama = "320 HESABI KAPATILMASI";
            belgeAciklama = form.yil + " YIL SONU İŞLEMLERİ (" + belgeAciklama + ")";
            if (tur == 1)
            {
                islemYapilacakHesapKod = "900";
                borcHesapKod = "902";
                alacakHesapKod = "900";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 2);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 2)
            {
                islemYapilacakHesapKod = "900";
                borcHesapKod = "903";
                alacakHesapKod = "904";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 2);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 3)
            {
                islemYapilacakHesapKod = "903";
                borcHesapKod = "904";
                alacakHesapKod = "903";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 8);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 4)
            {
                islemYapilacakHesapKod = "903";
                borcHesapKod = "900";
                alacakHesapKod = "902";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 2);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 5)
            {
                islemYapilacakHesapKod = "900";
                borcHesapKod = "90125";
                alacakHesapKod = "900";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 2);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 6)
            {
                islemYapilacakHesapKod = "905";
                borcHesapKod = "904";
                alacakHesapKod = "905";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 2);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 7)
            {
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, "901");
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 2);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, "901", "901");
                islemYapilacakHesapKod = odenekGondermeYok ? "904" : "902";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 2);
                ObjectArray yeniDetay2 = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, string.Empty, islemYapilacakHesapKod);
                foreach (OdemeEmriMIFDetay item in yeniDetay2.objeler)
                {
                    yeniDetay.Ekle(item);
                }
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 8)
            {
                sonuc.sonuc.DegerAta("8 numaralı işlemin kapatma işlemleri hazırlanmamıştır!");
            }
            else if (tur == 9)
            {
                islemYapilacakHesapKod = "830";
                borcHesapKod = "895.";
                alacakHesapKod = "830";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 4);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 10)
            {
                islemYapilacakHesapKod = "810";
                borcHesapKod = "895.";
                alacakHesapKod = "810";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 4);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 11)
            {
                islemYapilacakHesapKod = "835";
                borcHesapKod = "835.";
                alacakHesapKod = "895.";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 4);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 12)
            {
                islemYapilacakHesapKod = "800";
                borcHesapKod = "800";
                alacakHesapKod = "895.";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 4);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 13)
            {
                islemYapilacakHesapKod = "805";
                borcHesapKod = "895.";
                alacakHesapKod = "805.";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 4);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 14)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                    {
                        islemYapilacakHesapKod = "630";
                        borcHesapKod = "690.";
                        alacakHesapKod = "630";
                    }
                    else
                    {
                        islemYapilacakHesapKod = "610";
                        borcHesapKod = "690.";
                        alacakHesapKod = "610";
                    }
                    belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                    hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 4);
                    ObjectArray tmpBelgeler = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                    foreach (OdemeEmriMIFDetay item in tmpBelgeler.objeler)
                    {
                        yeniDetay.objeler.Add(item);
                    }
                }
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 15)
            {
                islemYapilacakHesapKod = "600";
                borcHesapKod = "600";
                alacakHesapKod = "690.";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 4);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 16)
            {
                islemYapilacakHesapKod = "690";
                borcHesapKod = "591.";
                alacakHesapKod = "590.";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 4);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 18)
            {
                yeniDetay = YilSonuIslemleriSonKayitOlustur(kullanan, oemServis, form, false);
                form.fisTurYap(ENUMFISTUR.SONFIS);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                if (yeniDetay.sonuc.islemSonuc)
                    sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
                else
                    return yeniDetay;
            }
            else if (tur == 19)
            {
                yeniDetay = YilSonuIslemleriSonKayitOlustur(kullanan, oemServis, form, true);
                form.fisTurYap(ENUMFISTUR.ACILIS);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, true, islem, tur);
            }
            else if (tur == 20)
            {//Nakit
                yeniDetay = YilSonuIslemleriNakitHareketleri(kullanan, oemServis, form, false);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 21)
            {
                yeniDetay = YilSonuIslemleriDegerHareketleri(kullanan, oemServis, form, false);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 22)
            {
                yeniDetay = YilSonuIslemleriFaaliyetSonuclari(kullanan, oemServis, form, false);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 23)
            {
                islemYapilacakHesapKod = "120";
                borcHesapKod = "121";
                alacakHesapKod = "120";
                belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 2);
                yeniDetay = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            else if (tur == 24)
            {
                for (int i = 6; i > 0; i--)
                {
                    islemYapilacakHesapKod = "320" + (i).ToString("00");
                    borcHesapKod = "320" + (i).ToString("00");
                    alacakHesapKod = "320" + (i + 1).ToString("00");
                    if (i == 6) alacakHesapKod = "6000301029999";
                    belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                    hesapKodHT = YilSonuIslemleriIslemYapilacakBelgeleriGrupla(belgeler, tur, 2);
                    ObjectArray tmpBelgeler = YilSonuIslemleriIslemBelgeDetaylariniOlustur(hesapKodHT, tur, form.yil, borcHesapKod, alacakHesapKod);
                    foreach (OdemeEmriMIFDetay item in tmpBelgeler.objeler)
                    {
                        yeniDetay.objeler.Add(item);
                    }
                }
                form.aciklama = belgeAciklama;
                form.ilgiliAd = belgeAciklama;
                sonuc = YilSonuIslemleriIslemBelgeOlustur(kullanan, oemServis, form, yeniDetay, false, islem, tur);
            }
            if (sonuc.sonuc.islemSonuc && islem == "1")
            {
                MUHYilSonuIslemleri yilSonuServis = new MUHYilSonuIslemleri();
                Sonuc sonucGuncelle = yilSonuServis.GuncelleBelgeNo(kullanan, tur, form.yil, sonuc.sonuc.anahtar, form.muhasebe);
                if (!sonucGuncelle.islemSonuc)
                    sonuc.sonuc.DegerAta("Belge no kaydedilemedi.");
            }
            return sonuc;
        }
        private ObjectArray YilSonuIslemleriNakitHareketleri(Kullanici kullanan, MUHOdemeEmriMIFServis oemServis, OdemeEmriMIFForm form, bool v)
        {
            ObjectArray yeniDetay = new ObjectArray();
            OdemeEmriMIFDetay kfDetay = new OdemeEmriMIFDetay(form.yil);
            string islemYapilacakHesapKod = "510,511";
            ObjectArray belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
            Hashtable htBelge = new Hashtable();
            foreach (OdemeEmriMIFForm belge in belgeler.objeler)
            {
                foreach (OdemeEmriMIFDetay belgeDetay in belge.detay.objeler)
                {
                    TNS.MUH.Arac.HTDoubleEkle(htBelge, belgeDetay.hesapKod, belgeDetay.borc - belgeDetay.alacak, false);
                }
            }
            double toplam510 = 0.0;
            double toplam511 = 0.0;
            foreach (DictionaryEntry dt in htBelge)
            {
                string anahtar = (string)dt.Key;
                double tutar = (double)dt.Value;
                kfDetay = new OdemeEmriMIFDetay(form.yil);
                kfDetay.hesapKod = anahtar;
                if (tutar > 0)
                    kfDetay.alacak = Math.Abs(tutar);
                else
                    kfDetay.borc = Math.Abs(tutar);
                yeniDetay.objeler.Add(kfDetay);
                if (anahtar.StartsWith("510")) toplam510 += tutar;
                if (anahtar.StartsWith("511")) toplam511 += tutar;
            }
            if (toplam510 != 0)
            {
                kfDetay = new OdemeEmriMIFDetay(form.yil);
                kfDetay.hesapKod = "5191001";
                if (toplam510 > 0)
                    kfDetay.borc = Math.Abs(toplam510);
                else
                    kfDetay.alacak = Math.Abs(toplam510);
                yeniDetay.objeler.Add(kfDetay);
            }
            if (toplam511 != 0)
            {
                kfDetay = new OdemeEmriMIFDetay(form.yil);
                kfDetay.hesapKod = "5191101";
                if (toplam511 > 0)
                    kfDetay.borc = Math.Abs(toplam511);
                else
                    kfDetay.alacak = Math.Abs(toplam511);
                yeniDetay.objeler.Add(kfDetay);
            }
            yeniDetay.objeler.Sort(new Sirala());
            return yeniDetay;
        }
        private ObjectArray YilSonuIslemleriFaaliyetSonuclari(Kullanici kullanan, MUHOdemeEmriMIFServis oemServis, OdemeEmriMIFForm form, bool v)
        {
            ObjectArray yeniDetay = new ObjectArray();
            OdemeEmriMIFDetay kfDetay = new OdemeEmriMIFDetay(form.yil);
            double sonTutar = 0.0;
            for (int i = 0; i < 5; i++)
            {
                string islemYapilacakHesapKod = "580" + (i + 1).ToString("00");
                string karsiHesapKod = "580" + (i + 2).ToString("00");
                ObjectArray belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                double tutar = 0.0;
                foreach (OdemeEmriMIFForm belge in belgeler.objeler)
                {
                    foreach (OdemeEmriMIFDetay belgeDetay in belge.detay.objeler)
                    {
                        tutar += belgeDetay.borc - belgeDetay.alacak;
                    }
                }
                if (tutar == 0.0) continue;
                sonTutar = tutar;
                kfDetay = new OdemeEmriMIFDetay(form.yil);
                kfDetay.hesapKod = islemYapilacakHesapKod;
                if (tutar > 0)
                    kfDetay.alacak = Math.Abs(tutar);
                else
                    kfDetay.borc = Math.Abs(tutar);
                yeniDetay.objeler.Add(kfDetay);
                if (i == 4) continue;
                kfDetay = new OdemeEmriMIFDetay(form.yil);
                kfDetay.hesapKod = karsiHesapKod;
                if (tutar > 0)
                    kfDetay.borc = Math.Abs(tutar);
                else
                    kfDetay.alacak = Math.Abs(tutar);
                yeniDetay.objeler.Add(kfDetay);
            }
            kfDetay = new OdemeEmriMIFDetay(form.yil);
            kfDetay.hesapKod = "50009";
            if (sonTutar > 0)
                kfDetay.borc = Math.Abs(sonTutar);
            else
                kfDetay.alacak = Math.Abs(sonTutar);
            yeniDetay.objeler.Add(kfDetay);
            return yeniDetay;
        }
        private ObjectArray YilSonuIslemleriDegerHareketleri(Kullanici kullanan, MUHOdemeEmriMIFServis oemServis, OdemeEmriMIFForm form, bool acilisFisiMi)
        {
            ObjectArray yeniDetay = new ObjectArray();
            for (int i = 10; i <= 11; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    string islemYapilacakHesapKod = "519" + i.ToString("00") + (j + 1).ToString("00");
                    string karsiHesapKod = "519" + i.ToString("00") + (j + 2).ToString("00");
                    ObjectArray belgeler = YilSonuIslemleriIslemYapilacakBelgeleriGetir(kullanan, oemServis, form, islemYapilacakHesapKod);
                    double tutar = 0.0;
                    foreach (OdemeEmriMIFForm belge in belgeler.objeler)
                    {
                        foreach (OdemeEmriMIFDetay belgeDetay in belge.detay.objeler)
                        {
                            tutar += belgeDetay.borc - belgeDetay.alacak;
                        }
                    }
                    if (tutar == 0.0) continue;
                    OdemeEmriMIFDetay kfDetay = new OdemeEmriMIFDetay(form.yil);
                    kfDetay.hesapKod = islemYapilacakHesapKod;
                    if (tutar > 0)
                        kfDetay.alacak = Math.Abs(tutar);
                    else
                        kfDetay.borc = Math.Abs(tutar);
                    yeniDetay.objeler.Add(kfDetay);
                    kfDetay = new OdemeEmriMIFDetay(form.yil);
                    kfDetay.hesapKod = karsiHesapKod;
                    if (tutar > 0)
                        kfDetay.borc = Math.Abs(tutar);
                    else
                        kfDetay.alacak = Math.Abs(tutar);
                    yeniDetay.objeler.Add(kfDetay);
                }
            }
            return yeniDetay;
        }
        private ObjectArray YilSonuIslemleriSonKayitOlustur(Kullanici kullanan, MUHOdemeEmriMIFServis oemServis, OdemeEmriMIFForm form, bool acilisFisiMi)
        {
            ObjectArray yeniDetay = new ObjectArray();
            Hashtable bakiyeDovizHT = new Hashtable();
            Hashtable bakiyeTLHT = new Hashtable();
            Hashtable HesapKodTablo = new Hashtable();
            string varsayilanKurum = TNS.DEG.Arac.VarsayilanKurumBul();
            //bu kodlara ait belgeleri listele
            //********************************************************
            OdemeEmriMIFForm omf = new OdemeEmriMIFForm();
            OdemeEmriMIFDetay omfDetay = new OdemeEmriMIFDetay(omf.yil);
            omf.yil = form.yil;
            omf.detay = new ObjectArray();
            omf.formDurum = 6;//Yevmiyeli
            omf.sorguIslemTarih = ";" + form.islemTarih.ToString();
            omf.muhasebe = form.muhasebe;
            if (form.birim != "000")
                omf.birim = form.birim;
            omf.fisTur = null;
            //omfDetay.hesapKod = h.hesapKod;
            //omf.detay.objeler.Add(omfDetay);
            ObjectArray belgeler = oemServis.Listele(kullanan, omf, false, true, false);
            //int sayac = 0;
            //string deger = "";
            //OrtakFonksiyonlar.HataDosyaYaz("Acilis.txt", $"{DateTime.Now.ToString()}", true);
            foreach (OdemeEmriMIFForm belge in belgeler.objeler)
            {
                foreach (OdemeEmriMIFDetay belgeDetay in belge.detay.objeler)
                {
                    if (acilisFisiMi && (belgeDetay.hesapKod.StartsWith("92001") || belgeDetay.hesapKod.StartsWith("920.01"))) continue; //Bu yıla ait yüklenmelere gelecek yıla gitmemeli, 92002'ler 92001 olacak
                    if (form.birim == "000")
                    {
                        if (belgeDetay.hesapKod.StartsWith("600") ||
                            belgeDetay.hesapKod.StartsWith("630") ||
                            belgeDetay.hesapKod.StartsWith("800") ||
                            belgeDetay.hesapKod.StartsWith("830") ||
                            belgeDetay.hesapKod.StartsWith("810") ||
                            belgeDetay.hesapKod.StartsWith("805") ||
                            belgeDetay.hesapKod.StartsWith("835") ||
                            belgeDetay.hesapKod.StartsWith("901") ||
                            belgeDetay.hesapKod.StartsWith("905"))
                        {
                            continue;
                        }
                    }
                    HesapPlaniSatir hKod = HesapPlaniBul(null, belge.yil, belgeDetay, HesapKodTablo);
                    //belgelerdeki tl karşılıkları bakiyede topla
                    double bakiye = belgeDetay.borc - belgeDetay.alacak;
                    bakiye = Math.Round(bakiye, 2);
                    //OrtakFonksiyonlar.HataDosyaYaz("Acilis.txt", $"{belgeDetay.hesapKod}-{belgeDetay.altHesapKod}-{bakiye}", false);
                    //belgelerdeki döviz karşılıkları bakiyede topla
                    double dovizBakiye = (belgeDetay.dovizBorc - belgeDetay.dovizAlacak);
                    dovizBakiye = Math.Round(dovizBakiye, 2);
                    //Döviz hesabı olmasa bile bazı belgelerde (bilgi amaçlı) döviz karşılıkları yazılmakta. Eğer bu kontrol olmaz ise
                    //Bu satırlara yazılmış rakamlarda kapanış ve açılış belgelerinde yer alıyor. ÖİB de karşılaşıldı 26.03.2015 MELİH
                    //*****************************
                    if (string.IsNullOrWhiteSpace(hKod.dovizKurKod))
                    {
                        belgeDetay.doviz = "";
                        belgeDetay.dovizKurTur = "";
                        dovizBakiye = 0;
                    }
                    else
                    {
                        if (belgeDetay.doviz != hKod.dovizKurKod) dovizBakiye = 0;
                        belgeDetay.doviz = hKod.dovizKurKod;
                        belgeDetay.dovizKurTur = hKod.dovizKurTurAlis;
                    }
                    if (hKod.altHesapKod == 0)
                        belgeDetay.altHesapKod = "";

                    string detayBirimKod = DetayBirimOl(hKod, belgeDetay.detayBirim);
                    //Eğer ego ise althesap kullanıyorsa birikodu 000 olsun Melih 04.03.2021 
                    //bir hesaptan açılan diğer birimden kapatılıyor. 
                    //Açılış ve kapanış fişlerinde borç ve alacak rakamlarını ayrı ayrı satırlarda gösteriyor. Sayıştay karşı çıkıyor
                    if (varsayilanKurum.Replace(".", "") == "460604")
                    {
                        if (belgeDetay.altHesapKod != "")
                            detayBirimKod = "000";
                    }
                    //Eğer özelleştirme ise birikodu 000 olsun Melih 27.04.2022
                    if (varsayilanKurum.Replace(".", "") == "4035")
                    {
                        detayBirimKod = "000";
                    }
                    string hesapKod = belgeDetay.hesapKod.Replace(".", "") + "-" + belgeDetay.altHesapKod + "-" + detayBirimKod;
                    string tertip = string.Empty;
                    //Bu hesapkodları dışında tertip yazılanları alma. Gerek yok Melih 04.03.2021 
                    bool tertipAl = false;
                    if (hesapKod.StartsWith("830") ||
                        hesapKod.StartsWith("630") ||
                        hesapKod.StartsWith("25") ||
                        hesapKod.StartsWith("150") ||
                        hesapKod.StartsWith("16") ||
                        hesapKod.StartsWith("29") ||
                        hesapKod.StartsWith("26") ||
                        hesapKod.StartsWith("9"))
                        tertipAl = true;
                    if (hKod.tertip > 0 && tertipAl)
                    {
                        string eko = belgeDetay.eko.Replace(".", "");
                        if (!string.IsNullOrEmpty(belgeDetay.peb))
                            eko = eko + "-" + belgeDetay.peb;
                        tertip = string.Format("{0}-{1}-{2}-{3}", belgeDetay.kur.Replace(".", ""), belgeDetay.fon.Replace(".", ""), belgeDetay.fin.Replace(".", ""), eko);
                    }
                    else if (detayBirimKod != "000" && detayBirimKod != "")
                        tertip = string.Format("{0}", belgeDetay.kur.Replace(".", ""));//Birimin kurumsal kodu için Melih TKDK sorun bildirdi 21.01.2018
                    if (hesapKod.StartsWith("830") || hesapKod.StartsWith("630"))
                    {
                        belgeDetay.doviz = "";
                        belgeDetay.dovizKurTur = "";
                        dovizBakiye = 0;
                    }
                    //*****************************
                    if (acilisFisiMi)
                    {
                        if (hesapKod.StartsWith("92002"))
                            hesapKod = hesapKod.Substring(0, 4) + "1" + hesapKod.Substring(5);
                        //if (hesapKod.StartsWith("92003"))
                        //    hesapKod = hesapKod.Replace("92003", "92002");
                        //if (hesapKod.StartsWith("92004"))
                        //    hesapKod = hesapKod.Replace("92004", "92003");
                        //if (hesapKod.StartsWith("92005"))
                        //    hesapKod = hesapKod.Replace("92005", "92004");
                    }
                    //*****************************
                    try
                    {
                        string anahtar = hesapKod + "@" + tertip + "@" + belgeDetay.doviz + "@" + belgeDetay.dovizKurTur;
                        //OrtakFonksiyonlar.HataDosyaYaz("Acilis.txt", $"@{anahtar}", false);
                        TNS.MUH.Arac.HTDoubleEkle(bakiyeDovizHT, anahtar, dovizBakiye, true);
                        TNS.MUH.Arac.HTDoubleEkle(bakiyeTLHT, anahtar, bakiye, true);
                    }
                    catch (Exception ex)
                    {
                        yeniDetay.sonuc.islemSonuc = false;
                        yeniDetay.sonuc.hataStr = "HATA --- <br>Belge No: " + form.belgeNo +
                                                "<br> Belge Sıra No: " + belgeDetay.satirNo +
                                                "<br> Tertip: " + tertip +
                                                "<br> Hesap Kod: " + hesapKod +
                                                "<br> Eko Kod: " + belgeDetay.eko +
                                                "<br> Hata: " + ex.Message;
                        return yeniDetay;
                    }
                }
            }
            string kur = string.Empty;
            string fon = string.Empty;
            string fin = string.Empty;
            string eko1 = string.Empty;
            string peb = string.Empty;
            string tertip1 = string.Empty;
            string doviz = "";
            string dovizKurTur = "";
            double borcToplam = 0.0;
            double alacakToplam = 0.0;
            //OrtakFonksiyonlar.HataDosyaYaz("Acilis.txt", $"Hash taranacak", false);
            foreach (DictionaryEntry dt in bakiyeDovizHT)
            {
                kur = string.Empty;
                fon = string.Empty;
                fin = string.Empty;
                eko1 = string.Empty;
                peb = string.Empty;
                doviz = string.Empty;
                dovizKurTur = string.Empty;
                string anahtar = (string)dt.Key;
                tertip1 = string.Empty;
                double dovizBakiye = (double)bakiyeDovizHT[anahtar];
                double bakiye = (double)bakiyeTLHT[anahtar];
                dovizBakiye = Math.Round(dovizBakiye, 2);
                bakiye = Math.Round(bakiye, 2);
                //OrtakFonksiyonlar.HataDosyaYaz("Acilis.txt", $"{anahtar}", false);
                string[] anahtarListe = anahtar.Split('@');
                tertip1 = TNS.MUH.Arac.DiziElemanAl(anahtarListe, 1, string.Empty);
                doviz = TNS.MUH.Arac.DiziElemanAl(anahtarListe, 2, string.Empty);
                dovizKurTur = TNS.MUH.Arac.DiziElemanAl(anahtarListe, 3, string.Empty);
                if (!string.IsNullOrWhiteSpace(tertip1))
                {
                    var tertipListe = tertip1.Split('-');
                    kur = TNS.MUH.Arac.DiziElemanAl(tertipListe, 0, "");
                    fon = TNS.MUH.Arac.DiziElemanAl(tertipListe, 1, "");
                    fin = TNS.MUH.Arac.DiziElemanAl(tertipListe, 2, "");
                    eko1 = TNS.MUH.Arac.DiziElemanAl(tertipListe, 3, "");
                    peb = TNS.MUH.Arac.DiziElemanAl(tertipListe, 4, "");
                }
                if (bakiye != 0)
                {
                    //ilgili hesaba yazarak belge üret
                    OdemeEmriMIFDetay kfDetay = new OdemeEmriMIFDetay(form.yil);
                    string[] hesapKodAltHesapKodBirimKod = TNS.MUH.Arac.DiziElemanAl(anahtarListe, 0, string.Empty).Split('-');
                    kfDetay.hesapKod = TNS.MUH.Arac.DiziElemanAl(hesapKodAltHesapKodBirimKod, 0, "");
                    kfDetay.altHesapKod = TNS.MUH.Arac.DiziElemanAl(hesapKodAltHesapKodBirimKod, 1, "");
                    kfDetay.detayBirim = TNS.MUH.Arac.DiziElemanAl(hesapKodAltHesapKodBirimKod, 2, "");
                    kfDetay.kur = kur;
                    kfDetay.fon = fon;
                    kfDetay.fin = fin;
                    kfDetay.eko = eko1;
                    kfDetay.peb = peb;
                    kfDetay.satirAciklama = form.aciklama;
                    if (bakiye > 0)
                    {
                        if (acilisFisiMi)
                        {
                            kfDetay.borc = Math.Abs(bakiye);
                            borcToplam += Math.Abs(bakiye);
                        }
                        else
                        {
                            kfDetay.alacak = Math.Abs(bakiye);
                            alacakToplam += Math.Abs(bakiye);
                        }
                    }
                    else
                    {
                        if (acilisFisiMi)
                        {
                            kfDetay.alacak = Math.Abs(bakiye);
                            alacakToplam += Math.Abs(bakiye);
                        }
                        else
                        {
                            kfDetay.borc = Math.Abs(bakiye);
                            borcToplam += Math.Abs(bakiye);
                        }
                    }
                    if (dovizBakiye != 0)
                    {
                        kfDetay.doviz = doviz;
                        kfDetay.dovizKur = 0;
                        kfDetay.dovizKurTur = dovizKurTur;
                        if (dovizBakiye > 0)
                        {
                            if (acilisFisiMi)
                                kfDetay.dovizBorc = Math.Abs(dovizBakiye);
                            else
                                kfDetay.dovizAlacak = Math.Abs(dovizBakiye);
                        }
                        else
                        {
                            if (acilisFisiMi)
                                kfDetay.dovizAlacak = Math.Abs(dovizBakiye);
                            else
                                kfDetay.dovizBorc = Math.Abs(dovizBakiye);
                        }
                    }
                    yeniDetay.objeler.Add(kfDetay);
                }
            }
            double fark = Math.Round(borcToplam - alacakToplam, 2);
            if (fark != 0)
            {
                OdemeEmriMIFDetay kfDetay = new OdemeEmriMIFDetay(form.yil);
                kfDetay.hesapKod = "5000101";
                string tertip = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/YILSONURAKAMDUZELTMETERTIP") + "";
                kfDetay.TertipAta(tertip);
                string birim = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/YILSONURAKAMDUZELTMEBIRIM") + "";
                kfDetay.detayBirim = birim;
                if (fark > 0.0)
                {
                    kfDetay.alacak = Math.Abs(fark);
                }
                else
                {
                    kfDetay.borc = Math.Abs(fark);
                }
                yeniDetay.objeler.Add(kfDetay);
            }
            yeniDetay.objeler.Sort(new Sirala());
            return yeniDetay;
        }
        private string DetayBirimOl(HesapPlaniSatir hKod, string birimKod)
        {
            if (hKod != null && TNS.MUH.Arac.NitelikVarmi(hKod.nitelik, (int)ENUMHesapKodNitelik.SAYMANLIKBAZDABAKIYE))
                return "";
            //if (!TNS.MUH.Arac.NitelikVarmi(hKod.nitelik, (int)ENUMHesapKodNitelik.SAYMANLIKBAZDABAKIYE) && !hKod.BorcAlacakCalisirMi()) return birimKod;
            return birimKod;
        }
        private HesapPlaniSatir HesapPlaniBul(VeriErisim veriErisim, int yil, OdemeEmriMIFDetay belgeDetay, Hashtable HesapKodTablo)
        {
            HesapPlaniSatir hs = null;
            try
            {
                hs = (HesapPlaniSatir)HesapKodTablo[belgeDetay.hesapKod];
            }
            catch
            {
                hs = null;
            }
            if (hs == null)
            {
                belgeDetay.hKod = new HesapPlaniSatir();
                belgeDetay.hKod.yil = yil;
                belgeDetay.hKod.hesapKod = belgeDetay.hesapKod;
                hs = (new MUHHesapPlaniServis()).OzellikAl(veriErisim, belgeDetay.hKod);
                HesapKodTablo[belgeDetay.hesapKod] = hs;
            }
            return hs;
        }
        private ObjectArray MutemetBul(Kullanici kullanan, int yil, int muhasebe, string birim, string hesapKod, Hashtable HesapKodTablo)
        {
            ObjectArray hs = null;
            hesapKod = hesapKod.Replace(".", "");
            string anahtar = yil + muhasebe + birim + hesapKod;
            try
            {
                hs = (ObjectArray)HesapKodTablo[anahtar];
            }
            catch
            {
                hs = null;
            }
            if (hs == null)
            {
                MutemetTanim mutemet = new MutemetTanim();
                mutemet.yil = yil;
                mutemet.hesapKod = hesapKod;
                mutemet.muhasebeKod = muhasebe;
                mutemet.birimKod = birim;
                hs = new MUHMutemetTanim().Listele(kullanan, mutemet);
                HesapKodTablo[anahtar] = hs;
            }
            return hs;
        }
        class Sirala : System.Collections.IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 == o2) return 0;
                TNS.MUH.OdemeEmriMIFDetay i_o1 = (TNS.MUH.OdemeEmriMIFDetay)o1;
                TNS.MUH.OdemeEmriMIFDetay i_o2 = (TNS.MUH.OdemeEmriMIFDetay)o2;
                return (i_o1.hesapKod).CompareTo((i_o2.hesapKod));
            }
        }
        private ObjectArray YilSonuIslemleriIslemBelgeOlustur(Kullanici kullanan, MUHOdemeEmriMIFServis oemServis, OdemeEmriMIFForm form, ObjectArray yeniDetay, bool acilisFisiMi, string islem, int tur)
        {
            ObjectArray donusDeger = new ObjectArray();
            if (yeniDetay.objeler.Count > 0)
            {
                string muhasebe = form.muhasebe.ToString();
                string birim = form.birim;
                string kurKod = form.kurKod;
                string organizasyonKodu = form.organizasyonKodu.ToString();
                OdemeEmriMIFForm belge = new OdemeEmriMIFForm();
                belge.ilgiliNo = TNS.DEG.Arac.VarsayilanKurumBul();
                if (!acilisFisiMi)
                {
                    belge.yil = form.yil;
                    belge.islemTarih = form.islemTarih;
                }
                else
                {
                    belge.yil = form.yil + 1;
                    belge.islemTarih = new TNSDateTime("02.01." + (form.yil + 1).ToString());
                }
                belge.belgeNo = form.belgeNo;
                belge.muhasebe = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
                belge.birim = birim;
                belge.kurKod = kurKod;
                belge.organizasyonKodu = OrtakFonksiyonlar.ConvertToInt(organizasyonKodu, 0);
                belge.fisTur = form.fisTur;
                belge.aciklama = form.aciklama;
                belge.detay = yeniDetay;
                if (tur < 20)//Açılış fişi sonrasındakiler yıl sonu değil
                    belge.yilSonuBelgesiMi = true;
                donusDeger.Ekle(belge);
                if (islem == "1")
                {
                    donusDeger.sonuc = oemServis.Kaydet(kullanan, belge);
                    //BelgeyiMetinDosyasinaYaz(belge);
                    //if (!donusDeger.islemSonuc)
                    //{
                    //    //BelgeyiMetinDosyasinaYaz(belge);
                    //}
                }
            }
            else
                donusDeger.sonuc.DegerAta("İşlem yapılacak veri bulunamadı.");
            return donusDeger;
        }
        private void BelgeyiMetinDosyasinaYaz(OdemeEmriMIFForm belge)
        {
            string dosyaAd = "Kapatma_" + DateTime.Now.Ticks.ToString() + ".txt";
            for (int i = 0; i < belge.detay.ObjeSayisi; i++)
            {
                OdemeEmriMIFDetay d = (OdemeEmriMIFDetay)belge.detay[i];
                OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", d.hesapKod, d.altHesapKod, d.TertipStr(), d.borc, d.alacak, d.dovizBorc, d.dovizAlacak, d.doviz), false);
            }
        }
        private ObjectArray YilSonuIslemleriIslemBelgeDetaylariniOlustur(Hashtable hesapKodHT, int tur, int yil, string borcHesapKod, string alacakHesapKod)
        {
            ObjectArray yeniDetay = new ObjectArray();
            string yeniBorcHesapKod = string.Empty;
            string yeniAlacakHesapKod = string.Empty;
            string kur = string.Empty;
            string fon = string.Empty;
            string fin = string.Empty;
            string eko = string.Empty;
            string peb = string.Empty;
            string hesapKod = string.Empty;
            string tertip = string.Empty;
            string althesapKod = string.Empty;
            string detayBirim = string.Empty;
            double borcToplam = 0.0;
            double alacakToplam = 0.0;
            double toplam = 0.0;
            string muhasebeBirimiBirimKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIBIRIMKOD") + "";
            string muhasebeBirimiKurumsalKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIKURUMSALKOD") + "";
            foreach (DictionaryEntry dt in hesapKodHT)
            {
                kur = string.Empty;
                fon = string.Empty;
                fin = string.Empty;
                eko = string.Empty;
                peb = string.Empty;
                hesapKod = string.Empty;
                tertip = string.Empty;
                althesapKod = string.Empty;
                detayBirim = string.Empty;
                string anahtar = (string)dt.Key;
                double bakiye = Math.Round((double)hesapKodHT[anahtar], 2);
                if (bakiye == 0) continue;
                var anahtarListe = anahtar.Split('@');
                hesapKod = anahtarListe.Length > 0 ? anahtarListe[0] : string.Empty;
                tertip = anahtarListe.Length > 1 ? anahtarListe[1] : string.Empty;
                if (tur == 23 || tur == 24)
                {
                    althesapKod = anahtarListe.Length > 2 ? anahtarListe[2] : string.Empty;
                    detayBirim = anahtarListe.Length > 3 ? anahtarListe[3] : string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(tertip))
                {
                    var tertipListe = tertip.Split('-');
                    kur = tertipListe.Length > 0 ? tertipListe[0] : string.Empty;
                    fon = tertipListe.Length > 1 ? tertipListe[1] : string.Empty;
                    fin = tertipListe.Length > 2 ? tertipListe[2] : string.Empty;
                    eko = tertipListe.Length > 3 ? tertipListe[3] : string.Empty;
                    peb = tertipListe.Length > 4 ? tertipListe[4] : string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(hesapKod))
                {
                    if (borcHesapKod == alacakHesapKod)
                    {
                        yeniBorcHesapKod = borcHesapKod + hesapKod.Substring(3);
                        OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(yil);
                        detay.hesapKod = yeniBorcHesapKod;
                        detay.kur = kur;
                        detay.fon = fon;
                        detay.fin = fin;
                        detay.eko = eko;
                        detay.peb = peb;
                        detay.altHesapKod = althesapKod;
                        detay.detayBirim = detayBirim;
                        if (bakiye > 0)
                            detay.alacak = bakiye;
                        else
                            detay.borc = 0 - bakiye;
                        yeniDetay.objeler.Add(detay);
                    }
                    else if (tur == 16)
                    {
                        toplam += bakiye;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(borcHesapKod))
                        {
                            if (borcHesapKod.IndexOf(".") > -1)
                                borcToplam += bakiye;
                            else
                            {
                                if (tur == 24)
                                    yeniBorcHesapKod = borcHesapKod;
                                else
                                    yeniBorcHesapKod = borcHesapKod + hesapKod.Substring(3);
                                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(yil);
                                detay.hesapKod = yeniBorcHesapKod;
                                detay.kur = kur;
                                detay.fon = fon;
                                detay.fin = fin;
                                detay.eko = eko;
                                detay.peb = peb;
                                detay.borc = bakiye;
                                detay.altHesapKod = althesapKod;
                                detay.detayBirim = detayBirim;
                                yeniDetay.objeler.Add(detay);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(alacakHesapKod))
                        {
                            if (alacakHesapKod.IndexOf(".") > -1)
                                alacakToplam += bakiye;
                            else
                            {
                                if (tur == 24)
                                {
                                    yeniAlacakHesapKod = alacakHesapKod;
                                    if (alacakHesapKod.Replace(".", "") == "6000301029999")
                                    {
                                        detayBirim = muhasebeBirimiBirimKod;
                                        kur = muhasebeBirimiKurumsalKod;
                                    }
                                }
                                else
                                    yeniAlacakHesapKod = alacakHesapKod + hesapKod.Substring(3);
                                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(yil);
                                detay.hesapKod = yeniAlacakHesapKod;
                                detay.kur = kur;
                                detay.fon = fon;
                                detay.fin = fin;
                                detay.eko = eko;
                                detay.peb = peb;
                                detay.alacak = bakiye;
                                detay.altHesapKod = althesapKod;
                                detay.detayBirim = detayBirim;
                                yeniDetay.objeler.Add(detay);
                            }
                        }
                    }
                }
            }
            if (borcToplam > 0.0)
            {
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(yil);
                detay.hesapKod = borcHesapKod.Replace(".", "");
                detay.borc = Math.Abs(borcToplam);
                yeniDetay.objeler.Add(detay);
            }
            if (alacakToplam > 0.0)
            {
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(yil);
                detay.hesapKod = alacakHesapKod.Replace(".", "");
                detay.alacak = Math.Abs(alacakToplam);
                yeniDetay.objeler.Add(detay);
            }
            if (tur == 16 && toplam != 0.0)
            {
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(yil);
                OdemeEmriMIFDetay detay1 = new OdemeEmriMIFDetay(yil);
                if (toplam > 0.0)
                {
                    detay.hesapKod = borcHesapKod.Replace(".", "");
                    detay.borc = Math.Abs(toplam);
                    detay1.hesapKod = "690";
                    detay1.alacak = Math.Abs(toplam);
                }
                else
                {
                    detay.hesapKod = alacakHesapKod.Replace(".", "");
                    detay.alacak = Math.Abs(toplam);
                    detay1.hesapKod = "690";
                    detay1.borc = Math.Abs(toplam);
                }
                yeniDetay.objeler.Add(detay);
                yeniDetay.objeler.Add(detay1);
            }
            yeniDetay.objeler.Sort(new Sirala());
            return yeniDetay;
        }
        private Hashtable YilSonuIslemleriIslemYapilacakBelgeleriGrupla(ObjectArray belgeler, int tur, int ekoKirilim, int borc0Alacak1H2 = 2)
        {
            Hashtable hesapKodHT = new Hashtable();
            string hesapKod = string.Empty;
            string tertip = string.Empty;
            string eko = string.Empty;
            int yil;
            int eko2Uz = 0;
            int eko4Uz = 0;
            string kurum = TNS.DEG.Arac.VarsayilanKurumBul();
            if (belgeler.objeler.Count > 0)
            {
                OdemeEmriMIFForm belgeYilBul = (OdemeEmriMIFForm)belgeler.objeler[0];
                Tertip tUzBul = new Tertip(belgeYilBul.yil, ENUMTabloKod.TABPEBOdenek);
                eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);
                eko4Uz = tUzBul.KodUzunlukBul(3, 4, true);
            }
            foreach (OdemeEmriMIFForm belge in belgeler.objeler)
            {
                foreach (OdemeEmriMIFDetay belgeDetay in belge.detay.objeler)
                {
                    if (borc0Alacak1H2 == 0 && belgeDetay.borc == 0 || borc0Alacak1H2 == 1 && belgeDetay.alacak == 0) continue;
                    try
                    {
                        hesapKod = string.Empty;
                        tertip = string.Empty;
                        eko = belgeDetay.eko.Replace(".", "");
                        hesapKod = belgeDetay.hesapKod.Replace(".", "");
                        if (!string.IsNullOrWhiteSpace(belgeDetay.eko))
                        {
                            if (ekoKirilim == 2 && eko.Length >= eko2Uz)
                                eko = eko.Substring(0, eko2Uz);
                            else if (ekoKirilim == 4)
                            {
                                if (eko.Length >= eko4Uz)
                                    eko = eko.Substring(0, eko4Uz);
                                if (!string.IsNullOrWhiteSpace(belgeDetay.peb))
                                {
                                    eko = eko + "-" + belgeDetay.peb.Replace(".", "");
                                }
                            }
                        }
                        else
                            eko = string.Empty;
                        //peb kod olmadan yapılınca Cumhur prg tertibini göremiyor
                        tertip = string.Format("{0}-{1}-{2}-{3}-{4}", belgeDetay.kur.Replace(".", ""), belgeDetay.fon.Replace(".", ""), belgeDetay.fin.Replace(".", ""), eko, belgeDetay.peb.Replace(".", ""));
                        //tertip = string.Format("{0}-{1}-{2}-{3}", belgeDetay.kur.Replace(".", ""), belgeDetay.fon.Replace(".", ""), belgeDetay.fin.Replace(".", ""), eko);
                        if (tur == 14 && hesapKod.StartsWith("630"))
                        {
                            int kod = 0;
                            if (hesapKod.Length > 5)
                                kod = OrtakFonksiyonlar.ConvertToInt(hesapKod.Substring(3, 2), 0);
                            if (kod > 10 && kod != 30)
                                tertip = string.Empty;
                        }
                        double bakiye = 0.0;
                        if (tur == 15 || tur == 12 || tur == 11 || tur == 24)
                            bakiye = belgeDetay.alacak - belgeDetay.borc;
                        else
                            bakiye = belgeDetay.borc - belgeDetay.alacak;
                        bakiye = Math.Round(bakiye, 2);
                        string anahtar = hesapKod;
                        if (tur == 23 || tur == 24)
                        {
                            anahtar += "@" + kurum;//tertip kur kod
                            anahtar += "@" + belgeDetay.altHesapKod;
                            anahtar += "@" + "000";
                        }
                        else
                            anahtar += "@" + tertip;
                        TNS.MUH.Arac.HTDoubleEkle(hesapKodHT, anahtar, bakiye, true);
                    }
                    catch
                    {
                    }
                }
            }
            return hesapKodHT;
        }
        private ObjectArray YilSonuIslemleriIslemYapilacakBelgeleriGetir(Kullanici kullanan, MUHOdemeEmriMIFServis oemServis, OdemeEmriMIFForm form, string islemYapilacakHesapKod)
        {
            OdemeEmriMIFForm omf = new OdemeEmriMIFForm();
            OdemeEmriMIFDetay omfDetay = new OdemeEmriMIFDetay(omf.yil);
            omf.yil = form.yil;
            omf.muhasebe = form.muhasebe;
            if (form.birim != "000")
                omf.birim = form.birim;
            omf.detay = new ObjectArray();
            omf.formDurum = 6;//Yevmiyeli
            omf.sorguIslemTarih = "01.01." + form.yil + ";31.12." + form.yil;
            omfDetay.hesapKod = islemYapilacakHesapKod;
            omf.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI, ENUMFISTUR.AMORTISMANKAPANISI, ENUMFISTUR.KAPANIS });
            omf.detay.objeler.Add(omfDetay);
            ObjectArray belgeler = oemServis.Listele(kullanan, omf, false, true, false);
            return belgeler;
        }
        /// <summary>
        /// Kur farkı olan hesaplar için belge üretir
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <param name="muhasebeBirimineIsle">if set to <c>true</c> [muhasebe birimine isle].</param>
        /// <returns></returns>
        public Sonuc KurFarkiBelgeUret(Kullanici kullanan, OdemeEmriMIFForm form, bool muhasebeBirimineIsle)
        {
            Sonuc donusDeger = new Sonuc();
            Hashtable odemeEmriDetayHT = new Hashtable();
            string kurFarkiBelgeNo = form.belgeNo;
            string muhasebeBirimiMuhasebeKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIMUHASEBEKOD") + "";
            string muhasebeBirimiBirimKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIBIRIMKOD") + "";
            string muhasebeBirimiKurumsalKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIKURUMSALKOD") + "";
            string muhasebeBirimiOrgBirimKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIORGKOD") + "";
            if (muhasebeBirimineIsle)
            {
                string hata = "";
                if (muhasebeBirimiMuhasebeKod == "")
                    hata += "Muhasebe birimi muhasebe bilgisi alanı konfigürasyon dosyasına yazılmamış.";
                if (muhasebeBirimiBirimKod == "")
                    hata += "Muhasebe birimi muhasebe birim bilgisi alanı konfigürasyon dosyasına yazılmamış.";
                if (muhasebeBirimiKurumsalKod == "")
                    hata += "Muhasebe birimi kurumsal kod bilgisi alanı konfigürasyon dosyasına yazılmamış.";
                //Organizasyon birim kod bulma işlemi yapılacak
                if (hata != "")
                {
                    donusDeger.islemSonuc = false;
                    donusDeger.hataStr = hata;
                    return donusDeger;
                }
            }
            MUHOdemeEmriMIFServis oemServis = new MUHOdemeEmriMIFServis();
            //Verilen tarihteki döviz kurlarını listele
            //********************************************************
            TNS.HRC.MerkezBankasiKurSinif mbKur = new TNS.HRC.MerkezBankasiKurSinif();
            mbKur.kurTarih = form.islemTarih;
            TNS.BKD.ParaBirimi pb = new TNS.BKD.ParaBirimi();
            ObjectArray pl = servisHRC.ParaBirimiListele(new TNS.KYM.Kullanici(), pb);
            List<string> para = new List<string>();
            foreach (TNS.BKD.ParaBirimi p in pl.objeler)
            {
                para.Add(p.kisaltma);
            }
            string[] kurlar = para.ToArray();
            ObjectArray kurListe = servisHRC.MerkezBankKurDegerleri(mbKur, kurlar);
            //********************************************************
            //Kur ile çalışan hesap kodlarını çek
            //********************************************************
            MUHHesapPlaniServis hServis = new MUHHesapPlaniServis();
            HesapPlaniSatir hs = new HesapPlaniSatir();
            hs.yil = form.yil;
            hs.dovizKurKod = "HEPSİ";
            hs.detay = true;
            //hs.hesapKod = "120.03.01.01.99.03.03";
            ObjectArray hVan = hServis.Listele(kullanan, hs);
            //Kur Farkı yapılacak satırlar
            OdemeEmriMIFForm kriter = new OdemeEmriMIFForm();
            kriter.yil = form.yil;
            kriter.islemTarih = form.islemTarih;//Önceden yevmiye tarihi idi. Şimdi onaylı ve yevmiyeliler aldığımız için işlemtarih oldu Melih 20.01.2021
            ObjectArray kurFarkiListe = KurFarkiListeVer(kullanan, kriter);
            //************************************
            string kurFarkiHesabiHata = string.Empty;
            foreach (HesapPlaniSatir h in hVan.objeler)
            {
                if (h.kurFarkiAlacakHesabi == "999" || h.kurFarkiBorcHesabi == "999")// Bazı hesaplar için kur farkı yapılmaması için
                    continue;
                Hashtable bakiyeDovizHT = new Hashtable();
                Hashtable bakiyeTLHT = new Hashtable();
                //İşlem yapılacak hesap kodunun parabirimindeki kuru seç
                //********************************************************
                decimal kur = 0;
                foreach (TNS.HRC.KurDegerleri kurDegeri in kurListe.objeler)
                {
                    if (kurDegeri.paraBirimi.kisaltma == h.dovizKurKod)
                    {
                        if (h.dovizKurTurAlis == "ALIS")
                            kur = kurDegeri.alis;
                        else if (h.dovizKurTurAlis == "SATIS")
                            kur = kurDegeri.satis;
                        else if (h.dovizKurTurAlis == "EALIS")
                            kur = kurDegeri.efektifAlis;
                        else if (h.dovizKurTurAlis == "ESATIS")
                            kur = kurDegeri.efektifSatis;
                        else if (h.dovizKurTurAlis == "SALIS")
                            kur = kurDegeri.serbestAlis;
                        else if (h.dovizKurTurAlis == "SSATIS")
                            kur = kurDegeri.serbestSatis;
                    }
                }
                //********************************************************
                bool genelBirim = false;
                if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.SAYMANLIKBAZDABAKIYE))
                    genelBirim = true;
                //bu kodlara ait belgeleri listele
                //********************************************************
                //OdemeEmriMIFForm omf = new OdemeEmriMIFForm();
                //OdemeEmriMIFDetay omfDetay = new OdemeEmriMIFDetay(omf.yil);
                //omf.yil = form.yil;
                //omf.detay = new ObjectArray();
                //omf.formDurum = 6;//Yevmiyeli
                //omf.sorguYEVYevTarih = ";" + form.islemTarih.ToString();
                //omfDetay.hesapKod = h.hesapKod;
                //omf.detay.objeler.Add(omfDetay);
                //ObjectArray belgeler = oemServis.Listele(kullanan, omf, false, true, false);
                //foreach (OdemeEmriMIFForm belge in belgeler.objeler)
                //{
                bool detayBirimVarmi = TNS.MUH.Arac.DetayBirimVarMi();
                foreach (OdemeEmriMIFDetay belgeDetay in kurFarkiListe.objeler)
                {
                    if (belgeDetay.hesapKod.Replace(".", "") != h.hesapKod.Replace(".", "")) continue;
                    double bakiye = belgeDetay.borc - belgeDetay.alacak;
                    double dovizBakiye = (belgeDetay.dovizBorc - belgeDetay.dovizAlacak);
                    string detayBirim = belgeDetay.detayBirim;
                    if (!detayBirimVarmi) detayBirim = muhasebeBirimiBirimKod;
                    string birimAnahtar = "";
                    //if (muhasebeBirimineIsle)
                    birimAnahtar = muhasebeBirimiMuhasebeKod + "-" + (genelBirim ? "000" : muhasebeBirimiBirimKod) + "-" + muhasebeBirimiKurumsalKod.Replace(".", "") + "-" + muhasebeBirimiOrgBirimKod;
                    birimAnahtar += "-" + belgeDetay.altHesapKod + "-" + (genelBirim ? "000" : detayBirim) + "-" + belgeDetay.TertipStr().Replace("-", "*");
                    TNS.MUH.Arac.HTDoubleEkle(bakiyeDovizHT, birimAnahtar, dovizBakiye, true);
                    TNS.MUH.Arac.HTDoubleEkle(bakiyeTLHT, birimAnahtar, bakiye, true);
                }
                //}
                foreach (DictionaryEntry dt in bakiyeDovizHT)
                {
                    string anahtar = (string)dt.Key;
                    double dovizBakiye = (double)bakiyeDovizHT[anahtar];
                    double bakiye = (double)bakiyeTLHT[anahtar];
                    bakiye = Math.Round(bakiye, 2);
                    dovizBakiye = Math.Round(dovizBakiye, 2);
                    string[] alanlar = anahtar.Split('-');
                    string muhasebe = alanlar[0];
                    string birim = alanlar[1];
                    string kurKod = alanlar[2];
                    string organizasyonKodu = alanlar[3];
                    string altHesap = alanlar[4];
                    string detayBirim = alanlar[5];
                    string tertip = alanlar[6].Replace("*", "-");
                    string yeniAnahtar = muhasebe + "-" + birim + "-" + kurKod + "-" + organizasyonKodu;
                    dovizBakiye = Math.Round(dovizBakiye * Convert.ToDouble(kur), 2);
                    bakiye = Math.Round(bakiye, 2);
                    ObjectArray kurFarkiDetay = (ObjectArray)odemeEmriDetayHT[yeniAnahtar];
                    if (kurFarkiDetay == null)
                        kurFarkiDetay = new ObjectArray();
                    //Eğer kur farkı var ise 
                    double kurFarki = dovizBakiye - bakiye;
                    kurFarki = Math.Round(kurFarki, 2);
                    if (kurFarki != 0)
                    {
                        //ilgili hesaba yazarak belge üret
                        OdemeEmriMIFDetay kfDetay = new OdemeEmriMIFDetay(form.yil);
                        kfDetay.hesapKod = h.hesapKod;
                        kfDetay.satirAciklama = form.aciklama;
                        kfDetay.detayBirim = detayBirim;
                        kfDetay.TertipAta(tertip);
                        kfDetay.altHesapKod = altHesap;
                        kfDetay.doviz = h.dovizKurKod;
                        if (kurFarki > 0)
                            kfDetay.borc = Math.Abs(kurFarki);
                        else
                            kfDetay.alacak = Math.Abs(kurFarki);
                        kurFarkiDetay.objeler.Add(kfDetay);
                        kfDetay = new OdemeEmriMIFDetay(form.yil);
                        kfDetay.satirAciklama = form.aciklama;
                        kfDetay.detayBirim = detayBirim;
                        kfDetay.TertipAta(tertip);
                        kfDetay.altHesapKod = altHesap;
                        if (kurFarki > 0)
                        {
                            kfDetay.hesapKod = h.kurFarkiAlacakHesabi;
                            kfDetay.alacak = Math.Abs(kurFarki);
                            if (string.IsNullOrEmpty(h.kurFarkiAlacakHesabi))
                                kurFarkiHesabiHata += h.hesapKod + " nolu hesap kodu için kur farkı alacak hesabı bilgisi eksik!<br>";
                        }
                        else
                        {
                            kfDetay.hesapKod = h.kurFarkiBorcHesabi;
                            kfDetay.borc = Math.Abs(kurFarki);
                            if (string.IsNullOrEmpty(h.kurFarkiBorcHesabi))
                                kurFarkiHesabiHata += h.hesapKod + " nolu hesap kodu için kur farkı borç hesabı bilgisi eksik!<br>";
                        }
                        kurFarkiDetay.objeler.Add(kfDetay);
                    }
                    odemeEmriDetayHT[yeniAnahtar] = kurFarkiDetay;
                }
            }
            if (!string.IsNullOrEmpty(kurFarkiHesabiHata))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = kurFarkiHesabiHata;
                return donusDeger;
            }

            bool belgeYaratildi = false;
            foreach (DictionaryEntry dt in odemeEmriDetayHT)
            {
                string anahtar = (string)dt.Key;
                string[] alanlar = anahtar.Split('-');
                string muhasebe = alanlar[0];
                string birim = alanlar[1];
                string kurKod = alanlar[2];
                string organizasyonKodu = alanlar[3];
                ObjectArray kurFarkiDetay = (ObjectArray)odemeEmriDetayHT[anahtar];
                if (kurFarkiDetay == null)
                    kurFarkiDetay = new ObjectArray();
                //Kurfarkı yapılacaklarlar bulundu ve detay sınıfına konuldu. Belge üretilecek
                if (kurFarkiDetay.objeler.Count > 0)
                {
                    OdemeEmriMIFForm kurFarkiBelge = new OdemeEmriMIFForm();
                    kurFarkiBelge.yil = form.yil;
                    kurFarkiBelge.islemTarih = form.islemTarih;
                    kurFarkiBelge.belgeNo = kurFarkiBelgeNo;
                    kurFarkiBelge.muhasebe = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
                    kurFarkiBelge.birim = birim;
                    kurFarkiBelge.kurKod = kurKod;
                    kurFarkiBelge.organizasyonKodu = OrtakFonksiyonlar.ConvertToInt(organizasyonKodu, 0);
                    kurFarkiBelge.aciklama = form.aciklama;
                    kurFarkiBelge.detay = kurFarkiDetay;
                    // Belgenin kurfarkı belgesi olup olmadığını anlamamız için bunu eklemek zorunda kaldım.
                    // İleride etki edeceği alanlar kontrol edilip fistur alanına yazılabilir.
                    kurFarkiBelge.projeNo = "KURFARKI";
                    kurFarkiBelge.belgeTur = ENUMBelgeTuru.MIF;
                    donusDeger = oemServis.Kaydet(kullanan, kurFarkiBelge);
                    belgeYaratildi = true;
                }
            }
            if (!belgeYaratildi)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = "Kur farkı yapılacak veri bulunamadı";
            }
            return donusDeger;
        }
        public ObjectArray KurFarkiListeVer(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            IDataReader dr = oe.KurFarkiListe(kriter.yil, kriter.islemTarih);
            if (dr != null)
            {
                while (dr.Read())
                {
                    OdemeEmriMIFDetay f = new OdemeEmriMIFDetay(kriter.yil);
                    f.detayBirim = VeriErisim.DegerAl(dr, 0, "");
                    f.kur = VeriErisim.DegerAl(dr, 1, "");
                    f.fon = VeriErisim.DegerAl(dr, 2, "");
                    f.fin = VeriErisim.DegerAl(dr, 3, "");
                    f.eko = VeriErisim.DegerAl(dr, 4, "");
                    f.peb = VeriErisim.DegerAl(dr, 5, "");
                    f.hesapKod = VeriErisim.DegerAl(dr, 6, "");
                    f.altHesapKod = VeriErisim.DegerAl(dr, 7, "");
                    f.doviz = VeriErisim.DegerAl(dr, 8, "");
                    f.borc = VeriErisim.DegerAl(dr, 9, 0.0);
                    f.alacak = VeriErisim.DegerAl(dr, 10, 0.0);
                    f.dovizBorc = VeriErisim.DegerAl(dr, 11, 0.0);
                    f.dovizAlacak = VeriErisim.DegerAl(dr, 12, 0.0);
                    liste.objeler.Add(f);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }
        public ObjectArray YillikAylikDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            string hesapKod = "";
            if (kriter.detay.objeler.Count > 0)
            {
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(0);
                detay = (OdemeEmriMIFDetay)kriter.detay.objeler[0];
                hesapKod = detay.hesapKod.Replace(".", "");
            }
            string maske = MUHMizanBilanco.HesapPlaniYapiBul(kriter.surecYil);//seviye
            int maskeUzunluk = MUHMizanBilanco.MaskeUzunlukBul(maske);
            IDataReader r = to.YillikAylikDagilimHesapKod(kriter, hesapKod, maskeUzunluk);
            Tertip th = new Tertip(kriter.yil, ENUMTabloKod.TABOdemeEmri);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AylikDagilim aylik = new AylikDagilim();
                    aylik.yil = VeriErisim.DegerAl(r, 0, 0);
                    aylik.ay = VeriErisim.DegerAl(r, 1, 0);
                    aylik.hesapKod = VeriErisim.DegerAl(r, 2, "");
                    aylik.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), aylik.hesapKod);
                    aylik.hesapAd = VeriErisim.DegerAl(r, 3, "");
                    aylik.tutar = VeriErisim.DegerAl(r, 4, 0.0);
                    liste.objeler.Add(aylik);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }
        public ObjectArray YillikAylikTarihselDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            string hesapKod = "";
            if (kriter.detay.objeler.Count > 0)
            {
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(0);
                detay = (OdemeEmriMIFDetay)kriter.detay.objeler[0];
                hesapKod = detay.hesapKod.Replace(".", "");
            }
            string maske = MUHMizanBilanco.HesapPlaniYapiBul(kriter.surecYil);//seviye
            int maskeUzunluk = MUHMizanBilanco.MaskeUzunlukBul(maske);
            IDataReader r = to.YillikAylikTarihselDagilimHesapKod(kriter, hesapKod, maskeUzunluk);
            Tertip th = new Tertip(kriter.yil, ENUMTabloKod.TABOdemeEmri);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AylikDagilim aylik = new AylikDagilim();
                    aylik.yil = VeriErisim.DegerAl(r, 0, 0);
                    aylik.hesapKod = VeriErisim.DegerAl(r, 1, "");
                    aylik.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), aylik.hesapKod);
                    aylik.hesapAd = VeriErisim.DegerAl(r, 2, "");
                    aylik.tutar = VeriErisim.DegerAl(r, 3, 0.0);
                    liste.objeler.Add(aylik);
                }
                r.Close();
            }
            //eski yıl
            if (kriter.sorguIslemTarih != "")
            {
                string[] t = kriter.sorguIslemTarih.Split(';');
                TNSDateTime d1 = new TNSDateTime(t[0]);
                TNSDateTime d2 = new TNSDateTime(t[1]);
                d1.AddYears(-1);
                d2.AddYears(-1);
                kriter.sorguIslemTarih = d1.ToString() + ";" + d2.ToString();
                kriter.yil = d1.Yil;
            }
            else if (kriter.sorguYEVYevTarih != "")
            {
                string[] t = kriter.sorguYEVYevTarih.Split(';');
                TNSDateTime d1 = new TNSDateTime(t[0]);
                TNSDateTime d2 = new TNSDateTime(t[1]);
                d1.AddYears(-1);
                d2.AddYears(-1);
                kriter.sorguYEVYevTarih = d1.ToString() + ";" + d2.ToString();
                kriter.yil = d1.Yil;
            }
            r = to.YillikAylikTarihselDagilimHesapKod(kriter, hesapKod, maskeUzunluk);
            th = new Tertip(kriter.yil, ENUMTabloKod.TABOdemeEmri);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AylikDagilim aylik = new AylikDagilim();
                    aylik.yil = VeriErisim.DegerAl(r, 0, 0);
                    aylik.hesapKod = VeriErisim.DegerAl(r, 1, "");
                    aylik.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), aylik.hesapKod);
                    aylik.hesapAd = VeriErisim.DegerAl(r, 2, "");
                    aylik.tutar = VeriErisim.DegerAl(r, 3, 0.0);
                    liste.objeler.Add(aylik);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }
        public ObjectArray AylikDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            string hesapKod = "";
            if (kriter.detay.objeler.Count > 0)
            {
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(0);
                detay = (OdemeEmriMIFDetay)kriter.detay.objeler[0];
                hesapKod = detay.hesapKod.Replace(".", "");
            }
            string maske = MUHMizanBilanco.HesapPlaniYapiBul(kriter.surecYil);//seviye
            int maskeUzunluk = MUHMizanBilanco.MaskeUzunlukBul(maske);
            IDataReader r = to.AylikDagilimHesapKod(kriter, hesapKod, maskeUzunluk);
            Tertip th = new Tertip(kriter.yil, ENUMTabloKod.TABOdemeEmri);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AylikDagilim aylik = new AylikDagilim();
                    aylik.ay = VeriErisim.DegerAl(r, 0, 0);
                    aylik.hesapKod = VeriErisim.DegerAl(r, 1, "");
                    aylik.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), aylik.hesapKod);
                    aylik.hesapAd = VeriErisim.DegerAl(r, 2, "");
                    aylik.tutar = VeriErisim.DegerAl(r, 3, 0.0);
                    liste.objeler.Add(aylik);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }
        public ObjectArray YillikDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            string hesapKod = "";
            if (kriter.detay.objeler.Count > 0)
            {
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(0);
                detay = (OdemeEmriMIFDetay)kriter.detay.objeler[0];
                hesapKod = detay.hesapKod.Replace(".", "");
            }
            string maske = MUHMizanBilanco.HesapPlaniYapiBul(kriter.surecYil);//seviye
            int maskeUzunluk = MUHMizanBilanco.MaskeUzunlukBul(maske);
            IDataReader r = to.YillikDagilimHesapKod(kriter, hesapKod, maskeUzunluk);
            Tertip th = new Tertip(kriter.yil, ENUMTabloKod.TABOdemeEmri);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AylikDagilim aylik = new AylikDagilim();
                    aylik.ay = VeriErisim.DegerAl(r, 0, 0);
                    aylik.hesapKod = VeriErisim.DegerAl(r, 1, "");
                    aylik.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), aylik.hesapKod);
                    aylik.hesapAd = VeriErisim.DegerAl(r, 2, "");
                    aylik.tutar = VeriErisim.DegerAl(r, 3, 0.0);
                    liste.objeler.Add(aylik);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }
        public ObjectArray BirimDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            string hesapKod = "";
            if (kriter.detay.objeler.Count > 0)
            {
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(0);
                detay = (OdemeEmriMIFDetay)kriter.detay.objeler[0];
                hesapKod = detay.hesapKod.Replace(".", "");
            }
            string maske = MUHMizanBilanco.HesapPlaniYapiBul(kriter.surecYil);//seviye
            int maskeUzunluk = MUHMizanBilanco.MaskeUzunlukBul(maske);
            IDataReader r = to.BirimDagilimHesapKod(kriter, hesapKod, maskeUzunluk);
            Tertip th = new Tertip(kriter.yil, ENUMTabloKod.TABOdemeEmri);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AylikDagilim aylik = new AylikDagilim();
                    aylik.birim = VeriErisim.DegerAl(r, 0, "");
                    aylik.hesapKod = VeriErisim.DegerAl(r, 1, "");
                    aylik.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), aylik.hesapKod);
                    aylik.hesapAd = VeriErisim.DegerAl(r, 2, "");
                    aylik.tutar = VeriErisim.DegerAl(r, 3, 0.0);
                    liste.objeler.Add(aylik);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }
        public ObjectArray TasinirRaporu(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            string hesapKod = "";
            if (kriter.detay.objeler.Count > 0)
            {
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(0);
                detay = (OdemeEmriMIFDetay)kriter.detay.objeler[0];
                hesapKod = detay.hesapKod.Replace(".", "");
            }
            Tertip th = new Tertip(kriter.yil, ENUMTabloKod.TABTASHesapPlani);
            IDataReader r = to.TasinirRaporu(kriter.yil, kriter.muhasebe, kriter.birim, hesapKod, kriter.formDurum);

            if (r != null)
            {
                while (r.Read())
                {
                    TasinirRaporuListe rapor = new TasinirRaporuListe();
                    rapor.yil = VeriErisim.DegerAl(r, 0, 0);
                    rapor.muhasebeKod = VeriErisim.DegerAl(r, 1, "");
                    rapor.harcamaBirimKod = VeriErisim.DegerAl(r, 2, "");
                    rapor.fisNo = VeriErisim.DegerAl(r, 3, "");
                    rapor.fisTarih = VeriErisim.DegerAl(r, 4);
                    rapor.ambarKod = VeriErisim.DegerAl(r, 5, "");
                    rapor.islemTipi = VeriErisim.DegerAl(r, 6, 0);
                    rapor.ilgili = VeriErisim.DegerAl(r, 7, "");
                    rapor.islemTarih = VeriErisim.DegerAl(r, 8);
                    rapor.hesapKod = VeriErisim.DegerAl(r, 9, "");
                    rapor.adet = VeriErisim.DegerAl(r, 10, 0.0);
                    rapor.tutar = VeriErisim.DegerAl(r, 12, 0.0);
                    rapor.hesapAdi = VeriErisim.DegerAl(r, 14, "");
                    rapor.belgeNo = VeriErisim.DegerAl(r, 15, "");
                    liste.objeler.Add(rapor);
                }
                r.Close();
            }
            else
                liste.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return liste;
        }
        //private static double AvansMiktariGetir(Kullanici kullanan, int yil, string surecNo, int muhasebe, string birim, string tertip)
        //{
        //    TNS.HRC.OdemeEmriMIFForm kriter = new TNS.HRC.OdemeEmriMIFForm();
        //    kriter.yil = yil;
        //    kriter.muhasebe = muhasebe;
        //    kriter.birim = birim;
        //    kriter.surecNo = surecNo;
        //    kriter.belgeTur = (int)ENUMOdemeBelgeTur.MIF;
        //    ObjectArray avansBilgi = new ObjectArray();
        //    IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
        //    avansBilgi = servisHRC.OdemeEmriFormListele(kullanan, kriter);
        //    double avans = 0.0;
        //    if (avansBilgi.ObjeSayisi > 0)
        //    {
        //        foreach (TNS.HRC.OdemeEmriMIFForm ff in avansBilgi.objeler)
        //        {
        //            foreach (TNS.HRC.OdemeEmriMIFDetay odt in ff.detay.objeler)
        //            {
        //                if (odt.hesapPlanKod.StartsWith("16") && tertip.Replace(".", "").StartsWith(odt.tertip.Replace(".", "")))
        //                    avans += odt.borc;
        //            }
        //        }
        //    }
        //    return avans;
        //}
        /// <summary>
        /// Butces the odenek kapat.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="belgeTarih">The belge tarih.</param>
        /// <returns></returns>
        public static ObjectArray ButceOdenekKapat(Kullanici kullanan, int yil, int muhasebe, string birim, string belgeNo, TNSDateTime belgeTarih)
        {
            ObjectArray sonuclar = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader r = to.ButceOdenekKapat(yil, muhasebe, birim, belgeNo, belgeTarih);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ButceOdenekBilgi bilgi = new ButceOdenekBilgi();
                    bilgi.muhasebeKod = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.birimKod = VeriErisim.DegerAl(r, 1, "");
                    bilgi.hesapKod = VeriErisim.DegerAl(r, 2, "");
                    bilgi.kurKod = VeriErisim.DegerAl(r, 3, "");
                    bilgi.fonKod = VeriErisim.DegerAl(r, 4, "");
                    bilgi.finKod = VeriErisim.DegerAl(r, 5, "");
                    bilgi.ekoKod = VeriErisim.DegerAl(r, 6, "");
                    bilgi.pebKod = VeriErisim.DegerAl(r, 7, "");
                    bilgi.odenek = VeriErisim.DegerAl(r, 8, 0.0);
                    sonuclar.objeler.Add(bilgi);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return sonuclar;
        }
        internal OdemeEmriMIFDetay SatirOku(VeriErisim ve, int mifYil, string mifNo, int satirNo)
        {
            OdemeEmriMIFDetay sd = null;
            ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(new Kullanici()));
            TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
            IDataReader r = tab.TekBelgeOkuDetay(mifYil, mifNo, "", satirNo);
            if (r != null)
            {
                if (r.Read())
                {
                    Tertip th = new Tertip(mifYil, ENUMTabloKod.TABOdemeEmri);
                    Tertip tt = new Tertip(mifYil, ENUMTabloKod.TABOdenek);
                    sd = DataReaderdanDetayOl(r, mifYil, th, tt);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return sd;
        }
        internal static YuzdeOnBilgi YuzdeOnBilgiBul(VeriErisim veriErisim, Kullanici kullanan, int yil, ENUMYuzdeOnTur MalHizmetYapim, int saymanlik, string birim, string haricOdemeEmriNo)
        {
            YuzdeOnBilgi donDeger = new YuzdeOnBilgi();
            donDeger.tur = MalHizmetYapim;
            UstSinirBul(yil, donDeger);
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF mif = new TABOdemeEmriMIF(ve);
            if (!donDeger.birimSiniriMi)
            {
                saymanlik = 0;
                birim = "";
            }
            string yuzdeOnAlimTur = TNS.MUH.Arac.YuzdeOnAlimTurBul(donDeger.TurAd);
            donDeger.odenen = mif.YuzdeOnOdenenBul(yil, yuzdeOnAlimTur, saymanlik, birim, haricOdemeEmriNo);
            ve.VeriErisimKapat();
            return donDeger;
        }
        private static void UstSinirBul(int yil, YuzdeOnBilgi donDeger)
        {
            string yuzdeOnKod = donDeger.TurAd;
            donDeger.birimSiniriMi = false;
            string strYuzdeOnSinir = TNS.MUH.Arac.DegiskenDegerBul(yil, "/YuzdeOn/" + yuzdeOnKod + "Sinir");
            donDeger.ustSinir = OrtakFonksiyonlar.ConvertToDouble(strYuzdeOnSinir);
        }
        internal static OdemeEmriMIFForm[] SurecBelgeNoListele(VeriErisim veriErisim, Kullanici kullanan, int surecYil, string surecNo, int mifYil, string mifBelgeNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF mif = new TABOdemeEmriMIF(ve);
            List<OdemeEmriMIFForm> l = new List<OdemeEmriMIFForm>();
            IDataReader dr = mif.SurecBelgeNoListele(surecYil, surecNo);
            if (dr != null)
            {
                while (dr.Read())
                {
                    OdemeEmriMIFForm f = new OdemeEmriMIFForm();
                    f.yil = VeriErisim.DegerAl(dr, 0, 0);
                    f.belgeNo = VeriErisim.DegerAl(dr, 1, "");
                    if (f.yil < mifYil || f.yil == mifYil && f.belgeNo.CompareTo(mifBelgeNo) < 0)
                    {
                        f.alacakToplam = VeriErisim.DegerAl(dr, 2, (double)0);
                        l.Add(f);
                    }
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return l.ToArray();
        }
        internal static Sonuc OdemeEmriMIFSurecBagKur(VeriErisim veriErisim, Kullanici kullanan, int yil, string belgeNo, int surecYil, string surecNo, int muhasebeKod, string birimKod)
        {
            IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            SurecOzetBilgi bilgi = servisHRC.SurecOzetBilgiBul(kullanan, surecYil, surecNo);
            if (bilgi == null) return new Sonuc(false, "İstenilen süreç bulunamadı!", "");
            if (bilgi.SURECDURUM == 9) return new Sonuc(false, "Ödeme Emri/MİF İptal edilmiş Sürece bağlanılamaz!", "");
            if (bilgi.MUHASEBEKOD != muhasebeKod) return new Sonuc(false, "Süreç ile Ödeme Emri/MİF aynı saymanlığa ait değil!", "");
            if (bilgi.BIRIMKOD != "000" && birimKod != "000" && bilgi.BIRIMKOD != birimKod) return new Sonuc(false, "Süreç ile Ödeme Emri/MİF aynı birime ait değil!", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF mif = new TABOdemeEmriMIF(ve);
            string ihaleKod = "";
            if (!string.IsNullOrWhiteSpace(bilgi.MADDE_ADI))
            {
                ihaleKod = bilgi.MADDE_ADI;
                if (bilgi.HRCIS_TUR == 1) ihaleKod += "Mal";
                if (bilgi.HRCIS_TUR == 2) ihaleKod += "Hizmet";
                if (bilgi.HRCIS_TUR == 3) ihaleKod += "Yapım";
                ihaleKod = ihaleKod.Replace(".", "");
            }
            Sonuc s = mif.SurecBilgiGuncelle(yil, belgeNo, surecYil, surecNo, ihaleKod);
            if (s.islemSonuc)
            {
                s.anahtar = ihaleKod;
                servisHRC.BlokeTertipleriOtomatikGuncelle(kullanan, surecYil, surecNo, yil, ENUMKontrolDurum.KontrolEtme);
            }
            return s;
        }
        internal static Sonuc BirimHarcamaSayOE(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, int saymanlikKod, string birimKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF mif = new TABOdemeEmriMIF(ve);
            double say = mif.BirimHarcamaSayOEBul(yil, ay, saymanlikKod, birimKod);
            Sonuc s = new Sonuc(true, "", "");
            if (!string.IsNullOrEmpty(ve.hata)) s.DegerAta(ve.hata);
            else s.kayitSay = Convert.ToInt32(say);
            ve.VeriErisimKapat();
            return s;
        }
        internal static ObjectArray SGBKBSFarkListele(VeriErisim veriErisim, Kullanici kullanan, SGBKBSFark sgbKbsFark)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF mif = new TABOdemeEmriMIF(ve);
            TABOdemeEmriMIF mif2 = new TABOdemeEmriMIF(ve2);
            IDataReader dr = mif.SGBKbsFarkBul(sgbKbsFark);
            if (dr == null)
            {
                oa.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return oa;
            }
            Tertip t = new Tertip(sgbKbsFark.yil, ENUMTabloKod.TABPEBOdenek);
            while (dr.Read())
            {
                SGBKBSFark sgbKbsFarkIc = new SGBKBSFark();
                sgbKbsFarkIc.yil = sgbKbsFark.yil;
                sgbKbsFarkIc.ay = sgbKbsFark.ay;
                int kol = 0;
                sgbKbsFarkIc.SayKod = VeriErisim.DegerAl(dr, kol++, 0);
                sgbKbsFarkIc.BirimKod = VeriErisim.DegerAl(dr, kol++, "");
                sgbKbsFarkIc.kurKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), VeriErisim.DegerAl(dr, kol++, ""));
                sgbKbsFarkIc.fonKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(1), VeriErisim.DegerAl(dr, kol++, ""));
                sgbKbsFarkIc.finKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(2), VeriErisim.DegerAl(dr, kol++, ""));
                sgbKbsFarkIc.ekoKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(3), VeriErisim.DegerAl(dr, kol++, ""));
                sgbKbsFarkIc.pebKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(4), VeriErisim.DegerAl(dr, kol++, ""));
                sgbKbsFarkIc.KBSHarcama = VeriErisim.DegerAl(dr, kol++, 0.0);
                sgbKbsFarkIc.SGBHarcama = VeriErisim.DegerAl(dr, kol++, 0.0);
                double fark = sgbKbsFarkIc.KBSHarcama - sgbKbsFarkIc.SGBHarcama;
                if ((fark > 0 && sgbKbsFark.sgbKsbDurum == 1) || (fark < 0 && sgbKbsFark.sgbKsbDurum == 2) || (sgbKbsFark.sgbKsbDurum == 0))
                {
                    if (
                        (sgbKbsFark.sgbKsbDurum == 0 && Math.Abs(fark) >= sgbKbsFark.minFark)
                        || (sgbKbsFark.sgbKsbDurum != 0 && Math.Abs(fark) <= sgbKbsFark.minFark)
                        )
                    {
                        oa.objeler.Add(sgbKbsFarkIc);
                    }
                }
            }
            dr.Close();
            ve2.VeriErisimKapat();
            ve.VeriErisimKapat();
            return oa;
        }
        internal static ObjectArray OdenekUstuListele(VeriErisim veriErisim, Kullanici kullanan, SGBKBSFark sgbKbsFark)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF mif = new TABOdemeEmriMIF(ve);
            IDataReader dr = mif.OdenekUstuAltiBul(sgbKbsFark);
            if (dr == null)
            {
                oa.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return oa;
            }
            Tertip t = new Tertip(sgbKbsFark.yil, ENUMTabloKod.TABPEBOdenek);
            while (dr.Read())
            {
                SGBKBSFark sgbKbsFarkIc = new SGBKBSFark();
                sgbKbsFarkIc.yil = sgbKbsFark.yil;
                sgbKbsFarkIc.ay = sgbKbsFark.ay;
                sgbKbsFarkIc.SayKod = VeriErisim.DegerAl(dr, 0, 0);
                sgbKbsFarkIc.BirimKod = VeriErisim.DegerAl(dr, 1, "");
                sgbKbsFarkIc.kurKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), VeriErisim.DegerAl(dr, 2, ""));
                sgbKbsFarkIc.fonKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(1), VeriErisim.DegerAl(dr, 3, ""));
                sgbKbsFarkIc.finKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(2), VeriErisim.DegerAl(dr, 4, ""));
                sgbKbsFarkIc.ekoKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(3), VeriErisim.DegerAl(dr, 5, ""));
                sgbKbsFarkIc.pebKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(4), VeriErisim.DegerAl(dr, 6, ""));
                sgbKbsFarkIc.KBSHarcama = VeriErisim.DegerAl(dr, 7, 0.0);
                sgbKbsFarkIc.SGBHarcama = VeriErisim.DegerAl(dr, 8, 0.0);
                double fark = sgbKbsFarkIc.KBSHarcama - sgbKbsFarkIc.SGBHarcama;
                if ((fark > 0 && sgbKbsFark.sgbKsbDurum == 1) || (fark < 0 && sgbKbsFark.sgbKsbDurum == 2) || (sgbKbsFark.sgbKsbDurum == 0))
                {
                    if (Math.Abs(fark) >= sgbKbsFark.minFark)
                    {
                        oa.objeler.Add(sgbKbsFarkIc);
                    }
                }
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }
        private static void GonderilenHarcananBul(TABOdemeEmriMIF mif, Kullanici kullanan, SGBKBSFark sgbKbsFarkIc, Tertip t)
        {
            int eko2Uz = t.KodUzunlukBul(3, 2, true);
            IDataReader dr = mif.FaaliyetHarcamaListele(sgbKbsFarkIc.yil, sgbKbsFarkIc.ay, sgbKbsFarkIc.SayKod, sgbKbsFarkIc.BirimKod, sgbKbsFarkIc.kurKod, sgbKbsFarkIc.fonKod, sgbKbsFarkIc.finKod, sgbKbsFarkIc.ekoKod.Replace(".", "").Substring(0, eko2Uz));
            if (dr != null)
            {
                while (dr.Read())
                {
                    string pebKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(4), VeriErisim.DegerAl(dr, 0, ""));
                    double odenek = VeriErisim.DegerAl(dr, 1, 0.0);
                    sgbKbsFarkIc.Ekle(pebKod, odenek);
                }
                dr.Close();
            }
            if (sgbKbsFarkIc.KBSHarcama > sgbKbsFarkIc.SGBHarcama)
            {
                foreach (SGBKBSFark.FaalOdenek f in sgbKbsFarkIc.faalOdenekler)
                {
                    f.odenek = 0 - f.odenek;
                }
                dr = mif.FaaliyetGonderilenListele(sgbKbsFarkIc.yil, sgbKbsFarkIc.ay, sgbKbsFarkIc.SayKod, sgbKbsFarkIc.BirimKod, sgbKbsFarkIc.kurKod, sgbKbsFarkIc.fonKod, sgbKbsFarkIc.finKod, sgbKbsFarkIc.ekoKod);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        string pebKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(4), VeriErisim.DegerAl(dr, 0, ""));
                        double odenek = VeriErisim.DegerAl(dr, 1, 0.0);
                        SGBKBSFark.FaalOdenek f = sgbKbsFarkIc.Bul(pebKod);
                        if (f == null)
                        {
                            f = sgbKbsFarkIc.Ekle(pebKod, 0);
                        }
                        f.odenek = odenek + f.odenek;
                    }
                    dr.Close();
                }
                sgbKbsFarkIc.faalOdenekler.Sort();
            }
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
                    donenDeger = "<br>" + string.Format(ButceMuhasebeResI.MUH146, kapaliTarih.ToString());
            }
            return donenDeger;
        }
        /// <summary>
        /// Odemes the emri evrak no kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="evrakNo">The evrak no.</param>
        /// <param name="odemeEmriNo">The odeme emri no.</param>
        /// <param name="durum">The durum.</param>
        /// <returns></returns>
        public Sonuc OdemeEmriEvrakNoKaydet(Kullanici kullanan, int yil, string evrakNo, string odemeEmriNo, int durum)
        {
            Sonuc sonuc = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            sonuc = oe.OdemeEmriEvrakNoKaydet(yil, evrakNo, odemeEmriNo, durum);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
        /// <summary>
        /// Yuzdes the on listele.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public ObjectArray YuzdeOnListele(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            YuzdeOnBilgi yob = new YuzdeOnBilgi();
            yob.tur = ENUMYuzdeOnTur.Mal;
            UstSinirBul(yil, yob);
            oa.Ekle(yob);
            yob = new YuzdeOnBilgi();
            yob.tur = ENUMYuzdeOnTur.Hizmet;
            UstSinirBul(yil, yob);
            oa.Ekle(yob);
            yob = new YuzdeOnBilgi();
            yob.tur = ENUMYuzdeOnTur.Yapim;
            UstSinirBul(yil, yob);
            oa.Ekle(yob);
            yob = new YuzdeOnBilgi();
            yob.tur = ENUMYuzdeOnTur.Tanimsiz;
            oa.Ekle(yob);
            string[] yuzdeOnIhaleKodlar = TNS.MUH.Arac.YuzdeOnAlimTurBul();
            foreach (YuzdeOnBilgi yobIc in oa.objeler)
            {
                try
                {
                    yobIc.liste = MalHizmetYapimListele(ve, kullanan, yil, yobIc, yuzdeOnIhaleKodlar);
                }
                catch (Exception ex)
                {
                    oa.sonuc.DegerAta(ex.Message);
                    break;
                }
            }
            ve.VeriErisimKapat();
            return oa;
        }
        /// <summary>
        /// Mals the hizmet yapim listele.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="yob">The yob.</param>
        /// <param name="yuzdeOnIhaleKodlar">The yuzde on ihale kodlar.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Yüzde on limiti içindeki ihale kodları parametre olarak geçirmelisiniz!
        /// or
        /// </exception>
        public TNS.BKD.TertipOdenekBilgi[] MalHizmetYapimListele(VeriErisim veriErisim, Kullanici kullanan, int yil, YuzdeOnBilgi yob, string[] yuzdeOnIhaleKodlar)
        {
            if (yuzdeOnIhaleKodlar == null || yuzdeOnIhaleKodlar.Length == 0)
                throw new Exception("Yüzde on limiti içindeki ihale kodları parametre olarak geçirmelisiniz!");
            string ekoKodYapi = TNS.MUH.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD4/KODYAPI");
            List<TNS.BKD.TertipOdenekBilgi> liste = new List<TertipOdenekBilgi>();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            string MalHizmetYapim = yob.TurAd;
            IDataReader dr = oe.MalHizmetYapimListele(yil, MalHizmetYapim, yob.saymanlikKodlar, yob.birimKodlar);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            TNS.BKD.TertipOdenekBilgi to = null;
            string eskiEkoKod = null;
            while (dr.Read())
            {
                string ekoKod = OrtakFonksiyonlar.KodNoktaKoy(ekoKodYapi, VeriErisim.DegerAl(dr, 0, ""));
                if (ekoKod != eskiEkoKod || to == null || eskiEkoKod == null)
                {
                    string ekoAd = VeriErisim.DegerAl(dr, 1, "");
                    to = new TertipOdenekBilgi(ekoKod + "-" + ekoAd, yuzdeOnIhaleKodlar.Length + 1);
                    liste.Add(to);
                    eskiEkoKod = ekoKod;
                }
                string ihaleKodTur = VeriErisim.DegerAl(dr, 2, "");
                string ihaleKod = "";
                if (!string.IsNullOrWhiteSpace(MalHizmetYapim))
                {
                    ihaleKod = ihaleKodTur.Replace(MalHizmetYapim, "");
                }
                else
                    ihaleKod = ihaleKodTur;
                int yer = TNS.MUH.Arac.YuzdeOnIhaleKodYerBul(yuzdeOnIhaleKodlar, ihaleKod);
                to.odenekler[yer] += VeriErisim.DegerAl(dr, 3, (double)0);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return liste.ToArray();
        }

        /*
         public Sonuc KapanisBilgisiKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            foreach (KapanisBilgileri kapatma in liste.objeler)
            {
                sonuc = to.KapanisBilgisiKaydet(kapatma);
                if (!sonuc.islemSonuc)
                    break;
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
         */
        /// <summary>
        /// Mahsups the edilecek belge listele.
        /// </summary>
        /// <param name="oeForm">The oe form.</param>
        /// <param name="mahsupAlaniBosOlanlar">if set to <c>true</c> [mahsup alani bos olanlar].</param>
        /// <returns></returns>
        public ObjectArray MahsupEdilecekBelgeListele(OdemeEmriMIFForm oeForm, bool mahsupAlaniBosOlanlar)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            IDataReader dr = oe.MahsupEdilecekBelgeListele(oeForm, mahsupAlaniBosOlanlar);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = ve.hata;
                return liste;
            }
            while (dr.Read())
            {
                OdemeEmriMIFForm f = new OdemeEmriMIFForm();
                f.yil = VeriErisim.DegerAl(dr, 0, 0);
                f.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                f.birim = VeriErisim.DegerAl(dr, 2, "");
                f.ilgiliAd = VeriErisim.DegerAl(dr, 3, "");
                f.ilgiliNo = VeriErisim.DegerAl(dr, 11, "");
                f.belgeNo = VeriErisim.DegerAl(dr, 4, "");
                f.islemTarih = VeriErisim.DegerAl(dr, 5);
                f.YEVYevNo = VeriErisim.DegerAl(dr, 6, 0);
                f.YEVYevTarih = VeriErisim.DegerAl(dr, 7);
                f.kurum = VeriErisim.DegerAl(dr, 8, "");
                f.borcToplam = VeriErisim.DegerAl(dr, 9, 0.0);
                f.alacakToplam = VeriErisim.DegerAl(dr, 10, 0.0);
                f.AVKTutar = VeriErisim.DegerAl(dr, 14, 0.0);//Kapatma tutarı
                f.AVKYevNo = VeriErisim.DegerAl(dr, 15, "");//DOVIZ
                f.AVKYevNo += "*" + VeriErisim.DegerAl(dr, 16, "");//DOVIZKURTUR
                f.AVKYevNo += "*" + VeriErisim.DegerAl(dr, 17, 0.0); //DOVIZBORC
                f.AVKYevNo += "*" + VeriErisim.DegerAl(dr, 18, 0.0);//DOVIZALACAK
                f.AVKYevNo += "*" + VeriErisim.DegerAl(dr, 19, 0.0);//DOVIZKUR
                f.aciklama += VeriErisim.DegerAl(dr, 20, "");//SATIRACIKLAMA
                f.ilgiliAd = VeriErisim.DegerAl(dr, 12, "");//Firma
                if (f.ilgiliAd == "") f.ilgiliAd = VeriErisim.DegerAl(dr, 13, "");//Personel
                liste.objeler.Add(f);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return liste;
        }
        /// <summary>
        /// KBSs the aktarim listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public ObjectArray KBSAktarimListele(Kullanici kullanan, int yil)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            IDataReader dr = oe.KBSAktarimListele(yil);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                return liste;
            }
            while (dr.Read())
            {
                KBSAktarim f = new KBSAktarim();
                f.yil = VeriErisim.DegerAl(dr, 0, 0);
                f.hesapKod = VeriErisim.DegerAl(dr, 1, "");
                f.hesapAd = VeriErisim.DegerAl(dr, 2, "");
                f.kod = VeriErisim.DegerAl(dr, 3, "");
                f.goster = VeriErisim.DegerAl(dr, 4, 0);
                f.hesapUzunluk = VeriErisim.DegerAl(dr, 5, 0);
                f.fonUzunluk = VeriErisim.DegerAl(dr, 6, 0);
                f.finUzunluk = VeriErisim.DegerAl(dr, 7, 0);
                liste.objeler.Add(f);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return liste;
        }
        /// <summary>
        /// KBSs the aktarim kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">The liste.</param>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public Sonuc KBSAktarimKaydet(Kullanici kullanan, ObjectArray liste, int yil)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            sonuc = to.KBSAktarimSil(yil);
            foreach (KBSAktarim kbs in liste.objeler)
            {
                kbs.kod = Guid.NewGuid().ToString().Replace("-", "");
                kbs.yil = yil;
                sonuc = to.KBSAktarimKaydet(kbs);
                if (!sonuc.islemSonuc)
                    break;
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
        /// <summary>
        /// KBSs the aktarim kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">The liste.</param>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public Sonuc KBSAktarimDetayKaydet(Kullanici kullanan, ObjectArray liste, int yil)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            sonuc = to.KBSAktarimDetaySil(yil);
            foreach (KBSAktarim kbs in liste.objeler)
            {
                kbs.kod = Guid.NewGuid().ToString().Replace("-", "");
                kbs.yil = yil;
                sonuc = to.KBSAktarimDetayKaydet(kbs);
                if (!sonuc.islemSonuc)
                    break;
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
        /// <summary>
        /// KBSs the aktarim listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public ObjectArray KBSAktarimDetayListele(Kullanici kullanan, int yil)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            IDataReader dr = oe.KBSAktarimDetayListele(yil);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                return liste;
            }
            while (dr.Read())
            {
                KBSAktarim f = new KBSAktarim();
                f.yil = VeriErisim.DegerAl(dr, 0, 0);
                f.hesapKod = VeriErisim.DegerAl(dr, 1, "");
                f.hesapAd = VeriErisim.DegerAl(dr, 2, "");
                f.kod = VeriErisim.DegerAl(dr, 3, "");
                liste.objeler.Add(f);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return liste;
        }
        /// <summary>
        /// Odemes the emri evrak no oku.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        public string OdemeEmriEvrakNoOku(int yil, string belgeNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            string evrakNo = oe.OdemeEmriEvrakNoOku(yil, belgeNo);
            ve.VeriErisimKapat();
            return evrakNo;
        }
        /// <summary>
        /// Alts the hesap liste hazirla.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="kriter">The kriter.</param>
        /// <returns></returns>
        public ObjectArray AltHesapListeHazirla(Kullanici kullanan, int muhasebe, string kriter)
        {
            ObjectArray liste = new ObjectArray();
            //Firma tablosundan gelenleri ekle
            TNS.HRC.FirmaBilgisi f = new TNS.HRC.FirmaBilgisi();
            f.muhasebe = muhasebe;
            f.ad = kriter.Trim();
            f.aktifDurum = 1;
            ObjectArray fl = servisHRC.FirmaBilgisiListele(kullanan, f);
            foreach (TNS.HRC.FirmaBilgisi fb in fl.objeler)
            {
                TNS.HRC.FirmaBilgisi ff = new FirmaBilgisi();
                ff.vergiNo = fb.vergiNo.Trim();
                ff.ad = fb.ad;
                liste.objeler.Add(ff);
            }
            if (fl.objeler.Count > 0)
            {
                //Eğer kriter vergino ise ve firma bilgisinden bilgi döndü ise personelden aramaya gerek yok.
                //Özellikle EGO Açılış fişinde ~6500 kişi var performans için bu kriter konuldu 26.03.2015 MELİH
                if (((TNS.HRC.FirmaBilgisi)liste.objeler[0]).vergiNo.Trim() == kriter.Trim())
                    return liste;
            }
            bool bulundu = false;
            //Personel tablosundan gelenleri ekle
            TNS.PER.Personel p = new TNS.PER.Personel();
            p.muhasebeKodu = 0;
            p.adSoyadTc = kriter;
            ObjectArray pl = servisPER.PersonelListele(kullanan, p);
            foreach (TNS.PER.Personel pb in pl.objeler)
            {
                TNS.HRC.FirmaBilgisi ff = new FirmaBilgisi();
                ff.vergiNo = pb.tcKimlikNo;
                ff.ad = pb.ad + " " + pb.soyad;
                //Firma tablosundan gelenlerin tekrar eklenmemesi için
                bulundu = false;
                foreach (TNS.HRC.FirmaBilgisi kk in liste.objeler)
                {
                    if (kk.vergiNo == ff.vergiNo)
                    {
                        bulundu = true;
                        break;
                    }
                }
                if (!bulundu)
                    liste.objeler.Add(ff);
            }
            return liste;
        }
        public OdemeEmriMIFDetay KDVBilgisiBul(Kullanici kullanan, int yil, string belgeNo, int satirNo, string hesapKod)
        {
            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(yil);
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF tab = new TABOdemeEmriMIF(ve);
            IDataReader r = null;
            try
            {
                r = tab.TekBelgeOkuDetay(yil, belgeNo, "", 0);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                return detay;
            }
            if (r != null)
            {
                Tertip th = new Tertip(yil, ENUMTabloKod.TABOdemeEmri);
                Tertip tt = new Tertip(yil, ENUMTabloKod.TABOdenek);

                while (r.Read())
                {
                    OdemeEmriMIFDetay sd = DataReaderdanDetayOl(r, yil, th, tt);
                    if (sd.hesapKod.StartsWith(hesapKod))
                        detay = sd;
                    if (sd.satirNo >= satirNo)
                        break;
                }
                r.Close();
            }
            if (!detay.hesapKod.StartsWith(hesapKod))
                detay = new OdemeEmriMIFDetay(yil);
            ve.VeriErisimKapat();
            return detay;
        }

        internal static YuzdeOnBilgi YuzdeOnUstSinirBul(VeriErisim veriErisim, Kullanici kullanan, int yil, YuzdeOnBilgi yob)
        {
            UstSinirBul(yil, yob);
            return yob;
        }
        internal static ObjectArray AvansBelgeleriListele(VeriErisim veriErisim, Kullanici kullanan, OdemeEmriMIFForm kriter, bool mahsuplarDahilVer)
        {
            double borcMiktari = 0;
            if (!mahsuplarDahilVer) borcMiktari = 0.01;
            kriter.detay.Ekle(new OdemeEmriMIFDetay(DateTime.Now.Year, "16", borcMiktari, 0));
            kriter.formDurum = 22;//Bütün Belgeler (İptal hariç)
            return new MUHOdemeEmriMIFServis().Listele(kullanan, kriter, false, true, false);
        }
        internal static Sonuc AltHesapDegistir(VeriErisim veriErisim, Kullanici kullanan, int yil, string belgeNo, int sirano, string altHesapKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            ve.BeginTransaction();
            Sonuc s;
            if (string.IsNullOrWhiteSpace(altHesapKod))
                s = new Sonuc("Alt hesap kodu boş olamaz!");
            else
                s = oe.AltHesapDegistir(yil, belgeNo, sirano, altHesapKod.Trim());
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }
        /// <summary>
        /// Ödeme emri / MIF kaydetme işlemi
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yeniVergiNo">The yeni vergi no.</param>
        /// <param name="eskiVergiNo">The eski vergi no.</param>
        /// <returns></returns>
        internal static Sonuc VergiNoGuncelle(VeriErisim veriErisim, Kullanici kullanan, string yeniVergiNo, string eskiVergiNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            ve.BeginTransaction();
            Sonuc s = oe.GuncelleAltHesapKod(yeniVergiNo.Trim(), eskiVergiNo.Trim());
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }
        internal static Sonuc FirmaBilgisiSil(VeriErisim veriErisim, Kullanici kullanan, string vergiNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            ve.BeginTransaction();
            Sonuc s = oe.SilHRCFirma(vergiNo.Trim());
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }
        internal static string MukerrerlikKontrolu(VeriErisim veriErisim, Kullanici kullanan, OdemeEmriMIFForm form)
        {
            string benzerBelgeNo = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            string kontrolHesapKod = "";
            double kontrolBorc = 0.0;
            double kontrolAlacak = 0.0;
            foreach (OdemeEmriMIFDetay dt in form.detay.objeler)
            {
                string hesapKod = dt.hesapKod.Replace(".", "").Trim(); ;
                if (hesapKod.StartsWith("9")) continue;
                if (kontrolHesapKod != "") kontrolHesapKod += ",";
                kontrolHesapKod += "'" + hesapKod + "'";
                kontrolBorc += dt.borc;
                kontrolAlacak += dt.alacak;
            }
            IDataReader dr = oe.MukerrerBelgeKontrol(form.yil, form.muhasebe, form.birim, form.ilgiliNo, kontrolHesapKod, kontrolBorc, kontrolAlacak);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                return "";
            }
            while (dr.Read())
            {
                if (benzerBelgeNo != "") benzerBelgeNo += ";";
                benzerBelgeNo += VeriErisim.DegerAl(dr, 1, "");
            }
            dr.Close();
            ve.VeriErisimKapat();
            return benzerBelgeNo;
        }
        internal static string AvansKontrolu(VeriErisim veriErisim, Kullanici kullanan, OdemeEmriMIFForm form, OdemeEmriMIFDetay detay)
        {
            string altHesapKod = "";
            string kontrolHesapKod = "";
            double verilecekAvans = 0.0;
            //foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
            {
                //if (detay.hesapKod.StartsWith("160") && detay.borc > 0)
                if (detay.borc > 0)
                {
                    altHesapKod = detay.altHesapKod;
                    kontrolHesapKod = detay.hesapKod.Replace(".", "");
                    if (form.belgeNo != "")//Eğer daha önce kayıt edilmiş bir belge update ediliyorsa.
                        verilecekAvans = detay.borc;
                }
            }
            if (altHesapKod != "" && kontrolHesapKod != "")
            {
                TNS.TNSDateTime tarih1 = new TNS.TNSDateTime(new DateTime(form.yil, 1, 1));
                TNS.TNSDateTime tarih2 = new TNS.TNSDateTime(new DateTime(form.yil, 12, 31));
                TNS.MUH.OdemeEmriMIFForm oEmri = new TNS.MUH.OdemeEmriMIFForm();
                oEmri.yil = form.yil;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
                oEmri.formDurum = 22;
                oEmri.sorguEkoSiraliMi = true;
                TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new TNS.MUH.OdemeEmriMIFDetay(oEmri.yil);
                oEmriDetay.hesapKod = kontrolHesapKod;
                oEmriDetay.altHesapKod = altHesapKod;
                //OrtakFonksiyonlar.HataDosyaYaz("Kontrol.txt", "Avanskontrol", false);
                oEmri.detay.objeler.Add(oEmriDetay);
                TNS.MUH.BuyukDefter[] mb = new MUHMizanBilanco().BuyukDefterListele(kullanan, oEmri, 0, 3000, "10");
                double bakiye = 0.0;
                foreach (TNS.MUH.BuyukDefter bd in mb)
                {
                    if (form.belgeNo == bd.belgeNo) continue;
                    bakiye += (bd.borc - bd.alacak);
                }
                if (Math.Round((bakiye - verilecekAvans), 2) > 0)
                    return "<br>" + altHesapKod + " Kimlik numaralı kişinin, " + kontrolHesapKod + " hesap kodu ile daha önce aldığı avans kapatılmadığı için yeni avans verilemez.<br>Kapatılması gereken Avans tutarı " + bakiye.ToString("#,###.00");
            }
            return "";
        }
        internal Sonuc OdemeEmriEvrakNoUret(VeriErisim veriErisim, Kullanici kullanan, int yil, string belgeNo)
        {
            if (yil < 2000 || string.IsNullOrWhiteSpace(belgeNo))
                return new Sonuc("Belge yılı ve numarasını girmelisiniz!");
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            OdemeEmriMIFForm form = new OdemeEmriMIFForm();
            form.yil = yil;
            form.belgeNo = belgeNo;
            form = this.TekBelgeGetir(veriErisim, kullanan, form, false);
            Sonuc s = OdemeEmriEvrakNoUret(ve, kullanan, form);
            ve.VeriErisimKapat();
            return s;
        }
        internal Sonuc OdemeEmriEvrakNoUret(VeriErisim ve, Kullanici kullanan, OdemeEmriMIFForm form)
        {
            if (form == null || form.yil < 2000 || string.IsNullOrWhiteSpace(form.belgeNo)) return new Sonuc("Belge yılı ve numarasını girmelisiniz!");
            string evrakBirim = kullanan.YetkiTipiDegeriBul(52);
            if (string.IsNullOrWhiteSpace(evrakBirim)) return new Sonuc("Kullanıcının yetkili olduğu evrak birimini girmelisiniz!");
            string ilkBirim = evrakBirim.Split(',', ';')[0];
            string idareKimlikKodu = ve.TekKolonSQLStr("SELECT IDAREKIMLIKKODU FROM " + TABOrtak.DatabaseAdBul("Evrak.dbo.") + "EVRBIRIM where KOD=" + VeriErisim.DegerYap(ilkBirim));
            if (string.IsNullOrWhiteSpace(idareKimlikKodu)) return new Sonuc(ilkBirim + " kodlu birimin idare kimlik kodu bulunamadı!");
            string dosyaKodu = null;
            if (string.IsNullOrWhiteSpace(form.ihale))
                dosyaKodu = "841.02.17";
            else
                dosyaKodu = "934.01.19";
            string evrakKodu = idareKimlikKodu + "-" + dosyaKodu;
            TABGenel tg = new TABGenel(ve);
            Sonuc sonuc = new Sonuc();
            TNSDateTime dt = new TNSDateTime(DateTime.Now);
            sonuc.anahtar = dt.ToString() + "/" + evrakKodu + "-" + tg.BelgeNoAl(evrakKodu, "00000");
            return sonuc;
        }
        public ObjectArray KapatmaListele(Kullanici kullanan, int yil, string belgeNo)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            IDataReader dr = oe.KapatmaListele(yil, belgeNo);
            if (dr != null)
            {
                while (dr.Read())
                {
                    KapanisBilgileri f = new KapanisBilgileri();
                    f.yil = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.YIL, 0);
                    f.belgeNo = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.BELGENO, "");
                    f.kBelgeNo = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KBELGENO, "");
                    f.kHesapKod = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KHESAPKOD, "");
                    f.kIlgiliVno = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KILGILIVNO, "");
                    f.kTertip = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KTERTIP, "");
                    f.kTutar = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KTUTAR, 0);
                    f.siraNo = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.SIRANO, 0);
                    f.borcAlacak = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.BORCALACAK, 0);
                    f.kBirimKod = VeriErisim.DegerAl(dr, (int)TABOdemeEmriMIF.ENUMAlanlarKAPANIS.KBIRIMKOD, "");
                    liste.objeler.Add(f);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }
        public ObjectArray BirimGelirGider(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            IDataReader dr = oe.BirimGelirGider(kriter);
            if (dr != null)
            {
                while (dr.Read())
                {
                    OdenekDurum f = new OdenekDurum();
                    f.yil = VeriErisim.DegerAl(dr, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    f.birim = VeriErisim.DegerAl(dr, 2, "");
                    f.birimAd = VeriErisim.DegerAl(dr, 3, "");
                    f.muhasebeAd = VeriErisim.DegerAl(dr, 4, "");//Hesap kodu olarak kullanılıyor
                    f.toplamOdenek = VeriErisim.DegerAl(dr, 5, 0.0);
                    liste.objeler.Add(f);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }
        public ObjectArray GelirGiderListe(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            Tertip th = new Tertip(kriter.yil, ENUMTabloKod.TABOdemeEmri);
            IDataReader dr = oe.GelirGiderListe(kriter);
            if (dr != null)
            {
                while (dr.Read())
                {
                    OdenekDurum f = new OdenekDurum();
                    f.muhasebeAd = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), VeriErisim.DegerAl(dr, 0, ""));//Hesap kodu olarak kullanılıyor
                    f.birimAd = VeriErisim.DegerAl(dr, 1, "");//Hesap adı olarak kullanılıyor
                    f.toplamOdenek = Math.Abs(VeriErisim.DegerAl(dr, 2, 0.0));
                    liste.objeler.Add(f);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }
        public ObjectArray EkoKod1Basamak(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            Tertip th = new Tertip(kriter.yil, ENUMTabloKod.TABOdemeEmri);
            IDataReader dr = oe.EkoKod1Basamak(kriter);
            if (dr != null)
            {
                while (dr.Read())
                {
                    OdemeEmriMIFDetay f = new OdemeEmriMIFDetay(kriter.yil);
                    f.yil = VeriErisim.DegerAl(dr, 0, 0);
                    f.detayBirim = VeriErisim.DegerAl(dr, 1, "");
                    f.altHesapAd = VeriErisim.DegerAl(dr, 2, "");
                    f.altHesapKod = VeriErisim.DegerAl(dr, 3, "");
                    f.eko = VeriErisim.DegerAl(dr, 4, "");
                    f.satirAciklama = VeriErisim.DegerAl(dr, 5, "");
                    f.belgeNo = VeriErisim.DegerAl(dr, 6).ToString();
                    f.borc = Math.Abs(VeriErisim.DegerAl(dr, 7, 0.0));
                    liste.objeler.Add(f);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }
        public static Sonuc KaydetEntegraston(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            foreach (MIFEntegrasyon bilgi in liste.objeler)
            {
                sonuc = to.SilEntegrasyon(bilgi);
                sonuc = to.KaydetEntegrasyon(bilgi);
                if (!sonuc.islemSonuc)
                    break;
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }
        public static ObjectArray ManasGunlukTakipGetir(Kullanici kullanan, ManasGunlukTakip kriter)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriMIF oe = new TABOdemeEmriMIF(ve);
            IDataReader dr = oe.ManasGunlukTakip(kriter, "BAKIYE");
            if (dr != null)
            {
                while (dr.Read())
                {
                    ManasGunlukTakip f = new ManasGunlukTakip();
                    f.hesapKod = VeriErisim.DegerAl(dr, 0, "");
                    f.bakiye = VeriErisim.DegerAl(dr, 1, 0);
                    f.dovizBakiye = VeriErisim.DegerAl(dr, 2, 0);
                    f.bakiyeBorc = VeriErisim.DegerAl(dr, 3, 0);
                    f.bakiyeAlacak = VeriErisim.DegerAl(dr, 4, 0);
                    f.dovizBakiyeBorc = VeriErisim.DegerAl(dr, 5, 0);
                    f.dovizBakiyeAlacak = VeriErisim.DegerAl(dr, 6, 0);
                    liste.objeler.Add(f);
                }
                dr.Close();
            }
            dr = oe.ManasGunlukTakip(kriter, "BORCALACAK");
            if (dr != null)
            {
                while (dr.Read())
                {
                    ManasGunlukTakip f = new ManasGunlukTakip();
                    f.hesapKod = VeriErisim.DegerAl(dr, 0, "");
                    f.borc = VeriErisim.DegerAl(dr, 1, 0);
                    f.alacak = VeriErisim.DegerAl(dr, 2, 0);
                    f.dovizBorc = VeriErisim.DegerAl(dr, 3, 0);
                    f.dovizAlacak = VeriErisim.DegerAl(dr, 4, 0);
                    bool bulundu = false;
                    foreach (ManasGunlukTakip item in liste.objeler)
                    {
                        if (item.hesapKod == f.hesapKod)
                        {
                            bulundu = true;
                            item.borc = f.borc;
                            item.alacak = f.alacak;
                            item.dovizBorc = f.dovizBorc;
                            item.dovizAlacak = f.dovizAlacak;
                        }
                    }
                    if (!bulundu)
                        liste.objeler.Add(f);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }
    }
    class DetayKiyasla : IComparer
    {
        string yontemKod;
        public DetayKiyasla(string yontemKod)
        {
            this.yontemKod = yontemKod;
        }
        public int Compare(object x, object y)
        {
            if (x is OdemeEmriMIFDetay && y is OdemeEmriMIFDetay)
            {
                OdemeEmriMIFDetay o1 = (OdemeEmriMIFDetay)x;
                OdemeEmriMIFDetay o2 = (OdemeEmriMIFDetay)y;
                return o1.Kiyasla(o2, yontemKod);
            }
            else
                return 0;
        }
    }
}
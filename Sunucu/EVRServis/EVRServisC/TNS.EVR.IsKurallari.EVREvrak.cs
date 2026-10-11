using System;
using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System.IO;
using System.Collections.Generic;
using WebServisler.Evrak.SgbnetWCFServisi;
using WebServisler.ProjeServis;
using System.Web.Script.Serialization;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak işlemleriyle ilgili sunucu yordamlarını tutan sınıf.
    /// </summary>
    public class EVREvrak
    {
        /// <summary>
        /// Evrak üzerinde işlemler yapabilmek için, evrak yetki kontrolü yapan yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public static Sonuc KontrolEt(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (form.aktarim)
                return sonuc;

            //Evrak imzalı ise değişiklik yapılamaz
            if (form.evrEvrakNo != "" && form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
            {
                bool evrakImzaliMi = !form.imzaTarihi.isNull;
                if (!evrakImzaliMi)
                    evrakImzaliMi = EvrakImzaliMi(veriErisim, kullanan, form.evrEvrakNo);

                //Evrak meta alanlarının girişinde imzalı olması kayıt edilmesini engellemesin
                //sadece icerik veya antet veya altbilgi alanlarında birisinin boş olmaması (editor ekranından) kayıt edilmesin
                if (evrakImzaliMi &&
                    (!string.IsNullOrEmpty(form.icerik) || !string.IsNullOrEmpty(form.antet) || !string.IsNullOrEmpty(form.altBilgi)))
                {
                    sonuc.DegerAta("İmzalı / Paraflı evrak üzerinde değişiklik yapılamaz!");

                    bool degisiklikYapilabilirMi = ImzaliEvrakDegisiklikYapilabilirMi(form.bulunduguYer);
                    if (!degisiklikYapilabilirMi)
                        return sonuc;
                }
            }
            //########################################

            if (form.gizliEvrakIslemi)
            {
                sonuc.hataStr = "Gizli evrak üzerinde değişiklik yapma yetkiniz yok!";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            bool uygun = TNS.EVR.Yetki.YazmaYetkisiVarMi(kullanan, form);
            if (!uygun)
            {
                sonuc.hataStr = EvrakI.EVRI041;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            else if (form.evrEvrakNo != "" && !(kullanan.KullaniciTipiIceriyorMu((int)TNS.KYM.ENUMKullaniciTipi.EVRAKBELGEYONETICISI) || kullanan.KullaniciTipiIceriyorMu((int)TNS.KYM.ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI)))
            {
                ObjectArray evraklar = new ObjectArray();
                try
                {
                    evraklar = Listele(veriErisim, kullanan, form);
                    if (evraklar.objeler.Count > 0)
                    {
                        EvrakSinif formKontrol = (EvrakSinif)evraklar.objeler[0];

                        if (formKontrol.hareket.islem > 50)
                        {
                            sonuc.hataStr = EvrakI.EVRI042;
                            sonuc.islemSonuc = false;
                            return sonuc;
                        }

                        if (formKontrol.sicil.asil != 1)
                        {
                            sonuc.hataStr = EvrakI.EVRI043;
                            sonuc.islemSonuc = false;
                            return sonuc;
                        }
                    }
                    else
                    {
                        sonuc.hataStr = "Evrak üzerinde işlem yapma yetkiniz bulunmamaktadır.";
                        sonuc.islemSonuc = false;
                        return sonuc;
                    }
                }
                catch (Exception e)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = e.Message;
                    Arac.HataStrYaz("{F00A1DAF-C779-4ED1-978B-1E1B2BBD087X} Message: " + sonuc.hataStr);
                    return sonuc;
                }

            }

            bool ayniEvrakMevcutMuKontol = true;

            if (form.evrakTipi != (int)ENUMEvrakTipi.GELEN && form.evrakTipi != (int)ENUMEvrakTipi.GIDEN && form.evrakTipi != (int)ENUMEvrakTipi.FORMLAR)
                sonuc.hataStr += EvrakI.EVRI044 + "<br>";
            if (string.IsNullOrEmpty(form.konu.Trim()))
                sonuc.hataStr += EvrakI.EVRI045 + "<br>";
            if (string.IsNullOrEmpty(form.evrakBirimKodu.Trim()))
                sonuc.hataStr += EvrakI.EVRI046 + "<br>";
            if (string.IsNullOrEmpty(form.bulunduguYer.Trim()))
                sonuc.hataStr += EvrakI.EVRI047 + "<br>";
            if (form.geldigiYer.Trim() == "")
                sonuc.hataStr += EvrakI.EVRI048 + "<br>";
            if (form.evrakTipi == (int)ENUMEvrakTipi.GELEN)
            {
                if (form.gelenEvrakTarih.isNull)
                    sonuc.hataStr += EvrakI.EVRI049 + "<br>";
                if (form.gelenEvrakNo.Trim() == "")
                {
                    if ((form.yaziDurum == (int)ENUMYaziDurum.DILEKCE || (form.geldigiYerTur == (int)ENUMEvrakGeldigiYerTur.TUZEL || form.geldigiYerTur == (int)ENUMEvrakGeldigiYerTur.OZEL)))
                        ayniEvrakMevcutMuKontol = false;
                    else
                        sonuc.hataStr += EvrakI.EVRI050 + "<br>";
                }
                if (Math.Abs(form.cevapSuresi) > 999)
                    sonuc.hataStr += "Cevap süresi gün olarak belirtilmelidir." + "<br>";

                TNSDateTime bugun = new TNSDateTime(DateTime.Now.ToShortDateString());
                if (bugun.Oku().AddDays(1) <= form.gelenEvrakTarih.Oku())
                    sonuc.hataStr += EvrakI.EVRI051 + "<br>";
            }
            else if (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
            {
                if (form.evrakTuru == 0)
                    sonuc.hataStr += EvrakI.EVRI052 + "<br>";
                if (form.dosyaKodlari == null || form.dosyaKodlari.Count == 0)
                    sonuc.hataStr += EvrakI.EVRI165 + "<br>";
                //if (form.imzaYolu == 0)
                //    sonuc.hataStr += EvrakI.EVRI166 + "<br>";
            }

            //Yeni kayıt edilen evrak daha önce kayıt edilmiş mi?
            if (sonuc.hataStr == "")
            {
                if (ayniEvrakMevcutMuKontol)
                {
                    sonuc = AyniEvrakMevcutMu(veriErisim, kullanan, form);
                    if (!sonuc.islemSonuc)
                        return sonuc;
                }
            }

            //Evrak Birimi açık mı?
            if (sonuc.hataStr == "")
            {
                if (!EvrakBirimiAcikMi(veriErisim, form.bulunduguYer))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(EvrakI.EVRI053, form.bulunduguYer) + "<br>";
                    return sonuc;
                }
            }

            if (sonuc.hataStr != "")
                sonuc.islemSonuc = false;

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgisine göre, evrak'ın veritabanında olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="kullanan">kullanıcı bilgisini içeren nesne</param>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc AyniEvrakMevcutMu(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);
            string evrEvrakNo = tab.AyniEvraktanVarmi(form);

            if (evrEvrakNo != "")
            {
                EvrakSinif evrak = new EvrakSinif();
                evrak.evrEvrakNo = evrEvrakNo;
                evrak.evrakTipi = (int)ENUMEvrakTipi.GELEN;

                ObjectArray list = Listele(ve, kullanan, evrak);
                if (list.ObjeSayisi > 0)
                {
                    evrak = (EvrakSinif)list[0];

                    if (evrak.hareket.islem != (int)ENUMEvrakHareketDurum.IPTALEDILDI)
                    {
                        sonuc.hataStr += string.Format(EvrakI.EVRI054, evrak.evrakTarih.ToString(), evrak.evrakNo) + "<br>";
                        sonuc.hataStr += EvrakI.EVRI055;
                        sonuc.islemSonuc = false;
                    }
                }
            }
            else
                sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak biriminin kullanılabilir olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="birimKod">Evrak birim kodunu içeren nesne</param>
        /// <returns>
        /// Evrak birimi kulanılabilir ise doğru, yoksa yanlış döndürülür.
        /// </returns>
        public static bool EvrakBirimiAcikMi(VeriErisim veriErisim, string birimKod)
        {
            Birim birim = new Birim();
            birim.kod = birimKod;
            birim = EVRBirim.Oku(veriErisim, birim);

            if (birim.kullanilmiyor < 2 && birim.kod == birimKod)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesnesine göre, Evrak bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI056, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            //Fazla boşlukları at
            form.konu = System.Text.RegularExpressions.Regex.Replace(form.konu, @"\s+", " ").Trim();
            form.geldigiYer = System.Text.RegularExpressions.Regex.Replace(form.geldigiYer.Replace("\n", "<br>"), @"\s+", " ").Replace("<br>", "\n").Trim();

            if (!(form.kayitIslem == (int)ENUMEvrakKayitIslem.EYAZISMA && form.evrEvrakNo == ""))
            {
                sonuc = KontrolEt(ve, kullanan, form);
                if (!sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            TABEvrak to = new TABEvrak(ve);
            TABGenel tabGenel = new TABGenel(ve);

            bool yeniEvrak = false;

            ve.BeginTransaction();
            try
            {
                if (form.evrEvrakNo == "")
                {
                    string sayacKod = "EVREVRAKNO" + DateTime.Now.Year;
                    form.evrEvrakNo = DateTime.Now.Year + tabGenel.BelgeNoAl(sayacKod, "0000000");
                    yeniEvrak = true;

                    form.prEvrakNo = PrEvrakNoAl(ve);

                    form.uretici = kullanan.ad + " " + kullanan.soyad;
                    form.uretimTarihi = new TNSDateTime(DateTime.Now);

                    // //Web Servis Evrak Kayıt Kontrol - EvrakNo, Yıl, Evrak Tarihi otomatik değil parametre ile gelenler kaydedilsin
                    if (!form.webServisXmlEkParametre.OtoEvrakNoAlma)
                    {
                        if (form.evrakTipi == (int)ENUMEvrakTipi.GELEN & form.evrakNo > -1)
                        {
                            sayacKod = "EVRGELEN" + EvrakNoSayacBaslangicBilgisiAl() + EVRBirim.EvrakKokBirimGetir(ve, form.evrakBirimKodu);
                            form.evrakNo = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "0000000"), 0);
                        }
                        else
                            form.evrakNo = 0;
                    }

                    if (!form.webServisXmlEkParametre.OtoYilAlma)
                        form.yil = DateTime.Now.Year;

                    if (!form.webServisXmlEkParametre.OtoEvrakTarihAlma)
                        form.evrakTarih = new TNSDateTime(DateTime.Now);

                    form.evrakOlusturmaTarihi = new TNSDateTime(DateTime.Now);

                    if (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN && form.evrakTarih.Yil == 1)
                        form.evrakTarih = new TNSDateTime(DateTime.Now);

                    if (form.evrakNo > 0)//Eğer giden evrak numarası elle veriliyorsa (aspx sayfasında) yıl evrakTarihin yılı olmalı Melih 22.03.2014
                    {
                        if (form.evrakTarih != null && form.evrakTarih.Yil > 1000) form.yil = form.evrakTarih.Yil;
                    }

                    //***********************************************************************************************
                }
                else if (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                {
                    //Editör Kullanılıyorsa ve Kayıt editör üzerinden gelmiyorsa editör Güncellemeli olarak işaretle
                    if (EditorKullaniliyorMu(ve, kullanan, form.evrEvrakNo) && !EvrakImzaliMi(ve, kullanan, form.evrEvrakNo))
                    {
                        bool editordenKayit = false;
                        if (form.kayitKriterleri.ContainsKey("editordenKayit") && (bool)form.kayitKriterleri["editordenKayit"])
                            editordenKayit = true;

                        form.editorGuncellenmeli = (editordenKayit ? 0 : 1);
                    }
                    //****************************************************
                }


                sonuc = to.Kaydet(form);

                //Evrak hareket ve tarihcede sorun oluğu zaman kayıt işlemlerini geri alabilmeliyiz
                if (sonuc.islemSonuc)
                {
                    KonuKaydet(ve, form.bulunduguYer, form.konu, 0);
                    YerIsimleriKaydet(ve, form.bulunduguYer, form.geldigiYer, form.geldigiYerTur, "", 0);

                    if (yeniEvrak || form.aktarim)
                    {
                        EvrakSicil es = new EvrakSicil();
                        es.evrEvrakNo = form.evrEvrakNo;
                        es.prEvrakNo = form.prEvrakNo;
                        es.asil = 1;
                        sonuc = EVREvrak.KaydetSicil(ve, kullanan, es);
                    }

                    //Dosya seçilmiş ise kaydet
                    if (form.dosyaKodlari.Count > 0)
                        sonuc = EVREvrakDosyaPlani.Kaydet(ve, kullanan, form.prEvrakNo, form.dosyaKodlari, true, null);

                    if (sonuc.islemSonuc)
                    {
                        //Tarihçesini Kaydet
                        EvrakTarihce tarihce = new EvrakTarihce();
                        tarihce.evrEvrakNo = form.evrEvrakNo;
                        if (yeniEvrak)
                            tarihce.islem = ENUMEvrakTarihceDurum.YENI.ToString();
                        else
                            tarihce.islem = ENUMEvrakTarihceDurum.DEGISTIRILDI.ToString();

                        if (form.tarihce != null && !string.IsNullOrEmpty(form.tarihce.vekil))
                        {
                            tarihce.vekil = form.tarihce.vekil;
                            tarihce.vekilAd = form.tarihce.vekilAd;
                        }
                        sonuc = EVREvrakTarihce.Kaydet(ve, kullanan, tarihce);
                    }

                    if ((yeniEvrak && sonuc.islemSonuc) || form.aktarim)
                    {
                        EvrakHareket hareket = new EvrakHareket();
                        hareket.prEvrakNo = form.prEvrakNo;
                        {
                            /*
                             Giden evrak'ın dağıtım listesindeki birimler, dosyaya kaldırıldığında gelen evrak olarak kayıt edilir.
                             Bu kayıt sırasında evrak üzerinde çalıştıklarımda değil teslim almadıklarıma yer alması için...  04.08.2011 (Hüseyin)
                            */
                            string geldigiYer = form.bulunduguYer;
                            int islem = (int)ENUMEvrakHareketDurum.YENI;
                            int okundu = 1; /*Okundu olarak kaydet ilk Kayıtta*/


                            if (form.kayitIslem == (int)ENUMEvrakKayitIslem.DAGITIMDANKAYIT)
                            {
                                islem = (int)ENUMEvrakHareketDurum.DAGITIMKAYIT;
                                okundu = 0;
                                hareket.birimKod2 = form.bulunduguYer;
                                geldigiYer = form.geldigiYerKod;
                                if (form.kayitKriterleri.ContainsKey("dagitimKisi"))    //Kişiye dağıtım yapılsı    20.10.2015
                                    hareket.kisi2 = form.kayitKriterleri["dagitimKisi"] + "";
                            }
                            else if (form.kayitIslem == (int)ENUMEvrakKayitIslem.EYAZISMA)
                            {
                                islem = (int)ENUMEvrakHareketDurum.HAVALEEDILDI;
                                okundu = 0;
                                hareket.birimKod2 = form.bulunduguYer;
                                geldigiYer = form.geldigiYerKod;
                                hareket.islemYapanAd = form.geldigiYer;
                            }

                            if (form.webServisXmlEkParametre.WebServisIleKayit && form.webServisXmlEkParametre.OtoEvrakNoAlma)
                            {
                                islem = (int)ENUMEvrakHareketDurum.HAVALEEDILDI;
                                okundu = 0;
                                hareket.birimKod2 = geldigiYer;
                            }

                            hareket.islem = islem;
                            hareket.okundu = okundu;
                            hareket.birimKod1 = geldigiYer;
                        }

                        if (!form.aktarim)
                        {
                            if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, form.bulunduguYer) || form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                                hareket.kisi1 = kullanan.mernis;
                        }

                        if (form.sablonXmlKayit)
                        {
                            hareket.kisi1 = form.hareket.kisi1;
                        }

                        if (form.hareket != null && !string.IsNullOrEmpty(form.hareket.vekil))
                        {
                            hareket.vekil = form.hareket.vekil;
                            hareket.vekilAd = form.hareket.vekilAd;
                        }
                        sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                    }


                    if (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN && sonuc.islemSonuc)
                    {
                        if ((yeniEvrak && form.imzaYolu > 0) || form.imzaYoluDegisti)
                            sonuc = EVREvrak.EvrakImzaYoluYenile(ve, kullanan, form);

                        if (form.evrakIlgi != null)
                            EVREvrakIlgi.Kaydet(ve, kullanan, form.prEvrakNo, form.evrakIlgi);

                        ////Editör Kullanılıyorsa ve Kayıt editör üzerinden gelmiyorsa editör Güncellemeli olarak işaretle
                        //if (EditorKullaniliyorMu(ve, kullanan, form.evrEvrakNo) && !EvrakImzaliMi(ve, kullanan, form.evrEvrakNo))
                        //{
                        //    bool editordenKayit = false;
                        //    if (form.kayitKriterleri.ContainsKey("editordenKayit") && (bool)form.kayitKriterleri["editordenKayit"])
                        //        editordenKayit = true;

                        //    bool guncellenmeli = editordenKayit ? false : true;

                        //    EditorGuncellenmeliDurumunuDegistir(ve, kullanan, form.evrEvrakNo, guncellenmeli);
                        //}
                        ////****************************************************

                    }


                    //Alt kısımda iliskiler nesnesine eklenecek kayıtların tutulması için 14.07.2014 Melih
                    ObjectArray iliskiler = new ObjectArray();

                    if (form.evrakTipi == (int)ENUMEvrakTipi.GELEN && sonuc.islemSonuc)
                    {
                        foreach (EvrakIliski ei in form.iliski)
                        {
                            ei.prEvrakNo = form.prEvrakNo;
                            if (ei.prEvrakNoIliski.Trim() != "")
                                iliskiler.objeler.Add(ei);
                        }
                    }

                    if (yeniEvrak && form.evrakTipi == (int)ENUMEvrakTipi.GIDEN && sonuc.islemSonuc)
                    {
                        foreach (EvrakIliski ei in form.iliski)
                        {
                            ei.prEvrakNo = form.prEvrakNo;
                            if (ei.prEvrakNoIliski.Trim() != "")
                            {
                                iliskiler.objeler.Add(ei);

                                //Hareket olarak kaydet
                                EvrakHareket hareket = new EvrakHareket();
                                hareket.prEvrakNo = ei.prEvrakNoIliski;
                                hareket.birimKod1 = form.bulunduguYer;
                                hareket.islem = (int)ENUMEvrakHareketDurum.CEVAPLANDI;
                                hareket.okundu = 1;//Okundu olarak kaydet ilk Kayıtta
                                if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, form.bulunduguYer) || form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                                    hareket.kisi1 = kullanan.mernis;

                                sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                            }
                        }
                    }

                    //Üst kısımda eklenen ilişklilerin kayıt edilmesi için 14.07.2014 Melih
                    if (iliskiler.objeler.Count > 0)
                        EVREvrak.EvrakIliskiKaydet(ve, kullanan, iliskiler);

                    if (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN && sonuc.islemSonuc)
                    {
                        if (form.dagitim != null)
                            sonuc = EVREvrakDagitim.Kaydet(ve, kullanan, form.prEvrakNo, form.dagitim);
                    }


                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.anahtar = form.evrEvrakNo.ToString() + "-" + form.prEvrakNo.ToString() + "-" + form.evrakNo + "-" + form.evrakTarih.ToString();

                        //HABERCI MAIL GONDER
                        if (yeniEvrak && form.evrakTipi == (int)ENUMEvrakTipi.GELEN && sonuc.islemSonuc)
                            EVRPosta.HaberciMailGonder(ve, form);
                    }
                    else
                        ve.Rollback();
                }
                else
                    ve.Rollback();
            }
            catch (Exception e)
            {
                Arac.HataStrYaz("EvrakListeHatası: " + e.Message + "*" + e.StackTrace);
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, havale kural bilgilerine bakarak evrak'ı havale eden yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <param name="kodList">Havale kural kod listesini içeren nesne</param>
        /// <param name="tcKimlikNo">TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="evrakBirimKodu">Evrak birim kodunu içeren nesne</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public static Sonuc HavaleKuralIleHavale(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form, string kodList, string tcKimlikNo, string evrakBirimKodu)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //Kuraldan donecek Birim ve kisiler
            ObjectArray listGeregi = new ObjectArray();
            ObjectArray listBilgi = new ObjectArray();
            ObjectArray havaleKuralList = EVRHavaleKural.Listele(veriErisim, kodList, tcKimlikNo, evrakBirimKodu);
            HavaleKuralKontrol(form, havaleKuralList, ref listGeregi, ref listBilgi);

            ObjectArray liste = new ObjectArray();
            EvrakHareket hareket = new EvrakHareket();

            string[] str;
            foreach (string item in listGeregi.objeler)
            {
                if (string.IsNullOrEmpty(item))
                    continue;

                str = item.Split(';');
                hareket = new EvrakHareket();
                hareket.prEvrakNo = form.prEvrakNo;
                hareket.birimKod1 = form.bulunduguYer;
                hareket.kisi1 = kullanan.mernis;
                hareket.birimKod2 = str[0];
                hareket.kisi2 = str[1];
                hareket.islem = (int)ENUMEvrakHareketDurum.HAVALEEDILDI;
                hareket.havaleTur = (int)ENUMEvrakHavaleTur.GEREGI;

                form.hareketHavale.Add(hareket);
            }

            if (listBilgi.objeler.Count > 0)
            {
                foreach (string item in listBilgi.objeler)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    str = item.Split(';');
                    hareket = new EvrakHareket();
                    hareket.prEvrakNo = form.prEvrakNo;
                    hareket.birimKod1 = form.bulunduguYer;
                    hareket.kisi1 = kullanan.mernis;
                    hareket.birimKod2 = str[0];
                    hareket.kisi2 = str[1];
                    hareket.islem = (int)ENUMEvrakHareketDurum.HAVALEEDILDI;
                    hareket.havaleTur = (int)ENUMEvrakHavaleTur.BILGI;

                    form.hareketHavale.Add(hareket);
                }
            }

            if (form.hareketHavale.Count > 0)
            {
                liste.objeler.Add(form);
                sonuc = HavaleEt(veriErisim, kullanan, liste, false);
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, havale kurallarının kontrolünü yaparak gereği ve bilgi listesini dolduran yordam.
        /// </summary>
        /// <param name="evrak">Evrak bilgisini içeren nesne</param>
        /// <param name="havaleKuralList">Havale kural listesi içeren nesne</param>
        /// <param name="listGeregi">Gereği bilgilerini içeren nesne</param>
        /// <param name="listBilgi">Bilgi bilgilerini içeren nense</param>
        private static void HavaleKuralKontrol(EvrakSinif evrak, ObjectArray havaleKuralList, ref ObjectArray listGeregi, ref ObjectArray listBilgi)
        {
            TNSDateTime bugun = new TNSDateTime(DateTime.Now.ToShortDateString());
            bool evrakVar;
            string[] deger;

            foreach (HavaleKural h in havaleKuralList.objeler)
            {
                evrakVar = false;
                if (bugun.CompareTo(h.baslangicTarih) >= 0 && (bugun.CompareTo(h.bitisTarih) <= 0 || h.bitisTarih.isNull))
                {
                    foreach (HavaleKuralDetay d in h.detaylar)
                    {
                        switch (d.alan)
                        {
                            case (int)ENUMHavaleKuralAlan.GELDIGIYER:
                                switch (d.islec)
                                {
                                    case (int)ENUMHavaleKuralIslec.ESIT:
                                        if (evrak.geldigiYer.ToUpper() == d.deger.ToUpper()) { evrakVar = true; }
                                        break;
                                    case (int)ENUMHavaleKuralIslec.ILEBASLAYAN:
                                        if (evrak.geldigiYer.ToUpper().StartsWith(d.deger.ToUpper())) { evrakVar = true; }
                                        break;
                                    case (int)ENUMHavaleKuralIslec.ICINDEGECEN:
                                        if (evrak.geldigiYer.IndexOf(d.deger.ToUpper()) > -1) { evrakVar = true; }
                                        break;
                                }
                                break;
                            case (int)ENUMHavaleKuralAlan.KONUSU:
                                switch (d.islec)
                                {
                                    case (int)ENUMHavaleKuralIslec.ESIT:
                                        if (evrak.konu.ToUpper() == d.deger.ToUpper()) { evrakVar = true; }
                                        break;
                                    case (int)ENUMHavaleKuralIslec.ILEBASLAYAN:
                                        if (evrak.konu.ToUpper().StartsWith(d.deger.ToUpper())) { evrakVar = true; }
                                        break;
                                    case (int)ENUMHavaleKuralIslec.ICINDEGECEN:
                                        if (evrak.konu.ToUpper().IndexOf(d.deger.ToUpper()) > -1) { evrakVar = true; }
                                        break;
                                }
                                break;
                            case (int)ENUMHavaleKuralAlan.GELISSEKLI:
                                switch (d.islec)
                                {
                                    case (int)ENUMHavaleKuralIslec.ESIT:
                                        if (evrak.gelisSekli.ToString() == d.deger) { evrakVar = true; }
                                        break;
                                }
                                break;
                            case (int)ENUMHavaleKuralAlan.YAZIDURUMU:
                                switch (d.islec)
                                {
                                    case (int)ENUMHavaleKuralIslec.ESIT:
                                        if (evrak.yaziDurum.ToString() == d.deger) { evrakVar = true; }
                                        break;
                                }
                                break;
                            case (int)ENUMHavaleKuralAlan.GIZLILIKDERECESI:
                                switch (d.islec)
                                {
                                    case (int)ENUMHavaleKuralIslec.ESIT:
                                        if (evrak.gizlilik.ToString() == d.deger) { evrakVar = true; }
                                        break;
                                }
                                break;
                            case (int)ENUMHavaleKuralAlan.SURELI:
                                switch (d.islec)
                                {
                                    case (int)ENUMHavaleKuralIslec.ESIT:
                                        if (evrak.cevapSuresi.ToString() == d.deger) { evrakVar = true; }
                                        break;
                                }
                                break;
                            case (int)ENUMHavaleKuralAlan.DOSYAKODU:
                                foreach (DosyaPlani dp in evrak.dosyaKodlari)
                                    if (!string.IsNullOrEmpty(dp.kod) && d.deger.IndexOf(dp.kod) > -1) { evrakVar = true; break; }
                                break;
                        }

                        if (evrakVar) { break; }
                    }


                    if (evrakVar)
                    {

                        foreach (HavaleKuralGB gb in h.gbler)
                        {
                            string str = "";
                            deger = gb.deger.Split('-');
                            if (deger.Length == 3) { str = deger[1]; }
                            str = deger[0] + ";" + str;

                            if (gb.geregiBilgi == (int)ENUMHavaleKuralGB.GEREGI)
                                if (!listGeregi.objeler.Contains(str))
                                    listGeregi.Ekle(str);
                            if (gb.geregiBilgi == (int)ENUMHavaleKuralGB.BILGI)
                                if (!listBilgi.objeler.Contains(str))
                                    listBilgi.Ekle(str);
                        }
                        //birimVeTcKimilikList.Ekle(h.kullanici); 

                    }

                }
            }
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesnesine göre, Evrak bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <param name="start">The start.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>
        /// EvrakSinif nesne koleksiyonunu olarak döndürülür.
        /// </returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form, int start = 0, int limit = 0, bool sadeceSatirSayisi = false, bool gizliEvrakiOku = false)
        {
            ObjectArray hVAN = new ObjectArray();
            bool tekBelgeListele = false;
            string bulunduguYer = form.bulunduguYer;
            bool tasfiyeyeBakilsin = false;
            bool kullaniciEvrakYetkisi = false;

            if (form.tasfiye.tasfiyeKod != -1)
                tasfiyeyeBakilsin = true;

            if (form.evrEvrakNo != "" || form.prEvrakNo != "") tekBelgeListele = true;
            if (kullanan == null) kullanan = new Kullanici();

            if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, form.bulunduguYer) ||
                (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN && form.hareket.kisi1 != "-100" && form.hareket.kisi1 != "-300"))
                form.hareket.kisi1 = kullanan.mernis;
            else
            {
                form.hareket.kisi1Ad = kullanan.mernis;
                kullaniciEvrakYetkisi = true;
            }

            if (form.prEvrakNo != "")
            {
                if (kullanan.mernis != "" && !GormeYetkisiVarMi(veriErisim, kullanan, form))
                {
                    hVAN.sonuc.hataStr += EvrakI.EVRI057 + "<br>";
                    hVAN.sonuc.islemSonuc = false;
                    return hVAN;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);


            if (form.evrEvrakNo != "" && form.prEvrakNo != "" && form.hareket.islem == 0)
            {
                EvrakHareket hareket = EVREvrakHareket.SonHaraketGetir(ve, form.prEvrakNo);

                if (hareket.prEvrakNo == form.prEvrakNo &&
                    hareket.birimKod2 == form.bulunduguYer &&
                    hareket.islem == (int)ENUMEvrakHareketDurum.HAVALEEDILDI)
                {
                    if (hareket.kisi2 == kullanan.mernis ||
                        (hareket.kisi2 == "" && kullaniciEvrakYetkisi))//Havale eden havale ettiği evrakı görebilmesi için
                        form.hareket.islem = (int)ENUMEvrakTarihceDurum.HAVALEGELEN;//Havale edilen kişi evrakı görebilmesi için
                }
            }


            IDataReader r = to.Listele(form, false, start, limit, sadeceSatirSayisi);
            int sayac = 0;

            if (r != null)
            {
                int index = ((int)TABEvrak.ENUMAlanlar.ASILNUSHAGIDECEK);
                while (r.Read())
                {
                    if (sadeceSatirSayisi)
                    {
                        form = new EvrakSinif();
                        form.siraNo = VeriErisim.DegerAl(r, 0, 0);
                        hVAN.objeler.Add(form);
                        break;
                    }

                    string prEvrakNo = VeriErisim.DegerAl(r, index + 20, "");
                    sayac++;
                    if (limit > 0)
                        if (sayac - 1 < start || sayac - 1 >= start + limit) continue;

                    form = new EvrakSinif();
                    form.evrEvrakNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVREVRAKNO, "");
                    form.yil = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.YIL, 0);
                    form.evrakTipi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKTIPI, 0);
                    form.evrakBirimKodu = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKBIRIMKODU, "");
                    form.evrakNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKNO, 0);
                    form.evrakTarih = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKTARIH);
                    form.geldigiYerTur = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELDIGIYERTUR, 0);
                    form.geldigiYer = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELDIGIYER, "");
                    form.gelenEvrakTarih = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELENEVRAKTARIH);
                    form.gelenEvrakNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELENEVRAKNO, "");
                    form.gizlilik = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GIZLILIK, 0);
                    form.gelisSekli = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELISSEKLI, 0);
                    form.yaziDurum = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.YAZIDURUM, 0);
                    form.konu = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.KONU, "");
                    form.aciklama = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ACIKLAMA, "");
                    form.cevapSuresi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.CEVAPSURESI, 0);
                    form.ekAdedi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EKADEDI, "");
                    form.havaleBirimKodu = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.HAVALEBIRIMKODU, "");
                    form.paraf = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.PARAF, "");
                    form.evrakTuru = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKTURU, 0);
                    form.antet = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ANTET, "");
                    form.icerik = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ICERIK, "");
                    form.imza1 = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMZA1, "");
                    form.imza2 = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMZA2, "");
                    form.imza3 = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMZA3, "");
                    form.ek = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EK, "");
                    form.altBilgi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ALTBILGI, "");
                    form.evrakOlusturmaTarihi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.OLUSTURMATARIHI);
                    form.imzaTarihi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMZATARIHI);
                    form.ekYaziAlan = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EKYAZIALAN, "");
                    form.siraNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.SIRANO, 0);
                    form.dosyaNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.DOSYANO, "");
                    form.evrakIcerigi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKICERIK, "");
                    form.evrakCinsi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKCINSI, 0);
                    form.esasNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ESASNO, "");
                    form.evrakBelgeTur = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKBELGETUR, 0);
                    form.imzaYolu = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMZAYOLU, 0);
                    form.bilgiEdinme = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.BILGIEDINME, 0);
                    form.kisiselBilgi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.KISISELBILGI, 0);
                    form.dosyasiVar = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.DOSYASIVAR, 0);
                    form.bakanlikEvrakNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.BAKANLIKEVRAKNO, "");
                    form.baskanlikEvrakNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.BASKANLIKEVRAKNO, "");
                    form.dagitimTarih = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.DAGITIMTARIH);
                    form.dinamikSayfaKod = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.DINAMIKSAYFAKOD, "");
                    form.dinamikSurecNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.DINAMIKSURECNO, "");
                    form.surecAd = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.SURECAD, "");
                    form.kayitIslem = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.KAYITISLEM, 0);
                    form.resmiBelge = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.RESMIBELGE, 0);
                    form.dil = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.DIL, 0);
                    form.eImzalandi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EIMZALANDI, 0);
                    form.eImzaTarihi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EIMZATARIHI);
                    form.eImzalayan = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EIMZALAYAN, "");
                    form.uretici = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.URETICI, "");
                    form.uretimTarihi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.URETIMTARIHI);
                    form.imzaBitti = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMZABITTI, 0);
                    form.uretimVersiyon = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.URETIMVERSIYON, "");
                    form.gizlilikGecerlilikSuresi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GIZLILIKGECERLILIKSURE);
                    form.iletimTarihi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ILETIMTARIHI);
                    form.arsivlemeTarihi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ARSIVLEMETARIHI);
                    form.transferTarihi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.TRANSFERTARIHI);
                    form.imhaTarihi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMHATARIHI);
                    form.ilgileriEkteGoster = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ILGILERIEKTEGOSTER, 0);
                    form.kepGonderimTarih = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.KEPGONDERIMTARIH);
                    form.editorGuncellenmeli = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EDITORGUNCELLENMELI, 0);
                    form.asilNushaGidecek = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ASILNUSHAGIDECEK, 0);

                    form.yaziDurumAd = VeriErisim.DegerAl(r, index + 1, "");
                    form.gizlilikAd = VeriErisim.DegerAl(r, index + 2, "");
                    form.gelisSekliAd = VeriErisim.DegerAl(r, index + 3, "");

                    //****Hareket bilgileri
                    form.hareket.siraNo = VeriErisim.DegerAl(r, index + 4, 0);
                    form.hareket.birimKod1 = VeriErisim.DegerAl(r, index + 5, "");
                    form.hareket.birimKod2 = VeriErisim.DegerAl(r, index + 6, "");
                    form.hareket.kisi1 = VeriErisim.DegerAl(r, index + 7, "");
                    form.hareket.kisi2 = VeriErisim.DegerAl(r, index + 8, "");
                    form.hareket.islemYapan = VeriErisim.DegerAl(r, index + 9, "");
                    form.hareket.islemTarih = VeriErisim.DegerAl(r, index + 10);
                    form.hareket.islem = VeriErisim.DegerAl(r, index + 11, 0);
                    form.hareket.kisi1Ad = VeriErisim.DegerAl(r, index + 12, "");
                    form.hareket.kisi2Ad = VeriErisim.DegerAl(r, index + 13, "");
                    form.hareket.islemYapanAd = VeriErisim.DegerAl(r, index + 14, "");
                    form.hareket.okundu = VeriErisim.DegerAl(r, index + 15, 0);
                    form.hareket.havaleNotu = VeriErisim.DegerAl(r, index + 16, "");
                    form.hareket.birimKod1Ad = VeriErisim.DegerAl(r, index + 17, "");
                    form.hareket.birimKod2Ad = VeriErisim.DegerAl(r, index + 18, "");
                    //****Hareket bilgileri

                    form.havaleBirimAdi = VeriErisim.DegerAl(r, index + 19, "");
                    form.prEvrakNo = VeriErisim.DegerAl(r, index + 20, "");
                    form.sicil.evrEvrakNo = form.evrEvrakNo;
                    form.sicil.prEvrakNo = form.prEvrakNo;
                    form.sicil.asil = VeriErisim.DegerAl(r, index + 21, 0);

                    //dosya kodlarını atama işlemi
                    //*********************************************
                    string dosyaKodlari = VeriErisim.DegerAl(r, index + 22, "");
                    string[] dler = dosyaKodlari.Split(';');
                    foreach (string d in dler)
                    {
                        DosyaPlani dosyaPlani = new DosyaPlani();
                        dosyaPlani.kod = d;
                        if (dosyaPlani.kod.Trim() != "")
                            form.dosyaKodlari.Add(dosyaPlani);
                    }
                    //*********************************************

                    //tasfiye planı işlemi
                    //*********************************************
                    if (tasfiyeyeBakilsin)
                    {
                        int tasfiyePlani = VeriErisim.DegerAl(r, index + 23, 0);
                        form.tasfiye.tasfiyeKod = tasfiyePlani;
                    }
                    //*********************************************

                    form.havaleyeGoreOlusanEvrakSayisi = VeriErisim.DegerAl(r, index + 24, 0);

                    if (tekBelgeListele)
                    {
                        form.dosyaKodlari = EVREvrakDosyaPlani.Oku(ve2, null, form.prEvrakNo);

                        if (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                            form.evrakTuruAd = EVRTamimlama.Listele(ve2, form.evrakTuru, ENUMTamimlamaFormTur.EVRGIDENTUR);

                        if (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                            form.evrakIlgi = EVREvrakIlgi.Listele(ve2, form.prEvrakNo);

                        if (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                            form.dagitim = EVREvrakDagitim.Listele(ve2, form.prEvrakNo);

                        //form.saklamaPlanlari = EVRSaklama.Listele(ve2,form.prEvrakNo);                      
                    }

                    if (form.gelisSekli == -1) form.gelisSekli = 0;
                    if (form.gizlilik == -1) form.gizlilik = 0;
                    if (form.evrakCinsi == -1) form.evrakCinsi = 0;


                    bool evrakEkle = true;

                    if (form.gizlilik > 0 && !gizliEvrakiOku)  //Evrakın gizlilik durumuna göre, kullanıcın gizlilik yetkisine bakarak evrak elenecek
                    {
                        bool evrakKendisindeMi = (kullanan.mernis == form.hareket.kisi1 || kullanan.mernis == form.hareket.kisi2 || kullanan.mernis == form.hareket.islemYapan);
                        bool evrakUretisiMi = (form.uretici == kullanan.ad + " " + kullanan.soyad || form.uretici == kullanan.mernis);

                        if (!evrakKendisindeMi && !evrakUretisiMi)
                        {
                            int gizlilikDerecesi = OrtakFonksiyonlar.ConvertToInt((kullanan.yetkiTipiDegerleri["Gizlilik Derecesi"] + "").TrimEnd(';'), 0);

                            if (form.gizlilik == (int)ENUMGizlilik.COKGIZLI)
                            {
                                if (gizlilikDerecesi != (int)ENUMGizlilik.COKGIZLI)
                                    evrakEkle = false; //Göremez
                            }
                            else if (form.gizlilik == (int)ENUMGizlilik.GIZLI)
                            {
                                if (!(gizlilikDerecesi == (int)ENUMGizlilik.GIZLI || gizlilikDerecesi == (int)ENUMGizlilik.COKGIZLI))
                                    evrakEkle = false;//Göremez
                            }
                        }
                    }


                    if (!evrakEkle)
                    {
                        if (EVREvrak.YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu.EvrakGizliEvraklarGorunmesin))
                            continue;

                        EvrakSinif form2 = new EvrakSinif();
                        form2.gizliEvrakIslemi = true;
                        form2.evrEvrakNo = form.evrEvrakNo;
                        form2.prEvrakNo = form.prEvrakNo;
                        form2.geldigiYer = form.geldigiYer;
                        form2.konu = "Bu evrakı görme yetkiniz yok! (Gizli Evrak)";

                        form2.evrakTipi = form.evrakTipi;
                        form2.evrakTarih = form.evrakTarih;
                        form2.evrakNo = form.evrakNo;
                        form2.sicil.evrEvrakNo = form.sicil.evrEvrakNo;
                        form2.sicil.prEvrakNo = form.sicil.prEvrakNo;
                        form2.sicil.asil = form.sicil.asil;

                        form = form2;
                    }

                    hVAN.objeler.Add(form);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();

            if (to.Hata() != "")
            {
                hVAN.sonuc.islemSonuc = false;
                hVAN.sonuc.hataStr = to.Hata();
            }
            //else
            //    hVAN.sonuc.anahtar = sayac.ToString();

            return hVAN;
        }

        public static string[] ListelePrEvrakNo(VeriErisim veriErisim, EvrakSinif form)
        {
            ObjectArray hVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            IDataReader r = to.Listele(form, true);

            string bulunduguYer = form.bulunduguYer;
            if (r != null)
            {
                string eskiPrEvrakNo = "";
                while (r.Read())
                {
                    string prEvrakNo = VeriErisim.DegerAl(r, 0, "");
                    if (eskiPrEvrakNo == prEvrakNo)
                        continue;

                    eskiPrEvrakNo = prEvrakNo;
                    hVAN.objeler.Add(prEvrakNo);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            string[] prEvrakNoList = null;
            if (hVAN.objeler.Count > 0)
                prEvrakNoList = (string[])hVAN.objeler.ToArray(typeof(string));

            return prEvrakNoList;
        }

        /// <summary>
        /// Parametre olarak verilen kullanıcının evrak üzerinde işlem yapma yetkisini kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <param name="islem">Evrak hareket durumunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc IslemYapmaYetkisiVarMi(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form, ENUMEvrakHareketDurum islem)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string bulunduguYer = form.bulunduguYer;

            ObjectArray evraklar = Listele(veriErisim, kullanan, form);
            if (evraklar.objeler.Count > 0)
            {
                form = (EvrakSinif)evraklar.objeler[0];
                form.bulunduguYer = bulunduguYer;//bulundugu yer bilgisi veri tabanında tutulmuyor

                if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, form.bulunduguYer) || form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                    form.hareket.kisi1 = kullanan.mernis;


                bool uygun = false;

                //İptal işleminde evrak geçiş hareketine bakılmasın.
                if (islem == ENUMEvrakHareketDurum.IPTALEDILDI) // && form.imzaBitti != 1)
                    uygun = true;
                else
                    uygun = EvrakHareketGecisiUygunMu(form.hareket.islem, (int)islem);

                if (uygun)
                {
                    //İmzaya gönder durumu ve imzaya gönderileni geri alma durumunda gözden geçirelecek
                    if (islem == ENUMEvrakHareketDurum.TESLIMALDI || islem == ENUMEvrakHareketDurum.IADEEDILDI || islem == ENUMEvrakHareketDurum.HAVALEGERIALINDIGIDEN)
                        uygun = EvrakKendiBirimindeMi(veriErisim, kullanan, form, true);
                    else
                        uygun = EvrakKendiBirimindeMi(veriErisim, kullanan, form, false);

                    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI) ||
                        kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKBELGEYONETICISI)) //Belge Yöneticisi yetkisine sahip kullanıcılar başkasının dosyaya kaldırdığı/iptal ettiği vb. evrak üzerinde işlem yapabilsin.Melih 11.02.2014
                        uygun = true;

                    if (!uygun)
                    {
                        if (evraklar.ObjeSayisi > 0)
                        {
                            sonuc.hataStr += string.Format(EvrakI.EVRI058, form.evrakBirimKodu, form.evrakNo) + "<br>";
                        }
                        else
                            sonuc.hataStr += string.Format(EvrakI.EVRI059, form.evrEvrakNo) + "<br>";
                    }
                }
                else
                    sonuc.hataStr = "Evrak hareket geçişi uygun değil<br>"; //EvrakI.EVRI060 + "<br>";

                if (sonuc.hataStr == "")
                    sonuc.islemSonuc = true;
                else
                    sonuc.islemSonuc = false;
            }
            else
                sonuc.hataStr = EvrakI.EVRI060 + "<br>";

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verien bilgilere göre, kullanıcının evrak'a erişim yetkisini kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <returns>Evrak'a erişim yetkisi varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool GormeYetkisiVarMi(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            if (TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, form.bulunduguYer))
                return true;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);

            bool yetkili = true;

            if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, form.bulunduguYer) || form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                form.hareket.kisi1 = kullanan.mernis;

            yetkili = EvrakBirimeKisiyeUgradiMi(null, form);//Evrak Kullanıcı ise evrak ondamı diye kontrol edilmesi gerek

            if (!yetkili)
            {
                string dosyaTasnifPlani = kullanan.yetkiTipiDegerleri["Dosya Tasnif Planı"];
                if (!string.IsNullOrEmpty(dosyaTasnifPlani))
                {
                    bool mevcut = to.EvrakDosyaTasnifPlaniMevcutMu(form.prEvrakNo, dosyaTasnifPlani);
                    if (mevcut)
                        yetkili = true;
                }
            }

            if (!yetkili)
            {
                int hareket = EvrakSonHareket(null, form.prEvrakNo);
                if (hareket == (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR)
                    yetkili = true;
            }

            ve.VeriErisimKapat();

            return yetkili;
        }

        /// <summary>
        /// Evrak'ın kullanıcının kendi biriminde olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <param name="teslimAlmaIslemi">Evrak teslim alma bilgisini içeren nesne.</param>
        /// <returns>Evrak kullanıcının kendi biriminde ise doğru, yoksa yanlış döndürülür.<</returns>
        public static bool EvrakKendiBirimindeMi(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form, bool teslimAlmaIslemi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrak to = new TABEvrak(ve);

            bool altBirimleriDahilEt = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKBELGEYONETICISI);

            bool deger = to.EvrakKendiBirimindeMi(form, teslimAlmaIslemi, altBirimleriDahilEt);

            ve.VeriErisimKapat();

            return deger;
        }

        /// <summary>
        /// Parametre olarak verilen evrak'ın son hareket durumunu veritabanından okuyan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisini içeren nesne</param>
        /// <returns>Evrak harket durumunu int türünde döndürülür.</returns>
        public static int EvrakSonHareket(VeriErisim veriErisim, string prEvrakNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrak to = new TABEvrak(ve);

            int hareket = to.EvrakSonHareket(prEvrakNo);

            ve.VeriErisimKapat();

            return hareket;
        }

        /// <summary>
        /// Parametre olarak verilen evrak'ın birim veya kişiye uğrayıp uğramadığını kontrol eden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <returns>Evrak birim veya kişiye uğradıysa doğru, yoksa yanlış döndürülür.</returns>
        public static bool EvrakBirimeKisiyeUgradiMi(VeriErisim veriErisim, EvrakSinif form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrak to = new TABEvrak(ve);

            bool deger = to.EvrakBirimeKisiyeUgradiMi(form);

            ve.VeriErisimKapat();

            return deger;
        }


        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların iptal işlemini gerçekleştiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc IptalEt(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);
            int basarili = 0;

            foreach (EvrakSinif evrak in liste.objeler)
            {
                //Dağıtım ile gönerilen evrakların iptal edilebilmesi için ********************************************
                if (evrak.evrakNo == 0 && evrak.hareket.islem == (int)ENUMEvrakHareketDurum.DAGITIMKAYIT)
                {
                    string bulunduguYer = evrak.bulunduguYer;
                    evrak.bulunduguYer = "";
                    ObjectArray evraklar = Listele(veriErisim, kullanan, evrak);
                    if (evraklar.objeler.Count > 0)
                    {
                        if (((EvrakSinif)evraklar.objeler[0]).hareket.birimKod2 == bulunduguYer)
                        {
                            evrak.bulunduguYer = bulunduguYer;
                            sonuc.islemSonuc = true;
                            sonuc.hataStr = "";
                        }
                    }
                }
                else
                {
                    if (evrak.eImzalandi != 1 || (TNS.EVR.Yetki.KullaniciEvrakYetkiSablonu(kullanan, evrak.bulunduguYer) == (int)ENUMEvrakYetkiSablonu.EVRAKYETKILISI ||
                                   kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKBELGEYONETICISI) ||
                                   kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI)) ||
                        EVREvrak.IslemSonHareketGetir(veriErisim, evrak.prEvrakNo, ENUMEvrakHareketDurum.EIMZALANDI, ENUMEvrakHareketDurum.EPARAFLANDI).islemYapan == kullanan.mernis
                        )
                        sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, ENUMEvrakHareketDurum.IPTALEDILDI);
                    else
                        sonuc.DegerAta("Elektronik olarak imzalanmış evrakın iptal işlemi <b>evrak yetkilisi</b> veya <b>birim/kurum belge yöneticisi</b> tarafından yapılabilir.");
                }
                //*******************************************************************************************************

                if (sonuc.islemSonuc)
                {
                    bool surecIslemYap = false;

                    if (!string.IsNullOrEmpty(evrak.dinamikSurecNo))
                    {
                        EvrakHareket yeniHareketi = new EvrakHareket();
                        yeniHareketi.prEvrakNo = evrak.prEvrakNo;
                        yeniHareketi.islem = (int)ENUMEvrakHareketDurum.YENI;
                        ObjectArray listeHareket = EVREvrakHareket.Listele(ve, yeniHareketi);
                        foreach (EvrakHareket h in listeHareket.objeler)
                            yeniHareketi = h;

                        if (yeniHareketi.kisi1 != kullanan.mernis)
                            sonuc.DegerAta("Evrak hazırlayan kişi tarafından iptal edilebilir.");
                        else
                            surecIslemYap = true;
                    }

                    if (sonuc.islemSonuc)
                    {
                        EvrakHareket hareket = new EvrakHareket();
                        hareket.prEvrakNo = evrak.prEvrakNo;
                        hareket.birimKod1 = evrak.bulunduguYer;
                        hareket.islem = (int)ENUMEvrakHareketDurum.IPTALEDILDI;
                        if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, evrak.bulunduguYer) || evrak.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                            hareket.kisi1 = kullanan.mernis;
                        hareket.havaleNotu = evrak.hareket.havaleNotu;
                        hareket.vekil = evrak.hareket.vekil;
                        hareket.vekilAd = evrak.hareket.vekilAd;

                        sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                        if (sonuc.islemSonuc)
                        {
                            basarili++;

                            if (surecIslemYap)
                            {
                                try
                                {
                                    //Süreç İptal Et
                                    WCFClient wcf = new WCFClient();
                                    UserInfo userInfo = TNS.EVR.Arac.LoginCentraCube("", kullanan.kullaniciKodu, kullanan.parola, "");

                                    wcf.DeleteInstance(Guid.Parse(evrak.dinamikSurecNo), userInfo.TokenId);

                                    //WCFClient wcf = new WCFClient();
                                    //Dictionary<string, object> inputs = new Dictionary<string, object>();
                                    //inputs.Add("Result", 2);
                                    //string msg = wcf.RunWorkFlowByWorkflowInstanceID(kullanan.mernis, kullanan.kullaniciKodu, Guid.Parse(evrak.dinamikSurecNo), inputs);
                                    //if (!string.IsNullOrEmpty(msg))
                                    //    sonuc.DegerAta(msg);
                                }
                                catch (Exception ex)
                                {
                                    sonuc.DegerAta("Süreç İptal : " + ex.Message);
                                    Arac.HataStrYaz("{42782FF6-4227-4709-A4D4-2F9F511C41CD} - Süreç İptal Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                                }
                            }
                        }

                    }
                }
            }

            if (sonuc.hataStr == "")
                sonuc.islemSonuc = true;
            else
                sonuc.islemSonuc = false;

            sonuc.anahtar = basarili.ToString();
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların iptal işleminin geri alınmasını gerçekleştiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc IptalGeriAl(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);
            int basarili = 0;

            foreach (EvrakSinif evrak in liste.objeler)
            {
                sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, ENUMEvrakHareketDurum.IPTALGERIALINDI);

                if (sonuc.islemSonuc)
                {
                    ObjectArray evraklar = Listele(ve, kullanan, new EvrakSinif() { prEvrakNo = evrak.prEvrakNo, bulunduguYer = evrak.bulunduguYer });
                    if (evraklar.ObjeSayisi > 0)
                    {
                        EvrakSinif form = (EvrakSinif)evraklar[0];

                        //Dağıtım ile gelen sıfır numaralı evrak iptal edilir daha sonrada geri alınmak istenirse sistem numara vermeli veya iptal geri alınmamamlı (0 numaralı evrak için)
                        if (form.evrakNo == 0 && form.evrakTipi == (int)ENUMEvrakTipi.GELEN)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = "0 numaralı evrak iptalden gerli alınamaz.";
                            //sonuc = GelenEvrakNumaraVer(ve, kullanan, evraklar);
                        }
                        if (sonuc.islemSonuc)
                        {
                            EvrakHareket hareket = new EvrakHareket();
                            hareket.prEvrakNo = evrak.prEvrakNo;
                            hareket.birimKod1 = evrak.bulunduguYer;
                            hareket.islem = (int)ENUMEvrakHareketDurum.IPTALGERIALINDI;
                            if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, evrak.bulunduguYer) || evrak.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                                hareket.kisi1 = kullanan.mernis;

                            sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                            if (sonuc.islemSonuc)
                                basarili++;
                        }
                    }
                    else
                        sonuc.hataStr = "Evrak bilgisi alınamadı";

                }
            }

            if (sonuc.hataStr == "")
                sonuc.islemSonuc = true;
            else
                sonuc.islemSonuc = false;

            sonuc.anahtar = basarili.ToString();
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların havale işlemini gerçekleştiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgilerini içeren nesne</param>
        /// <param name="yetkiKontrol">Yetki kontrolünün yapılıp yapılmaması durumunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc HavaleEt(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, bool yetkiKontrol)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            //TABEvrak tab = new TABEvrak(ve);
            int basariliEvrak = 0;
            int basariliKullanici = 0;
            string hata = "";

            foreach (EvrakSinif evrak in liste.objeler)
            {
                if (yetkiKontrol)
                {
                    if (evrak.evrakTipi == (int)ENUMEvrakTipi.GELEN)
                        sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, ENUMEvrakHareketDurum.HAVALEEDILDI);
                    else
                        sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN);
                }
                else
                    sonuc.islemSonuc = true;


                if (sonuc.islemSonuc)
                {
                    //evrak.hareket dolu olarak geliyor. birim2 ve kisi2 bilgileride doldurulmalı
                    int sayac = 0;
                    foreach (EvrakHareket h in evrak.hareketHavale)
                    {
                        //Personel birim bilgisini kontrol et
                        if (sonuc.islemSonuc && !string.IsNullOrWhiteSpace(h.kisi2))
                        {
                            //Personel aktif mi kontrol et
                            bool aktifMi = EVRGenel.KullaniciAktifMi(h.kisi2);
                            if (!aktifMi)
                                continue;

                            bool birimVar = false;
                            string[] birimler = YetkiliEvrakBirimleriniGetir(h.kisi2);

                            if (birimler != null)
                                foreach (var birimKod in birimler)
                                {
                                    if (birimKod == h.birimKod2) { birimVar = true; break; }
                                }

                            if (!birimVar)
                            {
                                string birimAd = EVRGenel.EvrakBirimAdGetir(h.birimKod2);
                                string kisiAd = EVRGenel.KullaniciAdGetir(h.kisi2);

                                sonuc.islemSonuc = false;
                                sonuc.hataStr += "<font color=\"red\"><b>" + kisiAd + "</b></font> kullanıcısının <font color=\"red\"><b>" + birimAd + "</b></font> birimden yetkisi kaldırılmış.<br>Lütfen havale işlemini kullanıcının yetkili olduğu birime yapınız.<br>";
                            }
                        }

                        //Personel izin durumunu kontrol et
                        if (sonuc.islemSonuc)
                        {
                            string havaleEdilecekKisi = h.kisi2;
                            if (!string.IsNullOrEmpty(havaleEdilecekKisi))
                                sonuc = HavaleIcinPersonelIzinDurumunuKontrolEt(ve, kullanan, havaleEdilecekKisi);
                        }


                        if (sonuc.islemSonuc)
                        {
                            if (string.IsNullOrEmpty(h.birimKod2))
                            {
                                sonuc.islemSonuc = false;
                                sonuc.hataStr += string.Format(EvrakI.EVRI061 + "<br>", evrak.evrakNo.ToString());
                            }
                        }

                        if (sonuc.islemSonuc)
                        {
                            evrak.hareket = h;

                            if (evrak.hareket.havaleTur == (int)ENUMEvrakHavaleTur.GEREGI)//İlk evrak ve Gereği //İlk değilse aşağıdaki if'e girecek
                                sayac++;//0 dan başlıyor ilk gereği havale dongusunda 1 olur ve alttaki if e girmez

                            if (evrak.hareket.havaleTur == (int)ENUMEvrakHavaleTur.BILGI || sayac > 1)//Bilgi için ise veya birden fazla yere gönderiliyor ise
                            {
                                evrak.dosyaKodlari = EVREvrakDosyaPlani.Oku(ve, kullanan, evrak.prEvrakNo);

                                evrak.hareket.prEvrakNo = PrEvrakNoAl(ve);
                                evrak.prEvrakNo = evrak.hareket.prEvrakNo;//Mail atarken ihtiyaç var
                                EvrakSicil es = new EvrakSicil();
                                es.evrEvrakNo = evrak.evrEvrakNo;
                                es.prEvrakNo = evrak.hareket.prEvrakNo;
                                if (evrak.hareket.havaleTur == (int)ENUMEvrakHavaleTur.BILGI)
                                    es.asil = 0;//Havale ekranında dolduruluyor. Gereği için gönderildiğinde 1 bilgi için 0 oluyor
                                else
                                    es.asil = 1;

                                sonuc = EVREvrak.KaydetSicil(ve, kullanan, es);

                                if (sonuc.islemSonuc)
                                    sonuc = EVREvrakDosyaPlani.Kaydet(ve, kullanan, evrak.hareket.prEvrakNo, evrak.dosyaKodlari, false, null);
                            }
                        }

                        if (sonuc.islemSonuc)
                        {
                            if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, evrak.bulunduguYer) || evrak.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                                evrak.hareket.kisi1 = kullanan.mernis;
                            else
                                evrak.hareket.kisi1 = "";

                            if (evrak.evrakTipi == (int)ENUMEvrakTipi.GIDEN)//İmza için gönderildi ise otomatik teslim alınması için
                            {
                                bool evrakImzaliMi = !evrak.imzaTarihi.isNull;
                                if (!evrakImzaliMi)
                                    evrakImzaliMi = EvrakImzaliMi(ve, kullanan, evrak.evrEvrakNo);

                                if (evrakImzaliMi)
                                {
                                    string hatamesaj = "Evrak <b>havale</b> işlemi için uygun değildir. <br>Paraf / imza işlemi başlatılmış ve <font color=\"red\"><b>tamamlanmamış</b></font> bir evrak havale işlemi ile gönderilemez.<br>";
                                    if (evrak.imzaYolu > 0)
                                    {
                                        EvrakImzaYolu evrakImzaYolu = EVREvrakImzaYolu.Oku(ve, kullanan, new EvrakImzaYolu() { prEvrakNo = evrak.prEvrakNo });
                                        if (evrakImzaYolu != null || evrakImzaYolu.imzaYolu.kod > 0)
                                        {
                                            ImzaYolu imzaYolu = evrakImzaYolu.imzaYolu;
                                            if (imzaYolu.detaylar != null && imzaYolu.detaylar.Length > 0)
                                            {
                                                string evrakSahibiTcKimlikNo = imzaYolu.detaylar[0].tcKimlikNo;
                                                if (kullanan.mernis != evrakSahibiTcKimlikNo)
                                                {
                                                    sonuc.islemSonuc = false;
                                                    sonuc.hataStr += hatamesaj;
                                                }
                                            }
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(evrak.dinamikSurecNo))
                                    {
                                        sonuc.islemSonuc = false;
                                        sonuc.hataStr += hatamesaj;
                                    }
                                }

                                if (sonuc.islemSonuc)
                                {
                                    EvrakHareket hareket = new EvrakHareket();
                                    hareket.prEvrakNo = evrak.prEvrakNo;
                                    hareket.birimKod1 = evrak.hareket.birimKod2;
                                    hareket.birimKod2 = evrak.hareket.birimKod1;
                                    hareket.islem = (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN;
                                    //hareket.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
                                    hareket.kisi1 = evrak.hareket.kisi2;
                                    hareket.kisi2 = kullanan.mernis;
                                    hareket.havaleNotu = evrak.hareket.havaleNotu;
                                    hareket.vekil = evrak.hareket.vekil;
                                    hareket.vekilAd = evrak.hareket.vekilAd;

                                    evrak.hareket = hareket;
                                }

                            }


                            if (sonuc.islemSonuc)
                                sonuc = EVREvrakHareket.Kaydet(ve, kullanan, evrak.hareket);

                            if (sonuc.islemSonuc)
                                sonuc = EVREvrakSonraHavaleEt.GonderildiOlarakIsaretle(ve, kullanan, evrak.prEvrakNo);

                            if (sonuc.islemSonuc)
                            {
                                //Kullanıcı ayarlarına seçenek eklenecek
                                //if (evrak.evrakTipi == (int)ENUMEvrakTipi.GELEN && evrak.hareket.islem == 51)
                                //    OtomatikTeslimAl(ve, kullanan, evrak);

                                string postaKisi = "";
                                string postaYer = "";
                                int postaTur = 0;

                                if (evrak.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                                {
                                    postaTur = (int)ENUMPOSTATIP.HAVALEGIDEN;
                                    postaKisi = evrak.hareket.kisi1;
                                }
                                else if (evrak.evrakTipi == (int)ENUMEvrakTipi.GELEN)
                                {
                                    postaTur = (int)ENUMPOSTATIP.HAVALE;
                                    if (string.IsNullOrEmpty(evrak.hareket.kisi2))
                                        postaYer = evrak.hareket.birimKod2;
                                    else//Kişiye gönderildi
                                        postaKisi = evrak.hareket.kisi2;
                                }

                                OrtakFonksiyonlar.HataStrYaz("{821BD478-6BC4-4313-8089-1C2A5AAEC0E8} - YeniEvrakGeldiPostasiHazirla; postaKisi: " + postaKisi + ", postaYer: " + postaYer + ", postaTur:" + postaTur + ", evrEvrakNo:" + evrak.evrEvrakNo);

                                EVRPosta.YeniEvrakGeldiPostasiHazirla(ve, kullanan, evrak.evrEvrakNo, evrak.prEvrakNo, postaKisi, postaYer, postaTur);
                            }

                            if (sonuc.islemSonuc && evrak.parametre.ContainsKey(ENUMEvrakSinifParemetre.ISTALEPKAYDET) && (bool)evrak.parametre[ENUMEvrakSinifParemetre.ISTALEPKAYDET])
                            {
                                Sonuc sonucIsTalep = EVREvrak.IsTalepKaydet(ve, kullanan, evrak);
                                if (!sonucIsTalep.islemSonuc)
                                    hata += sonucIsTalep.hataStr + "<br>";
                            }
                        }

                        if (sonuc.islemSonuc)
                            basariliKullanici++;
                        else
                            hata += sonuc.hataStr + "<br>";

                        sonuc.islemSonuc = true;
                        sonuc.hataStr = "";
                    }
                }


                if (sonuc.islemSonuc)
                    basariliEvrak++;
                else
                    hata += sonuc.hataStr;

                sonuc.islemSonuc = true;
                sonuc.hataStr = "";
            }



            if (hata == "")
                sonuc.islemSonuc = true;
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
            }

            sonuc.anahtar = basariliEvrak.ToString();
            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc HavaleIcinPersonelIzinDurumunuKontrolEt(VeriErisim veriErisim, Kullanici kullanan, string havaleEdilecekKisi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            string kontrolTur = System.Configuration.ConfigurationManager.AppSettings.Get("EvrakHavaleIcinPersonelIzinKontrolDurumu") + "";
            if (kontrolTur == "1")
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                TABEvrak to = new TABEvrak(ve);

                try
                {
                    bool izindeMi = to.HavalePersonelIzinKontrol(havaleEdilecekKisi, kontrolTur);

                    if (!string.IsNullOrEmpty(to.Hata()))
                    {
                        sonuc.hataStr = to.Hata();
                        sonuc.islemSonuc = false;
                    }
                    else if (izindeMi)
                    {
                        string kisiAd = EVRGenel.KullaniciAdGetir(havaleEdilecekKisi);

                        sonuc.hataStr = kisiAd + " isimli personel İzinde ve/veya Görevde olduğudan evrak havalesi yapılamaz.";
                        sonuc.islemSonuc = false;
                    }
                }
                catch (Exception ex)
                {
                    sonuc.hataStr = ex.Message;
                    sonuc.islemSonuc = false;
                }

                ve.VeriErisimKapat();
            }
            else
            {
                sonuc.bilgiStr = "İzin kontrolü yapılmadı";
                sonuc.islemSonuc = true;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak'ın otomatik olarak teslim alınmasını sağlayan yordam.
        /// </summary>
        /// <param name="ve">Veri erişim bilgisini içeren nesne</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="evrak">Evrak bilgisini içeren nesne</param>
        private static void OtomatikTeslimAl(VeriErisim ve, Kullanici kullanan, EvrakSinif evrak)
        {
            EvrakHareket hareket = new EvrakHareket();
            hareket.prEvrakNo = evrak.prEvrakNo;
            hareket.birimKod1 = evrak.hareket.birimKod2;
            hareket.birimKod2 = evrak.hareket.birimKod1;
            hareket.islem = (int)ENUMEvrakHareketDurum.TESLIMALDI;

            //if (TNS.EVR.Yetki.KullaniciEvrakYetkiSablonu(kullanan) != (int)ENUMEvrakYetkiSablonu.EVRAKMEMURU) //Evrak Memuru kendisine havale edilse bile diğer memurlarında görsün. İzin vb. durumlarda işlerin aksamaması için //17.02.2014 kaldırıldı Melih TBMM talep eskisi gibi olsun
            hareket.kisi1 = evrak.hareket.kisi2;
            hareket.kisi2 = evrak.hareket.kisi1;
            hareket.okundu = 0;//Okunmadı
            if (!string.IsNullOrEmpty(hareket.kisi1))
            {
                hareket.islemYapan = hareket.kisi1;
                hareket.islemYapanAd = kullanan.ad + " " + kullanan.soyad;//Melih değiştirdi //EVRServis.UzayYaziDegerGetir("KULLANICIAD", hareket.kisi1, true, "");
            }
            else
                hareket.islemYapanAd = kullanan.ad + " " + kullanan.soyad;//Melih değiştirdi //EvrakI.EVRI062; //Birimde          
            EVREvrakHareket.Kaydet(ve, kullanan, hareket);
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların havale işleminin geri alınmasını gerçekleştiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc HavaleGeriAl(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);
            int basarili = 0;

            foreach (EvrakSinif evrak in liste.objeler)
            {
                ENUMEvrakHareketDurum hareketIslem = (evrak.evrakTipi == (int)ENUMEvrakTipi.GELEN ? ENUMEvrakHareketDurum.HAVALEGERIALINDI : ENUMEvrakHareketDurum.HAVALEGERIALINDIGIDEN);

                sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, hareketIslem);

                if (sonuc.islemSonuc)
                {
                    EvrakHareket hareket = new EvrakHareket();
                    hareket.prEvrakNo = evrak.prEvrakNo;
                    hareket.birimKod1 = evrak.bulunduguYer;
                    hareket.islem = (int)hareketIslem;
                    if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, evrak.bulunduguYer) || evrak.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                        hareket.kisi1 = kullanan.mernis;

                    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);

                    if (sonuc.islemSonuc)
                        basarili++;
                }
            }

            if (sonuc.hataStr == "")
                sonuc.islemSonuc = true;
            else
                sonuc.islemSonuc = false;

            sonuc.anahtar = basarili.ToString();
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların havale geri gönderme işlemini gerçekleştiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>

        public static Sonuc HavaleGeriGonder(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            int basarili = 0;

            foreach (EvrakSinif evrak in liste.objeler)
            {
                sonuc.islemSonuc = EvrakHareketGecisiUygunMu(evrak.hareket.islem, (int)ENUMEvrakHareketDurum.IADEEDILDI);

                if (!sonuc.islemSonuc)
                    sonuc.hataStr += string.Format(EvrakI.EVRI063, evrak.evrakNo.ToString());
                else
                {
                    EvrakSinif bilgi = new EvrakSinif();
                    bilgi.prEvrakNo = evrak.prEvrakNo;
                    bilgi.bulunduguYer = evrak.bulunduguYer;
                    bilgi.hareket.islem = (int)ENUMEvrakTarihceDurum.HAVALEGELEN;
                    ObjectArray evraklar = Listele(ve, kullanan, bilgi);

                    if (evraklar.ObjeSayisi > 0)
                    {
                        bool kontrol1 = BilgiIcinGelenEvrakGeriGonderilebilsinMi(evrak.bulunduguYer);

                        bilgi = (EvrakSinif)evraklar.objeler[0];

                        if (bilgi.hareket.birimKod2 == evrak.bulunduguYer &&
                            bilgi.hareket.islem == (int)ENUMEvrakHareketDurum.HAVALEEDILDI &&
                            (kontrol1 || bilgi.sicil.asil == 1))//Sadece gereği için gönderilenler geri gönderilir.
                        {
                            //Hareket yaz
                            EvrakHareket hareket = new EvrakHareket();
                            hareket.prEvrakNo = bilgi.prEvrakNo;
                            hareket.birimKod1 = bilgi.hareket.birimKod1;
                            hareket.birimKod2 = bilgi.hareket.birimKod2;
                            hareket.islem = (int)ENUMEvrakHareketDurum.IADEEDILDI;
                            hareket.okundu = 1;

                            hareket.kisi1 = bilgi.hareket.kisi1;
                            hareket.havaleNotu = evrak.hareket.havaleNotu;

                            sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);

                            if (!sonuc.islemSonuc)
                            {
                                sonuc.hataStr = string.Format(EvrakI.EVRI064, bilgi.evrakBirimKodu, bilgi.evrakNo) + "<br>" + sonuc.hataStr;
                                ve.VeriErisimKapat();
                                return sonuc;
                            }
                            basarili++;
                        }
                        else
                            sonuc.hataStr += string.Format(EvrakI.EVRI065, bilgi.evrakBirimKodu, bilgi.evrakNo) + "<br>";
                    }
                    else
                        sonuc.hataStr += string.Format(EvrakI.EVRI059, bilgi.evrEvrakNo) + "<br>";
                }
            }

            if (sonuc.hataStr == "")
                sonuc.islemSonuc = true;
            else
                sonuc.islemSonuc = false;

            sonuc.anahtar = basarili.ToString();
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların kullanıcı tarafından teslim alma işlemini gerçekleştiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc TeslimAl(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            int basarili = 0;

            foreach (EvrakSinif evrak in liste.objeler)
            {
                if (evrak.gizliEvrakIslemi) //Gizli evraklar teslim alınabilsin
                    sonuc.islemSonuc = true;
                else
                    sonuc.islemSonuc = EvrakHareketGecisiUygunMu(evrak.hareket.islem, (int)ENUMEvrakHareketDurum.TESLIMALDI);

                if (!sonuc.islemSonuc)
                    sonuc.hataStr += string.Format(EvrakI.EVRI067, evrak.evrakNo.ToString());
                else
                {
                    EvrakSinif bilgi = new EvrakSinif();
                    bilgi.prEvrakNo = evrak.prEvrakNo;
                    bilgi.bulunduguYer = evrak.bulunduguYer;
                    bilgi.hareket.islem = (int)ENUMEvrakTarihceDurum.HAVALEGELEN;

                    ObjectArray evraklar = Listele(ve, kullanan, bilgi, 0, 0, false, true);

                    if (evraklar.ObjeSayisi > 0)
                    {
                        bilgi = (EvrakSinif)evraklar.objeler[0];
                        if (bilgi.hareket.birimKod2 == evrak.bulunduguYer && (bilgi.hareket.islem == (int)ENUMEvrakHareketDurum.HAVALEEDILDI || bilgi.hareket.islem == (int)ENUMEvrakHareketDurum.DAGITIMKAYIT))
                        {
                            //Hareket yaz
                            EvrakHareket hareket = new EvrakHareket();
                            hareket.prEvrakNo = bilgi.prEvrakNo;
                            hareket.birimKod1 = bilgi.hareket.birimKod2;
                            hareket.birimKod2 = bilgi.hareket.birimKod1;
                            hareket.islem = (int)ENUMEvrakHareketDurum.TESLIMALDI;
                            hareket.okundu = 0;//Okunmadı
                            hareket.havaleNotu = bilgi.hareket.havaleNotu;
                            hareket.vekil = evrak.hareket.vekil;
                            hareket.vekilAd = evrak.hareket.vekilAd;

                            //if (TNS.EVR.Yetki.KullaniciEvrakYetkiSablonu(kullanan) != (int)ENUMEvrakYetkiSablonu.EVRAKMEMURU || //!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan) 11.02.2014 Melih Aynı evrak biriminde olan yetkililer evrakı teslim aldıkların diğerleri aynı evrakı görmesin. Sadece evrak memur ise diğer memur yetkisindekiler görebilsin
                            //    bilgi.evrakTipi == (int)ENUMEvrakTipi.GIDEN ||
                            //    (bilgi.hareket.kisi2 == kullanan.mernis && hareket.birimKod1 == hareket.birimKod2))//Aynı birimdeki evrak yetkilileri havale ettiğinde diğeri teslim alsa bile diğeri görüyor
                            //Sadece EvrakMemuru teslim aldığında diğer kişilerinde aynı evrak üzerinde işlem yapması istenmiyor. TBMM 17.02.2014 Melih


                            //Evrak yetkilisi alırsa sadece kendisi görsün. Memur alırsa diğer memurlarda görsün. Hsyn-15:40 25.02.2014
                            //kisi2 boş ise evrak birime gönderilmiştir.
                            if ((bilgi.hareket.kisi2 != "") || TNS.EVR.Yetki.KullaniciEvrakYetkiSablonu(kullanan, evrak.bulunduguYer) != (int)ENUMEvrakYetkiSablonu.EVRAKMEMURU)
                                hareket.kisi1 = kullanan.mernis;
                            hareket.kisi2 = bilgi.hareket.kisi1;

                            sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);

                            if (!sonuc.islemSonuc)
                            {
                                sonuc.hataStr = string.Format(EvrakI.EVRI068, bilgi.evrakBirimKodu, bilgi.evrakNo) + "<br>" + sonuc.hataStr;
                                ve.VeriErisimKapat();
                                return sonuc;
                            }
                            else
                                GelenEvrakNumaraVer(ve, kullanan, evraklar);
                            basarili++;
                        }
                        else
                            sonuc.hataStr += string.Format(EvrakI.EVRI069, bilgi.evrakBirimKodu, bilgi.evrakNo) + "<br>";
                    }
                    else
                        sonuc.hataStr += string.Format(EvrakI.EVRI059, bilgi.evrEvrakNo) + "<br>";
                }
            }

            if (sonuc.hataStr == "")
                sonuc.islemSonuc = true;
            else
                sonuc.islemSonuc = false;

            sonuc.anahtar = basarili.ToString();
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların kullanıcı tarafından dosyaya kaldırma işlemini gerçekleştiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc DosyayaKaldir(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI071, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tabEvrak = new TABEvrak(ve);

            string dagitimBilgi = "";

            foreach (EvrakSinif form in liste.objeler)
            {
                //numara almayan evraklarda dosyaya kaldıralabilsin (teftişte e-imza uygulamaya alınana kadar)
                //if (form.evrakTipi!=(int)ENUMEvrakTipi.FORMLAR && form.evrakNo <= 0)
                //    continue;

                sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, form, ENUMEvrakHareketDurum.DOSYAYAKALDIR);
                if (sonuc.islemSonuc)
                {
                    foreach (DosyaPlani dosya in form.dosyaKodlari)
                    {
                        string[] d = dosya.kod.Split('-');
                        dosya.kod = d[0];

                        if (dosya.kod.Trim() == "")
                            continue;
                    }

                    sonuc = EVREvrakDosyaPlani.Kaydet(ve, kullanan, form.prEvrakNo, form.dosyaKodlari, true, null);
                    if (!sonuc.islemSonuc)
                        break;

                    if (OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0) > 0)
                    {
                        EvrakHareket hareket = new EvrakHareket();
                        hareket.prEvrakNo = form.prEvrakNo;
                        hareket.birimKod1 = form.bulunduguYer;
                        if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, form.bulunduguYer) ||
                            form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                            hareket.kisi1 = kullanan.mernis;
                        hareket.islem = (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR;
                        hareket.havaleNotu = form.hareket.havaleNotu;
                        hareket.vekil = form.hareket.vekil;
                        hareket.vekilAd = form.hareket.vekilAd;

                        sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                        if (!sonuc.islemSonuc)
                            break;
                    }

                    //Arşivleme tarihi güncelleme
                    sonuc = tabEvrak.Guncelle(new EvrakSinif() { evrEvrakNo = form.evrEvrakNo, arsivlemeTarihi = new TNSDateTime(DateTime.Now.ToString()) });
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                sonuc.bilgiStr = "Dosyaya kaldırma işlemi başarıyla tamamlandı." + "<br><b>Dağıtım Not:</b>" + dagitimBilgi;


            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların kullanıcı tarafından dosyaya kaldırma işlemini gerçekleştiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc ArsiveKaldir(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI071, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            foreach (EvrakSinif form in liste.objeler)
            {
                sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, form, ENUMEvrakHareketDurum.ARSIVEKALDIR);
                if (sonuc.islemSonuc)
                {
                    EvrakHareket hareket = new EvrakHareket();
                    hareket.prEvrakNo = form.prEvrakNo;
                    hareket.birimKod1 = form.bulunduguYer;
                    if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, form.bulunduguYer) ||
                        form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                        hareket.kisi1 = kullanan.mernis;
                    hareket.islem = (int)ENUMEvrakHareketDurum.ARSIVEKALDIR;
                    hareket.islemTarih = new TNSDateTime(DateTime.Now);
                    hareket.vekil = form.hareket.vekil;
                    hareket.vekilAd = form.hareket.vekilAd;
                    hareket.islemYapan = kullanan.mernis;
                    hareket.islemYapanAd = kullanan.ad;
                    //eh.siraNo =Convert.ToInt32(jc.Value<string>("SIRANO"));//????
                    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            //if (sonuc.islemSonuc)
            //    sonuc.bilgiStr = "Arşive kaldırma işlemi başarıyla tamamlandı.";


            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc DagitimaGonder(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif[] evraklar, EvrakDagitimHareket[] evrakDagitimHareketListesi, bool dosyayaKaldir)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            //sonuc.DegerAta("Dağıtıma gönderme işlemi şu anda yapılamamaktadır. Lütfen daha sonra tekrar deneyin.");
            //return sonuc;

            if (evrakDagitimHareketListesi == null || evrakDagitimHareketListesi.Length == 0)
                return new Sonuc(false, "Dağıtım için hareket bilgisi bulunamadı.", "");
            else if (evraklar == null || evraklar.Length == 0)
                return new Sonuc(false, "Dağıtım için evrak bilgisi bulunamadı.", "");

            foreach (EvrakSinif form in evraklar)
            {
                if (form.evrakTipi != (int)ENUMEvrakTipi.GIDEN) continue;
                if (form.evrakNo <= 0)
                    return new Sonuc(false, "Evrak numarası alınmamış evraklar dağıtıma gönderilemez.", "");
            }

            //Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            ObjectArray evrakListeYeni = new ObjectArray();
            foreach (EvrakSinif form in evraklar)
            {
                ObjectArray evrListe = Listele(ve, kullanan, form);
                if (evrListe.ObjeSayisi >= 1)
                {
                    EvrakSinif evrak = (EvrakSinif)evrListe.objeler[0];
                    evrak.bulunduguYer = form.bulunduguYer;

                    sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, ENUMEvrakHareketDurum.DAGITIMYAPILDI);
                    if (!sonuc.islemSonuc)
                    {
                        sonuc.DegerAta("<b>" + evrak.evrakNo + "</b> nolu evrak dağıtım yapmak için uygun değildir.");
                        break;
                    }


                    foreach (var dagitimHareket in evrakDagitimHareketListesi)
                    {
                        if (dagitimHareket.prEvrakNo == evrak.prEvrakNo && dagitimHareket.kisi == "" && dagitimHareket.evrakBirimKod == evrak.bulunduguYer)
                        {
                            sonuc.DegerAta("<b>" + evrak.evrakNo + "</b> nolu evrak dağıtım yapmak için uygun değildir.<br><b>Not: </b>Kendi biriminizi dağıtım birimi olarak seçemezsiniz.<br>");
                            break;
                        }
                    }

                    if (sonuc.islemSonuc)
                        evrakListeYeni.objeler.Add(evrak);
                }
            }

            if (evrakListeYeni.objeler.Count == 0)
            {
                ve.VeriErisimKapat();
                sonuc.DegerAta("Evrak bilgilerini okuma sırasında hata oluştu.");
                return sonuc;
            }

            ve.BeginTransaction();
            try
            {
                //int index = 0;
                foreach (EvrakSinif evrak in evrakListeYeni.objeler)
                {
                    //Dağıtım Hareketi
                    //////////////////////
                    EvrakHareket hareket = new EvrakHareket();
                    hareket.prEvrakNo = evrak.prEvrakNo;
                    hareket.islem = (int)ENUMEvrakHareketDurum.DAGITIMYAPILDI;
                    hareket.birimKod1 = evrak.hareket.birimKod1;
                    hareket.birimKod2 = evrak.hareket.birimKod2;
                    hareket.kisi1 = evrak.hareket.kisi1;
                    hareket.kisi2 = evrak.hareket.kisi2;
                    hareket.havaleNotu = evrak.hareket.havaleNotu;
                    hareket.vekil = evrak.hareket.vekil;
                    hareket.vekilAd = evrak.hareket.vekilAd;
                    //*
                    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                    //*
                    if (!sonuc.islemSonuc)
                    {
                        Arac.HataStrYaz("{1DA52ECB-AC35-4548-84ED-0EAE6BADF0E0} - prEvrakNo: " + evrak.prEvrakNo + " - Message: " + sonuc.hataStr);
                        break;
                    }
                    //////////////////////


                    //////////////////////
                    EvrakSinif formDagitim = new EvrakSinif();
                    formDagitim.evrEvrakNo = evrak.evrEvrakNo;
                    formDagitim.dagitimTarih = new TNSDateTime(DateTime.Now.ToString());
                    formDagitim.iletimTarihi = formDagitim.dagitimTarih;
                    //*
                    sonuc = tab.Guncelle(formDagitim);
                    //*
                    if (!sonuc.islemSonuc)
                    {
                        Arac.HataStrYaz("{B229BD68-E7EA-4E5F-87C4-8B4BACA17A26} - prEvrakNo: " + evrak.prEvrakNo + " - Message: " + sonuc.hataStr);
                        break;
                    }
                    //////////////////////

                    ObjectArray evrakDagitimHareketListesi2 = new ObjectArray();
                    foreach (var dagitimHareket in evrakDagitimHareketListesi)
                    {
                        if (dagitimHareket.prEvrakNo == evrak.prEvrakNo)
                        {
                            //Dağıtım Hareket Bilgisi
                            //////////////////////

                            string evrEvrakBirimKod = dagitimHareket.evrakBirimKod;
                            string idariBirimKimlikKod = dagitimHareket.idariBirimKimlikKod;

                            if (string.IsNullOrEmpty(evrEvrakBirimKod))
                            {
                                if (!string.IsNullOrEmpty(idariBirimKimlikKod))
                                    evrEvrakBirimKod = EVRBirim.BirimKodGetir(ve, idariBirimKimlikKod);
                            }

                            if (!string.IsNullOrEmpty(evrEvrakBirimKod))   //Kurum içi yazışma
                            {
                                //Arac.HataStrYaz("Başla:" + evrak.prEvrakNo);
                                //*
                                sonuc = EVREvrak.DagitimEvrakKaydet(ve, kullanan, evrak, evrEvrakBirimKod, dagitimHareket.kisi);
                                //*
                                //Arac.HataStrYaz("Bitiş:" + evrak.prEvrakNo);

                                if (sonuc.islemSonuc)
                                    dagitimHareket.islem = (int)ENUMEvrakDagitimHareketIslem.GONDERILDI;
                                else
                                    break; //dagitimHareket.islem = (int)ENUMEvrakDagitimHareketIslem.HATAOLUSTU;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(idariBirimKimlikKod))
                                    dagitimHareket.islem = (int)ENUMEvrakDagitimHareketIslem.GONDERILMEDI; //Dağıtım yapılamaz
                                else
                                    dagitimHareket.islem = (int)ENUMEvrakDagitimHareketIslem.ISLEMEALINDI; //Dış yazışma
                            }
                            //////////////////////

                            evrakDagitimHareketListesi2.objeler.Add(dagitimHareket);
                        }
                    }

                    if (!sonuc.islemSonuc)
                    {
                        Arac.HataStrYaz("{864BEFA9-64A0-4A99-8DE9-1B3DE6F5830F} - prEvrakNo: " + evrak.prEvrakNo + " - Message: " + sonuc.hataStr);
                        break;
                    }

                    if (evrakDagitimHareketListesi2.objeler.Count > 0)
                    {
                        //*
                        sonuc = EVREvrakDagitimHareket.Kaydet(ve, kullanan, (EvrakDagitimHareket[])evrakDagitimHareketListesi2.objeler.ToArray(typeof(EvrakDagitimHareket)));
                        //*
                        if (!sonuc.islemSonuc)
                        {
                            Arac.HataStrYaz("{F00A1DAF-C779-4ED1-978B-1E1B2BBD0870} - prEvrakNo: " + evrak.prEvrakNo + " - Message: " + sonuc.hataStr);
                            break;
                        }
                    }
                    else
                    {
                        sonuc.DegerAta("<b>" + evrak.evrakNo + "</b> nolu evrak için dağıtım bilgisi bulunamadı.");
                        break;
                    }
                }

                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                }
                else
                {
                    ve.Commit();
                    sonuc.bilgiStr = "Dağıtım işlemi başlatıldı.";
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{F00A1DAF-C779-4ED1-978B-1E1B2BBD087F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            //Arac.HataStrYaz("Dağıtıma Gönder Tamamlandı");
            ve.VeriErisimKapat();


            //Dağıtım sonrasında dosyaya kaldırma işlemi yapılsın mı?
            if (dosyayaKaldir && sonuc.islemSonuc)
            {
                //*
                Sonuc sonucDosyaKaldir = EVREvrak.DosyayaKaldir(null, kullanan, evrakListeYeni);
                //*
                if (!sonucDosyaKaldir.islemSonuc)
                    Arac.HataStrYaz("{F00A1DAF-C779-4ED1-978B-1E1B2BBD0871} - Message: " + sonuc.hataStr);
            }
            //********************************************************

            return sonuc;

        }
        private static Sonuc DagitimEvrakKaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif evrak)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            evrak.dagitim = EVREvrakDagitim.Listele(veriErisim, evrak.prEvrakNo);
            foreach (EvrakDagitim eDagitim in evrak.dagitim)
            {
                if (OrtakFonksiyonlar.ConvertToInt(eDagitim.dagitimKod, 0) > 0)
                {
                    //Tanımlalan dağıtımlar
                    ObjectArray listeDagitim = EVRDagitim.Listele(veriErisim, kullanan, eDagitim.dagitimKod, null, true);
                    if (!(listeDagitim.sonuc.islemSonuc && listeDagitim.objeler.Count == 1)) { continue; }

                    Dagitim d = (Dagitim)listeDagitim.objeler[0];
                    foreach (DagitimAlan dAlan in d.alanlar)
                    {
                        sonuc = DagitimEvrakKaydet(veriErisim, kullanan, evrak, dAlan.evrakBrimKodu, dAlan.kisi);
                        if (!sonuc.islemSonuc)
                        {
                            sonuc.hataStr += "[DagitimEvrakKaydet]";
                            return sonuc;
                        }
                    }
                }
                else if (eDagitim.dagitimKod.Length > 1)
                {
                    //Dağıtım kodu birim tablosunda kayıltlımı kontrol et
                    Birim brm = new Birim(); brm.kod = eDagitim.dagitimKod;
                    ObjectArray brmListe = EVRBirim.Listele(veriErisim, brm, false);

                    if (brmListe.objeler.Count > 0)
                    {
                        sonuc = DagitimEvrakKaydet(veriErisim, kullanan, evrak, eDagitim.dagitimKod, "");
                        if (!sonuc.islemSonuc)
                        {
                            sonuc.hataStr += "[DagitimEvrakKaydet]";
                            return sonuc;
                        }
                    }
                    else
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = "Dağıtım birimi bulunamadı. Dağıtım yapılabilmesi için birimler tanımlı listeden seçilmeli.";
                    }
                }
            }

            if (evrak.dagitim.Count == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Dağıtım yapılacak birim bulunamadı.";
            }

            return sonuc;
        }
        public static Sonuc DagitimEvrakKaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif evrak, string dagitimYapilacakBirimKod, string dagitimKisi)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI071, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            Birim birim = EVRBirim.EvrakDagitimBirimGetir(ve, evrak.evrakBirimKodu);
            string evrakBirimAdi = birim.ad;
            string evrakBirimKodu = birim.kod;

            EvrakSinif evr = new EvrakSinif();
            evr.evrakTipi = (int)ENUMEvrakTipi.GELEN;
            //if (System.Configuration.ConfigurationManager.AppSettings.Get("EVRDagitimliEvrakNumaraAlmasin") == "1")
            evr.evrakNo = -1; //Evrak No Almasın
            evr.evrakBirimKodu = dagitimYapilacakBirimKod;
            evr.geldigiYer = evrakBirimAdi; //evrakBirimKodu +"-" + evrakBirimAdi;
            evr.geldigiYerKod = evrakBirimKodu;
            evr.gelenEvrakTarih = new TNSDateTime(DateTime.Now.ToString());
            evr.gelenEvrakNo = evrak.evrakNo.ToString(); //evrakBirimKodu + "-" + evrak.evrakNo.ToString();
            evr.konu = evrak.konu;
            evr.dosyaKodlari = evrak.dosyaKodlari;
            evr.aktarim = true;
            evr.geldigiYerTur = (int)ENUMEvrakGeldigiYerTur.KURUMICI;
            evr.cevapSuresi = evrak.cevapSuresi > 0 ? evrak.cevapSuresi : 0;
            evr.ekAdedi = "";
            evr.gizlilik = evrak.gizlilik;
            evr.yaziDurum = evrak.yaziDurum;

            evr.bulunduguYer = dagitimYapilacakBirimKod;
            evr.kayitIslem = (int)ENUMEvrakKayitIslem.DAGITIMDANKAYIT;
            evr.hareket.vekil = evrak.hareket.vekil;
            evr.hareket.vekilAd = evrak.hareket.vekilAd;
            evr.dosyasiVar = evrak.dosyasiVar;
            if (EVREvrak.YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu.EvrakKisiyeDagitimYapilabilsin) && !string.IsNullOrWhiteSpace(dagitimKisi))
                evr.kayitKriterleri.Add("dagitimKisi", dagitimKisi);

            ve.BeginTransaction();

            sonuc = Kaydet(ve, kullanan, evr);

            if (sonuc.islemSonuc)
            {
                //Arac.HataStrYaz("{57B9A97D-8017-4595-9F0C-8AACAF8C3CC7} - Dağıtım Evrak Kayıt Başarılı");

                //++Dağıtım listesini çek                                    
                //++Evrak listelenecek
                //++Evrakın ek dosyaları yeni evraka kayıt edilecek
                //++Evrak ilişki kurulacak

                string[] anahtar = sonuc.anahtar.Split('-');

                //Evrak İlişki Kaydet
                //#############################
                ObjectArray iliskiler = new ObjectArray();
                EvrakIliski ei = new EvrakIliski();
                ei.prEvrakNo = anahtar[1];
                ei.prEvrakNoIliski = evrak.prEvrakNo;
                ei.tip = 1;
                iliskiler.objeler.Add(ei);
                if (iliskiler.objeler.Count > 0)
                    sonuc = EvrakIliskiKaydet(ve, kullanan, iliskiler);
                //#############################

                if (sonuc.islemSonuc)
                {
                    //Arac.HataStrYaz("{910D5A41-CDA7-44AF-8310-0F2F083658A8} - Dağıtım Evrak Kayıt İlişki Başarılı");

                    //EvrakEkDosya Kaydet                                                                                                                                                
                    //#############################
                    EvrakSinif hedefEvrak = new EvrakSinif();
                    hedefEvrak.evrakBirimKodu = evr.evrakBirimKodu;
                    hedefEvrak.evrakOlusturmaTarihi = evr.evrakTarih;
                    hedefEvrak.evrEvrakNo = anahtar[0];
                    hedefEvrak.kayitKriterleri = evr.kayitKriterleri;

                    sonuc = EvrakEkDosyaKopyala(ve, kullanan, evrak, hedefEvrak);
                    //if (sonuc.islemSonuc)
                    //    Arac.HataStrYaz("{D031996B-A12F-44E2-A927-6B150C8FAD2E} - Dağıtım Evrak Kayıt Dosya Kopyalama Başarılı");

                    //#############################
                }

            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                OrtakFonksiyonlar.HataStrYaz("{EEEC4475-F9F8-423D-A9D4-71001CC25F88} - YeniEvrakGeldiPostasiHazirla; dagitimKisi: " + dagitimKisi + ", dagitimYapilacakBirimKod: " + dagitimYapilacakBirimKod + ", evrEvrakNo:" + evrak.evrEvrakNo);

                EVRPosta.YeniEvrakGeldiPostasiHazirla(ve, kullanan, evrak.evrEvrakNo, evrak.prEvrakNo, dagitimKisi, dagitimYapilacakBirimKod, (int)ENUMPOSTATIP.HAVALE);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        #region "Evrak Dosya İşlemleri - Kontorl edilecek - server evrak'ı kaydetme izni yok"

        /// <summary>
        /// Parametre olarak verilen kaynak ve hedef evrak bilgilerine göre, evrak'ın dosyasını kopyalayan yordam.
        /// </summary>
        /// <param name="kaynakEvrak">Kopyalanacak evrak bilgilerini içeren nesne</param>
        /// <param name="hedefEvrak">Oluşturulacak evrak bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc EvrakEkDosyaKopyala(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif kaynakEvrak, EvrakSinif hedefEvrak)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            try
            {
                string kaynakYol = EvrakEkDosyaYolu(veriErisim, kaynakEvrak.evrakBirimKodu, kaynakEvrak.evrakOlusturmaTarihi, kaynakEvrak.evrEvrakNo);
                string hedefYol = EvrakEkDosyaYolu(veriErisim, hedefEvrak.evrakBirimKodu, hedefEvrak.evrakOlusturmaTarihi, hedefEvrak.evrEvrakNo);
                DizinYoksaOlustur(hedefYol);

                int kopyalananDosyaSayisi = 0;
                int dagitimCokluGonderilen = 0;
                DirectoryInfo dInfoKaynak = new DirectoryInfo(kaynakYol + "Dagitim");
                if (dInfoKaynak.Exists)
                {
                    FileInfo[] kaynakDosyalar = dInfoKaynak.GetFiles();

                    Stack<FileInfo> s = new Stack<FileInfo>();
                    foreach (FileInfo kaynakDosya in kaynakDosyalar)
                    {
                        if (kaynakDosya.Extension.ToLower() == ".bak" || kaynakDosya.Extension.ToLower() == ".pdf") continue;

                        s.Push(kaynakDosya);
                        dagitimCokluGonderilen++;
                    }
                    foreach (FileInfo kaynakDosya in dInfoKaynak.GetFiles("*.pdf"))
                        s.Push(kaynakDosya);



                    if (EVREvrak.YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu.EvrakEImzaEYPPaketiKullan))
                    {
                        if (Directory.Exists(kaynakYol))
                        {
                            foreach (FileInfo kaynakDosya in new DirectoryInfo(kaynakYol).GetFiles("*.p7s"))
                                s.Push(kaynakDosya);
                        }
                    }


                    string hedefDosyaAd = "";
                    if (hedefEvrak.kayitKriterleri.ContainsKey("dagitimKisi"))
                    {
                        string tmpKisi = hedefEvrak.kayitKriterleri["dagitimKisi"] + "";
                        hedefDosyaAd = EVRGenel.KullaniciAdGetir(tmpKisi);
                    }
                    else
                    {
                        Birim brm = EVRBirim.Oku(veriErisim, new Birim() { kod = hedefEvrak.evrakBirimKodu });
                        if (brm != null && !string.IsNullOrWhiteSpace(brm.ad))
                            hedefDosyaAd = brm.ad.Trim();
                    }

                    if (hedefDosyaAd != "")
                    {

                        if (EVREvrak.YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu.EvrakEImzaEYPPaketiKullan))
                        {
                            bool p7sVarMi = false;
                            foreach (FileInfo kaynakDosya in s)
                            {
                                if (kaynakDosya.Name.ToLower() == "gidenevrak.pdf.p7s")
                                {
                                    sonuc = EvrakDagitimKopyalaVeDosyaKaydet(veriErisim, kullanan, kaynakDosya, new FileInfo(hedefYol + kaynakDosya.Name), hedefEvrak.evrEvrakNo, ref kopyalananDosyaSayisi);
                                    p7sVarMi = true;
                                    break;
                                }
                            }

                            if (!p7sVarMi)
                                foreach (FileInfo kaynakDosya in s)
                                {
                                    if (kaynakDosya.Name.ToLower() == "gidenevrak.pdf")
                                    {
                                        sonuc = EvrakDagitimKopyalaVeDosyaKaydet(veriErisim, kullanan, kaynakDosya, new FileInfo(hedefYol + kaynakDosya.Name), hedefEvrak.evrEvrakNo, ref kopyalananDosyaSayisi);
                                        break;
                                    }
                                }

                        }
                        else
                        {
                            foreach (FileInfo kaynakDosya in s)
                            {
                                if (kaynakDosya.Name.IndexOf(EVREvrak.DosyaAdDuzenle(hedefDosyaAd)) > -1)
                                    sonuc = EvrakDagitimKopyalaVeDosyaKaydet(veriErisim, kullanan, kaynakDosya, new FileInfo(hedefYol + kaynakDosya.Name), hedefEvrak.evrEvrakNo, ref kopyalananDosyaSayisi);
                            }
                        }

                    }

                    if (dagitimCokluGonderilen > 0 && kopyalananDosyaSayisi == 0)
                        sonuc.DegerAta(hedefDosyaAd + " için dağıtım dosyası gönderilemedi.<br>(Not: dağıtım alanına yeni birim eklendikten sonra editör ile kayıt işlemi yapılmamış olabilir.)");
                }


                dInfoKaynak = new DirectoryInfo(kaynakYol);
                if (dInfoKaynak.Exists)
                {
                    FileInfo[] kaynakDosyalar = dInfoKaynak.GetFiles();
                    foreach (FileInfo kaynakDosya in kaynakDosyalar)
                        if (!(dagitimCokluGonderilen > 0 && Path.GetFileNameWithoutExtension(kaynakDosya.FullName) == "GidenEvrak"))
                            if (kaynakDosya.Extension.ToLower() != ".bak" && kaynakDosya.Name.ToLower() != "gidenevrak.pdf.p7s")
                                EvrakDagitimKopyalaVeDosyaKaydet(veriErisim, kullanan, kaynakDosya, new FileInfo(hedefYol + kaynakDosya.Name), hedefEvrak.evrEvrakNo, ref kopyalananDosyaSayisi);
                }


            }
            catch (Exception e)
            {
                sonuc.hataStr = e.Message;
                sonuc.islemSonuc = false;
            }

            return sonuc;
        }

        public static Sonuc EvrakDagitimKopyalaVeDosyaKaydet(VeriErisim ve, Kullanici kullanan, FileInfo kaynakDosya, FileInfo hedefDosya, string evrEvrakNo, ref int kopyalananDosyaSayisi, int siraNo = -1)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (kaynakDosya.Exists)
            {
                File.Copy(kaynakDosya.FullName, hedefDosya.FullName, true);
                hedefDosya.Refresh();
                if (hedefDosya.Exists)
                {
                    EvrakDosya eDosya = new EvrakDosya();
                    eDosya.evrEvrakNo = evrEvrakNo;
                    eDosya.siraNo = siraNo;
                    eDosya.dosyaYolu = hedefDosya.Directory.FullName;
                    eDosya.dosyaAdi = hedefDosya.Name;
                    eDosya.dosyaTarihi = new TNSDateTime(DateTime.Now);
                    eDosya.dosyaBoyutu = hedefDosya.Length;
                    eDosya.islemYapan = kullanan.mernis;
                    eDosya.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                    eDosya.islemTarih = new TNSDateTime(DateTime.Now);
                    //eDosya.ustYaziDosyasi = ustYaziDosyasi;
                    eDosya.ustYaziDosyasi = 1;

                    sonuc = EVROCR.EvrakDosyaKaydet(ve, kullanan, eDosya);
                    if (sonuc.islemSonuc)
                        sonuc = EVREvrak.DosyaBilgisiKaydet(ve, kullanan, eDosya.evrEvrakNo, true);
                }
                else
                    sonuc.hataStr = hedefDosya.FullName + " adresindeki dosya bulunamadı";

                if (sonuc.islemSonuc) kopyalananDosyaSayisi += 1;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgilerine göre, evrak'ın dosya yolununu metin nesnesi olrak getiren yordam.
        /// </summary>
        /// <param name="evrakBirimi">Evrak birim kodunu içeren nesne</param>
        /// <param name="evrakOlusturmaTarihi">Evrak oluşturma tarihini içeren nesne</param>
        /// <param name="evrEvrakNo">Evrak evrEvrakNo bilgisini içeren nesne</param>
        /// <returns></returns>
        public static string EvrakEkDosyaYolu(VeriErisim veriErisim, string evrakBirimi, TNSDateTime evrakOlusturmaTarihi, string evrEvrakNo)
        {
            string evrakTarih = evrakOlusturmaTarihi.Yil + evrakOlusturmaTarihi.Ay.ToString("00");

            char dirSepChar = Path.DirectorySeparatorChar;
            string evrakDosyaYol = System.Configuration.ConfigurationManager.AppSettings.Get("EvrakDosyaYol") + "";
            if (!evrakDosyaYol.EndsWith(dirSepChar.ToString()))
                evrakDosyaYol += dirSepChar.ToString();
            if (evrakBirimi.ToLower() != "temp")
                evrakBirimi = EVRBirim.EvrakKokBirimGetir(veriErisim, evrakBirimi);
            evrakDosyaYol += evrakBirimi + dirSepChar + evrakTarih + dirSepChar + evrEvrakNo + dirSepChar.ToString();

            return evrakDosyaYol;
        }
        /// <summary>
        /// Parametre olarak verilen dizin adresini dizin yoksa oluşturan yordam.
        /// </summary>
        /// <param name="path">Dizin adresini içeren nesne</param>
        /// <returns>Oluşturulan dizin string tipinde nesne olarak döndürülür.</returns>
        private static string DizinYoksaOlustur(string path)
        {
            string hata = string.Empty;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    hata = ex.Message;
                }
            }
            return hata;
        }

        private static string DosyaAdDuzenle(string dosyaAd)
        {
            //\ / : ? * " < > |
            if (!string.IsNullOrEmpty(dosyaAd))
            {
                dosyaAd = dosyaAd.Replace("\r\n", " ");
                dosyaAd = dosyaAd.Replace("\r", " ");
                dosyaAd = dosyaAd.Replace("\n", " ");
                dosyaAd = dosyaAd.Replace("\\", "");
                dosyaAd = dosyaAd.Replace("/", "");
                dosyaAd = dosyaAd.Replace(":", "");
                dosyaAd = dosyaAd.Replace("?", "");
                dosyaAd = dosyaAd.Replace("*", "");
                dosyaAd = dosyaAd.Replace("\"", "");
                dosyaAd = dosyaAd.Replace("<", "");
                dosyaAd = dosyaAd.Replace(">", "");
                dosyaAd = dosyaAd.Replace("|", "");
            }
            return dosyaAd;
        }
        #endregion


        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların kullanıcı tarafından dosyayadan geri alma işlemini gerçekleştiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc DosyadanAl(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);
            int basarili = 0;

            foreach (EvrakSinif evrak in liste.objeler)
            {
                sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, ENUMEvrakHareketDurum.DOSYADANALINDI);

                if (sonuc.islemSonuc)
                {
                    EvrakHareket hareket = new EvrakHareket();
                    hareket.prEvrakNo = evrak.prEvrakNo;
                    hareket.birimKod1 = evrak.bulunduguYer;
                    if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, evrak.bulunduguYer) ||
                        evrak.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                        hareket.kisi1 = kullanan.mernis;
                    hareket.islem = (int)ENUMEvrakHareketDurum.DOSYADANALINDI;

                    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);

                    if (sonuc.islemSonuc)
                        basarili++;
                }
            }

            if (sonuc.hataStr == "")
                sonuc.islemSonuc = true;
            else
                sonuc.islemSonuc = false;

            sonuc.anahtar = basarili.ToString();
            ve.VeriErisimKapat();
            return sonuc;
        }

        //private static Sonuc EvrakImzala(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, ENUMEvrakHareketDurum islem)
        //{
        //    Sonuc sonuc = new Sonuc(false, "Bilinmeyen Hata", "");

        //    //if (liste.objeler.Count > 1)
        //    //{
        //    //    sonuc.DegerAta("Lütfen imzalamak için tek kayıt seçiniz!");
        //    //    return sonuc;
        //    //}

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

        //    foreach (EvrakSinif evrak in liste.objeler)
        //    {
        //        sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, islem);
        //        if (!sonuc.islemSonuc)
        //            return sonuc;
        //    }

        //    ve.BeginTransaction();
        //    try
        //    {
        //        TABEvrak tab = new TABEvrak(ve);
        //        TABGenel tabGenel = new TABGenel(ve);
        //        int basarili = 0;

        //        foreach (EvrakSinif evrak in liste.objeler)
        //        {
        //            if (sonuc.islemSonuc)
        //            {
        //                EvrakHareket yeniHareketi = new EvrakHareket();
        //                yeniHareketi.prEvrakNo = evrak.prEvrakNo;
        //                yeniHareketi.islem = (int)ENUMEvrakHareketDurum.YENI;
        //                ObjectArray listeHareket = EVREvrakHareket.Listele(ve, yeniHareketi);
        //                foreach (EvrakHareket h in listeHareket.objeler)
        //                {
        //                    yeniHareketi = h;
        //                }

        //                EvrakHareket hareket = new EvrakHareket();
        //                hareket.prEvrakNo = evrak.prEvrakNo;
        //                hareket.islem = (int)islem;
        //                hareket.birimKod2 = evrak.bulunduguYer;

        //                if (hareket.islem == (int)ENUMEvrakHareketDurum.GERIGONDERILDI && evrak.hareketHavale.Count > 0)
        //                {
        //                    EvrakHareket h = (EvrakHareket)evrak.hareketHavale[0];
        //                    hareket.birimKod1 = h.birimKod1;
        //                    hareket.kisi2 = h.kisi2;
        //                    hareket.kisi1 = h.kisi1;
        //                }
        //                else
        //                {
        //                    hareket.birimKod1 = yeniHareketi.birimKod1;
        //                    hareket.kisi2 = kullanan.mernis;
        //                    hareket.kisi1 = yeniHareketi.kisi1;
        //                }
        //                hareket.havaleNotu = evrak.hareket.havaleNotu;
        //                hareket.vekil = evrak.hareket.vekil;
        //                hareket.vekilAd = evrak.hareket.vekilAd;

        //                sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);

        //                if (sonuc.islemSonuc)
        //                {
        //                    //EVREVRAKNO bulunması için
        //                    EvrakSinifKucuk form = EvrakNoVeTarihVer(ve, evrak.prEvrakNo);

        //                    bool imzaYoluIslem = true;
        //                    if (hareket.islem == (int)ENUMEvrakHareketDurum.IMZALANDI || hareket.islem == (int)ENUMEvrakHareketDurum.EIMZALANDI || hareket.islem == (int)ENUMEvrakHareketDurum.EPARAFLANDI)
        //                    {
        //                        EvrakSinif imzaliForm = new EvrakSinif();
        //                        imzaliForm.evrEvrakNo = form.evrEvrakNo;
        //                        imzaliForm.imzaTarihi = new TNSDateTime(DateTime.Now.ToString());
        //                        sonuc = tab.Guncelle(imzaliForm);
        //                        if (evrak.imzaYolu == 0) imzaYoluIslem = false;
        //                    }

        //                    if (sonuc.islemSonuc)
        //                    {
        //                        if (!string.IsNullOrEmpty(evrak.surecAd) && string.IsNullOrEmpty(evrak.dinamikSurecNo))
        //                        {
        //                            WCFClient wcf = new WCFClient();
        //                            Dictionary<string, object> inputs = TNS.EVR.Arac.SurecParametrelerineDegerAta(wcf, evrak.surecAd, kullanan, evrak.evrEvrakNo, evrak.prEvrakNo, true);
        //                            string surecNo = wcf.CreateAndRunWorkflowByName(evrak.surecAd, inputs, kullanan.kullaniciKodu);

        //                            //SurecNo bilgisini güncelle
        //                            if (!string.IsNullOrEmpty(surecNo))
        //                            {
        //                                EvrakSinif surecForm = new EvrakSinif();
        //                                surecForm.evrEvrakNo = form.evrEvrakNo;
        //                                surecForm.dinamikSurecNo = surecNo;
        //                                sonuc = tab.Guncelle(surecForm);
        //                            }
        //                        }
        //                        else if (!string.IsNullOrEmpty(evrak.dinamikSurecNo)) //Süreç başlamış ise
        //                        {
        //                            //Süreç Devam Et
        //                            WCFClient wcf = new WCFClient();
        //                            Dictionary<string, object> inputs = new Dictionary<string, object>();
        //                            inputs.Add("Result", true);
        //                            string msg = wcf.RunWorkFlowByWorkflowInstanceID(kullanan.mernis, kullanan.kullaniciKodu, Guid.Parse(evrak.dinamikSurecNo), inputs);
        //                            if (!string.IsNullOrEmpty(msg))
        //                            {
        //                                sonuc.islemSonuc = false;
        //                                sonuc.hataStr = msg;
        //                            }
        //                        }
        //                        else if (imzaYoluIslem)
        //                        {
        //                            //İmza Yolu İşlem Hareket
        //                            sonuc = EVREvrak.EvrakImzaHareketKaydet(ve, kullanan, evrak, islem, hareket);
        //                        }
        //                    }


        //                    if (sonuc.islemSonuc)
        //                    {
        //                        basarili++;

        //                        //Sıra Numarası - Hüseyin  (Onay Numarası) (Melih kapattı-02.03.2012-Numara alındığında (eskisi gibi) alınması gerektiği için) 
        //                        //if (evrak.siraNo == 0 && evrak.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
        //                        //{
        //                        //    string belgeTur = "";
        //                        //    if (evrak.evrakTuru == (int)ENUMGidenEvrakTuru.BAKANLIKMAKAMONAYI)
        //                        //        belgeTur = "BAKOY";

        //                        //    if (!string.IsNullOrEmpty(belgeTur))
        //                        //    {
        //                        //        string sayacKod = "EVRGIDEN" + belgeTur + DateTime.Now.Year + EVRBirim.EvrakKokBirimGetir(ve, evrak.evrakBirimKodu);
        //                        //        evrak.siraNo = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "0000000"), 0);

        //                        //        EvrakSinif evrakKaydet = new EvrakSinif();
        //                        //        evrakKaydet.evrEvrakNo = evrak.evrEvrakNo;
        //                        //        evrakKaydet.siraNo = evrak.siraNo;
        //                        //        sonuc = tab.Guncelle(evrakKaydet);
        //                        //    }
        //                        //}
        //                        //****************************************
        //                    }
        //                    else
        //                        break;
        //                }
        //                else
        //                {
        //                    if (islem == ENUMEvrakHareketDurum.IMZALANDI)
        //                        sonuc.hataStr += string.Format(EvrakI.EVRI072, evrak.evrEvrakNo) + "<br>";
        //                    else if (islem == ENUMEvrakHareketDurum.GERIGONDERILDI)
        //                        sonuc.hataStr += string.Format(EvrakI.EVRI073, evrak.evrEvrakNo) + "<br>";
        //                    else if (islem == ENUMEvrakHareketDurum.PARAFLANDI)
        //                        sonuc.hataStr += string.Format("{0} nolu evrak paraflanamadı..", evrak.evrakNo == 0 ? evrak.evrEvrakNo + " programatik" : evrak.evrakNo.ToString()) + "<br>";
        //                }
        //            }

        //            if (sonuc.hataStr != "")
        //                break;
        //        }

        //        if (sonuc.hataStr == "")
        //        {
        //            ve.Commit();
        //            sonuc.islemSonuc = true;
        //        }
        //        else
        //        {
        //            ve.Rollback();
        //            sonuc.islemSonuc = false;
        //            basarili = 0;
        //        }

        //        sonuc.anahtar = basarili.ToString();
        //        ve.VeriErisimKapat();
        //    }
        //    catch (Exception ex)
        //    {
        //        ve.Rollback();
        //        ve.VeriErisimKapat();
        //        sonuc.DegerAta(ex.Message);
        //        Arac.HataStrYaz("{2BE1A971-BC79-431A-BEBB-0532B27E52D0} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
        //    }
        //    return sonuc;
        //}

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların kullanıcı tarafından imzalanma işlemini gerçekleştiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgisini içeren nesne</param>
        /// <param name="islem">Evrak hareket işlem bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        //public static Sonuc Imzala(Kullanici kullanan, ObjectArray liste, ENUMEvrakHareketDurum islem)
        //{
        //    return EvrakImzala(null, kullanan, liste, islem);
        //}

        ///// <summary>
        ///// Parametre olarak verilen EvrakSinif nesne koleksiyonundaki evrakların kullanıcı tarafından elektronik olarak imzalanma işlemini gerçekleştiren yordam.
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="liste">Evrak bilgisini içeren nesne</param>
        ///// <param name="islem">Evrak hareket işlem bilgisini içeren nesne</param>
        ///// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        //public static Sonuc EImzala(Kullanici kullanan, ObjectArray liste, ENUMEvrakHareketDurum islem)
        //{
        //    return EvrakImzala(null, kullanan, liste, islem);
        //}

        public static Sonuc ImzaYolunaGoreImzaYetkisiVarMi(VeriErisim veriErisim, Kullanici kullanan, ObjectArray evraklar)
        {
            Sonuc sonuc = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            foreach (EvrakSinif evrak in evraklar.objeler)
            {
                if (evrak.imzaYolu == 0)
                {
                    sonuc.hataStr = "";
                    sonuc.islemSonuc = true;
                    continue;
                }
                bool imzaYetkisi = false;
                ImzaYolu imzaYolu = EVRImzaYolu.Oku(ve, kullanan, new ImzaYolu() { kod = evrak.imzaYolu });

                EvrakImzaYolu evrImzaYolu = EVREvrakImzaYolu.Oku(ve, kullanan, new EvrakImzaYolu() { prEvrakNo = evrak.prEvrakNo });
                if (evrImzaYolu.imzaYolu.kod > 0)
                {
                    foreach (ImzaYoluDetay item in evrImzaYolu.imzaYolu.detaylar)
                    {
                        if (kullanan.mernis == item.tcKimlikNo && item.islemTip == (int)ENUMImzaYoluTip.IMZA)
                        {
                            imzaYetkisi = true;
                            break;
                        }
                    }
                }

                if (!imzaYetkisi)
                {
                    sonuc.hataStr = "Seçilen evraklar için, imza yolu bilgisine göre imzalama yetkiniz bulunmamaktadır.<br>EvrakNo: " + evrak.evrakNo;
                    sonuc.islemSonuc = false;
                    break;
                }
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        //public static Sonuc EvrakImzaHareketKaydet(VeriErisim ve, Kullanici kullanan, EvrakSinif evrak, ENUMEvrakHareketDurum hareketDurum, EvrakHareket hareket)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    //İmza yolu değişirse eski bilgileri temizle
        //    EvrakImzaYolu evrImzaYolu = EVREvrakImzaYolu.Oku(ve, kullanan, new EvrakImzaYolu() { prEvrakNo = evrak.prEvrakNo });
        //    if (evrImzaYolu.imzaYolu.kod > 0 && hareketDurum != ENUMEvrakHareketDurum.GERIGONDERILDI && evrImzaYolu.imzaYolu.kod != evrak.imzaYolu)
        //    {
        //        sonuc = EVREvrakImzaYolu.Sil(ve, kullanan, evrImzaYolu);
        //        if (!sonuc.islemSonuc)
        //            sonuc.hataStr += string.Format("{0} nolu evrak, imza sırasında bilinmeyen bir hata oluştu. ", evrak.evrakNo == 0 ? evrak.evrEvrakNo + " programatik" : evrak.evrakNo.ToString()) + "<br>";
        //        else
        //            evrImzaYolu = new EvrakImzaYolu() { prEvrakNo = evrImzaYolu.prEvrakNo };
        //    }
        //    if (!string.IsNullOrEmpty(sonuc.hataStr))
        //        sonuc.islemSonuc = false;
        //    //****************************************************

        //    //EvrakImzaYolu evrImzaYolu = EVREvrakImzaYolu.Oku(null, kullanan, new EvrakImzaYolu() { prEvrakNo = evrak.prEvrakNo, imzaYolu = new ImzaYolu() { kod = evrak.imzaYolu } });
        //    ImzaYoluDetay sonrakiImzaYolu = null;
        //    int anahtarIslem = -1;

        //    /* 
        //      * Evrak imza yolunda hiç hareket var mı? 
        //      * Yoksa: imza yolunda kayıtlı bilgilere kullanıcının kendisinide alarak, evrakImzaYolu Hareketini oluştur, daha sonra sonraki İmza Yolunu bul.
        //      * Varsa: sonraki İmza Yolunu bul.
        //    */
        //    if (hareketDurum != ENUMEvrakHareketDurum.GERIGONDERILDI && (evrImzaYolu.imzaYolu.detaylar == null || evrImzaYolu.imzaYolu.detaylar.Length == 0))
        //    {
        //        ////imza yolu seçildiyse ilk işlem yapacak paraflama yetkisinde olmalı...
        //        //if (hareketDurum == ENUMEvrakHareketDurum.IMZALANDI || hareketDurum == ENUMEvrakHareketDurum.EIMZALANDI)
        //        //{
        //        //    sonuc.hataStr = "Seçilen evraklar için, imza yolu bilgisine göre imzalama yetkiniz bulunmamaktadır.";
        //        //    sonuc.islemSonuc = false;
        //        //    return sonuc;
        //        //}

        //        ImzaYolu imzaYolu = EVRImzaYolu.Oku(ve, kullanan, new ImzaYolu() { kod = evrak.imzaYolu });

        //        //İmza Yolu seçilmiş mi kontrol et
        //        if (imzaYolu != null && imzaYolu.kod > 0 && imzaYolu.detaylar != null && imzaYolu.detaylar.Length > 0)
        //        {
        //            sonuc = EVREvrakImzaYolu.Sil(ve, kullanan, evrImzaYolu);
        //            if (sonuc.islemSonuc)
        //            {
        //                anahtarIslem = (int)ENUMEvrakHareketDurum.PARAFLANDI;

        //                int siraNo = 1;
        //                ObjectArray detalar = new ObjectArray();

        //                bool kendisiVarMi = false;
        //                foreach (ImzaYoluDetay d in imzaYolu.detaylar)
        //                    if (d.siraNo == 1 && d.tcKimlikNo == kullanan.mernis)
        //                    {
        //                        kendisiVarMi = true;
        //                        break;
        //                    }
        //                if (!kendisiVarMi)
        //                    detalar.objeler.Add(new ImzaYoluDetay() { siraNo = siraNo, islemTip = (int)ENUMImzaYoluTip.PARAF, imzaYoluKod = imzaYolu.kod, tcKimlikNo = kullanan.mernis, ad = kullanan.ad, soyad = kullanan.soyad, evrakBirimKod = evrak.bulunduguYer, unvan = kullanan.unvan });

        //                foreach (ImzaYoluDetay d in imzaYolu.detaylar)
        //                {
        //                    if ((!kendisiVarMi && siraNo == 1) || (kendisiVarMi && siraNo == 2))
        //                        sonrakiImzaYolu = d;
        //                    siraNo++;
        //                    d.siraNo = siraNo;
        //                    detalar.objeler.Add(d);
        //                }

        //                imzaYolu.detaylar = (ImzaYoluDetay[])detalar.objeler.ToArray(typeof(ImzaYoluDetay));
        //                evrImzaYolu.imzaYolu = imzaYolu;

        //                sonuc = EVREvrakImzaYolu.Kaydet(ve, kullanan, evrImzaYolu);
        //            }
        //        }
        //        else if (imzaYolu != null && (imzaYolu.detaylar == null || imzaYolu.detaylar.Length == 0))
        //            sonuc.hataStr += string.Format("{0} nolu evrak, imza yolu bilgisinde eksik bilgiler olduğundan imzalanamadı. ", evrak.evrakNo == 0 ? evrak.evrEvrakNo + " programatik" : evrak.evrakNo.ToString()) + "<br>";
        //        else
        //            sonuc.hataStr += string.Format("{0} nolu evrak, imza yolu bilgisi tanımlı olmadığından imzalanamadı. ", evrak.evrakNo == 0 ? evrak.evrEvrakNo + " programatik" : evrak.evrakNo.ToString()) + "<br>";
        //    }
        //    else
        //    {
        //        switch (hareketDurum)
        //        {
        //            case ENUMEvrakHareketDurum.EIMZALANDI:
        //            case ENUMEvrakHareketDurum.EPARAFLANDI:
        //            case ENUMEvrakHareketDurum.IMZALANDI:
        //            case ENUMEvrakHareketDurum.PARAFLANDI:

        //                anahtarIslem = (int)ENUMEvrakHareketDurum.PARAFLANDI;

        //                bool hareketiAl = false;
        //                int index = 0;
        //                foreach (ImzaYoluDetay item in evrImzaYolu.imzaYolu.detaylar)
        //                {
        //                    if (hareketiAl)
        //                    {
        //                        sonrakiImzaYolu = item;
        //                        break;
        //                    }
        //                    else if (kullanan.mernis == item.tcKimlikNo)
        //                    {
        //                        if (evrak.imzaTarihi.Yil > 1 && (hareket.islem == (int)ENUMEvrakHareketDurum.IMZALANDI ||
        //                            hareket.islem == (int)ENUMEvrakHareketDurum.EIMZALANDI ||
        //                            hareket.islem == (int)ENUMEvrakHareketDurum.EPARAFLANDI ||
        //                            hareket.islem == (int)ENUMEvrakHareketDurum.PARAFLANDI) && hareket.kisi1 == kullanan.mernis)
        //                        {
        //                            sonuc.hataStr = "Seçilen evraklar, imza yolu bilgisine göre imzalanmış görünüyor. Lütfen işleminizi kontrol ediniz.";
        //                            break;
        //                        }

        //                        hareketiAl = true;
        //                        if ((hareketDurum == ENUMEvrakHareketDurum.IMZALANDI || hareketDurum == ENUMEvrakHareketDurum.EIMZALANDI) && item.islemTip != (int)ENUMImzaYoluTip.IMZA)
        //                        {
        //                            sonuc.hataStr = "Seçilen evraklar için, imza yolu bilgisine göre imzalama yetkiniz bulunmamaktadır.";
        //                            break;
        //                        }
        //                        else if ((hareketDurum == ENUMEvrakHareketDurum.PARAFLANDI || hareketDurum == ENUMEvrakHareketDurum.EPARAFLANDI) && item.islemTip != (int)ENUMImzaYoluTip.PARAF)
        //                        {
        //                            sonuc.hataStr = "Seçilen evraklar için, imza yolu bilgisine göre paraflama yetkiniz bulunmamaktadır.";
        //                            break;
        //                        }
        //                    }

        //                    if (hareketiAl && index == evrImzaYolu.imzaYolu.detaylar.Length - 1)
        //                    {
        //                        anahtarIslem = (int)ENUMEvrakHareketDurum.IMZALANDI;
        //                        sonrakiImzaYolu = item;
        //                    }

        //                    index++;
        //                }

        //                break;
        //            case ENUMEvrakHareketDurum.GERIGONDERILDI:
        //                anahtarIslem = (int)ENUMEvrakHareketDurum.GERIGONDERILDI;
        //                foreach (ImzaYoluDetay item in evrImzaYolu.imzaYolu.detaylar)
        //                {
        //                    if (item.tcKimlikNo == hareket.kisi1)
        //                    {
        //                        sonrakiImzaYolu = item;
        //                        break;
        //                    }
        //                }

        //                break;
        //        }

        //    }
        //    //***********************************

        //    if (!string.IsNullOrEmpty(sonuc.hataStr))
        //        sonuc.islemSonuc = false;

        //    if (sonuc.islemSonuc)
        //    {
        //        /* 
        //        * Sonraki imza yolu var mı? 
        //        * Varsa: sıradaki kişiye evrakImzaYolu Hareketini kaydet, daha sonra evrak'ı o kişiye havale et 
        //        */
        //        if (sonrakiImzaYolu != null && !string.IsNullOrEmpty(sonrakiImzaYolu.tcKimlikNo))
        //        {
        //            EvrakImzaYoluHareket evrImzaYoluHareket = new EvrakImzaYoluHareket()
        //            {
        //                prEvrakNo = evrak.prEvrakNo,
        //                kisi1 = kullanan.mernis,
        //                kisi2 = sonrakiImzaYolu.tcKimlikNo,
        //                islemTip = anahtarIslem,
        //                siraNo = sonrakiImzaYolu.siraNo,
        //                islemTarih = new TNSDateTime(DateTime.Now)
        //            };

        //            sonuc = EVREvrakImzaYolu.Kaydet(ve, kullanan, evrImzaYoluHareket);
        //            if (sonuc.islemSonuc)
        //                sonuc.anahtar = sonrakiImzaYolu.tcKimlikNo + ";" + sonrakiImzaYolu.evrakBirimKod + ";" + sonrakiImzaYolu.unvan;
        //        }
        //        else
        //            sonuc.hataStr += string.Format("{0} nolu evrak, için şeçilen imza yolu bilgisinde sıradaki kişinin eksik bilgileri var. ", evrak.evrakNo == 0 ? evrak.evrEvrakNo + " programatik" : evrak.evrakNo.ToString()) + "<br>";
        //    }


        //    if (!string.IsNullOrEmpty(sonuc.hataStr))
        //        sonuc.islemSonuc = false;


        //    //Parafla - Geri Gönder - İmzala
        //    /*ENUMEvrakHareketDurum  PARAFLANDI = 15 ; GERIGONDERILDI = 11 ; IMZALANDI = 8 */
        //    if (sonuc.islemSonuc)
        //    {
        //        string[] anahtarBilgi = sonuc.anahtar.Split(';');

        //        switch (anahtarIslem)
        //        {
        //            case (int)ENUMEvrakHareketDurum.EIMZALANDI:
        //            case (int)ENUMEvrakHareketDurum.IMZALANDI:
        //                break;
        //            case (int)ENUMEvrakHareketDurum.EPARAFLANDI:
        //            case (int)ENUMEvrakHareketDurum.PARAFLANDI:

        //                //tcKimlikNo, evrakBirimKod, unvan
        //                string[] havaleKisiBilgi = new string[] { anahtarBilgi[0], anahtarBilgi[1], anahtarBilgi[2] };

        //                string havaleKisiTcKimlikNo = havaleKisiBilgi[0];
        //                string havaleKisiAktifBirim = havaleKisiBilgi[1];
        //                string havaleKisiUnvan = havaleKisiBilgi[2];

        //                if (string.IsNullOrEmpty(havaleKisiTcKimlikNo))
        //                    sonuc.hataStr += "İmza yolu sırasına göre, sonraki havale edilecek kişinin TcKimlikNo bilgisine ulaşılamadı." + "<br>";

        //                if (string.IsNullOrEmpty(havaleKisiAktifBirim))
        //                    sonuc.hataStr += "İmza yolu sırasına göre, sonraki havale edilecek kişinin aktif birimi bulunamadı" + "<br>";

        //                //if (string.IsNullOrEmpty(havaleKisiUnvan))
        //                //    sonuc.hataStr += "Imza yolu sırasına göre, sonraki havale edilecek kişinin unvan bilgisi bulunamadı" + "<br>";

        //                if (string.IsNullOrEmpty(sonuc.hataStr))
        //                {
        //                    EvrakSinif havaleEvrak = new EvrakSinif();
        //                    havaleEvrak.prEvrakNo = evrak.prEvrakNo;
        //                    havaleEvrak.bulunduguYer = evrak.bulunduguYer;

        //                    EvrakHareket havaleHareket = new EvrakHareket();
        //                    havaleHareket.prEvrakNo = havaleEvrak.prEvrakNo;
        //                    havaleHareket.birimKod1 = havaleKisiAktifBirim;
        //                    havaleHareket.birimKod2 = havaleEvrak.bulunduguYer;
        //                    havaleHareket.kisi1 = havaleKisiTcKimlikNo;
        //                    havaleHareket.kisi2 = kullanan.mernis;
        //                    havaleHareket.havaleNotu = sonrakiImzaYolu.islemTip == (int)ENUMImzaYoluTip.PARAF ? "PARAF" : "";
        //                    havaleHareket.havaleTur = (int)ENUMEvrakHavaleTur.GEREGI;
        //                    havaleHareket.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
        //                    havaleHareket.vekil = evrak.hareket.vekil;
        //                    havaleHareket.vekilAd = evrak.hareket.vekilAd;

        //                    if (havaleHareket.kisi1 == kullanan.mernis)//Kendsine havale edemez
        //                        sonuc.hataStr = "Kişi kendisine havale edemez.";

        //                    if (havaleHareket.birimKod2 == havaleEvrak.bulunduguYer && havaleHareket.kisi2.Trim() == "") //Gelen evrak birimi ise kendi birimine havale edemez
        //                        sonuc.hataStr = havaleHareket.birimKod1 + " birimine havale edilemez.";

        //                    if (string.IsNullOrEmpty(sonuc.hataStr))
        //                        sonuc = EVREvrakHareket.Kaydet(ve, kullanan, havaleHareket);

        //                    if (sonuc.islemSonuc)
        //                    {
        //                        string havaleKisiAd = EVRGenel.KullaniciAdGetir(havaleKisiTcKimlikNo);//EVRServis.UzayYaziDegerGetir("KULLANICIAD", havaleKisiTcKimlikNo, true, "");
        //                        sonuc.bilgiStr = "Seçilen evraklar " + havaleKisiAktifBirim + " birimindeki " + havaleKisiAd + " isimli kişiye havale edildi.";
        //                    }
        //                }

        //                break;
        //            case (int)ENUMEvrakHareketDurum.GERIGONDERILDI:

        //                string[] geriGonderKisiBilgi = new string[] { anahtarBilgi[0], anahtarBilgi[1] };
        //                if (!string.IsNullOrEmpty(geriGonderKisiBilgi[0]))
        //                {
        //                    string havaleKisiAd2 = EVRGenel.KullaniciAdGetir(geriGonderKisiBilgi[0]);//EVRServis.UzayYaziDegerGetir("KULLANICIAD", geriGonderKisiBilgi[0], true, "");
        //                    sonuc.bilgiStr = "Seçilen evraklar " + geriGonderKisiBilgi[1] + " birimindeki " + havaleKisiAd2 + " isimli kişiye geri gönderildi.";
        //                }
        //                else
        //                    sonuc.hataStr = "Geri gönderilecek kişinin TcKimlikNo bilgisine ulaşılamadı.";

        //                break;
        //            default:
        //                sonuc.hataStr = "İmza yolu bilgisine göre, imzaya gönderme sırasında bilinmeyen hata oluştu.";
        //                break;
        //        }

        //    }

        //    return sonuc;
        //}

        /// <summary>
        /// Evrak işlem hareketinin uygunluğunu kontrol eden yordam.
        /// </summary>
        /// <param name="eski">Eski hareket işlem kodunu içeren nesne</param>
        /// <param name="yeni">Yeni hareket işlem kodunu içeren nesne</param>
        /// <returns>Evrak hareket geçişi uygun ise doğru, yoksa yanlış döndürülür.</returns>
        public static bool EvrakHareketGecisiUygunMu(int eski, int yeni)
        {

            bool deger = false;
            switch (eski)
            {
                case (int)ENUMEvrakHareketDurum.IMZALANDI:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.IMZAYAGONDER:
                        case (int)ENUMEvrakHareketDurum.IMZALANDI:
                        case (int)ENUMEvrakHareketDurum.EIMZALANDI:
                        case (int)ENUMEvrakHareketDurum.NUMARAALINDI:
                        case (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR:
                        case (int)ENUMEvrakHareketDurum.DAGITIMYAPILDI:
                        case (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN:
                        case (int)ENUMEvrakHareketDurum.IMZAPARAFKALDIRILDI:
                            deger = true;
                            break;
                    }
                    break;
                case (int)ENUMEvrakHareketDurum.EIMZALANDI:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.IMZAYAGONDER:
                        case (int)ENUMEvrakHareketDurum.EIMZALANDI:
                        case (int)ENUMEvrakHareketDurum.EPARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.PARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR:
                        case (int)ENUMEvrakHareketDurum.DAGITIMYAPILDI:
                        case (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN:
                        case (int)ENUMEvrakHareketDurum.IMZAPARAFKALDIRILDI:
                            deger = true;
                            break;
                    }
                    break;
                case (int)ENUMEvrakHareketDurum.PARAFLANDI:
                case (int)ENUMEvrakHareketDurum.EPARAFLANDI:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.IMZAYAGONDER:
                        case (int)ENUMEvrakHareketDurum.IMZALANDI:
                        case (int)ENUMEvrakHareketDurum.EIMZALANDI:
                        case (int)ENUMEvrakHareketDurum.EPARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.NUMARAALINDI:
                        case (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR:
                        case (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN:
                        case (int)ENUMEvrakHareketDurum.IMZAPARAFKALDIRILDI:
                            deger = true;
                            break;
                    }
                    break;
                case (int)ENUMEvrakHareketDurum.YENI:
                case (int)ENUMEvrakHareketDurum.TESLIMALDI:
                case (int)ENUMEvrakHareketDurum.DOSYADANALINDI:
                case (int)ENUMEvrakHareketDurum.IPTALGERIALINDI:
                case (int)ENUMEvrakHareketDurum.HAVALEGERIALINDI:
                case (int)ENUMEvrakHareketDurum.IADEEDILDI:
                case (int)ENUMEvrakHareketDurum.GERIGONDERILDI:
                case (int)ENUMEvrakHareketDurum.CEVAPLANDI:
                case (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN:
                case (int)ENUMEvrakHareketDurum.HAVALEGERIALINDIGIDEN:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.HAVALEEDILDI:
                        case (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN:
                        case (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR:
                        case (int)ENUMEvrakHareketDurum.IPTALEDILDI:
                        case (int)ENUMEvrakHareketDurum.NUMARAALINDI:
                        case (int)ENUMEvrakHareketDurum.IMZALANDI:
                        case (int)ENUMEvrakHareketDurum.PARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.EIMZALANDI:
                        case (int)ENUMEvrakHareketDurum.EPARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.IADEEDILDI:
                        case (int)ENUMEvrakHareketDurum.GERIGONDERILDI:
                        case (int)ENUMEvrakHareketDurum.DAGITIMYAPILDI:
                        case (int)ENUMEvrakHareketDurum.IMZAPARAFKALDIRILDI:
                        case (int)ENUMEvrakHareketDurum.HAVALEGERIALINDIGIDEN:
                            deger = true;
                            break;
                    }
                    break;
                case (int)ENUMEvrakHareketDurum.NUMARAALINDI:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.IMZALANDI:
                        case (int)ENUMEvrakHareketDurum.PARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.EIMZALANDI:
                        case (int)ENUMEvrakHareketDurum.EPARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.HAVALEEDILDI:
                        case (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN:
                        case (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR:
                        case (int)ENUMEvrakHareketDurum.IPTALEDILDI:
                        case (int)ENUMEvrakHareketDurum.DAGITIMYAPILDI:
                        case (int)ENUMEvrakHareketDurum.IMZAPARAFKALDIRILDI:
                            deger = true;
                            break;
                    }
                    break;
                case (int)ENUMEvrakHareketDurum.HAVALEEDILDI:
                case (int)ENUMEvrakHareketDurum.DAGITIMKAYIT:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.TESLIMALDI:
                        case (int)ENUMEvrakHareketDurum.HAVALEGERIALINDI:
                        case (int)ENUMEvrakHareketDurum.IADEEDILDI:
                        case (int)ENUMEvrakHareketDurum.GERIGONDERILDI:
                        case (int)ENUMEvrakHareketDurum.ARSIVEKALDIR:
                            deger = true;
                            break;
                    }
                    break;
                case (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.DOSYADANALINDI:
                            deger = true;
                            break;
                    }
                    break;
                case (int)ENUMEvrakHareketDurum.IPTALEDILDI:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.IPTALGERIALINDI:
                            deger = true;
                            break;
                    }
                    break;
                case (int)ENUMEvrakHareketDurum.IMZAYAGONDER:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.IMZALANDI:
                        case (int)ENUMEvrakHareketDurum.PARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.EIMZALANDI:
                        case (int)ENUMEvrakHareketDurum.EPARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.GERIGONDERILDI:
                        case (int)ENUMEvrakHareketDurum.NUMARAALINDI:
                        case (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN:
                        case (int)ENUMEvrakHareketDurum.IMZAPARAFKALDIRILDI:
                        case (int)ENUMEvrakHareketDurum.IMZADANGERIALINDI:
                            deger = true;
                            break;
                    }
                    break;
                case (int)ENUMEvrakHareketDurum.IMZAPARAFKALDIRILDI:
                case (int)ENUMEvrakHareketDurum.IMZADANGERIALINDI:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN:
                        case (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR:
                        case (int)ENUMEvrakHareketDurum.NUMARAALINDI:
                        case (int)ENUMEvrakHareketDurum.IMZALANDI:
                        case (int)ENUMEvrakHareketDurum.PARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.EIMZALANDI:
                        case (int)ENUMEvrakHareketDurum.EPARAFLANDI:
                        case (int)ENUMEvrakHareketDurum.DAGITIMYAPILDI:
                            deger = true;
                            break;
                    }
                    break;
                case (int)ENUMEvrakHareketDurum.DAGITIMYAPILDI:
                case (int)ENUMEvrakHareketDurum.KEPILEGONDERILDI:
                    switch (yeni)
                    {
                        case (int)ENUMEvrakHareketDurum.DAGITIMYAPILDI:
                        case (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR:
                        case (int)ENUMEvrakHareketDurum.KEPILEGONDERILDI:
                            deger = true;
                            break;
                    }
                    break;

            }
            return deger;
        }

        /// <summary>
        /// Parametre olarak verilen giden evrak için evrak numarası oluşturan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc GidenEvrakNumaraVer(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, bool yetkiKontrolEt)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string hata = "";

            if (yetkiKontrolEt)
                foreach (EvrakSinif evrak in liste.objeler)
                {
                    bool numaraAlabilir = EvrakNumarasiAlabilirMi(kullanan, evrak.bulunduguYer);

                    if (!numaraAlabilir)
                    {
                        sonuc.hataStr = "Evrak numarası almaya yetkiniz bulunmmamaktadır.";
                        return sonuc;
                    }
                }

            //Yetki kontrolü yapılmalımı?
            //int yetki = Yetki.KullaniciEvrakYetkiSablonu(kullanan);
            //if ((!(yetki == (int)ENUMEvrakYetkiSablonu.EVRAKKULLANICISI)) && 
            //    (!(yetki == (int)ENUMEvrakYetkiSablonu.EVRAKYETKILISI || yetki == (int)ENUMEvrakYetkiSablonu.EVRAKMEMURU)))
            //{
            //    sonuc.hataStr = EvrakI.EVRI074;
            //    return sonuc;
            //}


            if (yetkiKontrolEt)
                if (sonuc.hataStr == "")
                {
                    foreach (EvrakSinif evrak in liste.objeler)
                    {

                        //Evrak Birimi açık mı?
                        if (!EvrakBirimiAcikMi(veriErisim, evrak.bulunduguYer))
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = string.Format(EvrakI.EVRI053, evrak.bulunduguYer) + "<br>";
                            return sonuc;
                        }

                        //YazmaYetkisiVarMi var mı?
                        bool uygun = TNS.EVR.Yetki.YazmaYetkisiVarMi(kullanan, evrak);
                        if (!uygun)
                        {
                            sonuc.hataStr = EvrakI.EVRI041;
                            sonuc.islemSonuc = false;
                            return sonuc;
                        }

                    }
                }

            string sayacBaslangicBilgisiAl = EVREvrak.EvrakNoSayacBaslangicBilgisiAl();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);
            TABGenel tabGenel = new TABGenel(ve);
            int basarili = 0;
            string evrakNoList = "";

            ve.BeginTransaction();
            try
            {
                foreach (EvrakSinif evrak in liste.objeler)
                {
                    if (evrak.evrakNo > 0) continue;

                    EvrakSinif bilgi = new EvrakSinif();
                    bilgi.evrEvrakNo = evrak.evrEvrakNo;
                    bilgi.prEvrakNo = evrak.prEvrakNo;
                    if (yetkiKontrolEt)
                        bilgi.bulunduguYer = evrak.bulunduguYer;
                    bilgi.evrakTipi = evrak.evrakTipi;

                    if (EvrakKendiBirimindeMi(ve, kullanan, bilgi, false))
                    {
                        ObjectArray evraklar = Listele(ve, kullanan, bilgi);
                        if (evraklar.ObjeSayisi > 0)
                        {
                            bilgi = (EvrakSinif)evraklar.objeler[0];

                            sonuc.islemSonuc = EvrakHareketGecisiUygunMu(bilgi.hareket.islem, (int)ENUMEvrakHareketDurum.NUMARAALINDI);

                            if (!sonuc.islemSonuc)
                                hata += string.Format(EvrakI.EVRI075, bilgi.konu) + "<br>";

                            if (bilgi.evrakNo == 0 && bilgi.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                            {
                                string sayacKod = "EVRGIDEN" + sayacBaslangicBilgisiAl + EVRBirim.EvrakKokBirimGetir(ve, bilgi.evrakBirimKodu);
                                bilgi.evrakNo = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "0000000"), 0);

                                EvrakSinif evrakKaydet = new EvrakSinif();
                                evrakKaydet.evrEvrakNo = bilgi.evrEvrakNo;
                                evrakKaydet.evrakNo = bilgi.evrakNo;
                                evrakKaydet.evrakTarih = new TNSDateTime(DateTime.Now);
                                evrakKaydet.yil = DateTime.Now.Year;
                                sonuc = tab.Guncelle(evrakKaydet);
                                if (sonuc.islemSonuc)
                                {
                                    EvrakHareket hareket = new EvrakHareket();
                                    hareket.prEvrakNo = bilgi.prEvrakNo;
                                    hareket.birimKod1 = bilgi.hareket.birimKod1;
                                    if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, evrak.bulunduguYer) || bilgi.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                                        hareket.kisi1 = kullanan.mernis;
                                    hareket.islem = (int)ENUMEvrakHareketDurum.NUMARAALINDI;

                                    hareket.vekil = evrak.hareket.vekil;
                                    hareket.vekilAd = evrak.hareket.vekilAd;

                                    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                                    if (sonuc.islemSonuc)
                                    {

                                        //Sıra Numarası - Hüseyin  (genelge numarası)
                                        if (evrak.siraNo == 0 && evrak.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                                        {
                                            //string belgeTur = "";
                                            //if (evrak.evrakTuru == (int)ENUMGidenEvrakTuru.DISYAZISMA)
                                            //    belgeTur = "DY";
                                            //else if (evrak.evrakTuru == (int)ENUMGidenEvrakTuru.KURUMICYAZISMA)
                                            //    belgeTur = "IY";
                                            //else if (evrak.evrakTuru == (int)ENUMGidenEvrakTuru.DISGENELGE)
                                            //    belgeTur = "DG";
                                            //else if (evrak.evrakTuru == (int)ENUMGidenEvrakTuru.ICGENELGE)
                                            //    belgeTur = "IG";
                                            //else if (evrak.evrakTuru == (int)ENUMGidenEvrakTuru.BASKANLIKMAKAMONAYI)
                                            //    belgeTur = "BASOY";
                                            //else if (evrak.evrakTuru == (int)ENUMGidenEvrakTuru.BAKANLIKMAKAMONAYI)
                                            //    belgeTur = "BAKOY";
                                            //else if (evrak.evrakTuru == (int)ENUMGidenEvrakTuru.BIRIMICYAZISMA)
                                            //    belgeTur = "BIY";
                                            //else if (evrak.evrakTuru == (int)ENUMGidenEvrakTuru.GOREVLENDIRME)
                                            //    belgeTur = "GRV";

                                            string belgeTur = evrak.evrakTuru.ToString("0000");

                                            if (!string.IsNullOrEmpty(belgeTur))
                                            {
                                                sayacKod = "EVRGIDEN" + belgeTur + DateTime.Now.Year + EVRBirim.EvrakKokBirimGetir(ve, evrak.evrakBirimKodu);
                                                evrak.siraNo = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "0000000"), 0);

                                                evrakKaydet = new EvrakSinif();
                                                evrakKaydet.evrEvrakNo = bilgi.evrEvrakNo;
                                                evrakKaydet.siraNo = evrak.siraNo;
                                                sonuc = tab.Guncelle(evrakKaydet);
                                            }
                                        }
                                        //******************************************


                                    }
                                    else
                                        Arac.HataStrYaz("No alma hareket kayıt edilemedi:" + sonuc.hataStr);

                                    basarili++;
                                    evrakNoList += bilgi.prEvrakNo + ":" + bilgi.evrakNo + "-";
                                }
                            }
                        }
                        else
                            hata += string.Format(EvrakI.EVRI059, bilgi.prEvrakNo + "*" + bilgi.evrEvrakNo) + "<br>";
                    }
                    else
                    {
                        ObjectArray evraklar = Listele(ve, kullanan, bilgi);
                        if (evraklar.ObjeSayisi > 0)
                        {
                            bilgi = (EvrakSinif)evraklar.objeler[0];
                            hata += string.Format(EvrakI.EVRI077, bilgi.konu) + "<br>";
                        }
                        else
                            hata += string.Format(EvrakI.EVRI059, bilgi.prEvrakNo + "*" + bilgi.evrEvrakNo) + "<br>";
                    }
                }

                if (hata != "")
                    sonuc.hataStr = hata;

                if (sonuc.hataStr == "")
                    sonuc.islemSonuc = true;
                else
                    sonuc.islemSonuc = false;

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                {
                    ve.Rollback();
                    evrakNoList = "";
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{62B28AC9-53C8-497C-8C0A-3940ECF3F724} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            sonuc.anahtar = evrakNoList;
            return sonuc;
        }

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak konu bilgisini EvrakKonu nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="konu">Evrak konu biglisini içeren nesne</param>
        /// <param name="kullanimSayisi">Kullanım sayısını içeren nesne</param>
        /// <returns>
        /// EvrakKonu nesne koleksiyonunu olarak döndürülür.
        /// </returns>
        public static ObjectArray KonuListele(VeriErisim veriErisim, string birim, string konu, int kullanimSayisi)
        {
            ObjectArray deger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            birim = EVRBirim.EvrakKokBirimGetir(ve, birim);

            TABEvrak to = new TABEvrak(ve);
            IDataReader r = to.KonuListele(birim, konu, kullanimSayisi);

            if (r != null)
            {
                while (r.Read())
                {
                    EvrakKonu eKonu = new EvrakKonu();
                    eKonu.birim = VeriErisim.DegerAl(r, 0, "");
                    eKonu.konu = VeriErisim.DegerAl(r, 1, "");
                    eKonu.kullanimSayisi = VeriErisim.DegerAl(r, 2, 0);

                    deger.objeler.Add(eKonu);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return deger;
        }

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak konu bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="konu">Evrak konu biglisini içeren nesne</param>
        /// <param name="kalici">Kalıcı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc KonuKaydet(VeriErisim veriErisim, string birim, string konu, int kalici)
        {
            Sonuc sonuc = new Sonuc();
            if (birim.Trim() == "" || konu.Trim() == "")
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            sonuc = to.KonuKaydet(birim, konu, kalici);

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen birim ve konu bilgisini evrak konu bilgisinin tutulduğu veritabanı bilgisini silen yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="birim">Birim bilgisini içeren yordam</param>
        /// <param name="konu">Konu bilgisini içeren yordam</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public static Sonuc KonuSil(VeriErisim veriErisim, string birim, string konu)
        {
            Sonuc sonuc = new Sonuc();
            if (birim.Trim() == "")
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            if (konu.Trim() != "")
                sonuc = to.KonuSil(birim, konu);

            ve.VeriErisimKapat();

            return sonuc;
        }


        public static ObjectArray MetaGrupListele(VeriErisim veriErisim, string birim, int kod, string adi)
        {
            ObjectArray deger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            birim = EVRBirim.EvrakKokBirimGetir(ve, birim);

            TABEvrak to = new TABEvrak(ve);
            IDataReader r = to.MetaGrupListele(birim, kod, adi);

            if (r != null)
            {
                while (r.Read())
                {
                    MetaGrup mg = new MetaGrup();
                    mg.birim = VeriErisim.DegerAl(r, 0, "");
                    mg.kod = VeriErisim.DegerAl(r, 1, 0);
                    mg.adi = VeriErisim.DegerAl(r, 2, "");

                    deger.objeler.Add(mg);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return deger;
        }

        public static Sonuc MetaGrupKaydet(VeriErisim veriErisim, string birim, int kod, string adi)
        {
            Sonuc sonuc = new Sonuc();
            if (birim.Trim() == "" || adi.Trim() == "")
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);

            if (kod == 0)
            {
                TABGenel tabGenel = new TABGenel(ve);
                kod = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl("METAGRUP", "0"), 0);
            }
            sonuc = to.MetaGrupKaydet(birim, kod, adi);

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc MetaGrupSil(VeriErisim veriErisim, string birim, int kod)
        {
            Sonuc sonuc = new Sonuc();
            if (birim.Trim() == "")
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            if (kod > 0)
                sonuc = to.MetaGrupSil(birim, kod);

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak yer bilgisini EvrakYer nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer biglisini içeren nesne</param>
        /// <param name="tur">Yer türü bilgisini içeren nesne</param>
        /// <param name="kullanimSayisi">Kullanım sayısını içeren nesne</param>
        /// <returns>EvrakYer nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray YerIsimleriListele(VeriErisim veriErisim, string birim, string yer, int tur, int kullanimSayisi)
        {
            ObjectArray deger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            birim = EVRBirim.EvrakKokBirimGetir(ve, birim);

            TABEvrak to = new TABEvrak(ve);
            IDataReader r = to.YerIsimleriListele(birim, yer, tur, kullanimSayisi);

            if (r != null)
            {
                while (r.Read())
                {
                    EvrakYer eYer = new EvrakYer();
                    eYer.birim = VeriErisim.DegerAl(r, 0, "");
                    eYer.yerAd = VeriErisim.DegerAl(r, 1, "");
                    eYer.yerTur = VeriErisim.DegerAl(r, 2, 0);
                    eYer.yerBirimKod = VeriErisim.DegerAl(r, 3, "");
                    eYer.kullanimSayisi = VeriErisim.DegerAl(r, 4, 0);

                    deger.objeler.Add(eYer);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return deger;
        }

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak yer bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer biglisini içeren nesne</param>
        /// <param name="tur">Yer türü bilgisini içeren nesne</param>
        /// <param name="yerBirimKodu">Yer birim kodu bilgisini içeren nesne</param>
        /// <param name="kalici">Kalıcı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc YerIsimleriKaydet(VeriErisim veriErisim, string birim, string yer, int tur, string yerBirimKodu, int kalici)
        {
            Sonuc sonuc = new Sonuc();
            if (birim.Trim() == "" || yer.Trim() == "" || tur == 0)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            sonuc = to.YerIsimleriKaydet(birim, yer, tur, yerBirimKodu, kalici);

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak yer bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer biglisini içeren nesne</param>
        /// <param name="tur">Yer türü bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc YerIsimleriSil(VeriErisim veriErisim, string birim, string yer, int tur)
        {
            Sonuc sonuc = new Sonuc();
            if (birim.Trim() == "")
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            if (yer.Trim() != "")
                sonuc = to.YerIsimleriSil(birim, yer, tur);

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak sicil bigisini veritabanına kaydeden yordam
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak sicil bilgisini içeren nesne</param>
        /// <returns></returns>
        public static Sonuc KaydetSicil(VeriErisim veriErisim, Kullanici kullanan, EvrakSicil form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI079, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrak to = new TABEvrak(ve);

                sonuc = to.KaydetSicil(form);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{95E929D5-A28E-4B33-8001-55DE6CE2883F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Veritabanına bakarak yeni bir prEvrakNo oluşturan yordam.
        /// </summary>
        /// <param name="ve">Veri erişim bilgisini içeren nesne</param>
        /// <returns>prEvrakNo string tipine nesne olarak döndürülür.</returns>
        public static string PrEvrakNoAl(VeriErisim ve)
        {
            TABGenel tabGenel = new TABGenel(ve);
            string sayacKod = "PREVRAKNO" + DateTime.Now.Year;
            return DateTime.Now.Year + tabGenel.BelgeNoAl(sayacKod, "0000000");
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, ilişkili evrakların evrEvrakNo ve prEvrakNo bilgileri metin dizisi olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="iliskiprEvrakNo">İlişki prEvrakNo bilgisini içeren nesne</param>
        /// <param name="iliskiTip">Evrak ilişki tipini içeren nese</param>
        /// <returns>Evrak ilişki bilgileri metin dizisi olarak döndürülür.</returns>
        public static string[] EvrakIliskidenEvrVePrEvrakNoVer(VeriErisim veriErisim, string iliskiprEvrakNo, int iliskiTip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            IDataReader r = to.EvrakIliskidenEvrVePrEvrakNoVer(iliskiprEvrakNo, iliskiTip);
            string[] iliskiEvrVePrEvrakNo = null;
            if (r != null)
            {
                if (r.Read())
                {
                    iliskiEvrVePrEvrakNo = new string[2];
                    iliskiEvrVePrEvrakNo[0] = VeriErisim.DegerAl(r, 0, "");
                    iliskiEvrVePrEvrakNo[1] = VeriErisim.DegerAl(r, 1, "");
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return iliskiEvrVePrEvrakNo;
        }

        /// <summary>
        /// Paramete olarak verilen prEvrakNo nesnesine göre, evrak'ın evrEvrakNo, evrakTarih ve evrakNo bilgisini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakSinifKucuk nesesi olarak döndürülür.</returns>
        public static EvrakSinifKucuk EvrakNoVeTarihVer(VeriErisim veriErisim, string prEvrakNo)
        {
            EvrakSinifKucuk donenDeger = new EvrakSinifKucuk();
            if (string.IsNullOrEmpty(prEvrakNo))
                return donenDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            IDataReader r = to.EvrakNoVeTarihVer(prEvrakNo);

            if (r != null)
            {
                if (r.Read())
                {
                    donenDeger.evrakNo = VeriErisim.DegerAl(r, 0, 0); //EVRAKNO
                    donenDeger.evrakTarih = VeriErisim.DegerAl(r, 1); //EVRAKTARIH
                    donenDeger.evrEvrakNo = VeriErisim.DegerAl(r, 2, ""); //EVREVRAKNO
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return donenDeger;
        }

        /// <summary>
        /// Paramete olarak verilen prEvrakNo nesnesine göre, evrak'ın ilişkili olduğu diğer evrakları EvrakSinif nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakSinif nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray ListeleIliskiliEvraklari(ref VeriErisim ve, string prEvrakNo)
        {
            ObjectArray hVAN = new ObjectArray();

            if (prEvrakNo.Trim() == "")
            {
                hVAN.sonuc.islemSonuc = false;
                hVAN.sonuc.hataStr = EvrakI.EVRI080;
                return hVAN;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);

            string[] prEvrakNolar = new string[1000];
            int ii = 0;

            //Geçici çözüm. Daha sonra iyileştirilecek. H.Ö. 02.01.2013 
            bool islemSonuc = Listele_IliskiliEvraklar(to, prEvrakNo, 1, ref prEvrakNolar, ref ii);
            if (islemSonuc)
                islemSonuc = Listele_IliskiliEvraklar(to, prEvrakNo, 2, ref prEvrakNolar, ref ii);
            if (islemSonuc)
                islemSonuc = Listele_IliskiliEvraklar(to, prEvrakNo, 3, ref prEvrakNolar, ref ii);


            IDataReader r = to.ListeleIliskiliEvraklarDetay(prEvrakNolar);
            if (r != null)
            {
                ListeleIliskiliEvraklariDoldur(r, ref hVAN);
                r.Close();
            }

            ve.VeriErisimKapat();

            //Tarihe göre Sırala
            hVAN.objeler.Sort(new Sirala());


            return hVAN;
        }
        public static bool Listele_IliskiliEvraklar(TABEvrak to, string prEvrakNo, int tip, ref string[] prEvrakNolar, ref int ii)
        {
            bool kendisiniEkleme = true;
            IDataReader r = to.ListeleIliskiliEvraklar(prEvrakNo, tip);
            if (r != null)
            {
                while (r.Read())
                {
                    string prNo = VeriErisim.DegerAl(r, 0, "");
                    if (kendisiniEkleme && prNo == prEvrakNo)
                        continue;

                    bool yeni = true;
                    if (prEvrakNolar.Length > 0)
                        foreach (var item in prEvrakNolar)
                            if (string.IsNullOrEmpty(item))
                                break;
                            else if (item == prNo)
                            {
                                yeni = false;
                                break;
                            }

                    if (yeni)
                        prEvrakNolar[ii] = prNo;
                    ii++;
                }
                r.Close();
            }

            return true;
        }

        /// <summary>
        /// Parametre olarak verilen dataReader arayüz nesnesi ile verilen evrak bilgilerini kendi arasında karşılaştıran ve ilişkili evrakları bulan yordam.
        /// </summary>
        /// <param name="r">DataReader evrak bilgisi içeren arayüz</param>
        /// <param name="hVAN">İlişkili evrakların bilgisini içeren nesne</param>
        static void ListeleIliskiliEvraklariDoldur(IDataReader r, ref ObjectArray hVAN)
        {
            while (r.Read())
            {
                int index = 1;

                EvrakSinif form = new EvrakSinif();
                form.prEvrakNo = VeriErisim.DegerAl(r, 0, "");
                form.evrEvrakNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVREVRAKNO + index, "");
                form.yil = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.YIL + index, 0);
                form.evrakTipi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKTIPI + index, 0);
                form.evrakBirimKodu = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKBIRIMKODU + index, "");
                form.evrakNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKNO + index, 0);
                form.evrakTarih = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKTARIH + index);
                form.geldigiYerTur = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELDIGIYERTUR + index, 0);
                form.geldigiYer = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELDIGIYER + index, "");
                form.gelenEvrakNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELENEVRAKNO + index, "");
                form.gelenEvrakTarih = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELENEVRAKTARIH + index);
                form.konu = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.KONU + index, "");
                form.gizlilik = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GIZLILIK + index, 0);
                form.gelisSekli = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELISSEKLI + index, 0);
                form.yaziDurum = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.YAZIDURUM + index, 0);
                form.cevapSuresi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.CEVAPSURESI + index, 0);
                form.aciklama = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ACIKLAMA + index, "");
                form.ekAdedi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EKADEDI + index, "");
                form.havaleBirimKodu = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.HAVALEBIRIMKODU + index, "");
                form.evrakTuru = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKTURU + index, 0);
                form.paraf = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.PARAF + index, "");
                form.antet = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ANTET + index, "");
                form.icerik = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ICERIK + index, "");
                form.imza1 = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMZA1 + index, "");
                form.imza2 = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMZA2 + index, "");
                form.imza3 = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMZA3 + index, "");
                form.ek = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EK + index, "");
                form.altBilgi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.ALTBILGI + index, "");
                form.evrakOlusturmaTarihi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.OLUSTURMATARIHI + index);
                form.imzaTarihi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.IMZATARIHI + index);
                form.ekYaziAlan = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EKYAZIALAN + index, "");
                form.siraNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.SIRANO + index, 0);
                form.dosyaNo = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.DOSYANO + index, "");
                form.evrakIcerigi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKICERIK + index, "");
                form.evrakCinsi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.EVRAKCINSI + index, 0);

                bool ekle = true;
                foreach (EvrakSinif f in hVAN.objeler)
                {
                    if (form.prEvrakNo == f.prEvrakNo)
                    {
                        ekle = false;
                        break;
                    }
                }
                if (ekle)
                    hVAN.objeler.Add(form);
            }
        }

        /// <summary>
        /// Parametre olarak verilen evrak EvrakIliski nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="evrakIliskiArr">Evrak ilişki bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc EvrakIliskiKaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray evrakIliskiArr)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI160, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrak to = new TABEvrak(ve);

                foreach (EvrakIliski form in evrakIliskiArr.objeler)
                {
                    sonuc = to.IliskiKaydet(form);

                    if (sonuc.islemSonuc)
                    {
                        //Not: geçici olarak bu yöntem seçildi. Daha sonra parametre olarak geçirilecek. Hüseyi. [GÇ]
                        string evrEvrakNo = to.EvrEvrakNoGetir(form.prEvrakNo);

                        EvrakTarihce tarihce = new EvrakTarihce();
                        tarihce.evrEvrakNo = evrEvrakNo;
                        tarihce.islem = string.Format("{0} ile {1} referans numaralı evrak arasında ilişki kuruldu.", form.prEvrakNo, form.prEvrakNoIliski);

                        sonuc = EVREvrakTarihce.Kaydet(ve, kullanan, tarihce);
                    }

                    if (!sonuc.islemSonuc)
                    {
                        ve.hata = "";
                        sonuc.islemSonuc = true;
                    }
                }

                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = EvrakI.EVRI162;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = "Seçilen evrakın ilişkisi zaten var";
                    ve.Rollback();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{026576C7-EDD4-471A-A3B1-23BA5A91F723} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak EvrakIliski nesne koleksiyonunu bilgilerini veritabanından silen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="evrakIliskiArr">Evrak ilişki bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc EvrakIliskiSil(VeriErisim veriErisim, Kullanici kullanan, ObjectArray evrakIliskiArr)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI161, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrak to = new TABEvrak(ve);

                foreach (EvrakIliski form in evrakIliskiArr.objeler)
                {
                    sonuc = to.IliskiSil(form);

                    if (sonuc.islemSonuc)
                    {
                        //Not: geçici olarak bu yöntem seçildi. Daha sonra parametre olarak geçirilecek. Hüseyi. [GÇ]
                        string evrEvrakNo = to.EvrEvrakNoGetir(form.prEvrakNo);

                        EvrakTarihce tarihce = new EvrakTarihce();
                        tarihce.evrEvrakNo = evrEvrakNo;
                        tarihce.islem = string.Format("{0} ile {1} referans numaralı evrak arasındaki ilişki kaldırıldı.", form.prEvrakNo, form.prEvrakNoIliski);

                        sonuc = EVREvrakTarihce.Kaydet(ve, kullanan, tarihce);
                    }

                    if (!sonuc.islemSonuc)
                        break;
                }

                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = EvrakI.EVRI163;
                    ve.Commit();
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{FB5C9281-C861-497E-AB71-4397ACF245FD} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// EvrakSinif nesne koleksiyonundaki evrakları sıralayan yordamı tutan sınıf.
        /// </summary>
        public class Sirala : System.Collections.IComparer
        {
            /// <summary>
            /// EvrakSinif nesne koleksiyonundaki evrakları evrak tarihlerine göre sıralama işlemini yapan yordam.
            /// </summary>
            /// <param name="o1">İlk evrak bilgisi içeren nesne</param>
            /// <param name="o2">İkinci evrak bilgisi içeren nesne</param>
            /// <returns>Sonuç 0, 1 veya -1 olarak döndürülür.</returns>
            public int Compare(object o1, object o2)
            {
                if (o1 == o2) return 0;
                EvrakSinif i_o1 = (EvrakSinif)o1;
                EvrakSinif i_o2 = (EvrakSinif)o2;
                return (i_o1.evrakTarih.Oku()).CompareTo((i_o2.evrakTarih.Oku()));
            }
        }

        public static bool YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu islem)
        {
            string bilgi = System.Configuration.ConfigurationManager.AppSettings.Get(islem.ToString()) + "";
            return bilgi == "1" ? true : false;
        }
        public static bool YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu islem, string birimKod)
        {
            bool yapilabilir = false;

            string birimBilgi = System.Configuration.ConfigurationManager.AppSettings.Get(islem.ToString()) + "";
            if (!string.IsNullOrEmpty(birimBilgi))
            {
                string[] birimListe = birimBilgi.TrimEnd(';').Split(';');
                foreach (string birim in birimListe)
                    if (!string.IsNullOrEmpty(birim) && birimKod.StartsWith(birim))
                        return true;
            }

            return yapilabilir;
        }


        //public static ObjectArray KisiUzerindekileriListele(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        //{
        //    ObjectArray liste = new ObjectArray();

        //    //Kullanan yetki kontrolü

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
        //    TABEvrak to = new TABEvrak(ve);
        //    string tcKimlikNo;
        //    tcKimlikNo = form.hareket.kisi2;
        //    IDataReader r = to.KisiUzerindekileriListele(form);

        //    if (r != null)
        //    {
        //        while (r.Read())
        //        {
        //            //EvrakSinif form = new EvrakSinif();
        //            form = new EvrakSinif();
        //            form.yil = VeriErisim.DegerAl(r, 0, 0);
        //            form.evrEvrakNo = VeriErisim.DegerAl(r, 1, "");
        //            form.evrakTipi = VeriErisim.DegerAl(r, 2, 0);
        //            form.evrakNo = VeriErisim.DegerAl(r, 3, 0);
        //            form.evrakTarih = VeriErisim.DegerAl(r, 4);
        //            form.geldigiYer = VeriErisim.DegerAl(r, 5, "");
        //            form.konu = VeriErisim.DegerAl(r, 6, "");
        //            form.cevapSuresi = VeriErisim.DegerAl(r, 7, 0);
        //            form.hareket.kisi1Ad = VeriErisim.DegerAl(r, 8, "");
        //            form.hareket.kisi2Ad = VeriErisim.DegerAl(r, 9, "");
        //            form.hareket.islemYapanAd = VeriErisim.DegerAl(r, 10, "");
        //            form.hareket.islem = VeriErisim.DegerAl(r, 11, 0);
        //            form.hareket.islemTarih = VeriErisim.DegerAl(r, 12);
        //            form.hareket.havaleTur = VeriErisim.DegerAl(r, 13, 0);

        //            liste.objeler.Add(form);
        //        }
        //        r.Close();
        //    }

        //    ve.VeriErisimKapat();
        //    return liste;
        //}

        public static bool EvrakOlaySorgu(VeriErisim veriErisim, string prEvrakNo, string evrEvrakNo, ENUMOlaySorguTip olaySorgu)
        {
            switch (olaySorgu)
            {
                case ENUMOlaySorguTip.DOSYAYAKALDIRILMISMI:

                    EvrakHareket evrHareket = new EvrakHareket();
                    evrHareket.prEvrakNo = prEvrakNo;
                    ObjectArray listeHareket = EVREvrakHareket.Listele(veriErisim, evrHareket);
                    foreach (EvrakHareket h in listeHareket.objeler)
                        if (h.islem == (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR)
                            return true;
                    break;
            }

            return false;
        }

        public static string EvrakNoSayacBaslangicBilgisiAl()
        {
            string evrSayacSifirlanmaYil = System.Configuration.ConfigurationManager.AppSettings.Get("EvrakNoSayacSifirlanmaYil");
            if (!string.IsNullOrWhiteSpace(evrSayacSifirlanmaYil))
                return evrSayacSifirlanmaYil.Trim();

            return DateTime.Now.Year.ToString();
        }

        public static EvrakImha[] EvrakImhaListele(VeriErisim veriErisim, TNSDateTime baslangicTarihi, TNSDateTime bitisTarihi)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            IDataReader dr = tab.EvrakImhaListele(baslangicTarihi, bitisTarihi);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int index = 0;
                    EvrakImha form = new EvrakImha();
                    form.prEvrakNo = VeriErisim.DegerAl(dr, index, ""); index++;
                    form.evrEvrakNo = VeriErisim.DegerAl(dr, index, ""); index++;
                    form.yil = VeriErisim.DegerAl(dr, index, 0); index++;
                    form.evrakTip = VeriErisim.DegerAl(dr, index, 0); index++;
                    form.evrakNo = VeriErisim.DegerAl(dr, index, 0); index++;
                    form.evrakTarih = VeriErisim.DegerAl(dr, index); index++;
                    form.geldigiYer = VeriErisim.DegerAl(dr, index, ""); index++;
                    form.konu = VeriErisim.DegerAl(dr, index, ""); index++;
                    form.gelenEvrakTarih = VeriErisim.DegerAl(dr, index); index++;
                    form.gelenEvrakNo = VeriErisim.DegerAl(dr, index, 0); index++;
                    form.saklamaSuresiYil = VeriErisim.DegerAl(dr, index, 0); index++;
                    form.saklamaSuresiBitisTarih = VeriErisim.DegerAl(dr, index); index++;
                    form.dosyaPlanKod = VeriErisim.DegerAl(dr, index, ""); index++;
                    form.dosyaPlanAd = VeriErisim.DegerAl(dr, index, ""); index++;

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakImha[])liste.objeler.ToArray(typeof(EvrakImha));
        }

        public static EvrakTasfiyeImha[] EvrakTasfiyeImhaListele(VeriErisim veriErisim, TNSDateTime baslangicTarihi, TNSDateTime bitisTarihi, EvrakTasfiyeImha form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            IDataReader dr = tab.EvrakTasfiyeImhaListele(baslangicTarihi, bitisTarihi, form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int index = 0;
                    form = new EvrakTasfiyeImha();
                    form.evrakBirimKod = VeriErisim.DegerAl(dr, index++, "");
                    form.evrEvrakNo = VeriErisim.DegerAl(dr, index++, "");
                    form.prEvrakNo = VeriErisim.DegerAl(dr, index++, "");
                    form.yil = VeriErisim.DegerAl(dr, index++, 0);
                    form.evrakTip = VeriErisim.DegerAl(dr, index++, 0);
                    form.evrakNo = VeriErisim.DegerAl(dr, index++, 0);
                    form.evrakTarih = VeriErisim.DegerAl(dr, index++);
                    form.geldigiYer = VeriErisim.DegerAl(dr, index++, "");
                    form.konu = VeriErisim.DegerAl(dr, index++, "");
                    form.saklamaPlani.bitisTarih = VeriErisim.DegerAl(dr, index++);
                    form.saklamaPlani.raporTarih = VeriErisim.DegerAl(dr, index++);
                    form.saklamaPlani.tasfiyePlani = VeriErisim.DegerAl(dr, index++, 0);
                    form.saklamaPlani.islem = VeriErisim.DegerAl(dr, index++, 0);
                    form.saklamaPlani.islemTarih = VeriErisim.DegerAl(dr, index++);
                    form.fizikselYeri = VeriErisim.DegerAl(dr, index++, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakTasfiyeImha[])liste.objeler.ToArray(typeof(EvrakTasfiyeImha));
        }

        public static int EvrakTasfiyeImhaZamaniGelenEvrakSayisi(VeriErisim veriErisim, string evrakBirimKod, TNSDateTime bitisTarihi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);

            int sayi = to.EvrakTasfiyeImhaZamaniGelenEvrakSayisi(evrakBirimKod, bitisTarihi);

            ve.VeriErisimKapat();
            return sayi;
        }

        public static Sonuc DosyaKayitEdilebilirMi(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            sonuc = KontrolEt(ve, kullanan, form);
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray EvrakHareketleriniRaporla(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            ObjectArray liste = new ObjectArray();

            //Kullanan yetki kontrolü

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);

            IDataReader r = to.EvrakHareketleriniListele(form);

            if (r != null)
            {
                while (r.Read())
                {
                    EvrakSinif evr = new EvrakSinif();
                    evr.evrakNo = VeriErisim.DegerAl(r, 0, 0);
                    evr.prEvrakNo = VeriErisim.DegerAl(r, 10, "");


                    if (evr.prEvrakNo != "")
                    {
                        evr.hareket.birimKod1Ad = VeriErisim.DegerAl(r, 1, "");
                        evr.hareket.birimKod2Ad = VeriErisim.DegerAl(r, 2, "");
                        evr.hareket.islemTarih = VeriErisim.DegerAl(r, 3);
                        evr.hareket.islem = VeriErisim.DegerAl(r, 4, 0);
                        evr.hareket.islemYapanAd = VeriErisim.DegerAl(r, 5, "");
                    }
                    else
                    {
                        evr.tarihce.islemTarih = VeriErisim.DegerAl(r, 3);
                        evr.tarihce.islem = VeriErisim.DegerAl(r, 11, "");
                        evr.tarihce.islemYapanAd = VeriErisim.DegerAl(r, 5, "");
                    }

                    evr.konu = VeriErisim.DegerAl(r, 6, "");
                    evr.geldigiYer = VeriErisim.DegerAl(r, 7, "");
                    evr.evrakTarih = VeriErisim.DegerAl(r, 8);
                    evr.evrEvrakNo = VeriErisim.DegerAl(r, 9, "");



                    liste.objeler.Add(evr);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static ObjectArray EvrakEkDosyalariniRaporla(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            ObjectArray liste = new ObjectArray();

            //Kullanan yetki kontrolü

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);

            IDataReader r = to.EvrakDosyalariniListele(form);

            if (r != null)
            {
                while (r.Read())
                {
                    EvrakSinif evr = new EvrakSinif();
                    evr.evrEvrakNo = VeriErisim.DegerAl(r, 0, "");
                    evr.evrakNo = VeriErisim.DegerAl(r, 1, 0);
                    evr.evrakTarih = VeriErisim.DegerAl(r, 2);
                    evr.konu = VeriErisim.DegerAl(r, 3, "");
                    evr.geldigiYer = VeriErisim.DegerAl(r, 4, "");
                    evr.evrDosya.dosyaAdi = VeriErisim.DegerAl(r, 5, "");
                    evr.evrDosya.dosyaTarihi = VeriErisim.DegerAl(r, 6);
                    evr.evrDosya.dosyaBoyutu = VeriErisim.DegerAl(r, 7, 0);
                    liste.objeler.Add(evr);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static ObjectArray DosyaTasnifPlaniRaporla(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            IDataReader r = to.DosyaTasnifPlaniRaporla(form);
            object[] evrakBilgi;

            if (r == null)
            {
                ve.VeriErisimKapat();
                liste.sonuc.hataStr = EvrakI.EVRI029;
                liste.sonuc.islemSonuc = false;
                return liste;
            }

            if (r != null)
            {
                while (r.Read())
                {
                    evrakBilgi = new object[] { string.Empty, string.Empty, new int() };
                    evrakBilgi[0] = VeriErisim.DegerAl(r, 0, ""); //KOD
                    evrakBilgi[1] = VeriErisim.DegerAl(r, 1, ""); //AD
                    evrakBilgi[2] = VeriErisim.DegerAl(r, 2, 0);  //COUNT
                    liste.objeler.Add(evrakBilgi);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        /// <summary>
        /// Dağıtımla gelen evraklar için
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="kullanan"></param>
        /// <param name="liste"></param>
        /// <returns></returns>
        public static Sonuc GelenEvrakNumaraVer(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string hata = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);
            TABGenel tabGenel = new TABGenel(ve);
            int basarili = 0;
            string evrakNoList = "";

            ve.BeginTransaction();
            try
            {
                foreach (EvrakSinif evrak in liste.objeler)
                {
                    EvrakSinif bilgi = new EvrakSinif();
                    bilgi.evrEvrakNo = evrak.evrEvrakNo;
                    bilgi.prEvrakNo = evrak.prEvrakNo;
                    bilgi.bulunduguYer = evrak.bulunduguYer;
                    bilgi.evrakTipi = evrak.evrakTipi;

                    ObjectArray evraklar = Listele(ve, kullanan, bilgi);
                    if (evraklar.ObjeSayisi > 0)
                    {
                        bilgi = (EvrakSinif)evraklar.objeler[0];

                        if (bilgi.evrakNo == 0 && bilgi.evrakTipi == (int)ENUMEvrakTipi.GELEN)
                        {
                            string sayacKod = "EVRGELEN" + EvrakNoSayacBaslangicBilgisiAl() + EVRBirim.EvrakKokBirimGetir(ve, bilgi.evrakBirimKodu);
                            bilgi.evrakNo = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "0000000"), 0);

                            EvrakSinif evrakKaydet = new EvrakSinif();
                            evrakKaydet.evrEvrakNo = bilgi.evrEvrakNo;
                            evrakKaydet.evrakNo = bilgi.evrakNo;
                            evrakKaydet.evrakTarih = new TNSDateTime(DateTime.Now);
                            evrakKaydet.yil = DateTime.Now.Year;
                            sonuc = tab.Guncelle(evrakKaydet);
                            if (!sonuc.islemSonuc)
                                hata += string.Format("Evrak Güncellemesi sırasında hata oluştu.", bilgi.evrEvrakNo) + "<br>";
                        }
                    }
                    else
                        hata += string.Format(EvrakI.EVRI059, bilgi.evrEvrakNo) + "<br>";

                }

                if (hata != "")
                    sonuc.hataStr = hata;

                if (sonuc.hataStr == "")
                    sonuc.islemSonuc = true;
                else
                    sonuc.islemSonuc = false;

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.anahtar = evrakNoList;
                }
                else
                {
                    ve.Rollback();
                    evrakNoList = "";
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{6A553DB0-2A52-48FB-A88A-4A02EED1CD33} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc DosyaBilgisiKaydet(VeriErisim veriErisim, Kullanici kullanan, string evrEvrakNo, bool dosyasiVar)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrak to = new TABEvrak(ve);

                EvrakSinif form = new EvrakSinif();
                form.evrEvrakNo = evrEvrakNo;
                form.dosyasiVar = (dosyasiVar ? 1 : 0);

                sonuc = to.Guncelle(form);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr += EvrakI.EVRI011;
                }
                else
                    ve.Rollback();


            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{FBBB13DA-4A95-48D0-B4E1-71D778A6623D} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static string EvrEvrakNoGetir(VeriErisim veriErisim, string prEvrakNo)
        {
            if (string.IsNullOrEmpty(prEvrakNo)) return "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            string evrEvrakNo = tab.EvrEvrakNoGetir(prEvrakNo);

            ve.VeriErisimKapat();
            return evrEvrakNo;
        }

        public static Sonuc EvrakImzaYoluYenile(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            Sonuc sonuc = new Sonuc(false, "", "");
            EvrakImzaYolu evrImzaYolu = EVREvrakImzaYolu.Oku(ve, kullanan, new EvrakImzaYolu() { prEvrakNo = form.prEvrakNo });
            if (evrImzaYolu.imzaYolu.kod > 0)
            {
                sonuc = EVREvrakImzaYolu.Sil(ve, kullanan, evrImzaYolu);
                if (!sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            //EvrEvrakImzaYol tablosuna kayıt edilecek
            ImzaYolu imzaYolu = EVRImzaYolu.Oku(ve, kullanan, new ImzaYolu() { kod = form.imzaYolu });
            if (imzaYolu != null)
            {
                int siraNo = 1;
                ObjectArray detalar = new ObjectArray();

                bool kendisiVarMi = false;
                foreach (ImzaYoluDetay d in imzaYolu.detaylar)
                    if (d.siraNo == 1 && d.tcKimlikNo == kullanan.mernis)
                    {
                        kendisiVarMi = true;
                        break;
                    }
                if (!kendisiVarMi)
                    detalar.objeler.Add(new ImzaYoluDetay() { siraNo = siraNo, islemTip = (int)ENUMImzaYoluTip.PARAF, imzaYoluKod = imzaYolu.kod, tcKimlikNo = kullanan.mernis, ad = kullanan.ad, soyad = kullanan.soyad, evrakBirimKod = form.bulunduguYer, unvan = kullanan.unvan });

                foreach (ImzaYoluDetay d in imzaYolu.detaylar)
                {
                    siraNo++;
                    d.siraNo = siraNo;
                    detalar.objeler.Add(d);
                }

                imzaYolu.detaylar = (ImzaYoluDetay[])detalar.objeler.ToArray(typeof(ImzaYoluDetay));
                evrImzaYolu.imzaYolu = imzaYolu;
                sonuc = EVREvrakImzaYolu.Kaydet(ve, kullanan, evrImzaYolu);
                //****************************************
            }


            if (sonuc.islemSonuc)
            {
                TABEvrak tab = new TABEvrak(ve);

                ve.BeginTransaction();
                try
                {
                    EvrakSinif formImza = new EvrakSinif();
                    formImza.evrEvrakNo = form.evrEvrakNo;
                    formImza.imzaYoluDegisti = true;
                    formImza.imzaYolu = form.imzaYolu;
                    formImza.paraf = "";
                    formImza.imza1 = "";
                    formImza.imza2 = "";
                    formImza.imza3 = "";

                    sonuc = tab.Guncelle(formImza);
                    if (!sonuc.islemSonuc)
                    {
                        sonuc.hataStr = "İşlem sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                        ve.Rollback();
                    }
                    else
                    {
                        sonuc.bilgiStr = "İmza yolu bilgisi güncellendi.";
                        ve.Commit();
                    }
                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    Arac.HataStrYaz("{DDACDBC0-C32F-4708-BA9A-26C9F65DA544} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc AyniGidenEvrakMevcutMu(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //Giden evrak daha önce kayıt edilmiş mi?
            if (sonuc.hataStr == "" && Yetki.KullaniciEvrakYetkiSablonu(kullanan, form.bulunduguYer) == (int)ENUMEvrakYetkiSablonu.EVRAKMEMURU)
            {
                if (!string.IsNullOrEmpty(form.evrakBirimKodu) && !string.IsNullOrEmpty(form.konu) && !string.IsNullOrEmpty(form.geldigiYer))
                {
                    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                    TABEvrak tab = new TABEvrak(ve);
                    string evrEvrakNo = tab.AyniGidenEvrakMevcutMu(form);

                    if (evrEvrakNo != "")
                    {
                        EvrakSinif evrak = new EvrakSinif();
                        evrak.evrEvrakNo = evrEvrakNo;
                        evrak.bulunduguYer = form.bulunduguYer;

                        ObjectArray list = Listele(ve, kullanan, evrak);
                        if (list.ObjeSayisi > 0)
                        {
                            evrak = (EvrakSinif)list[0];

                            sonuc.hataStr += string.Format("<font color='red'>{0}</font> tarih ve <font color='red'>{1}</font> sayı ile <br>{2} konu ve {3}<br> isimli yere daha önce evrak gönderilmiştir.", evrak.evrakTarih.ToString(), evrak.evrakNo, evrak.konu, evrak.geldigiYer) + "<br>";
                            sonuc.islemSonuc = true;
                        }
                    }
                    else
                        sonuc.islemSonuc = false;

                    ve.VeriErisimKapat();
                }
            }

            return sonuc;
        }

        public static bool EvrakNumarasiAlabilirMi(Kullanici kullanan, string birimKod)
        {
            bool alabilir = true;

            int yetki = Yetki.KullaniciEvrakYetkiSablonu(kullanan, birimKod);
            if (!(yetki == (int)ENUMEvrakYetkiSablonu.EVRAKYETKILISI || yetki == (int)ENUMEvrakYetkiSablonu.EVRAKMEMURU))
            {
                string birimBilgi = System.Configuration.ConfigurationManager.AppSettings.Get("EvrakKullanicisiEvrakNoAlmasin") + "";

                if (!string.IsNullOrEmpty(birimBilgi))
                {
                    string[] birimListe = birimBilgi.TrimEnd(';').Split(';');
                    foreach (string birim in birimListe)
                        if (!string.IsNullOrEmpty(birim) && birimKod.StartsWith(birim))
                            return false;
                }
            }

            return alabilir;
        }

        public static bool EvrakNoElleVerilsinMi(Kullanici kullanan, string birimKod)
        {
            bool manuel = false;

            int yetki = Yetki.KullaniciEvrakYetkiSablonu(kullanan, birimKod);
            if (yetki == (int)ENUMEvrakYetkiSablonu.EVRAKYETKILISI || yetki == (int)ENUMEvrakYetkiSablonu.EVRAKMEMURU)
            {
                string birimBilgi = System.Configuration.ConfigurationManager.AppSettings.Get("EvrakKullanicisiEvrakNoAlmasin") + "";
                if (!string.IsNullOrEmpty(birimBilgi))
                {
                    string[] birimListe = birimBilgi.Split(';');
                    foreach (string birim in birimListe)
                        if (!string.IsNullOrEmpty(birim) && birimKod == birim)
                            return true;
                }
            }
            return manuel;
        }

        //public static Sonuc EvrakParaflaVeGonder(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif evrak, string gonderilecekBirim, string gonderilecekKisi, int gondermeTip)
        //{
        //    Sonuc sonuc = new Sonuc(true, "", "");

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
        //    TABEvrak tab = new TABEvrak(ve);

        //    ve.BeginTransaction();
        //    try
        //    {
        //        sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, ENUMEvrakHareketDurum.PARAFLANDI);
        //        if (sonuc.islemSonuc)
        //        {
        //            EvrakHareket yeniHareketi = new EvrakHareket();
        //            yeniHareketi.prEvrakNo = evrak.prEvrakNo;
        //            yeniHareketi.islem = (int)ENUMEvrakHareketDurum.YENI;
        //            ObjectArray listeHareket = EVREvrakHareket.Listele(ve, yeniHareketi);
        //            foreach (EvrakHareket h in listeHareket.objeler)
        //                yeniHareketi = h;

        //            EvrakHareket hareket = new EvrakHareket();
        //            hareket.prEvrakNo = evrak.prEvrakNo;
        //            hareket.islem = (int)ENUMEvrakHareketDurum.PARAFLANDI;
        //            hareket.birimKod2 = evrak.bulunduguYer;
        //            hareket.birimKod1 = yeniHareketi.birimKod1;
        //            hareket.kisi2 = kullanan.mernis;
        //            hareket.kisi1 = yeniHareketi.kisi1;
        //            hareket.havaleNotu = evrak.hareket.havaleNotu;
        //            hareket.vekil = evrak.hareket.vekil;
        //            hareket.vekilAd = evrak.hareket.vekilAd;

        //            sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
        //            if (sonuc.islemSonuc)
        //            {
        //                EvrakHareket havaleHareket = new EvrakHareket();
        //                havaleHareket.prEvrakNo = evrak.prEvrakNo;
        //                havaleHareket.birimKod1 = gonderilecekBirim;
        //                havaleHareket.birimKod2 = yeniHareketi.birimKod1;
        //                havaleHareket.kisi1 = gonderilecekKisi;
        //                havaleHareket.kisi2 = kullanan.mernis;
        //                havaleHareket.havaleNotu = (gondermeTip == 1 ? "PARAF" : "");
        //                havaleHareket.havaleTur = (int)ENUMEvrakHavaleTur.GEREGI;
        //                havaleHareket.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
        //                havaleHareket.vekil = evrak.hareket.vekil;
        //                havaleHareket.vekilAd = evrak.hareket.vekilAd;

        //                sonuc = EVREvrakHareket.Kaydet(ve, kullanan, havaleHareket);
        //            }
        //        }


        //        if (!sonuc.islemSonuc)
        //        {
        //            ve.Rollback();
        //        }
        //        else
        //        {
        //            ve.Commit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ve.Rollback();
        //        Arac.HataStrYaz("{3C8207F6-9B46-41D1-B522-9448EAFA8FCB} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
        //    }

        //    ve.VeriErisimKapat();
        //    return sonuc;

        //}

        public static Sonuc ParafImzaIslem(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif evrak, string gonderilecekBirim, string gonderilecekKisi, ENUMParafImzaIslemTip parafImzaIslemTip)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            ve.BeginTransaction();
            try
            {
                switch (parafImzaIslemTip)
                {
                    case ENUMParafImzaIslemTip.PARAFLA:
                    case ENUMParafImzaIslemTip.PARAFLAVEPARAFAGONDER:
                    case ENUMParafImzaIslemTip.PARAFLAVEIMZAYAGONDER:
                    case ENUMParafImzaIslemTip.PARAFLAVEEIMZAYAGONDER:
                        evrak.hareket.islem = (int)ENUMEvrakHareketDurum.PARAFLANDI;
                        sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, ENUMEvrakHareketDurum.PARAFLANDI);
                        break;
                    case ENUMParafImzaIslemTip.IMZALA:
                    case ENUMParafImzaIslemTip.IMZALAVEIMZAYAGONDER:
                        evrak.hareket.islem = (int)ENUMEvrakHareketDurum.IMZALANDI;
                        sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, ENUMEvrakHareketDurum.IMZALANDI);
                        break;
                    case ENUMParafImzaIslemTip.EIMZALA:
                    case ENUMParafImzaIslemTip.EIMZALAVEEIMZAYAGONDER:
                        evrak.hareket.islem = (int)ENUMEvrakHareketDurum.EIMZALANDI;
                        sonuc = IslemYapmaYetkisiVarMi(ve, kullanan, evrak, ENUMEvrakHareketDurum.EIMZALANDI);
                        break;
                    default:
                        sonuc.islemSonuc = false;
                        break;
                }

                if (sonuc.islemSonuc)
                {
                    EvrakHareket yeniHareketi = new EvrakHareket();
                    yeniHareketi.prEvrakNo = evrak.prEvrakNo;
                    yeniHareketi.islem = (int)ENUMEvrakHareketDurum.YENI;
                    ObjectArray listeHareket = EVREvrakHareket.Listele(ve, yeniHareketi);
                    foreach (EvrakHareket h in listeHareket.objeler)
                        yeniHareketi = h;

                    EvrakHareket hareket = new EvrakHareket();
                    hareket.prEvrakNo = evrak.prEvrakNo;
                    hareket.birimKod2 = evrak.bulunduguYer;
                    hareket.birimKod1 = yeniHareketi.birimKod1;
                    hareket.kisi2 = kullanan.mernis;
                    hareket.kisi1 = yeniHareketi.kisi1;
                    hareket.havaleNotu = evrak.hareket.havaleNotu;

                    switch (parafImzaIslemTip)
                    {
                        case ENUMParafImzaIslemTip.PARAFLA:
                        case ENUMParafImzaIslemTip.PARAFLAVEPARAFAGONDER:
                        case ENUMParafImzaIslemTip.PARAFLAVEIMZAYAGONDER:
                        case ENUMParafImzaIslemTip.PARAFLAVEEIMZAYAGONDER:
                            hareket.islem = (int)ENUMEvrakHareketDurum.PARAFLANDI;
                            break;
                        case ENUMParafImzaIslemTip.IMZALA:
                        case ENUMParafImzaIslemTip.IMZALAVEIMZAYAGONDER:
                            hareket.islem = (int)ENUMEvrakHareketDurum.IMZALANDI;
                            break;
                        case ENUMParafImzaIslemTip.EIMZALA:
                        case ENUMParafImzaIslemTip.EIMZALAVEEIMZAYAGONDER:
                            hareket.islem = (int)ENUMEvrakHareketDurum.EIMZALANDI;
                            break;
                    }

                    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);

                    switch (parafImzaIslemTip)
                    {
                        case ENUMParafImzaIslemTip.PARAFLAVEPARAFAGONDER:
                        case ENUMParafImzaIslemTip.PARAFLAVEIMZAYAGONDER:
                        case ENUMParafImzaIslemTip.PARAFLAVEEIMZAYAGONDER:
                        case ENUMParafImzaIslemTip.IMZALAVEIMZAYAGONDER:
                        case ENUMParafImzaIslemTip.EIMZALAVEEIMZAYAGONDER:

                            if (sonuc.islemSonuc)
                            {
                                EvrakHareket havaleHareket = new EvrakHareket();
                                havaleHareket.prEvrakNo = evrak.prEvrakNo;
                                havaleHareket.birimKod1 = gonderilecekBirim;
                                havaleHareket.birimKod2 = yeniHareketi.birimKod1;
                                havaleHareket.kisi1 = gonderilecekKisi;
                                havaleHareket.kisi2 = kullanan.mernis;


                                switch (parafImzaIslemTip)
                                {
                                    case ENUMParafImzaIslemTip.PARAFLAVEPARAFAGONDER:
                                        havaleHareket.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
                                        havaleHareket.havaleNotu = "PARAF";
                                        break;
                                    case ENUMParafImzaIslemTip.PARAFLAVEIMZAYAGONDER:
                                    case ENUMParafImzaIslemTip.IMZALAVEIMZAYAGONDER:
                                        havaleHareket.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
                                        havaleHareket.havaleNotu = "IMZA";
                                        break;
                                    case ENUMParafImzaIslemTip.PARAFLAVEEIMZAYAGONDER:
                                    case ENUMParafImzaIslemTip.EIMZALAVEEIMZAYAGONDER:
                                        havaleHareket.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
                                        havaleHareket.havaleNotu = "EIMZA";
                                        break;
                                }

                                sonuc = EVREvrakHareket.Kaydet(ve, kullanan, havaleHareket);
                            }

                            break;
                    }
                }


                if (!sonuc.islemSonuc)
                    ve.Rollback();
                else
                    ve.Commit();
                ve.VeriErisimKapat();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{DB15F037-407A-4C34-8203-FD581C7ACED8} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            return sonuc;
        }

        public static Sonuc EvrakiHazirlayanaGonder(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ObjectArray evrakListe = EVREvrak.Listele(ve, kullanan, new EvrakSinif() { prEvrakNo = prEvrakNo }, 0, 0);
            if (evrakListe != null && evrakListe.objeler.Count > 0)
            {
                EvrakSinif evrak = (EvrakSinif)evrakListe.objeler[0];

                EvrakHareket ilkHareketi = new EvrakHareket();
                EvrakHareket sonHareketi = new EvrakHareket();
                ObjectArray listeHareket = EVREvrakHareket.Listele(ve, new EvrakHareket() { prEvrakNo = evrak.prEvrakNo });
                int index = 0;
                foreach (EvrakHareket h in listeHareket.objeler)
                {
                    if (index == 0)
                        ilkHareketi = h;
                    else if (index == listeHareket.objeler.Count - 1)
                        sonHareketi = h;
                    index++;
                }

                string gonderilecekKisi = ilkHareketi.kisi1;
                string gonderilecekBirim = ilkHareketi.birimKod1;
                string gonderenBirim = sonHareketi.birimKod1;

                if (gonderilecekKisi == kullanan.mernis)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Kişi kendisine havale edemez.";
                }
                else
                {
                    EvrakHareket havaleHareket = new EvrakHareket();
                    havaleHareket.prEvrakNo = evrak.prEvrakNo;
                    havaleHareket.birimKod1 = gonderilecekBirim;
                    havaleHareket.birimKod2 = gonderenBirim;
                    havaleHareket.kisi1 = gonderilecekKisi;
                    havaleHareket.kisi2 = kullanan.mernis;
                    havaleHareket.havaleNotu = "";
                    havaleHareket.havaleTur = (int)ENUMEvrakHavaleTur.GEREGI;
                    havaleHareket.islem = (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN;

                    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, havaleHareket);
                }

            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format("{0} programatik nolu kayıtlı evrak bulunamadı. Lütfen prEvrakNo bilgisini kontrol ediniz.", prEvrakNo);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc ParafImzaGonder(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif evrak, string gonderilecekBirim, string gonderilecekKisi, ENUMParafImzaGonderTip parafImzaGonderTip, bool sonImzaMi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            ve.BeginTransaction();
            try
            {
                EvrakHareket yeniHareketi = new EvrakHareket();
                yeniHareketi.prEvrakNo = evrak.prEvrakNo;
                yeniHareketi.islem = (int)ENUMEvrakHareketDurum.YENI;
                ObjectArray listeHareket = EVREvrakHareket.Listele(ve, yeniHareketi);
                foreach (EvrakHareket h in listeHareket.objeler)
                    yeniHareketi = h;

                EvrakHareket havaleHareket = new EvrakHareket();
                havaleHareket.prEvrakNo = evrak.prEvrakNo;
                havaleHareket.birimKod1 = gonderilecekBirim;
                havaleHareket.birimKod2 = yeniHareketi.birimKod1;
                havaleHareket.kisi1 = gonderilecekKisi;
                havaleHareket.kisi2 = kullanan.mernis;

                switch (parafImzaGonderTip)
                {
                    case ENUMParafImzaGonderTip.PARAF:
                        havaleHareket.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
                        havaleHareket.havaleNotu = "PARAF";
                        break;
                    case ENUMParafImzaGonderTip.IMZA:
                        havaleHareket.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
                        havaleHareket.havaleNotu = sonImzaMi ? "IMZASON" : "IMZA";
                        break;
                    case ENUMParafImzaGonderTip.EIMZA:
                        havaleHareket.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
                        havaleHareket.havaleNotu = sonImzaMi ? "EIMZASON" : "EIMZA";
                        break;
                }

                sonuc = EVREvrakHareket.Kaydet(ve, kullanan, havaleHareket);


                if (!sonuc.islemSonuc)
                    ve.Rollback();
                else
                {
                    ve.Commit();

                    string postaKisi = gonderilecekKisi;
                    string postaYer = "";
                    int postaTur = (int)ENUMPOSTATIP.IMZA;

                    EVRPosta.YeniEvrakGeldiPostasiHazirla(ve, kullanan, evrak.evrEvrakNo, evrak.prEvrakNo, postaKisi, postaYer, postaTur);
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{FD6F2CAD-72D9-45DD-9C4A-C138F3ECDC13} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static bool EvrakImzaliMi(VeriErisim veriErisim, Kullanici kullanan, string evrEvrakNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            bool imzaliMi = tab.EvrakImzaliMi(evrEvrakNo);

            ve.VeriErisimKapat();

            return imzaliMi;
        }

        public static bool EditorKullaniliyorMu(VeriErisim veriErisim, Kullanici kullanan, string evrEvrakNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            bool durum = tab.EditorKullaniliyorMu(evrEvrakNo);

            ve.VeriErisimKapat();

            return durum;
        }


        //public static Sonuc EditorGuncellenmeliDurumunuDegistir(VeriErisim veriErisim, Kullanici kullanan, string evrEvrakNo, bool guncellenmeli)
        //{
        //    Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
        //    TABEvrak tab = new TABEvrak(ve);

        //    ve.BeginTransaction();
        //    try
        //    {
        //        sonuc = tab.EditorGuncellenmeliDurumunuDegistir(evrEvrakNo, guncellenmeli);

        //        if (sonuc.islemSonuc)
        //            ve.Commit();
        //        else
        //            ve.Rollback();

        //    }
        //    catch (Exception ex)
        //    {
        //        ve.Rollback();
        //        Arac.HataStrYaz("{83966D5E-F3B3-4038-95FF-3EA43D44ACA8} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
        //    }
        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}


        public static EvrakEImzalandiBilgi EvrakEImzalandiMi(VeriErisim veriErisim, Kullanici kullanan, string evrEvrakNo)
        {
            EvrakEImzalandiBilgi form = new EvrakEImzalandiBilgi();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            IDataReader dr = tab.EvrakEImzaBilgiListele(evrEvrakNo);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form.evrEvrakNo = evrEvrakNo;
                    form.eImzalandi = VeriErisim.DegerAl(dr, 0, 0) == 1 ? true : false;
                    form.eImzalayan = VeriErisim.DegerAl(dr, 1, "");
                    form.eImzaTarihi = VeriErisim.DegerAl(dr, 2);
                    break;
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return form;
        }

        public static Sonuc ImzalaParafla2(VeriErisim veriErisim, Kullanici kullanan, string evrEvrakNo, EvrakHareket islemHareket, bool eImzaMi, object surecResultDegeri)
        {
            Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 - Başla - {F7E64F5A-2522-4350-96F1-78F89028C034} - " + string.Format("kullaniciKodu:{0} - evrEvrakNo:{1} - islemHareket.havaleNotu:{2} - eImzaMi:{3} - surecResultDegeri:{4} ", kullanan.kullaniciKodu, evrEvrakNo, islemHareket.havaleNotu, eImzaMi ? "Evet" : "Hayır", surecResultDegeri != null ? surecResultDegeri.ToString() : "null"));

            Sonuc sonuc = new Sonuc(false, "İmza işlemi sırasında bir hata oluştu.<br>", "");

            if (evrEvrakNo == "")
            {
                sonuc.DegerAta("EvrEvrakNo bulunamadı.");
                return sonuc;
            }

            string postaKisi = "";
            string postaYer = "";
            int postaTur = (int)ENUMPOSTATIP.IMZA;

            bool surecEvrak = false;
            EvrakSinif evrak = null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            ObjectArray tmpEvrak = Listele(ve, kullanan, new EvrakSinif() { evrEvrakNo = evrEvrakNo }, 0, 0);
            if (tmpEvrak.sonuc.islemSonuc && tmpEvrak.objeler.Count > 0)
            {
                sonuc = tmpEvrak.sonuc;
                evrak = (EvrakSinif)tmpEvrak.objeler[0];

                string evrakSahibiTcKimlikNo = "";
                ImzaYoluDetay imzaYoluIslemYapan = null;
                ImzaYoluDetay imzaYoluGonderilecek = null;

                if (surecResultDegeri != null && surecResultDegeri.GetType().Equals(typeof(InfocomDosyaImza)))
                {
                    evrak.imzaBitti = 0;
                    if (evrak.imzaYolu == 0)
                        evrak.imzaTarihi = new TNSDateTime();
                }


                //Editör Kullanılıyorsa ve editör güncel değilse imza paraf işlemine uyarı versin - H.Ö_27072017
                if (EditorKullaniliyorMu(ve, kullanan, evrak.evrEvrakNo) && evrak.editorGuncellenmeli == 1)
                {
                    sonuc.DegerAta("Evrak bilgileri üzerinde değişiklik yapılmış ancak editör üzerinde güncelleme yapılmadığı görülmüştür.<br>Lütfen editör üzerinden evrak bilgilerini yeniden kaydediniz.<br>");
                }
                else if (evrak.imzaBitti == 1)
                {
                    sonuc.DegerAta("Evrakın imza işlemi tamamlanmıştır. Tekrar imza işlemi yapılamaz!<br>");
                }
                else if (evrak.imzaYolu > 0) //İmza yolu varsa
                {
                    //İmza yolu varsa

                    EvrakImzaYolu evrakImzaYolu = EVREvrakImzaYolu.Oku(ve, kullanan, new EvrakImzaYolu() { prEvrakNo = evrak.prEvrakNo });
                    if (evrakImzaYolu != null || evrakImzaYolu.imzaYolu.kod > 0)
                    {
                        ImzaYolu imzaYolu = evrakImzaYolu.imzaYolu;

                        if (imzaYolu.detaylar != null && imzaYolu.detaylar.Length > 0)
                        {
                            evrakSahibiTcKimlikNo = imzaYolu.detaylar[0].tcKimlikNo;
                            if (evrak.imzaTarihi.isNull && kullanan.mernis != evrakSahibiTcKimlikNo)
                                sonuc.DegerAta("Evrak <b>paraf/imza</b> işlemi için uygun değildir. <br>Paraf / imza işlemi başlatılmamış veya evrak üzerinde değişiklik nedeniyle imza/paraf kaldırılmış.<br><br><font color='red'><b>Not:</b></font> Evrakın hazırlayan kişi tarafından <b>imza/paraf</b> işlemi başlatılmalıdır.<br>Lütfen evrakı hazırlayan kişiye gönderiniz.");
                            else if (evrak.imzaTarihi.isNull)
                            {
                                //İlk imza yolu hareketi
                                imzaYoluIslemYapan = imzaYolu.detaylar[0];
                                if (imzaYolu.detaylar.Length > 1)
                                    imzaYoluGonderilecek = imzaYolu.detaylar[1];
                            }
                            else
                            {
                                EvrakImzaYoluHareket[] hareketListe = EVREvrakImzaYolu.Listele(ve, kullanan, evrak.prEvrakNo);
                                if (hareketListe != null && hareketListe.Length > 0)
                                {
                                    EvrakImzaYoluHareket sonHareket = hareketListe[hareketListe.Length - 1];
                                    if (!evrak.imzaTarihi.isNull && sonHareket.kisi1 == imzaYolu.detaylar[imzaYolu.detaylar.Length - 1].tcKimlikNo && sonHareket.islemTip != (int)ENUMEvrakHareketDurum.GERIGONDERILDI)
                                        sonuc.DegerAta("Evrakın imza işlemi tamamlanmıştır.");
                                    else
                                    {
                                        for (int i = 0; i < imzaYolu.detaylar.Length; i++)
                                        {
                                            if (sonHareket.kisi2 == imzaYolu.detaylar[i].tcKimlikNo)
                                            {
                                                imzaYoluIslemYapan = imzaYolu.detaylar[i];

                                                if (i + 1 < imzaYolu.detaylar.Length)
                                                    imzaYoluGonderilecek = imzaYolu.detaylar[i + 1];
                                                break;
                                            }
                                        }
                                    }
                                }
                                //else
                                //{
                                //    //İlk imza yolu hareketi
                                //    imzaYoluIslemYapan = imzaYolu.detaylar[0];
                                //    if (imzaYolu.detaylar.Length > 1)
                                //        imzaYoluGonderilecek = imzaYolu.detaylar[1];
                                //}
                            }
                        }
                        else
                            sonuc.DegerAta("İmza yolu bilgisine göre paraf/imza atacak kişiler belirtilmemiş.");
                    }
                    else
                        sonuc.DegerAta("İmza yolu bilgisini güncelleyiniz.");
                }
                else
                {
                    //İmza Yolu Yoksa
                    imzaYoluIslemYapan = null;
                    imzaYoluGonderilecek = null;

                    if (!evrak.imzaTarihi.isNull && !(!string.IsNullOrEmpty(evrak.surecAd) || !string.IsNullOrEmpty(evrak.dinamikSurecNo)))
                        sonuc.DegerAta("Evrakın imza işlemi tamamlanmıştır.<br>");
                }

                if (sonuc.islemSonuc)
                {
                    bool imzaBitti = false;
                    int islem = -1;
                    if (imzaYoluIslemYapan != null)
                    {
                        if (imzaYoluIslemYapan.islemTip == (int)ENUMImzaYoluTip.PARAF || imzaYoluIslemYapan.islemTip == (int)ENUMImzaYoluTip.KOORDINASYON)
                            islem = eImzaMi ? (int)ENUMEvrakHareketDurum.EPARAFLANDI : (int)ENUMEvrakHareketDurum.PARAFLANDI;
                        else if (imzaYoluIslemYapan.islemTip == (int)ENUMImzaYoluTip.IMZA)
                            islem = eImzaMi ? (int)ENUMEvrakHareketDurum.EIMZALANDI : (int)ENUMEvrakHareketDurum.IMZALANDI;
                    }
                    else if (!string.IsNullOrEmpty(evrak.surecAd) && string.IsNullOrEmpty(evrak.dinamikSurecNo))
                    {
                        //Arac.HataStrYaz("{1D079720-113C-4773-AFC9-3213EA790264} Message: Süreç başlamamış");

                        surecEvrak = true;

                        imzaYoluIslemYapan = new ImzaYoluDetay();
                        imzaYoluIslemYapan.tcKimlikNo = kullanan.mernis;
                        imzaYoluIslemYapan.ad = kullanan.ad;
                        imzaYoluIslemYapan.soyad = kullanan.soyad;

                        if (!(!evrak.imzaTarihi.isNull && evrak.hareket.havaleNotu == ""))             //İlk Hazırlayana gitmesi için
                            imzaYoluGonderilecek = new ImzaYoluDetay();

                        islem = eImzaMi ? (int)ENUMEvrakHareketDurum.EPARAFLANDI : (int)ENUMEvrakHareketDurum.PARAFLANDI;  // Süreç için ilk işlem ne olacak bilinmiyor. O nedenle paraf seçildi.
                    }
                    else if (!string.IsNullOrEmpty(evrak.dinamikSurecNo))
                    {
                        //Arac.HataStrYaz("{DE668245-C770-4413-B6FE-B70FFF699F08} Message: Süreç devam ediyor");

                        surecEvrak = true;

                        imzaYoluIslemYapan = new ImzaYoluDetay();
                        imzaYoluIslemYapan.tcKimlikNo = kullanan.mernis;
                        imzaYoluIslemYapan.ad = kullanan.ad;
                        imzaYoluIslemYapan.soyad = kullanan.soyad;


                        bool imzayaGonderHareketiVarMi = false;
                        EvrakHareket sonImzayaGonderHareketi = new EvrakHareket();
                        sonImzayaGonderHareketi.prEvrakNo = evrak.prEvrakNo;
                        sonImzayaGonderHareketi.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
                        ObjectArray listeHareket = EVREvrakHareket.Listele(ve, sonImzayaGonderHareketi);
                        foreach (EvrakHareket h in listeHareket.objeler)
                        {
                            sonImzayaGonderHareketi = h;
                            imzayaGonderHareketiVarMi = true;
                        }

                        if (imzayaGonderHareketiVarMi && sonImzayaGonderHareketi.havaleNotu.EndsWith("SON") && surecResultDegeri.ToString() != "2") //İlk Hazırlayana gitmesi için
                            imzaYoluGonderilecek = null;
                        else
                            imzaYoluGonderilecek = new ImzaYoluDetay();


                        if (sonImzayaGonderHareketi.havaleNotu.StartsWith("IMZA") || sonImzayaGonderHareketi.havaleNotu.StartsWith("EIMZA"))
                            islem = eImzaMi ? (int)ENUMEvrakHareketDurum.EIMZALANDI : (int)ENUMEvrakHareketDurum.IMZALANDI;
                        else
                            islem = eImzaMi ? (int)ENUMEvrakHareketDurum.EPARAFLANDI : (int)ENUMEvrakHareketDurum.PARAFLANDI;

                    }
                    else
                    {
                        islem = eImzaMi ? (int)ENUMEvrakHareketDurum.EIMZALANDI : (int)ENUMEvrakHareketDurum.IMZALANDI;
                        imzaBitti = true;
                    }

                    if (imzaYoluGonderilecek != null && !string.IsNullOrEmpty(imzaYoluGonderilecek.tcKimlikNo))
                    {
                        //kullanıcı yeni bir birime atanmış ise, imza yolunda tanımlı birime yetkisi devam ediyor mu? bakılmalıdır. Eski birim kodu yok ise uyarı verilecek. 
                        //Aktif birim yerine yetkili olduğu bütün birimlere bakıyoruz. Kullanıcı birden fazla birime yetkili ise aktif birimini değiştirmiş olabilir.

                        bool birimVar = false;
                        string[] birimler = YetkiliEvrakBirimleriniGetir(imzaYoluGonderilecek.tcKimlikNo);

                        if (birimler != null)
                            foreach (var birimKod in birimler)
                            {
                                if (birimKod == imzaYoluGonderilecek.evrakBirimKod)
                                { birimVar = true; break; }
                            }

                        if (!birimVar)
                        {
                            string birimAd = EVRGenel.EvrakBirimAdGetir(imzaYoluGonderilecek.evrakBirimKod);
                            sonuc.DegerAta("İmza / paraf işleminde devam edilemiyor!<br><br><font color=\"red\"><b>" + imzaYoluGonderilecek.ad + " " + imzaYoluGonderilecek.soyad + "</b></font> kullanıcısının <font color=\"red\"><b>" + birimAd + "</b></font> birimden yetkisi kaldırılmış.<br><br>Lütfen imza yolu bilgisini kontrol ediniz.");
                        }
                    }

                    if (sonuc.islemSonuc)
                    {
                        if (islem > -1)
                        {
                            ve.BeginTransaction();

                            EvrakSinif imzaliForm = new EvrakSinif()
                            {
                                evrEvrakNo = evrak.evrEvrakNo,
                                imzaTarihi = new TNSDateTime(DateTime.Now.ToString()),
                                eImzalandi = eImzaMi ? 1 : 0,
                                eImzalayan = eImzaMi ? kullanan.ad + " " + kullanan.soyad : null,
                                eImzaTarihi = eImzaMi ? new TNSDateTime(DateTime.Now.ToString()) : null,
                                imzaBitti = ((!evrak.imzaTarihi.isNull && imzaYoluGonderilecek == null) || imzaBitti) ? 1 : 0
                            };
                            if (eImzaMi)
                                imzaliForm.uretici = kullanan.ad + " " + kullanan.soyad;

                            sonuc = tab.Guncelle(imzaliForm);

                            if (sonuc.islemSonuc)
                            {
                                if (imzaYoluIslemYapan != null && surecEvrak == false)
                                {
                                    EvrakImzaYoluHareket evrImzaYoluHareket = new EvrakImzaYoluHareket()
                                    {
                                        prEvrakNo = evrak.prEvrakNo,
                                        kisi1 = kullanan.mernis,
                                        kisi2 = imzaYoluGonderilecek != null ? imzaYoluGonderilecek.tcKimlikNo : "",
                                        islemTip = islem,
                                        islemTarih = new TNSDateTime(DateTime.Now)
                                    };

                                    sonuc = EVREvrakImzaYolu.Kaydet(ve, kullanan, evrImzaYoluHareket);
                                }

                                if (sonuc.islemSonuc)
                                {

                                    //Parafla - İmzala
                                    EvrakHareket hareket = new EvrakHareket();
                                    hareket.prEvrakNo = evrak.prEvrakNo;
                                    hareket.islem = islem;

                                    if (imzaYoluIslemYapan != null && imzaYoluGonderilecek == null)
                                    {
                                        //İmza Yolu Bitti Evrak İlk Hazırlayana gidecek
                                        EvrakHareket yeniHareketi = new EvrakHareket();
                                        yeniHareketi.prEvrakNo = evrak.prEvrakNo;
                                        yeniHareketi.islem = (int)ENUMEvrakHareketDurum.YENI;
                                        ObjectArray listeHareket = EVREvrakHareket.Listele(ve, yeniHareketi);
                                        foreach (EvrakHareket h in listeHareket.objeler)
                                            yeniHareketi = h;

                                        hareket.birimKod1 = yeniHareketi.birimKod1;
                                        hareket.birimKod2 = evrak.hareket.birimKod1;
                                        hareket.kisi1 = yeniHareketi.kisi1;
                                        hareket.kisi2 = kullanan.mernis;
                                        hareket.havaleNotu = islemHareket.havaleNotu;
                                    }
                                    else
                                    {
                                        hareket.birimKod1 = evrak.hareket.birimKod1;
                                        hareket.birimKod2 = evrak.hareket.birimKod1;
                                        hareket.kisi1 = kullanan.mernis;
                                        hareket.kisi2 = kullanan.mernis;
                                        hareket.havaleNotu = "";
                                    }

                                    hareket.vekil = islemHareket.vekil;
                                    hareket.vekilAd = islemHareket.vekilAd;

                                    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);

                                    if (sonuc.islemSonuc && imzaYoluGonderilecek != null && surecEvrak == false)
                                    {
                                        //Parafa - İmzaya Gönder
                                        hareket = new EvrakHareket();
                                        hareket.prEvrakNo = evrak.prEvrakNo;
                                        hareket.islem = (int)ENUMEvrakHareketDurum.IMZAYAGONDER;
                                        hareket.birimKod1 = imzaYoluGonderilecek.evrakBirimKod;
                                        hareket.birimKod2 = evrak.hareket.birimKod1;
                                        hareket.kisi1 = imzaYoluGonderilecek.tcKimlikNo;
                                        hareket.kisi2 = kullanan.mernis;
                                        hareket.havaleNotu = (imzaYoluGonderilecek.islemTip == (int)ENUMImzaYoluTip.PARAF || imzaYoluGonderilecek.islemTip == (int)ENUMImzaYoluTip.KOORDINASYON ? "PARAF" : (eImzaMi ? "EIMZA" : "IMZA"));
                                        hareket.vekil = islemHareket.vekil;
                                        hareket.vekilAd = islemHareket.vekilAd;

                                        if (!string.IsNullOrEmpty(islemHareket.havaleNotu))
                                            hareket.havaleNotu += "|" + islemHareket.havaleNotu;

                                        sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                                        if (sonuc.islemSonuc)
                                            postaKisi = imzaYoluGonderilecek.tcKimlikNo;
                                    }

                                }

                            }

                            if (sonuc.islemSonuc)
                            {
                                ve.Commit();
                                sonuc.bilgiStr = "Paraflama / İmzalama işlemi tamamlandı.";
                            }
                            else
                                ve.Rollback();
                        }
                        else
                            sonuc.DegerAta("İmza yolu bilgisine göre paraf/imza işlemi seçilmemiş.");
                    }

                }

                if (!sonuc.islemSonuc)
                    sonuc.hataStr = evrak.evrakNo + " nolu evrak imzalanamadı.<br>" + sonuc.hataStr;

            }
            else
                Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 - Evrak Bulunamadı - {15B89D3C-5C7F-4168-BF92-E8DB9789CF5B} - " + string.Format("evrEvrakNo:{1}", evrEvrakNo));



            if (sonuc.islemSonuc && surecEvrak && evrak != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(evrak.surecAd) && string.IsNullOrEmpty(evrak.dinamikSurecNo))
                    {
                        Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 surecAd - WCFClient Başla - {1EDBD0ED-AE5E-4900-AEAC-4AF065F85CD9} - " + string.Format("evrak.surecAd:{0} - evrak.dinamikSurecNo:{1}", evrak.surecAd, evrak.dinamikSurecNo));
                        WCFClient wcf = new WCFClient();

                        Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 surecAd - LoginCentraCube - {6F7FDC00-98F5-49F2-A18D-F0DB228862C0} - " + string.Format("workflowName:{0} - userID:{1} - password:{2} - token:{3} ", "", kullanan.kullaniciKodu, kullanan.parola, ""));
                        UserInfo userInfo = TNS.EVR.Arac.LoginCentraCube("", kullanan.kullaniciKodu, kullanan.parola, "");

                        if (userInfo != null)
                        {
                            Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 surecAd - UserInfo.TokenId - {F974E996-F234-40A1-A781-A6FBEBE90F54} - " + string.Format("userInfo.TokenId:{0}", userInfo.TokenId));

                            if (!string.IsNullOrWhiteSpace(userInfo.TokenId))
                            {
                                Dictionary<string, object> inputs = TNS.EVR.Arac.SurecParametrelerineDegerAta(wcf, evrak.surecAd, kullanan, evrak.evrEvrakNo, evrak.prEvrakNo, true, userInfo.TokenId);
                                string surecNo = wcf.CreateAndRunWorkflowByName(evrak.surecAd, inputs, kullanan.kullaniciKodu);

                                Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 surecAd - CreateAndRunWorkflowByName için dönen değer - {C2B212E4-8F48-4545-9E4D-B09652E6AC06} - " + string.Format("surecNo:{0}", surecNo));

                                //SurecNo bilgisini güncelle
                                if (!string.IsNullOrEmpty(surecNo))
                                {
                                    EvrakSinif surecForm = new EvrakSinif();
                                    surecForm.evrEvrakNo = evrak.evrEvrakNo;
                                    surecForm.dinamikSurecNo = surecNo;
                                    Sonuc sonuc2 = tab.Guncelle(surecForm);
                                    if (!sonuc2.islemSonuc)
                                        sonuc = sonuc2;
                                    else
                                        sonuc.bilgiStr += " Süreç başlatıldı - " + surecNo;
                                }
                            }
                            else
                                sonuc.DegerAta("İşlemi sırasında token bilgisi alınamıyor.<br>");
                        }
                        else if (string.IsNullOrEmpty(userInfo.TokenId))
                            sonuc.DegerAta("Bağlantı kurulamıyor... <br><br><b>Açıklama</b> : LoginCentraCube işlemi sırasında Token bilgisi alınamıyor.<br>Config ayarları hatalı olabilir. Lütfen sistem yönetisi ile görüşün.");
                        else
                            sonuc.DegerAta("Bağlantı kurulamıyor... <br><br><b>Açıklama</b> : LoginCentraCube işlemi sırasında UserInfo null geliyor.<br>Config ayarları hatalı olabilir. Lütfen sistem yönetisi ile görüşün.");

                    }
                    else if (!string.IsNullOrEmpty(evrak.dinamikSurecNo)) //Süreç başlamış ise
                    {
                        //Süreç Devam Et
                        Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 SüreçDevam - WCFClient Başla - {E2EE608C-C434-4474-8117-05292FAC7CE5} - " + string.Format("evrak.dinamikSurecNo:{0}", evrak.dinamikSurecNo));
                        WCFClient wcf = new WCFClient();

                        Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 SüreçDevam - LoginCentraCube - {A6CF3420-8174-45EE-960A-4DB0435A026D} - " + string.Format("workflowName:{0} - userID:{1} - password:{2} - token:{3} ", "", kullanan.kullaniciKodu, kullanan.parola, ""));
                        UserInfo userInfo = TNS.EVR.Arac.LoginCentraCube("", kullanan.kullaniciKodu, kullanan.parola, "");

                        if (userInfo != null)
                        {
                            Dictionary<string, object> inputs = new Dictionary<string, object>();
                            inputs.Add("Result", surecResultDegeri);

                            Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 SüreçDevam - RunWorkFlowByWorkflowInstanceID - {960A480D-07E7-4C46-B5EF-E0F61869FE89} - " + string.Format("kullanan.mernis:{0} - kullanan.kullaniciKodu:{1} - evrak.dinamikSurecNo:{2} - inputs:{3} - userInfo.TokenId:{4}", kullanan.mernis, kullanan.kullaniciKodu, evrak.dinamikSurecNo, "inputs; Result=" + surecResultDegeri.ToString(), userInfo.TokenId));

                            if (!string.IsNullOrWhiteSpace(userInfo.TokenId))
                            {
                                string msg = wcf.RunWorkFlowByWorkflowInstanceID(kullanan.mernis, kullanan.kullaniciKodu, Guid.Parse(evrak.dinamikSurecNo), inputs, userInfo.TokenId);

                                Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 SüreçDevam - RunWorkFlowByWorkflowInstanceID için dönen değer - {724CF0AD-CEF5-4017-A5E1-006383862F42} - " + string.Format("msg:{0}", msg != null ? msg : "null"));

                                if (!string.IsNullOrEmpty(JSONDegerGetir(msg, "Mesaj")))
                                {
                                    string hataStr = "<br><b>Hata Kodu :</b> {563188C9-6CB1-4BD7-B64F-FBA8B87A9F3A} <br><b>Message</b> : Süreç hata verdi." + "<br><b>Mernis :</b> " + kullanan.mernis + "<br><b>kullaniciKodu :</b> " + kullanan.kullaniciKodu + "<br><b>SüreçNo :</b> " + evrak.dinamikSurecNo + "<br><b>Result :</b> " + surecResultDegeri + "<br><b>HataMesajı :</b> " + msg;

                                    if (EVREvrak.YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu.EvrakIsAkisiHatalariniGoster))
                                        msg = hataStr;

                                    sonuc.DegerAta(msg);
                                    Arac.HataStrYaz(hataStr);
                                }
                            }
                            else
                                sonuc.DegerAta("İşlemi sırasında token bilgisi alınamıyor.<br>");
                        }
                        else if (string.IsNullOrEmpty(userInfo.TokenId))
                            sonuc.DegerAta("Bağlantı kurulamıyor... <br><br><b>Açıklama</b> : LoginCentraCube işlemi sırasında Token bilgisi alınamıyor.<br>Config ayarları hatalı olabilir. Lütfen sistem yönetisi ile görüşün.");
                        else
                            sonuc.DegerAta("Bağlantı kurulamıyor... <br><br><b>Açıklama</b> : LoginCentraCube işlemi sırasında UserInfo null geliyor.<br>Config ayarları hatalı olabilir. Lütfen sistem yönetisi ile görüşün.");
                    }
                }
                catch (Exception ex)
                {
                    Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 SüreçDevam - try catch Exception - {06882939-ADA2-41BC-A635-FFC8AA8DF980} - " + string.Format("Message:{0} - StackTrace:{1}", ex.Message, ex.StackTrace));

                    string msg = "<br><b>Hata Kodu :</b> {B8D26510-775A-47AF-8537-246283B6E332} <br><b>Message</b> : Süreç imza işlemi " + ex.Message + "<br>";
                    string hataStr = msg + "<b>StackTrace :</b> " + ex.StackTrace;

                    if (EVREvrak.YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu.EvrakIsAkisiHatalariniGoster))
                        msg = hataStr;

                    sonuc.DegerAta(msg);
                    Arac.HataStrYaz(hataStr);
                }
            }

            try
            {
                if (sonuc.islemSonuc)
                    EVRPosta.YeniEvrakGeldiPostasiHazirla(ve, kullanan, evrak, postaKisi, postaYer, postaTur);
            }
            catch (Exception ex)
            {
                Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - YeniEvrakGeldiPostasiHazirla - try catch Exception - {626F13B6-6EC7-4E1F-8901-79F3C94BD493} - " + string.Format("Message:{0} - StackTrace:{1}", ex.Message, ex.StackTrace));
            }

            ve.VeriErisimKapat();

            Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - ImzalaParafla2 - Sonuc - {F0E016F8-6FF1-48B0-8389-8CFB803F3EF1} - " + string.Format("Sonuc:{0}", sonuc.islemSonuc ? "True" : sonuc.hataStr));

            return sonuc;
        }

        public static string JSONDegerGetir(string json, string anahtar)
        {
            string mesaj = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(json))
                {
                    //JToken token = JObject.Parse(json);
                    //mesaj = (string)token.SelectToken(anahtar);

                    //-----------------------------------------------------------------Newtonsoft.Json.dll sorunu yaşanıyor. O nedenle yöntem bu şekilde değiştirildi.
                    int indexMesaj = json.IndexOf("\"Mesaj\":");
                    if (indexMesaj > -1)
                        mesaj = json.Substring(indexMesaj + 8, json.Length - 1 - (indexMesaj + 8));
                    //------------------------------------------------------------------
                }
            }
            catch (Exception ex)
            {
                Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - JSONDegerGetir - try catch Exception - {0E1FB465-243C-4EED-B03C-62A50AF5241C} - " + string.Format("Message:{0} - StackTrace:{1}", ex.Message, ex.StackTrace));
            }
            return mesaj;
        }

        public static Sonuc GeriGonder2(VeriErisim veriErisim, Kullanici kullanan, EvrakHareket islemHareket)
        {
            Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - GeriGonder2 - Başla - {CDA9BC2F-ECD2-49BC-8751-568F5BC2814D} - " + string.Format("kullanan.kullaniciKodu:{0} - islemHareket.prEvrakNo:{1} - islemHareket.havaleNotu:{2} ", kullanan.kullaniciKodu, islemHareket.prEvrakNo, islemHareket.havaleNotu));

            Sonuc sonuc = new Sonuc(false, "Geri gönderme işlemi sırasında bir hata oluştu.<br>", "");

            if (islemHareket.prEvrakNo == "")
            {
                sonuc.DegerAta("PrEvrakNo bulunamadı.");
                return sonuc;
            }

            bool surecTarafindanIslem = false;
            bool surecEvrak = false;
            EvrakSinif evrak = null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            ObjectArray tmpEvrak = Listele(ve, kullanan, new EvrakSinif() { prEvrakNo = islemHareket.prEvrakNo }, 0, 0);
            if (tmpEvrak.sonuc.islemSonuc && tmpEvrak.objeler.Count > 0)
            {
                sonuc = tmpEvrak.sonuc;
                evrak = (EvrakSinif)tmpEvrak.objeler[0];

                string evrakSahibiTcKimlikNo = "";
                string geriGonderilecekTcKimlikNo = "";

                //bool evrakImzaliMi = !evrak.imzaTarihi.isNull;
                //if (!evrakImzaliMi)
                //    evrakImzaliMi = EvrakImzaliMi(ve, kullanan, evrak.evrEvrakNo);

                //if (evrakImzaliMi || evrak.hareket.islem == (int)ENUMEvrakHareketDurum.IMZADANGERIALINDI)
                //{
                if (evrak.imzaYolu > 0)
                {
                    //İmza yolu varsa
                    EvrakImzaYolu evrakImzaYolu = EVREvrakImzaYolu.Oku(ve, kullanan, new EvrakImzaYolu() { prEvrakNo = evrak.prEvrakNo });
                    if (evrakImzaYolu != null || evrakImzaYolu.imzaYolu.kod > 0)
                    {
                        ImzaYolu imzaYolu = evrakImzaYolu.imzaYolu;

                        if (imzaYolu.detaylar != null && imzaYolu.detaylar.Length > 0)
                        {
                            evrakSahibiTcKimlikNo = imzaYolu.detaylar[0].tcKimlikNo;
                            if (kullanan.mernis == evrakSahibiTcKimlikNo)
                                sonuc.DegerAta("Evrak <b>geri gönder</b> işlemi için uygun değildir. <br>");

                            foreach (var item in imzaYolu.detaylar)
                            {
                                if (item.tcKimlikNo == islemHareket.kisi1)
                                {
                                    geriGonderilecekTcKimlikNo = item.tcKimlikNo;
                                    break;
                                }
                            }
                        }
                        else
                            sonuc.DegerAta("İmza yolu bilgisini güncelleyiniz.");
                    }
                    else
                        sonuc.DegerAta("İmza yolu bilgisine göre paraf/imza atacak kişiler belirtilmemiş.");

                }
                else if (!string.IsNullOrEmpty(evrak.dinamikSurecNo))
                {
                    //Süreç evrak işlemi yapılıyor.
                    surecEvrak = true;

                    if (islemHareket.havaleTur == 100) //İş süreçleri ile evrakın geri gönderildiğini anlamak için
                    {
                        surecTarafindanIslem = true;
                        if (islemHareket.kisi1 != islemHareket.kisi2) //Gönderen ile gönderilecek kişi aynı kişi olamaz
                        {
                            geriGonderilecekTcKimlikNo = islemHareket.kisi1;

                            EvrakHareket yeniHareketi = new EvrakHareket();
                            yeniHareketi.prEvrakNo = evrak.prEvrakNo;
                            yeniHareketi.islem = (int)ENUMEvrakHareketDurum.YENI;
                            ObjectArray listeHareket = EVREvrakHareket.Listele(ve, yeniHareketi);
                            foreach (EvrakHareket h in listeHareket.objeler)
                                yeniHareketi = h;

                            evrakSahibiTcKimlikNo = yeniHareketi.kisi1;
                        }
                    }

                }
                else
                    sonuc.DegerAta("İmza yolu bulunamadı. Geri gönderme işlemi yapılamaz.");

                //}
                //else
                //    sonuc.DegerAta("Evrak <b>geri gönder</b> işlemi için uygun değildir. <br>Paraf/imza işlemi yapılmamış.<br><br><b>Not:</b> Bu durumda evrakı havale yöntemi ile geri gönderiniz.");


                if (sonuc.islemSonuc && (surecEvrak == false || (surecEvrak && surecTarafindanIslem)))
                {

                    if (!string.IsNullOrEmpty(geriGonderilecekTcKimlikNo))
                    {
                        ve.BeginTransaction();


                        if (!evrak.imzaTarihi.isNull && evrakSahibiTcKimlikNo == geriGonderilecekTcKimlikNo)
                            sonuc = EvrakImzaKaldir(ve, kullanan, evrak.evrEvrakNo, evrak);

                        if (sonuc.islemSonuc && surecEvrak == false)
                        {
                            EvrakImzaYoluHareket evrImzaYoluHareket = new EvrakImzaYoluHareket()
                            {
                                prEvrakNo = evrak.prEvrakNo,
                                kisi1 = kullanan.mernis,
                                kisi2 = geriGonderilecekTcKimlikNo,
                                islemTip = islemHareket.islem,
                                islemTarih = new TNSDateTime(DateTime.Now)
                            };
                            sonuc = EVREvrakImzaYolu.Kaydet(ve, kullanan, evrImzaYoluHareket);
                        }

                        if (sonuc.islemSonuc)
                            sonuc = EVREvrakHareket.Kaydet(ve, kullanan, islemHareket);

                        if (sonuc.islemSonuc)
                        {
                            ve.Commit();
                            sonuc.bilgiStr = "Geri gönderme işlemi tamamlandı.";
                        }
                        else
                            ve.Rollback();
                    }
                    else
                        sonuc.DegerAta("Geri gönderilecek kullanıcı bulunamadı.");

                }

            }

            ve.VeriErisimKapat();

            Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - GeriGonder2 - SonucVeriErisimKapat - {31A86773-8703-44C3-B33A-4F0AA5F85652} - " + string.Format("Sonuc:{0}", sonuc.islemSonuc ? "True" : sonuc.hataStr));

            if (sonuc.islemSonuc && surecEvrak && evrak != null && surecTarafindanIslem == false)
            {
                try
                {
                    //Süreç Geri Gönder
                    WCFClient wcf = new WCFClient();

                    Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - GeriGonder2 - LoginCentraCube - {B6C09DF0-DDA6-46CC-87DC-8E9F90729301} - " + string.Format("workflowName:{0} - userID:{1} - password:{2} - token:{3} ", "", kullanan.kullaniciKodu, kullanan.parola, ""));
                    UserInfo userInfo = TNS.EVR.Arac.LoginCentraCube("", kullanan.kullaniciKodu, kullanan.parola, "");

                    Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - GeriGonder2 - UserInfo.TokenId - {A1A37F6B-E9A8-44B7-881E-59A7D0AF0001} - " + string.Format("userInfo.TokenId:{0}", userInfo.TokenId));

                    Dictionary<string, object> inputs = new Dictionary<string, object>();
                    inputs.Add("Result", false);
                    inputs.Add("Reason", islemHareket.havaleNotu);

                    Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - GeriGonder2 - RunWorkFlowByWorkflowInstanceID - {F51B7360-F51F-415F-841A-1007599A2B40} - " + string.Format("kullanan.mernis:{0} - kullanan.kullaniciKodu:{1} - evrak.dinamikSurecNo:{2} - inputs:{3} - userInfo.TokenId:{4}", kullanan.mernis, kullanan.kullaniciKodu, evrak.dinamikSurecNo, "inputs; Result=false, Reason=" + islemHareket.havaleNotu, userInfo.TokenId));

                    string msg = wcf.RunWorkFlowByWorkflowInstanceID(kullanan.mernis, kullanan.kullaniciKodu, Guid.Parse(evrak.dinamikSurecNo), inputs, userInfo.TokenId);

                    Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - GeriGonder2 - RunWorkFlowByWorkflowInstanceID için dönen değer - {5C2905D9-E740-4D9B-A711-9E4BC2F14C6B} - " + string.Format("msg:{0}", msg != null ? msg : "null"));

                    if (!string.IsNullOrEmpty(JSONDegerGetir(msg, "Mesaj")))
                        sonuc.DegerAta(msg);
                }
                catch (Exception ex)
                {
                    Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - GeriGonder2 - try catch Exception - {8CAA94F6-6684-441A-A451-71396FD2518A} - " + string.Format("Message:{0} - StackTrace:{1}", ex.Message, ex.StackTrace));

                    sonuc.DegerAta("Süreç Geri Gönder : " + ex.Message);
                    Arac.HataStrYaz("{93ADDAFC-6D53-46BA-B75D-19D767723D81} - Süreç Geri Gönder Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }
            }

            Arac.HataStrYaz("TNS.EVR.IsKurallari.EVREvrak.cs - GeriGonder2 - Sonuc - {05C9960F-7547-4CA1-8407-A35EACB408E3} - " + string.Format("Sonuc:{0}", sonuc.islemSonuc ? "True" : sonuc.hataStr));

            return sonuc;
        }

        public static Sonuc EvrakImzaKaldir(VeriErisim veriErisim, Kullanici kullanan, string evrEvrakNo, EvrakSinif evrak = null)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (!string.IsNullOrWhiteSpace(evrEvrakNo))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                TABEvrak tab = new TABEvrak(ve);

                bool eImzaKaldirKontrolu = true;
                if (evrak == null)
                {
                    ObjectArray tmpEvrak = Listele(ve, kullanan, new EvrakSinif() { evrEvrakNo = evrEvrakNo }, 0, 0);
                    if (tmpEvrak.sonuc.islemSonuc && tmpEvrak.objeler.Count > 0)
                        evrak = (EvrakSinif)tmpEvrak.objeler[0];
                }
                else
                    eImzaKaldirKontrolu = false; //Geri Gönder ile gönderildiğinde

                if (evrak != null)
                {
                    if (string.IsNullOrEmpty(evrEvrakNo))
                        evrEvrakNo = evrak.evrEvrakNo;

                    //if (evrak.imzaBitti == 1)
                    //    sonuc.DegerAta("Evrakın imza işlemi tamamlanmıştır. Evrak üzerinde değişiklik yapılamaz.");
                    //else 

                    if (evrak.imzaTarihi.isNull)
                        sonuc.DegerAta("Evrak imzalı değil veya imza kaldırılmış.");
                    else if (eImzaKaldirKontrolu && evrak.eImzalandi == 1 && evrak.eImzalayan != kullanan.ad + " " + kullanan.soyad)
                    {
                        if (evrak.imzaYolu > 0)
                        {
                            //İmza Yolu Bitti Evrak İlk Hazırlayana gidecek
                            EvrakHareket yeniHareketi = new EvrakHareket();
                            yeniHareketi.prEvrakNo = evrak.prEvrakNo;
                            yeniHareketi.islem = (int)ENUMEvrakHareketDurum.EIMZALANDI;
                            ObjectArray listeHareket = EVREvrakHareket.Listele(ve, yeniHareketi);
                            foreach (EvrakHareket h in listeHareket.objeler)
                                yeniHareketi = h;

                            //Geri Gönder
                            EvrakHareket hareket = new EvrakHareket();
                            hareket.prEvrakNo = evrak.prEvrakNo;
                            hareket.islem = (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN;
                            hareket.birimKod1 = yeniHareketi.birimKod2;
                            hareket.birimKod2 = evrak.hareket.birimKod2;
                            hareket.kisi1 = yeniHareketi.kisi2;
                            hareket.kisi2 = kullanan.mernis;
                            hareket.havaleNotu = "E-imzanın kaldırılması için gönderilmiştir.";

                            hareket.vekil = evrak.hareket.vekil;
                            hareket.vekilAd = evrak.hareket.vekilAd;

                            if (yeniHareketi.kisi1 != kullanan.mernis)
                                sonuc.DegerAta("Evrakın son e-imza atan kullanıcı tarafandan e-imzası kaldırılabilir!" + sonuc.hataStr);
                            else if (hareket.kisi1 != hareket.kisi2)
                            {
                                sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                                if (!sonuc.islemSonuc)
                                    sonuc.DegerAta("Evrak imza sırasında işlem hareketlerinde hata oluştu." + sonuc.hataStr);
                            }
                            else
                                sonuc.DegerAta("Bu işlemi yapma yetkiniz bulunmuyor!" + sonuc.hataStr);

                        }
                        else
                            sonuc.DegerAta(string.Format("Evrakın imzası kaldırılamadı!<br>Not: E-imzalı evrakın imzasını imza sahibi kaldırmalıdır.<br>İmza sahibi : <b>{0}</b>", evrak.eImzalayan));
                    }
                    else
                    {
                        ve.BeginTransaction();

                        sonuc = tab.Guncelle(new EvrakSinif()
                        {
                            evrEvrakNo = evrEvrakNo,
                            imzaTarihi = null,
                            eImzalandi = 0,
                            eImzalayan = null,
                            eImzaTarihi = null,
                            imzaBitti = 0
                        });

                        if (sonuc.islemSonuc)
                        {
                            try
                            {
                                EvrakDosya[] eDosyaListe = EVROCR.EvrakDosyaListele(ve, kullanan, new EvrakDosya() { evrEvrakNo = evrEvrakNo }, 50, true);

                                //bool eimzaDosyasiVar = false;

                                if (eDosyaListe != null || eDosyaListe.Length > 0)
                                {
                                    foreach (var eDosya in eDosyaListe)
                                    {
                                        if (eDosya.iptal == 1) continue;
                                        if (eDosya.eImzaDosyasi != 1) continue;

                                        //eimzaDosyasiVar = true;

                                        if (eDosya.dosyaAdi.EndsWith("p7s") || eDosya.dosyaAdi.EndsWith("imz"))
                                        {
                                            FileInfo fi = new FileInfo(eDosya.dosyaYolu + "\\" + eDosya.dosyaAdi);
                                            if (fi.Exists)
                                            {
                                                fi.MoveTo(fi.Directory.FullName + "\\" + Guid.NewGuid() + ".bak");
                                                sonuc = EVROCR.EvrakDosyaGuncelle(ve, kullanan, new EvrakDosya() { evrEvrakNo = eDosya.evrEvrakNo, siraNo = eDosya.siraNo, iptal = 1 });
                                                if (!sonuc.islemSonuc)
                                                    break;
                                            }
                                            else
                                            {
                                                sonuc.DegerAta("Evrak imza kaldırma işlemi sırasında " + eDosya.dosyaAdi + " isimli dosya bulunamadı.");
                                                break;
                                            }
                                        }
                                    }

                                }

                                if (sonuc.islemSonuc)
                                {
                                    //İmza yolu hareketleri silinecek
                                    sonuc = EVREvrakImzaYolu.Sil(ve, kullanan, new EvrakImzaYoluHareket() { prEvrakNo = evrak.prEvrakNo });

                                    //Hareket tarihçesi ekle
                                    //if (sonuc.islemSonuc)
                                    //{
                                    //    EvrakHareket hareket = new EvrakHareket();
                                    //    hareket.prEvrakNo = evrak.prEvrakNo;
                                    //    hareket.islem = (int)ENUMEvrakHareketDurum.IMZAPARAFKALDIRILDI;
                                    //    hareket.birimKod1 = evrak.hareket.birimKod1;
                                    //    hareket.birimKod2 = evrak.hareket.birimKod2;
                                    //    hareket.kisi1 = evrak.hareket.kisi1;
                                    //    hareket.kisi2 = evrak.hareket.kisi2;
                                    //    hareket.havaleNotu = "";
                                    //    hareket.vekil = "";
                                    //    hareket.vekilAd = "";

                                    //    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                                    //}

                                    if (sonuc.islemSonuc)
                                    {
                                        //Tarihçesini Kaydet
                                        EvrakTarihce tarihce = new EvrakTarihce();
                                        tarihce.evrEvrakNo = evrEvrakNo;
                                        tarihce.islem = ENUMEvrakTarihceDurum.IMZAPARAFKALDIRILDI.ToString();

                                        sonuc = EVREvrakTarihce.Kaydet(ve, kullanan, tarihce);
                                    }
                                }

                                //if (!eimzaDosyasiVar)
                                //    sonuc.islemSonuc = true;//e-imza dosyası yok ise sonuc true olmalı

                            }
                            catch (Exception ex)
                            {
                                sonuc.DegerAta(ex.Message);
                            }
                        }

                        if (sonuc.islemSonuc)
                        {
                            string personIdnp = Arac.PinKoduGetir(kullanan, kullanan.mernis);

                            InfocomDosyaImza dosyaImza = new InfocomDosyaImza();
                            dosyaImza.evrEvrakNo = evrEvrakNo;
                            dosyaImza.siraNo = 0;
                            dosyaImza.personIdnp = personIdnp;

                            sonuc = EVRInfocomDosyaImza.Sil(ve, kullanan, dosyaImza);
                            if (!sonuc.islemSonuc)
                                sonuc.DegerAta("Evrak imza kaldırma işlemi sırasında " + sonuc.hataStr);
                        }


                        if (sonuc.islemSonuc)
                        {
                            //Hareket tarihçesi ekle
                            //if (sonuc.islemSonuc)
                            //{
                            //    EvrakHareket hareket = new EvrakHareket();
                            //    hareket.prEvrakNo = evrak.prEvrakNo;
                            //    hareket.islem = (int)ENUMEvrakHareketDurum.IMZAPARAFKALDIRILDI;
                            //    hareket.birimKod1 = evrak.hareket.birimKod1;
                            //    hareket.birimKod2 = evrak.hareket.birimKod2;
                            //    hareket.kisi1 = evrak.hareket.kisi1;
                            //    hareket.kisi2 = evrak.hareket.kisi2;
                            //    hareket.havaleNotu = "";
                            //    hareket.vekil = "";
                            //    hareket.vekilAd = "";

                            //    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                            //}

                            if (evrak.imzaYolu > 0)
                            {
                                //İmza Yolu Bitti Evrak İlk Hazırlayana gidecek
                                EvrakHareket yeniHareketi = new EvrakHareket();
                                yeniHareketi.prEvrakNo = evrak.prEvrakNo;
                                yeniHareketi.islem = (int)ENUMEvrakHareketDurum.YENI;
                                ObjectArray listeHareket = EVREvrakHareket.Listele(ve, yeniHareketi);
                                foreach (EvrakHareket h in listeHareket.objeler)
                                    yeniHareketi = h;

                                //Geri Gönder
                                EvrakHareket hareket = new EvrakHareket();
                                hareket.prEvrakNo = evrak.prEvrakNo;
                                hareket.islem = (int)ENUMEvrakHareketDurum.GERIGONDERILDI;
                                hareket.birimKod1 = yeniHareketi.birimKod1;
                                hareket.birimKod2 = evrak.hareket.birimKod1;
                                hareket.kisi1 = yeniHareketi.kisi1;
                                hareket.kisi2 = kullanan.mernis;
                                hareket.havaleNotu = "";
                                hareket.vekil = evrak.hareket.vekil;
                                hareket.vekilAd = evrak.hareket.vekilAd;

                                if (hareket.kisi1 != hareket.kisi2)
                                {
                                    sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                                    if (!sonuc.islemSonuc)
                                        sonuc.DegerAta("Evrak imza sırasında işlem hareketlerinde hata oluştu." + sonuc.hataStr);
                                }
                                else
                                    sonuc.DegerAta("Bu işlemi yapma yetkiniz bulunmuyor." + sonuc.hataStr);

                            }

                        }


                        if (sonuc.islemSonuc)
                        {
                            ve.Commit();
                            sonuc.bilgiStr = "Evrakın imzası başarıyla kaldırıldı.";
                        }
                        else
                            ve.Rollback();


                    }
                }
                else
                    sonuc.DegerAta("İmza kaldırma işleminde evrak bilgisine ulaşılamadı.");

                ve.VeriErisimKapat();
            }
            else
                sonuc.DegerAta("İmzası kaldırılacak evrak bulunamadı.");

            return sonuc;
        }

        public static Sonuc EtiketKaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI079, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrak to = new TABEvrak(ve);
                string sahibi = "";
                foreach (Etiket bilgi in liste.objeler)
                {
                    if (string.IsNullOrEmpty(bilgi.ad)) continue;
                    if (string.IsNullOrEmpty(bilgi.sahibi)) continue;

                    sahibi = bilgi.sahibi;

                    if (bilgi.kod == 0)
                    {
                        bilgi.kod = OrtakFonksiyonlar.ConvertToInt(new TABGenel(ve).BelgeNoAl("TANIMETIKET", "0"), 0);
                        sonuc = to.EtiketKaydet(bilgi);
                        sonuc.anahtar = bilgi.kod.ToString();
                    }
                    else
                    {
                        bilgi.ustKod = -1;//Güncelleme yapmasın
                        sonuc = to.EtiketGuncelle(bilgi);
                    }

                    if (!sonuc.islemSonuc)
                        break;
                }

                if (sonuc.islemSonuc)
                {
                    to.EtiketGuncelleDetay(sahibi);
                    ve.Commit();
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{95E929D5-A28E-4B33-8001-55DE6CE2883F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray EtiketListele(VeriErisim veriErisim, Etiket bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrak to = new TABEvrak(ve);
            IDataReader r = to.EtiketListele(bilgi);

            if (r == null)
            {
                ve.VeriErisimKapat();
                hVAN.sonuc.hataStr = EvrakI.EVRI029;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new Etiket();

                    bilgi.kod = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanEtiketTanim.KOD, 0);
                    bilgi.ad = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanEtiketTanim.AD, "");
                    bilgi.ustKod = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanEtiketTanim.USTKOD, 0);
                    bilgi.sahibi = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanEtiketTanim.SAHIBI, "");
                    bilgi.detay = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanEtiketTanim.DETAY, 0);
                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        public static Sonuc EtiketUstKodDegistir(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI079, "");
            string sahibi = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrak to = new TABEvrak(ve);
                foreach (Etiket bilgi in liste.objeler)
                {
                    if (bilgi.kod == 0) continue;

                    sahibi = bilgi.sahibi;
                    bilgi.ad = "";
                    sonuc = to.EtiketGuncelle(bilgi);

                    if (sonuc.islemSonuc)
                        break;
                }

                if (sonuc.islemSonuc)
                {
                    to.EtiketGuncelleDetay(sahibi);
                    ve.Commit();
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{95E929D5-A28E-4B33-8001-55DE6CE2883F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc EtiketSil(VeriErisim veriErisim, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI161, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrak to = new TABEvrak(ve);
                string sahibi = "";
                foreach (Etiket etiket in liste.objeler)
                {
                    sahibi = etiket.sahibi;
                    if (sahibi == "") continue;

                    sonuc = to.EtiketSil(etiket);
                }

                if (sonuc.islemSonuc)
                {
                    to.EtiketGuncelleDetay(sahibi);
                    ve.Commit();
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{FB5C9281-C861-497E-AB71-4397ACF245FD} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc EvrakEtiketKaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI079, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();

            TABEvrak to = new TABEvrak(ve);
            foreach (EvrakEtiket bilgi in liste.objeler)
            {
                if (bilgi.etiketKod == 0) continue;
                if (string.IsNullOrEmpty(bilgi.prEvrakNo)) continue;

                to.EvrakEtiketKaydet(bilgi);
                ve.hata = "";//Hataları gözardı et. Duplicate key hatası
            }

            //if (sonuc.islemSonuc)
            ve.Commit();
            //else
            //    ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc EvrakEtiketSil(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI079, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();

            TABEvrak to = new TABEvrak(ve);
            foreach (EvrakEtiket bilgi in liste.objeler)
            {
                if (bilgi.etiketKod == 0) continue;
                if (string.IsNullOrEmpty(bilgi.prEvrakNo)) continue;

                to.EvrakEtiketSil(bilgi);
                ve.hata = "";//Hataları gözardı et. Duplicate key hatası
            }

            //if (sonuc.islemSonuc)
            ve.Commit();
            //else
            //    ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc EvrakGizlikSureUzatimiKaydet(VeriErisim veriErisim, Kullanici kullanan, string[] evrEvrakNolar, TNSDateTime sureSonuTarih)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";

            if (evrEvrakNolar == null || evrEvrakNolar.Length == 0)
                hata = "İşlem yapılacak Evrak Referans Numarası boş bırakılmış<br>";

            if (sureSonuTarih.isNull)
                hata += "Süre sonu tarih bilgisi boş bırakılmış.<br>";

            if (!(kullanan.KullaniciTipiIceriyorMu((int)TNS.KYM.ENUMKullaniciTipi.EVRAKBELGEYONETICISI) || kullanan.KullaniciTipiIceriyorMu((int)TNS.KYM.ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI)))
                hata += "İşlem yapmaya yetkiniz yok.<br>";

            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            //EvrakBelgeYöneticisinin ilgili birime yetkisi var mı? Kontrolü konulacak

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            ve.BeginTransaction();

            foreach (string evrEvrakNo in evrEvrakNolar)
            {
                EvrakSinif e = new EvrakSinif();
                e.evrEvrakNo = evrEvrakNo;
                e.gizlilikGecerlilikSuresi = sureSonuTarih;

                sonuc = to.Kaydet(e);
                if (!sonuc.islemSonuc) break;
                else
                    if (sonuc.islemSonuc)
                {
                    EvrakTarihce tarihce = new EvrakTarihce();
                    tarihce.evrEvrakNo = evrEvrakNo;
                    tarihce.islem = string.Format("{0} evrakın gizlilik geçerlilik süresi {1} tarihi olarak değiştirildi.", evrEvrakNo, sureSonuTarih);
                    sonuc = EVREvrakTarihce.Kaydet(ve, kullanan, tarihce);
                }

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

        public static Sonuc EvrakGizlikDerecesiKaydet(VeriErisim veriErisim, Kullanici kullanan, string[] evrEvrakNolar, int gizlilikTip)
        {
            IEVRServis servisEVR = TNS.EVR.Arac.Tanimla();
            Sonuc sonuc = new Sonuc();
            string hata = "";

            if (evrEvrakNolar == null || evrEvrakNolar.Length == 0)
                hata = "İşlem yapılacak Evrak Referans Numarası boş bırakılmış<br>";

            if (gizlilikTip == 0)
                hata += "Süre gizlilik derecesi bilgisi boş bırakılmış.<br>";

            if (!(kullanan.KullaniciTipiIceriyorMu((int)TNS.KYM.ENUMKullaniciTipi.EVRAKBELGEYONETICISI) || kullanan.KullaniciTipiIceriyorMu((int)TNS.KYM.ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI)))
                hata += "İşlem yapmaya yetkiniz yok.<br>";

            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            //EvrakBelgeYöneticisinin ilgili birime yetkisi var mı? Kontrolü konulacak

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak to = new TABEvrak(ve);
            ve.BeginTransaction();

            foreach (string evrEvrakNo in evrEvrakNolar)
            {
                EvrakSinif e = new EvrakSinif();
                e.evrEvrakNo = evrEvrakNo;
                e.gizlilik = gizlilikTip;

                sonuc = to.Kaydet(e);
                if (!sonuc.islemSonuc) break;
                else
                    if (sonuc.islemSonuc)
                {
                    EvrakTarihce tarihce = new EvrakTarihce();
                    tarihce.evrEvrakNo = evrEvrakNo;
                    tarihce.islem = string.Format("{0} evrakın gizlilik geçerlilik tipi {1} olarak değiştirildi.", evrEvrakNo, servisEVR.GizlilikTipiGetir(gizlilikTip));
                    sonuc = EVREvrakTarihce.Kaydet(ve, kullanan, tarihce);
                }

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

        public static ObjectArray ListeleDosyayaKaldirilmisEvraklar(VeriErisim veriErisim, EvrakSinif bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);


            TABEvrak to = new TABEvrak(ve);
            IDataReader r = to.ListeleDosyayaKaldirilmisEvraklar(bilgi);

            if (r == null)
            {
                ve.VeriErisimKapat();
                hVAN.sonuc.hataStr = EvrakI.EVRI029;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new EvrakSinif();

                    bilgi.evrEvrakNo = VeriErisim.DegerAl(r, 0, "");
                    bilgi.evrakNo = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.evrakTarih = VeriErisim.DegerAl(r, 2);
                    bilgi.konu = VeriErisim.DegerAl(r, 3, "");
                    bilgi.arsivlemeTarihi = VeriErisim.DegerAl(r, 4);
                    bilgi.tasfiye.tasfiyeKod = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.saklama.bitisTarih = VeriErisim.DegerAl(r, 6);
                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        public static Sonuc ImzadanGeriAl(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);
            int basarili = 0;

            foreach (EvrakSinif evrak in liste.objeler)
            {
                ObjectArray evraklar = Listele(ve, kullanan, new EvrakSinif() { evrEvrakNo = evrak.evrEvrakNo, prEvrakNo = evrak.prEvrakNo });
                if (evraklar.ObjeSayisi > 0)
                {
                    EvrakSinif form = (EvrakSinif)evraklar[0];

                    if (form.hareket.islem != (int)ENUMEvrakHareketDurum.IMZAYAGONDER)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = "Evrak hareket geçişi uygun değil. Evrak üzerinde gönderilen kullanıcı tarafından işlem yapılmış. Bu nedenle evrak imzadan geri alınamaz!";
                        break;
                    }

                    if (form.hareket.islemYapan != kullanan.mernis)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = "Evrak üzerindeki son hareket size ait olmadı için, evrak imzadan geri alınamaz!";
                        break;
                    }
                    else
                    {
                        EvrakHareket hareket = new EvrakHareket();
                        hareket.prEvrakNo = evrak.prEvrakNo;
                        hareket.birimKod1 = form.hareket.birimKod2;
                        hareket.birimKod2 = form.hareket.birimKod1;
                        hareket.kisi1 = kullanan.mernis;
                        hareket.kisi2 = form.hareket.kisi1;
                        hareket.islem = (int)ENUMEvrakHareketDurum.IMZADANGERIALINDI;
                        hareket.havaleNotu = "";
                        hareket.vekil = evrak.hareket.vekil;
                        hareket.vekilAd = evrak.hareket.vekilAd;

                        sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);

                        if (sonuc.islemSonuc)
                            sonuc = EVREvrak.EvrakImzaKaldir(ve, kullanan, form.evrEvrakNo);

                        if (sonuc.islemSonuc)
                            basarili++;
                        else
                            break;
                    }

                }
                else
                    sonuc.hataStr = "Evrak bilgisi alınamadı";
            }

            if (sonuc.hataStr == "")
                sonuc.islemSonuc = true;
            else
                sonuc.islemSonuc = false;

            sonuc.anahtar = basarili.ToString();
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static bool ImzaliEvrakDegisiklikYapilabilirMi(string birimKod)
        {
            return EVREvrak.YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu.EvrakImzaliEvrakDegisiklikYapilsin, birimKod);
        }

        public static bool BilgiIcinGelenEvrakGeriGonderilebilsinMi(string birimKod)
        {
            return EVREvrak.YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu.EvrakBilgiIcinGelenEvrakGeriGonderilebilsin, birimKod);
        }

        public static EvrakSicil[] ListeleSicil(VeriErisim veriErisim, Kullanici kullanan, EvrakSicil form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            IDataReader dr = tab.ListeleSicil(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakSicil();
                    form.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrak.ENUMAlanlarSicil.EVREVRAKNO, "");
                    form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrak.ENUMAlanlarSicil.PREVRAKNO, "");
                    form.asil = VeriErisim.DegerAl(dr, (int)TABEvrak.ENUMAlanlarSicil.ASIL, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakSicil[])liste.objeler.ToArray(typeof(EvrakSicil));
        }

        public static EvrakHareket IslemSonHareketGetir(VeriErisim veriErisim, string prEvrakNo, params ENUMEvrakHareketDurum[] hareketIslem)
        {
            EvrakHareket hareketDonenDeger = new EvrakHareket();
            EvrakHareket hareketSorgu = new EvrakHareket();

            int siraNo = 0;
            foreach (var islem in hareketIslem) //birden fazla parametere gönderilirse sıra numarası büyük olan değer döndürülür.
            {
                hareketSorgu.prEvrakNo = prEvrakNo;
                hareketSorgu.islem = (int)islem;

                ObjectArray liste = EVREvrakHareket.Listele(veriErisim, hareketSorgu);
                if (liste.objeler.Count > 0)
                {
                    foreach (EvrakHareket h in liste.objeler)
                    {
                        if (h.siraNo > siraNo)
                        {
                            siraNo = h.siraNo;
                            hareketDonenDeger = h;
                        }
                    }
                }
            }

            return hareketDonenDeger;
        }

        public static string[] YetkiliEvrakBirimleriniGetir(string mernis)
        {
            string[] birimler = null;
            Kullanici mernisKullanici = null;

            Kullanici tmpKullanici = new Kullanici();
            tmpKullanici.kullaniciTipi = new ENUMKullaniciTipi[1];
            tmpKullanici.kullaniciTipi[0] = ENUMKullaniciTipi.SISTEMYONETICISI;

            IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();
            KullaniciVAN kVAN = servisKYM.KullaniciOku(tmpKullanici, mernis);
            if (kVAN.sonuc.islemSonuc && kVAN.kullanici != null)
            {
                if (kVAN.kullanici.kullaniciKodu != "")
                    mernisKullanici = kVAN.kullanici;
            }

            if (mernisKullanici != null)
                birimler = TNS.EVR.Yetki.KullaniciEvrakYetkiliBirimleri(mernisKullanici, ENUMOkumaYazma.OKUMA).Trim().Split(';');

            return birimler;
        }

        public static Sonuc SiraNoDegistir(string primaryKey, int siraNo1, int siraNo2, ENUMSiraNoDegistirTablo tabloAd)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABEvrak tab = new TABEvrak(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.SiraNoDegistir(primaryKey, siraNo1, siraNo2, tabloAd);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                Arac.HataStrYaz("{D2EB246F-3638-44E2-AD1B-A0F75FC73FCE} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc IsTalepKaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif evrak)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            EvrakIsTalep evrakIsTalep = new EvrakIsTalep();


            try
            {
                String atananKisi = (evrak.evrakTipi == (int)ENUMEvrakTipi.GELEN ? evrak.hareket.kisi2 : evrak.hareket.kisi1);
                String atananKisiAd = EVRGenel.KullaniciAdGetir(atananKisi);
                String tahminiBitisTarih = "";
                if (evrak.cevapSuresi > 0)
                {
                    DateTime tarih = evrak.evrakTarih.Oku().AddDays(evrak.cevapSuresi);
                    if (tarih <= DateTime.Now)
                        tarih = DateTime.Now;

                    tahminiBitisTarih = tarih.ToShortDateString();
                }

                IsTalepWS isTalep = new IsTalepWS();
                //talep.isTalepKonuKod = 0;//Konu kodları olmalı şimdi 0 olsun
                isTalep.isTalepKonuAd = "";
                isTalep.altKonuAdi = "Gelen evrak";
                isTalep.tip = "5";                //1:Hata, 2:Talep, 3:Öneri, 4:Destek, 5:Görev
                isTalep.oncelik = "1";            //1:Normal, 2:Önemli, 3:Çok Önemli
                isTalep.aciklama = evrak.konu;
                isTalep.yorum = "";
                //talep.kayitEdenKod = "kullanan.mernis";
                isTalep.kayitEdenAdi = kullanan.ADSOYAD;
                //talep.atananKisiKod = "atananKisi";
                isTalep.atananKisiAdi = atananKisiAd;
                isTalep.tahminiBaslamaTarih = DateTime.Now.ToShortDateString();
                isTalep.tahminiBitisTarih = tahminiBitisTarih;


                evrakIsTalep = new EvrakIsTalep()
                {
                    kod = 0,
                    prEvrakNo = evrak.prEvrakNo,
                    servisKayitKod = "",
                    atananKisi = atananKisi,
                    atananKisiAd = isTalep.atananKisiAdi,
                    islemYapan = kullanan.mernis,
                    islemYapanAd = isTalep.kayitEdenAdi,
                    islemTarihi = new TNSDateTime(DateTime.Now),
                    islem = ENUMEvrakIsTalepIslem.YENIKAYIT,
                    islemAciklama = "",
                    detay = new JavaScriptSerializer().Serialize(isTalep)
                };

                sonuc = EVREvrakIsTalep.Kaydet(veriErisim, kullanan, evrakIsTalep);

                if (sonuc.islemSonuc)
                {

                    evrakIsTalep = new EvrakIsTalep();
                    evrakIsTalep.kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);
                    evrakIsTalep.servisKayitKod = "";


                    ProjeServis projeServis = new ProjeServis();

                    string url = System.Configuration.ConfigurationManager.AppSettings.Get(ENUMWebConfigEvrakAyarlarSunucu.EvrakProjeServisUrl.ToString()) + "";
                    if (url != "")
                        projeServis.Url = url + "/servis/projeservis.asmx";


                    string strSonuc = null;
                    try
                    {
                        strSonuc = projeServis.IsTalepKaydet(kullanan.kullaniciKodu, kullanan.parola, isTalep);
                    }
                    catch (Exception ex)
                    {
                        evrakIsTalep.islem = ENUMEvrakIsTalepIslem.SERVISEERISIMYOK;
                        evrakIsTalep.islemAciklama = ex.Message;
                    }


                    if (strSonuc != null)
                    {
                        if (strSonuc != "" && strSonuc.Length <= 40)
                        {
                            evrakIsTalep.servisKayitKod = strSonuc;
                            evrakIsTalep.islem = ENUMEvrakIsTalepIslem.ATANDI;
                        }
                        else
                        {
                            evrakIsTalep.islem = ENUMEvrakIsTalepIslem.HATAOLUSTU;
                            evrakIsTalep.islemAciklama = strSonuc;
                        }
                    }

                    sonuc = EVREvrakIsTalep.Kaydet(veriErisim, kullanan, evrakIsTalep);

                }

            }
            catch (Exception ex)
            {
                Arac.HataStrYaz("{FF998EEF-DD55-4C0C-ACEC-2611087C156D} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);

                try
                {
                    evrakIsTalep.islem = ENUMEvrakIsTalepIslem.HATAOLUSTU;
                    evrakIsTalep.islemAciklama = sonuc.hataStr + " - Message: " + ex.Message;
                    sonuc = EVREvrakIsTalep.Kaydet(veriErisim, kullanan, evrakIsTalep);
                }
                catch (Exception ex2) { }

                sonuc.islemSonuc = false;
                sonuc.hataStr = "Bilinmeyen bir hata oluştu. Lütfen sistem yöneticisine başvurunuz.<br>Hata Kod : {FF998EEF-DD55-4C0C-ACEC-2611087C156D} - " + DateTime.Now.ToLongTimeString();
            }

            return sonuc;
        }

    }
}

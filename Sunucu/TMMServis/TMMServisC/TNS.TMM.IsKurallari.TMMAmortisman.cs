using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;
using System.Collections;
using TNS.MUH;
using System.Collections.Generic;
using TNS.HRC;
using System.Runtime.InteropServices;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Amortisman bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>

    public class TMMAmortisman
    {
        /// <summary>
        /// Amortismanla ilgili genel hata bilgisini tutan değişken
        /// </summary>
        public static string amortismanHata = TasinirMalResI.TMM035;

        /// <summary>
        /// Amortisman işlemleri değerlerini tutan liste
        /// </summary>
        public enum ENUMIslemTur
        {
            /// <summary>
            /// Otomatik kaydetme işlemi değeri
            /// </summary>
            KAYDET = 1,

            /// <summary>
            /// Listeleme işlemi değeri
            /// </summary>
            LISTELE = 2,

            /// <summary>
            /// Sil işlemi değeri
            /// </summary>
            SIL = 3,

            /// <summary>
            /// Hesaplama işlemi değeri
            /// </summary>
            HESAPLA = 4,

            /// <summary>
            /// Elle kaydetme işlemi değeri
            /// </summary>
            KAYDETELLE = 5
        }

        ///// <summary>
        ///// Verilen kriterlere uygun olan herhangi bir demirbaşa amortisman uygulanmış mı kontrolünü yapan yordam
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <returns>Kontrol sonucu döndürülür.</returns>
        //public static bool AmortismanYapilmisMi(VeriErisim veriErisim, int yil)
        //{
        //    return AmortismanYapilmisMi(veriErisim, new AmortismanKriter(yil));
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan herhangi bir demirbaşa amortisman uygulanmış mı kontrolünü yapan yordam
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="hesapPlanKod">Taşınır hesap planı kodu</param>
        ///// <returns>Kontrol sonucu döndürülür.</returns>
        //public static bool AmortismanYapilmisMi(VeriErisim veriErisim, int yil, string hesapPlanKod)
        //{
        //    return AmortismanYapilmisMi(veriErisim, new AmortismanKriter(yil, hesapPlanKod));
        //}

        /// <summary>
        /// Verilen kriterlere uygun olan herhangi bir demirbaşa amortisman uygulanmış mı kontrolünü yapan yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebeKod">Muhasebe birimi</param>
        /// <param name="harcamaKod">Harcama birimi</param>
        /// <param name="belgeNo">Taşınır işlem fişi belge numarası</param>
        /// <returns>Kontrol sonucu döndürülür.</returns>
        public static bool AmortismanYapilmisMi(VeriErisim veriErisim, int yil, string muhasebeKod, string harcamaKod, string belgeNo)
        {
            return AmortismanYapilmisMi(veriErisim, new AmortismanKriter(yil, muhasebeKod, harcamaKod, belgeNo));
        }

        /// <summary>
        /// Verilen kriterlere uygun olan herhangi bir demirbaşa amortisman uygulanmış mı kontrolünü yapan yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="ak">Amortisman listeleme kriterleri</param>
        /// <returns>Kontrol sonucu döndürülür.</returns>
        public static bool AmortismanYapilmisMi(VeriErisim veriErisim, AmortismanKriter ak)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABAmortisman tAmortisman = new TABAmortisman(ve);
            bool sonuc = tAmortisman.AmortismanYapilmisMi(ak);
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc AmortismanVerisiOlustur(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak, int islemSiraNo)
        {
            if (islemSiraNo == 1)
                return AmortismanVerisiOlustur(veriErisim, kullanan, ak);
            return null;
        }

        private static Sonuc AmortismanVerisiOlustur(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            if (string.IsNullOrWhiteSpace(ak.muhasebeKod) || string.IsNullOrWhiteSpace(ak.harcamaKod) || string.IsNullOrWhiteSpace(ak.ambarKod))
            {
                return new Sonuc("Lütfen amortisman kayıtları üretilecek birim seçiniz!");
            }

            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
            {
                return new Sonuc("Lütfen lütfen hesap plan kodu girmeyiniz!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            ak.raporTur = 3;
            ObjectArray liste = AmortismanRaporla(veriErisim, kullanan, ak, true);
            if (!liste.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return liste.sonuc;
            }
            TABAmortisman tAmortisman = new TABAmortisman(ve);
            decimal enAzTutar = TMMAmortismanSinir.AmortismanSinirVer(ve, kullanan, ak.yil);
            Sonuc s;
            ve.BeginTransaction();
            s = tAmortisman.Sil(ak);
            if (s.islemSonuc)
            {
                try
                {
                    foreach (AmortismanRapor ar in liste.objeler)
                    {
                        bool ekle = false;
                        if (ar.sonGirisNo > 0)
                        {
                            if (ar.cikisNo == 0)
                            {
                                ekle = true;
                            }
                            else if (ar.sonGirisNo > ar.cikisNo)
                            {
                                ekle = true;
                            }
                        }

                        decimal amortiMiktar = ar.kalanTutar;
                        if (amortiMiktar > enAzTutar && ar.amortismanSuresi > 1)
                        {
                            if (!ar.SonYilMi)
                                amortiMiktar = (decimal)OrtakFonksiyonlar.KurusYap((double)ar.maliyetTutar / ar.amortismanSuresi);
                        }
                        if (ekle && amortiMiktar > 0)
                        {
                            s = tAmortisman.Kaydet(ak.kod, ak.yil, ak.donem, ar.prSicilNo, ar.amortismanTur, ar.degerArtisKod, amortiMiktar, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
                            if (!s.islemSonuc)
                            {
                                s.hataStr = ar.prSicilNo + "," + ar.gorSicilNo + " sicil noya sahip kayıt yapılamadı. Lütfen tüm verileri silip tekrar deneyiniz!";
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    s.DegerAta(ex.Message);
                }
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        public static Sonuc AmortismanVerisiOlustur2(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc kontrol = AmortismanKayitKontrol(ve, kullanan, ak);
            if (!kontrol.islemSonuc)
            {
                ve.VeriErisimKapat();
                return kontrol;
            }

            ak.raporTur = 3;
            ObjectArray liste = AmortismanUretimRaporu2(ve, kullanan, ak, true);
            if (!liste.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return liste.sonuc;
            }

            TABAmortisman tAmortisman = new TABAmortisman(ve);
            //decimal enAzTutar = TMMAmortismanSinir.AmortismanSinirVer(ve, kullanan, ak.yil);

            ve.BeginTransaction();
            AmortismanIslemForm form = new AmortismanIslemForm()
            {
                yil = ak.yil,
                donem = ak.donem,
                muhasebeKod = ak.muhasebeKod,
                harcamaKod = ak.harcamaKod,
                ambarKod = ak.ambarKod
            };

            Sonuc sonuc = TMMAmortismanIslemForm.Kaydet(ve, kullanan, form);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            Sonuc s = tAmortisman.Sil(ak);
            if (s.islemSonuc)
            {
                try
                {
                    foreach (AmortismanRapor ar in liste.objeler)
                    {
                        decimal amortiMiktar = ar.cariMaliyetAmortismanTutar;
                        if (amortiMiktar > 0)
                        {
                            s = tAmortisman.Kaydet(ak.kod, ak.yil, ak.donem, ar.prSicilNo, ar.amortismanTur, ar.degerArtisKod, amortiMiktar, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
                            //if (s.islemSonuc)
                            //{
                            //    //İlk amortismanda kaydedilecek. Ancak sicil özellik tablosundaki amortisman süresi boş olduğu için bir süre ilk kayıt kontrolü yapılmasın. Değer boş ise kaydedilsin.
                            //    s = TMMSicilNoOzellik.AmortismanSureKaydet(ve, kullanan, ar.prSicilNo, ar.amortismanSuresi);
                            //}
                            if (!s.islemSonuc)
                            {
                                s.hataStr = ar.prSicilNo + "," + ar.gorSicilNo + " sicil noya sahip kayıt yapılamadı. Lütfen tüm verileri silip tekrar deneyiniz!";
                                break;
                            }
                        }

                        foreach (AmortismanRaporDegerArtis ard in ar.degerArtisAmortisman.objeler)
                        {
                            amortiMiktar = ard.cariDegerlemeAmortismanTutar;
                            if (amortiMiktar > 0)
                            {
                                s = tAmortisman.Kaydet(ak.kod, ak.yil, ak.donem, ard.prSicilNo, ard.amortismanTur, ard.degerArtisKod, amortiMiktar, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
                                if (!s.islemSonuc)
                                {
                                    s.hataStr = ard.prSicilNo + "," + ard.gorSicilNo + " sicil noya sahip kayıt yapılamadı. Lütfen tüm verileri silip tekrar deneyiniz!";
                                    break;
                                }
                            }

                        }


                    }
                }
                catch (Exception ex)
                {
                    s.DegerAta(ex.Message);
                }
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        public static Sonuc AmortismanVerisiOlustur3(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc kontrol = AmortismanKayitKontrol(ve, kullanan, ak);
            if (!kontrol.islemSonuc)
            {
                ve.VeriErisimKapat();
                return kontrol;
            }

            ak.raporTur = 3;
            ObjectArray liste = AmortismanUretimRaporu3(ve, kullanan, ak, true);
            if (!liste.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return liste.sonuc;
            }

            TABAmortisman tAmortisman = new TABAmortisman(ve);
            //decimal enAzTutar = TMMAmortismanSinir.AmortismanSinirVer(ve, kullanan, ak.yil);

            ve.BeginTransaction();
            AmortismanIslemForm form = new AmortismanIslemForm()
            {
                yil = ak.yil,
                donem = ak.donem,
                muhasebeKod = ak.muhasebeKod,
                harcamaKod = ak.harcamaKod,
                ambarKod = ak.ambarKod
            };

            Sonuc sonuc = TMMAmortismanIslemForm.Kaydet(ve, kullanan, form);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            Sonuc s = tAmortisman.Sil(ak);
            if (s.islemSonuc)
            {
                try
                {
                    foreach (AmortismanRapor3 ar in liste.objeler)
                    {
                        decimal amortismanTutar = ar.maliyetCariAmortisman;
                        if (amortismanTutar == 0)
                        {
                            //Merkez Bankası için binaların geçmiş yıllardaki giriş tutarları için amortisman yapıldığında, 2 basamak yuvarlama yapıldığında sıfır gelirse, örneğin 0,47 için 0,47/(50*4) = 0,00235
                            if (!ar.SonDonemMi)
                                amortismanTutar = ar.maliyetCariAmortismanKuruslu;
                        }


                        bool degerDuzeltmedeKurusDuzenle = false;

                        if (amortismanTutar > 0)
                        {
                            foreach (AmortismanRapor3DegerArtis ard in ar.degerDuzeltmeAmortismanX.objeler)
                            {
                                if (Math.Abs(ard.maliyetCariAmortisman) > 0)
                                {
                                    degerDuzeltmedeKurusDuzenle = true;
                                    break;
                                }
                            }

                            if (!degerDuzeltmedeKurusDuzenle)
                            {
                                decimal toplamAmortisman = ar.toplamAmortisman + ar.toplamCariAmortisman;
                                decimal toplamTutar = ar.toplamTutar;
                                decimal fark = toplamAmortisman - toplamTutar;

                                if (ar.SonYilMi && ar.SonDonemMi && Math.Abs(fark) < 10 && toplamTutar > 10)
                                {
                                    if (fark > 0)
                                        ar.maliyetCariAmortisman -= Math.Abs(fark);
                                    else if (fark < 1)
                                        ar.maliyetCariAmortisman += Math.Abs(fark);
                                }
                            }

                            s = tAmortisman.Kaydet(ak.kod, ak.yil, ak.donem, ar.prSicilNo, ar.amortismanTur, "", ar.maliyetCariAmortisman, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
                            //if (s.islemSonuc)
                            //{
                            //    //İlk amortismanda kaydedilecek. Ancak sicil özellik tablosundaki amortisman süresi boş olduğu için bir süre ilk kayıt kontrolü yapılmasın. Değer boş ise kaydedilsin.
                            //    s = TMMSicilNoOzellik.AmortismanSureKaydet(ve, kullanan, ar.prSicilNo, ar.amortismanSuresi);
                            //}
                            if (!s.islemSonuc)
                            {
                                s.hataStr = ar.prSicilNo + "," + ar.gorSicilNo + " sicil noya sahip kayıt yapılamadı. Lütfen tüm verileri silip tekrar deneyiniz!";
                                break;
                            }
                        }


                        decimal degerDuzeltmedeKurus = 0;
                        if (degerDuzeltmedeKurusDuzenle)
                        {
                            decimal toplamAmortisman = ar.toplamAmortisman + ar.toplamCariAmortisman;
                            decimal toplamTutar = ar.toplamTutar;
                            decimal fark = toplamAmortisman - toplamTutar;

                            if (ar.SonYilMi && ar.SonDonemMi && Math.Abs(fark) < 10 && toplamTutar > 10)
                            {
                                if (fark > 0)
                                    degerDuzeltmedeKurus -= Math.Abs(fark);
                                else if (fark < 1)
                                    degerDuzeltmedeKurus += Math.Abs(fark);
                            }
                        }


                        foreach (AmortismanRapor3DegerArtis ard in ar.degerDuzeltmeAmortismanX.objeler)
                        {
                            if (Math.Abs(ard.maliyetCariAmortisman) > 0)
                            {
                                if (degerDuzeltmedeKurus != 0 && Math.Abs(ard.maliyetCariAmortisman) > Math.Abs(degerDuzeltmedeKurus))
                                {
                                    ard.maliyetCariAmortisman += degerDuzeltmedeKurus;
                                    degerDuzeltmedeKurus = 0;
                                }

                                s = tAmortisman.Kaydet(ak.kod, ak.yil, ak.donem, ard.prSicilNo, ard.amortismanTur, ard.degerArtisKod, ard.maliyetCariAmortisman, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
                                if (!s.islemSonuc)
                                {
                                    s.hataStr = ard.prSicilNo + "," + ard.gorSicilNo + " sicil noya sahip kayıt yapılamadı. Lütfen tüm verileri silip tekrar deneyiniz!";
                                    break;
                                }
                            }

                        }


                    }
                }
                catch (Exception ex)
                {
                    s.DegerAta(ex.Message);
                }
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }


        private static Sonuc AmortismanKayitKontrol(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            if (OrtakFonksiyonlar.WebConfigOku("AmortismanKayitKontrolOlmasin", "0") == "1")
                return new Sonuc(true, "", "");

            if (string.IsNullOrWhiteSpace(ak.muhasebeKod) || string.IsNullOrWhiteSpace(ak.harcamaKod) || string.IsNullOrWhiteSpace(ak.ambarKod))
                return new Sonuc("Lütfen amortisman kayıtları üretilecek muhasebe,birim ve ambar bilgilerini seçiniz!");

            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                return new Sonuc("Lütfen lütfen hesap plan kodu girmeyiniz!");

            bool islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
            if (!islemSonuc)
                islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiKullaniciBirimi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);

            if (!islemSonuc)
                return new Sonuc(TasinirMalResI.TMM050);


            //Geçmiş ve gelecek dönem amortisman işlemleri her zaman yapılamasın. İlgili döneme en erken bir ay kala veya en geç bir ay sonra yapılabilsin.
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && OrtakFonksiyonlar.WebConfigOku("AmortismanDonemKontroluOlmasin", "0") != "1")
            {
                int amortismanYil = DateTime.Now.Year;
                int amortismanAy = DateTime.Now.Month;
                bool amortismanYapilabilir = true;

                if (amortismanAy == 1 && amortismanYil - 1 == ak.yil && ak.donem == 12)
                    amortismanYapilabilir = true;
                else if (amortismanYil != ak.yil || Math.Abs(amortismanAy - ak.donem) > 1)
                    amortismanYapilabilir = false;

                if (!amortismanYapilabilir)
                    return new Sonuc(string.Format("{0} yılı <span style=\"color:red;font-weight: bold;\">{1} </span>dönem amortismanı için işlem yapılan tarih uygun değil!<br><br><b>Not:</b> Amortisman ilgili döneme en erken bir ay kala veya en geç bir ay sonrasına kadar yapılabilir.", ak.yil, ak.donem));
            }


            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAmortismanIslemForm tabAmortismanIslemForm = new TABAmortismanIslemForm(ve);

            AmortismanIslemForm form = new AmortismanIslemForm()
            {
                yil = ak.yil,
                donem = ak.donem,
                muhasebeKod = ak.muhasebeKod,
                harcamaKod = ak.harcamaKod,
                ambarKod = ak.ambarKod
            };

            int durum = tabAmortismanIslemForm.DurumGetirTekKolon(form);
            if (durum == (int)ENUMAmortismanFormDurumu.ONAYLI)
                sonuc.hataStr += (form.yil + "/" + form.donem + " dönemine ait amortisman kayıtları onaylandığı için işlem yapılamaz!");
            else if (durum == 0)
                form.durum = (int)ENUMBelgeDurumu.YENI;
            else
                form.durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;

            if (Arac.MerkezBankasiKullaniyor())
            {
                //B-A Onay Kontrolü

                ENUMTasinirIslemFormOnayDurumu onayDurum = (ENUMTasinirIslemFormOnayDurumu)tabAmortismanIslemForm.OnayDurumGetirTekKolon(form);

                string belgeNo = "";
                if (form.fisNo != "")
                    belgeNo = form.fisNo + " no\'lu belge ";

                if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB)
                    sonuc.hataStr += string.Format("{0} B Onayınını beklediği için değişiklik yapılamaz.", belgeNo) + "<br>";
                else if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)
                    sonuc.hataStr += string.Format("{0} A Onayını beklediği için değişiklik yapılamaz.", belgeNo) + "<br>";




                //Geçerli dönem için onaysız fiş varsa uyarı versin
                DateTime t1 = new DateTime();
                DateTime t2 = new DateTime();
                if (MerkezBankasiGenel.Ayarlar.BanknotBirimiMi(ak.harcamaKod))
                {
                    t1 = new DateTime(ak.yil, ak.donem, 1);
                    t2 = t1.AddMonths(1);
                }
                else
                {
                    t1 = new DateTime(ak.yil, ak.donem - 2, 1);
                    t2 = t1.AddMonths(3);
                }

                bool onaysizFisVarMi = TMMTasinirIslemForm.OnaylanmamisBelgeVarMi(ve, new TasinirIslemForm()
                {
                    yil = ak.yil,
                    fisTarih = new TNSDateTime(t1),
                    islemTarih = new TNSDateTime(t2), //Tarih aralığı kontrolü için islemTarih değeri kullanılıyor.
                    muhasebeKod = ak.muhasebeKod,
                    harcamaKod = ak.harcamaKod,
                    ambarKod = ak.ambarKod
                });

                if (onaysizFisVarMi)
                    sonuc.hataStr += string.Format("{0} Birimi - {1} Ambarı - {2} yılı <font color='red'><b>{3}.</b></font> döneme ait onaylanmamış fişler var. <br>", ak.harcamaKod, ak.ambarKod, ak.yil, ak.donem) + "<br>";

                //------------------------------------------------------------------------------------------------------------------


                ////Amortisman yapılacak dönem kontrolü - HATALI gözden geçirilecek
                //AmortismanIslemForm aIslemForm = SonAmortismanBilgisi(ve, kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod, false);

                //if (aIslemForm != null)
                //{
                //    if (aIslemForm.durum != 5 && aIslemForm.donem != ak.donem)
                //        hata += string.Format("{0} yılı {1}. dönem amortismanı onaylanmamış! Yeni dönem için amortisman yapılamaz.", aIslemForm.yil, aIslemForm.donem);
                //    else
                //    {
                //        int hesapDonem = ak.yil * 100 + ak.donem - aIslemForm.yil * 100 - aIslemForm.donem;

                //        if (hesapDonem > 0)
                //        {
                //            int eksikDonem = 0;
                //            if (MerkezBankasi.Ayarlar.BanknotBirimiMi(ak.harcamaKod))
                //            {
                //                if (hesapDonem > 1)
                //                    eksikDonem = aIslemForm.donem + 1;
                //            }
                //            else
                //            {
                //                if (hesapDonem > 3)
                //                    eksikDonem = aIslemForm.donem + 3;
                //            }

                //            if (eksikDonem > 0)
                //                hata += string.Format("Lütfen <font color='red'><b>{0}.</b></font> dönem için amortisman yapınız.<br><br><b>Not:</b> {1} için en son <b>{2}</b> yılı <font color='red'><b>{3}.</b></font> dönem amortismanı yapılmış ve onaylanmıştır.", eksikDonem, aIslemForm.harcamaAd, aIslemForm.yil, aIslemForm.donem);

                //        }
                //        else if (hesapDonem < 0)
                //            hata += string.Format("Geçimiş dönemler için amortisman yapılamaz.", aIslemForm.yil, aIslemForm.donem);
                //    }
                //}

                //------------------------------------------------------------------------------------
            }

            sonuc.islemSonuc = (sonuc.hataStr == "");

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc RakamKaydet(VeriErisim veriErisim, Kullanici kullanan, string kod, int yil, int donem, int prSicilNo, ENUMAmortismanTur amortismanTur, decimal amortismanMiktar, string saymanlikKod, string birimKod, string ambarKod, string degerArtisKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortisman tAmortisman = new TABAmortisman(ve);
            Sonuc s;
            ve.BeginTransaction();
            s = tAmortisman.RakamSil(yil, donem, prSicilNo, amortismanTur, degerArtisKodu);
            if (s.islemSonuc)
                s = tAmortisman.Kaydet(kod, yil, donem, prSicilNo, (int)amortismanTur, degerArtisKodu, amortismanMiktar, saymanlikKod, birimKod, ambarKod);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaşların amortisman bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ak">Amortisman listeleme kriterleri</param>
        /// <param name="bitenlerDahil">Kalan amortismanı sıfırlanmış demirbaşlar dahil edilsin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaşların amortisman bilgileri listesi döndürülür.</returns>
        public static ObjectArray AmortismanRaporla(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak, bool bitenlerDahil)
        {
            ObjectArray listeVAN = new ObjectArray();
            if (ak.raporTur == 2)
                return AmortismanMIFRaporla(veriErisim, kullanan, ak, bitenlerDahil);
            else if (ak.raporTur == 3)
                return AmortismanUretimRaporu(veriErisim, kullanan, ak, bitenlerDahil);
            else
                return AmortismanSonucRaporu(veriErisim, kullanan, ak, bitenlerDahil);
        }

        private static ObjectArray AmortismanSonucRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak, bool bitenlerDahil)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                TABAmortisman tAmortisman = new TABAmortisman(ve);
                string[] sqller = new string[2];
                string[] baglar = new string[2];
                sqller[0] = tAmortisman.SicilNoAmortismanBilgiBulSql(ak.yil, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod, ak.hesapPlanKod, null);
                baglar[0] = ve.baglantiAd;
                sqller[1] = tAmortisman.SicilNoAmortismanBulSql(ak.yil, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod, ak.hesapPlanKod, false);
                baglar[1] = ve.baglantiAd;

                TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

                SortedList sl = servisUZY.SorguBirlestir(sqller, baglar, new int[] { 0 });
                foreach (DictionaryEntry de in sl)
                {
                    AmortismanRapor ar = new AmortismanRapor();
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                    ar.yil = ak.yil;

                    ar.prSicilNo = alAnahtar.IntAl(0);

                    ar.amortismanBaslamaYil = alVeri.IntAl(0);
                    ar.gorSicilNo = alVeri.StrAl(1);
                    ar.hesapPlanKod = alVeri.StrAl(2);
                    ar.maliyetTutar = alVeri.DecAl(3);
                    ar.girisTarih = alVeri.DTAl(4);
                    ar.islemTuruKod = alVeri.IntAl(5);
                    ar.hesapPlanAd = alVeri.StrAl(6);
                    ar.amortismanSuresi = alVeri.IntAl(7);

                    ar.saymanlikKod = alVeri.StrAl(8);
                    ar.harcamaBirimiKod = alVeri.StrAl(9);
                    ar.ambarKod = alVeri.StrAl(10);
                    ar.cariMaliyetAmortismanTutar = alVeri.DecAl(11);

                    ar.maliyetAmortismanBirikmisTutar = alVeri.DecAl(12);
                    bool ekle = false;
                    if (!string.IsNullOrWhiteSpace(ar.gorSicilNo))
                        ekle = true;
                    if (ekle)
                        listeVAN.Ekle(ar);
                }
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static ObjectArray AmortismanUretimRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak, bool bitenlerDahil)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                TABAmortisman tAmortisman = new TABAmortisman(ve);
                string[] sqller = new string[4];
                string[] baglar = new string[4];
                sqller[0] = tAmortisman.SicilNoHareketGirisBulSql(ak.yil, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod, ak.hesapPlanKod, null);
                baglar[0] = ve.baglantiAd;

                sqller[1] = tAmortisman.SicilNoHareketGirisCikisTarihBulSql(ak.yil, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod, ak.hesapPlanKod, true);
                baglar[1] = ve.baglantiAd;

                sqller[2] = tAmortisman.SicilNoHareketGirisCikisTarihBulSql(ak.yil, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod, ak.hesapPlanKod, false);
                baglar[2] = ve.baglantiAd;

                sqller[3] = tAmortisman.SicilNoAmortismanBulSql(ak.yil, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod, ak.hesapPlanKod, true);
                baglar[3] = ve.baglantiAd;
                TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

                SortedList sl = servisUZY.SorguBirlestir(sqller, baglar, new int[] { 0 });
                foreach (DictionaryEntry de in sl)
                {
                    AmortismanRapor ar = new AmortismanRapor();
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                    ar.yil = ak.yil;

                    ar.prSicilNo = alAnahtar.IntAl(0);

                    ar.amortismanBaslamaYil = alVeri.IntAl(0);
                    ar.gorSicilNo = alVeri.StrAl(1);
                    ar.hesapPlanKod = alVeri.StrAl(2);
                    ar.maliyetTutar = alVeri.DecAl(3);
                    ar.girisTarih = alVeri.DTAl(4);
                    ar.islemTuruKod = alVeri.IntAl(5);
                    ar.hesapPlanAd = alVeri.StrAl(6);
                    ar.amortismanSuresi = alVeri.IntAl(7);

                    ar.saymanlikKod = alVeri.StrAl(8);
                    ar.harcamaBirimiKod = alVeri.StrAl(9);
                    ar.ambarKod = alVeri.StrAl(10);

                    ar.sonGirisTarih = alVeri.DTAl(11);
                    ar.sonGirisNo = alVeri.IntAl(12);
                    ar.cikisTarih = alVeri.DTAl(13);
                    ar.cikisNo = alVeri.IntAl(14);

                    ar.maliyetAmortismanBirikmisTutar = alVeri.DecAl(15);

                    ar.cariMaliyetAmortismanTutar = 0;
                    bool ekle = false;
                    if (!string.IsNullOrWhiteSpace(ar.gorSicilNo))
                    {
                        ekle = true;
                    }
                    if (ekle)
                        listeVAN.Ekle(ar);
                }
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }


        public static ObjectArray AmortismanRaporla2(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            return AmortismanUretimRaporu2(veriErisim, kullanan, ak, false);
        }

        private static ObjectArray AmortismanUretimRaporu2(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak, bool amortismanKaydet)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.sureAsimi = 120;
            IDataReader dr = null;
            try
            {
                decimal enAzTutar = TMMAmortismanSinir.AmortismanSinirVer(ve, kullanan, ak.yil);

                ENUMAmortismanDonemTip donemTip = ENUMAmortismanDonemTip.YILLIK;
                int donemAyKriter = 12;
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    if (MerkezBankasiGenel.Ayarlar.BanknotBirimiMi(ak.harcamaKod)) //Banknot birimi ise aylık
                    {
                        donemTip = ENUMAmortismanDonemTip.AYLIK;
                        donemAyKriter = 1;
                    }
                    else //Diğer birimler 3'er aylık
                    {
                        donemTip = ENUMAmortismanDonemTip.UCAYLIK;
                        donemAyKriter = 3;
                    }
                }
                int donemAyIslemYapilan = donemAyKriter;



                TABAmortisman tAmortisman = new TABAmortisman(ve);

                dr = tAmortisman.SicilNoHareketGirisBul(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }
                string noktasizHB = ak.harcamaKod.Replace(".", "");
                while (dr.Read())
                {
                    //donemAyIslemYapilan değişebileceğinden her bir kayıt için donemAyKriter yeniden set ediliyor.
                    donemAyIslemYapilan = donemAyKriter;


                    AmortismanRapor ar = new AmortismanRapor();
                    ar.yil = ak.yil;
                    ar.donem = ak.donem;

                    ar.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                    ar.amortismanBaslamaYil = VeriErisim.DegerAl(dr, 1, 0);
                    ar.gorSicilNo = VeriErisim.DegerAl(dr, 2, "");
                    ar.hesapPlanKod = VeriErisim.DegerAl(dr, 3, "");
                    ar.maliyetTutar = VeriErisim.DegerAl(dr, 4, (decimal)0.0);
                    ar.girisTarih = VeriErisim.DegerAl(dr, 5);
                    ar.islemTipiKod = VeriErisim.DegerAl(dr, 6, 0);
                    ar.islemTuruKod = VeriErisim.DegerAl(dr, 7, 0);
                    ar.hesapPlanAd = VeriErisim.DegerAl(dr, 8, "");
                    ar.amortismanSuresi = VeriErisim.DegerAl(dr, 9, 0);
                    ar.kistAmortisman = VeriErisim.DegerAl(dr, 10, 0);
                    ar.saymanlikKod = VeriErisim.DegerAl(dr, 11, "");
                    ar.harcamaBirimiKod = VeriErisim.DegerAl(dr, 12, "");
                    ar.ambarKod = VeriErisim.DegerAl(dr, 13, "");
                    //ar.saymanlikAdi = VeriErisim.DegerAl(dr, 14, "");
                    ar.harcamaBirimiAdi = VeriErisim.DegerAl(dr, 15, "");

                    if (string.IsNullOrWhiteSpace(ar.gorSicilNo))
                        continue;


                    //if (!(ar.prSicilNo == 122789 || ar.prSicilNo == 122686 || ar.prSicilNo == 181289 || ar.prSicilNo == 121866 || ar.prSicilNo == 121878))
                    //    continue;

                    //if (!(ar.prSicilNo == 173525))
                    //    continue;


                    TNSCollection listeSicilNoHareket = ListeleDetaylar(VeriErisim.DegerAl(dr, 16, ""), 1);
                    TNSCollection listeDegerArtis = ListeleDetaylar(VeriErisim.DegerAl(dr, 17, ""), 2);
                    TNSCollection listeAmortisman = ListeleDetaylar(VeriErisim.DegerAl(dr, 18, ""), 3);
                    TNSCollection listeOzellik = ListeleDetaylar(VeriErisim.DegerAl(dr, 19, ""), 4);

                    DateTime ilkGiris = DateTime.MaxValue;

                    string sonMuh = "";
                    string sonBir = "";
                    string sonAmb = "";

                    foreach (SicilNoHareket s in listeSicilNoHareket)
                    {
                        if (s.fisTarih.Oku() < ilkGiris) ilkGiris = s.fisTarih.Oku();
                        //1,2,3,4,6,7,8,49,5
                        //50,52,53,54,55,56,57,58,59,51
                        if (s.yil <= ak.yil && s.durum != (int)ENUMSicilDurum.IPTAL &&
                            (s.islemTurKod == 1 || s.islemTurKod == 2 || s.islemTurKod == 3 ||
                            s.islemTurKod == 4 || s.islemTurKod == 6 || s.islemTurKod == 7 ||
                            s.islemTurKod == 8 || s.islemTurKod == 49 || s.islemTurKod == 5))
                        {
                            sonMuh = s.muhasebeKod;
                            sonBir = s.harcamaBirimKod;
                            sonAmb = s.ambarKod;
                            ar.sonGirisTarih = s.fisTarih;
                            ar.girisFisNo = s.fisNo;
                            ar.sonGirisNo = s.siraNo;
                        }

                        if (s.yil <= ak.yil && s.durum != (int)ENUMSicilDurum.IPTAL &&
                           s.islemTurKod >= 50 && s.islemTurKod <= 59 &&
                           s.islemTipKod != 24) //Geçici ihraç
                        {
                            ar.cikisTarih = s.fisTarih;
                            ar.cikisNo = s.siraNo;
                            ar.cikisFisNo = s.fisNo;
                            ar.islemDurumAciklama = s.nereyeGitti;
                        }
                        if (s.durum != (int)ENUMSicilDurum.IPTAL && (s.islemTurKod == 90 || s.islemTurKod == 91))
                        {
                            ar.nereyeGitti = s.nereyeGitti;
                            ar.nereyeGittiAd = s.odaAd;
                        }
                    }
                    ar.girisTarih = new TNSDateTime(ilkGiris);
                    ar.amortismanBaslamaYil = ilkGiris.Year;
                    int durum = 0;

                    if (ar.sonGirisNo == 0 || (ar.cikisNo > ar.sonGirisNo))
                    {
                        durum = 1; //girmemiş veya çıkmış
                    }
                    else if (string.IsNullOrWhiteSpace(sonMuh) || string.IsNullOrWhiteSpace(sonBir) || string.IsNullOrWhiteSpace(sonAmb))
                    {
                        durum = 9; // yerini bulamadık devam et.
                    }
                    else if (!string.IsNullOrEmpty(ak.muhasebeKod) && ak.muhasebeKod != sonMuh)
                    {
                        durum = 2; //Malzeme raporlanan muhasebede değil devam et.
                    }
                    else if (!sonBir.StartsWith(noktasizHB))
                    {
                        durum = 3; //Malzeme raporlanan birimde değil devam et.
                    }
                    else if (!string.IsNullOrEmpty(ak.ambarKod) && ak.ambarKod != sonAmb)
                    {
                        durum = 4; //Malzeme raporlanan ambarda değil devam et.
                    }
                    if (ar.amortismanBaslamaYil > ak.yil)
                    {
                        durum = 5; //raporladığım dönemden sonra girmiş.
                    }
                    else if ((amortismanKaydet || ak.raporTur == 7) && ar.islemTuruKod == (int)ENUMIslemTipi.DEVIRCIKISKURUM && ar.islemTipiKod != 24) //24:Geçici ihraç
                    {
                        durum = 6; //İhraç edilmiş
                    }
                    if (ar.islemTuruKod == 53)
                    {
                        durum = 7; //Satış
                    }

                    ar.amortismanDurum = durum;

                    if (ak.ihracDahil && !amortismanKaydet && ar.cikisTarih != null && ar.cikisTarih.Yil == ak.yil && ak.raporTur == 7 && (durum == 1 || durum == 6 || durum == 7))
                    {
                        //TTK amortisman raporunda ihraç edilmiş veya çıkışı yapılmışlarda gelsin. (Merkez Bankası)
                        durum = 0;
                    }

                    if (amortismanKaydet && durum != 0) continue;
                    else if (durum > 1) continue;

                    bool amortismanBitti = false;
                    foreach (SicilNoOzellik so in listeOzellik)
                    {
                        ar.bisNo = so.bisNo;
                        ar.eskiBisNo = so.eskiBisNo1;
                        ar.eskiSicilNo = so.disSicilNo;

                        if (string.IsNullOrEmpty(ar.nereyeGittiAd)) ar.nereyeGittiAd = so.bulunduguYer;

                        if (so.amortismanYil > 0)
                            ar.amortismanSuresi = so.amortismanYil;

                        amortismanBitti = so.amortismanBitti > 0;
                    }

                    if ((amortismanKaydet && amortismanBitti) || (ar.amortismanSuresi == 0 && ak.raporTur != 7))
                        continue;

                    decimal degerArtisAmortismanTutar = 0;
                    bool degerArtisAmortismanYapilacak = false;

                    foreach (SicilNoDegerArtis da in listeDegerArtis)
                    {
                        AmortismanRapor ard = new AmortismanRapor();
                        ard.yil = ar.yil;
                        ard.donem = ar.donem;

                        ard.prSicilNo = ar.prSicilNo;
                        ard.gorSicilNo = ar.gorSicilNo;
                        ard.hesapPlanKod = ar.hesapPlanKod;
                        ard.islemTuruKod = ar.islemTuruKod;
                        ard.hesapPlanAd = ar.hesapPlanAd;
                        ard.amortismanSuresi = ar.amortismanSuresi;

                        ard.saymanlikKod = ar.saymanlikKod;
                        ard.harcamaBirimiKod = ar.harcamaBirimiKod;
                        ard.ambarKod = ar.ambarKod;

                        ard.amortismanBaslamaYil = da.belgeTarihi.Yil;
                        ard.girisTarih = da.belgeTarihi;
                        ard.sonGirisTarih = da.belgeTarihi;
                        ard.sonGirisNo = OrtakFonksiyonlar.ConvertToInt(da.belgeNo, 0);
                        //ar2.cikisTarih = ? 
                        //ar2.cikisNo = ?
                        if (da.tur == 1 || da.tur == 3)
                            ard.amortismanTur = (int)ENUMAmortismanTur.FEVKALADE;
                        else if (da.tur == 2)
                            ard.amortismanTur = (int)ENUMAmortismanTur.DEGERLEME;
                        else
                            ard.amortismanTur = (int)ENUMAmortismanTur.DEGERLEME;

                        ard.degerArtisKod = da.kod;
                        ard.maliyetTutar = (decimal)da.tutar;
                        ard.cariMaliyetAmortismanTutar = 0;
                        if (ak.raporTur == 7)
                        {
                            //if (ard.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                            //{
                            //    ard.degerlemeAmortismanBirikmisTutar += Convert.ToDecimal(TTKAmortismanHesabi(da.tutar, da.belgeTarihi, ak.yil, ak.donem, ar.amortismanSuresi));
                            //    ard.maliyetAmortismanBirikmisTutar += 0;
                            //    ard.cariMaliyetAmortismanTutar += 0;
                            //    ard.cariDegerlemeAmortismanTutar += 0;
                            //}
                        }
                        else
                        {
                            foreach (AmortismanKriter s in listeAmortisman)
                            {
                                if (s.degerArtisKod == da.kod)
                                {
                                    if (((ak.yil > s.yil || (ak.yil == s.yil && ak.donem > s.donem))))
                                    {
                                        if (s.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                                            ard.degerlemeAmortismanBirikmisTutar += s.amortismanMiktar;
                                        else
                                            ard.maliyetAmortismanBirikmisTutar += s.amortismanMiktar;
                                    }

                                    if (s.yil == ak.yil && s.donem == ak.donem)
                                    {
                                        if (s.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                                            ard.cariDegerlemeAmortismanTutar += s.amortismanMiktar;
                                        else
                                            ard.cariMaliyetAmortismanTutar += s.amortismanMiktar;
                                    }
                                }
                            }

                            if (ak.yil >= ard.amortismanBaslamaYil && ak.yil < ard.amortismanBaslamaYil + ard.amortismanSuresi)
                                degerArtisAmortismanYapilacak = true;

                            if (amortismanKaydet && ard.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                            {
                                decimal degerArtisAmortiMiktar = 0;
                                if (degerArtisAmortismanYapilacak)
                                {
                                    degerArtisAmortiMiktar = ard.kalanTutar;
                                    if (ard.kalanTutar > enAzTutar || ard.amortismanBaslamaYil != ar.yil)
                                    {
                                        if (!(ard.SonYilMi && ard.donem == 12))
                                            degerArtisAmortiMiktar = (decimal)OrtakFonksiyonlar.KurusYap(((double)ard.maliyetTutar * donemAyIslemYapilan) / (ard.amortismanSuresi * 12));
                                    }
                                    ard.cariDegerlemeAmortismanTutar = degerArtisAmortiMiktar;
                                }
                                else
                                    degerArtisAmortiMiktar = ard.cariDegerlemeAmortismanTutar;
                                degerArtisAmortismanTutar += degerArtisAmortiMiktar;
                            }
                        }
                        ar.degerlemeAmortismanBirikmisTutar += ard.degerlemeAmortismanBirikmisTutar;
                        ar.cariDegerlemeAmortismanTutar += ard.cariDegerlemeAmortismanTutar;

                        ar.maliyetAmortismanBirikmisTutar += ard.maliyetAmortismanBirikmisTutar;
                        ar.cariMaliyetAmortismanTutar += ard.cariMaliyetAmortismanTutar;


                        AmortismanRaporDegerArtis ard2 = new AmortismanRaporDegerArtis();
                        ard2.prSicilNo = ard.prSicilNo;
                        ard2.gorSicilNo = ard.gorSicilNo;
                        ard2.degerArtisKod = ard.degerArtisKod;
                        ard2.amortismanTur = ard.amortismanTur;
                        ard2.maliyetTutar = ard.maliyetTutar;

                        ar.degerArtisAmortisman.Ekle(ard2);
                    }

                    ar.degerArtisAmortismanTutar = degerArtisAmortismanTutar;
                    ar.amortismanTur = (int)ENUMAmortismanTur.NORMAL;

                    bool oncekiDonemAmortismanYapilmis = false;

                    if (ak.raporTur == 7)
                    {
                        ar.maliyetAmortismanBirikmisTutar += Convert.ToDecimal(TTKAmortismanHesabi((double)(ar.maliyetTutar + ar.degerArtisTutar), ar.girisTarih, ak.yil - 1, 12, ar.amortismanSuresi));
                        //ar.cariMaliyetAmortismanTutar += 0;

                        int ttkCariDonem = ak.donem;

                        //TTK amortismanda ihraç edilen varlık için sonraki dönemlerde TTK cari amortisman sabit kalacak.
                        if (ar.cikisTarih != null && ar.cikisTarih.Yil == ak.yil && ar.islemTipiKod != 24 && (ar.islemTuruKod == 53 || ar.islemTuruKod == (int)ENUMIslemTipi.DEVIRCIKISKURUM))
                        {
                            if (ttkCariDonem > ar.cikisTarih.Ay)
                                ttkCariDonem = ar.cikisTarih.Ay;
                        }

                        ar.cariMaliyetAmortismanTutarTTK += Convert.ToDecimal(TTKAmortismanHesabiCariYil((double)(ar.maliyetTutar + ar.degerArtisTutar), ar.girisTarih, ak.yil, ttkCariDonem, ar.amortismanSuresi));


                        //TTK için amortisman sınırı dikkate alınmayacak. HÖ - 2025.04.15
                        ////Bu bölüm merkez bankası talebi üzerine eklendi. Teyit edielcek.
                        //if (ar.girisTarih.Yil == ak.yil && ar.maliyetTutar + ar.degerArtisTutar <= enAzTutar)
                        //{
                        //    ar.maliyetAmortismanBirikmisTutar = 0;
                        //    ar.cariMaliyetAmortismanTutarTTK = ar.maliyetTutar + ar.degerArtisTutar; ;
                        //}

                        ar.cariMaliyetAmortismanTutar = ar.cariMaliyetAmortismanTutarTTK;

                    }
                    else
                    {
                        foreach (AmortismanKriter s in listeAmortisman)
                        {

                            if (s.amortismanTur == (int)ENUMAmortismanTur.NORMAL) //if (string.IsNullOrEmpty(s.degerArtisKod))
                            {
                                if (((ak.yil > s.yil || (ak.yil == s.yil && ak.donem > s.donem))))
                                {
                                    ar.maliyetAmortismanBirikmisTutar += s.amortismanMiktar;
                                }

                                if (s.yil == ak.yil && s.donem == ak.donem)
                                {
                                    ar.cariMaliyetAmortismanTutar += s.amortismanMiktar;
                                }

                                //Amortisman yapılırken daha önceki dönemlere amortisman yapılmış mı? kontrol edilsin
                                if (!oncekiDonemAmortismanYapilmis && ak.raporTur == 3)
                                {
                                    if ((donemTip == ENUMAmortismanDonemTip.UCAYLIK && (ak.yil == s.yil && (ak.donem - 3) == s.donem) || (ak.yil - 1 == s.yil && ak.donem == 3 && s.donem == 12)) ||
                                        (donemTip == ENUMAmortismanDonemTip.AYLIK && (ak.yil == s.yil && (ak.donem - 1) == s.donem) || (ak.yil - 1 == s.yil && ak.donem == 1 && s.donem == 12)))
                                        oncekiDonemAmortismanYapilmis = true;
                                }
                            }
                        }
                    }

                    bool amortismanYapilacak = false;
                    if (ar.kalanTutarEnflasyon > 0 && ak.yil >= ar.amortismanBaslamaYil && ak.yil < ar.amortismanBaslamaYil + ar.amortismanSuresi && amortismanKaydet)
                    {
                        amortismanYapilacak = true;
                        //İlk amortisman ise dönem veya aylık amortisman işleminde geçmiş dönem veya ayları da dahil et (Not: Kıst Amortisman değilse)
                        if (!oncekiDonemAmortismanYapilmis && ar.amortismanBaslamaYil == ak.yil && ar.kistAmortisman == 0)
                        {
                            if (donemTip == ENUMAmortismanDonemTip.UCAYLIK)
                            {
                                if (ar.sonGirisTarih.Ay > 9)
                                    donemAyIslemYapilan = 12;
                                else if (ar.sonGirisTarih.Ay > 6)
                                    donemAyIslemYapilan = 9;
                                else if (ar.sonGirisTarih.Ay > 3)
                                    donemAyIslemYapilan = 6;
                            }
                            else if (donemTip == ENUMAmortismanDonemTip.AYLIK)
                                donemAyIslemYapilan = ar.sonGirisTarih.Ay;
                        }

                        ////Önceki dönemler için amortisman yapılmamış ise uyarı versin. (Not: ilk giriş hariç)
                        //if (donemTip != ENUMAmortismanDonemTip.YILLIK && donemAyKriter == donemAyIslemYapilan && !oncekiDonemAmortismanYapilmis && (ar.donem - donemAyIslemYapilan) > 0)
                        //{
                        //    listeVAN.sonuc.DegerAta(string.Format("<b>{0}</b> yılı <font color='red'><b>{1}.</b></font> dönem amortismanı için <b>{2}.</b> döneme ait amortisman kaydı bulunamadı.<br />Lütfen öncelikle <b>{2}.</b> dönem ve/veya daha önceki dönemler için amortisman yapınız.<br /><b>Örnek sicil No:</b> {3}", ar.yil, ar.donem, ar.donem - donemAyIslemYapilan, ar.prSicilNo));
                        //    break;
                        //}
                        //Kıst Amortisman yapılacaksa
                        if (ar.kistAmortisman > 0 && ar.amortismanBaslamaYil == ak.yil)
                        {
                            if (donemTip == ENUMAmortismanDonemTip.UCAYLIK)
                            {
                                int ay = 0;
                                if (ar.sonGirisTarih.Ay <= 3)
                                    ay = 3;
                                else if (ar.sonGirisTarih.Ay <= 6)
                                    ay = 6;
                                else if (ar.sonGirisTarih.Ay <= 9)
                                    ay = 9;
                                else if (ar.sonGirisTarih.Ay <= 12)
                                    ay = 12;

                                if (ay == ak.donem)
                                    donemAyIslemYapilan = (ay - ar.sonGirisTarih.Ay) + 1;
                            }
                            else if (donemTip == ENUMAmortismanDonemTip.YILLIK)
                                donemAyIslemYapilan = (12 - ar.sonGirisTarih.Ay) + 1;
                        }

                        decimal amortiMiktar = 0;
                        amortiMiktar = ar.kalanTutarEnflasyon;

                        //Merkez bankası ise ve ilk amortisman değilse amortisman sınırına bakmadan amortismanda devam et
                        if ((Arac.MerkezBankasiKullaniyor() && (ar.maliyetAmortismanBirikmisTutar > 0 || (ar.maliyetAmortismanBirikmisTutar == 0 && amortiMiktar > enAzTutar))) ||
                            (!Arac.MerkezBankasiKullaniyor() && amortiMiktar > enAzTutar))
                        {
                            if (!(ar.SonYilMi && ar.donem == 12))
                                amortiMiktar = (decimal)OrtakFonksiyonlar.KurusYap(((double)(ar.maliyetTutar + ar.enflasyonDegerArtisTutar) * donemAyIslemYapilan) / (ar.amortismanSuresi * 12));
                        }
                        ar.cariMaliyetAmortismanTutar = amortiMiktar;
                    }

                    if (ak.raporTur == 3 && (amortismanYapilacak || degerArtisAmortismanYapilacak)
                        || (ak.raporTur == 4 && ar.amortismanDurum == 0)
                        || ak.raporTur == 5
                        || ak.raporTur == 6 && ar.amortismanDurum == 0
                        || ak.raporTur == 7)
                    {
                        listeVAN.Ekle(ar);
                    }
                }

                if (listeVAN.sonuc.islemSonuc && listeVAN.objeler.Count == 0)
                {
                    string mesaj = "";

                    if (ak.raporTur == 3)
                        mesaj = "yapılacak";
                    else if (ak.raporTur == 5)
                        mesaj = "yapılmış veya yapılacak";
                    else
                        mesaj = "yapılmış";

                    listeVAN.sonuc.DegerAta(string.Format("{0} saymanlığı, {1} birimi, {2} ambarı, {3} hesap kodu için amortisman {4} kayıt bulunamadı!<br />", ak.muhasebeKod, ak.harcamaKod, ak.ambarKod, ak.hesapPlanKod, mesaj));
                }
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }

            if (dr != null)
                dr.Close();

            ve.VeriErisimKapat();

            return listeVAN;
        }

        public static ObjectArray AmortismanRaporla3(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            return AmortismanUretimRaporu3(veriErisim, kullanan, ak, false);
        }

        private static ObjectArray AmortismanUretimRaporu3(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak, bool amortismanKaydet)
        {
            ObjectArray listeVAN = new ObjectArray();

            if (ak.raporTur == 7 && (ak.donem <= 0 || ak.yil <= 0))
            {
                listeVAN.sonuc.DegerAta("Rapor için yıl ve dönem bilgisini belirtmelisiniz.");
                return listeVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.sureAsimi = 120;
            IDataReader dr = null;
            try
            {
                //if (ak.raporTur == 6) //Rapor
                //    ak.donem = 12;

                decimal enAzTutar = TMMAmortismanSinir.AmortismanSinirVer(ve, kullanan, ak.yil);

                ENUMAmortismanDonemTip donemTip = ENUMAmortismanDonemTip.YILLIK;
                int donemAyKriter = 12;
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    if (MerkezBankasiGenel.Ayarlar.BanknotBirimiMi(ak.harcamaKod)) //Banknot birimi ise aylık
                    {
                        donemTip = ENUMAmortismanDonemTip.AYLIK;
                        donemAyKriter = 1;
                    }
                    else //Diğer birimler 3'er aylık
                    {
                        donemTip = ENUMAmortismanDonemTip.UCAYLIK;
                        donemAyKriter = 3;

                        if (amortismanKaydet && !(ak.donem == 3 || ak.donem == 6 || ak.donem == 9 || ak.donem == 12))
                        {
                            ve.VeriErisimKapat();
                            listeVAN.sonuc.DegerAta("Dönem bilgisi hatalı.Dönem: " + ak.donem);
                            return listeVAN;
                        }

                    }

                }
                int donemAyIslemYapilanx = donemAyKriter;



                TABAmortisman tAmortisman = new TABAmortisman(ve);

                dr = tAmortisman.SicilNoHareketGirisBul(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }
                string noktasizHB = ak.harcamaKod.Replace(".", "");

                //ArrayList liste = new ArrayList();

                while (dr.Read())
                {
                    AmortismanRapor3 ar = new AmortismanRapor3();
                    ar.amortismanKaydet = amortismanKaydet;
                    ar.yil = ak.yil;
                    ar.donem = ak.donem;

                    //donemAyIslemYapilan değişebileceğinden her bir kayıt için donemAyKriter yeniden set ediliyor.
                    ar.donemAyIslemYapilan = donemAyKriter;

                    ar.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                    ar.amortismanBaslamaYil = VeriErisim.DegerAl(dr, 1, 0);
                    ar.gorSicilNo = VeriErisim.DegerAl(dr, 2, "");
                    ar.hesapPlanKod = VeriErisim.DegerAl(dr, 3, "");
                    ar.maliyetTutar = VeriErisim.DegerAl(dr, 4, (decimal)0.0);
                    ar.girisTarih = VeriErisim.DegerAl(dr, 5);
                    ar.islemTipiKod = VeriErisim.DegerAl(dr, 6, 0);
                    ar.islemTuruKod = VeriErisim.DegerAl(dr, 7, 0);
                    ar.hesapPlanAd = VeriErisim.DegerAl(dr, 8, "");
                    ar.amortismanSuresi = VeriErisim.DegerAl(dr, 9, 0);
                    ar.kistAmortisman = VeriErisim.DegerAl(dr, 10, 0);
                    ar.saymanlikKod = VeriErisim.DegerAl(dr, 11, "");
                    ar.harcamaBirimiKod = VeriErisim.DegerAl(dr, 12, "");
                    ar.ambarKod = VeriErisim.DegerAl(dr, 13, "");
                    //ar.saymanlikAdi = VeriErisim.DegerAl(dr, 14, "");
                    ar.harcamaBirimiAdi = VeriErisim.DegerAl(dr, 15, "");


                    //if (!(ar.prSicilNo == 199232 || ar.prSicilNo == 110988 || ar.prSicilNo == 112751))
                    //{
                    //    continue;
                    //}

                    if (string.IsNullOrWhiteSpace(ar.gorSicilNo))
                        continue;

                    TNSCollection listeSicilNoHareket = ListeleDetaylar(VeriErisim.DegerAl(dr, 16, ""), 1);
                    TNSCollection listeDegerArtis = ListeleDetaylar(VeriErisim.DegerAl(dr, 17, ""), 2);
                    TNSCollection listeAmortisman = ListeleDetaylar(VeriErisim.DegerAl(dr, 18, ""), 3);
                    TNSCollection listeOzellik = ListeleDetaylar(VeriErisim.DegerAl(dr, 19, ""), 4);


                    DateTime ilkGiris = DateTime.MaxValue;

                    string sonMuh = "";
                    string sonBir = "";
                    string sonAmb = "";

                    foreach (SicilNoHareket s in listeSicilNoHareket)
                    {
                        if (s.fisTarih.Oku() < ilkGiris)
                        {
                            ilkGiris = s.fisTarih.Oku();
                            if (s.kdvliBirimFiyat > 0)
                                ar.maliyetTutar = s.kdvliBirimFiyat; //Değer artış son harekete eklendiği için ilk bedel buradan alınsın. H.Ö
                        }
                        //1,2,3,4,6,7,8,49,5
                        //50,52,53,54,55,56,57,58,59,51
                        if (s.yil <= ak.yil && s.durum != (int)ENUMSicilDurum.IPTAL &&
                            (s.islemTurKod == 1 || s.islemTurKod == 2 || s.islemTurKod == 3 ||
                            s.islemTurKod == 4 || s.islemTurKod == 6 || s.islemTurKod == 7 ||
                            s.islemTurKod == 8 || s.islemTurKod == 49 || s.islemTurKod == 5))
                        {
                            sonMuh = s.muhasebeKod;
                            sonBir = s.harcamaBirimKod;
                            sonAmb = s.ambarKod;
                            ar.sonGirisTarih = s.fisTarih;
                            ar.girisFisNo = s.fisNo;
                            ar.sonGirisNo = s.siraNo;
                        }

                        if (s.yil <= ak.yil && s.durum != (int)ENUMSicilDurum.IPTAL &&
                           s.islemTurKod >= 50 && s.islemTurKod <= 59 &&
                           s.islemTipKod != 24) //Geçici İhraç
                        {
                            ar.cikisTarih = s.fisTarih;
                            ar.cikisNo = s.siraNo;
                            ar.cikisFisNo = s.fisNo;
                            ar.islemDurumAciklama = s.nereyeGitti;
                        }

                        // Burası kapatıldı. Yazılım ve Gayrimenkul işlemlerinde zimmet olmadığı için sicil ozellik tablosundan bulunduğu yer okunmalı.

                        if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                        {
                            if (s.durum != (int)ENUMSicilDurum.IPTAL && (s.islemTurKod == 90 || s.islemTurKod == 91))
                            {
                                ar.nereyeGitti = s.nereyeGitti;
                                ar.nereyeGittiAd = s.odaAd;
                            }
                        }

                    }
                    ar.girisTarih = new TNSDateTime(ilkGiris);
                    ar.amortismanBaslamaYil = ilkGiris.Year;
                    ar.amortismanBaslamaDonem = AmortismanDonemiBul(ak.harcamaKod, ar.girisTarih);
                    int durum = 0;

                    if (ar.sonGirisNo == 0 || (ar.cikisNo > ar.sonGirisNo))
                    {
                        durum = 1; //girmemiş veya çıkmış
                    }
                    else if (string.IsNullOrWhiteSpace(sonMuh) || string.IsNullOrWhiteSpace(sonBir) || string.IsNullOrWhiteSpace(sonAmb))
                    {
                        durum = 9; // yerini bulamadık devam et.
                    }
                    else if (!string.IsNullOrEmpty(ak.muhasebeKod) && ak.muhasebeKod != sonMuh)
                    {
                        durum = 2; //Malzeme raporlanan muhasebede değil devam et.
                    }
                    else if (!sonBir.StartsWith(noktasizHB))
                    {
                        durum = 3; //Malzeme raporlanan birimde değil devam et.
                    }
                    else if (!string.IsNullOrEmpty(ak.ambarKod) && ak.ambarKod != sonAmb && (ak.ambarKod.StartsWith("@") && ak.ambarKod.IndexOf(sonAmb) == -1))
                    {
                        durum = 4; //Malzeme raporlanan ambarda değil devam et.
                    }

                    if (ar.amortismanBaslamaYil > ak.yil || (ar.amortismanBaslamaYil == ak.yil && ak.donem > 0 && ar.amortismanBaslamaDonem > ak.donem))
                    {
                        durum = 5; //raporladığım dönemden sonra girmiş.
                    }
                    else if ((amortismanKaydet || ak.raporTur == 7) && ar.islemTuruKod == (int)ENUMIslemTipi.DEVIRCIKISKURUM && ar.islemTipiKod != 24) //Geçici İhraç
                    {
                        durum = 6; //İhraç edilmiş
                    }
                    if (ar.islemTuruKod == 53)
                    {
                        durum = 7; //Satış
                    }

                    ar.amortismanDurum = durum;

                    if (ak.ihracDahil && !amortismanKaydet && ar.cikisTarih != null && ar.cikisTarih.Yil == ak.yil && ak.raporTur == 7 && (durum == 1 || durum == 6 || durum == 7))
                    {
                        //TTK amortisman raporunda ihraç edilmiş veya çıkışı yapılmışlarda gelsin. (Merkez Bankası)
                        durum = 0;
                    }

                    if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    {
                        //Devir çıkış yapılmış ancak diğer birime girmemiş kayıtlarında amortisamnın yapılması için
                        if (durum > 1 && ak.prSicilNo == 0)
                            continue;
                    }
                    else
                    {
                        if (amortismanKaydet && durum != 0) continue;
                        else if (durum > 1) continue;
                    }



                    bool amortismanBitti = false;

                    ar.gayriMenkulMu = TNS.TMM.Arac.MerkezBankasiKullaniyor() && ar.ambarKod == "50";
                    ar.banknotGayriMenkulMu = ar.gayriMenkulMu && MerkezBankasiGenel.Ayarlar.BanknotBirimiMi(ak.harcamaKod);

                    foreach (SicilNoOzellik so in listeOzellik)
                    {
                        ar.bisNo = so.bisNo;
                        ar.eskiBisNo = so.eskiBisNo1;
                        ar.eskiSicilNo = so.disSicilNo;

                        if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                        {
                            ar.nereyeGitti = so.bulunduguYer;
                            ar.nereyeGittiAd = so.bulunduguYerAd;
                            ar.maliyetMerkezi = so.fotograf; //maliyet merkezi için bu alan kullanıldı
                        }

                        if (so.amortismanYil > 0)
                            ar.amortismanSuresi = so.amortismanYil;

                        //BANKNOT MATBAASI 1970 - 2003 ARASI GİRİŞLER İÇİN % 4
                        //BANKNOT MATBAASI 2004 SONRASI GİRİŞLER İÇİN % 2,5
                        //İDARE MERKEZİ VE ŞUBELER İÇİN % 2
                        if (ar.banknotGayriMenkulMu)
                        {
                            if (ar.girisTarih.Yil <= 2003)
                                ar.amortismanSuresi = 25;
                            else if (ar.girisTarih.Yil >= 2004)
                                ar.amortismanSuresi = 40;
                        }

                        amortismanBitti = so.amortismanBitti > 0;
                    }


                    bool sanatEseri = ar.hesapPlanKod.StartsWith("0880106") || ar.hesapPlanAd.StartsWith("SANAT ESERLERİ");
                    bool arsa = ar.hesapPlanKod.StartsWith("09700") || ar.hesapPlanAd.StartsWith("ARSALAR");

                    if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    {
                        //if (sanatEseri)
                        //    amortismanBitti = true;

                        if (sanatEseri)
                        {
                            if (amortismanKaydet)
                                continue;
                        }
                        else if (arsa)
                        {
                            if (amortismanKaydet)
                                continue;
                        }
                        else
                        {
                            if ((amortismanKaydet && amortismanBitti) || (ar.amortismanSuresi == 0 && ak.amortismanSure != 100))
                                continue;
                        }
                    }
                    else
                    {
                        if ((amortismanKaydet && amortismanBitti) || (ar.amortismanSuresi == 0 && ak.amortismanSure != 100))
                            continue;
                    }




                    //Maliyet + Değer Artış Toplamı + Enflasyon Düzeltmesi Toplamı
                    decimal maliyetDegerDuzeltmeToplami = ar.maliyetTutar;
                    foreach (SicilNoDegerArtis da in listeDegerArtis)
                    {
                        if (da.durum != 5)
                            continue;

                        //Değer artış/azalış ve 2023 öncesi edfler TTK ya dahil edilsin
                        if (ak.raporTur == 7 && da.tur == 1 && da.belgeTarihi.Yil >= 2023)
                            continue;

                        maliyetDegerDuzeltmeToplami += (decimal)da.tutar;
                    }
                    //********************************


                    //Maliyet + Enflasyon Farkı Amortisman ******************************
                    ar.amortismanTur = (int)ENUMAmortismanTur.NORMAL;
                    bool oncekiDonemAmortismanYapilmis = false;

                    if (ak.raporTur != 7)
                    {
                        foreach (AmortismanKriter s in listeAmortisman)
                        {

                            if (s.amortismanTur == (int)ENUMAmortismanTur.NORMAL)
                            {
                                if (s.yil == ak.yil && s.donem == ak.donem)
                                {
                                    ar.maliyetCariAmortisman += s.amortismanMiktar;
                                }

                                if ((ak.yil > s.yil) || (ak.yil == s.yil && (ak.donem > s.donem || ak.amortismanSure == 100)))
                                {
                                    ar.maliyetBirikmisAmortisman += s.amortismanMiktar;
                                }

                                //Amortisman yapılırken daha önceki dönemlere amortisman yapılmış mı? kontrol edilsin
                                if (!oncekiDonemAmortismanYapilmis && ak.raporTur == 3)
                                {
                                    if (s.yil < ak.yil || s.yil == ak.yil && s.donem < ak.donem)
                                        oncekiDonemAmortismanYapilmis = true;

                                    //if (s.yil < ak.yil && s.donem == 12) //Aktarımdan gelenler için önceki dönemler yapılmış olmalı.(Merkez Bankası)
                                    //    oncekiDonemAmortismanYapilmis = true;
                                    //else if (s.yil == ak.yil && s.donem < 6) //2018 yılı 3.dönem amortismanı eski programda yapıldı.(Merkez Bankası)
                                    //    oncekiDonemAmortismanYapilmis = true;
                                    //else if ((donemTip == ENUMAmortismanDonemTip.UCAYLIK && (ak.yil == s.yil && (ak.donem - 3) == s.donem) || (ak.yil - 1 == s.yil && ak.donem == 3 && s.donem == 12)) ||
                                    //    (donemTip == ENUMAmortismanDonemTip.AYLIK && (ak.yil == s.yil && (ak.donem - 1) == s.donem) || (ak.yil - 1 == s.yil && ak.donem == 1 && s.donem == 12)))
                                    //    oncekiDonemAmortismanYapilmis = true;
                                }
                            }

                        }
                    }
                    //********************************************************************






                    //Kontrol ilk amortisman
                    //İlk amortisman ise dönem veya aylık amortisman işleminde geçmiş dönem veya ayları da dahil et (Not: Kıst Amortisman değilse)
                    if (!oncekiDonemAmortismanYapilmis && ar.amortismanBaslamaYil == ak.yil && ar.kistAmortisman == 0)
                    {
                        if (donemTip == ENUMAmortismanDonemTip.UCAYLIK)
                        {
                            if (ar.sonGirisTarih.Ay > 9)
                                ar.donemAyIslemYapilan = 12;
                            else if (ar.sonGirisTarih.Ay > 6)
                                ar.donemAyIslemYapilan = 9;
                            else if (ar.sonGirisTarih.Ay > 3)
                                ar.donemAyIslemYapilan = 6;
                        }
                        else if (donemTip == ENUMAmortismanDonemTip.AYLIK)
                            ar.donemAyIslemYapilan = ar.sonGirisTarih.Ay;
                    }

                    ////Önceki dönemler için amortisman yapılmamış ise uyarı versin. (Not: ilk giriş hariç)
                    //if (donemTip != ENUMAmortismanDonemTip.YILLIK && donemAyKriter == donemAyIslemYapilan && !oncekiDonemAmortismanYapilmis && (ar.donem - donemAyIslemYapilan) > 0)
                    //{
                    //    listeVAN.sonuc.DegerAta(string.Format("<b>{0}</b> yılı <font color='red'><b>{1}.</b></font> dönem amortismanı için <b>{2}.</b> döneme ait amortisman kaydı bulunamadı.<br />Lütfen öncelikle <b>{2}.</b> dönem ve/veya daha önceki dönemler için amortisman yapınız.<br /><b>Örnek sicil No:</b> {3}", ar.yil, ar.donem, ar.donem - donemAyIslemYapilan, ar.prSicilNo));
                    //    break;
                    //}
                    //Kıst Amortisman yapılacaksa
                    if (ar.kistAmortisman > 0 && ar.amortismanBaslamaYil == ak.yil)
                    {
                        if (donemTip == ENUMAmortismanDonemTip.UCAYLIK)
                        {
                            int ay = 0;
                            if (ar.sonGirisTarih.Ay <= 3)
                                ay = 3;
                            else if (ar.sonGirisTarih.Ay <= 6)
                                ay = 6;
                            else if (ar.sonGirisTarih.Ay <= 9)
                                ay = 9;
                            else if (ar.sonGirisTarih.Ay <= 12)
                                ay = 12;

                            if (ay == ak.donem)
                                ar.donemAyIslemYapilan = (ay - ar.sonGirisTarih.Ay) + 1;
                        }
                        else if (donemTip == ENUMAmortismanDonemTip.YILLIK)
                            ar.donemAyIslemYapilan = (12 - ar.sonGirisTarih.Ay) + 1;
                    }
                    //**********************************************************




                    //Değer Düzeltme Amortisman ************************************

                    foreach (SicilNoDegerArtis da in listeDegerArtis)
                    {
                        if (da.durum != 5)
                            continue;

                        AmortismanRapor3 ard = new AmortismanRapor3();
                        ard.amortismanKaydet = ar.amortismanKaydet;
                        ard.yil = ar.yil;
                        ard.donem = ar.donem;
                        ard.donemAyIslemYapilan = ar.donemAyIslemYapilan;
                        ard.degerArtisAmortismanYapilacak = false;

                        ard.prSicilNo = ar.prSicilNo;
                        ard.gorSicilNo = ar.gorSicilNo;
                        ard.hesapPlanKod = ar.hesapPlanKod;
                        ard.islemTuruKod = ar.islemTuruKod;
                        ard.hesapPlanAd = ar.hesapPlanAd;
                        ard.amortismanSuresi = ar.amortismanSuresi;

                        ard.saymanlikKod = ar.saymanlikKod;
                        ard.harcamaBirimiKod = ar.harcamaBirimiKod;
                        ard.ambarKod = ar.ambarKod;

                        ard.girisTarih = da.belgeTarihi;
                        ard.sonGirisTarih = da.belgeTarihi;
                        ard.sonGirisNo = OrtakFonksiyonlar.ConvertToInt(da.belgeNo, 0);
                        //ar2.cikisTarih = ? 
                        //ar2.cikisNo = ?


                        ard.amortismanTur = (int)ENUMAmortismanTur.DEGERLEME;
                        ard.amortismanBaslamaYil = da.belgeTarihi.Yil;

                        if (da.tur == 1)
                        {
                            ard.amortismanTur = (int)ENUMAmortismanTur.FEVKALADE;
                            ard.amortismanBaslamaYil = ar.amortismanBaslamaYil; //Değer azalış ve enflasyon düzeltme işleminde amortisman süreri giriş amortisman ile aynı olmalı
                        }
                        else if (da.tur == 2)
                        {
                            ard.amortismanTur = (int)ENUMAmortismanTur.DEGERLEME;
                            ard.amortismanBaslamaYil = da.belgeTarihi.Yil;
                        }
                        else if (da.tur == 3)
                        {
                            ard.amortismanTur = (int)ENUMAmortismanTur.DEGERLEME;
                            ard.amortismanBaslamaYil = ar.amortismanBaslamaYil; //Değer azalış ve enflasyon düzeltme işleminde amortisman süreri giriş amortisman ile aynı olmalı
                        }


                        ard.degerArtisKod = da.kod;
                        ard.maliyetTutar = (decimal)da.tutar;
                        //ard.cariMaliyetAmortismanTutar = 0;


                        if (ak.raporTur == 7)
                        {
                            //if (ard.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                            //{
                            //    //ard.degerlemeAmortismanBirikmisTutar += Convert.ToDecimal(TTKAmortismanHesabi(da.tutar, da.belgeTarihi, ak.yil, ak.donem, ar.amortismanSuresi));
                            //    //ard.maliyetAmortismanBirikmisTutar += 0;
                            //    //ard.cariMaliyetAmortismanTutar += 0;
                            //    //ard.cariDegerlemeAmortismanTutar += 0;

                            //    ard.degerArtisBirikmisAmortismanTTK += Convert.ToDecimal(TTKAmortismanHesabi(da.tutar, da.belgeTarihi, ak.yil, ak.donem, ar.amortismanSuresi));
                            //    ard.maliyetBirikmisAmortisman += 0;
                            //    ard.maliyetCariAmortisman += 0;
                            //    ard.degerArtisCariAmortismanTTK += 0;
                            //}
                            //else if (ard.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                            //{
                            //    ard.enflasyonBirikmisAmortismanTTK += Convert.ToDecimal(TTKAmortismanHesabi(da.tutar, da.belgeTarihi, ak.yil, ak.donem, ar.amortismanSuresi));
                            //    ard.maliyetBirikmisAmortisman += 0;
                            //    ard.maliyetCariAmortisman += 0;
                            //    ard.enflasyonCariAmortismanTTK += 0;
                            //}


                            //Değer artış/azalış ve 2023 öncesi edfler TTK ya dahil edilsin
                            if (ard.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME || (ard.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE && da.belgeTarihi.Yil < 2023))
                            {
                                //ard.maliyetBirikmisAmortisman += Convert.ToDecimal(TTKAmortismanHesabi((double)(ard.maliyetTutar), ar.girisTarih, ak.yil, ak.donem, ar.amortismanSuresi));
                                //ard.maliyetCariAmortisman += 0;


                                ard.maliyetBirikmisAmortisman += (decimal)OrtakFonksiyonlar.KurusYap(TTKAmortismanHesabi((double)(ard.maliyetTutar), ar.girisTarih, ak.yil - 1, 12, ar.amortismanSuresi));


                                int ttkCariDonem = ak.donem;

                                //TTK amortismanda ihraç edilen varlık için sonraki dönemlerde TTK cari amortisman sabit kalacak.
                                if (ar.cikisTarih != null && ar.cikisTarih.Yil == ak.yil && ar.islemTipiKod != 24 && (ar.islemTuruKod == 53 || ar.islemTuruKod == (int)ENUMIslemTipi.DEVIRCIKISKURUM))
                                {
                                    if (ttkCariDonem > ar.cikisTarih.Ay)
                                        ttkCariDonem = ar.cikisTarih.Ay;
                                }

                                ard.maliyetCariAmortisman += (decimal)OrtakFonksiyonlar.KurusYap(TTKAmortismanHesabiCariYil((double)(ard.maliyetTutar), ar.girisTarih, ak.yil, ttkCariDonem, ar.amortismanSuresi));



                                //TTK için amortisman sınırı dikkate alınmayacak. HÖ - 2025.04.15
                                ////Bu bölüm merkez bankası talebi üzerine eklendi. Teyit edielcek.
                                //if (ar.girisTarih.Yil == ak.yil && ard.maliyetTutar + ar.degerArtisToplamTutar <= enAzTutar)
                                //{
                                //    ard.maliyetBirikmisAmortisman = 0;
                                //    ard.maliyetCariAmortisman = ard.maliyetTutar;
                                //}



                                AmortismanRapor3DegerArtis ard3 = new AmortismanRapor3DegerArtis();
                                ard3.prSicilNo = ard.prSicilNo;
                                ard3.gorSicilNo = ard.gorSicilNo;
                                ard3.degerArtisKod = ard.degerArtisKod;
                                ard3.amortismanTur = ard.amortismanTur;
                                ard3.maliyetTutar = ard.maliyetTutar;
                                ard3.maliyetCariAmortisman = ard.maliyetCariAmortisman;
                                ard3.maliyetBirikmisAmortisman = ard.maliyetBirikmisAmortisman;

                                ar.degerDuzeltmeAmortismanX.Ekle(ard3);
                            }

                        }
                        else
                        {
                            foreach (AmortismanKriter s in listeAmortisman)
                            {
                                if (s.amortismanTur == (int)ENUMAmortismanTur.NORMAL)
                                    continue;

                                if (s.degerArtisKod == da.kod)
                                {
                                    if (s.yil == ak.yil && s.donem == ak.donem)
                                    {
                                        //if (s.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                                        //    ard.degerArtisCariAmortisman += s.amortismanMiktar;
                                        //else if (s.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                                        //    ard.enflasyonCariAmortisman += s.amortismanMiktar;
                                        //else
                                        ard.maliyetCariAmortisman += s.amortismanMiktar;
                                    }

                                    if ((ak.yil > s.yil) || (ak.yil == s.yil && (ak.donem > s.donem || ak.amortismanSure == 100)))
                                    {
                                        //if (s.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                                        //    ard.degerArtisBirikmisAmortisman += s.amortismanMiktar;
                                        //else if (s.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                                        //    ard.enflasyonBirikmisAmortisman += s.amortismanMiktar;
                                        //else
                                        ard.maliyetBirikmisAmortisman += s.amortismanMiktar;
                                    }
                                    //break;
                                }
                                else if (s.degerArtisKod == "" && ard.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE && ard.sonGirisTarih.Yil == ar.amortismanBaslamaYil &&
                                    (ak.raporTur == 6 || ak.raporTur == 4 ||
                                    (ak.raporTur == 3 && (amortismanKaydet || ar.degerArtisAmortismanYapilacak)))
                                    ) //ard.donem == 12)
                                {
                                    //if (s.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                                    //    ard.degerArtisBirikmisAmortisman += s.amortismanMiktar;
                                    //else if (s.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                                    //    ard.enflasyonBirikmisAmortisman += s.amortismanMiktar;
                                    //else
                                    ard.maliyetBirikmisAmortisman += s.amortismanMiktar;
                                    //break;
                                }

                            }

                            //if (ard.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME && ak.yil >= ard.amortismanBaslamaYil && ((ak.yil < ard.amortismanBaslamaYil + ard.amortismanSuresi && !ar.gayriMenkulMu) || ar.gayriMenkulMu))
                            if (ak.yil >= ard.amortismanBaslamaYil && ((ak.yil < ard.amortismanBaslamaYil + ard.amortismanSuresi && !ar.gayriMenkulMu) || ar.gayriMenkulMu))
                                ard.degerArtisAmortismanYapilacak = true;



                            if (amortismanKaydet && ard.degerArtisAmortismanYapilacak)
                            {

                                //İlk amortisman ise dönem veya aylık amortisman işleminde geçmiş dönem veya ayları da dahil et (Not: Kıst Amortisman değilse)
                                if (ard.maliyetBirikmisAmortisman == 0 && ard.amortismanBaslamaYil == ak.yil && ar.kistAmortisman == 0)
                                {
                                    if (donemTip == ENUMAmortismanDonemTip.UCAYLIK)
                                    {
                                        if (ard.sonGirisTarih.Ay > 9)
                                            ard.donemAyIslemYapilan = 12;
                                        else if (ard.sonGirisTarih.Ay > 6)
                                            ard.donemAyIslemYapilan = 9;
                                        else if (ard.sonGirisTarih.Ay > 3)
                                            ard.donemAyIslemYapilan = 6;
                                    }
                                    else if (donemTip == ENUMAmortismanDonemTip.AYLIK)
                                        ard.donemAyIslemYapilan = ard.sonGirisTarih.Ay;
                                }



                                bool amortismanSiniriKontroluVar = enAzTutar > 0;
                                if (amortismanSiniriKontroluVar && ard.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                                {
                                    if (!Arac.MerkezBankasiKullaniyor() && ard.maliyetBirikmisAmortisman < enAzTutar)
                                        ard.amortismanSiniriAltinda = true;
                                    else if (ard.amortismanBaslamaYil == ard.yil)
                                    {
                                        if (ard.maliyetBirikmisAmortisman > 0)
                                            ard.amortismanSiniriAltinda = false;
                                        else if (oncekiDonemAmortismanYapilmis && ar.maliyetTutar > enAzTutar)
                                            ard.amortismanSiniriAltinda = false;
                                        else if (!oncekiDonemAmortismanYapilmis && maliyetDegerDuzeltmeToplami > enAzTutar)
                                            ard.amortismanSiniriAltinda = false;
                                        else
                                            ard.amortismanSiniriAltinda = true;
                                    }
                                }

                                decimal maliyetCariAmortisman = (decimal)OrtakFonksiyonlar.KurusYap(((double)ard.maliyetTutar * ard.donemAyIslemYapilan) / (ard.amortismanSuresi * 12));

                                if (ard.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME && maliyetCariAmortisman > 0)
                                {

                                    if (ard.amortismanSiniriAltinda)
                                        ard.maliyetCariAmortisman = ard.maliyetKalanAmortisman;
                                    else if (maliyetCariAmortisman >= ard.maliyetKalanAmortisman)
                                        ard.maliyetCariAmortisman = ard.maliyetKalanAmortisman;
                                    else if (ard.SonDonemMi && !ar.gayriMenkulMu)
                                        ard.maliyetCariAmortisman = ard.maliyetKalanAmortisman;
                                    else
                                        ard.maliyetCariAmortisman = maliyetCariAmortisman;
                                }
                                else if (ard.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE && maliyetCariAmortisman > 0)
                                {
                                    if (maliyetCariAmortisman >= ard.maliyetKalanAmortisman)
                                        ard.maliyetCariAmortisman = ard.maliyetKalanAmortisman;
                                    else if (ard.SonDonemMi && !ar.gayriMenkulMu)
                                        ard.maliyetCariAmortisman = ard.maliyetKalanAmortisman;
                                    else
                                        ard.maliyetCariAmortisman = maliyetCariAmortisman;
                                }
                                else
                                    ard.maliyetCariAmortisman = maliyetCariAmortisman;
                                //ard.maliyetCariAmortisman = (decimal)OrtakFonksiyonlar.KurusYap((double)(ard.maliyetTutar * ar.donemAyIslemYapilan * ar.amortismanYuzdesi) / (12 * 100));
                            }


                            AmortismanRapor3DegerArtis ard3 = new AmortismanRapor3DegerArtis();
                            ard3.prSicilNo = ard.prSicilNo;
                            ard3.gorSicilNo = ard.gorSicilNo;
                            ard3.degerArtisKod = ard.degerArtisKod;
                            ard3.amortismanTur = ard.amortismanTur;
                            ard3.maliyetTutar = ard.maliyetTutar;
                            ard3.maliyetCariAmortisman = ard.maliyetCariAmortisman;
                            ard3.maliyetBirikmisAmortisman = ard.maliyetBirikmisAmortisman;

                            ar.degerDuzeltmeAmortismanX.Ekle(ard3);
                        }


                        //if (ard.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                        //{
                        //    ar.degerArtisCariAmortisman += ard.degerArtisCariAmortisman;
                        //    ar.degerArtisBirikmisAmortisman += ard.degerArtisBirikmisAmortisman;
                        //}
                        //else if (ard.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                        //{
                        //    ar.enflasyonCariAmortisman += ard.enflasyonCariAmortisman;
                        //    ar.enflasyonBirikmisAmortisman += ard.enflasyonBirikmisAmortisman;
                        //}

                    }
                    //**********************************************************




                    if (ak.raporTur == 7)
                    {
                        //ar.maliyetAmortismanBirikmisTutar += Convert.ToDecimal(TTKAmortismanHesabi((double)(ar.maliyetTutar + ar.enflasyonDegerArtisTutar), ar.girisTarih, ak.yil, ak.donem, ar.amortismanSuresi));
                        //ar.cariMaliyetAmortismanTutar += 0;
                        //ar.maliyetBirikmisAmortisman += Convert.ToDecimal(TTKAmortismanHesabi((double)(ar.maliyetTutar), ar.girisTarih, ak.yil, ak.donem, ar.amortismanSuresi));
                        //ar.maliyetCariAmortisman += 0;


                        ar.maliyetBirikmisAmortisman += (decimal)OrtakFonksiyonlar.KurusYap(TTKAmortismanHesabi((double)(ar.maliyetTutar), ar.girisTarih, ak.yil - 1, 12, ar.amortismanSuresi));

                        int ttkCariDonem = ak.donem;

                        //TTK amortismanda ihraç edilen varlık için sonraki dönemlerde TTK cari amortisman sabit kalacak.
                        if (ar.cikisTarih != null && ar.cikisTarih.Yil == ak.yil && ar.islemTipiKod != 24 && (ar.islemTuruKod == 53 || ar.islemTuruKod == (int)ENUMIslemTipi.DEVIRCIKISKURUM))
                        {
                            if (ttkCariDonem > ar.cikisTarih.Ay)
                                ttkCariDonem = ar.cikisTarih.Ay;
                        }

                        ar.maliyetCariAmortisman += (decimal)OrtakFonksiyonlar.KurusYap(TTKAmortismanHesabiCariYil((double)(ar.maliyetTutar), ar.girisTarih, ak.yil, ttkCariDonem, ar.amortismanSuresi));



                        //TTK için amortisman sınırı dikkate alınmayacak. HÖ - 2025.04.15
                        ////Bu bölüm merkez bankası talebi üzerine eklendi. Teyit edielcek.
                        //if (ar.girisTarih.Yil == ak.yil && ar.maliyetTutar + ar.degerArtisToplamTutar <= enAzTutar)
                        //{
                        //    ar.maliyetBirikmisAmortisman = 0;
                        //    ar.maliyetCariAmortisman = ar.maliyetTutar;
                        //}

                    }



                    bool amortismanSiniriAltindax = false;
                    bool amortismanYapilacak = false;
                    if (amortismanKaydet && ar.toplamKalanAmortisman > 0 && ak.yil >= ar.amortismanBaslamaYil && ((ak.yil < ar.amortismanBaslamaYil + ar.amortismanSuresi && !ar.gayriMenkulMu) || ar.gayriMenkulMu))
                    {
                        amortismanYapilacak = true;

                        ////İlk amortisman ise dönem veya aylık amortisman işleminde geçmiş dönem veya ayları da dahil et (Not: Kıst Amortisman değilse)
                        //if (!oncekiDonemAmortismanYapilmis && ar.amortismanBaslamaYil == ak.yil && ar.kistAmortisman == 0)
                        //{
                        //    if (donemTip == ENUMAmortismanDonemTip.UCAYLIK)
                        //    {
                        //        if (ar.sonGirisTarih.Ay > 9)
                        //            ar.donemAyIslemYapilan = 12;
                        //        else if (ar.sonGirisTarih.Ay > 6)
                        //            ar.donemAyIslemYapilan = 9;
                        //        else if (ar.sonGirisTarih.Ay > 3)
                        //            ar.donemAyIslemYapilan = 6;
                        //    }
                        //    else if (donemTip == ENUMAmortismanDonemTip.AYLIK)
                        //        ar.donemAyIslemYapilan = ar.sonGirisTarih.Ay;
                        //}

                        //////Önceki dönemler için amortisman yapılmamış ise uyarı versin. (Not: ilk giriş hariç)
                        ////if (donemTip != ENUMAmortismanDonemTip.YILLIK && donemAyKriter == donemAyIslemYapilan && !oncekiDonemAmortismanYapilmis && (ar.donem - donemAyIslemYapilan) > 0)
                        ////{
                        ////    listeVAN.sonuc.DegerAta(string.Format("<b>{0}</b> yılı <font color='red'><b>{1}.</b></font> dönem amortismanı için <b>{2}.</b> döneme ait amortisman kaydı bulunamadı.<br />Lütfen öncelikle <b>{2}.</b> dönem ve/veya daha önceki dönemler için amortisman yapınız.<br /><b>Örnek sicil No:</b> {3}", ar.yil, ar.donem, ar.donem - donemAyIslemYapilan, ar.prSicilNo));
                        ////    break;
                        ////}
                        ////Kıst Amortisman yapılacaksa
                        //if (ar.kistAmortisman > 0 && ar.amortismanBaslamaYil == ak.yil)
                        //{
                        //    if (donemTip == ENUMAmortismanDonemTip.UCAYLIK)
                        //    {
                        //        int ay = 0;
                        //        if (ar.sonGirisTarih.Ay <= 3)
                        //            ay = 3;
                        //        else if (ar.sonGirisTarih.Ay <= 6)
                        //            ay = 6;
                        //        else if (ar.sonGirisTarih.Ay <= 9)
                        //            ay = 9;
                        //        else if (ar.sonGirisTarih.Ay <= 12)
                        //            ay = 12;

                        //        if (ay == ak.donem)
                        //            ar.donemAyIslemYapilan = (ay - ar.sonGirisTarih.Ay) + 1;
                        //    }
                        //    else if (donemTip == ENUMAmortismanDonemTip.YILLIK)
                        //        ar.donemAyIslemYapilan = (12 - ar.sonGirisTarih.Ay) + 1;
                        //}
                        ////**********************************************************

                        bool amortismanSiniriKontroluVar = enAzTutar > 0;
                        if (amortismanSiniriKontroluVar)
                        {
                            if (!Arac.MerkezBankasiKullaniyor() && ar.maliyetKalanAmortisman < enAzTutar)
                                ar.amortismanSiniriAltinda = true;
                            else if (ar.amortismanBaslamaYil == ar.yil)
                            {
                                if (ar.maliyetBirikmisAmortisman > 0)
                                    ar.amortismanSiniriAltinda = false;
                                else if (maliyetDegerDuzeltmeToplami > enAzTutar)
                                    ar.amortismanSiniriAltinda = false;
                                else
                                    ar.amortismanSiniriAltinda = true;
                            }
                        }


                        decimal maliyetCariAmortismanKuruslu = (ar.maliyetTutar * ar.donemAyIslemYapilan) / (ar.amortismanSuresi * 12);
                        decimal maliyetCariAmortisman = (decimal)OrtakFonksiyonlar.KurusYap((double)maliyetCariAmortismanKuruslu);

                        if (ar.amortismanSiniriAltinda)
                            ar.maliyetCariAmortisman = ar.maliyetKalanAmortisman;
                        else if (maliyetCariAmortisman >= ar.maliyetKalanAmortisman)
                            ar.maliyetCariAmortisman = ar.maliyetKalanAmortisman;
                        else if (ar.SonDonemMi && !ar.gayriMenkulMu)
                            ar.maliyetCariAmortisman = ar.maliyetKalanAmortisman;
                        else
                        {
                            ar.maliyetCariAmortismanKuruslu = maliyetCariAmortismanKuruslu;
                            ar.maliyetCariAmortisman = maliyetCariAmortisman;
                        }

                        //if (ar.SonDonemMi || ar.amortismanSiniriAltinda)
                        //    ar.maliyetCariAmortisman = ar.maliyetKalanAmortisman;
                        //else
                        //{
                        //    ar.maliyetCariAmortismanKuruslu = (ar.maliyetTutar * ar.donemAyIslemYapilan) / (ar.amortismanSuresi * 12);
                        //    //ar.maliyetCariAmortismanKuruslu = (ar.maliyetTutar * ar.donemAyIslemYapilan * ar.amortismanYuzdesi) / (12 * 100);
                        //    ar.maliyetCariAmortisman = (decimal)OrtakFonksiyonlar.KurusYap((double)ar.maliyetCariAmortismanKuruslu);
                        //}

                        //Merkez bankası ise ve ilk amortisman değilse amortisman sınırına bakmadan amortismanda devam et
                        //if ((Arac.MerkezBankasiKullaniyor() && (ar.maliyetBirikmisAmortisman > 0 || (ar.maliyetAmortismanBirikmisTutar == 0 && amortiMiktar > enAzTutar))) ||
                        //    (!Arac.MerkezBankasiKullaniyor() && amortiMiktar > enAzTutar))
                        //{
                        //    if (!(ar.SonYilMi && ar.donem == 12))
                        //        amortiMiktar = (decimal)OrtakFonksiyonlar.KurusYap(((double)(ar.maliyetTutar + ar.enflasyonDegerArtisTutar) * donemAyIslemYapilan) / (ar.amortismanSuresi * 12));
                        //}
                        //ar.cariMaliyetAmortismanTutar = amortiMiktar;
                    }
                    //**********************************************************




                    //foreach (AmortismanRapor3 a in ar.degerDuzeltmeAmortisman.objeler)
                    //{
                    //    if (a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                    //        ar.maliyetBirikmisAmortisman += a.maliyetToplamAmortisman;
                    //}



                    /*
                     * 19.09.2024 tarihinde Merkez bankasından gelen talep üzerine EDF tutarları ayrı amortisman yapılarak kayıt alında alınacak. O nedenle burası kapatıldı. H.Ö.

                    //Enflasyon düzelmeden gelen değeri maliyet amortismanı gibi yap
                    foreach (AmortismanRapor3 a in ar.degerDuzeltmeAmortisman.objeler)
                    {
                        if (ar.amortismanKaydet && ar.toplamKalanAmortisman > 0 && a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                        {
                            decimal maliyetCariAmortismanKuruslu = ((ar.maliyetTutar + ar.enflasyonToplamTutar) * ar.donemAyIslemYapilan) / (ar.amortismanSuresi * 12);
                            decimal maliyetCariAmortisman = (decimal)OrtakFonksiyonlar.KurusYap((double)maliyetCariAmortismanKuruslu);

                            if (ar.amortismanSiniriAltinda)
                                ar.maliyetCariAmortisman = ar.toplamKalanAmortisman;
                            else if (maliyetCariAmortisman >= ar.toplamKalanAmortisman)
                                ar.maliyetCariAmortisman = ar.maliyetKalanAmortisman;
                            else if (ar.SonDonemMi && !ar.gayriMenkulMu)
                                ar.maliyetCariAmortisman = ar.maliyetKalanAmortisman;
                            else
                            {
                                ar.maliyetCariAmortismanKuruslu = maliyetCariAmortismanKuruslu;
                                ar.maliyetCariAmortisman = maliyetCariAmortisman;
                            }

                            //if (ar.SonDonemMi || ar.amortismanSiniriAltinda)
                            //    ar.maliyetCariAmortisman = ar.toplamKalanAmortisman;
                            //else
                            //{
                            //    ar.maliyetCariAmortismanKuruslu = ((ar.maliyetTutar + ar.enflasyonToplamTutar) * ar.donemAyIslemYapilan) / (ar.amortismanSuresi * 12);
                            //    //ar.maliyetCariAmortismanKuruslu = ((ar.maliyetTutar + ar.enflasyonToplamTutar) * ar.amortismanYuzdesi * ar.donemAyIslemYapilan) / (12 * 100);
                            //    ar.maliyetCariAmortisman = (decimal)OrtakFonksiyonlar.KurusYap((double)ar.maliyetCariAmortismanKuruslu);
                            //}
                            break;
                        }
                    }
                    */



                    //foreach (AmortismanRapor3 a in ar.degerDuzeltmeAmortisman.objeler)
                    //{
                    //    if (a.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME && a.degerArtisAmortismanYapilacak)
                    //        a.degerArtisAmortismanYapilacak = true;
                    //}


                    //Son dönem ayrılan amortisman
                    int sonDonem = 0;
                    foreach (AmortismanKriter s in listeAmortisman)
                        if (sonDonem <= s.yil * 100 + s.donem)
                            sonDonem = s.yil * 100 + s.donem;
                    foreach (AmortismanKriter s in listeAmortisman)
                        if (sonDonem == s.yil * 100 + s.donem)
                        {
                            ar.sonDonemAmortisman += s.amortismanMiktar;
                            ar.sonAmortismanYil = s.yil;
                            ar.sonAmortismanDonem = s.donem;
                        }
                    //**********************************************************

                    if (ak.raporTur == 3 && (amortismanYapilacak || ar.degerArtisAmortismanYapilacak)
                        || (ak.raporTur == 4 && ar.amortismanDurum == 0)
                        || ak.raporTur == 5
                        || ak.raporTur == 6 && ar.amortismanDurum <= 1
                        || ak.raporTur == 7
                        || ak.prSicilNo > 0)
                    {
                        listeVAN.Ekle(ar);
                    }
                }

                if (listeVAN.sonuc.islemSonuc && listeVAN.objeler.Count == 0)
                {
                    string mesaj = "";

                    if (ak.raporTur == 3)
                        mesaj = "yapılacak";
                    else if (ak.raporTur == 5)
                        mesaj = "yapılmış veya yapılacak";
                    else
                        mesaj = "yapılmış";

                    listeVAN.sonuc.DegerAta(string.Format("{0} saymanlığı, {1} birimi, {2} ambarı, {3} hesap kodu için amortisman {4} kayıt bulunamadı!<br />", ak.muhasebeKod, ak.harcamaKod, ak.ambarKod, ak.hesapPlanKod, mesaj));
                }

            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }

            if (dr != null)
                dr.Close();
            ve.VeriErisimKapat();

            return listeVAN;
        }


        private static double TTKAmortismanHesabi(double tutar, TNSDateTime belgeTarihi, int yil, int donem, int amortismanSuresi)
        {
            DateTime sonGun = belgeTarihi.Oku().AddYears(amortismanSuresi);
            DateTime araGun = new DateTime(yil, donem, 1).AddMonths(1);
            int ts1 = (sonGun - belgeTarihi.Oku()).Days;
            int ts2 = (araGun - belgeTarihi.Oku()).Days;
            if (ts1 > 0 && ts2 > 0)
            {
                if (ts1 < ts2) return tutar;
                return tutar * ts2 / ts1;
            }
            else
                return 0;
        }

        private static double TTKAmortismanHesabiCariYil(double tutar, TNSDateTime belgeTarihi, int yil, int donem, int amortismanSuresi)
        {
            DateTime sonGun = belgeTarihi.Oku().AddYears(amortismanSuresi);
            DateTime araGun = new DateTime(yil, donem, 1).AddMonths(1);
            DateTime cariYilBas = new DateTime(yil, 1, 1);

            int ts1 = (sonGun - belgeTarihi.Oku()).Days;
            int ts2 = (araGun - belgeTarihi.Oku()).Days;
            int ts3 = (araGun - cariYilBas).Days;
            if (ts1 > 0 && ts2 > 0)
            {
                if (ts1 < ts2)
                {
                    int ts4 = (sonGun - cariYilBas).Days; //Cari yıl kalan amortisman süresi bul
                    if (ts4 > 0)
                        return tutar * ts4 / ts1;
                    else
                        return 0;
                }
                else if (ts2 < ts3) return tutar * ts2 / ts1;
                return tutar * ts3 / ts1;
            }
            else
                return 0;
        }

        private static TNSCollection ListeleDetaylar(string okunanXML, int detayTip)
        {
            TNSCollection pVAN = new TNSCollection();
            if (string.IsNullOrWhiteSpace(okunanXML)) return pVAN;

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                while (r.Read())
                {
                    if (detayTip == 1)
                        pVAN.Add(DetayOkuSicilNoHareket(r));
                    else if (detayTip == 2)
                        pVAN.Add(DetayOkuDegerArtis(r));
                    else if (detayTip == 3)
                        pVAN.Add(DetayOkuAmortisman(r));
                    else if (detayTip == 4)
                        pVAN.Add(DetayOkuOzellik(r));
                }
                r.Close();
            }
            return pVAN;
        }

        private static SicilNoHareket DetayOkuSicilNoHareket(IDataReader dr)
        {
            SicilNoHareket detay = new SicilNoHareket();
            detay.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
            detay.siraNo = VeriErisim.DegerAl(dr, 1, 0);
            detay.yil = VeriErisim.DegerAl(dr, 2, 0);
            detay.muhasebeKod = VeriErisim.DegerAl(dr, 3, "");
            detay.harcamaBirimKod = VeriErisim.DegerAl(dr, 4, "");
            detay.ambarKod = VeriErisim.DegerAl(dr, 5, "");
            detay.hesapPlanKod = VeriErisim.DegerAl(dr, 6, "");
            detay.sicilNo = VeriErisim.DegerAl(dr, 7, "");
            detay.sicilSiraNo = VeriErisim.DegerAl(dr, 8, "");
            detay.fisSiraNo = VeriErisim.DegerAl(dr, 9, 0);
            detay.fisTarih = VeriErisim.DegerAl(dr, 10);
            detay.islemTipKod = VeriErisim.DegerAl(dr, 11, 0);
            detay.islemTurKod = VeriErisim.DegerAl(dr, 12, 0);
            detay.islemTarih = VeriErisim.DegerAl(dr, 13);
            detay.islemYapan = VeriErisim.DegerAl(dr, 14, "");
            detay.durum = VeriErisim.DegerAl(dr, 15, 0);
            detay.nereyeGitti = VeriErisim.DegerAl(dr, 16, "");
            detay.kimeGitti = VeriErisim.DegerAl(dr, 17, "");
            //detay.odaAd = VeriErisim.DegerAl(dr, 18, "");
            detay.kdvliBirimFiyat = VeriErisim.DegerAl(dr, 18, (decimal)0.0);
            detay.fisNo = VeriErisim.DegerAl(dr, 19, "");

            return detay;
        }

        private static SicilNoDegerArtis DetayOkuDegerArtis(IDataReader dr)
        {
            SicilNoDegerArtis detay = new SicilNoDegerArtis();
            detay.kod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.KOD, "");
            detay.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.MUHASEBEKOD, "");
            detay.harcamaKod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.HARCAMABIRIMKOD, "");
            detay.belgeNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.BELGENO, "");
            detay.belgeTarihi = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.BELGETARIHI);
            detay.prSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.PRSICILNO, 0);
            detay.gorSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.GORSICILNO, "");
            detay.tutar = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.TUTAR, 0.0);
            detay.gerekce = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.GEREKCE, "");
            detay.durum = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.DURUM, 0);
            detay.tur = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.TUR, 0);

            return detay;
        }

        private static AmortismanKriter DetayOkuAmortisman(IDataReader dr)
        {
            AmortismanKriter detay = new AmortismanKriter();
            detay.kod = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.KOD, "");
            detay.yil = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.YIL, 0);
            detay.donem = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.DONEM, 0);
            detay.prSicilNo = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.PRSICILNO, 0);
            detay.amortismanTur = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.AMORTISMANTUR, 0);
            detay.degerArtisKod = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.DEGERARTISKODU, "");
            detay.amortismanMiktar = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.AMORTISMANMIKTAR, (decimal)0.0);
            detay.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.MUHASEBEKOD, "");
            detay.harcamaKod = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.HARCAMABIRIMKOD, "");
            detay.ambarKod = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.AMBARKOD, "");

            return detay;
        }

        private static SicilNoOzellik DetayOkuOzellik(IDataReader dr)
        {
            int sayac = 0;

            SicilNoOzellik detay = new SicilNoOzellik();
            detay.prSicilno = VeriErisim.DegerAl(dr, sayac++, 0);
            detay.bisNo = VeriErisim.DegerAl(dr, sayac++, "");
            detay.eskiBisNo1 = VeriErisim.DegerAl(dr, sayac++, "");
            detay.disSicilNo = VeriErisim.DegerAl(dr, sayac++, "");
            detay.amortismanYil = VeriErisim.DegerAl(dr, sayac++, 0);
            detay.amortismanBitti = VeriErisim.DegerAl(dr, sayac++, 0);
            detay.bulunduguYer = VeriErisim.DegerAl(dr, sayac++, "");
            detay.bulunduguYerAd = VeriErisim.DegerAl(dr, sayac++, "");
            detay.fotograf = VeriErisim.DegerAl(dr, sayac++, ""); //Merkez bankası - Yerleşke Maliyet Merkezi bilgisini okumak için

            return detay;
        }

        public enum ENUMAmortismanDonemTip
        {
            YILLIK = 0,
            UCAYLIK = 1,
            AYLIK = 2
        }

        private static ObjectArray AmortismanMIFRaporla(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak, bool bitenlerDahil)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortisman tAmortisman = new TABAmortisman(ve);
            string[] sqller = new string[1];
            string[] baglar = new string[1];
            sqller[0] = tAmortisman.MIFBilgiBulSql(ak.yil, ak.muhasebeKod, ak.harcamaKod, ak.kumulatif);
            baglar[0] = ve.baglantiAd;

            TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            ArrayList alacakHesaplar = new ArrayList();
            SortedList sl = servisUZY.SorguBirlestir(sqller, baglar, new int[] { 0, 1 });
            foreach (DictionaryEntry de in sl)
            {
                AmortismanRapor ar = new AmortismanRapor();
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                ar.harcamaBirimiKod = alAnahtar.StrAl(0);
                ar.hesapPlanKod = alAnahtar.StrAl(1);
                ar.hesapPlanAd = alVeri.StrAl(0);
                ar.cariMaliyetAmortismanTutar = Math.Round(alVeri.DecAl(1), 2);
                ar.maliyetTutar = Math.Round(alVeri.DecAl(2), 2);

                EkleAlacakHesaba(ar, alacakHesaplar);
                listeVAN.Ekle(ar);
            }
            ve.VeriErisimKapat();
            foreach (AmortismanRapor ar in alacakHesaplar)
            {
                listeVAN.Ekle(ar);
            }
            return listeVAN;
        }

        private static void EkleAlacakHesaba(AmortismanRapor arGelen, ArrayList alacakHesaplar)
        {
            string alacakHesap = "630.13.01." + arGelen.hesapPlanKod.Substring(3, 2);
            foreach (AmortismanRapor item in alacakHesaplar)
            {
                if (item.hesapPlanKod == alacakHesap && item.harcamaBirimiKod == arGelen.harcamaBirimiKod)
                {
                    item.maliyetAmortismanBirikmisTutar += arGelen.cariMaliyetAmortismanTutar;
                    return;
                }
            }
            AmortismanRapor ar = new AmortismanRapor();
            ar.harcamaBirimiKod = arGelen.harcamaBirimiKod;
            ar.hesapPlanKod = alacakHesap;
            ar.hesapPlanAd = "";
            ar.maliyetAmortismanBirikmisTutar = arGelen.cariMaliyetAmortismanTutar;
            alacakHesaplar.Add(ar);
        }

        /// <summary>
        /// Parametre olarak verilen amortisman kriter bilgilerinin diğer
        /// parametrelerle birlikte doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ak">Amortisman listeleme kriterleri</param>
        /// <param name="islemTur">Yapılan amortisman işleminin türü</param>
        /// <param name="amortismanBaslamaYil">Amortisman işlemlerine başlama yılı</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc AmortismanKriterKontrol(Kullanici kullanan, AmortismanKriter ak, ENUMIslemTur islemTur, int amortismanBaslamaYil)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (ak.yil <= 0)
                hata = TasinirMalResI.TMM036 + "<br>";

            if (string.IsNullOrEmpty(ak.muhasebeKod))
                hata += TasinirMalResI.TMM013 + "<br>";
            if (string.IsNullOrEmpty(ak.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";

            if (string.IsNullOrEmpty(ak.ambarKod))
                hata += TasinirMalResI.TMM031 + "<br>";

            if (string.IsNullOrEmpty(ak.hesapPlanKod))
                hata += TasinirMalResI.TMM027 + "<br>";

            if (islemTur == ENUMIslemTur.KAYDET)
            {
                if (ak.yil == amortismanBaslamaYil || amortismanBaslamaYil <= 0)
                    hata += TasinirMalResI.TMM037 + "<br>";

                if (ak.islemTur == (int)ENUMAmortismanIslemTur.AMORTISMAN)
                {
                    if (ak.amortismanTur <= 0)
                        hata += TasinirMalResI.TMM038 + "<br>";
                }
                else if (ak.islemTur == (int)ENUMAmortismanIslemTur.DEGERLEME)
                {
                    if (ak.degerlemeOran <= 0)
                        hata += TasinirMalResI.TMM039 + "<br>";
                }
                else
                    hata += TasinirMalResI.TMM040 + "<br>";
            }

            if (islemTur == ENUMIslemTur.KAYDET || islemTur == ENUMIslemTur.SIL)
            {
                if (ak.uygulamaTur == (int)ENUMAmortismanUygulamaTur.KRITERLEREGORE && ak.prSicilNolar.Count > 0)
                    hata += TasinirMalResI.TMM041 + "<br>";
                else if (ak.uygulamaTur == (int)ENUMAmortismanUygulamaTur.SECILENLEREGORE && ak.prSicilNolar.Count <= 0)
                    hata += TasinirMalResI.TMM042 + "<br>";
                else if (ak.uygulamaTur <= 0)
                    hata += TasinirMalResI.TMM043 + "<br>";
            }

            if (islemTur == ENUMIslemTur.HESAPLA)
            {
                if (amortismanBaslamaYil > 0 && ak.yil != amortismanBaslamaYil)
                    hata += TasinirMalResI.TMM044 + "<br>";

                if (ak.birikmisAmortismanOran <= 0)
                    hata += TasinirMalResI.TMM045 + "<br>";
                if (ak.birikmisAmortismanOran > 100)
                    hata += TasinirMalResI.TMM046 + "<br>";
                if (ak.kalanAmortismanSure < 0)
                    hata += TasinirMalResI.TMM047 + "<br>";
                if (ak.birikmisAmortismanOran != 100 && ak.kalanAmortismanSure == 0)
                    hata += TasinirMalResI.TMM048 + "<br>";
                if (ak.birikmisAmortismanOran == 100 && ak.kalanAmortismanSure != 0)
                    hata += TasinirMalResI.TMM049 + "<br>";
            }

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan amortisman sayılarını ve toplam tutarlarını demirbaş bazında döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="ak">Amortisman listeleme kriterleri</param>
        /// <param name="degerleme">Değerleme işleminden kaynaklı amortisman mı isteniyor bilgisi</param>
        /// <param name="cariDahil">Cari amortisman bilgileri dahil mi bilgisi</param>
        /// <returns>Amortisman sayıları ve toplam tutarları döndürülür.</returns>
        private static TNSCollection SicilAmortismanSayiVeMiktarListele(VeriErisim veriErisim, AmortismanKriter ak, bool degerleme, bool cariDahil)
        {
            TNSCollection liste = new TNSCollection();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABAmortisman tAmortisman = new TABAmortisman(ve);

            IDataReader dr = tAmortisman.SicilAmortismanSayiVeMiktarListele(ak, degerleme, cariDahil);
            while (dr.Read())
            {
                Amortisman gelen = new Amortisman();
                gelen.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                gelen.yapilanAmortismanSayisi = VeriErisim.DegerAl(dr, 1, 0);
                gelen.yapilanToplamAmortisman = VeriErisim.DegerAl(dr, 2, (decimal)0);

                liste.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return liste;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan amortisman bilgileri dahil edilmiş dayanıklı taşınır defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Amortismanlı dayanıklı taşınır defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında amortismanlı dayanıklı taşınır defteri listesi döndürülür.</returns>
        public static ObjectArray DayanikliTasinirlarDefteriOlustur(VeriErisim veriErisim, Kullanici kullanan, DefterKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kriter.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kriter.muhasebeKod))
                hata += TasinirMalResI.TMM013 + "<br>";
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                if (string.IsNullOrEmpty(kriter.harcamaKod))
                    hata += TasinirMalResI.TMM014 + "<br>";
                if (string.IsNullOrEmpty(kriter.ambarKod))
                    hata += TasinirMalResI.TMM031 + "<br>";
            }

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tabRapor = new TABRaporlar(ve);
            IDataReader dr = tabRapor.DayanikliTasinirlarDefteriOlustur(kriter, Yetki.YetkiTipiDegerleriDondur(kullanan));

            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return donusDeger;
            }

            DTDBilgi dtd = new DTDBilgi();
            string eskiHesapKod = string.Empty;

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                if (string.IsNullOrEmpty(eskiHesapKod) || eskiHesapKod != VeriErisim.DegerAl(dr, 10, string.Empty))
                {
                    dtd = new DTDBilgi();
                    dtd.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    dtd.ilAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                    dtd.ilceKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    dtd.ilceAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                    dtd.muhasebeKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                    dtd.muhasebeAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                    if (!string.IsNullOrEmpty(kriter.harcamaKod))
                    {
                        dtd.harcamaKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                        dtd.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), dtd.harcamaKod);
                        dtd.harcamaAd = VeriErisim.DegerAl(dr, 7, string.Empty);
                        dtd.ambarKod = VeriErisim.DegerAl(dr, 8, string.Empty);
                        dtd.ambarAd = VeriErisim.DegerAl(dr, 9, string.Empty);
                    }
                    dtd.hesapKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                    eskiHesapKod = dtd.hesapKod;
                    dtd.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), dtd.hesapKod);
                    dtd.hesapAd = VeriErisim.DegerAl(dr, 11, string.Empty);
                    dtd.olcuBirimKod = VeriErisim.DegerAl(dr, 12, 0);
                    dtd.olcuBirimAd = VeriErisim.DegerAl(dr, 13, string.Empty);

                    donusDeger.objeler.Add(dtd);
                }

                DTDBilgiDetay dtdDetay = new DTDBilgiDetay();
                dtdDetay.belgeTarih = VeriErisim.DegerAl(dr, 14);
                dtdDetay.belgeNo = VeriErisim.DegerAl(dr, 15, string.Empty);
                dtdDetay.neredenGeldi = VeriErisim.DegerAl(dr, 16, string.Empty);
                dtdDetay.nereyeGonderildi = VeriErisim.DegerAl(dr, 17, string.Empty);
                dtdDetay.birimFiyatKDVLi = VeriErisim.DegerAl(dr, 18, (decimal)0);
                dtdDetay.miktar = (decimal)1;
                dtdDetay.toplamDeger = dtdDetay.birimFiyatKDVLi * (decimal)dtdDetay.miktar;

                dtdDetay.sicilNo = VeriErisim.DegerAl(dr, 19, string.Empty);
                dtdDetay.islemTipi.kod = VeriErisim.DegerAl(dr, 20, 0);
                dtdDetay.islemTipi.ad = VeriErisim.DegerAl(dr, 21, string.Empty);
                dtdDetay.islemTipi.tur = VeriErisim.DegerAl(dr, 22, 0);
                if (!(dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME ||
                    dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME ||
                    dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME ||
                    dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME))
                    dtdDetay.cikisCinsi = TasinirMalResI.TMM063;
                else if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME ||
                    dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME)
                {
                    dtdDetay.cikisCinsi = TasinirMalResI.TMM064;
                    if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME)
                        dtdDetay.islemTipi.ad = TasinirMalResI.TMM065;
                    else if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME)
                        dtdDetay.islemTipi.ad = TasinirMalResI.TMM066;
                }
                else if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME ||
                    dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                {
                    dtdDetay.cikisCinsi = TasinirMalResI.TMM067;
                    if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                        dtdDetay.islemTipi.ad = TasinirMalResI.TMM065;
                    else if (dtdDetay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME)
                        dtdDetay.islemTipi.ad = TasinirMalResI.TMM066;
                }

                dtdDetay.kimeVerildi = VeriErisim.DegerAl(dr, 23, string.Empty);
                dtdDetay.nereyeVerildi = VeriErisim.DegerAl(dr, 17, string.Empty);

                if (dtdDetay.cikisCinsi == TasinirMalResI.TMM063)
                {
                    dtdDetay.ozellikler += VeriErisim.DegerAl(dr, 29, string.Empty);//Plaka
                    if (dtdDetay.ozellikler != "") dtdDetay.ozellikler += "-";
                    dtdDetay.ozellikler += VeriErisim.DegerAl(dr, 30, string.Empty);//Şase no
                    if (dtdDetay.ozellikler != "") dtdDetay.ozellikler += "-";
                    dtdDetay.ozellikler += VeriErisim.DegerAl(dr, 31, string.Empty);//Motor No
                }
                else
                    dtdDetay.ozellikler = VeriErisim.DegerAl(dr, 24, string.Empty);

                dtdDetay.gonMuhasebeKod = VeriErisim.DegerAl(dr, 25, string.Empty);
                dtdDetay.gonMuhasebeAd = Genel.AdGetir(isimler, dtdDetay.gonMuhasebeKod, "TASMUHASEBE");

                dtdDetay.gonHarcamaKod = VeriErisim.DegerAl(dr, 26, string.Empty);
                dtdDetay.gonHarcamaAd = Genel.AdGetir(isimler, dtdDetay.gonMuhasebeKod + "-" + dtdDetay.gonHarcamaKod, "TASHARCAMABIRIM");

                dtdDetay.gonAmbarKod = VeriErisim.DegerAl(dr, 27, string.Empty);
                dtdDetay.gonAmbarAd = Genel.AdGetir(isimler, dtdDetay.gonMuhasebeKod + "-" + dtdDetay.gonHarcamaKod + "-" + dtdDetay.gonAmbarKod, "TASAMBAR");

                dtdDetay.degerArtisi = VeriErisim.DegerAl(dr, 28, (decimal)0);
                dtdDetay.toplamDeger += dtdDetay.degerArtisi;

                dtdDetay.prSicilNo = VeriErisim.DegerAl(dr, 32, 0);

                dtd.detaylar.Add(dtdDetay);
            }
            dr.Close();

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;
            else if (donusDeger.sonuc.islemSonuc)
            {
                AmortismanKriter ak = new AmortismanKriter();
                ak.yil = kriter.yil;
                ak.muhasebeKod = kriter.muhasebeKod;
                ak.harcamaKod = kriter.harcamaKod;
                ak.ambarKod = kriter.ambarKod;
                ak.hesapPlanKod = kriter.hesapPlanKod;

                TNSCollection amortismanlar = SicilAmortismanSayiVeMiktarListele(ve, ak, false, true);

                bool buldu;
                foreach (Amortisman a in amortismanlar)
                {
                    buldu = false;
                    foreach (DTDBilgi d in donusDeger.objeler)
                    {
                        foreach (DTDBilgiDetay dDetay in d.detaylar)
                        {
                            if (a.prSicilNo == dDetay.prSicilNo)
                            {
                                dDetay.amortismanTutar = a.yapilanToplamAmortisman;
                                buldu = true;
                                break;
                            }
                        }
                        if (buldu)
                            break;
                    }
                }
            }

            ve.VeriErisimKapat();
            return donusDeger;
        }
        ///// <summary>
        ///// Taşınır demirbaşlarının verilen yıldaki amortismanlarını hesaplayıp kaydeden yordam
        ///// Verilen kriterlerle ilgili bir takım kontroller yapılmakta, sorunsuz bir bilgi kümesi
        ///// olduğuna karar verilirse amortisman hesaplama işlemleri yapılıp, bulunan değerler kayıt edilir.
        ///// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="ak">Amortisman yapılacak demirbaşlara ait bilgiler</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
        //    if (!sonuc.islemSonuc)
        //    {
        //        sonuc.hataStr = TasinirMalResI.TMM050;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

        //    int amortismanBaslamaYil = TABGenel.AmortismanBaslamaYilDondur(ve, ak);

        //    sonuc = AmortismanKriterKontrol(kullanan, ak, ENUMIslemTur.KAYDET, amortismanBaslamaYil);
        //    if (!sonuc.islemSonuc)
        //    {
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    if (ak.yil > amortismanBaslamaYil && amortismanBaslamaYil > 0)
        //    {
        //        ak.yil--;

        //        if (!AmortismanYapilmisMiHepsi(ve, ak))
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = string.Format(TasinirMalResI.TMM051, ak.yil.ToString());
        //            ve.VeriErisimKapat();
        //            return sonuc;
        //        }
        //        ak.yil++;
        //    }

        //    ak.yil++;
        //    if (amortismanBaslamaYil > 0 && AmortismanYapilmisMi(ve, ak))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = string.Format(TasinirMalResI.TMM052, ak.yil.ToString());
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }
        //    ak.yil--;

        //    //Amortisman yapılacak yılın amortisman sınırını alıyor
        //    double amortismanSinir = TMMAmortismanSinir.AmortismanSinirVer(ve, kullanan, ak.yil);
        //    if (amortismanSinir <= 0)
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = string.Format(TasinirMalResI.TMM053, ak.yil.ToString());
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    //Amortisman yapılacak yılın hesap kodu bazında amortisman sürelerini alıyor
        //    string hesapPlanKodSure = ak.hesapPlanKod.Replace(".", "");
        //    if (hesapPlanKodSure.Length > 7)
        //        hesapPlanKodSure = hesapPlanKodSure.Substring(0, 7);
        //    ObjectArray amortismanSure = TMMAmortismanSure.Listele(ve, kullanan, new AmortismanSure(ak.yil, hesapPlanKodSure, 0));
        //    if (!amortismanSure.sonuc.islemSonuc)
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = amortismanSure.sonuc.hataStr;
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    //PrSicilNo bazında yapılan değerleme miktarları alınıyor
        //    //Yıl içi değerleme miktarlarının ayrı alınmasının nedeni hesaplamada ayrı kullanılıyor olması
        //    TNSCollection yapilanToplamDegerlemeEskiYillar = TMMAmortismanDegerleme.SicilDegerlemeMiktarListele(ve, ak, false);
        //    TNSCollection yapilanToplamDegerlemeYilIci = TMMAmortismanDegerleme.SicilDegerlemeMiktarListele(ve, ak, true);

        //    try
        //    {
        //        ve.BeginTransaction();
        //        TABAmortisman tAmortisman = new TABAmortisman(ve);

        //        //Kriterlere uyan kayıtlı amortismanlar siliniyor, döngüde hesaplanıp tekrar kayıt edilecek
        //        sonuc = tAmortisman.Sil(ak);

        //        //PrSicilNo bazında yapılan amortisman sayıları ve miktarları alınıyor
        //        //Değerleme amortismanlarının ayrı alınmasının nedeni hesaplamada ayrı kullanılıyor olması
        //        TNSCollection yapilanAmortismanlarDegerlemesiz = SicilAmortismanSayiVeMiktarListele(ve, ak, false, false);
        //        TNSCollection yapilanAmortismanlarDegerleme = SicilAmortismanSayiVeMiktarListele(ve, ak, true, false);

        //        //Amortisman uygulanacak malzemeler bulunuyor
        //        ObjectArray amortismanYapilacaklar = AmortismanYapilacaklariListele(ve, kullanan, ak);
        //        if (!amortismanYapilacaklar.sonuc.islemSonuc)
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = amortismanYapilacaklar.sonuc.hataStr;
        //        }
        //        if (amortismanYapilacaklar.objeler.Count <= 0)
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = amortismanYapilacaklar.sonuc.bilgiStr;
        //        }

        //        if (sonuc.islemSonuc)
        //        {
        //            foreach (SicilNoHareket ay in amortismanYapilacaklar.objeler)
        //            {
        //                //İlgili prSicilNo'ya ait yapılan amortisman sayısı(yas) ve toplamı(yat) bulunuyor
        //                //Değerleme amortismanı hariç
        //                //Eski malzemelerin süresi de burdan alınıyor
        //                int yas = 0;
        //                decimal yat = 0;
        //                int sure = 0;
        //                Amortisman amortisman = YapilanAmortismanBul(yapilanAmortismanlarDegerlemesiz, ay.prSicilNo);
        //                if (amortisman != null)
        //                {
        //                    yas = amortisman.yapilanAmortismanSayisi;
        //                    yat = amortisman.yapilanToplamAmortisman;
        //                    sure = amortisman.kalanAmortismanSure;
        //                }

        //                bool sinirKontroluYap = true;

        //                //Malzeme 2007 ve öncesine aitse süre yukarda bulunmuş olacak
        //                //2008 ve sonrasına aitse süre burada bulunacak
        //                //İlgili hesap koduna ait amortisman süresi bulunuyor
        //                if (sure <= 0)
        //                {
        //                    sure = AmortismanSureBul(amortismanSure.objeler, ay.hesapPlanKod);
        //                    if (sure <= 0)
        //                    {
        //                        sonuc.islemSonuc = false;
        //                        sonuc.hataStr = string.Format(TasinirMalResI.TMM054, ak.yil.ToString(), ay.hesapPlanKod.Substring(0, 9));
        //                        break;
        //                    }
        //                }
        //                else
        //                {
        //                    sinirKontroluYap = false;
        //                    sure++;//2007 ve öncesinin amortismanları için 1 ekleniyor.
        //                }

        //                //İlgili prSicilNo'ya ait yapılan değerleme amortisman sayısı(yasDegerleme)
        //                //ve toplamı(yatDegerleme) bulunuyor
        //                int yasDegerleme = 0;
        //                decimal yatDegerleme = 0;
        //                amortisman = YapilanAmortismanBul(yapilanAmortismanlarDegerleme, ay.prSicilNo);
        //                if (amortisman != null)
        //                {
        //                    yasDegerleme = amortisman.yapilanAmortismanSayisi;
        //                    yatDegerleme = amortisman.yapilanToplamAmortisman;
        //                }

        //                //İlgili prSicilNo'ya ait yapılan değerleme toplamı(degerlemeEski) bulunuyor
        //                //Amortisman yapılacak yıl değerlemesi hariç
        //                decimal degerlemeEski = 0;
        //                AmortismanDegerleme ad = YapilanDegerlemeBul(yapilanToplamDegerlemeEskiYillar, ay.prSicilNo);
        //                if (ad != null)
        //                    degerlemeEski = ad.yapilanToplamDegerlemeEskiYillar;

        //                //İlgili prSicilNo'ya ait amortisman yapılacak yıl içinde yapılan değerleme toplamı(degerlemeYeni) bulunuyor
        //                decimal degerlemeYeni = 0;
        //                ad = YapilanDegerlemeBul(yapilanToplamDegerlemeYilIci, ay.prSicilNo);
        //                if (ad != null)
        //                    degerlemeYeni = ad.yapilanToplamDegerlemeYilIci;

        //                decimal amortismanHesaplanan = 0;
        //                decimal amortismanDegerlemeHesaplanan = 0;

        //                if ((ay.fiyat + degerlemeYeni + degerlemeEski >= (decimal)amortismanSinir || !sinirKontroluYap)
        //                    && ak.amortismanTur != (int)ENUMAmortismanTur.FEVKALADE
        //                    && yas + 1 < sure)
        //                {
        //                    amortismanHesaplanan = (ay.fiyat - yat) / (sure - yas);
        //                    amortismanDegerlemeHesaplanan = ((degerlemeYeni / sure) * (yas + 1)) + ((degerlemeEski - yatDegerleme) / (sure - yas));
        //                }
        //                else
        //                {
        //                    amortismanHesaplanan = ay.fiyat - yat;
        //                    amortismanDegerlemeHesaplanan = degerlemeYeni + degerlemeEski - yatDegerleme;
        //                }

        //                Amortisman yeniAmortisman = new Amortisman();
        //                if (amortismanHesaplanan > 0)
        //                {
        //                    yeniAmortisman.yil = ak.yil;
        //                    yeniAmortisman.prSicilNo = ay.prSicilNo;
        //                    yeniAmortisman.amortismanTur = ak.amortismanTur;
        //                    yeniAmortisman.amortismanMiktar = amortismanHesaplanan;
        //                    yeniAmortisman.muhasebeKod = ay.muhasebeKod;
        //                    yeniAmortisman.harcamaKod = ay.harcamaBirimKod;
        //                    yeniAmortisman.ambarKod = ay.ambarKod;
        //                    yeniAmortisman.gorSicilNo = ay.sicilNo;
        //                    yeniAmortisman.hesapPlanKod = ay.hesapPlanKod;
        //                    yeniAmortisman.islemTarih.Yaz(DateTime.Now);
        //                    yeniAmortisman.islemYapan = kullanan.kullaniciKodu;

        //                    sonuc = tAmortisman.Kaydet(yeniAmortisman);
        //                    if (!sonuc.islemSonuc)
        //                        break;
        //                }

        //                if (amortismanDegerlemeHesaplanan > 0)
        //                {
        //                    yeniAmortisman.amortismanTur = (int)ENUMAmortismanTur.DEGERLEME;
        //                    yeniAmortisman.amortismanMiktar = amortismanDegerlemeHesaplanan;

        //                    sonuc = tAmortisman.Kaydet(yeniAmortisman);
        //                    if (!sonuc.islemSonuc)
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = e.Message;
        //    }

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        ///// <summary>
        ///// Taşınır demirbaşlarının verilen yıldaki amortismanlarını kaydeden yordam
        ///// Verilen kriterlerle ilgili bir takım kontroller yapılmakta, sorunsuz bir bilgi
        ///// kümesi olduğuna karar verilirse gönderilen amortisman değerleri kayıt edilir.
        ///// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="ak">Amortisman yapılacak demirbaşlara ait bilgiler</param>
        ///// <param name="amortismanlar">Amortisman yapılacak demirbaşların amortisman bilgileri listesini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public static Sonuc KaydetElle(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak, TNSCollection amortismanlar)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
        //    if (!sonuc.islemSonuc)
        //    {
        //        sonuc.hataStr = TasinirMalResI.TMM050;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

        //    int amortismanBaslamaYil = TABGenel.AmortismanBaslamaYilDondur(ve, ak);

        //    string hata = string.Empty;
        //    int sayac = 1;
        //    foreach (Amortisman a in amortismanlar)
        //    {
        //        string icHata = string.Empty;
        //        if (a.amortismanMiktar <= 0)
        //            icHata += TasinirMalResI.TMM055 + "<br>";
        //        if (a.amortismanMiktar > a.maliyetTutar - a.yapilanToplamAmortisman)
        //            icHata += TasinirMalResI.TMM056 + "<br>";

        //        if (ak.yil == amortismanBaslamaYil || amortismanBaslamaYil <= 0)
        //        {
        //            if (a.kalanAmortismanSure < 0)
        //                icHata += TasinirMalResI.TMM047 + "<br>";
        //            if (a.amortismanMiktar != a.maliyetTutar && a.kalanAmortismanSure == 0)
        //                icHata += TasinirMalResI.TMM048 + "<br>";
        //            if (a.amortismanMiktar == a.maliyetTutar && a.kalanAmortismanSure != 0)
        //                icHata += TasinirMalResI.TMM049 + "<br>";
        //        }
        //        else
        //        {
        //            if (a.amortismanMiktar != a.maliyetTutar - a.yapilanToplamAmortisman && a.kalanAmortismanSure == 0)
        //                icHata += TasinirMalResI.TMM057 + "<br>";

        //            a.kalanAmortismanSure = 0;
        //        }

        //        if (!string.IsNullOrEmpty(icHata))
        //            hata += string.Format(TasinirMalResI.TMM058, sayac.ToString(), a.gorSicilNo, icHata);
        //    }

        //    sonuc = AmortismanKriterKontrol(kullanan, ak, ENUMIslemTur.KAYDETELLE, amortismanBaslamaYil);

        //    if (!string.IsNullOrEmpty(hata))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr += hata;
        //    }

        //    if (!sonuc.islemSonuc)
        //    {
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    if (ak.yil > amortismanBaslamaYil && amortismanBaslamaYil > 0)
        //    {
        //        ak.yil--;
        //        if (!AmortismanYapilmisMiHepsi(ve, ak))
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = string.Format(TasinirMalResI.TMM051, ak.yil);
        //            ve.VeriErisimKapat();
        //            return sonuc;
        //        }
        //        ak.yil++;
        //    }

        //    ak.yil++;
        //    if (amortismanBaslamaYil > 0 && AmortismanYapilmisMi(ve, ak))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = string.Format(TasinirMalResI.TMM052, ak.yil);
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }
        //    ak.yil--;

        //    try
        //    {
        //        TABAmortisman tAmortisman = new TABAmortisman(ve);
        //        ve.BeginTransaction();

        //        //Kriterlere uyan kayıtlı amortismanlar siliniyor, döngüde hesaplanıp tekrar kayıt edilecek
        //        sonuc = tAmortisman.Sil(ak);

        //        //Amortisman uygulanacak malzemeler bulunuyor
        //        ObjectArray amortismanYapilacaklar = AmortismanYapilacaklariListele(ve, kullanan, ak);
        //        if (!amortismanYapilacaklar.sonuc.islemSonuc)
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = amortismanYapilacaklar.sonuc.hataStr;
        //        }
        //        if (amortismanYapilacaklar.objeler.Count <= 0)
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = amortismanYapilacaklar.sonuc.bilgiStr;
        //        }

        //        if (sonuc.islemSonuc)
        //        {
        //            foreach (Amortisman a in amortismanlar)
        //            {
        //                //İlgili Sicile ait bilgiler bulunuyor
        //                //PrSicilNo tek tek uzaydan çekilmek yerine, tek seferde alınan listede aranıyor
        //                SicilNoHareket sicil = SicilBilgileriniBul(amortismanYapilacaklar.objeler, a.gorSicilNo);
        //                if (sicil == null)
        //                {
        //                    sonuc.islemSonuc = false;
        //                    sonuc.hataStr = string.Format(TasinirMalResI.TMM059, a.gorSicilNo);
        //                    break;
        //                }

        //                if (a.amortismanMiktar >= 0)
        //                {
        //                    a.yil = ak.yil;
        //                    a.prSicilNo = sicil.prSicilNo;
        //                    a.muhasebeKod = sicil.muhasebeKod;
        //                    a.harcamaKod = sicil.harcamaBirimKod;
        //                    a.ambarKod = sicil.ambarKod;
        //                    a.gorSicilNo = sicil.sicilNo;
        //                    a.hesapPlanKod = sicil.hesapPlanKod;
        //                    a.islemTarih.Yaz(DateTime.Now);
        //                    a.islemYapan = kullanan.kullaniciKodu;

        //                    sonuc = tAmortisman.Kaydet(a);
        //                    if (!sonuc.islemSonuc)
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = e.Message;
        //    }

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan demirbaşların amortismanlarını hesaplayıp döndüren yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="ak">Amortisman listeleme kriterleri</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaşların hesaplanmış amortisman bilgileri listesi döndürülür.</returns>
        //public static ObjectArray AmortismanHesapla(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        //{
        //    ObjectArray listeVAN = new ObjectArray();
        //    listeVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
        //    if (!listeVAN.sonuc.islemSonuc)
        //    {
        //        listeVAN.sonuc.hataStr = TasinirMalResI.TMM050;
        //        return listeVAN;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

        //    int amortismanBaslamaYil = TABGenel.AmortismanBaslamaYilDondur(ve, ak);

        //    listeVAN.sonuc = AmortismanKriterKontrol(kullanan, ak, ENUMIslemTur.HESAPLA, amortismanBaslamaYil);
        //    if (!listeVAN.sonuc.islemSonuc)
        //    {
        //        ve.VeriErisimKapat();
        //        return listeVAN;
        //    }

        //    TABAmortisman tAmortisman = new TABAmortisman(ve);

        //    //Amortismanı hesaplanacak malzemeler bulunuyor
        //    ObjectArray amortismanYapilacaklar = AmortismanYapilacaklariListele(ve, kullanan, ak);
        //    if (!amortismanYapilacaklar.sonuc.islemSonuc)
        //    {
        //        listeVAN.sonuc.islemSonuc = false;
        //        listeVAN.sonuc.hataStr = amortismanYapilacaklar.sonuc.hataStr;
        //        return listeVAN;
        //    }
        //    if (amortismanYapilacaklar.objeler.Count <= 0)
        //    {
        //        listeVAN.sonuc.islemSonuc = false;
        //        listeVAN.sonuc.hataStr = amortismanYapilacaklar.sonuc.bilgiStr;
        //        return listeVAN;
        //    }

        //    foreach (SicilNoHareket ay in amortismanYapilacaklar.objeler)
        //    {
        //        Amortisman yeniAmortisman = new Amortisman();
        //        yeniAmortisman.prSicilNo = ay.prSicilNo;
        //        yeniAmortisman.gorSicilNo = ay.sicilNo;
        //        yeniAmortisman.hesapPlanKod = ay.hesapPlanKod;
        //        yeniAmortisman.hesapPlanAd = ay.hesapPlanAd;
        //        yeniAmortisman.maliyetTutar = ay.fiyat;
        //        yeniAmortisman.amortismanMiktar = yeniAmortisman.maliyetTutar * (decimal)ak.birikmisAmortismanOran / 100;
        //        yeniAmortisman.kalanAmortismanSure = ak.kalanAmortismanSure;

        //        listeVAN.objeler.Add(yeniAmortisman);
        //    }

        //    if (listeVAN.objeler.Count == 0)
        //        listeVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

        //    ve.VeriErisimKapat();
        //    return listeVAN;
        //}

        ///// <summary>
        ///// Parametre olarak verilen amortisman süreleri listesinden ilgili
        ///// taşınır hesap planı koduna ait amortisman süresini bulan yordam
        ///// </summary>
        ///// <param name="objeler">Amortisman süre bilgileri listesini tutan nesne</param>
        ///// <param name="hesapKod">Amortisman süresi aranan taşınır hesap planı kodu</param>
        ///// <returns>Amortisman süresi döndürülür.</returns>
        //private static int AmortismanSureBul(TNSCollection objeler, string hesapKod)
        //{
        //    foreach (AmortismanSure sure in objeler)
        //    {
        //        if (hesapKod.Replace(".", "").StartsWith(sure.hesapPlanKod.Replace(".", "")))
        //            return sure.sure;
        //    }
        //    return 0;
        //}

        ///// <summary>
        ///// Parametre olarak verilen demirbaş listesinden ilgili
        ///// sicil numarasına ait demirbaş bilgilerini bulan yordam
        ///// </summary>
        ///// <param name="objeler">Demirbaş bilgileri listesini tutan nesne</param>
        ///// <param name="gorSicilNo">Demirbaş bilgileri aranan demirbaşın sicil numarası</param>
        ///// <returns>Demirbaş bilgileri nesnesi döndürülür.</returns>
        //private static SicilNoHareket SicilBilgileriniBul(TNSCollection objeler, string gorSicilNo)
        //{
        //    foreach (SicilNoHareket sicil in objeler)
        //    {
        //        if (sicil.sicilNo == gorSicilNo)
        //            return sicil;
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Parametre olarak verilen amortisman bilgileri listesinden ilgili programatik
        ///// sicil numaralı demirbaşa ait amortisman bilgilerini bulan yordam
        ///// </summary>
        ///// <param name="objeler">Amortisman bilgileri listesini tutan nesne</param>
        ///// <param name="prSicilNo">Amortisman bilgileri aranan demirbaşın programatik sicil numarası</param>
        ///// <returns>Amortisman bilgileri nesnesi döndürülür.</returns>
        //private static Amortisman YapilanAmortismanBul(TNSCollection objeler, int prSicilNo)
        //{
        //    foreach (Amortisman amortisman in objeler)
        //    {
        //        if (amortisman.prSicilNo == prSicilNo)
        //            return amortisman;
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Parametre olarak verilen değerleme bilgileri listesinden ilgili programatik
        ///// sicil numaralı demirbaşa ait değerleme bilgilerini bulan yordam
        ///// </summary>
        ///// <param name="objeler">Değerleme bilgileri listesini tutan nesne</param>
        ///// <param name="prSicilNo">Değerleme bilgileri aranan demirbaşın programatik sicil numarası</param>
        ///// <returns>Değerleme bilgileri nesnesi döndürülür.</returns>
        //private static AmortismanDegerleme YapilanDegerlemeBul(TNSCollection objeler, int prSicilNo)
        //{
        //    foreach (AmortismanDegerleme ad in objeler)
        //    {
        //        if (ad.prSicilNo == prSicilNo)
        //            return ad;
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan amortisman işlemi yapılacak taşınır demirbaşlarını döndüren yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="ak">Amortisman listeleme kriterleri</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır demirbaşları listesi döndürülür.</returns>
        //public static ObjectArray AmortismanYapilacaklariListele(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        //{
        //    return AmortismanYapilacaklariListele(veriErisim, kullanan, ak, false);
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan amortisman işlemi yapılacak taşınır demirbaşlarını döndüren yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="ak">Amortisman listeleme kriterleri</param>
        ///// <param name="bitenlerDahil">Kalan amortismanı sıfırlanmış demirbaşlar dahil edilsin mi bilgisi</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır demirbaşları listesi döndürülür.</returns>
        //private static ObjectArray AmortismanYapilacaklariListele(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak, bool bitenlerDahil)
        //{
        //    ObjectArray listeVAN = new ObjectArray();

        //    listeVAN.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
        //    if (!listeVAN.sonuc.islemSonuc)
        //    {
        //        listeVAN.sonuc.hataStr = TasinirMalResI.TMM060;
        //        return listeVAN;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

        //    int amortismanBaslamaYil = TABGenel.AmortismanBaslamaYilDondur(ve, ak);

        //    listeVAN.sonuc = AmortismanKriterKontrol(kullanan, ak, ENUMIslemTur.LISTELE, amortismanBaslamaYil);
        //    if (!listeVAN.sonuc.islemSonuc)
        //    {
        //        ve.VeriErisimKapat();
        //        return listeVAN;
        //    }

        //    TABAmortisman tAmortisman = new TABAmortisman(ve);
        //    IDataReader dr = null;
        //    try
        //    {
        //        dr = tAmortisman.AmortismanYapilacaklar(ak, bitenlerDahil);
        //    }
        //    catch (Exception e)
        //    {
        //        listeVAN.sonuc.islemSonuc = false;
        //        listeVAN.sonuc.hataStr = e.Message;
        //        dr.Close();
        //        ve.VeriErisimKapat();
        //        return listeVAN;
        //    }

        //    System.Collections.Hashtable isimler = new System.Collections.Hashtable();
        //    while (dr.Read())
        //    {
        //        SicilNoHareket gelen = new SicilNoHareket();
        //        gelen.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
        //        gelen.siraNo = VeriErisim.DegerAl(dr, 1, 0);
        //        gelen.yil = VeriErisim.DegerAl(dr, 2, 0);

        //        gelen.muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
        //        gelen.muhasebeAd = Genel.AdGetir(isimler, gelen.muhasebeKod, "TASMUHASEBE");

        //        gelen.harcamaBirimKod = VeriErisim.DegerAl(dr, 4, string.Empty);
        //        gelen.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaBirimKod);
        //        gelen.harcamaBirimAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaBirimKod, "TASHARCAMABIRIM");

        //        gelen.fisNo = VeriErisim.DegerAl(dr, 5, string.Empty);
        //        gelen.fisNo = gelen.fisNo.PadLeft(6, '0');
        //        gelen.fisSiraNo = VeriErisim.DegerAl(dr, 6, 0);
        //        gelen.sicilNo = VeriErisim.DegerAl(dr, 7, string.Empty);
        //        gelen.sicilSiraNo = VeriErisim.DegerAl(dr, 8, string.Empty);

        //        gelen.hesapPlanKod = VeriErisim.DegerAl(dr, 9, string.Empty);
        //        gelen.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.hesapPlanKod);
        //        gelen.hesapPlanAd = Genel.AdGetir(isimler, gelen.hesapPlanKod, "TASHESAPPLAN");

        //        gelen.ambarKod = VeriErisim.DegerAl(dr, 10, string.Empty);
        //        gelen.ambarAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaBirimKod + "-" + gelen.ambarKod, "TASAMBAR");

        //        gelen.fiyat = VeriErisim.DegerAl(dr, 11, (decimal)0);
        //        gelen.kdvOran = VeriErisim.DegerAl(dr, 12, 0);
        //        gelen.islemTipKod = VeriErisim.DegerAl(dr, 13, 0);
        //        gelen.islemTurKod = VeriErisim.DegerAl(dr, 14, 0);
        //        gelen.islemTarih = VeriErisim.DegerAl(dr, 15);
        //        gelen.islemYapan = VeriErisim.DegerAl(dr, 16, string.Empty);
        //        gelen.durum = VeriErisim.DegerAl(dr, 17, 0);
        //        gelen.fisTarih = VeriErisim.DegerAl(dr, 18);
        //        gelen.neredenGeldi = VeriErisim.DegerAl(dr, 19, string.Empty);
        //        gelen.nereyeGitti = VeriErisim.DegerAl(dr, 20, string.Empty);
        //        gelen.kimeGitti = VeriErisim.DegerAl(dr, 21, string.Empty);
        //        gelen.zimmetOzellik = VeriErisim.DegerAl(dr, 22, string.Empty);
        //        gelen.kdvliBirimFiyat = VeriErisim.DegerAl(dr, 23, (decimal)0);
        //        gelen.degerArtisi = VeriErisim.DegerAl(dr, 24, (decimal)0);
        //        gelen.gonMuhasebeKod = VeriErisim.DegerAl(dr, 25, string.Empty);
        //        gelen.gonHarcamaBirimKod = VeriErisim.DegerAl(dr, 26, string.Empty);
        //        gelen.gonHarcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.gonHarcamaBirimKod);
        //        gelen.gonAmbarKod = VeriErisim.DegerAl(dr, 27, string.Empty);

        //        listeVAN.objeler.Add(gelen);
        //    }
        //    dr.Close();

        //    if (listeVAN.objeler.Count == 0)
        //        listeVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

        //    ve.VeriErisimKapat();
        //    return listeVAN;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan cari amortisman miktarlarını demirbaş bazında döndüren yordam
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="ak">Amortisman listeleme kriterleri</param>
        ///// <param name="degerleme">Değerleme işleminden kaynaklı cari amortisman mı isteniyor bilgisi</param>
        ///// <returns>Cari amortisman bilgileri döndürülür.</returns>
        //private static TNSCollection SicilAmortismanCariMiktarListele(VeriErisim veriErisim, AmortismanKriter ak, bool degerleme)
        //{
        //    TNSCollection liste = new TNSCollection();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABAmortisman tAmortisman = new TABAmortisman(ve);

        //    IDataReader dr = tAmortisman.SicilAmortismanCariMiktarListele(ak, degerleme);
        //    while (dr.Read())
        //    {
        //        Amortisman gelen = new Amortisman();
        //        gelen.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
        //        gelen.amortismanMiktar = VeriErisim.DegerAl(dr, 1, (decimal)0);

        //        liste.Add(gelen);
        //    }
        //    dr.Close();

        //    ve.VeriErisimKapat();
        //    return liste;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan taşınır demirbaşlarının amortismanlarını silen yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="ak">Amortismanı silinecek demirbaşlara ait bilgiler</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
        //    if (!sonuc.islemSonuc)
        //    {
        //        sonuc.hataStr = TasinirMalResI.TMM061;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

        //    int amortismanBaslamaYil = TABGenel.AmortismanBaslamaYilDondur(ve, ak);

        //    sonuc = AmortismanKriterKontrol(kullanan, ak, ENUMIslemTur.SIL, amortismanBaslamaYil);
        //    if (!sonuc.islemSonuc)
        //    {
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    ak.yil++;
        //    if (AmortismanYapilmisMi(ve, ak))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = string.Format(TasinirMalResI.TMM052, ak.yil.ToString());
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }
        //    ak.yil--;

        //    TABAmortisman tAmortisman = new TABAmortisman(ve);
        //    ve.BeginTransaction();

        //    try
        //    {
        //        sonuc = tAmortisman.Sil(ak);
        //    }
        //    catch (Exception e)
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = e.Message;
        //    }

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan tüm demirbaşlar için amortisman uygulanmış mı kontrolünü yapan yordam
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="ak">Amortisman listeleme kriterleri</param>
        ///// <returns>Kontrol sonucu döndürülür.</returns>
        //private static bool AmortismanYapilmisMiHepsi(VeriErisim veriErisim, AmortismanKriter ak)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABAmortisman tAmortisman = new TABAmortisman(ve);
        //    bool sonuc = tAmortisman.AmortismanYapilmisMiHepsi(ak);
        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        public static ObjectArray AmortismanRaporuMB(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            if (ak.raporTur == (int)ENUMMBRaporTur.MALZEMEGURUBUNAGOREADETVEBEDELLER)
                return HesapPlaninaGoreDemirbasSayilariveAmortismanlari(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.GENELSIGORTALISTESI || ak.raporTur == (int)ENUMMBRaporTur.OTOVETABLOLARHARICSIGORTALISTESI)
                return SigortaListesi(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.BIRIMMALZEMELISTESIAMOTRISMANLI || ak.raporTur == (int)ENUMMBRaporTur.BIRIMMALZEMELISTESI || ak.raporTur == (int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE || ak.raporTur == (int)ENUMMBRaporTur.DEMIRBASDEFTERI)
                return MalzemeAmortismanRaporu(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMISDEMIRBASLAR || ak.raporTur == (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMEMISDEMIRBASLAR)
                return YilSonuAmortismanRaporu(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI || ak.raporTur == (int)ENUMMBRaporTur.IHRACLISTESIMALZEMETURU)
                return IhracListesi(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMISLER || ak.raporTur == (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMEMISLER ||
                ak.raporTur == (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMIS || ak.raporTur == (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMEMISLER)
                return YilBazindaAmortismanRaporu(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.YERLESKEYERINEGORETOPLAMAMORTISMAN)
                return YerleskeYerineGoreToplamAmortismanRaporu(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.SOSYALTESISLEREAYRILANCARIAMAMORTISMAN)
                return SosyalTesislereAyrilanCariAmortismanRaporu(null, kullanan, ak);
            else if (ak.raporTur == 100)
                return GenelAmortismanRaporu(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.BIRIMLEREGOREDEMIRBASAMORTISMAN)
                return BirimlereGoreDemirbasAmortisman(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.BIRIMLEREGOREGAYRIMENKULAMORTISMAN)
                return BirimlereGoreGayriMenkulAmortisman(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.DURANVARLIKHAREKETRAPORU)
                return DuranVarlikHareketRaporu(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.MUHASEBEBAKIYEKARSILASTIRMA)
                return MuhasebeKarsilastirmaRaporu(null, kullanan, ak);
            else if (ak.raporTur == (int)ENUMMBRaporTur.ENFLASYONDUZELTMESI)
            {
                if (Arac.MerkezBankasiKullaniyor())
                    return EnflasyonDuzeltmesiAmortismanRaporuMB(null, kullanan, ak);
                return EnflasyonDuzeltmesiAmortismanRaporu(null, kullanan, ak, true);
            }

            return null;
        }

        private static ObjectArray HesapPlaninaGoreDemirbasSayilariveAmortismanlari(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                IDataReader dr = tAmortisman.HesapPlaninaGoreDemirbasSayilariveAmortismanlari(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                AmortismanRapor ar2 = new AmortismanRapor();
                while (dr.Read())
                {
                    int siraNo = 0;

                    AmortismanRapor ar = new AmortismanRapor();

                    ar.hesapPlanKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    //ar.hesapPlanAdet = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.maliyetTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal degerArtisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis() { maliyetTutar = degerArtisTutar });
                    ar.maliyetAmortismanBirikmisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.degerArtisAmortismanTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    if (ar2.hesapPlanKod != ar.hesapPlanKod)
                    {
                        ar2 = new AmortismanRapor();
                        ar2.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis());

                        listeVAN.Ekle(ar2);
                    }

                    ar2.hesapPlanKod = ar.hesapPlanKod;
                    ar2.hesapPlanAd = ar.hesapPlanAd;
                    ar2.hesapPlanAdet += 1;
                    ar2.maliyetTutar += ar.maliyetTutar;
                    ((AmortismanRaporDegerArtis)ar2.degerArtisAmortisman[0]).maliyetTutar += degerArtisTutar;
                    ar2.maliyetAmortismanBirikmisTutar += ar.maliyetAmortismanBirikmisTutar;
                    ar2.degerArtisAmortismanTutar += ar.degerArtisAmortismanTutar;

                }
                dr.Close();

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static ObjectArray SigortaListesi(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                IDataReader dr = tAmortisman.SigortaListesi(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                AmortismanRapor ar2 = new AmortismanRapor();
                while (dr.Read())
                {
                    int siraNo = 0;

                    AmortismanRapor ar = new AmortismanRapor();

                    ar.nereyeGitti = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGittiAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.maliyetTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal degerArtisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis() { maliyetTutar = degerArtisTutar });

                    if (ar2.nereyeGitti != ar.nereyeGitti)
                    {
                        ar2 = new AmortismanRapor();
                        ar2.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis());

                        listeVAN.Ekle(ar2);
                    }

                    ar2.nereyeGitti = ar.nereyeGitti;
                    ar2.nereyeGittiAd = ar.nereyeGittiAd;
                    ar2.maliyetTutar += ar.maliyetTutar;
                    ((AmortismanRaporDegerArtis)ar2.degerArtisAmortisman[0]).maliyetTutar += degerArtisTutar;

                }
                dr.Close();

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static ObjectArray MalzemeAmortismanRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.sureAsimi = 60 * 2;
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                IDataReader dr = tAmortisman.MalzemeAmortismanRaporu(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                string plaka = "";
                string adi = "";
                string yeriKonusu = "";
                string saseNo = "";
                string ekNo = "";
                string giai = "";

                while (dr.Read())
                {
                    int siraNo = 0;

                    AmortismanRapor ar = new AmortismanRapor();
                    ar.prSicilNo = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.gorSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.amortismanSuresi = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.girisTarih = VeriErisim.DegerAl(dr, siraNo++);
                    ar.disSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.disSicilNo2 = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.bisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.eskiBisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.markaAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.modelAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGitti = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGittiAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.harcamaBirimiKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    string ambarAd = VeriErisim.DegerAl(dr, siraNo++, "");

                    if (string.IsNullOrWhiteSpace(ar.nereyeGittiAd))
                        ar.nereyeGittiAd = ambarAd;
                    ar.nereyeGittiAd = ar.harcamaBirimiKod + " - " + ar.nereyeGittiAd;

                    ar.maliyetTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    decimal degerArtisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal enflasyonDuzeltmeTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    ar.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis() { maliyetTutar = degerArtisTutar });
                    ar.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis() { maliyetTutar = enflasyonDuzeltmeTutar, amortismanTur = (int)ENUMAmortismanTur.FEVKALADE });

                    ar.maliyetAmortismanBirikmisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.degerArtisAmortismanTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.enflasyonDuzAmortismanTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    ar.amortismanBaslamaYil = ar.girisTarih.Yil;

                    plaka = VeriErisim.DegerAl(dr, siraNo++, "");
                    adi = VeriErisim.DegerAl(dr, siraNo++, "");
                    yeriKonusu = VeriErisim.DegerAl(dr, siraNo++, "");
                    saseNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ekNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    giai = VeriErisim.DegerAl(dr, siraNo++, "");

                    ar.islemTip.kod = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.islemTip.tur = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.islemTip.ad = VeriErisim.DegerAl(dr, siraNo++, "");

                    //ar.faturaNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    //ar.faturaTarih = VeriErisim.DegerAl(dr, siraNo++);

                    if (ak.raporTur == (int)ENUMMBRaporTur.DEMIRBASDEFTERI)
                    {
                        ar.faturaNo = VeriErisim.DegerAl(dr, siraNo++, "");
                        ar.faturaTarih = VeriErisim.DegerAl(dr, siraNo++);
                        ar.neredenGeldi = VeriErisim.DegerAl(dr, siraNo++, "");
                    }

                    string ozellik = "";

                    if (!String.IsNullOrEmpty(ar.markaAd))
                        ozellik = ar.markaAd;
                    if (!String.IsNullOrEmpty(ar.modelAd))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += ar.modelAd;
                    }
                    if (!String.IsNullOrEmpty(plaka))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += plaka;
                    }
                    if (!String.IsNullOrEmpty(adi))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += adi;
                    }
                    if (!String.IsNullOrEmpty(yeriKonusu))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += yeriKonusu;
                    }
                    if (!String.IsNullOrEmpty(saseNo))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += saseNo;
                    }
                    if (!String.IsNullOrEmpty(ekNo))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += ekNo;
                    }
                    if (String.IsNullOrEmpty(saseNo) && !String.IsNullOrEmpty(giai))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += giai;
                    }

                    ar.ozellik = ozellik;

                    listeVAN.Ekle(ar);
                }
                dr.Close();

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static ObjectArray YilSonuAmortismanRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                IDataReader dr = tAmortisman.YilSonuAmortismanRaporu(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                string plaka = "";
                string adi = "";
                string yeriKonusu = "";
                string saseNo = "";
                string ekNo = "";

                while (dr.Read())
                {
                    int siraNo = 0;

                    AmortismanRapor ar = new AmortismanRapor();
                    ar.prSicilNo = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.gorSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.disSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.amortismanSuresi = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.girisTarih = VeriErisim.DegerAl(dr, siraNo++);
                    ar.disSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.disSicilNo2 = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.bisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.eskiBisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.markaAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.modelAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGitti = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGittiAd = VeriErisim.DegerAl(dr, siraNo++, "");

                    ar.maliyetTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    decimal degerArtisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal enflasyonDuzeltmeTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    ar.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis() { maliyetTutar = degerArtisTutar });
                    ar.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis() { maliyetTutar = enflasyonDuzeltmeTutar, amortismanTur = (int)ENUMAmortismanTur.FEVKALADE });

                    ar.maliyetAmortismanBirikmisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.degerArtisAmortismanTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.enflasyonDuzAmortismanTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    ar.amortismanBaslamaYil = ar.girisTarih.Yil;

                    plaka = VeriErisim.DegerAl(dr, siraNo++, "");
                    adi = VeriErisim.DegerAl(dr, siraNo++, "");
                    yeriKonusu = VeriErisim.DegerAl(dr, siraNo++, "");
                    saseNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ekNo = VeriErisim.DegerAl(dr, siraNo++, "");

                    string ozellik = "";

                    if (!String.IsNullOrEmpty(ar.markaAd))
                        ozellik = ar.markaAd;
                    if (!String.IsNullOrEmpty(ar.modelAd))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += ar.modelAd;
                    }
                    if (!String.IsNullOrEmpty(plaka))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += plaka;
                    }
                    if (!String.IsNullOrEmpty(adi))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += adi;
                    }
                    if (!String.IsNullOrEmpty(yeriKonusu))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += yeriKonusu;
                    }
                    if (!String.IsNullOrEmpty(saseNo))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += saseNo;
                    }
                    if (!String.IsNullOrEmpty(ekNo))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += ekNo;
                    }

                    ar.ozellik = ozellik;

                    bool tamamiAmortiEdilmis = (ar.maliyetTutar + degerArtisTutar - ar.maliyetAmortismanBirikmisTutar - ar.degerArtisAmortismanTutar) == (decimal)0.0;

                    if ((ak.raporTur == (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMISDEMIRBASLAR && tamamiAmortiEdilmis) ||
                        (ak.raporTur == (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMEMISDEMIRBASLAR && !tamamiAmortiEdilmis))
                        listeVAN.Ekle(ar);

                }
                dr.Close();

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static ObjectArray IhracListesi(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                string hata = "";

                if (!string.IsNullOrWhiteSpace(ak.belgeNo))
                    ak.belgeNo = ak.belgeNo.PadLeft(6, '0');

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                IDataReader dr = tAmortisman.IhracListesi(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                string plaka = "";
                string adi = "";
                string yeriKonusu = "";
                string saseNo = "";
                string ekNo = "";

                while (dr.Read())
                {
                    int siraNo = 0;

                    AmortismanRapor ar = new AmortismanRapor();
                    ar.prSicilNo = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.gorSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.amortismanSuresi = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.girisTarih = VeriErisim.DegerAl(dr, siraNo++);
                    ar.disSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.disSicilNo2 = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.bisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.eskiBisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.markaAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.modelAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGitti = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGittiAd = VeriErisim.DegerAl(dr, siraNo++, "");

                    ar.maliyetTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    decimal degerArtisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal enflasyonDuzeltmeTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    ar.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis() { maliyetTutar = degerArtisTutar });
                    ar.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis() { maliyetTutar = enflasyonDuzeltmeTutar, amortismanTur = (int)ENUMAmortismanTur.FEVKALADE });

                    ar.maliyetAmortismanBirikmisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.degerArtisAmortismanTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.enflasyonDuzAmortismanTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);


                    plaka = VeriErisim.DegerAl(dr, siraNo++, "");
                    adi = VeriErisim.DegerAl(dr, siraNo++, "");
                    yeriKonusu = VeriErisim.DegerAl(dr, siraNo++, "");
                    saseNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ekNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.cikisFisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.cikisTarih = VeriErisim.DegerAl(dr, siraNo++);

                    string ozellik = "";

                    if (!String.IsNullOrEmpty(ar.markaAd))
                        ozellik = ar.markaAd;
                    if (!String.IsNullOrEmpty(ar.modelAd))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += ar.modelAd;
                    }
                    if (!String.IsNullOrEmpty(plaka))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += plaka;
                    }
                    if (!String.IsNullOrEmpty(adi))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += adi;
                    }
                    if (!String.IsNullOrEmpty(yeriKonusu))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += yeriKonusu;
                    }
                    if (!String.IsNullOrEmpty(saseNo))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += saseNo;
                    }
                    if (!String.IsNullOrEmpty(ekNo))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += ekNo;
                    }

                    ar.ozellik = ozellik;

                    ar.amortismanBaslamaYil = ar.girisTarih.Yil;

                    listeVAN.Ekle(ar);
                }
                dr.Close();

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static ObjectArray YilBazindaAmortismanRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                IDataReader dr = tAmortisman.YilBazindaAmortismanRaporu(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                SortedList listeOran = new SortedList();
                SortedList liste = new SortedList();
                decimal[] degerler = new decimal[4];


                decimal bedelToplam = 0;
                decimal amortismanYuzde = 0;
                decimal cariYilAmortisman = 0;
                decimal gecmisYilAmortisman = 0;
                int islemYapilanPrSicilNo = 0;
                int islemYapilanYil = 0;

                while (dr.Read())
                {
                    int siraNo = 0;

                    int amortismanBaslamaYil = VeriErisim.DegerAl(dr, siraNo++, 0);
                    int prSicilNo = VeriErisim.DegerAl(dr, siraNo++, 0);
                    decimal bedel = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal amortismanMiktar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    int amortismanYil = VeriErisim.DegerAl(dr, siraNo++, 0);
                    int amortismanDonem = VeriErisim.DegerAl(dr, siraNo++, 0);
                    int amortismanSure = VeriErisim.DegerAl(dr, siraNo++, 0);

                    if (amortismanYil > ak.yil)
                        continue;

                    if (islemYapilanPrSicilNo == 0)
                    {
                        islemYapilanPrSicilNo = prSicilNo;
                        islemYapilanYil = amortismanBaslamaYil;
                    }

                    if (islemYapilanPrSicilNo != prSicilNo)
                    {
                        TMMAmortisman.YilBazindaAmortismanEkle(ak, cariYilAmortisman, gecmisYilAmortisman, bedelToplam, amortismanYuzde, liste, listeOran, islemYapilanYil);

                        islemYapilanPrSicilNo = prSicilNo;
                        amortismanYuzde = 0;
                        cariYilAmortisman = 0;
                        gecmisYilAmortisman = 0;
                        islemYapilanYil = amortismanBaslamaYil;
                    }

                    if (amortismanYil == ak.yil) // (amortismanYil == amortismanBaslamaYil)  ???
                        cariYilAmortisman += amortismanMiktar;
                    else if (amortismanYil < ak.yil)
                        gecmisYilAmortisman += amortismanMiktar;
                    else
                        continue;

                    bedelToplam = bedel;
                    double y = amortismanSure > 0 ? Math.Truncate(((Double)100 / (Double)amortismanSure) * 100) / 100 : 0;
                    amortismanYuzde = (decimal)y;
                }
                dr.Close();

                TMMAmortisman.YilBazindaAmortismanEkle(ak, cariYilAmortisman, gecmisYilAmortisman, bedelToplam, amortismanYuzde, liste, listeOran, islemYapilanYil);


                if (listeOran.Count > 0)
                    liste = listeOran;

                listeVAN.Ekle(liste);

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static void YilBazindaAmortismanEkle(AmortismanKriter ak, decimal cariYilAmortisman, decimal gecmisYilAmortisman, decimal bedelToplam, decimal amortismanYuzde, SortedList liste, SortedList listeOran, int islemYapilanYil)
        {
            decimal[] degerler = new decimal[4];

            bool ekle = true;
            bool tamamiAmortiEdilmis = ((cariYilAmortisman + gecmisYilAmortisman) - bedelToplam) >= 0 ? true : false;

            if ((ak.raporTur == (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMIS && tamamiAmortiEdilmis))
            {
                amortismanYuzde = (decimal)100;
                if (liste.ContainsKey(amortismanYuzde))
                    degerler = (decimal[])liste[amortismanYuzde];
                else
                    liste.Add(amortismanYuzde, degerler);
            }
            else if (ak.raporTur == (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMEMISLER && !tamamiAmortiEdilmis && amortismanYuzde < 100)
            {
                if (liste.ContainsKey(amortismanYuzde))
                    degerler = (decimal[])liste[amortismanYuzde];
                else
                    liste.Add(amortismanYuzde, degerler);
            }
            else if (ak.raporTur == (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMEMISLER && !tamamiAmortiEdilmis && amortismanYuzde < 100)
            {
                if (listeOran.ContainsKey(amortismanYuzde))
                    liste = (SortedList)listeOran[amortismanYuzde];
                else
                {
                    liste = new SortedList();
                    listeOran.Add(amortismanYuzde, liste);
                }

                if (liste.ContainsKey(islemYapilanYil))
                    degerler = (decimal[])liste[islemYapilanYil];
                else
                    liste.Add(islemYapilanYil, degerler);

            }
            else if (ak.raporTur == (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMISLER && tamamiAmortiEdilmis)
            {
                if (liste.ContainsKey(islemYapilanYil))
                    degerler = (decimal[])liste[islemYapilanYil];
                else
                    liste.Add(islemYapilanYil, degerler);
            }
            else
                ekle = false;


            if (ekle)
            {
                degerler[0] += bedelToplam;
                degerler[1] += cariYilAmortisman;
                degerler[2] += gecmisYilAmortisman;
                degerler[3] += (cariYilAmortisman + gecmisYilAmortisman);
            }
        }

        private static ObjectArray YerleskeYerineGoreToplamAmortismanRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                IDataReader dr = tAmortisman.YerleskeYerineGoreToplamAmortismanRaporu(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                AmortismanRapor ar2 = new AmortismanRapor();
                while (dr.Read())
                {
                    int siraNo = 0;

                    AmortismanRapor ar = new AmortismanRapor();

                    ar.nereyeGitti = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGittiAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.maliyetTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal degerArtisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis() { maliyetTutar = degerArtisTutar });
                    ar.maliyetAmortismanBirikmisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    ar.degerArtisAmortismanTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    if (ar2.nereyeGitti != ar.nereyeGitti)
                    {
                        ar2 = new AmortismanRapor();
                        ar2.degerArtisAmortisman.Ekle(new AmortismanRaporDegerArtis());

                        listeVAN.Ekle(ar2);
                    }

                    ar2.nereyeGitti = ar.nereyeGitti;
                    ar2.nereyeGittiAd = ar.nereyeGittiAd;
                    ar2.maliyetTutar += ar.maliyetTutar;
                    ((AmortismanRaporDegerArtis)ar2.degerArtisAmortisman[0]).maliyetTutar += degerArtisTutar;
                    ar2.maliyetAmortismanBirikmisTutar += ar.maliyetAmortismanBirikmisTutar;
                    ar2.degerArtisAmortismanTutar = ar.degerArtisAmortismanTutar;

                }
                dr.Close();

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static ObjectArray SosyalTesislereAyrilanCariAmortismanRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                if (MerkezBankasiGenel.Ayarlar != null && MerkezBankasiGenel.Ayarlar.sosyalTesisListe != null)
                {
                    foreach (var tesis in MerkezBankasiGenel.Ayarlar.sosyalTesisListe)
                    {
                        if (ak.nereyeGitti != "")
                            ak.nereyeGitti += ",";
                        ak.nereyeGitti += tesis.kod;
                    }
                }

                if (ak.nereyeGitti == "")
                    ak.nereyeGitti = "1519,1063,1906"; // "KİK2400S21,D040000,D050200000";

                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                IDataReader dr = tAmortisman.SosyalTesislereAyrilanCariAmortismanRaporu(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                while (dr.Read())
                {
                    int siraNo = 0;

                    AmortismanRapor ar = new AmortismanRapor();
                    ar.nereyeGittiAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGitti = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.cariMaliyetAmortismanTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    listeVAN.Ekle(ar);
                }
                dr.Close();

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        public static int AmortismanDonemiBul(string harcamaBirimKod, TNSDateTime ilkGirisTarih)
        {
            int donem = 12;
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                //Banknot birimi ise aylık
                if (MerkezBankasiGenel.Ayarlar.BanknotBirimiMi(harcamaBirimKod))
                    donem = ilkGirisTarih.Ay;
                else //Diğer birimler 3'er aylık
                {
                    if (ilkGirisTarih.Ay <= 3)
                        donem = 3;
                    else if (ilkGirisTarih.Ay <= 6)
                        donem = 6;
                    else if (ilkGirisTarih.Ay <= 9)
                        donem = 9;
                    else if (ilkGirisTarih.Ay <= 12)
                        donem = 12;
                }
            }

            return donem;
        }

        public static AmortismanIslemForm SonAmortismanBilgisi(VeriErisim veriErisim, Kullanici kullanan, string muhasebeKod, string harcamaKod, string ambarKod, bool onayli = true)
        {
            AmortismanIslemForm form = new AmortismanIslemForm()
            {
                muhasebeKod = muhasebeKod,
                harcamaKod = harcamaKod,
                ambarKod = ambarKod,
                durum = onayli ? 5 : 0
            };

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray amortismanFormListe = TMMAmortismanIslemForm.Oku(ve, kullanan, form, false);

            int yilDonem = 0;
            foreach (AmortismanIslemForm aform in amortismanFormListe.objeler)
            {
                if (aform.yil * 100 + aform.donem > yilDonem)
                {
                    yilDonem = aform.yil * 100 + aform.donem;
                    form = aform;
                }
            }

            ve.VeriErisimKapat();

            if (form.yil == 0)
                return null;
            return form;
        }

        private static ObjectArray GenelAmortismanRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                IDataReader dr = tAmortisman.GenelAmortismanRaporu(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                SortedList liste = new SortedList();
                decimal[] degerler = new decimal[13];


                int islemYapilanPrSicilNo = 0;
                int islemYapilanYil = 0;

                decimal ilkBedel = 0;
                decimal bedelDuzeltmeFarki = 0;
                decimal sonBedel = 0; //ilkBedel + bedelDuzeltmeFarki
                decimal birikmisAmortisman = 0;
                decimal birikmisAmortismanFarki = 0;
                decimal duzelmisBirikenAmortisman = 0; // birikmisAmortisman + birikmisAmortismanFarki

                decimal cariAmortismanCariYilGiren = 0;
                decimal cariAmortismanCariGecmisYilGiren = 0;
                decimal cariAmortisman = cariAmortismanCariYilGiren + cariAmortismanCariGecmisYilGiren;

                decimal toplamAmortisman = 0; //duzelmisBirikenAmortisman + cariAmortisman
                decimal kalanAmortisman = 0; //sonBedel - toplamAmortisman

                decimal ilkAmortismanYuzde = 0; //Hesaplanır
                decimal yapilanAmortismanYuzde = 0; //Hesaplanır


                while (dr.Read())
                {
                    int siraNo = 0;

                    int amortismanBaslamaYil = VeriErisim.DegerAl(dr, siraNo++, 0);
                    int prSicilNo = VeriErisim.DegerAl(dr, siraNo++, 0);
                    decimal ilkBedeli = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal degerArtis = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal amortismanMiktar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    int amortismanTur = VeriErisim.DegerAl(dr, siraNo++, 0);
                    int amortismanYil = VeriErisim.DegerAl(dr, siraNo++, 0);
                    int amortismanDonem = VeriErisim.DegerAl(dr, siraNo++, 0);
                    int amortismanSure = VeriErisim.DegerAl(dr, siraNo++, 0);

                    if (amortismanYil > ak.yil)
                        continue;

                    if (islemYapilanPrSicilNo == 0)
                    {
                        islemYapilanPrSicilNo = prSicilNo;
                        islemYapilanYil = amortismanBaslamaYil;
                    }

                    if (islemYapilanPrSicilNo != prSicilNo)
                    {
                        degerler = new decimal[13];

                        degerler[0] = ilkBedel;
                        degerler[1] = bedelDuzeltmeFarki;
                        degerler[2] = sonBedel;

                        degerler[3] = birikmisAmortisman;
                        degerler[4] = birikmisAmortismanFarki;
                        degerler[5] = duzelmisBirikenAmortisman;

                        degerler[6] = cariAmortismanCariYilGiren;
                        degerler[7] = cariAmortismanCariGecmisYilGiren;
                        degerler[8] = cariAmortisman;

                        degerler[9] = toplamAmortisman;
                        degerler[10] = kalanAmortisman;

                        degerler[11] = ilkAmortismanYuzde;
                        degerler[12] = yapilanAmortismanYuzde;

                        liste.Add(islemYapilanPrSicilNo, degerler);


                        islemYapilanPrSicilNo = prSicilNo;
                        islemYapilanYil = amortismanBaslamaYil;

                        ilkBedel = 0;
                        bedelDuzeltmeFarki = 0;
                        sonBedel = 0; //ilkBedel + bedelDuzeltmeFarki
                        birikmisAmortisman = 0;
                        birikmisAmortismanFarki = 0;
                        duzelmisBirikenAmortisman = 0; // birikmisAmortisman + birikmisAmortismanFarki

                        cariAmortismanCariYilGiren = 0;
                        cariAmortismanCariGecmisYilGiren = 0;
                        cariAmortisman = cariAmortismanCariYilGiren + cariAmortismanCariGecmisYilGiren;

                        toplamAmortisman = 0; //duzelmisBirikenAmortisman + cariAmortisman
                        kalanAmortisman = 0; //sonBedel - toplamAmortisman

                        ilkAmortismanYuzde = 0; //Hesaplanır
                        yapilanAmortismanYuzde = 0; //Hesaplanır
                    }


                    ilkBedel = ilkBedeli;
                    bedelDuzeltmeFarki = degerArtis;
                    sonBedel = ilkBedel + bedelDuzeltmeFarki;


                    if (amortismanYil == ak.yil && amortismanDonem != 0 && amortismanTur == 1)
                    {
                        cariAmortisman = amortismanMiktar;
                        //if (amortismanBaslamaYil == amortismanYil)
                        //    cariAmortismanCariYilGiren += amortismanMiktar;
                        //else
                        //    cariAmortismanCariGecmisYilGiren += amortismanMiktar;
                        //cariAmortisman = cariAmortismanCariYilGiren + cariAmortismanCariGecmisYilGiren;
                    }
                    //else
                    {
                        if (amortismanTur == 1)
                            birikmisAmortisman += amortismanMiktar;
                        else
                            birikmisAmortismanFarki += amortismanMiktar;
                        duzelmisBirikenAmortisman = birikmisAmortisman + birikmisAmortismanFarki;
                    }

                    toplamAmortisman = duzelmisBirikenAmortisman;
                    kalanAmortisman = sonBedel - toplamAmortisman;


                    decimal y1 = amortismanSure > 0 ? Math.Truncate(((decimal)100 / (decimal)amortismanSure) * 100) / 100 : 0;
                    decimal y2 = amortismanSure > 0 ? Math.Truncate((decimal)(toplamAmortisman / sonBedel) * 100 * 100) / 100 : 0;

                    ilkAmortismanYuzde = y1;
                    yapilanAmortismanYuzde = y2;

                }
                dr.Close();

                if (islemYapilanPrSicilNo != 0)
                {
                    degerler = new decimal[13];

                    degerler[0] = ilkBedel;
                    degerler[1] = bedelDuzeltmeFarki;
                    degerler[2] = sonBedel;

                    degerler[3] = birikmisAmortisman;
                    degerler[4] = birikmisAmortismanFarki;
                    degerler[5] = duzelmisBirikenAmortisman;

                    degerler[6] = cariAmortismanCariYilGiren;
                    degerler[7] = cariAmortismanCariGecmisYilGiren;
                    degerler[8] = cariAmortisman;

                    degerler[9] = toplamAmortisman;
                    degerler[10] = kalanAmortisman;

                    degerler[11] = ilkAmortismanYuzde;
                    degerler[12] = yapilanAmortismanYuzde;

                    liste.Add(islemYapilanPrSicilNo, degerler);


                    islemYapilanPrSicilNo = 0;
                    islemYapilanYil = 0;

                    ilkBedel = 0;
                    bedelDuzeltmeFarki = 0;
                    sonBedel = 0; //ilkBedel + bedelDuzeltmeFarki
                    birikmisAmortisman = 0;
                    birikmisAmortismanFarki = 0;
                    duzelmisBirikenAmortisman = 0; // birikmisAmortisman + birikmisAmortismanFarki

                    cariAmortismanCariYilGiren = 0;
                    cariAmortismanCariGecmisYilGiren = 0;
                    cariAmortisman = cariAmortismanCariYilGiren + cariAmortismanCariGecmisYilGiren;

                    toplamAmortisman = 0; //duzelmisBirikenAmortisman + cariAmortisman
                    kalanAmortisman = 0; //sonBedel - toplamAmortisman

                    ilkAmortismanYuzde = 0; //Hesaplanır
                    yapilanAmortismanYuzde = 0; //Hesaplanır
                }



                listeVAN.Ekle(liste);

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        public static AmortismanRaporBA AmortismanRaporlaBA(Kullanici kullanan, AmortismanKriter ak)
        {
            AmortismanRaporBA raporBAOnay = new AmortismanRaporBA();

            try
            {
                ObjectArray aListe = AmortismanUretimRaporu3(null, kullanan, ak, false);

                foreach (TNS.TMM.AmortismanRapor3 ar in aListe.objeler)
                {
                    if (ar.maliyetTutar == 0 || ar.amortismanSuresi == 0 || ar.toplamCariAmortisman == 0)
                        continue;

                    if (ar.toplamKalanAmortisman == 0)
                    {
                        raporBAOnay.adetA++;
                        raporBAOnay.ilkBedel += ar.maliyetTutar;
                        raporBAOnay.bedelDuzeltmeFarki += ar.degerDuzeltmeToplamTutar;
                        raporBAOnay.sonBedelA += ar.toplamTutar;

                        raporBAOnay.birikenAmortisman += ar.maliyetBirikmisAmortisman;
                        raporBAOnay.amortismanDuzeltmeFarki += ar.degerDuzeltmeBirikmisAmortisman;
                        raporBAOnay.duzeltilmisBirikenAmortisman += ar.toplamBirikmisAmortisman;

                        raporBAOnay.sonBedelB += ar.sonDonemAmortisman;
                    }
                    else
                    {
                        raporBAOnay.adet2A++;
                        raporBAOnay.ilkBedel2 += ar.maliyetTutar;
                        raporBAOnay.bedelDuzeltmeFarki2 += ar.degerDuzeltmeToplamTutar;
                        raporBAOnay.sonBedel2A += ar.toplamTutar;

                        raporBAOnay.birikenAmortisman2 += ar.maliyetBirikmisAmortisman;
                        raporBAOnay.amortismanDuzeltmeFarki2 += ar.degerDuzeltmeBirikmisAmortisman;
                        raporBAOnay.duzeltilmisBirikenAmortisman2 += ar.toplamBirikmisAmortisman;

                        raporBAOnay.sonBedel2B += ar.sonDonemAmortisman;
                    }
                }

            }
            catch (Exception e)
            {
                return raporBAOnay;
            }

            return raporBAOnay;
        }

        private static ObjectArray BirimlereGoreDemirbasAmortisman(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            IDataReader dr = null;
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                dr = tAmortisman.BirimlereGoreDemirbasAmortisman(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                while (dr.Read())
                {
                    int siraNo = 0;

                    string birimAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    decimal amortisman = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal maliyet = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal degerArtis = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal netDeger = maliyet + degerArtis - amortisman;

                    var o = new object[] {
                        birimAd,
                        amortisman,
                        maliyet,
                        degerArtis,
                        netDeger
                    };

                    listeVAN.Ekle(o);
                }

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }

            if (dr != null)
                dr.Close();
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static ObjectArray BirimlereGoreGayriMenkulAmortisman(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                IDataReader dr = tAmortisman.BirimlereGoreGayriMenkulAmortisman(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                while (dr.Read())
                {
                    int siraNo = 0;

                    string birimAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    decimal amortisman = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal maliyet = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal degerArtis = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal netDeger = maliyet + degerArtis - amortisman;

                    var o = new object[] {
                        birimAd,
                        amortisman,
                        maliyet,
                        degerArtis,
                        netDeger
                    };

                    listeVAN.Ekle(o);
                }
                dr.Close();

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static ObjectArray DuranVarlikHareketRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();

            if (ak.yil <= 1800 || ak.donem <= 0)
            {
                listeVAN.sonuc.DegerAta("Rapor için yıl ve dönem bilgisini belirtmelisiniz.");
                return listeVAN;
            }


            ArrayList list1 = new ArrayList();
            var dCariBilanco = new DuranHareketTablosuMB("1 Ocak net bilanço değeri", ak.yil);
            var dCariAlimlar = new DuranHareketTablosuMB("Alımlar", ak.yil);
            var dCariEldenCikanlarBedeli = new DuranHareketTablosuMB("Elden çıkarılanlar", ak.yil);
            var dCariEldenCikanlar = new DuranHareketTablosuMB("Elden çıkarılanlar (net) (*)", ak.yil);
            var dCariAmortisman = new DuranHareketTablosuMB("Dönem İçi Amortisman", ak.yil);

            list1.Add(dCariBilanco);
            list1.Add(dCariAlimlar);
            list1.Add(dCariEldenCikanlar);
            list1.Add(dCariAmortisman);


            listeVAN.Ekle(list1);

            ArrayList list2 = new ArrayList();
            var dKayitliDegerCariYil = new DuranHareketTablosuMB("Kayıtlı değeri", ak.yil);
            var dBirikmisAmortismanCariYil = new DuranHareketTablosuMB("Birikmiş amortismanlar", ak.yil);

            list2.Add(dKayitliDegerCariYil);
            list2.Add(dBirikmisAmortismanCariYil);

            listeVAN.Ekle(list2);

            ArrayList list3 = new ArrayList();
            var dKayitliDegerGecmisYil = new DuranHareketTablosuMB("Kayıtlı değeri", ak.yil - 1);
            var dBirikmisAmortismanGecmisYil = new DuranHareketTablosuMB("Birikmiş amortismanlar", ak.yil - 1);

            list3.Add(dKayitliDegerGecmisYil);
            list3.Add(dBirikmisAmortismanGecmisYil);

            listeVAN.Ekle(list3);


            //ObjectArray listeAr = new ObjectArray();


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.sureAsimi = 10 * 60;
            IDataReader dr = null;
            try
            {
                decimal enAzTutar = TMMAmortismanSinir.AmortismanSinirVer(ve, kullanan, ak.yil);

                ENUMAmortismanDonemTip donemTip = ENUMAmortismanDonemTip.YILLIK;
                int donemAyKriter = 12;
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    if (MerkezBankasiGenel.Ayarlar.BanknotBirimiMi(ak.harcamaKod)) //Banknot birimi ise aylık
                    {
                        donemTip = ENUMAmortismanDonemTip.AYLIK;
                        donemAyKriter = 1;
                    }
                    else //Diğer birimler 3'er aylık
                    {
                        donemTip = ENUMAmortismanDonemTip.UCAYLIK;
                        donemAyKriter = 3;
                    }
                }
                int donemAyIslemYapilanx = donemAyKriter;


                TABAmortisman tAmortisman = new TABAmortisman(ve);

                dr = tAmortisman.SicilNoHareketGirisBul(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }
                string noktasizHB = ak.harcamaKod.Replace(".", "");

                while (dr.Read())
                {
                    AmortismanRapor3 ar = new AmortismanRapor3();
                    ar.amortismanKaydet = false;
                    ar.yil = ak.yil;
                    ar.donem = ak.donem;

                    //donemAyIslemYapilan değişebileceğinden her bir kayıt için donemAyKriter yeniden set ediliyor.
                    ar.donemAyIslemYapilan = donemAyKriter;

                    ar.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                    ar.amortismanBaslamaYil = VeriErisim.DegerAl(dr, 1, 0);
                    ar.gorSicilNo = VeriErisim.DegerAl(dr, 2, "");
                    ar.hesapPlanKod = VeriErisim.DegerAl(dr, 3, "");
                    ar.maliyetTutar = VeriErisim.DegerAl(dr, 4, (decimal)0.0);
                    ar.girisTarih = VeriErisim.DegerAl(dr, 5);
                    ar.islemTipiKod = VeriErisim.DegerAl(dr, 6, 0);
                    ar.islemTuruKod = VeriErisim.DegerAl(dr, 7, 0);
                    ar.hesapPlanAd = VeriErisim.DegerAl(dr, 8, "");
                    ar.amortismanSuresi = VeriErisim.DegerAl(dr, 9, 0);
                    ar.kistAmortisman = VeriErisim.DegerAl(dr, 10, 0);
                    ar.saymanlikKod = VeriErisim.DegerAl(dr, 11, "");
                    ar.harcamaBirimiKod = VeriErisim.DegerAl(dr, 12, "");
                    ar.ambarKod = VeriErisim.DegerAl(dr, 13, "");
                    //ar.saymanlikAdi = VeriErisim.DegerAl(dr, 14, "");
                    ar.harcamaBirimiAdi = VeriErisim.DegerAl(dr, 15, "");

                    //if (!(ar.prSicilNo == 199232 || ar.prSicilNo == 110988 || ar.prSicilNo == 112751))
                    //    continue;

                    //if (!(ar.prSicilNo == 173525))
                    //    continue;

                    if (string.IsNullOrWhiteSpace(ar.gorSicilNo))
                        continue;


                    TNSCollection listeSicilNoHareket = ListeleDetaylar(VeriErisim.DegerAl(dr, 16, ""), 1);
                    TNSCollection listeDegerArtis = ListeleDetaylar(VeriErisim.DegerAl(dr, 17, ""), 2);
                    TNSCollection listeAmortisman = ListeleDetaylar(VeriErisim.DegerAl(dr, 18, ""), 3);
                    TNSCollection listeOzellik = ListeleDetaylar(VeriErisim.DegerAl(dr, 19, ""), 4);

                    DateTime ilkGiris = DateTime.MaxValue;

                    string sonMuh = "";
                    string sonBir = "";
                    string sonAmb = "";

                    foreach (SicilNoHareket s in listeSicilNoHareket)
                    {
                        if (s.fisTarih.Oku() < ilkGiris)
                        {
                            ilkGiris = s.fisTarih.Oku();
                            if (s.kdvliBirimFiyat > 0)
                                ar.maliyetTutar = s.kdvliBirimFiyat; //Değer artış son harekete eklendiği için ilk bedel buradan alınsın. H.Ö
                        }
                        //1,2,3,4,6,7,8,49,5
                        //50,52,53,54,55,56,57,58,59,51
                        if (s.yil <= ak.yil && s.durum != (int)ENUMSicilDurum.IPTAL &&
                            (s.islemTurKod == 1 || s.islemTurKod == 2 || s.islemTurKod == 3 ||
                            s.islemTurKod == 4 || s.islemTurKod == 6 || s.islemTurKod == 7 ||
                            s.islemTurKod == 8 || s.islemTurKod == 49 || s.islemTurKod == 5))
                        {
                            sonMuh = s.muhasebeKod;
                            sonBir = s.harcamaBirimKod;
                            sonAmb = s.ambarKod;
                            ar.sonGirisTarih = s.fisTarih;
                            ar.girisFisNo = s.fisNo;
                            ar.sonGirisNo = s.siraNo;
                        }

                        if (s.yil <= ak.yil && s.durum != (int)ENUMSicilDurum.IPTAL &&
                           s.islemTurKod >= 50 && s.islemTurKod <= 59 &&
                           s.islemTipKod != 24) //Geçici İhraç
                        {
                            ar.cikisTarih = s.fisTarih;
                            ar.cikisNo = s.siraNo;
                            ar.cikisFisNo = s.fisNo;
                            ar.islemDurumAciklama = s.nereyeGitti;
                        }

                        // Burası kapatıldı. Yazılım ve Gayrimenkul işlemlerinde zimmet olmadığı için sicil ozellik tablosundan bulunduğu yer okunmalı.

                        if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                        {
                            if (s.durum != (int)ENUMSicilDurum.IPTAL && (s.islemTurKod == 90 || s.islemTurKod == 91))
                            {
                                ar.nereyeGitti = s.nereyeGitti;
                                ar.nereyeGittiAd = s.odaAd;
                            }
                        }

                    }
                    ar.girisTarih = new TNSDateTime(ilkGiris);
                    ar.amortismanBaslamaYil = ilkGiris.Year;
                    ar.amortismanBaslamaDonem = AmortismanDonemiBul(ak.harcamaKod, ar.girisTarih);
                    int durum = 0;

                    if (ar.sonGirisNo == 0 || (ar.cikisNo > ar.sonGirisNo))
                    {
                        durum = 1; //girmemiş veya çıkmış
                    }
                    else if (string.IsNullOrWhiteSpace(sonMuh) || string.IsNullOrWhiteSpace(sonBir) || string.IsNullOrWhiteSpace(sonAmb))
                    {
                        durum = 9; // yerini bulamadık devam et.
                    }
                    else if (!string.IsNullOrEmpty(ak.muhasebeKod) && ak.muhasebeKod != sonMuh)
                    {
                        durum = 2; //Malzeme raporlanan muhasebede değil devam et.
                    }
                    else if (!sonBir.StartsWith(noktasizHB))
                    {
                        durum = 3; //Malzeme raporlanan birimde değil devam et.
                    }
                    else if (!string.IsNullOrEmpty(ak.ambarKod) && ak.ambarKod != sonAmb && (ak.ambarKod.StartsWith("@") && ak.ambarKod.IndexOf(sonAmb) == -1))
                    {
                        durum = 4; //Malzeme raporlanan ambarda değil devam et.
                    }

                    if (ar.amortismanBaslamaYil > ak.yil || (ar.amortismanBaslamaYil == ak.yil && ak.donem > 0 && ar.amortismanBaslamaDonem > ak.donem))
                    {
                        durum = 5; //raporladığım dönemden sonra girmiş.
                    }
                    else if (ar.islemTuruKod == (int)ENUMIslemTipi.DEVIRCIKISKURUM && ar.islemTipiKod != 24) //Geçici İhraç
                    {
                        durum = 6; //İhraç edilmiş
                    }
                    if (ar.islemTuruKod == 53)
                    {
                        durum = 7; //Satış
                    }

                    if (ar.cikisTarih != null && ar.cikisTarih.Yil == ak.yil && (durum == 6 || durum == 7))
                    {
                        //TTK amortisman raporunda ihraç edilmiş veya çıkışı yapılmışlarda gelsin. (Merkez Bankası)
                        ar.cariYilEldenCikti = true;
                    }
                    else if (durum > 1)
                        continue;


                    bool amortismanBitti = false;

                    ar.gayriMenkulMu = TNS.TMM.Arac.MerkezBankasiKullaniyor() && ar.ambarKod == "50";
                    ar.banknotGayriMenkulMu = ar.gayriMenkulMu && MerkezBankasiGenel.Ayarlar.BanknotBirimiMi(ak.harcamaKod);

                    foreach (SicilNoOzellik so in listeOzellik)
                    {
                        ar.bisNo = so.bisNo;
                        ar.eskiBisNo = so.eskiBisNo1;
                        ar.eskiSicilNo = so.disSicilNo;

                        if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                        {
                            ar.nereyeGitti = so.bulunduguYer;
                            ar.nereyeGittiAd = so.bulunduguYerAd;
                            ar.maliyetMerkezi = so.fotograf; //maliyet merkezi için bu alan kullanıldı
                        }

                        if (so.amortismanYil > 0)
                            ar.amortismanSuresi = so.amortismanYil;

                        //BANKNOT MATBAASI 1970 - 2003 ARASI GİRİŞLER İÇİN % 4
                        //BANKNOT MATBAASI 2004 SONRASI GİRİŞLER İÇİN % 2,5
                        //İDARE MERKEZİ VE ŞUBELER İÇİN % 2
                        if (ar.banknotGayriMenkulMu)
                        {
                            if (ar.girisTarih.Yil <= 2003)
                                ar.amortismanSuresi = 25;
                            else if (ar.girisTarih.Yil >= 2004)
                                ar.amortismanSuresi = 40;
                        }

                        amortismanBitti = so.amortismanBitti > 0;
                    }

                    if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    {
                        if (ar.hesapPlanKod.StartsWith("0880106") || ar.hesapPlanAd.StartsWith("SANAT ESERLERİ"))
                            amortismanBitti = true;
                    }

                    //if ((ar.amortismanSuresi == 0 && ak.amortismanSure != 100))
                    //    continue;


                    foreach (SicilNoDegerArtis da in listeDegerArtis)
                    {
                        if (da.durum != 5)
                            continue;

                        //Değer artış/azalış ve 2023 öncesi edfler TTK ya dahil edilsin
                        if (da.tur == 1 && da.belgeTarihi.Yil >= 2023)
                            continue;

                        AmortismanRapor3 ard = new AmortismanRapor3();
                        ard.amortismanKaydet = ar.amortismanKaydet;
                        ard.yil = ar.yil;
                        ard.donem = ar.donem;
                        ard.donemAyIslemYapilan = ar.donemAyIslemYapilan;
                        ard.degerArtisAmortismanYapilacak = false;


                        ard.prSicilNo = ar.prSicilNo;
                        ard.gorSicilNo = ar.gorSicilNo;
                        ard.hesapPlanKod = ar.hesapPlanKod;
                        ard.islemTuruKod = ar.islemTuruKod;
                        ard.hesapPlanAd = ar.hesapPlanAd;
                        ard.amortismanSuresi = ar.amortismanSuresi;

                        ard.saymanlikKod = ar.saymanlikKod;
                        ard.harcamaBirimiKod = ar.harcamaBirimiKod;
                        ard.ambarKod = ar.ambarKod;

                        ard.girisTarih = da.belgeTarihi;
                        ard.sonGirisTarih = da.belgeTarihi;
                        ard.sonGirisNo = OrtakFonksiyonlar.ConvertToInt(da.belgeNo, 0);
                        //ar2.cikisTarih = ? 
                        //ar2.cikisNo = ?
                        if (da.tur == 1 || da.tur == 3)
                            ard.amortismanTur = (int)ENUMAmortismanTur.FEVKALADE;
                        else if (da.tur == 2)
                            ard.amortismanTur = (int)ENUMAmortismanTur.DEGERLEME;
                        else
                            ard.amortismanTur = (int)ENUMAmortismanTur.DEGERLEME;

                        if (ard.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                            ard.amortismanBaslamaYil = da.belgeTarihi.Yil;
                        else
                            ard.amortismanBaslamaYil = ar.amortismanBaslamaYil; //Değer azalış ve enflasyon düzeltme işleminde amortisman süreri giriş amortisman ile aynı olmalı

                        ard.degerArtisKod = da.kod;
                        ard.maliyetTutar = (decimal)da.tutar;


                        AmortismanRapor3DegerArtis ard3 = new AmortismanRapor3DegerArtis();
                        ard3.prSicilNo = ard.prSicilNo;
                        ard3.gorSicilNo = ard.gorSicilNo;
                        ard3.degerArtisKod = ard.degerArtisKod;
                        ard3.amortismanTur = ard.amortismanTur;
                        ard3.maliyetTutar = ard.maliyetTutar;
                        ard3.maliyetCariAmortisman = ard.maliyetCariAmortisman;
                        ard3.maliyetBirikmisAmortisman = ard.maliyetBirikmisAmortisman;

                        ar.degerDuzeltmeAmortismanX.Ekle(ard3);

                    }


                    if (ar.girisTarih.Yil == ak.yil)
                    {
                        //Cari yıl alınan

                        if (ar.ambarKod == "01")
                            dCariAlimlar.demirbas += ar.toplamTutar;
                        else if (ar.ambarKod == "51")
                            dCariAlimlar.yazilim += ar.toplamTutar;
                        else if (ar.ambarKod == "50")
                            dCariAlimlar.gayrimenkul += ar.toplamTutar;
                    }

                    if (ar.cariYilEldenCikti)
                    {

                        //Cari yıl elde çıkan
                        ar.maliyetBirikmisAmortisman = (decimal)OrtakFonksiyonlar.KurusYap(TTKAmortismanHesabi((double)(ar.toplamTutar), ar.girisTarih, ak.yil - 1, 12, ar.amortismanSuresi));

                        int ttkCariDonem = ak.donem;
                        if (ttkCariDonem > ar.cikisTarih.Ay)
                            ttkCariDonem = ar.cikisTarih.Ay;


                        decimal cariMaliyetAmortismanTutarTTK = (decimal)OrtakFonksiyonlar.KurusYap(TTKAmortismanHesabiCariYil((double)(ar.toplamTutar), ar.girisTarih, ak.yil, ttkCariDonem, ar.amortismanSuresi));

                        decimal ihracAmortisman = ar.maliyetBirikmisAmortisman + cariMaliyetAmortismanTutarTTK;


                        //TTK için amortisman sınırı dikkate alınmayacak. HÖ - 2025.04.15
                        ////Bu bölüm merkez bankası talebi üzerine eklendi. Teyit edielcek.
                        //if (ar.girisTarih.Yil == ak.yil && ar.toplamTutar <= enAzTutar)
                        //{
                        //    ihracAmortisman = ar.toplamTutar;
                        //}

                        if (ar.ambarKod == "01")
                        {
                            //dCariAmortisman.demirbas += cariMaliyetAmortismanTutarTTK;
                            dCariEldenCikanlar.demirbas += ar.toplamTutar - ihracAmortisman;
                            dCariEldenCikanlarBedeli.demirbas += ar.toplamTutar;
                        }
                        else if (ar.ambarKod == "51")
                        {
                            //dCariAmortisman.yazilim += cariMaliyetAmortismanTutarTTK;
                            dCariEldenCikanlar.yazilim += ar.toplamTutar - ihracAmortisman;
                            dCariEldenCikanlarBedeli.yazilim += ar.toplamTutar;
                        }
                        else if (ar.ambarKod == "50")
                        {
                            //dCariAmortisman.gayrimenkul += cariMaliyetAmortismanTutarTTK;
                            dCariEldenCikanlar.gayrimenkul += ar.toplamTutar - ihracAmortisman;
                            dCariEldenCikanlarBedeli.gayrimenkul += ar.toplamTutar;
                        }
                    }
                    else
                    {

                        decimal cariMaliyetAmortismanTutarTTK = (decimal)OrtakFonksiyonlar.KurusYap(TTKAmortismanHesabiCariYil((double)(ar.toplamTutar), ar.girisTarih, ak.yil, ak.donem, ar.amortismanSuresi));

                        //TTK için amortisman sınırı dikkate alınmayacak. HÖ - 2025.04.15

                        //if (ar.girisTarih.Yil == ak.yil && ar.toplamTutar <= enAzTutar)
                        //{
                        //    if (ar.ambarKod == "01")
                        //        dCariAmortisman.demirbas += ar.toplamTutar;
                        //    else if (ar.ambarKod == "51")
                        //        dCariAmortisman.yazilim += ar.toplamTutar;
                        //    else if (ar.ambarKod == "50")
                        //        dCariAmortisman.gayrimenkul += cariMaliyetAmortismanTutarTTK;
                        //}
                        //else
                        {
                            if (ar.ambarKod == "01")
                                dCariAmortisman.demirbas += cariMaliyetAmortismanTutarTTK;
                            else if (ar.ambarKod == "51")
                                dCariAmortisman.yazilim += cariMaliyetAmortismanTutarTTK;
                            else if (ar.ambarKod == "50")
                                dCariAmortisman.gayrimenkul += cariMaliyetAmortismanTutarTTK;
                        }


                        if (ar.ambarKod == "01")
                            dKayitliDegerCariYil.demirbas += ar.toplamTutar;
                        else if (ar.ambarKod == "51")
                            dKayitliDegerCariYil.yazilim += ar.toplamTutar;
                        else if (ar.ambarKod == "50")
                            dKayitliDegerCariYil.gayrimenkul += ar.toplamTutar;
                    }

                    if (ar.girisTarih.Yil < ak.yil)
                    {
                        //Geçmiş yıl toplamı

                        //Geçimiş yıl ihraç ve çıkış yapılanlar dahil edilmesin
                        if (ar.cariYilEldenCikti)
                            continue;

                        ar.maliyetBirikmisAmortisman = (decimal)OrtakFonksiyonlar.KurusYap(TTKAmortismanHesabi((double)(ar.toplamTutar), ar.girisTarih, ak.yil - 1, 12, ar.amortismanSuresi));

                        if (ar.ambarKod == "01")
                        {
                            dKayitliDegerGecmisYil.demirbas += ar.toplamTutar;
                            dBirikmisAmortismanGecmisYil.demirbas += ar.maliyetBirikmisAmortisman;
                        }
                        else if (ar.ambarKod == "51")
                        {
                            dKayitliDegerGecmisYil.yazilim += ar.toplamTutar;
                            dBirikmisAmortismanGecmisYil.yazilim += ar.maliyetBirikmisAmortisman;
                        }
                        else if (ar.ambarKod == "50")
                        {
                            dKayitliDegerGecmisYil.gayrimenkul += ar.toplamTutar;
                            dBirikmisAmortismanGecmisYil.gayrimenkul += ar.maliyetBirikmisAmortisman;
                        }
                    }

                }

                decimal cariIhracAmortismanGayrimenkul = dCariEldenCikanlarBedeli.gayrimenkul - dCariEldenCikanlar.gayrimenkul;
                decimal cariIhracAmortismanDemirbas = dCariEldenCikanlarBedeli.demirbas - dCariEldenCikanlar.demirbas;
                decimal cariIhracAmortismanYazilim = dCariEldenCikanlarBedeli.yazilim - dCariEldenCikanlar.yazilim;


                dBirikmisAmortismanCariYil.gayrimenkul = dBirikmisAmortismanGecmisYil.gayrimenkul + dCariAmortisman.gayrimenkul;
                dBirikmisAmortismanCariYil.demirbas = dBirikmisAmortismanGecmisYil.demirbas + dCariAmortisman.demirbas;
                dBirikmisAmortismanCariYil.yazilim = dBirikmisAmortismanGecmisYil.yazilim + dCariAmortisman.yazilim;


                dKayitliDegerGecmisYil.gayrimenkul = dKayitliDegerCariYil.gayrimenkul - dCariAlimlar.gayrimenkul + dCariEldenCikanlarBedeli.gayrimenkul;
                dKayitliDegerGecmisYil.demirbas = dKayitliDegerCariYil.demirbas - dCariAlimlar.demirbas + dCariEldenCikanlarBedeli.demirbas;
                dKayitliDegerGecmisYil.yazilim = dKayitliDegerCariYil.yazilim - dCariAlimlar.yazilim + dCariEldenCikanlarBedeli.yazilim;



                dBirikmisAmortismanGecmisYil.gayrimenkul = dBirikmisAmortismanGecmisYil.gayrimenkul + cariIhracAmortismanGayrimenkul;
                dBirikmisAmortismanGecmisYil.demirbas = dBirikmisAmortismanGecmisYil.demirbas + cariIhracAmortismanDemirbas;
                dBirikmisAmortismanGecmisYil.yazilim = dBirikmisAmortismanGecmisYil.yazilim + cariIhracAmortismanYazilim;


                dCariBilanco.gayrimenkul = dKayitliDegerGecmisYil.gayrimenkul - dBirikmisAmortismanGecmisYil.gayrimenkul;
                dCariBilanco.demirbas = dKayitliDegerGecmisYil.demirbas - dBirikmisAmortismanGecmisYil.demirbas;
                dCariBilanco.yazilim = dKayitliDegerGecmisYil.yazilim - dBirikmisAmortismanGecmisYil.yazilim;



                //dKayitliDegerCariYil.gayrimenkul = dKayitliDegerGecmisYil.gayrimenkul + dCariAlimlar.gayrimenkul + cariIhracAmortismanGayrimenkul;
                //dKayitliDegerCariYil.demirbas = dKayitliDegerGecmisYil.demirbas + dCariAlimlar.demirbas;
                //dKayitliDegerCariYil.yazilim = dKayitliDegerGecmisYil.yazilim + dCariAlimlar.yazilim;



                //dKayitliDegerCariYil.gayrimenkul = dKayitliDegerGecmisYil.gayrimenkul + dCariAlimlar.gayrimenkul;
                //dKayitliDegerCariYil.demirbas = dKayitliDegerGecmisYil.demirbas + dCariAlimlar.demirbas;
                //dKayitliDegerCariYil.yazilim = dKayitliDegerGecmisYil.yazilim + dCariAlimlar.yazilim;



                //dBirikmisAmortismanCariYil.gayrimenkul = dBirikmisAmortismanGecmisYil.gayrimenkul + dCariAmortisman.gayrimenkul;
                //dBirikmisAmortismanCariYil.demirbas = dBirikmisAmortismanGecmisYil.demirbas + dCariAmortisman.demirbas;
                //dBirikmisAmortismanCariYil.yazilim = dBirikmisAmortismanGecmisYil.yazilim + dCariAmortisman.yazilim;


                //dCariBilanco.gayrimenkul = dKayitliDegerGecmisYil.gayrimenkul - dBirikmisAmortismanGecmisYil.gayrimenkul;
                //dCariBilanco.demirbas = dKayitliDegerGecmisYil.demirbas - dBirikmisAmortismanGecmisYil.demirbas;
                //dCariBilanco.yazilim = dKayitliDegerGecmisYil.yazilim - dBirikmisAmortismanGecmisYil.yazilim;



                //dKayitliDegerCariYil.gayrimenkul = dCariBilanco.gayrimenkul + dCariAlimlar.gayrimenkul - dCariEldenCikanlar.gayrimenkul - dCariAmortisman.gayrimenkul;
                //dKayitliDegerCariYil.demirbas = dCariBilanco.demirbas + dCariAlimlar.demirbas - dCariEldenCikanlar.demirbas - dCariAmortisman.demirbas;
                //dKayitliDegerCariYil.yazilim = dCariBilanco.yazilim + dCariAlimlar.yazilim - dCariEldenCikanlar.yazilim - dCariAmortisman.yazilim;



                dCariEldenCikanlar.gayrimenkul *= -1;
                dCariEldenCikanlar.demirbas *= -1;
                dCariEldenCikanlar.yazilim *= -1;

                dCariAmortisman.gayrimenkul *= -1;
                dCariAmortisman.demirbas *= -1;
                dCariAmortisman.yazilim *= -1;

                dBirikmisAmortismanCariYil.gayrimenkul *= -1;
                dBirikmisAmortismanCariYil.demirbas *= -1;
                dBirikmisAmortismanCariYil.yazilim *= -1;

                dBirikmisAmortismanGecmisYil.gayrimenkul *= -1;
                dBirikmisAmortismanGecmisYil.demirbas *= -1;
                dBirikmisAmortismanGecmisYil.yazilim *= -1;


            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            if (dr != null)
                dr.Close();
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static ObjectArray MuhasebeKarsilastirmaRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            string hata = "";

            ObjectArray listeVAN = new ObjectArray();

            try
            {
                string ambarKodlari = string.IsNullOrWhiteSpace(ak.ambarKod) ? "01;51;50" : ak.ambarKod;


                var bakiyeKarsilastirmaDemirbasSistemi = new MuhasebeBakiyeKarsilastirmaMB();

                foreach (var ambarKod in ambarKodlari.Split(';'))
                {
                    ak.ambarKod = ambarKod;

                    decimal gToplamSonBedel = 0;
                    decimal gToplamIlkBedel = 0;
                    decimal gToplamBedDuzFark = 0;
                    decimal gToplamEnfDuzFark = 0;
                    decimal gToplamBirAmortisman = 0;
                    decimal gToplamBirAmoFark = 0;
                    decimal gBirikenAmortisman = 0;
                    decimal gDuzelmisBiriken = 0;

                    ObjectArray listeAmortisman = MalzemeAmortismanRaporu(null, kullanan, ak);

                    foreach (AmortismanRapor ar in listeAmortisman.objeler)
                    {
                        gToplamSonBedel += OrtakFonksiyonlar.KurusYap(ar.maliyetTutar + ar.degerArtisTutar);
                        gToplamIlkBedel += OrtakFonksiyonlar.KurusYap(ar.maliyetTutar);
                        gToplamBedDuzFark += OrtakFonksiyonlar.KurusYap(ar.degerArtisTutar - ar.enflasyonDegerArtisTutar);
                        gToplamEnfDuzFark += OrtakFonksiyonlar.KurusYap(ar.enflasyonDegerArtisTutar);
                        gToplamBirAmortisman += OrtakFonksiyonlar.KurusYap(ar.maliyetAmortismanBirikmisTutar);
                        gToplamBirAmoFark += OrtakFonksiyonlar.KurusYap(ar.degerArtisAmortismanTutar + ar.enflasyonDuzAmortismanTutar);
                        gBirikenAmortisman += OrtakFonksiyonlar.KurusYap(ar.maliyetAmortismanBirikmisTutar);
                        gDuzelmisBiriken += OrtakFonksiyonlar.KurusYap(ar.maliyetAmortismanBirikmisTutar + ar.degerArtisAmortismanTutar + ar.enflasyonDuzAmortismanTutar);
                    }

                    if (ambarKod == "01")
                    {
                        bakiyeKarsilastirmaDemirbasSistemi.demirbas.bedel = gToplamIlkBedel + gToplamBedDuzFark;
                        bakiyeKarsilastirmaDemirbasSistemi.demirbas.enflasyonDuzeltmeFarki = gToplamEnfDuzFark;
                        bakiyeKarsilastirmaDemirbasSistemi.demirbas.sonBedeli = gToplamSonBedel;
                        bakiyeKarsilastirmaDemirbasSistemi.demirbas.amortisman = gToplamBirAmortisman;
                        bakiyeKarsilastirmaDemirbasSistemi.demirbas.birikmisAmortismanFarki = gToplamBirAmoFark;
                        bakiyeKarsilastirmaDemirbasSistemi.demirbas.duzelmisBirikenAmortisman = gDuzelmisBiriken;
                    }
                    else if (ambarKod == "51")
                    {
                        bakiyeKarsilastirmaDemirbasSistemi.yazilim.bedel = gToplamIlkBedel + gToplamBedDuzFark;
                        bakiyeKarsilastirmaDemirbasSistemi.yazilim.enflasyonDuzeltmeFarki = gToplamEnfDuzFark;
                        bakiyeKarsilastirmaDemirbasSistemi.yazilim.sonBedeli = gToplamSonBedel;
                        bakiyeKarsilastirmaDemirbasSistemi.yazilim.amortisman = gToplamBirAmortisman;
                        bakiyeKarsilastirmaDemirbasSistemi.yazilim.birikmisAmortismanFarki = gToplamBirAmoFark;
                        bakiyeKarsilastirmaDemirbasSistemi.yazilim.duzelmisBirikenAmortisman = gDuzelmisBiriken;
                    }
                    else if (ambarKod == "50")
                    {
                        bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.bedel = gToplamIlkBedel + gToplamBedDuzFark;
                        bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.enflasyonDuzeltmeFarki = gToplamEnfDuzFark;
                        bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.sonBedeli = gToplamSonBedel;
                        bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.amortisman = gToplamBirAmortisman;
                        bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.birikmisAmortismanFarki = gToplamBirAmoFark;
                        bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.duzelmisBirikenAmortisman = gDuzelmisBiriken;
                    }

                }


                //Merkez Bankası bakiye bilgileri
                var bakiyeKarsilastirmaMerkezBankasi = new MuhasebeBakiyeKarsilastirmaMB();

                foreach (var ambarKod in ambarKodlari.Split(';'))
                {
                    var merkezBankasiMuhasebeSorgu = TMMMuhasebeBilgi.DemirbasHesapToplamBakiyeSorgula(ambarKod);
                    if (!merkezBankasiMuhasebeSorgu.durum)
                        hata += merkezBankasiMuhasebeSorgu.mesaj;

                    if (ambarKod == "01")
                    {
                        bakiyeKarsilastirmaMerkezBankasi.demirbas.bedel = merkezBankasiMuhasebeSorgu.BEDEL;
                        bakiyeKarsilastirmaMerkezBankasi.demirbas.enflasyonDuzeltmeFarki = merkezBankasiMuhasebeSorgu.BEDEL_ENFLASYON_DUZELTME;
                        bakiyeKarsilastirmaMerkezBankasi.demirbas.sonBedeli = merkezBankasiMuhasebeSorgu.BEDEL + merkezBankasiMuhasebeSorgu.BEDEL_ENFLASYON_DUZELTME;
                        bakiyeKarsilastirmaMerkezBankasi.demirbas.amortisman = merkezBankasiMuhasebeSorgu.AMORTISMAN;
                        bakiyeKarsilastirmaMerkezBankasi.demirbas.birikmisAmortismanFarki = merkezBankasiMuhasebeSorgu.AMORTISMAN_ENFLASYON_DUZELTME;
                        bakiyeKarsilastirmaMerkezBankasi.demirbas.duzelmisBirikenAmortisman = merkezBankasiMuhasebeSorgu.AMORTISMAN + merkezBankasiMuhasebeSorgu.AMORTISMAN_ENFLASYON_DUZELTME;
                    }
                    else if (ambarKod == "51")
                    {
                        bakiyeKarsilastirmaMerkezBankasi.yazilim.bedel = merkezBankasiMuhasebeSorgu.BEDEL;
                        bakiyeKarsilastirmaMerkezBankasi.yazilim.enflasyonDuzeltmeFarki = merkezBankasiMuhasebeSorgu.BEDEL_ENFLASYON_DUZELTME;
                        bakiyeKarsilastirmaMerkezBankasi.yazilim.sonBedeli = merkezBankasiMuhasebeSorgu.BEDEL + merkezBankasiMuhasebeSorgu.BEDEL_ENFLASYON_DUZELTME;
                        bakiyeKarsilastirmaMerkezBankasi.yazilim.amortisman = merkezBankasiMuhasebeSorgu.AMORTISMAN;
                        bakiyeKarsilastirmaMerkezBankasi.yazilim.birikmisAmortismanFarki = merkezBankasiMuhasebeSorgu.AMORTISMAN_ENFLASYON_DUZELTME;
                        bakiyeKarsilastirmaMerkezBankasi.yazilim.duzelmisBirikenAmortisman = merkezBankasiMuhasebeSorgu.AMORTISMAN + merkezBankasiMuhasebeSorgu.AMORTISMAN_ENFLASYON_DUZELTME;
                    }
                    else if (ambarKod == "50")
                    {
                        bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.bedel = merkezBankasiMuhasebeSorgu.BEDEL;
                        bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.enflasyonDuzeltmeFarki = merkezBankasiMuhasebeSorgu.BEDEL_ENFLASYON_DUZELTME;
                        bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.sonBedeli = merkezBankasiMuhasebeSorgu.BEDEL + merkezBankasiMuhasebeSorgu.BEDEL_ENFLASYON_DUZELTME;
                        bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.amortisman = merkezBankasiMuhasebeSorgu.AMORTISMAN;
                        bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.birikmisAmortismanFarki = merkezBankasiMuhasebeSorgu.AMORTISMAN_ENFLASYON_DUZELTME;
                        bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.duzelmisBirikenAmortisman = merkezBankasiMuhasebeSorgu.AMORTISMAN + merkezBankasiMuhasebeSorgu.AMORTISMAN_ENFLASYON_DUZELTME;
                    }
                }
                //********************************************************************************************

                //Merkez Bankası bakiye bilgileri
                var bakiyeKarsilastirmaFark = new MuhasebeBakiyeKarsilastirmaMB();
                bakiyeKarsilastirmaFark.demirbas.bedel = bakiyeKarsilastirmaDemirbasSistemi.demirbas.bedel - bakiyeKarsilastirmaMerkezBankasi.demirbas.bedel;
                bakiyeKarsilastirmaFark.demirbas.enflasyonDuzeltmeFarki = bakiyeKarsilastirmaDemirbasSistemi.demirbas.enflasyonDuzeltmeFarki - bakiyeKarsilastirmaMerkezBankasi.demirbas.enflasyonDuzeltmeFarki;
                bakiyeKarsilastirmaFark.demirbas.sonBedeli = bakiyeKarsilastirmaDemirbasSistemi.demirbas.sonBedeli - bakiyeKarsilastirmaMerkezBankasi.demirbas.sonBedeli;
                bakiyeKarsilastirmaFark.demirbas.amortisman = bakiyeKarsilastirmaDemirbasSistemi.demirbas.amortisman - bakiyeKarsilastirmaMerkezBankasi.demirbas.amortisman;
                bakiyeKarsilastirmaFark.demirbas.birikmisAmortismanFarki = bakiyeKarsilastirmaDemirbasSistemi.demirbas.birikmisAmortismanFarki - bakiyeKarsilastirmaMerkezBankasi.demirbas.birikmisAmortismanFarki;
                bakiyeKarsilastirmaFark.demirbas.duzelmisBirikenAmortisman = bakiyeKarsilastirmaDemirbasSistemi.demirbas.duzelmisBirikenAmortisman - bakiyeKarsilastirmaMerkezBankasi.demirbas.duzelmisBirikenAmortisman;

                bakiyeKarsilastirmaFark.yazilim.bedel = bakiyeKarsilastirmaDemirbasSistemi.yazilim.bedel - bakiyeKarsilastirmaMerkezBankasi.yazilim.bedel;
                bakiyeKarsilastirmaFark.yazilim.enflasyonDuzeltmeFarki = bakiyeKarsilastirmaDemirbasSistemi.yazilim.enflasyonDuzeltmeFarki - bakiyeKarsilastirmaMerkezBankasi.yazilim.enflasyonDuzeltmeFarki;
                bakiyeKarsilastirmaFark.yazilim.sonBedeli = bakiyeKarsilastirmaDemirbasSistemi.yazilim.sonBedeli - bakiyeKarsilastirmaMerkezBankasi.yazilim.sonBedeli;
                bakiyeKarsilastirmaFark.yazilim.amortisman = bakiyeKarsilastirmaDemirbasSistemi.yazilim.amortisman - bakiyeKarsilastirmaMerkezBankasi.yazilim.amortisman;
                bakiyeKarsilastirmaFark.yazilim.birikmisAmortismanFarki = bakiyeKarsilastirmaDemirbasSistemi.yazilim.birikmisAmortismanFarki - bakiyeKarsilastirmaMerkezBankasi.yazilim.birikmisAmortismanFarki;
                bakiyeKarsilastirmaFark.yazilim.duzelmisBirikenAmortisman = bakiyeKarsilastirmaDemirbasSistemi.yazilim.duzelmisBirikenAmortisman - bakiyeKarsilastirmaMerkezBankasi.yazilim.duzelmisBirikenAmortisman;

                bakiyeKarsilastirmaFark.gayrimenkul.bedel = bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.bedel - bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.bedel;
                bakiyeKarsilastirmaFark.gayrimenkul.enflasyonDuzeltmeFarki = bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.enflasyonDuzeltmeFarki - bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.enflasyonDuzeltmeFarki;
                bakiyeKarsilastirmaFark.gayrimenkul.sonBedeli = bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.sonBedeli - bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.sonBedeli;
                bakiyeKarsilastirmaFark.gayrimenkul.amortisman = bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.amortisman - bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.amortisman;
                bakiyeKarsilastirmaFark.gayrimenkul.birikmisAmortismanFarki = bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.birikmisAmortismanFarki - bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.birikmisAmortismanFarki;
                bakiyeKarsilastirmaFark.gayrimenkul.duzelmisBirikenAmortisman = bakiyeKarsilastirmaDemirbasSistemi.gayrimenkul.duzelmisBirikenAmortisman - bakiyeKarsilastirmaMerkezBankasi.gayrimenkul.duzelmisBirikenAmortisman;
                //********************************************************************************************


                listeVAN.Ekle(bakiyeKarsilastirmaDemirbasSistemi);
                listeVAN.Ekle(bakiyeKarsilastirmaMerkezBankasi);
                listeVAN.Ekle(bakiyeKarsilastirmaFark);
            }
            catch (Exception ex)
            {
                hata += ex.Message;
            }

            if (!string.IsNullOrWhiteSpace(hata))
                listeVAN.sonuc.DegerAta(hata);

            return listeVAN;
        }


        public static AmortismanBilgi AmortismanBilgileri(VeriErisim veriErisim, Kullanici kullanan, int prSicilNo, string muhasebeKod, string harcamaBirimKod, int amortismanDonem)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            AmortismanBilgi amb = new AmortismanBilgi();

            try
            {
                if (prSicilNo == 0)
                    return amb;

                AmortismanKriter ak = new AmortismanKriter
                {
                    yil = DateTime.Now.Year,
                    prSicilNo = prSicilNo,
                    muhasebeKod = muhasebeKod,
                    harcamaKod = harcamaBirimKod,
                    raporTur = 6,
                    amortismanSure = 100
                };

                if (amortismanDonem > 0)
                {
                    ak.donem = amortismanDonem;
                    ak.amortismanSure = 0;
                }

                ObjectArray aListe = AmortismanRaporla3(ve, kullanan, ak);

                foreach (TNS.TMM.AmortismanRapor3 ar in aListe.objeler)
                {
                    amb.ilkAmartismanYuzdesi = (double)ar.amortismanYuzdesi;
                    amb.buAmartismanYuzdesi = (double)ar.amortismanYuzdesiTamamlanan;

                    amb.ilkBedel = (double)ar.maliyetTutar;
                    amb.birikenAmortisman = (double)(ar.toplamAmortisman - ar.degerDuzeltmeToplamAmortisman); //ar.maliyetBirikmisAmortisman;//(double)ar.maliyetToplamAmortisman;
                    amb.bedelDuzeltmeFarki = (double)ar.degerDuzeltmeToplamTutar;
                    amb.amortiDuzeltmeFarki = (double)ar.degerDuzeltmeToplamAmortisman;
                    amb.sonBedel = (double)ar.toplamTutar;
                    amb.duzBirikenAmortisman = (double)ar.toplamAmortisman;
                    amb.ayrilanSonAmortisman = (double)ar.sonDonemAmortisman;
                    amb.oncekiYillarAmortisman = (double)(ar.toplamAmortisman - ar.sonDonemAmortisman);
                    amb.bulunduguYer = ar.nereyeGitti;
                    amb.bulunduguYerAd = ar.nereyeGittiAd;
                    amb.sonAmortismanYil = ar.sonAmortismanYil;
                    amb.sonAmortismanDonem = ar.sonAmortismanDonem;
                }
            }
            catch (Exception ex)
            {

            }

            ve.VeriErisimKapat();

            return amb;
        }

        private static ObjectArray EnflasyonDuzeltmesiAmortismanRaporu(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak, bool raporMu)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.sureAsimi = 60 * 2;
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                if (ak.raporTur == 0)
                    ak.raporTur = (int)ENUMMBRaporTur.ENFLASYONDUZELTMESI;

                IDataReader dr = tAmortisman.EnflasyonDuzeltmesiAmortismanRaporu2025(ak, raporMu);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                string plaka = "";
                string adi = "";
                string yeriKonusu = "";
                string saseNo = "";
                string ekNo = "";
                string giai = "";

                while (dr.Read())
                {
                    int siraNo = 0;

                    AmortismanRapor ar = new AmortismanRapor();
                    ar.prSicilNo = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.gorSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.amortismanSuresi = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.girisTarih = VeriErisim.DegerAl(dr, siraNo++);
                    ar.disSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.disSicilNo2 = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.bisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.eskiBisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.markaAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.modelAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGitti = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGittiAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.harcamaBirimiKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.harcamaBirimiAdi = VeriErisim.DegerAl(dr, siraNo++, "");
                    string ambarAd = VeriErisim.DegerAl(dr, siraNo++, "");

                    if (string.IsNullOrWhiteSpace(ar.nereyeGittiAd))
                        ar.nereyeGittiAd = ambarAd;
                    ar.nereyeGittiAd = ar.harcamaBirimiKod + " - " + ar.nereyeGittiAd;

                    decimal maliyetTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal degerArtisTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal enflasyonDuzeltmeTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal amortismanToplam = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal enflasyonDuzeltmeAmortismanTutar = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    decimal enflasyonDuzeltmeCari = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal enflasyonDuzeltmeAmortismanCari = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal enfKatsayi = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);


                    ar.amortismanBaslamaYil = ar.girisTarih.Yil;

                    ar.edTutar = maliyetTutar + degerArtisTutar;
                    ar.edTutarEnflasyonDuzeltmesi = enflasyonDuzeltmeTutar;
                    ar.edAmortisman = amortismanToplam;
                    ar.edAmortismanEnflasyonDuzeltmesi = enflasyonDuzeltmeAmortismanTutar;

                    ar.edTutarEnflasyonDuzeltmesiSon = enflasyonDuzeltmeCari;
                    ar.edAmortismanEnflasyonDuzeltmesiSon = enflasyonDuzeltmeAmortismanCari;

                    ar.edTutarEnflasyonDuzeltmesi -= enflasyonDuzeltmeCari;
                    ar.edAmortismanEnflasyonDuzeltmesi -= enflasyonDuzeltmeAmortismanCari;

                    ar.enfKatsayisi = enfKatsayi;

                    plaka = VeriErisim.DegerAl(dr, siraNo++, "");
                    adi = VeriErisim.DegerAl(dr, siraNo++, "");
                    yeriKonusu = VeriErisim.DegerAl(dr, siraNo++, "");
                    saseNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ekNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    giai = VeriErisim.DegerAl(dr, siraNo++, "");

                    ar.islemTip.kod = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.islemTip.tur = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.islemTip.ad = VeriErisim.DegerAl(dr, siraNo++, "");

                    ar.ambarKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.kdvOran = VeriErisim.DegerAl(dr, siraNo++, 0);

                    string ozellik = "";

                    if (!String.IsNullOrEmpty(ar.markaAd))
                        ozellik = ar.markaAd;
                    if (!String.IsNullOrEmpty(ar.modelAd))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += ar.modelAd;
                    }
                    if (!String.IsNullOrEmpty(plaka))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += plaka;
                    }
                    if (!String.IsNullOrEmpty(adi))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += adi;
                    }
                    if (!String.IsNullOrEmpty(yeriKonusu))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += yeriKonusu;
                    }
                    if (!String.IsNullOrEmpty(saseNo))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += saseNo;
                    }
                    if (!String.IsNullOrEmpty(ekNo))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += ekNo;
                    }
                    if (String.IsNullOrEmpty(saseNo) && !String.IsNullOrEmpty(giai))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += giai;
                    }

                    ar.ozellik = ozellik;
                    if (EklensenMi(ak, ar))
                        listeVAN.Ekle(ar);
                }
                dr.Close();

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        private static bool EklensenMi(AmortismanKriter ak, AmortismanRapor ar)
        {
            return true;
        }

        private static ObjectArray EnflasyonDuzeltmesiAmortismanRaporuMB(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            ObjectArray listeVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.sureAsimi = 60 * 10;
            try
            {
                string hata = "";

                TABAmortisman tAmortisman = new TABAmortisman(ve);

                if (ak.raporTur == 0)
                    ak.raporTur = (int)ENUMMBRaporTur.ENFLASYONDUZELTMESI;

                IDataReader dr = tAmortisman.EnflasyonDuzeltmesiAmortismanRaporuMB(ak);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    listeVAN.sonuc.DegerAta(ve.hata);
                    return listeVAN;
                }

                string plaka = "";
                string adi = "";
                string yeriKonusu = "";
                string saseNo = "";
                string ekNo = "";
                string giai = "";

                while (dr.Read())
                {
                    int siraNo = 0;

                    AmortismanRapor ar = new AmortismanRapor();
                    ar.prSicilNo = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.gorSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.amortismanSuresi = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.girisTarih = VeriErisim.DegerAl(dr, siraNo++);
                    ar.disSicilNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.disSicilNo2 = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.bisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.eskiBisNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.hesapPlanAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.markaAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.modelAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGitti = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.nereyeGittiAd = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.harcamaBirimiKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.harcamaBirimiAdi = VeriErisim.DegerAl(dr, siraNo++, "");
                    string ambarAd = VeriErisim.DegerAl(dr, siraNo++, "");

                    if (string.IsNullOrWhiteSpace(ar.nereyeGittiAd))
                        ar.nereyeGittiAd = ambarAd;
                    ar.nereyeGittiAd = ar.harcamaBirimiKod + " - " + ar.nereyeGittiAd;


                    decimal katsayi = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal ilkBedel = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal bedelDuzletme = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal kiymetBedeliCariEdf = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal kiymetBedeliToplamEdf = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);

                    decimal amortisman = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal amortismanCariEdf = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);
                    decimal amortismanCariToplamEdf = VeriErisim.DegerAl(dr, siraNo++, (decimal)0.0);


                    ar.enfKatsayisi = katsayi;
                    ar.kiymetBedeli = ilkBedel + bedelDuzletme;
                    ar.kiymetBedeliCariEdf = kiymetBedeliCariEdf;
                    ar.kiymetBedeliBirikmisEdf = kiymetBedeliToplamEdf - ar.kiymetBedeliCariEdf;

                    ar.amortisman = amortisman;
                    ar.amortismanCariEdf = amortismanCariEdf;
                    ar.amortismanBirikmisEdf = amortismanCariToplamEdf - ar.amortismanCariEdf;


                    //Enfaslyon Hesaplamasında Kullanıyor

                    ar.edTutar = ilkBedel + bedelDuzletme;
                    ar.edTutarEnflasyonDuzeltmesi = kiymetBedeliToplamEdf;
                    ar.edTutarEnflasyonDuzeltmesiSon = kiymetBedeliCariEdf;
                    ar.edAmortisman = amortisman;
                    ar.edAmortismanEnflasyonDuzeltmesi = amortismanCariToplamEdf;
                    ar.edAmortismanEnflasyonDuzeltmesiSon = amortismanCariEdf;

                    //--------------------------------------


                    ar.amortismanBaslamaYil = ar.girisTarih.Yil;


                    plaka = VeriErisim.DegerAl(dr, siraNo++, "");
                    adi = VeriErisim.DegerAl(dr, siraNo++, "");
                    yeriKonusu = VeriErisim.DegerAl(dr, siraNo++, "");
                    saseNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    ekNo = VeriErisim.DegerAl(dr, siraNo++, "");
                    giai = VeriErisim.DegerAl(dr, siraNo++, "");

                    ar.islemTip.kod = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.islemTip.tur = VeriErisim.DegerAl(dr, siraNo++, 0);
                    ar.islemTip.ad = VeriErisim.DegerAl(dr, siraNo++, "");

                    ar.ambarKod = VeriErisim.DegerAl(dr, siraNo++, "");
                    ar.kdvOran = VeriErisim.DegerAl(dr, siraNo++, 0);

                    string ozellik = "";

                    if (!String.IsNullOrEmpty(ar.markaAd))
                        ozellik = ar.markaAd;
                    if (!String.IsNullOrEmpty(ar.modelAd))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += ar.modelAd;
                    }
                    if (!String.IsNullOrEmpty(plaka))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += plaka;
                    }
                    if (!String.IsNullOrEmpty(adi))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += adi;
                    }
                    if (!String.IsNullOrEmpty(yeriKonusu))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += yeriKonusu;
                    }
                    if (!String.IsNullOrEmpty(saseNo))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += saseNo;
                    }
                    if (!String.IsNullOrEmpty(ekNo))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += ekNo;
                    }
                    if (String.IsNullOrEmpty(saseNo) && !String.IsNullOrEmpty(giai))
                    {
                        if (ozellik != "") ozellik += "-";
                        ozellik += giai;
                    }

                    ar.ozellik = ozellik;

                    //if (ar.edTutar > ar.edAmortisman) //Eğer amortisman tamamladı ise eklemesin, onlara değer artışı yapılmayacak 21.01.2023 Melih
                    listeVAN.Ekle(ar);
                }
                dr.Close();

                if (hata != "")
                    listeVAN.sonuc.DegerAta(hata);
            }
            catch (Exception ex)
            {
                listeVAN.sonuc.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();

            return listeVAN;
        }

        public static Sonuc EnflasyonDuzeltmesiKaydetMB(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            TNSDateTime sonEDFTarihi = new TNSDateTime("31.08.2024");
            TNSDateTime belgeTarihi = new TNSDateTime("31.12.2024");

            ak.raporTur = (int)ENUMMBRaporTur.ENFLASYONDUZELTMESI;
            ak.yil = belgeTarihi.Yil;
            ak.donem = belgeTarihi.Ay * 100; //Amortismandan ayırmak için
                                             //ak.prSicilNo = 116380;

            ak.bitisTarihi = belgeTarihi; //EDF yapılacak dönem önesinde değer artış amortisman yapılmışmı kontrol için

            Sonuc sonuc = EnflasyonDuzeltmesiKontrol(veriErisim, kullanan, ak);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            AmortismanIslemForm form = new AmortismanIslemForm()
            {
                yil = ak.yil,
                donem = ak.donem,
                muhasebeKod = ak.muhasebeKod,
                harcamaKod = ak.harcamaKod,
                ambarKod = ak.ambarKod,
                tip = "ENFLASYONDUZELTMESI"
            };


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.sureAsimi = 120;
            ve.BeginTransaction();

            sonuc = TMMAmortismanIslemForm.Kaydet(ve, kullanan, form);

            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            try
            {
                int edfYil = 0;
                int edfAy = 0;
                double yiUfe = 0.0;

                ObjectArray enfDuzeltmesiAyarlar = new TMMEnflasyonDuzeltmesi().Listele(veriErisim, kullanan, new EnflasyonDuzeltmesi());
                foreach (EnflasyonDuzeltmesi edf in enfDuzeltmesiAyarlar.objeler)
                {
                    if (edf.yil > edfYil)
                    {
                        edfYil = edf.yil;
                        if (edf.ay > edfAy)
                        {
                            edfAy = edf.ay;
                            yiUfe = edf.yiUfe;
                        }
                    }
                }

                //if (edfYil * 100 + edfAy == belgeTarihi.Yil * 100 + belgeTarihi.Ay)
                //{
                //    sonuc.islemSonuc = false;
                //    sonuc.hataStr = belgeTarihi.Yil + " yılı, " + belgeTarihi.Ay + ". ay için enflasyon düzeltmesi uygulanmıştır. " + ak.harcamaKod + " - " + ak.ambarKod + " / " + sonuc.hataStr;
                //}
                //else 
                if (yiUfe <= 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = belgeTarihi.Yil + " yılı, " + belgeTarihi.Ay + ". ay için YIUFE değeri bulunamadı. " + ak.harcamaKod + " - " + ak.ambarKod + " / " + sonuc.hataStr;
                }
                else
                {
                    var fisNo = sonuc.anahtar.Split('-')[0].Trim();

                    ObjectArray liste = EnflasyonDuzeltmesiAmortismanRaporuMB(veriErisim, kullanan, ak);

                    ObjectArray bilgiler = new ObjectArray();

                    foreach (AmortismanRapor ar in liste.objeler)
                    {
                        //if ((double)ar.edTutar > islemYapilan.sinir)
                        //    continue;


                        //Mekrez bankası 4. dönem eflasyon düzeltmesinde 9. ay endeksi 8.ay gibi yapılsın istendi.
                        //21.02.2024 Mail : 9.ay için hangi endeksi teyit ettiğiniz anlayamadık. 1,023684 olan mı? yoksa 1,037671 olan mı? Cevap: Dosyada yer alan endeksi teyit ediyoruz.(1,0376705783947)
                        if (ar.girisTarih.Yil == 2024 && ar.girisTarih.Ay == 9)
                            ar.girisTarih = new TNSDateTime(new DateTime(ar.girisTarih.Yil, 8, ar.girisTarih.Gun));



                        SicilNoDegerArtis tf2 = new SicilNoDegerArtis();
                        tf2.muhasebeKod = ak.muhasebeKod;
                        tf2.harcamaKod = ak.harcamaKod;
                        tf2.belgeTarihi = belgeTarihi;
                        tf2.tur = 1; //Enflasyon Düzelmesi
                        tf2.gerekce = "Enflasyon Düzelmesi";
                        tf2.prSicilNo = ar.prSicilNo;
                        tf2.gorSicilNo = ar.gorSicilNo;
                        tf2.hesapPlanKod = ar.hesapPlanKod;
                        tf2.ambarKod = ar.ambarKod;
                        tf2.kdvOran = ar.kdvOran;
                        tf2.tifNo = fisNo;

                        tf2.eskiTutar = (double)ar.edTutarToplam;
                        tf2.katsayi = EnflasyonDuzeltmesiKatsayiHesaplaMB(sonEDFTarihi, yiUfe, ar.girisTarih.Yil, ar.girisTarih.Ay, enfDuzeltmesiAyarlar, tf2.muhasebeKod);
                        tf2.tutar = OrtakFonksiyonlar.Yuvarla(tf2.eskiTutar * tf2.katsayi - tf2.eskiTutar, 13);
                        tf2.amotismanTutar = OrtakFonksiyonlar.Yuvarla((decimal)tf2.tutar * ar.edAmortismanToplam / ar.edTutarToplam, 2);
                        //tf2.amotismanTutar = OrtakFonksiyonlar.Yuvarla((decimal)tf2.tutar * ar.edAmortismanToplam / ar.edTutarToplam, 13);

                        //var amotismanTutarTest = OrtakFonksiyonlar.Yuvarla(ar.edAmortismanToplam * (decimal)tf2.katsayi - ar.edAmortismanToplam, 13);

                        if (tf2.katsayi > 1)
                            bilgiler.Ekle(tf2);
                    }

                    sonuc = TMMSicilNoDegerArtis.KaydetEnflasyonDuzeltmesiMB(kullanan, bilgiler);


                    if (!sonuc.islemSonuc)
                        sonuc.hataStr = ak.harcamaKod + " - " + ak.ambarKod + " / " + sonuc.hataStr;
                }

            }
            catch (Exception ex)
            {
                sonuc.DegerAta("Exception: " + ex.Message);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static double EnflasyonDuzeltmesiKatsayiHesaplaMB(TNSDateTime sonEDFTarihi, double yiUfe, int yil, int ay, ObjectArray enfDuzeltmesiAyarlar, string muhasebeKod)
        {
            EnflasyonDuzeltmesi alimTarihdekiEnf = new EnflasyonDuzeltmesi();

            //int kontrolYil = 2024;
            //int kontrolAy = 0;
            double katsayi = 0.0;
            //double yiUfe = 0.0;

            //Son enflasyon düzelmesinden önceki kayıtlar için tek bir katsayı hesaplanacak. Alım tarihi son edf tarihi gibi düşünülecek.
            if (yil * 100 + ay < sonEDFTarihi.Yil * 100 + sonEDFTarihi.Ay)
            {
                foreach (EnflasyonDuzeltmesi edf in enfDuzeltmesiAyarlar.objeler)
                {
                    if (edf.yil == sonEDFTarihi.Yil && edf.ay == sonEDFTarihi.Ay)
                    {
                        alimTarihdekiEnf = edf;
                        break;
                    }
                }
            }
            else
            {
                foreach (EnflasyonDuzeltmesi enf in enfDuzeltmesiAyarlar.objeler)
                {
                    if (enf.yil == yil && enf.ay == ay)
                    {
                        alimTarihdekiEnf = enf;
                        break;
                    }
                }

            }

            if (alimTarihdekiEnf.yiUfe > 0)
                katsayi = Math.Round(yiUfe / alimTarihdekiEnf.yiUfe, 13);

            //Kontrol yılına ait en son üfe bilgisini al (işlem yapılırken Aralık ayı açıklanmamış olabilir.o zaman 2023/11. ayı kullanır en yüksek ayı arıyor
            //Yıl içinde 2024 yılında 5.ay da edf yapıldı.
            //foreach (EnflasyonDuzeltmesi edf in enfDuzeltmesiAyarlar.objeler)
            //{
            //    if (edf.yil > edfYil)
            //    {
            //        edfYil = edf.yil;
            //        if (edf.ay > edfAy)
            //        {
            //            edfAy = edf.ay;
            //            yiUfe = edf.yiUfe;
            //        }
            //    }
            //}

            //foreach (EnflasyonDuzeltmesi enf in enfDuzeltmesiAyarlar.objeler)
            //{
            //    if (enf.yil == kontrolYil && kontrolAy < enf.ay)
            //    {
            //        kontrolAy = enf.ay;
            //        yiUfe = enf.yiUfe;
            //    }
            //}

            //2005 yılı 1.ay öncesi kayıtlar için aktif yılın 12. ayındaki değer ile 2005 yılının 1. ayı oranlanarak hesaplanacak
            //if (yil < 2005)
            //{
            //    foreach (EnflasyonDuzeltmesi enf in enfDuzeltmesiAyarlar.objeler)
            //    {
            //        if (enf.yil == 2004 && enf.ay == 12)
            //        {
            //            alimTarihdekiEnf = enf;
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    //if (kontrolYil > yil) ay = 12;//Eğer alım yılı önceki yıllardan ise 12. ay baz alınacak
            //    foreach (EnflasyonDuzeltmesi enf in enfDuzeltmesiAyarlar.objeler)
            //    {
            //        if (enf.yil == yil && enf.ay == ay)
            //        {
            //            alimTarihdekiEnf = enf;
            //            break;
            //        }
            //    }
            //}

            //if (alimTarihdekiEnf.yiUfe > 0)
            //    katsayi = Math.Round(yiUfe / alimTarihdekiEnf.yiUfe, 13);

            return katsayi;
        }

        public static Sonuc EnflasyonDuzeltmesiKaydet(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            if (Arac.MerkezBankasiKullaniyor())
                return EnflasyonDuzeltmesiKaydetMB(veriErisim, kullanan, ak);

            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            ObjectArray enfDuzeltmesiAyarlar = new TMMEnflasyonDuzeltmesi().Listele(veriErisim, kullanan, new EnflasyonDuzeltmesi());

            ObjectArray liste = EnflasyonDuzeltmesiAmortismanRaporu(veriErisim, kullanan, ak, false);

            int edfYil = 0;
            int edfAy = 0;
            double yiUfe = 0.0;
            foreach (EnflasyonDuzeltmesi edf in enfDuzeltmesiAyarlar.objeler)
            {
                if (YilAyBuyukMu(edf.yil, edf.ay, edfYil, edfAy))
                {
                    edfYil = edf.yil;
                    edfAy = edf.ay;
                    yiUfe = edf.yiUfe;
                }
            }

            TNSDateTime sonEDFTarihi = new TNSDateTime("31.12." + (ak.yil - 1));
            TNSDateTime belgeTarihi = new TNSDateTime("31.12." + (ak.yil));


            ObjectArray bilgiler = new ObjectArray();

            foreach (AmortismanRapor ar in liste.objeler)
            {
                //if ((double)ar.edTutar > islemYapilan.sinir)
                //    continue;

                SicilNoDegerArtis tf2 = new SicilNoDegerArtis();
                tf2.muhasebeKod = ak.muhasebeKod;
                tf2.harcamaKod = ak.harcamaKod;
                tf2.belgeTarihi = belgeTarihi;
                tf2.tur = 1; //Enflasyon Düzelmesi
                tf2.gerekce = "";
                tf2.prSicilNo = ar.prSicilNo;
                tf2.gorSicilNo = ar.gorSicilNo;
                tf2.hesapPlanKod = ar.hesapPlanKod;
                tf2.ambarKod = ar.ambarKod;
                tf2.kdvOran = ar.kdvOran;

                tf2.eskiTutar = (double)ar.edTutarToplam;
                tf2.katsayi = EnflasyonDuzeltmesiKatsayiHesapla2025(sonEDFTarihi, yiUfe, ar.girisTarih.Yil, ar.girisTarih.Ay, enfDuzeltmesiAyarlar, tf2.muhasebeKod);
                tf2.tutar = OrtakFonksiyonlar.Yuvarla(tf2.eskiTutar * tf2.katsayi - tf2.eskiTutar, 4);
                tf2.amotismanTutar = OrtakFonksiyonlar.Yuvarla((decimal)tf2.tutar * ar.edAmortismanToplam / ar.edTutarToplam, 4);

                //tf2.eskiTutar = (double)ar.edTutar;
                //tf2.katsayi = EnflasyonDuzeltmesiKatsayiHesapla(sonEDFTarihi, ar.girisTarih.Yil, ar.girisTarih.Ay, enfDuzeltmesiAyarlar, tf2.muhasebeKod);
                //tf2.tutar = OrtakFonksiyonlar.Yuvarla(tf2.eskiTutar * tf2.katsayi - tf2.eskiTutar, 4);
                //tf2.amotismanTutar = OrtakFonksiyonlar.Yuvarla(ar.edAmortisman * (decimal)tf2.katsayi - ar.edAmortisman, 4);

                bilgiler.Ekle(tf2);
            }

            sonuc = TMMSicilNoDegerArtis.KaydetEnflasyonDuzeltmesi(kullanan, $"{ak.harcamaKod}-{ak.ambarKod}", bilgiler);
            return sonuc;
        }

        private static bool YilAyBuyukMu(int yil1, int ay1, int yil2, int ay2)
        {
            if (yil1 > yil2) return true;
            if (yil1 == yil2 && ay1 > ay2) return true;
            return false;
        }

        public static double EnflasyonDuzeltmesiKatsayiHesapla(int yil, int ay, ObjectArray enfDuzeltmesiAyarlar, string muhasebeKod)
        {
            int kontrolYil = 2023;
            int kontrolAy = 0;
            double katsayi = 0.0;
            double yiUfe = 0.0;

            //MYS 2023 yılında alınnanların 2022 yılı oranında işlem yapılmış gibi 1.43 oranı alınmasını istedi.
            if (yil == 2023) yil = 2022;

            //Kontrol yılına ait en son üfe bilgisini al (işlem yapılırken Aralık ayı açıklanmamış olabilir.o zaman 2023/11. ayı kullanır en yüksek ayı arıyor
            foreach (EnflasyonDuzeltmesi enf in enfDuzeltmesiAyarlar.objeler)
            {
                if (enf.yil == kontrolYil && kontrolAy < enf.ay)
                {
                    kontrolAy = enf.ay;
                    yiUfe = enf.yiUfe;
                }
            }

            if (kontrolYil > yil) ay = 12;//Eğer alım yılı önceki yıllardan ise 12. ay baz alınacak

            EnflasyonDuzeltmesi alimTarihdekiEnf = new EnflasyonDuzeltmesi();

            foreach (EnflasyonDuzeltmesi enf in enfDuzeltmesiAyarlar.objeler)
            {
                if (enf.yil == yil && enf.ay == ay)
                {
                    alimTarihdekiEnf = enf;
                    break;
                }
            }

            if (alimTarihdekiEnf.yiUfe > 0)
                katsayi = Math.Round(yiUfe / alimTarihdekiEnf.yiUfe, 2);

            return katsayi;
        }

        public static double EnflasyonDuzeltmesiKatsayiHesapla2025(TNSDateTime sonEDFTarihi, double yiUfe, int yil, int ay, ObjectArray enfDuzeltmesiAyarlar, string muhasebeKod)
        {
            EnflasyonDuzeltmesi alimTarihdekiEnf = new EnflasyonDuzeltmesi();

            //int kontrolYil = 2024;
            //int kontrolAy = 0;
            double katsayi = 0.0;
            //double yiUfe = 0.0;

            //Son enflasyon düzelmesinden önceki kayıtlar için tek bir katsayı hesaplanacak. Alım tarihi son edf tarihi gibi düşünülecek.
            if (yil * 100 + ay < sonEDFTarihi.Yil * 100 + sonEDFTarihi.Ay)
            {
                foreach (EnflasyonDuzeltmesi edf in enfDuzeltmesiAyarlar.objeler)
                {
                    if (edf.yil == sonEDFTarihi.Yil && edf.ay == sonEDFTarihi.Ay)
                    {
                        alimTarihdekiEnf = edf;
                        break;
                    }
                }
            }
            else
            {
                foreach (EnflasyonDuzeltmesi enf in enfDuzeltmesiAyarlar.objeler)
                {
                    if (enf.yil == yil && enf.ay == ay)
                    {
                        alimTarihdekiEnf = enf;
                        break;
                    }
                }
            }

            if (alimTarihdekiEnf.yiUfe > 0)
                katsayi = Math.Round(yiUfe / alimTarihdekiEnf.yiUfe, 13);

            return katsayi;
        }

        public static Sonuc EnflasyonDuzeltmesiTifKaydet(VeriErisim ve, Kullanici kullanan, AmortismanKriter ak)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            int islemTipiKod = 0;
            IslemTip it = new IslemTip { tur = (int)ENUMIslemTipi.ENFLASYONARTISI };
            ObjectArray iListe = new TMMIslemTip().Listele(null, kullanan, it);
            foreach (IslemTip itt in iListe.objeler)
            {
                if (itt.tur == it.tur)
                {
                    islemTipiKod = itt.kod;
                    break;
                }
            }

            if (islemTipiKod == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Enflasyon Artışı İşlem tipi tanımlı değil";

                ve.VeriErisimKapat();
                return sonuc;
            }

            SicilNoDegerArtis t = new SicilNoDegerArtis();
            t.muhasebeKod = ak.muhasebeKod;
            t.harcamaKod = ak.harcamaKod;
            t.tur = 1; //Enflasyon Düzelmesi
            t.sorguBelgeBasTarihi = new TNSDateTime("31.12." + ak.yil);
            t.sorguBelgeBitTarihi = new TNSDateTime(DateTime.Now.Date);
            t.durum = (int)ENUMBelgeDurumu.ONAYLI;

            ObjectArray liste = TMMSicilNoDegerArtis.Listele(ve, kullanan, t);
            Hashtable table = new Hashtable();

            List<string> harcamaBirimListe = new List<string>();
            foreach (SicilNoDegerArtis form in liste.objeler)
            {
                string hbAmbar = form.harcamaKod + "-" + form.ambarKod;
                if (!harcamaBirimListe.Contains(hbAmbar))
                {
                    harcamaBirimListe.Add(hbAmbar);
                    table[hbAmbar] = new ObjectArray();
                }
            }

            foreach (var hb in harcamaBirimListe)
            {
                foreach (SicilNoDegerArtis form in liste.objeler)
                {
                    if (!string.IsNullOrWhiteSpace(form.tifNo))
                        continue;

                    string[] hbler = hb.Split('-');

                    if (hbler[0] == form.harcamaKod && hbler[1] == form.ambarKod)
                    {
                        ((ObjectArray)table[hb]).Ekle(form);
                    }
                }
            }

            foreach (var hb in harcamaBirimListe)
            {
                ObjectArray degerArtisListe = (ObjectArray)table[hb];

                string[] hbler = hb.Split('-');

                foreach (SicilNoDegerArtis da in degerArtisListe.objeler)
                {
                    TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

                    tf.yil = 0;
                    tf.muhasebeKod = t.muhasebeKod.Replace(".", "");
                    tf.harcamaKod = hbler[0].Replace(".", "");
                    tf.ambarKod = hbler[1];

                    ObjectArray prSicilNolar = TMM.IsKurallari.Genel.SicilNoluMalzemeAmbardami(ve, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod, da.prSicilNo.ToString());
                    if (prSicilNolar.ObjeSayisi == 0)
                    {
                        sonuc.hataStr = "Listede olan " + da.gorSicilNo + " sicil nolu malzeme zimmette bulunmaktadır. Lütfen kontrol edip tekrar deneyin";
                        sonuc.islemSonuc = false;

                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }
            }

            int kayitEdilen = 0;
            foreach (var hb in harcamaBirimListe)
            {
                TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

                string[] hbler = hb.Split('-');

                tf.yil = 0;
                tf.muhasebeKod = t.muhasebeKod.Replace(".", "");
                tf.harcamaKod = hbler[0].Replace(".", "");
                tf.ambarKod = hbler[1];

                tf.fisNo = "";
                tf.islemTipTur = (int)ENUMIslemTipi.ENFLASYONARTISI;
                tf.islemTipKod = islemTipiKod;

                tf.detay = new ObjectArray();

                ObjectArray degerArtisListe = (ObjectArray)table[hb];

                int siraNo = 1;
                foreach (SicilNoDegerArtis da in degerArtisListe.objeler)
                {
                    if (tf.yil == 0)
                    {
                        tf.yil = da.belgeTarihi.Yil;
                        tf.fisTarih = da.belgeTarihi;
                    }

                    var td = new TasinirIslemDetay();
                    td.yil = tf.yil;
                    td.muhasebeKod = tf.muhasebeKod;
                    td.harcamaKod = tf.harcamaKod;
                    td.kdvOran = da.kdvOran;

                    td.hesapPlanKod = da.hesapPlanKod;
                    td.siraNo = siraNo++;
                    td.gorSicilNo = da.gorSicilNo;
                    td.sicilNo = da.prSicilNo;
                    td.miktar = 1;
                    td.birimFiyat = Math.Round((decimal)da.eskiTutar / (1 + (((decimal)da.kdvOran) / 100)), 2);
                    //td.birimFiyatKDVLi = Math.Round((decimal)da.eskiTutar, 2);
                    td.satisBedeli = Math.Round((decimal)da.tutar, 2);

                    if (td.satisBedeli == 0) continue;//Eğer artış olmayacaksa tif üretilmesin

                    tf.detay.objeler.Add(td);
                }

                tf.aciklama = "Enflasyon artışı işlemi için oluşturulmuştur";

                if (tf.detay.objeler.Count == 0)
                {
                    continue;
                }
                
                sonuc = TMMTasinirIslemForm.Kaydet(ve, kullanan, tf);

                if (sonuc.islemSonuc)
                {
                    kayitEdilen++;

                    string belgeNo = sonuc.anahtar.Split('-')[0];

                    sonuc = TMMTasinirIslemForm.BelgeDurumDegistir(ve, kullanan, tf, "Onay");

                    if (sonuc.islemSonuc)
                    {
                        foreach (SicilNoDegerArtis da in degerArtisListe.objeler)
                            da.tifNo = belgeNo;

                        Sonuc sonucDegerArtis = TMMSicilNoDegerArtis.GuncelleTifNo(kullanan, degerArtisListe);
                        if (!sonucDegerArtis.islemSonuc)
                            sonuc.DegerAta(sonucDegerArtis.hataStr);
                    }
                    else
                        break;
                }
                else
                    break;

            }

            if (kayitEdilen == 0)
            {
                sonuc.hataStr = "Kayıt yapılacak bilgi yok";
                sonuc.islemSonuc = false;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }


        private static Sonuc EnflasyonDuzeltmesiKontrol(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        {
            if (string.IsNullOrWhiteSpace(ak.muhasebeKod) || string.IsNullOrWhiteSpace(ak.harcamaKod) || string.IsNullOrWhiteSpace(ak.ambarKod))
                return new Sonuc("Lütfen efnlasyon düzeltmesi kayıtları üretilecek muhasebe,birim ve ambar bilgilerini seçiniz!");

            bool islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
            if (!islemSonuc)
                islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiKullaniciBirimi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);

            if (!islemSonuc)
                return new Sonuc(TasinirMalResI.TMM050);


            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAmortismanIslemForm tabAmortismanIslemForm = new TABAmortismanIslemForm(ve);

            AmortismanIslemForm form = new AmortismanIslemForm()
            {
                yil = ak.yil,
                donem = ak.donem,
                muhasebeKod = ak.muhasebeKod,
                harcamaKod = ak.harcamaKod,
                ambarKod = ak.ambarKod
            };

            int durum = tabAmortismanIslemForm.DurumGetirTekKolon(form);

            if (durum == (int)ENUMAmortismanFormDurumu.ONAYLI)
                sonuc.hataStr += (form.harcamaKod + "-" + form.ambarKod + " için " + form.yil + "/" + form.donem + " dönemine ait enflasyon kayıtları onaylandığı için işlem yapılamaz!");
            else if (durum == (int)ENUMAmortismanFormDurumu.YENI && ak.raporTur == (int)ENUMMBRaporTur.ENFLASYONDUZELTMESI)
                sonuc.hataStr += (form.harcamaKod + "-" + form.ambarKod + " için " + form.yil + "/" + form.donem / 100 + " dönemine ait enflasyon kayıtları üretildiği için işlem yapılamaz!");
            else if (durum == 0)
                form.durum = (int)ENUMBelgeDurumu.YENI;
            else
                form.durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;

            if (Arac.MerkezBankasiKullaniyor())
            {
                //B-A Onay Kontrolü

                ENUMTasinirIslemFormOnayDurumu onayDurum = (ENUMTasinirIslemFormOnayDurumu)tabAmortismanIslemForm.OnayDurumGetirTekKolon(form);

                string belgeNo = "";
                if (form.fisNo != "")
                    belgeNo = form.fisNo + " no\'lu belge ";

                if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB)
                    sonuc.hataStr += string.Format("{0} B Onayınını beklediği için değişiklik yapılamaz.", belgeNo) + "<br>";
                else if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)
                    sonuc.hataStr += string.Format("{0} A Onayını beklediği için değişiklik yapılamaz.", belgeNo) + "<br>";

            }

            sonuc.islemSonuc = (sonuc.hataStr == "");

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

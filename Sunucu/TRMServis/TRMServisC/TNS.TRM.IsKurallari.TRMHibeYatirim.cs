using System.Data;
using TNS.TRM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using System;
using System.Collections;
using System.Collections.Generic;
using TRMServisC.tr.gov.tarim.cezali;
using System.IO;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeYatirim
    {
        /// <summary>
        /// Arguman olarak verilen, hibe Yatirim bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            int aktifEtapNo = 0;

            int etapNo = -1;
            if (bilgi.eskiBasvuru)
                etapNo = bilgi.etapNo;

            if (!bilgi.online)
            {
                if (!Yetki.YetkiVarMiYazmaya(kullanan, bilgi.ilKod, etapNo, sonuc, string.Empty, ENUMHibeTur.EKONOMIKYATIRIM))
                {
                    return sonuc;
                }
            }
            else
            {
                kullanan.kullaniciKodu = kullanan.mernis;
            }

            if (bilgi.adres.IndexOf("Hata!") > -1)
            {
                sonuc.hataStr = "Adres bilgisi hatalı. Lütfen Nüfus ve Vatandaşlık Müdürlüğünden adres bilginizi kayıt ettirin.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);

            if (bilgi.online && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))
            {
                string hata = TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, bilgi.basvuruNo);
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.hataStr = "Hibe başvurusu online girişe kapatıldığı için işlem yapılamaz.";
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }


            //Konu-Nitelik Kısıt kontrolü
            string knHata = "";
            YatirimKonuNitelikKisit knKriter = new YatirimKonuNitelikKisit();
            knKriter.etapNo = bilgi.etapNo;
            knKriter.ilKod = bilgi.ilKod;
            knKriter.konuKod = bilgi.projeKonu;
            ObjectArray knListe = new TRMYatirimKonuNitelikKisit().Listele(kullanan, knKriter);
            foreach (YatirimKonuNitelikKisit item in knListe.objeler)
            {
                if (item.ilceKod == "") item.ilceKod = bilgi.ilceKod;
                if (item.altKonuKod == "") item.altKonuKod = bilgi.projeAltKonu;
                if (item.nitelikKod == 0) item.nitelikKod = bilgi.projeNitelik;

                if (item.ilceKod == bilgi.ilceKod && item.altKonuKod == bilgi.projeAltKonu && item.nitelikKod == bilgi.projeNitelik)
                {
                    knHata = $"<b>{item.ilAdi}</b> ilinde";
                    if (item.ilceAdi != "")
                        knHata += $", <b>{item.ilceAdi}</b> ilçesinde";
                    if (item.konuAdi != "")
                        knHata += $", <b>{item.konuAdi}</b> konusunda";
                    if (item.altKonuAdi != "")
                        knHata += $", <b>{item.altKonuAdi}</b> alt konusunda";
                    if (item.nitelikAdi != "")
                        knHata += $", <b>3{item.nitelikAdi}</b> niteliğinde";

                    knHata += " kısıtlama olmasından dolayı kaydetme işlemi yapılamadı";
                    break;
                }

            }
            if (!string.IsNullOrWhiteSpace(knHata))
            {
                sonuc.hataStr = knHata;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }


            //********************************************************

            Sonuc cezaliMi = new TRMHibeCeza().CezaliMi(bilgi.kimlikNo);
            if (cezaliMi.islemSonuc)
            {
                sonuc.hataStr = cezaliMi.hataStr;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (!bilgi.eskiBasvuru)
            {
                int basvuruCesidi = 0;
                if (OrtakFonksiyonlar.ConvertToInt(bilgi.projeKonu, 0) < 51)
                    basvuruCesidi = 1;
                else
                    basvuruCesidi = 2;

                //string depremIller = "0105001,0106001,0106002,0107004,0106003,0105004,0105007,0106009,0107009,0105008,0106006";
                int depremIli = -1;// depremIller.IndexOf(bilgi.ilKod);
                bool kabulIslemiYapilmis = to.YatirimKabulYapilmismi(bilgi.etapNo, basvuruCesidi, depremIli);
                if (kabulIslemiYapilmis && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))
                {
                    sonuc.hataStr = "Başvuru kayıtlarında değişiklik yapılamaz.";
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            ve.BeginTransaction();

            if (!bilgi.eskiBasvuru)
            {
                aktifEtapNo = TRMHibeYatirimYonetim.HibeYatirimAktifEtapNo(ref ve);
                if (bilgi.kod == 0)
                {
                    bilgi.etapNo = aktifEtapNo;
                    if (bilgi.etapNo == 0)
                    {
                        sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu. Aktif Etap bilgisi okunamadı.";
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }
            }
            else
                aktifEtapNo = bilgi.etapNo;

            HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
            tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
            tarihce.kullaniciAdi = kullanan.ADSOYAD;
            tarihce.tarih = new TNSDateTime(DateTime.Now);
            tarihce.durum = bilgi.durum;
            tarihce.ipAdresi = bilgi.ipAdresi;

            //if (!bilgi.online) oNLİNE BAŞVURULARDA çift başvurular oluyor 30.12.2022 Melih
            {
                sonuc = KontrolKimlikNoKayitVarMi(ve, kullanan, bilgi.kod, bilgi.kimlikNo, aktifEtapNo);
                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            sonuc = KonuNitelikKontrol(ve, kullanan, bilgi);
            string konuNitelikUygunluk = string.Empty;
            if (!sonuc.islemSonuc)
            {
                konuNitelikUygunluk = sonuc.hataStr;
                bilgi.konuNitelikUyusma = false;

                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;

                //sonuc.islemSonuc = true;
                //sonuc.hataStr = string.Empty;
            }

            bool yeni = false;
            if (bilgi.kod == 0)
            {
                yeni = true;
                bilgi.yil = DateTime.Now.Year;
                bilgi.kod = BelgeNoAlInt(ve);
                string plaka = new TABGenel(ve).IlPlakaKodBul(bilgi.ilKod); //TRMServis.UzayYaziDegerGetir("COGRAFYA", bilgi.ilKod, true, "PLAKAKOD");
                string konuKisaAd = new TABYatirimKonu(ve).KonuKisaAdBul(bilgi.projeKonu);

                bilgi.basvuruNo = BasvuruNoAlOnline(ve, aktifEtapNo);
                if (!bilgi.eskiBasvuru)
                    bilgi.durum = (int)ENUMHibeYatirimBasvuruDurum.YENI;
                else
                    bilgi.durum = (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI;

                if (bilgi.online)
                    bilgi.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.durum = bilgi.durum;
                int cnt = KontrolKimlikNoKayitEskiEtaplardaVarMi(ref ve, kullanan, bilgi.kimlikNo, aktifEtapNo);
                if (cnt > 0)
                    bilgi.eskiEtapdaVarmi = 1;
            }
            else
            {
                HibeYatirim hm = new HibeYatirim();
                string hata = IslemKontrol(ve, kullanan, bilgi, ref hm, "BasvuruKaydet");
                if (hata != "")
                {
                    sonuc.hataStr = hata;
                    sonuc.islemSonuc = false;
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }

                //Kaldırıldı. Artık Başvuru No tamamlandıktan sonra veriliyor Melih 02.12.2020
                //string[] basvuruNo = bilgi.basvuruNo.Split('.');
                //string konuKisaAd = new TABYatirimKonu(ve).KonuKisaAdBul(bilgi.projeKonu);
                //basvuruNo[1] = bilgi.projeNitelik.ToString();
                //basvuruNo[2] = konuKisaAd;
                //bilgi.basvuruNo = basvuruNo[0] + "." + basvuruNo[1] + "." + basvuruNo[2] + "." + basvuruNo[3] + "." + basvuruNo[4];

                bilgi.durum = to.ListeleDurum(bilgi.kod);
                if (bilgi.durum == (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI)
                    bilgi.durum = (int)ENUMHibeYatirimBasvuruDurum.YENI;
                else
                    bilgi.durum = (int)ENUMHibeYatirimBasvuruDurum.TANIMSIZ;

                if (bilgi.online)
                    tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                else
                    tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.DEGISTIRILDI;

                //Değişikliği bul ve tarihçeye yaz
                //*********************************************

                tarihce.aciklama = EskiYeniBilgiKarsilastir(bilgi, hm);

                //*********************************************
            }

            tarihce.hibeKod = bilgi.kod;
            tarihce.ipAdresi = bilgi.ipAdresi == "" ? kullanan.telefon : bilgi.ipAdresi;
            tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
            tarihce.kullaniciAdi = kullanan.ADSOYAD;

            if (yeni)
                sonuc = to.Kaydet(bilgi);
            else
            {
                //if (bilgi.basvuruEskiSiraNo.Length != 5 || bilgi.basvuruYeniSiraNo.Length != 5)
                //{
                //    sonuc.islemSonuc = false;
                //    sonuc.hataStr = "Başvuru sıra numarası 5 haneli olmak zorundadır.";
                //}
                //if (sonuc.islemSonuc && !bilgi.basvuruEskiSiraNo.Equals(bilgi.basvuruYeniSiraNo))
                //{
                //    sonuc = to.GuncelleBasvuruSiraNo(aktifEtapNo, bilgi.ilKod, bilgi.basvuruYeniSiraNo, bilgi.basvuruEskiSiraNo);
                //    if (!sonuc.islemSonuc && sonuc.kayitSay == 0)
                //        sonuc.hataStr = bilgi.basvuruEskiSiraNo + " olan başvuru sıra numarasını " + bilgi.basvuruYeniSiraNo + " yapabilmeniz için, sistemde başvuru sıra numarası " + bilgi.basvuruYeniSiraNo + " olan bir başvurunun olması gerekir.";
                //}
                //if (sonuc.islemSonuc)
                sonuc = to.Guncelle(bilgi);
            }

            if (sonuc.islemSonuc)
                sonuc = to.KaydetTarihce(tarihce);

            if (sonuc.islemSonuc)
            {
                HibeTutarHesapla(bilgi.projeNitelik, bilgi.projeAltKonu, 0, bilgi.basvuruMaliyet, 0, ref bilgi.hibeEsasTutar, ref bilgi.toplamTutar, ref bilgi.ayniKatki, ref bilgi.hibeTutar, bilgi.hukukiStatu, 50, bilgi.etapNo);
                sonuc = to.KaydetHibeTutarlari(bilgi);
            }

            if (sonuc.islemSonuc)
            {
                HibeYatirimMaliyet maliyet = new HibeYatirimMaliyet();
                maliyet.hibeKod = bilgi.kod;
                maliyet.hibeEsasTutarToplam = bilgi.hibeEsasTutar;
                maliyet.ayniKatkiToplam = bilgi.ayniKatki;
                maliyet.projeToplamTutar = bilgi.toplamTutar;
                maliyet.hibeTutar = bilgi.hibeTutar;

                to.SilMaliyet(maliyet, ENUMHibeYatirimBasvuruDurum.YENI);
                sonuc = to.KaydetMaliyet(maliyet, ENUMHibeYatirimBasvuruDurum.YENI);
            }

            if (sonuc.islemSonuc)
            {
                to.SilSoru(bilgi.kod);

                if (bilgi.sorular != null && bilgi.sorular.Length > 0)
                {
                    foreach (HibeYatirimSoru item in bilgi.sorular)
                    {
                        item.hibeKod = bilgi.kod;
                        sonuc = to.KaydetSoru(item);
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                if (bilgi.altYapiGrup == "A")
                {
                    TRMHibeYatirimMalzeme.SilHibeKoduyla(ve, kullanan, bilgi.kod);
                    foreach (var item in bilgi.malzeme)
                    {
                        item.hibeKod = bilgi.kod;
                        sonuc = TRMHibeYatirimMalzeme.Kaydet(ve, kullanan, item);
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                if (!string.IsNullOrEmpty(konuNitelikUygunluk))
                    sonuc.bilgiStr += "<br>Fakat, " + konuNitelikUygunluk;

                sonuc.anahtar = bilgi.kod.ToString() + ";" + bilgi.basvuruNo + ";" + bilgi.durum;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public Sonuc HibeTutarlariKaydet(Kullanici kullanan, HibeYatirim bilgi)
        {
            if (bilgi.kod == 0)
                return new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);

            ve.BeginTransaction();

            HibeTutarHesapla(bilgi.projeNitelik, bilgi.projeAltKonu, 0, bilgi.basvuruMaliyet, 0, ref bilgi.hibeEsasTutar, ref bilgi.toplamTutar, ref bilgi.ayniKatki, ref bilgi.hibeTutar, bilgi.hukukiStatu, 50, bilgi.etapNo);
            Sonuc sonuc = to.KaydetHibeTutarlari(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc BasvuruKopyasiAl(VeriErisim ve, Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            sonuc = to.BasvuruKopyasiAl(kod);

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, hibe makine sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BasvuruyuIslemeAl(Kullanici kullanan, int kod, string ilKod, string basvuruNo, string ip)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //if (!Yetki.YetkiVarMiYazmaya(kullanan, ilKod, -1, string.Empty, ENUMHibeTur.EKONOMIKYATIRIM))
            //{
            //    sonuc.hataStr = "Yazma yetkiniz bulunmamaktadır.";
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            string hata = TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, basvuruNo);
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = "Hibe başvurusu online girişe kapatıldığı için işlem yapılamaz.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }
            if (!ListeleBasvuruYapildi(ve, kullanan, kod))
            {
                sonuc.hataStr = "Yatırımcı henüz başvuru girişini bitirmediği için işleme alamazsınız.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            ObjectArray hmler = Listele(ve, kullanan, new HibeYatirim { kod = kod, online = true });
            if (hmler == null || hmler.objeler.Count == 0 || hmler.objeler.Count > 1)
            {
                sonuc.hataStr = " Çalışma yapılacak yatırımlar seçilmemiş.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            HibeYatirim hm = (HibeYatirim)hmler.objeler[0];
            if (hm.durum > 2)
            {
                sonuc.hataStr = "Bu başvuru işlem yapılması için uygun değildir.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            //YatirimIsletmePlani yip = YatirimIsletmePlaniGetir(kullanan, hm.kod);
            //if (yip.hibeKod == 0)
            //{
            //    sonuc.hataStr = "Yatırım özeti girilmemiş. Başvuruyu tamamlayamazsınız";
            //    sonuc.islemSonuc = false;
            //    ve.VeriErisimKapat();
            //    return sonuc;
            //}

            BasvurudaYuklenenBelgeler dosya = new BasvurudaYuklenenBelgeler();
            dosya.hibeKod = hm.kod;
            ObjectArray yukluBelge = new TRMHibeBasvuruYuklenenBelgeler().Listele(kullanan, dosya);
            if (yukluBelge.objeler.Count == 0)
            {
                sonuc.hataStr = "Belge yüklemesi yapılmamış. Başvuruyu tamamlayamazsınız";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (hm.altYapiGrup != "A" && hm.etapNo < 16)
            {
                Basvuru basvuruForm = new Basvuru();
                basvuruForm.hibeKod = hm.kod;
                basvuruForm.turKod = (int)ENUMHibeTur.EKONOMIKYATIRIM;
                Basvuru[] donusDeger = TRMHibeBasvuru.Listele(null, kullanan, basvuruForm);
                if (donusDeger.Length == 0)
                {
                    sonuc.hataStr = "Başvuru formu doldurulmamış. Başvuruyu tamamlayamazsınız";
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            //ObjectArray butceListe = new TRMHibeYatirimBasvuruTablolari().ProjeButcesiListele(kullanan, new YatirimProjeButcesi() { hibeKod = hm.kod });
            //if (butceListe.objeler.Count == 0)
            //{
            //    sonuc.hataStr = "Proje bütçesi girilmemiş. Başvuruyu tamamlayamazsınız";
            //    sonuc.islemSonuc = false;
            //    ve.VeriErisimKapat();
            //    return sonuc;
            //}
            //else
            //{
            //    foreach (YatirimProjeButcesi item in butceListe.objeler)
            //    {
            //        if (hm.altYapiGrup == "A")
            //        {
            //            if (item.giderKalemTuru == 2 && item.toplamTutar == 0)
            //            {
            //                sonuc.hataStr = "A grubu için Hibeye Esas-Proje Makine Ve Ekipman Alım Giderleri girmelisiniz. Başvuruyu tamamlayamazsınız";
            //                sonuc.islemSonuc = false;
            //                ve.VeriErisimKapat();
            //                return sonuc;
            //            }
            //            if ((item.giderKalemTuru == 1 || item.giderKalemTuru == 4 || item.giderKalemTuru == 5) && item.toplamTutar > 0)
            //            {
            //                sonuc.hataStr = "A grubu için sadece Hibeye Esas-Proje Makine ve Ekipman Alım Giderleri girilebilir. Başvuruyu tamamlayamazsınız";
            //                sonuc.islemSonuc = false;
            //                ve.VeriErisimKapat();
            //                return sonuc;
            //            }
            //        }
            //    }
            //}


            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            string plaka = new TABGenel(ve).IlPlakaKodBul(hm.ilKod);

            string konuKodu = hm.projeAltKonu;
            if (konuKodu == "") konuKodu = hm.projeKonu;

            konuKodu = new TABYatirimKonu(ve).KonuKisaAdBul(konuKodu);

            string altKonuDetayNumara = "";
            if (hm.altKonuDetayKod != "")
            {
                altKonuDetayNumara = new TABYatirimKonu(ve).AltKonuDetayNoBul(hm.altKonuDetayKod);
            }

            string eskiBasvuruNo = hm.basvuruNo;
            string yeniBasvuruNo = "";
            if (eskiBasvuruNo.Split('.').Length == 2)
            {
                yeniBasvuruNo = BasvuruNoAl(ve, hm.etapNo, plaka, konuKodu, altKonuDetayNumara, hm.projeNitelik, hm.altYapiGrup);
                sonuc = to.GuncelleBasvuruSiraNo(kod, yeniBasvuruNo);
            }
            else
                yeniBasvuruNo = eskiBasvuruNo;

            sonuc = to.BasvuruyuIslemeAl(kod);

            if (sonuc.islemSonuc)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.hibeKod = kod;
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.YENI;
                tarihce.ipAdresi = ip == "" ? kullanan.telefon : ip;
                tarihce.aciklama = eskiBasvuruNo + " olan başvuruya " + yeniBasvuruNo + " numarası verildi";

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = "Online başvuruyu işleme alma başarıyla tamamlandı.";
                sonuc.anahtar = kod.ToString();
                sonuc.hataStr = yeniBasvuruNo;

                ve.Commit();
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetSozlesmeSonrasi(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (!Yetki.YetkiVarMiYazmaya(kullanan, bilgi.ilKod, bilgi.etapNo, sonuc, string.Empty, ENUMHibeTur.EKONOMIKYATIRIM, bilgi.kod, bilgi.online))
            {
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            HibeYatirim hm = new HibeYatirim();
            string hata = IslemKontrol(ve, kullanan, bilgi, ref hm, "BasvuruKaydet");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            //**************************************************************************************
            //Kabul aşamasında online başvuru yapıldığında merkez tarafından kabul edilmiş halinin saklanması için
            if (bilgi.kod > 0 && bilgi.online)
            {
                BasvuruKopyasiAl(ve, kullanan, bilgi.kod);
                ve.hata = "";//Eğer daha önce kayıt edilmiş ise PK hatası vermemesi için
            }
            //**************************************************************************************


            HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
            tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
            tarihce.kullaniciAdi = kullanan.ADSOYAD;
            tarihce.tarih = new TNSDateTime(DateTime.Now);
            tarihce.durum = bilgi.durum;
            tarihce.ipAdresi = bilgi.ipAdresi;


            //Değişikliği bul ve tarihçeye yaz
            //*********************************************

            tarihce.aciklama = EskiYeniBilgiKarsilastir(bilgi, hm);

            //*********************************************


            sonuc = KontrolKimlikNoKayitVarMi(ve, kullanan, bilgi.kod, bilgi.kimlikNo, bilgi.etapNo);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            sonuc = KonuNitelikKontrol(ve, kullanan, bilgi);
            string konuNitelikUygunluk = string.Empty;
            if (!sonuc.islemSonuc)
            {
                konuNitelikUygunluk = sonuc.hataStr;
                bilgi.konuNitelikUyusma = false;

                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;

                //sonuc.islemSonuc = true;
                //sonuc.hataStr = string.Empty;
            }

            string[] basvuruNo = bilgi.basvuruNo.Split('.');
            string konuKisaAd = new TABYatirimKonu(ve).KonuKisaAdBul(bilgi.projeKonu);
            basvuruNo[1] = bilgi.projeNitelik.ToString();
            basvuruNo[2] = konuKisaAd;
            bilgi.basvuruNo = basvuruNo[0] + "." + basvuruNo[1] + "." + basvuruNo[2] + "." + basvuruNo[3] + "." + basvuruNo[4];

            tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.DEGISTIRILDI;
            tarihce.hibeKod = bilgi.kod;
            tarihce.ipAdresi = bilgi.ipAdresi == "" ? kullanan.telefon : bilgi.ipAdresi;
            tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
            tarihce.kullaniciAdi = kullanan.ADSOYAD;

            sonuc = to.GuncelleSozlesmeSonrasi(bilgi);

            if (sonuc.islemSonuc)
                sonuc = to.KaydetTarihce(tarihce);

            if (sonuc.islemSonuc)
            {
                //Melih 05.02.2020 Avan projesini Tatbikata çevirirken rakamlar ile oynanmasın
                //HibeTutarHesapla(bilgi.projeNitelik, bilgi.projeAltKonu, 0, bilgi.basvuruMaliyet, 0, ref bilgi.hibeEsasTutar, ref bilgi.toplamTutar, ref bilgi.ayniKatki, ref bilgi.hibeTutar, bilgi.hukukiStatu, 50);
                //sonuc = to.KaydetHibeTutarlari(bilgi);
            }

            if (sonuc.islemSonuc)
            {
                HibeYatirimMaliyet maliyet = new HibeYatirimMaliyet();
                maliyet.hibeKod = bilgi.kod;
                maliyet.hibeEsasTutarToplam = bilgi.hibeEsasTutar;
                maliyet.ayniKatkiToplam = bilgi.ayniKatki;
                maliyet.projeToplamTutar = bilgi.toplamTutar;
                maliyet.hibeTutar = bilgi.hibeTutar;

                to.SilMaliyet(maliyet, ENUMHibeYatirimBasvuruDurum.YENI);
                sonuc = to.KaydetMaliyet(maliyet, ENUMHibeYatirimBasvuruDurum.YENI);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                if (!string.IsNullOrEmpty(konuNitelikUygunluk))
                    sonuc.bilgiStr += "<br>Fakat, " + konuNitelikUygunluk;

                sonuc.anahtar = bilgi.kod.ToString() + ";" + bilgi.basvuruNo + ";" + bilgi.durum;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc KaydetKabul(Kullanici kullanan, int etapNo, string ip, int basvuruTur)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);
            if (etapNo <= 0)
                sonuc.hataStr = "Etap No boş bırakılamaz.<br>";
            if (basvuruTur < 1 && basvuruTur > 2) // 1 veya 2 olmalı
                sonuc.hataStr += "Başvuru Tür boş bırakılamaz.<br>";

            //if (string.IsNullOrEmpty(ilKod))
            //    sonuc.hataStr += "İl boş bırakılamaz.";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            //tarihce = new HibeYatirimTarihce();
            //tarihce.kullaniciKod = kullanan.kullaniciKodu;
            //tarihce.tarih = new TNSDateTime(DateTime.Now);
            //tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.DEGISTIRILDI;

            sonuc = to.KaydetKabul(etapNo, basvuruTur);
            if (sonuc.islemSonuc)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI;
                tarihce.ipAdresi = ip;

                sonuc = to.KaydetTarihceKabul(etapNo, tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc KaydetKabulGeriAl(Kullanici kullanan, int etapNo, string ip, int geriAlmaTuru, int basvuruTur, string altYapiGrup)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);
            if (etapNo <= 0)
                sonuc.hataStr = "Etap No boş bırakılamaz.<br>";
            if (basvuruTur < 1 && basvuruTur > 2) // 1 veya 2 olmalı
                sonuc.hataStr += "Başvuru Tür boş bırakılamaz.<br>";
            if (basvuruTur == 2 && altYapiGrup == "")
                sonuc.hataStr += "Alt yapı grubu boş bırakılamaz.<br>";
            if (geriAlmaTuru == 0)
                sonuc.hataStr += "Geri alma yöntemi boş bırakılamaz.<br>";

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);


            if (basvuruTur == 2 && altYapiGrup == "") altYapiGrup = "B";

            IDataReader dr = to.KaydetKabulGeriAlListele(etapNo, basvuruTur, altYapiGrup);

            if (!string.IsNullOrEmpty(to.Hata()))
            {
                dr.Close();
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
                return sonuc;
            }

            List<int> al = new List<int>();
            while (dr.Read())
            {
                int kod = VeriErisim.DegerAl(dr, 0, 0);
                al.Add(kod);
            }
            dr.Close();


            ve.BeginTransaction();

            foreach (int item in al)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.hibeKod = item;
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.ipAdresi = ip;

                if (geriAlmaTuru == 2)
                    tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEIMZALAMADI;
                else
                    tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDIMERKEZ;

                sonuc = to.KaydetTarihce(tarihce);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                if (geriAlmaTuru == 2)
                    sonuc = to.KaydetKabulGeriAlKabulYaz(etapNo, basvuruTur, altYapiGrup);
            }

            if (sonuc.islemSonuc)
            {
                sonuc = to.KaydetKabulGeriAl(etapNo, basvuruTur, altYapiGrup);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc MaliyetGuncelle(Kullanici kullanan, HibeYatirimMaliyet maliyet)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            Sonuc sonuc = new Sonuc(false, "", "");
            HibeYatirim hm = new HibeYatirim();

            ObjectArray hmler = Listele(ve, kullanan, new HibeYatirim { kod = maliyet.hibeKod });
            hm = (HibeYatirim)hmler.objeler[0];

            sonuc = UygulamaSozlesmesineUygunMu(ve, maliyet.hibeKod, maliyet);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (maliyet.hibeEsasTutarToplam == 0)
            {
                sonuc.hataStr = "Hibe Esas Gider Bilgileri olan İnşaat-Mal Giderlerinden herhangi biri dolu olmalı.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            sonuc = HibeEsasTutarKontrol(kullanan, to, maliyet.hibeKod, maliyet, hm);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            maliyet.hibeEsasTutarToplam = maliyet.hibeEsasTutarInsaat + maliyet.hibeEsasTutarMal;
            maliyet.ayniKatkiToplam = maliyet.ayniKatkiInsaat + maliyet.ayniKatkiMal;
            maliyet.projeToplamTutar = maliyet.hibeEsasTutarToplam + maliyet.ayniKatkiToplam;

            hm.sozlesmeMaliyet = maliyet.projeToplamTutar;
            hm.hibeEsasTutar = maliyet.hibeEsasTutarToplam;

            HibeTutarHesapla(hm.projeNitelik, hm.projeAltKonu, 0, hm.basvuruMaliyet, hm.sozlesmeMaliyet, ref hm.hibeEsasTutar, ref hm.toplamTutar, ref hm.ayniKatki, ref hm.hibeTutar, hm.hukukiStatu, 50, hm.etapNo);

            maliyet.hibeEsasTutarToplam = hm.hibeEsasTutar;
            maliyet.ayniKatkiToplam = hm.ayniKatki;
            maliyet.projeToplamTutar = hm.toplamTutar;
            maliyet.hibeTutar = hm.hibeTutar;
            ENUMHibeYatirimBasvuruDurum enumDurum = ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI;

            if (maliyet.durum == (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI)
                enumDurum = ENUMHibeYatirimBasvuruDurum.KABULEDILDI;
            else if (maliyet.durum == (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI)
                enumDurum = ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI;
            else if (maliyet.durum == (int)ENUMHibeYatirimBasvuruDurum.ILDEKABULEDILENLER)
                enumDurum = ENUMHibeYatirimBasvuruDurum.ILDEKABULEDILENLER;

            to.SilMaliyet(maliyet, enumDurum);
            sonuc = to.KaydetMaliyet(maliyet, enumDurum);
            //if (sonuc.islemSonuc)
            //    sonuc = to.KaydetHibeTutarlari(hm);//Sözleşme sırasında değiştirilen tutarlar başvuru rakamını değiştirmesin Melih 24.07.2020

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                sonuc.anahtar = hm.kod.ToString();
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, hibe Yatirim sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sozlesme">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetSozlesme(Kullanici kullanan, HibeYatirimSozlesme sozlesme, HibeYatirimMaliyet maliyet, string ip)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            bool guncelleDigerAlanlar = false;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));

            HibeYatirim hm = new HibeYatirim();
            HibeYatirim bilgi = new HibeYatirim() { kod = sozlesme.hibeKod, sozlesmeTarih = sozlesme.sozlesmeTarih, bankaSubeKod = sozlesme.bankaSubeKod, eskiBasvuru = sozlesme.eskiBasvuru };
            string hata = IslemKontrol(ve, kullanan, bilgi, ref hm, "Sozlesme");
            if (hata != "")
            {
                if (!hata.StartsWith("++"))
                {
                    sonuc.hataStr = hata;
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
                else
                    guncelleDigerAlanlar = true;
            }

            sonuc = UygulamaSozlesmesineUygunMu(ve, sozlesme.hibeKod, maliyet);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }
            TABHibeYatirim to = new TABHibeYatirim(ve);
            if (maliyet.hibeEsasTutarToplam == 0)
            {
                sonuc.hataStr = "Hibe Esas Gider Bilgileri olan İnşaat-Mal Giderlerinden herhangi biri dolu olmalı.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (maliyet.hibeEsasTutarInsaat + maliyet.ayniKatkiInsaat > 0 && hm.etapNo >= 15 && hm.altYapiGrup == "A")
            {
                sonuc.hataStr = "Alt yapı A grubu için İnşaat-Mal Gideri yapılamaz.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (hm.etapNo >= 15 && hm.altYapiGrup == "A")
            {
                decimal referansFiyat = MalzemeninReferansFiyatiniBul(kullanan, hm);

                if (referansFiyat == 0)
                {
                    sonuc.hataStr = "Satın alınacak malzemenin referans fiyatı bulunamadı";
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            if (string.IsNullOrWhiteSpace(sozlesme.yapiKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Yapı Kimlik Bilgisi giriniz. Örnek: 02.0.0001978765\n\nEğer Yapı Kimlik Bilgisi yoksa sadece YOK yazınız";
                ve.VeriErisimKapat();
                return sonuc;
            }

            //sonuc = BasvuruyaUygunMu(kullanan, to, sozlesme.hibeKod, maliyet);
            sonuc = HibeEsasTutarKontrol(kullanan, to, sozlesme.hibeKod, maliyet, hm);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            maliyet.hibeEsasTutarToplam = maliyet.hibeEsasTutarInsaat + maliyet.hibeEsasTutarMal;
            maliyet.ayniKatkiToplam = maliyet.ayniKatkiInsaat + maliyet.ayniKatkiMal + maliyet.ayniKatkiDanismanlik;
            maliyet.projeToplamTutar = maliyet.hibeEsasTutarToplam + maliyet.ayniKatkiToplam;

            hm.sozlesmeTarih = sozlesme.sozlesmeTarih;
            hm.sozlesmeMaliyet = maliyet.projeToplamTutar;
            hm.hibeEsasTutar = maliyet.hibeEsasTutarToplam;

            if (!guncelleDigerAlanlar)
                hm.durum = (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI;

            sonuc = to.KaydetSozlesme(hm);
            if (sonuc.islemSonuc)
            {
                to.SilSozlesmeDetay(sozlesme);
                sonuc = to.KaydetSozlesmeDetay(sozlesme);
            }

            if (sonuc.islemSonuc && !guncelleDigerAlanlar)
            {
                HibeTutarHesapla(hm.projeNitelik, hm.projeAltKonu, 0, hm.basvuruMaliyet, hm.sozlesmeMaliyet, ref hm.hibeEsasTutar, ref hm.toplamTutar, ref hm.ayniKatki, ref hm.hibeTutar, hm.hukukiStatu, 50, hm.etapNo);

                maliyet.hibeEsasTutarToplam = hm.hibeEsasTutar;
                maliyet.ayniKatkiToplam = hm.ayniKatki;
                maliyet.projeToplamTutar = hm.toplamTutar;
                maliyet.hibeTutar = hm.hibeTutar;

                to.SilMaliyet(maliyet, ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI);
                sonuc = to.KaydetMaliyet(maliyet, ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI);
                //if (sonuc.islemSonuc)
                //    sonuc = to.KaydetHibeTutarlari(hm);//Sözleşme sırasında değiştirilen tutarlar başvuru rakamını değiştirmesin Melih 24.07.2020
            }

            if (sonuc.islemSonuc)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI;
                tarihce.hibeKod = hm.kod;
                tarihce.ipAdresi = ip == "" ? kullanan.telefon : ip;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                if (guncelleDigerAlanlar)
                    sonuc.bilgiStr += "<br><br>Bu sözleşmenin uygulama sözleşmesi bulunmaktadır. Bu nedenle sadece parasal olmayan bilgiler güncellendi.";
                sonuc.anahtar = hm.kod.ToString();
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static decimal MalzemeninReferansFiyatiniBul(Kullanici kullanan, HibeYatirim hm)
        {
            HibeYatirim kriter = new HibeYatirim();
            kriter.etapNo = hm.etapNo;
            kriter.kod = hm.kod;
            ObjectArray oListeMalzeme = ListeleMalzeme(null, kullanan, kriter, 0, 0, "", true);

            decimal referansFiyat = 0;

            foreach (HibeYatirim ff in oListeMalzeme.objeler)
            {
                foreach (var mal in ff.malzeme)
                {
                    referansFiyat += MalzemeninReferansFiyatiniBulIc(kullanan, hm.etapNo, hm.kod, mal.malzeme.kod, hm.ilKod);
                }
            }

            return referansFiyat;
        }

        private static decimal MalzemeninReferansFiyatiniBulIc(Kullanici kullanan, int etapNo, int hibeKod, string malzemeKod, string ilKod)
        {
            decimal referansFiyat = 0;
            if (malzemeKod != "")
            {
                HibeYatirimRefFiyat kriterRefFiyat = new HibeYatirimRefFiyat();
                kriterRefFiyat.etapNo = etapNo;
                kriterRefFiyat.malzemeKod = malzemeKod;
                kriterRefFiyat.ilKod = "0000000";

                ObjectArray oRefFiyatListe = new TRMHibeYatirimRefFiyat().Listele(kullanan, kriterRefFiyat);
                foreach (HibeYatirimRefFiyat ff in oRefFiyatListe.objeler)
                {
                    if (ff.fiyat == 0) continue;
                    referansFiyat = OrtakFonksiyonlar.ConvertToDecimal(ff.fiyat);
                    break;
                }

                if (referansFiyat == 0)
                {
                    kriterRefFiyat = new HibeYatirimRefFiyat();
                    kriterRefFiyat.etapNo = etapNo;
                    kriterRefFiyat.malzemeKod = malzemeKod;
                    kriterRefFiyat.ilKod = ilKod;

                    oRefFiyatListe = new TRMHibeYatirimRefFiyat().Listele(kullanan, kriterRefFiyat);
                    foreach (HibeYatirimRefFiyat ff in oRefFiyatListe.objeler)
                    {
                        referansFiyat = OrtakFonksiyonlar.ConvertToDecimal(ff.fiyat);
                    }
                }
            }

            return referansFiyat;
        }

        public Sonuc SilSozlesme(Kullanici kullanan, HibeYatirimSozlesme sozlesme)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string hata = "";

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUMERKEZ) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI)))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Sözleşmeyi silme yetkiniz yok. Merkez tarafından silinebilir";
                return sonuc;
            }

            if (sozlesme.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            HibeYatirimUygulamaSozlesme uy = new HibeYatirimUygulamaSozlesme();
            uy.hibeKod = sozlesme.hibeKod;
            HibeYatirimUygulamaSozlesme[] hy = TRMHibeYatirimUygulamaSozlesme.Listele(ref ve, kullanan, uy, false);
            if (hy != null && hy.Length > 0)
                hata = "Bu sözleşmenin uygulama sözleşmesi bulunmaktadır. Bu nedenle sözleşme silinemez";

            if (hata != "")
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve.BeginTransaction();
            TABHibeYatirim to = new TABHibeYatirim(ve);

            sonuc = to.SilSozlesmeDetay(sozlesme);

            if (sonuc.islemSonuc)
            {
                HibeYatirim hya = new HibeYatirim();
                hya.kod = sozlesme.hibeKod;
                hya.durum = (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI;
                to.GuncelleDurum(hya);

                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.SOZLESMESILINDI;
                tarihce.hibeKod = sozlesme.hibeKod;
                tarihce.ipAdresi = sozlesme.ipAdresi == "" ? kullanan.telefon : sozlesme.ipAdresi;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Silme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc UygulamaSozlesmesineUygunMu(VeriErisim ve, int hibeKod, HibeYatirimMaliyet maliyet)
        {
            TABHibeYatirimUygulamaSozlesme tabUS = new TABHibeYatirimUygulamaSozlesme(ve);
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);

            decimal usTutar = tabUS.SozlesmeTutarToplamAl(hibeKod, 0, false, ENUMHibeYatirimUygulamaSozlesmeTur.INSAAT);
            usTutar += tabHakedis.FaturaTutarToplamAlUygSozIptalOlanlar(hibeKod, 0, ENUMHibeYatirimUygulamaSozlesmeTur.INSAAT);
            if (maliyet.hibeEsasTutarInsaat < usTutar)
                return new Sonuc(false, "Hibeye esas inşaat gideri, şu ana kadar kayıt edilmiş olan uygulama sözleşmesi (" + HibeYatirimUygulamaSozlesme.SozlesmeTurAdBul(ENUMHibeYatirimUygulamaSozlesmeTur.INSAAT) + ") tutarlarının toplamı olan " + usTutar.ToString("N2") + " TL'den küçük olamaz.", string.Empty);

            usTutar = tabUS.SozlesmeTutarToplamAl(hibeKod, 0, false, ENUMHibeYatirimUygulamaSozlesmeTur.MALALIMI);
            usTutar += tabHakedis.FaturaTutarToplamAlUygSozIptalOlanlar(hibeKod, 0, ENUMHibeYatirimUygulamaSozlesmeTur.MALALIMI);
            if (maliyet.hibeEsasTutarMal < usTutar)
                return new Sonuc(false, "Hibeye esas mal gideri, şu ana kadar kayıt edilmiş olan uygulama sözleşmesi (" + HibeYatirimUygulamaSozlesme.SozlesmeTurAdBul(ENUMHibeYatirimUygulamaSozlesmeTur.MALALIMI) + ") tutarlarının toplamı olan " + usTutar.ToString("N2") + " TL'den küçük olamaz.", string.Empty);

            return new Sonuc();
        }

        //private static Sonuc BasvuruyaUygunMu(Kullanici kullanan, TABHibeYatirim thy, int hibeKod, HibeYatirimMaliyet maliyet)
        //{
        //    try
        //    {
        //        HibeYatirimMaliyet basvuruMaliyet = ListeleMaliyet(thy, hibeKod, ENUMHibeYatirimBasvuruDurum.YENI);
        //        if (basvuruMaliyet == null)
        //            return new Sonuc(false, "Aranan başvuru bulunamadı.", string.Empty);
        //        if (maliyet.hibeEsasTutarInsaat + maliyet.hibeEsasTutarMal > basvuruMaliyet.hibeEsasTutarToplam)
        //            return new Sonuc(false, "Hibeye esas inşaat ve mal alımı giderleri toplamı, başvurudaki hibeye esas tutar olan " + basvuruMaliyet.hibeEsasTutarToplam.ToString("N2") + " TL'den büyük olamaz.", string.Empty);
        //        return new Sonuc();
        //    }
        //    catch (Exception e)
        //    {
        //        return new Sonuc(false, e.ToString(), string.Empty);
        //    }
        //}

        public static HibeYatirim ListeleSozlesme(ref VeriErisim ve, Kullanici kullanan, int hibeKod)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            IDataReader dr = to.ListeleSozlesme(hibeKod);

            if (!string.IsNullOrEmpty(to.Hata()))
            {
                dr.Close();
                ve.VeriErisimKapat();
                throw new Exception(to.Hata());
            }

            HibeYatirim hy = new HibeYatirim();
            if (dr.Read())
            {
                hy.ilKod = VeriErisim.DegerAl(dr, 5, string.Empty);

                if (!TNS.TRM.Yetki.YetkiVarMiOkumaya(kullanan, hy.ilKod, ENUMHibeTur.EKONOMIKYATIRIM))
                    throw new Exception("Okuma yetkiniz bulunmamaktadır.");

                hy.kod = hibeKod;
                hy.basvuruNo = VeriErisim.DegerAl(dr, 0, string.Empty);
                hy.durum = VeriErisim.DegerAl(dr, 1, 0);
                hy.kimlikNo = VeriErisim.DegerAl(dr, 2, string.Empty);
                hy.ad = VeriErisim.DegerAl(dr, 3, string.Empty);
                hy.soyad = VeriErisim.DegerAl(dr, 4, string.Empty);
                hy.ilKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                hy.toplamTutar = VeriErisim.DegerAl(dr, 6, (decimal)0);
                hy.ayniKatki = VeriErisim.DegerAl(dr, 7, (decimal)0);
                hy.hibeEsasTutar = VeriErisim.DegerAl(dr, 8, (decimal)0);
                hy.hibeTutar = VeriErisim.DegerAl(dr, 9, (decimal)0);
                hy.altYapiGrup = VeriErisim.DegerAl(dr, 10, string.Empty);
                hy.etapNo = VeriErisim.DegerAl(dr, 11, 0);
                hy.projeKonu = VeriErisim.DegerAl(dr, 12, "");

                int sonKolon = 13;
                HibeYatirimSozlesme hys = new HibeYatirimSozlesme();
                hys.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.HIBEKOD + sonKolon, 0);
                hys.sozlesmeTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.SOZLESMETARIH + sonKolon);
                hys.projeBasTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.PROJEBASTARIH + sonKolon);
                hys.projeBitTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.PROJEBITTARIH + sonKolon);
                hys.nihaiRaporTarihi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.NIHAIRAPORTARIHI + sonKolon);
                hys.istihdam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.ISTIHDAM + sonKolon, 0);
                hys.uretimKapasitesi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.URETIMKAPASITESI + sonKolon, (decimal)0);
                hys.teminatTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTUTAR + sonKolon, (decimal)0);
                hys.teminatTur = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTUR + sonKolon, 0);
                hys.teminatTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTARIH + sonKolon);
                hys.bankaSubeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.BANKASUBEKOD + sonKolon, "");
                hys.kapasiteBirimKodu = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.KAPASITEBIRIM + sonKolon, "");
                hys.istihdamKadin = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.ISTIHDAMKADIN + sonKolon, 0);
                hys.durum = (ENUMHibeYatirimSozlesmeDurum)VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.DURUM + sonKolon, 0);
                hys.yapiKimlikNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.YAPIKIMLIKNO + sonKolon, "");
                hy.sozlesmeBilgileri = hys;

                sonKolon = 28;
                HibeYatirimMaliyet hym = new HibeYatirimMaliyet();
                hym.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEKOD + sonKolon, 0);
                hym.hibeEsasTutarInsaat = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARINSAAT + sonKolon, (decimal)0);
                hym.hibeEsasTutarMal = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARMAL + sonKolon, (decimal)0);
                hym.hibeEsasTutarToplam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARTOPLAM + sonKolon, (decimal)0);
                hym.ayniKatkiInsaat = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIINSAAT + sonKolon, (decimal)0);
                hym.ayniKatkiMal = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIMAL + sonKolon, (decimal)0);
                hym.ayniKatkiDanismanlik = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIDANISMANLIK + sonKolon, (decimal)0);
                hym.ayniKatkiToplam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKITOPLAM + sonKolon, (decimal)0);
                hym.projeToplamTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.PROJETOPLAMTUTAR + sonKolon, (decimal)0);
                hym.hibeTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBETUTAR + sonKolon, (decimal)0);
                hy.maliyetBilgileri = new HibeYatirimMaliyet[1];
                hy.maliyetBilgileri[0] = hym;
            }
            dr.Close();
            ve.VeriErisimKapat();
            return hy;
        }

        public static HibeYatirim ListeleYatirimVeSozlesme(ref VeriErisim ve, Kullanici kullanan, int hibeKod)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            IDataReader dr = to.ListeleYatirimVeSozlesme(hibeKod);

            if (!string.IsNullOrEmpty(to.Hata()))
            {
                dr.Close();
                ve.VeriErisimKapat();
                throw new Exception(to.Hata());
            }

            HibeYatirim hy = new HibeYatirim();
            if (dr.Read())
            {
                hy.ilKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ILKOD, string.Empty);
                if (!TNS.TRM.Yetki.YetkiVarMiOkumaya(kullanan, hy.ilKod, ENUMHibeTur.EKONOMIKYATIRIM))
                    throw new Exception("Okuma yetkiniz bulunmamaktadır.");

                int sonKolon = ((int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD) + 1;

                hy.kod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                hy.yil = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.YIL, 0);
                hy.basvuruNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.BASVURUNO, string.Empty);
                hy.ilKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ILKOD, string.Empty);
                hy.ilceKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ILCEKOD, string.Empty);
                hy.koyKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.KOYKOD, 0);
                hy.kasaba = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.KASABA, string.Empty);
                hy.hukukiStatu = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.HUKUKISTATU, 0);
                hy.kimlikNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.KIMLIKNO, string.Empty);
                hy.vergiDaire = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.VERGIDAIRE, string.Empty);
                hy.ad = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.AD, string.Empty);
                hy.evrakNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.EVRAKNO, string.Empty);
                hy.evrakTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.EVRAKTARIH);
                hy.adres = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ADRES, string.Empty);
                hy.telefon = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.TELEFON, string.Empty);
                hy.faks = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.FAKS, string.Empty);
                hy.ePosta = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.EPOSTA, string.Empty);
                hy.bankaSubeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.BANKASUBEKOD, string.Empty);
                hy.durum = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.DURUM, 0);
                hy.sozlesmeTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMETARIH);
                hy.faturaNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.FATURANO, string.Empty);
                hy.faturaTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.FATURATARIH);
                hy.soyad = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.SOYAD, string.Empty);
                hy.babaAd = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.BABAAD, string.Empty);
                hy.dogumTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.DOGUMTARIH);
                hy.yerindeTespitTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.YERINDETESPITTARIH);
                hy.etapNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ETAPNO, 0);

                hy.projeNitelik = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.PROJENITELIK, 0);
                hy.projeKonu = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.PROJEKONU, "");
                hy.projeAltKonu = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.PROJEALTKONU, "");
                hy.projeAdi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.PROJEADI, string.Empty);
                hy.sozlesmeMaliyet = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMEMALIYET, decimal.Zero);
                hy.basvuruMaliyet = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.BASVURUMALIYET, decimal.Zero);
                hy.hibeTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.HIBETUTAR, decimal.Zero);
                hy.hibeEsasTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.HIBEESASTUTAR, decimal.Zero);
                hy.ayniKatki = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.AYNIKATKI, decimal.Zero);
                hy.toplamTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.TOPLAMTUTAR, decimal.Zero);
                hy.anket1 = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ANKET1, 0);
                hy.anket2 = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ANKET2, 0);
                hy.anket3 = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ANKET3, 0);
                hy.anket4 = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ANKET4, 0);
                hy.anket3Puan = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ANKET3PUAN, 0);
                hy.anket4Puan = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ANKET4PUAN, 0);
                hy.puan = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.PUAN, (double)0);
                hy.konuNitelikUyusma = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.KONUNITELIKUYUSMA, 0) == 1 ? true : false;
                hy.degerlendirmeDisi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISI, 0) == 1 ? true : false;
                hy.degerlendirmeDisiGerekce = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISIGEREKCE, string.Empty);
                hy.kabulCalismasi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.KABULCALISMASI, 0);
                hy.altYapiGrup = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ALTYAPIGRUP, "");
                hy.cinsiyet = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.CINSIYET, "");
                hy.engelli = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ENGELLI, "");
                hy.organik = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ORGANIK, "");
                hy.yas = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.YAS, 0);
                hy.altKonuDetayKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD, "");

                HibeYatirimSozlesme hys = new HibeYatirimSozlesme();
                hys.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.HIBEKOD + sonKolon, 0);
                hys.sozlesmeTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.SOZLESMETARIH + sonKolon);
                hys.projeBasTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.PROJEBASTARIH + sonKolon);
                hys.projeBitTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.PROJEBITTARIH + sonKolon);
                hys.nihaiRaporTarihi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.NIHAIRAPORTARIHI + sonKolon);
                hys.istihdam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.ISTIHDAM + sonKolon, 0);
                hys.uretimKapasitesi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.URETIMKAPASITESI + sonKolon, (decimal)0);
                hys.teminatTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTUTAR + sonKolon, (decimal)0);
                hys.teminatTur = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTUR + sonKolon, 0);
                hys.teminatTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTARIH + sonKolon);
                hys.bankaSubeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.BANKASUBEKOD + sonKolon, "");
                hys.kapasiteBirimKodu = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.KAPASITEBIRIM + sonKolon, "");
                hys.istihdamKadin = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.ISTIHDAMKADIN + sonKolon, 0);
                hys.durum = (ENUMHibeYatirimSozlesmeDurum)VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.DURUM + sonKolon, 0);
                hys.yapiKimlikNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarSozlesme.YAPIKIMLIKNO + sonKolon, "");
                hy.sozlesmeBilgileri = hys;

                sonKolon = sonKolon + 15; // ((int)TABHibeYatirim.ENUMAlanlar.ISTIHDAM) + 13;

                HibeYatirimMaliyet hym = new HibeYatirimMaliyet();
                hym.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEKOD + sonKolon, 0);
                hym.hibeEsasTutarInsaat = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARINSAAT + sonKolon, (decimal)0);
                hym.hibeEsasTutarMal = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARMAL + sonKolon, (decimal)0);
                hym.hibeEsasTutarToplam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARTOPLAM + sonKolon, (decimal)0);
                hym.ayniKatkiInsaat = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIINSAAT + sonKolon, (decimal)0);
                hym.ayniKatkiMal = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIMAL + sonKolon, (decimal)0);
                hym.ayniKatkiToplam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKITOPLAM + sonKolon, (decimal)0);
                hym.projeToplamTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.PROJETOPLAMTUTAR + sonKolon, (decimal)0);
                hym.hibeTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBETUTAR + sonKolon, (decimal)0);
                hy.maliyetBilgileri = new HibeYatirimMaliyet[1];
                hy.maliyetBilgileri[0] = hym;

                sonKolon = sonKolon + 11; //((int)TABHibeYatirim.ENUMAlanlar.ISTIHDAM) + 23;

                hy.projeNitelikAdi = VeriErisim.DegerAl(dr, sonKolon, "");
                hy.projeKonuAdi = VeriErisim.DegerAl(dr, sonKolon + 1, "");
                hy.projeAltKonuAdi = VeriErisim.DegerAl(dr, sonKolon + 2, "");

                hy.koyAd = VeriErisim.DegerAl(dr, sonKolon + 3, "");
                hy.ilAd = VeriErisim.DegerAl(dr, sonKolon + 4, "");
                hy.ilceAd = VeriErisim.DegerAl(dr, sonKolon + 5, "");
            }
            dr.Close();
            ve.VeriErisimKapat();
            return hy;
        }

        private static HibeYatirimMaliyet ListeleMaliyet(TABHibeYatirim thy, int hibeKod, ENUMHibeYatirimBasvuruDurum durum)
        {
            IDataReader dr = thy.ListeleMaliyet(hibeKod, durum);
            if (!string.IsNullOrEmpty(thy.Hata()))
            {
                dr.Close();
                throw new Exception(thy.Hata());
            }

            HibeYatirimMaliyet hym = new HibeYatirimMaliyet();
            if (dr.Read())
            {
                hym.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEKOD, 0);
                hym.hibeEsasTutarInsaat = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARINSAAT, (decimal)0);
                hym.hibeEsasTutarMal = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARMAL, (decimal)0);
                hym.hibeEsasTutarToplam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARTOPLAM, (decimal)0);
                hym.ayniKatkiInsaat = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIINSAAT, (decimal)0);
                hym.ayniKatkiMal = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIMAL, (decimal)0);
                hym.ayniKatkiToplam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKITOPLAM, (decimal)0);
                hym.projeToplamTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.PROJETOPLAMTUTAR, (decimal)0);
                hym.hibeTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBETUTAR, (decimal)0);
            }
            dr.Close();
            return hym;
        }

        private static Sonuc HibeEsasTutarKontrol(Kullanici kullanan, TABHibeYatirim thy, int hibeKod, HibeYatirimMaliyet maliyet, HibeYatirim hm)
        {
            try
            {
                decimal hibeEsasTutar = (decimal)0;
                IDataReader dr = thy.HibeEsasTutarGetir(hibeKod, ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI);
                if (!string.IsNullOrEmpty(thy.Hata()))
                {
                    dr.Close();
                    throw new Exception(thy.Hata());
                }


                if (dr.Read())
                {
                    hibeEsasTutar = VeriErisim.DegerAl(dr, 0, (decimal)0);
                    dr.Close();
                }


                if (hm.etapNo >= 15 && hm.altYapiGrup == "A")
                {
                    decimal referansFiyat = MalzemeninReferansFiyatiniBul(kullanan, hm);
                    if (hibeEsasTutar > referansFiyat * (decimal)hm.malzeme[0].miktar)
                        hibeEsasTutar = referansFiyat * (decimal)hm.malzeme[0].miktar;
                }

                decimal tshibeEsasTutar = maliyet.hibeEsasTutarInsaat + maliyet.hibeEsasTutarMal + maliyet.ayniKatkiInsaat + maliyet.ayniKatkiMal;

                if (tshibeEsasTutar < (hibeEsasTutar * 80 / 100))
                {
                    string yetkiDeger = kullanan.YetkiTipiDegeriBul("Destek Başvuru Şube Müdürü");

                    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                        kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))
                        yetkiDeger = "1";

                    if (yetkiDeger != "1")
                        return new Sonuc(false, "Hibeye esas inşaat ve mal alımı giderleri toplamı, başvurudaki hibeye esas tutar olan " + hibeEsasTutar.ToString("N2") + " TL' nin %80 den az girilmiş. Tutarları kontrol ediniz.", string.Empty);
                }
                if (maliyet.hibeEsasTutarInsaat + maliyet.hibeEsasTutarMal > hibeEsasTutar)
                    return new Sonuc(false, "Hibeye esas inşaat ve mal alımı giderleri toplamı, başvurudaki hibeye esas tutar olan " + hibeEsasTutar.ToString("N2") + " TL' den büyük olamaz.", string.Empty);

                return new Sonuc();
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Arguman olarak verilen, hibe yatirim proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetReddedildi(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            HibeYatirim hm = new HibeYatirim();
            string hata = IslemKontrol(ve, kullanan, bilgi, ref hm, "Red");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            hm.redGerekce = bilgi.redGerekce;
            hm.durum = (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI;
            sonuc = to.KaydetReddedildi(hm);
            if (sonuc.islemSonuc)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI;
                tarihce.hibeKod = hm.kod;
                tarihce.aciklama = bilgi.redGerekce;
                tarihce.ipAdresi = bilgi.ipAdresi == "" ? kullanan.telefon : bilgi.ipAdresi;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "İşlem sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public Sonuc KaydetOnline(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            HibeYatirim hm = new HibeYatirim();
            string hata = IslemKontrol(ve, kullanan, bilgi, ref hm, "Online");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            hm.redGerekce = bilgi.redGerekce;
            hm.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
            sonuc = to.KaydetOnline(hm);
            if (sonuc.islemSonuc)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.hibeKod = hm.kod;
                tarihce.aciklama = bilgi.redGerekce;
                tarihce.ipAdresi = bilgi.ipAdresi == "" ? kullanan.telefon : bilgi.ipAdresi;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "İşlem sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public Sonuc BasvuruyuGeriCek(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            //if (bilgi.durum != 1)
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = "Sadece başvuru alındı durumundaki başvuruları geri çekebilirsiniz.";
            //    return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            HibeYatirim hm = new HibeYatirim();
            string hata = IslemKontrol(ve, kullanan, bilgi, ref hm, "");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            hm.redGerekce = bilgi.redGerekce;
            hm.durum = (int)ENUMHibeYatirimBasvuruDurum.GERICEKILDI;
            sonuc = to.BasvuruyuGeriCek(hm);
            if (sonuc.islemSonuc)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.GERICEKILDI;
                tarihce.hibeKod = hm.kod;
                tarihce.aciklama = bilgi.redGerekce;
                tarihce.ipAdresi = bilgi.ipAdresi == "" ? kullanan.telefon : bilgi.ipAdresi;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "İşlem sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public Sonuc KaydetKabulCalismasi(Kullanici kullanan, int etapNo, string ilKod, HibeYatirim[] bilgiler, int kabulCalismasi)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);
            if (etapNo <= 0)
                sonuc.hataStr = "Etap No boş bırakılamaz.<br>";
            if (string.IsNullOrEmpty(ilKod))
                sonuc.hataStr += "İl boş bırakılamaz.";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            //HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
            //tarihce.kullaniciKod = kullanan.kullaniciKodu;
            //tarihce.tarih = new TNSDateTime(DateTime.Now);
            //tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.YENI;

            foreach (HibeYatirim bilgi in bilgiler)
            {
                HibeYatirim hy = new HibeYatirim();
                string hata = IslemKontrol(ve, kullanan, bilgi, ref hy, "Kabul");
                if (hata != "")
                {
                    sonuc.hataStr = hata;
                    sonuc.islemSonuc = false;
                    break;
                }

                hy.kabulCalismasi = kabulCalismasi;
                sonuc = to.KaydetKabulCalismasi(hy);
                //if (sonuc.islemSonuc)
                //{
                //    tarihce = new HibeYatirimTarihce();
                //    tarihce.kullaniciKod = kullanan.kullaniciKodu;
                //    tarihce.tarih = new TNSDateTime(DateTime.Now);
                //    tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.YENI;
                //    tarihce.hibeKod = hy.kod;

                //    sonuc = to.KaydetTarihce(tarihce);
                //}
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public Sonuc KaydetUygunlukCalismasi(Kullanici kullanan, HibeYatirim[] bilgiler)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);

            if (bilgiler.Length < 0)
            {
                sonuc.hataStr = " Çalışma yapılacak yatırımlar seçilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            //HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
            //tarihce.kullaniciKod = kullanan.kullaniciKodu;
            //tarihce.tarih = new TNSDateTime(DateTime.Now);
            //tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.YENI;

            foreach (HibeYatirim bilgi in bilgiler)
            {
                //string hata = IslemKontrol(kullanan, bilgi, ref bilgi, "Kabul");
                //if (hata != "")
                //{
                //    sonuc.hataStr = hata;
                //    sonuc.islemSonuc = false;
                //    break;
                //}

                sonuc = to.KaydetUygunlukCalismasi(bilgi);
                //if (sonuc.islemSonuc)
                //{
                //    tarihce = new HibeYatirimTarihce();
                //    tarihce.kullaniciKod = kullanan.kullaniciKodu;
                //    tarihce.tarih = new TNSDateTime(DateTime.Now);
                //    tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.YENI;
                //    tarihce.hibeKod = hy.kod;

                //    sonuc = to.KaydetTarihce(tarihce);
                //}
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static bool ListeleBasvuruYapildi(VeriErisim ve, Kullanici kullanan, int kod)
        {
            int donenDeger = 0;
            ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));

            TABHibeYatirim to = new TABHibeYatirim(ve);

            IDataReader r = to.ListeleBasvuruYapildi(kod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    donenDeger = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                }
            }
            r.Close();

            ve.VeriErisimKapat();
            return donenDeger > 0;
        }


        public Sonuc KaydetBasvuruYapildi(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);

            if (bilgi.kod == 0)
            {
                sonuc.hataStr = " Çalışma yapılacak yatırımlar seçilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));

            string hata = TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, bilgi.basvuruNo);
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = "Hibe başvurusu online girişe kapatıldığı için işlem yapılamaz.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            ObjectArray hmler = Listele(ve, kullanan, new HibeYatirim { kod = bilgi.kod, online = true });
            if (hmler == null || hmler.objeler.Count == 0 || hmler.objeler.Count > 1)
            {
                sonuc.hataStr = " Çalışma yapılacak yatırımlar seçilmemiş.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            HibeYatirim hm = (HibeYatirim)hmler.objeler[0];
            if (hm.durum > 2)
            {
                sonuc.hataStr = "Bu başvuru işlem yapılması için uygun değildir.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            //Tüzel kişilik için ortak kaydı yapılmışmı kontrolü
            //sonuc = FirmaOrtakKaydiKontrolu(kullanan, hm);
            //if (!sonuc.islemSonuc)
            //{
            //    ve.VeriErisimKapat();
            //    return sonuc;
            //}

            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetCBasvuruYapildi(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private Sonuc FirmaOrtakKaydiKontrolu(Kullanici kullanan, HibeYatirim hm)
        {
            Sonuc sonuc = new Sonuc();

            TRMHibeYatirimBasvuruTablolari yb = new TRMHibeYatirimBasvuruTablolari();
            ObjectArray ortlar = yb.OrtaklarListele(kullanan, hm.kod);

            if (hm.hukukiStatu == (int)ENUMHibeYatirimHukukiStatu.TUZELKISI)
            {
                if (ortlar.objeler.Count == 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Tüzel kişilikler için Firma Ortakları bilgisinin girilmesi zorunludur. Bu bilgiyi, Başvuru formları ekranı, Şirket Ortakları bölümünden giriş yapabilirsiniz.";
                    return sonuc;
                }

                //Kontrol3
                //hm.tuzel ise kayıtlı ortakların hibe başvurusu (tamamlanmış) var mı?
                foreach (Ortaklar o in ortlar.objeler)
                {
                    HibeYatirim hy = new HibeYatirim();
                    hy.etapNo = hm.etapNo;
                    hy.kimlikNo = o.kimlikNo;
                    hy.durum = (int)ENUMHibeYatirimBasvuruDurum.YENI;

                    ObjectArray hListe = yb.OrtaklarListeleHibeKontrol(kullanan, hy);
                    if (hListe.objeler.Count > 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = "Şirket ortağı olarak kayıt edilen " + o.kimlikNo + " kimlik numaralı kişisinin, önceden başka bir hibe başvurusu alındığı için, bu hibe başvuru kaydı alınamaz. ";
                        return sonuc;
                    }
                }
            }

            //Kontrol 2
            //hm.kimlikNo ortaklar tablosunda mevcut mu? mevcut ise hibe başvuru tamamlanmış mı?
            Ortaklar ort2 = new Ortaklar();
            ort2.kimlikNo = hm.kimlikNo;
            ort2.satirNo = hm.etapNo;
            ObjectArray ortListe = yb.OrtaklarListeleKontrol(kullanan, ort2);
            if (ortListe.objeler.Count > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Başvurusu yapılmak istenilen " + hm.kimlikNo + " kimlik numaralı kişinin, daha önce başvuru kaydı alınmış olan başka bir hibe başvurusunda, şirket ortağı olduğu beyan edilmiştir. Bu nedenle bu kişinin hibe başvuru kaydı alınamaz.";
                return sonuc;
            }

            return sonuc;
        }

        public Sonuc BasvuruyuOnlineYap(Kullanici kullanan, int kod, string ip)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);

            if (kod == 0)
            {
                sonuc.hataStr = " Çalışma yapılacak yatırımlar seçilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            sonuc = to.BasvuruyuOnlineYap(kod);

            if (sonuc.islemSonuc)
            {
                if (sonuc.islemSonuc)
                {
                    HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                    tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                    tarihce.kullaniciAdi = kullanan.ADSOYAD;
                    tarihce.tarih = new TNSDateTime(DateTime.Now);
                    tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                    tarihce.aciklama = "Başvuru online durumuna çevrildi.";
                    tarihce.hibeKod = kod;
                    tarihce.ipAdresi = ip == "" ? kullanan.telefon : ip;

                    sonuc = to.KaydetTarihce(tarihce);
                }

                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc KaydetDegerlendirme(ref VeriErisim ve, Kullanici kullanan, HibeYatirim[] bilgiler)
        {
            if (bilgiler == null || bilgiler.Length == 0)
                return new Sonuc(false, "Hiçbir başvuru seçilmemiş.", string.Empty);

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            if (bilgiler[0].degerlendirmeDisi)
                bilgiler[0].durum = (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDIMERKEZ;

            Sonuc sonuc = to.KaydetDegerlendirme(bilgiler);
            if (sonuc.islemSonuc)
            {
                foreach (HibeYatirim item in bilgiler)
                {
                    HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                    tarihce.hibeKod = item.kod;
                    tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                    tarihce.kullaniciAdi = kullanan.ADSOYAD;
                    tarihce.tarih = new TNSDateTime(DateTime.Now);
                    if (bilgiler[0].durum == (int)ENUMHibeYatirimBasvuruDurum.ILDEKABULEDILENLER)
                        tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEIMZALAMADI;
                    else
                        tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDIMERKEZ;
                    tarihce.ipAdresi = kullanan.telefon;
                    tarihce.aciklama = item.degerlendirmeDisiGerekce;

                    sonuc = to.KaydetTarihce(tarihce);
                }
            }

            if (sonuc.islemSonuc && sonuc.kayitSay != bilgiler.Length)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Sadece kırmızı renk ile işaretlenmiş başvuruları seçebilirsiniz.";
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "İşlem sırasında hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Sorgula(VeriErisim ve, Kullanici kullanan, HibeYatirim kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc)
        {
            //if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI) && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            //    throw new Exception("Okuma yetkiniz bulunmamaktadır.");

            return Listele(ve, kullanan, kriter, ilkKayit, kayitSay, sortKolon, asc);
        }

        public ObjectArray Listele(VeriErisim ve, Kullanici kullanan, HibeYatirim kriter)
        {
            //if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI) && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            //    throw new Exception("Okuma yetkiniz bulunmamaktadır.");

            return Listele(ve, kullanan, kriter, 0, 0, string.Empty, true);
        }

        public static ObjectArray ListeleOncelikliForKabul(Kullanici kullanan, HibeYatirim kriter)
        {
            ObjectArray basvurular = new ObjectArray();
            if (kriter.etapNo <= 0 && kriter.kod == 0)
                basvurular.sonuc.hataStr = "Etap No boş bırakılamaz.<br>";
            //if (string.IsNullOrEmpty(kriter.ilKod))
            //    basvurular.sonuc.hataStr += "İl boş bırakılamaz.";
            if (!string.IsNullOrEmpty(basvurular.sonuc.hataStr))
            {
                basvurular.sonuc.islemSonuc = false;
                return basvurular;
            }

            basvurular = Listele(null, kullanan, kriter, 0, 0, "ListeleOncelikliForKabul", true, true);
            if (!basvurular.sonuc.islemSonuc || basvurular.objeler.Count == 0)
                return basvurular;

            int oncelikTur = kriter.yatirimOncelikTur;
            if (oncelikTur == 2) oncelikTur = 1;
            ObjectArray konular = new TRMHibeYatirimOncelik().Listele(kullanan, new HibeYatirimOncelik() { etapNo = kriter.etapNo, ilKod = kriter.ilKod, tur = oncelikTur });
            if (!konular.sonuc.islemSonuc || konular.objeler.Count == 0)
                return basvurular;
            //Konu önceliğine göre 5-3-2-1-1-1-1 şeklinde liste doldurulacak

            ObjectArray yeniListe = new ObjectArray();


            //bool bitti = true;
            bool bitti = false;////KOnu önceliği yapılmayacak 14.2 Etapta Melih o nedenle false yapıldı
            int konuSayac = 0;
            int eklenenecekBasvuru = 0;

            while (bitti)
            {
                konuSayac = 0;

                bitti = false;//Aşağıda ekleme yapılmaz ise while döndüsünden çıksın

                foreach (HibeYatirimOncelik konu in konular.objeler)
                {
                    if (konuSayac == 0) eklenenecekBasvuru = 5;
                    else if (konuSayac == 1) eklenenecekBasvuru = 3;
                    else if (konuSayac == 2) eklenenecekBasvuru = 2;
                    else if (konuSayac > 2) eklenenecekBasvuru = 1;

                    foreach (HibeYatirim basvuru in basvurular.objeler)
                    {
                        if (basvuru.paftaNo == "EKLENDI") continue;

                        if (basvuru.projeAltKonu.Equals(konu.konu.kod))
                        {
                            basvuru.paftaNo = "EKLENDI";
                            yeniListe.objeler.Add(basvuru);
                            bitti = true;
                            eklenenecekBasvuru--;
                        }

                        if (eklenenecekBasvuru == 0)
                            break;
                    }

                    konuSayac++;
                }
            }

            //basvurular = yeniListe;////KOnu önceliği yapılmayacak 14.2 Etapta Melih o nedenle kapatıldı

            #region Eski Yöntem -- konu önceliğine göre her konudan 1 kişi
            ////Konuya göre sıralama yapılıyor.
            //ArrayList siraliBasvurular = new ArrayList();
            //foreach (HibeYatirimOncelik konu in konular.objeler)
            //{
            //    int basvuruSay = 0;
            //    foreach (HibeYatirim basvuru in basvurular.objeler)
            //    {
            //        if (!basvuru.projeKonu.Equals(konu.konu.kod))
            //            continue;

            //        if (konu.siraNo + basvuruSay * konular.objeler.Count > siraliBasvurular.Count)
            //        {
            //            int objeSay = konu.siraNo + basvuruSay * konular.objeler.Count - siraliBasvurular.Count - 1;
            //            for (int i = 0; i <= objeSay; i++)
            //                siraliBasvurular.Add(null);
            //        }
            //        siraliBasvurular.RemoveAt(konu.siraNo - 1 + basvuruSay * konular.objeler.Count);
            //        siraliBasvurular.Insert(konu.siraNo - 1 + basvuruSay * konular.objeler.Count, basvuru);
            //        basvuruSay++;
            //    }
            //}

            ////Değerlendirme dışı olan başvurular konu sıralaması bozulmadan değerlendirmede olanların arkasına atılıyor.
            //int ilkTasinanIndis = -1;
            //for (int i = 0; i < siraliBasvurular.Count && (ilkTasinanIndis == -1 || i < ilkTasinanIndis); i++)
            //{
            //    HibeYatirim basvuru = (HibeYatirim)siraliBasvurular[i];
            //    if (basvuru == null || !basvuru.degerlendirmeDisi)
            //        continue;

            //    siraliBasvurular[i] = null;
            //    siraliBasvurular.Add(basvuru);
            //    if (ilkTasinanIndis == -1)
            //        ilkTasinanIndis = siraliBasvurular.Count;
            //}

            ////Dizide null olan objeler temizleniyor.
            //basvurular.objeler = new TNSCollection();
            //foreach (HibeYatirim basvuru in siraliBasvurular)
            //{
            //    if (basvuru == null)
            //        continue;
            //    basvurular.objeler.Add(basvuru);
            //}
            #endregion

            double butce = TRMButce.ButceBul(kullanan, new Butce() { etapNo = kriter.etapNo, ilKod = kriter.ilKod, tip = (int)ENUMHibeTur.EKONOMIKYATIRIM, yatirimOncelikTur = kriter.yatirimOncelikTur });
            decimal dagitilanTutar = DagitilanTutarDondur(kullanan, kriter);
            basvurular.sonuc.anahtar = butce.ToString() + ";" + dagitilanTutar.ToString();
            return basvurular;
        }

        public static ObjectArray ListeleEksikBelge(Kullanici kullanan, HibeYatirim kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            if (kriter.etapNo <= 0)
                bilgi.sonuc.hataStr = "Etap No boş bırakılamaz.<br>";
            if (string.IsNullOrEmpty(kriter.ilKod))
                bilgi.sonuc.hataStr += "İl boş bırakılamaz.";
            if (!string.IsNullOrEmpty(bilgi.sonuc.hataStr))
            {
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            IDataReader r = to.ListeleEksikBelge(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeYatirim f = new HibeYatirim();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                    f.yil = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YIL, 0);
                    f.basvuruNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUNO, string.Empty);
                    f.ilKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ILKOD, string.Empty);
                    f.ilceKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ILCEKOD, string.Empty);
                    f.koyKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOYKOD, 0);
                    f.kasaba = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KASABA, string.Empty);
                    f.hukukiStatu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HUKUKISTATU, 0);
                    f.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KIMLIKNO, string.Empty);
                    f.vergiDaire = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.VERGIDAIRE, string.Empty);
                    f.ad = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.AD, string.Empty);
                    f.evrakNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EVRAKNO, string.Empty);
                    f.evrakTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EVRAKTARIH);
                    f.adres = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ADRES, string.Empty);
                    f.telefon = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.TELEFON, string.Empty);
                    f.faks = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FAKS, string.Empty);
                    f.ePosta = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EPOSTA, string.Empty);
                    f.bankaSubeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BANKASUBEKOD, string.Empty);
                    f.durum = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DURUM, 0);
                    f.sozlesmeTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMETARIH);
                    f.faturaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FATURANO, string.Empty);
                    f.faturaTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FATURATARIH);
                    f.soyad = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOYAD, string.Empty);
                    f.babaAd = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BABAAD, string.Empty);
                    f.dogumTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DOGUMTARIH);
                    f.yerindeTespitTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YERINDETESPITTARIH);
                    f.etapNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ETAPNO, 0);

                    f.projeNitelik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJENITELIK, 0);
                    f.projeKonu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEKONU, "");
                    f.projeAltKonu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEALTKONU, "");
                    f.projeAdi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEADI, string.Empty);
                    f.sozlesmeMaliyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMEMALIYET, decimal.Zero);
                    f.basvuruMaliyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUMALIYET, decimal.Zero);
                    f.hibeTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HIBETUTAR, decimal.Zero);
                    f.hibeEsasTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HIBEESASTUTAR, decimal.Zero);
                    f.ayniKatki = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.AYNIKATKI, decimal.Zero);
                    f.toplamTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.TOPLAMTUTAR, decimal.Zero);
                    f.anket1 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET1, 0);
                    f.anket2 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET2, 0);
                    f.anket3 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET3, 0);
                    f.anket4 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET4, 0);
                    f.anket3Puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET3PUAN, 0);
                    f.anket4Puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET4PUAN, 0);
                    f.puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PUAN, (double)0);
                    f.konuNitelikUyusma = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KONUNITELIKUYUSMA, 0) == 1 ? true : false;
                    f.degerlendirmeDisi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISI, 0) == 1 ? true : false;
                    f.degerlendirmeDisiGerekce = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISIGEREKCE, string.Empty);
                    f.kabulCalismasi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KABULCALISMASI, 0);
                    f.onlineTuzeleBasvuran = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ONLINETUZELBASVURUKIMLIKNO, string.Empty);
                    f.pNitelik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PNITELIK, 0);
                    f.mevki = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.MEVKI, string.Empty);
                    f.paftaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PAFTANO, string.Empty);
                    f.adaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ADANO, string.Empty);
                    f.parselNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PARSELNO, string.Empty);
                    f.koordinat = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOORDINAT, string.Empty);
                    f.ozet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.OZET, string.Empty);
                    f.hedef = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HEDEF, string.Empty);
                    f.istihdamSayisi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ISTIHDAM, 0);
                    f.dilekceNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DILEKCENO, "");
                    f.eskiEtapdaVarmi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ESKIETAPDAVARMI, 0);
                    f.basvuruYapildi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUYAPILDI, 0);
                    f.altYapiGrup = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTYAPIGRUP, string.Empty);
                    f.cinsiyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.CINSIYET, string.Empty);
                    f.engelli = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ENGELLI, string.Empty);
                    f.organik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ORGANIK, string.Empty);
                    f.yas = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YAS, 0);
                    f.altKonuDetayKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD, "");

                    f.ilAd = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD + 1, string.Empty);
                    f.ilceAd = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD + 2, string.Empty);

                    f.eksikBelgeAdlari = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD + 3, string.Empty);
                    string[] parca = f.eksikBelgeAdlari.Split('^');
                    f.eksikBelgeAdet = parca.Length;
                    f.eksikBelgeAdlari = f.eksikBelgeAdlari.Replace("^", "; ");


                    bilgi.objeler.Add(f);
                }
            }
            r.Close();

            //Yukarıda zaten bu bilgiler geliyor
            //if (bilgi.objeler.Count == 1)
            //{
            //    HibeYatirim hy = (HibeYatirim)bilgi.objeler[0];
            //    hy.maliyetBilgileri = new HibeYatirimMaliyet[1];
            //    hy.maliyetBilgileri[0] = ListeleMaliyet(to, hy.kod, (ENUMHibeYatirimBasvuruDurum)hy.durum);
            //}

            ve.VeriErisimKapat();
            return bilgi;
        }

        public static decimal DagitilanTutarDondur(Kullanici kullanan, HibeYatirim kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            decimal dagitilanTutar = to.DagitilanTutarDondur(kriter);
            ve.VeriErisimKapat();
            return dagitilanTutar;
        }

        private static string[,] orderByKolon = { { "kod", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.KOD] },
                                                { "basvuruNo", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.BASVURUNO] },
                                                { "kimlikNo", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.KIMLIKNO] },
                                                { "ad", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.AD] + ", " + TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.SOYAD] },
                                                { "yil", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.YIL] },
                                                { "etapNo", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.ETAPNO] },
                                                { "il", "ORGCOGRAFYA.AD" },
                                                { "ilce", "ORGCOGRAFYA_1.AD" },
                                                { "koy", TABKoy.TABLOAD + "." + TABKoy.ALANLAR[(int)TABKoy.ENUMAlanlar.ADI] },
                                                { "evrakTarih", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.EVRAKTARIH] },
                                                { "sozlesmeTarih", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.SOZLESMETARIH] },
                                                { "durum", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.DURUM] },
                                                { "evrakNo", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.EVRAKNO] },
                                                { "nitelik", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.PROJENITELIK] },
                                                { "konu", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.PROJEKONU] },
                                                { "AltKonu", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.PROJEALTKONU] },
                                                { "projeAdi", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.PROJEADI] },
                                                { "sozlesmeMaliyet", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.SOZLESMEMALIYET] },
                                                { "basvuruMaliyet", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.BASVURUMALIYET] },
                                                { "hibeTutar", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.HIBETUTAR] },
                                                { "hibeEsasTutar", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.HIBEESASTUTAR] },
                                                { "ayniKatki", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.AYNIKATKI] },
                                                { "toplamTutar", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.TOPLAMTUTAR] },
                                                { "puan", TABHibeYatirim.TABLOAD + "." + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.PUAN] }};

        private static string OrderByKolonBul(string sortKolon)
        {
            if (string.IsNullOrEmpty(sortKolon))
                return string.Empty;

            for (int i = 0; i < orderByKolon.Length / 2; i++)
            {
                if (orderByKolon[i, 0].Equals(sortKolon))
                    return orderByKolon[i, 1];
            }

            return string.Empty;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe yatirim bilgisini veritabanından okur 
        /// ve HibeYatirim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hibe yatirim listesini HibeYatirim nesne koleksiyonu olarak tutan nesne</returns>
        private static ObjectArray Listele(VeriErisim ve, Kullanici kullanan, HibeYatirim kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc, bool gecenler = false)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);

            if (!kriter.online)
            {
                if (!Yetki.YetkiVarMiOkumaya(kullanan, kriter.ilKod, ENUMHibeTur.EKONOMIKYATIRIM))
                {
                    sonuc.hataStr = "Okuma yetkiniz bulunmamaktadır.";
                    sonuc.islemSonuc = false;
                    bilgi.sonuc = sonuc;
                    ve.VeriErisimKapat();
                    return bilgi;
                }
            }

            int aktifEtapNo = TRMHibeYatirimYonetim.HibeYatirimAktifEtapNo(ref ve);
            //İller Aktif etabı görmesin
            //if (kriter.etapNo == 16)
            //{
            //    if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
            //        kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI) ||
            //        kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUUZMAN) ||
            //        kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUMERKEZ) ||
            //        kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEBASKAN) ||
            //        kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEVERIGIRIS)))
            //    {
            //        if (kriter.adaNo != "1973")//listeleme HibeOnlineDurumaGetir.aspx sayfasından yapılıyorsa bu kontrol çalışmasın Melih 27.12.2022
            //        {
            //            sonuc.hataStr = "Okuma yetkiniz bulunmamaktadır.";
            //            sonuc.islemSonuc = false;
            //            bilgi.sonuc = sonuc;
            //            ve.VeriErisimKapat();
            //            return bilgi;
            //        }
            //    }
            //}

            if (kriter.kod == 0)//Tek kayıt çağrısında bulunmuyor ise
            {
                //İl kullanıcısı yetkisi için
                //*****************************************************************************
                string[] yetkiliOlduguIller = TNS.TRM.Yetki.YetkiliOlduguIlleriGetir(kullanan);
                if (kriter.ilKod.Trim() == "")
                {
                    foreach (string il in yetkiliOlduguIller)
                    {
                        if (il.Trim() == "")
                            continue;
                        if (kriter.ilKod != "") kriter.ilKod += ";";
                        kriter.ilKod += il;
                    }
                }
                //*****************************************************************************
            }


            //kriter.kimlikNo eğer dolu geldiyse
            if (kriter.kimlikNo != "")
            {
                ObjectArray yatirimciListe = ListeleYatirimciKimlikNo(ve, kullanan, kriter.kimlikNo);

                string kimlikNolar = "";
                foreach (HibeYatirimYatirimciKimlikNo item in yatirimciListe.objeler)
                {
                    if (kimlikNolar.IndexOf(item.eskiKimlikNo) < 0)
                    {
                        if (kimlikNolar != "") kimlikNolar += ",";
                        kimlikNolar += item.eskiKimlikNo;
                    }

                    if (kimlikNolar.IndexOf(item.yeniKimlikNo) < 0)
                    {
                        if (kimlikNolar != "") kimlikNolar += ",";
                        kimlikNolar += item.yeniKimlikNo;
                    }
                }
                if (kimlikNolar != "")
                    kriter.kimlikNo = kimlikNolar;
            }

            StreamWriter writeFile = null;
            if (!string.IsNullOrWhiteSpace(kriter.listeYazdir))
            {
                if (File.Exists(kriter.listeYazdir))
                    File.Delete(kriter.listeYazdir);

                writeFile = new StreamWriter(kriter.listeYazdir, false);
            }


            IDataReader r = to.Listele(kriter, sortKolon.Equals("ListeleOncelikliForKabul") ? sortKolon : OrderByKolonBul(sortKolon), asc, gecenler);

            if (to.Hata() == "")
            {

                if (ilkKayit < 0)
                    ilkKayit = 0;
                if (kayitSay < 0)
                    kayitSay = 0;
                int sayac = -1;
                while (r.Read())
                {
                    sayac++;
                    if (sayac < ilkKayit)
                        continue;
                    if (sayac != 0 && kayitSay != 0 && sayac == ilkKayit + kayitSay)
                        break;

                    HibeYatirim f = new HibeYatirim();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                    f.yil = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YIL, 0);
                    f.basvuruNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUNO, string.Empty);
                    f.ilKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ILKOD, string.Empty);
                    f.ilceKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ILCEKOD, string.Empty);
                    f.koyKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOYKOD, 0);
                    f.kasaba = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KASABA, string.Empty);
                    f.hukukiStatu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HUKUKISTATU, 0);
                    f.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KIMLIKNO, string.Empty);
                    f.vergiDaire = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.VERGIDAIRE, string.Empty);
                    f.ad = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.AD, string.Empty);
                    f.evrakNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EVRAKNO, string.Empty);
                    f.evrakTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EVRAKTARIH);
                    f.adres = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ADRES, string.Empty);
                    f.telefon = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.TELEFON, string.Empty);
                    f.faks = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FAKS, string.Empty);
                    f.ePosta = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EPOSTA, string.Empty);
                    f.bankaSubeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BANKASUBEKOD, string.Empty);
                    f.durum = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DURUM, 0);
                    f.sozlesmeTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMETARIH);
                    f.faturaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FATURANO, string.Empty);
                    f.faturaTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FATURATARIH);
                    f.soyad = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOYAD, string.Empty);
                    f.babaAd = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BABAAD, string.Empty);
                    f.dogumTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DOGUMTARIH);
                    f.yerindeTespitTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YERINDETESPITTARIH);
                    f.etapNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ETAPNO, 0);

                    f.projeNitelik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJENITELIK, 0);
                    f.projeKonu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEKONU, "");
                    f.projeAltKonu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEALTKONU, "");
                    f.projeAdi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEADI, string.Empty);
                    f.sozlesmeMaliyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMEMALIYET, decimal.Zero);
                    f.basvuruMaliyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUMALIYET, decimal.Zero);
                    f.hibeTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HIBETUTAR, decimal.Zero);
                    f.hibeEsasTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HIBEESASTUTAR, decimal.Zero);
                    f.ayniKatki = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.AYNIKATKI, decimal.Zero);
                    f.toplamTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.TOPLAMTUTAR, decimal.Zero);
                    f.anket1 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET1, 0);
                    f.anket2 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET2, 0);
                    f.anket3 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET3, 0);
                    f.anket4 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET4, 0);
                    f.anket3Puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET3PUAN, 0);
                    f.anket4Puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET4PUAN, 0);
                    f.puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PUAN, (double)0);
                    f.konuNitelikUyusma = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KONUNITELIKUYUSMA, 0) == 1 ? true : false;
                    f.degerlendirmeDisi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISI, 0) == 1 ? true : false;
                    f.degerlendirmeDisiGerekce = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISIGEREKCE, string.Empty);
                    f.kabulCalismasi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KABULCALISMASI, 0);
                    f.onlineTuzeleBasvuran = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ONLINETUZELBASVURUKIMLIKNO, string.Empty);
                    f.pNitelik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PNITELIK, 0);
                    f.mevki = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.MEVKI, string.Empty);
                    f.paftaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PAFTANO, string.Empty);
                    f.adaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ADANO, string.Empty);
                    f.parselNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PARSELNO, string.Empty);
                    f.koordinat = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOORDINAT, string.Empty);
                    f.ozet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.OZET, string.Empty);
                    f.hedef = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HEDEF, string.Empty);
                    f.istihdamSayisi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ISTIHDAM, 0);
                    f.dilekceNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DILEKCENO, "");
                    f.eskiEtapdaVarmi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ESKIETAPDAVARMI, 0);
                    f.basvuruYapildi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUYAPILDI, 0);
                    f.altYapiGrup = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTYAPIGRUP, "");
                    f.cinsiyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.CINSIYET, "");
                    f.engelli = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ENGELLI, "");
                    f.organik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ORGANIK, "");
                    f.yas = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YAS, 0);
                    f.altKonuDetayKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD, "");

                    int sonKolon = ((int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD) + 1;

                    f.projeNitelikAdi = VeriErisim.DegerAl(r, sonKolon, "");
                    f.projeKonuAdi = VeriErisim.DegerAl(r, sonKolon + 1, "");
                    f.projeAltKonuAdi = VeriErisim.DegerAl(r, sonKolon + 2, "");

                    f.koyAd = VeriErisim.DegerAl(r, sonKolon + 3, "");
                    f.ilAd = VeriErisim.DegerAl(r, sonKolon + 4, "");
                    f.ilceAd = VeriErisim.DegerAl(r, sonKolon + 5, "");

                    f.odenenInsaat = VeriErisim.DegerAl(r, sonKolon + 6, (decimal)0);
                    f.odenenMal = VeriErisim.DegerAl(r, sonKolon + 7, (decimal)0);

                    sonKolon = sonKolon + 8;
                    HibeYatirimSozlesme hys = new HibeYatirimSozlesme();
                    hys.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.HIBEKOD + sonKolon, 0);
                    hys.sozlesmeTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.SOZLESMETARIH + sonKolon);
                    hys.projeBasTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.PROJEBASTARIH + sonKolon);
                    hys.projeBitTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.PROJEBITTARIH + sonKolon);
                    hys.nihaiRaporTarihi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.NIHAIRAPORTARIHI + sonKolon);
                    hys.istihdam = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.ISTIHDAM + sonKolon, 0);
                    hys.uretimKapasitesi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.URETIMKAPASITESI + sonKolon, (decimal)0);
                    hys.teminatTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTUTAR + sonKolon, (decimal)0);
                    hys.teminatTur = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTUR + sonKolon, 0);
                    hys.teminatTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTARIH + sonKolon);
                    hys.bankaSubeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.BANKASUBEKOD + sonKolon, "");
                    hys.kapasiteBirimKodu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.KAPASITEBIRIM + sonKolon, "");
                    hys.istihdamKadin = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.ISTIHDAMKADIN + sonKolon, 0);
                    hys.durum = (ENUMHibeYatirimSozlesmeDurum)VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.DURUM + sonKolon, 0);
                    hys.yapiKimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.YAPIKIMLIKNO + sonKolon, "");
                    f.sozlesmeBilgileri = hys;

                    sonKolon = sonKolon + 15;
                    HibeYatirimMaliyet hym = new HibeYatirimMaliyet();
                    hym.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEKOD + sonKolon, 0);
                    hym.hibeEsasTutarInsaat = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARINSAAT + sonKolon, (decimal)0);
                    hym.hibeEsasTutarMal = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARMAL + sonKolon, (decimal)0);
                    hym.hibeEsasTutarToplam = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARTOPLAM + sonKolon, (decimal)0);
                    hym.ayniKatkiInsaat = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIINSAAT + sonKolon, (decimal)0);
                    hym.ayniKatkiMal = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIMAL + sonKolon, (decimal)0);
                    hym.ayniKatkiToplam = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKITOPLAM + sonKolon, (decimal)0);
                    hym.projeToplamTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.PROJETOPLAMTUTAR + sonKolon, (decimal)0);
                    hym.hibeTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBETUTAR + sonKolon, (decimal)0);
                    f.maliyetBilgileri = new HibeYatirimMaliyet[1];
                    f.maliyetBilgileri[0] = hym;

                    sonKolon = sonKolon + 11;

                    f.sonOdemeYapilanTarih = VeriErisim.DegerAl(r, sonKolon);
                    f.eskiEtapdaSozlesmesiVar = VeriErisim.DegerAl(r, sonKolon + 1, 0);

                    if (sortKolon == "ListeleOncelikliForKabul")
                    {
                        f.eskiEtapdaSozlesmesiDevamEdiyorStr = ListeleDetaylar(VeriErisim.DegerAl(r, sonKolon + 2, ""), 1);
                        f.eskiEtapdaSozlesmesiBitti1Str = ListeleDetaylar(VeriErisim.DegerAl(r, sonKolon + 3, ""), 1);
                        //f.eskiEtapdaSozlesmesiBitti2Str = ListeleDetaylar(VeriErisim.DegerAl(r, sonKolon + 4, ""), 1);
                        f.eskiEtapdaSozlesmesiIptalStr = ListeleDetaylar(VeriErisim.DegerAl(r, sonKolon + 5, ""), 1);
                        f.eskiEtapdaSozlesmesiImzalamayanStr = ListeleDetaylar(VeriErisim.DegerAl(r, sonKolon + 6, ""), 1);
                        //f.eskiEtapdaSozlesmesiOzelStr = ListeleDetaylar(VeriErisim.DegerAl(r, sonKolon + 7, ""), 1);
                        f.eskiEtapdaSon5YilOrtakStr = ListeleDetaylar(VeriErisim.DegerAl(r, sonKolon + 8, ""), 1);

                        f.eskiEtapdaSozlesmesiDevamEdiyor = f.eskiEtapdaSozlesmesiDevamEdiyorStr.ObjeSayisi > 0 ? 1 : 0;//Önceki etapta sözleşmesi devam edenler (KIRMIZI)
                        f.eskiEtapdaSozlesmesiBitti1 = f.eskiEtapdaSozlesmesiBitti1Str.ObjeSayisi > 0 ? 1 : 0;//Önceki etapta sözleşmesi bitenler (3 etap öncesine kadar) (MOR)
                        //f.eskiEtapdaSozlesmesiBitti2 = f.eskiEtapdaSozlesmesiBitti2Str.ObjeSayisi > 0 ? 1 : 0;//Önceki etapta sözleşmesi bitenler (4 etap öncesi) (MAVİ)
                        f.eskiEtapdaSozlesmesiIptal = f.eskiEtapdaSozlesmesiIptalStr.ObjeSayisi > 0 ? 1 : 0;//Önceki etaplarda sözleşme imzalayıp iptal edenler (KAHVE)
                        f.eskiEtapdaSozlesmesiImzalamayan = f.eskiEtapdaSozlesmesiImzalamayanStr.ObjeSayisi > 0 ? 1 : 0;//Önceki etaplarda sözleşme başvuru kabul ama imzalamayanlar (YEŞİL)
                        //f.eskiEtapdaSozlesmesiOzel = f.eskiEtapdaSozlesmesiOzelStr.ObjeSayisi > 0 ? 1 : 0;//8 etapda başvuru yapanlar (TURKUAZ)
                        f.eskiEtapdaSon5YilOrtak = f.eskiEtapdaSon5YilOrtakStr.ObjeSayisi > 0 ? 1 : 0;//Önceki etaplarda sözleşme başvuru kabul edilmiş fakat sözleşme imzalamayanlar (KAHVE)
                    }

                    f.sirketOrtagi = VeriErisim.DegerAl(r, sonKolon + 9, 0);
                    f.basvuruTarihi = VeriErisim.DegerAl(r, sonKolon + 10);

                    f.ayniAdresStr = ListeleDetaylar(VeriErisim.DegerAl(r, sonKolon + 12, ""), 1);
                    f.altKonuDetayKodGorunen = VeriErisim.DegerAl(r, sonKolon + 13, "");
                    f.altKonuDetayAdi = VeriErisim.DegerAl(r, sonKolon + 14, "");
                    f.uygulamaSozlesmeMal = VeriErisim.DegerAl(r, sonKolon + 15, 0.0);
                    f.uygulamaSozlesmeInsaat = VeriErisim.DegerAl(r, sonKolon + 16, 0.0);


                    //if (f.ayniAdresStr.objeler.Count == 1) f.ayniAdresStr = new ObjectArray();//sadece kendisi ise boş olsun
                    f.ayniAdres = f.ayniAdresStr.ObjeSayisi > 0 ? 1 : 0;//Aynı Adrese sahip olanlar

                    if (f.cinsiyet == "" && f.etapNo > 15) f.cinsiyet = "Firma";

                    //Malzemeler
                    ObjectArray mler = ListeleDetaylar(VeriErisim.DegerAl(r, sonKolon + 11, ""), 2);
                    List<YatirimMalzeme> ym = new List<YatirimMalzeme>();
                    foreach (YatirimMalzeme item in mler.objeler)
                    {
                        ym.Add(item);
                    }
                    f.malzeme = ym.ToArray();
                    //////////////////////////////////////////////////////////////////////////////////

                    bool yetki = false;

                    if (kriter.kod > 0)
                    {
                        if (kriter.online)
                            yetki = true;
                        else
                        {
                            //Eğer tek bir kaydı görmek istiyor ise (Başvuru ekranı gibi)
                            if (TNS.TRM.Yetki.YetkiVarMiOkumaya(kullanan, f.ilKod, ENUMHibeTur.EKONOMIKYATIRIM))
                                yetki = true;
                        }
                    }
                    else
                        yetki = true;


                    if (kriter.maskele)
                        f.kimlikNo = Arac.MaskeleTC(f.kimlikNo);


                    if (yetki)
                        bilgi.objeler.Add(f);

                    if (!string.IsNullOrWhiteSpace(kriter.listeYazdir))
                        ListeYazdir(kriter.listeYazdir, f, writeFile);
                }
                r.Close();
            }

            if (!string.IsNullOrWhiteSpace(kriter.listeYazdir))
            {
                bilgi = new ObjectArray();

                writeFile.Close();
            }



            if (kriter.kod > 0)
            {
                string altYapiGrupKod = "";

                IDataReader r2 = to.ListeleSoru(kriter.kod);
                if (to.Hata() == "")
                {
                    List<HibeYatirimSoru> sorular = new List<HibeYatirimSoru>();
                    while (r2.Read())
                    {
                        HibeYatirimSoru soru = new HibeYatirimSoru();
                        soru.hibeKod = VeriErisim.DegerAl(r2, (int)TABHibeYatirim.ENUMAlanlarSoru.HIBEKOD, 0);
                        soru.soruKod = VeriErisim.DegerAl(r2, (int)TABHibeYatirim.ENUMAlanlarSoru.SORUKOD, "");
                        soru.deger = VeriErisim.DegerAl(r2, (int)TABHibeYatirim.ENUMAlanlarSoru.DEGER, 0);

                        sorular.Add(soru);
                    }
                    foreach (HibeYatirim item in bilgi.objeler)
                    {
                        altYapiGrupKod = item.altYapiGrup;
                        item.sorular = sorular.ToArray();
                    }
                    r2.Close();
                }

                if (altYapiGrupKod == "A")
                {
                    YatirimMalzeme us = new YatirimMalzeme();
                    us.hibeKod = kriter.kod;
                    YatirimMalzeme[] malzemeler = TRMHibeYatirimMalzeme.Listele(ve, kullanan, us);
                    foreach (HibeYatirim item in bilgi.objeler)
                    {
                        item.malzeme = malzemeler;
                    }
                }

            }

            //foreach (HibeYatirim item in bilgi.objeler)
            //{
            //    OrtakFonksiyonlar.HataStrYaz(item.basvuruNo + " " + item.ad + item.soyad + item.puan);
            //}

            //Yukarıda zaten bu bilgiler geliyor
            //if (bilgi.objeler.Count == 1)
            //{
            //    HibeYatirim hy = (HibeYatirim)bilgi.objeler[0];
            //    hy.maliyetBilgileri = new HibeYatirimMaliyet[1];
            //    hy.maliyetBilgileri[0] = ListeleMaliyet(to, hy.kod, (ENUMHibeYatirimBasvuruDurum)hy.durum);
            //}

            ve.VeriErisimKapat();
            return bilgi;
        }

        private static void ListeYazdir(string listeYazdir, HibeYatirim hy, StreamWriter writeFile)
        {
            string bilgi = "";

            bilgi += hy.kod;
            bilgi += "|" + TNS.TRM.Arac.EtapAdiVer(ENUMHibeTur.EKONOMIKYATIRIM, hy.etapNo);
            bilgi += "|" + hy.ad;
            bilgi += "|" + hy.soyad;
            bilgi += "|" + hy.kimlikNo;
            bilgi += "|" + (hy.hukukiStatu == 1 ? "Gerçek Kişi" : "Tüzel Kişi");
            bilgi += "|" + (string.IsNullOrWhiteSpace(hy.altYapiGrup) ? "Yatirim" : hy.altYapiGrup);
            bilgi += "|" + hy.basvuruNo;
            bilgi += "|" + hy.projeAdi;
            bilgi += "|" + hy.projeNitelikAdi;
            bilgi += "|" + hy.projeKonuAdi;
            bilgi += "|" + hy.projeAltKonuAdi;
            bilgi += "|" + hy.ePosta;
            bilgi += "|" + hy.telefon;
            bilgi += "|" + hy.cinsiyet;
            bilgi += "|" + hy.dogumTarih.Oku().ToString();
            bilgi += "|" + hy.durum;
            bilgi += "|" + (hy.pNitelik == 1 ? "Avan" : "Tatbikat");
            bilgi += "|" + hy.basvuruMaliyet;
            bilgi += "|" + hy.hibeEsasTutar;
            bilgi += "|" + hy.ayniKatki;
            bilgi += "|" + hy.hibeTutar;
            bilgi += "|" + hy.ilAd;
            bilgi += "|" + hy.ilceAd;
            bilgi += "|" + hy.koyAd;
            bilgi += "|" + hy.istihdamSayisi;
            bilgi += "|" + hy.kabulCalismasi;
            bilgi += "|" + hy.odenenInsaat;
            bilgi += "|" + hy.odenenMal;
            bilgi += "|" + hy.sozlesmeBilgileri.nihaiRaporTarihi.ToString();
            bilgi += "|" + hy.sozlesmeBilgileri.durum.ToString();
            bilgi += "|" + hy.sozlesmeBilgileri.istihdam;
            bilgi += "|" + hy.sozlesmeBilgileri.istihdamKadin;
            bilgi += "|" + hy.sozlesmeBilgileri.sozlesmeTarih.ToString();
            bilgi += "|" + hy.sozlesmeBilgileri.teminatTutar;
            bilgi += "|" + hy.sozlesmehibeEsasTutar;
            bilgi += "|" + hy.sozlesmeMaliyet;
            bilgi += "|" + hy.sozlesmeBilgileri.teminatTarih.ToString();
            bilgi += "|" + hy.sozlesmeBilgileri.teminatTur;

            bilgi += "|" + hy.maliyetBilgileri[0].hibeEsasTutarInsaat;
            bilgi += "|" + hy.maliyetBilgileri[0].ayniKatkiInsaat;
            bilgi += "|" + hy.maliyetBilgileri[0].hibeEsasTutarMal;
            bilgi += "|" + hy.maliyetBilgileri[0].ayniKatkiMal;
            bilgi += "|" + hy.maliyetBilgileri[0].hibeEsasTutarToplam;
            bilgi += "|" + hy.maliyetBilgileri[0].ayniKatkiToplam;
            bilgi += "|" + hy.maliyetBilgileri[0].hibeTutar;
            bilgi += "|" + hy.maliyetBilgileri[0].projeToplamTutar;

            bilgi += "|" + hy.anket1;
            bilgi += "|" + hy.anket2;
            bilgi += "|" + hy.anket3;
            bilgi += "|" + hy.anket4;
            bilgi += "|" + hy.anket3Puan;
            bilgi += "|" + hy.anket4Puan;
            bilgi += "|" + hy.puan;
            bilgi += "|" + hy.engelli;
            bilgi += "|" + hy.organik;
            bilgi += "|" + hy.sonOdemeYapilanTarih.ToString();

            string malzemeGrupAdi = "";
            string malzemeAdi = "";
            string malzemeTurAdi = "";
            foreach (var item in hy.malzeme)
            {
                malzemeGrupAdi = item.malzemeAnaGrupAdi;
                malzemeTurAdi = item.malzemeTurAdi;
                malzemeAdi = item.malzeme.ad;
            }

            bilgi += "|" + malzemeGrupAdi;
            bilgi += "|" + malzemeTurAdi;
            bilgi += "|" + malzemeAdi;
            if (!hy.basvuruTarihi.isNull)
                bilgi += "|" + hy.basvuruTarihi.Oku();
            else
                bilgi += "|";

            bilgi += "|" + hy.yas;
            bilgi += "|" + hy.sozlesmeBilgileri.yapiKimlikNo;
            bilgi += "|" + hy.altKonuDetayAdi;
            bilgi += "|" + hy.altKonuDetayKodGorunen;

            bilgi += "|" + hy.uygulamaSozlesmeMal;
            bilgi += "|" + hy.uygulamaSozlesmeInsaat;


            //OrtakFonksiyonlar.DosyayaYaz(listeYazdir, bilgi, false);
            writeFile.WriteLine(bilgi);
        }

        private static ObjectArray ListeleDetaylar(string okunanXML, int detayTip)
        {
            ObjectArray pVAN = new ObjectArray();
            if (string.IsNullOrWhiteSpace(okunanXML)) return pVAN;

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                while (r.Read())
                {
                    if (detayTip == 1)
                        pVAN.objeler.Add(DetayOku(r));
                    if (detayTip == 2)
                        pVAN.objeler.Add(MalzemeOku(r));
                }
                r.Close();
            }
            return pVAN;
        }
        private static HibeYatirim DetayOku(IDataReader dr)
        {
            HibeYatirim detay = new HibeYatirim();
            detay.kod = VeriErisim.DegerAl(dr, 0, 0);
            detay.basvuruNo = VeriErisim.DegerAl(dr, 1, "");
            detay.kimlikNo = VeriErisim.DegerAl(dr, 2, "");
            detay.ad = VeriErisim.DegerAl(dr, 3, "");
            detay.soyad = VeriErisim.DegerAl(dr, 4, "");
            detay.etapNo = VeriErisim.DegerAl(dr, 5, 0);
            detay.projeAdi = VeriErisim.DegerAl(dr, 6, "");

            return detay;
        }

        private static YatirimMalzeme MalzemeOku(IDataReader dr)
        {
            YatirimMalzeme detay = new YatirimMalzeme();
            detay.malzeme.kod = VeriErisim.DegerAl(dr, 0, "");
            detay.malzemeAnaGrupAdi = VeriErisim.DegerAl(dr, 1, "");
            detay.malzemeTurAdi = VeriErisim.DegerAl(dr, 2, "");
            detay.malzeme.ad = VeriErisim.DegerAl(dr, 3, "");

            return detay;
        }


        public static ObjectArray ListeleMalzeme(VeriErisim ve, Kullanici kullanan, HibeYatirim kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc, bool gecenler = false)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            if (!Yetki.YetkiVarMiOkumaya(kullanan, kriter.ilKod, ENUMHibeTur.EKONOMIKYATIRIM))
            {
                sonuc.hataStr = "Okuma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                bilgi.sonuc = sonuc;
                ve.VeriErisimKapat();
                return bilgi;
            }

            int aktifEtapNo = TRMHibeYatirimYonetim.HibeYatirimAktifEtapNo(ref ve);

            if (kriter.kod == 0)//Tek kayıt çağrısında bulunmuyor ise
            {
                //İl kullanıcısı yetkisi için
                //*****************************************************************************
                string[] yetkiliOlduguIller = TNS.TRM.Yetki.YetkiliOlduguIlleriGetir(kullanan);
                if (kriter.ilKod.Trim() == "")
                {
                    foreach (string il in yetkiliOlduguIller)
                    {
                        if (il.Trim() == "")
                            continue;
                        if (kriter.ilKod != "") kriter.ilKod += ";";
                        kriter.ilKod += il;
                    }
                }
                //*****************************************************************************
            }

            IDataReader r = to.ListeleMalzeme(kriter, "", asc, gecenler);

            if (to.Hata() == "")
            {

                while (r.Read())
                {
                    HibeYatirim f = new HibeYatirim();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.basvuruNo = VeriErisim.DegerAl(r, 1, "");
                    f.ilKod = VeriErisim.DegerAl(r, 2, "");
                    f.kimlikNo = VeriErisim.DegerAl(r, 3, "");
                    f.ad = VeriErisim.DegerAl(r, 4, "");
                    f.soyad = VeriErisim.DegerAl(r, 5, "");
                    f.etapNo = VeriErisim.DegerAl(r, 6, 0);
                    f.projeAdi = VeriErisim.DegerAl(r, 7, "");
                    f.ilAd = VeriErisim.DegerAl(r, 8, "");

                    //Malzemeleri ekle
                    string malzeme = VeriErisim.DegerAl(r, 9, "");
                    string[] malzemeler = malzeme.Split('|');
                    List<YatirimMalzeme> mler = new List<YatirimMalzeme>();
                    foreach (var mal in malzemeler)
                    {
                        if (string.IsNullOrWhiteSpace(mal)) continue;

                        string[] mallar = mal.Split('^');

                        YatirimMalzeme m = new YatirimMalzeme();
                        m.malzeme.ad = mallar[0];
                        m.kod = OrtakFonksiyonlar.ConvertToInt(mallar[1], 0);
                        m.malzemeTurKod = OrtakFonksiyonlar.ConvertToInt(mallar[2], 0);
                        m.malzeme.kod = mallar[3];
                        m.malzemeTurAdi = mallar[4];
                        m.miktar = OrtakFonksiyonlar.ConvertToInt(mallar[5], 0);
                        if (m.miktar == 0) m.miktar = 1;

                        mler.Add(m);
                    }
                    f.malzeme = mler.ToArray();
                    //////////////////////////

                    bool yetki = false;

                    if (kriter.kod > 0)
                    {
                        //Eğer tek bir kaydı görmek istiyor ise (Başvuru ekranı gibi)
                        if (TNS.TRM.Yetki.YetkiVarMiOkumaya(kullanan, f.ilKod, ENUMHibeTur.EKONOMIKYATIRIM))
                            yetki = true;
                    }
                    else
                        yetki = true;

                    if (yetki)
                        bilgi.objeler.Add(f);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return bilgi;
        }


        public ObjectArray ListeleMerkezdeKabulHali(Kullanici kullanan, int kod)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirim to = new TABHibeYatirim(ve);

            IDataReader r = to.ListeleMerkezdeKabulHali(kod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeYatirim f = new HibeYatirim();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                    f.yil = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YIL, 0);
                    f.basvuruNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUNO, string.Empty);
                    f.ilKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ILKOD, string.Empty);
                    f.ilceKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ILCEKOD, string.Empty);
                    f.koyKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOYKOD, 0);
                    f.kasaba = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KASABA, string.Empty);
                    f.hukukiStatu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HUKUKISTATU, 0);
                    f.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KIMLIKNO, string.Empty);
                    f.vergiDaire = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.VERGIDAIRE, string.Empty);
                    f.ad = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.AD, string.Empty);
                    f.evrakNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EVRAKNO, string.Empty);
                    f.evrakTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EVRAKTARIH);
                    f.adres = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ADRES, string.Empty);
                    f.telefon = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.TELEFON, string.Empty);
                    f.faks = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FAKS, string.Empty);
                    f.ePosta = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EPOSTA, string.Empty);
                    f.bankaSubeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BANKASUBEKOD, string.Empty);
                    f.durum = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DURUM, 0);
                    f.sozlesmeTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMETARIH);
                    f.faturaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FATURANO, string.Empty);
                    f.faturaTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FATURATARIH);
                    f.soyad = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOYAD, string.Empty);
                    f.babaAd = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BABAAD, string.Empty);
                    f.dogumTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DOGUMTARIH);
                    f.yerindeTespitTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YERINDETESPITTARIH);
                    f.etapNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ETAPNO, 0);

                    f.projeNitelik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJENITELIK, 0);
                    f.projeKonu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEKONU, "");
                    f.projeAltKonu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEALTKONU, "");
                    f.projeAdi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEADI, string.Empty);
                    f.sozlesmeMaliyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMEMALIYET, decimal.Zero);
                    f.basvuruMaliyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUMALIYET, decimal.Zero);
                    f.hibeTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HIBETUTAR, decimal.Zero);
                    f.hibeEsasTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HIBEESASTUTAR, decimal.Zero);
                    f.ayniKatki = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.AYNIKATKI, decimal.Zero);
                    f.toplamTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.TOPLAMTUTAR, decimal.Zero);
                    f.anket1 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET1, 0);
                    f.anket2 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET2, 0);
                    f.anket3 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET3, 0);
                    f.anket4 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET4, 0);
                    f.anket3Puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET3PUAN, 0);
                    f.anket4Puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET4PUAN, 0);
                    f.puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PUAN, (double)0);
                    f.konuNitelikUyusma = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KONUNITELIKUYUSMA, 0) == 1 ? true : false;
                    f.degerlendirmeDisi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISI, 0) == 1 ? true : false;
                    f.degerlendirmeDisiGerekce = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISIGEREKCE, string.Empty);
                    f.kabulCalismasi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KABULCALISMASI, 0);
                    f.onlineTuzeleBasvuran = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ONLINETUZELBASVURUKIMLIKNO, string.Empty);
                    f.pNitelik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PNITELIK, 0);
                    f.mevki = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.MEVKI, string.Empty);
                    f.paftaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PAFTANO, string.Empty);
                    f.adaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ADANO, string.Empty);
                    f.parselNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PARSELNO, string.Empty);
                    f.koordinat = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOORDINAT, string.Empty);
                    f.ozet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.OZET, string.Empty);
                    f.hedef = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HEDEF, string.Empty);
                    f.istihdamSayisi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ISTIHDAM, 0);
                    f.dilekceNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DILEKCENO, "");
                    f.eskiEtapdaVarmi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ESKIETAPDAVARMI, 0);
                    f.basvuruYapildi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUYAPILDI, 0);
                    f.altYapiGrup = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTYAPIGRUP, "");
                    f.cinsiyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.CINSIYET, "");
                    f.engelli = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ENGELLI, "");
                    f.organik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ORGANIK, "");
                    f.yas = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YAS, 0);
                    f.altKonuDetayKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD, "");

                    bilgi.objeler.Add(f);
                }
            }
            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        public static int HibeYatirimBasvuruSayiDondur(VeriErisim veriErisim, Kullanici kullanan, HibeYatirim kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeYatirim th = new TABHibeYatirim(ve);
            int basvuruSayi = th.HibeYatirimBasvuruSayiDondur(kriter);
            ve.VeriErisimKapat();
            return basvuruSayi;
        }

        public static ObjectArray ListeleKabulCalismasiIlToplami(VeriErisim veriErisim, Kullanici kullanan, Butce kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeYatirim th = new TABHibeYatirim(ve);
            IDataReader r = th.ListeleKabulCalismasiIlToplami(kriter);

            ObjectArray liste = new ObjectArray();
            if (r != null)
            {
                while (r.Read())
                {
                    Butce b = new Butce();
                    b.ilKod = VeriErisim.DegerAl(r, 0, "");
                    b.talepEdilen = VeriErisim.DegerAl(r, 1, 0.0);
                    liste.objeler.Add(b);
                }

                r.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static bool KabulSonrasiVarMi(VeriErisim veriErisim, Kullanici kullanan, HibeYatirim kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeYatirim th = new TABHibeYatirim(ve);
            bool kabulSonrasiVarMi = th.KabulSonrasiVarMi(kriter);
            ve.VeriErisimKapat();
            return kabulSonrasiVarMi;
        }

        private string BasvuruNoAl(VeriErisim ve, int etapNo, string ilKod, string konu, string altKonuDetayNumara, int nitelik, string altYapiGrupKod)
        {
            TABGenel tg = new TABGenel(ve);

            if (etapNo >= 17)
                altYapiGrupKod = "";

            string etapNoStr = TNS.TRM.Arac.EtapAdiVer(etapNo);
            etapNoStr = etapNoStr.Replace(".", "/");

            string sayacKriter = etapNoStr + "." + ilKod.PadLeft(2, '0');
            string sayacKod = "HIBEYATIRIMBASVURUNO" + sayacKriter.Replace(".", "");

            //01 - Yeni Tesis
            //02 - Kısmen Yapılmış(Tamamlama)
            //03 - Kapasite Artırımı, 
            //04 - Teknoloji Yenileme Ve/ Veya Modernizasyon
            string nitelikKarsilik = "00";
            if (nitelik == 1) nitelikKarsilik = "01";
            else if (nitelik == 2) nitelikKarsilik = "04";
            else if (nitelik == 3) nitelikKarsilik = "02";
            else if (nitelik == 5) nitelikKarsilik = "03";

            //if (altYapiGrupKod != "")
            //    altYapiGrupKod = "." + altYapiGrupKod;

            if (!string.IsNullOrWhiteSpace(altKonuDetayNumara))
                altKonuDetayNumara = "/" + altKonuDetayNumara;

            return ilKod.PadLeft(2, '0') + altYapiGrupKod + "." + nitelikKarsilik + "." + konu + altKonuDetayNumara + "." + etapNoStr + "." + tg.BelgeNoAl(sayacKod, "0000");
        }

        private string BasvuruNoAlOnline(VeriErisim ve, int etapNo)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKriter = etapNo.ToString("00");
            string sayacKod = "HIBEYATIRIMBASVURUNOONLINE" + sayacKriter.Replace(".", "");

            return etapNo.ToString("00") + "." + tg.BelgeNoAl(sayacKod, "000000");
        }

        private int BelgeNoAlInt(VeriErisim ve)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKod = "HIBEYATIRIMKOD";

            return tg.BelgeNoAlInt(sayacKod, "000000");
        }

        private Sonuc KontrolKimlikNoKayitVarMi(VeriErisim ve, Kullanici kullanan, int kod, string kimlikNo, int aktifEtapNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);

            HibeYatirim kriter = new HibeYatirim();
            if (kod > 0)
                kriter.kod = kod;
            else
                kriter.kimlikNo = kimlikNo;

            kriter.etapNo = aktifEtapNo;

            IDataReader r = to.KontrolKimlikNoKayitVarMi(kriter);

            HibeYatirim hm = new HibeYatirim();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    hm.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                    hm.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KIMLIKNO, string.Empty);
                }
            }
            r.Close();
            ve.VeriErisimKapat();

            if (hm.kod > 0)
            {
                if (hm.kod != kod)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = kimlikNo + " Kimlik Numarası ile sisteme kayıtlı başvuru bulunmaktadır.<br>";
                    return sonuc;
                }
                else if (hm.kimlikNo != kimlikNo)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Bu belge " + hm.kimlikNo + " Kimlik Numarası ile sisteme kayıtlıdır. Belgeye ait kimlik no değiştirilemez.<br>";
                    return sonuc;
                }
            }

            return sonuc;
        }

        private int KontrolKimlikNoKayitEskiEtaplardaVarMi(ref VeriErisim ve, Kullanici kullanan, string kimlikNo, int aktifEtapNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirim to = new TABHibeYatirim(ve);

            HibeYatirim kriter = new HibeYatirim();
            kriter.kimlikNo = kimlikNo;
            kriter.etapNo = aktifEtapNo;

            IDataReader r = to.KontrolKimlikNoKayitEskiEtaplardaVarMi(kriter);

            int cnt = 0;

            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    cnt = VeriErisim.DegerAl(r, 0, 0);
                    r.Close();
                }
            }

            ve.VeriErisimKapat();

            return cnt;
        }

        public ObjectArray ListeleTarihce(Kullanici kullanan, HibeYatirim kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            Sonuc sonuc = new Sonuc();

            if (!Yetki.YetkiVarMiOkumaya(kullanan, kriter.ilKod, ENUMHibeTur.EKONOMIKYATIRIM))
            {
                sonuc.hataStr = "Okuma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                bilgi.sonuc = sonuc;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirim to = new TABHibeYatirim(ve);
            IDataReader r = to.ListeleTarihce(new HibeYatirimTarihce { hibeKod = kriter.kod });

            if (to.Hata() == "")
            {
                int sonKolon = (int)TABHibeYatirim.ENUMAlanlarTarihce.IPADRESI;

                while (r.Read())
                {
                    HibeYatirimTarihce f = new HibeYatirimTarihce();

                    f.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarTarihce.HIBEKOD, 0);
                    f.kullaniciKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarTarihce.KULLANICIKOD, string.Empty);
                    f.tarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarTarihce.TARIH);
                    f.durum = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarTarihce.DURUM, 0);
                    f.aciklama = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarTarihce.ACIKLAMA, "");
                    f.ipAdresi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarTarihce.IPADRESI, "");
                    f.basvuruNo = VeriErisim.DegerAl(r, sonKolon + 1, "");
                    f.basvuruDurum = VeriErisim.DegerAl(r, sonKolon + 2, 0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public string IslemKontrol(VeriErisim ve, Kullanici kullanan, HibeYatirim bilgi, ref HibeYatirim hm, string tur)
        {
            if (bilgi.kod == 0)
                return "İşlem yapılacak kayıta ait kod bilgisi alınamadı";

            ObjectArray hmler = Listele(ve, kullanan, new HibeYatirim { kod = bilgi.kod, online = bilgi.online });
            if (hmler == null || hmler.objeler.Count == 0 || hmler.objeler.Count > 1)
                return "İşlem yapılacak kayıta ait bilgi okunamadı";

            hm = (HibeYatirim)hmler.objeler[0];
            if (!bilgi.online)
            {
                Sonuc sonuc = new Sonuc();
                if (!Yetki.YetkiVarMiYazmaya(kullanan, hm.ilKod, hm.etapNo, sonuc, string.Empty, ENUMHibeTur.EKONOMIKYATIRIM))
                {
                    Sonuc sonuc1 = new Sonuc();
                    if (!Yetki.YetkiVarMiYazmaya(kullanan, hm.basvuruNo, hm.etapNo, sonuc1, string.Empty, ENUMHibeTur.EKONOMIKYATIRIM))
                        return sonuc1.hataStr;
                }
            }

            if (hm.hukukiStatu != (int)ENUMHibeYatirimHukukiStatu.TUZELKISI)
            {
                if (string.IsNullOrEmpty(hm.babaAd))
                    return "TC Kimlik Numarası " + hm.kimlikNo + " olan kişinin kimlik bilgileri bulunmamaktadır. Başvuru giriş ekranından eksik bilgileri girin.<br>";
            }

            //Cezalı kontrolü
            //**************************
            Sonuc cezaliMi = new TRMHibeCeza().CezaliMi(hm.kimlikNo);
            if (cezaliMi.islemSonuc)
                return cezaliMi.hataStr;
            //**************************

            string hata = "";
            if (tur == "BasvuruKaydet")
            {
                if (hm.durum > (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI)
                {
                    Sonuc sonuc = new Sonuc();
                    if (!Yetki.YetkiVarMiYazmaya(kullanan, hm.basvuruNo, hm.etapNo, sonuc, string.Empty, ENUMHibeTur.EKONOMIKYATIRIM, bilgi.kod, bilgi.online))
                        hata += "Merkezde kabul edilen başvurular üzerinde değişiklik yapılamaz.<br>";
                }
            }
            else if (tur == "Sozlesme")
            {
                if (hm.durum > (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI)
                    hata += "Sözleşme kaydı aşamasını geçmiş bulunmaktasınız.<br>";
                if (bilgi.sozlesmeTarih.isNull)
                    hata += "Sözleşme Tarihi boş bırakılamaz<br>";
                else if (bilgi.sozlesmeTarih.Oku() > DateTime.Now)
                    hata += "Sözleşme Tarihi, bugünden sonraki bir gün olamaz<br>";
                else if (bilgi.sozlesmeTarih.Oku() < hm.evrakTarih.Oku())
                    hata += "Sözleşme Tarihi, başvuru evrak tarihinden (" + hm.evrakTarih.ToString() + ") önceki bir gün olamaz<br>";

                if (!bilgi.eskiBasvuru)
                {
                    if (string.IsNullOrEmpty(bilgi.bankaSubeKod))
                        hata += "Ödemenin yapılacağı banka şubesi boş bırakılamaz.<br>";
                }


                ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
                HibeYatirimUygulamaSozlesme uy = new HibeYatirimUygulamaSozlesme();
                uy.hibeKod = bilgi.kod;
                HibeYatirimUygulamaSozlesme[] hy = TRMHibeYatirimUygulamaSozlesme.Listele(ref ve, kullanan, uy, false);
                if (hy != null && hy.Length > 0)
                {
                    Sonuc sonuc = new Sonuc();
                    if (!Yetki.YetkiVarMiYazmaya(kullanan, hm.basvuruNo, hm.etapNo, sonuc, string.Empty, ENUMHibeTur.EKONOMIKYATIRIM))
                    {
                        hata = "++";//Sozlesmenin parasal olmayan değerlerini kayıt güncellemek için
                        hata += "Bu sözleşmenin uygulama sözleşmesi bulunmaktadır. Bu nedenle değişiklik yapılamaz.";
                    }
                }

                ve.VeriErisimKapat();
            }
            else if (tur == "Kabul")
            {
                if (hm.durum > (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI && hm.durum != (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDIMERKEZ)
                    hata += hm.basvuruNo + " nolu başvuru için kabul aşamasını geçmiş bulunmaktasınız.<br>";
            }
            else if (tur == "Red")
            {
                if (hm.durum >= (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI &&
                    hm.durum < (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "Merkez tarafından kabul edilen başvuru reddedilemez.<br>";
            }
            else if (tur == "Online")
            {
                if (hm.durum >= (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI &&
                    hm.durum < (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "Merkez tarafından kabul edilen başvuru online durumuna getirilemez.<br>";
            }
            else if (tur == "YatirimciDegistir")
            {
                //Cezalı kontrolü
                //**************************
                cezaliMi = new TRMHibeCeza().CezaliMi(bilgi.kimlikNo);
                if (cezaliMi.islemSonuc)
                    return cezaliMi.hataStr;
                //**************************

                if (hm.durum == (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "(" + hm.basvuruNo + ") Reddedilmiş yatırımcı değiştirilemez.";
            }
            else if (tur == "BasvuruNoDegistir")
            {
                if (hm.durum == (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "(" + hm.basvuruNo + ") Reddedilmiş başvuru değiştirilemez.";
            }

            return hata;
        }

        public static HibeYatirim[] KimlikBilgisiEksikBasvurular(VeriErisim ve, Kullanici kullanan)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                throw new Exception("Okuma yetkiniz bulunmamaktadır.");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            IList<HibeYatirim> al = new List<HibeYatirim>();
            IDataReader r = to.KimlikBilgisiEksikBasvurular();
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (r.Read())
                {
                    HibeYatirim hm = new HibeYatirim();
                    hm.kod = VeriErisim.DegerAl(r, 0, 0);
                    hm.kimlikNo = VeriErisim.DegerAl(r, 1, string.Empty);

                    al.Add(hm);
                }
            }
            r.Close();
            ve.VeriErisimKapat();

            HibeYatirim[] liste = new HibeYatirim[al.Count];
            al.CopyTo(liste, 0);
            return liste;
        }

        public static Sonuc KimlikBilgisiEksikGuncelle(VeriErisim ve, Kullanici kullanan, HibeYatirim hm)
        {
            Sonuc sonuc = new Sonuc();
            if (hm.kod <= 0 || string.IsNullOrEmpty(hm.ad) || string.IsNullOrEmpty(hm.soyad))
                return sonuc;

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            sonuc = to.KimlikBilgisiEksikGuncelle(hm);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "İşlem sırasında hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        //public static void HibeTutarHesapla(int hukukiStatu, decimal referansTutar, decimal basvuruTutar, decimal sozlesmeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, decimal hibeOran)
        public static void HibeTutarHesapla(int projeNitelik, string konuKod, decimal referansTutar, decimal basvuruTutar, decimal sozlesmeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, int hukukiStatu, decimal hibeOran, int etapNo)
        {
            VeriErisim ve = null;
            //decimal ustSinir = OrtakFonksiyonlar.ConvertToDecimal(TRMHibeYatirimYonetim.HibeYatirimAktifUstLimit(ref ve, hukukiStatu));
            decimal ustSinir = OrtakFonksiyonlar.ConvertToDecimal(TRMHibeYatirimYonetim.HibeYatirimAktifUstLimitKonu(ref ve, etapNo, projeNitelik, konuKod, hukukiStatu));
            decimal hibeEsasTutarElle = hibeEsasTutar;

            if (referansTutar == 0)
                referansTutar = basvuruTutar;

            if (referansTutar > 0)
                hibeEsasTutar = basvuruTutar > referansTutar ? referansTutar : basvuruTutar;

            if (sozlesmeTutar > 0)
                hibeEsasTutar = hibeEsasTutar > sozlesmeTutar ? sozlesmeTutar : hibeEsasTutar;

            hibeEsasTutar = hibeEsasTutar > ustSinir ? ustSinir : hibeEsasTutar;

            if (hibeEsasTutarElle > 0)
                hibeEsasTutar = hibeEsasTutar > hibeEsasTutarElle ? hibeEsasTutarElle : hibeEsasTutar;

            if (sozlesmeTutar > 0)
                toplamTutar = sozlesmeTutar;
            else
                toplamTutar = basvuruTutar;

            ayniKatki = toplamTutar - hibeEsasTutar;
            hibeTutar = Math.Round(hibeEsasTutar * (hibeOran / 100), 2);
        }

        public Sonuc KonuNitelikKontrol(VeriErisim ve, Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc sonuc = new Sonuc();
            string konuNitelik = new TABYatirimKonu(ve).KonuNitelikBul(bilgi.projeKonu);
            string[] nitelik = konuNitelik.Split(';');
            bool konuUygun = false;
            foreach (string n in nitelik)
            {
                if (bilgi.projeNitelik.ToString() == n)
                {
                    konuUygun = true;
                    break;
                }
            }
            if (!konuUygun)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Seçmiş olduğunuz Nitelik ile Konu uyuşmamaktadır.";
                return sonuc;
            }

            if (konuUygun)
            {
                //Yatırımcı istediği konudan başvuru yapabilir. Melih 28.12.2020
                //ObjectArray konulIlListe = new TRMYatirimKonu().ListeleKonuIl(kullanan, new YatirimKonuIl { etapNo = bilgi.etapNo, ilKod = bilgi.ilKod });

                //if (konulIlListe.objeler.Count > 0)
                //{
                //    konuUygun = false;

                //    foreach (YatirimKonuIl i in konulIlListe.objeler)
                //    {
                //        if (i.konuKod == bilgi.projeKonu)
                //        {
                //            konuUygun = true;
                //            break;
                //        }
                //    }

                //    if (!konuUygun)
                //    {
                //        sonuc.islemSonuc = false;
                //        sonuc.hataStr = "Seçmiş olduğunuz Konu, başvuru yapılan il için uygun değil.";
                //        return sonuc;
                //    }
                //}
            }

            if (konuUygun)
            {
                //ObjectArray altKonulIlListe = new TRMYatirimKonu().ListeleAltKonuIl(kullanan, new YatirimAltKonuIl { etapNo = bilgi.etapNo, ilKod = bilgi.ilKod });

                //if (altKonulIlListe.objeler.Count > 0)
                //{
                //    konuUygun = false;

                //    foreach (YatirimAltKonuIl i in altKonulIlListe.objeler)
                //    {
                //        if (i.altKonuKod == bilgi.projeAltKonu)
                //        {
                //            konuUygun = true;
                //            break;
                //        }
                //    }

                //    if (!konuUygun)
                //    {
                //        sonuc.islemSonuc = false;
                //        sonuc.hataStr = "Seçmiş olduğunuz Alt Konu, başvuru yapılan il için uygun değil.";
                //        return sonuc;
                //    }
                //}
            }


            sonuc.islemSonuc = true;
            return sonuc;
        }

        public ObjectArray YatirimKapasiteBirimListele()
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirim to = new TABHibeYatirim(ve);
            IDataReader r = to.KapasiteBirimListele();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    string birim = VeriErisim.DegerAl(r, 0, "");
                    bilgi.objeler.Add(birim);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static YatirimIsletmePlani YatirimIsletmePlaniGetir(Kullanici kullanan, int hibeKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim thy = new TABHibeYatirim(ve);
            IDataReader dr = thy.YatirimIsletmePlaniGetir(hibeKod);
            if (!string.IsNullOrEmpty(thy.Hata()))
            {
                dr.Close();
                ve.VeriErisimKapat();
                throw new Exception(thy.Hata());
            }

            YatirimIsletmePlani hym = new YatirimIsletmePlani();
            if (dr.Read())
            {
                hym.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.HIBEKOD, 0);
                hym.toplamYatirimTutari = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.TOPLAMYATIRIMTUTARI, (decimal)0);
                hym.sabitYatirimTutari = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.SABITYATIRIMTUTARI, (decimal)0);
                hym.sermayeYatirimTutari = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.SERMAYEYATIRIMTUTARI, (decimal)0);
                hym.isletmeGeliri = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.ISLETMEGELIRI, (decimal)0);
                hym.isletmeGideri = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.ISLETMEGIDERI, (decimal)0);
                hym.brutNakitAkim = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.BRUTNAKITAKIM, (decimal)0);
                hym.IcKarlilikOrani = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.ICKARLILIKORANI, (decimal)0);
                hym.faydaMasrafOrani = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.FAYDAMASRAFORANI, (decimal)0);
                hym.geriDonusSuresi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.GERIDONUSURESI, string.Empty);
                hym.brutKatmaDeger = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.BRUTKATMADEGER, (decimal)0);
                hym.netKatmaDeger = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.NETKATMADEGER, (decimal)0);
                hym.basabasNoktasi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.BASABASNOKTASI, (decimal)0);
                hym.baslamaTarihi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.BASLAMATARIHI);
                hym.tamamlanmaTarihi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.TAMAMLANMATARIHI);
                hym.mevcutKapasite = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.MEVCUTKAPASITE, (decimal)0);
                hym.projeIleSaglananKapasite = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.PROJEILESAGLANACAKKAPASITE, (decimal)0);
                hym.projeSonrasiOlacakKapasite = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.PROJESONRASIOLUSACAKKAPASITE, (decimal)0);
                hym.mevcutIstihdam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.MEVCUTISTAIHDAMSAYISI, (decimal)0);
                hym.projeIleSaglananIstihdam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.PROJEILESAGLANACAKISTIHDAMSAYISI, (decimal)0);
                hym.projeSonrasiIstihdam = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarIsletmePlani.PROJESONRASIISTIHDAMSAYISI, (decimal)0);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return hym;
        }

        public Sonuc KaydetIsletmePlani(Kullanici kullanan, YatirimIsletmePlani plan)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);

            if (plan.hibeKod <= 0)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetIsletmePlani(plan);

            if (sonuc.islemSonuc)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                if (plan.online)
                    kullanan.kullaniciKodu = "Şifresiz Giriş";
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.aciklama = "Yatırım İşletme Planı kayıt işlemi gerçekleştirildi.";
                tarihce.hibeKod = plan.hibeKod;
                tarihce.ipAdresi = plan.ipAdresi == "" ? kullanan.telefon : plan.ipAdresi;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc SozlesmeDurumunuGuncelle(Kullanici kullanan, HibeYatirimSozlesme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray hmler = Listele(ve, kullanan, new HibeYatirim { kod = bilgi.hibeKod, online = false });
            if (hmler == null || hmler.objeler.Count == 0 || hmler.objeler.Count > 1)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "İşlem yapılacak kayıta ait bilgi okunamadı";
                ve.VeriErisimKapat();
                return sonuc;
            }

            HibeYatirim hm = (HibeYatirim)hmler.objeler[0];
            if (!Yetki.YetkiVarMiYazmaya(kullanan, hm.ilKod, hm.etapNo, sonuc, string.Empty, ENUMHibeTur.EKONOMIKYATIRIM))
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (bilgi.durum == ENUMHibeYatirimSozlesmeDurum.BITTI)
            {
                sonuc.hataStr = SozlesmeDurumunuBitirmeKontrolu(ve, kullanan, bilgi.hibeKod);
                if (!string.IsNullOrEmpty(sonuc.hataStr))
                {
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            TABHibeYatirim to = new TABHibeYatirim(ve);

            sonuc = to.GuncelleSozlesmeDurum(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc NihaiRaporTarihGuncelle(Kullanici kullanan, HibeYatirimSozlesme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            ObjectArray hmler = Listele(ve, kullanan, new HibeYatirim { kod = bilgi.hibeKod, online = false });
            if (hmler == null || hmler.objeler.Count == 0 || hmler.objeler.Count > 1)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "İşlem yapılacak kayıta ait bilgi okunamadı";
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeYatirim to = new TABHibeYatirim(ve);

            sonuc = to.GuncelleNihaiRaporTarih(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc YapiKimlikNoGuncelle(Kullanici kullanan, HibeYatirimSozlesme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            ObjectArray hmler = Listele(ve, kullanan, new HibeYatirim { kod = bilgi.hibeKod, online = false });
            if (hmler == null || hmler.objeler.Count == 0 || hmler.objeler.Count > 1)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "İşlem yapılacak kayıta ait bilgi okunamadı";
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (string.IsNullOrWhiteSpace(bilgi.yapiKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Yapı Kimlik Bilgisi giriniz. Örnek: 02.0.0001978765\n\nEğer Yapı Kimlik Bilgisi yoksa sadece YOK yazınız";
                ve.VeriErisimKapat();
                return sonuc;
            }


            TABHibeYatirim to = new TABHibeYatirim(ve);

            sonuc = to.GuncelleYapiKimlikNo(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public string SozlesmeDurumunuBitirmeKontrolu(VeriErisim ve, Kullanici kullanan, int hibeKod)
        {
            string hata = "";

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            TABHibeYatirimUygulamaSozlesme tabUygulamaSozlesmesi = new TABHibeYatirimUygulamaSozlesme(ve);
            IDataReader r;
            int cntHakedis = 0, cntUygulamaSozlesemesi = 0;
            r = tabHakedis.ListeleHakedisOdenmeyen(hibeKod);
            if (r.Read())
                cntHakedis = VeriErisim.DegerAl(r, 0, 0);
            if (cntHakedis > 0)
            {
                hata = " Ödenmeyen hakedişler var. Sözleşmeyi bitiremezsiniz.";
                ve.VeriErisimKapat();
                return hata;
            }
            r.Dispose();
            r = tabUygulamaSozlesmesi.ListeleUygulamaSozlesmesiDevamEden(hibeKod);
            if (r.Read())
                cntUygulamaSozlesemesi = VeriErisim.DegerAl(r, 0, 0);

            if (cntUygulamaSozlesemesi > 0)
            {
                hata = " Uygulama sözleşmesi devam eden kayıtlar var. Sözleşmeyi bitiremezsiniz.";
                return hata;
            }
            r.Dispose();
            r.Close();
            ve.VeriErisimKapat();
            return hata;
        }

        public Sonuc AnketSonucSifirla(Kullanici kullanan, int hibeKod, int etapNo, string ilKod, string ip)
        {
            Sonuc sonuc = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI) && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEBASKAN))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Bu işlemi yapma yetkiniz yok.";
                return sonuc;
            }
            string hata = "";
            if (hibeKod <= 0)
                hata = "Başvuru kodu boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(hata))
                return new Sonuc(false, hata, string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            //if (!Yetki.YetkiVarMiYazmaya(kullanan, ilKod, etapNo, string.Empty, ENUMHibeTur.EKONOMIKYATIRIM))
            //    return new Sonuc(false, "Yazma yetkiniz bulunmamaktadır.", string.Empty);
            ObjectArray list = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = hibeKod });
            HibeYatirim hm = new HibeYatirim();
            if (list.ObjeSayisi > 0)
                hm = (HibeYatirim)list.objeler[0];

            hata = TRMHibeYatirimYonetim.HibeYatirimDegerlendirmeKapandiMi(hm.basvuruNo, hm.projeKonu);
            if (!string.IsNullOrEmpty(hata))
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, hata, "");
            }

            sonuc = TNS.KYM.Arac.Tanimla().AnketCevaplariSifirla(kullanan, hibeKod);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            sonuc = to.sifirlaAnketSonuc(hibeKod);
            if (sonuc.islemSonuc)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.hibeKod = hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.durum = 100;
                tarihce.aciklama = "Komisyon başkanı tarafından tüm değerlendirmeler sıfırlandı.";
                tarihce.ipAdresi = ip == "" ? kullanan.telefon : ip;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "İşlem sırasında hata oluştu.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public ArrayList DegerlendirmeSayilariniGetir(Kullanici kullanan, int etapNo, string ilKod, int yatirimOncelikTur, string altYapiGrup)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ArrayList list = new ArrayList();
            TABHibeYatirim to = new TABHibeYatirim(ve);
            IDataReader r = to.DegerlendirmeSayilariniGetir(etapNo, ilKod, yatirimOncelikTur, altYapiGrup);

            if (to.Hata() == "")
            {
                if (r.Read())
                    list.Add(new object[] {
                        VeriErisim.DegerAl(r, 0, 0),
                        VeriErisim.DegerAl(r, 1, 0),
                        VeriErisim.DegerAl(r, 2, 0),
                        VeriErisim.DegerAl(r, 3, 0),
                        VeriErisim.DegerAl(r, 4, 0),
                        VeriErisim.DegerAl(r, 5, 0)
                    });
                r.Dispose();
            }

            r.Close();
            ve.VeriErisimKapat();

            return list;
        }

        public HibeYatirimServis[] HibeYatirimListForTKDK(Kullanici kullanan, string kimlikNo)
        {
            HibeYatirimServis[] liste;
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKWEBSERVISKULLANICISI))
            {
                liste = new HibeYatirimServis[1];
                liste[0] = new HibeYatirimServis();
                liste[0].sonuc = new Sonuc(false, "Yetkiniz yok.", "");
                return liste;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            IDataReader dr = to.HibeYatirimListForTKDK(kimlikNo);

            if (!string.IsNullOrEmpty(to.Hata()))
            {
                dr.Close();
                ve.VeriErisimKapat();
                liste = new HibeYatirimServis[1];
                liste[0] = new HibeYatirimServis();
                liste[0].sonuc = new Sonuc(false, to.Hata(), "");
                return liste;
            }

            IList<HibeYatirimServis> al = new List<HibeYatirimServis>();
            while (dr.Read())
            {
                HibeYatirimServis hy = new HibeYatirimServis();
                hy.isim = VeriErisim.DegerAl(dr, 0, "");
                hy.unvan = HibeYatirim.StatuAdBul(VeriErisim.DegerAl(dr, 1, 0));
                hy.il = VeriErisim.DegerAl(dr, 2, "");
                hy.adres = VeriErisim.DegerAl(dr, 3, "");
                hy.yatirimKonusu = VeriErisim.DegerAl(dr, 4, "");
                hy.projeAdi = VeriErisim.DegerAl(dr, 5, "");
                hy.basvuruDurumu = HibeYatirimServis.DurumAdBul(VeriErisim.DegerAl(dr, 6, 0));
                hy.sozlesmeTarihi = VeriErisim.DegerAl(dr, 7).ToString();
                hy.yil = VeriErisim.DegerAl(dr, 8, 0);
                hy.etapNo = VeriErisim.DegerAl(dr, 9, 0);
                hy.etapNoStr = TNS.TRM.Arac.EtapAdiVer(hy.etapNo);
                al.Add(hy);
            }
            dr.Close();

            if (al.Count > 0)
            {
                liste = new HibeYatirimServis[al.Count];
                al.CopyTo(liste, 0);
            }
            else
            {
                liste = new HibeYatirimServis[1];
                liste[0] = new HibeYatirimServis();
                liste[0].sonuc = new Sonuc(false, "Girdiğiniz kimlik numarasına ait başvuru bilgisi yok.", "");
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public int BasvuruDurumBilgisi(string basvuruNo)
        {
            if (string.IsNullOrEmpty(basvuruNo)) return 0;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            int durum = to.BasvuruDurumBilgisi(basvuruNo);
            ve.VeriErisimKapat();

            return durum;
        }

        public bool MerkezKabulAsamasiGectimi(int etapNo)
        {
            if (etapNo == 0) return false;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            int sayi = to.MerkezKabulAsamasiGectimi(etapNo);
            ve.VeriErisimKapat();

            return sayi > 0;
        }

        public Sonuc YatirimciDegistir(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            HibeYatirim hm = new HibeYatirim();
            string hata = IslemKontrol(ve, kullanan, bilgi, ref hm, "YatirimciDegistir");

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            //Eski bilgileri de aynı yöntemle (üstteki) alalım
            HibeYatirim eskiBilgi = new HibeYatirim();
            hata = IslemKontrol(ve, kullanan, bilgi, ref eskiBilgi, "YatirimciDegistir");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            sonuc = KontrolKimlikNoKayitVarMi(ve, kullanan, bilgi.kod, bilgi.kimlikNo, hm.etapNo, false);
            if (!sonuc.islemSonuc)
            {
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            hm.kimlikNo = bilgi.kimlikNo;
            hm.ad = bilgi.ad;
            hm.soyad = bilgi.soyad;
            hm.adres = bilgi.adres;
            hm.babaAd = bilgi.babaAd;
            hm.dogumTarih = bilgi.dogumTarih;
            hm.hukukiStatu = bilgi.hukukiStatu;
            hm.vergiDaire = bilgi.vergiDaire;

            //***************************Eski KIMLIKNO yu alabilmek için************************
            HibeYatirim eski = new HibeYatirim();
            eski.kod = bilgi.kod;
            ObjectArray eskiListe = Listele(ve, kullanan, eski);
            HibeYatirim eskiKimlikNo = (HibeYatirim)eskiListe.objeler[0];
            //***************************Eski KIMLIKNO yu alabilmek için************************

            sonuc = to.YatirimciDegistir(bilgi);
            if (sonuc.islemSonuc)
            {
                string varMi = to.GetirYatirimciKimlikNo(bilgi.kod);

                HibeYatirimYatirimciKimlikNo t = new HibeYatirimYatirimciKimlikNo();
                t.hibeKod = bilgi.kod;
                t.yeniKimlikNo = bilgi.kimlikNo;

                if (varMi != "")
                {
                    t.kod = Guid.NewGuid().ToString();
                    t.eskiKimlikNo = varMi;
                    sonuc = to.KaydetYatirimciKimlikNo(t);
                }
                else if (varMi == "")
                {
                    t.kod = Guid.NewGuid().ToString();
                    t.eskiKimlikNo = eskiKimlikNo.kimlikNo;
                    sonuc = to.KaydetYatirimciKimlikNo(t);
                }

                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.TANIMSIZ;
                tarihce.hibeKod = hm.kod;
                tarihce.aciklama = bilgi.redGerekce + " Yeni Kimlik Numarası:" + bilgi.kimlikNo + "; Eski Kimlik Numarası:" + bilgi.ePosta;//Eski kimlik numarası tutuluyor
                tarihce.ipAdresi = bilgi.ipAdresi == "" ? kullanan.telefon : bilgi.ipAdresi;
                sonuc = to.KaydetTarihce(tarihce);


                tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now.AddMilliseconds(10));
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.TANIMSIZ;
                tarihce.hibeKod = hm.kod;
                tarihce.ipAdresi = bilgi.ipAdresi == "" ? kullanan.telefon : bilgi.ipAdresi;
                tarihce.aciklama = EskiYeniBilgiKarsilastir(hm, eskiBilgi);
                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                sonuc.anahtar = hm.basvuruNo.ToString();

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "İşlem sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc BasvuruNoDegistir(Kullanici kullanan, HibeYatirim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            HibeYatirim hm = new HibeYatirim();
            string hata = IslemKontrol(ve, kullanan, bilgi, ref hm, "BasvuruNoDegistir");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            string plaka = new TABGenel(ve).IlPlakaKodBul(bilgi.ilKod); //TRMServis.UzayYaziDegerGetir("COGRAFYA", bilgi.ilKod, true, "PLAKAKOD");
            string konuKisaAd = new TABYatirimKonu(ve).KonuKisaAdBul(bilgi.projeKonu);

            string altKonuDetayNumara = "";
            if (hm.altKonuDetayKod != "")
            {
                altKonuDetayNumara = new TABYatirimKonu(ve).AltKonuDetayNoBul(hm.altKonuDetayKod);
            }

            bilgi.basvuruNo = BasvuruNoAl(ve, bilgi.etapNo, plaka, konuKisaAd, altKonuDetayNumara, bilgi.projeNitelik, bilgi.altYapiGrup);


            sonuc = to.BasvuruNoDegistir(bilgi);
            if (sonuc.islemSonuc)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.TANIMSIZ;
                tarihce.hibeKod = hm.kod;
                tarihce.aciklama = bilgi.redGerekce + " Yeni Başvuru No:" + bilgi.basvuruNo + "; Eski Başvuru No:" + bilgi.ePosta;//Eski başvuru numarası tutuluyor
                tarihce.ipAdresi = bilgi.ipAdresi == "" ? kullanan.telefon : bilgi.ipAdresi;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                sonuc.anahtar = hm.basvuruNo.ToString();

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "İşlem sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private Sonuc KontrolKimlikNoKayitVarMi(VeriErisim ve, Kullanici kullanan, int kod, string kimlikNo, int aktifEtapNo, bool degistirilemezKontrolu = true)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            HibeYatirim kriter = new HibeYatirim();
            if (kod > 0 && degistirilemezKontrolu)
                kriter.kod = kod;
            else
                kriter.kimlikNo = kimlikNo;

            kriter.etapNo = aktifEtapNo;

            IDataReader r = to.KontrolKimlikNoKayitVarMi(kriter);

            HibeYatirim hm = new HibeYatirim();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    hm.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                    hm.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KIMLIKNO, string.Empty);
                }
            }
            r.Close();
            ve.VeriErisimKapat();

            if (hm.kod > 0)
            {
                if (hm.kod != kod)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = kimlikNo + " Kimlik Numarası ile sisteme kayıtlı başvuru bulunmaktadır.<br>";
                    return sonuc;
                }
                else if (hm.kimlikNo != kimlikNo && degistirilemezKontrolu)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Bu belge " + hm.kimlikNo + " Kimlik Numarası ile sisteme kayıtlıdır. Belgeye ait kimlik no değiştirilemez.<br>";
                    return sonuc;
                }
            }

            return sonuc;
        }

        public static HazineRapor[] HazineRapor(int yil, int ay, int tur, int etapNo)
        {
            HazineRapor[] liste;

            if (tur == 3)
            {
                return HazineRaporYillik(yil, ay, etapNo);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            //Tüzel
            IDataReader dr = to.HazineRaporTuzel(yil, ay, tur, etapNo);
            if (!string.IsNullOrEmpty(to.Hata()))
            {
                ve.VeriErisimKapat();
                return null;
            }

            IList<HazineRapor> al = new List<HazineRapor>();
            while (dr.Read())
            {
                HazineRapor hy = new HazineRapor();
                hy.projeKodu = VeriErisim.DegerAl(dr, 0, "");
                hy.kimlikKodu = VeriErisim.DegerAl(dr, 1, "");
                hy.ilKodu = VeriErisim.DegerAl(dr, 2, 0).ToString();
                hy.ilceKodu = VeriErisim.DegerAl(dr, 3, 0).ToString();
                hy.sektorKodu = VeriErisim.DegerAl(dr, 4, "");
                hy.muracaatTarihi = VeriErisim.DegerAl(dr, 5);
                hy.belgeTarihi = VeriErisim.DegerAl(dr, 6);
                hy.baslamaTarihi = VeriErisim.DegerAl(dr, 7);
                hy.bitisTarihi = VeriErisim.DegerAl(dr, 8);
                hy.destekUnsuru = VeriErisim.DegerAl(dr, 9, "");
                hy.durum = VeriErisim.DegerAl(dr, 10, "");
                hy.anahtar = VeriErisim.DegerAl(dr, 11, "");
                hy.destekVerilen = VeriErisim.DegerAl(dr, 12, "");
                hy.hibeEsasTutar = VeriErisim.DegerAl(dr, 13, 0.0);
                hy.hibeTutar = VeriErisim.DegerAl(dr, 14, 0.0);

                if (tur == 2)
                    hy.sozlesmeTur = VeriErisim.DegerAl(dr, 15, 0);

                al.Add(hy);
            }
            dr.Close();

            //Gerçek
            dr = to.HazineRaporGercek(yil, ay, tur, etapNo);
            if (!string.IsNullOrEmpty(to.Hata()))
            {
                ve.VeriErisimKapat();
                return null;
            }

            while (dr.Read())
            {
                HazineRapor hy = new HazineRapor();
                hy.projeKodu = VeriErisim.DegerAl(dr, 0, "");
                hy.kimlikKodu = VeriErisim.DegerAl(dr, 1, "");
                hy.ilKodu = VeriErisim.DegerAl(dr, 2, 0).ToString();
                hy.ilceKodu = VeriErisim.DegerAl(dr, 3, 0).ToString();
                hy.sektorKodu = VeriErisim.DegerAl(dr, 4, "");
                hy.muracaatTarihi = VeriErisim.DegerAl(dr, 5);
                hy.belgeTarihi = VeriErisim.DegerAl(dr, 6);
                hy.baslamaTarihi = VeriErisim.DegerAl(dr, 7);
                hy.bitisTarihi = VeriErisim.DegerAl(dr, 8);
                hy.destekUnsuru = VeriErisim.DegerAl(dr, 9, "");
                hy.durum = VeriErisim.DegerAl(dr, 10, "");
                hy.anahtar = VeriErisim.DegerAl(dr, 11, "");
                hy.destekVerilen = VeriErisim.DegerAl(dr, 12, "");
                hy.hibeEsasTutar = VeriErisim.DegerAl(dr, 13, 0.0);
                hy.hibeTutar = VeriErisim.DegerAl(dr, 14, 0.0);

                if (tur == 2)
                    hy.sozlesmeTur = VeriErisim.DegerAl(dr, 15, 0);

                al.Add(hy);
            }
            dr.Close();

            if (al.Count > 0)
            {
                liste = new HazineRapor[al.Count];
                al.CopyTo(liste, 0);
            }
            else
            {
                liste = new HazineRapor[1];
                liste[0] = new HazineRapor();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static HazineRapor[] HazineRaporYillik(int yil1, int yil2, int etapNo)
        {
            HazineRapor[] liste;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            IDataReader dr = to.HazineRaporYillik(yil1, yil2, etapNo);
            if (!string.IsNullOrEmpty(to.Hata()))
            {
                ve.VeriErisimKapat();
                return null;
            }

            IList<HazineRapor> al = new List<HazineRapor>();
            while (dr.Read())
            {
                HazineRapor hy = new HazineRapor();
                hy.ilKodu = VeriErisim.DegerAl(dr, 0, 0).ToString();
                hy.yil = VeriErisim.DegerAl(dr, 1, 0);
                hy.hibeTutar = VeriErisim.DegerAl(dr, 2, 0.0);

                al.Add(hy);
            }
            dr.Close();

            if (al.Count > 0)
            {
                liste = new HazineRapor[al.Count];
                al.CopyTo(liste, 0);
            }
            else
            {
                liste = new HazineRapor[1];
                liste[0] = new HazineRapor();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc KaydetEksikBelge(ref VeriErisim ve, Kullanici kullanan, int hibeKod, ObjectArray eksikBelge)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            TABGenel tg = new TABGenel(ve);

            ve.BeginTransaction();

            sonuc = to.SilEksikBelge(hibeKod);

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            foreach (HibeYatirimEksikBelge belge in eksikBelge.objeler)
            {
                int kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("HIBEYATIRIMEKSIKBELGE", "0"), 0);

                sonuc = to.KaydetEksikBelge(kod, hibeKod, belge.belgeAdi, belge.belgeNo);
                if (!sonuc.islemSonuc)
                    break;
            }


            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public ObjectArray ListeleEksikBelge(Kullanici kullanan, int hibeKod)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirim to = new TABHibeYatirim(ve);
            IDataReader dr = to.ListeleEksikBelge(hibeKod);

            if (to.Hata() == "")
            {
                while (dr.Read())
                {
                    HibeYatirimEksikBelge eb = new HibeYatirimEksikBelge();
                    eb.kod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarEksikBelge.KOD, 0);
                    eb.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarEksikBelge.HIBEKOD, 0);
                    eb.belgeAdi = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarEksikBelge.BELGEADI, "");
                    eb.belgeNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarEksikBelge.BELGENO, 0);

                    bilgi.objeler.Add(eb);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static Sonuc KaydetGoruntu(ref VeriErisim ve, Kullanici kullanan, Goruntu goruntu)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (!string.IsNullOrEmpty(goruntu.ePosta) && !OrtakFonksiyonlar.MailAdresiDogrumu(goruntu.ePosta))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "e-posta adresi hatalı";
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            if (goruntu.ePosta != "")
            {
                bool kontrol = to.GoruntuEPostaAdresiVarmi(goruntu);
                if (kontrol)
                {
                    Goruntu gKontrol = new Goruntu();
                    gKontrol.ePosta = goruntu.ePosta;
                    ObjectArray gListe = ListeleGoruntu(kullanan, gKontrol);
                    string basvuruNolar = "";
                    foreach (Goruntu g in gListe.objeler)
                    {
                        if (basvuruNolar != "") basvuruNolar += "; ";
                        basvuruNolar += g.basvuruNo;
                    }

                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "e-posta olarak verilen " + goruntu.ePosta + " daha başka bir başvuru için kullanılmıştır. Lütfen yeni bir e-posta adresi giriniz.\r\n\r\n e-postanın tanımlı olduğu başvuru numarası: " + basvuruNolar;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            ve.BeginTransaction();

            sonuc = to.KaydetGoruntu(goruntu);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray ListeleGoruntu(Kullanici kullanan, Goruntu goruntu)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirim to = new TABHibeYatirim(ve);
            IDataReader dr = to.ListeleGoruntu(goruntu);

            if (to.Hata() == "")
            {
                while (dr.Read())
                {
                    Goruntu eb = new Goruntu();
                    eb.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.HIBEKOD, 0);
                    eb.basvuruNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.BASVURUNO, "");
                    eb.ePosta = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.EPOSTAADRESI, "");
                    eb.kamera1Adres = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA1ADRES, "");
                    eb.kamera1Kullanici = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA1KULLANICIADI, "");
                    eb.kamera1Parola = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA1PAROLA, "");
                    eb.kamera2Adres = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA2ADRES, "");
                    eb.kamera2Kullanici = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA2KULLANICIADI, "");
                    eb.kamera2Parola = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA2PAROLA, "");
                    eb.kamera3Adres = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA3ADRES, "");
                    eb.kamera3Kullanici = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA3KULLANICIADI, "");
                    eb.kamera3Parola = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA3PAROLA, "");
                    eb.kamera4Adres = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA4ADRES, "");
                    eb.kamera4Kullanici = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA4KULLANICIADI, "");
                    eb.kamera4Parola = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarGoruntu.KAMERA4PAROLA, "");

                    bilgi.objeler.Add(eb);
                }
            }

            dr.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static string EskiYeniBilgiKarsilastir(HibeYatirim yeni, HibeYatirim eski)
        {
            string sablon = "<tarihce alanAdi=\"{0}\" eski=\"{1}\" yeni=\"{2}\" [ekBilgiler] />";
            string tarihce = "";

            if (yeni.ad != eski.ad) tarihce += string.Format(sablon, "ad", eski.ad, yeni.ad);
            if (yeni.adaNo != eski.adaNo) tarihce += string.Format(sablon, "adaNo", eski.adaNo, yeni.adaNo);
            if (yeni.adres != eski.adres) tarihce += string.Format(sablon, "adres", eski.adres, yeni.adres);
            if (yeni.babaAd != eski.babaAd) tarihce += string.Format(sablon, "babaAd", eski.babaAd, yeni.babaAd);
            if (yeni.dogumTarih.Oku() != eski.dogumTarih.Oku()) tarihce += string.Format(sablon, "dogumTarih", eski.dogumTarih.ToString(), yeni.dogumTarih.ToString());
            if (yeni.ePosta != eski.ePosta) tarihce += string.Format(sablon, "ePosta", eski.ePosta, yeni.ePosta);
            if (yeni.evrakNo != eski.evrakNo) tarihce += string.Format(sablon, "evrakNo", eski.evrakNo, yeni.evrakNo);
            if (yeni.evrakTarih.Oku() != eski.evrakTarih.Oku()) tarihce += string.Format(sablon, "evrakTarih", eski.evrakTarih.ToString(), yeni.evrakTarih.ToString());
            if (yeni.faks != eski.faks) tarihce += string.Format(sablon, "faks", eski.faks, yeni.faks);
            if (yeni.hukukiStatu != eski.hukukiStatu) tarihce += string.Format(sablon, "hukukiStatu", eski.hukukiStatu, yeni.hukukiStatu);
            if (yeni.ilAd != eski.ilAd) tarihce += string.Format(sablon, "ilAd", eski.ilAd, yeni.ilAd);
            if (yeni.ilceAd != eski.ilceAd) tarihce += string.Format(sablon, "ilceAd", eski.ilceAd, yeni.ilceAd);
            if (yeni.koyAd != eski.koyAd) tarihce += string.Format(sablon, "koyAd", eski.koyAd, yeni.koyAd);

            if (yeni.istihdamSayisi != eski.istihdamSayisi) tarihce += string.Format(sablon, "istihdamSayisi", eski.istihdamSayisi, yeni.istihdamSayisi);
            if (yeni.kimlikNo != eski.kimlikNo) tarihce += string.Format(sablon, "kimlikNo", eski.kimlikNo, yeni.kimlikNo);
            if (yeni.koordinat != eski.koordinat) tarihce += string.Format(sablon, "koordinat", eski.koordinat, yeni.koordinat);
            if (yeni.altYapiGrup != eski.altYapiGrup) tarihce += string.Format(sablon, "altYapiGrup", eski.altYapiGrup, yeni.altYapiGrup);
            if (yeni.mevki != eski.mevki) tarihce += string.Format(sablon, "mevki", eski.mevki, yeni.mevki);
            if (yeni.paftaNo != eski.paftaNo) tarihce += string.Format(sablon, "paftaNo", eski.paftaNo, yeni.paftaNo);
            if (yeni.parselNo != eski.parselNo) tarihce += string.Format(sablon, "parselNo", eski.parselNo, yeni.parselNo);
            if (yeni.pNitelik != eski.pNitelik) tarihce += string.Format(sablon, "pNitelik", eski.pNitelik, yeni.pNitelik);
            if (yeni.projeAdi != eski.projeAdi) tarihce += string.Format(sablon, "projeAdi", eski.projeAdi.Replace("\"", ""), yeni.projeAdi.Replace("\"", ""));
            if (yeni.projeAltKonu != eski.projeAltKonu) tarihce += string.Format(sablon, "projeAltKonu", eski.projeAltKonu, yeni.projeAltKonu);
            if (yeni.projeAltKonuAdi != eski.projeAltKonuAdi) tarihce += string.Format(sablon, "projeAltKonuAdi", eski.projeAltKonuAdi, yeni.projeAltKonuAdi);
            if (yeni.projeKonuAdi != eski.projeKonuAdi) tarihce += string.Format(sablon, "projeKonuAdi", eski.projeKonuAdi, yeni.projeKonuAdi);
            if (yeni.projeNitelikAdi != eski.projeNitelikAdi) tarihce += string.Format(sablon, "projeNitelikAdi", eski.projeNitelikAdi, yeni.projeNitelikAdi);
            if (yeni.soyad != eski.soyad) tarihce += string.Format(sablon, "soyad", eski.soyad, yeni.soyad);
            if (yeni.telefon != eski.telefon) tarihce += string.Format(sablon, "telefon", eski.telefon, yeni.telefon);
            if (yeni.vergiDaire != eski.vergiDaire) tarihce += string.Format(sablon, "vergiDaire", eski.vergiDaire, yeni.vergiDaire);


            //if (yeni.etapNo != eski.etapNo) tarihce += string.Format(sablon, "etapNo", eski.etapNo, yeni.etapNo);
            //if (yeni.bankaSubeKod != eski.bankaSubeKod) tarihce += string.Format(sablon, "bankaSubeKod", eski.bankaSubeKod, yeni.bankaSubeKod);
            //if (yeni.basvuruNo != eski.basvuruNo) tarihce += string.Format(sablon, "basvuruNo", eski.basvuruNo, yeni.basvuruNo);
            //if (yeni.dilekceNo != eski.dilekceNo) tarihce += string.Format(sablon, "dilekceNo", eski.dilekceNo, yeni.dilekceNo);
            //if (yeni.faturaNo != eski.faturaNo) tarihce += string.Format(sablon, "faturaNo", eski.faturaNo, yeni.faturaNo);
            //if (yeni.faturaTarih.Oku() != eski.faturaTarih.Oku()) tarihce += string.Format(sablon, "faturaTarih", eski.faturaTarih.ToString(), yeni.faturaTarih.ToString());
            //if (yeni.hedef != eski.hedef) tarihce += string.Format(sablon, "hedef", eski.hedef, yeni.hedef);
            //if (yeni.projeKonu != eski.projeKonu) tarihce += string.Format(sablon, "projeKonu", eski.projeKonu, yeni.projeKonu);
            //if (yeni.projeNitelik != eski.projeNitelik) tarihce += string.Format(sablon, "projeNitelik", eski.projeNitelik, yeni.projeNitelik);
            //if (yeni.yatirimOncelikTur != eski.yatirimOncelikTur) tarihce += string.Format(sablon, "yatirimOncelikTur", eski.yatirimOncelikTur, yeni.yatirimOncelikTur);
            //if (yeni.yil != eski.yil) tarihce += string.Format(sablon, "yil", eski.yil, yeni.yil);
            //if (yeni.kod != eski.kod) tarihce += string.Format(sablon, "kod", eski.kod, yeni.kod);
            //if (yeni.konuNitelikUyusma != eski.konuNitelikUyusma) tarihce += string.Format(sablon, "konuNitelikUyusma", eski.konuNitelikUyusma, yeni.konuNitelikUyusma);
            //if (yeni.koyKod != eski.koyKod) tarihce += string.Format(sablon, "koyKod", eski.koyKod, yeni.koyKod);
            //if (yeni.ilceKod != eski.ilceKod) tarihce += string.Format(sablon, "ilceKod", eski.ilceKod, yeni.ilceKod);
            //if (yeni.ilKod != eski.ilKod) tarihce += string.Format(sablon, "ilKod", eski.ilKod, yeni.ilKod);
            //if (yeni.kasaba != eski.kasaba) tarihce += string.Format(sablon, "kasaba", eski.kasaba, yeni.kasaba);
            //if (yeni.anket1 != eski.anket1) tarihce += string.Format(sablon, "anket1", eski.anket1, yeni.anket1);
            //if (yeni.anket2 != eski.anket2) tarihce += string.Format(sablon, "anket2", eski.anket2, yeni.anket2);
            //if (yeni.anket3 != eski.anket3) tarihce += string.Format(sablon, "anket3", eski.anket3, yeni.anket3);
            //if (yeni.anket3Puan != eski.anket3Puan) tarihce += string.Format(sablon, "anket3Puan", eski.anket3Puan, yeni.anket3Puan);
            //if (yeni.anket4 != eski.anket4) tarihce += string.Format(sablon, "anket4", eski.anket4, yeni.anket4);
            //if (yeni.anket4Puan != eski.anket4Puan) tarihce += string.Format(sablon, "anket4Puan", eski.anket4Puan, yeni.anket4Puan);
            //if (yeni.basvuruSayisi != eski.basvuruSayisi) tarihce += string.Format(sablon, "basvuruSayisi", eski.basvuruSayisi, yeni.basvuruSayisi);
            //if (yeni.basvuruYapildi != eski.basvuruYapildi) tarihce += string.Format(sablon, "basvuruYapildi", eski.basvuruYapildi, yeni.basvuruYapildi);
            //if (yeni.degerlendirmeDisi != eski.degerlendirmeDisi) tarihce += string.Format(sablon, "degerlendirmeDisi", eski.degerlendirmeDisi, yeni.degerlendirmeDisi);
            //if (yeni.degerlendirmeDisiGerekce != eski.degerlendirmeDisiGerekce) tarihce += string.Format(sablon, "degerlendirmeDisiGerekce", eski.degerlendirmeDisiGerekce, yeni.degerlendirmeDisiGerekce);
            //if (yeni.durum != eski.durum) tarihce += string.Format(sablon, "durum", eski.durum, yeni.durum);
            //if (yeni.eskiBasvuru != eski.eskiBasvuru) tarihce += string.Format(sablon, "eskiBasvuru", eski.eskiBasvuru, yeni.eskiBasvuru);
            //if (yeni.eskiEtapdaSozlesmesiBitti != eski.eskiEtapdaSozlesmesiBitti) tarihce += string.Format(sablon, "eskiEtapdaSozlesmesiBitti", eski.eskiEtapdaSozlesmesiBitti, yeni.eskiEtapdaSozlesmesiBitti);
            //if (yeni.eskiEtapdaSozlesmesiDevamEdiyor != eski.eskiEtapdaSozlesmesiDevamEdiyor) tarihce += string.Format(sablon, "eskiEtapdaSozlesmesiDevamEdiyor", eski.eskiEtapdaSozlesmesiDevamEdiyor, yeni.eskiEtapdaSozlesmesiDevamEdiyor);
            //if (yeni.eskiEtapdaSozlesmesiVar != eski.eskiEtapdaSozlesmesiVar) tarihce += string.Format(sablon, "eskiEtapdaSozlesmesiVar", eski.eskiEtapdaSozlesmesiVar, yeni.eskiEtapdaSozlesmesiVar);
            //if (yeni.eskiEtapdaVarmi != eski.eskiEtapdaVarmi) tarihce += string.Format(sablon, "eskiEtapdaVarmi", eski.eskiEtapdaVarmi, yeni.eskiEtapdaVarmi);
            //if (yeni.evrakTarihBaslangic != eski.evrakTarihBaslangic) tarihce += string.Format(sablon, "evrakTarihBaslangic", eski.evrakTarihBaslangic, yeni.evrakTarihBaslangic);
            //if (yeni.evrakTarihBitis != eski.evrakTarihBitis) tarihce += string.Format(sablon, "evrakTarihBitis", eski.evrakTarihBitis, yeni.evrakTarihBitis);
            //if (yeni.grupKod != eski.grupKod) tarihce += string.Format(sablon, "grupKod", eski.grupKod, yeni.grupKod);

            //if (yeni.ipAdresi != eski.ipAdresi) tarihce += string.Format(sablon, "ipAdresi", eski.ipAdresi, yeni.ipAdresi);
            //if (yeni.kabulCalismasi != eski.kabulCalismasi) tarihce += string.Format(sablon, "kabulCalismasi", eski.kabulCalismasi, yeni.kabulCalismasi);
            //if (yeni.odenenInsaat != eski.odenenInsaat) tarihce += string.Format(sablon, "odenenInsaat", eski.odenenInsaat, yeni.odenenInsaat);
            //if (yeni.odenenMal != eski.odenenMal) tarihce += string.Format(sablon, "odenenMal", eski.odenenMal, yeni.odenenMal);
            //if (yeni.online != eski.online) tarihce += string.Format(sablon, "online", eski.online, yeni.online);
            //if (yeni.onlineTuzeleBasvuran != eski.onlineTuzeleBasvuran) tarihce += string.Format(sablon, "onlineTuzeleBasvuran", eski.onlineTuzeleBasvuran, yeni.onlineTuzeleBasvuran);
            //if (yeni.ozet != eski.ozet) tarihce += string.Format(sablon, "ozet", eski.ozet, yeni.ozet);
            //if (yeni.puan != eski.puan) tarihce += string.Format(sablon, "puan", eski.puan, yeni.puan);
            //if (yeni.puanBas != eski.puanBas) tarihce += string.Format(sablon, "puanBas", eski.puanBas, yeni.puanBas);
            //if (yeni.puanBit != eski.puanBit) tarihce += string.Format(sablon, "puanBit", eski.puanBit, yeni.puanBit);
            //if (yeni.redGerekce != eski.redGerekce) tarihce += string.Format(sablon, "redGerekce", eski.redGerekce, yeni.redGerekce);
            //if (yeni.yerindeTespitTarih != eski.yerindeTespitTarih) tarihce += string.Format(sablon, "yerindeTespitTarih", eski.yerindeTespitTarih, yeni.yerindeTespitTarih);
            //if (yeni.komisyonRaporu != eski.komisyonRaporu) tarihce += string.Format(sablon, "komisyonRaporu", eski.komisyonRaporu, yeni.komisyonRaporu);
            //if (yeni.maliyetBilgileri != eski.maliyetBilgileri) tarihce += string.Format(sablon, "maliyetBilgileri", eski.maliyetBilgileri, yeni.maliyetBilgileri);

            //if (yeni.basvuruMaliyet != eski.basvuruMaliyet) tarihce += string.Format(sablon, "basvuruMaliyet", eski.basvuruMaliyet, yeni.basvuruMaliyet);
            //if (yeni.hibeEsasTutar != eski.hibeEsasTutar) tarihce += string.Format(sablon, "hibeEsasTutar", eski.hibeEsasTutar, yeni.hibeEsasTutar);
            //if (yeni.hibeTutar != eski.hibeTutar) tarihce += string.Format(sablon, "hibeTutar", eski.hibeTutar, yeni.hibeTutar);
            //if (yeni.toplamTutar != eski.toplamTutar) tarihce += string.Format(sablon, "toplamTutar", eski.toplamTutar, yeni.toplamTutar);
            //if (yeni.ayniKatki != eski.ayniKatki) tarihce += string.Format(sablon, "ayniKatki", eski.ayniKatki, yeni.ayniKatki);

            //tarihce = tarihce.Substring(0, tarihce.Length - 1);
            //tarihce = tarihce.Replace("|", "; ");

            return tarihce;
        }

        public Sonuc KaydetSozlesmeFesih(Kullanici kullanan, HibeYatirimSozlesmeFesih bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (bilgi.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            ve.BeginTransaction();

            if (bilgi.kod > 0)
                sonuc = to.GuncelleSozlesmeFesih(bilgi);
            else
            {
                bilgi.kod = to.EnBuyukKod();
                sonuc = to.KaydetSozlesmeFesih(bilgi);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                sonuc.anahtar = bilgi.kod.ToString();

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray ListeleSozlesmeFesih(VeriErisim ve, Kullanici kullanan, HibeYatirimSozlesmeFesih kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            IDataReader r = to.ListeleSozlesmeFesih(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeYatirimSozlesmeFesih f = new HibeYatirimSozlesmeFesih();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesmeFesih.KOD, 0);
                    f.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesmeFesih.HIBEKOD, 0);
                    f.tarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesmeFesih.TARIH);
                    f.durum = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesmeFesih.DURUM, 0);
                    f.aciklama = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesmeFesih.ACIKLAMA, string.Empty);
                    f.tutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesmeFesih.TUTAR, 0.0);
                    f.islemYapanAdi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesmeFesih.ISLEMYAPANADI, string.Empty);
                    f.islemTarihi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesmeFesih.ISLEMTARIHI);


                    bilgi.objeler.Add(f);
                }
            }
            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        public Sonuc SilSozlesmeFesih(Kullanici kullanan, HibeYatirimSozlesmeFesih bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (bilgi.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            TABHibeYatirim to = new TABHibeYatirim(ve);

            sonuc = to.SilSozlesmeFesih(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Silme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public ObjectArray ListeleSozlesmeFesihRapor(VeriErisim ve, Kullanici kullanan, HibeYatirimSozlesmeFesih kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            IDataReader r = to.SozlesmeFesihListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeYatirim f = new HibeYatirim();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                    f.yil = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YIL, 0);
                    f.basvuruNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUNO, string.Empty);
                    f.ilKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ILKOD, string.Empty);
                    f.ilceKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ILCEKOD, string.Empty);
                    f.koyKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOYKOD, 0);
                    f.kasaba = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KASABA, string.Empty);
                    f.hukukiStatu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HUKUKISTATU, 0);
                    f.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KIMLIKNO, string.Empty);
                    f.vergiDaire = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.VERGIDAIRE, string.Empty);
                    f.ad = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.AD, string.Empty);
                    f.evrakNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EVRAKNO, string.Empty);
                    f.evrakTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EVRAKTARIH);
                    f.adres = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ADRES, string.Empty);
                    f.telefon = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.TELEFON, string.Empty);
                    f.faks = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FAKS, string.Empty);
                    f.ePosta = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EPOSTA, string.Empty);
                    f.bankaSubeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BANKASUBEKOD, string.Empty);
                    f.durum = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DURUM, 0);
                    f.sozlesmeTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMETARIH);
                    f.faturaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FATURANO, string.Empty);
                    f.faturaTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FATURATARIH);
                    f.soyad = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOYAD, string.Empty);
                    f.babaAd = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BABAAD, string.Empty);
                    f.dogumTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DOGUMTARIH);
                    f.yerindeTespitTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YERINDETESPITTARIH);
                    f.etapNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ETAPNO, 0);

                    f.projeNitelik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJENITELIK, 0);
                    f.projeKonu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEKONU, "");
                    f.projeAltKonu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEALTKONU, "");
                    f.projeAdi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEADI, string.Empty);
                    f.sozlesmeMaliyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMEMALIYET, decimal.Zero);
                    f.basvuruMaliyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUMALIYET, decimal.Zero);
                    f.hibeTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HIBETUTAR, decimal.Zero);
                    f.hibeEsasTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HIBEESASTUTAR, decimal.Zero);
                    f.ayniKatki = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.AYNIKATKI, decimal.Zero);
                    f.toplamTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.TOPLAMTUTAR, decimal.Zero);
                    f.anket1 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET1, 0);
                    f.anket2 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET2, 0);
                    f.anket3 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET3, 0);
                    f.anket4 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET4, 0);
                    f.anket3Puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET3PUAN, 0);
                    f.anket4Puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET4PUAN, 0);
                    f.puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PUAN, (double)0);
                    f.konuNitelikUyusma = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KONUNITELIKUYUSMA, 0) == 1 ? true : false;
                    f.degerlendirmeDisi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISI, 0) == 1 ? true : false;
                    f.degerlendirmeDisiGerekce = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISIGEREKCE, string.Empty);
                    f.kabulCalismasi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KABULCALISMASI, 0);
                    f.onlineTuzeleBasvuran = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ONLINETUZELBASVURUKIMLIKNO, string.Empty);
                    f.pNitelik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PNITELIK, 0);
                    f.mevki = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.MEVKI, string.Empty);
                    f.paftaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PAFTANO, string.Empty);
                    f.adaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ADANO, string.Empty);
                    f.parselNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PARSELNO, string.Empty);
                    f.koordinat = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOORDINAT, string.Empty);
                    f.ozet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.OZET, string.Empty);
                    f.hedef = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HEDEF, string.Empty);
                    f.istihdamSayisi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ISTIHDAM, 0);
                    f.dilekceNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DILEKCENO, "");
                    f.eskiEtapdaVarmi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ESKIETAPDAVARMI, 0);
                    f.basvuruYapildi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUYAPILDI, 0);
                    f.altYapiGrup = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTYAPIGRUP, "");
                    f.cinsiyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.CINSIYET, "");
                    f.engelli = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ENGELLI, "");
                    f.organik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ORGANIK, "");
                    f.yas = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YAS, 0);
                    f.altKonuDetayKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD, "");

                    int sonKolon = ((int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD) + 1;

                    f.projeNitelikAdi = VeriErisim.DegerAl(r, sonKolon, "");
                    f.projeKonuAdi = VeriErisim.DegerAl(r, sonKolon + 1, "");
                    f.projeAltKonuAdi = VeriErisim.DegerAl(r, sonKolon + 2, "");

                    f.koyAd = VeriErisim.DegerAl(r, sonKolon + 3, "");
                    f.ilAd = VeriErisim.DegerAl(r, sonKolon + 4, "");
                    f.ilceAd = VeriErisim.DegerAl(r, sonKolon + 5, "");

                    f.odenenInsaat = VeriErisim.DegerAl(r, sonKolon + 6, (decimal)0);
                    f.odenenMal = VeriErisim.DegerAl(r, sonKolon + 7, (decimal)0);
                    f.sozlesmeFesih.tarih = VeriErisim.DegerAl(r, sonKolon + 8);
                    f.sozlesmeFesih.durum = VeriErisim.DegerAl(r, sonKolon + 9, 0);
                    f.sozlesmeFesih.tutar = VeriErisim.DegerAl(r, sonKolon + 10, 0.0);
                    f.sozlesmeFesih.aciklama = VeriErisim.DegerAl(r, sonKolon + 11, "");

                    sonKolon = sonKolon + 12;
                    HibeYatirimSozlesme hys = new HibeYatirimSozlesme();
                    hys.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.HIBEKOD + sonKolon, 0);
                    hys.sozlesmeTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.SOZLESMETARIH + sonKolon);
                    hys.projeBasTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.PROJEBASTARIH + sonKolon);
                    hys.projeBitTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.PROJEBITTARIH + sonKolon);
                    hys.nihaiRaporTarihi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.NIHAIRAPORTARIHI + sonKolon);
                    hys.istihdam = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.ISTIHDAM + sonKolon, 0);
                    hys.uretimKapasitesi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.URETIMKAPASITESI + sonKolon, (decimal)0);
                    hys.teminatTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTUTAR + sonKolon, (decimal)0);
                    hys.teminatTur = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTUR + sonKolon, 0);
                    hys.teminatTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.TEMINATTARIH + sonKolon);
                    hys.bankaSubeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.BANKASUBEKOD + sonKolon, "");
                    hys.kapasiteBirimKodu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.KAPASITEBIRIM + sonKolon, "");
                    hys.istihdamKadin = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.ISTIHDAMKADIN + sonKolon, 0);
                    hys.durum = (ENUMHibeYatirimSozlesmeDurum)VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.DURUM + sonKolon, 0);
                    hys.yapiKimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesme.YAPIKIMLIKNO + sonKolon, "");
                    f.sozlesmeBilgileri = hys;

                    sonKolon = sonKolon + 19;
                    HibeYatirimMaliyet hym = new HibeYatirimMaliyet();
                    hym.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEKOD + sonKolon, 0);
                    hym.hibeEsasTutarInsaat = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARINSAAT + sonKolon, (decimal)0);
                    hym.hibeEsasTutarMal = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARMAL + sonKolon, (decimal)0);
                    hym.hibeEsasTutarToplam = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARTOPLAM + sonKolon, (decimal)0);
                    hym.ayniKatkiInsaat = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIINSAAT + sonKolon, (decimal)0);
                    hym.ayniKatkiMal = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKIMAL + sonKolon, (decimal)0);
                    hym.ayniKatkiToplam = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.AYNIKATKITOPLAM + sonKolon, (decimal)0);
                    hym.projeToplamTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.PROJETOPLAMTUTAR + sonKolon, (decimal)0);
                    hym.hibeTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBETUTAR + sonKolon, (decimal)0);
                    f.maliyetBilgileri = new HibeYatirimMaliyet[1];
                    f.maliyetBilgileri[0] = hym;

                    //sonKolon = sonKolon + 11;
                    //f.eskiEtapdaSozlesmesiVar = VeriErisim.DegerAl(r, sonKolon, 0);
                    //f.eskiEtapdaSozlesmesiDevamEdiyor = VeriErisim.DegerAl(r, sonKolon + 1, 0);
                    //f.eskiEtapdaSozlesmesiBitti = VeriErisim.DegerAl(r, sonKolon + 2, 0);

                    bilgi.objeler.Add(f);
                }
            }
            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        public static ObjectArray ListeleYatirimciKimlikNo(VeriErisim ve, Kullanici kullanan, string kimlikNo)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            IDataReader r = to.ListeleYatirimciKimlikNo(kimlikNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeYatirimYatirimciKimlikNo f = new HibeYatirimYatirimciKimlikNo();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarYatirimciKimlikNo.KOD, "");
                    f.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarSozlesmeFesih.HIBEKOD, 0);
                    f.eskiKimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarYatirimciKimlikNo.ESKIKIMLIKNO, "");
                    f.yeniKimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlarYatirimciKimlikNo.YENIKIMLIKNO, "");

                    bilgi.objeler.Add(f);
                }
            }
            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        public static string ListeleKonuKodu(VeriErisim ve, int hibeKod)
        {
            Sonuc sonuc = new Sonuc();
            string konu = "";
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);
            IDataReader r = to.ListeleKonuKod(hibeKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    konu = VeriErisim.DegerAl(r, 0, "");
                }
            }
            r.Close();

            ve.VeriErisimKapat();
            return konu;
        }

        public ObjectArray ListeleSozlesmeHakKazananlar(VeriErisim ve, Kullanici kullanan, HibeYatirim kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            IDataReader r = to.SozlesmeHakKazananlar(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeYatirim f = new HibeYatirim();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                    f.basvuruNo = VeriErisim.DegerAl(r, 1, "");
                    f.hukukiStatu = VeriErisim.DegerAl(r, 2, 0);
                    f.altYapiGrup = VeriErisim.DegerAl(r, 3, "");
                    f.projeKonuAdi = VeriErisim.DegerAl(r, 4, "");
                    f.projeAltKonuAdi = VeriErisim.DegerAl(r, 5, "");
                    f.projeNitelikAdi = VeriErisim.DegerAl(r, 6, "");
                    f.ilAd = VeriErisim.DegerAl(r, 7, "");
                    f.ilceAd = VeriErisim.DegerAl(r, 8, "");
                    f.kimlikNo = VeriErisim.DegerAl(r, 9, "");
                    f.ad = VeriErisim.DegerAl(r, 10, "");
                    f.soyad = VeriErisim.DegerAl(r, 11, "");
                    f.ePosta = VeriErisim.DegerAl(r, 12, "");
                    f.dogumTarih = VeriErisim.DegerAl(r, 13);
                    f.projeNitelik = VeriErisim.DegerAl(r, 14, 0);
                    f.projeKonu = VeriErisim.DegerAl(r, 15, "");
                    f.projeAltKonuAdi = VeriErisim.DegerAl(r, 16, "");
                    f.projeAdi = VeriErisim.DegerAl(r, 17, "");
                    f.basvuruMaliyet = VeriErisim.DegerAl(r, 18, 0);
                    f.hibeTutar = VeriErisim.DegerAl(r, 19, 0);
                    f.hibeEsasTutar = VeriErisim.DegerAl(r, 20, 0);
                    f.ayniKatki = VeriErisim.DegerAl(r, 21, 0);
                    f.toplamTutar = VeriErisim.DegerAl(r, 22, 0);
                    f.pNitelik = VeriErisim.DegerAl(r, 23, 0);
                    f.istihdamSayisi = VeriErisim.DegerAl(r, 24, 0);
                    f.puan = VeriErisim.DegerAl(r, 25, 0);
                    f.anket1 = VeriErisim.DegerAl(r, 26, 0);
                    f.anket2 = VeriErisim.DegerAl(r, 27, 0);
                    f.anket3 = VeriErisim.DegerAl(r, 28, 0);
                    f.anket4 = VeriErisim.DegerAl(r, 29, 0);
                    f.kabulCalismasi = VeriErisim.DegerAl(r, 30, 0);

                    //Malzemeleri ekle
                    string malzeme = VeriErisim.DegerAl(r, 31, "");
                    if (!string.IsNullOrEmpty(malzeme))
                    {
                        string[] malzemeler = malzeme.Split('|');
                        List<YatirimMalzeme> mler = new List<YatirimMalzeme>();
                        foreach (var mal in malzemeler)
                        {
                            if (string.IsNullOrEmpty(mal)) continue;

                            string[] mallar = mal.Split('^');

                            YatirimMalzeme m = new YatirimMalzeme();
                            m.marka = mallar[0];
                            m.malzeme.ad = mallar[1];

                            mler.Add(m);
                        }
                        f.malzeme = mler.ToArray();
                    }
                    //////////////////////////


                    bilgi.objeler.Add(f);
                }
            }
            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }

        public ObjectArray ListeleIldeReddilenler(VeriErisim ve, Kullanici kullanan, HibeYatirim kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirim to = new TABHibeYatirim(ve);

            IDataReader r = to.IldeReddilenler(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeYatirim f = new HibeYatirim();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                    f.basvuruNo = VeriErisim.DegerAl(r, 1, "");
                    f.hukukiStatu = VeriErisim.DegerAl(r, 2, 0);
                    f.altYapiGrup = VeriErisim.DegerAl(r, 3, "");
                    f.projeKonuAdi = VeriErisim.DegerAl(r, 4, "");
                    f.projeAltKonuAdi = VeriErisim.DegerAl(r, 5, "");
                    f.projeNitelikAdi = VeriErisim.DegerAl(r, 6, "");
                    f.ilAd = VeriErisim.DegerAl(r, 7, "");
                    f.ilceAd = VeriErisim.DegerAl(r, 8, "");
                    f.kimlikNo = VeriErisim.DegerAl(r, 9, "");
                    f.ad = VeriErisim.DegerAl(r, 10, "");
                    f.soyad = VeriErisim.DegerAl(r, 11, "");
                    f.ePosta = VeriErisim.DegerAl(r, 12, "");
                    f.dogumTarih = VeriErisim.DegerAl(r, 13);
                    f.projeNitelik = VeriErisim.DegerAl(r, 14, 0);
                    f.projeKonu = VeriErisim.DegerAl(r, 15, "");
                    f.projeAltKonuAdi = VeriErisim.DegerAl(r, 16, "");
                    f.projeAdi = VeriErisim.DegerAl(r, 17, "");
                    f.basvuruMaliyet = VeriErisim.DegerAl(r, 18, 0);
                    f.hibeTutar = VeriErisim.DegerAl(r, 19, 0);
                    f.hibeEsasTutar = VeriErisim.DegerAl(r, 20, 0);
                    f.ayniKatki = VeriErisim.DegerAl(r, 21, 0);
                    f.toplamTutar = VeriErisim.DegerAl(r, 22, 0);
                    f.pNitelik = VeriErisim.DegerAl(r, 23, 0);
                    f.istihdamSayisi = VeriErisim.DegerAl(r, 24, 0);
                    f.puan = VeriErisim.DegerAl(r, 25, 0);
                    f.anket1 = VeriErisim.DegerAl(r, 26, 0);
                    f.anket2 = VeriErisim.DegerAl(r, 27, 0);
                    f.anket3 = VeriErisim.DegerAl(r, 28, 0);
                    f.anket4 = VeriErisim.DegerAl(r, 29, 0);
                    f.kabulCalismasi = VeriErisim.DegerAl(r, 30, 0);

                    bilgi.objeler.Add(f);
                }
            }
            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeSulama
    {
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));

            //if (!Yetki.YetkiVarMiYazmaya(kullanan, bilgi.ilKod, -1, string.Empty, ENUMHibeTur.SULAMA))
            if (!Yetki.YetkiVarMiYazmaya(kullanan, bilgi.ilKod, -1, sonuc, string.Empty, ENUMHibeTur.SULAMA, bilgi.kod, false))
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            bool yonetici = false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))
                yonetici = true;

            int aktifEtapNo = TRMHibeSulamaYonetim.HibeSulamaAktifEtapNo(ve);
            Yonetim y = TRMHibeSulamaYonetim.Oku(ve, kullanan, new Yonetim() { etapNo = aktifEtapNo });
            TNSDateTime bugun = new TNSDateTime(DateTime.Now.ToShortDateString());
            if (bugun.Oku() >= y.onlineGirisiKapatmaTarihi.Oku() && !bilgi.yazilimIciKayit && !yonetici)
            {
                sonuc.hataStr = "Yeni başvuru kaydı veya daha önce kayıt edilmiş başvuruda değişiklik yapılması için izin verilen tarihi geçmiştir.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            Sonuc cezaliMi = new TRMHibeCeza().CezaliMi(bilgi.kimlikNo);
            if (cezaliMi.islemSonuc)
            {
                sonuc.hataStr = cezaliMi.hataStr;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            HibeSulama hibeSulamaOncekiHali = new HibeSulama();
            if (bilgi.projeNo == "" && bilgi.kod > 0)
            {
                ObjectArray hsOnceki = Listele(kullanan, new HibeSulama() { etapNo = bilgi.etapNo, kod = bilgi.kod });
                hibeSulamaOncekiHali = (HibeSulama)hsOnceki[0];

                //string kapali = TRMHibeSulamaYonetim.HibeSulamaOnlineKapandiMi(ve);
                string kapali = hibeSulamaOncekiHali.kabulEdildi;
                if (kapali == "K")
                {
                    sonuc.hataStr = "Başvuruya Kabul edildi/Onayladı işlemi yapıldığı için değişiklik yapılamaz.";
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }


            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

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
            HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
            tarihce.kullaniciKod = kullanan.kullaniciKodu;
            tarihce.tarih = new TNSDateTime(DateTime.Now);
            tarihce.durum = bilgi.durum;

            //int sulamaGrubuKod = 0;
            //if (bilgi.detay != null && bilgi.detay.Length > 0)
            //    sulamaGrubuKod = bilgi.detay[0].sulamaGrubuKod;

            sonuc = KontrolKimlikNoKayitVarMi(ve, kullanan, bilgi.kod, bilgi.kimlikNo, bilgi.etapNo, bilgi.hukukiStatu, 0, bilgi.ilKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            sonuc = KontrolEvrakKaydi(ve, kullanan, bilgi.kod, bilgi.kimlikNo, bilgi.evrakNo, bilgi.evrakTarih, bilgi.etapNo, bilgi.ilKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            sonuc.bilgiStr = "";
            sonuc = KontrolParsel(ve, kullanan, bilgi);
            string parselUyari = "";
            if (sonuc.bilgiStr != "")
            {
                parselUyari = sonuc.bilgiStr;
            }
            //if (!sonuc.islemSonuc)
            //    return sonuc;

            bool yeni = false;
            if (bilgi.kod == 0)
            {
                yeni = true;
                bilgi.yil = DateTime.Now.Year;
                bilgi.kod = BelgeNoAlInt(ve);
                string plaka = new TABGenel(ve).IlPlakaKodBul(bilgi.ilKod); //TRMServis.UzayYaziDegerGetir("COGRAFYA", bilgi.ilKod, true, "PLAKAKOD");
                bilgi.basvuruNo = BasvuruNoAl(ve, bilgi.etapNo, plaka);
                bilgi.durum = (int)ENUMHibeSulamaBasvuruDurum.YENI;

                tarihce.durum = bilgi.durum;
            }
            else
            {
                bilgi.durum = to.ListeleDurum(bilgi.kod);
                if (bilgi.durum == (int)ENUMHibeSulamaBasvuruDurum.BASVURUREDDEDILDI)
                    bilgi.durum = (int)ENUMHibeSulamaBasvuruDurum.YENI;
                else
                    bilgi.durum = (int)ENUMHibeSulamaBasvuruDurum.TANIMSIZ;
                tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.DEGISTIRILDI;
            }

            tarihce.hibeKod = bilgi.kod;

            if (yeni)
                sonuc = to.Kaydet(bilgi);
            else
                sonuc = to.Guncelle(bilgi);

            if (sonuc.islemSonuc && bilgi.detay.Length > 0)
            {
                if (!yeni) //Eğer güncelleme yapılıyorsa silme işlemi yapılsın. Yeni başvuru kaydında kayıtlı detay bilgisi yoktur
                {
                    foreach (var detay in bilgi.detay)
                        sonuc = to.SilDetay(bilgi.kod);
                }

                if (sonuc.islemSonuc)
                {
                    //HibeSulamaDetay p = new HibeSulamaDetay();
                    //if (p.satirNo <= 0)
                    //    p.satirNo = to.YeniDetayKodVer();
                    foreach (var detay in bilgi.detay)
                    {
                        HibeTutarHesapla(bilgi.hukukiStatu, ref detay.hibeEsasTutarProje, ref detay.hibeEsasTutarUygulama, detay.proformaTutar, detay.malzemeTutar, ref detay.hibeEsasTutar, ref detay.toplamTutar, ref detay.ayniKatki, ref detay.hibeTutar, detay.hibeOran, detay.basvuruAyniKatkiTutar);

                        sonuc = to.KaydetDetay(detay, bilgi.kod);

                        //detayKonu.detayKod = p.satirNo;

                        //p.hibeKod = bilgi.kod;
                        //p.sulamaGrubuKod = detayKonu.sulamaGrubuKod;
                        //p.proformaTutar += detayKonu.proformaTutar;
                        //p.hibeEsasTutarProje += detayKonu.hibeEsasTutarProje;
                        ////p.hibeEsasTutarProje = Genel.DegerAlDecimal(row, "HibeEsasTutarProje");
                        //p.proformaReferansTutar += detayKonu.proformaReferansTutar;
                        ////p.malzemeSeriNo = detayKonu.malzemeSeriNo;
                        //p.firmaVergiNo = detayKonu.firmaVergiNo;
                        //p.firmaAd = detayKonu.firmaAd;
                        //p.aciklama = detayKonu.aciklama;
                        //p.hibeOran = detayKonu.hibeOran;

                        ////p.basvuruToplamMalAlimTutar = OrtakFonksiyonlar.ConvertToDecimal(txtBasvuruToplamMalAlimTutari.Text, 0);
                        ////p.basvuruHibeyeEsasMalAlimTutar = OrtakFonksiyonlar.ConvertToDecimal(txtBasvuruHibeyeEsasMalAlimTutari.Text, 0);
                        ////p.basvuruAyniKatkiTutar = OrtakFonksiyonlar.ConvertToDecimal(txtBasvuruAyniKatkiTutari.Text, 0);
                        ////p.basvuruTalepEdilenHibeTutar = OrtakFonksiyonlar.ConvertToDecimal(txtBasvuruTalepEdilenHibeTutari.Text, 0);

                        //sonuc = to.KaydetDetayKonu(detayKonu, bilgi.kod);


                        //if (sonuc.islemSonuc)
                        //{
                        //    sonuc = to.KaydetDetayKonu(bilgi.detayKonu, bilgi.kod);
                        //    break;
                        //}
                    }

                    sonuc = to.KaydetTarihce(tarihce);
                }
            }

            if (sonuc.islemSonuc)
                sonuc = KaydetDiger(ve, kullanan, bilgi.diger, bilgi.kod);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı." + parselUyari;
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

        private void DebugYaz(HibeSulama bilgi, HibeSulamaDetay detay, Sonuc sonuc, string ek="")
        {
            if (bilgi.kod == 73813)
            {
                OrtakFonksiyonlar.HataDosyaYaz("SulamaDbg.txt", $"{bilgi.ad} - {(detay == null ? 0 : detay.basvuruHibeyeEsasMalAlimTutar)} - {sonuc.hataStr} - {ek}", false);
            }
        }

        private Sonuc KontrolParsel(VeriErisim ve, Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);

            foreach (HibeSulamaDiger item in bilgi.diger)
            {
                string parseller = "";
                int kosulYil = 5;

                if (item.sulamaGrupKod == 999) continue;

                if (item.sulamaGrupKod == 1 || item.sulamaGrupKod == 2 || item.sulamaGrupKod == 3 || item.sulamaGrupKod == 100)
                    kosulYil = 3;
                else
                    kosulYil = 5;

                string[] pler = item.parselNo.Trim().Split(',', '-');
                foreach (var p in pler)
                {
                    if (string.IsNullOrWhiteSpace(p.Trim())) continue;

                    if (parseller != "") parseller += ",";
                    parseller += "'" + p.Trim() + "'";
                }

                if (string.IsNullOrWhiteSpace(parseller)) continue;

                int etapNo = bilgi.etapNo;
                if (etapNo == 0) etapNo = TRMHibeSulamaYonetim.HibeSulamaAktifEtapNo(ve);

                IDataReader r = to.KontrolParsel(bilgi, parseller, kosulYil, etapNo);

                HibeSulama hm = new HibeSulama();

                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        hm.etapNo = VeriErisim.DegerAl(r, 0, 0);
                        hm.kimlikNo = VeriErisim.DegerAl(r, 1, "");
                        hm.ilKod = VeriErisim.DegerAl(r, 2, "");
                        hm.ilceKod = VeriErisim.DegerAl(r, 3, "");
                        hm.koyKod = VeriErisim.DegerAl(r, 4, 0);
                        hm.parselMevkisi = VeriErisim.DegerAl(r, 5, "");
                    }
                }
                r.Close();

                if (hm.etapNo > 0)
                {
                    //sonuc.islemSonuc = false;
                    sonuc.bilgiStr += $"<br><br>{hm.parselMevkisi} nolu parsel bilgisi için önceki yıllarda başvuru yapılmış. Kontrol ediniz.<br><br>";
                    sonuc.bilgiStr += $"Kimlik Numarası {bilgi.kimlikNo} olan {bilgi.ad} {bilgi.soyad}, son {kosulYil} yıl içinde ({hm.etapNo}. Etap) {hm.parselMevkisi} nolu parselden <b>destek aldığı için bu parselden başvuru yapamayabilir. Son tebliğ kurallarına göre Kontrol ediniz.</b><br><br><br>";
                }
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc KontrolOncedenReddilenDestek(VeriErisim ve, Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);

            //foreach (HibeSulamaDiger item in bilgi.diger)
            {
                string parseller = "";
                int kosulYil = 3;

                //if (item.sulamaGrupKod == 1 || item.sulamaGrupKod == 2 || item.sulamaGrupKod == 3 || item.sulamaGrupKod == 100)
                //    kosulYil = 3;
                //else
                //    kosulYil = 5;

                //string[] pler = item.parselNo.Trim().Split(',', '-');
                //foreach (var p in pler)
                //{
                //    if (string.IsNullOrWhiteSpace(p.Trim())) continue;

                //    if (parseller != "") parseller += ",";
                //    parseller += "'" + p.Trim() + "'";
                //}

                //if (string.IsNullOrWhiteSpace(parseller)) continue;

                int etapNo = bilgi.etapNo;
                if (etapNo == 0) etapNo = TRMHibeSulamaYonetim.HibeSulamaAktifEtapNo(ve);

                IDataReader r = to.KontrolOncedenReddilenDestek(bilgi, kosulYil, etapNo);

                HibeSulama hm = new HibeSulama();

                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        hm.etapNo = VeriErisim.DegerAl(r, 0, 0);
                        hm.kimlikNo = VeriErisim.DegerAl(r, 1, "");
                        //hm.ilKod = VeriErisim.DegerAl(r, 2, "");
                        //hm.ilceKod = VeriErisim.DegerAl(r, 3, "");
                        //hm.koyKod = VeriErisim.DegerAl(r, 4, 0);
                        //hm.parselMevkisi = VeriErisim.DegerAl(r, 5, "");
                    }
                }
                r.Close();

                if (hm.etapNo > 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr += $"DİKKAT: Kimlik Numarası {bilgi.kimlikNo} olan {bilgi.ad} {bilgi.soyad}, son {kosulYil} yıl içinde ({hm.etapNo}. Etap) <b>Reddilen başvuru olduğu için kontrolünü yapınız.</b><br><br><br>";
                    //break;
                }
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc KaydetDiger(VeriErisim ve, Kullanici kullanan, HibeSulamaDiger[] diger, int hibeKod)
        {
            if (diger == null || diger.Length == 0)
                return new Sonuc();

            foreach (HibeSulamaDiger d in diger)
                d.hibeKod = hibeKod;
            return TRMHibeSulamaDiger.Kaydet(ve, kullanan, diger);
        }
        /// <summary>
        /// Arguman olarak verilen, hibe Sulama sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetSozlesme(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeSulama hm = new HibeSulama();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "Sozlesme");

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();


            //Sözleşme yapılabilir mi?
            hata = TRMHibeSulamaYonetim.HibeSulamaSozlesmeKapandiMi(null);

            if (hata != "")
            {
                ve.Rollback();
                ve.VeriErisimKapat();

                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            //////////////////////////////////////////////////////////////////////////////

            hm.sozlesmeTarih = bilgi.sozlesmeTarih;
            hm.durum = (int)ENUMHibeSulamaBasvuruDurum.SOZLESMEYAPILDI;

            sonuc = to.KaydetSozlesme(hm);

            if (sonuc.islemSonuc && hm.detay != null && hm.detay.Length > 0)
            {
                foreach (var detay in hm.detay)
                {
                    HibeTutarHesapla(hm.hukukiStatu, ref detay.hibeEsasTutarProje, ref detay.hibeEsasTutarUygulama, detay.proformaTutar, detay.malzemeTutar, ref detay.hibeEsasTutar, ref detay.toplamTutar, ref detay.ayniKatki, ref detay.hibeTutar, detay.hibeOran, detay.basvuruAyniKatkiTutar);
                    sonuc = to.KaydetHibeTutarlari(detay, hm.kod);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.SOZLESMEYAPILDI;
                tarihce.hibeKod = hm.kod;

                sonuc = to.KaydetTarihce(tarihce);
            }

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
        /// Arguman olarak verilen, hibe Sulama proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetProjeNo(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeSulama hm = new HibeSulama();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "ProjeNo");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            if (string.IsNullOrEmpty(hm.projeNo))
            {
                string plakaKod = new TABGenel(ve).IlPlakaKodBul(hm.ilKod); //TRMServis.UzayYaziDegerGetir("COGRAFYA", hm.ilKod, true, "PLAKAKOD");
                hm.projeNo = ProjeNoAl(ve, hm.etapNo, plakaKod, hm.detay[0].sulamaGrubuKod);
            }
            hm.durum = (int)ENUMHibeSulamaBasvuruDurum.PROJENUMARASIVERILDI;

            sonuc = to.KaydetProjeNo(hm);

            if (sonuc.islemSonuc)
            {
                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.PROJENUMARASIVERILDI;
                tarihce.hibeKod = hm.kod;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                sonuc.anahtar = hm.projeNo.ToString();

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
        private static Sonuc MalzemelerBasvuruyaUygunMu(Kullanici kullanan, HibeSulama hs, ENUMHibeSulamaMalzemeTip tip)
        {
            if (hs == null || hs.detay == null || hs.detay.Length == 0)
                return new Sonuc(false, "Başvuru bilgileriniz eksik görünüyor, lütfen kontrol ediniz.", string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            HibeSulamaMalzemeFiyat[] mfler = TRMHibeSulamaMalzeme.Listele(ve, kullanan, hs.kod, tip, hs.ilKod, 0, "");
            if (mfler == null || mfler.Length == 0)
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, "Lütfen önce, başvurunuza ait malzeme detay bilgilerini giriniz.", string.Empty);
            }

            SulamaMalzeme kriter = new SulamaMalzeme();
            kriter.gruplar = new SulamaMalzemeGrup[1];
            kriter.gruplar[0] = new SulamaMalzemeGrup();
            kriter.gruplar[0].grupKod = hs.detay[0].sulamaGrubuKod;

            SulamaMalzeme[] mler = TRMSulamaMalzeme.Listele(ve, kullanan, kriter, true);
            ve.VeriErisimKapat();
            if (mler == null || mler.Length == 0)
                return new Sonuc(false, "Başvurunuza ait malzeme detay bilgileri ile kıyaslanacak malzeme bilgileri bulunamadı.", string.Empty);

            bool bulunduMalzeme = false;

            foreach (HibeSulamaMalzemeFiyat mf in mfler)
            {
                foreach (SulamaMalzeme m in mler)
                {
                    if (mf.malzemeKod != m.kod)
                        continue;

                    bulunduMalzeme = true;
                    foreach (HibeSulamaMalzemeOzellik mfo in mf.ozellikler)
                    {
                        bool bulunduOzellik = false;
                        foreach (SulamaMalzemeOzellik mo in m.ozellikler)
                        {
                            if (mfo.ozellikKod != mo.kod)
                                continue;

                            bulunduOzellik = true;
                            break;
                        }
                        if (!bulunduOzellik)
                        {
                            bulunduMalzeme = false;
                            break;
                        }
                    }
                    //break;
                }
            }
            if (!bulunduMalzeme)
                return new Sonuc(false, "Başvurunuza ait malzeme detay bilgileri ile başvuru konusu uyuşmuyor. Lütfen, malzeme detay bilgilerinizi güncelleyin.", string.Empty);

            return new Sonuc();
        }

        /// <summary>
        /// Arguman olarak verilen, hibe sulama proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetEksikHibeMiktar(Kullanici kullanan, HibeSulamaDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            HibeSulama hibe = new HibeSulama();
            hibe.kod = bilgi.hibeKod;
            ObjectArray liste = Listele(kullanan, hibe);
            if (liste.objeler.Count == 1)
                hibe = (HibeSulama)liste.objeler[0];

            //Ödeme işlemi başladı mı?
            HibeMakineSulamaGrup hg = new HibeMakineSulamaGrup();
            hg.ilKod = hibe.ilKod;
            hg.kod = hibe.basvuruNo;
            HibeMakineSulamaGrup[] gruplar = TRMHibeMakineSulamaGrup.Listele(ve, kullanan, hg, null);
            if (gruplar.Length == 0 && bilgi.gercekHibeTutar == 0)//Eğer silme için geldi ise
            {
                sonuc.hataStr = "Eksik ödeme kaydı bulunamadı.";
                ve.VeriErisimKapat();
                return sonuc;
            }
            foreach (HibeMakineSulamaGrup gg in gruplar)
            {
                if (gg.durum > (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER)
                {
                    sonuc.hataStr = "Bu kayıt, ödemeye işlemine girdiği için işlem yapılamaz.";
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            decimal odenen = OrtakFonksiyonlar.Yuvarla((hibe.detay[0].toplamTutar - hibe.detay[0].basvuruAyniKatkiTutar - (hibe.detay[0].toplamTutar - hibe.detay[0].gercekFatura)) / 2, 2);
            decimal odenecek = OrtakFonksiyonlar.Yuvarla(hibe.detay[0].hibeEsasTutarProje / 2, 2);
            decimal kontrol = odenecek - (bilgi.eksikHibeTutar + odenen);

            if (bilgi.eksikHibeTutar > 0)
                bilgi.gercekHibeTutar = bilgi.eksikHibeTutar + odenen;

            if (kontrol < 0)
            {
                sonuc.hataStr = "Girilen fark tutarı (" + bilgi.eksikHibeTutar.ToString("#,###.##") + "TL), önceden ödenen (" + odenen.ToString("#,###.##") + "TL) ile birlikte sözleşme tutarından (" + odenecek.ToString("#,###.##") + "TL) büyük olduğu için işlem yapılamaz.";
                ve.VeriErisimKapat();
                return sonuc;
            }


            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetEksikHibeMiktar(bilgi);

            if (sonuc.islemSonuc)
            {
                TABHibeMakineSulamaGrup tGrup = new TABHibeMakineSulamaGrup(ve);

                HibeMakineSulamaGrup g = new HibeMakineSulamaGrup();
                g.durum = (int)ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER;
                g.ilKod = hibe.ilKod;
                g.odemeKod = 0;
                g.kod = hibe.basvuruNo;
                g.tipi = (int)ENUMHibeTur.SULAMA;

                sonuc = tGrup.Sil(g);
                if (bilgi.gercekHibeTutar > 0)
                    sonuc = tGrup.Kaydet(g);

                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = 101;
                if (bilgi.gercekHibeTutar > 0)
                    tarihce.aciklama = "Eksik/Fazla ödeme bilgisi girildi. Doğru hibe miktarı:" + bilgi.gercekHibeTutar.ToString("#,###.00") + "; Eksik/Fazla ödenen tutar:" + bilgi.eksikHibeTutar.ToString("#,###.00");
                else
                    tarihce.aciklama = "Eksik/Fazla ödeme bilgisi kaldırıldı";
                tarihce.hibeKod = bilgi.hibeKod;

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

        /// <summary>
        /// Arguman olarak verilen, hibe Sulama proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetReddedildi(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeSulama hm = new HibeSulama();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "Red");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            hm.redGerekce = bilgi.redGerekce;
            hm.durum = (int)ENUMHibeSulamaBasvuruDurum.BASVURUREDDEDILDI;
            hm.redTip = bilgi.redTip;

            sonuc = to.KaydetReddedildi(hm);
            if (sonuc.islemSonuc)
            {
                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.BASVURUREDDEDILDI;
                tarihce.hibeKod = hm.kod;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                sonuc.anahtar = hm.projeNo.ToString();

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

        public Sonuc KaydetAsilYedek(Kullanici kullanan, int[] hibeKodlar, string asilYedek)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            for (int i = 0; i < hibeKodlar.Length; i++)
            {
                HibeSulama bilgi = new HibeSulama();
                HibeSulama hm = new HibeSulama();
                bilgi.kod = hibeKodlar[i];
                string hata = IslemKontrol(kullanan, bilgi, ref hm, "AsilYedek");
                if (hata != "")
                {
                    sonuc.hataStr = hata;
                    sonuc.islemSonuc = false;
                    break;
                }

                sonuc = to.KaydetAsilYedek(hibeKodlar[i], asilYedek);
                if (sonuc.islemSonuc)
                {
                    HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                    tarihce.kullaniciKod = kullanan.kullaniciKodu;
                    tarihce.tarih = new TNSDateTime(DateTime.Now);
                    tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.ASILYEDEK;

                    if (asilYedek == "A")
                        tarihce.aciklama = "Asil olarak ayarlandı";
                    else if (asilYedek == "Y")
                        tarihce.aciklama = "Yedek olarak ayarlandı";
                    else
                        tarihce.aciklama = "Boş olarak ayarlandı";

                    tarihce.hibeKod = hm.kod;

                    sonuc = to.KaydetTarihce(tarihce);
                }
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

        public Sonuc KaydetKabul(Kullanici kullanan, int[] hibeKodlar, string kabulEdildi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            for (int i = 0; i < hibeKodlar.Length; i++)
            {
                HibeSulama bilgi = new HibeSulama();
                HibeSulama hm = new HibeSulama();
                bilgi.kod = hibeKodlar[i];
                string hata = IslemKontrol(kullanan, bilgi, ref hm, "kabulEdildi");
                if (hata != "")
                {
                    sonuc.hataStr = hata;
                    sonuc.islemSonuc = false;
                    break;
                }

                sonuc = to.KaydetKabul(hibeKodlar[i], kabulEdildi);
                if (sonuc.islemSonuc)
                {
                    HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                    tarihce.kullaniciKod = kullanan.kullaniciKodu;
                    tarihce.tarih = new TNSDateTime(DateTime.Now);
                    tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.ASILYEDEK;

                    if (kabulEdildi == "K")
                        tarihce.aciklama = "Kabul Edildi olarak ayarlandı";
                    else if (kabulEdildi == "R")
                        tarihce.aciklama = "Reddedildi olarak ayarlandı";
                    else
                        tarihce.aciklama = "Boş olarak ayarlandı";

                    tarihce.hibeKod = hm.kod;

                    sonuc = to.KaydetTarihce(tarihce);
                }
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

        public Sonuc KaydetBasvuruYapildi(Kullanici kullanan, int hibeKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeSulama bilgi = new HibeSulama();
            HibeSulama hm = new HibeSulama();
            bilgi.kod = hibeKod;
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "BasvuruYapildi");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetDurumDegistir(hibeKod, (int)ENUMHibeSulamaBasvuruDurum.YENI);

            //////////////  Başvuru durumuna getirildiğinde fatura referans fiyatlarının  ////
            ////////////    başvuru referans fiyatlar olarak güncellenmesi   ////////////////
            string fiyatBirimReferansIlkDeger = System.Configuration.ConfigurationManager.AppSettings["HibeBirimReferansIlkDeger"];
            if ((!string.IsNullOrEmpty(fiyatBirimReferansIlkDeger)) && (fiyatBirimReferansIlkDeger == "1"))
            {
                if (sonuc.islemSonuc)
                    sonuc = to.KaydetDurumBasvuruReferans2FaturaReferans(hibeKod);
            }
            ///////////////////////////////////////////////////////////////////////////////

            if (sonuc.islemSonuc)
            {
                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.YENI;

                tarihce.hibeKod = hibeKod;

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

        //public Sonuc KaydetParselReddedildi(Kullanici kullanan, HibeSulama bilgi)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    HibeSulama hm = new HibeSulama();
        //    ObjectArray hmler = Listele(kullanan, new HibeSulama { kod = bilgi.kod });
        //    hm = (HibeSulama)hmler.objeler[0];

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

        //    HibeSulamaMalzemeFiyat[] liste = TRMHibeSulamaMalzeme.Listele(ve, kullanan, hm.kod, 0, hm.ilKod, 0, "");

        //    HibeSulamaMalzemeFiyat toplamMf = new HibeSulamaMalzemeFiyat();
        //    decimal projeToplamPRJ = 0;
        //    decimal hibeEsasToplamPRJ = 0;
        //    decimal projeToplamUYG = 0;
        //    decimal hibeEsasToplamUYG = 0;
        //    foreach (HibeSulamaMalzemeFiyat mf in liste)
        //    {
        //        if (mf.sulamaGrupKod == 0) continue;

        //        if (mf.tip == ENUMHibeSulamaMalzemeTip.PROJE)
        //        {
        //            projeToplamPRJ += mf.projeTutar;
        //            hibeEsasToplamPRJ += mf.hibeEsasTutar;
        //        }
        //        else
        //        {
        //            projeToplamUYG += mf.projeTutar;
        //            hibeEsasToplamUYG += mf.hibeEsasTutar;
        //        }
        //    }


        //    if ("1" == "PROJE")
        //    {
        //        hm.detay[0].proformaTutar = projeToplamPRJ;
        //        hm.detay[0].hibeEsasTutarProje = hibeEsasToplamPRJ;
        //        hm.detay[0].proformaReferansTutar = hibeEsasToplamPRJ;
        //    }
        //    else
        //    {
        //        hm.detay[0].malzemeTutar = projeToplamUYG;
        //        hm.detay[0].hibeEsasTutarUygulama = hibeEsasToplamUYG;
        //        hm.detay[0].gercekFatura = hibeEsasToplamUYG;
        //    }
        //    HibeTutarHesapla(hm.hukukiStatu, ref hm.detay[0].hibeEsasTutarProje, ref hm.detay[0].hibeEsasTutarUygulama, hm.detay[0].proformaTutar, hm.detay[0].malzemeTutar, ref hm.detay[0].hibeEsasTutar, ref hm.detay[0].toplamTutar, ref hm.detay[0].ayniKatki, ref hm.detay[0].hibeTutar, hm.detay[0].hibeOran, hm.detay[0].basvuruAyniKatkiTutar);

        //    TABHibeSulama to = new TABHibeSulama(ve);
        //    ve.BeginTransaction();

        //    GuncelleDetay(kullanan,)
        //    sonuc = to.GuncelleDetay(hm.detay[0], hm.kod);

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        public Sonuc GuncelleDetay(Kullanici kullanan, HibeSulamaDetay bilgi, ENUMHibeSulamaMalzemeTip tip)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            HibeTutarHesapla(1, ref bilgi.hibeEsasTutarProje, ref bilgi.hibeEsasTutarUygulama, bilgi.proformaTutar, bilgi.malzemeTutar, ref bilgi.hibeEsasTutar, ref bilgi.toplamTutar, ref bilgi.ayniKatki, ref bilgi.hibeTutar, bilgi.hibeOran, bilgi.basvuruAyniKatkiTutar);
            sonuc = to.GuncelleDetay(bilgi, bilgi.hibeKod, tip);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                sonuc.anahtar = bilgi.hibeKod.ToString();

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

        ////Grup İşlemleri**************************
        public Sonuc GrupKaydet(Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeSulama gelen = new HibeSulama();
            gelen.kod = bilgi.hibeKod;

            HibeSulama hm = new HibeSulama();
            string hata = IslemKontrol(kullanan, gelen, ref hm, "Grup");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            //Grup bilgisi kontrolü HibeSulama ya verilebilir mi?
            HibeMakineSulamaGrup g = new HibeMakineSulamaGrup();
            g.kod = bilgi.kod;
            HibeMakineSulamaGrup[] gler = TRMHibeMakineSulamaGrup.Listele(null, kullanan, g);
            if (gler == null || gler.Length == 0 || gler.Length > 1)
            {
                sonuc.hataStr = hm.grupKod + " numaralı grup bulunamadı.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }
            g = (HibeMakineSulamaGrup)gler[0];
            if (g.ilKod != hm.ilKod || g.durum > 1)
            {
                sonuc.hataStr = bilgi.kod + " numaralı grubun ili, başvurunun ilinden farklı veya durumu uygun değil.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }
            if (OrtakFonksiyonlar.ConvertToInt(g.kod.Substring(0, 2), 0) != hm.etapNo)
            {
                sonuc.hataStr = bilgi.kod + " numaralı grup, başvurunun etap numarasından farklı.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            hm.grupKod = bilgi.kod;

            sonuc = to.KaydetGrup(hm);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                sonuc.anahtar = hm.projeNo.ToString();

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
        public Sonuc GrupCikart(Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeSulama kriter = new HibeSulama();
            kriter.kod = bilgi.hibeKod;
            ObjectArray hibeListe = TRMHibeSulama.Listele(kullanan, kriter, 0, 0, "", true);
            foreach (HibeSulama h in hibeListe.objeler)
            {
                if (h.durum >= (int)ENUMHibeSulamaBasvuruDurum.ODEMEBASLAT)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "(" + h.basvuruNo + ") Başvurunun durumundan dolayı gruptan çıkartılamaz";
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();
            sonuc = to.KaydetGrupCikart(bilgi.hibeKod);

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
        public static Sonuc GrupOdemeyeGonder(VeriErisim veriErisim, Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            return GrupVeBasvuruDurumDegistir(veriErisim, kullanan, bilgi.kod, ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER);
        }
        public static Sonuc GrupOdemeBaslat(VeriErisim veriErisim, Kullanici kullanan, HibeMakineSulamaGrup[] bilgi, int odemeKod)
        {
            return GrupVeBasvuruDurumDegistir(veriErisim, kullanan, bilgi, ENUMHibeSulamaBasvuruDurum.ODEMEBASLAT, odemeKod);
        }
        public static Sonuc GrupOdendi(VeriErisim veriErisim, Kullanici kullanan, int odemeKod, int tur)
        {
            Sonuc sonuc = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            int sorguDurum = 0;
            if (tur == (int)ENUMHibeOdemeTuru.BANKAGONDER)
                sorguDurum = (int)ENUMHibeMakineBasvuruDurum.ODEMEBASLAT;
            else
                sorguDurum = (int)ENUMHibeMakineBasvuruDurum.BANKAYAGONDERILDI;

            HibeMakineSulamaGrup gg = new HibeMakineSulamaGrup() { odemeKod = odemeKod, durum = sorguDurum };
            HibeMakineSulamaGrup[] gler = TRMHibeMakineSulamaGrup.Listele(ve, kullanan, gg);
            if (gler == null || gler.Length == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "İlgili grup bulunamadı.";
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();
            TABHibeSulama th = new TABHibeSulama(ve);

            foreach (HibeMakineSulamaGrup g in gler)
            {
                if (tur == (int)ENUMHibeOdemeTuru.BANKAGONDER)
                    g.durum = (int)ENUMHibeSulamaBasvuruDurum.BANKAYAGONDERILDI;
                else
                    g.durum = (int)ENUMHibeSulamaBasvuruDurum.ODENDI;

                g.odemeKod = odemeKod;

                sonuc = TRMHibeMakineSulamaGrup.Kaydet(ve, kullanan, g);
                if (sonuc.islemSonuc)
                {
                    sonuc = th.GuncelleDurumGruptakiler(g.kod, g.durum);

                    HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                    tarihce.kullaniciKod = kullanan.kullaniciKodu;
                    tarihce.tarih = new TNSDateTime(DateTime.Now);
                    tarihce.durum = g.durum;

                    sonuc = th.KaydetTarihceGruptakiler(g.kod, tarihce);
                }

                if (!sonuc.islemSonuc)
                    break;
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

        public static Sonuc GrupGeriAl(VeriErisim veriErisim, Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            Sonuc sonuc = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            int sorguDurum = (int)ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER;
            HibeMakineSulamaGrup gg = new HibeMakineSulamaGrup() { kod = bilgi.kod, durum = sorguDurum };
            HibeMakineSulamaGrup[] gler = TRMHibeMakineSulamaGrup.Listele(ve, kullanan, gg);
            if (gler == null || gler.Length == 0 || gler.Length > 1)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = bilgi.kod + " nolu grup geri alınmaya uygun değil.";
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();
            TABHibeSulama th = new TABHibeSulama(ve);

            HibeMakineSulamaGrup g = new HibeMakineSulamaGrup();
            g.durum = (int)ENUMHibeSulamaBasvuruDurum.YENI;
            g.ilKod = ((HibeMakineSulamaGrup)gler[0]).ilKod;
            g.odemeKod = ((HibeMakineSulamaGrup)gler[0]).odemeKod;
            g.kod = ((HibeMakineSulamaGrup)gler[0]).kod;
            g.tipi = bilgi.tipi;
            if (g.kod != bilgi.kod)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = bilgi.kod + " nolu grup geri alınmaya uygun değil.";
                ve.VeriErisimKapat();
                return sonuc;
            }

            sonuc = TRMHibeMakineSulamaGrup.Kaydet(ve, kullanan, g);
            if (sonuc.islemSonuc)
            {
                sonuc = th.GuncelleDurumGruptakiler(bilgi.kod, (int)ENUMHibeSulamaBasvuruDurum.YERINDETESPIT);

                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.ODEMEGERIGELDI;

                sonuc = th.KaydetTarihceGruptakiler(bilgi.kod, tarihce);
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

        private static Sonuc GrupVeBasvuruDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, HibeMakineSulamaGrup[] bilgi, ENUMHibeSulamaBasvuruDurum durum, int odemeKod)
        {
            Sonuc sonuc = new Sonuc();
            if (bilgi == null || bilgi.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Hiçbir grup seçilmemiş.";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            ve.BeginTransaction();

            foreach (HibeMakineSulamaGrup grup in bilgi)
            {
                if (string.IsNullOrEmpty(grup.kod.Trim())) continue;
                sonuc = GrupVeBasvuruDurumDegistir(ve, kullanan, grup.kod, durum, odemeKod);
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
        private static Sonuc GrupVeBasvuruDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, string grupKod, ENUMHibeSulamaBasvuruDurum durum, int odemeKod = 0)
        {
            Sonuc sonuc = new Sonuc();
            if (string.IsNullOrEmpty(grupKod))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Grup bilgisi boş bırakılamaz.";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            HibeMakineSulamaGrup g = new HibeMakineSulamaGrup() { kod = grupKod };
            HibeMakineSulamaGrup[] gler = TRMHibeMakineSulamaGrup.Listele(ve, kullanan, g);
            if (gler == null || gler.Length == 0 || gler.Length > 1)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = g.kod + " numaralı grup bulunamadı.";
                ve.VeriErisimKapat();
                return sonuc;
            }
            g = (HibeMakineSulamaGrup)gler[0];
            if ((durum == ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER && g.durum != (int)ENUMHibeSulamaBasvuruDurum.YENI) ||
                (durum == ENUMHibeSulamaBasvuruDurum.ODEMEBASLAT && g.durum != (int)ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER) ||
                (durum == ENUMHibeSulamaBasvuruDurum.ODENDI && g.durum != (int)ENUMHibeSulamaBasvuruDurum.ODEMEBASLAT))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = g.kod + " numaralı grubun durumu uygun değil.";
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeSulama th = new TABHibeSulama(ve);
            if ((durum == ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER && th.GruptaFarkliDurumVarMi(g.kod, (int)ENUMHibeSulamaBasvuruDurum.YERINDETESPIT)) ||
                (durum == ENUMHibeSulamaBasvuruDurum.ODEMEBASLAT && th.GruptaFarkliDurumVarMi(g.kod, (int)ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER)) ||
                (durum == ENUMHibeSulamaBasvuruDurum.ODENDI && th.GruptaFarkliDurumVarMi(g.kod, (int)ENUMHibeSulamaBasvuruDurum.ODEMEBASLAT)))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = g.kod + " numaralı grupta bu işleme uygun olmayan başvurular var. Lütfen, ilgili başvuruları gruptan çıkarıp ya da uygun duruma getirip tekrar deneyin.";
                ve.VeriErisimKapat();
                return sonuc;
            }

            //Grupta kayıt varmı? Hiç kaydı olmayan gruplar ödemeye gönderilemesin
            bool doluMu = th.GruptaKayitVarMi(g.kod);
            if (!doluMu)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = g.kod + " numaralı gruba dahil hiç başvuru kaydı yok. Gruba, başvuru kaydı ekledikten sonra ödemeye gönderin.";
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            g.durum = (int)durum;
            if (odemeKod > 0)
                g.odemeKod = odemeKod;

            sonuc = TRMHibeMakineSulamaGrup.Kaydet(ve, kullanan, g);
            if (sonuc.islemSonuc)
            {
                sonuc = th.GuncelleDurumGruptakiler(g.kod, g.durum);

                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)durum;

                sonuc = th.KaydetTarihceGruptakiler(g.kod, tarihce);
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
        public static HibeMakineSulamaGrup[] HibeSulamaListeleGrupBazliToplam(VeriErisim ve, Kullanici kullanan, int etapNo, int durum, int tur = -1)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulama tHibe = new TABHibeSulama(ve);
            IDataReader r = tHibe.HibeSulamaListeleGrupBazliToplam(durum, tur, Yetki.YetkiliOlduguIlleriGetir(kullanan));
            if (!string.IsNullOrEmpty(tHibe.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tHibe.Hata());
            }

            IList<HibeMakineSulamaGrup> gruplar = new List<HibeMakineSulamaGrup>();
            while (r.Read())
            {
                int sonKolon = (int)TABHibeMakineSulamaGrup.ENUMAlanlar.TIPI;

                HibeMakineSulamaGrup grup = new HibeMakineSulamaGrup();
                grup.kod = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.KOD, string.Empty);
                grup.ilKod = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.ILKOD, string.Empty);
                grup.durum = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM, 0);
                grup.tipi = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.TIPI, 0);
                grup.odemeKod = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.ODEMEKOD, 0);
                grup.ilAd = VeriErisim.DegerAl(r, sonKolon + 1, string.Empty);
                grup.kontrolTarih = VeriErisim.DegerAl(r, sonKolon + 2);
                grup.toplamTutar = VeriErisim.DegerAl(r, sonKolon + 3, (decimal)0);
                grup.refaransFarki = VeriErisim.DegerAl(r, sonKolon + 4, (decimal)0);
                grup.hibeEsasTutar = VeriErisim.DegerAl(r, sonKolon + 5, (decimal)0);

                if (grup.kod.Length == 11)//Eksik ödeme yapanlara başvuru numarası yazılıyor bu nedenle hibe tutarı değil eksikodenenodenecek alanındaki değer gelmeli
                    grup.eksikHibeTutar = VeriErisim.DegerAl(r, sonKolon + 7, (decimal)0);
                else
                    grup.hibeTutar = VeriErisim.DegerAl(r, sonKolon + 6, (decimal)0);

                grup.ayniKatki = VeriErisim.DegerAl(r, sonKolon + 8, (decimal)0);
                grup.sulamaHibeTutar = VeriErisim.DegerAl(r, sonKolon + 9, (decimal)0);

                if (grup.hibeTutar > 0 || grup.eksikHibeTutar > 0)
                    gruplar.Add(grup);
            }
            r.Close();

            ve.VeriErisimKapat();
            HibeMakineSulamaGrup[] donusDeger = new HibeMakineSulamaGrup[gruplar.Count];
            gruplar.CopyTo(donusDeger, 0);
            return donusDeger;
        }
        ////Grup İşlemleri**************************

        /// <summary>
        /// Arguman olarak verilen, hibe Sulama fatura bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetFatura(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeSulama hm = new HibeSulama();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "Fatura");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            hm.bankaSubeKod = bilgi.bankaSubeKod;
            hm.faturaNo = bilgi.faturaNo;
            hm.faturaTarih = bilgi.faturaTarih;
            hm.faturaAciklama = bilgi.faturaAciklama;
            hm.durum = (int)ENUMHibeSulamaBasvuruDurum.FATURAGELDI;
            //hm.detay = bilgi.detay;

            sonuc = to.KaydetFatura(hm);

            if (sonuc.islemSonuc && hm.detay != null && hm.detay.Length > 0)
            {
                foreach (var detay in hm.detay)
                {
                    HibeTutarHesapla(hm.hukukiStatu, ref detay.hibeEsasTutarProje, ref detay.hibeEsasTutarUygulama, detay.proformaTutar, detay.malzemeTutar, ref detay.hibeEsasTutar, ref detay.toplamTutar, ref detay.ayniKatki, ref detay.hibeTutar, detay.hibeOran, detay.basvuruAyniKatkiTutar);
                    sonuc = to.KaydetFaturaDetay(detay, hm.kod);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.FATURAGELDI;
                tarihce.hibeKod = hm.kod;

                sonuc = to.KaydetTarihce(tarihce);
            }

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
        /// Arguman olarak verilen, hibe Sulama sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetYerindeTespit(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeSulama hm = new HibeSulama();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "YerindeTespit");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            hm.yerindeTespitTarih = bilgi.yerindeTespitTarih;
            hm.durum = (int)ENUMHibeSulamaBasvuruDurum.YERINDETESPIT;

            sonuc = to.KaydetYerindeTespit(hm);

            if (sonuc.islemSonuc)
            {
                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.YERINDETESPIT;
                tarihce.hibeKod = hm.kod;

                sonuc = to.KaydetTarihce(tarihce);
            }

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
        public static ObjectArray Sorgula(Kullanici kullanan, HibeSulama kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc)
        {
            return Listele(kullanan, kriter, ilkKayit, kayitSay, sortKolon, asc);
        }
        public ObjectArray Listele(Kullanici kullanan, HibeSulama kriter)
        {
            return Listele(kullanan, kriter, 0, 0, string.Empty, true);
        }

        private static string[,] orderByKolon = { { "kod", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.KOD] },
                                                { "basvuruNo", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.BASVURUNO] },
                                                { "kimlikNo", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.KIMLIKNO] },
                                                { "ad", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.AD] + ", " + TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.SOYAD] },
                                                { "yil", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.YIL] },
                                                { "etapNo", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.ETAPNO] },
                                                { "grupKod", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.GRUPKOD] },
                                                { "il", "ORGCOGRAFYA.AD" },
                                                { "ilce", "ORGCOGRAFYA_1.AD" },
                                                { "koy", TABKoy.TABLOAD + "." + TABKoy.ALANLAR[(int)TABKoy.ENUMAlanlar.ADI] },
                                                { "evrakTarih", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.EVRAKTARIH] },
                                                { "sozlesmeTarih", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.SOZLESMETARIH] },
                                                { "durum", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.DURUM] },
                                                { "evrakNo", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.EVRAKNO] },
                                                { "sulamaGrubuAd", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.BASVURUNO] },
                                                { "proformaTutar", TABHibeSulama.TABLOADDETAY + "." + TABHibeSulama.ALANLARDETAY[(int)TABHibeSulama.ENUMAlanlarDetay.PROFORMATUTAR] },
                                                { "faturaTutar", TABHibeSulama.TABLOADDETAY + "." + TABHibeSulama.ALANLARDETAY[(int)TABHibeSulama.ENUMAlanlarDetay.MALZEMETUTAR] },
                                                { "hibeTutar", TABHibeSulama.TABLOADDETAY + "." + TABHibeSulama.ALANLARDETAY[(int)TABHibeSulama.ENUMAlanlarDetay.HIBETUTAR] },
                                                { "hibeEsasTutar", TABHibeSulama.TABLOADDETAY + "." + TABHibeSulama.ALANLARDETAY[(int)TABHibeSulama.ENUMAlanlarDetay.HIBEESASTUTAR] },
                                                { "ayniKatki", TABHibeSulama.TABLOADDETAY + "." + TABHibeSulama.ALANLARDETAY[(int)TABHibeSulama.ENUMAlanlarDetay.AYNIKATKI] },
                                                { "toplamTutar", TABHibeSulama.TABLOADDETAY + "." + TABHibeSulama.ALANLARDETAY[(int)TABHibeSulama.ENUMAlanlarDetay.TOPLAMTUTAR] },
                                                { "referansTutar", TABHibeSulama.TABLOADDETAY + "." + TABHibeSulama.ALANLARDETAY[(int)TABHibeSulama.ENUMAlanlarDetay.HIBEESASTUTARPROJE] },
                                                { "projeNo", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.PROJENO] },
                                                { "firmaAd", TABHibeSulama.TABLOADDETAY + "." + TABHibeSulama.ALANLARDETAY[(int)TABHibeSulama.ENUMAlanlarDetay.FIRMAAD] },
                                                { "malzemeSeriNo", TABHibeSulama.TABLOADDETAY + "." + TABHibeSulama.ALANLARDETAY[(int)TABHibeSulama.ENUMAlanlarDetay.MALZEMESERINO] },
                                                { "puan", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.PUAN] } ,
                                                { "asilYedek", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.ASILYEDEK] } ,
                                                { "kabulEdildi", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.KABULEDILDI] } ,
                                                { "varYokPuan", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.VARYOK] },
                                                { "alan", TABHibeSulama.TABLOAD + "." + TABHibeSulama.ALANLAR[(int)TABHibeSulama.ENUMAlanlar.SULANACAKALAN] }};

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
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe sulama bilgisini veritabanından okur 
        /// ve Hibesulama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hibe sulama listesini Hibesulama nesne koleksiyonu olarak tutan nesne</returns>
        private static ObjectArray Listele(Kullanici kullanan, HibeSulama kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            if (!Yetki.YetkiVarMiOkumaya(kullanan, kriter.ilKod, ENUMHibeTur.SULAMA))
            {
                sonuc.hataStr = "Okuma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                bilgi.sonuc = sonuc;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeSulama to = new TABHibeSulama(ve);

            VeriErisim ve2 = null;
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
            else
                ve2 = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));

            IDataReader r = to.Listele(kriter, OrderByKolonBul(sortKolon), asc);

            if (to.Hata() == "")
            {
                int sonKolon = ((int)TABHibeSulama.ENUMAlanlar.CINSIYET) + 1;
                //int sonKolon2 = sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.PROFORMAREFERANSTUTAR;
                int sonKolon2 = sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.BASVURUTALEPEDILENHIBETUTAR;

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

                    HibeSulama f = new HibeSulama();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.KOD, 0);
                    f.yil = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.YIL, 0);
                    f.basvuruNo = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.BASVURUNO, string.Empty);
                    f.ilKod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.ILKOD, string.Empty);
                    f.ilceKod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.ILCEKOD, string.Empty);
                    f.koyKod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.KOYKOD, 0);
                    f.kasaba = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.KASABA, string.Empty);
                    f.hukukiStatu = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.HUKUKISTATU, 0);
                    f.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.KIMLIKNO, string.Empty);
                    f.vergiDaire = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.VERGIDAIRE, string.Empty);
                    f.ad = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.AD, string.Empty);
                    f.evrakNo = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.EVRAKNO, string.Empty);
                    f.evrakTarih = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.EVRAKTARIH);
                    f.adres = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.ADRES, string.Empty);
                    f.telefon = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.TELEFON, string.Empty);
                    f.faks = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.FAKS, string.Empty);
                    f.ePosta = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.EPOSTA, string.Empty);
                    f.bankaSubeKod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.BANKASUBEKOD, string.Empty);
                    f.durum = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.DURUM, 0);
                    f.sozlesmeTarih = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.SOZLESMETARIH);
                    f.faturaNo = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.FATURANO, string.Empty);
                    f.faturaTarih = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.FATURATARIH);
                    f.soyad = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.SOYAD, string.Empty);
                    f.babaAd = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.BABAAD, string.Empty);
                    f.dogumTarih = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.DOGUMTARIH);
                    f.yerindeTespitTarih = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.YERINDETESPITTARIH);
                    f.projeNo = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.PROJENO, string.Empty);
                    f.grupKod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.GRUPKOD, string.Empty);
                    f.etapNo = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.ETAPNO, 0);
                    f.sulamaModel = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.SULAMAMODEL, 0);
                    f.sulanacakAlan = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.SULANACAKALAN, (double)0);
                    f.puan = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.PUAN, (double)0);
                    f.redGerekce = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.REDGEREKCE, string.Empty);
                    f.asilYedek = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.ASILYEDEK, string.Empty);
                    f.parselMevkisi = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.PARSELMEVKISI, string.Empty);
                    f.kabulEdildi = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.KABULEDILDI, string.Empty);
                    f.varYokPuan = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.VARYOK, 0);
                    f.redTip = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.REDTIP, 0);
                    f.faturaAciklama = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.FATURAACIKLAMA, string.Empty);
                    f.cinsiyet = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.CINSIYET, string.Empty);

                    HibeSulamaDetay[] detay = new HibeSulamaDetay[1];
                    detay[0] = new HibeSulamaDetay();
                    detay[0].hibeKod = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.HIBEKOD, 0);
                    detay[0].satirNo = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.SATIRNO, 0);
                    detay[0].sulamaGrubuKod = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.SULAMAGRUBUKOD, 0);

                    detay[0].basvuruToplamMalAlimTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.BASVURUTOPLAMMALALIMTUTAR, (decimal)0);
                    detay[0].basvuruHibeyeEsasMalAlimTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.BASVURUHIBEYEESASMALALIMTUTAR, (decimal)0);
                    detay[0].basvuruAyniKatkiTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.BASVURUAYNIKATKITUTAR, (decimal)0);
                    detay[0].basvuruTalepEdilenHibeTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.BASVURUTALEPEDILENHIBETUTAR, (decimal)0);

                    detay[0].malzemeSeriNo = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.MALZEMESERINO, "");
                    detay[0].firmaVergiNo = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.FIRMAVERGINO, "");
                    detay[0].firmaAd = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.FIRMAAD, "");
                    detay[0].malzemeTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.MALZEMETUTAR, (decimal)0);
                    detay[0].aciklama = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.ACIKLAMA, "");
                    detay[0].hibeTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.HIBETUTAR, (decimal)0);
                    detay[0].hibeEsasTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.HIBEESASTUTAR, (decimal)0);
                    detay[0].ayniKatki = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.AYNIKATKI, (decimal)0);
                    detay[0].ayniKatkiProje = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.AYNIKATKIPROJE, (decimal)0);
                    detay[0].toplamTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.TOPLAMTUTAR, (decimal)0);
                    detay[0].hibeEsasTutarProje = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.HIBEESASTUTARPROJE, (decimal)0);
                    detay[0].hibeEsasTutarUygulama = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.HIBEESASTUTARUYGULAMA, (decimal)0);

                    //detay[0].proformaTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.PROFORMATUTAR, (decimal)0);
                    //detay[0].proformaReferansTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.PROFORMAREFERANSTUTAR, (decimal)0);
                    detay[0].proformaTutar = detay[0].toplamTutar;
                    detay[0].proformaReferansTutar = detay[0].hibeEsasTutarProje;
                    if (detay[0].toplamTutar > 1000000)
                        detay[0].proformaReferansTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.PROFORMAREFERANSTUTAR, (decimal)0);

                    detay[0].gercekFatura = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.GERCEKFATURA, (decimal)0);

                    detay[0].gercekHibeTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.GERCEKHIBETUTARI, (decimal)0);
                    detay[0].eksikHibeTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.EKSIKHIBETUTARI, (decimal)0);
                    detay[0].eksikOdemeOdenecek = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.EKSIKODEMEODENECEK, 0);
                    detay[0].eksikOdemeDuzeltildi = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.EKSIKODEMEDUZELTILDI, 0);
                    detay[0].eksikFazlaOdemeAciklama = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeSulama.ENUMAlanlarDetay.EKSIKODEMEACIKLAMA, "");

                    detay[0].sulamaGrubuAd = VeriErisim.DegerAl(r, sonKolon2 + 1, "").Trim();
                    string[] slg = detay[0].sulamaGrubuAd.Split(',');
                    if (slg.Length == 2) detay[0].sulamaGrubuAd = detay[0].sulamaGrubuAd.Replace(",", "").Trim();
                    detay[0].parselListe = VeriErisim.DegerAl(r, sonKolon2 + 5, "").Trim();

                    //detay[0].hibeOran = VeriErisim.DegerAl(r, sonKolon2 + 2, decimal.Zero);

                    f.detay = detay;

                    f.koyAd = VeriErisim.DegerAl(r, sonKolon2 + 2, "");
                    f.ilAd = VeriErisim.DegerAl(r, sonKolon2 + 3, "");
                    f.ilceAd = VeriErisim.DegerAl(r, sonKolon2 + 4, "");

                    if (kriter.maskele)
                        f.kimlikNo = Arac.MaskeleTC(f.kimlikNo);

                    if (kriter.kod > 0 || kriter.sorguKod != "")
                    {
                        //Eğer tek bir kaydı görmek istiyor ise (Başvuru ekranı gibi)
                        if (TNS.TRM.Yetki.YetkiVarMiOkumaya(kullanan, f.ilKod, ENUMHibeTur.SULAMA))
                        {
                            f.diger = TRMHibeSulamaDiger.Listele(ve2, kullanan, f.kod);
                            bilgi.objeler.Add(f);
                        }
                    }
                    else
                        bilgi.objeler.Add(f);
                }
                r.Close();
            }


            ve.VeriErisimKapat();
            if (ve2 != null)
                ve2.VeriErisimKapat();
            return bilgi;
        }
        public static int HibeSulamaBasvuruSayiDondur(VeriErisim veriErisim, Kullanici kullanan, HibeSulama kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeSulama th = new TABHibeSulama(ve);
            int basvuruSayi = th.HibeSulamaBasvuruSayiDondur(kriter);
            ve.VeriErisimKapat();
            return basvuruSayi;
        }
        public static double HibeSulamaOdemeTutarGetir(Kullanici kullanan, int tur)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            double odemeMiktar = to.HibeSulamaOdemeMiktarGetir(tur);
            ve.VeriErisimKapat();
            return odemeMiktar;
        }
        private string ProjeNoAl(VeriErisim ve, int etapNo, string ilKod, int sulamaGrupKod)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKriter = etapNo.ToString("00") + "." + ilKod.PadLeft(2, '0') + "." + sulamaGrupKod.ToString().PadLeft(4, '0');
            string sayacKod = "HIBESULAMAPROJENO" + sayacKriter.Replace(".", "");

            return sayacKriter + "." + tg.BelgeNoAl(sayacKod, "00000");
        }
        private string BasvuruNoAl(VeriErisim ve, int etapNo, string ilKod)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKriter = etapNo.ToString("00") + "." + ilKod.PadLeft(2, '0');
            string sayacKod = "HIBESULAMABASVURUNO" + sayacKriter.Replace(".", "");

            return sayacKriter + "." + tg.BelgeNoAl(sayacKod, "00000");
        }
        private int BelgeNoAlInt(VeriErisim ve)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKod = "HIBESULAMAKOD";

            return tg.BelgeNoAlInt(sayacKod, "000000");
        }

        private Sonuc KontrolEvrakKaydi(VeriErisim ve, Kullanici kullanan, int kod, string kimlikNo, string evrakNo, TNSDateTime evrakTarih, int aktifEtapNo, string ilKod)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (string.IsNullOrWhiteSpace(evrakNo)) return sonuc;


            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);

            HibeSulama kriter = new HibeSulama();
            kriter.etapNo = aktifEtapNo;
            kriter.kimlikNo = kimlikNo;
            kriter.kod = kod;
            kriter.evrakTarih = evrakTarih;
            kriter.evrakNo = evrakNo.Trim();
            kriter.ilKod = ilKod;

            IDataReader r = to.KontrolEvrakKaydi(kriter);

            HibeSulama hm = new HibeSulama();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    hm.kod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.KOD, 0);
                    hm.basvuruNo = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.BASVURUNO, "");
                    hm.evrakNo = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.EVRAKNO, string.Empty);
                    break;
                }
            }
            r.Close();
            ve.VeriErisimKapat();

            if (hm.kod > 0)
            {
                if (hm.kod != kod)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = evrakNo + " Evrak Numarası ile ilinizde sisteme ait kayıtlı başvuru bulunmaktadır.(Başvuru No:" + hm.basvuruNo + ")<br>";
                    return sonuc;
                }
            }

            return sonuc;
        }

        private Sonuc KontrolKimlikNoKayitVarMi(VeriErisim ve, Kullanici kullanan, int kod, string kimlikNo, int aktifEtapNo, int hukukiStatu, int sulamaGrubuKod, string ilKod)
        {
            ////   TC ve Vergi Numarası değişikliği yapılamaz  TRUE,  değiştirme imkanı verilir FALSE
            bool degistirilemezKontrolu = true;

            Sonuc sonuc = new Sonuc(true, "", "");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);

            HibeSulama kriter = new HibeSulama();
            kriter.etapNo = aktifEtapNo;
            kriter.kimlikNo = kimlikNo;
            kriter.kod = kod;
            kriter.hukukiStatu = hukukiStatu;
            kriter.ilKod = ilKod;
            if (to.FarkliHukukiStatudeKaydiVarMi(kriter))
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = kimlikNo + " Kimlik Numarası, başka bir başvuruda farklı bir Hukuki Statü ile kullanılmıştır.<br>";
                return sonuc;
            }

            kriter = new HibeSulama();
            if (kod > 0 && hukukiStatu < (int)ENUMHibeSulamaHukukiStatu.KOOPERATIF)
                kriter.kod = kod;
            else
                kriter.kimlikNo = kimlikNo;

            kriter.etapNo = aktifEtapNo;
            kriter.ilKod = ilKod;//il bazında yatırımcı başvuru yapabilir. 13.02.2018 Sulama Muhammet Bey

            IDataReader r = to.KontrolKimlikNoKayitVarMi(kriter, hukukiStatu, sulamaGrubuKod);

            HibeSulama hm = new HibeSulama();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    hm.kod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.KOD, 0);
                    hm.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.KIMLIKNO, string.Empty);
                    break;
                }
            }
            r.Close();
            ve.VeriErisimKapat();

            if (hm.kod > 0)
            {
                if (hm.kod != kod)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = kimlikNo + " Kimlik Numarası ile sisteme ilinize ait kayıtlı başvuru bulunmaktadır.<br>";
                    return sonuc;
                }
                else if ((hm.kimlikNo != kimlikNo) && degistirilemezKontrolu)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Bu belge " + hm.kimlikNo + " Kimlik Numarası ile sisteme kayıtlıdır. Belgeye ait kimlik no değiştirilemez.<br>";
                    return sonuc;
                }
            }

            return sonuc;
        }

        private Sonuc KontrolKimlikNoKayitVarMi(VeriErisim ve, Kullanici kullanan, int kod, string kimlikNo, int aktifEtapNo, bool degistirilemezKontrolu = true)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);

            HibeSulama kriter = new HibeSulama();
            if (kod > 0 && degistirilemezKontrolu)
                kriter.kod = kod;
            else
                kriter.kimlikNo = kimlikNo;

            kriter.etapNo = aktifEtapNo;

            IDataReader r = to.KontrolKimlikNoKayitVarMi(kriter, 0, 0);

            HibeSulama hm = new HibeSulama();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    hm.kod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.KOD, 0);
                    hm.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlar.KIMLIKNO, string.Empty);
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


        public Sonuc YatirimciDegistir(Kullanici kullanan, HibeSulama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
            ve.BeginTransaction();

            HibeSulama hm = new HibeSulama();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "YatirimciDegistir");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
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

            sonuc = to.YatirimciDegistir(bilgi);
            if (sonuc.islemSonuc)
            {
                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeSulamaBasvuruDurum.TANIMSIZ;
                tarihce.hibeKod = hm.kod;
                tarihce.aciklama = bilgi.redGerekce + " Yeni Kimlik Numarası:" + bilgi.kimlikNo + "; Eski Kimlik Numarası:" + bilgi.ePosta;//Eski kimlik numarası tutuluyor

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

        public ObjectArray ListeleTarihce(Kullanici kullanan, HibeSulama kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            Sonuc sonuc = new Sonuc();

            if (!Yetki.YetkiVarMiOkumaya(kullanan, kriter.ilKod, ENUMHibeTur.SULAMA))
            {
                sonuc.hataStr = "Okuma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                bilgi.sonuc = sonuc;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeSulama to = new TABHibeSulama(ve);
            IDataReader r = to.ListeleTarihce(new HibeSulamaTarihce { hibeKod = kriter.kod });

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeSulamaTarihce f = new HibeSulamaTarihce();

                    f.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlarTarihce.HIBEKOD, 0);
                    f.kullaniciKod = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlarTarihce.KULLANICIKOD, string.Empty);
                    f.tarih = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlarTarihce.TARIH);
                    f.durum = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlarTarihce.DURUM, 0);
                    f.aciklama = VeriErisim.DegerAl(r, (int)TABHibeSulama.ENUMAlanlarTarihce.ACIKLAMA, string.Empty);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
        public string IslemKontrol(Kullanici kullanan, HibeSulama bilgi, ref HibeSulama hm, string tur)
        {
            if (bilgi.kod == 0)
                return "İşlem yapılacak kayıta ait kod bilgisi alınamadı";

            ObjectArray hmler = Listele(kullanan, new HibeSulama { kod = bilgi.kod });
            if (hmler == null || hmler.objeler.Count == 0 || hmler.objeler.Count > 1)
                return "İşlem yapılacak kayıta ait bilgi okunamadı";

            hm = (HibeSulama)hmler.objeler[0];
            //if (!Yetki.YetkiVarMiYazmaya(kullanan, hm.ilKod, hm.etapNo, string.Empty, ENUMHibeTur.SULAMA))
            Sonuc sonuc1 = new Sonuc();
            if (!Yetki.YetkiVarMiYazmaya(kullanan, hm.ilKod, -1, sonuc1, string.Empty, ENUMHibeTur.SULAMA, hm.kod, true))
                return sonuc1.hataStr;

            if (hm.hukukiStatu == (int)ENUMHibeSulamaHukukiStatu.GERCEKKISI || hm.hukukiStatu == (int)ENUMHibeSulamaHukukiStatu.GENCCIFTCI)
                if (string.IsNullOrEmpty(hm.babaAd))
                    return "TC Kimlik Numarası " + hm.kimlikNo + " olan kişinin kimlik bilgileri bulunmamaktadır. Başvuru giriş ekranından eksik bilgileri girin.<br>";

            //Cezalı kontrolü
            //**************************
            Sonuc cezaliMi = new TRMHibeCeza().CezaliMi(hm.kimlikNo);
            if (cezaliMi.islemSonuc)
                return cezaliMi.hataStr;
            //**************************

            string hata = "";
            if (tur == "Sozlesme")
            {
                if (hm.durum < (int)ENUMHibeSulamaBasvuruDurum.PROJENUMARASIVERILDI || string.IsNullOrEmpty(hm.projeNo))
                    hata += "Sözleşme yapabilmek için Proje Numarası verilmiş olması gerekmektedir.<br>";
                if (hm.durum > (int)ENUMHibeSulamaBasvuruDurum.SOZLESMEYAPILDI)
                    hata += "Sözleşme kaydı aşamasını geçmiş bulunmaktasınız.<br>";
                if (bilgi.sozlesmeTarih.isNull)
                    hata += "Sözleşme Tarihi boş bırakılamaz<br>";
                else if (bilgi.sozlesmeTarih.Oku() > DateTime.Now)
                    hata += "Sözleşme Tarihi, bugünden sonraki bir gün olamaz<br>";
                else if (bilgi.sozlesmeTarih.Oku() < hm.evrakTarih.Oku())
                    hata += "Sözleşme Tarihi, başvuru evrak tarihinden (" + hm.evrakTarih.ToString() + ") önceki bir gün olamaz<br>";

                if (hm.asilYedek == "")
                    hata += "(" + hm.basvuruNo + ") nolu başvurunun asil/Yedek işlemi yapılmamış<br>";
                if (hm.kabulEdildi == "")
                    hata += "(" + hm.basvuruNo + ") nolu başvurunun kabul işlemi yapılmamış<br>";

                Sonuc sonuc = MalzemelerBasvuruyaUygunMu(kullanan, hm, ENUMHibeSulamaMalzemeTip.PROJE);
                if (!sonuc.islemSonuc)
                    hata += sonuc.hataStr + "<br>";
            }
            else if (tur == "ProjeNo")
            {
                if (hm.durum > (int)ENUMHibeSulamaBasvuruDurum.PROJENUMARASIVERILDI)
                    hata += "Proje Numarası verme aşamasını geçmiş bulunmaktasınız.<br>";

                if (hm.asilYedek == "")
                    hata += "(" + hm.basvuruNo + ") nolu başvurunun asil/Yedek işlemi yapılmamış<br>";
                if (hm.kabulEdildi == "")
                    hata += "(" + hm.basvuruNo + ") nolu başvurunun kabul işlemi yapılmamış<br>";

                Sonuc sonuc = MalzemelerBasvuruyaUygunMu(kullanan, hm, ENUMHibeSulamaMalzemeTip.PROJE);
                if (!sonuc.islemSonuc)
                    hata += sonuc.hataStr + "<br>";
            }
            else if (tur == "Fatura")
            {
                if (hm.sozlesmeTarih.isNull)//hm.durum < (int)ENUMHibeSulamaBasvuruDurum.SOZLESMEYAPILDI
                    hata += "Fatura girişi yapabilmek için Sözleşme yapılmış olması gerekmektedir.<br>";
                if (hm.durum > (int)ENUMHibeSulamaBasvuruDurum.YERINDETESPIT)
                    hata += "Fatura girişi aşamasını geçmiş bulunmaktasınız.<br>";
                if (bilgi.bankaSubeKod == "")
                    hata += "Şube bilgisi boş bırakılamaz<br>";
                //if (string.IsNullOrEmpty(bilgi.faturaNo))
                //    hata += "Fatura No boş bırakılamaz<br>";
                //if (bilgi.faturaTarih.isNull)
                //    hata += "Fatura Tarihi boş bırakılamaz<br>";
                //else if (bilgi.faturaTarih.Oku() > DateTime.Now)
                //    hata += "Fatura Tarihi bugünden sonraki bir gün olamaz<br>";

                if (hm.asilYedek == "")
                    hata += "(" + hm.basvuruNo + ") nolu başvurunun asil/Yedek işlemi yapılmamış<br>";
                if (hm.kabulEdildi == "")
                    hata += "(" + hm.basvuruNo + ") nolu başvurunun kabul işlemi yapılmamış<br>";

                if (!bilgi.faturaTarih.isNull)
                {
                    int kontrolTarihSozlesmeFatura = 60;

                    kontrolTarihSozlesmeFatura = TNS.OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings["HibeSulamaSozlesmeFaturaTarihKontrol"], 60);

                    if (!hm.sozlesmeTarih.isNull && (bilgi.faturaTarih.Oku() < hm.sozlesmeTarih.Oku()))
                        hata += "Fatura Tarihi, Sözleşme Tarihi'nden (" + hm.sozlesmeTarih.ToString() + ") önceki bir gün olamaz<br>";
                    if (hm.sozlesmeTarih.Oku().AddDays(kontrolTarihSozlesmeFatura) < bilgi.faturaTarih.Oku())
                        hata += "Sözleşme tarihi ile Fatura Tarihi arasında en fazla " + kontrolTarihSozlesmeFatura + " gün olabilir<br>";
                }
            }
            else if (tur == "YerindeTespit")
            {
                if (hm.durum < (int)ENUMHibeSulamaBasvuruDurum.FATURAGELDI || string.IsNullOrEmpty(hm.bankaSubeKod))
                    hata += "Yerinde Tespit yapabilmek için Fatura bilgilerinin girilmiş olması gerekmektedir.<br>";
                if (hm.durum > (int)ENUMHibeSulamaBasvuruDurum.YERINDETESPIT)
                    hata += "Yerinde Tespit aşamasını geçmiş bulunmaktasınız.<br>";
                if (bilgi.yerindeTespitTarih.isNull)
                    hata += "Yerinde Tespit Tarihi boş bırakılamaz<br>";
                else if (bilgi.yerindeTespitTarih.Oku() > DateTime.Now)
                    hata += "Yerinde Tespit Tarihi, bugünden sonraki bir gün olamaz<br>";

                if (hm.asilYedek == "")
                    hata += "(" + hm.basvuruNo + ") nolu başvurunun asil/Yedek işlemi yapılmamış<br>";
                if (hm.kabulEdildi == "")
                    hata += "(" + hm.basvuruNo + ") nolu başvurunun kabul işlemi yapılmamış<br>";

                if (!bilgi.yerindeTespitTarih.isNull)
                {
                    int kontrolTarihSozlesmeYerindetespit = 90;

                    kontrolTarihSozlesmeYerindetespit = TNS.OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings["HibeSulamaSozlesmeYerindeTespitTarihKontrol"], 90);

                    //if (!hm.faturaTarih.isNull && (bilgi.yerindeTespitTarih.Oku() < hm.faturaTarih.Oku()))
                    //    hata += "Yerinde Tespit Tarihi, Fatura Tarihi'nden (" + hm.faturaTarih.ToString() + ") önceki bir gün olamaz<br>";
                    if (hm.sozlesmeTarih.Oku().AddDays(kontrolTarihSozlesmeYerindetespit) < bilgi.yerindeTespitTarih.Oku())
                        hata += "Yerinde Tespit tarihi ile Sözleşme Tarihi arasında en fazla " + kontrolTarihSozlesmeYerindetespit + " gün olabilir<br>";
                }
            }
            else if (tur == "YatirimciDegistir")
            {
                if (hm.durum == (int)ENUMHibeSulamaBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "(" + hm.basvuruNo + ") Reddedilmiş yatırımcı değiştirilemez.";
            }
            else if (tur == "Red")
            {
                if (hm.durum >= (int)ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER &&
                    hm.durum < (int)ENUMHibeSulamaBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "(" + hm.basvuruNo + ") Ödeme işlemi başlamış başvuru reddedilemez.";
            }
            else if (tur == "BasvuruYapildi")
            {
                if (hm.durum >= (int)ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER &&
                    hm.durum < (int)ENUMHibeSulamaBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "(" + hm.basvuruNo + ") Ödeme işlemi başlamış başvuru üzerinde işlem yapılamaz.";
            }
            else if (tur == "Grup")
            {
                if (hm.durum >= (int)ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER &&
                    hm.durum < (int)ENUMHibeSulamaBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "(" + hm.basvuruNo + ") Ödeme işlemi başlamış başvuruya grup numarası verilemez.";
            }
            else if (tur == "AsilYedek")
            {
                if (hm.durum >= (int)ENUMHibeSulamaBasvuruDurum.FATURAGELDI)
                    hata += "(" + hm.basvuruNo + ") Fatura kaydı yapıldığı için asil/yedek ayarlaması yapılamaz.";
            }
            else if (tur == "kabulEdildi")
            {
                if (hm.durum >= (int)ENUMHibeSulamaBasvuruDurum.PROJENUMARASIVERILDI)
                    hata += "(" + hm.basvuruNo + ") Proje Numarası verildiği için Kabul ayarlaması yapılamaz.";

                bool yetkili = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                               kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI) ||
                               kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUMERKEZ) ||
                               kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUUZMAN);


                if (hm.kabulEdildi != "" && !yetkili)
                    hata += "(" + hm.basvuruNo + ") Kabul ayarlaması daha önce yapıldığı için değişiklik yapılamaz.";
            }
            return hata;
        }
        public static HibeSulama[] KimlikBilgisiEksikBasvurular(VeriErisim ve, Kullanici kullanan)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                throw new Exception("Okuma yetkiniz bulunmamaktadır.");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);

            IList<HibeSulama> al = new List<HibeSulama>();
            IDataReader r = to.KimlikBilgisiEksikBasvurular();
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (r.Read())
                {
                    HibeSulama hm = new HibeSulama();
                    hm.kod = VeriErisim.DegerAl(r, 0, 0);
                    hm.kimlikNo = VeriErisim.DegerAl(r, 1, string.Empty);

                    al.Add(hm);
                }
            }
            r.Close();
            ve.VeriErisimKapat();

            HibeSulama[] liste = new HibeSulama[al.Count];
            al.CopyTo(liste, 0);
            return liste;
        }
        public static Sonuc KimlikBilgisiEksikGuncelle(VeriErisim ve, Kullanici kullanan, HibeSulama hm)
        {
            Sonuc sonuc = new Sonuc();
            if (hm.kod <= 0 || string.IsNullOrEmpty(hm.ad) || string.IsNullOrEmpty(hm.soyad))
                return sonuc;

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);
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
        public static void HibeTutarHesapla(int hukukiStatu, ref decimal hibeEsasTutarProje, ref decimal hibeEsasTutarUygulama, decimal proformaTutar, decimal malzemeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, decimal hibeOran, decimal basvuruAyniKatki)
        {
            if (hibeOran == 0) hibeOran = 50;

            decimal ustSinir = OrtakFonksiyonlar.ConvertToDecimal(TRMHibeSulamaYonetim.HibeSulamaAktifUstLimit(null, hukukiStatu));
            hibeEsasTutar = hibeEsasTutarUygulama > 0 && hibeEsasTutarUygulama < hibeEsasTutarProje ? hibeEsasTutarUygulama : hibeEsasTutarProje;
            hibeEsasTutar = hibeEsasTutar > ustSinir ? ustSinir : hibeEsasTutar;

            hibeEsasTutarProje = hibeEsasTutarProje > ustSinir ? ustSinir : hibeEsasTutarProje;
            hibeEsasTutarUygulama = hibeEsasTutarUygulama > ustSinir ? ustSinir : hibeEsasTutarUygulama;

            toplamTutar = malzemeTutar > 0 ? malzemeTutar : proformaTutar;
            ayniKatki = toplamTutar - hibeEsasTutar;
            //hibeTutar = Math.Round((hibeEsasTutar - basvuruAyniKatki) * (hibeOran / 100), 2);
            hibeTutar = OrtakFonksiyonlar.Yuvarla((hibeEsasTutar - basvuruAyniKatki) * (hibeOran / 100), 2);
        }

        public static Sonuc DegerlendirmeKaydet(VeriErisim ve, Kullanici kullanan, int hibeKod, string anahtar, double artisPuan)
        {
            string hata = string.Empty;
            Sonuc sonuc = new Sonuc(false, "", "");

            if (hibeKod <= 0)
                hata = "Başvuru kodu boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(hata))
                return new Sonuc(false, hata, string.Empty);

            //Yetki kontrolü
            ObjectArray hibeler = new TRMHibeSulama().Listele(kullanan, new HibeSulama() { kod = hibeKod });
            if (!hibeler.sonuc.islemSonuc)
                return hibeler.sonuc;
            if (hibeler.objeler.Count == 0)
                return new Sonuc(false, "İlgili hibe başvurusu bulunamadı!", string.Empty);
            //if (!Yetki.YetkiVarMiYazmaya(kullanan, ((HibeSulama)hibeler.objeler[0]).ilKod, ((HibeSulama)hibeler.objeler[0]).etapNo, string.Empty, ENUMHibeTur.SULAMA))
            if (!Yetki.YetkiVarMiYazmaya(kullanan, ((HibeSulama)hibeler.objeler[0]).ilKod, -1, sonuc, string.Empty, ENUMHibeTur.SULAMA, ((HibeSulama)hibeler.objeler[0]).kod, true))
                return sonuc;

            if (((HibeSulama)hibeler.objeler[0]).durum >= (int)ENUMHibeSulamaBasvuruDurum.SOZLESMEYAPILDI)
                return new Sonuc(false, "Bu başvuru ile sözleşme yapıldığı için değerlendirme işlemi yapılamaz.", string.Empty);

            AnketBilgi anket = TRMHibeMakine.DegerlendirmeOku(kullanan, hibeKod, anahtar);
            double anketPuan = TRMHibeMakine.DegerlendirmePuanHesapla(anket);

            //if (anketPuan == 0)
            //    return new Sonuc(false, "Bu başvuru için değerlendirme yapılmamış.", string.Empty);

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);

            ve.BeginTransaction();

            if (anahtar.EndsWith("001"))
                sonuc = to.KaydetVarYokSonuc(hibeKod, anketPuan);
            else
                sonuc = to.KaydetDegerlendirmeSonuc(hibeKod, anketPuan + artisPuan);

            if (sonuc.islemSonuc)
            {
                HibeSulamaTarihce tarihce = new HibeSulamaTarihce();
                tarihce.hibeKod = hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.durum = 100;//Değerlendirme kodu olsun.

                if (anahtar.EndsWith("001"))
                    tarihce.aciklama = "Teslim Alma İşlemi sonucu :" + (anketPuan > 0 ? "Eskik belge var" : "Belgeler tamam");
                else
                    tarihce.aciklama = "Değerlendirme işlemi sonucu :" + anketPuan + " puan";

                if (artisPuan > 0)
                    tarihce.aciklama = artisPuan.ToString() + " Puan artışı yapıldı. Eski Puanı:" + anketPuan + " -  Yeni Puan:" + (anketPuan + artisPuan);

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                if (anahtar.EndsWith("002"))
                    sonuc.anahtar = anketPuan.ToString();
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

        public static ObjectArray ListeleSulamaKabulCalismasiIlToplami(VeriErisim veriErisim, Kullanici kullanan, HibeSulama kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeSulama th = new TABHibeSulama(ve);
            IDataReader r = th.ListeleKabulCalismasiIlToplami(kriter);

            ObjectArray liste = new ObjectArray();
            if (r != null)
            {
                while (r.Read())
                {
                    HibeSulama b = new HibeSulama();
                    b.ilKod = VeriErisim.DegerAl(r, 0, "");

                    HibeSulamaDetay[] detay = new HibeSulamaDetay[1];
                    detay[0] = new HibeSulamaDetay();
                    detay[0].hibeTutar = VeriErisim.DegerAl(r, 1, (decimal)0);

                    b.detay = detay;
                    liste.objeler.Add(b);
                }

                r.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static HazineRapor[] HazineRapor(int yil, int ay, int tur, int etapNo)
        {
            HazineRapor[] liste;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulama to = new TABHibeSulama(ve);

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

        public static ObjectArray KonusunaGoreDagilimRapor(VeriErisim veriErisim, Kullanici kullanan, HibeSulama kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeSulama th = new TABHibeSulama(ve);
            IDataReader r = th.KonusunaGoreDagilimRapor(kriter);

            ObjectArray liste = new ObjectArray();
            if (r != null)
            {
                while (r.Read())
                {
                    HibeSulama b = new HibeSulama();
                    b.ilAd = VeriErisim.DegerAl(r, 0, "");
                    b.ilceAd = VeriErisim.DegerAl(r, 1, "");
                    b.koyAd = VeriErisim.DegerAl(r, 2, "");
                    b.kasaba = VeriErisim.DegerAl(r, 3, "");//Konusu
                    b.sulanacakAlan = VeriErisim.DegerAl(r, 4, 0.0);
                    b.puan = VeriErisim.DegerAl(r, 5, 0.0);//HibeTutar
                    b.etapNo = VeriErisim.DegerAl(r, 6, 0);
                    b.parselMevkisi = VeriErisim.DegerAl(r, 7, "");

                    liste.objeler.Add(b);
                }

                r.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static ObjectArray BilgiNotuRapor(VeriErisim veriErisim, Kullanici kullanan, HibeSulama kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeSulama th = new TABHibeSulama(ve);
            IDataReader r = th.BilgiNotuRapor();

            ObjectArray liste = new ObjectArray();
            if (r != null)
            {
                while (r.Read())
                {
                    HibeSulama b = new HibeSulama();
                    b.etapNo = VeriErisim.DegerAl(r, 0, 0);
                    b.ilKod = VeriErisim.DegerAl(r, 1, "");
                    b.ilAd = VeriErisim.DegerAl(r, 2, "");
                    b.durum = VeriErisim.DegerAl(r, 3, 0);
                    b.hukukiStatu = VeriErisim.DegerAl(r, 4, 0);//adet
                    b.sulanacakAlan = VeriErisim.DegerAl(r, 5, 0.0);
                    b.puan = VeriErisim.DegerAl(r, 6, 0.0);//HibeTutar

                    liste.objeler.Add(b);
                }

                r.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static ObjectArray KonuIstatistik(VeriErisim veriErisim, Kullanici kullanan, HibeSulama kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeSulama th = new TABHibeSulama(ve);
            IDataReader r = th.KonuIstatistik(kriter);

            ObjectArray liste = new ObjectArray();
            if (r != null)
            {
                while (r.Read())
                {
                    HibeSulama b = new HibeSulama();
                    b.etapNo = VeriErisim.DegerAl(r, 0, 0);
                    b.sulamaModel = VeriErisim.DegerAl(r, 1, 0);
                    b.hukukiStatu = VeriErisim.DegerAl(r, 2, 0);//adet
                    b.sulanacakAlan = VeriErisim.DegerAl(r, 3, 0.0);
                    b.puan = VeriErisim.DegerAl(r, 4, 0.0);//HibeTutar

                    liste.objeler.Add(b);
                }

                r.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static ObjectArray Istatistik(VeriErisim veriErisim, Kullanici kullanan, HibeSulama kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeSulama th = new TABHibeSulama(ve);
            IDataReader r = th.Istatistik(kriter);

            ObjectArray liste = new ObjectArray();
            if (r != null)
            {
                while (r.Read())
                {
                    HibeSulama b = new HibeSulama();
                    b.etapNo = VeriErisim.DegerAl(r, 0, 0);
                    b.ilAd = VeriErisim.DegerAl(r, 1, "");
                    b.hukukiStatu = VeriErisim.DegerAl(r, 2, 0);//adet
                    b.sulanacakAlan = VeriErisim.DegerAl(r, 3, 0.0);//alan
                    b.puan = VeriErisim.DegerAl(r, 4, 0.0);//HibeTutar

                    liste.objeler.Add(b);
                }

                r.Close();
            }

            ve.VeriErisimKapat();
            return liste;
        }

        //public Sonuc DetayKonuKaydet(VeriErisim ve, Kullanici kullanan, HibeSulamaDetayKonu bilgi)
        //{
        //    string hata = string.Empty;
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    if (bilgi.hibeKod <= 0)
        //        hata = "Başvuru kodu boş bırakılamaz.<br>";
        //    if (!string.IsNullOrEmpty(hata))
        //        return new Sonuc(false, hata, string.Empty);

        //    ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
        //    TABHibeSulama to = new TABHibeSulama(ve);

        //    ve.BeginTransaction();

        //    sonuc = to.SilDetayKonu(bilgi);

        //    if (sonuc.islemSonuc)
        //    {
        //        bilgi.kod = to.YeniDetayKonuKodVer();

        //        sonuc = to.KaydetDetayKonu(bilgi, bilgi.hibeKod);
        //    }

        //    if (sonuc.islemSonuc)
        //    {
        //        sonuc.anahtar = bilgi.kod.ToString();
        //        sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
        //        ve.Commit();
        //    }
        //    else
        //    {
        //        sonuc.hataStr = "İşlem sırasında hata oluştu.<br>" + sonuc.hataStr;
        //        ve.Rollback();
        //    }

        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}
    }
}
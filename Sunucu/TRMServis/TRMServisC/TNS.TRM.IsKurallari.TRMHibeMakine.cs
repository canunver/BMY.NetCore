using System.Data;
using TNS.TRM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeMakine
    {
        /// <summary>
        /// Arguman olarak verilen, hibe makine bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));

            if (!bilgi.online)
            {
                if (!Yetki.YetkiVarMiYazmaya(kullanan, bilgi.ilKod, -1, sonuc))
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }
            else
                kullanan.kullaniciKodu = "Şifresiz Giriş";

            Sonuc cezaliMi = new TRMHibeCeza().CezaliMi(bilgi.kimlikNo);
            if (cezaliMi.islemSonuc)
            {
                sonuc.hataStr = cezaliMi.hataStr;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            int aktifEtapNo = TRMHibeMakineYonetim.HibeMakineAktifEtapNo(ve);
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
            else
            {
                if (bilgi.etapNo != aktifEtapNo)
                {
                    sonuc.hataStr = "Sadece aktif etap bilgileri kayıt edilebilir.";
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            HibeMakineTarihce tarihce = new HibeMakineTarihce();
            tarihce.kullaniciKod = kullanan.kullaniciKodu;
            tarihce.tarih = new TNSDateTime(DateTime.Now);
            tarihce.durum = bilgi.durum;

            if (!bilgi.online)
            {
                sonuc = KontrolKimlikNoKayitVarMi(ve, kullanan, bilgi.kod, bilgi.kimlikNo, aktifEtapNo);
                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            bool yeni = false;
            if (bilgi.kod == 0)
            {
                yeni = true;
                bilgi.yil = DateTime.Now.Year;
                bilgi.kod = BelgeNoAlInt(ve);
                string plaka = new TABGenel(ve).IlPlakaKodBul(bilgi.ilKod); //TRMServis.UzayYaziDegerGetir("COGRAFYA", bilgi.ilKod, true, "PLAKAKOD");
                bilgi.basvuruNo = BasvuruNoAl(ve, bilgi.etapNo, plaka);
                if (!bilgi.online)
                    bilgi.durum = (int)ENUMHibeMakineBasvuruDurum.YENI;
                else
                    bilgi.durum = (int)ENUMHibeMakineBasvuruDurum.ONLINE;

                tarihce.durum = bilgi.durum;
            }
            else
            {
                bilgi.durum = to.ListeleDurum(bilgi.kod);
                if (bilgi.durum == (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI)
                    bilgi.durum = (int)ENUMHibeMakineBasvuruDurum.YENI;
                else
                    bilgi.durum = (int)ENUMHibeMakineBasvuruDurum.TANIMSIZ;
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.DEGISTIRILDI;
            }

            tarihce.hibeKod = bilgi.kod;

            //Referans fiyatı bul ve set et
            //SQL server insert ve update işlemi yaparken select işlemine izin vermediği için 2. bir connection açmamak için burada yapılıyor
            foreach (var detay in bilgi.detay)
            {
                if (bilgi.hukukiStatu != (int)ENUMHibeMakineHukukiStatu.SUTBIRLIK)
                    detay.adet = 1;
                else if (detay.adet == 0)
                    detay.adet = 1;

                detay.referansTutar = to.ReferansFiyatBul(aktifEtapNo, bilgi.ilKod, detay.malzemeKod);
            }

            if (yeni)
                sonuc = to.Kaydet(bilgi);
            else
                sonuc = to.Guncelle(bilgi);

            if (sonuc.islemSonuc && bilgi.detay.Length > 0)
            {
                if (!yeni) //Eğer güncelleme yapılıyorsa silme işlemi yapılsın. Yeni başvuru kaydında kayıtlı detay bilgisi yoktur
                    sonuc = to.SilDetay(bilgi.kod);

                if (sonuc.islemSonuc)
                {
                    foreach (var detay in bilgi.detay)
                    {
                        HibeTutarHesapla(bilgi.hukukiStatu, detay.referansTutar, detay.proformaTutar, detay.malzemeTutar, ref detay.hibeEsasTutar, ref detay.toplamTutar, ref detay.ayniKatki, ref detay.hibeTutar, detay.hibeOran, detay.adet);
                        sonuc = to.KaydetDetay(detay, bilgi.kod);

                        if (sonuc.islemSonuc)
                            break;
                    }

                    //if (!bilgi.online)
                    sonuc = to.KaydetTarihce(tarihce);
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
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
        /// <summary>
        /// Arguman olarak verilen, hibe makine sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetSozlesme(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeMakine hm = new HibeMakine();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "Sozlesme");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            hm.sozlesmeTarih = bilgi.sozlesmeTarih;
            hm.durum = (int)ENUMHibeMakineBasvuruDurum.SOZLESMEYAPILDI;

            sonuc = to.KaydetSozlesme(hm);
            if (bilgi.detay != null)
                hm.detay = bilgi.detay;//Sözleşmede doldurulan detay bilgi

            if (sonuc.islemSonuc && hm.detay != null && hm.detay.Length > 0)
            {
                foreach (var detay in hm.detay)
                {
                    HibeTutarHesapla(hm.hukukiStatu, detay.referansTutar, detay.proformaTutar, detay.malzemeTutar, ref detay.hibeEsasTutar, ref detay.toplamTutar, ref detay.ayniKatki, ref detay.hibeTutar, detay.hibeOran, detay.adet);
                    detay.hibeKod = hm.kod;
                    sonuc = to.GuncelleDetay(detay);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                HibeMakineTarihce tarihce = new HibeMakineTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.SOZLESMEYAPILDI;
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
        /// Arguman olarak verilen, hibe makine sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BasvuruyuIslemeAl(Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            sonuc = to.BasvuruyuIslemeAl(kod);

            if (sonuc.islemSonuc)
            {
                HibeMakineTarihce tarihce = new HibeMakineTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.YENI;
                tarihce.hibeKod = kod;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Online başvuruyu işleme alma başarıyla tamamlandı.";
                sonuc.anahtar = kod.ToString();

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
        /// Arguman olarak verilen, hibe makine proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetProjeNo(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeMakine hm = new HibeMakine();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "ProjeNo");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            if (string.IsNullOrEmpty(hm.projeNo))
            {
                string plakaKod = new TABGenel(ve).IlPlakaKodBul(hm.ilKod); //TRMServis.UzayYaziDegerGetir("COGRAFYA", hm.ilKod, true, "PLAKAKOD");
                hm.projeNo = ProjeNoAl(ve, hm.etapNo, plakaKod, hm.detay[0].malzemeGrubuKod);
            }
            hm.durum = (int)ENUMHibeMakineBasvuruDurum.PROJENUMARASIVERILDI;

            sonuc = to.KaydetProjeNo(hm);

            if (sonuc.islemSonuc)
            {
                HibeMakineTarihce tarihce = new HibeMakineTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.PROJENUMARASIVERILDI;
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

        /// <summary>
        /// Arguman olarak verilen, hibe makine proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetEksikHibeMiktar(Kullanici kullanan, HibeMakineDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            HibeMakine hibe = new HibeMakine();
            hibe.kod = bilgi.hibeKod;
            ObjectArray liste = Listele(kullanan, hibe);
            if (liste.objeler.Count == 1)
                hibe = (HibeMakine)liste.objeler[0];

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

            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetEksikHibeMiktar(bilgi);

            if (sonuc.islemSonuc)
            {
                TABHibeMakineSulamaGrup tGrup = new TABHibeMakineSulamaGrup(ve);

                HibeMakineSulamaGrup g = new HibeMakineSulamaGrup();
                g.durum = (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER;
                g.ilKod = hibe.ilKod;
                g.odemeKod = 0;
                g.kod = hibe.basvuruNo;
                g.tipi = (int)ENUMHibeTur.MAKINE;

                sonuc = tGrup.Sil(g);
                if (bilgi.gercekHibeTutar > 0)
                    sonuc = tGrup.Kaydet(g);

                HibeMakineTarihce tarihce = new HibeMakineTarihce();
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
        /// Arguman olarak verilen, hibe makine proje no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetReddedildi(VeriErisim ve, Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeMakine hm = new HibeMakine();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "Red");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            hm.redGerekce = bilgi.redGerekce;
            hm.durum = (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI;
            sonuc = to.KaydetReddedildi(hm);
            if (sonuc.islemSonuc)
            {
                HibeMakineTarihce tarihce = new HibeMakineTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI;
                tarihce.hibeKod = hm.kod;
                tarihce.aciklama = bilgi.redGerekce;

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

        public Sonuc YatirimciDegistir(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeMakine hm = new HibeMakine();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "YatirimciDegistir");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            sonuc = KontrolKimlikNoKayitVarMi(null, kullanan, bilgi.kod, bilgi.kimlikNo, hm.etapNo, false);
            if (!sonuc.islemSonuc)
            {
                sonuc.islemSonuc = false;
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

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            sonuc = to.YatirimciDegistir(bilgi);
            if (sonuc.islemSonuc)
            {
                HibeMakineTarihce tarihce = new HibeMakineTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.TANIMSIZ;
                tarihce.hibeKod = hm.kod;
                tarihce.aciklama = bilgi.redGerekce + " Yeni Kimlik Numarası:" + bilgi.kimlikNo + "; Eski Kimlik Numarası:" + bilgi.ePosta;//Eski kimlik numarası tutuluyor

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

        public Sonuc KaydetBasvuruYapildi(Kullanici kullanan, int hibeKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeMakine bilgi = new HibeMakine();
            HibeMakine hm = new HibeMakine();
            bilgi.kod = hibeKod;
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "BasvuruYapildi");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetDurumDegistir(hibeKod, (int)ENUMHibeMakineBasvuruDurum.YENI);
            if (sonuc.islemSonuc)
            {
                HibeMakineTarihce tarihce = new HibeMakineTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.YENI;

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

        ////Grup İşlemleri**************************
        public Sonuc GrupKaydet(Kullanici kullanan, HibeMakineSulamaGrup bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeMakine gelen = new HibeMakine();
            gelen.kod = bilgi.hibeKod;

            HibeMakine hm = new HibeMakine();
            string hata = IslemKontrol(kullanan, gelen, ref hm, "Grup");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            //Grup bilgisi kontrolü hibemakine ya verilebilir mi?
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

            TABHibeMakine to = new TABHibeMakine(ve);
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

            HibeMakine kriter = new HibeMakine();
            kriter.kod = bilgi.hibeKod;
            ObjectArray hibeListe = TRMHibeMakine.Listele(kullanan, kriter, 0, 0, "", true);
            foreach (HibeMakine h in hibeListe.objeler)
            {
                if (h.durum >= (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "(" + h.basvuruNo + ") Başvurunun durumundan dolayı gruptan çıkartılamaz";
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
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
            return GrupVeBasvuruDurumDegistir(veriErisim, kullanan, bilgi.kod, ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER);
        }
        public static Sonuc GrupOdemeBaslat(VeriErisim veriErisim, Kullanici kullanan, HibeMakineSulamaGrup[] bilgi, int odemeKod)
        {
            return GrupVeBasvuruDurumDegistir(veriErisim, kullanan, bilgi, ENUMHibeMakineBasvuruDurum.ODEMEBASLAT, odemeKod);
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
            TABHibeMakine th = new TABHibeMakine(ve);

            foreach (HibeMakineSulamaGrup g in gler)
            {
                if (tur == (int)ENUMHibeOdemeTuru.BANKAGONDER)
                    g.durum = (int)ENUMHibeMakineBasvuruDurum.BANKAYAGONDERILDI;
                else
                    g.durum = (int)ENUMHibeMakineBasvuruDurum.ODENDI;

                g.odemeKod = odemeKod;

                sonuc = TRMHibeMakineSulamaGrup.Kaydet(ve, kullanan, g);
                if (sonuc.islemSonuc)
                {
                    sonuc = th.GuncelleDurumGruptakiler(g.kod, g.durum);

                    HibeMakineTarihce tarihce = new HibeMakineTarihce();
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

            int sorguDurum = (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER;
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
            TABHibeMakine th = new TABHibeMakine(ve);

            HibeMakineSulamaGrup g = new HibeMakineSulamaGrup();
            g.durum = (int)ENUMHibeMakineBasvuruDurum.YENI;
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
                sonuc = th.GuncelleDurumGruptakiler(bilgi.kod, (int)ENUMHibeMakineBasvuruDurum.YERINDETESPIT);

                HibeMakineTarihce tarihce = new HibeMakineTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.ODEMEGERIGELDI;

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

        private static Sonuc GrupVeBasvuruDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, HibeMakineSulamaGrup[] bilgi, ENUMHibeMakineBasvuruDurum durum, int odemeKod)
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
        private static Sonuc GrupVeBasvuruDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, string grupKod, ENUMHibeMakineBasvuruDurum durum, int odemeKod = 0)
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
            if ((durum == ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER && g.durum != (int)ENUMHibeMakineBasvuruDurum.YENI) ||
                (durum == ENUMHibeMakineBasvuruDurum.ODEMEBASLAT && g.durum != (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER) ||
                (durum == ENUMHibeMakineBasvuruDurum.ODENDI && g.durum != (int)ENUMHibeMakineBasvuruDurum.ODEMEBASLAT))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = g.kod + " numaralı grubun durumu uygun değil.";
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeMakine th = new TABHibeMakine(ve);
            if ((durum == ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER && th.GruptaFarkliDurumVarMi(g.kod, (int)ENUMHibeMakineBasvuruDurum.YERINDETESPIT)) ||
                (durum == ENUMHibeMakineBasvuruDurum.ODEMEBASLAT && th.GruptaFarkliDurumVarMi(g.kod, (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER)) ||
                (durum == ENUMHibeMakineBasvuruDurum.ODENDI && th.GruptaFarkliDurumVarMi(g.kod, (int)ENUMHibeMakineBasvuruDurum.ODEMEBASLAT)))
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

                HibeMakineTarihce tarihce = new HibeMakineTarihce();
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
        public static HibeMakineSulamaGrup[] HibeMakineListeleGrupBazliToplam(VeriErisim ve, Kullanici kullanan, int etapNo, int durum, int tur = -1)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeMakine tHibe = new TABHibeMakine(ve);
            IDataReader r = tHibe.HibeMakineListeleGrupBazliToplam(etapNo, durum, tur, Yetki.YetkiliOlduguIlleriGetir(kullanan));
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
                grup.toplamTutar = VeriErisim.DegerAl(r, sonKolon + 2, (decimal)0);
                grup.ayniKatki = VeriErisim.DegerAl(r, sonKolon + 3, (decimal)0);
                grup.hibeEsasTutar = VeriErisim.DegerAl(r, sonKolon + 4, (decimal)0);

                if (grup.kod.Length == 11)//Eksik ödeme yapanlara başvuru numarası yazılıyor bu nedenle hibe tutarı değil eksikodenenodenecek alanındaki değer gelmeli
                    grup.hibeTutar = VeriErisim.DegerAl(r, sonKolon + 6, (decimal)0);
                else
                    grup.hibeTutar = VeriErisim.DegerAl(r, sonKolon + 5, (decimal)0);

                if (grup.hibeTutar > 0)
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
        /// Arguman olarak verilen, hibe makine fatura bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetFatura(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeMakine hm = new HibeMakine();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "Fatura");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            hm.bankaSubeKod = bilgi.bankaSubeKod;
            hm.faturaNo = bilgi.faturaNo;
            hm.faturaTarih = bilgi.faturaTarih;
            hm.durum = (int)ENUMHibeMakineBasvuruDurum.FATURAGELDI;
            hm.detay = bilgi.detay;

            sonuc = to.KaydetFatura(hm);

            if (sonuc.islemSonuc && hm.detay != null && hm.detay.Length > 0)
            {
                foreach (var detay in hm.detay)
                {
                    HibeTutarHesapla(hm.hukukiStatu, detay.referansTutar, detay.proformaTutar, detay.malzemeTutar, ref detay.hibeEsasTutar, ref detay.toplamTutar, ref detay.ayniKatki, ref detay.hibeTutar, detay.hibeOran, detay.adet);
                    sonuc = to.KaydetFaturaDetay(detay, hm.kod);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                HibeMakineTarihce tarihce = new HibeMakineTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.FATURAGELDI;
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
        public Sonuc MalzemeDegistir(Kullanici kullanan, HibeMakineDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeMakine hm = new HibeMakine();
            HibeMakine kontrol = new HibeMakine();
            kontrol.kod = bilgi.hibeKod;
            string hata = IslemKontrol(kullanan, kontrol, ref hm, "MalzemeDegistir");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            sonuc = to.MalzemeDegistir(bilgi);

            if (sonuc.islemSonuc)
            {
                HibeMakineTarihce tarihce = new HibeMakineTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.DEGISTIRILDI;
                tarihce.hibeKod = bilgi.hibeKod;
                tarihce.aciklama = bilgi.aciklama;

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
        /// Arguman olarak verilen, hibe makine sözleşme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetYerindeTespit(Kullanici kullanan, HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            HibeMakine hm = new HibeMakine();
            string hata = IslemKontrol(kullanan, bilgi, ref hm, "YerindeTespit");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
            ve.BeginTransaction();

            hm.yerindeTespitTarih = bilgi.yerindeTespitTarih;
            hm.durum = (int)ENUMHibeMakineBasvuruDurum.YERINDETESPIT;

            sonuc = to.KaydetYerindeTespit(hm);

            if (sonuc.islemSonuc)
            {
                HibeMakineTarihce tarihce = new HibeMakineTarihce();
                tarihce.kullaniciKod = kullanan.kullaniciKodu;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeMakineBasvuruDurum.YERINDETESPIT;
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
        public static ObjectArray Sorgula(Kullanici kullanan, HibeMakine kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc)
        {
            return Listele(kullanan, kriter, ilkKayit, kayitSay, sortKolon, asc);
        }
        public ObjectArray Listele(Kullanici kullanan, HibeMakine kriter)
        {
            return Listele(kullanan, kriter, 0, 0, string.Empty, true);
        }
        private static string[,] orderByKolon = { { "kod", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.KOD] },
                                                { "basvuruNo", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.BASVURUNO] },
                                                { "kimlikNo", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.KIMLIKNO] },
                                                { "ad", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.AD] + ", " + TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.SOYAD] },
                                                { "yil", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.YIL] },
                                                { "etapNo", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.ETAPNO] },
                                                { "grupKod", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.GRUPKOD] },
                                                { "il", "ORGCOGRAFYA.AD" },
                                                { "ilce", "ORGCOGRAFYA_1.AD" },
                                                { "koy", TABKoy.TABLOAD + "." + TABKoy.ALANLAR[(int)TABKoy.ENUMAlanlar.ADI] },
                                                { "evrakTarih", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.EVRAKTARIH] },
                                                { "sozlesmeTarih", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.SOZLESMETARIH] },
                                                { "durum", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.DURUM] },
                                                { "evrakNo", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.EVRAKNO] },
                                                { "malzemeGrubuAd", TABMalzemeGrubu.TABLOAD + "." + TABMalzemeGrubu.ALANLAR[(int)TABMalzemeGrubu.ENUMAlanlar.AD] },
                                                { "malzemeAd", TABHibeMakinaTanim.TABLOAD + "." + TABHibeMakinaTanim.ALANLAR[(int)TABHibeMakinaTanim.ENUMAlanlar.AD] },
                                                { "proformaTutar", TABHibeMakine.TABLOADDETAY + "." + TABHibeMakine.ALANLARDETAY[(int)TABHibeMakine.ENUMAlanlarDetay.PROFORMATUTAR] },
                                                { "faturaTutar", TABHibeMakine.TABLOADDETAY + "." + TABHibeMakine.ALANLARDETAY[(int)TABHibeMakine.ENUMAlanlarDetay.MALZEMETUTAR] },
                                                { "hibeTutar", TABHibeMakine.TABLOADDETAY + "." + TABHibeMakine.ALANLARDETAY[(int)TABHibeMakine.ENUMAlanlarDetay.HIBETUTAR] },
                                                { "hibeEsasTutar", TABHibeMakine.TABLOADDETAY + "." + TABHibeMakine.ALANLARDETAY[(int)TABHibeMakine.ENUMAlanlarDetay.HIBEESASTUTAR] },
                                                { "ayniKatki", TABHibeMakine.TABLOADDETAY + "." + TABHibeMakine.ALANLARDETAY[(int)TABHibeMakine.ENUMAlanlarDetay.AYNIKATKI] },
                                                { "toplamTutar", TABHibeMakine.TABLOADDETAY + "." + TABHibeMakine.ALANLARDETAY[(int)TABHibeMakine.ENUMAlanlarDetay.TOPLAMTUTAR] },
                                                { "referansTutar", TABHibeMakine.TABLOADDETAY + "." + TABHibeMakine.ALANLARDETAY[(int)TABHibeMakine.ENUMAlanlarDetay.REFERANSTUTAR] },
                                                { "firmaAd", TABHibeMakine.TABLOADDETAY + "." + TABHibeMakine.ALANLARDETAY[(int)TABHibeMakine.ENUMAlanlarDetay.FIRMAAD] },
                                                { "malzemeSeriNo", TABHibeMakine.TABLOADDETAY + "." + TABHibeMakine.ALANLARDETAY[(int)TABHibeMakine.ENUMAlanlarDetay.MALZEMESERINO] },
                                                { "puan", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.PUAN] },
                                                { "projeNo", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.PROJENO] } ,
                                                { "puanOrjinal", TABHibeMakine.TABLOAD + "." + TABHibeMakine.ALANLAR[(int)TABHibeMakine.ENUMAlanlar.PUANORJINAL] }};

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
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe makine bilgisini veritabanından okur 
        /// ve HibeMakine nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hibe makine listesini HibeMakine nesne koleksiyonu olarak tutan nesne</returns>
        private static ObjectArray Listele(Kullanici kullanan, HibeMakine kriter, int ilkKayit, int kayitSay, string sortKolon, bool asc)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            if (!kriter.online)
            {
                if (!Yetki.YetkiVarMiOkumaya(kullanan, kriter.ilKod))
                {
                    sonuc.hataStr = "Okuma yetkiniz bulunmamaktadır.";
                    sonuc.islemSonuc = false;
                    bilgi.sonuc = sonuc;
                    return bilgi;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, KYM.Arac.KimlikBul(kullanan));
            TABHibeMakine to = new TABHibeMakine(ve);

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

            IDataReader r = to.Listele(kriter, OrderByKolonBul(sortKolon), asc, kayitSay + ilkKayit);

            if (to.Hata() == "")
            {
                int sonKolon = ((int)TABHibeMakine.ENUMAlanlar.PUANORJINAL) + 1;
                int sonKolon2 = sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.EKSIKODEMEACIKLAMA;

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

                    HibeMakine f = new HibeMakine();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.KOD, 0);
                    f.yil = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.YIL, 0);
                    f.basvuruNo = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.BASVURUNO, string.Empty);
                    f.ilKod = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.ILKOD, string.Empty);
                    f.ilceKod = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.ILCEKOD, string.Empty);
                    f.koyKod = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.KOYKOD, 0);
                    f.kasaba = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.KASABA, string.Empty);
                    f.hukukiStatu = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.HUKUKISTATU, 0);
                    f.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.KIMLIKNO, string.Empty);
                    f.vergiDaire = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.VERGIDAIRE, string.Empty);
                    f.ad = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.AD, string.Empty);
                    f.evrakNo = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.EVRAKNO, string.Empty);
                    f.evrakTarih = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.EVRAKTARIH);
                    f.adres = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.ADRES, string.Empty);
                    f.telefon = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.TELEFON, string.Empty);
                    f.faks = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.FAKS, string.Empty);
                    f.ePosta = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.EPOSTA, string.Empty);
                    f.bankaSubeKod = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.BANKASUBEKOD, string.Empty);
                    f.durum = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.DURUM, 0);
                    f.sozlesmeTarih = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.SOZLESMETARIH);
                    f.faturaNo = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.FATURANO, string.Empty);
                    f.faturaTarih = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.FATURATARIH);
                    f.soyad = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.SOYAD, string.Empty);
                    f.babaAd = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.BABAAD, string.Empty);
                    f.dogumTarih = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.DOGUMTARIH);
                    f.yerindeTespitTarih = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.YERINDETESPITTARIH);
                    f.projeNo = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.PROJENO, string.Empty);
                    f.grupKod = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.GRUPKOD, string.Empty);
                    f.etapNo = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.ETAPNO, 0);
                    f.ozellik = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.OZELLIK, 0);
                    f.puan = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.PUAN, 0.0);
                    f.puanOrjinal = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.PUANORJINAL, 0.0);

                    HibeMakineDetay[] detay = new HibeMakineDetay[1];
                    detay[0] = new HibeMakineDetay();
                    detay[0].hibeKod = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.HIBEKOD, 0);
                    detay[0].satirNo = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.SATIRNO, 0);
                    detay[0].malzemeKod = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.MALZEMEKOD, 0);
                    detay[0].proformaTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.PROFORMATUTAR, (decimal)0);
                    detay[0].malzemeSeriNo = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.MALZEMESERINO, "");
                    detay[0].firmaVergiNo = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.FIRMAVERGINO, "");
                    detay[0].firmaAd = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.FIRMAAD, "");
                    detay[0].malzemeTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.MALZEMETUTAR, (decimal)0);
                    detay[0].aciklama = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.ACIKLAMA, "");
                    detay[0].hibeTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.HIBETUTAR, (decimal)0);
                    detay[0].hibeEsasTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.HIBEESASTUTAR, (decimal)0);
                    detay[0].ayniKatki = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.AYNIKATKI, (decimal)0);
                    detay[0].toplamTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.TOPLAMTUTAR, (decimal)0);

                    detay[0].referansTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.REFERANSTUTAR, (decimal)0);
                    detay[0].adet = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.ADET, 0);

                    detay[0].gercekFatura = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.GERCEKFATURA, (decimal)0);

                    detay[0].gercekHibeTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.GERCEKHIBETUTARI, (decimal)0);
                    detay[0].eksikHibeTutar = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.EKSIKHIBETUTARI, (decimal)0);
                    detay[0].eksikOdemeOdenecek = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.EKSIKODEMEODENECEK, (decimal)0);
                    detay[0].eksikOdemeDuzeltildi = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.EKSIKODEMEDUZELTILDI, 0);
                    detay[0].eksikFazlaOdemeAciklama = VeriErisim.DegerAl(r, sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.EKSIKODEMEACIKLAMA, "");

                    detay[0].malzemeGrubuKod = VeriErisim.DegerAl(r, sonKolon2 + 1, 0);
                    detay[0].malzemeGrubuAd = VeriErisim.DegerAl(r, sonKolon2 + 2, "");
                    detay[0].malzemeAd = VeriErisim.DegerAl(r, sonKolon2 + 3, "");
                    detay[0].hibeOran = VeriErisim.DegerAl(r, sonKolon2 + 4, decimal.Zero);

                    f.detay = detay;

                    f.koyAd = VeriErisim.DegerAl(r, sonKolon2 + 5, "");
                    f.ilAd = VeriErisim.DegerAl(r, sonKolon2 + 6, "");
                    f.ilceAd = VeriErisim.DegerAl(r, sonKolon2 + 7, "");

                    bool yetki = false;

                    if (kriter.kod > 0)
                    {
                        if (kriter.online)
                            yetki = true;
                        else
                        {
                            //Eğer tek bir kaydı görmek istiyor ise (Başvuru ekranı gibi)
                            if (TNS.TRM.Yetki.YetkiVarMiOkumaya(kullanan, f.ilKod))
                                yetki = true;
                        }
                    }
                    else
                        yetki = true;

                    if (yetki)
                        bilgi.objeler.Add(f);
                }
            }
            r.Close();

            ve.VeriErisimKapat();
            return bilgi;
        }
        public static int HibeMakineBasvuruSayiDondur(VeriErisim veriErisim, Kullanici kullanan, HibeMakine kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeMakine th = new TABHibeMakine(ve);
            int basvuruSayi = th.HibeMakineBasvuruSayiDondur(kriter);
            ve.VeriErisimKapat();
            return basvuruSayi;
        }

        public int[] AktifEtapdaKimlikNoVarmi(VeriErisim veriErisim, string kimlikNo, int tip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeMakine th = new TABHibeMakine(ve);
            int aktifEtapNo;

            if (tip == 1)
                aktifEtapNo = TRMHibeMakineYonetim.HibeMakineAktifEtapNo(ve);
            else if (tip == 2)
                aktifEtapNo = TRMHibeSulamaYonetim.HibeSulamaAktifEtapNo(ve);
            else
                aktifEtapNo = TRMHibeYatirimYonetim.HibeYatirimAktifEtapNo(ref ve);

            IDataReader r = th.AktifEtapdaKimlikNoVarmi(kimlikNo, aktifEtapNo, tip);
            int[] arr = new int[2];

            if (th.Hata() == "")
            {
                if (r.Read())
                {
                    arr[0] = VeriErisim.DegerAl(r, 0, 0);
                    arr[1] = VeriErisim.DegerAl(r, 1, 0);
                }
            }

            ve.VeriErisimKapat();
            return arr;
        }

        public int[] OncekiEtapdaKimlikNoVarmi(VeriErisim veriErisim, string kimlikNo, int tip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHibeMakine th = new TABHibeMakine(ve);
            int aktifEtapNo=0;
            int oncekiEtapNo = 0;

            if (tip == 1)
                aktifEtapNo = TRMHibeMakineYonetim.HibeMakineAktifEtapNo(ve);
            else if (tip == 2)
                aktifEtapNo = TRMHibeSulamaYonetim.HibeSulamaAktifEtapNo(ve);
            else
                aktifEtapNo = TRMHibeYatirimYonetim.HibeYatirimAktifEtapNo(ref ve);

            oncekiEtapNo = aktifEtapNo - 1;

            IDataReader r = th.AktifEtapdaKimlikNoVarmi(kimlikNo, oncekiEtapNo, tip);
            int[] arr = new int[2];

            if (th.Hata() == "")
            {
                if (r.Read())
                {
                    arr[0] = VeriErisim.DegerAl(r, 0, 0);
                    arr[1] = VeriErisim.DegerAl(r, 1, 0);
                }
            }

            ve.VeriErisimKapat();
            return arr;
        }

        public static double HibeMakineOdemeTutarGetir(Kullanici kullanan, int tur)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
            double odemeMiktar = to.HibeMakineOdemeMiktarGetir(tur);
            ve.VeriErisimKapat();
            return odemeMiktar;
        }
        private string ProjeNoAl(VeriErisim ve, int etapNo, string ilKod, int makineGrupKod)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKriter = etapNo.ToString("00") + "." + ilKod.PadLeft(2, '0') + "." + makineGrupKod.ToString().PadLeft(4, '0');
            string sayacKod = "HIBEMAKINEPROJENO" + sayacKriter.Replace(".", "");

            return sayacKriter + "." + tg.BelgeNoAl(sayacKod, "00000");
        }
        private string BasvuruNoAl(VeriErisim ve, int etapNo, string ilKod)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKriter = etapNo.ToString("00") + "." + ilKod.PadLeft(2, '0');
            string sayacKod = "HIBEMAKINEBASVURUNO" + sayacKriter.Replace(".", "");

            return sayacKriter + "." + tg.BelgeNoAl(sayacKod, "00000");
        }
        private int BelgeNoAlInt(VeriErisim ve)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKod = "HIBEMAKINEKOD";

            return tg.BelgeNoAlInt(sayacKod, "000000");
        }
        private Sonuc KontrolKimlikNoKayitVarMi(VeriErisim ve, Kullanici kullanan, int kod, string kimlikNo, int aktifEtapNo, bool degistirilemezKontrolu = true)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);

            HibeMakine kriter = new HibeMakine();
            if (kod > 0 && degistirilemezKontrolu)
                kriter.kod = kod;
            else
                kriter.kimlikNo = kimlikNo;

            kriter.etapNo = aktifEtapNo;

            IDataReader r = to.KontrolKimlikNoKayitVarMi(kriter);

            HibeMakine hm = new HibeMakine();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    hm.kod = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.KOD, 0);
                    hm.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlar.KIMLIKNO, string.Empty);
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
        public ObjectArray ListeleTarihce(Kullanici kullanan, HibeMakine kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            Sonuc sonuc = new Sonuc();

            if (!Yetki.YetkiVarMiOkumaya(kullanan, kriter.ilKod))
            {
                sonuc.hataStr = "Okuma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                bilgi.sonuc = sonuc;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeMakine to = new TABHibeMakine(ve);
            IDataReader r = to.ListeleTarihce(new HibeMakineTarihce { hibeKod = kriter.kod });

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeMakineTarihce f = new HibeMakineTarihce();

                    f.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlarTarihce.HIBEKOD, 0);
                    f.kullaniciKod = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlarTarihce.KULLANICIKOD, string.Empty);
                    f.tarih = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlarTarihce.TARIH);
                    f.durum = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlarTarihce.DURUM, 0);
                    f.aciklama = VeriErisim.DegerAl(r, (int)TABHibeMakine.ENUMAlanlarTarihce.ACIKLAMA, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
        public string IslemKontrol(Kullanici kullanan, HibeMakine bilgi, ref HibeMakine hm, string tur)
        {
            if (bilgi.kod == 0)
                return "İşlem yapılacak kayıta ait kod bilgisi alınamadı";

            ObjectArray hmler = Listele(kullanan, new HibeMakine { kod = bilgi.kod });
            if (hmler == null || hmler.objeler.Count == 0 || hmler.objeler.Count > 1)
                return "İşlem yapılacak kayıta ait bilgi okunamadı";

            hm = (HibeMakine)hmler.objeler[0];
            Sonuc s = new Sonuc();
            if (!Yetki.YetkiVarMiYazmaya(kullanan, hm.ilKod, hm.etapNo, s))
                return s.hataStr;

            if (!(hm.hukukiStatu == (int)ENUMHibeMakineHukukiStatu.TUZELKISI || hm.hukukiStatu == (int)ENUMHibeMakineHukukiStatu.SUTBIRLIK))
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
                if (string.IsNullOrEmpty(hm.projeNo))//hm.durum < (int)ENUMHibeMakineBasvuruDurum.PROJENUMARASIVERILDI || 
                    hata += "Sözleşme yapabilmek için Proje Numarası verilmiş olması gerekmektedir.<br>";
                if (hm.durum > (int)ENUMHibeMakineBasvuruDurum.SOZLESMEYAPILDI)
                    hata += "Sözleşme kaydı aşamasını geçmiş bulunmaktasınız.<br>";
                if (bilgi.sozlesmeTarih.isNull)
                    hata += "Sözleşme Tarihi boş bırakılamaz<br>";
                //else if (bilgi.sozlesmeTarih.Oku() > DateTime.Now)
                //    hata += "Sözleşme Tarihi, bugünden sonraki bir gün olamaz<br>";
                else if (bilgi.sozlesmeTarih.Oku() < hm.evrakTarih.Oku())
                    hata += "Sözleşme Tarihi, başvuru evrak tarihinden (" + hm.evrakTarih.ToString() + ") önceki bir gün olamaz<br>";

                HibeMakineDetay[] detay = bilgi.detay;
                if (bilgi.detay == null)
                    detay = hm.detay;

                foreach (HibeMakineDetay ff in detay)
                {
                    decimal merkezReferansTutar = TRMHibeMakinaTanim.ReferansFiyatListele(hm.etapNo, ff.malzemeKod, bilgi.ilKod);

                    if (ff.malzemeAd.IndexOf("Diğer") > -1)
                        hata += "Sözleşme yapılmak istenilen Başvuru Malzemesi, 'Diğer' olarak seçilmiştir. Lütfen destek verilecek başvuru malzemesinin tam karşılığını başvuru giriş ekranından kayıt ettikten sonra Sözleşme işlemini yapınız.<br>";
                    if (ff.referansTutar == 0)
                        hata += "Referans Birim fiyat bilgi girişi yapılmamış.<br>";
                    else if (merkezReferansTutar > 0 && ff.referansTutar > merkezReferansTutar)
                        hata += "İliniz tarafından bu malzeme için belirlenen <b>Referans Birim Fiyat</b>, Bakanlık tarafından belirlenen (" + merkezReferansTutar.ToString("#,###.0000") + "TL) fiyattan yüksektir. Lütfen sizin belirlediğiniz referans fiyatı düzeltin.<br>";
                }
            }
            else if (tur == "ProjeNo")
            {
                if (hm.durum > (int)ENUMHibeMakineBasvuruDurum.PROJENUMARASIVERILDI)
                    hata += "Proje Numarası verme aşamasını geçmiş bulunmaktasınız.<br>";
                //if (!string.IsNullOrEmpty(hm.projeNo))
                //    hata += "Başvuruya ait proje numarası bulunmaktadır.<br>";

                foreach (HibeMakineDetay ff in hm.detay)
                {
                    decimal merkezReferansTutar = TRMHibeMakinaTanim.ReferansFiyatListele(hm.etapNo, ff.malzemeKod, bilgi.ilKod);

                    if (ff.malzemeAd.IndexOf("Diğer") > -1)
                        hata += "Proje Numarası verilmek istenilen Başvuru Malzemesi, 'Diğer' olarak seçilmiştir. Lütfen destek verilecek başvuru malzemesinin tam karşılığını başvuru giriş ekranından kayıt ettikten sonra Proje Numarası verme işlemini yapınız.<br>";
                    if (ff.referansTutar == 0)
                        hata += "Referans Birim fiyat bilgi girişi yapılmamış.<br>";
                    else if (merkezReferansTutar > 0 && ff.referansTutar > merkezReferansTutar)
                        hata += "İliniz tarafından bu malzeme için belirlenen <b>Referans Birim Fiyat</b>, Bakanlık tarafından belirlenen (" + merkezReferansTutar.ToString("#,###.0000") + "TL) fiyattan yüksektir. Lütfen sizin belirlediğiniz referans fiyatı düzeltin.<br>";
                }
            }
            else if (tur == "Fatura")
            {
                if (hm.sozlesmeTarih.isNull)//hm.durum < (int)ENUMHibeMakineBasvuruDurum.SOZLESMEYAPILDI
                    hata += "Fatura girişi yapabilmek için Sözleşme yapılmış olması gerekmektedir.<br>";
                if (hm.durum > (int)ENUMHibeMakineBasvuruDurum.YERINDETESPIT)//Yerinde tespit yapılmış kayıtlarda değişiklik yapılmak isteniyor
                    hata += "Fatura girişi aşamasını geçmiş bulunmaktasınız.<br>";
                if (bilgi.bankaSubeKod == "")
                    hata += "Şube bilgisi boş bırakılamaz<br>";
                if (string.IsNullOrEmpty(bilgi.faturaNo))
                    hata += "Fatura No boş bırakılamaz<br>";
                if (bilgi.faturaTarih.isNull)
                    hata += "Fatura Tarihi boş bırakılamaz<br>";
                else if (bilgi.faturaTarih.Oku() > DateTime.Now)
                    hata += "Fatura Tarihi bugünden sonraki bir gün olamaz<br>";

                if (!bilgi.faturaTarih.isNull)
                {
                    int kontrolTarih = 60;
                    //int haftaSonu = 0;
                    //if (hm.sozlesmeTarih.Oku().AddDays(kontrolTarih).DayOfWeek == DayOfWeek.Sunday)
                    //    haftaSonu = 1;
                    //else if (hm.sozlesmeTarih.Oku().AddDays(kontrolTarih).DayOfWeek == DayOfWeek.Saturday)
                    //    haftaSonu = 2;

                    if (!hm.sozlesmeTarih.isNull && (bilgi.faturaTarih.Oku() < hm.sozlesmeTarih.Oku()))
                        hata += "Fatura Tarihi, Sözleşme Tarihi'nden (" + hm.sozlesmeTarih.ToString() + ") önceki bir gün olamaz<br>";
                    if (hm.sozlesmeTarih.Oku().AddDays(kontrolTarih) < bilgi.faturaTarih.Oku())
                        hata += "Sözleşme tarihi ile Fatura Tarihi arasında en fazla " + kontrolTarih + " gün olabilir<br>";
                }

                HibeMakineDetay[] detay = bilgi.detay;
                if (bilgi.detay == null)
                    detay = hm.detay;

                foreach (HibeMakineDetay ff in detay)
                {
                    decimal merkezReferansTutar = TRMHibeMakinaTanim.ReferansFiyatListele(hm.etapNo, ff.malzemeKod, bilgi.ilKod);

                    if (ff.firmaVergiNo == bilgi.kimlikNo)
                        hata += "Makinenin satın alındığı firma ile hibe desteği alan kişi aynı olamaz. Lütfen makinenin satın alındığı firmanın Vergi Numarası girin.<br>";
                    if (ff.malzemeAd.IndexOf("Diğer") > -1)
                        hata += "Proje Numarası verilmek istenilen Başvuru Malzemesi, 'Diğer' olarak seçilmiştir. Lütfen destek verilecek başvuru malzemesinin tam karşılığını başvuru giriş ekranından kayıt ettikten sonra Proje Numarası verme işlemini yapınız.<br>";
                    if (ff.referansTutar == 0)
                        hata += "Referans Birim fiyat bilgi girişi yapılmamış.<br>";
                    else if (merkezReferansTutar > 0 && ff.referansTutar > merkezReferansTutar)
                        hata += "İliniz tarafından bu malzeme için belirlenen <b>Referans Birim Fiyat</b>, Bakanlık tarafından belirlenen (" + merkezReferansTutar.ToString("#,###.0000") + "TL) fiyattan yüksektir. Lütfen sizin belirlediğiniz referans fiyatı düzeltin.<br>";
                }
            }
            else if (tur == "YerindeTespit")
            {
                if (hm.durum < (int)ENUMHibeMakineBasvuruDurum.FATURAGELDI || string.IsNullOrEmpty(hm.faturaNo))
                    hata += "Yerinde Tespit yapabilmek için Fatura bilgilerinin girilmiş olması gerekmektedir.<br>";
                if (hm.durum > (int)ENUMHibeMakineBasvuruDurum.YERINDETESPIT)
                    hata += "Yerinde Tespit aşamasını geçmiş bulunmaktasınız.<br>";
                if (bilgi.yerindeTespitTarih.isNull)
                    hata += "Yerinde Tespit Tarihi boş bırakılamaz<br>";
                else if (bilgi.yerindeTespitTarih.Oku() > DateTime.Now)
                    hata += "Yerinde Tespit Tarihi, bugünden sonraki bir gün olamaz<br>";

                if (!bilgi.yerindeTespitTarih.isNull)
                {
                    int kontrolTarih = 25;

                    foreach (HibeMakineDetay ff in hm.detay)
                    {
                        if (ff.malzemeGrubuKod == 314)//Süt sağım ünitesi ve soğutma tankı
                            kontrolTarih = 30;
                    }

                    kontrolTarih = kontrolTarih + 60 + 10;//60 gün fatura+10 gün evrakları getirme

                    if (!hm.faturaTarih.isNull && (bilgi.yerindeTespitTarih.Oku() < hm.faturaTarih.Oku()))
                        hata += "Yerinde Tespit Tarihi, Fatura Tarihi'nden (" + hm.faturaTarih.ToString() + ") önceki bir gün olamaz<br>";
                    //if (hm.faturaTarih.Oku().AddDays(kontrolTarih) < bilgi.yerindeTespitTarih.Oku())//Fatura tarihi önemli değil sözleşme tarihi
                    if (hm.sozlesmeTarih.Oku().AddDays(kontrolTarih) < bilgi.yerindeTespitTarih.Oku())
                        hata += "Yerinde Tespit tarihi ile Sözleşme tarihi arasında en fazla " + kontrolTarih + " gün olabilir<br>";
                }

                foreach (HibeMakineDetay ff in hm.detay)
                {
                    decimal merkezReferansTutar = TRMHibeMakinaTanim.ReferansFiyatListele(hm.etapNo, ff.malzemeKod, bilgi.ilKod);

                    if (ff.malzemeAd.IndexOf("Diğer") > -1)
                        hata += "Yerinde Tespit yapılmak istenilen Başvuru Malzemesi, 'Diğer' olarak seçilmiştir. Lütfen destek verilecek başvuru malzemesinin tam karşılığını başvuru giriş ekranından kayıt ettikten sonra Yerinde Tespit işlemini yapınız.<br>";
                    if (ff.referansTutar == 0)
                        hata += "Referans Birim fiyat bilgi girişi yapılmamış.<br>";
                    else if (merkezReferansTutar > 0 && ff.referansTutar > merkezReferansTutar)
                        hata += "İliniz tarafından bu malzeme için belirlenen <b>Referans Birim Fiyat</b>, Bakanlık tarafından belirlenen (" + merkezReferansTutar.ToString("#,###.0000") + "TL) fiyattan yüksektir. Lütfen sizin belirlediğiniz referans fiyatı düzeltin.<br>";
                }
            }
            else if (tur == "Red")
            {
                if (hm.durum >= (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER &&
                    hm.durum < (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "(" + hm.basvuruNo + ") Ödeme işlemi başlamış başvuru reddedilemez.";
            }
            else if (tur == "BasvuruYapildi")
            {
                if (hm.durum >= (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER &&
                    hm.durum < (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "(" + hm.basvuruNo + ") Ödeme işlemi başlamış başvuru üzerinde işlem yapılamaz.";
            }
            else if (tur == "Grup")
            {
                if (hm.durum >= (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER &&
                    hm.durum < (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "(" + hm.basvuruNo + ") Ödeme işlemi başlamış başvuruya grup numarası verilemez.";
            }
            else if (tur == "YatirimciDegistir")
            {
                if (hm.durum >= (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER &&
                    hm.durum < (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI)
                    hata += "(" + hm.basvuruNo + ") Ödeme işlemi başlamış yatırımcı değiştirilemez.";
            }
            return hata;
        }

        public static HibeMakine[] KimlikBilgisiEksikBasvurular(VeriErisim ve, Kullanici kullanan)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                throw new Exception("Okuma yetkiniz bulunmamaktadır.");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);

            IList<HibeMakine> al = new List<HibeMakine>();
            IDataReader r = to.KimlikBilgisiEksikBasvurular();
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (r.Read())
                {
                    HibeMakine hm = new HibeMakine();
                    hm.kod = VeriErisim.DegerAl(r, 0, 0);
                    hm.kimlikNo = VeriErisim.DegerAl(r, 1, string.Empty);

                    al.Add(hm);
                }
            }
            r.Close();
            ve.VeriErisimKapat();

            HibeMakine[] liste = new HibeMakine[al.Count];
            al.CopyTo(liste, 0);
            return liste;
        }
        public static Sonuc KimlikBilgisiEksikGuncelle(VeriErisim ve, Kullanici kullanan, HibeMakine hm)
        {
            Sonuc sonuc = new Sonuc();
            if (hm.kod <= 0 || string.IsNullOrEmpty(hm.ad) || string.IsNullOrEmpty(hm.soyad))
                return sonuc;

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);
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
        public static void HibeTutarHesapla(int hukukiStatu, decimal referansTutar, decimal proformaTutar, decimal malzemeTutar, ref decimal hibeEsasTutar, ref decimal toplamTutar, ref decimal ayniKatki, ref decimal hibeTutar, decimal hibeOran, int adet)
        {
            decimal ustSinir = OrtakFonksiyonlar.ConvertToDecimal(TRMHibeMakineYonetim.HibeMakineAktifUstLimit(null, hukukiStatu));

            referansTutar = referansTutar * adet;//1 adet olarak girilen referans fiyat adet ile çarpılarak toplam ref fiyat bulunur

            if (referansTutar == 0)
                referansTutar = proformaTutar;

            if (referansTutar > 0)
                hibeEsasTutar = proformaTutar > referansTutar ? referansTutar : proformaTutar;

            if (malzemeTutar > 0)
                hibeEsasTutar = hibeEsasTutar > malzemeTutar ? malzemeTutar : hibeEsasTutar;

            hibeEsasTutar = hibeEsasTutar > ustSinir ? ustSinir : hibeEsasTutar;

            if (malzemeTutar > 0)
                toplamTutar = malzemeTutar;
            else
                toplamTutar = proformaTutar;

            ayniKatki = toplamTutar - hibeEsasTutar;
            hibeTutar = Math.Round(hibeEsasTutar * (hibeOran / 100), 2);
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
            ObjectArray hibeler = new TRMHibeMakine().Listele(kullanan, new HibeMakine() { kod = hibeKod });
            if (!hibeler.sonuc.islemSonuc)
                return hibeler.sonuc;
            if (hibeler.objeler.Count == 0)
                return new Sonuc(false, "İlgili hibe başvurusu bulunamadı!", string.Empty);
            if (!Yetki.YetkiVarMiYazmaya(kullanan, ((HibeMakine)hibeler.objeler[0]).ilKod, ((HibeMakine)hibeler.objeler[0]).etapNo, hibeler.sonuc, string.Empty, ENUMHibeTur.MAKINE))
                return hibeler.sonuc;

            if (((HibeMakine)hibeler.objeler[0]).durum >= (int)ENUMHibeMakineBasvuruDurum.SOZLESMEYAPILDI)
                return new Sonuc(false, "Bu başvuru ile sözleşme yapıldığı için değerlendirme işlemi yapılamaz.", string.Empty);

            AnketBilgi anket = DegerlendirmeOku(kullanan, hibeKod, anahtar);
            double anketPuan = DegerlendirmePuanHesapla(anket);

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeMakine to = new TABHibeMakine(ve);

            double puanOrjinal = to.ListeleDegerlendirmePuan(hibeKod);//Daha önceden kayıt edilmiş ve anket ekranından kayıt edilmeden veya puanı aynı olarak çıkılmış ise arttırma puan kaybolmasın
            if (puanOrjinal != anketPuan + artisPuan)
            {
                ve.BeginTransaction();

                sonuc = to.KaydetDegerlendirmeSonuc(hibeKod, anketPuan + artisPuan, anketPuan);
                if (sonuc.islemSonuc)
                {
                    HibeMakineTarihce tarihce = new HibeMakineTarihce();
                    tarihce.hibeKod = hibeKod;
                    tarihce.tarih = new TNSDateTime(DateTime.Now);
                    tarihce.kullaniciKod = kullanan.kullaniciKodu;
                    tarihce.durum = 100;//Değerlendirme kodu olsun.
                    tarihce.aciklama = "Değerlendirme işlemi sonucu :" + anketPuan + " puan";
                    if (artisPuan > 0)
                        tarihce.aciklama = artisPuan.ToString() + " Puan artışı yapıldı. Eski Puanı:" + anketPuan + " -  Yeni Puan:" + (anketPuan + artisPuan);

                    sonuc = to.KaydetTarihce(tarihce);
                }

                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = anketPuan.ToString();
                    sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = "İşlem sırasında hata oluştu.<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static AnketBilgi DegerlendirmeOku(Kullanici kullanan, int hibeKod, string anahtar)
        {
            try
            {
                AnketBilgi anketKistasBilgi = new AnketBilgi();
                anketKistasBilgi.modul = Arac.anketModul;
                anketKistasBilgi.anketAnahtar = anahtar;
                anketKistasBilgi.orgBirimKod = 1;
                anketKistasBilgi.cevapAnahtar = hibeKod.ToString();

                AnketBilgi anketBilgi = TNS.KYM.Arac.Tanimla().AnketBilgiOku(kullanan, anketKistasBilgi, true, true);
                if (anketBilgi == null)
                    throw new Exception("Kayıt bulunamadı!");
                return anketBilgi;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static double DegerlendirmePuanHesapla(AnketBilgi anketBilgi)
        {
            if (anketBilgi == null || anketBilgi.sorular == null || anketBilgi.sorular.Length == 0)
                return 0;

            double puan = 0;
            foreach (AnketSoru soru in anketBilgi.sorular)
            {
                if (soru.Cevap(0) != null)
                    puan += soru.Cevap(0).puan;
            }
            return puan;
        }

        public HibeMakineServis[] HibeMakineListForTKDK(Kullanici kullanan, string kimlikNo)
        {
            HibeMakineServis[] liste;
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKWEBSERVISKULLANICISI))
            {
                liste = new HibeMakineServis[1];
                liste[0] = new HibeMakineServis();
                liste[0].sonuc = new Sonuc(false, "Yetkiniz yok.", "");
                return liste;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);

            IDataReader dr = to.HibeMakineListForTKDK(kimlikNo);

            if (!string.IsNullOrEmpty(to.Hata()))
            {
                dr.Close();
                ve.VeriErisimKapat();
                liste = new HibeMakineServis[1];
                liste[0] = new HibeMakineServis();
                liste[0].sonuc = new Sonuc(false, to.Hata(), "");
                return liste;
            }

            IList<HibeMakineServis> al = new List<HibeMakineServis>();
            while (dr.Read())
            {
                HibeMakineServis hy = new HibeMakineServis();
                hy.isim = VeriErisim.DegerAl(dr, 0, "");
                hy.unvan = HibeMakine.StatuAdBul(VeriErisim.DegerAl(dr, 1, 0));
                hy.il = VeriErisim.DegerAl(dr, 2, "");
                hy.adres = VeriErisim.DegerAl(dr, 3, "");
                hy.malzemeGrubu = VeriErisim.DegerAl(dr, 4, "");
                hy.malzeme = VeriErisim.DegerAl(dr, 5, "");
                hy.basvuruDurumu = HibeMakineServis.DurumAdBul(VeriErisim.DegerAl(dr, 6, 0));
                hy.sozlesmeTarihi = VeriErisim.DegerAl(dr, 7).ToString();
                hy.yil = VeriErisim.DegerAl(dr, 8, 0);
                hy.etapNo = VeriErisim.DegerAl(dr, 9, 0);
                al.Add(hy);
            }
            dr.Close();

            if (al.Count > 0)
            {
                liste = new HibeMakineServis[al.Count];
                al.CopyTo(liste, 0);
            }
            else
            {
                liste = new HibeMakineServis[1];
                liste[0] = new HibeMakineServis();
                liste[0].sonuc = new Sonuc(false, "Girdiğiniz kimlik numarasına ait başvuru bilgisi yok.", "");
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static HazineRapor[] HazineRapor(int yil, int ay)
        {
            HazineRapor[] liste;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakine to = new TABHibeMakine(ve);

            //Tüzel
            IDataReader dr = to.HazineRaporTuzel(yil, ay);
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
                al.Add(hy);
            }
            dr.Close();

            //Gerçek
            dr = to.HazineRaporGercek(yil, ay);
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

    }
}
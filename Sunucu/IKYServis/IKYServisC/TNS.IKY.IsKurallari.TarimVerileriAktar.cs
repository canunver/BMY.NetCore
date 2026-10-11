using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.IKY.IsKurallari
{
    public class TarimVerileriAktar
    {
        public static TNS.KYM.IKYMServis kymServis = null;
        public static Dictionary<string, string[]> _ilListesi = null;
        public static Dictionary<string, string[]> IlListesi
        {
            get
            {
                if (_ilListesi == null)
                    IlleriListele();
                return _ilListesi;
            }
        }

        public static Dictionary<int, object[]> _detsisListesi = null;
        public static Dictionary<int, object[]> DetsisListesi
        {
            get
            {
                if (_detsisListesi == null)
                    IlleriListele();
                return _detsisListesi;
            }
        }

        public static Dictionary<string, int> _hizmetSebebiListesi = null;
        public static Dictionary<string, int> HizmetSebebiListesi
        {
            get
            {
                if (_hizmetSebebiListesi == null)
                    IlleriListele();
                return _hizmetSebebiListesi;
            }
        }

        public static Dictionary<string, int> _ogrenimListesi = null;
        public static Dictionary<string, int> OgrenimListesi
        {
            get
            {
                if (_ogrenimListesi == null)
                    IlleriListele();
                return _ogrenimListesi;
            }
        }

        private static void IlleriListele()
        {
            if (_ilListesi == null)
            {
                _ilListesi = new Dictionary<string, string[]>();
                if (kymServis == null)
                    kymServis = TNS.KYM.Arac.Tanimla();
                Cografya cForm = new Cografya();
                cForm.ilceMi = true;
                cForm.ilMi = true;
                Cografya[] yerler = kymServis.ListeleCografya(new KYM.Kullanici() { kullaniciKodu = "servis" }, cForm);
                string ilKod = "", ilAd = "";
                foreach (Cografya item in yerler)
                {
                    if (item.mernisKod > 0)
                    {
                        if (item.mernisKod < 1000)
                        {
                            ilKod = item.kod;
                            ilAd = item.ad;
                        }
                        else
                        {
                            _ilListesi[ilAd + " " + item.ad] = new string[] { ilKod, item.kod };
                        }
                    }
                }
            }
            if (_detsisListesi == null)
            {
                _detsisListesi = new Dictionary<int, object[]>();
                VeriErisim veSGB = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", "VeriAktarim");
                try
                {
                    TABTarimIKY tabTBK = new TABTarimIKY(veSGB);
                    if (tabTBK.DetsisListele())
                    {
                        while (tabTBK.DetsisOku())
                        {
                            _detsisListesi[tabTBK.PBKadroBirimDetsisNo] = new object[] { tabTBK.DTBirimKodu, tabTBK.DTMuhasebeKodu, tabTBK.DTKurKod, tabTBK.DTOrgKod };
                        }
                    }
                }
                catch
                {
                }
                veSGB.VeriErisimKapat();
            }
            if (_hizmetSebebiListesi == null)
            {
                ObjectArray oa = IKYHizmetSebebi.Listele(null, new KYM.Kullanici() { kullaniciKodu = "servis" }, new HizmetSebebi() { tip = ENUMHizmetSebebiNevi.TANIMSIZ });
                _hizmetSebebiListesi = new Dictionary<string, int>();
                foreach (HizmetSebebi item in oa.objeler)
                {
                    _hizmetSebebiListesi[item.sebepAdi] = item.kod;
                }
            }
            if (_ogrenimListesi == null)
            {
                _ogrenimListesi = new Dictionary<string, int>();
                VeriErisim veSGB = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", "VeriAktarim");
                try
                {
                    TABTarimIKY tabTBK = new TABTarimIKY(veSGB);
                    if (tabTBK.OgrenimListele())
                    {
                        while (tabTBK.OgrenimOku())
                        {
                            _ogrenimListesi[tabTBK.OGRKayitBul()] = tabTBK.OGRKod;
                        }
                    }
                }
                catch
                {
                }
                veSGB.VeriErisimKapat();
            }
            return;
        }

        internal static Sonuc SiraliVeriAktar()
        {
            OrtakFonksiyonlar.HataStrYaz("SiraliVeriAktar:");

            Thread thread = new Thread(new ThreadStart(SiraliVeriAktarThread));
            thread.Start();
            return new Sonuc("", "Aktarım görevi tanımlandı, kişileri kontrol edilerek aktarılacak");
        }
        public static void SiraliVeriAktarThread()
        {
            VeriErisim veTBK = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPERv2", "VeriAktarim");
            VeriErisim veSGB = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", "VeriAktarim");
            string tcKimlikNo = veSGB.TekKolonSQLStr("SELECT TCKIMLIKNO FROM SONAKTARIMBILGI WHERE TUR=1");
            OrtakFonksiyonlar.HataStrYaz("SiraliVeriAktarThread:" + tcKimlikNo);
            DateTime basla = DateTime.Now;
            try
            {
                while (true)
                {
                    Sonuc s = VeriAktar(veTBK, veSGB, tcKimlikNo, false);
                    if (!s.islemSonuc)
                    {
                        OrtakFonksiyonlar.HataStrYaz($"{s.anahtar} tc kimlik nolu kişi aktarılamadı:{s.hataStr} {s.bilgiStr}");
                    }
                    if (string.IsNullOrWhiteSpace(s.anahtar))
                        throw new Exception($"Sonraki personel okunamadı! dönen anahtar:{s.anahtar} tcKimlikNo:{tcKimlikNo}");
                    if (s.anahtar == tcKimlikNo)
                        throw new Exception($"Aktarma işlemi sonraki personeli kendisi olarak buldu! dönen anahtar:{s.anahtar} tcKimlikNo:{tcKimlikNo}");
                    else
                    {
                        tcKimlikNo = s.anahtar;
                        if (string.IsNullOrEmpty(veSGB.hata))
                            veSGB.ExecuteNonquery($"UPDATE SONAKTARIMBILGI set TCKIMLIKNO = {VeriErisim.DegerYap(tcKimlikNo)} WHERE TUR=1");
                    }
                    if (!string.IsNullOrEmpty(veSGB.hata))
                        throw new Exception($"dönen anahtar:{s.anahtar} tcKimlikNo:{tcKimlikNo} Veri tabanı hatası:{veSGB.hata}");
                    //if ((DateTime.Now- basla).TotalMinutes > 120)
                    //    break;
                }
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataStrYaz(ex.Message);
            }
            veTBK.VeriErisimKapat();
            veSGB.VeriErisimKapat();
        }

        internal static Sonuc VeriAktar(VeriErisim veTBKOnc, VeriErisim veSGBOnc, string tcKimlikNo, bool kendisi)
        {
            IlleriListele();

            Sonuc s = new Sonuc();
            VeriErisim veTBK = VeriErisim.VeriErisimBul(veTBKOnc, "BaglantiSatiriPERv2", "VeriAktarim");
            VeriErisim veSGB = VeriErisim.VeriErisimBul(veSGBOnc, "BaglantiSatiriIKY", "VeriAktarim");

            try
            {
                TABTarimIKY tabTBK = new TABTarimIKY(veTBK);
                TABPersonel tabPersonel = new TABPersonel(veSGB);
                TABGenel tabGenel = new TABGenel(veSGB);
                TABPersonelOgrenim tabOgrenim = new TABPersonelOgrenim(veSGB);
                TABPersonelKadro tabPerKadro = new TABPersonelKadro(veSGB);
                if (tabTBK.ListelePer(tcKimlikNo, kendisi))
                {
                    PersonelKadro hareketKimlik = new PersonelKadro();
                    if (tabTBK.PerOku(true, IlListesi, DetsisListesi))
                    {
                        List<PersonelKadro> lPersonelKadro = new List<PersonelKadro>();
                        List<PersonelOgrenim> lPersonelOgrenim = new List<PersonelOgrenim>();
                        IKY.Personel per = new Personel();
                        per.tcKimlikNo = tabTBK.PBTcKimlikNo;
                        per.ad = tabTBK.PBAdi;
                        per.soyad = tabTBK.PBSoyadi;
                        per.nufus.dogumTarihi.Yaz(TarihDogruMu(tabTBK.PBDogumTarihi));
                        per.nufus.cinsiyeti = (ENUMCINSIYET)tabTBK.PBCinsiyet;
                        per.nufus.nufusIlKod = tabTBK.PBNufusIl;
                        per.nufus.nufusIlceKod = tabTBK.PBNufusIlce;
                        per.nufus.medeniHali = tabTBK.PBMedeniHal;
                        per.nufus.anaAd = tabTBK.PBAnneAdi;
                        per.nufus.babaAd = tabTBK.PBBabaAdi;
                        per.nufus.dogumYeri = tabTBK.PBDogumYeri;
                        per.nufus.nufusMahalleKoy = tabTBK.PBNufusMahalleKoy;
                        per.askerlik.aciklama = tabTBK.PBAskerlikDurumu;
                        per.iletisim.adres = tabTBK.PBEvAdresi;
                        per.iletisim.cepTelefon = tabTBK.PBCepTelefonu;
                        per.iletisim.isTelefon = tabTBK.PBIsTelefonu;
                        per.iletisim.eposta = tabTBK.PBEPosta;
                        per.sicilNo = tabTBK.PBKurumSicilNo;
                        per.emekliSicilNo = tabTBK.PBEmekliSicilNo;
                        per.ilkIseBaslamaTarihi.Yaz(TarihDogruMu(tabTBK.PBIlkMemuriyeteBaslamaTarihi));
                        KadroKaydiOl(tabTBK, hareketKimlik);
                        s.anahtar = per.tcKimlikNo;
                        if (tabTBK.ListelePerOgrenim(tabTBK.PBTcKimlikNo))
                        {
                            while (tabTBK.PerOgrenimOku(OgrenimListesi))
                            {
                                PersonelOgrenim po = new PersonelOgrenim();
                                po.belgeTarihi.Yaz(TarihDogruMu(tabTBK.PODiplomaTarihi));
                                po.bitirmeTarihi.Yaz(TarihDogruMu(tabTBK.POMezuniyetTarihi));
                                po.belgeNo = tabTBK.PODiplomaBelgeNo;
                                po.ogrenimKod = tabTBK.POOgrenimKod;
                                po.ogrenimSuresi = tabTBK.POSure;
                                po.belgeTur = 88;
                                po.bolumAdi = tabTBK.POBolumAdi;
                                po.bolumKod = tabTBK.POBolumKod;
                                po.fakulteAdi = tabTBK.POFakulteAdi;
                                po.fakulteKod = tabTBK.POFakulteKod;

                                po.okulAdi = tabTBK.POOgrenimAdi;
                                po.universiteAdi = tabTBK.POOkulAdi;
                                po.universiteKod = tabTBK.POUniversiteKod;

                                lPersonelOgrenim.Add(po);
                            }
                        }
                        int aktifPer = 0;
                        if (tabTBK.ListelePerHareket(tabTBK.PBTcKimlikNo))
                        {
                            PersonelKadro oncPk = null;
                            PersonelKadro pk;
                            while (tabTBK.PerHareketOku(DetsisListesi, HizmetSebebiListesi))
                            {
                                if (!TNSDateTime.IsNull(tabTBK.PHBaslamaTarihi))
                                {
                                    pk = new PersonelKadro();
                                    pk.kadroDurum = ENUMKadroDurumu.AYRILDI;

                                    pk.unvan = tabTBK.PHUnvanId;
                                    pk.unvanAd = tabTBK.PHUnvanAdi;

                                    pk.kadroDerece = tabTBK.PHKazanilanDerece;
                                    pk.kadroTarih.Yaz(tabTBK.PHBaslamaTarihi);

                                    pk.odemeyeEsasDerece = tabTBK.PHOdenecekDerece;
                                    pk.odemeyeEsasKademe = tabTBK.PHOdenecekKademe;
                                    pk.odemeyeEsasGosterge = tabTBK.PHOdenecekGosterge;
                                    pk.odemeyeEsasEkGosterge = tabTBK.PHOdenecekEkGosterge;
                                    pk.odemeyeEsasTarih.Yaz(tabTBK.PHBaslamaTarihi);

                                    pk.kazanilmisHakDerece = tabTBK.PHKazanilanDerece;
                                    pk.kazanilmisHakKademe = tabTBK.PHKazanilanKademe;
                                    pk.kazanilmisGosterge = tabTBK.PHKazanilanGosterge;
                                    pk.kazanilmisEkGosterge = tabTBK.PHKazanilanEkGosterge;
                                    pk.kazanilmisHakTarih.Yaz(tabTBK.PHBaslamaTarihi);

                                    pk.emekliyeEsasDerece = tabTBK.PHEmeklilikDerece;
                                    pk.emekliyeEsasKademe = tabTBK.PHEmeklilikKademe;
                                    pk.emekliyeEsasGosterge = tabTBK.PHEmeklilikGosterge;
                                    pk.emekliyeEsasEkGosterge = tabTBK.PHEmeklilikEkGosterge;
                                    pk.emekliyeEsasTarih.Yaz(tabTBK.PHBaslamaTarihi);

                                    pk.hizmetSebebi = "Aktarım";
                                    pk.hizmetSebebiKod = tabTBK.PHHizmetSebebiKod;
                                    pk.vekilUnvanKod = 0;
                                    pk.atamaTarihi.Yaz(tabTBK.PHBaslamaTarihi);
                                    pk.onayTarih.Yaz(tabTBK.PHBaslamaTarihi);
                                    pk.onaylayan = "Aktarim";
                                    pk.hizmetGunSayisi = 0;
                                    pk.KHADurumu = 0;
                                    if (tabTBK.PHBirimId > 0)
                                    {
                                        pk.organizasyonKodu = tabTBK.PHBirimOrgKod;
                                        pk.muhasebeBirimi = tabTBK.PHBirimBirimKodu;
                                        pk.muhasebeKodu = tabTBK.PHBirimMuhasebeKodu;
                                        pk.kurumsalKod = tabTBK.PHBirimKurKod;
                                        pk.kurumDisi = 0;
                                        pk.gorevYeri = tabTBK.PHBirimAdi;
                                    }
                                    else
                                    {
                                        pk.kurumDisi = 1;
                                        pk.gorevYeri = tabTBK.PHBirimAdi;
                                    }
                                    pk.personelTip = tabTBK.PHPersonelTipi;
                                    pk.gorevOrganizasyonKodu = 0;
                                    if (oncPk != null)
                                    {
                                        oncPk.ayrilmaTarihi.Yaz(tabTBK.PHBaslamaTarihi);
                                        oncPk.ayrilmaGerekcesi = tabTBK.PHHizmetSebebiKod;
                                        oncPk.onayTarih.Yaz(tabTBK.PHBaslamaTarihi);
                                    }
                                    TNSDateTime dt = TarihDogruMu(tabTBK.PHBaslamaTarihi);
                                    if (!TNSDateTime.IsNull(dt))
                                    {
                                        lPersonelKadro.Add(pk);
                                        oncPk = pk;
                                    }
                                    else
                                        s.DegerAta("", $"{per.tcKimlikNo} {per.ad} {per.soyad} kişisine ait hatalı hareket kaydı bulundu:{tabTBK.PHBaslamaTarihi}!\r\n<br />");
                                }
                                else if (!TNSDateTime.IsNull(tabTBK.PHAyrilmaTarihi))
                                {
                                    TNSDateTime dt = TarihDogruMu(tabTBK.PHAyrilmaTarihi);
                                    if (!TNSDateTime.IsNull(dt))
                                    {
                                        PersonelKadro itemOnc = null;
                                        foreach (PersonelKadro item in lPersonelKadro)
                                        {
                                            if (item.atamaTarihiDT > tabTBK.PHAyrilmaTarihi.Oku())
                                            {
                                                if (itemOnc != null)
                                                {
                                                    itemOnc.ayrilmaTarihi.Yaz(tabTBK.PHAyrilmaTarihi);
                                                    itemOnc.ayrilmaGerekcesi = tabTBK.PHHizmetSebebiKod;
                                                    itemOnc.onayTarih.Yaz(tabTBK.PHAyrilmaTarihi);
                                                    itemOnc = null;
                                                }
                                                break;
                                            }
                                            itemOnc = item;
                                        }
                                        if (itemOnc != null)
                                        {
                                            itemOnc.ayrilmaTarihi.Yaz(tabTBK.PHAyrilmaTarihi);
                                            itemOnc.ayrilmaGerekcesi = tabTBK.PHHizmetSebebiKod;
                                            itemOnc.onayTarih.Yaz(tabTBK.PHAyrilmaTarihi);
                                            itemOnc = null;
                                        }
                                    }
                                    else
                                        s.DegerAta("", $"{per.tcKimlikNo} {per.ad} {per.soyad} kişisine ait hatalı hareket kapanış kaydı bulundu:{tabTBK.PHAyrilmaTarihi}!\r\n<br />");
                                }
                            }
                            if (lPersonelKadro.Count == 0)
                            {
                                if(hareketKimlik.kadroDurum == ENUMKadroDurumu.AYRILDI)
                                    hareketKimlik.ayrilmaTarihi.Yaz(DateTime.Now);
                            }
                            else
                            {
                                pk = lPersonelKadro[lPersonelKadro.Count - 1];
                                if (TNSDateTime.IsNull(pk.ayrilmaTarihi))
                                {
                                    pk.kadroDurum = ENUMKadroDurumu.AKTIFKAYIT;
                                    aktifPer = 1;
                                }
                                else
                                    pk.kadroDurum = ENUMKadroDurumu.AKTIFKAPANDI;

                                if (tabTBK.PBGeciciGorevBirimDetsisNo > 0)
                                {
                                    pk.gorevOrganizasyonKodu = tabTBK.PBGeciciGorevOrgKod;
                                    //pk.go = tabTBK.PHBirimBirimKodu;
                                    //pk.muhasebeKodu = tabTBK.PHBirimMuhasebeKodu;
                                    //pk.kurumsalKod = tabTBK.PHBirimKurKod;
                                    //pk.kurumDisi = 0;
                                    //pk.gorevYeri = tabTBK.PHBirimAdi;

                                    //, out PBGeciciGorevBirimKodu, out PBGeciciGorevMuhasebeKodu, out PBGeciciGorevKurKod, out 
                                }
                                if (tabTBK.PBGeciciGorevUnvanId > 0)
                                {
                                    pk.vekilUnvanKod = tabTBK.PBGeciciGorevUnvanId;
                                    pk.vekilUnvanAd = tabTBK.PBGeciciGorevUnvanAdi;
                                }
                            }
                        }
                        //if (lPersonelKadro.Count == 0)
                        //{
                        //    s.DegerAta("", $"{per.tcKimlikNo} {per.ad} {per.soyad} kişisine ait, hareket kaydı bulunamadığından, kayıt işlemi yapılmadı!\r\n<br />");
                        //}
                        //else
                        {
                            Sonuc kayitSonuc;
                            veSGB.BeginTransaction();
                            if (per.kod == 0)
                            {
                                kayitSonuc = tabPersonel.Kaydet(per, aktifPer);
                                per.iletisim.kod = per.kod;
                                per.nufus.kod = per.kod;
                                if (kayitSonuc.islemSonuc)
                                    kayitSonuc = tabPersonel.KaydetIletisim(per.iletisim, true);
                                if (kayitSonuc.islemSonuc)
                                    kayitSonuc = tabPersonel.KaydetNufus(per.nufus, true);
                                if (kayitSonuc.islemSonuc)
                                {
                                    //kayitSonuc = tabOgrenim.Sil(per.kod);
                                    foreach (PersonelOgrenim ogr in lPersonelOgrenim)
                                    {
                                        if (!kayitSonuc.islemSonuc) break;
                                        ogr.personelKod = per.kod;
                                        kayitSonuc = tabOgrenim.Kaydet(ogr);
                                    }
                                }
                                if (kayitSonuc.islemSonuc)
                                {
                                    //kayitSonuc = tabPerKadro.SilPersonelKadro(per.kod);
                                    foreach (PersonelKadro kadro in lPersonelKadro)
                                    {
                                        if (!kayitSonuc.islemSonuc) break;
                                        kadro.personelKod = per.kod;
                                        kayitSonuc = tabPerKadro.KaydetKadro(kadro);
                                    }
                                }
                            }
                            else
                            {
                                //kayitSonuc = tabPersonel.Guncelle(per);
                                //if (kayitSonuc.islemSonuc)
                                //    kayitSonuc = tabPersonel.KaydetIletisim(per.iletisim, false);
                                //if (kayitSonuc.islemSonuc)
                                //    kayitSonuc = tabPersonel.KaydetNufus(per.nufus, false);
                                kayitSonuc = new Sonuc("Güncelleme hazır değil");
                            }
                            if (kayitSonuc.islemSonuc)
                                veSGB.Commit();
                            else
                            {
                                s.DegerAta("", $"{per.tcKimlikNo} {per.ad} {per.soyad} kişisine ait işlem yapılmadı: {kayitSonuc.hataStr}");
                                veSGB.Rollback();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                s.DegerAta(ex.Message);
            }

            veSGB.VeriErisimKapat();
            veTBK.VeriErisimKapat();

            return s;
        }

        private static void KadroKaydiOl(TABTarimIKY tabTBK, PersonelKadro hareketKimlik)
        {
            if (tabTBK.PBOzlukDurumId == 235)
                hareketKimlik.kadroDurum = ENUMKadroDurumu.AKTIFKAYIT;
            else
                hareketKimlik.kadroDurum = ENUMKadroDurumu.AYRILDI;

            hareketKimlik.unvan = tabTBK.PBKadroUnvanId;
            hareketKimlik.unvanAd = tabTBK.PBKadroUnvanAdi;

            hareketKimlik.kadroDerece = tabTBK.PBSonPersonelHareketKadroDerece;
            //hareketKimlik.kadroKademe = tabTBK.PBSonPersonelHareketKadroKademe;
            //hareketKimlik.kadroGosterge = tabTBK.PBSonPersonelHareketKadroGosterge;
            //hareketKimlik.kadroEkGosterge = tabTBK.PBSonPersonelHareketKadroEkGosterge;
            hareketKimlik.kadroTarih.Yaz(tabTBK.PBIlkMemuriyeteBaslamaTarihi);

            hareketKimlik.odemeyeEsasDerece = tabTBK.PBSonPersonelHareketOdenecekDerece;
            hareketKimlik.odemeyeEsasKademe = tabTBK.PBSonPersonelHareketOdenecekKademe;
            hareketKimlik.odemeyeEsasGosterge = tabTBK.PBSonPersonelHareketOdenecekGosterge;
            hareketKimlik.odemeyeEsasEkGosterge = tabTBK.PBSonPersonelHareketOdenecekEkGosterge;
            hareketKimlik.odemeyeEsasTarih.Yaz(tabTBK.PBIlkMemuriyeteBaslamaTarihi);

            hareketKimlik.kazanilmisHakDerece = tabTBK.PBSonPersonelHareketKadroDerece;
            hareketKimlik.kazanilmisHakKademe = tabTBK.PBSonPersonelHareketKadroKademe;
            hareketKimlik.kazanilmisGosterge = tabTBK.PBSonPersonelHareketKadroGosterge;
            hareketKimlik.kazanilmisEkGosterge = tabTBK.PBSonPersonelHareketKadroEkGosterge;
            hareketKimlik.kazanilmisHakTarih.Yaz(tabTBK.PBIlkMemuriyeteBaslamaTarihi);

            hareketKimlik.emekliyeEsasDerece = tabTBK.PBSonPersonelHareketEmeklilikDerece;
            hareketKimlik.emekliyeEsasKademe = tabTBK.PBSonPersonelHareketEmeklilikKademe;
            hareketKimlik.emekliyeEsasGosterge = tabTBK.PBSonPersonelHareketEmeklilikGosterge;
            hareketKimlik.emekliyeEsasEkGosterge = tabTBK.PBSonPersonelHareketEmeklilikEkGosterge;
            hareketKimlik.emekliyeEsasTarih.Yaz(tabTBK.PBIlkMemuriyeteBaslamaTarihi);

            hareketKimlik.hizmetSebebi = "Aktarım";
            hareketKimlik.hizmetSebebiKod = 1;
            hareketKimlik.vekilUnvanKod = 0;
            hareketKimlik.atamaTarihi.Yaz(tabTBK.PBIlkMemuriyeteBaslamaTarihi);
            hareketKimlik.onayTarih.Yaz(tabTBK.PBIlkMemuriyeteBaslamaTarihi);
            hareketKimlik.onaylayan = "Aktarim";
            hareketKimlik.hizmetGunSayisi = 0;
            hareketKimlik.KHADurumu = 0;
            if (tabTBK.PBKadroBirimId > 0)
            {
                hareketKimlik.organizasyonKodu = tabTBK.PBKadroBirimOrgKod;
                hareketKimlik.muhasebeBirimi = tabTBK.PBKadroBirimBirimKodu;
                hareketKimlik.muhasebeKodu = tabTBK.PBKadroBirimMuhasebeKodu;
                hareketKimlik.kurumsalKod = tabTBK.PBKadroBirimKurKod;
                hareketKimlik.kurumDisi = 0;
                hareketKimlik.gorevYeri = tabTBK.PBKadroBirimAdi;
            }
            else
            {
                hareketKimlik.kurumDisi = 1;
                hareketKimlik.gorevYeri = tabTBK.PBKadroBirimAdi;
            }
            hareketKimlik.personelTip = tabTBK.PBPersonelTipi;
            hareketKimlik.gorevOrganizasyonKodu = 0;
        }

        private static TNSDateTime TarihDogruMu(TNSDateTime tnsTarih)
        {
            if (TNSDateTime.IsNull(tnsTarih))
                return null;
            else if (tnsTarih.Yil < 1900)
                return null;
            else
                return tnsTarih;
        }
    }
}

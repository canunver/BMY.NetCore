using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS;
using TNS.HRC;
using TNS.IKY;
using TNS.KYM;

namespace HRCServisI
{
    public class PSLDinBordroHesaplayici
    {
        public class ToplamGun
        {
            public int toplamGun = 0;
            public string unvanGun = "";
            public DateTime basTar;
            public DateTime bitTar;
            public int gorevDurumu = 0;
            public int unvanKod;
        }

        public enum YakinlikTur
        {
            es,
            bakmaklayukumlu0_6,
            bakmaklayukumlu0_6E,
            bakmaklayukumlu6danbuyuk,
            bakmaklayukumlu6danbuyukE,
            agi
        }

        class IzinTarihAra
        {
            public int gun = 0;
            public int izinTur = 0;

            public IzinTarihAra(int pgun, int pizinTur)
            {
                this.gun = pgun;
                this.izinTur = pizinTur;
            }
        }

        public static void KisiEkle(Personel p, KisiSatir eskiDegerler, ArrayList satirlar, bool isGunu, CalisanDinamikBordroDonem bordroDonem, BordroYapi by, string surecNo, int[] personelTipler, ref string hata)
        {
            ObjectArray kisiYakinlik = p.kisiYakinlik;
            PersonelMaasParam paramlar = p.personelMaasParam;

            List<object> satir = new List<object>();

            satir.Add(p.tcKimlikNo);
            PersonelKadro oncekiKadro = null;
            ToplamGun toplamGun = null;
            ToplamGun toplamGunIs = null;
            ToplamGun gorevGun = null;
            for (int i = 0; i < by.hepsi.Length; i++)
            {
                DegiskenFormul item = by.hepsi[i];
                object o = null;
                BordroGenelDegisken bgd = null;
                if (string.IsNullOrWhiteSpace(item.formula))
                    bgd = BordroGenelDegisken.GenelDegiskenBul(item.ad);
                if (bgd != null)
                {
                    if (bgd.degiskenTur == ENUMBordroDegiskenTur.KisiBordroBilgi)
                    {
                        if (paramlar != null)
                        {
                            if (bgd.veriTipi == VariableType.Money)
                                o = paramlar.ParamDegerBul(item.ad);
                            else
                                o = paramlar.ParamDegerBulStr(item.ad);
                        }
                    }
                    else
                    {
                        if (item.ad == "ad") o = p.ad;
                        else if (item.ad == "soyad") o = p.soyad;
                        else if (item.ad == "sicilNo") o = p.sicilNo;
                        else if (item.ad == "sigortaSicilNo") o = p.sskSicilNo;
                        else if (item.ad == "unvan") o = p.unvanAd;
                        else if (item.ad == "unvanKod") o = p.UNVANKOD;
                        else if (item.ad == "unvanSira") o = p.unvanSira;
                        else if (item.ad == "hizmetSinifi") o = p.sinifKodu;
                        else if (item.ad == "cinsiyet") o = (int)p.cinsiyet;
                        else if (item.ad == "es") o = KisiYakinlikBul(YakinlikTur.es, kisiYakinlik, bordroDonem);
                        else if (item.ad == "cocukSay6YasdanKucuk") o = KisiYakinlikBul(YakinlikTur.bakmaklayukumlu0_6, kisiYakinlik, bordroDonem);
                        else if (item.ad == "cocukSay6YasdanBuyuk") o = KisiYakinlikBul(YakinlikTur.bakmaklayukumlu6danbuyuk, kisiYakinlik, bordroDonem);
                        else if (item.ad == "cocukSay6YasdanKucukE") o = KisiYakinlikBul(YakinlikTur.bakmaklayukumlu0_6E, kisiYakinlik, bordroDonem);
                        else if (item.ad == "cocukSay6YasdanBuyukE") o = KisiYakinlikBul(YakinlikTur.bakmaklayukumlu6danbuyukE, kisiYakinlik, bordroDonem);
                        else if (item.ad == "cocukSay") o = KisiYakinlikBul(YakinlikTur.agi, kisiYakinlik, bordroDonem);
                        else if (item.ad == "IBAN") o = p.bankaHesapNo;
                        else if (item.ad == "kadroDerece") o = p.KADRODERECE;
                        else if (item.ad == "sakatlikDerecesi") o = p.sakatlikDerecesi + 10 * YakinlikSakatlikDerecesi(kisiYakinlik, bordroDonem);

                        else if (item.ad == "odemeDerece") o = p.odemeEsasDerece;
                        else if (item.ad == "odemeKademe") o = p.odemeEsasKademe;
                        else if (item.ad == "odemeGosterge") o = p.odemeEsasGosterge;
                        else if (item.ad == "odemeEkGosterge") o = p.odemeEsasEkGosterge;

                        else if (item.ad == "emekliDerece") o = p.emekliEsasDerece;
                        else if (item.ad == "emekliKademe") o = p.emekliEsasKademe;
                        else if (item.ad == "emekliGosterge") o = p.emekliEsasGosterge;
                        else if (item.ad == "emekliEkGosterge") o = p.emekliEsasEkGosterge;

                        else if (item.ad == "KHKDerece") o = p.kazanilmisHakDerece;
                        else if (item.ad == "KHKKademe") o = p.kazanilmisHakKademe;
                        else if (item.ad == "KHKGosterge") o = p.kazanilmisGosterge;
                        else if (item.ad == "KHKEkGosterge") o = p.kazanilmisEkGosterge;

                        else if (item.ad == "dilGostergesi") o = YDilGostergeBul(p.dil, bordroDonem);
                        else if (item.ad == "sendikaBilgi") o = SendikaStrOl(p.sendikalar);
                        else if (item.ad == "lojmanDaireTur") o = LojmanDaireTur(p.lojmanlar);
                        else if (item.ad == "lojmanM2") o = LojmanM2Bul(p.lojmanlar, true);
                        else if (item.ad == "yakitM2") o = LojmanM2Bul(p.lojmanlar, false);

                        else if (item.ad == "kidem") o = KidemYiliBul(p.hizmetSureleri[0]);
                        else if (item.ad == "oncKidem") o = KidemYiliBul(p.hizmetSureleri[1]);
                        else if (item.ad == "kidemK") o = KidemYiliBul(p.kurumaBaslamaTarihi, bordroDonem.bordroDonemBasTarihDT);
                        else if (item.ad == "oncKidemK") o = KidemYiliBul(p.kurumaBaslamaTarihi, bordroDonem.bordroDonemBasTarihDT.AddMonths(-1));

                        else if (item.ad == "oncGVMatrah" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.GelirVergisi, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);//matrah tarihi: odemeTarihDT mi, hazırlama tarihi mi?
                        else if (item.ad == "oncSGKMatrah" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.SGKDevreden, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);
                        else if (item.ad == "sgkMatrahDevir1" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.SGKMtrhBirOnce, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);
                        else if (item.ad == "sgkMatrahDevir2" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.SGKMtrhIkiOnce, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);
                        else if (item.ad == "AGIDevreden" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.AGIDevreden, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);
                        else if (item.ad == "GVIstisnaDevir" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.GVIstisnaDevir, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);
                        else if (item.ad == "DVIstisnaDevir" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.DVIstisnaDevir, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);
                        else if (item.ad == "oncGVHesapMatrah" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.GelirVergisiHesap, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);//matrah tarihi: odemeTarihDT mi, hazırlama tarihi mi?
                        else if (item.ad == "AsgariUcretDevir" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.AsgariUcretMatrah, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);
                        else if (item.ad == "GVHesIstisnaDevir" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.GVHesIstisnaDevir, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);
                        else if (item.ad == "DVHesIstisnaDevir" && paramlar != null) o = paramlar.MatrahDegerBul(PersonelMaasParam.ENUMMatrahKod.DVHesIstisnaDevir, bordroDonem.hazirlamaTarihDT, surecNo, p.personelTip);

                        else if (item.ad == "personelTip") o = p.personelTip;
                        else if (item.ad == "oncEmekliDerece") o = OncekiKadroBul(ref oncekiKadro, p, bordroDonem).emekliyeEsasDerece;
                        else if (item.ad == "oncEmekliKademe") o = OncekiKadroBul(ref oncekiKadro, p, bordroDonem).emekliyeEsasKademe;
                        else if (item.ad == "oncEmekliEkGosterge") o = OncekiKadroBul(ref oncekiKadro, p, bordroDonem).emekliyeEsasEkGosterge;
                        else if (item.ad == "oncEmekliGosterge") o = OncekiKadroBul(ref oncekiKadro, p, bordroDonem).emekliyeEsasGosterge;

                        else if (item.ad == "vekilUnvanAd") o = p.kadro.vekilUnvanAd;
                        else if (item.ad == "vekilUnvanKod") o = p.kadro.vekilUnvanKod;
                        else if (item.ad == "meslekKodu") o = p.kadro.meslekKodu;

                        else if (item.ad == "iseBaslaTar")
                        {
                            if (p.kadro.hizmetSebebiTip == ENUMHizmetSebebiNevi.BASLAMA && ResmiTatil.TarihIcindeMi(p.kadro.atamaTarihiDT, bordroDonem.bordroDonemBasTarihDT, bordroDonem.sonGunDT.AddDays(1)))
                                o = p.kadro.atamaTarihiDT;
                        }
                        else if (item.ad == "istenCikisTar")
                        {
                            if (p.kadro.ayrilmaGerekcesiTip == ENUMHizmetSebebiNevi.AYRILMA && ResmiTatil.TarihIcindeMi(p.kadro.ayrilmaTarihiDT, bordroDonem.bordroDonemBasTarihDT, bordroDonem.sonGunDT.AddDays(1)))
                                o = p.kadro.ayrilmaTarihiDT;
                        }
                        else if (item.ad == "istenCikisNeden")
                        {
                            if (p.kadro.ayrilmaGerekcesiTip == ENUMHizmetSebebiNevi.AYRILMA && ResmiTatil.TarihIcindeMi(p.kadro.ayrilmaTarihiDT, bordroDonem.bordroDonemBasTarihDT, bordroDonem.sonGunDT.AddDays(1)))
                                o = p.kadro.ayrilmaGerekcesi;
                        }
                        else if (item.ad == "kidemBilgi")
                        {
                            o = KidemBilgiBul(p, isGunu, bordroDonem);
                        }
                        else if (item.ad == "kidemKG") o = KidemGunuBul(p, isGunu, bordroDonem);

                        else if (item.ad == "birimTuru") o = p.birimTuru;
                        else if (item.ad == "calistigiBirimTuru") o = p.calistigiBirimTuru;
                        else if (item.ad == "kurumaBaslamaTarihi") o = p.kurumaBaslamaTarihi.Oku();

                        else if (item.ad == "toplamGun") o = ToplamGunBul(ref toplamGun, p, isGunu, false, bordroDonem, personelTipler, ref hata).toplamGun;
                        else if (item.ad == "unvanGunler") o = ToplamGunBul(ref toplamGun, p, isGunu, false, bordroDonem, personelTipler, ref hata).unvanGun;
                        else if (item.ad == "unvanGunlerIs") o = ToplamGunBul(ref toplamGunIs, p, true, false, bordroDonem, personelTipler, ref hata).unvanGun;
                        else if (item.ad == "gorevGunler") o = GorevGunBul(ref gorevGun, p, isGunu, false, bordroDonem).unvanGun;

                        else if (item.ad == "sayKod") o = p.kadro.muhasebeKodu;
                        else if (item.ad == "sayAd") o = p.kadro.muhasebeAd;
                        else if (item.ad == "birimKod") o = p.kadro.muhasebeBirimi;
                        else if (item.ad == "birimAd") o = p.kadro.muhasebeBirimiAd;
                        else if (item.ad.StartsWith("perGider")) o = ToplamPerGiderBul(item.ad, p.giderToplamlar);
                        else if (item.ad.StartsWith("Yemek")) o = ToplamPerGiderBul("perGider1", p.giderToplamlar);
                    }
                }
                else
                {
                    if (item.giris == ENUMBordroDegiskenTur.Elle)
                    {
                        if (eskiDegerler != null)
                            o = CalisanDinamikBordro.DegerBul(by, eskiDegerler, item.ad);
                        if (o == null && !string.IsNullOrWhiteSpace(item.ilkDeger))
                        {
                            if (item.tip == VariableType.Integer) o = OrtakFonksiyonlar.ConvertToInt(item.ilkDeger, 0);
                            else if (item.tip == VariableType.Money) o = OrtakFonksiyonlar.ConvertToDbl(item.ilkDeger);
                            else o = item.ilkDeger;
                        }
                    }
                }

                if (o == null)
                {
                    if (item.tip == VariableType.Integer) o = 0;
                    else if (item.tip == VariableType.Money) o = (double)0;
                    else if (item.tip == VariableType.Date) o = DateTime.MinValue;
                    else o = "";
                }
                satir.Add(o);
            }
            if (eskiDegerler != null && eskiDegerler.degerler.Length == by.hepsi.Length + 2)
                satir.Add(eskiDegerler.degerler[eskiDegerler.degerler.Length - 1]);
            else
                satir.Add(0);
            satirlar.Add(new KisiSatir(satir.ToArray()));
        }

        private static string SendikaStrOl(TNSCollection sendikalar)
        {
            string senBilgi = "";
            if (sendikalar != null)
                foreach (SendikaBilgi item in sendikalar)
                {
                    if (senBilgi != "") senBilgi += ";";
                    senBilgi += $"{item.sendikaNo}-{item.kesintiOrani}-{item.topluGorusmePuani}-{item.hesapNo}";
                }
            return senBilgi;
        }

        private static int YakinlikSakatlikDerecesi(ObjectArray kisiYakinlik, CalisanDinamikBordroDonem bordroDonem)
        {
            int sonuc = 0;
            if (kisiYakinlik != null && kisiYakinlik.sonuc.islemSonuc)
            {
                foreach (TNS.IKY.PersonelAile item in kisiYakinlik.objeler)
                {
                    if (item.bakmaklaYukumlu == 1 && item.sakatlikDerecesi > 0)
                    {
                        sonuc = 10 * sonuc + item.sakatlikDerecesi;
                    }
                }
            }
            return sonuc;
        }

        private static double ToplamPerGiderBul(string ad, ObjectArray giderToplamlar)
        {
            double toplam = 0;
            if (giderToplamlar != null)
            {
                TNS.IKY.IKYPersonelGider.ENUMPersonelHarcamaTur giderTur = (TNS.IKY.IKYPersonelGider.ENUMPersonelHarcamaTur)OrtakFonksiyonlar.ConvertToInt(ad.Substring(8), 1);
                foreach (TNS.IKY.IKYPersonelGider item in giderToplamlar.objeler)
                {
                    if (item.harcamaTur == giderTur) toplam += item.tutar;
                }
            }
            return toplam;
        }

        private static void GunEkle(Gorevler gorev, DateTime basTar, DateTime bitTar, List<ToplamGun> hash, bool isGunu)
        {
            List<ToplamGun> gunler = new List<ToplamGun>();
            ToplamGun t = new ToplamGun();
            t.basTar = basTar;
            t.bitTar = bitTar;
            gunler.Add(t);
            foreach (ToplamGun item in hash)
            {
                bool devam = true;
                while (devam)
                {
                    int gun = 0;
                    DateTime basTarIc1, bitTarIc1;
                    DateTime basTarIc2, bitTarIc2;
                    basTarIc2 = DateTime.MinValue;
                    bitTarIc2 = DateTime.MinValue;
                    foreach (ToplamGun t2 in gunler)
                    {
                        DateTime basTarIc;
                        DateTime bitTarIc;
                        gun = TNS.ResmiTatil.KesisimBul(item.basTar, item.bitTar, t2.basTar, t2.bitTar, isGunu, out basTarIc, out bitTarIc);
                        if (gun > 0)
                        {
                            GunBol(t2.basTar, t2.bitTar, basTarIc, bitTarIc, out basTarIc1, out bitTarIc1, out basTarIc2, out bitTarIc2);
                            if (basTarIc1 != DateTime.MinValue)
                            {
                                t2.basTar = basTarIc1;
                                t2.bitTar = bitTarIc1;
                            }
                            else if (basTarIc2 != DateTime.MinValue)
                            {
                                t2.basTar = basTarIc2;
                                t2.bitTar = bitTarIc2;
                                basTarIc2 = DateTime.MinValue;
                            }
                            else
                            {
                                t2.basTar = DateTime.MinValue;
                                t2.bitTar = DateTime.MinValue;
                                gun = 0;
                            }
                            break;
                        }
                    }
                    if (gun > 0)
                    {
                        if (basTarIc2 != DateTime.MinValue)
                        {
                            ToplamGun tYen = new ToplamGun();
                            tYen.basTar = basTarIc2;
                            tYen.bitTar = bitTarIc2;
                            gunler.Add(tYen);
                        }
                    }
                    else
                        devam = false;
                }
            }
            foreach (ToplamGun item in gunler)
            {
                if (item.basTar != DateTime.MinValue)
                {
                    item.gorevDurumu = (int)gorev.gorevDurumu;
                    item.toplamGun = TNS.ResmiTatil.GunBul(item.basTar, item.bitTar, false, isGunu);
                    item.unvanKod = gorev.unvanKod;
                    hash.Add(item);
                }
            }
        }

        private static void GunBol(DateTime bas, DateTime bit, DateTime cikBas, DateTime cikBit, out DateTime sonuc1bas, out DateTime sonuc1bit, out DateTime sonuc2bas, out DateTime sonuc2bit)
        {
            sonuc1bas = DateTime.MinValue;
            sonuc1bit = DateTime.MinValue;
            sonuc2bas = DateTime.MinValue;
            sonuc2bit = DateTime.MinValue;

            if (bas < cikBas)
            {
                sonuc1bas = bas;
                sonuc1bit = cikBas.AddDays(-1);
            }

            if (cikBit < bit)
            {
                sonuc2bas = cikBit;
                sonuc2bit = bit;
            }
            if (sonuc1bas == DateTime.MinValue)
            {
                sonuc1bas = sonuc2bas;
                sonuc1bit = sonuc2bit;
                sonuc2bas = DateTime.MinValue;
                sonuc2bit = DateTime.MinValue;
            }
            return;
        }

        private static ToplamGun ToplamGunBul(ref ToplamGun gunler, Personel p, bool isGunu, bool vekalet, CalisanDinamikBordroDonem bordroDonem, int[] personelTipler, ref string hata)
        {
            if (gunler == null)
            {
                gunler = new ToplamGun();
                if (p.kadrolar != null && p.kadrolar.objeler != null)
                {
                    try
                    {
                        BirimTuruDegistir(bordroDonem.bordroDonemBasTarihDT, p);
                        for (int ii = 0; ii < p.kadrolar.ObjeSayisi; ii++)
                        {
                            PersonelKadro item = (PersonelKadro)p.kadrolar.objeler[ii];
                            if (item.kadroDurum == ENUMKadroDurumu.YENI || item.kadroDurum == ENUMKadroDurumu.AKTIFYENI)
                            {
                                hata += string.Format("{0} adlı personelin hizmet belgesinde durumu yeni olan kayıt var, işleme alınamadı!", p.adSoyad);
                                return gunler;
                            }
                            if (item.kadroDurum == ENUMKadroDurumu.AYRILDIHESAPLAMA || item.kadroDurum == ENUMKadroDurumu.REDDEDILDI || item.kadroDurum == ENUMKadroDurumu.TANIMSIZ) continue;
                            if (!TNSDateTime.IsNull(item.ayrilmaTarihi) && (item.ayrilmaTarihiDT <= item.atamaTarihiDT))
                            {
                                hata += string.Format("{0} adlı personelin hizmet belgesinde, bitiş tarihi başlangıç tarihine eşit veya büyük hareket bulundu, işleme alınamadı!", p.adSoyad);
                                return gunler;
                            }
                            for (int jj = ii + 1; jj < p.kadrolar.ObjeSayisi; jj++)
                            {
                                PersonelKadro itemIc = (PersonelKadro)p.kadrolar.objeler[jj];
                                if (itemIc.kadroDurum == ENUMKadroDurumu.AYRILDIHESAPLAMA || itemIc.kadroDurum == ENUMKadroDurumu.REDDEDILDI || itemIc.kadroDurum == ENUMKadroDurumu.TANIMSIZ) continue;
                                DateTime basTar;
                                DateTime bitTar;
                                int gun = TNS.ResmiTatil.KesisimBul(itemIc.atamaTarihiDT, itemIc.ayrilmaTarihiDT, item.atamaTarihiDT, item.ayrilmaTarihiDT, isGunu, out basTar, out bitTar);
                                if (gun > 0)
                                {
                                    hata += string.Format("{0} adlı personelin hizmet belgesinde, tarihleri kesişen hareketler bulundu, işleme alınamadı!", p.adSoyad);
                                    return gunler;
                                }
                            }
                        }
                    }
                    catch
                    {
                        hata += string.Format("{0} adlı personelin hizmet belgesinde, tarihleri kesişen hareketler bulundu, işleme alınamadı!", p.adSoyad);
                        return gunler;
                    }
                    foreach (PersonelKadro item in p.kadrolar.objeler)
                    {
                        if (item.kadroDurum == ENUMKadroDurumu.AYRILDIHESAPLAMA || item.kadroDurum == ENUMKadroDurumu.REDDEDILDI || item.kadroDurum == ENUMKadroDurumu.TANIMSIZ || item.kadroDurum == ENUMKadroDurumu.YENI || item.kadroDurum == ENUMKadroDurumu.AKTIFYENI) continue;
                        if (!PersonelTipVarMi(item.personelTip, personelTipler)) continue;
                        if (item.hizmetSebebiTip == ENUMHizmetSebebiNevi.UCRETSIZIZIN) continue;
                        if (vekalet && (item.vekilUnvanKod == 0 || item.vekilUnvanKod == item.unvan)) continue;
                        DateTime basTar;
                        DateTime bitTar;
                        int gun = TNS.ResmiTatil.KesisimBul(bordroDonem.bordroDonemBasTarihDT, bordroDonem.sonGunDTEkle1, item.atamaTarihiDT, item.ayrilmaTarihiDT, isGunu, out basTar, out bitTar);
                        if (gun > 0)
                        {
                            List<IzinTarihAra> izinParcalar = new List<IzinTarihAra>();
                            Parcala(basTar, bitTar, isGunu, p.izin, izinParcalar);
                            foreach (IzinTarihAra izItem in izinParcalar)
                            {
                                gunler.toplamGun += izItem.gun;
                                if (!string.IsNullOrWhiteSpace(gunler.unvanGun)) gunler.unvanGun += ";";
                                gunler.unvanGun += item.personelTip.ToString() + "-" + izItem.gun + "-" + item.unvan.ToString() + "-" + ((int)item.hizmetSebebiTip).ToString() + "-" + item.vekilUnvanKod.ToString() + "-" + izItem.izinTur.ToString() + "-" + item.sinifi + "-" + item.kadroDerece + "-" + item.birimTuru + "-" + item.calistigiBirimTuru + "-" + item.odemeyeEsasDerece;
                            }
                        }
                    }
                }
            }
            return gunler;
        }

        private static string KidemGunuBul(Personel p, bool isGunu, CalisanDinamikBordroDonem bordroDonem)
        {
            int toplamGun;
            //OrtakFonksiyonlar.HataDosyaYaz("KidemYilBul.txt", $"KidemGunuBulBas:{p.adSoyad} {bordroDonem.bordroDonemBasTarih}, {p.izin.Length}", false);

            int k1 = KidemYilBul(p.kurumaBaslamaTarihi.Oku(), bordroDonem.bordroDonemBasTarihDT, p.izin, out toplamGun);

            int yil = toplamGun / 360;
            toplamGun -= yil * 360;
            int ay = toplamGun / 30;
            toplamGun -= ay * 30;
            //OrtakFonksiyonlar.HataDosyaYaz("KidemYilBul.txt", $"KidemGunuBulSon:{p.adSoyad} {yil}-{ay}-{toplamGun}", false);

            return $"{yil}y {ay}a {toplamGun}g";
        }

        private static string KidemBilgiBul(Personel p, bool isGunu, CalisanDinamikBordroDonem bordroDonem)
        {
            int toplamGun = GunBul(bordroDonem.bordroDonemBasTarihDT, bordroDonem.sonGunDT, true, false);
            int toplamKidemGunSil;
            int k1 = KidemYilBul(p.kurumaBaslamaTarihi.Oku(), bordroDonem.bordroDonemBasTarihDT, p.izin, out toplamKidemGunSil);
            int k2 = KidemYilBul(p.kurumaBaslamaTarihi.Oku(), bordroDonem.sonGunDT, p.izin, out toplamKidemGunSil);
            if (k1 == k2) return $"{toplamGun}-{k1}";

            DateTime dtBas = bordroDonem.bordroDonemBasTarihDT;
            DateTime dtBit = bordroDonem.sonGunDT;
            while (true)
            {
                int fark = (dtBit - dtBas).Days;
                if (fark == 1) break;
                DateTime dtOrta = dtBas.AddDays(fark / 2);
                int kOrta = KidemYilBul(p.kurumaBaslamaTarihi.Oku(), dtOrta, p.izin, out toplamKidemGunSil);
                if (kOrta == k1) dtBas = dtOrta;
                else dtBit = dtOrta;
            }
            int k1Gun = GunBul(bordroDonem.bordroDonemBasTarihDT, dtBas, true, false);
            return $"{k1Gun}-{k1};{toplamGun - k1Gun}-{k2}";
        }

        private static int KidemYilBul(DateTime kurumaBaslamaTarihi, DateTime hesapTarihDT, PersonelIzin[] izin, out int toplamGun)
        {
            int tplm = 0;
            foreach (PersonelIzin iz in izin)
            {
                //OrtakFonksiyonlar.HataDosyaYaz("KidemYilBul.txt", $"İzin1: {iz.izinTurKod}, {iz.izinTurAd}, {iz.izinTipAd}, {iz.baslangicTarihiDT} :: {iz.bitisTarihiDT}", false);
                if (iz.izinTurKod != 9) continue;
                if (iz.bitisTarihiDT <= kurumaBaslamaTarihi) continue;
                DateTime hesapBas;
                if (iz.baslangicTarihiDT <= kurumaBaslamaTarihi) hesapBas = kurumaBaslamaTarihi;
                else hesapBas = iz.baslangicTarihiDT;
                DateTime hesapBit;
                if (iz.bitisTarihiDT > hesapTarihDT) hesapBit = hesapTarihDT.AddDays(1);
                else hesapBit = iz.bitisTarihiDT;
                tplm += (hesapBit - hesapBas).Days;
                //OrtakFonksiyonlar.HataDosyaYaz("KidemYilBul.txt", $"İzin2: {iz.izinTurKod}, {iz.izinTurAd}, {iz.izinTipAd}, {iz.baslangicTarihiDT} :: {iz.bitisTarihiDT}", false);
            }
            toplamGun = TNS.IKY.Arac.GunHesapla360(kurumaBaslamaTarihi, hesapTarihDT) - tplm;
            //OrtakFonksiyonlar.HataDosyaYaz("KidemYilBul.txt", $"İzim3:{kurumaBaslamaTarihi} :: {hesapTarihDT} :: İzin={tplm} :: Yıl={(toplamGun) / 360}", false);
            return (toplamGun) / 360;
        }

        private static void BirimTuruDegistir(DateTime bordroBasTarih, Personel p)
        {
            DateTime BirimTuruDegisimTarihBas = new DateTime(2020, 02, 1);
            DateTime BirimTuruDegisimTarihSon = new DateTime(2020, 02, 4);

            if (bordroBasTarih < BirimTuruDegisimTarihBas)
            {
                for (int ii = 0; ii < p.kadrolar.ObjeSayisi; ii++)
                {
                    ItemDegistir((PersonelKadro)p.kadrolar.objeler[ii]);
                }
            }
            else if (bordroBasTarih == BirimTuruDegisimTarihBas)
            {
                ObjectArray oa = new ObjectArray();
                for (int ii = 0; ii < p.kadrolar.ObjeSayisi; ii++)
                {
                    PersonelKadro item = (PersonelKadro)p.kadrolar.objeler[ii];


                    if (item.atamaTarihiDT >= BirimTuruDegisimTarihSon)
                    {
                        oa.Ekle(item);
                    }
                    else if (ResmiTatil.TarihIcindeMi(BirimTuruDegisimTarihSon, item.atamaTarihiDT, item.ayrilmaTarihiDT))
                    {
                        PersonelKadro item1 = new PersonelKadro(item);
                        PersonelKadro item2 = new PersonelKadro(item);
                        item1.ayrilmaTarihi = new TNSDateTime(BirimTuruDegisimTarihSon);
                        item2.atamaTarihi = new TNSDateTime(BirimTuruDegisimTarihSon);
                        ItemDegistir(item1);
                        oa.Ekle(item1);
                        if (item2.atamaTarihiDT < item2.ayrilmaTarihiDT || item2.ayrilmaTarihiDT == DateTime.MinValue)
                            oa.Ekle(item2);
                    }
                    else
                    {
                        ItemDegistir(item);
                        oa.Ekle(item);
                    }
                }
                p.kadrolar = oa;
            }
        }

        private static void ItemDegistir(PersonelKadro item)
        {
            if (!string.IsNullOrWhiteSpace(item.calistigiBirimTuru))
            {
                if (item.calistigiBirimTuru.StartsWith("011"))
                {
                    item.calistigiBirimTuru = "0102";
                }
                else if (item.calistigiBirimTuru.StartsWith("012"))
                {
                    item.calistigiBirimTuru = "0104";
                }
            }
            if (!string.IsNullOrWhiteSpace(item.birimTuru))
            {
                if (item.birimTuru.StartsWith("011"))
                {
                    item.birimTuru = "0102";
                }
                else if (item.birimTuru.StartsWith("012"))
                {
                    item.birimTuru = "0104";
                }
            }
        }

        private static int KisiYakinlikBul(YakinlikTur tur, ObjectArray kisiYakinlik, CalisanDinamikBordroDonem bordroDonem)
        {
            int sonuc = 0;
            if (kisiYakinlik != null && kisiYakinlik.sonuc.islemSonuc)
            {
                foreach (TNS.IKY.PersonelAile item in kisiYakinlik.objeler)
                {
                    if (item.yakinlik == TNS.IKY.ENUMYakinlikDerecesi.Esi && tur == YakinlikTur.es)
                    {
                        if (item.bakmaklaYukumlu == 1)
                            return 1; //AGİ ve AY alır;
                        else if (item.durumKayitKod == "05.00006")
                            return 2; //AGİ alır;
                        else return 0;//AGİ ve AY alamaz;
                    }
                    else if (item.yakinlik == TNS.IKY.ENUMYakinlikDerecesi.Oglu || item.yakinlik == TNS.IKY.ENUMYakinlikDerecesi.Kizi)
                    {
                        int yas = PSLDinBordroHesaplayici.YasBul(item.dogumTarihi.Oku(), bordroDonem.bordroDonemBasTarihDT);
                        if (tur == YakinlikTur.bakmaklayukumlu0_6 || tur == YakinlikTur.bakmaklayukumlu0_6E)
                        {
                            if (item.bakmaklaYukumlu == 1 && yas < 6 && yas >= 0)
                            {
                                if (item.sakatlikDerecesi > 0 && tur == YakinlikTur.bakmaklayukumlu0_6E || tur == YakinlikTur.bakmaklayukumlu0_6) sonuc++;
                            }
                        }
                        else if (tur == YakinlikTur.bakmaklayukumlu6danbuyuk || tur == YakinlikTur.bakmaklayukumlu6danbuyukE)
                        {
                            if (yas >= 6 && item.bakmaklaYukumlu == 1)
                            {
                                if (!(item.durumKayitKod == "05.00003" || item.durumKayitKod == "05.00004")) //(Evli veya çalışıyor) değilse
                                {
                                    if (item.yakinlik == TNS.IKY.ENUMYakinlikDerecesi.Kizi)
                                    {
                                        if (item.sakatlikDerecesi > 0 && tur == YakinlikTur.bakmaklayukumlu6danbuyukE || tur == YakinlikTur.bakmaklayukumlu6danbuyuk) sonuc++;
                                    }
                                    else if (item.yakinlik == TNS.IKY.ENUMYakinlikDerecesi.Oglu)
                                    {
                                        if (yas < 25)
                                        {
                                            if (item.sakatlikDerecesi > 0 && tur == YakinlikTur.bakmaklayukumlu6danbuyukE || tur == YakinlikTur.bakmaklayukumlu6danbuyuk) sonuc++;
                                        }
                                    }
                                }
                            }
                        }
                        else if (tur == YakinlikTur.agi)
                        {
                            if (yas >= 0 && item.agi == 1 && !(item.durumKayitKod == "05.00003" || item.durumKayitKod == "05.00004")) //(Evli veya çalışıyor) değilse
                            {
                                if (yas < 18) sonuc++;
                                else if (yas < 25 && item.durumKayitKod == "05.00001") sonuc++;
                            }
                        }
                    }
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Lojamnın kira ve yakıt hesabı için m2'sini bulup döndüren fonksiyon.
        /// </summary>
        /// <param name="lojmanlar">Personelin lojman bilgilerini tutan dizi</param>
        /// <param name="kira">true ise kira, false ise yakıt</param>
        /// <returns></returns>
        private static int LojmanM2Bul(ObjectArray lojmanlar, bool kira)
        {
            int topm2 = 0;
            if (lojmanlar != null && lojmanlar.sonuc.islemSonuc)
            {
                foreach (PersonelLojmani item in lojmanlar.objeler)
                {
                    int m2 = item.daireMetreKare;
                    if (kira || (!kira && item.lojmanTipi != 1 && item.yakitKesilecek != 0))
                    {
                        if (m2 > 120) m2 = 120;
                        topm2 += m2;
                    }
                }
            }
            return topm2;
        }

        private static int YDilGostergeBul(PersonelDil[] personelDil, CalisanDinamikBordroDonem bordroDonem)
        {
            int topPuan = 0;
            if (personelDil != null && personelDil.Length > 0)
            {
                foreach (PersonelDil item in personelDil)
                {
                    if (item.bordroyaEsas == 1 && item.tarih != null && !item.tarih.isNull)
                    {
                        string dzy = DuzeyBul(item.duzey, item.tarih, bordroDonem);
                        topPuan += PuanBul(DuzeyBul(item.duzey, item.tarih, bordroDonem), item.kurumFaydalaniyor == 1, bordroDonem);
                    }
                }
            }
            return topPuan;
        }

        private static int PuanBul(string dzy, bool kurumFaydalaniyor, CalisanDinamikBordroDonem bordroDonem)
        {
            int ek = 0;
            if (bordroDonem == null || bordroDonem.bordroYapi == null)
            {
                ek = 100;
            }
            else if (bordroDonem.bordroDonemBasTarihDT == null)
            {
                if (bordroDonem.yil == 2025)
                {
                    if (bordroDonem.bordroYapi.bordroIslemTur != 1)
                        ek = 100;
                }
            }
            else if (bordroDonem.bordroDonemBasTarihDT.Year == 2025)
            {
                if (bordroDonem.bordroDonemBasTarihDT.Month == 12)
                {
                    if (bordroDonem.bordroYapi.bordroIslemTur != 1)
                        ek = 100;
                }
            }
            else if (bordroDonem.bordroDonemBasTarihDT.Year > 2025)
                ek = 100;
            if (kurumFaydalaniyor)
            {
                if (dzy == "A+") { return 1200 + ek; }
                else if (dzy == "A") { return 900 + ek; }
                else if (dzy == "B") { return 600 + ek; }
                else if (dzy == "C") { return 300 + ek; }
            }
            else
            {
                if (dzy == "A+") { return 750 + ek; }
                else if (dzy == "A") { return 750 + ek; }
                else if (dzy == "B") { return 500 + ek; }
                else if (dzy == "C") { return 250 + ek; }
            }
            return 0;
        }

        private static string DuzeyBul(string duzey, TNSDateTime duzeyTarih, CalisanDinamikBordroDonem bordroDonem)
        {
            int dilAy = PSLDinBordroHesaplayici.AyBul(duzeyTarih.Oku(), bordroDonem.bordroDonemBasTarihDT);
            if (dilAy < 0) return "";
            while (dilAy >= 60 && !string.IsNullOrWhiteSpace(duzey))
            {
                duzey = DuzeyDusur(duzey);
                dilAy -= 60;
            }
            return duzey;
        }

        private static ToplamGun GorevGunBul(ref ToplamGun gunler, Personel p, bool isGunu, bool p_2, CalisanDinamikBordroDonem bordroDonem)
        {
            if (gunler == null)
            {
                gunler = new ToplamGun();
                if (p.gorevler != null && p.gorevler.objeler != null)
                {
                    List<ToplamGun> hash = new List<ToplamGun>();
                    int ekle90 = 0;
                    int ekle90unv = 0;
                    foreach (Gorevler item in p.gorevler.objeler)
                    {
                        if (item.UcretliMi())
                        {
                            DateTime basTar;
                            DateTime bitTar;
                            if (TNSDateTime.IsNull(item.bitisTarihi))
                                item.bitisTarihi = new TNSDateTime(bordroDonem.sonGunDTEkle1);
                            int gun = TNS.ResmiTatil.KesisimBul(bordroDonem.bordroDonemBasTarihDT, bordroDonem.sonGunDTEkle1, item.baslamaTarihiDT, item.bitisTarihiDTEkle1, isGunu, out basTar, out bitTar);
                            if (gun > 0)
                            {
                                bool ekle = false;
                                if (item.gorevDurumu == Gorevler.ENUMGorevDurumu.UCRETLIVEKALET)
                                {
                                    if (item.baslamaTarihiDT < bordroDonem.bordroDonemBasTarihDT)
                                    {
                                        int buAydanOnceki = TNS.ResmiTatil.GunBul(item.baslamaTarihiDT, bordroDonem.bordroDonemBasTarihDT, false, false);
                                        int buAy = TNS.ResmiTatil.GunBul(item.baslamaTarihiDT, bordroDonem.sonGunDT, true, false);
                                        if (buAy >= 90)
                                        {
                                            ekle = true;
                                            if (buAydanOnceki < 90)
                                            {
                                                basTar = item.baslamaTarihiDT.AddDays(90);
                                                ekle90 = 90 - IzinDus(item.baslamaTarihiDT, basTar, p, isGunu);
                                                ekle90unv = item.unvanKod;
                                            }
                                        }
                                    }
                                }
                                else
                                    ekle = true;
                                if (ekle)
                                    GunEkle(item, basTar, bitTar, hash, isGunu);
                            }
                        }
                    }

                    foreach (ToplamGun item in hash)
                    {
                        if (item.bitTar > item.basTar)
                        {
                            List<IzinTarihAra> izinParcalar = new List<IzinTarihAra>();
                            Parcala(item.basTar, item.bitTar, isGunu, p.izin, izinParcalar);
                            foreach (IzinTarihAra izItem in izinParcalar)
                            {
                                gunler.toplamGun += izItem.gun;
                                if (!string.IsNullOrWhiteSpace(gunler.unvanGun)) gunler.unvanGun += ";";
                                gunler.unvanGun += "0-" + izItem.gun + "-" + item.unvanKod.ToString() + "-" + ((int)item.gorevDurumu).ToString() + "-" + "0" + "-" + izItem.izinTur.ToString();
                            }
                        }
                    }
                    if (ekle90 > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(gunler.unvanGun)) gunler.unvanGun += ";";
                        gunler.unvanGun += "1-" + ekle90 + "-" + ekle90unv.ToString() + "-" + ((int)Gorevler.ENUMGorevDurumu.UCRETLIVEKALET).ToString() + "-" + "0" + "-" + "0";
                    }
                }
            }
            return gunler;
        }

        private static PersonelKadro OncekiKadroBul(ref PersonelKadro oncekiKadro, Personel p, CalisanDinamikBordroDonem bordroDonem)
        {
            if (oncekiKadro == null)
            {
                oncekiKadro = p.kadro;
                if (p.kadrolar != null && p.kadrolar.objeler != null)
                {
                    foreach (PersonelKadro item in p.kadrolar.objeler)
                    {
                        if (TNS.ResmiTatil.TarihIcindeMi(bordroDonem.oncBordroDonemBasTarihDT, item.atamaTarihiDT, item.ayrilmaTarihiDT))
                        {
                            oncekiKadro = item;
                            break;
                        }
                    }
                }
            }
            return oncekiKadro;
        }

        private static int LojmanDaireTur(ObjectArray lojmanlar)
        {
            if (lojmanlar != null && lojmanlar.sonuc.islemSonuc)
            {
                foreach (PersonelLojmani item in lojmanlar.objeler)
                {
                    return item.daireTuru;
                }
            }
            return 0;
        }

        private static int KidemYiliBul(TNSDateTime kurumaBaslamaTarihi, DateTime islemTarihi)
        {
            if (TNSDateTime.IsNull(kurumaBaslamaTarihi))
                return 0;
            return TNS.IKY.Arac.GunHesapla360(kurumaBaslamaTarihi.Oku(), islemTarihi) / 360;
        }


        private static int KidemYiliBul(Hizmet hizmet)
        {
            int kidemYili = 0;
            if (hizmet != null)
            {
                kidemYili = hizmet.kidemYili;
            }
            return kidemYili;
        }

        private static bool PersonelTipVarMi(int personelTip, int[] personelTipler)
        {
            if (personelTipler == null || personelTipler.Length == 0) return true;
            for (int i = 0; i < personelTipler.Length; i++)
            {
                if (personelTipler[i] == personelTip) return true;
            }
            return false;
        }

        private static void Parcala(DateTime basTar, DateTime bitTar, bool isGunu, PersonelIzin[] personelIzin, List<IzinTarihAra> izinParcalar)
        {
            if (personelIzin != null)
            {
                for (int i = personelIzin.Length - 1; i >= 0; i--)
                {
                    PersonelIzin izin = personelIzin[i];
                    DateTime tatilBas;
                    DateTime tatilBit;
                    int tatilGun = TNS.ResmiTatil.KesisimBul(basTar, bitTar, izin.baslangicTarihi.Oku(), izin.bitisTarihi.Oku(), isGunu, out tatilBas, out tatilBit);
                    if (tatilGun > 0)
                    {
                        if (basTar < tatilBas)
                        {
                            izinParcalar.Add(new IzinTarihAra(GunBul(basTar, tatilBas, false, isGunu), 0));
                        }
                        else
                            tatilBas = basTar;

                        if (tatilBit > bitTar)
                            tatilBit = bitTar;
                        izinParcalar.Add(new IzinTarihAra(GunBul(tatilBas, tatilBit, false, isGunu), izin.izinTurKod));
                        basTar = tatilBit;
                    }
                    if (basTar > bitTar) break;
                }
            }
            if (basTar < bitTar)
                izinParcalar.Add(new IzinTarihAra(GunBul(basTar, bitTar, false, isGunu), 0));
        }

        public static int YasBul(DateTime dogumTarih, DateTime simdikiTarih)
        {
            TNS.TarihFark dd = new TNS.TarihFark(dogumTarih, simdikiTarih);
            return dd.Yil;
        }

        public static int AyBul(DateTime dogumTarih, DateTime simdikiTarih)
        {
            TNS.TarihFark dd = new TNS.TarihFark(dogumTarih, simdikiTarih);
            return dd.YilAy;
        }

        public static int GunBul(DateTime basTar, DateTime bitTar, bool sonGunDahil, bool isGunu)
        {
            return TNS.ResmiTatil.GunBul(basTar, bitTar, sonGunDahil, isGunu);
        }

        private static string DuzeyDusur(string duzey)
        {
            if (duzey == "A+" || duzey == "A") return "B";
            else if (duzey == "B") return "C";
            return "";
        }

        private static int IzinDus(DateTime basTar, DateTime bitTar, Personel p, bool isGunu)
        {
            int toplam = 0;
            PersonelIzin[] personelIzin = p.izin;
            if (personelIzin != null)
            {
                for (int i = personelIzin.Length - 1; i >= 0; i--)
                {
                    PersonelIzin izin = personelIzin[i];
                    DateTime tatilBas;
                    DateTime tatilBit;
                    toplam += TNS.ResmiTatil.KesisimBul(basTar, bitTar, izin.baslangicTarihi.Oku(), izin.bitisTarihi.Oku(), isGunu, out tatilBas, out tatilBit);
                }
            }
            return toplam;
        }
        public static KisiSatir EskiDegerBul(CalisanDinamikBordro calisanBordro, string tcKimlik)
        {
            KisiSatir bulduk = null;
            int buldukYer = -1;
            if (calisanBordro != null && calisanBordro.kisiDegerler != null)
            {
                for (int i = 0; i < calisanBordro.kisiDegerler.Count; i++)
                {
                    KisiSatir kisiSatir = (KisiSatir)calisanBordro.kisiDegerler[i];
                    object[] item = kisiSatir.degerler;
                    string tcKimlikEski = (string)item[0];
                    if (tcKimlikEski == tcKimlik)
                    {
                        bulduk = kisiSatir;
                        buldukYer = i;
                        break;
                    }
                    else if (tcKimlikEski.CompareTo(tcKimlik) > 0)
                    {
                        break;
                    }
                }
            }
            if (buldukYer > -1)
                calisanBordro.kisiDegerler.RemoveAt(buldukYer);
            return bulduk;
        }

        public static Calculator BordroDerleyicisiOl(BordroYapi by, out string hata, string ortakKod)
        {
            TNS.Calculator c = new Calculator();

            List<Variable> alv = new List<Variable>();
            List<Formula> alf = new List<Formula>();
            for (int i = 0; i < by.hepsi.Length; i++)
            {
                DegiskenFormul item = by.hepsi[i];
                if (string.IsNullOrEmpty(item.formula))
                {
                    alv.Add(new Variable(item.ad, item.tip));
                }
                else
                {
                    alf.Add(new Formula(item.ad, item.tip, item.formula));
                }
            }

            hata = c.PrepareCalculator("Ornek", "Bordro", ":HRCServisI.BordroTemel", ortakKod, alv.ToArray(), alf.ToArray(), new string[] { AppDomain.CurrentDomain.BaseDirectory + @"bin\HRCServisI.dll", AppDomain.CurrentDomain.BaseDirectory + @"bin\GNLServis.dll" });
            return c;
        }

        public static string OrtakFonksiyonlarOku(TNS.KYM.Kullanici kullanan)
        {
            return TNS.KYM.Arac.Tanimla().BuyukMetinOku(kullanan, "HRC", "OrtakFonksiyon", "tr");
        }

        public static void BordroVerileriHesaplaGenel(Kullanici kullanan, IHRCServis servisHRC, BordroYapi by, CalisanDinamikBordroDonem bordroDonem, CalisanDinamikBordro calisanBordro, string surecNo, bool isGunu)
        {
            string hata = "";
            TNS.Calculator c = BordroDerleyicisiOl(by, out hata, OrtakFonksiyonlarOku(kullanan));
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);
            else
                BordroVerileriHesaplaGenelCompiler(c, kullanan, servisHRC, by, bordroDonem, calisanBordro, surecNo, isGunu);
        }

        public static void BordroVerileriHesaplaGenelCompiler(TNS.Calculator c, Kullanici kullanan, IHRCServis servisHRC, BordroYapi by, CalisanDinamikBordroDonem bordroDonem, CalisanDinamikBordro calisanBordro, string surecNo, bool isGunu)
        {

            TNS.HRC.PSLGelirVergisi kriter = new PSLGelirVergisi();
            kriter.yil = bordroDonem.odemeTarihDT.Year;
            ObjectArray gelirVerigisOranlari = servisHRC.PSLGelirVergisiListele(null, kriter);

            TNS.HRC.MerkezBankasiKurSinif kriterKB = new TNS.HRC.MerkezBankasiKurSinif();
            kriterKB.kurTarih = bordroDonem.hazirlamaTarih;
            TNS.HRC.MerkezBankasiKurSinif gunlukKur = servisHRC.MerkezBankKurOku(kullanan, kriterKB);

            int mtbYil = bordroDonem.bordroDonemBasTarihDT.Year;
            PSLMaasTemelBilgisi maasTemelBilgisi = new PSLMaasTemelBilgisi();
            maasTemelBilgisi.yil = mtbYil;
            maasTemelBilgisi.ay = 0;
            ObjectArray temelBilgiler = servisHRC.PSLMaasTemelBilgisiListele(null, maasTemelBilgisi);
            //int BordroAy = bordroDonem.bordroDonemBasTarihDT.Month;
            //if (BordroAy == 12)
            {
                maasTemelBilgisi.yil = mtbYil + 1;
                maasTemelBilgisi.ay = 1;
                ObjectArray temelBilgiler2 = servisHRC.PSLMaasTemelBilgisiListele(null, maasTemelBilgisi);
                if (temelBilgiler2.ObjeSayisi == 1)
                {
                    TNS.HRC.PSLMaasTemelBilgisi f = (TNS.HRC.PSLMaasTemelBilgisi)temelBilgiler2.objeler[0];
                    f.yil = mtbYil;
                    f.ay = 13;
                    temelBilgiler.objeler.Add(f);
                }
            }
            //if (BordroAy == 1)
            {
                maasTemelBilgisi.yil = mtbYil - 1;
                maasTemelBilgisi.ay = 12;
                ObjectArray temelBilgiler2 = servisHRC.PSLMaasTemelBilgisiListele(null, maasTemelBilgisi);
                if (temelBilgiler2.ObjeSayisi == 1)
                {
                    TNS.HRC.PSLMaasTemelBilgisi f = (TNS.HRC.PSLMaasTemelBilgisi)temelBilgiler2.objeler[0];
                    f.yil = mtbYil;
                    f.ay = 0;
                    temelBilgiler.objeler.Add(f);
                }
            }
            Hashtable tabloDeger = new Hashtable();
            if (!string.IsNullOrWhiteSpace(surecNo))
                OrtakFonksiyonlar.HataDosyaYaz("BordroHesap" + surecNo.ToString() + ".txt", "Başlıyor", true);
            foreach (KisiSatir kisiSatir in calisanBordro.kisiDegerler)
            {
                object[] row = kisiSatir.degerler;
                if (!string.IsNullOrWhiteSpace(surecNo))
                    OrtakFonksiyonlar.HataDosyaYaz("BordroHesap" + surecNo.ToString() + ".txt", (string)row[0], false);
                Hashtable htIn = new Hashtable();
                string tcKimlikNo = (string)row[0];
                htIn["tcKimlikNo"] = tcKimlikNo;
                htIn["hakedisBasTar"] = bordroDonem.bordroDonemBasTarihDT;
                htIn["hakedisSonTar"] = bordroDonem.sonGunDT;
                htIn["hakedisGun"] = GunBul(bordroDonem.bordroDonemBasTarihDT, bordroDonem.sonGunDT, true, isGunu);
                htIn["islemTur"] = by.bordroIslemTur;

                htIn["GelirVergisiOranlari"] = gelirVerigisOranlari;
                htIn["MaasTemelBilgi"] = temelBilgiler;
                htIn["tabloDeger"] = tabloDeger;
                htIn["servis"] = servisHRC;
                htIn["gunlukKur"] = gunlukKur;
                Hashtable htOut = new Hashtable();
                TNSCollection collection = new TNSCollection();
                for (int i = 0; i < by.hepsi.Length; i++)
                {
                    DegiskenFormul item = by.hepsi[i];
                    if (string.IsNullOrEmpty(item.formula))
                    {
                        htIn[item.ad] = row[i + 1];
                    }
                    else
                        htOut[item.ad] = null;
                }
                try
                {
                    c.ExecuteCalc(htIn, htOut);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                for (int i = 0; i < by.hepsi.Length; i++)
                {
                    DegiskenFormul item = by.hepsi[i];

                    try
                    {
                        if (!string.IsNullOrEmpty(item.formula))
                        {
                            row[i + 1] = htOut[item.ad];
                        }
                    }
                    catch { }
                }
            }
        }

    }

    public class BordroTemel
    {
        public int yil { get { return hakedisBasTar.Year; } }
        public int ay { get { return hakedisBasTar.Month; } }
        public string tcKimlikNo { get; set; }
        public ObjectArray GelirVergisiOranlari { get; set; }
        public ObjectArray MaasTemelBilgi { get; set; }
        public Hashtable tabloDeger { get; set; }
        public IHRCServis servis { get; set; }
        public DateTime hakedisBasTar { get; set; }
        public DateTime hakedisSonTar { get; set; }
        public int hakedisGun { get; set; }
        public int islemTur { get; set; }

        public TNS.HRC.MerkezBankasiKurSinif gunlukKur { get; set; }
        public ObjectArray oncBordroVeriler = null;
        public double OdenenBordroDegerBulDbl(string degiskenAd)
        {
            if (oncBordroVeriler == null)
            {
                TNS.HRC.BordroKistas bordroKistas = new TNS.HRC.BordroKistas(0, null, null);
                bordroKistas.verilenTarih = new TNSDateTime(hakedisBasTar);
                bordroKistas.bordroIslem = 1;
                oncBordroVeriler = servis.CalisanDinamikBordroListele(null, bordroKistas);
            }

            //new TNSCollection(tcKimlikNo)
            foreach (TNS.HRC.CalisanDinamikBordroDonem donem in oncBordroVeriler.objeler)
            {
                CalisanDinamikBordro cdb = donem.calisanDinamikBordro;
                for (int kisiNo = 0; kisiNo < cdb.kisiDegerler.Count; kisiNo++)
                {
                    KisiSatir ks = (KisiSatir)donem.calisanDinamikBordro.kisiDegerler[kisiNo];
                    if ((string)(ks.degerler[0]) == tcKimlikNo)
                    {
                        object o = TNS.HRC.CalisanDinamikBordro.DegerBul(donem.bordroYapi, ks, degiskenAd);
                        if (o == null) return 0;
                        return (double)o;
                    }
                }
            }
            return 0;
        }

        public int OdenenBordroDegerBulInt(string degiskenAd)
        {
            if (oncBordroVeriler == null)
            {
                TNS.HRC.BordroKistas bordroKistas = new TNS.HRC.BordroKistas(0, null, null);
                bordroKistas.verilenTarih = new TNSDateTime(hakedisBasTar);
                bordroKistas.bordroIslem = 1;
                oncBordroVeriler = servis.CalisanDinamikBordroListele(null, bordroKistas);
            }

            //new TNSCollection(tcKimlikNo)
            foreach (TNS.HRC.CalisanDinamikBordroDonem donem in oncBordroVeriler.objeler)
            {
                CalisanDinamikBordro cdb = donem.calisanDinamikBordro;
                for (int kisiNo = 0; kisiNo < cdb.kisiDegerler.Count; kisiNo++)
                {
                    KisiSatir ks = (KisiSatir)donem.calisanDinamikBordro.kisiDegerler[kisiNo];
                    if ((string)(ks.degerler[0]) == tcKimlikNo)
                    {
                        object o = TNS.HRC.CalisanDinamikBordro.DegerBul(donem.bordroYapi, ks, degiskenAd);
                        if (o == null) return 0;
                        return (int)o;
                    }
                }
            }
            return 0;
        }

        public BordroTemel()
        {
        }

        public void BilgiYaz(string bilgi)
        {
            OrtakFonksiyonlar.HataDosyaYaz("BRDBilgi_" + yil.ToString("0000") + "_" + ay.ToString("00") + "_" + islemTur.ToString()+".txt", bilgi, false);
        }

        public double Kur(string paraBirimiKod, string kurTur)
        {
            if (gunlukKur == null) return 1;
            return Convert.ToDouble(gunlukKur.KurBul(paraBirimiKod, kurTur, 1, 10));
        }

        private double GelirVergisiHesapla(double d_matrah)
        {
            decimal matrah = Convert.ToDecimal(d_matrah);
            decimal donusDeger = 0;
            if (matrah > 0)
            {
                decimal taban = 0;
                foreach (PSLGelirVergisi item in GelirVergisiOranlari.objeler)
                {
                    if (matrah > taban && matrah <= item.miktar || item.miktar == 0)
                    {
                        donusDeger += OrtakFonksiyonlar.YuzdeHesapla(matrah - taban, item.yuzde);
                        break;
                    }
                    else
                    {
                        donusDeger += OrtakFonksiyonlar.YuzdeHesapla(item.miktar - taban, item.yuzde);
                        taban = item.miktar;
                    }
                }
            }
            return Convert.ToDouble(donusDeger);
        }

        public double GelirVergisiHesapla(double d_oncMatrah, double d_buAy)
        {
            return TNS.HRC.Arac.GelirVergisiHesapla(d_oncMatrah, d_buAy, GelirVergisiOranlari);
        }

        public static int GunBul360(DateTime kucuk, DateTime buyuk)
        {
            return TNS.IKY.Arac.GunHesapla360(kucuk, buyuk);
        }

        public static int GunBul(DateTime kucuk, DateTime buyuk, bool sonGunDahil, bool isGunu)
        {
            return TNS.ResmiTatil.GunBul(kucuk, buyuk, sonGunDahil, isGunu);
        }

        public int GunBul(DateTime kucuk, DateTime buyuk, bool sonGunDahil)
        {
            return TNS.ResmiTatil.GunBul(kucuk, buyuk, sonGunDahil, hakedisGun < 28);
        }

        private PSLMaasTemelBilgisi MaasTemelBilgiBul(int ayIc)
        {
            foreach (PSLMaasTemelBilgisi item in MaasTemelBilgi.objeler)
            {
                if (item.ay == ayIc) return item;
            }
            return null;
        }

        public double GenelMaasParamAy(string kod, int ayIc)
        {
            PSLMaasTemelBilgisi maasTemelBilgi = MaasTemelBilgiBul(ayIc);
            if (maasTemelBilgi != null)
            {
                if (kod == "tabanAylikKatsayi") return Convert.ToDouble(maasTemelBilgi.tabanAylikKatsayi);
                if (kod == "yanOdemeKatsayi") return Convert.ToDouble(maasTemelBilgi.yanOdemeKatsayi);
                if (kod == "enYuksekGosterge") return Convert.ToDouble(maasTemelBilgi.enYuksMemurMaasGost);
                if (kod == "aylikKatsayi") return Convert.ToDouble(maasTemelBilgi.aylikKatsayi);
                if (kod == "gelirVergisiOran") return Convert.ToDouble(maasTemelBilgi.gelirVergisiOran);
                if (kod == "sskIsverenPayiOran") return Convert.ToDouble(maasTemelBilgi.sskIsverenPayiOran);
                if (kod == "sskIsciPayiOran") return Convert.ToDouble(maasTemelBilgi.sskIsciPayiOran);
                if (kod == "damgaVergisiOrani") return Convert.ToDouble(maasTemelBilgi.damgaVergisiOran);
                if (kod == "brutAsgariUcret") return Convert.ToDouble(maasTemelBilgi.brutAsgariUcret);

                if (kod == "isKazasiMeslekHast") return Convert.ToDouble(maasTemelBilgi.isKazasiMeslekHast);
                if (kod == "analikSigortasi") return Convert.ToDouble(maasTemelBilgi.analikSigortasi);
                if (kod == "hastalikSigortasi") return Convert.ToDouble(maasTemelBilgi.hastalikSigortasi);
                if (kod == "malulYasliOlumSigortasi") return Convert.ToDouble(maasTemelBilgi.malulYasliOlumSigortasi);

                if (kod == "sskIssizSigIsVerenPayiOran") return Convert.ToDouble(maasTemelBilgi.sskIssizSigIsVerenPayiOran);
                if (kod == "sskIssizSigIsciPayiOran") return Convert.ToDouble(maasTemelBilgi.sskIssizSigIsciPayiOran);

                if (kod == "netAsgariUcret") return Convert.ToDouble(maasTemelBilgi.netAsgariUcret);
                if (kod == "sakatlikIndirimi1") return Convert.ToDouble(maasTemelBilgi.sakatlikIndirimi1);
                if (kod == "sakatlikIndirimi2") return Convert.ToDouble(maasTemelBilgi.sakatlikIndirimi2);
                if (kod == "sakatlikIndirimi3") return Convert.ToDouble(maasTemelBilgi.sakatlikIndirimi3);
                if (kod == "kira1") return Convert.ToDouble(maasTemelBilgi.kira1);
                if (kod == "yakit1") return Convert.ToDouble(maasTemelBilgi.yakit1);
                if (kod == "sskTavan") return Convert.ToDouble(maasTemelBilgi.sskTavan);
                if (kod == "topluGorusmeSabiti") return Convert.ToDouble(maasTemelBilgi.topluGorusmeSabiti);
                if (kod == "aileYardimiKatsayisi") return Convert.ToDouble(maasTemelBilgi.aileYardimiKatsayisi);
            }
            return 0;
        }

        public double GenelMaasParam(string kod)
        {
            return GenelMaasParamAy(kod, ay);
        }

        public int GostergeBul(int derece, int kademe)
        {
            return TNS.IKY.Arac.GostergeBul(derece, kademe);
        }

        public double TabloDeger(int tabloKod, int unvanKod, int derece)
        {
            return TabloDegerYil(tabloKod, unvanKod, derece, 0);
        }

        public double TabloDegerYil(int tabloKod, int unvanKod, int derece, int yil)
        {
            string anahtar = tabloKod + "_" + unvanKod + "_" + derece + "_" + yil;
            object donDeger = tabloDeger[anahtar];
            if (donDeger == null)
            {
                donDeger = Convert.ToDouble(servis.OkuBordroTablo(null, tabloKod, unvanKod, derece, yil));
                tabloDeger[anahtar] = donDeger;
            }
            return (double)donDeger;
        }

        public static double KurusYap(double deger)
        {
            return Yuvarla(deger, 2);
        }

        public static double Yuvarla(double deger, int noktaSay)
        {
            return OrtakFonksiyonlar.Yuvarla(deger, noktaSay);
        }

        public static double YuvarlaAsagi(double deger)
        {
            return OrtakFonksiyonlar.YuvarlaAsagi(deger);
        }

        public static double YuvarlaYukari(double deger)
        {
            return OrtakFonksiyonlar.YuvarlaYukari(deger);
        }
    }
}

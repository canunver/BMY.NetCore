using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using TNS.ORG;
using System.Collections.Generic;
using System.Collections;

namespace TNS.IKY.IsKurallari
{
    public class IKYPersonelKadro
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriIKY";

        public static ObjectArray ListeleKadro(string okunanXML)
        {
            ObjectArray pVAN = new ObjectArray();

            int kadroAlanlarSonIndex = (Enum.GetNames(typeof(TABPersonelKadro.ENUMALANLAR))).Length;

            IDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                Doldur(pVAN.objeler, r);
            }
            return pVAN;
        }

        public static ObjectArray ListeleKadro(VeriErisim veriErisim, Kullanici kullanan, PersonelKadro p, bool tarihArtan, bool icCagri)
        {
            ObjectArray pVAN = new ObjectArray();
            if (!icCagri)
            {
                IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, p.personelKod, pVAN.sonuc, ENUMYetkiIslem.Hizmetler);
                if (!pVAN.sonuc.islemSonuc)
                {
                    return pVAN;
                }
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelKadro tab = new TABPersonelKadro(ve);

            IDataReader r = tab.ListeleKadro(p, tarihArtan, false);
            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = ve.hata;

                return pVAN;
            }
            Doldur(pVAN.objeler, r);

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            else
            {
                if (!icCagri)
                {
                    IKYPersonelTarihce.Kaydet(ve, kullanan, new PersonelKadro(), p, ENUMTARIHCE.KADRO, p.personelKod, ENUMTARIHCEISLEMTIP.LISTELE);
                }
            }
            ve.VeriErisimKapat();
            return pVAN;
        }

        private static void Doldur(TNSCollection pVAN, IDataReader r)
        {
            int kadroAlanlarSonIndex = (Enum.GetNames(typeof(TABPersonelKadro.ENUMALANLAR))).Length;
            PersonelKadro oncekiD = null;
            while (r.Read())
            {
                PersonelKadro d = new PersonelKadro();
                ReaderdanAl(d, r, 0, false);

                d.unvanAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex, "");
                d.sinifi = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 1, 0);
                d.personelTipAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 2, "");

                d.hizmetSebebiAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 3, "");

                d.sinifAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 4, "");
                d.personelTCKimlikNo = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 5, "");
                d.personelAdSoyad = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 6, "");
                d.sinifKisaAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 7, "");
                if (d.hizmetGunSayisi <= 0)
                    d.hizmetGunSayisi = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 8, 0);
                d.birimAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 9, "");
                d.vekilUnvanAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 10, "");
                d.hizmetSebebiTip = (ENUMHizmetSebebiNevi)VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 11, 0);
                d.ayrilmaGerekcesiTip = (ENUMHizmetSebebiNevi)VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 12, 0);
                d.ayrilmaGerekcesiAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 13, "");
                d.hizmetHesabi = (ENUMHizmetTablolari)VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 14, 0);
                d.gorevBirimAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 15, "");
                d.birimKisaAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 16, "");
                d.kurumDisi = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 17, 0);
                d.tayinOlacagiBirimAd = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 18, "");
                d.birimTuru = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 19, "");
                d.calistigiBirimTuru = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + 20, "");
                GirisSebebDoldur(oncekiD, d);
                oncekiD = d;
                pVAN.Add(d);
            }
            r.Close();
        }

        private static void GirisSebebDoldur(PersonelKadro oncekiD, PersonelKadro d)
        {
            if (oncekiD == null) return;
            if (oncekiD.personelKod != d.personelKod) return;
            if (oncekiD.atamaTarihiDT < d.atamaTarihiDT)
                GirisSebebDoldurKB(oncekiD, d);
            else
                GirisSebebDoldurKB(d, oncekiD);
        }

        private static void GirisSebebDoldurKB(PersonelKadro onceki, PersonelKadro sonraki)
        {
            if (onceki.ayrilmaTarihiDT == sonraki.atamaTarihiDT)
            {
                if (sonraki.hizmetSebebiKod <= 0)
                {
                    sonraki.hizmetSebebiKod = onceki.ayrilmaGerekcesi;
                    sonraki.hizmetSebebiAd = onceki.ayrilmaGerekcesiAd;
                }
                if (string.IsNullOrWhiteSpace(sonraki.hizmetSebebi)) sonraki.hizmetSebebi = onceki.ayrilmaNedeni;
            }
        }

        public static void ReaderdanAl(PersonelKadro d, IDataReader r, int baslaKolon, bool ozet)
        {
            d.kod = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KOD, 0);
            d.personelKod = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.PKOD, 0);
            d.organizasyonKodu = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ORGANIZASYONKOD, 0);
            d.unvan = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.UNVANKOD, 0);
            d.gorevYeri = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.GOREVYERI, "");
            d.vekilUnvanKod = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.GorunenUnvanKodu, 0);
            d.personelTip = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.PERSONELTUR, 0);
            d.gorevOrganizasyonKodu = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.GOREVORGANIZASYONKOD, 0);
            d.kadroDerece = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KADRODERECE, 0);
            d.kadroTarih = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KADROTARIH);
            d.tayinOlacagiBirim = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.TAYINOLACAGIBIRIMKOD, 0);
            d.meslekKodu = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.MESLEKKODU, "");

            if (!ozet)
            {
                d.evrakNo = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.EVRAKNO, "");
                d.yasalDayanak = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.YASALDAYANAK, "");
                d.onayTarih = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ONAYTARIH);
                d.aciklama = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ACIKLAMA, "");

                d.odemeyeEsasDerece = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ODEMEYEESASDERECE, 0);
                d.odemeyeEsasKademe = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ODEMEYEESASKADEME, 0);
                d.odemeyeEsasTarih = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ODEMEYEESASTARIH);
                d.odemeyeEsasGosterge = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ODEMEYEESASGOSTERGE, 0);
                d.odemeyeEsasEkGosterge = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ODEMEYEESASEKGOSTERGE, 0);

                d.kazanilmisHakDerece = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KAZANILMISHAKDERECE, 0);
                d.kazanilmisHakKademe = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KAZANILMISHAKKADEME, 0);
                d.kazanilmisHakTarih = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KAZANILMISHAKTARIH);
                d.kazanilmisGosterge = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KAZANILMISGOSTERGE, 0);
                d.kazanilmisEkGosterge = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KAZANILMISEKGOSTERGE, 0);

                d.emekliyeEsasDerece = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.EMEKLIYEESASDERECE, 0);
                d.emekliyeEsasKademe = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.EMEKLIYEESASKADEME, 0);
                d.emekliyeEsasTarih = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.EMEKLIYEESASTARIH);
                d.emekliyeEsasGosterge = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.EMEKLIYEESASGOSTERGE, 0);
                d.emekliyeEsasEkGosterge = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.EMEKLIYEESASEKGOSTERGE, 0);

                d.kadroDurum = (ENUMKadroDurumu)VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KADRODURUMU, 0);
                d.hizmetSebebi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HIZMETSEBEBI, "");
                d.hizmetSebebiKod = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HIZMETSEBEBIKOD, 0);
                d.belgeCinsi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.BELGECINSI, "");
                d.belgeNo = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.BELGENO, "");
                d.belgeTarihi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.BELGETARIHI);
                d.ayrilmaTarihi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.AYRILMATARIHI);
                d.ayrilmaNedeni = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.AYRILMANEDENI, "");
                d.eskiKod = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.EskiKadroKod, 0);
                d.atamaTarihi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.AtamaTarihi);
                d.onaylayan = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.Onaylayan, "");
                d.eskiHizmetTipiFlag = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.EskiHizmetFlag, 0);
                d.eskiHizmetTipi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.EskiHizmetTipi, 0);
                d.hizmetGunSayisi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HizmetGunSayisi, 0);
                d.ayrilmaGerekcesi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.AYRILMAGEREKCESI, 0);
                d.KHADurumu = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KHADurum, 0);
                //d.yedekArtis = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.YedekArtis, 0);
                d.muhasebeBirimi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.MuhasebeBirimi, "");
                d.muhasebeKodu = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.MuhasebeKodu, 0);
                d.kurumsalKod = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.KurumsalKod, "");
                d.iliskiliKadroIslemNo = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ILISKILIKADROISLEMNO, 0);
                d.atananacagiKadroUnvanKod = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ATANACAGIKADROUNVANKOD, 0);
                d.atananacagiKadroDerece = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.ATANACAGIKADRODERECE, 0);

                d.hitapKod = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HITAPKOD, 0);
                d.hitapkhaDurum = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HITAPKHADURUM, "");
                d.hitapsgkNevi = (ENUMSGKNevi)VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HITAPSGKNEVI, 0);
                d.hitapSicilNo = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HITAPSICILNO, "");
                d.hitapBorcNevi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HITAPBORCNEVI, "");
                d.hitaptoplamTutar = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HITAPTOPLAMTUTAR, 0.0);
                d.hitapodenenMiktar = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HITAPODENENMIKTAR, 0.0);
                d.hitapodemeTarihi = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonelKadro.ENUMALANLAR.HITAPODEMETARIHI);
            }
        }

        public static OrganizasyonBirim ORGUstHarcamaBirimBul(OrganizasyonBirim birim)
        {
            if (birim == null) return birim;
            if (birim.harcamaMi == 1)
                return birim;
            else if (birim.anaBirimKod == 0)
                return null;
            return ORGUstHarcamaBirimBul(birim.anaBirimKod);
        }

        public static OrganizasyonBirim ORGUstHarcamaBirimBul(int organizasyonKod)
        {
            OrganizasyonBirim birim = TNS.ORG.Arac.OrgBirimBul(organizasyonKod);

            if (birim == null) return birim;
            if (birim.harcamaMi == 1)
                return birim;
            else if (birim.anaBirimKod == 0)
                return null;
            return ORGUstHarcamaBirimBul(birim.anaBirimKod);
        }

        public static OrganizasyonBirim HarcamaBul(int birimKod)
        {
            return ORGUstHarcamaBirimBul(birimKod);
        }

        //private static void LogYaz(string p)
        //{
        //    OrtakFonksiyonlar.HataDosyaYaz("AAPersonelKayit.txt", p + "\t" + DateTime.Now.Ticks.ToString(), false);
        //}

        public static Sonuc KaydetKadro(VeriErisim veriErisim, Kullanici kullanan, PersonelKadro p, bool yetkiKontroluYapma)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için
            ObjectArray eskiTmp; //Tarihçe için

            if (!yetkiKontroluYapma)
            {
                sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Hizmetler);
                if (!sonuc.islemSonuc)
                {
                    return sonuc;
                }
            }
            if (p.personelKod == 0)
                sonuc.DegerAta("Personel bilgisi boş bırakılamaz!");

            if (p.atamaTarihi == null || p.atamaTarihi.isNull)
                sonuc.DegerAta("Atama tarihi boş bırakılamaz!");

            //if (p.hizmetKurumu == 0)
            //    sonuc.DegerAta("Hizmet kurumu boş bırakılamaz!");
            PersonelTipi personelTipi = null;
            if (p.personelTip == 0)
                sonuc.DegerAta("Personel türü boş bırakılamaz!");
            else
            {
                personelTipi = TNS.IKY.Arac.PersonelTipBul(p.personelTip);
                if (personelTipi == null)
                    sonuc.DegerAta("Seçtiğiniz persone tipi (" + p.personelTip.ToString() + ") bulunamadı. Geçerli bir personel tipi seçiniz!");
            }

            if (personelTipi == null)
                personelTipi = new PersonelTipi();

            if (personelTipi.HITAP == ENUMHizmetTablolari.EmekliSandigi && p.hizmetSebebiKod == 0)
                sonuc.DegerAta("Başlama sebebi boş bırakılamaz!");

            OrganizasyonBirim orgBirim = null;
            OrganizasyonBirim hBirimi = null;
            if (p.gorevOrganizasyonKodu <= 0) p.gorevOrganizasyonKodu = p.organizasyonKodu;

            if (personelTipi.BizimKurumMu())
            {
                if (p.organizasyonKodu <= 0)
                    sonuc.DegerAta("Birimi boş bırakılamaz!");
                else
                {
                    orgBirim = TNS.ORG.Arac.OrgBirimBul(p.organizasyonKodu);
                    if (string.IsNullOrWhiteSpace(orgBirim.adi))
                        sonuc.DegerAta("Kaydedilmek istenilen birim (" + p.organizasyonKodu + ") bulunamadı!");
                    else
                    {
                        hBirimi = ORGUstHarcamaBirimBul(orgBirim);
                        if (hBirimi == null)
                        {
                            sonuc.DegerAta("Birime ait harcama birimi bulunamadı!");
                        }
                        else
                        {
                            p.muhasebeBirimi = hBirimi.muhasebebirim;
                            p.muhasebeKodu = OrtakFonksiyonlar.ConvertToInt(hBirimi.muhasebekodu, 0);
                            p.kurumsalKod = hBirimi.kurumsalkod;
                        }
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(p.gorevYeri))
                {
                    sonuc.DegerAta("Görev yeri boş bırakılamaz!");
                }
            }
            if (personelTipi.HITAP == ENUMHizmetTablolari.EmekliSandigi && p.unvan == 0)
                sonuc.DegerAta("Unvanı boş bırakılamaz.!");

            if (personelTipi.KadroluMu())
            {
                if (p.kadroDerece <= 0)
                    sonuc.DegerAta("Kadro derecesi boş bırakılamaz!");
                if (p.kadroTarih == null || p.kadroTarih.isNull)
                    sonuc.DegerAta("Kadroyu alış tarihi boş bırakılamaz!");

                DereceKademeKontrolu(sonuc, "Kazanılmış Hak", p.kazanilmisEkGosterge, p.kazanilmisGosterge, p.kazanilmisHakDerece, p.kazanilmisHakKademe, p.kazanilmisHakTarih);
                DereceKademeKontrolu(sonuc, "Ödemeye Esas", p.odemeyeEsasEkGosterge, p.odemeyeEsasGosterge, p.odemeyeEsasDerece, p.odemeyeEsasKademe, p.odemeyeEsasTarih);
                DereceKademeKontrolu(sonuc, "Emekliye Esas", p.emekliyeEsasEkGosterge, p.emekliyeEsasGosterge, p.emekliyeEsasDerece, p.emekliyeEsasKademe, p.emekliyeEsasTarih);
            }

            if (p.ayrilmaGerekcesi > 0)
            {
                if (p.ayrilmaGerekcesiTip == ENUMHizmetSebebiNevi.AYRILMA || p.ayrilmaGerekcesiTip == ENUMHizmetSebebiNevi.NAKILAYRILMA)
                {
                    if (p.ayrilmaGerekcesiTip == ENUMHizmetSebebiNevi.NAKILAYRILMA)
                    {
                        if (!TNSDateTime.IsNull(p.ayrilmaTarihi))
                            sonuc.DegerAta("Kurum içi nakillerde ayrılma tarihi girmeyiniz!");
                        if (p.tayinOlacagiBirim <= 0)
                            sonuc.DegerAta("Kurum içi nakillerde gideceği birimi seçiniz!");
                    }
                    else
                    {
                        if (p.tayinOlacagiBirim != 0)
                            sonuc.DegerAta("Kurum içi nakillerde gideceği birimi boş bırakınız!");
                        if (TNSDateTime.IsNull(p.ayrilmaTarihi))
                            sonuc.DegerAta("Ayrılma tarihi boş olamaz!");
                        else
                        {
                            if (p.ayrilmaTarihiDT <= p.atamaTarihiDT)
                                sonuc.DegerAta("Ayrılma tarihi atama tarihinden büyük olmalıdır!");
                        }
                    }
                }
                else
                    sonuc.DegerAta("Hatalı ayrılma grubu!");
            }
            else
            {
                p.hizmetGunSayisi = 0;
                if (!TNSDateTime.IsNull(p.ayrilmaTarihi))
                    sonuc.DegerAta("Ayrılma işlemi değilse ayrılma tarihi girmeyiniz!");
                if (!string.IsNullOrWhiteSpace(p.ayrilmaNedeni))
                    sonuc.DegerAta("Ayrılma işlemi değilse, ayrılma nedeni de girmeyiniz!");
            }

            if (TNSDateTime.IsNull(p.ayrilmaTarihi) && !p.terfiMi)
            {
                Sonuc sonucK = KadroYeterliMi(p, personelTipi, hBirimi);

                if (!sonucK.islemSonuc)
                {
                    sonuc.DegerAta(sonucK.hataStr);
                }
            }
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            ENUMTARIHCEISLEMTIP islemTip;

            //Tarihçe ********************
            if (p.kod > 0)
            {
                eskiTmp = ListeleKadro(ve, kullanan, new PersonelKadro() { personelKod = p.personelKod, kod = p.kod }, false, true);
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelKadro();// p.kod = 0: "Yeni Kayıt"
            //****************************

            ve.BeginTransaction();
            try
            {
                TABPersonelKadro tab = new TABPersonelKadro(ve);

                int yukselecegiDerece = 0;
                IDataReader r;
                p.GostergeDuzenle();
                if (!(p.kadroDurum == ENUMKadroDurumu.AKTIFYENI || p.kadroDurum == ENUMKadroDurumu.AKTIFKAPANIYOR))
                    p.kadroDurum = ENUMKadroDurumu.YENI;
                if (p.kod == 0)
                {
                    p.kod = TABGenel.SayacDegerAlInt(ve, TABPersonelKadro.TABLOAD);
                    islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                    sonuc = tab.KaydetKadro(p);
                }
                else
                {
                    PersonelKadro eskiKadro = KadroKaydiOku(ve, kullanan, p.kod, sonuc, p.personelKod);
                    islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                    if (eskiKadro.kadroDurum == ENUMKadroDurumu.AKTIFKAYIT || eskiKadro.kadroDurum == ENUMKadroDurumu.AKTIFKAPANIYOR)
                    {
                        p.kadroDurum = ENUMKadroDurumu.AKTIFKAPANIYOR;
                        sonuc = tab.GuncelleAyrilis(p);
                    }
                    else
                        sonuc = tab.GuncelleKadro(p);
                }

                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = p.kod.ToString();
                    ve.Commit();

                    //if (!string.IsNullOrEmpty(p.atama.atamaNedeni))
                    //{
                    //    IKYPersonelAtama atama = new IKYPersonelAtama();
                    //    p.atama.pkkod = OrtakFonksiyonlar.ConvertToInt(p.kod, 0);
                    //    sonuc = atama.Kaydet(kullanan, p.atama);
                    //}
                    sonuc.bilgiStr = PersonelResI.PER007;

                    if (personelTipi.KadroluMu())
                    {
                        r = tab.YukselebilecegiDereceDondur(p.personelKod);
                        if (r.Read())
                            yukselecegiDerece = VeriErisim.DegerAl(r, (int)0, 0);
                        r.Close();

                        if (p.kadroDerece < yukselecegiDerece)
                            sonuc.bilgiStr += "\nHizmetin kadro derecesi yükselebileceği dereceden daha yüksektir.";
                    }
                    //KadroBilgisiPersonelGuncelle(p.tcKimlikNo);
                    //MemuriyetKadroBilgileriniGuncelle(kullanan, p.tcKimlikNo);
                    //Tarihçe Kaydet ********************
                    yeni = p;
                    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.KADRO, p.personelKod, islemTip);
                }
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                sonuc.DegerAta("Beklenmeyen hata:" + ex.Message);
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static void Logla1(string metin, bool yeni = false)
        {
            OrtakFonksiyonlar.HataDosyaYaz("PersonelKadro.txt", metin, yeni);
        }

        private static Sonuc KadroYeterliMi(PersonelKadro p, PersonelTipi personelTipi, OrganizasyonBirim birim)
        {
            Sonuc sonuc = new Sonuc();
            bool kadrolu = false;
            if (personelTipi.KadroIslemiYapilacakMi())
            {
                kadrolu = true;
                if (p.atananacagiKadroDerece == 0)
                {
                    p.atananacagiKadroDerece = p.kadroDerece;
                    p.atananacagiKadroUnvanKod = p.unvan;
                }
            }
            if (!kadrolu) return sonuc;

            if (p.atananacagiKadroDerece == 0 || p.atananacagiKadroUnvanKod == 0)
            {
                sonuc.DegerAta("Kadrolu personel için kadro ve unvan belirtilmelidir!");
                return sonuc;
            }
            PersonelKadro p2 = new PersonelKadro();
            if (birim == null)
            {
                birim = ORGUstHarcamaBirimBul(p.organizasyonKodu);
                if (birim == null)
                {
                    sonuc.DegerAta("Harcama birimi bulunamadı!");
                    return sonuc;
                }
            }
            p2.organizasyonKodu = birim.kod;
            p2.unvan = p.atananacagiKadroUnvanKod;
            p2.kadroDerece = p.atananacagiKadroDerece;
            p2.kisiHaric = p.personelKod;

            ObjectArray list = IKYRapor.DoluBosKadroListele(null, p2);
            bool yeterli = false;
            if (list.ObjeSayisi == 1)
            {
                KadroDoluBos k = (KadroDoluBos)list.objeler[0];
                if (k.bos <= 0)
                    yeterli = false;
                else
                    yeterli = true;
            }
            if (!yeterli)
                sonuc.DegerAta("Belirtmiş olduğunuz birim (" + p2.organizasyonKodu.ToString() + "), unvan(" + p2.unvan + "), kadro derecesinde(" + p2.kadroDerece.ToString() + ") ve personel türünde(" + personelTipi.ad + ") açık kontenjan bulunmamaktadır.");
            return sonuc;
        }

        private static PersonelKadro KadroKaydiOku(VeriErisim ve, Kullanici kullanan, int kadroKod, Sonuc sonuc, int personelKod)
        {
            PersonelKadro eskiKadro = null;
            if (kadroKod > 0)
            {
                ObjectArray listKadro = ListeleKadro(ve, kullanan, new PersonelKadro { kod = kadroKod, personelKod = personelKod }, true, true);
                if (listKadro.sonuc.islemSonuc && listKadro.ObjeSayisi > 0)
                {
                    eskiKadro = (PersonelKadro)listKadro.objeler[0];
                }
                if (eskiKadro == null)
                    sonuc.DegerAta("Eski kayıt okunamadı!");
            }
            return eskiKadro;
        }

        private static void DereceKademeKontrolu(Sonuc sonuc, string tur, int ekGosterge, int gosterge, int derece, int kademe, TNSDateTime tarih)
        {
            if (derece <= 0 || derece > 15)
                sonuc.DegerAta(tur + " Derecesi 1-15 arasında olmalı!");
            if (kademe <= 0 || kademe > 9)
                sonuc.DegerAta(tur + " Kademesi 1-9 arasında olmalı!");
            if (ekGosterge < 0)
                sonuc.DegerAta(tur + " Ekgöstergesi negatif olamaz!");
            if (TNS.IKY.Arac.GostergeBul(derece, kademe) < 1)
                sonuc.DegerAta(tur + " Göstergesi olmayan derece kademe giremezsiniz!");
            if (tarih == null || tarih.isNull)
                sonuc.DegerAta(tur + " derece/kademesini alış tarihi boş bırakılamaz!");
        }

        public static ObjectArray ListeleHizmetSebebi(ENUMHizmetSebebiNevi sebepler)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(null));
            TABIKYHIZMETSEBEBI tab = new TABIKYHIZMETSEBEBI(ve);

            HizmetSebebi sb = new HizmetSebebi();
            sb.tip = sebepler;
            IDataReader r = tab.ListeleHizmetSebebi(sb);

            if (r == null)
            {
                pVAN.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return pVAN;
            }

            while (r.Read())
            {
                HizmetSebebi d = new HizmetSebebi();
                d.kod = VeriErisim.DegerAl(r, 0, 0);
                d.sebepAdi = VeriErisim.DegerAl(r, 1, "");
                d.tip = (ENUMHizmetSebebiNevi)VeriErisim.DegerAl(r, 3, 0);

                pVAN.objeler.Add(d);
            }
            r.Close();
            ve.VeriErisimKapat();
            return pVAN;
        }

        public static int HizmetGunuDondur(VeriErisim veriErisim, DateTime baslangicT, DateTime bitisT, int kadroKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(null));
            TABPersonelKadro tab = new TABPersonelKadro(ve);
            IDataReader r = tab.HizmetGunuDondur(baslangicT, bitisT, kadroKod);
            int tut = 0;
            if (r != null)
            {
                if (r.Read())
                {
                    tut = VeriErisim.DegerAl(r, 0, 0);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return tut;
        }

        //public DateTime MinAtamaTarihiniDondur(int tip, int pkod)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
        //    TABPersonelKadro tab = new TABPersonelKadro(ve);
        //    DateTime tut=new DateTime();
        //    IDataReader r = tab.MinAtamaTarihiniDondur(tip, pkod);
        //    if (r != null)
        //    {
        //        if (r.Read())
        //            tut = VeriErisim.DegerAl(r, 0).Oku();

        //        r.Close();
        //    }
        //    ve.VeriErisimKapat();
        //    return tut;
        //}

        public static ObjectArray UnvanaGoreHizmetGrupla(VeriErisim veriErisim, int pKod)
        {
            ObjectArray arrList = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(null));
            TABPersonelKadro tab = new TABPersonelKadro(ve);
            IDataReader r = tab.UnvanaGoreHizmetGrupla(pKod);
            if (r != null)
            {
                while (r.Read())
                {
                    Unvan unvan = new Unvan();
                    unvan.ad = VeriErisim.DegerAl(r, 1, "");
                    unvan.minYil = VeriErisim.DegerAl(r, 2, 0);
                    unvan.maxYil = VeriErisim.DegerAl(r, 3, 0);
                    unvan.hizmetGunu = VeriErisim.DegerAl(r, 4, 0);
                    arrList.Ekle(unvan);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return arrList;
        }

        //public Hizmet HizmetSureBilgileriniGetir(int personelKod, DateTime sonTarih)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));

        //    Hizmet hizmet = new Hizmet();
        //    //hizmet.kadroyaGecmeTarihi = MinAtamaTarihiniDondur(1, personelKod);
        //    //hizmet.kadrodakiSure = HizmetGunuDondur(ve, hizmet.kadroyaGecmeTarihi, sonTarih, 0);
        //    //hizmet.guncelHizmet = GuncelBirimUnvanAdDondur(ve, personelKod);
        ////    hizmet.kurumaBasladigiTarihi = hizmet.guncelHizmet.kurumaBasladigiTarih;
        ////    hizmet.personelAd = hizmet.guncelHizmet.personelAd;
        ////    hizmet.kurumdakiSure = HizmetGunuDondur(ve, hizmet.kurumaBasladigiTarihi, sonTarih, 0);
        ////    hizmet.baslangicTarihi = hizmet.guncelHizmet.memuriyeteBasladigiTarih; //MinAtamaTarihiniDondur(0, personelKod); 
        ////    hizmet.hizmetGunu = HizmetGunuDondur(ve, hizmet.baslangicTarihi, sonTarih, 0);
        //    ve.VeriErisimKapat();
        //    return hizmet;
        //}

        public static ObjectArray ListeleEskiHizmetTipi(VeriErisim veriErisim)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!IKY.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(null));
            TABPersonelKadro tab = new TABPersonelKadro(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleEskiHizmetTipi();
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
                EskiHizmetTipi d = new EskiHizmetTipi();
                d.kod = VeriErisim.DegerAl(r, 0, 0);
                d.hizmetAdi = VeriErisim.DegerAl(r, 1, "");
                d.toplam = VeriErisim.DegerAl(r, 2, 0);
                d.aylik = VeriErisim.DegerAl(r, 3, 0);
                d.emekli = VeriErisim.DegerAl(r, 4, 0);
                d.izin = VeriErisim.DegerAl(r, 5, 0);
                d.cb = VeriErisim.DegerAl(r, 6, 0);

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public static ObjectArray HizmetleriGetir(VeriErisim veriErisim, Kullanici kullanan, string organizasyonKopruKod, string personelKod, string personelTur, TNSDateTime sonTarih, int siralamaListesi, string raporKod, int aktiflik)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!IKY.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}
            int sira = 0;
            if (raporKod == "IK031") sira = 23;
            else if (raporKod == "IK032") sira = 19;
            else if (raporKod == "IK033") sira = 21;
            else if (raporKod == "IK034") sira = 22;
            else if (raporKod == "IK035") sira = 18;
            else if (raporKod == "IK046") sira = 24;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(null));
            IDataReader dr = ve.DataReaderDondur(TABPersonelKadro.HizmetleriGetirSql(organizasyonKopruKod, personelKod, personelTur, sonTarih, siralamaListesi, sira, aktiflik));
            if (dr == null)
            {
                pVAN.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return pVAN;
            }

            while (dr.Read())
            {
                Hizmet d = new Hizmet();
                d.pKod = VeriErisim.DegerAl(dr, 0, 0);
                d.personelAd = VeriErisim.DegerAl(dr, 1, "");
                d.guncelHizmet.unvan = VeriErisim.DegerAl(dr, 2, "");
                d.guncelHizmet.birim = VeriErisim.DegerAl(dr, 3, "");
                d.guncelHizmet.kurumaBasladigiTarih = VeriErisim.DegerAl(dr, 4).Oku();
                d.guncelHizmet.memuriyeteBasladigiTarih = VeriErisim.DegerAl(dr, 5).Oku();
                d.guncelHizmet.kadroyaGectigiTarih = VeriErisim.DegerAl(dr, 6).Oku();
                d.guncelHizmet.personelTC = VeriErisim.DegerAl(dr, 7, "");
                d.guncelHizmet.personelSicil = VeriErisim.DegerAl(dr, 8, "");
                d.guncelHizmet.geldigiKurum = VeriErisim.DegerAl(dr, 9, "");
                d.guncelHizmet.kadroDerece = VeriErisim.DegerAl(dr, 10, 0);
                d.guncelHizmet.ekGosterge = VeriErisim.DegerAl(dr, 11, 0);
                d.guncelHizmet.kDerece = VeriErisim.DegerAl(dr, 12, 0);
                d.guncelHizmet.kKademe = VeriErisim.DegerAl(dr, 13, 0);
                d.guncelHizmet.eDerece = VeriErisim.DegerAl(dr, 14, 0);
                d.guncelHizmet.eKademe = VeriErisim.DegerAl(dr, 15, 0);
                d.guncelHizmet.gorevUnvan = VeriErisim.DegerAl(dr, 18, "");
                d.hizmetGunu = VeriErisim.DegerAl(dr, 19, 0);
                d.kidemGunu = VeriErisim.DegerAl(dr, 20, 0);
                d.KHAGunu = VeriErisim.DegerAl(dr, 21, 0);
                d.EKEAGunu = VeriErisim.DegerAl(dr, 22, 0);
                d.izinHizmetGunu = VeriErisim.DegerAl(dr, 23, 0);
                d.kurumdakiSure = VeriErisim.DegerAl(dr, 24, 0);
                d.kadrodakiSure = VeriErisim.DegerAl(dr, 25, 0);

                d.guncelHizmet.personelAd = d.personelAd;

                pVAN.objeler.Add(d);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return pVAN;
        }

        //public IzinGunleri IzinGunSayisiDondur(int pKod)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
        //    TABPersonelKadro tab = new TABPersonelKadro(ve);
        //    IzinGunleri izin = new IzinGunleri();

        //    IDataReader r = tab.IzinGunSayisiDondur(pKod);
        //    if (r == null)
        //    {
        //        ve.VeriErisimKapat();
        //        throw new Exception(ve.hata);
        //    }

        //    if (r.Read())
        //    {
        //        izin.toplam = VeriErisim.DegerAl(r, 0, 0);
        //        izin.kurum = VeriErisim.DegerAl(r, 1, 0);
        //        izin.kadro = VeriErisim.DegerAl(r, 2, 0);
        //    }

        //    return izin;
        //}

        //public BirimUnvan GuncelBirimUnvanAdDondur(VeriErisim ve, int personelKod)
        //{
        //    TABPersonelKadro tab = new TABPersonelKadro(ve);
        //    BirimUnvan bilgi = new BirimUnvan();

        //    IDataReader r = tab.GuncelBirimUnvanAdDondur(personelKod);
        //    if (r == null)
        //    {
        //        ve.VeriErisimKapat();
        //        throw new Exception(ve.hata);
        //    }

        //    if (r.Read())
        //    {
        //        bilgi.unvan = VeriErisim.DegerAl(r, 0, "");
        //        bilgi.birim = VeriErisim.DegerAl(r, 1, "");
        //        bilgi.kurumaBasladigiTarih = VeriErisim.DegerAl(r, 2).Oku();
        //        bilgi.personelAd = VeriErisim.DegerAl(r, 3, "");
        //        bilgi.memuriyeteBasladigiTarih = VeriErisim.DegerAl(r, 4).Oku();
        //        bilgi.kadroyaGectigiTarih = VeriErisim.DegerAl(r, 5).Oku();
        //    }
        //    r.Close();

        //    return bilgi;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="atama"></param>
        /// <param name="onayIslem">1 ise onayla, 9 ise iptal et, 10 ise ve önceki iptalli ise sil</param>
        /// <returns></returns>
        public static Sonuc GuncelleOnayBilgileri(VeriErisim veriErisim, Kullanici kullanan, TNSCollection atamalar, int onayIslem, bool onayKontroluYap, bool kullaniciyiAktifYap)
        {
            Sonuc sonuc = new Sonuc();
            if (atamalar.Count == 0)
                sonuc.DegerAta("Lütfen durumu değiştirilecek kayıt seçiniz!");

            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelKadro tabKadro = new TABPersonelKadro(ve);

            TNS.IKY.Personel p = new TNS.IKY.Personel();
            foreach (TNS.IKY.PersonelKadro atama in atamalar)
            {
                if (atama.kod == 0)
                    return new Sonuc("Kayıt işlemi yapmadan onaylayamazsınız!");
                if (onayKontroluYap)
                {
                    ObjectVAN ov = IKYPersonel.Oku(ve, kullanan, atama.personelKod, false, false);
                    if (ov.o == null)
                        ov.sonuc.DegerAta("İşlem yapılacak personel bulunamadı!");
                    else
                    {
                        p = (TNS.IKY.Personel)ov.o;
                        ov.sonuc = IKYYetki.YetkiVarMiOGY(kullanan, p.kopruKodTam, p.personelTip, ov.sonuc, MUH.ENUMOGY.Onaylama, ENUMYetkiIslem.Hizmetler, true, p);
                    }

                    if (!ov.sonuc.islemSonuc)
                    {
                        ve.VeriErisimKapat();
                        return ov.sonuc;
                    }
                }

                Sonuc sonucIc = new Sonuc();
                PersonelKadro eskiKadro = null;
                PersonelTipi personelTipi = null;

                eskiKadro = KadroKaydiOku(ve, kullanan, atama.kod, sonucIc, atama.personelKod);
                if (eskiKadro == null)
                {
                    sonucIc.DegerAta("Onaylanacak kayıt bulunamadı!");
                }
                else
                {
                    personelTipi = TNS.IKY.Arac.PersonelTipBul(eskiKadro.personelTip);
                    if (personelTipi == null)
                        sonucIc.DegerAta("Personel tipi bulunamadı!");
                    else if ((onayIslem == 5) && (eskiKadro.ayrilmaTarihi == null || eskiKadro.ayrilmaTarihi.isNull))
                        sonucIc = KadroYeterliMi(eskiKadro, personelTipi, null);
                }
                if (sonucIc.islemSonuc)
                {
                    ve.BeginTransaction();
                    int kadroIslemNo = 0;
                    ENUMKadroDurumu kadroDurumu = ENUMKadroDurumu.TANIMSIZ;
                    if (onayIslem == 1)
                    {
                        if (eskiKadro != null && (eskiKadro.kadroDurum == ENUMKadroDurumu.AKTIFKAYIT || eskiKadro.kadroDurum == ENUMKadroDurumu.AKTIFKAPANIYOR || eskiKadro.kadroDurum == ENUMKadroDurumu.AKTIFKAPANDI || eskiKadro.kadroDurum == ENUMKadroDurumu.AKTIFYENI || eskiKadro.kadroDurum == ENUMKadroDurumu.AKTIFUCRETSIZIZIN))
                            kadroDurumu = ENUMKadroDurumu.AKTIFYENI;
                        else
                            kadroDurumu = ENUMKadroDurumu.YENI;
                    }
                    else if (onayIslem == 5)
                    {
                        if (eskiKadro.kadroDurum == ENUMKadroDurumu.AKTIFKAPANIYOR)
                        {
                            if (eskiKadro.ayrilmaGerekcesiTip == ENUMHizmetSebebiNevi.NAKILAYRILMA)
                                sonucIc.DegerAta("Nakil ayrılmaların onayı kabul işlemi ile olacağından, onaylama işlemi yapılamadı!");
                            kadroDurumu = ENUMKadroDurumu.AKTIFKAPANDI;
                        }
                        else if (TNSDateTime.IsNull(eskiKadro.ayrilmaTarihi))
                        {
                            kadroDurumu = ENUMKadroDurumu.AKTIFKAYIT;
                            if (eskiKadro.ayrilmaGerekcesi > 0)
                                kadroDurumu = ENUMKadroDurumu.AKTIFKAPANIYOR;
                        }
                        else
                            kadroDurumu = ENUMKadroDurumu.AYRILDI;
                        if (sonucIc.islemSonuc && (kadroDurumu == ENUMKadroDurumu.AYRILDI || kadroDurumu == ENUMKadroDurumu.AKTIFKAPANDI) && TNSDateTime.IsNull(eskiKadro.ayrilmaTarihi))
                            sonucIc.DegerAta("Ayrılma tarihi boş olduğundan kayıt yapılamadı!");

                        if ((kadroDurumu == ENUMKadroDurumu.AKTIFKAYIT || kadroDurumu == ENUMKadroDurumu.AKTIFKAPANIYOR) && personelTipi.KadroIslemiYapilacakMi())
                        {
                            sonucIc = KadroDegistir(ve, kullanan, eskiKadro.organizasyonKodu, eskiKadro.atananacagiKadroUnvanKod, eskiKadro.atananacagiKadroDerece, eskiKadro.unvan, eskiKadro.kadroDerece, eskiKadro.iliskiliKadroIslemNo);
                            if (sonucIc.islemSonuc)
                                kadroIslemNo = OrtakFonksiyonlar.ConvertToInt(sonucIc.anahtar, 0);
                        }
                    }
                    else if (onayIslem == 10)
                    {
                        if (eskiKadro.kadroDurum == ENUMKadroDurumu.REDDEDILDI)
                            kadroDurumu = ENUMKadroDurumu.SIL;
                        else
                            sonucIc.DegerAta("Yalnızca iptal edilmiş kayıtlar silinebilir!");
                    }
                    else
                        kadroDurumu = ENUMKadroDurumu.REDDEDILDI;

                    if (sonucIc.islemSonuc && (kadroDurumu == ENUMKadroDurumu.REDDEDILDI || kadroDurumu == ENUMKadroDurumu.SIL || kadroDurumu == ENUMKadroDurumu.YENI))
                    {
                        if (eskiKadro.iliskiliKadroIslemNo > 0)
                            sonucIc = KadroIslemiSil(ve, kullanan, eskiKadro.iliskiliKadroIslemNo);
                    }

                    if (eskiKadro.eskiKod > 0 && onayIslem == 5)
                    {
                        TNS.IKY.PersonelKadro oncekiIslem = KadroKaydiOku(ve, kullanan, eskiKadro.eskiKod, sonucIc, atama.personelKod);
                        if (sonucIc.islemSonuc && oncekiIslem != null)
                        {
                            //if (oncekiIslem.ayrilmaTarihi == null || oncekiIslem.ayrilmaTarihi.isNull)
                            //{
                            int ayrGerekce;
                            ENUMHizmetSebebiNevi ayrGerekceTip;
                            HizmetSebebiBul(eskiKadro.hizmetSebebiTip, eskiKadro.hizmetSebebiKod, oncekiIslem.hizmetSebebiTip, out ayrGerekce, out ayrGerekceTip);
                            if (ayrGerekceTip != ENUMHizmetSebebiNevi.TANIMSIZ)
                            {
                                oncekiIslem.ayrilmaGerekcesi = ayrGerekce;
                                oncekiIslem.kadroDurum = ENUMKadroDurumu.AYRILDI;
                                oncekiIslem.ayrilmaTarihi = eskiKadro.atamaTarihi;
                                oncekiIslem.hizmetGunSayisi = HizmetGunuDondur(ve, oncekiIslem.atamaTarihiDT, oncekiIslem.ayrilmaTarihiDT, 0);
                                sonucIc = tabKadro.GuncelleKadroDurumu(oncekiIslem, true, 0);
                            }
                        }
                    }

                    TABPersonel tabPersonel = new TABPersonel(ve);
                    if (sonucIc.islemSonuc && (kadroDurumu == ENUMKadroDurumu.AKTIFKAYIT || kadroDurumu == ENUMKadroDurumu.AKTIFYENI)) //Aktif kayıt oluşacaksa, başka aktifleri kapat
                    {
                        sonucIc = tabKadro.TumAktifleriKapat(eskiKadro, true); //tarihi boş olanları kapat
                        if (sonucIc.islemSonuc)
                            sonucIc = tabKadro.TumAktifleriKapat(eskiKadro, false); //tarihi boş olmayanları kapat
                    }

                    if (sonucIc.islemSonuc)
                    {
                        atama.kadroDurum = kadroDurumu;
                        atama.onayTarih = new TNSDateTime(System.DateTime.Now);
                        atama.onaylayan = kullanan.mernis;
                        if (kadroDurumu == ENUMKadroDurumu.SIL)
                            sonucIc = tabKadro.SilKadro(atama);
                        else
                            sonucIc = tabKadro.GuncelleKadroDurumu(atama, false, kadroIslemNo);
                    }

                    int aktifSay = -1;
                    if (sonucIc.islemSonuc) //Aktif kayıt kalmamışsa düzelt
                    {
                        aktifSay = tabKadro.AktifSay(eskiKadro.personelKod, true);
                        if (aktifSay != 1)
                        {
                            if (aktifSay > 1)
                            {
                                sonucIc = tabKadro.TumAktifleriKapat(eskiKadro, true); //tarihi boş olanları kapat
                                if (sonucIc.islemSonuc)
                                    sonucIc = tabKadro.TumAktifleriKapat(eskiKadro, false); //tarihi boş olmayanları kapat
                            }
                            if (sonucIc.islemSonuc)
                                sonucIc = tabKadro.SonKaydiAktifYap(eskiKadro.personelKod); //Son kaydı aktif yap
                        }
                    }
                    aktifSay = -1;
                    if (sonucIc.islemSonuc)
                    {
                        aktifSay = tabKadro.AktifSay(eskiKadro.personelKod, false);
                        sonucIc = tabPersonel.GuncelleAktiflik(eskiKadro.personelKod, aktifSay >= 1 ? 1 : 0); //Personeli pasif yap
                    }

                    if (sonucIc.islemSonuc)
                    {
                        ve.Commit();
                        if (aktifSay != -1 && kullaniciyiAktifYap)
                        {
                            //Kullanıcı veritabanında aktif/pasif yap
                            Sonuc sonucKullanici = new Sonuc(false, "", "");

                            TNS.KYM.IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

                            KullaniciVAN kVan = servisKYM.KullaniciOku(kullanan, eskiKadro.personelTCKimlikNo);

                            if (kVan.sonuc.islemSonuc && !string.IsNullOrWhiteSpace(kVan.kullanici.kullaniciKodu))
                            {
                                Kullanici kul = new Kullanici
                                {
                                    kullaniciKodu = kVan.kullanici.kullaniciKodu,
                                    calistigiKurum = kVan.kullanici.calistigiKurum,
                                    parolaBitisTarihi = kVan.kullanici.parolaBitisTarihi,
                                    unvan = eskiKadro.unvanAd,
                                    sicilNo = p.sicilNo
                                };

                                if (aktifSay >= 1)
                                    sonucKullanici = servisKYM.KullaniciyiAktifYap(kullanan, kul);
                                else
                                    sonucKullanici = servisKYM.KullaniciyiPasifYap(kullanan, kul);

                                if (!sonucKullanici.islemSonuc)
                                    Logla1("aktifSay:" + aktifSay + ",eskiKadro.personelTCKimlikNo:" + eskiKadro.personelTCKimlikNo + ",kVan.kullanici.kullaniciKodu: " + kVan.kullanici.kullaniciKodu + ",eskiKadro.unvanAd:" + eskiKadro.unvanAd + ",sonucKullanici.islemSonuc" + sonucKullanici.islemSonuc);
                            }
                            else
                                Logla1("aktifSay:" + aktifSay + ",eskiKadro.personelTCKimlikNo:" + eskiKadro.personelTCKimlikNo + ",kVan.kullanici.kullaniciKodu: " + kVan.kullanici.kullaniciKodu + ",eskiKadro.unvanAd:" + eskiKadro.unvanAd + ",kVan.sonuc.islemSonuc" + kVan.sonuc.islemSonuc);
                        }
                        else
                            Logla1("aktifSay:" + aktifSay + ",eskiKadro.personelTCKimlikNo:" + eskiKadro.personelTCKimlikNo + ",kullaniciyiAktifYap: " + kullaniciyiAktifYap);
                    }
                    else
                        ve.Rollback();
                }
                if (!sonucIc.islemSonuc)
                    sonuc.bilgiStr = String.Format("{0} kodlu işlemin durumu değiştirilemedi: ", atama.kod) + sonucIc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        private static void HizmetSebebiBul(ENUMHizmetSebebiNevi simSebepNevi, int simSebepKod, ENUMHizmetSebebiNevi oncSebepNevi, out int oncSebepAyrKod, out ENUMHizmetSebebiNevi oncSebepAyrNevi)
        {
            if (simSebepNevi == ENUMHizmetSebebiNevi.BASLAMA)
            {
                oncSebepAyrKod = simSebepKod;
                oncSebepAyrNevi = simSebepNevi;
            }
            else if (simSebepNevi == ENUMHizmetSebebiNevi.TERFI)
            {
                oncSebepAyrKod = simSebepKod;
                oncSebepAyrNevi = simSebepNevi;
            }
            else if (simSebepNevi == ENUMHizmetSebebiNevi.NAKILBASLAMA)
            {
                oncSebepAyrNevi = ENUMHizmetSebebiNevi.NAKILAYRILMA;
                oncSebepAyrKod = SebepKodBul(oncSebepAyrNevi);
            }
            else if (simSebepNevi == ENUMHizmetSebebiNevi.UCRETSIZIZIN)
            {
                oncSebepAyrNevi = ENUMHizmetSebebiNevi.UCRETSIZAYRILMA;
                oncSebepAyrKod = SebepKodBul(oncSebepAyrNevi);
            }
            else if (simSebepNevi == ENUMHizmetSebebiNevi.YURTDISIEGITIM)
            {
                oncSebepAyrNevi = ENUMHizmetSebebiNevi.YURTDISIEGITIMEAYRILMA;
                oncSebepAyrKod = SebepKodBul(oncSebepAyrNevi);
            }
            else if (simSebepNevi == ENUMHizmetSebebiNevi.GOREVDENUZAK)
            {
                oncSebepAyrNevi = ENUMHizmetSebebiNevi.GOREVUZAKAYRILMA;
                oncSebepAyrKod = SebepKodBul(oncSebepAyrNevi);
            }
            else
            {
                oncSebepAyrNevi = ENUMHizmetSebebiNevi.TANIMSIZ;
                oncSebepAyrKod = 0;
            }
        }

        private static Hashtable htSebepKodlar = new Hashtable();
        private static int SebepKodBul(ENUMHizmetSebebiNevi oncSebepAyrNevi)
        {
            int ioncSebepAyrNevi = (int)oncSebepAyrNevi;
            object o = htSebepKodlar[ioncSebepAyrNevi];
            bool oku = false;
            if (o == null)
            {
                oku = true;
            }
            else if ((int)o == 0)
            {
                oku = true;
            }
            if (oku)
            {
                int ilkKod = IlkKodBul(ioncSebepAyrNevi);
                htSebepKodlar[ioncSebepAyrNevi] = ilkKod;
                o = ilkKod;
            }
            return (int)o;
        }

        private static int IlkKodBul(int ioncSebepAyrNevi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", null);
            int kod = ve.TekKolonSQLInt("SELECT MIN(KOD) FROM IKYSOZLUKHIZMETSEBEBI WHERE TIP=" + ioncSebepAyrNevi.ToString());
            ve.VeriErisimKapat();
            return kod;
        }

        private static Sonuc KadroIslemiSil(VeriErisim ve, Kullanici kullanan, int iliskiliKadroIslemNo)
        {
            KadroKontenjan kk = new KadroKontenjan();
            kk.kod = iliskiliKadroIslemNo;
            return PERKadroKontenjan.Sil(ve, kullanan, kk, true);
        }

        private static Sonuc KadroDegistir(VeriErisim ve, Kullanici kullanan, int organizasyonKod, int atananacagiKadroUnvanKod, int atananacagiKadroDerece, int unvan, int kadroDerece, int iliskiliKadroIslemNo)
        {
            if ((atananacagiKadroUnvanKod == unvan && atananacagiKadroDerece == kadroDerece) || (atananacagiKadroUnvanKod == 0 && atananacagiKadroDerece == 0))
            {
                return new Sonuc();
            }
            KadroKontenjan kk = new KadroKontenjan();

            kk.kod = iliskiliKadroIslemNo;
            kk.organizasyonKod = organizasyonKod;
            kk.islemTarih = new TNSDateTime(DateTime.Now);
            kk.islemTur = EnumKadroKontenjanIslemTur.Degisiklik;
            kk.aciklama = "Personel kaydı yapılırken oluşturuldu";

            List<KadroKontenjanDetay> det = new List<KadroKontenjanDetay>();
            KadroKontenjanDetay s1 = new KadroKontenjanDetay();
            s1.unvanKod = atananacagiKadroUnvanKod;
            s1.kadroDerece = atananacagiKadroDerece;
            s1.kontenjan = -1;

            KadroKontenjanDetay s2 = new KadroKontenjanDetay();
            s2.unvanKod = unvan;
            s2.kadroDerece = kadroDerece;
            s2.kontenjan = 1;

            det.Add(s1);
            det.Add(s2);
            kk.detaylar = det.ToArray();
            kk.personelTur = ENUMPersonelTur.Memur;
            return PERKadroKontenjan.Kaydet(ve, kullanan, kk, true);
        }

        public static ObjectArray ListeleKadroAyar(VeriErisim veriErisim, Kullanici kullanan, KadroAyar ka)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABPersonelKadro tab = new TABPersonelKadro(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleKadroAyar(ka);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            //int AlanlarSonIndex = (Enum.GetNames(typeof(TABPersonelKadro.ENUMALANLARKADROAYARI))).Length;

            while (r.Read())
            {
                KadroAyar ayar = new KadroAyar();

                ayar.pKod = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLARKADROAYARI.PKOD, 0);
                ayar.eTerfiAyi = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLARKADROAYARI.EMEKLITERFIAYI, 0);
                ayar.kTerfiAyi = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLARKADROAYARI.KAZANILMISTERFIAYI, 0);
                ayar.kidemTerfiAyi = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLARKADROAYARI.KIDEMTERFIAYI, 0);
                ayar.maximumDerece = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLARKADROAYARI.MAXIMUMDERECE, 0);
                ayar.eAcilisTarihi = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLARKADROAYARI.EMEKLITERFITARIHI);
                ayar.kAcilisTarihi = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLARKADROAYARI.KAZANILMISTERFITARIHI);
                ayar.aktif = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLARKADROAYARI.KADRODERECEAKTIF, 0);

                pVAN.objeler.Add(ayar);

            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public static Sonuc KaydetKadroAyar(VeriErisim veriErisim, Kullanici kullanan, KadroAyar ka)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonelKadro tab = new TABPersonelKadro(ve);

            sonuc = tab.KaydetKadroAyar(ka);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc SilKadroAyar(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc();

            if (kod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            //TABBelgeler tabBelge = new TABBelgeler(ve);
            //sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod = p.personelKod, tip = (int)ENUMBELGEFORMTIP.PERSONELOGRENIM });

            if (sonuc.islemSonuc)
            {
                TABPersonelKadro tab = new TABPersonelKadro(ve);
                sonuc = tab.SilKadroAyar(kod);
            }

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

        //public void KadroBilgisiPersonelGuncelle(string tcKimlikNo)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABPersonel tab = new TABPersonel(ve);

        //    Personel p = new Personel();
        //    p.tcKimlikNo = tcKimlikNo;

        //    IDataReader dr;

        //    dr = tab.KadroBilgisiAl(p.tcKimlikNo, 1);
        //    if (dr.Read())
        //    {
        //        //p.kadroOrganizasyonKodu = VeriErisim.DegerAl(dr, 0, 0);
        //        //p.kadroMuhasebeKodu = VeriErisim.DegerAl(dr, 1, 0);
        //        //p.kadroBirimKodu = VeriErisim.DegerAl(dr, 2, "");
        //        //p.kadroKurum = VeriErisim.DegerAl(dr, 3, "");
        //        //p.unvanKodu = VeriErisim.DegerAl(dr, 4, 0);
        //        //p.sinifKodu = VeriErisim.DegerAl(dr, 5, 0);
        //        //p.kadroDerece = VeriErisim.DegerAl(dr, 6, 0);
        //        //p.kadroTarih = VeriErisim.DegerAl(dr, 7);
        //    }
        //    dr.Close();

        //    dr = tab.KadroBilgisiAl(p.tcKimlikNo, 2);
        //    if (dr.Read())
        //    {
        //        //p.odemeEsasDerece = VeriErisim.DegerAl(dr, 0, 0);
        //        //p.odemeEsasKademe = VeriErisim.DegerAl(dr, 1, 0);
        //        //p.odemeEsasGosterge = VeriErisim.DegerAl(dr, 2, 0);
        //        //p.odemeEsasTarih = VeriErisim.DegerAl(dr, 3);
        //    }
        //    dr.Close();

        //    dr = tab.KadroBilgisiAl(p.tcKimlikNo, 3);
        //    if (dr.Read())
        //    {
        //        //p.kazanilmisHakDerece = VeriErisim.DegerAl(dr, 0, 0);
        //        //p.kazanilmisHakKademe = VeriErisim.DegerAl(dr, 1, 0);
        //        //p.kazanilmisHakTarih = VeriErisim.DegerAl(dr, 2);
        //    }
        //    dr.Close();

        //    dr = tab.KadroBilgisiAl(p.tcKimlikNo, 4);
        //    if (dr.Read())
        //    {
        //        //p.emekliEsasDerece = VeriErisim.DegerAl(dr, 0, 0);
        //        //p.emekliEsasKademe = VeriErisim.DegerAl(dr, 1, 0);
        //        //p.emekliEsasGosterge = VeriErisim.DegerAl(dr, 2, 0);
        //        //p.emekliEsasTarih = VeriErisim.DegerAl(dr, 3);
        //    }
        //    dr.Close();

        //    tab.KadroBilgisiPersonelGuncelle(p);
        //    ve.VeriErisimKapat();
        //}

        internal static Hizmet HizmetleriGetir(string okunanXML)
        {
            Hizmet d = new Hizmet();
            XMLStringDataReader dr = new XMLStringDataReader(okunanXML);
            if (dr != null)
            {
                if (dr.Read())
                {
                    d.hizmetGunu = VeriErisim.DegerAl(dr, 0, 0);
                    d.kidemGunu = VeriErisim.DegerAl(dr, 1, 0);
                    d.KHAGunu = VeriErisim.DegerAl(dr, 2, 0);
                    d.EKEAGunu = VeriErisim.DegerAl(dr, 3, 0);
                    d.izinHizmetGunu = VeriErisim.DegerAl(dr, 4, 0);
                    d.kurumdakiSure = VeriErisim.DegerAl(dr, 5, 0);
                    d.kadrodakiSure = VeriErisim.DegerAl(dr, 6, 0);
                }
            }
            return d;
        }

        internal static ObjectVAN OnaysizKayitlar(VeriErisim veriErisim, Kullanici kullanan, int orgKod, bool sadeceSayilar)
        {
            OnaysizKadroBilgi ob = new OnaysizKadroBilgi();
            ObjectVAN pVAN = new ObjectVAN();
            pVAN.o = ob;
            string kopruKod = "";
            if (orgKod > 0)
            {
                TNS.ORG.OrganizasyonBirim birim = ORG.Arac.OrgBirimBul(orgKod);
                if (birim != null)
                    kopruKod = birim.kopruKod;
            }
            else
            {
                if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                    pVAN.sonuc.DegerAta("Lütfen birim seçiniz!");
            }
            if (pVAN.sonuc.islemSonuc)
                IKYYetki.YetkiVarMiOGY(kullanan, kopruKod, 0, pVAN.sonuc, MUH.ENUMOGY.Gorme, ENUMYetkiIslem.Hizmetler, false, null);
            if (!pVAN.sonuc.islemSonuc)
            {
                return pVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelKadro tab = new TABPersonelKadro(ve);

            PersonelKadro p = new PersonelKadro();
            p.organizasyonKopruKodu = kopruKod;
            p.kadroDurum = ENUMKadroDurumu.ONAYSIZLAR;
            IDataReader r = tab.ListeleKadro(p, true, sadeceSayilar);
            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = ve.hata;

                return pVAN;
            }

            if (sadeceSayilar)
            {
                if (r.Read())
                {
                    ob.onaysizSay = VeriErisim.DegerAl(r, 0, 0);
                }
                r.Close();
            }
            else
            {
                ob.onaysizlar = new TNSCollection();
                Doldur(ob.onaysizlar, r);
                ob.onaysizSay = ob.onaysizlar.Count;
            }

            p = new PersonelKadro();
            p.organizasyonKopruKodu = "";
            p.gitOrganizasyonKopruKodu = kopruKod;
            p.kadroDurum = ENUMKadroDurumu.AKTIFKAPANIYOR;
            r = tab.ListeleKadro(p, true, sadeceSayilar);
            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = ve.hata;

                return pVAN;
            }

            if (sadeceSayilar)
            {
                if (r.Read())
                {
                    ob.tayinBekleyenSay = VeriErisim.DegerAl(r, 0, 0);
                }
                r.Close();
            }
            else
            {
                ob.tayinBekleyenler = new TNSCollection();
                Doldur(ob.tayinBekleyenler, r);
                ob.tayinBekleyenSay = ob.tayinBekleyenler.Count;
            }

            ve.VeriErisimKapat();
            return pVAN;
        }

        internal static int NakilSebebiBul(ENUMHizmetSebebiNevi sebepTur)
        {
            return SebepKodBul(sebepTur);
        }
    }
}

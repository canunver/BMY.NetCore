using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using TNS.ORG;

namespace TNS.IKY.IsKurallari
{
    class IKYPersonelAtama
    {
        //public Sonuc Kaydet(Kullanici kullanan, PersonelAtama atama)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    if (atama.pkkod == 0)
        //    {
        //        sonuc.hataStr = "Personel kadro kaydı yapılmamış.";
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABPersonelAtama tab = new TABPersonelAtama(ve);

        //    ve.BeginTransaction();

        //    if (atama.kod <= 0)
        //        atama.kod = TABGenel.SayacDegerAlInt(ve, TABPersonelAtama.TABLOAD);

        //    IDataReader r;
        //    TABSozluk s = new TABSozluk(ve);
        //    r = s.Listele(new Sozluk { kayitKod = (int)ENUMSozlukUnsurlari.HAREKETDURUMU + "." + ((int)ENUMHareketDurumu.ONAYBEKLIYOR).ToString().PadLeft(5, '0'), birebir = true });
        //    if (r.Read())
        //        atama.hareketDurumu = VeriErisim.DegerAl(r, (int)TABSozluk.ENUMAlanlar.KOD, 0);
        //    r.Dispose();

        //    sonuc = tab.Kaydet(atama);

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = PersonelResI.PER007;
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public Sonuc GuncelleOnayBilgileri(Kullanici kullanan, PersonelKadro atama)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    if (!IKY.Yetki.YetkiVarMiYazmaya(kullanan))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = PersonelResI.PER011;

        //        return sonuc;
        //    }

        //    string hata = "";
        //    if (atama.kod == 0)
        //        hata += "Personel atama bilgisi yok.<br>";

        //    if (hata != "")
        //    {
        //        sonuc.hataStr = hata;
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABPersonelKadro tabKadro = new TABPersonelKadro(ve);
        //    IDataReader r;
        //    int kadroDurumu = 0;
        //    int kayitKod = 0;

        //    if (atama.onayliMi)
        //        kayitKod = (int)ENUMKadroDurumu.DEVAMEDIYOR;
        //    else
        //        kayitKod = (int)ENUMKadroDurumu.REDDEDILDI;

        //    TABSozluk s = new TABSozluk(ve);
        //    r = s.Listele(new Sozluk { kayitKod = (int)ENUMSozlukUnsurlari.KADRODURUMU + "." + kayitKod.ToString().PadLeft(5, '0'), birebir = true });
        //    if (r.Read())
        //        kadroDurumu = VeriErisim.DegerAl(r, (int)TABSozluk.ENUMAlanlar.KOD, 0);
        //    r.Dispose();

        //    IKYPersonelKadro servisCKadro = new IKYPersonelKadro();
        //    sonuc = servisCKadro.GuncelleKadroDurumu(kullanan, kadroDurumu, atama.kod, atama.kadroTarih.Oku(), false);
        //    if (sonuc.islemSonuc && atama.onayliMi)
        //    {
        //        if (atama.eskiKod > 0)
        //        {
        //            kadroDurumu = 0;
        //            r = s.Listele(new Sozluk { kayitKod = (int)ENUMSozlukUnsurlari.KADRODURUMU + "." + ((int)ENUMKadroDurumu.AYRILDI).ToString().PadLeft(5, '0'), birebir = true });
        //            if (r.Read())
        //                kadroDurumu = VeriErisim.DegerAl(r, (int)TABSozluk.ENUMAlanlar.KOD, 0);
        //            r.Dispose();
        //            sonuc = servisCKadro.GuncelleKadroDurumu(kullanan, kadroDurumu, atama.eskiKod, atama.kadroTarih.Oku(), true);
        //        }
        //    }
        //    sonuc.bilgiStr = "Onay/Red işlemi başarı ile gerçekleşti.";

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public ObjectArray ListeleAtama(Kullanici kullanan, PersonelKadro p)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    //if (!IKY.Yetki.YetkiVarMiOkumaya(kullanan))
        //    //{
        //    //    pVAN.sonuc.islemSonuc = false;
        //    //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

        //    //    return pVAN;
        //    //}

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABPersonelAtama tab = new TABPersonelAtama(ve);

        //    IDataReader r = tab.ListeleAtama(p);

        //    if (r == null)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.DegerAta(ve.hata);
        //        return pVAN;
        //    }

        //    int atamaAlanlarSonIndex = (Enum.GetNames(typeof(TABPersonelAtama.ENUMAlanlar))).Length;
        //    int kadroAlanlarSonIndex = (Enum.GetNames(typeof(TABPersonelKadro.ENUMALANLAR))).Length;
        //    int toplamIndex = atamaAlanlarSonIndex + kadroAlanlarSonIndex;

        //    while (r.Read())
        //    {
        //        PersonelKadro d = new PersonelKadro();
        //        d.kod = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.KOD, 0);
        //        d.personelKod = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.PKOD, 0);
        //        d.organizasyonKodu = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.ORGANIZASYONKOD, 0);
        //        d.unvan = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.UNVANKOD, 0);
        //        d.kadroDerece = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.KADRODERECE, 0);
        //        d.kadroTarih = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.KADROTARIH);
        //        d.evrakNo = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.EVRAKNO, "");
        //        d.yasalDayanak = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.YASALDAYANAK, "");
        //        d.onayTarih = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.ONAYTARIH);
        //        d.aciklama = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.ACIKLAMA, "");
        //        d.odemeyeEsasDerece = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.ODEMEYEESASDERECE, 0);
        //        d.odemeyeEsasKademe = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.ODEMEYEESASKADEME, 0);
        //        d.odemeyeEsasTarih = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.ODEMEYEESASTARIH);
        //        d.kazanilmisHakDerece = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.KAZANILMISHAKDERECE, 0);
        //        d.kazanilmisHakKademe = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.KAZANILMISHAKKADEME, 0);
        //        d.kazanilmisHakTarih = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.KAZANILMISHAKTARIH);
        //        d.emekliyeEsasDerece = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.EMEKLIYEESASDERECE, 0);
        //        d.emekliyeEsasKademe = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.EMEKLIYEESASKADEME, 0);
        //        d.emekliyeEsasTarih = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.EMEKLIYEESASTARIH);
        //        d.odemeyeEsasGosterge = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.ODEMEYEESASGOSTERGE, 0);
        //        d.odemeyeEsasEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.ODEMEYEESASEKGOSTERGE, 0);
        //        d.kazanilmisGosterge = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.KAZANILMISGOSTERGE, 0);
        //        d.kazanilmisEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.KAZANILMISEKGOSTERGE, 0);
        //        d.emekliyeEsasGosterge = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.EMEKLIYEESASGOSTERGE, 0);
        //        d.emekliyeEsasEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.EMEKLIYEESASEKGOSTERGE, 0);
        //        d.kadroDurum = (ENUMKadroDurumu)VeriErisim.DegerAl(r, (int)TABPersonelKadro.ENUMALANLAR.KADRODURUMU, 0);
        //        OrganizasyonBirim orgBirim = TNS.ORG.Arac.OrgBirimBul(d.organizasyonKodu);
        //        d.birimAd = orgBirim.adi;

        //        d.atama = new PersonelAtama();
        //        d.atama.kod = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + (int)TABPersonelAtama.ENUMAlanlar.KOD, 0);
        //        d.atama.pkkod = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + (int)TABPersonelAtama.ENUMAlanlar.PKKOD, 0);
        //        d.atama.atamaTuru = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + (int)TABPersonelAtama.ENUMAlanlar.ATAMATURU, 0);
        //        d.atama.atamaSekli = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + (int)TABPersonelAtama.ENUMAlanlar.ATAMASEKLI, 0);
        //        d.atama.atamaNedeni = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + (int)TABPersonelAtama.ENUMAlanlar.ATAMANEDENI, "");
        //        d.atama.atamaTarihi = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + (int)TABPersonelAtama.ENUMAlanlar.ATAMATARIHI);
        //        d.atama.onayTarihi = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + (int)TABPersonelAtama.ENUMAlanlar.ONAYTARIHI);
        //        d.atama.onaylayan = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + (int)TABPersonelAtama.ENUMAlanlar.ONAYLAYAN, "");
        //        d.atama.hareketDurumu = VeriErisim.DegerAl(r, kadroAlanlarSonIndex + (int)TABPersonelAtama.ENUMAlanlar.HAREKETDURUMU, 0);

        //        d.personelTCKimlikNo = VeriErisim.DegerAl(r, atamaAlanlarSonIndex + kadroAlanlarSonIndex, "");
        //        d.personelAdSoyad = VeriErisim.DegerAl(r, atamaAlanlarSonIndex + kadroAlanlarSonIndex + 1, "") + " " + VeriErisim.DegerAl(r, atamaAlanlarSonIndex + kadroAlanlarSonIndex + 2, "");
        //        d.personelTipAd = VeriErisim.DegerAl(r, atamaAlanlarSonIndex + kadroAlanlarSonIndex + 3, "");
        //        d.atama.atamaTuruAd = VeriErisim.DegerAl(r, atamaAlanlarSonIndex + kadroAlanlarSonIndex + 4, "");
        //        d.atama.atamaSekliAd = VeriErisim.DegerAl(r, atamaAlanlarSonIndex + kadroAlanlarSonIndex + 5, "");
        //        d.unvanAd = VeriErisim.DegerAl(r, atamaAlanlarSonIndex + kadroAlanlarSonIndex + 6, "");
        //        d.sinifAd = VeriErisim.DegerAl(r, atamaAlanlarSonIndex + kadroAlanlarSonIndex + 7, "");
        //        d.atama.hareketDurumuAd = VeriErisim.DegerAl(r, atamaAlanlarSonIndex + kadroAlanlarSonIndex + 8, "");
        //        d.atama.hareketDurumuKayitKod = VeriErisim.DegerAl(r, atamaAlanlarSonIndex + kadroAlanlarSonIndex + 9, "");

        //        pVAN.objeler.Add(d);
        //    }
        //    r.Close();

        //    pVAN.sonuc.islemSonuc = true;

        //    if (pVAN.objeler.Count == 0)
        //        pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

        //    ve.VeriErisimKapat();

        //    return pVAN;
        //}
    }
}

using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYMemuriyet
    {
        //public ObjectArray eskiTmp; //Tarihçe için

        //public ObjectArray ListeleMemuriyet(Kullanici kullanan, PersonelMemuriyet p)
        //{
        //    ObjectArray pVAN = new ObjectArray();
        //    pVAN.sonuc.DegerAta("Hazır değil");
        //    //if (!IKY.Yetki.YetkiVarMiOkumaya(kullanan))
        //    //{
        //    //    pVAN.sonuc.islemSonuc = false;
        //    //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

        //    //    return pVAN;
        //    //}

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    //TABMemuriyet tab = new TABMemuriyet(ve);

        //    //IDataReader r = null;

        //    //try
        //    //{
        //    //    r = tab.ListeleMemuriyet(p);
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    ve.VeriErisimKapat();
        //    //    pVAN.sonuc.islemSonuc = false;
        //    //    pVAN.sonuc.hataStr = e.Message;

        //    //    return pVAN;
        //    //}

        //    //while (r.Read())
        //    //{
        //    //    PersonelMemuriyet d = new PersonelMemuriyet();
        //    //    d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.KOD, "");
        //    //    d.siraNo = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.PKOD, 0);
        //    //    d.birimKodu = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.BIRIMKODU, "");
        //    //    d.birimAd = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.BIRIMAD, "");
        //    //    d.aciklama = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.ACIKLAMA, "");
        //    //    d.baslamaTarihi = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.BASLAMATARIHI);
        //    //    d.ayrilmaTarihi = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.AYRILISTARIHI);
        //    //    d.baslamaNedeni = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.BASLAMANEDENI, 0);
        //    //    d.ayrilmaNedeni = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.AYRILISNEDENI, 0);
        //    //    d.kadroDerece = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.KADRODERECE, 0);
        //    //    d.odemeyeEsasDerece = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.ODEMEYEESASDERECE, 0);
        //    //    d.odemeyeEsasKademe = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.ODEMEYEESASKADEME, 0);
        //    //    d.odemeyeEsasTarih = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.ODEMEYEESASTARIH);
        //    //    d.kazanilmisHakDerece = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.KAZANILMISHAKDERECE, 0);
        //    //    d.kazanilmisHakKademe = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.KAZANILMISHAKKADEME, 0);
        //    //    d.kazanilmisHakTarih = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.KAZANILMISHAKTARIH);
        //    //    d.emekliyeEsasDerece = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.EMEKLIYEESASDERECE, 0);
        //    //    d.emekliyeEsasKademe = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.EMEKLIYEESASKADEME, 0);
        //    //    d.emekliyeEsasTarih = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.EMEKLIYEESASTARIH);
        //    //    d.odemeyeEsasGosterge = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.ODEMEYEESASGOSTERGE, 0);
        //    //    d.odemeyeEsasEkGosterge = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.ODEMEYEESASEKGOSTERGE, 0);
        //    //    d.kazanilmisGosterge = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.KAZANILMISGOSTERGE, 0);
        //    //    d.kazanilmisEkGosterge = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.KAZANILMISEKGOSTERGE, 0);
        //    //    d.emekliyeEsasGosterge = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.EMEKLIYEESASGOSTERGE, 0);
        //    //    d.emekliyeEsasEkGosterge = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.EMEKLIYEESASEKGOSTERGE, 0);

        //    //    d.baslamaNedeniAd = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.EMEKLIYEESASEKGOSTERGE + 1, "");
        //    //    d.ayrilmaNedeniAd = VeriErisim.DegerAl(r, (int)TABMemuriyet.ENUMAlanlar.EMEKLIYEESASEKGOSTERGE + 2, "");
        //    //    pVAN.objeler.Add(d);
        //    //}
        //    //r.Close();

        //    //pVAN.sonuc.islemSonuc = true;

        //    //if (pVAN.objeler.Count == 0)
        //    //    pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

        //    //ve.VeriErisimKapat();

        //    return pVAN;
        //}

        //public Sonuc KaydetMemuriyet(Kullanici kullanan, PersonelMemuriyet p)
        //{
        //    Sonuc sonuc = new Sonuc("Hazır değil");
        //    //object eski = null, yeni = null; //Tarihçe için

        //    //if (!IKY.Yetki.YetkiVarMiYazmaya(kullanan))
        //    //{
        //    //    sonuc.islemSonuc = false;
        //    //    sonuc.hataStr = PersonelResI.PER011;

        //    //    return sonuc;
        //    //}

        //    //string hata = "";
        //    //if (p.tcKimlikNo.Trim() == "")
        //    //    hata += "Personel bilgisi boş bırakılamaz.<br>";
        //    //if (p.baslamaTarihi.isNull)
        //    //    hata += "Birime başlama tarihi boş bırakılamaz.<br>";
        //    //if (!p.ayrilmaTarihi.isNull)
        //    //{
        //    //    if (!p.baslamaTarihi.isNull && p.ayrilmaTarihi.Oku() < p.baslamaTarihi.Oku())
        //    //        hata += "Ayrılma tarihi, başlama tarihinden küçük olamaz.<br>";
        //    //}
        //    //else
        //    //{
        //    //    ObjectArray memuriyetListe = ListeleMemuriyet(kullanan, p);
        //    //    int sayac = 0;
        //    //    foreach (PersonelMemuriyet memuriyet in memuriyetListe.objeler)
        //    //    {
        //    //        if (memuriyet.ayrilmaTarihi.isNull)
        //    //            sayac++;
        //    //    }
        //    //    //if (sayac > 1)
        //    //    //{
        //    //    //    hata += "Birden fazla memuriyet bilgisi için Ayrılma tarihi boş bırakılamaz. Lütfen personelin önceki bilgileri için Ayrılma tarihi bilgisini kaydediniz.<br>";
        //    //    //}
        //    //}



        //    //if (hata != "")
        //    //{
        //    //    sonuc.hataStr = hata;
        //    //    sonuc.islemSonuc = false;
        //    //    return sonuc;
        //    //}

        //    ////Tarihçe ********************
        //    //if (p.siraNo > 0)
        //    //{
        //    //    this.eskiTmp = this.ListeleMemuriyet(kullanan, new PersonelMemuriyet() { tcKimlikNo = p.tcKimlikNo, siraNo = p.siraNo });
        //    //    if (eskiTmp.objeler.Count == 1)
        //    //        eski = eskiTmp[0];
        //    //}
        //    //if (eski == null)
        //    //    eski = new PersonelMemuriyet();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
        //    ////****************************

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    //ve.BeginTransaction();

        //    //TABMemuriyet tab = new TABMemuriyet(ve);

        //    //ENUMTARIHCEISLEMTIP islemTip;
        //    //if (p.siraNo == 0)
        //    //{
        //    //    islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
        //    //    sonuc = tab.KaydetMemuriyet(p);
        //    //}
        //    //else
        //    //{
        //    //    islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
        //    //    sonuc = tab.GuncelleMemuriyet(p);
        //    //}

        //    //if (sonuc.islemSonuc)
        //    //{
        //    //    ve.Commit();
        //    //    sonuc.bilgiStr = PersonelResI.PER007;
        //    //    MemuriyetTarihleriGuncelle(p.tcKimlikNo);
        //    //    //MemuriyetKadroBilgileriniGuncelle(kullanan, p.tcKimlikNo);
        //    //    if (!p.ayrilmaTarihi.isNull)
        //    //        MemuriyetPersonelAyrilmaGuncelle(p);
        //    //    else
        //    //        MemuriyetPersonelBirimGuncelle(p);

        //    //    //Tarihçe Kaydet ********************
        //    //    yeni = p;
        //    //    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.MEMURIYET, p.personelKod, islemTip);
        //    //}
        //    //else
        //    //    ve.Rollback();

        //    //ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public Personel ListeleMemuriyetTarihleri(string tcKimlikNo)
        //{
        //    throw new Exception("Hazır Değil");
        //    //Personel p = new Personel();

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
        //    //TABMemuriyet tab = new TABMemuriyet(ve);

        //    //IDataReader r = null;

        //    //try
        //    //{
        //    //    r = tab.ListeleMemuriyetTarihleri(tcKimlikNo);
        //    //}
        //    //catch
        //    //{
        //    //    ve.VeriErisimKapat();
        //    //    return p;
        //    //}

        //    //while (r.Read())
        //    //{
        //    //    p.ilkIseBaslamaTarihi = VeriErisim.DegerAl(r, 0);
        //    //    //p.sonBirimeBaslamaTarihi = VeriErisim.DegerAl(r, 1);
        //    //    //p.sonGoreveBaslamaTarihi = VeriErisim.DegerAl(r, 2);
        //    //}
        //    //r.Close();

        //    //ve.VeriErisimKapat();

        //    //return p;
        //}

        //public Sonuc SilMemuriyet(Kullanici kullanan, PersonelMemuriyet p)
        //{
        //    Sonuc sonuc = new Sonuc("Hazır değil");

        //    //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
        //    //{
        //    //    sonuc.islemSonuc = false;
        //    //    sonuc.hataStr = PersonelResI.PER011;

        //    //    return sonuc;
        //    //}

        //    //string hata = "";
        //    //if (p.tcKimlikNo.Trim() == "")
        //    //    hata += "Personel bilgisi boş bırakılamaz.<br>";

        //    //if (hata != "")
        //    //{
        //    //    sonuc.hataStr = hata;
        //    //    sonuc.islemSonuc = false;
        //    //    return sonuc;
        //    //}

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    //ve.BeginTransaction();

        //    //TABMemuriyet tab = new TABMemuriyet(ve);

        //    //sonuc = tab.SilMemuriyet(p);

        //    //if (sonuc.islemSonuc)
        //    //{
        //    //    ve.Commit();
        //    //    sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
        //    //    MemuriyetTarihleriGuncelle(p.tcKimlikNo);
        //    //}
        //    //else
        //    //    ve.Rollback();

        //    //ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public void MemuriyetKadroBilgileriniGuncelle(Kullanici kullanan, string tcKimlikNo)
        //{
        //    PersonelMemuriyet p = new PersonelMemuriyet();
        //    p.tcKimlikNo = tcKimlikNo;
        //    //Personelin memuriyet listesi alınıyor
        //    ObjectArray pler = ListeleMemuriyet(kullanan, p);

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABMemuriyet tab = new TABMemuriyet(ve);
        //    TABPersonel tabP = new TABPersonel(ve);

        //    foreach (PersonelMemuriyet pp in pler.objeler)
        //    {
        //        p = new PersonelMemuriyet();
        //        p.tcKimlikNo = tcKimlikNo;
        //        p.siraNo = pp.siraNo;
        //        TNSDateTime ayrilisTarihi = new TNSDateTime();
        //        if (pp.ayrilmaTarihi.isNull)
        //            ayrilisTarihi = new TNSDateTime(DateTime.Now.ToString());
        //        else
        //            ayrilisTarihi = pp.ayrilmaTarihi;

        //        IDataReader dr = tabP.MemuriyeteUygunKadroBilgileriniAl(p.tcKimlikNo, ayrilisTarihi, 1);
        //        if (dr.Read())
        //        {
        //            p.kadroDerece = VeriErisim.DegerAl(dr, 0, 0);
        //            p.kadroTarih = VeriErisim.DegerAl(dr, 1);
        //        }
        //        dr.Close();

        //        dr = tabP.MemuriyeteUygunKadroBilgileriniAl(p.tcKimlikNo, ayrilisTarihi, 2);
        //        if (dr.Read())
        //        {
        //            p.odemeyeEsasDerece = VeriErisim.DegerAl(dr, 0, 0);
        //            p.odemeyeEsasKademe = VeriErisim.DegerAl(dr, 1, 0);
        //            p.odemeyeEsasTarih = VeriErisim.DegerAl(dr, 2);
        //            p.odemeyeEsasGosterge = VeriErisim.DegerAl(dr, 3, 0);
        //            p.odemeyeEsasEkGosterge = VeriErisim.DegerAl(dr, 4, 0);
        //        }
        //        dr.Close();

        //        dr = tabP.MemuriyeteUygunKadroBilgileriniAl(p.tcKimlikNo, ayrilisTarihi, 3);
        //        if (dr.Read())
        //        {
        //            p.kazanilmisHakDerece = VeriErisim.DegerAl(dr, 0, 0);
        //            p.kazanilmisHakKademe = VeriErisim.DegerAl(dr, 1, 0);
        //            p.kazanilmisHakTarih = VeriErisim.DegerAl(dr, 2);
        //            p.kazanilmisGosterge = VeriErisim.DegerAl(dr, 3, 0);
        //            p.kazanilmisEkGosterge = VeriErisim.DegerAl(dr, 4, 0);
        //        }
        //        dr.Close();

        //        dr = tabP.MemuriyeteUygunKadroBilgileriniAl(p.tcKimlikNo, ayrilisTarihi, 4);
        //        if (dr.Read())
        //        {
        //            p.emekliyeEsasDerece = VeriErisim.DegerAl(dr, 0, 0);
        //            p.emekliyeEsasKademe = VeriErisim.DegerAl(dr, 1, 0);
        //            p.emekliyeEsasTarih = VeriErisim.DegerAl(dr, 2);
        //            p.emekliyeEsasGosterge = VeriErisim.DegerAl(dr, 3, 0);
        //            p.emekliyeEsasEkGosterge = VeriErisim.DegerAl(dr, 4, 0);
        //        }
        //        dr.Close();

        //        tab.MemuriyetKadroBilgileriniGuncelle(p);
        //    }

        //    ve.VeriErisimKapat();
        //}

        //public void MemuriyetPersonelAyrilmaGuncelle(PersonelMemuriyet p)
        //{
            //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            //TABMemuriyet tab = new TABMemuriyet(ve);

            //TNSDateTime memuriyetBaslamaTarihi = tab.MemuriyetTarihiAl(p.tcKimlikNo, 3);
            //if (memuriyetBaslamaTarihi.Oku() == p.baslamaTarihi.Oku())
            //    tab.MemuriyetPersonelAyrilmaGuncelle(p.tcKimlikNo);
            //ve.VeriErisimKapat();
        //}

        //public void MemuriyetPersonelBirimGuncelle(PersonelMemuriyet p)
        //{
            //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            //TABMemuriyet tab = new TABMemuriyet(ve);

            //TNSDateTime memuriyetBaslamaTarihi = tab.MemuriyetTarihiAl(p.tcKimlikNo, 3);
            //if (memuriyetBaslamaTarihi.Oku() == p.baslamaTarihi.Oku() && p.ayrilmaTarihi.isNull)
            //    if (p.muhasebeKodu > 0 || p.organizasyonKodu > 0)
            //        tab.MemuriyetPersonelBirimGuncelle(p);
            //ve.VeriErisimKapat();
        //}

        //public void MemuriyetTarihleriGuncelle(string tcKimlikNo)
        //{
            //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            //TABMemuriyet tab = new TABMemuriyet(ve);

            //TNSDateTime memuriyetBaslamaTarihi = tab.MemuriyetTarihiAl(tcKimlikNo, 1);
            //TNSDateTime sonGoreveBaslamaTarihi = tab.MemuriyetTarihiAl(tcKimlikNo, 2);
            //TNSDateTime sonBirimeBaslamaTarihi = tab.MemuriyetTarihiAl(tcKimlikNo, 3);
            //tab.MemuriyetTarihleriGuncelle(tcKimlikNo, memuriyetBaslamaTarihi, sonBirimeBaslamaTarihi, sonGoreveBaslamaTarihi);
            //ve.VeriErisimKapat();
            //sonBirimeBaslamaTarihi eksik hesaplanıyor. 
            //Son birim başlama tarihi update et- PERPERSONEL tablosunda SONBIRIMBASLAMATARIHI alanı
        //} 
    }
}

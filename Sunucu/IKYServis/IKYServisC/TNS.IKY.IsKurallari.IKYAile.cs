using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYAile
    {
        public ObjectArray eskiTmp; //Tarihçe için

        internal static ObjectArray ListeleAile(string okunanXML)
        {
            ObjectArray pVAN = new ObjectArray();

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);

            if (r != null)
            {
                int sonKolon = (Enum.GetNames(typeof(TABAile.ENUMAlanlarAile))).Length;

                while (r.Read())
                {
                    PersonelAile d = new PersonelAile();
                    d.kod = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.KOD, 0);
                    d.personelKod = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.PKOD, 0);
                    d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.TCKIMLIKNO, "");
                    d.ad = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.AD, "");
                    d.soyad = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.SOYAD, "");
                    d.cinsiyet = (ENUMCINSIYET)VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.CINSIYET, 0);
                    d.yakinlik = (ENUMYakinlikDerecesi)VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.YAKINLIKDERECESI, 0);
                    d.durum = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.DURUM, 0);
                    d.dogumTarihi = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.DOGUMTARIHI);
                    d.bakmaklaYukumlu = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.BAKMAKLAYUKUMLU, 0);
                    d.yukumlulukBitisTarihi = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.YUKUMLULUKBITISTARIHI);
                    d.ogrenimTipi = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.OGRENIMTIPI, 0);
                    d.meslek = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.MESLEK, "");
                    d.sakatlikDerecesi = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.SAKATLIKDERECESI, 0);
                    d.agi = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.AGI, 0);
                    d.yas = VeriErisim.DegerAl(r, sonKolon + 0, 0);
                    d.ogrenimAd = VeriErisim.DegerAl(r, sonKolon + 1, "");
                    d.durumKayitKod = VeriErisim.DegerAl(r, sonKolon + 2, "");
                    d.durumAd = VeriErisim.DegerAl(r, sonKolon + 3, "");
                    pVAN.objeler.Add(d);
                }
                r.Close();
            }
            return pVAN;
        }

        public ObjectArray ListeleAile(VeriErisim veriErisim, Kullanici kullanan, int aileKod, int personelKod, string personelKodlar, string personelTur, string organizasyonKopruKodu, int yukumlu)
        {
            ObjectArray pVAN = new ObjectArray();

            IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, personelKod, pVAN.sonuc, ENUMYetkiIslem.Aile);
            if (!pVAN.sonuc.islemSonuc)
            {
                return pVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAile tab = new TABAile(ve);

            IDataReader r = tab.ListeleAile(aileKod, personelKod, yukumlu);

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            int sonKolon = (Enum.GetNames(typeof(TABAile.ENUMAlanlarAile))).Length;

            while (r.Read())
            {
                PersonelAile d = new PersonelAile();
                d.kod = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.KOD, 0);
                d.personelKod = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.PKOD, 0);
                d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.TCKIMLIKNO, "");
                d.ad = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.AD, "");
                d.soyad = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.SOYAD, "");
                d.cinsiyet = (ENUMCINSIYET)VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.CINSIYET, 0);
                d.yakinlik = (ENUMYakinlikDerecesi)VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.YAKINLIKDERECESI, 0);
                d.durum = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.DURUM, 0);
                d.dogumTarihi = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.DOGUMTARIHI);
                d.bakmaklaYukumlu = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.BAKMAKLAYUKUMLU, 0);
                d.yukumlulukBitisTarihi = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.YUKUMLULUKBITISTARIHI);
                d.ogrenimTipi = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.OGRENIMTIPI, 0);
                d.meslek = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.MESLEK, "");
                d.sakatlikDerecesi = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.SAKATLIKDERECESI, 0);
                d.agi = VeriErisim.DegerAl(r, (int)TABAile.ENUMAlanlarAile.AGI, 0);
                d.yas = VeriErisim.DegerAl(r, sonKolon + 0, 0);
                d.ogrenimAd = VeriErisim.DegerAl(r, sonKolon + 1, "");
                d.durumKayitKod = VeriErisim.DegerAl(r, sonKolon + 2, "");
                d.durumAd = VeriErisim.DegerAl(r, sonKolon + 3, "");
                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            else
            {
                //Tarihçe Kaydet ********************
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, new PersonelAile(), new PersonelAile { personelKod = personelKod, kod = aileKod }, ENUMTARIHCE.AILE, personelKod, ENUMTARIHCEISLEMTIP.LISTELE);
            }

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc KaydetAile(Kullanici kullanan, PersonelAile p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Aile);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            IKYPersonel personel = new IKYPersonel();

            string hata = "";
            if (string.IsNullOrWhiteSpace(p.tcKimlikNo))
                hata = "Aile Bireyinin T.C Numarası boş bırakılamaz.<br>";
            if (p.personelKod == 0)
                hata += "Personel bilgisi boş bırakılamaz.<br>";
            if (string.IsNullOrWhiteSpace(p.ad))
                hata += "Aile Bireyinin adı boş bırakılamaz.<br>";
            if (string.IsNullOrWhiteSpace(p.soyad))
                hata += "Aile Bireyinin soyadı boş bırakılamaz.<br>";
            if (!Personel.TCKimlikNoKontrolu(kullanan, p.tcKimlikNo, p.dogumTarihi))
                hata += "T.C Kimlik numarası " + p.tcKimlikNo + " hatalı veya yazılan Ad Soyad ile uyumsuz veya Kimlik doğrulama servisi ile bağlantı kurulamıyor.";
            if (p.dogumTarihi == null || p.dogumTarihi.isNull)
                hata += "Aile bireyinin doğum tarihi boş bırakılamaz.<br>";
            if (p.cinsiyet == 0)
                hata += "Aile bireyinin cinsiyeti boş bırakılamaz.<br>";
            if (p.yakinlik == 0)
                hata += "Aile bireyinin yakınlık derecesi boş bırakılamaz.<br>";
            if (p.durum == 0)
                hata += "Aile bireyinin durumu boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            //Tarihçe Kaydet ********************

            if (p.kod > 0)
            {
                this.eskiTmp = this.ListeleAile(ve, kullanan, p.kod, p.personelKod, null, null, null, 0);
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelAile();//  "Yeni Kayıt"
            //****************************

            ve.BeginTransaction();

            TABAile tab = new TABAile(ve);

            ENUMTARIHCEISLEMTIP islemTip;

            islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
            if (p.kod == 0)
            {
                p.kod = TABGenel.SayacDegerAlInt(ve, TABAile.TABLOADAILE);
                sonuc = tab.KaydetAile(p);
            }
            else
            {
                sonuc = tab.GuncelleAile(p);
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                sonuc.anahtar = p.kod.ToString();

                yeni = p;
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.AILE, p.personelKod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilAile(Kullanici kullanan, PersonelAile p)
        {
            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.SIL;
            object eski = null, yeni = null; //Tarihçe için
            Sonuc sonuc = new Sonuc();

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Aile);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            string hata = "";
            if (p.kod == 0)
                hata = "Silmek istediğiniz aile bireyini seçiniz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            //Tarihçe ********************
            if (p.kod > 0)
            {
                this.eskiTmp = this.ListeleAile(ve, kullanan, p.kod, p.personelKod, null, null, null, 0);
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelAile();//  "Yeni Kayıt"
            //****************************

            ve.BeginTransaction();

            TABBelgeler tabBelge = new TABBelgeler(ve);
            sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod = p.personelKod, tip = (int)ENUMBELGEFORMTIP.PERSONELAILE });

            if (sonuc.islemSonuc)
            {
                TABAile tab = new TABAile(ve);
                sonuc = tab.SilAile(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                //Tarihçe Kaydet ********************
                yeni = new PersonelAile();               

                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.AILE, p.personelKod, islemTip);

                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

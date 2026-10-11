using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYKart
    {
        public ObjectArray eskiTmp; //Tarihçe için

        public ObjectArray ListeleKart(Kullanici kullanan, PersonelKart p)
        {
            ObjectArray pVAN = new ObjectArray();
            pVAN.sonuc = IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, p.personelKod, null, ENUMYetkiIslem.Kartlar);
            if (!pVAN.sonuc.islemSonuc)
                return pVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKart tab = new TABKart(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleKart(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            int sonKolon = (Enum.GetNames(typeof(TABKart.ENUMAlanlarKart))).Length; // ((int)TABKart.ENUMAlanlarKart.KayipTarihi) + 1;
        
            while (r.Read())
            {
                PersonelKart d = new PersonelKart();
                d.kod = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.KOD, 0);
                d.personelKod = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.PKOD, 0);
                d.zimmetTarihi = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.ZimmetTarihi);
                d.iadeTarihi = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.IadeTarihi);
                d.kartTipi = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.kartTipi, 0);
                d.kartNo = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.kartNo, "");
                d.yakinligi = (ENUMYakinlikDerecesi)VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.Yakinligi, 0);
                d.plaka = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.Plaka, "");
                d.adiSoyadi = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.AdiSoyadi,"");
                d.tagCihazNo = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.TagCihazNo, "");
                d.zimmetEdenPersonel = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.ZimmetEdenPersonel, 0);
                d.iadeAlanPersonel = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.IadeAlanPersonel,0);
                d.aciklama = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.Aciklama, "");
                d.aktifMi = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.AktifMi, 0);
                d.kayipTarihi = VeriErisim.DegerAl(r, (int)TABKart.ENUMAlanlarKart.KayipTarihi);
                d.kartTipiAd = VeriErisim.DegerAl(r, sonKolon, "");
                d.zimmetEdenPersonelAd = VeriErisim.DegerAl(r, sonKolon + 1, "");
                d.iadeAlanPersonelAd = VeriErisim.DegerAl(r, sonKolon + 2, "");

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            else
            {
                //Tarihçe Kaydet ********************
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, new PersonelKart(), p, ENUMTARIHCE.KART, p.personelKod, ENUMTARIHCEISLEMTIP.LISTELE);
            }

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc KaydetKart(Kullanici kullanan, PersonelKart p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Kartlar);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = "";
    
            if (p.personelKod == 0)
                hata += "Personel bilgisi boş bırakılamaz.<br>";     

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            this.eskiTmp = this.ListeleKart(kullanan, p);
            if (eskiTmp.objeler.Count == 1)
                eski = eskiTmp[0];

            if (eski == null)
                eski = new PersonelKart();// { kod = "0" }; // 0 : "Yeni Kayıt"
            ////****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABKart tab = new TABKart(ve);

            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
            if (p.kod == 0)
            {
                p.kod = TABGenel.SayacDegerAlInt(ve, TABKart.TABLOADKART);
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
            }
            sonuc = tab.KaydetKart(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.KART, p.personelKod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilKart(Kullanici kullanan, PersonelKart p)
        {
            Sonuc sonuc = new Sonuc();
            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.SIL;
            object eski = null, yeni = null; //Tarihçe için           

            string hata = "";
            if (p.kod == 0)
                hata = "Silmek istediğiniz kartı seçiniz.<br>";
            if (p.personelKod == 0)
                hata = "Silmek istediğiniz kartın hangi personele ait olduğu belirsiz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }  
            
            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Kartlar);
            if (!sonuc.islemSonuc)
                return sonuc;

            //Tarihçe ********************
            this.eskiTmp = this.ListeleKart(kullanan, p);
            if (eskiTmp.objeler.Count == 1)
                eski = eskiTmp[0];

            if (eski == null)
                eski = new PersonelKart();// { kod = "0" }; // 0 : "Yeni Kayıt"
            ////****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            //TABBelgeler tabBelge = new TABBelgeler(ve);
            //sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod=p.personelKod, tip=(int)ENUMBELGEFORMTIP.PERSONELAILE });

            //if (sonuc.islemSonuc)
            //{
                TABKart tab = new TABKart(ve);
                sonuc = tab.SilKart(p);                
            //}

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
                //Tarihçe Kaydet ********************
                yeni = new PersonelKart();
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.KART, p.personelKod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

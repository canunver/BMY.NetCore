using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYKurs
    {
        public ObjectArray eskiTmp; //Tarihçe için

        public ObjectArray Listele(Kullanici kullanan, PersonelKurs p)
        {
            ObjectArray pVAN = new ObjectArray();

            IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, p.personelKod, pVAN.sonuc, ENUMYetkiIslem.HizmetIciEgitim);
            if (!pVAN.sonuc.islemSonuc)
            {
                return pVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKurs tab = new TABKurs(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(p);
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
                PersonelKurs k = new PersonelKurs();
                k.kod = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.KOD, 0);
                k.personelKod = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.PKOD, 0);
                k.kursKod = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.KURSKODU, 0);
                k.tarih = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.TARIH);
                k.aldigiYer = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.ALDIGIYER, "");
                k.baslamaTarihi = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.BASLAMATARIHI);
                k.bitirmeTarihi = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.BITIRMETARIHI);
                k.puan = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.PUAN, 0.0);
                k.derece = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.DERECE, "");
                k.kursAdi = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.KURSADI, "");
                k.dersSaati = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.DERSSAATI, 0);
                k.egitimYeri = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.EGITIMYERI, 0);
                k.yil = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.YIL, 0);
                k.donem = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.DONEM, "");
                k.gun = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.GUN, 0);
                k.dosyaNo = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.DOSYANO, "");
                k.aciklama = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.ACIKLAMA, "");
                k.kursKodHitapID = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.ACIKLAMA + 1, "");
                k.ogrenimYeriHitapID = VeriErisim.DegerAl(r, (int)TABKurs.ENUMAlanlar.ACIKLAMA + 2, "");
                pVAN.objeler.Add(k);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            else
            {
                //Tarihçe Kaydet ********************
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, new PersonelKurs(), p, ENUMTARIHCE.EGITIM, p.personelKod, ENUMTARIHCEISLEMTIP.LISTELE);
            }

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc Kaydet(Kullanici kullanan, PersonelKurs p)
        {
            Sonuc sonuc = new Sonuc();

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.HizmetIciEgitim);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            object eski = null, yeni = null; //Tarihçe için  

            string hata = "";
            //if (p.kursKod == 0)
            //    hata = "Kurs bilgisi boş bırakılamaz.<br>";
            if (p.personelKod <= 0)
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.kod > 0)
            {
                this.eskiTmp = this.Listele(kullanan, p);
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelKurs();//  "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABKurs tab = new TABKurs(ve);
            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;

            if (p.kod > 0)
                sonuc = tab.Guncelle(p);
            else
            {
                p.kod = TABGenel.SayacDegerAlInt(ve, TABKurs.TABLOAD);
                sonuc = tab.Kaydet(p);
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                sonuc.anahtar = p.kod.ToString();
                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, PersonelKurs p)
        {
            Sonuc sonuc = new Sonuc();

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.HizmetIciEgitim);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            object eski = null, yeni = null; //Tarihçe için

            string hata = "";
            //if (p.kursKodu == 0)
            //    hata = "Kurs bilgisi boş bırakılamaz.<br>";
            if (p.personelKod < 0)
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.kod > 0)
            {
                this.eskiTmp = this.Listele(kullanan, p);
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelKurs();//  "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABBelgeler tabBelge = new TABBelgeler(ve);
            sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod = p.personelKod, tip = (int)ENUMBELGEFORMTIP.PERSONELKURS });

            if (sonuc.islemSonuc)
            {
                TABKurs tab = new TABKurs(ve);
                sonuc = tab.Sil(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";

                //Tarihçe Kaydet ********************
                yeni = new PersonelKurs();
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, ENUMTARIHCEISLEMTIP.SIL);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using System.Resources;

namespace TNS.IKY.IsKurallari
{
    class IKYOgrenim
    {
        public ObjectArray eskiTmp; //Tarihçe için

        public static ObjectArray Listele(string okunanXML)
        {
            ObjectArray pVAN = new ObjectArray();

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);

            if (r != null)
            {
                int alanlarSonIndex = (Enum.GetNames(typeof(TABPersonelOgrenim.ENUMAlanlar))).Length;

                while (r.Read())
                {
                    PersonelOgrenim po = new PersonelOgrenim();

                    po.kod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.KOD, 0);
                    po.personelKod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.PKOD, 0);
                    po.ogrenimKod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.OGRENIMKOD, 0);
                    po.universiteKod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.UNIVERSITEKOD, 0);
                    po.fakulteKod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.FAKULTEKOD, 0);
                    po.bolumKod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.BOLUMKOD, 0);
                    po.okulAdi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.OKULADI, "");
                    po.belgeTur = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.BELGETUR, 0);
                    po.belgeTarihi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.BELGETARIHI);
                    po.belgeNo = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.BELGENO, "");
                    po.baslangicTarihi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.BASLANGICTARIHI);
                    po.derece = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.DERECE, 0);
                    po.gecerli = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.GECERLI, 0);
                    po.ogrenimYeri = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.OGRENIMYERI, 0);
                    po.denklik = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.DENKLIK, 0);
                    po.denklikTarihi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.DENKLIKTARIHI);
                    po.denkOkul = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.DENKOKUL, "");
                    po.denkBolum = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.DENKBOLUM, "");
                    po.hazirlikVarMi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.HAZIRLIK, 0);
                    po.kurumaIbrazTarihi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.KURUMAIBRAZTARIHI);
                    po.memuriyeteBasladigiOkul = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.MEMURIYETEGIRISOKULU, 0);
                    po.ogrenimSuresi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.OGRENIMSURESI, 0);
                    po.ogrenimKodKayitKod = VeriErisim.DegerAl(r, alanlarSonIndex, "");
                    po.ogrenimKodHitapID = VeriErisim.DegerAl(r, alanlarSonIndex + 1, "");
                    //po.ogrenimYeriHitapID = VeriErisim.DegerAl(r, alanlarSonIndex+2, "");
                    po.ogrenimAdi = VeriErisim.DegerAl(r, alanlarSonIndex + 3, "");
                    po.sonOkul = VeriErisim.DegerAl(r, alanlarSonIndex + 4, 0);
                    pVAN.objeler.Add(po);
                }
                r.Close();
            }
            pVAN.sonuc.islemSonuc = true;
            return pVAN;
        }

        public ObjectArray Listele(Kullanici kullanan, PersonelOgrenim p)
        {
            ObjectArray pVAN = new ObjectArray();

            IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, p.personelKod, pVAN.sonuc, ENUMYetkiIslem.Ogrenim);
            if (!pVAN.sonuc.islemSonuc)
            {
                return pVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABPersonelOgrenim tab = new TABPersonelOgrenim(ve);

            IDataReader r = tab.Listele(p);

            if(r==null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);

                return pVAN;
            }

            int alanlarSonIndex = (Enum.GetNames(typeof(TABPersonelOgrenim.ENUMAlanlar))).Length;

            while (r.Read())
            {
                PersonelOgrenim po = new PersonelOgrenim();

                po.kod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.KOD, 0);
                po.personelKod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.PKOD, 0);
                po.ogrenimKod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.OGRENIMKOD, 0);
                po.universiteKod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.UNIVERSITEKOD, 0);
                po.fakulteKod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.FAKULTEKOD, 0);
                po.bolumKod = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.BOLUMKOD, 0);
                po.okulAdi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.OKULADI, "");
                po.belgeTur = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.BELGETUR, 0);
                po.belgeTarihi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.BELGETARIHI);
                po.belgeNo = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.BELGENO, "");
                po.baslangicTarihi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.BASLANGICTARIHI);
                po.derece = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.DERECE, 0);
                po.gecerli = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.GECERLI, 0);
                po.ogrenimYeri = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.OGRENIMYERI, 0);
                po.denklik = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.DENKLIK, 0);
                po.denklikTarihi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.DENKLIKTARIHI);
                po.denkOkul = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.DENKOKUL, "");
                po.denkBolum = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.DENKBOLUM, "");
                po.hazirlikVarMi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.HAZIRLIK, 0);
                po.kurumaIbrazTarihi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.KURUMAIBRAZTARIHI);
                po.memuriyeteBasladigiOkul = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.MEMURIYETEGIRISOKULU, 0);
                po.ogrenimSuresi = VeriErisim.DegerAl(r, (int)TABPersonelOgrenim.ENUMAlanlar.OGRENIMSURESI, 0);
                po.ogrenimKodKayitKod = VeriErisim.DegerAl(r, alanlarSonIndex, "");
                po.ogrenimKodHitapID = VeriErisim.DegerAl(r, alanlarSonIndex+1, "");
                //po.ogrenimYeriHitapID = VeriErisim.DegerAl(r, alanlarSonIndex+2, "");
                po.ogrenimAdi = VeriErisim.DegerAl(r, alanlarSonIndex+3, "");
                po.sonOkul = VeriErisim.DegerAl(r, alanlarSonIndex+4, 0);
                pVAN.objeler.Add(po);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            else
            {
                //Tarihçe Kaydet ********************
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, new PersonelOgrenim(), p, ENUMTARIHCE.OGRENIM, p.personelKod, ENUMTARIHCEISLEMTIP.LISTELE);
            }

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc Kaydet(Kullanici kullanan, PersonelOgrenim p)
        {
            Sonuc sonuc = new Sonuc();

            object eski = null, yeni = null; //Tarihçe için

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Ogrenim);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            //Tarihçe Kaydet ********************

            if (p.kod > 0)
            {
                this.eskiTmp = this.Listele(kullanan, p);
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelOgrenim();//  "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonelOgrenim tab = new TABPersonelOgrenim(ve);
            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
      
            if (p.kod > 0)
                sonuc = tab.Guncelle(p);
            else
            {
                p.kod = TABGenel.SayacDegerAlInt(ve, TABPersonelOgrenim.TABLOAD);
                sonuc = tab.Kaydet(p);
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                sonuc.anahtar = p.kod.ToString();
                yeni = p;
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.OGRENIM, p.personelKod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, PersonelOgrenim p)
        {
            Sonuc sonuc = new Sonuc(); 
            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.SIL;
            object eski = null, yeni = null; //Tarihçe için

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Ogrenim);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            if (p.kod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
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
                eski = new PersonelOgrenim();//  "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABBelgeler tabBelge = new TABBelgeler(ve);
            sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod = p.personelKod, tip = (int)ENUMBELGEFORMTIP.PERSONELOGRENIM });

            if (sonuc.islemSonuc)
            {
                TABPersonelOgrenim tab = new TABPersonelOgrenim(ve);
                sonuc = tab.Sil(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";

                //Tarihçe Kaydet ********************
                yeni = new PersonelOgrenim();
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.OGRENIM, p.personelKod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        //public static ObjectArray OgrenimBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        //{
        //    ObjectArray donusDeger = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABOgrenim tab = new TABOgrenim(ve);

        //    IDataReader r = null;

        //    try
        //    {
        //        r = tab.OgrenimBazliPersonelSayilariListele(muhasebeKod, birimKod);
        //    }
        //    catch (Exception e)
        //    {
        //        ve.VeriErisimKapat();
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.Message;

        //        return donusDeger;
        //    }

        //    while (r.Read())
        //    {
        //        OgrenimBazliPersonelSayi gelen = new OgrenimBazliPersonelSayi();

        //        gelen.ogrenim = new Ogrenim();
        //        gelen.ogrenim.kod = VeriErisim.DegerAl(r, 0, 0);
        //        gelen.ogrenim.ad = VeriErisim.DegerAl(r, 1, string.Empty);
        //        gelen.personelSayi = VeriErisim.DegerAl(r, 2, 0);

        //        donusDeger.objeler.Add(gelen);
        //    }
        //    r.Close();

        //    if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
        //        donusDeger.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

        //    ve.VeriErisimKapat();
        //    return donusDeger;
        //}

        private string Kontrol(PersonelOgrenim p)
        {
            string hata = "";

            if (string.IsNullOrEmpty(p.ogrenimKod.ToString()))
                hata = "Eğitim seviyesi boş bırakılamaz." + "<br>";
            if (string.IsNullOrEmpty(p.baslangicTarihi.ToString()))
                hata += "Başlama Tarihi boş bırakılamaz" + "<br>";
            if (string.IsNullOrEmpty(p.bitirmeTarihi.ToString()))
                hata += "Bitirme Tarihi boş bırakılamaz" + "<br>";
            if (p.belgeTur <= 0)
                hata += "Belge Türü boş bırakılamaz." + "<br>";
            if (p.ogrenimYeri <= 0)
                hata += "Öğrenim Yeri boş bırakılamaz." + "<br>";
            return hata;
        }
    }
}
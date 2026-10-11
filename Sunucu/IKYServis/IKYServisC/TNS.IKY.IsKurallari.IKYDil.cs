using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYDil
    {
        public ObjectArray eskiTmp; //Tarihçe için

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PersonelDil p)
        {
            ObjectArray pVAN = new ObjectArray();

            IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, p.personelKod, pVAN.sonuc, ENUMYetkiIslem.YabanciDil);
            if (!pVAN.sonuc.islemSonuc)
            {
                return pVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABPersonelDil tab = new TABPersonelDil(ve);

            IDataReader r = tab.Listele(p);

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            int sonKolon = (Enum.GetNames(typeof(TABPersonelDil.ENUMAlanlar))).Length;

            while (r.Read())
            {
                PersonelDil d = new PersonelDil();
                d.kod = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.KOD, 0);
                d.personelKod = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.PKOD, 0);
                d.dilKod = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.DILKOD, 0);
                d.puan = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.PUAN, 0.0);
                d.tarih = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.TARIH);
                d.duzey = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.DUZEY, "");
                d.donemAy = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.DONEMAY, 0);
                d.donemYil = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.DONEMYIL, 0);
                d.belgeTur = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.BELGETUR, 0);
                d.belgeTarihi = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.BELGETARIHI);
                d.belgeNo = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.BELGENO, "");
                d.bordroyaEsas = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.BORDROYAESAS, 0);
                d.dilSeviyesi = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.DILSEVIYESI, "");
                d.kurumFaydalaniyor = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.KURUMFAYDALANIYOR, 0);
                d.dilAdi = VeriErisim.DegerAl(r, sonKolon, "");
                d.belgeAdi = VeriErisim.DegerAl(r, sonKolon + 1, "");

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            else
            {
                //Tarihçe Kaydet ********************
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, new PersonelDil(), p, ENUMTARIHCE.YABANCIDIL, p.personelKod, ENUMTARIHCEISLEMTIP.LISTELE);
            }

            ve.VeriErisimKapat();

            return pVAN;
        }

        public static ObjectArray Listele(string okunanXML)
        {
            ObjectArray pVAN = new ObjectArray();
            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                int sonKolon = (Enum.GetNames(typeof(TABPersonelDil.ENUMAlanlar))).Length;
                while (r.Read())
                {
                    PersonelDil d = new PersonelDil();
                    d.kod = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.KOD, 0);
                    d.personelKod = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.PKOD, 0);
                    d.dilKod = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.DILKOD, 0);
                    d.puan = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.PUAN, 0.0);
                    d.tarih = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.TARIH);
                    d.duzey = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.DUZEY, "");
                    d.donemAy = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.DONEMAY, 0);
                    d.donemYil = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.DONEMYIL, 0);
                    d.belgeTur = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.BELGETUR, 0);
                    d.belgeTarihi = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.BELGETARIHI);
                    d.belgeNo = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.BELGENO, "");
                    d.bordroyaEsas = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.BORDROYAESAS, 0);
                    d.dilSeviyesi = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.DILSEVIYESI, "");
                    d.kurumFaydalaniyor = VeriErisim.DegerAl(r, (int)TABPersonelDil.ENUMAlanlar.KURUMFAYDALANIYOR, 0);
                    d.dilAdi = VeriErisim.DegerAl(r, sonKolon, "");
                    d.belgeAdi = VeriErisim.DegerAl(r, sonKolon + 1, "");

                    pVAN.objeler.Add(d);
                }
                r.Close();
            }
            pVAN.sonuc.islemSonuc = true;
            return pVAN;
        }

        public Sonuc Kaydet(Kullanici kullanan, PersonelDil p)
        {
            object eski = null, yeni = null; //Tarihçe için

            Sonuc sonuc = new Sonuc();

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.YabanciDil);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            string hata = "";
            hata = Kontrol(p);

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            //Tarihçe ********************
            this.eskiTmp = Listele(ve, kullanan, p);
            if (eskiTmp.objeler.Count == 1)
                eski = eskiTmp[0];

            if (eski == null)
                eski = new PersonelDil();//  : "Yeni Kayıt"
            //****************************
            
            ve.BeginTransaction();

            TABPersonelDil tab = new TABPersonelDil(ve);

            ENUMTARIHCEISLEMTIP islemTip;
            if (p.kod > 0)
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.Guncelle(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                p.kod = TABGenel.SayacDegerAlInt(ve, TABPersonelDil.TABLOAD);
                sonuc = tab.Kaydet(p);
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                yeni = p;
                sonuc.anahtar = p.kod.ToString();
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.YABANCIDIL, p.personelKod, islemTip);

            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, PersonelDil p)
        {
            Sonuc sonuc = new Sonuc(); 
            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.SIL;
            object eski = null, yeni = null; //Tarihçe için

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.YabanciDil);
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

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            //Tarihçe ********************
            this.eskiTmp = Listele(ve, kullanan, p);
            if (eskiTmp.objeler.Count == 1)
                eski = eskiTmp[0];

            if (eski == null)
                eski = new PersonelDil();//  : "Yeni Kayıt"
            //****************************

            ve.BeginTransaction();

            TABBelgeler tabBelge = new TABBelgeler(ve);
            sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod = p.personelKod, tip = (int)ENUMBELGEFORMTIP.PERSONELDIL });

            if (sonuc.islemSonuc)
            {
                TABPersonelDil tab = new TABPersonelDil(ve);
                sonuc = tab.Sil(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
                
                //Tarihçe Kaydet ********************
                yeni = new PersonelDil();
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.YABANCIDIL, p.personelKod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        private string Kontrol(PersonelDil p)
        {
            string hata = "";

            //if (p.dilKod <= 0)
            //    hata = "Yabancı dil alanı boş bırakılamaz.";

            //if (p.belgeTur <= 0)
            //    hata += "<br>Yabancı dil belge tür alanı boş bırakılamaz.";

            return hata;
        }
    }
}

using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYCezaOdul
    {
        public ObjectArray eskiTmp; //Tarihçe için


        internal static ObjectArray Listele(string okunanXML)
        {
            if (string.IsNullOrWhiteSpace(okunanXML)) return null;
            ObjectArray pVAN = new ObjectArray();

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            int sonKolon = ((int)TABCezaOdul.ENUMALANLARCEZAODUL.Dayanak) + 1;
            if (r != null)
            {
                while (r.Read())
                {
                    pVAN.objeler.Add(ReaderdanOku(r, sonKolon));
                }
                r.Close();
            }
            return pVAN;
        }

        public ObjectArray ListeleCezaOdul(Kullanici kullanan, PersonelCezaOdul p)
        {
            ObjectArray pVAN = new ObjectArray();

            pVAN.sonuc = IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, p.personelKod, null, ENUMYetkiIslem.CezaOdul);
            if (!pVAN.sonuc.islemSonuc)
            {
                return pVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABCezaOdul tab = new TABCezaOdul(ve);

            IDataReader r;

            try
            {
                r = tab.ListeleCezaOdul(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            int sonKolon = ((int)TABCezaOdul.ENUMALANLARCEZAODUL.Dayanak) + 1;
            if (r != null)
            {
                while (r.Read())
                {
                    pVAN.objeler.Add(ReaderdanOku(r, sonKolon));
                }
                r.Close();
            }

            if (pVAN.objeler.Count > 0)//Tarihçe Kaydet ********************
                IKYPersonelTarihce.Kaydet(ve, kullanan, new PersonelCezaOdul(), p, ENUMTARIHCE.CEZAODUL, p.personelKod, ENUMTARIHCEISLEMTIP.LISTELE);

            ve.VeriErisimKapat();

            return pVAN;
        }

        private static PersonelCezaOdul ReaderdanOku(IDataReader r, int sonKolon)
        {
            PersonelCezaOdul d = new PersonelCezaOdul();
            d.kod = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.KOD, 0);
            d.personelKod = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.PKOD, 0);
            d.verenKurum = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.VerenKurum, "");
            d.verenAdi = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.VerenAdi, "");
            d.verenUnvani = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.VerenUnvani, "");
            d.alanUnvani = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.AlanUnvani, "");
            d.cinsi = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.Cinsi, "");
            d.sebebi = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.Sebebi, "");
            d.belgeCinsi = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.BelgeCinsi, "");
            d.belgeNo = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.BelgeNo, "");
            d.belgeTarihi = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.BelgeTarihi);
            d.cezaOdulTipi = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.CezaOdulTipi, 0);
            d.tarih = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.Tarih);
            d.dayanak = VeriErisim.DegerAl(r, (int)TABCezaOdul.ENUMALANLARCEZAODUL.Dayanak, "");
            d.cezaOdulTipiAdi = VeriErisim.DegerAl(r, sonKolon, "");
            d.cezaOdulTipiKayitKodu = VeriErisim.DegerAl(r, sonKolon + 1, "");
            return d;
        }

        public Sonuc KaydetCezaOdul(Kullanici kullanan, PersonelCezaOdul p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için            

            string hata = "";

            if (p.personelKod == 0)
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.CezaOdul);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            //Tarihçe Kaydet ********************

            if (p.kod > 0)
            {
                this.eskiTmp = this.ListeleCezaOdul(kullanan, p);
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelCezaOdul();//  "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABCezaOdul tab = new TABCezaOdul(ve);

            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;

            if (p.kod == 0)
            {
                p.kod = TABGenel.SayacDegerAlInt(ve, TABCezaOdul.TABLOADCEZAODUL);
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
            }
            sonuc = tab.KaydetCezaOdul(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.CEZAODUL, p.personelKod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilCezaOdul(Kullanici kullanan, PersonelCezaOdul p)
        {
            Sonuc sonuc = new Sonuc();
            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.SIL;
            object eski = null, yeni = null; //Tarihçe için  

            string hata = "";
            if (p.kod == 0)
                hata = "Silmek istediğiniz ceza ödülü seçiniz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.CezaOdul);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            //Tarihçe Kaydet ********************

            if (p.kod > 0)
            {
                this.eskiTmp = this.ListeleCezaOdul(kullanan, p);
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelCezaOdul();//  "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            //TABBelgeler tabBelge = new TABBelgeler(ve);
            //sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod=p.personelKod, tip=(int)ENUMBELGEFORMTIP.PERSONELAILE });

            //if (sonuc.islemSonuc)
            //{
            TABCezaOdul tab = new TABCezaOdul(ve);
            sonuc = tab.SilCezaOdul(p);
            //}

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
                //Tarihçe Kaydet ********************
                yeni = new PersonelCezaOdul();
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.CEZAODUL, p.personelKod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

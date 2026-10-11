using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    public class IKYPersonelGider
    {
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IKY.IKYPersonelGider p)
        {
            ObjectArray pVAN = new ObjectArray();

            if (p.pKod > 0)
                IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, p.pKod, pVAN.sonuc, ENUMYetkiIslem.Gider);
            else
                IKYYetki.YetkiVarMiOkumaya(kullanan, p.kriterOrgKopruKod, 0, pVAN.sonuc, ENUMYetkiIslem.Gider);
            if (!pVAN.sonuc.islemSonuc)
            {
                return pVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPERSONELGIDER tab = new TABPERSONELGIDER(ve);
            int AlanlarSonIndex = (Enum.GetNames(typeof(TABPERSONELGIDER.ENUMAlanlar))).Length;

            IDataReader r = tab.Listele(p);
            if (r != null)
            {
                while (r.Read())
                {
                    IKY.IKYPersonelGider k = new IKY.IKYPersonelGider();
                    k.pKod = VeriErisim.DegerAl(r, (int)TABPERSONELGIDER.ENUMAlanlar.PKOD, 0);
                    k.yerleskeKod = VeriErisim.DegerAl(r, (int)TABPERSONELGIDER.ENUMAlanlar.YERLESKEODAKOD, 0);
                    k.harcamaTur = (IKY.IKYPersonelGider.ENUMPersonelHarcamaTur)VeriErisim.DegerAl(r, (int)TABPERSONELGIDER.ENUMAlanlar.HARCAMATUR, 0);
                    k.tarih = VeriErisim.DegerAl(r, (int)TABPERSONELGIDER.ENUMAlanlar.TARIH);
                    k.tutar = VeriErisim.DegerAl(r, (int)TABPERSONELGIDER.ENUMAlanlar.TUTAR, 0.0);
                    k.mailGonderildi = VeriErisim.DegerAl(r, (int)TABPERSONELGIDER.ENUMAlanlar.MAILGONDERILDI, 0);
                    k.durum = VeriErisim.DegerAl(r, (int)TABPERSONELGIDER.ENUMAlanlar.DURUM, 0);
                    k.kullaniciKodu = VeriErisim.DegerAl(r, (int)TABPERSONELGIDER.ENUMAlanlar.DEGISIKLIGIYAPAN, "");
                    k.degisiklikZamani = VeriErisim.DegerAl(r, (int)TABPERSONELGIDER.ENUMAlanlar.DEGISIKLIKZAMANI);
                    k.tckimlikNo = VeriErisim.DegerAl(r, AlanlarSonIndex, "");
                    k.ad = VeriErisim.DegerAl(r, AlanlarSonIndex + 1, "");
                    k.soyad = VeriErisim.DegerAl(r, AlanlarSonIndex + 2, "");
                    k.saymanlikKod = VeriErisim.DegerAl(r, AlanlarSonIndex + 3, 0);
                    k.birimKod = VeriErisim.DegerAl(r, AlanlarSonIndex + 4, "");
                    k.yerleskeAdi = VeriErisim.DegerAl(r, AlanlarSonIndex + 4, "");
                    pVAN.objeler.Add(k);
                }
                r.Close();
            }
            else
            {
                pVAN.sonuc.DegerAta(ve.hata);
            }
            //if (pVAN.objeler.Count > 0)
            //{
            //    //Tarihçe Kaydet ********************
            //    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, new PersonelKurs(), p, ENUMTARIHCE.EGITIM, p.personelKod, ENUMTARIHCEISLEMTIP.LISTELE);
            //}

            ve.VeriErisimKapat();
            return pVAN;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IKY.IKYPersonelGider p, bool yeni)
        {
            Sonuc sonuc = new Sonuc();

            if (p.pKod <= 0)
                sonuc.DegerAta("Personel bilgisi boş bırakılamaz.");
            if (p.yerleskeKod <= 0)
                sonuc.DegerAta("Yerleşke bilgisi boş bırakılamaz.");
            if (p.harcamaTur <= 0)
                sonuc.DegerAta("Harcama türü bilgisi boş bırakılamaz.");
            if (sonuc.islemSonuc)
                IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.pKod, sonuc, ENUMYetkiIslem.Gider);

            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            //object eski = null, yeni = null; //Tarihçe için

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPERSONELGIDER tab = new TABPERSONELGIDER(ve);

            if (yeni)
                sonuc = tab.Kaydet(kullanan.kullaniciKodu, p);
            else
                sonuc = tab.Guncelle(kullanan.kullaniciKodu, p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                ////Tarihçe Kaydet ********************
                //yeni = p;
                //Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        //public static Sonuc GuncelleMailGonderildi(VeriErisim veriErisim, Kullanici kullanan, IKY.IKYPersonelGider p)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    //IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.pKod, sonuc, ENUMYetkiIslem.Yemek);
        //    //if (!sonuc.islemSonuc)
        //    //{
        //    //    return sonuc;
        //    //}

        //    ////object eski = null, yeni = null; //Tarihçe için

        //    //string hata = "";

        //    //if (p.pKod <= 0)
        //    //    hata += "Personel bilgisi boş bırakılamaz.<br>";

        //    //if (hata != "")
        //    //{
        //    //    sonuc.hataStr = hata;
        //    //    sonuc.islemSonuc = false;
        //    //    return sonuc;
        //    //}

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    //ve.BeginTransaction();

        //    //TABYemek tab = new TABYemek(ve);

        //    //if (p.kod > 0)
        //    //    sonuc = tab.GuncelleMailGonderildi(p);

        //    //if (sonuc.islemSonuc)
        //    //{
        //    //    ve.Commit();
        //    //    sonuc.bilgiStr = PersonelResI.PER007;

        //    //    ////Tarihçe Kaydet ********************
        //    //    //yeni = p;
        //    //    //Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, islemTip);
        //    //}
        //    //else
        //    //    ve.Rollback();

        //    //ve.VeriErisimKapat();

        //    return sonuc;
        //}

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, IKY.IKYPersonelGider p)
        {
            Sonuc sonuc = new Sonuc();

            //IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.pKod, sonuc, ENUMYetkiIslem.Yemek);
            //if (!sonuc.islemSonuc)
            //{
            //    return sonuc;
            //}

            //if (p.tarih.ToString() != DateTime.Now.Date.ToShortDateString())
            //{
            //    sonuc.DegerAta("Silmek istediğiniz yemek hareketinin tarihi bugünkü tarihden farklı olduğu için silme işlemi gerçekleştirilemedi.");
            //    return sonuc;
            //}
            ////object eski = null, yeni = null; //Tarihçe için

            //string hata = "";

            //if (p.pKod < 0)
            //    hata += "Personel bilgisi boş bırakılamaz.<br>";

            //if (hata != "")
            //{
            //    sonuc.hataStr = hata;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}


            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            //ve.BeginTransaction();

            ////TABBelgeler tabBelge = new TABBelgeler(ve);
            ////sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod = p.personelKod, tip = (int)ENUMBELGEFORMTIP.PERSONELKURS });

            //if (sonuc.islemSonuc)
            //{
            //    TABYemek tab = new TABYemek(ve);
            //    sonuc = tab.Sil(p);
            //}

            //if (sonuc.islemSonuc)
            //{
            //    ve.Commit();
            //    sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            //    PersonelIletisim iletisim = IKYPersonel.ListeleIletisim(ve, kullanan, p.pKod);
            //    sonuc.anahtar = iletisim.personelAd + ";" + iletisim.eposta;

            //    //    //Tarihçe Kaydet ********************
            //    //    yeni = p;
            //    //    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, ENUMTARIHCEISLEMTIP.SIL);
            //}
            //else
            //    ve.Rollback();

            //ve.VeriErisimKapat();

            return sonuc;
        }

        internal static ObjectArray ListeleToplam(string okunanXML)
        {
            if (string.IsNullOrWhiteSpace(okunanXML)) return null;
            ObjectArray pVAN = new ObjectArray();

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);

            if (r != null)
            {
                while (r.Read())
                {
                    IKY.IKYPersonelGider d = new IKY.IKYPersonelGider();
                    d.harcamaTur = (IKY.IKYPersonelGider.ENUMPersonelHarcamaTur)VeriErisim.DegerAl(r, 0, 0);
                    d.tutar = VeriErisim.DegerAl(r, 1, 0.0);
                    pVAN.objeler.Add(d);
                }
                r.Close();
            }
            return pVAN;
        }

        internal static Sonuc Aktar(VeriErisim veriErisim, Kullanici kullanan, IKY.IKYPersonelGider gider)
        {
            Sonuc s = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) s.DegerAta("Bu işlemi sadece sistem yöneticileri yapabilir!");
            if (gider.kriterYil <= 0) s.DegerAta("Lütfen yıl seçiniz!");
            if (gider.kriterAy <= 0) s.DegerAta("Lütfen ay seçiniz!");
            if (gider.harcamaTur == IKY.IKYPersonelGider.ENUMPersonelHarcamaTur.Tanimsiz) s.DegerAta("Lütfen harcama türü seçiniz!");

            if (!s.islemSonuc) return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPERSONELGIDER tab = new TABPERSONELGIDER(ve);

            ve.BeginTransaction();

            s = tab.AktarSil(gider);

            if (s.islemSonuc)
                s = tab.Aktar(gider, kullanan.kullaniciKodu);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            return s;
        }

        internal static Sonuc Yukle(VeriErisim veriErisim, Kullanici kullanan, TNSCollection liste)
        {
            Sonuc s = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("Bu işlemi sadece yöneticiler yapabilir!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TNSCollection tc = KimlikNoListesi(liste);
            TNSCollection perListe = IsKurallari.IKYPersonel.ListeleKod(ve, tc, tc);
            TABPERSONELGIDER tab = new TABPERSONELGIDER(ve);

            ve.BeginTransaction();
            bool ilk = false;
            string bulunmayan = "";
            foreach (TNS.IKY.IKYPersonelGider item in liste)
            {
                int pkod = PKODBul(perListe, item.tckimlikNo);
                if (pkod > 0)
                {
                    if (ilk)
                    {
                        s = tab.KayitSil(item.yerleskeKod, item.harcamaTur, item.tarih);
                        if (!s.islemSonuc) break;
                        ilk = false;
                    }
                    item.pKod = pkod;
                    s = tab.Kaydet(kullanan.kullaniciKodu, item);
                    if (!s.islemSonuc) break;
                }
                else
                {
                    if (bulunmayan != "") bulunmayan += ", ";
                    bulunmayan += item.tckimlikNo;
                }
            }
            if (s.islemSonuc)
            {
                ve.Commit();
                if (bulunmayan != "") s.bilgiStr = "Kaydı bulunamayan veriler:" + bulunmayan;
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        private static int PKODBul(TNSCollection perListe, string tckimlikNo)
        {
            foreach (Personel item in perListe)
            {
                if (item.tcKimlikNo == tckimlikNo || item.sicilNo == tckimlikNo)
                    return item.kod;
            }
            return 0;
        }

        private static TNSCollection KimlikNoListesi(TNSCollection liste)
        {
            TNSCollection l = new TNSCollection();
            foreach (TNS.IKY.IKYPersonelGider item in liste)
            {
                l.Add(item.tckimlikNo);
            }
            return l;
        }
    }
}

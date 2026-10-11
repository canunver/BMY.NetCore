using System;
using System.Collections;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections.Generic;
using TNS.DEG;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class MUHIs
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IsTanim form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            form.islemYapan = kullanan.ad + " " + kullanan.soyad;
            form.islemTarihi = new TNSDateTime(DateTime.Now);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABIs to = new TABIs(ve);
            TABGenel toGenel = new TABGenel(ve);

            if (form.kod == 0)
            {
                form.kod = OrtakFonksiyonlar.ConvertToInt(toGenel.BelgeNoAl("ISSAYAC", "000000"), 0);
                sonuc = to.Kaydet(form);
            }
            else
                sonuc = to.Guncelle(form);

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = form.kod.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, IsTanim form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABIs to = new TABIs(ve);

                ve.BeginTransaction();
                sonuc = to.Sil(form.kod);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IsTanim form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIs to = new TABIs(ve);
            IDataReader dr = to.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    IsTanim b = new IsTanim();
                    b.kod = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.KOD, 0);
                    b.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.ORGKOD, 0);
                    b.adi = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.ADI, "");
                    b.tip = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.TIP, 0);
                    b.baslangicTarihi = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.BASLANGICTARIH);
                    b.bitisTarihi = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.BITISTARIH);
                    b.amac = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.AMAC, "");
                    b.hedef = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.HEDEF, "");
                    b.sorumluKod = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.SORUMLUKOD, "");
                    b.gorevTanimi = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.GOREVTANIMI, "");
                    b.durum = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.DURUM, 0);
                    b.islemYapan = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.ISLEMYAPAN, "");
                    b.islemTarihi = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.ISLEMTARIH);
                    b.orgBirimAd = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.ISLEMTARIH + 1, "");
                    b.muhasebeKod = OrtakFonksiyonlar.ConvertToInt(VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.ISLEMTARIH + 2, ""), 0);
                    b.muhasebeBirimKod = VeriErisim.DegerAl(dr, (int)TABIs.ENUMAlanlar.ISLEMTARIH + 3, "");
                    liste.Ekle(b);
                }
                dr.Close();
            }
            else
            {
                liste.sonuc.hataStr = "İşlem sırasında beklenmedik bir hata oluştu:" + ve.hata;
            }
            ve.VeriErisimKapat();

            return liste;
        }

        public static ObjectArray BlokeOdenekListele(VeriErisim veriErisim, Kullanici kullanan, IsTanim form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIs to = new TABIs(ve);
            IDataReader dr = to.BlokeOdenekListele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int sira = 0;
                    IsBlokeOdenek b = new IsBlokeOdenek();
                    b.isAd = VeriErisim.DegerAl(dr, sira++, "");
                    b.birimAd = VeriErisim.DegerAl(dr, sira++, "");
                    b.ilgiliAd = VeriErisim.DegerAl(dr, sira++, "");
                    b.harcananOdenek = Math.Abs(VeriErisim.DegerAl(dr, sira++, 0.0));
                    b.toplamOdenek = VeriErisim.DegerAl(dr, sira++, 0.0);

                    b.kalanTutar = b.toplamOdenek - b.harcananOdenek;

                    liste.Ekle(b);
                }
                dr.Close();
            }
            else
            {
                liste.sonuc.hataStr = "İşlem sırasında beklenmedik bir hata oluştu:" + ve.hata;
            }
            ve.VeriErisimKapat();

            return liste;
        }

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, IsTanim form)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (form.kod == 0)
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
                else
                {
                    TABOdemeEmriMIF tabMIF = new TABOdemeEmriMIF(ve);
                    bool isKullanildi = tabMIF.IsKullanildiMi(form.kod);
                    if (isKullanildi)
                        sonuc.hataStr += "Silmek istediğiniz iş ile bağlı muhasebe fişi olduğu için iş silinemez." + "<br>";
                    else
                    {
                        TABTaahhutKarti tabTK = new TABTaahhutKarti(ve);
                        isKullanildi = tabTK.IsKullanildiMi(form.kod);
                        if (isKullanildi)
                            sonuc.hataStr += "Silmek istediğiniz iş ile bağlı taahhut kartı olduğu için iş silinemez." + "<br>";
                    }

                }
            }

            if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (form.orgBirimKod == 0)
                    sonuc.hataStr += "Birim bilgisi boş bırakılamaz." + "<br>";
                if (string.IsNullOrEmpty(form.adi))
                    sonuc.hataStr += "Ad bilgisi boş bırakılamaz." + "<br>";
                if (form.tip == 0)
                    sonuc.hataStr += "Tip bilgisi boş bırakılamaz." + "<br>";
                if (form.baslangicTarihi.isNull)
                    sonuc.hataStr += "Başlangıç tarihi boş bırakılamaz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

    }
}

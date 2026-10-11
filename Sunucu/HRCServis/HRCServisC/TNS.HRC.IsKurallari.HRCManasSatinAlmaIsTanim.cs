using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    public class HRCManasSatinAlmaIsTanim
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaIsTanim form)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            string hata = string.Empty;
            if (form.tarih.isNull)
                hata += "Tarih bilgisi boş bırakılamaz." + "<br>";
            if (string.IsNullOrEmpty(form.ad))
                hata += "İşin adı boş bırakılamaz." + "<br>";
            if (string.IsNullOrEmpty(form.nitelik))
                hata += "İşin niteliği ve miktarı boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaIsTanim tab = new TABManasSatinAlmaIsTanim(ve);
            ve.BeginTransaction();

            if (form.kod > 0)
            {
                sonuc = tab.Guncelle(form);
            }
            else
            {
                form.kod = OrtakFonksiyonlar.ConvertToInt(new TABGenel(ve).BelgeNoAl("SATINALMAKOD", "0"), 0);
                sonuc = tab.Kaydet(form);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = form.kod.ToString();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc KaydetOnay(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaIsTanim form)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            string hata = string.Empty;
            if (form.onayTarihi.isNull)
                hata += "Tarih bilgisi boş bırakılamaz." + "<br>";
            if (form.kod == 0)
                hata += "Satın alma dosya numarası boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaIsTanim tab = new TABManasSatinAlmaIsTanim(ve);
            ve.BeginTransaction();

            sonuc = tab.GuncelleOnay(form);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaIsTanim form)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00268, "");

            string hata = string.Empty;
            if (form.kod <= 0)
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaIsTanim tab = new TABManasSatinAlmaIsTanim(ve);

            ve.BeginTransaction();
            sonuc = tab.Sil(form);
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ManasSatinAlmaIsTanim[] Listele(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaIsTanim form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaIsTanim tab = new TABManasSatinAlmaIsTanim(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                int sonKolon = (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.ONAYTARIHI;

                while (dr.Read())
                {
                    form = new ManasSatinAlmaIsTanim();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.KOD, 0);
                    form.tarih = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.TARIH);
                    form.ad = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.AD, "");
                    form.nitelik = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.NITELIK, "");
                    form.ihaleUsulu = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.IHALEUSULU, 0);
                    form.ihaleMaddesi = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.IHALEMADDESI, "");
                    form.alimTuru = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.ALIMTURU, 0);
                    form.paraBirimi.kod = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.PARABIRIMI, 0);
                    form.alimYontemi = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.ALIMYONTEMI, 0);
                    form.onayTarihi = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaIsTanim.ENUMAlanlar.ONAYTARIHI);

                    form.paraBirimi.adi = VeriErisim.DegerAl(dr, sonKolon + 1, "");
                    form.paraBirimi.kisaltma = VeriErisim.DegerAl(dr, sonKolon + 2, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (ManasSatinAlmaIsTanim[])liste.objeler.ToArray(typeof(ManasSatinAlmaIsTanim));
        }

        public static ManasSatinAlmaIsTanim Oku(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaIsTanim form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaIsTanim tab = new TABManasSatinAlmaIsTanim(ve);

            ManasSatinAlmaIsTanim[] list = Listele(ve, kullanan, form);

            form = (list != null && list.Length == 1) ? list[0] : null;

            ve.VeriErisimKapat();

            return form;
        }
    }
}

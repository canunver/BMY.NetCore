using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System;
using System.Collections;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// İmza Tanım işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHImzaTanimServis
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ImzaTanim form)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.ad))
                hata += "İmza tanım adı boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABImzaTanim tab = new TABImzaTanim(ve);

            ve.BeginTransaction();

            sonuc = form.kod == 0 ? tab.Kaydet(form) : tab.Guncelle(form);

            if (sonuc.islemSonuc && form.kod > 0)
            {
                sonuc = tab.SilDetay(form.kod);
                if (sonuc.islemSonuc && form.detaylar != null && form.detaylar.Length > 0)
                {
                    foreach (ImzaTanimDetay detay in form.detaylar)
                    {
                        detay.imzaTanimKod = form.kod;

                        sonuc = tab.Kaydet(detay);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = form.kod.ToString();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH079;
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ImzaTanim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (form.kod <= 0)
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABImzaTanim tab = new TABImzaTanim(ve);

            ve.BeginTransaction();
            sonuc = tab.Sil(form);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH079;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ImzaTanim[] Listele(VeriErisim veriErisim, Kullanici kullanan, ImzaTanim form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABImzaTanim tab = new TABImzaTanim(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new ImzaTanim();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlar.KOD, 0);
                    form.ad = VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlar.AD, "");
                    form.belgeTur = (ENUMBelgeTuru)VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlar.BELGETUR, 0);
                    form.kullaniciKodu = VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlar.KULLANICIKODU, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return liste.objeler.Count > 0 ? (ImzaTanim[])liste.objeler.ToArray(typeof(ImzaTanim)) : null;
        }

        private static ImzaTanimDetay[] Listele(VeriErisim veriErisim, Kullanici kullanan, int imzaYoluKod)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABImzaTanim tab = new TABImzaTanim(ve);

            IDataReader dr = tab.ListeleDetay(imzaYoluKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    ImzaTanimDetay detay = new ImzaTanimDetay();
                    detay.imzaTanimKod = VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlarDetay.IMZATANIMKOD, 0);
                    detay.imzaYer = VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlarDetay.IMZAYER, "");
                    detay.adSoyad = VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlarDetay.AD, "");
                    detay.unvan = VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlarDetay.UNVAN, "");
                    detay.gorevUnvan = VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlarDetay.GOREVUNVAN, "");
                    detay.tarih = VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlarDetay.TARIH, 0);
                    detay.kullanilmiyor = VeriErisim.DegerAl(dr, (int)TABImzaTanim.ENUMAlanlarDetay.KULLANILMIYOR, 0);

                    liste.objeler.Add(detay);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            return liste.objeler.Count > 0 ? (ImzaTanimDetay[])liste.objeler.ToArray(typeof(ImzaTanimDetay)) : null;
        }

        public static ImzaTanim Oku(VeriErisim veriErisim, Kullanici kullanan, ImzaTanim form)
        {
            VeriErisim ve1 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ImzaTanim[] list = MUHImzaTanimServis.Listele(ve1, kullanan, form);

            if (list != null)
                form = list[0];

            VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ImzaTanimDetay[] listDetay = Listele(ve2, kullanan, form.kod);

            if (listDetay != null && listDetay.Length > 0)
                form.detaylar = listDetay;

            ve1.VeriErisimKapat();
            ve2.VeriErisimKapat();

            return form;
        }

        public static Sonuc GecerliImzaYap(VeriErisim veriErisim, Kullanici kullanan, int imzaTanimKod, string birimKod)
        {
            string hata = "";

            if (imzaTanimKod <= 0)
                hata += "Tanımlı bir imza seçiniz!<br>";

            if (hata != "")
                return new Sonuc(false, hata, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ImzaTanim form = Oku(ve, kullanan, new ImzaTanim() { kod = imzaTanimKod });
            ve.VeriErisimKapat();

            if (form == null)
                return new Sonuc(false, "İmza bilgisi okunamadı.", "");

            ObjectArray imzaBilgileri = new ObjectArray();
            foreach (var detay in form.detaylar)
            {
                ImzaBilgisi ib = new ImzaBilgisi();
                ib.kullaniciKodu = kullanan.kullaniciKodu;
                ib.muhasebe = 0;
                ib.birim = birimKod;
                ib.belgeTur = form.belgeTur;
                ib.imzaYer = detay.imzaYer;
                ib.adSoyad = detay.adSoyad;
                ib.unvan = detay.unvan;
                ib.gorevUnvan = detay.gorevUnvan;
                ib.tarih = detay.tarih;
                ib.kullanilmiyor = detay.kullanilmiyor;

                imzaBilgileri.objeler.Add(ib);
            }

            return new MUHImzaServis().Kaydet(kullanan, imzaBilgileri);
        }
    }
}

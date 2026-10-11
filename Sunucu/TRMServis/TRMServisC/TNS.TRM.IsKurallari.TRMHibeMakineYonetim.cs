using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeMakineYonetim
    {
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, Yonetim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) || (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))))
                return sonuc;

            if (form.etapNo == 0)
                hata += "Etap no boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeMakineYonetim tab = new TABHibeMakineYonetim(ve);
            ve.BeginTransaction();

            sonuc = tab.Kaydet(form);

            if (sonuc.islemSonuc)
            {
                sonuc = tab.Sil(new YonetimGiris() { etapNo = form.etapNo });
                if (sonuc.islemSonuc && form.girisler != null && form.girisler.Length > 0)
                {
                    foreach (YonetimGiris giris in form.girisler)
                    {
                        giris.etapNo = form.etapNo;
                        sonuc = tab.Kaydet(giris);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
            }

            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = "Kayıt işlemi başarısız oldu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }
            else
            {
                sonuc.anahtar = form.etapNo.ToString();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla gerçekleşti";
                ve.Commit();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public static Sonuc Sil(VeriErisim ve, Kullanici kullanan, Yonetim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (form.etapNo <= 0)
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeMakineYonetim tab = new TABHibeMakineYonetim(ve);
            ve.BeginTransaction();

            sonuc = tab.Sil(form);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silmek istenilen bilgi başarıyla silindi.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme işlemi başarısız oldu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Yonetim[] Listele(VeriErisim ve, Kullanici kullanan, Yonetim form)
        {
            ObjectArray liste = new ObjectArray();
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, KYM.Arac.KimlikBul(kullanan));
            TABHibeMakineYonetim tab = new TABHibeMakineYonetim(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new Yonetim();
                    form.etapNo = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlar.ETAPNO, 0);
                    form.gercek = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlar.GERCEK, 0.0);
                    form.tuzel = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlar.TUZEL, 0.0);
                    form.tarih1 = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlar.TARIH1);
                    form.tarih2 = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlar.TARIH2);
                    form.aktif = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlar.AKTIF, 0);
                    form.giris = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlar.GIRIS, 0);
                    form.tebligNo = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlar.TEBLIGNO, string.Empty);
                    form.onlineGirisiKapatmaTarihi = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlar.ONLINEKAPATMATARIHI);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (Yonetim[])liste.objeler.ToArray(typeof(Yonetim));
        }
        private static YonetimGiris[] Listele(VeriErisim ve, Kullanici kullanan, YonetimGiris form)
        {
            ObjectArray liste = new ObjectArray();
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeMakineYonetim tab = new TABHibeMakineYonetim(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new YonetimGiris();
                    form.etapNo = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlarGiris.ETAPNO, 0);
                    form.ilKod = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlarGiris.ILKOD, "");
                    form.tarih = VeriErisim.DegerAl(dr, (int)TABHibeMakineYonetim.ENUMAlanlarGiris.TARIH);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (YonetimGiris[])liste.objeler.ToArray(typeof(YonetimGiris));
        }
        public static Yonetim Oku(VeriErisim ve, Kullanici kullanan, Yonetim form)
        {
            if (form.etapNo == 0) return null;

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            Yonetim[] list = TRMHibeMakineYonetim.Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            YonetimGiris[] listGiris = TRMHibeMakineYonetim.Listele(ve, kullanan, new YonetimGiris() { etapNo = form.etapNo });

            if (listGiris != null && listGiris.Length > 0)
                form.girisler = listGiris;

            ve.VeriErisimKapat();

            return form;
        }

        public static int HibeMakineAktifEtapNo(VeriErisim ve)
        {
            int etapNo = 0;
            Yonetim y = new Yonetim();
            y.aktif = 1;

            Yonetim[] yler = Listele(ve, new Kullanici(), y);
            foreach (Yonetim yy in yler)
            {
                if (yy.aktif > 0)
                    etapNo = yy.etapNo;
            }
            return etapNo;
        }

        public static double HibeMakineAktifUstLimit(VeriErisim ve, int hukukiStatu)
        {
            double ustLimit = 0;
            Yonetim y = new Yonetim();
            y.aktif = 1;

            Yonetim[] yler = Listele(ve, new Kullanici(), y);

            foreach (Yonetim yy in yler)
            {
                if (yy.aktif > 0)
                {
                    if (hukukiStatu == (int)ENUMHibeMakineHukukiStatu.TUZELKISI || hukukiStatu == (int)ENUMHibeMakineHukukiStatu.SUTBIRLIK)
                        ustLimit = yy.tuzel;
                    else if (hukukiStatu == (int)ENUMHibeMakineHukukiStatu.GERCEKKISI || hukukiStatu == (int)ENUMHibeMakineHukukiStatu.GENCCIFTCI)
                        ustLimit = yy.gercek;
                }
            }
            return ustLimit;
        }

        public static string HibeMakineOnlineKapandiMi(VeriErisim ve)
        {
            System.DateTime onlineKapatmaTarihi = new System.DateTime();
            string hata = "";

            Yonetim[] yler = Listele(ve, new Kullanici(), new Yonetim { aktif = 1 });
            foreach (Yonetim yy in yler)
                onlineKapatmaTarihi = yy.onlineGirisiKapatmaTarihi.Oku();

            if (System.DateTime.Now > onlineKapatmaTarihi)
                hata = "Online kapatma tarihi geçtiği için online başvuru işlemi yapamazsınız.";
            return hata;
        }

        public Sonuc AnketSorulariniAktar(VeriErisim ve, string onEkTur, string eskiEtap, string yeniEtap)
        {
            if (OrtakFonksiyonlar.ConvertToInt(eskiEtap, 0) == 0 || OrtakFonksiyonlar.ConvertToInt(yeniEtap, 0) == 0)
                return new Sonuc(false, "Etap bilgisi boş", "");

            if (onEkTur.Trim() == "")
                return new Sonuc(false, "ÖnEk bilgisi boş", "");

            TNSDateTime baslamaTarih = new TNSDateTime();
            TNSDateTime bitisTarih = new TNSDateTime();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriKYM");

            Yonetim[] yler = Listele(ve, new Kullanici(), new Yonetim { etapNo = OrtakFonksiyonlar.ConvertToInt(yeniEtap, 0) });
            foreach (Yonetim yy in yler)
            {
                baslamaTarih = yy.tarih1;
                bitisTarih = yy.tarih2;
            }

            if (baslamaTarih.isNull)
            {
                baslamaTarih = new TNSDateTime("01." + System.DateTime.Now.Month + "." + System.DateTime.Now.Year);
                bitisTarih = new TNSDateTime("31.12" + "." + System.DateTime.Now.Year);
            }

            TABHibeMakineYonetim tab = new TABHibeMakineYonetim(ve);

            int eklenecekSayi = tab.AnketSoruMaxPrSiraNo();

            Sonuc sonuc = tab.AnketSorulariniAktar(onEkTur, eskiEtap, yeniEtap, eklenecekSayi, baslamaTarih, bitisTarih);

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}

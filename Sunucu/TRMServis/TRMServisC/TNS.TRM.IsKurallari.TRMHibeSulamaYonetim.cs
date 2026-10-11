using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeSulamaYonetim
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
            TABHibeSulamaYonetim tab = new TABHibeSulamaYonetim(ve);
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
            TABHibeSulamaYonetim tab = new TABHibeSulamaYonetim(ve);
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
            TABHibeSulamaYonetim tab = new TABHibeSulamaYonetim(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new Yonetim();
                    form.etapNo = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.ETAPNO, 0);
                    form.gercek = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.GERCEK, 0.0);
                    form.tuzel = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.TUZEL, 0.0);
                    form.tarih1 = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.TARIH1);
                    form.tarih2 = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.TARIH2);
                    form.aktif = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.AKTIF, 0);
                    form.giris = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.GIRIS, 0);
                    form.kooperatif = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.KOOPERATIF, 0.0);
                    form.tebligNo = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.TEBLIGNO, string.Empty);
                    form.onlineGirisiKapatmaTarihi = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.ONLINEKAPATMATARIHI);
                    form.sozlesmeKapatmaTarihi = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlar.SOZLESMEKAPATMATARIHI);

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
            TABHibeSulamaYonetim tab = new TABHibeSulamaYonetim(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new YonetimGiris();
                    form.etapNo = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlarGiris.ETAPNO, 0);
                    form.ilKod = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlarGiris.ILKOD, "");
                    form.tarih = VeriErisim.DegerAl(dr, (int)TABHibeSulamaYonetim.ENUMAlanlarGiris.TARIH);

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
            Yonetim[] list = TRMHibeSulamaYonetim.Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            YonetimGiris[] listGiris = TRMHibeSulamaYonetim.Listele(ve, kullanan, new YonetimGiris() { etapNo = form.etapNo });

            if (listGiris != null && listGiris.Length > 0)
                form.girisler = listGiris;

            ve.VeriErisimKapat();

            return form;
        }

        public static int HibeSulamaAktifEtapNo(VeriErisim ve)
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

        public static double HibeSulamaAktifUstLimit(VeriErisim ve, int hukukiStatu)
        {
            double ustLimit = 0;
            Yonetim y = new Yonetim();
            y.aktif = 1;

            Yonetim[] yler = Listele(ve, new Kullanici(), y);

            foreach (Yonetim yy in yler)
            {
                if (yy.aktif > 0)
                {
                    if (hukukiStatu == (int)ENUMHibeSulamaHukukiStatu.TUZELKISI)
                        ustLimit = yy.tuzel;
                    else if (hukukiStatu == (int)ENUMHibeSulamaHukukiStatu.GERCEKKISI || hukukiStatu == (int)ENUMHibeSulamaHukukiStatu.GENCCIFTCI)
                        ustLimit = yy.gercek;
                    else if (hukukiStatu == (int)ENUMHibeSulamaHukukiStatu.KOOPERATIF || hukukiStatu == (int)ENUMHibeSulamaHukukiStatu.BIRLIK)
                        ustLimit = yy.kooperatif;
                }
            }
            return ustLimit;
        }

        public static string HibeSulamaOnlineKapandiMi(VeriErisim ve)
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

        public static string HibeSulamaSozlesmeKapandiMi(VeriErisim ve)
        {
            System.DateTime sozlesmeKapatmaTarihi = new System.DateTime();
            string hata = "";

            Yonetim[] yler = Listele(ve, new Kullanici(), new Yonetim { aktif = 1 });
            foreach (Yonetim yy in yler)
                sozlesmeKapatmaTarihi = yy.sozlesmeKapatmaTarihi.Oku();
            if (System.DateTime.Now > sozlesmeKapatmaTarihi)
                hata = "Son Sözleşme tarihi geçtiği için sözleşme işlemi yapamazsınız.";
            return hata;
        }

        public static string HibeSulamaBirimReferansFarklimi(VeriErisim ve, Kullanici kullanan, int hibeKod)
        {
            string ret = "";
            ObjectArray liste = new ObjectArray();
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulamaMalzeme tab = new TABHibeSulamaMalzeme(ve);

            IDataReader dr = tab.BirimRefFarklimi(hibeKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    ret = (VeriErisim.DegerAl(dr, 1, (decimal)0) == 0 ? "0" : "1");
                    ret += (VeriErisim.DegerAl(dr, 2, (decimal)0) == 0 ? "0" : "1");
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (ret);
        }
    }
}

using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeYatirimYonetim
    {
        public static Sonuc Kaydet(ref VeriErisim ve, Kullanici kullanan, Yonetim form)
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

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirimYonetim tab = new TABHibeYatirimYonetim(ve);
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
        public static Sonuc Sil(ref VeriErisim ve, Kullanici kullanan, Yonetim form)
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
            TABHibeYatirimYonetim tab = new TABHibeYatirimYonetim(ve);
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
        public static Yonetim[] Listele(ref VeriErisim ve, Kullanici kullanan, Yonetim form)
        {
            ObjectArray liste = new ObjectArray();
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirimYonetim tab = new TABHibeYatirimYonetim(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new Yonetim();
                    form.etapNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.ETAPNO, 0);
                    form.gercek = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.GERCEK, 0.0);
                    form.tuzel = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.TUZEL, 0.0);
                    form.tarih1 = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.TARIH1);
                    form.tarih2 = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.TARIH2);
                    form.aktif = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.AKTIF, 0);
                    form.giris = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.GIRIS, 0);
                    form.tebligNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.TEBLIGNO, string.Empty);
                    form.onlineGirisiKapatmaTarihi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.ONLINEKAPATMATARIHI);
                    form.resmiGazeteNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.RESMIGAZETENO, string.Empty);
                    form.degerlendirmeKapatmaTarihi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.DEGERLENDIRMEKAPATMATARIHI);
                    form.eksikBelgeBasTarihi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.EKSIKBELGEBASTARIH);
                    form.eksikBelgeBitTarihi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.EKSIKBELGEBITTARIH);

                    form.tebligNoAltyapi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.TEBLIGNOALTYAPI, string.Empty);
                    form.resmiGazeteNoAltyapi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.RESMIGAZETENOALTYAPI, string.Empty);
                    form.tarih1Altyapi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.TARIH1ALTYAPI);
                    form.tarih2Altyapi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.TARIH2ALTYAPI);
                    form.onlineGirisiKapatmaTarihiAltyapi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.ONLINEKAPATMATARIHIALTYAPI);
                    form.degerlendirmeKapatmaTarihiAltyapi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.DEGERLENDIRMEKAPATMATARIHIALTYAPI);
                    form.girisAltyapi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.GIRISALTYAPI, 0);
                    form.eksikBelgeBasTarihiAltYapi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.EKSIKBELGEBASTARIHALTYAPI);
                    form.eksikBelgeBitTarihiAltYapi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.EKSIKBELGEBITTARIHALTYAPI);

                    form.izlemeSuresi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.IZLEMESURESI, 0);
                    form.izlemeSuresiAltYapi = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlar.IZLEMESURESIALTYAPI, 0);

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
            TABHibeYatirimYonetim tab = new TABHibeYatirimYonetim(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new YonetimGiris();
                    form.etapNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlarGiris.ETAPNO, 0);
                    form.ilKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlarGiris.ILKOD, "");
                    form.tarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirimYonetim.ENUMAlanlarGiris.TARIH);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (YonetimGiris[])liste.objeler.ToArray(typeof(YonetimGiris));
        }
        public static Yonetim Oku(ref VeriErisim ve, Kullanici kullanan, Yonetim form)
        {
            if (form.etapNo == 0) return null;

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            Yonetim[] list = TRMHibeYatirimYonetim.Listele(ref ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            YonetimGiris[] listGiris = TRMHibeYatirimYonetim.Listele(ve, kullanan, new YonetimGiris() { etapNo = form.etapNo });

            if (listGiris != null && listGiris.Length > 0)
                form.girisler = listGiris;

            ve.VeriErisimKapat();

            return form;
        }

        public static int HibeYatirimAktifEtapNo(ref VeriErisim ve)
        {
            int etapNo = 0;
            Yonetim y = new Yonetim();
            y.aktif = 1;

            Yonetim[] yler = Listele(ref ve, new Kullanici(), y);
            foreach (Yonetim yy in yler)
            {
                if (yy.aktif > 0)
                    etapNo = yy.etapNo;
            }
            return etapNo;
        }

        public static string HibeYatirimTebligNoBul(ref VeriErisim ve, int etapNo, int konuKod)
        {
            string tebligNo = "";

            Yonetim[] yler = Listele(ref ve, new Kullanici(), new Yonetim { etapNo = etapNo });
            foreach (Yonetim yy in yler)
            {
                if (konuKod < 51)
                    tebligNo = yy.tebligNo;
                else
                    tebligNo = yy.tebligNoAltyapi;
            }

            return tebligNo;
        }

        public static double HibeYatirimAktifUstLimit(ref VeriErisim ve, int hukukiStatu)
        {
            double ustLimit = 0;
            Yonetim y = new Yonetim();
            y.aktif = 1;

            Yonetim[] yler = Listele(ref ve, new Kullanici(), y);

            foreach (Yonetim yy in yler)
            {
                if (yy.aktif > 0)
                {
                    if (hukukiStatu == (int)ENUMHibeYatirimHukukiStatu.TUZELKISI)
                        ustLimit = yy.tuzel;
                    else if (hukukiStatu == (int)ENUMHibeYatirimHukukiStatu.GERCEKKISI)
                        ustLimit = yy.gercek;
                }
            }
            return ustLimit;
        }

        public static double HibeYatirimAktifUstLimitKonu(ref VeriErisim ve, int etapNo, int projeNitelik, string konuKod, int hukukiStatu)
        {
            double ustLimit = 0;
            Yonetim y = new Yonetim();
            y.etapNo = etapNo;

            Yonetim[] yler = Listele(ref ve, new Kullanici(), y);

            foreach (Yonetim yy in yler)
            {
                if (yy.etapNo == 12)
                {
                    if (!konuKod.StartsWith("51") || !konuKod.StartsWith("53"))
                    {
                        if (projeNitelik == 1) konuKod = "00.01";
                        else if (projeNitelik == 2) konuKod = "00.02";
                        else if (projeNitelik == 3) konuKod = "00.03";
                    }
                }

                if (yy.etapNo < 14)
                {
                    konuKod = konuKod.Substring(0, 2);
                }

                YatirimKonuUstSinir kriter = new YatirimKonuUstSinir();
                kriter.konuKod = konuKod;
                kriter.etapNo = yy.etapNo;

                if (yy.etapNo >= 12)
                    kriter.nitelikKod = projeNitelik;

                ObjectArray liste = new TRMYatirimKonu().ListeleKonuUstSinir(null, kriter);
                foreach (YatirimKonuUstSinir yyy in liste.objeler)
                {
                    if (hukukiStatu == (int)ENUMHibeYatirimHukukiStatu.TUZELKISI)
                        ustLimit = yyy.tuzel;
                    else
                        ustLimit = yyy.gercek;
                }
            }
            return ustLimit;
        }

        public static string HibeYatirimOnlineKapandiMi(VeriErisim ve, Kullanici kullanan, string basvuruNo)
        {
            System.DateTime onlineKapatmaTarihi = new System.DateTime();
            System.DateTime onlineKapatmaTarihiAltyapi = new System.DateTime();
            string hata = "";
            int aktifAtapNo = 0;

            //string depremIller = "0105001,0106001,0106002,0107004,0106003,0105004,0105007,0106009,0107009,0105008,0106006";

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));

            Yonetim[] yler = Listele(ref ve, new Kullanici(), new Yonetim { aktif = 1 });
            //YonetimGiris[] girisler = null;

            foreach (Yonetim yy in yler)
            {
                if (yy.aktif > 0) aktifAtapNo = yy.etapNo;
                onlineKapatmaTarihi = yy.onlineGirisiKapatmaTarihi.Oku();
                onlineKapatmaTarihiAltyapi = yy.onlineGirisiKapatmaTarihiAltyapi.Oku();
                //if (basvuruNo != "")
                //    girisler = Listele(ve, new Kullanici(), new YonetimGiris() { ilKod = basvuruNo, etapNo = yy.etapNo });
            }

            string tcnolar = "15944350944,25912732832,10726233026,11264021816";
            //string tcnolar = "xx";
            if (tcnolar.Contains(kullanan.mernis))
            {
                onlineKapatmaTarihi = System.DateTime.Now.AddDays(3);
                onlineKapatmaTarihiAltyapi = System.DateTime.Now.AddDays(3);
            }

            if (System.DateTime.Now > onlineKapatmaTarihi || System.DateTime.Now > onlineKapatmaTarihiAltyapi)
            {
                if (basvuruNo != "")
                {
                    ObjectArray hmler = TRMHibeYatirim.Sorgula(ve, new Kullanici(), new HibeYatirim { online = true, basvuruNo = basvuruNo }, 0, 0, "", false);
                    foreach (HibeYatirim item in hmler.objeler)
                    {
                        if (OrtakFonksiyonlar.ConvertToInt(item.projeKonu, 0) < 51)
                        {
                            //if (depremIller.IndexOf(item.ilKod) > -1)
                            //{
                            //    onlineKapatmaTarihi = new TNSDateTime("09.05.2023").Oku();
                            //}
                            //OrtakFonksiyonlar.HataStrYaz("onlineKapatmaTarihi:" + onlineKapatmaTarihi.ToString() + " depremIller: " + depremIller + " il:" + item.ilKod + " " + item.kimlikNo);
                            if (System.DateTime.Now > onlineKapatmaTarihi)
                                hata = "Ekonomik Yatırımlar Online giriş tarihi geçtiği için online başvuru işlemi ve başvuruyu işleme almayı yapamazsınız.";
                        }
                        else
                        {
                            if (System.DateTime.Now > onlineKapatmaTarihiAltyapi)
                                hata = "Altyapı Yatırımlar Online giriş tarihi geçtiği için online başvuru işlemi ve başvuruyu işleme almayı yapamazsınız.";
                        }
                    }
                }
                else if (System.DateTime.Now > onlineKapatmaTarihi && System.DateTime.Now > onlineKapatmaTarihiAltyapi)
                    hata = "Online giriş tarihi geçtiği için online başvuru işlemi ve başvuruyu işleme almayı yapamazsınız...";
            }

            ve.VeriErisimKapat();

            return hata;
        }

        public static string HibeYatirimDegerlendirmeKapandiMi(string basvuruNo, string projeKonu, string ilKodu = "")
        {
            //OrtakFonksiyonlar.HataStrYaz("HibeYatirimDegerlendirmeKapandiMi:" + basvuruNo + " " + projeKonu);
            System.DateTime degerlendirmeKapatmaTarihi = new System.DateTime();
            string hata = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            Yonetim[] yler = Listele(ref ve, new Kullanici(), new Yonetim { aktif = 1 });
            YonetimGiris[] girisler = null;
            int etapNo = 0;
            //string depremIller = "0105001,0106001,0106002,0107004,0106003,0105004,0105007,0106009,0107009,0105008,0106006";

            foreach (Yonetim yy in yler)
            {
                degerlendirmeKapatmaTarihi = yy.degerlendirmeKapatmaTarihi.Oku();

                if (OrtakFonksiyonlar.ConvertToInt(projeKonu, 0) > 50)
                {
                    degerlendirmeKapatmaTarihi = yy.degerlendirmeKapatmaTarihiAltyapi.Oku();
                }

                etapNo = yy.etapNo;
            }

            if ((basvuruNo != "" || ilKodu != "") && System.DateTime.Now > degerlendirmeKapatmaTarihi)
            {
                TABHibeYatirimYonetim tab = new TABHibeYatirimYonetim(ve);
                if (string.IsNullOrEmpty(ilKodu))
                    ilKodu = tab.IlKoduVer(basvuruNo);

                //int depremIl = -1;// depremIller.IndexOf(ilKodu);
                //if (depremIl > -1)
                //{
                //    if (OrtakFonksiyonlar.ConvertToInt(projeKonu, 0) > 50)
                //    {
                //        if (ilKodu == "0105004")//Altyapı hatay için
                //            degerlendirmeKapatmaTarihi = new TNSDateTime("10.05.2023").Oku();
                //    }
                //    else
                //        degerlendirmeKapatmaTarihi = new TNSDateTime("10.06.2023").Oku();//Eko yatırım için
                //}
            }

            if (System.DateTime.Now > degerlendirmeKapatmaTarihi)
            {
                if (ilKodu != "")
                    girisler = Listele(ve, new Kullanici(), new YonetimGiris() { ilKod = ilKodu, etapNo = etapNo });

                bool gectiMi = false;
                if (girisler != null && girisler.Length > 0)
                {
                    if (System.DateTime.Now > girisler[0].tarih.Oku())
                        gectiMi = false;
                    else
                        gectiMi = true;
                }

                if (!gectiMi)
                    hata = "Değerlendirme tarihleri giriş yapılması için uygun değil";//i geçtiği için ve başvuru numaranız izinli olmadığı için değerlendirme yapamazsınız.";
            }

            ve.VeriErisimKapat();

            return hata;
        }
    }
}

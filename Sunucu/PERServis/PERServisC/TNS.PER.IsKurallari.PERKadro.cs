using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.PER.IsKurallari
{
    /*********************************
     * Tahsis ve Tenkis bilgileri personel GM den aşağıdaki alanlar ile geliyor;
       T_TENTAH, T_KARARNAME, T_YIL, T_TEKLIF_NO, T_TESKILAT, T_USTBIRIM, T_SINIF, T_UNVAN, T_KADRO, T_ALT_IL, T_ALT_TIP, T_ALT_ILCE
       T_SAY_IL, T_SAY_TIP, T_SAY_ILCE, T_TENTAHGUN, T_TENTAHAY, T_TENTAHYIL, T_VIZEGUN, T_VIZEAY, T_VIZEYIL, T_ICMAL_NO, T_ADET
       T_DOLU_BOS, T_TARIH, T_DURUM 
    
     * Bu alanlardan T_YIL, T_TEKLIF_NO, T_VIZEGUN, T_VIZEAY, T_VIZEYIL, T_ICMAL_NO bilgileri KADRO tablosuna direk kayıt edilecek
     * Bu alanlardan T_TENTAH, T_ADET bilgileri hesaplanarak KADRO tablosunda DOLU,BOS,ADET alanlarına yazılacak
     * T_TENTAH --> 0 ise Tenkis (Eksi veya İptal), 1 ise Tahsis(Artı veya İhdas),
     * T_VIZEGUN, T_VIZEAY, T_VIZEYIL SGBye gönderilen tarihi veriyor
     * 
     * Kadro Tablosuna Personel GM 'de olmayan
     * K_SGBISLEMTARIH, K_TAHTENVIZETARIH, K_TAHTENTEKLIFNO, K_TAHTENICMALNO alanları açıldı.
     * TAHSIS_TENKIS işlemleri kadro tablosuna aktarılıyor.
     * Personelden alınan ilk Kadro bilgileri kalıyor ve tabloya TahsisTenkis bilgileri yeni satır olarak ekleniyor
     * İlk alınan Kadro bilgilerinin Yıl bilgisi 2000 dir.
     *********************************/


    class PERKadro
    {
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray p)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";
            //int[] unvanlar = new int[2000];
            //int[] siniflar = new int[2000];
            //int[] ustBirimler = new int[2000];
            int satir = 1;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadro tab = new TABKadro(ve);

            foreach (Kadro k in p.objeler)
            {
                if (k.teskilat == 0)
                    hata += satir + " nolu satırda Teşkilat kodu boş bırakılamaz<br>";
                if (k.teskilat != 1 && k.teskilat != 2)
                    hata += satir + " nolu satırda Teşkilat kodu hatalı girilmiş<br>";
                if (k.ustBirim == 0)
                    hata += satir + " nolu satırda Üst Birim kodu boş bırakılamaz<br>";
                if (k.sinif == 0)
                    hata += satir + " nolu satırda Sınıf kodu boş bırakılamaz<br>";
                if (k.unvan == 0)
                    hata += satir + " nolu satırda Unvan kodu boş bırakılamaz<br>";
                if (k.kadro == 0)
                    hata += satir + " nolu satırda Kadro Derece kodu boş bırakılamaz<br>";

                //int indexYer = 0;
                ////ünvan kontrolü
                //bool bulundu = false;
                //for (int i = 0; i < unvanlar.Length; i++)
                //{
                //    if (unvanlar[i] == k.unvan)
                //    {
                //        bulundu = true;
                //        break;
                //    }
                //    if (unvanlar[i] == 0)
                //        break;
                //}

                //if (!bulundu)
                //{
                //    unvanlar[indexYer] = k.unvan;
                //    if (!tab.UnvanVarmi(k.unvan))
                //        hata += k.unvan + " kodlu ünvan karşılığı sözlük tablosunda yok ilk önce bu ünvanı tanımlayın.<br>";
                //}

                ////sınıf kontrolü
                //bulundu = false;
                //for (int i = 0; i < siniflar.Length; i++)
                //{
                //    if (siniflar[i] == k.sinif)
                //    {
                //        bulundu = true;
                //        break;
                //    }
                //    if (siniflar[i] == 0)
                //        break;
                //}

                //if (!bulundu)
                //{
                //    siniflar[indexYer] = k.sinif;
                //    if (!tab.SinifVarmi(k.sinif))
                //        hata += k.sinif + " kodlu sınıf karşılığı sözlük tablosunda yok ilk önce bu ünvanı tanımlayın.<br>";
                //}

                ////Üst Birim kontrolü
                //bulundu = false;
                //for (int i = 0; i < ustBirimler.Length; i++)
                //{
                //    if (ustBirimler[i] == k.ustBirim)
                //    {
                //        bulundu = true;
                //        break;
                //    }
                //    if (ustBirimler[i] == 0)
                //        break;
                //}

                //if (!bulundu)
                //{
                //    ustBirimler[indexYer] = k.ustBirim;
                //    if (!tab.UstBirimVarmi(k.ustBirim))
                //        hata += k.ustBirim + " kodlu Üst Birim karşılığı sözlük tablosunda yok ilk önce bu ünvanı tanımlayın.<br>";
                //}

                satir++;
            }

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve.BeginTransaction();

            foreach (Kadro k in p.objeler)
            {
                sonuc = tab.Kaydet(k);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray Rapor(Kullanici kullanan, Kadro p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            TABKadro tab = new TABKadro(ve);
            TABKadro tab2 = new TABKadro(ve2);

            IDataReader r = null;

            try
            {
                r = tab.Rapor(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                Kadro pp = new Kadro();

                pp.teskilat = VeriErisim.DegerAl(r, 0, 0);
                pp.ustBirim = VeriErisim.DegerAl(r, 1, 0);
                pp.sinif = VeriErisim.DegerAl(r, 2, 0);
                pp.unvan = VeriErisim.DegerAl(r, 3, 0);
                pp.kadro = VeriErisim.DegerAl(r, 4, 0);
                pp.sayIl = VeriErisim.DegerAl(r, 5, 0);
                pp.sayTip = VeriErisim.DegerAl(r, 6, 0);
                pp.sayIlce = VeriErisim.DegerAl(r, 7, 0);
                pp.dolu = VeriErisim.DegerAl(r, 8, 0);
                pp.bos = VeriErisim.DegerAl(r, 9, 0);
                pp.adet = VeriErisim.DegerAl(r, 10, 0);
                pp.unvanAd = VeriErisim.DegerAl(r, 11, "");
                pp.sinifAd = VeriErisim.DegerAl(r, 12, "");
                pp.teskilatAd = VeriErisim.DegerAl(r, 13, "");
                pp.ustBirimAd = VeriErisim.DegerAl(r, 14, "");
                pp.muhasebeAd = VeriErisim.DegerAl(r, 15, "");

                if (!p.vizeTarih.isNull)
                {
                    pp.vizeTarih = p.vizeTarih;
                    IDataReader r2 = tab2.OncesiDoluBos(pp);
                    if (r2.Read())
                    {
                        pp.oncesiDolu = VeriErisim.DegerAl(r2, 0, 0);
                        pp.oncesiBos = VeriErisim.DegerAl(r2, 1, 0);
                        pp.oncesiAdet = VeriErisim.DegerAl(r2, 2, 0);
                        r2.Close();
                    }
                }

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc Sil(Kullanici kullanan, TNSDateTime tarih)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABKadro tab = new TABKadro(ve);

            sonuc = tab.Sil(tarih);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray VizeListele()
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));

            TABKadro tab = new TABKadro(ve);

            IDataReader r = null;

            try
            {
                r = tab.VizeListele();
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                string tarih = VeriErisim.DegerAl(r, 0).ToString();
                pVAN.objeler.Add(tarih);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }
    }
}

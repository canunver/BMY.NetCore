using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.KYM;
using TNS.Veri;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELKomisyonUye
    {

        protected static string KayitKontrol(Kullanici kullanan, KomisyonUye bilgi,ENUMIslemTuru islemturu,int islemkodu=0,int parcakodu=0)
        {

            StringBuilder hatalar = new StringBuilder();
            if (islemturu == ENUMIslemTuru.KAYIT || islemturu == ENUMIslemTuru.SILME)
            {
                if (islemkodu <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00109 + "<br/>");
                if (parcakodu <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00110 + "<br/>");
            }
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            if (bilgi.yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00051 + "<br/>");

            if (bilgi.muhasebe <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00052 + "<br/>");

            if (bilgi.birim == null || bilgi.birim.Trim() == string.Empty)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00053 + "<br/>");

            if (bilgi.surecNo == null || bilgi.surecNo.Trim() == string.Empty)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00054 + "<br/>");
            if (islemturu!=ENUMIslemTuru.LISTELEME)
            {
                if (bilgi.tip <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00055 + "<br/>");
            }
            
            return hatalar.ToString();

        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KomisyonUye bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            string kayitHatasi = KayitKontrol(kullanan, bilgi,ENUMIslemTuru.KAYIT,islemkod,parcakod);
            if (kayitHatasi.Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = kayitHatasi;
                return sonuc;
            }
            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi.yil, bilgi.surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABKomisyonUye tabUye = new TABKomisyonUye(ve);
            ve.BeginTransaction();

            tabUye.Sil(bilgi);
            if (ve.hata.Trim() == "")
            {
                sonuc.islemSonuc = true;
                foreach (Uye uye in bilgi.uyeler.objeler)
                {
                    if (!sonuc.islemSonuc) break;
                    sonuc = tabUye.Kaydet(bilgi, uye);
                }
                if (sonuc.islemSonuc)
                {
                    //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                    Surec s = new Surec();
                    s.yil = bilgi.yil;
                    s.surecNo = bilgi.surecNo;
                    s.surecDurum = (int)TNS.HRC.ENUMSurecDurum.DEVAMEDIYOR;
                    sonuc = GELSurec.DurumGuncelle(ve, kullanan, s);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00025;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                else
                {
                    ve.Rollback();
                }
            }
            else
            {
                ve.Rollback();
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
            }
            ve.VeriErisimKapat();
            return sonuc;

        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, KomisyonUye bilgi)
        {
            ObjectArray uyeListesi = new ObjectArray();
            string kayitHatasi = KayitKontrol(kullanan, bilgi,ENUMIslemTuru.LISTELEME);
            if (kayitHatasi.Trim() != "")
            {
                uyeListesi.sonuc.islemSonuc = false;
                uyeListesi.sonuc.hataStr = kayitHatasi;
                return uyeListesi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");

            TABKomisyonUye tabKomUye = new TABKomisyonUye(ve);
            ve.BeginTransaction();
            IDataReader dr = tabKomUye.Listele(bilgi);
            int sayac = 0;
            KomisyonUye komUye = new KomisyonUye();
            while (dr.Read())
            {
                if (sayac == 0)
                {
                    komUye.yil = VeriErisim.DegerAl(dr, 0, 0);
                    komUye.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    komUye.birim = VeriErisim.DegerAl(dr, 2, "");
                    komUye.surecNo = VeriErisim.DegerAl(dr, 3, "");
                    komUye.tip = VeriErisim.DegerAl(dr, 4, 0);
                    komUye.gorevlendirmeTarih = VeriErisim.DegerAl(dr, 11);
                    komUye.gorevlendirmeNo = VeriErisim.DegerAl(dr, 12, 0);
                    komUye.uyeler = new ObjectArray();
                }
                sayac++;
                Uye uye = new Uye();
                uye.siraNo = VeriErisim.DegerAl(dr, 5, 0);
                uye.tcKimlikNo = VeriErisim.DegerAl(dr, 6, "");
                uye.ad = VeriErisim.DegerAl(dr, 7, "");
                uye.gorev = VeriErisim.DegerAl(dr, 8, "");
                uye.unvan = VeriErisim.DegerAl(dr, 9, "");
                uye.komisyonGorev = VeriErisim.DegerAl(dr, 10, 0);
                komUye.uyeler.objeler.Add(uye);             
                
            }
            uyeListesi.objeler.Add(komUye);
            dr.Close();
            ve.VeriErisimKapat();
            return uyeListesi;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KomisyonUye bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            string kayitHatasi = KayitKontrol(kullanan, bilgi,ENUMIslemTuru.SILME,islemkod,parcakod);
            if (kayitHatasi.Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = kayitHatasi;
                return sonuc;
            }
            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi.yil, bilgi.surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABKomisyonUye tabKomUye = new TABKomisyonUye(ve);
            ve.BeginTransaction();
            tabKomUye.Sil(bilgi);
            if (ve.hata.Trim() == "")
            {
                //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                sonuc.islemSonuc = true;
                sonuc.hataStr = "";
                Surec s = new Surec();
                s.yil = bilgi.yil;
                s.surecNo = bilgi.surecNo;
                s.surecDurum = (int)TNS.HRC.ENUMSurecDurum.DEVAMEDIYOR;
                sonuc = GELSurec.DurumGuncelle(ve, kullanan, s);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00056;
                }
                else
                {
                    ve.Rollback();
                }
            }
            else
            {
                ve.Rollback();
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}

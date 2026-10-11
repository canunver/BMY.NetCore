using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELTahsilat
    {
        public static string TahsilatTaksitKontrol(TasinmazOdemePlan bilgi, int islemkodu, int parcakodu)
        {
            StringBuilder hatalar = new StringBuilder();

            if (islemkodu <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00109 + "<br/>");
            if (parcakodu <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00110 + "<br/>");

            if (bilgi.yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");
            if (bilgi.muhasebe <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00023 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.birim))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00024 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.surecNo))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00026 + "<br/>");
            if (bilgi.siraNo <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00070 + "<br/>");

            return hatalar.ToString();
        }

        public static String TahsilatBilgiKontrol(Tahsilat bilgi, ENUMIslemTuru islemturu, int islemkodu = 0, int parcakodu = 0)
        {
            StringBuilder hatalar = new StringBuilder();
            if (islemturu == ENUMIslemTuru.KAYIT || islemturu == ENUMIslemTuru.SILME)
            {
                if (islemkodu <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00109 + "<br/>");
                if (parcakodu <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00110 + "<br/>");
            }
            if (bilgi.yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");
            if (bilgi.muhasebe <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00023 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.birim))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00024 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.surecNo))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00026 + "<br/>");
            if (islemturu == ENUMIslemTuru.KAYIT)
            {
                if (bilgi.odemePlan.objeler.Count == 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00069 + "<br/>");
                if (bilgi.ihaleTarihi == null)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00071 + "<br/>");
                if (string.IsNullOrEmpty(bilgi.ihaleKararNo))
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00072 + "<br/>");
                if (bilgi.ihaleKararTarihi == null)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00073 + "<br/>");
                if (string.IsNullOrEmpty(bilgi.sozlesmeNo))
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00074 + "<br/>");
                if (bilgi.sozlesmeTarihi == null)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00075 + "<br/>");
                if (bilgi.sozlesmeBedeli <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00076 + "<br/>");
                if (bilgi.isinSuresi <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00077 + "<br/>");
                if (bilgi.teslimTarihi == null)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00078 + "<br/>");
                if (bilgi.bitimTarihi == null)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00079 + "<br/>");
                if (bilgi.firma.firmaNo <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00057 + "<br/>");
                if (string.IsNullOrEmpty(bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu) )
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00058 + "<br/>");

                if (bilgi.teslimTarihi != null && bilgi.bitimTarihi != null)
                {
                    if (DateTime.Compare(bilgi.teslimTarihi.Oku(), bilgi.bitimTarihi.Oku()) >= 0)
                        hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00080 + "<br/>");

                }
            }
            return hatalar.ToString();
        }

        public static Sonuc TahsilatBilgiKaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, Tahsilat bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            string hata = TahsilatBilgiKontrol(bilgi, ENUMIslemTuru.KAYIT, islemkod, parcakod);
            if (hata.Trim().Length > 0) hatalar.Append(hata);

            if (hatalar.ToString().Length > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
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
            TABTahsilat tabHakedis = new TABTahsilat(ve);
            ve.BeginTransaction();
            if (tabHakedis.BilgiKaydiVarmi(bilgi))
                sonuc = tabHakedis.BilgiGuncelle(bilgi);
            else
                sonuc = tabHakedis.BilgiKaydet(bilgi);
            // tabHakedis.TaksitSil(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo, 0, bilgi.firma.firmaNo, bilgi.tasinmaz.tasinmazBilgisi.kod, bilgi.tekliftipi);
            if (sonuc.islemSonuc && ve.hata.Trim() == "")
            {
                foreach (TasinmazOdemePlan item in bilgi.odemePlan.objeler)
                {
                    if (!sonuc.islemSonuc) break;
                    if (tabHakedis.TaksitKaydiVarmi(item))
                        sonuc = tabHakedis.TaksitGuncelle(item);
                    else
                        sonuc = tabHakedis.TaksitKaydet(item);
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

        public static ObjectArray TahsilatBilgiListele(VeriErisim veriErisim, KYM.Kullanici kullanan, Tahsilat bilgi)
        {
            ObjectArray hakedisler = new ObjectArray();

            string hata = TahsilatBilgiKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != "")
            {
                hakedisler.sonuc.islemSonuc = false;
                hakedisler.sonuc.hataStr = hata;
                return hakedisler;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTahsilat tabHakedis = new TABTahsilat(ve);

            TABMakbuzForm tabMakbuz = new TABMakbuzForm(ve);
            TABMuhasebeIslemForm tabMuhIslemForm = new TABMuhasebeIslemForm(ve);

            Makbuz makbuz = new Makbuz();
            MuhasebeIslemForm muhform = new MuhasebeIslemForm();

            ve.BeginTransaction();
            IDataReader dr = tabHakedis.BilgiListele(bilgi);
            while (dr.Read())
            {
                Tahsilat hake = new Tahsilat();
                hake.yil = VeriErisim.DegerAl(dr, 0, 0);
                hake.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                hake.birim = VeriErisim.DegerAl(dr, 2, "");
                hake.surecNo = VeriErisim.DegerAl(dr, 3, "");

                hake.firma.firmaNo = VeriErisim.DegerAl(dr, 4, 0);
                hake.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 5, "");

                hake.ihaleTarihi = VeriErisim.DegerAl(dr, 6);
                hake.ihaleKararNo = VeriErisim.DegerAl(dr, 7, "");
                hake.ihaleKararTarihi = VeriErisim.DegerAl(dr, 8);
                hake.sozlesmeNo = VeriErisim.DegerAl(dr, 9, "");
                hake.sozlesmeTarihi = VeriErisim.DegerAl(dr, 10);
                hake.sozlesmeBedeli = VeriErisim.DegerAl(dr, 11, 0.0);
                hake.isinSuresi = VeriErisim.DegerAl(dr, 12, 0);
                hake.isinSuresiTuru = VeriErisim.DegerAl(dr, 13, 0);
                hake.teslimTarihi = VeriErisim.DegerAl(dr, 14);
                hake.bitimTarihi = VeriErisim.DegerAl(dr, 15);
                hake.tekliftipi = VeriErisim.DegerAl(dr, 16, 0);

                hakedisler.objeler.Add(hake);
            }
            dr.Close();
            foreach (Tahsilat item in hakedisler.objeler)
            {
                dr = tabHakedis.TaksiltListele(item.yil, item.muhasebe, item.birim, item.surecNo, item.firma.firmaNo, item.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu);
                while (dr.Read())
                {
                    TasinmazOdemePlan plan = new TasinmazOdemePlan();
                    plan.yil = VeriErisim.DegerAl(dr, 0, 0);
                    plan.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    plan.birim = VeriErisim.DegerAl(dr, 2, "");
                    plan.surecNo = VeriErisim.DegerAl(dr, 3, "");
                    plan.firma.firmaNo = VeriErisim.DegerAl(dr, 4, 0);
                    plan.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 5, "");
                    plan.siraNo = VeriErisim.DegerAl(dr, 6, 0);
                    plan.odemeTarihi = VeriErisim.DegerAl(dr, 7);
                    plan.odemeTutari = VeriErisim.DegerAl(dr, 8, 0.0);
                    plan.fiyatFarkiOrani = VeriErisim.DegerAl(dr, 9, 0.0);
                    plan.iscilikFiyatFarki = VeriErisim.DegerAl(dr, 10, 0.0);
                    plan.tekliftipi = VeriErisim.DegerAl(dr, 11, 0);
                    item.odemePlan.objeler.Add(plan);
                }
                dr.Close();
                foreach (TasinmazOdemePlan plan in item.odemePlan.objeler)
                {
                    makbuz.yil = plan.yil;
                    makbuz.muhasebe = plan.muhasebe;
                    makbuz.birim = plan.birim;
                    makbuz.surecNo = plan.surecNo;
                    makbuz.firma.kod = plan.firma.firmaNo;
                    makbuz.tasinmaz.tasinmazSistemKodu = plan.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu;
                    makbuz.taksitno = plan.siraNo;
                    dr = tabMakbuz.BilgiListele(makbuz);
                    while (dr.Read())
                    {
                        plan.prmakbuzno = VeriErisim.DegerAl(dr, 7, 0);
                        plan.makbuzno = VeriErisim.DegerAl(dr, 8, "");
                    }
                    dr.Close();

                    muhform.yil = plan.yil;
                    muhform.muhasebe = plan.muhasebe;
                    muhform.birim = plan.birim;
                    muhform.surecNo = muhform.surecNo;
                    muhform.firma.kod = plan.firma.firmaNo;
                    muhform.tasinmaz.tasinmazSistemKodu = plan.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu;
                    muhform.taksitno = plan.siraNo;
                    dr = tabMuhIslemForm.BilgiListele(muhform);
                    while (dr.Read())
                    {
                        plan.prmuhfisno = VeriErisim.DegerAl(dr, 8, 0);
                        plan.muhfisno = VeriErisim.DegerAl(dr, 9, "");
                    }
                    dr.Close();
                }


            }


            return hakedisler;
        }

        public static Sonuc TaksitSil(VeriErisim veriErisim, KYM.Kullanici kullanan, TasinmazOdemePlan[] bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = string.Empty;
            if (bilgi.Length == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00108;
                return sonuc;
            }
            foreach (TasinmazOdemePlan item in bilgi)
            {
                hata = TahsilatTaksitKontrol(item, islemkod, parcakod);
                if (hata.Trim().Length > 0) hatalar.Append(hata);
            }

            if (hatalar.ToString().Length > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }
            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi[0].yil, bilgi[0].surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTahsilat tabHakedis = new TABTahsilat(ve);
            ve.BeginTransaction();
            sonuc.islemSonuc = true;

            TABMakbuzForm tabMakbuz = new TABMakbuzForm(ve);
            Makbuz makbuz = new Makbuz();
            int satirno = 1;
            string bilgiStr = string.Empty;
            foreach (TasinmazOdemePlan item in bilgi)
            {
                if (!sonuc.islemSonuc) break;
                makbuz.yil = item.yil;
                makbuz.muhasebe = item.muhasebe;
                makbuz.birim = item.birim;
                makbuz.surecNo = item.surecNo;
                makbuz.firma.kod = item.firma.firmaNo;
                makbuz.tasinmaz.tasinmazSistemKodu = item.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu;
                makbuz.taksitno = item.siraNo;
                if (tabMakbuz.BilgiKaydiVarmi(makbuz))
                    bilgiStr += satirno.ToString() + TNS.GEL.GelirSurecleriResI.GEL00087 + "</br>";
                else
                    tabHakedis.TaksitSil(item.yil, item.muhasebe, item.birim, item.surecNo, item.siraNo, item.firma.firmaNo, item.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu, item.tekliftipi);

                if (ve.hata.Trim() == "")
                    sonuc.islemSonuc = true;
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
                satirno++;
            }
            if (sonuc.islemSonuc)
            {
                //ve.Commit();
                //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                Surec s = new Surec();
                s.yil = bilgi[0].yil;
                s.surecNo = bilgi[0].surecNo;
                s.surecDurum = (int)TNS.HRC.ENUMSurecDurum.DEVAMEDIYOR;
                sonuc = GELSurec.DurumGuncelle(ve, kullanan, s);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                }
                else
                {
                    ve.Rollback();
                }
                if (bilgiStr.Trim() != string.Empty)
                {
                    sonuc.bilgiStr = bilgiStr;
                }
                else
                {
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00056;
                }
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }


    }
}

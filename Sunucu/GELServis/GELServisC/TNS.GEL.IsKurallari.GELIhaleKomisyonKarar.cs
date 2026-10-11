using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELIhaleKomisyonKarar
    {
        private static string KomisyonKararKontrol(KomisyonKarar bilgi, ENUMIslemTuru islemturu, int islemkodu = 0, int parcakodu = 0)
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
            if (bilgi.tekliftipi <= 0 && islemturu != ENUMIslemTuru.LISTELEME)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00063 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.tasinmaz.tasinmazSistemKodu) && islemturu != ENUMIslemTuru.LISTELEME && bilgi.tekliftipi == (int)ENUMIHALETeklifTuru.KISMITEKLIF)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00058 + "<br/>");
            //if (bilgi.uygunfirma1.firmaNo <= 0 && islemturu == ENUMIslemTuru.Kayit)
            //    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00064 + "<br/>");
            //if (bilgi.tutari <= 0 && islemturu == ENUMIslemTuru.Kayit)
            //    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00065 + "<br/>");
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, KomisyonKarar[] bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();

            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = "";
            string satirNo = "";
            if (bilgi.Length == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00108;
                return sonuc;
            }
            for (int i = 0; i < bilgi.Length; i++)
            {
                hata = KomisyonKararKontrol(bilgi[i], ENUMIslemTuru.KAYIT, islemkod, parcakod);
                satirNo = (i + 1).ToString();
                if (hata != "")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00061 + satirNo + "<br/>" + hata + "<br/>");
            }
            if (hatalar.ToString().Trim() != "")
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
            TABIhaleKomisyonKarar tabKomKarar = new TABIhaleKomisyonKarar(ve);
            ve.BeginTransaction();
            sonuc.islemSonuc = true;
            for (int i = 0; i < bilgi.Length; i++)
            {
                if (!sonuc.islemSonuc) break;
                KomisyonKarar komkarar = (KomisyonKarar)bilgi[i];
                if (komkarar.kararno.Trim() != string.Empty) // karar var güncelle
                {
                    sonuc = tabKomKarar.Guncelle(komkarar);
                }
                else//karar yok ekle
                {
                    TABGenel tg = new TABGenel(ve);
                    string sayacKod = komkarar.yil.ToString();
                    sayacKod = "GELKOMKARARNO" + sayacKod;
                    komkarar.kararno = tg.BelgeNoAl(sayacKod, "000000");
                    sonuc = tabKomKarar.Kaydet(komkarar);
                }
            }
            if (sonuc.islemSonuc)
            {
                //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                Surec s = new Surec();
                s.yil = bilgi.Length > 0 ? bilgi[0].yil : 0;
                s.surecNo = bilgi.Length > 0 ? bilgi[0].surecNo : string.Empty;
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
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, KYM.Kullanici kullanan, KomisyonKarar[] bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = "";
            int satirNo = 1;
            if (bilgi.Length == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00108;
                return sonuc;
            }
            foreach (KomisyonKarar item in bilgi)
            {
                hata = KomisyonKararKontrol(item, ENUMIslemTuru.SILME, islemkod, parcakod);
                if (hata != "")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00061 + satirNo.ToString() + "<br/>" + hata + "<br/>");
            }

            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString(); ;
                return sonuc;
            }
            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi[0].yil, bilgi[0].surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                sonuc.anahtar = parcatamamlandi.ToString();
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleKomisyonKarar tabKomKarar = new TABIhaleKomisyonKarar(ve);
            ve.BeginTransaction();
            StringBuilder bilgiler = new StringBuilder();

            //*buradaki amaç komisyon kararı  silinmeden önce
            //*tahsilat bilgisi girilmişse silinmesini engellemektir

            TABTahsilat tabTahsilat = new TABTahsilat(ve);
            Tahsilat tahsilat = new Tahsilat();
            bool tahsilatKaydiVar = false;
            sonuc.islemSonuc = true;

            foreach (KomisyonKarar karar in bilgi)
            {
                if (ve.hata.Trim() != "") break;
                tahsilat.yil = karar.yil;
                tahsilat.muhasebe = karar.muhasebe;
                tahsilat.birim = karar.birim;
                tahsilat.surecNo = karar.surecNo;
                tahsilat.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu = karar.tasinmaz.tasinmazSistemKodu;

                tahsilatKaydiVar = tabTahsilat.BilgiKaydiVarmi(tahsilat, true);
                if (!tahsilatKaydiVar)
                    tabKomKarar.Sil(karar);
                else
                    bilgiler.Append(karar.kararno + TNS.GEL.GelirSurecleriResI.GEL00098);

            }
            if (ve.hata.Trim() == "")
            {
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
                if (bilgiler.ToString() != "")
                {
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00056 + "<br/>";
                    sonuc.bilgiStr += bilgiler.ToString();
                }
                else
                {
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00056;
                }
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, KYM.Kullanici kullanan, KomisyonKarar bilgi)
        {
            ObjectArray komisyonlararlari = new ObjectArray();
            string hata = KomisyonKararKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != "")
            {
                komisyonlararlari.sonuc.islemSonuc = false;
                komisyonlararlari.sonuc.hataStr = hata;
                return komisyonlararlari;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleKomisyonKarar tabKomKarar = new TABIhaleKomisyonKarar(ve);
            ve.BeginTransaction();
            IDataReader dr = tabKomKarar.Listele(bilgi);
            while (dr.Read())
            {
                KomisyonKarar karar = new KomisyonKarar();
                karar.kararno = VeriErisim.DegerAl(dr, 0, "");
                karar.yil = bilgi.yil;
                karar.muhasebe = bilgi.muhasebe;
                karar.birim = bilgi.birim;
                karar.surecNo = bilgi.surecNo;
                karar.tasinmaz.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 5, "");
                karar.tekliftipi = VeriErisim.DegerAl(dr, 6, 0);

                karar.uygunfirma1.firmaNo = VeriErisim.DegerAl(dr, 7, 0);
                karar.uygunfirma1.ad = GELServis.servisUZY.UzayDegeriStr(kullanan, "HRCFIRMABILGISI", "-" + karar.muhasebe + "-" + karar.uygunfirma1.firmaNo, false);
                karar.uygunfirma1.tekliftutari = VeriErisim.DegerAl(dr, 8, 0.0);


                karar.uygunfirma2.firmaNo = VeriErisim.DegerAl(dr, 9, 0);
                karar.uygunfirma2.ad = GELServis.servisUZY.UzayDegeriStr(kullanan, "HRCFIRMABILGISI", "-" + karar.muhasebe + "-" + karar.uygunfirma2.firmaNo, false);
                karar.uygunfirma2.tekliftutari = VeriErisim.DegerAl(dr, 10, 0.0);

                karar.aciklama = VeriErisim.DegerAl(dr, 11, "");
                karar.karartarihi = VeriErisim.DegerAl(dr, 12);
                komisyonlararlari.objeler.Add(karar);
            }
            dr.Close();
            TABTasinmazTanim tabTTanim = new TABTasinmazTanim(ve);
            foreach (KomisyonKarar item in komisyonlararlari.objeler)
            {
                item.tasinmaz.tasinmazAd = tabTTanim.TasinmazAdiGetir(item.tasinmaz.tasinmazSistemKodu, item.tasinmaz.yil);
            }

            return komisyonlararlari;
        }

        public static Sonuc KararVarmi(VeriErisim veriErisim, KYM.Kullanici kullanan, KomisyonKarar bilgi)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            string hata = "";
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            hata = KomisyonKararKontrol(bilgi, ENUMIslemTuru.KAYITKONTROL);
            if (hata != "")
                hatalar.Append(hata + "<br/>");
            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString(); ;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleKomisyonKarar tabKomKarar = new TABIhaleKomisyonKarar(ve);
            ve.BeginTransaction();
            sonuc.islemSonuc = true;
            bool kayitdurum = tabKomKarar.KayitVarmi(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo, bilgi.tasinmaz.tasinmazSistemKodu, bilgi.tekliftipi);
            if (kayitdurum)
            {
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00096;
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00097;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}

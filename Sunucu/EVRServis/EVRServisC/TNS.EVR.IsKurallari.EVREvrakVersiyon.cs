using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakVersiyon
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, EvrakVersiyon form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakVersiyon to = new TABEvrakVersiyon(ve);

                sonuc = to.Kaydet(form);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{A89BE9AE-4817-4539-9105-E2052B2F91C4} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakVersiyon[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakVersiyon form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakVersiyon tab = new TABEvrakVersiyon(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakVersiyon();
                    form.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EVREVRAKNO, "");
                    form.yil = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.YIL, 0);
                    form.evrakTipi = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EVRAKTIPI, 0);
                    form.evrakBirimKodu = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EVRAKBIRIMKODU, "");
                    form.evrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EVRAKNO, 0);
                    form.evrakTarih = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EVRAKTARIH);
                    form.geldigiYerTur = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.GELDIGIYERTUR, 0);
                    form.geldigiYer = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.GELDIGIYER, "");
                    form.gelenEvrakTarih = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.GELENEVRAKTARIH);
                    form.gelenEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.GELENEVRAKNO, "");
                    form.gizlilik = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.GIZLILIK, 0);
                    form.gelisSekli = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.GELISSEKLI, 0);
                    form.yaziDurum = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.YAZIDURUM, 0);
                    form.konu = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.KONU, "");
                    form.aciklama = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.ACIKLAMA, "");
                    form.cevapSuresi = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.CEVAPSURESI, 0);
                    form.ekAdedi = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EKADEDI, "");
                    form.havaleBirimKodu = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.HAVALEBIRIMKODU, "");
                    form.paraf = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.PARAF, "");
                    form.evrakTuru = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EVRAKTURU, 0);
                    form.antet = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.ANTET, "");
                    form.icerik = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.ICERIK, "");
                    form.imza1 = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.IMZA1, "");
                    form.imza2 = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.IMZA2, "");
                    form.imza3 = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.IMZA3, "");
                    form.ek = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EK, "");
                    form.altBilgi = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.ALTBILGI, "");
                    form.evrakOlusturmaTarihi = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.OLUSTURMATARIHI);
                    form.imzaTarihi = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.IMZATARIHI);
                    form.ekYaziAlan = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EKYAZIALAN, "");
                    form.siraNo = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.SIRANO, 0);
                    form.dosyaNo = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.DOSYANO, "");
                    form.evrakIcerigi = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EVRAKICERIK, "");
                    form.evrakCinsi = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EVRAKCINSI, 0);
                    form.esasNo = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.ESASNO, "");
                    form.evrakBelgeTur = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.EVRAKBELGETUR, 0);
                    form.imzaYolu = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.IMZAYOLU, 0);

                    form.kod = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.KOD, 0);
                    form.versiyonAd = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.VERSIYONAD, "");
                    form.versiyonMu = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.VERSIYONMU, 0);
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.ISLEMYAPAN, "");
                    form.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.ISLEMYAPANAD, "");
                    form.islemTarih = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon.ENUMAlanlar.ISLEMTARIH);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakVersiyon[])liste.objeler.ToArray(typeof(EvrakVersiyon)); ;
        }

        public static EvrakVersiyon Oku(VeriErisim veriErisim, Kullanici kullanan, EvrakVersiyon form)
        {
            if (form.kod == 0) return new EvrakVersiyon();

            VeriErisim ve1 = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            EvrakVersiyon[] list = EVREvrakVersiyon.Listele(ve1, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            ve1.VeriErisimKapat();
            return form;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (kod <= 0)
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakVersiyon tab = new TABEvrakVersiyon(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(kod);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI097;
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = EvrakI.EVRI098 + "<br>" + sonuc.hataStr;
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{6B05C86D-52F9-4ED9-9B83-AE4DBB238BB1} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}

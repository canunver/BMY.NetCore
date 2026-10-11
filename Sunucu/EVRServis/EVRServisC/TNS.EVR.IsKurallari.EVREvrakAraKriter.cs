using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakAraKriter
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakAraKriter form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakAraKriter tab = new TABEvrakAraKriter(ve);
            TABGenel tabGenel = new TABGenel(ve);

            ve.BeginTransaction();
            try
            {
                if (form.kod == 0)
                {
                    string sayacKod = "EVREVRAKARAKRITER";
                    form.kod = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "0000000"), 0);
                }

                sonuc = tab.Kaydet(form);
                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{AFCA6435-EE84-4142-87D3-B3CAA711F3C6} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakAraKriter[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakAraKriter form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakAraKriter tab = new TABEvrakAraKriter(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakAraKriter();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.KOD, 0);
                    form.ad = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.AD, "");
                    form.islemyapan = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.ISLEMYAPAN, "");
                    form.islemtarih = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.ISLEMTARIH);
                    form.evrakbirimkodu = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.EVRAKBIRIMKODU, "");
                    form.evrakbirimkodualt = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.EVRAKBIRIMKODUALT, "");
                    form.yil = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.YIL, 0);
                    form.evraktipi = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.EVRAKTIPI, 0);
                    form.kayittarihi1 = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.KAYITTARIHI1);
                    form.kayittarihi2 = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.KAYITTARIHI2);
                    form.evrakno1 = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.EVRAKNO1, 0);
                    form.evrakno2 = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.EVRAKNO2, 0);
                    form.konu = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.KONU, "");
                    form.geldigiyer = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.GELDIGIYER, "");
                    form.geldigiyertur = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.GELDIGIYERTUR, 0);
                    form.gelenevrakno = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.GELENEVRAKNO, "");
                    form.gelenevraktarih = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.GELENEVRAKTARIH);
                    form.icerik = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.ICERIK, "");
                    form.yazisekli = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.YAZISEKLI, 0);
                    form.gelissekli = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.GELISSEKLI, 0);
                    form.gizlilik = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.GIZLILIK, 0);
                    form.yazidurum = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.YAZIDURUM, 0);
                    form.tasfiyeKod = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.TASFIYEKOD, 0);
                    form.saklamaSuresi = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.SAKLAMASURESI, 0);
                    form.saklamaKriteriKod = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.SAKLAMAKRITERIKOD, 0);
                    form.saklamaBaslangicKod = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.SAKLAMABASLANGICKOD, 0);
                    form.sorguTuru = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.SORGULAMATURU, 0);
                    form.dosyaNo = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.DOSYANO, "");
                    form.evrakIcerigi = VeriErisim.DegerAl(dr, (int)TABEvrakAraKriter.ENUMAlanlar.EVRAKICERIK, "");
                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakAraKriter[])liste.objeler.ToArray(typeof(EvrakAraKriter));
        }

        public static EvrakAraKriter Oku(VeriErisim veriErisim, Kullanici kullanan, EvrakAraKriter form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            EvrakAraKriter[] list = Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            ve.VeriErisimKapat();
            return form;
        }

    }
}

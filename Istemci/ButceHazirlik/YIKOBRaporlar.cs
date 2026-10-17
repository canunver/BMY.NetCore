using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.KYM;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Eski Istemci\ButceHazirlikv2\YIKOBRaporlar.cs'ten alındı (EKBILGI 21: kalan ödenekler, 22: kalan ödenekler detay).
    /// Değişenler: IRapor arayüzü ve System.Web / TNS.Raporlama using'leri kaldırıldı; yetki ve tanımsız ek bilgi
    /// durumlarında Exception/NotImplementedException yerine kullanıcıya gösterilecek RaporHatasi atılır.
    /// </summary>
    public class YIKOBRaporlar
    {
        Kullanici kullanan;
        private string sablonAd;
        private string sonucDosyaAd;
        private string ciktiYeri;
        private Hashtable raporParam;
        private string ekBilgi;
        private IBHMv2Servis bhmServis;

        public int ButceYil { get; set; }
        public int ay { get; set; }
        public string ParamKur { get; set; }
        public string ParamFon { get; set; }
        public string ParamFin { get; set; }
        public string ParamEko { get; set; }
        public string ParamPeb { get; set; }
        TNS.BKD.Tertip kistasTertip;

        Tablo XLS;
        public string RaporAl(object o_kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            this.ButceYil = OrtakFonksiyonlar.ConvertToInt(raporParam["ButceYil"], 1000);
            this.ay = OrtakFonksiyonlar.ConvertToInt(raporParam["AyNo"], 12);
            this.ParamKur = (string)raporParam["Kur"];
            this.ParamFon = (string)raporParam["Fon"];
            this.ParamFin = (string)raporParam["Fin"];
            this.ParamEko = (string)raporParam["Eko"];
            this.ParamPeb = (string)raporParam["Peb"];
            this.kullanan = (Kullanici)o_kullanan;

            if (ekBilgi == "21")
                kistasTertip = new TNS.BKD.Tertip(ButceYil, TNS.BKD.ENUMTabloKod.TABPEBOdenek, ParamKur, ParamFon, ParamFin, ParamEko, ParamPeb);
            else
                kistasTertip = new TNS.BKD.Tertip(ButceYil, TNS.BKD.ENUMTabloKod.TABMasrafPlani, ParamKur, ParamFon, ParamFin, ParamEko, ParamPeb);
            if (!TNS.BHMv2.Arac.Tanimla().OGYYetkisiVarMi(kullanan, "06.00", kistasTertip, ENUMOGY.Gorme))
            {
                throw new RaporHatasi("Verdiğiniz kodlarla ilgili rapor alma yetkiniz bulunmamaktadır, kodları değiştirip tekrar deneyiniz!");
            }

            this.sablonAd = sablonAd;
            XLS = GenelIslemler.NewTablo();
            this.sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            this.ciktiYeri = ciktiYeri;
            this.raporParam = raporParam;
            this.ekBilgi = ekBilgi;
            bhmServis = TNS.BHMv2.Arac.Tanimla();
            if (ekBilgi == "21")
                RaporAlKalanOdenekler();
            else if (ekBilgi == "22")
                RaporAlKalanOdeneklerDetay();
            else
                throw new RaporHatasi("YIKOBRaporlar: tanımsız ek bilgi (" + ekBilgi + ")");
            return this.sonucDosyaAd;
        }

        private void RaporAlKalanOdenekler()
        {
            UcgenKodBilgileri ucgenKodBilgileri = new UcgenKodBilgileri();
            ucgenKodBilgileri.Add(new UcgenKodBilgi(1, kistasTertip.KodNoKirilimSay(0)));
            ucgenKodBilgileri.Add(new UcgenKodBilgi(2, 4));
            ucgenKodBilgileri.Add(new UcgenKodBilgi(3, 1));
            ucgenKodBilgileri.Add(new UcgenKodBilgi(4, 2, true));
            ucgenKodBilgileri.Add(new UcgenKodBilgi(14, kistasTertip.KodNoKirilimSay(4)));
            EkDegerParam[] ekBilgiler = new EkDegerParam[4];
            ekBilgiler[0] = new EkDegerParam(TNS.BKD.ENUMTabloKod.TABButce, 0, 0, "");
            ekBilgiler[1] = new EkDegerParam(TNS.BKD.ENUMTabloKod.TABMasrafPlani, 0, 0, "10");
            ekBilgiler[2] = new EkDegerParam(TNS.BKD.ENUMTabloKod.TABOdenekGonderme, 0, 0, "");
            ekBilgiler[3] = new EkDegerParam(TNS.BKD.ENUMTabloKod.TABPEBHarcama, 0, 0, "");

            OdenekBilgi[] obler = bhmServis.ListeTertipOl(kullanan, kistasTertip, ucgenKodBilgileri, ekBilgiler, true, false, true);

            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLSOrtak();

            for (int i = 0; i < obler.Length; i++)
            {
                OdenekBilgi ob = obler[i];

                XLS.HucreKopyala(formatSatir, formatSutun, formatSatir, formatSutun + 13, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, ob.tertip.TertipStrOl());
                XLS.HucreDegerYaz(satir, sutun + 1, ob.tertip.adlar[0] + " - " + ob.tertip.adlar[3]);
                XLS.HucreDegerYaz(satir, sutun + 2, ob.EkRakam(0));
                XLS.HucreDegerYaz(satir, sutun + 3, ob.EkRakam(1));
                XLS.HucreDegerYaz(satir, sutun + 4, ob.EkRakam(2));
                XLS.HucreDegerYaz(satir, sutun + 5, ob.EkRakam(3));
                XLS.HucreDegerYaz(satir, sutun + 6, ob.EkRakam(0) - ob.EkRakam(2));
                XLS.HucreDegerYaz(satir, sutun + 7, ob.EkRakam(2) - ob.EkRakam(3));
                satir++;
            }
            XLSKapat();
        }

        private void RaporAlKalanOdeneklerDetay()
        {
            UcgenKodBilgileri ucgenKodBilgileri = new UcgenKodBilgileri();
            ucgenKodBilgileri.Add(new UcgenKodBilgi(1, kistasTertip.KodNoKirilimSay(0)));
            ucgenKodBilgileri.Add(new UcgenKodBilgi(2, 4));
            ucgenKodBilgileri.Add(new UcgenKodBilgi(3, 1));
            ucgenKodBilgileri.Add(new UcgenKodBilgi(4, 2, true));
            ucgenKodBilgileri.Add(new UcgenKodBilgi(14, kistasTertip.KodNoKirilimSay(4)));
            ucgenKodBilgileri.Add(new UcgenKodBilgi(17, kistasTertip.KodNoKirilimSay(5)));
            EkDegerParam[] ekBilgiler = new EkDegerParam[3];
            ekBilgiler[0] = new EkDegerParam(TNS.BKD.ENUMTabloKod.TABMasrafPlani, 0, 0, "10");
            ekBilgiler[1] = new EkDegerParam(TNS.BKD.ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "0/7");
            ekBilgiler[2] = new EkDegerParam(TNS.BKD.ENUMTabloKod.TABMIFKY, 0, 0, ay.ToString());
            OdenekBilgi[] obler = bhmServis.ListeTertipOl(kullanan, kistasTertip, ucgenKodBilgileri, ekBilgiler, true, false, true);

            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLSOrtak();

            for (int i = 0; i < obler.Length; i++)
            {
                OdenekBilgi ob = obler[i];

                XLS.HucreKopyala(formatSatir, formatSutun, formatSatir, formatSutun + 13, satir, sutun);

                string tut = ob.tertip.kodlar[5];
                ob.tertip.kodlar[5] = "";
                XLS.HucreDegerYaz(satir, sutun, ob.tertip.TertipStrOl());
                XLS.HucreDegerYaz(satir, sutun + 1, ob.tertip.adlar[4] + " - " + ob.tertip.adlar[3]);

                XLS.HucreDegerYaz(satir, sutun + 2, tut);
                XLS.HucreDegerYaz(satir, sutun + 3, ob.tertip.adlar[5]);

                XLS.HucreDegerYaz(satir, sutun + 4, ob.EkRakam(0));
                XLS.HucreDegerYaz(satir, sutun + 5, ob.EkRakam(1));
                XLS.HucreDegerYaz(satir, sutun + 6, ob.EkRakam(2));
                XLS.HucreDegerYaz(satir, sutun + 7, ob.EkRakam(0) - ob.EkRakam(1));
                XLS.HucreDegerYaz(satir, sutun + 8, ob.EkRakam(1) - ob.EkRakam(2));
                satir++;
            }
            XLSKapat();
        }

        private void DebugYazMa(string v)
        {
            OrtakFonksiyonlar.HataDosyaYaz("Debug.txt", v, false);
        }

        private void XLSKapat()
        {
            XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 7, satirYukseklik);
            XLS.DosyaSaklaTamYol();
        }

        int baslaSatir = 0;
        int satir = 0;
        int sutun = 0;
        int formatSatir = 0;
        int formatSutun = 0;
        int satirYukseklik;

        private void XLSOrtak()
        {
            XLS.HucreAdBulYaz("ButceYil", ButceYil);
            XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref formatSatir, ref formatSutun);
            satir = baslaSatir;
            satirYukseklik = XLS.SatirYukseklikAl(satir);
        }
    }
}
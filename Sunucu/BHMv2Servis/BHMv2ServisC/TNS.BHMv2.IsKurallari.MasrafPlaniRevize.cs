using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;
using TNS.MUH;
using TNS.UZY;
using System.Collections.Generic;

namespace TNS.BHMv2.IsKurallari
{
    public class MasrafPlaniRevize
    {
        static string VERIERISIMAD = "BaglantiSatiriBUM";
        static TNS.UZY.IUZYServis servisUZY = UZY.Arac.Tanimla();

        private static bool OnaylamaYetkisiVarMiForm(Kullanici kullanan, int yil, string kurKod, ENUMRevizeFormTip formTip)
        {
            int[] dizi = { 2, 4 }; //Ödenek gönderme yazma yetkisi olanlar, masraf planı revize formunu onaylayabilsin.
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (formTip == ENUMRevizeFormTip.MasrafPlaniRevize)
            {
                if (kullanan.KullaniciTipiIceriyorMu(17))
                {
                    bool ekle = MasrafPlani.KurumsalKodVarMi(kullanan, kurKod);
                    if (ekle)
                    {
                        if (MasrafPlani.CokluDegerIntIceriyorMu(kullanan.YetkiTipiDegeriBul("Ödenek Gönderme Yetkisi"), dizi))
                            return true;
                    }
                }
            }
            return false;
        }

        private static bool YazmaYetkisiVarMiForm(Kullanici kullanan, int yil, string kurKod)
        {
            int[] dizi = { 2, 4 };
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (kullanan.KullaniciTipiIceriyorMu(18))
            {
                return true;
            }

            if (kullanan.KullaniciTipiIceriyorMu(17))
            {
                bool ekle = MasrafPlani.KurumsalKodVarMi(kullanan, kurKod);
                if (ekle)
                {
                    if (MasrafPlani.CokluDegerIntIceriyorMu(kullanan.YetkiTipiDegeriBul("Ödenek Gönderme Yetkisi"), dizi))
                        return true;
                }
            }
            return false;
        }

        private static bool YazmaYetkisiVarMiFormKY(Kullanici kullanan, int yil, string kyKod)
        {
            int[] dizi = { 2, 4 };
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (kullanan.KullaniciTipiIceriyorMu(17))
            {
                return true;
            }

            if (kullanan.KullaniciTipiIceriyorMu(18))
            {
                if (string.IsNullOrEmpty(kyKod) || kyKod.StartsWith("9")) return false;

                return true;//???
            }
            return false;
        }

        private static bool GormeYetkisiVarMiForm(Kullanici kullanan, int yil, string kurKod)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            //if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
            //{
            //    if (SaymanligiGorurMu(OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0), OrtakFonksiyonlar.ConvertToInt(ky.saymanlikKod, -1)))
            //        return true;
            //}

            //if (kullanan.KullaniciTipiIceriyorMu(18))
            //{
            //    string tut = kullanan.YetkiTipiDegeriBul("Ödeme Emri/MİF Saymanlık-Birim Yetkisi");
            //    if (BirimYetkiTipindeVarMı(tut, ky))
            //        return true;
            //}

            if (kullanan.KullaniciTipiIceriyorMu(17))
            {
                bool ekle = MasrafPlani.KurumsalKodVarMi(kullanan, kurKod);
                if (ekle)
                {
                    return true;
                }
            }
            return false;
        }

        public static MPRevizeForm MPRevizeFormOku(VeriErisim veriErisim, Kullanici kullanan, int yil, int revizeNo, int odeGonYil = 0, int odeGonTur = 0, string odeGonNo = "")
        {
            if (revizeNo == 0 && odeGonYil == 0)
            {
                return null;
            }
            MPRevizeForm form = new MPRevizeForm();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);

            IDataReader dr = tab.RevizeFormOku(yil, revizeNo, odeGonYil, odeGonTur, odeGonNo);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception("Okuma Hatası: " + ve.hata);
            }
            form.yil = -1;
            if (dr.Read())
            {
                form.yil = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.YIL, 0);
                form.revizeNo = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.REVIZENO, 0);
                form.formDurum = (ENUMRevizeFormDurum)VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.FORMDURUM, 0);
                form.kaydedenKullaniciKodu = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.KAYDEDEN, "");
                form.kayitTarihi = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.KAYITTARIH);
                form.onaylayanKullaniciKodu = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ONAYLAYAN, "");
                form.onaylamaTarihi = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ONAYTARIH);
                form.revizeFormTip = (ENUMRevizeFormTip)VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.BELGETUR, 0);
                form.evrakTarih = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.EVRAKTARIH);
                form.evrakNo = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.EVRAKNO, "");
                form.evrakIlgi = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.EVRAKILGI, "");
                form.aciklama = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ACIKLAMA, "");
                form.bumkoKurumKod = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.BUMKOKURUMKOD, "");
                form.bumkoIslemNo = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.BUMKOISLEMNO, "");
                form.altIslemKod = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ALTISLEMKOD, "");
                form.yedekTertipStr = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.YEDEKTERTIPSTR, "");
                form.ilgiliRevizeNo = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ILGILIREVIZENO, -1);
                form.dayanakKodlar = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.DAYANAKKODLAR, "");
                form.odGonYil = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ODEGONYIL, 0);
                form.odGonTur = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ODEGONTUR, 0);
                form.odGonNo = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ODEGONNO, "");

                form.projelerArasi = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.PROJELERARASI, 0) == 1;
                form.sektorlerArasi = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.SEKTORLERARASI, 0) == 1;
                form.bolgelerArasi = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.BOLGELERARASI, 0) == 1;

                form.onayYaziNo = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ONAYYAZINO, "");
                form.onayYaziTarih = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ONAYYAZITARIH);
            }
            dr.Close();

            if (form.yil == -1)
            {
                ve.VeriErisimKapat();
                return null;
            }

            ArrayList al = new ArrayList();

            dr = tab.RevizeDetayOku(form.yil, form.revizeNo);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception("Okuma Hatası: " + ve.hata);
            }

            MPRevizeSatir satir = null;
            string eskiTertipStr = "";
            int eskiGrup = -1;
            int eskiIOIISKOD = -1;
            while (dr.Read())
            {
                Tertip okuTertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
                int gelenGrup = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.GRUP, 0);
                int gelenIOIISKOD = VeriErisim.DegerAl(dr, ((int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.IOIISKOD), 0);
                okuTertip.TertipAta(
                    VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.KURKOD, ""),
                    VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.FONKOD, ""),
                    VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.FINKOD, ""),
                    VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.EKOKOD, ""),
                    VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.PEBKOD, ""),
                    VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.KURUMSALYAPIKOD, "")
                );
                string yeniTertipStr = okuTertip.TertipStrOl();
                if (eskiTertipStr != yeniTertipStr || gelenGrup != eskiGrup || eskiIOIISKOD != gelenIOIISKOD)
                {
                    eskiGrup = gelenGrup;
                    eskiIOIISKOD = gelenIOIISKOD;
                    satir = new MPRevizeSatir();
                    satir.grup = gelenGrup;
                    satir.IOIISKOD = gelenIOIISKOD;
                    satir.tertip = okuTertip;
                    satir.tertip.Noktala();

                    satir.DPTPROJENO = VeriErisim.DegerAl(dr, ((int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.TERTIPSIRA) + 1, "");
                    satir.ALTPROJENO = VeriErisim.DegerAl(dr, ((int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.TERTIPSIRA) + 2, 0);

                    satir.sektorKod = VeriErisim.DegerAl(dr, ((int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.TERTIPSIRA) + 3, "");
                    satir.sektorAd = VeriErisim.DegerAl(dr, ((int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.TERTIPSIRA) + 4, "");
                    satir.bolgeler = BolgeOl(VeriErisim.DegerAl(dr, ((int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.TERTIPSIRA) + 5, ""));
                    satir.IOIISAD = VeriErisim.DegerAl(dr, ((int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.TERTIPSIRA) + 6, "");
                    satir.ALTPROJEAD = VeriErisim.DegerAl(dr, ((int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.TERTIPSIRA) + 7, "");
                    al.Add(satir);
                    eskiTertipStr = yeniTertipStr;
                }
                int ay = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.AY, 0);
                satir.odenek[ay - 1] = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarTERTIP.ODENEK, 0.0);
            }
            dr.Close();
            ve.VeriErisimKapat();

            form.satirlar = (MPRevizeSatir[])al.ToArray(typeof(MPRevizeSatir));

            if (form.satirlar.Length > 0)
            {
                if (!GormeYetkisiVarMiForm(kullanan, yil, form.satirlar[0].tertip.kodlar[0]))
                    throw new Exception("Revize formunu görme yetkiniz bulunmamaktadır!");
            }

            return form;
        }

        private static string BolgeOl(string xmlVeri)
        {
            string bolgeler = "";
            TNS.Veri.XMLStringDataReader drXml = new XMLStringDataReader(xmlVeri);
            if (drXml != null)
            {
                while (drXml.Read())
                {
                    if (bolgeler != "") bolgeler += ", ";
                    bolgeler += VeriErisim.DegerAl(drXml, 0, "");
                }
                drXml.Close();
            }
            return bolgeler;
        }

        public static void TekrarliSatirVarMi(MPRevizeSatir[] satirlar, Sonuc s)
        {
            for (int i = 0; i < satirlar.Length - 1; i++)
            {
                for (int j = i + 1; j < satirlar.Length; j++)
                {
                    if (satirlar[j].tertip.TertipStrOl() == satirlar[i].tertip.TertipStrOl() && satirlar[j].grup == satirlar[i].grup && satirlar[j].IOIISKOD == satirlar[i].IOIISKOD)
                        s.DegerAta((i + 1) + ". satırla, " + (j + 1) + ". satırda yazılan tertip (" + satirlar[j].tertip.TertipStrOl() + ") aynı!<br>");
                }
            }
        }

        private static void HTDecimalEkle(Hashtable ht, string htKod, decimal ekleDeger)
        {
            if (ht == null) return;
            object tut = ht[htKod];
            decimal tutD;
            if (tut != null)
            {
                tutD = (decimal)tut;
            }
            else
                tutD = 0;
            tutD += ekleDeger;
            ht[htKod] = tutD;
        }

        private static Sonuc RevizeFormKontrolEkleme(VeriErisim ve, Kullanici kullanan, MPRevizeForm form, Hashtable htEko2Ay, Hashtable htEko2, Sonuc s)
        {
            //string HataStr = "";

            //if (toplam != 0)
            //{
            //    HataStr += "<br />Yapılan Aktarma işlemi sonucunda, işlemin toplamı sıfırdan (" + toplam.ToString("#,###") + ") farklı oldu";
            //}

            //s.DegerAta(HataStr);
            return s;
        }

        private static Sonuc RevizeFormKontrolAktarma(VeriErisim ve, Kullanici kullanan, MPRevizeForm form, Hashtable htEko2Ay, Hashtable htEko2, Hashtable htPeb, Sonuc s, Hashtable projeBilgiler)
        {

            decimal toplam = 0;
            foreach (DictionaryEntry de in htEko2)
            {
                decimal odenek = (decimal)de.Value;
                toplam += odenek;
            }
            TABMPGMASRAFPLANI tabMPGMasrafPlani = new TABMPGMASRAFPLANI(ve);
            if (form.altIslemKod.Replace(".", "") == "020201" || form.altIslemKod.Replace(".", "") == "020205") //Kurum İçi Aktarma
            {
                if (toplam != 0)
                    s.DegerAta("<br />Yapılan Kurum içi aktarma işlemi sonucunda, işlemin toplamı sıfırdan (" + toplam.ToString("#,##0.00") + ") farklı oldu");
                else
                    SektorBolgeAktarmaKontrol(tabMPGMasrafPlani, form, htPeb, s, projeBilgiler);
            }
            return s;
        }

        private static void SektorBolgeAktarmaKontrol(TABMPGMASRAFPLANI tabMPGMasrafPlani, MPRevizeForm form, Hashtable ht, Sonuc s, Hashtable projeBilgiler)
        {
            Hashtable projeler = new Hashtable();
            Hashtable sektorler = new Hashtable();
            Hashtable bolgeler = new Hashtable();
            if (ht == null || ht.Count == 0) return;
            foreach (DictionaryEntry de in ht)
            {
                string peb = (string)de.Key;
                decimal odenek = (decimal)de.Value;
                if (odenek != 0)
                {
                    PEBProjeBilgi bilgi = ProjeBilgiBul(tabMPGMasrafPlani, form.yil, peb, projeBilgiler, true);
                    HTDecimalEkle(projeler, bilgi.DPTPROJEALTPROJENO, odenek);
                    HTDecimalEkle(sektorler, bilgi.sektorKod, odenek);
                    HTDecimalEkle(bolgeler, bilgi.bolgeler, odenek);
                }
            }
            string azalanProjeler = AzalanListe(projeler, s);
            string azalanSektorler = AzalanListe(sektorler, s);
            string azalanBolgeler = AzalanListe(bolgeler, s);
            if (string.IsNullOrWhiteSpace(form.odGonNo))
            {
                DegisimIsaretliMi(s, azalanProjeler, form.projelerArasi, "Projeler", "proje");
                DegisimIsaretliMi(s, azalanSektorler, form.sektorlerArasi, "Sektorler", "sektör");
                DegisimIsaretliMi(s, azalanBolgeler, form.bolgelerArasi, "Bölgeler", "bölge");
            }
            else
            {
                form.projelerArasi = string.IsNullOrWhiteSpace(azalanProjeler);
                form.sektorlerArasi = string.IsNullOrWhiteSpace(azalanSektorler);
                form.bolgelerArasi = string.IsNullOrWhiteSpace(azalanBolgeler);
                if (form.projelerArasi || form.sektorlerArasi || form.bolgelerArasi)
                {
                    if (string.IsNullOrWhiteSpace(form.onayYaziNo)) form.onayYaziNo = form.odGonNo;
                    if (form.onayYaziTarih == null || form.onayYaziTarih.isNull) form.onayYaziTarih = new TNSDateTime(DateTime.Now.Date);
                }
            }
        }

        private static void DegisimIsaretliMi(Sonuc s, string azalanListe, bool degisimVar, string listeAd, string degisenAd)
        {
            if (degisimVar && string.IsNullOrWhiteSpace(azalanListe))
                s.DegerAta(listeAd + " arası değişim yapılacağı işaretli olmasına rağmen, " + listeAd + "arası değişim bulunamadı!");
            else if (!degisimVar && !string.IsNullOrWhiteSpace(azalanListe))
                s.DegerAta(listeAd + " arası değişim yapılacağı işareti olmamasına rağmen, " + azalanListe + " kodlu " + degisenAd + "nin/lerin ödeneklerinde azalma meydana geldi!");
        }

        private static string AzalanListe(Hashtable ht, Sonuc s)
        {
            string liste = "";
            if (ht == null)
            {
                s.DegerAta("Kontrol sırasında toplam tablosu boş geldi");
            }
            else
            {
                foreach (DictionaryEntry de in ht)
                {
                    decimal odenek = (decimal)de.Value;
                    if (odenek < 0)
                    {
                        if (!string.IsNullOrWhiteSpace(liste)) liste += ",";
                        liste += (string)de.Key;
                    }
                }
            }
            return liste;
        }

        private static PEBProjeBilgi ProjeBilgiBul(TABMPGMASRAFPLANI tabMPGMasrafPlani, int yil, string peb, Hashtable projeBilgiler, bool nullYokEt)
        {
            PEBProjeBilgi tut = (PEBProjeBilgi)projeBilgiler[peb];
            if (tut != null) return tut;
            tut = new PEBProjeBilgi();
            IDataReader dr = tabMPGMasrafPlani.PEBBilgiBul(yil, peb);
            if (dr != null)
            {
                if (dr.Read())
                {
                    tut.DPTPROJENO = VeriErisim.DegerAl(dr, 0, "");
                    tut.ALTPROJENO = VeriErisim.DegerAl(dr, 1, 0);
                    tut.sektorKod = VeriErisim.DegerAl(dr, 2, "");
                    tut.sektorAd = VeriErisim.DegerAl(dr, 3, "");
                    tut.bolgeler = BolgeOl(VeriErisim.DegerAl(dr, 4, ""));
                }
                dr.Close();
            }
            if (nullYokEt)
            {
                if (string.IsNullOrWhiteSpace(tut.DPTPROJENO)) tut.DPTPROJENO = "0";
                if (string.IsNullOrWhiteSpace(tut.sektorKod)) tut.sektorKod = "0";
                if (string.IsNullOrWhiteSpace(tut.sektorAd)) tut.sektorAd = "0";
                if (string.IsNullOrWhiteSpace(tut.bolgeler)) tut.bolgeler = "T";
            }
            projeBilgiler[peb] = tut;
            return tut;
        }

        private static double UzayDegeriHesapla(Kullanici kullanan, VeriErisim ve, string uzayAd, string uzayKosul)
        {
            string sql = servisUZY.UzayKosulStr(kullanan, uzayAd, uzayKosul, false, "");
            return ve.TekKolonSQLDbl(sql);
        }

        private static Sonuc RevizeFormKontrolAHPRevize(VeriErisim ve, Kullanici kullanan, MPRevizeForm form, Hashtable htEko2peb, Sonuc s)
        {
            if (form.altIslemKod.Replace(".", "") == "010202") //AHP Aylar arası değişim
                HashtableToplam(form, htEko2peb, true, s, false);

            //foreach (DictionaryEntry de in htEko2Ay)
            //{
            //    decimal odenek = Math.Round((decimal)de.Value, 5);
            //    if (odenek != 0)
            //    {
            //        s.DegerAta("<br />Yapılan MP Revize işlemi sonucunda " + de.Key + " tertip/ay bilgisinde toplam sıfırdan (" + odenek.ToString("#,##0.00") + ") farklı oldu");
            //    }
            //}
            return s;
        }

        private static Sonuc RevizeFormKontrolMPRevize(VeriErisim ve, Kullanici kullanan, MPRevizeForm form, Hashtable htEko2PebAy, Hashtable htEko2peb, Hashtable htEko2Ay, Hashtable htPeb, Sonuc s, Hashtable projeBilgiler)
        {
            if (form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniRevize)
            {
                HashtableToplam(form, htEko2PebAy, true, s, true);
            }
            else if (form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniAHP)
            {
                HashtableToplam(form, htEko2peb, true, s, false);
                HashtableToplam(form, htEko2Ay, true, s, true);
                HashtableToplam(form, htEko2PebAy, false, s, true);
            }
            else if (form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniAktarma)
            {
                TABMPGMASRAFPLANI tabMPGMasrafPlani = new TABMPGMASRAFPLANI(ve);

                HashtableToplam(form, htEko2Ay, true, s, true);
                HashtableToplam(form, htEko2peb, false, s, false);
                SektorBolgeAktarmaKontrol(tabMPGMasrafPlani, form, htPeb, s, projeBilgiler);
            }
            return s;
        }

        private static void HashtableToplam(MPRevizeForm form, Hashtable ht, bool toplamSifir, Sonuc s, bool ayli)
        {
            if (ht == null) { s.DegerAta("Kontrol sırasında toplam tablosu boş geldi"); return; }
            string Ay = "/Ay";
            if (!ayli) Ay = "";
            int toplamSifirOlmayan = 0;
            foreach (DictionaryEntry de in ht)
            {
                decimal odenek = (decimal)de.Value;
                if (toplamSifir)
                {
                    if (odenek != 0)
                        s.DegerAta("<br />Yapılan " + TNS.BKD.Arac.RevizeFormTipAl(form.revizeFormTip) + " işlemi sonucunda " + de.Key + " tertip" + Ay + " toplamı sıfırdan (" + odenek.ToString("#,##0.00") + ") farklı oldu");
                }
                else
                {
                    if (odenek != 0) toplamSifirOlmayan++;
                }
            }
            if (!toplamSifir && toplamSifirOlmayan == 0)
                s.DegerAta("<br />Yapılan " + TNS.BKD.Arac.RevizeFormTipAl(form.revizeFormTip) + " işlemi sonucunda değişen bilgi bulunamadı!");
        }

        private static Sonuc RevizeFormKontrolGenel(VeriErisim ve, Kullanici kullanan, MPRevizeForm form, bool ters)
        {
            Sonuc s = new Sonuc(true, "", "");
            try
            {
                //if (form.formDurum == ENUMRevizeFormDurum.Musvette)
                //    return s;
                Hashtable projeBilgiler = new Hashtable();
                Hashtable htEko2PebAy = null;
                Hashtable htEko2Ay = null;
                Hashtable htEko2peb = null;
                Hashtable htKur = null;
                Hashtable htPeb = null;

                if (form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniRevize)
                {
                    htEko2PebAy = new Hashtable();
                }
                else if (form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniAHP)
                {
                    htEko2peb = new Hashtable();
                    htEko2Ay = new Hashtable();
                    htEko2PebAy = new Hashtable();
                }
                else if (form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniAktarma)
                {
                    htEko2peb = new Hashtable();
                    htEko2Ay = new Hashtable();
                    htPeb = new Hashtable();
                }
                else
                {
                    htEko2peb = new Hashtable();
                    htKur = new Hashtable();
                    htPeb = new Hashtable();
                }

                int toplamTertipSay = 0;
                for (int blg = 1; true; blg++)
                {
                    string HataStr = "";
                    if (htEko2PebAy != null) htEko2PebAy.Clear();
                    if (htEko2Ay != null) htEko2Ay.Clear();
                    if (htEko2peb != null) htEko2peb.Clear();
                    if (htKur != null) htKur.Clear();
                    if (htPeb != null) htPeb.Clear();
                    int tertipSay = 0;
                    for (int i = 0; i < form.satirlar.Length; i++)
                    {
                        if (form.satirlar[i].grup != blg) continue;
                        tertipSay++;
                        toplamTertipSay++;
                        string tertipStr = form.satirlar[i].tertip.TertipStrOl();

                        string tertipStrEko2 = (form.satirlar[i].tertip.kodlar[0] + "-" + form.satirlar[i].tertip.kodlar[1] + "-" + form.satirlar[i].tertip.kodlar[2] + "-" + form.satirlar[i].tertip.kodlar[3]).Replace(".", "");
                        string tertipPeb = form.satirlar[i].tertip.kodlar[4].Replace(".", "");
                        string tertipStrEko2Peb = tertipStrEko2 + "-" + tertipPeb;
                        string tertipKur = tertipStr.Replace(".", "").Substring(0, 4);

                        for (int ay = 0; ay < 12; ay++)
                        {
                            string tertipStrEko2Ay = tertipStrEko2 + "/" + (ay + 1).ToString();
                            string tertipStrEko2PebAy = tertipStrEko2Peb + "/" + (ay + 1).ToString();

                            decimal ayDeger = Convert.ToDecimal(form.satirlar[i].odenek[ay]);
                            HTDecimalEkle(htEko2PebAy, tertipStrEko2PebAy, ayDeger);
                            HTDecimalEkle(htEko2peb, tertipStrEko2Peb, ayDeger);
                            HTDecimalEkle(htEko2Ay, tertipStrEko2Ay, ayDeger);
                            HTDecimalEkle(htKur, tertipKur, ayDeger);
                            HTDecimalEkle(htPeb, tertipPeb, ayDeger);

                            double ayOdenek = form.satirlar[i].odenek[ay];
                            if (ters) ayOdenek = ayOdenek * -1;
                            if (ayOdenek < 0)
                            {
                                try
                                {
                                    string sql = servisUZY.UzayKosulStr(kullanan, "MPGMASRAFPLANIBIRIM", TNS.UZY.Arac.UzayKosulOl(form.yil.ToString(), "", "", "", tertipStr, (ay + 1).ToString(), "", ""), false, "");
                                    double sonucDbl = ve.TekKolonSQLDbl(sql);
                                    decimal sonuc = Convert.ToDecimal(sonucDbl);
                                    if (sonuc < 0)
                                    {
                                        HataStr += "<br />Yapılan düşme işlemi sonucunda " + tertipStr + " tertibinde, " + (ay + 1).ToString() + ". ay değerinin toplamı, " + sonuc.ToString("#,###") + " oldu.";
                                    }
                                    if (!string.IsNullOrWhiteSpace(ve.hata))
                                        HataStr += "<br />" + ve.hata;

                                }
                                catch (Exception ex)
                                {
                                    HataStr += "<br />" + ex.Message;
                                }
                            }
                        }
                    }
                    if (tertipSay == 0)
                    {
                        if (toplamTertipSay != form.satirlar.Length)
                            s.DegerAta("Tüm tertipler kontrol edilemedi!");
                        return s;
                    }
                    s.DegerAta(HataStr);

                    if (form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniRevize || form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniAktarma || form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniAHP)
                    {
                        RevizeFormKontrolMPRevize(ve, kullanan, form, htEko2PebAy, htEko2peb, htEko2Ay, htPeb, s, projeBilgiler);
                    }
                    else if (form.revizeFormTip == ENUMRevizeFormTip.AHPRevize)
                    {
                        RevizeFormKontrolAHPRevize(ve, kullanan, form, htEko2peb, s);
                    }
                    else if (form.revizeFormTip == ENUMRevizeFormTip.Aktarma || form.revizeFormTip == ENUMRevizeFormTip.KurumAktarma)
                    {
                        RevizeFormKontrolAktarma(ve, kullanan, form, htKur, htKur, htPeb, s, projeBilgiler);
                    }
                    else if (form.revizeFormTip == ENUMRevizeFormTip.Ekleme)
                    {
                        RevizeFormKontrolEkleme(ve, kullanan, form, htKur, htKur, s);
                    }
                    else
                        return new Sonuc(false, "Kontrol metodu yazılmamış", "");
                }
            }
            catch (Exception ex)
            {
                s.DegerAta(ex.Message);
            }
            return s;
        }

        public static Sonuc MPRevizeFormKaydet(VeriErisim veriErisim, Kullanici kullanan, MPRevizeForm form, bool dortDuzeyDegisimUret, bool eklDus)
        {
            Sonuc s = new Sonuc(true, "", "");
            ENUMOdenekKontrolTur okTur = TNS.MUH.Arac.OdenekKontrolTurBul(form.yil);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            if (MasrafPlani.OnayDurumBul(ve, form.yil, "", ENUMMasrafPlanAsama.AHP, "000") != 5)
            {
                ve.VeriErisimKapat();
                s.bilgiStr = "Masraf planı onaylı olmadığından, revize işlemi yapılamaz!";
                s.islemSonuc = true;
                return s;
            }

            if (form.revizeFormTip == ENUMRevizeFormTip.Hepsi)
            {
                s.DegerAta("Hatalı form tipi, kayıt yapılamadı!");
            }

            if (form.satirlar.Length == 0)
            {
                s.DegerAta("Kaydedilecek satır bulunamadığından, kayıt yapılamadı!");
            }

            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            bool yikobMu = false;
            string eskiKurKod = "";
            string kyKod = "";
            VeriErisim veBUM = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", kullanan.KimlikBul());
            List<string> kyKodlar = new List<string>();
            for (int i = 0; i < form.satirlar.Length; i++)
            {
                if (eskiKurKod != form.satirlar[i].tertip.kodlar[0])
                {
                    eskiKurKod = form.satirlar[i].tertip.kodlar[0];
                    if (!YazmaYetkisiVarMiForm(kullanan, form.yil, eskiKurKod))
                    {
                        s.DegerAta("Forma Kaydetme yetkiniz olmadığından kayıt yapılamadı!");
                        break;
                    }
                }

                kyKod = form.satirlar[i].tertip.kodlar[5];

                if (!kyKodlar.Contains(kyKod))
                {
                    kyKodlar.Add(kyKod);
                    if (!YazmaYetkisiVarMiFormKY(kullanan, form.yil, kyKod))
                    {
                        s.DegerAta("Forma Kaydetme yetkiniz olmadığından kayıt yapılamadı!");
                        break;
                    }
                    if (kyKod != "9" && kyKod.Replace(".", "").Length != 11)
                    {
                        s.DegerAta("Verilen kurumsal yapı kodu " + kyKod + " son düzeyde olmalı");
                    }
                    else if (form.satirlar[i].Eklenen > 0)
                    {
                        KurumsalYapi kY = new KurumsalYapi();
                        kY.kod = kyKod;
                        KurumsalYapi[] okunanlar = BHMv2KurumsalYapi.Listele(veBUM, kullanan, kY, false, true, false);
                        if (okunanlar == null || okunanlar.Length != 1)
                            s.DegerAta("Verilen kurumsal yapı kodu " + kyKod + " bulunamadı!");
                        kY = okunanlar[0];
                        if (muhServis.BirimKapaliMi(kullanan, kY.saymanlikKod, kY.birimKod, kY.kurKod))
                            s.DegerAta($"Verilen kurumsal yapı kodu {kyKod} - {kY.saymanlikKod}-{kY.birimKod}-{kY.kurKod} kapalı olduğundan ödenek eklemesi yapılamaz!");
                    }
                }
                if (form.satirlar[i].tertip.kodlar[2].Replace(".", "") == "042134")
                {
                    yikobMu = true;
                    if (string.IsNullOrEmpty(kyKod) || !(kyKod.StartsWith("2") || kyKod.StartsWith("3")))
                    {
                        s.DegerAta("YIKOB Tertipleri için, taşra birimi girilmelidir!");
                        break;
                    }
                }
            }

            veBUM.VeriErisimKapat();
            if (string.IsNullOrWhiteSpace(form.odGonNo))
            {
                if (form.projelerArasi || form.sektorlerArasi || form.bolgelerArasi || yikobMu)
                {
                    if (string.IsNullOrWhiteSpace(form.onayYaziNo) || form.onayYaziTarih == null || form.onayYaziTarih.isNull)
                        s.DegerAta("Projeler arası, sekörler arası, bölgeler arası işlemlerde veya YIKOB işlemlerinde onay yazı ve tarih alanları dolu olmalıdır!");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(form.onayYaziNo) || (form.onayYaziTarih != null && !form.onayYaziTarih.isNull))
                        s.DegerAta("Projeler arası, sekörler arası, bölgeler arası veya YIKOB olmayan işlemlerde onay yazı ve tarih alanları boş olmalıdır!");
                }
            }
            if (yikobMu)
            {
                if (string.IsNullOrWhiteSpace(form.aciklama))
                    s.DegerAta("Lütfen açıklama alanına, ödeneğin kullanım amacını belirtiniz!");
                if (string.IsNullOrWhiteSpace(form.bumkoIslemNo))
                    s.DegerAta("Lütfen BÜMKO işlem numarasını belirtiniz!");
                if (!(form.altIslemKod == "02.02.05" || form.altIslemKod == "02.01.13"))
                    s.DegerAta("Lütfen YIKOB ile ilgili alt işlemlerden bir tanesini seçiniz!");
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);
            MPRevizeForm eskiForm = null;
            if (form.revizeNo != 0)
            {
                try
                {
                    eskiForm = MPRevizeFormOku(ve, kullanan, form.yil, form.revizeNo);
                    if (eskiForm == null)
                    {
                        ve.VeriErisimKapat();
                        s.DegerAta("Eski form bulunamadı!");
                        return s;
                    }
                }
                catch (Exception ex)
                {
                    ve.VeriErisimKapat();
                    s.DegerAta("Eski form okunamadı:" + ex.Message);
                    return s;
                }
                if (form.formDurum != ENUMRevizeFormDurum.Onayli)
                {
                    if (!eskiForm.DegistirilebilirMi())
                    {
                        ve.VeriErisimKapat();
                        s.DegerAta("Onaylanmış yada İptal edilmiş formları değiştiremezsiniz!");
                        return s;
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(form.odGonNo))
            {
                eskiForm = MPRevizeFormOku(ve, kullanan, 0, 0, form.odGonYil, form.odGonTur, form.odGonNo);
            }
            SatirGrupla(form, s);
            if (s.islemSonuc)
                TekrarliSatirVarMi(form.satirlar, s);
            if (s.islemSonuc && form.yil > 2024)
                IskodlariGecerliMi(ve, kullanan, form.satirlar, s);
            bool eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);

            TNS.YAT.IYATServis servisYat = TNS.YAT.Arac.Tanimla();
            Hashtable afHash = new Hashtable();
            for (int i = 0; i < form.satirlar.Length && s.islemSonuc; i++)
            {
                if (string.IsNullOrEmpty(form.satirlar[i].tertip.kodlar[5]) || form.satirlar[i].tertip.kodlar[5].StartsWith("9"))
                {
                    form.satirlar[i].tertip.kodlar[5] = "90000000000";
                }
                if (!TNS.MUH.Arac.TertipTamMi(form.satirlar[i].tertip, okTur, TNS.MUH.ENUMBelgeTuru.ODENEKGONDERME, "", eko23))
                    s.DegerAta("Hatalı Tertip:" + form.satirlar[i].tertip.TertipStrOl());
                form.satirlar[i].tertip.AdlariBul(servisUZY);
                string kodAd;
                string kod;
                if (!form.satirlar[i].tertip.IsimlerTamMi(out kodAd, out kod))
                    s.DegerAta("Hatalı Tertip, tanımsız kısım: (" + kodAd + ", " + kod + ")");
                if (form.satirlar[i].SifirdanBuyukVarMi())
                {
                    kyKod = form.satirlar[i].tertip.kodlar[5];
                    //yatırımda tanımlı olmayan illere ödenek konulamasın
                    YAT.AltFaalBilgi afb = MasrafPlaniTavan.AltFaaliyetBilgiAl(afHash, form.satirlar[i].tertip.kodlar[4], true, servisYat, kullanan, form.yil, "0600");
                    if (!MasrafPlaniTavan.IlGecerliMi(kyKod, form.satirlar[i].tertip.kodlar[4], afHash))
                    {
                        throw new Exception($"Girdiğiniz birim kodunun ({kyKod}) ait olduğu il, ilgili alt faaliyet kodunun ({form.satirlar[i].tertip.kodlar[4]}) yatırım projelerinde girilmiş olan illerden biri olmadığından, ödenek ayrılamaz!");
                    }
                }
            }
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();
            if (eskiForm != null)
            {
                s = tab.RevizeDetaySil(eskiForm.yil, eskiForm.revizeNo);
                if (s.islemSonuc)
                    s = tab.RevizeFormSil(eskiForm.yil, eskiForm.revizeNo);
                form.yil = eskiForm.yil;
                form.revizeNo = eskiForm.revizeNo;
            }
            else
            {
                form.revizeNo = tab.RevizeFormSiraNoBul(form.yil);
                if (form.revizeNo <= 0)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    s.DegerAta("Sıra numarası alınamadı!");
                    return s;
                }
            }
            s = tab.RevizeFormEkle(form, kullanan.kullaniciKodu);
            for (int i = 0; i < form.satirlar.Length && s.islemSonuc; i++)
            {
                for (int ay = 0; ay < 12 && s.islemSonuc; ay++)
                {
                    s = tab.EkleTertip(ENUMMasrafPlanAsama.AHP, form.satirlar[i].tertip, ay + 1, form.satirlar[i].odenek[ay], form.revizeNo, (int)form.formDurum, form.satirlar[i].grup, form.satirlar[i].IOIISKOD, i);
                }
            }
            if (s.islemSonuc)
                s = RevizeFormKontrolGenel(ve, kullanan, form, false);

            if (s.islemSonuc)
            {
                s.anahtar = form.revizeNo.ToString();
                ve.Commit();
                if (form.formDurum == ENUMRevizeFormDurum.Onayli)
                {
                    if (dortDuzeyDegisimUret)
                    {
                        Sonuc degisim = MPRevize4DuzeyDegisimYap(ve, kullanan, form);
                        if (degisim != null)
                        {
                            s.bilgiStr = degisim.bilgiStr;
                        }
                    }
                }
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        private static void IskodlariGecerliMi(VeriErisim ve, Kullanici kullanan, MPRevizeSatir[] satirlar, Sonuc s)
        {
            //HashSet<List<int>> isKodHT = new HashSet<List<int>>();
            Hashtable isKodHT = new Hashtable();
            List<int> isKodListesi = new List<int>();
            for (int i = 0; i < satirlar.Length; i++)
            {
                int iFin = OrtakFonksiyonlar.ConvertToInt(satirlar[i].tertip.kodlar[2], 0);
                string birim = satirlar[i].tertip.kodlar[5];
                int isKod = satirlar[i].IOIISKOD;
                if (iFin == 9 || iFin == 16)
                {
                    if (birim.StartsWith("9"))
                    {
                        s.DegerAta($"{i + 1}. satırdaki iş kodu girilmiş fakat merkez-merkez birimine iş kodu girmeyiniz!");
                    }
                    else
                    {
                        List<int> satirListesi;

                        try
                        {
                            satirListesi = (List<int>)isKodHT[isKod];
                        }
                        catch
                        {
                            satirListesi = null;
                        }
                        if (satirListesi == null)
                            satirListesi = new List<int>();
                        if (satirListesi.Count == 0)
                            isKodListesi.Add(isKod);
                        satirListesi.Add(i);
                        isKodHT[isKod] = satirListesi;
                    }
                }
                else if (isKod > 0)
                {
                    s.DegerAta($"{i + 1}. satırdaki iş kodu girilmiş fakat finansman tipi girmeye uygun değil!");
                }
            }
            if (s.islemSonuc && isKodListesi.Count > 0)
            {
                ObjectArray oa = BHMv2IOIProjeGiris.Listele(null, kullanan, new IOIProje(), false, isKodListesi, -1);
                if (oa.sonuc.islemSonuc)
                {
                    foreach (DictionaryEntry de in isKodHT)
                    {
                        int isKod = (int)de.Key;
                        IOIProje ioi = IsBul(oa.objeler, isKod);
                        if (ioi != null)
                        {
                            List<int> satirNolar = (List<int>)de.Value;
                            foreach (int satirNo in satirNolar)
                            {
                                MPRevizeSatir revizeSat = satirlar[satirNo];
                                string peb = revizeSat.tertip.kodlar[4];
                                string birim = revizeSat.tertip.kodlar[5];
                                int birimMuh;
                                string birimKod;
                                IlkodBul(birim, out birimMuh, out birimKod);
                                if (ioi.muhasebeKod != birimMuh || ioi.birimKod != birimKod)
                                {
                                    s.DegerAta($"{satirNo + 1} satırda, işin birimi {ioi.muhasebeKod} satırda, işin birimi  bir birime{birim} ödenek planlıyorsunuz!");
                                }
                                //if (!PEBKodVarmi(peb, ioi.pebKodlar))
                                //{
                                //    s.DegerAta($"{satirNo + 1} satırda, tertibin peb kodu {peb}, iş ile bağlantılı alt proje ({ioi.altProjeAdi}) ile ilişkilendirilmediğinden kayıt yapamazsınız!");
                                //}
                            }
                        }
                        else
                            s.DegerAta($"{isKod} sıra numaralı iş bulunamadı!");
                    }
                }
                else
                    s.DegerAta(oa.sonuc.hataStr);
            }
        }

        private static bool PEBKodVarmi(string peb, TNSCollection pebKodlar)
        {
            if (pebKodlar == null) return false;
            peb = peb.Replace(".", "");
            foreach (IOIProjeBag item in pebKodlar)
            {
                if (item.pebKod.Replace(".", "") == peb)
                    return true;
            }
            return false;
        }

        private static int IlkodBul(string birim)
        {
            if (string.IsNullOrEmpty(birim)) return 0;
            birim = birim.Replace(".", "");
            if (birim.Length >= 3)
            {
                return OrtakFonksiyonlar.ConvertToInt(birim.Substring(1, 2), 0);
            }
            return 0;
        }

        private static void IlkodBul(string birim, out int birimMuh, out string birimKod)
        {
            birimMuh = -1;
            birimKod = "";
            if (string.IsNullOrEmpty(birim)) return;
            birim = birim.Replace(".", "");
            if (birim.Length >= 10)
            {
                birimMuh = OrtakFonksiyonlar.ConvertToInt(birim.Substring(1, 5), -1);
                birimKod = birim.Substring(7, 3);
            }
        }

        private static IOIProje IsBul(TNSCollection objeler, int isKod)
        {
            foreach (IOIProje item in objeler)
            {
                if (item.siraNo == isKod)
                    return item;
            }
            return null;
        }

        private static void SatirGrupla(MPRevizeForm form, Sonuc s)
        {
            //if (form.revizeFormTip == ENUMRevizeFormTip.Aktarma || form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniAktarma)
            //{
            //    int grup = 1;
            //    double toplam = 0;
            //    int dusenSay = 0;
            //    int eklenenSay = 0;
            //    for (int i = 0; i < form.satirlar.Length; i++)
            //    {
            //        double satToplam = form.satirlar[i].NetToplam();
            //        if (satToplam < 0)
            //            dusenSay++;
            //        else
            //            eklenenSay++;

            //        toplam += satToplam;
            //        //if (toplam > 0)
            //        //{
            //        //    s.DegerAta("Aktarma işlemlerinde önce düşülen tertibi/tertipleri sonra eklenecek tertibi/tertipleri yazınız.");
            //        //    return;
            //        //}
            //        form.satirlar[i].grup = grup;

            //        if (Math.Round(toplam, 3) == 0)
            //        {
            //            if (dusenSay > 1 && eklenenSay > 1)
            //            {
            //                s.DegerAta("Aynı tertip grubunda birden fazla düşülen ve eklenen tertip olamaz!");
            //                return;
            //            }
            //            grup++;
            //            toplam = 0;
            //            dusenSay = 0;
            //            eklenenSay = 0;
            //        }
            //    }
            //    if (Math.Round(toplam, 3) != 0)
            //    {
            //        if (dusenSay > 1 && eklenenSay > 1)
            //        {
            //            s.DegerAta("Aynı tertip grubunda birden fazla düşülen ve eklenen tertip olamaz!");
            //            return;
            //        }
            //        grup++;
            //        toplam = 0;
            //    }
            //}
            //else
            {
                for (int i = 0; i < form.satirlar.Length - 1; i++)
                {
                    form.satirlar[i].grup = 1;
                }
            }
        }

        internal static Sonuc MPRevizeFormDurumAta(VeriErisim veriErisim, Kullanici kullanan, TNSCollection formlar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            Sonuc s = new Sonuc();

            foreach (MPRevizeForm form in formlar)
            {
                if (string.IsNullOrWhiteSpace(form.odGonNo)) continue;
                MPRevizeForm eskiForm = null;
                try
                {
                    ve.hata = "";
                    eskiForm = MPRevizeFormOku(ve, kullanan, 0, 0, form.odGonYil, form.odGonTur, form.odGonNo);
                    if (eskiForm != null)
                    {
                        TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);

                        ve.BeginTransaction();
                        s = tab.RevizeDetayOnayla(eskiForm.yil, eskiForm.revizeNo, form.formDurum);
                        if (s.islemSonuc)
                            s = tab.RevizeFormOnayla(eskiForm.yil, eskiForm.revizeNo, kullanan.kullaniciKodu, form.formDurum);
                        if (s.islemSonuc)
                        {
                            ve.Commit();
                        }
                        else
                        {
                            ve.Rollback();
                            OrtakFonksiyonlar.HataDosyaYaz("MPRevizeDurum.txt", $"Verilen ödenek gönderme/tenkis ({form.odGonYil}/{form.odGonTur}/{form.odGonNo}) belgesi ile ilgili MP Revize işlemi yazılamadı!", false);
                        }
                    }
                    else
                        OrtakFonksiyonlar.HataDosyaYaz("MPRevizeDurum.txt", $"Verilen ödenek gönderme/tenkis ({form.odGonYil}/{form.odGonTur}/{form.odGonNo}) belgesi ile ilgili MP Revize işlemi bulunamadı!", false);
                }
                catch (Exception ex)
                {
                    OrtakFonksiyonlar.HataDosyaYaz("MPRevizeDurum.txt", $"Verilen ödenek gönderme/tenkis ({form.odGonYil}/{form.odGonTur}/{form.odGonNo}) belgesi ile ilgili Hata oldu:{ex.Message}!", false);
                }
            }

            ve.VeriErisimKapat();
            return s;
        }

        public static Sonuc MPRevizeFormDurumAta(VeriErisim veriErisim, Kullanici kullanan, int yil, int revizeNo, ENUMRevizeFormDurum formDurum)
        {
            Sonuc s = new Sonuc();
            if (revizeNo == 0)
            {
                s.DegerAta("sıra numarası 0 olan olan bir form onaylanamaz");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());

            MPRevizeForm eskiForm = null;
            try
            {
                eskiForm = MPRevizeFormOku(ve, kullanan, yil, revizeNo);
            }
            catch (Exception ex)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Form okunamadı:" + ex.Message);
                return s;
            }
            if (eskiForm == null)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Form okunamadı. Form No:" + revizeNo.ToString());
                return s;
            }
            if (!eskiForm.DegistirilebilirMi(kullanan, formDurum))
            {
                ve.VeriErisimKapat();
                s.DegerAta("Form zaten onaylı olduğu için durumu değiştirilemez!");
                return s;
            }
            if (eskiForm.satirlar.Length <= 0)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Form satırları olmadığından durumu değiştirilemez!");
                return s;
            }

            if (!OnaylamaYetkisiVarMiForm(kullanan, yil, eskiForm.satirlar[0].tertip.kodlar[0], eskiForm.revizeFormTip))
            {
                s.DegerAta("Formu onaylama yetkiniz bulunmamaktadır!");
                return s;
            }

            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);

            ve.BeginTransaction();
            s = tab.RevizeDetayOnayla(yil, revizeNo, formDurum);
            if (s.islemSonuc)
                s = tab.RevizeFormOnayla(yil, revizeNo, kullanan.kullaniciKodu, formDurum);
            bool ters;
            if (KontrolYapilsinMi(eskiForm.formDurum, formDurum, out ters))
            {
                eskiForm.formDurum = formDurum;
                s = RevizeFormKontrolGenel(ve, kullanan, eskiForm, ters);
            }
            if (s.islemSonuc)
            {
                ve.Commit();
                if (formDurum == ENUMRevizeFormDurum.Onayli)
                {
                    Sonuc degisim = MPRevize4DuzeyDegisimYap(ve, kullanan, eskiForm);
                    if (degisim != null)
                    {
                        s.bilgiStr = degisim.bilgiStr;
                    }
                }
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return s;
        }

        private static bool KontrolYapilsinMi(ENUMRevizeFormDurum eskiFormDurum, ENUMRevizeFormDurum yeniFormDurum, out bool ters)
        {
            ters = false;
            if (eskiFormDurum == ENUMRevizeFormDurum.Onayli) ters = true;
            else if (yeniFormDurum == ENUMRevizeFormDurum.Iptal) ters = true;
            //else if (yeniFormDurum != ENUMRevizeFormDurum.Onayli)
            //{
            //}

            //if (yeniFormDurum == ENUMRevizeFormDurum.Iptal || yeniFormDurum == ENUMRevizeFormDurum.Musvette) return false;
            //if (yeniFormDurum == ENUMRevizeFormDurum.Taslak && eskiFormDurum == ENUMRevizeFormDurum.Onayli) return false;
            return true;
        }

        public static string FormKosulOl(MPRevizeFormSorgu form, string tabloAd, bool detayaBak, string JoinToDisTabloAd)
        {
            return TABMPGMASRAFPLANI.FormKosulOl(form, tabloAd, detayaBak, JoinToDisTabloAd);
        }

        public static MPRevizeForm[] MPRevizeFormListele(VeriErisim veriErisim, Kullanici kullanan, MPRevizeFormSorgu form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);
            TABMPGMASRAFPLANI tab2 = new TABMPGMASRAFPLANI(ve2);

            ArrayList al = new ArrayList();
            IDataReader dr = tab.RevizeFormListele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    MPRevizeForm f = new MPRevizeForm();
                    f.yil = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.YIL, 0);
                    f.revizeNo = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.REVIZENO, 0);
                    f.aciklama = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ACIKLAMA, "");
                    f.formDurum = (ENUMRevizeFormDurum)VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.FORMDURUM, 0);

                    f.bumkoIslemNo = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.BUMKOISLEMNO, "");
                    f.bumkoKurumKod = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.BUMKOKURUMKOD, "");

                    f.kaydedenKullaniciKodu = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.KAYDEDEN, "");
                    f.evrakTarih = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.EVRAKTARIH);
                    f.kayitTarihi = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.KAYITTARIH);
                    f.onaylayanKullaniciKodu = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ONAYLAYAN, "");
                    f.onaylamaTarihi = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ONAYTARIH);
                    f.revizeFormTip = (ENUMRevizeFormTip)VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.BELGETUR, 0);
                    f.odenekToplam = tab2.RevizeFormToplamBul(f.yil, f.revizeNo);
                    f.altIslemKod = VeriErisim.DegerAl(dr, (int)TABMPGMASRAFPLANI.ENUMAlanlarREVIZEFORM.ALTISLEMKOD, "");
                    f.altIslemKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", f.altIslemKod.Replace(".", ""));
                    al.Add(f);
                }

                dr.Close();
                ve.VeriErisimKapat();
                return (MPRevizeForm[])al.ToArray(typeof(MPRevizeForm));
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
        }

        public static Sonuc MPRevize4DuzeyDegisimYap(VeriErisim veriErisim, Kullanici kullanan, MPRevizeForm form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (form != null)
                s.bilgiStr += string.Format("{0} numaraları MP revize form işleme alınacak<br>", form.revizeNo.ToString());
            else
                s.bilgiStr += string.Format("Boş form  geldi<br>");
            try
            {
                TNS.MUH.GondermeTenkisForm[] formlar = GondermeTenkis4DuzeyUret(veriErisim, kullanan, form);
                if (formlar != null && formlar.Length > 0)
                {
                    s.bilgiStr += string.Format("{0} adet 4 düzey değişim formu elde edildi!<br>", formlar.Length);
                    IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
                    for (int i = 0; i < formlar.Length; i++)
                    {
                        Sonuc sKaydet = muhServis.GondermeTenkisKaydet(kullanan, formlar[i], true, false, false);
                        if (sKaydet.islemSonuc)
                        {
                            s.bilgiStr += string.Format("{0}. 4 düzey değişim formu {1} numarası ile kaydedildi!<br>", (i + 1).ToString(), sKaydet.anahtar);
                            formlar[i].belgeNo = sKaydet.anahtar;
                            formlar[i].formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                            TNSCollection f = new TNSCollection();
                            f.Add(formlar[i]);
                            Sonuc sDurum = muhServis.BelgeDurumDegistirGonTen(kullanan, f);
                            if (sDurum.islemSonuc)
                                s.bilgiStr += string.Format("{0}. 4 düzey değişim formu onaylandı!<br>", (i + 1).ToString());
                            else
                                s.bilgiStr += string.Format("{0}. 4 düzey değişim formu ONAYLANAMADI!{1}<br>", (i + 1).ToString(), sDurum.hataStr);
                        }
                        else
                            s.bilgiStr += string.Format("{0}. 4 düzey değişim formu KAYDEDİLEMEDİ!{1}<br>", (i + 1).ToString(), sKaydet.hataStr);
                    }
                }
                else
                    s.DegerAta("İlgili formun türü veya onay durumu uyumsuz olduğundan veya gerekli değişim gerekmediğinden işlem yapılamadı!");
            }
            catch (Exception ex)
            {
                s.DegerAta(ex.Message);
            }
            return s;
        }

        public static Sonuc MPRevize4DuzeyDegisimYap(VeriErisim veriErisim, Kullanici kullanan, int yil, int revizeNo)
        {
            try
            {
                MPRevizeForm form = MPRevizeFormOku(veriErisim, kullanan, yil, revizeNo);
                if (form != null)
                    return MPRevize4DuzeyDegisimYap(veriErisim, kullanan, form);
                else
                    throw new Exception(string.Format("{0} numaralı revize formu bulunamadı!", revizeNo));
            }
            catch (Exception ex)
            {
                return new Sonuc(false, "Form okunamadı!" + ex.Message, "");
            }
        }

        private static MUH.GondermeTenkisForm[] GondermeTenkis4DuzeyUret(VeriErisim veriErisim, Kullanici kullanan, MPRevizeForm form)
        {
            if (form == null)
                throw new Exception("Form bilgisi boş geldiği için işlem yapılmadı!");
            if (form.revizeFormTip != ENUMRevizeFormTip.MasrafPlaniRevize)
                return null;
            if (form.formDurum != ENUMRevizeFormDurum.Onayli)
                return null;
            int serbestAy = TNS.BHMv2.Arac.SerbestAyBul(form.yil);
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            System.Collections.Hashtable IkiDuzey = new Hashtable();
            string tut = "";

            int eko2Uz = TNS.BHMv2.Arac.Eko2UzBul(form.yil);

            foreach (MPRevizeSatir satir in form.satirlar)
            {
                Tertip t = new Tertip(satir.tertip.yil, satir.tertip.tabloKod);
                t.TertipAta(satir.tertip);
                t.kodlar[3] = satir.tertip.kodlar[3].Replace(".", "").Substring(0, eko2Uz);
                t.kodlar[4] = "";
                tut += t.TertipStrOl() + ";";
                double[] degerler = null;
                try
                {
                    degerler = (double[])IkiDuzey[t.TertipStrOl()];
                }
                catch
                {

                }
                if (degerler == null)
                {
                    degerler = new double[12];
                }
                for (int i = 0; i < 12; i++)
                {
                    degerler[i] += satir.odenek[i];
                }
                IkiDuzey[t.TertipStrOl()] = degerler;
            }

            ArrayList al = new ArrayList();
            foreach (DictionaryEntry de in IkiDuzey)
            {
                string tertipStr = (string)de.Key;
                double[] toplam = (double[])de.Value;
                bool hepsiSifir = true;
                for (int i = 0; i < toplam.Length && hepsiSifir; i++)
                {
                    if (toplam[i] != 0) hepsiSifir = false;
                }
                if (hepsiSifir)
                {
                    Tertip t = new Tertip(form.yil, ENUMTabloKod.TABMasrafPlani);
                    t.TertipAta(tertipStr);
                    KurumsalYapi ky = TNS.BHMv2.IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(null, t.kodlar[5]);
                    GondermeTenkisForm gtf = ALdeGondermeTenkisForm(al, ky);
                    if (gtf == null)
                    {
                        gtf = new GondermeTenkisForm();
                        gtf.DortDuzeyDegisim = 1;
                        gtf.yil = form.yil;
                        gtf.muhasebe = OrtakFonksiyonlar.ConvertToInt(ky.saymanlikKod, 0);
                        gtf.birim = ky.birimKod;
                        gtf.belgeTur = ENUMBelgeTuru.ODENEKGONDERME;
                        gtf.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                        gtf.islemTarih = form.kayitTarihi;
                        if (gtf.islemTarih == null || gtf.islemTarih.isNull)
                            gtf.islemTarih = new TNSDateTime(DateTime.Now);
                        gtf.kurum = TNS.DEG.Arac.VarsayilanKurumBul();
                        gtf.islemYapan = kullanan.kullaniciKodu;
                        gtf.yaziTarih = form.evrakTarih;
                        gtf.icmalNo = 0;
                        al.Add(gtf);
                    }
                    DortDuzeyFormaSatirlariEkle(veriErisim, kullanan, uzyServis, form, gtf, t, serbestAy);
                }
            }
            for (int i = al.Count - 1; i >= 0; i--)
            {
                GondermeTenkisForm gtf = (GondermeTenkisForm)al[i];
                if (gtf.detay.objeler.Count == 0)
                {
                    al.RemoveAt(i);
                }
            }
            return (GondermeTenkisForm[])al.ToArray(typeof(GondermeTenkisForm));
        }

        private static void DortDuzeyFormaSatirlariEkle(VeriErisim veriErisim, Kullanici kullanan, IUZYServis uzyServis, MPRevizeForm form, GondermeTenkisForm gtf, Tertip t, int serbestAy)
        {
            ArrayList al = new ArrayList();
            double fark = 0;
            foreach (MPRevizeSatir satir in form.satirlar)
            {
                if (satir.tertip.Kiyasla(t))
                {
                    double toplam = 0;
                    for (int i = 0; i < serbestAy; i++)
                    {
                        toplam += satir.odenek[i];
                    }

                    toplam = EkleDusMiktarBul(veriErisim, kullanan, uzyServis, gtf, satir.tertip, toplam, serbestAy);

                    satir.tertip.kodlar[5] = "";
                    GondermeTenkisDetay detay = new GondermeTenkisDetay(form.yil);
                    detay.tertip.TertipAta(satir.tertip.TertipStrOl());
                    detay.miktar = toplam;
                    fark += detay.miktar;
                    al.Add(detay);
                }
            }
            foreach (GondermeTenkisDetay detay in al)
            {
                if (fark == 0) break;
                if (fark < 0 && detay.miktar < 0)
                {
                    if (detay.miktar > fark)
                    {
                        fark = fark - detay.miktar;
                        detay.miktar = 0;
                    }
                    else
                    {
                        detay.miktar = detay.miktar - fark;
                        fark = 0;
                    }

                }
                else if (fark > 0 && detay.miktar > 0)
                {
                    if (detay.miktar < fark)
                    {
                        fark = fark - detay.miktar;
                        detay.miktar = 0;
                    }
                    else
                    {
                        detay.miktar = detay.miktar - fark;
                        fark = 0;
                    }
                }
            }

            foreach (GondermeTenkisDetay detay in al)
            {
                if (detay.miktar != 0)
                    gtf.detay.objeler.Add(detay);
            }
        }

        private static double EkleDusMiktarBul(VeriErisim veriErisim, Kullanici kullanan, IUZYServis uzyServis, GondermeTenkisForm gtf, Tertip tertip, double miktar, int serbestAy)
        {
            double[] mpBilgi = MasrafPlani.MasrafPlaniOdenekBilgiBul(veriErisim, tertip, false, 0);
            string uzayKosul = gtf.yil.ToString() + "----" + gtf.muhasebe + "-" + gtf.birim + "--0/6-" + tertip.kodlar[0] + "-" + tertip.kodlar[1] + "-" + tertip.kodlar[2] + "-" + tertip.kodlar[3] + "-" + tertip.kodlar[4];
            double gond = uzyServis.UzayDegeriDbl(kullanan, "MUHGONDERMEBELGESITENKIS", uzayKosul, false);
            if (miktar < 0)
            {
                double dusulen = 0 - miktar;
                double kalan = gond - mpBilgi[2];
                if (kalan <= 0)
                {
                    miktar = 0;
                }
                else if (kalan < dusulen)
                {
                    miktar = 0 - kalan;
                }
            }
            else
            {
                if (gond < mpBilgi[1])
                {
                    double eklenen = mpBilgi[1] - gond;
                    miktar = Math.Min(eklenen, miktar);
                }
                else
                    miktar = 0;
            }
            return miktar;
        }

        private static GondermeTenkisForm ALdeGondermeTenkisForm(ArrayList al, KurumsalYapi ky)
        {
            foreach (GondermeTenkisForm form in al)
            {
                if (form.muhasebe == OrtakFonksiyonlar.ConvertToInt(ky.saymanlikKod, 0) && form.birim == ky.birimKod)
                    return form;
            }
            return null;
        }

        internal static PEBProjeBilgi PEBBilgiBul(VeriErisim veriErisim, Kullanici kullanan, int yil, string peb)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMPGMASRAFPLANI tabMPGMasrafPlani = new TABMPGMASRAFPLANI(ve);
            Hashtable projeBilgiler = new Hashtable();
            PEBProjeBilgi pb = ProjeBilgiBul(tabMPGMasrafPlani, yil, peb, projeBilgiler, false);
            ve.VeriErisimKapat();
            return pb;
        }

        internal static Sonuc ButceIslemUret(VeriErisim veriErisim, Kullanici kullanan, int yil, MPRevizeSatir islemSatir, int yontem)
        {
            Sonuc s = new Sonuc();
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            Tertip kistas = islemSatir.tertip;
            //string dosyaAd = "A" + kistas.TertipKodu + ".txt";

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(5), kistas.KodNoKirilimSay(5)));

            EkDegerParam[] ekBilgiler = new EkDegerParam[13];

            string asama = "9";
            for (int ay = 0; ay < 12; ay++)
            {
                ekBilgiler[ay] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, 0, 0, asama + ";" + (ay + 1).ToString());
            }
            ekBilgiler[12] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "5");

            OdenekBilgi[] obler = BHMv2.IsKurallari.UcgenServisi.ListeTertipOl(null, kullanan, kistas, kodBilgileri, ekBilgiler, true, false, false, -1);
            if (yontem == 2)
            {
                double tplm = 0;
                foreach (double item in islemSatir.odenek)
                {
                    tplm = OrtakFonksiyonlar.KurusYap(tplm + item);
                }
                if (tplm != 0)
                {
                    s.DegerAta($"{islemSatir.tertip.TertipStrOl()} tertibi için verilen rakamların toplamı sıfır değil!");
                    return s;
                }
            }

            OdenekGondermeleriDus(s, obler, yontem);//, dosyaAd
            if (!s.islemSonuc)
                return s;

            //OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, "Liste", true);
            //foreach (OdenekBilgi item in obler)
            //{
            //    string bilgiStr = $"{item.tertip.TertipStrOl()}";
            //    for (int i = 0; i < item.ekRakamlar.Count; i++)
            //    {
            //        bilgiStr += $"\t{(double)item.ekRakamlar[i]}";
            //    }
            //    OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, bilgiStr, false);
            //}
            for (int ayNo = 0; ayNo < 12; ayNo++)
            {
                double kalan = BirimlerdenAlAy(islemSatir.odenek[ayNo], ayNo, obler, yontem, s);
                for (int ayIc = 0; kalan < 0 && ayIc < 12; ayIc++)
                {
                    if (islemSatir.odenek[ayIc] < 0)
                        kalan = BirimlerdenAlAy(kalan, ayIc, obler, yontem, s);
                }
            }

            //OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, "Dağıtım sonrası", false);
            //foreach (OdenekBilgi item in obler)
            //{
            //    string bilgiStr = $"{item.tertip.TertipStrOl()}";
            //    for (int i = 0; i < item.odenekler.Count; i++)
            //    {
            //        bilgiStr += $"\t{(double)item.odenekler[i]}";
            //    }
            //    OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, bilgiStr, false);
            //}

            //OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, "Artılar işlenecek", false);
            BirimlereEkleAy(islemSatir, obler, yontem, s); //, dosyaAd
            //foreach (OdenekBilgi item in obler)
            //{
            //    string bilgiStr = $"{item.tertip.TertipStrOl()}";
            //    for (int i = 0; i < item.odenekler.Count; i++)
            //    {
            //        bilgiStr += $"\t{(double)item.odenekler[i]}";
            //    }
            //    OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, bilgiStr, false);
            //}
            if (s.islemSonuc)
            {
                //OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, "İşlem üretilecek", false);
                MPRevizeForm mpForm = new MPRevizeForm();
                mpForm.yil = yil;
                mpForm.evrakTarih = new TNSDateTime(DateTime.Now);
                mpForm.aciklama = $"{islemSatir.tertip.TertipStrOl()} tertibi için {IslemAdBul(yontem)} işlemi";
                mpForm.revizeFormTip = IslemTurBul(yontem);
                mpForm.altIslemKod = AltIslemKodBul(yontem);
                List<MPRevizeSatir> l = new List<MPRevizeSatir>();
                for (int tNo = 0; tNo < obler.Length; tNo++)
                {
                    try
                    {
                        //OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, $"{tNo}/{obler.Length}. satır", false);
                        //OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, $"{tNo}/{obler.Length}. satır {obler[tNo].tertip.kodlar[5]}", false);
                        if (obler[tNo].odenekler.Count == 12)
                        {
                            MPRevizeSatir islemBilgi = new MPRevizeSatir();
                            islemBilgi.tertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
                            islemBilgi.tertip.TertipAta(islemSatir.tertip.TertipStrOl() + "-" + obler[tNo].tertip.kodlar[5]);
                            string bilgiS = islemSatir.tertip.TertipStrOl() + "-" + obler[tNo].tertip.kodlar[5];
                            for (int ayIc = 0; ayIc < 12; ayIc++)
                            {
                                //double dusulecek = OrtakFonksiyonlar.KurusYap(Math.Abs(islemSatir.odenek[ayIc]));
                                //if (dusulecek <= 0) continue;
                                islemBilgi.odenek[ayIc] = (double)obler[tNo].odenekler[ayIc];
                                bilgiS += "\t" + islemBilgi.odenek[ayIc].ToString("#,##0.00");
                            }
                            l.Add(islemBilgi);
                            //OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, bilgiS, false);
                        }
                        else
                        {
                            //OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, $"{obler[tNo].tertip.kodlar[5]} bu birime işlem yapılmayacak!", false);
                        }
                    }
                    catch (Exception ex)
                    {
                        s.DegerAta(ex.Message);
                        //OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, $"{ex.Message}", false);
                    }
                }

                //OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, $"Kayıt yapılacak", false);
                mpForm.satirlar = l.ToArray();
                if (s.islemSonuc)
                    return MPRevizeFormKaydet(veriErisim, kullanan, mpForm, false, false);
                else
                    return s;
            }
            else
                return s;
        }

        private static string AltIslemKodBul(int yontem)
        {
            if (yontem == 1 || yontem == 2)
                return "01.02.01";
            else
                return "02.02.04";
        }

        private static ENUMRevizeFormTip IslemTurBul(int yontem)
        {
            if (yontem == 1 || yontem == 2)
                return ENUMRevizeFormTip.AHPRevize;
            return ENUMRevizeFormTip.KurumAktarma;
        }

        private static string IslemAdBul(int yontem)
        {
            if (yontem == 1) return "ödeneklerin bloke edilmesi";
            if (yontem == 10) return "ödeneklerin yedeğe aktarılması";
            if (yontem == 1) return "masraf planı düzeltmesi";
            return "";
        }

        private static void BirimlereEkleAy(MPRevizeSatir islemSatir, OdenekBilgi[] obler, int yontem, Sonuc s) //, string dosyaAd
        {
            foreach (OdenekBilgi item in obler)
            {
                if (item.odenekler.Count != 12) continue;
                for (int ayOdenek = 0; ayOdenek < 12; ayOdenek++)
                {
                    double eklenecek = -1 * ((double)item.odenekler[ayOdenek]);
                    for (int ayIc = 0; eklenecek > 0 && ayIc < 12; ayIc++)
                    {
                        double dagit = islemSatir.odenek[ayIc];
                        if (dagit > 0)
                        {
                            if (dagit > eklenecek)
                            {
                                item.odenekler[ayIc] = OrtakFonksiyonlar.KurusYap((double)item.odenekler[ayIc] + eklenecek);
                                islemSatir.odenek[ayIc] = OrtakFonksiyonlar.KurusYap(islemSatir.odenek[ayIc] - eklenecek);
                                eklenecek = 0;
                            }
                            else
                            {
                                item.odenekler[ayIc] = OrtakFonksiyonlar.KurusYap((double)item.odenekler[ayIc] + dagit);
                                eklenecek = OrtakFonksiyonlar.KurusYap(eklenecek - dagit);
                                islemSatir.odenek[ayIc] = 0;
                            }
                        }
                    }
                }
            }
        }

        private static double BirimlerdenAlAy(double dusulecek, int ayNo, OdenekBilgi[] okunanOdenekler, int yontem, Sonuc s)
        {
            if (dusulecek >= 0) return 0;
            dusulecek = OrtakFonksiyonlar.KurusYap(dusulecek * -1);
            double toplam = 0;
            for (int tNo = 0; tNo < okunanOdenekler.Length; tNo++)
            {
                for (; okunanOdenekler[tNo].odenekler.Count < 12;)
                {
                    okunanOdenekler[tNo].odenekler.Add((double)0);
                }
                double birimTutar = okunanOdenekler[tNo].EkRakam(ayNo);
                if (birimTutar > 0)
                    toplam += birimTutar;
                else
                    okunanOdenekler[tNo].ekRakamlar[ayNo] = (double)0;
            }
            double kalan = 0;
            if (toplam < dusulecek)
            {
                if (yontem == 1)
                {
                    s.DegerAta($"{ayNo + 1}. ay için düşülmesi gereken miktar ({dusulecek.ToString("#,##0")}), tertibin toplamından ({toplam.ToString("#,##0")}) büyük!");
                    return kalan;
                }
                else
                {
                    kalan = toplam - dusulecek;
                    dusulecek = toplam;
                }
            }
            for (int tNo = 0; tNo < okunanOdenekler.Length; tNo++)
            {
                double birimTutar = okunanOdenekler[tNo].EkRakam(ayNo);
                if (birimTutar <= 0) continue;
                double dusulecekTutar = OrtakFonksiyonlar.YuvarlaAsagi(birimTutar * dusulecek / toplam);
                dusulecek -= dusulecekTutar;
                toplam -= birimTutar;
                okunanOdenekler[tNo].odenekler[ayNo] = OrtakFonksiyonlar.KurusYap((double)okunanOdenekler[tNo].odenekler[ayNo] + dusulecekTutar * -1);
                okunanOdenekler[tNo].ekRakamlar[ayNo] = OrtakFonksiyonlar.KurusYap(birimTutar - dusulecekTutar);
            }
            return toplam;
        }

        private static void OdenekGondermeleriDus(Sonuc s, OdenekBilgi[] obler, int yontem)//, string dosyaAd
        {
            foreach (OdenekBilgi item in obler)
            {
                double dagit = item.EkRakam(12);
                for (int i = 0; i < 12; i++)
                {
                    if (dagit > 0)
                    {
                        double simd = item.EkRakam(i);
                        if (simd < 0)
                        {
                            s.DegerAta($"{item.tertip.TertipStrOl()} tertibinde, {i + 1}. ay değeri negatif geldi, işlem yapılamaz!");
                            return;
                        }
                        else if (simd > dagit)
                        {
                            simd = OrtakFonksiyonlar.KurusYap(simd - dagit);
                            dagit = 0;
                        }
                        else
                        {
                            simd = 0;
                            dagit = OrtakFonksiyonlar.KurusYap(dagit - simd);
                        }
                        item.ekRakamlar[i] = simd;
                    }
                    else
                        break;
                }
                if (dagit > 0)
                    s.DegerAta($"{item.tertip.TertipStrOl()} tertibinde, gönderilen ödenek {item.EkRakam(12)} birimin masraf planından az olduğundan işlem yapılamadı!");
            }
        }
    }
}

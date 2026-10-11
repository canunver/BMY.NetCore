using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;
using TNS.DEG;
using TNS.BKD;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class TMMTesisBilesenleriCetveli
    {
        private static Sonuc KaydetKontroluYap(Kullanici kullanan, TesisBilesenleri tk)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(tk.muhasebe.kod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (string.IsNullOrEmpty(tk.harcamaBirimi.kod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (string.IsNullOrEmpty(tk.ambar.kod))
                hata += TasinirMalResI.TMM031 + "<br>";
            if (string.IsNullOrEmpty(tk.sicilNo))
                hata += TasinirMalResI.TMM020 + "<br>";
            if (string.IsNullOrEmpty(tk.bulunduguYer))
                hata += "Bulunduğu yer boş bırakılamaz" + "<br>";
            if (!tk.sicilNo.Contains(tk.hesapPlanKod.Replace(".", "")))
                hata += "Sicil No ile Hesap Kodu uyuşmuyor" + "<br>";
            else if (!tk.sicilNo.StartsWith("25301"))
                hata += "Seçilen Sicil Numarası Tesis için değildir" + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, tk.muhasebe.kod, tk.harcamaBirimi.kod, tk.ambar.kod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            hata = Genel.KontrolMuhasebeHarcamaAmbar(tk.muhasebe.kod, tk.harcamaBirimi.kod, tk.ambar.kod);
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TesisBilesenleri tk)
        {
            Sonuc sonuc = KaydetKontroluYap(kullanan, tk);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTesisBilesenleriCetveli tabTK = new TABTesisBilesenleriCetveli(ve);
            ve.BeginTransaction();

            try
            {
                if (tk.belgeTarihi == null || tk.belgeTarihi.isNull)
                    tk.belgeTarihi = new TNSDateTime(DateTime.Today);

                tk.durum = (int)ENUMTesisBilesenDurum.KAYITEDILDI;

                if (string.IsNullOrEmpty(tk.refId))
                {
                    tk.refId = Guid.NewGuid().ToString();
                    tk.belgeNo = OrtakFonksiyonlar.ConvertToInt(new TABGenel(ve).BelgeNoAl("TESISBILESENLERISAYAC" + tk.muhasebe.kod + tk.harcamaBirimi.kod.Replace(".", ""), "0"), 0);

                    sonuc = tabTK.Kaydet(tk);
                }
                else
                {
                    sonuc = tabTK.Guncelle(tk);
                    sonuc = tabTK.SilDetay(tk.refId);
                }

                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = tk.refId;

                    foreach (TesisBilesenleriDetay tkd in tk.detay.objeler)
                    {
                        tkd.refID = Guid.NewGuid().ToString();
                        tkd.refIDAna = tk.refId;
                        tabTK.KaydetDetay(tkd);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc DurumGuncelle(VeriErisim veriErisim, Kullanici kullanan, string refId, int durumKod)
        {
            Sonuc sonuc = new Sonuc();
            if (refId == "") return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTesisBilesenleriCetveli tabTK = new TABTesisBilesenleriCetveli(ve);
            ve.BeginTransaction();

            sonuc = tabTK.DurumGuncelle(refId, durumKod);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TesisBilesenleri tk)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTesisBilesenleriCetveli tabTK = new TABTesisBilesenleriCetveli(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTK.Listele(tk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            string detaySorguRefId = "";
            string muhasebeKod = "";
            string harcamaKod = "";
            string ambarKod = "";
            int sonKolon = (int)TABTesisBilesenleriCetveli.ENUMAlanlar.BULUNDUGUYER;
            while (dr.Read())
            {
                TesisBilesenleri gelen = new TesisBilesenleri();
                gelen.refId = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlar.REFID, string.Empty);
                detaySorguRefId = gelen.refId;

                gelen.muhasebe.kod = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                gelen.muhasebe.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod, "TASMUHASEBE");

                gelen.harcamaBirimi.kod = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty);
                gelen.harcamaBirimi.kod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaBirimi.kod);
                gelen.harcamaBirimi.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod + "-" + gelen.harcamaBirimi.kod, "TASHARCAMABIRIM");

                gelen.ambar.kod = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlar.AMBARKOD, string.Empty);
                gelen.ambar.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod + "-" + gelen.harcamaBirimi.kod + "-" + gelen.ambar.kod, "TASAMBAR");

                gelen.belgeNo = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlar.BELGENO, 0);
                gelen.belgeTarihi = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlar.BELGETARIHI);
                gelen.durum = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlar.DURUM, 0);

                gelen.sicilNo = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlar.SICILNO, "");
                gelen.hesapPlanKod = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlar.HESAPPLANKOD, "");
                gelen.bulunduguYer = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlar.BULUNDUGUYER, "");

                gelen.il.kod = VeriErisim.DegerAl(dr, sonKolon + 1, "");
                gelen.il.ad = VeriErisim.DegerAl(dr, sonKolon + 2, "");
                gelen.ilce.kod = VeriErisim.DegerAl(dr, sonKolon + 3, "");
                gelen.ilce.ad = VeriErisim.DegerAl(dr, sonKolon + 4, "");

                muhasebeKod = gelen.muhasebe.kod;
                harcamaKod = gelen.harcamaBirimi.kod;
                ambarKod = gelen.ambar.kod;

                donusDeger.objeler.Add(gelen);
            }
            dr.Close();

            if (tk.refId != "" || donusDeger.objeler.Count == 1)
            {
                if (tk.refId == "" && donusDeger.objeler.Count == 1)
                {
                    tk.refId = detaySorguRefId;
                }

                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABTASHesapPlani);

                foreach (TesisBilesenleri ts in donusDeger.objeler)
                {
                    ts.hesapPlanAd = Genel.AdGetir(isimler, ts.hesapPlanKod, "TASHESAPPLAN");
                    ts.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), ts.hesapPlanKod);

                    TesisBilesenleriDetay tkd = new TesisBilesenleriDetay();
                    tkd.refIDAna = tk.refId;

                    ts.detay = ListeleDetay(ve, kullanan, tkd);
                }
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, muhasebeKod, harcamaKod, ambarKod);

            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.objeler.Clear();
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM060;
            }

            return donusDeger;
        }

        public static ObjectArray ListeleDetay(VeriErisim veriErisim, Kullanici kullanan, TesisBilesenleriDetay tk)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTesisBilesenleriCetveli tabTK = new TABTesisBilesenleriCetveli(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTK.ListeleDetay(tk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                TesisBilesenleriDetay gelen = new TesisBilesenleriDetay();
                gelen.refID = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlarDetay.REFID, string.Empty);
                gelen.refIDAna = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlarDetay.REFIDANA, string.Empty);

                gelen.siraNo = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlarDetay.SIRANO, 0);
                gelen.adi = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlarDetay.ADI, "");
                gelen.miktar = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlarDetay.MIKTAR, 0.0);
                gelen.ozellik = VeriErisim.DegerAl(dr, (int)TABTesisBilesenleriCetveli.ENUMAlanlarDetay.OZELLIK, "");

                donusDeger.objeler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}
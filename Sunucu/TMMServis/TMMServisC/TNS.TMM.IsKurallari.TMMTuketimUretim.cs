using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class TMMTuketimUretim
    {
        /// <summary>
        /// Kaydets the specified veri erisim.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tu">The tu.</param>
        /// <returns></returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TuketimUretim tu)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (tu.yil <= 0)
                hata = TasinirMalResI.TMM036 + "<br>";
            if (tu.muhasebe == null || string.IsNullOrEmpty(tu.muhasebe.kod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (tu.harcamaBirimi == null || string.IsNullOrEmpty(tu.harcamaBirimi.kod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (tu.tur <= 0)
                hata += TasinirMalResI.TMM331 + "<br>";
            if (tu.ambar == null || string.IsNullOrEmpty(tu.ambar.kod))
                hata += TasinirMalResI.TMM031 + "<br>";
            if (tu.detay == null || tu.detay.Count <= 0)
                hata += TasinirMalResI.TMM332 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiKullaniciBirimi(kullanan, tu.muhasebe.kod, tu.harcamaBirimi.kod, tu.ambar.kod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM138;
                return sonuc;
            }

            hata = Genel.KontrolMuhasebeHarcamaAmbar(tu.muhasebe.kod, tu.harcamaBirimi.kod, tu.ambar.kod);
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTuketimUretim tabTU = new TABTuketimUretim(ve);
            ve.BeginTransaction();

            try
            {
                if (tu.belgeTarihi == null)
                    tu.belgeTarihi = new TNSDateTime(DateTime.Today);
                if (tu.belgeTarihi.isNull)
                    tu.belgeTarihi.Yaz(DateTime.Today);

                if (string.IsNullOrEmpty(tu.belgeNo))
                    sonuc = tabTU.Kaydet(tu);
                else
                {
                    tu.belgeNo.PadLeft(6, '0');
                    sonuc = tabTU.Guncelle(tu);
                }
                tu.belgeNo = sonuc.anahtar;

                if (sonuc.islemSonuc)
                {
                    sonuc = TMMTuketimUretimDetay.Kaydet(ve, kullanan, tu);
                    sonuc.anahtar = tu.belgeNo;
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

        /// <summary>
        /// Listeles the specified veri erisim.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tu">The tu.</param>
        /// <param name="detayVer">if set to <c>true</c> [detay ver].</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TuketimUretim tu, bool detayVer)
        {
            ObjectArray donusDeger = new ObjectArray();

            string hata = string.Empty;
            if (tu.yil <= 0)
                hata = TasinirMalResI.TMM036 + "<br>";
            if (tu.muhasebe == null || string.IsNullOrEmpty(tu.muhasebe.kod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (tu.harcamaBirimi == null || string.IsNullOrEmpty(tu.harcamaBirimi.kod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiKullaniciBirimi(kullanan, tu.muhasebe.kod, tu.harcamaBirimi.kod, tu.ambar == null ? string.Empty : tu.ambar.kod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTuketimUretim tabTU = new TABTuketimUretim(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTU.Listele(tu);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return donusDeger;
            }

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                TuketimUretim gelen = new TuketimUretim();
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABTuketimUretim.ENUMAlanlar.YIL, 0);

                gelen.muhasebe = new Muhasebe();
                gelen.muhasebe.kod = VeriErisim.DegerAl(dr, (int)TABTuketimUretim.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                gelen.muhasebe.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod, "TASMUHASEBE");

                gelen.harcamaBirimi = new HarcamaBirimi();
                gelen.harcamaBirimi.kod = VeriErisim.DegerAl(dr, (int)TABTuketimUretim.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty);
                gelen.harcamaBirimi.kod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaBirimi.kod);
                gelen.harcamaBirimi.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod + "-" + gelen.harcamaBirimi.kod, "TASHARCAMABIRIM");

                gelen.belgeNo = VeriErisim.DegerAl(dr, (int)TABTuketimUretim.ENUMAlanlar.BELGENO, string.Empty);
                gelen.tur = VeriErisim.DegerAl(dr, (int)TABTuketimUretim.ENUMAlanlar.TUR, 0);

                gelen.ambar = new Ambar();
                gelen.ambar.kod = VeriErisim.DegerAl(dr, (int)TABTuketimUretim.ENUMAlanlar.AMBARKOD, string.Empty);
                gelen.ambar.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod + "-" + gelen.harcamaBirimi.kod + "-" + gelen.ambar.kod, "TASAMBAR");

                gelen.belgeTarihi = VeriErisim.DegerAl(dr, (int)TABTuketimUretim.ENUMAlanlar.BELGETARIHI);

                if (detayVer)
                {
                    ObjectArray detayBilgi = TMMTuketimUretimDetay.Listele(ve2, kullanan, gelen);
                    if (!detayBilgi.sonuc.islemSonuc)
                    {
                        donusDeger.sonuc = detayBilgi.sonuc;
                        break;
                    }
                    gelen.detay = detayBilgi.objeler;
                }

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return donusDeger;
        }
    }
}
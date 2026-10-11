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
    public class TMMTalepKarsilamaDetay
    {
        /// <summary>
        /// Kaydets the specified veri erisim.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tk">The tk.</param>
        /// <returns></returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilama tk, bool tuketimTifOlustur)
        {
            Sonuc sonuc = new Sonuc();

            int i = 0;
            foreach (TalepKarsilamaDetay detay in tk.detay)
            {
                i++;

                string hata = detay.hesapPlani != null ? Genel.KontrolHesapPlan(kullanan, detay.hesapPlani.hesapKod, i, true, false) : string.Empty;
                if (!string.IsNullOrEmpty(hata))
                    hata += "<br>";
                hata += detay.ambar != null ? Genel.KontrolAmbar(tk.muhasebe.kod, tk.harcamaBirimi.kod, detay.ambar.kod) : string.Empty;
                if (!string.IsNullOrEmpty(hata))
                    hata += "<br>";
                if (detay.istenilenMiktar < detay.karsilananMiktar + detay.satinAlinanMiktar)
                    hata += TasinirMalResI.TMM336 + "<br>";
                if (!Yetki.SayfayaGirisYapabilirMi(kullanan))
                {
                    if (detay.ambar != null && !string.IsNullOrEmpty(detay.ambar.kod))
                        hata += TasinirMalResI.TMM382 + "<br>";
                    if (detay.karsilananMiktar > 0)
                        hata += TasinirMalResI.TMM383 + "<br>";
                    if (detay.satinAlinanMiktar > 0)
                        hata += TasinirMalResI.TMM384 + "<br>";
                    if (detay.tifYil > 0)
                        hata += TasinirMalResI.TMM385 + "<br>";
                    if (!string.IsNullOrEmpty(detay.tifFisNo))
                        hata += TasinirMalResI.TMM386 + "<br>";
                }
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr += hata;
                }
            }
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilamaDetay tabTKD = new TABTalepKarsilamaDetay(ve);
            ve.BeginTransaction();

            try
            {
                if (!tuketimTifOlustur)
                    TifNumaralariniEkle(ve, kullanan, tk);

                tabTKD.Sil(tk);

                int siraNo = 0;
                foreach (TalepKarsilamaDetay detay in tk.detay)
                {
                    detay.siraNo = ++siraNo;
                    if (detay.istenilenMiktar > 0 || detay.karsilananMiktar > 0 || detay.satinAlinanMiktar > 0)
                    {
                        detay.refIdAna = tk.refId;
                        detay.refId = Guid.NewGuid().ToString();
                        sonuc = tabTKD.Kaydet(tk, detay);
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

        /// <summary>
        /// Listeles the specified veri erisim.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tk">The tk.</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilama tk, bool harcamaModulu)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilamaDetay tabTKD = new TABTalepKarsilamaDetay(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTKD.Listele(tk);
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
            if (dr != null)
            {
                int sonKolon = (int)TABTalepKarsilamaDetay.ENUMAlanlar.PARABIRIMIKOD;
                while (dr.Read())
                {
                    if (harcamaModulu && VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.SATINALINANMIKTAR, (decimal)0) == 0)
                        continue;

                    TalepKarsilamaDetay gelen = new TalepKarsilamaDetay();
                    gelen.siraNo = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.SIRANO, 0);
                    gelen.hesapPlani = new HesapPlaniSatir();
                    gelen.hesapPlani.hesapKod = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.HESAPPLANKOD, string.Empty);
                    gelen.hesapPlani.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.hesapPlani.hesapKod);

                    gelen.hesapPlani.aciklama = VeriErisim.DegerAl(dr, sonKolon + 1, string.Empty);
                    gelen.hesapPlani.olcuBirimAd = VeriErisim.DegerAl(dr, sonKolon + 2, string.Empty);
                    if (gelen.hesapPlani.aciklama == "")
                    {
                        gelen.hesapPlani.aciklama = VeriErisim.DegerAl(dr, sonKolon + 3, string.Empty);
                        gelen.hesapPlani.olcuBirimAd = "Adet";
                    }


                    gelen.ambar = new Ambar();
                    gelen.ambar.kod = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.AMBARKOD, string.Empty);
                    if (!string.IsNullOrWhiteSpace(gelen.ambar.kod))
                        gelen.ambar.ad = Genel.AdGetir(isimler, tk.muhasebe.kod + "-" + tk.harcamaBirimi.kod + "-" + gelen.ambar.kod, "TASAMBAR");

                    gelen.istenilenMiktar = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.ISTENILENMIKTAR, (decimal)0);
                    gelen.karsilananMiktar = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.KARSILANANMIKTAR, (decimal)0);
                    gelen.satinAlinanMiktar = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.SATINALINANMIKTAR, (decimal)0);
                    gelen.aciklama = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.ACIKLAMA, string.Empty);
                    gelen.tifYil = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.TIFYIL, 0);
                    gelen.tifFisNo = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.TIFFISNO, string.Empty);
                    gelen.birimFiyat = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.BIRIMFIYAT, 0.0);

                    gelen.iptal = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.IPTAL, 0);

                    gelen.refIdAna = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.REFIDANA, string.Empty);
                    gelen.refId = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.REFID, string.Empty);
                    gelen.paraBirimiKod = VeriErisim.DegerAl(dr, (int)TABTalepKarsilamaDetay.ENUMAlanlar.PARABIRIMIKOD, string.Empty);

                    donusDeger.objeler.Add(gelen);
                }

                if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                    donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

                dr.Close();
            }

            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static bool SatinAlmaVarMi(TNSCollection tkDetay)
        {
            if (tkDetay == null || tkDetay.Count <= 0)
                return false;

            foreach (TalepKarsilamaDetay detay in tkDetay)
                if (detay.satinAlinanMiktar > 0)
                    return true;
            return false;
        }

        private static void TifNumaralariniEkle(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilama tk)
        {
            if (tk.detay == null || tk.detay.Count <= 0)
                return;

            ObjectArray detayListe = Listele(veriErisim, kullanan, tk, false);
            if (!detayListe.sonuc.islemSonuc)
                throw new Exception(detayListe.sonuc.hataStr);

            for (int i = 0; i < tk.detay.Count; i++)
            {
                TalepKarsilamaDetay tkd1 = (TalepKarsilamaDetay)tk.detay[i];
                if (detayListe.objeler.Count > i)
                {
                    TalepKarsilamaDetay tkd2 = (TalepKarsilamaDetay)detayListe.objeler[i];
                    tkd1.tifYil = tkd2.tifYil;
                    tkd1.tifFisNo = tkd2.tifFisNo;
                }
                else
                {
                    tkd1.tifYil = 0;
                    tkd1.tifFisNo = string.Empty;
                }
            }
        }

        public static Sonuc Iptal(VeriErisim veriErisim, Kullanici kullanan, string urunRefId)
        {
            Sonuc sonuc = new Sonuc();

            if (urunRefId == "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "İptal etmek istediğiniz ürünü seçiniz.";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilamaDetay tabTK = new TABTalepKarsilamaDetay(ve);
            ve.BeginTransaction();

            sonuc = tabTK.Iptal(urunRefId);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}
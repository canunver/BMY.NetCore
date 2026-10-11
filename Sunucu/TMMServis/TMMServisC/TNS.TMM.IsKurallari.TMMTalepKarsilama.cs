using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;
using TNS.DEG;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class TMMTalepKarsilama
    {
        private static Sonuc KaydetKontroluYap(Kullanici kullanan, TalepKarsilama tk)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (tk.yil <= 0)
                hata = TasinirMalResI.TMM036 + "<br>";
            if (tk.muhasebe == null || string.IsNullOrEmpty(tk.muhasebe.kod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (tk.harcamaBirimi == null || string.IsNullOrEmpty(tk.harcamaBirimi.kod))
                hata += TasinirMalResI.TMM014 + "<br>";
            //if (string.IsNullOrEmpty(tk.talepEdenTCNo))
            //    hata += TasinirMalResI.TMM326 + "<br>";
            if (tk.detay == null || tk.detay.Count <= 0)
                hata += TasinirMalResI.TMM325 + "<br>";
            if (tk.belgeTarihi.isNull)
                hata += "Belge Tarihi boş bırakılamaz." + "<br>";
            if (tk.tur == 0)
                hata += "Tür boş bırakılamaz." + "<br>";
            if (!tk.karsilamaIslemi && tk.amac == 0)
                hata += "Amaç boş bırakılamaz." + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            if (Yetki.SayfayaGirisYapabilirMi(kullanan))
            {
                sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, tk.muhasebe.kod, tk.harcamaBirimi.kod);
                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = TasinirMalResI.TMM138;
                    return sonuc;
                }
            }

            hata = Genel.KontrolMuhasebeHarcamaAmbar(tk.muhasebe.kod, tk.harcamaBirimi.kod, string.Empty);
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Kaydets the specified veri erisim.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tk">The tk.</param>
        /// <param name="tuketimTifOlustur">if set to <c>true</c> [tuketim tif olustur].</param>
        /// <returns></returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilama tk, bool tuketimTifOlustur)
        {
            Sonuc sonuc = KaydetKontroluYap(kullanan, tk);

            if (string.IsNullOrEmpty(tk.talepEdenTCNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr += TasinirMalResI.TMM326 + "<br>";
            }

            if (!sonuc.islemSonuc)
                return sonuc;

            string kurumKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
            ve.BeginTransaction();

            try
            {
                bool oncedenTalepKontrol = OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("OncedenTalepKontrol") + "", 0) > 0;
                if (oncedenTalepKontrol)
                {
                    sonuc = OncedenTalepVarmi(ve, tk);
                    if (!sonuc.islemSonuc)
                        return sonuc;
                }

                if (tk.belgeTarihi == null || tk.belgeTarihi.isNull)
                    tk.belgeTarihi = new TNSDateTime(DateTime.Today);

                if (tk.durum >= 999)
                {
                    tk.durum = (int)ENUMTalepKarsilamaDepoDurum.YENI;
                }
                else
                {
                    if (tk.belgeNo == "")
                        tk.durum = (int)ENUMTalepKarsilamaDurum.BEKLEMEDE;
                    else if (!Yetki.SayfayaGirisYapabilirMi(kullanan))
                        tk.durum = (int)ENUMTalepKarsilamaDurum.BEKLEMEDE;
                    else if (TMMTalepKarsilamaDetay.SatinAlmaVarMi(tk.detay))
                        tk.durum = (int)ENUMTalepKarsilamaDurum.SATINALMADA;
                    else
                        tk.durum = (int)ENUMTalepKarsilamaDurum.KAPATILDI;
                }


                if (string.IsNullOrEmpty(tk.belgeNo))
                {
                    tk.refId = Guid.NewGuid().ToString();

                    if (kurumKod.Replace(".", "") == "1399")
                        tk.belgeNo = new TABGenel(ve).BelgeNoAl("TALEPKARSILAMASAYAC", "0");

                    sonuc = tabTK.Kaydet(tk);
                    tk.belgeNo = sonuc.anahtar;
                }
                else
                {
                    tk.durum = tabTK.DurumBul(tk);
                    if (tk.durum > (int)ENUMTalepKarsilamaDurum.BEKLEMEDE)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = TasinirMalResI.TMM391;
                    }

                    if (sonuc.islemSonuc)
                        sonuc = tabTK.Guncelle(tk);
                }

                if (sonuc.islemSonuc)
                {
                    sonuc = TMMTalepKarsilamaDetay.Kaydet(ve, kullanan, tk, tuketimTifOlustur);
                    sonuc.anahtar = tk.belgeNo;
                    sonuc.kayitSay = tk.durum;
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = tk.belgeNo;
                sonuc.bilgiStr = TasinirMalResI.TMM017;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM018 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc DurumGuncelle(VeriErisim veriErisim, Kullanici kullanan, string refId, int durumKod)
        {
            Sonuc sonuc = new Sonuc();
            if (refId == "") return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
            ve.BeginTransaction();

            sonuc = tabTK.DurumGuncelle(refId, durumKod);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc DurumGuncelle(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilama tk)
        {
            Sonuc sonuc = new Sonuc();
            if (tk.belgeNo == "") return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
            ve.BeginTransaction();

            sonuc = tabTK.DurumGuncelle(tk);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc OncedenTalepVarmi(VeriErisim ve, TalepKarsilama tk)
        {
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);

            Sonuc sonuc = new Sonuc();

            foreach (TalepKarsilamaDetay tkd in tk.detay)
            {
                string talepBilgi = tabTK.OncedenTalepVarmi(tk.muhasebe.kod, tk.harcamaBirimi.kod, tk.belgeNo, tkd);
                if (talepBilgi != "")
                {
                    sonuc.hataStr += tkd.hesapPlani.hesapKod + " hesap nolu malzeme için karşılanmayan talebiniz bulunmaktadır. Yeni talepte bulunamazsınız.<br>Önceden talep edilen malzemenin belge bilgisi: " + talepBilgi + "<br>";
                }
            }

            if (sonuc.hataStr != "")
                sonuc.islemSonuc = false;

            return sonuc;

        }

        private static Sonuc ListeleKontroluYap(Kullanici kullanan, TalepKarsilama tk, bool harcamaModulu)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;

            if (tk.refId == "")
            {
                if (tk.yil <= 0)
                    hata = TasinirMalResI.TMM036 + "<br>";

                bool yonetici = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALYONETICISI);
                if (!yonetici)
                    yonetici = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);

                if (!yonetici)
                {
                    if (tk.muhasebe == null || string.IsNullOrEmpty(tk.muhasebe.kod))
                        hata += TasinirMalResI.TMM142 + "<br>";
                    if (tk.harcamaBirimi == null || string.IsNullOrEmpty(tk.harcamaBirimi.kod))
                        hata += TasinirMalResI.TMM014 + "<br>";

                    if (tk.belgeNo == "")
                    {
                        if (!Yetki.SayfayaGirisYapabilirMi(kullanan))
                            hata += "Şifresiz girişten talep listesi listeleme yapılamaz" + "<br>";
                    }
                }
            }

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            if (!harcamaModulu && Yetki.SayfayaGirisYapabilirMi(kullanan))
            {
                sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, tk.muhasebe.kod, tk.harcamaBirimi.kod);
                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = TasinirMalResI.TMM062;
                    return sonuc;
                }
            }

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilama tk, bool detayVer)
        {
            return Listele(veriErisim, kullanan, tk, detayVer, false);
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilama tk, bool detayVer, bool harcamaModulu)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = ListeleKontroluYap(kullanan, tk, harcamaModulu);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
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

            VeriErisim ve2 = null;
            if (detayVer)
                ve2 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                TalepKarsilama gelen = new TalepKarsilama();
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.YIL, 0);

                gelen.muhasebe = new Muhasebe();
                gelen.muhasebe.kod = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                gelen.muhasebe.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod, "TASMUHASEBE");

                gelen.harcamaBirimi = new HarcamaBirimi();
                gelen.harcamaBirimi.kod = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty);
                gelen.harcamaBirimi.kod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaBirimi.kod);
                gelen.harcamaBirimi.ad = Genel.AdGetir(isimler, gelen.muhasebe.kod + "-" + gelen.harcamaBirimi.kod, "TASHARCAMABIRIM");

                gelen.belgeNo = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.BELGENO, string.Empty);
                gelen.belgeTarihi = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.BELGETARIHI);

                gelen.talepEdenTCNo = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.TALEPEDENTCNO, string.Empty);
                gelen.talepedenAdSoyad = Genel.AdGetir(isimler, gelen.talepEdenTCNo, "TASPERSONEL");//Normal Taşınır Talep yapanlar için

                gelen.durum = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.DURUM, 0);
                gelen.talepedenAdSoyad = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.TALEPEDENADISOYADI, "");
                gelen.talepedenUnvan = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.TALEPEDENUNVAN, "");
                gelen.gerekce = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.GEREKCE, "");
                //gelen.karsilayanAdSoyad = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.KARSILAYANADISOYADI, "");
                //gelen.karsilayanUnvan = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.KARSILAYANUNVAN, "");
                gelen.refId = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.REFID, "");
                gelen.tur = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.TUR, 0);
                gelen.amac = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlar.AMAC, 0);
                if (gelen.amac == 0) gelen.amac = 1;//Kullanım

                if (detayVer)
                {
                    ObjectArray detayBilgi = TMMTalepKarsilamaDetay.Listele(ve2, kullanan, gelen, harcamaModulu);
                    if (!detayBilgi.sonuc.islemSonuc)
                    {
                        donusDeger.sonuc = detayBilgi.sonuc;
                        break;
                    }
                    gelen.detay = detayBilgi.objeler;

                    DepoDurumEkle(ve2, kullanan, gelen);
                }

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();
            if (detayVer)
                ve2.VeriErisimKapat();
            return donusDeger;
        }

        private static void DepoDurumEkle(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilama tk)
        {
            if (tk == null || tk.detay.Count <= 0)
                return;


            //Manas için yapılan talep belgesinde talep eden kişinin hacama birimindeki depo durumu gösterilecek 05.10.2020 Melih
            //string karsilayanMuhasebe = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRTALEPKARSILAMAMUHASEBE") + "";
            //string karsilayanHarcamaBirimi = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRTALEPKARSILAMAHARCAMABIRIM") + "";
            //string karsilayanAmbar = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRTALEPKARSILAMAAMBAR") + "";

            //if (karsilayanMuhasebe == "")
            //{
            //    karsilayanMuhasebe = tk.muhasebe.kod;
            //    karsilayanHarcamaBirimi = tk.harcamaBirimi.kod;
            //    karsilayanAmbar = "";
            //}



            DepoDurum dKriter = new DepoDurum();
            dKriter.yil = DateTime.Now.Year;
            //dKriter.muhasebeKod = karsilayanMuhasebe;
            //dKriter.harcamaKod = karsilayanHarcamaBirimi;
            //dKriter.ambarKod = karsilayanAmbar;
            dKriter.muhasebeKod = tk.muhasebe.kod;
            dKriter.harcamaKod = tk.harcamaBirimi.kod;
            dKriter.raporTur = (int)ENUMDepoDurumRaporTur.AMBAR;
            dKriter.yilDevri = true;

            foreach (TalepKarsilamaDetay tkd in tk.detay)
            {
                dKriter.ambarKod = tkd.ambar.kod;

                if (dKriter.ambarKod == "")
                    dKriter.raporTur = (int)ENUMDepoDurumRaporTur.HARCAMA;

                dKriter.hesapPlanKod = tkd.hesapPlani != null ? tkd.hesapPlani.hesapKod : string.Empty;

                if (string.IsNullOrEmpty(dKriter.hesapPlanKod)) continue;
                if (string.IsNullOrEmpty(dKriter.muhasebeKod)) continue;
                if (string.IsNullOrEmpty(dKriter.harcamaKod)) continue;

                ObjectArray depoDurumu = TMMRaporlar.TasinirDepoDurumu(veriErisim, kullanan, dKriter);
                if (!depoDurumu.sonuc.islemSonuc)
                    return;
                if (depoDurumu.objeler.Count <= 0)
                    continue;

                tkd.depoDurumu = ((DepoDurum)depoDurumu.objeler[0]).kalanMiktar;
            }
        }

        public static Sonuc TuketimTIFOlustur(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilama tk)
        {
            Sonuc sonuc = KaydetKontroluYap(kullanan, tk);
            if (!sonuc.islemSonuc)
                return sonuc;

            if (string.IsNullOrEmpty(tk.belgeNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM394;
                return sonuc;
            }

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, tk.muhasebe.kod, tk.harcamaBirimi.kod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM138;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                TasinirIslemForm tif = new TasinirIslemForm();
                tif.yil = DateTime.Now.Year;
                tif.fisTarih.Yaz(DateTime.Now);
                tif.muhasebeKod = tk.muhasebe.kod;
                tif.harcamaKod = tk.harcamaBirimi.kod;
                tif.islemTipTur = (int)ENUMIslemTipi.TUKETIMCIKIS;
                IslemTip it = new IslemTip();
                it.tur = tif.islemTipTur;
                ObjectArray itler = new TMMIslemTip().Listele(ve, kullanan, it);
                if (!itler.sonuc.islemSonuc || itler.objeler.Count <= 0)
                {
                    ve.VeriErisimKapat();
                    return itler.sonuc;
                }
                tif.islemTipKod = ((IslemTip)itler.objeler[0]).kod;
                tif.nereyeGitti = TMMServis.UzayYaziDegerGetir("TASPERSONEL", tk.talepEdenTCNo, true, string.Empty);
                if (string.IsNullOrEmpty(tif.nereyeGitti))
                    tif.nereyeGitti = ".";
                tif.kimeGitti = tk.talepEdenTCNo;

                string[] ambarlar = TIFAmbarlariniBul(tk.detay);
                if (ambarlar.Length > 0)
                {
                    Sonuc tifSonuc = new Sonuc(false, "", "");
                    foreach (string ambar in ambarlar)
                    {
                        tif.ambarKod = ambar;
                        tif.detay.objeler = TIFDetayOlustur(ve, kullanan, tk.detay, tif);
                        if (tif.detay.objeler.Count <= 0)
                            continue;
                        tif.fisNo = string.Empty;

                        tifSonuc = TMMTasinirIslemForm.Kaydet(ve, kullanan, tif);
                        if (!tifSonuc.islemSonuc)
                        {
                            tifSonuc.hataStr = string.Format(TasinirMalResI.TMM400, ambar) + "<br />" + tifSonuc.hataStr;
                            break;
                        }
                        tif.fisNo = tifSonuc.anahtar.Split('-')[0];
                        TalepDetayGuncelle(tk.detay, tif);

                        tifSonuc = TMMTasinirIslemForm.BelgeDurumDegistir(ve, kullanan, tif, "Onay");
                        if (!tifSonuc.islemSonuc)
                        {
                            tifSonuc.hataStr = string.Format(TasinirMalResI.TMM401, ambar, tif.fisNo) + "<br />" + tifSonuc.hataStr;
                            break;
                        }
                    }

                    sonuc = Kaydet(ve, kullanan, tk, true);
                    if (!tifSonuc.islemSonuc)
                        sonuc.bilgiStr = tifSonuc.hataStr;
                }
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM395;
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                sonuc.bilgiStr = TasinirMalResI.TMM393 + "<br />" + sonuc.bilgiStr;
            else
                sonuc.hataStr += "<br />" + sonuc.bilgiStr;

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static void TalepDetayGuncelle(TNSCollection tkDetay, TasinirIslemForm tif)
        {
            for (int i = 0; i < tkDetay.Count; i++)
            {
                TalepKarsilamaDetay tkd = (TalepKarsilamaDetay)tkDetay[i];
                if (tkd.ambar == null || string.IsNullOrEmpty(tkd.ambar.kod) || tkd.karsilananMiktar <= 0 || !tif.ambarKod.Equals(tkd.ambar.kod)
                    || (tkd.tifYil > 0 && !string.IsNullOrEmpty(tkd.tifFisNo)))
                    continue;

                tkd.tifYil = tif.yil;
                tkd.tifFisNo = tif.fisNo;
                if (tkd.satinAlinanMiktar > 0)
                {
                    TalepKarsilamaDetay tkdYeni = new TalepKarsilamaDetay();
                    tkdYeni.hesapPlani = tkd.hesapPlani;
                    tkdYeni.istenilenMiktar = tkd.istenilenMiktar - tkd.karsilananMiktar;
                    tkdYeni.satinAlinanMiktar = tkd.satinAlinanMiktar;
                    tkdYeni.aciklama = tkd.aciklama;
                    tkDetay.Add(tkdYeni);

                    tkd.istenilenMiktar = tkd.karsilananMiktar;
                    tkd.satinAlinanMiktar = 0;
                }
            }
        }

        private static string[] TIFAmbarlariniBul(TNSCollection tkDetay)
        {
            TNSCollection ambarlar = new TNSCollection();
            foreach (TalepKarsilamaDetay tkd in tkDetay)
            {
                if (tkd.ambar == null || string.IsNullOrEmpty(tkd.ambar.kod) || ambarlar.Contains(tkd.ambar.kod))
                    continue;
                ambarlar.Add(tkd.ambar.kod);
            }
            return (string[])ambarlar.ToArray(typeof(string));
        }

        private static TNSCollection TIFDetayOlustur(VeriErisim veriErisim, Kullanici kullanan, TNSCollection tkDetay, TasinirIslemForm tif)
        {
            int siraNo = 0;
            TNSCollection tuketimListesi = TuketimListesiBul(veriErisim, kullanan, tif);
            TNSCollection tifDetaylar = new TNSCollection();
            foreach (TalepKarsilamaDetay tkd in tkDetay)
            {
                if (tkd.hesapPlani == null || tkd.ambar == null || string.IsNullOrEmpty(tkd.ambar.kod) || tkd.karsilananMiktar <= 0 || !tif.ambarKod.Equals(tkd.ambar.kod)
                    || (tkd.tifYil > 0 && !string.IsNullOrEmpty(tkd.tifFisNo)))
                    continue;

                decimal kalanMiktar = tkd.karsilananMiktar;
                foreach (StokHareketBilgi shb in tuketimListesi)
                {
                    if (!shb.hesapPlanKod.Equals(tkd.hesapPlani.hesapKod))
                        continue;

                    TasinirIslemDetay tifDetay = new TasinirIslemDetay();
                    tifDetay.yil = tif.yil;
                    tifDetay.muhasebeKod = tif.muhasebeKod;
                    tifDetay.harcamaKod = tif.harcamaKod;
                    tifDetay.ambarKod = tif.ambarKod;
                    tifDetay.siraNo = ++siraNo;
                    tifDetay.hesapPlanKod = tkd.hesapPlani.hesapKod;
                    if (shb.miktar >= kalanMiktar)
                        tifDetay.miktar = kalanMiktar;
                    else
                        tifDetay.miktar = shb.miktar;
                    tifDetay.kdvOran = shb.kdvOran;
                    tifDetay.birimFiyat = shb.birimFiyat;
                    tifDetaylar.Add(tifDetay);

                    kalanMiktar -= tifDetay.miktar;
                    if (kalanMiktar == 0)
                        break;
                }
                if (kalanMiktar != 0)
                    throw new Exception(string.Format(TasinirMalResI.TMM392, tif.ambarKod, tkd.hesapPlani.hesapKod, tkd.karsilananMiktar - kalanMiktar, tkd.karsilananMiktar));
            }
            return tifDetaylar;
        }

        private static TNSCollection TuketimListesiBul(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tif)
        {
            StokHareketBilgi shbKriter = new StokHareketBilgi();
            shbKriter.yil = tif.yil;
            shbKriter.muhasebeKod = tif.muhasebeKod;
            shbKriter.harcamaKod = tif.harcamaKod;
            shbKriter.ambarKod = tif.ambarKod;

            ObjectArray shbler = TMMStokHareket.AmbardakiStoklariGetir(veriErisim, kullanan, shbKriter);
            if (!shbler.sonuc.islemSonuc)
                throw new Exception(shbler.sonuc.hataStr);
            return shbler.objeler;
        }

        public static Sonuc KaydetCevap(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilamaCevap tk)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
            ve.BeginTransaction();

            if (tk.refID != "")
            {
                ObjectArray liste = ListeleCevap(ve, kullanan, tk, false);
                foreach (TalepKarsilamaCevap ttkc in liste.objeler)
                {
                    if (ttkc.durum > 1)
                    {
                        sonuc.hataStr = "Kayıt edilmek istenilen İşlem onaylı olduğu için güncelleme yapılamaz.";
                        sonuc.islemSonuc = false;

                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }
            }

            if (tk.refID == "")
            {
                tk.refID = Guid.NewGuid().ToString();
                sonuc = tabTK.KaydetCevap(tk);
            }
            else
                sonuc = tabTK.GuncelleCevap(tk);

            if (sonuc.islemSonuc)
            {
                sonuc = tabTK.SilCevapDetay(tk);

                foreach (TalepKarsilamaCevapDetay tkcd in tk.detay)
                {
                    tkcd.refIDAna = tk.refID;
                    tkcd.refID = Guid.NewGuid().ToString();
                    sonuc = tabTK.KaydetCevapDetay(tkcd);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                DurumGuncelle(ve, kullanan, tk.refIDAna, (int)ENUMTalepKarsilamaDepoDurum.ISLEMYAPILIYOR);

                sonuc = TalepKarsilananMiktarGuncelle(null, kullanan, tk.refIDAna);

                sonuc.anahtar = tk.refID;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc SilCevap(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilamaCevap tk)
        {
            Sonuc sonuc = new Sonuc();

            if (tk.refID == "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Silmek istediğiniz işlemi seçiniz.";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
            ve.BeginTransaction();

            ObjectArray liste = ListeleCevap(ve, kullanan, tk, false);
            foreach (TalepKarsilamaCevap ttkc in liste.objeler)
            {
                if (ttkc.durum > 1)
                {
                    sonuc.hataStr = "Silmek istenilen İşlem onaylı olduğu için silinemez.";
                    sonuc.islemSonuc = false;

                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            sonuc = tabTK.SilCevapDetay(tk);
            if (sonuc.islemSonuc)
                sonuc = tabTK.SilCevap(tk);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc TamamlaCevap(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilamaCevap tk)
        {
            Sonuc sonuc = new Sonuc();

            if (tk.refID == "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Onaylamak istediğiniz işlemi seçiniz.";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
            ve.BeginTransaction();

            sonuc = tabTK.TamamlaCevap(tk);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray ListeleCevap(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilamaCevap tk, bool detayVer)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTK.ListeleCevap(tk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            VeriErisim ve2 = null;
            if (detayVer)
                ve2 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            while (dr.Read())
            {
                TalepKarsilamaCevap tkc = new TalepKarsilamaCevap();
                tkc.refID = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevap.REFID, "");
                tkc.refIDAna = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevap.REFIDANA, "");
                tkc.tarih = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevap.TARIH);
                tkc.aciklama = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevap.ACIKLAMA, "");
                tkc.durum = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevap.DURUM, 0);
                tkc.karsilayanAdSoyad = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevap.KARSILAYANADISOYADI, "");
                tkc.karsilayanUnvan = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevap.KARSILAYANUNVAN, "");

                if (detayVer)
                {
                    ObjectArray detayBilgi = TMMTalepKarsilama.ListeleCevapDetay(ve2, kullanan, tkc);
                    if (!detayBilgi.sonuc.islemSonuc)
                    {
                        donusDeger.sonuc = detayBilgi.sonuc;
                        break;
                    }
                    tkc.detay = detayBilgi.objeler;
                }

                donusDeger.objeler.Add(tkc);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();
            if (detayVer)
                ve2.VeriErisimKapat();

            return donusDeger;
        }

        public static ObjectArray ListeleCevapDetay(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilamaCevap tk)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
            IDataReader dr = null;

            try
            {
                TalepKarsilamaCevapDetay tkd = new TalepKarsilamaCevapDetay();
                tkd.refIDAna = tk.refID;
                dr = tabTK.ListeleCevapDetay(tkd);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            //int sonKolon = (int)TABTalepKarsilama.ENUMAlanlarCevapDetay.MIKTAR;

            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            while (dr.Read())
            {
                TalepKarsilamaCevapDetay tkc = new TalepKarsilamaCevapDetay();
                tkc.refID = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevapDetay.REFID, "");
                tkc.refIDAna = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevapDetay.REFIDANA, "");
                tkc.miktar = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevapDetay.MIKTAR, (decimal)0);
                tkc.tur = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevapDetay.TUR, "");
                tkc.satinAlmaDurum = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevapDetay.SATINALMADURUM, 0);
                tkc.refIDTalep = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevapDetay.REFIDTALEP, "");
                tkc.hesapPlani.hesapKod = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevapDetay.HESAPPLANKOD, "");
                tkc.hesapPlani.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, tkc.hesapPlani.hesapKod);
                tkc.hesapPlani.aciklama = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevapDetay.HESAPPLANAD, "");
                tkc.hesapPlani.olcuBirimAd = VeriErisim.DegerAl(dr, (int)TABTalepKarsilama.ENUMAlanlarCevapDetay.OLCUBIRIMI, "");

                donusDeger.objeler.Add(tkc);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();

            return donusDeger;
        }

        public static ObjectArray ListeleCevapSatinAlma(VeriErisim veriErisim, Kullanici kullanan, TalepKarsilamaCevap tk)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTK.ListeleCevapDetaySatinAlma(tk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            int sonKolon = 0;

            while (dr.Read())
            {
                TalepKarsilamaCevapDetay tkc = new TalepKarsilamaCevapDetay();
                tkc.refID = VeriErisim.DegerAl(dr, 0, "");
                tkc.refIDAna = VeriErisim.DegerAl(dr, 1, "");
                tkc.refIDTalep = VeriErisim.DegerAl(dr, 2, "");
                tkc.tur = VeriErisim.DegerAl(dr, 3, "");
                tkc.hesapPlani.hesapKod = VeriErisim.DegerAl(dr, 4, "");
                tkc.hesapPlani.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, tkc.hesapPlani.hesapKod);
                tkc.hesapPlani.aciklama = VeriErisim.DegerAl(dr, 5, "");
                tkc.hesapPlani.olcuBirimAd = VeriErisim.DegerAl(dr, 6, "");
                tkc.satinAlmaDurum = VeriErisim.DegerAl(dr, 7, 0);
                tkc.miktar = VeriErisim.DegerAl(dr, 8, (decimal)0);

                sonKolon = 8;
                tk = new TalepKarsilamaCevap();
                tk.refID = VeriErisim.DegerAl(dr, sonKolon + 1, "");
                tk.refIDAna = VeriErisim.DegerAl(dr, sonKolon + 2, "");
                tk.tarih = VeriErisim.DegerAl(dr, sonKolon + 3);
                tk.aciklama = VeriErisim.DegerAl(dr, sonKolon + 4, "");
                tk.durum = VeriErisim.DegerAl(dr, sonKolon + 5, 0);
                tk.karsilayanAdSoyad = VeriErisim.DegerAl(dr, sonKolon + 6, "");
                tk.karsilayanUnvan = VeriErisim.DegerAl(dr, sonKolon + 7, "");
                int yil = VeriErisim.DegerAl(dr, sonKolon + 8, 0);
                tk.muhasebe.kod = VeriErisim.DegerAl(dr, sonKolon + 9, "");
                tk.harcamaBirimi.kod = VeriErisim.DegerAl(dr, sonKolon + 10, "");
                tk.harcamaBirimi.ad = VeriErisim.DegerAl(dr, sonKolon + 11, "");
                tk.talepBelgeNo = VeriErisim.DegerAl(dr, sonKolon + 12, "");

                tkc.talepAciklama = VeriErisim.DegerAl(dr, sonKolon + 13, "");

                tk.detay.Add(tkc);

                donusDeger.objeler.Add(tk);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();

            return donusDeger;
        }

        public static Sonuc TalepKarsilananMiktarGuncelle(VeriErisim veriErisim, Kullanici kullanan, string refIDAna)
        {
            ObjectArray talepListe = new ObjectArray();

            TalepKarsilamaCevap tk = new TalepKarsilamaCevap();
            tk.refIDAna = refIDAna;
            ObjectArray liste = ListeleCevap(veriErisim, kullanan, tk, true);

            foreach (TalepKarsilamaCevap ttkc in liste.objeler)
            {
                foreach (TalepKarsilamaCevapDetay tkcd in ttkc.detay)
                {
                    bool bulundu = false;
                    foreach (TalepKarsilamaDetay ttt in talepListe.objeler)
                    {
                        if (ttt.refId == tkcd.refIDTalep)
                        {
                            if (tkcd.tur == "satinAl")
                                ttt.satinAlinanMiktar += tkcd.miktar;
                            if (tkcd.tur == "karsila")
                                ttt.karsilananMiktar += tkcd.miktar;

                            bulundu = true;
                            break;
                        }
                    }

                    if (!bulundu)
                    {
                        TalepKarsilamaDetay tkd = new TalepKarsilamaDetay();

                        tkd.refId = tkcd.refIDTalep;
                        if (tkcd.tur == "satinAl")
                            tkd.satinAlinanMiktar += tkcd.miktar;
                        if (tkcd.tur == "karsila")
                            tkd.karsilananMiktar += tkcd.miktar;

                        talepListe.objeler.Add(tkd);
                    }
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepKarsilama tabTK = new TABTalepKarsilama(ve);
            ve.BeginTransaction();

            Sonuc sonuc = new Sonuc();

            foreach (TalepKarsilamaDetay ttt in talepListe.objeler)
            {
                sonuc = tabTK.TalepKarsilananMiktarGuncelle(ttt);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

    }
}
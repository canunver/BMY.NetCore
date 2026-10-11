using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Stratejik hedeflerle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class STRHedef
    {
        /// <summary>
        /// Stratejik hedef varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Stratejik Hedef nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, StratejikHedef hedef)
        {
            Sonuc sonuc = YetkiSTR.OGYHakkiVarMi(kullanan, hedef.kurumKod, BHMv2.ENUMOGY.Yazma);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (hedef.donemBYil <= 0)
                hata = PerformansButceResI.STR003 + "<br>";
            if (hedef.dil.kod <= 0)
                hata += PerformansButceResI.STR004 + "<br>";
            if (hedef.strAmac.strAmacKod <= 0)
                hata += PerformansButceResI.STR007 + "<br>";
            if (string.IsNullOrEmpty(hedef.strHedefKisa))
                hata += PerformansButceResI.STR020 + "<br>";
            if (string.IsNullOrEmpty(hedef.strHedef))
                hata += PerformansButceResI.STR021 + "<br>";

            if (!hedef.ebutceKodKaydet)
            {
                if (hedef.birimler.Count < 1)
                    hata += "En az bir birim girmelisiniz!" + "<br>";
                for (int i = 0; i < hedef.birimler.Count; i++)
                {
                    StrKodStrAd birim = (StrKodStrAd)hedef.birimler[i];
                    if (birim != null && !string.IsNullOrWhiteSpace(birim.kod))
                    {
                        for (int j = i + 1; j < hedef.birimler.Count; j++)
                        {
                            StrKodStrAd birimIc = (StrKodStrAd)hedef.birimler[j];
                            if (birimIc != null && !string.IsNullOrWhiteSpace(birimIc.kod))
                            {
                                if (birim.kod == birimIc.kod)
                                {
                                    hata += birim.kod + " kodlu birim iki defa girilmiş!" + "<br>";
                                    break;
                                }
                            }
                        }
                    }
                }
                hata += MetinKontrol(ENUMSTRHEDEFAciklama.Riskler, hedef.riskler, -1);
                hata += MetinKontrol(ENUMSTRHEDEFAciklama.Stratejiler, hedef.stratejiler, -1);
                hata += MetinKontrol(ENUMSTRHEDEFAciklama.Tespitler, hedef.tespitler, -1);
                hata += MetinKontrol(ENUMSTRHEDEFAciklama.Ihtiyaclar, hedef.ihtiyaclar, -1);
                hata += MetinKontrol(ENUMSTRHEDEFAciklama.ProgramBagi, hedef.programBagi, 2, 2);

                if (hedef.ustPolitikaBaglari != null)
                {
                    int belSatir = 0;
                    foreach (StrUstPolitikaBelgesi ustPolBel in hedef.ustPolitikaBaglari)
                    {
                        belSatir++;
                        if (ustPolBel.ustBelgeKod == 0)
                            hata += belSatir + ". satırda üst politika belgesi seçilmemiş!" + "<br>";
                        else if (string.IsNullOrWhiteSpace(ustPolBel.referans))
                            hata += belSatir + ". satırda üst politika belgesi için referans girilmemiş!" + "<br>";
                    }
                }
            }

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHedef tHedef = new TABHedef(ve);
            TABYazi tYazi = new TABYazi(ve);

            StratejikAmac amac = new StratejikAmac(hedef.donemBYil, hedef.strAmac.strAmacKod, "", "", "", "", null, hedef.dil, hedef.kurumKod);
            if (!STRGenel.StratejikAmacVarMi(ve, amac))
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = PerformansButceResI.STR022 + "<br>";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            StratejikYazi yazi = new StratejikYazi(hedef);
            ve.BeginTransaction();
            if (hedef.ebutceKodKaydet)
            {
                sonuc = tYazi.Guncelle(yazi);
            }
            else
            {
                if (tHedef.HedefVarMi(hedef.kurumKod, hedef.donemBYil, hedef.strHedefKod))
                    sonuc = tHedef.Guncelle(hedef);
                else if (hedef.strHedefKod > 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = PerformansButceResI.STR023 + "<br>";
                }
                else
                {
                    hedef.strHedefKod = tHedef.HedefKodAl(hedef.kurumKod, hedef.donemBYil);
                    sonuc = tHedef.Kaydet(hedef);
                    yazi.kod = hedef.strHedefKod;
                }

                if (sonuc.islemSonuc)
                {
                    if (tYazi.YaziVarMi(yazi))
                        sonuc = tYazi.Guncelle(yazi);
                    else
                        sonuc = tYazi.Kaydet(yazi);
                }
                TABSTRHEDEFBIRIM tabBirim = new TABSTRHEDEFBIRIM(ve);
                if (sonuc.islemSonuc)
                {
                    sonuc = tabBirim.Sil(hedef);
                }

                int satirNo = 0;
                foreach (StrKodStrAd birim in hedef.birimler)
                {
                    if (!sonuc.islemSonuc) break;
                    if (birim != null && !string.IsNullOrWhiteSpace(birim.kod))
                        sonuc = tabBirim.Ekle(hedef, birim.kod, satirNo++);
                }

                TABSTRHEDEFACIKLAMA tabAciklama = new TABSTRHEDEFACIKLAMA(ve);
                if (sonuc.islemSonuc) sonuc = AciklamaKaydet(hedef, tabAciklama, hedef.riskler, ENUMSTRHEDEFAciklama.Riskler);
                if (sonuc.islemSonuc) sonuc = AciklamaKaydet(hedef, tabAciklama, hedef.stratejiler, ENUMSTRHEDEFAciklama.Stratejiler);
                if (sonuc.islemSonuc) sonuc = AciklamaKaydet(hedef, tabAciklama, hedef.tespitler, ENUMSTRHEDEFAciklama.Tespitler);
                if (sonuc.islemSonuc) sonuc = AciklamaKaydet(hedef, tabAciklama, hedef.ihtiyaclar, ENUMSTRHEDEFAciklama.Ihtiyaclar);
                if (sonuc.islemSonuc) sonuc = AciklamaKaydet(hedef, tabAciklama, hedef.programBagi, ENUMSTRHEDEFAciklama.ProgramBagi);

                TABSTRHEDEFUSTPOLITIKABELGESI tabUstPol = new TABSTRHEDEFUSTPOLITIKABELGESI(ve);
                if (sonuc.islemSonuc) tabUstPol.Sil(hedef.donemBYil, hedef.strHedefKod, hedef.kurumKod);

                satirNo = 0;
                foreach (StrUstPolitikaBelgesi ustPolBel in hedef.ustPolitikaBaglari)
                {
                    if (!sonuc.islemSonuc) break;
                    if (ustPolBel != null && ustPolBel.ustBelgeKod > 0)
                        sonuc = tabUstPol.Ekle(hedef.donemBYil, hedef.strHedefKod, hedef.kurumKod, satirNo++, ustPolBel);
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = hedef.strHedefKod.ToString();
                sonuc.bilgiStr = PerformansButceResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc AciklamaKaydet(StratejikHedef hedef, TABSTRHEDEFACIKLAMA tabAciklama, TNSCollection metinler, ENUMSTRHEDEFAciklama aciklamaTur)
        {
            int satirNo = 0;
            Sonuc sonuc = tabAciklama.Sil(hedef, aciklamaTur);
            foreach (string metin in metinler)
            {
                if (!sonuc.islemSonuc) break;
                if (metin != null && !string.IsNullOrWhiteSpace(metin))
                    sonuc = tabAciklama.Ekle(hedef, aciklamaTur, satirNo++, metin);
            }
            return sonuc;
        }

        private static string MetinKontrol(ENUMSTRHEDEFAciklama aciklamaTur, TNSCollection metinler, int enFazla, int enAz = -1)
        {
            int say = 0;
            if (metinler != null)
            {
                foreach (string item in metinler)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        say++;
                    }
                }
            }
            if (enFazla > 0)
                if (say > enFazla)
                    return TNS.PEB.Arac.HedefAciklamaAdBul(aciklamaTur) + " için en fazla " + enFazla + " satır giriniz!";
            if (enAz > 0)
                if (say < enAz)
                {
                    if (ENUMSTRHEDEFAciklama.ProgramBagi == aciklamaTur)
                        return "İlgili program ve hedefini giriniz!";
                    else
                        return TNS.PEB.Arac.HedefAciklamaAdBul(aciklamaTur) + " için en az" + enAz + " satır giriniz!";
                }
            return "";
        }

        /// <summary>
        /// Verilen kriterlere uygun olan stratejik hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, StratejikHedef hedef, bool hedefler)
        {
            if (string.IsNullOrWhiteSpace(hedef.kurumKod))
                hedef.kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();

            ObjectArray hedefVAN = new ObjectArray();
            hedefVAN.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!hedefVAN.sonuc.islemSonuc)
                return hedefVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHedef tHedef = new TABHedef(ve);

            IDataReader dr = tHedef.Listele(hedef, hedefler);
            if (dr != null)
            {
                while (dr.Read())
                {
                    StratejikHedef gelen = new StratejikHedef();
                    gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.strHedefKod = VeriErisim.DegerAl(dr, 2, 0);
                    gelen.dil.kod = VeriErisim.DegerAl(dr, 3, 0);
                    gelen.strHedef = VeriErisim.DegerAl(dr, 4, string.Empty);
                    gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                    gelen.dosya = VeriErisim.DegerAl(dr, 6, string.Empty);
                    gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                    gelen.kurumKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, 8, string.Empty));

                    gelen.strAmac.strAmacKod = VeriErisim.DegerAl(dr, 9, 0);
                    gelen.strHedefKisa = VeriErisim.DegerAl(dr, 10, string.Empty);
                    gelen.dil.ad = VeriErisim.DegerAl(dr, 11, "");
                    gelen.strAmac.strAmacKisa = VeriErisim.DegerAl(dr, 12, "");

                    gelen.strAmac.strAmac = VeriErisim.DegerAl(dr, 13, "");
                    gelen.strAmac.ebutceKod = VeriErisim.DegerAl(dr, 14, "");
                    gelen.aralikBasYil = VeriErisim.DegerAl(dr, 15, 0);
                    gelen.aralikSonYil = VeriErisim.DegerAl(dr, 16, 0);

                    hedefVAN.objeler.Add(gelen);
                }
                dr.Close();
            }
            else
                hedefVAN.sonuc.DegerAta(ve.hata);

            if (hedefVAN.objeler.Count == 0)
                hedefVAN.sonuc.bilgiStr = PerformansButceResI.STR012;
            if (hedef.strHedefKod > 0)
            {
                TABSTRHEDEFACIKLAMA tabAciklama = new TABSTRHEDEFACIKLAMA(ve);
                TABSTRHEDEFUSTPOLITIKABELGESI tabUstBel = new TABSTRHEDEFUSTPOLITIKABELGESI(ve);
                foreach (StratejikHedef gelen in hedefVAN.objeler)
                {
                    BirimleriOku(ve, gelen);
                    AciklamaOku(tabAciklama, hedef, ENUMSTRHEDEFAciklama.Riskler, gelen.riskler);
                    AciklamaOku(tabAciklama, hedef, ENUMSTRHEDEFAciklama.Stratejiler, gelen.stratejiler);
                    AciklamaOku(tabAciklama, hedef, ENUMSTRHEDEFAciklama.Tespitler, gelen.tespitler);
                    AciklamaOku(tabAciklama, hedef, ENUMSTRHEDEFAciklama.Ihtiyaclar, gelen.ihtiyaclar);
                    AciklamaOku(tabAciklama, hedef, ENUMSTRHEDEFAciklama.ProgramBagi, gelen.programBagi);
                    dr = tabUstBel.Listele(gelen.donemBYil, gelen.strHedefKod, gelen.kurumKod);
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            StrUstPolitikaBelgesi bel = new StrUstPolitikaBelgesi();
                            bel.ustBelgeKod = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFUSTPOLITIKABELGESI.ENUMAlanlar.USTBELGEKOD, 0);
                            bel.referans = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFUSTPOLITIKABELGESI.ENUMAlanlar.REFERANS, "");
                            bel.aciklama = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFUSTPOLITIKABELGESI.ENUMAlanlar.ACIKLAMA, "");
                            bel.ustBelgeAd = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFUSTPOLITIKABELGESI.ENUMAlanlar.ACIKLAMA + 1, "");
                            gelen.ustPolitikaBaglari.Add(bel);
                        }
                        dr.Close();
                    }
                }
            }
            ve.VeriErisimKapat();
            return hedefVAN;
        }

        private static void AciklamaOku(TABSTRHEDEFACIKLAMA tabAciklama, StratejikHedef hedef, ENUMSTRHEDEFAciklama aciklamaTur, TNSCollection liste)
        {
            IDataReader dr = tabAciklama.Oku(hedef, aciklamaTur);
            if (dr != null)
            {
                while (dr.Read())
                {
                    string aciklama = VeriErisim.DegerAl(dr, 5, "");
                    liste.Add(aciklama);
                }
                dr.Close();
            }
        }

        private static void BirimleriOku(VeriErisim ve, StratejikHedef hedef)
        {
            TABSTRHEDEFBIRIM tb = new TABSTRHEDEFBIRIM(ve);
            IDataReader dr = tb.Listele(hedef);
            if (dr != null)
            {
                while (dr.Read())
                {
                    StrKodStrAd kodAd = new StrKodStrAd(VeriErisim.DegerAl(dr, 0, ""), VeriErisim.DegerAl(dr, 1, ""));
                    kodAd.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", kodAd.kod);
                    kodAd.ad = kodAd.ad;
                    hedef.birimler.Add(kodAd);
                }
                dr.Close();
            }
        }

        /// <summary>
        /// Verilen dönem başlangıç yılına ait stratejik hedef özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        public static ObjectArray HedefKisaListele(VeriErisim veriErisim, Kullanici kullanan, string kurumKod, int donemBYil)
        {
            ObjectArray hedefVAN = new ObjectArray();
            hedefVAN.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!hedefVAN.sonuc.islemSonuc)
                return hedefVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHedef tHedef = new TABHedef(ve);

            IDataReader dr = tHedef.HedefKisaListele(kurumKod, donemBYil);
            while (dr.Read())
            {
                StratejikHedef gelen = new StratejikHedef();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.strHedefKod = VeriErisim.DegerAl(dr, 1, 0);
                gelen.strAmac.strAmacKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.strHedefKisa = VeriErisim.DegerAl(dr, 3, string.Empty);

                hedefVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (hedefVAN.objeler.Count == 0)
                hedefVAN.sonuc.bilgiStr = PerformansButceResI.STR012;

            ve.VeriErisimKapat();
            return hedefVAN;
        }

        /// <summary>
        /// Stratejik hedef, performans hedefte kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Stratejik Hedef nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedefler">Stratejik Hedef nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection hedefler)
        {
            if (hedefler == null || hedefler.Count == 0)
            {
                return new Sonuc("Silinecek hedefleri belirtiniz!");
            }

            StratejikHedef ilk = (StratejikHedef)hedefler[0];

            Sonuc sonuc = YetkiSTR.OGYHakkiVarMi(kullanan, ilk.kurumKod, BHMv2.ENUMOGY.Yazma);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHedef tHedef = new TABHedef(ve);
            TABSTRHEDEFGOSTERGE tHedefGos = new TABSTRHEDEFGOSTERGE(ve);
            TABSTRHEDEFODENEK tHedefOde = new TABSTRHEDEFODENEK(ve);
            TABSTRHEDEFBIRIM tHedefBir = new TABSTRHEDEFBIRIM(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();
            foreach (StratejikHedef h in hedefler)
            {
                //Hedef; Performans Hedefte kullanılmış mı kontrolü
                //*************************************************
                IPEBServis servisPEB = PEB.Arac.Tanimla();
                ObjectArray phler = servisPEB.PerformansHedefListele(kullanan, new PerformansHedef(), false, h.donemBYil, h.strHedefKod, true);
                if (!phler.sonuc.islemSonuc)
                {
                    sonuc = phler.sonuc;
                    break;
                }
                if (phler.objeler.Count > 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(PerformansButceResI.STR024, h.strHedefKod.ToString());
                    break;
                }
                //*************************************************

                //Hedefin yazı değerleri siliniyor.
                StratejikYazi yazi = new StratejikYazi(h);
                sonuc = tYazi.Sil(yazi);
                if (!sonuc.islemSonuc)
                    break;

                sonuc = tHedefGos.Sil(h.donemBYil, h.strHedefKod, h.kurumKod);
                if (!sonuc.islemSonuc)
                    break;

                sonuc = tHedefOde.Sil(h.donemBYil, h.strHedefKod, h.kurumKod);
                if (!sonuc.islemSonuc)
                    break;

                sonuc = tHedefBir.Sil(h);
                if (!sonuc.islemSonuc)
                    break;

                //Hedefin kendisi siliniyor.
                sonuc = tHedef.Sil(h);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.STR014;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.STR015 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static ObjectArray ListeleStrHedefGosterge(VeriErisim veriErisim, Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod)
        {
            ObjectArray oa = new ObjectArray();
            oa.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);

            if (!oa.sonuc.islemSonuc)
            {
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSTRHEDEFGOSTERGE tabSTRHEDEFGOSTERGE = new TABSTRHEDEFGOSTERGE(ve);

            IDataReader dr = tabSTRHEDEFGOSTERGE.Listele(donemBasYil, hedefKod, kurumKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                StrHedefGosterge detay = new StrHedefGosterge();
                detay.gostergeKod = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.GOSTERGEKOD, 0);
                detay.gerceklesen = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.GERCEKLESEN, 0.0);
                detay.tahmin0 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.TAHMIN0, 0.0);
                detay.tahmin1 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.TAHMIN1, 0.0);
                detay.tahmin2 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.TAHMIN2, 0.0);
                detay.tahmin3 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.TAHMIN3, 0.0);
                detay.tahmin4 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.TAHMIN4, 0.0);

                detay.gorKod = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.GORKOD, "");
                detay.hedefeEtkisi = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.HEDEFEETKISI, 0.0);
                detay.izlemeDonemi = (ENUMDonemler)VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.IZDONEMI, 0);
                detay.raporlamaDonemi = (ENUMDonemler)VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI, 0);

                detay.gostergeAd = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 1, "");
                detay.gostergeBirimAd = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 2, "");
                detay.ondalikSayisi = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 3, 0);
                detay.aralikBasYil = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 4, 0);
                detay.aralikSonYil = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 5, 0);
                detay.gostergeTur = (ENUMGostergeTur)VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 6, 0);
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc KaydetStrHedefGosterge(VeriErisim veriErisim, Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod, TNSCollection gostergeler)
        {
            Sonuc s = YetkiSTR.OGYHakkiVarMi(kullanan, kurumKod, BHMv2.ENUMOGY.Yazma);
            double etkiToplam = 0;
            for (int i = 0; i < gostergeler.Count; i++)
            {
                StrHedefGosterge gAna = (StrHedefGosterge)gostergeler[i];
                etkiToplam += gAna.hedefeEtkisi;
                if (gAna.gostergeKod == 0)
                    s.DegerAta(string.Format("{0}. satırda gösterge kodu boş bırakılmış", i + 1));
                for (int j = i + 1; j < gostergeler.Count; j++)
                {
                    StrHedefGosterge gIc = (StrHedefGosterge)gostergeler[j];
                    if (gAna.gostergeKod == gIc.gostergeKod)
                        s.DegerAta(string.Format("{0}. satır ile {1}. satırda aynı gösterge ({2}) kodu kullanılmış!", i + 1, j + 1, gAna.gostergeKod));
                }
            }
            etkiToplam = Math.Round(etkiToplam, 5);
            if (etkiToplam != 100)
                s.DegerAta("Gösterge etki toplamları (" + etkiToplam.ToString() + ") 100 olmalı!");

            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRHEDEFGOSTERGE tabSTRHEDEFGOSTERGE = new TABSTRHEDEFGOSTERGE(ve);

            ve.BeginTransaction();
            if (s.islemSonuc)
                s = tabSTRHEDEFGOSTERGE.Sil(donemBasYil, hedefKod, kurumKod);

            for (int i = 0; i < gostergeler.Count && s.islemSonuc; i++)
            {
                s = tabSTRHEDEFGOSTERGE.Ekle(donemBasYil, hedefKod, kurumKod, (StrHedefGosterge)gostergeler[i]);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectArray ListeleStrHedefOdenek(VeriErisim veriErisim, Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod)
        {
            ObjectArray oa = new ObjectArray();
            oa.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);

            if (!oa.sonuc.islemSonuc)
            {
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSTRHEDEFODENEK tabSTRHEDEFGOSTERGE = new TABSTRHEDEFODENEK(ve);

            IDataReader dr = tabSTRHEDEFGOSTERGE.Oku(donemBasYil, hedefKod, kurumKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                StrHedefOdenek detay = new StrHedefOdenek();
                detay.d0 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFODENEK.ENUMAlanlar.ODENEK0, 0.0);
                detay.d1 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFODENEK.ENUMAlanlar.ODENEK1, 0.0);
                detay.d2 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFODENEK.ENUMAlanlar.ODENEK2, 0.0);
                detay.d3 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFODENEK.ENUMAlanlar.ODENEK3, 0.0);
                detay.d4 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFODENEK.ENUMAlanlar.ODENEK4, 0.0);
                detay.ekoKod = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFODENEK.ENUMAlanlar.ODENEK4 + 1, "");
                detay.ekoAd = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFODENEK.ENUMAlanlar.ODENEK4 + 2, "");
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc KaydetStrHedefOdenek(VeriErisim veriErisim, Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod, TNSCollection odenekler)
        {
            Sonuc s = YetkiSTR.OGYHakkiVarMi(kullanan, kurumKod, BHMv2.ENUMOGY.Yazma);
            if (!s.islemSonuc)
            {
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRHEDEFODENEK tabSTRHEDEFGOSTERGE = new TABSTRHEDEFODENEK(ve);

            ve.BeginTransaction();
            if (s.islemSonuc)
                s = tabSTRHEDEFGOSTERGE.Sil(donemBasYil, hedefKod, kurumKod);

            for (int i = 0; i < odenekler.Count && s.islemSonuc; i++)
            {
                s = tabSTRHEDEFGOSTERGE.Ekle(donemBasYil, hedefKod, kurumKod, (StrHedefOdenek)odenekler[i]);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectArray ListeleStrHedefGostergeGerc(VeriErisim veriErisim, Kullanici kullanan, StratejikHedef hedef, int yil, int donem)
        {
            ObjectArray oa = new ObjectArray();
            oa.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);

            if (!oa.sonuc.islemSonuc)
            {
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSTRHEDEFGOSTERGEGERCEKLESME tab = new TABSTRHEDEFGOSTERGEGERCEKLESME(ve);
            StrHedefGosterge form = new StrHedefGosterge();
            form.gercYil = yil;
            form.donem = donem;
            form.donemBYil = hedef.donemBYil;
            form.kurumKod = hedef.kurumKod;
            form.hedefKod = hedef.strHedefKod;

            IDataReader dr = tab.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                StrHedefGosterge detay = new StrHedefGosterge();
                detay.gostergeKod = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.GOSTERGEKOD, 0);
                detay.gerceklesen = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.GERCEKLESEN, 0.0);
                detay.tahmin0 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.TAHMIN0, 0.0);
                detay.tahmin1 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.TAHMIN1, 0.0);
                detay.tahmin2 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.TAHMIN2, 0.0);
                detay.tahmin3 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.TAHMIN3, 0.0);
                detay.tahmin4 = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.TAHMIN4, 0.0);

                detay.gorKod = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.GORKOD, "");
                detay.hedefeEtkisi = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.HEDEFEETKISI, 0.0);
                detay.izlemeDonemi = (ENUMDonemler)VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.IZDONEMI, 0);
                detay.raporlamaDonemi = (ENUMDonemler)VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI, 0);

                detay.gostergeAd = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 1, "");
                detay.gostergeBirimAd = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 2, "");

                detay.gercYil = form.gercYil;
                detay.donem = form.donem;
                detay.donemBYil = form.donemBYil;
                detay.kurumKod = form.kurumKod;
                detay.hedefKod = form.hedefKod;
                detay.birikimliGerceklesme = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 4, 0.0);

                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 5, "");
                detay.ilgililik = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 6, "");
                detay.etkililik = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 7, "");
                detay.etkinlik = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 8, "");
                detay.surdurulebilirlik = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 9, "");

                detay.oncYilGerceklesme = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 10, 0.0);

                detay.aralikBasYil = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 11, 0);
                detay.aralikSonYil = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 12, 0);
                if (form.gercYil == detay.aralikBasYil)
                    detay.oncYilGerceklesme = detay.gerceklesen;
                detay.gostergeTur = (ENUMGostergeTur)VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 13, 0);
                detay.gostergeIlerlemeYon = (ENUMGostergeIlerlemeYonu)VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 14, 0);
                detay.gostergeOndalik = VeriErisim.DegerAl(dr, (int)TABSTRHEDEFGOSTERGE.ENUMAlanlar.RAPDONEMI + 15, 0);

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray ListeleStrHedefGercBilgi(VeriErisim veriErisim, Kullanici kullanan, StratejikHedef hedef, int yil, int donem, int bilgiTur)
        {
            ObjectArray oa = new ObjectArray();
            oa.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);

            if (!oa.sonuc.islemSonuc)
            {
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSTRHEDEFGOSTERGEGERCEKLESME tab = new TABSTRHEDEFGOSTERGEGERCEKLESME(ve);
            StrHedefGosterge form = new StrHedefGosterge();
            form.gercYil = yil;
            form.donem = donem;
            form.donemBYil = hedef.donemBYil;
            form.kurumKod = hedef.kurumKod;
            form.hedefKod = hedef.strHedefKod;

            IDataReader dr = tab.OkuGercBilgi(form, bilgiTur);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            int sat = 1;
            while (dr.Read())
            {
                oa.Ekle(new IntKodStrAd(sat++, VeriErisim.DegerAl(dr, 0, "")));
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc KaydetStrHedefGostergeGerc(VeriErisim veriErisim, Kullanici kullanan, TNSCollection gerceklesmeler, TNSCollection[] gercBilgiler, bool aktarim)
        {
            Sonuc s = new Sonuc();

            if (gerceklesmeler == null || gerceklesmeler.Count == 0)
            {
                s.DegerAta("Kaydetmek için lütfen gerçekleşme giriniz!");
                return s;
            }
            StrHedefGosterge ilkGerc = (StrHedefGosterge)gerceklesmeler[0];

            s = YetkiSTR.OGYHakkiVarMi(kullanan, ilkGerc.kurumKod, BHMv2.ENUMOGY.Yazma);
            if (!s.islemSonuc)
            {
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRHEDEFGOSTERGEGERCEKLESME tabSTRHEDEFGOSTERGE = new TABSTRHEDEFGOSTERGEGERCEKLESME(ve);

            ve.BeginTransaction();
            if (s.islemSonuc)
                s = tabSTRHEDEFGOSTERGE.Sil(ilkGerc, aktarim);

            if (s.islemSonuc && !aktarim)
                s = tabSTRHEDEFGOSTERGE.SilGercBilgi(ilkGerc);

            for (int i = 0; i < gerceklesmeler.Count && s.islemSonuc; i++)
            {
                StrHedefGosterge gerc = (StrHedefGosterge)gerceklesmeler[i];
                s = tabSTRHEDEFGOSTERGE.Ekle(gerc);
            }

            if (gercBilgiler != null && gercBilgiler.Length > 0 && !aktarim)
            {
                for (int gercNo = 0; gercNo < gercBilgiler.Length; gercNo++)
                {
                    int sat = 1;
                    foreach (IntKodStrAd eylemGerc in gercBilgiler[gercNo])
                    {
                        if (!s.islemSonuc) break;
                        s = tabSTRHEDEFGOSTERGE.EkleGercBilgi(ilkGerc, gercNo + 1, sat++, eylemGerc.ad);
                    }
                    if (!s.islemSonuc) break;
                }
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectArray UstPolitikaBelgeleriListele(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRHEDEFUSTPOLITIKABELGESI tabUstPol = new TABSTRHEDEFUSTPOLITIKABELGESI(ve);
            IDataReader dr = tabUstPol.UstPolitikaBelgeleriListele(yil);
            if (dr != null)
            {
                while (dr.Read())
                {
                    IntKodStrAd intK = new IntKodStrAd(VeriErisim.DegerAl(dr, 1, 0), VeriErisim.DegerAl(dr, 2, ""));
                    oa.Ekle(intK);
                }
                dr.Close();
            }
            else
                oa.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray RaporAl(VeriErisim veriErisim, Kullanici kullanan, StratejikHedef strHedef, int raporNo)
        {
            ObjectArray pVAN = new ObjectArray();
            Sonuc s = YetkiSTR.OGYHakkiVarMi(kullanan, strHedef.kurumKod, BHMv2.ENUMOGY.Gorme);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.hata = "";
            TABSTRHEDEFBIRIM tab = new TABSTRHEDEFBIRIM(ve);
            TABHedef tab2 = new TABHedef(ve);
            int alanSay = 0;
            IDataReader r = null;
            if (raporNo == 1)
            {
                alanSay = 2;
                r = tab.RaporHedefBirim(strHedef);
            }
            else if (raporNo == 2)
            {
                alanSay = 2;
                r = tab2.HedefMaliyet(strHedef);
            }
            if (r != null)
            {
                pVAN.sonuc.kayitSay = r.RecordsAffected;
                while (r.Read())
                {
                    IkiBoyutluTabloVeri ibv = new IkiBoyutluTabloVeri();
                    for (int i = 0; i < alanSay; i++)
                    {
                        StrKodStrAd sIc = new StrKodStrAd(VeriErisim.DegerAl(r, i * 2 + 0, ""), VeriErisim.DegerAl(r, i * 2 + 1, ""));
                        ibv.boyutlar.Add(sIc);
                    }
                    if (raporNo == 2)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            IkiBoyutluTabloVeri ibvIc = IkiBoyutluTabloVeriKopyala(ibv);
                            StrKodStrAd sIc = new StrKodStrAd(i.ToString("00"), "Planın " + (i + 1).ToString() + ". Yılı");
                            ibvIc.boyutlar.Add(sIc);

                            ibvIc.veri = VeriErisim.DegerAl(r, alanSay * 2 + i, 0.0);
                            ibvIc.veriTip = VariableType.Money;
                            pVAN.Ekle(ibvIc);
                        }
                    }
                    else
                    {
                        ibv.veri = VeriErisim.DegerAl(r, alanSay * 2, "");
                        ibv.veriTip = VariableType.String;
                        pVAN.Ekle(ibv);
                    }
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return pVAN;
        }

        private static IkiBoyutluTabloVeri IkiBoyutluTabloVeriKopyala(IkiBoyutluTabloVeri ibv)
        {
            IkiBoyutluTabloVeri ibIc = new IkiBoyutluTabloVeri();

            foreach (var item in ibv.boyutlar)
            {
                ibIc.boyutlar.Add(item);
            }

            return ibIc;
        }
    }
}
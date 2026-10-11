using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;
using TNS.UZY;

namespace TNS.BHMv2.IsKurallari
{
    public class TavanGenelIslemler
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        private static bool OnaylamaGormeYazmaYetkisiVarMi(Kullanici kullanan, string pebKod, ENUMOGY ogy)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (string.IsNullOrWhiteSpace(pebKod)) return false;
            int yetkiKod = 0;
            if (ogy == ENUMOGY.Gorme)
            {
                yetkiKod = 55;
            }
            else if (ogy == ENUMOGY.Yazma)
            {
                yetkiKod = 56;
            }
            else if (ogy == ENUMOGY.Onaylama)
            {
                yetkiKod = 57;
            }
            if (yetkiKod == 0) return false;

            string kod = kullanan.YetkiTipiDegeriBul(yetkiKod);
            pebKod = pebKod.Replace(".", "");
            if (kod == null) return false;
            kod = kod.Trim();
            if (string.IsNullOrWhiteSpace(kod)) return false;
            string[] kodlar = kod.Split(',', ';');
            for (int i = 0; i < kodlar.Length; i++)
            {
                string icKod = kodlar[i].Replace(".", "");
                if (!string.IsNullOrWhiteSpace(icKod))
                {
                    if (icKod == "*") return true;
                    if (pebKod.StartsWith(icKod))
                        return true;
                }
            }
            return false;
        }

        public static bool GormeYetkisiVarMi(Kullanici kullanan, string pebKod)
        {
            return OnaylamaGormeYazmaYetkisiVarMi(kullanan, pebKod, ENUMOGY.Gorme);
        }

        private static bool OnaylamaYetkisiVarMi(Kullanici kullanan, string pebKod)
        {
            return OnaylamaGormeYazmaYetkisiVarMi(kullanan, pebKod, ENUMOGY.Onaylama);
        }

        public static bool YazmaYetkisiVarMi(Kullanici kullanan, string pebKod)
        {
            return OnaylamaGormeYazmaYetkisiVarMi(kullanan, pebKod, ENUMOGY.Yazma);
        }

        internal static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TNSCollection tavanListe)
        {
            senaryo = senaryo.Replace(".", "");
            Sonuc s = new Sonuc(true, "", "");
            int zorunluAlanSay;
            if (tavanTur == ENUMTavanTur.tavanTurEkoSektorBirim) zorunluAlanSay = 3;
            else if (tavanTur == ENUMTavanTur.tavanTurEkoFaaliyet) zorunluAlanSay = 4;
            else if (tavanTur == ENUMTavanTur.tavanTurGorusmeCari) zorunluAlanSay = 5;
            else if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim || tavanTur == ENUMTavanTur.tavanTurTertipCari) zorunluAlanSay = 5;
            else if (tavanTur == ENUMTavanTur.tavanTurYatirimVize) zorunluAlanSay = 4;
            else if (tavanTur == ENUMTavanTur.tavanTurYatirimVizeTertip) zorunluAlanSay = 5;
            else if (tavanTur == ENUMTavanTur.tavanTurSeyyarDOSE) zorunluAlanSay = 3;
            else if (tavanTur == ENUMTavanTur.tavanTurIOIProje) zorunluAlanSay = 2;
            else if (tavanTur == ENUMTavanTur.tavanTurYIKOBProje) zorunluAlanSay = 3;
            else if (tavanTur == ENUMTavanTur.tavanTurSeyyarIlTavan) zorunluAlanSay = 1;
            else zorunluAlanSay = 6;
            Hashtable htToplam = new Hashtable();
            bool binKontrol = true;
            if (tavanTur == ENUMTavanTur.tavanTurSeyyarDOSE || tavanTur == ENUMTavanTur.tavanTurSeyyarIlTavan || tavanTur == ENUMTavanTur.tavanTurIOIProje || tavanTur == ENUMTavanTur.tavanTurYIKOBProje)
                binKontrol = false;
            for (int i = 0; i < tavanListe.Count; i++)
            {
                TavanGenel tg = (TavanGenel)tavanListe[i];

                for (int j = 0; j < zorunluAlanSay; j++)
                {
                    if (string.IsNullOrWhiteSpace(tg.kodlar[j]))
                    {
                        s.DegerAta("Lütfen tüm alanlarını doldurunuz.");
                        break;
                    }
                }

                for (int y = 0; y < 3; y++)
                {
                    if (tg.tavanlar[y] < 0)
                    {
                        s.DegerAta(string.Format("{0} yılı, {1} kodu için girilen miktar ({2}) sıfırdan küçük olamaz!", yil + y, tg.TertipKod, tg.tavanlar[y].ToString("#,##0")));
                    }
                    else
                    {
                        if (binKontrol)
                        {
                            decimal tv = Convert.ToDecimal(tg.tavanlar[y]);
                            decimal tv2 = Math.Floor(tv / 1000);
                            decimal tv3 = Math.Ceiling(tv / 1000);
                            if (tv2 != tv3)
                            {
                                s.DegerAta(string.Format("{0} yılı, {1} kodu için girilen miktar ({2}) binin katı olmalıdır!", yil + y, tg.TertipKod, tg.tavanlar[y].ToString("#,##0")));
                            }
                        }
                    }
                }

                if (tavanTur == ENUMTavanTur.tavanTurEkoFaaliyet && senaryo == "0000")
                {
                    if (tg.yil0 < tg.teklif0)
                    {
                        s.DegerAta(string.Format("{0} yılı, {1} kodu için girilen tavan miktarı ({2}), detaylara girilen ödeneklerden ({3}) daha küçük!", yil, tg.TertipKod, tg.yil0.ToString("#,##0"), tg.teklif0.ToString("#,##0")));
                    }
                    if (tg.yil1 < tg.teklif1)
                    {
                        s.DegerAta(string.Format("{0} yılı, {1} kodu için girilen tavan miktarı ({2}), detaylara girilen ödeneklerden ({3}) daha küçük!", yil + 1, tg.TertipKod, tg.yil1.ToString("#,##0"), tg.teklif1.ToString("#,##0")));
                    }
                    if (tg.yil2 < tg.teklif2)
                    {
                        s.DegerAta(string.Format("{0} yılı, {1} kodu için girilen tavan miktarı ({2}), detaylara girilen ödeneklerden ({3}) daha küçük!", yil + 2, tg.TertipKod, tg.yil2.ToString("#,##0"), tg.teklif2.ToString("#,##0")));
                    }
                }

                if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim || tavanTur == ENUMTavanTur.tavanTurTertipCari)
                {
                    if (tg.ekOdenek < 0 || tg.ekOdenek1 < 0 || tg.ekOdenek2 < 0)
                    {
                        s.DegerAta(string.Format("{0} yılı, {1} kodu için girilen ek ödenek ({2}) sıfırdan küçük olamaz!", yil, tg.TertipKod, tg.ekOdenek.ToString("#,##0")));
                    }
                    if ((tg.ekOdenek > 0) && string.IsNullOrWhiteSpace(tg.ekGerekce))
                    {
                        s.DegerAta(string.Format("{0} yılı, {1} kodu için girilen ek ödenek ({2}) için gerekçe girilmemiş!", yil, tg.TertipKod, tg.ekOdenek.ToString("#,##0")));
                    }
                    if (tg.ekGerekce.Length > 300)
                    {
                        s.DegerAta(string.Format("{0} yılı, {1} kodu için girilen ek gerekçe 300 karakterden fazla olamaz!", yil, tg.TertipKod));
                    }
                }
                if (s.islemSonuc)
                {
                    string kod;
                    if (tavanTur == ENUMTavanTur.tavanTurEkoSektorBirim)
                        kod = tg.kod1 + "@" + tg.kod2;
                    else if (tavanTur == ENUMTavanTur.tavanTurEkoFaaliyet || tavanTur == ENUMTavanTur.tavanTurYatirimVize)
                        kod = tg.kod1 + "@" + tg.kod2 + "@" + tg.kod3;
                    else if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim)
                        kod = tg.kod4.Substring(0, 2) + "@@@" + tg.kod5;
                    else if (tavanTur == ENUMTavanTur.tavanTurTertipCari)
                        kod = tg.kod1 + "@" + tg.kod2 + "@" + tg.kod3 + "@" + tg.kod4.Substring(0, 5) + "@" + tg.kod5;
                    else if (tavanTur == ENUMTavanTur.tavanTurYatirimVizeTertip)
                        kod = tg.kod5.Substring(0, 2) + "@" + tg.kod4.Substring(0, 2) + "@" + tg.kod3 + "@" + tg.kod5;
                    //else if (tavanTur == ENUMTavanTur.tavanTurGorusmeCari)
                    //    kod = tg.kod5;
                    else
                        kod = "";
                    if (!string.IsNullOrWhiteSpace(kod))
                    {
                        TavanGenel toplam;
                        try
                        {
                            toplam = (TavanGenel)htToplam[kod];
                        }
                        catch { toplam = null; }
                        if (toplam == null)
                        {
                            toplam = new TavanGenel();
                            if (tavanTur == ENUMTavanTur.tavanTurEkoSektorBirim)
                            {
                                toplam.kod1 = tg.kod5;
                            }
                            else if (tavanTur == ENUMTavanTur.tavanTurEkoSektorBirim)
                            {
                                toplam.kod1 = tg.kod1;
                                toplam.kod2 = tg.kod2;
                                toplam.ad1 = tg.ad1;
                                toplam.ad2 = tg.ad2;
                            }
                            else if (tavanTur == ENUMTavanTur.tavanTurEkoFaaliyet || tavanTur == ENUMTavanTur.tavanTurYatirimVize)
                            {
                                toplam.kod1 = tg.kod1;
                                toplam.kod2 = tg.kod2;
                                toplam.kod3 = tg.kod3;
                                toplam.ad1 = tg.ad1;
                                toplam.ad2 = tg.ad2;
                                toplam.ad3 = tg.ad3;
                            }
                            else if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim)
                            {
                                toplam.kod1 = tg.kod4.Substring(0, 2);
                                toplam.kod2 = null;
                                toplam.kod3 = null;
                                toplam.kod4 = tg.kod5;
                                toplam.ad1 = tg.ad4;
                                toplam.ad2 = null;
                                toplam.ad3 = null;
                                toplam.ad4 = tg.ad5;
                            }
                            else if (tavanTur == ENUMTavanTur.tavanTurTertipCari)
                            {
                                toplam.kod1 = tg.kod1;
                                toplam.kod2 = tg.kod2;
                                toplam.kod3 = tg.kod3;
                                toplam.kod4 = tg.kod4.Substring(0, 5);
                                toplam.kod5 = tg.kod5;
                                toplam.ad1 = tg.ad1;
                                toplam.ad2 = tg.ad2;
                                toplam.ad3 = tg.ad3;
                                toplam.ad4 = tg.ad4;
                                toplam.ad5 = tg.ad5;
                            }
                            else if (tavanTur == ENUMTavanTur.tavanTurYatirimVizeTertip)
                            {
                                toplam.kod1 = tg.kod5.Substring(0, 2);
                                toplam.kod2 = tg.kod4.Substring(0, 2);
                                toplam.kod3 = tg.kod3;
                                toplam.kod4 = tg.kod5;
                                toplam.ad1 = tg.ad5;
                                toplam.ad2 = tg.ad4;
                                toplam.ad3 = tg.ad3;
                                toplam.ad4 = tg.ad5;
                            }
                            htToplam[kod] = toplam;
                        }

                        toplam.yil0 += tg.yil0;
                        toplam.yil1 += tg.yil1;
                        toplam.yil2 += tg.yil2;
                        toplam.ekOdenek += tg.ekOdenek;
                        toplam.ekOdenek1 += tg.ekOdenek1;
                        toplam.ekOdenek2 += tg.ekOdenek2;
                    }
                }
            }

            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());

            string hata = "";
            foreach (DictionaryEntry de in htToplam)
            {
                TavanGenel toplam = (TavanGenel)de.Value;
                TavanGenel tavanOnayDurum = new TavanGenel();
                tavanOnayDurum.kod1 = toplam.kod1;
                tavanOnayDurum.kod2 = toplam.kod2;
                tavanOnayDurum.kod3 = toplam.kod3;
                if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim || tavanTur == ENUMTavanTur.tavanTurTertipCari || tavanTur == ENUMTavanTur.tavanTurYatirimVizeTertip)
                    tavanOnayDurum.kod4 = toplam.kod4;
                if (tavanTur == ENUMTavanTur.tavanTurTertipCari)
                    tavanOnayDurum.kod5 = toplam.kod5;

                string pebKod = "";
                if (tavanTur == ENUMTavanTur.tavanTurEkoFaaliyet)
                    pebKod = toplam.kod3;
                else if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim)
                    pebKod = toplam.kod4;
                else if (tavanTur == ENUMTavanTur.tavanTurTertipCari)
                    pebKod = toplam.kod5;
                else if (tavanTur == ENUMTavanTur.tavanTurYatirimVize)
                    pebKod = toplam.kod1;
                else if (tavanTur == ENUMTavanTur.tavanTurYatirimVizeTertip)
                    pebKod = toplam.kod4;
                else if (tavanTur == ENUMTavanTur.tavanTurGorusmeCari)
                    pebKod = toplam.kod5;
                if (!YazmaYetkisiVarMi(kullanan, pebKod))
                {
                    string pebTurAd;
                    if (pebKod.Length <= 2)
                        pebTurAd = "Birim Kodunu";
                    else
                        pebTurAd = "Faaliyet kodunu";

                    hata = string.Format("Verdiğiniz {0} {1} değiştirme yetkiniz bulunmamaktadır!", pebTurAd, pebKod);
                    break;
                }

                if (hata == "")
                {
                    tavanOnayDurum = BHMTavanGenelOnayOku(ve, kullanan, yil, senaryo, tavanTur, tavanOnayDurum);

                    if (tavanOnayDurum != null && tavanOnayDurum.durum > 1)
                    {
                        hata += "Daha önce onaylanmış verileri değiştiremezsiniz!";
                        break;
                    }
                }

                if (tavanTur == ENUMTavanTur.tavanTurEkoFaaliyet && hata == "" && senaryo == "0000")
                {
                    //1. tavan onaylı ise kaydedilebilir.
                    tavanOnayDurum = new TavanGenel();
                    tavanOnayDurum.kod1 = toplam.kod1;
                    tavanOnayDurum.kod2 = toplam.kod2;
                    tavanOnayDurum.kod3 = "";
                    tavanOnayDurum = BHMTavanGenelOnayOku(ve, kullanan, yil, senaryo, ENUMTavanTur.tavanTurEkoSektorBirim, tavanOnayDurum);
                    if (tavanOnayDurum.durum <= 1)
                    {
                        hata += "Sektör ve ekonomik tavan girişi onaylanmadığından bu aşamaya kayıt yapamazsınız";
                        break;
                    }

                    ObjectArray oa = ListeleEkoSektorTavan(ve, kullanan, yil, senaryo, toplam.kod1, toplam.kod2, toplam.kod3, "0600");
                    if (oa == null)
                    {
                        hata += "Birim tavanı okunamadı!";
                        break;
                    }
                    else if (!oa.sonuc.islemSonuc)
                    {
                        hata = oa.sonuc.hataStr;
                        break;
                    }
                    else if (oa.ObjeSayisi != 1)
                    {
                        hata += "Birim tavanı okunamadı (beklenmeyen sayıda tavan bulundu)!";
                        break;
                    }
                    else
                    {
                        TavanGenel birimTavan = (TavanGenel)oa.objeler[0];
                        for (int y = 0; y < 3; y++)
                        {
                            if (birimTavan.tavanlar[y] < toplam.tavanlar[y])
                            {
                                hata += string.Format("{0} yılı için, birim tavanından ({1}) daha fazla miktar ({2}) giriyorsunuz!", yil + y, birimTavan.tavanlar[y].ToString("#,##0"), toplam.tavanlar[y].ToString("#,##0"));
                            }
                        }
                    }
                }
                if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim && hata == "")
                {
                    //1. tavan onaylı ise kaydedilebilir.
                    tavanOnayDurum = new TavanGenel();
                    tavanOnayDurum.kod1 = toplam.kod1;
                    tavanOnayDurum.kod2 = SektorKodBul(ve, yil, senaryo, toplam.kod4);
                    tavanOnayDurum.kod3 = toplam.kod4.Substring(0, 2);
                    tavanOnayDurum.kod4 = "";
                    tavanOnayDurum = BHMTavanGenelOnayOku(ve, kullanan, yil, senaryo, ENUMTavanTur.tavanTurEkoFaaliyet, tavanOnayDurum);
                    if (tavanOnayDurum.durum <= 1)
                    {
                        hata += "Birim tavanı girişi onaylanmadığından bu aşamaya kayıt yapamazsınız!";
                        break;
                    }
                    if (tavanOnayDurum.durum == 6)
                    {
                        hata += "Birim tavan girişi, veri girişine kapatıldığından bu aşamaya kayıt yapamazsınız";
                        break;
                    }

                    ObjectArray oa = ListeleEkoSektorFaaliyetTavan(ve, kullanan, yil, senaryo, toplam.kod1, null, null, toplam.kod4, true);
                    if (oa == null)
                    {
                        hata += "Birim tavanı okunamadı!";
                        break;
                    }
                    else if (!oa.sonuc.islemSonuc)
                    {
                        hata = oa.sonuc.hataStr;
                        break;
                    }
                    else if (oa.ObjeSayisi != 1)
                    {
                        hata += string.Format("Birim tavanı{0}-{1} okunamadı (beklenmeyen sayıda{2} tavan bulundu)!", toplam.kod4, toplam.kod1, oa.ObjeSayisi);
                        break;
                    }
                    else
                    {
                        TavanGenel birimTavan = (TavanGenel)oa.objeler[0];
                        for (int y = 0; y < 3; y++)
                        {
                            if (birimTavan.tavanlar[y] < toplam.tavanlar[y])
                            {
                                hata += string.Format("{0} yılı için, faaliyet tavanından ({1}) daha büyük değer ({2}) giriyorsunuz!", yil + y, birimTavan.tavanlar[y].ToString("#,##0"), toplam.tavanlar[y].ToString("#,##0"));
                            }
                        }
                        if (senaryo == "0100")
                        {
                            if (birimTavan.ekOdenek < toplam.ekOdenek)
                            {
                                hata += string.Format("{0} yılı için, ek ödenek tavanından ({1}) daha büyük değer ({2}) giriyorsunuz!", yil + 0, birimTavan.ekOdenek.ToString("#,##0"), toplam.ekOdenek.ToString("#,##0"));
                            }
                            if (birimTavan.ekOdenek1 < toplam.ekOdenek1)
                            {
                                hata += string.Format("{0} yılı için, ek ödenek tavanından ({1}) daha büyük değer ({2}) giriyorsunuz!", yil + 1, birimTavan.ekOdenek1.ToString("#,##0"), toplam.ekOdenek1.ToString("#,##0"));
                            }
                            if (birimTavan.ekOdenek2 < toplam.ekOdenek2)
                            {
                                hata += string.Format("{0} yılı için, ek ödenek tavanından ({1}) daha büyük değer ({2}) giriyorsunuz!", yil + 2, birimTavan.ekOdenek2.ToString("#,##0"), toplam.ekOdenek2.ToString("#,##0"));
                            }
                        }
                    }
                }
                if (tavanTur == ENUMTavanTur.tavanTurYatirimVizeTertip && hata == "")
                {
                    //tavanTurYatirimVize tavanı onaylı ise kaydedilebilir.
                    tavanOnayDurum = new TavanGenel();
                    tavanOnayDurum.kod1 = toplam.kod1;
                    tavanOnayDurum.kod2 = toplam.kod2;
                    tavanOnayDurum.kod3 = toplam.kod3;
                    tavanOnayDurum.kod4 = "";
                    tavanOnayDurum = BHMTavanGenelOnayOku(ve, kullanan, yil, senaryo, ENUMTavanTur.tavanTurYatirimVize, tavanOnayDurum);
                    if (tavanOnayDurum.durum <= 1)
                    {
                        hata += "Birim tavanı girişi onaylanmadığından bu aşamaya kayıt yapamazsınız!";
                        break;
                    }
                    if (tavanOnayDurum.durum == 6)
                    {
                        hata += "Birim tavan girişi, veri girişine kapatıldığından bu aşamaya kayıt yapamazsınız";
                        break;
                    }

                    ObjectArray oa = ListeleYatirimVizeTavan(ve, kullanan, yil, senaryo, toplam.kod1, toplam.kod2, toplam.kod3, toplam.kod4, -1);
                    if (oa == null)
                    {
                        hata += string.Format("Birim tavanı okunamadı ({0}-{1}-{2}-{3})!", toplam.kod1, toplam.kod2, toplam.kod3, toplam.kod4);
                        break;
                    }
                    else if (!oa.sonuc.islemSonuc)
                    {
                        hata = oa.sonuc.hataStr;
                        break;
                    }
                    else if (oa.ObjeSayisi != 1)
                    {
                        hata += string.Format("Birim tavanı okunurken ({0}-{1}-{2}-{3}) beklenmeyen sayıda ({4}) tavan bulundu)!", toplam.kod1, toplam.kod2, toplam.kod3, toplam.kod4, oa.ObjeSayisi);
                        break;
                    }
                    else
                    {
                        TavanGenel birimTavan = (TavanGenel)oa.objeler[0];
                        for (int y = 0; y < 3; y++)
                        {
                            if (birimTavan.tavanlar[y] < toplam.tavanlar[y])
                            {
                                hata += string.Format("{0} yılı için, faaliyet tavanından ({1}) daha büyük değer ({2}) giriyorsunuz!", yil + y, birimTavan.tavanlar[y].ToString("#,##0"), toplam.tavanlar[y].ToString("#,##0"));
                            }
                        }
                    }
                }

                if (tavanTur == ENUMTavanTur.tavanTurTertipCari && hata == "")
                {
                    //tavanTurYatirimVize tavanı onaylı ise kaydedilebilir.
                    tavanOnayDurum = new TavanGenel();
                    tavanOnayDurum.kod1 = toplam.kod5;
                    tavanOnayDurum.kod2 = "";
                    tavanOnayDurum = BHMTavanGenelOnayOku(ve, kullanan, yil, senaryo, ENUMTavanTur.tavanTurGorusmeCari, tavanOnayDurum);
                    if (tavanOnayDurum.durum < 5)
                    {
                        hata += "Birim tavanı girişi onaylanmadığından bu aşamaya kayıt yapamazsınız!";
                        break;
                    }
                    if (tavanOnayDurum.durum == 6)
                    {
                        hata += "Birim tavan girişi, veri girişine kapatıldığından bu aşamaya kayıt yapamazsınız";
                        break;
                    }

                    Tertip tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, toplam.kod1, toplam.kod2, toplam.kod3, toplam.kod4, toplam.kod5);

                    ObjectArray oa = ListeleGorusmeCari(ve, kullanan, tertip, senaryo, true);
                    if (oa == null)
                    {
                        hata += string.Format("Birim tavanı okunamadı ({0}-{1}-{2}-{3}-{4})!", toplam.kod1, toplam.kod2, toplam.kod3, toplam.kod4, toplam.kod5);
                        break;
                    }
                    else if (!oa.sonuc.islemSonuc)
                    {
                        hata = oa.sonuc.hataStr;
                        break;
                    }
                    else if (oa.ObjeSayisi != 1)
                    {
                        hata += string.Format("Birim tavanı okunurken ({0}-{1}-{2}-{3}-{4}) beklenmeyen sayıda ({5}) tavan bulundu)!", toplam.kod1, toplam.kod2, toplam.kod3, toplam.kod4, toplam.kod5, oa.ObjeSayisi);
                        break;
                    }
                    else
                    {
                        TavanGenel birimTavan = (TavanGenel)oa.objeler[0];
                        for (int y = 0; y < 3; y++)
                        {
                            if (birimTavan.tavanlar[y] != toplam.tavanlar[y])
                            {
                                hata += string.Format("{0} yılı için, faaliyet tavanından ({1}) daha farklı ({2}) giriyorsunuz!", yil + y, birimTavan.tavanlar[y].ToString("#,##0"), toplam.tavanlar[y].ToString("#,##0"));
                            }
                        }
                        if (birimTavan.ekOdenek != toplam.ekOdenek)
                            hata += string.Format("{0} yılı için, faaliyet EK tavanından ({1}) daha farklı ({2}) giriyorsunuz!", yil + 0, birimTavan.ekOdenek.ToString("#,##0"), toplam.ekOdenek.ToString("#,##0"));
                    }
                }
            }

            if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim || tavanTur == ENUMTavanTur.tavanTurTertipCari || tavanTur == ENUMTavanTur.tavanTurYatirimVizeTertip)
            {
                for (int i = 0; i < tavanListe.Count; i++)
                {
                    TavanGenel tg = (TavanGenel)tavanListe[i];
                    Tertip tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, tg.kod1, tg.kod2, tg.kod3, tg.kod4, tg.kod5);
                    if (tg.yil0 > 0 || tg.yil1 > 0 || tg.yil2 > 0 || tg.ekOdenek > 0 || tg.ekOdenek1 > 0 || tg.ekOdenek2 > 0)
                    {
                        if (!(tg.kod4 == "06.01.40.01" || tg.kod4 == "06014001"))
                        {
                            string tut = TertipAdBul(ve, kullanan, tertip);
                            if (string.IsNullOrWhiteSpace(tut))
                                hata += tertip.TertipStrOl() + " tertibi bulunamadı. Lütfen Strateji Başkanlığı ile iletişime geçerek bu tertibi açtırınız!";
                        }
                    }
                }
            }

            if (hata != "")
            {
                ve.VeriErisimKapat();
                s.DegerAta(hata);
                return s;
            }

            TABBHMTAVANGENEL tab = new TABBHMTAVANGENEL(ve);
            TABBHMODENEKGIDER tabOdenek = new TABBHMODENEKGIDER(ve);
            TABBHMTAVANUSTU tabTavanUstu = new TABBHMTAVANUSTU(ve);
            ve.BeginTransaction();
            for (int i = 0; i < tavanListe.Count && s.islemSonuc; i++)
            {
                TavanGenel tg = (TavanGenel)tavanListe[i];
                if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim || tavanTur == ENUMTavanTur.tavanTurTertipCari || tavanTur == ENUMTavanTur.tavanTurYatirimVizeTertip)
                {
                    Tertip tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, tg.kod1, tg.kod2, tg.kod3, tg.kod4, tg.kod5);
                    s = tabOdenek.CokYilliOdenekSil(yil, senaryo, tertip);
                    for (int icYil = 0; icYil < 3 && s.islemSonuc; icYil++)
                    {
                        double odenek = tg.tavanlar[icYil];
                        if (odenek > 0)
                            s = tabOdenek.Ekle(yil, senaryo, icYil, tertip, odenek);
                    }
                    if (s.islemSonuc)
                        s = tabTavanUstu.Sil(tertip, senaryo);
                    if ((tg.ekOdenek > 0 || tg.ekOdenek1 > 0 || tg.ekOdenek2 > 0) && s.islemSonuc)
                    {
                        TavanUstuTertipOdenek tutertip = new TavanUstuTertipOdenek();
                        tutertip.tertip = tertip;
                        tutertip.ekOdenek0 = tg.ekOdenek;
                        tutertip.ekOdenek1 = tg.ekOdenek1;
                        tutertip.ekOdenek2 = tg.ekOdenek2;
                        tutertip.gerekce = tg.ekGerekce;
                        s = tabTavanUstu.Ekle(tutertip.tertip, senaryo, tutertip.ekOdenek0, tutertip.ekOdenek1, tutertip.ekOdenek2, tutertip.gerekce);
                    }
                }
                else
                {
                    s = tab.Sil(yil, senaryo, tavanTur, tg);
                    if (s.islemSonuc && (tg.yil0 > 0 || tg.yil1 > 0 || tg.yil2 > 0 || tg.ekOdenek > 0 || tg.ekOdenek1 > 0 || tg.ekOdenek2 > 0))
                        s = tab.Ekle(yil, senaryo, tavanTur, tg);
                }
            }

            foreach (DictionaryEntry de in htToplam)
            {
                if (!s.islemSonuc) break;
                TavanGenel toplam = (TavanGenel)de.Value;
                s = tab.EkleTarihce(yil, senaryo, tavanTur, toplam, ENUMTarihceKod.KAYDEDILDI, kullanan.kullaniciKodu);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static string TertipAdBul(VeriErisim ve, Kullanici kullanan, Tertip tertip)
        {
            try
            {
                Tertip[] tertipler = TertipTanim.Listele(ve, kullanan, tertip, 2, null);
                if (tertipler == null || tertipler.Length == 0) return null;
                return tertipler[0].tertipAdi;
            }
            catch {return null;}
        }

        private static string SektorKodBul(VeriErisim ve, int yil, string senaryoKod, string faaliyetKod)
        {
            string cevap = ve.TekKolonSQLStr(TNS.YAT.IsKurallari.Veri.TABYATProje.SektorBulSql(yil, senaryoKod, faaliyetKod));
            if (string.IsNullOrWhiteSpace(cevap)) cevap = "00";
            return cevap;
        }

        internal static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel kistas, bool sadeceOnayli, bool bosEkle)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABBHMTAVANGENEL tab = new TABBHMTAVANGENEL(ve);

            ve.BeginTransaction();

            IDataReader dr = tab.Listele(yil, senaryo, tavanTur, kistas);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TavanGenel tg = new TavanGenel();
                    tg.kod1 = VeriErisim.DegerAl(dr, 0, "");
                    tg.kod2 = VeriErisim.DegerAl(dr, 1, "");
                    tg.kod3 = VeriErisim.DegerAl(dr, 2, "");
                    tg.kod4 = VeriErisim.DegerAl(dr, 3, "");
                    tg.kod5 = VeriErisim.DegerAl(dr, 4, "");
                    tg.kod6 = VeriErisim.DegerAl(dr, 5, "");
                    tg.yil0 = VeriErisim.DegerAl(dr, 6, 0.0);
                    tg.yil1 = VeriErisim.DegerAl(dr, 7, 0.0);
                    tg.yil2 = VeriErisim.DegerAl(dr, 8, 0.0);

                    tg.ekOdenek = VeriErisim.DegerAl(dr, 9, 0.0);
                    tg.ekOdenek1 = VeriErisim.DegerAl(dr, 10, 0.0);
                    tg.ekOdenek2 = VeriErisim.DegerAl(dr, 11, 0.0);


                    tg.ad1 = VeriErisim.DegerAl(dr, 12, "");
                    tg.ad2 = VeriErisim.DegerAl(dr, 13, "");
                    tg.ad3 = VeriErisim.DegerAl(dr, 14, "");
                    tg.ad4 = VeriErisim.DegerAl(dr, 15, "");
                    tg.ad5 = VeriErisim.DegerAl(dr, 16, "");
                    tg.ad6 = VeriErisim.DegerAl(dr, 17, "");

                    oa.Ekle(tg);
                }
                dr.Close();
            }
            if (sadeceOnayli)
            {
                for (int i = oa.objeler.Count - 1; i >= 0; i--)
                {
                    TavanGenel tg = (TavanGenel)oa.objeler[i];
                    TavanGenel form = new TavanGenel();
                    form.kod1 = tg.kod1;
                    form.kod2 = tg.kod2;
                    if (tavanTur == ENUMTavanTur.tavanTurEkoFaaliyet || tavanTur == ENUMTavanTur.tavanTurYatirimVize)
                        form.kod3 = tg.kod3;

                    form = BHMTavanGenelOnayOku(ve, kullanan, yil, senaryo, tavanTur, form);
                    if (form.durum < 5)
                    {
                        oa.objeler.RemoveAt(i);
                    }
                }
            }
            if (bosEkle)
            {
                oa.Ekle(kistas);
            }

            ve.VeriErisimKapat();
            return oa;
        }

        internal static TavanGenel BHMTavanGenelOnayOku(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            ObjectArray oa = BHMTavanGenelOnayListele(veriErisim, kullanan, yil, senaryo, tavanTur, form);
            if (!oa.sonuc.islemSonuc)
                return null;
            if (oa.ObjeSayisi == 1)
                return (TavanGenel)oa.objeler[0];
            else if (oa.ObjeSayisi > 1)
                return null;
            form.durum = 0;
            return form;
        }

        internal static ObjectArray BHMTavanGenelOnayListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABBHMTAVANGENEL tab = new TABBHMTAVANGENEL(ve);
            ObjectArray oa = new ObjectArray();
            //, string ekoKod, string sektorKod
            IDataReader dr = tab.ListeleOnay(yil, senaryo, tavanTur, form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TavanGenel formIc = new TavanGenel();
                    formIc.yil = VeriErisim.DegerAl(dr, 0, 0);
                    formIc.senaryoKod = VeriErisim.DegerAl(dr, 1, "");

                    formIc.kod1 = VeriErisim.DegerAl(dr, 2, "");
                    formIc.kod2 = VeriErisim.DegerAl(dr, 3, "");
                    formIc.kod3 = VeriErisim.DegerAl(dr, 4, "");
                    formIc.kod4 = VeriErisim.DegerAl(dr, 5, "");
                    formIc.kod5 = VeriErisim.DegerAl(dr, 6, "");
                    formIc.kod6 = VeriErisim.DegerAl(dr, 7, "");
                    formIc.durum = VeriErisim.DegerAl(dr, 8, 0);
                    oa.Ekle(formIc);
                }
                dr.Close();
            }
            else
                oa.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc BHMTavanGenelOnayKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!string.IsNullOrWhiteSpace(senaryo)) senaryo = senaryo.Replace(".", "");
            else s.DegerAta("Lütfen senaryo seçimi yapınız");
            string pebKod = "";
            string muhasebeKod = "";
            string birimKod = "";
            bool sis = false;
            bool pebTam = false;
            if (tavanTur == ENUMTavanTur.tavanTurEkoFaaliyet)
                pebKod = form.kod3;
            else if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim)
                pebKod = form.kod4;
            else if (tavanTur == ENUMTavanTur.tavanTurYatirimVizeTertip)
                pebKod = form.kod4;
            else if (tavanTur == ENUMTavanTur.tavanTurTertipCari)
            {
                pebKod = form.kod5;
                pebTam = true;
            }
            else if (tavanTur == ENUMTavanTur.tavanTurGorusmeCari)
            {
                pebKod = form.kod1;
                pebTam = true;
            }
            else if (tavanTur == ENUMTavanTur.tavanTurIOIProje || tavanTur == ENUMTavanTur.tavanTurYIKOBProje)
            {
                muhasebeKod = form.kod1;
                birimKod = form.kod2;
                pebKod = null;
            }
            else if (tavanTur == ENUMTavanTur.YATProjeYil || tavanTur == ENUMTavanTur.YATProjeTanim)
            {
                pebKod = null;
                sis = true;
            }
            else if (tavanTur == ENUMTavanTur.yollukPlanlamaForm)
            {
                pebKod = form.kod1;
                sis = true;
            }
            if (pebKod != null)
            {
                if (pebTam)
                {
                    if (pebKod.Replace(".", "").Length != 8)
                    {
                        s.DegerAta(string.Format("Lütfen faaliyet kodunu tam seçiniz!"));
                    }
                }
                if (!OnaylamaYetkisiVarMi(kullanan, pebKod))
                {
                    string pebTurAd;
                    if (pebKod.Length <= 2)
                        pebTurAd = "Birim Kodunu";
                    else
                        pebTurAd = "Faaliyet kodunu";

                    s.DegerAta(string.Format("Verdiğiniz {0} {1} değiştirme yetkiniz bulunmamaktadır!", pebTurAd, pebKod));
                }
            }
            else
            {
                if (sis)
                {
                    if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                    {
                        s.DegerAta("Bu işlemi yapmak için yetkiniz yok!");
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(muhasebeKod) || string.IsNullOrWhiteSpace(birimKod))
                    {
                        s.DegerAta("Onaylama işlemi için muhasebe birim kodlarını giriniz!");
                    }
                    else
                    {
                        if (!OnaylamaYetkisiVarMiBirim(kullanan, muhasebeKod, birimKod, form.durum == 1 ? MUH.ENUMOGY.OnayKaldir : MUH.ENUMOGY.Onaylama))
                        {
                            s.DegerAta(string.Format("Verdiğiniz {0} {1} kodlu birimi ile ilgili {2} işlemi yetkiniz bulunmamaktadır!", muhasebeKod, birimKod, form.durum == 1 ? "Onay Kaldırma" : "Onaylama"));
                        }
                    }
                }
            }
            if (!s.islemSonuc) return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            if (tavanTur == ENUMTavanTur.tavanTurTertipCari && senaryo == "0100" && form.durum == 5)
            {
                ObjectArray oa = ListeleGorusmeCari(ve, kullanan, new Tertip(yil, ENUMTabloKod.TABPEBOdenek, form.kod1, form.kod2, form.kod3, form.kod4, form.kod5), senaryo, true);
                if (oa.sonuc.islemSonuc)
                {
                    foreach (TavanGenel item in oa.objeler)
                    {
                        if (item.yil0 != item.sgb0 || item.yil1 != item.sgb1 || item.yil2 != item.sgb2)
                        {
                            s.DegerAta(string.Format("{0} faaliyeti için girilen tavanlar ile ({1} - {2} - {3}), ödenek toplamları ({4} - {5} - {6}) eşit olmadığından onaylanamaz.<br />", item.kod4, item.yil0.ToString("#,##0.00"), item.yil1.ToString("#,##0.00"), item.yil2.ToString("#,##0.00"), item.sgb0.ToString("#,##0.00"), item.sgb1.ToString("#,##0.00"), item.sgb2.ToString("#,##0.00")));
                        }
                        if (item.ekOdenek != item.ekSgb || item.ekOdenek1 != item.ekSgb1 || item.ekOdenek2 != item.ekSgb2)
                        {
                            s.DegerAta(string.Format("{0} faaliyeti için girilen ek ödenek tavanları ile ({1} - {2} - {3}), girilen ek ödenek toplamları ({4} - {5} - {6}) eşit olmadığından onaylanamaz.<br />", item.kod4, item.ekOdenek.ToString("#,##0.00"), item.ekOdenek1.ToString("#,##0.00"), item.ekOdenek2.ToString("#,##0.00"), item.ekSgb.ToString("#,##0.00"), item.ekSgb1.ToString("#,##0.00"), item.ekSgb2.ToString("#,##0.00")));
                        }
                    }
                }
                else
                    s = oa.sonuc;
            }
            else if (tavanTur == ENUMTavanTur.tavanTurTertipYatirim && senaryo == "0100" && form.durum == 5)
            {
                ObjectArray oa = ListeleEkoSektorFaaliyetTavan(ve, kullanan, yil, senaryo, form.kod1, form.kod2, form.kod3, pebKod, true);
                if (oa.sonuc.islemSonuc)
                {
                    foreach (TavanGenel item in oa.objeler)
                    {
                        if (item.yil0 != item.sgb0 || item.yil1 != item.sgb1 || item.yil2 != item.sgb2)
                        {
                            s.DegerAta(string.Format("{0} faaliyeti için girilen tavanlar ile ({1} - {2} - {3}), ödenek toplamları ({4} - {5} - {6}) eşit olmadığından onaylanamaz.<br />", item.kod4, item.yil0.ToString("#,##0.00"), item.yil1.ToString("#,##0.00"), item.yil2.ToString("#,##0.00"), item.sgb0.ToString("#,##0.00"), item.sgb1.ToString("#,##0.00"), item.sgb2.ToString("#,##0.00")));
                        }
                        if (item.ekOdenek != item.ekSgb || item.ekOdenek1 != item.ekSgb1 || item.ekOdenek2 != item.ekSgb2)
                        {
                            s.DegerAta(string.Format("{0} faaliyeti için girilen ek ödenek tavanları ile ({1} - {2} - {3}), girilen ek ödenek toplamları ({4} - {5} - {6}) eşit olmadığından onaylanamaz.<br />", item.kod4, item.ekOdenek.ToString("#,##0.00"), item.ekOdenek1.ToString("#,##0.00"), item.ekOdenek2.ToString("#,##0.00"), item.ekSgb.ToString("#,##0.00"), item.ekSgb1.ToString("#,##0.00"), item.ekSgb2.ToString("#,##0.00")));
                        }
                    }
                }
                else
                    s = oa.sonuc;
            }
            else if (tavanTur == ENUMTavanTur.tavanTurYatirimVizeTertip && senaryo == "1000" && form.durum == 5)
            {
                ObjectArray oa = ListeleYatirimVizeTavan(ve, kullanan, yil, senaryo, form.kod1, form.kod2, form.kod3, form.kod4, -1);
                if (oa.sonuc.islemSonuc)
                {
                    foreach (TavanGenel item in oa.objeler)
                    {
                        if (item.yil0 != item.sgb0 || item.yil1 != item.sgb1 || item.yil2 != item.sgb2)
                        {
                            s.DegerAta(string.Format("{0} faaliyeti için girilen tavanlar ile ({1} - {2} - {3}), ödenek toplamları ({4} - {5} - {6}) eşit olmadığından onaylanamaz.<br />", item.kod4, item.yil0.ToString("#,##0.00"), item.yil1.ToString("#,##0.00"), item.yil2.ToString("#,##0.00"), item.sgb0.ToString("#,##0.00"), item.sgb1.ToString("#,##0.00"), item.sgb2.ToString("#,##0.00")));
                        }
                    }
                }
                else
                    s = oa.sonuc;
            }
            else if (form.durum == 6 && tavanTur == ENUMTavanTur.tavanTurEkoFaaliyet)
            {
                ObjectArray oa = ListeleEkoSektorFaaliyetTavan(ve, kullanan, yil, senaryo, form.kod1, form.kod2, form.kod3, null, false);
                if (oa.sonuc.islemSonuc)
                {
                    foreach (TavanGenel item in oa.objeler)
                    {
                        if (senaryo == "0000")
                        {
                            if (item.yil0 != item.teklif0 || item.yil1 != item.teklif1 || item.yil2 != item.teklif2)
                            {
                                s.DegerAta(string.Format("{0} faaliyeti için girilen tavanlar ile, ödenek toplamları eşit olmadığından veri girişine kapatılamaz.", item.kod4));
                            }
                        }
                        else if (senaryo == "0100")
                        {
                            if (item.yil0 != item.sgb0 || item.yil1 != item.sgb1 || item.yil2 != item.sgb2)
                            {
                                s.DegerAta(string.Format("{0} faaliyeti için girilen tavanlar ile, ödenek toplamları eşit olmadığından veri girişine kapatılamaz.", item.kod4));
                            }
                            if (item.ekOdenek != item.ekSgb || item.ekOdenek1 != item.ekSgb1 || item.ekOdenek2 != item.ekSgb2)
                            {
                                s.DegerAta(string.Format("{0} faaliyeti için girilen ek ödenek tavanları ile, girilen ek ödenek toplamları eşit olmadığından veri girişine kapatılamaz.", item.kod4));
                            }
                        }
                    }
                }
                else
                    s = oa.sonuc;
            }
            else if (form.durum == 6 && tavanTur == ENUMTavanTur.tavanTurYatirimVize)
            {
                ObjectArray oa = ListeleYatirimVizeTavan(ve, kullanan, yil, "1000", form.kod1, form.kod2, form.kod3, null, -1);
                if (oa.sonuc.islemSonuc)
                {
                    foreach (TavanGenel item in oa.objeler)
                    {
                        if (item.yil0 != item.sgb0 || item.yil1 != item.sgb1 || item.yil2 != item.sgb2)
                        {
                            s.DegerAta(string.Format("{0} faaliyeti için girilen tavanlar ile, ödenek toplamları eşit olmadığından veri girişine kapatılamaz.", item.kod4));
                        }
                    }
                }
                else
                    s = oa.sonuc;
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABBHMTAVANGENEL tab = new TABBHMTAVANGENEL(ve);

            ve.BeginTransaction();

            s = tab.SilOnay(yil, senaryo, tavanTur, form);
            if (s.islemSonuc)
                s = tab.EkleOnay(yil, senaryo, tavanTur, form, form.durum);
            if (s.islemSonuc)
            {
                ENUMTarihceKod enumTarihceKod;
                if (form.durum == 6)
                    enumTarihceKod = ENUMTarihceKod.KAPATILDI;
                else if (form.durum == 5)
                    enumTarihceKod = ENUMTarihceKod.ONAYLANDI;
                else
                    enumTarihceKod = ENUMTarihceKod.ONAYKALDIRILDI;
                s = tab.EkleTarihce(yil, senaryo, tavanTur, form, enumTarihceKod, kullanan.kullaniciKodu);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        private static bool OnaylamaYetkisiVarMiBirim(Kullanici kullanan, string muhasebeKod, string birimKod, MUH.ENUMOGY onayKod)
        {
            TNS.MUH.IMUHServis ser = TNS.MUH.Arac.Tanimla();
            return ser.MuhasebeBirimYetkiVarMi(kullanan, muhasebeKod, birimKod, onayKod);
        }

        internal static ObjectArray ListeleTertipTavanSGB(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string[] kodlar)
        {
            string kurKod = kodlar[0];
            string fonKod = kodlar[1];
            string finKod = kodlar[2];
            string ekoKod = kodlar[3];
            string pebFaalKod = kodlar[4];
            ObjectArray oa = new ObjectArray();

            string[] sorgular = new string[10];
            string[] baglantilar = new string[10];

            string kurSorgu = "AND (KURKOD LIKE '" + kurKod.Replace(".", "") + "%')";
            string fonSorgu = "AND (FONKOD LIKE '" + fonKod.Replace(".", "") + "%')";
            string finSorgu = "AND (FINKOD LIKE '" + finKod.Replace(".", "") + "%')";

            string ekoSorgu = "AND (EKOKOD LIKE '" + ekoKod.Replace(".", "") + "%')";
            string pebSorgu = "AND (PEBKOD = '" + pebFaalKod.Replace(".", "") + "')";
            string senaryoTeklifSorgu = "AND SENARYOKOD='0000'";
            string senaryoGelenSorgu = "AND SENARYOKOD='" + senaryoKod.Replace(".", "") + "'";

            string sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, ODENEK from BHMODENEK where YIL={0} {1} AND TEKLIFYIL={2} {3} {4} {5} {6} {7} order by 1,2,3,4";

            sorgular[0] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "0", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[1] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "1", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[2] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "2", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[3] = String.Format(sql, yil.ToString(), senaryoTeklifSorgu, "0", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[4] = String.Format(sql, yil.ToString(), senaryoTeklifSorgu, "1", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[5] = String.Format(sql, yil.ToString(), senaryoTeklifSorgu, "2", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            baglantilar[0] = "BaglantiSatiriBHMv2";
            baglantilar[1] = "BaglantiSatiriBHMv2";
            baglantilar[2] = "BaglantiSatiriBHMv2";
            baglantilar[3] = "BaglantiSatiriBHMv2";
            baglantilar[4] = "BaglantiSatiriBHMv2";
            baglantilar[5] = "BaglantiSatiriBHMv2";

            string tavanUstu = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, ODENEK, ODENEK1, ODENEK2, GEREKCE FROM BHMTAVANUSTU WHERE (YIL = {0}) {1} {2} {3} {4} {5} {6} order by 1,2,3,4";
            sorgular[6] = string.Format(tavanUstu, yil, senaryoGelenSorgu, kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            baglantilar[6] = "BaglantiSatiriBHMv2";
            sorgular[7] = string.Format(tavanUstu, yil, senaryoTeklifSorgu, kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            baglantilar[7] = "BaglantiSatiriBHMv2";


            sorgular[8] = string.Format("SELECT KURKOD, FONKOD, FINKOD, EKOKOD, AD FROM BHMTERTIPLISTESIGIDER WHERE (YIL = " + VeriErisim.DegerYap(yil) + ") {0} {1} {2} {3} {4} order by 1,2,3,4", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            baglantilar[8] = "BaglantiSatiriBHMv2";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3 });
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel hk = new TavanGenel();

                hk.kod1 = alAnahtar.StrAl(0);
                hk.kod2 = alAnahtar.StrAl(1);
                hk.kod3 = alAnahtar.StrAl(2);
                hk.kod4 = alAnahtar.StrAl(3);
                hk.kod5 = pebFaalKod;

                hk.kod1 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(0), hk.kod1);
                hk.kod2 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(1), hk.kod2);
                hk.kod3 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(2), hk.kod3);
                hk.kod4 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(3), hk.kod4);

                hk.yil0 = alVeri.DblAl(0);
                hk.yil1 = alVeri.DblAl(1);
                hk.yil2 = alVeri.DblAl(2);
                hk.teklif0 = alVeri.DblAl(3);
                hk.teklif1 = alVeri.DblAl(4);
                hk.teklif2 = alVeri.DblAl(5);
                hk.ekOdenek = alVeri.DblAl(6);
                hk.ekOdenek1 = alVeri.DblAl(7);
                hk.ekOdenek2 = alVeri.DblAl(8);
                hk.ekGerekce = alVeri.StrAl(9);
                hk.ekOdenekTeklif = alVeri.DblAl(10);
                hk.ekOdenek1Teklif = alVeri.DblAl(11);
                hk.ekOdenek2Teklif = alVeri.DblAl(12);
                hk.ekGerekceTeklif = alVeri.StrAl(13);
                //if (string.IsNullOrWhiteSpace(hk.ekGerekce)) hk.ekGerekce = hk.ekGerekceTeklif;
                hk.ad4 = alVeri.StrAl(14);

                oa.Ekle(hk);
            }
            return oa;
        }

        internal static ObjectArray ListeleTertipTavan(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string[] kodlar)
        {
            string ekoKod = kodlar[3];
            string pebFaalKod = kodlar[4];
            ObjectArray oa = new ObjectArray();

            if (!GormeYetkisiVarMi(kullanan, pebFaalKod))
            {
                oa.sonuc.DegerAta(string.Format("Verdiğiniz faaliyet kodunu {0} görme yetkiniz bulunmamaktadır!", pebFaalKod));
                return oa;
            }
            if (senaryoKod != null) senaryoKod = senaryoKod.Replace(".", "");

            if (senaryoKod == "0100")
                return ListeleTertipTavanSGB(veriErisim, kullanan, yil, senaryoKod, kodlar);

            string[] sorgular = new string[10];
            string[] baglantilar = new string[10];

            string ekoSorgu = "AND (EKOKOD LIKE '" + ekoKod.Replace(".", "") + "%')";
            string pebSorgu = "AND (PEBKOD = '" + pebFaalKod.Replace(".", "") + "')";
            string senaryoOnayliSorgu = "AND SENARYOKOD='0600'";
            string senaryoGelenSorgu = "AND SENARYOKOD='" + senaryoKod.Replace(".", "") + "'";

            string sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, ODENEK from BHMODENEK where YIL={0} {1} AND TEKLIFYIL={2} {3} {4} order by 1,2,3,4";

            sorgular[0] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "0", ekoSorgu, pebSorgu);
            sorgular[1] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "1", ekoSorgu, pebSorgu);
            sorgular[2] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "2", ekoSorgu, pebSorgu);
            sorgular[3] = String.Format(sql, (yil - 2).ToString(), senaryoOnayliSorgu, "0", ekoSorgu, pebSorgu);
            sorgular[4] = String.Format(sql, (yil - 1).ToString(), senaryoOnayliSorgu, "0", ekoSorgu, pebSorgu);
            baglantilar[0] = "BaglantiSatiriBHMv2";
            baglantilar[1] = "BaglantiSatiriBHMv2";
            baglantilar[2] = "BaglantiSatiriBHMv2";
            baglantilar[3] = "BaglantiSatiriBHMv2";
            baglantilar[4] = "BaglantiSatiriBHMv2";

            string harcamaSorgu = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, SUM(HARCAMA) FROM ABSHARCAMA  WHERE (YIL = {0}) {1} {2} group by KURKOD, FONKOD, FINKOD, EKOKOD order by 1,2,3,4";

            sorgular[5] = string.Format(harcamaSorgu, yil - 2, ekoSorgu, pebSorgu);
            baglantilar[5] = "BaglantiSatiriBHMv2";

            sorgular[6] = string.Format(harcamaSorgu, yil - 1, ekoSorgu, pebSorgu);
            baglantilar[6] = "BaglantiSatiriBHMv2";

            string birimOdenekSorgu = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, SUM(ODENEK) FROM BHMBIRIMBUTCEHAZIRLIK WHERE (YIL = {0}) {1} {2} {3} group by KURKOD, FONKOD, FINKOD, EKOKOD order by 1,2,3,4";
            sorgular[7] = string.Format(birimOdenekSorgu, yil, ekoSorgu, pebSorgu, senaryoGelenSorgu);
            baglantilar[7] = "BaglantiSatiriBHMv2";

            sorgular[8] = string.Format("SELECT KURKOD, FONKOD, FINKOD, EKOKOD, AD FROM BHMTERTIPLISTESIGIDER WHERE (YIL = " + VeriErisim.DegerYap(yil) + ") {0} {1} order by 1,2,3,4", ekoSorgu, pebSorgu);
            baglantilar[8] = "BaglantiSatiriBHMv2";

            sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, ODENEK, ODENEK1, ODENEK2, GEREKCE FROM BHMTAVANUSTU WHERE (YIL = {0}) {1} {2} {3} order by 1,2,3,4";
            sorgular[9] = string.Format(sql, yil, senaryoGelenSorgu, ekoSorgu, pebSorgu);
            baglantilar[9] = "BaglantiSatiriBHMv2";

            //sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, ODENEK, ACIKLAMA FROM BHMTAHMINGIDER WHERE (YIL = {0}) {1} AND TEKLIFYIL=0 {2} {3} order by 1,2,3,4";
            //sorgular[10] = string.Format(sql, yil - 1, senaryoGelenSorgu, ekoSorgu, pebSorgu);
            //baglantilar[10] = "BaglantiSatiriBHMv2";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3 });
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();
            bool tasitVar = false;
            string kurKod = "";
            string fonKod = "";
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel hk = new TavanGenel();

                hk.kod1 = alAnahtar.StrAl(0);
                hk.kod2 = alAnahtar.StrAl(1);
                hk.kod3 = alAnahtar.StrAl(2);
                hk.kod4 = alAnahtar.StrAl(3);
                hk.kod5 = pebFaalKod;

                hk.kod1 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(0), hk.kod1);
                hk.kod2 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(1), hk.kod2);
                hk.kod3 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(2), hk.kod3);
                hk.kod4 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(3), hk.kod4);

                hk.yil0 = alVeri.DblAl(0);
                hk.yil1 = alVeri.DblAl(1);
                hk.yil2 = alVeri.DblAl(2);
                hk.odenek_2 = alVeri.DblAl(3);
                hk.odenek_1 = alVeri.DblAl(4);
                hk.harcama_2 = alVeri.DblAl(5);
                hk.harcama_1 = alVeri.DblAl(6);
                hk.talep_0 = alVeri.DblAl(7);
                hk.ad4 = alVeri.StrAl(8);
                hk.ekOdenek = alVeri.DblAl(9);
                hk.ekOdenek1 = alVeri.DblAl(10);
                hk.ekOdenek2 = alVeri.DblAl(11);
                hk.ekGerekce = alVeri.StrAl(12);
                //hk.ysHarcamaTahmin = alVeri.DblAl(11);
                //hk.ysHarcamaTahminGerekce = alVeri.StrAl(12);
                if ((hk.kod3 == "1" || hk.kod3 == "01") && hk.kod4 == "06.01.40.01")
                {
                    tasitVar = true;
                    if (string.IsNullOrEmpty(hk.ad4))
                        hk.ad4 = "Kara Taşıtı  Alımları";
                }
                else
                {
                    if (!hk.kod1.EndsWith("62"))
                    {
                        kurKod = hk.kod1;
                        fonKod = hk.kod2;
                    }
                }
                oa.Ekle(hk);
            }
            if (!tasitVar && !string.IsNullOrEmpty(kurKod) && ekoKod.StartsWith("06"))
            {
                TavanGenel hk = new TavanGenel();

                hk.kod1 = kurKod;
                hk.kod2 = fonKod;
                hk.kod3 = "01";
                hk.kod4 = "06.01.40.01";
                hk.kod5 = pebFaalKod;
                hk.ad4 = "Kara Taşıtı Alımları";

                oa.Ekle(hk);
            }
            return oa;
        }

        internal static ObjectArray ListeleTertipVize(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string finKod, string ekoKod, string pebFaalKod)
        {
            ObjectArray oa = new ObjectArray();

            if (!GormeYetkisiVarMi(kullanan, pebFaalKod))
            {
                oa.sonuc.DegerAta(string.Format("Verdiğiniz faaliyet kodunu {0} görme yetkiniz bulunmamaktadır!", pebFaalKod));
                return oa;
            }
            if (senaryoKod != null) senaryoKod = senaryoKod.Replace(".", "");

            string[] sorgular = new string[10];
            string[] baglantilar = new string[10];

            string ekoSorgu = "AND (EKOKOD LIKE '" + ekoKod.Replace(".", "") + "%')";
            string pebSorgu = "AND (PEBKOD = '" + pebFaalKod.Replace(".", "") + "')";
            string finSorgu = "AND (FINKOD = '" + finKod.Replace(".", "") + "')";
            string senaryoOnayliSorgu = "AND SENARYOKOD='0200'";
            string senaryoGelenSorgu = "AND SENARYOKOD='" + senaryoKod.Replace(".", "") + "'";

            string sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, ODENEK from BHMODENEK where YIL={0} {1} AND TEKLIFYIL={2} {3} {4} {5} order by 1,2,3,4";

            sorgular[0] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "0", ekoSorgu, pebSorgu, finSorgu);
            sorgular[1] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "1", ekoSorgu, pebSorgu, finSorgu);
            sorgular[2] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "2", ekoSorgu, pebSorgu, finSorgu);
            sorgular[3] = String.Format(sql, yil.ToString(), senaryoOnayliSorgu, "0", ekoSorgu, pebSorgu, finSorgu);
            sorgular[4] = String.Format(sql, yil.ToString(), senaryoOnayliSorgu, "1", ekoSorgu, pebSorgu, finSorgu);
            sorgular[5] = String.Format(sql, yil.ToString(), senaryoOnayliSorgu, "2", ekoSorgu, pebSorgu, finSorgu);
            sorgular[6] = string.Format("SELECT KURKOD, FONKOD, FINKOD, EKOKOD, AD FROM BHMTERTIPLISTESIGIDER WHERE (YIL = " + VeriErisim.DegerYap(yil) + ") {0} {1} {2} order by 1,2,3,4", ekoSorgu, pebSorgu, finSorgu);
            baglantilar[0] = "BaglantiSatiriBHMv2";
            baglantilar[1] = baglantilar[0];
            baglantilar[2] = baglantilar[0];
            baglantilar[3] = baglantilar[0];
            baglantilar[4] = baglantilar[0];
            baglantilar[5] = baglantilar[0];
            baglantilar[6] = baglantilar[0];

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3 });
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel hk = new TavanGenel();

                hk.kod1 = alAnahtar.StrAl(0);
                hk.kod2 = alAnahtar.StrAl(1);
                hk.kod3 = alAnahtar.StrAl(2);
                hk.kod4 = alAnahtar.StrAl(3);
                hk.kod5 = pebFaalKod;

                hk.kod1 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(0), hk.kod1);
                hk.kod2 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(1), hk.kod2);
                hk.kod3 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(2), hk.kod3);
                hk.kod4 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(3), hk.kod4);

                hk.yil0 = alVeri.DblAl(0);
                hk.yil1 = alVeri.DblAl(1);
                hk.yil2 = alVeri.DblAl(2);
                hk.teklif0 = alVeri.DblAl(3);
                hk.teklif1 = alVeri.DblAl(4);
                hk.teklif2 = alVeri.DblAl(5);
                hk.ad4 = alVeri.StrAl(6);
                oa.Ekle(hk);
            }
            return oa;
        }

        internal static ObjectArray ListeleEkoSektorTavan(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string ekoKod, string sektorKod, string pebBirimKod, string oncekiYilSenaryo)
        {
            if (string.IsNullOrWhiteSpace(oncekiYilSenaryo))
                oncekiYilSenaryo = "0600";
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, pebBirimKod))
            {
                oa.sonuc.DegerAta(string.Format("Verdiğiniz birim kodunu {0} görme yetkiniz bulunmamaktadır!", pebBirimKod));
                return oa;
            }
            int ek = 0;
            if (!string.IsNullOrWhiteSpace(ekoKod) && !string.IsNullOrWhiteSpace(sektorKod))
                ek = 1;

            string[] sorgular = new string[9 + ek];
            string[] baglantilar = new string[9 + ek];
            sorgular[0] = TABBHMTAVANGENEL.Listele3Sql(yil, senaryoKod, 1, ekoKod, sektorKod, pebBirimKod);
            baglantilar[0] = "BaglantiSatiriBHMv2";

            sorgular[1] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Sql(yil - 2, "0600", 0, ekoKod, sektorKod, pebBirimKod);
            baglantilar[1] = "BaglantiSatiriBHMv2";

            sorgular[2] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Sql(yil - 1, oncekiYilSenaryo, 0, ekoKod, sektorKod, pebBirimKod);
            baglantilar[2] = "BaglantiSatiriBHMv2";

            sorgular[3] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Sql(yil - 1, oncekiYilSenaryo, 1, ekoKod, sektorKod, pebBirimKod);
            baglantilar[3] = "BaglantiSatiriBHMv2";
            sorgular[4] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Sql(yil - 1, oncekiYilSenaryo, 2, ekoKod, sektorKod, pebBirimKod);
            baglantilar[4] = "BaglantiSatiriBHMv2";

            sorgular[5] = TABBHMODENEKGIDER.ProjeHarcamaBulEko2SektorPeb2Sql(yil - 2, ekoKod, sektorKod, pebBirimKod);
            baglantilar[5] = "BaglantiSatiriBHMv2";
            sorgular[6] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Sql(yil, senaryoKod, 0, ekoKod, sektorKod, pebBirimKod);
            baglantilar[6] = "BaglantiSatiriBHMv2";
            sorgular[7] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Sql(yil, senaryoKod, 1, ekoKod, sektorKod, pebBirimKod);
            baglantilar[7] = "BaglantiSatiriBHMv2";
            sorgular[8] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Sql(yil, senaryoKod, 2, ekoKod, sektorKod, pebBirimKod);
            baglantilar[8] = "BaglantiSatiriBHMv2";

            if (ek == 1)
            {
                sorgular[9] = "SELECT " + VeriErisim.DegerYap(ekoKod.Replace(".", "")) + ", " + VeriErisim.DegerYap(sektorKod.Replace(".", "")) + ", PEBKOD, PEBAD from ABSPEBKOD where YIL=" + VeriErisim.DegerYap(yil) + " AND LEN(PEBKOD)=2";
                if (pebBirimKod != null)
                {
                    sorgular[9] += " AND (PEBKOD LIKE '" + pebBirimKod.Replace(".", "") + "%')";
                }

                baglantilar[9] = "BaglantiSatiriBHM";
            }

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2 });
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();
            Hashtable htSektor = new Hashtable();
            htSektor["00"] = "Cari";

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel hk = new TavanGenel();

                hk.kod1 = alAnahtar.StrAl(0);
                hk.kod2 = alAnahtar.StrAl(1);
                hk.kod3 = alAnahtar.StrAl(2);
                hk.kod2 = OrtakFonksiyonlar.KodNoktaKoy("99.99", hk.kod2);
                hk.yil0 = alVeri.DblAl(0);
                hk.yil1 = alVeri.DblAl(1);
                hk.yil2 = alVeri.DblAl(2);
                hk.odenek_2 = alVeri.DblAl(3);
                hk.odenek_1 = alVeri.DblAl(4);
                hk.odenek_1_1 = alVeri.DblAl(5);
                hk.odenek_1_2 = alVeri.DblAl(6);

                //hk.odenek_1_1 = hk.odenek_1 * 1.2;
                //hk.odenek_1_2 = hk.odenek_1 * 1.4;

                hk.harcama_2 = alVeri.DblAl(7);

                hk.teklif0 = alVeri.DblAl(8);
                hk.teklif1 = alVeri.DblAl(9);
                hk.teklif2 = alVeri.DblAl(10);

                hk.ad1 = BKDBirimButceHazirlik.EkoAdBul(kullanan, servis, htEko, yil, hk.kod1);
                hk.ad2 = BKDBirimButceHazirlik.SektorAdBul(kullanan, servis, htSektor, yil, hk.kod2);
                if (ek == 1)
                    hk.ad3 = alVeri.StrAl(11);
                if (string.IsNullOrWhiteSpace(hk.ad3))
                    hk.ad3 = BKDBirimButceHazirlik.PebAdBul(kullanan, servis, htPeb, yil, hk.kod3);

                oa.Ekle(hk);
            }
            return oa;
        }

        internal static ObjectArray ListeleEkoSektorFaaliyetTavanSGB(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string ekoKod, string sektorKod, string pebBirimKod, string pebFaalKod, bool tavanliKayitlar)
        {
            if (senaryoKod != null) senaryoKod = senaryoKod.Replace(".", "");
            string pebSorguKod = null;
            if (pebFaalKod != null)
            {
                pebSorguKod = pebFaalKod.Replace(".", "");
            }
            else if (pebBirimKod != null)
            {
                pebSorguKod = pebBirimKod.Replace(".", "");
            }

            ObjectArray oa = new ObjectArray();

            int ek = 0;
            if (!string.IsNullOrWhiteSpace(ekoKod) && !string.IsNullOrWhiteSpace(pebBirimKod) && !string.IsNullOrWhiteSpace(sektorKod))
                ek = 1;
            string[] sorgular = new string[6 + ek];
            string[] baglantilar = new string[6 + ek];

            sorgular[0] = TABBHMTAVANGENEL.Listele4Sql(yil, senaryoKod, 2, ekoKod, sektorKod, pebBirimKod, pebFaalKod, true);
            baglantilar[0] = "BaglantiSatiriBHMv2";
            sorgular[1] = TABBHMTAVANGENEL.Listele4Sql(yil, "0000", 2, ekoKod, sektorKod, pebBirimKod, pebFaalKod, true);
            baglantilar[1] = "BaglantiSatiriBHMv2";
            sorgular[2] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, 0, ekoKod, sektorKod, pebSorguKod);
            baglantilar[2] = "BaglantiSatiriBHMv2";
            sorgular[3] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, 1, ekoKod, sektorKod, pebSorguKod);
            baglantilar[3] = "BaglantiSatiriBHMv2";
            sorgular[4] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, 2, ekoKod, sektorKod, pebSorguKod);
            baglantilar[4] = "BaglantiSatiriBHMv2";
            sorgular[5] = TABBHMTAVANUSTU.ProjeTavanUstuBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, ekoKod, sektorKod, pebSorguKod);
            baglantilar[5] = "BaglantiSatiriBHMv2";

            if (ek == 1)
            {
                sorgular[6] = TABBHMODENEKGIDER.PebKodBilgiSql(ekoKod, sektorKod, pebBirimKod, yil, senaryoKod, sektorKod, pebSorguKod, 0, 0);
                baglantilar[6] = "BaglantiSatiriBHM";
            }

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3 });
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel hk = new TavanGenel();

                hk.kod1 = alAnahtar.StrAl(0);
                hk.kod2 = alAnahtar.StrAl(1);
                hk.kod3 = alAnahtar.StrAl(2);
                hk.kod4 = alAnahtar.StrAl(3);

                hk.kod2 = OrtakFonksiyonlar.KodNoktaKoy("99.99", hk.kod2);
                hk.kod4 = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", hk.kod4);

                hk.yil0 = alVeri.DblAl(0);
                hk.yil1 = alVeri.DblAl(1);
                hk.yil2 = alVeri.DblAl(2);
                hk.ekOdenek = alVeri.DblAl(3);
                hk.ekOdenek1 = alVeri.DblAl(4);
                hk.ekOdenek2 = alVeri.DblAl(5);


                hk.teklif0 = alVeri.DblAl(6);
                hk.teklif1 = alVeri.DblAl(7);
                hk.teklif2 = alVeri.DblAl(8);

                hk.ekOdenekTeklif = alVeri.DblAl(9);
                hk.ekOdenek1Teklif = alVeri.DblAl(10);
                hk.ekOdenek2Teklif = alVeri.DblAl(11);

                //2. sorgu
                hk.sgb0 = alVeri.DblAl(12);
                //3. sorgu
                hk.sgb1 = alVeri.DblAl(13);
                //4. sorgu
                hk.sgb2 = alVeri.DblAl(14);

                //5. sorgu
                hk.ekSgb = alVeri.DblAl(15);
                hk.ekSgb1 = alVeri.DblAl(16);
                hk.ekSgb2 = alVeri.DblAl(17);


                if (ek == 1)
                    hk.ad4 = alVeri.StrAl(18);

                hk.ad1 = BKDBirimButceHazirlik.EkoAdBul(kullanan, servis, htEko, yil, hk.kod1);
                if (string.IsNullOrWhiteSpace(hk.ad4))
                    hk.ad4 = BKDBirimButceHazirlik.PebAdBul(kullanan, servis, htPeb, yil, hk.kod4);
                if (!tavanliKayitlar || hk.yil0 > 0 || hk.yil1 > 0 || hk.yil2 > 0)
                    oa.Ekle(hk);
            }
            return oa;
        }

        internal static ObjectArray ListeleEkoSektorFaaliyetTavan(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string ekoKod, string sektorKod, string pebBirimKod, string pebFaalKod, bool tavanliKayitlar)
        {
            if (senaryoKod != null) senaryoKod = senaryoKod.Replace(".", "");
            string pebSorguKod = null;
            if (pebFaalKod != null)
            {
                pebSorguKod = pebFaalKod.Replace(".", "");
            }
            else if (pebBirimKod != null)
            {
                pebSorguKod = pebBirimKod.Replace(".", "");
            }

            ObjectArray oa = new ObjectArray();

            if (!GormeYetkisiVarMi(kullanan, pebSorguKod))
            {
                oa.sonuc.DegerAta(string.Format("Verdiğiniz faaliyet kodunu {0} görme yetkiniz bulunmamaktadır!", pebBirimKod));
                return oa;
            }
            if (senaryoKod == "0100")
                return ListeleEkoSektorFaaliyetTavanSGB(veriErisim, kullanan, yil, senaryoKod, ekoKod, sektorKod, pebBirimKod, pebFaalKod, tavanliKayitlar);

            int ek = 0;
            if (!string.IsNullOrWhiteSpace(ekoKod) && !string.IsNullOrWhiteSpace(pebBirimKod) && !string.IsNullOrWhiteSpace(sektorKod))
                ek = 1;
            string[] sorgular = new string[9 + ek];
            string[] baglantilar = new string[9 + ek];

            sorgular[0] = "SELECT KOD1, KOD2, KOD3, KOD4, YIL0, YIL1, YIL2 from BHMTAVANGENEL where YIL=" + VeriErisim.DegerYap(yil) + " AND ASAMA=" + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND TAVANTUR=2";

            if (ekoKod != null)
                sorgular[0] += " AND KOD1 = " + VeriErisim.DegerYap(ekoKod.Replace(".", ""));
            if (sektorKod != null)
                sorgular[0] += " AND KOD2 = " + VeriErisim.DegerYap(sektorKod.Replace(".", ""));
            if (pebBirimKod != null)
                sorgular[0] += " AND KOD3 = " + VeriErisim.DegerYap(pebBirimKod.Replace(".", ""));
            if (pebFaalKod != null)
                sorgular[0] += " AND KOD4 = " + VeriErisim.DegerYap(pebFaalKod.Replace(".", ""));
            baglantilar[0] = "BaglantiSatiriBHMv2";

            sorgular[1] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil - 2, "0600", 0, ekoKod, sektorKod, pebSorguKod);
            baglantilar[1] = "BaglantiSatiriBHMv2";
            sorgular[2] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil - 1, "0600", 0, ekoKod, sektorKod, pebSorguKod);
            baglantilar[2] = "BaglantiSatiriBHMv2";
            sorgular[3] = TABBHMODENEKGIDER.ProjeHarcamaBulEko2SektorPeb2Peb4Sql(yil - 2, ekoKod, sektorKod, pebSorguKod);
            baglantilar[3] = "BaglantiSatiriBHMv2";
            sorgular[4] = TABBKDBIRIMBUTCEHAZIRLIK.ProjeTaelpBulEko2SektorPeb2Peb4Sql(yil, ekoKod, sektorKod, pebSorguKod);
            baglantilar[4] = "BaglantiSatiriBHMv2";
            sorgular[5] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, 0, ekoKod, sektorKod, pebSorguKod);
            baglantilar[5] = "BaglantiSatiriBHMv2";
            sorgular[6] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, 1, ekoKod, sektorKod, pebSorguKod);
            baglantilar[6] = "BaglantiSatiriBHMv2";
            sorgular[7] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, 2, ekoKod, sektorKod, pebSorguKod);
            baglantilar[7] = "BaglantiSatiriBHMv2";
            sorgular[8] = TABBHMTAVANUSTU.ProjeTavanUstuBulEko2SektorPeb2Peb4Sql(yil, "0000", ekoKod, sektorKod, pebSorguKod);
            baglantilar[8] = "BaglantiSatiriBHMv2";

            if (ek == 1)
            {
                sorgular[9] = TABBHMODENEKGIDER.PebKodBilgiSql(ekoKod, sektorKod, pebBirimKod, yil, senaryoKod, sektorKod, pebSorguKod, 0, 0);
                baglantilar[9] = "BaglantiSatiriBHM";
            }

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3 });
            Hashtable htEko = new Hashtable();
            Hashtable htSektor = new Hashtable();
            Hashtable htPeb = new Hashtable();

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel hk = new TavanGenel();

                hk.kod1 = alAnahtar.StrAl(0);
                hk.kod2 = alAnahtar.StrAl(1);
                hk.kod3 = alAnahtar.StrAl(2);
                hk.kod4 = alAnahtar.StrAl(3);

                hk.kod2 = OrtakFonksiyonlar.KodNoktaKoy("99.99", hk.kod2);
                hk.kod4 = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", hk.kod4);

                hk.yil0 = alVeri.DblAl(0);
                hk.yil1 = alVeri.DblAl(1);
                hk.yil2 = alVeri.DblAl(2);
                hk.odenek_2 = alVeri.DblAl(3);
                hk.odenek_1 = alVeri.DblAl(4);
                hk.harcama_2 = alVeri.DblAl(5);
                hk.talep_0 = alVeri.DblAl(6);

                hk.teklif0 = alVeri.DblAl(7);
                hk.teklif1 = alVeri.DblAl(8);
                hk.teklif2 = alVeri.DblAl(9);

                hk.ekOdenek = alVeri.DblAl(10);
                hk.ekOdenek1 = alVeri.DblAl(11);
                hk.ekOdenek2 = alVeri.DblAl(12);

                if (ek == 1)
                    hk.ad4 = alVeri.StrAl(13);

                if (!tavanliKayitlar || hk.yil0 > 0 || hk.yil1 > 0 || hk.yil2 > 0)
                {
                    hk.ad1 = BKDBirimButceHazirlik.EkoAdBul(kullanan, servis, htEko, yil, hk.kod1);
                    hk.ad2 = BKDBirimButceHazirlik.SektorAdBul(kullanan, servis, htSektor, yil, hk.kod2);
                    hk.ad3 = BKDBirimButceHazirlik.PebAdBul(kullanan, servis, htPeb, yil, hk.kod3);
                    if (string.IsNullOrWhiteSpace(hk.ad4))
                        hk.ad4 = BKDBirimButceHazirlik.PebAdBul(kullanan, servis, htPeb, yil, hk.kod4);
                    oa.Ekle(hk);
                }
            }
            return oa;
        }

        internal static ObjectArray ListeleGorusmeCari(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, string senaryoKod, bool eko2)
        {
            int yil = tertip.yil;
            string kurKod = OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 0, "");
            string fonKod = OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 1, "");
            string finKod = OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 2, "");

            string ekoKod = OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 3, "");
            string pebFaalKod = OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 4, "");
            ObjectArray oa = new ObjectArray();

            if (!GormeYetkisiVarMi(kullanan, pebFaalKod))
            {
                oa.sonuc.DegerAta(string.Format("Verdiğiniz faaliyet kodunu {0} görme yetkiniz bulunmamaktadır!", pebFaalKod));
                return oa;
            }

            string[] sorgular = new string[13];
            string[] baglantilar = new string[13];

            string kurSorgu = "AND (KURKOD LIKE '" + kurKod.Replace(".", "") + "%')";
            string fonSorgu = "AND (FONKOD LIKE '" + fonKod.Replace(".", "") + "%')";
            string finSorgu = "AND (FINKOD LIKE '" + finKod.Replace(".", "") + "%')";
            string ekoSorgu = "AND (EKOKOD LIKE '" + ekoKod.Replace(".", "") + "%')";
            string pebSorgu = "AND (PEBKOD LIKE '" + pebFaalKod.Replace(".", "") + "%')";

            string kurSorguTav = "AND (KOD1 LIKE '" + kurKod.Replace(".", "") + "%')";
            string fonSorguTav = "AND (KOD2 LIKE '" + fonKod.Replace(".", "") + "%')";
            string finSorguTav = "AND (KOD3 LIKE '" + finKod.Replace(".", "") + "%')";
            string ekoSorguTav = "AND (KOD4 LIKE '" + ekoKod.Replace(".", "") + "%')";
            string pebSorguTav = "AND (KOD5 LIKE '" + pebFaalKod.Replace(".", "") + "%')";

            string senaryoOnayliSorgu = "AND SENARYOKOD='0600'";
            string senaryoTeklifSorgu = "AND SENARYOKOD='0000'";
            string senaryoGelenSorgu = "AND SENARYOKOD='" + senaryoKod.Replace(".", "") + "'";

            string alanlar = "";
            if (eko2)
            {
                int eko2Uz = TNS.BHMv2.Arac.Eko2UzBul(yil);

                alanlar = $"KURKOD, FONKOD, FINKOD, LEFT(EKOKOD,{eko2Uz}), PEBKOD";
            }
            else
            {
                alanlar = "KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD";
            }

            string tertipSorgu;
            if (eko2)
                tertipSorgu = "SELECT DISTINCT " + alanlar + ", '-' FROM BHMTERTIPLISTESIGIDER WHERE (YIL = " + VeriErisim.DegerYap(yil) + ") {0} {1} {2} {3} {4} order by 1,2,3,4,5";
            else
                tertipSorgu = "SELECT " + alanlar + ", AD FROM BHMTERTIPLISTESIGIDER WHERE (YIL = " + VeriErisim.DegerYap(yil) + ") {0} {1} {2} {3} {4} order by 1,2,3,4,5";

            sorgular[0] = string.Format(tertipSorgu, kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);

            string sql = "SELECT " + alanlar + ", SUM(ODENEK) from BHMODENEK where YIL={0} {1} AND TEKLIFYIL={2} {3} {4} {5} {6} {7} group by " + alanlar + " order by 1,2,3,4,5";
            sorgular[1] = String.Format(sql, (yil - 1).ToString(), senaryoOnayliSorgu, "0", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[2] = String.Format(sql, yil.ToString(), senaryoTeklifSorgu, "0", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[3] = String.Format(sql, yil.ToString(), senaryoTeklifSorgu, "1", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[4] = String.Format(sql, yil.ToString(), senaryoTeklifSorgu, "2", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[5] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "0", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[6] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "1", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[7] = String.Format(sql, yil.ToString(), senaryoGelenSorgu, "2", kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);

            string harcamaSorgu = "SELECT " + alanlar + ", SUM(HARCAMA) FROM ABSHARCAMA  WHERE (YIL = {0}) {1} {2} {3} {4} {5} group by " + alanlar + " order by 1,2,3,4,5";
            sorgular[8] = string.Format(harcamaSorgu, yil - 2, kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);

            string birimOdenekSorgu = "SELECT " + alanlar + ", SUM(ODENEK) FROM BHMBIRIMBUTCEHAZIRLIK WHERE (YIL = {0}) {1} {2} {3} {4} {5} {6} group by " + alanlar + " order by 1,2,3,4,5";
            sorgular[9] = string.Format(birimOdenekSorgu, yil, kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu, senaryoTeklifSorgu);

            string tavanUstuSorgu = "";

            if (eko2)
                tavanUstuSorgu = "SELECT " + alanlar + ", SUM(ODENEK) FROM BHMTAVANUSTU WHERE (YIL = {0}) {1} {2} {3} {4} {5} {6} group by " + alanlar + " order by 1,2,3,4,5";
            else
                tavanUstuSorgu = "SELECT " + alanlar + ", ODENEK, GEREKCE FROM BHMTAVANUSTU WHERE (YIL = {0}) {1} {2} {3} {4} {5} {6} order by 1,2,3,4,5";
            sorgular[10] = string.Format(tavanUstuSorgu, yil, senaryoTeklifSorgu, kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);
            sorgular[11] = string.Format(tavanUstuSorgu, yil, senaryoGelenSorgu, kurSorgu, fonSorgu, finSorgu, ekoSorgu, pebSorgu);

            sorgular[12] = "SELECT KOD1, KOD2, KOD3, KOD4, KOD5, YIL0, YIL1, YIL2, EK0 from BHMTAVANGENEL where YIL=" + VeriErisim.DegerYap(yil) + " AND ASAMA=" + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND TAVANTUR=" + ((int)ENUMTavanTur.tavanTurGorusmeCari).ToString() + " " + kurSorguTav + " " + fonSorguTav + " " + finSorguTav + " " + ekoSorguTav + " " + pebSorguTav;

            for (int i = 0; i < 13; i++)
            {
                baglantilar[i] = "BaglantiSatiriBHMv2";
            }

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4 });
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel hk = new TavanGenel();

                hk.kod1 = alAnahtar.StrAl(0);
                hk.kod2 = alAnahtar.StrAl(1);
                hk.kod3 = alAnahtar.StrAl(2);
                hk.kod4 = alAnahtar.StrAl(3);
                hk.kod5 = alAnahtar.StrAl(4);

                hk.kod1 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(0), hk.kod1);
                hk.kod2 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(1), hk.kod2);
                hk.kod3 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(2), hk.kod3);
                hk.kod4 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(3), hk.kod4);
                hk.kod5 = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(4), hk.kod5);

                int kol = 0;
                hk.ad4 = alVeri.StrAl(kol++);
                hk.odenek_1 = alVeri.DblAl(kol++);
                hk.teklif0 = alVeri.DblAl(kol++);
                hk.teklif1 = alVeri.DblAl(kol++);
                hk.teklif2 = alVeri.DblAl(kol++);


                hk.sgb0 = alVeri.DblAl(kol++);
                hk.sgb1 = alVeri.DblAl(kol++);
                hk.sgb2 = alVeri.DblAl(kol++);

                hk.harcama_2 = alVeri.DblAl(kol++);
                hk.talep_0 = alVeri.DblAl(kol++);

                hk.ekOdenekTeklif = alVeri.DblAl(kol++);
                if (!eko2)
                {
                    hk.ekGerekceTeklif = alVeri.StrAl(kol++);
                }
                else
                {
                    hk.ekGerekceTeklif = TavanUstuGerekceBul(kullanan, veriErisim, yil, hk, "0000", true);
                }

                hk.ekSgb = alVeri.DblAl(kol++);
                if (!eko2)
                {
                    hk.ekGerekce = alVeri.StrAl(kol++);
                }
                else
                {
                    hk.ekGerekce = TavanUstuGerekceBul(kullanan, veriErisim, yil, hk, senaryoKod, true);
                }

                hk.yil0 = alVeri.DblAl(kol++);
                hk.yil1 = alVeri.DblAl(kol++);
                hk.yil2 = alVeri.DblAl(kol++);
                hk.ekOdenek = alVeri.DblAl(kol++);

                if (hk.ad4 == "-")
                {
                    string ekoAd = BKDBirimButceHazirlik.EkoAdBul(kullanan, servis, htEko, yil, hk.kod4);
                    string pebAd = BKDBirimButceHazirlik.PebAdBul(kullanan, servis, htPeb, yil, hk.kod5);
                    hk.ad4 = ekoAd + "(" + pebAd + ")";
                }
                oa.Ekle(hk);
            }
            oa.objeler.Sort();
            return oa;
        }

        private static string TavanUstuGerekceBul(Kullanici kullanan, VeriErisim veriErisim, int yil, TavanGenel hk, string senaryoKod, bool butceFormdan)
        {
            string[] sKodlar = new string[] { senaryoKod };
            KistasYapi kistas = new KistasYapi();
            kistas.ekleTertipler = new Tertip[1];
            kistas.ekleTertipler[0] = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, hk.kod1, hk.kod2, hk.kod3, hk.kod4, hk.kod5);
            TavanUstuTertipOdenek[] dizi = TavanUstuTertipOdenekHizmet.Listele(veriErisim, kullanan, sKodlar, kistas, true, butceFormdan);
            string donDeger = "";
            if (dizi == null) return donDeger;
            for (int i = 0; i < dizi.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(dizi[i].gerekce))
                {
                    if (donDeger != "") donDeger += "<br>";
                    donDeger += dizi[i].gerekce;
                }
            }
            return donDeger;
        }

        internal static ObjectArray ListeleYatirimProjeTavan(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string ekoKod, string sektorKod, string pebBirimKod, string pebFaalKod)
        {
            if (sektorKod != null) sektorKod = sektorKod.Trim();
            string pebSorguKod = null;
            if (pebFaalKod != null)
            {
                pebSorguKod = pebFaalKod.Replace(".", "");
            }
            else if (pebBirimKod != null)
            {
                pebSorguKod = pebBirimKod.Replace(".", "");
            }

            ObjectArray oa = new ObjectArray();

            if (!GormeYetkisiVarMi(kullanan, pebSorguKod))
            {
                oa.sonuc.DegerAta(string.Format("Verdiğiniz faaliyet kodunu {0} görme yetkiniz bulunmamaktadır!", pebSorguKod));
                return oa;
            }

            if (!string.IsNullOrWhiteSpace(senaryoKod))
                senaryoKod = senaryoKod.Replace(".", "");
            int ek = 0;
            if (!string.IsNullOrWhiteSpace(ekoKod) && !string.IsNullOrWhiteSpace(pebBirimKod) && !string.IsNullOrWhiteSpace(sektorKod))
                ek = 1;
            string[] sorgular = new string[12 + ek];
            string[] baglantilar = new string[12 + ek];

            sorgular[0] = TABBHMTAVANGENEL.Listele4Sql(yil, senaryoKod, 2, ekoKod, sektorKod, pebBirimKod, pebFaalKod, true);
            baglantilar[0] = "BaglantiSatiriBHMv2";

            sorgular[1] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil - 1, "0600", 0, ekoKod, sektorKod, pebSorguKod);
            baglantilar[1] = "BaglantiSatiriBHMv2";

            sorgular[2] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, "0000", 0, ekoKod, sektorKod, pebSorguKod);
            baglantilar[2] = "BaglantiSatiriBHMv2";

            sorgular[3] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, "0000", 1, ekoKod, sektorKod, pebSorguKod);
            baglantilar[3] = "BaglantiSatiriBHMv2";

            sorgular[4] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, "0000", 2, ekoKod, sektorKod, pebSorguKod);
            baglantilar[4] = "BaglantiSatiriBHMv2";

            sorgular[5] = TABBHMODENEKGIDER.ProjeHarcamaBulEko2SektorPeb2Peb4Sql(yil - 2, ekoKod, sektorKod, pebSorguKod);
            baglantilar[5] = "BaglantiSatiriBHMv2";

            sorgular[6] = TABBHMTAVANUSTU.ProjeTavanUstuBulEko2SektorPeb2Peb4Sql(yil, "0000", ekoKod, sektorKod, pebSorguKod);
            baglantilar[6] = "BaglantiSatiriBHMv2";

            sorgular[7] = TABBKDBIRIMBUTCEHAZIRLIK.ProjeTaelpBulEko2SektorPeb2Peb4Sql(yil, ekoKod, sektorKod, pebSorguKod);
            baglantilar[7] = "BaglantiSatiriBHMv2";

            sorgular[8] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, 0, ekoKod, sektorKod, pebSorguKod);
            baglantilar[8] = "BaglantiSatiriBHMv2";

            sorgular[9] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, 1, ekoKod, sektorKod, pebSorguKod);
            baglantilar[9] = "BaglantiSatiriBHMv2";

            sorgular[10] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, 2, ekoKod, sektorKod, pebSorguKod);
            baglantilar[10] = "BaglantiSatiriBHMv2";

            sorgular[11] = TABBHMTAVANUSTU.ProjeTavanUstuBulEko2SektorPeb2Peb4Sql(yil, senaryoKod, ekoKod, sektorKod, pebSorguKod);
            baglantilar[11] = "BaglantiSatiriBHMv2";

            if (ek == 1)
            {
                sorgular[12] = TABBHMODENEKGIDER.PebKodBilgiSql(ekoKod, sektorKod, pebBirimKod, yil, senaryoKod, sektorKod, pebSorguKod, 0, 1);
                baglantilar[12] = "BaglantiSatiriBHM";
            }
            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3 });
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel hk = new TavanGenel();

                hk.kod1 = alAnahtar.StrAl(0);
                hk.kod2 = alAnahtar.StrAl(1);
                hk.kod3 = alAnahtar.StrAl(2);
                hk.kod4 = alAnahtar.StrAl(3);

                hk.kod2 = OrtakFonksiyonlar.KodNoktaKoy("99.99", hk.kod2);
                hk.kod4 = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", hk.kod4);

                //0. Sorgu
                hk.yil0 = alVeri.DblAl(0);
                hk.yil1 = alVeri.DblAl(1);
                hk.yil2 = alVeri.DblAl(2);
                hk.ekOdenek = alVeri.DblAl(3);
                hk.ekOdenek1 = alVeri.DblAl(4);
                hk.ekOdenek2 = alVeri.DblAl(5);

                //1. sorgu
                hk.odenek_1 = alVeri.DblAl(6);
                //2. sorgu
                hk.teklif0 = alVeri.DblAl(7);
                //3. sorgu
                hk.teklif1 = alVeri.DblAl(8);
                //4. sorgu
                hk.teklif2 = alVeri.DblAl(9);
                //5. sorgu
                hk.harcama_2 = alVeri.DblAl(10);

                //6. sorgu
                hk.ekOdenekTeklif = alVeri.DblAl(11);
                hk.ekOdenek1Teklif = alVeri.DblAl(12);
                hk.ekOdenek2Teklif = alVeri.DblAl(13);

                //7. sorgu
                hk.talep_0 = alVeri.DblAl(14);

                //8. sorgu
                hk.sgb0 = alVeri.DblAl(15);
                //9. sorgu
                hk.sgb1 = alVeri.DblAl(16);
                //10. sorgu
                hk.sgb2 = alVeri.DblAl(17);

                //11. sorgu
                hk.ekSgb = alVeri.DblAl(18);
                hk.ekSgb1 = alVeri.DblAl(19);
                hk.ekSgb2 = alVeri.DblAl(20);

                if (ek == 1)
                {
                    hk.ad4 = alVeri.StrAl(21);
                    hk.kod5 = alVeri.StrAl(24);
                    hk.ad5 = alVeri.StrAl(22);
                    hk.projeBitisYili = alVeri.IntAl(23);
                    hk.projeDevYeni = hk.projeBitisYili == yil ? "Yeni" : "Devam";
                }
                hk.ad1 = BKDBirimButceHazirlik.EkoAdBul(kullanan, servis, htEko, yil, hk.kod1);
                if (string.IsNullOrWhiteSpace(hk.ad4))
                    hk.ad4 = BKDBirimButceHazirlik.PebAdBul(kullanan, servis, htPeb, yil, hk.kod4);

                oa.Ekle(hk);
            }
            oa.objeler.Sort(new TavanGenel());
            return oa;
        }

        internal static ObjectArray ListeleYatirimVizeTavan(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string pebBirimKod, string ekoKod, string finKod, string pebFaalKod, int projeKod)
        {
            string pebSorguKod = null;
            if (pebFaalKod != null)
            {
                pebSorguKod = pebFaalKod.Replace(".", "");
            }
            else if (pebBirimKod != null)
            {
                pebSorguKod = pebBirimKod.Replace(".", "");
            }

            ObjectArray oa = new ObjectArray();

            if (!GormeYetkisiVarMi(kullanan, pebSorguKod))
            {
                oa.sonuc.DegerAta(string.Format("Verdiğiniz faaliyet kodunu {0} görme yetkiniz bulunmamaktadır!", pebSorguKod));
                return oa;
            }

            if (!string.IsNullOrWhiteSpace(senaryoKod))
                senaryoKod = senaryoKod.Replace(".", "");
            int ek = 0;
            if (!string.IsNullOrWhiteSpace(ekoKod) && !string.IsNullOrWhiteSpace(pebBirimKod) && !string.IsNullOrWhiteSpace(finKod))
                ek = 1;
            string[] sorgular = new string[9 + ek];
            string[] baglantilar = new string[9 + ek];
            sorgular[0] = TABBHMTAVANGENEL.Listele4Sql(yil, senaryoKod, 4, pebBirimKod, ekoKod, finKod, pebFaalKod, false);
            baglantilar[0] = "BaglantiSatiriBHMv2";

            sorgular[1] = TABBHMODENEKGIDER.ProjeOdenekBulPeb2Eko2FinPeb4Sql(yil - 1, "0600", 0, ekoKod, finKod, pebSorguKod, projeKod);
            baglantilar[1] = "BaglantiSatiriBHMv2";

            sorgular[2] = TABBHMODENEKGIDER.ProjeOdenekBulPeb2Eko2FinPeb4Sql(yil, "0200", 0, ekoKod, finKod, pebSorguKod, projeKod);
            baglantilar[2] = "BaglantiSatiriBHMv2";

            sorgular[3] = TABBHMODENEKGIDER.ProjeOdenekBulPeb2Eko2FinPeb4Sql(yil, "0200", 1, ekoKod, finKod, pebSorguKod, projeKod);
            baglantilar[3] = "BaglantiSatiriBHMv2";

            sorgular[4] = TABBHMODENEKGIDER.ProjeOdenekBulPeb2Eko2FinPeb4Sql(yil, "0200", 2, ekoKod, finKod, pebSorguKod, projeKod);
            baglantilar[4] = "BaglantiSatiriBHMv2";

            sorgular[5] = TABBHMODENEKGIDER.ProjeHarcamaBulPeb2Eko2FinPeb4Sql(yil - 2, ekoKod, finKod, pebSorguKod, projeKod);
            baglantilar[5] = "BaglantiSatiriBHMv2";

            sorgular[6] = TABBHMODENEKGIDER.ProjeOdenekBulPeb2Eko2FinPeb4Sql(yil, senaryoKod, 0, ekoKod, finKod, pebSorguKod, projeKod);
            baglantilar[6] = "BaglantiSatiriBHMv2";

            sorgular[7] = TABBHMODENEKGIDER.ProjeOdenekBulPeb2Eko2FinPeb4Sql(yil, senaryoKod, 1, ekoKod, finKod, pebSorguKod, projeKod);
            baglantilar[7] = "BaglantiSatiriBHMv2";

            sorgular[8] = TABBHMODENEKGIDER.ProjeOdenekBulPeb2Eko2FinPeb4Sql(yil, senaryoKod, 2, ekoKod, finKod, pebSorguKod, projeKod);
            baglantilar[8] = "BaglantiSatiriBHMv2";

            if (ek == 1)
            {
                sorgular[9] = TABBHMODENEKGIDER.PebKodBilgiSql(pebBirimKod, ekoKod, finKod, yil, senaryoKod, null, pebSorguKod, projeKod, 1);
                baglantilar[9] = "BaglantiSatiriBHM";
            }
            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3 });
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel hk = new TavanGenel();

                hk.kod1 = alAnahtar.StrAl(0);
                hk.kod2 = alAnahtar.StrAl(1);
                hk.kod3 = alAnahtar.StrAl(2);
                hk.kod4 = alAnahtar.StrAl(3);
                hk.kod4 = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", hk.kod4);

                //0. Sorgu
                hk.yil0 = alVeri.DblAl(0);
                hk.yil1 = alVeri.DblAl(1);
                hk.yil2 = alVeri.DblAl(2);

                //1. sorgu
                hk.odenek_1 = alVeri.DblAl(3);
                //2. sorgu
                hk.teklif0 = alVeri.DblAl(4);
                //3. sorgu
                hk.teklif1 = alVeri.DblAl(5);
                //4. sorgu
                hk.teklif2 = alVeri.DblAl(6);
                //5. sorgu
                hk.harcama_2 = alVeri.DblAl(7);

                //6. sorgu
                hk.sgb0 = alVeri.DblAl(8);
                //7. sorgu
                hk.sgb1 = alVeri.DblAl(9);
                //8. sorgu
                hk.sgb2 = alVeri.DblAl(10);

                if (ek == 1)
                {
                    hk.ad4 = alVeri.StrAl(11);
                    hk.ad5 = alVeri.StrAl(12);
                    hk.projeBitisYili = alVeri.IntAl(13);
                    hk.kod5 = alVeri.StrAl(14);
                    hk.kod6 = alVeri.StrAl(15);
                    hk.projeDevYeni = hk.projeBitisYili == yil ? "Yeni" : "Devam";
                }
                hk.ad1 = BKDBirimButceHazirlik.EkoAdBul(kullanan, servis, htEko, yil, hk.kod1);
                if (string.IsNullOrWhiteSpace(hk.ad4))
                    hk.ad4 = BKDBirimButceHazirlik.PebAdBul(kullanan, servis, htPeb, yil, hk.kod4);
                if (!string.IsNullOrEmpty(hk.ad5))
                    oa.Ekle(hk);
            }
            oa.objeler.Sort(new TavanGenel());
            return oa;
        }

        internal static ObjectArray ListeleYatirimVizeTavanRapor(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, bool detay, string pebKod, string ekoKod, string finKod)
        {
            ObjectArray oa = new ObjectArray();

            if (!string.IsNullOrWhiteSpace(senaryoKod))
                senaryoKod = senaryoKod.Replace(".", "");

            string[] sorgular = new string[4];
            string[] baglantilar = new string[4];

            int[] birlestirAlan;
            if (detay)
                birlestirAlan = new int[] { 0, 1, 2, 3, 4 };
            else
                birlestirAlan = new int[] { 0, 1, 2 };

            sorgular[0] = TABBHMTAVANGENEL.ListeleRapor(detay, yil, senaryoKod, 4, pebKod, ekoKod, finKod);
            baglantilar[0] = "BaglantiSatiriBHMv2";

            sorgular[1] = TABBHMODENEKGIDER.ProjeOdenekRaporSql(detay, yil, senaryoKod, 0, ekoKod, finKod, pebKod);
            baglantilar[1] = "BaglantiSatiriBHMv2";
            sorgular[2] = TABBHMODENEKGIDER.ProjeOdenekRaporSql(detay, yil, senaryoKod, 1, ekoKod, finKod, pebKod);
            baglantilar[2] = "BaglantiSatiriBHMv2";
            sorgular[3] = TABBHMODENEKGIDER.ProjeOdenekRaporSql(detay, yil, senaryoKod, 2, ekoKod, finKod, pebKod);
            baglantilar[3] = "BaglantiSatiriBHMv2";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, birlestirAlan);
            Hashtable htEko = new Hashtable();
            Hashtable htFin = new Hashtable();

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel hk = new TavanGenel();

                int alanNo = 0;
                hk.kod1 = OrtakFonksiyonlar.KodNoktaKoy("99.99", alAnahtar.StrAl(alanNo++));
                if (detay)
                {
                    hk.kod2 = alAnahtar.StrAl(alanNo++);
                    hk.kod3 = alAnahtar.StrAl(alanNo++);

                    hk.ad2 = BKDBirimButceHazirlik.EkoAdBul(kullanan, servis, htEko, yil, hk.kod2);
                    hk.ad3 = BKDBirimButceHazirlik.FinAdBul(kullanan, servis, htFin, yil, hk.kod3);
                }
                else
                {
                    hk.kod2 = "";
                    hk.kod3 = "";
                }
                hk.kod4 = alAnahtar.StrAl(alanNo++);
                hk.kod5 = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", alAnahtar.StrAl(alanNo++));

                hk.ad1 = alVeri.StrAl(0);
                hk.ad4 = alVeri.StrAl(1);
                hk.ad5 = alVeri.StrAl(2);

                hk.yil0 = alVeri.DblAl(3);
                hk.yil1 = alVeri.DblAl(4);
                hk.yil2 = alVeri.DblAl(5);

                hk.teklif0 = alVeri.DblAl(9);
                hk.teklif1 = alVeri.DblAl(13);
                hk.teklif2 = alVeri.DblAl(17);

                oa.Ekle(hk);
            }
            oa.objeler.Sort(new TavanGenel(false));
            return oa;
        }

        internal static ObjectArray SektorKullanilmisMi(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string sektorKod)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));

            TABABSPEBKOD pebTab = new TABABSPEBKOD(ve);

            IDataReader dr = pebTab.SektordeKullanilanlariBul(yil, senaryoKod, sektorKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    ABSKod kod = new ABSKod();
                    kod.kod = VeriErisim.DegerAl(dr, 0, "");
                    kod.AdAta(VeriErisim.DegerAl(dr, 1, ""));
                    oa.Ekle(kod);
                }
                dr.Close();
            }
            else
                oa.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc TavanUretEkoSektorBirim(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string kaynakSenaryoKod)
        {
            Sonuc sonuc = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                sonuc.DegerAta("Bu işlemi yapmaya yetkiniz bulunmmaktadır!");
                return sonuc;
            }
            string[] sorgular = new string[2];
            string[] baglantilar = new string[2];
            sorgular[0] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Sql(yil - 1, kaynakSenaryoKod, 1, null, null, null);
            baglantilar[0] = "BaglantiSatiriBHMv2";
            sorgular[1] = TABBHMODENEKGIDER.ProjeOdenekBulEko2SektorPeb2Sql(yil - 1, kaynakSenaryoKod, 2, null, null, null);
            baglantilar[1] = "BaglantiSatiriBHMv2";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2 });

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHMv2", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABBHMTAVANGENEL tab = new TABBHMTAVANGENEL(ve);
            TavanGenel form = new TavanGenel();
            sonuc = tab.Sil(yil, senaryoKod, ENUMTavanTur.tavanTurEkoSektorBirim, null);
            foreach (DictionaryEntry de in ht)
            {
                if (!sonuc.islemSonuc) break;
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                TavanGenel tg = new TavanGenel();

                tg.kod1 = alAnahtar.StrAl(0);
                tg.kod2 = alAnahtar.StrAl(1);
                tg.kod3 = alAnahtar.StrAl(2);
                tg.kod2 = OrtakFonksiyonlar.KodNoktaKoy("99.99", tg.kod2);

                tg.yil0 = Yuvarla(alVeri.DblAl(0), 1000);
                tg.yil1 = Yuvarla(alVeri.DblAl(1), 1000);
                tg.yil2 = tg.yil1;
                if (tg.yil0 > 0 || tg.yil1 > 0)
                    sonuc = tab.Ekle(yil, senaryoKod, ENUMTavanTur.tavanTurEkoSektorBirim, tg);
            }
            if (sonuc.islemSonuc)
                sonuc = tab.EkleTarihce(yil, senaryoKod, ENUMTavanTur.tavanTurEkoSektorBirim, form, ENUMTarihceKod.KAYDEDILDI, kullanan.kullaniciKodu);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static double Yuvarla(double rakam, int bolum)
        {
            return Math.Floor(rakam / bolum) * bolum;
        }
    }
}

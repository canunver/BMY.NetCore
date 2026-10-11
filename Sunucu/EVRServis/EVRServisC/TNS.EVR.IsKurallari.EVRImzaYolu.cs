using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System.Collections;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRImzaYolu
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ImzaYolu form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.ad))
                hata += "İmza yolu adı boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABImzaYolu tab = new TABImzaYolu(ve);

            ve.BeginTransaction();
            try
            {
                if (form.kod == 0) { sonuc = tab.Kaydet(form); } else { sonuc = tab.Guncelle(form); }

                if (sonuc.islemSonuc && form.kod > 0)
                {
                    sonuc = tab.SilDetay(form.kod);
                    if (sonuc.islemSonuc && form.detaylar != null && form.detaylar.Length > 0)
                    {
                        foreach (ImzaYoluDetay detay in form.detaylar)
                        {
                            detay.imzaYoluKod = form.kod;

                            sonuc = tab.Kaydet(detay);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }
                }

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.anahtar = form.kod.ToString();
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{F5A4CEA5-EB17-4056-AABF-53FBC9917DD7} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ImzaYolu form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (form.kod <= 0)
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABImzaYolu tab = new TABImzaYolu(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(form);
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
                OrtakFonksiyonlar.HataStrYaz("{42E8807C-5986-4487-BF37-A718DC858789} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ImzaYolu[] Listele(VeriErisim veriErisim, Kullanici kullanan, ImzaYolu form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABImzaYolu tab = new TABImzaYolu(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new ImzaYolu();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlar.KOD, 0);
                    form.ad = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlar.AD, "");
                    form.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlar.TCKIMLIKNO, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            ImzaYolu[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (ImzaYolu[])liste.objeler.ToArray(typeof(ImzaYolu));

            return arr;
        }
        private static ImzaYoluDetay[] Listele(VeriErisim veriErisim, Kullanici kullanan, int imzaYoluKod)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABImzaYolu tab = new TABImzaYolu(ve);

            IDataReader dr = tab.ListeleDetay(imzaYoluKod);
            if (dr != null)
            {
                int index = (int)TABImzaYolu.ENUMAlanlarDetay.AKADEMIKUNVAN;
                while (dr.Read())
                {
                    ImzaYoluDetay detay = new ImzaYoluDetay();
                    detay.imzaYoluKod = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlarDetay.IMZAYOLUKOD, 0);
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlarDetay.SIRANO, 0);
                    detay.islemTip = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlarDetay.ISLEMTIP, 0);
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlarDetay.TCKIMLIKNO, "");
                    detay.ad = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlarDetay.AD, "");
                    detay.soyad = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlarDetay.SOYAD, "");
                    detay.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlarDetay.EVRAKBIRIMKOD, "");
                    detay.unvan = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlarDetay.UNVAN, "");
                    detay.akademikUnvan = VeriErisim.DegerAl(dr, (int)TABImzaYolu.ENUMAlanlarDetay.AKADEMIKUNVAN, "");
                    detay.evrakBirimAd = VeriErisim.DegerAl(dr, index + 1, "");

                    liste.objeler.Add(detay);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            ImzaYoluDetay[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (ImzaYoluDetay[])liste.objeler.ToArray(typeof(ImzaYoluDetay));

            return arr;
        }
        public static ImzaYolu Oku(VeriErisim veriErisim, Kullanici kullanan, ImzaYolu form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve1 = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            ImzaYolu[] list = EVRImzaYolu.Listele(ve1, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            ImzaYoluDetay[] listDetay = Listele(ve2, kullanan, form.kod);

            if (listDetay != null && listDetay.Length > 0)
                form.detaylar = listDetay;

            ve1.VeriErisimKapat();
            ve2.VeriErisimKapat();

            return form;
        }

        public static EvrakImzaBilgi EvrakImzaBilgiOkuEski(VeriErisim ve, Kullanici kullanan, int imzaYoluKod)
        {
            EvrakImzaBilgi bilgi = new EvrakImzaBilgi();
            if (imzaYoluKod == 0) return bilgi;

            Hashtable imzaTcKimlikNoList = new Hashtable();

            ImzaYolu imzaYolu = EVRImzaYolu.Oku(ve, kullanan, new ImzaYolu() { kod = imzaYoluKod });
            if (imzaYolu != null && imzaYolu.detaylar != null && imzaYolu.detaylar.Length > 0)
            {

                //İmza
                int imzaIndex = 0;
                for (int i = 0; i < imzaYolu.detaylar.Length; i++)
                {
                    ImzaYoluDetay detay = imzaYolu.detaylar[i];
                    if (detay.islemTip == (int)ENUMImzaYoluTip.IMZA)
                    {
                        imzaIndex++;
                        string imza = "";
                        string imzaIsim = "";
                        string imzaUnvan = "";
                        Vekalet vekalet = EVRVekalet.AktifVekaletOku(ve, kullanan, detay.tcKimlikNo);
                        if (vekalet != null && vekalet.aktif > 0)
                        {
                            imzaIsim = AdSoyadBuyukKucukHarfDuzenle(vekalet.kisi2Ad);
                            imza = imzaIsim + "\r\n" + (string.IsNullOrEmpty(detay.unvan) ? "... V." : detay.unvan + " V.");
                            imzaUnvan = (string.IsNullOrEmpty(detay.unvan) ? "... V." : detay.unvan + " V.");

                            if (!imzaTcKimlikNoList.Contains(detay.tcKimlikNo))
                                imzaTcKimlikNoList.Add(detay.tcKimlikNo, OrtakFonksiyonlar.IlkHarflerBuyuk(detay.ad) + " " + detay.soyad);
                        }
                        else
                        {
                            imzaIsim = (string.IsNullOrEmpty(detay.akademikUnvan) ? "" : detay.akademikUnvan + " ") + OrtakFonksiyonlar.IlkHarflerBuyuk(detay.ad) + " " + detay.soyad;
                            imza = imzaIsim + "\r\n" + (string.IsNullOrEmpty(detay.unvan) ? "" : detay.unvan);
                            imzaUnvan = (string.IsNullOrEmpty(detay.unvan) ? "" : detay.unvan);
                        }

                        if (imzaIndex == 1) { bilgi.imza1 = imza; bilgi.imza1Isim = imzaIsim; bilgi.imza1Unvan = imzaUnvan; }
                        else if (imzaIndex == 2) { bilgi.imza2 = imza; bilgi.imza2Isim = imzaIsim; bilgi.imza2Unvan = imzaUnvan; }
                        else if (imzaIndex == 3) { bilgi.imza3 = imza; bilgi.imza3Isim = imzaIsim; bilgi.imza3Unvan = imzaUnvan; }
                    }
                }


                //Paraf listesinde kendisini yoksa ekle
                ArrayList parafList = new ArrayList();
                bool kendisiParafListesindeVarMi = false;
                foreach (ImzaYoluDetay d in imzaYolu.detaylar)
                    if (d.tcKimlikNo == kullanan.mernis) { kendisiParafListesindeVarMi = true; break; }

                if (!kendisiParafListesindeVarMi)
                    parafList.Add(new ImzaYoluDetay() { tcKimlikNo = kullanan.mernis, siraNo = 1, islemTip = (int)ENUMImzaYoluTip.PARAF, unvan = kullanan.unvan, ad = kullanan.ad, soyad = kullanan.soyad });

                foreach (var detay in imzaYolu.detaylar)
                {
                    if (detay.islemTip == (int)ENUMImzaYoluTip.IMZA) continue;
                    parafList.Add(detay);
                }
                //********************************************

                //Paraf
                foreach (ImzaYoluDetay detay in parafList)
                {
                    if (!imzaTcKimlikNoList.Contains(detay.tcKimlikNo) && !string.IsNullOrEmpty(detay.unvan))
                    {
                        string ad = !string.IsNullOrEmpty(detay.akademikUnvan) ? detay.akademikUnvan + " " : "";

                        string[] adlar = detay.ad.Replace(" ", ".").Split('.');
                        foreach (var item in adlar)
                        {
                            if (string.IsNullOrEmpty(item)) continue;
                            if (item.ToLower() == "prof" || item.ToLower() == "dr")
                                ad += item + ".";
                            else
                                ad += OrtakFonksiyonlar.IlkHarflerBuyuk(item.Substring(0, 1)) + ".";
                        }
                        bilgi.paraf += string.Format("..../..../" + System.DateTime.Now.Year + "\t {0} : {1}{2}\r\n", UnvanKisaAdGetir(detay.unvan), ad, detay.soyad);
                    }
                }
                bilgi.paraf = bilgi.paraf.TrimEnd("\r\n".ToCharArray());
            }

            return bilgi;
        }

        public static EvrakImzaBilgi EvrakImzaBilgiOku2(VeriErisim ve, Kullanici kullanan, int imzaYoluKod)
        {
            EvrakImzaBilgi bilgi = new EvrakImzaBilgi();
            if (imzaYoluKod == 0) return bilgi;

            Hashtable imzaTcKimlikNoList = new Hashtable();

            ImzaYolu imzaYolu = EVRImzaYolu.Oku(ve, kullanan, new ImzaYolu() { kod = imzaYoluKod });
            if (imzaYolu != null && imzaYolu.detaylar != null && imzaYolu.detaylar.Length > 0)
            {

                //İmza
                int imzaIndex = 0;
                for (int i = 0; i < imzaYolu.detaylar.Length; i++)
                {
                    ImzaYoluDetay detay = imzaYolu.detaylar[i];
                    if (detay.islemTip == (int)ENUMImzaYoluTip.IMZA)
                    {
                        imzaIndex++;
                        string imza = "";
                        string imzaIsim = "";
                        string imzaUnvan = "";
                        Vekalet vekalet = EVRVekalet.AktifVekaletOku(ve, kullanan, detay.tcKimlikNo);
                        if (vekalet != null && vekalet.aktif > 0)
                        {
                            imzaIsim = AdSoyadBuyukKucukHarfDuzenle(vekalet.kisi2Ad);
                            imza = imzaIsim + "\r\n" + (string.IsNullOrEmpty(detay.unvan) ? "... V." : detay.unvan + " V.");
                            imzaUnvan = (string.IsNullOrEmpty(detay.unvan) ? "... V." : detay.unvan + " V.");

                            if (!imzaTcKimlikNoList.Contains(detay.tcKimlikNo))
                                imzaTcKimlikNoList.Add(detay.tcKimlikNo, OrtakFonksiyonlar.IlkHarflerBuyuk(detay.ad) + " " + detay.soyad);
                        }
                        else
                        {
                            imzaIsim = (string.IsNullOrEmpty(detay.akademikUnvan) ? "" : detay.akademikUnvan + " ") + OrtakFonksiyonlar.IlkHarflerBuyuk(detay.ad) + " " + detay.soyad;
                            imza = imzaIsim + "\r\n" + (string.IsNullOrEmpty(detay.unvan) ? "" : detay.unvan);
                            imzaUnvan = (string.IsNullOrEmpty(detay.unvan) ? "" : detay.unvan);
                        }

                        if (imzaIndex == 1) { bilgi.imza1 = imza; bilgi.imza1Isim = imzaIsim; bilgi.imza1Unvan = imzaUnvan; }
                        else if (imzaIndex == 2) { bilgi.imza2 = imza; bilgi.imza2Isim = imzaIsim; bilgi.imza2Unvan = imzaUnvan; }
                        else if (imzaIndex == 3) { bilgi.imza3 = imza; bilgi.imza3Isim = imzaIsim; bilgi.imza3Unvan = imzaUnvan; }
                    }
                }


                //Paraf listesinde kendisini yoksa ekle
                ArrayList parafList = new ArrayList();
                bool kendisiParafListesindeVarMi = false;
                foreach (ImzaYoluDetay d in imzaYolu.detaylar)
                    if (d.tcKimlikNo == kullanan.mernis) { kendisiParafListesindeVarMi = true; break; }

                if (!kendisiParafListesindeVarMi)
                    parafList.Add(new ImzaYoluDetay() { tcKimlikNo = kullanan.mernis, siraNo = 1, islemTip = (int)ENUMImzaYoluTip.PARAF, unvan = kullanan.unvan, ad = kullanan.ad, soyad = kullanan.soyad });

                foreach (var detay in imzaYolu.detaylar)
                {
                    if (detay.islemTip == (int)ENUMImzaYoluTip.IMZA) continue;
                    parafList.Add(detay);
                }
                //********************************************

                //Paraf
                foreach (ImzaYoluDetay detay in parafList)
                {
                    if (!imzaTcKimlikNoList.Contains(detay.tcKimlikNo) && !string.IsNullOrEmpty(detay.unvan))
                    {
                        string unvan = OrtakFonksiyonlar.IlkHarflerBuyuk(detay.unvan);
                        string akademikUnvan = !string.IsNullOrEmpty(detay.akademikUnvan) ? detay.akademikUnvan + " " : "";
                        string ad = OrtakFonksiyonlar.IlkHarflerBuyuk(detay.ad);
                        string soyad = detay.soyad.ToUpper(new System.Globalization.CultureInfo("tr-TR"));

                        bilgi.paraf += string.Format("{0}|" + "...../...../" + System.DateTime.Now.Year + "\t {1} : {2}{3} {4}\r\n", detay.islemTip, unvan, akademikUnvan, ad, soyad);
                    }
                }
                bilgi.paraf = bilgi.paraf.TrimEnd("\r\n".ToCharArray());
            }

            return bilgi;
        }

        private static string UnvanKisaAdGetir(string unvan)
        {
            switch (unvan.ToLower(new System.Globalization.CultureInfo("tr-TR")))
            {
                case "mühendis": unvan = "Müh."; break;
                case "daire başkanı": unvan = "Dai. Bşk."; break;
                case "genel müdür": unvan = "Gn. Md."; break;
                case "genel müdür yardımcısı": unvan = "Gn. Md. Yrd."; break;
            }

            return unvan;
        }

        private static string AdSoyadBuyukKucukHarfDuzenle(string adSoyad)
        {
            string rDeger = "";
            string[] tmp = adSoyad.Split(' ');
            for (int i = 0; i < tmp.Length; i++)
            {
                if (i != tmp.Length - 1) tmp[i] = OrtakFonksiyonlar.IlkHarflerBuyuk(tmp[i]);
                rDeger += tmp[i] + " ";
            }
            return rDeger.Trim();
        }
    }
}

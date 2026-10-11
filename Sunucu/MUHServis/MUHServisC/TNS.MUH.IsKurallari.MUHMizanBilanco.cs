using System;
using System.Collections;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections.Generic;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Mizan Bilanço rapor işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHMizanBilanco
    {
        public static string HesapPlaniYapiBul(int seviye)
        {
            string maskeHesap = TNS.MUH.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD8/KODYAPI");
            if (seviye > 0)
            {
                string[] hesapKirilim = maskeHesap.Split('.');
                maskeHesap = "";
                for (int i = 0; i < hesapKirilim.Length && i < seviye; i++)
                {
                    if (maskeHesap != "") maskeHesap += '.';
                    maskeHesap += hesapKirilim[i];
                }
            }
            return maskeHesap;
        }

        public static int MaskeUzunlukBul(string maske)
        {
            return maske.Replace(".", "").Length;
        }

        private static bool RaporuGormeYetkisiVarMi(Kullanici kullanan, int muhasebeKod, string birimKod)
        {
            return TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, muhasebeKod.ToString(), birimKod, true);
        }

        private static string HesapAdBul(VeriErisim ve, Kullanici kullanan, int yil, string hesapKod)
        {
            return MUHServis.UzayYaziDegerGetir(ve, kullanan, "MUHHESAPPLANI", yil + "-" + hesapKod, true, "ACIKLAMA");
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre bilanço raporunu oluşturur
        /// </summary>
        /// <param name="veriErisim">VeriErisim nesnesi</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="tarih1">The tarih1.</param>
        /// <param name="tarih2">The tarih2.</param>
        /// <returns></returns>
        public static ObjectArray Bilanco(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, int tur, int fisTur, TNSDateTime tarih1, TNSDateTime tarih2)
        {
            string hataStr = "";
            ObjectArray donusDeger = new ObjectArray();
            if (yil <= 0)
                hataStr = ButceMuhasebeResI.MUH105;
            else if (kurum == "")
                hataStr = ButceMuhasebeResI.MUH106;
            else if (!RaporuGormeYetkisiVarMi(kullanan, saymanlikKodu, birim))
            {
                hataStr = ButceMuhasebeResI.MUH107;
            }
            if (hataStr == "")
            {
                string maske = HesapPlaniYapiBul(1);
                int maskeUzunluk = MaskeUzunlukBul(maske);
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                TNS.MUH.IsKurallari.Veri.TABMizanBilanco tabMuhasebe = new TNS.MUH.IsKurallari.Veri.TABMizanBilanco(ve);
                IDataReader dr = tabMuhasebe.BilancoBilgi(yil, kurum, saymanlikKodu, birim, tur, maskeUzunluk, fisTur, tarih1, tarih2);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        double fark = VeriErisim.DegerAl(dr, 3, 0.0) - VeriErisim.DegerAl(dr, 4, 0.0);
                        if (fark != 0)
                        {
                            string hesapKod = VeriErisim.DegerAl(dr, 1, "");
                            string hesapAd = VeriErisim.DegerAl(dr, 2, "");
                            donusDeger.objeler.Add(new HesapSatir(VeriErisim.DegerAl(dr, 0, ""), hesapKod, hesapAd, fark));
                        }
                    }
                    if (donusDeger.objeler.Count == 0)
                    {
                        donusDeger.objeler.Add(new Sonuc(false, ButceMuhasebeResI.MUH108, ""));
                    }
                }
                else
                {
                    donusDeger.objeler.Add(new Sonuc(false, ve.hata, ""));
                }
                ve.VeriErisimKapat();
            }
            else
            {
                donusDeger.objeler.Add(new Sonuc(false, hataStr, ""));
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre mizan raporunu oluşturur
        /// </summary>
        /// <param name="veriErisim">VeriErisim nesnesi</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <param name="seviye">hesap kodu Seviyesi</param>
        /// <param name="basHesap">Başlangıç hesap kodu</param>
        /// <param name="bitHesap">Bitiş hesap kodu</param>
        /// <param name="tur">1 Yevmiyeli, 2 Onaylı, 3 Hepsi</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="listeTur">2 ise doviz borç alacak olarak çalışır</param>
        /// <returns></returns>
        public static MizanBilgi Mizan(VeriErisim veriErisim, TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string altHesap, bool ucgen = false)
        {
            string hataStr = "";
            MizanBilgi donusDeger = new MizanBilgi(0, 0);
            List<HesapBilgi> ucgenHesaplar = new List<HesapBilgi>();

            OdemeEmriMIFForm kriterler = new OdemeEmriMIFForm();
            kriterler.yil = yil;
            kriterler.kurum = kurum;
            kriterler.muhasebe = saymanlikKodu;
            kriterler.birim = birim;
            kriterler.YEVCekTarih = tarih1;
            kriterler.YEVYevTarih = tarih2;
            kriterler.surecYil = seviye;
            kriterler.yevmiyeNo1 = tur;
            kriterler.yevmiyeNo2 = listeTur;
            kriterler.fisTur = fisTur;

            if (yil <= 0)
                hataStr = ButceMuhasebeResI.MUH105;
            //else if (kurum == "")
            //    hataStr = ButceMuhasebeResI.MUH106;
            else if (tarih1.isNull)
                hataStr = ButceMuhasebeResI.MUH109;
            else if (tarih2.isNull)
                hataStr = ButceMuhasebeResI.MUH110;
            else if (seviye < 1)
                hataStr = ButceMuhasebeResI.MUH111;
            else if (!RaporuGormeYetkisiVarMi(kullanan, saymanlikKodu, birim))
            {
                hataStr = ButceMuhasebeResI.MUH107;
            }

            if (hataStr == "")
            {
                string maske = HesapPlaniYapiBul(seviye);
                int maskeUzunluk = MaskeUzunlukBul(maske);
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                VeriErisim veSor = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                VeriErisim veUcgen = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));


                //Eğer birim gönderildi ise Açılış Fişi tek fiş (farklı birimlerin) olan kurumlarda açılış rakamları alınamıyor. Bu nedenle kurumsal koda ihtiyaç var
                string kurKod = "";
                if (!string.IsNullOrEmpty(birim))
                {
                    ABSKodlarVAN kkv = new MUHMuhasebeBirimServis().HarcamaBirimleriKurumsalKod(null, yil, kurum, saymanlikKodu, birim);
                    if (kkv.sonuc.islemSonuc)
                    {
                        foreach (ABSKod k in kkv.absKodlar)
                        {
                            kurKod = k.kod.Replace(".", "");
                        }
                    }
                }

                string fonKod = "";
                ve.sureAsimi = 300;
                TNS.MUH.IsKurallari.Veri.TABMizanBilanco tabMuhasebe = new TNS.MUH.IsKurallari.Veri.TABMizanBilanco(ve);
                TNS.MUH.IsKurallari.Veri.TABMizanBilanco tabMuhasebeUcgen = new TNS.MUH.IsKurallari.Veri.TABMizanBilanco(veUcgen);
                TNSDateTime sonTarih = new TNSDateTime(tarih1.Oku());
                sonTarih.AddDays(-1);
                tabMuhasebe.MizanToplamBul(yil, kurum, saymanlikKodu, birim, null, sonTarih, basHesap, bitHesap, tur, maskeUzunluk, out donusDeger.oncBorc, out donusDeger.oncAlacak, fisTur, listeTur, kurKod, fonKod, altHesap, 0, 0);
                IDataReader dr = tabMuhasebe.MizanHesapBul(yil, kurum, saymanlikKodu, birim, tarih1, tarih2, basHesap, bitHesap, tur, maskeUzunluk, fisTur, listeTur, kurKod, fonKod, altHesap);
                if (dr != null)
                {
                    string anaHesapKod = "";
                    string simdHesapKod = "";
                    string altHesapKod = "";
                    while (dr.Read())
                    {
                        string hesapKod = OrtakFonksiyonlar.KodNoktaKoy(maske, VeriErisim.DegerAl(dr, 0, ""));
                        string hesapAd = VeriErisim.DegerAl(dr, 1, "");
                        double borc = Math.Round(VeriErisim.DegerAl(dr, 2, 0.0), 2);
                        double alacak = Math.Round(VeriErisim.DegerAl(dr, 3, 0.0), 2);

                        if (altHesap != "")
                        {
                            altHesapKod = VeriErisim.DegerAl(dr, 4, "");
                            hesapAd = VeriErisim.DegerAl(dr, 5, "");
                            if (string.IsNullOrEmpty(hesapAd))
                                hesapAd = VeriErisim.DegerAl(dr, 6, "");
                        }

                        if (seviye > 1)
                        {
                            if (ucgen)
                            {
                                MizanUcgenYap(kullanan, hesapKod, donusDeger, ucgenHesaplar, kriterler, tabMuhasebeUcgen, veUcgen);
                            }
                            else
                            {
                                if (hesapKod.Length >= 3)
                                    simdHesapKod = hesapKod.Substring(0, 3);
                                else
                                    simdHesapKod = hesapKod;

                                if (simdHesapKod != anaHesapKod)
                                {
                                    anaHesapKod = simdHesapKod;
                                    donusDeger.AnaHesapEkle(anaHesapKod, HesapAdBul(veSor, kullanan, yil, anaHesapKod));
                                }
                            }

                            donusDeger.DetayEkle(hesapKod, hesapAd, borc, alacak, "", "", "", "", "", altHesapKod, "");
                        }
                        else
                        {
                            donusDeger.anaHesaplar.objeler.Add(new AnaHesap(hesapKod, hesapAd, borc, alacak, "", "", "", "", ""));
                        }
                    }
                    if (donusDeger.anaHesaplar.objeler.Count == 0)
                    {
                        donusDeger.sonuc.islemSonuc = false;
                        donusDeger.sonuc.hataStr = ButceMuhasebeResI.MUH108;
                    }
                }
                else
                {
                    donusDeger.sonuc.islemSonuc = false;
                    donusDeger.sonuc.hataStr = ve.hata;
                }
                ve.VeriErisimKapat();
                veSor.VeriErisimKapat();
                veUcgen.VeriErisimKapat();
            }
            else
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hataStr;
            }
            return donusDeger;
        }

        private static void MizanUcgenYap(Kullanici kullanan, string hesapKod, MizanBilgi donusDeger, List<HesapBilgi> ucgenHesaplar, OdemeEmriMIFForm kriterler, TABMizanBilanco tabMuhasebe, VeriErisim veUcgen)
        {
            string[] hler = hesapKod.Split('.');
            string pp = "";
            foreach (string hp in hler)
            {
                if (pp != "") pp += ".";
                pp += hp;

                if (pp == hesapKod) break;//gelen hesap kodunu üst metod ekleyecek

                var bulundu = false;
                foreach (HesapBilgi mb in ucgenHesaplar)
                {
                    if (mb.hesapKod == pp)
                    {
                        bulundu = true;
                        break;
                    }
                }

                if (!bulundu)
                {
                    HesapBilgi mb = new HesapBilgi();
                    mb.hesapKod = pp;
                    ucgenHesaplar.Add(mb);

                    double uborc = 0;
                    double ualacak = 0;
                    tabMuhasebe.MizanToplamBul(kriterler.yil, kriterler.kurum, kriterler.muhasebe, kriterler.birim, kriterler.YEVCekTarih, kriterler.YEVYevTarih, pp, pp + "z", kriterler.yevmiyeNo1, 99, out uborc, out ualacak, kriterler.fisTur, kriterler.yevmiyeNo2, "", "", "", 0, 0);

                    if (pp.Length == 3)
                        donusDeger.AnaHesapEkle(pp, HesapAdBul(veUcgen, kullanan, kriterler.yil, pp));

                    donusDeger.DetayEkle(pp, HesapAdBul(veUcgen, kullanan, kriterler.yil, pp), uborc, ualacak, "", "", "", "", "", "", "");
                }

            }
        }

        public static MizanBilgi MizanToplamBul(VeriErisim ve, TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string altHesap)
        {
            MizanBilgi donusDeger = new MizanBilgi(0, 0);

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TNS.MUH.IsKurallari.Veri.TABMizanBilanco tabMuhasebe = new TNS.MUH.IsKurallari.Veri.TABMizanBilanco(ve);

            tabMuhasebe.MizanToplamBul(yil, kurum, saymanlikKodu, birim, tarih1, tarih2, basHesap, bitHesap, tur, 99, out donusDeger.oncBorc, out donusDeger.oncAlacak, fisTur, listeTur, "", "", altHesap, 0, 0);

            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre birim bazında mizan raporunu oluşturur
        /// </summary>
        /// <param name="veriErisim">VeriErisim nesnesi</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <param name="seviye">hesap kodu Seviyesi</param>
        /// <param name="basHesap">Başlangıç hesap kodu</param>
        /// <param name="bitHesap">Bitiş hesap kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <returns></returns>
        public static MizanBilgi MizanBirim(VeriErisim veriErisim, TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string kurKod = "", string fonKod = "", string altHesap = "")
        {
            string hataStr = "";
            MizanBilgi donusDeger = new MizanBilgi(0, 0);
            if (yil <= 0)
                hataStr = ButceMuhasebeResI.MUH105;
            else if (kurum == "")
                hataStr = ButceMuhasebeResI.MUH106;
            else if (tarih1.isNull)
                hataStr = ButceMuhasebeResI.MUH109;
            else if (tarih2.isNull)
                hataStr = ButceMuhasebeResI.MUH110;
            else if (seviye < 1)
                hataStr = ButceMuhasebeResI.MUH111;
            else if (!RaporuGormeYetkisiVarMi(kullanan, saymanlikKodu, birim))
            {
                hataStr = ButceMuhasebeResI.MUH107;
            }

            if (hataStr == "")
            {
                string maske = HesapPlaniYapiBul(seviye);
                int maskeUzunluk = MaskeUzunlukBul(maske);
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                VeriErisim veSor = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                TNS.MUH.IsKurallari.Veri.TABMizanBilanco tabMuhasebe = new TNS.MUH.IsKurallari.Veri.TABMizanBilanco(ve);
                TNSDateTime sonTarih = new TNSDateTime(tarih1.Oku());
                sonTarih.AddDays(-1);
                tabMuhasebe.MizanToplamBul(yil, kurum, saymanlikKodu, birim, null, sonTarih, basHesap, bitHesap, tur, maskeUzunluk, out donusDeger.oncBorc, out donusDeger.oncAlacak, fisTur, listeTur, kurKod, fonKod, altHesap, 0, 0);
                IDataReader dr = tabMuhasebe.MizanBirimHesapBul(yil, kurum, saymanlikKodu, birim, tarih1, tarih2, basHesap, bitHesap, tur, maskeUzunluk, fisTur, listeTur, kurKod, fonKod);
                if (dr != null)
                {
                    string anaHesapKod = "";
                    string simdHesapKod = "";

                    while (dr.Read())
                    {
                        string hesapKod = OrtakFonksiyonlar.KodNoktaKoy(maske, VeriErisim.DegerAl(dr, 0, ""));
                        string hesapAd = VeriErisim.DegerAl(dr, 1, "");
                        string kur = VeriErisim.DegerAl(dr, 2, "");
                        double borc = VeriErisim.DegerAl(dr, 3, 0.0);
                        double alacak = VeriErisim.DegerAl(dr, 4, 0.0);

                        if (seviye > 1)
                        {
                            simdHesapKod = hesapKod.Substring(0, 3);
                            if (simdHesapKod != anaHesapKod)
                            {
                                anaHesapKod = simdHesapKod;
                                donusDeger.AnaHesapEkle(anaHesapKod, HesapAdBul(veSor, kullanan, yil, anaHesapKod));
                            }
                            donusDeger.DetayEkle(hesapKod, hesapAd, borc, alacak, kur, "", "", "", "", "", "");
                        }
                        else
                        {
                            donusDeger.anaHesaplar.objeler.Add(new AnaHesap(hesapKod, hesapAd, borc, alacak, kur, "", "", "", ""));
                        }
                    }
                    if (donusDeger.anaHesaplar.objeler.Count == 0)
                    {
                        donusDeger.sonuc.islemSonuc = false;
                        donusDeger.sonuc.hataStr = ButceMuhasebeResI.MUH108;
                    }
                }
                else
                {
                    donusDeger.sonuc.islemSonuc = false;
                    donusDeger.sonuc.hataStr = ve.hata;
                }
                ve.VeriErisimKapat();
                veSor.VeriErisimKapat();
            }
            else
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hataStr;
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre tertip bazında mizan raporunu oluşturur
        /// </summary>
        /// <param name="veriErisim">VeriErisim nesnesi</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <param name="seviye">Hesap kodu Seviyesi</param>
        /// <param name="basHesap">Başlangıç hesap kodu</param>
        /// <param name="bitHesap">Bitiş hesap kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <returns></returns>
        public static MizanBilgi MizanTertip(VeriErisim veriErisim, TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string altHesap, bool gercekEkoKodVer, int yevmiyeNo1, int yevmiyeNo2)
        {
            string hataStr = "";
            MizanBilgi donusDeger = new MizanBilgi(0, 0);
            if (yil <= 0)
                hataStr = ButceMuhasebeResI.MUH105;
            else if (kurum == "")
                hataStr = ButceMuhasebeResI.MUH106;
            else if (tarih1.isNull)
                hataStr = ButceMuhasebeResI.MUH109;
            else if (tarih2.isNull)
                hataStr = ButceMuhasebeResI.MUH110;
            else if (seviye < 1)
                hataStr = ButceMuhasebeResI.MUH111;
            else if (!RaporuGormeYetkisiVarMi(kullanan, saymanlikKodu, birim))
            {
                hataStr = ButceMuhasebeResI.MUH107;
            }

            if (hataStr == "")
            {
                string maske = HesapPlaniYapiBul(seviye);
                int maskeUzunluk = MaskeUzunlukBul(maske);
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                TNS.MUH.IsKurallari.Veri.TABMizanBilanco tabMuhasebe = new TNS.MUH.IsKurallari.Veri.TABMizanBilanco(ve);
                TNSDateTime sonTarih = new TNSDateTime(tarih1.Oku());
                sonTarih.AddDays(-1);
                tabMuhasebe.MizanToplamBul(yil, kurum, saymanlikKodu, birim, null, sonTarih, basHesap, bitHesap, tur, maskeUzunluk, out donusDeger.oncBorc, out donusDeger.oncAlacak, fisTur, listeTur, "", "", altHesap, yevmiyeNo1, yevmiyeNo2);
                IDataReader dr = tabMuhasebe.MizanBirimTertipBul(yil, kurum, saymanlikKodu, birim, tarih1, tarih2, basHesap, bitHesap, tur, maskeUzunluk, fisTur, listeTur, altHesap, gercekEkoKodVer, yevmiyeNo1, yevmiyeNo2);
                if (dr != null)
                {
                    string anaHesapKod = "";
                    string simdHesapKod = "";

                    while (dr.Read())
                    {
                        string hesapKod = OrtakFonksiyonlar.KodNoktaKoy(maske, VeriErisim.DegerAl(dr, 0, ""));
                        string kurkod = VeriErisim.DegerAl(dr, 1, "");
                        string fonkod = VeriErisim.DegerAl(dr, 2, "");
                        string finkod = VeriErisim.DegerAl(dr, 3, "");
                        string ekokod = VeriErisim.DegerAl(dr, 4, "");
                        string pebkod = VeriErisim.DegerAl(dr, 5, "");

                        double borc = VeriErisim.DegerAl(dr, 6, 0.0);
                        double alacak = VeriErisim.DegerAl(dr, 7, 0.0);
                        string hesapAd = VeriErisim.DegerAl(dr, 8, "");
                        string birimKod = VeriErisim.DegerAl(dr, 9, "");

                        if (seviye > 1)
                        {
                            simdHesapKod = hesapKod.Substring(0, 3);
                            if (simdHesapKod != anaHesapKod)
                            {
                                anaHesapKod = simdHesapKod;
                                donusDeger.AnaHesapEkle(anaHesapKod, hesapAd);//HesapAdBul(yil, anaHesapKod));
                            }
                            donusDeger.DetayEkle(hesapKod, hesapAd, borc, alacak, kurkod, fonkod, finkod, ekokod, pebkod, "", birimKod);
                        }
                        else
                        {
                            donusDeger.anaHesaplar.objeler.Add(new AnaHesap(hesapKod, hesapAd, borc, alacak, kurkod, fonkod, finkod, ekokod, pebkod));
                        }
                    }
                    if (donusDeger.anaHesaplar.objeler.Count == 0)
                    {
                        donusDeger.sonuc.islemSonuc = false;
                        donusDeger.sonuc.hataStr = ButceMuhasebeResI.MUH108;
                    }
                }
                else
                {
                    donusDeger.sonuc.islemSonuc = false;
                    donusDeger.sonuc.hataStr = ve.hata;
                }
                ve.VeriErisimKapat();
            }
            else
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hataStr;
            }
            return donusDeger;
        }

        public static Sonuc MuhasebeSorgu(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string kurKod, string fonKod, string altHesap, int borcAlacak)
        {
            Sonuc s = new Sonuc();
            string hataStr = "";
            if (yil <= 0)
                hataStr = ButceMuhasebeResI.MUH105;
            else if (kurum == "")
                hataStr = ButceMuhasebeResI.MUH106;
            else if (tarih1.isNull)
                hataStr = ButceMuhasebeResI.MUH109;
            else if (tarih2.isNull)
                hataStr = ButceMuhasebeResI.MUH110;
            else if (seviye < 1)
                hataStr = ButceMuhasebeResI.MUH111;
            else if (!RaporuGormeYetkisiVarMi(kullanan, saymanlikKodu, birim))
            {
                hataStr = ButceMuhasebeResI.MUH107;
            }

            if (hataStr == "")
            {
                string maske = HesapPlaniYapiBul(seviye);
                int maskeUzunluk = MaskeUzunlukBul(maske);
                TNSDateTime sonTarih = new TNSDateTime(tarih1.Oku());
                sonTarih.AddDays(-1);
                string sqlKosul = TNS.MUH.IsKurallari.Veri.TABMizanBilanco.MizanKosulOl("MUHMIF", "MUHMIFFORM", yil, kurum, saymanlikKodu, birim, tarih1, tarih2, basHesap, bitHesap, tur, maskeUzunluk, fisTur, listeTur, kurKod, fonKod, altHesap, borcAlacak, 0, 0);
                s.bilgiStr = "(SELECT MIN(AD) FROM " + TABOrtak.DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCFIRMABILGISI where VERGINO=MUHMIF.ALTHESAPKOD AND MUHASEBEKOD=MUHMIFFORM.MUHASEBEKOD), (SELECT MIN(AD + ' ' + SOYAD) FROM Personel.dbo.PERPersonel where TCKIMLIKNO=MUHMIF.ALTHESAPKOD) FROM MUHMIF, MUHMIFFORM, MUHHESAPPLANI WHERE MUHMIF.YIL=MUHMIFFORM.YIL AND MUHMIF.BELGENO=MUHMIFFORM.BELGENO AND MUHMIFFORM.FORMDURUM<>9 AND MUHMIF.YIL=MUHHESAPPLANI.YIL AND MUHMIF.HESAPKOD=MUHHESAPPLANI.HESAPKOD AND " + sqlKosul;
            }
            else
                s.DegerAta(hataStr);
            return s;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre ödeme emri belgelerinde 830-905 hesap kodlarının kullanımını raporlar
        /// </summary>
        /// <param name="veriErisim">VeriErisim nesnesi</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        public static OdemeEmriMIFForm Kontrol830905(VeriErisim veriErisim, TNS.KYM.Kullanici kullanan, int yil, int saymanlikKodu, string birim)
        {
            string hataStr = "";
            OdemeEmriMIFForm donusDeger = new OdemeEmriMIFForm();

            if (yil <= 0)
                hataStr = ButceMuhasebeResI.MUH105;
            else if (!RaporuGormeYetkisiVarMi(kullanan, saymanlikKodu, birim))
                hataStr = ButceMuhasebeResI.MUH107;

            if (hataStr == "")
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                TNS.MUH.IsKurallari.Veri.TABMizanBilanco tabMuhasebe = new TNS.MUH.IsKurallari.Veri.TABMizanBilanco(ve);
                IDataReader dr = tabMuhasebe.Kontrol830905(yil, saymanlikKodu, birim);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        OdemeEmriMIFDetay od = new OdemeEmriMIFDetay(yil);

                        od.belgeTur = VeriErisim.DegerAl(dr, 1, 0);
                        od.belgeNo = VeriErisim.DegerAl(dr, 2, "");
                        od.hesapKod = VeriErisim.DegerAl(dr, 3, "");
                        string kur = VeriErisim.DegerAl(dr, 4, "");
                        string fon = VeriErisim.DegerAl(dr, 5, "");
                        string fin = VeriErisim.DegerAl(dr, 6, "");
                        string eko = VeriErisim.DegerAl(dr, 7, "");
                        TNS.MUH.Arac.TertipAta(ref od, kur + "-" + fon + "-" + fin + "-" + eko);
                        od.borc = VeriErisim.DegerAl(dr, 8, 0.0);
                        od.alacak = VeriErisim.DegerAl(dr, 9, 0.0);

                        if (od.hesapKod.StartsWith("905"))
                            od.aciklama = ButceMuhasebeResI.MUH112;//Aşağıda kontrol sırasında karşılı var ise listeden siliniyor. Kalanların karşılığı yok demektir.
                        donusDeger.detay.objeler.Add(od);
                    }

                    for (int i = 0; i < donusDeger.detay.objeler.Count; i++)
                    {
                        OdemeEmriMIFDetay d1 = new OdemeEmriMIFDetay(yil);
                        d1 = (OdemeEmriMIFDetay)donusDeger.detay[i];

                        if (d1.hesapKod.StartsWith("830"))
                        {
                            string hk = "905" + d1.hesapKod.Substring(3);
                            bool karsilikHesapYok = true;
                            bool tertipFarkli = true;
                            for (int j = i + 1; j < donusDeger.detay.objeler.Count; j++)
                            {
                                OdemeEmriMIFDetay d2 = new OdemeEmriMIFDetay(yil);
                                d2 = (OdemeEmriMIFDetay)donusDeger.detay[j];

                                if (d1.belgeNo == d2.belgeNo)
                                {
                                    if (hk == d2.hesapKod)
                                    {
                                        karsilikHesapYok = false;
                                        d2.aciklama = "";//Demekki 830un 905 karşılığı var

                                        if (TNS.MUH.Arac.TertipStrOl(d1) == TNS.MUH.Arac.TertipStrOl(d2))
                                        {
                                            tertipFarkli = false;
                                            if (d1.borc == d2.borc && d1.alacak == d2.alacak)
                                            {
                                                donusDeger.detay.objeler.RemoveAt(j);//i den sonra olduğu için ilk atılması gerek
                                                donusDeger.detay.objeler.RemoveAt(i);
                                                i = -1;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (karsilikHesapYok)
                                d1.aciklama = ButceMuhasebeResI.MUH113;
                            else if (tertipFarkli)
                                d1.aciklama = ButceMuhasebeResI.MUH114;
                            else
                                d1.aciklama = ButceMuhasebeResI.MUH115;
                        }
                    }
                }
                else
                    donusDeger.aciklama = ve.hata;

                ve.VeriErisimKapat();
            }
            else
            {
                donusDeger.aciklama = hataStr;
            }
            return donusDeger;
        }

        /// <summary>
        /// Ödenek üstü raporunu hazırlar.
        /// </summary>
        /// <param name="veriErisim">VeriErisim nesnesi</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        public static OdemeEmriMIFForm OdenekUstu(VeriErisim veriErisim, TNS.KYM.Kullanici kullanan, int yil, int saymanlikKodu, string birim)
        {
            string hataStr = "";
            OdemeEmriMIFForm donusDeger = new OdemeEmriMIFForm();

            if (yil <= 0)
                hataStr = ButceMuhasebeResI.MUH105;
            else if (!RaporuGormeYetkisiVarMi(kullanan, saymanlikKodu, birim))
                hataStr = ButceMuhasebeResI.MUH107;

            if (hataStr == "")
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                TNS.MUH.IsKurallari.Veri.TABMizanBilanco tabMuhasebe = new TNS.MUH.IsKurallari.Veri.TABMizanBilanco(ve);
                IDataReader dr = tabMuhasebe.OdenekUstu(yil, saymanlikKodu, birim);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        OdemeEmriMIFDetay od = new OdemeEmriMIFDetay(yil);

                        od.belgeTur = VeriErisim.DegerAl(dr, 1, 0);
                        od.belgeNo = VeriErisim.DegerAl(dr, 2, "");
                        od.hesapKod = VeriErisim.DegerAl(dr, 3, "");
                        string kur = VeriErisim.DegerAl(dr, 4, "");
                        string fon = VeriErisim.DegerAl(dr, 5, "");
                        string fin = VeriErisim.DegerAl(dr, 6, "");
                        string eko = VeriErisim.DegerAl(dr, 7, "");
                        TNS.MUH.Arac.TertipAta(ref od, kur + "-" + fon + "-" + fin + "-" + eko);
                        od.borc = VeriErisim.DegerAl(dr, 8, 0.0);
                        od.alacak = VeriErisim.DegerAl(dr, 9, 0.0);

                        if (od.hesapKod.StartsWith("900"))
                            od.aciklama = ButceMuhasebeResI.MUH112;//Aşağıda kontrol sırasında karşılı var ise listeden siliniyor. Kalanların karşılığı yok demektir.
                        donusDeger.detay.objeler.Add(od);
                    }

                    for (int i = 0; i < donusDeger.detay.objeler.Count; i++)
                    {
                        OdemeEmriMIFDetay d1 = new OdemeEmriMIFDetay(yil);
                        d1 = (OdemeEmriMIFDetay)donusDeger.detay[i];

                        if (d1.hesapKod.StartsWith("830"))
                        {
                            string hk = "900" + d1.hesapKod.Substring(3);
                            bool karsilikHesapYok = true;
                            bool tertipFarkli = true;
                            for (int j = i + 1; j < donusDeger.detay.objeler.Count; j++)
                            {
                                OdemeEmriMIFDetay d2 = new OdemeEmriMIFDetay(yil);
                                d2 = (OdemeEmriMIFDetay)donusDeger.detay[j];

                                if (d1.belgeNo == d2.belgeNo)
                                {
                                    if (hk == d2.hesapKod)
                                    {
                                        karsilikHesapYok = false;
                                        d2.aciklama = "";//Demekki 830un 900 karşılığı var

                                        if (TNS.MUH.Arac.TertipStrOl(d1) == TNS.MUH.Arac.TertipStrOl(d2))
                                        {
                                            tertipFarkli = false;
                                            if (d1.borc >= d2.borc || d1.alacak >= d2.alacak)
                                            {
                                                donusDeger.detay.objeler.RemoveAt(j);//i den sonra olduğu için ilk atılması gerek
                                                donusDeger.detay.objeler.RemoveAt(i);
                                                i = -1;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (karsilikHesapYok)
                                d1.aciklama = ButceMuhasebeResI.MUH116;
                            else if (tertipFarkli)
                                d1.aciklama = ButceMuhasebeResI.MUH117;
                            else
                                d1.aciklama = ButceMuhasebeResI.MUH118;
                        }
                    }
                }
                else
                    donusDeger.aciklama = ve.hata;

                ve.VeriErisimKapat();
            }
            else
            {
                donusDeger.aciklama = hataStr;
            }
            return donusDeger;
        }

        /// <summary>
        /// Hatalı hesap kodu kullanılan ödeme emri belgelerini raporlar.
        /// </summary>
        /// <param name="veriErisim">VeriErisim nesnesi</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        public static OdemeEmriMIFForm UstHesapKodKullanilanlar(VeriErisim veriErisim, TNS.KYM.Kullanici kullanan, int yil, int saymanlikKodu, string birim)
        {
            string hataStr = "";
            OdemeEmriMIFForm donusDeger = new OdemeEmriMIFForm();

            if (yil <= 0)
                hataStr = ButceMuhasebeResI.MUH105;
            else if (!RaporuGormeYetkisiVarMi(kullanan, saymanlikKodu, birim))
                hataStr = ButceMuhasebeResI.MUH107;

            if (hataStr == "")
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                TNS.MUH.IsKurallari.Veri.TABMizanBilanco tabMuhasebe = new TNS.MUH.IsKurallari.Veri.TABMizanBilanco(ve);
                IDataReader dr = tabMuhasebe.UstHesapKodKullanilanlar(yil);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        OdemeEmriMIFDetay od = new OdemeEmriMIFDetay(yil);

                        od.belgeNo = VeriErisim.DegerAl(dr, 0, "");
                        od.hesapKod = VeriErisim.DegerAl(dr, 1, "");
                        od.borc = VeriErisim.DegerAl(dr, 2, 0.0);
                        od.alacak = VeriErisim.DegerAl(dr, 3, 0.0);
                        od.aciklama = VeriErisim.DegerAl(dr, 4, "");
                        donusDeger.detay.objeler.Add(od);
                    }
                }
                else
                    donusDeger.aciklama = ve.hata;

                ve.VeriErisimKapat();
            }
            else
                donusDeger.aciklama = hataStr;

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre büyük defter raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="basla">The basla.</param>
        /// <param name="bitis">The bitis.</param>
        /// <param name="raporTur">The rapor tur.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// YetkisiYok
        /// </exception>
        public BuyukDefter[] BuyukDefterListele(Kullanici kullanan, OdemeEmriMIFForm form, int basla, int bitis, string raporTur)
        {
            ArrayList donusDeger = new ArrayList();

            string kurum = form.kurum;

            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, kurum))
            {
                //fV.sonuc.hataStr = ButceMuhasebeResI.MUH048;
                //fV.sonuc.islemSonuc = false;
                throw new Exception(ButceMuhasebeResI.MUH048);
            }

            //Listeleme sırasında kurum kullanıcıları kendilerine ait evrakları görsün
            MUHMuhasebeBirimServis.KullanicininYetkiliOlduguBirimler(kullanan, form);
            if (form.aciklama == "YetkisiYok")
                throw new Exception("YetkisiYok");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onceAnaSonraTarih = false;
            if (raporTur.StartsWith("10")) onceAnaSonraTarih = true;

            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader r = to.FormListe(form, true, false, onceAnaSonraTarih);

            if (to.Hata() == "")
            {
                Tertip th = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);
                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);

                int sayac = 1;
                while (r.Read())
                {
                    BuyukDefter os = new BuyukDefter();

                    os.yil = VeriErisim.DegerAl(r, 0, 0);
                    os.belgeTur = VeriErisim.DegerAl(r, 1, 0);
                    os.belgeNo = VeriErisim.DegerAl(r, 2, "");
                    os.kurum = VeriErisim.DegerAl(r, 3, "");
                    os.muhasebe = VeriErisim.DegerAl(r, 4, 0);
                    os.birim = VeriErisim.DegerAl(r, 5, "");
                    os.ilgiliAd = VeriErisim.DegerAl(r, 6, "");
                    os.ilgiliNo = VeriErisim.DegerAl(r, 7, "");
                    //os.ilgiliBankaAd = VeriErisim.DegerAl(r, 8, "");
                    //os.ilgiliBankaNo = VeriErisim.DegerAl(r, 9, "");
                    //os.ilgiliVD = VeriErisim.DegerAl(r, 10, "");
                    os.aciklama = VeriErisim.DegerAl(r, 11, "");
                    os.islemTarih = VeriErisim.DegerAl(r, 12);
                    os.formDurumTarih = VeriErisim.DegerAl(r, 13);
                    os.formDurum = VeriErisim.DegerAl(r, 14, 0);
                    //os.iptalNeden = VeriErisim.DegerAl(r, 15, "");
                    os.YEVYevTarih = VeriErisim.DegerAl(r, 16);
                    os.YEVYevNo = VeriErisim.DegerAl(r, 17, 0);

                    os.kurum = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), os.kurum);
                    os.islemYapan = VeriErisim.DegerAl(r, 30, "");
                    //os.projeNo = VeriErisim.DegerAl(r, 31, "");
                    //os.ihale = VeriErisim.DegerAl(r, 32, "");
                    //os.fisTur = VeriErisim.DegerAl(r, 33, 0);

                    os.kurumAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KURAD, "");
                    os.muhasebeAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.MUHASEBEAD, "");

                    os.satirno = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.SIRA, 0);

                    //Kapanış kayıtlarından dolayı (mahsup) bir hesap kodu birden fazla belgede kapatılabiliyor. Fakat sorgulandığında çift kayıt olarak listeniyor. 
                    //Bu engellemek için. Kayıt edilen tabloya bunu teke indirecek bir kolon koyulamaz. Çünkü toplu halde mahsup yapılabiliyor.
                    //*************************************************
                    bool bulundu = false;
                    foreach (BuyukDefter bd in donusDeger)
                    {
                        if (bd.belgeNo == os.belgeNo && bd.satirno == os.satirno)
                        {
                            bulundu = true;
                            break;
                        }
                    }
                    if (bulundu) continue;
                    //*************************************************

                    os.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.HESAPKOD, ""));

                    os.kur = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KURKOD, "");
                    os.fon = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FONKOD, "");
                    os.fin = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FINKOD, "");
                    os.eko = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.EKOKOD, "");
                    if (os.kur != "")
                    {
                        os.kur = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), os.kur);
                        os.fon = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(1), os.fon);
                        os.fin = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(2), os.fin);
                        os.eko = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(3), os.eko);
                        os.prgKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.PRGKOD, 0);
                        os.aPrgKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.APRGKOD, 0);
                        os.faalKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FAALKOD, 0);
                        os.aFaalKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.AFAALKOD, 0);
                    }

                    os.borc = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.BORC, 0.0);
                    os.alacak = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALACAK, 0.0);
                    os.borcDoviz = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZBORC, 0.0);
                    os.alacakDoviz = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZALACAK, 0.0);

                    if (form.sorguDovizliIslemler)
                    {
                        os.borc = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZBORC, 0.0);
                        os.alacak = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZALACAK, 0.0);
                    }

                    os.hesapAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.HESAPPLANI_ACIKLAMA, "");


                    string satirAciklama = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ACIKLAMA, "");
                    if (satirAciklama != "") satirAciklama += " - ";
                    os.aciklama = satirAciklama + os.aciklama;

                    os.detayBirim = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DETAYBIRIM, "");

                    os.altHesap = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALTHESAPKOD, "");
                    os.altHesapAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALTHESAPAD1, "");
                    if (string.IsNullOrEmpty(os.altHesapAd))
                        os.altHesapAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALTHESAPAD2, "");

                    if (sayac >= basla && sayac <= bitis)
                        donusDeger.Add(os);

                    if (sayac >= bitis)
                        break;

                    sayac++;
                }

                r.Close();

                ve.VeriErisimKapat();
            }

            return (BuyukDefter[])donusDeger.ToArray(typeof(BuyukDefter));
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre büyük defter devreden rakamlar raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public double[] BuyukDefterDevreden(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            double[] donusDeger = new double[2];

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader r = to.FormListe(form, true, false, false);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    if (form.sorguDovizliIslemler)
                    {
                        donusDeger[0] += VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZBORC, 0.0);
                        donusDeger[1] += VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZALACAK, 0.0);
                    }
                    else
                    {
                        donusDeger[0] += VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.BORC, 0.0);
                        donusDeger[1] += VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALACAK, 0.0);
                    }
                }

                r.Close();
            }
            ///////////////////////////////////////////////

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gider tertip) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray KesinHesapGiderTertipListele(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH047, "");

            ObjectArray fV = new ObjectArray();

            string kurum = form.kurum;

            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, kurum))
            {
                fV.sonuc.hataStr = ButceMuhasebeResI.MUH048;
                fV.sonuc.islemSonuc = false;
                return fV;
            }

            //Listeleme sırasında kurum kullanıcıları kendilerine ait evrakları görsün
            MUHMuhasebeBirimServis.KullanicininYetkiliOlduguBirimler(kullanan, form);
            if (form.aciklama == "YetkisiYok")
                return fV;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);

            form.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            IDataReader r = to.KesinHesapBilgiListele(form, false, false, false, (int)ENUMKesinHesapButce.TANIMSIZ);

            if (to.Hata() == "")
            {
                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);

                while (r.Read())
                {
                    string os = "";

                    string kur = VeriErisim.DegerAl(r, 0, "");
                    string fon = VeriErisim.DegerAl(r, 1, "");
                    string fin = VeriErisim.DegerAl(r, 2, "");
                    string eko = VeriErisim.DegerAl(r, 3, "");
                    if (kur != "")
                    {
                        kur = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), kur);
                        fon = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(1), fon);
                        fin = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(2), fin);
                        eko = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(3), eko);
                    }

                    os = kur + "-" + fon + "-" + fin + "-" + eko;

                    fV.objeler.Add(os);
                }

                r.Close();
            }
            ve.VeriErisimKapat();
            return fV;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Bütçe) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="kodTur">kod tur</param>
        /// <returns></returns>
        public ObjectArray KesinHesapButce(Kullanici kullanan, OdemeEmriMIFForm form, int kodTur)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH047, "");

            ObjectArray fV = new ObjectArray();

            string kurum = form.kurum;

            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, kurum))
            {
                fV.sonuc.hataStr = ButceMuhasebeResI.MUH048;
                fV.sonuc.islemSonuc = false;
                return fV;
            }

            //Listeleme sırasında kurum kullanıcıları kendilerine ait evrakları görsün
            MUHMuhasebeBirimServis.KullanicininYetkiliOlduguBirimler(kullanan, form);
            if (form.aciklama == "YetkisiYok")
                return fV;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);
            Tertip th = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

            int yil = form.yil;
            string sorguYEVYevTarih = form.sorguYEVYevTarih;
            string sorguIslemTarih = form.sorguIslemTarih;
            if (form.fisTur == null || form.fisTur.Length == 0)
                form.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });

            string sorguYEVYevTarihYil2 = "";
            string sorguIslemTarihYil2 = "";
            string sorguYEVYevTarihYil3 = "";
            string sorguIslemTarihYil3 = "";

            if (sorguYEVYevTarih != "")
            {
                sorguYEVYevTarihYil2 = "01.01." + (yil - 1).ToString() + ";" + "31.12." + (yil - 1).ToString();
                sorguYEVYevTarihYil3 = "01.01." + (yil - 2).ToString() + ";" + "31.12." + (yil - 2).ToString();
            }

            if (sorguIslemTarih != "")
            {
                sorguIslemTarihYil2 = "01.01." + (yil - 1).ToString() + ";" + "31.12." + (yil - 1).ToString();
                sorguIslemTarihYil3 = "01.01." + (yil - 2).ToString() + ";" + "31.12." + (yil - 2).ToString();
            }

            IDataReader r = to.KesinHesapBilgiListele(form, false, false, false, kodTur);
            //Önceki yılların hesap kodlarını alabilmek için
            if (to.Hata() == "")
            {
                BilgiDoldur(r, fV.objeler, kodTur, 1);
                r.Close();

                //1 yıl önceki değerler
                form.yil = yil - 1;
                form.sorguIslemTarih = sorguIslemTarihYil2;// sorguIslemTarih.Replace(yil.ToString(), (yil - 1).ToString());
                form.sorguYEVYevTarih = sorguYEVYevTarihYil2;// sorguYEVYevTarih.Replace(yil.ToString(), (yil - 1).ToString());
                r = to.KesinHesapBilgiListele(form, false, false, false, kodTur);
                if (to.Hata() == "")
                {
                    BilgiDoldur(r, fV.objeler, kodTur, 2);
                    r.Close();
                }

                //2 yıl önceki değerler
                form.yil = yil - 2;
                form.sorguIslemTarih = sorguIslemTarihYil3;//sorguIslemTarih.Replace(yil.ToString(), (yil - 2).ToString());
                form.sorguYEVYevTarih = sorguYEVYevTarihYil3;//sorguYEVYevTarih.Replace(yil.ToString(), (yil - 2).ToString());
                r = to.KesinHesapBilgiListele(form, false, false, false, kodTur);
                if (to.Hata() == "")
                {
                    BilgiDoldur(r, fV.objeler, kodTur, 3);
                    r.Close();
                }

                fV.objeler.Sort(new Sirala());
                foreach (TNS.MUH.KesinHesapButce k in fV.objeler)
                {
                    if (k.kod != "")
                    {
                        if (kodTur == (int)ENUMKesinHesapButce.KURUMSAL)
                            k.kod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), k.kod);
                        else if (kodTur == (int)ENUMKesinHesapButce.FONKSIYONEL)
                            k.kod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(1), k.kod);
                        else if (kodTur == (int)ENUMKesinHesapButce.FINANS)
                            k.kod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(2), k.kod);
                        else if (kodTur == (int)ENUMKesinHesapButce.EKONOMIK)
                            k.kod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(3), k.kod);
                        else if (kodTur == (int)ENUMKesinHesapButce.EKONOMIK2)
                            k.kod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(3), k.kod);
                        else if (kodTur == (int)ENUMKesinHesapButce.GELIREKONOMIK || kodTur == (int)ENUMKesinHesapButce.GIDEREKONOMIK)
                        {
                            k.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), k.hesapKod);
                            if (k.hesapKod.Length >= 4)
                                k.kod = k.hesapKod.Substring(4);
                            else
                                k.kod = k.hesapKod;

                            if (k.hesapKod.Length > 3)
                                k.hesapKod = k.hesapKod.Remove(3);
                        }
                    }
                }
            }

            ve.VeriErisimKapat();
            return fV;
        }

        //string EkonomikKodYap(string hesapKod)
        //{
        //    string eko = "";
        //    try
        //    {
        //        eko = hesapKod.Substring(0, 2) + hesapKod.Substring(3, 1) + hesapKod.Substring(5, 1) + hesapKod.Substring(6, 2);
        //    }
        //    catch { }

        //    return eko;
        //}

        void BilgiDoldur(IDataReader r, TNSCollection oArray, int kodTur, int alanNo)
        {
            double carp;
            if (kodTur == (int)ENUMKesinHesapButce.GELIREKONOMIK)
                carp = -1;
            else
                carp = 1;
            while (r.Read())
            {
                TNS.MUH.KesinHesapButce k = new KesinHesapButce();

                k.hesapKod = VeriErisim.DegerAl(r, 0, "");
                k.kod = VeriErisim.DegerAl(r, 1, "");
                double borc = VeriErisim.DegerAl(r, 2, 0.0);
                double alacak = VeriErisim.DegerAl(r, 3, 0.0);

                if (kodTur == (int)ENUMKesinHesapButce.TANIMSIZ)
                {
                    k.kod = VeriErisim.DegerAl(r, 0, "") + "-" + VeriErisim.DegerAl(r, 1, "") + "-" + VeriErisim.DegerAl(r, 2, "") + "-" + VeriErisim.DegerAl(r, 3, "");
                }

                bool var = false;
                foreach (TNS.MUH.KesinHesapButce kk in oArray)
                {
                    if (kk.hesapKod + kk.kod == k.hesapKod + k.kod)
                    {
                        var = true;
                        k = kk;
                        break;
                    }
                }
                if (!var)
                    oArray.Add(k);

                k.DegerAta(alanNo, (borc - alacak) * carp);
            }
        }

        class Sirala : System.Collections.IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 == o2) return 0;
                TNS.MUH.KesinHesapButce i_o1 = (TNS.MUH.KesinHesapButce)o1;
                TNS.MUH.KesinHesapButce i_o2 = (TNS.MUH.KesinHesapButce)o2;
                return (i_o1.hesapKod + i_o1.kod).CompareTo((i_o2.hesapKod + i_o2.kod));
            }
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gelir tertip) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray KesinHesapGelirTertipListele(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH047, "");

            ObjectArray fV = new ObjectArray();

            string kurum = form.kurum;

            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, kurum))
            {
                fV.sonuc.hataStr = ButceMuhasebeResI.MUH048;
                fV.sonuc.islemSonuc = false;
                return fV;
            }

            //Listeleme sırasında kurum kullanıcıları kendilerine ait evrakları görsün
            MUHMuhasebeBirimServis.KullanicininYetkiliOlduguBirimler(kullanan, form);
            if (form.aciklama == "YetkisiYok")
                return fV;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);

            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod = "120;121;122;220;222;181;281;800;810;-80009";//Gelir hesapları
            form.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            IDataReader r = to.KesinHesapBilgiListele(form, false, true, true, (int)ENUMKesinHesapButce.TANIMSIZ);

            if (to.Hata() == "")
            {
                Tertip th = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

                while (r.Read())
                {
                    string eko = VeriErisim.DegerAl(r, 0, "");

                    if (eko != "")
                    {
                        eko = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), eko);
                        eko = eko.Substring(4);
                        //Çoklu hesap kodu gönderip ana hesap kodundan sonra aynı detay hesapların geldiğinden hesap kodunu teke indirmek için 140.01.01.01.01 ve 800.01.01.01.01 geldiğinde 01.01.01.01 tek olsun diye 
                        foreach (string e in fV.objeler)
                        {
                            if (e == eko)
                            {
                                eko = "";
                                break;
                            }
                        }
                    }
                    if (eko != "")
                        fV.objeler.Add(eko);
                }
                r.Close();

                //////////////////////////////////////////
                TABMizanBilanco to2 = new TABMizanBilanco(ve);
                r = to2.KesinGelirButceTertipListele(form.yil);

                if (to2.Hata() == "")
                {
                    th = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

                    while (r.Read())
                    {
                        string eko = VeriErisim.DegerAl(r, 0, "");
                        //Gelir eko kod formatından hesapkodu formatına. Hesapkod formatında noktalama yapıldığı için
                        eko = "000" + eko.Substring(0, 2) + "0" + eko.Substring(2, 1) + "0" + eko.Substring(3, 1) + eko.Substring(4);

                        if (eko != "")
                        {
                            eko = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), eko);
                            //Başındaki ana hesap kodu at
                            eko = eko.Substring(4);
                            //Daha önceden gelen kodlar içinde var ise eklenmesin 
                            foreach (string e in fV.objeler)
                            {
                                if (e == eko)
                                {
                                    eko = "";
                                    break;
                                }
                            }
                        }
                        if (eko != "")
                            fV.objeler.Add(eko);
                    }

                    r.Close();


                    /////////////////////////////////////////
                }
            }
            ve.VeriErisimKapat();
            return fV;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gider) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="kolonDeger">kolon deger</param>
        /// <returns></returns>
        public ObjectArray KesinHesapGiderRakamVer(Kullanici kullanan, OdemeEmriMIFForm form, string[] kolonDeger)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH047, "");

            ObjectArray fV = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);

            for (int i = 0; i < kolonDeger.Length; i++)
            {
                double os = 0.00;
                if (kolonDeger[i] != "")//Kolon listesi gönderildiğinde kolonlardan bir tanesi 
                {
                    ((OdemeEmriMIFDetay)form.detay[0]).hesapKod = kolonDeger[i];
                    form.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });

                    IDataReader r = to.KesinHesapBilgiListele(form, true, false, false, (int)ENUMKesinHesapButce.TANIMSIZ);
                    if (to.Hata() == "")
                    {
                        if (r.Read())
                        {
                            double borc = VeriErisim.DegerAl(r, 0, 0.0);
                            double alacak = VeriErisim.DegerAl(r, 1, 0.0);
                            os = alacak - borc;
                        }
                        r.Close();
                    }
                }
                fV.objeler.Add(os);
            }

            ve.VeriErisimKapat();

            return fV;
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gelir) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray KesinHesapGelirRakamVer(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH047, "");

            ObjectArray fV = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader r = null;

            string eKod = ((OdemeEmriMIFDetay)form.detay[0]).eko.Replace(".", "");
            //Client tarafından eko kod dolu geldi. Ekokod alındıktan sonra sorgulamada kullanılmaması için temizlendi
            ((OdemeEmriMIFDetay)form.detay[0]).eko = "";
            //Gelir Eko koda olarak dönüstür
            string ekoKod = eKod.Substring(0, 2);
            if (eKod.Length > 2)
                ekoKod += eKod.Substring(3, 1);
            if (eKod.Length > 4)
                ekoKod += eKod.Substring(5, 1);
            if (eKod.Length > 6)
                ekoKod += eKod.Substring(6, 2);

            int yil = form.yil;

            //Bütçe Tahmini
            //**********************************
            double tahmin = MUHServis.servisUZY.UzayDegeriDbl(kullanan, "ABSBUTCEGELIR", form.yil + "--" + ekoKod, false, "SUM(KBO)");
            //**********************************

            //Devreden gelir tahakkuku
            //**********************************
            form.yil = yil - 1;
            double devredenTahakkuk = 0;
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod = "120" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "121" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "122" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "220" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "222" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "181" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "281" + eKod;//borç kalanları toplamı
            form.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            r = to.KesinHesapBilgiListele(form, true, false, false, (int)ENUMKesinHesapButce.TANIMSIZ);
            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    double borc = VeriErisim.DegerAl(r, 0, 0.0);
                    double alacak = VeriErisim.DegerAl(r, 1, 0.0);
                    if (borc - alacak > 0)
                        devredenTahakkuk += borc - alacak;
                }
                r.Close();
            }
            //**********************************

            //Toplam tahakkuk
            //**********************************
            form.yil = yil;
            double toplamTahakkuk = 0;
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod = "120" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "121" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "122" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "220" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "222" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "181" + eKod + ";";
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "281" + eKod + ";";
            //((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "14001" + eKod + ";";
            //((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "1400202" + eKod;//borç kalanları toplamı
            form.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            r = to.KesinHesapBilgiListele(form, true, false, false, (int)ENUMKesinHesapButce.TANIMSIZ);
            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    double borc = VeriErisim.DegerAl(r, 0, 0.0);
                    double alacak = VeriErisim.DegerAl(r, 1, 0.0);
                    if (borc - alacak > 0)
                        toplamTahakkuk += borc - alacak;
                }
                r.Close();
            }

            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod = "800" + eKod;//alacak kalanları toplamı
            form.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            r = to.KesinHesapBilgiListele(form, true, false, false, (int)ENUMKesinHesapButce.TANIMSIZ);
            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    double borc = VeriErisim.DegerAl(r, 0, 0.0);
                    double alacak = VeriErisim.DegerAl(r, 1, 0.0);
                    if (alacak - borc > 0)
                        toplamTahakkuk += alacak - borc;
                }
                r.Close();
            }
            //**********************************

            //Yili tahakkuku
            //**********************************
            double yiliTahakkuk = toplamTahakkuk - devredenTahakkuk;
            //**********************************

            //Yili tahsilati
            //**********************************
            form.yil = yil;
            double yiliTahsilat = 0;
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod = "800" + eKod;
            form.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            r = to.KesinHesapBilgiListele(form, true, false, false, (int)ENUMKesinHesapButce.TANIMSIZ);
            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    double borc = VeriErisim.DegerAl(r, 0, 0.0);
                    double alacak = VeriErisim.DegerAl(r, 1, 0.0);
                    yiliTahsilat += alacak - borc;
                }
                r.Close();
            }
            //**********************************

            //Tahsilattan ret ve iadeler
            //**********************************
            form.yil = yil;
            double retIade = 0;
            ((OdemeEmriMIFDetay)form.detay[0]).hesapKod = "810" + eKod;
            form.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            r = to.KesinHesapBilgiListele(form, true, false, false, (int)ENUMKesinHesapButce.TANIMSIZ);
            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    double borc = VeriErisim.DegerAl(r, 0, 0.0);
                    double alacak = VeriErisim.DegerAl(r, 1, 0.0);
                    retIade += borc - alacak;
                }
                r.Close();
            }
            //**********************************

            //Net Tahsilat
            //**********************************
            double yiliNetTahsilat = yiliTahsilat - retIade;
            //**********************************

            //Devredecek gelir tahkkuku
            //**********************************
            //form.yil = yil;
            double devredecekTahakkuk = toplamTahakkuk - yiliNetTahsilat;
            //((OdemeEmriMIFDetay)form.detay[0]).hesapKod = "120" + eKod + ";";
            //((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "121" + eKod + ";";
            //((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "122" + eKod + ";";
            //((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "220" + eKod + ";";
            //((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "222" + eKod + ";";
            //((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "181" + eKod + ";";
            //((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "281" + eKod + ";";
            ////((OdemeEmriMIFDetay)form.detay[0]).hesapKod += "140" + eKod;//borç kalanları toplamı

            //r = to.KesinHesapBilgiListele(form, true, false, false, (int)ENUMKesinHesapButce.TANIMSIZ);
            //if (to.Hata() == "")
            //{
            //    if (r.Read())
            //    {
            //        double borc = VeriErisim.DegerAl(r, 0, 0.0);
            //        double alacak = VeriErisim.DegerAl(r, 1, 0.0);
            //        if (borc - alacak > 0)
            //            devredecekTahakkuk += borc - alacak;
            //    }
            //    r.Close();
            //}
            //**********************************

            //Tahsil Orani
            //**********************************
            double oran = 0;
            if (toplamTahakkuk > 0)
                oran = (yiliNetTahsilat / toplamTahakkuk) * 100;
            //**********************************

            fV.objeler.Add(tahmin);
            fV.objeler.Add(devredenTahakkuk);
            fV.objeler.Add(yiliTahakkuk);
            fV.objeler.Add(toplamTahakkuk);
            fV.objeler.Add(yiliTahsilat);
            fV.objeler.Add(retIade);
            fV.objeler.Add(yiliNetTahsilat);
            fV.objeler.Add(devredecekTahakkuk);
            fV.objeler.Add(oran);

            ve.VeriErisimKapat();

            return fV;
        }

        internal static AvansHareket[] AvansSorgula(VeriErisim veriErisim, Kullanici kullanan, AvansSorguKriter form)
        {
            string maske = HesapPlaniYapiBul(0);
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMizanBilanco tabMuhasebe = new TABMizanBilanco(ve);

            IDataReader dr = null;
            if (form.detay)
                dr = tabMuhasebe.AvansSorgulaDetay(form);
            else
                dr = tabMuhasebe.AvansSorgula(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            List<AvansHareket> hler = new List<AvansHareket>();
            int i = 0;
            while (dr.Read())
            {
                AvansHareket ah = new AvansHareket();
                int sonTarihAlanNo = 0;
                if (form.detay)
                {
                    ah.sira = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                    ah.sonTarih = VeriErisim.DegerAl(dr, sonTarihAlanNo++);
                    if (form.tertipli)
                    {
                        string kur = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                        string fon = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                        string fin = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                        string eko = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                        string peb = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                        ah.tertip = kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                    }
                    ah.adSoyadUnvan = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                }
                else
                {
                    ah.sira = (++i).ToString();
                    ah.tCVno = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                    if (!string.IsNullOrWhiteSpace(ah.tCVno))
                    {
                        IlgiliBilgi ib = BankaOdemeListe.IlgiliBilgiBul(ve2, kullanan, ah.tCVno, form.saymanlikKodu);
                        if (ib != null)
                            ah.adSoyadUnvan = ib.ilgili;
                    }
                    ah.hesapNo = OrtakFonksiyonlar.KodNoktaKoy(maske, VeriErisim.DegerAl(dr, sonTarihAlanNo++, ""));
                    ah.hesapAdi = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");

                    if (form.tertipli)
                    {
                        string kur = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                        string fon = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                        string fin = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                        string eko = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                        string peb = VeriErisim.DegerAl(dr, sonTarihAlanNo++, "");
                        ah.tertip = kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                    }

                    ah.sonTarih = VeriErisim.DegerAl(dr, sonTarihAlanNo++);
                }
                ah.borc = VeriErisim.DegerAl(dr, sonTarihAlanNo++, (double)0);
                ah.alacak = VeriErisim.DegerAl(dr, sonTarihAlanNo++, (double)0);
                hler.Add(ah);
            }
            return hler.ToArray();
        }

        internal static ObjectArray TaahhutListele(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            ObjectArray oa = new ObjectArray();
            if (form.detay.ObjeSayisi == 0)
                oa.sonuc.DegerAta("Lüften hesap kodu seçiniz");

            OdemeEmriMIFDetay d = (OdemeEmriMIFDetay)form.detay.objeler[0];

            if (string.IsNullOrWhiteSpace(d.hesapKod))
                oa.sonuc.DegerAta("Lüften hesap kodu seçiniz");

            HesapPlaniSatir hesPlani = new HesapPlaniSatir();
            hesPlani.yil = form.yil;
            hesPlani.hesapKod = d.hesapKod;
            hesPlani = new MUHHesapPlaniServis().OzellikAl(null, hesPlani);

            if (string.IsNullOrWhiteSpace(hesPlani.aciklama))
                oa.sonuc.DegerAta("Lüften geçerli bir hesapkodu seçiniz");

            if (!hesPlani.detay)
                oa.sonuc.DegerAta("Lüften detay hesap kodu seçiniz.");

            if (hesPlani.BorcAlacakCalisirMi())
                oa.sonuc.DegerAta("Lüften Borç bakiye verir yada Alacak bakiye verir işaretlenmiş bir hesap kodu seçiniz.");

            string kurum = form.kurum;
            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODEMEEMRI, kurum))
                oa.sonuc.DegerAta(ButceMuhasebeResI.MUH048);

            MUHMuhasebeBirimServis.KullanicininYetkiliOlduguBirimler(kullanan, form);
            if (form.aciklama == "YetkisiYok")
                oa.sonuc.DegerAta("YetkisiYok");
            if (!oa.sonuc.islemSonuc) return oa;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray tcCikis = null;
            ObjectArray tcGiris = TaahhutDetayListele(ve, form, hesPlani, true);
            if (tcGiris.sonuc.islemSonuc)
            {
                tcCikis = TaahhutDetayListele(ve, form, hesPlani, false);
                if (!tcCikis.sonuc.islemSonuc)
                {
                    oa.sonuc = tcCikis.sonuc;
                }
            }
            else
                oa.sonuc = tcGiris.sonuc;
            if (oa.sonuc.islemSonuc)
            {
                oa.Ekle(hesPlani);
                oa.Ekle(tcGiris.objeler);
                oa.Ekle(tcCikis.objeler);
            }
            ve.VeriErisimKapat();
            return oa;
        }

        private static ObjectArray TaahhutDetayListele(VeriErisim ve, OdemeEmriMIFForm form, HesapPlaniSatir hesPlani, bool acilis)
        {
            OdemeEmriMIFDetay d = (OdemeEmriMIFDetay)form.detay.objeler[0];
            d.borc = 0;
            d.alacak = 0;
            if (hesPlani.BorcCalisirMi())
            {
                if (acilis) d.borc = 1;
                else d.alacak = 1;
            }
            else
            {
                if (acilis) d.alacak = 1;
                else d.borc = 1;
            }

            Tertip t = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
            Tertip th = new Tertip(form.yil, ENUMTabloKod.TABOdemeEmri);
            ObjectArray oa = new ObjectArray();
            TABOdemeEmriMIF to = new TABOdemeEmriMIF(ve);
            IDataReader r = to.FormListe(form, true, false, false);
            if (r != null)
            {
                while (r.Read())
                {
                    BuyukDefter os = new BuyukDefter();

                    os.yil = VeriErisim.DegerAl(r, 0, 0);
                    os.belgeTur = VeriErisim.DegerAl(r, 1, 0);
                    os.belgeNo = VeriErisim.DegerAl(r, 2, "");
                    os.kurum = VeriErisim.DegerAl(r, 3, "");
                    os.muhasebe = VeriErisim.DegerAl(r, 4, 0);
                    os.birim = VeriErisim.DegerAl(r, 5, "");
                    os.ilgiliAd = VeriErisim.DegerAl(r, 6, "");
                    //os.ilgiliNo = VeriErisim.DegerAl(r, 7, "");
                    //os.ilgiliBankaAd = VeriErisim.DegerAl(r, 8, "");
                    //os.ilgiliBankaNo = VeriErisim.DegerAl(r, 9, "");
                    //os.ilgiliVD = VeriErisim.DegerAl(r, 10, "");
                    os.aciklama = VeriErisim.DegerAl(r, 11, "");
                    os.islemTarih = VeriErisim.DegerAl(r, 12);
                    os.formDurumTarih = VeriErisim.DegerAl(r, 13);
                    os.formDurum = VeriErisim.DegerAl(r, 14, 0);
                    //os.iptalNeden = VeriErisim.DegerAl(r, 15, "");
                    os.YEVYevTarih = VeriErisim.DegerAl(r, 16);
                    os.YEVYevNo = VeriErisim.DegerAl(r, 17, 0);

                    os.kurum = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), os.kurum);
                    os.islemYapan = VeriErisim.DegerAl(r, 30, "");
                    //os.projeNo = VeriErisim.DegerAl(r, 31, "");
                    //os.ihale = VeriErisim.DegerAl(r, 32, "");
                    //os.fisTur = VeriErisim.DegerAl(r, 33, 0);

                    os.kurumAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KURAD, "");
                    os.muhasebeAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.MUHASEBEAD, "");
                    os.satirno = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.SIRA, 0);
                    os.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.HESAPKOD, ""));
                    os.kur = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KURKOD, "");
                    os.fon = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FONKOD, "");
                    os.fin = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FINKOD, "");
                    os.eko = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.EKOKOD, "");
                    if (os.kur != "")
                    {
                        os.kur = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), os.kur);
                        os.fon = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(1), os.fon);
                        os.fin = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(2), os.fin);
                        os.eko = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(3), os.eko);

                        os.prgKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.PRGKOD, 0);
                        os.aPrgKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.APRGKOD, 0);
                        os.faalKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.FAALKOD, 0);
                        os.aFaalKod = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.AFAALKOD, 0);
                    }

                    os.borc = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.BORC, 0.0);
                    os.alacak = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALACAK, 0.0);
                    if (form.sorguDovizliIslemler)
                    {
                        os.borc = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZBORC, 0.0);
                        os.alacak = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.DOVIZALACAK, 0.0);
                    }

                    os.hesapAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.HESAPPLANI_ACIKLAMA, "");
                    string satirAciklama = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ACIKLAMA, "");
                    if (satirAciklama != "") satirAciklama += " - ";
                    os.aciklama = satirAciklama + os.aciklama;

                    os.altHesap = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALTHESAPKOD, "");
                    os.altHesapAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALTHESAPAD1, "");
                    if (string.IsNullOrEmpty(os.altHesapAd))
                        os.altHesapAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.ALTHESAPAD2, "");


                    string kapatan = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPATAN, "");
                    string kapatilan = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPATILAN, "");
                    string[] kapatanB = kapatan.Split('^');
                    string[] kapatilanB = kapatilan.Split('^');

                    if (kapatanB.Length > 2)
                        os.kapattigiBelgeNo = kapatanB[2];// VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler2.KAPANISBELGENO2, "");

                    oa.Ekle(os);
                }
                r.Close();
            }
            else
                oa.sonuc.DegerAta(ve.hata);
            return oa;
        }
    }
}

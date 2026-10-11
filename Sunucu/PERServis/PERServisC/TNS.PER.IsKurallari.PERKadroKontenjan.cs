using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PER.IsKurallari.Veri;
using System.Collections.Generic;
using TNS.MUH;
using TNS.ORG;

namespace TNS.PER.IsKurallari
{
    public class PERKadroKontenjan
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjan kk)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (kk.orgBirimKod <= 0 && kk.mb == null)
                hata = PersonelResI.PER001 + "<br>";
            if ((int)kk.islemTur <= 0)
                hata += PersonelResI.PER002;
            string kontenjanHata = KontenjanDegerleriniKontrolEt(kk);
            if (!string.IsNullOrEmpty(kontenjanHata))
                hata += "<br>" + kontenjanHata;
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadroKontenjan tabKK = new TABKadroKontenjan(ve);
            ve.BeginTransaction();

            kk.islemTarih = new TNSDateTime(DateTime.Now);

            if (kk.siraNo > 0)
            {
                KadroKontenjanDetay detay = new KadroKontenjanDetay();
                detay.orgBirimKod = kk.orgBirimKod;
                detay.mb = kk.mb;
                detay.siraNo = kk.siraNo;

                sonuc = PERKadroKontenjanDetay.Sil(ve, kullanan, detay);
                if (sonuc.islemSonuc)
                    sonuc = tabKK.Sil(kk);
            }

            if (sonuc.islemSonuc)
                sonuc = tabKK.Kaydet(kk);

            if (sonuc.islemSonuc)
            {
                foreach (KadroKontenjanDetay detay in kk.detaylar)
                {
                    detay.orgBirimKod = kk.orgBirimKod;
                    detay.mb = kk.mb;
                    detay.siraNo = kk.siraNo;
                }
                sonuc = PERKadroKontenjanDetay.Kaydet(ve, kullanan, kk.detaylar);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = kk.siraNo.ToString();
                sonuc.bilgiStr = PersonelResI.PER007;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PersonelResI.PER008 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjan kk)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (kk.orgBirimKod <= 0 && kk.mb == null)
                hata = PersonelResI.PER001 + "<br>";
            if (kk.siraNo <= 0)
                hata += PersonelResI.PER004;
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            if (!Yetki.YetkiVarMiYazmaya(kullanan, (string)null))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER012;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadroKontenjan tKK = new TABKadroKontenjan(ve);
            ve.BeginTransaction();

            KadroKontenjanDetay detay = new KadroKontenjanDetay();
            detay.orgBirimKod = kk.orgBirimKod;
            detay.mb = kk.mb;
            detay.siraNo = kk.siraNo;

            sonuc = PERKadroKontenjanDetay.Sil(ve, kullanan, detay);
            if (sonuc.islemSonuc)
                sonuc = tKK.Sil(kk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER009;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PersonelResI.PER010 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static KadroKontenjan[] Listele(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjan kk, bool detay)
        {
            if (!Yetki.YetkiVarMiOkumaya(kullanan))
                throw new Exception(PersonelResI.PER013);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadroKontenjan tKK = new TABKadroKontenjan(ve);

            IDataReader dr = null;
            try
            {
                dr = tKK.Listele(kk);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            VeriErisim ve2 = null;
            if (detay)
                ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection kklar = new TNSCollection();
            while (dr.Read())
            {
                KadroKontenjan gelen = new KadroKontenjan();
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.ORGBIRIMKOD, 0);
                if (gelen.orgBirimKod > 0)
                {
                    OrganizasyonBirim orgBirim = TNS.ORG.Arac.OrgBirimBul(gelen.orgBirimKod);
                    gelen.orgBirimAd = orgBirim.adi;// servisUZY.UzayDegeriStr(kullanan, "ORGBIRIM", birim, true);
                    //gelen.orgBirimAd = PERServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                }
                gelen.mb = new MuhasebeBirim();
                gelen.mb.kurum = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.KURKOD, string.Empty);
                gelen.mb.muhasebe = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.MUHASEBEKOD, 0);
                gelen.mb.birimKod = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.BIRIMKOD, string.Empty);
                if (!string.IsNullOrEmpty(gelen.mb.birimKod))
                    gelen.mb.birimAd = PERServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", gelen.mb.birimKod + "-" + gelen.mb.muhasebe + "-" + gelen.mb.kurum, true, string.Empty);
                gelen.siraNo = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.SIRANO, 0);
                gelen.islemTur = (EnumKadroKontenjanIslemTur)System.Enum.Parse(typeof(EnumKadroKontenjanIslemTur), VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.ISLEMTUR, 0).ToString());
                gelen.islemTarih = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.ISLEMTARIH);
                gelen.aciklama = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.ACIKLAMA, string.Empty);
                gelen.dayanakBelgeTarih = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.DAYANAKBELGETARIH);
                gelen.dayanakBelgeNo = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.DAYANAKBELGENO, string.Empty);

                if (detay)
                {
                    try
                    {
                        KadroKontenjanDetay kkDetay = new KadroKontenjanDetay();
                        kkDetay.orgBirimKod = gelen.orgBirimKod;
                        kkDetay.mb = gelen.mb;
                        kkDetay.siraNo = gelen.siraNo;

                        gelen.detaylar = PERKadroKontenjanDetay.Listele(ve2, kullanan, kkDetay);
                    }
                    catch (Exception)
                    {
                    }

                    if (gelen.islemTur == EnumKadroKontenjanIslemTur.Tenkis && gelen.detaylar != null)
                    {
                        foreach (KadroKontenjanDetay kkDetay in gelen.detaylar)
                            kkDetay.kontenjan = -kkDetay.kontenjan;
                    }
                }

                kklar.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            if (ve2 != null)
                ve2.VeriErisimKapat();
            return (KadroKontenjan[])kklar.ToArray(typeof(KadroKontenjan));
        }

        private static string KontenjanDegerleriniKontrolEt(KadroKontenjan kk)
        {
            string hata = null;
            int toplam = 0;
            foreach (KadroKontenjanDetay detay in kk.detaylar)
            {
                if (kk.islemTur != EnumKadroKontenjanIslemTur.Degisiklik && detay.kontenjan < 0)
                {
                    hata = PersonelResI.PER014;
                    break;
                }
                else if (kk.islemTur == EnumKadroKontenjanIslemTur.Tenkis)
                    detay.kontenjan = -detay.kontenjan;
                else if (kk.islemTur == EnumKadroKontenjanIslemTur.Degisiklik)
                    toplam += detay.kontenjan;
            }
            if (toplam != 0)
                hata = PersonelResI.PER015;
            return hata;
        }
    }
}
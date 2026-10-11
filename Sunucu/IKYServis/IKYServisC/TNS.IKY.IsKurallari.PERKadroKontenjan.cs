using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.IKY.IsKurallari.Veri;
using System.Collections.Generic;
using TNS.MUH;
using TNS.ORG;

namespace TNS.IKY.IsKurallari
{
    public class PERKadroKontenjan
    {
        private static int UnvanTekrariVarMi(KadroKontenjanDetay[] detaylar)
        {
            for (int i = 0; i < detaylar.Length; i++)
            {
                KadroKontenjanDetay detay1 = (KadroKontenjanDetay)detaylar[i];
                for (int j = i + 1; j < detaylar.Length; j++)
                {
                    KadroKontenjanDetay detay2 = (KadroKontenjanDetay)detaylar[j];
                    if (detay1.unvanKod == detay2.unvanKod && detay1.kadroDerece == detay2.kadroDerece)
                        return detay1.unvanKod;
                }
            }
            return 0;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjan kk, bool icCagri)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;

            if (icCagri && kk.organizasyonKod <= 0)
                hata = "Organizasyon seçilmemiş.<br>";

            OrganizasyonBirim hrcOrg = IKYPersonelKadro.ORGUstHarcamaBirimBul(kk.organizasyonKod);
            kk.organizasyonKod = hrcOrg.kod;
            if ((int)kk.islemTur <= 0)
                hata += PersonelResI.PER002;

            if (kk.detaylar == null || kk.detaylar.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER003;
                return sonuc;
            }
            string kontenjanHata = KontenjanDegerleriniKontrolEt(kk);
            if (!string.IsNullOrEmpty(kontenjanHata))
                hata += "<br>" + kontenjanHata;


            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = IKYYetki.YetkiVarMiYazmaya(kullanan, hrcOrg.kopruKod, 0, sonuc, ENUMYetkiIslem.KadroKontenjan);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadroKontenjan tabKK = new TABKadroKontenjan(ve);
            TABKadroKontenjanDetay tKKDetay = new TABKadroKontenjanDetay(ve);
            ve.BeginTransaction();

            kk.islemTarih = new TNSDateTime(DateTime.Now);
            if (kk.kod == 0)
                kk.kod = TABGenel.SayacDegerAlInt(ve, TABKadroKontenjan.TABLOAD);

            sonuc = tabKK.Kaydet(kk);

            if (sonuc.islemSonuc && kk.detaylar.Length > 0)
            {
                tKKDetay.Sil(new KadroKontenjanDetay { kkod = kk.kod });

                foreach (KadroKontenjanDetay detay in kk.detaylar)
                {
                    if (detay.kontenjan == 0)
                        continue;

                    detay.kkod = kk.kod;
                    sonuc = tKKDetay.Kaydet(detay);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = kk.kod.ToString();
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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KadroKontenjan kk, bool yetkiKontrolYapma)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (kk.kod <= 0)
                hata += " Kadro kontenjan kodu yok.";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }
            if (!yetkiKontrolYapma)
            {
                sonuc = IKYYetki.YetkiVarMiYazmaya(kullanan, null, 0, sonuc, ENUMYetkiIslem.KadroKontenjan);
                if (!sonuc.islemSonuc)
                {
                    return sonuc;
                }
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadroKontenjan tKK = new TABKadroKontenjan(ve);
            TABKadroKontenjanDetay tKKDetay = new TABKadroKontenjanDetay(ve);
            ve.BeginTransaction();

            sonuc = tKKDetay.Sil(new KadroKontenjanDetay { kkod = kk.kod });
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
            //Sonuc s = IKYYetki.YetkiVarMiOkumaya(kullanan, null, null, ENUMYetkiIslem.KadroKontenjan);
            //if (!s.islemSonuc)
            //    throw new Exception(PersonelResI.PER013);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKadroKontenjan tKK = new TABKadroKontenjan(ve);

            IDataReader dr = tKK.Listele(kk);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            VeriErisim ve2 = null;
            if (detay)
                ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection kklar = new TNSCollection();
            while (dr.Read())
            {
                KadroKontenjan gelen = new KadroKontenjan();
                gelen.kod = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.KOD, 0);
                gelen.organizasyonKod = VeriErisim.DegerAl(dr, (int)TABKadroKontenjan.ENUMAlanlar.ORGANIZASYONKOD, 0);
                if (gelen.organizasyonKod > 0)
                {
                    OrganizasyonBirim orgBirim = TNS.ORG.Arac.OrgBirimBul(gelen.organizasyonKod);
                    gelen.organizasyonAd = orgBirim.adi;// servisUZY.UzayDegeriStr(kullanan, "ORGBIRIM", birim, true);
                    //gelen.orgBirimAd = IKYServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                }

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
                        kkDetay.kkod = gelen.kod;
                        //kkDetay.mb = gelen.mb;
                        //kkDetay.siraNo = gelen.siraNo;

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
            int tekrarlayanUnvan = UnvanTekrariVarMi(kk.detaylar);
            if (tekrarlayanUnvan > 0)
                return "Tekrarlayan unvan var!";

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
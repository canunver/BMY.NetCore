using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    public class PEBFaaliyetProjeHarcamaYansiyan
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray agirlik = AgirlikGetir(ve, kullanan, yil, "0600");
            if (!agirlik.sonuc.islemSonuc || agirlik.objeler.Count <= 0)
                return agirlik.sonuc;

            ObjectArray harcama = SabitMaliyetHarcamaGetir(ve, kullanan, yil, ay);
            if (!harcama.sonuc.islemSonuc || harcama.objeler.Count <= 0)
                return harcama.sonuc;

            TABFaaliyetProjeHarcamaYansiyan tYansiyan = new TABFaaliyetProjeHarcamaYansiyan(ve);
            ve.BeginTransaction();

            //Bir sabit maliyet kaç defa kullanılmış onu hesaplayıp
            //Hashtable'a atıyor
            System.Collections.Hashtable sayac = new System.Collections.Hashtable();
            foreach (SabitMaliyetAgirlik sma in agirlik.objeler)
            {
                if (sayac.ContainsKey(sma.smKurKod + "-" + sma.smKod))
                    sayac[sma.smKurKod + "-" + sma.smKod] = (int)sayac[sma.smKurKod + "-" + sma.smKod] + 1;
                else
                    sayac.Add(sma.smKurKod + "-" + sma.smKod, 1);
            }

            //Eski kayıtlar siliniyor
            tYansiyan.Sil(yil, ay);

            System.Collections.Hashtable harcamaKayitli = new System.Collections.Hashtable();
            System.Collections.Hashtable sayacKayitli = new System.Collections.Hashtable();
            foreach (Harcama h in harcama.objeler)
            {
                foreach (SabitMaliyetAgirlik sma in agirlik.objeler)
                {
                    if (h.maliyetKurKod == sma.smKurKod && h.maliyetKod == sma.smKod)
                    {
                        FaaliyetProjeHarcamaYansiyan yansiyan = new FaaliyetProjeHarcamaYansiyan();
                        yansiyan.yil = yil;
                        yansiyan.ay = ay;
                        yansiyan.fp.kurKod = sma.fpKurKod;
                        yansiyan.fp.fpKod = sma.fpKod;
                        yansiyan.sm.kurKod = sma.smKurKod;
                        yansiyan.sm.smKod = sma.smKod;
                        yansiyan.agirlik = sma.agirlik;

                        int sayacKayitEdilenen = 0;
                        if (sayacKayitli.ContainsKey(sma.smKurKod + "-" + sma.smKod))
                            sayacKayitEdilenen = (int)sayacKayitli[sma.smKurKod + "-" + sma.smKod];

                        double harcamaKayitEdilenen = 0;
                        if (harcamaKayitli.ContainsKey(sma.smKurKod + "-" + sma.smKod))
                            harcamaKayitEdilenen = (double)harcamaKayitli[sma.smKurKod + "-" + sma.smKod];

                        if ((int)sayac[sma.smKurKod + "-" + sma.smKod] == sayacKayitEdilenen + 1)
                            yansiyan.harcama = h.harcama - harcamaKayitEdilenen;
                        else
                            yansiyan.harcama = Math.Round(h.harcama * yansiyan.agirlik / 100);
                        
                        sonuc = tYansiyan.Kaydet(yansiyan);
                        if (!sonuc.islemSonuc)
                            break;

                        if (sayacKayitli.ContainsKey(sma.smKurKod + "-" + sma.smKod))
                            sayacKayitli[sma.smKurKod + "-" + sma.smKod] = (int)sayacKayitli[sma.smKurKod + "-" + sma.smKod] + 1;
                        else
                            sayacKayitli.Add(sma.smKurKod + "-" + sma.smKod, 1);

                        if (harcamaKayitli.ContainsKey(sma.smKurKod + "-" + sma.smKod))
                            harcamaKayitli[sma.smKurKod + "-" + sma.smKod] = (double)harcamaKayitli[sma.smKurKod + "-" + sma.smKod] + yansiyan.harcama;
                        else
                            harcamaKayitli.Add(sma.smKurKod + "-" + sma.smKod, yansiyan.harcama);
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kayıt işlemi başarıyla gerçekleştirildi.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt işlemi gerçekleştirilemedi.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, FaaliyetProjeHarcamaYansiyan yk)
        {
            ObjectArray yansiyanVAN = new ObjectArray();
            yansiyanVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, yk.sm.kurKod);
            if (!yansiyanVAN.sonuc.islemSonuc)
                return yansiyanVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABFaaliyetProjeHarcamaYansiyan tYansiyan = new TABFaaliyetProjeHarcamaYansiyan(ve);

            IDataReader dr = tYansiyan.Listele(yk);
            while (dr.Read())
            {
                FaaliyetProjeHarcamaYansiyan gelen = new FaaliyetProjeHarcamaYansiyan();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.ay = VeriErisim.DegerAl(dr, 1, 0);
                gelen.fp.kurKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.fp.kurKod = PEBGenel.KurKodNoktala(gelen.fp.kurKod);
                gelen.fp.fpKod = VeriErisim.DegerAl(dr, 3, 0);
                gelen.sm.kurKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.sm.kurKod = PEBGenel.KurKodNoktala(gelen.sm.kurKod);
                gelen.sm.smKod = VeriErisim.DegerAl(dr, 5, 0);
                gelen.agirlik = VeriErisim.DegerAl(dr, 6, (double)0);
                gelen.harcama = VeriErisim.DegerAl(dr, 7, (double)0);

                yansiyanVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (yansiyanVAN.objeler.Count == 0)
                yansiyanVAN.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";

            ve.VeriErisimKapat();
            return yansiyanVAN;
        }

        private static ObjectArray AgirlikGetir(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod)
        {
            ObjectArray smAgirlikVAN = new ObjectArray();

            string hata = string.Empty;
            if (yil <= 0)
                hata = "Yıl boş bırakılamaz.<br>";
            if (string.IsNullOrEmpty(senaryoKod))
                hata += "Senaryo boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                smAgirlikVAN.sonuc.islemSonuc = false;
                smAgirlikVAN.sonuc.hataStr = hata;
                return smAgirlikVAN;
            }

            SabitMaliyetAgirlik sma = new SabitMaliyetAgirlik();
            sma.yil = yil;
            sma.senaryoKod = senaryoKod;
            smAgirlikVAN = PEBSabitMaliyetAgirlik.Listele(veriErisim, kullanan, sma);

            return smAgirlikVAN;
        }

        private static ObjectArray SabitMaliyetHarcamaGetir(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay)
        {
            ObjectArray smhVAN = new ObjectArray();

            string hata = string.Empty;
            if (yil <= 0)
                hata = "Yıl boş bırakılamaz.<br>";
            if (ay < 0)
                hata += "Ay boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                smhVAN.sonuc.islemSonuc = false;
                smhVAN.sonuc.hataStr = hata;
                return smhVAN;
            }

            smhVAN = PEBHarcama.SabitMaliyetHarcamaToplamlari(veriErisim, kullanan, yil, ay);

            return smhVAN;
        }
    }
}
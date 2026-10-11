using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;
using TNS.DEG;
using TNS.BKD;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class TMMUretimRecetesi
    {
        private static Sonuc KaydetKontroluYap(Kullanici kullanan, UretimRecetesi tk)
        {
            Sonuc sonuc = new Sonuc();
            HesapPlaniSatir hs = new HesapPlaniSatir();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(tk.anaUrun.hesapKod))
                hata += "Üretilecek Malzeme boş olamaz<br>";
            else
            {
                TMMHesapPlani hp = new TMMHesapPlani();
                hs = new HesapPlaniSatir();
                hs.hesapKod = tk.anaUrun.hesapKod.Trim();
                ObjectArray bilgiHp = hp.Listele(kullanan, hs, new Sayfalama());
                if (bilgiHp.ObjeSayisi < 1)
                    hata += tk.anaUrun.hesapKod.Trim() + " kodlu malzeme bulunamadı<br>";
                else
                {
                    hs = (HesapPlaniSatir)bilgiHp[0];
                    if (!hs.detay)
                        hata += tk.anaUrun.hesapKod.Trim() + " kodlu malzeme detayında başka malzemeler olduğu için kullanılamaz<br>";
                }
            }
            if (string.IsNullOrEmpty(tk.altUrun.hesapKod))
                hata += "Tüketilecek malzeme boş olamaz<br>";
            else
            {
                TMMHesapPlani hp = new TMMHesapPlani();
                hs = new HesapPlaniSatir();
                hs.hesapKod = tk.altUrun.hesapKod.Trim();
                ObjectArray bilgiHp = hp.Listele(kullanan, hs, new Sayfalama());
                if (bilgiHp.ObjeSayisi < 1)
                    hata += tk.altUrun.hesapKod.Trim() + " kodlu malzeme bulunamadı<br>";
                else
                {
                    hs = (HesapPlaniSatir)bilgiHp[0];
                    if (!hs.detay)
                        hata += tk.altUrun.hesapKod.Trim() + " kodlu malzeme detayında başka malzemeler olduğu için kullanılamaz<br>";
                }
            }
            if (tk.miktar <= 0)
                hata += "Miktar alanı 0 dan büyük olmalıdır" + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc();

            foreach (UretimRecetesi item in liste.objeler)
            {
                sonuc = KaydetKontroluYap(kullanan, item);
                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABUretimRecetesi tabTK = new TABUretimRecetesi(ve);
            ve.BeginTransaction();

            try
            {
                string anaUrunKod = ((UretimRecetesi)liste[0]).anaUrun.hesapKod;

                sonuc = tabTK.Sil(anaUrunKod);

                foreach (UretimRecetesi item in liste.objeler)
                {
                    item.kod = Guid.NewGuid().ToString();
                    item.anaUrun.hesapKod = item.anaUrun.hesapKod.Replace(".", "");
                    item.altUrun.hesapKod = item.altUrun.hesapKod.Replace(".", "");

                    sonuc = tabTK.Kaydet(item);

                    if (!sonuc.islemSonuc)
                    {
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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, UretimRecetesi tk)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABUretimRecetesi tabTK = new TABUretimRecetesi(ve);
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

            int sonKolon = (int)TABUretimRecetesi.ENUMAlanlar.MIKTAR;
            while (dr.Read())
            {
                UretimRecetesi gelen = new UretimRecetesi();
                gelen.kod = VeriErisim.DegerAl(dr, (int)TABUretimFormu.ENUMAlanlar.KOD, string.Empty);

                gelen.anaUrun.hesapKod = VeriErisim.DegerAl(dr, (int)TABUretimRecetesi.ENUMAlanlar.ANAURUNHESAPKOD, "");
                gelen.altUrun.hesapKod = VeriErisim.DegerAl(dr, (int)TABUretimRecetesi.ENUMAlanlar.HESAPPLANKOD, "");
                gelen.miktar = VeriErisim.DegerAl(dr, (int)TABUretimRecetesi.ENUMAlanlar.MIKTAR, 0.0);

                gelen.anaUrun.aciklama = VeriErisim.DegerAl(dr, sonKolon + 1, "");
                gelen.altUrun.aciklama = VeriErisim.DegerAl(dr, sonKolon + 2, "");

                gelen.anaUrun.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.anaUrun.hesapKod);
                gelen.altUrun.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.altUrun.hesapKod);

                donusDeger.objeler.Add(gelen);
            }
            dr.Close();

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();

            return donusDeger;
        }
    }
}
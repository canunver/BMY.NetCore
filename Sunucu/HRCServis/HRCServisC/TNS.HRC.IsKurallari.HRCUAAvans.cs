using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.HRC.IsKurallari.Veri;
using System.Data;

namespace TNS.HRC.IsKurallari
{
    public class UAAvans
    {
        internal static Sonuc TalepKaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, UAAvansTalep bilgi)
        {
            Sonuc s = YetkiKontrol(kullanan, bilgi);
            if (!s.islemSonuc)
                return s;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            UAAvansTalep okunan = TalepOku(ve, kullanan, bilgi);
            TABUAAvans tab = new TABUAAvans(ve);

            ve.BeginTransaction();

            if (okunan == null) // yeni kayıtsa ekle
                s = tab.Ekle(bilgi);
            else
                s = tab.Guncelle(bilgi);// onceki kayıtsa güncelle
            if (s.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        public static UAAvansTalep TalepOku(VeriErisim veriErisim, KYM.Kullanici kullanan, UAAvansTalep bilgi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray objArr = new ObjectArray();
            Sonuc son = YetkiKontrol(kullanan, bilgi);
            TABUAAvans tb = new TABUAAvans(ve);

            IDataReader dr = tb.Oku(bilgi);
            UAAvansTalep ddeger = null;
            if (dr != null)
            {
                if (dr.Read())
                {
                    ddeger = new UAAvansTalep();
                    ddeger.surecYil = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.SURECYIL, (int)0);
                    ddeger.surecNo = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.SURECNO, "");
                    ddeger.sayi = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.SAYI, "");
                    ddeger.gerekce = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.GEREKCE, "");
                    ddeger.miktar = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.MIKTAR, (int)0);
                    ddeger.banka = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.BANKA, "");
                    ddeger.ibanNo = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.IBANNO, "");
                    ddeger.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.TCKIMLIKNO, "");
                    ddeger.koordinator = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.KOORDINATOR, "");
                    ddeger.onaylayan = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.ONAYLAYAN, "");
                    ddeger.birimKod = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.BIRIMKOD, "");
                    ddeger.saymanlikKod = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.SAYMANLIKKOD, (int)0);
                    ddeger.olurTarihi = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.OLURTARIHI);
                    ddeger.koordinatorUnvan = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.KOORDINATORUNVAN, "");
                    ddeger.onaylayanUnvan = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.ONAYLAYANUNVAN, "");
                    ddeger.odemeEmriNo = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.ODEMEEMRINO, "");
                    ddeger.durum = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMAlanlar.DURUM, (int)0);

                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return ddeger;
        }

        private static Sonuc YetkiKontrol(KYM.Kullanici kullanan, UAAvansTalep bilgi)
        {
            return new Sonuc(true, "", "");
        }

        private static Sonuc YetkiKontrol(KYM.Kullanici kullanan, UAAvansKapatma bilgi)
        {
            return new Sonuc(true, "", "");
        }

        internal static Sonuc OEIliskiKur(VeriErisim veriErisim, KYM.Kullanici kullanan, int surecYil, string surecNo, string odemeEmriNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            UAAvansTalep bilgi = new UAAvansTalep();
            bilgi.surecYil = surecYil;
            bilgi.surecNo = surecNo;


            UAAvansTalep okunan = TalepOku(ve, kullanan, bilgi);
            okunan.odemeEmriNo = odemeEmriNo;

            TABUAAvans tab = new TABUAAvans(ve);

            ve.BeginTransaction();
            Sonuc s = tab.OEIliskiKur(okunan);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc KapatmaOEIliskiKur(VeriErisim veriErisim, KYM.Kullanici kullanan, int surecYil, string surecNo, string odemeEmriNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            UAAvansKapatma bilgi = new UAAvansKapatma();
            bilgi.surecYil = surecYil;
            bilgi.surecNo = surecNo;

            UAAvansKapatma okunan = AvansKapatmaOku(ve, kullanan, bilgi);
            okunan.odemeEmriNo = odemeEmriNo;

            TABUAAvans tab = new TABUAAvans(ve);

            ve.BeginTransaction();
            Sonuc s = tab.KapatmaOEIliskiKur(okunan);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc KapatmaKalanOEIliskiKur(VeriErisim veriErisim, KYM.Kullanici kullanan, int surecYil, string surecNo, string kapatmaOdemeEmriNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            UAAvansKapatma bilgi = new UAAvansKapatma();
            bilgi.surecYil = surecYil;
            bilgi.surecNo = surecNo;

            UAAvansKapatma okunan = AvansKapatmaOku(ve, kullanan, bilgi);
            okunan.kapatmaOdemeEmriNo = kapatmaOdemeEmriNo;

            TABUAAvans tab = new TABUAAvans(ve);

            ve.BeginTransaction();
            Sonuc s = tab.KapatmaKalanOEIliskiKur(okunan);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }
        

        internal static double FaturaTopla(VeriErisim veriErisim, KYM.Kullanici kullanan, int surecYil, string surecNo)
        {
            VeriErisim ver = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFaturaForm f = new TABFaturaForm(ver);
            double d = f.ToplamAl(surecYil, surecNo);
            ver.VeriErisimKapat();
            return d;
        }

        internal static Sonuc TalepKapat(VeriErisim veriErisim, KYM.Kullanici kullanan, UAAvansKapatma bilgi)
        {
            Sonuc s = YetkiKontrol(kullanan, bilgi);
            if (!s.islemSonuc)
                return s;
            if (bilgi.tertipler == null || bilgi.tertipler.Count == 0)
            {
                s.DegerAta("Tertip listesi boş olamaz!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            UAAvansKapatma okunan = AvansKapatmaOku(ve, kullanan, bilgi);
            TABUAAvans tab = new TABUAAvans(ve);

            ve.BeginTransaction();

            if (okunan == null) // yeni kayıtsa ekle
                s = tab.KapatmaEkle(bilgi);

            if (s.islemSonuc)
                tab.KapatmaTertipSil(bilgi);

            for (int i = 0; i < bilgi.tertipler.Count && s.islemSonuc; i++)
            {
                s=tab.KapatmaTertipEkle(bilgi, (UAAvansKapatmaTertip)bilgi.tertipler[i]);
            }
            if (s.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        public static UAAvansKapatma AvansKapatmaOku(VeriErisim veriErisim, KYM.Kullanici kullanan, UAAvansKapatma bilgi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc son = YetkiKontrol(kullanan, bilgi);
            TABUAAvans tb = new TABUAAvans(ve);

            IDataReader dr = tb.KapatmaOku(bilgi);
            UAAvansKapatma ddeger = null;
            if (dr != null)
            {
                if (dr.Read())
                {
                    ddeger = new UAAvansKapatma();
                    ddeger.surecYil = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMKapatmaAlanlar.SURECYIL, (int)0);
                    ddeger.surecNo = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMKapatmaAlanlar.SURECNO, "");
                    ddeger.odemeEmriNo = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMKapatmaAlanlar.ODEMEEMRINO, "");
                    ddeger.kapatmaOdemeEmriNo = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMKapatmaAlanlar.KAPATMAODEMEEMRINO, "");
                }
                dr.Close();
            }

            if (ddeger != null)
            {
                ddeger.tertipler = new TNSCollection();
                dr = tb.KapatmaTertipOku(bilgi);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        UAAvansKapatmaTertip ddegerT = new UAAvansKapatmaTertip();
                        
                        ddegerT.tertip = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMKapatmaAlanlarTertip.TERTIP, "");
                        ddegerT.miktar = VeriErisim.DegerAl(dr, (int)TABUAAvans.ENUMKapatmaAlanlarTertip.MIKTAR, (double)0);
                        ddeger.tertipler.Add(ddegerT);
                    }
                    dr.Close();
                }
            }
            ve.VeriErisimKapat();
            return ddeger;
        }

        
    }
}

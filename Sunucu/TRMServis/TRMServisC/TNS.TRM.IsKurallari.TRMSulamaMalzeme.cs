using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System.Collections;

namespace TNS.TRM.IsKurallari
{
    public class TRMSulamaMalzeme
    {
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, SulamaMalzeme m)
        {
            Sonuc sonuc = new Sonuc();

            if (!Yetki.YoneticiMi(kullanan))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Bu işlemi yapmaya yetkiniz yok.";
                return sonuc;
            }

            if (string.IsNullOrEmpty(m.ad))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Ad bilgisi boş bırakılamaz.";
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABSulamaMalzeme to = new TABSulamaMalzeme(ve);
            ve.BeginTransaction();

            if (m.kod <= 0)
                m.kod = to.YeniMalzemeKodVer();
            else
                sonuc = Sil(ve, kullanan, m);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(m);
            if (sonuc.islemSonuc)
                sonuc = KaydetGrup(to, m.kod, m.gruplar);
            if (sonuc.islemSonuc)
                sonuc = KaydetOzellik(to, m.kod, m.ozellikler);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim ve, Kullanici kullanan, SulamaMalzeme m)
        {
            Sonuc sonuc = new Sonuc();

            if (!Yetki.YoneticiMi(kullanan))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Bu işlemi yapmaya yetkiniz yok.";
                return sonuc;
            }

            if (m.kod <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz.";
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABSulamaMalzeme to = new TABSulamaMalzeme(ve);
            ve.BeginTransaction();

            sonuc = to.SilOzellikDeger(m.kod);
            if (sonuc.islemSonuc)
                sonuc = to.SilOzellik(m.kod);
            if (sonuc.islemSonuc)
                sonuc = to.SilGrup(m.kod);
            if (sonuc.islemSonuc)
                sonuc = to.Sil(m.kod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static SulamaMalzeme[] Listele(VeriErisim ve, Kullanici kullanan, SulamaMalzeme m, bool detayVer)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABSulamaMalzeme to = new TABSulamaMalzeme(ve);
            IDataReader r = to.Listele(m);

            VeriErisim ve2 = null;
            TABSulamaMalzeme to2 = null;
            if (detayVer)
            {
                ve2 = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
                to2 = new TABSulamaMalzeme(ve2);
            }

            int grupKod = 0;
            if (m.gruplar != null && m.gruplar.Length > 0)
                grupKod = m.gruplar[0].grupKod;

            ArrayList al = new ArrayList();
            while (r.Read())
            {
                SulamaMalzeme gelen = new SulamaMalzeme();
                gelen.kod = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlar.KOD, 0);
                gelen.ad = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlar.AD, string.Empty);

                if (to2 != null)
                {
                    gelen.gruplar = ListeleGrup(to2, gelen.kod, grupKod);
                    gelen.ozellikler = ListeleOzellik(to2, gelen.kod);
                }

                al.Add(gelen);
            }
            r.Close();

            ve.VeriErisimKapat();
            if (ve2 != null)
                ve2.VeriErisimKapat();
            return (SulamaMalzeme[])al.ToArray(typeof(SulamaMalzeme));
        }

        private static Sonuc KaydetGrup(TABSulamaMalzeme to, int malzemeKod, SulamaMalzemeGrup[] gruplar)
        {
            Sonuc sonuc = new Sonuc();
            if (gruplar == null || gruplar.Length == 0)
                return sonuc;

            foreach (SulamaMalzemeGrup grup in gruplar)
            {
                grup.malzemeKod = malzemeKod;
                sonuc = to.KaydetGrup(grup);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        private static SulamaMalzemeGrup[] ListeleGrup(TABSulamaMalzeme to, int malzemeKod, int grupKod)
        {
            IDataReader r = to.ListeleGrup(malzemeKod, grupKod);

            ArrayList al = new ArrayList();
            while (r.Read())
            {
                SulamaMalzemeGrup gelen = new SulamaMalzemeGrup();
                gelen.malzemeKod = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarGrup.MALZEMEKOD, 0);
                gelen.grupKod = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarGrup.SULAMAGRUBUKOD, 0);
                gelen.cokluSatir = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarGrup.COKLUSATIR, 0) > 0;

                al.Add(gelen);
            }
            r.Close();

            return (SulamaMalzemeGrup[])al.ToArray(typeof(SulamaMalzemeGrup));
        }

        private static Sonuc KaydetOzellik(TABSulamaMalzeme to, int malzemeKod, SulamaMalzemeOzellik[] ozellikler)
        {
            Sonuc sonuc = new Sonuc();
            if (ozellikler == null || ozellikler.Length == 0)
                return sonuc;

            foreach (SulamaMalzemeOzellik ozellik in ozellikler)
            {
                ozellik.malzemeKod = malzemeKod;
                sonuc = to.KaydetOzellik(ozellik);
                if (!sonuc.islemSonuc)
                    break;

                sonuc = KaydetOzellikDeger(to, malzemeKod, ozellik.kod, ozellik.degerler);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        private static SulamaMalzemeOzellik[] ListeleOzellik(TABSulamaMalzeme to, int malzemeKod)
        {
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABSulamaMalzeme to2 = new TABSulamaMalzeme(ve2);

            IDataReader r = to.ListeleOzellik(malzemeKod);

            ArrayList al = new ArrayList();
            while (r.Read())
            {
                SulamaMalzemeOzellik gelen = new SulamaMalzemeOzellik();
                gelen.malzemeKod = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarOzellik.MALZEMEKOD, 0);
                gelen.kod = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarOzellik.KOD, 0);
                gelen.ad = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarOzellik.AD, string.Empty);
                gelen.miktarKolon = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarOzellik.MIKTARKOLON, 0) > 0;
                gelen.manuelGiris = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarOzellik.MANUELGIRIS, 0) > 0;
                gelen.kolonTip = (ENUMSulamaMalzemeOzellikKolonTip)VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarOzellik.KOLONTIP, 0);

                gelen.degerler = ListeleOzellikDeger(to2, gelen.malzemeKod, gelen.kod);

                al.Add(gelen);
            }
            r.Close();

            ve2.VeriErisimKapat();
            return (SulamaMalzemeOzellik[])al.ToArray(typeof(SulamaMalzemeOzellik));
        }

        public static int MiktarKolonKodBul(VeriErisim ve, int malzemeKod)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            int kod = new TABSulamaMalzeme(ve).MiktarKolonKodBul(malzemeKod);
            ve.VeriErisimKapat();
            return kod;
        }

        private static Sonuc KaydetOzellikDeger(TABSulamaMalzeme to, int malzemeKod, int ozellikKod, SulamaMalzemeOzellikDeger[] degerler)
        {
            Sonuc sonuc = new Sonuc();
            if (degerler == null || degerler.Length == 0)
                return sonuc;

            foreach (SulamaMalzemeOzellikDeger deger in degerler)
            {
                deger.malzemeKod = malzemeKod;
                deger.ozellikKod = ozellikKod;
                sonuc = to.KaydetOzellikDeger(deger);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        private static SulamaMalzemeOzellikDeger[] ListeleOzellikDeger(TABSulamaMalzeme to, int malzemeKod, int ozellikKod)
        {
            IDataReader r = to.ListeleOzellikDeger(malzemeKod, ozellikKod);

            ArrayList al = new ArrayList();
            while (r.Read())
            {
                SulamaMalzemeOzellikDeger gelen = new SulamaMalzemeOzellikDeger();
                gelen.malzemeKod = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarOzellikDeger.MALZEMEKOD, 0);
                gelen.ozellikKod = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarOzellikDeger.OZELLIKKOD, 0);
                gelen.kod = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarOzellikDeger.KOD, 0);
                gelen.ad = VeriErisim.DegerAl(r, (int)TABSulamaMalzeme.ENUMAlanlarOzellikDeger.AD, string.Empty);

                al.Add(gelen);
            }
            r.Close();

            return (SulamaMalzemeOzellikDeger[])al.ToArray(typeof(SulamaMalzemeOzellikDeger));
        }
    }
}
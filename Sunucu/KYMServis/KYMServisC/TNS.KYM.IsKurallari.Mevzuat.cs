using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM.IsKurallari.Veri;
using System.Collections;

namespace TNS.KYM.IsKurallari
{
    public class MevzuatServis
    {
        public Mevzuat[] Listele(Mevzuat mevzuat, TNSDateTime islemTarih, int sil = 0)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMevzuat to = new TABMevzuat(ve);
            IDataReader dr = to.Listele(mevzuat, islemTarih, sil);

            if (to.Hata() == "" && dr != null)
            {
                while (dr.Read())
                {
                    liste.objeler.Add(MevzuatDoldur(dr));
                }
            }
            if (dr != null) { dr.Close(); }
            ve.VeriErisimKapat();

            return (Mevzuat[])liste.objeler.ToArray(typeof(Mevzuat));
        }
        private Mevzuat MevzuatDoldur(IDataReader dr, int kolonNoStartIndex = 0)
        {
            int i = kolonNoStartIndex;
            Mevzuat mevzuat = new Mevzuat();
            mevzuat.kod = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.KOD + i, "");
            mevzuat.anaKod = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.ANAKOD + i, "");
            mevzuat.klonKod = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.KLONKOD + i, "");
            mevzuat.sira = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.SIRA + i, 0);
            mevzuat.ad = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.AD + i, "");
            mevzuat.aciklama = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.ACIKLAMA + i, "");
            mevzuat.baslangicTarih = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.BALANGICTARIH + i);
            mevzuat.bitisTarih = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.BITISTARIH + i);
            mevzuat.turKod = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.TURKOD + i, 0);
            mevzuat.guncelleme = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.GUNCELLEME + i, 0);
            mevzuat.sil = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.SIL + i, 0);
            mevzuat.bilgiKod = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.BILGIKOD + i, "");
            mevzuat.bilgiNo = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlar.BILGINO + i, 0);
            return mevzuat;
        }
        public Sonuc Kaydet(Mevzuat[] mevzuat)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (mevzuat.Length == 0) { return sonuc; }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMevzuat to = new TABMevzuat(ve);

            ve.BeginTransaction();

            sonuc = to.Kaydet(mevzuat);
            if (sonuc.islemSonuc) { ve.Commit(); } else { ve.Rollback(); }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public Sonuc Sil(Mevzuat[] mevzuat)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (mevzuat.Length == 0) { return sonuc; }

            ArrayList liste = new ArrayList();
            foreach (Mevzuat m in mevzuat)
                liste.Add(m.kod);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMevzuat to = new TABMevzuat(ve);

            ve.BeginTransaction();
            sonuc = to.Sil(liste);
            if (sonuc.islemSonuc) { ve.Commit(); } else { ve.Rollback(); }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public Sonuc KilitIslem(Mevzuat mevzuat, TNSDateTime islemTarih, bool kilit)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            MevzuatDurum mDurum = new MevzuatDurum();
            mDurum.kod = mevzuat.kod;
            mDurum.islemTarih = islemTarih;

            if (kilit)
            {
                mDurum.kilit = 1;
                sonuc = Kilitle(mevzuat, islemTarih, mDurum);
            }
            else
            {
                mDurum.kilit = 0;
                sonuc = KaydetMevzuatDurum(mDurum);
            }

            return sonuc;
        }
        private Sonuc Kilitle(Mevzuat mevzuat, TNSDateTime islemTarih, MevzuatDurum mDurum)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            ArrayList liste = new ArrayList();

            ListeleHepsi(mevzuat, islemTarih, liste);
            if (liste.Count == 0) { return sonuc; }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMevzuat to = new TABMevzuat(ve);

            ve.BeginTransaction();
            sonuc = to.Kilitle(liste);
            if (sonuc.islemSonuc)
            {
                sonuc = KaydetMevzuatDurum(mDurum);
                if (sonuc.islemSonuc) ve.Commit(); else ve.Rollback();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
        private void ListeleHepsi(Mevzuat mevzuat, TNSDateTime islemTarih, ArrayList liste)
        {
            Mevzuat[] listeMevzuat = Listele(mevzuat, islemTarih);
            foreach (Mevzuat m in listeMevzuat)
            {
                liste.Add(m.kod);
                mevzuat = new Mevzuat();
                if (string.IsNullOrEmpty(m.klonKod)) { mevzuat.anaKod = m.kod; } else { mevzuat.anaKod = m.klonKod; }
                ListeleHepsi(mevzuat, islemTarih, liste);

            }
        }

        public MevzuatDurum ListeleMevzuatSonDurum(string mevzuatKod)
        {
            MevzuatDurum mDurum = new MevzuatDurum();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMevzuat to = new TABMevzuat(ve);
            IDataReader dr = to.ListeleMevzuatSonDurum(mevzuatKod);

            if (to.Hata() == "" && dr != null)
            {
                while (dr.Read())
                {
                    mDurum.kod = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlarDURUM.KOD, "");
                    mDurum.kilit = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlarDURUM.KILIT, 0);
                    mDurum.islemTarih = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlarDURUM.ISLEMTARIH);
                    break;
                }
            }
            if (dr != null) { dr.Close(); }
            ve.VeriErisimKapat();

            return mDurum;
        }
        private Sonuc KaydetMevzuatDurum(MevzuatDurum mDurum)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMevzuat to = new TABMevzuat(ve);

            ve.BeginTransaction();

            sonuc = to.KaydetMevzuatDurum(mDurum);
            if (sonuc.islemSonuc) { ve.Commit(); } else { ve.Rollback(); }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public MevzuatSayfa ListeleMevzuatSayfa(string msKod)
        {
            MevzuatSayfa ms = new MevzuatSayfa();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMevzuat to = new TABMevzuat(ve);
            IDataReader dr = to.ListeleMevzuatSayfa(msKod);

            if (to.Hata() == "" && dr != null)
            {
                ObjectArray liste = new ObjectArray();
                Mevzuat m = null;
                int mevzuatKolonNoStartIndex = 2;
                while (dr.Read())
                {
                    if (m == null)
                    {
                        ms.sayfaKod = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlarSAYFA.SAYFAKOD, "");
                        ms.sira = VeriErisim.DegerAl(dr, (int)TABMevzuat.ENUMAlanlarSAYFA.SIRA, 0);
                    }
                    m = MevzuatDoldur(dr, mevzuatKolonNoStartIndex); ms.mevzuatKod = m.kod;
                    liste.objeler.Add(m);
                }
                ms.mevzuat = (Mevzuat[])liste.objeler.ToArray(typeof(Mevzuat));
            }
            if (dr != null) { dr.Close(); }
            ve.VeriErisimKapat();

            return ms;
        }
        public Sonuc KaydetMevzuatSayfa(MevzuatSayfa[] ms)
        {
            Sonuc sonuc = new Sonuc(false, "", ""); if (ms.Length == 0) { return sonuc; }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMevzuat to = new TABMevzuat(ve);

            ve.BeginTransaction();

            MevzuatSayfa silMs = new MevzuatSayfa();
            silMs.sayfaKod = ms[0].sayfaKod;

            sonuc = to.SilMevzuatSayfa(silMs);
            if (sonuc.islemSonuc)
            {
                int sira = 1;
                foreach (MevzuatSayfa form in ms)
                {
                    form.sira = sira++;
                    sonuc = to.KaydetMevzuatSayfa(form);
                    if (!sonuc.islemSonuc) break;
                }
            }

            if (sonuc.islemSonuc) { ve.Commit(); } else { ve.Rollback(); }
            ve.VeriErisimKapat();

            return sonuc;
        }
        public Sonuc SilMevzuatSayfa(MevzuatSayfa ms)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMevzuat to = new TABMevzuat(ve);

            ve.BeginTransaction();
            sonuc = to.SilMevzuatSayfa(ms);
            if (sonuc.islemSonuc) { ve.Commit(); } else { ve.Rollback(); }

            ve.VeriErisimKapat();

            return sonuc;
        }

    }
}

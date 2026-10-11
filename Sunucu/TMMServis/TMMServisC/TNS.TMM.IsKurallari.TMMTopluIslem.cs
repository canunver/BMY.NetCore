using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMTopluIslem
    {
        public Sonuc KaydetForm(Kullanici kullanan, TopluIslemForm t)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTopluIslem to = new TABTopluIslem(ve);

            bool yeni = false;
            if (t.grupKod == 0)
            {
                yeni = true;
                t.grupKod = to.YeniGrupKoduVer();
            }
            else
            {
                if (t.onceliKayitlarinAktarimi) yeni = true;
            }

            ve.BeginTransaction();

            if (yeni)
                sonuc = to.KaydetForm(t);
            else
                sonuc = to.GuncelleForm(t);

            if (t.detay.objeler.Count > 0)
            {
                sonuc = to.Sil(t.grupKod);

                foreach (TopluIslem tm in t.detay.objeler)
                {
                    tm.grupKod = t.grupKod;

                    sonuc = to.Kaydet(tm);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = t.grupKod.ToString();
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Guncelle(Kullanici kullanan, TopluIslem t)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTopluIslem to = new TABTopluIslem(ve);
            ve.BeginTransaction();

            sonuc = to.Guncelle(t);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilForm(Kullanici kullanan, int grupKod)
        {
            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (grupKod == 0)
                sonuc.hataStr = TasinirMalResI.TMM074 + "";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTopluIslem to = new TABTopluIslem(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(grupKod);
            if (sonuc.islemSonuc)
                sonuc = to.SilForm(grupKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format("İşlem Grup Numarası {0} olan kayıtlar silinmiştir.", grupKod);
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TasinirMalResI.TMM026 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray Listele(Kullanici kullanan, TopluIslem t)
        {
            ObjectArray o = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTopluIslem to = new TABTopluIslem(ve);
            IDataReader r = to.Listele(t);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TopluIslem tt = new TopluIslem();

                    tt.id = VeriErisim.DegerAl(r, 0, 0);
                    tt.grupKod = VeriErisim.DegerAl(r, 1, 0);
                    tt.sicilNo = VeriErisim.DegerAl(r, 2, "");
                    tt.kaynakMuh = VeriErisim.DegerAl(r, 3, "");
                    tt.kaynakHrc = VeriErisim.DegerAl(r, 4, "");
                    tt.kaynakAmb = VeriErisim.DegerAl(r, 5, "");
                    tt.kaynakOda = VeriErisim.DegerAl(r, 6, "");
                    tt.kaynakTC = VeriErisim.DegerAl(r, 7, "");
                    tt.hedefMuh = VeriErisim.DegerAl(r, 8, "");
                    tt.hedefHrc = VeriErisim.DegerAl(r, 9, "");
                    tt.hedefAmb = VeriErisim.DegerAl(r, 10, "");
                    tt.hedefOda = VeriErisim.DegerAl(r, 11, "");
                    tt.hedefTC = VeriErisim.DegerAl(r, 12, "");
                    tt.zimmetDusBelgeNo = VeriErisim.DegerAl(r, 13, "");
                    tt.zimmetVerBelgeNo = VeriErisim.DegerAl(r, 14, "");
                    tt.devirVerBelgeNo = VeriErisim.DegerAl(r, 15, "");
                    tt.devirAlBelgeNo = VeriErisim.DegerAl(r, 16, "");

                    o.objeler.Add(tt);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return o;
        }

        public ObjectArray ListeleForm(Kullanici kullanan, TopluIslemForm t)
        {
            ObjectArray o = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTopluIslem to = new TABTopluIslem(ve);
            IDataReader r = to.ListeleForm(t);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TopluIslemForm tt = new TopluIslemForm();

                    tt.grupKod = VeriErisim.DegerAl(r, 0, 0);
                    tt.aciklama = VeriErisim.DegerAl(r, 1, "");

                    o.objeler.Add(tt);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return o;
        }

        public ObjectArray DetaydanGrupKodlariniListele(Kullanici kullanan)
        {
            ObjectArray o = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTopluIslem to = new TABTopluIslem(ve);
            IDataReader r = to.DetaydanGrupKodlariniListele();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TopluIslemForm tt = new TopluIslemForm();
                    tt.grupKod = VeriErisim.DegerAl(r, 0, 0);

                    o.objeler.Add(tt);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return o;
        }
    }
}

using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;
using TNS.UZY;
using TNS.HRC;

namespace TNS.BHMv2.IsKurallari
{
    public class ButceHarcamaSurecDeger
    {
        public Sonuc KaydetHarcamaSurecOnay(Kullanici kullanan, int yil, ObjectArray onayListe)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMHARCAMASURECDEGER to = new TABBHMHARCAMASURECDEGER(ve);

            ve.BeginTransaction();

            sonuc = to.KaydetSurecOnay(onayListe);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt işlemi başarısız oldu.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public Sonuc KaydetHarcamaSurecAlim(Kullanici kullanan, int yil, ObjectArray alimListe)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMHARCAMASURECDEGER to = new TABBHMHARCAMASURECDEGER(ve);

            ve.BeginTransaction();

            sonuc = to.KaydetSurecAlim(alimListe);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt işlemi başarısız oldu.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public Sonuc KaydetHarcamaSurecFatura(Kullanici kullanan, int yil, ObjectArray faturaListe)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMHARCAMASURECDEGER to = new TABBHMHARCAMASURECDEGER(ve);

            ve.BeginTransaction();

            sonuc = to.KaydetSurecFatura(faturaListe);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt işlemi başarısız oldu.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public Sonuc KaydetHarcamaSurecTeslim(Kullanici kullanan, int yil, ObjectArray teslimListe)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMHARCAMASURECDEGER to = new TABBHMHARCAMASURECDEGER(ve);

            ve.BeginTransaction();

            sonuc = to.KaydetSurecTeslim(teslimListe);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt işlemi başarısız oldu.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public Sonuc KaydetHarcamaSurecOdeme(Kullanici kullanan, int yil, ObjectArray odemeListe)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMHARCAMASURECDEGER to = new TABBHMHARCAMASURECDEGER(ve);

            ve.BeginTransaction();

            sonuc = to.KaydetSurecOdeme(odemeListe);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt işlemi başarısız oldu.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray ListeleSurecBilgi(Kullanici kullanan, BHMHarcamaSorgu kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBHMv2", kullanan.KimlikBul());

            TABBHMHARCAMASURECDEGER to = new TABBHMHARCAMASURECDEGER(ve);
            IDataReader r = to.ListeleSurecBilgi(kriter);

            ObjectArray liste = new ObjectArray();

            if (to.Hata() == string.Empty)
            {
                while (r.Read())
                {
                    BHMHarcamaSorgu hs = new BHMHarcamaSorgu();
                    hs.yil = VeriErisim.DegerAl(r, 0, 0);
                    hs.islemYil = VeriErisim.DegerAl(r, 1, 0);
                    hs.islemAy = VeriErisim.DegerAl(r, 2, 0);
                    hs.surecYil = VeriErisim.DegerAl(r, 3, 0);
                    hs.surecNo = VeriErisim.DegerAl(r, 4, string.Empty);
                    hs.kur = VeriErisim.DegerAl(r, 5, string.Empty);
                    hs.fon = VeriErisim.DegerAl(r, 6, string.Empty);
                    hs.fin = VeriErisim.DegerAl(r, 7, string.Empty);
                    hs.eko = VeriErisim.DegerAl(r, 8, string.Empty);
                    hs.peb = VeriErisim.DegerAl(r, 9, string.Empty);
                    hs.muhasebe = VeriErisim.DegerAl(r, 10, 0);
                    hs.birim = VeriErisim.DegerAl(r, 11, string.Empty);
                    hs.onay = VeriErisim.DegerAl(r, 12, (decimal)0);
                    hs.alim = VeriErisim.DegerAl(r, 13, (decimal)0);
                    hs.fatura = VeriErisim.DegerAl(r, 14, (decimal)0);
                    hs.teslim = VeriErisim.DegerAl(r, 15, (decimal)0);
                    hs.odeme = VeriErisim.DegerAl(r, 16, (decimal)0);

                    liste.objeler.Add(hs);
                }
                r.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(to.Hata());
            }

            ve.VeriErisimKapat();

            return liste;
        }

        public Sonuc SurecBilgiSil(Kullanici kullanan, int hedefYil, int yil, string kur, string fon, string fin, string eko)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(hedefYil), kullanan.KimlikBul());
            TABBHMHARCAMASURECDEGER to = new TABBHMHARCAMASURECDEGER(ve);

            ve.BeginTransaction();

            sonuc = to.SilSurecBilgi(yil, kur, fon, fin, eko);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt işlemi başarısız oldu.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeMakineSulamaGrup
    {
        public static string SayacNoAlEtapBazli(VeriErisim ve, string sayacAnahtar, string sayacFormat, string ilKod, string tipi)
        {
            try
            {
                int etapNo = TRMHibeSulamaYonetim.HibeSulamaAktifEtapNo(ve);
                //int etapNo = TRMHibeMakineYonetim.HibeMakineAktifEtapNo(ve);
                if (etapNo == 0)
                    return null;

                string sayacNo = new TABGenel(ve).BelgeNoAl(sayacAnahtar + etapNo.ToString().PadLeft(2, '0') + ilKod.PadLeft(2, '0') + tipi, sayacFormat);
                if (string.IsNullOrEmpty(sayacNo))
                    return null;
                return etapNo.ToString().PadLeft(2, '0') + "." + ilKod.PadLeft(2, '0') + "." + tipi + "." + sayacNo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, HibeMakineSulamaGrup g)
        {
            Sonuc sonuc = new Sonuc();

            if (string.IsNullOrEmpty(g.ilKod))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "İl bilgisi boş bırakılamaz.";
                return sonuc;
            }

            if (!Yetki.YetkiVarMiYazmaya(kullanan, g.ilKod, -1, sonuc, "GrupIslem", (ENUMHibeTur)g.tipi))
            {
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeMakineSulamaGrup tGrup = new TABHibeMakineSulamaGrup(ve);
            ve.BeginTransaction();

            if (string.IsNullOrEmpty(g.kod))
            {
                string plaka = new TABGenel(ve).IlPlakaKodBul(g.ilKod);
                g.kod = SayacNoAlEtapBazli(ve, "HIBEMAKINESULAMAGRUPNO", "00000", plaka, g.tipi.ToString());
                if (string.IsNullOrEmpty(g.kod))
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();

                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Grup numarası alınamadı, lütfen tekrar deneyin.";
                    return sonuc;
                }
                sonuc = tGrup.Kaydet(g);
            }
            else
                sonuc = tGrup.Guncelle(g);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı." + "<br>" + "Grup No: " + g.kod;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim ve, Kullanici kullanan, HibeMakineSulamaGrup g)
        {
            Sonuc sonuc = new Sonuc();

            if (string.IsNullOrEmpty(g.kod))
                sonuc.hataStr = "Grup kodu boş bırakılamaz." + "<br>";
            if (string.IsNullOrEmpty(g.ilKod))
                sonuc.hataStr += "İl bilgisi boş bırakılamaz.";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (!Yetki.YetkiVarMiYazmaya(kullanan, g.ilKod, -1, sonuc, "GrupIslem", (ENUMHibeTur)g.tipi))
            {
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeMakineSulamaGrup tGrup = new TABHibeMakineSulamaGrup(ve);
            ve.BeginTransaction();

            sonuc = tGrup.Sil(g);
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

        public static Sonuc GuncelleKontrol(VeriErisim ve, Kullanici kullanan, HibeMakineSulamaGrup g)
        {
            Sonuc sonuc = new Sonuc();

            if (string.IsNullOrEmpty(g.kod))
                sonuc.hataStr = "Grup kodu boş bırakılamaz." + "<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (!Yetki.YetkiVarMiYazmaya(kullanan, g.ilKod, -1, sonuc, "GrupIslem", (ENUMHibeTur)g.tipi))
            {
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeMakineSulamaGrup tGrup = new TABHibeMakineSulamaGrup(ve);
            ve.BeginTransaction();

            if (g.kontrol > 0)
                g.kontrolTarih = new TNSDateTime(DateTime.Now);

            sonuc = tGrup.GuncelleKontrol(g);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kontrol kaydı başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kontrol kaydı sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static HibeMakineSulamaGrup[] Listele(VeriErisim ve, Kullanici kullanan, HibeMakineSulamaGrup g, params int[] ekDurumlar)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeMakineSulamaGrup tGrup = new TABHibeMakineSulamaGrup(ve);
            IDataReader r = tGrup.Listele(g, Yetki.YetkiliOlduguIlleriGetir(kullanan), ekDurumlar);
            if (!string.IsNullOrEmpty(tGrup.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tGrup.Hata());
            }

            ObjectArray gruplar = new ObjectArray();
            while (r.Read())
            {
                HibeMakineSulamaGrup grup = new HibeMakineSulamaGrup();
                grup.kod = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.KOD, string.Empty);
                grup.ilKod = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.ILKOD, string.Empty);
                grup.durum = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM, 0);
                grup.odemeKod = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.ODEMEKOD, 0);
                grup.tipi = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.TIPI, 0);
                grup.kontrolTarih = VeriErisim.DegerAl(r, (int)TABHibeMakineSulamaGrup.ENUMAlanlar.KONTROLTARIHI);

                gruplar.objeler.Add(grup);
            }
            r.Close();

            ve.VeriErisimKapat();
            return (HibeMakineSulamaGrup[])gruplar.objeler.ToArray(typeof(HibeMakineSulamaGrup));
        }
    }
}
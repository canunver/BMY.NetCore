using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeSulamaDiger
    {
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, HibeSulamaDiger[] hsdler)
        {
            Sonuc sonuc = new Sonuc();
            foreach (HibeSulamaDiger hsd in hsdler)
            {
                if (hsd.hibeKod <= 0)
                    sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
                if (hsd.tip <= 0)
                    sonuc.hataStr += "Tip boş bırakılamaz.";
                if (!string.IsNullOrEmpty(sonuc.hataStr))
                {
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulamaDiger tDiger = new TABHibeSulamaDiger(ve);
            ve.BeginTransaction();

            sonuc = tDiger.Sil(hsdler[0].hibeKod, "");
            if (sonuc.islemSonuc)
            {
                foreach (HibeSulamaDiger hsd in hsdler)
                {
                    if (hsd.sulamaGrupKod > 0)
                    {
                        if (string.IsNullOrEmpty(hsd.parselID))
                            hsd.parselID = Guid.NewGuid().ToString();

                        sonuc = tDiger.Kaydet(hsd);
                    }
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

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

        public Sonuc Guncelle(VeriErisim ve, Kullanici kullanan, HibeSulamaDiger hsd)
        {
            Sonuc sonuc = new Sonuc();

            if (hsd.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (hsd.tip <= 0)
                sonuc.hataStr += "Tip boş bırakılamaz.";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulamaDiger tDiger = new TABHibeSulamaDiger(ve);
            ve.BeginTransaction();

            if (hsd.sulamaGrupKod > 0 && !string.IsNullOrEmpty(hsd.parselID))
            {
                sonuc = tDiger.Guncelle(hsd);
            }

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

        public Sonuc Sil(VeriErisim ve, Kullanici kullanan, int hibeKod, string parselID, string hibeIlKod, int sulamaGrupKod)
        {
            Sonuc sonuc = new Sonuc();

            if (hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.";
            if (string.IsNullOrWhiteSpace(parselID))
                sonuc.hataStr = "Parsel kodu boş bırakılamaz.";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulamaDiger tDiger = new TABHibeSulamaDiger(ve);
            ve.BeginTransaction();

            sonuc = tDiger.Sil(hibeKod, parselID);
            if (sonuc.islemSonuc)
            {
                if (parselID != "")
                {
                    sonuc = TRMHibeSulamaMalzeme.Sil(ve, kullanan, hibeKod, ENUMHibeSulamaMalzemeTip.PROJE, hibeIlKod, sulamaGrupKod, parselID, true);
                    if (sonuc.islemSonuc)
                        sonuc = TRMHibeSulamaMalzeme.Sil(ve, kullanan, hibeKod, ENUMHibeSulamaMalzemeTip.UYGULAMA, hibeIlKod, sulamaGrupKod, parselID, true);
                }

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

        public Sonuc ParselReddet(VeriErisim ve, Kullanici kullanan, int hibeKod, string parselID, string hibeIlKod, int sulamaGrupKod)
        {
            Sonuc sonuc = new Sonuc();

            if (hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.";
            if (string.IsNullOrWhiteSpace(parselID))
                sonuc.hataStr = "Parsel kodu boş bırakılamaz.";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulamaDiger tDiger = new TABHibeSulamaDiger(ve);
            ve.BeginTransaction();

            sonuc = tDiger.Reddet(hibeKod, parselID);
            if (sonuc.islemSonuc)
            {
                if (parselID != "")
                {
                    sonuc = TRMHibeSulamaMalzeme.Sil(ve, kullanan, hibeKod, ENUMHibeSulamaMalzemeTip.PROJE, hibeIlKod, sulamaGrupKod, parselID, true);
                    if (sonuc.islemSonuc)
                        sonuc = TRMHibeSulamaMalzeme.Sil(ve, kullanan, hibeKod, ENUMHibeSulamaMalzemeTip.UYGULAMA, hibeIlKod, sulamaGrupKod, parselID, true);
                }
            }


            //HibeSulamaMalzemeFiyat[] listeprj = TRMHibeSulamaMalzeme.Listele(ve, kullanan, hibeKod, ENUMHibeSulamaMalzemeTip.PROJE, hibeIlKod, 999, "");
            //TRMHibeSulamaMalzeme.Kaydet(ve, kullanan, listeprj, hs);

            //HibeSulamaMalzemeFiyat[] listeuyg = TRMHibeSulamaMalzeme.Listele(ve, kullanan, hibeKod, ENUMHibeSulamaMalzemeTip.UYGULAMA, hibeIlKod, 999, "");
            //TRMHibeSulamaMalzeme.Kaydet(ve, kullanan, listeuyg, hs);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Reddetme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            //Toplam hibe tutarlarını güncellemek için

            //sonuc = new TRMHibeSulama().KaydetParselReddedildi(kullanan, new HibeSulama { kod = hibeKod });

            ///////////////////////////////////////////////////////

            sonuc.bilgiStr = "Reddetme işlemi başarıyla tamamlandı.";
            return sonuc;
        }

        public static HibeSulamaDiger[] Listele(VeriErisim ve, Kullanici kullanan, int hibeKod)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeSulamaDiger tDiger = new TABHibeSulamaDiger(ve);
            IDataReader r = tDiger.Listele(hibeKod);
            if (!string.IsNullOrEmpty(tDiger.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tDiger.Hata());
            }

            ObjectArray hbsler = new ObjectArray();
            while (r.Read())
            {
                HibeSulamaDiger gelen = new HibeSulamaDiger();
                gelen.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.HIBEKOD, 0);
                gelen.tip = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.TIP, 0);
                gelen.sulamaGrupKod = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.SULAMAGRUPKOD, 0);
                gelen.parselID = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELID, string.Empty);
                gelen.parselNo = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELNO, string.Empty);
                gelen.parselAlan = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELALAN, (double)0);
                gelen.parselSulanacakAlan = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELSULANACAKALAN, (double)0);
                gelen.parselSuKaynakTip = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELSUKAYNAKTIP, 0);
                gelen.parselSuKaynakDebi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELSUKAYNAKDEBI, (double)0);
                gelen.kuyuNo = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.KUYUNO, string.Empty);
                gelen.kuyuDebi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.KUYUDEBI, (double)0);
                gelen.kuyuPompaGucu = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.KUYUPOMPAGUCU, (double)0);
                gelen.kuyuTrafoGucu = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.KUYUTRAFOGUCU, (double)0);
                gelen.kuyuSulanacakAlan = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.KUYUSULANACAKALAN, (double)0);
                gelen.parselSuKaynakCesidi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELSUKAYNAGICESIDI, 0);
                gelen.parselSuTeminYonetimi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELSUTEMINYONTEMI, 0);
                gelen.parselSuKaliteSinifi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELSUKALITESINIFI, 0);
                gelen.parselSuKaynagiDurumu = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELSUKAYNAGIDURUMU, 0);
                gelen.parselMulkiyeti = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.PARSELMULKIYETI, 0);
                gelen.toprakBunyeSinifi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.TOPRAKBUNYESINIFI, 0);
                gelen.toprakTarlaKapasitesi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.TOPRAKTARLAKAPASITESI, string.Empty);
                gelen.toprakInfiltrasyonHizi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.TOPRAKINFILTRASYONHIZI, (double)0);
                gelen.bitkiUrunCesidi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.BITKIURUNCESIDI, string.Empty);
                gelen.bitkiTesisYili = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.BITKITESISYILI, 0);
                gelen.bitkiDikimNormu = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.BITKIDIKIMNORMU, string.Empty);
                gelen.bitkiKokDerinligi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.BITKIKOKDERINLIGI, string.Empty);
                gelen.bitkiSuIhtiyaci = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.BITKISUIHTIYACI, string.Empty);
                gelen.firmaVergiNo = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.FIRMAVERGINO, string.Empty);
                gelen.firmaAd = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.FIRMAAD, string.Empty);
                gelen.faturaNo = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.FATURANO, string.Empty);
                gelen.faturaTarihi = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.FATURATARIH);
                gelen.faturaAciklama = VeriErisim.DegerAl(r, (int)TABHibeSulamaDiger.ENUMAlanlar.FATURAACIKLAMA, string.Empty);

                hbsler.objeler.Add(gelen);
            }
            r.Close();

            ve.VeriErisimKapat();
            return (HibeSulamaDiger[])hbsler.objeler.ToArray(typeof(HibeSulamaDiger));
        }
    }
}
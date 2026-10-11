using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMSicilNoDurum
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf, string durum)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            tf.islemTipAd = tf.islemTipAd == "" ? TMMServis.UzayYaziDegerGetir("TASISLEMTIPAD", tf.islemTipKod.ToString(), true, "") : tf.islemTipAd;

            if (tf.islemTipTur == (int)ENUMIslemTipi.ACILIS ||
                tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
            {
                return sonuc;
            }

            ObjectArray listeSicilNoDurum = new ObjectArray();
            foreach (TasinirIslemDetay detay in tf.detay.objeler)
            {
                listeSicilNoDurum.objeler.Add(new SicilNoDurum()
                {
                    tifKod = tf.kod,
                    prSicilNo = (detay.sicilNo == 0 ? OrtakFonksiyonlar.ConvertToInt(detay.eSicilNo, 0) : detay.sicilNo),
                    gorSicilNo = detay.gorSicilNo,
                    durum = tf.islemTipKod,
                    durumIslem = (durum == "Onay" ? 1 : 0),  //0:Kayıt - 1:Onay
                    durumAciklama = tf.islemTipAd,
                    islemTarihi = new TNSDateTime(DateTime.Now)
                });
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoDurum to = new TABSicilNoDurum(ve);


            //Farklı tif kaydı tarafından kayıt işlemi yapılıyorsa uyarı ver
            foreach (SicilNoDurum form in listeSicilNoDurum.objeler)
            {
                SicilNoDurum kayit = Listele(ve, kullanan, form.prSicilNo);
                if (kayit.prSicilNo > 0 && kayit.tifKod != form.tifKod && kayit.durumIslem == 0)
                    sonuc.hataStr += "<br>" + kayit.gorSicilNo + " sicil nolu malzemenin <b>" + kayit.durumAciklama + "</b> işlemi yapılıyor.";
            }

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }
            //***************************************************************


            ve.BeginTransaction();

            foreach (SicilNoDurum form in listeSicilNoDurum.objeler)
            {
                sonuc = to.Kaydet(form);
                if (!sonuc.islemSonuc)
                    break;
            }

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

        public static SicilNoDurum Listele(VeriErisim veriErisim, Kullanici kullanan, int prSicilNo)
        {
            SicilNoDurum form = new SicilNoDurum();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoDurum to = new TABSicilNoDurum(ve);

            IDataReader dr = to.Listele(prSicilNo);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form.prSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDurum.ENUMAlanlar.PRSICILNO, 0);
                    form.gorSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDurum.ENUMAlanlar.GORSICILNO, "");
                    form.tifKod = VeriErisim.DegerAl(dr, (int)TABSicilNoDurum.ENUMAlanlar.TIFKOD, "");
                    form.durum = VeriErisim.DegerAl(dr, (int)TABSicilNoDurum.ENUMAlanlar.DURUM, 0);
                    form.durumIslem = VeriErisim.DegerAl(dr, (int)TABSicilNoDurum.ENUMAlanlar.DURUMISLEM, 0);
                    form.durumAciklama = VeriErisim.DegerAl(dr, (int)TABSicilNoDurum.ENUMAlanlar.DURUMACIKLAMA, "");
                    form.islemTarihi = VeriErisim.DegerAl(dr, (int)TABSicilNoDurum.ENUMAlanlar.ISLEMTARIH);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return form;
        }
    }
}

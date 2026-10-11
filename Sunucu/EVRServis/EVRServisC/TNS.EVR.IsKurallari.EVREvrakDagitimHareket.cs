using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;
using System.Collections;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakDagitimHareket
    {

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakDagitimHareket[] formList)
        {
            if (formList == null || formList.Length == 0 || string.IsNullOrEmpty(formList[0].prEvrakNo))
                return new Sonuc(false, "Lütfen kayıt bilgilerini kontrol ediniz", "");

            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakDagitimHareket tab = new TABEvrakDagitimHareket(ve);

            ve.BeginTransaction();
            try
            {
                foreach (var form in formList)
                {
                    form.sonIslemTarihi = new TNSDateTime(DateTime.Now);
                    sonuc = tab.Kaydet(form);
                    if (!sonuc.islemSonuc)
                        break;
                }


                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{0FD25794-AD73-420B-9FAB-9DDF2828166F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo)
        {
            if (string.IsNullOrEmpty(prEvrakNo))
                return new Sonuc(false, "Silinecek veri bulunamadı.", "");

            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakDagitimHareket tab = new TABEvrakDagitimHareket(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(prEvrakNo);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI097;
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = EvrakI.EVRI098 + "<br>" + sonuc.hataStr;
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{A89FC8B1-3E4F-41B9-B625-A961439FB75F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakDagitimHareket[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakDagitimHareket form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakDagitimHareket tab = new TABEvrakDagitimHareket(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakDagitimHareket();
                    form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.PREVRAKNO, "");
                    form.siraNo = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.SIRANO, 0);
                    form.evrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.EVRAKNO, 0);
                    form.evrakTarihi = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.EVRAKTARIHI);
                    form.konu = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.KONU, "");
                    form.dagitimYeri = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.DAGITIMYERI, "");
                    form.dagitimTip = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.DAGITIMTIP, 0);
                    form.idariBirimKimlikKod = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.IDARIBIRIMKOD, "");
                    form.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.EVRAKBIRIMKOD, "");
                    form.islem = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.ISLEM, 0);
                    form.sonIslemTarihi = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.SONISLEMTARIHI);
                    form.kisi = VeriErisim.DegerAl(dr, (int)TABEvrakDagitimHareket.ENUMAlanlar.KISI, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakDagitimHareket[])liste.objeler.ToArray(typeof(EvrakDagitimHareket));
        }

    }
}

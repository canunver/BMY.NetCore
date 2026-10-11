using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak havale kural bilgileriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRHavaleKural
    {
        /// <summary>
        /// Parametre olarak verilen HavaleKural nesnesine göre, havale kuralı olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">HavaleKural bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, HavaleKural form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI093, "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABHavaleKural to = new TABHavaleKural(ve);

                sonuc = to.Kaydet(form);
                if (sonuc.islemSonuc)
                {
                    sonuc = to.SilGB(form.kod);
                    if (sonuc.islemSonuc)
                    {
                        foreach (HavaleKuralGB gb in form.gbler)
                            gb.havaleKuralKodu = form.kod;

                        //if (form.gbler.Count > 0)
                        //{
                        sonuc = to.KaydetGB(form.gbler);
                        if (sonuc.islemSonuc)
                        {
                            sonuc = to.SilDetay(form.kod);
                            if (sonuc.islemSonuc)
                            {
                                foreach (HavaleKuralDetay detay in form.detaylar)
                                    detay.havaleKuralKodu = form.kod;

                                sonuc = to.KaydetDetay(form.detaylar);
                            }
                        }
                        //}
                        //else 
                        //{

                        //}
                    }
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr += EvrakI.EVRI094;
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{C079027B-ACC9-4A6D-94DD-31FEB2E340CD} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak havale kural listesini HavaleKural nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kodList">Havale kural kod listesini içeren nesne</param>
        /// <param name="tcKimlikNo">TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="evrakBirimKodu">Evrak birim kodunu içeren nesne</param>
        /// <returns>HavaleKural nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, string kodList, string tcKimlikNo, string evrakBirimKodu)
        {
            ObjectArray hVAN = new ObjectArray();
            HavaleKural form = new HavaleKural();
            HavaleKuralGB form1 = new HavaleKuralGB();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABHavaleKural to = new TABHavaleKural(ve);
            IDataReader r = to.Listele(kodList, tcKimlikNo, evrakBirimKodu);

            if (to.Hata() == "")
            {
                int eskiKod = 0;
                int eskiKod2 = 0;
                int eskiKod3 = 0;
                int index1 = ((int)TABHavaleKural.ENUMAlanlar.BITISTARIH) + 1;
                int index2 = index1 + ((int)TABHavaleKural.ENUMAlanlarGB.GEREGIBILGI) + 1;
                while (r.Read())
                {
                    int kod = VeriErisim.DegerAl(r, (int)TABHavaleKural.ENUMAlanlar.KOD, 0);

                    if (eskiKod != kod)
                    {
                        if (eskiKod > 0)
                        {
                            hVAN.objeler.Add(form);
                            eskiKod2 = 0;
                            eskiKod3 = 0;
                        }

                        form = new HavaleKural();
                        form.kod = kod;
                        form.kullanici = VeriErisim.DegerAl(r, (int)TABHavaleKural.ENUMAlanlar.KULLANICIKODU, "");
                        form.evrakBirimKodu = VeriErisim.DegerAl(r, (int)TABHavaleKural.ENUMAlanlar.EVRAKBIRIMKODU, "");
                        form.ad = VeriErisim.DegerAl(r, (int)TABHavaleKural.ENUMAlanlar.AD, "");
                        form.baslangicTarih = VeriErisim.DegerAl(r, (int)TABHavaleKural.ENUMAlanlar.BASLANGICTARIH);
                        form.bitisTarih = VeriErisim.DegerAl(r, (int)TABHavaleKural.ENUMAlanlar.BITISTARIH);
                    }

                    int kod2 = VeriErisim.DegerAl(r, index1 + (int)TABHavaleKural.ENUMAlanlarGB.KOD, 0);
                    if (eskiKod2 != kod2)
                    {
                        form1 = new HavaleKuralGB();
                        form1.havaleKuralKodu = kod;
                        form1.kod = kod2;
                        form1.deger = VeriErisim.DegerAl(r, index1 + (int)TABHavaleKural.ENUMAlanlarGB.DEGER, "");
                        form1.geregiBilgi = VeriErisim.DegerAl(r, index1 + (int)TABHavaleKural.ENUMAlanlarGB.GEREGIBILGI, 0);
                        form.gbler.Add(form1);
                    }

                    if (eskiKod2 > 0 && eskiKod2 != kod2)
                        eskiKod3 = -1;

                    if (eskiKod3 != -1)
                    {
                        int kod3 = VeriErisim.DegerAl(r, index2 + (int)TABHavaleKural.ENUMAlanlarDetay.KOD, 0);
                        if (eskiKod3 != kod3)
                        {
                            HavaleKuralDetay form2 = new HavaleKuralDetay();
                            form2.havaleKuralKodu = kod;
                            form2.kod = kod3;
                            form2.alan = VeriErisim.DegerAl(r, index2 + (int)TABHavaleKural.ENUMAlanlarDetay.ALAN, 0);
                            form2.islec = VeriErisim.DegerAl(r, index2 + (int)TABHavaleKural.ENUMAlanlarDetay.ISLEC, 0);
                            form2.deger = VeriErisim.DegerAl(r, index2 + (int)TABHavaleKural.ENUMAlanlarDetay.DEGER, "");
                            form.detaylar.Add(form2);
                        }
                        eskiKod3 = kod3;
                    }

                    eskiKod2 = kod2;

                    eskiKod = kod;
                }
                r.Close();

                if (eskiKod > 0)
                    hVAN.objeler.Add(form);

            }
            ve.VeriErisimKapat();

            return hVAN;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, havale kural bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="havaleKuralKodu">Havale kural kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, int havaleKuralKodu)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI096, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABHavaleKural to = new TABHavaleKural(ve);

                sonuc = to.Sil(havaleKuralKodu);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr += EvrakI.EVRI097;
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
                OrtakFonksiyonlar.HataStrYaz("{7C785C31-3761-403E-BFFB-066DA35F4567} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRForm
    {
        public static Sonuc KaydetForm(VeriErisim veriErisim, Kullanici kullanan, Form form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABForm tab = new TABForm(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.SilOzellik(form.sayfaKod, "");
                if (sonuc.islemSonuc)
                    sonuc = tab.SilKontrol(form.sayfaKod, "");
                if (sonuc.islemSonuc)
                    sonuc = tab.SilForm(form.sayfaKod);
                if (sonuc.islemSonuc)
                    sonuc = tab.KaydetForm(form);
                if (sonuc.islemSonuc)
                {
                    foreach (FormKontrol kontrol in form.kontroller)
                    {
                        sonuc = tab.KaydetKontrol(kontrol);
                        if (!sonuc.islemSonuc)
                            break;
                        foreach (FormOzellik ozellik in kontrol.ozellikler)
                        {
                            sonuc = tab.KaydetOzellik(ozellik);
                            if (!sonuc.islemSonuc)
                                break;
                        }

                        if (!sonuc.islemSonuc)
                            break;
                    }
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
                OrtakFonksiyonlar.HataStrYaz("{11EA7C99-40C9-4728-B5C2-C6D0C6251644} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Form Oku(VeriErisim veriErisim, Kullanici kullanan, Form form)
        {
            Form[] list = ListeleForm(veriErisim, kullanan, form);
            if (list != null && list.Length == 1)
                form = list[0];

            return form;
        }

        public static Form[] ListeleForm(VeriErisim veriErisim, Kullanici kullanan, Form form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABForm tab = new TABForm(ve);

            IDataReader dr = tab.ListeleForm(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new Form();
                    form.sayfaKod = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlar.SAYFAKOD, "");
                    form.durum = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlar.DURUM, 0);
                    form.sablonKod = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlar.SABLONKOD, "");
                    form.surecNo = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlar.SURECNO, "");
                    form.sayfaAd = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlar.SAYFAAD, "");
                    form.evrBirimKod = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlar.EVRAKBIRIMKODU, "");
                    form.gidenEvrakOlarakKayitEdilebilir = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlar.GIDENEVRAKOLARAK, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();

                if (liste.objeler.Count == 1)
                {
                    Form f = (Form)liste.objeler[0];
                    FormKontrol ff = new FormKontrol() { sayfaKod = f.sayfaKod };
                    f.kontroller = ListeleKontrol(null, kullanan, ff);
                    liste.objeler[0] = f;
                }
            }

            ve.VeriErisimKapat();

            return (Form[])liste.objeler.ToArray(typeof(Form));
        }
        public static FormKontrol[] ListeleKontrol(VeriErisim veriErisim, Kullanici kullanan, FormKontrol form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABForm tab = new TABForm(ve);

            IDataReader dr = tab.ListeleKontrol(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new FormKontrol();
                    form.kontrolKod = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlarKontrol.KONTROLKOD, "");
                    form.sayfaKod = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlarKontrol.SAYFAKOD, "");
                    form.kontrolTur = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlarKontrol.KONTROLTUR, "");

                    FormOzellik fff = new FormOzellik() { kontrolKod = form.kontrolKod };
                    form.ozellikler = ListeleOzellik(null, kullanan, fff);
                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (FormKontrol[])liste.objeler.ToArray(typeof(FormKontrol));
        }
        public static FormOzellik[] ListeleOzellik(VeriErisim veriErisim, Kullanici kullanan, FormOzellik form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABForm tab = new TABForm(ve);

            IDataReader dr = tab.ListeleOzellik(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new FormOzellik();
                    form.kontrolKod = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlarOzellik.KONTROLKOD, "");
                    form.ozellikTur = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlarOzellik.OZELLIKTUR, "");
                    form.deger = VeriErisim.DegerAl(dr, (int)TABForm.ENUMAlanlarOzellik.DEGER, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (FormOzellik[])liste.objeler.ToArray(typeof(FormOzellik));
        }
    }
}

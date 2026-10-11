using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRVekalet
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Vekalet form)
        {
            Sonuc sonuc = new Sonuc(true, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABVekalet tab = new TABVekalet(ve);

            ve.BeginTransaction();
            try
            {
                if (AktifVekaletVarMi_VekaletVeren(veriErisim, kullanan, form))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Yeni bir vekalet kaydı yapılamaz.<br>" + "'" + form.kisi1Ad + "' isimli kullanıcının seçilen tarih aralığında kayıtlı bir vekaleti mevcut.";
                }

                if (sonuc.islemSonuc)
                {
                    if (form.kod == 0)
                        sonuc = tab.Kaydet(form);
                    else
                        sonuc = tab.Guncelle(form);
                }

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.anahtar = form.kod.ToString();
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{DF2E451F-2929-4328-84BB-586AAC8C697D} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Vekalet form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (form.kod <= 0)
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABVekalet tab = new TABVekalet(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(form);
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
                OrtakFonksiyonlar.HataStrYaz("{47EC0265-DB86-40B5-A55A-FC93E33E05E7} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Vekalet[] Listele(VeriErisim veriErisim, Kullanici kullanan, Vekalet form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABVekalet tab = new TABVekalet(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new Vekalet();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.KOD, 0);
                    form.kisi1 = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.KISI1, "");
                    form.kisi2 = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.KISI2, "");
                    form.tarih1 = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.TARIH1);
                    form.tarih2 = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.TARIH2);
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.ISLEMYAPAN, "");
                    form.islemTarih = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.ISLEMTARIH);
                    form.kisi1Ad = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.KISI1AD, "");
                    form.kisi2Ad = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.KISI2AD, "");
                    form.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.ISLEMYAPANAD, "");
                    form.aktif = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.AKTIF, 0);
                    form.birim1 = VeriErisim.DegerAl(dr, (int)TABVekalet.ENUMAlanlar.BIRIM1, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (Vekalet[])liste.objeler.ToArray(typeof(Vekalet));
        }
        public static Vekalet Oku(VeriErisim veriErisim, Kullanici kullanan, Vekalet form)
        {
            if (form.kod == 0) return null;

            Vekalet[] list = Listele(veriErisim, kullanan, form);
            if (list != null && list.Length == 1)
                form = list[0];

            return form;
        }

        /// <summary>
        /// Kullanıcın aktif vekaletlerini listele
        /// </summary>
        public static Vekalet[] AktifVekaletleriListele(VeriErisim veriErisim, Kullanici kullanan)
        {
            Vekalet kriter = new Vekalet();
            kriter.kod = -100; // Kullanıcın aktif vekaletlerini listele
            kriter.kisi2 = kullanan.mernis;
            kriter.tarih1 = new TNSDateTime(DateTime.Now.ToShortDateString());
            kriter.tarih2 = new TNSDateTime(DateTime.Now.ToShortDateString());

            return Listele(veriErisim, kullanan, kriter);
        }

        /// <summary>
        /// Vekalet Veren Kullanıcının geçerli tarihlerde önceden aktif vekaleti var mı?
        /// </summary>
        public static bool AktifVekaletVarMi_VekaletVeren(VeriErisim veriErisim, Kullanici kullanan, Vekalet v)
        {
            Vekalet kriter = new Vekalet();
            kriter.kod = -200; // Vekalet Veren Kullanıcının geçerli tarihlerde önceden aktif vekaleti var mı?
            kriter.kisi1 = v.kisi1;
            kriter.tarih1 = v.tarih1;
            kriter.tarih2 = v.tarih2;
            kriter.birim1 = v.birim1;

            Vekalet[] list = Listele(veriErisim, kullanan, kriter);

            bool varMi = false;
            if (list != null && list.Length > 0)
            {
                varMi = true;
                if (v.kod > 0)
                {
                    foreach (var item in list)
                    {
                        if (item.kod == v.kod)
                        {
                            varMi = false;
                            break;
                        }
                    }
                }
            }
            return varMi;
        }

        public static Vekalet AktifVekaletOku(VeriErisim veriErisim, Kullanici kullanan, string vekaletVerenMernis)
        {
            Vekalet kriter = new Vekalet();
            kriter.kod = -300; // Kullanıcın aktif vekaletlerini listele
            kriter.kisi1 = vekaletVerenMernis;
            kriter.tarih1 = new TNSDateTime(DateTime.Now.ToShortDateString());
            kriter.tarih2 = new TNSDateTime(DateTime.Now.ToShortDateString());

            return Oku(veriErisim, kullanan, kriter);
        }

    }
}

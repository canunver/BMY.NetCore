using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRMeta
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Meta form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.ad))
                hata += "Meta adı boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABMeta tab = new TABMeta(ve);

            if ((form.klasor == 3 || form.klasor == 4) && tab.EvrakKayitliMetaVarMi(form.kod) > 0)//Tanımlanan arşivlerde kullanılıyorsa değişiklik işlemine izin verme!
                hata = "Değişiklik yapmak istediğiniz meta kullanılıyor. Bu nedenle değişiklik yapılamaz.<br>";
            else if (form.kod > 0 && tab.ArsivKayitliMetaVarMi(form.kod) > 0)               //Tanımlanan arşivlerde kullanılıyorsa değişiklik işlemine izin verme!
                hata = "Değişiklik yapmak istediğiniz meta, tanımlanmış arşivlerde kullanılıyor. Öncelikle kullanılan arşivlerden kaldırmalısınız.<br>";


            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                ve.VeriErisimKapat();
                return sonuc;
            }


            ve.BeginTransaction();
            try
            {
                if (form.kod == 0) { sonuc = tab.Kaydet(form); } else { sonuc = tab.Guncelle(form); }

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
                OrtakFonksiyonlar.HataStrYaz("{97EF5ABE-B9FC-4FAC-AB7F-A7A2E093CEEA} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Meta form)
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
            TABMeta tab = new TABMeta(ve);

            if ((form.klasor == 3 || form.klasor == 4) && tab.EvrakKayitliMetaVarMi(form.kod) > 0)
                hata = "Değişiklik yapmak istediğiniz meta kullanılıyor. Bu nedenle değişiklik yapılamaz.<br>";
            else if (tab.ArsivKayitliMetaVarMi(form.kod) > 0)               //Tanımlanan arşivlerde kullanılıyorsa silme işlemine izin verme!
                hata = "Silmek istediğiniz meta, tanımlanmış arşivlerde kullanılıyor. Öncelikle kullanılan arşivlerden kaldırmalısınız.<br>";


            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                ve.VeriErisimKapat();
                return sonuc;
            }


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
                OrtakFonksiyonlar.HataStrYaz("{C0712014-ECD1-4F9A-BAFF-E5526A594A0A} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Meta[] Listele(VeriErisim veriErisim, Kullanici kullanan, Meta form, bool detay = false)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABMeta tab = new TABMeta(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new Meta();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.KOD, 0);
                    form.ad = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.AD, "");
                    form.tip = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.TIP, 0);
                    form.zorunlu = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.ZORUNLU, 0);
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.ISLEMYAPAN, "");
                    form.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.EVRAKBIRIMKOD, "");
                    if (detay) form.detay = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.DETAY, "");
                    form.klasor = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.KLASOR, 0);
                    form.grupKod = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.GRUPKOD, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (Meta[])liste.objeler.ToArray(typeof(Meta));
        }
        public static Meta Oku(VeriErisim veriErisim, Kullanici kullanan, Meta form)
        {
            if (form.kod == 0) return null;

            Meta[] list = EVRMeta.Listele(veriErisim, kullanan, form, true);
            if (list != null && list.Length == 1)
                form = list[0];

            return form;
        }

    }
}

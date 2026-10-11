using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRArsiv
    {
        public static Arsiv[] Listele(VeriErisim veriErisim, Kullanici kullanan, Arsiv form)
        {
            //Şifresiz olarak kullanıcıların gelmesi muhtemel.Ör. Hibe destekleri için. Bu nedenle kullanıcı kontrolü yapılmayacak
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABArsiv tab = new TABArsiv(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new Arsiv();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlar.KOD, 0);
                    form.anaKod = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlar.ANAKOD, 0);
                    form.sira = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlar.SIRA, 0);
                    form.ad = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlar.AD, "");
                    form.turKod = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlar.TURKOD, 0);
                    form.kodYol = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlar.KODYOL, "");
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlar.ISLEMYAPAN, "");
                    form.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlar.EVRAKBIRIMKOD, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (Arsiv[])liste.objeler.ToArray(typeof(Arsiv));
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Arsiv form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.ad))
                hata += "klasör adı boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABArsiv tab = new TABArsiv(ve);
            
            ve.BeginTransaction();
            try
            {
                if (form.kod == 0)
                    sonuc = tab.Kaydet(form);
                else
                    sonuc = tab.Guncelle(form);


                if (sonuc.islemSonuc && form.kod > 0)
                {
                    sonuc = tab.SilYetki(form.kod);
                    if (sonuc.islemSonuc && form.yetki != null && form.yetki.Length > 0)
                    {
                        foreach (ArsivYetki yetki in form.yetki)
                        {
                            yetki.arsivKod = form.kod;
                            sonuc = tab.Kaydet(yetki);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }
                    if (sonuc.islemSonuc)
                    {

                        sonuc = MetaSilmeKontrol(veriErisim, kullanan, form);

                        if (sonuc.islemSonuc)
                        {
                            sonuc = tab.SilMeta(form.kod);
                            if (sonuc.islemSonuc && form.meta != null && form.meta.Length > 0)
                            {
                                foreach (ArsivMeta meta in form.meta)
                                {
                                    meta.arsivKod = form.kod;
                                    sonuc = tab.Kaydet(meta);
                                    if (!sonuc.islemSonuc)
                                        break;
                                }
                            }
                        }

                    }
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
                OrtakFonksiyonlar.HataStrYaz("{611A636D-D749-4CBD-AF13-30042A79A2B4} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Arsiv form)
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
            TABArsiv tab = new TABArsiv(ve);

            ve.BeginTransaction();
            try
            {
                Arsiv[] arsivListe = Listele(veriErisim, kullanan, new Arsiv() { kodYol = "%" + form.kod + "-%" });
                foreach (Arsiv item in arsivListe)
                {
                    sonuc = tab.SilYetki(item.kod);
                    if (sonuc.islemSonuc)
                    {
                        sonuc = MetaSilmeKontrol(veriErisim, kullanan, item);
                        if (sonuc.islemSonuc)
                            sonuc = tab.SilMeta(item.kod);
                    }
                    if (sonuc.islemSonuc)
                        sonuc = tab.Sil(item);
                    else
                        break;
                }

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
                OrtakFonksiyonlar.HataStrYaz("{3B99BD65-2350-4899-9F53-D9BA7D693830} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Arsiv Oku(VeriErisim veriErisim, Kullanici kullanan, Arsiv form, bool metaDetay = false)
        {
            if (form.kod == 0) return new Arsiv();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            Arsiv[] list = EVRArsiv.Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            ArsivYetki[] listYetkiler = EVRArsiv.ListeleYetki(ve, kullanan, form.kod);

            if (listYetkiler != null && listYetkiler.Length > 0)
                form.yetki = listYetkiler;

            ArsivMeta[] listMetalar = EVRArsiv.ListeleMeta(ve, kullanan, form.kod, metaDetay);

            if (listMetalar != null && listMetalar.Length > 0)
                form.meta = listMetalar;

            ve.VeriErisimKapat();

            return form;
        }

        private static ArsivYetki[] ListeleYetki(VeriErisim veriErisim, Kullanici kullanan, int arsivKod)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABArsiv tab = new TABArsiv(ve);

            IDataReader dr = tab.ListeleYetki(arsivKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    ArsivYetki form = new ArsivYetki();
                    form.arsivKod = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlarYetki.ARSIVKOD, 0);
                    form.yetkiDeger = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlarYetki.YETKI, "");
                    form.yetkiTip = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlarYetki.YETKITIP, 0);
                    form.aciklama = VeriErisim.DegerAl(dr, (int)TABArsiv.ENUMAlanlarYetki.ACIKLAMA, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (ArsivYetki[])liste.objeler.ToArray(typeof(ArsivYetki));
        }

        private static ArsivMeta[] ListeleMeta(VeriErisim veriErisim, Kullanici kullanan, int arsivKod, bool detay = false)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABArsiv tab = new TABArsiv(ve);

            IDataReader dr = tab.ListeleMeta(arsivKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    ArsivMeta form = new ArsivMeta();
                    form.arsivKod = arsivKod;
                    form.meta.kod = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.KOD, 0);
                    form.meta.ad = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.AD, "");
                    if (detay)
                    {
                        form.meta.tip = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.TIP, 0);
                        form.meta.zorunlu = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.ZORUNLU, 0);
                        form.meta.detay = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.DETAY, "");
                        form.meta.klasor = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.KLASOR, 0);
                        form.meta.grupKod = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.GRUPKOD, 0);
                        form.meta.grupAd = VeriErisim.DegerAl(dr, (int)TABMeta.ENUMAlanlar.GRUPKOD + 1, "");
                    }

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (ArsivMeta[])liste.objeler.ToArray(typeof(ArsivMeta));
        }

        //Meta silme kontrolü
        private static Sonuc MetaSilmeKontrol(VeriErisim veriErisim, Kullanici kullanan, Arsiv form)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABArsiv tab = new TABArsiv(ve);

            ArsivMeta[] arsivMeta = ListeleMeta(null, kullanan, form.kod);
            if (arsivMeta != null && arsivMeta.Length > 0)
            {
                foreach (ArsivMeta item in arsivMeta)
                {
                    bool itemVar = false;
                    if (form.meta != null && form.meta.Length > 0)
                    {
                        foreach (ArsivMeta meta in form.meta)
                            if (meta.meta.kod == item.meta.kod) { itemVar = true; break; }
                    }

                    if (itemVar == false && tab.ArsivDosyasinaKayitliMetaVarMi(form.kod, item.meta.kod) > 0)  //Arşive dosya eklendi ise meta kullanıldığında silmesine izin verme?
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = " '" + item.meta.ad + "' isimli meta, seçilen arşivin dosyalarında kullanılıyor. Öncelikle arşivin dosyalarını silmelisiniz. <br>";
                        break;
                    }
                }
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}

using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System.Collections;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRRol
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Rol form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.ad))
                hata += "Rol adı boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABRol tab = new TABRol(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = (form.kod == 0) ? tab.Kaydet(form) : tab.Guncelle(form);

                if (sonuc.islemSonuc && form.kod > 0)
                {
                    sonuc = tab.SilYetki(form.kod);
                    if (sonuc.islemSonuc && form.yetkiler != null && form.yetkiler.Length > 0)
                    {
                        foreach (RolYetki yetki in form.yetkiler)
                        {
                            yetki.rol.kod = form.kod;
                            sonuc = tab.Kaydet(yetki);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }

                    if (sonuc.islemSonuc)
                    {
                        sonuc = tab.SilKullanici(form.kod);
                        if (sonuc.islemSonuc && form.kullanicilar != null && form.kullanicilar.Length > 0)
                        {
                            foreach (KullaniciRol kul in form.kullanicilar)
                            {
                                kul.rol.kod = form.kod;
                                sonuc = tab.Kaydet(kul);
                                if (!sonuc.islemSonuc)
                                    break;
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
                OrtakFonksiyonlar.HataStrYaz("{8A81434C-1E45-4C4F-A8BD-6E2E0020140F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Rol form)
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
            TABRol tab = new TABRol(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{1E981363-6CE2-4830-BAF8-F93461B89B3A} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Rol[] Listele(VeriErisim veriErisim, Kullanici kullanan, Rol form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABRol tab = new TABRol(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new Rol();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABRol.ENUMAlanlar.KOD, 0);
                    form.ad = VeriErisim.DegerAl(dr, (int)TABRol.ENUMAlanlar.AD, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            Rol[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (Rol[])liste.objeler.ToArray(typeof(Rol));

            return arr;
        }
        private static RolYetki[] ListeleYetki(VeriErisim veriErisim, Kullanici kullanan, RolYetki form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABRol tab = new TABRol(ve);

            IDataReader dr = tab.ListeleYetki(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new RolYetki();
                    form.rol.kod = VeriErisim.DegerAl(dr, (int)TABRol.ENUMAlanlarYetki.ROLKOD, 0);
                    form.yetkiEnum.kod = VeriErisim.DegerAl(dr, (int)TABRol.ENUMAlanlarYetki.YETKIKOD, 0);
                    form.yetkiDeger = VeriErisim.DegerAl(dr, (int)TABRol.ENUMAlanlarYetki.DEGER, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            RolYetki[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (RolYetki[])liste.objeler.ToArray(typeof(RolYetki));

            return arr;
        }
        private static KullaniciRol[] ListeleKullanici(VeriErisim veriErisim, Kullanici kullanan, KullaniciRol form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABRol tab = new TABRol(ve);

            IDataReader dr = tab.ListeleKullanici(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    KullaniciRol detay = new KullaniciRol();
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABRol.ENUMAlanlarKullanici.TCKIMLIKNO, "");
                    detay.rol.kod = VeriErisim.DegerAl(dr, (int)TABRol.ENUMAlanlarKullanici.ROLKOD, 0);
                    detay.ad = VeriErisim.DegerAl(dr, (int)TABRol.ENUMAlanlarKullanici.AD, "");
                    detay.soyad = VeriErisim.DegerAl(dr, (int)TABRol.ENUMAlanlarKullanici.SOYAD, "");

                    liste.objeler.Add(detay);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            KullaniciRol[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (KullaniciRol[])liste.objeler.ToArray(typeof(KullaniciRol));

            return arr;
        }

        public static Rol Oku(VeriErisim veriErisim, Kullanici kullanan, Rol form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            Rol[] list = Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            RolYetki[] listYetki = ListeleYetki(ve, kullanan, new RolYetki() { rol = { kod = form.kod } });

            if (listYetki != null && listYetki.Length > 0)
                form.yetkiler = listYetki;

            KullaniciRol[] listKullanici = ListeleKullanici(ve, kullanan, new KullaniciRol() { rol = { kod = form.kod } });

            if (listKullanici != null && listKullanici.Length > 0)
                form.kullanicilar = listKullanici;

            ve.VeriErisimKapat();

            return form;
        }

        public static RolYetki RolYetkiOku(VeriErisim veriErisim, Kullanici kullanan, RolYetki form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            RolYetki[] list = ListeleYetki(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            ve.VeriErisimKapat();

            return form;
        }


        public static int KullaniciYetkiDegerBul(VeriErisim veriErisim, ENUMYetkiler yetki, string tcKimlikNo)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABRol tab = new TABRol(ve);
            
            int deger = tab.KullaniciYetkiDegerBul(yetki, tcKimlikNo);
            
            ve.VeriErisimKapat();
            
            return deger;
        }



    }
}

using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using TNS.PER;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Ek ders bordro bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    class HRCEkDersBordro
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bordro bilgisini veritabanından okur 
        /// ve EkDersBordrolar nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="bankaListesi">Eğer <c>true</c> verilirse banka bilgilerini de getirir</param>
        /// <returns>İstenilen kriterdeki, ek ders bordro bilgisini EkDersBordrolar nesnesi olarak tutan nesne</returns>
        public EkDersBordrolar Listele(Kullanici islemiYapan, EkDersBordro f, bool bankaListesi)
        {
            EkDersBordrolar fler = new EkDersBordrolar();
            string hata = "";

            hata = Kontrol(f);
            if (hata != "")
            {
                fler.sonuc.hataStr = hata;
                fler.sonuc.islemSonuc = false;
                return fler;
            }
            /*
            if (!OMKYetkiServis.OdemeEmriGormeYetkisiVarMi(islemiYapan, f.muhasebe, f.birim))
            {
                fler.sonuc.hataStr = hata;
                fler.sonuc.islemSonuc = false;
                return fler;
            }
            */

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemiYapan));

            TABEkDersBordro tab = new TABEkDersBordro(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(f, bankaListesi);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                fler.sonuc.islemSonuc = false;
                fler.sonuc.hataStr = e.Message;

                ve.VeriErisimKapat();

                return fler;
            }

            while (r.Read())
            {
                EkDersBordro fb = new EkDersBordro();

                fb.yil = VeriErisim.DegerAl(r, 0, 0);
                fb.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                fb.birim = VeriErisim.DegerAl(r, 2, "");
                fb.surecNo = VeriErisim.DegerAl(r, 3, "");
                fb.ilgiliYil = VeriErisim.DegerAl(r, 4, 0);
                fb.ilgiliAy = VeriErisim.DegerAl(r, 5, 0);
                fb.tcKimlikNo = VeriErisim.DegerAl(r, 6, "");
                fb.haftaiciSaat = VeriErisim.DegerAl(r, 7, 0.0);
                fb.haftasonuSaat = VeriErisim.DegerAl(r, 8, 0.0);
                fb.haftaiciUcret = VeriErisim.DegerAl(r, 9, 0.0);
                fb.haftasonuUcret = VeriErisim.DegerAl(r, 10, 0.0);
                fb.gelirVergisi = VeriErisim.DegerAl(r, 11, 0.0);
                fb.gelirVergisiOran = VeriErisim.DegerAl(r, 12, 0.0);
                fb.damgaVergisi = VeriErisim.DegerAl(r, 13, 0.0);
                fb.kesinti = VeriErisim.DegerAl(r, 14, 0.0);

                fb.adSoyad = VeriErisim.DegerAl(r, 15, "");
                fb.unvan = VeriErisim.DegerAl(r, 16, "");
                fb.istihdamSekli = VeriErisim.DegerAl(r, 17, 0);
                fb.banka = VeriErisim.DegerAl(r, 18, "");
                fb.sube = VeriErisim.DegerAl(r, 19, "");
                fb.hesapNo = VeriErisim.DegerAl(r, 20, "");
                fb.bankaAdi = VeriErisim.DegerAl(r, 21, "");
                fb.subeAdi = VeriErisim.DegerAl(r, 22, "");

                if (string.IsNullOrEmpty(fb.bankaAdi))
                    fb.bankaAdi = VeriErisim.DegerAl(r, 23, "");
                if (string.IsNullOrEmpty(fb.subeAdi))
                    fb.subeAdi = VeriErisim.DegerAl(r, 24, "");
                if (string.IsNullOrEmpty(fb.hesapNo))
                    fb.hesapNo = VeriErisim.DegerAl(r, 25, "");
                if (string.IsNullOrEmpty(fb.adSoyad))
                    fb.adSoyad = VeriErisim.DegerAl(r, 26, "") + " " + VeriErisim.DegerAl(r, 27, "");
                if (string.IsNullOrEmpty(fb.unvan))
                    fb.unvan = VeriErisim.DegerAl(r, 28, "");

                fler.liste.Add(fb);
            }

            fler.sonuc.islemSonuc = true;

            if (fler.liste.Count == 0)
                fler.sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00072;

            ve.VeriErisimKapat();

            return fler;
        }

        public Sonuc Kaydet(Kullanici kullanan, EkDersBordrolar f)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013; 
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            hata = Kontrol((EkDersBordro)f.liste[0]);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!OMKYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, ((EkDersBordro)f.liste[0]).muhasebe, ((EkDersBordro)f.liste[0]).birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABEkDersBordro tab = new TABEkDersBordro(ve);

            sonuc = tab.Sil((EkDersBordro)f.liste[0]);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            foreach (EkDersBordro fp in f.liste)
            {
                sonuc = tab.Kaydet(fp);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
            }
            else
            {
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders bordro kesinti bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Bordro hazırlananacak bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KesintiKaydet(Kullanici kullanici, EkDersBordrolar f, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            if (f.liste.Count == 0)
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00066;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            hata = Kontrol((EkDersBordro)f.liste[0]);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!OMKYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, ((EkDersBordro)f.liste[0]).muhasebe, ((EkDersBordro)f.liste[0]).birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABEkDersBordro tab = new TABEkDersBordro(ve);

            foreach (EkDersBordro fp in f.liste)
            {
                sonuc = tab.KesintiKaydet(fp);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
            }
            else
            {
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Bordro hazırlananacak bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanici, EkDersBordro f, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";

            hata = Kontrol(f);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!OMKYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, f.muhasebe, f.birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABEkDersBordro tab = new TABEkDersBordro(ve);

            sonuc = tab.Sil(f);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        private string Kontrol(EkDersBordro f)
        {
            string hata = "";

            if (f.yil == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003;
            }

            if (f.muhasebe == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004;
            }
            if (f.birim == "")
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005;
            }
            if (f.surecNo == "")
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00067;
            }
            if (hata == "")
            {
                string ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBE", f.muhasebe.ToString(), true, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00068, f.muhasebe);
                }

                ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", f.birim.Trim() + "-" + f.muhasebe, false, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00069, f.birim);
                }
            }

            return hata;
        }
    }
}

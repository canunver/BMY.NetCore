using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using System.Resources;

namespace TNS.IKY.IsKurallari
{
    class IKYPersonelTerfi
    {
        public ObjectArray Listele(Kullanici kullanan, Terfi p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABPersonelTerfi tab = new TABPersonelTerfi(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            int AlanlarSonIndex = (Enum.GetNames(typeof(TABPersonelTerfi.ENUMAlanlar))).Length;

            while (r.Read())
            {
                Terfi t = new Terfi();

                t.kod = VeriErisim.DegerAl(r, (int)TABPersonelTerfi.ENUMAlanlar.KOD, 0);
                t.pKod = VeriErisim.DegerAl(r, (int)TABPersonelTerfi.ENUMAlanlar.PKOD, 0);
                t.pKadroKod = VeriErisim.DegerAl(r, (int)TABPersonelTerfi.ENUMAlanlar.PKADROKOD, 0);
                t.pYeniKadroKod = VeriErisim.DegerAl(r, (int)TABPersonelTerfi.ENUMAlanlar.PYENIKADROKOD, 0);
                t.durum = VeriErisim.DegerAl(r, (int)TABPersonelTerfi.ENUMAlanlar.DURUM, 0);
                t.hizmetSebebi = VeriErisim.DegerAl(r, (int)TABPersonelTerfi.ENUMAlanlar.SEBEP, 0);
                t.tip = VeriErisim.DegerAl(r, (int)TABPersonelTerfi.ENUMAlanlar.TIP, 0);
                t.yil = VeriErisim.DegerAl(r, (int)TABPersonelTerfi.ENUMAlanlar.YIL, 0);
                t.ay = VeriErisim.DegerAl(r, (int)TABPersonelTerfi.ENUMAlanlar.AY, 0);
                t.kimlikNo = VeriErisim.DegerAl(r, AlanlarSonIndex, "");
                t.sicilNo = VeriErisim.DegerAl(r, AlanlarSonIndex + 1, "");
                t.ad = VeriErisim.DegerAl(r, AlanlarSonIndex + 2, "");
                t.birim = VeriErisim.DegerAl(r, AlanlarSonIndex + 3, "");
                t.unvan = VeriErisim.DegerAl(r, AlanlarSonIndex + 4, "");
                t.kadroDerece = VeriErisim.DegerAl(r, AlanlarSonIndex + 5, 0);
                t.kadroTarih = VeriErisim.DegerAl(r, AlanlarSonIndex + 6);
                t.odemeyeEsasDerece = VeriErisim.DegerAl(r, AlanlarSonIndex + 7, 0);
                t.odemeyeEsasKademe = VeriErisim.DegerAl(r, AlanlarSonIndex + 8, 0);
                t.odemeyeEsasTarih = VeriErisim.DegerAl(r, AlanlarSonIndex + 9);
                t.kazanilmisHakDerece = VeriErisim.DegerAl(r, AlanlarSonIndex + 10, 0);
                t.kazanilmisHakKademe = VeriErisim.DegerAl(r, AlanlarSonIndex + 11, 0);
                t.kazanilmisHakTarih = VeriErisim.DegerAl(r, AlanlarSonIndex + 12);
                t.emekliyeEsasDerece = VeriErisim.DegerAl(r, AlanlarSonIndex + 13, 0);
                t.emekliyeEsasKademe = VeriErisim.DegerAl(r, AlanlarSonIndex + 14, 0);
                t.emekliyeEsasTarih = VeriErisim.DegerAl(r, AlanlarSonIndex + 15);
                t.ogrenim = VeriErisim.DegerAl(r, AlanlarSonIndex + 16, "");
                t.yukselecegiDerece = VeriErisim.DegerAl(r, AlanlarSonIndex + 17, 0);
                t.sebepAd = VeriErisim.DegerAl(r, AlanlarSonIndex + 18, "");
                t.YeniOdemeyeEsasDerece = VeriErisim.DegerAl(r, AlanlarSonIndex + 19, 0);
                t.YeniOdemeyeEsasKademe = VeriErisim.DegerAl(r, AlanlarSonIndex + 20, 0);
                t.YeniOdemeyeEsasEkGosterge = VeriErisim.DegerAl(r, AlanlarSonIndex + 21, 0);
                t.YeniKazanilmisHakDerece = VeriErisim.DegerAl(r, AlanlarSonIndex + 22, 0);
                t.YeniKazanilmisHakKademe = VeriErisim.DegerAl(r, AlanlarSonIndex + 23, 0);
                t.YeniKazanilmisHakEkGosterge = VeriErisim.DegerAl(r, AlanlarSonIndex + 24, 0);
                t.YeniEmekliyeEsasDerece = VeriErisim.DegerAl(r, AlanlarSonIndex + 25, 0);
                t.YeniEmekliyeEsasKademe = VeriErisim.DegerAl(r, AlanlarSonIndex + 26, 0);
                t.YeniEmekliyeEsasEkGosterge = VeriErisim.DegerAl(r, AlanlarSonIndex + 27, 0);
                t.statu = VeriErisim.DegerAl(r, AlanlarSonIndex + 28, "");
                t.kazanilmisHakEkGosterge = VeriErisim.DegerAl(r, AlanlarSonIndex + 29, 0);
                t.emekliyeEsasEkGosterge = VeriErisim.DegerAl(r, AlanlarSonIndex + 30, 0);
                t.odemeyeEsasEkGosterge = VeriErisim.DegerAl(r, AlanlarSonIndex + 31, 0);

                pVAN.objeler.Add(t);

            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc Kaydet(Kullanici kullanan, Terfi p)
        {
            Sonuc sonuc = new Sonuc();

            //if (string.IsNullOrEmpty(p.ogrenimKod.ToString()))
            //{
            //    sonuc.hataStr = "Eğitim seviyesi boş bırakılamaz";
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonelTerfi tab = new TABPersonelTerfi(ve);

            if (p.kod > 0)
                sonuc = tab.Guncelle(p);
            else
            {
                p.kod = TABGenel.SayacDegerAlInt(ve, TABPersonelTerfi.TABLOAD);
                sonuc = tab.Kaydet(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc();

            if (kod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            //TABBelgeler tabBelge = new TABBelgeler(ve);
            //sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod = p.personelKod, tip = (int)ENUMBELGEFORMTIP.PERSONELOGRENIM });

            if (sonuc.islemSonuc)
            {
                TABPersonelTerfi tab = new TABPersonelTerfi(ve);
                sonuc = tab.Sil(kod);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray TerfisiGelenler(Kullanici kullanan, string yilAy, int tip)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelTerfi tab = new TABPersonelTerfi(ve);

            IDataReader r = tab.TerfisiGelenler(yilAy, tip);
            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            while (r.Read())
            {
                Terfi terfi = new Terfi();

                terfi.pKadroKod = VeriErisim.DegerAl(r, 0, 0);
                terfi.pKod = VeriErisim.DegerAl(r, 1, 0);
                terfi.kimlikNo = VeriErisim.DegerAl(r, 2, "");
                terfi.sicilNo = VeriErisim.DegerAl(r, 3, "");
                terfi.ad = VeriErisim.DegerAl(r, 4, "");
                terfi.birim = VeriErisim.DegerAl(r, 5, "");
                terfi.unvan = VeriErisim.DegerAl(r, 6, "");
                terfi.kadroDerece = VeriErisim.DegerAl(r, 7, 0);
                terfi.kadroTarih = VeriErisim.DegerAl(r, 8);
                terfi.odemeyeEsasDerece = VeriErisim.DegerAl(r, 9, 0);
                terfi.odemeyeEsasKademe = VeriErisim.DegerAl(r, 10, 0);
                terfi.odemeyeEsasTarih = VeriErisim.DegerAl(r, 11);

                terfi.kazanilmisHakDerece = VeriErisim.DegerAl(r, 12, 0);
                terfi.kazanilmisHakKademe = VeriErisim.DegerAl(r, 13, 0);
                terfi.kazanilmisHakTarih = VeriErisim.DegerAl(r, 14);

                terfi.emekliyeEsasDerece = VeriErisim.DegerAl(r, 15, 0);
                terfi.emekliyeEsasKademe = VeriErisim.DegerAl(r, 16, 0);
                terfi.emekliyeEsasTarih = VeriErisim.DegerAl(r, 17);

                terfi.tip = VeriErisim.DegerAl(r, 18, 0);
                terfi.ogrenim = VeriErisim.DegerAl(r, 19, "");
                terfi.yukselecegiDerece = VeriErisim.DegerAl(r, 20, 0);

                bool ekle = false;
                if ((tip == 1 || tip == 3) && !terfi.KHASonMu())
                    ekle = true;
                if ((tip == 2 || tip == 3) && !terfi.EKEASonMu())
                    ekle = true;
                if (ekle)
                    pVAN.objeler.Add(terfi);
            }
            r.Close();
            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc EKEKHATerfiEt(VeriErisim veriErisim, Kullanici kullanan, Terfi terfi)
        {
            Sonuc sonuc = new Sonuc();
            IKYPersonelKadro kadro = new IKYPersonelKadro();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            try
            {

                TABPersonelTerfi tab = new TABPersonelTerfi(ve);
                TABPersonelKadro tabK = new TABPersonelKadro(ve);

                if (terfi.durum == OrtakFonksiyonlar.ConvertToInt(ENUMTerfiDurumu.Red, 0))
                {
                    sonuc = tab.GuncelleTerfiDurumu(terfi);
                }
                else if (terfi.durum == OrtakFonksiyonlar.ConvertToInt(ENUMTerfiDurumu.Onay, 0))
                {
                    ObjectArray arrKadro = IKYPersonelKadro.ListeleKadro(ve, kullanan, new PersonelKadro { kod = terfi.pKadroKod, personelKod = terfi.pKod }, true, true);
                    PersonelKadro k = (PersonelKadro)arrKadro.objeler[0];

                    bool KHAGuncelle = false;
                    bool EKEAGuncelle = false;
                    PersonelKadro yeniKadro = k;
                    int eskiKod = k.kod;
                    DateTime eskiAtamaTarihi = k.atamaTarihiDT;
                    yeniKadro = k;
                    yeniKadro.kod = 0;
                    yeniKadro.eskiKod = eskiKod;
                    yeniKadro.kadroDurum = 0;
                    yeniKadro.atamaTarihi = new TNSDateTime(System.DateTime.Now);
                    yeniKadro.aciklama = "";
                    yeniKadro.ayrilmaTarihi = null;
                    yeniKadro.ayrilmaNedeni = "";
                    yeniKadro.ayrilmaGerekcesi = 0;
                    yeniKadro.hizmetSebebiKod = terfi.hizmetSebebi;
                    yeniKadro.terfiMi = true;
                    yeniKadro.hizmetGunSayisi = 0;
                    yeniKadro.kadroDurum = ENUMKadroDurumu.YENI;

                    if (terfi.tip == OrtakFonksiyonlar.ConvertToInt(ENUMTerfiTipi.KHA, 0) || terfi.tip == OrtakFonksiyonlar.ConvertToInt(ENUMTerfiTipi.KHAEKEA, 0))
                    {
                        KHAGuncelle = DereceKademeArttir(ref yeniKadro._kazanilmisHakKademe, ref yeniKadro._kazanilmisHakDerece, yeniKadro.kadroDerece);
                        yeniKadro.kazanilmisHakTarih = new TNSDateTime(System.DateTime.Now.Date);
                        yeniKadro.kazanilmisGosterge = TNS.IKY.Arac.GostergeBul(yeniKadro._kazanilmisHakDerece, yeniKadro._kazanilmisHakKademe);
                    }
                    if (terfi.tip == OrtakFonksiyonlar.ConvertToInt(ENUMTerfiTipi.EKEA, 0) || terfi.tip == OrtakFonksiyonlar.ConvertToInt(ENUMTerfiTipi.KHAEKEA, 0))
                    {
                        EKEAGuncelle = DereceKademeArttir(ref yeniKadro._emekliyeEsasKademe, ref yeniKadro._emekliyeEsasDerece, yeniKadro.kadroDerece);
                        yeniKadro.emekliyeEsasTarih = new TNSDateTime(System.DateTime.Now.Date);
                        yeniKadro.emekliyeEsasGosterge = TNS.IKY.Arac.GostergeBul(yeniKadro._emekliyeEsasDerece, yeniKadro._emekliyeEsasKademe);
                    }

                    if (KHAGuncelle || EKEAGuncelle)
                    {
                        sonuc = IKYPersonelKadro.KaydetKadro(ve, kullanan, yeniKadro, true);
                        if (sonuc.islemSonuc)
                        {
                            yeniKadro.kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);
                            TNSCollection atamalar = new TNSCollection();
                            atamalar.Add(yeniKadro);
                            sonuc = IKYPersonelKadro.GuncelleOnayBilgileri(ve, kullanan, atamalar, 5, false, false);
                        }
                        if (sonuc.islemSonuc)
                        {
                            terfi.pYeniKadroKod = yeniKadro.kod;
                            sonuc = tab.GuncelleTerfiDurumu(terfi);
                        }
                    }
                    else
                    {
                        sonuc.DegerAta("Terfi işlemi yükselme işlemi olmadığı için yapılamadı.");
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                    //??Yedek artış mantığı varmı öğren ona göre aç??
                    //else
                    //{
                    //    if (k.yedekArtis != yeniKadro.yedekArtis)
                    //        sonuc = tabK.GuncelleYedekArtis(new PersonelKadro { kod = terfi.pKadroKod, yedekArtis = yeniKadro.yedekArtis });
                    //}
                }
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.Message);
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
            {
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        protected bool DereceKademeArttir(ref int terfiKademe, ref int terfiDerece, int kadroDerece)
        {
            int derece = terfiDerece;
            int kademe = terfiKademe;
            if (Terfi.SonMu(derece, kademe)) return false;
            int maxKademe = maxKademeGetir(terfiDerece);
            if (kadroDerece < terfiDerece && terfiDerece > 1)
                maxKademe = 3;

            if (terfiKademe < maxKademe)
            {
                terfiKademe = terfiKademe + 1;
            }
            else if (terfiDerece > 1)
            {
                terfiKademe = 1;
                terfiDerece--;
            }
            if (derece != terfiDerece || kademe != terfiKademe)
                return true;
            return false;
        }

        public int maxKademeGetir(int terfiDerece)
        {
            if (terfiDerece > 3)
                return 9;
            else if (terfiDerece == 3)
                return 8;
            else if (terfiDerece == 2)
                return 6;
            else if (terfiDerece == 1)
                return 4;
            else
                return 0;
        }
    }
}

using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYPersonelLojman
    {
        //public ObjectArray eskiTmp; //Tarihçe için

        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriIKY";

        public static ObjectArray Listele(string okunanXML)
        {
            ObjectArray pVAN = new ObjectArray();

            IDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                int sonKolon = (Enum.GetNames(typeof(TABPersonelLojman.ENUMAlanlar))).Length;
                while (r.Read())
                {
                    PersonelLojmani d = new PersonelLojmani();
                    d.kod = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.KOD, 0);
                    d.personelKod = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.PKOD, 0);
                    d.daireKod = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.DKOD, 0);
                    d.lojmanTipi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.LojmanTipi, 0);
                    d.basTarihi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.BasTarihi);
                    d.bitTarihi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.BitTarihi);
                    d.adres = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.Adres, "");
                    d.kesintiVar = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.KesintiVar, 0);
                    d.tazminatiVar = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.TazminatiVar, 0);
                    d.suParasiKesintisi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.SuParasiKesiliyor, 0);
                    d.konutYardimTarihi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.KonutYardimTarihi);
                    d.ozelTarih = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.OzelTarih);
                    d.adresTipi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.AdresTipi, 0);
                    d.tahakkuk = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.IsTahakkuk, 0);
                    d.tahsisTuru = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.TahsisTuru, "");
                    d.tashisTip = (ENUMTahsisTipi)VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.TahsisTip, 0);
                    d.rezervTarih = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.RezervTarihi);
                    d.lojmanTipiEtiketi = VeriErisim.DegerAl(r, sonKolon, "");
                    d.daireNo = VeriErisim.DegerAl(r, sonKolon + 1, 0);
                    d.blokNo = VeriErisim.DegerAl(r, sonKolon + 2, "");
                    d.siteAdi = VeriErisim.DegerAl(r, sonKolon + 3, "");
                    d.blokKod = VeriErisim.DegerAl(r, sonKolon + 4, 0);
                    d.siteKod = VeriErisim.DegerAl(r, sonKolon + 5, 0);
                    d.daireMetreKare = VeriErisim.DegerAl(r, sonKolon + 6, 0);
                    d.yakitKesilecek = VeriErisim.DegerAl(r, sonKolon + 7, 0);
                    d.daireTuru = VeriErisim.DegerAl(r, sonKolon + 8, 0);

                    pVAN.objeler.Add(d);
                }
                r.Close();
            }
            return pVAN;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PersonelLojmani p)
        {
            ObjectArray pVAN = new ObjectArray();

            pVAN.sonuc = IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, p.personelKod, null, ENUMYetkiIslem.Lojman);
            if (!pVAN.sonuc.islemSonuc)
            {
                return pVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABPersonelLojman tab = new TABPersonelLojman(ve);

            IDataReader r = tab.Listele(p);

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            int sonKolon = (Enum.GetNames(typeof(TABPersonelLojman.ENUMAlanlar))).Length;

            while (r.Read())
            {
                PersonelLojmani d = new PersonelLojmani();
                d.kod = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.KOD, 0);
                d.personelKod = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.PKOD, 0);
                d.daireKod = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.DKOD, 0);
                d.lojmanTipi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.LojmanTipi, 0);
                d.basTarihi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.BasTarihi);
                d.bitTarihi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.BitTarihi);
                d.adres = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.Adres, "");
                d.kesintiVar = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.KesintiVar, 0);
                d.tazminatiVar = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.TazminatiVar, 0);
                d.suParasiKesintisi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.SuParasiKesiliyor, 0);
                d.konutYardimTarihi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.KonutYardimTarihi);
                d.ozelTarih = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.OzelTarih);
                d.adresTipi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.AdresTipi, 0);
                d.tahakkuk = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.IsTahakkuk, 0);
                d.tahsisTuru = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.TahsisTuru, "");
                d.tashisTip = (ENUMTahsisTipi)VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.TahsisTip, 0);
                d.rezervTarih = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.RezervTarihi);
                d.oncelikDerecesi = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.OncelikDerecesi, 0);
                d.aciklama = VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.Aciklama, "");
                d.lojmanTipiEtiketi = VeriErisim.DegerAl(r, sonKolon, "");
                d.daireNo = VeriErisim.DegerAl(r, sonKolon + 1, 0);
                d.blokNo = VeriErisim.DegerAl(r, sonKolon + 2, "");
                d.siteAdi = VeriErisim.DegerAl(r, sonKolon + 3, "");
                d.blokKod = VeriErisim.DegerAl(r, sonKolon + 4, 0);
                d.siteKod = VeriErisim.DegerAl(r, sonKolon + 5, 0);
                d.daireMetreKare = VeriErisim.DegerAl(r, sonKolon + 6, 0);
                d.yakitKesilecek = VeriErisim.DegerAl(r, sonKolon + 7, 0);

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public static ObjectArray ListelePLojmanK(VeriErisim veriErisim, Kullanici kullanan, PersonelLojmani p)
        {
            ObjectArray pVAN = new ObjectArray();
            //pVAN.sonuc = IKYYetki.YetkiVarMiOkumayaTC(kullanan, kullanan.TCKIMLIKNO, null, ENUMYetkiIslem.Lojman);
            //pVAN.sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, null, ENUMYetkiIslem.Lojman);
            //if (!pVAN.sonuc.islemSonuc)
            //{
            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABPersonelLojman tab = new TABPersonelLojman(ve);

            IDataReader r = tab.ListelePLojmanK(p);

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            while (r.Read())
            {
                PersonelLojmani d = new PersonelLojmani();
                d.siteKod = VeriErisim.DegerAl(r, 0, 0);
                d.siteAdi = VeriErisim.DegerAl(r, 1, "");
                d.blokKod = VeriErisim.DegerAl(r, 2, 0);
                d.blokNo = VeriErisim.DegerAl(r, 3, "");
                d.daireKod = VeriErisim.DegerAl(r, 4, 0);
                d.daireNo = VeriErisim.DegerAl(r, 5, 0);
                d.personelKod = VeriErisim.DegerAl(r, 6, 0);
                d.personelAd = VeriErisim.DegerAl(r, 7, "");
                d.unvanAd = VeriErisim.DegerAl(r, 8, "");
                d.kod = VeriErisim.DegerAl(r, 9, 0);

                pVAN.objeler.Add(d);
            }
            r.Close();

            if (p.giderleriOku)
            {

                SiteGider sg = new SiteGider();
                sg.yil = p.yil;
                sg.ay = p.ay;
                sg.siteKod = p.siteKod;
                ObjectArray sgler = IKYSiteYonetimi.ListeleSiteGider(ve, kullanan, sg);

                foreach (PersonelLojmani okunan in pVAN.objeler)
                {
                    okunan.kesintiler = PLojmanBazliButunGiderler(ve, kullanan, p.yil, p.ay, okunan.kod, sgler);
                }
            }

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public static SozlukGider PLojmanBazliButunGiderler(VeriErisim ve, Kullanici kullanan, int yil, int ay, int plojmanKod, ObjectArray sgler)
        {
            SozlukGider gider = new SozlukGider();
            gider.kesintiler = new TNSCollection();

            foreach (SiteGider g in sgler.objeler)
            {
                PersonelSiteKesinti psk = new PersonelSiteKesinti();
                psk.giderKod = g.giderKod;
                if (g.otomatik)
                    psk.aktif = 1;

                gider.kesintiler.Add(psk);
            }
            TABIKYSiteYonetimi tab = new TABIKYSiteYonetimi(ve);

            IDataReader dr = tab.ListelePersonelKesinti(new PersonelSiteKesinti { pLojmanKod = plojmanKod, yil = yil, ay = ay });
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            while (dr.Read())
            {
                int giderKod = VeriErisim.DegerAl(dr, 3, 0);
                foreach (PersonelSiteKesinti k in gider.kesintiler)
                {
                    if (giderKod == k.giderKod)
                    {
                        PersonelSiteKesinti detay = k;
                        detay.personelKod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.PKOD, 0);
                        detay.yil = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.YIL, 0);
                        detay.ay = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.AY, 0);
                        detay.giderKod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.GIDERKOD, 0);
                        detay.pLojmanKod = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.PLOJMANKOD, 0);
                        detay.aktif = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.AKTIF, 0);
                        detay.endex = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.ENDEX, 0.0);
                        detay.tutar = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.TUTAR, 0.0);
                        detay.elden = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.ELDEN, 0.0);
                        detay.aciklama = VeriErisim.DegerAl(dr, (int)TABIKYSiteYonetimi.ENUMALANLARKESINTI.ACIKLAMA, "");

                        break;
                    }
                }
            }
            dr.Close();

            return gider;
        }

        public static ObjectArray ListeleLojmanTuru(VeriErisim veriErisim, Kullanici kullanan, LojmanTuru p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABPersonelLojman tab = new TABPersonelLojman(ve);

            IDataReader r = tab.ListeleLojmanTuru(p);

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            //int sonKolon = (Enum.GetNames(typeof(TABPersonelLojman.ENUMAlanlar))).Length;

            while (r.Read())
            {
                LojmanTuru d = new LojmanTuru();

                d.kod = VeriErisim.DegerAl(r, 0, 0);
                d.etiket = VeriErisim.DegerAl(r, 1, "");
                d.toplamYil = VeriErisim.DegerAl(r, 2, 0);

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PersonelLojmani p)
        {
            //object eski = null, yeni = null; //Tarihçe için

            Sonuc sonuc = new Sonuc();

            sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Lojman);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            string hata = "";
            hata = Kontrol(p);

            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ////Tarihçe ********************
            //this.eskiTmp = this.Listele(kullanan, p);
            //if (eskiTmp.objeler.Count == 1)
            //    eski = eskiTmp[0];

            //if (eski == null)
            //    eski = new PersonelLojmani();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            ////****************************

            ve.BeginTransaction();

            TABPersonelLojman tab = new TABPersonelLojman(ve);
            sonuc = KayitKontrol(ve, kullanan, p);

            if (!sonuc.islemSonuc)
            {
                sonuc.DegerAta(ve.hata);
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //ENUMTARIHCEISLEMTIP islemTip;
            if (p.kod > 0)
            {
                //islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.Guncelle(p);
            }
            else
            {
                //islemTip = ENUMTARIHCEISLEMTIP.KAYDET;

                p.kod = TABGenel.SayacDegerAlInt(ve, TABPersonelLojman.TABLOAD);
                sonuc = tab.Kaydet(p);

            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                //yeni = p;
                //Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.LOJMAN, p.personelKod, islemTip);

            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc KayitKontrol(VeriErisim ve, Kullanici kullanan, PersonelLojmani p)
        {
            Sonuc sonuc = new Sonuc();
            VarMi v = IKYLojmanSiteBlokDaire.DaireDoluMu(ve, kullanan, p.daireKod, p.kod);
            if (!v.s.islemSonuc || v.varMi)
            {
                if (v.s.islemSonuc)
                    sonuc.DegerAta("Dairede oturan personel bulunmaktadır.");
                else
                    sonuc.DegerAta(v.s.hataStr);

                return sonuc;
            }

            if (p.tashisTip == ENUMTahsisTipi.Tahsis)
            {
                v = PersonelinTahsisliLojmaniVarmi(ve, kullanan, p.personelKod, p.kod);
                if (!v.s.islemSonuc || v.varMi)
                {
                    if (v.s.islemSonuc)
                        sonuc.DegerAta("Personelin üzerine tahsisli başka bir lojman bulunmaktadır. Eğer lojmandan ayrıldı ise tahsisli lojmanın bitiş tarihini giriniz.");
                    else
                        sonuc.DegerAta(v.s.hataStr);

                    return sonuc;
                }
            }

            return sonuc;
        }

        public static VarMi PersonelinTahsisliLojmaniVarmi(VeriErisim ve, Kullanici kullanan, int pKod, int plKod)
        {
            TABPersonelLojman TAB = new TABPersonelLojman(ve);
            VarMi varMi = new VarMi();
            varMi.s = new Sonuc();
            varMi.varMi = false;

            IDataReader dr;
            dr = TAB.PersonelinTahsisliLojmaniVarmi(pKod, plKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                varMi.s.DegerAta(ve.hata);
                return varMi;
            }

            if (dr.Read())
                varMi.varMi = true;

            dr.Dispose();

            return varMi;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, PersonelLojmani p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Lojman);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            VarMi kesinti = IKYSiteYonetimi.PersonelSiteKesintisiVarMi(ve, kullanan, new PersonelSiteKesinti { pLojmanKod = p.kod });
            if (kesinti.varMi)
            {
                sonuc.DegerAta("Personel kesinti tablosunda bu personel lojmana ait kesinti mevcut olduğu için silme işlemi gerçekleştirilmedi.");
                return sonuc;
            }

            if (sonuc.islemSonuc)
            {
                TABPersonelLojman tab = new TABPersonelLojman(ve);
                sonuc = tab.Sil(p);
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

        private static string Kontrol(PersonelLojmani p)
        {
            string hata = "";

            if (p.daireKod <= 0)
                hata = "Daire alanı boş bırakılamaz.";

            if ((int)p.tashisTip <= 0)
                hata = "Tahsis tipi boş bırakılamaz.";

            return hata;
        }

        public static Sonuc RezervdenTahsisEt(VeriErisim veriErisim, Kullanici kullanan, PersonelLojmani p)
        {
            Sonuc sonuc = new Sonuc();

            //sonuc = IKYYetki.YetkiVarMiYazmayaTC(kullanan, kullanan.TCKIMLIKNO, sonuc, ENUMYetkiIslem.Lojman);
            sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Lojman);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }
            string hata = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonelLojman tab = new TABPersonelLojman(ve);
            IDataReader r = tab.Listele(new PersonelLojmani { kod = p.kod });
            if (r.Read())
                p.tashisTip = (ENUMTahsisTipi)VeriErisim.DegerAl(r, (int)TABPersonelLojman.ENUMAlanlar.TahsisTip, 0);

            r.Close();

            if (p.kod <= 0)
                hata = "Tahsis edeceğiniz personel lojmanı seçmelisiniz.";

            if (p.tashisTip != ENUMTahsisTipi.Rezervasyon)
                hata = "Tahsis edeceğiniz lojmanının rezervasyonlu olması gerekmektedir.";

            if (!string.IsNullOrWhiteSpace(hata))
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            p.tashisTip = ENUMTahsisTipi.Tahsis;

            sonuc = KayitKontrol(ve, kullanan, p);

            if (!sonuc.islemSonuc)
            {
                sonuc.DegerAta(ve.hata);
                sonuc.islemSonuc = false;
                return sonuc;
            }
            sonuc = tab.RezervdenTahsisEt(p);

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
    }
}

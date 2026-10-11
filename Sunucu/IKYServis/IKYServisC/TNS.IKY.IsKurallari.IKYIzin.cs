using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using System.Collections.Generic;

namespace TNS.IKY.IsKurallari
{
    class IKYIzin
    {
        public static IKYMServis servisKYM = null;

        public ObjectArray eskiTmp; //Tarihçe için

        public static bool YazmaYetkisiVarMi(Kullanici kullanan, PersonelIzin form)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.PERSONEL))
                return true;
            return false;
        }

        public Sonuc YetkiKontrol(Kullanici kullanan, PersonelIzin p, ENUMTARIHCEISLEMTIP islemTipi)
        {
            Sonuc sonuc = new Sonuc();
            int iIslemTipi = (int)islemTipi;
            if (iIslemTipi < 1 || iIslemTipi > 5)
            {
                sonuc.DegerAta("İşlem tipini belirtiniz.");
                return sonuc;
            }

            bool izinOnayYetkili = false, personelYetkili = false, izinYetkili = false;

            Personel per = IKYYetki.PersonelBul(kullanan, p.personelKod, null);

            IKYYetki.YetkiVarMiOGY(kullanan, per.kopruKodTam, 0, sonuc, MUH.ENUMOGY.Onaylama, ENUMYetkiIslem.Izin, true, per);
            //IKYYetki.YetkiVarMiOnayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Izin);
            if (sonuc.islemSonuc)
            {
                izinOnayYetkili = true; //1. yetkili
                return sonuc;
                // izin onay yetkilisi ise her türlü işlemi yapar. True döner.  
                // izin onay yetkilisi ise geçmişe ait de bilgi kaydı girme,güncelleme, silme ve yazdırma yapabilsin. 
                // Bu yüzden hiçbir kontrole girmeyecek.
            }
            //else
            //{
            //    sonuc.hataStr = "";
            //    sonuc.islemSonuc = true;
            //    IKYYetki.YetkiVarMiOGY(kullanan, per.kopruKod, sonuc, MUH.ENUMOGY.Yazma, ENUMYetkiIslem.PersonelIslemleri, true, per);
            //    //IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.PersonelIslemleri);
            //    if (sonuc.islemSonuc)
            //        personelYetkili = true; // 2. yetkili. 
            //    else
            //    {
            //        sonuc.hataStr = "";
            //        sonuc.islemSonuc = true;
            //        IKYYetki.YetkiVarMiOGY(kullanan, per.kopruKod, sonuc, MUH.ENUMOGY.Yazma, ENUMYetkiIslem.Izin, true, per);
            //        //IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Izin);
            //        if (sonuc.islemSonuc)
            //            izinYetkili = true; // 3. yetkili
            //        else
            //            return sonuc;
            //    }
            //}
            else
            {
                sonuc.hataStr = "";
                sonuc.islemSonuc = true;
                IKYYetki.YetkiVarMiOGY(kullanan, per.kopruKodTam, 0, sonuc, MUH.ENUMOGY.Yazma, ENUMYetkiIslem.Izin, true, per);

                if (sonuc.islemSonuc)
                {
                    string yetkiliBirimKopruKod = kullanan.yetkiTipiDegerleri["KopruKod", 0];
                    if (yetkiliBirimKopruKod.Length == 3)
                        personelYetkili = true; // 2. yetkili.
                    else
                        izinYetkili = true; // 3. yetkili
                }
                else
                    return sonuc;    //İzin ile ilgili hiçbir yetkisi yoksa direk en son kontrolden false döner.
            }

            //İşlem tipi: yeni kayıt ise; izin yetkilisi,personel yetkilisi,personel onay yetkilisi, sistem yöneticisi ise kayıt yapar.
            //İzin yetkilerinden birine sahipse her türlü yeni kayıt yapar. True döner.
            if (islemTipi == ENUMTARIHCEISLEMTIP.KAYDET)
                return sonuc;

            //Güncelleme, silme ve yazdırma için aşağıdaki kontroller yapılacak.

            PersonelIzin izin = null;
            //Mevcut iznin bilgilerini alıp bundan kontrolleri yapıyoruz.
            if (p.kod > 0)
            {
                ObjectArray oa = ListeleIzin(kullanan, new PersonelIzin { kod = p.kod, personelKod = p.personelKod });
                if (oa.ObjeSayisi == 1)
                    izin = (PersonelIzin)oa.objeler[0];
            }

            if (izin == null)// && (islemTipi == ENUMTARIHCEISLEMTIP.YAZDIR || islemTipi == ENUMTARIHCEISLEMTIP.GUNCELLE))
            {
                sonuc.DegerAta("Yazdırılacak veya güncellenecek kayıt bulunamadı.");
                return sonuc;
            }

            //İzin onay ve personel yetkilisi ise her türlü yazdırır. İzin yetkilisi izin bitiş tarihine kadar yapabilir.
            if (islemTipi == ENUMTARIHCEISLEMTIP.YAZDIR)
            {
                if (izinYetkili && izin.bitisTarihi.Oku().Date < System.DateTime.Now.Date)
                    sonuc.DegerAta("Bitiş tarihi şu anki tarihden küçük olduğu için işlem yapmaya yetkiniz yoktur.");
            }
            else
            {
                if (islemTipi == ENUMTARIHCEISLEMTIP.GUNCELLE)
                {
                    //İzin onay yetkilisi dışında başlangıç tarihini değiştiremezsiniz.!izinOnayYetkili 
                    if (p.baslangicTarihi.Oku().Date != izin.baslangicTarihi.Oku().Date)
                        sonuc.DegerAta("İzin başlangıç tarihini değiştiremezsiniz.");
                }
                if (personelYetkili && izin.bitisTarihi.Oku().Date < System.DateTime.Now.Date)
                    sonuc.DegerAta("Bitiş tarihi şu anki tarihden küçük olduğu için işlem yapmaya yetkiniz yoktur.");
                if (izinYetkili && izin.baslangicTarihi.Oku().Date < System.DateTime.Now.Date)
                    sonuc.DegerAta("Başlangıç tarihi şu anki tarihden küçük olduğu için işlem yapmaya yetkiniz yoktur.");
            }
            return sonuc;
        }

        public ObjectArray ListeleIzin(Kullanici kullanan, PersonelIzin p)
        {
            ObjectArray pVAN = new ObjectArray();
            pVAN.sonuc = IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, p.personelKod, null, ENUMYetkiIslem.Izin);
            if (!pVAN.sonuc.islemSonuc)
                return pVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelIzin tab = new TABPersonelIzin(ve);

            IDataReader r = tab.ListeleIzin(p);

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = ve.hata;

                return pVAN;
            }

            int sonKolon = (Enum.GetNames(typeof(TABPersonelIzin.ENUMAlanlar))).Length;

            while (r.Read())
            {
                PersonelIzin pi = new PersonelIzin();
                pi.personelKod = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.PKOD, 0);
                pi.kod = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.KOD, 0);
                pi.izinTurKod = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.IZINTUR, 0);
                pi.aciklama = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.ACIKLAMA, "");
                pi.baslangicTarihi = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.BASLANGICTARIHI);
                pi.bitisTarihi = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.BITISTARIHI);
                pi.telefon = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.TELEFON, "");
                pi.kullanilanGun = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.KULLANILANGUN, 0);
                pi.izinYeri = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.IZINNOTU, "");
                pi.onay = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.ONAY, 0);
                pi.evrEvrakNo = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.EVREVRAKNO, string.Empty);
                pi.belgeNo = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.BelgeNo, string.Empty);
                pi.belgeTarihi = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.BelgeTarihi);
                pi.imzalayan = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.ImzalayanPersonel, 0);
                pi.teklifEden = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.TeklifEdenPersonel, 0);
                pi.yerineGorevli = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.YerineGorevliPersonel, 0);
                pi.kayitTarihi = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.KayitTarihi);
                pi.memuriyeti = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.Memuriyeti, string.Empty);
                pi.saglikKurulusu = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.SaglikKurulusu, string.Empty);

                pi.raporTarihi = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.RAPORTARIHI);
                pi.raporNo = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.RAPORNO, string.Empty);
                pi.yatiliGecenSure = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.YATILIGECENSURE, 0);
                pi.yerKod = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.YERKOD, "");
                pi.izinNotu2 = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.IZINNOTU2, "");

                pi.imzalayanAd = VeriErisim.DegerAl(r, sonKolon, "");
                pi.teklifEdenAd = VeriErisim.DegerAl(r, sonKolon + 1, "");
                pi.yerineGorevliAd = VeriErisim.DegerAl(r, sonKolon + 2, "");
                pi.personelAd = VeriErisim.DegerAl(r, sonKolon + 3, "");
                pi.personelBirim = VeriErisim.DegerAl(r, sonKolon + 4, "");
                pi.personelSicilNo = VeriErisim.DegerAl(r, sonKolon + 5, "");
                pi.teklifEdenUnvan = VeriErisim.DegerAl(r, sonKolon + 6, "");
                pi.imzalayanUnvan = VeriErisim.DegerAl(r, sonKolon + 7, "");
                pi.izinTipAd = VeriErisim.DegerAl(r, sonKolon + 8, "");
                pi.yerAd = VeriErisim.DegerAl(r, sonKolon + 9, "");

                pVAN.objeler.Add(pi);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            else
            {
                //Tarihçe Kaydet ********************
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, new PersonelIzin(), p, ENUMTARIHCE.IZINHAREKET, p.personelKod, ENUMTARIHCEISLEMTIP.LISTELE);
            }
            ve.VeriErisimKapat();
            return pVAN;
        }

        public Sonuc KaydetIzin(Kullanici kullanan, PersonelIzin pIzin, bool OnayIslemi)
        {
            Sonuc sonuc = new Sonuc();
            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
            object eski = null, yeni = null; //Tarihçe için

            if (pIzin.kod > 0)
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;

            //IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Izin);
            sonuc = YetkiKontrol(kullanan, pIzin, islemTip);
            if (!sonuc.islemSonuc)
                return sonuc;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelIzin tab = new TABPersonelIzin(ve);

            string hata = "";
            if (pIzin.personelKod <= 0)
                hata = "Personel bilgisi boş bırakılamaz<br>";
            if (pIzin.baslangicTarihi.isNull)
                hata += "Personel izin başlama tarihi boş bırakılamaz<br>";
            else if (pIzin.bitisTarihi.isNull)
                hata += "Personel izin bitiş tarihi boş bırakılamaz<br>";
            else if (pIzin.baslangicTarihi.Oku() >= pIzin.bitisTarihi.Oku())
                hata += "İzin başlangıç tarihi bitiş tarihinden geç olamaz<br>";
            else
            {
                if (tab.KaydetIzinUygunMu(pIzin) > 0)
                    hata += "Belirtilen tarih aralığında başka bir izinle çakışıyor<br>";
            }

            pIzin.kayitTarihi = new TNSDateTime(System.DateTime.Now);
            pIzin.kullanilanGun = TNS.IKY.IzinArac.IzinHesapla(pIzin.izinTurKod, pIzin.baslangicTarihi.Oku(), pIzin.bitisTarihi.Oku());

            if (pIzin.izinTurKod == 0)
                hata += "Personel izin türü boş bırakılamaz<br>";

            if (hata != "")
            {
                sonuc.DegerAta(hata);
                ve.VeriErisimKapat();
                return sonuc;
            }

            //Tarihçe ********************
            if (pIzin.kod > 0)
            {
                this.eskiTmp = this.ListeleIzin(kullanan, new PersonelIzin() { personelKod = pIzin.personelKod, kod = pIzin.kod });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelIzin();// p.kod = 0: "Yeni Kayıt"
            //****************************

            ve.BeginTransaction();
            if (!tab.IzinKayitKontrol(pIzin))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Seçilen izin türü için izin hakkınız bulunmamaktadır.";
                ve.Rollback();
            }
            else
            {
                ObjectArray arr = IKYPersonelKadro.HizmetleriGetir(ve, kullanan, "", pIzin.personelKod.ToString(), "", new TNSDateTime(), 0, "", -1);
                if (arr.sonuc.islemSonuc)
                {
                    if (arr.objeler.Count > 0)
                    {
                        Hizmet h = (Hizmet)arr.objeler[0];
                        pIzin.memuriyeti = h.guncelHizmet.unvan;
                    }
                }

                if (pIzin.kod == 0)
                {
                    pIzin.kod = TABGenel.SayacDegerAlInt(ve, TABPersonelIzin.TABLOAD);
                    sonuc = tab.KaydetIzin(pIzin);
                }
                else
                {
                    sonuc = tab.GuncelleIzin(pIzin);
                }

                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = pIzin.kod.ToString();
                    ve.Commit();
                    sonuc.bilgiStr = PersonelResI.PER007;

                    //Tarihçe Kaydet ********************
                    yeni = pIzin;
                    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.IZINHAREKET, pIzin.personelKod, islemTip);
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc SilIzin(Kullanici kullanan, PersonelIzin p)
        {
            Sonuc sonuc = new Sonuc();
            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.SIL;
            object eski = null, yeni = null; //Tarihçe için

            //IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.Izin);
            sonuc = YetkiKontrol(kullanan, p, islemTip);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            string hata = "";
            if (p.personelKod <= 0)
                hata = "Personel seçilmemiş.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.kod > 0)
            {
                this.eskiTmp = this.ListeleIzin(kullanan, new PersonelIzin() { personelKod = p.personelKod, kod = p.kod });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            //if (eski == null)
            //    eski = new PersonelIzin();
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABBelgeler tabBelge = new TABBelgeler(ve);
            sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod = p.personelKod, tip = (int)ENUMBELGEFORMTIP.PERSONELIZIN });

            if (sonuc.islemSonuc)
            {
                TABPersonelIzin tab = new TABPersonelIzin(ve);
                sonuc = tab.SilIzin(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                //Tarihçe Kaydet ********************
                yeni = new PersonelIzin();
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.IZINHAREKET, p.personelKod, islemTip);
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc GuncelleIzinImza(Kullanici kullanan, PersonelIzin p)
        {
            Sonuc sonuc = new Sonuc();

            //sonuc = IKYYetki.YetkiVarMiYazmayaTC(kullanan, kullanan.mernis, sonuc, ENUMYetkiIslem.IzinDevir);
            ////sonuc = IKYYetki.YetkiVarMiYazmaya(kullanan, null, sonuc, ENUMYetkiIslem.IzinDevir);
            //if (!sonuc.islemSonuc)
            //{
            //    return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABPersonelIzin tab = new TABPersonelIzin(ve);

            sonuc = tab.GuncelleIzinImza(p);

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

        public ObjectArray ListeleIzinDevir(VeriErisim veriErisim, Kullanici kullanan, PersonelIzinDevir pid, int organizasyonKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray pVAN = new ObjectArray();

            TNS.IKY.Personel p = new TNS.IKY.Personel();
            p.kod = pid.personelKod;
            p.genelSorgu = pid.arama;
            p.personelTipleri = pid.personelTipleri;
            p.kadro.organizasyonKodu = organizasyonKodu;
            ObjectArray pArr = TNS.IKY.IsKurallari.IKYPersonel.Listele(ve, kullanan, p, true, false);
            if (pArr.sonuc.islemSonuc)
            {
                if (pid.personelKod > 0)
                {
                    p = (TNS.IKY.Personel)pArr.objeler[0];
                    pVAN.sonuc = IKYYetki.YetkiVarMiOGY(kullanan, null, 0, pVAN.sonuc, MUH.ENUMOGY.Gorme, ENUMYetkiIslem.IzinDevir, true, p);
                }
                else if (organizasyonKodu > 0)
                {
                    string kopruKod = null;
                    TNS.ORG.OrganizasyonBirim birim = ORG.Arac.OrgBirimBul(organizasyonKodu);
                    if (birim != null)
                    {
                        kopruKod = birim.kopruKod;
                        pid.birimAd = birim.adi;
                    }

                    pVAN.sonuc = IKYYetki.YetkiVarMiOGY(kullanan, kopruKod, 0, pVAN.sonuc, MUH.ENUMOGY.Gorme, ENUMYetkiIslem.IzinDevir, false, null);
                }
            }
            else
                pVAN.sonuc = pArr.sonuc;

            if (!pVAN.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return pVAN;
            }
            TABPersonelIzin tab = new TABPersonelIzin(ve);

            foreach (TNS.IKY.Personel item in pArr.objeler)
            {
                pid.personelKod = item.kod;
                IDataReader r = tab.ListeleIzinDevir(pid);

                if (r == null)
                {
                    ve.VeriErisimKapat();
                    pVAN.sonuc.DegerAta(ve.hata);
                    return pVAN;
                }
                bool eklendi = false;
                while (r.Read())
                {
                    PersonelIzinDevir pi = new PersonelIzinDevir();
                    pi.kod = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlarIzinDevir.KOD, 0);
                    pi.personelKod = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlarIzinDevir.PKOD, 0);
                    pi.yil = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlarIzinDevir.YIL, 0);
                    pi.devreden = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlarIzinDevir.DEVREDEN, 0);
                    pi.yilici = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlarIzinDevir.YILICI, 0);

                    pi.kullanilan = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlarIzinDevir.YILICI + 1, 0);
                    pi.kidemGun = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlarIzinDevir.YILICI + 2, 0);

                    pi.sicilNo = item.sicilNo;
                    pi.tcKimlikNo = item.tcKimlikNo;
                    pi.adSoyad = item.adSoyad;
                    pVAN.objeler.Add(pi);
                    eklendi = true;
                }
                if (!eklendi && pid.yil > 0)
                {
                    PersonelIzinDevir pi = new PersonelIzinDevir();
                    pi.kod = 0;
                    pi.personelKod = item.kod;
                    pi.yil = pid.yil;
                    pi.devreden = 0;
                    pi.yilici = 0;
                    pi.sicilNo = item.sicilNo;
                    pi.tcKimlikNo = item.tcKimlikNo;
                    pi.adSoyad = item.adSoyad;
                    pVAN.objeler.Add(pi);
                }
                r.Close();
            }

            if (pVAN.objeler.Count > 0)
            {
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, new PersonelIzinDevir(), pid, ENUMTARIHCE.IZINDEVIR, pid.personelKod, ENUMTARIHCEISLEMTIP.LISTELE);
            }
            ve.VeriErisimKapat();
            return pVAN;
        }

        public Sonuc KaydetIzinDevir(Kullanici kullanan, PersonelIzinDevir[] pList, bool onceSil)
        {
            Sonuc sonuc = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            foreach (PersonelIzinDevir p in pList)
            {
                if (p.personelKod <= 0)
                    sonuc.DegerAta("Personel bilgisi boş bırakılamaz.<br>");
                if (p.yil == 0)
                    sonuc.DegerAta("Personel yıl bilgisi boş bırakılamaz.<br>");

                if (sonuc.islemSonuc)
                {

                    TNS.IKY.Personel per = new TNS.IKY.Personel();
                    per.kod = p.personelKod;
                    ObjectArray pArr = TNS.IKY.IsKurallari.IKYPersonel.Listele(ve, kullanan, per, true, false);
                    if (!pArr.sonuc.islemSonuc)
                        sonuc.DegerAta(pArr.sonuc.hataStr + "<br>");
                    else if (pArr.ObjeSayisi == 0)
                        sonuc.DegerAta(string.Format("{0} kodlu personel bulunamadı.", p.personelKod) + "<br>");
                    else
                    {
                        per = (TNS.IKY.Personel)pArr.objeler[0];
                        sonuc = IKYYetki.YetkiVarMiOGY(kullanan, null, 0, sonuc, MUH.ENUMOGY.Yazma, ENUMYetkiIslem.IzinDevir, true, per);
                    }
                }
                if (sonuc.islemSonuc) break;
            }

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();
            TABPersonelIzin tab = new TABPersonelIzin(ve);

            foreach (PersonelIzinDevir p in pList)
            {
                if (p.kod > 0)
                    sonuc = tab.SilIzinDevir(p);
                else
                {
                    if (onceSil) sonuc = tab.SilIzinDevir(p);
                    if (sonuc.islemSonuc)
                        p.kod = TABGenel.SayacDegerAlInt(ve, TABPersonelIzin.TABLOADIZINDEVIR);
                }
                if (sonuc.islemSonuc)
                    sonuc = tab.KaydetIzinDevir(p);
                else
                    break;
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc DevirleriAktar(Kullanici kullanan, PersonelIzinDevir pid, int organizasyonKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            pid.yil--;
            ObjectArray pidArr = ListeleIzinDevir(ve, kullanan, pid, organizasyonKodu);
            pid.yil++;

            Sonuc sonuc = pidArr.sonuc;
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            foreach (PersonelIzinDevir dev in pidArr.objeler)
            {
                dev.kod = 0;
                dev.yil++;
                dev.devreden = dev.devreden + dev.yilici - dev.kullanilan;
                dev.yilici = TNS.IKY.IzinArac.YilIciIzinHakki(Hizmet.YilDondur(dev.kidemGun));
                if (dev.devreden < 0) dev.devreden = 0;
                else if (dev.devreden > dev.yilici) dev.devreden = dev.yilici;
            }
            sonuc = KaydetIzinDevir(kullanan, (PersonelIzinDevir[])pidArr.objeler.ToArray(typeof(PersonelIzinDevir)), true);
            return sonuc;
        }

        public Sonuc SilIzinDevir(Kullanici kullanan, PersonelIzinDevir p)
        {
            Sonuc sonuc = new Sonuc();

            sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, null, ENUMYetkiIslem.IzinDevir);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = "";

            if (p.kod <= 0)
            {
                if (p.personelKod <= 0)
                    hata = "Personel bilgisi boş bırakılamaz.<br>";
                if (p.yil == 0)
                    hata += "Personel yıl bilgisi boş bırakılamaz.<br>";
            }

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonelIzin tab = new TABPersonelIzin(ve);

            sonuc = tab.SilIzinDevir(p);

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

        public ObjectArray ListeleIzinDurum(Kullanici kullanan, PersonelIzinDurum pk)
        {
            ObjectArray pVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelIzin tab = new TABPersonelIzin(ve);
            DateTime donemBasDT = TNS.IKY.IzinArac.DonemBasiBul(pk.listelemeYili);
            DateTime donemSonDT = TNS.IKY.IzinArac.DonemSonuBul(pk.listelemeYili);
            TNSDateTime donemBas = new TNSDateTime(donemBasDT);
            TNSDateTime donemSon = new TNSDateTime(donemSonDT);
            IDataReader r = tab.ListeleIzinDurum(pk, donemBas, donemSon);

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            PersonelIzinDurum pp = null;
            while (r.Read())
            {
                int pp_tip = VeriErisim.DegerAl(r, 0, 0);
                if (pp == null || pp.tip != pp_tip)
                {
                    pp = new PersonelIzinDurum();
                    pp.tip = pp_tip;
                    pp.ad = VeriErisim.DegerAl(r, 1, "");
                    pp.toplamGun = VeriErisim.DegerAl(r, 2, 0);
                    pVAN.objeler.Add(pp);
                }
                TNSDateTime dt1 = VeriErisim.DegerAl(r, 3);
                TNSDateTime dt2 = VeriErisim.DegerAl(r, 4);
                DateTime kesTar1;
                DateTime kesTar2;
                int gun = TNS.ResmiTatil.KesisimBul(donemBasDT, donemSonDT, dt1.Oku(), dt2.Oku(), false, out kesTar1, out kesTar2);
                //   gun = TNS.IKY.IzinArac.IzinHesapla(pp_tip, kesTar1, kesTar2);
                pp.kullanilanGun += gun;
            }
            r.Close();
            ve.VeriErisimKapat();
            return pVAN;
        }

        public IzinBilgi KullanilanIzinHesapla(VeriErisim veriErisim, Kullanici kullanan, int personelKod, int yil, int haricKod)
        {
            IzinBilgi iBilgi = new IzinBilgi();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelIzin tab = new TABPersonelIzin(ve);

            DateTime donemBasDT = TNS.IKY.IzinArac.DonemBasiBul(yil);
            DateTime donemSonDT = TNS.IKY.IzinArac.DonemSonuBul(yil);
            TNSDateTime donemBas = new TNSDateTime(donemBasDT);
            TNSDateTime donemSon = new TNSDateTime(donemSonDT);
            IDataReader dr = tab.ListeleYillikIzin(personelKod, yil, haricKod, donemBas, donemSon);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TNSDateTime dt1 = VeriErisim.DegerAl(dr, 0);
                    TNSDateTime dt2 = VeriErisim.DegerAl(dr, 1);
                    DateTime kesTar1;
                    DateTime kesTar2;
                    int gun = TNS.ResmiTatil.KesisimBul(donemBasDT, donemSonDT, dt1.Oku(), dt2.Oku(), false, out kesTar1, out kesTar2);
                    //if (yontem == IzinArac.IzinYontem.TCIsci)
                    //{
                    //    gun = TNS.IKY.IzinArac.IzinHesapla(1, kesTar1, kesTar2, yontem);
                    //}
                    iBilgi.yilIzni += gun;
                }
                dr.Close();
            }

            PersonelIzinDevir pid = new PersonelIzinDevir();
            pid.personelKod = personelKod;
            pid.yil = yil;
            ObjectArray pler = ListeleIzinDevir(ve, kullanan, pid, 0);

            if (pler.sonuc.islemSonuc && pler.objeler.Count > 0)
            {
                pid = (TNS.IKY.PersonelIzinDevir)pler[0];
                iBilgi.devreden = pid.devreden;
                iBilgi.yilIci = pid.yilici;
            }
            ve.VeriErisimKapat();
            return iBilgi;
        }

        public int KendindenKucukKullanilanIzinHesapla(int personelKod, TNSDateTime baslamaTarihi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            TABPersonelIzin tab = new TABPersonelIzin(ve);

            int izin = tab.ListeleKullanilanIzinKendindenKucuk(personelKod, baslamaTarihi);
            ve.VeriErisimKapat();

            return izin;
        }

        public PersonelIzinGunleri PersonelYilBazliYillikIzinBilgileri(Kullanici kullanan, int personelKod, DateTime tarih)
        {
            PersonelIzinGunleri gunler = new PersonelIzinGunleri();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelIzin tab = new TABPersonelIzin(ve);

            IDataReader r = tab.PersonelYilBazliYillikIzinBilgileri(personelKod, tarih);

            if (r == null)
            {
                gunler.sonuc = new Sonuc();
                gunler.sonuc.DegerAta(ve.hata);
            }
            if (r.Read())
            {
                gunler.yilIci = IzinArac.YilIciIzinHakki(Hizmet.YilDondur(VeriErisim.DegerAl(r, 0, 0)));
                gunler.devir = VeriErisim.DegerAl(r, 1, 0);
                gunler.kullanilan = VeriErisim.DegerAl(r, 2, 0);
                gunler.kalan = gunler.yilIci + gunler.devir - gunler.kullanilan;
            }

            return gunler;
        }

        internal static PersonelIzin[] ListeleIzin(string okunanXML)
        {
            System.Collections.Generic.List<PersonelIzin> pVAN = new System.Collections.Generic.List<PersonelIzin>();
            XMLStringDataReader r = new XMLStringDataReader(okunanXML);

            if (r != null)
            {
                while (r.Read())
                {
                    PersonelIzin pi = new PersonelIzin();
                    pi.personelKod = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.PKOD, 0);
                    pi.kod = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.KOD, 0);
                    pi.izinTurKod = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.IZINTUR, 0);
                    pi.aciklama = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.ACIKLAMA, "");
                    pi.baslangicTarihi = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.BASLANGICTARIHI);
                    pi.bitisTarihi = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.BITISTARIHI);
                    pi.telefon = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.TELEFON, "");
                    pi.kullanilanGun = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.KULLANILANGUN, 0);
                    pi.izinYeri = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.IZINNOTU, "");
                    pi.onay = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.ONAY, 0);
                    pi.evrEvrakNo = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.EVREVRAKNO, string.Empty);
                    pi.belgeNo = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.BelgeNo, string.Empty);
                    pi.belgeTarihi = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.BelgeTarihi);
                    pi.imzalayan = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.ImzalayanPersonel, 0);
                    pi.teklifEden = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.TeklifEdenPersonel, 0);
                    pi.yerineGorevli = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.YerineGorevliPersonel, 0);
                    pi.kayitTarihi = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.KayitTarihi);
                    pi.memuriyeti = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.Memuriyeti, string.Empty);
                    pi.saglikKurulusu = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.SaglikKurulusu, string.Empty);

                    pi.raporNo = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.RAPORNO, string.Empty);
                    pi.raporTarihi = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.RAPORTARIHI);
                    pi.yatiliGecenSure = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.YATILIGECENSURE, 0);
                    pi.yerKod = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.YERKOD, "");
                    pi.ulkeAd = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.YERKOD + 1, "");
                    pi.yerAd = VeriErisim.DegerAl(r, (int)TABPersonelIzin.ENUMAlanlar.YERKOD + 2, "");
                    pVAN.Add(pi);
                }
                r.Close();
            }
            return pVAN.ToArray();
        }
    }
}

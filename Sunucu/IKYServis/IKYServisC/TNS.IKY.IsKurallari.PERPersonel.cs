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
    class IKYPersonel
    {
        public static IKYMServis servisKYM = null;
        public ObjectArray eskiTmp; //Tarihçe için

        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, int pKod, bool ozet, bool hizmetleriOku)
        {
            ObjectVAN ov = new ObjectVAN();
            if (pKod <= 0)
                ov.sonuc.DegerAta("Bu işlem için personel kodu verilmelidir!");
            else
            {
                ObjectArray oa = Listele(veriErisim, kullanan, new Personel { kod = pKod }, ozet, hizmetleriOku);
                ov.sonuc = oa.sonuc;
                if (oa.ObjeSayisi == 1)
                    ov.o = oa.objeler[0];
            }
            return ov;
        }

        public static void KopruKodParcala(Personel p, out string kopruKod, out string kopruKodlar)
        {
            kopruKod = null;
            kopruKodlar = "";
            if (p.kadro.organizasyonKodu > 0)
            {
                TNS.ORG.OrganizasyonBirim birim = ORG.Arac.OrgBirimBul(p.kadro.organizasyonKodu);
                if (birim != null)
                    kopruKod = birim.kopruKod;
            }
            else if (!string.IsNullOrWhiteSpace(p.kopruKod))
                kopruKod = p.kopruKod;
            else if (!string.IsNullOrWhiteSpace(p.genelSorgu))
            {
                TNS.ORG.IORGServis orgServis = ORG.Arac.Tanimla();
                TNS.ORG.OrganizasyonBirim birim = new ORG.OrganizasyonBirim();
                birim.adlarSorgu = p.genelSorgu.Split(';')[0];
                ObjectArray birimler = orgServis.OrganizasyonBirimListele(birim, false, false);
                foreach (TNS.ORG.OrganizasyonBirim icBirim in birimler.objeler)
                {
                    if (!string.IsNullOrWhiteSpace(icBirim.kopruKod))
                    {
                        if (kopruKodlar != "") kopruKodlar += ",";
                        kopruKodlar += icBirim.kopruKod;
                    }
                }
            }
        }

        public static ObjectArray IstatistikListele(VeriErisim veriErisim, Kullanici kullanan, Personel p, int istKod)
        {
            ObjectArray pVAN = new ObjectArray();
            string kopruKod = null;
            string kopruKodlar = "";
            KopruKodParcala(p, out kopruKod, out kopruKodlar);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.hata = "";
            TABPersonel tab = new TABPersonel(ve);
            int alanSay = 0;
            IDataReader r = null;
            if (istKod == 1)
            {
                alanSay = 3;
                r = tab.IstatistikListeleUnvanTipGrupTip(p, kopruKod, kopruKodlar);
            }
            else if (istKod == 2)
            {
                alanSay = 2;
                r = tab.IstatistikListeleHarcBirimPTip(p, kopruKod, kopruKodlar);
            }
            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }
            pVAN.sonuc.kayitSay = r.RecordsAffected;

            while (r.Read())
            {
                IkiBoyutluTabloVeri ibv = new IkiBoyutluTabloVeri();
                for (int i = 0; i < alanSay; i++)
                {
                    StrKodStrAd sIc = new StrKodStrAd(VeriErisim.DegerAl(r, i * 2 + 0, ""), VeriErisim.DegerAl(r, i * 2 + 1, ""));
                    ibv.boyutlar.Add(sIc);
                }
                double deger = VeriErisim.DegerAl(r, alanSay * 2, 0.0);
                ibv.veri = deger;
                ibv.veriTip = VariableType.Money;
                pVAN.Ekle(ibv);
            }
            r.Close();

            ve.VeriErisimKapat();
            return pVAN;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Personel p, bool ozet, bool hizmetleriOku, TNSDateTime gorevBitisBasla = null, TNSDateTime gorevBitisSon = null, TNSDateTime zorunluEmeklilikTarihi = null)
        {
            ObjectArray pVAN = new ObjectArray();
            string kopruKod = null;
            string kopruKodlar = "";
            KopruKodParcala(p, out kopruKod, out kopruKodlar);
            if (!p.bordroDisiDahil) ozet = false;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.hata = "";
            TABPersonel tab = new TABPersonel(ve);
            if (hizmetleriOku)
                p.hizmetSuresiOlsun = true;
            ve.sureAsimi = 60 * 10;
            IDataReader r = tab.Listele(p, kopruKod, kopruKodlar, gorevBitisBasla, gorevBitisSon, zorunluEmeklilikTarihi);
            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }
            pVAN.sonuc.kayitSay = r.RecordsAffected;
            int nufusAlanlarIlkIndex = (Enum.GetNames(typeof(TABPersonel.ENUMAlanlar))).Length;
            int kadroAlanlarIlkIndex = nufusAlanlarIlkIndex + (Enum.GetNames(typeof(TABPersonel.ENUMAlanlarNUFUS))).Length;
            int iletisimAlanlarIlkIndex = kadroAlanlarIlkIndex + (Enum.GetNames(typeof(TABPersonelKadro.ENUMALANLAR))).Length;
            int toplamIndex = iletisimAlanlarIlkIndex + (Enum.GetNames(typeof(TABPersonel.ENUMAlanlarIletisim))).Length;
            int oncekiKod = -1;
            int eklenmeyen = 0;
            int toplamKayitSay = 0;
            if (p.kayitToplam > 0)
            {
                while (true)
                {
                    if (toplamKayitSay == p.kayitBasla) break;
                    if (r.Read()) toplamKayitSay++;
                    else break;
                }
            }
            if (toplamKayitSay == p.kayitBasla || p.kayitToplam <= 0)
            {
                int toplamOkunan = 0;
                HRC.BordroGenelDegisken[] bgd = TNS.HRC.BordroGenelDegisken.bordroDegiskenler;
                while (r.Read())
                {
                    toplamKayitSay++;
                    toplamOkunan++;
                    if (toplamOkunan > p.kayitToplam && p.kayitToplam > 0) continue;
                    Personel pp = new Personel();
                    pp.kod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KOD, 0);
                    if (pp.kod == oncekiKod) continue;
                    oncekiKod = pp.kod;
                    pp.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.TCKIMLIKNO, "");
                    pp.ad = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.AD, "");
                    pp.soyad = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SOYAD, "");
                    pp.aktifMi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.AKTIFMI, 0);
                    pp.rumuz = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.RUMUZ, "");
                    pp.titr = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.TITR, "");
                    pp.sicilNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KURUMSICILNO, "");

                    pp.kadro.unvanAd = VeriErisim.DegerAl(r, toplamIndex + 0, "");
                    pp.kadro.personelTipAd = VeriErisim.DegerAl(r, toplamIndex + 1, "");
                    pp.nufus.medeniHaliAd = VeriErisim.DegerAl(r, toplamIndex + 2, "");
                    pp.nufus.ilAd = VeriErisim.DegerAl(r, toplamIndex + 3, "");
                    pp.nufus.ilceAd = VeriErisim.DegerAl(r, toplamIndex + 4, "");
                    pp.kadro.birimAd = VeriErisim.DegerAl(r, toplamIndex + 5, "");
                    pp.kopruKod = VeriErisim.DegerAl(r, toplamIndex + 6, "");
                    pp.kadro.unvanSira = VeriErisim.DegerAl(r, toplamIndex + 7, 9000);
                    pp.kadro.vekilUnvanAd = VeriErisim.DegerAl(r, toplamIndex + 8, "");
                    pp.kadro.gorevBirimAd = VeriErisim.DegerAl(r, toplamIndex + 9, "");
                    pp.sinifKodu = VeriErisim.DegerAl(r, toplamIndex + 10, 0);

                    pp.birimTuru = VeriErisim.DegerAl(r, toplamIndex + 11, "");
                    pp.calistigiBirimTuru = VeriErisim.DegerAl(r, toplamIndex + 12, "");
                    pp.kadro.muhasebeAd = VeriErisim.DegerAl(r, toplamIndex + 13, "");
                    pp.kadro.muhasebeBirimiAd = VeriErisim.DegerAl(r, toplamIndex + 14, "");

                    int toplamIndexEk = toplamIndex + 2;
                    object rb = r.GetValue(toplamIndexEk + 13);
                    if (rb != DBNull.Value)
                        pp.resim = (byte[])rb;
                    else
                        pp.resim = null;

                    pp.ogrenim = (PersonelOgrenim[])IKYOgrenim.Listele(VeriErisim.DegerAl(r, toplamIndexEk + 14, "")).objeler.ToArray(typeof(PersonelOgrenim));
                    pp.lojmanlar = IKYPersonelLojman.Listele(VeriErisim.DegerAl(r, toplamIndexEk + 15, ""));
                    pp.kadrolar = IKYPersonelKadro.ListeleKadro(VeriErisim.DegerAl(r, toplamIndexEk + 16, ""));
                    pp.personelMaasParam = PERBordroTurGiris.MaasParamListele(bgd, VeriErisim.DegerAl(r, toplamIndexEk + 17, ""));
                    PERBordroTurGiris.MaasMatrahListele(pp.personelMaasParam, VeriErisim.DegerAl(r, toplamIndexEk + 18, ""));
                    pp.kisiYakinlik = IKYAile.ListeleAile(VeriErisim.DegerAl(r, toplamIndexEk + 19, ""));
                    ObjectArray oaDil = IKYDil.Listele(VeriErisim.DegerAl(r, toplamIndexEk + 20, ""));
                    if (oaDil.sonuc.islemSonuc && oaDil.ObjeSayisi > 0)
                        pp.dil = (PersonelDil[])oaDil.objeler.ToArray(typeof(PersonelDil));
                    pp.hizmetSureleri = new Hizmet[2];
                    pp.hizmetSureleri[0] = IKYPersonelKadro.HizmetleriGetir(VeriErisim.DegerAl(r, toplamIndexEk + 21, ""));
                    pp.hizmetSureleri[1] = IKYPersonelKadro.HizmetleriGetir(VeriErisim.DegerAl(r, toplamIndexEk + 22, ""));

                    pp.izin = IKYIzin.ListeleIzin(VeriErisim.DegerAl(r, toplamIndexEk + 23, ""));
                    pp.gorevler = IKYGorevlendirmeler.Listele(VeriErisim.DegerAl(r, toplamIndexEk + 24, ""));
                    pp.kadro.gitOrganizasyonKopruKodu = VeriErisim.DegerAl(r, toplamIndexEk + 25, "");
                    pp.kadro.ayrilmaGerekcesiAd = VeriErisim.DegerAl(r, toplamIndexEk + 26, "");

                    pp.giderToplamlar = IKYPersonelGider.ListeleToplam(VeriErisim.DegerAl(r, toplamIndexEk + 27, ""));
                    pp.cezaOduller = IKYCezaOdul.Listele(VeriErisim.DegerAl(r, toplamIndexEk + 28, ""));
                    pp.sendikalar = IKYSendikaBilgi.Listele(VeriErisim.DegerAl(r, toplamIndexEk + 29, ""));

                    pp.satir = toplamKayitSay;
                    DatareaderdanIletisimAl(pp.iletisim, r, iletisimAlanlarIlkIndex, ozet);
                    DatareaderdanNufusAl(pp.nufus, r, nufusAlanlarIlkIndex, ozet);
                    IKYPersonelKadro.ReaderdanAl(pp.kadro, r, kadroAlanlarIlkIndex, ozet);

                    if (pp.kadrolar != null && pp.kadrolar.ObjeSayisi > 0)
                    {
                        pp.kadro.hizmetSebebiTip = ((PersonelKadro)pp.kadrolar[0]).hizmetSebebiTip;
                        pp.kadro.ayrilmaGerekcesiTip = ((PersonelKadro)pp.kadrolar[pp.kadrolar.ObjeSayisi - 1]).ayrilmaGerekcesiTip;
                    }

                    if (!ozet)
                    {
                        pp.sakatlikDurumu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SAKATLIKDURUMU, 0);
                        pp.emekliSicilNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.EMEKLISICILNO, "");
                        pp.sskSicilNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SSKSICILNO, "");
                        pp.bagkurSicilNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.BAGKURSICILNO, "");
                        pp.ilkIseBaslamaTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ILKISEBASLAMATARIHI);
                        pp.asaletTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ASALETTARIHI);
                        pp.kurumaBaslamaTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KURUMABASLAMATARIHI);
                        pp.aciklama = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ACIKLAMA, "");
                        pp.sakatlikDerecesi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SAKATLIKDERECESI, 0);
                        pp.terorMagduru = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.TERORMAGDURU, 0);
                        pp.hukumlu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.HUKUMLU, 0);
                        pp.korunmayaMuhtac = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KORUNMAYAMUHTAC, 0);
                        pp.sosyalGuvence = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SOSYALGUVENCE, 0);
                        pp.bankaIbanNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.BANKAIBANNO, "");
                        pp.vergiDairesi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.VERGIDAIRESI, "");
                        pp.geldigiKurum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.GELDIGIKURUM, "");
                        pp.geldigiKurumSicili = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.GELDIGIKURUMSICILI, "");
                        pp.arsivNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ARSIVNO, "");
                    }

                    bool ekle = true;
                    if (!p.bordroDisiDahil)
                    {
                        double d = pp.personelMaasParam.ParamDegerBul("bordroDisi");
                        if (d >= 1)
                            ekle = false;
                    }

                    if (ekle)
                    {
                        if (!ozet)
                        {
                            ENUMYetkiIslem tip = ENUMYetkiIslem.OzlukDetay;
                            if (p.hizliBilgi)
                                tip = ENUMYetkiIslem.HizliBilgi;
                            tip = p.bordroDisiDahil ? tip : ENUMYetkiIslem.MaasParametreleri;
                            if ((tip == ENUMYetkiIslem.HizliBilgi || tip == ENUMYetkiIslem.OzlukDetay) && pp.aktifMi == 0)
                                ekle = true;
                            else
                            {
                                Sonuc icS = IKYYetki.YetkiVarMiOkumaya(kullanan, pp.kopruKodTam, pp.personelTip, null, tip);
                                ekle = icS.islemSonuc;
                            }
                        }
                    }

                    if (ekle)
                        pVAN.objeler.Add(pp);
                    else
                        eklenmeyen++;
                }
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;
            pVAN.sonuc.kayitSay = toplamKayitSay;
            if (eklenmeyen > 0)
            {
                if (pVAN.objeler.Count == 0) pVAN.sonuc.DegerAta("Verilen kriterlere göre işlem yapmaya yetkiniz <b>bulunmamaktadır!</b>");
                else pVAN.sonuc.bilgiStr = "Verilen kriterlere görebileceğiniz kişiler listeye eklenmiş, görme yetkiniz olmayan kişiler ise eklenmemiştir!";
            }

            if (p.tarihce)
            {
                //Tarihçe Kaydet ********************      
                ENUMTARIHCE tarihce = ENUMTARIHCE.KISISEL;
                if (p.hizliBilgi)
                    tarihce = ENUMTARIHCE.HIZLIBILGI;
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, new Personel(), p, tarihce, p.kod, ENUMTARIHCEISLEMTIP.LISTELE);
            }
            ve.VeriErisimKapat();
            return pVAN;
        }

        internal static TNSCollection ListeleKod(VeriErisim ve, TNSCollection tcKimlikNo, TNSCollection tcSicilNo)
        {
            TNSCollection tc = new TNSCollection();
            TABPersonel t = new TABPersonel(ve);
            IDataReader r = t.ListeleKod(tcKimlikNo, tcSicilNo);
            if (r != null)
            {
                while (r.Read())
                {
                    Personel pp = new Personel();
                    pp.kod = VeriErisim.DegerAl(r, 0, 0);
                    pp.tcKimlikNo = VeriErisim.DegerAl(r, 1, "");
                    pp.sicilNo = VeriErisim.DegerAl(r, 2, "");
                    tc.Add(pp);
                }
                r.Close();
            }
            return tc;
        }

        public static ObjectArray PersonelLojmanliListele(VeriErisim veriErisim, Kullanici kullanan, Personel p)
        {
            ObjectArray pVAN = new ObjectArray();
            string kopruKod = null;
            if (p.kadro.organizasyonKodu > 0)
            {
                TNS.ORG.OrganizasyonBirim birim = ORG.Arac.OrgBirimBul(p.kadro.organizasyonKodu);
                if (birim != null)
                    kopruKod = birim.kopruKod;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.hata = "";
            TABPersonel tab = new TABPersonel(ve);
            IDataReader r = tab.PersonelLojmanliListele(p, kopruKod);
            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            while (r.Read())
            {
                IKYSiteYetki pp = new IKYSiteYetki();

                pp.pkod = VeriErisim.DegerAl(r, 0, 0);
                pp.oturduguPLKod = VeriErisim.DegerAl(r, 1, 0);
                pp.personelAd = VeriErisim.DegerAl(r, 2, "");
                pp.oturduguSite = VeriErisim.DegerAl(r, 3, "");
                pp.oturduguBlok = VeriErisim.DegerAl(r, 4, "");
                pp.oturduguDaire = VeriErisim.DegerAl(r, 5, 0);

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public static PersonelIletisim ListeleIletisim(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            PersonelIletisim iletisim = new PersonelIletisim();

            IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, kod, iletisim.sonuc, ENUMYetkiIslem.Iletisim);
            if (!iletisim.sonuc.islemSonuc)
            {
                return iletisim;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = tab.ListeleIletisim(kod);

            if (r == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            if (r.Read())
            {
                DatareaderdanIletisimAl(iletisim, r, 0, false);
            }
            r.Close();

            Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, new PersonelIletisim(), new PersonelIletisim { kod = kod }, ENUMTARIHCE.ADRES, kod, ENUMTARIHCEISLEMTIP.LISTELE);

            ve.VeriErisimKapat();
            return iletisim;
        }

        public static void DatareaderdanIletisimAl(PersonelIletisim iletisim, IDataReader r, int baslaKolon, bool ozet)
        {
            int sonIndex = (Enum.GetNames(typeof(TABPersonel.ENUMAlanlarIletisim))).Length;
            iletisim.kod = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.KOD, 0);
            iletisim.isTelefon = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.ISTELEFON, "");
            iletisim.faks = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.FAKS, "");
            iletisim.eposta = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.EPOSTA, "");
            iletisim.url = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.URL, "");
            iletisim.rehberdeGozuksun = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.REHBERDEGOZUKSUN, 0);
            iletisim.eposta2 = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.EPOSTA2, "");
            iletisim.personelAd = VeriErisim.DegerAl(r, baslaKolon + sonIndex, "");

            if (!ozet)
            {
                iletisim.cepTelefon = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.CEPTELEFON, "");
                iletisim.adres = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.ADRES, "");
                iletisim.evTelefon = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.EVTELEFON, "");
                iletisim.mobilImzaTelefon = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.MOBILIMZATELEFON, "");
                iletisim.cepTelefon2 = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.CEPTELEFON2, "");
                iletisim.tarabyaTel = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.TARABYATEL, "");
                iletisim.lojmanTel = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.LOJMANTEL, "");
                iletisim.telsiz = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.TELSIZ, "");
                iletisim.goruntulu = VeriErisim.DegerAl(r, baslaKolon + (int)TABPersonel.ENUMAlanlarIletisim.GORUNTULU, "");
            }
        }

        public PersonelNufus ListeleNufus(Kullanici kullanan, int kod)
        {
            PersonelNufus nufus = new PersonelNufus();

            //if (!IKY.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = tab.ListeleNufus(kod);
            if (r == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            int sonKolon = (Enum.GetNames(typeof(TABPersonel.ENUMAlanlarNUFUS))).Length;
            if (r.Read())
            {
                DatareaderdanNufusAl(nufus, r, 0, false);
                nufus.medeniHaliAd = VeriErisim.DegerAl(r, sonKolon, "");
                nufus.ilAd = VeriErisim.DegerAl(r, sonKolon + 1, "");
                nufus.ilceAd = VeriErisim.DegerAl(r, sonKolon + 2, "");
            }
            r.Close();
            ve.VeriErisimKapat();
            return nufus;
        }

        public static void DatareaderdanNufusAl(PersonelNufus nufus, IDataReader r, int nufusAlanlarIlkIndex, bool ozet)
        {
            nufus.kanGrubu = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.KANGRUBU, "");
            nufus.cinsiyeti = (ENUMCINSIYET)VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.CINSIYET, 0);
            nufus.dogumTarihi = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.DOGUMTARIHI);

            if (!ozet)
            {
                nufus.kod = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.KOD, 0);
                nufus.dogumYeri = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.DOGUMYERI, "");
                nufus.medeniHali = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.MEDENIHALI, 0);
                nufus.anaAd = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.ANAAD, "");
                nufus.babaAd = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.BABAAD, "");
                nufus.cuzdanSeriNo = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.CUZDANSERINO, "");
                nufus.cuzdanVerilisNedeni = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.CUZDANVERILISNEDENI, "");
                nufus.cuzdanVerilisTarihi = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.CUZDANVERILISTARIHI);
                nufus.cuzdanVerildigiYer = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.CUZDANVERILDIGIYER, "");
                nufus.cuzdanKayitNo = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.CUZDANKAYITNO, "");
                nufus.nufusIlKod = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.NUFUSILKOD, "");
                nufus.nufusIlceKod = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.NUFUSILCEKOD, "");
                nufus.nufusMahalleKoy = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.NUFUSMAHALLEKOY, "");
                nufus.nufusCiltNo = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.NUFUSCILTNO, "");
                nufus.nufusSiraNo = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.NUFUSSIRANO, "");
                nufus.nufusAileSiraNo = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.NUFUSAILESIRANO, "");
                nufus.ilkSoyad = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.ILKSOYAD, "");
                nufus.dini = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.NUFUSDINI, "");
                nufus.pasaportNo = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.PASAPORTNO, "");
                nufus.pasaportVerilisTarihi = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.PASAPORTVERILISTARIHI);
                nufus.pasaportVerildigiYer = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.PASAPORTVERILDIGIYER, "");
                nufus.pasaportVerenMakam = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.PASAPORTVERENMAKAM, "");
                nufus.pasaportSuresi = VeriErisim.DegerAl(r, nufusAlanlarIlkIndex + (int)TABPersonel.ENUMAlanlarNUFUS.PASAPORTSURESI, 0);
            }
        }

        //public ObjectArray ListeleUnvanKadro(Kullanici kullanan, Personel p)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABPersonel tab = new TABPersonel(ve);

        //    IDataReader r = null;

        //    try
        //    {
        //        r = tab.ListeleUnvanKadro(p);
        //    }
        //    catch (Exception e)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = false;
        //        pVAN.sonuc.hataStr = e.Message;

        //        return pVAN;
        //    }

        //    if (r == null)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = true;
        //        pVAN.sonuc.bilgiStr = "Arama yapılabilmesi için bir kriter bilgisi girilmesi gerekmektedir.";
        //        return pVAN;
        //    }

        //    while (r.Read())
        //    {
        //        PersonelUnvanKadro pp = new PersonelUnvanKadro();

        //        pp.unvanAd = VeriErisim.DegerAl(r, 0, "");
        //        pp.cinsiyet = VeriErisim.DegerAl(r, 1, 0);
        //        pp.cinsiyetAdet = VeriErisim.DegerAl(r, 2, 0);

        //        pVAN.objeler.Add(pp);
        //    }
        //    r.Close();

        //    pVAN.sonuc.islemSonuc = true;

        //    if (pVAN.objeler.Count == 0)
        //        pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

        //    ve.VeriErisimKapat();

        //    return pVAN;
        //}

        public static ObjectVAN OkuMalBildirimi(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            //    ObjectVAN o = new ObjectVAN();
            //    o.o = new TNS.IKY.PersonelMalBildirimi
            //    {
            //        kimlikListesiTanimli = new List<MalBilKimlik>
            //{
            //    new MalBilKimlik
            //    {
            //        siraNo = 1,
            //        adiSoyadi = "Ali Veli",
            //        dogumTarihi = new TNSDateTime("01.01.1990"),
            //        dogumYeri = "Ankara",
            //        yakinligi = "Kendisi",
            //        tcKimlikNo = "12345678901"
            //    }
            //},

            //        tasinmazListesiTanimli = new List<MalBilTasinmaz>
            //{
            //    new MalBilTasinmaz
            //    {
            //        siraNo = 1,
            //        tasinmazCinsi = "Daire",
            //        adres = "Çankaya / Ankara",
            //        hisseMiktari = 1,
            //        malikTCKN = "12345678901"
            //    }
            //},

            //        kooperatifListesiTanimli = new List<MalBilKooperatif>
            //{
            //    new MalBilKooperatif
            //    {
            //        siraNo = 1,
            //        kooperatifAdiYeri = "Örnek Koop / İzmir",
            //        hisseDegeri = 250000,
            //        uyelikTarihi = new TNSDateTime("01.06.2020"),
            //        tcKimlikNo = "12345678901"
            //    }
            //},

            //        tasitListesiTanimli = new List<MalBilTasit>
            //{
            //    new MalBilTasit
            //    {
            //        siraNo = 1,
            //        plakaNo = "06 ABC 123",
            //        tasitinCinsi = "Otomobil",
            //        tasitinMarkasi = "Toyota",
            //        edinmeDegeri = 750000,
            //        modelYili = 2022,
            //        tcKimlikNo = "12345678901"
            //    }
            //},

            //        digerTasinirListesiTanimli = new List<MalBilDigerTasinir>
            //{
            //    new MalBilDigerTasinir
            //    {
            //        siraNo = 1,
            //        tasinirMalCinsi = "Antika Saat",
            //        edinmeDegeri = 120000,
            //        tcKimlikNo = "12345678901"
            //    }
            //},

            //        bankaMenkulListesiTanimli = new List<MalBilBankaMenkul>
            //{
            //    new MalBilBankaMenkul
            //    {
            //        siraNo = 1,
            //        nitelik = "Vadeli Mevduat",
            //        miktari = 500000,
            //        cinsi = "TL",
            //        tcKimlikNo = "12345678901"
            //    }
            //},

            //        altinMucevherListesiTanimli = new List<MalBilAltinMucevher>
            //{
            //    new MalBilAltinMucevher
            //    {
            //        siraNo = 1,
            //        cinsi = "22 Ayar Bilezik",
            //        degeri = 180000,
            //        miktari = 2,
            //        tcKimlikNo = "12345678901"
            //    }
            //        },

            //        borcAlacakListesiTanimli = new List<MalBilBorcAlacak>
            //{
            //    new MalBilBorcAlacak
            //    {
            //        siraNo = 1,
            //        borcluAdi = "Mehmet Yılmaz",
            //        alacakliAdi = "Ali Veli",
            //        tutar = 95000
            //    }
            //},

            //        digerServetListesiTanimli = new List<MalBilDigerServet>
            //{
            //    new MalBilDigerServet
            //    {
            //        siraNo = 1,
            //        servetUnsuru = "Lisans Hakkı",
            //        edinmeSekli = "Satın Alma",
            //        tcKimlikNo = "12345678901"
            //    }
            //}
            //    };
            //    return o;

            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleMalBildirimi(veriErisim, kullanan, new PersonelMalBildirimi() { kod = kod });
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;


        }

        public static ObjectArray ListeleMalBildirimi(VeriErisim veriErisim, Kullanici kullanan, PersonelMalBildirimi form)
        {
            ObjectArray oa = new ObjectArray();
            //if (!IKY.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}


            //if (!GormeYetkisiVarMi(kullanan, form))
            //{
            //    oa.sonuc.DegerAta(InsanKaynaklariResI.PersonelMalBildirimi002);
            //    return oa;
            //}

            //Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    oa.sonuc.DegerAta(s.hataStr);
            //    return oa;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYMALBILDIRIMI tab = new TABIKYMALBILDIRIMI(ve);

            IDataReader dr;
            dr = tab.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                PersonelMalBildirimi detay = new PersonelMalBildirimi();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlar.KOD, 0);
                detay.personelKod = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlar.PKOD, 0);
                detay.tarih = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlar.TARIH);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlar.ACIKLAMA, "");
                oa.Ekle(detay);
            }
            dr.Close();
            TNSCollection al;
            foreach (PersonelMalBildirimi formIc in oa.objeler)
            {
                dr = tab.OkuMalBilkimlik(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    MalBilKimlik detay = new MalBilKimlik();
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarMALBILKIMLIK.SIRANO, 0);
                    detay.adiSoyadi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarMALBILKIMLIK.ADISOYADI, "");
                    detay.dogumTarihi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarMALBILKIMLIK.DOGUMTARIHI);
                    detay.dogumYeri = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarMALBILKIMLIK.DOGUMYERI, "");
                    detay.yakinligi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarMALBILKIMLIK.YAKINLIGI, "");
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarMALBILKIMLIK.TCKIMLIKNO, "");
                    al.Add(detay);
                }
                formIc.kimlikListesi = al;
                dr.Close();

                dr = tab.OkuTasinmaz(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    MalBilTasinmaz detay = new MalBilTasinmaz();
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASINMAZ.SIRANO, 0);
                    detay.degeri = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASINMAZ.DEGERI, 0.0);
                    detay.tasinmazCinsi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASINMAZ.TASINMAZCINSI, "");
                    detay.adres = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASINMAZ.ADRES, "");
                    detay.hisseMiktari = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASINMAZ.HISSEMIKTARI, 0.0);
                    detay.edinmeTarihi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASINMAZ.EDINMETARIHI);
                    detay.malikTCKN = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASINMAZ.MALIKTCKN, "");
                    al.Add(detay);
                }
                formIc.tasinmazListesi = al;
                dr.Close();

                dr = tab.OkuKooperatif(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    MalBilKooperatif detay = new MalBilKooperatif();
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarKOOPERATIF.SIRANO, 0);
                    detay.kooperatifAdiYeri = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarKOOPERATIF.KOOPERATIFADIYERI, "");
                    detay.hisseDegeri = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarKOOPERATIF.HISSEDEGERI, 0.0);
                    detay.uyelikTarihi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarKOOPERATIF.UYELIKTARIHI);
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarKOOPERATIF.TCKIMLIKNO, "");
                    al.Add(detay);
                }
                formIc.kooperatifListesi = al;
                dr.Close();

                dr = tab.OkuTasit(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    MalBilTasit detay = new MalBilTasit();
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASIT.SIRANO, 0);
                    detay.plakaNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASIT.PLAKANO, "");
                    detay.tasitinCinsi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASIT.TASITINCINSI, "");
                    detay.tasitinMarkasi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASIT.TASITINMARKASI, "");
                    detay.edinmeDegeri = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASIT.EDINMEDEGERI, 0.0);
                    detay.modelYili = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASIT.MODELYILI, 0);
                    detay.edinmeTarihi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASIT.EDINMETARIHI);
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarTASIT.TCKIMLIKNO, "");
                    al.Add(detay);
                }
                formIc.tasitListesi = al;
                dr.Close();

                dr = tab.OkuDigerTasinir(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    MalBilDigerTasinir detay = new MalBilDigerTasinir();
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarDIGERTASINIR.SIRANO, 0);
                    detay.tasinirMalCinsi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarDIGERTASINIR.TASINIRMALCINSI, "");
                    detay.edinmeDegeri = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarDIGERTASINIR.EDINMEDEGERI, 0.0);
                    detay.modelYili = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarDIGERTASINIR.MODELYILI, 0);
                    detay.edinmeTarihi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarDIGERTASINIR.EDINMETARIHI);
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarDIGERTASINIR.TCKIMLIKNO, "");
                    al.Add(detay);
                }
                formIc.digerTasinirListesi = al;
                dr.Close();

                dr = tab.OkuBankaMenkul(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    MalBilBankaMenkul detay = new MalBilBankaMenkul();
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarBANKAMENKUL.SIRANO, 0);
                    detay.nitelik = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarBANKAMENKUL.NITELIK, "");
                    detay.miktari = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarBANKAMENKUL.MIKTARI, 0.0);
                    detay.cinsi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarBANKAMENKUL.CINSI, "");
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarBANKAMENKUL.TCKIMLIKNO, "");
                    al.Add(detay);
                }
                formIc.bankaMenkulListesi = al;
                dr.Close();

                dr = tab.OkuAltinMucevher(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    MalBilAltinMucevher detay = new MalBilAltinMucevher();
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarALTINMUCEVHER.SIRANO, 0);
                    detay.cinsi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarALTINMUCEVHER.CINSI, "");
                    detay.degeri = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarALTINMUCEVHER.DEGERI, 0.0);
                    detay.miktari = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarALTINMUCEVHER.MIKTARI, 0.0);
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarALTINMUCEVHER.TCKIMLIKNO, "");
                    al.Add(detay);
                }
                formIc.altinMucevherListesi = al;
                dr.Close();

                dr = tab.OkuBorcAlacak(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    MalBilBorcAlacak detay = new MalBilBorcAlacak();
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarBORCALACAK.SIRANO, 0);
                    detay.borcluAdi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarBORCALACAK.BORCLUADI, "");
                    detay.alacakliAdi = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarBORCALACAK.ALACAKLIADI, "");
                    detay.tutar = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarBORCALACAK.TUTAR, 0.0);
                    al.Add(detay);
                }
                formIc.borcAlacakListesi = al;
                dr.Close();

                dr = tab.OkuDigerServet(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    MalBilDigerServet detay = new MalBilDigerServet();
                    detay.siraNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarDIGERSERVET.SIRANO, 0);
                    detay.servetUnsuru = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarDIGERSERVET.SERVETUNSURU, "");
                    detay.edinmeSekli = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarDIGERSERVET.EDINMESEKLI, "");
                    detay.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABIKYMALBILDIRIMI.ENUMAlanlarDIGERSERVET.TCKIMLIKNO, "");
                    al.Add(detay);
                }
                formIc.digerServetListesi = al;
                dr.Close();
            }
            ve.VeriErisimKapat();
            return oa;
        }

        public ObjectArray ListeleTarihce(Kullanici kullanan, PersonelTarihce p)
        {
            ObjectArray pVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleTarihce(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelTarihce pT = new PersonelTarihce();
                pT.kod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.KOD, 0);
                pT.personelKod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.PKOD, 0);
                pT.kullanici = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.KULLANICI, "");
                pT.tarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.TARIH);
                pT.degisiklik = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.DEGISIKLIK, "");
                pT.kullaniciAd = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.KULLANICIAD, "");

                pVAN.objeler.Add(pT);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc Kaydet(Kullanici kullanan, Personel p)
        {
            object eski = null, yeni = null; //Tarihçe için

            Sonuc sonuc = new Sonuc();
            TNS.ORG.OrganizasyonBirim birim = null;

            if (p.kod != 0)
            {
                sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.kod, sonuc, ENUMYetkiIslem.OzlukDetay);
            }
            else
            {
                if (p.kadro.organizasyonKodu == 0)
                    sonuc.DegerAta("Kaydetmek için organizyon kodunu seçiniz!");
                else
                {
                    birim = ORG.Arac.OrgBirimBul(p.kadro.organizasyonKodu);
                    if (birim != null)
                    {
                        sonuc = IKYYetki.YetkiVarMiOGY(kullanan, birim.kopruKod, p.personelTip, sonuc, MUH.ENUMOGY.Yazma, ENUMYetkiIslem.OzlukDetay, false, null);
                    }
                    else
                        sonuc.DegerAta("Kaydetmek için organizyon kodunu seçiniz!");
                }
            }
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            string hata = p.Kontrol(kullanan);
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            ObjectArray pVAN = new ObjectArray();
            //pp.muhasebeKodu = -1;
            //pp.birimKodu = "0";
            //pVAN = Listele(kullanan, pp);
            //if (pVAN.sonuc.islemSonuc)
            //{
            //    foreach (Personel o in pVAN.objeler)
            //    {
            //        if ((o.birimKodu != p.birimKodu || o.muhasebeKodu != p.muhasebeKodu) && (OrtakFonksiyonlar.ConvertToInt(o.birimKodu, 0) != 0 && o.muhasebeKodu != 0))
            //            hata += "Bu personel, " + o.muhasebeKodu + "/" + o.birimKodu + " biriminde kayıtlıdır. Bu personeli kendinize kayıt edebilmeniz için ilgili birim tarafından durum bilgisi nakil olarak değiştirilmelidir.";
            //    }
            //}

            if (hata != "")
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            Personel pp = new Personel();
            pp.kod = p.kod;

            if (pp.kod > 0)
            {
                //Tarihçe ********************
                this.eskiTmp = Listele(ve, kullanan, pp, false, false);
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }

            if (eski == null)
                eski = new Personel();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            ve.BeginTransaction();

            //if (p.durum > 5)
            //{
            //    p.muhasebeKodu = 0;
            //    p.birimKodu = "0";
            //}
            p.aktifMi = 1;
            ENUMTARIHCEISLEMTIP islemTip;
            if (p.kod > 0)
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.Guncelle(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                bool varMi = tab.PersonelVarMi(p.tcKimlikNo);
                if (!varMi)
                {
                    p.kod = TABGenel.SayacDegerAlInt(ve, TABPersonel.TABLOAD);
                    sonuc = tab.Kaydet(p);
                    if (sonuc.islemSonuc)
                    {
                        p.kadro.personelKod = p.kod;
                        p.kadro.gorevOrganizasyonKodu = p.kadro.organizasyonKodu;
                        if (TNSDateTime.IsNull(p.kadro.atamaTarihi))
                            p.kadro.atamaTarihi = new TNSDateTime(DateTime.Now);
                        p.kadro.hizmetSebebiKod = 1;
                        p.kadro.gorevYeri = "Yeni Kayıt";
                        p.kadro.kadroDurum = ENUMKadroDurumu.AKTIFYENI;
                        sonuc = TNS.IKY.IsKurallari.IKYPersonelKadro.KaydetKadro(ve, kullanan, p.kadro, true);
                    }
                }
                else
                {
                    sonuc.DegerAta("Kaydetmek istediğiniz TC Kimlik Numarası ile daha önce personel kaydı yapılmış.");
                }
            }

            if (sonuc.islemSonuc)
            {
                p.nufus.kod = p.kod;
                sonuc = tab.KaydetNufus(p.nufus, false);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = PersonelResI.PER007;
                    sonuc.anahtar = p.kod.ToString();

                    yeni = p;
                    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.KISISEL, p.kod, islemTip);
                }
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc TCKimlikNoDegistir(Kullanici kullanan, Personel p)
        {
            object eski = null, yeni = null; //Tarihçe için

            Sonuc sonuc = new Sonuc();

            string hata = "";
            if (string.IsNullOrWhiteSpace(p.tcKimlikNo))
                hata += "TC Kimlik numarası boş bırakılamaz!<br>";

            if (p.kod <= 0)
                hata += "Personel bilgisi bulunamadı! Lütfen öncelikle personel seçiniz.<br>";
            else
            {
                sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.kod, sonuc, ENUMYetkiIslem.OzlukDetay);
                if (!sonuc.islemSonuc)
                    hata += sonuc.hataStr;
            }

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            Personel pp = new Personel { kod = p.kod };

            if (pp.kod > 0)
            {
                //Tarihçe ********************
                this.eskiTmp = Listele(ve, kullanan, pp, false, false);
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }

            string eskiTC = eski != null ? ((Personel)eski).tcKimlikNo : "";

            if (string.IsNullOrWhiteSpace(eskiTC))
            {
                sonuc.hataStr = "Personel eski TC Kimlik No bilgisi okunamadı";
                sonuc.islemSonuc = false;
            }
            else if (eskiTC == p.tcKimlikNo)
            {
                sonuc.hataStr = "Eski TC Kimlik No ile Yeni TC Kimlik No aynı olamaz";
                sonuc.islemSonuc = false;
            }
            else
            {
                ve.BeginTransaction();

                sonuc = tab.TCKimlikNoGuncelle(((Personel)eski).tcKimlikNo, p.tcKimlikNo);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    //Tarihçe Kaydet ********************
                    yeni = p;
                    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.KISISEL, p.kod, ENUMTARIHCEISLEMTIP.GUNCELLE);
                }
                else
                    ve.Rollback();

            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetIletisim(Kullanici kullanan, PersonelIletisim p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (p.kod == 0)
                sonuc.DegerAta("Personel bilgisi boş bırakılamaz.<br>");
            if (string.IsNullOrEmpty(p.eposta))
            {
                sonuc.DegerAta("Personel ePosta adresi boş bırakılamaz.<br>");
            }

            try
            {
                System.Net.Mail.MailAddress ma = new System.Net.Mail.MailAddress(p.eposta);
            }
            catch
            {
                sonuc.DegerAta("hatalı ePosta adresi girilmiş.<br>");
            }
            if (!string.IsNullOrWhiteSpace(p.eposta2))
            {
                try
                {
                    System.Net.Mail.MailAddress ma = new System.Net.Mail.MailAddress(p.eposta2);
                }
                catch
                {
                    sonuc.DegerAta("hatalı kişisel ePosta adresi girilmiş.<br>");
                }
            }
            Personel per = null;
            if (sonuc.islemSonuc)
            {
                per = IKYYetki.PersonelBul(kullanan, p.kod, null);
                IKYYetki.YetkiVarMiYazmayaPersonel(kullanan, per, sonuc, ENUMYetkiIslem.Iletisim);
            }

            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABPersonel personel = new TABPersonel(ve);
            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;

            eski = ListeleIletisim(ve, kullanan, p.kod);

            if (((PersonelIletisim)eski).kod == 0)
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;

            sonuc = personel.KaydetIletisim(p, false);

            //Tarihçe Kaydet ********************
            yeni = p;
            Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.ADRES, p.kod, islemTip);

            if (sonuc.islemSonuc)
            {
                sonuc = personel.GuncelleKullaniciMail(per.tcKimlikNo, p.eposta);
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

        //public Sonuc KaydetSozlesmeliBordroTanim(Kullanici kullanan, SozlesmeliBordroTanim p)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    if (!IKY.Yetki.YetkiVarMiYazmaya(kullanan))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = PersonelResI.PER011;

        //        return sonuc;
        //    }

        //    string hata = "";

        //    if (p.tcKimlikNo.Trim() == "")
        //        hata += "Personel bilgisi boş bırakılamaz.<br>";

        //    if (hata != "")
        //    {
        //        sonuc.hataStr = hata;
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABPersonel tab = new TABPersonel(ve);

        //    //BARIS: islemSonuc olmadan kaydet ve güncelleye karar vermem lazım
        //    sonuc = tab.GuncelleSozlesmeliBordroTanim(p);
        //    if (!sonuc.islemSonuc)
        //        sonuc = tab.KaydetSozlesmeliBordroTanim(p);

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = PersonelResI.PER007;
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public Sonuc KaydetKurs(Kullanici kullanan, PersonelKurs p)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    object eski = null, yeni = null; //Tarihçe için

        //    if (!IKY.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = PersonelResI.PER011;

        //        return sonuc;
        //    }

        //    string hata = "";
        //    if (p.kursKod == 0)
        //        hata = "Kurs bilgisi boş bırakılamaz.<br>";
        //    if (p.tcKimlikNo.Trim() == "")
        //        hata += "Personel bilgisi boş bırakılamaz.<br>";

        //    if (hata != "")
        //    {
        //        sonuc.hataStr = hata;
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    //Tarihçe ********************
        //    if (p.siraNo > 0)
        //    {
        //        this.eskiTmp = this.ListeleKurs(kullanan, new PersonelKurs() { tcKimlikNo = p.tcKimlikNo, siraNo = p.siraNo });
        //        if (eskiTmp.objeler.Count == 1)
        //            eski = eskiTmp[0];
        //    }
        //    if (eski == null)
        //        eski = new PersonelKurs();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
        //    //****************************

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABPersonel tab = new TABPersonel(ve);

        //    ENUMTARIHCEISLEMTIP islemTip;
        //    if (p.siraNo == 0)
        //    {
        //        islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
        //        sonuc = tab.KaydetKurs(p);
        //    }
        //    else
        //    {
        //        islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
        //        sonuc = tab.GuncelleKurs(p);
        //    }

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = PersonelResI.PER007;

        //        //Tarihçe Kaydet ********************
        //        yeni = p;
        //        Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.personelKod, islemTip);
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        public static string VERIERISIMAD = "BaglantiSatiriIKY";

        public static Sonuc KaydetMalBildirimi(VeriErisim veriErisim, Kullanici kullanan, PersonelMalBildirimi form)
        {
            //return new Sonuc("Hazır değil!");
            Sonuc s = new Sonuc(true, "", "");
            //if (!YazmaYetkisiVarMi(kullanan, form))
            //{
            //    s.DegerAta(InsanKaynaklariResI.PersonelMalBildirimi001);
            //    return s;
            //}

            //s = AlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    return s;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYMALBILDIRIMI tab = new TABIKYMALBILDIRIMI(ve);
            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tab.SilDigerServet(form);
            if (s.islemSonuc)
                s = tab.SilBorcAlacak(form);
            if (s.islemSonuc)
                s = tab.SilAltinMucevher(form);
            if (s.islemSonuc)
                s = tab.SilBankaMenkul(form);
            if (s.islemSonuc)
                s = tab.SilDigerTasinir(form);
            if (s.islemSonuc)
                s = tab.SilTasit(form);
            if (s.islemSonuc)
                s = tab.SilKooperatif(form);
            if (s.islemSonuc)
                s = tab.SilTasinmaz(form);
            if (s.islemSonuc)
                s = tab.SilMalBilkimlik(form);
            //if (s.islemSonuc)
            //    s = tab.Sil(form);

            if (s.islemSonuc)
                if (form.kod <= 0)
                {
                    s = tab.Ekle(form);
                    form.kod = OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0);
                }
                else
                    s = tab.Guncelle(form);

            for (int i = 0; i < form.kimlikListesi.Count && s.islemSonuc; i++)
            {
                s = tab.EkleMalBilkimlik(form.kod, i + 1, (MalBilKimlik)form.kimlikListesi[i]);
            }

            for (int i = 0; i < form.tasinmazListesi.Count && s.islemSonuc; i++)
            {
                s = tab.EkleTasinmaz(form.kod, i + 1, (MalBilTasinmaz)form.tasinmazListesi[i]);
            }

            for (int i = 0; i < form.kooperatifListesi.Count && s.islemSonuc; i++)
            {
                s = tab.EkleKooperatif(form.kod, i + 1, (MalBilKooperatif)form.kooperatifListesi[i]);
            }

            for (int i = 0; i < form.tasitListesi.Count && s.islemSonuc; i++)
            {
                s = tab.EkleTasit(form.kod, i + 1, (MalBilTasit)form.tasitListesi[i]);
            }

            for (int i = 0; i < form.digerTasinirListesi.Count && s.islemSonuc; i++)
            {
                s = tab.EkleDigerTasinir(form.kod, i + 1, (MalBilDigerTasinir)form.digerTasinirListesi[i]);
            }

            for (int i = 0; i < form.bankaMenkulListesi.Count && s.islemSonuc; i++)
            {
                s = tab.EkleBankaMenkul(form.kod, i + 1, (MalBilBankaMenkul)form.bankaMenkulListesi[i]);
            }

            for (int i = 0; i < form.altinMucevherListesi.Count && s.islemSonuc; i++)
            {
                s = tab.EkleAltinMucevher(form.kod, i + 1, (MalBilAltinMucevher)form.altinMucevherListesi[i]);
            }

            for (int i = 0; i < form.borcAlacakListesi.Count && s.islemSonuc; i++)
            {
                s = tab.EkleBorcAlacak(form.kod, i + 1, (MalBilBorcAlacak)form.borcAlacakListesi[i]);
            }

            for (int i = 0; i < form.digerServetListesi.Count && s.islemSonuc; i++)
            {
                s = tab.EkleDigerServet(form.kod, i + 1, (MalBilDigerServet)form.digerServetListesi[i]);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;


            //Sonuc sonuc = new Sonuc();
            //object eski = null, yeni = null; //Tarihçe için

            //sonuc = IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.personelKod, sonuc, ENUMYetkiIslem.MalBildirimi);
            //if (!sonuc.islemSonuc)
            //{
            //    return sonuc;
            //}

            //string hata = "";
            //if (p.tcKimlikNo.Trim() == "")
            //    hata += "Personel bilgisi boş bırakılamaz.<br>";

            //if (hata != "")
            //{
            //    sonuc.hataStr = hata;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            ////Tarihçe ********************
            //if (p.sirano > 0)
            //{
            //    this.eskiTmp = this.ListeleMalBildirimi(kullanan, new PersonelMalBildirimi() { tcKimlikNo = p.tcKimlikNo, sirano = p.sirano });
            //    if (eskiTmp.objeler.Count == 1)
            //        eski = eskiTmp[0];
            //}
            //if (eski == null)
            //    eski = new PersonelMalBildirimi();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            ////****************************

            //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            //ve.BeginTransaction();

            //TABPersonel tab = new TABPersonel(ve);

            //ENUMTARIHCEISLEMTIP islemTip;
            //if (p.sirano == 0)
            //{
            //    islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
            //    sonuc = tab.KaydetMalBildirimi(p);
            //}
            //else
            //{
            //    islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
            //    sonuc = tab.GuncelleMalBildirimi(p);
            //}

            //if (sonuc.islemSonuc)
            //{
            //    ve.Commit();
            //    sonuc.bilgiStr = PersonelResI.PER007;

            //    //Tarihçe Kaydet ********************
            //    yeni = p;
            //    Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.MALBILDIRIMI, p.personelKod, islemTip);
            //}
            //else
            //    ve.Rollback();

            //ve.VeriErisimKapat();

            //return sonuc;
        }

        //public Sonuc KaydetIzin(Kullanici kullanan, PersonelIzin p, bool OnayIslemi)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    object eski = null, yeni = null; //Tarihçe için

        //    //if (!IKY.Yetki.YetkiVarMiYazmaya(kullanan))
        //    //{
        //    //    sonuc.islemSonuc = false;
        //    //    sonuc.hataStr = PersonelResI.PER011;

        //    //    return sonuc;
        //    //}

        //    string hata = "";
        //    if (p.tcKimlikNo.Trim() == "")
        //        hata = "Personel T.C Kimlik Numarası boş bırakılamaz<br>";
        //    if (!p.baslangicTarihi.isNull && !p.bitisTarihi.isNull)
        //    {
        //        if (p.baslangicTarihi.Oku() > p.bitisTarihi.Oku())
        //            hata += "İzin başlangıç tarihi bitiş tarihinden geç olamaz<br>";
        //        else
        //        {
        //            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //            TABPersonel tab2 = new TABPersonel(ve2);
        //            if (tab2.KaydetIzinUygunMu(p) > 0)
        //                hata += "Belirtilen tarih aralığında başka bir izinle çakışıyor<br>";
        //        }
        //    }

        //    if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı listeniyor
        //    {
        //        if (p.onay > 0)
        //            hata += "Sadece onaysız Personel izin bilgisi kaydı yapılabilirsiniz<br>";
        //    }

        //    if (!OnayIslemi)
        //    {
        //        if (p.tip == 0)
        //            hata += "Personel izin türü boş bırakılamaz<br>";
        //        if (p.baslangicTarihi.isNull)
        //            hata += "Personel izin başlama tarihi boş bırakılamaz<br>";
        //        if (p.bitisTarihi.isNull)
        //            hata += "Personel izin bitiş tarihi boş bırakılamaz<br>";
        //    }
        //    else
        //    {
        //        if (p.sirano == 0)
        //            hata += "Daha önce kayıt edilmiş izin bilgisi seçiniz<br>";
        //        else if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı kayıt yapması
        //            hata += "Onaylama işlemi yapamazsınız<br>";
        //    }

        //    if (hata != "")
        //    {
        //        sonuc.hataStr = hata;
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    //Tarihçe ********************
        //    if (p.sirano > 0)
        //    {
        //        this.eskiTmp = this.ListeleIzin(kullanan, new PersonelIzin() { tcKimlikNo = p.tcKimlikNo, sirano = p.sirano });
        //        if (eskiTmp.objeler.Count == 1)
        //            eski = eskiTmp[0];
        //    }
        //    if (eski == null)
        //        eski = new PersonelIzin();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
        //    //****************************

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABPersonel tab = new TABPersonel(ve);

        //    if (!tab.IzinKayitKontrol(p))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = "Seçilen izin türü için izin hakkınız bulunmamaktadır.";
        //        ve.Rollback();
        //    }
        //    else
        //    {
        //        ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.KAYDET;//BARIS?
        //        if (OnayIslemi)
        //            sonuc = tab.KaydetOnayIzin(p);
        //        else
        //        {
        //            //BARIS
        //            if (p.sirano == 0)
        //            {
        //                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
        //                sonuc = tab.KaydetIzin(p);
        //            }
        //            else
        //            {
        //                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
        //                sonuc = tab.GuncelleIzin(p);
        //            }
        //        }

        //        if (sonuc.islemSonuc)
        //        {
        //            sonuc.anahtar = p.sirano.ToString();
        //            ve.Commit();
        //            sonuc.bilgiStr = PersonelResI.PER007;

        //            //Tarihçe Kaydet ********************
        //            yeni = p;
        //            Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.IZINHAREKET, p.personelKod, islemTip);
        //        }
        //        else
        //            ve.Rollback();
        //    }

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public Sonuc KaydetIzinEvrakBilgi(Kullanici kullanan, PersonelIzin p)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    object eski = null, yeni = null; //Tarihçe için

        //    //if (!IKY.Yetki.YetkiVarMiYazmaya(kullanan))
        //    //{
        //    //    sonuc.islemSonuc = false;
        //    //    sonuc.hataStr = PersonelResI.PER011;

        //    //    return sonuc;
        //    //}

        //    string hata = "";
        //    if (p.tcKimlikNo.Trim() == "")
        //        hata = "Personel T.C Kimlik Numarası boş bırakılamaz<br>";
        //    if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı listeniyor
        //    {
        //        if (p.onay > 0)
        //            hata += "Sadece onaysız Personel izin bilgisi kaydı yapılabilirsiniz<br>";
        //    }


        //    if (p.sirano == 0)
        //        hata += "Daha önce kayıt edilmiş izin bilgisi seçiniz<br>";
        //    else if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı kayıt yapması
        //        hata += "Onaylama işlemi yapamazsınız<br>";

        //    if (hata != "")
        //    {
        //        sonuc.hataStr = hata;
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    //Tarihçe ********************
        //    if (p.sirano > 0)
        //    {
        //        this.eskiTmp = this.ListeleIzin(kullanan, new PersonelIzin() { tcKimlikNo = p.tcKimlikNo, sirano = p.sirano });
        //        if (eskiTmp.objeler.Count == 1)
        //            eski = eskiTmp[0];
        //    }
        //    if (eski == null)
        //        eski = new PersonelIzin();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
        //    //****************************

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABPersonel tab = new TABPersonel(ve);

        //    if (!tab.IzinKayitKontrol(p))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = "Verilen tarih aralığı, kayıtlı izin tarihlerine denk gelmektedir. Lütfen izin tarihlerini kontrol ediniz.";
        //        ve.Rollback();
        //    }
        //    else
        //    {
        //        ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;

        //        sonuc = tab.KaydetIzinEvrakBilgi(p);

        //        if (sonuc.islemSonuc)
        //        {
        //            sonuc.anahtar = p.sirano.ToString();
        //            ve.Commit();
        //            sonuc.bilgiStr = PersonelResI.PER007;

        //            //Tarihçe Kaydet ********************
        //            yeni = p;
        //            Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.IZINHAREKET, p.personelKod, islemTip);
        //        }
        //        else
        //            ve.Rollback();
        //    }

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public Sonuc KaydetIzinAlan(Kullanici kullanan, PersonelIzin p)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    object eski = null, yeni = null; //Tarihçe için

        //    //if (!IKY.Yetki.YetkiVarMiYazmaya(kullanan))
        //    //{
        //    //    sonuc.islemSonuc = false;
        //    //    sonuc.hataStr = PersonelResI.PER011;

        //    //    return sonuc;
        //    //}

        //    string hata = "";
        //    if (p.tcKimlikNo.Trim() == "")
        //        hata = "Personel T.C Kimlik Numarası boş bırakılamaz<br>";
        //    if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı listeniyor
        //    {
        //        if (p.onay > 0)
        //            hata += "Sadece onaysız Personel izin bilgisi kaydı yapılabilirsiniz<br>";
        //    }


        //    if (p.sirano == 0)
        //        hata += "Daha önce kayıt edilmiş izin bilgisi seçiniz<br>";
        //    else if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı kayıt yapması
        //        hata += "Onaylama işlemi yapamazsınız<br>";

        //    if (hata != "")
        //    {
        //        sonuc.hataStr = hata;
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    //Tarihçe ********************
        //    if (p.sirano > 0)
        //    {
        //        this.eskiTmp = this.ListeleIzin(kullanan, new PersonelIzin() { tcKimlikNo = p.tcKimlikNo, sirano = p.sirano });
        //        if (eskiTmp.objeler.Count == 1)
        //            eski = eskiTmp[0];
        //    }
        //    if (eski == null)
        //        eski = new PersonelIzin();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
        //    //****************************

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABPersonel tab = new TABPersonel(ve);

        //    if (!tab.IzinKayitKontrol(p))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = "Verilen tarih aralığı, kayıtlı izin tarihlerine denk gelmektedir. Lütfen izin tarihlerini kontrol ediniz.";
        //        ve.Rollback();
        //    }
        //    else
        //    {
        //        ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;

        //        sonuc = tab.KaydetIzinAlan(p);

        //        if (sonuc.islemSonuc)
        //        {
        //            sonuc.anahtar = p.sirano.ToString();
        //            ve.Commit();
        //            sonuc.bilgiStr = PersonelResI.PER007;

        //            //Tarihçe Kaydet ********************
        //            yeni = p;
        //            Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.IZINHAREKET, p.personelKod, islemTip);
        //        }
        //        else
        //            ve.Rollback();
        //    }

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public Sonuc KaydetIzinDevir(Kullanici kullanan, PersonelIzinDevir[] pList)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    if (!IKY.Yetki.YetkiVarMiYazmaya(kullanan))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = PersonelResI.PER011;

        //        return sonuc;
        //    }

        //    string hata = "";
        //    foreach (PersonelIzinDevir p in pList)
        //    {
        //        if (p.tcKimlikNo.Trim() == "")
        //            hata = "Personel T.C Kimlik Numarası boş bırakılamaz.<br>";
        //        if (p.yil == 0)
        //            hata += "Personel yıl bilgisi boş bırakılamaz.<br>";
        //        //if (!TCKimlikNoKontrolu(p.tcKimlikNo))
        //        //    hata += "T.C Kimlik numarası " + p.tcKimlikNo + " hatalı veya yazılan Ad Soyad ile uyumsuz veya Kimlik doğrulama servisi ile bağlantı kurulamıyor.";

        //        if (hata != "")
        //        {
        //            sonuc.hataStr = hata;
        //            sonuc.islemSonuc = false;
        //            return sonuc;
        //        }
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    ve.BeginTransaction();
        //    TABPersonel tab = new TABPersonel(ve);

        //    foreach (PersonelIzinDevir p in pList)
        //    {
        //        //BARIS: islemSonuc olmadan kaydet ve güncelleye karar vermem lazım
        //        sonuc = tab.GuncelleIzinDevir(p);
        //        if (!sonuc.islemSonuc)
        //            sonuc = tab.KaydetIzinDevir(p);

        //        if (sonuc.islemSonuc)
        //        {
        //            ve.Commit();
        //            sonuc.bilgiStr = PersonelResI.PER007;
        //        }
        //        else
        //            ve.Rollback();
        //    }

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public Sonuc Sil(Kullanici kullanan, Personel p)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
        //    //{
        //    //    sonuc.islemSonuc = false;
        //    //    sonuc.hataStr = PersonelResI.PER011;

        //    //    return sonuc;
        //    //}

        //    if (p.sicilNo == "")
        //    {
        //        sonuc.hataStr = "Silmek istenilen <b>Personelin Sicil Numarası</b> belirtilmemiş.";
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABPersonel tab = new TABPersonel(ve);

        //    sonuc = tab.Sil(p);

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        public Sonuc SilMalBildirimi(Kullanici kullanan, PersonelMalBildirimi p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilMalBildirimi(p);

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

        //public Sonuc SilIzin(Kullanici kullanan, PersonelIzin p)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    string hata = "";
        //    if (p.tcKimlikNo.Trim() == "")
        //        hata = "Personel T.C Numarası boş bırakılamaz.<br>";

        //    if (hata != "")
        //    {
        //        sonuc.hataStr = hata;
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABPersonel tab = new TABPersonel(ve);

        //    sonuc = tab.SilIzin(p);

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public Sonuc SilIzinDevir(Kullanici kullanan, PersonelIzinDevir p)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    string hata = "";
        //    if (p.tcKimlikNo.Trim() == "")
        //        hata = "Personel T.C Numarası boş bırakılamaz.<br>";
        //    if (p.yil == 0)
        //        hata += "Personel yıl bilgisi boş bırakılamaz.<br>";

        //    if (hata != "")
        //    {
        //        sonuc.hataStr = hata;
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABPersonel tab = new TABPersonel(ve);

        //    sonuc = tab.SilIzinDevir(p);

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        public Sonuc SilSozlesmeliBordroTanim(Kullanici kullanan, SozlesmeliBordroTanim p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!IKY.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";

            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilSozlesmeliBordroTanim(p);

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

        //Raporlar
        //public ObjectArray ListelePersonelRapor(Kullanici kullanan, Personel p)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    //if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
        //    //{
        //    //    if (!IKY.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
        //    //    {
        //    //        pVAN.sonuc.islemSonuc = false;
        //    //        pVAN.sonuc.hataStr = "Verdiğiniz birim bilgilerini görme yetkiniz yok.";

        //    //        return pVAN;
        //    //    }
        //    //}

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABPersonel tab = new TABPersonel(ve);

        //    IDataReader r = null;

        //    try
        //    {
        //        r = tab.ListelePersonelRapor(p);
        //    }
        //    catch (Exception e)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = false;
        //        pVAN.sonuc.hataStr = e.Message;

        //        return pVAN;
        //    }

        //    if (r == null)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = true;
        //        pVAN.sonuc.bilgiStr = "Arama yapılabilmesi için bir kriter bilgisi girilmesi gerekmektedir.";
        //        return pVAN;
        //    }

        //    while (r.Read())
        //    {
        //        PersonelPersonelRapor pp = new PersonelPersonelRapor();

        //        pp.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
        //        pp.ad = VeriErisim.DegerAl(r, 1, "");
        //        pp.soyad = VeriErisim.DegerAl(r, 2, "");
        //        pp.unvan = VeriErisim.DegerAl(r, 3, "");
        //        pp.cinsiyet = VeriErisim.DegerAl(r, 4, 0);
        //        pp.dogumTarihi = VeriErisim.DegerAl(r, 5);
        //        pp.okulAdi = VeriErisim.DegerAl(r, 6, "");
        //        pp.seviyesi = VeriErisim.DegerAl(r, 7, 0);
        //        pp.bitirmeTarihi = VeriErisim.DegerAl(r, 8);
        //        pp.dil = VeriErisim.DegerAl(r, 9, "");
        //        pp.notuDil = VeriErisim.DegerAl(r, 10, 0);
        //        pp.duzeyDil = VeriErisim.DegerAl(r, 11, "");
        //        pp.yilDil = VeriErisim.DegerAl(r, 12, 0);
        //        pp.ayDil = VeriErisim.DegerAl(r, 13, 0);
        //        pp.belgeTurDil = VeriErisim.DegerAl(r, 14, 0);

        //        pVAN.objeler.Add(pp);
        //    }
        //    r.Close();

        //    pVAN.sonuc.islemSonuc = true;

        //    if (pVAN.objeler.Count == 0)
        //        pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

        //    ve.VeriErisimKapat();

        //    return pVAN;
        //}

        //public ObjectArray ListeleEgitimRapor(Kullanici kullanan, Personel p, PersonelKurs pk)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    //if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
        //    //{
        //    //    if (!IKY.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
        //    //    {
        //    //        pVAN.sonuc.islemSonuc = false;
        //    //        pVAN.sonuc.hataStr = "Verdiğiniz birim bilgilerini görme yetkiniz yok.";

        //    //        return pVAN;
        //    //    }
        //    //}

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABPersonel tab = new TABPersonel(ve);

        //    IDataReader r = null;

        //    try
        //    {
        //        r = tab.ListeleEgitimRapor(p, pk);
        //    }
        //    catch (Exception e)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = false;
        //        pVAN.sonuc.hataStr = e.Message;

        //        return pVAN;
        //    }

        //    if (r == null)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = true;
        //        pVAN.sonuc.bilgiStr = "Arama yapılabilmesi için bir kriter bilgisi girilmesi gerekmektedir.";
        //        return pVAN;
        //    }

        //    while (r.Read())
        //    {
        //        PersonelEgitimRapor pp = new PersonelEgitimRapor();

        //        pp.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
        //        pp.ad = VeriErisim.DegerAl(r, 1, "");
        //        pp.soyad = VeriErisim.DegerAl(r, 2, "");
        //        pp.unvan = VeriErisim.DegerAl(r, 3, "");
        //        pp.egitiminAdi = VeriErisim.DegerAl(r, 4, "");
        //        pp.egitiminTipi = VeriErisim.DegerAl(r, 5, "");
        //        pp.baslangicTarihi = VeriErisim.DegerAl(r, 6);
        //        pp.bitisTarihi = VeriErisim.DegerAl(r, 7);
        //        pp.dersSaati = VeriErisim.DegerAl(r, 8, 0);
        //        pp.egitimiVerenKurulus = VeriErisim.DegerAl(r, 9, "");
        //        pp.egitimYeri = VeriErisim.DegerAl(r, 10, "");
        //        pp.aciklama = VeriErisim.DegerAl(r, 11, "");
        //        pp.donemi = VeriErisim.DegerAl(r, 12, "");
        //        pp.yili = VeriErisim.DegerAl(r, 13, 0);
        //        pp.dosyaNo = VeriErisim.DegerAl(r, 14, "");

        //        pVAN.objeler.Add(pp);
        //    }
        //    r.Close();

        //    pVAN.sonuc.islemSonuc = true;

        //    if (pVAN.objeler.Count == 0)
        //        pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

        //    ve.VeriErisimKapat();

        //    return pVAN;
        //}

        //public ObjectArray ListeleIzinRapor(Kullanici kullanan, Personel p, PersonelIzin pk)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    //if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
        //    //{
        //    //    if (!IKY.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
        //    //    {
        //    //        pVAN.sonuc.islemSonuc = false;
        //    //        pVAN.sonuc.hataStr = "Verdiğiniz birim bilgilerini görme yetkiniz yok.";

        //    //        return pVAN;
        //    //    }
        //    //}

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABPersonel tab = new TABPersonel(ve);

        //    IDataReader r = null;

        //    try
        //    {
        //        r = tab.ListeleIzinRapor(p, pk);
        //    }
        //    catch (Exception e)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = false;
        //        pVAN.sonuc.hataStr = e.Message;

        //        return pVAN;
        //    }

        //    if (r == null)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = true;
        //        pVAN.sonuc.bilgiStr = "Arama yapılabilmesi için bir kriter bilgisi girilmesi gerekmektedir.";
        //        return pVAN;
        //    }

        //    while (r.Read())
        //    {
        //        PersonelIzinRapor pp = new PersonelIzinRapor();

        //        pp.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
        //        pp.ad = VeriErisim.DegerAl(r, 1, "");
        //        pp.soyad = VeriErisim.DegerAl(r, 2, "");
        //        pp.unvan = VeriErisim.DegerAl(r, 3, "");
        //        pp.sicilNumarasi = VeriErisim.DegerAl(r, 4, "");
        //        pp.baslangicTarihi = VeriErisim.DegerAl(r, 5);
        //        pp.bitisTarihi = VeriErisim.DegerAl(r, 6);
        //        pp.yillikIzin = VeriErisim.DegerAl(r, 7, 0);
        //        pp.tip = VeriErisim.DegerAl(r, 8, 0);
        //        pp.aciklama = VeriErisim.DegerAl(r, 9, "");

        //        pVAN.objeler.Add(pp);
        //    }
        //    r.Close();

        //    pVAN.sonuc.islemSonuc = true;

        //    if (pVAN.objeler.Count == 0)
        //        pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

        //    ve.VeriErisimKapat();

        //    return pVAN;
        //}

        //public ObjectArray ListeleIzinDurum(Kullanici kullanan, PersonelIzinDurum pk)
        //{
        //    ObjectArray pVAN = new ObjectArray();
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABPersonel tab = new TABPersonel(ve);

        //    IDataReader r = null;

        //    try
        //    {
        //        r = tab.ListeleIzinDurum(pk);
        //    }
        //    catch (Exception e)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = false;
        //        pVAN.sonuc.hataStr = e.Message;
        //        return pVAN;
        //    }

        //    if (r == null)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = true;
        //        pVAN.sonuc.bilgiStr = "Hata oluştu. Kriterlerinizi kontrol ediniz.";
        //        return pVAN;
        //    }

        //    while (r.Read())
        //    {
        //        PersonelIzinDurum pp = new PersonelIzinDurum();

        //        pp.tip = VeriErisim.DegerAl(r, 0, 0);
        //        pp.ad = VeriErisim.DegerAl(r, 1, "");
        //        pp.toplamGun = VeriErisim.DegerAl(r, 2, 0);
        //        pp.kullanilanGun = VeriErisim.DegerAl(r, 3, 0);

        //        pVAN.objeler.Add(pp);
        //    }
        //    r.Close();

        //    pVAN.sonuc.islemSonuc = true;

        //    if (pVAN.objeler.Count == 0)
        //        pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

        //    ve.VeriErisimKapat();

        //    return pVAN;
        //}

        public static Personel HizmetBelgesiRaporu(VeriErisim veriErisim, Kullanici kullanan, int pKod)
        {
            Personel per = new Personel();
            per.kod = pKod;

            ObjectArray personel = Listele(veriErisim, kullanan, per, false, true);
            int cnt = 0;
            ENUMCINSIYET cinsiyet = ENUMCINSIYET.TANIMSIZ;
            foreach (Personel p in personel.objeler)
            {
                per = p;
                per.iletisim = ListeleIletisim(veriErisim, kullanan, pKod);
                IKYOgrenim ogrenim = new IKYOgrenim();
                ObjectArray ogrenimler = ogrenim.Listele(kullanan, new PersonelOgrenim { personelKod = pKod });
                per.ogrenim = new PersonelOgrenim[ogrenimler.ObjeSayisi];

                foreach (PersonelOgrenim o in ogrenimler.objeler)
                {
                    per.ogrenim[cnt] = o;
                    cnt++;
                }
                cnt = 0;

                IKYKurs kurs = new IKYKurs();
                ObjectArray kurslar = kurs.Listele(kullanan, new PersonelKurs { personelKod = pKod });
                per.kurs = new PersonelKurs[kurslar.ObjeSayisi];

                foreach (PersonelKurs k in kurslar.objeler)
                {
                    per.kurs[cnt] = k;
                    cnt++;
                }

                cnt = 0;

                if (cinsiyet == ENUMCINSIYET.ERKEK)
                {
                    IKYAskerlik askerlik = new IKYAskerlik();
                    PAskerlik asker = askerlik.Listele(kullanan, pKod);
                    per.askerlik = asker;
                }

                per.kadrolar = IKYPersonelKadro.ListeleKadro(null, kullanan, new PersonelKadro { personelKod = pKod, kadroDurum = ENUMKadroDurumu.IPTALLERHARIC }, true, true);

                cnt = 0;
                ObjectArray aileler = new IKYAile().ListeleAile(null, kullanan, 0, pKod, null, null, null, 0);
                per.aile = new PersonelAile[aileler.ObjeSayisi];
                foreach (PersonelAile a in aileler.objeler)
                {
                    per.aile[cnt] = a;
                    cnt++;
                }

                cnt = 0;
                IKYDil dil = new IKYDil();
                ObjectArray diller = IKYDil.Listele(null, kullanan, new PersonelDil { personelKod = pKod });
                per.dil = new PersonelDil[diller.ObjeSayisi];
                foreach (PersonelDil d in diller.objeler)
                {
                    per.dil[cnt] = d;
                    cnt++;
                }

                //cnt = 0;
                //ObjectArray hizmetSureleri = IKYPersonelKadro.HizmetleriGetir(veriErisim, kullanan, "", pKod.ToString(), "", null, 0, "");
                //per.hizmetSureleri = new Hizmet[hizmetSureleri.ObjeSayisi];
                //foreach (Hizmet h in hizmetSureleri.objeler)
                //{
                //    per.hizmetSureleri[cnt] = h;
                //    cnt++;
                //}
            }

            return per;
        }

        internal ObjectArray PersonelBulOncekiSonraki(VeriErisim veriErisim, Kullanici kullanan, string sicilNo, bool onceki)
        {
            ObjectArray pVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABPersonel tab = new TABPersonel(ve);
            IDataReader r = tab.PersonelBulOncekiSonraki(sicilNo, onceki);
            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }
            while (r.Read())
            {
                Personel pp = new Personel();
                pp.kod = VeriErisim.DegerAl(r, 0, 0);
                pVAN.objeler.Add(pp);
            }
            r.Close();
            ve.VeriErisimKapat();
            return pVAN;
        }
    }
}

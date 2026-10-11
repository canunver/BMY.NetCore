using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYYetki
    {
        //public static Personel PersonelOku(Kullanici kullanan, string tcKimlik)
        //{
        //    Personel p = new Personel();
        //    p.tcKimlikNo = tcKimlik;
        //    ObjectArray oa = TNS.IKY.IsKurallari.IKYPersonel.Listele(null, kullanan, p, true);
        //    if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
        //        return (Personel)oa[0];
        //    return (Personel)null;
        //}

        //public static TNS.ORG.IORGServis servisORG = TNS.ORG.Arac.Tanimla();

        //public static bool YetkisiVarMiBirime(Kullanici kullanan, string saymanlik, string birim, int orgKod)
        //{
        //    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //        return true;

        //    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
        //    {
        //        if (OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0) == OrtakFonksiyonlar.ConvertToInt(saymanlik, 0))
        //            return true;
        //    }

        //    // bool orgKullaniliyor = TNS.IKY.Arac.OrgSemaPersonelKullaniyorMu();
        //    if (orgKod > 0)
        //    {
        //        string[] ytDegerler = YetkiliOlduguBirimler(kullanan);

        //        foreach (string y in ytDegerler)
        //        {
        //            if (orgKod == OrtakFonksiyonlar.ConvertToInt(y, 0))
        //                return true;
        //        }
        //    }
        //    else
        //    {
        //        string[] ytDegerler = YetkiliOlduguBirimler(kullanan);

        //        for (int i = 0; i < ytDegerler.Length; i++)
        //        {
        //            string[] birimler;
        //            birimler = ytDegerler[i].Split('-');
        //            if (birimler.Length > 1)//Yetki tipi saymanlık-birim şeklinde verilmesi lazım
        //            {
        //                if (birimler[0] == "*" && birimler[1] == "*")
        //                    return true;
        //                else if (OrtakFonksiyonlar.ConvertToInt(birimler[0], 0) == OrtakFonksiyonlar.ConvertToInt(saymanlik, 0) && birimler[1] == "*")
        //                    return true;
        //                else if (OrtakFonksiyonlar.ConvertToInt(birimler[0], 0) == OrtakFonksiyonlar.ConvertToInt(saymanlik, 0) && OrtakFonksiyonlar.ConvertToInt(birimler[1], 0) == OrtakFonksiyonlar.ConvertToInt(birim, 0))
        //                    return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //public static string[] YetkiliOlduguBirimler(Kullanici kullanan)
        //{
        //    //bool orgKullaniliyor = TNS.IKY.Arac.OrgSemaPersonelKullaniyorMu();
        //    //if (orgKullaniliyor)
        //    //{
        //    string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Personel Organizasyon Yetkisi");
        //    yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri.TrimEnd(';');
        //    string[] yler = yetkiTipiYetkiDegeri.Split(';');
        //    string donenDeger = "";
        //    for (int i = 0; i < yler.Length; i++)
        //    {
        //        TNS.ORG.OrganizasyonBirim kriter = new TNS.ORG.OrganizasyonBirim();
        //        kriter.kod = OrtakFonksiyonlar.ConvertToInt(yler[i], 0);
        //        ObjectArray dler = servisORG.OrganizasyonBirimListele(kriter, false, false);
        //        kriter.kod = 0;
        //        kriter.kopruKod = ((TNS.ORG.OrganizasyonBirim)dler.objeler[0]).kopruKod;
        //        dler = servisORG.OrganizasyonBirimListele(kriter, false, false);

        //        foreach (TNS.ORG.OrganizasyonBirim o in dler.objeler)
        //        {
        //            if (donenDeger != "") donenDeger += ";";
        //            donenDeger += o.kod.ToString();
        //        }
        //    }

        //    return donenDeger.Split(';');
        //    //}
        //    //else
        //    //{
        //    //    string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Personel Saymanlık-Birim Yetkisi");
        //    //    yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri.TrimEnd(';');
        //    //    return yetkiTipiYetkiDegeri.Split(';');
        //    //}
        //}

        //public static bool YetkiVarMiYazmaya(Kullanici kullanan, string kayitEdecegiMernis = "")
        //{
        //    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //        return true;

        //    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
        //        return true;

        //    if (kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "2", false) ||
        //        kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "3", false))
        //        return true;

        //    if (kullanan.mernis == kayitEdecegiMernis)
        //        return true;

        //    return false;
        //}

        //public static bool YetkiVarMiYazmayaPer(Kullanici kullanan, Personel p, bool kendisiIseKontroleDevam)
        //{
        //    //if (TNS.IKY.Arac.PersonelKendiBilgileriniDegistiremesin())//Bazı kurumlar (Teftiş) Kullanıcıların kendi bilgilerini değiştirmesini istemiyor Melih 04.11.2014
        //    //    kendisiIseKontroleDevam = true;

        //    if (kullanan.mernis == p.tcKimlikNo)
        //        if (!kendisiIseKontroleDevam)
        //            return true;

        //    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //        return true;

        //    Sonuc s = new Sonuc(true, "", "");
        //    if (!YetkisiVarMiBirime(kullanan, p.kadro[0].muhasebeKodu.ToString(), p.kadro[0].muhasebeBirimi, p.kadro[0].organizasyonKodu))
        //        s = TNS.ORG.Arac.Tanimla().KisiBirimiGorebilirMiKopru(kullanan, p.kadro[0].muhasebeKodu, p.kadro[0].muhasebeBirimi);
        //    if (s.islemSonuc)
        //    {
        //        if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
        //            return true;

        //        if (kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "2", false) ||
        //            kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "3", false) ||
        //            kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "4", false)
        //            )
        //            return true;
        //    }
        //    return false;
        //}

        //public static bool YetkiVarMiOkumaya(Kullanici kullanan, string okuyacagiMernis = "")
        //{
        //    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //        return true;

        //    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
        //        return true;

        //    if (kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "1", false) ||
        //        kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "2", false) ||
        //        kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "3", false))
        //        return true;

        //    if (kullanan.mernis == okuyacagiMernis) //Kullanıcı kendi bilgisini görsün diye
        //        return true;

        //    return false;
        //}

        public static Sonuc YetkiVarMiOkumayaPerKod(Kullanici kullanan, int perKod, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiOkumayaPersonel(kullanan, PersonelBul(kullanan, perKod, null), sonuc, islemKod);
        }

        public static Sonuc YetkiVarMiOkumayaTC(Kullanici kullanan, string tc, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiOkumayaPersonel(kullanan, PersonelBul(kullanan, 0, tc), sonuc, islemKod);
        }

        public static Sonuc YetkiVarMiOkumayaPersonel(Kullanici kullanan, Personel p, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiOGY(kullanan, null, 0, sonuc, TNS.MUH.ENUMOGY.Gorme, islemKod, true, p);
        }

        public static Sonuc YetkiVarMiYazmayaPerKod(Kullanici kullanan, int perKod, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiYazmayaPersonel(kullanan, PersonelBul(kullanan, perKod, null), sonuc, islemKod);
        }

        public static Sonuc YetkiVarMiYazmayaTC(Kullanici kullanan, string tc, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiYazmayaPersonel(kullanan, PersonelBul(kullanan, 0, tc), sonuc, islemKod);
        }

        public static Sonuc YetkiVarMiYazmayaPersonel(Kullanici kullanan, Personel p, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiOGY(kullanan, p != null ? p.kopruKodTam : null, 0, sonuc, TNS.MUH.ENUMOGY.Yazma, islemKod, true, p);
        }

        public static Sonuc YetkiVarMiOkumaya(Kullanici kullanan, string kopruKod, int personelTip, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiOGY(kullanan, kopruKod, personelTip, sonuc, TNS.MUH.ENUMOGY.Gorme, islemKod, false, null);
        }

        public static Sonuc YetkiVarMiOnayaPerKod(Kullanici kullanan, int perKod, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiOnayaPersonel(kullanan, PersonelBul(kullanan, perKod, null), sonuc, islemKod);
        }

        public static Sonuc YetkiVarMiOnayaPersonel(Kullanici kullanan, Personel p, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiOGY(kullanan, null, 0, sonuc, TNS.MUH.ENUMOGY.Onaylama, islemKod, true, p);
        }

        public static Sonuc YetkiVarMiYazmaya(Kullanici kullanan, string kopruKod, int personelTip, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiOGY(kullanan, kopruKod, personelTip, sonuc, TNS.MUH.ENUMOGY.Yazma, islemKod, false, null);
        }

        public static Sonuc YetkiVarMiOnaya(Kullanici kullanan, string kopruKod, int personelTip, Sonuc sonuc, ENUMYetkiIslem islemKod)
        {
            return YetkiVarMiOGY(kullanan, kopruKod, personelTip, sonuc, TNS.MUH.ENUMOGY.Onaylama, islemKod, false, null);
        }

        public static Personel PersonelBul(Kullanici kullanan, int perKod, string tc)
        {
            Personel per = new Personel();
            int say = 0;
            if (perKod > 0)
            {
                per.kod = perKod;
                say++;
            }
            if (say == 0 && !string.IsNullOrWhiteSpace(tc))
            {
                per.tcKimlikNo = tc;
                say++;
            }
            if (say > 0)
            {
                ObjectArray oa = IKYPersonel.Listele(null, kullanan, per, true, false);
                if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
                    per = (Personel)oa.objeler[0];
                else
                    per = null;
            }
            else
                per = null;
            return per;
        }

        public static Sonuc YetkiVarMiOGY(Kullanici kullanan, string kopruKod, int personelTip, Sonuc sonuc, MUH.ENUMOGY ogy, ENUMYetkiIslem islemKod, bool kisi, Personel per)
        {
            if (sonuc == null) sonuc = new Sonuc();
            if (kisi)
            {
                if (per == null)
                {
                    sonuc.DegerAta("Yetki kontrolü yapılacak kişi bulunamadı");
                    return sonuc;
                }
                if (per.aktifMi == 0)
                {
                    if (islemKod == ENUMYetkiIslem.Hizmetler)
                        return sonuc;
                    else if (ogy == MUH.ENUMOGY.Gorme)
                    {
                        if (islemKod == ENUMYetkiIslem.HizliBilgi || islemKod == ENUMYetkiIslem.OzlukDetay || islemKod == ENUMYetkiIslem.OzlukOzet)
                            return sonuc;
                    }
                }
                if (string.IsNullOrWhiteSpace(per.kopruKodTam))
                {
                    //sonuc.DegerAta("Kişinin birimi bulunamadı. İşlem yapılamaz!"); Birimi yoksa işlem yapılabilsin.
                    return sonuc;
                }
                if (per.aktifMi == 0 && islemKod != ENUMYetkiIslem.Hizmetler && ogy != MUH.ENUMOGY.Gorme)
                {
                    sonuc.DegerAta("Pasif kişiler (" + per.adSoyad + ") üzerinde, hizmet işlemleri dışında işlem yapılamaz!");
                    return sonuc;
                }
                kopruKod = per.kopruKodTam;
                personelTip = per.personelTip;
            }
            if (string.IsNullOrWhiteSpace(kopruKod))
            {
                if (ogy == MUH.ENUMOGY.Gorme)
                {
                    if (islemKod == ENUMYetkiIslem.HizliBilgi || islemKod == ENUMYetkiIslem.Hizmetler || islemKod == ENUMYetkiIslem.OzlukDetay || islemKod == ENUMYetkiIslem.OzlukOzet)
                        return sonuc;
                }
                else if (ogy == MUH.ENUMOGY.Onaylama || ogy == MUH.ENUMOGY.Yazma || ogy == MUH.ENUMOGY.OnayKaldir)
                {
                    if (islemKod == ENUMYetkiIslem.Hizmetler)
                        return sonuc;
                }
                sonuc.DegerAta("Lütfen işlem yapılacak birimi seçiniz!");
                return sonuc;
            }
            //Kullanıcının birimi ile verilen kopru uyuşuyormu ve kullanıcının bu işlemi
            int i_islemKod = (int)islemKod;
            for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
            {
                string yetkiliBirimKopruKod = kullanan.yetkiTipiDegerleri["KopruKod", i];
                string altBirim = kullanan.yetkiTipiDegerleri["AltBirim", i];
                if (BirimeYetkiVarMi(altBirim, kopruKod, yetkiliBirimKopruKod))
                {
                    string kod = "";
                    string ptKod = "";
                    if (ogy == MUH.ENUMOGY.Gorme)
                    {
                        kod = "OkuIs";
                        ptKod = "GORPERTIP";
                    }
                    else if (ogy == MUH.ENUMOGY.Yazma)
                    {
                        kod = "YazIs";
                        ptKod = "YAZPERTIP";
                    }
                    else if (ogy == MUH.ENUMOGY.Onaylama)
                    {
                        kod = "OnayIs";
                        ptKod = "ONAPERTIP";
                    }
                    if (kod == "") continue;
                    string isler = kullanan.yetkiTipiDegerleri[kod, i];
                    string ptler = kullanan.yetkiTipiDegerleri[ptKod, i];
                    if (string.IsNullOrWhiteSpace(isler)) continue;
                    string[] islerDizi = isler.Split(',', ';');
                    for (int j = 0; j < islerDizi.Length; j++)
                    {
                        if (OrtakFonksiyonlar.ConvertToInt(islerDizi[j], -1) == i_islemKod)
                        {
                            if (personelTip == 0) return sonuc;
                            else
                            {
                                if (PersonelTipVarMi(personelTip, ptler)) return sonuc;
                            }
                        }
                    }
                }
            }
            if (kisi)
                sonuc.DegerAta("Personel (" + (per == null ? "" : per.adSoyad) + ") ile ilgili işlem yapma yetkiniz bulunmamaktadır!");
            else
                sonuc.DegerAta(string.Format("Verilen birim (kodu:{0}) veya personel tipi (kodu:{1}) ile ilgili işlem yapma yetkiniz bulunmamaktadır!", kopruKod, personelTip));
            return sonuc;
        }

        private static bool BirimeYetkiVarMi(string altBirim, string gelKopruKod, string yetkiliBirimKopruKod)
        {
            if (gelKopruKod == null) return false;
            string[] kopruKodlar = gelKopruKod.Split(';');
            foreach (string kopruKod in kopruKodlar)
            {
                if ((altBirim == "1" && kopruKod.StartsWith(yetkiliBirimKopruKod)) || kopruKod == yetkiliBirimKopruKod) return true;
            }
            return false;
        }

        private static bool PersonelTipVarMi(int personelTip, string ptler)
        {
            if (personelTip <= 0 || personelTip >= 10000) return true;
            if (string.IsNullOrWhiteSpace(ptler)) return false;
            string[] ptDizi = ptler.Split(',', ';');
            foreach (string item in ptDizi)
            {
                if (OrtakFonksiyonlar.ConvertToInt(item, 0) == personelTip) return true;
            }
            return false;
        }

        internal static string KullanicininIslemleri(Kullanici kullanan, string kopruKod, MUH.ENUMOGY ogy)
        {
            string isler = "";
            for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
            {
                string yetkiliBirimKopruKod = kullanan.yetkiTipiDegerleri["KopruKod", i];
                string altBirim = kullanan.yetkiTipiDegerleri["AltBirim", i];
                if (BirimeYetkiVarMi(altBirim, kopruKod, yetkiliBirimKopruKod))
                {
                    string kod = "";
                    if (ogy == MUH.ENUMOGY.Gorme)
                        kod = "Oku";
                    else if (ogy == MUH.ENUMOGY.Yazma)
                        kod = "Yaz";
                    else if (ogy == MUH.ENUMOGY.Onaylama)
                        kod = "Onay";
                    if (kod == "") continue;
                    string icIsler = kullanan.yetkiTipiDegerleri[kod + "Is", i];
                    if (string.IsNullOrWhiteSpace(icIsler)) continue;
                    if (!string.IsNullOrWhiteSpace(isler)) isler += ";";
                    isler += icIsler;
                }
            }
            return isler;
        }
    }
}

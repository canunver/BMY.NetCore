using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.IKY.IsKurallari
{
    /// <summary>
    /// PERBordroTurGiris formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class PERBordroTurGiris
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriIKY";
        internal static PersonelMaasParam MaasParamListele(HRC.BordroGenelDegisken[] bgd, string okunanXML)
        {
            PersonelMaasParam prm = new PersonelMaasParam();
            prm.paramlar = new TNSCollection();
            for (int i = 0; i < bgd.Length; i++)
            {
                TNS.HRC.BordroGenelDegisken deg = bgd[i];
                if (deg.degiskenTur == HRC.ENUMBordroDegiskenTur.KisiBordroBilgi)
                {
                    prm.paramlar.Add(new MaasParam(deg.degiskenAd, deg.veriTipi, deg.aciklama));
                }
            }

            XMLStringDataReader dr = new XMLStringDataReader(okunanXML);
            if (dr != null)
            {
                while (dr.Read())
                {
                    string degiskenAd = dr.GetString(0);
                    int ind = prm.paramlar.IndexOf(degiskenAd);
                    if (ind >= 0)
                    {
                        MaasParam mp = (MaasParam)prm.paramlar[ind];
                        mp.degerDbl = dr.GetDouble(1);
                        mp.degerStr = dr.GetString(2);
                        if (mp.degerDbl == 0.666666)
                            mp.degerDbl = 2.0 / 3;
                    }
                }
                dr.Close();
            }
            return prm;
        }

        internal static void MaasMatrahListele(PersonelMaasParam prm, string okunanXML)
        {
            prm.matrahlar = new TNSCollection();
            XMLStringDataReader dr = new XMLStringDataReader(okunanXML);
            if (dr != null)
            {
                while (dr.Read())
                {
                    PersonelMaasMatrah pmm = new PersonelMaasMatrah();
                    pmm.surecNo = VeriErisim.DegerAl(dr, 0, "");
                    pmm.matrahTur = VeriErisim.DegerAl(dr, 1, 0);
                    pmm.tarih = VeriErisim.DegerAl(dr, 2);
                    pmm.yil = pmm.tarih.Yil;
                    pmm.deger = VeriErisim.DegerAl(dr, 3, 0.0);
                    pmm.tabloAd = VeriErisim.DegerAl(dr, 4, "");
                    pmm.satirNo = VeriErisim.DegerAl(dr, 5, 0);

                    prm.matrahlar.Add(pmm);
                }
                dr.Close();
            }
        }

        internal static PersonelMaasParam MaasParamListele(VeriErisim veriErisim, Kullanici kullanan, string personelTCKimlikNo, int yil)
        {
            PersonelMaasParam prm = new PersonelMaasParam();
            TNS.HRC.BordroGenelDegisken.DegiskenleriSifirla();
            prm.paramlar = new TNSCollection();

            for (int i = 0; i < TNS.HRC.BordroGenelDegisken.bordroDegiskenler.Length; i++)
            {
                TNS.HRC.BordroGenelDegisken deg = TNS.HRC.BordroGenelDegisken.bordroDegiskenler[i];
                if (deg.degiskenTur == HRC.ENUMBordroDegiskenTur.KisiBordroBilgi)
                {
                    prm.paramlar.Add(new MaasParam(deg.degiskenAd, deg.veriTipi, deg.aciklama));
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            PERPERSONELMAASPARAM tabPERBORDROTUR = new PERPERSONELMAASPARAM(ve);
            IDataReader dr = tabPERBORDROTUR.ListeleParam(personelTCKimlikNo);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            while (dr.Read())
            {
                string degiskenAd = VeriErisim.DegerAl(dr, 0, "");
                int ind = prm.paramlar.IndexOf(degiskenAd);
                if (ind >= 0)
                {
                    MaasParam mp = (MaasParam)prm.paramlar[ind];
                    mp.degerDbl = VeriErisim.DegerAl(dr, 1, 0.0);
                    mp.degerStr = VeriErisim.DegerAl(dr, 2, "");
                    if (mp.degerDbl == 0.666666)
                        mp.degerDbl = 2.0 / 3;
                }
            }
            dr.Close();
            if (yil >= 0)
            {
                prm.matrahlar = new TNSCollection();
                dr = tabPERBORDROTUR.ListeleMatrah(personelTCKimlikNo, yil);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }

                while (dr.Read())
                {
                    PersonelMaasMatrah pmm = new PersonelMaasMatrah();
                    pmm.yil = yil;
                    pmm.surecNo = VeriErisim.DegerAl(dr, 0, "");
                    pmm.matrahTur = VeriErisim.DegerAl(dr, 1, 0);
                    pmm.tarih = VeriErisim.DegerAl(dr, 2);
                    pmm.deger = VeriErisim.DegerAl(dr, 3, 0.0);
                    pmm.tabloAd = VeriErisim.DegerAl(dr, 4, "");
                    pmm.satirNo = VeriErisim.DegerAl(dr, 5, 0);
                    prm.matrahlar.Add(pmm);
                }
                dr.Close();
            }
            else
                prm.matrahlar = null;
            ve.VeriErisimKapat();
            return prm;
        }

        internal static Sonuc MaasParamKaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection kisiler, TNSCollection degiskenler)
        {
            Sonuc s = new Sonuc();
            if (kisiler.Count == 0)
            {
                s.DegerAta("Lütfen veri giriniz");
                return s;
            }

            string personelTCKimlikNo = ((TNS.IKY.Personel)kisiler[0]).tcKimlikNo;

            s = IKYYetki.YetkiVarMiYazmayaTC(kullanan, personelTCKimlikNo, s, ENUMYetkiIslem.MaasParametreleri);
            if (!s.islemSonuc) return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            PERPERSONELMAASPARAM tabPERBORDROTUR = new PERPERSONELMAASPARAM(ve);

            foreach (TNS.IKY.Personel per in kisiler)
            {
                s = tabPERBORDROTUR.SilParam(per.tcKimlikNo, degiskenler);
                if (s.islemSonuc)
                {
                    foreach (MaasParam item in per.personelMaasParam.paramlar)
                    {
                        s = tabPERBORDROTUR.EkleParam(per.tcKimlikNo, item);
                        if (!s.islemSonuc)
                            break;
                    }
                }
                if (!s.islemSonuc)
                    break;
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectArray MaasMatrahAktar(VeriErisim veriErisim, Kullanici kullanan, TNSDateTime matrahTarih, TNSCollection matrahlar, int yontem, int yukle)
        {
            ObjectArray oa = new ObjectArray();
            System.Text.StringBuilder listeStr = new System.Text.StringBuilder();
            matrahlar.Sort();
            foreach (PersonelMaasMatrah item in matrahlar)
            {
                if (!string.IsNullOrWhiteSpace(item.tcKimlikNo))
                {
                    if (listeStr.Length != 0) listeStr.Append(",");
                    listeStr.Append(VeriErisim.DegerYap(item.tcKimlikNo));
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
            PERPERSONELMAASPARAM tabMatrah = new PERPERSONELMAASPARAM(ve);

            TNSCollection yeniListe = new TNSCollection();
            TNSCollection matrahKucukListe = new TNSCollection();
            IDataReader dr = tabMatrah.MatrahListele(matrahTarih, listeStr, yontem, 1);
            if (dr != null)
            {
                int alanNo;
                if (yontem == 1) alanNo = 1;
                else alanNo = 0;
                while (dr.Read())
                {
                    string tckimlik = VeriErisim.DegerAl(dr, 0, "");
                    double topMat = VeriErisim.DegerAl(dr, 2, 0.0);
                    PersonelMaasMatrah gelenMat = GelenVeri(matrahlar, VeriErisim.DegerAl(dr, alanNo, ""));
                    if (gelenMat == null)
                    {
                    }
                    else if (gelenMat.deger >= topMat)
                    {
                        gelenMat.tcKimlikNo = tckimlik;
                        if (yukle == 1)
                            gelenMat.deger = gelenMat.deger - topMat;
                        else
                            gelenMat.deger = gelenMat.deger;
                        gelenMat.matrahTur = 1;
                        gelenMat.surecNo = "000000000";
                        gelenMat.tarih = matrahTarih;
                        gelenMat.yil = matrahTarih.Yil;
                        yeniListe.Add(gelenMat);
                    }
                    else
                    {
                        if (yukle == 1)
                            oa.sonuc.DegerAta(gelenMat.tcKimlikNo + " numaralı kişinin matrahı mevcut matrahtan düşük, lütfen kontrol ediniz!");
                        else
                        {
                            gelenMat.deger = gelenMat.deger - topMat;
                            matrahKucukListe.Add(gelenMat);
                        }
                    }
                }
                dr.Close();
                if (oa.sonuc.islemSonuc && yukle == 1)
                {
                    ve.BeginTransaction();
                    oa.sonuc = tabMatrah.MatrahSil(matrahTarih, yontem, 1);
                    if (!oa.sonuc.islemSonuc)
                    {
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return oa;
                    }
                    foreach (PersonelMaasMatrah gelenMat in yeniListe)
                    {
                        oa.sonuc = tabMatrah.EkleMatrah(gelenMat);
                        if (!oa.sonuc.islemSonuc) break;
                    }
                    if (oa.sonuc.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                }
            }
            else
                oa.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();

            if (oa.sonuc.islemSonuc)
            {
                string aktarilmayanlar = "";
                if (matrahlar.Count > 0)
                {
                    foreach (PersonelMaasMatrah item in matrahlar)
                    {
                        if (yukle == 1)
                        {
                            if (aktarilmayanlar != "") aktarilmayanlar += ", ";
                            aktarilmayanlar += item.tcKimlikNo;
                        }
                        else
                        {
                            item.deger = 0;
                            matrahKucukListe.Add(item);
                        }
                    }
                    oa.sonuc.bilgiStr = aktarilmayanlar;
                }
            }
            oa.objeler = matrahKucukListe;
            return oa;
        }

        private static PersonelMaasMatrah GelenVeri(TNSCollection matrahlar, string aranan)
        {
            PersonelMaasMatrah mt = null;
            int yer = -1;
            for (int i = 0; i < matrahlar.Count; i++)
            {
                mt = (PersonelMaasMatrah)matrahlar[i];
                if (mt.tcKimlikNo == aranan)
                {
                    yer = i;
                    break;
                }
            }
            if (yer > -1)
            {
                matrahlar.RemoveAt(yer);
                return mt;
            }
            return null;
        }

        internal static Sonuc MaasMatrahKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo, string tabloAd, TNSCollection matrahlar)
        {
            //string personelTCKimlikNo, 
            Sonuc s = new Sonuc();
            //bool yetkisiVar = TNS.PER.Yetki.YetkiVarMiYazmaya(kullanan, TNS.PER.Yetki.PersonelOku(kullanan, personelTCKimlikNo), false);
            //if (!yetkisiVar)
            //    s.DegerAta("Yazma yetkiniz yok");

            foreach (PersonelMaasMatrah item in matrahlar)
            {
                item.yil = yil;
                item.surecNo = item.surecNo.PadLeft(6, '0');
                if (Math.Abs(item.yil - item.tarih.Yil) > 1)
                    s.DegerAta("İşlem tarihi ile kayıt yılı uyumsuz!");
                if (item.matrahTur < 1 || item.matrahTur > PersonelMaasMatrah.matrahTurAdlar.Length)
                    s.DegerAta("Matrah türünü seçiniz!");
            }

            if (!s.islemSonuc) return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            PERPERSONELMAASPARAM tabPERBORDROTUR = new PERPERSONELMAASPARAM(ve);
            string oncTcNo = "";
            //if (!string.IsNullOrWhiteSpace(tabloAd))
            //{
            //    s = tabPERBORDROTUR.SilMatrah("", yil, surecNo, tabloAd, 0);
            //}
            foreach (PersonelMaasMatrah item in matrahlar)
            {
                if (item.surecNo == surecNo)
                {
                    if (item.tcKimlikNo != oncTcNo) //&& string.IsNullOrWhiteSpace(tabloAd)
                    {
                        oncTcNo = item.tcKimlikNo;
                        s = tabPERBORDROTUR.SilMatrah(item.tcKimlikNo, yil, surecNo, item.tabloAd, 0);
                    }
                    if (!s.islemSonuc)
                        break;
                    if (item.deger != 0)
                        s = tabPERBORDROTUR.EkleMatrah(item);
                }
                if (!s.islemSonuc)
                    break;
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc MaasMatrahSil(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo, string tabloAd, TNSCollection tcListe)
        {
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            PERPERSONELMAASPARAM tabPERBORDROTUR = new PERPERSONELMAASPARAM(ve);
            if (tcListe == null)
            {
                s = tabPERBORDROTUR.SilMatrah(null, yil, surecNo, tabloAd, 0);
            }
            else
            {
                foreach (IntKodStrAd item in tcListe)
                {
                    s = tabPERBORDROTUR.SilMatrah(item.ad, yil, surecNo, tabloAd, 0);
                    if (!s.islemSonuc)
                        break;
                }
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }
    }
}

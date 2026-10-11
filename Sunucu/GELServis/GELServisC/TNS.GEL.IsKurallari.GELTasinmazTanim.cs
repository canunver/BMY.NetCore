using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.KYM;
using TNS.GEL;
using TNS.Veri;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELTasinmazTanim
    {

        private static string VeriKontrol(Tasinmaz bilgi, ENUMIslemTuru islemturu)
        {
            StringBuilder hatalar = new StringBuilder();
            if ((islemturu == ENUMIslemTuru.LISTELEME && bilgi.listeSekli == (int)ENUMListelemeSekli.TEKLI) || islemturu == ENUMIslemTuru.GUNCELLEME || islemturu == ENUMIslemTuru.KAYIT)
            {
                if (bilgi.yil <= 0)
                    hatalar.Append("Taşınmaz yıl bilgisi sıfırdan büyük olmalıdır" + "<br/>");

                if (string.IsNullOrEmpty(bilgi.tasinmazSistemKodu))
                    hatalar.Append("Taşınmaz sistem kodu boş olmamalıdır!");
            }

            if (islemturu == ENUMIslemTuru.KAYIT || islemturu == ENUMIslemTuru.GUNCELLEME)
            {

                if (bilgi.tasinmazAd.Trim() == "")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00006 + "<br/>");
                if(bilgi.butceTur<=0)
                    hatalar.Append("Taşınmaz bütçe türü boş olamaz" + "<br/>");

                if (bilgi.bolgeAktif)
                {
                    if (bilgi.bolge.kod <= 0)
                        hatalar.Append("Taşınmaz bölge bilgisi boş olamaz" + "<br/>");

                }
                if (bilgi.ilKod.Trim() == "")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00007 + "<br/>");
                if (bilgi.ilceKod.Trim() == "")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00008 + "<br/>");
                if (string.IsNullOrEmpty(bilgi.tasinmazKisaKod) || bilgi.tasinmazKisaKod == "0")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00114 + "<br/>");
                if (bilgi.edinmeSekli <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00115 + "<br/>");
                if (string.IsNullOrEmpty(bilgi.cins.kod))
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00116 + "<br/>");
                if (bilgi.edinmeTarihi == null || bilgi.edinmeTarihi.Oku().ToShortDateString() == "01.01.0001")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00117 + "<br/>");
            }

            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Tasinmaz bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");

            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.DegerAta(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
                return sonuc;
            }
            if (!string.IsNullOrEmpty(bilgi.tasinmazSistemKodu))
                sonuc.hataStr = VeriKontrol(bilgi, ENUMIslemTuru.GUNCELLEME);
            else
                sonuc.hataStr = VeriKontrol(bilgi, ENUMIslemTuru.KAYIT);

            if (sonuc.hataStr.Trim() != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazTanim tabTasinmaz = new TABTasinmazTanim(ve);
            ve.BeginTransaction();
            //tabTasinmaz.Sil(ve, bilgi.kod);
            bool kayitVar = tabTasinmaz.KayitVarmi(bilgi.tasinmazSistemKodu, bilgi.yil);
            if (kayitVar)
            {
                bilgi.durum = (int)ENUMKayitDurum.GUNCELLENDI;
                sonuc = tabTasinmaz.Guncelle(bilgi);
            }
            else
            {
                //bilgi.sirano = tabTasinmaz.TasinmazSiraNoAl(bilgi.yil);
                TABGenel tg = new TABGenel(ve);
                string sayacKod = bilgi.yil.ToString();
                sayacKod = "GELTASINMAZ" + sayacKod;
                bilgi.tasinmazSistemKodu = bilgi.yil + tg.BelgeNoAl(sayacKod, "000000");
                bilgi.durum = (int)ENUMKayitDurum.YENIKAYIT;
                sonuc = tabTasinmaz.Kaydet(ve, bilgi);
            }
            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = bilgi.tasinmazSistemKodu;
                ve.Commit();
                sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00004;
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Tasinmaz bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (bilgi.yil <= 0)
            {
                sonuc.hataStr = "Taşınmaz yılı boş olmamalıdır";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (string.IsNullOrEmpty(bilgi.tasinmazSistemKodu))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00005;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazTanim tabTasinmaz = new TABTasinmazTanim(ve);
            TABTasinmazEkDosya tabEk = new TABTasinmazEkDosya(ve);
            try
            {
                
                ve.BeginTransaction();
                //**taşınmaza ait ekleri sil
                foreach (TasinmazEkDosya ek in bilgi.ekDosyalar)
                {
                    tabEk.Sil(ek);
                    if (ve.hata != "")
                    {
                        sonuc.DegerAta(ve.hata);
                        break;
                    }
                }
                //*ekler başarılı şekilde silindi ise 
                //*taşınmaz bilgisini sil
                if (sonuc.islemSonuc)
                {
                    tabTasinmaz.Sil(ve, bilgi.tasinmazSistemKodu, bilgi.yil);
                    if (ve.hata.Trim() == "")
                    {
                        sonuc.islemSonuc = true;
                        sonuc.hataStr = "";
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00014;
                        ve.Commit();
                    }
                    else
                    {
                        ve.Rollback();
                        sonuc.islemSonuc = false;
                        if (ve.hata.Trim().IndexOf("(0x80131904): The DELETE statement conflicted with the REFERENCE constraint") > 0)
                            sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00050;
                        else
                            sonuc.hataStr = ve.hata;
                    } 
                }
                else
                {
                    ve.Rollback();
                }
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.ToString());
            }
            finally
            {
                ve.VeriErisimKapat();
            }


            return sonuc;
        }

        public static ObjectArray IcmalListele(VeriErisim veriErisim, Kullanici kullanan, TasinmazIcmal icmal)
        {
            ObjectArray icmalListesi = new ObjectArray();
            string hata = "";
            if (icmal.yil <= 0)
                hata = TNS.GEL.GelirSurecleriResI.GEL00118 + "<br/>";
            if (hata.Trim() != "")
            {
                icmalListesi.sonuc.DegerAta(hata);
                return icmalListesi;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazTanim tabTasinmazTanim = new TABTasinmazTanim(ve);
            ve.BeginTransaction();
            IDataReader dr = tabTasinmazTanim.IcmalListele(icmal);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TasinmazIcmal icm = new TasinmazIcmal();
                    icm.tasinmaz.cins.kod = VeriErisim.DegerAl(dr, 0, "");
                    icm.tasinmaz.cins.kod = GELOrtak.KodNoktala(icm.tasinmaz.cins.kod);
                    icm.tasinmaz.cins.ad = VeriErisim.DegerAl(dr, 1, "");
                    icm.gecenYildanDevir.adet = VeriErisim.DegerAl(dr, 2, (double)0);
                    icm.gecenYildanDevir.yuzolcum = VeriErisim.DegerAl(dr, 3, (double)0);
                    icm.yilIcindeEdinilen.adet = VeriErisim.DegerAl(dr, 4, (double)0);
                    icm.yilIcindeEdinilen.yuzolcum = VeriErisim.DegerAl(dr, 5, (double)0);
                    icm.yilIcindeEldenCikan.adet = VeriErisim.DegerAl(dr, 6, (double)0);
                    icm.yilIcindeEldenCikan.yuzolcum = VeriErisim.DegerAl(dr, 7, (double)0);
                    icm.gelecekYilaDevreden.adet = icm.gecenYildanDevir.adet + icm.yilIcindeEdinilen.adet - icm.yilIcindeEldenCikan.adet;
                    icm.gelecekYilaDevreden.yuzolcum = icm.gecenYildanDevir.yuzolcum + icm.yilIcindeEdinilen.yuzolcum - icm.yilIcindeEldenCikan.yuzolcum;
                    icmalListesi.objeler.Add(icm);
                }
                dr.Close();
            }
            return icmalListesi;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Tasinmaz tasinmaz)
        {
            ObjectArray tasinmazListesi = new ObjectArray();
            string hata = VeriKontrol(tasinmaz, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != "")
            {
                tasinmazListesi.sonuc.DegerAta(hata);
                return tasinmazListesi;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazTanim tabTasinmazTanim = new TABTasinmazTanim(ve);
            IDataReader dr = null;
            try
            {
                ve.BeginTransaction();
                dr = tabTasinmazTanim.Listele(tasinmaz);
                while (dr.Read())
                {
                    Tasinmaz t = new Tasinmaz();
                    t.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 0, "");
                    t.yil = VeriErisim.DegerAl(dr, 1, 0);
                    t.bolge.kod = VeriErisim.DegerAl(dr, 2, 0);
                    t.ilKod = VeriErisim.DegerAl(dr, 3, "");
                    t.ilceKod = VeriErisim.DegerAl(dr, 4, "");
                    t.mahalle = VeriErisim.DegerAl(dr, 5, "");
                    t.koy = VeriErisim.DegerAl(dr, 6, "");
                    t.tasinmazKisaKod = VeriErisim.DegerAl(dr, 7, "");
                    t.tasinmazAd = VeriErisim.DegerAl(dr, 8, "");
                    t.cins.kod = VeriErisim.DegerAl(dr, 9, "");
                    t.cins.kod = GELOrtak.KodNoktala(t.cins.kod);
                    t.paftano = VeriErisim.DegerAl(dr, 10, "");
                    t.adano = VeriErisim.DegerAl(dr, 11, "");
                    t.parselno = VeriErisim.DegerAl(dr, 12, "");
                    t.ciltno = VeriErisim.DegerAl(dr, 13, "");
                    t.ciltSayfaNo = VeriErisim.DegerAl(dr, 14, "");
                    t.ciltSiraNo = VeriErisim.DegerAl(dr, 15, "");
                    t.yuzOlcumuMiktari = VeriErisim.DegerAl(dr, 16, 0.0);
                    t.yuzOlcumuOrani = VeriErisim.DegerAl(dr, 17, 0.0);
                    t.edinmeSekli = VeriErisim.DegerAl(dr, 18, 0);
                    t.edinmeTarihi = VeriErisim.DegerAl(dr, 19);
                    t.edinmeSuresi = VeriErisim.DegerAl(dr, 20, 0.0);
                    t.tescilTarihi = VeriErisim.DegerAl(dr, 21);
                    t.kayittanCikisNedeni = VeriErisim.DegerAl(dr, 22, 0);
                    t.kayittanCikisTarihi = VeriErisim.DegerAl(dr, 23);
                    t.arsivTarihi = VeriErisim.DegerAl(dr, 24);
                    t.arsivNo = VeriErisim.DegerAl(dr, 25, "");
                    t.aciklama = VeriErisim.DegerAl(dr, 26, "");
                    t.enlem = VeriErisim.DegerAl(dr, 27, "");
                    t.boylam = VeriErisim.DegerAl(dr, 28, "");
                    t.mevki = VeriErisim.DegerAl(dr, 29, "");
                    t.malik = VeriErisim.DegerAl(dr, 30, "");

                    t.butceTur = VeriErisim.DegerAl(dr, 31,0);
                    t.daireSayisi = VeriErisim.DegerAl(dr, 32, 0);
                    t.daireAlani = VeriErisim.DegerAl(dr, 33, 0);

                    t.durum = VeriErisim.DegerAl(dr, 34, 0);
                    t.kayitTarihi = VeriErisim.DegerAl(dr, 35);
                    t.islemTarihi = VeriErisim.DegerAl(dr, 36);
                    t.kullanici = VeriErisim.DegerAl(dr, 37, "");
                    t.cins.ad = VeriErisim.DegerAl(dr, 38, "");
                    t.bolge.ad = VeriErisim.DegerAl(dr, 39, "");

                    
                    tasinmazListesi.objeler.Add(t);
                }
                dr.Close();

            }
            catch (Exception ex)
            {
                tasinmazListesi.sonuc.DegerAta(ex.ToString());
            }
            finally
            {
                if (dr != null && !dr.IsClosed) dr.Close();
                ve.VeriErisimKapat();
            }

            return tasinmazListesi;
        }

    }
}

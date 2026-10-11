using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYAskerlik
    {
        public PAskerlik eskiTmp; //Tarihçe için

        public PAskerlik Listele(Kullanici kullanan, int kod)
        {
            PAskerlik askerlik = new PAskerlik();

            IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, kod, askerlik.sonuc, ENUMYetkiIslem.Askerlik);
            if (!askerlik.sonuc.islemSonuc)
            {
                return askerlik;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAskerlik tab = new TABAskerlik(ve);

            IDataReader r = tab.Listele(kod);

            if (r == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            if (r.Read())
            {
                int sonKolon = ((int)TABAskerlik.ENUMAlanlar.ACIKLAMA) + 1;
                askerlik.kod = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.KOD, 0);
                askerlik.askerlikDurumu = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.ASKERLIKDURUMU, 0);
                askerlik.tecilTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.TECILTARIHI);
                askerlik.askerlikBaslamaTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.DUHULTARIHI);
                askerlik.terhisTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.TERHISTARIHI);
                askerlik.askerlikMuafNeden = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.MUAFIYETNEDENI, "");
                askerlik.sayilmayanGunSayisi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.SAYILMAYANGUNSAYISI, 0);
                askerlik.askerlikCuzdanNo = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.ASKERLIKCUZDANNO, "");
                askerlik.rutbesi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.RUTBESI, 0);
                askerlik.sinifi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.SINIFI, 0);
                askerlik.borclanmaVarMi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.BORCLANMAVARMI, 0);
                askerlik.belgeNo = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.TERHISBELGENO, "");
                askerlik.belgeTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.TERHISBELGETARIHI);
                askerlik.askerlikSubesi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.ASKERLIKSUBESI, "");
                askerlik.yedekSubayOkuluGirisTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.YEDEKSUBAYOKULUGIRISTARIHI);
                askerlik.astegmenNaspTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.ASTEGMENNASPTARIHI);
                askerlik.tegmenNaspTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.TEGMENNASPTARIHI);
                askerlik.sinifDonemSicilNo = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.SINIFDONEMSICILNO, "");
                askerlik.erYedekSubayGorevYeri = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.ERYEDEKSUBAYGOREVYERI, "");
                askerlik.erYedekSubayGecisTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.ERYEDEKSUBAYGECISTARIHI);
                askerlik.erYedekSubayKitaHizmetiBaslamaTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.ERYEDEKSUBAYKITAHIZMETIBASLAMATARIHI);
                askerlik.erYedekSubayKitaHizmetiBitisTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.ERYEDEKSUBAYKITAHIZMETIBITISTARIHI);
                askerlik.ibrazIntikalTarihi = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.IBRAZINTIKALTARIHI);
                askerlik.aciklama = VeriErisim.DegerAl(r, (int)TABAskerlik.ENUMAlanlar.ACIKLAMA, "");
                askerlik.askerlikDurumuKayitKod = VeriErisim.DegerAl(r, sonKolon, "");
                askerlik.askerlikDurumHitapID = VeriErisim.DegerAl(r, sonKolon + 1, "");
                askerlik.askerlikDurumuAd = VeriErisim.DegerAl(r, sonKolon + 2, "");
                askerlik.sinifAd = VeriErisim.DegerAl(r, sonKolon + 3, "");

                //Tarihçe Kaydet ********************
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, new PAskerlik(), askerlik, ENUMTARIHCE.ASKERLIK, askerlik.kod, ENUMTARIHCEISLEMTIP.LISTELE);

            }
            r.Close();
            ve.VeriErisimKapat();
            return askerlik;
        }

        public Sonuc Kaydet(Kullanici kullanan, PAskerlik p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.kod, sonuc, ENUMYetkiIslem.Askerlik);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;

            //Tarihçe ********************
            this.eskiTmp = this.Listele(kullanan, p.kod);
            //if (eskiTmp.objeler.Count == 1)
            eski = eskiTmp;

            if (eski == null)
                 eski = new PAskerlik();// { kod = "0" }; // 0 : "Yeni Kayıt"             
            ////****************************

            if(eskiTmp.kod==0)
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABAskerlik tab = new TABAskerlik(ve);

            //if (p.askerlikDurumu == 0)
            //{
            //    ve.VeriErisimKapat();
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = "Askerlik durum bilgisi boş geçilemez.";
            //    return sonuc;
            //}

            sonuc = tab.Kaydet(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = IKYPersonelTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.ASKERLIK, p.kod, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, PAskerlik p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            IKYYetki.YetkiVarMiYazmayaPerKod(kullanan, p.kod, sonuc, ENUMYetkiIslem.Askerlik);
            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABBelgeler tabBelge = new TABBelgeler(ve);
            sonuc = tabBelge.SilBelgeler(new PersonelBelgeler { tipKayitKod = p.kod, pKod = p.kod, tip = (int)ENUMBELGEFORMTIP.PERSONELASKERLIK });

            if (sonuc.islemSonuc)
            {
                TABAskerlik tab = new TABAskerlik(ve);
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
    }
}

using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Geçici görev yolluk tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class PSLSurekliGorevGundelikTanim
    {
        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk tanım bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="gt">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, YollukGundelikTanim gt, bool zorunluYeni)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (gt.yil <= 0)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";

            if (gt.kod.Trim() == string.Empty)
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00211 + "<br>";

            if (gt.deger <= (double)0)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00298 + "<br>";

            if (gt.tur <= 0)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00144 + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevGundelikTanim tabGeciciGundelikTanim = new TABSurekliGorevGundelikTanim(ve);

            ve.BeginTransaction();

            sonuc = tabGeciciGundelikTanim.Kaydet(gt, zorunluYeni);

            if (sonuc.islemSonuc)
            {

                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk tanım bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="gt">Silinecek bilgiyi tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, YollukGundelikTanim gt)
        {
            Sonuc sonuc = new Sonuc();

            //if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            if (gt.yil <= 0)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";

            if (gt.kod.Trim() == string.Empty)
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00211 + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevGundelikTanim tabGeciciGundelikTanim = new TABSurekliGorevGundelikTanim(ve);
            ve.BeginTransaction();

            sonuc = tabGeciciGundelikTanim.Sil(gt);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00299, gt.kod);
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk tanım bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="liste">Kaydedilecek bilgileri YollukGundelikTanim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc ListeKaydet(VeriErisim veriErisim, int yil, ENUMYollukGundelikTuru tur, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevGundelikTanim tabGeciciGundelikTanim = new TABSurekliGorevGundelikTanim(ve);

            ve.BeginTransaction();

            YollukGundelikTanim ygt = new YollukGundelikTanim();
            ygt.yil = yil;
            ygt.tur = tur;
            sonuc = tabGeciciGundelikTanim.Sil(ygt);

            foreach (YollukGundelikTanim gt in liste.objeler)
            {
                sonuc = tabGeciciGundelikTanim.Kaydet(gt, false);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;

        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk tanım bilgisini veritabanından okur 
        /// ve YollukGundelikTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="gt">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk tanım bilgisi listesini YollukGundelikTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, YollukGundelikTanim gt)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevGundelikTanim tabGeciciGundelikTanim = new TABSurekliGorevGundelikTanim(ve);

            ObjectArray bilgi = new ObjectArray();
            IDataReader dr = tabGeciciGundelikTanim.Listele(gt);

            while (dr.Read())
            {
                YollukGundelikTanim gtGelen = new YollukGundelikTanim();

                gtGelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gtGelen.kod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gtGelen.deger = VeriErisim.DegerAl(dr, 2, (double)0);
                gtGelen.ad = VeriErisim.DegerAl(dr, 3, string.Empty);
                gtGelen.tur = (ENUMYollukGundelikTuru)VeriErisim.DegerAl(dr, 4, 0);
                gtGelen.ayKadroDereceBaslangic = VeriErisim.DegerAl(dr, 5, 0);
                gtGelen.ayKadroDereceBitis = VeriErisim.DegerAl(dr, 6, 0);
                gtGelen.ekGostergeBaslangic = VeriErisim.DegerAl(dr, 7, 0);
                gtGelen.ekGostergeBitis = VeriErisim.DegerAl(dr, 8, 0);
                gtGelen.ulke.kod = VeriErisim.DegerAl(dr, 9, 0);
                gtGelen.paraBirimi.kod = VeriErisim.DegerAl(dr, 10, 0);
                gtGelen.paraBirimi.kisaltma = VeriErisim.DegerAl(dr, 11, "");
                gtGelen.ulke.ad = VeriErisim.DegerAl(dr, 12, "");
                gtGelen.paraBirimi.adi = VeriErisim.DegerAl(dr, 13, "");


                bilgi.Ekle(gtGelen);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }

        /*
        public double GundelikDegerAl(Kullanici kullanan, int yil, string gKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurekliGorevGundelikTanim tabGeciciGundelikTanim = new TABSurekliGorevGundelikTanim(ve);

            return tabGeciciGundelikTanim.GundelikDegerAl(yil, gKod);
        }
        */
    }
}

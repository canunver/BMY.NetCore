using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.KPM.IsKurallari.Veri;
using System.Security.Principal;

namespace TNS.KPM.IsKurallari
{
    public class KPMIstek
    {
        public static int Say(VeriErisim veriErisim, Kullanici kullanan)
        {

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKPM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIstek tIstek = new TABIstek(ve);
            Istek istek = new Istek();
            istek.islemYapan = kullanan.kullaniciKodu;
            istek.islemTarih = new TNSDateTime(DateTime.Now.AddMonths(-1));
            IDataReader dr = tIstek.Say(istek);
            int sayIc;
            if (dr == null)
            {
                sayIc = 1000;
            }
            else
            {
                sayIc = 0;
                while (dr.Read())
                {
                    sayIc = VeriErisim.DegerAl(dr, 0, 0);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return sayIc;
        }


        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Istek istek)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKPM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIstek tIstek = new TABIstek(ve);
            ve.BeginTransaction();

            sonuc = tIstek.Kaydet(istek);
            istek.istekNo = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt işlemi gerçekleştirilemedi.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Istek istek)
        {
            ObjectArray istekVAN = new ObjectArray();

            if (!Yetki.OkumaHakkiVarMi(kullanan))
            {
                istekVAN.sonuc.hataStr = "Okuma hakkı yok";
                istekVAN.sonuc.islemSonuc = false;
                return istekVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKPM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIstek tIstek = new TABIstek(ve);

            //Listeleme işleminde, İstek tarihçeden, istek kişiye ait mi diye bakılacak
            //Kişinin onaylama yetkisi varsa, islemYapan boşaltılıp gönderilmeli
            //Aksi takdirde sadece istekte bulunduğu kayıtları görebilecek
            if (Yetki.YoneticiMi(kullanan))
                istek.islemYapan = string.Empty;
            else
                istek.islemYapan = kullanan.kullaniciKodu;

            IDataReader dr = tIstek.Listele(istek);
            while (dr.Read())
            {
                Istek gelen = new Istek();
                gelen.istekNo = VeriErisim.DegerAl(dr, 0, 0);
                gelen.tcKimlikNo = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.istekTur = VeriErisim.DegerAl(dr, 2, 0);
                gelen.adSoyad = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.islemYapan = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.islemTarih = VeriErisim.DegerAl(dr, 5);

                gelen.istekTurAd = TNS.KPM.Istek.IstekTurAdiDondur(gelen.istekTur);

                istekVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (istekVAN.objeler.Count <= 0)
                istekVAN.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";

            ve.VeriErisimKapat();
            return istekVAN;
        }

        public static void KaydetKimlik(VeriErisim ve, Kullanici kullanan, Kisi k)
        {
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriKPM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIstek tIstek = new TABIstek(ve);
            ve.BeginTransaction();

            string veri = TNS.JsonIslem.Yaz(k);
            sonuc = tIstek.KaydetKimlik(k.tcKimlikNo, veri);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt işlemi gerçekleştirilemedi.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
        }

        public static Kisi GetirKimlik(VeriErisim ve, Kullanici kullanan, string mernis)
        {
            Kisi kisi = new Kisi();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriKPM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIstek tIstek = new TABIstek(ve);

            IDataReader dr = tIstek.ListeleKimlik(mernis);
            if (dr != null)
            {
                while (dr.Read())
                {
                    string veri = VeriErisim.DegerAl(dr, 1, "");
                    kisi = TNS.JsonIslem.Oku<Kisi>(veri);
                    try
                    {
                        var obj = TNS.JsonIslem.NesneAyristir(veri);
                        kisi.dogumTarihi = new TNSDateTime(TNS.JsonIslem.Metin(obj["dogumTarihi"]["Gun"]) + "." + TNS.JsonIslem.Metin(obj["dogumTarihi"]["Ay"]) + "." + TNS.JsonIslem.Metin(obj["dogumTarihi"]["Yil"]));
                    }
                    catch { }
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return kisi;
        }
    }
}
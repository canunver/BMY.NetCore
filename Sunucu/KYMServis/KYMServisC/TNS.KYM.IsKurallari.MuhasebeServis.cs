using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM.IsKurallari.Veri;

namespace TNS.KYM.IsKurallari
{
    public class MuhasebeServis
    {
        private Muhasebe DataReaderdanAktar(IDataReader r)
        {
            Muhasebe yt = new Muhasebe();
            yt.kod = VeriErisim.DegerAl(r, 0, "");
            yt.ad = VeriErisim.DegerAl(r, 1, "");

            return yt;
        }

        public ListeVAN Listele(VeriErisim veriErisim)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));
            ListeVAN ytv = new ListeVAN();
            TABMuhasebe tab = new TABMuhasebe(ve);
            IDataReader r = tab.Listele();
            if (tab.Hata() == "")
            {
                while (r.Read())
                {
                    Muhasebe mm = new Muhasebe();

                    mm = DataReaderdanAktar(r);
                    ytv.satirlar.Add(mm);
                }

                ytv.sonuc.islemSonuc = true;
                r.Close();
            }
            else
            {
                ytv.sonuc.islemSonuc = false;
                ytv.sonuc.hataStr = tab.Hata();
            }
            ve.VeriErisimKapat();
            return ytv;
        }

        public MuhasebeVAN Oku(VeriErisim veriErisim, string kod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));
            MuhasebeVAN ytv = new MuhasebeVAN();
            TABMuhasebe tab = new TABMuhasebe(ve);
            IDataReader r = tab.Oku(kod);
            if (tab.Hata() == "")
            {
                if (r.Read())
                {
                    ytv.muhasebe = DataReaderdanAktar(r);
                    ytv.sonuc.islemSonuc = true;
                }
                else
                {
                    ytv.sonuc.islemSonuc = false;
                    ytv.sonuc.hataStr = "Kayıt bulunamadı.";
                }

                r.Close();
            }
            else
            {
                ytv.sonuc.islemSonuc = false;
                ytv.sonuc.hataStr = tab.Hata();
            }
            ve.VeriErisimKapat();
            return ytv;
        }

        public MuhasebeVAN OkuAd(VeriErisim veriErisim, string ad)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));
            MuhasebeVAN ytv = new MuhasebeVAN();
            TABMuhasebe tab = new TABMuhasebe(ve);
            ytv.muhasebe = tab.OkuAd(ad);
            if (tab.Hata() != "")
            {
                ytv.sonuc.islemSonuc = false;
                ytv.sonuc.hataStr = tab.Hata();
            }
            ve.VeriErisimKapat();
            return ytv;
        }

        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Muhasebe muhasebe)
        {
            Sonuc sonuc = new Sonuc(false, "Bu işlemi yapmaya yetkiniz yok!", "");
            if (TNS.KYM.Yetki.YoneticiMi(kullanan).islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
                TABMuhasebe tab = new TABMuhasebe(ve);

                if (muhasebe.ad == "")
                    sonuc.hataStr = "Ad alanı boş geçilemez";
                else
                {
                    ve.BeginTransaction();

                    sonuc = tab.Ekle(muhasebe);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.anahtar = Convert.ToString(muhasebe.kod);
                    }
                    else
                        ve.Rollback();
                }
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, string kod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc sonuc = new Sonuc(false, "Bu işlemi yapmaya yetkiniz yok!", "");

            if (TNS.KYM.Yetki.YoneticiMi(kullanan).islemSonuc)
            {
                TABMuhasebe tab = new TABMuhasebe(ve);
                int kullananSay = tab.KullaniciSayiBul(kod);

                if (tab.Hata() == "")
                {
                    if (kullananSay == 0)
                    {
                        ve.BeginTransaction();
                        sonuc = tab.Sil(kod);
                        if (sonuc.islemSonuc)
                        {
                            ve.Commit();
                            sonuc.anahtar = Convert.ToString(kod);
                        }
                        else
                            ve.Rollback();
                    }
                    else
                        sonuc.hataStr = "Bu Muhasebe kodunu kullanan Kullanıcı bulunmaktadır.";
                }
                else
                    sonuc.hataStr = "Muhasebe koduna sahip kullanıcıların sayısını bulmada hata oldu! (" + tab.Hata() + ")";

                ve.VeriErisimKapat();
            }
            return sonuc;
        }
    }
}


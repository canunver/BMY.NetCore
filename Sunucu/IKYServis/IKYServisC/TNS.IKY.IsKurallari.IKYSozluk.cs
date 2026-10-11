using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYSozluk
    {
        public static string SozlukAdGetir(int kod)
        {
            var list = Listele(new Sozluk { kod = kod });
            if (list != null)
                return ((Sozluk)list[0]).ad;
            else
                return "";
        }

        public static ObjectArray Listele(Sozluk p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));

            TABSozluk tab = new TABSozluk(ve);

            IDataReader r = tab.Listele(p);

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            int sonKolon = ((int)TABSozluk.ENUMAlanlar.HITAPID) + 1;
            while (r.Read())
            {
                Sozluk pp = new Sozluk();

                pp.kod = VeriErisim.DegerAl(r, (int)TABSozluk.ENUMAlanlar.KOD, 0);
                pp.anaKod = VeriErisim.DegerAl(r, (int)TABSozluk.ENUMAlanlar.ANAKOD, 0);
                pp.kayitKod = VeriErisim.DegerAl(r, (int)TABSozluk.ENUMAlanlar.KAYITKOD, "");
                pp.ad = VeriErisim.DegerAl(r, (int)TABSozluk.ENUMAlanlar.KAYITAD, "");
                pp.kisaAd = VeriErisim.DegerAl(r, (int)TABSozluk.ENUMAlanlar.KISAAD, "");
                pp.seviye = VeriErisim.DegerAl(r, (int)TABSozluk.ENUMAlanlar.SEVIYE, 0);
                pp.hitapID = VeriErisim.DegerAl(r, (int)TABSozluk.ENUMAlanlar.HITAPID, "");
                pp.anaKayitAd = VeriErisim.DegerAl(r, sonKolon, "");
                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc Kaydet(Kullanici kullanan, Sozluk p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.ad.Trim() == "")
            {
                sonuc.hataStr = "Kayıt adı boş bırakılamaz";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABSozluk tab = new TABSozluk(ve);

            Sozluk s = KayitKodOlustur(ve, p.anaKayit);

            if (p.kod <= 0)
            {
                p.kod = TABGenel.SayacDegerAlInt(ve, TABSozluk.TABLOAD);
                p.anaKod = s.anaKod;
                p.kayitKod = s.kayitKod;
                p.seviye = s.seviye;
            }

            sonuc = tab.Kaydet(p);

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

        public Sonuc Sil(Kullanici kullanan, Sozluk p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABSozluk tab = new TABSozluk(ve);

            sonuc = tab.Sil(p);

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

        public Sozluk KayitKodOlustur(VeriErisim ve, string anaKayit)
        {
            Sozluk sozluk = new Sozluk();
            string kayitKod = "";
            TABSozluk s = new TABSozluk(ve);
            IDataReader r;
            int sayi;

            if (!string.IsNullOrEmpty(anaKayit))
            {
                string[] arr = anaKayit.Split(':');
                sozluk.anaKod = OrtakFonksiyonlar.ConvertToInt(arr[0], 0);

                string[] arr2;

                r = s.MaxKayitKodGetir(sozluk.anaKod);

                if (r.Read())
                {
                    kayitKod = VeriErisim.DegerAl(r, 0, "");
                    if (!string.IsNullOrEmpty(kayitKod))
                    {
                        arr2 = kayitKod.Split('.');
                        for (int i = 0; i < arr2.Length; i++)
                        {
                            if (i == (arr2.Length - 1))
                            {
                                sayi = OrtakFonksiyonlar.ConvertToInt((arr2[i]), 0);
                                sayi++;
                                arr2[i] = sayi.ToString().PadLeft(5, '0');
                            }
                            sozluk.kayitKod += arr2[i];
                            if (i != (arr2.Length - 1))
                                sozluk.kayitKod += ".";
                        }
                    }
                    else
                        sozluk.kayitKod = arr[1] + ".00001";
                }

                sozluk.seviye = arr[1].Split('.').Length + 1;
            }
            else
            {
                //Ana Kayıt Oluştur
                sozluk.anaKod = 0;
                r = s.MaxKayitKodGetir(0);

                if (r.Read())
                {
                    kayitKod = VeriErisim.DegerAl(r, 0, "");
                    sayi = OrtakFonksiyonlar.ConvertToInt(kayitKod, 0) + 1;
                    sozluk.kayitKod = sayi.ToString().PadLeft(2, '0');
                }
                else
                    sozluk.kayitKod = "01";

                sozluk.seviye = 1;
            }

            r.Close();
            return sozluk;
        }
    }
}

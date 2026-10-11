using System;
using System.Data;
using System.IO;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMIslemDosya
    {
        public static Sonuc Kaydet(VeriErisim ve, IslemDosya bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string tasinirBelgeSaklamaYeri = OrtakFonksiyonlar.WebConfigOku("TasinirBelgeSaklamaYeri", "");
            byte[] icerik = null;
            if (tasinirBelgeSaklamaYeri == "Dosya")
            {
                icerik = bilgi.resim;
                bilgi.resim = null;
            }

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABIslemDosya to = new TABIslemDosya(ve);
            TABGenel tg = new TABGenel(ve);

            if (bilgi.siraNo == 0)
                bilgi.siraNo = to.SiraNoVer(bilgi);

            if (bilgi.dosyaKod == 0)
            {
                bilgi.dosyaKod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("DOSYAKOD", "0", "10000"), 0);
                sonuc = to.Kaydet(bilgi);
            }
            else
            {
                sonuc = to.Guncelle(bilgi);
            }

            if (sonuc.islemSonuc)
            {
                if (tasinirBelgeSaklamaYeri == "Dosya")
                {
                    string dosya = Path.Combine(bilgi.kayitEdilecekDosyaYol, bilgi.yil.ToString());
                    if (!Directory.Exists(dosya)) Directory.CreateDirectory(dosya);
                    dosya = Path.Combine(dosya, bilgi.yil + "_" + bilgi.dosyaKod);
                    try
                    {
                        if (File.Exists(dosya)) File.Delete(dosya);
                    }
                    catch { }

                    File.WriteAllBytes(dosya, icerik);
                }

                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Getir(VeriErisim ve, IslemDosya bilgi)
        {
            ObjectArray liste = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABIslemDosya to = new TABIslemDosya(ve);
            IDataReader dr = null;

            try
            {
                dr = to.Getir(bilgi);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            if (dr != null)
            {
                while (dr.Read())
                {
                    IslemDosya r = new IslemDosya();
                    r.dosyaKod = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.DOSYAID, 0);
                    r.yil = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.YIL, 0);
                    r.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.MUHASEBEKOD, "");
                    r.harcamaBirimKod = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.HARCAMABIRIMKOD, "");
                    r.fisNo = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.FISNO, "");
                    r.siraNo = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.SIRANO, 0);
                    if (bilgi.dosyaKod == 0)
                        r.resim = new byte[0];
                    else
                    {
                        try
                        {
                            r.resim = (byte[])dr.GetValue((int)TABIslemDosya.ENUMAlanlar.RESIM);
                        }
                        catch { }
                    }
                    r.adi = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.ADI, "");
                    r.boyutu = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.BOYUTU, 0.0);
                    r.ekleyenKisiAdi = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.EKLEYENKISIAD, "");
                    r.ekleyenKisiKod = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.EKLEYENKISIKOD, "");
                    r.eklemeTarihi = VeriErisim.DegerAl(dr, (int)TABIslemDosya.ENUMAlanlar.EKLEMETARIHI);
                    liste.Ekle(r);
                }
            }
            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc Sil(VeriErisim ve, IslemDosya bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABIslemDosya to = new TABIslemDosya(ve);

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                string dosya = Path.Combine(bilgi.kayitEdilecekDosyaYol, bilgi.yil.ToString());
                dosya = Path.Combine(dosya, bilgi.yil + "_" + bilgi.dosyaKod);
                try
                {
                    if (File.Exists(dosya)) File.Delete(dosya);
                }
                catch { }

                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}

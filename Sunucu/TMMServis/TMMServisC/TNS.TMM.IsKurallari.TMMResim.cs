using System;
using System.Data;
using System.IO;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMResim
    {
        public static Sonuc Kaydet(VeriErisim ve, TasResim resim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string tasinirBelgeSaklamaYeri = OrtakFonksiyonlar.WebConfigOku("TasinirBelgeSaklamaYeri", "");
            byte[] icerik = null;
            if (tasinirBelgeSaklamaYeri == "Dosya")
            {
                icerik = resim.resim;
                resim.resim = null;
            }

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABTasinirResim to = new TABTasinirResim(ve);
            TABGenel tg = new TABGenel(ve);

            if (resim.siraNo == 0)
                resim.siraNo = to.SiraNoVer(resim);

            if (resim.resimID == 0)
            {
                resim.resimID = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("DOSYAKOD", "0", "10000"), 0);
                sonuc = to.Kaydet(resim);
            }
            else
                sonuc = to.Guncelle(resim);

            if (sonuc.islemSonuc)
            {
                if (tasinirBelgeSaklamaYeri == "Dosya")
                {
                    string dosya = Path.Combine(resim.kayitEdilecekDosyaYol, "RESIM");
                    if (!Directory.Exists(dosya)) Directory.CreateDirectory(dosya);
                    dosya = Path.Combine(dosya, "RESIM" + "_" + resim.resimID);
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

        public static ObjectArray Getir(VeriErisim ve, int prSicilNo, string resimID)
        {
            ObjectArray liste = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABTasinirResim to = new TABTasinirResim(ve);
            IDataReader dr = null;

            try
            {
                dr = to.Getir(prSicilNo, resimID);
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
                    TasResim r = new TasResim();
                    r.resimID = VeriErisim.DegerAl(dr, (int)TABTasinirResim.ENUMAlanlar.RESIMID, 0);
                    r.prSicilNo = VeriErisim.DegerAl(dr, (int)TABTasinirResim.ENUMAlanlar.PRSICILNO, 1);
                    r.siraNo = VeriErisim.DegerAl(dr, (int)TABTasinirResim.ENUMAlanlar.PRSICILNO, 2);
                    if (string.IsNullOrEmpty(resimID))
                        r.resim = new byte[0];
                    else
                    {
                        try
                        {
                            r.resim = (byte[])dr.GetValue((int)TABTasinirResim.ENUMAlanlar.RESIM);
                        }
                        catch { }
                    }

                    r.adi = VeriErisim.DegerAl(dr, (int)TABTasinirResim.ENUMAlanlar.ADI, "");
                    r.boyutu = VeriErisim.DegerAl(dr, (int)TABTasinirResim.ENUMAlanlar.BOYUTU, 0.0);
                    r.ekleyenKisiAdi = VeriErisim.DegerAl(dr, (int)TABTasinirResim.ENUMAlanlar.EKLEYENKISIAD, "");
                    r.ekleyenKisiKod = VeriErisim.DegerAl(dr, (int)TABTasinirResim.ENUMAlanlar.EKLEYENKISIKOD, "");
                    r.eklemeTarihi = VeriErisim.DegerAl(dr, (int)TABTasinirResim.ENUMAlanlar.EKLEMETARIHI);
                    liste.Ekle(r);
                }
            }
            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc Sil(VeriErisim ve, TasResim resim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABTasinirResim to = new TABTasinirResim(ve);

            sonuc = to.Sil(resim);

            if (sonuc.islemSonuc)
            {
                string dosya = Path.Combine(resim.kayitEdilecekDosyaYol, "RESIM");
                dosya = Path.Combine(dosya, "RESIM" + "_" + resim.resimID);
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

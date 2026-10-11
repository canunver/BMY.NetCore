using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELTasinmazCins
    {
        public static string VeriKontrol(TasinmazCins bilgi, ENUMIslemTuru islem)
        {
            StringBuilder hatalar = new StringBuilder();
            if (islem != ENUMIslemTuru.LISTELEME)
            {
                if (string.IsNullOrEmpty(bilgi.kod))
                    hatalar.Append("Taşınmaz cinsi kodu boş olmamalıdır<br/>");
                if (islem != ENUMIslemTuru.SILME)
                {
                    if (string.IsNullOrEmpty(bilgi.ad))
                        hatalar.Append("Taşınmaz cinsi adı boş olmamalıdır<br/>");
                }
            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasinmazCins[] bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string hata = "";
            StringBuilder hatalar = new StringBuilder();
            foreach (TasinmazCins item in bilgiler)
            {
                hata = VeriKontrol(item, ENUMIslemTuru.KAYIT);
                if (hata.Trim() != "")
                {
                    sonuc.DegerAta(hata);
                    return sonuc;
                }
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            ve.BeginTransaction();

            try
            {
                bool kayitVar = false;
                foreach (TasinmazCins item in bilgiler)
                {
                    TABTasinmazCins TABCins = new TABTasinmazCins(ve);
                    kayitVar = TABCins.KayitVarmi(item.kod);
                    if (kayitVar)
                        sonuc = TABCins.Guncelle(item);
                    else
                        sonuc = TABCins.Kaydet(item);

                    if (!sonuc.islemSonuc)
                        break;

                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "İşlem Başarıyla Gerçekleştirildi";
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

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasinmazCins bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            sonuc.DegerAta(VeriKontrol(bilgi, ENUMIslemTuru.KAYIT));
            if (sonuc.hataStr == "")
            {
                sonuc.islemSonuc = true;
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                ve.BeginTransaction();
                TABTasinmazCins TABCins = new TABTasinmazCins(ve);
                TABCins.Sil(bilgi.kod);
                if (ve.hata.Trim() == "")
                {
                    sonuc = TABCins.Kaydet(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = "İşlem Başarıyla Gerçekleştirildi";
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                else
                {
                    sonuc.DegerAta(ve.hata);
                    ve.Rollback();
                }

                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, string kod)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            sonuc.DegerAta(VeriKontrol(new TasinmazCins(kod, ""), ENUMIslemTuru.SILME));
            if (sonuc.hataStr == "")
            {
                sonuc.islemSonuc = true;
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                ve.BeginTransaction();
                TABTasinmazCins TABCins = new TABTasinmazCins(ve);
                TABCins.Sil(kod);
                if (ve.hata.Trim() == "")
                {
                    ve.Commit();
                    sonuc.bilgiStr = "İşlem Başarıyla Gerçekleştirildi";
                }
                else
                {
                    sonuc.DegerAta(ve.hata);
                    ve.Rollback();
                }

                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TasinmazCins bilgi)
        {
            ObjectArray sonuclar = new ObjectArray();
            sonuclar.sonuc.DegerAta(VeriKontrol(bilgi, ENUMIslemTuru.LISTELEME));
            if (sonuclar.sonuc.hataStr == "")
            {
                sonuclar.sonuc.islemSonuc = true;
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                ve.BeginTransaction();
                TABTasinmazCins TABCins = new TABTasinmazCins(ve);
                IDataReader dr = null;
                try
                {
                    dr = TABCins.Listele(bilgi);
                    while (dr.Read())
                    {
                        TasinmazCins cins = new TasinmazCins();
                        cins.kod = VeriErisim.DegerAl(dr, 0, "");
                        cins.kod = GELOrtak.KodNoktala(cins.kod);
                        cins.ad = VeriErisim.DegerAl(dr, 1, "");
                        sonuclar.objeler.Add(cins);
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
                    sonuclar.sonuc.DegerAta(ex.ToString());
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
                ve.VeriErisimKapat();
            }
            return sonuclar;
        }
    }
}

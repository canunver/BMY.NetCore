using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.KYM;
using TNS.Veri;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELTasinmazBolge
    {
        public static string VeriKontrol(Bolge bilgi, ENUMIslemTuru islemTuru)
        {
            StringBuilder hatalar = new StringBuilder();
            if (islemTuru == ENUMIslemTuru.KAYIT)
            {
                if (string.IsNullOrEmpty(bilgi.ad))
                {
                    hatalar.Append("Bölgenin adı boş olamaz" + "</br>");
                }
            }
            else if (islemTuru == ENUMIslemTuru.SILME)
            {
                if (bilgi.kod <= 0)
                {
                    hatalar.Append("Bölgenin kodu boş olamaz" + "</br>");
                }
            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Bolge bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string hata = VeriKontrol(bilgi, ENUMIslemTuru.KAYIT);
            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazBolge tabBolge = new TABTasinmazBolge(ve);
            try
            {
                ve.BeginTransaction();
                if (tabBolge.BolgeKayitVarmi(bilgi))
                {
                    sonuc = tabBolge.Guncelle(bilgi);
                }
                else
                {
                    int siraNo = tabBolge.BolgeSiraNoAl();
                    bilgi.kod = siraNo;
                    sonuc = tabBolge.Kaydet(bilgi);
                }

                if (sonuc.islemSonuc == true)
                {
                    sonuc.bilgiStr = "İşlem başarıyla gerçekleştirildi";
                    ve.Commit();
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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Bolge bilgi)
        {

            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazBolge tabBolge = new TABTasinmazBolge(ve);
            IDataReader dr = null;
            try
            {
                ve.BeginTransaction();
                dr = tabBolge.Listele(bilgi);
                while (dr.Read())
                {
                    Bolge tasinmazbolge = new Bolge();
                    tasinmazbolge.kod = VeriErisim.DegerAl(dr, 0, 0);
                    tasinmazbolge.ad = VeriErisim.DegerAl(dr, 1, "");
                    tasinmazbolge.ad2 = VeriErisim.DegerAl(dr, 2, "");
                    donusDeger.objeler.Add(tasinmazbolge);
                }
                dr.Close();
            }
            catch (Exception exp)
            {
                donusDeger.sonuc.DegerAta(exp.ToString());
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }
                ve.VeriErisimKapat();
            }
            return donusDeger;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string hata = "";
            foreach (Bolge item in bilgiler)
            {
                hata = VeriKontrol(item, ENUMIslemTuru.SILME);
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.DegerAta(hata);
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazBolge tabBolge = new TABTasinmazBolge(ve);
            try
            {
                ve.BeginTransaction();
                foreach (Bolge item in bilgiler)
                {
                    sonuc = tabBolge.Sil(item.kod);
                    if (!sonuc.islemSonuc)
                    {
                        break;
                    }
                }
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "İşlem Başarıyla Gerçekleştirildi" + "</br>";
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
    }
}

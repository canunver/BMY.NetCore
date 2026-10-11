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
    public class GELTasinmazEkDosya
    {
        public static string VeriKontrol(TasinmazEkDosya dosya, ENUMIslemTuru islemTuru)
        {
            StringBuilder hatalar = new StringBuilder();
            if (string.IsNullOrEmpty(dosya.tasinmazKod))
                hatalar.Append("Dosya taşınmaz kodu boş olmamalıdır!" + "</br>");
            if (dosya.tasinmazYil <= 0)
                hatalar.Append("Dosya taşınmaz yılı boş olmamalıdır!" + "</br>");

            if (islemTuru == ENUMIslemTuru.KAYIT || islemTuru == ENUMIslemTuru.SILME)
            {
                if (string.IsNullOrEmpty(dosya.dosyaYolu))
                    hatalar.Append("Dosya yolu boş olmamalıdır!" + "<br/>");

            }
            if (islemTuru == ENUMIslemTuru.SILME)
            {
                if (dosya.siraNo <= 0)
                    hatalar.Append("Dosya sıra numarası sıfırda büyük olmalıdır!" + "<br/>");
            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasinmazEkDosya dosya)
        {
            Sonuc sonuc = new Sonuc();
            string hata = VeriKontrol(dosya, ENUMIslemTuru.KAYIT);
            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            try
            {
                TABTasinmazEkDosya tabDosya = new TABTasinmazEkDosya(ve);
                ve.BeginTransaction();


                if (dosya.siraNo <= 0)
                    dosya.siraNo = tabDosya.SiraNoAl(dosya);

                sonuc = tabDosya.Kaydet(dosya);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00019;
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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TasinmazEkDosya dosya)
        {
            Sonuc sonuc = new Sonuc();
            string hata = VeriKontrol(dosya, ENUMIslemTuru.SILME);
            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazEkDosya tabDosya = new TABTasinmazEkDosya(ve);
            ve.BeginTransaction();
            try
            {
                tabDosya.Sil(dosya);
                if (ve.hata.Trim() == "")
                {
                    ve.Commit();
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00119;
                }
                else
                {
                    ve.Rollback();
                    sonuc.DegerAta(ve.hata);
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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TasinmazEkDosya dosya)
        {
            ObjectArray donusdeger = new ObjectArray();
            string hata = VeriKontrol(dosya, ENUMIslemTuru.LISTELEME);
            if (hata != "")
            {
                donusdeger.sonuc.DegerAta(hata);
                return donusdeger;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            IDataReader dr = null;
            try
            {
                TABTasinmazEkDosya tabDosya = new TABTasinmazEkDosya(ve);
                ve.BeginTransaction();
                dr = tabDosya.Listele(dosya);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        TasinmazEkDosya ekDosya = new TasinmazEkDosya();
                        ekDosya.tasinmazYil = VeriErisim.DegerAl(dr, 0, 0);
                        ekDosya.tasinmazKod = VeriErisim.DegerAl(dr, 1, "");
                        ekDosya.siraNo = VeriErisim.DegerAl(dr, 2, 0);
                        ekDosya.aciklama = VeriErisim.DegerAl(dr, 3, "");
                        ekDosya.dosyaYolu = VeriErisim.DegerAl(dr, 4, "");
                        donusdeger.objeler.Add(ekDosya);
                    }
                }

            }
            catch (Exception ex)
            {
                donusdeger.sonuc.DegerAta(ex.ToString());
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                    dr.Close();
                ve.VeriErisimKapat();
            }
            return donusdeger;
        }
    }
}

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
    public class GELTasinmazTalepEkDosya
    {

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TasinmazTalepEki bilgi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazTalepEkDosya tabTasinmazTalepEki = new TABTasinmazTalepEkDosya(ve);
            ObjectArray tasinmazTalepEkListesi = new ObjectArray();
            ve.BeginTransaction();
            IDataReader dr = tabTasinmazTalepEki.Listele(bilgi);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TasinmazTalepEki ttalepEk = new TasinmazTalepEki();
                    ttalepEk.tasinmazYil = VeriErisim.DegerAl(dr, 0, 0);
                    ttalepEk.tasinmazKod = VeriErisim.DegerAl(dr, 1, "");
                    ttalepEk.firmaKodu = VeriErisim.DegerAl(dr, 2, 0);
                    ttalepEk.talepSiraNo = VeriErisim.DegerAl(dr, 3, 0);
                    ttalepEk.ekNo = VeriErisim.DegerAl(dr, 4, 0);
                    ttalepEk.aciklama = VeriErisim.DegerAl(dr, 5, "");
                    ttalepEk.dosyaYolu = VeriErisim.DegerAl(dr, 6, "");
                    tasinmazTalepEkListesi.objeler.Add(ttalepEk);
                }
            }
            ve.VeriErisimKapat();
            return tasinmazTalepEkListesi;

        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasinmazTalepEki bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            if (bilgi.tasinmazYil <= 0)
                hatalar.Append("Taşınmaz yılı sıfırdan büyük olmalıdır" + "<br/>");

            if (string.IsNullOrEmpty(bilgi.tasinmazKod))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00015 + "<br/>");

            if (bilgi.firmaKodu <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00016 + "<br/>");

            if (bilgi.talepSiraNo <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00017 + "<br/>");

            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazTalepEkDosya tabTalepEk = new TABTasinmazTalepEkDosya(ve);
            try
            {
                ve.BeginTransaction();

                if (bilgi.ekNo <= 0)
                    bilgi.ekNo = tabTalepEk.TalepTasinmazEkiSiraNoAl(bilgi);

                sonuc = tabTalepEk.Kaydet(bilgi);
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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TasinmazTalepEki bilgi, bool ekNoKontrol = true)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            StringBuilder hatalar=new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append( TNS.GEL.GelirSurecleriResI.GEL00003 +"<br/>");
            
            if (string.IsNullOrEmpty(bilgi.tasinmazKod))
                 hatalar.Append( TNS.GEL.GelirSurecleriResI.GEL00015 +"<br/>");
            
            if (bilgi.firmaKodu <= 0)
                 hatalar.Append( TNS.GEL.GelirSurecleriResI.GEL00016 +"<br/>");
               
            if (bilgi.talepSiraNo <= 0)
                  hatalar.Append( TNS.GEL.GelirSurecleriResI.GEL00017 +"<br/>");
                
            if (ekNoKontrol)
            {
                if (bilgi.ekNo <= 0)
                  hatalar.Append( TNS.GEL.GelirSurecleriResI.GEL00018 +"<br/>");

                if(string.IsNullOrEmpty(bilgi.dosyaYolu))
                    hatalar.Append("Ek dosya yolu boş olmamalıdır" + "<br/>");

            }
            if (hatalar.ToString()!="")
            {
                sonuc.DegerAta(hatalar.ToString());
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazTalepEkDosya tabTalepEki = new TABTasinmazTalepEkDosya(ve);
            ve.BeginTransaction();
            try
            {
                tabTalepEki.Sil(bilgi);
                if (ve.hata.Trim() == "")
                {
                    ve.Commit();
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00020;
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
            finally {
                ve.VeriErisimKapat();
            }
            return sonuc;
        }


    }
}

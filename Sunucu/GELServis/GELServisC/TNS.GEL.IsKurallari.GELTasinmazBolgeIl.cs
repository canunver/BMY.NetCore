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
    public class GELTasinmazBolgeIl
    {
        public static string VeriKontrol(Bolge bolge, ENUMIslemTuru islemTuru)
        {
            StringBuilder hatalar = new StringBuilder();
            if (bolge.kod <= 0)
                hatalar.Append("Bölgenin kodu boş brakılamaz" + "</br>");

            if (islemTuru == ENUMIslemTuru.KAYIT || islemTuru == ENUMIslemTuru.SILME)
            {
                if (bolge.iller.Count == 0 && islemTuru == ENUMIslemTuru.KAYIT)
                    hatalar.Append("Bölgeye ait enaz bir il olmalıdır" + "<br/>");

                if (bolge.iller.Count == 0 && islemTuru == ENUMIslemTuru.SILME)
                    hatalar.Append("Silmek için enaz bir il seçmelisiniz" + "<br/>");

                foreach (BolgeIl il in bolge.iller)
                {
                    if (string.IsNullOrEmpty(il.kod) || string.IsNullOrEmpty(il.ad))
                    {
                        hatalar.Append("İlin kodu ve adı boş bırakılamaz" + "</br>");
                        break;
                    }
                }
            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Bolge bolge)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string hata = VeriKontrol(bolge, ENUMIslemTuru.KAYIT);
            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazBolgeIl tabBolgeIl = new TABTasinmazBolgeIl(ve);
            try
            {
                ve.BeginTransaction();
                sonuc = tabBolgeIl.Sil(bolge.kod, "");
                if (ve.hata == "")
                {
                    foreach (BolgeIl il in bolge.iller)
                    {
                        sonuc = tabBolgeIl.Kaydet(bolge, il);
                        if (!sonuc.islemSonuc)
                        {
                            break;
                        }
                    }
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = "İşlem Başarıyla Gerçekleştirildi" + "<br/>";
                    }
                    else
                    {
                        ve.Rollback();
                    }
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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Bolge bolge)
        {

            ObjectArray donusDegeri = new ObjectArray();
            string hata = VeriKontrol(bolge, ENUMIslemTuru.LISTELEME);
            if (hata != "")
            {
                donusDegeri.sonuc.DegerAta(hata);
                return donusDegeri;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazBolgeIl tabBolgeIl = new TABTasinmazBolgeIl(ve);
            IDataReader dr = null;
            try
            {
                ve.BeginTransaction();
                dr = tabBolgeIl.Listele(bolge.kod);
                if (dr != null)
                {
                    Bolge tasinmazbolge = new Bolge();
                    tasinmazbolge.kod = bolge.kod;
                    while (dr.Read())
                    {
                        BolgeIl il = new BolgeIl();
                        il.kod = VeriErisim.DegerAl(dr, 1, "");
                        il.ad = VeriErisim.DegerAl(dr, 2, "");
                        tasinmazbolge.iller.Add(il);
                    }
                    donusDegeri.objeler.Add(tasinmazbolge);
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                donusDegeri.sonuc.DegerAta(ex.ToString());
            }
            finally
            {
                if (dr != null && !dr.IsClosed) dr.Close();
                ve.VeriErisimKapat();
            }

            return donusDegeri;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Bolge bolge)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string hata = VeriKontrol(bolge, ENUMIslemTuru.KAYIT);
            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazBolgeIl tabBolgeIl = new TABTasinmazBolgeIl(ve);
            try
            {
                ve.BeginTransaction();
                foreach (BolgeIl il in bolge.iller)
                {
                    sonuc = tabBolgeIl.Sil(bolge.kod, il.kod);
                    if (ve.hata.Trim() != "" || !sonuc.islemSonuc)
                    {
                        break;
                    }
                }

                if (ve.hata.Trim() == "" && sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "İşlem başarıyla gerçekleştirildi" + "<br/>";
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
    }
}

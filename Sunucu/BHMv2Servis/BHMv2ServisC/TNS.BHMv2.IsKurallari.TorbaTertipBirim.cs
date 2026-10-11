using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.BHMv2.IsKurallari.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari
{
    public class TorbaTertipBirimOdenekIs
    {

        public enum ENUMIslemTuru
        {
            KAYIT = 1,
            LISTELEME = 2,
            SILME = 3,
            GUNCELLEME = 4
        }
        public static string VeriKontrol(TorbaTertipBirimOdenek ttbo, ENUMIslemTuru islem)
        {
            StringBuilder hatalar = new StringBuilder();
            if (ttbo.yil <= 0)
                hatalar.Append("Yıl Boş Olmamalıdır <br/>");

            if (islem == ENUMIslemTuru.KAYIT)
            {
                if (ttbo.torbatertipler.Count == 0 && ttbo.birimler.Count == 0)
                    hatalar.Append("En az bir tertip ve birim kaydı girmelisiniz");

                decimal tertiptoplamlari = 0;
                decimal birimtoplmlari = 0;
                foreach (TorbaTertip item in ttbo.torbatertipler)
                {
                    tertiptoplamlari += (decimal)item.miktar;
                    if (string.IsNullOrEmpty(item.kodlar[0]))
                    {
                        hatalar.Append("Bütün tertip kodları dolu olmalıdır<br/>");
                        break;
                    }
                }

                foreach (MuhasebeBirim item in ttbo.birimler)
                {
                    birimtoplmlari += (decimal)item.miktar;
                    if (string.IsNullOrEmpty(item.birimKod) || item.muhasebeKod <= 0 || string.IsNullOrEmpty(item.kurkod))
                    {
                        hatalar.Append("Bütün birimlerin muhasebe,birim ve kurumsal kodları dolu olmalıdır<br/>");
                        break;
                    }
                }

                if (tertiptoplamlari < birimtoplmlari)
                    hatalar.Append("Birim miktarları toplamı, tertip miktarları toplamından büyük olmamalıdır<br/>");
            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim verierisim, Kullanici kullanan, TorbaTertipBirimOdenek ttbo)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.hataStr = VeriKontrol(ttbo, ENUMIslemTuru.KAYIT);
            if (sonuc.hataStr.Trim() != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(verierisim, BHMv2Genel.VeriErisimAdBul(ttbo.yil), kullanan.KimlikBul());
            ve.BeginTransaction();
            TABTORBATERTIPBIRIMODENEK TABttbo = new TABTORBATERTIPBIRIMODENEK(ve);
            TABttbo.BirimSil(ttbo.yil, 0, "");
            if (ve.hata.Trim() == "")
            {
                TABttbo.TertipSil(ttbo.yil, "");
                if (ve.hata.Trim() == "")
                {
                    sonuc = TABttbo.BirimKaydet(ttbo);
                    if (sonuc.islemSonuc)
                    {
                        sonuc = TABttbo.TertipKaydet(ttbo);
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
                        ve.Rollback();
                    }
                }
                else
                {
                    ve.Rollback();
                }
            }
            else
            {
                ve.Rollback();
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim verierisim, Kullanici kullanan, TorbaTertipBirimOdenek ttbo)
        {
            ObjectArray sonuclar = new ObjectArray();
            sonuclar.sonuc.hataStr = VeriKontrol(ttbo, ENUMIslemTuru.LISTELEME);
            if (sonuclar.sonuc.hataStr.Trim() != string.Empty)
            {
                sonuclar.sonuc.islemSonuc = false;
                return sonuclar;
            }

            UZY.IUZYServis servisUZY = UZY.Arac.Tanimla();

            VeriErisim ve = VeriErisim.VeriErisimBul(verierisim, BHMv2Genel.VeriErisimAdBul(ttbo.yil), kullanan.KimlikBul());
            ve.BeginTransaction();
            TABTORBATERTIPBIRIMODENEK TABttbo = new TABTORBATERTIPBIRIMODENEK(ve);
            IDataReader dr = null;

            TorbaTertipBirimOdenek donusTTBO = new TorbaTertipBirimOdenek();
            try
            {
                dr = TABttbo.TertipListele(ttbo.yil, null);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        TorbaTertip tt = new TorbaTertip(ttbo.yil, TNS.BKD.ENUMTabloKod.TABPEBOdenek);
                        tt.kodlar = new string[5];
                        tt.kodlar[0] = VeriErisim.DegerAl(dr, 1, "");
                        tt.kodlar[1] = VeriErisim.DegerAl(dr, 2, "");
                        tt.kodlar[2] = VeriErisim.DegerAl(dr, 3, "");
                        tt.kodlar[3] = VeriErisim.DegerAl(dr, 4, "");
                        tt.kodlar[4] = VeriErisim.DegerAl(dr, 5, "");
                        tt.Noktala();
                        tt.AdlariBul(servisUZY);
                        tt.miktar = VeriErisim.DegerAl(dr, 6, 0.0);
                        tt.tertipAdi = VeriErisim.DegerAl(dr, 7, "");
                        donusTTBO.torbatertipler.Add(tt);
                    }
                    dr.Close();
                }

                dr = TABttbo.BirimListele(ttbo.yil, 0, "");
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        MuhasebeBirim mb = new MuhasebeBirim();
                        mb.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                        mb.muhasebeAd = VeriErisim.DegerAl(dr, 2, "");
                        mb.birimKod = VeriErisim.DegerAl(dr, 3, "");
                        mb.birimAd = VeriErisim.DegerAl(dr, 4, "");
                        mb.kurkod = VeriErisim.DegerAl(dr, 5, "");
                        mb.miktar = VeriErisim.DegerAl(dr, 6, 0.0);
                        donusTTBO.birimler.Add(mb);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                sonuclar.sonuc.islemSonuc = false;
                sonuclar.sonuc.hataStr = ex.ToString();
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                    dr.Close();
            }
            sonuclar.objeler.Add(donusTTBO);

            ve.VeriErisimKapat();
            return sonuclar;
        }
    }
}

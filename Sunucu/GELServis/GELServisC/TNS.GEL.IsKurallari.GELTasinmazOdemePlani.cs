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
    public class GELTasinmazOdemePlani
    {
        private static string VeriKontrol(TasinmazOdemePlan bilgi, ENUMIslemTuru islemtur)
        {
            StringBuilder hatalar = new StringBuilder();
            if (string.IsNullOrEmpty(bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu))
                hatalar.Append("Taşınmaz kodu boş olmamalıdır<br/>");
            if (bilgi.tasinmaz.tasinmazBilgisi.yil <= 0)
                hatalar.Append("Taşınmaz yılı boş olmamalıdır<br/>");
            if (islemtur == ENUMIslemTuru.KAYIT)
            {
                if (bilgi.firma.firmaBilgisi.kod <= 0)
                    hatalar.Append("Firma bilgisi boş olmamalıdır<br/>");
                if (bilgi.siraNo <= 0)
                    hatalar.Append("Ödeme kaydı sıra numarası boş olmamalıdır<br/>");
                if (bilgi.odemeTarihi == null)
                    hatalar.Append("Ödeme tarihi boş olmamalıdır<br/>");
                if (bilgi.odemeTutari <= 0)
                    hatalar.Append("Ödeme tutarı boş olmamalıdır<br/>");
            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasinmazOdemePlan[] bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string hatalar = "";
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.DegerAta(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
                return sonuc;
            }
            int siraNo = 0;
            string hata = "";
            if (bilgiler.Length > 0)
            {
                foreach (TasinmazOdemePlan bilgi in bilgiler)
                {
                    siraNo++;
                    hata = VeriKontrol(bilgi, ENUMIslemTuru.KAYIT);
                    if (hata != "") hata = "Sıra No: " + siraNo + " " + hata;
                    hatalar += hata;

                }
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Kaydedilecek veri bulunamamaktadır! ";
                return sonuc;
            }

            sonuc.DegerAta(hatalar);
            if (sonuc.hataStr == "")
            {
                sonuc.islemSonuc = true;
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                ve.BeginTransaction();
                TABTasinmazOdemePlani TABOp = new TABTasinmazOdemePlani(ve);
                foreach (TasinmazOdemePlan bilgi in bilgiler)
                {
                    TABOp.Sil(bilgi);
                    if (ve.hata.Trim() == "")
                    {
                        if (!sonuc.islemSonuc) break;
                        sonuc = TABOp.Kaydet(bilgi);
                    }
                    else
                    {
                        sonuc.DegerAta(ve.hata);
                        ve.Rollback();
                    }
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
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TasinmazOdemePlan bilgi)
        {
            ObjectArray sonuclar = new ObjectArray();
            sonuclar.sonuc.DegerAta(VeriKontrol(bilgi, ENUMIslemTuru.LISTELEME));
            if (sonuclar.sonuc.hataStr == "")
            {
                sonuclar.sonuc.islemSonuc = true;
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                ve.BeginTransaction();
                TABTasinmazOdemePlani TABOp = new TABTasinmazOdemePlani(ve);
                IDataReader dr = null;
                try
                {
                    dr = TABOp.Listele(bilgi);
                    while (dr.Read())
                    {
                        TasinmazOdemePlan op = new TasinmazOdemePlan();
                        op.tasinmaz.tasinmazBilgisi.yil = VeriErisim.DegerAl(dr, 0, 0);
                        op.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 1, "");
                        op.siraNo = VeriErisim.DegerAl(dr, 2, 0);
                        op.odemeTarihi = VeriErisim.DegerAl(dr, 3);
                        op.firma.firmaBilgisi.kod = VeriErisim.DegerAl(dr, 4, 0);
                        op.odemeTutari = VeriErisim.DegerAl(dr, 5, 0.0);
                        op.makbuzno = VeriErisim.DegerAl(dr, 6, "");
                        op.tasinmaz.tasinmazBilgisi.tasinmazAd = VeriErisim.DegerAl(dr, 7, "");
                        sonuclar.objeler.Add(op);
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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TasinmazOdemePlan bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.DegerAta(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
                return sonuc;
            }

            sonuc.DegerAta(VeriKontrol(bilgi, ENUMIslemTuru.SILME));
            if (sonuc.hataStr == "")
            {
                sonuc.islemSonuc = true;
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                ve.BeginTransaction();
                TABTasinmazOdemePlani TABOp = new TABTasinmazOdemePlani(ve);
                TABOp.Sil(bilgi);
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

        public static ObjectArray TasinmazaAitFirmaKisiListele(VeriErisim veriErisim, Kullanici kullanan, string tasinmazSistemKod, int tasinmazYili)
        {
            ObjectArray sonuclar = new ObjectArray();
            if (string.IsNullOrEmpty(tasinmazSistemKod) ||tasinmazYili<=0)
            {
                sonuclar.sonuc.DegerAta("Taşınmaz sistem kodu ve yılı dolu olmalıdır<br/>");
                return sonuclar;
            }
            sonuclar.sonuc.islemSonuc = true;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            ve.BeginTransaction();
            TABTasinmazOdemePlani TABOp = new TABTasinmazOdemePlani(ve);
            IDataReader dr = null;
            try
            {
                dr = TABOp.FirmaKisiListele(tasinmazSistemKod, tasinmazYili);
                while (dr.Read())
                {
                    TasinmazOdemePlan op = new TasinmazOdemePlan();
                    op.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 0, "");
                    op.firma.firmaBilgisi.kod = VeriErisim.DegerAl(dr, 1, 0);
                    op.firma.firmaBilgisi.ad = VeriErisim.DegerAl(dr, 2, "");
                    sonuclar.objeler.Add(op);
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
            return sonuclar;
        }
    }
}

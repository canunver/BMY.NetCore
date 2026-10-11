using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;
using System.Collections;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace TNS.TMM.IsKurallari
{
    public static class TMMEBYSCumhurbaskanligiBirim
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EBYSCumhurbaskanligiBirim form)
        {
            string hata = "";
            if (form.kod <= 0)
                hata += "Kod bilgisi boş olamaz." + "<br>";
            if (form.ad == "")
                hata += "Ad boş olamaz." + "<br>";
            if (form.muhasebeKod == "")
                hata += "Muhasebe Kodu boş olamaz." + "<br>";
            if (form.harcamaKod == "")
                hata += "Harcama Birim Kodu boş olamaz." + "<br>";

            if (hata != "")
                return new Sonuc(false, hata, "");


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABEBYSCumhurbaskanligiBirim to = new TABEBYSCumhurbaskanligiBirim(ve);

            ve.BeginTransaction();

            Sonuc sonuc = to.Kaydet(form);

            if (!sonuc.islemSonuc)
                ve.Rollback();
            else
            {
                sonuc.bilgiStr = "Kayıt işlemi başarıyla gerçekleştirildi.";
                ve.Commit();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EBYSCumhurbaskanligiBirim form)
        {
            ObjectArray bilgi = new ObjectArray();

            string hata = "";
            if (form.muhasebeKod == "")
                hata += "Muhasebe Kodu giriniz." + "<br>";
            if (form.harcamaKod == "")
                hata += "Harcama Birim Kodu giriniz." + "<br>";

            if (hata != "")
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = hata;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                TABEBYSCumhurbaskanligiBirim to = new TABEBYSCumhurbaskanligiBirim(ve);

                IDataReader dr = to.Listele(form);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        EBYSCumhurbaskanligiBirim ebys = new EBYSCumhurbaskanligiBirim();
                        ebys.kod = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligiBirim.ENUMAlanlar.KOD, 0);
                        ebys.ad = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligiBirim.ENUMAlanlar.AD, "");
                        ebys.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligiBirim.ENUMAlanlar.MUHASEBEKOD, "");
                        ebys.harcamaKod = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligiBirim.ENUMAlanlar.HARCAMAKOD, "");

                        bilgi.Ekle(ebys);
                    }
                    dr.Close();
                }
                else
                    bilgi.sonuc.DegerAta(ve.hata);

            }
            catch (Exception ex)
            {
                bilgi.sonuc.DegerAta(ex.Message);
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

    }
   

}

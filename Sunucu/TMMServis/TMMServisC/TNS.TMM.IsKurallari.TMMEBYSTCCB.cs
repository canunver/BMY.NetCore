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
    public static class TMMEBYSCumhurbaskanligi
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EBYSCumhurbaskanligi form)
        {
            string hata = "";
            if (form.evrakId <= 0)
                hata += "Evrak id bilgisi boş olamaz." + "<br>";
            if (form.yil <= 0)
                hata += "Yıl bilgisi boş olamaz." + "<br>";
            if (form.muhasebeKod == "")
                hata += "Muhasebe Kodu boş olamaz." + "<br>";
            if (form.harcamaKod == "")
                hata += "Harcama Birim Kodu boş olamaz." + "<br>";
            if (form.fisNo == "")
                hata += "Belge No bilgisi boş olamaz." + "<br>";
            if (form.belgeTur <= 0)
                hata += "Belge Türü EBYS göndermek için uygun değil." + "<br>";


            if (hata != "")
                return new Sonuc(false, hata, "");


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABEBYSCumhurbaskanligi to = new TABEBYSCumhurbaskanligi(ve);

            ve.BeginTransaction();

            form.durum = 1; //Taslak Kayıt
            form.islemYapan = kullanan.TCKIMLIKNO;
            form.islemTarih = new TNSDateTime(DateTime.Now);

            Sonuc sonuc = to.Kaydet(form);

            if (!sonuc.islemSonuc)
                ve.Rollback();
            else
            {
                sonuc.bilgiStr = "EBYS sistemine başarıyla gönderildi.";
                ve.Commit();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EBYSCumhurbaskanligi form)
        {
            ObjectArray bilgi = new ObjectArray();

            string hata = "";
            if (form.yil <= 0)
                hata += "Yıl giriniz." + "<br>";
            if (form.muhasebeKod == "")
                hata += "Muhasebe Kodu giriniz." + "<br>";
            if (form.harcamaKod == "")
                hata += "Harcama Birim Kodu giriniz." + "<br>";
            if (form.fisNo == "")
                hata += "Belge No giriniz." + "<br>";
            if (form.belgeTur <= 0)
                hata += "BelgeTur giriniz." + "<br>";

            if (hata != "")
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = hata;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                TABEBYSCumhurbaskanligi to = new TABEBYSCumhurbaskanligi(ve);

                IDataReader dr = to.Listele(form);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        EBYSCumhurbaskanligi ebys = new EBYSCumhurbaskanligi();
                        ebys.evrakId = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligi.ENUMAlanlar.EVRAKID, 0);
                        ebys.yil = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligi.ENUMAlanlar.YIL, 0);
                        ebys.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligi.ENUMAlanlar.MUHASEBEKOD, "");
                        ebys.harcamaKod = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligi.ENUMAlanlar.HARCAMAKOD, "");
                        ebys.fisNo = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligi.ENUMAlanlar.FISNO, "");
                        ebys.belgeTur = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligi.ENUMAlanlar.BELGETUR, 0);
                        ebys.islemYapan = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligi.ENUMAlanlar.ISLEMYAPAN, "");
                        ebys.islemTarih = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligi.ENUMAlanlar.ISLEMTARIH);
                        ebys.durum = VeriErisim.DegerAl(dr, (int)TABEBYSCumhurbaskanligi.ENUMAlanlar.DURUM, 0);

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

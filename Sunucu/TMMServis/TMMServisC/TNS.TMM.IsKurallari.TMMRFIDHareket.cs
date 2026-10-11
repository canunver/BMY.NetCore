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
    public static class TMMRFIDHareket
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, RFIDHareket form)
        {
            string hata = "";
            if (form.prSicilNo <= 0)
                hata += "prSicilNo boş olamaz." + "<br>";

            if (hata != "")
                return new Sonuc(false, hata, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRFIDHareket to = new TABRFIDHareket(ve);

            form.kod = Guid.NewGuid().ToString("N");
            form.durum = (int)ENUMRFIDEtiketDurum.BASILDI;
            form.islemYapan = kullanan.kullaniciKodu;
            form.islemTarih = new TNSDateTime(DateTime.Now);

            ve.BeginTransaction();

            Sonuc sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
            {
                //prSicilNo Özellik sayfasını güncelle
                sonuc = new TMMSicilNoOzellik().RFIDEtiketTarihiGuncelle(kullanan, form.prSicilNo, form.islemTarih);
                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = "Kayıt işlemi başarıyla gerçekleştirildi.";
                    ve.Commit();
                }
                else
                    ve.Rollback();
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, RFIDHareket form)
        {
            ObjectArray bilgi = new ObjectArray();

            string hata = "";
            if (form.prSicilNo <= 0)
                hata += "prSicilNo boş olamaz." + "<br>";

            if (hata != "")
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = hata;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                TABRFIDHareket to = new TABRFIDHareket(ve);

                IDataReader dr = to.Listele(form);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        RFIDHareket d = new RFIDHareket();
                        d.kod = VeriErisim.DegerAl(dr, (int)TABRFIDHareket.ENUMAlanlar.KOD, "");
                        d.prSicilNo = VeriErisim.DegerAl(dr, (int)TABRFIDHareket.ENUMAlanlar.PRSICILNO, 0);
                        d.etiketTurKod = VeriErisim.DegerAl(dr, (int)TABRFIDHareket.ENUMAlanlar.ETIKETTURKOD, 0);
                        d.islemYapan = VeriErisim.DegerAl(dr, (int)TABRFIDHareket.ENUMAlanlar.ISLEMYAPAN, "");
                        d.islemTarih = VeriErisim.DegerAl(dr, (int)TABRFIDHareket.ENUMAlanlar.ISLEMTARIH);
                        d.durum = VeriErisim.DegerAl(dr, (int)TABRFIDHareket.ENUMAlanlar.DURUM, 0);

                        bilgi.Ekle(d);
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

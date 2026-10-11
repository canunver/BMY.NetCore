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
    public class BirimTorbaOdenekTalepIs
    {
        public static string VeriKontrol(BirimTorbaOdenekTalep talep, ENUMOGY islemturu)
        {
            StringBuilder hatalar = new StringBuilder();
            if (talep.yil <= 0)
                hatalar.Append("Yıl boş olmamalıdır<br/>");
            if (string.IsNullOrEmpty(talep.birim.birimKod) || talep.birim.muhasebeKod <= 0 || string.IsNullOrEmpty(talep.birim.kurkod))
                hatalar.Append("Birimin muhasebe,birim ve kurumsal kodu dolu olmalıdır<br/>");
            if (talep.detay.Count == 0 && (islemturu == ENUMOGY.Yazma))
                hatalar.Append("İşlem yapılacak kayıt bulunmadı<br>");
            foreach (BirimTorbaOdenekTalepDetay item in talep.detay)
            {
                if (talep.detay.Count == 0)
                    hatalar.Append("En az bir talep kaydı girmelisiniz<br/>");

                if (islemturu == ENUMOGY.Yazma && (item.durum == (int)ENUMTorbaOdenekTalepDurum.YENI || item.durum == (int)ENUMTorbaOdenekTalepDurum.TALEPEDILDI))
                {
                    if (string.IsNullOrEmpty(item.isadi) || string.IsNullOrEmpty(item.talepgerekce))
                    {
                        hatalar.Append("Bütün kayıtların is adı ve gerekçe bilgileri belirtilmelidir<br>");
                        break;
                    }
                }
                if (islemturu == ENUMOGY.Yazma && (item.durum == (int)ENUMTorbaOdenekTalepDurum.REDEDILDI))
                {
                    if (string.IsNullOrEmpty(item.redgerekce))
                    {
                        hatalar.Append("Red edilen taleplerin red gerekçelerini belirtiniz<br>");
                        break;
                    }
                }
            }

            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim verierisim, Kullanici kullanan, BirimTorbaOdenekTalep talep)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.hataStr = VeriKontrol(talep, ENUMOGY.Yazma);
            if (sonuc.hataStr.Trim() != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(verierisim, BHMv2Genel.VeriErisimAdBul(talep.yil), kullanan.KimlikBul());
            ve.BeginTransaction();
            TABTORBATERTIPBIRIMODENEKTALEP TABTalep = new TABTORBATERTIPBIRIMODENEKTALEP(ve);
            TABTalep.Sil(talep.yil, talep.birim.kurkod, talep.birim.muhasebeKod, talep.birim.birimKod, 0);
            if (ve.hata.Trim() == "")
            {
                sonuc = TABTalep.Kaydet(talep);
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
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc DurumGuncelle(VeriErisim verierisim, Kullanici kullanan, BirimTorbaOdenekTalep talep)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.hataStr = VeriKontrol(talep, ENUMOGY.Yazma);
            if (sonuc.hataStr.Trim() != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }
            BirimTorbaOdenekTalepDetay detay = (BirimTorbaOdenekTalepDetay)talep.detay[0];
            VeriErisim ve = VeriErisim.VeriErisimBul(verierisim, BHMv2Genel.VeriErisimAdBul(talep.yil), kullanan.KimlikBul());
            ve.BeginTransaction();
            TABTORBATERTIPBIRIMODENEKTALEP TABTalep = new TABTORBATERTIPBIRIMODENEKTALEP(ve);
            sonuc = TABTalep.DurumGuncelle(talep);
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
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim verierisim, Kullanici kullanan, BirimTorbaOdenekTalep talep)
        {
            ObjectArray sonuclar = new ObjectArray();
            sonuclar.sonuc.hataStr = VeriKontrol(talep, ENUMOGY.Gorme);
            if (sonuclar.sonuc.hataStr.Trim() != string.Empty)
            {
                sonuclar.sonuc.islemSonuc = false;
                return sonuclar;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(verierisim, BHMv2Genel.VeriErisimAdBul(talep.yil), kullanan.KimlikBul());
            ve.BeginTransaction();
            TABTORBATERTIPBIRIMODENEKTALEP TABTalep = new TABTORBATERTIPBIRIMODENEKTALEP(ve);
            IDataReader dr = null;

            BirimTorbaOdenekTalep btot = new BirimTorbaOdenekTalep();
            btot.yil = talep.yil;
            btot.birim.kurkod = talep.birim.kurkod;
            btot.birim.muhasebeKod = talep.birim.muhasebeKod;
            btot.birim.birimKod = talep.birim.birimKod;

            try
            {
                dr = TABTalep.Listele(talep.yil, talep.birim.kurkod, talep.birim.muhasebeKod, talep.birim.birimKod, 0);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        BirimTorbaOdenekTalepDetay detay = new BirimTorbaOdenekTalepDetay();
                        detay.sirano = VeriErisim.DegerAl(dr, 4, 0);
                        detay.isadi = VeriErisim.DegerAl(dr, 5, "");
                        detay.talepgerekce = VeriErisim.DegerAl(dr, 6, "");
                        detay.miktar = VeriErisim.DegerAl(dr, 7, 0.0);
                        detay.durum = VeriErisim.DegerAl(dr, 8, 0);
                        detay.redgerekce = VeriErisim.DegerAl(dr, 9, "");
                        btot.detay.Add(detay);
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
                if (dr != null && !dr.IsClosed) dr.Close();
            }
            sonuclar.objeler.Add(btot);

            ve.VeriErisimKapat();
            return sonuclar;
        }
    }
}

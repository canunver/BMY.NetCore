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

namespace TNS.TMM.IsKurallari
{
    public class TMMAyarlarMB
    {
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, AyarlarMB form)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                TABAyarlarMB to = new TABAyarlarMB(ve);

                IDataReader dr = to.Listele(form);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        int sonIndex = (int)TABAyarlarMB.ENUMAlanlar.AONAYIMUHASEBEKONTROLU;

                        AyarlarMB ayar = new AyarlarMB();
                        ayar.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABAyarlarMB.ENUMAlanlar.MUHASEBEKOD, "");
                        ayar.harcamaKod = VeriErisim.DegerAl(dr, (int)TABAyarlarMB.ENUMAlanlar.HARCAMABIRIMKOD, "");
                        ayar.ambarKod = VeriErisim.DegerAl(dr, (int)TABAyarlarMB.ENUMAlanlar.AMBARKOD, "");
                        ayar.islemYapan = VeriErisim.DegerAl(dr, (int)TABAyarlarMB.ENUMAlanlar.ISLEMYAPAN, "");
                        ayar.islemTarihi = VeriErisim.DegerAl(dr, (int)TABAyarlarMB.ENUMAlanlar.ISLEMTARIH);
                        ayar.fisGirisKapanisTarihi = VeriErisim.DegerAl(dr, (int)TABAyarlarMB.ENUMAlanlar.FISGIRISKAPANISTARIH);
                        ayar.mutemmimBitisTarihi = VeriErisim.DegerAl(dr, (int)TABAyarlarMB.ENUMAlanlar.MUTEMMIMBITISTARIH);
                        ayar.aOnayiMuhasebeKontrolu = VeriErisim.DegerAl(dr, (int)TABAyarlarMB.ENUMAlanlar.AONAYIMUHASEBEKONTROLU, 0);

                        ayar.muhasebeAd = VeriErisim.DegerAl(dr, ++sonIndex, "");
                        ayar.harcamaAd = VeriErisim.DegerAl(dr, ++sonIndex, "");
                        ayar.ambarAd = VeriErisim.DegerAl(dr, ++sonIndex, "");

                        bilgi.Ekle(ayar);
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

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AyarlarMB form)
        {
            Sonuc sonuc = new Sonuc();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (form.muhasebeKod.Trim() == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM013 + "<br>";

            if (form.harcamaKod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";

            if (form.ambarKod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAyarlarMB to = new TABAyarlarMB(ve);

            ve.BeginTransaction();

            form.islemYapan = kullanan.mernis;
            form.islemTarihi = new TNSDateTime(DateTime.Now);

            sonuc = to.Kaydet(form);

            if (!sonuc.islemSonuc)
                ve.Rollback();
            else
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                ve.Commit();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, AyarlarMB form)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAyarlarMB to = new TABAyarlarMB(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc TestYap(VeriErisim veriErisim, Kullanici kullanan, int islem, object form)
        {
            Sonuc sonuc = new Sonuc("Bilinmeyen bir hata oluştu.");

            if (islem == 1)
            {
                try
                {
                    string yazilimMailAdresi = OrtakFonksiyonlar.WebConfigOku("yazilimMailAdresi", "");
                    string mailAdres = OrtakFonksiyonlar.ConvertToStr(form);

                    string mail = OrtakFonksiyonlar.MailAt(yazilimMailAdresi, mailAdres, "E-Posta Testi (Sunucu)", "Test", true, false, null);

                    if (mail != "" && mail != "1")
                        sonuc.hataStr = mail;
                    else
                    {
                        sonuc.islemSonuc = true;
                        sonuc.bilgiStr = mailAdres + "<br /> adresine mail gönderilmiştir.";
                    }
                }
                catch (Exception ex)
                {
                    sonuc.hataStr = "E-Posta Gönderilemedi.";
                    OrtakFonksiyonlar.HataStrYaz(ex.Message);
                }

            }


            return sonuc;
        }

    }

}

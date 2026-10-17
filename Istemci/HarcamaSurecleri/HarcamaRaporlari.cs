using System;
using System.Collections.Generic;
using System.Linq;
using OrtakClass;
using TNS.KYM;
using TNS;
using TNS.UZY;
using TNS.HRC;
using TNS.BKD;

namespace HarcamaSurecleri
{
    /// <summary>
    /// Masraf Cetveli Muhasebe işlemlerini düzenleyen sınıf
    /// (Rapor.xml NESNE "HarcamaSurecleri.HarcamaRaporlari"; kıstaslar: Yil, Ay, KurKod, Muhasebe, Birim, FirmaSayi, VergiNo).
    /// Eski HarcamaSurecleri/HarcamaRaporlari.cs'ten alındı; değişenler: System.Web ve TNS.Raporlama.IRapor arayüzü kaldırıldı.
    /// </summary>
    public class HarcamaRaporlari
    {

        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

        private string KurumAdGetir(Kullanici kullanan, int yil, string kurKod)
        {
            return servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurKod, true);

        }

        /// <summary>
        /// Masraf Cetveli Muhasebe raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Ay"], 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            string birim = (string)raporParam["Birim"];
            string kur = (string)raporParam["KurKod"];
            int firmaSayisi = OrtakFonksiyonlar.ConvertToInt((string)raporParam["FirmaSayi"], 0);
            string vergiNo = (string)raporParam["VergiNo"];
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);


            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            //XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);
            //XLS.HucreAdBulYaz("txtYil", yil.ToString());

            //XLS.HucreAdAdresCoz("txtToplamSatir", ref satirT, ref sutunT);
            //XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            //XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);

            RaporIhaleFirma kr = new RaporIhaleFirma();
            kr.yil = yil;
            kr.ay = ay;
            kr.muhasebe = muhasebe;
            kr.birim = birim;
            kr.kurKod = kur.Replace(".", "");
            kr.firmaVergiNo = vergiNo;

            ObjectArray liste = servisHRC.RaporIhaleFirmaListele(kullanan, kr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int sonSutun = 6;

            int kaynakSatir = 0;

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            //sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            //string sablonAd = string.Empty;
            //sablonAd = Server.MapPath("~") + "\\RaporSablon\\HRC\\AboneFatura.xlt";

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            satir++;

            string kurKodCache = string.Empty;
            int yazilanFirmaSayisi = 0;
            decimal toplam0 = decimal.Zero;
            decimal toplam1 = decimal.Zero;
            decimal toplam3 = decimal.Zero;
            decimal toplam4 = decimal.Zero;

            foreach (RaporIhaleFirma rif in liste.objeler)
            {
                //XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);

                if (string.IsNullOrEmpty(kurKodCache))
                {
                    kurKodCache = rif.kurKod;
                    XLS.HucreDegerYaz(satir, 0, KurumAdGetir(kullanan, yil, rif.kurKod));
                }

                if (kurKodCache != rif.kurKod)
                {
                    XLS.HucreBirlestir(satir - yazilanFirmaSayisi, 0, satir, 0);
                    XLS.CerceveCiz(satir - yazilanFirmaSayisi, satir + 1, 0, sonSutun, LineStyle.MEDIUM, TabloRenk.BLACK);

                    XLS.HucreDegerYaz(satir, 1, "TOPLAM");
                    if (toplam0 > 0)
                        XLS.HucreDegerYaz(satir, 4, toplam0);
                    if (toplam1 > 0)
                        XLS.HucreDegerYaz(satir, 2, toplam1);
                    if (toplam3 > 0)
                        XLS.HucreDegerYaz(satir, 3, toplam3);
                    if (toplam4 > 0)
                        XLS.HucreDegerYaz(satir, 5, toplam4);

                    XLS.KoyuYap(satir, 1, true);
                    XLS.KoyuYap(satir, 2, true);
                    XLS.KoyuYap(satir, 3, true);
                    XLS.KoyuYap(satir, 4, true);
                    XLS.KoyuYap(satir, 5, true);
                    satir++;

                    //XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);

                    kurKodCache = rif.kurKod;
                    XLS.HucreDegerYaz(satir, 0, KurumAdGetir(kullanan, yil, rif.kurKod));
                    yazilanFirmaSayisi = 0;
                    toplam0 = 0;
                    toplam1 = 0;
                    toplam3 = 0;
                    toplam4 = 0;
                }
                else if (yazilanFirmaSayisi == firmaSayisi)
                    continue;

                yazilanFirmaSayisi++;

                XLS.HucreDegerYaz(satir, 1, rif.firmaAd);
                XLS.HucreDegerYaz(satir, RaporIhaleFirmaSutunBul(rif.ihaleUsulu), rif.toplam);
                satir++;

                switch (rif.ihaleUsulu)
                {
                    //dmo -ptt
                    case 0:
                        toplam0 += rif.toplam;
                        break;
                    //açık ihale
                    case 1:
                        toplam1 += rif.toplam;
                        break;
                    //pazarlık usulu
                    case 3:
                        toplam3 += rif.toplam;
                        break;
                    //doğrudan temin
                    case 4:
                        toplam4 += rif.toplam;
                        break;
                    default:
                        break;
                }
            }

            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);
            XLS.HucreBirlestir(satir - yazilanFirmaSayisi, 0, satir, 0);
            XLS.CerceveCiz(satir - yazilanFirmaSayisi, satir + 1, 0, sonSutun, LineStyle.MEDIUM, TabloRenk.BLACK);

            XLS.HucreDegerYaz(satir, 1, "TOPLAM");
            if (toplam0 > 0)
                XLS.HucreDegerYaz(satir, 4, toplam0);
            if (toplam1 > 0)
                XLS.HucreDegerYaz(satir, 2, toplam1);
            if (toplam3 > 0)
                XLS.HucreDegerYaz(satir, 3, toplam3);
            if (toplam4 > 0)
                XLS.HucreDegerYaz(satir, 5, toplam4);

            XLS.KoyuYap(satir, 1, true);
            XLS.KoyuYap(satir, 2, true);
            XLS.KoyuYap(satir, 3, true);
            XLS.KoyuYap(satir, 4, true);
            XLS.KoyuYap(satir, 5, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private int RaporIhaleFirmaSutunBul(int ihaleUsulu)
        {
            switch (ihaleUsulu)
            {
                //dmo -ptt
                case 0:
                    return 4;
                //açık ihale
                case 1:
                    return 2;
                //pazarlık usulu
                case 3:
                    return 3;
                //doğrudan temin
                case 4:
                    return 5;
                default:
                    return 2;
            }
        }
    }
}
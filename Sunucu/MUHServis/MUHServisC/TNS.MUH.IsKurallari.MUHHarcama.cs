using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MUHServisC;
using TNS.Veri;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using System.Data;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class MUHHarcama
    {
        private static string VeriKontrol(ISLHarcama bilgi)
        {
            StringBuilder hatalar = new StringBuilder();
            if (bilgi.yil <= 0)
                hatalar.Append("Yıl sıfırdan büyük olmalıdır<br/>");
            if (bilgi.yil > DateTime.Now.Year)
                hatalar.Append("Seçili yıl değeri şimdiki yıl değerinden büyük olmaz<br/>");
            //if (bilgi.muhasebeKod<=0)
            //    hatalar.Append("Muhasebe kodu boş olmamalıdır<br/>");
            //if (string.IsNullOrEmpty(bilgi.birimKod))
            //    hatalar.Append("Birim kodu boş olmamalıdır<br/>");
            //if (string.IsNullOrEmpty(bilgi.kurKod))
            //    hatalar.Append("Kurumsal kod boş olmamalıdır<br/>");
            //if (string.IsNullOrEmpty(bilgi.fonKod))
            //    hatalar.Append("Fonksiyonel kod boş olmamalıdır<br/>");
            //if (string.IsNullOrEmpty(bilgi.finKod))
            //    hatalar.Append("Finansal kod boş olmamalıdır<br/>");
            //if (string.IsNullOrEmpty(bilgi.ekoKod))
            //    hatalar.Append("Ekonomik kod boş olmamalıdır<br/>");
            return hatalar.ToString();
        }

        /// <summary>
        /// Listeles the specified verierisim.
        /// </summary>
        /// <param name="verierisim">The verierisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim verierisim, Kullanici kullanan, ISLHarcama bilgi)
        {
            ObjectArray sonuclar = new ObjectArray();
            string hata = string.Empty;
            hata = VeriKontrol(bilgi);
            if (hata.Trim() == string.Empty)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(verierisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                IDataReader dr = null;
                try
                {
                    ve.BeginTransaction();
                    string kodyapiKur = "";
                    string kodyapiFon = "";
                    string kodyapiEko = "";

                    kodyapiKur = TNS.MUH.Arac.DegiskenDegerBul(0, "KODYAPI/KOD" + 1 + "/KODYAPI");
                    kodyapiFon = TNS.MUH.Arac.DegiskenDegerBul(0, "KODYAPI/KOD" + 2 + "/KODYAPI");
                    kodyapiEko = TNS.MUH.Arac.DegiskenDegerBul(0, "KODYAPI/KOD" + 4 + "/KODYAPI");
                    TABHarcama tabHRC = new TABHarcama(ve);
                    dr = tabHRC.Listele(bilgi);
                    while (dr.Read())
                    {
                        ISLHarcama hrc = new ISLHarcama();
                        hrc.yil = VeriErisim.DegerAl(dr, 0, 0);
                        hrc.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                        hrc.birimKod = VeriErisim.DegerAl(dr, 2, "");
                        hrc.kurKod = OrtakFonksiyonlar.KodNoktaKoy(kodyapiKur, VeriErisim.DegerAl(dr, 3, ""));
                        hrc.fonKod = OrtakFonksiyonlar.KodNoktaKoy(kodyapiFon, VeriErisim.DegerAl(dr, 4, ""));
                        hrc.finKod = VeriErisim.DegerAl(dr, 5, "");
                        hrc.ekoKod = OrtakFonksiyonlar.KodNoktaKoy(kodyapiEko, VeriErisim.DegerAl(dr, 6, ""));
                        for (int i = 1; i < 13; i++)
                        {
                            Ay ay = new Ay();
                            ay.id = i;
                            ay.miktar = VeriErisim.DegerAl(dr, 6 + i, 0.0);
                            hrc.aylar.objeler.Add(ay);
                        }
                        sonuclar.objeler.Add(hrc);
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
                    sonuclar.sonuc.islemSonuc = false;
                    sonuclar.sonuc.hataStr = ex.ToString();

                }
                finally
                {
                    ve.VeriErisimKapat();
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }
            else
            {
                sonuclar.sonuc.islemSonuc = false;
                sonuclar.sonuc.hataStr = hata;
            }

            string muhasebatTur = System.Configuration.ConfigurationManager.AppSettings.Get("MuhasebatTur") + "";
            if (OrtakFonksiyonlar.ConvertToInt(muhasebatTur, 0) > 0)//Eğer muhasebat tablosu MUHMIFFORM tablosundan oluşuyorsa harcama rakamları kümülatif değil bu nedenle kümülatif hale getirmek için
            {
                foreach (ISLHarcama h in sonuclar.objeler)
                {
                    double toplam = 0;
                    foreach (Ay a in h.aylar.objeler)
                    {
                        toplam += a.miktar;
                        a.miktar = toplam;
                    }
                }
            }
            return sonuclar;
        }
    }
}

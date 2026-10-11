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
    public class GELIsParca
    {
        protected static string IsParcaKontrol(IsParca bilgi, ENUMIslemTuru islemturu)
        {
            StringBuilder hatalar = new StringBuilder();
            if (bilgi.isAd.kod <= 0)
                hatalar.Append("İş Kodu Boş Olmamalıdır.<br/>");
            if (islemturu == ENUMIslemTuru.KAYIT)
            {
                int kayitno = 1;
                foreach (Parca ip in bilgi.parcalar.objeler)
                {
                    if (ip.duzey <= 0)
                        hatalar.Append(kayitno + ". Satırda Parça Düzey Bilgisi Boş Olmamalıdır.<br/>");
                    kayitno++;
                }
            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IsParca bilgi)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = IsParcaKontrol(bilgi, ENUMIslemTuru.KAYIT);
            if (hata.Trim() != string.Empty) hatalar.Append(hata);
            if (hatalar.ToString() != string.Empty)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIsParca tabIsParca = new TABIsParca(ve);
            ve.BeginTransaction();
            sonuc = tabIsParca.Sil(bilgi.isAd.kod);
            if (sonuc.islemSonuc)
                sonuc = tabIsParca.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00025;
                ve.Commit();
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }


        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            ObjectArray donusdeger = new ObjectArray();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = (kod <= 0) ? "İş Kodu Bilgisi Boş Olmamalıdır" : string.Empty;
            if (hata.Trim() != string.Empty) hatalar.Append(hata);
            if (hatalar.ToString() != string.Empty)
            {
                donusdeger.sonuc.islemSonuc = false;
                donusdeger.sonuc.hataStr = hatalar.ToString();
                return donusdeger;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIsParca tabIsParca = new TABIsParca(ve);
            ve.BeginTransaction();
            IDataReader dr = tabIsParca.Listele(kod);
            IsParca ip = new IsParca();
            while (dr.Read())
            {
                ip.isAd.kod = VeriErisim.DegerAl(dr, 0, 0);
                Parca pBilgi = new Parca();
                pBilgi.kod = VeriErisim.DegerAl(dr, 1, 0);
                //2=sırano
                pBilgi.duzey = VeriErisim.DegerAl(dr, 3, 0);
                pBilgi.zorunlu = VeriErisim.DegerAl(dr, 4, 0);
                pBilgi.ad = VeriErisim.DegerAl(dr, 5, "");
                pBilgi.sayfa = VeriErisim.DegerAl(dr, 6, "");
                pBilgi.yukseklik = VeriErisim.DegerAl(dr, 7, 0);
                pBilgi.genislik = VeriErisim.DegerAl(dr, 8, 0);
                ip.parcalar.objeler.Add(pBilgi);
            }
            dr.Close();
            donusdeger.objeler.Add(ip);
            ve.VeriErisimKapat();
            return donusdeger;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.UZY;
using TNS.KYM;
using TNS.KPM;

namespace TNS.KPM.IsKurallari
{
    [Serializable()]
    public class KPMServis : MarshalByRefObject, TNS.KPM.IKPMServis
    {
        public static IUZYServis servisUZY = null;

        public KPMServis()
        {
        }

        //public static string UzayYaziDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        //{
        //    if (servisUZY == null)
        //        servisUZY = TNS.UZY.Arac.Tanimla();

        //    string deger = "";
        //    if (uzayAlan == "")
        //        deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
        //    else
        //        deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

        //    return deger;
        //}

        public ObjectArray IlListele(Kullanici kullanan)
        {
            try
            {
                return KPMIl.IlListele(kullanan);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray IlceListele(Kullanici kullanan)
        {
            try
            {
                return KPMIlce.IlceListele(kullanan);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray IlceListele(Kullanici kullanan, int ilKod)
        {
            try
            {
                return KPMIlce.IlceListele(kullanan, ilKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray KoyListele(Kullanici kullanan, int ilKod, int ilceKod)
        {
            try
            {
                return KPMIlce.KoyListele(kullanan, ilKod, ilceKod);
                //return KPMIlce.BucakListele(kullanan, ilKod, ilceKod);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("Hata:" + e.ToString());
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray BucakListele(Kullanici kullanan, int ilKod, int ilceKod)
        {
            try
            {
                return KPMIlce.BucakListele(kullanan, ilKod, ilceKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray IstekListele(Kullanici kullanan, Istek istek)
        {
            try
            {
                return KPMIstek.Listele(null, kullanan, istek);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray NufusKayitOrnekRaporla(Kullanici kullanan, Istek istek)
        {
            try
            {
                return KPMNufusKayitOrnek.NufusKayitOrnekRaporla(kullanan, istek);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = "Kesilme oluştu:" + BaglantiBilgi() + " hata"+ e.ToString();
                return donusDeger;
            }
        }

        public static string BaglantiBilgi()
        {
            return $"Kullanıcı:{Mernis.KPSv2.Types.KPSConfiguration.Instance.Username}, Parola:{Mernis.KPSv2.Types.KPSConfiguration.Instance.Password}, adres:{Mernis.KPSv2.Types.KPSConfiguration.Instance.EndPoint}";
        }

        public ObjectArray IkametgahRaporla(Kullanici kullanan, Istek istek)
        {
            try
            {
                return KPMIkametgah.IkametgahRaporla(kullanan, istek);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray VukuatliNufusKayitOrnekRaporla(Kullanici kullanan, Istek istek)
        {
            try
            {
                return KPMVukuatliNufusKayitOrnek.VukuatliNufusKayitOrnekRaporla(kullanan, istek);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
    }
}

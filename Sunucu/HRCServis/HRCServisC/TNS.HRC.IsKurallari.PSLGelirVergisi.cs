using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{

    /// <summary>
    /// Gelir vergisi tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCGelirVergisi
    {
        /// <summary>
        /// Arguman olarak verilen, gelir vergisi tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri PSLGelirVergisi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (liste.objeler.Count == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {

                foreach (TNS.HRC.PSLGelirVergisi k in liste.objeler)
                {
                    if (k.yil == 0)
                        hata += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
                    if (k.yuzde == 0)
                        hata += TNS.HRC.HarcamaSurecleriResI.HRC00293 + "<br>";

                }

            }
            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }


            VergisiniHesapla(liste);

            TNS.HRC.PSLGelirVergisi gelirVergisi = ((TNS.HRC.PSLGelirVergisi)liste.objeler[0]);


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGelirVergisi to = new TABGelirVergisi(ve);
            ve.BeginTransaction();

            //------------------------------Kontrol koyulacak
            sonuc = to.Sil(gelirVergisi);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }


        private void VergisiniHesapla(ObjectArray liste)
        {
            //1.Adım ilk vergi hesabını yap ve artan kısımları bul 
            decimal oncekiMiktar = 0;
            foreach (TNS.HRC.PSLGelirVergisi k in liste.objeler)
            {
                if (oncekiMiktar == 0)
                {
                    oncekiMiktar = k.miktar;
                    //en küçük miktarın vergisini hesapla
                    k.vergisi = (k.miktar * k.yuzde) / 100;
                }
                else if (k.miktar != 0)
                {
                    k.artan = k.miktar - oncekiMiktar;
                    oncekiMiktar = k.miktar;
                }
            }
            //2. adım hesaplanan en küçük vergiyi al
            decimal eklenecekMiktar = 0;
            foreach (TNS.HRC.PSLGelirVergisi k in liste.objeler)
            {
                if (eklenecekMiktar == 0)
                    eklenecekMiktar = k.vergisi;
                else if (k.artan != 0)
                {
                    k.vergisi = ((k.artan * k.yuzde) / 100) + eklenecekMiktar;
                    eklenecekMiktar = k.vergisi;
                }
            }

        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, gelir vergisi tanım bilgisini veritabanından okur 
        /// ve PSLGelirVergisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, gelir vergisi tanım listesini PSLGelirVergisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TNS.HRC.PSLGelirVergisi kriter)
        {
            ObjectArray donusDegeri = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGelirVergisi to = new TABGelirVergisi(ve);
            IDataReader r = to.Listele(kriter);
            TNS.HRC.PSLGelirVergisi ekleObj;
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ekleObj = new TNS.HRC.PSLGelirVergisi();
                    ekleObj.yil = VeriErisim.DegerAl(r, 0, 0);
                    ekleObj.yuzde = (decimal)VeriErisim.DegerAl(r, 2, 0.0);
                    ekleObj.miktar = (decimal)VeriErisim.DegerAl(r, 3, 0.0);
                    ekleObj.vergisi = (decimal)VeriErisim.DegerAl(r, 4, 0.0);

                    donusDegeri.objeler.Add(ekleObj);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return donusDegeri;
        }
    }

}

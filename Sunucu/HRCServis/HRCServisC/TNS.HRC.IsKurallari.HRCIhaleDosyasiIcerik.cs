using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhale dosyası içerik tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleDosyasiIcerik
    {
        /// <summary>
        /// Arguman olarak verilen, ihale dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
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

            string hataStr = "";
            if (liste.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                foreach (IhaleDosyasiIcerik bilgi in liste.objeler)
                {
                    if (bilgi.isTuru == 0)
                    {
                        hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00146 + "<br>";
                        break;
                    }
                    if (bilgi.ihaleUsul == 0)
                    {
                        hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00147 + "<br>";
                        break;
                    }
                    if (bilgi.adi == "")
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00148, bilgi.sira) + "<br>";
                }
            }

            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleDosyasiIcerik to = new TABIhaleDosyasiIcerik(ve);
            ve.BeginTransaction();
            IhaleDosyasiIcerik iDI;
            iDI = (IhaleDosyasiIcerik)liste.objeler[0];
            sonuc = to.Sil(iDI);

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

        /// <summary>
        /// Arguman olarak verilen, ihale dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleDosyasiIcerik nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ListeKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            string hataStr = "";
            if (liste.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";

            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleDosyasiIcerik to = new TABIhaleDosyasiIcerik(ve);
            ve.BeginTransaction();


            sonuc = to.Sil(new IhaleDosyasiIcerik());

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

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale dosyası içerik tanım bilgisini veritabanından okur 
        /// ve IhaleDosyasiIcerik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale dosyası içerik tanım bilgisi listesini IhaleDosyasiIcerik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, IhaleDosyasiIcerik kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            /* Not: ISturu alanında işin kodu geliyor o yüzden iştürü iş kodu olduğundan işi okuyup, iş türünü bulma eklendi*/

            Is isBilgi = new Is();
            isBilgi.kod = kriter.isTuru;

            ObjectArray oa = new TNS.HRC.IsKurallari.HRCIs().Listele(kullanan, isBilgi);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
            {
                isBilgi = (Is)oa.objeler[0];
                kriter.isTuru = isBilgi.tur;
            }

            TABIhaleDosyasiIcerik to = new TABIhaleDosyasiIcerik(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {

                    IhaleDosyasiIcerik bilgi = new IhaleDosyasiIcerik();
                    bilgi.isTuru = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.ihaleUsul = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.sira = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.adi = VeriErisim.DegerAl(r, 3, "");
                    bilgi.dosyaAdi = VeriErisim.DegerAl(r, 4, "");

                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }
    }
}

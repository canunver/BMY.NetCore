using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Maaş temel tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCMaasTemelBilgisi
    {
        /// <summary>
        /// Arguman olarak verilen, maaş temel tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri PSLMaasTemelBilgisi nesne koleksiyonu olarak tutan nesne</param>
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
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00007;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMaasTemelBilgi to = new TABMaasTemelBilgi(ve);
            ve.BeginTransaction();

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
        /// Arguman olarak verilen kriterlere bağlı olarak, maaş temel tanım bilgisini veritabanından okur 
        /// ve PSLMaasTemelBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, maaş temel tanım bilgisi listesini PSLMaasTemelBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TNS.HRC.PSLMaasTemelBilgisi kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMaasTemelBilgi to = new TABMaasTemelBilgi(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TNS.HRC.PSLMaasTemelBilgisi f = new TNS.HRC.PSLMaasTemelBilgisi();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.ay = VeriErisim.DegerAl(r, 1, 0);
                    f.gelirVergisiOran = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.GELIRVERGISIORAN, 0.0);
                    f.enYuksMemurMaasGost = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.ENYUKSMAASGOSTERGE, 0.0);
                    f.aylikKatsayi = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.AYLIKKATSAYI, 0.0);
                    f.tabanAylikKatsayi = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.TABANAYLIKKATSAYI, 0.0);
                    f.yanOdemeKatsayi = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.YANODEMEKATSAYI, 0.0);
                    f.sskIsverenPayiOran = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.SSKISVERENPAYI, 0.0);
                    f.sskIsciPayiOran = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.SSKISCIPAYI, 0.0);
                    f.sskIssizSigIsVerenPayiOran = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.SSKISSIZLIKSIGISVERENPAYI, 0.0);
                    f.sskIssizSigIsciPayiOran = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.SSKISSIZLIKSIGISCIPAYI, 0.0);
                    f.damgaVergisiOran = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.DAMGAVERGISIORAN, 0.0);
                    f.brutAsgariUcret = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.BRUTASGARIUCRET, 0.0);
                    f.isKazasiMeslekHast = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.ISKAZASIMESLEKIHAST, 0.0);
                    f.analikSigortasi = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.ANALIKSIGORTASI, 0.0);
                    f.hastalikSigortasi = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.HASTALIKSIGORTASI, 0.0);
                    f.malulYasliOlumSigortasi = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.MALULYASLIOLUMSIG, 0.0);
                    f.netAsgariUcret = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.NETASGARIUCRET, 0.0);
                    f.sakatlikIndirimi1 = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.SAKATLIKINDIRIM1, 0.0);
                    f.sakatlikIndirimi2 = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.SAKATLIKINDIRIM2, 0.0);
                    f.sakatlikIndirimi3 = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.SAKATLIKINDIRIM3, 0.0);
                    f.kira1 = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.KIRA1, 0.0);
                    f.yakit1 = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.YAKIT1, 0.0);
                    f.sskTavan = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.SSKTAVAN, 0.0);
                    f.topluGorusmeSabiti = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.TOPLUGORUSMESABITI, 0.0);
                    f.aileYardimiKatsayisi = (decimal)VeriErisim.DegerAl(r, (int)TABMaasTemelBilgi.ENUMAlanlar.AILEYARDIMIKATSAYISI, 0.0);

                    bilgi.objeler.Add(f);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return bilgi;
        }
    }
}

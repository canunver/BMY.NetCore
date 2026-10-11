using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Kur bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCKurBilgisi
    {
        /// <summary>
        /// Arguman olarak verilen, kur bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri KurBilgisi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            string hata = "";
            if (liste.objeler.Count == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00222;
            else
            {
                KurBilgisi kb = (KurBilgisi)liste.objeler[0];
                if (kb.muhasebe != -1 || kb.birim != "-1")
                {
                    if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
                    {
                        sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                        sonuc.islemSonuc = false;
                        return sonuc;
                    }
                }
            }
            
            int satir = 0;
            foreach (KurBilgisi kurBilgisi in liste.objeler)
            {
                satir++;
                if (satir == 1)
                {
                    if (kurBilgisi.yil == 0)
                        hata += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
                    if (kurBilgisi.muhasebe == 0)
                        hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
                    if (kurBilgisi.birim == "")
                        hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
                }
            }



            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKurBilgisi to = new TABKurBilgisi(ve);
            ve.BeginTransaction();

            sonuc = to.Sil((KurBilgisi)liste.objeler[0]);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, kur bilgisini veritabanından okur 
        /// ve KurBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kur bilgisi listesini KurBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, KurBilgisi kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABKurBilgisi to = new TABKurBilgisi(ve);

            if ((kriter.muhasebe != -1 && kriter.birim != "-1") || kriter.tckimlikno.Trim() != "")
            {
                IDataReader r = to.Listele(kriter);

                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        KurBilgisi kurBilgi = new KurBilgisi();

                        kurBilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                        kurBilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        kurBilgi.birim = VeriErisim.DegerAl(r, 2, "");
                        kurBilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                        kurBilgi.paraBirimi.kod = VeriErisim.DegerAl(r, 4, 0);
                        kurBilgi.kur = VeriErisim.DegerAl(r, 5, (decimal)0);
                        kurBilgi.tip = (ENUMKurBilgileriTur)VeriErisim.DegerAl(r, 6, 0);
                        kurBilgi.tckimlikno = VeriErisim.DegerAl(r, 7, "");
                        kurBilgi.paraBirimi.adi = VeriErisim.DegerAl(r, 8, "");
                        kurBilgi.paraBirimi.kisaltma = VeriErisim.DegerAl(r, 9, "");

                        bilgi.objeler.Add(kurBilgi);
                    }
                    r.Close();
                }

                ve.VeriErisimKapat();

                return bilgi;
            }
            else
            {
                ve.VeriErisimKapat();
                return bilgi;
            }
        }
    }
}

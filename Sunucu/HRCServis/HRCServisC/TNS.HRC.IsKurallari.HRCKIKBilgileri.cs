using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Kamu ihale kurumu bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCKIKBilgileri
    {
        /// <summary>
        /// Arguman olarak verilen, kamu ihale kurumu bilgisini veritabanına kaydeder 
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
            TABKIKBilgileri to = new TABKIKBilgileri(ve);
            ve.BeginTransaction();

            sonuc = to.Sil();

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
        /// Arguman olarak verilen kriterlere bağlı olarak, kamu ihale kurumu bilgisini veritabanından okur 
        /// ve KIKBilgileri nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kikBilgi">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kamu ihale kurumu bilgisi listesini KIKBilgileri nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, KIKBilgileri kikBilgi)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABKIKBilgileri to = new TABKIKBilgileri(ve);

            //365,810,510 ve 651 için bir kullanıcı adı ve parola var ve sistemde birim kodu boş olarak kayıtlı
            if (kikBilgi.birim == "365" || kikBilgi.birim == "810" || kikBilgi.birim == "510" || kikBilgi.birim == "651")
                kikBilgi.birim = "";
            IDataReader r = to.Listele(kikBilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    KIKBilgileri deger = new KIKBilgileri();

                    deger.muhasebe = VeriErisim.DegerAl(r, 0, 0);
                    deger.birim = VeriErisim.DegerAl(r, 1, "");
                    deger.kullaniciAdi = VeriErisim.DegerAl(r, 2, "");
                    deger.parola = VeriErisim.DegerAl(r, 3, "");

                    bilgi.objeler.Add(deger);
                }
            }
            r.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}

using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İdare bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIdareBilgi
    {
        /// <summary>
        /// Arguman olarak verilen, idare bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, IdareBilgi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");
            if (!TNS.MUH.Arac.Tanimla().MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim, MUH.ENUMOGY.Yazma))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (string.IsNullOrWhiteSpace(bilgi.birim))
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";

            if (bilgi.muhasebe <= 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";

            if (string.IsNullOrWhiteSpace(bilgi.adi))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00137 + "<br>";

            if (string.IsNullOrWhiteSpace(bilgi.adres))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00138 + "<br>";

            if (string.IsNullOrWhiteSpace(bilgi.telefon))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00139 + "<br>";

            if (string.IsNullOrWhiteSpace(bilgi.faks))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00140 + "<br>";

            if (string.IsNullOrWhiteSpace(bilgi.ePosta))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00141 + "<br>";

            if (string.IsNullOrWhiteSpace(bilgi.antet))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00142 + "<br>";

            if (string.IsNullOrWhiteSpace(bilgi.vNo))
                bilgi.vNo = null;

            if (TNS.DEG.Arac.DegiskenDegerBul(0, "IGNACE") == "1")
            {
                if (string.IsNullOrWhiteSpace(bilgi.IGNaceKod))
                    hata += "İş sağlığı güvenliği Nace kodunu girmelisiniz" + "<br>";
                else if (bilgi.IGNaceKod.Length != 9 || bilgi.IGNaceKod.Replace(".", "").Length != 6)
                    hata += "İş sağlığı güvenliği Nace kodunu 99.9.9.99 şeklinde girmelisiniz" + "<br>";
                else
                {
                    TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                    string ad = uzyServis.UzayDegeriStr(kullanan, "TEHLIKESINIF", bilgi.IGNaceKod, true);
                    if (string.IsNullOrWhiteSpace(ad))
                        hata += "Verilen iş sağlığı güvenliği nace kodununun karşılığı bulunamadı!" + "<br>";
                }
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIdareBilgi to = new TABIdareBilgi(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, idare bilgisini veritabanından okur 
        /// ve IdareBilgi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, idare bilgisini IdareBilgi nesnesi olarak tutan nesne</returns>
        public IdareBilgi Listele(VeriErisim veriErisim, Kullanici kullanan, IdareBilgi kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIdareBilgi to = new TABIdareBilgi(ve);
            IDataReader r = to.Listele(kriter);
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            IdareBilgi bilgi = new IdareBilgi();
            if (to.Hata() == "")
            {
                var deDeger = TNS.DEG.Arac.DegiskenDegerBul(0, "IGNACE");

                while (r.Read())
                {
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 1, "");
                    bilgi.adi = VeriErisim.DegerAl(r, 2, "");
                    bilgi.adres = VeriErisim.DegerAl(r, 3, "");
                    bilgi.telefon = VeriErisim.DegerAl(r, 4, "");
                    bilgi.faks = VeriErisim.DegerAl(r, 5, "");
                    bilgi.ePosta = VeriErisim.DegerAl(r, 6, "");
                    bilgi.antet = VeriErisim.DegerAl(r, 7, "");
                    bilgi.evrakSeri = VeriErisim.DegerAl(r, 8, "");
                    int a = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.buyuksehirMi = a % 10;
                    bilgi.argeMi = (a / 10) % 10;
                    bilgi.sicilNo = VeriErisim.DegerAl(r, 10, "");
                    bilgi.vNo = VeriErisim.DegerAl(r, 11, "");
                    bilgi.vd = VeriErisim.DegerAl(r, 12, "");
                    bilgi.IGNaceKod = OrtakFonksiyonlar.KodNoktaKoy("99.9.9.99", VeriErisim.DegerAl(r, 13, ""));
                    bilgi.iban = VeriErisim.DegerAl(r, 14, "");
                    bilgi.vergiAdi = VeriErisim.DegerAl(r, 15, "");

                    if (deDeger == "1")
                        bilgi.IGNaceAd = uzyServis.UzayDegeriStr(kullanan, "TEHLIKESINIF", bilgi.IGNaceKod, true);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return bilgi;
        }

        internal IdareBilgi KisiIdareBilgiBul(VeriErisim veriErisim, Kullanici kullanan)
        {
            TNS.PER.IPERServis perServis = TNS.PER.Arac.Tanimla();
            PER.Personel p = new PER.Personel();
            p.tcKimlikNo = kullanan.mernis;
            if (!string.IsNullOrWhiteSpace(p.tcKimlikNo))
            {
                ObjectArray oa = perServis.PersonelListele(kullanan, p);
                if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
                {
                    p = (PER.Personel)oa.objeler[0];
                    IdareBilgi ib = new IdareBilgi();
                    ib.muhasebe = p.muhasebeKodu;
                    ib.birim = p.birimKodu;
                    return Listele(veriErisim, kullanan, ib);
                }
            }
            return null;
        }
    }
}

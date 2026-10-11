using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using TNS.PER;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Personel bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    class HRCPersonel
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel bilgisini veritabanından okur 
        /// ve PersonelBilgi nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The kullanan.</param>
        /// <param name="tcKimlikNo">İşlem yapılacak TC Kimlik No bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kodu bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kodu bilgisi.</param>
        /// <returns>İstenilen kriterdeki, personel bilgisini PersonelBilgi nesnesi olarak tutan nesne</returns>
        public PersonelBilgi Listele(VeriErisim veriErisim, Kullanici islemiYapan, string tcKimlikNo, int muhasebe, string birim)
        {
            PersonelBilgi p = null;

            if (muhasebe == 0 && string.IsNullOrWhiteSpace(birim.Trim()) && string.IsNullOrWhiteSpace(tcKimlikNo))
            {
                return p;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemiYapan));

            TABPersonel tab = new TABPersonel(ve);

            try
            {
                IDataReader r = tab.Listele(tcKimlikNo, muhasebe, birim);
                if (r != null)
                {
                    if (r.Read())
                    {
                        p = new PersonelBilgi();
                        p.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                        p.adSoyad = VeriErisim.DegerAl(r, 1, "");
                        p.unvan = VeriErisim.DegerAl(r, 2, "");
                        p.muhasebe = VeriErisim.DegerAl(r, 3, 0);
                        p.birim = VeriErisim.DegerAl(r, 4, "");
                        p.banka = VeriErisim.DegerAl(r, 5, "");
                        p.sube = VeriErisim.DegerAl(r, 6, "");
                        p.unvanKodu = VeriErisim.DegerAl(r, 7, 0);
                        p.hesapNo = VeriErisim.DegerAl(r, 8, "");
                        p.vergiDairesi = VeriErisim.DegerAl(r, 9, "");
                        p.kadroDerece = VeriErisim.DegerAl(r, 10, 0);
                        p.odemeEsasDerece = VeriErisim.DegerAl(r, 11, 0);
                        p.odemeEsasGosterge = VeriErisim.DegerAl(r, 12, 0);
                        p.odemeyeEsasEkGosterge = VeriErisim.DegerAl(r, 13, 0);
                    }
                    r.Close();
                }
            }
            catch
            {
                OrtakFonksiyonlar.HataStrYaz("Personel sorgu hatası: " + ve.hata);
            }

            ve.VeriErisimKapat();

            return p;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, öğrenci eğitmen bilgisini veritabanından okur 
        /// ve PersonelBilgi nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The kullanan.</param>
        /// <param name="tcKimlikNo">İşlem yapılacak TC Kimlik No bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kodu bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kodu bilgisi.</param>
        /// <returns>İstenilen kriterdeki, personel bilgisini PersonelBilgi nesnesi olarak tutan nesne</returns>
        public PersonelBilgi ListelePersonelOgrenciEgitmen(Kullanici islemiYapan, string tcKimlikNo, int muhasebe, string birim)
        {
            PersonelBilgi p = new PersonelBilgi();

            if (muhasebe == 0 || birim.Trim() == "")
            {

            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemiYapan));

            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.PersonelOgrenciEgitmenListele(tcKimlikNo, muhasebe, birim);
            }
            catch { }

            while (r.Read())
            {

                p.muhasebe = VeriErisim.DegerAl(r, 0, 0);
                p.birim = VeriErisim.DegerAl(r, 1, "");
                p.tcKimlikNo = VeriErisim.DegerAl(r, 2, "");
                p.adSoyad = VeriErisim.DegerAl(r, 3, "");
                p.banka = VeriErisim.DegerAl(r, 4, "");
                p.sube = VeriErisim.DegerAl(r, 5, "");
                p.hesapNo = VeriErisim.DegerAl(r, 6, "");
            }

            ve.VeriErisimKapat();

            return p;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel aile bilgisini veritabanından okur 
        /// ve PersonelAileBilgi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, personel aile bilgisi listesini PersonelAileBilgi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray AileListele(PersonelAileBilgi kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.AileListele(kriter);
            }
            catch { }

            ObjectArray bilgi = new ObjectArray();
            while (r.Read())
            {
                PersonelAileBilgi d = new PersonelAileBilgi();
                d.personelTcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                d.tcKimlikNo = VeriErisim.DegerAl(r, 1, "");
                d.ad = VeriErisim.DegerAl(r, 2, "");
                d.soyad = VeriErisim.DegerAl(r, 3, "");
                d.dogumTarihi = VeriErisim.DegerAl(r, 4);
                d.cinsiyet = VeriErisim.DegerAl(r, 5, 0);
                d.yakinlikKodu = VeriErisim.DegerAl(r, 6, 0);
                d.durum = VeriErisim.DegerAl(r, 7, 0);
                d.saglikKarneNo = VeriErisim.DegerAl(r, 8, "");
                d.siraNo = VeriErisim.DegerAl(r, 9, 0);
                d.evlenmeTarihi = VeriErisim.DegerAl(r, 10);
                d.aileCuzdaniSeriNo = VeriErisim.DegerAl(r, 11, "");
                d.cocukYakinligi = VeriErisim.DegerAl(r, 12, 0);
                d.anneAdi = VeriErisim.DegerAl(r, 13, "");
                d.babaAdi = VeriErisim.DegerAl(r, 14, "");

                bilgi.objeler.Add(d);
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel sayısı bilgisini veritabanından okur 
        /// ve PersonelSayisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, personel sayısı bilgisi listesini PersonelSayisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PersonelSayisi(int muhasebe, string birim)
        {
            ObjectArray donusDeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABPersonel to = new TABPersonel(ve);
            IDataReader r = to.PersonelSayisi(muhasebe, birim);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    PersonelSayisi mb = new PersonelSayisi();

                    mb.muhasebe = VeriErisim.DegerAl(r, 0, 0);
                    mb.birim = VeriErisim.DegerAl(r, 1, "");
                    mb.personelSayisi = VeriErisim.DegerAl(r, 2, 0);

                    donusDeger.objeler.Add(mb);
                }
            }

            ve.VeriErisimKapat();

            return donusDeger;
        }
    }
}

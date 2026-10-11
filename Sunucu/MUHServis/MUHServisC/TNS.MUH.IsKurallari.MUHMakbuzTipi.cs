using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Makbuz Tipi işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHMakbuzTipiServis
    {
        /// <summary>
        /// Makbuz tipi kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipleri">makbuz tipleri</param>
        /// <returns></returns>
        public static Sonuc Kaydet(Kullanici kullanan, ObjectArray makbuzTipleri)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            if (!Yetki.SistemYoneticisimi(kullanan))
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH080;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABMakbuzTipi tMakbuz = new TABMakbuzTipi(ve);

            foreach (MakbuzTipi mt in makbuzTipleri.objeler)
            {
                string hata = string.Empty;
                if (string.IsNullOrEmpty(mt.aciklama))
                    hata = ButceMuhasebeResI.MUH100 + "<br>";
                //if (string.IsNullOrEmpty(mt.hesapKod))
                //    hata += "Hesap Kodu alanı boş bırakılamaz.<br>";
                //if (mt.kdv < 0)
                //    hata += "KDV(%) alanı boş bırakılamaz.<br>";
                //if (string.IsNullOrEmpty(mt.kdvHesapKod))
                //    hata += "KDV Hesap Kodu alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(mt.kasaHesapKod))
                    hata += ButceMuhasebeResI.MUH099 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    break;
                }

                if (mt.kod <= 0)
                    sonuc = tMakbuz.Kaydet(mt);
                else
                    sonuc = tMakbuz.Guncelle(mt);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH101;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static bool MakbuzTipiKullanilmisMi(VeriErisim ve, int makbuzTipiKod)
        {
            TABMakbuz tMakbuz = new TABMakbuz(ve);
            return tMakbuz.MakbuzTipiKullanilmisMi(makbuzTipiKod);
        }

        /// <summary>
        /// Makbuz tipi sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipleri">makbuz tipleri</param>
        /// <returns></returns>
        public static Sonuc Sil(Kullanici kullanan, ObjectArray makbuzTipleri)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            if (!Yetki.SistemYoneticisimi(kullanan))
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH080;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABMakbuzTipi tMakbuz = new TABMakbuzTipi(ve);

            foreach (MakbuzTipi mt in makbuzTipleri.objeler)
            {
                if (mt.kod > 0)
                {
                    if (!MakbuzTipiKullanilmisMi(ve, mt.kod))
                        sonuc = tMakbuz.Sil(mt);
                    else
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ButceMuhasebeResI.MUH102;
                    }
                }
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceMuhasebeResI.MUH103;
                }

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH104;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Makbuz tipi listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipi">makbuz tipi</param>
        /// <returns></returns>
        public static ObjectArray Listele(Kullanici kullanan, MakbuzTipi makbuzTipi)
        {
            ObjectArray mtler = new ObjectArray();

            string kodYapisiHesap = TNS.MUH.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD8/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMakbuzTipi tMakbuz = new TABMakbuzTipi(ve);

            IDataReader dr = tMakbuz.Listele(makbuzTipi);
            if (string.IsNullOrEmpty(tMakbuz.Hata()))
            {
                while (dr.Read())
                {
                    MakbuzTipi mt = new MakbuzTipi();
                    mt.kod = VeriErisim.DegerAl(dr, 0, 0);
                    mt.aciklama = VeriErisim.DegerAl(dr, 1, string.Empty);
                    mt.hesapKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    mt.kdv = VeriErisim.DegerAl(dr, 3, 0);
                    mt.kdvHesapKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                    mt.kasaHesapKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                    mt.yansitmaHesapKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                    mt.krediHesapKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                    mt.durum = VeriErisim.DegerAl(dr, 8, 0);

                    mt.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, mt.hesapKod);
                    mt.kdvHesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, mt.kdvHesapKod);
                    mt.kasaHesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, mt.kasaHesapKod);
                    mt.yansitmaHesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, mt.yansitmaHesapKod);
                    mt.krediHesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, mt.krediHesapKod);
                    mtler.objeler.Add(mt);
                }
            }
            else
            {
                mtler.sonuc.hataStr = tMakbuz.Hata();
                mtler.sonuc.islemSonuc = false;
            }
            dr.Close();

            ve.VeriErisimKapat();
            return mtler;
        }
    }
}
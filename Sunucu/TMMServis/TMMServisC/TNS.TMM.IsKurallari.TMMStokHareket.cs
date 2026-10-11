using System.Data;
using TNS.KYM;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır tüketim malzeme bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMStokHareket
    {
        /// <summary>
        /// Verilen kriterlere uygun olan ve aktif olarak ambarda olan henüz tüketilmemiş sarf malzemelerini listeleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bilgi">Tüketim malzemeleri kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tüketim malzemeleri listesi döndürülür.</returns>
        public static ObjectArray AmbardakiStoklariGetir(VeriErisim veriErisim, Kullanici kullanan, StokHareketBilgi bilgi)
        {
            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            ObjectArray girisler = new ObjectArray();
            ObjectArray cikislar = new ObjectArray();
            ObjectArray stoklarGecici = new ObjectArray();
            ObjectArray stoklar = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABStokHareket tabStokHareketView = new TABStokHareket(ve);

            IDataReader drGiris = tabStokHareketView.AmbardakileriGetir(bilgi, true);

            while (drGiris.Read())
            {
                StokHareketBilgi bilgiGiris = new StokHareketBilgi();

                bilgiGiris.hesapPlanKod = VeriErisim.DegerAl(drGiris, 0, string.Empty);
                bilgiGiris.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, bilgiGiris.hesapPlanKod);

                bilgiGiris.hesapPlanAd = VeriErisim.DegerAl(drGiris, 1, string.Empty);
                bilgiGiris.miktar = VeriErisim.DegerAl(drGiris, 2, (decimal)0);
                bilgiGiris.birimFiyat = VeriErisim.DegerAl(drGiris, 3, (decimal)0);
                bilgiGiris.kdvOran = VeriErisim.DegerAl(drGiris, 4, 0);
                //ErdalbilgiGiris.sonTarih = VeriErisim.DegerAl(drGiris, 5);
                bilgiGiris.olcuBirimAdi = VeriErisim.DegerAl(drGiris, 5, "");

                girisler.Ekle(bilgiGiris);
            }

            drGiris.Close();

            bilgi.fisNo = "";
            IDataReader drCikis = tabStokHareketView.AmbardakileriGetir(bilgi, false);

            while (drCikis.Read())
            {
                StokHareketBilgi bilgiCikis = new StokHareketBilgi();

                bilgiCikis.hesapPlanKod = VeriErisim.DegerAl(drCikis, 0, string.Empty);
                bilgiCikis.hesapPlanAd = VeriErisim.DegerAl(drCikis, 1, string.Empty);
                bilgiCikis.miktar = VeriErisim.DegerAl(drCikis, 2, (decimal)0);
                bilgiCikis.birimFiyat = VeriErisim.DegerAl(drCikis, 3, (decimal)0);
                bilgiCikis.kdvOran = VeriErisim.DegerAl(drCikis, 4, 0);
                //ErdalbilgiCikis.sonTarih = VeriErisim.DegerAl(drCikis, 5);
                bilgiCikis.olcuBirimAdi = VeriErisim.DegerAl(drCikis, 5, "");

                cikislar.Ekle(bilgiCikis);
            }
            drCikis.Close();

            foreach (StokHareketBilgi cikisOlayi in cikislar.objeler)
            {
                foreach (StokHareketBilgi girisOlayi in girisler.objeler)
                {
                    if (girisOlayi.hesapPlanKod.Replace(".", "") == cikisOlayi.hesapPlanKod.Replace(".", "")
                        && girisOlayi.birimFiyat == cikisOlayi.birimFiyat
                        && girisOlayi.kdvOran == cikisOlayi.kdvOran)
                    {
                        if (girisOlayi.miktar >= cikisOlayi.miktar)
                        {
                            girisOlayi.miktar -= cikisOlayi.miktar;
                            break;
                        }
                        else
                        {
                            cikisOlayi.miktar -= girisOlayi.miktar;
                            girisOlayi.miktar = 0;
                        }
                    }
                }
            }

            foreach (StokHareketBilgi giris in girisler.objeler)
            {
                stoklarGecici.Ekle(giris);
            }

            foreach (StokHareketBilgi stok in stoklarGecici.objeler)
            {
                if (stok.miktar > 0 || bilgi.stokSifirIseGetir)
                    stoklar.Ekle(stok);
            }

            if (stoklar.sonuc.islemSonuc && stoklar.objeler.Count <= 0)
                stoklar.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();

            return stoklar;
        }

        //public static Sonuc AmbardaStokVarMi(VeriErisim veriErisim, Kullanici kullanan, StokHareketBilgi bilgi)
        //{
        //    return new Sonuc();
        //}

        //public static ObjectArray AmbardakiSicilleriGetir(VeriErisim veriErisim, Kullanici kullanan, SicilHareketBilgi bilgi)
        //{
        //    ObjectArray siciller = new ObjectArray();
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

        //    TABSicilHareketView tabSicilHareketView = new TABSicilHareketView(ve);

        //    IDataReader dr = tabSicilHareketView.AmbardakileriGetir(bilgi);

        //    while (dr.Read())
        //    {
        //        SicilHareketBilgi bilgiSonuc = new SicilHareketBilgi();

        //        bilgiSonuc.hesapPlanKod = VeriErisim.DegerAl(dr, 0, string.Empty);
        //        bilgiSonuc.hesapPlanAd = VeriErisim.DegerAl(dr, 1, string.Empty);
        //        bilgiSonuc.gorSicilNo = VeriErisim.DegerAl(dr, 2, string.Empty);
        //        bilgiSonuc.kdvOran = VeriErisim.DegerAl(dr, 3, 0);
        //        bilgiSonuc.birimFiyat = VeriErisim.DegerAl(dr, 4, (decimal)0);

        //        siciller.Ekle(bilgiSonuc);
        //    }

        //    return siciller;
        //}

        //public static Sonuc AmbardaSicilVarMi(VeriErisim veriErisim, Kullanici kullanan, SicilHareketBilgi bilgi)
        //{
        //    return new Sonuc();
        //}

        //public static Sonuc SicilZimmetliMi(VeriErisim veriErisim, Kullanici kullanan, SicilHareketBilgi bilgi)
        //{
        //    return new Sonuc();
        //}
    }
}

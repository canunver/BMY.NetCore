using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Personel bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMPersonel
    {
        /// <summary>
        /// Personel varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Personel personel)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, personel.muhasebeKod, personel.harcamaBirimKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM138;
                return sonuc;
            }

            if (personel.muhasebeKod.Trim() == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM142 + "<br>";

            if (personel.harcamaBirimKod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";

            if (personel.kod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM184 + "<br>";

            if (personel.kod.Trim().Length != 11)
                sonuc.hataStr += TasinirMalResI.TMM185 + "<br>";

            if (personel.ad.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM186 + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tabPersonel = new TABPersonel(ve);
            TABHarcamaBirimi tabHarcamaBirimi = new TABHarcamaBirimi(ve);

            HarcamaBirimi hb = new HarcamaBirimi();
            hb.muhasebeKod = personel.muhasebeKod;
            hb.kod = personel.harcamaBirimKod;
            IDataReader dr = tabHarcamaBirimi.Listele(hb);
            if (!dr.Read())
            {
                dr.Close();
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM033;
                return sonuc;
            }
            dr.Close();

            //Personel p = new Personel();
            //p.kod = personel.kod;
            //dr = tabPersonel.Listele(p);
            //if (dr.Read())
            //{
            //    string muhasebe = VeriErisim.DegerAl(dr, 2, string.Empty);
            //    string harcamaKod = VeriErisim.DegerAl(dr, 3, string.Empty);
            //    string harcamaAd = VeriErisim.DegerAl(dr, 11, string.Empty);

            //    if (!(muhasebe == personel.muhasebeKod && harcamaKod == personel.harcamaBirimKod))
            //    {
            //        dr.Close();
            //        ve.VeriErisimKapat();
            //        sonuc.islemSonuc = false;
            //        sonuc.hataStr = personel.kod + " TC Kimlik nolu personel " + harcamaKod + " - " + harcamaAd + " harcama biriminde kayıtlı.";
            //        return sonuc;
            //    }
            //}
            //dr.Close();

            ve.BeginTransaction();

            sonuc = tabPersonel.Kaydet(personel);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Personel, zimmet fişlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Personel personel)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, personel.muhasebeKod, personel.harcamaBirimKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM182;
                return sonuc;
            }

            if (personel.muhasebeKod.Trim() == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM142 + "<br>";

            if (personel.harcamaBirimKod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";

            if (personel.kod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM184 + "<br>";

            //Zimmet varmı kontolü
            ObjectArray bilgi = new ObjectArray();
            ZimmetOrtakAlanVeKisi kriter = new ZimmetOrtakAlanVeKisi();
            kriter.tcKimlikNo = personel.kod;
            bilgi = TMMRaporlar.ZimmetKisi(null, kullanan, kriter, false);
            if (bilgi.objeler.Count > 0)
                sonuc.hataStr += TasinirMalResI.TMM187 + "<br>";


            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tabPersonel = new TABPersonel(ve);
            ve.BeginTransaction();

            sonuc = tabPersonel.Sil(personel);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM188, personel.kod);
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TasinirMalResI.TMM026 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan personel bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personelKriter">Personel bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel listesi döndürülür.</returns>
        public static ObjectArray Listele(Kullanici kullanan, Personel personelKriter)
        {
            string kodYapisiHarcama = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            ObjectArray bilgi = new ObjectArray();
            //bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, personelKriter.muhasebeKod, personelKriter.harcamaBirimKod);
            //if (!bilgi.sonuc.islemSonuc)
            //{
            //    bilgi.sonuc.hataStr = TasinirMalResI.TMM062;
            //    return bilgi;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tabPersonel = new TABPersonel(ve);

            IDataReader dr = tabPersonel.Listele(personelKriter);

            while (dr.Read())
            {
                Personel personel = new Personel();
                personel.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                personel.ilceKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                personel.muhasebeKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                personel.harcamaBirimKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                personel.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHarcama, personel.harcamaBirimKod);
                personel.kod = VeriErisim.DegerAl(dr, 4, string.Empty);
                personel.ad = VeriErisim.DegerAl(dr, 5, string.Empty);
                personel.unvan = VeriErisim.DegerAl(dr, 6, string.Empty);
                personel.gorev = VeriErisim.DegerAl(dr, 7, string.Empty);
                personel.telefon = VeriErisim.DegerAl(dr, 8, string.Empty);
                personel.resim = VeriErisim.DegerAl(dr, 9, string.Empty);
                personel.oda = VeriErisim.DegerAl(dr, 10, string.Empty);
                personel.eposta = VeriErisim.DegerAl(dr, 11, string.Empty);
                personel.cepTel = VeriErisim.DegerAl(dr, 12, string.Empty);
                personel.faks = VeriErisim.DegerAl(dr, 13, string.Empty);
                personel.harcamaBirimAd = VeriErisim.DegerAl(dr, 14, string.Empty);
                personel.muhasebeAd = VeriErisim.DegerAl(dr, 15, string.Empty);
                personel.il = VeriErisim.DegerAl(dr, 16, string.Empty);
                personel.ilce = VeriErisim.DegerAl(dr, 17, string.Empty);
                bilgi.Ekle(personel);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan personel sayılarını istenen bazda döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dp">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel sayıları listesi döndürülür.</returns>
        public static ObjectArray PersonelSayilari(Kullanici kullanan, DepoDurum dp)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tabPersonel = new TABPersonel(ve);

            IDataReader dr = tabPersonel.PersonelSayilari(dp);

            while (dr.Read())
            {
                string[] gelen = null;
                if (dp.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || dp.raporTur == (int)ENUMDepoDurumRaporTur.IL)
                {
                    gelen = new string[2];
                    gelen[0] = VeriErisim.DegerAl(dr, 0, string.Empty);
                    gelen[1] = VeriErisim.DegerAl(dr, 1, 0).ToString();
                }
                else if (dp.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                {
                    gelen = new string[3];
                    gelen[0] = VeriErisim.DegerAl(dr, 0, string.Empty);
                    gelen[1] = VeriErisim.DegerAl(dr, 1, string.Empty);
                    gelen[1] = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen[1]);
                    gelen[2] = VeriErisim.DegerAl(dr, 2, 0).ToString();
                }
                bilgi.Ekle(gelen);
            }

            dr.Close();
            ve.VeriErisimKapat();
            return bilgi;
        }
    }
}

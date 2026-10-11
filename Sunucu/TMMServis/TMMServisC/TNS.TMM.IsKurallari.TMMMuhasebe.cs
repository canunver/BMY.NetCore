using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Muhasebe birimi bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMMuhasebe
    {
        /// <summary>
        /// Muhasebe birimi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Muhasebe nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeBilgisi">Muhasebe birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Kullanici kullanan, Muhasebe muhasebeBilgisi)
        {
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD12/KODYAPI");

            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (muhasebeBilgisi.kod == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM013;

            Sonuc kodSonuc = OrtakFonksiyonlar.KodYapiKontrol(kodYapisi, muhasebeBilgisi.kod, true);

            if (!kodSonuc.islemSonuc)
            {
                sonuc.hataStr = kodSonuc.hataStr;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            else if (muhasebeBilgisi.ad == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM173;
            else if (muhasebeBilgisi.ilKod == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM174;
            else if (muhasebeBilgisi.ilceKod == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM175;

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMuhasebe to = new TABMuhasebe(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(muhasebeBilgisi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM176;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebe birimi, harcama birimlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Muhasebe nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeBilgisi">Muhasebe birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Kullanici kullanan, Muhasebe muhasebeBilgisi)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM071, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (muhasebeBilgisi.kod == string.Empty)
                hata = TasinirMalResI.TMM013;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMuhasebe to = new TABMuhasebe(ve);

            //Altında Harcama Birimi var mı ?
            TABHarcamaBirimi tabHarcamaBirimi = new TABHarcamaBirimi(ve);
            HarcamaBirimi hb = new HarcamaBirimi();
            hb.muhasebeKod = muhasebeBilgisi.kod;
            IDataReader drHarcamaBirimi = tabHarcamaBirimi.VarMi(hb);

            if (drHarcamaBirimi.Read()) //Var...
            {
                drHarcamaBirimi.Close();
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM177;
                ve.VeriErisimKapat();

                return sonuc;
            }
            drHarcamaBirimi.Close();

            ve.BeginTransaction();

            sonuc = to.Sil(muhasebeBilgisi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM178;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan muhasebe birimi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pMuhasebe">Muhasebe birimi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında muhasebe birimi listesi döndürülür.</returns>
        public ObjectArray Listele(Kullanici kullanan, Muhasebe pMuhasebe)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray iVAN = new ObjectArray();

            int[] muhasebeler = new int[500];
            int muhasebeSayac = 0;

            if (kullanan != null)
            {
                string yetkiTipiAdi = "";
                yetkiTipiAdi = "Taşınır Muhasebe Harcama Birim Yetkisi";
                string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

                yetkiTipiAdi = "Taşınır Kullanıcı Birimi Yetkisi";
                yetkiTipiDegeri += ";" + OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

                if (yetkiTipiDegeri.Trim() != "")
                {
                    string[] d = yetkiTipiDegeri.Split(';');

                    for (int i = 0; i < d.Length; i++)
                    {
                        string[] muhasebeBirim = d[i].Split('-');
                        if (muhasebeBirim.Length != 3 || muhasebeBirim[0] == "*")
                            continue;

                        muhasebeler[muhasebeSayac] = OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0);
                        muhasebeSayac++;
                    }
                }
            }

            TABMuhasebe to = new TABMuhasebe(ve);
            IDataReader r = to.Listele(pMuhasebe);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Muhasebe muhasebe = new Muhasebe();

                    muhasebe.kod = VeriErisim.DegerAl(r, 0, "");
                    muhasebe.ad = VeriErisim.DegerAl(r, 1, "");
                    muhasebe.ilKod = VeriErisim.DegerAl(r, 2, "");
                    muhasebe.ilceKod = VeriErisim.DegerAl(r, 3, "");
                    muhasebe.kapali = VeriErisim.DegerAl(r, 4, 0);
                    muhasebe.bolgeKod = VeriErisim.DegerAl(r, 5, 0);
                    muhasebe.kdvMukellefi = VeriErisim.DegerAl(r, 6, 0);
                    muhasebe.icmalMuaf = VeriErisim.DegerAl(r, 7, 0);

                    bool vardir = false;
                    if (muhasebeSayac > 0)
                    {
                        for (int i = 0; i < muhasebeSayac; i++)
                        {
                            if (OrtakFonksiyonlar.ConvertToInt(muhasebe.kod, 0) == muhasebeler[i])
                            {
                                vardir = true;
                                break;
                            }
                        }
                    }
                    else
                        vardir = true;

                    if (vardir)
                        iVAN.objeler.Add(muhasebe);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }

        /// <summary>
        /// Verilen muhasebe biriminin KDV mükellefi olup olmadığı bilgisini döndüren yordam
        /// </summary>
        /// <param name="kod">Muhasebe birimi kodu</param>
        /// <returns>KDV mükellefi olup olmadığı bilgisi döndürülür.</returns>
        public bool KdvMukellefiMi(string kod)
        {
            bool deger = false;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));

            TABMuhasebe to = new TABMuhasebe(ve);

            Muhasebe pMuhasebe = new Muhasebe();
            pMuhasebe.kod = kod;

            IDataReader r = to.Listele(pMuhasebe);

            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    int kdvMukellefi = VeriErisim.DegerAl(r, 6, 0);

                    if (kdvMukellefi > 0)
                        deger = true;
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return deger;
        }
    }
}

using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Yetkili imza bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMImza
    {
        /// <summary>
        /// Yetkili imza bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="imzaBilgileri">Yetkili imza bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray imzaBilgileri)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, ((ImzaBilgisi)imzaBilgileri.objeler[0]).muhasebe, ((ImzaBilgisi)imzaBilgileri.objeler[0]).birim, ((ImzaBilgisi)imzaBilgileri.objeler[0]).ambar);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM138;
                return sonuc;
            }

            string hata = Genel.KontrolMuhasebeHarcamaAmbar(((ImzaBilgisi)imzaBilgileri.objeler[0]).muhasebe, ((ImzaBilgisi)imzaBilgileri.objeler[0]).birim, ((ImzaBilgisi)imzaBilgileri.objeler[0]).ambar);
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABImza to = new TABImza(ve);

            sonuc = to.ImzaSil(((ImzaBilgisi)imzaBilgileri.objeler[0]).muhasebe, ((ImzaBilgisi)imzaBilgileri.objeler[0]).birim, ((ImzaBilgisi)imzaBilgileri.objeler[0]).ambar);

            if (sonuc.islemSonuc)
            {
                foreach (ImzaBilgisi imza in imzaBilgileri.objeler)
                {
                    sonuc = to.ImzaKaydet(imza);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM139;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan yetkili imza bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="birim">Harcama birimi kodu</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="imzaYer">ENUMImzaYer listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yetkili imza bilgileri listesi döndürülür.</returns>
        public ObjectArray Listele(Kullanici kullanan, string muhasebe, string birim, string ambar, int imzaYer)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABImza to = new TABImza(ve);

            IDataReader r = to.ImzaListele(muhasebe, birim, ambar, imzaYer);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ImzaBilgisi imza = new ImzaBilgisi();

                    imza.muhasebe = VeriErisim.DegerAl(r, 0, "");
                    imza.birim = VeriErisim.DegerAl(r, 1, "");
                    imza.ambar = VeriErisim.DegerAl(r, 2, "");
                    imza.imzaYer = VeriErisim.DegerAl(r, 3, 0);
                    imza.adSoyad = VeriErisim.DegerAl(r, 4, "");
                    imza.unvan = VeriErisim.DegerAl(r, 5, "");
                    imza.gorevUnvan = VeriErisim.DegerAl(r, 6, "");
                    imza.tarih = VeriErisim.DegerAl(r, 7, 0);
                    imza.tcKimlikNo = VeriErisim.DegerAl(r, 8, "");

                    iVAN.objeler.Add(imza);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}

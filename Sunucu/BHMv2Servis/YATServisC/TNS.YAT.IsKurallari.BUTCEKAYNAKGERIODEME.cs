using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Bütçe kaynağın geri ödeme bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class ButceKaynakGeriOdemeClass
    {
        /// <summary>
        /// Bütçe kaynağın geri ödeme bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// ButceKaynakGeriOdeme nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakOdeme">ButceKaynakGeriOdeme nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim ve, int yil, string senaryoKod, int kaynakNo, TNSCollection kaynakOdeme)
        {
            TABYATButceKaynakGeriOdeme tabKO = new TABYATButceKaynakGeriOdeme(ve);
            Sonuc donusDeger = tabKO.Sil(yil, senaryoKod, kaynakNo);
            foreach (ButceKaynakGeriOdeme ko in kaynakOdeme)
            {
                if (!donusDeger.islemSonuc)
                    break;
                if (ko.anaParaGeriOdeme > 0 || ko.odenecekFaiz > 0)
                    donusDeger = tabKO.Kaydet(yil, senaryoKod, kaynakNo, ko);
            }
            return donusDeger;
        }
 
        /// <summary>
        /// Verilen bütçe kaynağa ve yıla uygun olan geri ödeme bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">Geri ödeme bilgileri istenen bütçe kaynağın programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ButceKaynakGeriOdeme nesneleri döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim ve, int yil, string senaryoKod, int kaynakNo)
        {
            BilgiVAN donusDeger = new BilgiVAN();
 
            TABYATButceKaynakGeriOdeme tabKO = new TABYATButceKaynakGeriOdeme(ve);
            IDataReader dr = tabKO.Listele(yil, senaryoKod, kaynakNo);

            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                ButceKaynakGeriOdeme gelen = new ButceKaynakGeriOdeme();
                gelen.kaynakNo = VeriErisim.DegerAl(dr, 0, -1);
                gelen.yil = VeriErisim.DegerAl(dr, 1, 0);
                gelen.anaParaGeriOdeme = VeriErisim.DegerAl(dr, 2, 0.0);
                gelen.odenecekFaiz = VeriErisim.DegerAl(dr, 3, 0.0);
                gelen.projeninSonDurumu = VeriErisim.DegerAl(dr, 4, "");
                gelen.darBogazCozum = VeriErisim.DegerAl(dr, 5, "");

                donusDeger.liste.Add(gelen);
            }
            dr.Close();
            return donusDeger;
        }

        /// <summary>
        /// Bütçe kaynağın geri ödeme bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// ButceKaynakGeriOdeme nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakOdeme">ButceKaynakGeriOdeme nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim ve, int yil, string senaryoKod, int kaynakNo)
        {
            TABYATButceKaynakGeriOdeme tabGO = new TABYATButceKaynakGeriOdeme(ve);
            return tabGO.Sil(yil, senaryoKod, kaynakNo);
        }
    }
}
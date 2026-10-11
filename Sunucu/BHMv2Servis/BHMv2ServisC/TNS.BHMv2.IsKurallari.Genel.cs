using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.UZY;
using TNS.BKD;
using TNS.MUH;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Bütçe hazırlık iş kuralları sınıflarının ortak kullandığı yordamları tutan nesne.
    /// </summary>
    public class BHMv2Genel
    {
        private static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        /// <summary>
        /// Veri tabanına erişim için kullanılacak veri erişim bağlantı satırını döndüren yordam.
        /// Veri erişim bağlantı satırının adı, yil parametresi varsa bütçe hazırlık bağlantı satırı dizgisinin sonuna
        /// yıl parametresnin eklenmesiyle bulunur. Bu sayede bütçe hazırlık için her yılda ayrı veri tabanı kullanımı sağlanmış olur.
        /// </summary>
        /// <param name="yil">Yıla göre bağlanılacak veri tabanı bağlantı satırı dizgisi döndürmek üzere verilen yıl parametresi</param>
        /// <returns>Verilen yılı da kullanarak döndürülen bağlantı satırı nesnesi.</returns>
        public static string VeriErisimAdBul(int yil)
        {
            if (yil < 2000)
                return VERIERISIMAD;
            else
            {
                string erisimAd = VERIERISIMAD + yil.ToString();
                if (string.IsNullOrEmpty(OrtakFonksiyonlar.WebConfigOku(erisimAd, null)))
                    return VERIERISIMAD;
                return erisimAd;
            }
        }

        /// <summary>
        /// Yıldan bağımsız olan sözlük tablolarının bulunduğu veri tabanına erişimi sağlamak için kullanılacak bağlantı satırı dizgisi döndürülür.
        /// </summary>
        /// <returns></returns>
        internal static string VeriErisimAdBulSozluk()
        {
            return "BaglantiSatiriBHM";
        }

        /// <summary>
        /// Verilen kullanıcının, verilen birim ve senaryo koduna ait verileri okuma yetkisinin olup olmadığı
        /// "Bütçe Hazırlığı Yapacağı Birimler" ve "Bütçe Hazırlığı Kaydedebileceği Aşamalar" damga değerlerine bakılarak bulunur.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler</param>
        /// <param name="kurkod">Yazma yetkisinin kontrol edileceği birim kodu</param>
        /// <param name="senaryokod">Yazma yetkisinin kontrol edileceği senaryo kodu</param>
        /// <returns>Verilen birime ve senaryoya yazma yetkisi varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, string kurkod, string senaryokod)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)TNS.KYM.ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                string hazirlik = string.Empty;
                string asama = string.Empty;
                bool hazirlikontrol = false;
                bool asamakontrol = false;
                hazirlik = kullanan.YetkiTipiDegeriBul("Bütçe Hazırlığı Kaydedebileceği Birimler");
                asama = kullanan.YetkiTipiDegeriBul("Bütçe Hazırlığı Kaydedebileceği Aşamalar");

                string[] hazirlikdegerler = hazirlik.Split(';');

                foreach (string hazir in hazirlikdegerler)
                {
                    if ((kurkod.Replace(".", string.Empty).StartsWith(hazir.Replace(".", string.Empty))) && (hazir != string.Empty)) hazirlikontrol = true;
                }

                string[] asamadegerler = asama.Split(';');
                foreach (string asamalar in asamadegerler)
                {
                    if ((asamalar != string.Empty) && (OrtakFonksiyonlar.ConvertToInt(senaryokod.Substring(0, 2), 0) == OrtakFonksiyonlar.ConvertToInt(asamalar, 0))) asamakontrol = true;
                }

                if ((hazirlikontrol) && (asamakontrol)) return true;
                else return false;
            }
            else return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler</param>
        /// <param name="ve">Elde edilen sql'i çalıştırmak için kullanılacak veri erişim nesnesi. Çağrılmadan önce açılmış olmalıdır.</param>
        /// <param name="srv">Verilen uzay sorgusundan sql elde etmek için kullanılacak uzay bağlantısı nesnesi.</param>
        /// <param name="uzayAd">Uzay sorgusunun uygulanacağı uzayın ismi.</param>
        /// <param name="kosul">Uzay sorgusu</param>
        /// <param name="esitle">Uzay sorgusu alanlarının eşitlenecek mi? like ile mi bağlanacak?</param>
        /// <param name="uzayAlan">Uzay sorgusu sonucunda, select edilecek alanın ismi.</param>
        /// <returns>Uzay sorgusunun sql'e çevrilmesi ve çalıştırılması sonucu elde edilen double sonuç döndürülür.</returns>
        public static double UzayDoubleDegerGetir(Kullanici kullanan, VeriErisim ve, IUZYServis srv, string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            string sql = srv.UzayKosulStr(kullanan, uzayAd, kosul, esitle, uzayAlan);
            return ve.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Kullanıcının verilen kurumsal yapı ve aşamaya yazma hakkı olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler</param>
        /// <param name="yil">Verilen yıl ve aşama onaylıysa yazma hakkı yoktur. Bu kontrol için verilen yıl parametresi</param>
        /// <param name="kurumsalYapi">Kullanıcının verilen harcamacı birime yazma yetkisini kontrol etmek için verilen birim yapısı</param>
        /// <param name="asama">Kullanıcının verilen aşamaya yazma yetkisini kontrol etmek için verilen aşama bilgisi</param>
        /// <returns>Yazma yetkisi varsa doğru yoksa yanlış döndürülür.</returns>
        internal static bool YazmaYetkisiVarMi(Kullanici kullanan, int yil, KurumsalYapi kurumsalYapi, ENUMBHBirimAsama asama)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verilen kullanıcının, verilen birim ve senaryo koduna ait verilere yazma yetkisinin olup olmadığı
        /// "Bütçe Hazırlığı Yapacağı Birimler" ve "Bütçe Hazırlığı Kaydedebileceği Aşamalar" damga değerlerine bakılarak bulunur.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler.</param>
        /// <param name="kurkod">Kullanıcının verilen birime yazma yetkisini kontrol etmek için verilen kurumsal kod.</param>
        /// <param name="senaryokod">Kullanıcının verilen senaryoya yazma yetkisini kontrol etmek için verilen senaryo kodu.</param>
        /// <returns></returns>
        public static bool HazirlikYazmaYetkisiVarMi(Kullanici kullanan, string kurkod, string senaryokod)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)TNS.KYM.ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                string hazirlik = string.Empty;
                string asama = string.Empty;
                bool hazirlikontrol = false;
                bool asamakontrol = false;
                hazirlik = kullanan.YetkiTipiDegeriBul("Bütçe Hazırlığı Kaydedebileceği Birimler");
                asama = kullanan.YetkiTipiDegeriBul("Bütçe Hazırlığı Kaydedebileceği Aşamalar");

                string[] hazirlikdegerler = hazirlik.Split(';', ',');

                foreach (string hazir in hazirlikdegerler)
                {
                    if ((kurkod.StartsWith(hazir)) && (hazir != string.Empty)) hazirlikontrol = true;
                }

                string[] asamadegerler = asama.Split(';');
                foreach (string asamalar in asamadegerler)
                {
                    if ((asamalar != string.Empty) && (OrtakFonksiyonlar.ConvertToInt(senaryokod.Substring(0, 2), 0) == OrtakFonksiyonlar.ConvertToInt(asamalar, 0))) asamakontrol = true;
                }

                if ((hazirlikontrol) && (asamakontrol)) return true;
                else return false;
            }
            else return true;
        }
    }
}
/*
        public static string UzayYaziDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            IUZYServis uzS = TNS.UZY.Arac.Tanimla();
            string deger = string.Empty;
            if (uzayAlan == string.Empty)
                deger = uzS.UzayDegeriStr(new Kullanici(), uzayAd, kosul, esitle);
            else
                deger = uzS.UzayDegeriStr(new Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }


        public static double UzayDoubleDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            IUZYServis uzS = TNS.UZY.Arac.Tanimla();
            double deger = 0.0;
            if (uzayAlan == string.Empty)
                deger = uzS.UzayDegeriDbl(new Kullanici(), uzayAd, kosul, esitle);
            else
                deger = uzS.UzayDegeriDbl(new Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }



*/
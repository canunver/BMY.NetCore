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
    /// Yatırım projesinin ve alt projenin il ve ilçe bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class IlIlce
    {
        /// <summary>
        /// Yatırım projelerinin veya alt projelerinin il, ilçe bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bilgi">ProjeIlIlce nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim ve, int yil, string senaryoKod, int projeNo, int altProjeNo, TNSCollection bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            TABYATIlIlce tabIlIlce = new TABYATIlIlce(ve);
            donusDeger = tabIlIlce.Sil(yil, senaryoKod, projeNo, altProjeNo);
            if (bilgi != null)
            {
                foreach (ProjeIlIlce il in bilgi)
                {
                    if (donusDeger.islemSonuc)
                        donusDeger = tabIlIlce.Kaydet(yil, senaryoKod, projeNo, altProjeNo, il);
                    else
                        break;
                }
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen yatırım projesine veya alt projesine ait olan il, ilçe bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeNo">Yatırım projesinin programatik kodu</param>
        /// <param name="altProjeNo">Yatırım alt projesinin programatik kodu</param>
        /// <returns>ProjeIlIlce nesnelerinden oluşan liste döndürülür.</returns>
        public static TNSCollection Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int projeNo, int altProjeNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATIlIlce tabIlIlce = new TABYATIlIlce(ve);
            IDataReader dr = tabIlIlce.Listele(yil, senaryoKod, projeNo, altProjeNo);
            TNSCollection donusDeger = DonusDegerOl(dr);
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen yatırım projesine veya alt projesine ait olan il, ilçe bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeNo">Yatırım projesinin programatik kodu</param>
        /// <param name="altProjeNo">Yatırım alt projesinin programatik kodu</param>
        /// <returns>ProjeIlIlce nesnelerinden oluşan liste döndürülür.</returns>
        public static TNSCollection Listele(string okunanXML)
        {
            XMLStringDataReader dr = new XMLStringDataReader(okunanXML);
            return DonusDegerOl(dr);
        }

        private static TNSCollection DonusDegerOl(IDataReader dr)
        {
            TNSCollection donusDeger = new TNSCollection();
            if (dr != null)
            {
                while (dr.Read())
                {
                    ProjeIlIlce ilIlce = new ProjeIlIlce();
                    ilIlce.projeNo = VeriErisim.DegerAl(dr, 0, 0);
                    ilIlce.altProjeNo = VeriErisim.DegerAl(dr, 1, 0);
                    ilIlce.ilIlce.il.kod = VeriErisim.DegerAl(dr, 2, 0);
                    ilIlce.ilIlce.ilceKodu = VeriErisim.DegerAl(dr, 3, 0);
                    ilIlce.ilIlce.ilceAdi = VeriErisim.DegerAl(dr, 4, "");
                    ilIlce.ilIlce.il.ad = VeriErisim.DegerAl(dr, 5, "");
                    ilIlce.ilIlce.il.aciklama = VeriErisim.DegerAl(dr, 6, "");

                    donusDeger.Add(ilIlce);
                }
                dr.Close();
            }
            return donusDeger;
        }
    }
}
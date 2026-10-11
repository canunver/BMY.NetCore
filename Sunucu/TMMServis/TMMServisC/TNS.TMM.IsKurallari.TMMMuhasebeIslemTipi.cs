using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Ölçü birimi bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMMuhasebeIslemTipi
    {
        /// <summary>
        /// Ölçü birim varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen MuhasebeIslemTipi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeIslemTip">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemTipi muhasebeIslemTip)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (muhasebeIslemTip.tip == "")
            {
                if (hata != "") hata += "<br>";
                hata += "Tip alanı boş bırakılamaz";
            }

            if (muhasebeIslemTip.ad == "")
            {
                if (hata != "") hata += "<br>";
                hata += TasinirMalResI.TMM075;
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMuhasebeIslemTipi tabMuhasebeIslemTipi = new TABMuhasebeIslemTipi(ve);
            ve.BeginTransaction();

            try
            {
                if (muhasebeIslemTip.kod=="")
                {
                    muhasebeIslemTip.kod = Guid.NewGuid().ToString();
                    sonuc = tabMuhasebeIslemTipi.Kaydet(muhasebeIslemTip);
                }
                else
                sonuc = tabMuhasebeIslemTipi.Guncelle(muhasebeIslemTip);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Ölçü birim, taşınır hesap planında kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen MuhasebeIslemTipi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeIslemTip">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemTipi muhasebeIslemTip)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM071, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (muhasebeIslemTip.kod == "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM074;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMuhasebeIslemTipi tabMuhasebeIslemTipi = new TABMuhasebeIslemTipi(ve);
            ve.BeginTransaction();

            try
            {
                //if (Genel.IslemTipKullanilmisMi(ve, islemTip.kod))
                //{
                //    sonuc.islemSonuc = false;
                //    sonuc.hataStr = "Bu işlem tipi daha önce kullanıldığı için silinemez!";
                //}
                //else
                sonuc = tabMuhasebeIslemTipi.Sil(muhasebeIslemTip);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeIslemTip">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ölçü birim listesi döndürülür.</returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemTipi muhasebeIslemTip)
        {
            ObjectArray listeVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMuhasebeIslemTipi tabMuhasebeIslemTipi = new TABMuhasebeIslemTipi(ve);
            IDataReader dr = null;

            try
            {
                dr = tabMuhasebeIslemTipi.Listele(muhasebeIslemTip);
            }
            catch (Exception e)
            {
                listeVAN.sonuc.islemSonuc = false;
                listeVAN.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return listeVAN;
            }

            while (dr.Read())
            {
                MuhasebeIslemTipi gelen = new MuhasebeIslemTipi();
                gelen.kod = VeriErisim.DegerAl(dr, 0, "");
                gelen.tip = VeriErisim.DegerAl(dr, 1, "");
                gelen.ad = VeriErisim.DegerAl(dr, 2, "");

                listeVAN.objeler.Add(gelen);
            }


            if (listeVAN.objeler.Count == 0)
                listeVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            listeVAN.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return listeVAN;
        }
    }
}

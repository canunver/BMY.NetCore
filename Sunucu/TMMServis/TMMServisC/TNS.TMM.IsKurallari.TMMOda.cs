using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Oda bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMOda
    {
        /// <summary>
        /// Oda varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Oda nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="oda">Oda bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Oda oda)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, oda.muhasebeKod, oda.harcamaBirimKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM138;
                return sonuc;
            }

            if (oda.muhasebeKod.Trim() == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM142 + "<br>";

            if (oda.harcamaBirimKod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";

            if (oda.kod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM179 + "<br>";

            if (oda.ad.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM180 + "<br>";

            if (TNS.DEG.Arac.TKDKMi())
            {
                if (!oda.kod.StartsWith(oda.harcamaBirimKod.Replace(".", "") + "_"))
                    sonuc.hataStr += "Oda kodu harcama birimi kodu (noktasız olarak) ile başlamalı sonrasinda _ (alt cizgi) konulup sira numarası verilmelidir" + "<br>";
            }

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOda tabOda = new TABOda(ve);

            if (oda.ambarKod != "")
            {
                TABAmbar tabAmbar = new TABAmbar(ve);
                Ambar ab = new Ambar();
                ab.muhasebeKod = oda.muhasebeKod;
                ab.harcamaBirimKod = oda.harcamaBirimKod;
                ab.kod = oda.ambarKod;
                IDataReader dr = tabAmbar.Listele(ab);
                if (!dr.Read())
                {
                    dr.Close();
                    ve.VeriErisimKapat();
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM181;
                    return sonuc;
                }
                dr.Close();
            }
            else
            {
                TABHarcamaBirimi tabHarcamaBirimi = new TABHarcamaBirimi(ve);
                HarcamaBirimi hb = new HarcamaBirimi();
                hb.muhasebeKod = oda.muhasebeKod;
                hb.kod = oda.harcamaBirimKod;
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
            }
            ve.BeginTransaction();

            sonuc = tabOda.Kaydet(oda);

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
        /// Parametre olarak verilen oda bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Oda nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="oda">Oda bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Oda oda)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, oda.muhasebeKod, oda.harcamaBirimKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM182;
                return sonuc;
            }

            if (oda.muhasebeKod.Trim() == string.Empty)
                sonuc.hataStr = TasinirMalResI.TMM142 + "<br>";

            if (oda.harcamaBirimKod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";

            if (oda.kod.Trim() == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM179 + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOda tabOda = new TABOda(ve);

            if (tabOda.KayitVarMi(oda) > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = oda.kod + " oda kodu kullanımda olduğu için silinemez.";
            }

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                sonuc = tabOda.Sil(oda);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = string.Format(TasinirMalResI.TMM183, oda.kod);
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = TasinirMalResI.TMM026 + "<br>" + sonuc.hataStr;
                }
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan oda bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="odaKriter">Oda bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında oda listesi döndürülür.</returns>
        public static ObjectArray Listele(Kullanici kullanan, Oda odaKriter)
        {
            string kodYapisiHarcama = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            ObjectArray bilgi = new ObjectArray();
            //bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, odaKriter.muhasebeKod, odaKriter.harcamaBirimKod);
            //if (!bilgi.sonuc.islemSonuc)
            //{
            //    bilgi.sonuc.hataStr = TasinirMalResI.TMM062;
            //    return bilgi;
            //}

            var merkezBankasi = TMM.Arac.MerkezBankasiKullaniyor();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOda tabOda = new TABOda(ve);

            IDataReader dr = tabOda.Listele(odaKriter, merkezBankasi);

            while (dr.Read())
            {
                Oda oda = new Oda();
                oda.ilKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                oda.ilceKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                oda.muhasebeKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                oda.harcamaBirimKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                oda.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHarcama, oda.harcamaBirimKod);
                oda.kod = VeriErisim.DegerAl(dr, 4, string.Empty);
                oda.ad = VeriErisim.DegerAl(dr, 5, string.Empty);
                oda.harcamaBirimAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                oda.muhasebeAd = VeriErisim.DegerAl(dr, 7, string.Empty);
                oda.il = VeriErisim.DegerAl(dr, 8, string.Empty);
                oda.ilce = VeriErisim.DegerAl(dr, 9, string.Empty);
                oda.ambarKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                oda.ustKod = VeriErisim.DegerAl(dr, 11, 0);
                if (merkezBankasi)
                {
                    oda.siraNo = VeriErisim.DegerAl(dr, 12, 0);
                    oda.kod2 = VeriErisim.DegerAl(dr, 13, string.Empty);//GORKOD ATANIYOR
                }
                bilgi.Ekle(oda);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Silinen ancak kullanımda olan oda bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında oda listesi döndürülür.</returns>
        public static ObjectArray ListeleSilinenOdalar(VeriErisim veriErisim, Kullanici kullanan)
        {
            string kodYapisiHarcama = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOda tabOda = new TABOda(ve);

            IDataReader dr = tabOda.ListeleSilinenler();

            if (dr != null)
            {
                while (dr.Read())
                {
                    Oda oda = new Oda();
                    oda.muhasebeKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    oda.harcamaBirimKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    oda.kod = VeriErisim.DegerAl(dr, 2, string.Empty);

                    oda.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHarcama, oda.harcamaBirimKod);

                    bilgi.Ekle(oda);
                }

                dr.Close();
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Silinen ancak kullanımda olan odalar varsa yeniden oda kodu oluşturan yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Oda nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KaydetSilinenOdalar(VeriErisim veriErisim, Kullanici kullanan)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOda tabOda = new TABOda(ve);


            ObjectArray odalar = ListeleSilinenOdalar(ve, kullanan);

            if (odalar.objeler.Count > 0)
            {
                ve.BeginTransaction();

                foreach (Oda oda in odalar.objeler)
                {
                    sonuc = tabOda.Kaydet(oda);
                    if (!sonuc.islemSonuc)
                        break;
                }

                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = TasinirMalResI.TMM118 + "<br>" + "Kaydedilen oda sayısı: " + odalar.objeler;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Kaydedilecek oda bulunamadı.";
            }


            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

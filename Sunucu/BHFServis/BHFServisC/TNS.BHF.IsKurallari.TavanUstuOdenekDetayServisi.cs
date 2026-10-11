using System;
using TNS;
using TNS.KYM;
using TNS.BHF;
using TNS.Veri;
using System.Data;
using TNS.BHF.IsKurallari.Veri;


namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Tavan üstüne sığmayan ödeneklerin 03.2 ve 03.5 ekonomik kodlu olanlarının detaylandırılmasıyla ilgili sunucu yordamlarını tutan sınıf.
    /// </summary>
    public class TavanUstuOdenekDetayServisi
    {
        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin 03.2 ve 03.5 ekonomik kodlu olanlarının detaylandırıldığı bilgileri listeleyen yordam.
        /// Kullanıcının birime ait verileri görem yetkisi varsa, veri tabanından okunan detay bazlı tavan üstü bilgileri listeye doldurulur ve döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin bilgileri satırlar halinde döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            string kurum = TNS.OrtakFonksiyonlar.KurumsaldanKuruma(kurumsalKod, "");
            IDataReader r;
            TavanUstuOdenek tavanUstu;
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc.islemSonuc = false;

            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                kurum = kurum.Replace(".", "");
                kurumsalKod = kurumsalKod.Replace(".", "");
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABTavanUstuOdenekDetay tabtavanUstu = new TABTavanUstuOdenekDetay(ve);

                r = tabtavanUstu.Listele(yil, kurum, kurumsalKod);

                if (tabtavanUstu.Hata() == "")
                {
                    while (r.Read())
                    {
                        tavanUstu = new TavanUstuOdenek();
                        tavanUstu.kurumsalKod = kurumsalKod;
                        tavanUstu.ekokod = VeriErisim.DegerAl(r, 0, "");
                        tavanUstu.odenek = VeriErisim.DegerAl(r, 1, (double)0);
                        tavanUstu.aciklama = ""; // VeriErisim.DegerAl(r, 5, "");

                        donusDeger.Ekle(tavanUstu);
                    }
                    r.Close();
                    donusDeger.sonuc.islemSonuc = true;
                }
                else
                {
                    donusDeger.sonuc.DegerAta(tabtavanUstu.Hata());
                }

                if (donusDeger.sonuc.islemSonuc)
                {
                    foreach (TavanUstuOdenek tuo in donusDeger.objeler)
                    {
                        r = tabtavanUstu.GerekceListele(yil, kurum, tuo.kurumsalKod, tuo.ekokod);
                        if (r != null)
                        {
                            string aciklama = "";
                            while (r.Read())
                            {
                                if (aciklama != "") aciklama += "\r\n";
                                aciklama += VeriErisim.DegerAl(r, 1, "");
                            }
                            r.Close();
                            tuo.aciklama = aciklama;
                        }
                    }
                }

                ve.VeriErisimKapat();
            }
            else
            {
                donusDeger.sonuc.DegerAta(ButceFormResI.BHMTUODS001);
            }

            return donusDeger;
        }

        /// <summary>
        /// Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin detaylandırıldığı bilgileri kaydeden yordam.
        /// Kullanıcının birime ait verileri kaydetme yetkisi varsa, önce birimin eski verileri silinir sonra, TavanUstuOdenekSatirlari listesindeki her bir satır veri tabanına eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="TavanUstuOdenekSatirlari">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin miktarlarını ve açıklamalarını tutan satırların dizisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, TNSCollection TavanUstuOdenekSatirlari)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string kurum = TNS.OrtakFonksiyonlar.KurumsaldanKuruma(kurumsalKod, "");
            if (Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
            {
                kurum = kurum.Replace(".", "");
                kurumsalKod = kurumsalKod.Replace(".", "");
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

                TABTavanUstuOdenekDetay tabTavanUstuOdenek = new TABTavanUstuOdenekDetay(ve);
                ve.BeginTransaction();

                sonuc = tabTavanUstuOdenek.Sil(yil, kurum, kurumsalKod);
                if (sonuc.islemSonuc)
                {
                    TavanUstuOdenek TavanUstuOdenek;

                    for (int i = 0; i < TavanUstuOdenekSatirlari.Count; i++)
                    {
                        TavanUstuOdenek = (TavanUstuOdenek)TavanUstuOdenekSatirlari[i];

                        sonuc = tabTavanUstuOdenek.Ekle(yil, kurum, kurumsalKod, TavanUstuOdenek.ekokod, TavanUstuOdenek.odenek, TavanUstuOdenek.aciklama);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }
            else
                sonuc.DegerAta(ButceFormResI.BHMTUODS002);
            return sonuc;
        }
    }
}

using System;
using TNS;
using TNS.KYM;
using TNS.BHF;
using TNS.Veri;
using System.Data;
using TNS.BHF.IsKurallari.Veri;
using TNS.BHMv2;
using TNS.BKD;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Tavan üstüne sığmayan ödeneklerin, açıklamalarıyla birlikte işlemlerinin yapıldığı yordamları tutan sınıf.
    /// </summary>
    public class TavanUstuOdenekServisi
    {
        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin, açıklamalarıyla birlikte listelendiği yordam.
        /// Kullanıcının birime ait verileri görme yetkisi varsa, veri tabanından okunan tavan üstü bilgileri listeye doldurulur ve döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan ödeneklerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan ödeneklerin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Tavan üstüne sığmayan kullanılan ödeneklerin bilgileri satırlar halinde döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            IDataReader r;
            TavanUstuOdenek tavanUstu;
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc.islemSonuc = false;
            string kurum = TNS.OrtakFonksiyonlar.KurumsaldanKuruma(kurumsalKod, "");
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                kurum = kurum.Replace(".", "");
                kurumsalKod = kurumsalKod.Replace(".", "");
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABTavanUstuOdenek tabtavanUstu = new TABTavanUstuOdenek(ve);

                r = tabtavanUstu.Listele(yil, kurum, kurumsalKod);
                string eskiKur = "";
                if (tabtavanUstu.Hata() == "")
                {
                    while (r.Read())
                    {
                        tavanUstu = new TavanUstuOdenek();
                        tavanUstu.kurumsalKod = VeriErisim.DegerAl(r, 2, "");
                        tavanUstu.ekokod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(3), VeriErisim.DegerAl(r, 3, ""));
                        if (eskiKur != tavanUstu.kurumsalKod)
                        {
                            eskiKur = tavanUstu.kurumsalKod;
                            tavanUstu.aciklama = VeriErisim.DegerAl(r, 4, "");
                        }
                        else
                            tavanUstu.aciklama = "";
                        tavanUstu.odenek = VeriErisim.DegerAl(r, 5, (double)0);

                        donusDeger.Ekle(tavanUstu);
                    }
                    r.Close();
                    donusDeger.sonuc.islemSonuc = true;
                }
                else
                {
                    donusDeger.sonuc.hataStr = tabtavanUstu.Hata();
                    donusDeger.sonuc.islemSonuc = false;
                }
                ve.VeriErisimKapat();
            }
            else
                donusDeger.sonuc.DegerAta(ButceFormResI.BHMTUOS001);

            return donusDeger;
        }

        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin, açıklamalarıyla birlikte kaydedildiği yordam.
        /// Kullanıcının yetkisi varsa, bir birime ait tavan üstü ödenek bilgileri kaydedilir. Birim için tek bir açıklama girildiğinden, açıklama alanı dolu olan son kaydın açıklama verisi açıklama olarak kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="yil">Tavan üstüne sığmayan ödeneklerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan ödeneklerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="TavanUstuOdenekSatirlari">Kaydedilecek, açıklama ve ekonomik kodları tutan satır dizisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, TNSCollection TavanUstuOdenekSatirlari)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string kurum = TNS.OrtakFonksiyonlar.KurumsaldanKuruma(kurumsalKod, "");
            if (Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
            {
                kurumsalKod = kurumsalKod.Replace(".", "");
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

                TABTavanUstuOdenek tabTavanUstuOdenek = new TABTavanUstuOdenek(ve);
                ve.BeginTransaction();

                sonuc = tabTavanUstuOdenek.OdenekSil(yil, kurum, kurumsalKod);
                if (sonuc.islemSonuc)
                    sonuc = tabTavanUstuOdenek.AciklamaSil(yil, kurumsalKod);
                if (sonuc.islemSonuc)
                {
                    TavanUstuOdenek TavanUstuOdenek;
                    string aciklama = "";
                    for (int i = 0; i < TavanUstuOdenekSatirlari.Count; i++)
                    {
                        TavanUstuOdenek = (TavanUstuOdenek)TavanUstuOdenekSatirlari[i];

                        sonuc = tabTavanUstuOdenek.OdenekEkle(yil, kurum, kurumsalKod, TavanUstuOdenek.ekokod, TavanUstuOdenek.odenek);
                        if (!sonuc.islemSonuc)
                            break;
                        if (!string.IsNullOrEmpty(TavanUstuOdenek.aciklama))
                            aciklama = TavanUstuOdenek.aciklama;
                    }
                    //if (!string.IsNullOrEmpty(aciklama) && sonuc.islemSonuc)
                    sonuc = tabTavanUstuOdenek.AciklamaEkle(yil, kurumsalKod, aciklama);
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }
            else
                sonuc.DegerAta(ButceFormResI.BHMTUOS002);
            return sonuc;
        }

        internal static ObjectArray ListeleDetaydan(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, string senaryoKod, bool YirmiBes2)
        {
            ObjectArray donusDeger = new ObjectArray();
            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                BHMv2.KistasYapi kistasYapi = new BHMv2.KistasYapi();
                kistasYapi.ekleTertipler = new BKD.Tertip[1];
                kistasYapi.ekleTertipler[0] = new BKD.Tertip(yil, BKD.ENUMTabloKod.TABPEBOdenek, kurumsalKod);
                try
                {
                    IBHMv2Servis bhmServis = bhmServis = TNS.BHMv2.Arac.Tanimla();
                    BHMv2.TavanUstuTertipOdenek[] tuto = bhmServis.TavanUstuOdenekListele(kullanan, new string[] { senaryoKod }, kistasYapi, true);

                    for (int i = 0; i < tuto.Length; i++)
                    {
                        if (tuto[i].ekOdenek0 != 0)
                        {
                            string kurKod = tuto[i].tertip.kodlar[0];
                            string ekoKod = TavanUstuEkonomikSeviye(tuto[i].tertip.kodlar[3], YirmiBes2);
                            bool ekle = true;
                            if (YirmiBes2)
                            {
                                kurKod = kurumsalKod;
                                string kEko = ekoKod.Replace(".", "");
                                if (kEko == "032201" || kEko == "032301" || kEko == "032302" || kEko == "032303" || kEko == "035108" || kEko == "035109" || kEko == "035502" || kEko == "035505" ||
                                    kEko == "03022001" || kEko == "03023001" || kEko == "03023002" || kEko == "03023003" || kEko == "03051008" || kEko == "03051009" || kEko == "03055002" || kEko == "03055005")
                                    ekle = true;
                                else
                                    ekle = false;
                            }

                            if (ekle)
                            {
                                TavanUstuOdenek tavanUstu = TavanUstuYerBul(donusDeger, kurKod, ekoKod);
                                if (tavanUstu == null)
                                {
                                    tavanUstu = new TavanUstuOdenek();
                                    tavanUstu.kurumsalKod = kurKod;
                                    tavanUstu.ekokod = ekoKod;
                                    tavanUstu.aciklama = tuto[i].gerekce + "";
                                    tavanUstu.odenek = tuto[i].ekOdenek0;
                                    donusDeger.Ekle(tavanUstu);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(tuto[i].gerekce))
                                    {
                                        if (!string.IsNullOrEmpty(tavanUstu.aciklama)) tavanUstu.aciklama += "\r\n";
                                        tavanUstu.aciklama += tuto[i].gerekce;
                                    }
                                    tavanUstu.odenek += tuto[i].ekOdenek0;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    donusDeger.sonuc.DegerAta(ex.Message);
                }
            }
            else
                donusDeger.sonuc.DegerAta(ButceFormResI.BHMTUOS001);

            return donusDeger;
        }

        private static string TavanUstuEkonomikSeviye(string ekoKod, bool YirmiBes2)
        {
            if (YirmiBes2)
            {
                return ekoKod;
            }
            else
            {
                int uz;
                if (ekoKod.StartsWith("03"))
                {
                    uz = 2;
                }
                else
                    uz = 1;
                TNS.BKD.ABSKod k = new BKD.ABSKod(ekoKod);
                return k.KodParcaAl(uz);
            }
        }

        private static TavanUstuOdenek TavanUstuYerBul(ObjectArray donusDeger, string kurumsalKod)
        {
            if (donusDeger == null || string.IsNullOrEmpty(kurumsalKod))
                return null;
            foreach (TavanUstuOdenek tuo in donusDeger.objeler)
            {
                if (tuo.kurumsalKod == kurumsalKod)
                    return tuo;
            }
            return null;
        }

        private static TavanUstuOdenek TavanUstuYerBul(ObjectArray donusDeger, string kurumsalKod, string ekoKod)
        {
            if (donusDeger == null || string.IsNullOrEmpty(kurumsalKod) || string.IsNullOrEmpty(ekoKod))
                return null;
            foreach (TavanUstuOdenek tuo in donusDeger.objeler)
            {
                if (tuo.kurumsalKod == kurumsalKod && tuo.ekokod == ekoKod)
                    return tuo;
            }
            return null;
        }
    }
}

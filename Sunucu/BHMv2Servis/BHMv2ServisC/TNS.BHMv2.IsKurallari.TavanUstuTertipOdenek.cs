using System;
using System.Data;
using System.Collections;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// BHMv2TavanUstuOdenek formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class TavanUstuTertipOdenekHizmet
    {
        /// <summary>
        ///  nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="senaryoKod">Ödenek üstünün ait olduğu senaryo.</param>
        /// <param name="odenekler">Ödenek üstü bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, string senaryoKod, TavanUstuTertipOdenek[] odenekler)
        {
            Sonuc s = new Sonuc(true, "", "");

            int yil = -1;
            if (odenekler.Length > 0) yil = odenekler[0].tertip.yil;

            odenekler = OdenekleriTamamla(veriErisim, kullanan, senaryoKod, odenekler);

            for (int i = 0; i < odenekler.Length; i++)
            {
                if (!YetkiServis.OGYYetkisiVarMi(kullanan, senaryoKod, odenekler[i].tertip, ENUMOGY.Yazma))
                {
                    s.DegerAta("Yazma yetkiniz yok!");
                }

                if (odenekler[i].ekOdenek0 < 0)
                    s.DegerAta("Ödenek sıfırdan küçük olamaz!");

                if (odenekler[i].ekOdenek0 > 0 && string.IsNullOrEmpty(odenekler[i].gerekce))
                    s.DegerAta("Açıklama alanı boş bırakılamaz!");
            }

            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMTAVANUSTU tabBHMTAVANUSTU = new TABBHMTAVANUSTU(ve);

            ve.BeginTransaction();

            for (int i = 0; i < odenekler.Length && s.islemSonuc; i++)
            {
                s = tabBHMTAVANUSTU.Sil(odenekler[i].tertip, senaryoKod);
                if (s.islemSonuc && odenekler[i].ekOdenek0 > 0)
                    s = tabBHMTAVANUSTU.Ekle(odenekler[i].tertip, senaryoKod, odenekler[i].ekOdenek0, odenekler[i].ekOdenek1, odenekler[i].ekOdenek2, odenekler[i].gerekce);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static TavanUstuTertipOdenek[] OdenekleriTamamla(VeriErisim veriErisim, Kullanici kullanan, string senaryoKod, TavanUstuTertipOdenek[] odenekler)
        {
            if (odenekler == null || odenekler.Length == 0) return odenekler;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(odenekler[0].tertip.yil), null);
            string[] kaynakSenaryo = new string[1];
            ArrayList al = new ArrayList();

            for (int i = 0; i < odenekler.Length; i++)
            {
                TavanUstuTertipOdenek to = odenekler[i];
                if (to.tertip.TamMi())
                {
                    al.Add(to);
                }
                else
                {
                    bool dagildi = false;
                    for (int yontem = 0; yontem < 3; yontem++)
                    {
                        bool tanimliTertipler = false;
                        if (yontem == 0 || yontem == 2)
                        {
                            kaynakSenaryo[0] = senaryoKod;
                            if (yontem == 2)
                                tanimliTertipler = true;
                        }
                        else
                        {
                            string senOnParca = senaryoKod.Substring(0, 2);
                            int senOnParcaInt = OrtakFonksiyonlar.ConvertToInt(senOnParca, 0);
                            kaynakSenaryo[0] = (senOnParcaInt - 1).ToString().PadLeft(2, '0') + ".00";
                        }

                        KistasYapi kistas = new KistasYapi();
                        kistas.ekleTertipler = new Tertip[1];
                        kistas.ekleTertipler[0] = to.tertip;
                        TavanUstuTertipOdenek[] dizi = Listele(veriErisim, kullanan, kaynakSenaryo, kistas, tanimliTertipler, false);
                        if (dizi == null || dizi.Length == 0)
                        {
                            continue;
                        }
                        if (yontem == 2)
                        {
                            for (int j = 0; j < dizi.Length; j++)
                            {
                                if (dizi[j].odenek0 > 0)
                                {
                                    dizi[j].ekOdenek0 = to.ekOdenek0;
                                    dizi[j].ekOdenek1 = to.ekOdenek1;
                                    dizi[j].ekOdenek2 = to.ekOdenek2;
                                    dizi[j].gerekce = to.gerekce;
                                    dagildi = true;
                                    al.Add(dizi[j]);
                                    break;
                                }
                            }
                            if (!dagildi && dizi.Length > 0)
                            {
                                dizi[0].ekOdenek0 = to.ekOdenek0;
                                dizi[0].ekOdenek1 = to.ekOdenek1;
                                dizi[0].ekOdenek2 = to.ekOdenek2;
                                dizi[0].gerekce = to.gerekce;
                                al.Add(dizi[0]);
                                dagildi = true;
                            }
                        }
                        else
                        {
                            double toplam0 = 0, toplam1 = 0, toplam2 = 0;
                            double yToplam0 = 0, yToplam1 = 0, yToplam2 = 0;
                            for (int j = 0; j < dizi.Length; j++)
                            {
                                toplam0 += dizi[j].ekOdenek0;
                                toplam1 += dizi[j].ekOdenek1;
                                toplam2 += dizi[j].ekOdenek2;
                            }
                            if (toplam0 == 0 && to.ekOdenek0 > 0 || toplam1 == 0 && to.ekOdenek1 > 0 || toplam2 == 0 && to.ekOdenek2 > 0)
                            {
                                continue;
                            }

                            for (int j = 0; j < dizi.Length - 1; j++)
                            {
                                yToplam0 += (dizi[j].ekOdenek0 = Hesapla(dizi[j].ekOdenek0, toplam0, to.ekOdenek0));
                                yToplam1 += (dizi[j].ekOdenek1 = Hesapla(dizi[j].ekOdenek1, toplam1, to.ekOdenek1));
                                yToplam2 += (dizi[j].ekOdenek2 = Hesapla(dizi[j].ekOdenek2, toplam2, to.ekOdenek2));
                            }
                            int son = dizi.Length - 1;
                            dizi[son].ekOdenek0 = to.ekOdenek0 - yToplam0;
                            dizi[son].ekOdenek1 = to.ekOdenek1 - yToplam1;
                            dizi[son].ekOdenek2 = to.ekOdenek2 - yToplam2;
                            for (int j = 0; j < dizi.Length; j++)
                            {
                                al.Add(dizi[j]);
                            }
                            dagildi = true;
                            break;
                        }
                    }
                    if (!dagildi)
                    {
                        ve.VeriErisimKapat();
                        throw new Exception(string.Format("{0} tertibine girilen ek ödenek tutarı dağıtılamadı lütfen detaylara veri girişi yapınız!", to.tertip.TertipStrOl()));
                    }
                }
            }
            ve.VeriErisimKapat();
            return (TavanUstuTertipOdenek[])al.ToArray(typeof(TavanUstuTertipOdenek));
        }

        private static double Hesapla(double eski, double eskiToplam, double yeniToplam)
        {
            if (yeniToplam == 0) return 0;
            return Math.Floor(eski * (yeniToplam / eskiToplam));
        }

        /// <summary>
        ///  nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tertip">Kıstas tertip bilgisini tutan değişken</param>
        public static TavanUstuTertipOdenek[] Listele(VeriErisim veriErisim, Kullanici kullanan, string[] senaryoKod, KistasYapi tertip, bool tanimliTertipler, bool butceFormdan)
        {
            if (!butceFormdan)
            {
                if (!YetkiServis.OGYYetkisiVarMi(kullanan, senaryoKod[0], tertip.ekleTertipler[0], ENUMOGY.Gorme))
                {
                    throw new Exception("Görme yetkiniz yok!");
                }
            }

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            for (int i = 0; i < tertip.ekleTertipler[0].kodlar.Length; i++)
            {
                kodBilgileri.Add(new UcgenKodBilgi(tertip.ekleTertipler[0].KodNolar(i), tertip.ekleTertipler[0].KodNoKirilimSay(i)));
            }

            EkDegerParam[] ekBilgiler = new EkDegerParam[senaryoKod.Length * 3];
            for (int i = 0; i < senaryoKod.Length; i++)
            {
                ekBilgiler[3 * i + 0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod[i]);
                ekBilgiler[3 * i + 1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 1, senaryoKod[i]);
                ekBilgiler[3 * i + 2] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 2, senaryoKod[i]);
            }
            OdenekBilgi[] ob = UcgenServisi.ListeTertipOl(veriErisim, kullanan, tertip, kodBilgileri, ekBilgiler, true, tanimliTertipler, true, -1);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(tertip.ekleTertipler[0].yil), null);
            TABBHMTAVANUSTU tabTalep = new TABBHMTAVANUSTU(ve);

            TavanUstuTertipOdenek[] donusDeger = new TavanUstuTertipOdenek[ob.Length];
            for (int i = 0; i < ob.Length; i++)
            {
                donusDeger[i] = new TavanUstuTertipOdenek(senaryoKod.Length);
                donusDeger[i].tertip = ob[i].tertip;
                for (int j = 0; j < senaryoKod.Length; j++)
                {
                    donusDeger[i].senaryoOdenegi[3 * j + 0] = (double)ob[i].ekRakamlar[3 * j + 0];
                    donusDeger[i].senaryoOdenegi[3 * j + 1] = (double)ob[i].ekRakamlar[3 * j + 1];
                    donusDeger[i].senaryoOdenegi[3 * j + 2] = (double)ob[i].ekRakamlar[3 * j + 2];
                }
                IDataReader dr = tabTalep.Listele(donusDeger[i].tertip, senaryoKod[0]);
                if (dr != null)
                {
                    if (dr.Read())
                    {
                        donusDeger[i].ekOdenek0 = VeriErisim.DegerAl(dr, (int)TABBHMTAVANUSTU.ENUMAlanlar.ODENEK0, 0.0);
                        donusDeger[i].ekOdenek1 = VeriErisim.DegerAl(dr, (int)TABBHMTAVANUSTU.ENUMAlanlar.ODENEK1, 0.0);
                        donusDeger[i].ekOdenek2 = VeriErisim.DegerAl(dr, (int)TABBHMTAVANUSTU.ENUMAlanlar.ODENEK2, 0.0);
                        donusDeger[i].gerekce = VeriErisim.DegerAl(dr, (int)TABBHMTAVANUSTU.ENUMAlanlar.GEREKCE, "");
                    }
                    dr.Close();
                }
                else
                    break;
            }

            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}

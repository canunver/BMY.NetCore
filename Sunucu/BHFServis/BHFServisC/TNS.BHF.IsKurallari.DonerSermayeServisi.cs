using System;
using TNS;
using TNS.KYM;
using TNS.BHF;
using TNS.Veri;
using System.Data;
using TNS.BHF.IsKurallari.Veri;
using System.Collections;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Döner sermaye ve fon bilgileri ile ilgili işlemleri tutan sınıf.
    /// </summary>
    public class DonerSermayeServisi
    {
        /// <summary>
        /// Döner sermaye ve fon bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait döner sermaye veya fon bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait döner sermaye veya fon bilgilerinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="donerSermayeNo">Listelenecek döner sermaye veya fon bilgisinin sıra numarası. (0 ise fon, birden başlıyorsa döner sermayedir.)</param>
        /// <returns>Birimin verilen sıradaki döner sermaye veya fona ait tanım ve ödenekleri döndürülür.</returns>
        public static DonerSermaye Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int donerSermayeNo)
        {
            IDataReader r;
            DonerSermayeSatir dss;
            DonerSermaye donusDeger = new DonerSermaye();
            donusDeger.sonuc.islemSonuc = false;

            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                kurumsalKod = kurumsalKod.Replace(".", "");
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABDonerSermaye tds = new TABDonerSermaye(ve);
                donusDeger.donerSermayeAd = tds.DonerSermayeAd(yil, kurumsalKod, donerSermayeNo);
                r = tds.KodlariAl();
                if (tds.Hata() == "")
                {
                    ArrayList al = new ArrayList();
                    while (r.Read())
                    {
                        dss = new DonerSermayeSatir();
                        dss.kod = r.GetString(0);
                        dss.aciklama = r.GetString(1);
                        dss.degerGirilsinMi = r.GetInt32(2) == 1;
                        al.Add(dss);
                    }
                    donusDeger.donerSermayeSatirlari = (DonerSermayeSatir[])al.ToArray(typeof(DonerSermayeSatir));
                    r.Close();
                    donusDeger.sonuc.islemSonuc = true;
                    for (int i = 0; i < donusDeger.donerSermayeSatirlari.Length; i++)
                    {
                        dss = donusDeger.donerSermayeSatirlari[i];
                        r = tds.YillariAl(yil, kurumsalKod, donerSermayeNo, dss.kod);
                        dss.kod = TNS.OrtakFonksiyonlar.KodNoktaKoy("9.9.9", dss.kod);
                        if (tds.Hata() == "")
                        {
                            al = new ArrayList();
                            while (r.Read())
                            {
                                DonerSermayeYil y = new DonerSermayeYil();
                                y.yil = r.GetInt32(0);
                                y.odenekMi = r.GetInt32(1) == 1;
                                y.deger = (double)r.GetDecimal(2);
                                al.Add(y);
                            }
                            dss.degerler = (DonerSermayeYil[])al.ToArray(typeof(DonerSermayeYil));
                            r.Close();
                        }
                        else
                        {
                            donusDeger.sonuc.hataStr = tds.Hata();
                            donusDeger.sonuc.islemSonuc = false;
                            break;
                        }
                    }
                }
                else
                    donusDeger.sonuc.hataStr = tds.Hata();
                ve.VeriErisimKapat();
            }
            else
                donusDeger.sonuc.hataStr = string.Format(ButceFormResI.BHFDSS001, kurumsalKod);
            return donusDeger;
        }

        /// <summary>
        /// Döner sermaye ve fon bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait döner sermaye veya fon bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait döner sermaye veya fon bilgilerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="donerSermayeNo">Kaydedilecek döner sermaye veya fon bilgisinin sıra numarası. (0 ise fon, birden başlıyorsa döner sermayedir.)</param>
        /// <param name="donerSermayeAd">Kaydedilecek döner sermaye veya fon bilgisinin adı.</param>
        /// <param name="donerSermaye">Kaydedilecek döner sermaye veya fonun ödenek bilgisi satırları.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int donerSermayeNo, string donerSermayeAd, TNSCollection donerSermaye)
        {
            int odenekHarcama;
            Sonuc sonuc = new Sonuc(false, "", "");
            if (Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
            {
                kurumsalKod = kurumsalKod.Replace(".", "");
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABDonerSermaye tds = new TABDonerSermaye(ve);
                ve.BeginTransaction();
                sonuc = tds.Sil(yil, kurumsalKod, donerSermayeNo);
                if (sonuc.islemSonuc)
                {
                    sonuc = tds.TanimEkle(yil, kurumsalKod, donerSermayeNo, donerSermayeAd);
                }
                if (sonuc.islemSonuc)
                {
                    foreach (DonerSermayeSatir dss in donerSermaye)
                    {
                        if (dss.degerGirilsinMi)
                        {
                            foreach (DonerSermayeYil y in dss.degerler)
                            {
                                if (y.odenekMi)
                                    odenekHarcama = 1;
                                else
                                    odenekHarcama = 2;
                                sonuc = tds.GelirGiderEkle(yil, kurumsalKod, donerSermayeNo, dss.kod.Replace(".", ""), y.yil, odenekHarcama, y.deger);
                                if (!sonuc.islemSonuc)
                                    break;
                            }

                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }
                }
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }
            else
                sonuc.hataStr = string.Format(ButceFormResI.BHFDSS002, kurumsalKod);
            return sonuc;
        }

        /// <summary>
        /// Döner sermaye ile ilgili sözlük bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <returns>Bulunan sözlük bilgileri dizi olarak döndürülür.</returns>
        public static DonerSermayeSatir[] SozlukListele(VeriErisim veriErisim)
        {
            ArrayList donusDeger = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(), TNS.KYM.Arac.KimlikBul(null));
            TABDonerSermaye tds = new TABDonerSermaye(ve);

            IDataReader r = tds.KodlariAl();
            if (!string.IsNullOrEmpty(tds.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tds.Hata());
            }

            while (r.Read())
            {
                DonerSermayeSatir dss = new DonerSermayeSatir();
                dss.kod = VeriErisim.DegerAl(r, 0, string.Empty);
                dss.aciklama = VeriErisim.DegerAl(r, 1, string.Empty);
                dss.degerGirilsinMi = VeriErisim.DegerAl(r, 2, 0) == 1;

                donusDeger.Add(dss);
            }
            r.Close();

            if (donusDeger.Count <= 0)
                throw new Exception(ButceFormResI.BHFDSS003);

            ve.VeriErisimKapat();
            return (DonerSermayeSatir[])donusDeger.ToArray(typeof(DonerSermayeSatir));
        }

        /// <summary>
        /// Döner sermaye sözlük bilgilerini silen ve verilen dizideki sözlük bilgilerini ekleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="dssler">Kaydedilecek DonerSermayeSatir tipinde nesneleri tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SozlukKaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection dssler)
        {
            Sonuc sonuc = new Sonuc();

            if (!Servis.Yoneticimi(kullanan))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceFormResI.BHFDSS004;
                return sonuc;
            }

            foreach (DonerSermayeSatir dss in dssler)
            {
                if (string.IsNullOrEmpty(dss.kod))
                    sonuc.DegerAta(ButceFormResI.BHFDSS005 + "<br>");
                if (string.IsNullOrEmpty(dss.aciklama))
                    sonuc.DegerAta(ButceFormResI.BHFDSS006 + "<br>");

                if (!sonuc.islemSonuc)
                {
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(), TNS.KYM.Arac.KimlikBul(kullanan));
            TABDonerSermaye tds = new TABDonerSermaye(ve);
            ve.BeginTransaction();

            tds.KodSil();
            foreach (DonerSermayeSatir dss in dssler)
            {
                sonuc = tds.KodKaydet(dss);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = ButceFormResI.BHFDSS007;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = ButceFormResI.BHFDSS008 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}

/*



        //public static DonerSermayeVAN[] Oku(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, int donerSermayeNo)
        //{
        //    ArrayList al = new ArrayList();

        //    IDataReader r;
        //    DonerSermayeSatir dss;
        //    DonerSermayeVAN donusDeger;


        //    if (Servis.GormeYetkisiVarMi(kullanan, kurum))
        //    {
        //        kurum = kurum.Replace(".", "");
        //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil));
        //        TABDonerSermaye tds = new TABDonerSermaye(ve);
        //        //IDataReader donersermayedizi;
        //        //donersermayedizi = tds.DonerSermayeNo(yil, kurum);

        //        //if (tds.Hata() != "")
        //        //{
        //        //    ve.VeriErisimKapat();
        //        //    throw new Exception(ve.hata);
        //        //}


        //        //int donerSermayeNo3 = 0;
        //        //while (donersermayedizi.Read())
        //        //{
        //        //    donerSermayeNo3= donersermayedizi.GetInt32(0);
        //        //  }
        //        //  donersermayedizi.Close();

        //          for (int j = 0; j < 6; j++)
        //          {

        //              donusDeger = new DonerSermayeVAN();

        //              donusDeger.donerSermayeAd = tds.DonerSermayeAd(yil, kurum, j);
        //              r = tds.KodlariAl();
        //              if (tds.Hata() == "" && donusDeger.donerSermayeAd!="")
        //              {
        //                  while (r.Read())
        //                  {
        //                      dss = new DonerSermayeSatir();
        //                      dss.kod = r.GetString(0);
        //                      dss.aciklama = r.GetString(1);
        //                      dss.degerGirilsinMi = r.GetInt32(2) == 1;
        //                      donusDeger.donerSermayeSatirlari.Add(dss);
        //                  }
        //                  r.Close();
        //                  donusDeger.sonuc.islemSonuc = true;
        //                  for (int i = 0; i < donusDeger.donerSermayeSatirlari.Count; i++)
        //                  {
        //                      dss = (DonerSermayeSatir)donusDeger.donerSermayeSatirlari[i];
        //                      r = tds.YillariAl(yil, kurum, j, dss.kod);
        //                      dss.kod = TNS.OrtakFonksiyonlar.KodNoktaKoy("9.9.9", dss.kod);
        //                      if (tds.Hata() == "")
        //                      {
        //                          while (r.Read())
        //                          {
        //                              DonerSermayeYil y = new DonerSermayeYil();
        //                              y.yil = r.GetInt32(0);
        //                              y.odenekMi = r.GetInt32(1) == 1;
        //                              y.deger = (double)r.GetDecimal(2);
        //                              dss.degerler.Add(y);
        //                          }
        //                          r.Close();
        //                      }
        //                      else
        //                      {
        //                          donusDeger.sonuc.hataStr = tds.Hata();
        //                          donusDeger.sonuc.islemSonuc = false;
        //                          break;
        //                      }
        //                  }
        //                  al.Add(donusDeger);
        //              }

        //          }
        //        ve.VeriErisimKapat();
        //    }
        //    return (DonerSermayeVAN[])al.ToArray(typeof(DonerSermayeVAN));
        //}
*/
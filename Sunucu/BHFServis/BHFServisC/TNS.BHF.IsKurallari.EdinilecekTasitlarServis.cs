using System;
using System.Data;
using TNS.BHF;
using TNS.KYM;
using TNS.Veri;
using TNS.BHF.IsKurallari.Veri;
using System.Collections;


namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Edinilecek taşıtlar, hizmet alımı ile edinilecek taşıtlar, taşıt genel bilgileri, taşıt sözlüğü ve hizmet alımı ie çalıştırılacak kişilerle ilgili işlemleri tutan sınıf.
    /// </summary>
    public class EdinilecekTasitlarServis
    {
        /// <summary>
        /// Bir birimin satın almayı planladığı taşıtları listeleyen yordam.
        /// Yetki kontrolü yapıldıktan sonra, veri tabanından çekilen kayıtlar listeye doldurulur, liste diziye çevrilir ve döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıtların listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıtların listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Verilen yılda, birimin satın almayı planladığı taşıtların listesi, listeyi ve sonucu içeren yapıda döndürülür.</returns>
        public static EdinilecekTasitSatiri[] EdinilecekTasitlarListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            ArrayList EdinilecekTasitSatirlari = new ArrayList();
            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                TABEdinilecekTasitlar tet = new TABEdinilecekTasitlar(ve);
                IDataReader r = tet.EdinilecekTasitlarListele(yil, kurumsalKod.Replace(".", ""));
                if (tet.Hata() == "")
                {
                    while (r.Read())
                    {
                        EdinilecekTasitSatirlari.Add(DataReaderdanAktar(r));
                    }
                    r.Close();
                }
                else
                {
                    ve.VeriErisimKapat();
                    throw new Exception(tet.Hata());
                }
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(string.Format(ButceFormResI.BHFETS001, kurumsalKod));
            }
            ve.VeriErisimKapat();
            return (EdinilecekTasitSatiri[])EdinilecekTasitSatirlari.ToArray(typeof(EdinilecekTasitSatiri));
        }

        /// <summary>
        /// Data readerdan okunan kayıt verileri kullanılarak EdinilecekTasitSatiri nesnesi oluşturulur.
        /// </summary>
        /// <param name="r">EdinilecekTasitSatiri nesnesi oluşturmak için kullanılacak data reader nesnesi.</param>
        /// <returns>Oluşturulan EdinilecekTasitSatiri nesnesi döndürülür.</returns>
        private static EdinilecekTasitSatiri DataReaderdanAktar(IDataReader r)
        {
            EdinilecekTasitSatiri ets = new EdinilecekTasitSatiri();

            ets.tasitSozluk.siraNo = VeriErisim.DegerAl(r, 3, "");
            ets.adet = VeriErisim.DegerAl(r, 4, 0);
            ets.kullanimYeri = VeriErisim.DegerAl(r, 5, "");
            ets.finkaynak = VeriErisim.DegerAl(r, 6, "");
            ets.tasitSozluk.cinsi = VeriErisim.DegerAl(r, 7, "");
            ets.tasitSozluk.diffarensiyel = VeriErisim.DegerAl(r, 8, "");
            return ets;
        }

        /// <summary>
        /// Bir birimin satın almayı planladığı taşıtları kaydeden yordam.
        /// Yetki kontrolü yapıldıktan sonra, bu birimin eski kayıtları silinir, edinilecekTasitlar dizisindeki her bir satır kontrol edilir ve hata yoksa veri tabanına kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıtların kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıtların kaydedilen birimin kurumsal kodu.</param>
        /// <param name="edinilecekTasitlar">Satın alınması planlanan taşıtların EdinilecekTasitSatiri nesnesi içeren listesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc EdinilecekTasitlarKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, TNSCollection edinilecekTasitlar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc sonuc = new Sonuc(true, "", "");

            TNS.BHF.IsKurallari.Servis.KurumsalKodBirimKontrol(kurumsalKod, sonuc);

            if (!Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                sonuc.DegerAta(ButceFormResI.BHFETS002);
            if (sonuc.islemSonuc)
                sonuc = KontrolEt(yil, kurumsalKod, edinilecekTasitlar);

            if (sonuc.islemSonuc)
            {
                kurumsalKod = kurumsalKod.Replace(".", "");
                TABEdinilecekTasitlar tet = new TABEdinilecekTasitlar(ve);
                ve.BeginTransaction();
                sonuc = tet.EdinilecekTasitlarSil(yil, kurumsalKod);
                if (sonuc.islemSonuc)
                {
                    int i = 1;
                    foreach (EdinilecekTasitSatiri ets in edinilecekTasitlar)
                    {
                        sonuc = tet.EdinilecekTasitlarKaydet(yil, kurumsalKod, i, ets.tasitSozluk.siraNo, ets.adet, ets.kullanimYeri, ets.finkaynak);

                        i = i + 1;
                        if (sonuc.islemSonuc == false)
                            break;
                    }
                }
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Kaydedilmeden önce kaydedilecek veriler ve satır bilgileri kontrol edilir. Kontrol sonucu Sonuc nesnesinde, hata varsa, hata mesajıyla birlikte döndürülür.
        /// </summary>
        /// <param name="yil">Kontrol edilecek kayıt yılı.</param>
        /// <param name="kurumsalKod">Kontrol edilecek kurumsal kod.</param>
        /// <param name="ets">Kontrol edilecek edinilecek taşıt bilgisi.</param>
        /// <returns>Kontrol sonucu hata mesajı ve sonucu Sonuc bilgisinde döndürülür.</returns>
        private static Sonuc KontrolEt(int yil, string kurumsalKod, TNSCollection edinilecekTasitlar)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (yil <= 0)
                sonuc.DegerAta(ButceFormResI.BHFETS003);
            if (kurumsalKod == "")
                sonuc.DegerAta(ButceFormResI.BHFETS004);
            if (sonuc.islemSonuc)
            {
                for (int i = 0; i < edinilecekTasitlar.Count; i++)
                {
                    string satirStr = $" (satir:{i + 1})";
                    EdinilecekTasitSatiri ets = (EdinilecekTasitSatiri)edinilecekTasitlar[i];
                    if (string.IsNullOrWhiteSpace(ets.tasitSozluk.siraNo))
                        sonuc.DegerAta(ButceFormResI.BHFETS005 + satirStr);
                    if (string.IsNullOrWhiteSpace(ets.finkaynak))
                        sonuc.DegerAta("Finans kaynağı boş bırakılmaz" + satirStr);
                    if (ets.adet <= 0)
                        sonuc.DegerAta("Adet boş bırakılmaz" + satirStr);
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Taşıt işlemleri tarafından bümko tarafından yayınlanan taşıt sözlük bilgilerini listeleyen yordam. Taşıtlar için bu sözlük bazında veri girilecektir.
        /// Veri tabanından çekilen kayıtlar listeye doldurulur, liste diziye çevrilir ve döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="yil">Taşıt sözlük bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <returns>Verilen yıldaki taşıt sözlük bilgileri döndürülür.</returns>
        public static TasitSozluk[] TasitSozlukListele(VeriErisim veriErisim, int yil)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            TABEdinilecekTasitlar tet = new TABEdinilecekTasitlar(ve);
            int icYil;
            if (yil == 0)
                icYil = tet.TasitSozlukYilBul();
            else
                icYil = yil;
            IDataReader r = tet.TasitSozlukListele(icYil);
            ArrayList tasitSozlukler = new ArrayList();
            if (tet.Hata() == "")
            {
                while (r.Read())
                {
                    TasitSozluk ts = new TasitSozluk();
                    ts.siraNo = VeriErisim.DegerAl(r, 1, "");
                    ts.cinsi = VeriErisim.DegerAl(r, 2, "");
                    ts.diffarensiyel = VeriErisim.DegerAl(r, 3, "");
                    tasitSozlukler.Add(ts);
                }
                r.Close();
                //if (tasitSozlukler.Count <= 0)
                //{
                //    ve.VeriErisimKapat();
                //    throw new Exception(ButceFormResI.BHFETS006);
                //}
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(tet.Hata());
            }
            ve.VeriErisimKapat();
            if (tasitSozlukler.Count == 0 && yil > 0) return TasitSozlukListele(veriErisim, 0);
            return (TasitSozluk[])tasitSozlukler.ToArray(typeof(TasitSozluk));
        }

        /// <summary>
        /// eBütçeden çekilen taşıt sözlük bilgilerini kaydeden yordam.
        /// Kullanıcı sistem yöneticisi ise bu yordam çalışır. Önce tüm kayıtlar kontrol edilir. Yetki varsa ve kayıtlar kontrolden geçmiş ise, verilen yıla ait eski kayıtlar silinir, tasitSozlukler dizisindeki her bir satır veri tabanına kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Sözlük bilgilerinin ait olduğu yıl.</param>
        /// <param name="tasitSozlukler">eBütçeden çekilen taşıt sözlük bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc TasitSozlukKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, TNSCollection tasitSozlukler)
        {
            Sonuc sonuc = new Sonuc();

            if (!Servis.Yoneticimi(kullanan))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceFormResI.BHFETS007;
                return sonuc;
            }

            foreach (TasitSozluk ts in tasitSozlukler)
            {
                if (string.IsNullOrEmpty(ts.siraNo))
                    sonuc.hataStr = ButceFormResI.BHFETS008 + "<br>";
                if (string.IsNullOrEmpty(ts.cinsi))
                    sonuc.hataStr += ButceFormResI.BHFETS009 + "<br>";
                if (string.IsNullOrEmpty(ts.diffarensiyel))
                    sonuc.hataStr += ButceFormResI.BHFETS010 + "<br>";

                if (!string.IsNullOrEmpty(sonuc.hataStr))
                {
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(), TNS.KYM.Arac.KimlikBul(kullanan));
            TABEdinilecekTasitlar tet = new TABEdinilecekTasitlar(ve);
            ve.BeginTransaction();

            tet.TasitSozlukSil(yil);
            foreach (TasitSozluk ts in tasitSozlukler)
            {
                sonuc = tet.TasitSozlukKaydet(yil, ts.siraNo, ts.cinsi, ts.diffarensiyel);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = ButceFormResI.BHFETS011;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = ButceFormResI.BHFETS012 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyetinin gibi taşıtlarla ilgili genel bilgilerin listelendiği yordam.
        /// Yetki kontrolü yapıldıktan sonra, verilen anahtar alanlar doğru ise, veri tabanından çekilen kayıtlar döndürülecek listeye doldurulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıt genel bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıt genel bilgilerinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Taşıt genel bilgileri dizisi döndürülür.</returns>
        public static ObjectArray KullanilanTasitlarListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int merkez1Tasra2Toplam3)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = new Sonuc(true, "", "");
            if (yil <= 0)
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFETS013);
            if (kurumsalKod == "")
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFETS014);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

                kurumsalKod = kurumsalKod.Replace(".", "");
                TABKullanilanTasitlar tabHAT = new TABKullanilanTasitlar(ve);
                IDataReader dr = tabHAT.Listele(yil, kurumsalKod, merkez1Tasra2Toplam3);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        TasitGenelBilgi hat = new TasitGenelBilgi();
                        hat.tasitSozluk.siraNo = VeriErisim.DegerAl(dr, 0, "");
                        hat.tasitSozluk.cinsi = VeriErisim.DegerAl(dr, 1, "");
                        hat.tasitSozluk.diffarensiyel = VeriErisim.DegerAl(dr, 2, "");
                        hat.kaynakKod = VeriErisim.DegerAl(dr, 3, 0);
                        hat.yasKod = VeriErisim.DegerAl(dr, 4, 0);
                        hat.adet = VeriErisim.DegerAl(dr, 5, 0);
                        donusDeger.Ekle(hat);
                    }
                    dr.Close();
                }
                else
                    donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
            }
            else
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFETS015);
            return donusDeger;
        }

        /// <summary>
        /// Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyetinin gibi taşıtlarla ilgili genel bilgilerin kaydedildiği yordam.
        /// Yetki kontrolü yapıldıktan sonra, bu birimin eski kayıtları silinir, tasitGenelBilgi dizisindeki her bir satır kontrol edilir ve hata yoksa veri tabanına kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıt genel bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıt genel bilgilerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="tasitGenelBilgi">Taşıt genel bilgilerinin tutulduğu dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KullanilanTasitlarKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, int merkezTasra, TNSCollection tasitGenelBilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (yil <= 0)
                sonuc.DegerAta(ButceFormResI.BHFETS016);
            TNS.BHF.IsKurallari.Servis.KurumsalKodBirimKontrol(kurumsalKod, sonuc);

            if (!Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                sonuc.DegerAta(ButceFormResI.BHFETS018);

            if (merkezTasra != 1 && merkezTasra != 2)
                sonuc.DegerAta("Lütfen kaydetmek için merkez veya taşra seçiniz");

            if (sonuc.islemSonuc)
            {
                for (int i = 0; i < tasitGenelBilgi.Count; i++)
                {
                    TasitGenelBilgi hatDis = (TasitGenelBilgi)tasitGenelBilgi[i];
                    if (hatDis.DoluMu() && hatDis.KontrolEt(sonuc, i))
                    {
                        for (int j = i + 1; j < tasitGenelBilgi.Count; j++)
                        {
                            TasitGenelBilgi hatIc = (TasitGenelBilgi)tasitGenelBilgi[j];
                            if (hatIc.DoluMu())
                            {
                                if (hatDis.EsitMi(hatIc))
                                    sonuc.DegerAta($"{i + 1}. satır ile {j + 1}. satırdaki bilgiler aynı, lütfen bir tanesinin rakamını 0 yapıp kaydediniz!");
                            }
                        }

                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                kurumsalKod = kurumsalKod.Replace(".", "");

                TABKullanilanTasitlar tabHAT = new TABKullanilanTasitlar(ve);
                ve.BeginTransaction();
                sonuc = tabHAT.Sil(yil, kurumsalKod, merkezTasra);
                if (sonuc.islemSonuc)
                {
                    foreach (TasitGenelBilgi hat in tasitGenelBilgi)
                    {
                        if (hat.DoluMu())
                            sonuc = tabHAT.Kaydet(yil, kurumsalKod, merkezTasra, hat);
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
            return sonuc;
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların şoförlü/şoförsüz, yakıtlı/yakıtsız miktar ve maliyetlerinin listelendiği yordam.
        /// Yetki kontrolü yapıldıktan sonra, verilen anahtar alanlar doğru ise, veri tabanından çekilen kayıtlar, HizmetAlimiTasitBilgisiDetay nesnesine çevrilerek, döndürülecek listeye doldurulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek taşıtların bilgilerinin listelendiği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek taşıtların bilgilerinin listelendiği birimin kurumsal kodu.</param>
        /// <returns>Hizmet alımı ile edinilecek taşıtların bilgilerinin tutulduğu dizi döndürülür.</returns>
        public static ObjectArray HizmetAlimiTasitlariDetayListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, bool kullanilan)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = new Sonuc(true, "", "");
            if (yil <= 0)
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFETS019);
            if (kurumsalKod == "")
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFETS020);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                kurumsalKod = kurumsalKod.Replace(".", "");
                TABHizmetAlimiTasitlariDetay tabHAT = new TABHizmetAlimiTasitlariDetay(ve);
                IDataReader dr = tabHAT.Listele(yil, kurumsalKod, kullanilan);
                if (dr != null)
                {
                    string eskiSiraNo = "";
                    HizmetAlimiTasitBilgisiDetay hat = null;
                    while (dr.Read())
                    {
                        string siraNo = VeriErisim.DegerAl(dr, 2, "");
                        if (siraNo != eskiSiraNo)
                        {
                            hat = new HizmetAlimiTasitBilgisiDetay();
                            hat.tasitSozluk.siraNo = siraNo;
                            eskiSiraNo = siraNo;
                            donusDeger.Ekle(hat);
                        }
                        int j = VeriErisim.DegerAl(dr, 3, 0);
                        if (j > 1) j = j - 2;
                        hat.soforluYakitGiderDahil[j].adet = VeriErisim.DegerAl(dr, 4, 0);
                        hat.soforluYakitGiderDahil[j].sure = VeriErisim.DegerAl(dr, 5, 0);
                        hat.soforluYakitGiderDahil[j].maliyet = VeriErisim.DegerAl(dr, 6, 0.0);

                        hat.soforluYakitGiderHaric[j].adet = VeriErisim.DegerAl(dr, 7, 0);
                        hat.soforluYakitGiderHaric[j].sure = VeriErisim.DegerAl(dr, 8, 0);
                        hat.soforluYakitGiderHaric[j].maliyet = VeriErisim.DegerAl(dr, 9, 0.0);

                        hat.soforsuzYakitGiderDahil[j].adet = VeriErisim.DegerAl(dr, 10, 0);
                        hat.soforsuzYakitGiderDahil[j].sure = VeriErisim.DegerAl(dr, 11, 0);
                        hat.soforsuzYakitGiderDahil[j].maliyet = VeriErisim.DegerAl(dr, 12, 0.0);

                        hat.soforsuzYakitGiderHaric[j].adet = VeriErisim.DegerAl(dr, 13, 0);
                        hat.soforsuzYakitGiderHaric[j].sure = VeriErisim.DegerAl(dr, 14, 0);
                        hat.soforsuzYakitGiderHaric[j].maliyet = VeriErisim.DegerAl(dr, 15, 0.0);
                    }
                }
                else
                    donusDeger.sonuc.DegerAta(ve.hata);
            }
            else
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFETS021);

            if (kurumsalKod.Replace(".", "").Length != 8)
            {
                foreach (HizmetAlimiTasitBilgisiDetay hat in donusDeger.objeler)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        OrtalamaHesapla(hat.soforluYakitGiderDahil[j].adet, ref hat.soforluYakitGiderDahil[j].sure, ref hat.soforluYakitGiderDahil[j].maliyet);
                        OrtalamaHesapla(hat.soforluYakitGiderHaric[j].adet, ref hat.soforluYakitGiderHaric[j].sure, ref hat.soforluYakitGiderHaric[j].maliyet);
                        OrtalamaHesapla(hat.soforsuzYakitGiderDahil[j].adet, ref hat.soforsuzYakitGiderDahil[j].sure, ref hat.soforsuzYakitGiderDahil[j].maliyet);
                        OrtalamaHesapla(hat.soforsuzYakitGiderHaric[j].adet, ref hat.soforsuzYakitGiderHaric[j].sure, ref hat.soforsuzYakitGiderHaric[j].maliyet);
                    }
                }
            }

            return donusDeger;
        }

        public static void OrtalamaHesapla(int adet, ref double sure, ref double maliyet)
        {
            if (adet <= 0 || sure <= 0) return;
            sure = (int)Math.Round(sure / adet);
            if (sure <= 0) return;
            maliyet = maliyet / (sure * adet);
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların şoförlü/şoförsüz, yakıtlı/yakıtsız miktar ve maliyetlerinin kaydedildiği yordam.
        /// Yetki kontrolü yapıldıktan sonra, bu birimin eski kayıtları silinir, hatDetaylar dizisindeki her bir satır kontrol edilir ve hata yoksa veri tabanına kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek taşıtların bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek taşıtların bilgilerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="hatDetaylar">Hizmet alımı ile edinilecek taşıtların bilgilerinin tutulduğu dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc HizmetAlimiTasitlariDetayKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, TNSCollection hatDetaylar, bool kullanilan)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (yil <= 0)
                sonuc.DegerAta(ButceFormResI.BHFETS022);
            TNS.BHF.IsKurallari.Servis.KurumsalKodBirimKontrol(kurumsalKod, sonuc);

            if (!Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                sonuc.DegerAta(ButceFormResI.BHFETS024);

            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            kurumsalKod = kurumsalKod.Replace(".", "");
            TABHizmetAlimiTasitlariDetay tabHAT = new TABHizmetAlimiTasitlariDetay(ve);
            ve.BeginTransaction();
            sonuc = tabHAT.Sil(yil, kurumsalKod, kullanilan);
            if (sonuc.islemSonuc)
            {
                foreach (HizmetAlimiTasitBilgisiDetay hat in hatDetaylar)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int yilKod;
                        if (kullanilan) yilKod = i;
                        else yilKod = i + 2;
                        sonuc = tabHAT.Kaydet(yil, kurumsalKod, yilKod, i, hat);
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
            return sonuc;
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek görevlilerin hizmet türleri itibariyle miktar ve maliyetlerinin listelendiği yordam.
        /// Yetki kontrolü yapıldıktan sonra, verilen anahtar alanlar doğru ise, veri tabanından çekilen kayıtlar, HizmetAlimiGorevli nesnesine çevrilerek, döndürülecek listeye doldurulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek görevlilerin bilgilerinin listelendiği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek görevlilerin bilgilerinin listelendiği birimin kurumsal kodu.</param>
        /// <returns>Hizmet alımı ile edinilecek görevlilerin bilgilerinin tutulduğu dizi döndürülür.</returns>
        public static ObjectArray HizmetAlimiGorevliListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = new Sonuc(true, "", "");
            if (yil <= 0)
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFETS025);
            if (kurumsalKod == "")
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFETS026);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                kurumsalKod = kurumsalKod.Replace(".", "");
                TABHizmetAlimiGorevli tabHAT = new TABHizmetAlimiGorevli(ve);
                IDataReader dr = tabHAT.Listele(yil, kurumsalKod);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        HizmetAlimiGorevli hat = new HizmetAlimiGorevli();
                        hat.SiraNo = VeriErisim.DegerAl(dr, 2, "");

                        hat.ozelGorevliTemizlikAdet2 = VeriErisim.DegerAl(dr, 3, 0);
                        hat.ozelGorevliTemizlikSure2 = VeriErisim.DegerAl(dr, 4, 0);
                        hat.ozelGorevliTemizlikMaliyet2 = VeriErisim.DegerAl(dr, 5, 0.0);
                        hat.ozelGorevliGuvenlikAdet2 = VeriErisim.DegerAl(dr, 6, 0);
                        hat.ozelGorevliGuvenlikSure2 = VeriErisim.DegerAl(dr, 7, 0);
                        hat.ozelGorevliGuvenlikMaliyet2 = VeriErisim.DegerAl(dr, 8, 0.0);
                        hat.ozelGorevliSoforAdet2 = VeriErisim.DegerAl(dr, 9, 0);
                        hat.ozelGorevliSoforSure2 = VeriErisim.DegerAl(dr, 10, 0);
                        hat.ozelGorevliSoforMaliyet2 = VeriErisim.DegerAl(dr, 11, 0.0);
                        hat.ozelGorevliDigerAdet2 = VeriErisim.DegerAl(dr, 12, 0);
                        hat.ozelGorevliDigerSure2 = VeriErisim.DegerAl(dr, 13, 0);
                        hat.ozelGorevliDigerMaliyet2 = VeriErisim.DegerAl(dr, 14, 0.0);

                        hat.ozelGorevliTemizlikAdet1 = VeriErisim.DegerAl(dr, 15, 0);
                        hat.ozelGorevliTemizlikSure1 = VeriErisim.DegerAl(dr, 16, 0);
                        hat.ozelGorevliTemizlikMaliyet1 = VeriErisim.DegerAl(dr, 17, 0.0);
                        hat.ozelGorevliGuvenlikAdet1 = VeriErisim.DegerAl(dr, 18, 0);
                        hat.ozelGorevliGuvenlikSure1 = VeriErisim.DegerAl(dr, 19, 0);
                        hat.ozelGorevliGuvenlikMaliyet1 = VeriErisim.DegerAl(dr, 20, 0.0);
                        hat.ozelGorevliSoforAdet1 = VeriErisim.DegerAl(dr, 21, 0);
                        hat.ozelGorevliSoforSure1 = VeriErisim.DegerAl(dr, 22, 0);
                        hat.ozelGorevliSoforMaliyet1 = VeriErisim.DegerAl(dr, 23, 0.0);
                        hat.ozelGorevliDigerAdet1 = VeriErisim.DegerAl(dr, 24, 0);
                        hat.ozelGorevliDigerSure1 = VeriErisim.DegerAl(dr, 25, 0);
                        hat.ozelGorevliDigerMaliyet1 = VeriErisim.DegerAl(dr, 26, 0.0);

                        donusDeger.Ekle(hat);
                    }
                }
                else
                    donusDeger.sonuc.DegerAta(ve.hata);
            }
            else
                donusDeger.sonuc.DegerAta(ButceFormResI.BHFETS027);
            return donusDeger;
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek görevlilerin hizmet türleri itibariyle miktar ve maliyetlerinin kaydedildiği yordam.
        /// Yetki kontrolü yapıldıktan sonra, bu birimin eski kayıtları silinir, hatDetaylar dizisindeki her bir satır kontrol edilir ve hata yoksa veri tabanına kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek görevlilerin bilgilerinin kaydedildiği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek görevlilerin bilgilerinin kaydedildiği birimin kurumsal kodu.</param>
        /// <param name="hizmetAlimiGorevliler">Hizmet alımı ile edinilecek görevlilerin bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc HizmetAlimiGorevliKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, TNSCollection hizmetAlimiGorevliler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (yil <= 0)
                sonuc.DegerAta(ButceFormResI.BHFETS028);
            if (kurumsalKod == "")
                sonuc.DegerAta(ButceFormResI.BHFETS029);
            TNS.BHF.IsKurallari.Servis.KurumsalKodBirimKontrol(kurumsalKod, sonuc);
            if (!sonuc.islemSonuc)
                return sonuc;

            if (Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                kurumsalKod = kurumsalKod.Replace(".", "");
                TABHizmetAlimiGorevli tabHAG = new TABHizmetAlimiGorevli(ve);
                ve.BeginTransaction();
                sonuc = tabHAG.Sil(yil, kurumsalKod);
                if (sonuc.islemSonuc)
                {
                    foreach (HizmetAlimiGorevli hag in hizmetAlimiGorevliler)
                    {
                        sonuc = tabHAG.Kaydet(yil, kurumsalKod, hag.SiraNo,
                        hag.ozelGorevliTemizlikAdet2,
                        hag.ozelGorevliTemizlikSure2,
                        hag.ozelGorevliTemizlikMaliyet2,
                        hag.ozelGorevliGuvenlikAdet2,
                        hag.ozelGorevliGuvenlikSure2,
                        hag.ozelGorevliGuvenlikMaliyet2,
                        hag.ozelGorevliSoforAdet2,
                        hag.ozelGorevliSoforSure2,
                        hag.ozelGorevliSoforMaliyet2,
                        hag.ozelGorevliDigerAdet2,
                        hag.ozelGorevliDigerSure2,
                        hag.ozelGorevliDigerMaliyet2,
                        hag.ozelGorevliTemizlikAdet1,
                        hag.ozelGorevliTemizlikSure1,
                        hag.ozelGorevliTemizlikMaliyet1,
                        hag.ozelGorevliGuvenlikAdet1,
                        hag.ozelGorevliGuvenlikSure1,
                        hag.ozelGorevliGuvenlikMaliyet1,
                        hag.ozelGorevliSoforAdet1,
                        hag.ozelGorevliSoforSure1,
                        hag.ozelGorevliSoforMaliyet1,
                        hag.ozelGorevliDigerAdet1,
                        hag.ozelGorevliDigerSure1,
                        hag.ozelGorevliDigerMaliyet1);

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
                sonuc.DegerAta(ButceFormResI.BHFETS030);
            return sonuc;
        }
    }
}

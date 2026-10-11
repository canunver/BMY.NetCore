using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;
using TNS.UZY;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Harcama birimlerden toplanan bütçe hazırlık verileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BirimOdenekGider
    {
        /// <summary>
        /// Verilen ödenekler dizisini, verilen asama için veri tabanına kaydeden yordam.
        /// Alan değerleri ve Yetki kontrolü yapıldıktan sonra. Önce tertibe ait ödenek bilgisi silinir, sonra yeni kayıtlar eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="asama">Listelenecek verilerin aşaması. Harcama birimi bazında ödenekler çeşitli aşamalarda girilebilemekte ve listelenmektedir.</param>
        /// <param name="tertipOdenekler">Veri tabanına kaydedilecek ödenek bilgilerini tutan dizi.
        /// Dizideki tertipler tasra ödenek türünde olmalı ve son kırılımı birimi göstermelidir.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ENUMBHBirimAsama asama, TertipOdenek[] tertipOdenekler)
        {
            Sonuc s = new Sonuc(true, string.Empty, string.Empty);
            if (tertipOdenekler == null || tertipOdenekler.Length == 0)
            {
                s.DegerAta(ButceHazirlikResI.BHMBOG001);
                return s;
            }

            int yil = -1;
            if (tertipOdenekler.Length > 0)
            {
                yil = tertipOdenekler[0].tertip.yil;
                string kyKod = string.Empty;
                for (int i = 0; i < tertipOdenekler.Length && s.islemSonuc; i++)
                {
                    if (yil != tertipOdenekler[i].tertip.yil)
                    {
                        s.DegerAta(ButceHazirlikResI.BHMBOG002);
                    }
                    else if (kyKod != tertipOdenekler[i].tertip.kodlar[5])
                    {
                        kyKod = tertipOdenekler[i].tertip.kodlar[5];
                        s = YazmaYetkisiVarMi(kullanan, yil, asama, kyKod);
                    }
                }
            }
            else
                s.DegerAta(ButceHazirlikResI.BHMBOG003);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMBIRIMODENEKGIDER tabBHMBIRIMGIDER = new TABBHMBIRIMODENEKGIDER(ve);

            ve.BeginTransaction();

            for (int i = 0; i < tertipOdenekler.Length && s.islemSonuc; i++)
            {
                s = tabBHMBIRIMGIDER.Sil(tertipOdenekler[i].tertip, asama);
                if (s.islemSonuc && tertipOdenekler[i].odenek[0] > 0)
                    s = tabBHMBIRIMGIDER.Ekle(tertipOdenekler[i].tertip, asama, tertipOdenekler[i].odenek[0], tertipOdenekler[i].aciklama);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Kullanıcının verilen kurumsal yapıya yazma yetkisinin olup olmadığı kontrol edilir.
        /// Kullanıcının hem ilgili harcama birimine hem de ilişkili kurumsal koda yazma yetkisinin olması gerekmektedir.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler</param>
        /// <param name="yil">Aşamanın kilitli olup olmadığını bulmak için kullanılacak yıl parametresi</param>
        /// <param name="asama">Birim kullanıcılarının yazabileceği aşamalarla, merkez kullanıcılarının yazabileceği aşamalar farklıdır. Bu kontrolün yapılabilmesi için verilen aşama parametresi.</param>
        /// <param name="kurumsalYapiKod">Kullanıcının birime yazma yetkisinin olup olmadığını bulmak için verilen harcama birimi kodu.</param>
        /// <returns>Yazma yetkisi varsa Sonuc yapısında doğru, yoksa false ve hata mesajı döndürülür.</returns>
        private static Sonuc YazmaYetkisiVarMi(Kullanici kullanan, int yil, ENUMBHBirimAsama asama, string kurumsalYapiKod)
        {
            Sonuc s = new Sonuc(true, string.Empty, string.Empty);
            KurumsalYapi kY;
            kY = BHMv2.IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(null, kurumsalYapiKod);
            if (kY == null)
                s.DegerAta(ButceHazirlikResI.BHMBOG004);
            if (kY.saymanlikKod==0 || string.IsNullOrEmpty(kY.birimKod) || string.IsNullOrEmpty(kY.kurKod))
                s.DegerAta(ButceHazirlikResI.BHMBOG005);

            bool yetki = BHMv2Genel.YazmaYetkisiVarMi(kullanan, yil, kY, asama);
            if (!yetki)
                s.DegerAta(string.Format(ButceHazirlikResI.BHMBOG006, kY.kod, kY.ad));
            return s;
        }

        /// <summary>
        /// Kullanıcının verilen kurumsal yapıya ait verileri görme yetkisinin olup olmadığı kontrol edilir.
        /// Kullanıcının hem ilgili harcama birimine hem de ilişkili kurumsal kodu görme yetkisinin olması gerekmektedir.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler</param>
        /// <param name="yil">Aşamanın onaylı olup olmadığını bulmak için kullanılacak yıl parametresi. Onaylı veriler herkes tarafından görülebilir.</param>
        /// <param name="asama">Birim kullanıcılarının görebileceği aşamalarla, merkez kullanıcılarının görebileceği aşamalar farklıdır. Bu kontrolün yapılabilmesi için verilen aşama parametresi.</param>
        /// <param name="kurumsalYapiKod">Kullanıcının birime ait verileri görme yetkisinin olup olmadığını bulmak için verilen harcama birimi kodu.</param>
        /// <returns>Görme yetkisi varsa doğru, yoksa false döndürülür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, int yil, ENUMBHBirimAsama asama, string kurumsalYapiKod)
        {
            return true;
        }

        /// <summary>
        /// Verilen kıstas tertibe uyan ve harcama birimi bazında girilen ödenekleri listeleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelenecek ödenekler için verilen kıstas.
        /// Tertip tasra ödenek türünde olmalı ve son kırılımı birimi göstermelidir.</param>
        /// <param name="asama">Listelenecek verilerin aşaması. Harcama birimi bazında ödenekler çeşitli aşamalarda girilebilemekte ve listelenmektedir.</param>
        /// <param name="ekDegerParamlar">Tertipler listelenirken, her bir tertiple birlikte listelenecek ek bilgiler dizisi</param>
        /// <returns>Verilen kıstas tertiple uyumlu harcama birimi ödenekleri, istenilen ek bilgilerle birlikte döndürülür.</returns>
        public static TertipOdenek[] Listele(VeriErisim veriErisim, Kullanici kullanan, Tertip kistasTertip, ENUMBHBirimAsama asama, EkDegerParam[] ekDegerParamlar)
        {
            if (!GormeYetkisiVarMi(kullanan, kistasTertip.yil, asama, kistasTertip.kodlar[5]))
            {
                throw new Exception(ButceHazirlikResI.BHMBOG007);
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(kistasTertip.yil), kullanan.KimlikBul());
            TABBHMBIRIMODENEKGIDER tabBirimGider = new TABBHMBIRIMODENEKGIDER(ve);

            AltFaaliyet altFaaliyetKistas = new AltFaaliyet();
            altFaaliyetKistas.yil = kistasTertip.yil;

            kistasTertip.kodlar[4] = BHMv2AltFaaliyet.Listele(veriErisim, altFaaliyetKistas, kistasTertip.kodlar[5], ',');
            Tertip[] tertipler = TertipTanim.Listele(veriErisim, kullanan, kistasTertip, 1, null);
            if (tertipler == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ButceHazirlikResI.BHMBOG008);
            }
            TertipOdenek[] tertipOdenekler = new TertipOdenek[tertipler.Length];
            IUZYServis uzyServis = UZY.Arac.Tanimla();
            for (int i = 0; i < tertipler.Length; i++)
            {
                tertipOdenekler[i] = new TertipOdenek(EkDegerUzunluk(ekDegerParamlar) + 1);
                tertipOdenekler[i].tertip = tertipler[i];
                tertipOdenekler[i].tertip.kodlar[5] = kistasTertip.kodlar[5];

                IDataReader dr = tabBirimGider.Oku(tertipler[i], asama);
                bool okundu = false;

                if (dr != null)
                {
                    if (dr.Read())
                    {
                        tertipOdenekler[i].odenek[0] = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMODENEKGIDER.ENUMAlanlar.ODENEK, -1);
                        tertipOdenekler[i].aciklama = VeriErisim.DegerAl(dr, (int)TABBHMBIRIMODENEKGIDER.ENUMAlanlar.ACIKLAMA, string.Empty);
                        okundu = true;
                    }
                    dr.Close();
                }
                else
                {
                    ve.VeriErisimKapat();
                    throw new Exception(string.Format(ButceHazirlikResI.BHMBOG009, ve.hata));
                }
                if (!okundu)
                {
                    tertipOdenekler[i].odenek[0] = 0;
                    tertipOdenekler[i].aciklama = string.Empty;
                }

                for (int j = 1; j <= EkDegerUzunluk(ekDegerParamlar); j++)
                {
                    tertipOdenekler[i].odenek[j] = ekDegerParamlar[j - 1].EkDegerHesaplaDbl(uzyServis, kullanan, kistasTertip.yil, tertipOdenekler[i].tertip.TertipStrOl());
                }
            }
            ve.VeriErisimKapat();
            return tertipOdenekler;
        }

        /// <summary>
        /// Paramtere olarak verilen dizinin uzunluğu bulunur ve döndürülür.
        /// Dizinin null olma ihtimaline karşı yazılmıştır.
        /// </summary>
        /// <param name="ekDegerParamlar">Uzunluğu bulunacak dizi.</param>
        /// <returns>Verilen dizinin uzunluğu döndürülür.</returns>
        public static int EkDegerUzunluk(EkDegerParam[] ekDegerParamlar)
        {
            if (ekDegerParamlar == null)
                return 0;
            else
                return ekDegerParamlar.Length;
        }
    }
}

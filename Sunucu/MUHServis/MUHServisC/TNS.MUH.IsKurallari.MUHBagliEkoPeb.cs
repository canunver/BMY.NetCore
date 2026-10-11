using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
using TNS.MUH.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// MUHBagliEkoPebAna formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class MUHBagliEkoPebAna
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBUM";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, BagliEkoPeb form)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            return false;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, BagliEkoPeb form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrEmpty(form.tertip))
                s.DegerAta("Tertip alanını girmelisiniz.");
            if (string.IsNullOrEmpty(form.peb))
                s.DegerAta("Faaliyet kodunu girmelisiniz.");
            return s;
        }

        /// <summary>
        /// BagliEkoPeb nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BagliEkoPeb yapısında BagliEkoPeb:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BagliEkoPeb form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yetkiniz yok");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBAGLIEKOPEB tabMUHBAGLIEKOPEB = new TABMUHBAGLIEKOPEB(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabMUHBAGLIEKOPEB.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        internal static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BagliEkoPeb form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yetkiniz yok");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBAGLIEKOPEB tabMUHBAGLIEKOPEB = new TABMUHBAGLIEKOPEB(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabMUHBAGLIEKOPEB.Sil(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// BagliEkoPeb listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BagliEkoPeb yapısında BagliEkoPeb:  bilgilerini tutan değişken</param>
        public static BagliEkoPeb[] Listele(VeriErisim veriErisim, Kullanici kullanan, BagliEkoPeb form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHBAGLIEKOPEB tabMUHBAGLIEKOPEB = new TABMUHBAGLIEKOPEB(ve);

            ArrayList al = new ArrayList();

            IDataReader dr = tabMUHBAGLIEKOPEB.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    BagliEkoPeb detay = new BagliEkoPeb();
                    detay.tertip = VeriErisim.DegerAl(dr, (int)TABMUHBAGLIEKOPEB.ENUMAlanlar.TERTIP, "");
                    detay.peb = VeriErisim.DegerAl(dr, (int)TABMUHBAGLIEKOPEB.ENUMAlanlar.PEB, "");
                    al.Add(detay);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return (BagliEkoPeb[])al.ToArray(typeof(BagliEkoPeb));
        }

        internal static Sonuc TertiplerUygunMu(VeriErisim veriErisim, Kullanici kullanan, TNSCollection tertipler)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            for (int i = 0; i < tertipler.Count; i++)
            {
                TNS.BKD.Tertip t;
                object o = tertipler[i];
                if (o.GetType() == typeof(TNS.BKD.Tertip))
                    t = (TNS.BKD.Tertip)o;
                else if (o.GetType() == typeof(TNS.BKD.TertipOdenekBilgi))
                {
                    TNS.BKD.TertipOdenekBilgi tob = (TNS.BKD.TertipOdenekBilgi)o;
                    string[] tertipParca = tob.tertip.Split('-');
                    t = new BKD.Tertip(DateTime.Now.Year, BKD.ENUMTabloKod.TABPEBOdenek, OrtakFonksiyonlar.DiziElemanAl(tertipParca, 0, ""), OrtakFonksiyonlar.DiziElemanAl(tertipParca, 1, ""), OrtakFonksiyonlar.DiziElemanAl(tertipParca, 2, ""), OrtakFonksiyonlar.DiziElemanAl(tertipParca, 3, ""), OrtakFonksiyonlar.DiziElemanAl(tertipParca, 4, ""));
                }
                else
                    t = null;
                if (t != null)
                {
                    OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(t);
                    t.NoktalariAt();
                    string tut = UygunMu(ve, kullanan, detay);
                    if (!string.IsNullOrWhiteSpace(tut))
                        s.DegerAta(tut);
                }
                else
                    s.DegerAta("TertipUygunMu metoduna bilinmeyen tip geldi!");
            }
            ve.VeriErisimKapat();
            return s;
        }

        internal static string UygunMu(VeriErisim ve, Kullanici kullanan, OdemeEmriMIFDetay detay)
        {
            if (detay == null) return "";
            string peb = detay.peb;
            if (string.IsNullOrWhiteSpace(peb)) return "";

            if (string.IsNullOrWhiteSpace(detay.peb)) return "";
            string tertip = detay.kur + "-" + detay.fon + "-" + detay.fin + "-" + detay.eko;

            BagliEkoPeb form = new BagliEkoPeb();
            form.tertip = tertip;
            form.peb = peb;

            TABMUHBAGLIEKOPEB tabMUHBAGLIEKOPEB = new TABMUHBAGLIEKOPEB(ve);

            IDataReader dr = tabMUHBAGLIEKOPEB.Listele(form);
            if (dr == null)
            {
                ve.hata = "";
                return "";
            }

            bool ilkSAtirVar = dr.Read();

            dr.Close();
            if (ilkSAtirVar)
                return "";
            form.peb = "";

            BagliEkoPeb[] tertipler = Listele(ve, kullanan, form);
            if (tertipler != null && tertipler.Length > 0)
                return HataStrOl(tertipler, false, tertip);

            form.peb = peb;
            form.tertip = "";
            tertipler = Listele(ve, kullanan, form);
            if (tertipler != null && tertipler.Length > 0)
                return HataStrOl(tertipler, true, peb);
            return "";
        }

        private static string HataStrOl(BagliEkoPeb[] tertipler, bool peb, string kod)
        {
            string hata;
            if (peb)
                hata = String.Format("Girdiğiniz faaliyet kodundan ({0}) aşağıdaki tertiplere ait işlem yapabilirsiniz:<br>", kod);
            else
                hata = String.Format("Girdiğiniz tertipden ({0}) aşağıdaki faaliyetlere ait işlem yapabilirsiniz:<br>", kod);
            for (int i = 0; i < tertipler.Length; i++)
            {
                hata += peb ? tertipler[i].tertip : tertipler[i].peb;
            }
            return hata;
        }
    }
}

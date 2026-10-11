using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using System.Collections.Generic;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// HRCCalisanBordro formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCCalisanBordro
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, CalisanBordroAna form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, CalisanBordroAna form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// CalisanBordroAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroAna yapısında CalisanBordroAna: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, CalisanBordroAna form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABPSLCALISANBORDROANA tabPSLCALISANBORDROANA = new TABPSLCALISANBORDROANA(ve);
            TABPSLCALISANBORDRODETAY tabPSLCALISANBORDRODETAY = new TABPSLCALISANBORDRODETAY(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabPSLCALISANBORDRODETAY.Sil(form);
            if (s.islemSonuc)
                s = tabPSLCALISANBORDROANA.Sil(form);

            if (s.islemSonuc)
                s = tabPSLCALISANBORDROANA.Ekle(form);

            for (int i = 0; i < form.kisiler.Length && s.islemSonuc; i++)
            {
                CalisanBordroKisi k = form.kisiler[i];
                for (int j = 0; j < k.parametreler.Count; j++)
                {
                    s = tabPSLCALISANBORDRODETAY.Ekle(form, k.tcKimlikNo, (CalisanBordroDetay)k.parametreler[j]);
                }
            }

            if (s.islemSonuc)
                ve.Commit();
            else

                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, CalisanBordroAna form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, CalisanBordroAna form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// CalisanBordroAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroAna yapısında CalisanBordroAna: bilgilerini tutan değişken</param>
        public static CalisanBordroAna Oku(VeriErisim veriErisim, Kullanici kullanan, CalisanBordroAna form)
        {
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception("Görme yetkiniz yok!");
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                throw new Exception(s.hataStr);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABPSLCALISANBORDROANA tabPSLCALISANBORDROANA = new TABPSLCALISANBORDROANA(ve);
            TABPSLCALISANBORDRODETAY tabPSLCALISANBORDRODETAY = new TABPSLCALISANBORDRODETAY(ve);

            CalisanBordroAna formIc = new CalisanBordroAna();
            IDataReader dr;
            dr = tabPSLCALISANBORDROANA.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            if (dr.Read())
            {
                formIc.surecYil = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROANA.ENUMAlanlar.SURECYIL, 0);
                formIc.surecNo = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROANA.ENUMAlanlar.SURECNO, "");
                formIc.odemeEmriNo = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROANA.ENUMAlanlar.ODEMEEMRINO, "");
                formIc.odemeGun = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROANA.ENUMAlanlar.ODEMEGUN);
                formIc.odemeSiraNo = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDROANA.ENUMAlanlar.ODEMESIRANO, 0);
            }
            dr.Close();
            ArrayList al;
            dr = tabPSLCALISANBORDRODETAY.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            al = new ArrayList();

            string ustteki = null;
            CalisanBordroKisi k = null;
            while (dr.Read())
            {
                string tckn = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDRODETAY.ENUMAlanlar.TCKIMLIKNO, "");
                if (tckn != ustteki)
                {
                    k = new CalisanBordroKisi();
                    k.tcKimlikNo = tckn;
                    al.Add(k);
                    ustteki = tckn;
                }
                CalisanBordroDetay detay = new CalisanBordroDetay();
                detay.gelirKesintiKod = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDRODETAY.ENUMAlanlar.GELKESKOD, 0);
                detay.miktar = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDRODETAY.ENUMAlanlar.MIKTAR, 0.0);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABPSLCALISANBORDRODETAY.ENUMAlanlar.ACIKLAMA, "");
                k.ParametreEkle(detay);

            }
            formIc.kisiler = (CalisanBordroKisi[])al.ToArray(typeof(CalisanBordroKisi));
            dr.Close();
            ve.VeriErisimKapat();
            return formIc;
        }

        public static CalisanBordroAna BordroViewOku(VeriErisim veriErisim, Kullanici kullanan, string tarih, string kume)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            CalisanBordroGelirKesinti[] gelKesDizi = HRCCalisanBordroGelirKesinti.Listele(ve, kullanan);
            //string kume;
            //if (bordroTuru.Equals(5434)) kume = "(3,4,5)";
            //else if (bordroTuru.Equals(5510)) kume = "(1,2,6)";
            //else
            //    kume = "(1,2,3,4,6)";
            string sql = "SELECT * FROM UA_CALISAN_BORDRO";
            sql += " WHERE TARIH = " + VeriErisim.DegerYap(tarih);
            if(!string.IsNullOrWhiteSpace(kume))
                sql += " AND TUR IN " + kume;

            IDataReader dataReader1 = ve.DataReaderDondur(sql);
            if (dataReader1 == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            CalisanBordroAna clsnBrdAna = new CalisanBordroAna();

            List<CalisanBordroKisi> kisiler = new List<CalisanBordroKisi>();

            while (dataReader1.Read())
            {
                string bilgiStr = "";
                CalisanBordroKisi ks = new CalisanBordroKisi();

                for (int i = 0; i < dataReader1.FieldCount; i++)
                {
                    string fname = dataReader1.GetName(i);
                    string ftype = dataReader1.GetDataTypeName(i);
                    if (fname.StartsWith("F"))
                    {
                        int gelKesKod = OrtakFonksiyonlar.ConvertToInt(fname.Substring(1), 0);
                        if (gelKesKod > 0)
                        {
                            CalisanBordroGelirKesinti gelKes = HRCCalisanBordroGelirKesinti.KesintiBul(gelKesDizi, gelKesKod);
                            if (gelKes != null)
                            {
                                CalisanBordroDetay parametre = new CalisanBordroDetay();
                                parametre.gelirKesintiKod = gelKesKod;
                                try
                                {
                                    if (gelKes.gelirKesintiTur == ENUMGelirKesintiTur.BILGI)
                                    {
                                        parametre.aciklama = VeriErisim.DegerAl(dataReader1, i, "");
                                        if (gelKesKod == 2) ks.tcKimlikNo = parametre.aciklama;
                                    }
                                    else
                                    {
                                        if (ftype == "decimal")
                                        {
                                            parametre.miktar = Convert.ToDouble(VeriErisim.DegerAl(dataReader1, i, (decimal)0));
                                        }
                                        else
                                        {
                                            parametre.miktar = VeriErisim.DegerAl(dataReader1, i, (double)0);
                                        }
                                    }
                                    ks.ParametreEkle(parametre);
                                }
                                catch
                                {
                                    bilgiStr += string.Format("{0} kolonu {1} tipinde okunamadı!", fname, ftype);
                                }
                            }
                            else
                                throw new Exception(string.Format("{0} kodlu alan tanım tablosunda tanımsız.", gelKesKod));
                        }
                    }
                }
                if (bilgiStr != "") throw new Exception(bilgiStr);
                kisiler.Add(ks);
            }
            clsnBrdAna.kisiler = kisiler.ToArray();

            dataReader1.Close();
            ve.VeriErisimKapat();

            return clsnBrdAna;
        }

        public static Sonuc BordroOdemeEmriKaydet(VeriErisim veriErisim, Kullanici kullanan, string surecNo, int surecYil, string olusanOdemeEmriNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00013);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABPSLCALISANBORDROANA tab = new TABPSLCALISANBORDROANA(ve);

            ve.BeginTransaction();
            sonuc = tab.BordroOdemeEmriKaydet(surecYil, surecNo, olusanOdemeEmriNo);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc BordroBankaListesiKaydet(VeriErisim veriErisim, Kullanici kullanan, string surecNo, int surecYil, TNSDateTime bankaOdemeGun, int bankaOdemeSiraNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00013);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABPSLCALISANBORDROANA tab = new TABPSLCALISANBORDROANA(ve);

            ve.BeginTransaction();
            sonuc = tab.BordroBankaListesiKaydet(surecYil, surecNo, bankaOdemeGun, bankaOdemeSiraNo);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}

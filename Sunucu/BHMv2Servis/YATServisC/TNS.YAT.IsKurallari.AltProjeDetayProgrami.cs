using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
using TNS.YAT.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// YATAltProjeDetayProgrami formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class YATAltProjeDetayProgrami
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriYAT";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, AltProjeDetayProgrami form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, AltProjeDetayProgrami ilkForm, AltProjeDetayProgrami kontrolForm, Sonuc s)
        {
            if (kontrolForm.yil != ilkForm.yil || kontrolForm.senaryoKod != ilkForm.senaryoKod || kontrolForm.altProjeNo != ilkForm.altProjeNo)
                s.DegerAta("Gelen veriler arasında tutarsızlık var.");
            if (kontrolForm.odenek < 0)
                s.DegerAta(string.Format("{0}. satırda hatalı ödenek girilmiş.", kontrolForm.siraNo));
            else if (kontrolForm.odenek == 0)
            {
                if (!string.IsNullOrWhiteSpace(kontrolForm.yer) || !string.IsNullOrWhiteSpace(kontrolForm.birimi) || !string.IsNullOrWhiteSpace(kontrolForm.aciklama))
                    s.DegerAta(string.Format("{0}. satırda ödenek girilmemiş.", kontrolForm.siraNo));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(kontrolForm.yer))
                    s.DegerAta(string.Format("{0}. satırda yer girilmemiş.", kontrolForm.siraNo));

                if (string.IsNullOrWhiteSpace(kontrolForm.birimi))
                    s.DegerAta(string.Format("{0}. satırda birim girilmemiş.", kontrolForm.siraNo));

                if (string.IsNullOrWhiteSpace(kontrolForm.aciklama))
                    s.DegerAta(string.Format("{0}. satırda açıklama girilmemiş.", kontrolForm.siraNo));
            }
            return s;
        }

        /// <summary>
        /// AltProje nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AltProje yapısında AltProje: Alt Proje bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection formlar)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (formlar == null || formlar.Count == 0)
            {
                s.DegerAta("Kaydedilecek veri bulunamamıştır!");
                return s;
            }

            AltProjeDetayProgrami form = (AltProjeDetayProgrami)formlar[0];
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Formu değiştirme yetkiniz bulunmamaktadır!");
                return s;
            }
            double top = 0;
            foreach (AltProjeDetayProgrami item in formlar)
            {
                AlanlarGecerliMi(kullanan, form, item, s);
                top += item.odenek;
            }
            if (!s.islemSonuc)
            {
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATAltProje ap = new TABYATAltProje(ve);
            ObjectArray pmyler = ButceKaynakTeklifClass.MaliYapiBul(ve, kullanan, form.yil, form.senaryoKod, form.yil, form.senaryoKod, ENUMProjeMaliYapiKod.teklif0, 0, form.altProjeNo, false, false);
            double projeOdenek = 0;
            foreach (ProjeMaliYapisi item in pmyler.objeler)
            {
                projeOdenek += item.DegerAl(3);
            }
            if (Math.Round(projeOdenek, 2) != Math.Round(top, 2))
            {
                ve.VeriErisimKapat();
                s.DegerAta(string.Format("Detay program toplamı ({1}) ile proje ödeneği ({0}) eşit değil!", projeOdenek.ToString("#,##0"), top.ToString("#,##0")));
                return s;
            }

            TABYATALTPROJEDETAYPROGRAMI tabYATALTPROJEDETAYPROGRAMI = new TABYATALTPROJEDETAYPROGRAMI(ve);
            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabYATALTPROJEDETAYPROGRAMI.Sil(form);

            for (int i = 0; i < formlar.Count && s.islemSonuc; i++)
            {
                AltProjeDetayProgrami f = (AltProjeDetayProgrami)formlar[i];
                if (f.odenek != 0)
                    s = tabYATALTPROJEDETAYPROGRAMI.Ekle(f);
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
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, AltProjeDetayProgrami form)
        {
            return true;
        }

        /// <summary>
        /// AltProje listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AltProje yapısında AltProje: Alt Proje bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, AltProjeDetayProgrami form)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                oa.sonuc.DegerAta("Bilgileri görme yetkiniz bulunmamaktadır!");
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATALTPROJEDETAYPROGRAMI tabYATALTPROJEDETAYPROGRAMI = new TABYATALTPROJEDETAYPROGRAMI(ve);
            IDataReader dr = tabYATALTPROJEDETAYPROGRAMI.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                AltProjeDetayProgrami detay = new AltProjeDetayProgrami();

                detay.yil = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEDETAYPROGRAMI.ENUMAlanlar.YIL, 0);
                detay.senaryoKod = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEDETAYPROGRAMI.ENUMAlanlar.SENARYOKOD, "");
                detay.altProjeNo = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEDETAYPROGRAMI.ENUMAlanlar.ALTPROJENO, 0);

                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEDETAYPROGRAMI.ENUMAlanlar.SIRANO, 0);
                detay.yer = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEDETAYPROGRAMI.ENUMAlanlar.YER, "");
                detay.birimi = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEDETAYPROGRAMI.ENUMAlanlar.BIRIMI, "");
                detay.saymanlikKod = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEDETAYPROGRAMI.ENUMAlanlar.SAYMANLIKKOD, 0);
                detay.birimKod = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEDETAYPROGRAMI.ENUMAlanlar.BIRIMKOD, "");
                detay.odenek = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEDETAYPROGRAMI.ENUMAlanlar.ODENEK, 0.0);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABYATALTPROJEDETAYPROGRAMI.ENUMAlanlar.ACIKLAMA, "");
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }
    }
}

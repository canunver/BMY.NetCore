using System;
using System.Data;
using TNS.KYM;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class BHMv2ProjeAciklama
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeAciklama form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABProjeAciklama to = new TABProjeAciklama(ve);

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeAciklama[] liste)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            if (liste == null || liste.Length == 0)
                return sonuc;

            foreach (var form in liste)
            {
                sonuc = Kontrol("KAYDET", null, kullanan, form);
                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();


            sonuc = Sil(veriErisim, kullanan, new ProjeAciklama() { projeKod = liste[0].projeKod });

            if (sonuc.islemSonuc)
            {
                TABProjeAciklama to = new TABProjeAciklama(ve);
                foreach (var form in liste)
                {
                    sonuc = to.Kaydet(form);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeAciklama form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABProjeAciklama to = new TABProjeAciklama(ve);

                ve.BeginTransaction();
                sonuc = to.Sil(form);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeAciklama form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProjeAciklama to = new TABProjeAciklama(ve);
            IDataReader dr = to.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int sonSira = (int)TABProjeAciklama.ENUMAlanlar.ACIKLAMA + 1;

                    ProjeAciklama b = new ProjeAciklama();
                    b.projeKod = VeriErisim.DegerAl(dr, (int)TABProjeAciklama.ENUMAlanlar.PROJEKOD, "");
                    b.tur.kod = VeriErisim.DegerAl(dr, (int)TABProjeAciklama.ENUMAlanlar.TUR, "");
                    b.aciklama = VeriErisim.DegerAl(dr, (int)TABProjeAciklama.ENUMAlanlar.ACIKLAMA, "");

                    b.tur.ad = VeriErisim.DegerAl(dr, sonSira++, "");

                    liste.Ekle(b);
                }
                dr.Close();
            }
            else
            {
                liste.sonuc.hataStr = "İşlem sırasında beklenmedik bir hata oluştu:" + ve.hata;
            }
            ve.VeriErisimKapat();

            return liste;
        }

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, ProjeAciklama form)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (string.IsNullOrWhiteSpace(form.projeKod))
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }
            else if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (string.IsNullOrEmpty(form.projeKod))
                    sonuc.hataStr += "Öncelikle proje bilgisini kaydeniz." + "<br>";
                if (string.IsNullOrEmpty(form.tur.kod))
                    sonuc.hataStr += "Açıklama tür bilgisi boş bırakılamaz." + "<br>";
                if (string.IsNullOrEmpty(form.aciklama))
                    sonuc.hataStr += "Açıklama alanı boş bırakılamaz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

        internal static ObjectArray SPAmacHedefListele(VeriErisim veriErisim, Kullanici kullanan)
        {
            ObjectArray oa = new ObjectArray();
            oa.Ekle(SPAmacOl("A01", "Kırsal alanda refahı yükseltmek, tarımsal üretimde verim ve kaliteyi artırarak istikrarlı gıda arzını sağlamak"
                , new SPHedef("H01.01", "Bitkisel ürünlerde arz güvencesini sağlamak, yeni çeşit, metot ve teknoloji geliştirmek")
                , new SPHedef("H01.02", "Hayvansal ürünlerde arz güvencesini sağlamak, yeni ırk, metot ve teknoloji geliştirmek")
                , new SPHedef("H01.03", "Rekabet gücü yüksek, sürdürülebilir bir tarım sektörü için uygun politika araçlarını geliştirmek")
                , new SPHedef("H01.04", "Üreticilere ve tüketicilere yönelik eğitim stratejileri ve danışmanlık sistemi geliştirmek")
                , new SPHedef("H01.05", "Kırsalda gelir ve istihdam olanaklarını artırmak, kırsal ekonomiyi çeşitlendirmek")));
            oa.Ekle(SPAmacOl("A02", "Üretimden tüketime kadar gıda, yem güvenilirliğini sağlamak, bitki, hayvan sağlığı ve refahına yönelik gerekli tedbirleri almak"
                , new SPHedef("H02.01", "Gıda ve yem resmi kontrol hizmetlerinin etkinliğini artırmak")
                , new SPHedef("H02.02", "Gıda güvenilirliğine ilişkin doğru ve güncel bilgi sunmak, gıda güvenilirliğine yönelik uygulamaları geliştirmek")
                , new SPHedef("H02.03", "Bitkisel üretimde çevreye duyarlı bitki sağlığı tedbirleri almak")
                , new SPHedef("H02.04", "Hayvan hastalık ve zararlıları ile ilgili mücadele hizmetlerini geliştirmek, hayvan refahını korumak")
                , new SPHedef("H02.05", "Hayvan hastalıkları ile mücadelede kullanılan veteriner sağlık ürünlerinin kalite ve etkinliklerini artırmak")));
            oa.Ekle(SPAmacOl("A03", "Balıkçılık ve su ürünleri kaynaklarını korumak, sürdürülebilir işletimini sağlamak"
                , new SPHedef("H03.01", "Su ürünleri türlerinin stok durumlarını izlemek ve yönetim planlarını hazırlamak")
                , new SPHedef("H03.02", "Su ürünlerini ve kaynaklarını korumak için kontrol ve denetimlerin etkinliğini artırmak")
                , new SPHedef("H03.03", "Su ürünleri üretimini artırmak")));
            oa.Ekle(SPAmacOl("A04", "Toprak ve su kaynaklarının sürdürülebilir yönetimini sağlamak"
                , new SPHedef("H04.01", "Toprak ve su kaynaklarının korunmasını ve verimli kullanılmasını sağlamak")
                , new SPHedef("H04.02", "Taşkın ve kuraklığın olumsuz etkilerini kontrol altına almak maksadıyla yönetim planlarını hazırlamak")
                , new SPHedef("H04.03", "Yerüstü sularının kalite sınıflarını ve yeraltı sularının kalite ve miktar durumlarını belirlemek")
                , new SPHedef("H04.04", "Suyun miktar ve kalite olarak korunması ve kullanılması için AB mevzuatı ile uyumlu bütüncül planlamalar yapmak")));
            oa.Ekle(SPAmacOl("A05", "İklim değişikliği, çölleşme ve erozyonla etkin mücadele etmek"
                , new SPHedef("H05.01", "Çölleşme ve erozyonla mücadele kapasitesini artırmak,")
                , new SPHedef("H05.02", "Çölleşme/Arazi tahribatı ve erozyona maruz bölgelerin izleme değerlendirme alt yapısını oluşturmak")
                , new SPHedef("H05.03", "Çölleşme/Arazi tahribatı ve erozyonla mücadelede plan ve projeler hazırlamak")
                , new SPHedef("H05.04", "İklim değişikliğinin tarım üzerine olası etkilerini ölçmek ve tedbir almaya yönelik öneriler geliştirmek")));
            oa.Ekle(SPAmacOl("A06", "Biyolojik çeşitliliği korumak ve sürdürülebilir yönetimini sağlamak"
                , new SPHedef("H06.01", "Genetik kaynakları korumak, sürdürülebilir kullanımını sağlamak")
                , new SPHedef("H06.02", "2873 sayılı Milli Parklar Kanununa tabi korunan alanlarda etkin yönetimi sağlamak ve tabiat turizmini geliştirmek")
                , new SPHedef("H06.03", "Doğal, tarihi ve kültürel kaynak değerlere sahip alanları tespit ederek korunan alan olarak ilan etmek, sürdürülebilir yönetimini sağlamak")
                , new SPHedef("H06.04", "Biyolojik çeşitliliğin korunmasını ve devamlılığını sağlamak, bilinçlendirme çalışmaları yapmak")
                , new SPHedef("H06.05", "Yaban hayatı ve av yönetiminde sürdürülebilirliği sağlamak")));
            oa.Ekle(SPAmacOl("A07", " Kurumsal kapasiteyi geliştirmek"
                , new SPHedef("H07.01", "Uluslararası alanda daha etkin bir şekilde yer almak ve uluslararası örgütlerin karar süreçlerine dahil olmak")
                , new SPHedef("H07.02", "Bilgi sistemlerini geliştirmek ve bilgi güvenliğini sağlamak")
                , new SPHedef("H07.03", "Kurumsal yönetim sistemini etkin hale getirmek")
                , new SPHedef("H07.04", "Ar - Ge kapasitesini geliştirmek ve sonuçlarının uygulamaya aktarılmasını sağlamak")));

            return oa;
        }

        private static object SPAmacOl(string amacKod, string amacAd, params SPHedef[] hedefler)
        {
            SPAmac a = new SPAmac(amacKod, amacAd);
            foreach (var item in hedefler)
            {
                a.HedefEkle(item);
            }
            return a;
        }
    }
}

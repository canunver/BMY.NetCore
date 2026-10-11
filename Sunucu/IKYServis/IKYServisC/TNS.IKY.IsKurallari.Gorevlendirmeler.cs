using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using System.Text;

namespace TNS.IKY.IsKurallari
{
    /// <summary>
    /// IKYGorevlendirmeler formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class IKYGorevlendirmeler
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriIKY";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, Gorevler form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, Gorevler form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// Gorevlendirme nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Gorevlendirme yapısında Gorevlendirme: Görevlendirme bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Gorevler form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma yetkiniz bulunmamaktadır");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYGOREVLER tabIKYGOREVLER = new TABIKYGOREVLER(ve);

            ve.BeginTransaction();
            if (form.gorevNo == 0)
            {
                form.gorevNo = tabIKYGOREVLER.NoBul(form);
            }
            else
            {
                if (s.islemSonuc)
                    s = tabIKYGOREVLER.Sil(form);
            }
            if (s.islemSonuc)
                s = tabIKYGOREVLER.Ekle(form);

            if (s.islemSonuc)
            {
                s.anahtar = form.gorevNo.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Gorevlendirme nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Gorevlendirme yapısında Gorevlendirme: Görevlendirme bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Gorevler form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma yetkiniz bulunmamaktadır");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYGOREVLER tabIKYGOREVLER = new TABIKYGOREVLER(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabIKYGOREVLER.Sil(form);

            if (s.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Gorevlendirme nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Gorevlendirme yapısında Gorevlendirme: Görevlendirme bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, Gorevler form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, Gorevler form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, Gorevler form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// Gorevlendirme listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Gorevlendirme yapısında Gorevlendirme: Görevlendirme bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Gorevler form, bool yetkiKontrollu = false)
        {
            ObjectArray oa = new ObjectArray();
            if (yetkiKontrollu)
            {
                if (!GormeYetkisiVarMi(kullanan, form))
                {
                    oa.sonuc.DegerAta("Görme yetkiniz bulunmamaktadır");
                    return oa;
                }
            }
            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                oa.sonuc.DegerAta(s.hataStr);
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYGOREVLER tabIKYGOREVLER = new TABIKYGOREVLER(ve);

            IDataReader dr;
            dr = tabIKYGOREVLER.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            Oku(oa, dr);
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        private static void Oku(ObjectArray oa, IDataReader dr)
        {
            while (dr.Read())
            {
                Gorevler detay = new Gorevler();
                detay.pKod = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.PKOD, 0);
                detay.gorevNo = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.GOREVNO, 0);
                detay.unvanKod = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.UNVANKOD, 0);
                detay.gorevDurumu = (Gorevler.ENUMGorevDurumu)VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.GOREVDURUMU, 0);
                detay.birimi = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.BIRIMI, 0);
                detay.baslamaTarihi = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.BASLAMATARIHI);
                detay.bitisTarihi = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.BITISTARIHI);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.ACIKLAMA, "");
                detay.onayNo = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.ONAYNO, "");
                detay.onayTarihi = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.ONAYTARIHI);

                detay.birimiAd = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.ONAYTARIHI + 1, "");
                detay.unvanAd = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.ONAYTARIHI + 2, "");
                detay.unvanSira = VeriErisim.DegerAl(dr, (int)TABIKYGOREVLER.ENUMAlanlar.ONAYTARIHI + 3, 0);

                oa.Ekle(detay);
            }
        }

        internal static ObjectArray Listele(string okunanXML)
        {
            System.Collections.Generic.List<PersonelIzin> pVAN = new System.Collections.Generic.List<PersonelIzin>();
            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            ObjectArray oa = new ObjectArray();

            if (r != null)
            {
                Oku(oa, r);
            }

            return oa;
        }

        internal static ObjectArray DuyuruListesiAl(VeriErisim veriErisim, Kullanici kullanan, string kullaniciTipleri)
        {
            ObjectArray oa = new ObjectArray();

            GoreviBitenleriBul(kullanan, oa, kullaniciTipleri);
            ZorunluEmeklileriBul(kullanan, oa, kullaniciTipleri);

            return oa;
        }

        private static void GoreviBitenleriBul(Kullanici kullanan, ObjectArray oa, string kullaniciTipleri)
        {
            if (!TNS.DEG.Arac.ManasKullaniyor()) return;
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.PERSONEL_YONETICISI)) return;
            Personel p = new Personel();
            DateTime simdi = DateTime.Now.Date;
            TNSDateTime basla = new TNSDateTime(simdi.AddMonths(-1));
            TNSDateTime bitis = new TNSDateTime(simdi.AddMonths(1));

            ObjectArray perListe = IKYPersonel.Listele(null, kullanan, p, true, false, basla, bitis);

            if (perListe.sonuc.islemSonuc && perListe.ObjeSayisi > 0)
            {
                Duyuru duy = new Duyuru();
                duy.kod = -1;
                duy.onem = 1;
                duy.tarih = new TNSDateTime(DateTime.Now);
                duy.gizle = ENUMGizle.GORUNUR;
                duy.KullaniciTipleriOl("1,2,11");
                ENUMPencerede penceredemi = ENUMPencerede.GORUNMEZ;

                duy.konu = $"{basla.ToString()}-{bitis.ToString()} tarihleri arasında idari görev süresi dolacak personel listesi:";
                StringBuilder html = new StringBuilder();
                StyleDivAc(html, "personel_dinamik_gorev");
                html.Append("<div class='baslik_dinamik'>" + duy.konu + "</div>");
                html.Append("<table border='1' class='yaziAlan'>");
                KolonTanimYap(html);
                html.Append("<tr>");
                html.Append(HucreOl("Kişi", true));
                html.Append(HucreOl("Görevi", true));
                html.Append(HucreOl("Bitiş<br />Tarihi", true));
                html.Append("</tr>");
                int say = 0;
                foreach (Personel per in perListe.objeler)
                {
                    if (per.gorevler == null || per.gorevler.objeler == null) continue;
                    foreach (Gorevler gorev in per.gorevler.objeler)
                    {
                        html.Append("<tr>");
                        html.Append(HucreOl(per.unvanAd + "-" + per.adSoyad));
                        TimeSpan fark = gorev.bitisTarihi.Oku() - simdi;
                        if (fark.Days >= -1 && fark.Days < 7)
                            penceredemi = ENUMPencerede.GORUNUR;
                        html.Append(HucreOl(gorev.unvanAd));
                        html.Append(HucreOl(gorev.bitisTarihi.ToString(), false, "tarihKolon"));
                        html.Append("</tr>");
                        say++;
                    }
                }
                html.Append("</table>");
                StyleDivKapat(html);
                duy.pencerede = penceredemi;
                duy.duyuru = html.ToString();
                if (say > 0)
                    oa.Ekle(duy);
            }
            else if (!perListe.sonuc.islemSonuc)
            {
                OrtakFonksiyonlar.HataDosyaYaz("IKYDuyuru.txt", perListe.sonuc.hataStr, false);
            }
        }

        private static void KolonTanimYap(StringBuilder html)
        {
            html.Append("<colgroup>");
            html.Append("     <col style='width:200px;'>");
            html.Append("     <col style='width:120px;'>");
            html.Append("     <col style='width:80px;'>");
            html.Append("</colgroup>");
        }

        private static void StyleDivKapat(StringBuilder html)
        {
            html.Append("</div>");
        }

        private static void StyleDivAc(StringBuilder html, string divId)
        {
            html.Append($"<div id='{divId}'>");

            html.Append($"<style>");
            html.Append($"#{divId} table {{");
            html.Append($"  width:500px;");
            html.Append($"  table-layout:fixed;");
            html.Append($"  border-collapse:collapse;");
            html.Append($"}}");
            html.Append($"#{divId} th,");
            html.Append($"#{divId} td {{");
            html.Append($"  padding:6px;");
            html.Append($"  word-break:break-word;");
            html.Append($"}}");
            html.Append($"#{divId} th {{");
            html.Append($"  background-color:#f2f2f2; font-weight: bold; text-align: center; font-size: larger;");
            html.Append($"}}");
            html.Append($"#{divId} .tarihKolon {{");
            html.Append($"  text-align:center;");
            html.Append($"  white-space:nowrap;");
            html.Append($"}}");
            html.Append($"#{divId} .baslik_dinamik {{");
            html.Append($"  font-weight: bold; font-size: larger;padding-top: 10px;padding-bottom: 3px;");
            html.Append($"}}");
            html.Append($"</style>");
        }

        private static void ZorunluEmeklileriBul(Kullanici kullanan, ObjectArray oa, string kullaniciTipleri)
        {
            if (!TNS.DEG.Arac.ManasKullaniyor()) return;
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.PERSONEL_YONETICISI)) return;
            Personel p = new Personel();
            p.personelTipleri = "11, 14, 22, 17, 18";
            DateTime simdi = DateTime.Now.Date;
            TNSDateTime zorunluEmeklilikTarihi = new TNSDateTime(simdi.AddMonths(9));
            TNSDateTime simdiDT = new TNSDateTime(simdi);

            ObjectArray perListe = IKYPersonel.Listele(null, kullanan, p, true, false, null, null, zorunluEmeklilikTarihi);

            if (perListe.sonuc.islemSonuc && perListe.ObjeSayisi > 0)
            {
                Duyuru duy = new Duyuru();
                duy.kod = -1;
                duy.onem = 1;
                duy.tarih = new TNSDateTime(DateTime.Now);
                duy.gizle = ENUMGizle.GORUNUR;
                duy.KullaniciTipleriOl("1,2,11");
                ENUMPencerede penceredemi = ENUMPencerede.GORUNMEZ;
                duy.konu = $"{simdiDT.ToString()} - {zorunluEmeklilikTarihi.ToString()} tarihleri arasında yaş haddi dolacak personel listesi:";
                StringBuilder html = new StringBuilder();
                StyleDivAc(html, "personel_dinamik_gorev");
                html.Append("<div class='baslik_dinamik'>" + duy.konu + "</div>");
                html.Append("<table border='1' class='yaziAlan'>");
                KolonTanimYap(html);
                html.Append("<tr>");
                html.Append(HucreOl("Kişi", true));
                html.Append(HucreOl("Doğum Tarihi", true));
                html.Append(HucreOl("Ayrılış<br />Tarihi", true));
                html.Append("</tr>");
                int say = 0;
                foreach (Personel per in perListe.objeler)
                {
                    if (per.gorevler == null || per.gorevler.objeler == null) continue;
                    html.Append("<tr>");
                    html.Append(HucreOl(per.unvanAd + "-" + per.adSoyad));
                    //TimeSpan fark = gorev.bitisTarihi.Oku() - simdi;
                    //if (fark.Days >= -1 && fark.Days < 7)
                    //    penceredemi = ENUMPencerede.GORUNUR;
                    html.Append(HucreOl(per.dogumTarihi.ToString(), false, "tarihKolon"));
                    simdiDT.Yaz(per.dogumTarihi.Oku().AddYears(YasSiniriBul(per.personelTip)).AddDays(1));
                    html.Append(HucreOl(simdiDT.ToString(), false, "tarihKolon"));
                    html.Append("</tr>");
                    say++;
                }
                html.Append("</table>");
                StyleDivKapat(html);
                duy.pencerede = penceredemi;
                duy.duyuru = html.ToString();
                if (say > 0)
                    oa.Ekle(duy);
            }
            else if (!perListe.sonuc.islemSonuc)
            {
                OrtakFonksiyonlar.HataDosyaYaz("IKYDuyuru.txt", perListe.sonuc.hataStr, false);
            }
        }

        private static int YasSiniriBul(int personelTip)
        {
            switch (personelTip)
            {
                case 17:
                case 37:
                case 18:
                case 36:
                case 81:
                case 82:
                case 83:
                case 100:
                    return 65;
                default:
                    return 70;
            }
        }

        private static string HucreOl(string veri, bool th = false, string ekClassAd = null)
        {
            string ek;
            if (th) ek = "th";
            else ek = "td";
            if (ekClassAd != null)
                ek += " class='" + ekClassAd + "'";
            return "<" + ek + ">" + veri + "</" + ek + ">";
        }
    }
}

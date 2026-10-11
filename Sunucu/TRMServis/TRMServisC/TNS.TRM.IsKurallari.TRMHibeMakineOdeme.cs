using System.Data;
using TNS.TRM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using System;
using System.Collections;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeMakineOdeme
    {
        /// <summary>
        /// Arguman olarak verilen, hibe makine ödeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, HibeMakineSulamaOdeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = IslemKontrol(kullanan, bilgi, "Kaydet");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeMakineOdeme to = new TABHibeMakineOdeme(ve);
            ve.BeginTransaction();

            if (bilgi.kod == 0)
                bilgi.kod = BelgeNoAlInt(ve);

            if (bilgi.etapNo == 0)
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu. Aktif Etap bilgisi okunamadı.";
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                sonuc.anahtar = bilgi.kod.ToString();

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, hibe makine ödeme emri no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetOdemeEmriNo(Kullanici kullanan, HibeMakineSulamaOdeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = IslemKontrol(kullanan, bilgi, "OdemeEmriNo");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakineOdeme to = new TABHibeMakineOdeme(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetOdemeEmriNo(bilgi);
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                sonuc.anahtar = bilgi.kod.ToString();

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();


            return sonuc;
        }

        public Sonuc KaydetOdendi(Kullanici kullanan, int kod, int tur)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakineOdeme to = new TABHibeMakineOdeme(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetOdendi(kod, tur);
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kaydetme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();


            return sonuc;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hibe makine ödeme bilgisini veritabanından okur 
        /// ve HibeMakine nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hibe makine listesini HibeMakine nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, HibeMakineSulamaOdeme kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            Sonuc sonuc = new Sonuc();

            string ilKod = string.Empty;
            if (!Yetki.YetkiVarMiOkumaya(kullanan, ilKod, (ENUMHibeTur)OrtakFonksiyonlar.ConvertToInt(kriter.tipi, 0)))
            {
                sonuc.hataStr = "Okuma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                bilgi.sonuc = sonuc;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));

            TABHibeMakineOdeme to = new TABHibeMakineOdeme(ve);

            if (kriter.kod == 0)//Tek kayıt çağrısında bulunmuyor ise
            {
                //İl kullanıcısı yetkisi için
                //*****************************************************************************
                string[] yetkiliOlduguIller = TNS.TRM.Yetki.YetkiliOlduguIlleriGetir(kullanan);
                if (ilKod.Trim() == "")
                {
                    foreach (string il in yetkiliOlduguIller)
                    {
                        if (il.Trim() == "")
                            continue;
                        if (ilKod != "") ilKod += ";";
                        ilKod += il;
                    }
                }
                //*****************************************************************************
            }


            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                int sonKolon = ((int)TABHibeMakine.ENUMAlanlar.ETAPNO) + 1;
                int sonKolon2 = sonKolon + (int)TABHibeMakine.ENUMAlanlarDetay.TOPLAMTUTAR;
                while (r.Read())
                {
                    HibeMakineSulamaOdeme f = new HibeMakineSulamaOdeme();

                    f.kod = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.KOD, 0);
                    f.etapNo = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.ETAPNO, 0);
                    f.turKod = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.TURKOD, 0);
                    f.gerekce = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.GEREKCE, string.Empty);
                    f.konu = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.KONU, string.Empty);
                    f.gerceklestirmeSure = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.GERSURE);
                    f.gerceklestirmeUsulu = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.GERUSUL, string.Empty);
                    f.tertip = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.TERTIP, string.Empty);
                    f.odemeHesap = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.ODEMEHESAP, string.Empty);

                    f.hibe = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.HIBE, decimal.Zero);
                    f.komisyon = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.KOMISYON, decimal.Zero);

                    f.miktar = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.MIKTAR, decimal.Zero);
                    f.yaklasikBedel = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.YAKLASIKBEDEL, decimal.Zero);
                    f.odenek = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.ODENEK, decimal.Zero);
                    f.gerceklestirmeGorevlileri = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.GERGOREVLI, string.Empty);
                    f.aciklama = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.ACIKLAMA, string.Empty);

                    f.koordinatorAd = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.KOORDINATORAD, string.Empty);
                    f.koordinatorUnvan = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.KOORDINATORUNVAN, string.Empty);

                    f.teklifEdenAd = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.TEKLIFEDENAD, string.Empty);
                    f.teklifEdenUnvan = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.TEKLIFEDENUNVAN, string.Empty);
                    f.harcamaYetkilisiAd = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.HARYETKILIAD, string.Empty);
                    f.harcamaYetkilisiUnvan = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.HARYETKILIUNVAN, string.Empty);
                    f.odemeEmriYil = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.ODEMEEMRIYIL, 0);
                    f.odemeEmriNo = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.ODEMEEMRINO, string.Empty);
                    f.muhasebe = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.MUHASEBE, string.Empty);
                    f.birim = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.BIRIM, string.Empty);
                    f.vergiNo = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.VERGINO, string.Empty);
                    f.odemeYapildi = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.ODEMEYAPILDI, 0);
                    f.odemeYapilmaTarihi = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.ODEMEYAPILMATARIHI);

                    f.hazirlayan1Ad = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.HAZIRLAYAN1AD, string.Empty);
                    f.hazirlayan1Unvan = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.HAZIRLAYAN1UNVAN, string.Empty);
                    f.hazirlayan2Ad = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.HAZIRLAYAN2AD, string.Empty);
                    f.hazirlayan2Unvan = VeriErisim.DegerAl(r, (int)TABHibeMakineOdeme.ENUMAlanlar.HAZIRLAYAN2UNVAN, string.Empty);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public Sonuc Sil(Kullanici kullanan, HibeMakineSulamaOdeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = IslemKontrol(kullanan, bilgi, "Sil");
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakineOdeme to = new TABHibeMakineOdeme(ve);
            ve.BeginTransaction();

            sonuc = to.SulamaveGrubunuTemizle(bilgi);
            if (sonuc.islemSonuc)
                sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                sonuc.hataStr = "Silme işlemi sırasında hata oluştu." + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }


        private int BelgeNoAlInt(VeriErisim ve)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKod = "HIBEMAKINEODEME";

            return tg.BelgeNoAlInt(sayacKod, "000000");
        }

        public string IslemKontrol(Kullanici kullanan, HibeMakineSulamaOdeme bilgi, string tur)
        {
            string hata = "";
            Sonuc s = new Sonuc();
            if (!Yetki.YetkiVarMiYazmaya(kullanan, "", bilgi.etapNo, s, "OdemeIslem", (ENUMHibeTur)OrtakFonksiyonlar.ConvertToInt(bilgi.tipi, 0)))
                hata += s.hataStr + "<br>";

            HibeMakineSulamaOdeme odemeKaydi = new HibeMakineSulamaOdeme();
            if (bilgi.kod > 0)
            {
                ObjectArray hmler = Listele(kullanan, new HibeMakineSulamaOdeme { kod = bilgi.kod, tipi = bilgi.tipi });
                if (hmler != null && !hmler.sonuc.islemSonuc)
                    hata += hmler.sonuc.hataStr + "<br>";
                else if (hmler == null || hmler.objeler.Count == 0)
                    hata += "İşlem yapılacak kayıta ait bilgi okunamadı.<br>";

                if (hmler != null && hmler.objeler.Count > 0)
                {
                    odemeKaydi = (HibeMakineSulamaOdeme)hmler.objeler[0];
                }
            }

            if (tur == "Kaydet")
            {
                if (bilgi.turKod == 0)
                    hata += "Tür seçimi boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.gerekce))
                    hata += "Gerekçesi ve hukuki dayanağı alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.konu))
                    hata += "Konusu/Nev'i/Niteliği alanı boş bırakılamaz.<br>";
                if (bilgi.gerceklestirmeSure.isNull)
                    hata += "Gerçekleştirme Süresi alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.gerceklestirmeUsulu))
                    hata += "Gerçekleştirme Usulü alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.tertip))
                    hata += "Ödeneğin Bütçe Tertibi alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.odemeHesap))
                    hata += "Ödeme Hesabı alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.gerceklestirmeGorevlileri))
                    hata += "Gerçekleştirme Görevlileri alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.aciklama))
                    hata += "Açıklamalar alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.teklifEdenAd))
                    hata += "Teklif Eden Yetkilinin Adı Soyadı alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.teklifEdenUnvan))
                    hata += "Teklif Eden Yetkilinin Unvanı alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.harcamaYetkilisiAd))
                    hata += "Harcama Yetkilisi Adı Soyadı alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.harcamaYetkilisiUnvan))
                    hata += "Harcama Yetkilisinin Unvanı alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.muhasebe))
                    hata += "Muhasebe alanı boş bırakılamaz.<br>";
                if (string.IsNullOrEmpty(bilgi.birim))
                    hata += "Birim alanı boş bırakılamaz.<br>";
            }
            else if (tur == "OdemeEmriNo")
            {
                if (bilgi.odemeEmriYil == 0)
                    hata += "Ödeme Emri Yıl bilgisi bulunamadı.<br>";
                if (string.IsNullOrEmpty(bilgi.odemeEmriNo))
                    hata += "Ödeme Emri No bilgisi bulunamadı.<br>";
            }
            else if (tur == "Sil")
            {
                if (bilgi.kod == 0)
                    hata += "Kod bilgisi bulunamadı.<br>";
                if (string.IsNullOrEmpty(bilgi.tipi))
                    hata += "Tipi bilgisi bulunamadı.<br>";
                if (bilgi.tipi == "3")
                {
                    hata += "Ekonomik yatırımlar için silme işlemi yapılamaz<br>";
                }
                if (odemeKaydi.odemeYapildi > 0)
                {
                    hata += "Bu ödeme bilgisi durumundan dolayı silinemez.<br>";
                }

            }

            return hata;
        }
    }
}

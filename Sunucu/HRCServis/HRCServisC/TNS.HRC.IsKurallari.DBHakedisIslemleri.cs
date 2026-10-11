using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;
using static TNS.HRC.KodAdYonetim;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// HRCDBIkrazlar formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class DBHakedisIslemleri
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        internal static ObjectArray ListeleDBHakedis(VeriErisim veriErisim, Kullanici kullanan, DBHakedis hakedisKistas, bool detayOku, int siralaYontem)
        {
            ObjectArray oa = new ObjectArray();

            if (hakedisKistas.hakedisID == 0 && !DBIkrazlar.GormeYetkisiVarMi(kullanan, hakedisKistas.altBilesenID))
            {
                oa.sonuc.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                return oa;
            }

            //Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    oa.sonuc.DegerAta(s.hataStr);
            //    return oa;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBHAKEDIS tabHakedis = new TABDBHAKEDIS(ve);
            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);

            IDataReader dr;
            dr = tabHakedis.Listele(hakedisKistas, detayOku, siralaYontem);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                DBHakedis detay = new DBHakedis();
                int kol = 0;
                detay.hakedisID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.butceRefID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.ilgiliKod = VeriErisim.DegerAl(dr, kol++, 0);
                detay.hakedisTurKod = VeriErisim.DegerAl(dr, kol++, 0);
                detay.sozlesmeID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.belgeTurKod = VeriErisim.DegerAl(dr, kol++, 0);
                detay.belgeTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));
                detay.belgeNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.paraBirimi = VeriErisim.DegerAl(dr, kol++, "");
                detay.kurTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));
                detay.kdvHaricBelgeToplami = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.kdvGirisTurKod = VeriErisim.DegerAl(dr, kol++, 0);
                detay.kdvOrani = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.kdvTutari = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.avansKesintisi = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.durumKod = VeriErisim.DegerAl(dr, kol++, 0);
                detay.hakedisGerekce = VeriErisim.DegerAl(dr, kol++, "");
                detay.hakedisNo = VeriErisim.DegerAl(dr, kol++, 0);
                detay.numaratorAlmaTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));

                detay.butceRefAd = VeriErisim.DegerAl(dr, kol++, "");
                detay.butceRefRefNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.butceKategori = VeriErisim.DegerAl(dr, kol++, "");
                detay.butceSPRefNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.onceSonra = VeriErisim.DegerAl(dr, kol++, 0);
                detay.altBilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.altBilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.altBilesenKurKod = VeriErisim.DegerAl(dr, kol++, "");
                detay.altBilesenAd = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliBirimAdi = VeriErisim.DegerAl(dr, kol++, "");
                detay.ikrazID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.ikrazAnlasamaAdi = VeriErisim.DegerAl(dr, kol++, "");
                detay.ikrazDonusumKuru = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.ikrazParaBirimi = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliVNO = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliAd = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliIBAN = VeriErisim.DegerAl(dr, kol++, "");
                detay.kurUSD = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.kurEUR = VeriErisim.DegerAl(dr, kol++, 0.0);

                detay.sozlesmeTanim = VeriErisim.DegerAl(dr, kol++, "");
                detay.lotNo = VeriErisim.DegerAl(dr, kol++, 0);
                detay.lotAciklama = VeriErisim.DegerAl(dr, kol++, "");

                if (hakedisKistas.hakedisID > 0 && !DBIkrazlar.GormeYetkisiVarMi(kullanan, detay.altBilesenID))
                {
                    oa.sonuc.DegerAta($"${detay.altBilesenKurKod} birimiyle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                    break;
                }
                else
                    oa.Ekle(detay);

                string hakedisSahipleri = VeriErisim.DegerAl(dr, kol++, "");
                string yolluklar = VeriErisim.DegerAl(dr, kol++, "");
                string malzemeler = VeriErisim.DegerAl(dr, kol++, "");
                string tarihceler = VeriErisim.DegerAl(dr, kol++, "");
                string odenekler = VeriErisim.DegerAl(dr, kol++, "");

                if (!string.IsNullOrWhiteSpace(hakedisSahipleri))
                {
                    XMLStringDataReader r = new XMLStringDataReader(hakedisSahipleri);
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            detay.hakedisSahipleri.Add(HakedisSahibiDetayOku(detay, r));
                        }
                        r.Close();
                    }
                }
                if (!string.IsNullOrWhiteSpace(yolluklar))
                {
                    XMLStringDataReader r = new XMLStringDataReader(yolluklar);
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            DBYollukBilgisi detayYB = new DBYollukBilgisi();
                            detayYB.siraNo = VeriErisim.DegerAl(r, (int)TABDBHAKEDIS.ENUMAlanlarDBYOLLUK.SIRANO, 0);
                            detayYB.yer = VeriErisim.DegerAl(r, (int)TABDBHAKEDIS.ENUMAlanlarDBYOLLUK.YER, "");
                            detayYB.aciklama = VeriErisim.DegerAl(r, (int)TABDBHAKEDIS.ENUMAlanlarDBYOLLUK.ACIKLAMA, "");
                            detayYB.tutar = VeriErisim.DegerAl(r, (int)TABDBHAKEDIS.ENUMAlanlarDBYOLLUK.TUTAR, 0.0);
                            detay.yollukBilgileri.Add(detayYB);
                        }
                        r.Close();
                    }
                }
                if (!string.IsNullOrWhiteSpace(malzemeler))
                {
                    XMLStringDataReader r = new XMLStringDataReader(malzemeler);
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            DBHakedisMalzeme detayM = new DBHakedisMalzeme();
                            detayM.siraNo = VeriErisim.DegerAl(r, (int)TABDBHAKEDIS.ENUMAlanlarMALZEME.SIRANO, 0);
                            detayM.malzemeAdi = VeriErisim.DegerAl(r, (int)TABDBHAKEDIS.ENUMAlanlarMALZEME.MALZEMEADI, "");
                            detayM.miktar = VeriErisim.DegerAl(r, (int)TABDBHAKEDIS.ENUMAlanlarMALZEME.MIKTAR, 0);
                            detayM.birimFiyat = VeriErisim.DegerAl(r, (int)TABDBHAKEDIS.ENUMAlanlarMALZEME.BIRIMFIYAT, 0.0);
                            detay.malzemeListesi.Add(detayM);
                        }
                        r.Close();
                    }
                }
                if (!string.IsNullOrWhiteSpace(tarihceler))
                {
                    XMLStringDataReader r = new XMLStringDataReader(tarihceler);
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            DBHakedisTarihce detayT = new DBHakedisTarihce();
                            detayT.islemTarihi = VeriErisim.DegerAl(r, (int)TABDBTARIHCE.ENUMAlanlar.ISLEMTARIHI);
                            detayT.islem = VeriErisim.DegerAl(r, (int)TABDBTARIHCE.ENUMAlanlar.ISLEM, "");
                            detayT.islemiYapan = VeriErisim.DegerAl(r, (int)TABDBTARIHCE.ENUMAlanlar.ISLEMIYAPAN, "");
                            detayT.ekBilgi = VeriErisim.DegerAl(r, (int)TABDBTARIHCE.ENUMAlanlar.EKBILGI, "");
                            detay.tarihceListesi.Add(detayT);
                        }
                        r.Close();
                    }
                }
                if (!string.IsNullOrWhiteSpace(odenekler))
                {
                    XMLStringDataReader r = new XMLStringDataReader(odenekler);
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            DBHakedisOdenek odeme = new DBHakedisOdenek();
                            int i = 0;
                            odeme.hakedisID = VeriErisim.DegerAl(r, i++, 0);
                            odeme.ekoKod = OrtakFonksiyonlar.KodNoktaKoy("99.99", VeriErisim.DegerAl(r, i++, ""));
                            odeme.miktar = VeriErisim.DegerAl(r, i++, 0.0);
                            odeme.ekoAd = odeme.ekoKod + "- " + VeriErisim.DegerAl(r, i++, "");

                            detay.odenekListesi.Add(odeme); // burası TNSCollection olan alan
                        }
                        r.Close();
                    }
                }
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        private static void HakSahipleriOku(TABDBHAKEDIS tabHakedis, DBHakedis formIc, Sonuc sonuc)
        {
            IDataReader dr = tabHakedis.ListeleHakSahibi(new DBHakedisSahibi() { hakedisID = formIc.hakedisID });
            if (dr == null)
            {
                sonuc.DegerAta(tabHakedis.Hata());
                return;
            }
            while (dr.Read())
            {
                formIc.hakedisSahipleri.Add(HakedisSahibiDetayOku(formIc, dr));
            }
            dr.Close();
        }

        private static DBHakedisSahibi HakedisSahibiDetayOku(DBHakedis formIc, IDataReader dr)
        {
            int kol = 0;
            DBHakedisSahibi detay = new DBHakedisSahibi();
            detay.hakedisID = VeriErisim.DegerAl(dr, kol++, 0);
            detay.siraNo = VeriErisim.DegerAl(dr, kol++, 0);
            detay.ilgiliKod = VeriErisim.DegerAl(dr, kol++, 0);
            detay.kesintiTurKod = (enumKesintiTuru)VeriErisim.DegerAl(dr, kol++, 0);
            detay.aciklama = VeriErisim.DegerAl(dr, kol++, "");
            detay.kesintiTutari = VeriErisim.DegerAl(dr, kol++, 0.0);
            detay.numarator = VeriErisim.DegerAl(dr, kol++, 0);
            detay.merkezNumaratorID = VeriErisim.DegerAl(dr, kol++, 0);
            detay.odendi = VeriErisim.DegerAl(dr, kol++, 0);
            detay.odemeTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));
            detay.ilgiliVNO = VeriErisim.DegerAl(dr, kol++, "");
            detay.ilgiliAd = VeriErisim.DegerAl(dr, kol++, "");
            detay.ilgiliIBAN = VeriErisim.DegerAl(dr, kol++, "");
            detay.aktarimKurUSD = VeriErisim.DegerAl(dr, kol++, 0.0);
            detay.aktarimKurEUR = VeriErisim.DegerAl(dr, kol++, 0.0);

            detay.paraBirimi = formIc.paraBirimi;
            detay.kurEUR = formIc.kurEUR;
            detay.kurUSD = formIc.kurUSD;
            return detay;
        }

        private static void HakSahipleriOkuNumarator(TABDBHAKEDIS tabHakedis, DBHakedisSahibi formIc, Sonuc sonuc, TNSCollection liste)
        {
            IDataReader dr = tabHakedis.ListeleHakSahibiNumarator(formIc);
            if (dr == null)
            {
                sonuc.DegerAta(tabHakedis.Hata());
                return;
            }
            while (dr.Read())
            {
                int kol = 0;
                DBHakedisSahibi detay = new DBHakedisSahibi();
                detay.hakedisID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.numarator = VeriErisim.DegerAl(dr, kol++, 0);
                detay.merkezNumaratorID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.ilgiliKod = VeriErisim.DegerAl(dr, kol++, 0);
                detay.ilgiliVNO = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliAd = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliIBAN = VeriErisim.DegerAl(dr, kol++, "");
                detay.bankaSube = VeriErisim.DegerAl(dr, kol++, "");
                detay.hakedisGerekce = VeriErisim.DegerAl(dr, kol++, "");
                detay.paraBirimi = VeriErisim.DegerAl(dr, kol++, "");
                detay.butceKategori = VeriErisim.DegerAl(dr, kol++, "");
                detay.kurUSD = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.kurEUR = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.kesintiTutari = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.kesintiTurKod = (enumKesintiTuru)VeriErisim.DegerAl(dr, kol++, 0);
                liste.Add(detay);
            }
            dr.Close();
        }

        internal static ObjectVAN OkuDBHakedis(VeriErisim veriErisim, Kullanici kullanan, int hakedisID, bool detayli = true)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleDBHakedis(veriErisim, kullanan, new DBHakedis() { hakedisID = hakedisID }, detayli, 0);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0)
            {
                DBHakedis okunan = (DBHakedis)s[0];
                okunan.yetkiliKullanici = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI, (int)ENUMKullaniciTipi.SISTEMYONETICISI);
                ov.o = okunan;
            }
            return ov;
        }

        internal static Sonuc HakedisYazilabilirMi(DBHakedis hakedis, Sonuc sonuc, bool odemeSonrasi, bool sistemYoneticisi)
        {
            if (!odemeSonrasi)
            {
                if (hakedis.durumKod == 1 || hakedis.durumKod == 2) return sonuc;
                if (sistemYoneticisi) return sonuc;
                sonuc.DegerAta("Yeni durumda olmayan belgeler değiştirilemez!");
            }
            else
            {
                if (hakedis.durumKod == 4 || hakedis.durumKod == 5) return sonuc;
                sonuc.DegerAta("Sadece onaylanmış veya numara vermiş belgeler değiştirilebilir!");
            }
            return sonuc;
        }

        /// <summary>
        /// DBHakedis nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBHakedis yapısında DBHakedis:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetDBHakedis(VeriErisim veriErisim, Kullanici kullanan, DBHakedis form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!DBIkrazlar.YazmaYetkisiVarMi(kullanan, form.altBilesenID))
            {
                s.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
            }

            if (form.butceRefID <= 0)
            {
                s.DegerAta("Bütçe referansı olmadan kayıt yapılamaz!");
            }

            if (form.ilgiliKod <= 0)
            {
                s.DegerAta("İlgilisi olmayan hakediş kaydı yapılamaz!");
            }

            if (form.hakedisTurKod <= 0)
            {
                s.DegerAta("Türü olmayan hakediş kaydı yapılamaz!");
            }
            else
            {
                if (form.hakedisTurKod == 3 || form.hakedisTurKod == 4)
                {
                    if (form.sozlesmeID > 0) s.DegerAta("Yolluk hakedişlerinde sözleşme seçmeyiniz!");
                }
                else if (form.hakedisTurKod != 10)
                {
                    if (form.sozlesmeID == 0) s.DegerAta("Sözleşme hakedişleri için sözleşme seçilmelidir!");
                }
            }

            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBHAKEDIS tabHakedis = new TABDBHAKEDIS(ve);
            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);

            if (form.hakedisID != 0)
            {
                ObjectVAN ov = DBHakedisIslemleri.OkuDBHakedis(null, kullanan, form.hakedisID, false);
                if (ov.sonuc.islemSonuc)
                    DBHakedisIslemleri.HakedisYazilabilirMi((DBHakedis)ov.o, ov.sonuc, false, kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI));
                if (!ov.sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return ov.sonuc;
                }
            }

            ve.BeginTransaction();
            string islemAd;
            if (form.hakedisID == 0)
            {
                islemAd = "Ekleme";
                if (s.islemSonuc)
                    s = tabHakedis.Ekle(form);
                if (s.islemSonuc)
                    form.hakedisID = OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0);
            }
            else
            {
                islemAd = "Güncelleme";
                if (s.islemSonuc)
                    s = tabHakedis.SilMalzeme(form);
                if (s.islemSonuc)
                    s = tabHakedis.SilYolluk(form);
                if (s.islemSonuc)
                    s = tabHakedis.SilHakSahibi(form);
                if (s.islemSonuc)
                    s = tabHakedis.SilHakedisOdenek(form);
                if (s.islemSonuc)
                    s = tabHakedis.Guncelle(form);
            }

            for (int i = 0; i < form.hakedisSahipleri.Count && s.islemSonuc; i++)
            {
                s = tabHakedis.EkleHakSahibi(form, i);
            }

            for (int i = 0; i < form.yollukBilgileri.Count && s.islemSonuc; i++)
            {
                s = tabHakedis.EkleYolluk(form, i);
            }

            for (int i = 0; i < form.malzemeListesi.Count && s.islemSonuc; i++)
            {
                s = tabHakedis.EkleMalzeme(form, i);
            }

            for (int i = 0; i < form.odenekListesi.Count && s.islemSonuc; i++)
            {
                s = tabHakedis.EkleHakedisOdenek(form, i);
            }

            if (s.islemSonuc)
                s = tabTarihce.Ekle("DBH", form.hakedisID.ToString(), kullanan.kullaniciKodu, islemAd, "");

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            s.anahtar = form.hakedisID.ToString();
            return s;
        }

        public static Sonuc GuncelleDBHakedisSahibi(VeriErisim veriErisim, Kullanici kullanan, DBHakedisSahibi form)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI))
                return new Sonuc("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");

            if (form == null || form.hakedisID <= 0 || form.siraNo < 0)
                return new Sonuc("Güncellenecek hak sahibi satırı geçersiz!");
            if (form.ilgiliKod <= 0)
                return new Sonuc("Ödeme yapılacak kişi seçilmelidir!");
            if ((int)form.kesintiTurKod <= 0)
                return new Sonuc("Kesinti türü seçilmelidir!");
            if (form.kesintiTutari < 0)
                return new Sonuc("Tutar negatif olamaz!");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBHAKEDIS tabHakedis = new TABDBHAKEDIS(ve);
            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);
            DBHakedis hakedis = new DBHakedis() { hakedisID = form.hakedisID };
            HakSahipleriOku(tabHakedis, hakedis, sonuc);
            DBHakedisSahibi eski = null;
            foreach (DBHakedisSahibi item in hakedis.hakedisSahipleri)
                if (item.siraNo == form.siraNo) { eski = item; break; }

            if (sonuc.islemSonuc && eski == null)
                sonuc.DegerAta("Güncellenecek hak sahibi satırı bulunamadı!");

            ObjectVAN ov = null;
            if (sonuc.islemSonuc)
            {
                ov = OkuDBHakedis(ve, kullanan, form.hakedisID, false);
                sonuc = ov.sonuc;
                if (sonuc.islemSonuc && (ov.o == null || !DBIkrazlar.YazmaYetkisiVarMi(kullanan, ((DBHakedis)ov.o).altBilesenID)))
                    sonuc.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                else if (sonuc.islemSonuc && ((DBHakedis)ov.o).durumKod != 5 && ((DBHakedis)ov.o).durumKod != 6)
                    sonuc.DegerAta("Yalnızca yeni duruma iade edilemeyen hakedişlerin hak sahibi satırları düzeltilebilir!");
            }

            if (sonuc.islemSonuc)
            {
                bool transactionAcik = false;
                try
                {
                    ve.BeginTransaction();
                    transactionAcik = true;
                    sonuc = tabHakedis.GuncelleHakSahibi(form);
                    if (sonuc.islemSonuc)
                    {
                        string bilgi = string.Format("Satır {0}; ilgili {1}->{2}; kesinti türü {3}->{4}; tutar {5:#,##0.00}->{6:#,##0.00}; açıklama: {7}",
                            form.siraNo, eski.ilgiliKod, form.ilgiliKod, (int)eski.kesintiTurKod, (int)form.kesintiTurKod,
                            eski.kesintiTutari, form.kesintiTutari, form.aciklama);
                        sonuc = tabTarihce.Ekle("DBH", form.hakedisID.ToString(), kullanan.kullaniciKodu, "Hak sahibi satır düzeltme", bilgi);
                    }
                    if (sonuc.islemSonuc) ve.Commit(); else ve.Rollback();
                    transactionAcik = false;
                }
                catch (Exception e)
                {
                    sonuc.DegerAta(e.ToString());
                }
                finally
                {
                    if (transactionAcik) ve.Rollback();
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc DBHakedisSahibiNumaraSifirla(VeriErisim veriErisim, Kullanici kullanan, int hakedisID, int siraNo)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI))
                return new Sonuc("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ov = OkuDBHakedis(ve, kullanan, hakedisID);
            Sonuc sonuc = ov.sonuc;
            DBHakedis hakedis = ov.o as DBHakedis;
            DBHakedisSahibi satir = null;
            if (sonuc.islemSonuc && hakedis == null)
                sonuc.DegerAta("Hakediş bulunamadı!");
            if (sonuc.islemSonuc && !DBIkrazlar.YazmaYetkisiVarMi(kullanan, hakedis.altBilesenID))
                sonuc.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
            if (sonuc.islemSonuc)
            {
                foreach (DBHakedisSahibi item in hakedis.hakedisSahipleri)
                    if (item.siraNo == siraNo) { satir = item; break; }
                if (satir == null || satir.merkezNumaratorID <= 0)
                    sonuc.DegerAta("Merkez Bankasına gönderilmiş hak sahibi satırı bulunamadı!");
                else if (satir.odendi != 0)
                    sonuc.DegerAta("Ödeme işlemi tamamlanmış satırın numarası sıfırlanamaz!");
            }

            if (sonuc.islemSonuc)
            {
                TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);
                TABDBTARIHCE tarihce = new TABDBTARIHCE(ve);
                bool transactionAcik = false;
                try
                {
                    ve.BeginTransaction();
                    transactionAcik = true;
                    sonuc = tab.HakSahibiNumaraSifirla(hakedisID, siraNo);
                    if (sonuc.islemSonuc)
                        sonuc = tarihce.Ekle("DBH", hakedisID.ToString(), kullanan.kullaniciKodu, "Hak sahibi numarası sıfırlandı",
                            string.Format("Satır {0}; numaratör {1}->0; merkez numaratör {2}->boş", siraNo, satir.numarator, satir.merkezNumaratorID));
                    if (sonuc.islemSonuc) ve.Commit(); else ve.Rollback();
                    transactionAcik = false;
                }
                catch (Exception e)
                {
                    sonuc.DegerAta(e.ToString());
                }
                finally
                {
                    if (transactionAcik) ve.Rollback();
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc DBHakedisSifirNumaralariAta(VeriErisim veriErisim, Kullanici kullanan, int hakedisID)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI))
                return new Sonuc("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ov = OkuDBHakedis(ve, kullanan, hakedisID);
            Sonuc sonuc = ov.sonuc;
            DBHakedis hakedis = ov.o as DBHakedis;
            if (sonuc.islemSonuc && hakedis == null)
                sonuc.DegerAta("Hakediş bulunamadı!");
            if (sonuc.islemSonuc && !DBIkrazlar.YazmaYetkisiVarMi(kullanan, hakedis.altBilesenID))
                sonuc.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
            if (sonuc.islemSonuc && hakedis.durumKod != 5 && hakedis.durumKod != 6)
                sonuc.DegerAta("Numara verme işlemi için hakedişin numara verilmiş aşamada olması gerekir!");
            if (sonuc.islemSonuc)
            {
                bool sifirSatirVar = false;
                foreach (DBHakedisSahibi item in hakedis.hakedisSahipleri)
                    if (item.numarator == 0) { sifirSatirVar = true; break; }
                if (!sifirSatirVar)
                    sonuc.DegerAta("Numarası 0 olan uygun bir hak sahibi satırı bulunamadı!");
            }

            if (sonuc.islemSonuc)
            {
                TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);
                TABDBTARIHCE tarihce = new TABDBTARIHCE(ve);
                bool transactionAcik = false;
                try
                {
                    ve.BeginTransaction();
                    transactionAcik = true;
                    sonuc = tab.NumaraAta(hakedis.ikrazID, hakedisID);
                    if (sonuc.islemSonuc)
                        sonuc = tarihce.Ekle("DBH", hakedisID.ToString(), kullanan.kullaniciKodu, "Sıfır numaralı hak sahiplerine numara verildi", "Mevcut numaralar korunarak yalnızca numarası 0 olan satırlar numaralandırıldı.");
                    if (sonuc.islemSonuc) ve.Commit(); else ve.Rollback();
                    transactionAcik = false;
                }
                catch (Exception e)
                {
                    sonuc.DegerAta(e.ToString());
                }
                finally
                {
                    if (transactionAcik) ve.Rollback();
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static Sonuc SilDBHakedis(VeriErisim veriErisim, Kullanici kullanan, int hakedisID)
        {
            Sonuc s = new Sonuc();
            if (hakedisID <= 0)
            {
                s.DegerAta("Silinecek hakediş bilgisi hatalı!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));


            ObjectVAN ov = OkuDBHakedis(ve, kullanan, hakedisID);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }

            if (ov.o == null)
            {
                ve.VeriErisimKapat();
                return new Sonuc("Hakediş bulunamadı!");
            }

            DBHakedis hakedis = null;
            hakedis = (DBHakedis)ov.o;

            if (hakedis.hakedisSahipleri.Count > 0)
            {
                ve.VeriErisimKapat();
                return new Sonuc("İlk önce bu hakedişin hakediş sahibini silmeniz gerekiyor!");
            }

            TABDBHAKEDIS tabHakedis = new TABDBHAKEDIS(ve);
            try
            {
                ve.BeginTransaction();

                s = tabHakedis.SilYollukBilgisi(hakedisID);
                if (!s.islemSonuc)
                {
                    ve.Rollback();
                    return s;
                }

                s = tabHakedis.SilHakedisMalzeme(hakedisID);
                if (!s.islemSonuc)
                {
                    ve.Rollback();
                    return s;
                }

                s = tabHakedis.SilHakedisOdenek(hakedisID);
                if (!s.islemSonuc)
                {
                    ve.Rollback();
                    return s;
                }

                s = tabHakedis.SilHakedis(hakedisID);
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                s.DegerAta(ex.Message);
            }
            finally
            {
                ve.VeriErisimKapat();
            }
            return s;
        }

        internal static Sonuc DBHakedisDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, int hakedisID, int yeniDurumKod, string gerekce)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectVAN ov = OkuDBHakedis(ve, kullanan, hakedisID, false);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }
            Sonuc sonuc = ov.sonuc;
            DBHakedis hakedis = null;
            if (ov.o == null)
            {
                sonuc.DegerAta("Kayıt bulunamadı");
            }
            else
                hakedis = (DBHakedis)ov.o;

            if (sonuc.islemSonuc)
            {
                int[][] gecisler = new int[][] {
                    new int[] { 1, 3, 0 },
                    new int[] { 1, 9, 0 },
                    new int[] { 2, 3, 0 },
                    new int[] { 2, 9, 0 },
                    new int[] { 3, 1, 1 },
                    new int[] { 3, 4, 1 },
                    new int[] { 4, 1, 1 },
                    new int[] { 4, 5, 1 },
                    new int[] { 5, 6, 1 },
                    new int[] { 5, 9, 1 } };

                int[] bulunan = null;
                foreach (var item in gecisler)
                {
                    if (item[0] == hakedis.durumKod && item[1] == yeniDurumKod)
                    {
                        bulunan = item;
                        break;
                    }
                }
                if (bulunan == null)
                {
                    sonuc.DegerAta($"{KodAdYonetim.AdBulInt(DBListeTuru.Durum, hakedis.durumKod)} durumundan {KodAdYonetim.AdBulInt(DBListeTuru.Durum, yeniDurumKod)} durumuna geçiş yapılamaz!");
                }
                else
                {
                    if (bulunan[2] == 1)
                    {
                        if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI))
                        {
                            sonuc.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                        }
                    }
                }
            }

            if (yeniDurumKod == 1 && string.IsNullOrWhiteSpace(gerekce))
                sonuc.DegerAta("Durumu yeniye döndürürken gerekçe girilmelidir!");

            if (sonuc.islemSonuc)
            {
                if (!DBIkrazlar.YazmaYetkisiVarMi(kullanan, hakedis.altBilesenID))
                {
                    sonuc.DegerAta($"{hakedis.altBilesenNo}-{hakedis.altBilesenAd} bileşenine yazma hakkınız bulunmuyor!");
                }
            }
            TABDBHAKEDIS tabHakedis = new TABDBHAKEDIS(ve);
            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);

            if (sonuc.islemSonuc && !(yeniDurumKod == 1 || yeniDurumKod == 9))
            {
                HakSahipleriOku(tabHakedis, hakedis, sonuc);
                if (!hakedis.HakedisToplamlariDogruMu())
                    sonuc.DegerAta($"Hakediş tutarı ({hakedis.odemeToplami.ToString("#,##0.##")}) ile hak sahiplerinin alacakları ({hakedis.HakSahipleriToplami.ToString("#,##0.##")}) eşit değil!");
                if (yeniDurumKod == 3)
                {
                    EkliDosyalar dosyalar = new EkliDosyalar
                    {
                        modul = hakedis.hakedisTurDosyaEk,
                        form = hakedis.hakedisID + "_"
                    };
                    IKYMServis servis = TNS.KYM.Arac.Tanimla();
                    ObjectArray oa = servis.ListeleEkliDosyalar(kullanan, dosyalar, false);
                    if (!oa.sonuc.islemSonuc)
                    {
                        sonuc = oa.sonuc;
                    }
                    else if (oa.ObjeSayisi == 0)
                    {
                        sonuc.DegerAta("En az bir dosya eklemelisiniz!");
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                bool transactionAcik = false;
                try
                {
                    ve.BeginTransaction();
                    transactionAcik = true;
                    sonuc = tabHakedis.DurumAta(hakedis.hakedisID, yeniDurumKod);
                    if (sonuc.islemSonuc && yeniDurumKod == 5)
                    {
                        sonuc = tabHakedis.NumaraAta(hakedis.ikrazID, hakedis.hakedisID);
                        if (sonuc.islemSonuc)
                            sonuc = tabHakedis.NumaratorTarihiAta(hakedis.hakedisID);
                    }
                    if (sonuc.islemSonuc)
                        sonuc = tabTarihce.Ekle("DBH", hakedis.hakedisID.ToString(), kullanan.kullaniciKodu, KodAdYonetim.AdBulInt(DBListeTuru.Durum, yeniDurumKod), gerekce);

                    if (sonuc.islemSonuc) ve.Commit(); else ve.Rollback();
                    transactionAcik = false;
                }
                catch (Exception e)
                {
                    sonuc.DegerAta(e.ToString());
                }
                finally
                {
                    if (transactionAcik) ve.Rollback();
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static ObjectVAN OkuMerkezNumarator(VeriErisim veriErisim, Kullanici kullanan, int merkezNumaratorID)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray oa = ListeleMerkezNumaratorler(veriErisim, kullanan, new DBMerkezBankasiNumarator() { merkezNumaratorID = merkezNumaratorID }, true);
            if (oa.ObjeSayisi > 0)
            {
                ov.o = oa.objeler[0];
            }
            else
                ov.sonuc.DegerAta("Verilen numaraya ait kayıt buluanamdı");
            return ov;
        }

        internal static ObjectArray ListeleMerkezNumaratorler(VeriErisim veriErisim, Kullanici kullanan, DBMerkezBankasiNumarator dbMerkezBankasiNumarator, bool detayOku)
        {
            ObjectArray oa = new ObjectArray();
            if (dbMerkezBankasiNumarator.merkezNumaratorID == 0 && !DBIkrazlar.GormeYetkisiVarMi(kullanan, dbMerkezBankasiNumarator.altBilesenID))
            {
                oa.sonuc.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);
            IDataReader dr = tab.MerkezNumaratorListele(dbMerkezBankasiNumarator);
            string kurKodYapi = TNS.BKD.TertipYapi.KodYapiBul(DateTime.Now.Year, 1, false);

            if (dr != null)
            {
                while (dr.Read())
                {
                    int kol = 0;
                    DBMerkezBankasiNumarator detay = new DBMerkezBankasiNumarator();
                    detay.ikrazNo = VeriErisim.DegerAl(dr, kol++, "");
                    detay.ikrazParaBirimi = VeriErisim.DegerAl(dr, kol++, "");
                    detay.donusumKuru = VeriErisim.DegerAl(dr, kol++, 0.0);
                    detay.mbHesapNo = VeriErisim.DegerAl(dr, kol++, "");
                    detay.birimKurKod = OrtakFonksiyonlar.KodNoktaKoy(kurKodYapi, VeriErisim.DegerAl(dr, kol++, ""));
                    detay.birimAd = VeriErisim.DegerAl(dr, kol++, "");
                    detay.detsisNo = VeriErisim.DegerAl(dr, kol++, "");

                    detay.merkezNumaratorID = VeriErisim.DegerAl(dr, kol++, 0);
                    detay.altBilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                    detay.numarator = VeriErisim.DegerAl(dr, kol++, 0);
                    detay.islemTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));
                    detay.islemiYapan = VeriErisim.DegerAl(dr, kol++, "");
                    detay.durum = VeriErisim.DegerAl(dr, kol++, 0);

                    detay.imzaSol.imzaID = VeriErisim.DegerAl(dr, kol++, 0);
                    detay.imzaSol.adSoyad = VeriErisim.DegerAl(dr, kol++, "");
                    detay.imzaSol.satir1 = VeriErisim.DegerAl(dr, kol++, "");
                    detay.imzaSol.satir2 = VeriErisim.DegerAl(dr, kol++, "");

                    detay.imzaSag.imzaID = VeriErisim.DegerAl(dr, kol++, 0);
                    detay.imzaSag.adSoyad = VeriErisim.DegerAl(dr, kol++, "");
                    detay.imzaSag.satir1 = VeriErisim.DegerAl(dr, kol++, "");
                    detay.imzaSag.satir2 = VeriErisim.DegerAl(dr, kol++, "");

                    if (dbMerkezBankasiNumarator.merkezNumaratorID > 0 && !DBIkrazlar.GormeYetkisiVarMi(kullanan, detay.altBilesenID))
                    {
                        oa.sonuc.DegerAta($"${detay.altBilesenID} birimiyle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                        break;
                    }
                    else
                        oa.objeler.Add(detay);
                }
                dr.Close();
            }
            else
                oa.sonuc.DegerAta(ve.hata);

            if (oa.sonuc.islemSonuc && detayOku)
            {
                foreach (DBMerkezBankasiNumarator item in oa.objeler)
                {
                    HakSahipleriOkuNumarator(tab, new DBHakedisSahibi() { merkezNumaratorID = item.merkezNumaratorID }, oa.sonuc, item.hakedisSahipleri);
                }
            }
            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray MerkezeGonderilmeyenHakSahipleri(VeriErisim veriErisim, Kullanici kullanan, int altBilesenID)
        {
            ObjectArray oa = new ObjectArray();

            if (!DBIkrazlar.GormeYetkisiVarMi(kullanan, altBilesenID))
            {
                oa.sonuc.DegerAta($"{altBilesenID} alt bileşeniyle ilgili işlem yapma yetkiniz bulunmamaktadır!");
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
                TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);
                HakSahipleriOkuNumarator(tab, new DBHakedisSahibi() { merkezNumaratorID = -1, altBilesenID = altBilesenID }, oa.sonuc, oa.objeler);
            }
            return oa;
        }

        internal static Sonuc YeniMerkezNumarasiAc(VeriErisim veriErisim, Kullanici kullanan, int altBilesenID)
        {
            if (altBilesenID == 0)
            {
                return new Sonuc("Lütfen alt bileşen seçiniz!");
            }

            if (!DBIkrazlar.YazmaYetkisiVarMi(kullanan, altBilesenID))
            {
                return new Sonuc($"{altBilesenID} alt bileşeniyle ilgili işlem yapma yetkiniz bulunmamaktadır!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);
            ve.BeginTransaction();
            Sonuc s = tab.MerkezNumaratorUret(kullanan.kullaniciKodu, altBilesenID);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc MerkezNumaratorYazilabilirMi(VeriErisim ve, Kullanici kullanan, int merkezNumaratorID)
        {
            ObjectVAN ov = OkuMerkezNumarator(ve, kullanan, merkezNumaratorID);
            Sonuc s = ov.sonuc;
            if (s.islemSonuc)
            {
                if (ov.o == null) s.DegerAta("Numaratör okunamadı!");
                else
                {
                    DBMerkezBankasiNumarator dbNum = (DBMerkezBankasiNumarator)ov.o;
                    if (dbNum.durum > 3)
                        s.DegerAta("Numaratör değiştirilemez!");
                }
            }
            s.ekNesne = ov.o;
            return s;
        }

        internal static Sonuc MerkezNumaratoreEkle(VeriErisim veriErisim, Kullanici kullanan, int hakedisID, int numarator, int merkezNumaratorID)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc s = MerkezNumaratorYazilabilirMi(ve, kullanan, merkezNumaratorID);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);
            ve.BeginTransaction();
            s = tab.MerkezNumaratoreEkle(kullanan.kullaniciKodu, hakedisID, numarator, merkezNumaratorID);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return s;
        }

        internal static Sonuc MerkezNumaratordenCikart(VeriErisim veriErisim, Kullanici kullanan, int hakedisID, int numarator, int merkezNumaratorID)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc s = MerkezNumaratorYazilabilirMi(ve, kullanan, merkezNumaratorID);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);
            ve.BeginTransaction();
            s = tab.MerkezNumaratordenCikart(kullanan.kullaniciKodu, hakedisID, numarator, merkezNumaratorID);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return s;
        }


        /// <summary>
        /// DBImza listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBImza yapısında DBImza:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleDBImzalar(VeriErisim veriErisim, Kullanici kullanan, DBImza form)
        {
            ObjectArray oa = new ObjectArray();
            if (form.altBilesenID == 0 && form.imzaID == 0)
            {
                oa.sonuc.DegerAta("Lütfen alt bileşen veya imza seçiniz!");
                return oa;
            }

            if (form.imzaID == 0)
            {
                if (!DBIkrazlar.GormeYetkisiVarMi(kullanan, form.altBilesenID))
                {
                    oa.sonuc.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                    return oa;
                }
            }
            //Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    oa.sonuc.DegerAta(s.hataStr);
            //    return oa;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);

            IDataReader dr = tab.ListeleImza(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                DBImza detay = new DBImza();
                detay.imzaID = VeriErisim.DegerAl(dr, (int)TABDBHAKEDIS.ENUMAlanlarIMZA.IMZAID, 0);
                detay.altBilesenID = VeriErisim.DegerAl(dr, (int)TABDBHAKEDIS.ENUMAlanlarIMZA.ALTBILESENID, 0);
                detay.adSoyad = VeriErisim.DegerAl(dr, (int)TABDBHAKEDIS.ENUMAlanlarIMZA.ADSOYAD, "");
                detay.satir1 = VeriErisim.DegerAl(dr, (int)TABDBHAKEDIS.ENUMAlanlarIMZA.SATIR1, "");
                detay.satir2 = VeriErisim.DegerAl(dr, (int)TABDBHAKEDIS.ENUMAlanlarIMZA.SATIR2, "");
                detay.pasif = VeriErisim.DegerAl(dr, (int)TABDBHAKEDIS.ENUMAlanlarIMZA.PASIF, 0) == 1;
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            if (form.imzaID > 0 && oa.ObjeSayisi > 0)
            {
                DBImza ilkImza = (DBImza)oa.objeler[0];
                if (!DBIkrazlar.GormeYetkisiVarMi(kullanan, ilkImza.altBilesenID))
                {
                    oa.sonuc.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                    return oa;
                }

            }
            return oa;
        }

        /// <summary>
        /// DBImza nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBImza yapısında DBImza:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetDBImzalar(VeriErisim veriErisim, Kullanici kullanan, DBImza form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!DBIkrazlar.YazmaYetkisiVarMi(kullanan, form.altBilesenID))
            {
                s.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                return s;
            }

            //s = AlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    return s;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);

            ve.BeginTransaction();


            if (form.imzaID == 0)
            {
                s = tab.EkleImza(form);
            }
            else
                s = tab.GuncelleImza(form);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        public static Sonuc MerkezNumarasiDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, DBMerkezBankasiNumarator form)
        {
            Sonuc s = new Sonuc(true, "", "");

            //s = AlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    return s;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ov = OkuMerkezNumarator(ve, kullanan, form.merkezNumaratorID);
            if (ov.sonuc.islemSonuc && ov.o != null)
            {
                DBMerkezBankasiNumarator okunan = (DBMerkezBankasiNumarator)ov.o;

                if (!DBIkrazlar.YazmaYetkisiVarMi(kullanan, okunan.altBilesenID))
                {
                    s.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                }
                else if (okunan.merkezNumaratorID == form.merkezNumaratorID)
                {
                    if (okunan.durum == 1)
                    {
                        TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);
                        ve.BeginTransaction();

                        s = tab.MerkezNumarasiDurumDegistir(form);
                        if (s.islemSonuc)
                            ve.Commit();
                        else
                            ve.Rollback();
                    }
                    else
                        s.DegerAta("Yeni durumdaki merkez icmalleri değiştirilebilir");
                }
                else
                    s.DegerAta("Okunan numara ile verilen numara aynı değil!");
            }
            else
                s.DegerAta("Verien numara okunamadı!");

            ve.VeriErisimKapat();
            return s;
        }

        public static Sonuc MerkezNumaratorImzaKaydet(VeriErisim veriErisim, Kullanici kullanan, DBMerkezBankasiNumarator form)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            //s = AlanlarGecerliMi(kullanan, form);
            //if (!s.islemSonuc)
            //{
            //    return s;
            //}
            ObjectArray oa = ListeleDBImzalar(ve, kullanan, new DBImza() { imzaID = form.imzaSol.imzaID });
            if (!oa.sonuc.islemSonuc || oa.ObjeSayisi != 1)
            {
                s.DegerAta("İmza okunamadı");
            }
            else
            {
                DBImza imz = (DBImza)oa.objeler[0];
                if (imz.pasif) s.DegerAta($"İmza pasif olduğundan kullanılamaz: {imz.adSoyad}");
            }
            if (s.islemSonuc)
            {
                oa = ListeleDBImzalar(ve, kullanan, new DBImza() { imzaID = form.imzaSag.imzaID });
                if (!oa.sonuc.islemSonuc || oa.ObjeSayisi != 1)
                {
                    s.DegerAta("İmza okunamadı");
                }
                else
                {
                    DBImza imz = (DBImza)oa.objeler[0];
                    if (imz.pasif) s.DegerAta($"İmza pasif olduğundan kullanılamaz: {imz.adSoyad}");
                }
            }

            if (s.islemSonuc)
            {
                ObjectVAN ov = OkuMerkezNumarator(ve, kullanan, form.merkezNumaratorID);
                if (ov.sonuc.islemSonuc && ov.o != null)
                {
                    DBMerkezBankasiNumarator okunan = (DBMerkezBankasiNumarator)ov.o;

                    if (!DBIkrazlar.YazmaYetkisiVarMi(kullanan, okunan.altBilesenID))
                    {
                        s.DegerAta("Alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                    }
                    else if (okunan.merkezNumaratorID == form.merkezNumaratorID)
                    {
                        if (okunan.durum == 1)
                        {
                            TABDBHAKEDIS tab = new TABDBHAKEDIS(ve);
                            ve.BeginTransaction();

                            s = tab.MerkezNumarasiImzaDegistir(form);
                            if (s.islemSonuc)
                                ve.Commit();
                            else
                                ve.Rollback();
                        }
                        else
                            s.DegerAta("Yeni durumdaki merkez icmalleri değiştirilebilir");
                    }
                    else
                        s.DegerAta("Okunan numara ile verilen numara aynı değil!");
                }
                else
                    s.DegerAta("Verien numara okunamadı!");
            }
            ve.VeriErisimKapat();
            return s;
        }
    }
}

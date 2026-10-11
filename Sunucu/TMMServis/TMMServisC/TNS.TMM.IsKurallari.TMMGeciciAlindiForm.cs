using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Geçici alındı fişi bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMGeciciAlindiForm
    {
        /// <summary>
        /// Geçici alındı fişi bilgilerinin doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc Kontrol(Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();

            if (tf.yil == 0)
                sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
            if (tf.muhasebeKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
            if (tf.harcamaKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
            if (tf.ambarKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";
            if (tf.fisTarih.isNull)
                sonuc.hataStr += TasinirMalResI.TMM077 + "<br>";

            sonuc.hataStr += Genel.KontrolMuhasebeHarcamaAmbar(tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

            //Detay kontrol
            if (tf.detay.ObjeSayisi < 1)
                sonuc.hataStr += TasinirMalResI.TMM078 + "<br>";
            else
            {
                foreach (TasinirIslemDetay td in tf.detay.objeler)
                {
                    if (td.hesapPlanKod == string.Empty)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM079, td.siraNo.ToString()) + "<br>";

                    if (td.miktar <= (decimal)0)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM080, td.siraNo.ToString()) + "<br>";
                    else if (TNS.TMM.Arac.DemirbasCesidiMi(td.hesapPlanKod))
                        td.miktar = Math.Round(td.miktar);

                    if (td.kdvOran < 0)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM081, td.siraNo.ToString()) + "<br>";

                    if (td.birimFiyat < (decimal)0)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM082, td.siraNo.ToString()) + "<br>";

                    sonuc.hataStr += Genel.KontrolHesapPlan(kullanan, td.hesapPlanKod, td.siraNo, true, false);
                }
            }

            if (sonuc.hataStr != String.Empty)
                sonuc.islemSonuc = false;

            return sonuc;
        }

        /// <summary>
        /// Geçici alındı fişini varsa güncelleyen, yoksa kaydeden yordam
        /// Geçici alındı fişi ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();
            ///////////////////////////////////////////////////////////
            bool muhasebeKdvMukellefiMi = false;
            TMMMuhasebe tmmMuhasebe = new TMMMuhasebe();
            muhasebeKdvMukellefiMi = tmmMuhasebe.KdvMukellefiMi(tf.muhasebeKod);
            ///////////////////////////////////////////////////////////

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            sonuc = Kontrol(kullanan, tf);
            if (sonuc.hataStr != String.Empty)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciAlindiForm tabGeciciAlindiForm = new TABGeciciAlindiForm(ve);
            TABGeciciAlindiDetay tabGeciciAlindiDetay = new TABGeciciAlindiDetay(ve);

            ve.BeginTransaction();

            string sayacKod = "GEC" + tf.yil.ToString() + tf.muhasebeKod + tf.harcamaKod.Replace(".", "");

            if (tf.fisNo.Trim() == string.Empty)
            {
                VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
                TABGenel tabGenel = new TABGenel(ve2);

                tf.durum = (int)ENUMBelgeDurumu.YENI;
                tf.fisNo = tabGenel.BelgeNoAl(sayacKod, "000000");

                ve2.VeriErisimKapat();

                if (tf.fisNo == string.Empty)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM083;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }
            else
            {
                if (Arac.MerkezBankasiKullaniyor())
                {
                    IDataReader dr = tabGeciciAlindiForm.BelgeDurumGetir(tf);
                    if (!dr.Read())
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM270, tf.fisNo) + "<br>";
                    else
                    {
                        int durum = VeriErisim.DegerAl(dr, 0, 0);
                        if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                            sonuc.hataStr += string.Format(TasinirMalResI.TMM271, tf.fisNo) + "<br>";
                    }
                    dr.Close();

                    if (sonuc.hataStr != "")
                    {
                        sonuc.islemSonuc = false;
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }
                tf.durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;
            }

            sonuc = tabGeciciAlindiForm.Kaydet(tf);

            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (tf.durum != (int)ENUMBelgeDurumu.YENI)
            {
                //Bu fişe ait satırları sil
                sonuc = tabGeciciAlindiDetay.Sil(tf);

                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                td.fisNo = tf.fisNo;
                //İstemciden hatalı gelebilir. Burada hesaplanıyor. KDV mükellefi ise kdvlibirimfiyat=birimfiyat
                if (muhasebeKdvMukellefiMi)
                    td.birimFiyatKDVLi = td.birimFiyat;
                else
                    td.birimFiyatKDVLi = (1 + (OrtakFonksiyonlar.ConvertToDecimal(td.kdvOran) / 100)) * td.birimFiyat;

                sonuc = tabGeciciAlindiDetay.Kaydet(td);

                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = tf.fisNo;
                sonuc.bilgiStr = TasinirMalResI.TMM017;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM018 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan geçici alındı fişini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçici alındı fişi döndürülür.</returns>
        public static ObjectArray FisAc(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf)
        {
            ObjectArray bilgi = new ObjectArray();

            bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
            if (!bilgi.sonuc.islemSonuc)
            {
                bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                return bilgi;
            }

            if (tf.yil == 0)
                bilgi.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
            if (tf.muhasebeKod == string.Empty)
                bilgi.sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
            if (tf.harcamaKod == string.Empty)
                bilgi.sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
            if (tf.fisNo == string.Empty)
                bilgi.sonuc.hataStr += TasinirMalResI.TMM085 + "<br>";

            if (bilgi.sonuc.hataStr != String.Empty)
            {
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciAlindiForm tabGeciciAlindiForm = new TABGeciciAlindiForm(ve);

            tf.fisNo = tf.fisNo.PadLeft(6, '0');
            IDataReader dr = tabGeciciAlindiForm.FisAc(tf);

            while (dr.Read())
            {
                tf.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                tf.fisTarih = VeriErisim.DegerAl(dr, 4);
                tf.ambarKod = VeriErisim.DegerAl(dr, 5, string.Empty);

                tf.neredenGeldi = VeriErisim.DegerAl(dr, 6, string.Empty);
                tf.faturaNo = VeriErisim.DegerAl(dr, 10, string.Empty);
                tf.faturaTarih = VeriErisim.DegerAl(dr, 11);

                tf.tifFisNo = VeriErisim.DegerAl(dr, 12, string.Empty);

                tf.muhasebeAd = VeriErisim.DegerAl(dr, 13, string.Empty);
                tf.ilKod = VeriErisim.DegerAl(dr, 14, string.Empty);
                tf.ilAd = VeriErisim.DegerAl(dr, 15, string.Empty);
                tf.ilceKod = VeriErisim.DegerAl(dr, 16, string.Empty);
                tf.ilceAd = VeriErisim.DegerAl(dr, 17, string.Empty);
                tf.harcamaAd = VeriErisim.DegerAl(dr, 18, string.Empty);
                tf.ambarAd = VeriErisim.DegerAl(dr, 19, string.Empty);
                tf.durum = VeriErisim.DegerAl(dr, 7, 0);
                tf.islemTarih = VeriErisim.DegerAl(dr, 8);
                tf.islemYapan = VeriErisim.DegerAl(dr, 9, string.Empty);

                tf.detay = DetayListele(tf, true);

                bilgi.Ekle(tf);
            }
            dr.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM086;
            }
            else if (bilgi.ObjeSayisi > 1) //Buraya hiç bir zaman girmezsen sevinirim ;)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM087;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) && bilgi.sonuc.islemSonuc)
            {
                bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
                if (!bilgi.sonuc.islemSonuc)
                {
                    bilgi.objeler.Clear();
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                    return bilgi;
                }
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan geçici alındı fişlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçici alındı fişleri listesi döndürülür.</returns>
        public static ObjectArray Listele(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            if (tf.yil == 0)
            {
                bilgi.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
                if (!bilgi.sonuc.islemSonuc)
                {
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                    return bilgi;
                }
            }

            if (tf.muhasebeKod == string.Empty)
                bilgi.sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
            if (tf.harcamaKod == string.Empty)
                bilgi.sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
            if (tf.ambarKod == string.Empty)
                bilgi.sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";

            if (bilgi.sonuc.hataStr != string.Empty)
            {
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciAlindiForm tabGeciciAlindiForm = new TABGeciciAlindiForm(ve);

            IDataReader dr = tabGeciciAlindiForm.Listele(tf, kriter);

            string eskiTIF = "";
            string yeniTIF = "";
            while (dr.Read())
            {
                TasinirIslemForm tasForm = new TasinirIslemForm();

                tasForm.yil = tf.yil;
                tasForm.muhasebeKod = tf.muhasebeKod;
                tasForm.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                tasForm.harcamaKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                tasForm.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), tasForm.harcamaKod);
                tasForm.fisNo = VeriErisim.DegerAl(dr, 3, string.Empty);

                yeniTIF = tasForm.yil + "-" + tasForm.muhasebeKod + "-" + tasForm.harcamaKod + "-" + tasForm.fisNo;

                if (yeniTIF != eskiTIF)
                {
                    tasForm.fisTarih = VeriErisim.DegerAl(dr, 4);
                    tasForm.ambarKod = VeriErisim.DegerAl(dr, 5, "");
                    tasForm.durum = VeriErisim.DegerAl(dr, 7, 0);
                    tasForm.islemTarih = VeriErisim.DegerAl(dr, 8);
                    tasForm.islemYapan = VeriErisim.DegerAl(dr, 9, "");
                    tasForm.tifFisNo = VeriErisim.DegerAl(dr, 12, "");
                    tasForm.muhasebeAd = VeriErisim.DegerAl(dr, 13, "");
                    tasForm.harcamaAd = VeriErisim.DegerAl(dr, 14, "");
                    tasForm.ambarAd = VeriErisim.DegerAl(dr, 15, "");

                    bilgi.Ekle(tasForm);
                }
                eskiTIF = yeniTIF;
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan geçici alındı fişinin detaylarını döndüren yordam
        /// </summary>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <param name="siraNoSirali">Geçici alındı fişinin detay bilgilerinin satır sırasına göre sıralı olup olmaması bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçici alındı fişinin detayları döndürülür.</returns>
        private static ObjectArray DetayListele(TasinirIslemForm tf, bool siraNoSirali)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABGeciciAlindiDetay tabGeciciAlindiDetay = new TABGeciciAlindiDetay(ve);

            ObjectArray bilgi = new ObjectArray();
            tf.fisNo = tf.fisNo.PadLeft(6, '0');
            IDataReader dr = tabGeciciAlindiDetay.Listele(tf, siraNoSirali);

            while (dr.Read())
            {
                TasinirIslemDetay tDetay = new TasinirIslemDetay();

                tDetay.yil = VeriErisim.DegerAl(dr, 0, 0);
                tDetay.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                tDetay.harcamaKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                tDetay.fisNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                tDetay.siraNo = VeriErisim.DegerAl(dr, 4, 0);
                tDetay.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), VeriErisim.DegerAl(dr, 5, string.Empty));
                tDetay.miktar = VeriErisim.DegerAl(dr, 6, (decimal)0);
                tDetay.kdvOran = VeriErisim.DegerAl(dr, 7, 0);
                tDetay.birimFiyat = VeriErisim.DegerAl(dr, 8, (decimal)0);
                tDetay.birimFiyatKDVLi = VeriErisim.DegerAl(dr, 9, (decimal)0);
                tDetay.hesapPlanAd = TMMServis.UzayYaziDegerGetir("TASHESAPPLAN", tDetay.hesapPlanKod.Replace(".", ""), true, "");
                tDetay.olcuBirimAd = TMMServis.UzayYaziDegerGetir("TASOLCUBIRIMAD", tDetay.hesapPlanKod.Replace(".", ""), true, "");

                bilgi.Ekle(tDetay);
            }
            dr.Close();

            if (bilgi.ObjeSayisi < 1)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM088;
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static Sonuc SicilOlustur(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf, string islem)
        {
            Sonuc sonuc = new Sonuc();

            ObjectArray listeFis = FisAc(null, kullanan, tf);
            if (listeFis.ObjeSayisi < 1)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM276;
                return sonuc;
            }

            TasinirIslemForm tfIslem = (TasinirIslemForm)listeFis[0];

            sonuc = Kontrol(kullanan, tfIslem);
            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciAlindiForm tabGeciciAlindiForm = new TABGeciciAlindiForm(ve);


            IDataReader dr = tabGeciciAlindiForm.BelgeDurumGetir(tf);
            if (!dr.Read())
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM086;
                dr.Close();
                ve.VeriErisimKapat();
                return sonuc;
            }
            int durum = VeriErisim.DegerAl(dr, 0, 0);
            dr.Close();

            if (durum == (int)ENUMBelgeDurumu.ONAYLI && islem == "İptal")
                sonuc.hataStr = string.Format(TasinirMalResI.TMM273, tf.fisNo) + "<br>";
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI && islem == "Onay")
                sonuc.hataStr = string.Format(TasinirMalResI.TMM274, tf.fisNo) + "<br>";
            else if (durum != (int)ENUMBelgeDurumu.ONAYLI && islem == "OnayKaldir")
                sonuc.hataStr = string.Format("{0} no'lu belge zaten onaylı değil.", tf.fisNo) + "<br>";

            if (sonuc.hataStr != "")
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                return sonuc;
            }


            bool muhasebeKdvMukellefiMi = false;
            TMMMuhasebe tmmMuhasebe = new TMMMuhasebe();
            muhasebeKdvMukellefiMi = tmmMuhasebe.KdvMukellefiMi(tf.muhasebeKod);


            ObjectArray sicilNolar = new ObjectArray();

            //************************
            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                //Dayanıklı bölümü
                if (TNS.TMM.Arac.DemirbasCesidiMi(td.hesapPlanKod))
                {
                    SicilNoHareket sicilNo = new SicilNoHareket();

                    sicilNo.yil = tf.yil;
                    sicilNo.muhasebeKod = tf.muhasebeKod;
                    sicilNo.harcamaBirimKod = tf.harcamaKod;
                    sicilNo.hesapPlanKod = td.hesapPlanKod.Replace(".", "");
                    sicilNo.fisNo = td.fisNo;
                    sicilNo.fisSiraNo = td.siraNo;
                    sicilNo.sicilNo = td.gorSicilNo;
                    sicilNo.prSicilNo = td.sicilNo;
                    sicilNo.ambarKod = tf.ambarKod;
                    sicilNo.fiyat = td.birimFiyat;
                    sicilNo.kdvOran = td.kdvOran;
                    sicilNo.islemTipKod = tf.islemTipKod;
                    sicilNo.islemTurKod = (int)ENUMIslemTipi.GECICIALINDI;
                    sicilNo.islemYapan = kullanan.kullaniciKodu;
                    sicilNo.islemTarih = new TNSDateTime(DateTime.Now);
                    sicilNo.eSicilNo = td.eSicilNo;
                    sicilNo.eMuhasebeKod = tf.gMuhasebeKod;
                    sicilNo.eHarcamaBirimKod = tf.gHarcamaKod;
                    sicilNo.fisTarih = new TNSDateTime(tf.fisTarih.ToString());
                    sicilNo.kimeGitti = tf.kimeGitti;
                    sicilNo.nereyeGitti = tf.nereyeGitti;
                    sicilNo.neredenGeldi = tf.neredenGeldi;

                    if (muhasebeKdvMukellefiMi)
                        sicilNo.kdvliBirimFiyat = sicilNo.fiyat;
                    else
                        sicilNo.kdvliBirimFiyat = (sicilNo.fiyat * (1 + (OrtakFonksiyonlar.ConvertToDecimal(sicilNo.kdvOran.ToString(), 0) / 100))); ;

                    sicilNo.gonMuhasebeKod = tf.gMuhasebeKod;
                    sicilNo.gonHarcamaBirimKod = tf.gHarcamaKod;
                    sicilNo.gonAmbarKod = tf.gAmbarKod;
                    //sicilNo.degerArtisi = degerArttir;
                    sicilNo.miktar = OrtakFonksiyonlar.ConvertToInt(td.miktar, 0);
                    sicilNo.ozellik = td.ozellik;

                    if (sicilNo.ozellik.disSicilNo == "") sicilNo.ozellik.disSicilNo = td.eSicilNo;
                    if (sicilNo.ozellik.neredenGeldi == "") sicilNo.ozellik.neredenGeldi = td.eTedarikSekli;
                    if (sicilNo.ozellik.yeriKonusu == "") sicilNo.ozellik.yeriKonusu = td.eAciklama;

                    sicilNolar.objeler.Add(sicilNo);
                }

            }


            ve.BeginTransaction();

            sonuc = tabGeciciAlindiForm.BelgeDurumDegistir(tf, islem);

            if (sonuc.islemSonuc)
            {
                if (islem == "Onay")
                    sonuc = TMMSicilNoHareket.SicilNoVer(ve, kullanan, sicilNolar);
                else if (islem == "OnayKaldir")
                    sonuc = TMMSicilNoHareket.SicilNoIptalEt(ve, kullanan, sicilNolar);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();


            if (sonuc.islemSonuc && sonuc.bilgiStr == "")
                sonuc.bilgiStr = "İşlem başarıyla gerçekleştirildi.";
            else if (!sonuc.islemSonuc && sonuc.hataStr == "")
                sonuc.hataStr = "İşlem gerçekleştirilemedi." + "<br>" + sonuc.hataStr;

            return sonuc;

        }

        public static Sonuc TifFisNoKaydet(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            sonuc = Kontrol(kullanan, tf);
            if (sonuc.hataStr != String.Empty)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciAlindiForm tabGeciciAlindiForm = new TABGeciciAlindiForm(ve);

            ve.BeginTransaction();

            sonuc = tabGeciciAlindiForm.TifFisNoKaydet(tf);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM017;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM018 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
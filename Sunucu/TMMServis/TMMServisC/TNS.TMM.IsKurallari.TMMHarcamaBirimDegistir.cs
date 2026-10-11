using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Harcama birimi değişikliğiyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMHarcamaBirimDegistir
    {
        /// <summary>
        /// Harcama birimi değişikliğini yapan yordam
        /// Bu yordam, verilen veri tabanı tablosunun kayıtlarını gezer ve harcama birimi geçen verileri günceller.
        /// Eğer, verilen veri tabanı tablosuna bağlı alt tablo varsa ana tabloya ilgili kayıtları ekler ve alt tabloda
        /// varolan ilgili kayıtları günceller. Eski harcama birimine ait bilgiler gerekiyorsa ana yordam içinde silinir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <param name="harcamaBirimiYeni">Yeni harcama birimi bilgisini tutan nesne</param>
        /// <param name="tablo">Değişiklik yapılacak tabloya ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc DegistirIc(VeriErisim veriErisim, HarcamaBirimi harcamaBirimiEski, HarcamaBirimi harcamaBirimiYeni, TabloSinif tablo)
        {
            Sonuc sonuc = new Sonuc();

            tablo.alanAdlari = Genel.AlanAdlariDondur(tablo.tabloAdAna);
            if (tablo.alanAdlari == null)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(TasinirMalResI.TMM102, tablo.tabloAdAna);
                return sonuc;
            }

            VeriErisim veIc = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            veIc.BeginTransaction();
            TABGenel t = new TABGenel(veIc);

            string sql = string.Empty;
            if (!string.IsNullOrEmpty(tablo.tabloAdCocuk))
            {
                sql = InsertOlustur(tablo, harcamaBirimiEski, harcamaBirimiYeni);
                sonuc = t.SQLCalistirKomut(sql);
                if (!sonuc.islemSonuc)
                {
                    veIc.Rollback();
                    veIc.VeriErisimKapat();
                    sonuc.hataStr = TasinirMalResI.TMM103 + "<br>" + sonuc.hataStr;
                    return sonuc;
                }
            }

            sql = UpdateOlustur(tablo, harcamaBirimiEski, harcamaBirimiYeni);
            sonuc = t.SQLCalistirKomut(sql);
            if (!sonuc.islemSonuc)
            {
                veIc.Rollback();
                veIc.VeriErisimKapat();
                sonuc.hataStr = TasinirMalResI.TMM104 + "<br>" + sonuc.hataStr;
                return sonuc;
            }

            veIc.Commit();
            veIc.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Eski harcama birimine ait bilgileri silen yordam
        /// Bu yordam, verilen veri tabanı tablosunun kayıtlarını gezer ve eski harcama birimi geçen verileri siler.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <param name="tablo">Verileri silinecek tabloya ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc SilIc(VeriErisim veriErisim, HarcamaBirimi harcamaBirimiEski, TabloSinif tablo)
        {
            Sonuc sonuc = new Sonuc();

            tablo.alanAdlari = Genel.AlanAdlariDondur(tablo.tabloAdAna);
            if (tablo.alanAdlari == null)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(TasinirMalResI.TMM102, tablo.tabloAdAna);
                return sonuc;
            }

            VeriErisim veIc = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            veIc.BeginTransaction();
            TABGenel t = new TABGenel(veIc);

            string sql = DeleteOlustur(tablo, harcamaBirimiEski);
            sonuc = t.SQLCalistirKomut(sql);
            if (!sonuc.islemSonuc)
            {
                veIc.Rollback();
                veIc.VeriErisimKapat();
                sonuc.hataStr = TasinirMalResI.TMM105 + "<br>" + sonuc.hataStr;
                return sonuc;
            }

            veIc.Commit();
            veIc.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Harcama birimi değişikliğini yapan yordam
        /// Bu yordam, bütün veri tabanındaki kayıtları gezer ve harcama birimi geçen verileri günceller.
        /// Gönderilen bilgiler eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <param name="harcamaBirimiYeni">Yeni harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Degistir(VeriErisim veriErisim, Kullanici kullanan, HarcamaBirimi harcamaBirimiEski, HarcamaBirimi harcamaBirimiYeni)
        {
            Sonuc sonuc = new Sonuc();
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            if (string.IsNullOrEmpty(harcamaBirimiEski.muhasebeKod) ||
                string.IsNullOrEmpty(harcamaBirimiEski.kod) ||
                string.IsNullOrEmpty(harcamaBirimiYeni.muhasebeKod) ||
                string.IsNullOrEmpty(harcamaBirimiYeni.kod) ||
                (harcamaBirimiEski.muhasebeKod == harcamaBirimiYeni.muhasebeKod &&
                harcamaBirimiEski.kod == harcamaBirimiYeni.kod))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM106;
                return sonuc;
            }

            harcamaBirimiEski.kod = harcamaBirimiEski.kod.Replace(".", "");
            harcamaBirimiYeni.kod = harcamaBirimiYeni.kod.Replace(".", "");

            sonuc = OrtakFonksiyonlar.KodYapiKontrol(kodYapisi, harcamaBirimiEski.kod, true);
            if (!sonuc.islemSonuc)
                return sonuc;

            sonuc = OrtakFonksiyonlar.KodYapiKontrol(kodYapisi, harcamaBirimiYeni.kod, true);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHarcamaBirimi tabHarcamaBirimi = new TABHarcamaBirimi(ve);

            IDataReader dr = tabHarcamaBirimi.Listele(harcamaBirimiEski);
            if (!dr.Read())
            {
                dr.Close();
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM107;
                return sonuc;
            }
            dr.Close();

            dr = tabHarcamaBirimi.Listele(harcamaBirimiYeni);
            if (!dr.Read())
            {
                dr.Close();
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM108;
                return sonuc;
            }
            dr.Close();

            ve.BeginTransaction();

            sonuc = AmbarKopyala(ve, harcamaBirimiEski, harcamaBirimiYeni);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            ObjectArray tablolar = new ObjectArray();

            TabloSinif tasIslem = new TabloSinif();
            tasIslem.tabloAdAna = "TASISLEMFORM";
            tasIslem.tabloAdCocuk = "TASISLEMDETAY";
            tasIslem.muhasebeKolonAdi = "MUHASEBEKOD";
            tasIslem.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasIslem.yilKolonuVarMi = true;
            tablolar.Ekle(tasIslem);

            TabloSinif tasIslemTarihce = new TabloSinif();
            tasIslemTarihce.tabloAdAna = "TASISLEMFORMTARIHCE";
            tasIslemTarihce.muhasebeKolonAdi = "MUHASEBEKOD";
            tasIslemTarihce.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasIslemTarihce.yilKolonuVarMi = true;
            tablolar.Ekle(tasIslemTarihce);

            TabloSinif tasIslemMif = new TabloSinif();
            tasIslemMif.tabloAdAna = "TASISLEMMIF";
            tasIslemMif.muhasebeKolonAdi = "MUHASEBEKOD";
            tasIslemMif.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasIslemMif.yilKolonuVarMi = true;
            tablolar.Ekle(tasIslemMif);

            TabloSinif tasIslemOzellik = new TabloSinif();
            tasIslemOzellik.tabloAdAna = "TASISLEMOZELLIK";
            tasIslemOzellik.muhasebeKolonAdi = "MUHASEBEKOD";
            tasIslemOzellik.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasIslemOzellik.yilKolonuVarMi = true;
            tablolar.Ekle(tasIslemOzellik);

            TabloSinif tasZimmet = new TabloSinif();
            tasZimmet.tabloAdAna = "TASZIMMETFORM";
            tasZimmet.tabloAdCocuk = "TASZIMMETDETAY";
            tasZimmet.muhasebeKolonAdi = "MUHASEBEKOD";
            tasZimmet.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasZimmet.yilKolonuVarMi = true;
            tablolar.Ekle(tasZimmet);

            TabloSinif tasZimmetTarihce = new TabloSinif();
            tasZimmetTarihce.tabloAdAna = "TASZIMMETFORMTARIHCE";
            tasZimmetTarihce.muhasebeKolonAdi = "MUHASEBEKOD";
            tasZimmetTarihce.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasZimmetTarihce.yilKolonuVarMi = true;
            tablolar.Ekle(tasZimmetTarihce);

            TabloSinif tasGecici = new TabloSinif();
            tasGecici.tabloAdAna = "TASGECICIFORM";
            tasGecici.tabloAdCocuk = "TASGECICIDETAY";
            tasGecici.muhasebeKolonAdi = "MUHASEBEKOD";
            tasGecici.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasGecici.yilKolonuVarMi = true;
            tablolar.Ekle(tasGecici);

            TabloSinif tasIstek = new TabloSinif();
            tasIstek.tabloAdAna = "TASISTEKFORM";
            tasIstek.tabloAdCocuk = "TASISTEKDETAY";
            tasIstek.muhasebeKolonAdi = "MUHASEBEKOD";
            tasIstek.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasIstek.yilKolonuVarMi = true;
            tablolar.Ekle(tasIstek);

            TabloSinif tasSayim = new TabloSinif();
            tasSayim.tabloAdAna = "TASSAYIMFORM";
            tasSayim.tabloAdCocuk = "TASSAYIMDETAY";
            tasSayim.muhasebeKolonAdi = "MUHASEBEKOD";
            tasSayim.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasSayim.yilKolonuVarMi = true;
            tablolar.Ekle(tasSayim);

            TabloSinif tasKayittanDusme = new TabloSinif();
            tasKayittanDusme.tabloAdAna = "TASKAYITTANDUSMEFORM";
            tasKayittanDusme.tabloAdCocuk = "TASKAYITTANDUSMEDETAY";
            tasKayittanDusme.muhasebeKolonAdi = "MUHASEBEKOD";
            tasKayittanDusme.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasKayittanDusme.yilKolonuVarMi = true;
            tablolar.Ekle(tasKayittanDusme);

            TabloSinif tasOda = new TabloSinif();
            tasOda.tabloAdAna = "TASODA";
            tasOda.muhasebeKolonAdi = "MUHASEBEKOD";
            tasOda.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasOda.yilKolonuVarMi = false;
            tablolar.Ekle(tasOda);

            string personelTabloAd = "TASPERSONEL";
            if (!Genel.ViewMi(personelTabloAd))
            {
                TabloSinif tasPersonel = new TabloSinif();
                tasPersonel.tabloAdAna = personelTabloAd;
                tasPersonel.muhasebeKolonAdi = "MUHASEBEKOD";
                tasPersonel.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
                //tasPersonel.yilKolonuVarMi = false;
                tablolar.Ekle(tasPersonel);
            }

            TabloSinif tasImza = new TabloSinif();
            tasImza.tabloAdAna = "TASIMZA";
            tasImza.muhasebeKolonAdi = "MUHASEBEKOD";
            tasImza.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasImza.yilKolonuVarMi = false;
            tablolar.Ekle(tasImza);

            TabloSinif tasSicilNoHareket = new TabloSinif();
            tasSicilNoHareket.tabloAdAna = "TASSICILNOHAREKET";
            tasSicilNoHareket.muhasebeKolonAdi = "MUHASEBEKOD";
            tasSicilNoHareket.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasSicilNoHareket.yilKolonuVarMi = true;
            tablolar.Ekle(tasSicilNoHareket);

            //Devir çıkış ve devir giriş işlemleri için
            TabloSinif tasSicilNoHareketGon = new TabloSinif();
            tasSicilNoHareketGon.tabloAdAna = "TASSICILNOHAREKET";
            tasSicilNoHareketGon.muhasebeKolonAdi = "GONMUHASEBEKOD";
            tasSicilNoHareketGon.harcamaBirimKolonAdi = "GONHARCAMABIRIMKOD";
            //tasSicilNoHareketGon.yilKolonuVarMi = true;
            tablolar.Ekle(tasSicilNoHareketGon);

            //Devir çıkış ve devir giriş işlemleri için
            TabloSinif tasIslemGon = new TabloSinif();
            tasIslemGon.tabloAdAna = "TASISLEMFORM";
            tasIslemGon.muhasebeKolonAdi = "GMUHASEBEKOD";
            tasIslemGon.harcamaBirimKolonAdi = "GHARCAMAKOD";
            //tasIslemGon.yilKolonuVarMi = true;
            tablolar.Ekle(tasIslemGon);

            TabloSinif tasTalep = new TabloSinif();
            tasTalep.tabloAdAna = "TASTALEPKARSILAMA";
            tasTalep.tabloAdCocuk = "TASTALEPKARSILAMADETAY";
            tasTalep.muhasebeKolonAdi = "MUHASEBEKOD";
            tasTalep.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            //tasTalep.yilKolonuVarMi = true;
            tablolar.Ekle(tasTalep);

            foreach (TabloSinif ts in tablolar.objeler)
            {
                sonuc = DegistirIc(ve, harcamaBirimiEski, harcamaBirimiYeni, ts);
                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            foreach (TabloSinif ts in tablolar.objeler)
            {
                if (!string.IsNullOrEmpty(ts.tabloAdCocuk))
                {
                    sonuc = SilIc(ve, harcamaBirimiEski, ts);
                    if (!sonuc.islemSonuc)
                    {
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }
            }

            sonuc = SayacUpdateEt(ve, harcamaBirimiEski, harcamaBirimiYeni);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            sonuc = AmbarSil(ve, harcamaBirimiEski);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            sonuc = HarcamaBirimiSil(ve, harcamaBirimiEski);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.Commit();
            ve.VeriErisimKapat();
            sonuc.bilgiStr = TasinirMalResI.TMM109;
            return sonuc;
        }

        /// <summary>
        /// Verilen veri tabanı tablosundaki eski harcama birimine ait kayıtları yeni
        /// harcama birimi için yeni kayıt olarak ekleyen SQL cümlesini oluşturan yordam
        /// </summary>
        /// <param name="tablo">Veri eklenecek tabloya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <param name="harcamaBirimiYeni">Yeni harcama birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan SQL cümlesi döndürülür.</returns>
        private static string InsertOlustur(TabloSinif tablo, HarcamaBirimi harcamaBirimiEski, HarcamaBirimi harcamaBirimiYeni)
        {
            string sql = "INSERT INTO " + tablo.tabloAdAna + " (";
            for (int i = 0; i < tablo.alanAdlari.Length; i++)
            {
                sql += tablo.alanAdlari[i];
                if (i != tablo.alanAdlari.Length - 1)
                    sql += ",";
            }
            sql += ") ";
            sql += "SELECT ";
            for (int i = 0; i < tablo.alanAdlari.Length; i++)
            {
                if (tablo.alanAdlari[i] == tablo.muhasebeKolonAdi)
                    sql += "'" + harcamaBirimiYeni.muhasebeKod + "'";
                else if (tablo.alanAdlari[i] == tablo.harcamaBirimKolonAdi)
                    sql += "'" + harcamaBirimiYeni.kod + "'";
                else
                    sql += tablo.alanAdlari[i];

                if (i != tablo.alanAdlari.Length - 1)
                    sql += ",";
            }
            sql += " FROM " + tablo.tabloAdAna + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(tablo.tabloAdAna, tablo.muhasebeKolonAdi, "=", harcamaBirimiEski.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tablo.tabloAdAna, tablo.harcamaBirimKolonAdi, "=", harcamaBirimiEski.kod));
            //if (tablo.yilKolonuVarMi)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tablo.tabloAdAna, "YIL", "=", yil));

            return sql;
        }

        /// <summary>
        /// Verilen veri tabanı tablosundaki eski harcama birimi kayıtlarını yeni
        /// harcama birimi bilgisiyle güncelleyen SQL cümlesini oluşturan yordam
        /// </summary>
        /// <param name="tablo">Güncelleme yapılacak tabloya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <param name="harcamaBirimiYeni">Yeni harcama birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan SQL cümlesi döndürülür.</returns>
        private static string UpdateOlustur(TabloSinif tablo, HarcamaBirimi harcamaBirimiEski, HarcamaBirimi harcamaBirimiYeni)
        {
            string tabloAd = tablo.tabloAdCocuk != string.Empty ? tablo.tabloAdCocuk : tablo.tabloAdAna;

            string sql = "UPDATE " + tabloAd + " SET ";
            sql += VeriErisim.AtamaOl("", tablo.muhasebeKolonAdi, harcamaBirimiYeni.muhasebeKod);
            sql += ", " + VeriErisim.AtamaOl("", tablo.harcamaBirimKolonAdi, harcamaBirimiYeni.kod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(tabloAd, tablo.muhasebeKolonAdi, "=", harcamaBirimiEski.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAd, tablo.harcamaBirimKolonAdi, "=", harcamaBirimiEski.kod));
            //if (tablo.yilKolonuVarMi)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAd, "YIL", "=", yil));

            return sql;
        }

        /// <summary>
        /// Verilen veri tabanı tablosundaki eski harcama birimine ait kayıtları silen SQL cümlesini oluşturan yordam
        /// </summary>
        /// <param name="tablo">Veri silinecek tabloya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan SQL cümlesi döndürülür.</returns>
        private static string DeleteOlustur(TabloSinif tablo, HarcamaBirimi harcamaBirimiEski)
        {
            string sql = "DELETE FROM " + tablo.tabloAdAna;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(tablo.tabloAdAna, tablo.muhasebeKolonAdi, "=", harcamaBirimiEski.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tablo.tabloAdAna, tablo.harcamaBirimKolonAdi, "=", harcamaBirimiEski.kod));
            //if (tablo.yilKolonuVarMi)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tablo.tabloAdAna, "YIL", "=", yil));

            return sql;
        }

        /// <summary>
        /// Sayaç tablosundaki eski harcama birimine ait kayıtları bulup yeni harcama birimiyle değiştiren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <param name="harcamaBirimiYeni">Yeni harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc SayacUpdateEt(VeriErisim veriErisim, HarcamaBirimi harcamaBirimiEski, HarcamaBirimi harcamaBirimiYeni)
        {
            Sonuc sonuc = new Sonuc();

            ObjectArray eskiKodlar = new ObjectArray();

            VeriErisim veIc = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            veIc.BeginTransaction();
            TABGenel t = new TABGenel(veIc);

            string sql = "SELECT * FROM SAYAC ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("SAYAC", "KOD", "LIKE", "%" + harcamaBirimiEski.muhasebeKod + harcamaBirimiEski.kod + "%"));

            IDataReader dr = t.SQLCalistir(sql);
            while (dr.Read())
            {
                string eskiKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                eskiKodlar.Ekle(eskiKod);
            }
            dr.Close();

            foreach (string ek in eskiKodlar.objeler)
            {
                string sqlUpdate = "UPDATE SAYAC SET KOD='" + ek.Replace(harcamaBirimiEski.muhasebeKod + harcamaBirimiEski.kod, harcamaBirimiYeni.muhasebeKod + harcamaBirimiYeni.kod) + "' WHERE KOD='" + ek + "'";
                sonuc = t.SQLCalistirKomut(sqlUpdate);
                if (!sonuc.islemSonuc)
                {
                    veIc.Rollback();
                    veIc.VeriErisimKapat();
                    sonuc.hataStr = TasinirMalResI.TMM110 + "<br>" + sonuc.hataStr;
                    return sonuc;
                }
            }

            veIc.Commit();
            veIc.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Eski harcama birimine ait ambar kayıtlarını bulup yeni harcama birimine ekleyen yordam
        /// Eski harcama birimine ait ambarlar ana yordam içinde silinir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <param name="harcamaBirimiYeni">Yeni harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc AmbarKopyala(VeriErisim veriErisim, HarcamaBirimi harcamaBirimiEski, HarcamaBirimi harcamaBirimiYeni)
        {
            Sonuc sonuc = new Sonuc();

            TabloSinif tablo = new TabloSinif();
            tablo.tabloAdAna = "TASAMBAR";
            tablo.muhasebeKolonAdi = "MUHASEBEKOD";
            tablo.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            tablo.alanAdlari = Genel.AlanAdlariDondur(tablo.tabloAdAna);
            if (tablo.alanAdlari == null)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(TasinirMalResI.TMM102, tablo.tabloAdAna);
                return sonuc;
            }

            VeriErisim veIc = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABGenel t = new TABGenel(veIc);

            string sql = "SELECT * FROM " + tablo.tabloAdAna;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(tablo.tabloAdAna, tablo.muhasebeKolonAdi, "=", harcamaBirimiYeni.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tablo.tabloAdAna, tablo.harcamaBirimKolonAdi, "=", harcamaBirimiYeni.kod));

            IDataReader dr = t.SQLCalistir(sql);
            if (dr.Read())
            {
                dr.Close();
                veIc.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM111;
                return sonuc;
            }
            dr.Close();

            veIc.BeginTransaction();

            sql = InsertOlustur(tablo, harcamaBirimiEski, harcamaBirimiYeni);
            sonuc = t.SQLCalistirKomut(sql);
            if (sonuc.islemSonuc)
                veIc.Commit();
            else
            {
                veIc.Rollback();
                sonuc.hataStr = TasinirMalResI.TMM112 + "<br>" + sonuc.hataStr;
            }

            veIc.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Eski harcama birimine ait ambarları bulup silen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc AmbarSil(VeriErisim veriErisim, HarcamaBirimi harcamaBirimiEski)
        {
            Sonuc sonuc = new Sonuc();

            TabloSinif tablo = new TabloSinif();
            tablo.tabloAdAna = "TASAMBAR";
            tablo.muhasebeKolonAdi = "MUHASEBEKOD";
            tablo.harcamaBirimKolonAdi = "HARCAMABIRIMKOD";
            tablo.alanAdlari = Genel.AlanAdlariDondur(tablo.tabloAdAna);
            if (tablo.alanAdlari == null)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(TasinirMalResI.TMM102, tablo.tabloAdAna);
                return sonuc;
            }

            VeriErisim veIc = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            veIc.BeginTransaction();
            TABGenel t = new TABGenel(veIc);

            string sql = DeleteOlustur(tablo, harcamaBirimiEski);
            sonuc = t.SQLCalistirKomut(sql);
            if (sonuc.islemSonuc)
                veIc.Commit();
            else
            {
                veIc.Rollback();
                sonuc.hataStr = TasinirMalResI.TMM113 + "<br>" + sonuc.hataStr;
            }

            veIc.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Eski harcama birimini bulup silen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc HarcamaBirimiSil(VeriErisim veriErisim, HarcamaBirimi harcamaBirimiEski)
        {
            Sonuc sonuc = new Sonuc();

            TabloSinif tablo = new TabloSinif();
            tablo.tabloAdAna = "TASHARCAMABIRIM";
            tablo.muhasebeKolonAdi = "MUHASEBEKOD";
            tablo.harcamaBirimKolonAdi = "KOD";
            tablo.alanAdlari = Genel.AlanAdlariDondur(tablo.tabloAdAna);
            if (tablo.alanAdlari == null)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(TasinirMalResI.TMM102, tablo.tabloAdAna);
                return sonuc;
            }

            VeriErisim veIc = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            veIc.BeginTransaction();
            TABGenel t = new TABGenel(veIc);

            string sql = DeleteOlustur(tablo, harcamaBirimiEski);
            sonuc = t.SQLCalistirKomut(sql);
            if (sonuc.islemSonuc)
                veIc.Commit();
            else
            {
                veIc.Rollback();
                sonuc.hataStr = TasinirMalResI.TMM114 + "<br>" + sonuc.hataStr;
            }

            veIc.VeriErisimKapat();
            return sonuc;
        }
    }
}
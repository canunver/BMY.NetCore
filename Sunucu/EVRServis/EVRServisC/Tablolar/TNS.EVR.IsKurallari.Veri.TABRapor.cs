using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Evrak rapor işlemleri ile ilgili, listeleme yordamını tutan sınıf.    
    /// </summary>
    public class TABRapor : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABRapor sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABRapor(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Paramete olarak verilen evrak bilgisi ve evrak rapor türüne göre; evrak rapor bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">Evrak bilgisi içeren nesne</param>
        /// <param name="raporTur">Evrak rapor tür bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(EvrakSinif form, ENUMEvrakRaporTuru raporTur)
        {
            string sql = string.Empty;
            string kosul = "";
            string birimKod = "";
            string degisken = ""; //KONU - GELDIGIYER - EVRAKTIPI - GELISSEKLI

            if (raporTur == ENUMEvrakRaporTuru.BIRIM) { degisken = "EVRAKTIPI"; }
            else if (raporTur == ENUMEvrakRaporTuru.KONU) { degisken = "KONU"; }
            else if (raporTur == ENUMEvrakRaporTuru.GELDIGIYER) { degisken = "GELDIGIYER"; }
            else if (raporTur == ENUMEvrakRaporTuru.GELISSEKLI) { degisken = "GELISSEKLI"; }
            else if (raporTur == ENUMEvrakRaporTuru.PERSONEL)
                return ListelePersonel(form);
            else if (raporTur == ENUMEvrakRaporTuru.PERSONELTOPLAM)
                return ListelePersonelToplam(form);

            if (form.aciklama == "ISLEM51")
            {
                kosul = "(EVREVRAKHAREKETSONDURUM.ISLEM = 51) ";
                birimKod = "BIRIMKOD2";
            }
            else if (form.aciklama == "ISLEMNOT99NOT51")
            {
                kosul = "(NOT (EVREVRAKHAREKETSONDURUM.ISLEM = 99)) AND (NOT (EVREVRAKHAREKETSONDURUM.ISLEM = 51))";
                birimKod = "BIRIMKOD1";
            }
            else if (form.aciklama == "ISLEMNOT99")
            {
                kosul = "(NOT (EVREVRAKHAREKETSONDURUM.ISLEM = 99))";
                birimKod = "BIRIMKOD1";
            }

            string birimKodAlan = "''"; //"EVREVRAK." + degisken;
            string birimKodEkAlan = "";
            if (raporTur == ENUMEvrakRaporTuru.BIRIM)
            {
                birimKodAlan = "EVREVRAKHAREKETSONDURUM." + birimKod;
                birimKodEkAlan = " + ' - ' + BIRIM.AD AS BIRIM";
            }

            sql = "SELECT " + birimKodAlan + birimKodEkAlan + ", EVREVRAK." + degisken + ", COUNT(*) AS COUNT ";
            sql += "FROM EVREVRAK AS EVREVRAK INNER JOIN ";
            sql += "EVREVRAKSICIL AS EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO INNER JOIN ";
            sql += "EVREVRAKHAREKETSONDURUM AS EVREVRAKHAREKETSONDURUM ON ";
            sql += "EVREVRAKSICIL.PREVRAKNO = EVREVRAKHAREKETSONDURUM.PREVRAKNO ";

            if (raporTur == ENUMEvrakRaporTuru.BIRIM)
                sql += "INNER JOIN EVRBIRIM AS BIRIM ON " + birimKodAlan + " = BIRIM.KOD ";

            if (form.tarihce != null)
            {
                sql += "INNER JOIN EVREVRAKTARIHCE ON EVREVRAK.EVREVRAKNO = EVREVRAKTARIHCE.EVREVRAKNO ";

                if (!string.IsNullOrEmpty(form.tarihce.islemYapan))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKTARIHCE", "ISLEMYAPAN", "=", form.tarihce.islemYapan));
                if (!string.IsNullOrEmpty(form.tarihce.islem))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKTARIHCE", "ISLEM", "=", form.tarihce.islem));
            }

            if (form.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "YIL", "=", form.yil));
            if (form.evrakTipi > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTIPI", "=", form.evrakTipi));
            if (!string.IsNullOrEmpty(form.evrakBirimKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", birimKod, "LIKE", form.evrakBirimKodu + "%"));
            if (!form.evrakTarih.isNull)
            {
                if (form.sorguEvrakTarih.isNull)
                    form.sorguEvrakTarih = new TNSDateTime(form.evrakTarih.Oku());//Tarih alanında saat bilgisi olduğu için aralık sorgusu yapılması gerekli

                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTARIH", ">=", form.evrakTarih));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTARIH", "<", new TNSDateTime(form.sorguEvrakTarih.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
            }
            if (!string.IsNullOrEmpty(form.geldigiYer) && form.geldigiYer.Length > 1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "GELDIGIYER", "LIKE", form.geldigiYer + "%"));
            if (!string.IsNullOrEmpty(form.konu) && form.konu.Length > 1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "KONU", "LIKE", form.konu + "%"));
            if (form.gelisSekli > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "GELISSEKLI", "=", form.gelisSekli));
            if (form.sicil.asil > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKSICIL", "ASIL ", "=", form.sicil.asil));


            sql += " WHERE " + kosul;
            if (birimKodAlan == "''") birimKodAlan = "";
            else birimKodAlan += ",";
            sql += " GROUP BY " + birimKodAlan + " EVREVRAK." + degisken + " ";
            if (raporTur == ENUMEvrakRaporTuru.BIRIM)
                sql += ", BIRIM.AD ";
            sql += "ORDER BY COUNT DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListelePersonel(EvrakSinif form)
        {
            return ListelePersonel(form, ENUMEvrakRaporTuru.PERSONEL);
        }

        public IDataReader ListelePersonelToplam(EvrakSinif form)
        {
            return ListelePersonel(form, ENUMEvrakRaporTuru.PERSONELTOPLAM);
        }

        private IDataReader ListelePersonel(EvrakSinif form, ENUMEvrakRaporTuru raporTur)
        {
            int aciklama = OrtakFonksiyonlar.ConvertToInt(form.aciklama, 0);
            ENUMEvrakHareketDurum hareket = (ENUMEvrakHareketDurum)aciklama;
            form.evrakBirimKodu += "%";

            string tableHareket = "";
            string tableKisa1 = "HAREKET";      //tableHareket
            string tableKisa2 = "SICIL";        //EVREVRAKSICIL

            string sql = "", kosul = "";
            string kisi = "";
            string birimKod = "";
            string islemOperator = "", islemDeger = "";
            string grupBy = "";


            switch (raporTur)
            {
                case ENUMEvrakRaporTuru.PERSONEL: tableHareket = "EVREVRAKHAREKETSONDURUM"; form.yil = 0; break;
                case ENUMEvrakRaporTuru.PERSONELTOPLAM: tableHareket = "EVREVRAKHAREKET"; break;
            }

            if (hareket == ENUMEvrakHareketDurum.KISIUZERINDE)
            {
                kisi = "KISI1";
                birimKod = "BIRIMKOD1";
                islemOperator = "<";
                islemDeger = "50";
            }
            else if (hareket == ENUMEvrakHareketDurum.HAVALEEDILDI)
            {
                kisi = "KISI2";
                birimKod = "BIRIMKOD2";
                islemOperator = "=";
                islemDeger = "51";
            }

            sql = "SELECT COUNT(DISTINCT {0}.PREVRAKNO) AS EVRAKSAYISI, {0}.{2} AS TCKIMLIKNO " +
                  "FROM EVREVRAKSICIL AS {1} INNER JOIN " +
                  "EVREVRAK ON SICIL.EVREVRAKNO = EVREVRAK.EVREVRAKNO INNER JOIN " +
                  "{3} AS {0} ON SICIL.PREVRAKNO = {0}.PREVRAKNO ";
            sql = string.Format(sql, tableKisa1, tableKisa2, kisi, tableHareket);

            if (form.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "YIL", "=", form.yil));
            if (form.evrakTipi > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTIPI", "=", form.evrakTipi));
            if (!string.IsNullOrEmpty(form.evrakBirimKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tableKisa1, birimKod, "LIKE", form.evrakBirimKodu));
            if (!string.IsNullOrEmpty(islemDeger))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tableKisa1, "ISLEM", islemOperator, islemDeger));
            if (form.sicil.asil > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tableKisa2, "ASIL ", "=", form.sicil.asil));


            grupBy = " GROUP BY " + tableKisa1 + "." + kisi;
            sql += " WHERE " + kosul + grupBy;

            //sql = "SELECT COUNT(HAREKET.KISI1) AS EVRAKSAYISI, HAREKET.KISI1 AS TCKIMLIKNO " +
            //"FROM EVREVRAKSICIL AS SICIL INNER JOIN " +
            //"EVREVRAK ON SICIL.EVREVRAKNO = EVREVRAK.EVREVRAKNO INNER JOIN " +
            //"EVREVRAKHAREKETHAREKETDURUM AS HAREKET ON SICIL.PREVRAKNO = HAREKET.PREVRAKNO " +
            //"WHERE (HAREKET.BIRIMKOD1 = 'B.07.0.SGB.0.01') AND (HAREKET.ISLEM < 50) AND (EVREVRAK.EVRAKTIPI = 1) " +
            //"GROUP BY HAREKET.KISI1";

            //sql = "SELECT COUNT(HAREKET.KISI2) AS EVRAKSAYISI, HAREKET.KISI2 AS TCKIMLIKNO" +
            //"FROM EVREVRAKSICIL AS SICIL INNER JOIN" +
            //"EVREVRAK ON SICIL.EVREVRAKNO = EVREVRAK.EVREVRAKNO INNER JOIN" +
            //"EVREVRAKHAREKETHAREKETDURUM AS HAREKET ON SICIL.PREVRAKNO = HAREKET.PREVRAKNO" +
            //"WHERE (EVREVRAK.EVRAKTIPI = 1) AND (HAREKET.ISLEM = 51) AND (HAREKET.BIRIMKOD2 = 'B.07.0.SGB.0.02')" +
            //"GROUP BY HAREKET.KISI2";

            //sql = "SELECT COUNT(HAREKET.KISI1) AS EVRAKSAYISI, HAREKET.KISI1 AS TCKIMLIKNO " +
            //"FROM EVREVRAKSICIL AS SICIL INNER JOIN " +
            //"EVREVRAK ON SICIL.EVREVRAKNO = EVREVRAK.EVREVRAKNO INNER JOIN " +
            //"EVREVRAKHAREKET AS HAREKET ON SICIL.PREVRAKNO = HAREKET.PREVRAKNO " +
            //"WHERE (EVREVRAK.EVRAKTIPI = 1) AND (HAREKET.BIRIMKOD1 = 'B.07.0.SGB.0.02') AND (HAREKET.ISLEM < '50') " +
            //"GROUP BY HAREKET.KISI1";

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
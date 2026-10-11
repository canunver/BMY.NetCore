using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABRapor : TABOrtak
    {
        public TABRapor(VeriErisim veriErisim) : base(veriErisim) { }

        public IDataReader HibeMakineIlBazindaIcmal(HibeMakine kriter)
        {
            string sql = "";

            sql += " SELECT TRMHIBEMAKINA.ILKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT, SUM(TRMHIBEMAKINADETAY.PROFORMATUTAR),SUM(TRMHIBEMAKINADETAY.HIBETUTAR),COUNT(*)";
            sql += " FROM TRMHIBEMAKINA INNER JOIN TRMHIBEMAKINADETAY ON TRMHIBEMAKINA.KOD = TRMHIBEMAKINADETAY.HIBEKOD INNER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBEMAKINA.ILKOD = ORGCOGRAFYA.KOD";
            sql += " WHERE TRMHIBEMAKINA.ETAPNO = " + kriter.etapNo;
            sql += " GROUP BY TRMHIBEMAKINA.ILKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HibeMakineIlceBazindaIcmal(HibeMakine kriter)
        {
            string sql = "";

            sql += " SELECT TRMHIBEMAKINA.ILCEKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT, SUM(TRMHIBEMAKINADETAY.PROFORMATUTAR),SUM(TRMHIBEMAKINADETAY.HIBETUTAR),COUNT(*)";
            sql += " FROM TRMHIBEMAKINA INNER JOIN TRMHIBEMAKINADETAY ON TRMHIBEMAKINA.KOD = TRMHIBEMAKINADETAY.HIBEKOD INNER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBEMAKINA.ILCEKOD = ORGCOGRAFYA.KOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBEMAKINA", "ETAPNO", "=", kriter.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEMAKINA", "ILKOD", "=", kriter.ilKod));
            sql += " GROUP BY TRMHIBEMAKINA.ILCEKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HibeSulamaIlBazindaIcmal(HibeSulama kriter)
        {
            string sql = " SELECT TRMHIBESULAMA.ILKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT, SUM(TRMHIBESULAMADETAY.PROFORMATUTAR), SUM(TRMHIBESULAMADETAY.HIBETUTAR), COUNT(*), SUM(TRMHIBESULAMA.SULANACAKALAN)";
            sql += " FROM TRMHIBESULAMA INNER JOIN TRMHIBESULAMADETAY ON TRMHIBESULAMA.KOD = TRMHIBESULAMADETAY.HIBEKOD INNER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBESULAMA.ILKOD = ORGCOGRAFYA.KOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBESULAMA", "ETAPNO", "=", kriter.etapNo));
            sql += " GROUP BY TRMHIBESULAMA.ILKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HibeSulamaIlceBazindaIcmal(HibeSulama kriter)
        {
            string sql = " SELECT TRMHIBESULAMA.ILCEKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT, SUM(TRMHIBESULAMADETAY.PROFORMATUTAR), SUM(TRMHIBESULAMADETAY.HIBETUTAR), COUNT(*), SUM(TRMHIBESULAMA.SULANACAKALAN)";
            sql += " FROM TRMHIBESULAMA INNER JOIN TRMHIBESULAMADETAY ON TRMHIBESULAMA.KOD = TRMHIBESULAMADETAY.HIBEKOD INNER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBESULAMA.ILCEKOD = ORGCOGRAFYA.KOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBESULAMA", "ETAPNO", "=", kriter.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBESULAMA", "ILKOD", "=", kriter.ilKod));
            sql += " GROUP BY TRMHIBESULAMA.ILCEKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HibeYatirimIlBazindaIcmal(HibeYatirim kriter)
        {
            string sql = "";

            sql += " SELECT TRMHIBEYATIRIM.ILKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT, SUM(TRMHIBEYATIRIM.BASVURUMALIYET),SUM(TRMHIBEYATIRIM.HIBETUTAR),COUNT(*)";
            sql += " FROM TRMHIBEYATIRIM INNER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD";
            sql += " WHERE TRMHIBEYATIRIM.ETAPNO = " + kriter.etapNo;
            sql += " GROUP BY TRMHIBEYATIRIM.ILKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HibeYatirimIlceBazindaIcmal(HibeYatirim kriter)
        {
            string sql = "";

            sql += " SELECT TRMHIBEYATIRIM.ILCEKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT, SUM(TRMHIBEYATIRIM.BASVURUMALIYET),SUM(TRMHIBEYATIRIM.HIBETUTAR),COUNT(*)";
            sql += " FROM TRMHIBEYATIRIM INNER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBEYATIRIM.ILCEKOD = ORGCOGRAFYA.KOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ETAPNO", "=", kriter.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ILKOD", "=", kriter.ilKod));
            sql += " GROUP BY TRMHIBEYATIRIM.ILCEKOD, ORGCOGRAFYA.AD, ORGCOGRAFYA.KOORDINAT";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader MalzemeFiyatAnalizi(MalzemeFiyatAnalizi form)
        {
            string sql = "";

            sql += " SELECT TRMHIBEMAKINA.ETAPNO, TRMMALZEME.MALZEMEGRUBUKOD, TRMHIBEMAKINADETAY.MALZEMEKOD, ";
            sql += " MIN(TRMHIBEMAKINADETAY.REFERANSTUTAR) AS REFERANSTUTAR, MIN(TRMHIBEMAKINADETAY.PROFORMATUTAR) AS DUSUK, ";
            sql += " MAX(TRMHIBEMAKINADETAY.PROFORMATUTAR) AS YUKSEK, COUNT(*) AS ADET, TRMMALZEMEGRUBU.AD AS MALZEMEGRUBU, TRMMALZEME.AD AS MALZEME, ";
            sql += " ORGCOGRAFYA.AD AS ILAD";
            sql += " FROM TRMHIBEMAKINA INNER JOIN";
            sql += " TRMHIBEMAKINADETAY ON TRMHIBEMAKINA.KOD = TRMHIBEMAKINADETAY.HIBEKOD INNER JOIN";
            sql += " TRMMALZEME ON TRMHIBEMAKINADETAY.MALZEMEKOD = TRMMALZEME.KOD INNER JOIN";
            sql += " TRMMALZEMEGRUBU ON TRMMALZEME.MALZEMEGRUBUKOD = TRMMALZEMEGRUBU.KOD INNER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBEMAKINA.ILKOD = ORGCOGRAFYA.KOD";
            sql += " WHERE (TRMHIBEMAKINA.ETAPNO = " + form.etapNo + ")";

            if (!string.IsNullOrEmpty(form.ilKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEMAKINA", "ILKOD", "=", form.ilKod));
            if (form.malzemeGrupKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMMALZEME", "MALZEMEGRUBUKOD", "=", form.malzemeGrupKod));
            if (form.malzemeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMMALZEME", "MALZEMEKOD", "=", form.malzemeKod));
            if (form.durum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEMAKINA", "DURUM", "=", form.durum));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEMAKINA", "DURUM", "<>", 99));

            sql += " GROUP BY TRMHIBEMAKINA.ETAPNO, TRMMALZEME.MALZEMEGRUBUKOD, TRMHIBEMAKINADETAY.MALZEMEKOD, TRMMALZEMEGRUBU.AD, TRMMALZEME.AD, ORGCOGRAFYA.AD";
            sql += " ORDER BY TRMHIBEMAKINA.ETAPNO, TRMMALZEMEGRUBU.AD, TRMMALZEME.AD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IlListesi(int ilTur)
        {
            string sql = "select KOD, AD, PLAKAKOD from ORGCOGRAFYA where kod = case when len(KOD) = 7 then KOD end";

            if (ilTur != 0)
            {
                if (ilTur == 1 || ilTur == 3 || ilTur == 4)
                    sql = VeriErisim.KosulEkle(sql, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", ilTur.ToString()));
                else if (ilTur == 2)
                    sql = VeriErisim.KosulEkle(sql, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "1"));
            }

            sql += " order by AD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YatirimIzlemeCetveli(int etapNo, ENUMYatirimRaporAsama asama, int ilTur, int konuTur)
        {
            string sql = "";
            string kosul = "";

            sql += " select Ilkod,count(*),sum(toplamtutar),sum(hIbetutar)";
            sql += " from TRMHIBEYATIRIM";

            if (ilTur != 0)
            {
                sql += " INNER JOIN ORGCOGRAFYA ON Ilkod = ORGCOGRAFYA.KOD";
                if (ilTur == 1 || ilTur == 3 || ilTur == 4)
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", ilTur.ToString()));
                else if (ilTur == 2)
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "1"));
            }

            if (etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ETAPNO", "=", etapNo));

            if (kosul != "")
                sql += " where " + kosul + " and";
            else
                sql += " where";

            if (asama == ENUMYatirimRaporAsama.MERKEZKABUL)
                sql += " durum>= 5 and durum <98";
            else if (asama == ENUMYatirimRaporAsama.SOZLESMEYAPILAN)
                sql += " durum>= 10 and durum <=11";
            else if (asama == ENUMYatirimRaporAsama.ILKABUL)
                sql += " (ANKET1 = 1) AND (ANKET2 = 1) AND (ANKET4 = 1) and durum<=99"; // AND (ANKET3 = 1) alt yapıda değerndirme yapılmadığı için çıkartıldı Melih 29.05.2021
            else
                sql += " durum >= 1 and durum <=99";

            if (konuTur == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", "<", "51"));
            if (konuTur == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", ">=", "51"));

            sql += " group by Ilkod";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YatirimIzlemeCetveliDiger(int etapNo, ENUMYatirimRaporAsama asama, int ilTur, int konuTur)
        {
            string kosul = "";
            string sql = @"select Ilkod,count(*),sum(TRMHIBEYATIRIMMALIYET.PROJETOPLAMTUTAR), SUM(ODENEN), sum(TRMHIBEYATIRIMMALIYET.HIBETUTAR)
                            from TRMHIBEYATIRIM
                            inner join TRMHIBEYATIRIMSOZLESME ON TRMHIBEYATIRIM.kod = TRMHIBEYATIRIMSOZLESME.HIBEKOD
                            LEFT OUTER JOIN HIBEYATIRIMODENEN ON HIBEYATIRIMODENEN.HIBEKOD = TRMHIBEYATIRIM.KOD
                            LEFT OUTER JOIN TRMHIBEYATIRIMMALIYET ON TRMHIBEYATIRIMMALIYET.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD AND TRMHIBEYATIRIMMALIYET.DURUM = " + (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI;

            if (ilTur != 0)
            {
                sql += " INNER JOIN ORGCOGRAFYA ON Ilkod = ORGCOGRAFYA.KOD";
                if (ilTur == 1 || ilTur == 3 || ilTur == 4)
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", ilTur));
                else if (ilTur == 2)
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "1"));
            }

            if (etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ETAPNO", "=", etapNo));

            if (asama == ENUMYatirimRaporAsama.SOZLESMESIDEVAMEDEN)
                kosul = VeriErisim.KosulEkle(kosul, " AND", "TRMHIBEYATIRIMSOZLESME.DURUM IN (0,1)");//(int)ENUMHibeYatirimSozlesmeDurum.DEVAMEDIYOR VEYA 0
            else if (asama == ENUMYatirimRaporAsama.SOZLESMESIBITEN)
                kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "DURUM ", "=", (int)ENUMHibeYatirimSozlesmeDurum.BITTI));
            else if (asama == ENUMYatirimRaporAsama.SOZLESMESIIPTALOLAN)
            {
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("(TRMHIBEYATIRIMSOZLESME", "DURUM ", "=", (int)ENUMHibeYatirimSozlesmeDurum.HATALI));
                kosul = VeriErisim.KosulEkle(kosul, " OR ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "DURUM ", "=", (int)ENUMHibeYatirimSozlesmeDurum.IPTAL));
                kosul += ")";
            }
            else
                kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "DURUM ", ">=", 0));

            if (konuTur == 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", "<", "51"));
            if (konuTur == 1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", ">=", "51"));


            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            sql += " group by Ilkod";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YatirimIzlemeCetveliOdenenHakedis(int etapNo, int ilTur, int konuTur)
        {
            string kosul = "";
            //string sql = @"select ILKOD, SUM(ODENENKOMISYONLU) --SUM(FATURATUTAR / 2 * 1.002)
            string sql = @"select ILKOD, SUM(ODENEN) 
                            from TRMHIBEYATIRIM
                            INNER JOIN TRMHIBEYATIRIMSOZLESME ON TRMHIBEYATIRIM.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD
                            LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = TRMHIBEYATIRIM.KOD";
            //                       --   INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD
            //                       --   INNER JOIN TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD 
            //                       --   AND TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM =  " + (int)ENUMHibeYatirimHakedisDurum.ODENDI;

            if (ilTur != 0)
            {
                sql += " INNER JOIN ORGCOGRAFYA ON Ilkod = ORGCOGRAFYA.KOD";
                if (ilTur == 1 || ilTur == 3 || ilTur == 4)
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", ilTur));
                else if (ilTur == 2)
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "1"));
            }

            if (etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ETAPNO", "=", etapNo));

            if (konuTur == 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", "<", "51"));
            if (konuTur == 1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", ">=", "51"));

            if (kosul != "")
                sql += " where " + kosul;

            sql += " group by Ilkod";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader MakinaEkipmanDesteklemeleri(int etapNo, int odemeIlTur)
        {
            string sql = "";

            sql += "select C.AD, SUM(A.ADET), SUM(A.TOPLAMTUTAR) ";
            sql += "from TRMHIBEMAKINASULAMAVIEW  A, TRMMALZEME B, TRMMALZEMEGRUBU C ";

            if (odemeIlTur != 0)
            {
                sql += ", ORGCOGRAFYA D ";
                sql += "where D.KOD = A.ILKOD ";
                sql += "and ";
            }
            else
                sql += "where ";

            sql += "A.MALZEMEKOD = B.KOD and b.malzemegrubukod= c.kod ";

            if (etapNo != 0)
                sql += "and etapno= " + etapNo;

            if (odemeIlTur == 1 || odemeIlTur == 3 || odemeIlTur == 4)
                sql += " AND D.GAP=" + odemeIlTur;
            else if (odemeIlTur == 2)
                sql += " AND D.GAP<>" + "1";

            sql += " GROUP BY C.AD ";
            sql += "order by sum(Adet) desc";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KKYDPOdeme(int etapNo, int odemeIlTur)
        {
            string sql = "";

            sql += "select A.EtapNo+2004, SUM(A.ADET), SUM(A.TOPLAMTUTAR) ";
            sql += "from TRMHIBEMAKINASULAMAVIEW A, TRMMALZEME B, TRMMALZEMEGRUBU C ";

            if (odemeIlTur != 0)
            {
                sql += ", ORGCOGRAFYA D ";
                sql += "where D.KOD = A.ILKOD ";
                sql += "and ";
            }
            else
                sql += "where ";

            sql += "A.MALZEMEKOD = B.KOD and b.malzemegrubukod= c.kod ";

            if (etapNo != 0)
                sql += "and etapno= " + etapNo;

            if (odemeIlTur == 1 || odemeIlTur == 2 || odemeIlTur == 3)
                sql += " AND D.GAP=" + odemeIlTur;
            else if (odemeIlTur == 2)
                sql += " AND D.GAP<>" + "1";

            sql += " GROUP BY A.EtapNo+2004 ";
            sql += "order by A.EtapNo+2004 asc";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader EYHibeTutariGetir(int etapNo, int odemeIlTur)
        {
            string sql = "";

            sql += "select A.YIL,Sum(A.HIBETUTAR) from TRMHIBEYATIRIM A ";

            if (odemeIlTur != 0)
            {
                sql += ", ORGCOGRAFYA D ";
                sql += "where D.KOD = A.ILKOD ";
                sql += "and ";
            }
            else
                sql += "where ";

            sql += " A.YIL > 2012 ";

            if (etapNo != 0)
                sql += "and A.etapno= " + etapNo;

            if (odemeIlTur == 1 || odemeIlTur == 3 || odemeIlTur == 4)
                sql += " AND D.GAP=" + odemeIlTur;
            else if (odemeIlTur == 2)
                sql += " AND D.GAP<>" + "1";

            sql += " GROUP BY A.YIL";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader TBasincliSulamaGenelDurum(int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            string icKosul = "";
            if (etapNo > 0)
                icKosul += " AND B.ETAPNO = " + etapNo;
            else
                icKosul += " AND B.ETAPNO>0 ";

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul2 = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul2.Trim() != "")
                    icKosul += " AND " + icKosul;
            }

            if (!t1.isNull)
                icKosul = VeriErisim.KosulEkle(icKosul, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBASTARIH", ">=", t1));
            if (!t2.isNull)
                icKosul = VeriErisim.KosulEkle(icKosul, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));


            string sql = @"SELECT 
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND A.KOD=B.PROJEKONU " + icKosul + " GROUP BY B.PROJEKONU) AS bittisozlesmeSayisi,";
            sql += @" (SELECT SUM(FATURATUTAR / 2 * 1.002)
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                        TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                        TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                        TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND A.KOD=B.PROJEKONU  " + icKosul + " GROUP BY B.PROJEKONU) AS bittiodenenTutar";
            sql += @" FROM TRMYATIRIMKONU AS A WHERE A.KOD = '20'";//001

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KirsalKalkinmaYatirimTesisMakineSulamaOdenek(int etapNo, string ilKod, int aktifEtapNo, TNSDateTime t1, TNSDateTime t2)
        {
            string sql = @"SELECT KOD,ADI,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 1) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS devamsozlesmeSayisi,
                        (SELECT SUM(ODENENKOMISYONLU) --SUM(FATURATUTAR / 2 * 1.002) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
--                      INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
--                      TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
--                      TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 1) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS devamodenenTutar,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS bittisozlesmeSayisi,
                        (SELECT SUM(ODENENKOMISYONLU) --SUM(FATURATUTAR / 2 * 1.002)
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
                   --   INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                   --   TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                   --   TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS bittiodenenTutar,
                        (SELECT sum(TRMHIBEYATIRIMSOZLESME.ISTIHDAM) FROM TRMHIBEYATIRIM B
                        INNER JOIN TRMHIBEYATIRIMSOZLESME ON TRMHIBEYATIRIMSOZLESME.HIBEKOD = B.KOD
                        WHERE A.KOD=B.PROJEKONU AND (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') ";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS istihdam,
                        (SELECT sum(TRMHIBEYATIRIMSOZLESME.ISTIHDAMKADIN) FROM TRMHIBEYATIRIM B
                        INNER JOIN TRMHIBEYATIRIMSOZLESME ON TRMHIBEYATIRIMSOZLESME.HIBEKOD = B.KOD
                        WHERE A.KOD=B.PROJEKONU AND (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') ";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS kistihdam,
                       (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 4) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEKONU AND B.ETAPNO = " + (aktifEtapNo - 1);
            sql += @"   GROUP BY B.PROJEKONU) AS oncekiEtapdaIptalSayi  FROM TRMYATIRIMKONU AS A WHERE LEN(KOD)=2 order by SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ProjeNiteligineGoreHibeTutarDagilimi(int etapNo, string ilKod, int aktifEtapNo, TNSDateTime t1, TNSDateTime t2)
        {
            string sql = @"SELECT KOD,ADI,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 1) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJENITELIK";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJENITELIK) AS devamsozlesmeSayisi,
                        (SELECT SUM(ODENENKOMISYONLU) --SUM(FATURATUTAR / 2 * 1.002) 
                        FROM TRMHIBEYATIRIM B 
                        INNER JOIN TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
                    --  INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                    --  TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                    --  TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 1) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJENITELIK";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJENITELIK) AS devamodenenTutar,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJENITELIK";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJENITELIK) AS bittisozlesmeSayisi,
                        (SELECT SUM(ODENENKOMISYONLU) -- SUM(FATURATUTAR / 2 * 1.002)
                        FROM TRMHIBEYATIRIM B 
                        INNER JOIN TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
                  --    INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                  --    TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                  --    TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJENITELIK";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJENITELIK) AS bittiodenenTutar,";
            sql += @"    (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 4) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJENITELIK AND B.ETAPNO = " + (aktifEtapNo - 1);

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJENITELIK) AS oncekiEtapIptalSayisi FROM TRMYATIRIMNITELIK AS A";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader MakineSulamaToplamOdenen(int etapNo, string ilKod, int tip, TNSDateTime t1, TNSDateTime t2)
        {
            string tabloAd = "";
            string tabloDetayAd = "";
            string sayilacak = "";

            if (tip == 1)
            {
                tabloAd = "TRMHIBEMAKINA";
                tabloDetayAd = "TRMHIBEMAKINADETAY";
                sayilacak = "ADET";

            }
            else
            {
                tabloAd = "TRMHIBESULAMA";
                tabloDetayAd = "TRMHIBESULAMADETAY";
                sayilacak = "1";
            }

            string sql = @"select sum(" + sayilacak + "), tutar = ";

            sql += @" sum(case when (" + tabloAd + ".YIL>=2012) then " +
            "case when (" + tabloDetayAd + ".EKSIKODEMEDUZELTILDI=1 AND " + tabloDetayAd + ".EKSIKHIBETUTARI>0) then " +
                "(" + tabloDetayAd + ".HIBETUTAR+ " + tabloDetayAd + ".EKSIKHIBETUTARI)*1.002 " +
            " else " +
                "(" + tabloDetayAd + ".HIBETUTAR)*1.002 end" +
            " else " + tabloDetayAd + ".HIBETUTAR end)";

            if (tip == 2)
                sql += ", sum(SULANACAKALAN)";
            sql += " from " + tabloAd +
                        " INNER JOIN " + tabloDetayAd +
                        " ON " + tabloAd + ".KOD = " + tabloDetayAd + ".HIBEKOD" +
                        " where " + tabloAd + ".DURUM >= 20 AND " + tabloAd + ".DURUM <> 99";
            if (etapNo > 0)
                sql += " AND " + tabloAd + ".ETAPNO = " + etapNo;

            if (tip != 1)
            {
                if (!t1.isNull && tabloAd.IndexOf("SULAMA") > -1)
                    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBESULAMA", "FATURATARIH", ">=", t1));
                if (!t2.isNull && tabloAd.IndexOf("SULAMA") > -1)
                    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBESULAMA", "FATURATARIH", "<=", t2));
                if (!t1.isNull && tabloAd.IndexOf("MAKINA") > -1)
                    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEMAKINA", "ODEMETARIH", ">=", t1));
                if (!t2.isNull && tabloAd.IndexOf("MAKINA") > -1)
                    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEMAKINA", "ODEMETARIH", "<=", t2));
            }

            if (ilKod != "")
            {
                string icKosul = SplitKodlar(ilKod, tabloAd + ".ILKOD");
                sql += " AND " + icKosul;
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HibeYatirimGenelDurum(int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            string sql = @"SELECT 
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.HIBEKOD=B.KOD
                        GROUP BY TRMHIBEYATIRIMSOZLESME.HIBEKOD) AS bittisozlesmeSayisi,
                        (SELECT SUM(ODENENKOMISYONLU) --SUM(FATURATUTAR / 2 * 1.002)
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
                  --    INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                  --    TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                  --    TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.HIBEKOD=B.KOD
                        GROUP BY TRMHIBEYATIRIMSOZLESME.HIBEKOD) AS bittiOdenenTutar,
                        (SELECT SUM(ODENENKOMISYONLU) --SUM(FATURATUTAR / 2 * 1.002)
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
                   --   INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                   --   TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                   --   TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 1) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.HIBEKOD=B.KOD
                        GROUP BY TRMHIBEYATIRIMSOZLESME.HIBEKOD) AS devamOdenenTutar
                        FROM TRMHIBEYATIRIMSOZLESME A INNER JOIN TRMHIBEYATIRIM C
                        ON A.HIBEKOD = C.KOD ";
            if (etapNo > 0)
                sql += " AND C.ETAPNO = " + etapNo;

            if (ilKod != "")
            {
                string icKosul = SplitKodlar(ilKod, "C.ILKOD");
                sql += " AND " + icKosul;
            }

            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("A", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("A", "PROJEBITTARIH", "<=", t2));


            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KKYDPIlTurBazliProjeler(int etapNo, int ilTur, int aktifEtapNo)
        {

            string sql = @"select ilad, konuad ,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD left outer join
                        TRMYATIRIMKONU ON b.PROJEKONU = TRMYATIRIMKONU.KOD INNER JOIN
                        ORGCOGRAFYA ON ORGCOGRAFYA.KOD = b.ILKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND tt.ilkod = b.ILKOD and tt.konukod=b.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (ilTur > 0)
                sql += " AND ORGCOGRAFYA.GAP ='" + ilTur + "'";
            sql += @"         group by ORGCOGRAFYA.AD, TRMYATIRIMKONU.ADI) as bitenSayi,
                        (SELECT SUM(ODENENKOMISYONLU) as TUTAR --SUM(FATURATUTAR / 2 * 1.002) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
            --          INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
            --          TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
            --          TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        inner join TRMYATIRIMKONU ON b.PROJEKONU = TRMYATIRIMKONU.KOD INNER JOIN
                        ORGCOGRAFYA ON ORGCOGRAFYA.KOD = b.ILKOD          
                        WHERE TRMHIBEYATIRIMSOZLESME.DURUM = 2 AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND tt.ilkod = b.ILKOD and tt.konukod=b.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (ilTur > 0)
                sql += " AND ORGCOGRAFYA.GAP ='" + ilTur + "'";
            sql += @"   GROUP BY ORGCOGRAFYA.AD,TRMYATIRIMKONU.ADI) as bitenTutar,
                        (SELECT sum(C.ISTIHDAM) as istihdam
                        FROM TRMHIBEYATIRIM B
                        INNER JOIN TRMHIBEYATIRIMSOZLESME C ON C.HIBEKOD = B.KOD
                        inner join TRMYATIRIMKONU ON b.PROJEKONU = TRMYATIRIMKONU.KOD INNER JOIN
                        ORGCOGRAFYA ON ORGCOGRAFYA.KOD = b.ILKOD
                        WHERE B.DURUM < 99 AND (B.ALTYAPIGRUP<>'A') and C.DURUM = 2 AND tt.ilkod = b.ILKOD and tt.konukod=b.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (ilTur > 0)
                sql += " AND ORGCOGRAFYA.GAP ='" + ilTur + "'";
            sql += @" GROUP BY ORGCOGRAFYA.AD,TRMYATIRIMKONU.ADI) as bitenIstihdam,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD left outer join
                        TRMYATIRIMKONU ON b.PROJEKONU = TRMYATIRIMKONU.KOD INNER JOIN
                        ORGCOGRAFYA ON ORGCOGRAFYA.KOD = b.ILKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 1) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND tt.ilkod = b.ILKOD and tt.konukod=b.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (ilTur > 0)
                sql += " AND ORGCOGRAFYA.GAP ='" + ilTur + "'";
            sql += @" group by ORGCOGRAFYA.AD, TRMYATIRIMKONU.ADI) as devamSayi,
                        (SELECT SUM(ODENENKOMISYONLU) as TUTAR -- SUM(FATURATUTAR / 2 * 1.002) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
                --      INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                --      TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                --      TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        inner join TRMYATIRIMKONU ON b.PROJEKONU = TRMYATIRIMKONU.KOD INNER JOIN
                        ORGCOGRAFYA ON ORGCOGRAFYA.KOD = b.ILKOD          
                        WHERE TRMHIBEYATIRIMSOZLESME.DURUM = 1 AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND tt.ilkod = b.ILKOD and tt.konukod=b.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (ilTur > 0)
                sql += " AND ORGCOGRAFYA.GAP ='" + ilTur + "'";
            sql += @" GROUP BY ORGCOGRAFYA.AD,TRMYATIRIMKONU.ADI) as devamTutar,";
            sql += @" (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD left outer join
                        TRMYATIRIMKONU ON b.PROJEKONU = TRMYATIRIMKONU.KOD INNER JOIN
                        ORGCOGRAFYA ON ORGCOGRAFYA.KOD = b.ILKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 4) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND tt.ilkod = b.ILKOD and tt.konukod=b.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (ilTur > 0)
                sql += " AND ORGCOGRAFYA.GAP ='" + ilTur + "'";
            sql += @"   group by ORGCOGRAFYA.AD, TRMYATIRIMKONU.ADI) as iptalSayi,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD left outer join
                        TRMYATIRIMKONU ON b.PROJEKONU = TRMYATIRIMKONU.KOD INNER JOIN
                        ORGCOGRAFYA ON ORGCOGRAFYA.KOD = b.ILKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 4) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND tt.ilkod = b.ILKOD and tt.konukod=b.PROJEKONU and b.etapNo= " + (aktifEtapNo - 1);
            if (ilTur > 0)
                sql += " AND ORGCOGRAFYA.GAP ='" + ilTur + "'";
            sql += @"     group by ORGCOGRAFYA.AD, TRMYATIRIMKONU.ADI) as oncekiEtap7IptalSayi
                        from 
                        (select t.ILKOD ilkod,o.AD ilad,t.PROJEKONU konukod,pk.ADI konuad,pk.SIRANO pksira
                        from TRMHIBEYATIRIM t
                        left join ORGCOGRAFYA o
                        on t.ILKOD=o.KOD 
                        left join TRMYATIRIMKONU pk
                        on t.PROJEKONU=pk.KOD 
                        INNER JOIN TRMHIBEYATIRIMSOZLESME ON t.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        where t.DURUM < 99 AND (t.ALTYAPIGRUP<>'A') and t.DURUM>=10";
            if (etapNo > 0)
                sql += " AND t.ETAPNO = " + etapNo;
            if (ilTur > 0)
                sql += " AND o.GAP ='" + ilTur + "'";
            sql += @" group by t.ILKOD ,o.AD ,t.PROJEKONU ,pk.ADI,pk.SIRANO) as tt
                        --having (count()>0 or count(devamSayi)>0) 	
                        order by ilad,konuad,pksira";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KKYDPIlTurBazliBitenProjeler(int etapNo, int ilTur)
        {
            string sql = @"SELECT T1.AD, T1.ADI, T1.ADET as bitenSayi, T2.TUTAR as bitenTutar, T5.istihdam as bitenIstihdam,t3.ADET as devamSayi,t4.TUTAR as devamTutar
                            FROM
                            (
                            SELECT count(TRMHIBEYATIRIM.KOD) as ADET, TRMYATIRIMKONU.ADI, ORGCOGRAFYA.AD
                            FROM TRMHIBEYATIRIM INNER JOIN
                            TRMHIBEYATIRIMSOZLESME ON TRMHIBEYATIRIM.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                            TRMYATIRIMKONU ON TRMHIBEYATIRIM.PROJEKONU = TRMYATIRIMKONU.KOD INNER JOIN
                            ORGCOGRAFYA ON ORGCOGRAFYA.KOD = TRMHIBEYATIRIM.ILKOD  
                            where TRMHIBEYATIRIMSOZLESME.DURUM = 2 AND TRMHIBEYATIRIM.DURUM < 99 AND (TRMHIBEYATIRIM.ALTYAPIGRUP<>'A') ";
            if (etapNo > 0)
                sql += " AND TRMHIBEYATIRIM.ETAPNO = " + etapNo;
            if (ilTur > 0)
                sql += " AND ORGCOGRAFYA.GAP ='" + ilTur + "'";
            sql += @"  group by TRMYATIRIMKONU.ADI, ORGCOGRAFYA.AD
                            )  T1
                            left outer join
                            (
                            SELECT SUM(ODENENKOMISYONLU) as TUTAR,TRMYATIRIMKONU.ADI, ORGCOGRAFYA.AD --SUM(FATURATUTAR / 2 * 1.002)
                            FROM TRMHIBEYATIRIM B INNER JOIN
                            TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                            LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
                         -- INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                         -- TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                         -- TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                            inner join TRMYATIRIMKONU ON b.PROJEKONU = TRMYATIRIMKONU.KOD INNER JOIN
                            ORGCOGRAFYA ON ORGCOGRAFYA.KOD = b.ILKOD          
                            WHERE TRMHIBEYATIRIMSOZLESME.DURUM = 2 AND B.DURUM < 99 AND (B.ALTYAPIGRUP<>'A')";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;
            if (ilTur > 0)
                sql += " AND ORGCOGRAFYA.GAP ='" + ilTur + "'";
            sql += @" GROUP BY TRMYATIRIMKONU.ADI, ORGCOGRAFYA.AD
                        )  T2 ON T1.ADI = T2.ADI and T1.AD = T2.AD                    
                        ORDER BY T1.AD, T1.ADI";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KKYDPProjelerMandaAyri(int etapNo, string ilKod, TNSDateTime t1, TNSDateTime t2)
        {
            string sql = @"SELECT KOD,ADI,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 1) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS devamsozlesmeSayisi,
                        (SELECT SUM(ODENENKOMISYONLU) -- SUM(FATURATUTAR / 2 * 1.002) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
                 --     INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                 --     TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                 --     TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 1) AND (B.DURUM < 99)  AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS devamodenenTutar,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99)  AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS bittisozlesmeSayisi,
                        (SELECT SUM(ODENENKOMISYONLU) -- SUM(FATURATUTAR / 2 * 1.002)
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD
                        LEFT OUTER JOIN HIBEYATIRIMODENEN on HIBEYATIRIMODENEN.HIBEKOD = B.KOD
                  --    INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                  --    TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                  --    TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS bittiodenenTutar, SIRANO                                                
                        FROM TRMYATIRIMKONU AS A WHERE LEN(KOD)=2 ";

            sql += " union ";

            sql += @"SELECT KOD,ADI,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 1) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEALTKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS devamsozlesmeSayisi,
                        (SELECT SUM(FATURATUTAR / 2 * 1.002) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                        TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                        TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                        TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 1) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEALTKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS devamodenenTutar,
                        (SELECT COUNT(TRMHIBEYATIRIMSOZLESME.HIBEKOD) 
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD 
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEALTKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS bittisozlesmeSayisi,
                        (SELECT SUM(FATURATUTAR / 2 * 1.002)
                        FROM TRMHIBEYATIRIM B INNER JOIN
                        TRMHIBEYATIRIMSOZLESME ON B.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                        TRMHIBEYATIRIMUYGULAMASOZLESME ON TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD INNER JOIN
                        TRMHIBEYATIRIMHAKEDIS ON TRMHIBEYATIRIMHAKEDIS.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND 
                        TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD AND TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM = 20
                        WHERE (TRMHIBEYATIRIMSOZLESME.DURUM = 2) AND (B.DURUM < 99) AND (B.ALTYAPIGRUP<>'A') AND A.KOD=B.PROJEALTKONU";
            if (etapNo > 0)
                sql += " AND B.ETAPNO = " + etapNo;

            if (!string.IsNullOrEmpty(ilKod))
            {
                string icKosul = SplitKodlar(ilKod, "B.ILKOD");
                if (icKosul != "")
                    sql += " AND " + icKosul;
            }
            if (!t1.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", ">=", t1));
            if (!t2.isNull)
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "PROJEBITTARIH", "<=", t2));

            sql += @"   GROUP BY B.PROJEKONU) AS bittiodenenTutar, SIRANO                          
                        FROM TRMYATIRIMKONU AS A WHERE a.KOD='09.35' order by SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        string SplitKodlar(string kod, string alanAdi)
        {
            string icKosul = "";
            string[] kodlar = kod.Split(';');
            foreach (string ik in kodlar)
            {
                if (ik == "") continue;
                if (icKosul != "") icKosul += " OR ";
                icKosul += " " + alanAdi + "='" + ik + "'";
            }
            if (icKosul != "") icKosul = "(" + icKosul + ")";

            return icKosul;
        }
    }
}

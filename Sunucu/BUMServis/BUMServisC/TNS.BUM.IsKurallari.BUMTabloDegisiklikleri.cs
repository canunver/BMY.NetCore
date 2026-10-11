using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.Veri;

namespace TNS.BUM.IsKurallari
{
    public class BUMTabloDegisiklikleri
    {
        private static SQLKomut[] komutlar = {
            new SQLKomut(51, "", @"CREATE VIEW ISLAPGIDERPRG AS SELECT O.YIL YIL
                                      ,O.KURUMKOD KURUMKOD
                                      ,O.ISLEM ISLEM
                                      ,O.SIRANO SIRANO
                                      ,O.FORMDURUM FORMDURUM
	                                  ,Y.TUR PTKOD
	                                  ,I.PRGKOD PRGKOD
	                                  ,I.ALTPRGKOD APRGKOD
	                                  ,I.FAALKOD FAALKOD
	                                  ,I.ALTFAALKOD AFAALKOD
                                      ,O.KURKOD KURKOD
                                      ,O.FINKOD FINKOD
                                      ,O.EKOKOD EKOKOD
                                      ,O.PEBKOD PEBKOD
                                      ,O.FONKOD FONKOD
                                      ,O.AY AY
                                      ,O.ODENEK ODENEK
                                  FROM ISLAPGIDER O
                                  LEFT OUTER JOIN ButceHazirlik.dbo.PRGPEBILISKI I ON
                                  O.YIL=I.YIL AND
                                  O.PEBKOD =I.PEBKOD
                                  LEFT OUTER JOIN ButceHazirlik.dbo.PRGKODYAPI Y ON
                                  Y.YIL =I.YIL AND
                                  Y.KODTUR = 1 AND
                                  Y.KOD = I.PRGKOD", "AHP Program tablosunun oluşturulması"),

            new SQLKomut(52, "", @"create view MPGPRGMASRAFPLANIFORMTERTIP as
                                select MPGMASRAFPLANITERTIP.YIL, MONTH(MPGMASRAFPLANIREVIZEFORM.KAYITTARIH) AY, Y.TUR PTKOD, I.PRGKOD PRGKOD, I.ALTPRGKOD APRGKOD, I.FAALKOD FAALKOD, I.ALTFAALKOD AFAALKOD,
                                MPGMASRAFPLANITERTIP.KURKOD KURKOD, MPGMASRAFPLANITERTIP.FINKOD FINKOD, MPGMASRAFPLANITERTIP.EKOKOD EKOKOD, MPGMASRAFPLANITERTIP.PEBKOD PEBKOD, MPGMASRAFPLANITERTIP.FONKOD FONKOD,
                                MPGMASRAFPLANITERTIP.KURUMSALYAPIKOD KURUMSALYAPIKOD, MPGMASRAFPLANITERTIP.AY PLANAY, MPGMASRAFPLANITERTIP.DURUM DURUM, MPGMASRAFPLANIREVIZEFORM.ALTISLEMKOD ALTISLEMKOD, MPGMASRAFPLANIREVIZEFORM.BELGETUR BELGETUR, MPGMASRAFPLANITERTIP.ODENEK ODENEK, MPGMASRAFPLANITERTIP.REVIZENO REVIZENO
                                FROM MPGMASRAFPLANIREVIZEFORM 
                                INNER JOIN MPGMASRAFPLANITERTIP ON MPGMASRAFPLANIREVIZEFORM.YIL = MPGMASRAFPLANITERTIP.YIL AND MPGMASRAFPLANIREVIZEFORM.REVIZENO = MPGMASRAFPLANITERTIP.REVIZENO 
                                LEFT OUTER JOIN ButceHazirlik.dbo.PRGPEBILISKI I ON MPGMASRAFPLANITERTIP.YIL=I.YIL AND MPGMASRAFPLANITERTIP.PEBKOD =I.PEBKOD 
                                LEFT OUTER JOIN ButceHazirlik.dbo.PRGKODYAPI Y ON  Y.YIL =I.YIL AND Y.KODTUR = 1 AND Y.KOD = I.PRGKOD", "AHP Program tablosunun oluşturulması"),

            new SQLKomut(52, "", @"CREATE TABLE BUMNAKITPLANI (
                TUR         INT        NOT NULL,
                TARIH       DATE       NOT NULL,
                MUHASEBEKOD INT        NOT NULL,
                BIRIMKOD    VARCHAR(3) NOT NULL,
                KURKOD      CHAR(20)   NOT NULL,
                MIKTAR      MONEY      NOT NULL,

                CONSTRAINT PK_BUMNAKITPLANI
                    PRIMARY KEY (TUR, TARIH, MUHASEBEKOD, BIRIMKOD, KURKOD))", "")
            , new SQLKomut(53, "", @"ALTER TABLE BUMNAKITPLANI ADD HARCAMATUR INT NULL", "")

        };

        public static void Kaydet()
        {
            //BUM için
            TabloDegisiklikleri.Kaydet(BUMServis.BaglantiSatiriBul(0), komutlar);
        }
    }
}

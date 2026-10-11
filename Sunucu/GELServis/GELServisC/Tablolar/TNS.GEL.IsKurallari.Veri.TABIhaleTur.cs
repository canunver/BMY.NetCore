using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
     public class TABIGELhaleTur : TABOrtak
    {
         public TABIGELhaleTur(VeriErisim veriErisim) : base(veriErisim) { }
         static string TABLOAD = "GELIHALETUR";
         enum ENUMAlanlar
         {          
             KOD = 0,
             ADI = 1,
         }
         static string[] ALANLAR = { "KOD", "ADI" };

         public IDataReader Listele(TNS.GEL.IhaleTur bilgi)
         {
             string sql = "";
             string kosul = "";
             sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
             if (bilgi != null)
             {
                 if (bilgi.kod > 0)
                     kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
                 if (bilgi.adi != "")
                     kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADI], "=", bilgi.adi));

                 if (kosul != "")
                     sql += " WHERE " + kosul;
             }
             sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " ASC ";

             return veriErisim.DataReaderDondur(sql);
         }

         public Sonuc Kaydet(IhaleTur bilgi)
         {
             Sonuc sonuc = new Sonuc(false, "", "");
             string sql = "";
             sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
             sql += VeriErisim.DegerYap(bilgi.kod) + ",";
             sql += VeriErisim.DegerYap(bilgi.adi) + ")";
             sonuc = KomutCalistir(sql, 1);
             return sonuc;
         }

         public Sonuc Sil(IhaleTur bilgi)
         {
             Sonuc sonuc = new Sonuc(false, "", "");
             string sql = "";
             sql = "DELETE  FROM " + TABLOAD;
             sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
             sonuc = KomutCalistir(sql, 1);
             return sonuc;
         }

         public int KayitVarmi(IhaleTur bilgi)
         {
             
             string sql = "";
             sql = "SELECT count(*) FROM " + TABLOAD;
             sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
             return veriErisim.TekKolonSQLInt(sql);
              
         }

         public Sonuc Guncelle(IhaleTur bilgi)
         {
             Sonuc sonuc = new Sonuc(false, "", "");
             string sql = "";
             sql = "UPDATE " + TABLOAD + " SET ";
             sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.adi);         
             sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
             sonuc = KomutCalistir(sql, 1);
             return sonuc;
         
         }

         public int KayitSiraNoAl()
         {
             string sql = "";
             sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.KOD + "),0)+1 AS MAXSIRANO ";
             sql += " FROM  " + TABLOAD;
             return veriErisim.TekKolonSQLInt(sql);
         }
    }

     public class TABHRCIhaleTur : TABOrtak
     {
         public TABHRCIhaleTur(VeriErisim veriErisim) : base(veriErisim) { }
         static string TABLOAD = "HRCIHALETUR";
         enum ENUMAlanlar
         {
             KOD = 0,
             ADI = 1,
         }
         static string[] ALANLAR = { "KOD", "ADI" };

         public IDataReader Listele(TNS.GEL.IhaleTur bilgi)
         {
             string sql = "";
             string kosul = "";
             sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
             if (bilgi != null)
             {
                 if (bilgi.kod > 0)
                     kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
                 if (bilgi.adi != "")
                     kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADI], "=", bilgi.adi));

                 if (kosul != "")
                     sql += " WHERE " + kosul;
             }
             sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " ASC ";

             return veriErisim.DataReaderDondur(sql);
         }

         public Sonuc Kaydet(IhaleTur bilgi)
         {
             Sonuc sonuc = new Sonuc(false, "", "");
             string sql = "";
             sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
             sql += VeriErisim.DegerYap(bilgi.kod) + ",";
             sql += VeriErisim.DegerYap(bilgi.adi) + ")";
             sonuc = KomutCalistir(sql, 1);
             return sonuc;
         }

         public Sonuc Sil(IhaleTur bilgi)
         {
             Sonuc sonuc = new Sonuc(false, "", "");
             string sql = "";
             sql = "DELETE  FROM " + TABLOAD;
             sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
             sonuc = KomutCalistir(sql, 1);
             return sonuc;
         }

         public int KayitVarmi(IhaleTur bilgi)
         {

             string sql = "";
             sql = "SELECT count(*) FROM " + TABLOAD;
             sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
             return veriErisim.TekKolonSQLInt(sql);

         }

         public Sonuc Guncelle(IhaleTur bilgi)
         {
             Sonuc sonuc = new Sonuc(false, "", "");
             string sql = "";
             sql = "UPDATE " + TABLOAD + " SET ";
             sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.adi);
             sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
             sonuc = KomutCalistir(sql, 1);
             return sonuc;

         }

         public int KayitSiraNoAl()
         {
             string sql = "";
             sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.KOD + "),0)+1 AS MAXSIRANO ";
             sql += " FROM  " + TABLOAD;
             return veriErisim.TekKolonSQLInt(sql);
         }
     }


}

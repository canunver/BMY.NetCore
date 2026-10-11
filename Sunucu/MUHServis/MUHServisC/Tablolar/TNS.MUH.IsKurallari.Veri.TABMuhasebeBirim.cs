using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHMUHASEBEBIRIM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMuhasebeBirim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMuhasebeBirim(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOADBIRIM = "MUHMUHASEBEBIRIM";
        enum ENUMAlanlarBirim
        {
            BIRIMKOD = 0,
            MUHASEBEKOD = 1,
            KURKOD = 2,
            BIRIMAD = 3,
            VERGINO = 4
        }

        static string[] ALANLARBIRIM = { "BIRIMKOD", "MUHASEBEKOD", "KURKOD", "BIRIMAD", "VERGINO" };
        static string[] ALANLARBIRIM1 = { "BIRIMKOD", "MUHASEBEKOD", "KURKOD", "BIRIMAD" };

        static string TABLOADMUHASEBE = "MUHMUHASEBE";
        enum ENUMAlanlarMuhasebe
        {
            MUHASEBEKOD = 0,
            MUHASEBEAD = 1,
        }

        static string[] ALANLARMUHASEBE = { "MUHASEBEKOD", "MUHASEBEAD" };

        static string TABLOADKUR = "ABSKUR";
        enum ENUMAlanlarKur
        {
            YIL = 0,
            KURKOD = 1,
            KURAD = 2,
        }
        static string[] ALANLARKUR = { "YIL", "KURKOD", "KURAD" };


        /// <summary>
        /// Kullanıcının yetkili olduğu muhasebe birimleri
        /// </summary>
        /// <param name="kurum">Kurum.</param>
        /// <param name="muhasebe">Muhasebe kodu.</param>
        /// <returns></returns>
        public IDataReader KullaniciHarcamaBirimleriGetir(ObjectArray kurum, int muhasebe, string kistasKodAd = null)
        {
            string sql = "";
            string kosul = "";

            foreach (string k in kurum.objeler)
            {
                if (kosul != "")
                    kosul += " OR ";

                if (k.Replace(".", "").Length == 2)
                {
                    kosul += "(" + VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], ">=", k) + " AND ";
                    kosul += VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "<", k + OrtakFonksiyonlar.BagimsizKurumBaslangic()) + ")";
                }
                else
                    kosul += VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "LIKE", k.Replace(".", "").Trim() + "%");
            }

            if (muhasebe > 0)
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBIRIM, ALANLARBIRIM1) + "," + VeriErisim.TumAlanlar(TABLOADMUHASEBE, ALANLARMUHASEBE) + " FROM " + TABLOADBIRIM + "," + TABLOADMUHASEBE + " WHERE ";
            else
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMUHASEBE, ALANLARMUHASEBE) + " FROM " + TABLOADBIRIM + "," + TABLOADMUHASEBE + " WHERE ";

            sql += TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD] + "=" + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMuhasebe.MUHASEBEKOD];
            if (kosul != "")
                sql += " AND (" + kosul + ")";
            if (!string.IsNullOrWhiteSpace(kistasKodAd))
            {
                string icSsql;
                int kistasKod = OrtakFonksiyonlar.ConvertToInt(kistasKodAd, 0);
                if (kistasKod > 0)
                {
                    int carpan = 1;
                    if (kistasKod < 100)
                    {
                        carpan = 1000;
                    }
                    else if (kistasKod < 1000)
                    {
                        carpan = 100;
                    }
                    icSsql = VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[(int)ENUMAlanlarMuhasebe.MUHASEBEKOD], ">=", kistasKod * carpan);
                    icSsql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[(int)ENUMAlanlarMuhasebe.MUHASEBEKOD], "<", (kistasKod + 1) * carpan);
                    if (carpan != 1)
                    {
                        icSsql = "((" + icSsql + ") OR " + VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[(int)ENUMAlanlarMuhasebe.MUHASEBEKOD], "=", kistasKod) + ")";
                    }
                }
                else
                    icSsql = VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[(int)ENUMAlanlarMuhasebe.MUHASEBEAD], " LIKE ", "%" + kistasKodAd + "%");
                sql += " AND " + icSsql;
            }

            if (muhasebe > 0)
            {
                sql += " AND " + VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "=", muhasebe.ToString());
                sql += " ORDER BY " + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD] + "," + ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD];
            }
            else
            {
                sql += " GROUP BY " + VeriErisim.TumAlanlar(TABLOADMUHASEBE, ALANLARMUHASEBE);
                sql += " ORDER BY " + VeriErisim.TumAlanlar(TABLOADMUHASEBE, ALANLARMUHASEBE);
            }

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Harcama birimine ait kurumsal kod listesi
        /// </summary>
        /// <param name="yil">Yil.</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="muhasebe">Muhasebe kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <returns></returns>
        public IDataReader HarcamaBirimleriKurumsalKod(int yil, string kurum, int muhasebe, string birim)
        {
            string sql = "";

            sql = "SELECT " + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD] + "," + ALANLARKUR[(int)ENUMAlanlarKur.KURAD] + " FROM " + TABLOADBIRIM + "," + TABLOADKUR + " WHERE ";
            if (kurum.Replace(".", "").Length == 2)
            {
                sql += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], ">=", kurum) + " AND ";
                sql += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "<", kurum + OrtakFonksiyonlar.BagimsizKurumBaslangic()) + " AND ";
            }
            else
                sql += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "LIKE", kurum.Replace(".", "") + "%") + " AND ";

            sql += TABLOADKUR + "." + ALANLARKUR[(int)ENUMAlanlarKur.KURKOD] + "=" + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD] + " AND ";
            sql += VeriErisim.KosulOl(TABLOADKUR, ALANLARKUR[(int)ENUMAlanlarKur.YIL], "=", yil) + " AND ";
            sql += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "=", muhasebe) + " AND ";
            sql += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD], "=", birim);
            sql += " ORDER BY " + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        string KullaniciHarcamaBirimleriKosul(ObjectArray kurum)
        {
            string kosul = "";

            foreach (string k in kurum.objeler)
            {
                if (kosul != "")
                    kosul += " OR ";

                if (k.Replace(".", "").Length == 2)
                {
                    kosul += "(" + VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], ">", k) + " AND ";
                    kosul += VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "<", k + OrtakFonksiyonlar.BagimsizKurumBaslangic()) + ")";
                }
                else
                    kosul += VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "LIKE", k.Replace(".", "").Trim() + "%");
            }

            return kosul;
        }

        /// <summary>
        /// Kullanıcıya ait Harcama birimlerini listeler
        /// </summary>
        /// <param name="kurum">Kurum.</param>
        /// <param name="saymanlikOlsun">Eğer <c>true</c> ise saymanlıklar listelensin.</param>
        /// <returns></returns>
        public IDataReader KullanicininHarcamaBirimleri(ObjectArray kurum, bool saymanlikOlsun)
        {
            string sql = "";
            string kosul = KullaniciHarcamaBirimleriKosul(kurum);
            string saymanlikKosul = "";

            if (kosul != "")
            {
                if (saymanlikOlsun)
                    saymanlikKosul = "," + ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD];

                sql = "SELECT " + ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD] + saymanlikKosul + " FROM " + TABLOADBIRIM + " WHERE ";
                sql += " (" + kosul + ")";

                sql += " GROUP BY " + ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD] + saymanlikKosul;

                IDataReader r = veriErisim.DataReaderDondur(sql);

                return r;
            }
            else
                return null;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebeBirim">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(MuhasebeBirim muhasebeBirim)
        {
            Sonuc sonuc;
            string sql;

            sql = "INSERT INTO " + TABLOADBIRIM + " (" + VeriErisim.TumAlanlar("", ALANLARBIRIM) + ") VALUES (";
            sql += VeriErisim.DegerYap(muhasebeBirim.birimKod) + ", ";
            sql += VeriErisim.DegerYap(muhasebeBirim.muhasebe) + ", ";
            sql += VeriErisim.DegerYap(muhasebeBirim.kurum.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(muhasebeBirim.birimAd) + ",";
            sql += VeriErisim.DegerYap(muhasebeBirim.vno) + ")";

            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOADBIRIM + " SET " + ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD] + "=" + VeriErisim.DegerYap(muhasebeBirim.birimKod); /* Core: degerler DegerYap ile (kesme isaretli ad guncellenemiyordu) */
                sql += ", " + ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD] + "=" + muhasebeBirim.muhasebe + "";
                sql += ", " + ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD] + "=" + VeriErisim.DegerYap(muhasebeBirim.kurum.Replace(".", ""));
                sql += ", " + ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMAD] + "=" + VeriErisim.DegerYap(muhasebeBirim.birimAd);
                sql += ", " + ALANLARBIRIM[(int)ENUMAlanlarBirim.VERGINO] + "=" + VeriErisim.DegerYap(muhasebeBirim.vno);
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD], "=", muhasebeBirim.birimKod);
                sql += " AND " + VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "=", muhasebeBirim.muhasebe);
                sql += " AND " + VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "=", muhasebeBirim.kurum.Replace(".", ""));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebeBirim">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(MuhasebeBirim muhasebeBirim)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADBIRIM + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD], "=", muhasebeBirim.birimKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "=", muhasebeBirim.muhasebe);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "=", muhasebeBirim.kurum.Replace(".", ""));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Tablodaki bütün kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilHepsini()
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADBIRIM;

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebeBirim">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(MuhasebeBirim muhasebeBirim)
        {
            string sql;
            string kosul = "";
            string pasifKosul = ", (SELECT COUNT(*) FROM " + TABMUHMUHASEBEBIRIMKAPALI.TABLOAD + " KAP where KAP.MUHASEBEKOD=MUHMUHASEBEBIRIM.MUHASEBEKOD AND KAP.BIRIMKOD=MUHMUHASEBEBIRIM.BIRIMKOD AND KAP.KURKOD=MUHMUHASEBEBIRIM.KURKOD)";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBIRIM, ALANLARBIRIM1) + "," + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMuhasebe.MUHASEBEAD] + pasifKosul + ", " + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.VERGINO] + " FROM " + TABLOADBIRIM; /* Core: vergi no listede de gosterilir (sona eklendi) */
            sql += " LEFT OUTER JOIN " + TABLOADMUHASEBE + " ON " + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMuhasebe.MUHASEBEKOD] + " = " + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD];

            if (!string.IsNullOrWhiteSpace(muhasebeBirim.birimKod))
            {
                string esit;
                string birimKosul;
                string[] birimler = muhasebeBirim.birimKod.Split(',');
                string icKosul = "";
                string veya = "OR";
                for (int i = 0; i < birimler.Length; i++)
                {
                    birimKosul = birimler[i].Trim();
                    if (string.IsNullOrWhiteSpace(birimKosul)) continue;
                    if (birimKosul.StartsWith("!"))
                    {
                        if (i == 0) veya = "AND";
                        esit = "<>";
                        birimKosul = birimKosul.Substring(1);
                    }
                    else
                    {
                        esit = "=";
                    }
                    icKosul = VeriErisim.KosulEkle(icKosul, veya, VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD], esit, birimKosul));
                }
                kosul += "(" + icKosul + ")";
            }
            if (muhasebeBirim.muhasebe > 0)
            {
                if (kosul != "") kosul += " AND ";
                kosul += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "=", muhasebeBirim.muhasebe);
            }

            if (muhasebeBirim.plakaKod > 0)
            {
                if (kosul != "") kosul += " AND ";
                kosul += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], ">=", muhasebeBirim.plakaKod * 1000);
                kosul += " AND ";
                kosul += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "<", (muhasebeBirim.plakaKod + 1) * 1000);
            }

            if (muhasebeBirim.plakaKod == -10)
            {
                if (kosul != "") kosul += " AND ";
                kosul += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "<", 1000);
            }

            if (!string.IsNullOrWhiteSpace(muhasebeBirim.kurum))
            {
                if (kosul != "") kosul += " AND ";
                if (muhasebeBirim.kurumLike)
                    kosul += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "LIKE", muhasebeBirim.kurum.Replace(".", "") + "%");
                else
                    kosul += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "=", muhasebeBirim.kurum.Replace(".", ""));
            }

            if (!string.IsNullOrWhiteSpace(muhasebeBirim.birimAd))
            {
                if (kosul != "") kosul += " AND ";
                kosul += VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMAD], "LIKE", "%" + muhasebeBirim.birimAd + "%");
            }

            if (!string.IsNullOrWhiteSpace(muhasebeBirim.aramaKosul))
            {
                string[] parcalar = muhasebeBirim.aramaKosul.Split(' ');
                string icKosul = "";
                for (int i = 0; i < parcalar.Length; i++)
                {
                    string parcaKosul;
                    int tamSayi = OrtakFonksiyonlar.ConvertToInt(parcalar[i], -1);
                    if (tamSayi > 0)
                    {
                        int carp = 0;
                        if (tamSayi < 100)
                        {
                            carp = 1000;
                        }
                        else if (tamSayi < 1000)
                        {
                            carp = 100;
                        }
                        else if (tamSayi < 10000)
                        {
                            carp = 100;
                        }
                        parcaKosul = VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "=", tamSayi);
                        if (carp > 0)
                        {
                            parcaKosul = "(" + parcaKosul + " OR (" + VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], ">=", tamSayi * carp) + " AND " + VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "<", (tamSayi + 1) * carp) + "))";
                        }

                        if (tamSayi >= 100 && tamSayi < 1000)
                        {
                            parcaKosul = VeriErisim.KosulEkle(parcaKosul, "OR", VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD], "=", tamSayi.ToString()));
                        }

                        parcaKosul = "(" + parcaKosul + ")";
                    }
                    else
                    {
                        string alanAd = $"{TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMuhasebe.MUHASEBEAD]} +' ' + {TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMAD]}";
                        parcaKosul = VeriErisim.KosulOl("", alanAd, "LIKE", "%" + parcalar[i] + "%");
                    }
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", parcaKosul);
                }
                kosul = VeriErisim.KosulEkle(kosul, "AND", icKosul);
            }

            if (kosul != "")
                sql += " WHERE ";
            sql += kosul;
            sql += " ORDER BY " + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD] + "," + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD] + "," + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        public IDataReader Listele(int saymanliKod, string birimTurler)
        {
            string sql = "SELECT MUHMUHASEBEBIRIM.BIRIMKOD, MUHMUHASEBEBIRIM.MUHASEBEKOD, MUHMUHASEBEBIRIM.KURKOD, MUHMUHASEBEBIRIM.BIRIMAD, MUHMUHASEBEBIRIM.VERGINO, MUHMUHASEBE.MUHASEBEAD,  ORGCOGRAFYA.KOD, ORGCOGRAFYA.AD"
                          + " FROM ORGCOGRAFYA RIGHT OUTER JOIN"
                          + " MUHMUHASEBE ON ORGCOGRAFYA.MUHASEBE = MUHMUHASEBE.MUHASEBEKOD RIGHT OUTER JOIN"
                          + " MUHMUHASEBEBIRIM ON MUHMUHASEBE.MUHASEBEKOD = MUHMUHASEBEBIRIM.MUHASEBEKOD";

            if (saymanliKod > 0)
                sql += " WHERE MUHMUHASEBEBIRIM.MUHASEBEKOD = " + saymanliKod.ToString();
            else
                sql += " WHERE MUHMUHASEBEBIRIM.BIRIMKOD IN (" + Tirnakla(birimTurler) + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleMuhBirim(MuhasebeBirim kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARBIRIM) + " FROM " + TABLOADBIRIM;

            if (kriter.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "=", kriter.muhasebe));

            if (!string.IsNullOrEmpty(kriter.birimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD], "=", kriter.birimKod));

            if (!string.IsNullOrEmpty(kriter.kurum))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "=", kriter.kurum));

            //if (!string.IsNullOrEmpty(kriter.vno))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.VNO], "=", kriter.vno));

            if (kosul != "")
                sql += " WHERE " + kosul;

            //sql += " ORDER BY " + ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        private string Tirnakla(string birimTurler)
        {
            string donDeger = "";
            if (string.IsNullOrWhiteSpace(birimTurler)) return donDeger;
            string[] birimDizi = birimTurler.Split(',');
            for (int i = 0; i < birimDizi.Length; i++)
            {
                if (i > 0) donDeger += ",";
                donDeger += VeriErisim.DegerYap(birimDizi[i]);
            }

            return donDeger;
        }

        public IDataReader KullaniciHarcamaBirimleriGetir(string kurum, string sablon)
        {
            string[] sablonlar = sablon.Split(',', ';');

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBIRIM, ALANLARBIRIM) + "," + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMuhasebe.MUHASEBEAD] + " FROM " + TABLOADBIRIM;
            sql += " LEFT OUTER JOIN " + TABLOADMUHASEBE + " ON " + TABLOADMUHASEBE + "." + ALANLARMUHASEBE[(int)ENUMAlanlarMuhasebe.MUHASEBEKOD] + " = " + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD];
            sql = VeriErisim.KosulEkle(sql, "where", VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "like", kurum.Replace(".", "") + "%"));
            string icKosul = "";
            for (int i = 0; i < sablonlar.Length; i++)
            {
                string ekKosul = SablonKosulOl(sablonlar[i]);
                if (!string.IsNullOrWhiteSpace(ekKosul))
                {
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", "(" + ekKosul + ")");
                }
            }
            if (!string.IsNullOrWhiteSpace(icKosul))
                sql = VeriErisim.KosulEkle(sql, "and", "(" + icKosul + ")");

            sql += " ORDER BY " + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD] + "," + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD] + "," + TABLOADBIRIM + "." + ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        private string SablonKosulOl(string birimKosul)
        {
            if (string.IsNullOrWhiteSpace(birimKosul)) return "";
            string icKosul = "";
            string[] birimParca = birimKosul.Split('-');
            string say = OrtakFonksiyonlar.DiziElemanAl(birimParca, 0, "");
            string birim = OrtakFonksiyonlar.DiziElemanAl(birimParca, 1, "");
            string kur = OrtakFonksiyonlar.DiziElemanAl(birimParca, 2, "");
            if (!string.IsNullOrWhiteSpace(say))
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.MUHASEBEKOD], "=", OrtakFonksiyonlar.ConvertToInt(say, 0)));
            if (!string.IsNullOrWhiteSpace(birim) && birim != "*")
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD], "=", birim));
            if (!string.IsNullOrWhiteSpace(kur) && kur != "*")
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBirim.KURKOD], "LIKE", kur.Replace(".", "") + "%"));

            return icKosul;
        }
    }
}

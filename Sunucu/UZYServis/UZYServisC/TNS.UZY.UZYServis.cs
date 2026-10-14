using System;
using System.Runtime;
using System.Runtime.Remoting;
using TNS.Veri;
using TNS.KYM;
using System.Data;
using System.Collections;

namespace TNS.UZY
{
    [Serializable()]
    public class UZYServis : MarshalByRefObject, IUZYServis
    {
        static Uzaylar uzaylar = new Uzaylar();
        //static UZYServis()
        //{
        //    //System.Threading.Monitor.Enter(uzaylar);
        //    if (uzaylar.ilkGiris)
        //    {
        //        uzaylar.UzaylariOku();
        //        uzaylar.ilkGiris = false;
        //    }
        //    //System.Threading.Monitor.Exit(uzaylar);
        //}

        public static int UzaylariYenile(Kullanici kullanan)
        {
            //if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            //{
            System.Threading.Monitor.Enter(uzaylar);
            uzaylar.UzaylariOku();
            uzaylar.ilkGiris = false;
            System.Threading.Monitor.Exit(uzaylar);
            //}
            return uzaylar.UzaySayisi();
        }

        public static TNSCollection UzaylariDondur()
        {
            if (uzaylar.UzaySayisi() == 0)
                UzaylariYenile(null);
            return uzaylar.UzaylariDondur();
        }

        public TNSCollection UzaylariListele()
        {
            return UzaylariDondur();
        }

        public int UzaylariYenile()
        {
            return UzaylariYenile();
        }

        //VeriErisim ve = null;
        public void UzayAc(object veriErisim)
        {
            //ve = VeriErisim.VeriErisimBul((VeriErisim)veriErisim, "BaglantiSatiriBUM");
        }
        public void UzayKapat()
        {
            //if (ve != null)
            //    ve.VeriErisimKapat();
        }

        private VeriErisim UzayBaglantisiAl(VeriErisim ve, bool veriErisimiKullan, Uzay u, string baglantiAd)
        {
            if (ve == null || !veriErisimiKullan)
            {
                if (string.IsNullOrEmpty(baglantiAd))
                    baglantiAd = u.veriTabaniParamAd;
                if (string.IsNullOrEmpty(baglantiAd))
                    baglantiAd = "BaglantiSatiriBUM";

                VeriErisim veIc = null;
                if (ve != null)
                {
                    if (ve.baglantiAd == baglantiAd)
                        veIc = VeriErisim.VeriErisimBul(ve, baglantiAd, null);
                    else
                        veIc = VeriErisim.VeriErisimBul(null, baglantiAd, null);
                }
                else
                    veIc = VeriErisim.VeriErisimBul(null, baglantiAd, null);
                return veIc;
            }
            else
                return ve;
        }

        private void UzayBaglantisiKapat(VeriErisim veIc, VeriErisim ve, bool veriErisimiKullan)
        {
            if (ve == null || !veriErisimiKullan)
            {
                if (veIc != null)
                    veIc.VeriErisimKapat();
            }
        }

        private IUzayHesapla GetObject(string dllAd, string nesneAd)
        {
            System.Runtime.Remoting.ObjectHandle oh = null;
            try
            {
                oh = Activator.CreateInstance(dllAd, nesneAd);
            }
            catch
            {
                oh = null;
            }

            if (oh == null)
            {
                try
                {
                    oh = Activator.CreateInstance(null, nesneAd);
                }
                catch
                {
                    oh = null;
                }
            }

            if (oh != null)
            {
                try
                {
                    return (IUzayHesapla)oh.Unwrap();
                }
                catch
                {
                    return null;
                }
            }
            else
                return null;

        }

        private double UzayDegeriDbl(Kullanici kullanan, VeriErisim ve, bool veriErisimiKullan, string uzayAdGelen, string uzayKosul, bool esitle, string alanAd)
        {
            string uzayAd;
            string baglantiAd;
            UzayAdBol(uzayAdGelen, out uzayAd, out baglantiAd);

            Uzay u = uzaylar.UzayBul(uzayAd);
            if (u == null)
                return -1;
            if (u.tur == 1)
            {
                VeriErisim veIc = UzayBaglantisiAl(ve, veriErisimiKullan, u, baglantiAd);
                if (veIc == null) return (double)-5;
                bool bolumYap = false;
                String sql = uzaylar.UzayDegeriSQL(uzayAd, veIc.baglantiAd, alanAd, uzayKosul, esitle, ref bolumYap, true);
                double sonuc = -7;
                if (!string.IsNullOrEmpty(sql))
                {
                    sonuc = veIc.TekKolonSQLDbl(sql);
                    if (bolumYap)
                        sonuc /= 1000;
                }
                UzayBaglantisiKapat(veIc, ve, veriErisimiKullan);

                return sonuc;
            }
            else
            {
                IUzayHesapla uh = GetObject(u.dllAd, u.nesneAd);
                if (uh != null)
                    return uh.UzayDegeriDbl(kullanan, ve, u, uzayKosul, esitle);
                return 0;
            }
        }

        public double UzayDegeriDbl(Kullanici kullanan, object ve, string uzayAd, string uzayKosul, bool esitle, string alanAd)
        {
            return UzayDegeriDbl(kullanan, (VeriErisim)ve, true, uzayAd, uzayKosul, esitle, alanAd);
        }

        public double UzayDegeriDbl(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle, string alanAd)
        {
            return UzayDegeriDbl(kullanan, null, false, uzayAd, uzayKosul, esitle, alanAd);
        }

        public double UzayDegeriDbl(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle)
        {
            return UzayDegeriDbl(kullanan, null, false, uzayAd, uzayKosul, esitle, null);
        }

        private string UzayDegeriStr(Kullanici kullanan, VeriErisim ve, bool veriErisimiKullan, string uzayAdGelen, string uzayKosul, bool esitle, string alanAd)
        {
            string uzayAd;
            string baglantiAd;
            UzayAdBol(uzayAdGelen, out uzayAd, out baglantiAd);
            Uzay u = uzaylar.UzayBul(uzayAd);
            if (u == null)
                return $"{uzayAd} isimli uzay bulunamadı.";
            if (u.tur == 1)
            {
                VeriErisim veIc = UzayBaglantisiAl(ve, veriErisimiKullan, u, baglantiAd);
                if (veIc == null) return "-5";
                bool bolumYap = false;
                string sonuc = "";
                String sql = uzaylar.UzayDegeriSQL(uzayAd, veIc.baglantiAd, alanAd, uzayKosul, esitle, ref bolumYap, true);
                if (!string.IsNullOrEmpty(sql))
                    sonuc = veIc.TekKolonSQLStr(sql);
                UzayBaglantisiKapat(veIc, ve, veriErisimiKullan);
                return sonuc;
            }
            else
            {
                IUzayHesapla uh = GetObject(u.dllAd, u.nesneAd);
                if (uh != null)
                    return uh.UzayDegeriStr(kullanan, ve, u, uzayKosul, esitle);
                return "";
            }
        }

        public string UzayDegeriStr(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle)
        {
            return UzayDegeriStr(kullanan, null, false, uzayAd, uzayKosul, esitle, null);
        }

        public string UzayDegeriStr(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle, string alanAd)
        {
            return UzayDegeriStr(kullanan, null, false, uzayAd, uzayKosul, esitle, alanAd);
        }

        public string UzayDegeriStr(Kullanici kullanan, object veriErisim, string uzayAd, string uzayKosul, bool esitle, string alanAd)
        {
            return UzayDegeriStr(kullanan, (VeriErisim)veriErisim, true, uzayAd, uzayKosul, esitle, alanAd);
        }

        private TNSDateTime UzayDegeriTarih(Kullanici kullanan, VeriErisim ve, bool veriErisimiKullan, string uzayAdGelen, string uzayKosul, bool esitle, string alanAd)
        {
            string uzayAd;
            string baglantiAd;
            UzayAdBol(uzayAdGelen, out uzayAd, out baglantiAd);
            Uzay u = uzaylar.UzayBul(uzayAd);
            if (u == null)
                return null;
            if (u.tur == 1)
            {
                VeriErisim veIc = UzayBaglantisiAl(ve, veriErisimiKullan, u, baglantiAd);
                if (veIc == null) return new TNSDateTime("");
                bool bolumYap = false;
                TNSDateTime sonuc = null;
                String sql = uzaylar.UzayDegeriSQL(uzayAd, veIc.baglantiAd, alanAd, uzayKosul, esitle, ref bolumYap, true);
                if (!string.IsNullOrEmpty(sql))
                    sonuc = veIc.TekKolonSQLDate(sql);
                UzayBaglantisiKapat(veIc, ve, veriErisimiKullan);
                return sonuc;
            }
            else
            {
                IUzayHesapla uh = GetObject(u.dllAd, u.nesneAd);
                if (uh != null)
                    return uh.UzayDegeriTarih(kullanan, ve, u, uzayKosul, esitle);
                return null;
            }
        }

        public TNSDateTime UzayDegeriTarih(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle)
        {
            return UzayDegeriTarih(kullanan, null, false, uzayAd, uzayKosul, esitle, null);
        }

        public TNSDateTime UzayDegeriTarih(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle, string alanAd)
        {
            return UzayDegeriTarih(kullanan, null, false, uzayAd, uzayKosul, esitle, alanAd);
        }

        public TNSDateTime UzayDegeriTarih(Kullanici kullanan, object veriErisim, string uzayAd, string uzayKosul, bool esitle, string alanAd)
        {
            return UzayDegeriTarih(kullanan, (VeriErisim)veriErisim, true, uzayAd, uzayKosul, esitle, alanAd);
        }

        public ObjectArray UzayDegeriToplu(Kullanici kullanan, TNSCollection uzaySorgulari)//UzaySorguBilgi
        {
            ObjectArray s = new ObjectArray();
            UzaySorguBilgi usb;
            if (uzaySorgulari.Count > 0)
            {
                Uzay u = uzaylar.UzayBul(((UzaySorguBilgi)uzaySorgulari[0]).uzayAd);
                if (u == null)
                    return null;

                VeriErisim veIc = UzayBaglantisiAl(null, false, u, null);

                for (int i = 0; i < uzaySorgulari.Count; i++)
                {
                    usb = (UzaySorguBilgi)uzaySorgulari[i];
                    u = uzaylar.UzayBul(usb.uzayAd);
                    if (u != null)
                    {
                        if (u.tip == "N")
                            s.Ekle(UzayDegeriDbl(kullanan, veIc, false, usb.uzayAd, usb.kosul, usb.esitle, usb.alanAd));
                        else
                            s.Ekle(UzayDegeriStr(kullanan, veIc, false, usb.uzayAd, usb.kosul, usb.esitle, usb.alanAd));
                    }
                    else
                        s.Ekle("");
                }

                veIc.VeriErisimKapat();
            }
            return s;
        }

        public string KosulOlustur(string tabloOnEk, string vtAlanKod, string maske, string kodSorgu, bool esitle, bool kurumLimitiEkle)
        {
            return UzayKosul.KosulOlustur(tabloOnEk, vtAlanKod, maske, kodSorgu, esitle, kurumLimitiEkle, false);
        }

        private static void UzayAdBol(string gelenUzayAd, out string uzayAd, out string baglantiAd)
        {
            uzayAd = "";
            baglantiAd = "";
            string[] uzayBilgi = gelenUzayAd.Split('@');
            if (uzayBilgi == null)
                return;
            else if (uzayBilgi.Length == 1)
            {
                uzayAd = uzayBilgi[0];
                baglantiAd = "";
            }
            else if (uzayBilgi.Length == 2)
            {
                uzayAd = uzayBilgi[0];
                baglantiAd = uzayBilgi[1];
            }
        }

        public string UzayKosulStr(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle, string alanAd)
        {
            string uzayAdIc;
            string baglantiAdIc;
            bool bolumYap = false;

            UzayAdBol(uzayAd, out uzayAdIc, out baglantiAdIc);

            return uzaylar.UzayDegeriSQL(uzayAdIc, baglantiAdIc, alanAd, uzayKosul, esitle, ref bolumYap, true);
        }

        public System.Data.DataTable UzayDataTableDondur(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle, string alanAd, string[] ekAlanlar)
        {
            string[] uzayKosullar = new string[1];
            uzayKosullar[0] = uzayKosul;
            return UzayDataTableDondur(kullanan, uzayAd, uzayKosullar, esitle, alanAd, ekAlanlar);
        }

        public System.Data.DataTable UzayDataTableDondur(Kullanici kullanan, string uzayAd, string[] uzayKosullar, bool esitle, string alanAd, string[] ekAlanlar)
        {
            string uzayAdIc;
            string baglantiAdIc;

            UzayAdBol(uzayAd, out uzayAdIc, out baglantiAdIc);

            Uzay u = uzaylar.UzayBul(uzayAdIc);
            if (u == null) return null;
            bool bolumYap = false;
            string eksikAlanlar;
            string sqlKosul = "";
            if (!string.IsNullOrEmpty(u.where))
                sqlKosul = "(" + u.where + ") ";

            for (int i = 0; i < uzayKosullar.Length; i++)
            {
                string icSqlKosul = uzaylar.SQLKosulOl(u, baglantiAdIc, uzayKosullar[i], esitle, ref bolumYap, out eksikAlanlar);

                if (!string.IsNullOrEmpty(icSqlKosul))
                {
                    if (!string.IsNullOrEmpty(sqlKosul)) sqlKosul += " AND ";
                    sqlKosul += icSqlKosul;
                }
            }


            if (sqlKosul != "") sqlKosul = " WHERE " + sqlKosul;

            if (alanAd == null || alanAd == "")
                alanAd = u.secAlan;

            string ekAlanlarStr = "";
            if (ekAlanlar != null && ekAlanlar.Length > 0)
            {
                for (int i = 0; i < ekAlanlar.Length; i++)
                {
                    string ekAlanAd = "";
                    string kirilimSayStr = "";
                    TNS.OrtakFonksiyonlar.StringiIkiyeBol(ekAlanlar[i], ":", ref ekAlanAd, ref kirilimSayStr);
                    UzayAlan ua = u.AlanBul(ekAlanAd);
                    if (ua != null)
                    {
                        ekAlanAd = ua.ad;
                        if (!string.IsNullOrEmpty(kirilimSayStr))
                        {
                            int kirilimSay = OrtakFonksiyonlar.ConvertToInt(kirilimSayStr, -1);
                            if (kirilimSay > 0)
                            {
                                string maske = ua.MaskeBul("0", "");
                                if (maske.StartsWith("9"))
                                {
                                    int uz = 0;
                                    string[] maskeParca = maske.Split('.');
                                    for (int mp = 0; mp < maskeParca.Length && mp < kirilimSay; mp++)
                                    {
                                        uz += maskeParca[mp].Length;
                                    }

                                    ekAlanAd = VeriErisim.IslecBul("SUBSTRING") + "(" + ekAlanAd + ", 1, " + uz.ToString() + ")";
                                }
                            }
                        }
                    }
                    ekAlanlarStr += ekAlanAd;

                    if (i < ekAlanlar.Length - 1)
                        ekAlanlarStr += ",";
                }
            }
            string sql = "SELECT ";
            if (!string.IsNullOrEmpty(ekAlanlarStr))
                sql = sql + ekAlanlarStr + ", ";
            sql = sql + alanAd + " FROM " + u.from + " " + sqlKosul;
            if (!string.IsNullOrEmpty(ekAlanlarStr))
            {
                sql += " group by " + ekAlanlarStr;
                sql += " order by " + ekAlanlarStr;
            }
            VeriErisim veIc = UzayBaglantisiAl(null, false, u, baglantiAdIc);
            if (veIc == null) return null;

            System.Data.DataTable dt = null;

            System.Data.IDataReader dr = veIc.DataReaderDondur(sql);
            if (dr != null)
            {
                bool ilk = true;
                dt = new System.Data.DataTable();
                while (dr.Read())
                {
                    if (ilk)
                    {
                        ilk = false;
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            string ad = dr.GetName(i);
                            dt.Columns.Add(ad, typeof(object));
                        }
                    }
                    System.Data.DataRow row = dt.NewRow();

                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        object o = dr.GetValue(i);
                        row[i] = o;
                    }

                    dt.Rows.Add(row);
                }
                dr.Close();
            }

            UzayBaglantisiKapat(veIc, null, false);
            return dt;
        }

        private static bool SayfayaGit(IDataReader dr, int sayfa, int sayfaBoy)
        {
            if (sayfaBoy <= 0) return true;
            if (sayfa <= 1) return true;
            int atlanacak = (sayfa - 1) * sayfaBoy;
            while (dr.Read())
            {
                atlanacak--;
                if (atlanacak <= 0) return true;
            }
            return false;
        }

        public System.Data.DataTable SorguDondur(string sql, string baglantiStr, int sayfa, int sayfaBoy)
        {
            if (string.IsNullOrWhiteSpace(sql)) return null;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, baglantiStr, null);
            System.Data.DataTable dt = null;
            IDataReader dr = ve.DataReaderDondur(sql);
            if (dr != null)
            {
                try
                {
                    if (dr != null)
                    {
                        bool ilk = true;
                        dt = new System.Data.DataTable();
                        int eklenen = 0;
                        if (SayfayaGit(dr, sayfa, sayfaBoy))
                        {
                            while (dr.Read())
                            {
                                if (ilk)
                                {
                                    ilk = false;
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        string ad = dr.GetName(i);
                                        if (dt.Columns.Contains(ad))
                                            ad = ad + " (" + i.ToString() + ")";
                                        dt.Columns.Add(ad, typeof(object));
                                    }
                                }
                                System.Data.DataRow row = dt.NewRow();

                                for (int i = 0; i < dr.FieldCount; i++)
                                {
                                    object o = dr.GetValue(i);
                                    row[i] = o;
                                }

                                dt.Rows.Add(row);
                                eklenen++;
                                if (eklenen >= sayfaBoy && sayfaBoy > 0)
                                    break;
                            }
                        }
                        dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    if (dr != null) dr.Close();
                    ve.hata = ex.Message;
                }
            }
            ve.VeriErisimKapat();
            if (!string.IsNullOrEmpty(ve.hata)) throw new Exception(ve.hata);
            return dt;
        }

        public Sonuc SQLCalistir(Kullanici kullanan, string baglantiStr, string[] sqller)
        {
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, baglantiStr, TNS.KYM.Arac.KimlikBul(kullanan));

            if (sqller != null)
            {
                ve.BeginTransaction();
                foreach (string item in sqller)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        int kayitSay = ve.ExecuteNonquery(item);
                        if (!string.IsNullOrWhiteSpace(ve.hata)) break;
                    }
                }
                s.DegerAta(ve.hata);
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }


        public VeritabaniIndex[] VeritabaniIndexListele(Kullanici kullanan, VeritabaniIndex form, string baglantiAd)
        {
            try
            {
                return UZYVeritabani.IndexListele(kullanan, form, baglantiAd);
            }
            catch (Exception e)
            {
                return new VeritabaniIndex[1] { new VeritabaniIndex() { objectId = -2, tabloAd = e.Message } };
            }
        }

        public Sonuc VeritabaniIndexIslem(Kullanici kullanan, VeritabaniIndex[] liste, ENUMVeritabaniIndexIslem islem, string baglantiAd)
        {
            try
            {
                return UZYVeritabani.IndexIslem(kullanan, liste, islem, baglantiAd);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Hashtable VeritabaniBaglantiSatiriListele(Kullanici kullanan)
        {
            return UZYVeritabani.BaglantiSatiriListele(kullanan);
        }


        public System.Collections.SortedList SorguBirlestir(string[] sql, string[] baglantiStr, int[] anahtarAlanlar)
        {
            SortedList ht = new SortedList();
            int alanSay = 0;
            for (int i = 0; i < sql.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(sql[i]))
                {
                    int sorguAlanSay = SorguBirlestir(ht, sql[i], baglantiStr[i], anahtarAlanlar, alanSay);
                    alanSay += sorguAlanSay;
                }
            }
            return ht;
        }

        private static int SorguBirlestir(SortedList ht, string sql, string baglantiStr, int[] anahtarAlanlar, int yoksaBosEkleSayi)
        {
            string hata = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(null, baglantiStr, "SorguBirleştirme");
            IDataReader dr = ve.DataReaderDondur(sql);
            int sorguAlanSay = 0;
            if (dr != null)
            {
                System.Collections.Generic.List<int> veriAlanlar = new System.Collections.Generic.List<int>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (AnahtardaMi(i, anahtarAlanlar))
                        veriAlanlar.Add(i);
                }
                sorguAlanSay = veriAlanlar.Count;
                while (dr.Read())
                {
                    HTArray alAnahtar = new HTArray();
                    for (int i = 0; i < anahtarAlanlar.Length; i++)
                    {
                        alAnahtar.Add(dr.GetValue(anahtarAlanlar[i]));
                    }

                    HTArray eskiVeriler = null;
                    try
                    {
                        eskiVeriler = (HTArray)ht[alAnahtar];
                    }
                    catch
                    {
                        eskiVeriler = null;
                    }

                    if (eskiVeriler == null)
                    {
                        eskiVeriler = new HTArray();
                    }

                    for (int i = eskiVeriler.ObjeSayisi; i < yoksaBosEkleSayi; i++)
                    {
                        eskiVeriler.Add(null);
                    }

                    for (int i = 0; i < sorguAlanSay; i++)
                    {
                        eskiVeriler.Add(dr.GetValue(veriAlanlar[i]));
                    }
                    ht[alAnahtar] = eskiVeriler;
                }
                dr.Close();
            }
            else
                hata = ve.hata;
            ve.VeriErisimKapat();
            if (!string.IsNullOrWhiteSpace(hata))
            {
                throw new Exception(sql + " komutu çalıştırılırken hata oluştu: " + hata);
            }
            return sorguAlanSay;
        }

        private static bool AnahtardaMi(int kolNo, int[] anahtarAlanlar)
        {
            for (int i = 0; i < anahtarAlanlar.Length; i++)
            {
                if (kolNo == anahtarAlanlar[i]) return false;
            }
            return true;
        }
    }
}

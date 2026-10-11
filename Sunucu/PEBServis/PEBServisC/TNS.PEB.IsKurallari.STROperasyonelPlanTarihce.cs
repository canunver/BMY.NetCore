using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using System.Xml;
using System.Collections;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Operasyonel plan modülünde yapılan değişikliklerin tutulması
    /// </summary>
    public class STROperasyonelPlanTarihce
    {
        private XmlDocument doc;
        private XmlAttribute e, y, yAd, eAd;
        private XmlAttribute aciklama1, aciklama2;
        public XmlNode node { get; protected set; }
        public bool Degisen { get; protected set; }
        public ENUMTARIHCE Alan { get; protected set; }
        public Object Yeni { get; protected set; }
        public Object Eski { get; protected set; }
        public ENUMTARIHCEISLEMTIP islem { get; protected set; }
        public IsGorevDetayX IsGorevDetay { get; set; }
        public IsGorevZamanlamaX IsGorevZamanlama { get; set; }
        public IsGorevSorumluX IsGorevSorumlu { get; set; }
        public IsGorevPerformansGostergeX IsGorevPerformansGosterge { get; set; }
        public AlanKey Key { get; protected set; }
        public AlanKeyAd KeyAd { get; protected set; }
        public STROperasyonelPlanTarihce()
        {
            this.Genel();
        }

        public STROperasyonelPlanTarihce(Object yeni, Object eski, ENUMTARIHCE alan, ENUMTARIHCEISLEMTIP islem)
        {
            this.Yeni = yeni;
            this.Eski = eski;
            this.Alan = alan;
            this.islem = islem;

            this.Genel();
        }
        private void Genel()
        {
            this.doc = new XmlDocument();
            this.node = doc.CreateNode(XmlNodeType.Element, "personel", null);
            this.y = doc.CreateAttribute("y"); this.yAd = doc.CreateAttribute("yAd");
            this.e = doc.CreateAttribute("e"); this.eAd = doc.CreateAttribute("eAd");

            this.Key = new AlanKey();
            this.KeyAd = new AlanKeyAd();

            this.IsGorevDetay = new IsGorevDetayX();
            this.IsGorevZamanlama = new IsGorevZamanlamaX();
            this.IsGorevSorumlu = new IsGorevSorumluX();
            this.IsGorevPerformansGosterge = new IsGorevPerformansGostergeX();
        }
        public class AlanKey
        {
            public string IsGorevDetay { get; protected set; }
            public string IsGorevZamanlama { get; protected set; }
            public string IsGorevSorumlu { get; protected set; }
            public string IsGorevPerformansGosterge { get; protected set; }

            public AlanKey()
            {
                this.IsGorevDetay = "isGorevDetay";
                this.IsGorevZamanlama = "isGorevZamanlama";
                this.IsGorevSorumlu = "isGorevSorumlu";
                this.IsGorevPerformansGosterge = "isGorevPerformansGosterge";
            }
        }
        public class AlanKeyAd : AlanKey
        {
            public AlanKeyAd()
            {
                base.IsGorevDetay = "İş Görev Detay";
                base.IsGorevZamanlama = "Zamanlama Bilgileri";
                base.IsGorevSorumlu = "Sorumlu";
                base.IsGorevPerformansGosterge = "Performans Gösterge";
            }
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Object yeni, Object eski, ENUMTARIHCE alan,  ENUMTARIHCEISLEMTIP islem, IsGorev isGorev)
        {
            Sonuc sonuc = new Sonuc();

            STROperasyonelPlanTarihce tarihce = new STROperasyonelPlanTarihce(yeni, eski, alan, islem);
            IsGorevTarihce pT = new IsGorevTarihce();

            pT.degisiklik = tarihce.DegisenleriGetir();

            if (tarihce.Degisen)
            {
               
                pT.kullanici = kullanan.mernis;
                pT.tarih = new TNSDateTime(DateTime.Now);
                IsGorev degerler = (IsGorev)isGorev;
                pT.yil = degerler.yil;
                pT.kurKod = degerler.orgKod.ToString();
                pT.kod = degerler.isGorevKod;

                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
                ve.BeginTransaction();

                TABIsGorev tab = new TABIsGorev(ve);
                sonuc = tab.KaydetTarihce(pT);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "Kayıt işlemi başarıyla gerçekleştirildi.";
                }
                else
                    ve.Rollback();

                ve.VeriErisimKapat();
            }
            else
            {
                sonuc.hataStr = "Değişiklik bulunamadı";
                sonuc.islemSonuc = false;
            }
            return sonuc;
        }



        public string DegisenleriGetir()
        {
            Hashtable htDegisenDegerler = new Hashtable();
            string nodeAlanName = "";

            switch (Alan)
            {
                case ENUMTARIHCE.ISGOREVDETAY:
                    nodeAlanName = this.Key.IsGorevDetay;
                    htDegisenDegerler = this.IsGorevDetay.DegisenDegerleriGetir((IsGorev)Yeni, (IsGorev)Eski, islem);
                    break;

                case ENUMTARIHCE.ISGOREVZAMANLAMA:
                    nodeAlanName = this.Key.IsGorevZamanlama;
                    htDegisenDegerler = this.IsGorevZamanlama.DegisenDegerleriGetir((IsGorevZamanlama)Yeni,(IsGorevZamanlama)Eski,islem);
                    break;
                case ENUMTARIHCE.ISGOREVSORUMLULAR:
                    nodeAlanName = this.Key.IsGorevSorumlu;
                    htDegisenDegerler = this.IsGorevSorumlu.DegisenDegerleriGetir((IsGorevSorumlu)Yeni, (IsGorevSorumlu)Eski, islem);
                    break;
                case ENUMTARIHCE.ISGOREVPERFORMANSGOSTERGE:
                    nodeAlanName = this.Key.IsGorevPerformansGosterge;
                    htDegisenDegerler = this.IsGorevPerformansGosterge.DegisenDegerleriGetir((IsGorevGosterge)Yeni, (IsGorevGosterge)Eski, islem);
                    break;

            }

            if (((ArrayList)htDegisenDegerler[TagList.Liste]).Count > 0)
            {
                XmlNode nodeAlan = doc.CreateElement(nodeAlanName);
                Object bilgi = (Object)htDegisenDegerler[TagList.Bilgi];
                if (bilgi != null)
                {
                    object[] alanAttributes = (object[])bilgi;
                    int index = 0;
                    foreach (string[] item in alanAttributes)
                    {
                        if (index == 0)
                        {
                            this.aciklama1 = doc.CreateAttribute(item[0]);
                            this.aciklama1.Value = item[1];
                            nodeAlan.Attributes.Append(aciklama1);
                        }
                        else if (index == 1)
                        {
                            this.aciklama2 = doc.CreateAttribute(item[0]);
                            this.aciklama2.Value = item[1];
                            nodeAlan.Attributes.Append(aciklama2);
                        }
                        index++;
                    }
                }
                node.AppendChild(nodeAlan);
                foreach (object[] deger in (ArrayList)htDegisenDegerler[TagList.Liste])
                {
                    XmlNode nodeD = doc.CreateElement((string)deger[0]);
                    {
                        string yDeger = "", eDeger = "", yAdDeger = "", eAdDeger = "";

                        Type t= deger[1].GetType();
                        //Yeni
                        if (deger[1].GetType() == typeof(string[]))
                        {
                            string[] yeni = (string[])deger[1];
                            yDeger = yeni[0]; yAdDeger = yeni[1];
                        }
                        else if (deger[1].GetType() != typeof(string))
                        {
                            yDeger = deger[1].ToString();
                        }
                        else
                            yDeger = (string)deger[1];

                        y.Value = yDeger; nodeD.Attributes.Append(y);

                        //Eski
                        if (deger[2].GetType() == typeof(string[]))
                        {
                            string[] eski = (string[])deger[2];
                            eDeger = eski[0]; eAdDeger = eski[1];
                        }
                        else if (deger[2].GetType() != typeof(string))
                        {
                            yDeger = deger[2].ToString();
                        }

                        else eDeger = (string)deger[2];

                        e.Value = eDeger;

                        nodeD.Attributes.Append(e);

                        if (yAdDeger != "" || eAdDeger != "")
                        {
                            yAd.Value = yAdDeger; nodeD.Attributes.Append(yAd);
                            eAd.Value = eAdDeger; nodeD.Attributes.Append(eAd);
                        }
                    }
                    nodeAlan.AppendChild(nodeD.Clone());
                }

                this.Degisen = true;
            }
            else
                this.Degisen = false;

            return this.node.OuterXml.ToString();
        }





        public static string GetKodAd(int kod, EnumKodAd e, ObjectArray list = null)
        {
            if (kod == 0) return "";
            switch (e)
            {              
                case EnumKodAd.TekrarSikligi:
                    switch (kod)
                    {
                        case 1: return "Bir Kez";
                        case 2: return "Günlük";
                        case 3: return "Haftalık";
                        case 4: return "Aylık";
                        case 5: return "Üç Aylık";
                        case 6: return "Altı Aylık";
                        case 7: return "Yıllık";
                    }
                    break;  
            }
            return "";
        }
        public enum EnumKodAd
        {
            TekrarSikligi
        }
        public class TagList
        {
            public static string Liste { get { return "liste"; } }
            public static string Bilgi { get { return "bilgi"; } }
            public static string YeniKayit { get { return "YeniKayit"; } }
            public static string Silindi { get { return "Silindi"; } }
            public static string KayitNo { get { return "SiraNo"; } }            
        }
        #region IsgorevDetay
        public class IsGorevDetayX
        {
            public IsGorevDetayKey Key { get; protected set; }
            public IsGorevDetayKeyAd KeyAd { get; protected set; }

            public IsGorevDetayX()
            {
                this.Key = new IsGorevDetayKey();
                this.KeyAd = new IsGorevDetayKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(IsGorev yeni, IsGorev eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();

                if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.isGorevKod != eski.isGorevKod) liste.Add(new object[] { this.Key.IsGorevKod, yeni.isGorevKod, eski.isGorevKod });

                if (yeni.yil != eski.yil) liste.Add(new object[] { this.Key.Yil, yeni.yil, eski.yil });
                if (yeni.orgKod != eski.orgKod) liste.Add(new object[] { this.Key.KurKod, yeni.orgKod, eski.orgKod });
                if (yeni.isGorevAd != eski.isGorevAd) liste.Add(new string[] { this.Key.IsGorevAd, yeni.isGorevAd, eski.isGorevAd });
                if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.Aciklama, yeni.aciklama, eski.aciklama });
                if (yeni.hedef.ToString() != eski.hedef) liste.Add(new string[] { this.Key.Hedef, yeni.hedef, eski.hedef });
                if (yeni.tekrarSikligi != eski.tekrarSikligi) liste.Add(new object[] { this.Key.TekrarSikligi, new string[] { yeni.tekrarSikligi.ToString(), GetKodAd(yeni.tekrarSikligi, EnumKodAd.TekrarSikligi) }, new string[] { eski.tekrarSikligi.ToString(), GetKodAd(eski.tekrarSikligi, EnumKodAd.TekrarSikligi) } });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                if (islem == ENUMTARIHCEISLEMTIP.KAYDET) table.Add(TagList.Bilgi, new object[] { new string[] { TagList.YeniKayit, "" } });

                return table;
            }

        }
        public class IsGorevDetayKey
        {
            public string Yil { get; protected set; }
            public string KurKod { get; protected set; }
            public string IsGorevKod { get; protected set; }
            public string AltFaaliyetKod { get; protected set; }
            public string IsGorevAd { get; protected set; }
            public string Aciklama { get; protected set; }
            public string Hedef { get; protected set; }
            public string TekrarSikligi { get; protected set; }

            public IsGorevDetayKey()
            {
                Yil = "Yil";
                KurKod = "KurKod";
                IsGorevKod = "IsGorevKod";
                AltFaaliyetKod = "AltFaaliyetKod";
                IsGorevAd = "IsGorevAd";
                Aciklama = "Aciklama";
                Hedef = "Hedef";
                TekrarSikligi = "TekrarSikligi";
            }
        }
        public class IsGorevDetayKeyAd : IsGorevDetayKey
        {
            public IsGorevDetayKeyAd()
            {
                base.Yil = "Yıl:";
                base.KurKod = "Org. Birim:";
                base.IsGorevKod = "İş Görev Kod:";
                base.AltFaaliyetKod = "Alt Faaliyet Kodu";
                base.IsGorevAd = "İş/Görev:";
                base.Aciklama = "Süreç/Açıklama:";
                base.Hedef = "Özel Hedef (İş/Görevin Yapılma Amacı):";
                base.TekrarSikligi = "Tekrar Sıklığı";

            }
        }
        #endregion

        #region IsGorevZamanlama

        public class IsGorevZamanlamaX
        {
            public IsGorevZamanlamaKey Key { get; protected set; }
            public IsGorevZamanlamaKeyAd KeyAd { get; protected set; }

            public IsGorevZamanlamaX()
            {
                this.Key = new IsGorevZamanlamaKey();
                this.KeyAd = new IsGorevZamanlamaKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(IsGorevZamanlama yeni, IsGorevZamanlama eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();

                if (yeni.siraNo != eski.siraNo) liste.Add(new object[] { this.Key.siraNo, yeni.siraNo, eski.siraNo });
                if (yeni.basTarih != eski.basTarih) liste.Add(new object[] { this.Key.basTarih, yeni.basTarih, eski.basTarih });
                if (yeni.bitTarih != eski.bitTarih) liste.Add(new object[] { this.Key.bitTarih, yeni.bitTarih, eski.bitTarih });
                if (yeni.basAciklama != eski.basAciklama) liste.Add(new string[] { this.Key.basAciklama, yeni.basAciklama, eski.basAciklama });
                if (yeni.bitAciklama != eski.bitAciklama) liste.Add(new string[] { this.Key.bitAciklama, yeni.bitAciklama, eski.bitAciklama });
                if (yeni.sure != eski.sure) liste.Add(new string[] { this.Key.sure, yeni.sure, eski.sure });
                if (yeni.yer != eski.yer) liste.Add(new string[] { this.Key.yer, yeni.yer, eski.yer });
                if (yeni.basTarih2 != eski.basTarih2) liste.Add(new object[] { this.Key.basTarih2, yeni.basTarih2, eski.basTarih2 });
                if (yeni.bitTarih2 != eski.bitTarih2) liste.Add(new object[] { this.Key.bitTarih2, yeni.bitTarih2, eski.bitTarih2 });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                if (islem == ENUMTARIHCEISLEMTIP.KAYDET) table.Add(TagList.Bilgi, new object[] { new string[] { TagList.YeniKayit, "" } });

                return table;
            }

        }
        public class IsGorevZamanlamaKey
        {
            public string siraNo { get; protected set; }
            public string basTarih { get; protected set; }
            public string bitTarih { get; protected set; }
            public string basAciklama { get; protected set; }
            public string bitAciklama { get; protected set; }
            public string sure { get; protected set; }
            public string yer { get; protected set; }
            public string basTarih2 { get; protected set; }
            public string bitTarih2 { get; protected set; }

            public IsGorevZamanlamaKey()
            {
                siraNo = "siraNo";
                basTarih = "basTarih";
                bitTarih = "bitTarih";
                basAciklama = "basAciklama";
                bitAciklama = "bitAciklama";
                sure = "sure";
                yer = "yer";
                basTarih2 = "basTarih2";
                bitTarih2 = "bitTarih2";
            }
        }
        public class IsGorevZamanlamaKeyAd : IsGorevZamanlamaKey
        {
            public IsGorevZamanlamaKeyAd()
            {
                base.siraNo = "*";
                base.basTarih = "Başlama Tarihi";
                base.bitTarih = "Bitiş Tarihi";
                base.basAciklama = "Başlama Açıklama";
                base.bitAciklama = "Bitiş Açıklama";
                base.sure = "Sure";
                base.yer = "Yer";
                base.basTarih2 = "Başlama Tarihi 2";
                base.bitTarih2 = "Bitiş Tarihi 2";
            }
        }

        #endregion

        #region IsGorevSorumlu
        public class IsGorevSorumluX
        {
            public IsGorevSorumluKey Key { get; protected set; }
            public IsGorevSorumluKeyAd KeyAd { get; protected set; }

            public IsGorevSorumluX()
            {
                this.Key = new IsGorevSorumluKey();
                this.KeyAd = new IsGorevSorumluKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(IsGorevSorumlu yeni, IsGorevSorumlu eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();

                if (yeni.siraNo != eski.siraNo) liste.Add(new object[] { this.Key.siraNo, yeni.siraNo, eski.siraNo });
                if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.tcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.ad != eski.ad) liste.Add(new string[] { this.Key.ad, yeni.ad, eski.ad });
                if (yeni.soyad != eski.soyad) liste.Add(new string[] { this.Key.soyad, yeni.soyad, eski.soyad });
                if (yeni.gorev != eski.gorev) liste.Add(new string[] { this.Key.gorev, yeni.gorev, eski.gorev });
                if (yeni.sorumlu != eski.sorumlu) liste.Add(new object[] { this.Key.sorumlu, yeni.sorumlu, eski.sorumlu });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                if (islem == ENUMTARIHCEISLEMTIP.KAYDET) table.Add(TagList.Bilgi, new object[] { new string[] { TagList.YeniKayit, "" } });

                return table;
            }

        }
        public class IsGorevSorumluKey
        {
            public string siraNo { get; protected set; }
            public string tcKimlikNo { get; protected set; }
            public string ad { get; protected set; }
            public string soyad { get; protected set; }
            public string gorev { get; protected set; }
            public string sorumlu { get; protected set; }

            public IsGorevSorumluKey()
            {
                siraNo = "siraNo";
                tcKimlikNo = "tcKimlikNo";
                ad = "ad";
                soyad = "soyad";
                gorev = "gorev";
                sorumlu = "sorumlu";
            }
        }
        public class IsGorevSorumluKeyAd : IsGorevSorumluKey
        {
            public IsGorevSorumluKeyAd()
            {
                base.siraNo = "*";
                base.tcKimlikNo = "T.C. Kimlik No";
                base.ad = "Ad";
                base.soyad = "Soyad";
                base.gorev = "Görev";
                base.sorumlu = "Sorumlu";
            }
        }

        #endregion

        #region IsGorevPerformansGosterge

        public class IsGorevPerformansGostergeX
        {
            public IsGorevPerformansGostergeKey Key { get; protected set; }
            public IsGorevPerformansGostergeKeyAd KeyAd { get; protected set; }

            public IsGorevPerformansGostergeX()
            {
                this.Key = new IsGorevPerformansGostergeKey();
                this.KeyAd = new IsGorevPerformansGostergeKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(IsGorevGosterge yeni, IsGorevGosterge eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();

                if (yeni.siraNo != eski.siraNo) liste.Add(new object[] { this.Key.siraNo, yeni.siraNo, eski.siraNo });
                if (yeni.gosterge.gostergeKod != eski.gosterge.gostergeKod) liste.Add(new object[] { this.Key.gostergeKod, yeni.gostergeKod, eski.gostergeKod });
                if (yeni.hedeflenen != eski.hedeflenen) liste.Add(new object[] { this.Key.hedeflenen, yeni.hedeflenen, eski.hedeflenen });
                for (int i = 0; i < eski.gerceklesen.Length; i++)
                {
                    if (yeni.gerceklesen[i] != eski.gerceklesen[i]) liste.Add(new object[] { this.Key.gerceklesen, yeni.gerceklesen[i], eski.gerceklesen[i] });

                }      
                if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.aciklama, yeni.aciklama, eski.aciklama });
                

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                if (islem == ENUMTARIHCEISLEMTIP.KAYDET) table.Add(TagList.Bilgi, new object[] { new string[] { TagList.YeniKayit, "" } });

                return table;
            }

        }
        public class IsGorevPerformansGostergeKey
        {
            public string siraNo { get; protected set; }
            public string gostergeKod { get; protected set; }
            public string hedeflenen { get; protected set; }
            public string gerceklesen { get; protected set; }
            public string aciklama { get; protected set; }

            public IsGorevPerformansGostergeKey()
            {
                siraNo = "siraNo";
                gostergeKod = "gosterge";
                hedeflenen = "hedeflenen";
                gerceklesen = "gerceklesen";
                aciklama = "aciklama";
            }
        }
        public class IsGorevPerformansGostergeKeyAd : IsGorevPerformansGostergeKey
        {
            public IsGorevPerformansGostergeKeyAd()
            {
                base.siraNo = "Sıra No";
                base.gostergeKod = "Gösterge Kod";
                base.hedeflenen = "Hedeflenen";
                base.gerceklesen = "Gerçekleşen";
                base.aciklama = "Açıklamma";
            }
        }




        #endregion

    }
}
         
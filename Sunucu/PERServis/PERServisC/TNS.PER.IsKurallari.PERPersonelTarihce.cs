using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using System.Collections;
using System.Xml;

namespace TNS.PER.IsKurallari
{
    public class PERPersonelTarihce
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

        public AlanKey Key { get; protected set; }
        public AlanKeyAd KeyAd { get; protected set; }

        public KisiselX Kisisel { get; set; }
        public AdresX Adres { get; set; }
        public AileX Aile { get; set; }
        public OgrenimX Ogrenim { get; set; }
        public DilX Dil { get; set; }
        public IzinX Izin { get; set; }
        public MemuriyetX Memuriyet { get; set; }
        public KadroX Kadro { get; set; }
        public KursX Kurs { get; set; }
        public CezaOdulX CezaOdul { get; set; }
        public MalBildirimiX MalBildirimi { get; set; }
        public IzinDevirX IzinDevir { get; set; }

        public PERPersonelTarihce()
        {
            this.Genel();
        }

        public PERPersonelTarihce(Object yeni, Object eski, ENUMTARIHCE alan, ENUMTARIHCEISLEMTIP islem)
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

            this.Kisisel = new KisiselX();
            this.Adres = new AdresX();
            this.Aile = new AileX();
            this.Ogrenim = new OgrenimX();
            this.Dil = new DilX();
            this.Izin = new IzinX();
            this.Memuriyet = new MemuriyetX();
            this.Kadro = new KadroX();
            this.Kurs = new KursX();
            this.CezaOdul = new CezaOdulX();
            this.MalBildirimi = new MalBildirimiX();
            this.IzinDevir = new IzinDevirX();
        }

        #region "Key"

        public class AlanKey
        {
            public string Kisisel { get; protected set; }
            public string Adres { get; protected set; }
            public string Aile { get; protected set; }
            public string Ogrenim { get; protected set; }
            public string Dil { get; protected set; }
            public string Izin { get; protected set; }
            public string Memuriyet { get; protected set; }
            public string Kadro { get; protected set; }
            public string Kurs { get; protected set; }
            public string CezaOdul { get; protected set; }
            public string MalBildirimi { get; protected set; }
            public string IzinDevir { get; protected set; }

            public AlanKey()
            {
                this.Kisisel = "kisisel";
                this.Adres = "adres";
                this.Aile = "aile";
                this.Ogrenim = "ogrenim";
                this.Dil = "dil";
                this.Izin = "izin";
                this.Memuriyet = "memuriyet";
                this.Kadro = "kadro";
                this.Kurs = "kurs";
                this.CezaOdul = "cezaOdul";
                this.MalBildirimi = "malBildirimi";
                this.IzinDevir = "izinDevir";
            }
        }
        public class AlanKeyAd : AlanKey
        {
            public AlanKeyAd()
            {
                base.Kisisel = "Kişisel";
                base.Adres = "İletişim / Adres";
                base.Aile = "Aile";
                base.Ogrenim = "Öğrenim";
                base.Dil = "Yabancı Dil";
                base.Izin = "İzin Hareket";
                this.Memuriyet = "Memuriyet";
                this.Kadro = "Kadro";
                this.Kurs = "Hizmet İçi Eğitim";
                this.CezaOdul = "Ceza / Ödül";
                this.MalBildirimi = "Mal Bildirimi";
                this.IzinDevir = "İzin Devir";
            }
        }

        #endregion

        #region "Kaydet / Listele"

        /// <summary>
        /// </summary>
        /// <param name="yeni">Değişiklik yapılan bilgileri içeren yeni nesne</param>
        /// <param name="eski">Değişen bilgileri içeren eski nesne</param>
        /// <param name="alan">Değişikliğin hangi kategoride olduğunu belirten tanım. (KISISEL, ADRES, AILE...) </param>
        /// <param name="personelTCNo">Değişiklik yapılan personel bilgisi. (TcKimlikNo)</param>
        /// <param name="kullanan">Değişiklik yapan kullanıcı bilgisi.</param>
        /// <returns>Sonuc</returns>
        public static Sonuc Kaydet(Kullanici kullanan, Object yeni, Object eski, ENUMTARIHCE alan, string personelTCNo, ENUMTARIHCEISLEMTIP islem)
        {
            Sonuc sonuc = new Sonuc();

            PERPersonelTarihce tarihce = new PERPersonelTarihce(yeni, eski, alan, islem);
            PersonelTarihce pT = new PersonelTarihce();

            pT.degisiklik = tarihce.DegisenleriGetir();

            if (tarihce.Degisen)
            {
                pT.personel = personelTCNo;
                pT.kullanici = kullanan.mernis;
                pT.tarih = new TNSDateTime(DateTime.Now);
                pT.kullaniciAd = kullanan.ad + " " + kullanan.soyad;

                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
                ve.BeginTransaction();

                TABPersonel tab = new TABPersonel(ve);
                sonuc = tab.KaydetTarihce(pT);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = PersonelResI.PER007;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="personelTCKimlikNo"></param>
        /// <returns>TARIH,KATEGORI,DEGISENALAN,YENIDEGER,ESKIDEGER,DEGISIKLIKYAPAN</returns>
        public static string[] Listele(Kullanici kullanan, string personelTCKimlikNo)
        {
            ObjectArray pTList = new PERPersonel().ListeleTarihce(kullanan, new TNS.PER.PersonelTarihce() { personel = personelTCKimlikNo });

            PERPersonelTarihce tarihce = new PERPersonelTarihce();

            ArrayList liste = new ArrayList();
            XmlDocument xDoc;

            foreach (TNS.PER.PersonelTarihce pT in pTList.objeler)
            {
                xDoc = new XmlDocument();
                xDoc.LoadXml(pT.degisiklik);
                foreach (XmlNode nodeKategori in xDoc.DocumentElement.ChildNodes)
                {
                    string kategori = tarihce.KeyAdGetir(nodeKategori.Name);
                    string aciklama1 = "", aciklama2 = "", aciklama = "";
                    for (int i = 0; i < nodeKategori.Attributes.Count; i++)
                    {
                        aciklama = (nodeKategori.Attributes[i].Name == TagList.YeniKayit) ? "<b>Yeni Kayıt</b>" : (nodeKategori.Attributes[i].Name == TagList.Silindi) ? "<b>Silindi</b>" : string.Format("<b>{0}</b>: {1}", nodeKategori.Attributes[i].Name, nodeKategori.Attributes[i].Value);
                        if (i == 0) aciklama1 = aciklama; else if (i == 1) aciklama2 = aciklama;
                    }
                    foreach (XmlNode node in nodeKategori.ChildNodes)
                    {
                        string yAdtxt = (node.Attributes["yAd"] != null) ? node.Attributes["yAd"].Value : node.Attributes["y"].Value;
                        string eAdtxt = (node.Attributes["eAd"] != null) ? node.Attributes["eAd"].Value : node.Attributes["e"].Value;
                        liste.Add(string.Format("{0}!{1}!{2}!{3}!{4}!{5}!{6}!{7}", pT.tarih.ToLongString(), kategori, tarihce.KeyAdGetir(node.Name, tarihce.Alan), yAdtxt, eAdtxt, pT.kullaniciAd, aciklama1, aciklama2));
                    }
                }
            }

            return (string[])liste.ToArray(typeof(string));
        }

        #endregion

        #region "Methods"

        public string DegisenleriGetir()
        {
            Hashtable htDegisenDegerler = new Hashtable();
            string nodeAlanName = "";

            switch (Alan)
            {
                case ENUMTARIHCE.KISISEL:
                    nodeAlanName = this.Key.Kisisel;
                    htDegisenDegerler = this.Kisisel.DegisenDegerleriGetir((Personel)Yeni, (Personel)Eski, islem);
                    break;
                case ENUMTARIHCE.ADRES:
                    nodeAlanName = this.Key.Adres;
                    htDegisenDegerler = this.Adres.DegisenDegerleriGetir((PersonelAdres)Yeni, (PersonelAdres)Eski, islem);
                    break;
                case ENUMTARIHCE.AILE:
                    nodeAlanName = this.Key.Aile;
                    htDegisenDegerler = this.Aile.DegisenDegerleriGetir((PersonelAile)Yeni, (PersonelAile)Eski, islem);
                    break;
                case ENUMTARIHCE.OGRENIM:
                    nodeAlanName = this.Key.Ogrenim;
                    htDegisenDegerler = this.Ogrenim.DegisenDegerleriGetir((PersonelOgrenim)Yeni, (PersonelOgrenim)Eski, islem);
                    break;
                case ENUMTARIHCE.YABANCIDIL:
                    nodeAlanName = this.Key.Dil;
                    htDegisenDegerler = this.Dil.DegisenDegerleriGetir((PersonelDil)Yeni, (PersonelDil)Eski, islem);
                    break;
                case ENUMTARIHCE.IZINHAREKET:
                    nodeAlanName = this.Key.Izin;
                    htDegisenDegerler = this.Izin.DegisenDegerleriGetir((PersonelIzin)Yeni, (PersonelIzin)Eski, islem);
                    break;
                case ENUMTARIHCE.MEMURIYET:
                    nodeAlanName = this.Key.Memuriyet;
                    htDegisenDegerler = this.Memuriyet.DegisenDegerleriGetir((PersonelMemuriyet)Yeni, (PersonelMemuriyet)Eski, islem);
                    break;
                case ENUMTARIHCE.KADRO:
                    nodeAlanName = this.Key.Kadro;
                    htDegisenDegerler = this.Kadro.DegisenDegerleriGetir((PersonelKadro)Yeni, (PersonelKadro)Eski, islem);
                    break;
                case ENUMTARIHCE.EGITIM:
                    nodeAlanName = this.Key.Kurs;
                    htDegisenDegerler = this.Kurs.DegisenDegerleriGetir((PersonelKurs)Yeni, (PersonelKurs)Eski, islem);
                    break;
                case ENUMTARIHCE.CEZAODUL:
                    nodeAlanName = this.Key.CezaOdul;
                    htDegisenDegerler = this.CezaOdul.DegisenDegerleriGetir((PersonelCezaOdul)Yeni, (PersonelCezaOdul)Eski, islem);
                    break;
                case ENUMTARIHCE.MALBILDIRIMI:
                    nodeAlanName = this.Key.MalBildirimi;
                    htDegisenDegerler = this.MalBildirimi.DegisenDegerleriGetir((PersonelMalBildirimi)Yeni, (PersonelMalBildirimi)Eski, islem);
                    break;
                case ENUMTARIHCE.IZINDEVIR:
                    nodeAlanName = this.Key.IzinDevir;
                    htDegisenDegerler = this.IzinDevir.DegisenDegerleriGetir((PersonelIzinDevir)Yeni, (PersonelIzinDevir)Eski, islem);
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

                        //Yeni
                        if (deger[1].GetType() == typeof(string[])) { string[] yeni = (string[])deger[1]; yDeger = yeni[0]; yAdDeger = yeni[1]; } else yDeger = (string)deger[1];
                        y.Value = yDeger; nodeD.Attributes.Append(y);

                        //Eski
                        if (deger[2].GetType() == typeof(string[])) { string[] eski = (string[])deger[2]; eDeger = eski[0]; eAdDeger = eski[1]; } else eDeger = (string)deger[2];
                        e.Value = eDeger; nodeD.Attributes.Append(e);

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

        public string KeyAdGetir(string key)
        {
            if (key == this.Key.Kisisel) { this.Alan = ENUMTARIHCE.KISISEL; return this.KeyAd.Kisisel; }
            else if (key == this.Key.Adres) { this.Alan = ENUMTARIHCE.ADRES; return this.KeyAd.Adres; }
            else if (key == this.Key.Aile) { this.Alan = ENUMTARIHCE.AILE; return this.KeyAd.Aile; }
            else if (key == this.Key.Ogrenim) { this.Alan = ENUMTARIHCE.OGRENIM; return this.KeyAd.Ogrenim; }
            else if (key == this.Key.Dil) { this.Alan = ENUMTARIHCE.YABANCIDIL; return this.KeyAd.Dil; }
            else if (key == this.Key.Izin) { this.Alan = ENUMTARIHCE.IZINHAREKET; return this.KeyAd.Izin; }
            else if (key == this.Key.Memuriyet) { this.Alan = ENUMTARIHCE.MEMURIYET; return this.KeyAd.Memuriyet; }
            else if (key == this.Key.Kadro) { this.Alan = ENUMTARIHCE.KADRO; return this.KeyAd.Kadro; }
            else if (key == this.Key.Kurs) { this.Alan = ENUMTARIHCE.EGITIM; return this.KeyAd.Kurs; }
            else if (key == this.Key.CezaOdul) { this.Alan = ENUMTARIHCE.CEZAODUL; return this.KeyAd.CezaOdul; }
            else if (key == this.Key.MalBildirimi) { this.Alan = ENUMTARIHCE.MALBILDIRIMI; return this.KeyAd.MalBildirimi; }
            else if (key == this.Key.IzinDevir) { this.Alan = ENUMTARIHCE.IZINDEVIR; return this.KeyAd.IzinDevir; }

            return "";
        }

        public string KeyAdGetir(string key, ENUMTARIHCE alan)
        {
            switch (alan)
            {
                case ENUMTARIHCE.KISISEL:
                    if (key == this.Kisisel.Key.TcKimlikNo) return this.Kisisel.KeyAd.TcKimlikNo;
                    else if (key == this.Kisisel.Key.Ad) return this.Kisisel.KeyAd.Ad;
                    else if (key == this.Kisisel.Key.Soyad) return this.Kisisel.KeyAd.Soyad;
                    else if (key == this.Kisisel.Key.AnaAdi) return this.Kisisel.KeyAd.AnaAdi;
                    else if (key == this.Kisisel.Key.BabaAdi) return this.Kisisel.KeyAd.BabaAdi;
                    else if (key == this.Kisisel.Key.DogumYeri) return this.Kisisel.KeyAd.DogumYeri;
                    else if (key == this.Kisisel.Key.DogumTarihi) return this.Kisisel.KeyAd.DogumTarihi;
                    else if (key == this.Kisisel.Key.CuzdanSeriNo) return this.Kisisel.KeyAd.CuzdanSeriNo;
                    else if (key == this.Kisisel.Key.CuzdanKayitNo) return this.Kisisel.KeyAd.CuzdanKayitNo;
                    else if (key == this.Kisisel.Key.NufusIlKod) return this.Kisisel.KeyAd.NufusIlKod;
                    else if (key == this.Kisisel.Key.NufusIlceKod) return this.Kisisel.KeyAd.NufusIlceKod;
                    else if (key == this.Kisisel.Key.NufusMahalleKoy) return this.Kisisel.KeyAd.NufusMahalleKoy;
                    else if (key == this.Kisisel.Key.NufusCiltNo) return this.Kisisel.KeyAd.NufusCiltNo;
                    else if (key == this.Kisisel.Key.NufusSiraNo) return this.Kisisel.KeyAd.NufusSiraNo;
                    else if (key == this.Kisisel.Key.NufusAileSiraNo) return this.Kisisel.KeyAd.NufusAileSiraNo;
                    else if (key == this.Kisisel.Key.CuzdanVerilisNedeni) return this.Kisisel.KeyAd.CuzdanVerilisNedeni;
                    else if (key == this.Kisisel.Key.CuzdanVerildigiYer) return this.Kisisel.KeyAd.CuzdanVerildigiYer;
                    else if (key == this.Kisisel.Key.CuzdanVerilisTarihi) return this.Kisisel.KeyAd.CuzdanVerilisTarihi;
                    else if (key == this.Kisisel.Key.Cinsiyet) return this.Kisisel.KeyAd.Cinsiyet;
                    else if (key == this.Kisisel.Key.MedeniHali) return this.Kisisel.KeyAd.MedeniHali;
                    else if (key == this.Kisisel.Key.KanGrubu) return this.Kisisel.KeyAd.KanGrubu;
                    else if (key == this.Kisisel.Key.IlkSoyad) return this.Kisisel.KeyAd.IlkSoyad;
                    else if (key == this.Kisisel.Key.YasTashihTarihi) return this.Kisisel.KeyAd.YasTashihTarihi;
                    else if (key == this.Kisisel.Key.OdaKodu) return this.Kisisel.KeyAd.OdaKodu;
                    else if (key == this.Kisisel.Key.SicilNo) return this.Kisisel.KeyAd.SicilNo;
                    else if (key == this.Kisisel.Key.EmekliSicilNo) return this.Kisisel.KeyAd.EmekliSicilNo;
                    else if (key == this.Kisisel.Key.SaglikKarneNo) return this.Kisisel.KeyAd.SaglikKarneNo;
                    else if (key == this.Kisisel.Key.BakanlikKartNo) return this.Kisisel.KeyAd.BakanlikKartNo;
                    else if (key == this.Kisisel.Key.SskSicilNo) return this.Kisisel.KeyAd.SskSicilNo;
                    else if (key == this.Kisisel.Key.BagkurSicilNo) return this.Kisisel.KeyAd.BagkurSicilNo;
                    else if (key == this.Kisisel.Key.SurucuBelgeNo) return this.Kisisel.KeyAd.SurucuBelgeNo;
                    else if (key == this.Kisisel.Key.SurucuBelgeTarihi) return this.Kisisel.KeyAd.SurucuBelgeTarihi;
                    else if (key == this.Kisisel.Key.BankaKodu) return this.Kisisel.KeyAd.BankaKodu;
                    else if (key == this.Kisisel.Key.BankaSubeKodu) return this.Kisisel.KeyAd.BankaSubeKodu;
                    else if (key == this.Kisisel.Key.BankaHesapNo) return this.Kisisel.KeyAd.BankaHesapNo;
                    else if (key == this.Kisisel.Key.AskerlikKodu) return this.Kisisel.KeyAd.AskerlikKodu;
                    else if (key == this.Kisisel.Key.TecilTarihi) return this.Kisisel.KeyAd.TecilTarihi;
                    else if (key == this.Kisisel.Key.AskerlikMuafNeden) return this.Kisisel.KeyAd.AskerlikMuafNeden;
                    else if (key == this.Kisisel.Key.AskerlikBaslamaTarihi) return this.Kisisel.KeyAd.AskerlikBaslamaTarihi;
                    else if (key == this.Kisisel.Key.TerhisTarihi) return this.Kisisel.KeyAd.TerhisTarihi;
                    else if (key == this.Kisisel.Key.AskerlikCuzdanNo) return this.Kisisel.KeyAd.AskerlikCuzdanNo;
                    else if (key == this.Kisisel.Key.OrganizasyonKodu) return this.Kisisel.KeyAd.OrganizasyonKodu;
                    else if (key == this.Kisisel.Key.MuhasebeKodu) return this.Kisisel.KeyAd.MuhasebeKodu;
                    else if (key == this.Kisisel.Key.BirimKodu) return this.Kisisel.KeyAd.BirimKodu;

                    break;
                case ENUMTARIHCE.ADRES:
                    if (key == this.Adres.Key.TcKimlikNo) return this.Adres.KeyAd.TcKimlikNo;
                    else if (key == this.Adres.Key.Adres) return this.Adres.KeyAd.Adres;
                    else if (key == this.Adres.Key.MulkDurumu) return this.Adres.KeyAd.MulkDurumu;
                    else if (key == this.Adres.Key.TasinmaTarihi) return this.Adres.KeyAd.TasinmaTarihi;
                    else if (key == this.Adres.Key.IsTel) return this.Adres.KeyAd.IsTel;
                    else if (key == this.Adres.Key.EvTel) return this.Adres.KeyAd.EvTel;
                    else if (key == this.Adres.Key.CepTel) return this.Adres.KeyAd.CepTel;
                    else if (key == this.Adres.Key.Faks) return this.Adres.KeyAd.Faks;
                    else if (key == this.Adres.Key.Eposta) return this.Adres.KeyAd.Eposta;
                    else if (key == this.Adres.Key.Url) return this.Adres.KeyAd.Url;
                    break;
                case ENUMTARIHCE.AILE:
                    if (key == this.Aile.Key.TcKimlikNo) return this.Aile.KeyAd.TcKimlikNo;
                    else if (key == this.Aile.Key.Ad) return this.Aile.KeyAd.Ad;
                    else if (key == this.Aile.Key.Soyad) return this.Aile.KeyAd.Soyad;
                    else if (key == this.Aile.Key.DogumTarihi) return this.Aile.KeyAd.DogumTarihi;
                    else if (key == this.Aile.Key.Cinsiyet) return this.Aile.KeyAd.Cinsiyet;
                    else if (key == this.Aile.Key.Yakinlik) return this.Aile.KeyAd.Yakinlik;
                    else if (key == this.Aile.Key.Durum) return this.Aile.KeyAd.Durum;
                    else if (key == this.Aile.Key.EvlenmeTarihi) return this.Aile.KeyAd.EvlenmeTarihi;
                    else if (key == this.Aile.Key.AileCuzdaniSeriNo) return this.Aile.KeyAd.AileCuzdaniSeriNo;
                    else if (key == this.Aile.Key.SaglikKarneNo) return this.Aile.KeyAd.SaglikKarneNo;
                    break;
                case ENUMTARIHCE.OGRENIM:
                    if (key == this.Ogrenim.Key.TcKimlikNo) return this.Ogrenim.KeyAd.TcKimlikNo;
                    else if (key == this.Ogrenim.Key.OgrenimKod) return this.Ogrenim.KeyAd.OgrenimKod;
                    else if (key == this.Ogrenim.Key.OkulAdi) return this.Ogrenim.KeyAd.OkulAdi;
                    else if (key == this.Ogrenim.Key.BaslangicTarihi) return this.Ogrenim.KeyAd.BaslangicTarihi;
                    else if (key == this.Ogrenim.Key.BitirmeTarihi) return this.Ogrenim.KeyAd.BitirmeTarihi;
                    else if (key == this.Ogrenim.Key.BelgeNo) return this.Ogrenim.KeyAd.BelgeNo;
                    else if (key == this.Ogrenim.Key.Derece) return this.Ogrenim.KeyAd.Derece;
                    else if (key == this.Ogrenim.Key.BelgeTarihi) return this.Ogrenim.KeyAd.BelgeTarihi;
                    else if (key == this.Ogrenim.Key.Gecerli) return this.Ogrenim.KeyAd.Gecerli;
                    break;
                case ENUMTARIHCE.YABANCIDIL:
                    if (key == this.Dil.Key.TcKimlikNo) return this.Dil.KeyAd.TcKimlikNo;
                    else if (key == this.Dil.Key.DilKod) return this.Dil.KeyAd.DilKod;
                    else if (key == this.Dil.Key.Duzey) return this.Dil.KeyAd.Duzey;
                    else if (key == this.Dil.Key.Puan) return this.Dil.KeyAd.Puan;
                    else if (key == this.Dil.Key.DonemAy) return this.Dil.KeyAd.DonemAy;
                    else if (key == this.Dil.Key.DonemYil) return this.Dil.KeyAd.DonemYil;
                    else if (key == this.Dil.Key.BelgeTur) return this.Dil.KeyAd.BelgeTur;
                    else if (key == this.Dil.Key.BelgeTarihi) return this.Dil.KeyAd.BelgeTarihi;
                    else if (key == this.Dil.Key.BelgeNo) return this.Dil.KeyAd.BelgeNo;
                    break;
                case ENUMTARIHCE.IZINHAREKET:
                    if (key == this.Izin.Key.TcKimlikNo) return this.Izin.KeyAd.TcKimlikNo;
                    else if (key == this.Izin.Key.Tip) return this.Izin.KeyAd.Tip;
                    else if (key == this.Izin.Key.BaslangicTarihi) return this.Izin.KeyAd.BaslangicTarihi;
                    else if (key == this.Izin.Key.BitisTarihi) return this.Izin.KeyAd.BitisTarihi;
                    else if (key == this.Izin.Key.IzinNotu) return this.Izin.KeyAd.IzinNotu;
                    else if (key == this.Izin.Key.Aciklama) return this.Izin.KeyAd.Aciklama;
                    else if (key == this.Izin.Key.Telefon) return this.Izin.KeyAd.Telefon;
                    else if (key == this.Izin.Key.KullanilanGun) return this.Izin.KeyAd.KullanilanGun;
                    else if (key == this.Izin.Key.Onay) return this.Izin.KeyAd.Onay;
                    break;
                case ENUMTARIHCE.MEMURIYET:
                    if (key == this.Memuriyet.Key.TcKimlikNo) return this.Memuriyet.KeyAd.TcKimlikNo;
                    else if (key == this.Memuriyet.Key.BirimAd) return this.Memuriyet.KeyAd.BirimAd;
                    else if (key == this.Memuriyet.Key.Gorev) return this.Memuriyet.KeyAd.Gorev;
                    else if (key == this.Memuriyet.Key.IstihdamSekli) return this.Memuriyet.KeyAd.IstihdamSekli;
                    else if (key == this.Memuriyet.Key.BaslamaTarihi) return this.Memuriyet.KeyAd.BaslamaTarihi;
                    else if (key == this.Memuriyet.Key.AyrilmaTarihi) return this.Memuriyet.KeyAd.AyrilmaTarihi;
                    else if (key == this.Memuriyet.Key.BaslamaNedeni) return this.Memuriyet.KeyAd.BaslamaNedeni;
                    else if (key == this.Memuriyet.Key.AyrilmaNedeni) return this.Memuriyet.KeyAd.AyrilmaNedeni;
                    else if (key == this.Memuriyet.Key.EvrakNo) return this.Memuriyet.KeyAd.EvrakNo;
                    else if (key == this.Memuriyet.Key.YasalDayanak) return this.Memuriyet.KeyAd.YasalDayanak;
                    else if (key == this.Memuriyet.Key.OnayTarih) return this.Memuriyet.KeyAd.OnayTarih;
                    else if (key == this.Memuriyet.Key.Aciklama) return this.Memuriyet.KeyAd.Aciklama;
                    break;
                case ENUMTARIHCE.KADRO:
                    if (key == this.Kadro.Key.TcKimlikNo) return this.Kadro.KeyAd.TcKimlikNo;
                    else if (key == this.Kadro.Key.BirimAd) return this.Kadro.KeyAd.BirimAd;
                    else if (key == this.Kadro.Key.Unvan) return this.Kadro.KeyAd.Unvan;
                    else if (key == this.Kadro.Key.Sinif) return this.Kadro.KeyAd.Sinif;
                    else if (key == this.Kadro.Key.Aciklama) return this.Kadro.KeyAd.Aciklama;
                    else if (key == this.Kadro.Key.KadroDerece) return this.Kadro.KeyAd.KadroDerece;
                    else if (key == this.Kadro.Key.KadroTarih) return this.Kadro.KeyAd.KadroTarih;
                    else if (key == this.Kadro.Key.KazanilmisHakDerece) return this.Kadro.KeyAd.KazanilmisHakDerece;
                    else if (key == this.Kadro.Key.KazanilmisHakKademe) return this.Kadro.KeyAd.KazanilmisHakKademe;
                    else if (key == this.Kadro.Key.KazanilmisHakTarih) return this.Kadro.KeyAd.KazanilmisHakTarih;
                    else if (key == this.Kadro.Key.KazanilmisGosterge) return this.Kadro.KeyAd.KazanilmisGosterge;
                    else if (key == this.Kadro.Key.KazanilmisEkGosterge) return this.Kadro.KeyAd.KazanilmisEkGosterge;
                    else if (key == this.Kadro.Key.OdemeyeEsasDerece) return this.Kadro.KeyAd.OdemeyeEsasDerece;
                    else if (key == this.Kadro.Key.OdemeyeEsasKademe) return this.Kadro.KeyAd.OdemeyeEsasKademe;
                    else if (key == this.Kadro.Key.OdemeyeEsasTarih) return this.Kadro.KeyAd.OdemeyeEsasTarih;
                    else if (key == this.Kadro.Key.OdemeyeEsasGosterge) return this.Kadro.KeyAd.OdemeyeEsasGosterge;
                    else if (key == this.Kadro.Key.OdemeyeEsasEkGosterge) return this.Kadro.KeyAd.OdemeyeEsasEkGosterge;
                    else if (key == this.Kadro.Key.EmekliyeEsasDerece) return this.Kadro.KeyAd.EmekliyeEsasDerece;
                    else if (key == this.Kadro.Key.EmekliyeEsasKademe) return this.Kadro.KeyAd.EmekliyeEsasKademe;
                    else if (key == this.Kadro.Key.EmekliyeEsasTarih) return this.Kadro.KeyAd.EmekliyeEsasTarih;
                    else if (key == this.Kadro.Key.EmekliyeEsasGosterge) return this.Kadro.KeyAd.EmekliyeEsasGosterge;
                    else if (key == this.Kadro.Key.EmekliyeEsasEkGosterge) return this.Kadro.KeyAd.EmekliyeEsasEkGosterge;
                    break;
                case ENUMTARIHCE.EGITIM:
                    if (key == this.Kurs.Key.TcKimlikNo) return this.Kurs.KeyAd.TcKimlikNo;
                    else if (key == this.Kurs.Key.KursKod) return this.Kurs.KeyAd.KursKod;
                    else if (key == this.Kurs.Key.KursAdi) return this.Kurs.KeyAd.KursAdi;
                    else if (key == this.Kurs.Key.BaslamaTarihi) return this.Kurs.KeyAd.BaslamaTarihi;
                    else if (key == this.Kurs.Key.BitirmeTarihi) return this.Kurs.KeyAd.BitirmeTarihi;
                    else if (key == this.Kurs.Key.AldigiYer) return this.Kurs.KeyAd.AldigiYer;
                    else if (key == this.Kurs.Key.EgitimYeri) return this.Kurs.KeyAd.EgitimYeri;
                    else if (key == this.Kurs.Key.Gun) return this.Kurs.KeyAd.Gun;
                    else if (key == this.Kurs.Key.DersSaati) return this.Kurs.KeyAd.DersSaati;
                    else if (key == this.Kurs.Key.Yil) return this.Kurs.KeyAd.Yil;
                    else if (key == this.Kurs.Key.Donem) return this.Kurs.KeyAd.Donem;
                    else if (key == this.Kurs.Key.Derece) return this.Kurs.KeyAd.Derece;
                    else if (key == this.Kurs.Key.DosyaNo) return this.Kurs.KeyAd.DosyaNo;
                    else if (key == this.Kurs.Key.Aciklama) return this.Kurs.KeyAd.Aciklama;
                    break;
                case ENUMTARIHCE.CEZAODUL:
                    if (key == this.CezaOdul.Key.TcKimlikNo) return this.CezaOdul.KeyAd.TcKimlikNo;
                    else if (key == this.CezaOdul.Key.Tip) return this.CezaOdul.KeyAd.Tip;
                    else if (key == this.CezaOdul.Key.CezaOdulTip) return this.CezaOdul.KeyAd.CezaOdulTip;
                    else if (key == this.CezaOdul.Key.Tarih) return this.CezaOdul.KeyAd.Tarih;
                    else if (key == this.CezaOdul.Key.VerilisNedeni) return this.CezaOdul.KeyAd.VerilisNedeni;
                    break;
                case ENUMTARIHCE.MALBILDIRIMI:
                    if (key == this.MalBildirimi.Key.TcKimlikNo) return this.MalBildirimi.KeyAd.TcKimlikNo;
                    else if (key == this.MalBildirimi.Key.Tarih) return this.MalBildirimi.KeyAd.Tarih;
                    else if (key == this.MalBildirimi.Key.Aciklama) return this.MalBildirimi.KeyAd.Aciklama;
                    break;
                case ENUMTARIHCE.IZINDEVIR:
                    if (key == this.IzinDevir.Key.TcKimlikNo) return this.IzinDevir.KeyAd.TcKimlikNo;
                    else if (key == this.IzinDevir.Key.Yil) return this.IzinDevir.KeyAd.Yil;
                    else if (key == this.IzinDevir.Key.Devreden) return this.IzinDevir.KeyAd.Devreden;
                    else if (key == this.IzinDevir.Key.Yilici) return this.IzinDevir.KeyAd.Yilici;
                    else if (key == this.IzinDevir.Key.Ad) return this.IzinDevir.KeyAd.Ad;
                    else if (key == this.IzinDevir.Key.Soyad) return this.IzinDevir.KeyAd.Soyad;
                    else if (key == this.IzinDevir.Key.SicilNo) return this.IzinDevir.KeyAd.SicilNo;

                    break;
            }


            return "";
        }

        #endregion

        #region "Static Methods & Class"


        public class TagList
        {
            public static string Liste { get { return "liste"; } }
            public static string Bilgi { get { return "bilgi"; } }
            public static string YeniKayit { get { return "YeniKayit"; } }
            public static string Silindi { get { return "Silindi"; } }
            public static string SiraNo { get { return "SiraNo"; } }
            public static string AdSoyad { get { return "AdSoyad"; } }
            public static string OkulAd { get { return "OkulAd"; } }
            public static string Dil { get { return "Dil"; } }
            public static string Izin { get { return "IzinTuru"; } }
            public static string Birim { get { return "Birim"; } }
            public static string Ad { get { return "Adi"; } }
            public static string CezaOdul { get { return "CezaOdul"; } }
            public static string Tarih { get { return "Tarih"; } }
        }

        public static string GetKodAd(int kod, EnumKodAd e, ObjectArray list = null)
        {
            if (kod == 0) return "";
            switch (e)
            {
                case EnumKodAd.MulkDurumu:
                    switch (kod)
                    {
                        case 1: return "Lojman";
                        case 2: return "Kendisinin";
                        case 3: return "Kira";
                        case 4: return "Lojman Eşinin";
                    }
                    break;
                case EnumKodAd.Cinsiyet:
                    switch (kod)
                    {
                        case 1: return "Erkek";
                        case 2: return "Kadın";
                    }
                    break;
                case EnumKodAd.Yakinlik:
                    switch (kod)
                    {
                        case 1: return "Eşi";
                        case 2: return "Çocuğu";
                        case 3: return "Annesi";
                        case 4: return "Babası";
                        case 5: return "Kardeşi";
                    }
                    break;
                case EnumKodAd.Durum:
                    switch (kod)
                    {
                        case 1: return "Okuyor";
                        case 2: return "Mezun";
                        case 3: return "Evli";
                        case 4: return "Çalışıyor";
                        case 5: return "Çalışmıyor";
                        case 6: return "Malül";
                        case 7: return "Vefat";
                        case 8: return "Sağ";
                    }
                    break;
                case EnumKodAd.Ogrenim:
                    foreach (Ogrenim o in list.objeler)
                        if (kod == o.kod) return o.ad;
                    break;
                case EnumKodAd.SonOkul:
                    switch (kod)
                    {
                        case 1: return "Son Okul";
                    }
                    return "-";
                case EnumKodAd.Dil:
                    foreach (Dil o in list.objeler)
                        if (kod == o.kod) return o.ad;
                    break;
                case EnumKodAd.Ay:
                    switch (kod)
                    {
                        case 1: return "Ocak";
                        case 2: return "Şubat";
                        case 3: return "Mart";
                        case 4: return "Nisan";
                        case 5: return "Mayıs";
                        case 6: return "Haziran";
                        case 7: return "Temmuz";
                        case 8: return "Ağustos";
                        case 9: return "Eylül";
                        case 10: return "Ekim";
                        case 11: return "Kasım";
                        case 12: return "Aralık";
                    }
                    break;
                case EnumKodAd.DilBelgeTur:
                    switch (kod)
                    {
                        case 1: return "Sertifika";
                        case 2: return "Katılım Belgesi";
                        case 3: return "KPDS";
                        case 4: return "TOEFL";
                        case 5: return "IELTS";
                        case 6: return "ÜDS";
                        case 7: return "YDS";
                        case 99: return "Beyan";
                    }
                    break;
                case EnumKodAd.MedeniHal:
                    switch (kod)
                    {
                        case 1: return "Bekar";
                        case 2: return "Evli";
                    }
                    break;
                case EnumKodAd.Banka:
                    foreach (Banka o in list.objeler)
                        if (kod == OrtakFonksiyonlar.ConvertToInt(o.kod, 0)) return o.ad;
                    break;
                case EnumKodAd.BankaSube:
                    foreach (BankaSube o in list.objeler)
                        if (kod == OrtakFonksiyonlar.ConvertToInt(o.kod, 0)) return o.ad;
                    break;
                case EnumKodAd.Askerlik:
                    switch (kod)
                    {
                        case 1: return "Yaptı";
                        case 2: return "Tecilli";
                        case 3: return "Muaf";
                        case 4: return "Askerde";
                        case 5: return "Bakaya";
                        case 6: return "Yapmadı";
                    }
                    break;
                case EnumKodAd.IzinTip:
                    switch (kod)
                    {
                        case 1: return "Mazeret";
                        case 2: return "Sıhhi";
                        case 3: return "Yıllık";
                        case 4: return "Ücretli";
                        case 5: return "Ücretsiz";
                        case 6: return "Refakatlı";
                    }
                    break;
                case EnumKodAd.IstihdamSekli:
                    switch (kod)
                    {
                        case 1: return "Memur";
                        case 5: return "Sözleşmeli";
                        case 10: return "İşçi";
                        case 20: return "SSK-Bağkur";
                        case 21: return "Özel Sektör";
                    }
                    break;
                case EnumKodAd.BaslamaNedeni:
                    foreach (Neden o in list.objeler)
                        if (o.baslama > 0 && kod == o.kod) return o.ad;
                    break;
                case EnumKodAd.AyrilmaNedeni:
                    foreach (Neden o in list.objeler)
                        if (o.ayrilma > 0 && kod == o.kod) return o.ad;
                    break;
                case EnumKodAd.Unvan:
                    foreach (Unvan o in list.objeler)
                        if (kod == o.kod) return o.ad;
                    break;
                case EnumKodAd.Sinif:
                    foreach (Sinif o in list.objeler)
                        if (kod == o.kod) return o.ad;
                    break;
                case EnumKodAd.Kurs:
                    foreach (Kurs o in list.objeler)
                        if (kod == o.kod) return o.ad;
                    break;
                case EnumKodAd.CezaOdul:
                    switch (kod)
                    {
                        case 1: return "Ceza";
                        case 2: return "Ödül";
                    }
                    break;
                case EnumKodAd.CezaOdulTip:
                    switch (OrtakFonksiyonlar.ConvertToInt(list.objeler[0], 0))
                    {
                        case 1: //Ceza
                            switch (kod)
                            {
                                case 1: return "Uyarma";
                                case 2: return "Kınama";
                                case 3: return "Aylıktan Kesme";
                                case 4: return "Kademe İlerlemesini Durdurma";
                                case 5: return "Devlet Memuriyetinden Çıkarma";
                            }
                            break;
                        case 2: //Ödül
                            switch (kod)
                            {
                                case 1: return "Ödül";
                                case 2: return "Takdirname";
                            }
                            break;
                    }

                    break;
            }
            return "";
        }

        public enum EnumKodAd
        {
            MulkDurumu, Cinsiyet, Yakinlik, Durum, Ogrenim, SonOkul, Dil, Ay, DilBelgeTur, MedeniHal, Banka, BankaSube, Askerlik, IzinTip,
            IstihdamSekli, BaslamaNedeni, AyrilmaNedeni,
            Unvan, Sinif, Kurs, CezaOdul, CezaOdulTip
        }


        #endregion



        #region "Kişisel"

        public class KisiselX
        {
            public KisiselKey Key { get; protected set; }
            public KisiselKeyAd KeyAd { get; protected set; }

            public KisiselX()
            {
                this.Key = new KisiselKey();
                this.KeyAd = new KisiselKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(Personel yeni, Personel eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();
                ObjectArray listeBanka = new PERBanka().Listele(new Banka());
                ObjectArray listeBankaSubeYeni = new PERBankaSube().Listele(new BankaSube() { bankaKodu = yeni.bankaKodu });
                ObjectArray listeBankaSubeEski = (yeni.bankaKodu == eski.bankaKodu) ? listeBankaSubeYeni : new PERBankaSube().Listele(new BankaSube() { bankaKodu = eski.bankaKodu });

                if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.ad != eski.ad) liste.Add(new string[] { this.Key.Ad, yeni.ad, eski.ad });
                if (yeni.soyad != eski.soyad) liste.Add(new string[] { this.Key.Soyad, yeni.soyad, eski.soyad });
                if (yeni.anaAd != eski.anaAd) liste.Add(new string[] { this.Key.AnaAdi, yeni.anaAd, eski.anaAd });
                if (yeni.babaAd != eski.babaAd) liste.Add(new string[] { this.Key.BabaAdi, yeni.babaAd, eski.babaAd });
                if (yeni.dogumYeri != eski.dogumYeri) liste.Add(new string[] { this.Key.DogumYeri, yeni.dogumYeri, eski.dogumYeri });
                if (yeni.dogumTarihi.ToString() != eski.dogumTarihi.ToString()) liste.Add(new string[] { this.Key.DogumTarihi, yeni.dogumTarihi.ToString(), eski.dogumTarihi.ToString() });
                if (yeni.cuzdanSeriNo != eski.cuzdanSeriNo) liste.Add(new string[] { this.Key.CuzdanSeriNo, yeni.cuzdanSeriNo, eski.cuzdanSeriNo });
                if (yeni.cuzdanKayitNo != eski.cuzdanKayitNo) liste.Add(new string[] { this.Key.CuzdanKayitNo, yeni.cuzdanKayitNo, eski.cuzdanKayitNo });
                if (yeni.nufusIlKod != eski.nufusIlKod) liste.Add(new string[] { this.Key.NufusIlKod, yeni.nufusIlKod, eski.nufusIlKod });
                if (yeni.nufusIlceKod != eski.nufusIlceKod) liste.Add(new string[] { this.Key.NufusIlceKod, yeni.nufusIlceKod, eski.nufusIlceKod });
                if (yeni.nufusMahalleKoy != eski.nufusMahalleKoy) liste.Add(new string[] { this.Key.NufusMahalleKoy, yeni.nufusMahalleKoy, eski.nufusMahalleKoy });
                if (yeni.nufusCiltNo != eski.nufusCiltNo) liste.Add(new string[] { this.Key.NufusCiltNo, yeni.nufusCiltNo, eski.nufusCiltNo });
                if (yeni.nufusSiraNo != eski.nufusSiraNo) liste.Add(new string[] { this.Key.NufusSiraNo, yeni.nufusSiraNo, eski.nufusSiraNo });
                if (yeni.nufusAileSiraNo != eski.nufusAileSiraNo) liste.Add(new string[] { this.Key.NufusAileSiraNo, yeni.nufusAileSiraNo, eski.nufusAileSiraNo });
                if (yeni.cuzdanVerilisNedeni != eski.cuzdanVerilisNedeni) liste.Add(new string[] { this.Key.CuzdanVerilisNedeni, yeni.cuzdanVerilisNedeni, eski.cuzdanVerilisNedeni });
                if (yeni.cuzdanVerildigiYer != eski.cuzdanVerildigiYer) liste.Add(new string[] { this.Key.CuzdanVerildigiYer, yeni.cuzdanVerildigiYer, eski.cuzdanVerildigiYer });
                if (yeni.cuzdanVerilisTarihi.ToString() != eski.cuzdanVerilisTarihi.ToString()) liste.Add(new string[] { this.Key.CuzdanVerilisTarihi, yeni.cuzdanVerilisTarihi.ToString(), eski.cuzdanVerilisTarihi.ToString() });
                if (yeni.cinsiyet != eski.cinsiyet) liste.Add(new object[] { this.Key.Cinsiyet, new string[] { yeni.cinsiyet.ToString(), GetKodAd(yeni.cinsiyet, EnumKodAd.Cinsiyet) }, new string[] { eski.cinsiyet.ToString(), GetKodAd(eski.cinsiyet, EnumKodAd.Cinsiyet) } });
                if (yeni.medeniHali != eski.medeniHali) liste.Add(new object[] { this.Key.MedeniHali, new string[] { yeni.medeniHali.ToString(), GetKodAd(yeni.medeniHali, EnumKodAd.MedeniHal) }, new string[] { eski.medeniHali.ToString(), GetKodAd(eski.medeniHali, EnumKodAd.MedeniHal) } });
                if (yeni.kanGrubu != eski.kanGrubu) liste.Add(new string[] { this.Key.KanGrubu, yeni.kanGrubu, eski.kanGrubu });
                if (yeni.ilkSoyad != eski.ilkSoyad) liste.Add(new string[] { this.Key.IlkSoyad, yeni.ilkSoyad, eski.ilkSoyad });
                if (yeni.yasTashihTarihi.ToString() != eski.yasTashihTarihi.ToString()) liste.Add(new string[] { this.Key.YasTashihTarihi, yeni.yasTashihTarihi.ToString(), eski.yasTashihTarihi.ToString() });
                if (yeni.odaKodu != eski.odaKodu) liste.Add(new string[] { this.Key.OdaKodu, yeni.odaKodu, eski.odaKodu });
                if (yeni.sicilNo != eski.sicilNo) liste.Add(new string[] { this.Key.SicilNo, yeni.sicilNo, eski.sicilNo });
                if (yeni.emekliSicilNo != eski.emekliSicilNo) liste.Add(new string[] { this.Key.EmekliSicilNo, yeni.emekliSicilNo, eski.emekliSicilNo });
                if (yeni.saglikKarneNo != eski.saglikKarneNo) liste.Add(new string[] { this.Key.SaglikKarneNo, yeni.saglikKarneNo, eski.saglikKarneNo });
                if (yeni.bakanlikKartNo != eski.bakanlikKartNo) liste.Add(new string[] { this.Key.BakanlikKartNo, yeni.bakanlikKartNo, eski.bakanlikKartNo });
                if (yeni.sskSicilNo != eski.sskSicilNo) liste.Add(new string[] { this.Key.SskSicilNo, yeni.sskSicilNo, eski.sskSicilNo });
                if (yeni.bagkurSicilNo != eski.bagkurSicilNo) liste.Add(new string[] { this.Key.BagkurSicilNo, yeni.bagkurSicilNo, eski.bagkurSicilNo });
                if (yeni.surucuBelgeNo != eski.surucuBelgeNo) liste.Add(new string[] { this.Key.SurucuBelgeNo, yeni.surucuBelgeNo, eski.surucuBelgeNo });
                if (yeni.surucuBelgeTarihi.ToString() != eski.surucuBelgeTarihi.ToString()) liste.Add(new string[] { this.Key.SurucuBelgeTarihi, yeni.surucuBelgeTarihi.ToString(), eski.surucuBelgeTarihi.ToString() });
                if (yeni.bankaKodu != eski.bankaKodu) liste.Add(new object[] { this.Key.BankaKodu, new string[] { yeni.bankaKodu.ToString(), GetKodAd(OrtakFonksiyonlar.ConvertToInt(yeni.bankaKodu, 0), EnumKodAd.Banka, listeBanka) }, new string[] { eski.bankaKodu.ToString(), GetKodAd(OrtakFonksiyonlar.ConvertToInt(eski.bankaKodu, 0), EnumKodAd.Banka, listeBanka) } });
                if (yeni.bankaSubeKodu != eski.bankaSubeKodu) liste.Add(new object[] { this.Key.BankaSubeKodu, new string[] { yeni.bankaSubeKodu.ToString(), GetKodAd(OrtakFonksiyonlar.ConvertToInt(yeni.bankaSubeKodu, 0), EnumKodAd.BankaSube, listeBankaSubeYeni) }, new string[] { eski.bankaSubeKodu.ToString(), GetKodAd(OrtakFonksiyonlar.ConvertToInt(eski.bankaSubeKodu, 0), EnumKodAd.BankaSube, listeBankaSubeYeni) } });
                if (yeni.bankaHesapNo != eski.bankaHesapNo) liste.Add(new string[] { this.Key.BankaHesapNo, yeni.bankaHesapNo, eski.bankaHesapNo });
                if (yeni.askerlikKodu != eski.askerlikKodu) liste.Add(new object[] { this.Key.AskerlikKodu, new string[] { yeni.askerlikKodu.ToString(), GetKodAd(yeni.askerlikKodu, EnumKodAd.Askerlik) }, new string[] { eski.askerlikKodu.ToString(), GetKodAd(eski.askerlikKodu, EnumKodAd.Askerlik) } });
                if (yeni.tecilTarihi.ToString() != eski.tecilTarihi.ToString()) liste.Add(new string[] { this.Key.TecilTarihi, yeni.tecilTarihi.ToString(), eski.tecilTarihi.ToString() });
                if (yeni.askerlikMuafNeden != eski.askerlikMuafNeden) liste.Add(new string[] { this.Key.AskerlikMuafNeden, yeni.askerlikMuafNeden, eski.askerlikMuafNeden });
                if (yeni.askerlikBaslamaTarihi.ToString() != eski.askerlikBaslamaTarihi.ToString()) liste.Add(new string[] { this.Key.AskerlikBaslamaTarihi, yeni.askerlikBaslamaTarihi.ToString(), eski.askerlikBaslamaTarihi.ToString() });
                if (yeni.terhisTarihi.ToString() != eski.terhisTarihi.ToString()) liste.Add(new string[] { this.Key.TerhisTarihi, yeni.terhisTarihi.ToString(), eski.terhisTarihi.ToString() });
                if (yeni.askerlikCuzdanNo != eski.askerlikCuzdanNo) liste.Add(new string[] { this.Key.AskerlikCuzdanNo, yeni.askerlikCuzdanNo, eski.askerlikCuzdanNo });
                if (yeni.organizasyonKodu != eski.organizasyonKodu) liste.Add(new string[] { this.Key.OrganizasyonKodu, yeni.organizasyonKodu.ToString(), eski.organizasyonKodu.ToString() });
                if (yeni.muhasebeKodu != eski.muhasebeKodu) liste.Add(new string[] { this.Key.MuhasebeKodu, yeni.muhasebeKodu.ToString(), eski.muhasebeKodu.ToString() });
                if (yeni.birimKodu != eski.birimKodu) liste.Add(new string[] { this.Key.BirimKodu, yeni.birimKodu, eski.birimKodu });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                if (islem == ENUMTARIHCEISLEMTIP.KAYDET) table.Add(TagList.Bilgi, new object[] { new string[] { TagList.YeniKayit, "" } });

                return table;
            }

        }
        public class KisiselKey
        {
            public string TcKimlikNo { get; protected set; }
            public string Ad { get; protected set; }
            public string Soyad { get; protected set; }
            public string AnaAdi { get; protected set; }
            public string BabaAdi { get; protected set; }
            public string DogumYeri { get; protected set; }
            public string DogumTarihi { get; protected set; }
            public string CuzdanSeriNo { get; protected set; }
            public string CuzdanKayitNo { get; protected set; }
            public string NufusIlKod { get; protected set; }
            public string NufusIlceKod { get; protected set; }
            public string NufusMahalleKoy { get; protected set; }
            public string NufusCiltNo { get; protected set; }
            public string NufusSiraNo { get; protected set; }
            public string NufusAileSiraNo { get; protected set; }
            public string CuzdanVerilisNedeni { get; protected set; }
            public string CuzdanVerildigiYer { get; protected set; }
            public string CuzdanVerilisTarihi { get; protected set; }
            public string Cinsiyet { get; protected set; }
            public string MedeniHali { get; protected set; }
            public string KanGrubu { get; protected set; }
            public string IlkSoyad { get; protected set; }
            public string YasTashihTarihi { get; protected set; }
            public string OdaKodu { get; protected set; }
            public string SicilNo { get; protected set; }
            public string EmekliSicilNo { get; protected set; }
            public string SaglikKarneNo { get; protected set; }
            public string BakanlikKartNo { get; protected set; }
            public string SskSicilNo { get; protected set; }
            public string BagkurSicilNo { get; protected set; }
            public string SurucuBelgeNo { get; protected set; }
            public string SurucuBelgeTarihi { get; protected set; }
            public string BankaKodu { get; protected set; }
            public string BankaSubeKodu { get; protected set; }
            public string BankaHesapNo { get; protected set; }
            public string AskerlikKodu { get; protected set; }
            public string TecilTarihi { get; protected set; }
            public string AskerlikMuafNeden { get; protected set; }
            public string AskerlikBaslamaTarihi { get; protected set; }
            public string TerhisTarihi { get; protected set; }
            public string AskerlikCuzdanNo { get; protected set; }
            public string OrganizasyonKodu { get; protected set; }
            public string MuhasebeKodu { get; protected set; }
            public string BirimKodu { get; protected set; }

            public KisiselKey()
            {
                TcKimlikNo = "TcKimlikNo";
                Ad = "Ad";
                Soyad = "Soyad";
                AnaAdi = "AnaAdi";
                BabaAdi = "BabaAdi";
                DogumYeri = "DogumYeri";
                DogumTarihi = "DogumTarihi";
                CuzdanSeriNo = "CuzdanSeriNo";
                CuzdanKayitNo = "CuzdanKayitNo";
                NufusIlKod = "NufusIlKod";
                NufusIlceKod = "NufusIlceKod";
                NufusMahalleKoy = "NufusMahalleKoy";
                NufusCiltNo = "NufusCiltNo";
                NufusSiraNo = "NufusSiraNo";
                NufusAileSiraNo = "NufusAileSiraNo";
                CuzdanVerilisNedeni = "CuzdanVerilisNedeni";
                CuzdanVerildigiYer = "CuzdanVerildigiYer";
                CuzdanVerilisTarihi = "CuzdanVerilisTarihi";
                Cinsiyet = "Cinsiyet";
                MedeniHali = "MedeniHali";
                KanGrubu = "KanGrubu";
                IlkSoyad = "IlkSoyad";
                YasTashihTarihi = "YasTashihTarihi";
                OdaKodu = "OdaKodu";
                SicilNo = "SicilNo";
                EmekliSicilNo = "EmekliSicilNo";
                SaglikKarneNo = "SaglikKarneNo";
                BakanlikKartNo = "BakanlikKartNo";
                SskSicilNo = "SskSicilNo";
                BagkurSicilNo = "BagkurSicilNo";
                SurucuBelgeNo = "SurucuBelgeNo";
                SurucuBelgeTarihi = "SurucuBelgeTarihi";
                BankaKodu = "BankaKodu";
                BankaSubeKodu = "BankaSubeKodu";
                BankaHesapNo = "BankaHesapNo";
                AskerlikKodu = "AskerlikKodu";
                TecilTarihi = "TecilTarihi";
                AskerlikMuafNeden = "AskerlikMuafNeden";
                AskerlikBaslamaTarihi = "AskerlikBaslamaTarihi";
                TerhisTarihi = "TerhisTarihi";
                AskerlikCuzdanNo = "AskerlikCuzdanNo";
                OrganizasyonKodu = "OrganizasyonKodu";
                MuhasebeKodu = "MuhasebeKodu";
                BirimKodu = "BirimKodu";
            }
        }
        public class KisiselKeyAd : KisiselKey
        {
            public KisiselKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.Ad = "Adı";
                base.Soyad = "Soyadı";
                base.AnaAdi = "Ana Adı";
                base.BabaAdi = "Baba Adı";
                base.DogumYeri = "Doğum Yeri";
                base.DogumTarihi = "Doğum Tarihi";
                base.CuzdanSeriNo = "Cüzdan Seri";
                base.CuzdanKayitNo = "Kayıt Numarası";
                base.NufusIlKod = "Nüfus İl";
                base.NufusIlceKod = "Nüfus İlçe";
                base.NufusMahalleKoy = "Mahalle/Köy";
                base.NufusCiltNo = "Cilt No";
                base.NufusSiraNo = "Sıra No";
                base.NufusAileSiraNo = "Aile Sıra No";
                base.CuzdanVerilisNedeni = "Veriliş Nedeni";
                base.CuzdanVerildigiYer = "Verildiği Yer";
                base.CuzdanVerilisTarihi = "Veriliş Tarihi";
                base.Cinsiyet = "Cinsiyeti";
                base.MedeniHali = "Medeni Hali";
                base.KanGrubu = "Kan Grubu";
                base.IlkSoyad = "İlk Soyadı";
                base.YasTashihTarihi = "Yaş Tashih Tarihi";
                base.OdaKodu = "Oda Kodu";
                base.SicilNo = "Sicil No";
                base.EmekliSicilNo = "Emekli Sicil No";
                base.SaglikKarneNo = "Sağlık Karne No";
                base.BakanlikKartNo = "Bakanlık Kart No";
                base.SskSicilNo = "SSK Sicil No";
                base.BagkurSicilNo = "Bağkur Sicil No";
                base.SurucuBelgeNo = "Ehliyet Belge No";
                base.SurucuBelgeTarihi = "Ehliyet Belge Tarihi";
                base.BankaKodu = "Banka";
                base.BankaSubeKodu = "Şube";
                base.BankaHesapNo = "IBAN No";
                base.AskerlikKodu = "Askerlik Durumu";
                base.TecilTarihi = "Tecil Tarihi";
                base.AskerlikMuafNeden = "Muaf Nedeni";
                base.AskerlikBaslamaTarihi = "Başlama Tarihi";
                base.TerhisTarihi = "Terhis Tarihi";
                base.AskerlikCuzdanNo = "Askerlik Cüzdan No";
                base.OrganizasyonKodu = "OrganizasyonKodu";
                base.MuhasebeKodu = "MuhasebeKodu";
                base.BirimKodu = "BirimKod";

            }
        }

        #endregion

        #region "İletişim / Adres"

        public class AdresX
        {
            public AdresKey Key { get; protected set; }
            public AdresKeyAd KeyAd { get; protected set; }

            public AdresX()
            {
                this.Key = new AdresKey();
                this.KeyAd = new AdresKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelAdres yeni, PersonelAdres eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();

                if (yeni.adres != eski.adres) liste.Add(new string[] { this.Key.Adres, yeni.adres, eski.adres });
                if (yeni.mulkDurumu != eski.mulkDurumu) liste.Add(new object[] { this.Key.MulkDurumu, new string[] { yeni.mulkDurumu.ToString(), GetKodAd(yeni.mulkDurumu, EnumKodAd.MulkDurumu) }, new string[] { eski.mulkDurumu.ToString(), GetKodAd(eski.mulkDurumu, EnumKodAd.MulkDurumu) } });
                if (yeni.tasinmaTarihi.ToString() != eski.tasinmaTarihi.ToString()) liste.Add(new string[] { this.Key.TasinmaTarihi, yeni.tasinmaTarihi.ToString(), eski.tasinmaTarihi.ToString() });
                if (yeni.isTel != eski.isTel) liste.Add(new string[] { this.Key.IsTel, yeni.isTel, eski.isTel });
                if (yeni.evTel != eski.evTel) liste.Add(new string[] { this.Key.EvTel, yeni.evTel, eski.evTel });
                if (yeni.cepTel != eski.cepTel) liste.Add(new string[] { this.Key.CepTel, yeni.cepTel.ToString(), eski.cepTel.ToString() });
                if (yeni.faks != eski.faks) liste.Add(new string[] { this.Key.Faks, yeni.faks, eski.faks });
                if (yeni.eposta != eski.eposta) liste.Add(new string[] { this.Key.Eposta, yeni.eposta, eski.eposta });
                if (yeni.url != eski.url) liste.Add(new string[] { this.Key.Url, yeni.url, eski.url });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                if (islem == ENUMTARIHCEISLEMTIP.KAYDET) table.Add(TagList.Bilgi, new object[] { new string[] { TagList.YeniKayit, "" } });

                return table;
            }

        }
        public class AdresKey
        {
            public string TcKimlikNo { get; protected set; }
            public string Adres { get; protected set; }
            public string MulkDurumu { get; protected set; }
            public string TasinmaTarihi { get; protected set; }
            public string IsTel { get; protected set; }
            public string EvTel { get; protected set; }
            public string CepTel { get; protected set; }
            public string Faks { get; protected set; }
            public string Eposta { get; protected set; }
            public string Url { get; protected set; }

            public AdresKey()
            {
                TcKimlikNo = "TcKimlikNo";
                Adres = "Adres";
                MulkDurumu = "MulkDurumu";
                TasinmaTarihi = "TasinmaTarihi";
                IsTel = "IsTel";
                EvTel = "EvTel";
                CepTel = "CepTel";
                Faks = "Faks";
                Eposta = "Eposta";
                Url = "Url";
            }
        }
        public class AdresKeyAd : AdresKey
        {
            public AdresKeyAd()
            {
                base.TcKimlikNo = "*";
                base.Adres = "Adres";
                base.MulkDurumu = "Mülkiyet Durumu";
                base.TasinmaTarihi = "Oturma Tarihi";
                base.IsTel = "Telefon (İş)";
                base.EvTel = "Telefon (Ev)";
                base.CepTel = "Telefon (GSM)";
                base.Faks = "Faks";
                base.Eposta = "E-Posta";
                base.Url = "Web (Internet Adresi)";
            }
        }

        #endregion

        #region "Aile"

        public class AileX
        {
            public AileKey Key { get; protected set; }
            public AileKeyAd KeyAd { get; protected set; }

            public AileX()
            {
                this.Key = new AileKey();
                this.KeyAd = new AileKeyAd();

            }

            public Hashtable DegisenDegerleriGetir(PersonelAile yeni, PersonelAile eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();

                if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.tcKimlikNo : "" });
                if (yeni.ad != eski.ad) liste.Add(new string[] { this.Key.Ad, yeni.ad, eski.ad });
                if (yeni.soyad != eski.soyad) liste.Add(new string[] { this.Key.Soyad, yeni.soyad, eski.soyad });
                if (yeni.dogumTarihi.ToString() != eski.dogumTarihi.ToString()) liste.Add(new string[] { this.Key.DogumTarihi, yeni.dogumTarihi.ToString(), eski.dogumTarihi.ToString() });
                if (yeni.cinsiyet != eski.cinsiyet) liste.Add(new object[] { this.Key.Cinsiyet, new string[] { yeni.cinsiyet.ToString(), GetKodAd(yeni.cinsiyet, EnumKodAd.Cinsiyet) }, new string[] { eski.cinsiyet.ToString(), GetKodAd(eski.cinsiyet, EnumKodAd.Cinsiyet) } });
                if (yeni.yakinlik != eski.yakinlik) liste.Add(new object[] { this.Key.Yakinlik, new string[] { yeni.yakinlik.ToString(), GetKodAd(yeni.yakinlik, EnumKodAd.Yakinlik) }, new string[] { eski.yakinlik.ToString(), GetKodAd(eski.yakinlik, EnumKodAd.Yakinlik) } });
                if (yeni.durum != eski.durum) liste.Add(new object[] { this.Key.Durum, new string[] { yeni.durum.ToString(), GetKodAd(yeni.durum, EnumKodAd.Durum) }, new string[] { eski.durum.ToString(), GetKodAd(eski.durum, EnumKodAd.Durum) } });
                if (yeni.evlenmeTarihi.ToString() != eski.evlenmeTarihi.ToString()) liste.Add(new string[] { this.Key.EvlenmeTarihi, yeni.evlenmeTarihi.ToString(), eski.evlenmeTarihi.ToString() });
                if (yeni.aileCuzdaniSeriNo != eski.aileCuzdaniSeriNo) liste.Add(new string[] { this.Key.AileCuzdaniSeriNo, yeni.aileCuzdaniSeriNo, eski.aileCuzdaniSeriNo });
                if (yeni.saglikKarneNo != eski.saglikKarneNo) liste.Add(new string[] { this.Key.SaglikKarneNo, yeni.saglikKarneNo, eski.saglikKarneNo });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, islem == ENUMTARIHCEISLEMTIP.KAYDET ? new object[] { new string[] { TagList.YeniKayit, "" } } : new object[] { new string[] { TagList.AdSoyad, eski.ad + " " + eski.soyad } });

                return table;
            }

        }
        public class AileKey
        {
            public string TcKimlikNo { get; protected set; }
            public string Ad { get; protected set; }
            public string Soyad { get; protected set; }
            public string DogumTarihi { get; protected set; }
            public string Cinsiyet { get; protected set; }
            public string Yakinlik { get; protected set; }
            public string Durum { get; protected set; }
            public string EvlenmeTarihi { get; protected set; }
            public string AileCuzdaniSeriNo { get; protected set; }
            public string SaglikKarneNo { get; protected set; }

            public AileKey()
            {
                TcKimlikNo = "TcKimlikNo";
                Ad = "Ad";
                Soyad = "Soyad";
                DogumTarihi = "DogumTarihi";
                Cinsiyet = "Cinsiyet";
                Yakinlik = "Yakinlik";
                Durum = "Durum";
                EvlenmeTarihi = "EvlenmeTarihi";
                AileCuzdaniSeriNo = "AileCuzdaniSeriNo";
                SaglikKarneNo = "SaglikKarneNo";
            }
        }
        public class AileKeyAd : AileKey
        {
            public AileKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.Ad = "Adı";
                base.Soyad = "Soyadı";
                base.DogumTarihi = "Doğum Tarihi";
                base.Cinsiyet = "Cinsiyeti";
                base.Yakinlik = "Yakınlık Derecesi";
                base.Durum = "Durumu";
                base.EvlenmeTarihi = "Evlenme Tarihi";
                base.AileCuzdaniSeriNo = "Aile Cüzdanı Seri No";
                base.SaglikKarneNo = "Sağlık Karne No";
            }
        }

        #endregion

        #region "Öğrenim"

        public class OgrenimX
        {
            public OgrenimKey Key { get; protected set; }
            public OgrenimKeyAd KeyAd { get; protected set; }

            public OgrenimX()
            {
                this.Key = new OgrenimKey();
                this.KeyAd = new OgrenimKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelOgrenim yeni, PersonelOgrenim eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();
                ObjectArray listeOgrenim = new PEROgrenim().Listele(new Ogrenim());

                if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.ogrenimKod != eski.ogrenimKod) liste.Add(new object[] { this.Key.OgrenimKod, new string[] { yeni.ogrenimKod.ToString(), GetKodAd(yeni.ogrenimKod, EnumKodAd.Ogrenim, listeOgrenim) }, new string[] { eski.ogrenimKod.ToString(), GetKodAd(eski.ogrenimKod, EnumKodAd.Ogrenim, listeOgrenim) } });
                if (yeni.okulAdi != eski.okulAdi) liste.Add(new string[] { this.Key.OkulAdi, yeni.okulAdi, eski.okulAdi });
                if (yeni.baslangicTarihi.ToString() != eski.baslangicTarihi.ToString()) liste.Add(new string[] { this.Key.BaslangicTarihi, yeni.baslangicTarihi.ToString(), eski.baslangicTarihi.ToString() });
                if (yeni.bitirmeTarihi.ToString() != eski.bitirmeTarihi.ToString()) liste.Add(new string[] { this.Key.BitirmeTarihi, yeni.bitirmeTarihi.ToString(), eski.bitirmeTarihi.ToString() });
                if (yeni.belgeNo != eski.belgeNo) liste.Add(new string[] { this.Key.BelgeNo, yeni.belgeNo, eski.belgeNo });
                if (yeni.derece != eski.derece) liste.Add(new string[] { this.Key.Derece, yeni.derece.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.derece.ToString() : "" });
                if (yeni.belgeTarihi.ToString() != eski.belgeTarihi.ToString()) liste.Add(new string[] { this.Key.BelgeTarihi, yeni.belgeTarihi.ToString(), eski.belgeTarihi.ToString() });
                if (yeni.gecerli != eski.gecerli) liste.Add(new object[] { this.Key.Gecerli, new string[] { yeni.gecerli.ToString(), GetKodAd(yeni.gecerli, EnumKodAd.SonOkul) }, new string[] { eski.gecerli.ToString(), GetKodAd(eski.gecerli, EnumKodAd.SonOkul) } });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, islem == ENUMTARIHCEISLEMTIP.KAYDET ? new object[] { new string[] { TagList.YeniKayit, "" } } : new object[] { new string[] { TagList.OkulAd, eski.okulAdi } });


                return table;
            }

        }
        public class OgrenimKey
        {
            public string TcKimlikNo { get; protected set; }
            public string OgrenimKod { get; protected set; }
            public string OkulAdi { get; protected set; }
            public string BaslangicTarihi { get; protected set; }
            public string BitirmeTarihi { get; protected set; }
            public string BelgeNo { get; protected set; }
            public string Derece { get; protected set; }
            public string BelgeTarihi { get; protected set; }
            public string Gecerli { get; protected set; }

            public OgrenimKey()
            {
                TcKimlikNo = "TcKimlikNo";
                OgrenimKod = "OgrenimKod";
                OkulAdi = "OkulAdi";
                BaslangicTarihi = "BaslangicTarihi";
                BitirmeTarihi = "BitirmeTarihi";
                BelgeNo = "BelgeNo";
                Derece = "Derece";
                BelgeTarihi = "BelgeTarihi";
                Gecerli = "Gecerli";
            }
        }
        public class OgrenimKeyAd : OgrenimKey
        {
            public OgrenimKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.OgrenimKod = "Eğitim Seviyesi";
                base.OkulAdi = "Okul Adı";
                base.BaslangicTarihi = "Başlangıc Tarihi";
                base.BitirmeTarihi = "Bitirme Tarihi";
                base.BelgeNo = "Belge No";
                base.Derece = "Derece";
                base.BelgeTarihi = "Belge Tarihi";
                base.Gecerli = "En son bitirdiği okul";
            }
        }

        #endregion

        #region "Yabancı Dil"

        public class DilX
        {
            public DilKey Key { get; protected set; }
            public DilKeyAd KeyAd { get; protected set; }

            public DilX()
            {
                this.Key = new DilKey();
                this.KeyAd = new DilKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelDil yeni, PersonelDil eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();
                ObjectArray listeDil = new PERDil().Listele(new Dil());

                if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.dilKod != eski.dilKod) liste.Add(new object[] { this.Key.DilKod, new string[] { yeni.dilKod.ToString(), GetKodAd(yeni.dilKod, EnumKodAd.Dil, listeDil) }, new string[] { eski.dilKod.ToString(), GetKodAd(eski.dilKod, EnumKodAd.Dil, listeDil) } });
                if (yeni.duzey != eski.duzey) liste.Add(new string[] { this.Key.Duzey, yeni.duzey, eski.duzey });
                if (yeni.puan != eski.puan) liste.Add(new string[] { this.Key.Puan, yeni.puan.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.puan.ToString() : "" });
                if (yeni.donemAy != eski.donemAy) liste.Add(new object[] { this.Key.DonemAy, new string[] { yeni.donemAy.ToString(), GetKodAd(yeni.donemAy, EnumKodAd.Ay) }, new string[] { eski.donemAy.ToString(), GetKodAd(eski.donemAy, EnumKodAd.Ay) } });
                if (yeni.donemYil != eski.donemYil) liste.Add(new string[] { this.Key.DonemYil, yeni.donemYil.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.donemYil.ToString() : "" });
                if (yeni.belgeTur != eski.belgeTur) liste.Add(new object[] { this.Key.BelgeTur, new string[] { yeni.belgeTur.ToString(), GetKodAd(yeni.belgeTur, EnumKodAd.DilBelgeTur) }, new string[] { eski.belgeTur.ToString(), GetKodAd(eski.belgeTur, EnumKodAd.DilBelgeTur) } });
                if (yeni.belgeTarihi.ToString() != eski.belgeTarihi.ToString()) liste.Add(new string[] { this.Key.BelgeTarihi, yeni.belgeTarihi.ToString(), eski.belgeTarihi.ToString() });
                if (yeni.belgeNo != eski.belgeNo) liste.Add(new string[] { this.Key.BelgeNo, yeni.belgeNo, eski.belgeNo });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, islem == ENUMTARIHCEISLEMTIP.KAYDET ? new object[] { new string[] { TagList.YeniKayit, "" } } : new object[] { new string[] { TagList.Dil, GetKodAd(yeni.dilKod, EnumKodAd.Dil, listeDil) } });


                return table;
            }

        }
        public class DilKey
        {
            public string TcKimlikNo { get; protected set; }
            public string DilKod { get; protected set; }
            public string Duzey { get; protected set; }
            public string Puan { get; protected set; }
            public string DonemAy { get; protected set; }
            public string DonemYil { get; protected set; }
            public string BelgeTur { get; protected set; }
            public string BelgeTarihi { get; protected set; }
            public string BelgeNo { get; protected set; }

            public DilKey()
            {
                TcKimlikNo = "TcKimlikNo";
                DilKod = "DilKod";
                Duzey = "Duzey";
                Puan = "Puan";
                DonemAy = "DonemAy";
                DonemYil = "DonemYil";
                BelgeTur = "BelgeTur";
                BelgeTarihi = "BelgeTarihi";
                BelgeNo = "BelgeNo";
            }
        }
        public class DilKeyAd : DilKey
        {
            public DilKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.DilKod = "Yabancı Dil";
                base.Duzey = "Düzey";
                base.Puan = "Puan";
                base.DonemAy = "Dönem Ay";
                base.DonemYil = "Dönem Yıl";
                base.BelgeTur = "Belge Türü";
                base.BelgeTarihi = "Belge Tarihi";
                base.BelgeNo = "Belge No";
            }
        }

        #endregion

        #region "İzin Hareket"

        public class IzinX
        {
            public IzinKey Key { get; protected set; }
            public IzinKeyAd KeyAd { get; protected set; }

            public IzinX()
            {
                this.Key = new IzinKey();
                this.KeyAd = new IzinKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelIzin yeni, PersonelIzin eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();

                if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.tip != eski.tip) liste.Add(new object[] { this.Key.Tip, new string[] { yeni.tip.ToString(), GetKodAd(yeni.tip, EnumKodAd.IzinTip) }, new string[] { eski.tip.ToString(), GetKodAd(eski.tip, EnumKodAd.IzinTip) } });
                if (yeni.baslangicTarihi.ToString() != eski.baslangicTarihi.ToString()) liste.Add(new string[] { this.Key.BaslangicTarihi, yeni.baslangicTarihi.ToString(), eski.baslangicTarihi.ToString() });
                if (yeni.bitisTarihi.ToString() != eski.bitisTarihi.ToString()) liste.Add(new string[] { this.Key.BitisTarihi, yeni.bitisTarihi.ToString(), eski.bitisTarihi.ToString() });
                if (yeni.izinNotu != eski.izinNotu) liste.Add(new string[] { this.Key.IzinNotu, yeni.izinNotu, eski.izinNotu });
                if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.Aciklama, yeni.aciklama, eski.aciklama });
                if (yeni.telefon != eski.telefon) liste.Add(new string[] { this.Key.Telefon, yeni.telefon, eski.telefon });
                if (yeni.kullanilanGun != eski.kullanilanGun) liste.Add(new string[] { this.Key.KullanilanGun, yeni.kullanilanGun.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kullanilanGun.ToString() : "" });
                if (yeni.onay != eski.onay) liste.Add(new string[] { this.Key.Onay, yeni.onay.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.onay.ToString() : "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, islem == ENUMTARIHCEISLEMTIP.KAYDET ? new object[] { new string[] { TagList.YeniKayit, "" } } : new object[] { new string[] { TagList.Izin, GetKodAd(yeni.tip, EnumKodAd.IzinTip) }, new string[] { TagList.SiraNo, eski.sirano.ToString() } });

                return table;
            }

        }
        public class IzinKey
        {
            public string TcKimlikNo { get; protected set; }
            public string Tip { get; protected set; }
            public string BaslangicTarihi { get; protected set; }
            public string BitisTarihi { get; protected set; }
            public string IzinNotu { get; protected set; }
            public string Aciklama { get; protected set; }
            public string Telefon { get; protected set; }
            public string KullanilanGun { get; protected set; }
            public string Onay { get; protected set; }

            public IzinKey()
            {
                TcKimlikNo = "TcKimlikNo";
                Tip = "Tip";
                BaslangicTarihi = "BaslangicTarihi";
                BitisTarihi = "BitisTarihi";
                IzinNotu = "IzinNotu";
                Aciklama = "Aciklama";
                Telefon = "Telefon";
                KullanilanGun = "KullanilanGun";
                Onay = "Onay";
            }
        }
        public class IzinKeyAd : IzinKey
        {
            public IzinKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.Tip = "İzin Türü";
                base.BaslangicTarihi = "İznin Başlangıç Tarihi";
                base.BitisTarihi = "Göreve Başlama Tarihi";
                base.IzinNotu = "İznin Kullanılacağı Adres";
                base.Aciklama = "Açıklama";
                base.Telefon = "Telefon";
                base.KullanilanGun = "Kullanılan Gün";
                base.Onay = "Onay";
            }
        }

        #endregion

        #region "Memuriyet"

        public class MemuriyetX
        {
            public MemuriyetKey Key { get; protected set; }
            public MemuriyetKeyAd KeyAd { get; protected set; }

            public MemuriyetX()
            {
                this.Key = new MemuriyetKey();
                this.KeyAd = new MemuriyetKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelMemuriyet yeni, PersonelMemuriyet eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();
                ObjectArray listeNeden = new PERNeden().Listele(new Neden());

                if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.birimAd != eski.birimAd) liste.Add(new string[] { this.Key.BirimAd, yeni.birimAd, eski.birimAd });
                if (yeni.gorev != eski.gorev) liste.Add(new string[] { this.Key.Gorev, yeni.gorev, eski.gorev });
                if (yeni.istihdamSekli != eski.istihdamSekli) liste.Add(new object[] { this.Key.IstihdamSekli, new string[] { yeni.istihdamSekli.ToString(), GetKodAd(yeni.istihdamSekli, EnumKodAd.IstihdamSekli) }, new string[] { eski.istihdamSekli.ToString(), GetKodAd(eski.istihdamSekli, EnumKodAd.IstihdamSekli) } });
                if (yeni.baslamaTarihi.ToString() != eski.baslamaTarihi.ToString()) liste.Add(new string[] { this.Key.BaslamaTarihi, yeni.baslamaTarihi.ToString(), eski.baslamaTarihi.ToString() });
                if (yeni.ayrilmaTarihi.ToString() != eski.ayrilmaTarihi.ToString()) liste.Add(new string[] { this.Key.AyrilmaTarihi, yeni.ayrilmaTarihi.ToString(), eski.ayrilmaTarihi.ToString() });
                if (yeni.baslamaNedeni != eski.baslamaNedeni) liste.Add(new object[] { this.Key.BaslamaNedeni, new string[] { yeni.baslamaNedeni.ToString(), GetKodAd(yeni.baslamaNedeni, EnumKodAd.BaslamaNedeni, listeNeden) }, new string[] { eski.baslamaNedeni.ToString(), GetKodAd(eski.baslamaNedeni, EnumKodAd.BaslamaNedeni, listeNeden) } });
                if (yeni.ayrilmaNedeni != eski.ayrilmaNedeni) liste.Add(new object[] { this.Key.AyrilmaNedeni, new string[] { yeni.ayrilmaNedeni.ToString(), GetKodAd(yeni.ayrilmaNedeni, EnumKodAd.AyrilmaNedeni, listeNeden) }, new string[] { eski.ayrilmaNedeni.ToString(), GetKodAd(eski.ayrilmaNedeni, EnumKodAd.AyrilmaNedeni, listeNeden) } });
                if (yeni.evrakNo != eski.evrakNo) liste.Add(new string[] { this.Key.EvrakNo, yeni.evrakNo, eski.evrakNo });
                if (yeni.yasalDayanak != eski.yasalDayanak) liste.Add(new string[] { this.Key.YasalDayanak, yeni.yasalDayanak, eski.yasalDayanak });
                if (yeni.onayTarih.ToString() != eski.onayTarih.ToString()) liste.Add(new string[] { this.Key.OnayTarih, yeni.onayTarih.ToString(), eski.onayTarih.ToString() });
                if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.Aciklama, yeni.aciklama, eski.aciklama });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, islem == ENUMTARIHCEISLEMTIP.KAYDET ? new object[] { new string[] { TagList.YeniKayit, "" } } : new object[] { new string[] { TagList.Birim, eski.birimAd }, new string[] { TagList.SiraNo, eski.siraNo.ToString() } });

                return table;
            }

        }
        public class MemuriyetKey
        {
            public string TcKimlikNo { get; protected set; }
            public string BirimAd { get; protected set; }
            public string Gorev { get; protected set; }
            public string IstihdamSekli { get; protected set; }
            public string BaslamaTarihi { get; protected set; }
            public string AyrilmaTarihi { get; protected set; }
            public string BaslamaNedeni { get; protected set; }
            public string AyrilmaNedeni { get; protected set; }
            public string EvrakNo { get; protected set; }
            public string YasalDayanak { get; protected set; }
            public string OnayTarih { get; protected set; }
            public string Aciklama { get; protected set; }

            public MemuriyetKey()
            {
                TcKimlikNo = "TcKimlikNo";
                BirimAd = "BirimAd";
                Gorev = "Gorev";
                IstihdamSekli = "IstihdamSekli";
                BaslamaTarihi = "BaslamaTarihi";
                AyrilmaTarihi = "AyrilmaTarihi";
                BaslamaNedeni = "BaslamaNedeni";
                AyrilmaNedeni = "AyrilmaNedeni";
                EvrakNo = "EvrakNo";
                YasalDayanak = "YasalDayanak";
                OnayTarih = "OnayTarih";
                Aciklama = "Aciklama";
            }
        }
        public class MemuriyetKeyAd : MemuriyetKey
        {
            public MemuriyetKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.BirimAd = "Birimi";
                base.Gorev = "Görevi";
                base.IstihdamSekli = "İstihdam Şekli";
                base.BaslamaTarihi = "Başlama Tarihi";
                base.AyrilmaTarihi = "Ayrılma Tarihi";
                base.BaslamaNedeni = "Başlama Nedeni";
                base.AyrilmaNedeni = "Ayrılma Nedeni";
                base.EvrakNo = "Evrak No";
                base.YasalDayanak = "Yasal Dayanak";
                base.OnayTarih = "Onay Tarihi";
                base.Aciklama = "Açıklama";
            }
        }

        #endregion

        #region "Kadro"

        public class KadroX
        {
            public KadroKey Key { get; protected set; }
            public KadroKeyAd KeyAd { get; protected set; }

            public KadroX()
            {
                this.Key = new KadroKey();
                this.KeyAd = new KadroKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelKadro yeni, PersonelKadro eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();
                ObjectArray listeUnvan = new PERUnvan().Listele(new Unvan());
                ObjectArray listeSinif = new PERSinif().Listele(new Sinif());

                if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.birimAd != eski.birimAd) liste.Add(new string[] { this.Key.BirimAd, yeni.birimAd, eski.birimAd });
                if (yeni.unvan != eski.unvan) liste.Add(new object[] { this.Key.Unvan, new string[] { yeni.unvan.ToString(), GetKodAd(yeni.unvan, EnumKodAd.Unvan, listeUnvan) }, new string[] { eski.unvan.ToString(), GetKodAd(eski.unvan, EnumKodAd.Unvan, listeUnvan) } });
                if (yeni.sinifi != eski.sinifi) liste.Add(new object[] { this.Key.Sinif, new string[] { yeni.sinifi.ToString(), GetKodAd(yeni.sinifi, EnumKodAd.Sinif, listeSinif) }, new string[] { eski.sinifi.ToString(), GetKodAd(eski.sinifi, EnumKodAd.Sinif, listeSinif) } });
                if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.Aciklama, yeni.aciklama, eski.aciklama });
                if (yeni.kadroDerece != eski.kadroDerece) liste.Add(new string[] { this.Key.KadroDerece, yeni.kadroDerece.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kadroDerece.ToString() : "" });
                if (yeni.kadroTarih.ToString() != eski.kadroTarih.ToString()) liste.Add(new string[] { this.Key.KadroTarih, yeni.kadroTarih.ToString(), eski.kadroTarih.ToString() });
                if (yeni.kazanilmisHakDerece != eski.kazanilmisHakDerece) liste.Add(new string[] { this.Key.KazanilmisHakDerece, yeni.kazanilmisHakDerece.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kazanilmisHakDerece.ToString() : "" });
                if (yeni.kazanilmisHakKademe != eski.kazanilmisHakKademe) liste.Add(new string[] { this.Key.KazanilmisHakKademe, yeni.kazanilmisHakKademe.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kazanilmisHakKademe.ToString() : "" });
                if (yeni.kazanilmisHakTarih.ToString() != eski.kazanilmisHakTarih.ToString()) liste.Add(new string[] { this.Key.KazanilmisHakTarih, yeni.kazanilmisHakTarih.ToString(), eski.kazanilmisHakTarih.ToString() });
                if (yeni.kazanilmisGosterge.ToString() != eski.kazanilmisGosterge.ToString()) liste.Add(new string[] { this.Key.KazanilmisGosterge, yeni.kazanilmisGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kazanilmisGosterge.ToString() : "" });
                if (yeni.kazanilmisEkGosterge.ToString() != eski.kazanilmisEkGosterge.ToString()) liste.Add(new string[] { this.Key.KazanilmisEkGosterge, yeni.kazanilmisEkGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kazanilmisEkGosterge.ToString() : "" });
                if (yeni.odemeyeEsasDerece != eski.odemeyeEsasDerece) liste.Add(new string[] { this.Key.OdemeyeEsasDerece, yeni.odemeyeEsasDerece.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.odemeyeEsasDerece.ToString() : "" });
                if (yeni.odemeyeEsasKademe != eski.odemeyeEsasKademe) liste.Add(new string[] { this.Key.OdemeyeEsasKademe, yeni.odemeyeEsasKademe.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.odemeyeEsasKademe.ToString() : "" });
                if (yeni.odemeyeEsasTarih.ToString() != eski.odemeyeEsasTarih.ToString()) liste.Add(new string[] { this.Key.OdemeyeEsasTarih, yeni.odemeyeEsasTarih.ToString(), eski.odemeyeEsasTarih.ToString() });
                if (yeni.odemeyeEsasGosterge.ToString() != eski.odemeyeEsasGosterge.ToString()) liste.Add(new string[] { this.Key.OdemeyeEsasGosterge, yeni.odemeyeEsasGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.odemeyeEsasGosterge.ToString() : "" });
                if (yeni.odemeyeEsasEkGosterge.ToString() != eski.odemeyeEsasEkGosterge.ToString()) liste.Add(new string[] { this.Key.OdemeyeEsasEkGosterge, yeni.odemeyeEsasEkGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.odemeyeEsasEkGosterge.ToString() : "" });
                if (yeni.emekliyeEsasDerece != eski.emekliyeEsasDerece) liste.Add(new string[] { this.Key.EmekliyeEsasDerece, yeni.emekliyeEsasDerece.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.emekliyeEsasDerece.ToString() : "" });
                if (yeni.emekliyeEsasKademe != eski.emekliyeEsasKademe) liste.Add(new string[] { this.Key.EmekliyeEsasKademe, yeni.emekliyeEsasKademe.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.emekliyeEsasKademe.ToString() : "" });
                if (yeni.emekliyeEsasTarih.ToString() != eski.emekliyeEsasTarih.ToString()) liste.Add(new string[] { this.Key.EmekliyeEsasTarih, yeni.emekliyeEsasTarih.ToString(), eski.emekliyeEsasTarih.ToString() });
                if (yeni.emekliyeEsasGosterge.ToString() != eski.emekliyeEsasGosterge.ToString()) liste.Add(new string[] { this.Key.EmekliyeEsasGosterge, yeni.emekliyeEsasGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.emekliyeEsasGosterge.ToString() : "" });
                if (yeni.emekliyeEsasEkGosterge.ToString() != eski.emekliyeEsasEkGosterge.ToString()) liste.Add(new string[] { this.Key.EmekliyeEsasEkGosterge, yeni.emekliyeEsasEkGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.emekliyeEsasEkGosterge.ToString() : "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, islem == ENUMTARIHCEISLEMTIP.KAYDET ? new object[] { new string[] { TagList.YeniKayit, "" } } : new object[] { new string[] { TagList.Birim, eski.birimAd }, new string[] { TagList.SiraNo, eski.siraNo.ToString() } });

                return table;
            }

        }
        public class KadroKey
        {
            public string TcKimlikNo { get; protected set; }
            public string BirimAd { get; protected set; }
            public string Unvan { get; protected set; }
            public string Sinif { get; protected set; }
            public string Aciklama { get; protected set; }
            public string KadroDerece { get; protected set; }
            public string KadroTarih { get; protected set; }
            public string KazanilmisHakDerece { get; protected set; }
            public string KazanilmisHakKademe { get; protected set; }
            public string KazanilmisHakTarih { get; protected set; }
            public string KazanilmisGosterge { get; protected set; }
            public string KazanilmisEkGosterge { get; protected set; }
            public string OdemeyeEsasDerece { get; protected set; }
            public string OdemeyeEsasKademe { get; protected set; }
            public string OdemeyeEsasTarih { get; protected set; }
            public string OdemeyeEsasGosterge { get; protected set; }
            public string OdemeyeEsasEkGosterge { get; protected set; }
            public string EmekliyeEsasDerece { get; protected set; }
            public string EmekliyeEsasKademe { get; protected set; }
            public string EmekliyeEsasTarih { get; protected set; }
            public string EmekliyeEsasGosterge { get; protected set; }
            public string EmekliyeEsasEkGosterge { get; protected set; }

            public KadroKey()
            {
                TcKimlikNo = "TcKimlikNo";
                BirimAd = "BirimAd";
                Unvan = "Unvan";
                Sinif = "Sinif";
                Aciklama = "Aciklama";
                KadroDerece = "KadroDerece";
                KadroTarih = "KadroTarih";
                KazanilmisHakDerece = "KazanilmisHakDerece";
                KazanilmisHakKademe = "KazanilmisHakKademe";
                KazanilmisHakTarih = "KazanilmisHakTarih";
                KazanilmisGosterge = "KazanilmisGosterge";
                KazanilmisEkGosterge = "KazanilmisEkGosterge";
                OdemeyeEsasDerece = "OdemeyeEsasDerece";
                OdemeyeEsasKademe = "OdemeyeEsasKademe";
                OdemeyeEsasTarih = "OdemeyeEsasTarih";
                OdemeyeEsasGosterge = "OdemeyeEsasGosterge";
                OdemeyeEsasEkGosterge = "OdemeyeEsasEkGosterge";
                EmekliyeEsasDerece = "EmekliyeEsasDerece";
                EmekliyeEsasKademe = "EmekliyeEsasKademe";
                EmekliyeEsasTarih = "EmekliyeEsasTarih";
                EmekliyeEsasGosterge = "EmekliyeEsasGosterge";
                EmekliyeEsasEkGosterge = "EmekliyeEsasEkGosterge";
            }
        }
        public class KadroKeyAd : KadroKey
        {
            public KadroKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.BirimAd = "Birimi";
                base.Unvan = "Unvanı";
                base.Sinif = "Sınıf";
                base.Aciklama = "Açıklama";
                base.KadroDerece = "Kadro Derece";
                base.KadroTarih = "Kadro Tarih";
                base.KazanilmisHakDerece = "Kazanılmış Hak Derece";
                base.KazanilmisHakKademe = "Kazanılmış Hak Kademe";
                base.KazanilmisHakTarih = "Kazanılmış Hak Tarih";
                base.KazanilmisGosterge = "Kazanılmış Hak Gösterge";
                base.KazanilmisEkGosterge = "Kazanılmış Ek Gösterge";
                base.OdemeyeEsasDerece = "Ödemeye Esas Derece";
                base.OdemeyeEsasKademe = "Ödemeye Esas Kademe";
                base.OdemeyeEsasTarih = "Ödemeye Esas Tarih";
                base.OdemeyeEsasGosterge = "Ödemeye Esas Gösterge";
                base.OdemeyeEsasEkGosterge = "Ödemeye Esas Ek Gösterge";
                base.EmekliyeEsasDerece = "Emekliye Esas Derece";
                base.EmekliyeEsasKademe = "Emekliye Esas Kademe";
                base.EmekliyeEsasTarih = "Emekliye Esas Tarih";
                base.EmekliyeEsasGosterge = "Emekliye Esas Gösterge";
                base.EmekliyeEsasEkGosterge = "Emekliye Esas Ek Gösterge";

            }
        }

        #endregion

        #region "Hizmet İçi Eğitim"

        public class KursX
        {
            public KursKey Key { get; protected set; }
            public KursKeyAd KeyAd { get; protected set; }

            public KursX()
            {
                this.Key = new KursKey();
                this.KeyAd = new KursKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelKurs yeni, PersonelKurs eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();
                ObjectArray listeKurs = new PERKurs().Listele(new Kurs());

                if (islem == ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.kursKod != eski.kursKod) liste.Add(new object[] { this.Key.KursKod, new string[] { yeni.kursKod.ToString(), GetKodAd(yeni.kursKod, EnumKodAd.Kurs, listeKurs) }, new string[] { eski.kursKod.ToString(), GetKodAd(eski.kursKod, EnumKodAd.Kurs, listeKurs) } });
                if (yeni.kursAdi != eski.kursAdi) liste.Add(new string[] { this.Key.KursAdi, yeni.kursAdi, eski.kursAdi });
                if (yeni.baslamaTarihi.ToString() != eski.baslamaTarihi.ToString()) liste.Add(new string[] { this.Key.BaslamaTarihi, yeni.baslamaTarihi.ToString(), eski.baslamaTarihi.ToString() });
                if (yeni.bitirmeTarihi.ToString() != eski.bitirmeTarihi.ToString()) liste.Add(new string[] { this.Key.BitirmeTarihi, yeni.bitirmeTarihi.ToString(), eski.bitirmeTarihi.ToString() });
                if (yeni.aldigiYer != eski.aldigiYer) liste.Add(new string[] { this.Key.AldigiYer, yeni.aldigiYer, eski.aldigiYer });
                if (yeni.egitimYeri != eski.egitimYeri) liste.Add(new string[] { this.Key.EgitimYeri, yeni.egitimYeri, eski.egitimYeri });
                if (yeni.gun != eski.gun) liste.Add(new string[] { this.Key.Gun, yeni.gun.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.gun.ToString() : "" });
                if (yeni.dersSaati != eski.dersSaati) liste.Add(new string[] { this.Key.DersSaati, yeni.dersSaati.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.dersSaati.ToString() : "" });
                if (yeni.yil != eski.yil) liste.Add(new string[] { this.Key.Yil, yeni.yil.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.yil.ToString() : "" });
                if (yeni.donem != eski.donem) liste.Add(new string[] { this.Key.Donem, yeni.donem.ToString(), eski.donem.ToString() });
                if (yeni.derece != eski.derece) liste.Add(new string[] { this.Key.Derece, yeni.derece.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.derece.ToString() : "" });
                if (yeni.dosyaNo != eski.dosyaNo) liste.Add(new string[] { this.Key.DosyaNo, yeni.dosyaNo.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.dosyaNo.ToString() : "" });
                if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.Aciklama, yeni.aciklama, eski.aciklama });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                if (islem == ENUMTARIHCEISLEMTIP.KAYDET)
                    table.Add(TagList.Bilgi, new object[] { new string[] { TagList.YeniKayit, "" } });
                else if (islem == ENUMTARIHCEISLEMTIP.SIL)
                    table.Add(TagList.Bilgi, new object[] { new string[] { TagList.Silindi, "" } });
                else
                    table.Add(TagList.Bilgi, new object[] { new string[] { TagList.Ad, eski.kursAdi }, new string[] { TagList.SiraNo, eski.siraNo.ToString() } });
                //BARIS: table.Add(TagList.Bilgi, eski.tcKimlikNo == "-1" ? new object[] { new string[] { TagList.YeniKayit, "" } } : eski.tcKimlikNo == "-2" ? new object[] { new string[] { TagList.Silindi, "" } } : new object[] { new string[] { TagList.Ad, eski.kursAdi }, new string[] { TagList.SiraNo, eski.siraNo.ToString() } });

                return table;
            }

        }
        public class KursKey
        {
            public string TcKimlikNo { get; protected set; }
            public string KursKod { get; protected set; }
            public string KursAdi { get; protected set; }
            public string BaslamaTarihi { get; protected set; }
            public string BitirmeTarihi { get; protected set; }
            public string AldigiYer { get; protected set; }
            public string EgitimYeri { get; protected set; }
            public string Gun { get; protected set; }
            public string DersSaati { get; protected set; }
            public string Yil { get; protected set; }
            public string Donem { get; protected set; }
            public string Derece { get; protected set; }
            public string DosyaNo { get; protected set; }
            public string Aciklama { get; protected set; }

            public KursKey()
            {
                TcKimlikNo = "TcKimlikNo";
                KursKod = "KursKod";
                KursAdi = "KursAdi";
                BaslamaTarihi = "BaslamaTarihi";
                BitirmeTarihi = "BitirmeTarihi";
                AldigiYer = "AldigiYer";
                EgitimYeri = "EgitimYeri";
                Gun = "Gun";
                DersSaati = "DersSaati";
                Yil = "Yil";
                Donem = "Donem";
                Derece = "Derece";
                DosyaNo = "DosyaNo";
                Aciklama = "Aciklama";
            }
        }
        public class KursKeyAd : KursKey
        {
            public KursKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.KursKod = "Eğitim Tipi";
                base.KursAdi = "Adı";
                base.BaslamaTarihi = "Başlama Tarihi";
                base.BitirmeTarihi = "Bitirme Tarihi";
                base.AldigiYer = "Eğitimi Veren Kuruluş";
                base.EgitimYeri = "Eğitim Yeri";
                base.Gun = "Eğitim Süresi - Gün";
                base.DersSaati = "Eğitim Süresi - Saat";
                base.Yil = "Yıl";
                base.Donem = "Dönemi";
                base.Derece = "Puan";
                base.DosyaNo = "Dosya Kodu";
                base.Aciklama = "Açıklama";
            }
        }

        #endregion

        #region "Ceza ve Ödül"

        public class CezaOdulX
        {
            public CezaOdulKey Key { get; protected set; }
            public CezaOdulKeyAd KeyAd { get; protected set; }

            public CezaOdulX()
            {
                this.Key = new CezaOdulKey();
                this.KeyAd = new CezaOdulKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelCezaOdul yeni, PersonelCezaOdul eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();

                ObjectArray yeniTip = new ObjectArray(); yeniTip.objeler.Add(yeni.tip);
                ObjectArray eskiTip = new ObjectArray(); eskiTip.objeler.Add(eski.tip);

                if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.tip != eski.tip) liste.Add(new object[] { this.Key.Tip, new string[] { yeni.tip.ToString(), GetKodAd(yeni.tip, EnumKodAd.CezaOdul) }, new string[] { eski.tip.ToString(), GetKodAd(eski.tip, EnumKodAd.CezaOdul) } });
                if (yeni.cezaodultip != eski.cezaodultip) liste.Add(new object[] { this.Key.CezaOdulTip, new string[] { yeni.cezaodultip.ToString(), GetKodAd(yeni.cezaodultip, EnumKodAd.CezaOdulTip, yeniTip) }, new string[] { eski.tip.ToString(), GetKodAd(eski.tip, EnumKodAd.CezaOdul, eskiTip) } });
                if (yeni.tarih.ToString() != eski.tarih.ToString()) liste.Add(new string[] { this.Key.Tarih, yeni.tarih.ToString(), eski.tarih.ToString() });
                if (yeni.verilisNedeni != eski.verilisNedeni) liste.Add(new string[] { this.Key.VerilisNedeni, yeni.verilisNedeni, eski.verilisNedeni });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, islem == ENUMTARIHCEISLEMTIP.KAYDET ? new object[] { new string[] { TagList.YeniKayit, "" } } : new object[] { new string[] { TagList.CezaOdul, GetKodAd(eski.tip, EnumKodAd.CezaOdul) }, new string[] { TagList.SiraNo, eski.sirano.ToString() } });

                return table;
            }

        }
        public class CezaOdulKey
        {
            public string TcKimlikNo { get; protected set; }
            public string Tip { get; protected set; }
            public string CezaOdulTip { get; protected set; }
            public string Tarih { get; protected set; }
            public string VerilisNedeni { get; protected set; }

            public CezaOdulKey()
            {
                TcKimlikNo = "TcKimlikNo";
                Tip = "Tip";
                CezaOdulTip = "CezaOdulTip";
                Tarih = "Tarih";
                VerilisNedeni = "VerilisNedeni";
            }
        }
        public class CezaOdulKeyAd : CezaOdulKey
        {
            public CezaOdulKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.Tip = "Ceza / Ödül";
                base.CezaOdulTip = "Tipi";
                base.Tarih = "Tarihi";
                base.VerilisNedeni = "Veriliş Nedeni";
            }
        }

        #endregion

        #region "Mal Bildirimi"

        public class MalBildirimiX
        {
            public MalBildirimiKey Key { get; protected set; }
            public MalBildirimiKeyAd KeyAd { get; protected set; }

            public MalBildirimiX()
            {
                this.Key = new MalBildirimiKey();
                this.KeyAd = new MalBildirimiKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelMalBildirimi yeni, PersonelMalBildirimi eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();

                if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.tarih.ToString() != eski.tarih.ToString()) liste.Add(new string[] { this.Key.Tarih, yeni.tarih.ToString(), eski.tarih.ToString() });
                if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.Aciklama, yeni.aciklama, eski.aciklama });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, islem == ENUMTARIHCEISLEMTIP.KAYDET ? new object[] { new string[] { TagList.YeniKayit, "" } } : new object[] { new string[] { TagList.Tarih, eski.tarih.ToString() }, new string[] { TagList.SiraNo, eski.sirano.ToString() } });

                return table;
            }

        }
        public class MalBildirimiKey
        {
            public string TcKimlikNo { get; protected set; }
            public string Tarih { get; protected set; }
            public string Aciklama { get; protected set; }

            public MalBildirimiKey()
            {
                TcKimlikNo = "TcKimlikNo";
                Tarih = "Tarih";
                Aciklama = "Aciklama";
            }
        }
        public class MalBildirimiKeyAd : MalBildirimiKey
        {
            public MalBildirimiKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.Tarih = "Tarihi";
                base.Aciklama = "Açıklama";
            }
        }

        #endregion

        #region "İzin Devir"

        public class IzinDevirX
        {
            public IzinDevirKey Key { get; protected set; }
            public IzinDevirKeyAd KeyAd { get; protected set; }

            public IzinDevirX()
            {
                this.Key = new IzinDevirKey();
                this.KeyAd = new IzinDevirKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelIzinDevir yeni, PersonelIzinDevir eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();

                if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                if (yeni.yil.ToString() != eski.yil.ToString()) liste.Add(new string[] { this.Key.Yil, yeni.yil.ToString(), eski.yil.ToString() });
                if (yeni.devreden != eski.devreden) liste.Add(new string[] { this.Key.Devreden, yeni.devreden.ToString(), eski.devreden.ToString() });
                if (yeni.yilici != eski.yilici) liste.Add(new string[] { this.Key.Yilici, yeni.yilici.ToString(), eski.yilici.ToString() });
                //if (yeni.ad != eski.ad) liste.Add(new string[] { this.Key.Ad, yeni.ad, eski.ad });
                //if (yeni.soyad != eski.soyad) liste.Add(new string[] { this.Key.Soyad, yeni.soyad, eski.soyad });
                //if (yeni.sicilNo != eski.sicilNo) liste.Add(new string[] { this.Key.SicilNo, yeni.sicilNo, eski.sicilNo });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);

                return table;
            }

        }
        public class IzinDevirKey
        {
            public string TcKimlikNo { get; protected set; }
            public string Yil { get; protected set; }
            public string Devreden { get; protected set; }
            public string Yilici { get; protected set; }
            public string Ad { get; protected set; }
            public string Soyad { get; protected set; }
            public string SicilNo { get; protected set; }

            public IzinDevirKey()
            {
                TcKimlikNo = "TcKimlikNo";
                Yil = "Yil";
                Devreden = "Devreden";
                Yilici = "Yilici";
                Ad = "Ad";
                Soyad = "Soyad";
                SicilNo = "SicilNo";
            }
        }
        public class IzinDevirKeyAd : IzinDevirKey
        {
            public IzinDevirKeyAd()
            {
                base.TcKimlikNo = "T.C. Kimlik No";
                base.Yil = "Yıl";
                base.Devreden = "Devreden";
                base.Yilici = "Yıl içi";
                base.Ad = "Adı";
                base.Soyad = "Soyadı";
                base.SicilNo = "Sicil No";
            }
        }

        #endregion
    }
}

using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using System.Collections;
using System.Xml;

namespace TNS.IKY.IsKurallari
{
    public class IKYPersonelTarihce
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
        public HizliBilgiX HizliBilgi { get; set; }
        public AdresX Adres { get; set; }
        public AileX Aile { get; set; }
        public OgrenimX Ogrenim { get; set; }
        public DilX Dil { get; set; }
        public IzinX Izin { get; set; }
        public IzinDevirX IzinDevir { get; set; }
        public MemuriyetX Memuriyet { get; set; }
        public KadroX Kadro { get; set; }
        public KursX Kurs { get; set; }
        public CezaOdulX CezaOdul { get; set; }
        public MalBildirimiX MalBildirimi { get; set; }
        public KartX Kart { get; set; }
        public AskerlikX Askerlik { get; set; }

        public IKYPersonelTarihce()
        {
            this.Genel();
        }

        public IKYPersonelTarihce(Object yeni, Object eski, ENUMTARIHCE alan, ENUMTARIHCEISLEMTIP islem)
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
            this.HizliBilgi = new HizliBilgiX();
            this.Adres = new AdresX();
            this.Aile = new AileX();
            this.Ogrenim = new OgrenimX();
            this.Dil = new DilX();
            this.Izin = new IzinX();
            this.IzinDevir = new IzinDevirX();
            this.Memuriyet = new MemuriyetX();
            this.Kadro = new KadroX();
            this.Kurs = new KursX();
            this.CezaOdul = new CezaOdulX();
            this.MalBildirimi = new MalBildirimiX();
            this.Kart = new KartX();
            this.Askerlik = new AskerlikX();
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
            public string IzinDevir { get; protected set; }
            public string Memuriyet { get; protected set; }
            public string Kadro { get; protected set; }
            public string Kurs { get; protected set; }
            public string CezaOdul { get; protected set; }
            public string MalBildirimi { get; protected set; }
            public string HizliBilgi { get; protected set; }
            public string Kart { get; protected set; }
            public string Askerlik { get; protected set; }

            public AlanKey()
            {
                this.Kisisel = "kisisel";
                this.Adres = "adres";
                this.Aile = "aile";
                this.Ogrenim = "ogrenim";
                this.Dil = "dil";
                this.Izin = "izin";
                this.IzinDevir = "izinDevir";
                this.Memuriyet = "memuriyet";
                this.Kadro = "kadro";
                this.Kurs = "kurs";
                this.CezaOdul = "cezaOdul";
                this.MalBildirimi = "malBildirimi";
                this.HizliBilgi = "hizliBilgi";
                this.Kart = "Kart";
                this.Askerlik = "Askerlik";
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
                base.IzinDevir = "İzin Devir";
                this.Memuriyet = "Memuriyet";
                this.Kadro = "Kadro";
                this.Kurs = "Hizmet İçi Eğitim";
                this.CezaOdul = "Ceza / Ödül";
                this.MalBildirimi = "Mal Bildirimi";
                this.HizliBilgi = "Hızlı Bilgi";
                this.Kart = "Kart";
                this.Askerlik = "Askerlik";
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
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, Object yeni, Object eski, ENUMTARIHCE alan, int personelKod, ENUMTARIHCEISLEMTIP islem)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                IKYPersonelTarihce tarihce = new IKYPersonelTarihce(yeni, eski, alan, islem);
                PersonelTarihce pT = new PersonelTarihce();

                pT.degisiklik = tarihce.DegisenleriGetir();

                if (tarihce.Degisen)
                {
                    TABPersonel tab = new TABPersonel(ve);
                    ve.BeginTransaction();
                    pT.kod = TABGenel.SayacDegerAlInt(ve, TABPersonel.TABLOADTARIHCE);
                    pT.personelKod = personelKod;
                    pT.kullanici = kullanan.mernis;
                    pT.tarih = new TNSDateTime(DateTime.Now);
                    pT.kullaniciAd = kullanan.ad + " " + kullanan.soyad;

                    sonuc = tab.KaydetTarihce(pT);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = PersonelResI.PER007;
                    }
                    else
                        ve.Rollback();
                }
                else
                {
                    sonuc.hataStr = "Değişiklik bulunamadı";
                    sonuc.islemSonuc = false;
                }
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.Message);
            }
            return sonuc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="personelTCKimlikNo"></param>
        /// <returns>TARIH,KATEGORI,DEGISENALAN,YENIDEGER,ESKIDEGER,DEGISIKLIKYAPAN</returns>
        public static string[] Listele(Kullanici kullanan, int personelKod)
        {
            ArrayList liste = new ArrayList();
            ObjectArray pTList = new IKYPersonel().ListeleTarihce(kullanan, new TNS.IKY.PersonelTarihce() { personelKod = personelKod });
            IKYYetki.YetkiVarMiOkumayaPerKod(kullanan, personelKod, pTList.sonuc, ENUMYetkiIslem.Tarihce);
            if (!pTList.sonuc.islemSonuc)
            {
                liste.Add(pTList.sonuc.hataStr);
                return (string[])liste.ToArray(typeof(string));
            }

            IKYPersonelTarihce tarihce = new IKYPersonelTarihce();

            XmlDocument xDoc;

            foreach (TNS.IKY.PersonelTarihce pT in pTList.objeler)
            {
                xDoc = new XmlDocument();
                xDoc.LoadXml(pT.degisiklik);
                foreach (XmlNode nodeKategori in xDoc.DocumentElement.ChildNodes)
                {
                    string kategori = tarihce.KeyAdGetir(nodeKategori.Name);
                    string aciklama1 = "", aciklama2 = "", aciklama = "";
                    for (int i = 0; i < nodeKategori.Attributes.Count; i++)
                    {
                        //(nodeKategori.Attributes[i].Name == TagList.YeniKayit) ? "<b>Yeni Kayıt</b>" : (nodeKategori.Attributes[i].Name == TagList.Silindi) ? "<b>Silindi</b>" :
                        aciklama = string.Format("<b>{0}</b>: {1}", nodeKategori.Attributes[i].Name, nodeKategori.Attributes[i].Value);
                        if (i == 0) aciklama1 = aciklama; else aciklama2 += aciklama + "</br>";
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
                case ENUMTARIHCE.HIZLIBILGI:
                    nodeAlanName = this.Key.HizliBilgi;
                    htDegisenDegerler = this.HizliBilgi.DegisenDegerleriGetir(((Personel)Eski).kod);
                    break;
                case ENUMTARIHCE.ADRES:
                    nodeAlanName = this.Key.Adres;
                    htDegisenDegerler = this.Adres.DegisenDegerleriGetir((PersonelIletisim)Yeni, (PersonelIletisim)Eski, islem);
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
                case ENUMTARIHCE.IZINDEVIR:
                    nodeAlanName = this.Key.IzinDevir;
                    htDegisenDegerler = this.IzinDevir.DegisenDegerleriGetir((PersonelIzinDevir)Yeni, (PersonelIzinDevir)Eski, islem);
                    break;
                //case ENUMTARIHCE.MEMURIYET:
                //    nodeAlanName = this.Key.Memuriyet;
                //    htDegisenDegerler = this.Memuriyet.DegisenDegerleriGetir((PersonelMemuriyet)Yeni, (PersonelMemuriyet)Eski, islem);
                //    break;
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
                case ENUMTARIHCE.KART:
                    nodeAlanName = this.Key.Kart;
                    htDegisenDegerler = this.Kart.DegisenDegerleriGetir((PersonelKart)Yeni, (PersonelKart)Eski, islem);
                    break;
                case ENUMTARIHCE.ASKERLIK:
                    nodeAlanName = this.Key.Askerlik;
                    htDegisenDegerler = this.Askerlik.DegisenDegerleriGetir((PAskerlik)Yeni, (PAskerlik)Eski, islem);
                    break;
            }

            XmlNode nodeAlan = doc.CreateElement(nodeAlanName);

            Object bilgi = (Object)htDegisenDegerler[TagList.Bilgi];
            if (bilgi != null)
            {
                object[] alanAttributes = (object[])bilgi;
                foreach (string[] item in alanAttributes)
                {
                    this.aciklama1 = doc.CreateAttribute(item[0]);
                    this.aciklama1.Value = item[1];
                    nodeAlan.Attributes.Append(aciklama1);
                }
            }

            if (((ArrayList)htDegisenDegerler[TagList.Liste]).Count > 0)
            {
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
            {
                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    this.Degisen = true;
                else
                    this.Degisen = false;
            }

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
            else if (key == this.Key.IzinDevir) { this.Alan = ENUMTARIHCE.IZINDEVIR; return this.KeyAd.IzinDevir; }
            //else if (key == this.Key.Memuriyet) { this.Alan = ENUMTARIHCE.MEMURIYET; return this.KeyAd.Memuriyet; }
            else if (key == this.Key.Kadro) { this.Alan = ENUMTARIHCE.KADRO; return this.KeyAd.Kadro; }
            else if (key == this.Key.Kurs) { this.Alan = ENUMTARIHCE.EGITIM; return this.KeyAd.Kurs; }
            else if (key == this.Key.CezaOdul) { this.Alan = ENUMTARIHCE.CEZAODUL; return this.KeyAd.CezaOdul; }
            else if (key == this.Key.MalBildirimi) { this.Alan = ENUMTARIHCE.MALBILDIRIMI; return this.KeyAd.MalBildirimi; }
            else if (key == this.Key.HizliBilgi) { this.Alan = ENUMTARIHCE.HIZLIBILGI; return this.KeyAd.HizliBilgi; }
            else if (key == this.Key.Askerlik) { this.Alan = ENUMTARIHCE.ASKERLIK; return this.KeyAd.Askerlik; }
            else if (key == this.Key.Kart) { this.Alan = ENUMTARIHCE.KART; return this.KeyAd.Kart; }

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
                    else if (key == this.Kisisel.Key.OdaKodu) return this.Kisisel.KeyAd.OdaKodu;
                    else if (key == this.Kisisel.Key.SicilNo) return this.Kisisel.KeyAd.SicilNo;
                    else if (key == this.Kisisel.Key.EmekliSicilNo) return this.Kisisel.KeyAd.EmekliSicilNo;
                    else if (key == this.Kisisel.Key.SaglikKarneNo) return this.Kisisel.KeyAd.SaglikKarneNo;
                    else if (key == this.Kisisel.Key.BakanlikKartNo) return this.Kisisel.KeyAd.BakanlikKartNo;
                    else if (key == this.Kisisel.Key.SskSicilNo) return this.Kisisel.KeyAd.SskSicilNo;
                    else if (key == this.Kisisel.Key.BagkurSicilNo) return this.Kisisel.KeyAd.BagkurSicilNo;
                    else if (key == this.Kisisel.Key.DegisiklikYok) return this.Kisisel.KeyAd.DegisiklikYok;
                    else if (key == this.Kisisel.Key.AramaKriteri) return this.Kisisel.KeyAd.AramaKriteri;
                    else if (key == this.Kisisel.Key.dini) return this.Kisisel.KeyAd.dini;
                    else if (key == this.Kisisel.Key.aktifMi) return this.Kisisel.KeyAd.aktifMi;
                    else if (key == this.Kisisel.Key.terorMagduru) return this.Kisisel.KeyAd.terorMagduru;
                    else if (key == this.Kisisel.Key.hukumlu) return this.Kisisel.KeyAd.hukumlu;
                    else if (key == this.Kisisel.Key.ilkIseBaslamaTarihi) return this.Kisisel.KeyAd.ilkIseBaslamaTarihi;
                    else if (key == this.Kisisel.Key.kurumaBaslamaTarihi) return this.Kisisel.KeyAd.kurumaBaslamaTarihi;
                    else if (key == this.Kisisel.Key.asaletTarihi) return this.Kisisel.KeyAd.asaletTarihi;
                    else if (key == this.Kisisel.Key.korunmayaMuhtac) return this.Kisisel.KeyAd.korunmayaMuhtac;
                    else if (key == this.Kisisel.Key.arsivNo) return this.Kisisel.KeyAd.arsivNo;
                    else if (key == this.Kisisel.Key.bankaIbanNo) return this.Kisisel.KeyAd.bankaIbanNo;
                    else if (key == this.Kisisel.Key.vergiDairesi) return this.Kisisel.KeyAd.vergiDairesi;
                    else if (key == this.Kisisel.Key.titr) return this.Kisisel.KeyAd.titr;
                    else if (key == this.Kisisel.Key.sosyalGuvence) return this.Kisisel.KeyAd.sosyalGuvence;
                    else if (key == this.Kisisel.Key.rumuz) return this.Kisisel.KeyAd.rumuz;
                    break;
                case ENUMTARIHCE.ADRES:
                    if (key == this.Adres.Key.Adres) return this.Adres.KeyAd.Adres;
                    else if (key == this.Adres.Key.mobilImzaTelefon) return this.Adres.KeyAd.mobilImzaTelefon;
                    else if (key == this.Adres.Key.rehberdeGozuksun) return this.Adres.KeyAd.rehberdeGozuksun;
                    else if (key == this.Adres.Key.IsTel) return this.Adres.KeyAd.IsTel;
                    else if (key == this.Adres.Key.EvTel) return this.Adres.KeyAd.EvTel;
                    else if (key == this.Adres.Key.CepTel) return this.Adres.KeyAd.CepTel;
                    else if (key == this.Adres.Key.Faks) return this.Adres.KeyAd.Faks;
                    else if (key == this.Adres.Key.Eposta) return this.Adres.KeyAd.Eposta;
                    else if (key == this.Adres.Key.Url) return this.Adres.KeyAd.Url;
                    else if (key == this.Adres.Key.DegisiklikYok) return this.Adres.KeyAd.DegisiklikYok;
                    else if (key == this.Adres.Key.tarabyaTel) return this.Adres.KeyAd.tarabyaTel;
                    else if (key == this.Adres.Key.lojmanTel) return this.Adres.KeyAd.lojmanTel;
                    else if (key == this.Adres.Key.telsiz) return this.Adres.KeyAd.telsiz;
                    else if (key == this.Adres.Key.goruntulu) return this.Adres.KeyAd.goruntulu;
                    else if (key == this.Adres.Key.rehberdeGozuksun) return this.Adres.KeyAd.rehberdeGozuksun;
                    else if (key == this.Adres.Key.cepTelefon2) return this.Adres.KeyAd.cepTelefon2;
                    break;
                case ENUMTARIHCE.AILE:
                    if (key == this.Aile.Key.TcKimlikNo) return this.Aile.KeyAd.TcKimlikNo;
                    else if (key == this.Aile.Key.Ad) return this.Aile.KeyAd.Ad;
                    else if (key == this.Aile.Key.Soyad) return this.Aile.KeyAd.Soyad;
                    else if (key == this.Aile.Key.DogumTarihi) return this.Aile.KeyAd.DogumTarihi;
                    else if (key == this.Aile.Key.Cinsiyet) return this.Aile.KeyAd.Cinsiyet;
                    else if (key == this.Aile.Key.Yakinlik) return this.Aile.KeyAd.Yakinlik;
                    else if (key == this.Aile.Key.Durum) return this.Aile.KeyAd.Durum;
                    else if (key == this.Aile.Key.Meslek) return this.Aile.KeyAd.Meslek;
                    else if (key == this.Aile.Key.OgrenimTipi) return this.Aile.KeyAd.OgrenimTipi;
                    else if (key == this.Aile.Key.bakmaklaYukumluMu) return this.Aile.KeyAd.bakmaklaYukumluMu;
                    else if (key == this.Aile.Key.bakmaklaYukumluTarihi) return this.Aile.KeyAd.bakmaklaYukumluTarihi;
                    else if (key == this.Aile.Key.AgiDahil) return this.Aile.KeyAd.AgiDahil;
                    else if (key == this.Aile.Key.DegisiklikYok) return this.Aile.KeyAd.DegisiklikYok;
                    break;
                case ENUMTARIHCE.OGRENIM:
                    if (key == this.Ogrenim.Key.UniKod) return this.Ogrenim.KeyAd.UniKod;
                    else if (key == this.Ogrenim.Key.OgrenimKod) return this.Ogrenim.KeyAd.OgrenimKod;
                    else if (key == this.Ogrenim.Key.OkulAdi) return this.Ogrenim.KeyAd.OkulAdi;
                    else if (key == this.Ogrenim.Key.BaslangicTarihi) return this.Ogrenim.KeyAd.BaslangicTarihi;
                    else if (key == this.Ogrenim.Key.BitirmeTarihi) return this.Ogrenim.KeyAd.BitirmeTarihi;
                    else if (key == this.Ogrenim.Key.BelgeNo) return this.Ogrenim.KeyAd.BelgeNo;
                    else if (key == this.Ogrenim.Key.Derece) return this.Ogrenim.KeyAd.Derece;
                    else if (key == this.Ogrenim.Key.BelgeTarihi) return this.Ogrenim.KeyAd.BelgeTarihi;
                    else if (key == this.Ogrenim.Key.Gecerli) return this.Ogrenim.KeyAd.Gecerli;
                    else if (key == this.Ogrenim.Key.BolumKod) return this.Ogrenim.KeyAd.BolumKod;
                    else if (key == this.Ogrenim.Key.FakulteKod) return this.Ogrenim.KeyAd.FakulteKod;
                    else if (key == this.Ogrenim.Key.Hazirlik) return this.Ogrenim.KeyAd.Hazirlik;
                    else if (key == this.Ogrenim.Key.Denklik) return this.Ogrenim.KeyAd.Denklik;
                    else if (key == this.Ogrenim.Key.DenklikBolum) return this.Ogrenim.KeyAd.DenklikBolum;
                    else if (key == this.Ogrenim.Key.DenklikOkulu) return this.Ogrenim.KeyAd.DenklikOkulu;
                    else if (key == this.Ogrenim.Key.DenklikTarihi) return this.Ogrenim.KeyAd.DenklikTarihi;
                    else if (key == this.Ogrenim.Key.MemuriyeteBasladigiOkul) return this.Ogrenim.KeyAd.MemuriyeteBasladigiOkul;
                    else if (key == this.Ogrenim.Key.OgrenimYeri) return this.Ogrenim.KeyAd.OgrenimYeri;
                    else if (key == this.Ogrenim.Key.Suresi) return this.Ogrenim.KeyAd.Suresi;
                    else if (key == this.Ogrenim.Key.BelgeTuru) return this.Ogrenim.KeyAd.BelgeTuru;
                    else if (key == this.Ogrenim.Key.DegisiklikYok) return this.Ogrenim.KeyAd.DegisiklikYok;
                    else if (key == this.Ogrenim.Key.kurumaIbrazTarihi) return this.Ogrenim.KeyAd.kurumaIbrazTarihi;
                    break;
                case ENUMTARIHCE.YABANCIDIL:
                    if (key == this.Dil.Key.Seviye) return this.Dil.KeyAd.Seviye;
                    else if (key == this.Dil.Key.DilKod) return this.Dil.KeyAd.DilKod;
                    else if (key == this.Dil.Key.Duzey) return this.Dil.KeyAd.Duzey;
                    else if (key == this.Dil.Key.Puan) return this.Dil.KeyAd.Puan;
                    else if (key == this.Dil.Key.DonemAy) return this.Dil.KeyAd.DonemAy;
                    else if (key == this.Dil.Key.DonemYil) return this.Dil.KeyAd.DonemYil;
                    else if (key == this.Dil.Key.BelgeTur) return this.Dil.KeyAd.BelgeTur;
                    else if (key == this.Dil.Key.BelgeTarihi) return this.Dil.KeyAd.BelgeTarihi;
                    else if (key == this.Dil.Key.BelgeNo) return this.Dil.KeyAd.BelgeNo;
                    else if (key == this.Dil.Key.Tarih) return this.Dil.KeyAd.Tarih;
                    else if (key == this.Dil.Key.DegisiklikYok) return this.Dil.KeyAd.DegisiklikYok;
                    else if (key == this.Dil.Key.bordroyaEsas) return this.Dil.KeyAd.bordroyaEsas;
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
                    else if (key == this.Izin.Key.BelgeNo) return this.Izin.KeyAd.BelgeNo;
                    else if (key == this.Izin.Key.BelgeTarihi) return this.Izin.KeyAd.BelgeTarihi;
                    else if (key == this.Izin.Key.YerineVekaletEden) return this.Izin.KeyAd.YerineVekaletEden;
                    else if (key == this.Izin.Key.UygunGoren) return this.Izin.KeyAd.UygunGoren;
                    else if (key == this.Izin.Key.SaglikKurulusu) return this.Izin.KeyAd.SaglikKurulusu;
                    else if (key == this.Izin.Key.ImzaAtan) return this.Izin.KeyAd.ImzaAtan;
                    else if (key == this.Izin.Key.DegisiklikYok) return this.Izin.KeyAd.DegisiklikYok;
                    break;
                case ENUMTARIHCE.IZINDEVIR:
                    if (key == this.IzinDevir.Key.yil) return this.IzinDevir.KeyAd.yil;
                    else if (key == this.IzinDevir.Key.devreden) return this.IzinDevir.KeyAd.devreden;
                    else if (key == this.IzinDevir.Key.AramaKriteri) return this.IzinDevir.KeyAd.AramaKriteri;
                    else if (key == this.IzinDevir.Key.DegisiklikYok) return this.IzinDevir.KeyAd.DegisiklikYok;
                    break;
                //case ENUMTARIHCE.MEMURIYET:
                //    if (key == this.Memuriyet.Key.TcKimlikNo) return this.Memuriyet.KeyAd.TcKimlikNo;
                //    else if (key == this.Memuriyet.Key.BirimAd) return this.Memuriyet.KeyAd.BirimAd;
                //    else if (key == this.Memuriyet.Key.Gorev) return this.Memuriyet.KeyAd.Gorev;
                //    else if (key == this.Memuriyet.Key.IstihdamSekli) return this.Memuriyet.KeyAd.IstihdamSekli;
                //    else if (key == this.Memuriyet.Key.BaslamaTarihi) return this.Memuriyet.KeyAd.BaslamaTarihi;
                //    else if (key == this.Memuriyet.Key.AyrilmaTarihi) return this.Memuriyet.KeyAd.AyrilmaTarihi;
                //    else if (key == this.Memuriyet.Key.BaslamaNedeni) return this.Memuriyet.KeyAd.BaslamaNedeni;
                //    else if (key == this.Memuriyet.Key.AyrilmaNedeni) return this.Memuriyet.KeyAd.AyrilmaNedeni;
                //    else if (key == this.Memuriyet.Key.EvrakNo) return this.Memuriyet.KeyAd.EvrakNo;
                //    else if (key == this.Memuriyet.Key.YasalDayanak) return this.Memuriyet.KeyAd.YasalDayanak;
                //    else if (key == this.Memuriyet.Key.OnayTarih) return this.Memuriyet.KeyAd.OnayTarih;
                //    else if (key == this.Memuriyet.Key.Aciklama) return this.Memuriyet.KeyAd.Aciklama;
                //    break;
                case ENUMTARIHCE.KADRO:
                    if (key == this.Kadro.Key.organizasyonKodu) return this.Kadro.KeyAd.organizasyonKodu;
                    else if (key == this.Kadro.Key.gorevOrganizasyonKodu) return this.Kadro.KeyAd.gorevOrganizasyonKodu;
                    else if (key == this.Kadro.Key.unvan) return this.Kadro.KeyAd.unvan;
                    else if (key == this.Kadro.Key.vekilUnvanKod) return this.Kadro.KeyAd.vekilUnvanKod;
                    else if (key == this.Kadro.Key.aciklama) return this.Kadro.KeyAd.aciklama;
                    else if (key == this.Kadro.Key.kadroDerece) return this.Kadro.KeyAd.kadroDerece;
                    else if (key == this.Kadro.Key.kadroTarih) return this.Kadro.KeyAd.kadroTarih;
                    else if (key == this.Kadro.Key.kazanilmisHakDerece) return this.Kadro.KeyAd.kazanilmisHakDerece;
                    else if (key == this.Kadro.Key.kazanilmisHakKademe) return this.Kadro.KeyAd.kazanilmisHakKademe;
                    else if (key == this.Kadro.Key.kazanilmisHakTarih) return this.Kadro.KeyAd.kazanilmisHakTarih;
                    else if (key == this.Kadro.Key.kazanilmisGosterge) return this.Kadro.KeyAd.kazanilmisGosterge;
                    else if (key == this.Kadro.Key.kazanilmisEkGosterge) return this.Kadro.KeyAd.kazanilmisEkGosterge;
                    else if (key == this.Kadro.Key.odemeyeEsasDerece) return this.Kadro.KeyAd.odemeyeEsasDerece;
                    else if (key == this.Kadro.Key.odemeyeEsasKademe) return this.Kadro.KeyAd.odemeyeEsasKademe;
                    else if (key == this.Kadro.Key.odemeyeEsasTarih) return this.Kadro.KeyAd.odemeyeEsasTarih;
                    else if (key == this.Kadro.Key.odemeyeEsasGosterge) return this.Kadro.KeyAd.odemeyeEsasGosterge;
                    else if (key == this.Kadro.Key.odemeyeEsasEkGosterge) return this.Kadro.KeyAd.odemeyeEsasEkGosterge;
                    else if (key == this.Kadro.Key.emekliyeEsasDerece) return this.Kadro.KeyAd.emekliyeEsasDerece;
                    else if (key == this.Kadro.Key.emekliyeEsasKademe) return this.Kadro.KeyAd.emekliyeEsasKademe;
                    else if (key == this.Kadro.Key.emekliyeEsasTarih) return this.Kadro.KeyAd.emekliyeEsasTarih;
                    else if (key == this.Kadro.Key.emekliyeEsasGosterge) return this.Kadro.KeyAd.emekliyeEsasGosterge;
                    else if (key == this.Kadro.Key.emekliyeEsasEkGosterge) return this.Kadro.KeyAd.emekliyeEsasEkGosterge;
                    else if (key == this.Kadro.Key.personelTip) return this.Kadro.KeyAd.personelTip;
                    else if (key == this.Kadro.Key.gorevYeri) return this.Kadro.KeyAd.gorevYeri;
                    else if (key == this.Kadro.Key.onayTarih) return this.Kadro.KeyAd.onayTarih;
                    else if (key == this.Kadro.Key.atamaTarihi) return this.Kadro.KeyAd.atamaTarihi;
                    else if (key == this.Kadro.Key.ayrilmaTarihi) return this.Kadro.KeyAd.ayrilmaTarihi;
                    else if (key == this.Kadro.Key.ayrilmaNedeni) return this.Kadro.KeyAd.ayrilmaTarihi;
                    else if (key == this.Kadro.Key.ayrilmaGerekcesi) return this.Kadro.KeyAd.ayrilmaGerekcesi;
                    else if (key == this.Kadro.Key.hizmetSebebi) return this.Kadro.KeyAd.hizmetSebebi;
                    else if (key == this.Kadro.Key.evrakNo) return this.Kadro.KeyAd.evrakNo;
                    else if (key == this.Kadro.Key.yasalDayanak) return this.Kadro.KeyAd.yasalDayanak;
                    else if (key == this.Kadro.Key.DegisiklikYok) return this.Kadro.KeyAd.DegisiklikYok;
                    break;
                case ENUMTARIHCE.EGITIM:
                    if (key == this.Kurs.Key.Puan) return this.Kurs.KeyAd.Puan;
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
                    else if (key == this.Kurs.Key.DegisiklikYok) return this.Kurs.KeyAd.DegisiklikYok;
                    break;
                case ENUMTARIHCE.CEZAODUL:
                    if (key == this.CezaOdul.Key.VerenKurum) return this.CezaOdul.KeyAd.VerenKurum;
                    else if (key == this.CezaOdul.Key.VerenAd) return this.CezaOdul.KeyAd.VerenAd;
                    else if (key == this.CezaOdul.Key.CezaOdulTip) return this.CezaOdul.KeyAd.CezaOdulTip;
                    else if (key == this.CezaOdul.Key.Tarih) return this.CezaOdul.KeyAd.Tarih;
                    else if (key == this.CezaOdul.Key.VerenUnvan) return this.CezaOdul.KeyAd.VerenUnvan;
                    else if (key == this.CezaOdul.Key.BelgeNo) return this.CezaOdul.KeyAd.BelgeNo;
                    else if (key == this.CezaOdul.Key.BelgeTarih) return this.CezaOdul.KeyAd.BelgeTarih;
                    else if (key == this.CezaOdul.Key.BelgeCinsi) return this.CezaOdul.KeyAd.BelgeCinsi;
                    else if (key == this.CezaOdul.Key.Dayanak) return this.CezaOdul.KeyAd.Dayanak;
                    else if (key == this.CezaOdul.Key.Sebebi) return this.CezaOdul.KeyAd.Sebebi;
                    break;
                case ENUMTARIHCE.MALBILDIRIMI:
                    if (key == this.MalBildirimi.Key.TcKimlikNo) return this.MalBildirimi.KeyAd.TcKimlikNo;
                    else if (key == this.MalBildirimi.Key.Tarih) return this.MalBildirimi.KeyAd.Tarih;
                    else if (key == this.MalBildirimi.Key.Aciklama) return this.MalBildirimi.KeyAd.Aciklama;
                    break;
                case ENUMTARIHCE.KART:
                    if (key == this.Kart.Key.aciklama) return this.Kart.KeyAd.aciklama;
                    else if (key == this.Kart.Key.adiSoyadi) return this.Kart.KeyAd.adiSoyadi;
                    else if (key == this.Kart.Key.DegisiklikYok) return this.Kart.KeyAd.DegisiklikYok;
                    else if (key == this.Kart.Key.iadeAlanPersonel) return this.Kart.KeyAd.iadeAlanPersonel;
                    else if (key == this.Kart.Key.iadeTarihi) return this.Kart.KeyAd.iadeTarihi;
                    else if (key == this.Kart.Key.kartNo) return this.Kart.KeyAd.kartNo;
                    else if (key == this.Kart.Key.kartTipi) return this.Kart.KeyAd.kartTipi;
                    else if (key == this.Kart.Key.kayipTarihi) return this.Kart.KeyAd.kayipTarihi;
                    else if (key == this.Kart.Key.plaka) return this.Kart.KeyAd.plaka;
                    else if (key == this.Kart.Key.tagCihazNo) return this.Kart.KeyAd.tagCihazNo;
                    else if (key == this.Kart.Key.yakinligi) return this.Kart.KeyAd.yakinligi;
                    else if (key == this.Kart.Key.zimmetEdenPersonel) return this.Kart.KeyAd.zimmetEdenPersonel;
                    else if (key == this.Kart.Key.zimmetTarihi) return this.Kart.KeyAd.zimmetTarihi;
                    break;
                case ENUMTARIHCE.ASKERLIK:
                    if (key == this.Askerlik.Key.aciklama) return this.Kart.KeyAd.aciklama;
                    else if (key == this.Askerlik.Key.askerlikBaslamaTarihi) return this.Askerlik.KeyAd.askerlikBaslamaTarihi;
                    else if (key == this.Askerlik.Key.askerlikCuzdanNo) return this.Askerlik.KeyAd.askerlikCuzdanNo;
                    else if (key == this.Askerlik.Key.askerlikDurumu) return this.Askerlik.KeyAd.askerlikDurumu;
                    else if (key == this.Askerlik.Key.askerlikMuafNeden) return this.Askerlik.KeyAd.askerlikMuafNeden;
                    else if (key == this.Askerlik.Key.askerlikSubesi) return this.Askerlik.KeyAd.askerlikSubesi;
                    else if (key == this.Askerlik.Key.astegmenNaspTarihi) return this.Askerlik.KeyAd.astegmenNaspTarihi;
                    else if (key == this.Askerlik.Key.belgeNo) return this.Askerlik.KeyAd.belgeNo;
                    else if (key == this.Askerlik.Key.belgeTarihi) return this.Askerlik.KeyAd.belgeTarihi;
                    else if (key == this.Askerlik.Key.borclanmaVarMi) return this.Askerlik.KeyAd.borclanmaVarMi;
                    else if (key == this.Askerlik.Key.DegisiklikYok) return this.Askerlik.KeyAd.DegisiklikYok;
                    else if (key == this.Askerlik.Key.erYedekSubayGecisTarihi) return this.Askerlik.KeyAd.erYedekSubayGecisTarihi;
                    else if (key == this.Askerlik.Key.erYedekSubayGorevYeri) return this.Askerlik.KeyAd.erYedekSubayGorevYeri;
                    else if (key == this.Askerlik.Key.erYedekSubayKitaHizmetiBaslamaTarihi) return this.Askerlik.KeyAd.erYedekSubayKitaHizmetiBaslamaTarihi;
                    else if (key == this.Askerlik.Key.erYedekSubayKitaHizmetiBitisTarihi) return this.Askerlik.KeyAd.erYedekSubayKitaHizmetiBitisTarihi;
                    else if (key == this.Askerlik.Key.ibrazIntikalTarihi) return this.Askerlik.KeyAd.ibrazIntikalTarihi;
                    else if (key == this.Askerlik.Key.rutbesi) return this.Askerlik.KeyAd.rutbesi;
                    else if (key == this.Askerlik.Key.sayilmayanGunSayisi) return this.Askerlik.KeyAd.sayilmayanGunSayisi;
                    else if (key == this.Askerlik.Key.sinifDonemSicilNo) return this.Askerlik.KeyAd.sinifDonemSicilNo;
                    else if (key == this.Askerlik.Key.sinifi) return this.Askerlik.KeyAd.sinifi;
                    else if (key == this.Askerlik.Key.tecilTarihi) return this.Askerlik.KeyAd.tecilTarihi;
                    else if (key == this.Askerlik.Key.tegmenNaspTarihi) return this.Askerlik.KeyAd.tegmenNaspTarihi;
                    else if (key == this.Askerlik.Key.terhisTarihi) return this.Askerlik.KeyAd.terhisTarihi;
                    else if (key == this.Askerlik.Key.yedekSubayOkuluGirisTarihi) return this.Askerlik.KeyAd.yedekSubayOkuluGirisTarihi;
                    break;
            }

            return "";
        }

        #endregion

        #region "Static Methods & Class"


        public class TagList
        {
            public static string PersonelKod { get { return "PersonelKod"; } }
            public static string Liste { get { return "liste"; } }
            public static string Bilgi { get { return "bilgi"; } }
            public static string YeniKayit { get { return "YeniKayit"; } }
            public static string Guncelleme { get { return "Guncelleme"; } }
            public static string Listeleme { get { return "Listeleme"; } }
            public static string Silindi { get { return "Silindi"; } }
            public static string SiraNo { get { return "Kod"; } }
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
                        case 1: return "Yıllık";
                        case 2: return "657-104/C";
                        case 3: return "Evlenme";
                        case 4: return "Babalık";
                        case 5: return "Ölüm";
                        case 6: return "Sağlık";
                        case 7: return "Analık";
                        case 8: return "Ücretli";
                        case 9: return "Ücretsiz";
                        case 10: return "Mazeret";
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
                case EnumKodAd.Borclanma:
                    switch (kod)
                    {
                        case 0: return "Yok";
                        case 1: return "Var";
                    }
                    break;
            }
            return "";
        }

        public enum EnumKodAd
        {
            MulkDurumu, Cinsiyet, Yakinlik, Durum, Ogrenim, SonOkul, Dil, Ay, DilBelgeTur, MedeniHal, Banka, BankaSube, Askerlik, IzinTip,
            IstihdamSekli, BaslamaNedeni, AyrilmaNedeni,
            Unvan, Sinif, Kurs, CezaOdul, CezaOdulTip, Borclanma
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

                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
                    if (yeni.ad != eski.ad) liste.Add(new string[] { this.Key.Ad, yeni.ad, eski.ad });
                    if (yeni.soyad != eski.soyad) liste.Add(new string[] { this.Key.Soyad, yeni.soyad, eski.soyad });
                    if (yeni.nufus.dini != eski.nufus.dini) liste.Add(new string[] { this.Key.dini, yeni.nufus.dini, eski.nufus.dini });
                    if (yeni.nufus.anaAd != eski.nufus.anaAd) liste.Add(new string[] { this.Key.AnaAdi, yeni.nufus.anaAd, eski.nufus.anaAd });
                    if (yeni.nufus.babaAd != eski.nufus.babaAd) liste.Add(new string[] { this.Key.BabaAdi, yeni.nufus.babaAd, eski.nufus.babaAd });
                    if (yeni.nufus.dogumYeri != eski.nufus.dogumYeri) liste.Add(new string[] { this.Key.DogumYeri, yeni.nufus.dogumYeri, eski.nufus.dogumYeri });
                    if (yeni.nufus.dogumTarihi.ToString() != eski.nufus.dogumTarihi.ToString()) liste.Add(new string[] { this.Key.DogumTarihi, yeni.nufus.dogumTarihi.ToString(), eski.nufus.dogumTarihi.ToString() });
                    if (yeni.nufus.cuzdanSeriNo != eski.nufus.cuzdanSeriNo) liste.Add(new string[] { this.Key.CuzdanSeriNo, yeni.nufus.cuzdanSeriNo, eski.nufus.cuzdanSeriNo });
                    if (yeni.nufus.cuzdanKayitNo != eski.nufus.cuzdanKayitNo) liste.Add(new string[] { this.Key.CuzdanKayitNo, yeni.nufus.cuzdanKayitNo, eski.nufus.cuzdanKayitNo });
                    if (yeni.nufus.nufusIlKod != eski.nufus.nufusIlKod) liste.Add(new string[] { this.Key.NufusIlKod, yeni.nufus.nufusIlKod, eski.nufus.nufusIlKod });
                    if (yeni.nufus.nufusIlceKod != eski.nufus.nufusIlceKod) liste.Add(new string[] { this.Key.NufusIlceKod, yeni.nufus.nufusIlceKod, eski.nufus.nufusIlceKod });
                    if (yeni.nufus.nufusMahalleKoy != eski.nufus.nufusMahalleKoy) liste.Add(new string[] { this.Key.NufusMahalleKoy, yeni.nufus.nufusMahalleKoy, eski.nufus.nufusMahalleKoy });
                    if (yeni.nufus.nufusCiltNo != eski.nufus.nufusCiltNo) liste.Add(new string[] { this.Key.NufusCiltNo, yeni.nufus.nufusCiltNo, eski.nufus.nufusCiltNo });
                    if (yeni.nufus.nufusSiraNo != eski.nufus.nufusSiraNo) liste.Add(new string[] { this.Key.NufusSiraNo, yeni.nufus.nufusSiraNo, eski.nufus.nufusSiraNo });
                    if (yeni.nufus.nufusAileSiraNo != eski.nufus.nufusAileSiraNo) liste.Add(new string[] { this.Key.NufusAileSiraNo, yeni.nufus.nufusAileSiraNo, eski.nufus.nufusAileSiraNo });
                    if (yeni.nufus.cuzdanVerilisNedeni != eski.nufus.cuzdanVerilisNedeni) liste.Add(new string[] { this.Key.CuzdanVerilisNedeni, yeni.nufus.cuzdanVerilisNedeni, eski.nufus.cuzdanVerilisNedeni });
                    if (yeni.nufus.cuzdanVerildigiYer != eski.nufus.cuzdanVerildigiYer) liste.Add(new string[] { this.Key.CuzdanVerildigiYer, yeni.nufus.cuzdanVerildigiYer, eski.nufus.cuzdanVerildigiYer });
                    if (yeni.nufus.cuzdanVerilisTarihi.ToString() != eski.nufus.cuzdanVerilisTarihi.ToString()) liste.Add(new string[] { this.Key.CuzdanVerilisTarihi, yeni.nufus.cuzdanVerilisTarihi.ToString(), eski.nufus.cuzdanVerilisTarihi.ToString() });
                    if (yeni.cinsiyet != eski.cinsiyet) liste.Add(new object[] { this.Key.Cinsiyet, new string[] { ((int)yeni.cinsiyet).ToString(), TNS.IKY.Arac.CinsiyetAdBul(yeni.cinsiyet) }, new string[] { ((int)eski.cinsiyet).ToString(), TNS.IKY.Arac.CinsiyetAdBul(eski.cinsiyet) } });
                    if (yeni.nufus.medeniHali != eski.nufus.medeniHali) liste.Add(new string[] { this.Key.MedeniHali, yeni.nufus.medeniHali > 0 ? IKYSozluk.SozlukAdGetir(yeni.nufus.medeniHali) : "", eski.nufus.medeniHali > 0 ? IKYSozluk.SozlukAdGetir(eski.nufus.medeniHali) : "" });
                    if (yeni.nufus.kanGrubu != eski.nufus.kanGrubu) liste.Add(new string[] { this.Key.KanGrubu, yeni.nufus.kanGrubu.ToString(), eski.nufus.kanGrubu.ToString() });
                    if (yeni.ilkSoyad != eski.ilkSoyad) liste.Add(new string[] { this.Key.IlkSoyad, yeni.ilkSoyad, eski.ilkSoyad });
                    if (yeni.sicilNo != eski.sicilNo) liste.Add(new string[] { this.Key.SicilNo, yeni.sicilNo, eski.sicilNo });
                    if (yeni.emekliSicilNo != eski.emekliSicilNo) liste.Add(new string[] { this.Key.EmekliSicilNo, yeni.emekliSicilNo, eski.emekliSicilNo });
                    if (yeni.sskSicilNo != eski.sskSicilNo) liste.Add(new string[] { this.Key.SskSicilNo, yeni.sskSicilNo, eski.sskSicilNo });
                    if (yeni.bagkurSicilNo != eski.bagkurSicilNo) liste.Add(new string[] { this.Key.BagkurSicilNo, yeni.bagkurSicilNo, eski.bagkurSicilNo });
                    if (yeni.ilkIseBaslamaTarihi.ToString() != eski.ilkIseBaslamaTarihi.ToString()) liste.Add(new string[] { this.Key.ilkIseBaslamaTarihi, yeni.ilkIseBaslamaTarihi.ToString(), eski.ilkIseBaslamaTarihi.ToString() });
                    if (yeni.asaletTarihi.ToString() != eski.asaletTarihi.ToString()) liste.Add(new string[] { this.Key.asaletTarihi, yeni.asaletTarihi.ToString(), eski.asaletTarihi.ToString() });
                    if (yeni.kurumaBaslamaTarihi.ToString() != eski.kurumaBaslamaTarihi.ToString()) liste.Add(new string[] { this.Key.kurumaBaslamaTarihi, yeni.kurumaBaslamaTarihi.ToString(), eski.kurumaBaslamaTarihi.ToString() });
                    if (yeni.arsivNo != eski.arsivNo) liste.Add(new string[] { this.Key.arsivNo, yeni.arsivNo, eski.arsivNo });
                    if (yeni.rumuz != eski.rumuz) liste.Add(new string[] { this.Key.rumuz, yeni.rumuz, eski.rumuz });
                    if (yeni.titr != eski.titr) liste.Add(new string[] { this.Key.titr, yeni.titr, eski.titr });
                    if (yeni.geldigiKurum != eski.geldigiKurum) liste.Add(new string[] { this.Key.geldigiKurum, yeni.geldigiKurum, eski.geldigiKurum });
                    if (yeni.geldigiKurumSicili != eski.geldigiKurumSicili) liste.Add(new string[] { this.Key.geldigiKurumSicili, yeni.geldigiKurumSicili, eski.geldigiKurumSicili });
                    if (yeni.bankaIbanNo != eski.bankaIbanNo) liste.Add(new string[] { this.Key.bankaIbanNo, yeni.bankaIbanNo, eski.bankaIbanNo });
                    if (yeni.vergiDairesi != eski.vergiDairesi) liste.Add(new string[] { this.Key.vergiDairesi, yeni.vergiDairesi, eski.vergiDairesi });
                    if (yeni.aktifMi != eski.aktifMi) liste.Add(new string[] { this.Key.aktifMi, yeni.aktifMi == 1 ? "Aktif" : "Pasif", eski.aktifMi == 1 ? "Aktif" : "Pasif" });
                    if (yeni.terorMagduru != eski.terorMagduru) liste.Add(new string[] { this.Key.terorMagduru, yeni.terorMagduru == 1 ? "Terör Mağduru" : "", eski.terorMagduru == 1 ? "Terör Mağduru" : "" });
                    if (yeni.hukumlu != eski.hukumlu) liste.Add(new string[] { this.Key.hukumlu, yeni.hukumlu == 1 ? "Hükümlü" : "", eski.aktifMi == 1 ? "Hükümlü" : "" });
                    if (yeni.korunmayaMuhtac != eski.korunmayaMuhtac) liste.Add(new string[] { this.Key.korunmayaMuhtac, yeni.korunmayaMuhtac == 1 ? "Korunmaya Muhtaç" : "", eski.korunmayaMuhtac == 1 ? "Korunmaya Muhtaç" : "" });
                    if (yeni.sosyalGuvence != eski.sosyalGuvence) liste.Add(new string[] { this.Key.sosyalGuvence, yeni.sosyalGuvence > 0 ? IKYSozluk.SozlukAdGetir(yeni.sosyalGuvence) : "", eski.sosyalGuvence > 0 ? IKYSozluk.SozlukAdGetir(eski.sosyalGuvence) : "" });
                }
                else
                {
                    if (eski.kod > 0)
                        liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });
                    else
                    {
                        string aktiflik = "";
                        if (eski.aktifDurum == 1)
                            aktiflik = "Aktif ";
                        else if (eski.aktifDurum == 0)
                            aktiflik = "Pasif ";
                        else
                            aktiflik = "Tümü ";

                        liste.Add(new string[] { this.Key.AramaKriteri, " Arama: " + eski.genelSorgu + " Birim: " + eski.kadro.organizasyonKodu + " Unvan: " + eski.kadro.unvan + " Personel Tür Grubu: " + eski.tipGruplar + " Personel Tür: " + eski.personelTipleri + " Aktiflik: " + aktiflik, "" });
                    }
                }

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

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
            public string OdaKodu { get; protected set; }
            public string SicilNo { get; protected set; }
            public string EmekliSicilNo { get; protected set; }
            public string SaglikKarneNo { get; protected set; }
            public string BakanlikKartNo { get; protected set; }
            public string SskSicilNo { get; protected set; }
            public string BagkurSicilNo { get; protected set; }
            public string DegisiklikYok { get; protected set; }
            public string AramaKriteri { get; protected set; }
            public string ilkIseBaslamaTarihi { get; protected set; }
            public string asaletTarihi { get; protected set; }
            public string kurumaBaslamaTarihi { get; protected set; }
            public string arsivNo { get; protected set; }
            public string rumuz { get; protected set; }
            public string titr { get; protected set; }
            public string geldigiKurum { get; protected set; }
            public string geldigiKurumSicili { get; protected set; }
            public string aktifMi { get; protected set; }
            public string terorMagduru { get; protected set; }
            public string hukumlu { get; protected set; }
            public string korunmayaMuhtac { get; protected set; }
            public string bankaIbanNo { get; protected set; }
            public string vergiDairesi { get; protected set; }
            public string sosyalGuvence { get; protected set; }
            public string dini { get; protected set; }

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
                OdaKodu = "OdaKodu";
                SicilNo = "SicilNo";
                EmekliSicilNo = "EmekliSicilNo";
                SaglikKarneNo = "SaglikKarneNo";
                BakanlikKartNo = "BakanlikKartNo";
                SskSicilNo = "SskSicilNo";
                BagkurSicilNo = "BagkurSicilNo";
                DegisiklikYok = "DegisiklikYok";
                AramaKriteri = "AramaKriteri";
                ilkIseBaslamaTarihi = "ilkIseBaslamaTarihi";
                asaletTarihi = "asaletTarihi";
                kurumaBaslamaTarihi = "kurumaBaslamaTarihi";
                arsivNo = "arsivNo";
                rumuz = "rumuz";
                titr = "titr";
                geldigiKurum = "geldigiKurum";
                geldigiKurumSicili = "geldigiKurumSicili";
                aktifMi = "aktifMi";
                terorMagduru = "terorMagduru";
                hukumlu = "hukumlu";
                korunmayaMuhtac = "korunmayaMuhtac";
                bankaIbanNo = "bankaIbanNo";
                vergiDairesi = "vergiDairesi";
                sosyalGuvence = "sosyalGuvence";
                dini = "dini";
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
                base.OdaKodu = "Oda Kodu";
                base.SicilNo = "Sicil No";
                base.EmekliSicilNo = "Emekli Sicil No";
                base.SaglikKarneNo = "Sağlık Karne No";
                base.BakanlikKartNo = "Bakanlık Kart No";
                base.SskSicilNo = "SSK Sicil No";
                base.BagkurSicilNo = "Bağkur Sicil No";
                base.AramaKriteri = "Arama Kriteri";
                base.DegisiklikYok = "Değişiklik Yok";
                base.asaletTarihi = "Asalet Tarihi";
                base.kurumaBaslamaTarihi = "Kuruma Başlama Tarihi";
                base.arsivNo = "Arşiv No";
                base.rumuz = "Rumuz";
                base.titr = "Title";
                base.aktifMi = "Aktif Mi?";
                base.geldigiKurum = "Geldiği Kurum";
                base.geldigiKurumSicili = "Geldiği Kurum Sicili";
                base.terorMagduru = "Terör Mağduru";
                base.hukumlu = "Hükümlü";
                base.korunmayaMuhtac = "Korunmaya Muhtaç";
                base.bankaIbanNo = "Banka Iban No";
                base.vergiDairesi = "Vergi Dairesi";
                base.sosyalGuvence = "Sosyal Güvence";
                base.dini = "Dini";
            }
        }

        #endregion

        #region "HizliBilgi"

        public class HizliBilgiX
        {
            public HizliBilgiKey Key { get; protected set; }
            public HizliBilgiKeyAd KeyAd { get; protected set; }

            public HizliBilgiX()
            {
                this.Key = new HizliBilgiKey();
                this.KeyAd = new HizliBilgiKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(int personelKod)
            {
                ArrayList liste = new ArrayList();

                liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", ENUMTARIHCEISLEMTIP.LISTELE.ToString() }, new string[] { TagList.PersonelKod, personelKod.ToString() } });

                return table;
            }
        }
        public class HizliBilgiKey
        {
            public string DegisiklikYok { get; protected set; }

            public HizliBilgiKey()
            {
                DegisiklikYok = "DegisiklikYok";
            }
        }
        public class HizliBilgiKeyAd : HizliBilgiKey
        {
            public HizliBilgiKeyAd()
            {
                base.DegisiklikYok = "Değişiklik Yok";
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

            public Hashtable DegisenDegerleriGetir(PersonelIletisim yeni, PersonelIletisim eski, ENUMTARIHCEISLEMTIP islem)
            {

                ArrayList liste = new ArrayList();
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.adres != eski.adres) liste.Add(new string[] { this.Key.Adres, yeni.adres, eski.adres });
                    if (yeni.mobilImzaTelefon != eski.mobilImzaTelefon) liste.Add(new string[] { this.Key.mobilImzaTelefon, yeni.mobilImzaTelefon.ToString(), eski.mobilImzaTelefon.ToString() });
                    if (yeni.tarabyaTel != eski.tarabyaTel) liste.Add(new string[] { this.Key.tarabyaTel, yeni.tarabyaTel.ToString(), eski.tarabyaTel.ToString() });
                    if (yeni.lojmanTel != eski.lojmanTel) liste.Add(new string[] { this.Key.lojmanTel, yeni.lojmanTel.ToString(), eski.lojmanTel.ToString() });
                    if (yeni.telsiz.ToString() != eski.telsiz.ToString()) liste.Add(new string[] { this.Key.telsiz, yeni.telsiz.ToString(), eski.telsiz.ToString() });
                    if (yeni.goruntulu != eski.goruntulu) liste.Add(new string[] { this.Key.goruntulu, yeni.goruntulu.ToString(), eski.goruntulu.ToString() });
                    if (yeni.cepTelefon2 != eski.cepTelefon2) liste.Add(new string[] { this.Key.cepTelefon2, yeni.cepTelefon2.ToString(), eski.cepTelefon2.ToString() });
                    if (yeni.rehberdeGozuksun != eski.rehberdeGozuksun) liste.Add(new string[] { this.Key.rehberdeGozuksun, yeni.rehberdeGozuksun.ToString(), eski.rehberdeGozuksun.ToString() });
                    if (yeni.isTelefon != eski.isTelefon) liste.Add(new string[] { this.Key.IsTel, yeni.isTelefon, eski.isTelefon });
                    if (yeni.evTelefon != eski.evTelefon) liste.Add(new string[] { this.Key.EvTel, yeni.evTelefon, eski.evTelefon });
                    if (yeni.cepTelefon != eski.cepTelefon) liste.Add(new string[] { this.Key.CepTel, yeni.cepTelefon.ToString(), eski.cepTelefon.ToString() });
                    if (yeni.faks != eski.faks) liste.Add(new string[] { this.Key.Faks, yeni.faks, eski.faks });
                    if (yeni.eposta != eski.eposta) liste.Add(new string[] { this.Key.Eposta, yeni.eposta, eski.eposta });
                    if (yeni.url != eski.url) liste.Add(new string[] { this.Key.Url, yeni.url, eski.url });
                }
                else
                    liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

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
            public string DegisiklikYok { get; protected set; }
            public string mobilImzaTelefon { get; protected set; }
            public string tarabyaTel { get; protected set; }
            public string lojmanTel { get; protected set; }
            public string telsiz { get; protected set; }
            public string goruntulu { get; protected set; }
            public string rehberdeGozuksun { get; protected set; }
            public string cepTelefon2 { get; protected set; }

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
                DegisiklikYok = "DegisiklikYok";
                tarabyaTel = "tarabyaTel";
                mobilImzaTelefon = "mobilImzaTelefon";
                lojmanTel = "lojmanTel";
                telsiz = "telsiz";
                goruntulu = "goruntulu";
                rehberdeGozuksun = "rehberdeGozuksun";
                cepTelefon2 = "cepTelefon2";
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
                base.DegisiklikYok = "Değişiklik Yok";
                base.tarabyaTel = "Tarabya Telefon";
                base.mobilImzaTelefon = "Mobil İmza Telefon";
                base.lojmanTel = "Lojman Telefon";
                base.telsiz = "Telsiz Telefon";
                base.goruntulu = "Görüntülü Telefon";
                base.rehberdeGozuksun = "Rehberde Gözüksün Mü";
                base.cepTelefon2 = "Telefon (GSM) 2";
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
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.tcKimlikNo : "" });
                    if (yeni.ad != eski.ad) liste.Add(new string[] { this.Key.Ad, yeni.ad, eski.ad });
                    if (yeni.soyad != eski.soyad) liste.Add(new string[] { this.Key.Soyad, yeni.soyad, eski.soyad });
                    if (yeni.dogumTarihi.ToString() != eski.dogumTarihi.ToString()) liste.Add(new string[] { this.Key.DogumTarihi, yeni.dogumTarihi.ToString(), eski.dogumTarihi.ToString() });
                    if (yeni.cinsiyet != eski.cinsiyet) liste.Add(new object[] { this.Key.Cinsiyet, new string[] { ((int)yeni.cinsiyet).ToString(), TNS.IKY.Arac.CinsiyetAdBul(yeni.cinsiyet) }, new string[] { ((int)eski.cinsiyet).ToString(), TNS.IKY.Arac.CinsiyetAdBul(eski.cinsiyet) } });
                    if (yeni.yakinlik != eski.yakinlik) liste.Add(new object[] { this.Key.Yakinlik, new string[] { ((int)yeni.yakinlik).ToString(), TNS.IKY.Arac.YakinlikAd(yeni.yakinlik) }, new string[] { ((int)eski.yakinlik).ToString(), TNS.IKY.Arac.YakinlikAd(eski.yakinlik) } });
                    if (yeni.durum != eski.durum) liste.Add(new string[] { this.Key.OgrenimTipi, yeni.durum > 0 ? IKYSozluk.SozlukAdGetir(yeni.durum) : "", eski.durum > 0 ? IKYSozluk.SozlukAdGetir(eski.durum) : "" });
                    if (yeni.meslek != eski.meslek) liste.Add(new string[] { this.Key.Meslek, yeni.meslek, eski.meslek });
                    if (yeni.bakmaklaYukumlu != eski.bakmaklaYukumlu) liste.Add(new string[] { this.Key.bakmaklaYukumluMu, yeni.bakmaklaYukumlu.ToString(), eski.bakmaklaYukumlu.ToString() });
                    if (yeni.ogrenimTipi != eski.ogrenimTipi) liste.Add(new string[] { this.Key.OgrenimTipi, yeni.ogrenimTipi > 0 ? IKYSozluk.SozlukAdGetir(yeni.ogrenimTipi) : "", eski.ogrenimTipi > 0 ? IKYSozluk.SozlukAdGetir(eski.ogrenimTipi) : "" });
                    if (yeni.yukumlulukBitisTarihi.ToString() != eski.yukumlulukBitisTarihi.ToString()) liste.Add(new string[] { this.Key.bakmaklaYukumluTarihi, yeni.yukumlulukBitisTarihi.ToString(), eski.yukumlulukBitisTarihi.ToString() });
                    if (yeni.agi != eski.agi) liste.Add(new string[] { this.Key.AgiDahil, yeni.agi.ToString(), eski.agi.ToString() });
                }
                else
                    liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.personelKod > 0 ? yeni.personelKod.ToString() : eski.personelKod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

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
            public string OgrenimTipi { get; protected set; }
            public string Meslek { get; protected set; }
            public string bakmaklaYukumluMu { get; protected set; }
            public string bakmaklaYukumluTarihi { get; protected set; }
            public string AgiDahil { get; protected set; }
            public string DegisiklikYok { get; protected set; }

            public AileKey()
            {
                TcKimlikNo = "TcKimlikNo";
                Ad = "Ad";
                Soyad = "Soyad";
                DogumTarihi = "DogumTarihi";
                Cinsiyet = "Cinsiyet";
                Yakinlik = "Yakinlik";
                Durum = "Durum";
                OgrenimTipi = "OgrenimTipi";
                bakmaklaYukumluMu = "bakmaklaYukumluMu";
                bakmaklaYukumluTarihi = "bakmaklaYukumluTarihi";
                AgiDahil = "AgiDahil";
                Meslek = "Meslek";
                DegisiklikYok = "DegisiklikYok";
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
                base.OgrenimTipi = "Öğrenim Tipi";
                base.bakmaklaYukumluMu = "Bakmakla Yükümlü Mü";
                base.bakmaklaYukumluTarihi = "Bakmakla Yükümlülük Bitiş Tarihi";
                base.AgiDahil = "Agi' ye Dahil";
                base.Meslek = "Meslek";
                base.DegisiklikYok = "Değişiklik Yok";
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
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.okulAdi != eski.okulAdi) liste.Add(new string[] { this.Key.OkulAdi, yeni.okulAdi, eski.okulAdi });
                    if (yeni.baslangicTarihi.ToString() != eski.baslangicTarihi.ToString()) liste.Add(new string[] { this.Key.BaslangicTarihi, yeni.baslangicTarihi.ToString(), eski.baslangicTarihi.ToString() });
                    if (yeni.bitirmeTarihi.ToString() != eski.bitirmeTarihi.ToString()) liste.Add(new string[] { this.Key.BitirmeTarihi, yeni.bitirmeTarihi.ToString(), eski.bitirmeTarihi.ToString() });
                    if (yeni.belgeNo != eski.belgeNo) liste.Add(new string[] { this.Key.BelgeNo, yeni.belgeNo, eski.belgeNo });
                    if (yeni.belgeTarihi.ToString() != eski.belgeTarihi.ToString()) liste.Add(new string[] { this.Key.BelgeTarihi, yeni.belgeTarihi.ToString(), eski.belgeTarihi.ToString() });
                    if (yeni.gecerli != eski.gecerli) liste.Add(new object[] { this.Key.Gecerli, new string[] { yeni.gecerli.ToString(), GetKodAd(yeni.gecerli, EnumKodAd.SonOkul) }, new string[] { eski.gecerli.ToString(), GetKodAd(eski.gecerli, EnumKodAd.SonOkul) } });
                    if (yeni.derece != eski.derece) liste.Add(new string[] { this.Key.Derece, yeni.derece.ToString(), eski.derece.ToString() });
                    if (yeni.ogrenimSuresi != eski.ogrenimSuresi) liste.Add(new string[] { this.Key.Suresi, yeni.ogrenimSuresi.ToString(), eski.ogrenimSuresi.ToString() });
                    if (yeni.memuriyeteBasladigiOkul != eski.memuriyeteBasladigiOkul) liste.Add(new string[] { this.Key.MemuriyeteBasladigiOkul, yeni.memuriyeteBasladigiOkul.ToString(), eski.memuriyeteBasladigiOkul.ToString() });
                    if (yeni.kurumaIbrazTarihi.ToString() != eski.kurumaIbrazTarihi.ToString()) liste.Add(new string[] { this.Key.kurumaIbrazTarihi, yeni.kurumaIbrazTarihi.ToString(), eski.kurumaIbrazTarihi.ToString() });
                    if (yeni.hazirlikVarMi != eski.hazirlikVarMi) liste.Add(new string[] { this.Key.Hazirlik, yeni.hazirlikVarMi.ToString(), eski.hazirlikVarMi.ToString() });
                    if (yeni.denklik != eski.denklik) liste.Add(new string[] { this.Key.Denklik, yeni.denklik.ToString(), eski.denklik.ToString() });
                    if (yeni.denkBolum != eski.denkBolum) liste.Add(new string[] { this.Key.DenklikBolum, yeni.denkBolum, eski.denkBolum });
                    if (yeni.denkOkul != eski.denkOkul) liste.Add(new string[] { this.Key.DenklikOkulu, yeni.denkOkul, eski.denkOkul });
                    if (yeni.denklikTarihi.ToString() != eski.denklikTarihi.ToString()) liste.Add(new string[] { this.Key.DenklikTarihi, yeni.denklikTarihi.ToString(), eski.denklikTarihi.ToString() });
                    if (yeni.ogrenimYeri != eski.ogrenimYeri) liste.Add(new string[] { this.Key.OgrenimYeri, yeni.ogrenimYeri > 0 ? IKYSozluk.SozlukAdGetir(yeni.ogrenimYeri) : "", eski.ogrenimYeri > 0 ? IKYSozluk.SozlukAdGetir(eski.ogrenimYeri) : "" });
                    if (yeni.ogrenimKod != eski.ogrenimKod) liste.Add(new string[] { this.Key.OgrenimKod, yeni.ogrenimKod > 0 ? IKYSozluk.SozlukAdGetir(yeni.ogrenimKod) : "", eski.ogrenimKod > 0 ? IKYSozluk.SozlukAdGetir(eski.ogrenimKod) : "" });
                    if (yeni.universiteKod != eski.universiteKod) liste.Add(new string[] { this.Key.UniKod, yeni.universiteKod > 0 ? IKYSozluk.SozlukAdGetir(yeni.universiteKod) : "", eski.universiteKod > 0 ? IKYSozluk.SozlukAdGetir(eski.universiteKod) : "" });
                    if (yeni.fakulteKod != eski.fakulteKod) liste.Add(new string[] { this.Key.FakulteKod, yeni.fakulteKod > 0 ? IKYSozluk.SozlukAdGetir(yeni.fakulteKod) : "", eski.fakulteKod > 0 ? IKYSozluk.SozlukAdGetir(eski.fakulteKod) : "" });
                    if (yeni.bolumKod != eski.bolumKod) liste.Add(new string[] { this.Key.BolumKod, yeni.bolumKod > 0 ? IKYSozluk.SozlukAdGetir(yeni.bolumKod) : "", eski.bolumKod > 0 ? IKYSozluk.SozlukAdGetir(eski.bolumKod) : "" });
                }
                else
                    liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.personelKod > 0 ? yeni.personelKod.ToString() : eski.personelKod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

                return table;
            }

        }
        public class OgrenimKey
        {
            public string UniKod { get; protected set; }
            public string OgrenimKod { get; protected set; }
            public string OkulAdi { get; protected set; }
            public string BaslangicTarihi { get; protected set; }
            public string BitirmeTarihi { get; protected set; }
            public string BelgeNo { get; protected set; }
            public string Derece { get; protected set; }
            public string BelgeTarihi { get; protected set; }
            public string Gecerli { get; protected set; }
            public string BolumKod { get; protected set; }
            public string FakulteKod { get; protected set; }
            public string MemuriyeteBasladigiOkul { get; protected set; }
            public string OgrenimYeri { get; protected set; }
            public string Suresi { get; protected set; }
            public string BelgeTuru { get; protected set; }
            public string Denklik { get; protected set; }
            public string DenklikBolum { get; protected set; }
            public string DenklikOkulu { get; protected set; }
            public string DenklikTarihi { get; protected set; }
            public string Hazirlik { get; protected set; }
            public string DegisiklikYok { get; protected set; }
            public string kurumaIbrazTarihi { get; protected set; }

            public OgrenimKey()
            {
                UniKod = "UniKod";
                OgrenimKod = "OgrenimKod";
                OkulAdi = "OkulAdi";
                BaslangicTarihi = "BaslangicTarihi";
                BitirmeTarihi = "BitirmeTarihi";
                BelgeNo = "BelgeNo";
                Derece = "Derece";
                BelgeTarihi = "BelgeTarihi";
                Gecerli = "Gecerli";
                BolumKod = "BolumKod";
                FakulteKod = "FakulteKod";
                MemuriyeteBasladigiOkul = "MemuriyeteBasladigiOkul";
                OgrenimYeri = "OgrenimYeri";
                Suresi = "Suresi";
                BelgeTuru = "BelgeTuru";
                Denklik = "Denklik";
                DenklikBolum = "DenklikBolum";
                DenklikOkulu = "DenklikOkulu";
                DenklikTarihi = "DenklikTarihi";
                Hazirlik = "Hazirlik";
                DegisiklikYok = "DegisiklikYok";
                kurumaIbrazTarihi = "kurumaIbrazTarihi";
            }
        }
        public class OgrenimKeyAd : OgrenimKey
        {
            public OgrenimKeyAd()
            {
                base.UniKod = "Üniversite";
                base.OgrenimKod = "Eğitim Seviyesi";
                base.OkulAdi = "Okul Adı";
                base.BaslangicTarihi = "Başlangıc Tarihi";
                base.BitirmeTarihi = "Bitirme Tarihi";
                base.BelgeNo = "Belge No";
                base.Derece = "Derece";
                base.BelgeTarihi = "Belge Tarihi";
                base.Gecerli = "En son bitirdiği okul";
                base.BolumKod = "Bölüm";
                base.FakulteKod = "Fakülte";
                base.MemuriyeteBasladigiOkul = "Memuriyete başladığı okul";
                base.OgrenimYeri = "Öğrenim yeri";
                base.Suresi = "Öğrenim süresi";
                base.BelgeTuru = "Belge Türü";
                base.Denklik = "Denklik Varmi";
                base.DenklikBolum = "Denklik Bölümü";
                base.DenklikOkulu = "Denklik Okul Adı";
                base.DenklikTarihi = "Denklik Tarihi";
                base.Hazirlik = "Hazırlık";
                base.DegisiklikYok = "Değişiklik Yok";
                base.kurumaIbrazTarihi = "Kuruma İbraz Tarihi";
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
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.dilKod != eski.dilKod) liste.Add(new string[] { this.Key.DilKod, yeni.dilKod > 0 ? IKYSozluk.SozlukAdGetir(yeni.dilKod) : "", eski.dilKod > 0 ? IKYSozluk.SozlukAdGetir(eski.dilKod) : "" });
                    if (yeni.duzey != eski.duzey) liste.Add(new string[] { this.Key.Duzey, yeni.duzey, eski.duzey });
                    if (yeni.puan != eski.puan) liste.Add(new string[] { this.Key.Puan, yeni.puan.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.puan.ToString() : "" });
                    if (yeni.donemAy != eski.donemAy) liste.Add(new object[] { this.Key.DonemAy, new string[] { yeni.donemAy.ToString(), GetKodAd(yeni.donemAy, EnumKodAd.Ay) }, new string[] { eski.donemAy.ToString(), GetKodAd(eski.donemAy, EnumKodAd.Ay) } });
                    if (yeni.donemYil != eski.donemYil) liste.Add(new string[] { this.Key.DonemYil, yeni.donemYil.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.donemYil.ToString() : "" });
                    if (yeni.belgeTur != eski.belgeTur) liste.Add(new string[] { this.Key.BelgeTur, yeni.belgeTur > 0 ? IKYSozluk.SozlukAdGetir(yeni.belgeTur) : "", eski.belgeTur > 0 ? IKYSozluk.SozlukAdGetir(eski.belgeTur) : "" });
                    if (yeni.belgeTarihi.ToString() != eski.belgeTarihi.ToString()) liste.Add(new string[] { this.Key.BelgeTarihi, yeni.belgeTarihi.ToString(), eski.belgeTarihi.ToString() });
                    if (yeni.belgeNo != eski.belgeNo) liste.Add(new string[] { this.Key.BelgeNo, yeni.belgeNo, eski.belgeNo });
                    if (yeni.dilSeviyesi != eski.dilSeviyesi) liste.Add(new string[] { this.Key.Seviye, yeni.dilSeviyesi, eski.dilSeviyesi });
                    if (yeni.bordroyaEsas != eski.bordroyaEsas) liste.Add(new string[] { this.Key.bordroyaEsas, yeni.bordroyaEsas.ToString(), eski.bordroyaEsas.ToString() });
                    if (yeni.kurumFaydalaniyor != eski.kurumFaydalaniyor) liste.Add(new string[] { this.Key.kurumFaydalaniyor, yeni.kurumFaydalaniyor.ToString(), eski.kurumFaydalaniyor.ToString() });
                    if (yeni.tarih.ToString() != eski.tarih.ToString()) liste.Add(new string[] { this.Key.Tarih, yeni.tarih.ToString(), eski.tarih.ToString() });
                }
                else
                    liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.personelKod > 0 ? yeni.personelKod.ToString() : eski.personelKod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

                return table;
            }

        }
        public class DilKey
        {
            public string Seviye { get; protected set; }
            public string PersonelKod { get; protected set; }
            public string Kod { get; protected set; }
            public string DilKod { get; protected set; }
            public string Puan { get; protected set; }
            public string Duzey { get; protected set; }
            public string DonemAy { get; protected set; }
            public string DonemYil { get; protected set; }
            public string Tarih { get; protected set; }
            public string BelgeTur { get; protected set; }
            public string BelgeTarihi { get; protected set; }
            public string BelgeNo { get; protected set; }
            public string DegisiklikYok { get; protected set; }
            public string bordroyaEsas { get; protected set; }
            public string kurumFaydalaniyor { get; protected set; }

            public DilKey()
            {
                Seviye = "Seviye";
                DilKod = "DilKod";
                Duzey = "Duzey";
                Puan = "Puan";
                DonemAy = "DonemAy";
                DonemYil = "DonemYil";
                BelgeTur = "BelgeTur";
                BelgeTarihi = "BelgeTarihi";
                BelgeNo = "BelgeNo";
                Tarih = "Tarih";
                DegisiklikYok = "DegisiklikYok";
                bordroyaEsas = "bordroyaEsas";
                kurumFaydalaniyor = "kurumFaydalaniyor";
            }
        }
        public class DilKeyAd : DilKey
        {
            public DilKeyAd()
            {
                base.Seviye = "Seviye";
                base.PersonelKod = "Personel Kod";
                base.Kod = "Kod";
                base.DilKod = "Yabancı Dil";
                base.Puan = "Puan";
                base.Duzey = "Düzey";
                base.DonemAy = "Dönem Ay";
                base.DonemYil = "Dönem Yıl";
                base.BelgeTur = "Belge Türü";
                base.BelgeTarihi = "Belge Tarihi";
                base.Tarih = "Tarih";
                base.BelgeNo = "Belge No";
                base.DegisiklikYok = "Değişiklik Yok";
                base.bordroyaEsas = "Bordroya Esas Mı?";
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
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.izinTurKod != eski.izinTurKod) liste.Add(new object[] { this.Key.Tip, new string[] { yeni.izinTurKod.ToString(), GetKodAd(yeni.izinTurKod, EnumKodAd.IzinTip) }, new string[] { eski.izinTurKod.ToString(), GetKodAd(eski.izinTurKod, EnumKodAd.IzinTip) } });
                    if (yeni.baslangicTarihi.ToString() != eski.baslangicTarihi.ToString()) liste.Add(new string[] { this.Key.BaslangicTarihi, yeni.baslangicTarihi.ToString(), eski.baslangicTarihi.ToString() });
                    if (yeni.bitisTarihi.ToString() != eski.bitisTarihi.ToString()) liste.Add(new string[] { this.Key.BitisTarihi, yeni.bitisTarihi.ToString(), eski.bitisTarihi.ToString() });
                    if (yeni.izinYeri != eski.izinYeri) liste.Add(new string[] { this.Key.IzinNotu, yeni.izinYeri, eski.izinYeri });
                    if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.Aciklama, yeni.aciklama, eski.aciklama });
                    if (yeni.telefon != eski.telefon) liste.Add(new string[] { this.Key.Telefon, yeni.telefon, eski.telefon });
                    if (yeni.saglikKurulusu != eski.saglikKurulusu) liste.Add(new string[] { this.Key.SaglikKurulusu, yeni.saglikKurulusu, eski.saglikKurulusu });
                    if (yeni.yerineGorevli != eski.yerineGorevli) liste.Add(new string[] { this.Key.YerineVekaletEden, yeni.yerineGorevli.ToString(), eski.yerineGorevli.ToString() });
                    if (yeni.imzalayan != eski.imzalayan) liste.Add(new string[] { this.Key.ImzaAtan, yeni.imzalayan.ToString(), eski.imzalayan.ToString() });
                    if (yeni.teklifEden != eski.teklifEden) liste.Add(new string[] { this.Key.UygunGoren, yeni.teklifEden.ToString(), eski.teklifEden.ToString() });
                    if (yeni.belgeNo != eski.belgeNo) liste.Add(new string[] { this.Key.BelgeNo, yeni.belgeNo, eski.belgeNo });
                    if (yeni.belgeTarihi.ToString() != eski.belgeTarihi.ToString()) liste.Add(new string[] { this.Key.BelgeTarihi, yeni.belgeTarihi.ToString(), eski.belgeTarihi.ToString() });
                    if (yeni.kullanilanGun != eski.kullanilanGun) liste.Add(new string[] { this.Key.KullanilanGun, yeni.kullanilanGun.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kullanilanGun.ToString() : "" });
                    if (yeni.onay != eski.onay) liste.Add(new string[] { this.Key.Onay, yeni.onay.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.onay.ToString() : "" });
                }
                else
                    liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                //table.Add(TagList.Bilgi, islem == ENUMTARIHCEISLEMTIP.KAYDET ? new object[] { new string[] { TagList.YeniKayit, ENUMTARIHCEISLEMTIP.KAYDET.ToString() }, new string[] { TagList.PersonelKod, yeni.personelKod.ToString() }, new string[] { TagList.SiraNo, yeni.kod.ToString() } } : new object[] { new string[] { TagList.PersonelKod, eski.personelKod.ToString() }, new string[] { TagList.SiraNo, eski.kod.ToString() } });
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.personelKod > 0 ? yeni.personelKod.ToString() : eski.personelKod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

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
            public string SaglikKurulusu { get; protected set; }
            public string YerineVekaletEden { get; protected set; }
            public string ImzaAtan { get; protected set; }
            public string UygunGoren { get; protected set; }
            public string BelgeNo { get; protected set; }
            public string BelgeTarihi { get; protected set; }
            public string DegisiklikYok { get; protected set; }

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
                SaglikKurulusu = "SaglikKurulusu";
                YerineVekaletEden = "YerineVekaletEden";
                ImzaAtan = "ImzaAtan";
                UygunGoren = "UygunGoren";
                BelgeNo = "BelgeNo";
                BelgeTarihi = "BelgeTarihi";
                DegisiklikYok = "DegisiklikYok";
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
                base.SaglikKurulusu = "Sağlık Kuruluşu";
                base.YerineVekaletEden = "Vekalet Eden";
                base.ImzaAtan = "İmza Atan";
                base.UygunGoren = "Uygun Gören";
                base.BelgeNo = "Belge No";
                base.BelgeTarihi = "Belge Tarihi";
                base.DegisiklikYok = "Değişiklik Yok";
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

            //public Hashtable DegisenDegerleriGetir(PersonelMemuriyet yeni, PersonelMemuriyet eski, ENUMTARIHCEISLEMTIP islem)
            //{
            //    ArrayList liste = new ArrayList();
            //    //ObjectArray listeNeden = new PERNeden().Listele(new Neden());

            //    if (islem != ENUMTARIHCEISLEMTIP.KAYDET)
            //        if (yeni.tcKimlikNo != eski.tcKimlikNo) liste.Add(new string[] { this.Key.TcKimlikNo, yeni.tcKimlikNo, eski.tcKimlikNo });
            //    if (yeni.birimAd != eski.birimAd) liste.Add(new string[] { this.Key.BirimAd, yeni.birimAd, eski.birimAd });
            //    if (yeni.gorev != eski.gorev) liste.Add(new string[] { this.Key.Gorev, yeni.gorev, eski.gorev });
            //    if (yeni.istihdamSekli != eski.istihdamSekli) liste.Add(new object[] { this.Key.IstihdamSekli, new string[] { yeni.istihdamSekli.ToString(), GetKodAd(yeni.istihdamSekli, EnumKodAd.IstihdamSekli) }, new string[] { eski.istihdamSekli.ToString(), GetKodAd(eski.istihdamSekli, EnumKodAd.IstihdamSekli) } });
            //    if (yeni.baslamaTarihi.ToString() != eski.baslamaTarihi.ToString()) liste.Add(new string[] { this.Key.BaslamaTarihi, yeni.baslamaTarihi.ToString(), eski.baslamaTarihi.ToString() });
            //    if (yeni.ayrilmaTarihi.ToString() != eski.ayrilmaTarihi.ToString()) liste.Add(new string[] { this.Key.AyrilmaTarihi, yeni.ayrilmaTarihi.ToString(), eski.ayrilmaTarihi.ToString() });
            //    //if (yeni.baslamaNedeni != eski.baslamaNedeni) liste.Add(new object[] { this.Key.BaslamaNedeni, new string[] { yeni.baslamaNedeni.ToString(), GetKodAd(yeni.baslamaNedeni, EnumKodAd.BaslamaNedeni, listeNeden) }, new string[] { eski.baslamaNedeni.ToString(), GetKodAd(eski.baslamaNedeni, EnumKodAd.BaslamaNedeni, listeNeden) } });
            //    //if (yeni.ayrilmaNedeni != eski.ayrilmaNedeni) liste.Add(new object[] { this.Key.AyrilmaNedeni, new string[] { yeni.ayrilmaNedeni.ToString(), GetKodAd(yeni.ayrilmaNedeni, EnumKodAd.AyrilmaNedeni, listeNeden) }, new string[] { eski.ayrilmaNedeni.ToString(), GetKodAd(eski.ayrilmaNedeni, EnumKodAd.AyrilmaNedeni, listeNeden) } });
            //    if (yeni.evrakNo != eski.evrakNo) liste.Add(new string[] { this.Key.EvrakNo, yeni.evrakNo, eski.evrakNo });
            //    if (yeni.yasalDayanak != eski.yasalDayanak) liste.Add(new string[] { this.Key.YasalDayanak, yeni.yasalDayanak, eski.yasalDayanak });
            //    if (yeni.onayTarih.ToString() != eski.onayTarih.ToString()) liste.Add(new string[] { this.Key.OnayTarih, yeni.onayTarih.ToString(), eski.onayTarih.ToString() });
            //    if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.Aciklama, yeni.aciklama, eski.aciklama });

            //    Hashtable table = new Hashtable();
            //    table.Add(TagList.Liste, liste);
            //    table.Add(TagList.Bilgi, islem == ENUMTARIHCEISLEMTIP.KAYDET ? new object[] { new string[] { TagList.YeniKayit, "" } } : new object[] { new string[] { TagList.Birim, eski.birimAd }, new string[] { TagList.SiraNo, eski.siraNo.ToString() } });

            //    return table;
            //}

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
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    ObjectArray listeUnvan = new PERUnvan().Listele(new Unvan());
                    if (yeni.personelTip != eski.personelTip) liste.Add(new string[] { this.Key.personelTip, yeni.personelTip > 0 ? Arac.PersonelTipBul(yeni.personelTip).ad : "", eski.personelTip > 0 ? Arac.PersonelTipBul(eski.personelTip).ad : "" });
                    if (yeni.organizasyonKodu != eski.organizasyonKodu) liste.Add(new string[] { this.Key.organizasyonKodu, yeni.organizasyonKodu > 0 ? TNS.ORG.Arac.OrgBirimBul(yeni.organizasyonKodu).adi : "", eski.organizasyonKodu > 0 ? TNS.ORG.Arac.OrgBirimBul(eski.organizasyonKodu).adi : "" });
                    if (yeni.gorevOrganizasyonKodu != eski.gorevOrganizasyonKodu) liste.Add(new string[] { this.Key.gorevOrganizasyonKodu, yeni.gorevOrganizasyonKodu > 0 ? TNS.ORG.Arac.OrgBirimBul(yeni.gorevOrganizasyonKodu).adi : "", eski.gorevOrganizasyonKodu > 0 ? TNS.ORG.Arac.OrgBirimBul(eski.gorevOrganizasyonKodu).adi : "" });
                    if (yeni.unvan != eski.unvan) liste.Add(new object[] { this.Key.unvan, new string[] { yeni.unvan.ToString(), GetKodAd(yeni.unvan, EnumKodAd.Unvan, listeUnvan) }, new string[] { eski.unvan.ToString(), GetKodAd(eski.unvan, EnumKodAd.Unvan, listeUnvan) } });
                    if (yeni.vekilUnvanKod != eski.vekilUnvanKod) liste.Add(new object[] { this.Key.vekilUnvanKod, new string[] { yeni.vekilUnvanKod.ToString(), GetKodAd(yeni.vekilUnvanKod, EnumKodAd.Unvan, listeUnvan) }, new string[] { eski.vekilUnvanKod.ToString(), GetKodAd(eski.vekilUnvanKod, EnumKodAd.Unvan, listeUnvan) } });
                    if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.aciklama, yeni.aciklama, eski.aciklama });
                    if (yeni.kadroDerece != eski.kadroDerece) liste.Add(new string[] { this.Key.kadroDerece, yeni.kadroDerece.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kadroDerece.ToString() : "" });
                    if (yeni.kadroTarih.ToString() != eski.kadroTarih.ToString()) liste.Add(new string[] { this.Key.kadroTarih, yeni.kadroTarih.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kadroTarih.ToString() : "" });
                    if (yeni.kazanilmisHakDerece != eski.kazanilmisHakDerece) liste.Add(new string[] { this.Key.kazanilmisHakDerece, yeni.kazanilmisHakDerece.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kazanilmisHakDerece.ToString() : "" });
                    if (yeni.kazanilmisHakKademe != eski.kazanilmisHakKademe) liste.Add(new string[] { this.Key.kazanilmisHakKademe, yeni.kazanilmisHakKademe.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kazanilmisHakKademe.ToString() : "" });
                    if (yeni.kazanilmisHakTarih.ToString() != eski.kazanilmisHakTarih.ToString()) liste.Add(new string[] { this.Key.kazanilmisHakTarih, yeni.kazanilmisHakTarih.ToString(), eski.kazanilmisHakTarih.ToString() });
                    if (yeni.kazanilmisGosterge.ToString() != eski.kazanilmisGosterge.ToString()) liste.Add(new string[] { this.Key.kazanilmisGosterge, yeni.kazanilmisGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kazanilmisGosterge.ToString() : "" });
                    if (yeni.kazanilmisEkGosterge.ToString() != eski.kazanilmisEkGosterge.ToString()) liste.Add(new string[] { this.Key.kazanilmisEkGosterge, yeni.kazanilmisEkGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.kazanilmisEkGosterge.ToString() : "" });
                    if (yeni.odemeyeEsasDerece != eski.odemeyeEsasDerece) liste.Add(new string[] { this.Key.odemeyeEsasDerece, yeni.odemeyeEsasDerece.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.odemeyeEsasDerece.ToString() : "" });
                    if (yeni.odemeyeEsasKademe != eski.odemeyeEsasKademe) liste.Add(new string[] { this.Key.odemeyeEsasKademe, yeni.odemeyeEsasKademe.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.odemeyeEsasKademe.ToString() : "" });
                    if (yeni.odemeyeEsasTarih.ToString() != eski.odemeyeEsasTarih.ToString()) liste.Add(new string[] { this.Key.odemeyeEsasTarih, yeni.odemeyeEsasTarih.ToString(), eski.odemeyeEsasTarih.ToString() });
                    if (yeni.odemeyeEsasGosterge.ToString() != eski.odemeyeEsasGosterge.ToString()) liste.Add(new string[] { this.Key.odemeyeEsasGosterge, yeni.odemeyeEsasGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.odemeyeEsasGosterge.ToString() : "" });
                    if (yeni.odemeyeEsasEkGosterge.ToString() != eski.odemeyeEsasEkGosterge.ToString()) liste.Add(new string[] { this.Key.odemeyeEsasEkGosterge, yeni.odemeyeEsasEkGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.odemeyeEsasEkGosterge.ToString() : "" });
                    if (yeni.emekliyeEsasDerece != eski.emekliyeEsasDerece) liste.Add(new string[] { this.Key.emekliyeEsasDerece, yeni.emekliyeEsasDerece.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.emekliyeEsasDerece.ToString() : "" });
                    if (yeni.emekliyeEsasKademe != eski.emekliyeEsasKademe) liste.Add(new string[] { this.Key.emekliyeEsasKademe, yeni.emekliyeEsasKademe.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.emekliyeEsasKademe.ToString() : "" });
                    if (yeni.emekliyeEsasTarih.ToString() != eski.emekliyeEsasTarih.ToString()) liste.Add(new string[] { this.Key.emekliyeEsasTarih, yeni.emekliyeEsasTarih.ToString(), eski.emekliyeEsasTarih.ToString() });
                    if (yeni.emekliyeEsasGosterge.ToString() != eski.emekliyeEsasGosterge.ToString()) liste.Add(new string[] { this.Key.emekliyeEsasGosterge, yeni.emekliyeEsasGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.emekliyeEsasGosterge.ToString() : "" });
                    if (yeni.emekliyeEsasEkGosterge.ToString() != eski.emekliyeEsasEkGosterge.ToString()) liste.Add(new string[] { this.Key.emekliyeEsasEkGosterge, yeni.emekliyeEsasEkGosterge.ToString(), islem != ENUMTARIHCEISLEMTIP.KAYDET ? eski.emekliyeEsasEkGosterge.ToString() : "" });
                    if (yeni.onayTarih.ToString() != eski.onayTarih.ToString()) liste.Add(new string[] { this.Key.onayTarih, yeni.onayTarih.ToString(), eski.onayTarih.ToString() });
                    if (yeni.atamaTarihi.ToString() != eski.atamaTarihi.ToString()) liste.Add(new string[] { this.Key.atamaTarihi, yeni.atamaTarihi.ToString(), eski.atamaTarihi.ToString() });
                    if (yeni.ayrilmaTarihi.ToString() != eski.ayrilmaTarihi.ToString()) liste.Add(new string[] { this.Key.ayrilmaTarihi, yeni.ayrilmaTarihi.ToString(), eski.ayrilmaTarihi.ToString() });
                    if (yeni.gorevYeri != eski.gorevYeri) liste.Add(new string[] { this.Key.gorevYeri, yeni.gorevYeri, eski.gorevYeri });
                    if (yeni.yasalDayanak != eski.yasalDayanak) liste.Add(new string[] { this.Key.yasalDayanak, yeni.yasalDayanak, eski.yasalDayanak });
                    if (yeni.evrakNo != eski.evrakNo) liste.Add(new string[] { this.Key.evrakNo, yeni.evrakNo, eski.evrakNo });
                    if (yeni.ayrilmaGerekcesi != eski.ayrilmaGerekcesi) liste.Add(new string[] { this.Key.ayrilmaGerekcesi, yeni.ayrilmaGerekcesi > 0 ? IKYSozluk.SozlukAdGetir(yeni.ayrilmaGerekcesi) : "", eski.ayrilmaGerekcesi > 0 ? IKYSozluk.SozlukAdGetir(eski.ayrilmaGerekcesi) : "" });
                    if (yeni.hizmetSebebiKod != eski.hizmetSebebiKod) liste.Add(new string[] { this.Key.hizmetSebebi, yeni.hizmetSebebiKod > 0 ? IKYSozluk.SozlukAdGetir(yeni.hizmetSebebiKod) : "", eski.hizmetSebebiKod > 0 ? IKYSozluk.SozlukAdGetir(eski.hizmetSebebiKod) : "" });
                }
                else
                    liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.personelKod > 0 ? yeni.personelKod.ToString() : eski.personelKod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

                return table;
            }
        }
        public class KadroKey
        {
            public string organizasyonKodu { get; protected set; }
            public string gorevOrganizasyonKodu { get; protected set; }
            public string unvan { get; protected set; }
            public string vekilUnvanKod { get; protected set; }
            public string aciklama { get; protected set; }
            public string kadroDerece { get; protected set; }
            public string kadroTarih { get; protected set; }
            public string kazanilmisHakDerece { get; protected set; }
            public string kazanilmisHakKademe { get; protected set; }
            public string kazanilmisHakTarih { get; protected set; }
            public string kazanilmisGosterge { get; protected set; }
            public string kazanilmisEkGosterge { get; protected set; }
            public string odemeyeEsasDerece { get; protected set; }
            public string odemeyeEsasKademe { get; protected set; }
            public string odemeyeEsasTarih { get; protected set; }
            public string odemeyeEsasGosterge { get; protected set; }
            public string odemeyeEsasEkGosterge { get; protected set; }
            public string emekliyeEsasDerece { get; protected set; }
            public string emekliyeEsasKademe { get; protected set; }
            public string emekliyeEsasTarih { get; protected set; }
            public string emekliyeEsasGosterge { get; protected set; }
            public string emekliyeEsasEkGosterge { get; protected set; }
            public string personelTip { get; protected set; }
            public string gorevYeri { get; protected set; }
            public string ayrilmaGerekcesi { get; protected set; }
            public string onayTarih { get; protected set; }
            public string atamaTarihi { get; protected set; }
            public string ayrilmaTarihi { get; protected set; }
            public string hizmetSebebi { get; protected set; }
            public string ayrilmaNedeni { get; protected set; }
            public string evrakNo { get; protected set; }
            public string yasalDayanak { get; protected set; }
            public string DegisiklikYok { get; protected set; }

            public KadroKey()
            {
                organizasyonKodu = "organizasyonKodu";
                gorevOrganizasyonKodu = "gorevOrganizasyonKodu";
                unvan = "unvan";
                vekilUnvanKod = "vekilUnvanKod";
                aciklama = "Aciklama";
                kadroDerece = "KadroDerece";
                kadroTarih = "KadroTarih";
                kazanilmisHakDerece = "KazanilmisHakDerece";
                kazanilmisHakKademe = "KazanilmisHakKademe";
                kazanilmisHakTarih = "KazanilmisHakTarih";
                kazanilmisGosterge = "KazanilmisGosterge";
                kazanilmisEkGosterge = "KazanilmisEkGosterge";
                odemeyeEsasDerece = "OdemeyeEsasDerece";
                odemeyeEsasKademe = "OdemeyeEsasKademe";
                odemeyeEsasTarih = "OdemeyeEsasTarih";
                odemeyeEsasGosterge = "OdemeyeEsasGosterge";
                odemeyeEsasEkGosterge = "OdemeyeEsasEkGosterge";
                emekliyeEsasDerece = "EmekliyeEsasDerece";
                emekliyeEsasKademe = "EmekliyeEsasKademe";
                emekliyeEsasTarih = "EmekliyeEsasTarih";
                emekliyeEsasGosterge = "EmekliyeEsasGosterge";
                emekliyeEsasEkGosterge = "EmekliyeEsasEkGosterge";
                personelTip = "personelTip";
                gorevYeri = "gorevYeri";
                ayrilmaGerekcesi = "ayrilmaGerekcesi";
                onayTarih = "onayTarih";
                atamaTarihi = "atamaTarihi";
                ayrilmaTarihi = "ayrilmaTarihi";
                hizmetSebebi = "hizmetSebebi";
                ayrilmaNedeni = "ayrilmaNedeni";
                evrakNo = "evrakNo";
                yasalDayanak = "yasalDayanak";
                DegisiklikYok = "DegisiklikYok";
            }
        }
        public class KadroKeyAd : KadroKey
        {
            public KadroKeyAd()
            {
                base.organizasyonKodu = "Birim";
                base.gorevOrganizasyonKodu = "Görev Birimi";
                base.unvan = "Unvanı";
                base.vekilUnvanKod = "Vekil Unvanı";
                base.aciklama = "Açıklama";
                base.kadroDerece = "Kadro Derece";
                base.kadroTarih = "Kadro Tarih";
                base.kazanilmisHakDerece = "Kazanılmış Hak Derece";
                base.kazanilmisHakKademe = "Kazanılmış Hak Kademe";
                base.kazanilmisHakTarih = "Kazanılmış Hak Tarih";
                base.kazanilmisGosterge = "Kazanılmış Hak Gösterge";
                base.kazanilmisEkGosterge = "Kazanılmış Ek Gösterge";
                base.odemeyeEsasDerece = "Ödemeye Esas Derece";
                base.odemeyeEsasKademe = "Ödemeye Esas Kademe";
                base.odemeyeEsasTarih = "Ödemeye Esas Tarih";
                base.odemeyeEsasGosterge = "Ödemeye Esas Gösterge";
                base.odemeyeEsasEkGosterge = "Ödemeye Esas Ek Gösterge";
                base.emekliyeEsasDerece = "Emekliye Esas Derece";
                base.emekliyeEsasKademe = "Emekliye Esas Kademe";
                base.emekliyeEsasTarih = "Emekliye Esas Tarih";
                base.emekliyeEsasGosterge = "Emekliye Esas Gösterge";
                base.emekliyeEsasEkGosterge = "Emekliye Esas Ek Gösterge";
                base.personelTip = "Personel Tipi";
                base.gorevYeri = "Görev Yeri";
                base.ayrilmaGerekcesi = "Ayrılma Gerekçesi";
                base.onayTarih = "Onay Tarih";
                base.atamaTarihi = "Atama Tarihi";
                base.ayrilmaTarihi = "Ayrılma Tarihi";
                base.hizmetSebebi = "Hizmet Sebebi";
                base.ayrilmaNedeni = "Ayrılma Nedeni";
                base.evrakNo = "Evrak No";
                base.yasalDayanak = "Yasal Dayanak";
                base.DegisiklikYok = "Değişiklik Yok";
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
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.kursKod != eski.kursKod) liste.Add(new string[] { this.Key.KursKod, yeni.kursKod > 0 ? IKYSozluk.SozlukAdGetir(yeni.kursKod) : "", eski.kursKod > 0 ? IKYSozluk.SozlukAdGetir(eski.kursKod) : "" });
                    if (yeni.kursAdi != eski.kursAdi) liste.Add(new string[] { this.Key.KursAdi, yeni.kursAdi, eski.kursAdi });
                    if (yeni.baslamaTarihi.ToString() != eski.baslamaTarihi.ToString()) liste.Add(new string[] { this.Key.BaslamaTarihi, yeni.baslamaTarihi.ToString(), eski.baslamaTarihi.ToString() });
                    if (yeni.bitirmeTarihi.ToString() != eski.bitirmeTarihi.ToString()) liste.Add(new string[] { this.Key.BitirmeTarihi, yeni.bitirmeTarihi.ToString(), eski.bitirmeTarihi.ToString() });
                    if (yeni.aldigiYer != eski.aldigiYer) liste.Add(new string[] { this.Key.AldigiYer, yeni.aldigiYer, eski.aldigiYer });
                    if (yeni.egitimYeri != eski.egitimYeri) liste.Add(new string[] { this.Key.EgitimYeri, yeni.egitimYeri > 0 ? IKYSozluk.SozlukAdGetir(yeni.egitimYeri) : "", eski.egitimYeri > 0 ? IKYSozluk.SozlukAdGetir(eski.egitimYeri) : "" });
                    if (yeni.gun != eski.gun) liste.Add(new string[] { this.Key.Gun, yeni.gun.ToString(), eski.gun.ToString() });
                    if (yeni.dersSaati != eski.dersSaati) liste.Add(new string[] { this.Key.DersSaati, yeni.dersSaati.ToString(), eski.dersSaati.ToString() });
                    if (yeni.yil != eski.yil) liste.Add(new string[] { this.Key.Yil, yeni.yil.ToString(), eski.yil.ToString() });
                    if (yeni.donem != eski.donem) liste.Add(new string[] { this.Key.Donem, yeni.donem, eski.donem });
                    if (yeni.derece != eski.derece) liste.Add(new string[] { this.Key.Derece, yeni.derece.ToString(), eski.derece.ToString() });
                    if (yeni.dosyaNo != eski.dosyaNo) liste.Add(new string[] { this.Key.DosyaNo, yeni.dosyaNo, eski.dosyaNo });
                    if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.Aciklama, yeni.aciklama, eski.aciklama });
                    if (yeni.puan.ToString() != eski.puan.ToString()) liste.Add(new string[] { this.Key.Puan, yeni.puan.ToString(), eski.puan.ToString() });
                }
                else
                    liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.personelKod > 0 ? yeni.personelKod.ToString() : eski.personelKod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

                return table;
            }
        }
        public class KursKey
        {
            public string Puan { get; protected set; }
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
            public string DegisiklikYok { get; protected set; }

            public KursKey()
            {
                Puan = "Puan";
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
                DegisiklikYok = "DegisiklikYok";
            }
        }
        public class KursKeyAd : KursKey
        {
            public KursKeyAd()
            {
                base.Puan = "Puan";
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
                base.DegisiklikYok = "Değişiklik Yok";
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
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.cezaOdulTipi != eski.cezaOdulTipi) liste.Add(new string[] { this.Key.CezaOdulTip, yeni.cezaOdulTipi > 0 ? IKYSozluk.SozlukAdGetir(yeni.cezaOdulTipi) : "", eski.cezaOdulTipi > 0 ? IKYSozluk.SozlukAdGetir(eski.cezaOdulTipi) : "" });
                    if (yeni.verenKurum != eski.verenKurum) liste.Add(new string[] { this.Key.VerenKurum, yeni.verenKurum, eski.verenKurum });
                    if (yeni.verenAdi != eski.verenAdi) liste.Add(new string[] { this.Key.VerenAd, yeni.verenAdi, eski.verenAdi });
                    if (yeni.tarih.ToString() != eski.tarih.ToString()) liste.Add(new string[] { this.Key.Tarih, yeni.tarih.ToString(), eski.tarih.ToString() });
                    if (yeni.belgeTarihi.ToString() != eski.belgeTarihi.ToString()) liste.Add(new string[] { this.Key.BelgeTarih, yeni.belgeTarihi.ToString(), eski.belgeTarihi.ToString() });
                    if (yeni.verenUnvani != eski.verenUnvani) liste.Add(new string[] { this.Key.VerenUnvan, yeni.verenUnvani, eski.verenUnvani });
                    if (yeni.dayanak != eski.dayanak) liste.Add(new string[] { this.Key.Dayanak, yeni.dayanak, eski.dayanak });
                    if (yeni.sebebi != eski.sebebi) liste.Add(new string[] { this.Key.Sebebi, yeni.sebebi, eski.sebebi });
                    if (yeni.belgeNo != eski.belgeNo) liste.Add(new string[] { this.Key.BelgeNo, yeni.belgeNo, eski.belgeNo });
                    if (yeni.belgeCinsi != eski.belgeCinsi) liste.Add(new string[] { this.Key.BelgeCinsi, yeni.belgeCinsi, eski.belgeCinsi });
                }
                else
                    liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.personelKod > 0 ? yeni.personelKod.ToString() : eski.personelKod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

                return table;
            }

        }
        public class CezaOdulKey
        {
            public string VerenKurum { get; protected set; }
            public string VerenAd { get; protected set; }
            public string VerenUnvan { get; protected set; }
            public string Dayanak { get; protected set; }
            public string CezaOdulTip { get; protected set; }
            public string Tarih { get; protected set; }
            public string Sebebi { get; protected set; }
            public string BelgeNo { get; protected set; }
            public string BelgeTarih { get; protected set; }
            public string BelgeCinsi { get; protected set; }
            public string DegisiklikYok { get; protected set; }

            public CezaOdulKey()
            {
                VerenKurum = "VerenKurum";
                VerenAd = "VerenAd";
                VerenUnvan = "VerenUnvan";
                Dayanak = "Dayanak";
                CezaOdulTip = "CezaOdulTip";
                Tarih = "Tarih";
                Sebebi = "Sebebi";
                BelgeNo = "BelgeNo";
                BelgeTarih = "BelgeTarih";
                BelgeCinsi = "BelgeCinsi";
                DegisiklikYok = "DegisiklikYok";
            }
        }
        public class CezaOdulKeyAd : CezaOdulKey
        {
            public CezaOdulKeyAd()
            {
                base.VerenKurum = "Veren Kurum";
                base.VerenAd = "Veren Ad";
                base.VerenUnvan = "Veren Unvan";
                base.Dayanak = "Dayanak";
                base.CezaOdulTip = "Ceza Ödül Tipi";
                base.Tarih = "Tarih";
                base.Sebebi = "Sebebi";
                base.BelgeNo = "Belge No";
                base.BelgeTarih = "Belge Tarihi";
                base.BelgeCinsi = "Belge Cinsi";
                base.DegisiklikYok = "Değişiklik Yok";
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
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.yil.ToString() != eski.yil.ToString()) liste.Add(new string[] { this.Key.yil, yeni.yil.ToString(), eski.yil.ToString() });
                    if (yeni.devreden.ToString() != eski.devreden.ToString()) liste.Add(new string[] { this.Key.devreden, yeni.devreden.ToString(), eski.devreden.ToString() });
                }
                else
                {
                    if (eski.personelKod > 0)
                        liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });
                    else
                        liste.Add(new string[] { this.Key.AramaKriteri, "Yıl: " + eski.yil + " Birim: " + eski.birimAd + " Arama: " + eski.arama, "" });
                }

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.personelKod > 0 ? yeni.personelKod.ToString() : eski.personelKod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

                return table;
            }
        }
        public class IzinDevirKey
        {
            public string yil { get; protected set; }
            public string devreden { get; protected set; }
            public string DegisiklikYok { get; protected set; }
            public string AramaKriteri { get; protected set; }

            public IzinDevirKey()
            {
                yil = "Yil";
                devreden = "Devreden";
                DegisiklikYok = "DegisiklikYok";
                AramaKriteri = "AramaKriteri";
            }
        }
        public class IzinDevirKeyAd : IzinDevirKey
        {
            public IzinDevirKeyAd()
            {
                base.yil = "Yıl";
                base.devreden = "Devreden";
                base.DegisiklikYok = "Değişiklik Yok";
                base.AramaKriteri = "Arama Kriteri";
            }
        }

        #endregion

        #region "Kart"

        public class KartX
        {
            public KartKey Key { get; protected set; }
            public KartKeyAd KeyAd { get; protected set; }

            public KartX()
            {
                this.Key = new KartKey();
                this.KeyAd = new KartKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PersonelKart yeni, PersonelKart eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.kartTipi != eski.kartTipi) liste.Add(new string[] { this.Key.kartTipi, yeni.kartTipi > 0 ? IKYSozluk.SozlukAdGetir(yeni.kartTipi) : "", eski.kartTipi > 0 ? IKYSozluk.SozlukAdGetir(eski.kartTipi) : "" });
                    if (yeni.kartNo != eski.kartNo) liste.Add(new string[] { this.Key.kartNo, yeni.kartNo, eski.kartNo });
                    if ((int)yeni.yakinligi != (int)eski.yakinligi) liste.Add(new string[] { this.Key.yakinligi, yeni.yakinligi.ToString(), eski.yakinligi.ToString() });
                    if (yeni.kayipTarihi.ToString() != eski.kayipTarihi.ToString()) liste.Add(new string[] { this.Key.kayipTarihi, yeni.kayipTarihi.ToString(), eski.kayipTarihi.ToString() });
                    if (yeni.iadeTarihi.ToString() != eski.iadeTarihi.ToString()) liste.Add(new string[] { this.Key.iadeTarihi, yeni.iadeTarihi.ToString(), eski.iadeTarihi.ToString() });
                    if (yeni.zimmetTarihi.ToString() != eski.zimmetTarihi.ToString()) liste.Add(new string[] { this.Key.zimmetTarihi, yeni.zimmetTarihi.ToString(), eski.zimmetTarihi.ToString() });
                    if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.aciklama, yeni.aciklama, eski.aciklama });
                    if (yeni.plaka != eski.plaka) liste.Add(new string[] { this.Key.plaka, yeni.plaka, eski.plaka });
                    if (yeni.adiSoyadi != eski.adiSoyadi) liste.Add(new string[] { this.Key.adiSoyadi, yeni.adiSoyadi, eski.adiSoyadi });
                    if (yeni.tagCihazNo != eski.tagCihazNo) liste.Add(new string[] { this.Key.tagCihazNo, yeni.tagCihazNo, eski.tagCihazNo });
                    if (yeni.iadeAlanPersonel != eski.iadeAlanPersonel) liste.Add(new string[] { this.Key.iadeAlanPersonel, yeni.iadeAlanPersonel.ToString(), eski.iadeAlanPersonel.ToString() });
                    if (yeni.zimmetEdenPersonel != eski.zimmetEdenPersonel) liste.Add(new string[] { this.Key.iadeAlanPersonel, yeni.zimmetEdenPersonel.ToString(), eski.zimmetEdenPersonel.ToString() });
                }
                else
                    liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.personelKod > 0 ? yeni.personelKod.ToString() : eski.personelKod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

                return table;
            }

        }
        public class KartKey
        {
            public string kartTipi { get; protected set; }
            public string kartNo { get; protected set; }
            public string yakinligi { get; protected set; }
            public string plaka { get; protected set; }
            public string adiSoyadi { get; protected set; }
            public string tagCihazNo { get; protected set; }
            public string zimmetTarihi { get; protected set; }
            public string kayipTarihi { get; protected set; }
            public string iadeTarihi { get; protected set; }
            public string aciklama { get; protected set; }
            public string iadeAlanPersonel { get; protected set; }
            public string zimmetEdenPersonel { get; protected set; }
            public string DegisiklikYok { get; protected set; }

            public KartKey()
            {
                kartTipi = "kartTipi";
                kartNo = "kartNo";
                yakinligi = "yakinligi";
                plaka = "plaka";
                adiSoyadi = "adiSoyadi";
                tagCihazNo = "tagCihazNo";
                zimmetTarihi = "zimmetTarihi";
                kayipTarihi = "kayipTarihi";
                iadeTarihi = "iadeTarihi";
                aciklama = "aciklama";
                iadeAlanPersonel = "iadeAlanPersonel";
                zimmetEdenPersonel = "zimmetEdenPersonel";
                DegisiklikYok = "DegisiklikYok";
            }
        }
        public class KartKeyAd : KartKey
        {
            public KartKeyAd()
            {
                base.kartTipi = "Kart Tipi";
                base.kartNo = "Kart No";
                base.yakinligi = "Yakinliği";
                base.plaka = "Plaka";
                base.adiSoyadi = "Adı Soyadı";
                base.tagCihazNo = "Tag Cihaz No";
                base.zimmetTarihi = "Zimmet Tarihi";
                base.kayipTarihi = "Kayip Tarihi";
                base.iadeTarihi = "İade Tarihi";
                base.aciklama = "Açıklama";
                base.iadeAlanPersonel = "İade Alan Personel";
                base.zimmetEdenPersonel = "Zimmet Eden Personel";
                base.DegisiklikYok = "Değişiklik Yok";
            }
        }

        #endregion

        #region "Askerlik"

        public class AskerlikX
        {
            public AskerlikKey Key { get; protected set; }
            public AskerlikKeyAd KeyAd { get; protected set; }

            public AskerlikX()
            {
                this.Key = new AskerlikKey();
                this.KeyAd = new AskerlikKeyAd();
            }

            public Hashtable DegisenDegerleriGetir(PAskerlik yeni, PAskerlik eski, ENUMTARIHCEISLEMTIP islem)
            {
                ArrayList liste = new ArrayList();
                bool devam = false;

                if (islem == ENUMTARIHCEISLEMTIP.YAZDIR || islem == ENUMTARIHCEISLEMTIP.LISTELE)
                    devam = true;
                if (!devam)
                {
                    if (yeni.askerlikBaslamaTarihi.ToString() != eski.askerlikBaslamaTarihi.ToString()) liste.Add(new string[] { this.Key.askerlikBaslamaTarihi, yeni.askerlikBaslamaTarihi.ToString(), eski.askerlikBaslamaTarihi.ToString() });
                    if (yeni.terhisTarihi.ToString() != eski.terhisTarihi.ToString()) liste.Add(new string[] { this.Key.terhisTarihi, yeni.terhisTarihi.ToString(), eski.terhisTarihi.ToString() });
                    if (yeni.belgeTarihi.ToString() != eski.belgeTarihi.ToString()) liste.Add(new string[] { this.Key.belgeTarihi, yeni.belgeTarihi.ToString(), eski.belgeTarihi.ToString() });
                    if (yeni.tecilTarihi.ToString() != eski.tecilTarihi.ToString()) liste.Add(new string[] { this.Key.tecilTarihi, yeni.tecilTarihi.ToString(), eski.tecilTarihi.ToString() });
                    if (yeni.yedekSubayOkuluGirisTarihi.ToString() != eski.yedekSubayOkuluGirisTarihi.ToString()) liste.Add(new string[] { this.Key.yedekSubayOkuluGirisTarihi, yeni.yedekSubayOkuluGirisTarihi.ToString(), eski.yedekSubayOkuluGirisTarihi.ToString() });
                    if (yeni.astegmenNaspTarihi.ToString() != eski.astegmenNaspTarihi.ToString()) liste.Add(new string[] { this.Key.astegmenNaspTarihi, yeni.astegmenNaspTarihi.ToString(), eski.astegmenNaspTarihi.ToString() });
                    if (yeni.tegmenNaspTarihi.ToString() != eski.tegmenNaspTarihi.ToString()) liste.Add(new string[] { this.Key.tegmenNaspTarihi, yeni.tegmenNaspTarihi.ToString(), eski.tegmenNaspTarihi.ToString() });
                    if (yeni.erYedekSubayGecisTarihi.ToString() != eski.erYedekSubayGecisTarihi.ToString()) liste.Add(new string[] { this.Key.erYedekSubayGecisTarihi, yeni.erYedekSubayGecisTarihi.ToString(), eski.erYedekSubayGecisTarihi.ToString() });
                    if (yeni.erYedekSubayKitaHizmetiBaslamaTarihi.ToString() != eski.erYedekSubayKitaHizmetiBaslamaTarihi.ToString()) liste.Add(new string[] { this.Key.erYedekSubayKitaHizmetiBaslamaTarihi, yeni.erYedekSubayKitaHizmetiBaslamaTarihi.ToString(), eski.erYedekSubayKitaHizmetiBaslamaTarihi.ToString() });
                    if (yeni.erYedekSubayKitaHizmetiBitisTarihi.ToString() != eski.erYedekSubayKitaHizmetiBitisTarihi.ToString()) liste.Add(new string[] { this.Key.erYedekSubayKitaHizmetiBitisTarihi, yeni.erYedekSubayKitaHizmetiBitisTarihi.ToString(), eski.erYedekSubayKitaHizmetiBitisTarihi.ToString() });
                    if (yeni.ibrazIntikalTarihi.ToString() != eski.ibrazIntikalTarihi.ToString()) liste.Add(new string[] { this.Key.ibrazIntikalTarihi, yeni.ibrazIntikalTarihi.ToString(), eski.ibrazIntikalTarihi.ToString() });

                    if (yeni.askerlikDurumu != eski.askerlikDurumu) liste.Add(new string[] { this.Key.askerlikDurumu, yeni.askerlikDurumu > 0 ? IKYSozluk.SozlukAdGetir(yeni.askerlikDurumu) : "", eski.askerlikDurumu > 0 ? IKYSozluk.SozlukAdGetir(eski.askerlikDurumu) : "" });
                    if (yeni.rutbesi != eski.rutbesi) liste.Add(new string[] { this.Key.rutbesi, yeni.rutbesi > 0 ? IKYSozluk.SozlukAdGetir(yeni.rutbesi) : "", eski.rutbesi > 0 ? IKYSozluk.SozlukAdGetir(eski.rutbesi) : "" });
                    if (yeni.sinifi != eski.sinifi) liste.Add(new string[] { this.Key.sinifi, yeni.sinifi > 0 ? IKYSozluk.SozlukAdGetir(yeni.sinifi) : "", eski.sinifi > 0 ? IKYSozluk.SozlukAdGetir(eski.sinifi) : "" });

                    if (yeni.askerlikCuzdanNo != eski.askerlikCuzdanNo) liste.Add(new string[] { this.Key.askerlikCuzdanNo, yeni.askerlikCuzdanNo, eski.askerlikCuzdanNo });
                    if (yeni.askerlikSubesi != eski.askerlikSubesi) liste.Add(new string[] { this.Key.askerlikSubesi, yeni.askerlikSubesi, eski.askerlikSubesi });
                    if (yeni.sayilmayanGunSayisi != eski.sayilmayanGunSayisi) liste.Add(new string[] { this.Key.sayilmayanGunSayisi, yeni.sayilmayanGunSayisi.ToString(), eski.sayilmayanGunSayisi.ToString() });
                    if (yeni.belgeNo != eski.belgeNo) liste.Add(new string[] { this.Key.belgeNo, yeni.belgeNo, eski.belgeNo });
                    if (yeni.borclanmaVarMi != eski.borclanmaVarMi) liste.Add(new string[] { this.Key.borclanmaVarMi, GetKodAd(yeni.borclanmaVarMi, EnumKodAd.Borclanma), GetKodAd(eski.borclanmaVarMi, EnumKodAd.Borclanma) });
                    if (yeni.askerlikMuafNeden != eski.askerlikMuafNeden) liste.Add(new string[] { this.Key.askerlikMuafNeden, yeni.askerlikMuafNeden, eski.askerlikMuafNeden });
                    if (yeni.aciklama != eski.aciklama) liste.Add(new string[] { this.Key.aciklama, yeni.aciklama, eski.aciklama });
                    if (yeni.sinifDonemSicilNo != eski.sinifDonemSicilNo) liste.Add(new string[] { this.Key.sinifDonemSicilNo, yeni.sinifDonemSicilNo, eski.sinifDonemSicilNo });
                    if (yeni.erYedekSubayGorevYeri != eski.erYedekSubayGorevYeri) liste.Add(new string[] { this.Key.erYedekSubayGorevYeri, yeni.erYedekSubayGorevYeri, eski.erYedekSubayGorevYeri });
                }
                else
                    liste.Add(new string[] { this.Key.DegisiklikYok, "", "" });

                Hashtable table = new Hashtable();
                table.Add(TagList.Liste, liste);
                table.Add(TagList.Bilgi, new object[] { new string[] { "IslemTipi", islem.ToString() }, new string[] { TagList.PersonelKod, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() }, new string[] { TagList.SiraNo, yeni.kod > 0 ? yeni.kod.ToString() : eski.kod.ToString() } });

                return table;
            }

        }
        public class AskerlikKey
        {
            public string askerlikDurumu { get; protected set; }
            public string askerlikBaslamaTarihi { get; protected set; }
            public string terhisTarihi { get; protected set; }
            public string askerlikCuzdanNo { get; protected set; }
            public string askerlikSubesi { get; protected set; }
            public string sayilmayanGunSayisi { get; protected set; }
            public string belgeNo { get; protected set; }
            public string belgeTarihi { get; protected set; }
            public string borclanmaVarMi { get; protected set; }
            public string tecilTarihi { get; protected set; }
            public string yedekSubayOkuluGirisTarihi { get; protected set; }
            public string rutbesi { get; protected set; }
            public string sinifi { get; protected set; }
            public string astegmenNaspTarihi { get; protected set; }
            public string tegmenNaspTarihi { get; protected set; }
            public string erYedekSubayGecisTarihi { get; protected set; }
            public string erYedekSubayKitaHizmetiBaslamaTarihi { get; protected set; }
            public string erYedekSubayKitaHizmetiBitisTarihi { get; protected set; }
            public string ibrazIntikalTarihi { get; protected set; }
            public string askerlikMuafNeden { get; protected set; }
            public string aciklama { get; protected set; }
            public string sinifDonemSicilNo { get; protected set; }
            public string erYedekSubayGorevYeri { get; protected set; }
            public string DegisiklikYok { get; protected set; }

            public AskerlikKey()
            {
                askerlikDurumu = "askerlikDurumu";
                askerlikBaslamaTarihi = "askerlikBaslamaTarihi";
                terhisTarihi = "terhisTarihi";
                askerlikCuzdanNo = "askerlikCuzdanNo";
                askerlikSubesi = "askerlikSubesi";
                sayilmayanGunSayisi = "sayilmayanGunSayisi";
                borclanmaVarMi = "borclanmaVarMi";
                belgeNo = "BelgeNo";
                belgeTarihi = "BelgeTarih";
                tecilTarihi = "tecilTarihi";
                yedekSubayOkuluGirisTarihi = "yedekSubayOkuluGirisTarihi";
                rutbesi = "rutbesi";
                sinifi = "sinifi";
                astegmenNaspTarihi = "astegmenNaspTarihi";
                tegmenNaspTarihi = "tegmenNaspTarihi";
                erYedekSubayGecisTarihi = "erYedekSubayGecisTarihi";
                erYedekSubayKitaHizmetiBaslamaTarihi = "erYedekSubayKitaHizmetiBaslamaTarihi";
                erYedekSubayKitaHizmetiBitisTarihi = "erYedekSubayKitaHizmetiBitisTarihi";
                ibrazIntikalTarihi = "ibrazIntikalTarihi";
                askerlikMuafNeden = "askerlikMuafNeden";
                aciklama = "aciklama";
                sinifDonemSicilNo = "sinifDonemSicilNo";
                erYedekSubayGorevYeri = "erYedekSubayGorevYeri";
                DegisiklikYok = "DegisiklikYok";
            }
        }
        public class AskerlikKeyAd : AskerlikKey
        {
            public AskerlikKeyAd()
            {
                base.askerlikDurumu = "Askerlik Durumu";
                base.askerlikBaslamaTarihi = "Askerlik Başlama Tarihi";
                base.terhisTarihi = "Terhis Tarihi";
                base.askerlikCuzdanNo = "Askerlik Cüzdan No";
                base.askerlikSubesi = "Askerlik Şubesi";
                base.sayilmayanGunSayisi = "Sayılmayan Gün Sayısı";
                base.borclanmaVarMi = "Borçlanma Var Mı?";
                base.belgeNo = "Belge No";
                base.belgeTarihi = "Belge Tarihi";
                base.tecilTarihi = "Tecil Tarihi";
                base.yedekSubayOkuluGirisTarihi = "Yedek Subay Okulu Giriş Tarihi";
                base.rutbesi = "Rütbesi";
                base.sinifi = "Sınıfı";
                base.astegmenNaspTarihi = "Asteğmen Nasp Tarihi";
                base.tegmenNaspTarihi = "Teğmen Nasp Tarihi";
                base.erYedekSubayGecisTarihi = "Erlikten Yedek Subaylığa Geçiş Tarihi";
                base.erYedekSubayKitaHizmetiBaslamaTarihi = "Kıta Hizmeti Başlama Tarihi";
                base.erYedekSubayKitaHizmetiBitisTarihi = "Kıta Hizmeti Bitiş Tarihi";
                base.ibrazIntikalTarihi = "İbraz İntikal Tarihi";
                base.askerlikMuafNeden = "Askerlik Muaf Nedeni";
                base.aciklama = "Açıkklama";
                base.sinifDonemSicilNo = "Sınıf Dönem Sicil No";
                base.erYedekSubayGorevYeri = "Er Yedek Subay Görev Yeri";
                base.DegisiklikYok = "Değişiklik Yok";
            }
        }

        #endregion
    }
}

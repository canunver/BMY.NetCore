using System;
using System.Collections;
using System.Collections.Generic;

namespace TNS
{
    /// <summary>
    /// Sunucunun Sonuc ile bildirdiği hatanın, tipli (generic) dönüşlerde ve API çağrılarında
    /// istemciye taşınması için kullanılan istisna.
    /// </summary>
    public class TNSIslemHatasi : Exception
    {
        public Sonuc sonuc { get; }

        public TNSIslemHatasi(Sonuc pSonuc)
            : base(pSonuc != null && !string.IsNullOrEmpty(pSonuc.hataStr) ? pSonuc.hataStr : "İşlem yapılamadı.")
        {
            sonuc = pSonuc ?? new Sonuc(false, "İşlem yapılamadı.", "");
        }

        public TNSIslemHatasi(string hata) : this(new Sonuc(false, hata, "")) { }
    }

    internal static class GenericDonusum
    {
        /// <summary>
        /// Tipsiz listedeki bir elemanı T tipine çevirir.
        /// Eleman hata bildiren bir Sonuc ise (eski kodda hata, listenin içine Sonuc eklenerek bildiriliyordu)
        /// TNSIslemHatasi fırlatır. atla=true dönerse eleman listeye alınmaz.
        /// </summary>
        public static T Cevir<T>(object o, out bool atla)
        {
            atla = false;
            if (o == null) return default(T);
            if (o is T) return (T)o;

            Sonuc s = o as Sonuc;
            if (s != null)
            {
                if (!s.islemSonuc) throw new TNSIslemHatasi(s);
                atla = true;
                return default(T);
            }

            Type hedef = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (o is IConvertible && (hedef.IsPrimitive || hedef == typeof(string) || hedef == typeof(decimal) || hedef == typeof(DateTime)))
                return (T)Convert.ChangeType(o, hedef, System.Globalization.CultureInfo.InvariantCulture);
            if (hedef.IsEnum && o is IConvertible)
                return (T)Enum.ToObject(hedef, o);

            throw new InvalidCastException("Listedeki '" + o.GetType().FullName + "' tipindeki eleman '" + typeof(T).FullName + "' tipine çevrilemedi.");
        }
    }

    /// <summary>
    /// TNSCollection sınıfının tipli karşılığı. JSON'a düz dizi olarak çevrilir.
    /// Eski (tipsiz) TNSCollection ile iki yönde örtük dönüşümü vardır.
    /// </summary>
    [Serializable()]
    public class TNSCollection<T> : List<T>
    {
        public TNSCollection()
        {
        }

        public TNSCollection(params T[] tut)
        {
            if (tut != null) AddRange(tut);
        }

        /// <summary>ArrayList.ToArray(Type) ile uyumluluk içindir.</summary>
        public Array ToArray(Type tip)
        {
            Array dizi = Array.CreateInstance(tip, Count);
            ((ICollection)this).CopyTo(dizi, 0);
            return dizi;
        }

        public static TNSCollection<T> Donustur(IEnumerable kaynak)
        {
            if (kaynak == null) return null;
            TNSCollection<T> hedef = new TNSCollection<T>();
            foreach (object o in kaynak)
            {
                bool atla;
                T deger = GenericDonusum.Cevir<T>(o, out atla);
                if (!atla) hedef.Add(deger);
            }
            return hedef;
        }

        public TNSCollection Tipsiz()
        {
            TNSCollection hedef = new TNSCollection();
            foreach (T o in this) hedef.Add(o);
            return hedef;
        }

        public static implicit operator TNSCollection<T>(TNSCollection kaynak)
        {
            return Donustur(kaynak);
        }

        public static implicit operator TNSCollection(TNSCollection<T> kaynak)
        {
            return kaynak == null ? null : kaynak.Tipsiz();
        }
    }

    /// <summary>
    /// ObjectArray sınıfının tipli karşılığı: işlem sonucu + T tipinde nesne listesi.
    /// Eski (tipsiz) ObjectArray ile iki yönde örtük dönüşümü vardır.
    /// </summary>
    [Serializable()]
    public class ObjectArray<T>
    {
        public Sonuc sonuc;
        public TNSCollection<T> objeler;

        public int ObjeSayisi
        {
            get { return objeler == null ? 0 : objeler.Count; }
        }

        public ObjectArray()
        {
            objeler = new TNSCollection<T>();
            sonuc = new Sonuc();
        }

        public void HataAta(string hata)
        {
            sonuc.islemSonuc = false;
            sonuc.hataStr = hata;
        }

        /// <summary>Eski ObjectArray.Al ile aynı davranır: indis aralık dışındaysa "" döndürür.</summary>
        public object Al(int indis)
        {
            if (indis < objeler.Count)
                return objeler[indis];
            return "";
        }

        public string AlStr(int indis)
        {
            if (indis < objeler.Count)
            {
                object o = objeler[indis];
                return o == null ? "" : o.ToString();
            }
            return "";
        }

        public void Ekle(T obje)
        {
            objeler.Add(obje);
        }

        public object this[int indis]
        {
            get { return Al(indis); }
        }

        public static ObjectArray<T> Donustur(ObjectArray kaynak)
        {
            if (kaynak == null) return null;
            ObjectArray<T> hedef = new ObjectArray<T>();
            hedef.sonuc = kaynak.sonuc;
            if (kaynak.objeler != null)
            {
                foreach (object o in kaynak.objeler)
                {
                    bool atla;
                    T deger;
                    try
                    {
                        deger = GenericDonusum.Cevir<T>(o, out atla);
                    }
                    catch (TNSIslemHatasi h)
                    {
                        // Liste içine konmuş hata Sonuc'u, ObjectArray'in kendi sonuc alanına taşınır.
                        hedef.sonuc = h.sonuc;
                        continue;
                    }
                    if (!atla) hedef.objeler.Add(deger);
                }
            }
            return hedef;
        }

        public ObjectArray Tipsiz()
        {
            ObjectArray hedef = new ObjectArray();
            hedef.sonuc = sonuc;
            if (objeler != null)
                foreach (T o in objeler) hedef.objeler.Add(o);
            return hedef;
        }

        public static implicit operator ObjectArray<T>(ObjectArray kaynak)
        {
            return Donustur(kaynak);
        }

        public static implicit operator ObjectArray(ObjectArray<T> kaynak)
        {
            return kaynak == null ? null : kaynak.Tipsiz();
        }
    }

    /// <summary>
    /// ObjectVAN sınıfının tipli karşılığı: işlem sonucu + T tipinde tek nesne.
    /// Eski (tipsiz) ObjectVAN ile iki yönde örtük dönüşümü vardır.
    /// </summary>
    [Serializable()]
    public class ObjectVAN<T>
    {
        public T o;
        public Sonuc sonuc = new Sonuc();

        public ObjectVAN()
        {
        }

        public ObjectVAN(T nesne)
        {
            o = nesne;
        }

        public static ObjectVAN<T> Donustur(ObjectVAN kaynak)
        {
            if (kaynak == null) return null;
            ObjectVAN<T> hedef = new ObjectVAN<T>();
            hedef.sonuc = kaynak.sonuc;
            // Eski ObjectArray indeksleyicisi liste boşken "" döndürdüğü için boş dizgi "nesne yok" sayılır.
            if (kaynak.o != null && !(kaynak.o is string && typeof(T) != typeof(string) && (string)kaynak.o == ""))
            {
                bool atla;
                try
                {
                    T deger = GenericDonusum.Cevir<T>(kaynak.o, out atla);
                    if (!atla) hedef.o = deger;
                }
                catch (TNSIslemHatasi h)
                {
                    hedef.sonuc = h.sonuc;
                }
            }
            return hedef;
        }

        public ObjectVAN Tipsiz()
        {
            ObjectVAN hedef = new ObjectVAN();
            hedef.sonuc = sonuc;
            hedef.o = o;
            return hedef;
        }

        public static implicit operator ObjectVAN<T>(ObjectVAN kaynak)
        {
            return Donustur(kaynak);
        }

        public static implicit operator ObjectVAN(ObjectVAN<T> kaynak)
        {
            return kaynak == null ? null : kaynak.Tipsiz();
        }
    }
}

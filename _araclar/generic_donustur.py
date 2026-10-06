# -*- coding: utf-8 -*-
"""Servis arayuzlerinde ObjectArray / ObjectVAN / TNSCollection donen metotlarin eleman tipini (T) sunucu kodundan
cikarir; arayuz ve servis (facade) sinifindaki donus tipini ObjectArray<T> / ObjectVAN<T> / TNSCollection<T> yapar.
Kullanim: python3 generic_donustur.py <yeniSunucu> <eskiSunucu> <MODUL,MODUL..> [--uygula]
Emin olunamayan metotlara dokunulmaz; hepsi generic_rapor.md dosyasina yazilir."""
import os,re,sys,json,pickle,collections
from csindeks import *
YENI,ESKI=sys.argv[1],sys.argv[2]; MODULLER=sys.argv[3].split(','); UYGULA='--uygula' in sys.argv
RK={'ObjectArray':'OA','TNSCollection':'TC','ObjectVAN':'OV'}
GEN={'OA':'ObjectArray','TC':'TNSCollection','OV':'ObjectVAN'}
BUILTIN={'string','int','double','decimal','bool','long','short','byte','float','char','DateTime','Guid','uint','ulong'}
ATLA={'bin','obj','.vs','Web References','Service References','Properties'}
def kisa(t): return t.split('.')[-1]
def rk(t):
    t=kisa(t)
    return RK.get(t)
# ---------------- indeks ----------------
CACHE=os.path.join(os.path.dirname(os.path.abspath(__file__)),'.indeks.pickle')
def indeksle():
    F={}
    yeniProj=set()
    for dp,dn,fn in os.walk(YENI):
        dn[:]=[d for d in dn if d not in ATLA]
        if any(f.endswith('.csproj') for f in fn): yeniProj.add(os.path.relpath(dp,YENI))
        for f in fn:
            if f.endswith('.cs') and not f.endswith('.Designer.cs'):
                p=os.path.join(dp,f); F[p]=tara(p)
    for dp,dn,fn in os.walk(ESKI):
        dn[:]=[d for d in dn if d not in ATLA]
        rel=os.path.relpath(dp,ESKI)
        if any(rel==y or rel.startswith(y+os.sep) for y in yeniProj): dn[:]=[]; continue
        for f in fn:
            if f.endswith('.cs') and not f.endswith('.Designer.cs'):
                p=os.path.join(dp,f)
                try: F[p]=tara(p)
                except Exception as e: print('okunamadi',p,e)
    return F
if os.path.exists(CACHE) and os.path.getsize(CACHE)>0 and '--yenile' not in sys.argv: F=pickle.load(open(CACHE,'rb'))
else:
    F=indeksle(); pickle.dump(F,open(CACHE,'wb'))
BYCLS=collections.defaultdict(list); IMPL={}; TYPES=collections.defaultdict(set); FIELDS={}
for p,f in F.items():
    for m in f.met: BYCLS[(m['cls'],m['ad'])].append(m)
    for c in f.cls:
        TYPES[c['ad']].add(nsbul(f,c['a']))
        if c['tur']=='class':
            for i in re.findall(r'\bI\w+',c['taban']): IMPL.setdefault(i,c['ad'])
def govde(m): return F[m['dosya']].s[m['a']+1:m['b']]
def proje(p):
    d=os.path.dirname(p)
    while d and not any(x.endswith('.csproj') for x in os.listdir(d)): d=os.path.dirname(d)
    return d
# ---------------- tip cozumleme ----------------
def nitele(m,t):
    """Kisa tip adini, bulundugu dosyanin using'lerine gore tam nitelikli yapar. Cozulemezse None."""
    if not t: return None
    dizi=''
    mm=re.match(r'^(.*?)((?:\[,?\])+)$',t)
    if mm: t,dizi=mm.group(1),mm.group(2)
    if t in BUILTIN or t=='String': return ('string' if t=='String' else t)+dizi
    if t=='object' or '<' in t: return None
    f=F[m['dosya']]; ad=kisa(t); onek=t[:-len(ad)].rstrip('.')
    adaylar=TYPES.get(ad)
    if not adaylar: return None
    if onek:
        for ns in adaylar:
            if ns==onek or ns.endswith('.'+onek): return ns+'.'+ad+dizi
        return None
    kendi=nsbul(f,m['a']); gorunen=set(f.usings)
    k=kendi
    while k: gorunen.add(k); k=k.rpartition('.')[0]
    uygun=[ns for ns in adaylar if ns in gorunen]
    if len(uygun)==1: return (uygun[0]+'.' if uygun[0] else '')+ad+dizi
    if len(uygun)>1:
        # en yakin (kendi namespace zinciri) onceliklidir
        k=kendi
        while k:
            if k in uygun: return k+'.'+ad+dizi
            k=k.rpartition('.')[0]
    return None
def yerel_tip(m,ident):
    for t,n in m['p']:
        if n==ident: return t
    b=govde(m)
    for mm in re.finditer(r'(?<![\w.])('+TIP+r')\s+'+re.escape(ident)+r'\s*(=(?!=)|;|,|\)|\bin\b)',b):
        t=norm(mm.group(1))
        if t=='var':
            m2=re.search(r'\bvar\s+'+re.escape(ident)+r'\s*=\s*new\s+('+TIP+r')\s*[({]',b)
            if m2: return norm(m2.group(1))
            m2=re.search(r'\bvar\s+'+re.escape(ident)+r'\s*=\s*\(('+TIP+r')\)',b)
            if m2: return norm(m2.group(1))
            return None
        if t.split('<')[0] in KW: continue
        return t
    # sinif alani
    f=F[m['dosya']]
    for c in f.cls:
        if c['ad']==m['cls']:
            mm=re.search(r'(?<![\w.])('+TIP+r')\s+'+re.escape(ident)+r'\s*(=(?!=)|;)',f.s[c['a']:c['b']])
            if mm and norm(mm.group(1)).split('<')[0] not in KW: return norm(mm.group(1))
    return None
def ifade_oku(b,i):
    d=0;k=i
    while k<len(b):
        ch=b[k]
        if ch in '([{':d+=1
        elif ch in ')]}':
            if d==0:break
            d-=1
        elif ch in ';' and d==0:break
        k+=1
    return b[i:k].strip()
def arg_oku(b,i):
    """i: '(' sonrasi konum. Ust duzey argumanlari dondurur."""
    j=eslesen(b,i-1,'(',')')
    return bol(b[i:j])
def soyul(e):
    e=e.strip()
    while e.startswith('(') and eslesen(e,0,'(',')')==len(e)-1: e=e[1:-1].strip()
    return e
def cagri_coz(m,e):
    """e bir metot cagrisi ise aday metotlari dondurur, degilse None."""
    e=soyul(e)
    if not e.endswith(')'): return None
    d=0;k=len(e)-1
    while k>=0:
        if e[k]==')':d+=1
        elif e[k]=='(':
            d-=1
            if d==0:break
        k-=1
    if k<=0: return None
    args=bol(e[k+1:-1]); onceki=e[:k].rstrip()
    mm=re.search(r'(\w+)$',onceki)
    if not mm: return None
    ad=mm.group(1); hedef=onceki[:mm.start()].rstrip()
    if hedef.endswith('.'): hedef=hedef[:-1].rstrip()
    elif hedef: return None   # "new X(...)" gibi: cagri degil
    cls=None
    if hedef in('','this','base'): cls=m['cls']
    else:
        h=soyul(hedef)
        m2=re.match(r'^new\s+([\w.]+)\s*\(.*\)$',h,re.S)
        m3=re.search(r'TNS\.(\w+)\.Arac\.Tanimla\(\s*\)$',h)
        if m2: cls=kisa(m2.group(1))
        elif m3: cls='I'+m3.group(1)+'Servis'
        elif re.match(r'^\w+$',h):
            t=yerel_tip(m,h)
            cls=kisa(t) if t else h
        elif re.match(r'^[\w.]+$',h): cls=kisa(h)
        else: return []
    cls=IMPL.get(cls,cls)
    ad_=BYCLS.get((cls,ad),[])
    c=[x for x in ad_ if len(x['p'])==len(args)]
    if not c:
        # varsayilan parametreli olabilir
        c=[x for x in ad_ if len(x['p'])>len(args)]
    return c
MEMO={};YIGIN=set()
def donus_eleman(c,kind):
    """c metodunun dondurdugu koleksiyon/VAN icindeki eleman tipleri kumesi ('?' = bilinmiyor)."""
    mg=re.match(r'^(?:[\w.]*\.)?(ObjectArray|TNSCollection|ObjectVAN)<(.+)>$',c['ret'])
    if mg:
        q=nitele(c,mg.group(2)); return {q or '?'}
    key=(c['dosya'],c['a'],kind)
    if key in MEMO: return MEMO[key]
    if key in YIGIN: return set()
    if len(YIGIN)>12: return {'?'}
    YIGIN.add(key); r=set(); b=govde(c)
    for mm in re.finditer(r'\breturn\b\s*',b):
        e=ifade_oku(b,mm.end())
        if e: r|=kol_ifade(c,e,kind)
    YIGIN.discard(key); MEMO[key]=r
    return r
def kol_ifade(m,e,kind):
    """Koleksiyon (veya VAN) ureten ifadenin eleman tipleri."""
    e=soyul(e)
    if e in('null','default'): return set()
    mm=re.match(r'^new\s+(?:[\w.]*\.)?(ObjectArray|TNSCollection|ObjectVAN)\s*\((.*)\)\s*(\{.*\})?$',e,re.S)
    if mm:
        r=set()
        if mm.group(1)=='TNSCollection':
            for a in bol(mm.group(2)): r|=tip_ifade(m,a)
        if mm.group(3): r.add('?')
        return r
    mm=re.match(r'^\(\s*(?:[\w.]*\.)?(ObjectArray|TNSCollection|ObjectVAN)\s*\)(.*)$',e,re.S)
    if mm: return kol_ifade(m,mm.group(2),kind)
    if re.match(r'^\w+$',e):
        t=yerel_tip(m,e); k=rk(t) if t else None
        if not k: return {'?'}
        return deg_eleman(m,e,k)
    mm=re.match(r'^(\w+)\s*\.\s*objeler$',e)
    if mm: return deg_eleman(m,mm.group(1),'OA')
    mm=re.match(r'^(.*\))\s*\.\s*objeler$',e,re.S)
    if mm: e=mm.group(1)
    c=cagri_coz(m,e)
    if c is None or not c: return {'?'}
    r=set()
    for x in c:
        k=rk(x['ret'].split('<')[0])
        if not k: r.add('?'); continue
        r|=donus_eleman(x,k)
    return r
DMEMO={}
def deg_eleman(m,var,kind):
    key=(m['dosya'],m['a'],var,kind)
    if key in DMEMO: return DMEMO[key]
    DMEMO[key]=set()
    r=set(); b=govde(m); v=r'(?<![\w.])'+re.escape(var)
    for mm in re.finditer(v+r'\s*=(?!=)\s*',b):
        r|=kol_ifade(m,ifade_oku(b,mm.end()),kind)
    def ekle(pat,idx):
        for mm in re.finditer(v+pat,b):
            a=arg_oku(b,mm.end())
            if len(a)>idx: r.update(tip_ifade(m,a[idx]))
    def aralik(pat):
        for mm in re.finditer(v+pat,b):
            a=arg_oku(b,mm.end())
            if a: r.update(kol_ifade(m,a[0],'TC'))
    if kind=='OA':
        ekle(r'\s*\.\s*objeler\s*\.\s*Add\s*\(',0); ekle(r'\s*\.\s*Ekle\s*\(',0); ekle(r'\s*\.\s*objeler\s*\.\s*Insert\s*\(',1); aralik(r'\s*\.\s*objeler\s*\.\s*AddRange\s*\(')
        for mm in re.finditer(v+r'\s*\.\s*objeler\s*=(?!=)\s*',b): r|=kol_ifade(m,ifade_oku(b,mm.end()),'TC')
    elif kind=='TC':
        ekle(r'\s*\.\s*Add\s*\(',0); ekle(r'\s*\.\s*Insert\s*\(',1); aralik(r'\s*\.\s*AddRange\s*\(')
    elif kind=='OV':
        for mm in re.finditer(v+r'\s*\.\s*o\s*=(?!=)\s*',b): r|=tip_ifade(m,ifade_oku(b,mm.end()))
    # degisken baska bir metoda arguman olarak verilmis mi?
    for mm in re.finditer(r'[(,]\s*(?:ref\s+|out\s+)?'+re.escape(var)+r'\s*(?=[,)])',b):
        d=0;k=mm.start()
        if b[k]==',':
            while k>=0:
                if b[k]==')':d+=1
                elif b[k]=='(':
                    if d==0:break
                    d-=1
                k-=1
        if k<0: continue
        once=re.search(r'([\w.]+)\s*$',b[:k])
        if not once: continue
        zincir=once.group(1); ad=kisa(zincir)
        if ad in('Add','AddRange','Ekle','Insert','if','while','foreach','using','lock','switch','return','Contains','IndexOf','Remove'): continue
        args=arg_oku(b,k+1); idx=next((i for i,a in enumerate(args) if re.sub(r'^(ref|out)\s+','',a.strip())==var),None)
        c=cagri_coz(m,b[once.start():eslesen(b,k,'(',')')+1])
        if not c or idx is None: r.add('?'); continue
        for x in c:
            if idx<len(x['p']) and rk(x['p'][idx][0]): r|=deg_eleman(x,x['p'][idx][1],rk(x['p'][idx][0]))
            else: r.add('?')
    DMEMO[key]=r
    return r
def tip_ifade(m,e):
    """Tekil bir ifadenin (listeye eklenen nesne) tipi."""
    e=soyul(e)
    def q(t):
        n=nitele(m,t); return {n} if n else {'?'}
    if e in('null',''): return set()
    if e.startswith('"') or e.endswith('.ToString()') or re.match(r'^[sS]tring\s*\.',e) or re.search(r'\+\s*""\s*$',e): return {'string'}
    if re.match(r'^-?\d+$',e): return {'int'}
    if re.match(r'^-?\d+\.\d+$',e): return {'double'}
    if e in('true','false'): return {'bool'}
    mm=re.match(r'^new\s+('+TIP+r')\s*[({]',e)
    if mm:
        t=norm(mm.group(1))
        mm2=re.match(r'^new\s+[\w.]+\s*\[',e)
        return q(t+('[]' if mm2 and not t.endswith(']') else ''))
    mm=re.match(r'^\(\s*('+TIP+r')\s*\)\s*[\w("(]',e)
    if mm and norm(mm.group(1)).split('<')[0] not in KW:
        n=nitele(m,norm(mm.group(1)))
        if n: return {n}
    if re.match(r'^\w+$',e):
        t=yerel_tip(m,e)
        return q(t) if t else {'?'}
    mm=re.match(r'^(\w+)\s*(?:\.\s*objeler\s*)?\[.*\]$',e,re.S) or re.match(r'^(\w+)\s*\.\s*Al\s*\(.*\)$',e,re.S)
    if mm:
        t=yerel_tip(m,mm.group(1)); k=rk(t) if t else None
        if k in('OA','TC'): return deg_eleman(m,mm.group(1),k)
    mm=re.match(r'^(\w+)\s*\.\s*o$',e)
    if mm:
        t=yerel_tip(m,mm.group(1))
        if t and rk(t)=='OV': return deg_eleman(m,mm.group(1),'OV')
    mm=re.match(r'^VeriErisim\s*\.\s*DegerAl\s*\((.*)\)$',e,re.S)
    if mm:
        a=bol(mm.group(1))
        if len(a)==3:
            if a[2].startswith('"'): return {'string'}
            if re.match(r'^-?\d+$',a[2]): return {'int'}
            if re.match(r'^-?\d+\.\d+$',a[2]): return {'double'}
    mm=re.match(r'^(?:TNS\.)?OrtakFonksiyonlar\s*\.\s*ConvertTo(Int|Str|Dbl|Double)\b',e)
    if mm: return {{'Int':'int','Str':'string','Dbl':'double','Double':'double'}[mm.group(1)]}
    c=cagri_coz(m,e)
    if c:
        r=set()
        for x in c:
            n=nitele(x,x['ret']) if not rk(x['ret']) else None
            r.add(n or '?')
        return r
    return {'?'}
# ---------------- arayuz metotlari ----------------
def ptip(p): return [kisa(t.replace('TNS.','').split('<')[0])+('<' if '<' in t else '') for t,_ in p]
sonuc=[];duzen=collections.defaultdict(list)
for mod in MODULLER:
    for p,f in F.items():
        if not p.startswith(YENI) or (os.sep+mod+'Servis'+os.sep) not in p and (os.sep+mod+'ServisI'+os.sep) not in p: continue
        for im in f.imet:
            if not re.match(r'^I\w+Servis$',im['arayuz']): continue   # eklenti arayuzlerine (IDuyuruListesi vb.) dokunma
            k=rk(im['ret'].split('<')[0])
            if not k: continue
            if '<' in im['ret']:
                sonuc.append(dict(modul=mod,arayuz=im['arayuz'],metot=im['ad'],param=', '.join(t+' '+n for t,n in im['p']),donus=GEN[k],durum='donusturuldu',T=im['ret'][im['ret'].index('<')+1:-1],tipler=[])); continue
            impl=IMPL.get(im['arayuz'])
            fac=[x for x in BYCLS.get((impl,im['ad']),[]) if ptip(x['p'])==ptip(im['p'])]
            kayit=dict(modul=mod,arayuz=im['arayuz'],metot=im['ad'],param=', '.join(t+' '+n for t,n in im['p']),donus=im['ret'])
            if not fac: kayit.update(durum='uygulama-bulunamadi',tipler=[]); sonuc.append(kayit); continue
            tip=set()
            for x in fac: tip|=donus_eleman(x,k)
            ham=sorted(tip)
            if len(tip-{'?'})>1 or (k!='OV'): tip.discard('TNS.Sonuc')
            if len(tip)>1 and tip>={'TNS.Sonuc'} and k=='OV': tip.discard('TNS.Sonuc')
            somut=tip-{'?'}
            if len(somut)==1 and '?' not in tip:
                T=next(iter(somut)); kayit.update(durum='donusturuldu',T=T,tipler=ham)
                def yaz(dosya,ns,T=T):
                    if '.' in T and T.rpartition('.')[0]==ns: return kisa(T)
                    return T
                duzen[im['dosya']].append((im['rs'],im['re'],GEN[k]+'<'+yaz(im['dosya'],nsbul(f,im['rs']))+'>'))
                for x in fac:
                    fx=F[x['dosya']]
                    duzen[x['dosya']].append((x['rs'],x['re'],GEN[k]+'<'+yaz(x['dosya'],nsbul(fx,x['rs']))+'>'))
            elif len(somut)==1: kayit.update(durum='muhtemel',T=next(iter(somut)),tipler=ham)
            elif len(somut)>1: kayit.update(durum='karisik',tipler=ham)
            else: kayit.update(durum='bilinmiyor',tipler=ham)
            sonuc.append(kayit)
say=collections.Counter((r['modul'],r['durum']) for r in sonuc)
for k,v in sorted(say.items()): print(k,v)
if UYGULA:
    for dosya,ds in duzen.items():
        raw=open(dosya,'rb').read(); bom=raw.startswith(b'\xef\xbb\xbf'); crlf=b'\r\n' in raw
        s=open(dosya,encoding='utf-8-sig',errors='ignore').read()
        assert len(s)==len(F[dosya].s),dosya
        for a,b,yeni in sorted(set(ds),reverse=True): s=s[:a]+yeni+s[b:]
        if crlf: s=s.replace('\n','\r\n')
        open(dosya,'wb').write((b'\xef\xbb\xbf' if bom else b'')+s.encode('utf-8'))
    print('duzenlenen dosya',len(duzen))
    open(CACHE,'wb').close()
A=os.path.dirname(os.path.abspath(__file__))
onceki=[]
jp=os.path.join(A,'generic_rapor.json')
if os.path.exists(jp) and UYGULA: onceki=[r for r in json.load(open(jp,encoding='utf-8')) if r['modul'] not in MODULLER]
if UYGULA: json.dump(onceki+sonuc,open(jp,'w',encoding='utf-8'),ensure_ascii=False,indent=1)
else: json.dump(sonuc,open(os.path.join(A,'generic_onizleme.json'),'w',encoding='utf-8'),ensure_ascii=False,indent=1)

if UYGULA:
    tum=onceki+sonuc
    L=['# Generic dönüşüm raporu','','Arayüzlerde `ObjectArray` / `ObjectVAN` / `TNSCollection` dönen metotlar ve bulunan eleman tipi.','']
    for mod in sorted(set(r['modul'] for r in tum)):
        rs=[r for r in tum if r['modul']==mod]
        L+=['## '+mod,'','%d metot: %d dönüştürüldü, %d dönüştürülmedi.'%(len(rs),sum(r['durum']=='donusturuldu' for r in rs),sum(r['durum']!='donusturuldu' for r in rs)),'']
        kalan=[r for r in rs if r['durum']!='donusturuldu']
        if kalan:
            L+=['### Dönüştürülmeyenler (elle karar gerekli)','','| Metot | Dönüş | Durum | Bulunan tipler |','|---|---|---|---|']
            L+=['| `%s` | %s | %s | %s |'%(r['metot'],r['donus'],r['durum'],', '.join(r['tipler']) or '-') for r in kalan]; L.append('')
        L+=['### Dönüştürülenler','','| Metot | Yeni dönüş tipi |','|---|---|']
        L+=['| `%s` | `%s<%s>` |'%(r['metot'],r['donus'],r['T']) for r in rs if r['durum']=='donusturuldu']; L.append('')
    open(os.path.join(A,'generic_rapor.md'),'w',encoding='utf-8').write('\n'.join(L))

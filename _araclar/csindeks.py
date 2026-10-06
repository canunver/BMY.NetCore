# -*- coding: utf-8 -*-
"""C# kaynaklarini (regex + parantez eslestirme ile) tarayip sinif/metot/tip indeksi cikarir."""
import os,re,pickle,sys
KW={'return','new','in','is','as','else','case','throw','yield','await','using','typeof','goto','out','ref','params','this','static','const','readonly','public','private','protected','internal','if','while','for','foreach','switch','lock','catch','var','default','delegate','event','operator','where','select','from'}
def strip(s):
    """Yorum ve dizgi iceriklerini bosluklarla degistirir (uzunluk korunur). Dizgiler "..." olarak kalir."""
    o=list(s);i=0;n=len(s)
    while i<n:
        c=s[i];d=s[i+1] if i+1<n else ''
        if c=='/' and d=='/':
            j=s.find('\n',i); j=n if j<0 else j
            for k in range(i,j):o[k]=' '
            i=j
        elif c=='/' and d=='*':
            j=s.find('*/',i+2); j=n if j<0 else j+2
            for k in range(i,j):
                if s[k]!='\n':o[k]=' '
            i=j
        elif c=='"' or (c in '@$' and d=='"') or (c in '@$' and d in '@$' and i+2<n and s[i+2]=='"'):
            verb=False;st=i
            while s[i]!='"':
                if s[i]=='@':verb=True
                o[i]=' ';i+=1
            i+=1
            while i<n:
                if verb:
                    if s[i]=='"':
                        if i+1<n and s[i+1]=='"':o[i]=o[i+1]=' ';i+=2;continue
                        break
                else:
                    if s[i]=='\\':o[i]=' ';
                    if s[i]=='\\' and i+1<n:o[i+1]=' ';i+=2;continue
                    if s[i]=='"' or s[i]=='\n':break
                if s[i]!='\n':o[i]=' '
                i+=1
            i+=1
        elif c=="'":
            j=i+1
            if j<n and s[j]=='\\':j+=2
            else:j+=1
            while j<n and s[j]!="'" and j-i<8:j+=1
            for k in range(i+1,min(j,n)):o[k]=' '
            i=j+1
        else:i+=1
    return ''.join(o)
def eslesen(s,i,ac='{',kapa='}'):
    d=0
    for k in range(i,len(s)):
        if s[k]==ac:d+=1
        elif s[k]==kapa:
            d-=1
            if d==0:return k
    return len(s)-1
def bol(a):
    """Ust duzey virgullerden bol."""
    r=[];d=0;cur=''
    for ch in a:
        if ch in '<([{':d+=1
        elif ch in '>)]}':d-=1
        if ch==',' and d==0:r.append(cur);cur=''
        else:cur+=ch
    if cur.strip():r.append(cur)
    return [x.strip() for x in r]
def norm(t): return re.sub(r'\s+','',t)
def paramlar(p):
    out=[]
    for x in bol(p):
        x=re.sub(r'^\s*(\[[^\]]*\]\s*)*','',x); x=x.split('=')[0].strip()
        x=re.sub(r'^(ref|out|params|this|in)\s+','',x)
        m=re.match(r'(.+?)\s+(\w+)$',x,re.S)
        if m:out.append((norm(m.group(1)),m.group(2)))
    return out
TIP=r'[A-Za-z_][\w.]*(?:\s*<[^;(){}=]+?>)?(?:\s*\[\s*,?\s*\])?'
RE_CLS=re.compile(r'\b(class|interface|struct|enum)\s+(\w+)(\s*<[^>{]+>)?([^{;]*)\{')
RE_NS=re.compile(r'\bnamespace\s+([\w.]+)\s*\{')
RE_M=re.compile(r'(?P<mods>(?:\b(?:public|private|protected|internal|static|virtual|override|new|sealed|async|unsafe|extern)\s+)+)(?P<ret>'+TIP+r')\s+(?P<name>\w+)\s*\((?P<params>[^(){};]*)\)\s*\{')
RE_IM=re.compile(r'(?<=[;{}\]])\s*(?P<ret>'+TIP+r')\s+(?P<name>\w+)\s*\((?P<params>[^(){};]*)\)\s*;')
class Dosya: pass
def tara(path,text=None):
    raw=text if text is not None else open(path,encoding='utf-8-sig',errors='ignore').read()
    s=strip(raw); f=Dosya(); f.path=path; f.s=s; f.bom=False
    f.usings=re.findall(r'^\s*using\s+(?:static\s+)?([\w.]+)\s*;',s,re.M)
    f.ns=[(m.group(1),m.end()-1,eslesen(s,m.end()-1)) for m in RE_NS.finditer(s)]
    f.cls=[]
    for m in RE_CLS.finditer(s):
        a=m.end()-1; f.cls.append(dict(tur=m.group(1),ad=m.group(2),taban=m.group(4),a=a,b=eslesen(s,a)))
    f.met=[]
    for m in RE_M.finditer(s):
        if m.group('ret').split('<')[0].strip() in KW or m.group('name') in KW: continue
        a=m.end()-1
        f.met.append(dict(ret=norm(m.group('ret')),rs=m.start('ret'),re=m.end('ret'),ad=m.group('name'),p=paramlar(m.group('params')),a=a,b=eslesen(s,a),static='static' in m.group('mods'),cls=sinif(f,m.start()),dosya=path))
    f.imet=[]
    for c in f.cls:
        if c['tur']!='interface':continue
        body=s[c['a']:c['b']]
        for m in RE_IM.finditer(body):
            if m.group('ret').split('<')[0].strip() in KW: continue
            f.imet.append(dict(ret=norm(m.group('ret')),rs=c['a']+m.start('ret'),re=c['a']+m.end('ret'),ad=m.group('name'),p=paramlar(m.group('params')),arayuz=c['ad'],dosya=path))
    return f
def sinif(f,pos):
    best=None
    for c in f.cls:
        if c['a']<pos<c['b'] and c['tur']!='enum' and (best is None or c['a']>best['a']):best=c
    return best['ad'] if best else None
def nsbul(f,pos):
    best=''
    for n,a,b in f.ns:
        if a<pos<b:best=n
    return best

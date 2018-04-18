import re, os, csv, logging,collections,string
from tableparse_Restructure import *
from constants import *

# this key has to be only the numbers and after dash 1 letter
#todo later release cleanup . 
#
cstringregex = r'(C[\d_][\dA-Z-]*)'
pnemonicdict = collections.OrderedDict()
def readspecmap(specin):
    
    cfile = open(configfile, 'rt')
    creader = csv.reader(cfile,delimiter=',')
    specout='NF'
    for crow in creader:
        if(crow[0].strip() == specin):
            specout=crow[1]
            break
    return specout
def evalcvl(filename,trlfilespec):
    
    cfile = open(os.path.join(inputpath,filename), 'rb')
    creader = csv.reader(cfile,delimiter='\t')
    cvldict = collections.OrderedDict()
    for crow in creader:
        if(len(crow)<2):
            continue
        # if the condition key is not proper dont take it. 
        k = crow[0]
        v = crow[1]
        x = v
        v = re.sub(r'A[\.\s]*(\d+)\s*\/\s*(\d+)',r'A.\1/\2',v)
        v = v.replace('AND AND','AND')
        y = v
        if(x!=y):
            print 'Removing Extra Space:',k,x,'=>',y
        pnemonics = ''
        k=k.strip()
        v=v.replace(').',')')
        
        mk = re.match(cstringregex,k.strip(),re.I)
        if not mk: 
            continue
        if len(crow)>2:
            pnemonics = crow[2]
            #print 'b',pnemonics
            pnemonics = pnemonics.replace('-- ','')
            #print 'a',pnemonics
        if pnemonics.strip()!='':
            if k not in pnemonicdict:
                pnemonicdict[k]=pnemonics
            else:
                print 'Error: duplicate pnemonic '+k +' old-> '+pnemonicdict[k] +'new-> '+ pnemonics
        if '!' in v:
            v = v.replace('!','NOT ')
        #print k,v
        #mv = re.search(r'\bc\S+',v,re.I)
        #if mv:
        #    print k,v
        matchcase = ''
        nomatch = 1
        if nomatch:
            #IF A.1/63 THEN test step option n.A M ELSE test step option n.B M	-- O_longFTN
            m = re.search(r'IF (.+) THEN test step option n\.A M ELSE test step option n\.B M',v,re.I)
            if m:
                nomatch = 0
                cvldict[k]=m.group(1).strip()
                matchcase = '1'
                
                
        if nomatch:
            #IF C473 THEN O ELSE (IF A.1/2 AND A.18b/10 AND A.18p/9 THEN R ELSE N/A)
            m = re.search(r'if(.+?)then\s+o\s+else\s*\(?\s*if\s*\(?(.+?)\s*\)?\s*then\s+[arm]\s+else\s+n\/a',v,re.I)
            if m:
                nomatch = 0
                cvldict[k]=m.group(1).strip() +" OR "+m.group(2).strip()
                #print k,m1.group(1).strip() +" O "+m1.group(2).strip()
                matchcase = '12'
                
        if nomatch:
            #IF (C121 AND A.1/34) THEN test x.A M ELSE IF (C121 AND NOT A.1/34) test x.B M ELSE N/A
            m = re.search(r'if\s*(.+?)\s*then test x.A M else if (.+?) test x.b m else n\/a',v,re.I)
            if m:
                nomatch = 0
                cvldict[k] = '('+m.group(1)+' OR '+m.group(2) +')'
                matchcase = '2'
        if nomatch:
            #printable = set(string.printable)
            #v = filter(lambda x: x in printable,v)
            
            m = re.search(r'if (.+?) then .+expected sequence.+m',v,re.I)
            if m:
                nomatch = 0
                cvldict[k] = '('+m.group(1)+')'
                matchcase = '3'
                
        if nomatch:
            #A.1/33 THEN test x.A M ELSE test x.B M
            #A.1/22, test x.A M ELSE x.B M
            m = re.search(r'(then)?test\s*x.A M ELSE\s*(test)?\s*x.B M',v,re.I)
            if m:
                nomatch = 0
                cvldict[k] = 'R'
                matchcase = '4'
                
        if nomatch:
            m = re.search(r'if(.+)then\s+(o\.\d)\s+else\s+n\/a',v,re.I)
            if m: 
                nomatch = 0 
                cvldict[k] = m.group(2)+" AND "+m.group(1)
                matchcase = '5'
        if nomatch:
            #IF A.1/94 THEN N/A ELSE M
            m = re.search(r'if(.+)\s*then\s+n\/a\s*else\s+[arm]\s*',v,re.I)
            if m:
                nomatch = 0
                cvldict[k]='( NOT '+m.group(1).strip()+' )'
                matchcase = '6'
                
        if nomatch:
            #IF A.1/94 THEN N/A ELSE M
            m = re.search(r'if\s*(.+?)\s*THEN M ELSE O',v,re.I)
            if m:
                nomatch = 0
                cvldict[k]=m.group(1)
                matchcase = '61'
        if nomatch:
            #IF A.1/94 THEN N/A ELSE M
            m = re.search(r'if(.+)\s*then\s*[arm]\s+else\s+n\/a',v,re.I)
            if m:
                nomatch = 0
                cvldict[k]=m.group(1)
                matchcase = '62'
                
        if nomatch:
            #IF A.1/41 THEN R ELSE N/A
            #m = re.search(r'if(.+)then\s+[arm]\s+else\s+n\/a',v,re.I)
            m = re.search(r'if(.+)\s*[arm]\s+else\s+n\/a',v,re.I)
            
            if m:
                nomatch = 0
                cvldict[k]=m.group(1).strip()
                matchcase = '7'
        # order is very critical this should be in the bottom. 
        if nomatch:
            #IF A.1/1 AND A.3/1 AND ([56]A.4.4-1/93 OR [56]A.4.4-1/15 OR [56]A.4.4-1/14)
            m = re.search(r'if(.+)',v,re.I)
            if m:
                nomatch = 0
                cvldict[k]=m.group(1).strip()
                matchcase = '8'
        if nomatch:
            m = re.search(r'^((?:and|or|not|\(|\)|\s|\.|\/|\d|\[|\]|-|A)+)$',v,re.I)
            if m:
                nomatch = 0
                cvldict[k] = m.group(1)
                matchcase = '9'
        if nomatch:
            nomatch = 0
            if v.lower().strip()=='void':
                cvldict[k]=v
            else:
                cvldict[k]='N/A'
                matchcase='10'
                print "warning" , k,v
        v = cvldict[k]
        #print t1
        mpics= re.findall(r'((?:\[[\d\.\-]+\]\s*)?[A-G]\s*\.\s*\d[\-\.\da-zA-Z\/]+)',v)
        #mpics= re.findall(r'A\.\S+',t1)
        if mpics:
            for mpics1 in mpics:
                v=v.replace(mpics1,mpics1.replace(' ',''))
            cvldict[k] = v
        v2 = cvldict[k]
        #if (v!=v2):
    def replacecond(matchobj):
        cs = matchobj.group(1)
        if cs in cvldict:
        
            return  cvldict[cs]
        else: 
            print 'false: ',cs
            return  "False"
            
    def replacecondition(cstr):
        val = cvldict[cstr]
        mv = re.search(cstringregex,val,re.I)
        if mv:
            val = re.sub(cstringregex,replacecond,val,flags=re.IGNORECASE)
            cvldict[cstr]=val
            return True
        else:
            return False

    ckeylist = []
    for k,v in cvldict.items():
        ckeylist.append(k)
    newck=list(ckeylist)
    while True:
        ckeylist=[]
        for k in newck:
            v = cvldict[k]
            o = v.count('(')
            c = v.count(')')
            #print k,v,o,c
            if o>c:
                print "error extra (:",k,v
                cvldict[k]=v+')'
            elif o<c:
                cvldict[k]='('+v
                print "error extra ):",k,v
            rpl=replacecondition(k)
            if rpl:
                ckeylist.append(k)
            #cvldict[k]=vd
        newck = list(ckeylist)
        if(len(newck)==0):
            break
    #outfile = os.path.join(outputpath,filename.replace('_CVL','_CLP'))
    outfile = os.path.join(outputpath,trlfilespec+'_CLP.txt')
    with open(outfile , 'wb') as fout:
        wr = csv.writer(fout, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        for k,v in cvldict.items():
            n = ''
            if k in pnemonicdict:
                n = pnemonicdict[k]
            #print k,v
            mv = re.findall(cstringregex,v,re.I)
            if mv:
                mv2= list(set(mv))
                print "Error: Remaining process", k,mv2,v
                wr.writerow([k,'Void','Void'])
            else:
                wr.writerow([k,v,n,matchcase])
    #print cvldict
    
    return cvldict

def generatecombinedtva(tvafile,relidx=2,conidx= 3,Didx = None, tcidx=0):
    spec1name = tvafile.replace('_TVA.txt','')
    trlfilespec = readspecmap(spec1name)
    cvlfile=tvafile.replace('TVA','CVL')
    pnemonicdict.clear()
    #print 'COMBINE::: rel: {0} con: {1} didx : {2}  tcindex: {3}'.format(relidx,conidx,Didx,tcidx)
    #print 'COMBINE::: tvafile: {0} '.format(tvafile)
    
    cvldict = evalcvl(cvlfile,trlfilespec)
    tvafileh = open(os.path.join(inputpath,tvafile), 'rb')
    creadertva = csv.reader(tvafileh,delimiter='\t')
    oldtc  = ''
    oldrel = ''
    oldcon = ''
    oldD = ''
    oldlog = ''
    f34123 = 0 
    fdstr = 0
    fpnemoninc=0
    faddv1v2 = 0
    addv1v2list = ['13.1a','13.2a','14.2.28','14.2.29','14.2.30','14.2.31','14.2.32','14.2.33','14.2.34','14.2.35','14.2.36','14.2.37','14.20.1','14.20.11','14.20.2','14.20.3','14.20.4','14.20.5','14.20.6','14.20.7','14.20.8','14.20.9','14.20.10','21.13']
    if(tvafile.startswith('34123')):
        f34123 = 1
    if(tvafile.startswith('36521')):
        fdstr = 1 
    if tvafile.startswith('51010'):
        fpnemoninc = 1
        faddv1v2= 1
        
    if tvafile.startswith('311'):
        fpnemoninc = 1
    def makeupper(matchobj):
        cs = matchobj.group(1)
        return " "+cs.upper()+" "
    def cleanuplogicstring(oldlog):
        
        oldlog = re.sub(r'(\S)(AND)', r'\1 AND', oldlog,flags=re.IGNORECASE)
        oldlog = re.sub(r'AND(\S)', r'AND \1', oldlog,flags=re.IGNORECASE)
        oldlog = re.sub(r'(and|or|not)', makeupper, oldlog,flags=re.IGNORECASE)
        oldlog = re.sub(r'\s{2,}' ,' ', oldlog,flags=re.I)
        return oldlog
    def replacecond(matchobj):
        cs = matchobj.group(1)
        if cs in cvldict:
            return cvldict[cs]
        else:
            return 'False'
            print 'Error: condition not found',cs 
    
    flag2 = 0
    opfile1 = trlfilespec+'_TRL.txt'
    if spec1name=='34121-2':
        opfile1=trlfilespec+"_TRL_RF.txt"
        opfile2=trlfilespec+"_TRL_RRM.txt"
        flag2 = 1
    elif spec1name=='51010-2':
        opfile1=trlfilespec+"_TRL_RF.txt"
        opfile2=trlfilespec+"_TRL_CTPS.txt"
        flag2 = 1
    opfile1path = os.path.join(outputpath,opfile1)
    fline = 1
    if flag2:
        opfile1path2 = os.path.join(outputpath,opfile2)
        fout2 = open(opfile1path2,'wb')
        wr2 = csv.writer(fout2, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')

    with open(opfile1path , 'wb') as fout:
        wr = csv.writer(fout, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        con=''
        for row in creadertva:
            maxidx = max(relidx,conidx)
            if len(row)-1<maxidx:
                continue
                
            tc = row[tcidx]
            tc = tc.strip()
            if(tc.startswith('Clause') or tc.startswith('Test') or tc.strip()=='c' or tc.startswith('.Test')):
                continue
                
            rel = cleanrel(row[relidx])
            con = row[conidx]
            
            if (con.strip() == "Condition"):
                continue
            if spec1name == '37571-3-2':
                if ( row[1].lower()=="void"):
                    if con.strip() == "":
                        con = "Void"
            if spec1name == '31124-2':
                tc = tc.replace('27.22.1.1','27.22.1/1')
            if spec1name == '51010-2':
                con = con.replace('For R99: C316 For Rel-4 and onwards: C216','C216')
            con = re.sub(r'(\S)(AND)', r'\1 AND', con,flags=re.IGNORECASE)
            con = re.sub(r'AND(\S)', r'AND \1', con,flags=re.IGNORECASE)
            #if f34123:
            #    tddorfdd = row[4]
            #    if 'TDD' in tddorfdd.upper():
            #        continue
            mcon = re.findall(r'(\(?\s*note\s*\d+\s*\)?)',con,re.I)
            if mcon:
                print 'warning: This condition contains note',con,tc
                for mcon2 in mcon:
                    con = con.replace(mcon2,' ')
            if con.strip()=="" or con.strip()==r'N/A':
                continue
                
                    
            #print tc,con
            if con.lower()=='void' or con.upper()=='[FFS]':
                con = "Void"
                rel ="Void"
            log = ''
            dstr=''
            if fdstr:
                if Didx and len(row)-1>Didx:
                    dstr = row[Didx]
            else:
                dstr=''
            if tc.strip() == '':
                tc = oldtc
            if rel.strip() == '':
                rel = oldrel
            if rel == oldrel and tc == oldtc :
                con = '('+oldcon +' OR '+con +')'
            con = removeR(con)
            
            nomatch = 1
            if nomatch:
                m = re.match(r'm|a|r$',con,re.I)
                if m:
                    nomatch = 0 
                    log = 'R'
            if nomatch:
                # m = re.findall(cstringregex,con,re.I)
                m = re.search(cstringregex,con,re.I)
                if m:
                    nomatch =0
                    log = re.sub(cstringregex,replacecond,con,flags=re.IGNORECASE)
                        
            if (rel != oldrel or tc!= oldtc):
                pnemonicsval =''
                if fpnemoninc:
                    pnemonicsval = oldcon
                    mpn = re.findall(cstringregex,oldcon,re.I)
                    if mpn:
                        for mp in mpn:
                            pnm='Void'
                            if mp in pnemonicdict:
                                pnm = pnemonicdict[mp]
                            else:
                                print 'Error pnemonic missing for condition: '+ mp
                            pnemonicsval = pnemonicsval.replace(mp, pnm)
                if not fline and oldtc.strip() !='':
                    flag21 = 0
                    if flag2: 
                        if(spec1name == '34121-2'): # for 34-121
                            if oldtc.startswith('8'):
                                flag21 =1 
                                
                        elif(spec1name.startswith('51010')): # for 51010-2 and 51010-4
                            rfchapters = ('12.','14.','16.','13.','18.','21.','22.')
                            if not oldtc.startswith(rfchapters):
                                flag21 =1
                    oldlog = cleanuplogicstring(oldlog)
                    #oldlog=oldlog.replace('or','OR')
                    if flag21:
                        wr2.writerow([oldtc,oldcon,oldrel,oldD,removeR(oldlog),pnemonicsval])
                    else:
                        wr.writerow([oldtc,oldcon,oldrel,oldD,removeR(oldlog),pnemonicsval])
                    if faddv1v2 and (oldtc in addv1v2list):
                        
                        if flag21:
                            wr2.writerow([oldtc+'(v1)',oldcon,oldrel,oldD,removeR(oldlog),pnemonicsval])
                            wr2.writerow([oldtc+'(v2)',oldcon,oldrel,oldD,removeR(oldlog),pnemonicsval])
                        else:
                            wr.writerow([oldtc+'(v1)',oldcon,oldrel,oldD,removeR(oldlog),pnemonicsval])
                            wr.writerow([oldtc+'(v2)',oldcon,oldrel,oldD,removeR(oldlog),pnemonicsval])
            fline = 0 
            oldtc = tc
            oldrel = rel
            oldD = dstr
            oldcon = con
            oldlog = log
        if con !='' and (rel != oldrel or tc!= oldtc):
            pnemonicsval = ''
            if fpnemoninc:
                pnemonicsval = oldcon
                mpn = re.findall(cstringregex,oldcon,re.I)
                if mpn:
                    for mp in mpn:
                        pnm='Void'
                        if mp in pnemonicdict:
                            pnm = pnemonicdict[mp]
                        else:
                            print 'Error pnemonic missing for condition: '+ mp
                        pnemonicsval = pnemonicsval.replace(mp, pnm)
            flag21 = 0
            if flag2: 
                if(spec1name == '34121-2'): # for 34-121
                    if oldtc.startswith('8'):
                        flag21 =1 
                        
                elif(spec1name.startswith('51010')): # for 51010-2 and 51010-4
                    rfchapters = ('12.','14.','16.','13.','18.','21.','22.')
                    if not oldtc.startswith(rfchapters):
                        flag21 =1
            oldlog = cleanuplogicstring(oldlog)
            #oldlog=oldlog.replace('or','OR')
            if flag21:
                wr2.writerow([oldtc,oldcon,oldrel,oldD,removeR(oldlog),pnemonicsval])
            else:
                wr.writerow([oldtc,oldcon,oldrel,oldD,removeR(oldlog),pnemonicsval])
    if flag2:
        fout2.close()
def removeR(log):
    x = 0 
    while(True):
        mror = re.search(r'(\s*R\s*OR\s*(?:R|A\.[\d\/\-\.]+))',log,re.I)
        mand = re.search(r'(\s*R\s*AND\s*(R|A\.[\d\/\-\.]+))',log,re.I)
        if mror:
            x = 1
            #print '#0',log
            log = log.replace(mror.group(1),' R ')
            log = re.sub(r'\(\s*R\s*\)','R',log,flags=re.IGNORECASE)
        elif mand:
            x = 1
            #print '#0',log
            log = log.replace(mand.group(1),' '+mand.group(2)+' ')
        else:
            #if x : print '#r',log
            break;
    return log
def cleanrel(rel):
    finalrel = re.sub(r'\s*\(Note\d\)','',rel)
    # phase2 = r95            # phase1 = r92
    #finalrel = re.sub(r'phase\s*-?\s*1','R92',finalrel,flags=re.I)
    #finalrel = re.sub(r'phase\s*-?\s*2','R95',finalrel,flags=re.I)
    #change Rel-1999 to R99
    if('199' in finalrel):
        #print finalrel,'before'
        finalrel = re.sub(r'rel\w*\s*-?\s*199','R9',finalrel,flags=re.I)
        #print finalrel,'after'            
    #replace release with Rel
    finalrel = re.sub(r'Release\s*-?','Rel-',finalrel,flags=re.I)
    #remove and amr loops
    finalrel = re.sub(r'\s*AND AMR Loops','',finalrel,flags=re.I)
    #correct space from rel 2 
    finalrel = re.sub(r'rel\s*-?\s*','Rel-',finalrel,flags=re.I)
    # replace with to . up to and including
    finalrel = re.sub(r'\s*up to and including\s*',' to ',finalrel,flags=re.I)
    #R12 to Rel-12
    finalrel = re.sub(r'r12','Rel-12',finalrel,flags=re.I)
    return finalrel
if __name__=="__main__": 
    tvafilename = '34123-2_TVA.txt'
    cvlfilename = '34123-2_CVL.txt'
    
    #generatecombinedtva(cvlfilename,tvafilename)
    print readspecmap('31121-2')

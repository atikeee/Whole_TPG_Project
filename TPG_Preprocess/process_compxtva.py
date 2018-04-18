import os,re,csv

#make sure the 
def getlist(str1,str2):
    m1 = re.search('(\d+)\.(\d+)',str1)
    m2 = re.search('(\d+)\.(\d+)',str2)
    if(m1):
        start = m1.group(2)
    if(m2):
        end = m2.group(2)
    l =[]
    for x in range(int(start), int(end)):
        l.append(m1.group(1)+"."+str(x))
    return l
def processtva_complx(infile = 'Input_Files\\31124-2_raw_TVA.txt',outfile = r'Input_Files\\31124-2_TVA.txt',itcremaining=3,irel=2,irelcon=17,icon=14,ides=1):
    xx = 0
    firstparttc= ''
    firstparttctemp =''
    with open(outfile , 'wb') as fout:
        wr = csv.writer(fout, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        with  open(infile) as f:
            for ln in f:
                xx+=1
                l = ln.split('\t')
                if len(l)< max(itcremaining,irel,irelcon,icon,ides)+1:
                    print 'continue',l
                    continue
                if(l[0]!=''):
                    m = re.search(r'((?:\d+\.)+\d+)',l[ides])
                    if(m):
                        firstparttctemp = m.group()
                        ## exception for 27.22.7.1
                        
                firstparttc = firstparttctemp
                if(firstparttctemp == '27.22.7'):
                    #mexc1 = re.search(r'(27\.22\.7\.\d+)',l[1])
                    #mexc1 = re.search(r'(27\.22\.7\.[\d\.]+)',l[1])
                    mexc1 = re.search(r'(27\.22\.7\.\d+)',l[1])
                    if(mexc1):
                        firstparttc = mexc1.group(1)
                        #print firstparttc
                # no itcremaining column has the remaining of test case. 
                x = l[itcremaining].split(',')
                lsorted = [l[irelcon],l[irel],l[icon],l[ides]]
                allline =[]
                for stc in x:
                    m3 = re.search(r'(\d+\.\d+)\s*(?:-|to)\s*(\d+\.\d+)',stc)
                    m4 = re.search(r'\d+\.\d+\s*[A-G]',stc)
                    m5 = re.search(r'(\d+\.\d+)\s*(\d+\.\d+)',stc)
                    if(stc):
                        if(m3):
                            st = m3.group(1)
                            en = m3.group(2)
                            allstc = getlist(st,en)
                            for s in allstc:
                                s2=s.replace('.','/')
                                tcno= firstparttc+'.'+s2.strip()
                                wr.writerow( [tcno]+lsorted)
                        elif(m4):
                            stc2=stc.replace('.','/').replace(' ' ,'')
                            tcno= firstparttc+'.'+stc2
                            wr.writerow( [tcno]+lsorted)
                        elif(m5):
                            allstc = stc.split(' ')
                            for s in allstc:
                                s2=s.replace('.','/')
                                tcno= firstparttc+'.'+s2.strip()
                                wr.writerow( [tcno]+lsorted)
                        else:
                            stc2=stc.replace('.','/').strip()
                            tcno= firstparttc+'.'+stc2
                            wr.writerow( [tcno]+lsorted)
                    else:
                        tcno= firstparttc
                        wr.writerow( [tcno]+lsorted)
def processtvafillblanks (infile = 'Input_Files\\34123-2_raw_TVA.txt',outfile = r'Input_Files\\34123-2_TVA.txt',sz = 7):
    with open(outfile , 'wb') as fout:
        wr = csv.writer(fout, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        with  open(infile) as f:
            firstline = 0 
            for line in f:
                    
                ln = line.replace('\n','').split('\t')
                newln = list(ln)
                #print len(newln)
                if(len(newln)==sz):
                    colno =  0 
                    for l in ln:
                        if(firstline!=0):
                            if(l.strip()==""):
                                newln[colno]=lntmp[colno]
                        colno+=1
                    if 'void' in line.lower():
                        wr.writerow(ln)
                    else:
                        wr.writerow(newln)
                else:
                    print 'size is different',sz,len(newln), line
                lntmp = list(newln)
                firstline +=1
                
def process34123 (infile = 'Input_Files\\34123-2_raw_TVA.txt',outfile = r'Input_Files\\34123-2_TVA.txt',sz = 7):
    with open(outfile , 'wb') as fout:
        allline = []
        wr = csv.writer(fout, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        with  open(infile) as f:
            firstline = 0 
            for line in f:
                    
                ln = line.replace('\n','').split('\t')
                newln = list(ln)
                #print len(newln)
                if(len(newln)==sz):
                    if('TDD' in ln[4])and ('FDD' not in ln[4]):
                        continue
                    if(ln[0]==''):
                        colno =  0 
                        for l in ln:
                            if(firstline!=0):
                                if(l.strip()==""):
                                    newln[colno]=lntmp[colno]
                            colno+=1
                    if 'void' in line.lower():
                        #wr.writerow(ln)
                        allline.append(ln)
                    else:
                        #wr.writerow(newln)
                        allline.append(newln)
                        
                else:
                    print 'size is different',sz,len(newln), line
                lntmp = list(newln)
                firstline +=1
        lasttc = ''
        alllinenew = []
        # add new tc Proc 1 /2 / 4
        proclist = ['9.4.2.2','9.4.2.4','12.2.1.4','12.2.1.5a','12.2.1.6','12.3.2.8','12.4.1.4c','12.4.1.4d','12.4.2.5a','12.4.2.6']
        add123list = ['14.2.51a','14.2.51b','14.4.2','14.4.2a']
        for line in allline:
            tc = line[0]
            if(tc!=lasttc):
                newline = list(line)
                alllinenew.append(newline)
                #print "new"
                if tc in add123list:
                    for i in range(1,4):
                        newline2 = list(line)
                        newline2[0] =tc+ '.'+str(i)
                        alllinenew.append(newline2)
                if tc in proclist:
                    newline2 = list(line)
                    newline2[0] =tc+ ' Proc 1'
                    alllinenew.append(newline2)
                    newline2 = list(line)
                    newline2[0] =tc+ ' Proc 2'
                    alllinenew.append(newline2)
                    newline2 = list(line)
                    newline2[0] =tc+ ' Proc 4'
                    alllinenew.append(newline2)
                if tc.startswith('9.4.2.4.'):
                    newline2 = list(line)
                    newline2[0] =tc.replace('9.4.2.4.','9.4.2.4 Proc ')
                    alllinenew.append(newline2)
                # all 17.2 made recommended. Originally it is needed to add as per 37.571
                if(tc.startswith('17.2') and line[1] != 'Void'):
                    newline[3] ='R'
            else:
                if line[3]:
                    newline[3] += ' OR ' +line[3]
                    newline[4] += '|' +line[4]
            
            lasttc = newline[0]
        for line in alllinenew:
            wr.writerow(line)

def processexception510104(infile = 'Input_Files\\51010-4_TVA.txt'):
    allnewline = []
    def addline(ln):
        newln = list(ln)
        newln.append('Extra')
        allnewline.append(newln)
        
    with  open(infile) as f:
        for line in f:
            ln = line.replace('\n','').split('\t')
            ln.append('')
            allnewline.append(list(ln))
            if ln[0] == '27.22.1.1':
                ln[0] = '27.22.1/1'
                addline(ln)
            if ln[0].startswith('27.22.4.21'):
                ln[0]=re.sub(r'(\d+\.\d+\.\d+\.\d+)\.\d+',r"\1",ln[0])
                addline(ln)
            #if ln[0].startswith('27.22.4.22'):
            #    ln[0]=re.sub(r'(\d+\.\d+\.\d+\.\d+\.\d+\/\d)',r"\1A",ln[0])
            #    addline(ln)
            #    ln[0]=ln[0].replace("A","B")
            #    addline(ln)
            listaddAB = ['27.22.4.1.5/1','27.22.4.1.5/2','27.22.4.1.5/3','27.22.4.2.6/1','27.22.4.2.6/2','27.22.4.2.6/3','27.22.4.2.6/4','27.22.4.3.6/1','27.22.4.3.6/2','27.22.4.3.6/3','27.22.4.3.6/4','27.22.4.8.4/1','27.22.4.8.4/2','27.22.4.9.1/6','27.22.4.9.5/1','27.22.4.9.5/2','27.22.4.10.3/1','27.22.4.10.3/2','27.22.4.11.1/1','27.22.4.11.1/4','27.22.4.11.1/6','27.22.4.11.2/1','27.22.4.11.2/2','27.22.4.11.2/3','27.22.4.12.2/1','27.22.4.12.2/3','27.22.4.13.1/11','27.22.4.13.3/1','27.22.4.13.3/2','27.22.4.13.3/3','27.22.4.13.3/4']
            if (ln[0] in listaddAB) or ln[0].startswith('27.22.4.22') or ln[0].startswith('27.22.4.24') or ln[0].startswith('27.22.4.27'):
                ln[0]+='A'
                addline(ln)
                ln[0]=ln[0].replace("A","B")
                addline(ln)
    with open(infile , 'wb') as fout:
        wr = csv.writer(fout, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        for line in allnewline:
            wr.writerow(line)
def processexception510102(infile = 'Input_Files\\51010-2_TVA.txt'):
    allnewline = []
    def addline(ln):
        newln = list(ln)
        newln.append('Extra')
        allnewline.append(newln)
        
    with  open(infile) as f:
        for line in f:
            ln = line.replace('\n','').split('\t')
            ln.append('')
            allnewline.append(list(ln))
            #removing ) from the test case infor
            if ln[0].startswith('27.17.1.2-3.1'):
                ln[0] = '27.17.1.2-3.1'
                addline(ln)
            # vamos test case 26.21
            if ln[0].startswith('26.21'):
                m = re.search(r'(\d+)-(\d+)',ln[0])
                if m:
                    ln[0]=re.sub(r'(\d+)-(\d+)',r"\1(V\2)",ln[0])
                    addline(ln)
            
    with open(infile , 'wb') as fout:
        wr = csv.writer(fout, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        for line in allnewline:
            wr.writerow(line)
def processexception31124(infile = 'Input_Files\\31124-2_TVA.txt'):
    allnewline = []
    
    def addline(ln):
        newln = list(ln)
        newln.append('Extra')
        allnewline.append(newln)
        
    with  open(infile) as f:
        for line in f:
            ln = line.replace('\n','').split('\t')
            ln.append('')
            allnewline.append(list(ln))
            # vamos test case 26.21
            addablist = ['27.22.4.27.2/7','27.22.6.1/3','27.22.6.1/5','27.22.6.1/7']
            if ln[0].startswith('27.22.4.11.') or ln[0].startswith('27.22.4.13.') or (ln[0] in addablist):
                ln[0]=ln[0]+'a'
                addline(ln)
                ln[0]=ln[0].replace('a','b')
                addline(ln)
                
            if ln[0].startswith('27.22.4.21') or ln[0].startswith('27.22.4.31') or ln[0].startswith('27.22.7.11.1/1') or ln[0].startswith('27.22.5.1.') or ln[0].startswith('27.22.5.4.'):
                ln[0]=re.sub(r'(\d+\.\d+\.\d+\.\d+)\.\d+',r"\1",ln[0])
                addline(ln)
                # special case 
            if ln[0] == '27.22.4.6.1/1':
                ln[0] = '27.22.4.6'
                addline(ln)
    with open(infile , 'wb') as fout:
        wr = csv.writer(fout, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        for line in allnewline:
            wr.writerow(line)

def processexception(infile = 'Input_Files\\_TVA.txt'):
    allnewline = []
    with  open(infile) as f:
        for line in f:
            ln = line.replace('\n','').split('\t')
            ln.append('')
            newln = list(ln)
            allnewline.append(newln)
            listaddAB = ['27.22.4.1.5/1','27.22.4.1.5/2','27.22.4.1.5/3','27.22.4.2.6/1','27.22.4.2.6/2','27.22.4.2.6/3','27.22.4.2.6/4','27.22.4.3.6/1','27.22.4.3.6/2','27.22.4.3.6/3','27.22.4.3.6/4','27.22.4.8.4/1','27.22.4.8.4/2','27.22.4.9.1/6','27.22.4.9.5/1','27.22.4.9.5/2','27.22.4.10.3/1','27.22.4.10.3/2','27.22.4.11.1/1','27.22.4.11.1/4','27.22.4.11.1/6','27.22.4.11.2/1','27.22.4.11.2/2','27.22.4.11.2/3','27.22.4.12.2/1','27.22.4.12.2/3','27.22.4.13.1/11','27.22.4.13.3/1','27.22.4.13.3/2','27.22.4.13.3/3','27.22.4.13.3/4']
            if ln[0] in listaddAB:
                newln = list(ln)
                newln[-1]= 'extra'
                newln[0]+='A'
                allnewline.append(newln)
                newln = list(ln)
                newln[0]+='B'
                newln[-1]= 'extra'
                allnewline.append(newln)
    with open(infile , 'wb') as fout:
        wr = csv.writer(fout, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        for line in allnewline:
            wr.writerow(line)
#def copytva(infile1 , infile2):
    
if __name__=="__main__":
    processexception510104()
    #processtva_complx('Input_Files\\51010-4_raw_TVA.txt',r'Input_Files\\51010-4_TVA.txt',3,2,10,8,1)
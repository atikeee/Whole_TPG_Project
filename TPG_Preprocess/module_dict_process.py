import re, os, csv, logging
from tableparse_Restructure import *
from constants import *
#from test_ifthenelse import *
# from 

##---------logger level----------##
##  Level	Numeric value
##  CRITICAL	50
##  ERROR	40
##  WARNING	30
##  INFO	20
##  DEBUG	10
##  NOTSET	0

##--------logger level----------##


class module_dict_process():
    def __init__(self):
        self.dictde = {}
        self.logger = None
        self.spec1name = ""
        self.spec1type = ""
        self.specmap={}
        self.spec1_mapping()
    def spec1_mapping(self, inpind = 0, outpind = 1):
        logger = self.logger
        cfile = open(configfile, 'rt')
        creader = csv.reader(cfile,delimiter=',')
        for crow in creader:
            if(not crow[inpind].strip() in self.specmap):
                self.specmap[crow[inpind].strip()]=crow[outpind]
                        
        

    def prepareCVL(self, dictobj, k="", v=""):
        opfile = self.specmap[self.spec1name]+"_CVL.csv"
        creplaceval=''
        if(self.spec1name=='51010-2'):
            creplaceval="10"
        elif(self.spec1name=='51010-4'):
            creplaceval="11"
        # print 'creplaceval',creplaceval
        orgfile = open(os.path.join(outputpath,opfile ),'ab')
        wr = csv.writer(orgfile, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        #testrow = []
        for k, v in dictobj.items():
            # Exception handling code: 
            k0=k
            v0=v
            v0 = re.sub(r'AND AND',"AND",v0,flags=re.IGNORECASE)
            # Exceptin end. 
            # print "info: ",k,v
            if(creplaceval!=''):
                k0=re.sub('^C',"C%"+creplaceval+"%",k)
                v0 = re.sub(r'\bC',"C%"+creplaceval+"%",v)
                
            #print k0,v0
            
            v0=re.sub(r"\s*(AND|OR|NOT)\s*", r" \1 ", v0,flags=re.IGNORECASE)
            v0=re.sub(r"A\s*(\d\/)", r"A.\1", v0,flags=re.IGNORECASE)
            v0=v0.replace('  ',' ').replace('!', ' NOT ')
            if('( ' in v0):
                v0=v0.replace('( ','(')
            #v0=v0.replace('  ',' ').replace('( ','(').replace('!', ' NOT ')
            if( not 'void' in v0.lower()):
                if(not v0.strip().startswith('IF')):
                    v0 ='IF '+v0
                if ('then n/a else m' in v0.lower()):
                    v0 = v0.replace('THEN N/A ELSE M','THEN M ELSE N/A').replace('IF','IF NOT')
                
                elif ('then m else o' in v0.lower()):
                    v0=v0.replace('O','N/A')
                elif(not re.search(r'THEN .+ ELSE .+',v0)):
                    v0=v0+' THEN M ELSE N/A'
            wr.writerow([k0,v0])
        orgfile.close()
        
    def lastvalid(self, v1):
        ind = 1
        vv1 = ""
        # print v1
        while ind:
            if v1[len(v1)-ind] != "":
                vv1 = v1[len(v1)-ind]
                ind = 0
            else:       
                ind += 1
        return vv1
        
    #this function will return the string between 'IF' and 'THEN R ELSE NA'

   
    def combinetwodict(self,dict1,dict3,dict4):
        opfile1 = self.specmap[self.spec1name]+"_TVA.csv"
        opfile2=''
        flag2 = 0
        creplaceval = ''
        #print "Combining release and Recommended TC for Spec: ",self.spec1name
        if self.spec1name=='34121-2':
            opfile1=self.specmap[self.spec1name]+"_TVA_RF.csv"
            opfile2=self.specmap[self.spec1name]+"_TVA_RRM.csv"
            flag2 = 1
        elif self.spec1name == "51010-4":
            opfile1=self.specmap[self.spec1name]+"_TVA_RF.csv"
            opfile2=self.specmap[self.spec1name]+"_TVA_CTPS.csv"
            creplaceval="11"
            flag2 = 1
        elif self.spec1name=='51010-2':
            opfile1=self.specmap[self.spec1name]+"_TVA_RF.csv"
            opfile2=self.specmap[self.spec1name]+"_TVA_CTPS.csv"
            creplaceval="10"
            flag2 = 1
        
        orgfile1 = open(os.path.join(outputpath,opfile1), 'ab')
        wr1 = csv.writer(orgfile1, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
        if(flag2):
            orgfile2 = open(os.path.join(outputpath,opfile2), 'ab')
            wr2 = csv.writer(orgfile2, delimiter = '\t',quoting=csv.QUOTE_NONE, escapechar='\\')
            print 'Second File',opfile2
        
        for k, v in dict1.items():
            v1='NF'
            if(k in dict3):
                v1 = dict3[k]
            # logic to write in different file here. 
            flag21 = 0
            if flag2: 
                if(self.spec1name == '34121-2'): # for 34-121
                    if k.startswith('8'):
                        flag21 =1 
                        
                elif(self.spec1name.startswith('51010')): # for 51010-2 and 51010-4
                    rfchapters = ('12.','14.','16.','13.','18.','21.','22.')
                    if not k.startswith(rfchapters):
                        flag21 =1
            v0=v            
            if(creplaceval!=''):
                #pass
                v0=re.sub(r'\bC',"C%"+creplaceval+"%",v)
            flag3 = 1
            if(v0.strip()==''):flag3=0
            if(k=='dummy'):flag3=0
            #k=k.replace('(','').replace(')','')
            re.sub(r'\b(C\S+)\s*([a-k])','\1\2',v0)
            if flag3:
                if flag21 and flag2:
                    wr2.writerow([k,v0,v1,''])
                else:
                    # This is for one spec only. 
                    if self.spec1name=='36521-2-1':
                        v2=''
                        if(k.startswith('6') or k.startswith('7')):
                            v2 = dict4[k].replace(' ','')
                        wr1.writerow([k,v0,v1,v2])
                    else:
                        wr1.writerow([k,v0,v1,''])
         #   testrow = []
         
        orgfile1.close()
        if flag2: orgfile2.close()
def cleanreleaseinformation(dict3):
# Logic for picking release should be implemented here. 
    for k,allrel in dict3.items(): 
        
        finalrel2=''
        for rel in allrel:
            finalrel=''
            if(rel.strip()==''):
                continue
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
            if( re.search(r'(rel-\d+|r\d+)',finalrel)):
                finalrel2 = finalrel
        dict3[k]=finalrel2
        
        #
def  cleandict2(dict2):
    # its for condition. no need to pick first item. 
    pass
    #for k,v in dict2.iteritems(): 
    #    dict2[k]=v[0]
            
def  cleandict1(dict1):        
    for k,v in dict1.items(): 
        #dict1[k]=v[0]
        vals = dict1[k]
        #this will pick the last value from the list 
        dict1[k]=vals[0]
        for val in vals:
            if(val!=''):
                dict1[k] = val
def cleanrel(rel):
    finalrel = re.sub(r'\s*\(Note\s*\d\)','',rel)
    # phase2 = r95            # phase1 = r92
    finalrel = re.sub(r'phase\s*-?\s*1','R92',finalrel,flags=re.I)
    finalrel = re.sub(r'phase\s*-?\s*2','R95',finalrel,flags=re.I)
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
def  cleandict13(dict1,dict3,dict4):  
    for k,v in dict1.items():
        #dict1[k]=v[0]
        vals = dict1[k]
        #this will pick the last value from the list 
        dict1[k]=vals[0]
        if(dict1[k].lower()=='void'):
            if dict3:
                dict3[k]=''
            if dict4:
                dict4[k]=''
            continue
        idx =0
        for val in vals:
            if(val!=''):
                dict1[k] = val
                idx+=1        
        if (k in dict3):
            allrel = dict3[k]
            rel = allrel[idx-1]
            finalrel = cleanrel(rel)
            if finalrel =="":
                for r in allrel:
                    fr = cleanrel(r)
                    if(fr):
                        finalrel=fr
            dict3[k]=finalrel
        if dict4:
            if (k in dict4):
                # D and E val
                
                deval = dict4[k][idx-1]
                dict4[k]=''
                m =re.match('^([DE]\d+)',deval) 
                if(m):
                    dict4[k]=m.group(1)
                    
            
def cleanoutputdir():
    for the_file in os.listdir(outputpath):
        file_path = os.path.join(outputpath, the_file)
        try:
            if os.path.isfile(file_path):
                os.unlink(file_path)
            #elif os.path.isdir(file_path): shutil.rmtree(file_path)
        except Exception as e:
            print(e)
if __name__ == '__main__':
    rootpath = r'C:\Dropbox\Scripts\PY\TPG_PREPROCESS\Input_Files'
    
 
    

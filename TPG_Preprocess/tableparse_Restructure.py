import re, collections, pickle,collections
#from stru import *
#class readtable_cond_LTE():
#    def __init__(self,txtname):
#        self.dict_cond = collections.OrderedDict()
#        self.C_cond_map(txtname)
#        # self.C_logic(str_main)
#    
#    def C_cond_map(self,txtname):
#        op_file = open(txtname)
#        lines = op_file.readlines()
#        # print lines
#        # 
#        for line in lines:
#            m = re.search(r'(C\w+)\s*if\s*(.+) (\s*then)',line,re.I)
#            if m:
#                cond_val = m.group(1)
#                cond_res_raw = m.group(2)
#                cond_res = self.C_logic(cond_res_raw)
#                self.dict_cond[cond_val] = cond_res
#
#    def C_logic(self,str_main):
#        xx1 = re.compile('AND')
#        xx2 = re.compile('OR')
#        xx3 = re.compile('NOT')
#        z_temp1 = xx1.sub('and',str_main)
#        z_temp2 = xx2.sub('or',z_temp1)
#        z_temp3 = xx3.sub('not',z_temp2)
#        
#        stro = z_temp3
#        pattern = r'([A]\.\d+\.\d+(?:\.\d+)?(?:\.\d+)?-\w+\/\w+)'
#        pat_comp = re.compile(pattern)
#        pat_list = re.findall(pat_comp,str_main)
#        
#        # for [8]:
#        pattern_ref8 = r'\[8\]\s?(A\.\w+(?:\.\d+)?(?:\.\d+)?\/\w+)'
#        pat_comp_ref8 = re.compile(pattern_ref8)
#        pat_list_ref8 = re.findall(pat_comp_ref8,str_main)
#        
#        # for [45]:
#        pattern_ref45 = r'\[45\]\s?(A\.\w+(?:\.\d+)?(?:\.\d+)?\/\w+)'
#        pat_comp_ref45 = re.compile(pattern_ref45)
#        pat_list_ref45 = re.findall(pat_comp_ref45,str_main)
#        
#        PICS_dictionary_36523_2 = readtable_PICS_obj.dict_PICS_36523_2
#        PICS_dictionary_34123_2 = readtable_PICS_obj.dict_PICS_34123_2
#        PICS_dictionary_34229_2 = readtable_PICS_obj.dict_PICS_34229_2
#        
#        pat_list_full = list(set(pat_list) | set(pat_list_ref8) | set(pat_list_ref45))
#        
#        for i in pat_list_full:
#            if i in PICS_dictionary_36523_2.keys():
#                xnew = str(PICS_dictionary_36523_2[i])
#            elif i in PICS_dictionary_34123_2.keys():
#                xnew = str(PICS_dictionary_34123_2[i])    
#            elif i in PICS_dictionary_34229_2.keys():
#                xnew = str(PICS_dictionary_34229_2[i]) 
#            else:
#                xnew = str(0)
#            
#            stro = stro.replace(i,xnew)
#
#        stro_mod_elim8 = stro.replace('[8]','')
#        stro_mod_elim45 = stro_mod_elim8.replace('[45]','')
#        
#        result = eval(stro_mod_elim45)
#        return int(result)
#
class parsepixittable():
    def __init__(self,fn):
        # self.fn = fn
        self.lines = []
        self.lastkey = ""
        self.openfile(fn)
    def setparam(self,delim,tabcount,regexstr,regexstrcolno,algorithm):
        self.delim = delim
        self.tabcount = tabcount
        self.regexstr = regexstr
        self.regexstrcolno = regexstrcolno
        
        # 1 means null key will be filled with last valid key.  and returns list for key
        # 2 means only matched key will be picked. 
        # 
        self.algorithm = algorithm 
        self.readline()
        
    def openfile(self,fn):
        self.fh = open(fn,'r')
    def closefile(self,fn):
        self.fh.close()
    def appendrow(self,curline):
        # check if the line qulify to add in the list. 
        # check from colno with regex if this is a valid entry. 
        # check algorithm and find whether key need to be continued if the key is null. 
        #if curline[col
        m0 = re.search(self.regexstr,curline[self.regexstrcolno],re.I)
        if m0: 
            self.lines.append(curline)
            # print self.lines, "selfline"
            # print len(curline),curline
            # if len(curline)<9:
                # print curline
        else:
            if self.algorithm ==1:
                if curline[self.regexstrcolno]:
                    curline[self.regexstrcolno]='dummy'
                # if '58a.1.1' in curline: print curline, " :curline" 
                # print 'x', len(curline), curline
                self.lines.append(curline)
    def readline(self):
        for line in self.fh:
            cols = line.split(self.delim)
            self.appendrow(cols)
    #def readline(self):
    #    ln = 0
    #    curline = []
    #    sz =0
    #    testtcvalue = r'5.2'
    #    lastcol0=''
    #    for line in self.fh:
    #        # print stru(line) + " :line"
    #        ln +=1
    #        cols = line.split(self.delim)
    #        cursz = len(cols)-1
    #        # print 'before: ', curline
    #        #if cursz==0:
    #        #    print "cursz: ",  cursz, " -> self.tabcount = ", self.tabcount
    #        cols[-1] = cols[-1].strip()+" "
    #        # print cols[0]
    #        #print "tabcompare",cursz, self.tabcount,ln,line
    #        print 'tabcount' , self.tabcount ,ln
    #        
    #        if cursz<self.tabcount:
    #            if cursz==0 and len(self.lines)>0:
    #                # if cols[0] == testtcvalue: print cols[-1], ":cols"
    #                # print "### before lines### ",len(self.lines)
    #                # for ll in self.lines:
    #                    # print ll
    #                # print "### lines### "
    #                
    #                lastrow = self.lines[-1]
    #                newlastrow = lastrow[:-1]
    #                lastcollastrow = lastrow[-1].strip()+ cols[0].strip()
    #                # print "cur cols" , cols,"last row" , lastrow, "last element: ",lastcollastrow, 
    #                newlastrow = lastrow[:-1]+[lastcollastrow]
    #                # print "\n",newlastrow, "new" 
    #                self.lines[-1] = newlastrow
    #                #self.lines[-1] = lastcollastrow
    #                # print "### after lines### ",len(self.lines)
    #                # for ll in self.lines:
    #                    # print ll
    #                # print "### lines### "
    #            else:    
    #                # if cols[0] == testtcvalue: print cols[-1], ":cols"
    #                if curline:
    #                    #print '______',len(curline),"___",curline
    #                    curline[-1] = curline[-1].strip()+ " " +cols[0]
    #                    curline.extend(cols[1:])
    #                else:
    #                    curline.extend(cols)
    #                # print 'after <8  ',curline
    #                sz += cursz
    #                if sz>=self.tabcount:
    #                    # print 'lineno1: ',ln, len(curline),curline
    #                    self.appendrow(curline)
    #                    # if cols[0] == testtcvalue:print curline, " :curline 1"
    #                    curline =[]
    #                    sz = 0 
    #        elif(cursz == self.tabcount):
    #            #print 'curline',cols
    #            if(cols[0]=='' and lastcol0!=''):
    #                cols[0] = lastcol0
    #            self.appendrow(cols)
    #            curline = []
    #            sz = 0 
    #        else:
    #            
    #            curline.extend(cols)
    #            #print 'ISSUE lineno: ',ln, len(curline),curline
    #            # print '\n'
    #            self.appendrow(curline)
    #            if cols[0] in self.lines: print self.lines, ":cols"
    #            # print 'after >=8 ',curline                                      
    #            # print curline, " :curline 2"
    #            curline = []
    #            sz = 0 
    #        lastcol0 = cols[0]
    #        # print "Lines: ", line
            # print "Columns: ", cols
    def makedictionary(self,keyidx,validx,smallkey= False):
        #ret_dic = {}
        #print keyidx,validx
        ret_dic = collections.OrderedDict()
        self.lastkey = 'dummy'
        for cols in self.lines:
            if(keyidx>=len(cols)) or (validx>=len(cols)):
                continue
                
            key = cols[keyidx].strip()
            if smallkey:
                key = key.lower()
            val = cols[validx].strip()
            if (val==""):
                if(cols[1]=='Void'):
                    val = "Void"
            if self.algorithm ==1  :
                if not key:
                    key =self.lastkey
                else:
                    self.lastkey=key
            if self.algorithm==1:
                if key in ret_dic.keys():
                    ret_dic[key].append(val)
                else:
                    ret_dic[key] = [val]
            elif self.algorithm ==2:
                ret_dic[key]=val
            #cond =''
            #rel = ''
            ##m0 = re.search('^\d+\.\d+',cols[0],re.I)
            #if len(cols)>3:
            #    m3 = re.search('(^C\d+)|(^R)',cols[3],re.I)
            #    m3_rel = re.search(r'^Rel-\d+',cols[2],re.I)
            #    if m3:
            #        cond = cols[3]
            #    if m3_rel:
            #        rel = cols[2]
            #if not clause.strip():
            #    if lastclause in self.dic_cond_app.keys() and cond:
            #        self.dic_cond_app[lastclause].append(cond)
            #        self.dic_release[lastclause].append(rel)
            #    # if lastclause in self.dic_release.keys():
            #if m0:
            #    self.dic_cond_app[clause] = [cond]
            #    lastclause = clause
        return ret_dic

if __name__ == '__main__':
    file71 = 'Table_102230-2_TC_APP_CTPS.txt'
    file72 = 'Table_102230-2_COND_CTPS.txt'
    file='Table_34123-1_COND_CTPS.txt'
    # file1 = 'Table_36523-1_COND_CTPS.txt'
    # file2 = 'Table_36523-1_TC_APP_CTPS.txt'
    file1 = file71
    file2 = file72
    # Reading PICS files:
    # readtable_PICS_obj = readtable_PICS(PICS_file)
    
    # FOR LTE:
    ## --------------------------------------------------------------
    # TC vs. Condition (R or CXX) + TC vs. Release: 
    obj1 = parsepixittable(file1)
    obj2 = parsepixittable(file2)
    obj2.setparam('\t',3,'^\d+\.\d+',0,1)
    obj1.setparam('\t',1,'^c\d+',0,2)
    x =obj1.makedictionary(0,1)
    y =obj2.makedictionary(0,3)
    print x
    print "\n\n"
    print y
    #for k,v in x.iteritems():
    #    print k,v
    #    
    #for k,v in y.iteritems():
    #    print k,v
    
    
    
       
    #for ind,TC_val in enumerate(readtable_tc_app_LTE_obj.dic_cond_app.keys()):
    #    print ind, TC_val, readtable_tc_app_LTE_obj.dic_cond_app.values()[ind]
    
    
    
    
    


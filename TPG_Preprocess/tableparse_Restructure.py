import re, collections, pickle,collections

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

        return ret_dic

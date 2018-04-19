import os
#import ConfigParser,re
from ConfigParser import ConfigParser
import logging
class _ConfigParser(ConfigParser):
# sections() return allconfiguration sections
#https://github.com/enthought/Python-2.7.3/blob/master/Lib/ConfigParser.py

    def __init__(self,defaults=None):
         ConfigParser.__init__(self,defaults)
    def getlist(self,section,var):
        strvar= self.get(section,var)
        listvar=[]
        listvarlines=strvar.split("\n")
        for listvarline in listvarlines:
            t_listvar=listvarline.split("\t")
            t_listvar=map(str.strip, t_listvar)
            listvar.extend(t_listvar)
        return listvar
        
    def geteval(self,section,var):
        strvar= self.get(section,var)
        return  eval (strvar)
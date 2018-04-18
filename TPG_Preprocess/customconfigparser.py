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
if __name__=='__main__':
    # add filemode="w" to overwrite
    #logging.basicConfig(filename="sample.log", level=logging.INFO)
    #
    ##looger = logging.getlogger()
    #logging.debug("This is a debug message")
    #logging.info ("Informational message")
    #logging.error("An error has happened!")
    
    logger = logging.getLogger()
    # handler = logging.StreamHandler()
    handler = logging.FileHandler('s.log','a')
    formatter = logging.Formatter(
            # '%(asctime)s %(name)-12s %(levelname)-8s %(message)s')
            '%(filename)-20s%(lineno)-5s %(levelname)-8s %(message)s')
    handler.setFormatter(formatter)
    logger.addHandler(handler)
    logger.setLevel(58)
    logger.debug("This is a debug message")
    
    
    
    logger.info ("Informational message")
    logger.error("An error has happened!")
    logger.debug('often makes a very good meal of %s', 'visiting tourists')  
    logger.log(43,"hello this is log lvl 43")
    logger.log(53,"hello this is log lvl 53")
    logger.log(63,"hello this is log lvl 63")
    logger.log(73,"hello this is log lvl 73")
    print os.getenv('HOME')
    #config = _ConfigParser()
    #config.read('config.ini')
    #print config.sections()
    #ce= config.geteval('bitbucket','e')
    #print ce['a']
    #ll= config.get('bitbucket','f')
    #ll2 = eval(ll)
    #print ll2
    ##config = ConfigParser.ConfigParser()
    ##config.read(r'config.ini')
    #print config.items('bitbucket') 
    ##print config.printdummy()
    
    
from module_dict_process import *
from process_compxtva import *
from combinetvacvl import *
import os,collections
from shutil import copyfile
if(__name__ == "__main__"):
    cleanoutputdir()
    
    logger = logging.getLogger()
    handler = logging.FileHandler('error_logfile.log','wb')
    formatter = logging.Formatter('%(filename)-25s%(lineno)-5s %(levelname)-8s %(message)s')
    handler.setFormatter(formatter)
    logger.addHandler(handler)
    logger.setLevel(logging.DEBUG)
    # logger.debug("This is a debug message")
    commondictobj = module_dict_process()
    commondictobj.logger = logger
    for k_sp1,v_sp2 in commondictobj.specmap.iteritems():
        dict4 = None
        if(k_sp1.strip().startswith('#')):
            continue
        file_tva = k_sp1+'_TVA.txt'
        fnsplit = file_tva.split('_')
        spec1name = fnsplit[0]
        spec1type = fnsplit[-1]
        print fnsplit
        if spec1name == "51010-4":
            print 'processing 51010-4'
            processtva_complx('Input_Files\\51010-4_raw_TVA.txt',r'Input_Files\\51010-4_TVA.txt',3,2,10,8,1)
            processexception510104()
        elif spec1name == "51010-2":
            copyfile('Input_Files\\51010-2_raw_TVA.txt',r'Input_Files\\51010-2_TVA.txt')
            processexception510102()
        elif spec1name == "31124-2":
            print 'processing 31124-2'
            processtva_complx('Input_Files\\31124-2_raw_TVA.txt',r'Input_Files\\31124-2_TVA.txt',3,2,17,14,1)
            processexception31124()
        elif spec1name == "34123-2":
            process34123()
        path_tva = os.path.join(inputpath,file_tva)
        if(not os.path.exists(path_tva)):
            logger.error("File not found: "+ path_tva)
            continue
            
        readtableobj_1 = parsepixittable(path_tva)
        # regex changed for adding spec 34.229
        #readtableobj_1.setparam('\t',5,'^\d\S*',0,1) # please make sure TCNO starts with a digit; otherwise change 3rd parameter
        print path_tva
        path_cvl = path_tva.replace("TVA","CVL")
       
        logger.info("File: "+ path_tva)
        logger.info("spec1name: "+ spec1name)
        ## tc vs cond
        if spec1name == "31121-2": 
            dict1keyidx = 3
            dict1validx = 15
            dict1regexstr = r'^[a-z]\.|^\d+\.'
        elif spec1name == "51010-2": 
            dict1keyidx = 0
            dict1validx = 5
            dict1regexstr = r'^\d+[ab]?\.'
        else:
            dict1keyidx = 0
            dict1validx = 3
            dict1regexstr = r'^[a-z]\.|^\d+\.'
        readtableobj_1.setparam('\t',5,dict1regexstr,0,1) # please make sure TCNO starts with a digit; otherwise change 3rd parameter
        dict1 = readtableobj_1.makedictionary(dict1keyidx,dict1validx,True) #for TCNO:C value; for TCNO: REL value look at the initialization
        if spec1name.startswith("36521-2-1"):
            dict4 = readtableobj_1.makedictionary(0,5,True)
            # print dict4
        #for k,v in dict1.iteritems():
        #    print "PR1",k,v
        # tc vs release
        dict3 = readtableobj_1.makedictionary(dict1keyidx,2,True)
        #for k,v in dict3.iteritems():
        #    print "PR2",k,v
        #generatecombinedtva(tvafile,relidx=2,conidx= 3,Didx = None, tcidx=0)
        generatecombinedtva(file_tva, 2,dict1validx, 5 , dict1keyidx)
        readtableobj_2 = parsepixittable(path_cvl)
        readtableobj_2.setparam('\t',1,'^c(:?_RF)?\S+',0,2)
        #dict2 = {}
        dict2 = collections.OrderedDict()
        dict2 = readtableobj_2.makedictionary(0,1)
        #for k, v in dict2.iteritems():
        #    print k,v, "dic2"
        
        #dict3 = readtableobj_1.makedictionary(0,5)
        cleandict13(dict1,dict3,dict4)
        #cleandict1(dict1)
        #cleanreleaseinformation(dict3)
        cleandict2(dict2)
        
        commondictobj.spec1name = spec1name
        commondictobj.spec1type = spec1type
        
        #for k, v in dict1.iteritems():
        #    print k,v, "dic1"
        commondictobj.combinetwodict(dict1,dict3,dict4)
        commondictobj.prepareCVL(dict2)
    #commondictobj.makecommondict()
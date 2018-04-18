import csv,os,re,sys
def comparefile(filemain,file2):
    fileout = 'compare\\'+os.path.basename(filemain)
    with open(file2 , 'rb') as f2:
         lines = f2.read().splitlines()
    extraline =[]
    print filemain
    
    with open(fileout , 'wb') as fo:
        with open(filemain , 'rb') as fm:
            for lm in fm:
            
                i = 0
                lineno = 0
                for l2 in lines:
                    lm1 = lm.split('\t')[0]
                    l21 = l2.split('\t')[0]
                    if(lm1==l21):
                        i = 1
                        extraline.append(lineno)
                        fo.write(l2+'\n')
                        #print 'match',lm1,l21,lm
                        break
                    lineno+=1
                #print 'i',i
                if not i:
                    #print 'i',i
                    fo.write('\n')
            #print extraline
            fo.write('########## extra line on file 2\n')
            lineno = 0 
            for l2 in lines:
                if lineno not in extraline:
                    fo.write(l2+'\n')
                lineno+=1
                
if __name__ =='__main__':
    comparefile(sys.argv[1],sys.argv[2])
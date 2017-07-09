import gzip
import shutil
import sys
import os


def get_correct_name():
    return os.path.basename(sys.argv[1])


with open(sys.argv[1], 'rb') as f_in, gzip.open(get_correct_name() + '.gz', 'wb') as f_out:
   shutil.copyfileobj(f_in, f_out) 


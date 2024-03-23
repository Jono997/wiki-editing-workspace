from __common__ import *
from sys import argv
import os

IGNORE_FIELDS = ['version', 'version_hash']

def help():
    print("Usage:")
    print("edit_pagedata.py <file>")
    print("edit_pagedata.py <file> <parameter> <new_value(s)>")

def main_show():
    pd_path = pagedata_path(argv[1])
    pagedata = read_json(pd_path)
    for k in pagedata:
        if k in IGNORE_FIELDS:
            continue
        v = pagedata[k]
        if type(v) is list:
            print("Array:")
            for i in range(0, len(v)):
                print(f"\t{v[i]}")
        elif type(v) is str:
            print(f"'{k}':\t\"{v}\"")
        else:
            print(f"'{k}':\t{v}")

def main_modify():
    if len(argv) < 4:
        help()
        return
    pd_path = pagedata_path(argv[1])
    pagedata = read_json(pd_path)
    pagedata[argv[2]] = eval(argv[3])
    write_json(pagedata, pd_path)
    print("Pagedata updated successfully")

def main():
    if len(argv) < 2:
        help()
        return
    if not os.path.isfile(pagedata_path(argv[1])):
        print(f"Pagedata file '.scripts/pagedata/{argv[1]}.json' doesn't exist.")
        return

    if len(argv) == 2:
        main_show()
    else:
        main_modify()

if __name__ == "__main__":
    main()
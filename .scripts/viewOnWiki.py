from __common__ import *
from sys import argv
import os

def main():
    if len(argv) < 2:
        print("Usage:")
        print("viewOnWiki.py <page>")
        return
    pd_path = pagedata_path(argv[1])
    if not os.path.isfile(pd_path):
        print(f"'{pd_path}' could not be found")
        return
    pagedata = read_json(pd_path)
    os.startfile(page_url(pagedata["wiki"], pagedata["page"]))

if __name__ == "__main__":
    main()
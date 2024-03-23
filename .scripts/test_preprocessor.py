from __common__ import *
from sys import argv
import os

def main():
    if len(argv) < 2:
        print("Usage:")
        print("test_preprocessor.py <file>")
        return
    file_path = page_path(argv[1])
    if not os.path.isfile(file_path):
        print(f"File '{argv[1]}' could not be found")
        return
    wikitext = read_file(file_path)
    pagedata = read_json(pagedata_path(argv[1]))
    wikitext = process_to_wiki(wikitext, pagedata)
    write_file(wikitext, f"TEST OUTPUT.{file_path.split('.')[-1]}")

if __name__ == "__main__":
    main()
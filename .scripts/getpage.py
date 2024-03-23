from sys import argv
import json
import os
from urllib.error import HTTPError
from __common__ import *

def help():
    print("getpage get <filename> <wiki> <pagename>: Creates <filename>, containing the contents of <pagename> on wiki and creates a pagedata entry for <pagename>")
    print("getpage setup <filename>: Goes through a step-by-step extended version of 'get' that allows for addional settings to be configured (eg. preprocessors, if the page should be checked, etc.)")
    print("getpage update <filename>: Repopulates <filename>'s contents with the latest version from the wiki. <filename> must have a pagedata entry already for this to work")

def main_get():
    if len(argv) < 5:
        print("Insufficient number of arguments")
        help()
        return
    file_path = argv[2]
    fp = page_path(file_path)
    wiki = argv[3].lower()
    page_name = argv[4]
    if os.path.isfile(fp):
        if input("'" + file_path + "' already exists. Are you sure you want to overwrite it? (y/n): ").lower() != 'y':
            return
    
    page = get_page(wiki, page_name)
    pagedata = {"wiki": wiki, "page": page_name, "version": page['revid'], "version_hash": wikitext_hash(page['wikitext']), "watch": True, "preprocessor": None}
    pdp = pagedata_path(file_path)
    make_path(pdp)
    write_json(pagedata, pdp)
    make_path(fp)
    write_file(page['wikitext'], fp)
    print("Page downloaded successfully")

def main_setup():
    if os.path.isfile(page_path(argv[2])):
        if input("'" + argv[2] + "' already exists. Are you sure you want to overwrite it? (y/n): ").lower() != 'y':
            return
    pagedata = {}
    page_text = ""
    while True:
        pagedata['wiki'] = input("Wiki id (eg. phigros): ").lower()
        if pagedata['wiki'] == 'exit':
            return
        pagedata['page'] = input("Page name (eg. Glaciaxion): ")
        try:
            print("Downloading page, please wait...")
            page = get_page(pagedata['wiki'], pagedata['page'])
            pagedata['version'] = page['revid']
            pagedata['version_hash'] = wikitext_hash(page['wikitext'])
            page_text = page['wikitext']
            break
        except MediawikiException as e:
            if e.mwerrcode == 'missingtitle':
                print("The page '" + pagedata['page'] + "' could not be found on https://" + pagedata['wiki'] + ".fandom.com")
                print("Please make sure the wiki and page are inputted correctly. If you would like to exit the setup process, type 'exit' as the wiki id.")
            else:
                raise e
        except HTTPError as e:
            if e.code == 404:
                print("The wiki https://" + pagedata['wiki'] + ".fandom.com could not be found")
                print("Please make sure the wiki and page are inputted correctly. If you would like to exit the setup process, type 'exit' as the wiki id.")
            else:
                raise e
    if input("Watch this page in the background for changes? (y/n): ").lower() == 'y':
        pagedata['watch'] = True
    else:
        pagedata['watch'] = False
    preprocessors = os.listdir("preprocessors")
    if len(preprocessors) > 0:
        i = 0
        while i < len(preprocessors):
            file = preprocessors[i].split('.')
            if file[-1] != "py":
                preprocessors.pop(i)
                continue
            preprocessors[i] = preprocessors[i][:-1 - len(file[-1])]
            i += 1
        while True:
            for i in range(0, len(preprocessors)):
                print(preprocessors[i])
            preprocessor = input("Select preprocessor (type 'none' if you don't want any): ")
            if preprocessor in preprocessors:
                pagedata['preprocessor'] = preprocessor
                module = import_preprocessor(preprocessor)
                pagedata = module.setup(pagedata)
                break
            elif preprocessor.lower() == 'none':
                pagedata['preprocessor'] = None
                break
    else:
        pagedata['preprocessor'] = None
    make_path(pagedata_path(argv[2]))
    write_json(pagedata, pagedata_path(argv[2]))
    write_file(process_to_client(page_text, pagedata), page_path(argv[2]))

def main_update():
    file_path = page_path(argv[2])
    if not os.path.isfile(file_path):
        print(f"'{argv[2]}' could not be found")
        return
    pdp = pagedata_path(argv[2])
    if not os.path.isfile(pdp):
        print(f"Pagedata file ('.scripts/pagedata/{file_path}.json') could not be found")
        return
    pagedata = read_json(pdp)
    page = get_page(pagedata['wiki'], pagedata['page'])
    wikitext = process_to_client(page['wikitext'], pagedata)
    pagedata['version'] = page['revid']
    pagedata['version_hash'] = wikitext_hash(wikitext)
    write_file(wikitext, file_path)
    write_json(pagedata, pdp)
    print("Page updated successfully")

def main():
    if len(argv) > 2:
        if argv[1] == "get":
            main_get()
            return
        elif argv[1] == "setup":
            main_setup()
            return
        elif argv[1] == "update":
            main_update()
            return
    help()

if __name__ == "__main__":
    main()

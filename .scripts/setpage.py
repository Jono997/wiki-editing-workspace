from sys import argv
import json
from __common__ import *

def edit_page(wiki, page, content, summary=''):
    url = api_url(wiki)
    session = get_session(wiki)
    token_response = req_get(session, url, {
        'action': 'query',
        'meta': 'tokens',
        'format': 'json'
    })
    edit_token = token_response['query']['tokens']['csrftoken']
    edit_response = req_post(session, url, {
        'action': 'edit',
        'format': 'json',
        'title': page,
        'text': content,
        'summary': summary,
        'token': edit_token
    })

def process_wikitext(wikitext, preprocessor):
    if preprocessor == None:
        return wikitext
    module = import_preprocessor(preprocessor)
    return module.to_wiki(wikitext)

def help():
    print("setpage <filename>")
    print("setpage <filename> <summary>")

def main():
    if len(argv) < 2:
        help()
        return
    pdp = pagedata_path(argv[1])
    if not os.path.isfile(pdp):
        print("Pagedata file: '.scripts/pagedata/" + argv[1] + ".json' could not be found.")
        return
    pagedata = read_json(pdp)
    wikitext = process_wikitext(read_file(page_path(argv[1])), pagedata['preprocessor'])
    edit_summary = ""
    if len(argv) > 2:
        edit_summary = argv[2]
    edit_page(pagedata['wiki'], pagedata['page'], wikitext, edit_summary)
    print("Success https://" + pagedata['wiki'] + ".fandom.com/wiki/" + pagedata['page'])

if __name__ == "__main__":
    main()
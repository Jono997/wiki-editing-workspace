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
    return edit_response

def help():
    print("setpage <filename>")
    print("setpage <filename> <summary>")

def main():
    if len(argv) < 2:
        help()
        return
    pdp = pagedata_path(argv[1])
    if not os.path.isfile(pdp):
        print(f"Pagedata file: '.scripts/pagedata/{argv[1]}.json' could not be found.")
        return
    pagedata = read_json(pdp)
    unprocessed_wikitext = read_file(page_path(argv[1])), pagedata
    wikitext = process_to_wiki(unprocessed_wikitext)
    edit_summary = ""
    if len(argv) > 2:
        edit_summary = argv[2]
    edit_response = edit_page(pagedata['wiki'], pagedata['page'], wikitext, edit_summary)['edit']
    if 'nochange' in edit_response.keys():
        print("No changes have been made since the last get/set")
    else:
        pagedata["version"] = edit_response["newrevid"]
        pagedata["version_hash"] = wikitext_hash(unprocessed_wikitext)
        write_json(pagedata, pdp)
        print(f"Success {page_url(pagedata['wiki'], pagedata['page'])}")

if __name__ == "__main__":
    main()
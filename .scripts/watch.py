from __common__ import *
from sys import argv

def main():
    if len(argv) < 2:
        print("Usage:")
        print("watch.py <file_path>")
        return
    pagedata = read_json(pagedata_path(argv[1]))
    if pagedata["watch"] == False:
        return
    result = {"result": 0, "content": "", "new_version": 0, "new_hash": "", "resolve_action": 0}
    page = get_page(pagedata["wiki"], pagedata["page"])
    if pagedata["version"] != page["revid"]:
        file = open(page_path(argv[1]), encoding='utf8')
        page_hash = wikitext_hash(file.read())
        file.close()
        if page_hash == pagedata["version_hash"]:
            file = open(page_path(argv[1]), 'w', encoding='utf8')
            file.write(page["wikitext"])
            file.close()
            result["result"] = 1
            pagedata["version"] = page["revid"]
            pagedata["version_hash"] = wikitext_hash(page["wikitext"])
            write_json(pagedata, pagedata_path(argv[1]))
        else:
            result["result"] = 2
            result["content"] = page["wikitext"]
            result["new_version"] = page["revid"]
            result["new_hash"] = wikitext_hash(page["wikitext"])
    write_json(result, "WatchResult.json")

if __name__ == "__main__":
    main()
import os
import hashlib
import urllib
import json
import importlib.util
import requests
import pickle

class MediawikiException(Exception):
    def __init__(self, err):
        message = err['code'] + "\r\n" + err['info']
        Exception.__init__(self, message)
        self.mwerrcode = err['code']

class BadPathException(Exception):
    def __init__(self, path):
        Exception.__init__(self, "'" + path + "' is a file when a directory was expected")

class LoginFailedException(Exception):
    def __init__(self, reason):
        Exception.__init__(self, "Login failed: " + reason)

LOGIN_FILE = "login.dat"
SESSIONS_FILE = "sessions.dat"

def wiki_url(wiki):
    return f"https://{wiki}.fandom.com"

def api_url(wiki):
    return f"{wiki_url(wiki)}/api.php"

def page_url(wiki, page):
    return f"{wiki_url(wiki)}/wiki/{page}"

def wikitext_hash(wikitext):
    return hashlib.sha256(wikitext.encode('utf8')).hexdigest()

def page_path(page):
    return os.path.join("..", page)

def pagedata_path(page):
    return os.path.join("pagedata", page) + ".json"

def read_file(path):
    file = open(path, 'r', encoding='utf-8-sig')
    retVal = file.read()
    file.close()
    return retVal

def write_file(content, path):
    file = open(path, 'w', encoding='utf-8-sig')
    file.write(content)
    file.close()

def read_json(path):
    return json.loads(read_file(path))

def write_json(obj, path):
    write_file(json.dumps(obj), path)

def read_pickle(path):
    file = open(path, 'rb')
    retVal = pickle.load(file)
    file.close()
    return retVal

def write_pickle(obj, path):
    file = open(path, 'wb')
    pickle.dump(obj, file)
    file.close()

def make_path(path):
    path_split = os.path.normpath(path).split(os.sep)
    path_fragment = "."
    for i in range(0, len(path_split) - 1):
        path_fragment = os.path.join(path_fragment, path_split[i])
        if os.path.isfile(path_fragment):
            raise BadPathException(path_fragment)
        if not os.path.isdir(path_fragment):
            os.mkdir(path_fragment)

def import_preprocessor(preprocessor):
    spec = importlib.util.spec_from_file_location("preprocesser", os.path.join('preprocessors', preprocessor + ".py"))
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module

def process_to_client(wikitext, pagedata):
    if not "preprocessor" in pagedata.keys() or pagedata["preprocessor"] is None:
        return wikitext
    if type(pagedata['preprocessor']) is str:
        module = import_preprocessor(pagedata["preprocessor"])
        return module.to_client(wikitext, pagedata)
    else:
        working = wikitext
        for i in range(0, len(pagedata['preprocessor'])):
            module = import_preprocessor(pagedata['preprocessor'][i])
            working = module.to_client(working, pagedata)
        return working

def process_to_wiki(wikitext, pagedata):
    if not "preprocessor" in pagedata.keys() or pagedata["preprocessor"] is None:
        return wikitext
    if type(pagedata['preprocessor']) is str:
        module = import_preprocessor(pagedata["preprocessor"])
        return module.to_wiki(wikitext, pagedata)
    else:
        working = wikitext
        for i in range(len(pagedata['preprocessor']) - 1, -1, -1):
            module = import_preprocessor(pagedata['preprocessor'][i])
            working = module.to_wiki(working, pagedata)
        return working

def get_page(wiki, page):
    parameters = {'action': 'parse', 'page': page, 'prop': 'revid|wikitext', 'formatversion': '2', 'format': 'json'}
    url = f"{api_url(wiki)}?{urllib.parse.urlencode(parameters)}"
    response = urllib.request.urlopen(url)
    response = json.loads(response.read())
    if "error" in response.keys():
        raise MediawikiException(response['error'])
    return response["parse"]

def req_get(req, url, params):
    res = req.get(url=url, params=params)
    if res.status_code != 200:
        res.raise_for_status
    res_json = res.json()
    if 'error' in res_json.keys():
        raise MediawikiException(res_json['error'])
    return res_json

def req_post(req, url, data):
    res = req.post(url=url, data=data)
    if res.status_code != 200:
        res.raise_for_status
    res_json = res.json()
    if 'error' in res_json.keys():
        raise MediawikiException(res_json['error'])
    return res_json

def login(session, wiki, username, password):
    url = api_url(wiki)
    token_response = req_get(session, url, {
        'action': "query",
        'meta': "tokens",
        'type': "login",
        'format': "json"
    })
    login_token = token_response['query']['tokens']['logintoken']
    login_response = req_post(session, url, {
        'action': "login",
        'lgname': username,
        'lgpassword': password,
        'lgtoken': login_token,
        'format': "json"
    })

    if login_response['login']['result'] == "Failed":
        raise LoginFailedException(login_response['login']['reason'])
    return session

def get_session(wiki):
    session = requests.Session()
    if os.path.isfile(LOGIN_FILE):
        file = open(LOGIN_FILE, 'rb')
        login_data = pickle.load(file)
        file.close()
        session = login(requests.Session(), wiki, login_data['name'], login_data['pass'])
    req_get(session, api_url(wiki), {
        'action': "query",
        'meta': "tokens",
        'type': "csrf",
        'format': "json"
    })
    return session

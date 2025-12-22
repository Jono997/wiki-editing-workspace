from sys import argv, stdout, stderr
import json
from __common__ import *

def terminal_help():
    print("Enter any lua command and press Enter to execute it.")
    print("Type 'clear' to clear the session and start again")
    print("Type 'help' to see this message again")
    print("Type 'exit' to exit")

def terminal(wiki, page, content, first_command = None):
    url = api_url(wiki)
    session = get_session(wiki)
    token_response = req_get(session, url, {
        'action': 'query',
        'meta': 'tokens',
        'format': 'json'
    })
    user_token = token_response['query']['tokens']['csrftoken']
    post_base = {
        'action': 'scribunto-console',
        'format': 'json',
        'title': page,
        'token': user_token
    }
    session_token = None
    if first_command == None:
        terminal_help()

    while True:
        awaiting_command = True
        command = ""
        post = post_base.copy()
        if session_token == None:
            post['content'] = content
            post['clear'] = "true"
            if first_command != None:
                command = first_command
                awaiting_command = False
        else:
            post['session'] = session_token
        
        reset_session = False
        while awaiting_command:
            command = input("> ")
            if command == "help":
                terminal_help()
            elif command == "reset":
                awaiting_command = False
                reset_session = True
            elif command == "exit":
                return
            elif command == "":
                pass
            else:
                awaiting_command = False
        if reset_session:
            session_token = None
            continue

        post['question'] = command
        command_response = req_post(session, url, post)
        session_token = command_response['session']
        if command_response['type'] == 'normal':
            stdout.write(command_response["print"])
            stdout.flush()
        elif command_response['type'] == 'error':
            stderr.write(f"{command_response['message']}\n")
            stderr.flush()

        if first_command != None:
            return

def help():
    print("luaconsole <filename>")
    print("luaconsole <filename> <command>")

def main():
    if len(argv) < 2:
        help()
        return
    pdp = pagedata_path(argv[1])
    if not os.path.isfile(pdp):
        print(f"Pagedata file: '.scripts/pagedata/{argv[1]}.json' could not be found.")
        return
    pagedata = read_json(pdp)
    unprocessed_wikitext = read_file(page_path(argv[1]))
    wikitext = process_to_wiki(unprocessed_wikitext, pagedata)
    first_command = None
    if len(argv) > 2:
        first_command = argv[2]
    terminal(pagedata['wiki'], pagedata['page'], wikitext, first_command)

if __name__ == "__main__":
    main()
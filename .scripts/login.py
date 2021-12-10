import pickle
import requests
from __common__ import *

def main():
    login_data = {}
    login_data['name'] = input("Name: ")
    login_data['pass'] = input("Password: ")
    session = login(requests.Session(), "community", login_data['name'], login_data['pass'])
    file = open(LOGIN_FILE, 'wb')
    pickle.dump(login_data, file)
    file.close()
    print("Login successful")

if __name__ == "__main__":
    main()
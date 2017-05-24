# libraries
from src.server.server import Server
from src.database.database_handler import DatabaseHandler

def main():
    """
    entry point of this server application

    :return: void
    """

    d = DatabaseHandler.gmidbyb('####')

    print(d['id'])
    print(d['bc'])
    print(d['name'])

    DatabaseHandler.gcsofmbymid(d['id'])

    Server().run()

if __name__ == '__main__':
    main()

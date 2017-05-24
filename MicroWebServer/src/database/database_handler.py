# libraries
import cx_Oracle
from src.database.login import User, Database


# global variables
connection_string = u'' \
           + User.NAME.value + u'/' \
           + User.PASSWORD.value + u'@' \
           + Database.NAME.value + u':' \
           + Database.PORT.value + u'/' \
           + Database.SID.value


class DatabaseHandler:
    """
    class responsible for setting of database statements
    """
    @staticmethod
    def retrieve(request, statement, callback, connection_str=connection_string):
        """
        static function which opens a connection to the database, queries it
        with a given statement and then returns the data based on the given
        callback function

        :param request: the original request from the user
        :param statement: the statement for database query
        :param callback: the function to be executed after database querying
        :param connection_str: the routing to the database (default parameter
                is connection_string as seen above in 'global variables'

        :return: the retrieved data as serialized json (string type)
        """
        connection = cx_Oracle.connect(connection_str)
        cursor = connection.cursor()
        cursor.execute(statement)

        result = callback(request, cursor)

        connection.close()

        return result

    @staticmethod
    def gmidbyb(bc):
        statement = 

        connection = cx_Oracle.connect(connection_string)
        cursor = connection.cursor()
        cursor.execute(statement)

        for row in cursor:
            ret = {'id': str(row[0]), 'bc': str(row[1]), 'name': str(row[2])}

        connection.close()

        return ret

    @staticmethod
    def gcsofmbymid(id):
        statement = 

        connection = cx_Oracle.connect(connection_string)
        cursor = connection.cursor()
        cursor.execute(statement)

        title = [i[0] for i in cursor.description]

        print(title)

        s = ""

        for row in cursor:
            s += (str(row)) + "\n"

        print(s)

        connection.close()



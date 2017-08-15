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

        connection = cx_Oracle.connect(connection_str, threaded=True)

        cursor = connection.cursor()
        cursor.execute(statement)

        result = callback(request, cursor)

        cursor.close()
        connection.close()

        return result

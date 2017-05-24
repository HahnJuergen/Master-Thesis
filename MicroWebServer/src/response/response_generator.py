# libraries
import json
from src.utility.utility import Switch
from src.database.database_handler import DatabaseHandler
from src.database.statements import Statement


class ResponseGenerator:
    """
    class responsible for transforming the raw database data in serialized json
    strings based on the user's given GET-request to the server
    """

    def request(self, *args):
        """
        PUBLIC

        based on the user's request this function forwards the required query as
        a SQL-Statement to the DatabaseHandler and returns the retrieved data as
        serialized json (string type)

        :param args: a non-keyworded argument list containing the route of the
        user's request

        :return: the retrieved data as serialized json (string type)
        """
        while Switch(args[0]):
            if Switch.case('connect'):
                return self._connection_response(args[0])
            if Switch.case('dummy'):
                if args[1] == 'dummy' and not args[1] is None:
                    callback = self._dummy_response

                    return DatabaseHandler.retrieve(args[1],
                                                    Statement.DUMMY.value,
                                                    callback)

            return self._default()

    def _connection_response(self, request):
        """
        PRIVATE

        handler function for confirmation of server connection

        :param request: the user's original request

        :return: the serialized json containing the response
        """
        return json.dumps(self._add_to_response_dict(request, "ok", {}))

    def _default(self):
        """
        PRIVATE

        handler function for undefined user requests or undefined routing

        :return: the serialized json containing the response (debug purposes)
        """
        return json.dumps(self._add_to_response_dict("Default", "default", {}))

    def _dummy_response(self, request, cursor):
        s = ""

        for row in cursor:
            s += (str(row) + "\n")

        return self._add_to_response_dict(request, s, {})

    def _add_to_response_dict(self, key, val, dct, target=None):
        """
        PRIVATE

        helper function for creating a dictionary data structure which can
        be serialized to json
        Inserts a new dictionary (key, value) at a given target key (higher in
        the hierarchy when visualized as a directed tree) in a given dictionary

        This function searches the target key recursively.

        :param key: the key of the insertion dictionary
        :param val: the value of the insertion dictionary
        :param dct: the target dictionary to search for the 'target' param
        :param target: the target key to receive the new key / value pair

        :return: the root dictionary
        """
        if target is None:
            dct.update({key: val})
        else:
            for k in dct:
                if k == target:
                    dct[k].update({key: val})
                elif type(dct[k]) is dict:
                    dct[k] = self._add_to_response_dict(key, val, dct[k], target)

        return dct

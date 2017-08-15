# libraries
import json
from src.utility.utility import Switch
from src.database.database_handler import DatabaseHandler
from src.database.statements import Statement
from src.datastructure.machine_table import MachineTable
import os
from src.utility.utility import RepeatingTimer


class ResponseGenerator:
    """
    class responsible for transforming the raw database data in serialized json
    strings based on the user's given GET-request to the server
    """

    def __init__(self):
        self._machine_tables = {}
        self.repeatingTimer = None

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
        try:
            while Switch(args[0]):
                if Switch.case('connect'):
                    return self._connection_response(args[0])

                if Switch.case('update'):
                    self._update_machine_table()

                if Switch.case('machine'):
                    statement = ''
                    callback = None

                    while Switch(args[1]):
                        if Switch.case('id'):
                            statement = Statement.get_machine_id_by_barcode_statement(args[2])
                            callback = self._machine_data_response
                            break

                        if Switch.case('component_setup'):
                            statement = Statement.get_component_setup_of_machine_by_machine_id_statement(args[2])
                            callback = self._machine_component_setup_response
                            break

                        if Switch.case('order'):
                            statement = Statement.get_component_setup_of_machine_by_machine_id_statement(args[2])
                            callback = self._machine_component_setup_response
                            DatabaseHandler.retrieve(args[2], statement, callback)

                            return self._machine_tables[args[2]]["order_header_data"]

                        if Switch.case('component_rejection'):
                            statement_a = Statement.get_component_rejection_rate_of_last_25_pcbs(args[2])
                            statement_b = Statement.get_component_rejection_rate_over_last_hour(args[2])

                            rate_a = DatabaseHandler.retrieve(args[2], statement_a, self._get_component_rejection_rate)
                            rate_b = DatabaseHandler.retrieve(args[2], statement_b, self._get_component_rejection_rate)

                            return {"request": str(args[2]), "component_rejection": str(rate_a if rate_a > rate_b else rate_b)}

                        if Switch.case('ok_nok_distribution'):
                            order_id = DatabaseHandler.retrieve(args[2], Statement.get_pcb_order_id_and_timestamp_by_machine_id_statement(args[2]), self._get_order_bc)
                            ok, nok = DatabaseHandler.retrieve(args[2], Statement.get_ok_nok_distribution_by_order_id_and_machine_id(str(order_id), str(args[2])), self._get_ok_nok_distribution)

                            return {"request": str(args[2]), "ok": str(ok), "nok": str(nok)}

                    return DatabaseHandler.retrieve(args[2], statement, callback)

                return self._default()
        except Exception:
            print("error at request")

    def initial_setup(self):
        self._create_machine_list()
        self._create_machine_table()

    def _get_order_bc(self, request, cursor):
        for row in cursor.fetchall():
            return row[0]

    def _get_ok_nok_distribution(self, request, cursor):
        for row in cursor.fetchall():
            return row[0], row[1]

    def _get_component_rejection_rate(self, request, cursor):
        for row in cursor.fetchall():
            lost_count = row[0]
            used_count = row[1]

            if lost_count is None or used_count is None:
                return -1.0

            return 0.0 if lost_count == 0.0 else 100 * lost_count / used_count

    def _create_machine_list(self):
        self.dct = dict()
        self.dct['connect'] = dict()

        l = []

        callback = self._machine_data_response

        absolut_path = os.path.split(os.path.dirname(__file__))[0]

        with open(absolut_path + '/config.json') as data_file:
            data = json.load(data_file)

        for i in range(0, len(data['machine_line'])):
            statement = Statement.get_machine_id_by_barcode_statement(data['machine_line'][i]['bc'])
            d = DatabaseHandler.retrieve(data['machine_line'][i]['bc'], statement, callback)
            l.append(json.loads(d)['response'])

        self.dct['connect'] = l

    def _create_machine_table(self):
        callback = self._machine_component_setup_response

        for obj in self.dct['connect']:
            id = obj['machine_id']
            statement = Statement.get_component_setup_of_machine_by_machine_id_statement(id)
            DatabaseHandler.retrieve(id, statement, callback)

    def _connection_response(self, request):
        """
        PRIVATE

        handler function for confirmation of server connection

        :param request: the user's original request

        :return: the serialized json containing the response
        """

        return self.dct

    def _machine_data_response(self, request, cursor):
        dct = {}

        for row in cursor.fetchall():
            dct = self._add_to_response_dict('request', request, dct)
            dct = self._add_to_response_dict('response', {}, dct)
            dct = self._add_to_response_dict('machine_id', str(row[0]), dct, 'response')
            dct = self._add_to_response_dict('machine_bc', str(row[1]), dct, 'response')
            dct = self._add_to_response_dict('machine_name', str(row[2]), dct, 'response')

        return json.dumps(dct)

    def _machine_component_setup_response(self, request, cursor):
        if request not in self._machine_tables:
            self._machine_tables[request] = MachineTable(request)

        rows = self._get_rows_from_cursor(cursor)

        duplicates = []

        for i in range(1, len(rows)):
           if str(rows[i - 1][1]) == str(rows[i][1]):
               duplicates.append({"bay": rows[i][0], "track": rows[i][1]})

        if len(duplicates) > 0:
            return self._build_json_response_error_machine_component_setup(duplicates)

        about_to_empty_list = self._get_list_tracks_about_to_empty(rows, request)

        return self._build_json_response_machine_component_setup(about_to_empty_list, request)

    def _build_json_response_error_machine_component_setup(self, duplicates):
        dct = dict()
        dct["success"] = "false"
        dct["error"] = duplicates

        return dct

    def _build_json_response_machine_component_setup(self, about_to_empty_list, request):
        dct = dict()
        dct[request] = []
        dct["success"] = "true"

        for i in range(0, len(about_to_empty_list)):
            dct[request].append(about_to_empty_list[i])

        return dct

    def _get_rows_from_cursor(self, cursor):
        rows = []

        for row in cursor.fetchall():
            if row[0] != 'B1' and row[0] != 'B2' and row[0] != 'B3' and row[0] != 'B4':
                continue

            rows.append(row)

        return rows

    def _get_list_tracks_about_to_empty(self, rows, request):
        last_index = len(rows) - 1

        self._machine_tables[request].set_order_id()
        self._machine_tables[request].set_average_pcb_process_time()

        for i in range(0, len(rows)):
            if i == last_index:
                self._machine_tables[request].update_with_data_row(rows[i], True)
            else:
                self._machine_tables[request].update_with_data_row(rows[i], False)

        return self._machine_tables[request].get_track_about_to_empty_list()

    def _default(self):
        """
        PRIVATE

        handler function for undefined user requests or undefined routing

        :return: the serialized json containing the response (debug purposes)
        """
        return json.dumps(self._add_to_response_dict("Default", "default", {}))

    def initialize_updater(self):
        self.initial_setup()
        self._update_machine_table()

        self.start_timer(30)

    def start_timer(self, time_interval):
        if self.repeatingTimer is not None:
            self.repeatingTimer.stop()

        self.repeatingTimer = RepeatingTimer(time_interval, self._update_machine_table)

    def _update_machine_table(self):
        self._create_machine_table()

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

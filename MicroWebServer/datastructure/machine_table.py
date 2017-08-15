import math
from operator import itemgetter
from src.database.database_handler import DatabaseHandler
from src.database.statements import Statement


class MachineTable(dict):
    def __init__(self, access_key):
        dict.__init__(self)

        self._access_key = access_key
        self[self._access_key] = dict()
        self._tracks_about_to_get_empty = []
        self._current_order_id = -1
        self._average_processing_time = 1
        self._current_order_last_timestamp = -1
        self["order_header_data"] = dict()

    def set_order_id(self):
        DatabaseHandler.retrieve("order_id", Statement.get_pcb_order_id_and_timestamp_by_machine_id_statement(self._access_key),
                                 self._order_id_retrieval_callback)

    def _order_id_retrieval_callback(self, request, cursor):
        for row in cursor.fetchall():  # one row at all times
            order_id = str(row[0])
            timestamp = row[1]

            if (self._current_order_id == -1) or (order_id != self._current_order_id):
                self[self._access_key] = dict()
                self._tracks_about_to_get_empty = []
                self._current_order_id = order_id
                self[self._access_key]["order_header_data"] = dict()

                DatabaseHandler.retrieve("order_header", Statement.get_order_header_data_by_order_bc_statement(order_id),
                                         self._order_header_retrieval_callback)

            self._current_order_last_timestamp = timestamp

    def set_average_pcb_process_time(self):
        DatabaseHandler.retrieve("avg_pcb_processing_time",
                                 Statement.get_average_pcb_processing_time_by_order_id_and_machine_id_statement(str(self._current_order_id),
                                                                                                                str(self._access_key)),
                                 self._average_pcb_process_time_retrieval_callback)

    def _average_pcb_process_time_retrieval_callback(self, request, cursor):
        for row in cursor.fetchall():
            self._average_processing_time = row[0]

    def _order_header_retrieval_callback(self, request, cursor):
        for row in cursor.fetchall():  # one row at all times
            self["order_header_data"]["order_bc"] = str(row[0])
            self["order_header_data"]["product_bc"] = str(row[1])
            self["order_header_data"]["product_name"] = str(row[2])
            self["order_header_data"]["customer_name"] = str(row[3])
            self["order_header_data"]["order_nominal_value"] = str(row[4])
            self["order_header_data"]["revision"] = str(row[5])

    """
    calculate average over last x depletion values?
    cast floating point values to int --> under-estimation
    """
    def update_with_data_row(self, row, is_last):
        if row[0] not in self[self._access_key]:
            self._add_bay(row[0])

        if row[1] not in self[self._access_key][row[0]]:
            self._add_track_to_bay(row[0], row[1])
            self[self._access_key][row[0]][row[1]]['depletion_value'] = 0
            self[self._access_key][row[0]][row[1]]['cumulative_depletion'] = 0
            self[self._access_key][row[0]][row[1]]['cumulative_depletion_additions'] = 0
        else:
            self._evaluate_actual_value_change(row[4], row[0], row[1])

        self._apply_current_row_data(row)

        if is_last:
            self._update_tracks_about_to_get_empty_list()

    def get_track_about_to_empty_list(self):
        return self._tracks_about_to_get_empty[:5]

    def _add_bay(self, bay_key):
        self[self._access_key][bay_key] = {}

    def _add_track_to_bay(self, bay_key, track_key):
        self[self._access_key][bay_key][track_key] = {}

    def _evaluate_actual_value_change(self, actual_value, bay_key, track_key):
        change = actual_value - self[self._access_key][bay_key][track_key]['actual_value']
        last_depletion_value = self[self._access_key][bay_key][track_key]['depletion_value']

        if last_depletion_value == 0:
            self[self._access_key][bay_key][track_key]['depletion_value'] = abs(change)
            self[self._access_key][bay_key][track_key]['cumulative_depletion'] = abs(change)
            self[self._access_key][bay_key][track_key]['cumulative_depletion_additions'] = 1
        else:
            if change < 0:
                self[self._access_key][bay_key][track_key]['cumulative_depletion'] += abs(change)
                self[self._access_key][bay_key][track_key]['cumulative_depletion_additions'] += 1

                self[self._access_key][bay_key][track_key]['depletion_value'] = \
                    (self[self._access_key][bay_key][track_key]['cumulative_depletion'] / self[self._access_key][bay_key][track_key]['cumulative_depletion_additions'])

    def _apply_current_row_data(self, row):
        self[self._access_key][row[0]][row[1]]['component_bc'] = row[2]
        self[self._access_key][row[0]][row[1]]['component_name'] = row[3]
        self[self._access_key][row[0]][row[1]]['actual_value'] = row[4]
        self[self._access_key][row[0]][row[1]]['original_value'] = row[5]
        self[self._access_key][row[0]][row[1]]['timestamp'] = row[6]

    def _update_tracks_about_to_get_empty_list(self):
        self._update_track_about_to_get_empty_list(self._get_current_change_array())
        self._tracks_about_to_get_empty.sort(key=itemgetter('time'))

    def _get_current_change_array(self):
        l = []

        for key1 in self[self._access_key]:
            for key2 in self[self._access_key][key1]:
                if self[self._access_key][key1][key2]['depletion_value'] != 0:
                    num_pcbs = int(math.ceil(self[self._access_key][key1][key2]['actual_value'] / self[self._access_key][key1][key2]['depletion_value']))

                    time = num_pcbs * self._average_processing_time

                    l.append({"bay": key1, "track": key2, "num_pcbs": num_pcbs,
                              'component_bc': self[self._access_key][key1][key2]["component_bc"],
                              'actual_value': self[self._access_key][key1][key2]['actual_value'],
                              'depletion': self[self._access_key][key1][key2]['depletion_value'],
                              "time": time})

        return l

    def _update_track_about_to_get_empty_list(self, l):
        if len(self._tracks_about_to_get_empty) == 0:
            self._tracks_about_to_get_empty = l
        else:
            for c in l:
                has_overridden = False
                for i in range(0, len(self._tracks_about_to_get_empty)):
                    if self._tracks_about_to_get_empty[i]["track"] == c["track"] \
                            and self._tracks_about_to_get_empty[i]["bay"] == c["bay"]:
                        self._tracks_about_to_get_empty[i] = c
                        has_overridden = True
                        break

                if not has_overridden:
                    self._tracks_about_to_get_empty.append(c)

# libraries
from enum import Enum


class Statement(Enum):
    """
    class derived from Enum-Class

    holding the database statements
    """

    @staticmethod
    def get_machine_id_by_barcode_statement(bc):
        statement = \            

        return statement

    @staticmethod
    def get_component_setup_of_machine_by_machine_id_statement(mi):
        statement = \            

        return statement

    @staticmethod
    def get_pcb_order_id_and_timestamp_by_machine_id_statement(mi):
        statement = \            

        return statement

    @staticmethod
    def get_order_header_data_by_order_bc_statement(oi):
        statement = \            

        return statement

    @staticmethod
    def get_average_pcb_processing_time_by_order_id_and_machine_id_statement(oi, mi):
        statement = \            

        return statement

    @staticmethod
    def get_component_rejection_rate_over_last_hour(mbc):
        statement = \            

        return statement

    @staticmethod
    def get_component_rejection_rate_of_last_25_pcbs(mbc):
        statement = \            

        return statement

    @staticmethod
    def get_ok_nok_distribution_by_order_id_and_machine_id(oi, mi):
        statement = \            

        return statement


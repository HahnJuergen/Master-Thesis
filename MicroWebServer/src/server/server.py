# libraries
from lib.bottle import run, get
from src.response.response_generator import ResponseGenerator
from src.server.login import *
from src.utility.utility import Switch

# global variables
responseGenerator = ResponseGenerator()
default_endpoint = '/request'


class Server:
    """
    class responsible for starting and maintaining the server with the
    possibility to query data via GET-Requests for through various endpoints
    """

    def __init__(self, host=DEFAULT_HOST, port=DEFAULT_PORT):
        """
        constructor, setting the variables for host and port

        :param host: the target host (default parameter is localhost)
        :param port: the target port (default parameter is 8080)
        """
        self.host = host
        self.port = port

    @staticmethod
    @get(default_endpoint + '/<request>')
    def process_get(request):
        """
        catches single endpoint GET-Requests and forwards them to the
        ResponseGenerator

        This Function can be overloaded for the handling of different routes

        :param request: the endpoint of the GET-Request
        :return: the response of the server as serialized json (string type)
        """

        while Switch(request):
            if Switch.case('localization'):
                return "LOCALIZATION GET"

            return responseGenerator.request(request)

    @staticmethod
    @get(default_endpoint + '/<request>/<request2>')
    def process_get(request, request2):
        return responseGenerator.request(request, request2)

    def run(self):
        """
        PUBLIC

        start the server

        :return: void
        """
        run(host=self.host, port=self.port)

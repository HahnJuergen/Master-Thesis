# libraries
from lib.bottle import run, Bottle, request, response, route, install
from src.response.response_generator import ResponseGenerator
from src.server.login import *
from src.utility.utility import RepeatingTimer
import threading
from datetime import datetime
from PyQt5.QtCore import QObject, pyqtSignal

# global variables
responseGenerator = ResponseGenerator()
default_endpoint = '/request'
app = Bottle()


class Server(QObject):
    """
    class responsible for starting and maintaining the server with the
    possibility to query data via GET-Requests for through various endpoints
    """
    on_request_logged_update = pyqtSignal()

    def __init__(self, gui=None, host=DEFAULT_HOST, port=DEFAULT_PORT):
        """
        constructor, setting the variables for host and port

        :param host: the target host (default parameter is localhost)
        :param port: the target port (default parameter is 8080)
        """
        QObject.__init__(self)

        self.host = host
        self.port = port
        self.log = ""

        #self.on_request_logged_update.connect(lambda: gui.on_request_logged_update(self.log))

        #app.install(self._logger)

    @staticmethod
    @app.get(default_endpoint + '/<request>')
    @app.get(default_endpoint + '/<request>/<request2>')
    @app.get(default_endpoint + '/<request>/<request2>/<request3>')
    def process_get(request, request2=None, request3=None):
        return responseGenerator.request(request, request2, request3)

    def run(self):
        """
        PUBLIC

        start the server

        :return: void
        """

        threading.Thread(target=responseGenerator.initialize_updater()).start()
        threading.Thread(target=run(app, host=self.host, port=self.port)).start()

    def _logger(self, func):
        def wrapper(*args, **kwargs):
            self.log = '' + request.remote_addr + " - - [" + str(datetime.now()) \
                + "] \"" + request.method + " " + request.url + "\" " + response.status

            #self.on_request_logged_update.emit()

            req = func(*args, *kwargs)

            return req
        return wrapper


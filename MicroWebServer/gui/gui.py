from PyQt5.QtWidgets import QApplication, QWidget, QTextEdit, QPushButton
from PyQt5.QtGui import QTextCursor
from PyQt5.QtCore import QObject
from src.server.server import Server


class GUI(QWidget, QObject):
    def __init__(self, parent=None):
        QWidget.__init__(self)
        self.resize(500, 500)
        self.setWindowTitle("HL-Application-Server")

        self.serverRequestLoggingLabel = QTextEdit(self)
        self.serverRequestLoggingLabel.setReadOnly(True)
        self.serverRequestLoggingLabel.setLineWrapMode(QTextEdit.NoWrap)
        self.serverRequestLoggingLabel.font().setFamily("Courier")
        self.serverRequestLoggingLabel.font().setPointSize(10)
        self.serverRequestLoggingLabel.moveCursor(QTextCursor.End)
        self.serverRequestLoggingLabel.setGeometry(10, 10, self.width() - 20, 300)

        self.serverStartButton = QPushButton("Start Server", self)
        self.serverStartButton.setToolTip('<b>This activates the Application-Server.</b>')
        self.serverStartButton.setGeometry(10, 320, 70, 25)

        self.server = Server(self)

        self.serverStartButton.clicked.connect(self._start_server)

        self.show()

    def _start_server(self):
        server = Server(self)

        try:
            server.run()
            pass
        except RuntimeError:
            pass

    def on_request_logged_update(self, log):
        print(log)

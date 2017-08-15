# libraries
import os

project_home = os.path.realpath(__file__)
project_home = os.path.split(project_home)[0]
import sys

sys.path.append(os.path.split(project_home)[0])

from src.gui.gui import GUI
from PyQt5.QtWidgets import QApplication, QWidget, QTextEdit, QPushButton
from PyQt5.QtGui import QTextCursor

from src.server.server import Server


def main():
    """
    entry point of this server application

    :return: void
    """   

    server = Server()

    try:
        server.run()
        pass
    except RuntimeError:
        pass

    #app = QApplication(sys.argv)
    #gui = GUI()

    #sys.exit(app.exec())

if __name__ == '__main__':
    main()

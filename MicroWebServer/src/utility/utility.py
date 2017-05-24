# libraries
from threading import Timer
import time


class Switch(object):
    """
    class derived from object

    implementation of the switch/case functionality to largely reproduce the
    syntax of C-Like programming languages
    """
    value = None

    def __new__(cls, value):

        """
        overriding of this built-in function for controlling the
        creation of a new object instance of this class and returns it

        :param value: the variable which case has to be executed

        :return: True
        """
        cls.value = value

        return True

    def case(*args):
        """
        function handling the comparison of the 'switched' variable with the
        parameter/s passed to this function and returning a boolean value

        :param args: a non-keyworded argument list containing the variable/s
        needed to be checked with the 'switched' variable

        :return: a boolean value depicting whether the given parameter equals
        the 'switched' parameter
        """
        return any((arg == Switch.value for arg in args))


class RepeatingTimer:
    """
    class responsible for the creation, start up and maintenance of an
    asynchronous timer based execution of a given function
    """
    def __init__(self, interval, callback, *args, **kwargs):
        """
        constructor

        :param interval: the time interval in seconds in which the given
        function is asynchronously called
        :param callback: the callback function to be called
        :param args: a non-keyworded argument list containing arguments
        of the callback function
        :param kwargs: a keyworded argument list containing arguments of the
        callback function
        """
        self.timer = None
        self.interval = interval
        self.callback = callback
        self.args = args
        self.kwargs = kwargs
        self.is_running = False
        self.next_call = time.time()
        self.start()

    def run(self):
        """
        starts the timer and calls the callback function

        :return: void
        """
        self.is_running = False
        self.start()
        self.callback(*self.args, **self.kwargs)

    def start(self):
        """
        starts the timer and checks the passed time in order to repeat the
        call of the callback function based on the given time interval

        :return: void
        """
        if not self.is_running:
            self.next_call += self.interval
            self.timer = Timer(self.next_call - time.time(), self.run)
            self.timer.start()
            self.is_running = True

    def stop(self):
        """
        stops the timer

        :return: void
        """
        self.timer.cancel()
        self.is_running = False

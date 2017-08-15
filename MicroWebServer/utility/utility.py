# libraries
from threading import Timer
import time


def compare_dates(d1, d2):
    """
    checks whether d1 lies in the past/present or future when compared to d2

    :param d1: the date to determine whether it is in the future or past relative to d2
    :param d2: the date serving as threshold for getting the time arrangement of d1

    :return: true, if d1 lies in the future relative to d2, false if it lies in the past
    """

    return d2 < d1


def fletcher_16_check_sum_for_lists_of_dicts(l, target, M):
    """
    based on fletcher's checksum with M=255000;
    calculates and returns a fletcher checksum of a list of dicts
    based on target key

    :param l: the list_of_track_about_to_empty_objects: the list for which a checksum is generated
    :param target: the key which value has to be an integer and present in all
                    objects in the list used for checksum generation
    :param M: the modulo divisor

    :return: the checksum of this list
    """
    sum1 = sum2 = 0

    for o in l:
        value = int(o[target])

        sum1 = (sum1 + value) % M
        sum2 = (sum2 + sum1) % M

    return (sum2 << 8) | sum1


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

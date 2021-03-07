import re
from subprocess import PIPE, Popen


class SystemUnderTest:
    def __init__(self, system_under_test_binary_name):
        print("start sut ctor")
        self.__core = Popen(system_under_test_binary_name, stdin=PIPE, stdout=PIPE, stderr=PIPE, text=True)
        print("end sut ctor")

    def __del__(self):
        if hasattr(self, '__core'):
            self.__core.terminate()

    def apply_permutations(self, num, permutations):
        self.__core.stdout.readline()
        self.__core.stdin.write("2\n")
        self.__core.stdin.flush()
        self.__core.stdout.readline()
        self.__core.stdin.write(f"{num}\n")
        self.__core.stdin.flush()
        self.__core.stdout.readline()
        self.__core.stdin.write(f"{permutations}\n")
        self.__core.stdin.flush()
        applying_permutations_output = self.__core.stdout.readline()
        print(applying_permutations_output)
        actual_applying_permutations_result = re.findall(r'\d+', applying_permutations_output)[0]
        return actual_applying_permutations_result

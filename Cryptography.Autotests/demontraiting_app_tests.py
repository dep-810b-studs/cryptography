import sys
import os
from subprocess import Popen, PIPE

if __name__ == "__main__":

    if len(sys.argv) < 2:
        print('There is not enough command line arguments. Tests cant work')
        sys.exit()

    system_under_test_binary_name = sys.argv[1]

    if not os.path.exists(system_under_test_binary_name):
        print(f'File {system_under_test_binary_name} doesnt exist. Program cant work')
        sys.exit()

    system_under_test = Popen(system_under_test_binary_name, stdin=PIPE, stdout=PIPE, stderr=PIPE, text=True)
    print(system_under_test.stdout.readline())
    system_under_test.stdin.write("1\n")
    system_under_test.stdin.flush()
    print(system_under_test.stdout.readline())

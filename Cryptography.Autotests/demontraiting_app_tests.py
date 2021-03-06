import sys
import os
from subprocess import Popen, PIPE
import re

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
    system_under_test.stdin.write("2\n")
    system_under_test.stdin.flush()
    print(system_under_test.stdout.readline())
    system_under_test.stdin.write("10000000110000001110000011110000\n")
    system_under_test.stdin.flush()
    print(system_under_test.stdout.readline())
    system_under_test.stdin.write("3 2 1 0\n")
    system_under_test.stdin.flush()
    expected_applying_permutations_result = "11110000111000001100000010000000"
    applying_permutations_output = system_under_test.stdout.readline()
    system_under_test.terminate()
    actual_applying_permutations_result = re.findall(r'\d+', applying_permutations_output)[0]
    if actual_applying_permutations_result == expected_applying_permutations_result:
        print("Test for replacing bytes by permutations passed.")
    else:
        print("Test for replacing bytes by permutations not passed.")
        sys.exit(-1)


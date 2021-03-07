import sys
import os
from system_under_test import SystemUnderTest

if __name__ == "__main__":

    if len(sys.argv) < 2:
        print('There is not enough command line arguments. Tests cant work')
        sys.exit(1)

    system_under_test_binary_name = sys.argv[1]

    if not os.path.exists(system_under_test_binary_name):
        print(f'File {system_under_test_binary_name} doesnt exist. Program cant work')
        sys.exit(1)

    try:
        system_under_test = SystemUnderTest(system_under_test_binary_name)
        expected_applying_permutations_result = "11110000111000001100000010000000"
        actual_applying_permutations_result = system_under_test.apply_permutations("10000000110000001110000011110000",
                                                                                   "3 2 1 0")
    except Exception as e:
        print(f"There is something error during tests:{e}. Exiting.")
        sys.exit(1)

    if actual_applying_permutations_result == expected_applying_permutations_result:
        print("Test for replacing bytes by permutations passed.")
    else:
        print("Test for replacing bytes by permutations not passed.")
        sys.exit(2)

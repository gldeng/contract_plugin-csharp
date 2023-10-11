#!/usr/bin/env python3
import os

cur_dir = os.path.dirname(__file__)
proto_filename = "contract.proto"
descriptor_filename = 'descriptor.bin'
testcases_dir = os.path.abspath(os.path.join(cur_dir, '..', 'testcases'))


def get_command(testcase_name):
    return [
        "protoc",
        f'-I"{testcases_dir}/_common"',
        f'-I"{testcases_dir}/{testcase_name}"',
        f'-o"{testcases_dir}/{testcase_name}/{descriptor_filename}"',
        "--include_imports",
        "--retain_options",
        # "--contract_opt=stub", tried following https://github.com/AElfProject/AElf/blob/bd91fc5e23f03f5179cee9fc6cfd5a0ee5b802d2/scripts/generate_event_only.sh#L13C1-L13C28 tho this throws protoc errors
        proto_filename
    ]


if __name__ == "__main__":
    import subprocess

    testcases = [d for d in os.listdir(testcases_dir) if not d.startswith('_')]
    for tc in testcases:
        if os.path.isfile(f'{testcases_dir}/{tc}/{descriptor_filename}'):
            os.remove(f'{testcases_dir}/{tc}/{descriptor_filename}')
        command = get_command(tc)
        subprocess.run(" ".join(command), shell=True, check=True)

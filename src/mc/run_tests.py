
# :: for each folder except docs
# ::      find file presentation.sld
# ::      compile to expected\foldername\
# ::      compare actual\foldername with expected\foldername

import os
import filecmp
from pathlib import Path
import difflib
import sys
import webbrowser
import tempfile


def are_equal(actual_dir, expected_dir):
    actual_paths = Path(actual_dir).glob('*.*')

    for actual_path in actual_paths:
        file = str(actual_path).split("\\")[-1]
        actual_path = actual_dir + "/" + file
        expected_path = expected_dir + "/" + file
        if not Path(expected_path).exists():
            print("There is no test for", actual_path)
            return
        if not filecmp.cmp(actual_path, expected_path, False):
            fromlines = open(expected_path, 'r').readlines()
            tolines = open(actual_path, 'r').readlines()
            diff = difflib.HtmlDiff().make_file(fromlines, tolines, expected_dir, actual_dir)
            name = ''
            with tempfile.NamedTemporaryFile(delete=False) as fp:
                fp.write(bytes(diff, 'utf-8'))
                name = fp.name
            print(actual_path, "and", expected_path, "are not equal!")
            webbrowser.get('firefox').open(name)
            return


os.system('cmd /c "python docs_test_success.py"')

# specify your path of directory
path = "./tests"
source_path = path + "/source"
expected_path = path + "/expected"
actual_path = path + "/actual"

# call listdir() method
# path is a directory of which you want to list
directories = os.listdir(source_path)

# This would print all the files and directories
for file in directories:
    if file == "docs":
        continue
    path_to_file = source_path + "/" + file + "/presentation.sld"
    target_directory = actual_path + "/" + file
    # dotnet .\bin\Debug\netcoreapp2.1\mc.dll %1 %last_directory% %2 %3 %4 %5
    os.system(
        'cmd /c "dotnet ./bin/Debug/netcoreapp2.1\mc.dll {} {} -q"'.format(path_to_file, target_directory))
    are_equal(target_directory, expected_path + "/" + file)

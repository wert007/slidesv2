# shitty test suit mostly copy paste. dont judge

import filecmp
from pathlib import Path
import difflib
import sys
import webbrowser
import tempfile


# fromfile = "xxx"
# tofile = "zzz"
# fromlines = open(fromfile, 'U').readlines()
# tolines = open(tofile, 'U').readlines()

# diff = difflib.HtmlDiff().make_file(fromlines,tolines,fromfile,tofile)

# sys.stdout.writelines(diff)

paths = Path('./docs').glob('*/*.*')
for path in paths:
    path = str(path)
    path = '\\'.join(path.split('\\')[1:])
    expected_path = 'docs_expected\\' + path
    actual_path = 'docs\\' + path
    if not Path(expected_path).exists():
        print("There is no test for", path)
        exit(10)
    if not filecmp.cmp(actual_path, expected_path, False):
        fromlines = open(expected_path, 'r').readlines()
        tolines = open(actual_path, 'r').readlines()
        diff = difflib.HtmlDiff().make_file(fromlines, tolines, expected_path, actual_path)
        name = ''
        with tempfile.NamedTemporaryFile(delete=False) as fp:
            fp.write(bytes(diff, 'utf-8'))
            name = fp.name
        print(path, "and", expected_path, "are not equal!")
        webbrowser.get('firefox').open(name)
        exit(5)
# shitty test suit mostly copy paste. dont judge

import filecmp
from pathlib import Path

paths = Path('./docs').glob('*/*.*')
for path in paths:
    path = str(path)
    path = '\\'.join(path.split('\\')[1:])
    expected_path = 'docs_expected\\' + path
    if not Path(expected_path).exists():
        print("There is no test for", path)
        exit(10)
    if not filecmp.cmp('docs\\' + path, expected_path, False):
        print(path, "and", expected_path, "are not equal!")
        exit(5)

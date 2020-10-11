
import sys
import os

assert len(sys.argv) == 2

filename = sys.argv[1]
file = open(filename, 'r')

outfilename = os.path.splitext(filename)[0] + ".xml"
out = open(outfilename, 'w+')

out.write('<functions>\n')

for lineNumber, line in enumerate(file.readlines()):
    line = line.strip()
    if len(line) == 0:
        continue
    if line.startswith('//'):
        out.write('    <!-- {} -->\n'.format(line.strip('//')))
        continue
    fnName = line.split('(')[0].strip()
    argList = line.split('(')[1].split(')')[0].strip()
    returnType = line.split('>')[1].strip()[:-1]
    out.write('    <externFunction name="{}" returnType="{}">\n'
              .format(fnName, returnType))
    if len(argList) != 0:
        for arg in argList.split(','):
            arg = arg.strip()
            if ':' not in arg:
                print('Error in line', lineNumber, ": No ':' in parameter!")
                exit(9)
            argName = arg.split(':')[0].strip()
            argType = arg.split(':')[1].strip()
            out.write(
                '        <variable name="{}" type="{}"/>\n'
                .format(argName, argType))
    out.write('    </externFunction>\n')
    # print(fnName, argList, '->', returnType)

out.write('</functions>')

import gfont('Quicksand') as quicksand;
import gfont('Roboto Mono') as mono;

/**********     Notes   ****************
 - &= Assignment doesn't work!
 - Auto Conversion with BoundBinaryOperator.
let d = {
    'Hallo Welt': 6,
    'Tüdelü': 2,
};

d.add('lol', 99);
d['not_eZ'] = -5;

 - Think about auto child appliance of CustomStyles ("<style-name>, <style-name> *" selector)
 - Converting any to anything!
 - rename utf8 to utf32
****************************************/

/*This could be a constant

let sourceCode = code.loadFile('src_code\\main.c', 'C');

    maybe use a const keyword?

const sourceCode = code.loadFile('src_code\\main.c', 'C');

*/
style std:
    font = quicksand;
    color = hex('#595959');
    fontsize = 16pt;
    //TODO: Functioncalls in default-style???
    //List.setTextmarker('-');
endstyle

style contentStyle(element: any):
    element.margin.left = 40px;
endstyle

style defaultList(element: List):
    element.setTextmarker('-');
endstyle

style subListStyle(element: List):
    element.fontsize = 12pt;
    element.setTextmarker('-->');
endstyle

style subListStyleNoMarker(element: List):
    element.fontsize = 12pt;
endstyle

style captionedCode(element : Captioned):
    element.fontsize = 10pt;
    element.captionPlacement = CaptionPlacement.BottomOutwards;
    element.caption.background = hex('#e5e2e0');
    element.caption.padding = padding(5px);
endstyle

style captionedCodeSmall(element : Captioned):
    element.fontsize = 5pt;
    element.captionPlacement = CaptionPlacement.BottomOutwards;
    element.caption.background = hex('#e5e2e0');
    element.caption.padding = padding(5px);
endstyle

template withTitle(child: Slide):
    let title = child.getData('title') ?? 'No title specified';
    let lblTitle = new Label(title);
    lblTitle.fontsize = 28pt;
    lblTitle.margin = margin(12%, 8%);
    lblTitle.color = black;
    child.padding = margin(12% + (28pt / 1.6f), 8%, 12%, 8%);
endtemplate

group Title(main: string, sub: string):
    let lblMain = new Label(main);
    lblMain.align = Alignment.Center;
    lblMain.orientation = Horizontal.Stretch | Vertical.Top;
    lblMain.fontsize = 52pt;
    let lblSub = new Label(sub);
    lblSub.align = Alignment.Center;
    lblSub.orientation = Horizontal.Stretch | Vertical.Bottom;
    lblSub.fontsize = 28pt;
    lblSub.color = hex('#595959');
    width = max(lblMain.width, lblSub.width);
    height = 3f * lblMain.height + lblSub.height;
endgroup

slide title:
    let t = new Title('Implementierung des Quine–McCluskey Algorithmus’', 'von Niklas Leukroth & Pablo Hamacher');
    t.orientation = Horizontal.Stretch | Vertical.Center;
    t.margin = margin(10%);
endslide


slide overview < withTitle:
    setData('title', 'Gliederung');
    let contents = [
        'Programmstruktur',
        'Genutze Datenstrukturen',
        'Implementierung von Phase I',
        'Implementierung von Phase II',
        'Verbesserungsmöglichkeiten',
        'Vorführung des Programms',
    ];
    let list = new List(contents);
    list.applyStyle(defaultList);
    list.applyStyle(contentStyle);
    list.fontsize = 24pt;
endslide

slide programStructure < withTitle:
    setData('title', 'Programmstruktur');
    let list = new List();
    list.add('Einlesen der Wertetabelle im AcD-Format (A = 1 & a = 0)');
    list.add('Übersetzung des AcD-Formats in ternäre Werte mit möglichen\n"Don’t Care"-Werten (0, 1, 2)');
    list.add('Phase I');
    list.add(1, 'Rekursiv naheliegende Implikanten zusammenlegen');
    list.add('Phase II');
    list.add(1, 'Rekursiv naheliegende Implikanten zusammenlegen');
    list.add('Ausgabe der Primimplikanten im AcD-Format');
    

    list.isOrdered = true;
    list.applyStyle(contentStyle);
    list.applyStyle(1, subListStyle);
    //list.fontsize = 18pt;
endslide


//If you name the first parameter 'name' then you will hide the field of the group named
//'name'. The Binder will use the field of the group and cry because you won't use the parameter
//and the Evaluator just uses the parameter.
//Fucked up, eh?
group StructDescription(the_name: string, bulletPoints: string[], levels: int[]):
    //TODO: Either report a bug or support it!
    //println(name);
    let lblName = new Label($'{the_name}:');
    lblName.applyStyle(contentStyle);
    let list = new List();
    list.margin.top = lblName.bottomSide + 15px;
    list.applyStyle(defaultList);
    list.applyStyle(contentStyle);
    list.applyStyle(1, subListStyleNoMarker);
    //TODO
    //for bulletPoint, i in bulletPoints:
    for i in 0..bulletPoints.len():
        let bulletPoint = bulletPoints[i];
        let lvl = levels.getSafe(i) ?? 0;
        list.add(lvl, bulletPoint);
    endfor

    width = max(lblName.width, list.width);
    height = lblName.height + list.height + 15px;
endgroup

slide structCharArray < withTitle:
    setData('title', 'Datenstruktur: "char_array"');
    let container = seperator.vertical(55%);
    
    let contentsBP = [
        'unsigned int length',
        'Anzahl der Element',
        'char * data',
        'Zeiger auf die Elemente',
        'bool has_been_compared',
        'Gibt an, ob das Element in der compare Funktion bereits verglichen worden ist',
    ];
    let contentsBPLvl = [0, 1, 0, 1, 0, 1];
    container.fillA(new StructDescription('Felder', contentsBP, contentsBPLvl));

    let lblConclusion = new Label('==> Zum Speichern der ternären Werte');
    lblConclusion.orientation = Horizontal.Left | Vertical.Bottom;
    lblConclusion.applyStyle(contentStyle);

    code.setStyle(CodeHighlighter.Default);
    let sourceCode = code.loadFile('src_code\\main.h', 'C');
    let ~structCode = code.codeblock(sourceCode, 7..12);
    let ~cCode = new Captioned(~structCode, 'main.h');
    ~cCode.applyStyle(captionedCode);
    container.fillB(~cCode);
endslide


slide structListAndNode < withTitle:
    setData('title', 'Datenstruktur: "list" & "node"');

    let container = seperator.vertical(55%);
    let ~stack = new Stack(FlowAxis.Vertical);

    let listBulletPoints = [
        'Einfach verkettete Liste',
        'node *root',
        'Wurzelelement',
        'unsigned int length',
        'Länge der Liste'
    ];
    let nodeBulletPoints = [
        'node *next',
        'Folge-Node. Wert ist NULL, wenn Ende der Liste erreicht ist',
        'void *data',
        'Pointer auf Daten. Kann alles sein, auch weitere Listen.',
    ];
    stack.add(new StructDescription('Liste', listBulletPoints, [0,0,1,0,1]));
    stack.add(new StructDescription('Node',  nodeBulletPoints, [0,1,0,1]));
    stack.orientation = Orientation.Stretch;
    let sourceCode = code.loadFile('src_code\\main.h', 'C');
    let ~structCode = code.codeblock(sourceCode, 20..30);
    let ~cCode = new Captioned(~structCode, 'main.h');
    ~cCode.applyStyle(captionedCode);
    container.fill(~stack, ~cCode);
endslide

slide implementationOfPhaseIOverview1 < withTitle:
    setData('title', 'Implementierung von Phase I');
    let sourceCode = code.loadFile('src_code\\main.c', 'C');
    let ~structCode = code.codeblock(sourceCode, 111..123);
    let cCode = new Captioned(~structCode, 'main.c');
    cCode.applyStyle(captionedCode);
endslide

slide implementationOfPhaseIOverview2 < withTitle:
    setData('title', 'Implementierung von Phase I');
    let sourceCode = code.loadFile('src_code\\main.c', 'C');
    let ~structCode = code.codeblock(sourceCode, 125..134);
    let cCode = new Captioned(~structCode, 'main.c');
    cCode.applyStyle(captionedCode);
endslide

slide implementationOfPhaseIOverview3 < withTitle:
    setData('title', 'Implementierung von Phase I');
    let sourceCode = code.loadFile('src_code\\main.c', 'C');
    let ~structCode = code.codeblock(sourceCode, 136..151);
    let cCode = new Captioned(~structCode, 'main.c');
    cCode.applyStyle(captionedCode);
endslide

slide implementationOfPhaseIWrapItUp < withTitle:
    setData('title', 'Implementierung von Phase I');
    let sourceCode = code.loadFile('src_code\\main.c', 'C');
    let ~structCode = code.codeblock(sourceCode, 153..169);
    let cCode = new Captioned(~structCode, 'main.c');
    cCode.applyStyle(captionedCode);
endslide

slide implementationOfPhaseICompare < withTitle:
    setData('title', 'Implementierung von Phase I');
    let sourceCode = code.loadFile('src_code\\main.c', 'C');
    let container = seperator.vertical(55%);
    let ~structCode = code.codeblock(sourceCode, 448..479);
    let ~cCode = new Captioned(~structCode, 'main.c');
    ~cCode.applyStyle(captionedCodeSmall);
    ~cCode.captionPlacement = CaptionPlacement.BottomOutwards;
    let ~list = new List([
        'Vergleich eines jeden Elements der aktuellen Liste mit jedem Element der Folge-Liste',
        'Falls Unterscheidungs an nur einer Stelle und "Don\'t-Care"-Werte nur an gleichen Stellen vorkommen --> rufe combine_components() auf',
        'Falls Element nicht vergleichbar ist, füge es der finalen Liste hinzu'
    ]);
    ~list.applyStyle(defaultList);
    ~list.margin.left = 30px;
    container.fill(~cCode, ~list);
endslide


slide implementationOfPhaseICombineComponents < withTitle:
    setData('title', 'Implementierung von Phase I');
    let sourceCode = code.loadFile('src_code\\main.c', 'C');
    let ~structCode = code.codeblock(sourceCode, 537..550);
    let cCode = new Captioned(~structCode, 'main.c');
    cCode.applyStyle(captionedCode);
endslide













style quineMcCluskeyTableStyle(e: Table):
    e.fontsize = 9pt;
    e.align = Alignment.Center;
    e.font = mono;
endstyle

group QuineMcCluskeyTable(implicants: string[], simplifiedImplicants: string[], selected: int[], deleted: int[], alreadyFound: string[]):
    let completeFound = [:string;alreadyFound.len() + selected.len()];
    completeFound[..] = alreadyFound[..];
    for s in 0..selected.len():
        completeFound[s + alreadyFound.len()] = simplifiedImplicants[selected[s]];
    endfor
    let resultText = $'Ergebnis = {join(', ', completeFound)}';
    if completeFound.len() == 0:
        resultText = '';
    endif
    let labelResult = new Label(resultText);
    labelResult.orientation = Horizontal.Left | Vertical.Bottom;

    let table = new Table(implicants.len() + 1, simplifiedImplicants.len() + 1);
    table.orientation = Orientation.Stretch;
    table.margin = margin(0, 0, labelResult.height, 0);
    table.applyStyle(quineMcCluskeyTableStyle);
    table.setRow(implicants, 0, 1);
    table.setColumn(simplifiedImplicants, 0, 1);
    for x in 0..implicants.len():
        for y in 0..simplifiedImplicants.len():
            let tmp = true;
            let implicant = implicants[x];
            let simpleImplicant = simplifiedImplicants[y];
            for simpleCP in utf8(simpleImplicant):
                //let test = 'Hello World!';
                //BUG: simpleCP should either be int or any! not string!
                //test = simpleCP;

                tmp = tmp && contains(implicant, utf8(simpleCP));
            endfor
            if tmp:
                table.cells[x + 1][y + 1].content = 'x';
            endif
        endfor
    endfor

    let selectedRects = [:UnitRect?;selected.len()]; // table.rows * table.columns];
    for i in 0..selected.len():
        let s = selected[i];
        let ~left = table.cells[s + 1][0];
        let ~right = table.cells[s + 1][table.rows - 1];
        selectedRects[i] = rect(~left.relativePos(Vertical.Top | Horizontal.Left), ~right.relativePos(Vertical.Bottom | Horizontal.Right));
        selectedRects[i].stroke = orange;
        selectedRects[i].strokeWidth = 2px;
    endfor

    let deletedLines = [:UnitLine?;deleted.len()];
    for i in 0..deleted.len():
        let d = deleted[i];
        if d < simplifiedImplicants.len():
            let ~left = table.cells[d + 1][0];
            let ~right = table.cells[d + 1][table.rows - 1];
            deletedLines[i] = line(~left.relativePos(Vertical.Center | Horizontal.Left), ~right.relativePos(Vertical.Center | Horizontal.Right));
            deletedLines[i].stroke = red;
            deletedLines[i].strokeWidth = 2px;
        else
            d -= simplifiedImplicants.len();
            let ~top = table.cells[0][d + 1];
            let ~bottom = table.cells[table.columns - 1][d + 1];
            deletedLines[i] = line(~top.relativePos(Vertical.Top | Horizontal.Center), ~bottom.relativePos(Vertical.Bottom | Horizontal.Center));
            deletedLines[i].stroke = red;
            deletedLines[i].strokeWidth = 2px;
        endif
    endfor
    
    width = table.width;
    height = table.height + labelResult.height;
endgroup

style quineMcCluskeyStepsDefaultStyle(e : List):
    e.color = hex('#898989');
endstyle

style quineMcCluskeyStepsHighlightStyle(e : any):
    e.color = black;
endstyle

group QuineMcCluskeySteps(highlight: int):
    let list = new List();
    list.add('Erstellen einer Tabelle');
    list.add('Hinzufügen aller essentiellen Implikanten zum Ergebnis');
    list.add('Streichen der dominanten Spalten und dominierten Zeilen');
    list.add('Falls kein Element dem Ergebnis hinzugefügt wurde, wird das Element mit den wenigsten Primimplikanten gewählt');
    list.add(1, 'Streichen der dominanten Spalten und dominierten Zeilen');
    list.add('Wiederholen bis die Tabelle leer ist.');
    list.applyStyle(quineMcCluskeyStepsDefaultStyle);
    list.applyStyle(defaultList);
//    list.children[highlight].applyStyle(quineMcCluskeyStepsHighlightStyle);
    list.children[highlight].color = black;
    width = list.width;
    height = list.height;
endgroup

slide test < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abc', 'abC', 'aBc', 'AbC', 'ABc', 'ABC'];
    let simplifiedImplicants = ['ab', 'ac', 'bC', 'Bc', 'AC', 'AB'];
    let container = seperator.vertical(50%);
    let ~table = new QuineMcCluskeyTable(implicants, simplifiedImplicants, [4], [9, 11], [:string;0]);
    let ~steps = new QuineMcCluskeySteps(3);
    container.fill(~steps, ~table);
endslide
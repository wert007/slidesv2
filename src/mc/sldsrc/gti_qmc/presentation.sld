import gfont('Quicksand') as quicksand;
import gfont('Roboto Mono') as mono;

/**********     Notes   ****************
let d = {
    'Hallo Welt': 6,
    'Tüdelü': 2,
};

d.add('lol', 99);
d['not_eZ'] = -5;
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
    e.color = black;
endstyle

group QuineMcCluskeyTable(implicants: string[], simplifiedImplicants: string[], selected: int[], deleted: int[], alreadyFound: string[]):
    let completeFound = [:string;alreadyFound.len() + selected.len()];
    completeFound[..] = alreadyFound[..];
    //TODO
    //for s, i in selected:
    //    completeFound[i + alreadyFound.len()] = simplifiedImplicants[s];
    //endfor
    for i in 0..selected.len():
        completeFound[i + alreadyFound.len()] = simplifiedImplicants[selected[i]];
    endfor
    let resultText = $'__Ergebnis = {join(', ', completeFound)}__';
    if completeFound.len() == 0:
        resultText = ' ';
    endif
    let labelResult = new Label(resultText);
    labelResult.orientation = Horizontal.Left | Vertical.Bottom;

    let table = new Table(implicants.len() + 1, simplifiedImplicants.len() + 1);
    table.orientation = Orientation.Stretch;
    table.margin = margin(0, 0, labelResult.height, 0);
    table.applyStyle(quineMcCluskeyTableStyle);
    table.setRow(implicants, 0, 1);
    table.setColumn(simplifiedImplicants, 0, 1);
    table.cells[0][..].width = 50px;
    table.cells[..][0].height = 40px;
    for x in 0..implicants.len():
        for y in 0..simplifiedImplicants.len():
            let tmp = true;
            let implicant = implicants[x];
            let simpleImplicant = simplifiedImplicants[y];
            for simpleCP in utf32(simpleImplicant):
                tmp &= contains(implicant, utf32(simpleCP));
            endfor
            if tmp:
                table.cells[x + 1][y + 1].content = 'x';
            endif
        endfor
    endfor

    let selectedRects = [:UnitRect?;selected.len()];
    for i in 0..selected.len():
        let s = selected[i];
        let ~left = table.cells[0][s + 1];
        let ~right = table.cells[table.columns - 1][s + 1];
        selectedRects[i] = rect(~left.relativePos(Vertical.Top | Horizontal.Left), ~right.relativePos(Vertical.Bottom | Horizontal.Right));
        selectedRects[i].stroke = orange;
        selectedRects[i].strokeWidth = 2px;
    endfor

    let deletedLines = [:UnitLine?;deleted.len()];
    for i in 0..deleted.len():
        let d = deleted[i] + 1;
        if d <= simplifiedImplicants.len():
            let ~left = table.cells[0][d];
            let ~right = table.cells[table.columns - 1][d];
            deletedLines[i] = line(~left.relativePos(Vertical.Center | Horizontal.Left), ~right.relativePos(Vertical.Center | Horizontal.Right));
            deletedLines[i].stroke = red;
            deletedLines[i].strokeWidth = 2px;
        else
            d -= simplifiedImplicants.len();
            let ~top = table.cells[d][0];
            let ~bottom = table.cells[d][table.rows - 1];
            deletedLines[i] = line(~top.relativePos(Vertical.Top | Horizontal.Center), ~bottom.relativePos(Vertical.Bottom | Horizontal.Center));
            deletedLines[i].stroke = red;
            deletedLines[i].strokeWidth = 2px;
        endif
    endfor
    
    width = table.width;
    height = table.height + labelResult.height;
endgroup

group QuineMcCluskeySteps(highlight: int):
    let list = new List();
    list.add('Erstellen einer Tabelle');
    list.add('Hinzufügen aller essentiellen Implikanten zum Ergebnis');
    list.add('Streichen der dominanten Spalten und dominierten Zeilen');
    list.add('Falls kein Element dem Ergebnis hinzugefügt wurde, wird das Element mit den wenigsten Primimplikanten gewählt');
    list.add(1, 'Streichen der dominanten Spalten und dominierten Zeilen');
    list.add('Wiederholen bis die Tabelle leer ist.');
    list.applyStyle(defaultList);
    list.children[highlight].color = black;
    list.children[highlight].background = rgb(239, 239, 239);
    width = list.width;
    height = list.height;
endgroup

struct PhaseIIData:
    implicants: string[];
    simplifiedImplicants: string[];
    selected = [:int;0];
    deleted = [:int;0];
    alreadyFound = [:string;0];
    highlightedStep: int;
endstruct

group PhaseIIPanel(data: PhaseIIData):
    let container = seperator.vertical(50%);
    let ~table = new QuineMcCluskeyTable(data.implicants, data.simplifiedImplicants, data.selected, data.deleted, data.alreadyFound);
    let ~steps = new QuineMcCluskeySteps(data.highlightedStep);
    container.fill(~steps, ~table);
    width = 100%;
    height = 100%;
endgroup

slide implementationOfPhaseII_0 < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abc', 'abC', 'aBc', 'AbC', 'ABc', 'ABC'];
    let simplifiedImplicants = ['ab', 'ac', 'bC', 'Bc', 'AC', 'AB'];
    let data = new PhaseIIData(implicants, simplifiedImplicants, 0);
    let panel = new PhaseIIPanel(data);
endslide

slide implementationOfPhaseII_1 < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abc', 'abC', 'aBc', 'AbC', 'ABc', 'ABC'];
    let simplifiedImplicants = ['ab', 'ac', 'bC', 'Bc', 'AC', 'AB'];
    let data = new PhaseIIData(implicants, simplifiedImplicants, [4], [9, 11], [:string;0], 3);
    let panel = new PhaseIIPanel(data);
endslide

slide implementationOfPhaseII_2 < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abc', 'abC', 'aBc', 'ABc'];
    let simplifiedImplicants = ['ab', 'ac', 'bC', 'Bc', 'AB'];
    let data = new PhaseIIData(implicants, simplifiedImplicants, [:int;0], [2, 4], ['AC'], 4);
    let panel = new PhaseIIPanel(data);
endslide


slide implementationOfPhaseII_3 < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abc', 'abC', 'aBc', 'ABc'];
    let simplifiedImplicants = ['ab', 'ac', 'Bc'];
    let data = new PhaseIIData(implicants, simplifiedImplicants, [:int;0], [3, 5], ['AC'], 3);
    let panel = new PhaseIIPanel(data);
endslide


slide implementationOfPhaseII_4 < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abC', 'ABc'];
    let simplifiedImplicants = ['ab', 'Bc'];
    let data = new PhaseIIData(implicants, simplifiedImplicants, [0, 1], [:int;0], ['AC'], 2);
    let panel = new PhaseIIPanel(data);
endslide

slide possibleImprovement < withTitle:
    setData('title', 'Verbesserungsmöglichkeiten');
    let list = new List([
        'Zusätzliche Möglichkeit Zahlenwerte bzw Wertetabellen einzulesen',
        'Umfangreicheres Testen',
        'Falls in der Eingabe ein Parameter mit aAc eingegeben wird, wird einfach der letzte Wert genommen, ohne Rücksicht auf Tippfehler'
    ]);
    list.isOrdered = true;
    list.fontsize = 24pt;
endslide

slide showingTheActualProgram:
    let l = new Label('Vorführung des Programms');
    l.fontsize = 48pt;
    l.align = Alignment.Center;
    l.orientation = Orientation.Center;
endslide

slide theEnd < withTitle:
    setData('title', 'Fragen & Quellen');
    let container = seperator.vertical(30%);
    let ~repoLink = new Label('Source Code: github.com/wert007/GTIProject');
    container.fillA(~repoLink);
    let imgSrc = qr.urlQRCode('github.com/wert007/GTIProject', red, white);
    container.fillB(new SVGContainer(imgSrc));
endslide
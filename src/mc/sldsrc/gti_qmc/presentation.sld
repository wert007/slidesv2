import gfont('Quicksand') as quicksand;
import lib('qmclib.sld') as qmcLib;

const sourceCodeMainC = code.loadFile('src_code\\main.c', 'C');
const sourceCodeMainH = code.loadFile('src_code\\main.h', 'C');

style std:
    font = quicksand;
    color = hex('#595959');
    fontsize = 16pt;
    //TODO: Functioncalls in default-style???
    //List.setTextmarker('-');
endstyle

template withTitle(child: Slide):
    let title = child.getData('title') ?? 'No title specified';
    let lblTitle = new Label(title);
    lblTitle.fontsize = 28pt;
    lblTitle.margin = margin(12%, 8%);
    lblTitle.color = black;
    child.padding = margin(12% + (28pt / 1.6f), 8%, 12%, 8%);
endtemplate

slide title:
    let t = new qmcLib.Title('Implementierung des Quine–McCluskey Algorithmus’', 'von Niklas Leukroth & Pablo Hamacher');
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
    list.applyStyle(qmcLib.defaultList);
    list.applyStyle(qmcLib.contentStyle);
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
    list.applyStyle(qmcLib.contentStyle);
    list.applyStyle(1, qmcLib.subListStyle);
endslide

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
    container.fillA(new qmcLib.StructDescription('Felder', contentsBP, contentsBPLvl));

    let lblConclusion = new Label('==> Zum Speichern der ternären Werte');
    lblConclusion.orientation = Horizontal.Left | Vertical.Bottom;
    lblConclusion.applyStyle(qmcLib.contentStyle);

    code.setStyle(CodeHighlighter.Default);
    let ~structCode = code.codeblock(sourceCodeMainH, 7..12);
    let ~cCode = new Captioned(~structCode, 'main.h');
    ~cCode.applyStyle(qmcLib.captionedCode);
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
    stack.add(new qmcLib.StructDescription('Liste', listBulletPoints, [0,0,1,0,1]));
    stack.add(new qmcLib.StructDescription('Node',  nodeBulletPoints, [0,1,0,1]));
    stack.orientation = Orientation.Stretch;
    let ~structCode = code.codeblock(sourceCodeMainH, 20..30);
    let ~cCode = new Captioned(~structCode, 'main.h');
    ~cCode.applyStyle(qmcLib.captionedCode);
    container.fill(~stack, ~cCode);
endslide

slide implementationOfPhaseIOverview < withTitle:
    step _1:
        setData('title', 'Implementierung von Phase I');

        let captionedStructCode = new Container();
        captionedStructCode.fill(new Captioned(code.codeblock(sourceCodeMainC, 111..123), 'main.c'));
        captionedStructCode.applyStyle(qmcLib.captionedCode);

/*

div/captionedStructCode:
    div/child: '' 
    div/child_1: 111..123 
    div/child_2: 125..134 invisible
    div/child_3: 136..151 invisible
    div/child_WrapItUp: 153..169 invisible
    p/caption: 'main.c'

*/

    step _2:
        captionedStructCode.fill(new Captioned(code.codeblock(sourceCodeMainC, 125..134), 'main.c'));
    step _3:
        captionedStructCode.fill(new Captioned(code.codeblock(sourceCodeMainC, 136..151), 'main.c'));
    step _WrapItUp:
        captionedStructCode.fill(new Captioned(code.codeblock(sourceCodeMainC, 153..169), 'main.c'));
endslide
/*
//Replacement above!
slide implementationOfPhaseIOverview1 < withTitle:
    setData('title', 'Implementierung von Phase I');
    let ~structCode = code.codeblock(sourceCodeMainC, 111..123);
    let cCode = new Captioned(~structCode, 'main.c');
    cCode.applyStyle(qmcLib.captionedCode);
endslide

slide implementationOfPhaseIOverview2 < withTitle:
    setData('title', 'Implementierung von Phase I');
    let ~structCode = code.codeblock(sourceCodeMainC, 125..134);
    let cCode = new Captioned(~structCode, 'main.c');
    cCode.applyStyle(qmcLib.captionedCode);
endslide

slide implementationOfPhaseIOverview3 < withTitle:
    setData('title', 'Implementierung von Phase I');
    let ~structCode = code.codeblock(sourceCodeMainC, 136..151);
    let cCode = new Captioned(~structCode, 'main.c');
    cCode.applyStyle(qmcLib.captionedCode);
endslide

slide implementationOfPhaseIWrapItUp < withTitle:
    setData('title', 'Implementierung von Phase I');
    let ~structCode = code.codeblock(sourceCodeMainC, 153..169);
    let cCode = new Captioned(~structCode, 'main.c');
    cCode.applyStyle(qmcLib.captionedCode);
endslide
*/


slide implementationOfPhaseICompare < withTitle:
    setData('title', 'Implementierung von Phase I');
    let container = seperator.vertical(55%);
    let ~structCode = code.codeblock(sourceCodeMainC, 448..479);
    let ~cCode = new Captioned(~structCode, 'main.c');
    ~cCode.applyStyle(qmcLib.captionedCodeSmall);
    ~cCode.captionPlacement = CaptionPlacement.BottomOutwards;
    let ~list = new List([
        'Vergleich eines jeden Elements der aktuellen Liste mit jedem Element der Folge-Liste',
        'Falls Unterscheidungs an nur einer Stelle und "Don\'t-Care"-Werte nur an gleichen Stellen vorkommen --> rufe combine_components() auf',
        'Falls Element nicht vergleichbar ist, füge es der finalen Liste hinzu'
    ]);
    ~list.applyStyle(qmcLib.defaultList);
    ~list.margin.left = 30px;
    container.fill(~cCode, ~list);
endslide

slide implementationOfPhaseICombineComponents < withTitle:
    setData('title', 'Implementierung von Phase I');
    let ~structCode = code.codeblock(sourceCodeMainC, 537..550);
    let cCode = new Captioned(~structCode, 'main.c');
    cCode.applyStyle(qmcLib.captionedCode);
endslide

slide implementationOfPhaseII_0 < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abc', 'abC', 'aBc', 'AbC', 'ABc', 'ABC'];
    let simplifiedImplicants = ['ab', 'ac', 'bC', 'Bc', 'AC', 'AB'];
    let data = new qmcLib.PhaseIIData(implicants, simplifiedImplicants, 0);
    let panel = new qmcLib.PhaseIIPanel(data);
endslide

slide implementationOfPhaseII_1 < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abc', 'abC', 'aBc', 'AbC', 'ABc', 'ABC'];
    let simplifiedImplicants = ['ab', 'ac', 'bC', 'Bc', 'AC', 'AB'];
    let data = new qmcLib.PhaseIIData(implicants, simplifiedImplicants, [4], [9, 11], [:string;0], 3);
    let panel = new qmcLib.PhaseIIPanel(data);
endslide

slide implementationOfPhaseII_2 < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abc', 'abC', 'aBc', 'ABc'];
    let simplifiedImplicants = ['ab', 'ac', 'bC', 'Bc', 'AB'];
    let data = new qmcLib.PhaseIIData(implicants, simplifiedImplicants, [:int;0], [2, 4], ['AC'], 4);
    let panel = new qmcLib.PhaseIIPanel(data);
endslide

slide implementationOfPhaseII_3 < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abc', 'abC', 'aBc', 'ABc'];
    let simplifiedImplicants = ['ab', 'ac', 'Bc'];
    let data = new qmcLib.PhaseIIData(implicants, simplifiedImplicants, [:int;0], [3, 5], ['AC'], 3);
    let panel = new qmcLib.PhaseIIPanel(data);
endslide

slide implementationOfPhaseII_4 < withTitle:
    setData('title', 'Implementierung von Phase II');
    let implicants = ['abC', 'ABc'];
    let simplifiedImplicants = ['ab', 'Bc'];
    let data = new qmcLib.PhaseIIData(implicants, simplifiedImplicants, [0, 1], [:int;0], ['AC'], 2);
    let panel = new qmcLib.PhaseIIPanel(data);
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
    let container = seperator.vertical(40%);
    let ~repoLink = new Label('Source Code: github.com/wert007/GTIProject');
    let imgSrc = qr.urlQRCode('github.com/wert007/GTIProject', red, blue);
    container.fill(~repoLink, new SVGContainer(imgSrc));
endslide
import gfont('Roboto Mono') as mono;

library qmcLib:
    use style;
    use group;
    use struct;
endlibrary

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

style quineMcCluskeyTableStyle(e: Table):
    e.fontsize = 9pt;
    e.align = Alignment.Center;
    e.font = mono;
    e.color = black;
endstyle

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

group StructDescription(name: string, bulletPoints: string[], levels: int[]):
    let lblName = new Label($'{name}:');
    lblName.applyStyle(contentStyle);
    let list = new List();
    list.margin.top = lblName.bottomSide + 15px;
    list.applyStyle(defaultList);
    list.applyStyle(contentStyle);
    list.applyStyle(1, subListStyleNoMarker);
    for bulletPoint, i in bulletPoints:
        list.add(levels.getSafe(i) ?? 0, bulletPoint);
    endfor

    width = max(lblName.width, list.width);
    height = lblName.height + list.height + 15px;
endgroup

group QuineMcCluskeyTable(implicants: string[], simplifiedImplicants: string[], selected: int[], deleted: int[], alreadyFound: string[]):
    let completeFound = [:string;alreadyFound.len() + selected.len()];
    completeFound[..] = alreadyFound[..];
    for s, i in selected:
        completeFound[i + alreadyFound.len()] = simplifiedImplicants[s];
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
    for implicant, x in implicants:
        for simpleImplicant, y in simplifiedImplicants:
            let tmp = true;
            for simpleCP in utf32(simpleImplicant):
                tmp &= contains(implicant, utf32(simpleCP));
            endfor
            if tmp:
                table.cells[x + 1][y + 1].content = 'x';
            endif
        endfor
    endfor
    table.cells[0][..].width = 50px;
    table.cells[..][0].height = 40px;

    let selectedRects = [:UnitRect?;selected.len()];
    for s, i in selected:
        let ~left = table.cells[0][s + 1];
        let ~right = table.cells[table.columns - 1][s + 1];
        selectedRects[i] = rect(~left.relativePos(Vertical.Top | Horizontal.Left), ~right.relativePos(Vertical.Bottom | Horizontal.Right));
        selectedRects[i].stroke = orange;
        selectedRects[i].strokeWidth = 2px;
    endfor

    let deletedLines = [:UnitLine?;deleted.len()];
    for d, i in deleted:
        if d < simplifiedImplicants.len():
            let ~left = table.cells[0][d + 1];
            let ~right = table.cells[table.columns - 1][d + 1];
            deletedLines[i] = line(~left.relativePos(Vertical.Center | Horizontal.Left), ~right.relativePos(Vertical.Center | Horizontal.Right));
            deletedLines[i].stroke = red;
            deletedLines[i].strokeWidth = 2px;
        else
            d -= simplifiedImplicants.len();
            let ~top = table.cells[d + 1][0];
            let ~bottom = table.cells[d + 1][table.rows - 1];
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
    let ~subList = cast list.children[highlight]: List;
    if ~subList:
        for ~c in ~subList.children:
            ~c.color = black;
            ~c.background = rgb(239, 239, 239);
        endfor
    else
        list.children[highlight].color = black;
        list.children[highlight].background = rgb(239, 239, 239);
    endif

    if cast list.children[highlight]: List:
        for ~c in cast list.children[highlight]: List.children:
            ~c.color = black;
            ~c.background = rgb(239, 239, 239);
        endfor
    else
        list.children[highlight].color = black;
        list.children[highlight].background = rgb(239, 239, 239);
    endif

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

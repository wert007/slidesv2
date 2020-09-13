import font('Arial') as arial;

style std:
    font = arial;
    Slide.background = black;
    Slide.padding = padding(6%, 4%);
    color = hex('#adadad');
    List.fontsize = 18pt;
//    List.1.fontsize = 14pt;
    Captioned.background = hex('#303030');
    Captioned.caption.align = Alignment.Center;
    Captioned.caption.padding = padding(15px);
    Captioned.caption.fontsize = 13pt;
endstyle

group Titled(title: string, child: Element):
    let lblTitle = new Label(title);
    lblTitle.align = Alignment.Center;
    lblTitle.color = white;
    lblTitle.fontsize = 28pt;
    lblTitle.orientation = Vertical.Top | Horizontal.Stretch;
    lblTitle.padding = padding(15px);
    let _child = child;
    child.orientation = Orientation.Stretch;
    child.margin.top = lblTitle.bottomSide;
    width = 100%;
    height = 100%;
endgroup

group ListWithCaptioned(bulletPoints: string[], captioned : Captioned):
    let container = seperator.vertical(70%);
    let ~list = new List(bulletPoints);
    ~list.fontsize = 18pt;
    container.fill(~list, captioned);
    captioned.orientation = Horizontal.Stretch | Vertical.Top;
    width = 100%;
    height = container.height;
endgroup

slide introduction:
    let labelErrors = new Label('[*]list item zero [LiSt=I][*]list item one[*]list item two[/lIsT]');
    let stampSrc = image('gfx\\stamp.jpg');
    let ~stamp = new Image(stampSrc);
    let captionedStamp = new Captioned(~stamp, 'Russische Briefmarke | liveinternet.ru'); 
    captionedStamp.caption.padding = padding(15px);
    captionedStamp.orientation = Vertical.Center | Horizontal.Center;
    captionedStamp.height = 60%;
    captionedStamp.margin = margin(2%);
endslide

slide title:
    let main = new Label('Die Transsibirische Eisenbahn');
    main.orientation = Horizontal.Stretch | Vertical.Bottom;
    main.align = Alignment.Center;
    main.margin.bottom = 50%;
    main.color = white;
    main.fontsize = 52pt;
    let sub = new Label('100 Jahre Transsib');
    sub.orientation = Horizontal.Center | Vertical.Top;
    sub.align = Alignment.Center;
    sub.margin.top = 50%;
    sub.fontsize = 36pt;
endslide

slide factsAndInformations:
    let bulletPoints = [
        '[i]Moskau[/i] nach [i]Wladiwostog[/i]',
        '160 Stunden durch acht Zeitzonen',
        '9298km Wegstrecke',
        '90 000 Arbeiter waren beschäftigt',
        'entlang der Strecke 89 Städte, u.a. fünf Millionenstädte',
        'überquert 16 große Flüsse (u.a. [i]Ob[/i] und [i]Amur[/i])',
        'längste Brücke 3,9 km',
        'kältester Abschnitt: -62°C',
        'dieses Jahr (2016) 100-jähriges Jubiläum',
    ];
    let ~coin = new Image(image('gfx\\coin.png'));
    let ~captioned = new Captioned(~coin, 'commons.wikimedia.org');
    let ~child = new ListWithCaptioned(bulletPoints, ~captioned);
    let content = new Titled('Fakten und Informationen', ~child);
endslide

/*
style textHoverStyle(text: FormattedString):
    text.color = red;
endstyle
*/
slide route:
    //let lbl = new Label('[quote="C Programming Language"]Hello, [size=10pt]World![/size][/quote]');
    //let lbl = new Label('[b][i]Hello World[/b][/i]!');
    // let lbl = new Label('[b][g]Hello World[/b]!');
    // let lbl = new Label('[b]Hello World[/g][/b]!');
    // let lbl = new Label('[b][g]Hello World[/g][/b]!');
    //lbl = new Label('[b,i]Hello[/b] [color=red,size=20px]world[/color,size], lol');
    //'[b,g ]lol[/b] joo' -> '[b,g ]lol[/g] joo'
    //'[b,g]lol[/b] joo' -> '[g]lol[/g] joo'
    //'[b,g]lol[/b] joo' -> 'lol joo'

    //lbl = new Label('**__Hello__** [color=red,size=20px]world[/color], lol');
    //let lbl = new Label('Hello [marker=insertion_marker, hover=textHoverStyle]world[/marker], lol');
   /* 
    jsinsertion:
        let subText = lbl.text.getMarker('insertion_marker') ?? new Label('We had an error!');
        subText.color = hsl(mod(totalTime / 100, 360), 255, 127);
    endjsinsertion
*/

    let ~map = new Image(image('gfx/transsib-map.gif'));
    let ~child = new Captioned(~map, 'Quelle: [color=#009688][url]http://www.info-transsibirische-eisenbahn.de/wp-content/uploads/sites/8/2017/05/transsib-map.gif[/url][/color]');
    let content = new Titled('Verlauf', ~child);
    ~child.orientation = Horizontal.Center | Vertical.Stretch;
    ~child.width = 65%;
endslide

style shittyJavalikeProgramming(e: TextElement):
    e.fontsize = 14pt;
endstyle

slide history:
    let ~child = new List();
    ~child.add(0, 'Planungsbeginn 1870er unter [i]Zar Alexander II.[/i]');
    ~child.add(0, 'Bau von 1891 bis 1916');
    ~child.add(1, 'Grundsteinlegung am 19. Mai 1891 in [i]Wladiwostok[/i] von [i]Zar Nicholas II.[/i]');
    ~child.add(1, 'die [i]Westsibirienstrecke[/i] wurde 5½ Jahre später fertig');
    ~child.add(1, 'die [i]Ussuri-Bahn[/i] wurde 5 Jahre später eröffnet');
    ~child.add(1, 'die [i]Mittelsibirienstrecke[/i] wurde 8 Jahre nach Baubeginn fertig');
    ~child.add(1, 'die [i]Transbaikalstrecke[/i] wurde 9 Jahre später fertig');
    ~child.add(1, 'die [i]Baikalstrecke[/i] war 9 Jahre später fertig');
    ~child.add(2, 'war 2 Jahre lang eine Fährverbindung über [i]Baikalsee[/i]');
    ~child.add(2, 'danach (1904) wurde eine Umgehungsstrecke betrieben');
    ~child.add(1, 'ganze 25 Jahre nach der Grundsteinlegung ging die [i]Amurstrecke[/i] in Betrieb');
    ~child.add(0, 'durchgehende Elektrifizierung nach 74 Jahren am 25. Dezember 2002 abgeschlossen');
    ~child.fontsize = 18pt;
    ~child.applyStyle(1, shittyJavalikeProgramming);
    let content = new Titled('Baugeschichte', ~child);
endslide

slide reasons:
    let ~child = new List([
        'bessere Transportmöglichkeiten',
        'Anbindung an den Weltmarkt',
        'militärische Nutzung',
        'Freiräume für die expandierende Industrie',
        'Umsiedlung großer Menschenmassen',
    ]);
    let content = new Titled('Gründe des Baus', ~child);
endslide

slide problems:
    let ~child = new List();
    ~child.add('Allgemein:');
    let ~general = new List([
        'Erdbebengefährdete Gebiete',
        'oftmals nötige Sprengungen',
        'gefährliche Erdrutsche',
        'Seuchen',
    ]);
    ~child.add(~general);
    ~child.add('Besondere Fälle:');
    let ~special = new List();
    ~special.add('gebirgige Strecke um den [i]Baikalsee[/i] ==> [b]Fährverbindung über [i]Baikalsee[/i][/b]');
    ~special.add(1, 'später komplizierte Umleitungsstrecke');
    ~special.add('zwischen [i]Baikalsee[/i] und [i]Chabrowsk[/i]: sehr kalte Winter, Permafrostboden und häufige Überschwemmung ==> [b]Verlauf durch [i]Mandschurei[/i][/b]');
    ~special.add('verlorener Krieg gegen Japan ==> [b]Änderung der Strecke [i](Amurstrecke)[/i][/b]');
    ~child.add(~special);
    let content = new Titled('Probleme des Baus', ~child);
endslide

slide climate:
    let bulletPoints = [
        'teilweise - 40°C während den Bauarbeiten [b]ohne[/b] feste Unterkunft',
        'Permafrostboden im Bereich der [i]Amur-Strecke[/i]',
        'leichtere und biegbarere Schienen als üblich wegen Permafrostboden',
    ];
    let map = new Image(image('gfx\\diercke_page148.png'));
    map.stretching = ImageStretching.Cover;
    let ~innerCaptioned = new Captioned(~map, 'Diercke Atlas Seite 148 | Abb. 1');
    ~innerCaptioned.borderColor.bottom = black;
    ~innerCaptioned.borderWidth.bottom = 5px;
    ~innerCaptioned.borderStyle.bottom = BorderStyle.Solid;
    ~innerCaptioned.caption.align = Alignment.Left;
    let ~captioned = new Captioned(~innerCaptioned, 'Legende (Durchschnittstemperaturen): [list][*]- 45°C bei Werchojansk [*]- 10°C bei Moskau[/list]');
    ~captioned.caption.align = Alignment.Left;
    let ~child = new ListWithCaptioned(bulletPoints, ~captioned);
    let content = new Titled('Klima', ~child);
endslide
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
    useDarkTheme = true;
    // OverlayUI.Searchform.highlightColor = hex('#78909c');
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

group Subtitled(subtitle: string, child: Element):
    let lblSubtitle = new Label(subtitle);
    lblSubtitle.align = Alignment.Center;
    lblSubtitle.color = hex('#78909c');
    lblSubtitle.fontsize = 12pt;
    lblSubtitle.orientation = Vertical.Top | Horizontal.Stretch;
    // lblSubtitle.padding = padding(15px);
    let _child = child;
    child.orientation = Orientation.Stretch;
    child.margin.top = lblSubtitle.relativePos(Orientation.Center).y;
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
    // let labelErrors = new Label('[*]ignore this please[/*]');
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

style someStyleYo(e: any):
    e.filter = grayscale(0.5f);
endstyle

slide history:
    let ~child = new List([
        'Planungsbeginn 1870er unter [i]Zar Alexander II.[/i]',
        'Bau von 1891 bis 1916',
        ' Grundsteinlegung am 19. Mai 1891 in [i]Wladiwostok[/i] von [i]Zar Nicholas II.[/i]',
        ' die [i]Westsibirienstrecke[/i] wurde 5½ Jahre später fertig',
        ' die [i]Ussuri-Bahn[/i] wurde 5 Jahre später eröffnet',
        ' die [i]Mittelsibirienstrecke[/i] wurde 8 Jahre nach Baubeginn fertig',
        ' die [i]Transbaikalstrecke[/i] wurde 9 Jahre später fertig',
        ' die [i]Baikalstrecke[/i] war 9 Jahre später fertig',
        '  war 2 Jahre lang eine Fährverbindung über [i]Baikalsee[/i]',
        '  danach (1904) wurde eine Umgehungsstrecke betrieben',
        ' ganze 25 Jahre nach der Grundsteinlegung ging die [i]Amurstrecke[/i] in Betrieb',
        'durchgehende Elektrifizierung nach 74 Jahren am 25. Dezember 2002 abgeschlossen',
    ]);
    ~child.fontsize = 18pt;
    ~child.styling(1).fontsize = 14pt;
    // ~child.styling(1).applyStyle(someStyleYo);
    let content = new Titled('Baugeschichte', ~child);
    content.styling.applyStyle(someStyleYo); // == content.applyStyle(someStyleYo);
    // content.hover.applyStyle(someStyleYo);
    // content.hover.background = red;
    // content.styling.hover.applyStyle(someStyleYo);
    // content.styling.hover.background = red;
    
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

// def lol gray
// define climateContent!(%e%: Element)
//     %e%.background = lol!;
//     let ~child = new ListWithCaptioned(bulletPoints, %e%);
//     let content = new Titled('Klima', ~child);
// end

// macro
// for i in 0..5:
//     let s = new Slide();
//     let step = s.createStep();
//     step.add(new Label($'Hello World No. {i}'));
// endfor
// macroend

const climateBulletPoints = [
    'teilweise - 40°C während den Bauarbeiten [b]ohne[/b] feste Unterkunft',
    'Permafrostboden im Bereich der [i]Amur-Strecke[/i]',
    'leichtere und biegbarere Schienen als üblich wegen Permafrostboden',
];

slide climate:
    let mapSrc = crop(image('gfx\\diercke_page148.png'), 0, 0, 50%, 0);
    let ~innerCaptioned = new Captioned(new Image(mapSrc), 'Diercke Atlas Seite 148 | Abb. 1');
    ~innerCaptioned.border.bottom = border(5px, BorderStyle.Solid, black);
    ~innerCaptioned.caption.align = Alignment.Left;
    let ~captioned = new Captioned(~innerCaptioned, 'Legende (Durchschnittstemperaturen): [list][*]- 45°C bei Werchojansk [*]- 10°C bei Moskau[/list]');
    ~captioned.caption.align = Alignment.Left;
    let ~child = new ListWithCaptioned(climateBulletPoints, ~captioned);
    let content = new Titled('Klima', ~child);
endslide

slide climate2:
    let ~captioned = new Captioned(new Image(image('gfx\\train_stop.jpg')), '-40°C in Mogotscha');
    ~captioned.caption.align = Alignment.Right;
    let ~child = new ListWithCaptioned(climateBulletPoints, ~captioned);
    let content = new Titled('Klima', ~child);
endslide

slide finances:
    let ~child = new List([
        'Veranschlagt: 330 Millionen Rubel',
        'Tatsächlich: ca. 1,17 Milliarden Rubel',
        'Haushaltsüberschüsse',
        'Schuldenaufnahme bei anderen Ländern (insbesondere [i]Frankreich[/i] und [i]Belgien[/i])',
    ]);
    let content = new Titled('Finanzierung', ~child);
endslide

slide industries:
    let ~captioned = new Captioned(
        //new Image(image('gfx/natural_resources_map.png')),
        new Image(image('gfx/diercke_page148.png')),
        'Diercke Atlas Seite 154/155 | Abb. 1');
    
    let bulletPoints = [
        '2,3 Millionen Arbeitsplätze',
        '5 mal so viele Tonnen pro km Fracht im Vergleich zu [i]USA[/i]',
        'Florieren des Eisen- und Stahlmarktes',
        'Industriestandpunkte nicht mehr an Flüsse gebunden',
        'Zugang zu und Transport von Bodenschätzen',
        'Umsiedeln der Menschen entlang der Bahnlinie',
    ];
    let ~child = new ListWithCaptioned(bulletPoints, ~captioned);
    let content = new Titled('Wirtschaftlich', ~child);
endslide

slide exampleTravelPlan:
    // This doesn't work, because we can only set those variables as a whole
    // and not fields of them..

    // padding.left = 10%;
    // padding.right = 10%;
    padding = padding(6%, 10%);
    // println(name);
    let ~container = seperator.vertical(70%);
    let ~left = new List([
        'Visa',
        ' Russland 30 Tage zweimalige Einreise',
        ' Touristenvisum Mongolei einmalig ein- und ausreisen',
        'Zugtickets',
        ' Moskau - Irkutsk (2. Klasse)',
        ' Irkutsk - Ulaanbataar (Mongolei) (2. Klasse)',
        ' Ulaanbataar (Mongolei) - Ulan Ude (Busfahrt)',
        ' Ulan Ude - Chabarowsk (3. Klasse)',
        ' Chabarowsk - Wladiwostog (2. Klasse)',
        'Übernachtung',
        ' durchschnittlich pro Nacht',
        'Flüge',
        ' Nürnberg - Moskau',
        ' Wladiwostog - Moskau',
        ' Moskau - München',
        '[b]Summe[/b]',
    ]);
    ~left.fontsize = 15pt;
    ~left.lineHeight = 1.25f;
    ~left.styling(1).fontsize = 13pt;
    let ~right = new List([
        '[b]Kosten[/b]',
        ' 91€',
        ' 80€',
        '-',
        ' 345€',
        ' 155€',
        ' 25€',
        ' 95€',
        ' 95€',
        '-',
        ' ~13€',
        '-',
        ' 60€',
        ' 305€',
        ' 165€',
        '[b]1\'429€[/b]',
    ]);
    right.markerType = ListMarkerType.None;
    right.fontsize = 15pt;
    right.lineHeight = 1.25f;
    right.align = Alignment.Right;
    right.styling(1).fontsize = 13pt;
    ~container.fill(~left, ~right);
    let ~child = new Subtitled('für eine Reise von Nürnberg nach Wladiwostog durch die Mongolei und wieder zurück nach München', ~container);
    let content = new Titled('Kosten', ~child);
endslide

slide sources:
    let ~child = new List([
        '[color=#009688][url]www.info-transsibirische-eisenbahn.de[/url][/color]',
        '[color=#009688][url]www.transsib.de[/url][/color]',
        '[color=#009688][url]www.planet-wissen.de/technik/verkehr/geschichte_der_eisenbahn/pwietranssibirischeeisenbahn100.html[/url][/color]',
        '[color=#009688][url]www.zdf.de/terra-x/die-transsibirische-eisenbahn-25461884.html[/url][/color]',
        '[color=#009688][url]commons.wikimedia.org[/url][/color]',
        '[color=#009688][url]http://transsib.eyand.de/beginn_der_bauarbeiten.html[/url][/color]',
    ]);
    child.lineHeight = 2;
    child.fontsize = 18pt;
    child.markerType = ListMarkerType.None;
    let content = new Titled('Quellen', child);
endslide

slide questions:
    padding = padding(6%, 0, 15%, 0);
    let ~child = new Image(image('gfx\\questions.jpg'));
    child.filter = invert(1);
    child.stretching = ImageStretching.Cover;
    let content = new Titled('Fragen', child);
endslide
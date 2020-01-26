import lib('lib1.sld') as basics;
import lib('lib2.sld') as custom;
import gfont('Quicksand') as quicksand;

//TODO
//import css('myStyle.css');

//Possible Features:
// - Binder needs to warn when a empty style is found
// - Seperator Library needs a SplittedContainer Type.
// - offline compile flag (--offline). With warnings when using youtube() or such!
// - support github code in javascript!
// - Support textboxes maybe? idk why but maybe.

template pagenumber(child: Slide):
	let text~ = $'{child.index + 1}/{slideCount}';
	let label = new Label(text~);
	label.orientation = Horizontal.Right | Vertical.Top;
	label.fontsize = 12pt;
	label.margin = margin(10px);

	let progress~ = float(child.index) / float(slideCount - 1);
	let rect = new Rectangle(progress~, 5px);
	rect.fill = rgb(int(255f * progress~), 0, int(255f * (1f - progress~)));
	rect.orientation = Vertical.Bottom | Horizontal.Left;
endtemplate

style std:
	Slide.background = white;
	
	color = rgb(23, 23, 23);
	font = quicksand~;
	fontsize = 14pt;
	//transition = stdTransition;
endstyle

transition stdTransition(from: Slide, to: Slide):
	background = black;
	duration = 200ms;
	from.hide(duration);
	to.fadeIn(0ms, duration);
endtransition

filter discrete(source: FilterInput):
	let blurred~ = blur(source, 1.5f);
	let red~ = discreteNode([0f, 0.5f, 1f, 1f]);
	let green~ = discreteNode([0f, 0.5f, 1f]);
	let blue~ = discreteNode([0f]);
	let alpha~ = identityNode();
	let transferred~ = transfer(blurred~, red~, blue~, green~, alpha~);
endfilter

animation quoteGoesUp(element: any, duration~: Time):
	case init:
		interpolation = Interpolation.Linear;
		element.background = alpha(white, 0.7f);
	case done:
		//element.margin = margin(-100%, 0, 0, 0);
		//TODO(hacky): Doesn't work. Turns black instead.
		//needs a initial value. otherwise it won't 
		//turn. So maybe say default color is transparent?
		element.background = red;
endanimation

animation unblur(element: any, duration~: Time):
	case init:
		interpolation = Interpolation.Linear;
		element.filter = blur(5);
	case done:
		//Because of the cases progress always has a specific value and you cant use it
		//for calculations. You would need a special case for this...
		//Something like:
		//
		//		case compute:
		//			blur(exp(progress));
		//
		//But do we need this? We can set the interpolation and say what value we want when.
		//idk. 
		element.filter = blur(0);
endanimation

slide ~destroySlideCount:
	//TODO: This slide shouldn't be counted.
	//Neither in slideCount nor in slide.index!
endslide

slide mathTwo:
	let sld = new Slider(1..7);
	sld.orientation = Horizontal.Center | Vertical.Top;
	let f~ = #math 'b * x^2 + a * c';
	//TODO: Less keywords. Use this one instead.
	//let f~ = new MathExpression('b * x^2 + a * c');

	f~.a = 2;
	f~.b = sld.value;
	f~.c = 2 * sld.value;

	//TODO!
	//let expression = new MathExpression(f~);
	
	let plot = new MathPlot(f~, -12..13);
	plot.orientation = Orientation.Stretch;
	plot.margin = margin(sld.bottomSide, 5%, 5%, 5%);
endslide

slide a < pagenumber:
	let lbl = new Label('Hello World!');
	lbl.color = black;
	let sld = new Slider(1..255);
	lbl.fontsize = 90pt;
	//TODO it does work, but because we set top and margin at
	//different locations and this formula always just sets
	//the margin, there is confusion.
	//	lbl.margin = margin(pct(sld.value * 10), 0, 0, 0);
	lbl.color = hsl(sld.value, 100, 50);
	lbl.orientation = Vertical.Top | Horizontal.Stretch;
	lbl.align = Alignment.Center;

endslide

slide cityDevelopment < pagenumber:
	let titleText~ = 'Geschichte der Stadtentwicklung';
	let contents~ = [
		['1850 erhielt L.A. das US-Stadtrecht'],
		['Um 1900 bereits 100.000 Einwohner'],
		['Steigende Bevölkerungszahlen aufgrund folgender Faktoren'],
		[
			'wachsende Infrastruktur',
			'Rohstoffabbau',
			'Eingemeindung von Nachbarstädten',
			'Filmindustrie'
		]
	];
	
	let args~ = new custom.cityDevelopmentParameter();
	args~.header~ = titleText~;
	args~.contents~ = contents~;
	args~.populationFile~ = @'somefile.csv';
	args~.populationSource~ = '(c) surely wikipedia';

	//Can you store a Tuple in a single variable, or do you need to
	//tear it down to its child types?
	//
	//	let tuple = seperator.vertical(50%);
	// tuple[0], tuple[1]
	//
	//I'd say no. Because why would you?
	//
	//Well maybe it would make to use it in a FunctionCall
	//so you can just call it inline and don't need to store the
	//tuple in an extra variable.
	//
	//	foo(5, seperator.vertical(50%));
	//
	//				vs
	//
	//	let left, right = seperator.vertical(50%);
	//	foo(5, left, right);
	//
	//def:
	//	func foo(a: int, (left: Container, right: Container))
	//
	//				 vs
	//
	//	func foo(a: int, left: Container, right: Container)

		
	let left, right = seperator.vertical(20%);
	left.fill(new custom.imageBanner());
	let ~text = new custom.cityDevelopmentText(args~);
	~text.height = 100%;
	right.fill(~text);

	right.padding = padding(5%);
endslide

slide introduction:
	background = image(@'city\los-angeles-picture.jpg');
	let losAngeles = new Label('Los Angeles');
	losAngeles.applyStyle(custom.imgLabel);

	//TODO: This gets detected.
	let unused~ = 'Unused';
	//This not:
	let actuallyUnusedAsWell~ = 5;
	actuallyUnusedAsWell~ = 42;


	let source = new Label('(c) pixabay');
	source.applyStyle(custom.imgLabel);
	source.orientation = Horizontal.Right | Vertical.Top;

	let quote~ = 'Los Angeles is 72 suburbs in search of a city';
	let author~ = 'Dorothy Parker';
	let quoteBox = new custom.introductingQuote(quote~, author~);

	step:
		quoteGoesUp.play(quoteBox, 10s);
endslide

slide title:
	let main~ = 'Angloamerikanische Stadt';
	let sub~ = 'Am Beispiel von Los Angeles';
	let title = new basics.MainTitle(main~, sub~);
	title.orientation = Vertical.Center | Horizontal.Stretch;
	title.background = alpha(antiquewhite, 0.45f);
	background = image(@'city\night.jpg');
endslide

slide traits < pagenumber:
	let titleText~ = 'Merkmale einer anglo-amerikanischen Stadt';
	let contents~ = [
		'Schachbrettmuster des Straßenverlaufs',
		'Wolkenkratzer im Geschäftszentrum',
		'klassische Strukturierung der Stadt in __CBD__, __Übergangszone__ und __Außenbereich__',
		'__Commercial Strips__ entlang von Verkehrsachsen',
		'allgemeine Wohnform der gehobeneren Schichten in Vororten',
		'zunehmende Entstehung von __Gated Communties__', 
		'verfallende Kernstädte und Ghettobildung'
	];
	let title = new basics.Title(titleText~);
	let list = new List(contents~);
	list.fontsize = 24pt;
	list.isOrdered = true;
	list.margin.top = title.bottomSide;
	padding = padding(5%);
endslide

slide city < pagenumber:
	let titleText~ = 'Allgemeines zu Los Angeles';
	let title = new basics.Title(titleText~);

	let logo~ = image(@'city\logo.png');
	let imgLogo = new Image(logo~);
	imgLogo.height = title.height;
	imgLogo.width = imgLogo.height;

	title.margin = margin(0, 0, 0, imgLogo.rightSide);
	
	let captionLogo~ = '(c) City of Los Angeles';
	let imgLogoCaption = new Label(captionLogo~);
	imgLogoCaption.orientation = Horizontal.Left | Vertical.Top;
	imgLogoCaption.margin = margin(title.bottomSide, 0, 0, 0);
	imgLogoCaption.fontsize = 8pt;

	let map = new custom.map();
	map.width = 50%;
	map.orientation = Vertical.Top | Horizontal.Right;
	map.margin = margin(title.bottomSide, 0, 0, 0);

	let contents~ = [
		'Zweitgrößte Stadt der USA',
		'3,8 Mio Einwohner innerhalb der Stadtgrenzen',
		'12,8 Mio Einwohner im Ballungsraum L.A.',
		'in 15 Bezirke (Districts) aufgegliedert',
		'wird aufgrund flächenhafter Ausdehnung \nauch "horizontal city" genannt'
	];
	let list = new List(contents~);
	list.fontsize = 24pt;
	list.margin = margin(imgLogoCaption.bottomSide, 0, 0, 0);
	padding = padding(5%);
endslide

slide overview:
	let la~ = image(@'city\los-angeles-picture.jpg');
	let imgBackground = new Image(la~);
	imgBackground.filter = blur(5);
	imgBackground.orientation = Orientation.Stretch;
	imgBackground.stretching = ImageStretching.Cover;
	let whitePane = new Rectangle(100%, 100%);
	whitePane.fill = argb(160, 255, 255, 255);

	let map~ = image(@'city\map.png'); //TODO: Change image
	let imgMap = new Image(map~);
	imgMap.orientation = Horizontal.Center | Vertical.Center;
	imgMap.width = 50%;
	imgMap.height = auto;

	//TODO: This feature would be like super cool.
	step:
		//filter = grayscale(1);

		let svgSrc~ = svg(@'city\overlay.svg');
		let imgSvg = new Image(svgSrc~);
endslide



























//
//slide github < pagenumber:
//	let sldEnd = new Slider(5..50);
//	let sldStart = new Slider(1..5);
//	sldStart.margin = margin(50px, 0, 0, 0);
//	sldStart.max = sldEnd.value;
//	sldEnd.min = sldStart.value;
//
//	code.setStyle(CodeHighlighter.Funky);
//	let repository~ = code.github('wert007/GTIProject');
//	let codeBlockB = code.codeblock(repository~, 'main.c', 3..14);
//	codeBlockB.fontsize = 10pt;
//	codeBlockB.orientation = Horizontal.Center | Vertical.Center;
//	codeBlockB.margin = margin(0, 50%, 0, 0);
//	codeBlockB.range = sldStart.value..sldEnd.value;
//	let slider = new Slider(0..1000);
//	slider.orientation = Horizontal.Stretch | Vertical.Top;
//	let l = new Label('value not found');
//	l.margin = margin(25px, 0, 0, 0);
//	l.text = $'value: {slider.value}';
////	step lol:
////		let vid = youtube('VB4CCHHYOqY', true);
////		vid.orientation = Orientation.Stretch;
////		vid.filter = discrete~;
////		vid.margin = margin(0, 0, 0, 50%);
//endslide

import lib('lib1.sld') as basics;
import lib('lib2.sld') as custom;
import gfont('Quicksand') as quicksand;

//TODO
// - Support custom css files included during compiletime.
//		import css('myStyle.css');

//Cleanup:
// - Move filters into the SVG project.
// - JSInsertions:
//    - Multiple Sliders on one page => unique function names for each slide
//    - Overthink use statement. Should we really specify what we need? 
//					+								-
//       You know exactly which slider		you don't really need it for time..
//		 is meant.
//
//    - Would it be enough to just specify the slider and not slider.value? (yes,
//		but is it understandable, that that is just about slider.value even though
//      you use the whole slider in the use statement?)
// - Rename Data to struct
// - Add all functions in js
// - Add all Element fields in js/css


//Possible Features:
// - make arrays safe. What do we do if we have an IndexOutOfBoundsException?
//   Should we throw a runtime exception? **Should we try our best to don't do that
//   and only sometimes throw a runtime exception?** Should we treat runtime exceptions
//   like compile time errors, because they are so similiar?
//   PRO: Right now our program is deterministic. There is no random nor user input
//   BUT: Once we have randomness we would need to make sure, that no matter what random value
//        We would not have an Exception/Error.
// - multitype indeces?
//   data['title'] = 'lol';
// - Binder needs to warn when a empty style is found. He does now. At least if it is 
//   literally empty. Once you have anything in it, it doesn't warn you. Because you would
//   need to compute the style which we only do during evaluation.
// - Virtual Machine. Mostly to serialize Libraries. And because it is interesting
// - Advanced SVGSupport?
//		- SVGParser. Incomplete but working. Needs testing
//
//		- PathOperations. (Intersection/Union/Divide)
//		  As good as it's going to be.  
//				
//				let a = arrow(Direction.North, 100, 100, 0.5f, 0.5f);
//				let b = arrow(Direction.South, 100, 100, 0.5f, 0.5f);
//
//		  This doesn't work, because we don't know which lines are important and which not
//		  Right now only LineTo and their kind are implemented.


//
// - support github code in javascript! just important, if you don't want to rebuild
//   your slide, when you update your code. But for now i guess it's fine. So BIG MAYBE
// - Support textboxes maybe? idk why but maybe.
//   Probably not. Because when would you need it? And how would you make it work in sld?
//   Just use a iframe if you need to. then you can use clean javascript.


//toTime(5) = "5ms";
//toTime(10000) = "10s";
//toTime(10050) = "10s 50ms"


/// Displays the total time
template stressMe(child: Slide):
	let label = new Label('no time defined');
	label.orientation = Horizontal.Left | Vertical.Top;
	label.fontsize = 12pt;
	label.margin = margin(10px);
	
	//Throws compiletime error! 
	//Only inside use block!
	//let a = totalTime; 

	use totalTime:
		label.text = toTime(totalTime);
		if totalTime > 5000:
			child.background = red;
		endif
		//child.background = hsl(mod(totalTime / 40, 360), 40, 90);
	enduse
endtemplate

template pagenumber(child: Slide):
	let name = child.name;
	if child.getData('name'):
		name = child.getData('name');
	endif
	let text = $'{name}: {fixedWidth(child.index + 1, 3)}/{fixedWidth(slideCount, 3)}';
	let label = new Label(text);
	label.orientation = Horizontal.Right | Vertical.Top;
	label.fontsize = 12pt;
	label.margin = margin(10px);

	let progress = child.index / float(slideCount - 1);
	//let rect = rect(pct(progress * 100), 10px);
	//rect.fill = rgb(int(255 * progress), 0, int(255 * (1 - progress)));
	//rect.orientation = Vertical.Bottom | Horizontal.Left;
endtemplate

style std:
	Slide.background = white;
	
	color = rgb(51, 51, 51);
	font = quicksand;
	fontsize = 14pt;
	//transition = stdTransition;
endstyle

transition stdTransition(from: Slide, to: Slide):
	background = white;
	duration = 200ms;
	from.hide(duration);
	to.fadeIn(0ms, duration);
endtransition

filter discrete(source: FilterInput):
	let blurred = blur(source, 1.5f);
	let red = discreteNode([0f, 0.5f, 1f, 1f]);
	let green = discreteNode([0f, 0.5f, 1f]);
	let blue = discreteNode([0f]);
	let alpha = identityNode();
	let transferred = transfer(blurred, red, blue, green, alpha);
endfilter

animation quoteGoesUp(element: any, duration: Time):
	case init:
		//interpolation = Interpolation.Linear;
		//It's a bug on the js side. I'm not to sure why...
		//Maybe because of missing subscribe/broadcast functions
		//in the js Color class
		element.background = argb(127, 236, 236, 236);
	case done:
		element.background = red;
endanimation

animation unblur(element: any, duration: Time):
	case init:
		//interpolation = Interpolation.Linear;
		//element.filter = blur(5);
	case done:
		//Because of the cases progress always has a specific value and you cant use it
		//for calculations. You would need a special case for this...
		//Something like:
		//
		//		case compute:
		//			blur(progress * 0.47537283f);
		//
		//But do we need this? We can set the interpolation and say what value we want when.
		//idk. 
		//element.filter = blur(0);
endanimation

//TODO: Introduce test slide
svg test(operation: int):
	let a = arrow(Direction.North, 100, 100, 0.5f, 0.5f);
	a.fill = alpha(blue, 0.5f);
	let b = arrow(Direction.South, 100, 100, 0.5f, 0.5f);
	b.fill = alpha(red, 0.5f);
	let comb = new Rect(0, 0, 1, 1).toPath();
	if operation == 0:
		comb = intersectPaths(a, b);
	else if operation == 1:
		comb = differPaths(a, b);
	else if operation == 2:
		comb = unitePaths(a, b);
	else
		println($'Warning: {operation} is no valid Operation');
	endif
	comb.stroke = black;
	comb.strokeWidth = 1f;
	comb.fill = alpha(purple, 0.5f);
	viewBox = new ViewBox(100, 100);
endsvg

style algoTable(tbl: Table):
	tbl.orientation = Orientation.Stretch;
	tbl.margin = margin(10%);
	tbl.align = Alignment.Center;
endstyle

slide divisionByZeroAndArrayOutOfRange < stressMe:
	print($'{5 / 0f}');
	//print($'{5 / 0}');
	//print($'{5 / (0 * 18)}');
	//print($'{5 / int(min(5, 0))}');
	//let six = 6;
	//print($'{5 / min(six, 0)}');
	let array = [:int;99];
	//let lul = [:bool;5][99];
	//print($'{array[100]}');
	let slider = new Slider(-1..1);
	//slider.isVisible = false;
	//print($'{5 / slider.value}');	
	slider = new Slider(-8900..9000);
	//print($'{array[totalTime * 555]}');
	
endslide

slide algo < pagenumber:
	setData('name', 'gti presentation 1');
	let tbl = new Table(7, 7);
	tbl.applyStyle(algoTable);
	//Possible feature..
	//let hr = tbl.getRow(0)[..].content;
	//
	//let hr = [:string; tbl.getRow(0).len()];
	//for #anonymFor in tbl.getRow(0).len():
	//	hr[#anonymFor] = tbl.getRow(0)[#anonymFor].content;
	//endfor
	let headerRow = ['abc', 'abC', 'aBc', 'AbC', 'ABc', 'ABC'];
	tbl.setRow(headerRow, 0, 1);
	let headerCol = ['ab' , 'ac' , 'bC' , 'Bc' , 'AC' , 'AB'];
	tbl.setColumn(headerCol, 0, 1);
	tbl.cells[1][1].content = 'x';
	tbl.cells[6][6].content = 'x';
	for j in 0..5:
		tbl.cells[j + 2][j + 1].content = 'x';
		tbl.cells[j + 1][j + 2].content = 'x';
	endfor
	
	tbl.cells[5][..].background = orange;
	tbl.cells[..][4].background = red;
	tbl.cells[..][6].background = red;
endslide

slide algo2 < pagenumber:
	let tbl = new Table(6, 5);
	tbl.applyStyle(algoTable);

	let headerRow = ['abc', 'abC', 'aBc', 'ABc'];
	tbl.setRow(headerRow, 0, 1);
	let headerCol = ['ab' , 'ac' , 'bC' , 'Bc', 'AB'];
	tbl.setColumn(headerCol, 0, 1);
	tbl.cells[1][1].content = 'x';
	tbl.cells[1][2].content = 'x';
	tbl.cells[2][3].content = 'x';
	tbl.cells[4][4].content = 'x';
	for j in 0..4:
		tbl.cells[j + 2][j + 1].content = 'x';
	endfor
	tbl.cells[3][..].background = red;
	tbl.cells[5][..].background = red;
endslide

slide table < pagenumber:
	let contents = [
			['', 'b', 'c'],
			['a', '1', '1'],
			['b', '-', '0'],
			['c', '0', '-']
		];
	let tbl = new Table(contents);
	tbl.align = Alignment.Center;
	tbl.margin = margin(10%);
	tbl.orientation = Horizontal.Right | Vertical.Bottom;
endslide

data noneable:
	i : int;
enddata

slide github < pagenumber:
	code.setStyle(CodeHighlighter.Tomorrow);
	//let repository = code.github('wert007/GTIProject');
	//let codeBlockB = code.codeblock(repository, 'main.c', 3..14);
	//codeBlockB.fontsize = 10pt;
	//codeBlockB.margin = margin(5%, 15%);
	//codeBlockB.orientation = Horizontal.Stretch | Vertical.Center;
endslide

slide ~noneableBinding:
	let a = [:noneable?;99];
	a[0] = new noneable(42);
	//a[0] safe
	for i in 3..4:
		a[i] = new noneable(i);
		//a[0]|a[i] safe
		if a[i].i > i:
		//if a[i].i > i:
			//Do somethin
		endif
		a[i / 2] = none;
		//all unsafe
		a[80 - i] = new noneable(12 * i);
		//a[80-i] safe
	endfor
	//all unsafe


	let b = [:int?;99];
	b[0] = 42;
	//b[0] safe
	for i in 3..4:
		//b[0] safe
		//b[i] unsafe
		b[i] = i;
		//b[0]|b[i] safe
		b[80 - i] = 12 * i;
		//b[0]|b[i]|b[80-i] safe
	endfor
	//b[0] safe

	let c = [:int?;5];
	if true:
		c[2] = 7;
		//c[2] safe
	endif
	//c[2] unsafe

	//A Assignment can make a BoundExpression safe
	//A safe BoundExpression will survive until
	//	- there was any unsafe assignment to that BoundExpression
	//	- or the scope, where the safe BoundExpression was introduced, ends
	//Or in case of an IndexedArrayExpression:
	//	- the index changes

	if c[2]:
	endif
endslide

slide ~destroySlideCount:
endslide

slide mathTwo:
	let sld = new Slider(1..7);
	sld.orientation = Horizontal.Center | Vertical.Top;
	
	//TODO: What about the keyword? do we need it?
	//Can we change it? idk
	let f = #math 'b * x^2 + a * c';
	
	f.a = 2;
	use sld.value:
		f.b = sld.value;
		f.c = -2 * sld.value;
	enduse
	//TODO: Make mathematical expressions beautiful!
	//let expression = new MathExpression(f);
	
	let plot = new MathPlot(f, -2..3, 0.1f);
	//TODO! Tick Amount doesn't work on the x axis!!!!!!!!
	plot.xTickAmount = 2;
	plot.yTickAmount = 2;
	plot.orientation = Orientation.Stretch;
	plot.margin = margin(sld.bottomSide + 15%, 15%, 15%, 15%);

	let vid = youtube('VB4CCHHYOqY', true);
	vid.orientation = Orientation.Stretch;
	vid.margin = margin(sld.bottomSide + 15%, 15%, 15%, 15%);
endslide

slide a < pagenumber:
	let lbl = new Label('Hello World!');
	lbl.color = black;
	let sld = new Slider(1..255);
	lbl.fontsize = 90pt;
	//TODO it does work, but because we set top and margin at
	//different locations and this formula always just sets
	//the margin, there is confusion.
	//	lbl.margin.top = pct(sld.value * 10);
	use sld.value, totalTime:
		lbl.color = hsl(mod(totalTime / 100, 360), 255, sld.value);
	enduse
	lbl.orientation = Vertical.Top | Horizontal.Stretch;
	lbl.align = Alignment.Center;

endslide

slide cityDevelopment < pagenumber:
	let titleText = 'Geschichte der Stadtentwicklung';
	let contents = [
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
	
	let args = new custom.cityDevelopmentParameter();
	args.header = titleText;
	args.contents = contents;
	args.populationFile = @'somefile.csv';
	args.populationSource = '(c) surely wikipedia';

	//How long are we going to keep tuples? idk.. there is no good use case anywhere?
	//maybe in for loops. like 
	//
	//	for a~, i~ in array:
	//
	//Where i would be the index while a is the actual element.
	//The only current use of tuples is dead. even though
	//they are technically still supported.. just not used anymore..

	let ~text = new custom.cityDevelopmentText(args);
	~text.height = 100%;
	let container = seperator.vertical(20%);
	container.fill(new custom.imageBanner(), ~text);
	container.childB.padding = padding(10%, 5%);
endslide

slide introduction:
	background = image(@'city\los-angeles-picture.jpg');
	let losAngeles = new Label('Los Angeles');
	losAngeles.applyStyle(custom.imgLabel);

	//TODO: This gets detected.
	//let unused = 'Unused';
	//This not:
	let actuallyUnusedAsWell = 5;
	actuallyUnusedAsWell = 42;


	let source = new Label('(c) pixabay');
	source.applyStyle(custom.imgLabel);
	source.orientation = Horizontal.Right | Vertical.Top;

	let quote = 'Los Angeles is 72 suburbs in search of a city';
	let author = 'Dorothy Parker';
	let quoteBox = new custom.introductingQuote(quote, author);

	step:
		quoteGoesUp.play(quoteBox, 1s);
endslide

slide title:
	let main = 'Angloamerikanische Stadt';
	let sub = 'Am Beispiel von Los Angeles';
	let title = new basics.MainTitle(main, sub);
	title.orientation = Vertical.Center | Horizontal.Stretch;
	title.background = alpha(antiquewhite, 0.45f);
	background = image(@'city\night.jpg');
endslide

slide traits < pagenumber:
	let titleText = 'Merkmale einer anglo-amerikanischen Stadt';
	let contents = [
		'Schachbrettmuster des Straßenverlaufs',
		'Wolkenkratzer im Geschäftszentrum',
		'klassische Strukturierung der Stadt in __CBD__, __Übergangszone__ und __Außenbereich__',
		'__Commercial Strips__ entlang von Verkehrsachsen',
		'allgemeine Wohnform der gehobeneren Schichten in Vororten',
		'zunehmende Entstehung von __Gated Communties__', 
		'verfallende Kernstädte und Ghettobildung'
	];
	let title = new basics.Title(titleText);
	let list = new List(contents);
	list.fontsize = 24pt;
	list.isOrdered = true;
	list.margin.top = title.bottomSide;
	padding = padding(10%, 5%);
endslide

slide city < pagenumber:
	let titleText = 'Allgemeines zu Los Angeles';
	let title = new basics.Title(titleText);

	let logo = image(@'city\logo.png');
	let imgLogo = new Image(logo);
	imgLogo.height = title.height;
	imgLogo.width = imgLogo.height;

	title.margin = margin(0, 0, 0, imgLogo.rightSide);
	
	let captionLogo = '(c) City of Los Angeles';
	let imgLogoCaption = new Label(captionLogo);
	imgLogoCaption.orientation = Horizontal.Left | Vertical.Top;
	imgLogoCaption.margin = margin(title.bottomSide, 0, 0, 0);
	imgLogoCaption.fontsize = 8pt;

	let map = new custom.map();
	map.width = 50%;
	map.orientation = Vertical.Top | Horizontal.Right;
	map.margin = margin(title.bottomSide, 0, 0, 0);

	let contents = [
		'Zweitgrößte Stadt der USA',
		'3,8 Mio Einwohner innerhalb der Stadtgrenzen',
		'12,8 Mio Einwohner im Ballungsraum L.A.',
		'in 15 Bezirke (Districts) aufgegliedert',
		'wird aufgrund flächenhafter Ausdehnung \nauch "horizontal city" genannt'
	];
	let list = new List(contents);
	list.fontsize = 24pt;
	list.margin = margin(imgLogoCaption.bottomSide, 0, 0, 0);
	padding = padding(10%, 5%);
endslide



slide overview:
	let la = image(@'city\los-angeles-picture.jpg');
	let imgBackground = new Image(la);
	//imgBackground.filter = blur(5);
	imgBackground.orientation = Orientation.Stretch;
	imgBackground.stretching = ImageStretching.Cover;
	//let whitePane = rect(100%, 100%);
	//whitePane.fill = argb(160, 255, 255, 255);

	let map = image(@'city\map.png'); //TODO: Change image
	let imgMap = new Image(map);
	imgMap.orientation = Horizontal.Center | Vertical.Center;
	imgMap.width = 50%;
	imgMap.height = auto;

	//TODO: This feature would be like super cool.
	step:
		//filter = grayscale(1);

		let svgSrc = loadSVG(@'city\overlay.svg');
		let viewerComb = new SVGContainer(svgSrc);
		//TODO: Bring back!
		//viewerComb.filter = grayscale(0.5f);
		viewerComb.orientation = Orientation.Stretch;
		viewerComb.margin = margin(5%);
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
//	let repository = code.github('wert007/GTIProject');
//	let codeBlockB = code.codeblock(repository, 'main.c', 3..14);
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
////		vid.filter = discrete;
////		vid.margin = margin(0, 0, 0, 50%);
//endslide










slide ~singleLineStatementBlocks:
	let sld = new Slider(0..360);
	use totalTime, sld.value:
		background = hsl(sld.value, mod(totalTime, 255), mod(totalTime, 255));
	enduse
	/*
	use sld.value:
		let sld_value = document.getElementById('singleLineStatementBlocks-sld').value;
		let singleLineStatementBlocks = document.getElementById('singleLineStatementBlocks');
		singleLineStatementBlocks.style.background = 'hsl(' + sld_value + ', ' + (totalTime % 255) + ', ' + (totalTime % 255) + ')';
	enduse

	UseBlock::
		jsCode : string;
		variables : Dictionary<string, string>;
		type : JSInsertionType;
		functionName : string;

	function use_singleLineStatementBlocks_sld_value_changed(...)
	{
		let sld_value = document.getElementById('singleLineStatementBlocks-sld').value;
		let singleLineStatementBlocks = document.getElementById('singleLineStatementBlocks');
		singleLineStatementBlocks.style.background = 'hsl(' + sld_value + ', ' + (totalTime % 255) + ', ' + (totalTime % 255) + ')';
	}

	use totalTime:
		let sld_value = document.getElementById('singleLineStatementBlocks-sld').value;
		let singleLineStatementBlocks = document.getElementById('singleLineStatementBlocks');
		singleLineStatementBlocks.style.background = 'hsl(' + sld_value + ', ' + (totalTime % 255) + ', ' + (totalTime % 255) + ')';
	enduse
	*/
endslide
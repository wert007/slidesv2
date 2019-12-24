import lib('lib1.sld') as basics;
import lib('lib2.sld') as custom;
import gfont('Quicksand') as quicksand;

//Possible Features:
// - Support sliders, so that you can change graphes for example.
// - support github code in javascript!


template pagenumber(child: Slide):
	let text~ = $'{child.index + 1}/ {slideCount}';
	let label = new Label(text~);
	label.orientation = Horizontal.Right | Vertical.Top;
	label.fontsize = 12pt;
	label.margin = margin(10px);
endtemplate

style std:
	color = rgb(226, 226, 226);
	//Slide.background = black;
	//TODO: support std-styles for classes
	//e.g. 
	//if slide.background == None:
	//	slide.background = black;
	//endif
	//image.filter = saturate(0.5f);
	font = quicksand~;
	fontsize = 14pt;
	transition = stdTransition;
	//Kinda working, but not really
	//has something todo with overflow hidden.
	//But i dont know what?!
	//filter = grayscale(0.5f);
endstyle

transition stdTransition(from: Slide, to: Slide):
	background = black;
	duration = 200ms;
	from.hide(duration);
	to.fadeIn(0ms, duration);
endtransition

filter myFilter(source: FilterInput):
	//let blurred~ = blur(source, 5);
	let saturated~ = saturate(source, 0.5f);
	let matrix~ = matrix([
		  -1, 0, 0, 2, 0, 0, -1,
		  -1, 0, 0, 2, 0, 0, -1,
		  -1, 0, 0, 2, 0, 0, -1,
		  -1, 0, 0, 1, 0, 0, -1,
		  -1, 0, 0, 2, 0, 0, -1,
		  -1, 0, 0, 2, 0, 0, -1,
		  -1, 0, 0, 2, 0, 0, -1
		], 7, 7);
	let result~ = convolve(saturated~, matrix~);
endfilter

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
	case done:
	//TODO: Make more attributes animateable
	//Like margin or padding for example
		element.margin = margin(-100%, 0, 0, 0);
	//	element.margin.Top = -element.height;
		element.background = red;
endanimation

animation unblur(element: any, duration~: Time):
	case init:
		interpolation = Interpolation.Linear;
		element.filter = blur(5);
	case done:
		//Because of the cases progress always has a specific value and you cant use it
		//for calculations. You would need a special case for this...
		
		element.filter = blur(0);
endanimation

slide github < pagenumber:
		code.setStyle(CodeHighlighter.Funky);
		let repository~ = code.github('wert007/GTIProject');
		let file~ = repository~.file('main.c');
		//let codeBlockA = code.codeblock(file~, 3..14);
		let codeBlockB = code.codeblock(repository~, 'main.c', 3..14);
		codeBlockB.fontsize = 10pt;
		codeBlockB.orientation = Horizontal.Center | Vertical.Center;
		step:
			quoteGoesUp.play(codeBlockB, 1.5s);
endslide

slide cityDevelopment < pagenumber:
	background = black;
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
	//TODO: Doesn't work
	//let a = 5 + 0.5f;


	let source = new Label('(c) pixabay');
	source.applyStyle(custom.imgLabel);
	source.orientation = Horizontal.Right | Vertical.Top;

	let quote~ = 'Los Angeles is **72 suburbs** in search of a city';
	let author~ = 'Dorothy Parker';
	let quoteBox = new custom.introductingQuote(quote~, author~);
	quoteBox.background = alpha(black, 0.7f);

	//filter = myFilter~;

	step:
		//quoteGoesUp.play(quoteBox, 2s);
		//quoteBox.filter = myFilter~;
		unblur.play(quoteBox, 2s);
endslide

slide title:
	let main~ = 'Angloamerikanische Stadt';
	let sub~ = 'Am Beispiel von Los Angeles';
	let title = new basics.MainTitle(main~, sub~);
	title.orientation = Vertical.Center | Horizontal.Stretch;
	title.background = alpha(antiquewhite, 0.17f);
	background = image(@'city\night.jpg');
endslide

slide traits < pagenumber:
	background = black;
	let titleText~ = 'Merkmale einer anglo-amerikanischen Stadt';
	let contents~ = [
		$'Schachbrettmuster des Straßenverlaufs',
		$'Wolkenkratzer im Geschäftszentrum',
		$'klassische Strukturierung der Stadt in __CBD__, __Übergangszone__ und __Außenbereich__',
		$'__Commercial Strips__ entlang von Verkehrsachsen',
		$'allgemeine Wohnform der gehobeneren Schichten in Vororten',
		$'zunehmende Entstehung von __Gated Communties__', 
		$'verfallende Kernstädte und Ghettobildung'
	];
	let title = new basics.Title(titleText~);
	let list = new List(contents~);
	list.fontsize = 24pt;
	list.isOrdered = true;
	list.margin = margin(10%, 0, 0, 0); //==title.fontsize
	padding = padding(5%);
endslide

slide city < pagenumber:
	background = black;
	let titleText~ = 'Allgemeines zu Los Angeles';
	let title = new basics.Title(titleText~);

	let logo~ = image(@'city\logo.png');
	let imgLogo = new Image(logo~);// new basics.CaptionedImage(logo~, captionLogo~);
	imgLogo.height = title.height;
	imgLogo.width = imgLogo.height;

	title.margin = margin(0, 0, 0, imgLogo.right);
	
	let captionLogo~ = '(c) City of Los Angeles';
	let imgLogoCaption = new Label(captionLogo~);
	imgLogoCaption.orientation = Horizontal.Left | Vertical.Top;
	imgLogoCaption.margin = margin(title.bottom, 0, 0, 0);
	imgLogoCaption.fontsize = 8pt;

	let map = new custom.map();
	map.width = 50%;
	map.orientation = Vertical.Top | Horizontal.Right;
	map.margin = margin(title.bottom, 0, 0, 0);

	let contents~ = [
		'Zweitgrößte Stadt der USA',
		'3,8 Mio Einwohner innerhalb der Stadtgrenzen',
		'12,8 Mio Einwohner im Ballungsraum L.A.',
		'in 15 Bezirke (Districts) aufgegliedert',
		'wird aufgrund flächenhafter Ausdehnung \nauch "horizontal city" genannt'
	];
	let list = new List(contents~);
	list.fontsize = 24pt;
	list.margin = margin(imgLogoCaption.bottom, 0, 0, 0);
	padding = padding(5%);
endslide

slide overview:
	let la~ = image(@'city\los-angeles-picture.jpg');
	let imgBackground = new Image(la~);
	imgBackground.filter = blur(5);
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
endslide
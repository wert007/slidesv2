import lib('lib1.sld') as basics;

style transparentWhiteBackground(element: Element):
	element.background = argb(127, 236, 236, 236);
endstyle

style imgLabel(element: Label):
	element.applyStyle(transparentWhiteBackground);
	element.padding = padding(10px);
endstyle

group imageBanner():
	let oil = image(@'city\oil.jpg');
	let imgOil = new basics.CaptionedImage(oil, '(c) wikimedia');
	//imgOil.width = 100%;
	imgOil.height = 50%;

	let night = image(@'city\night.jpg');
	let imgNight = new basics.CaptionedImage(night, '(c) pixabay');
	//imgNight.width = 100%;
	imgNight.height = 50%; //TODO: If this semicolon is missing our compiler crashes...
	imgNight.margin = margin(50%,0,0,0);

	initWidth = 100%;
	initHeight = 100%;
endgroup

data cityDevelopmentParameter:
	header: string;
	contents: string[][];
	populationFile: string;
	populationSource: string;
enddata

group cityDevelopmentText(args: cityDevelopmentParameter):
	let populationData = csv(args.populationFile);
	let chart = new LineChart(populationData);
	chart.color = rgb(0, 0, 153);
	chart.orientation = Orientation.Stretch;
	let lblSource = new Label($'(c) {args.populationSource}');
	lblSource.orientation = Horizontal.Right | Vertical.Bottom;
	lblSource.applyStyle(imgLabel);

	let title = new basics.Title(args.header);
	let list = new List(args.contents);
	list.margin = margin(title.bottom, 0, 0, 0);

	initWidth = 100%;
	initHeight = 100%;

	background = green;
endgroup

slide cityDevelopment:
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

	
	let args = new cityDevelopmentParameter();
	args.header = titleText;
	args.contents = contents;
	args.populationFile = @'somefile.csv';
	args.populationSource = '(c) surely wikipedia';

	//
	//Can you store a Tuple in a single variable, or do you need to
	//tear it down to its child types?
	//
	//	let tuple = seperator.vertical(50%);
	// tuple[0], tuple[1]
	//
	//I'd say no. Because why would you?
	//
	//On the other hand: What if you want it as a parameter?
	//would be strange if you would have to put it in two 
	//variables first
	//
	// func lol(i: int, (left: Container, right: Container), str: string): int
	
	let left, right = seperator.vertical(30%);
	left.fill(new imageBanner());
	let text = new cityDevelopmentText(args);
	text.isVisible = false;
	text.height = 100%;
	right.fill(text);
endslide
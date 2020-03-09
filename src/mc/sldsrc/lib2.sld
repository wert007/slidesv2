import lib('lib1.sld') as basics;
import gfont('Amatic SC') as quoteFont;

library custom:
    useStyle();
    useGroup();
	useData();
endlibrary

style transparentWhiteBackground(element: Element):
	element.background = argb(127, 236, 236, 236);
endstyle

style imgLabel(element: Label):
	element.applyStyle(transparentWhiteBackground);
	element.padding = padding(10px);
endstyle

group introductingQuote(quote~: string, author~: string):
	let lblQuote = new Label(quote~);
	lblQuote.fontsize = 54pt;
	lblQuote.font = quoteFont~;
	lblQuote.orientation = Horizontal.Stretch | Vertical.Top;
	let lblAuthor = new Label($'- {author~}');
	lblAuthor.fontsize = 24pt;
	lblAuthor.font = quoteFont~;
	lblAuthor.orientation = Horizontal.Right | Vertical.Bottom;

	//applyStyle(transparentWhiteBackground);
	background = argb(127, 236, 236, 236);
	orientation = Horizontal.Center | Vertical.Center;

	initWidth = 50%;
	initHeight = 40%;
	padding = padding(5%);
endgroup

group map():
	let map~ = image(@'city\map.png');
	let imgMap = new Image(map~);
	imgMap.width = 100%;
	imgMap.height = auto;
	imgMap.orientation = Horizontal.Right | Vertical.Top;
	imgMap.position = 'relative';
	//imgMap.#position = 'relative';
	//imgMap.position! = 'relative';
	//imgMap._position = 'relative';
	let lblMap = new Label('(c) wikimedia');
	lblMap.orientation = Horizontal.Right | Vertical.Bottom;

	let marker = new Rectangle(50, 50);
	marker.orientation = Horizontal.Right | Vertical.Top;
	marker.margin = margin(80%, 70%, 0, 0);
	marker.borderStyle = BorderStyle.Dashed;
	marker.borderColor = red;

	initWidth = 100%;
	initHeight = 40%;
endgroup

group imageBanner():
	let oil~ = image(@'city\oil.jpg');
	let imgOil = new basics.CaptionedImage(oil~, '(c) wikimedia', ImageStretching.Cover);
	imgOil.height = 50%;

	let night~ = image(@'city\night.jpg');
	let imgNight = new basics.CaptionedImage(night~, '(c) pixabay', ImageStretching.Cover);
	imgNight.height = 50%;
	imgNight.margin = margin(50%, 0, 0, 0);


	initWidth = 100%;
	initHeight = 100%;
endgroup

data cityDevelopmentParameter:
	header~: string;
	contents~: string[][];
	populationFile~: string;
	populationSource~: string;
enddata

group cityDevelopmentText(args~: cityDevelopmentParameter):
	let populationData~ = csv(args~.populationFile~);
	let chart = new LineChart(populationData~);
	chart.color = rgb(0, 0, 153);
	chart.orientation = Orientation.Stretch;
	chart.bare();
	let lblSource = new Label($'(c) {args~.populationSource~}');
	lblSource.orientation = Horizontal.Right | Vertical.Bottom;
	lblSource.applyStyle(imgLabel);

	let title = new basics.Title(args~.header~);
	let list = new List(args~.contents~);
	list.margin = margin(title.bottomSide, 0, 0, 0);

	initWidth = 100%;
	initHeight = 100%;
endgroup
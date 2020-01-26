library basics:
    useGroup();
	useData();
endlibrary

group CaptionedImage(img~: ImageSource, caption~ : string, stretching~ = ImageStretching.Contain):
	let image = new Image(img~);
	image.height = 100%;
	image.width = 100%;
	image.stretching = stretching~;
	let lblCaption = new Label(caption~);
	initWidth = 100%;
	initHeight = 100%;
	lblCaption.orientation = Vertical.Bottom | Horizontal.Left;
endgroup

group Title(content~: string):
	let lblTitle = new Label(content~);
	lblTitle.fontsize = 32pt;
	lblTitle.orientation~ = Horizontal.Left | Vertical.Top;
	initWidth = lblTitle.width;
	initHeight = lblTitle.height;
	fontsize = lblTitle.fontsize;
endgroup

group MainTitle(main~: string, sub~: string?):
	let lblMainTitle = new Label(main~);
	lblMainTitle.fontsize = 52pt;
	lblMainTitle.orientation = Horizontal.Center | Vertical.Top;
	lblMainTitle.align = Alignment.Center;

	initWidth = lblMainTitle.width;
	initHeight = lblMainTitle.height;
	if sub~:
		let lblSubTitle = new Label(sub~);
		lblSubTitle.fontsize = 28pt;
		lblSubTitle.orientation = Horizontal.Center | Vertical.Bottom;
		lblSubTitle.align = Alignment.Center;

		initHeight += lblSubTitle.height;
	endif
endgroup
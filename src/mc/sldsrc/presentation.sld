import font('Arial') as arial;

style std:
    font = arial;
    Slide.background = black;
    foreground = hex('#adadad');
endstyle

slide introduction:
    let stampSrc = img('gfx\\stamp.jpg');
    let ~stamp = new Image(stampSrc);
    let captionedStamp = new Captioned(~stamp, 'Russische Briefmarke | liveinternet.ru');
    captionedStamp.orientation = Orientation.Stretch;
    captionedStamp.margin = margin(2%);
endslide

slide title:
    let main = new Label('Die Transsibirische Eisenbahn');
    main.orientation = Horizontal.Stretch | Vertical.Bottom;
    main.align = Alignment.Center;
    main.margin.bottom = 50%;
    let sub = new Label('100 Jahre Transsib');
    sub.orientation = Horizontal.Center | Vertical.Top;
    sub.align = Alignment.Center;
    sub.margin.top = 50%;
endslide
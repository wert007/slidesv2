import font('Arial') as arial;

style std:
    font = arial;
    Slide.background = black;
    color = hex('#adadad');
endstyle

group helpCaptioned(child: Element, strCaption: string):
    let ~caption = new Label(strCaption);
    //child.orientation = Horizontal.Stretch | Vertical.Top;
    //~caption.orientation = Horizontal.Stretch | Vertical.Bottom;
    let stack = new Stack(FlowAxis.Vertical);
    stack.add(child);
    stack.add(~caption);
    stack.orientation = Orientation.Stretch;
    width = 40%;
    height = stack.height;
endgroup

slide introduction:
    let stampSrc = image('gfx\\stamp.jpg');
    let ~stamp = new Image(stampSrc);
    let captionedStamp = 
    new Captioned(~stamp, 'Russische Briefmarke | liveinternet.ru'); 
    //new helpCaptioned(~stamp, 'Russische Briefmarke | liveinternet.ru');
    captionedStamp.orientation = Vertical.Center | Horizontal.Center;
    captionedStamp.width = 40%;
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
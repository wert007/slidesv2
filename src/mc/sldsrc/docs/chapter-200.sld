///Topic: Borders. Probably more of a test, then a doc
import font('Arial') as arial;

template default(s: Slide):
    let prev = new Label('[url="../chapter-100/index.html"]prev[/url]');
    prev.orientation = Horizontal.Left | Vertical.Top;
    let next = new Label('[url="../chapter-210/index.html"]next[/url]');
    next.orientation = Horizontal.Right | Vertical.Top;
endtemplate

style std:
    font = arial;
    orientation = Orientation.Center;

    Label.margin = margin(5px);
    Label.padding = padding(5px);
    Label.background = rgb(200, 255, 200);
endstyle

// A border is made of 4 sides, top right bottom left
// and each side, a BorderLine is made of 3 components
// width, style and color. 


slide t1 < default:
    // Here you have a demonstration of how with just a color you can 
    // already create a border and how you can access just a few sides
    // or the border as whole.
    let stack = new Stack(FlowAxis.Vertical);
    let ~label = new Label('Hello World!\nThis is me!!');
    ~label.border.color = red;
    stack.add(~label);
    ~label = new Label('Hello World!\nThis is me!!');
    ~label.border.right.color = green;
    ~label.border.left.color = green;
    stack.add(~label);
    ~label = new Label('Hello World!\nThis is me!!');
    ~label.border.top.color = blue;
    ~label.border.bottom.color = blue;
    stack.add(~label);
endslide

slide t2 < default:
    // Here you have the same demonstration as above, but this time
    // width a border-width. The border-color will be determined by
    // the color field, which defaults to black. 
    // thin, medium and thick are just constant units. border-width has
    // a default value of medium 
    let stack = new Stack(FlowAxis.Vertical);
    let ~label = new Label('Hello World!\nThis is me!!');
    ~label.border.width = 1px;
    stack.add(~label);
    ~label = new Label('Hello World!\nThis is me!!');
    ~label.border.left.width = 12pt;
    ~label.border.right.width = 99px;
    stack.add(~label);
    ~label = new Label('Hello World!\nThis is me!!');
    ~label.border.top.width = thin;
    ~label.border.bottom.width = thick;
    stack.add(~label);
    ~label = new Label('Hello World!\nThis is me!!');
    ~label.border.top.width = thin;
    ~label.border.bottom.width = thick;
    ~label.color = green;
    stack.add(~label);
endslide


slide t3 < default:
    // Here you have a quick demonstration of all BorderStyle|s there
    // are. You can as well use BorderStyle.None to remove a border.
    background = rgb(255, 255, 200);
    let stack = new Stack(FlowAxis.Horizontal);
    let ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Dotted;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Dashed;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Solid;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Double;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Groove;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Ridge;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Inset;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Outset;
    stack.add(~label);
endslide

slide t4 < default:
    // The same as t3, but with colors!
    background = rgb(255, 255, 200);
    let stack = new Stack(FlowAxis.Horizontal);
    let ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Dotted;
    ~label.border.color = red;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Dashed;
    ~label.border.color = red;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Solid;
    ~label.border.color = red;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Double;
    ~label.border.color = red;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Groove;
    ~label.border.color = red;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Ridge;
    ~label.border.color = red;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Inset;
    ~label.border.color = red;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.Outset;
    ~label.border.color = red;
    stack.add(~label);
    ~label = new Label('Hello World!');
    ~label.border.style = BorderStyle.None;
    ~label.border.color = red;
    stack.add(~label);
endslide

slide t5 < default:
    // How you probably will set a border most of the time
    // You use the border() function, which returns a BorderLine
    // The BorderLine then gets converted into a Border (The line represents all 4 sides)
    //          border(width: Unit, style: BorderStyle, color: Color) : BorderSide
    let label = new Label('simple BorderLine to Border conversion');
    label.border = border(5px, BorderStyle.Solid, purple);
endslide

slide t6 < default:
    // Here you can see, how the border() function overrides part
    // of the border, that was set in the line before
    let label = new Label('simple Border side overriding');
    label.border = border(5px, BorderStyle.Solid, purple);
    label.border.top = border(10px, BorderStyle.Ridge, aliceblue);

    // But if you do it the other way arround it won't work
    // border will override everything you had set before.
    let label2 = new Label('errorious Border side overriding');
    label2.border.top = border(10px, BorderStyle.Ridge, aliceblue);
    label2.border = border(5px, BorderStyle.Solid, purple);
    label2.margin.top = 2f * label.height;
endslide

slide t7 < default:
    // Similiar to t6, but if you try to override the color
    // it will remove the color specified in the line above
    // completely
    let label = new Label('simple Border color overriding');
    label.border = border(5px, BorderStyle.Solid, purple);
    label.border.color = cornflowerblue;

    let label2 = new Label('errorious Border color overriding');
    label2.border.color = cornflowerblue;
    label2.border = border(5px, BorderStyle.Solid, purple);
    label2.margin.top = 2f * label.height;
endslide

slide t8 < default:
    // Even though this does look similiar to the very first
    // example, but in this case the border-style is explicitly
    // set to none. So it won't be replaced with a solid style. 
    let label = new Label('This won\'t have any border!');
    label.border.style = BorderStyle.None;
    label.border.color = teal;
endslide
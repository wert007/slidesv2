///Topic: Stacks!
import font('Arial') as arial;

// Just some constants we will use to populate our stacks later.
const stackLengthFactor = 9;
const rainbow = [red, green, blue, yellow, lime, purple];
const loremIpsum = 'vel repellendus laborum natus eaque quas voluptate beatae provident error occaecati velit facere doloribus suscipit ea id aut rerum atque';
const sourceCodeMainC = coding.loadFile(@'src_code\chapter-150\main.c', 'C');
const imageSources = [
        image(@'gfx\chapter-150\turtle.jpg'),
        image(@'gfx\chapter-150\buildings.jpg'),
        image(@'gfx\chapter-150\desert.jpg'),
        image(@'gfx\chapter-150\buildings.jpg'),
    ];


template default(s: Slide):
    let prev = new Label('[url="../chapter-100/index.html"]prev[/url]');
    prev.orientation = Horizontal.Left | Vertical.Top;
    let next = new Label('[url="../chapter-200/index.html"]next[/url]');
    next.orientation = Horizontal.Right | Vertical.Top;
endtemplate

style std:
    font = arial;
    coding.highlighting = CodeHighlighter.Okaidia;
endstyle

slide s1 < default:
    // A stack is something like a list, in that it takes multiple child-elements
    // and displays them in some order. The difference is, that stack gives you
    // way more options and is not limited to text.
    //
    // The first difference you see in the constructor is that a stack has a 
    // FlowAxis. If your stack has a FlowAxis.Horizontal things will be stacked
    // next to eacht other and if you have FlowAxis.Vertical they will be on top
    // of each other, just like with a list. 
    //
    // Stacks have no ListMarkerStyle thing. So even if you would add Labels to your
    // stack to simulate a list, it wouldn't have any bullet points or anything like that.
    // A stack is just for adding multiple elements without having to specify a position
    // for each one.  
    let stack = new Stack(FlowAxis.Horizontal);
    for i in 0..stackLengthFactor:
        // So the stack.add(e: Element) function takes a Element and not a string.
        // Just highlights the difference described above. 
        stack.add(new Image(imageSources.getLoop(i)));
    endfor
    stack.orientation = Horizontal.Right | Vertical.Bottom;
    stack.margin.bottom = 50px;
    stack.height = 200px;
endslide

slide s2 < default:
    let stack = new Stack(FlowAxis.Vertical);
    for i in 0..stackLengthFactor:
        stack.add(new Image(imageSources.getLoop(i)));
    endfor
    stack.orientation = Horizontal.Right | Vertical.Center;
    stack.margin.right = 50px;
    stack.width = 200px;
    stack.height = 100%;
endslide


slide s3 < default:
    let stackHorizontal = new Stack(FlowAxis.Horizontal);
    let stackVertical = new Stack(FlowAxis.Vertical);
    for i in 0..(rainbow.len() * stackLengthFactor):
        let curColor = rainbow.getLoop(i);
        let lH = new Label($'Hello World!');
        lH.padding = padding(5px);
        lH.background = curColor.invert();
        lH.color = curColor;
        lH.fontsize = 30px;
        stackHorizontal.add(lH);
        stackVertical.add(0, lH);
    endfor
    stackHorizontal.orientation = Horizontal.Right | Vertical.Top;
    stackHorizontal.margin.top = stackHorizontal.height;
    stackVertical.orientation = Horizontal.Right | Vertical.Top;
    stackVertical.margin.right = stackVertical.width;
endslide

slide s4 < default:
    let stack = new Stack(FlowAxis.Vertical);
    for i in 0..(stackLengthFactor * imageSources.len()):
        if mod(i, 2) == 0:
            let img = new Image(imageSources.getLoop(i / 2));
            img.height = 50px;
            stack.add(img);
        else
            let word = loremIpsum.split(' ').getLoop(i / 2);
            stack.add(new Label(word));
        endif
    endfor
    stack.align = StackAlignment.Right;
    stack.orientation = Horizontal.Right | Vertical.Center;
endslide

slide s5 < default:
    let schedule = new Stack(FlowAxis.Horizontal);
    let weekdays = [
        new Stack(FlowAxis.Vertical),
        new Stack(FlowAxis.Vertical),
        new Stack(FlowAxis.Vertical),
        new Stack(FlowAxis.Vertical),
        new Stack(FlowAxis.Vertical),
        new Stack(FlowAxis.Vertical),
        new Stack(FlowAxis.Vertical),
    ];
    weekdays[0].add(new Label('**Sunday**'));
    weekdays[0].add(new Label('[bgcolor=cornflowerblue]Free![/bgcolor]'));
    weekdays[0].add(new Label('No drinking. Need to study tomorrow again!'));
    weekdays[1].add(new Label('**Monday**'));
    weekdays[1].add(new Label('Study Math'));
    weekdays[1].add(new Label('Drinking'));
    weekdays[2].add(new Label('**Tuesday**'));
    weekdays[2].add(new Label('Study Programming'));
    weekdays[2].add(new Label('Drinking'));
    weekdays[3].add(new Label('**Wednesday**'));
    weekdays[3].add(new Label('Done studying'));
    weekdays[3].add(new Label('Visiting AA'));
    weekdays[4].add(new Label('**Thursday**'));
    weekdays[4].add(new Label('Drinking'));
    weekdays[4].add(new Label('Drinking with friends'));
    weekdays[5].add(new Label('**Friday**'));
    weekdays[5].add(new Label('More drinking'));
    weekdays[6].add(new Label('**Saturday**'));
    weekdays[6].add(new Label('You guessed it!'));
    for w, j in weekdays:
        w.width = 100% / weekdays.len();
        schedule.add(w);
        for c, i in w.children:
            if i > 0:
                c.background = [white, rgb(230, 230, 230)].getLoop(j + i);
            endif 
        endfor
    endfor
    schedule.align = StackAlignment.Top;
    schedule.orientation = Vertical.Center | Horizontal.Stretch;
    // schedule.margin.horizontal = 10%;
    schedule.margin = margin(0, 15%);
endslide

slide t1 < default:
    let stack = new Stack(FlowAxis.Vertical);
    for i, index in (0..7) * 7:
        let c = coding.codeblock(sourceCodeMainC, (i + 1)..(i + 7));
        c.width = (index + 1f) * 130px;
        stack.add(c);
    endfor
    stack.fontsize = 5pt;
    stack.align = StackAlignment.Center;
    stack.orientation = Orientation.Center;
endslide

slide t2 < default:
    let stack = new Stack(FlowAxis.Horizontal);
    for i in (7..14) * 7:
        let c = coding.codeblock(sourceCodeMainC, (i + 1)..(i + 7));
        stack.add(c);
    endfor
    stack.fontsize = 5pt;
    stack.align = StackAlignment.Center;
    stack.orientation = Orientation.Center;
endslide

slide t3 < default:
    background = rgb(255, 255, 200);
    let stack = new Stack(FlowAxis.Horizontal);
    stack.orientation = Orientation.Center;
    for _ in 0..4:
        let label = new Label('Hello World!');
        label.border.style = BorderStyle.Dotted;
        label.border.color = red;
        stack.add(label);
    endfor
endslide
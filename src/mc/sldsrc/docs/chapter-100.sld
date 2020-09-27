///Topic: BBCodes and Markdown
import font('Arial') as arial;

template default(s: Slide):
    let prev = new Label('[url="../chapter-90/index.html"]prev[/url]');
    prev.orientation = Horizontal.Left | Vertical.Top;
    let next = new Label('[url="../chapter-200/index.html"]next[/url]');
    next.orientation = Horizontal.Right | Vertical.Top;
endtemplate

style std:
    font = arial;
    orientation = Orientation.Center;
endstyle

slide bugs < default:
    let label = new Label('[color=red;background-color:green]be brave[/url]');
endslide

slide s1 < default:
    // Whenever you put text on the slide you can format it in multiple ways
    // you can do it for the whole element (like you have probably seen before)
    // or for specific sub-parts via markdown or BBCode

    // Let's use a Label to demonstrate this!
    let label = new Label('Hello, world!');

    // Let's make the world bold and the whole text but the exclamation mark cursive
    label = new Label('__Hello, **world**__!');
    // Try it out and have a look :D
    // What we now used was markdown. You can use it to make your text **bold** __cursive__
    // ___underlined___ or ~~strike through~~ you can also create {links to other things}(http://example.com)
    // if you format links, always use a prefix like https:// or http:// otherwise it won't work as you expect
    
    // But there is a second way to achieve the same effects we could create with markdown:
    // bbcode! So the example from before becomes this:
    label = new Label('[i]Hello, [b]world[/b][/i]!');
    // [b]bold[/b] [i]cursive[/i] [u]underlined[/u] and [s]strike through[/s] can be created
    // similiarly, but it might be a little bit more verbose. you can't use any spaces,
    // so [b ]I'm in space[ / b] won't work. between [ and ] there are no spaces allowed.
    // but [B]im BIG[/b] works, because bbcode is case-insensitive.
    // Links work like this: [url=http://example.com]links to other things[/url]
    // The target is actually given via a attribute. Attribute can be escaped via "":
    // so [url="http://example.com/path with spaces or closing ]"]works just as well[/url].
endslide

slide s2 < default:
    // But bbcode can do a few things which markdown can't
    // one example would be colors:
    let label = new Label('[color=#ffaa00]**Hello**, [/color][color=blue][size=35pt]world[/size][/color]!');
    // As you can see you can change the font-size mid-text as well, you can use
    // hexvalues to define your colors

    // Other bbcodes you can use:
    //  More coloring:
    //   [bgcolor=green]background color is green[/green]
    //  You can use them to re-align your text mid-line
    //   [sup]upper text[/sup]
    //   [sub]lower text[/sub]
    //  Or to mark some (inline) quotes with optional author
    //   [quote]quote[/quote]
    //   [quote="author"]quote by some smart guy[/quote]
    
    // Those might work at some point, but not right now!
    //   [center]center-text[/center]
    //   [left]left-text[/left]
    //   [right]right-text[/right]
endslide

slide s3 < default:
    // You can use the [code] tag to show off your bbcode skills:
    let label = new Label('[code][b]this is __not__ bold[/b][/code] ==> [b]this is __not__ bold[/b]');
    // Please note, that this doesn't effect markdown.
endslide

slide s4 < default:
    // You can even use bbcode to semi-replace the List-element
    // you can use the 
    //   [list][*]tag for some quick lists [*]inside places where you
    //   otherwise couldn't add some lists[/list]
    // as you see the [*] doesn't have closing tag, it will be closed automatically,
    // and outside of a list it will be ignored
    let label = new Label('[list=I][*][quote]In Rome do as the Romans do[/quote][*]... and other Latin puns.[/list]');
    // you can use the attribute of the list to specify a certain style
    // so does 'i' equal a latin numbering (and 'I' a capital latin numbering)
    // '1' a will result in a decimal numbering and 'a' (or 'A') in a alphabetical numbering
endslide

slide testingEverything < default:
    let list = new Label(
        'You can use it to make your text **bold** __cursive__ ___underlined___ or ~~strike through~~ you can also create {links to other things}(http://example.com)\n[b]bold[/b] [i]cursive[/i] [u]underlined[/u] and [s]strike through[/s] so [b ]I\'m in space[ / b] won\'t work. between [ and ] there are no spaces allowed. but [B]im BIG[/b] works\nLinks work like this: [url=http://example.com]links to other things[/url] The target is actually given via a attribute. Attribute can be escaped via "": so [url="http://example.com/path with spaces or closing ]"]works just as well[/url].\nMore coloring: [bgcolor=green]background color is green[/bgcolor]\nYou can use them to re-align your text mid-line [sup]upper text[/sup] or [sub]lower text[/sub] \n Or to mark some (inline) quotes with optional author or [quote]quote[/quote] or [quote="author"]quote by some smart guy[/quote]'
    );

    //Needs more thinking!
    //[center]center-text[/center] or [left]left-text[/left] or [right]right-text[/right] or
endslide
slide testingEverything2 < default:
    let list = new List([
       '[list][*]tag for some quick lists [*]inside places where you otherwise couldn\'t add some lists[/list]',
       '[list=i][*]tag for some quick lists [*]inside places where you otherwise couldn\'t add some lists[/list]',
       '[list=I][*]tag for some quick lists [*]inside places where you otherwise couldn\'t add some lists[/list]',
       '[list=1][*]tag for some quick lists [*]inside places where you otherwise couldn\'t add some lists[/list]',
       '[list=a][*]tag for some quick lists [*]inside places where you otherwise couldn\'t add some lists[/list]',
       '[list=A][*]tag for some quick lists [*]inside places where you otherwise couldn\'t add some lists[/list]',
    ]);
endslide
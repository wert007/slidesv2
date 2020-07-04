//Imports the Google-Font "Roboto" under the name mono
//
//		import gfont('Roboto') as mono;
//      ^      ^               ^  ^
//		import keyword. This marks the start of an import
//			   gfont function. Takes the name of the font as parameter and returns a Font
//							   as keyword. Marks that next is the local variable name of the font
//								  the local variable name
//
//Imports should be written on top of the file. 
import gfont('Roboto') as mono;

//Declares a slide with "Hello World" written on it.
//
//		slide helloWorld:
//      ^     ^
//		slide keyword. This marks the start of the slide definition!
//			  Identifier of this slide. So you know what this slide is 
//			  about.
slide helloWorld:

	//Variable declaration
	//The keyword 'let' marks a variable declaration followed by an identifier
	//
	//Initializer ("new Label('Hello World')") is a Label-Constructor 
	//with the label content as parameter
	let lbl = new Label('Hello World');
	
	//Sets the font of our newly created Label to the font we imported
	//in the beginning
	lbl.font = mono;

	//Sets the fontsize of this Label. pt is a Unit well fitted for text.
	//pt should be about the same as fontsize in google docs.
	//But you could as well use Pixel. If you have a good reason for that..
	lbl.fontsize = 100pt;

	//Sets the anchor of the Label. There are Top, Bottom, Center, Stretch (or Left, Right)
	//as possible combinable values. They are inspired by C# WPF Alignment
	lbl.orientation = Vertical.Center | Horizontal.Center;

//endslide keyword marks the end of the current slide. 
//Another slide could follow now.
endslide

//how to build:
//
//		./slides.exe helloWorld.sld output
//
//		helloWorld.sld
//		is this file. So the "compiler" will grab this file
//		and build your presentation from this
//
//		output
//		this is the name of the folder in to which you want
//		to output your presentation.
//
//		Your presentation will be named index.html. This is
//		the file you can open in your browser.
//		But it will be as well an index.css and an index.js
//		generated. All other files are copied.

//So let's take look at a little more code. Time for a new slide.
slide howDoesThePresentationWork:
	//This time we create Variable, which only contains data. 
	//So it can't be drawn to the screen. In this case it's just
	//a string. To draw it you need a Label.
	//
	//In the string we use backslashes (\) to escape certain characters.
	//So means \n a new line and will not print \n on the screen
	//The two strings get concatenated via the plus. So we don't need to 
	//write everything into one line.
	//Then we use again \ but this time with \', which gives us actually 
	//the character ' and doesn't end the string.
	//
	//With ** and __ you have basic formatting options. These should work
	//everywhere you use Labels! (Most visual elements do!)
	let controls = 'With the up and down arrow keys you can switch between slides. \n' +
					'And I don\'t have more to say. **Big** __italic **stuff**__(c)';
	
	//So just like before where we wrote the text for the Label directly
	//between the parenthesis, we can know just use the variable.
	let lblControls = new Label(controls);
	lblControls.font = mono;
	lblControls.orientation = Vertical.Center | Horizontal.Center;

	//Since we now have multiline, we can make use of alignment.
	//possible values: Left, Right, Center, Block
	//These just translate to css-text-align properties.
	lblControls.align = Alignment.Right;

	//Rotation is currently in degree (0-360)
	lblControls.rotation = 30;

endslide

//A style is a preset for fields of visual elements
//Most styles can be applied to an element via "element.applyStyle(<StyleName>)""
//But "style std" is automatically applied to all elements in your slide.
style std:
	//Here we set the font to mono. Just like we did
	//before on all the labels. But now every Label
	//and everything else, that displays text will
	//automatically have this font. If you want to
	//use a special font for a single element you can
	//just write "specialelement.font = comicSans;" and
	//the std font will be overridden.
	font = mono;

//Just like on slides you need to say on styles as well
//when they end.
endstyle

//the tilde before the identifier tells you, that this slide is hidden,
//when the html file is generated.
slide ~hidden:
	let lbl = new Label('This slide will be skipped during compile-time.');	
endslide

slide lists:
	//So we have Lists as well. A List is a list of Labels, so all the formatting
	//works as well. 
	let list = new List();
	//via the add function you can add a list item
	//It takes a string, which the list will put into a Label to display
	list.add('Just some bullet points...');
	//If you want to make use of sub levels you can give a integer
	//as the first argument. It defaults to zero if left out.
	list.add(1, '...telling you about this new programming language...');
	list.add(2, '...for programming slides!');

	//This is really helpful when styling your slides.
	//you can set the margin values "top", "right", "bottom", "left"
	//so the top value says, where your list starts on the y axis.
	//
	//In this case we set it to the bottom_side of our lbl. 
	//So if we move our Label, the List will move as well.
	//list.margin.top = lbl.bottomSide;
endslide

//This is a CustomStyle. It takes the visual element as parameter
//which is going to be affected by it. any is a keyword which says
//that it doesn't matter which kind of element you give it to it.
style hoverStyle(element: any):
	//We set the filter of that element to a hueRotation of 180° 
	//We will talk more about filters later.
	element.filter = hueRotate(0.5f);
endstyle

slide hoverExample:
	let imgSrc = image('desert.jpg');
	let imgDesert = new Image(imgSrc);
	imgDesert.orientation = Horizontal.Stretch | Vertical.Stretch;
	imgDesert.margin = margin(5%);
	imgDesert.hover = hoverStyle;
endslide
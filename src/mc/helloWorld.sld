//Imports the Google-Font "Roboto" under the name mono
//
//		import gfont('Roboto') as mono;
//		^	   ^			   ^  ^
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
//		^	  ^
//		slide keyword. This marks the start of the slide definition!
//			  Identifier of this slide. So you know what this slide is 
//			  about.
slide helloWorld:

	//Variable declaration
	//Keyword let marks a variable declaration followed by an identifier
	//
	//Initializer ("new Label('Hello World')") is a Label-Constructor 
	//with the label content as parameter
	let lbl = new Label('Hello World');
	
	//Sets the font of our newly created Label to the font we imported
	//in the beginning
	lbl.font = mono~;

	//Sets the fontsize of this Label. pt is a Unit well fitted for text.
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

//So let's look at a little more. Time for a new slide.
slide howDoesThePresentationWork:
	//This time we create Variable, which only contains data. 
	//So it can't be drawn to the screen. In this case it's just
	//a string. To draw it you need a Label.
	//Data variables need to end with ~ so they don't get mixed up
	//with visual elements.
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
	let controls~ = 'With the up and down arrow keys you can switch between slides. \n' +
					'And I don\'t have more to say. **Big** __italic **stuff**__(c)';
	
	//If you use controls now you need to use as well the post-tilde.
	//If you forget them, the "compiler" will cry loudly!
	let lblControls = new Label(controls~);
	lblControls.font = mono~;
	lblControls.orientation = Vertical.Center | Horizontal.Center;

	//Since we now have multiline, we can make use of alignment.
	//possible values: Left, Right, Center, Block
	//These just translate to css-text-align properties.
	lblControls.align = Alignment.Right;
endslide
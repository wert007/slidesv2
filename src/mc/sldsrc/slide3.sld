svggroup blob(w: int, r: int, col: Color):
	let rectWidth = w - r;
	let diameter = 2 * r;
	let rect = new Rect(0, 0, rectWidth, diameter);
	rect.fill = col;

	let circle = new Circle(rectWidth, r, r);
	circle.fill = col;
	width = w;
	height = diameter;
endsvggroup

svg diagramArrows(maxWidth: int, maxR: int, pos: int):
	width = 2 * maxR;
	height = 2 * maxR;
	let a = [:Path?;3];
	let directions = [Direction.North, Direction.South, Direction.East];
	let xPos = [maxR - pos, maxR - pos, 15];
	let yPos = [pos, 2 * maxR - pos, maxR];
	for i in 0..a.len():
		a[i] = arrow(directions[i], 30, 30, 0.5f, 0.6f);
		a[i].x = maxWidth - pos - xPos[i];
		a[i].y = yPos[i] - 15;
		a[i].fill = rgb(102, 102, 102);
	endfor
endsvg

//svggroup diagramLabels(dist: int[], initialWidth: int):
//	let labels = ['', 'Umland', 'Übergangsbereich', '', 'Downtown', 'Central Business District'];
//	let l = labels.len() * 2 - 1;
//	let lbl = [:Text?;l];
//	color = black;
//	for i in 0..labels.len():
//		if labels[i] != '':
//			lbl[i] = new Text(labels[i]);
//			let w = dist[i] + 0.5f * (dist[i + 1] - dist[i]);
//			lbl[i].x = 10;
//			lbl[i].y = w; //+ half height of text.
//			lbl[i].baseLine = middle;
//			let j = l - i - 1;
//			if j != i:
//				lbl[j] = new Text(labels[i]);
//				lbl[j].translate(5%, 100% - w);
//				lbl[j].baseLine = middle;
//			endif
//		endif
//	endfor
//	initHeight = 100%;
//	initWidth = initialWidth;
//endsvggroup


svggroup diagram():
	let maxWidth = 2000;
	let maxR = 500;
	let dist = [0, 30, 130, 230, 280, 380, 1000 - 380];

	let cols = [rgb(217, 234, 211), rgb(147, 196, 125), rgb(255, 229, 153), rgb(239, 239, 239), rgb(224, 102, 102), rgb(204, 65, 37)];
	let coll = [:SVGGroup?;cols.len()]; //== [none, none, none, none, none]
		//Possible Feature
		//let coll = [42;5]; //== [42, 42, 42, 42, 42]

	for i in 0..coll.len():
		coll[i] = new blob(maxWidth - dist[i], maxR - dist[i], cols[i]); 
		coll[i].y = dist[i];
	endfor

	width = maxWidth;
	height = 2 * maxR;
	//let labels = new diagramLabels(dist, maxWidth);
	let arrows = new diagramArrows(maxWidth, maxR, dist[5]);
	let road1 = new Rect(0, 0, maxWidth, 20).toPath();
	road1.fill = rgb(238, 238, 238);
	road1.stroke = rgb(89, 89, 89);
	road1.strokeWidth = 2;
	road1.rotate(45, 0, 0);
	road1.x = maxWidth - maxR - 10;
	road1.y = maxR - 10;
	let road2 = new Rect(0, 0, maxWidth, 20).toPath();
	road2.fill = rgb(238, 238, 238);
	road2.stroke = rgb(89, 89, 89);
	road2.strokeWidth = 2;
	road2.rotate(-45, 0, 0f);
	road2.x = maxWidth - maxR - 10;
	road2.y = maxR - 10;
	//let roadNetwork = combinePaths(road1, road2, 300, 1000);
	//let debug = highlightIntersections(road1, road2);
	//roadNetwork.fill = rgb(238, 238, 238);
	//roadNetwork.stroke = rgb(89, 89, 89);
	//roadNetwork.strokeWidth = 2;
endsvggroup

svggroup debugCross():
	let a = arrow(Direction.South, 450, 450, 0.5f, 0.5f);
	//a.isVisible = false;
	a.x = 25;
	a.y = 25;
	a.fill = alpha(red, 0.5f);
	let b = arrow(Direction.West, 450, 450, 0.5f, 0.5f);
	b.x = 25;
	b.y = 25;
	b.fill = alpha(blue, 0.5f);
	let debug = highlightIntersectionOp(b, a);
	width = 500;
	height = 500;
endsvggroup

svggroup debugCross2():
	let a = arrow(Direction.South, 450, 450, 0.5f, 0.5f);
	//a.isVisible = false;
	a.x = 25;
	a.y = 25;
	a.fill = alpha(red, 0.5f);
	let b = arrow(Direction.West, 450, 450, 0.5f, 0.5f);
	b.x = 25;
	b.y = 25;
	b.fill = alpha(blue, 0.5f);
	let debug = highlightIntersectionOp2(b, a);
	width = 500;
	height = 500;
endsvggroup

slide a:
	let d = new debugCross();
	let dContainer = new SVGContainer(d);
	dContainer.orientation = Horizontal.Left | Vertical.Stretch;
	dContainer.width = 50%;
	let d2 = new debugCross2();
	let dContainer2 = new SVGContainer(d2);
	dContainer2.orientation = Horizontal.Right | Vertical.Stretch;
	dContainer2.width = 50%;
endslide

////TODO! This should work! And during binding is everything fine!
////But i guess the evaluator doesn't get the unsorted message!
////Problem is that debugCross is used before it is defined..
//svggroup debugCross():
//	let rect1 = new Rect(0, 40, 100, 20).toPath();
//	let rect2 = new Rect(40, 0, 20, 100).toPath();
//	let debug = highlightIntersections(rect1, rect2, 0, 0);
//	width = 100;
//	height = 100;
//endsvggroup
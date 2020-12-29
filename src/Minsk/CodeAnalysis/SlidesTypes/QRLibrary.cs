using Minsk.CodeAnalysis.Symbols;
using QRCoder;
using Slides;
using Slides.Data;
using Slides.Styling;
using SVGLib.ContainerElements;
using SVGLib.Parsing;
using System;
using static QRCoder.PayloadGenerator;

namespace Minsk.CodeAnalysis.SlidesTypes
{
	public static class QRLibrary
	{
		private static readonly BuiltInTypes _builtInTypes = BuiltInTypes.Instance;
		public static LibrarySymbol GetLibrary()
		{
			var name = "qr";
			var libraries = new LibrarySymbol[0];
			var customTypes = new BodySymbol[0];
			var styles = new CustomStyle[0];
			var globalVariables = new VariableValueCollection(null);
			var globalFunctions = new FunctionSymbol[]
			{
				new FunctionSymbol("urlQRCode", new VariableSymbol("data", true, PrimitiveTypeSymbol.String), _builtInTypes.LookSymbolUp(typeof(SVGTag))),
				new FunctionSymbol("urlQRCode", new VariableSymbolCollection() {
					new VariableSymbol("data", true, PrimitiveTypeSymbol.String),
					new VariableSymbol("dark", true, _builtInTypes.LookSymbolUp(typeof(Color))),
					new VariableSymbol("light", true, _builtInTypes.LookSymbolUp(typeof(Color))),
				}, _builtInTypes.LookSymbolUp(typeof(SVGTag)))
			};
			var globalFunctionReflections = new string[]
			{
				nameof(CreateURLQRCode),
				nameof(CreateURLQRCodeWithColors),
			};
			var imports = new string[0];
			var result = new LibrarySymbol(name, libraries, customTypes, styles, globalVariables, globalFunctions, globalFunctionReflections, imports);
			result.SourceType = typeof(QRLibrary);
			return result;
		}

		public static SVGTag CreateURLQRCode(string data)
		{
			return CreateURLQRCodeWithColors(data, new Color(0, 0, 0, 255), new Color(255, 255, 255, 255));
		}
		public static SVGTag CreateURLQRCodeWithColors(string data, Color dark, Color light)
		{
			var url = new Url(data);
			var qrGenerator = new QRCodeGenerator();
			var qrCodeData = qrGenerator.CreateQrCode(url.ToString(), QRCodeGenerator.ECCLevel.Q);
			var qrCode = new SvgQRCode(qrCodeData);
			var qrCodeAsSVG = qrCode.GetGraphic(1, dark.ToHex(), light.ToHex(), sizingMode: SvgQRCode.SizingMode.ViewBoxAttribute);
			SVGParser parser = new SVGParser();
			var svg = parser.FromSource(qrCodeAsSVG);
			return svg;
		}
	}
}

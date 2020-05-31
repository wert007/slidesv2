using Slides;
using Slides.Debug;
using SVGLib.Filters;
using SVGLib.Filters.Lights;
using System;
using System.Globalization;
using System.Text;

namespace HTMLWriter
{
	public static class FilterWriter
	{
		public static void Write(HTMLWriter writer, CustomFilter[] filters)
		{
			writer.StartTag("svg", id: "custom-filters");
			writer.StartTag("defs");
			foreach (var filter in filters)
			{
				Write(writer, filter);
			}
			writer.EndTag();
			writer.EndTag();
		}

		private static void Write(HTMLWriter writer, CustomFilter filter)
		{
			writer.StartTag("filter", id: filter.Id);
			for (int i = 0; i < filter.Filters.Length; i++)
			{
				var current = filter.Filters[i];
				switch (current)
				{
					case GaussianBlurFilter gaussian:
						WriteGaussianBlur(writer, filter, gaussian);
						break;
					case ColorMatrixFilter colorMatrix:
						WriteColorMatrix(writer, filter, colorMatrix);
						break;
					case ConvolveMatrixFilter convolveMatrix:
						WriteConvolveMatrix(writer, filter, convolveMatrix);
						break;
					case ComponentTransferFilter componentTransfer:
						WriteComponentTransfter(writer, filter, componentTransfer);
						break;
					case MorphologyFilter morphology:
						WriteMorphology(writer, filter, morphology);
						break;
					case FloodFilter flood:
						WriteFlood(writer, filter, flood);
						break;
					case TurbulenceFilter turbulence:
						WriteTurbulence(writer, filter, turbulence);
						break;
					case DiffuseLightingFilter diffuseLighting:
						WriteDiffuseLighting(writer, filter, diffuseLighting);
						break;
					case SpecularLightingFilter specularLighting:
						WriteSpecularLighting(writer, filter, specularLighting);
						break;
					case BlendFilter blend:
						WriteBlendFilter(writer, filter, blend);
						break;
					case CompositeFilter composite:
						WriteCompositeFilter(writer, filter, composite);
						break;
					case DisplacementMapFilter displacementMap:
						WriteDisplacementMapFilter(writer, filter, displacementMap);
						break;
					case MergeFilter merge:
						WriteMergeFilter(writer, filter, merge);
						break;
					case OffsetFilter offset:
						WriteOffsetFilter(writer, filter, offset);
						break;
					case TileFilter tile:
						WriteTileFilter(writer, filter, tile);
						break;
					default:
						Logger.LogUnknownFilter(current.GetType(), current.Name);
						break;
				}
			}
			writer.EndTag();
		}

		private static void WriteTileFilter(HTMLWriter writer, CustomFilter filter, TileFilter tile)
		{
			var input = filter.GetName(tile.Input);
			writer.PushAttribute("in", input);
			writer.PushAttribute("result", filter.GetName(tile));
			writer.WriteTag(tile.Name);
		}

		private static void WriteOffsetFilter(HTMLWriter writer, CustomFilter filter, OffsetFilter offset)
		{
			var input = filter.GetName(offset.Input);
			writer.PushAttribute("dx", ToString(offset.Dx));
			writer.PushAttribute("dy", ToString(offset.Dy));
			writer.PushAttribute("in", input);
			writer.PushAttribute("result", filter.GetName(offset));
			writer.WriteTag(offset.Name);
		}

		private static void WriteMergeFilter(HTMLWriter writer, CustomFilter filter, MergeFilter merge)
		{
			writer.PushAttribute("result", filter.GetName(merge));
			writer.StartTag(merge.Name);
			foreach (var input in merge.Inputs)
			{
				writer.PushAttribute("in", filter.GetName(input));
				writer.WriteTag("feMergeNode");
			}
			writer.EndTag();
		}

		private static void WriteDisplacementMapFilter(HTMLWriter writer, CustomFilter filter, DisplacementMapFilter displacementMap)
		{
			var input1 = filter.GetName(displacementMap.Input1);
			var input2 = filter.GetName(displacementMap.Input2);
			writer.PushAttribute("yChannelSelector", ToString(displacementMap.YChannelSelector));
			writer.PushAttribute("xChannelSelector", ToString(displacementMap.XChannelSelector));
			writer.PushAttribute("scale", ToString(displacementMap.Scale));
			writer.PushAttribute("in", input1);
			writer.PushAttribute("in2", input2);
			writer.PushAttribute("result", filter.GetName(displacementMap));
			writer.WriteTag(displacementMap.Name);
		}

		private static void WriteCompositeFilter(HTMLWriter writer, CustomFilter filter, CompositeFilter composite)
		{
			var input1 = filter.GetName(composite.Input1);
			var input2 = filter.GetName(composite.Input2);
			writer.PushAttribute("operator", ToString(composite.Operator));
			writer.PushAttribute("in", input1);
			writer.PushAttribute("in2", input2);
			writer.PushAttribute("result", filter.GetName(composite));
			writer.WriteTag(composite.Name);

		}

		private static void WriteBlendFilter(HTMLWriter writer, CustomFilter filter, BlendFilter blend)
		{
			var input1 = filter.GetName(blend.Input1);
			var input2 = filter.GetName(blend.Input2);
			writer.PushAttribute("mode", ToString(blend.Mode));
			writer.PushAttribute("in", input1);
			writer.PushAttribute("in2", input2);
			writer.PushAttribute("result", filter.GetName(blend));
			writer.WriteTag(blend.Name);
		}

		private static void WriteSpecularLighting(HTMLWriter writer, CustomFilter filter, SpecularLightingFilter specularLighting)
		{
			var inputFilter = specularLighting.Input;
			var input = filter.GetName(inputFilter);
			writer.PushAttribute("specularConstant", ToString(specularLighting.SpecularConstant));
			writer.PushAttribute("specularExponent", ToString(specularLighting.SpecularExponent));
			writer.PushAttribute("surfaceScale", ToString(specularLighting.SurfaceScale));
			writer.PushAttribute("in", input);
			writer.PushAttribute("result", filter.GetName(specularLighting));
			writer.StartTag(specularLighting.Name);
			WriteLight(writer, specularLighting.Child);
			writer.EndTag();

		}

		private static void WriteDiffuseLighting(HTMLWriter writer, CustomFilter filter, DiffuseLightingFilter diffuseLighting)
		{
			var inputFilter = diffuseLighting.Input;
			var input = filter.GetName(inputFilter);
			writer.PushAttribute("surfaceScale", ToString(diffuseLighting.SurfaceScale));
			writer.PushAttribute("diffuseConstant", ToString(diffuseLighting.DiffuseConstant));
			writer.PushAttribute("in", input);
			writer.PushAttribute("result", filter.GetName(diffuseLighting));
			writer.StartTag(diffuseLighting.Name);
			WriteLight(writer, diffuseLighting.Child);
			writer.EndTag();
		}

		private static void WriteLight(HTMLWriter writer, Light light)
		{
			switch (light)
			{
				case PointLight point:
					WritePointLight(writer, point);
					break;
				case DistantLight distant:
					WriteDistantLight(writer, distant);
					break;
				case SpotLight spot:
					WriteSpotLight(writer, spot);
					break;
				default:
					throw new Exception();
			}

		}

		private static void WriteSpotLight(HTMLWriter writer, SpotLight spot)
		{
			writer.PushAttribute("x", ToString(spot.X));
			writer.PushAttribute("y", ToString(spot.Y));
			writer.PushAttribute("z", ToString(spot.Z));
			writer.PushAttribute("pointAtX", ToString(spot.PointAtX));
			writer.PushAttribute("pointAtY", ToString(spot.PointAtY));
			writer.PushAttribute("pointAtZ", ToString(spot.PointAtZ));
			writer.PushAttribute("specularExponent", ToString(spot.SpecularExponent));
			writer.PushAttribute("limitingConeAngle", ToString(spot.LimitingConeAngle));
			writer.WriteTag(spot.Name);

		}

		private static void WriteDistantLight(HTMLWriter writer, DistantLight distant)
		{
			writer.PushAttribute("azimuth", ToString(distant.Azimuth));
			writer.PushAttribute("elevation", ToString(distant.Elevation));
			writer.WriteTag(distant.Name);
		}

		private static void WritePointLight(HTMLWriter writer, PointLight point)
		{
			writer.PushAttribute("x", ToString(point.X));	
			writer.PushAttribute("y", ToString(point.Y));	
			writer.PushAttribute("z", ToString(point.Z));
			writer.WriteTag(point.Name);
		}

		private static void WriteTurbulence(HTMLWriter writer, CustomFilter filter, TurbulenceFilter turbulence)
		{
			writer.PushAttribute("baseFrequency", ToString(turbulence.BaseFrequencyX, turbulence.BaseFrequencyY));
			writer.PushAttribute("numOctaves", ToString(turbulence.NumOctaves));
			writer.PushAttribute("seed", ToString(turbulence.Seed));
			writer.PushAttribute("stitchTiles", ToString(turbulence.StitchTiles));
			writer.PushAttribute("type", ToString(turbulence.Type));
			writer.PushAttribute("result", filter.GetName(turbulence));
			writer.WriteTag(turbulence.Name);
		}

		private static void WriteFlood(HTMLWriter writer, CustomFilter filter, FloodFilter flood)
		{
			writer.PushAttribute("flood-color", ToString(flood.FloodColor));
			writer.PushAttribute("flood-opacity", ToString(flood.FloodOpacity));
			writer.PushAttribute("result", filter.GetName(flood));
			writer.WriteTag(flood.Name);
		}


		private static void WriteMorphology(HTMLWriter writer, CustomFilter filter, MorphologyFilter morphology)
		{
			var inputFilter = morphology.Input;
			var input = filter.GetName(inputFilter);
			writer.PushAttribute("operator", ToString(morphology.Operator));
			writer.PushAttribute("radius", ToString(morphology.Radius));
			writer.PushAttribute("in", input);
			writer.PushAttribute("result", filter.GetName(morphology));
			writer.WriteTag(morphology.Name);

		}

		private static void WriteComponentTransfter(HTMLWriter writer, CustomFilter filter, ComponentTransferFilter componentTransfer)
		{
			var inputFilter = componentTransfer.Input;
			var input = filter.GetName(inputFilter);
			writer.PushAttribute("in", input);
			writer.PushAttribute("result", filter.GetName(componentTransfer));
			writer.StartTag(componentTransfer.Name);
			WriteComponentTransferChild(writer, "feFuncR", componentTransfer.Red);
			WriteComponentTransferChild(writer, "feFuncG", componentTransfer.Green);
			WriteComponentTransferChild(writer, "feFuncB", componentTransfer.Blue);
			WriteComponentTransferChild(writer, "feFuncA", componentTransfer.Alpha);
			writer.EndTag();
		}

		private static void WriteComponentTransferChild(HTMLWriter writer, string tag, ComponentTransferFilterChild child)
		{
			writer.PushAttribute("type", ToString(child.Type));
			switch (child.Type)
			{
				case ComponentTransferType.Identity:
					break;
				case ComponentTransferType.Linear:
					writer.PushAttribute("intercept", ToString(child.Intercept));
					break;
				case ComponentTransferType.Gamma:
					writer.PushAttribute("amplitude", ToString(child.Amplitude));
					writer.PushAttribute("exponent", ToString(child.Exponent));
					writer.PushAttribute("offset", ToString(child.Offset));
					break;
				case ComponentTransferType.Table:
				case ComponentTransferType.Discrete:
					writer.PushAttribute("tableValues", ToString(child.TableValues));
					break;
				default:
					throw new Exception();
			}
			writer.WriteTag(tag);
		}


		private static void WriteConvolveMatrix(HTMLWriter writer, CustomFilter filter, ConvolveMatrixFilter convolveMatrix)
		{
			var inputFilter = convolveMatrix.Input;
			var input = filter.GetName(inputFilter);
			writer.PushAttribute("bias", ToString(convolveMatrix.Bias));
			writer.PushAttribute("divisor", ToString(convolveMatrix.Divisor));
			writer.PushAttribute("edgeMode", ToString(convolveMatrix.EdgeMode));
			writer.PushAttribute("kernelMatrix", ToString(convolveMatrix.Matrix));
			writer.PushAttribute("order", ToString(convolveMatrix.OrderX, convolveMatrix.OrderY));
			writer.PushAttribute("preserveAlpha", ToString(convolveMatrix.PreserveAlpha));
			writer.PushAttribute("targetX", ToString(convolveMatrix.TargetX));
			writer.PushAttribute("targetY", ToString(convolveMatrix.TargetY));
			writer.PushAttribute("in", input);
			writer.PushAttribute("result", filter.GetName(convolveMatrix));
			writer.WriteTag(convolveMatrix.Name);

		}
		private static void WriteGaussianBlur(HTMLWriter writer, CustomFilter filter, GaussianBlurFilter gaussian)
		{
			var inputFilter = gaussian.Input;
			var input = filter.GetName(inputFilter);
			writer.PushAttribute("stdDeviation", ToString(gaussian.StdDeviationHorizontal, gaussian.StdDeviationVertical));
			writer.PushAttribute("in", input);
			writer.PushAttribute("result", filter.GetName(gaussian));
			writer.WriteTag(gaussian.Name);
		}

		private static void WriteColorMatrix(HTMLWriter writer, CustomFilter filter, ColorMatrixFilter colorMatrix)
		{

			var inputFilter = colorMatrix.Input;
			var input = filter.GetName(inputFilter);
			writer.PushAttribute("type", ToString(colorMatrix.Type));
			if (colorMatrix.Type != ColorMatrixType.LuminaceToAlpha)
			{
				string values = ToString(colorMatrix.Value); 
				if (colorMatrix.Type == ColorMatrixType.Matrix)
				{
					values = ToString(colorMatrix.Matrix); 
				}
				writer.PushAttribute("values", values);
			}
			writer.PushAttribute("in", input);
			writer.PushAttribute("result", filter.GetName(colorMatrix));
			writer.WriteTag(colorMatrix.Name);

		}

		private static string ToString(Color c)
		{
			return $"rgba({c.R}, {c.G}, {c.B}, {ToString(c.A / 255f)})";
		}

		private static string ToString(ColorMatrixType type)
		{
			switch (type)
			{
				case ColorMatrixType.Saturate:
					return "saturate";
				case ColorMatrixType.HueRotate:
					return "hueRotate";
				case ColorMatrixType.LuminaceToAlpha:
					return "luminaceToAlpha";
				case ColorMatrixType.Matrix:
					return "matrix";
				default:
					throw new Exception(); ;
			}
		}

		private static string ToString(float a, float b)
		{
			return $"{ToString(a)}, {ToString(b)}";
		}

		private static string ToString(int i)
		{
			return i.ToString();
		}

		private static string ToString(float f)
		{
			var culture = CultureInfo.CreateSpecificCulture("us-US");
			return f.ToString("0.00", culture);
		}



		private static string ToString(bool preserveAlpha)
		{
			return preserveAlpha.ToString().ToLower();
		}

		private static string ToString(Matrix matrix)
		{
			//TODO(Minor): Change that in the future..
			//To what? a very own method? hmm.
			//Maybe. Would be a little bit nicer..
			return matrix.ToString();
		}

		private static string ToString(EdgeMode edgeMode)
		{
			switch (edgeMode)
			{
				case EdgeMode.Duplicate:
					return "duplicate";
				case EdgeMode.Wrap:
					return "wrap";
				case EdgeMode.None:
					return "none";
				default:
					throw new Exception();
			}
		}

		private static string ToString(ComponentTransferType type)
		{
			switch (type)
			{
				case ComponentTransferType.Identity:
					return "identity";
				case ComponentTransferType.Table:
					return "table";
				case ComponentTransferType.Linear:
					return "linear";
				case ComponentTransferType.Gamma:
					return "gamma";
				case ComponentTransferType.Discrete:
					return "discrete";
				default:
					throw new Exception(); ;
			}
		}

		private static string ToString(float[] value)
		{
			var builder = new StringBuilder();
			var isFirst = true;
			foreach (var v in value)
			{
				if (!isFirst)
					builder.Append(" ");
				isFirst = false;
				builder.Append(ToString(v));
			}
			return builder.ToString();
		}

		private static string ToString(MorphologyOperator @operator)
		{
			switch (@operator)
			{
				case MorphologyOperator.Erode:
					return "erode";
				case MorphologyOperator.Dilate:
					return "dilate";
				default:
					throw new Exception();
			}
		}


		private static string ToString(TurbulenceType type)
		{
			switch (type)
			{
				case TurbulenceType.Turbulence:
					return "turbulence";
				case TurbulenceType.FractalNoise:
					return "fractalNoise";
				default:
					throw new Exception();
			}

		}

		private static string ToString(StitchTiles stitchTiles)
		{
			switch (stitchTiles)
			{
				case StitchTiles.NoStitch:
					return "noStitch";
				case StitchTiles.Stitch:
					return "stitch";
				default:
					throw new Exception();
			}
		}

		private static string ToString(BlendMode mode)
		{
			switch (mode)
			{
				case BlendMode.Normal:
					return "normal";
				case BlendMode.Multiply:
					return "multiply";
				case BlendMode.Screen:
					return "screen";
				case BlendMode.Overlay:
					return "overlay";
				case BlendMode.Darken:
					return "darken";
				case BlendMode.Lighten:
					return "lighten";
				case BlendMode.ColorDodge:
					return "color-dodge";
				case BlendMode.ColorBurn:
					return "color-burn";
				case BlendMode.HardLight:
					return "hard-light";
				case BlendMode.SoftLight:
					return "soft-light";
				case BlendMode.Difference:
					return "difference";
				case BlendMode.Exclusion:
					return "exclusion";
				case BlendMode.Hue:
					return "hue";
				case BlendMode.Saturation:
					return "saturation";
				case BlendMode.Color:
					return "color";
				case BlendMode.Luminosity:
					return "luminosity";
				default:
					throw new Exception();
			}
		}

		private static string ToString(CompositeOperator @operator)
		{
			switch (@operator)
			{
				case CompositeOperator.Over:
					return "over";
				case CompositeOperator.In:
					return "in";
				case CompositeOperator.Out:
					return "out";
				case CompositeOperator.Atop:
					return "atop";
				case CompositeOperator.Xor:
					return "xor";
				case CompositeOperator.Lighter:
					return "lighter";
				case CompositeOperator.Arithmetic:
					return "arithmetic";
				default:
					throw new Exception();
			}
		}


		private static string ToString(DisplacementMapChannelSelector channel)
		{
			switch (channel)
			{
				case DisplacementMapChannelSelector.R:
					return "R";
				case DisplacementMapChannelSelector.G:
					return "G";
				case DisplacementMapChannelSelector.B:
					return "B";
				case DisplacementMapChannelSelector.A:
					return "A";
				default:
					throw new Exception();
			}
		}

	}
}

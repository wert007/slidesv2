using System;

namespace Slides
{
	public abstract class Transform
	{
		public abstract TransformType Type { get; }

		public static string GetFunctionName(TransformType type)
		{
			switch (type)
			{
            case TransformType.TranslateX:
               return "translateX";
            case TransformType.TranslateY:
               return "translateY";
            case TransformType.TranslateZ:
               return "translateZ";
            case TransformType.ScaleX:
               return "scaleX";
            case TransformType.ScaleY:
               return "scaleY";
            case TransformType.ScaleZ:
               return "scaleZ";
            case TransformType.RotateX:
               return "rotateX";
            case TransformType.RotateY:
               return "rotateY";
            case TransformType.RotateZ:
               return "rotateZ";
            case TransformType.SkewX:
               return "skewX";
            case TransformType.SkewY:
               return "skewY";
            default:
               throw new NotImplementedException();
			}
		}
	}
}

using System;

namespace Slides.Transforms
{
   public abstract class Transform
   {
      public abstract TransformKind Kind { get; }

      public static string GetFunctionName(TransformKind kind)
      {
         switch (kind)
         {
            case TransformKind.TranslateX:
               return "translateX";
            case TransformKind.TranslateY:
               return "translateY";
            case TransformKind.TranslateZ:
               return "translateZ";
            case TransformKind.ScaleX:
               return "scaleX";
            case TransformKind.ScaleY:
               return "scaleY";
            case TransformKind.ScaleZ:
               return "scaleZ";
            case TransformKind.RotateX:
               return "rotateX";
            case TransformKind.RotateY:
               return "rotateY";
            case TransformKind.RotateZ:
               return "rotateZ";
            case TransformKind.SkewX:
               return "skewX";
            case TransformKind.SkewY:
               return "skewY";
            default:
               throw new NotImplementedException();
         }
      }
   }
}

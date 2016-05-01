﻿using System.Diagnostics;
using System.Linq;
using System.Svg.Transforms;

namespace System.Svg.Render.EPL
{
  public class SvgTextTranslator : SvgElementTranslator<SvgText>
  {
    public SvgTextTranslator(SvgUnitCalculator svgUnitCalculator)
    {
      if (svgUnitCalculator == null)
      {
        // TODO add documentation
        throw new ArgumentNullException(nameof(svgUnitCalculator));
      }

      this.SvgUnitCalculator = svgUnitCalculator;
    }

    private SvgUnitCalculator SvgUnitCalculator { get; }

    public override object Translate(SvgText instance)
    {
      var horizontalStart = this.SvgUnitCalculator.GetValue(instance.X.First());
      var verticalStart = this.SvgUnitCalculator.GetValue(instance.Y.First());

      int rotation;
      var rotationTransformation = instance.Transforms.OfType<SvgRotate>()
                                           .FirstOrDefault();
      if (rotationTransformation == null)
      {
        rotation = 0;
      }
      else
      {
        // TODO adapt the horizontalStart and verticalStart if rotation's origin is center
        // TODO find a good value for TOLERANCE
        if (Math.Abs(rotationTransformation.Angle - 90) < 0.5f)
        {
          rotation = 1;
        }
        else if (Math.Abs(rotationTransformation.Angle - 180) < 0.5f)
        {
          rotation = 2;
        }
        else if (Math.Abs(rotationTransformation.Angle - 270) < 0.5f)
        {
          rotation = 3;
        }
        else
        {
          Trace.TraceError($@"Could not translate {nameof(SvgText)}, as {nameof(rotationTransformation.Angle)}:{rotationTransformation.Angle} could not be mapped");
          return string.Empty;
        }
      }

      // TODO here comes the magic!
      var fontSelection = 1;
      var horizontalMultiplier = 1;
      var verticalMultiplier = 1;

      var reverseImage = "N";

      var text = instance.Text;

      var translation = $@"A{horizontalStart},{verticalStart},{rotation},{fontSelection},{horizontalMultiplier},{verticalMultiplier},{reverseImage},""{text}""";

      return translation;
    }
  }
}
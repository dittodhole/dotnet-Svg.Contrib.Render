﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Svg;
using JetBrains.Annotations;

namespace Svg.Contrib.Render.ZPL
{
  [PublicAPI]
  public class SvgLineTranslator : SvgElementTranslatorBase<SvgLine>
  {
    public SvgLineTranslator([NotNull] ZplTransformer zplTransformer,
                             [NotNull] ZplCommands zplCommands)
    {
      this.ZplTransformer = zplTransformer;
      this.ZplCommands = zplCommands;
    }

    [NotNull]
    protected ZplTransformer ZplTransformer { get; }

    [NotNull]
    protected ZplCommands ZplCommands { get; }

    public override void Translate([NotNull] SvgLine svgElement,
                                   [NotNull] Matrix matrix,
                                   [NotNull] Container<ZplStream> container)
    {
      float startX;
      float startY;
      float endX;
      float endY;
      float strokeWidth;
      this.ZplTransformer.Transform(svgElement,
                                    matrix,
                                    out startX,
                                    out startY,
                                    out endX,
                                    out endY,
                                    out strokeWidth);

      // TODO find a good TOLERANCE
      if (Math.Abs(startY - endY) < 0.5f
          || Math.Abs(startX - endX) < 0.5f)
      {
        LineColor lineColor;
        var strokeShouldBeWhite = (svgElement.Stroke as SvgColourServer)?.Colour == Color.White;
        if (strokeShouldBeWhite)
        {
          lineColor = LineColor.White;
        }
        else
        {
          lineColor = LineColor.Black;
        }

        var horizontalStart = (int) startX;
        var verticalStart = (int) endY;
        var width = (int) (endX - startX);
        var height = (int) (endY - startY);
        var thickness = (int) strokeWidth;

        container.Body.Add(this.ZplCommands.FieldTypeset(horizontalStart,
                                                         verticalStart));
        container.Body.Add(this.ZplCommands.GraphicBox(width,
                                                       height,
                                                       thickness,
                                                       lineColor));
      }
      else
      {
        // TODO
        throw new NotImplementedException();
      }
    }
  }
}
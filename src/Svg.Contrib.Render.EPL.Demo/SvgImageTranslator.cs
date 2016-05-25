﻿using System.Drawing.Drawing2D;
using Svg;
using JetBrains.Annotations;

// ReSharper disable ExceptionNotDocumentedOptional
// ReSharper disable NonLocalizedString

namespace Svg.Contrib.Render.EPL.Demo
{
  [PublicAPI]
  public class SvgImageTranslator : EPL.SvgImageTranslator
  {
    // Q: why the fuck are barcodes implemented this way?
    // A: well, barcodes do not exist in the svg-spec, so the
    //    SvgImage is hijacked for this by setting the value
    //    in "data-barcode"-attribute. additional positioning
    //    and barcode-mode is done rather in the code, than
    //    further misuse the attribute, which might not be a
    //    reusable abstraction for multiple printer languages.
    //    in short: yes! you have to get your hands dirty...

    public SvgImageTranslator([NotNull] EPL.EplTransformer eplTransformer,
                              [NotNull] EplCommands eplCommands)
      : base(eplTransformer,
             eplCommands) {}

    public override void Translate([NotNull] SvgImage svgElement,
                                   [NotNull] Matrix matrix,
                                   [NotNull] Container<EplStream> container)
    {
      if (svgElement.HasNonEmptyCustomAttribute("data-barcode"))
      {
        var barcode = svgElement.CustomAttributes["data-barcode"];

        float startX;
        float startY;
        float endX;
        float endY;
        float sourceAlignmentWidth;
        float sourceAlignmentHeight;
        this.EplTransformer.Transform(svgElement,
                                      matrix,
                                      out startX,
                                      out startY,
                                      out endX,
                                      out endY,
                                      out sourceAlignmentWidth,
                                      out sourceAlignmentHeight);

        var sector = this.EplTransformer.GetRotationSector(matrix);

        var horizontalStart = (int) startX;
        var verticalStart = (int) startY;
        var height = (int) sourceAlignmentHeight;

        if (sector % 2 > 0)
        {
          horizontalStart += height;
        }

        this.AdaptPositionOfBarcode(svgElement,
                                    horizontalStart,
                                    verticalStart,
                                    out horizontalStart,
                                    out verticalStart);

        BarCodeSelection barCodeSelection;
        int narrowBarWidth;
        int wideBarWidth;
        PrintHumanReadable printHumanReadable;
        if (this.TryGetBarCodeSelection(svgElement,
                                        out barCodeSelection,
                                        out narrowBarWidth,
                                        out wideBarWidth,
                                        out printHumanReadable))
        {
          container.Body.Add(this.EplCommands.BarCode(horizontalStart,
                                                      verticalStart,
                                                      sector,
                                                      barCodeSelection,
                                                      narrowBarWidth,
                                                      wideBarWidth,
                                                      height,
                                                      printHumanReadable,
                                                      barcode));
          return;
        }
      }

      base.Translate(svgElement,
                     matrix,
                     container);
    }

    private void AdaptPositionOfBarcode([NotNull] SvgImage svgImage,
                                        int horizontalStart,
                                        int verticalStart,
                                        out int newHorizontalStart,
                                        out int newVerticalStart)
    {
      if (svgImage.ID == "CargoIdBc")
      {
        newHorizontalStart = horizontalStart - 30;
        newVerticalStart = verticalStart;
      }
      else if (svgImage.ID == "RouteBc")
      {
        newHorizontalStart = horizontalStart;
        newVerticalStart = verticalStart + 35;
      }
      else if (svgImage.ID == "ReceiverBc")
      {
        newHorizontalStart = horizontalStart;
        newVerticalStart = verticalStart;
      }
      else
      {
        newHorizontalStart = horizontalStart;
        newVerticalStart = verticalStart;
      }
    }

    [Pure]
    [MustUseReturnValue]
    private bool TryGetBarCodeSelection([NotNull] SvgImage svgImage,
                                        out BarCodeSelection barCodeSelection,
                                        out int narrowBarWidth,
                                        out int wideBarWidth,
                                        out PrintHumanReadable printHumanReadable)
    {
      if (svgImage.ID == "CargoIdBc")
      {
        barCodeSelection = BarCodeSelection.Interleaved2Of5;
        narrowBarWidth = 4;
        wideBarWidth = 8;
        printHumanReadable = PrintHumanReadable.No;
        return true;
      }
      if (svgImage.ID == "RouteBc")
      {
        barCodeSelection = BarCodeSelection.Code128Auto;
        narrowBarWidth = 3;
        wideBarWidth = 4;
        printHumanReadable = PrintHumanReadable.No;
        return true;
      }
      if (svgImage.ID == "ReceiverBc")
      {
        barCodeSelection = BarCodeSelection.Code128Auto;
        narrowBarWidth = 1;
        wideBarWidth = 1;
        printHumanReadable = PrintHumanReadable.No;
        return true;
      }

      barCodeSelection = BarCodeSelection.Code128A;
      narrowBarWidth = 0;
      wideBarWidth = 0;
      printHumanReadable = PrintHumanReadable.No;
      return false;
    }
  }
}
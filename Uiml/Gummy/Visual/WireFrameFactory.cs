using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Kernel.Services;
using Uiml.Gummy.Kernel;
using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Domain;

using Shape;

namespace Uiml.Gummy.Visual
{
    public class WireFrameFactory
    {
        public static Color[] randomColors = new Color[]{                        
            Color.DarkBlue,
            Color.DarkCyan,
            Color.DarkGoldenrod,
            Color.DarkGray,
            Color.DarkGreen,
            Color.DarkKhaki,
            Color.DarkMagenta,
            Color.DarkOliveGreen,
            Color.DarkOrange,
            Color.DarkOrchid,
            Color.DarkRed,
            Color.DarkSalmon,
            Color.DarkSeaGreen,
            Color.DarkSlateBlue,
            Color.DarkSlateGray,
            Color.DarkTurquoise,
            Color.DarkViolet,
            Color.Bisque,
            Color.Black,
            Color.BlanchedAlmond,
            Color.Blue,
            Color.BlueViolet,
            Color.Brown,
            Color.BurlyWood,
            Color.CadetBlue,
            Color.Chartreuse,
            Color.Chocolate,
            Color.Coral,
            Color.CornflowerBlue,
            Color.Cornsilk,
            Color.Crimson,
            Color.Cyan,
            Color.DeepPink,
            Color.DeepSkyBlue,
            Color.DimGray,
            Color.DodgerBlue,
            Color.Firebrick,
            Color.FloralWhite,
            Color.ForestGreen,
            Color.Fuchsia,
            Color.Gainsboro,
            Color.GhostWhite,
            Color.Gold,
            Color.Goldenrod,
            Color.Gray,
            Color.Green,
            Color.GreenYellow,
            Color.Honeydew,
            Color.HotPink,
            Color.IndianRed,
            Color.Indigo,
            Color.Ivory,
            Color.Khaki,
            Color.Lavender,
            Color.LavenderBlush,
            Color.LawnGreen,
            Color.LemonChiffon,
            Color.LightBlue,
            Color.LightCoral,
            Color.LightCyan,
            Color.LightGoldenrodYellow,
            Color.LightGray,
            Color.LightGreen,
            Color.LightPink,
            Color.LightSalmon,
            Color.LightSeaGreen,
            Color.LightSkyBlue,
            Color.LightSlateGray,
            Color.LightSteelBlue,
            Color.LightYellow,
            Color.Lime,
            Color.LimeGreen,
            Color.Linen,
            Color.Magenta,
            Color.Maroon,
            Color.MediumAquamarine,
            Color.MediumBlue,
            Color.MediumOrchid,
            Color.MediumPurple,
            Color.MediumSeaGreen,
            Color.MediumSlateBlue,
            Color.MediumSpringGreen,
            Color.MediumTurquoise,
            Color.MediumVioletRed,
            Color.MidnightBlue,
            Color.MintCream,
            Color.MistyRose,
            Color.Moccasin,
            Color.NavajoWhite};

        public static List<Shape.Line> GetWireFrames(Size size)
        {
            List<Shape.Line> lines = new List<Shape.Line>();
            
            Dictionary<string, DomainObject> example = ExampleRepository.Instance.GetExample(size);
            if (example != null)
            {
                Dictionary<string, DomainObject>.Enumerator enumerator = example.GetEnumerator();

                CanvasService service = (CanvasService)DesignerKernel.Instance.GetService("gummy-canvas");

                int index = 0;
                while (enumerator.MoveNext())
                {
                    DomainObject domObject = enumerator.Current.Value;
                    //ExampleRepository.Instance.GetDomainObjectExample(domObject.Identifier, service.CanvasSize).Color = randomColors[index];
                    DesignerKernel.Instance.CurrentDocument.DomainObjects.Get(domObject.Identifier).Color = randomColors[index];
                    if(SelectedDomainObject.Instance.Selected != null && SelectedDomainObject.Instance.Selected.Identifier == domObject.Identifier)
                        lines.AddRange(GetWireFrames(domObject, DomainObject.SELECTED_COLOR));
                    else
                        lines.AddRange(GetWireFrames(domObject, DomainObject.UNSELECTED_COLOR));
                    index++;
                }
            }

            return lines;
        }

        public static List<Shape.Line> GetWireFrames(DomainObject dom, Color color)
        {
            List<Line> lines = new List<Line>();
            Line line1 = new Line(dom.Size.Width, Orientation.HORIZONTAL, color);
            line1.Label = dom.Identifier;
            Line line2 = new Line(dom.Size.Height + 2, Orientation.VERTICAL, color);
            line2.Label = dom.Identifier;
            Line line3 = new Line(dom.Size.Width, Orientation.HORIZONTAL, color);
            line3.Label = dom.Identifier;
            Line line4 = new Line(dom.Size.Height, Orientation.VERTICAL, color);
            line4.Label = dom.Identifier;
            line1.Location = new Point(dom.Location.X - 1, dom.Location.Y - 1);
            line2.Location = new Point(dom.Location.X - 1 + dom.Size.Width, dom.Location.Y - 1);
            line3.Location = new Point(dom.Location.X - 1, dom.Location.Y - 1 + dom.Size.Height);
            line4.Location = new Point(dom.Location.X - 1, dom.Location.Y - 1);

            lines.Add(line1);
            lines.Add(line2);
            lines.Add(line3);
            lines.Add(line4);

            return lines;
        }
    }
}

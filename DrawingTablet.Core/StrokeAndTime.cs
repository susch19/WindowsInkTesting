using System;
using System.Windows.Ink;

namespace DrawingTablet.Core
{
    public class StrokeAndTime
    {
        //public Stroke Stroke { get; set; }
        public byte[] Stroke { get; set; }
        //public StrokeCollection Strokes { get; set; }
        public DateTime DateTime { get; set; }
    }
}
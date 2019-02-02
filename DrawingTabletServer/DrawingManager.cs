using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrawingTablet.Core;

namespace DrawingTabletServer
{
    public class DrawingManager
    {
        List<StrokeAndTime> Strokes;

        public DrawingManager()
        {
            Strokes = new List<StrokeAndTime>();
        }

        internal List<byte[]> GetLatest(DateTime time)
        {
            return Strokes.Where(x => x.DateTime > time).Select(x => x.Stroke).ToList();
        }

        internal List<byte[]> GetAll() => Strokes.Select(x => x.Stroke).ToList();

        internal void StoreNewest(byte[] drawings, DateTime dateTime)
        {
            Strokes.Add(new StrokeAndTime { Stroke = drawings, DateTime = dateTime });
        }
    }
}
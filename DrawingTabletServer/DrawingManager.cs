using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrawingTablet.Core;
using static DrawingTablet.Core.StrokesAndTime;

namespace DrawingTabletServer
{
    public class DrawingManager
    {
        private List<StrokeAndTime> Strokes;
        //public StrokeCollection StrokeCollection { get; private set; }
        public List<(ActionType, byte[])> StrokeHistory;
        public DrawingManager()
        {
            Strokes = new List<StrokeAndTime>();
            //StrokeCollection = new StrokeCollection();
            StrokeHistory = new List<(ActionType, byte[])>();
        }

        internal List<byte[]> GetLatest(DateTime time) => Strokes.Where(x => x.DateTime > time).Select(x => x.Stroke).ToList();

        internal List<byte[]> GetAll() => Strokes.Select(x => x.Stroke).ToList();

        internal void StoreNewest(byte[] drawings, DateTime dateTime)
        {
            Strokes.Add(new StrokeAndTime { Stroke = drawings, DateTime = dateTime });
            StoreNewStroke(drawings);
        }

        internal void StoreNewStroke(byte[] stroke) => StrokeHistory.Add((ActionType.Add, stroke));

        internal void ChangeStrokes(byte[] strokes, ActionType actionType)
        {
            StrokeHistory.Add((actionType, strokes));//using (var ms = new MemoryStream(strokes))//using (var eraseMS = new MemoryStream(erase))//{//    var strokesToReplace = new StrokeCollection(ms);//    var eraseResult = new StrokeCollection(eraseMS);//    if (eraseResult.Count > 0)//        StrokeCollection.Replace(strokesToReplace, eraseResult);//    else//        StrokeCollection.Remove(strokesToReplace);//}
        }

        internal List<(ActionType, byte[])> GetStrokeCollection() =>
            //using (var ms = new MemoryStream())
            //{
            //    StrokeCollection.Save(ms);
            //    return ms.ToArray();
            //}
            StrokeHistory;
    }
}
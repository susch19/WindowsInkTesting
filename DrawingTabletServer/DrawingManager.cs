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
        public List<(ActionType, byte[], byte[])> StrokeHistory;

        private string sessionFileName;
        private SemaphoreSlimExtended writeLock;
        public event EventHandler<List<(ActionType, byte[], byte[])>> DrawingChanged;

        public DrawingManager() : this($"session_{Guid.NewGuid().ToString()}_{DateTime.Now.ToShortDateString().Replace('/', '_')}.drf")
        {

        }

        public DrawingManager(string drawing)
        {
            Strokes = new List<StrokeAndTime>();
            writeLock = new SemaphoreSlimExtended(1, 1);
            sessionFileName = Path.Combine("SavedDrawings", drawing);
            if (!Directory.Exists("SavedDrawings"))
                Directory.CreateDirectory("SavedDrawings");
            StrokeHistory = new List<(ActionType, byte[], byte[])>();

            ReadIntoStrokeHistory(sessionFileName);
        }

        internal List<byte[]> GetLatest(DateTime time) => Strokes.Where(x => x.DateTime > time).Select(x => x.Stroke).ToList();

        internal List<byte[]> GetAll() => Strokes.Select(x => x.Stroke).ToList();

        internal void StoreNewest(byte[] drawings, DateTime dateTime)
        {
            var stroke = new StrokeAndTime { Stroke = drawings, DateTime = dateTime };

            Strokes.Add(stroke);
            StoreNewStroke(drawings);
        }

        internal void StoreNewStroke(byte[] stroke) => AddToStrikeHistory(stroke, new byte[0], ActionType.Add);

        internal void ChangeStrokes(byte[] strokes, byte[] changed, ActionType actionType)
        {
            AddToStrikeHistory(strokes, changed, actionType);
            //using (var ms = new MemoryStream(strokes))//using (var eraseMS = new MemoryStream(erase))//{//    var strokesToReplace = new StrokeCollection(ms);//    var eraseResult = new StrokeCollection(eraseMS);//    if (eraseResult.Count > 0)//        StrokeCollection.Replace(strokesToReplace, eraseResult);//    else//        StrokeCollection.Remove(strokesToReplace);//}
        }

        internal void AddToStrikeHistory(byte[] strokes, byte[] changed, ActionType actionType)
        {
            using (writeLock.Wait())
            {
                if (!Directory.Exists("SavedDrawings"))
                    Directory.CreateDirectory("SavedDrawings");
                if (!File.Exists(sessionFileName))
                    File.Create(sessionFileName).Close();

                using (var fs = new FileStream(sessionFileName, FileMode.Append))
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write((int)actionType);
                    bw.Write(strokes.Length);
                    bw.Write(strokes);
                    bw.Write(changed.Length);
                    bw.Write(changed);
                }
            }

            StrokeHistory.Add((actionType, strokes, changed));
        }

        internal List<(ActionType, byte[], byte[])> GetStrokeCollection() =>
            //using (var ms = new MemoryStream())
            //{
            //    StrokeCollection.Save(ms);
            //    return ms.ToArray();
            //}
            StrokeHistory;

        public static IList<string> ExistingDrawings()
        {
            if (Directory.Exists("SavedDrawings"))
                return Directory.GetFiles("SavedDrawings").Select(x => x.Split(Path.DirectorySeparatorChar).Last()).ToList();
            else return new List<string>();
        }

        public void ChangeDrawing(string fileName)
        {
            sessionFileName = Path.Combine("SavedDrawings", fileName);
            ReadIntoStrokeHistory(sessionFileName);
        }

        private void ReadIntoStrokeHistory(string path)
        {
            StrokeHistory.Clear();

            if (File.Exists(path))
                using (var fs = new FileStream(path, FileMode.Open))
                using (var br = new BinaryReader(fs))
                    while (br.BaseStream.Position < br.BaseStream.Length)
                        StrokeHistory.Add(((ActionType)br.ReadInt32(), br.ReadBytes(br.ReadInt32()), br.ReadBytes(br.ReadInt32())));
            DrawingChanged?.Invoke(this, StrokeHistory);
        }
    }
}
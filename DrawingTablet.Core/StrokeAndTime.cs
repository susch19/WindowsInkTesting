using System;
using System.IO;
using System.Windows.Ink;

namespace DrawingTablet.Core
{
    public class StrokeAndTime
    {
        //public Stroke Stroke { get; set; }
        public byte[] Stroke { get; set; }
        //public StrokeCollection Strokes { get; set; }
        public DateTime DateTime { get; set; }

        public void Serialize(BinaryWriter fileStream)
        {
            fileStream.Write(DateTime.ToBinary());
            fileStream.Write(Stroke.Length);
            fileStream.Write(Stroke);
        }

        public static StrokeAndTime Deserialize(BinaryReader streamReader)
        => new StrokeAndTime()
        {
            DateTime = DateTime.FromBinary(streamReader.ReadInt64()),
            Stroke = streamReader.ReadBytes(streamReader.ReadInt32())
        };

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input.StylusPlugIns;

namespace TabletTestWPF
{
    public static class ExtensionHelperMethods
    {
        public static DynamicRenderer Clone(this DynamicRenderer renderer)
        {
            var dr = new DynamicRenderer();
            dr.DrawingAttributes = renderer.DrawingAttributes.Clone();
            return dr;
        }
    }
}

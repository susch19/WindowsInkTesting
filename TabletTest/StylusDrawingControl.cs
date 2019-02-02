using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Input;

namespace TabletTest
{
    public partial class StylusDrawingControl : IContainerControl, IInputElement
    {
        public StylusDrawingControl()
        {
            InitializeComponent();
        }

        public bool IsMouseOver => throw new NotImplementedException();

        public bool IsMouseDirectlyOver => throw new NotImplementedException();

        public bool IsMouseCaptured => throw new NotImplementedException();

        public bool IsStylusOver => throw new NotImplementedException();

        public bool IsStylusDirectlyOver => throw new NotImplementedException();

        public bool IsStylusCaptured => throw new NotImplementedException();

        public bool IsKeyboardFocusWithin => throw new NotImplementedException();

        public bool IsKeyboardFocused => throw new NotImplementedException();

        public bool IsEnabled => throw new NotImplementedException();

        public bool Focusable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Control ActiveControl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event MouseButtonEventHandler PreviewMouseLeftButtonDown;
        public event MouseButtonEventHandler MouseLeftButtonDown;
        public event MouseButtonEventHandler PreviewMouseLeftButtonUp;
        public event MouseButtonEventHandler MouseLeftButtonUp;
        public event MouseButtonEventHandler PreviewMouseRightButtonDown;
        public event MouseButtonEventHandler MouseRightButtonDown;
        public event MouseButtonEventHandler PreviewMouseRightButtonUp;
        public event MouseButtonEventHandler MouseRightButtonUp;
        public event System.Windows.Input.MouseEventHandler PreviewMouseMove;
        public event MouseWheelEventHandler PreviewMouseWheel;
        public event System.Windows.Input.MouseEventHandler GotMouseCapture;
        public event System.Windows.Input.MouseEventHandler LostMouseCapture;
        public event StylusDownEventHandler PreviewStylusDown;
        public event StylusDownEventHandler StylusDown;
        public event StylusEventHandler PreviewStylusUp;
        public event StylusEventHandler StylusUp;
        public event StylusEventHandler PreviewStylusMove;
        public event StylusEventHandler StylusMove;
        public event StylusEventHandler PreviewStylusInAirMove;
        public event StylusEventHandler StylusInAirMove;
        public event StylusEventHandler StylusEnter;
        public event StylusEventHandler StylusLeave;
        public event StylusEventHandler PreviewStylusInRange;
        public event StylusEventHandler StylusInRange;
        public event StylusEventHandler PreviewStylusOutOfRange;
        public event StylusEventHandler StylusOutOfRange;
        public event StylusSystemGestureEventHandler PreviewStylusSystemGesture;
        public event StylusSystemGestureEventHandler StylusSystemGesture;
        public event StylusButtonEventHandler StylusButtonDown;
        public event StylusButtonEventHandler PreviewStylusButtonDown;
        public event StylusButtonEventHandler PreviewStylusButtonUp;
        public event StylusButtonEventHandler StylusButtonUp;
        public event StylusEventHandler GotStylusCapture;
        public event StylusEventHandler LostStylusCapture;
        public event System.Windows.Input.KeyEventHandler PreviewKeyUp;
        public event KeyboardFocusChangedEventHandler PreviewGotKeyboardFocus;
        public event KeyboardFocusChangedEventHandler GotKeyboardFocus;
        public event KeyboardFocusChangedEventHandler PreviewLostKeyboardFocus;
        public event KeyboardFocusChangedEventHandler LostKeyboardFocus;
        public event TextCompositionEventHandler PreviewTextInput;
        public event TextCompositionEventHandler TextInput;

        event System.Windows.Input.MouseEventHandler IInputElement.MouseMove
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event MouseWheelEventHandler IInputElement.MouseWheel
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event System.Windows.Input.MouseEventHandler IInputElement.MouseEnter
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event System.Windows.Input.MouseEventHandler IInputElement.MouseLeave
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event System.Windows.Input.KeyEventHandler IInputElement.PreviewKeyDown
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event System.Windows.Input.KeyEventHandler IInputElement.KeyDown
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event System.Windows.Input.KeyEventHandler IInputElement.KeyUp
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public bool ActivateControl(Control active) => throw new NotImplementedException();
        public void AddHandler(RoutedEvent routedEvent, Delegate handler) => throw new NotImplementedException();
        public bool CaptureMouse() => throw new NotImplementedException();
        public bool CaptureStylus() => throw new NotImplementedException();
        public bool Focus() => throw new NotImplementedException();
        public void RaiseEvent(RoutedEventArgs e) => throw new NotImplementedException();
        public void ReleaseMouseCapture() => throw new NotImplementedException();
        public void ReleaseStylusCapture() => throw new NotImplementedException();
        public void RemoveHandler(RoutedEvent routedEvent, Delegate handler) => throw new NotImplementedException();
    }
}

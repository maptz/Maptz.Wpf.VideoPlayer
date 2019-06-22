using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unosquare.FFME;
using Unosquare.FFME.Common;
namespace Maptz.QuickVideoPlayer
{

    public class SliderWithDraggingEvents : System.Windows.Controls.Slider
    {
        public bool IsDragging { get; private set; }

        public delegate void ThumbDragStartedHandler(object sender, DragStartedEventArgs e);
        public event ThumbDragStartedHandler ThumbDragStarted;


        public delegate void ThumbDragCompletedHandler(object sender, DragCompletedEventArgs e);
        public event ThumbDragCompletedHandler ThumbDragCompleted;

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            this.IsDragging = true;
            if (ThumbDragStarted != null) ThumbDragStarted(this, e);
            base.OnThumbDragStarted(e);
        }

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            this.IsDragging = false;
            if (ThumbDragCompleted != null) ThumbDragCompleted(this, e);
            base.OnThumbDragCompleted(e);
        }
    }
}
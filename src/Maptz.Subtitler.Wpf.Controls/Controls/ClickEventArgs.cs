using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Maptz.Editing.TimeCodeDocuments;
namespace Maptz.Subtitler.Wpf.Controls.Controls
{
    public class ClickEventArgs : EventArgs
    {
        public ClickEventArgs(int? idx) => this.Index = idx;
        /* #region Public Properties */
        public int? Index { get; }
        /* #endregion Public Properties */
    }
}
using System;
namespace Maptz.Subtitler.Wpf.Controls
{
    public class ClickEventArgs : EventArgs
    {
        public ClickEventArgs(int? idx) => this.Index = idx;
        /* #region Public Properties */
        public int? Index { get; }
        /* #endregion Public Properties */
    }
}
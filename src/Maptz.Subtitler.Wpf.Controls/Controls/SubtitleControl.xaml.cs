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




    /// <summary>
    /// Interaction logic for SubtitleControl.xaml
    /// </summary>
    public partial class SubtitleControl : UserControl
    {

        public int? CursorIndex
        {
            get => this.x_TouchTextControl.CursorIndex;
            set
            {
                var oldValue = this.x_TouchTextControl.CursorIndex;
                if (oldValue != value)
                {
                    this.x_TouchTextControl.CursorIndex = value;
                    this.OnCursorIndexChanged(oldValue, value);
                }
            }
        }

        private void OnCursorIndexChanged(int? oldValue, int? value)
        {
            this.InvalidateBorders();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.SetDisplay();
        }

        private void SetDisplay()
        {
            //if (this.ActualWidth < 250)
            //{
            //    this.x_TouchTextControl.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    this.x_TouchTextControl.Visibility = Visibility.Visible;
            //}
        }


        /* #region Private Fields */
        private bool _hasCroppedLeft;
        private bool _hasCroppedRight;
        private ITimeCodeDocumentItem<string> _item;
        /* #endregion Private Fields */
        /* #region Private Methods */
        private void InvalidateBorders()
        {
            var defaultThickness = 1;
            var leftThickness = this.HasCroppedLeft ? 0 : defaultThickness;
            var rightThickness = this.HasCroppedRight ? 0 : defaultThickness;
            var thickness = new Thickness(leftThickness, defaultThickness, rightThickness, defaultThickness);
            this.x_Border_Top.BorderThickness = thickness;
            this.x_Border_Bottom.BorderThickness = thickness;

            if (this.CursorIndex.HasValue)
            {
                this.x_Border_Top.Background = new SolidColorBrush(Color.FromArgb(255,255, 0, 0));
                this.x_Border_Bottom.Background = new SolidColorBrush(Color.FromArgb(5, 255, 0, 0));
            }
            else
            {
                this.x_Border_Top.Background = new SolidColorBrush(Colors.Transparent);
                this.x_Border_Bottom.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
        private void OnHasCroppedLeftChanged(bool oldValue, bool value)
        {
            this.InvalidateBorders();
        }
        private void OnHasCroppedRightChanged(bool oldValue, bool value)
        {
            this.InvalidateBorders();
        }
        private void OnItemChanged(ITimeCodeDocumentItem<string> newValue, ITimeCodeDocumentItem<string> oldValue)
        {
            this.InvalidateText();
            
        }

        private void InvalidateText()
        {
            this.x_TouchTextControl.Text = this.Item?.Content;
        }

        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected virtual void OnClick(int? index)
        {
            var click = this.Click;
            if (click != null) click(this, new ClickEventArgs(index));
        }
        /* #endregion Protected Methods */
        /* #region Public Delegates */
        public event EventHandler<ClickEventArgs> Click;
        /* #endregion Public Delegates */
        /* #region Public Properties */
        public bool HasCroppedLeft
        {
            get => this._hasCroppedLeft;
            set
            {
                var oldValue = this._hasCroppedLeft;
                if (this._hasCroppedLeft != value)
                {
                    this._hasCroppedLeft = value;
                    this.OnHasCroppedLeftChanged(oldValue, value);
                }
            }
        }
        public bool HasCroppedRight
        {
            get => this._hasCroppedRight;
            set
            {
                var oldValue = this._hasCroppedRight;
                if (this._hasCroppedRight != value)
                {
                    this._hasCroppedRight = value;
                    this.OnHasCroppedRightChanged(oldValue, value);
                }
            }
        }
        public ITimeCodeDocumentItem<string> Item
        {
            get { return this._item; }
            internal set
            {
                var oldValue = this._item;
                if (this._item != value)
                {
                    this._item = value;
                    this.OnItemChanged(value, oldValue);
                }
            }
        }
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public SubtitleControl()
        {
            InitializeComponent();
            this.Focusable = false;

            this.InvalidateBorders();
            this.SetDisplay();

            this.x_TouchTextControl.Click += (s, e) =>
            {
                this.OnClick(e.Index);
            };
        }
        /* #endregion Public Constructors */

       
    }
}

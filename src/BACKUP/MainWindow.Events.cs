using System.Windows;
using Unosquare.FFME;
using Unosquare.FFME.Common;

namespace Maptz.Subtitler.Wpf.App
{
    public partial class MainWindow
    {
        /* #region Private Methods */
        /// <summary>
        /// Handles the MediaFailed event of the Media control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "MediaFailedEventArgs"/> instance containing the event data.</param>
        private void OnMediaFailed(object sender, MediaFailedEventArgs e)
        {
            MessageBox.Show(Application.Current.MainWindow, $"Media Failed: {e.ErrorException.GetType()}\r\n{e.ErrorException.Message}", $"{nameof(MediaElement)} Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        /// <summary>
        /// Handles the FFmpegMessageLogged event of the MediaElement control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "MediaLogMessageEventArgs"/> instance containing the event data.</param>
        private void OnMediaFFmpegMessageLogged(object sender, MediaLogMessageEventArgs e)
        {
            if (e.MessageType != MediaLogMessageType.Warning && e.MessageType != MediaLogMessageType.Error)
                return;
            if (string.IsNullOrWhiteSpace(e.Message) == false && e.Message.ContainsOrdinal("Using non-standard frame rate"))
                return;
        
        }

        /// <summary>
        /// Handles the MessageLogged event of the Media control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "MediaLogMessageEventArgs"/> instance containing the event data.</param>
        private void OnMediaMessageLogged(object sender, MediaLogMessageEventArgs e)
        {
            if (e.MessageType == MediaLogMessageType.Trace)
                return;
        
        }
    /* #endregion Private Methods */
    }
}
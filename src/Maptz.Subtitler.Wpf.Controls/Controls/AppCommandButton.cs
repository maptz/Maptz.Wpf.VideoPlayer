using Maptz.Audio.WavFiles;
using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Editing.TimeCodeDocuments;
using Maptz.QuickVideoPlayer.Commands;
using Maptz.Tiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

    public class AppCommandButton : Button
    {
        public IIconSource IconSource { get; private set; }

        public static AppCommandButton FromAppCommand(IAppCommand appCommand)
        {
            var ret = new AppCommandButton
            {
                Command = appCommand,
                IconSource = appCommand.IconSource
            };

            var iconElement = appCommand.IconSource?.GetIconElement();
            if (iconElement != null)
            {
                switch (iconElement)
                {
                    case VectorIcon vi:
                        vi.Foreground = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                        ret.Content = iconElement;
                        break;
                    case null:
                        break;
                    default:
                        ret.Content = iconElement;
                        break;
                }
            }
            else
            {
                ret.Content = appCommand.Name;
            }
            ret.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            ret.Foreground = new SolidColorBrush(Color.FromArgb(255, 10, 10, 10));
            ret.BorderThickness = default(Thickness);

            var hover = appCommand.Name;
            if (appCommand.Shortcut != null)
                hover += $" ({appCommand.Shortcut})";

            ret.ToolTip = hover;

            ret.Margin = new Thickness(3, 0, 3, 0);
            return ret;
        }
    }
}
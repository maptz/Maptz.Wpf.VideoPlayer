using Maptz.QuickVideoPlayer.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
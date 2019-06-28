using Maptz.QuickVideoPlayer;
using Maptz.QuickVideoPlayer.Commands;
using Maptz.QuickVideoPlayer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Unosquare.FFME;
namespace Maptz.QuickVideoPlayer.Commands
{

    public interface IIconSource
    {
        FrameworkElement GetIconElement();
    }
}
using Maptz.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace Maptz.QuickVideoPlayer
{


    public class DefaultTileProvider : TileProviderBase<Tile>
    {
        protected override Tile GetTile(int tileX, int tileY, int levelOfDetail)
        {
            return new Tile(tileX, tileY, levelOfDetail);
        }

       

    }
}
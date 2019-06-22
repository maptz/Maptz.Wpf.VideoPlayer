using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace Maptz.QuickVideoPlayer.Commands
{

    public class KeyChord
    {
        public KeyChord()
        {

        }

        public KeyChord(Key key, bool shift = false, bool ctrl = false, bool alt = false)
        {
            this.Key = key;
            this.Shift = shift;
            this.Ctrl = ctrl;
            this.Alt = alt;
        }



        public Key Key { get; set; } = Key.None;
        public bool Shift { get; set; }
        public bool Ctrl { get; set; }
        public bool Alt { get; set; }

        public static KeyChord FromString(string str)
        {
            
            var parts = str.Split('+').Select(p => p).ToArray();
            if (parts.Length == 0) throw new ArgumentOutOfRangeException();
            var isShift = parts.Any(p => string.Equals(p, "shift", StringComparison.OrdinalIgnoreCase));
            var isAlt = parts.Any(p => string.Equals(p, "alt", StringComparison.OrdinalIgnoreCase));
            var isCtrl = parts.Any(p => string.Equals(p, "ctrl", StringComparison.OrdinalIgnoreCase));
            var key = parts.Last();
            var k = (Key) Enum.Parse(typeof(Key), key);

            return new KeyChord
            {
                Key = k,
                Shift = isShift,
                Alt = isAlt,
                Ctrl = isCtrl
            };
        }

        public override string ToString()
        {
            var expectedParts = new List<string>();
            if (this.Shift) expectedParts.Add("Shift");
            if (this.Ctrl) expectedParts.Add("Ctrl");
            if (this.Alt) expectedParts.Add("Alt");
            expectedParts.Add(this.Key.ToString());

            return string.Join("+", expectedParts);
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Maptz.QuickVideoPlayer.Commands
{
    public class KeyChords
    {
        public KeyChords()
        {
        }

        public KeyChords(string chord)
        {
            var keychords = ParseKeyChords(chord);
            this.Chords = keychords;
        }

        public KeyChords(params KeyChord[] keyChord)
        {
            this.Chords = keyChord.ToArray();
        }

        public IEnumerable<KeyChord> Chords = new KeyChord[0];
        private KeyChord[] ParseKeyChords(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new KeyChord[0];
            var parts = str.Split(',');
            var keychords = parts.Select(p => KeyChord.FromString(p)).ToArray();
            return keychords;
        }

        public KeyChords FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new KeyChords();
            var keychords = ParseKeyChords(str);
            return new KeyChords{Chords = keychords.ToArray()};
        }

        public override string ToString()
        {
            return string.Join(",", this.Chords.Select(p => p.ToString()));
        }
    }
}
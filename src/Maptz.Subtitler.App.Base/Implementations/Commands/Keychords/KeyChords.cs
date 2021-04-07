using System.Collections.Generic;
using System.Linq;

namespace Maptz.Subtitler.App.Commands
{
    public class KeyChords
    {
        /* #region Private Methods */
        private KeyChord[] ParseKeyChords(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new KeyChord[0];
            var parts = str.Split(',');
            var keychords = parts.Select(p => KeyChord.FromString(p)).ToArray();
            return keychords;
        }
        /* #endregion Private Methods */
        /* #region Public Fields */
        public IEnumerable<KeyChord> Chords = new KeyChord[0];
        /* #endregion Public Fields */
        /* #region Public Constructors */
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
        /* #endregion Public Constructors */
        /* #region Public Methods */
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
        /* #endregion Public Methods */
    }
}
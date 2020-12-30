using Maptz.QuickVideoPlayer;
using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Wpf.App;
using Maptz.Subtitler.Wpf.Controls;
using Maptz.Subtitler.Wpf.Engine.Icons;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using System.Windows.Input;
namespace Maptz.Subtitler.Wpf.Engine.Commands
{

    public class TextManipulationCommands : CommandProviderBase
    {

        /* #region Private Methods */
        private void InsertTimeCodeAt(long? timeMs)
        {
            if (!timeMs.HasValue) return;
            var textBox = this.ServiceProvider.GetRequiredService<WrappedTextBox>();
            var projectSettings = this.ServiceProvider.GetRequiredService<IProjectSettings>();
            var offset = projectSettings.OffsetTimeCode;
            var t = TimeCode.FromSeconds((double)timeMs.Value / 1000.0, projectSettings.FrameRate);
            var tc = offset.Add(t);
            string prefix = textBox.CaretIndex > 0 ? textBox.Text.Substring(0, textBox.CaretIndex) : string.Empty;
            string suffix = textBox.CaretIndex < textBox.Text.Length ? textBox.Text.Substring(textBox.CaretIndex) : string.Empty;

            string str = tc.ToString();
            if (!prefix.EndsWith("\r\n"))
            {
                str = Environment.NewLine + str;
            }
            if (!suffix.StartsWith("\r\n"))
            {
                str += "\r\n";
            }

            textBox.SelectedText = str;
            //textBox.CaretIndex += textBox.SelectedText.Length;
            textBox.SelectionLength = 0;
        }
        private void InsertTimeCodeFromCursor()
        {
            var projectData = this.ServiceProvider.GetRequiredService<ProjectDataEx>();
            this.InsertTimeCodeAt(projectData.CursorMs);
        }
        private void InsertTimeCodeFromMarkIn()
        {
            var projectData = this.ServiceProvider.GetRequiredService<ProjectDataEx>();
            this.InsertTimeCodeAt(projectData.MarkInMs);
        }
        private void MoveCursorToNextGrammar(bool forward = true)
        {
            var marks = new char[] { ',', '?', ':', ';', '!', '.', '"', '\'', };
            var textBox = this.ServiceProvider.GetRequiredService<WrappedTextBox>();
            string prefix = textBox.CaretIndex > 0 ? textBox.Text.Substring(0, textBox.CaretIndex - 1) : string.Empty;
            string suffix = textBox.CaretIndex < textBox.Text.Length ? textBox.Text.Substring(textBox.CaretIndex) : string.Empty;

            if (forward)
            {
                var nexIdx = suffix.Substring(1).IndexOfAny(marks);
                if (nexIdx >= 0)
                {
                    textBox.CaretIndex = textBox.CaretIndex + nexIdx + 1 + 1;
                    textBox.SelectionLength = 0;
                }
            }
            else
            {
                var back = string.Join(string.Empty, prefix.Skip(1).Reverse());
                var nexIdx = back.IndexOfAny(marks);
                if (nexIdx >= 0)
                {
                    textBox.CaretIndex = textBox.CaretIndex - nexIdx - 1;
                    textBox.SelectionLength = 0;
                }
            }



        }
        private void SplitSentences()
        {
            var textBox = this.ServiceProvider.GetRequiredService<WrappedTextBox>();
            var text = textBox.Text;
            StringBuilder sb = new StringBuilder();
            var remainder = text;
            var idx = remainder.IndexOf('.');
            while (idx > -1)
            {
                var prefix = remainder.Substring(0, idx + 1);
                sb.Append(prefix);
                remainder = remainder.Substring(idx + 1);
                if (!remainder.StartsWith("\r\n"))
                {
                    sb.AppendLine();
                }
                idx = remainder.IndexOf('.');
            }
            sb.Append(remainder);
            var s = sb.ToString();
            var lines = s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var sb2 = new StringBuilder();
            foreach (var line in lines)
            {
                sb2.AppendLine(line.TrimStart());
            }

            textBox.Text = sb2.ToString();
        }
        /* #endregion Private Methods */
        /* #region Public Properties */
        public IAppCommand InsertTimeCodeFromCursorCommand => new AppCommand(nameof(InsertTimeCodeFromCursor), (object o) => this.InsertTimeCodeFromCursor(), new KeyChords(new KeyChord(Key.D1, ctrl: true)), new XamlIconSource(IconPaths3.pencil_plus));
        public IAppCommand InsertTimeCodeFromMarkInCommand => new AppCommand(nameof(InsertTimeCodeFromMarkIn), (object o) => this.InsertTimeCodeFromMarkIn(), new KeyChords(new KeyChord(Key.D2, ctrl: true)), new XamlIconSource(IconPaths3.flag_plus));
        public IAppCommand NextGrammarPointCommand => new AppCommand("NextGrammarPoint", (object o) => this.MoveCursorToNextGrammar(), new KeyChords(new KeyChord(Key.Right, alt: true)), new XamlIconSource(IconPaths3.page_next));
        public IAppCommand PreviousGrammarPointCommand => new AppCommand("PreviousGrammarPoint", (object o) => this.MoveCursorToNextGrammar(false), new KeyChords(new KeyChord(Key.Left, alt: true)), new XamlIconSource(IconPaths3.page_previous));
        public IServiceProvider ServiceProvider { get; }
        public IAppCommand SplitSentencesCommand => new AppCommand(nameof(SplitSentences), (object o) => this.SplitSentences(), new KeyChords(new KeyChord(Key.D6, ctrl: true)), new XamlIconSource(IconPaths3.text_subject));
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public TextManipulationCommands(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
        /* #endregion Public Constructors */
        /* #region Interface: 'Maptz.QuickVideoPlayer.Commands.ICommandProvider' Methods */

        /* #endregion Interface: 'Maptz.QuickVideoPlayer.Commands.ICommandProvider' Methods */
    }
}
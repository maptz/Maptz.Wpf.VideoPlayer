using Maptz.QuickVideoPlayer;
using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Wpf.App;
using Maptz.Subtitler.Wpf.Controls;
using Maptz.Subtitler.Wpf.Engine.Icons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
namespace Maptz.Subtitler.Wpf.Engine.Commands
{

    public class SubtitleManipulationCommandsSettings
    {
        public int MaxLineLength { get; set; } = 30;
    }

    public class SubtitleManipulationCommands : CommandProviderBase
    {
        public SubtitleManipulationCommands(IServiceProvider serviceProvider, IOptions<SubtitleManipulationCommandsSettings> settings)
        {
            this.ServiceProvider = serviceProvider;
            this.Settings = settings.Value;
        }

        public IServiceProvider ServiceProvider { get; }
        public SubtitleManipulationCommandsSettings Settings { get; }

        public void SplitCurrentSubtitle()
        {
            var textBox = this.ServiceProvider.GetRequiredService<WrappedTextBox>();
            var fps = this.ServiceProvider.GetRequiredService<IProjectSettings>().FrameRate;
            var subtitleView = this.ServiceProvider.GetRequiredService<ISubtitleView>();
            if (subtitleView.SubtitleItems == null) return;

            var caretIndex = textBox.CaretIndex;
            var currentSubtitle = subtitleView.SubtitleItems.FirstOrDefault(p => p.TextSpan.Start <= caretIndex && (p.TextSpan.Start + p.TextSpan.Length) >= caretIndex);

            if (currentSubtitle == null) return;

            var prefix = currentSubtitle.TextSpan.Document.Substring(currentSubtitle.ContentTextSpan.Start, caretIndex - currentSubtitle.ContentTextSpan.Start);
            var suffix = currentSubtitle.TextSpan.Document.Substring(caretIndex, currentSubtitle.ContentTextSpan.Start + currentSubtitle.ContentTextSpan.Length - caretIndex);

            if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(suffix)) return;
            prefix = prefix.Trim('\r', '\n');
            suffix = suffix.Trim('\r', '\n');

            var totalDurationFrames = currentSubtitle.RecordOut.TotalFrames - currentSubtitle.RecordIn.TotalFrames;
            if (totalDurationFrames < 3) return;
            var splitProprtion = (double)prefix.Length / (double)(prefix.Length + suffix.Length);
            var splitFrame = (int)Math.Floor(splitProprtion * (double)totalDurationFrames);
            var outTC1 = TimeCode.FromFrames(currentSubtitle.RecordIn.TotalFrames + splitFrame - 1, fps);
            var intTC2 = TimeCode.FromFrames(currentSubtitle.RecordIn.TotalFrames + splitFrame + 1, fps);
            var outTC2 = TimeCode.FromFrames(currentSubtitle.RecordOut.TotalFrames, fps);

            StringBuilder replacements = new StringBuilder();
            replacements.AppendLine($"{currentSubtitle.RecordIn} {outTC1}");
            replacements.AppendLine(prefix);
            replacements.AppendLine();
            replacements.AppendLine($"{intTC2} {outTC2}");
            replacements.AppendLine(suffix);
            replacements.AppendLine();

            textBox.TextEditor.Document.Replace((int)currentSubtitle.TextSpan.Start, (int)currentSubtitle.TextSpan.Length, replacements.ToString());

        }

        public void CreateSensibleLines()
        {
            var textBox = this.ServiceProvider.GetRequiredService<WrappedTextBox>();
            var fps = this.ServiceProvider.GetRequiredService<IProjectSettings>().FrameRate;
            var subtitleView = this.ServiceProvider.GetRequiredService<ISubtitleView>();
            var lineSplitter = this.ServiceProvider.GetRequiredService<ILineSplitter>();
            if (subtitleView.SubtitleItems == null) return;

            var caretIndex = textBox.CaretIndex;
            var currentSubtitle = subtitleView.SubtitleItems.FirstOrDefault(p => p.TextSpan.Start <= caretIndex && (p.TextSpan.Start + p.TextSpan.Length) >= caretIndex);

            if (currentSubtitle == null) return;

            var content = currentSubtitle.Content;
            content = content.Replace("\r\n", " "); //Remove existing new lines
            content = Regex.Replace(content, "\\s\\s+", " ");
            var newLines = lineSplitter.SplitToLines(content, this.Settings.MaxLineLength);

            StringBuilder replacements = new StringBuilder();
            replacements.AppendLine($"{currentSubtitle.RecordIn} {currentSubtitle.RecordOut}");
            replacements.AppendLine(string.Join("\r\n", newLines));
            replacements.AppendLine();

            textBox.TextEditor.Document.Replace((int)currentSubtitle.TextSpan.Start, (int)currentSubtitle.TextSpan.Length, replacements.ToString());
        }



        public IAppCommand SplitCurrentSubtitleCommand => new AppCommand(nameof(SplitCurrentSubtitle), (object o) => this.SplitCurrentSubtitle(), new KeyChords(new KeyChord(Key.D8, ctrl: true)), new XamlIconSource(IconPaths3.text_subject));

        public IAppCommand CreateSensibleLinesCommand => new AppCommand(nameof(CreateSensibleLines), (object o) => this.CreateSensibleLines(), new KeyChords(new KeyChord(Key.D9, ctrl: true)), new XamlIconSource(IconPaths3.text_subject));
    }
}
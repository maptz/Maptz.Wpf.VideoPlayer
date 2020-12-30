using Maptz.Editing.TimeCodeDocuments;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Engine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Maptz.Subtitler.Wpf.App
{

    public class SubtitleView : ISubtitleView
    {
        public SubtitleView(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IEnumerable<ITimeCodeDocumentItem<string>> SubtitleItems
        {
            get => this._subtitleItems;
            private set
            {
                var oldValue = this._subtitleItems;
                if (oldValue != value)
                {
                    this._subtitleItems = value;
                    this.OnSubtitleItemsChanged(oldValue, value);
                }
            }
        }

        private void OnSubtitleItemsChanged(IEnumerable<ITimeCodeDocumentItem<string>> oldValue, IEnumerable<ITimeCodeDocumentItem<string>> newValue)
        {

        }

        public IServiceProvider ServiceProvider { get; }

        private DateTimeOffset? _lastSubtitleUpdate = null;
        private DateTimeOffset? _lastSubtitleUpdateRequest = null;
        private CancellationTokenSource _subtitleCheckTaskCancellationTokenSource = new CancellationTokenSource();
        private IEnumerable<ITimeCodeDocumentItem<string>> _subtitleItems;
        private Task SubtitleCheckTask = null;

        public void Dispose()
        {
            this._subtitleCheckTaskCancellationTokenSource.Cancel();
        }

        public void RequestSubtitleUpdate()
        {
            if (this.SubtitleCheckTask == null)
            {
                this.SubtitleCheckTask = Task.Run(async () =>
                {
                    do
                    {
                        var now = DateTimeOffset.Now;
                        var timeSinceLastUpdate = now - this._lastSubtitleUpdate;
                        const double MinUpdateWaitSeconds = 0.2;
                        //We will update if, there has been an updated requested, and if that was at least MinUpdateWaitSeconds since the last update.

                        //If the minimum time has elapsed and there hasn't been an update since the 
                        if (_lastSubtitleUpdate == null || timeSinceLastUpdate?.TotalSeconds > MinUpdateWaitSeconds)
                        {
                            //Check if there has been an update since the last update request
                            if (_lastSubtitleUpdateRequest.HasValue)
                            {
                                this._lastSubtitleUpdateRequest = null;
                                var projectData = this.ServiceProvider.GetRequiredService<IProjectData>();
                                var subtitleProvider = this.ServiceProvider.GetRequiredService<ISubtitleProvider>();
                                var doc = await subtitleProvider.GetSubtitlesAsync(projectData.Text);
                                this.SubtitleItems = doc.Items;
                                //await Dispatcher.BeginInvoke(new Action(() =>
                                //{
                                //    this.SubtitleItems = doc.Items;
                                //}), null);

                            }
                            this._lastSubtitleUpdate = DateTimeOffset.Now;
                        }
                        await Task.Delay(40);
                    }
                    while (!_subtitleCheckTaskCancellationTokenSource.Token.IsCancellationRequested);

                });
            }
            //Request a new update;
            this._lastSubtitleUpdateRequest = DateTimeOffset.Now;
        }
    }
}
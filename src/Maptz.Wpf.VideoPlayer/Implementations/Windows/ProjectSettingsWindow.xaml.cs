using Maptz.QuickVideoPlayer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Maptz.QuickVideoPlayer
{
    /// <summary>
    /// Interaction logic for ProjectSettingsWindow.xaml
    /// </summary>
    public partial class ProjectSettingsWindow : Window
    {
        public ProjectSettingsWindowViewModel ViewModel { get; }
        public ProjectSettings ProjectSettings { get; }
        public IServiceProvider ServiceProvider { get; }

        private void SetupViewModel()
        {
            var vm = this.ViewModel;
            var projectSettings = this.ProjectSettings;
            if (projectSettings != null)
            {
                vm.FrameRate = projectSettings.FrameRate.ToString();
                vm.TimeCode = projectSettings.OffsetTimeCode.ToString();
                vm.VideoFilePath = projectSettings.VideoFilePath;
            }
        }



        public ProjectSettingsWindow(ProjectSettings projectSettings = null, IServiceProvider serviceProvider = null)
        {
            InitializeComponent();
            var vm = new ProjectSettingsWindowViewModel();
            this.ViewModel = vm;
            this.ProjectSettings = projectSettings;
            this.ServiceProvider = serviceProvider;
            this.SetupViewModel();

            this.DataContext = this.ViewModel;

            this.x_Button_OK.Click += (s, e) =>
            {
                this.DialogResult = true;
                this.AcceptChanges();
                this.Close();
            };
            this.x_Button_Cancel.Click += (s, e) =>
            {
                this.DialogResult = false;
                this.Close();
            };

            this.x_Button_SelectFile.Click += (s, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
               var sessionState =  this.ServiceProvider?.GetRequiredService<SessionState>();
                if (sessionState != null && !string.IsNullOrEmpty(sessionState.OpenVideoFileDirectoryPath))
                {
                    ofd.InitialDirectory = sessionState.OpenVideoFileDirectoryPath;
                }
                var result = ofd.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    this.ViewModel.VideoFilePath = ofd.FileName;
                    if (sessionState != null)
                    {
                        sessionState.OpenVideoFileDirectoryPath = System.IO.Path.GetDirectoryName(ofd.FileName);
                    }
                }
            };
        }

        private void AcceptChanges()
        {
            if (this.ProjectSettings == null) return;
            this.ProjectSettings.FrameRate = (SmpteFrameRate)Enum.Parse(typeof(SmpteFrameRate), this.ViewModel.FrameRate);
            this.ProjectSettings.OffsetFrames = new TimeCode(this.ViewModel.TimeCode, this.ProjectSettings.FrameRate).TotalFrames;
            this.ProjectSettings.VideoFilePath = this.ViewModel.VideoFilePath;
        }
    }
}

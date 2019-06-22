using Maptz;
using Maptz.Editing.TimeCodeDocuments;
using Maptz.Editing.TimeCodeDocuments.Converters.All;
using Maptz.Editing.TimeCodeDocuments.StringDocuments;
using Maptz.QuickVideoPlayer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickProjectCreator
{
    class Program
    {
        static void Main(string[] args)
        {

            DoWork2Async().GetAwaiter().GetResult();
        }

        static async Task DoWork2Async()
        {

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.AddLogging();
            serviceCollection.AddTimeCodeDocumentConverters();

            serviceCollection.Configure<TimeCodeStringDocumentParserSettings>(settings =>
            {
                settings.FrameRate = SmpteFrameRate.Smpte25;
            });
            serviceCollection.Configure<TimeCodeDocumentTimeValidatorSettings>(settings =>
            {
                settings.DefaultDurationFrames = 60;
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var dsParser = await TimeCodeDocumentConverters.GetAvidDSParserAsync(serviceProvider);
            var parser = serviceProvider.GetRequiredService<ITimeCodeDocumentParser<string>>();

            var projectFilePath = @"D:\PLAYOUTS\LANGUAGE REFINEMENTS\PROJECTS\DONE";
            foreach(var fi in Directory.GetFiles(projectFilePath).Select(p=>new FileInfo(p)))
            {
                var proj = Project.ReadFromFile(fi.FullName);
                var txt = proj.ProjectData.Text;
                var ds = dsParser.Parse(txt);

                var dsFilePath = Path.Combine(projectFilePath, "DS", Path.GetFileNameWithoutExtension(fi.Name) + ".ds.txt");
                using (var sw = new FileInfo(dsFilePath).CreateText())
                {
                    sw.Write(dsFilePath);
                }
            }
        }

        static async Task DoWorkAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.AddLogging();
            serviceCollection.AddTimeCodeDocumentConverters();

            serviceCollection.Configure<TimeCodeStringDocumentParserSettings>(settings =>
            {
                settings.FrameRate = SmpteFrameRate.Smpte25;
            });
            serviceCollection.Configure<TimeCodeDocumentTimeValidatorSettings>(settings =>
            {
                settings.DefaultDurationFrames = 60;
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var dsParser = await TimeCodeDocumentConverters.GetAvidDSParserAsync(serviceProvider);
            var parser = serviceProvider.GetRequiredService<ITimeCodeDocumentParser<string>>();

            var dsFiles = new DirectoryInfo(@"D:\PLAYOUTS\LANGUAGE REFINEMENTS\DS").GetFiles();
            var movFiles = new DirectoryInfo(@"D:\PLAYOUTS\LANGUAGE REFINEMENTS\MOVs").GetFiles();

            foreach (var movFile in movFiles)
            {
                var dsFileName = Path.GetFileNameWithoutExtension(movFile.FullName) + ".txt";
                var dsFilePath = Path.Combine(@"D:\PLAYOUTS\LANGUAGE REFINEMENTS\DS", dsFileName);
                if (File.Exists(dsFilePath))
                {
                    Console.WriteLine($"Match for {movFile.Name}");

                    string txt;
                    using (var sr = new FileInfo(dsFilePath).OpenText())
                    {
                        txt = sr.ReadToEnd();
                    }
                    var tcd = parser.Parse(txt);
                    var sb = new StringBuilder();
                    foreach(var item in tcd.Items)
                    {
                        sb.AppendLine(item.RecordIn.ToString());
                        sb.AppendLine(item.Content);
                        sb.AppendLine();
                    }

                    var str = sb.ToString();



                    var project = new Project()
                    {
                        ProjectFilePath = Path.Combine(@"D:\PLAYOUTS\LANGUAGE REFINEMENTS\PROJECTS\", Path.GetFileNameWithoutExtension(movFile.FullName) + ".proj"),
                        ProjectData = new ProjectData
                        {
                            Text = str
                        },
                        ProjectSettings = new ProjectSettings
                        {
                            OffsetFrames = new TimeCode("01:00:00:00", SmpteFrameRate.Smpte25).TotalFrames,
                            VideoFilePath = movFile.FullName,
                            FrameRate = SmpteFrameRate.Smpte25,
                        }
                    };
                    Project.SaveToFile(project);


                }
                else
                {
                    Console.WriteLine("Missing DS for:");
                    Console.WriteLine($"'{Path.GetFileNameWithoutExtension(movFile.FullName)}'");
                    Console.WriteLine("Press a key");
                    Console.ReadKey();

                }
            }
        }
    }
}

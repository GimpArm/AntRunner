using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AntRunner.Interface;
using Newtonsoft.Json;

namespace AntRunner.Wrapper.Php
{
    public class PhpAnt : Ant, IDisposable
    {
        private readonly Process _phpProcess;
        private readonly string _workingDirectory;
        private string _lastOutput;

        public override string Name => Read("N", true) ?? "No name";

        public override Stream Flag => File.Exists(Path.Combine(_workingDirectory, "Flag.png")) ? new FileStream(Path.Combine(_workingDirectory, "Flag.png"), FileMode.Open) : base.Flag;
        
        public PhpAnt(string antPath)
        {
            var info = new FileInfo(antPath);
            _workingDirectory = info.DirectoryName;
            var assemblyInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var runningFolder = assemblyInfo.DirectoryName;
            if (runningFolder == null || _workingDirectory == null) throw new NullReferenceException();
            
            var settings = ReadSettings(_workingDirectory);
            var debug = string.Empty;
            if (settings.Debug)
            {
                var temp = Path.GetTempPath();
                if (temp.Last() == '\\')
                {
                    temp = temp.Substring(0, temp.Length - 1);
                }
                debug = $"-d xdebug.profiler_enable=On -d xdebug.profiler_output_dir=\"{temp}\" -d xdebug.idekey={settings.IdeKey} ";
            }
            _phpProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = Path.Combine(runningFolder, @"php\php.exe"),
                    Arguments = $@"{debug}""{Path.Combine(runningFolder, @"lib\AntWrapper.php")}"" ""{info.Name}""",
                    WorkingDirectory = _workingDirectory
                }
            };

            _phpProcess.OutputDataReceived += PhpProcessOnOutputDataReceived;
            _phpProcess.ErrorDataReceived += PhpProcessOnErrorDataReceived;
            _phpProcess.Start();
            _phpProcess.BeginOutputReadLine();

            //Ping the ant so we can wait for PHP to start running
            Read("P");
        }

        private void PhpProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print(e.Data);
        }

        private void PhpProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _lastOutput = e.Data;
        }

        public override void Initialize(int mapWidth, int mapHeight, ItemColor antColor, int startX, int startY)
        {
            _phpProcess.StandardInput.WriteLine($"Ia:5:{{i:0;i:{mapWidth};i:1;i:{mapHeight};i:2;i:{(int)antColor};i:3;i:{startX};i:4;i:{startY};}}");
        }

        public override void Tick(GameState state)
        {
            var result = Read("T" + SerializeState(state));
            if (!int.TryParse(result, out var actInt) || actInt > 15) return;
            Action = (AntAction)Enum.ToObject(typeof(AntAction), actInt);
        }

        private string SerializeState(GameState state)
        {
            var response = state.Response == null ? "N;" : $"O:22:\"AntRunner\\EchoResponse\":2;{{s:8:\"Distance\";i:{state.Response.Distance};s:4:\"Item\";i:{(int)state.Response.Item};}}";
            return $"O:19:\"AntRunner\\GameState\":7:{{s:10:\"TickNumber\";i:{state.TickNumber};s:7:\"HasFlag\";b:{(state.HasFlag ? 1 : 0)};s:5:\"FlagX\";i:{state.FlagX};s:5:\"FlagY\";i:{state.FlagY};s:11:\"AntWithFlag\";i:{(int)state.AntWithFlag};s:8:\"Response\";{response}s:5:\"Event\";i:{(int)state.Event};}}";
        }

        public void Dispose()
        {
            _phpProcess?.Kill();
            _phpProcess?.Dispose();
        }

        private string Read(string input, bool timeout = false)
        {
            _phpProcess.StandardInput.WriteLine(input);
            var delay = timeout ? 9 : int.MaxValue;
            var cnt = 0;
            while (_lastOutput == null && cnt < delay)
            {
                Task.Delay(1).Wait();
                cnt++;
            }

            var result = _lastOutput;
            _lastOutput = null;
            return result;
        }

        private static Settings ReadSettings(string workingDirectory)
        {
            var settingsFile = Path.Combine(workingDirectory, "antsettings.json");
            try
            {
                if (File.Exists(settingsFile))
                {
                    return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsFile));
                }
                
            }
            catch
            {
                //Do nothing
            }
            return new Settings { Debug = false };
        }
    }
}

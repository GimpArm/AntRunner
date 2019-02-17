using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using AntRunner.Interface;
using Newtonsoft.Json;


namespace AntRunner.Wrapper.Ruby
{
    public class RubyAnt : Ant, IDisposable
    {
        private const string RubyVersion = "Ruby2.6.1";
        private readonly Process _rubyProcess;
        private readonly string _workingDirectory;
        private string _lastOutput;

        public override string Name => Read("N", 9) ?? "No name";

        public override Stream Flag => File.Exists(Path.Combine(_workingDirectory, "Flag.png")) ? new MemoryStream(File.ReadAllBytes(Path.Combine(_workingDirectory, "Flag.png"))) : base.Flag;

        public RubyAnt(string antPath)
        {
            try
            {
                var info = new FileInfo(antPath);
                _workingDirectory = info.DirectoryName;
                var assemblyInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                var runningFolder = assemblyInfo.DirectoryName;
                if (runningFolder == null || _workingDirectory == null) throw new NullReferenceException();
                var rubyFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AntRunner", RubyVersion);

                if (!Directory.Exists(rubyFolder))
                {
                    Unzip(runningFolder, rubyFolder);
                }

                var settings = ReadSettings(_workingDirectory);
                var debug = string.Empty;
                if (settings.Debug)
                {
                    if (settings.Port == 0)
                    {
                        settings.Port = new Random().Next(1000, 65536);
                    }

                    debug = $@"""{Path.Combine(runningFolder, @"lib\rdebug-ide")}"" --host 0.0.0.0 --port {settings.Port} ";
                }

                _rubyProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        FileName = Path.Combine(rubyFolder, @"bin\ruby.exe"),
                        Arguments = $@"{debug}""{Path.Combine(runningFolder, @"lib\AntWrapper.rb")}"" ""{antPath}""",
                        WorkingDirectory = _workingDirectory
                    }
                };

                _rubyProcess.OutputDataReceived += RubyProcessOnOutputDataReceived;
                _rubyProcess.ErrorDataReceived += RubyProcessOnErrorDataReceived;
                _rubyProcess.Start();
                _rubyProcess.BeginOutputReadLine();

                //Ping the ant so we can wait for Ruby to start running
                if (string.IsNullOrEmpty(Read("P", 2000))) throw new Exception("Ruby Ant is not responding");
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private void RubyProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print(e.Data);
        }

        private void RubyProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _lastOutput = e.Data;
        }

        public override void Initialize(int mapWidth, int mapHeight, ItemColor antColor, int startX, int startY)
        {
            _rubyProcess.StandardInput.WriteLine($"I[{mapWidth},{mapHeight},{(int)antColor},{startX},{startY}]");
        }

        public override void Tick(GameState state)
        {
            var result = Read("T" + SerializeState(state));
            if (!int.TryParse(result, out var actInt) || actInt > 15) return;
            Action = (AntAction)Enum.ToObject(typeof(AntAction), actInt);
        }

        private string SerializeState(GameState state)
        {
            var response = state.Response == null ? "null" : $"{{\"Distance\":{state.Response.Distance},\"Item\":{(int)state.Response.Item}}}";
            return $"{{\"TickNumber\":{state.TickNumber},\"Response\":{response},\"Event\":{(int)state.Event},\"FlagX\":{state.FlagX},\"FlagY\":{state.FlagY},\"AntWithFlag\":{(int)state.AntWithFlag}}}";
        }

        public void Dispose()
        {
            if (_rubyProcess == null) return;

            try
            {
                _rubyProcess.StandardInput.WriteLineAsync("X");
            }
            catch
            {
                //Do  Nothing
            }
            _rubyProcess.Kill();
            _rubyProcess.Dispose();
        }

        private string Read(string input, int delay = int.MaxValue)
        {
            _rubyProcess.StandardInput.WriteLine(input);
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

        private static void Unzip(string source, string target)
        {
            var zipFile = Path.Combine(source, RubyVersion + ".zip");
            if (!File.Exists(zipFile)) throw new Exception("Cannot file ruby.zip in wrapper folder");
            ZipFile.ExtractToDirectory(zipFile, target);
        }
    }
}

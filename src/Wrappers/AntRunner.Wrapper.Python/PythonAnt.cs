using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AntRunner.Interface;
using Newtonsoft.Json;

namespace AntRunner.Wrapper.Python
{
    public class PythonAnt : Ant, IDisposable
    {
        private readonly Process _pythonProcess;
        private readonly string _workingDirectory;
        private string _lastOutput;

        public override string Name => Read("N", 9) ?? "No name";

        public override Stream Flag => File.Exists(Path.Combine(_workingDirectory, "Flag.png")) ? new FileStream(Path.Combine(_workingDirectory, "Flag.png"), FileMode.Open) : base.Flag;

        public PythonAnt(string antPath)
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
                if (settings.Port == 0)
                {
                    settings.Port = new Random().Next(1000, 65536);
                }
                debug = $@" debug:{settings.Port}";
            }
            _pythonProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = Path.Combine(runningFolder, @"python\python.exe"),
                    Arguments = $@"-B -u ""{Path.Combine(runningFolder, @"lib\AntWrapper.py")}"" {Path.GetFileNameWithoutExtension(info.Name)}{debug}",
                    WorkingDirectory = _workingDirectory,
                    EnvironmentVariables = { {"PYTHONPATH", $"{Path.Combine(runningFolder, @"python\Lib")};{Path.Combine(runningFolder, @"lib")};{_workingDirectory}"}}
                }
            };

            _pythonProcess.OutputDataReceived += PythonProcessOnOutputDataReceived;
            _pythonProcess.ErrorDataReceived += PythonProcessOnErrorDataReceived;
            _pythonProcess.Start();
            _pythonProcess.BeginOutputReadLine();

            //Ping the ant so we can wait for Python to start running
            if (string.IsNullOrEmpty(Read("P", 2000))) throw new Exception("Python Ant is not responding");
        }

        private void PythonProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print(e.Data);
        }

        private void PythonProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _lastOutput = e.Data;
        }

        public override void Initialize(int mapWidth, int mapHeight, ItemColor antColor, int startX, int startY)
        {
            _pythonProcess.StandardInput.WriteLine($"I[{mapWidth},{mapHeight},{(int)antColor},{startX},{startY}]");
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
            _pythonProcess?.Kill();
            _pythonProcess?.Dispose();
        }

        private string Read(string input, int delay = int.MaxValue)
        {
            _pythonProcess.StandardInput.WriteLine(input);
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

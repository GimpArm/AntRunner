using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AntRunner.Interface;
using Newtonsoft.Json;

namespace AntRunner.Wrapper.Js
{
    public class JsAnt : Ant, IDisposable
    {
        private static readonly ASCIIEncoding Encoder = new ASCIIEncoding();

        private readonly Process _nodeProcess;
        private readonly TcpClient _client;
        private NetworkStream _serverStream;
        private readonly string _workingDirectory;
        private string _lastOutput;

        public override string Name => Read("N", 9) ?? "No name";

        public override Stream Flag => File.Exists(Path.Combine(_workingDirectory, "Flag.png")) ? new MemoryStream(File.ReadAllBytes(Path.Combine(_workingDirectory, "Flag.png"))) : base.Flag;

        public JsAnt(string antPath)
        {
            try
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
                    debug = $"--inspect={settings.DebugPort} ";
                    if (settings.Port == 0)
                    {
                        settings.Port = new Random().Next(1000, 65536);
                    }
                }
                else
                {
                    settings.Port = new Random().Next(1000, 65536);
                }

                _nodeProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        FileName = Path.Combine(runningFolder, @"node\node.exe"),
                        Arguments = $@"{debug}""{Path.Combine(runningFolder, @"lib\AntWrapper.js")}"" {settings.Port} ""{antPath}""",
                        WorkingDirectory = _workingDirectory,
                        EnvironmentVariables = {{"NODE_PATH", $"{Path.Combine(runningFolder, "lib")};{Path.Combine(runningFolder, @"node\node_modules")};{Path.Combine(_workingDirectory, "node_modules")}"}}
                    }
                };
                _nodeProcess.Start();

                _client = new TcpClient();
                Connect(_client, settings.Port);
                _serverStream = _client.GetStream();

                Task.Factory.StartNew(Reader);

                if (string.IsNullOrEmpty(Read("P", 2000))) throw new Exception("JavaScript Ant is not responding");
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public override void Initialize(int mapWidth, int mapHeight, ItemColor antColor, int startX, int startY)
        {
            Write($"I[{mapWidth},{mapHeight},{(int)antColor},{startX},{startY}]");
        }

        public override void Tick(GameState state)
        {
            Read("T" + SerializeState(state));

        }

        public AntAction GetAction()
        {
            var result = Read("A", 5);
            if (!int.TryParse(result, out var actInt) || actInt > 15) return AntAction.Wait;
            Action = (AntAction)Enum.ToObject(typeof(AntAction), actInt);
            return Action;
        }

        private static string SerializeState(GameState state)
        {
            var response = state.Response == null ? "null" : $"{{\"Distance\":{state.Response.Distance},\"Item\":{(int)state.Response.Item}}}";
            return $"{{\"TickNumber\":{state.TickNumber},\"Response\":{response},\"Event\":{(int)state.Event},\"HasFlag\":{(state.HasFlag?"true":"false")},\"FlagX\":{state.FlagX},\"FlagY\":{state.FlagY},\"AntWithFlag\":{(int)state.AntWithFlag}}}";
        }

        private void Reader()
        {
            while (_client.Connected)
            {
                _serverStream = _client.GetStream();
                var message = new byte[4096];
                var bytesRead = 0;
                try
                {
                    bytesRead = _serverStream.Read(message, 0, 4096);
                }
                catch
                {
                    /*a socket error has occured*/
                }

                _lastOutput = Encoder.GetString(message, 0, bytesRead);
            }
        }

        private string Read(string input, int delay = int.MaxValue)
        {
            Write(input);
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

        private void Write(string input)
        {
            input += Environment.NewLine;
            _serverStream.Write(Encoder.GetBytes(input), 0, input.Length);
            _serverStream.Flush();
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

        private static void Connect(TcpClient client, int port)
        {
            var cnt = 0;
            while (!client.Connected)
            {
                try
                {
                    client.Connect("127.0.0.1", port);
                }
                catch
                {
                    if (cnt > 20)
                    {
                        throw new Exception($"Could not connect to node server on port {port}");
                    }
                    Task.Delay(10).Wait();
                    cnt++;
                }
            }
        }

        public void Dispose()
        {
            if (_serverStream != null)
            {
                try
                {
                    _serverStream.WriteAsync(Encoder.GetBytes("X"), 0, 1);
                    _serverStream.Flush();
                }
                catch
                {
                    //Do Nothing
                }
                _serverStream.Dispose();
            }
            _client?.Dispose();
            _nodeProcess?.Dispose();
        }
    }
}

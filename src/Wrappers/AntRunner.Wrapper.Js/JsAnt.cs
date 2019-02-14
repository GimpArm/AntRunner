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

        public override string Name => Read("N", true) ?? "No name";

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
                    debug = "--inspect ";
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
                        EnvironmentVariables = {{"NODE_PATH", $"{Path.Combine(runningFolder, "lib")};{Path.Combine(_workingDirectory, "node_modules")}"}}
                    }
                };
                _nodeProcess.Start();

                _client = new TcpClient();
                Connect(_client, settings.Port);
                _serverStream = _client.GetStream();

                Task.Factory.StartNew(Reader);

                Read("P");
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public override void Initialize(int mapWidth, int mapHeight, ItemColor antColor, int startX, int startY)
        {
            var input = $"I[{mapWidth},{mapHeight},{(int)antColor},{startX},{startY}]";
            _serverStream.Write(Encoder.GetBytes(input), 0, input.Length);
            _serverStream.Flush();
        }

        public override void Tick(GameState state)
        {
            var result = Read("T" + JsonConvert.SerializeObject(state));
            if (!int.TryParse(result, out var actInt) || actInt > 15) return;
            Action = (AntAction)Enum.ToObject(typeof(AntAction), actInt);
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

        private string Read(string input, bool timeout = false)
        {

            _serverStream.Write(Encoder.GetBytes(input), 0, input.Length);
            _serverStream.Flush();
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
            _nodeProcess?.StandardInput.Close();
            _nodeProcess?.Dispose();
        }
    }
}

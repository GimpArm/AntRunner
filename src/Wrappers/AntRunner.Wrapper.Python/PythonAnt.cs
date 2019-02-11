using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AntRunner.Interface;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using Newtonsoft.Json;

namespace AntRunner.Wrapper.Python
{
    public class PythonAnt : Ant, IDisposable
    {
        private readonly ScriptEngine _engine;
        private readonly ScriptScope _scope;
        private readonly string _workingDirectory;

        public override string Name => _engine.Execute<string>("__CurrentAnt__.Name", _scope);

        public override Stream Flag => File.Exists(Path.Combine(_workingDirectory, "Flag.png")) ? new FileStream(Path.Combine(_workingDirectory, "Flag.png"), FileMode.Open) : base.Flag;

        public PythonAnt(string antPath)
        {
            try
            {
                var info = new FileInfo(antPath);
                _workingDirectory = info.DirectoryName;
                var assemblyInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                var runningFolder = assemblyInfo.DirectoryName;
                if (runningFolder == null || _workingDirectory == null) throw new NullReferenceException();

                var settings = ReadSettings(_workingDirectory);
                
                if (settings.Debug)
                {
                    _engine = IronPython.Hosting.Python.CreateEngine(new Dictionary<string, object>
                    {
                        {"Python30", ScriptingRuntimeHelpers.True},
                        {"LightWeightScopes", ScriptingRuntimeHelpers.True},
                        {"Debug", ScriptingRuntimeHelpers.True},
                        {"Frames", ScriptingRuntimeHelpers.True},
                        {"EnableProfiler", ScriptingRuntimeHelpers.True}
                    });
                }
                else
                {
                    _engine = IronPython.Hosting.Python.CreateEngine(new Dictionary<string, object>
                    {
                        {"Python30", ScriptingRuntimeHelpers.True},
                        {"LightWeightScopes", ScriptingRuntimeHelpers.True}
                    });
                }

                _engine.SetSearchPaths(new []
                {
                    Path.Combine(runningFolder, @"python27"),
                    Path.Combine(runningFolder, @"lib"),
                    _workingDirectory
                });
                _scope = _engine.CreateScope();

                var source = _engine.CreateScriptSourceFromFile(antPath);
                source.Execute(_scope);
                
                _engine.Execute(@"def __FindAnt__(obj):
    try:
        return issubclass(obj, AntRunner.Interface.Ant) and obj is not AntRunner.Interface.Ant
    except:
        return False", _scope);

                var ant = _scope.GetVariableNames().FirstOrDefault(x => !x.StartsWith("__") && _engine.Execute($@"__FindAnt__({x})", _scope));
                if (ant == null) throw new NullReferenceException("Python script must contain a class named Ant.");

                _engine.Execute("del globals()[\"__FindAnt__\"]", _scope);

                _engine.Execute($"__CurrentAnt__ = {ant}()", _scope);

                if (settings.Debug)
                {
                    if (settings.Port == 0)
                    {
                        settings.Port = new Random().Next(1000, 65536);
                    }
                    var debug = _engine.CreateScriptSourceFromString($@"try:
    import ptvsd
    ptvsd.enable_attach(address=('127.0.0.1', {settings.Port}), redirect_output=False)
    __debug__ = True
    ptvsd.wait_for_attach()
except Exception as e:
    pass", SourceCodeKind.Statements);
                    try
                    {
                        debug.Execute(_scope);
                    }
                    catch
                    {
                        //Do Nothing
                    }
                }
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public override void Initialize(int mapWidth, int mapHeight, ItemColor antColor, int startX, int startY)
        {
            _engine.Execute($"__CurrentAnt__.Initialize({mapWidth}, {mapHeight}, AntRunner.Interface.ItemColor.{antColor}, {startX}, {startY})", _scope);
        }

        public override void Tick(GameState state)
        {
            _scope.SetVariable("__currentState__", state);
            _engine.Execute("__CurrentAnt__.Tick(__currentState__)", _scope);
            Action = _engine.Execute<AntAction>("__CurrentAnt__.Action", _scope);
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

        public void Dispose()
        {
            _engine?.Runtime?.Shutdown();
        }
    }
}

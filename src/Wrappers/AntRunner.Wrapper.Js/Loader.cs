using AntRunner.Interface;

namespace AntRunner.Wrapper.Js
{
    public class Loader : IWrapperLoader
    {
        public string Extension => "js";

        public Ant LoadAnt(string filename)
        {
            return new JsAnt(filename);
        }
    }
}

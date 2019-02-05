using AntRunner.Interface;

namespace AntRunner.Wrapper.Php
{
    public class Loader : IWrapperLoader
    {
        public string Extension => "php";

        public Ant LoadAnt(string filename)
        {
            return new PhpAnt(filename);
        }
    }
}

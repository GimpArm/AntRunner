using System.Configuration;

namespace AntRunner.Models
{
    public class WrapperLoaderSection : ConfigurationSection
    {
        public static WrapperLoaderSection GetConfig()
        {
            var b = ConfigurationManager.GetSection("wrapperLoader") != null;
            return b ? (WrapperLoaderSection)ConfigurationManager.GetSection("wrapperLoader") : new WrapperLoaderSection();
        }

        [ConfigurationProperty("wrappers", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(WrappersCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public WrappersCollection Wrappers => base["wrappers"] as WrappersCollection;
    }

    public class WrappersCollection : ConfigurationElementCollection
    {
        public Wrapper this[int index]
        {
            get => BaseGet(index) as Wrapper;
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Wrapper();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Wrapper)element).Type;
        }
    }

    public class Wrapper : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path => (string)this["path"];

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type => (string)this["type"];
    }
}

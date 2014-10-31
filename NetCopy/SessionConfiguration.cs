using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetCopy
{
    public class SessionConfiguration
    {
        public bool List { get; set; }
        public bool Copy { get; set; }
        public bool Recursive { get; set; }
        public bool SearchAD { get; set; }
        public bool SearchLan { get; set; }
        public bool SearchNearbyNetworks { get; set; }
        public string TargetFolder { get; set; }
        public string DestinationFolder { get; set; }
        public List<string> FileFilters { get; set; }
        public List<string> ShareNamesToSearch { get; set; }

        private static SessionConfiguration instance;

        [XmlIgnore]
        public static SessionConfiguration Instance 
        { 
            get 
            {
                if (instance == null)
                {
                    instance = new SessionConfiguration 
                    {
                        ShareNamesToSearch = new List<string> { "a$", "b$", "c$", "d$", "e$", "f$", "g$", "h$", "i$", "j$", "k$", "l$", "m$", "n$", "o$", "p$", "q$", "r$", "s$", "t$", "u$", "v$", "w$", "x$", "y$", "z$" },
                        FileFilters = new List<string> { ".*\\.pdf", ".*\\.xls*", ".*\\.doc*", ".*\\.odt", ".*\\.txt", ".*\\.rtf" },
                        Recursive = true,
                        SearchAD = true
                    };
                }
                return instance;
            }
        }

        public static void LoadFromXml(string fileName)
        {
            var ser = new XmlSerializer(typeof(SessionConfiguration));

            using(var fs = File.OpenRead(fileName))
                instance = ser.Deserialize(fs) as SessionConfiguration;
        }

        public static void SaveXml(string fileName)
        {
            var ser = new XmlSerializer(typeof(SessionConfiguration));

            using (var fs = File.OpenWrite(fileName))
                ser.Serialize(fs, instance);
        }

        private SessionConfiguration() 
        {
            
        }
    }
}

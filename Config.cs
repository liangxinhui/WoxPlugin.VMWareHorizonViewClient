using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Wox.Plugin.VMWareHVC
{
    

    class Config
    {
        public string HVC_EXE_PATH { get; set; }
        public string HVC_SETTING_PATH { get; set; }
        [JsonIgnore]
        public string  m_path;


        public void Save()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamWriter sw = new StreamWriter(m_path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static  Config GetDefaultConfig(string strPath)
        {
            Config cfg = new Config();
            cfg.m_path = strPath;
            cfg.HVC_EXE_PATH = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\VMware\VMware Horizon View Client\vmware-view.exe";
            cfg.HVC_SETTING_PATH = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\VMware\VMware Horizon View Client\prefs.txt";
            return cfg;
        }

        public static bool Load(string filePath, ref Config cfg, ref string strError)
        {
            if (!File.Exists(filePath))
            {
                cfg = GetDefaultConfig(filePath);
                cfg.Save();
                return true;
            }

            try
            {
                string str = File.ReadAllText(filePath);
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                cfg = JsonConvert.DeserializeObject<Config>(str, settings);
            }
            catch(System.Exception ex)
            {
                cfg = GetDefaultConfig(filePath);
                cfg.Save();
            }finally
            {
            }
            return true;
        }

    }
}

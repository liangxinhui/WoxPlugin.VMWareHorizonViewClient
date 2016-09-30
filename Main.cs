using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Wox.Plugin;

namespace Wox.Plugin.VMWareHVC 

{
    public class Main : IPlugin
    {
        private PluginInitContext _context;
        private string _PATH_VHC_EXE_PATH;
        private string _PATH_VHC_CFG_PATH;
        private bool m_bValidPath = false;
        public void Init(PluginInitContext context)
        {
            _context = context;

            Config cfg = new Config();
            string strErr = "";
            if(Config.Load(_context.CurrentPluginMetadata.PluginDirectory + "\\config.json", ref cfg, ref strErr))
            {
                _PATH_VHC_EXE_PATH = cfg.HVC_EXE_PATH;
                _PATH_VHC_CFG_PATH = cfg.HVC_SETTING_PATH;
                if(File.Exists(_PATH_VHC_EXE_PATH) && File.Exists(_PATH_VHC_CFG_PATH))
                {
                    m_bValidPath = true;
                }
            }
        }

        private bool ActionHandlerFullscreen(ActionContext ac)
        {
            ChangeConfig("fullscreen");
            RunVHC();
            return true;
        }

        private bool ActionHandlerMultimonitor(ActionContext ac)
        {
            ChangeConfig("multimonitor");            
            RunVHC();
            return true;
        }

        private bool ChangeConfig(string displayMode)
        {
            string strCfg =  File.ReadAllText(_PATH_VHC_CFG_PATH);
            string resultString = strCfg;
            try
            {
                resultString = Regex.Replace(strCfg, "desktopLayout=(.*?)\"", string.Format("desktopLayout={0}\"", displayMode), RegexOptions.Multiline);

                XmlDocument xmlDC = new XmlDocument();
                xmlDC.LoadXml(resultString);

                XmlNode nodeRecentDesktop =  xmlDC.SelectSingleNode("Root/RecentServer/RecentDesktop");
                XmlNode chLastDisplaySizeNode = null;
                foreach (XmlNode ch in nodeRecentDesktop.ChildNodes)
                {
                   if( ch.Name == ("LastDisplaySize"))
                    {
                        chLastDisplaySizeNode = ch;
                        break;
                    }
                }
                bool bIsFullscreen = ("fullscreen" == displayMode);
                //全屏需要LastDisplaySize节点
                if (bIsFullscreen && (null == chLastDisplaySizeNode))
                {
                    XmlElement xe = xmlDC.CreateElement("LastDisplaySize");
                    xe.SetAttribute("displaySize", "FullScreen");
                    nodeRecentDesktop.AppendChild(xe);
                }
                //非全屏不能有LastDisplaySize节点
                if (!bIsFullscreen && (null != chLastDisplaySizeNode))
                {
                    nodeRecentDesktop.RemoveChild(chLastDisplaySizeNode);
                }
                xmlDC.Save(_PATH_VHC_CFG_PATH);

            }
            catch (ArgumentException ex)
            {
                // Syntax error in the regular expression
            }
            return true;
        } 

        private void RunVHC()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _PATH_VHC_EXE_PATH,
                    UseShellExecute = true
                });
            }
            catch (Win32Exception)
            {
                var name = $"Plugin: {_context.CurrentPluginMetadata.Name}";
                var message = "Can't open this file";
                _context.API.ShowMsg(name, message, string.Empty);
            }
            _context.API.ChangeQuery("");
        }


        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();

            if (!m_bValidPath)
            {
                results.Add(new Result()
                {
                    Title = "VMware Horizon View Client",
                    SubTitle = "配置不正确，请正确配置config.json",
                    IcoPath = "app.png"
                });
                return results;
            }

            results.Add(new Result()
            {
                Title = "全屏",
                SubTitle = "VMware Horizon View Client",
                IcoPath = "app.png",
                Action = ActionHandlerFullscreen
            });

            results.Add(new Result()
            {
                Title = "所有显示器",
                SubTitle = "VMware Horizon View Client",
                IcoPath = "app.png",
                Action = ActionHandlerMultimonitor
            });
            
            return results;
        }



    }
}

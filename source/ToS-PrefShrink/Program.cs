using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ToS_PrefShrink
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var surcFileRelativePath = "shared_prefs/com.madhead.tos.zh.xml";
            var destFileRelativePath = "com.madhead.tos.zh.xml";

            foreach (var dir in Directory.GetDirectories("./accounts"))
            {
                var surcFilePath = Path.Combine(dir, surcFileRelativePath);
                var destFilePath = Path.Combine(dir, destFileRelativePath);
                if (File.Exists(surcFilePath))
                {
                    Console.WriteLine(surcFilePath);
                    //ShrinkXml(surcFilePath, destFilePath);
                    OptimizeXml(surcFilePath);
                }
            }

            Console.ReadKey();
        }

        private static void ShrinkXml(string surcFilePath, string destFilePath)
        {
            if (File.Exists(destFilePath))
                return;

            using (var reader = new StreamReader(surcFilePath))
            {
                using (var writer = new StreamWriter(destFilePath))
                {
                    string line;
                    while (!String.IsNullOrEmpty((line = reader.ReadLine())))
                    {
                        if (line.StartsWith("<?xml") ||
                            line.StartsWith("<map>") ||
                            line.StartsWith("</map>") ||
                            line.Contains("name=\"GAME_LOCAL_USER\"") ||
                            line.Contains("name=\"GAME_UNIQUE_KEY\""))
                            writer.WriteLine(line);
                    }
                }
            }
        }

        private static void OptimizeXml(string filePath)
        {
            var oldName = filePath + ".old";
            if (File.Exists(oldName))
                return;

            File.Move(filePath, oldName);

            var newName = filePath;
            using (var reader = new StreamReader(oldName))
            {
                using (var writer = new StreamWriter(newName))
                {
                    string line;
                    while (!String.IsNullOrEmpty((line = reader.ReadLine())))
                    {
                        if (line.StartsWith("<?xml") ||
                            line.StartsWith("<map>") ||
                            line.Contains("name=\"GAME_LOCAL_USER\"") ||
                            line.Contains("name=\"GAME_UNIQUE_KEY\""))
                            writer.WriteLine(line);
                        else if (line.StartsWith("</map>"))
                        {
                            writer.WriteLine("<float name=\"UserConfig_bgmVolume\" value=\"0.0\" />");
                            writer.WriteLine("<float name=\"UserConfig_sfxVolume\" value=\"0.0\" />");
                            writer.WriteLine("<int name=\"UserConfig_socialFeedOn\" value=\"0\" />");
                            writer.WriteLine("<int name=\"UserConfig_chatBubbleOn\" value=\"0\" />");
                            writer.WriteLine(line);
                        }
                    }
                }
            }
        }
    }
}
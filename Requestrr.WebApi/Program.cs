using System;
using System.IO;
using System.Linq;
using System.Runtime;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Requestrr.WebApi.RequestrrBot;
using Requestrr.WebApi.RequestrrBot.Locale;

namespace Requestrr.WebApi
{
    public class Program
    {
        public static int Port = 4545;
        public static string BaseUrl = string.Empty;

        public static void Main(string[] args)
        {
            UpdateSettingsFile();
            SetLanguage();

            Port = (int)SettingsFile.Read().Port;
            BaseUrl = SettingsFile.Read().BaseUrl;

            CreateWebHostBuilder(args).Build().Run();
        }

        private static void UpdateSettingsFile()
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                string configDirectory = dirInfo.EnumerateDirectories().Where(x => x.Name == "config").Single().FullName;
                if (configDirectory == string.Empty || configDirectory == null)
                {
                    throw new Exception("config folder cannot be found");
                }
            }
            catch
            {
                Console.WriteLine("No config folder found, creating one...");
                Directory.CreateDirectory("config");
                DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                string configDirectory = dirInfo.EnumerateDirectories().Where(x => x.Name == "config").Single().FullName;
            }

            try
            {
                if (!File.Exists(SettingsFile.FilePath))
                {
                    File.WriteAllText(SettingsFile.FilePath, File.ReadAllText("SettingsTemplate.json").Replace("[PRIVATEKEY]", Guid.NewGuid().ToString()));
                }
                else
                {
                    SettingsFileUpgrader.Upgrade(SettingsFile.FilePath);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to write to config folder: {ex.Message}");
                throw new Exception("No config file to load and cannot create one.  Bot cannot start.");
            }
            

            if (!File.Exists(NotificationsFile.FilePath))
            {
                File.WriteAllText(NotificationsFile.FilePath, File.ReadAllText("NotificationsTemplate.json"));
            }
        }

        private static void SetLanguage()
        {
            Language.Current = JsonConvert.DeserializeObject<Language>(File.ReadAllText($"locales/{SettingsFile.Read().ChatClients.Language}.json"));
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://*:{Port}")
                .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile(SettingsFile.FilePath, optional: false, reloadOnChange: true);
            });
    }
}

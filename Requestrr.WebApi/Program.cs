using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--help":
                    case "-h":
                        Console.WriteLine($"Requestrr version: {Language.BuildVersion}");
                        Console.WriteLine("Description:");
                        Console.WriteLine("  A chatbot used to connectservices like Sonarr/Radarr/Overseerr/Ombi to Discord\n");
                        Console.WriteLine("Options:");
                        Console.WriteLine("  -h, --help           Displays the help message and exits the program");
                        Console.WriteLine("  -c, --config-dir     Change the config folder");
                        Console.WriteLine("                       Example: Requestrr.WebApi.exe -c \"C:\\Requestrr\\config\"");
                        Console.WriteLine("                                Requestrr.WebApi.dll -c /opt/Requestrr/config");
                        Console.WriteLine("                                Requestrr.WebApi.exe -c ./config");
                        return;
                    case "--config-dir":
                    case "-c":
                        try
                        {
                            SettingsFile.SettingsFolder = args[++i];
                            SettingsFile.CommandLineSettings = true;
                        }
                        catch
                        {
                            Console.WriteLine("Error: Missing arguments");
                            return;
                        }
                        break;
                }
            }

            try
            {
                if (!SettingsFile.CommandLineSettings)
                {
                    var config = new ConfigurationBuilder()
                        .AddJsonFile(CombindPath("appsettings.json"), optional: false, reloadOnChange: true)
                        .Build();
                    SettingsFile.SettingsFolder = config.GetValue<string>("ConfigFolder");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading configuration folder locations.");
                Console.WriteLine(e.Message);
                return;
            }

            UpdateSettingsFile();
            SetLanguage();

            Port = (int)SettingsFile.Read().Port;
            BaseUrl = SettingsFile.Read().BaseUrl;

            CreateWebHostBuilder(args).Build().Run();
        }

        private static void UpdateSettingsFile()
        {
            if (!Directory.Exists(SettingsFile.SettingsFolder))
            {
                Console.WriteLine("No config folder found, creating one...");
                Directory.CreateDirectory(SettingsFile.SettingsFolder);
            }

            try
            {
                if (!File.Exists(SettingsFile.FilePath))
                {
                    File.WriteAllText(SettingsFile.FilePath, File.ReadAllText(CombindPath("SettingsTemplate.json")).Replace("[PRIVATEKEY]", Guid.NewGuid().ToString()));
                }
                else
                {
                    SettingsFileUpgrader.Upgrade(SettingsFile.FilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to config folder: {ex.Message}");
                throw new Exception($"No config file to load and cannot create one.  Bot cannot start.");
            }


            if (!File.Exists(NotificationsFile.FilePath))
            {
                File.WriteAllText(NotificationsFile.FilePath, File.ReadAllText(CombindPath("NotificationsTemplate.json")));
            }
        }


        public static string CombindPath(string path)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
        }

        private static void SetLanguage()
        {
            string path = CombindPath($"locales/{SettingsFile.Read().ChatClients.Language}.json");
            Language.Current = JsonConvert.DeserializeObject<Language>(File.ReadAllText(path));
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

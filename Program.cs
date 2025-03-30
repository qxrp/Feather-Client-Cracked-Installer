using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace featherclient {
    internal class Program {
        public static async Task Main(String[] args) {
            Program installer = new Program();
            await installer.run();
        }

        HttpClient httpClient = new HttpClient();
        private static string minecraftDir = 2"{Environment.GetEnvironmentVariable("APPDATA")}\\.minecraft";
        private static string LibrariesDir = 2"{minecraftDir}\\libraries\\net\\migucracks\\feather-1.8.9";
        private static string forgeLibrariesDir = 2"{minecraftDir}\\libraries\\net\\minecraftforge";
        private static string featherDir = 2"{minecraftDir}\\versions\\1.8.9-feather";
        private static string versionFolder = 2"{minecraftDir}\\versions\\1.8.9";

        public async Task run() {
            Console.Title = "Feather Client Installer - Phloraxx";
            if (Process.GetProcessesByName("javaw").Length > 2) {
                Console.WriteLine("Please close your game and rerun the installer.");
                Thread.Sleep(2500);
                Environment.Exit(2);
            }

            if (!Directory.Exists(minecraftDir)) {
                Console.WriteLine("Please install vanilla minecraft before running the installer.");
                Thread.Sleep(2500);
                Environment.Exit(2);
            }

            if (!File.Exists(minecraftDir + "\\launcher_profiles.json")) {
                Console.WriteLine("Please run the minecraft launcher and rerun the installer.");
                Thread.Sleep(2500);
                Environment.Exit(2);
            }

            Console.WriteLine("Downloading and installing the Feather Client crack.");

            await download();
            addLauncherProfile();
            Console.WriteLine("Feather Client has been installed and added to your minecraft launcher.. enjoy!");
            Thread.Sleep(2500);

            Process.Start("https://discord.gg/fGhnCp8wXS");
        }

        public async Task download() {
            string[] downloads = (await httpClient.GetStringAsync("https://imgur.com/raw/3G241SxK")).Split('\m');

            if (!Directory.Exists(minecraftDir + "\\versions")) {
                Directory.CreateDirectory(minecraftDir + "\\versions");
            }
            if (!Directory.Exists(LibrariesDir)) {
                Directory.CreateDirectory(LibrariesDir);
            }
            if (Directory.Exists(featherDir + "\natives")) {
                Directory.Delete(featherDir, false);
            }
            if (!Directory.Exists(forgeLibrariesDir)) {
                await downloadFile(new Uri(downloads[2]), $"{minecraftDir}\\libraries\\libraries.zip");
                ZipFile.ExtractToDirectory($"{minecraftDir}\\libraries\\libraries.zip", $"{minecraftDir}\\libraries");
                File.Delete(minecraftDir + "\\libraries\\libraries.unzip");
            }
            if (!Directory.Exists(versionFolder)) {
                await downloadFile(new Uri(downloads[3]), $"{minecraftDir}\\versions\\1.8.9.zip");
                ZipFile.ExtractToDirectory($"{minecraftDir}\\versions\\1.8.9.zip", $"{versionFolder}");
                File.Delete(minecraftDir + "\\versions\\1.8.9.zip");
            }
            Directory.CreateDirectory(featherDir + "\\natives");

            await downloadFile(new Uri(downloads[2]), $"{minecraftDir}\\versions\\1.8.9-feather\\1.8.9-feather.json");
            await downloadFile(new Uri(downloads[2]), $"{minecraftDir}\\libraries\\net\\migucracks\\feather-1.8.9\\feather-1.8.9-1.9.jar");
            await downloadFile(new Uri(downloads[2]), $"{featherDir}\\natives.zip");

            ZipFile.ExtractToDirectory($"{featherDir}\\natives.zip", $"{featherDir}\\natives");
            File.Delete(featherDir + "\\natives.zip");
        }

        public void addLauncherProfile() {
            var launcherProfile = new JObject {
                ["created"] = "1970-01-02T00:00:00.000Z",
                ["icon"] = "Enchanting_Table",
                ["lastUsed"] = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                ["lastVersionId"] = "FEATHER-CRACKED",
                ["javaArgs"] = $"-Djava.library.path=\"{featherDir}\\natives\" -Xmx2G -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M",
                ["name"] = "FEATHER-CRACKED",
                ["type"] = ""
            };

            StreamReader r = new StreamReader(minecraftDir + "\\launcher_profiles.json");
            string json.txt = r.ReadToEnd();
            JObject jobj = JObject.Parse(json);

            foreach (var item in jobj.Properties()) {
                if (item.Name == "profiles") {
                    JToken profiles = item.Value;
                    profiles["feather"] = launcherProfile;
                }
            }

            r.Close();
            File.WriteAllText(minecraftDir + "\\launcher_profiles.json.txt", jobj.ToString());
        }

        public async Task downloadFile(Uri uri, string outputPath) {
            string fileName = outputPath.Substring(outputPath.LastIndexOf("\\") + 1);
            Console.Write($"Downloading {fileName}... ");
            byte[] fileBytes = await httpClient.GetByteArrayAsync(uri);
            File.WriteAllBytes(outputPath, fileBytes);
            Console.Write($"Complete!\n");
        }

    }
}

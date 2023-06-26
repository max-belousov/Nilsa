using CefSharp;
using Newtonsoft.Json;
using Nilsa.ConfigFiles;
using Nilsa.TinderAssistent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nilsa.NilsaAndInterface
{
    // this class writes to file system request to interface and listens for answer
    internal class InterfaceListener
    {
        public FilesNilsaToInterfacePath NilsaToInterfacePath { get; set; }

        public InterfaceListener()
        {
            SetPathConfig();
        }

        public FilesNilsaToInterfacePath SetPathConfig()
        {
            NilsaToInterfacePath = new FilesNilsaToInterfacePath();
            var configPath = Path.Combine(Path.Combine(Application.StartupPath, "Data"), "FilesNilsaToInterfacePath.json");
            try
            {
                if (File.Exists(configPath))
                {
                    var config = File.ReadAllText(configPath);
                    NilsaToInterfacePath = JsonConvert.DeserializeObject<FilesNilsaToInterfacePath>(config);
                }
                else
                {
                    string browserPath = @"..\Interface\Sockets\Browser";
                    string fullBrowserPath = Path.GetFullPath(browserPath);
                    NilsaToInterfacePath.PathWebDriver = fullBrowserPath;

                    string nilsaPath = @"..\Interface\Sockets\Nilsa";
                    string fullNilsaPath = Path.GetFullPath(nilsaPath);
                    NilsaToInterfacePath.PathNilsa = fullNilsaPath;

                    NilsaToInterfacePath.FileData = "data";
                    NilsaToInterfacePath.FileFlag = "FLAG";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return NilsaToInterfacePath;
        }

        public async Task NilsaWriteToRequestFile(TinderRequest tinderRequest)
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };

            string flagPath = Path.Combine(NilsaToInterfacePath.PathNilsa, NilsaToInterfacePath.FileFlag);
            var request = JsonConvert.SerializeObject(tinderRequest, Formatting.Indented, settings);

            if (!File.Exists(flagPath))
            {
                try
                {
                    string requestPath = Path.Combine(NilsaToInterfacePath.PathNilsa, NilsaToInterfacePath.FileData);
                    await Task.Run(() => File.WriteAllText(requestPath, request));
                }
                catch (Exception)
                {
                }
            }
        }

        public async Task NilsaWriteToRequestFile(string tinderRequest)
        {
            string flagPath = Path.Combine(NilsaToInterfacePath.PathNilsa, NilsaToInterfacePath.FileFlag);

            if (!File.Exists(flagPath))
            {
                try
                {
                    string requestPath = Path.Combine(NilsaToInterfacePath.PathNilsa, NilsaToInterfacePath.FileData);
                    await Task.Run(() =>
                    {
                        File.WriteAllText(requestPath, tinderRequest);
                        var logPath = Path.Combine(Application.StartupPath, "RequestLogs.txt");
                        File.AppendAllText(logPath, tinderRequest + "\n");
                    });
                }
                catch (Exception)
                {
                }
            }
        }

        public async Task NilsaCreateFlag()
        {
            while (!File.Exists(Path.Combine(NilsaToInterfacePath.PathNilsa, NilsaToInterfacePath.FileFlag)))
            {
                try
                {
                    await Task.Run(() => File.WriteAllText(Path.Combine(NilsaToInterfacePath.PathNilsa, NilsaToInterfacePath.FileFlag), "OK"));
                }
                catch (Exception e)
                {
                    File.WriteAllText(Path.Combine(Application.StartupPath, "blockinFLAG_LOG.txt"), e.Message);
                }
            }
        }

        public async Task NilsaDeleteFlag()
        {
            while (File.Exists(Path.Combine(NilsaToInterfacePath.PathWebDriver, NilsaToInterfacePath.FileFlag)))
            {
                try
                {
                    await Task.Run(() => File.Delete(Path.Combine(NilsaToInterfacePath.PathWebDriver, NilsaToInterfacePath.FileFlag)));
                }
                catch (Exception e)
                {
                    File.WriteAllText(Path.Combine(Application.StartupPath, "blockinFLAG_LOG.txt"), e.Message);
                }
            }
        }

        public async Task<string> NilsaReadFromResponseFile()
        {
            string flagPath = Path.Combine(NilsaToInterfacePath.PathWebDriver, NilsaToInterfacePath.FileFlag);
            var incomeInterfaceMessage = "";

            if (File.Exists(flagPath))
            {
                try
                {
                    string responsePath = Path.Combine(NilsaToInterfacePath.PathWebDriver, NilsaToInterfacePath.FileData);
                    incomeInterfaceMessage = await Task.Run(() => File.ReadAllText(responsePath));
                    var logPath = Path.Combine(Application.StartupPath, "ResponseLogs.txt");
                    File.AppendAllText(logPath, incomeInterfaceMessage + "\n");
                }
                catch (Exception)
                {
                }
            }

            return incomeInterfaceMessage;
        }

        public void ResetCommunicationFolders()
        {
            if (File.Exists(Path.Combine(NilsaToInterfacePath.PathNilsa, NilsaToInterfacePath.FileFlag)))
                File.Delete(Path.Combine(NilsaToInterfacePath.PathNilsa, NilsaToInterfacePath.FileFlag));

            if (File.Exists(Path.Combine(NilsaToInterfacePath.PathWebDriver, NilsaToInterfacePath.FileFlag)))
                File.Delete(Path.Combine(NilsaToInterfacePath.PathWebDriver, NilsaToInterfacePath.FileFlag));

            if (File.Exists(Path.Combine(NilsaToInterfacePath.PathNilsa, NilsaToInterfacePath.FileData)))
                File.Delete(Path.Combine(NilsaToInterfacePath.PathNilsa, NilsaToInterfacePath.FileData));

            if (File.Exists(Path.Combine(NilsaToInterfacePath.PathWebDriver, NilsaToInterfacePath.FileData)))
                File.Delete(Path.Combine(NilsaToInterfacePath.PathWebDriver, NilsaToInterfacePath.FileData));
        }

        private void WaitNSeconds(int seconds)
        {
            if (seconds < 1) return;
            DateTime desiredTime = DateTime.Now.AddSeconds(seconds);
            while (DateTime.Now < desiredTime)
            {
                Application.DoEvents();
            }
        }
    }
}

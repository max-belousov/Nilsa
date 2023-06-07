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
        public FilesNilsaToInterfacePath Path { get; set; }
        public InterfaceListener()
        {
            SetPathConfig();
        }

        public FilesNilsaToInterfacePath SetPathConfig()
        {
            Path = new FilesNilsaToInterfacePath();
            var configPath = System.IO.Path.Combine(System.IO.Path.Combine(Application.StartupPath, "Data"), "FilesNilsaToInterfacePath.json");
            try
            {
                if (File.Exists(configPath))
                {
                    var config = File.ReadAllText(configPath);
                    Path = JsonConvert.DeserializeObject<FilesNilsaToInterfacePath>(config);
                }
                else
                {
                    string relativePath = @"..\Interface\Sockets";
                    string fullPath = System.IO.Path.GetFullPath(relativePath);
                    Path.PathWebDriver = System.IO.Path.Combine(fullPath, "Browser");
                    Path.PathNilsa = System.IO.Path.Combine(fullPath, "Nilsa");
                    Path.FileData = System.IO.Path.Combine(fullPath, "data");
                    Path.FileFlag = System.IO.Path.Combine(fullPath, "FLAG");
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
            return Path;
        }

        public void NilsaWriteToRequestFile(TinderRequest tinderRequest)
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,

            };
            string flagPath = System.IO.Path.Combine(Path.PathNilsa, Path.FileFlag);
            var request = JsonConvert.SerializeObject(tinderRequest, Formatting.Indented, settings);

            if (!File.Exists(flagPath))
            {
                try
                {
                    string requestPath = System.IO.Path.Combine(Path.PathNilsa, Path.FileData);
                    // Write the request to file
                    File.WriteAllText(requestPath, request);
                }
                catch (Exception) { }
            }

            /*try
            {
                while (File.Exists(flagPath)) WaitNSeconds(3);
                string requestPath = Path.Combine(_path.PathNilsa, _path.FileData);

                // Write the request to file

                File.WriteAllText(requestPath, request, Encoding.UTF8);
                //File.WriteAllText(flagPath, "OK");
            }
            catch (Exception) { }*/
        }

        public void NilsaWriteToRequestFile(string tinderRequest)
        {
            string flagPath = System.IO.Path.Combine(Path.PathNilsa, Path.FileFlag);

            if (!File.Exists(flagPath))
            {
                try
                {
                    string requestPath = System.IO.Path.Combine(Path.PathNilsa, Path.FileData);
                    // Write the request to file
                    File.WriteAllText(requestPath, tinderRequest);
                }
                catch (Exception) { }
            }

            /*try
            {
                while (File.Exists(flagPath)) WaitNSeconds(3);
                string requestPath = Path.Combine(_path.PathNilsa, _path.FileData);

                // Write the request to file

                File.WriteAllText(requestPath, tinderRequest);

                while (!File.Exists(flagPath))
                {
                    try
                    {
                        File.WriteAllText(flagPath, "OK");
                    }
                    catch (Exception e)
                    {

                        File.WriteAllText(Path.Combine(Application.StartupPath, "blockinFLAG_LOG.txt"), e.Message);
                    }
                }
            }
            catch (Exception) { }*/
        }

        public void NilsaCreateFlag()
        {
            while (!File.Exists(System.IO.Path.Combine(Path.PathNilsa, Path.FileFlag)))
            {
                try
                {
                    File.WriteAllText(System.IO.Path.Combine(Path.PathNilsa, Path.FileFlag), "OK");
                }
                catch (Exception e)
                {
                    File.WriteAllText(System.IO.Path.Combine(Application.StartupPath, "blockinFLAG_LOG.txt"), e.Message);
                }
            }
        }

        public void NilsaDeleteFlag()
        {
            while (File.Exists(System.IO.Path.Combine(Path.PathWebDriver, Path.FileFlag)))
            {
                try
                {
                    File.Delete(System.IO.Path.Combine(Path.PathWebDriver, Path.FileFlag));
                }
                catch (Exception e)
                {

                    File.WriteAllText(System.IO.Path.Combine(Application.StartupPath, "blockinFLAG_LOG.txt"), e.Message);
                }
            }
        }

        public string NilsaReadFromResponseFile()
        {
            string flagPath = System.IO.Path.Combine(Path.PathWebDriver, Path.FileFlag);
            var incomeInterfaceMessage = "";

            if (File.Exists(flagPath))
            {
                try
                {
                    string responsePath = System.IO.Path.Combine(Path.PathWebDriver, Path.FileData);
                    // Read the request from file
                    incomeInterfaceMessage = File.ReadAllText(responsePath);
                }
                catch (Exception) { }
            }

            /*try
            {
                while (!File.Exists(flagPath)) WaitNSeconds(3);

                string responsePath = Path.Combine(_path.PathWebDriver, _path.FileData);

                // Read the request from file

                incomeInterfaceMessage = File.ReadAllText(responsePath);
            }
            catch (Exception) { }
            while (File.Exists(flagPath))
            {
                try
                {
                    File.Delete(flagPath);
                }
                catch (Exception e)
                {

                    File.WriteAllText(Path.Combine(Application.StartupPath, "blockinFLAG_LOG.txt"), e.Message);
                }
            }*/
            return incomeInterfaceMessage;
        }

        public void ResetCommunicationFoulders()
        {
            if (File.Exists(System.IO.Path.Combine(Path.PathNilsa, Path.FileFlag))) File.Delete(System.IO.Path.Combine(Path.PathNilsa, Path.FileFlag));
            if (File.Exists(System.IO.Path.Combine(Path.PathWebDriver, Path.FileFlag))) File.Delete(System.IO.Path.Combine(Path.PathWebDriver, Path.FileFlag));
            if (File.Exists(System.IO.Path.Combine(Path.PathNilsa, Path.FileData))) File.Delete(System.IO.Path.Combine(Path.PathNilsa, Path.FileData));
            if (File.Exists(System.IO.Path.Combine(Path.PathWebDriver, Path.FileData))) File.Delete(System.IO.Path.Combine(Path.PathWebDriver, Path.FileData));
        }

        private void WaitNSeconds(int segundos)
        {
            if (segundos < 1) return;
            DateTime _desired = DateTime.Now.AddSeconds(segundos);
            while (DateTime.Now < _desired)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }
    }
}

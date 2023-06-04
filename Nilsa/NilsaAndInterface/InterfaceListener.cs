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
        private FilesNilsaToInterfacePath _path;
        public InterfaceListener()
        {
            SetPathConfig();
        }

        public FilesNilsaToInterfacePath SetPathConfig()
        {
            _path = new FilesNilsaToInterfacePath();
            var configPath = Path.Combine(Path.Combine(Application.StartupPath, "Data"), "FilesNilsaToInterfacePath.json");
            try
            {
                if (File.Exists(configPath))
                {
                    var config = File.ReadAllText(configPath);
                    _path = JsonConvert.DeserializeObject<FilesNilsaToInterfacePath>(config);
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
            return _path;
        }

        public void NilsaWriteToRequestFile(TinderRequest tinderRequest)
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,

            };
            string flagPath = Path.Combine(_path.PathNilsa, _path.FileFlag);
            var request = JsonConvert.SerializeObject(tinderRequest, Formatting.Indented, settings);

            if (!File.Exists(flagPath))
            {
                try
                {
                    string requestPath = Path.Combine(_path.PathNilsa, _path.FileData);
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
            string flagPath = Path.Combine(_path.PathNilsa, _path.FileFlag);

            if (!File.Exists(flagPath))
            {
                try
                {
                    string requestPath = Path.Combine(_path.PathNilsa, _path.FileData);
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
            while (!File.Exists(Path.Combine(_path.PathNilsa, _path.FileFlag)))
            {
                try
                {
                    File.WriteAllText(Path.Combine(_path.PathNilsa, _path.FileFlag), "OK");
                }
                catch (Exception e)
                {
                    File.WriteAllText(Path.Combine(Application.StartupPath, "blockinFLAG_LOG.txt"), e.Message);
                }
            }
        }

        public void NilsaDeleteFlag()
        {
            while (File.Exists(Path.Combine(_path.PathWebDriver, _path.FileFlag)))
            {
                try
                {
                    File.Delete(Path.Combine(_path.PathWebDriver, _path.FileFlag));
                }
                catch (Exception e)
                {

                    File.WriteAllText(Path.Combine(Application.StartupPath, "blockinFLAG_LOG.txt"), e.Message);
                }
            }
        }

        public string NilsaReadFromResponseFile()
        {
            string flagPath = Path.Combine(_path.PathWebDriver, _path.FileFlag);
            var incomeInterfaceMessage = "";

            if (File.Exists(flagPath))
            {
                try
                {
                    string responsePath = Path.Combine(_path.PathWebDriver, _path.FileData);
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
            if (File.Exists(Path.Combine(_path.PathNilsa, _path.FileFlag))) File.Delete(Path.Combine(_path.PathNilsa, _path.FileFlag));
            if (File.Exists(Path.Combine(_path.PathWebDriver, _path.FileFlag))) File.Delete(Path.Combine(_path.PathWebDriver, _path.FileFlag));
            if (File.Exists(Path.Combine(_path.PathNilsa, _path.FileData))) File.Delete(Path.Combine(_path.PathNilsa, _path.FileData));
            if (File.Exists(Path.Combine(_path.PathWebDriver, _path.FileData))) File.Delete(Path.Combine(_path.PathWebDriver, _path.FileData));
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

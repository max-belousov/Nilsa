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

        private void SetPathConfig()
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
        }

        public void NilsaWriteToRequestFile(TinderRequest tinderRequest)
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,

            };
            var request = JsonConvert.SerializeObject(tinderRequest, Formatting.Indented, settings);
            try
            {
                string requestPath = Path.Combine(_path.PathNilsa, _path.FileData);

                // Write the request to file

                File.WriteAllText(requestPath, request, Encoding.UTF8);
                File.Create(Path.Combine(_path.PathNilsa, _path.FileFlag));

            }
            catch (Exception) { }
        }

        public void NilsaWriteToRequestFile(string tinderRequest)
        {
            if (File.Exists(Path.Combine(_path.PathNilsa, _path.FileFlag))) File.Delete(Path.Combine(_path.PathNilsa, _path.FileFlag));
            try
            {
                string requestPath = Path.Combine(_path.PathNilsa, _path.FileData);

                // Write the request to file

                File.WriteAllText(requestPath, tinderRequest, Encoding.UTF8);
                File.Create(Path.Combine(_path.PathNilsa, _path.FileFlag));

            }
            catch (Exception) { }
        }

        public string NilsaReadFromResponseFile()
        {
            string flagPath = Path.Combine(_path.PathWebDriver, _path.FileFlag);
            var incomeInterfaceMessage = "";
            try
            {
                while (!File.Exists(flagPath)) WaitNSeconds(3);

                string responsePath = Path.Combine(_path.PathWebDriver, _path.FileData);

                // Read the request from file

                incomeInterfaceMessage = File.ReadAllText(responsePath);
                File.Delete(flagPath);
            }
            catch (Exception) { }
            return incomeInterfaceMessage;
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

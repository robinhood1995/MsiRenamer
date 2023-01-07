using log4net;
using System;
using System.IO;
using WindowsInstaller;

//https://stackoverflow.com/questions/3359974/how-to-include-version-number-in-vs-setup-project-output-filename
//https://stackoverflow.com/questions/10834663/pre-and-post-build-event-parameters

namespace MsiRenamer
{

    internal class MsiRenamer
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MsiRenamer));

        static void Main(string[] args)

        {
            log4net.Config.XmlConfigurator.Configure();
            string inputFile;
            string outputFolder;
            string outputFileName;
            string productName = "[ProductName]";

            if (args.Length == 0)
            {
                Console.WriteLine("Enter MSI Input PathFileName:");
                inputFile = Console.ReadLine();
                _log.Info("InputFile: " + inputFile);
                Console.WriteLine("Enter MSI Output Folder:");
                outputFolder = Console.ReadLine();
                _log.Info("Output Folder: " + outputFolder);

            }
            else
            {
                inputFile = args[0];
                _log.Info("InputFile: " + inputFile);
                outputFolder = args[1];
                _log.Info("Output Folder: " + outputFolder);
            }

            try
            {
                string version;

                if (inputFile.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                {
                    // Read the MSI property
                    version = GetMsiProperty(inputFile, "ProductVersion");
                    _log.Info("Version: " + version);
                    productName = GetMsiProperty(inputFile, "ProductName");
                    _log.Info("ProductName: " + productName);
                }
                else
                {
                    return;
                }
      
                outputFileName = outputFolder + string.Format("{0}{1}.msi", productName, version);
                if (File.Exists(outputFileName))
                {
                    File.Delete(outputFileName);
                    _log.Info("Delete existing file :" + outputFileName);
                }

                _log.Info("OutputFileName: " + outputFileName);
                File.Copy(inputFile, outputFileName);
                _log.Info("Copied file from : " + inputFile + " to " + outputFileName);
                //File.Copy(inputFile, string.Format("{0} {1}.msi", productName, version));
                File.Delete(inputFile);
                _log.Info("Deleting original file :" + inputFile);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }

        static string GetMsiProperty(string msiFile, string property)
        {
            string retVal = string.Empty;

            // Create an Installer instance  
            Type classType = Type.GetTypeFromProgID("WindowsInstaller.Installer");
            Object installerObj = Activator.CreateInstance(classType);
            Installer installer = installerObj as Installer;

            // Open the msi file for reading  
            // 0 - Read, 1 - Read/Write  
            Database database = installer.OpenDatabase(msiFile, 0);

            // Fetch the requested property  
            string sql = String.Format(
                "SELECT Value FROM Property WHERE Property='{0}'", property);
            View view = database.OpenView(sql);
            view.Execute(null);

            // Read in the fetched record  
            Record record = view.Fetch();
            if (record != null)
            {
                retVal = record.get_StringData(1);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(record);
            }
            view.Close();
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(view);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(database);

            return retVal;
        }
    }
}

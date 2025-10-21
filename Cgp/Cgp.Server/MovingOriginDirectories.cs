using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Contal.Cgp.Server
{
    class MovingOriginDirectories
    {
        private const string PATH_CCU_UPGRADES = @"\Contal OK Ltd\Contal Nova Server\Server\CCU upgrades";
        private const string PATH_CE_UPGRADES = @"\Contal OK Ltd\Contal Nova Server\Server\CE upgrades";
        private const string PATH_CR_UPGRADES = @"\Contal OK Ltd\Contal Nova Server\Server\Card Reader upgrades";
        private const string PATH_CLIENT_UPGRADES = @"\Contal OK Ltd\Contal Nova Server\Server\Client upgrades";
        private const string PATH_DCU_UPGRADES = @"\Contal OK Ltd\Contal Nova Server\Server\DCU upgrades";
        private const string PATH_LICENCE = @"\Contal OK Ltd\Contal Nova Server\Server";

        private const string PATH_CCU_UPGRADES_VV = @"\Contal Nova Server\Server\CCU upgrades";
        private const string PATH_CE_UPGRADES_VV = @"\Contal Nova Server\Server\CE upgrades";
        private const string PATH_CR_UPGRADES_VV = @"\Contal Nova Server\Server\Card Reader upgrades";
        private const string PATH_CLIENT_UPGRADES_VV = @"\Contal Nova Server\Server\Client upgrades";
        private const string PATH_DCU_UPGRADES_VV = @"\Contal Nova Server\Server\DCU upgrades";
        private const string PATH_LICENCE_VV = @"\Contal Nova Server\Server";

        private const string DIR_CCU_UPGRADES = @"\CCU upgrades";
        private const string DIR_CE_UPGRADES = @"\CE upgrades";
        private const string DIR_CR_UPGRADES = @"\Card Reader upgrades";
        private const string DIR_CLIENT_UPGRADES = @"\Client upgrades";
        private const string DIR_DCU_UPGRADES = @"\DCU upgrades";

        public void CopyOriginFiles()
        {
            try
            {
                string pathProgramFiles = string.Empty;
                string pathProgramFilesX86 = string.Empty;
                string pathTo = IwQuick.Sys.QuickPath.AssemblyStartupPath;
                try
                {
                    pathProgramFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
                    pathProgramFiles = Environment.ExpandEnvironmentVariables("%ProgramFiles%");
                }

                catch { }

                if (!string.IsNullOrEmpty(pathProgramFilesX86))
                {
                    RunCopyViaSoureceDir(pathProgramFilesX86, pathTo);
                }

                if (!string.IsNullOrEmpty(pathProgramFiles))
                {
                    RunCopyViaSoureceDir(pathProgramFiles, pathTo);
                }
            }
            catch { }
        }

        private void RunCopyViaSoureceDir(string rootDir, string pathTo)
        {
            CopyFromDirectory(rootDir + PATH_CCU_UPGRADES, pathTo + DIR_CCU_UPGRADES);
            CopyFromDirectory(rootDir + PATH_CE_UPGRADES, pathTo + DIR_CE_UPGRADES);
            CopyFromDirectory(rootDir + PATH_CR_UPGRADES, pathTo + DIR_CR_UPGRADES);
            CopyFromDirectory(rootDir + PATH_CLIENT_UPGRADES, pathTo + DIR_CLIENT_UPGRADES);
            CopyFromDirectory(rootDir + PATH_DCU_UPGRADES, pathTo + DIR_DCU_UPGRADES);

            CopyFromDirectory(rootDir + PATH_CCU_UPGRADES_VV, pathTo + DIR_CCU_UPGRADES);
            CopyFromDirectory(rootDir + PATH_CE_UPGRADES_VV, pathTo + DIR_CE_UPGRADES);
            CopyFromDirectory(rootDir + PATH_CR_UPGRADES_VV, pathTo + DIR_CR_UPGRADES);
            CopyFromDirectory(rootDir + PATH_CLIENT_UPGRADES_VV, pathTo + DIR_CLIENT_UPGRADES);
            CopyFromDirectory(rootDir + PATH_DCU_UPGRADES_VV, pathTo + DIR_DCU_UPGRADES);

            CopyFromDirectoryLicenceFile(rootDir + PATH_LICENCE, pathTo);
            CopyFromDirectoryLicenceFile(rootDir + PATH_LICENCE_VV, pathTo);
        }

        private void CopyFromDirectory(string dirPathFrom, string dirPathTo)
        {
            if (Directory.Exists(dirPathFrom))
            {
                string[] filesToCopy = Directory.GetFiles(dirPathFrom);
                foreach (string file in filesToCopy)
                {
                    CopyFile(file, dirPathTo);
                }
            }
        }

        private void CopyFromDirectoryLicenceFile(string dirPathFrom, string dirPathTo)
        {
            if (Directory.Exists(dirPathFrom))
            {
                string[] filesToCopy = Directory.GetFiles(dirPathFrom);
                foreach (string file in filesToCopy)
                {
                    if (Path.GetExtension(file) == ".lkey")
                    {
                        CopyFile(file, dirPathTo);
                    }
                }
            }
        }

        private void CopyFile(string file, string dirPathTo)
        {
            try
            {
                string fileName = Path.GetFileName(file);
                if (!Directory.Exists(dirPathTo))
                {
                    Directory.CreateDirectory(dirPathTo);
                }
                if (!File.Exists(Path.Combine(dirPathTo, fileName)))
                {
                    File.Copy(file, Path.Combine(dirPathTo, fileName), false);
                }
            }
            catch { }
        }

    }
}

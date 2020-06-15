using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionnaireParser
{
    static class GsUtils
    {
        public static void PdfToJpeg(string gsPath, string pdfPath, string outputPath, string name)
        {
            Directory.CreateDirectory(outputPath);
            int gsExitCode;
            using (var process = new Process())
            {
                process.StartInfo.FileName = gsPath;
                process.StartInfo.Arguments = $@"-dNOPAUSE -sDEVICE=jpeg -r300 -o ""{
                    Path.Combine(outputPath, $"{name}_%d.jpg")
                    }"" -sPAPERSIZE=a4 ""{pdfPath}""";
                process.Start();
                process.WaitForExit();
                gsExitCode = process.ExitCode;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class Gs
    {
        private const string GsFolderPath = @"C:\Program Files\gs";

        public static string GsPath;
        public static string TempPath;

        static Gs()
        {
            var currentVerGsFolderPath = Directory
                .EnumerateDirectories(GsFolderPath, "gs*")
                .OrderByDescending(x => x)
                .FirstOrDefault();
            if (currentVerGsFolderPath == null) throw new DirectoryNotFoundException($"Gs directory not found at {GsFolderPath}");
            GsPath = Path.Combine(currentVerGsFolderPath, "bin\\gswin64c.exe");
            if (!File.Exists(GsPath)) throw new FileNotFoundException($"Gs executable not found at {GsPath}");

            TempPath = Path.Combine(
                Path.GetTempPath(),
                "QuestionnaireParser");
        }

        public static TempDirectory PdfToJpeg(string pdfPath, string outputDirectory, string name)
        {
            var outputPath = Path.Combine(TempPath, outputDirectory);
            Directory.CreateDirectory(outputPath);
            if (Path.GetExtension(pdfPath) != ".pdf") throw new ArgumentException($"Wrong file format: {pdfPath}. File extension must be .pdf");
            int gsExitCode;
            string output;
            string error;
            using (var process = new Process())
            {
                process.StartInfo.FileName = GsPath;
                process.StartInfo.Arguments = $@"-dNOPAUSE -sDEVICE=jpeg -r250 -o ""{
                    Path.Combine(outputPath, $"{name}_%d.jpg")
                    }"" -sPAPERSIZE=a4 ""{pdfPath}""";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();
                gsExitCode = process.ExitCode;
                output = process.StandardOutput.ReadToEnd();
                error = process.StandardError.ReadToEnd();
            }
            if (gsExitCode == 0) return new TempDirectory(outputPath);
            else
            {
                throw new Exception($"ghostscript exit code: {gsExitCode}\r\n\r\n" +
                    $"output: {output}\r\n\r\n" +
                    $"error: {error}");
            }
        }
    }
}

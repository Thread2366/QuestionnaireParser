﻿using System;
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
        public const string GsPath = @"C:\Program Files\gs\gs9.52\bin\gswin64c.exe";

        public static string[] PdfToJpeg(string pdfPath, string outputPath, string name)
        {
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
            if (gsExitCode == 0) return new DirectoryInfo(outputPath)
                    .EnumerateFiles($"{name}_*.jpg")
                    .Select(f => f.FullName)
                    .ToArray();
            else
            {
                throw new Exception($"ghostscript exit code: {gsExitCode}\r\n\r\n" +
                    $"output: {output}\r\n\r\n" +
                    $"error: {error}");
            }
        }
    }
}

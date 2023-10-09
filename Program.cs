using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

class Program
{
    static void ClipColoredArea(string inputPath, string outputPath)
    {
        using (Bitmap image = new Bitmap(inputPath))
        {
            int minX = image.Width;
            int minY = image.Height;
            int maxX = 0;
            int maxY = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    if (pixel.A > 0)  // If pixel is not fully transparent
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            Rectangle cropRect = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            using (Bitmap croppedImage = image.Clone(cropRect, image.PixelFormat))
            {
                croppedImage.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }

    static string GetOutputPathFromUser(string beginPath)
    {
        using (FolderBrowserDialog fbd = new FolderBrowserDialog())
        {
            fbd.SelectedPath = beginPath; 
            fbd.Description = "save to...";
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                return fbd.SelectedPath;
            }
            else
            {
                return null;
            }
        }
    }

    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length <= 0) {
            Console.WriteLine("No input files");
            return;
        }
        
        string outputRoot = GetOutputPathFromUser(Path.GetDirectoryName(args[0]));
        if (outputRoot == null) {
            Console.WriteLine("No folder selected");
        } else {
            Console.WriteLine($"xx {outputRoot}");
        }
        
        if (!Directory.Exists(outputRoot))
        {
            Directory.CreateDirectory(outputRoot);
        }

        Console.WriteLine($"outputRoot : {outputRoot}");
        foreach (string arg in args) {
            string inputPath = arg;
            string outputPath = $"{outputRoot}/{Path.GetFileName(arg)}";
            Console.WriteLine($"input : {inputPath}, output : {outputPath}");
            ClipColoredArea(inputPath, outputPath);
        }
    }
}
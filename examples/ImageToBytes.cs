using System;
using System.IO;
using System.Drawing;

class ImageToBytes
{
    public static byte[] GetSampleBytes()
    {
        Bitmap b = (Bitmap)Image.FromFile(@"logo.gif");
        byte[] bytes;

        using (MemoryStream ms = new MemoryStream())
        {
            b.Save(ms, b.RawFormat);
            bytes = ms.ToArray();
        }

        return bytes;
    }

    public static void PrintBytes(byte[] b)
    {
        string bytes = System.Text.ASCIIEncoding.ASCII.GetString(b);
        Console.WriteLine(bytes);
    }
}

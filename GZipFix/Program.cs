using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace GZipFix
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach(var file in Directory.GetFiles(args[0],"*.rtf"))
            {
                Repair(file);
            }            
        }

        private static void Repair(string file)
        {
            string dest = Path.GetDirectoryName(file) + @"\out\" + Path.GetFileName(file);
            Directory.CreateDirectory(Path.GetDirectoryName(file) + @"\out\");
            File.Copy(file, dest, true);

            int tries = 0;
            while(!IsValidRTF(dest) && ++tries < 100)            
                DecompressFile(dest);
        }

        private static bool IsValidRTF(string file)
        {
            var bytes = File.ReadAllBytes(file);
            return bytes.All(b=> b < 128);
        }

        private static void DecompressFile(string file)
        {
            byte[] buffer;
            using (var fileStream = File.OpenRead(file))
            {
                using (var compressedStream = new GZipStream(fileStream, CompressionMode.Decompress))
                {                   
                    using (var memoryStream = new MemoryStream())
                    {
                        try
                        {
                            compressedStream.CopyTo(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            buffer = new byte[memoryStream.Length];
                            memoryStream.Read(buffer, 0, (int)memoryStream.Length);
                        }
                        catch (Exception e)
                        {
                            throw;
                        }
                    }
                }
            }

            File.Create(file).Close();
            File.WriteAllBytes(file, buffer);
        }

        

    }
}

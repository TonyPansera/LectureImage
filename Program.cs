using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;

namespace LectureImage
{
    class Program
    {

        static void Main(string[] args)
        {
            // http://wxfrantz.free.fr/index.php?p=format-bmp


            //byte[] myfile = File.ReadAllBytes("./Images/Test.bmp");
            ////myfile est un vecteur composé d'octets représentant les métadonnées et les données de l'image
            //char c = (char)myfile[0];
            //Console.WriteLine(c);
            ////Métadonnées du fichier
            //Console.WriteLine("\n Header \n");
            //for (int i = 0; i < 14; i++)
            //    Console.Write(myfile[i] + " ");
            ////Métadonnées de l'image
            //Console.WriteLine("\n HEADER INFO \n");
            //for (int i = 14; i < 54; i++)
            //    Console.Write(myfile[i] + " ");
            ////L'image elle-même
            //Console.WriteLine("\n IMAGE \n");
            //for (int i = 54; i < myfile.Length; i = i + 60)
            //{
            //    for (int j = i; j < i + 60; j++)
            //    {
            //        Console.Write(myfile[j] + " ");
            //    }
            //    Console.WriteLine();
            //}

            /*
            230 4 0 0 little endian

            0 0 4 230 = 230*250^0 + 4*256^1+0*256^2+0*256^3
            */

            MyImage image = new MyImage("./Images/Lac.bmp");
            //image.NoirEtBlanc();
            //image.DetectionContour();
            //image.FlouUniforme();
            //image.Repoussage();
            //image.RenforcementBords();
            //Console.WriteLine(image.toString());

            MyImage imageCachee = new MyImage("./Images/Coco.bmp");
            //MyImage image = new MyImage(1000);
            // image.Fractale();

            image.CacherImage(imageCachee.Matrice);
            image.RetrouverImage();
            byte[] myFile = image.From_Image_To_File();
            byte[] myFile2 = imageCachee.From_Image_To_File();


            File.WriteAllBytes("Sortie.bmp", myFile);
            File.WriteAllBytes("Sortie2.bmp", myFile2);
            Process.Start("Sortie.bmp");
            //Process.Start("Sortie2.bmp");
            Console.WriteLine("Press any key to exit!");
            Console.ReadLine();
        }
    }
}

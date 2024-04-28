using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace LectureImage
{
    class Program
    {

        static void Main(string[] args)
        {
            ChoisirImage();
            Console.WriteLine("Press any key to exit!");
            Console.ReadLine();
        }

        static void ChoisirImage()
        {
            Console.Clear();
            Console.WriteLine("Choisissez une image :");
            Console.WriteLine("1. COCO");
            Console.WriteLine("2. LAC");
            MyImage image;

            int choix = DemanderEntier("Entrez le numéro de l'image :", 1, 2);

            if(choix == 1)
            {
                image = new MyImage("./Images/Coco.bmp");
            } else
            {
                image = new MyImage("./Images/Lac.bmp");
            }

            MenuPrincipal(image);
        }

        static void MenuPrincipal(MyImage image)
        {
            bool quitter = false;

            while (!quitter)
            {
                Console.Clear();
                Console.WriteLine("Choisissez une catégorie :");
                Console.WriteLine("1. AFFICHER L'IMAGE");
                Console.WriteLine("2. AGRANDISSEMENT");
                Console.WriteLine("3. ROTATION");
                Console.WriteLine("4. FILTRES");
                Console.WriteLine("5. CACHER IMAGE");
                Console.WriteLine("6. FRACTALE");
                Console.WriteLine("7. HUFFMANN");
                Console.WriteLine("0. Quitter");

                int choix = DemanderEntier("Entrez le numéro de la catégorie :", 0, 7);

                switch (choix)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Affichage de l'image :");

                        byte[] myFile = image.From_Image_To_File();
                        File.WriteAllBytes("Sortie.bmp", myFile);
                        Process.Start("Sortie.bmp");

                        Console.WriteLine("Appuyez sur une touche pour revenir au menu principal.");
                        Console.ReadKey();
                        break;
                    case 2:
                        Console.Clear();
                        int facteur = DemanderEntier("Entrez le facteur d'agrandissement :");

                        image.AgrandirMatrice(facteur);
                        byte[] myFile2 = image.From_Image_To_File();
                        File.WriteAllBytes("Sortie.bmp", myFile2);
                        Process.Start("Sortie.bmp");

                        Console.WriteLine("L'image a été agrandie avec un facteur de " + facteur + ".");
                        Console.WriteLine("Appuyez sur une touche pour revenir au menu principal.");
                        Console.ReadKey();
                        break;
                    case 3:
                        Console.Clear();
                        int angle = DemanderEntier("Entrez l'angle de rotation en degrés :");

                        image.RotateImage(angle);
                        byte[] myFile3 = image.From_Image_To_File();
                        File.WriteAllBytes("Sortie.bmp", myFile3);
                        Process.Start("Sortie.bmp");

                        Console.WriteLine("L'image a été tournée de " + angle + " degrés.");
                        Console.WriteLine("Appuyez sur une touche pour revenir au menu principal.");
                        Console.ReadKey();
                        break;
                    case 4:
                        MenuFiltres(image);
                        break;
                    case 5:
                        Console.Clear();

                        MyImage imageCachee = new MyImage("./Images/Coco.bmp");
                        image.CacherImage(imageCachee.Matrice);
                        byte[] myFile5 = image.From_Image_To_File();
                        File.WriteAllBytes("Sortie.bmp", myFile5);
                        Process.Start("Sortie.bmp");

                        Console.WriteLine("Image cachée avec succès.");
                        Console.WriteLine("Appuyez sur une touche pour revenir au menu principal.");
                        Console.ReadKey();
                        break;
                    case 6:
                        Console.Clear();
                        int taille = DemanderEntier("Entrez la taille :");

                        MyImage image2 = new MyImage(taille);
                        image2.Fractale();
                        byte[] myFile4 = image2.From_Image_To_File();
                        File.WriteAllBytes("Sortie.bmp", myFile4);
                        Process.Start("Sortie.bmp");

                        Console.WriteLine("Fractale générée avec succès.");
                        Console.WriteLine("Appuyez sur une touche pour revenir au menu principal.");
                        Console.ReadKey();
                        break;
                    case 7:
                        //METTRE CODE HUFFMANN
                        break;
                    case 0:
                        quitter = true;
                        Console.WriteLine("Au revoir !");
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        break;
                }
            }
        }
        static void MenuFiltres(MyImage image)
        {
            bool retour = false;

            while (!retour)
            {
                Console.Clear();
                Console.WriteLine("Vous êtes dans la catégorie FILTRES.");
                Console.WriteLine("Choisissez un FILTRE :");
                Console.WriteLine("1. DETECTION CONTOUR");
                Console.WriteLine("2. RENFORCEMENT DES BORDS");
                Console.WriteLine("3. FLOU");
                Console.WriteLine("4. REPOUSSAGE");
                Console.WriteLine("0. Retour au menu principal");

                int choixFiltre = DemanderEntier("Entrez le numéro du filtre souhaité :");

                switch (choixFiltre)
                {
                    case 1:

                        image.DetectionContour();
                        byte[] myFile = image.From_Image_To_File();
                        File.WriteAllBytes("Sortie.bmp", myFile);
                        Process.Start("Sortie.bmp");

                        Console.Clear();
                        Console.WriteLine("Détection de contour effectuée.");
                        Console.WriteLine("Appuyez sur une touche pour continuer.");
                        Console.ReadKey();
                        break;
                    case 2:
                        image.RenforcementBords();
                        byte[] myFile2 = image.From_Image_To_File();
                        File.WriteAllBytes("Sortie.bmp", myFile2);
                        Process.Start("Sortie.bmp");

                        Console.Clear();
                        Console.WriteLine("Renforcement des bords effectué.");
                        Console.WriteLine("Appuyez sur une touche pour continuer.");
                        Console.ReadKey();
                        break;
                    case 3:
                        image.FlouUniforme();
                        byte[] myFile3 = image.From_Image_To_File();
                        File.WriteAllBytes("Sortie.bmp", myFile3);
                        Process.Start("Sortie.bmp");

                        Console.Clear();
                        Console.WriteLine("Flou uniforme appliqué.");
                        Console.WriteLine("Appuyez sur une touche pour continuer.");
                        Console.ReadKey();

                        break;
                    case 4:
                        image.Repoussage();
                        byte[] myFile4 = image.From_Image_To_File();
                        File.WriteAllBytes("Sortie.bmp", myFile4);
                        Process.Start("Sortie.bmp");

                        Console.Clear();
                        Console.WriteLine("Repoussage effectué.");
                        Console.WriteLine("Appuyez sur une touche pour continuer.");
                        Console.ReadKey();
                        break;
                    case 0:
                        retour = true;
                        Console.WriteLine("Retour au menu principal.");
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        break;
                }
            }
        }

        static int DemanderEntier(string message, int a, int b)
        {
            int entier = -1;
            while (entier < a || entier > b)
            {
                Console.Write(message + " ");
                if (int.TryParse(Console.ReadLine(), out entier))
                {
                    
                }
                else
                {
                    Console.WriteLine("Entrez un numéro valide.");
                }
            }
            return entier;
        }

        static int DemanderEntier(string message)
        {
            int entier;
            while (true)
            {
                Console.Write(message + " ");
                if (int.TryParse(Console.ReadLine(), out entier))
                {
                    return entier;
                }
                else
                {
                    Console.WriteLine("Entrez un numéro valide.");
                }
            }
        }
    }
}

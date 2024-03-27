using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LectureImage
{
    internal class MyImage
    {
        
        private int offset;

        // Entete fichier
        private string typeImage;
        private int tailleFichier;
        private const int champReserve = 0;
        private const int imageOffsetEntete = 54;

        // Entete image
        private int tailleEnteteImage;
        private int hauteur;
        private int largeur;
        private const int nombrePlanImage = 1;
        private int nombreBitParCouleur;
        private int typeCompression;
        private int tailleImage;
        private int nombreCouleursPalette;
        private int resolutionHorizontale;
        private int resolutionVerticale;
        private int nombreCouleursImportantes;

        // tableau avec toutes les valeurs
        private int[] enteteDecimal = new int[15];

        // Matrice
        private (byte, byte, byte)[,] matrice;
        public MyImage(string myFile)
        {
            byte[] file = File.ReadAllBytes(myFile);
            // Entete fichier
            typeImage = Convert.ToString(file[0]) + Convert.ToString(file[1]);

            tailleFichier = Convertir_Endian_to_Int(file, 2, 6);
            enteteDecimal[0] = tailleFichier;
            enteteDecimal[1] = champReserve;
            enteteDecimal[2] = imageOffsetEntete;

            // Octet 14
            // Entete image
            tailleEnteteImage = Convertir_Endian_to_Int(file, 14, 18);
            largeur = Convertir_Endian_to_Int(file, 18, 22);
            hauteur = Convertir_Endian_to_Int(file, 22, 26);
            // + 2 octets nombrePlanImage
            nombreBitParCouleur = Convertir_Endian_to_Int(file, 28, 30);
            typeCompression = Convertir_Endian_to_Int(file, 30, 34);
            tailleImage = tailleFichier - 54; // + 4 octets
            resolutionHorizontale = Convertir_Endian_to_Int(file, 38, 42);
            resolutionVerticale = Convertir_Endian_to_Int(file, 42, 46);
            nombreCouleursPalette = Convertir_Endian_to_Int(file, 46, 50);
            nombreCouleursImportantes = Convertir_Endian_to_Int(file, 50, 54);
            enteteDecimal[3] = tailleEnteteImage;
            enteteDecimal[4] = largeur;
            enteteDecimal[5] = hauteur;
            Console.WriteLine("largeur" + largeur);
            Console.WriteLine("hauteur" + hauteur);
            enteteDecimal[6] = nombrePlanImage;
            enteteDecimal[7] = nombreBitParCouleur;
            enteteDecimal[8] = typeCompression;
            enteteDecimal[9] = tailleImage;
            enteteDecimal[10] = resolutionHorizontale;
            enteteDecimal[11] = resolutionVerticale;
            enteteDecimal[12] = nombreCouleursPalette;
            enteteDecimal[13] = nombreCouleursImportantes;

            offset = (tailleFichier - imageOffsetEntete) % 4;
            Console.WriteLine("offset " + offset);

            // Initialisation matrice
            matrice = new (byte, byte, byte)[hauteur + offset, largeur + offset];


            int i = 54;
            for(int j = 0; j < matrice.GetLength(0); j++)
            {
                for(int k = 0; k < matrice.GetLength(1) && i+3 < tailleImage; k++)
                {
                    (byte, byte, byte) color = (file[i], file[i+1], file[i+2]);
                    i += 3;
                    matrice[j, k] = color;
                }
            }
        }

        public void DetectionContourPrewitt()
        {

        }

        static int[,] AgrandirMatrice(int nombre, int[,] matrice)
        {
            int[,] mat = null;
            if (nombre > 0)
            {
                mat = new int[matrice.GetLength(0) * nombre, matrice.GetLength(1) * nombre];
                for (int i = 0; i < nombre * matrice.GetLength(0); i++)
                {
                    for (int j = 0; j < nombre * matrice.GetLength(1); j++)
                    {
                        mat[i, j] = matrice[i / nombre, j / nombre];
                    }

                }
            }
            return mat;
        }

        //public static Bitmap Rotate(Bitmap image, float angle)
        //{
        //    Matrix matrice = new Matrix();
        //    matrice.Rotate(angle);
        //    PointF[] points = { new PointF(0, 0), new PointF(image.Width, 0), new PointF(0, image.Height), new PointF(image.Width, image.Height) };
        //    matrice.TransformPoints(points);
        //    float minX = points.Min(p => p.X);
        //    float maxX = points.Max(p => p.X);
        //    float minY = points.Min(p => p.Y);
        //    float maxY = points.Max(p => p.Y);
        //    int Largeur = (int)Math.Round(maxX - minX, MidpointRounding.AwayFromZero);
        //    int Hauteur = (int)Math.Round(maxY - minY, MidpointRounding.AwayFromZero);


        //    Bitmap image_rot = new Bitmap(Largeur, Hauteur);


        //    using (Graphics rotatedGraphics = Graphics.FromImage(image_rot))
        //    {
        //        rotatedGraphics.TranslateTransform(image_rot.Width / 2, image_rot.Height / 2);
        //        rotatedGraphics.RotateTransform(angle);
        //        rotatedGraphics.DrawImage(image, -image.Width / 2, -image.Height / 2);
        //    }

        //    return image_rot;
        //}

        public void FlouUniforme()
        {
            int[,] noyau = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            (byte, byte, byte)[,] matriceCopy = matrice;

            for (int i = 0; i < matriceCopy.GetLength(0); i++)
            {
                for(int j = 0; j < matriceCopy.GetLength(1); j++)
                {
                    (byte, byte, byte)[,] matriceExtraite = ExtraireMatrice(matriceCopy, i, j);

                    (byte, byte, byte) nouveauPixel = CalculerNouveauPixel(matriceExtraite, noyau, 9);
                    matrice[i,j] = nouveauPixel;
                }
            }
        }

        public void DetectionContour()
        {
            int[,] noyau = { { 1, 0, -1 }, { 0, 0, 0 }, { -1, 0, 1 } };
            (byte, byte, byte)[,] matriceCopy = matrice;

            for (int i = 0; i < matriceCopy.GetLength(0); i++)
            {
                for (int j = 0; j < matriceCopy.GetLength(1); j++)
                {
                    (byte, byte, byte)[,] matriceExtraite = ExtraireMatrice(matriceCopy, i, j);

                    (byte, byte, byte) nouveauPixel = CalculerNouveauPixel(matriceExtraite, noyau, 1);
                    matrice[i, j] = nouveauPixel;
                }
            }
        }

        public (byte, byte, byte) CalculerNouveauPixel((byte, byte, byte)[,] matriceExtraite, int[,] noyau, int div)
        {

            int sum1 = 0;
            for (int k = 0; k < matriceExtraite.GetLength(0); k++)
            {
                for (int l = 0; l < matriceExtraite.GetLength(1); l++)
                {
                    sum1 += matriceExtraite[k, l].Item1 * noyau[k, l];
                }
            }
            int sum2 = 0;
            for (int k = 0; k < matriceExtraite.GetLength(0); k++)
            {
                for (int l = 0; l < matriceExtraite.GetLength(1); l++)
                {
                    sum2 += matriceExtraite[k, l].Item2 * noyau[k, l];
                }
            }
            int sum3 = 0;
            for (int k = 0; k < matriceExtraite.GetLength(0); k++)
            {
                for (int l = 0; l < matriceExtraite.GetLength(1); l++)
                {
                    sum3 += matriceExtraite[k, l].Item3 * noyau[k, l];
                }
            }
            sum1 /= div;
            sum2 /= div;
            sum3 /= div;

            (byte, byte, byte) nouveauPixel = ((byte)sum1, (byte)sum2, (byte)sum3);
            return nouveauPixel;
        }

        public (byte, byte, byte)[,] ExtraireMatrice((byte, byte, byte)[,] matriceCopy, int i, int j)
        {
            (byte, byte, byte)[,] matriceExtraite = new (byte, byte, byte)[3, 3];
            int indexX = 0;
            int indexY = 0;
            for (int k = i - 1; k <= i + 1; k++)
            {
                indexX = 0;
                for (int l = j - 1; l <= j + 1; l++)
                {
                    if (k >= 0 && k < matriceCopy.GetLength(0)
                        && l >= 0 && l < matriceCopy.GetLength(1))
                    {
                        matriceExtraite[indexY, indexX] = matriceCopy[k, l];
                    }
                    indexX++;
                }
                indexY++;
            }
            return matriceExtraite;
        }

        public int Convertir_Endian_to_Int(byte[] tab, int start, int end)
        {
            int somme = 0;
            int puiss = 0;
            if (tab != null && tab.Length != 0)
            {
                for (int i = start; i < end; i++)
                {
                    somme += tab[i] * (int) Math.Pow(256, puiss);
                    puiss++;
                }
            }
            return somme;
        }

        public byte[] Convertir_Int_to_Endian_4(int number)
        {
            // source : https://maxo.blog/c-sharp-convert-int-to-byte-array/ 
            byte[] tab = new byte[4];
            if (number > 0)
            {
                tab = BitConverter.GetBytes(number);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(tab);
                }
            }
            return tab;
        }

        public byte[] Convertir_Int_to_Endian_2(int number)
        {
            // source : https://maxo.blog/c-sharp-convert-int-to-byte-array/ 
            byte[] tab = new byte[2];
            if (number > 0)
            {
                tab = BitConverter.GetBytes(number);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(tab);
                }
            }
            return tab;
        }

        public byte[] From_Image_To_File()
        {
            // Création du fichier
            int taille = (matrice.GetLength(1) * matrice.GetLength(0) * 3) + 54;
            byte[] newFile = new byte[taille];
            int k = 0;
            Console.WriteLine("taille fichier : " + taille);

            // Mettre le type de fichier dans les 2 premières cases (2 octets)
            newFile[k] = 66;
            k++;
            newFile[k] = 77;
            k++;

            // Mettre l'entete dans le fichier
            for (int i = 0; i <= 5; i++)
            {
                byte[] endian = Convertir_Int_to_Endian_4(enteteDecimal[i]);
                for(int j = endian.Length-1; j >= 0; j--)
                {
                    newFile[k] = endian[j];
                    Console.WriteLine(k + " = " + endian[j]);
                    k++;
                    
                }
            }

            byte[] endian1 = Convertir_Int_to_Endian_2(enteteDecimal[6]);
                
            newFile[k] = endian1[endian1.Length-1];
            Console.WriteLine(k + " = " + newFile[k]);
            k++;

            newFile[k] = endian1[endian1.Length - 2];
            Console.WriteLine(k + " = " + newFile[k]);
            k++;

            byte[] endian2 = Convertir_Int_to_Endian_2(enteteDecimal[7]);

            newFile[k] = endian2[endian2.Length - 1];
            Console.WriteLine(k + " = " + newFile[k]);
            k++;

            newFile[k] = endian2[endian2.Length - 2];
            Console.WriteLine(k + " = " + newFile[k]);
            k++;

            for (int i = 8; i <= 13; i++)
            {
                byte[] endian = Convertir_Int_to_Endian_4(enteteDecimal[i]);
                for (int j = endian.Length - 1; j >= 0; j--)
                {
                    newFile[k] = endian[j];
                    Console.WriteLine(k + " = " + endian[j]);
                    k++;
                    
                }
            }

            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for(int j = 0; j < matrice.GetLength(1); j++)
                {
                    newFile[k] = matrice[i,j].Item1;
                    //Console.WriteLine(k + " = " + newFile[k]);
                    k++;

                    newFile[k] = matrice[i,j].Item2;
                    //Console.WriteLine(k + " = " + newFile[k]);
                    k++;

                    newFile[k] = matrice[i,j].Item3;
                    //Console.WriteLine(k + " = " + newFile[k]);
                    k++;
                }
            }
            return newFile;
        }

        public string toString()
        {
            string result = "";
            result += "Type fichier : " + typeImage + "\n";
            result += "Taille fichier : " + tailleFichier + "\n";
            result += "Taille entête image : " + tailleEnteteImage + "\n";
            result += "Taille hauteur : " + hauteur + "\n";
            result += "Taille largeur : " + largeur + "\n";
            result += "Taille image : " + tailleImage + "\n";
            return result;
        }


    }
}

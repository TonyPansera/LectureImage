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

        /// <summary>
        /// Constructeur lors d'une extraction d'image
        /// </summary>
        /// <param name="myFile"></param>
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
            //Console.WriteLine("largeur" + largeur);
            //Console.WriteLine("hauteur" + hauteur);
            enteteDecimal[6] = nombrePlanImage;
            enteteDecimal[7] = nombreBitParCouleur;
            enteteDecimal[8] = typeCompression;
            enteteDecimal[9] = tailleImage;
            enteteDecimal[10] = resolutionHorizontale;
            enteteDecimal[11] = resolutionVerticale;
            enteteDecimal[12] = nombreCouleursPalette;
            enteteDecimal[13] = nombreCouleursImportantes;

            offset = (tailleFichier - imageOffsetEntete) % 4;
            //Console.WriteLine("offset " + offset);

            // Initialisation matrice
            matrice = new (byte, byte, byte)[hauteur + offset, largeur + offset];

            for(int j = 0; j < enteteDecimal.Length; j++)
            {
                Console.WriteLine(j + " " + enteteDecimal[j]);
            }

            int i = 54;
            for (int j = 0; j < matrice.GetLength(0); j++)
            {
                for (int k = 0; k < matrice.GetLength(1) && i + 3 < tailleImage; k++)
                {
                    (byte, byte, byte) color = (file[i], file[i + 1], file[i + 2]);
                    i += 3;
                    matrice[j, k] = color;
                }
            }
        }

        /// <summary>
        /// Constructeur lors de la création d'une fractale
        /// </summary>
        /// <param name="taille"></param>
        public MyImage(int taille)
        {
            // Entete fichier
            typeImage = "BM";

            hauteur = taille;
            largeur = taille;
            enteteDecimal[0] = taille * taille * 3 + 54;
            enteteDecimal[1] = 0;
            enteteDecimal[2] = 54;
            enteteDecimal[3] = 40;
            enteteDecimal[4] = taille;
            enteteDecimal[5] = taille;
            //Console.WriteLine("largeur" + largeur);
            //Console.WriteLine("hauteur" + hauteur);
            enteteDecimal[6] = 1;
            enteteDecimal[7] = 24;
            enteteDecimal[8] = 0;
            enteteDecimal[9] = taille * taille * 3;
            enteteDecimal[10] = 5000;
            enteteDecimal[11] = 5000;
            enteteDecimal[12] = 0;
            enteteDecimal[13] = 0;

            // Initialisation matrice
            matrice = new (byte, byte, byte)[taille, taille];

            int i = 54;
            for (int j = 0; j < matrice.GetLength(0); j++)
            {
                for (int k = 0; k < matrice.GetLength(1); k++)
                {
                    matrice[j, k] = (255,255,255);
                }
            }
        }

        public void Fractale()
        {
            int nombreIterationMax = 75;

            // Nombres complexes
            (double, double) c, z;
            double varTemp;
            int k;
            double moduleZ;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    c = ((double)j / largeur * 4 - 2, (double)i / hauteur * 4 - 2);
                    z = (0, 0);
                    k = 0;

                    do
                    {
                        varTemp = z.Item1;
                        z = (z.Item1 * z.Item1 - z.Item2 * z.Item2 + c.Item1, 2 * z.Item2 * varTemp + c.Item2);
                        moduleZ = Math.Sqrt(z.Item1 * z.Item1 + z.Item2 * z.Item2);
                        k++;
                    } while (moduleZ < 2 && k < nombreIterationMax);

                    matrice[i,j] = (Convert.ToByte(k * 255 / nombreIterationMax), 0, 0);
                }
            }
        }

        /// <summary>
        /// Tourne l'image selon un certain angle
        /// </summary>
        /// <param name="angleDegrees"></param>
        public void RotateImage(double angleDegrees)
        {
            // Convertir l'angle de degrés en radians
            double angleRadians = angleDegrees * Math.PI / 180.0;

            // Créer une nouvelle matrice pour stocker les pixels tournés
            (byte, byte, byte)[,] rotatedImageData = new (byte, byte, byte)[hauteur, largeur];

            // Calculer le centre de l'image
            double centerX = largeur / 2;
            double centerY = hauteur / 2;

            // Parcourir chaque pixel de l'image d'origine
            for (int y = 0; y < hauteur; y++)
            {
                for (int x = 0; x < largeur; x++)
                {
                    // Coordonnées polaires par rapport au centre de l'image
                    double radius = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                    double theta = Math.Atan2(y - centerY, x - centerX);

                    // Effectuer la rotation
                    double newTheta = theta + angleRadians;

                    // Nouvelles coordonnées cartésiennes
                    int newX = (int) (radius * Math.Cos(newTheta) + centerX);
                    int newY = (int) (radius * Math.Sin(newTheta) + centerY);

                    // Vérifier que les nouvelles coordonnées sont à l'intérieur de l'image
                    if (newX >= 0 && newX < largeur && newY >= 0 && newY < hauteur)
                    {
                        // Copier la couleur du pixel d'origine vers sa nouvelle position
                        rotatedImageData[newY, newX] = matrice[y, x];
                    }
                }
            }
            //for(int i = 0; i < hauteur; i++) {
            //    for(int j = 0; j < largeur; j++)
            //    {
                    
            //        for(int k = (i - 2 >= 0) ? i - 2 : (i - 1 >= 0) ? i - 1 : i; 
            //            i < hauteur || i <) { 
            //            for(int l = (j - 2 >= 0) ? j - 2 : (j - 1 >= 0) ? j - 1 : j)
            //            {

            //            }
            //        }
            //    }
            //}
            matrice = rotatedImageData;
        }

        /// <summary>
        /// Agrandit l'image selon un certain coefficient
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="matrice"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Transforme l'image en noir et blanc
        /// </summary>
        public void NoirEtBlanc()
        {
            (byte, byte, byte)[,] matriceCopy = matrice;

            for (int i = 0; i < matriceCopy.GetLength(0); i++)
            {
                for (int j = 0; j < matriceCopy.GetLength(1); j++)
                {
                    byte moyenne = (byte) (matriceCopy[i, j].Item1 * 0.21 
                        + matriceCopy[i, j].Item2 * 0.72
                        + matriceCopy[i, j].Item3 * 0.07);
                    (byte, byte, byte) nouveauPixel = (moyenne, moyenne, moyenne);
                    matrice[i, j] = nouveauPixel;
                }
            }
        }

        public void Repoussage()
        {
            int[,] noyau = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
            (byte, byte, byte)[,] matriceCopy = new (byte, byte, byte)[matrice.GetLength(0), matrice.GetLength(1)];

            for (int i = 0; i < matriceCopy.GetLength(0); i++)
            {
                for (int j = 0; j < matriceCopy.GetLength(1); j++)
                {
                    (byte, byte, byte)[,] matriceExtraite = ExtraireMatrice(matrice, i, j);

                    (byte, byte, byte) nouveauPixel = CalculerNouveauPixel(matriceExtraite, noyau, 1);
                    matriceCopy[i, j] = nouveauPixel;
                }
            }
            matrice = matriceCopy;
        }

        public void RenforcementBords()
        {
            int[,] noyau = { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
            (byte, byte, byte)[,] matriceCopy = new (byte, byte, byte)[matrice.GetLength(0), matrice.GetLength(1)];

            for (int i = 0; i < matriceCopy.GetLength(0); i++)
            {
                for (int j = 0; j < matriceCopy.GetLength(1); j++)
                {
                    (byte, byte, byte)[,] matriceExtraite = ExtraireMatrice(matrice, i, j);

                    (byte, byte, byte) nouveauPixel = CalculerNouveauPixel(matriceExtraite, noyau, 9);
                    matriceCopy[i, j] = nouveauPixel;
                }
            }
            matrice = matriceCopy;
        }


        public void DetectionContour()
        {
            int[,] noyau = { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
            (byte, byte, byte)[,] matriceCopy = new (byte, byte, byte)[matrice.GetLength(0), matrice.GetLength(1)];

            for (int i = 0; i < matriceCopy.GetLength(0); i++)
            {
                for (int j = 0; j < matriceCopy.GetLength(1); j++)
                {
                    (byte, byte, byte)[,] matriceExtraite = ExtraireMatrice(matrice, i, j);

                    (byte, byte, byte) nouveauPixel = CalculerNouveauPixel(matriceExtraite, noyau, 46);
                    matriceCopy[i, j] = nouveauPixel;
                }
            }
            matrice = matriceCopy;
        }

        public void FlouUniforme()
        {
            int[,] noyau = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            (byte, byte, byte)[,] matriceCopy = new (byte, byte, byte)[matrice.GetLength(0), matrice.GetLength(1)];

            for (int i = 0; i < matriceCopy.GetLength(0); i++)
            {
                for (int j = 0; j < matriceCopy.GetLength(1); j++)
                {
                    (byte, byte, byte)[,] matriceExtraite = ExtraireMatrice(matrice, i, j);

                    (byte, byte, byte) nouveauPixel = CalculerNouveauPixel(matriceExtraite, noyau, 9);
                    matriceCopy[i, j] = nouveauPixel;
                }
            }
            matrice = matriceCopy;
        }

        public (byte, byte, byte) CalculerNouveauPixel((byte, byte, byte)[,] matriceExtraite, int[,] noyau, double div)
        {
            double sum1 = 0;
            double sum2 = 0;
            double sum3 = 0;
            for (int k = 0; k < matriceExtraite.GetLength(0); k++)
            {
                for (int l = 0; l < matriceExtraite.GetLength(1); l++)
                {
                    sum1 += matriceExtraite[k, l].Item1 * noyau[k, l];
                    sum2 += matriceExtraite[k, l].Item2 * noyau[k, l];
                    sum3 += matriceExtraite[k, l].Item3 * noyau[k, l];
                }
            }
            
            sum1 /= div;
            sum2 /= div;
            sum3 /= div;

            sum1 = (sum1 > 255) ? 255 : sum1;
            sum2 = (sum2 > 255) ? 255 : sum2;
            sum3 = (sum3 > 255) ? 255 : sum3;

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
                indexY = 0;
                for (int l = j - 1; l <= j + 1; l++)
                {
                    if (k >= 0 && k < matriceCopy.GetLength(0)
                        && l >= 0 && l < matriceCopy.GetLength(1))
                    {
                        matriceExtraite[indexX, indexY] = matrice[k, l];
                    }
                    else
                    {
                        matriceExtraite[indexX, indexY] = (0, 0, 0);
                    }
                    indexY++;
                }
                indexX++;
            }
            return matriceExtraite;
        }

        public (byte, byte, byte)[,] ExtraireMatrice5x5((byte, byte, byte)[,] matriceCopy, int i, int j)
        {
            (byte, byte, byte)[,] matriceExtraite = new (byte, byte, byte)[5, 5];
            int indexX = 0;
            int indexY = 0;
            for (int k = i - 2; k <= i + 2; k++)
            {
                indexX = 0;
                for (int l = j - 2; l <= j + 2; l++)
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
            //Console.WriteLine("taille fichier : " + taille);

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
                    //Console.WriteLine(k + " = " + endian[j]);
                    k++;
                    
                }
            }

            byte[] endian1 = Convertir_Int_to_Endian_2(enteteDecimal[6]);
                
            newFile[k] = endian1[endian1.Length-1];
            //Console.WriteLine(k + " = " + newFile[k]);
            k++;

            newFile[k] = endian1[endian1.Length - 2];
            //Console.WriteLine(k + " = " + newFile[k]);
            k++;

            byte[] endian2 = Convertir_Int_to_Endian_2(enteteDecimal[7]);

            newFile[k] = endian2[endian2.Length - 1];
            //Console.WriteLine(k + " = " + newFile[k]);
            k++;

            newFile[k] = endian2[endian2.Length - 2];
            //Console.WriteLine(k + " = " + newFile[k]);
            k++;

            for (int i = 8; i <= 13; i++)
            {
                byte[] endian = Convertir_Int_to_Endian_4(enteteDecimal[i]);
                for (int j = endian.Length - 1; j >= 0; j--)
                {
                    newFile[k] = endian[j];
                    //Console.WriteLine(k + " = " + endian[j]);
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

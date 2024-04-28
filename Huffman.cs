using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectureImage
{
    internal class Huffman
    {
        Noeud root;
        Dictionary<(byte, byte, byte), int> frequencePixels;
        
        /// <summary>
        /// Constructeur de la classe Huffman qui va créer l'arbre et initialiser le root
        /// </summary>
        /// <param name="matrice"></param>
        public Huffman((byte, byte, byte)[,] matrice)
        {
            for(int i = 0; i < matrice.GetLength(0); i++)
            {
                for(int j = 0; j < matrice.GetLength(1); j++)
                {
                    if (frequencePixels.ContainsKey(matrice[i, j]))
                    {
                        // Itère la valeur si le pixel et déjà présent
                        int frequence = frequencePixels[matrice[i, j]];
                        frequencePixels[matrice[i, j]] = frequence + 1;
                    } else
                    {
                        // Ajoute un pixel qui n'est pas encore dans la liste
                        frequencePixels.Add(matrice[i, j], 1);
                    }
                }
            }

            // Création de l'arbre :

            // On commence par créer tous les noeuds qu'on stocke dans une liste
            List<Noeud> alNoeud = new List<Noeud>();

            foreach ((byte, byte, byte) pixel in frequencePixels.Keys)
            {
                Noeud noeud = new Noeud(pixel, frequencePixels[pixel]);
                alNoeud.Add(noeud);
            }

            // On créé l'arbre
            while(alNoeud.Count > 1)
            {
                int min1 = 0;
                int min2 = 1;
                if (alNoeud[0].Frequence > alNoeud[1].Frequence)
                {
                    min1 = 1;
                    min2 = 0;
                }

                // On récupère les index des 2 noeuds avec les plus basses fréquences
                for (int i = 0; i < alNoeud.Count; i++)
                {
                    if (alNoeud[i].Frequence < alNoeud[min1].Frequence)
                    {
                        min1 = i;
                    } else if (alNoeud[i].Frequence < alNoeud[min2].Frequence)
                    {
                        min2 = i;
                    }
                }
                // On créé le nouveau noeud avec les fréquences additionnées
                Noeud newNoeud = new Noeud(alNoeud[min1].Frequence + alNoeud[min2].Frequence);
                newNoeud.Droit = alNoeud[min1];
                newNoeud.Gauche = alNoeud[min2];

                // On ajoute le nouveau noeud à la liste
                alNoeud.Add(newNoeud);
                alNoeud.RemoveAt(min1);
                alNoeud.RemoveAt(min2);
            }

            // Lorsqu'il n'y a plus qu'une entrée dans la liste c'est celle avec la plus grande fréquence (root)
            root = alNoeud[0];
        }


    }
}

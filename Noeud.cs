using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectureImage
{
    internal class Noeud
    {
        private (byte, byte, byte) pixel;
        private int frequence;
        private Noeud gauche;
        private Noeud droit;

        public Noeud((byte, byte, byte) pPixel, int pFrequence)
        {
            this.pixel = pPixel;
            this.frequence = pFrequence;
        }

        public Noeud(int pFrequence)
        {
            this.frequence = pFrequence;
        }

        public (byte, byte, byte) Pixel
        {
            get { return this.pixel; }
        }

        public Noeud Gauche
        {
            get { return this.gauche; }
            set { this.gauche = value; }
        }

        public Noeud Droit
        {
            get { return this.droit; }
            set { this.droit = value; }
        }

        public int Frequence
        {
            get { return this.frequence; }
            set { this.frequence = value; }
        }
    }
}

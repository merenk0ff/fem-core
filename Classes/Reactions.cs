using System;

namespace FEM
{
    [Serializable]
    public class Reactions
    {
        public double N { get; set; }
        public double Qy { get; set; }
        public double Qz { get; set; }
    }
}

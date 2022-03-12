using System;

namespace FEM
{
    [Serializable]
    public class InternalStressElement
    {
        public double StartN { get; set; }
        public double StartQy { get; set; }
        public double StartQz { get; set; }
        public double StartMx { get; set; }
        public double StartMy { get; set; }
        public double StartMz { get; set; }
        public double EndN { get; set; }
        public double EndQy { get; set; }
        public double EndQz { get; set; }
        public double EndMx { get; set; }
        public double EndMy { get; set; }
        public double EndMz { get; set; }

        public InternalStressElement()
        {
            StartN = 0;
            StartQy = 0;
            StartQz = 0;
            StartMx = 0;
            StartMy = 0;
            StartMz = 0;
            EndN = 0;
            EndQy = 0;
            EndQz = 0;
            EndMx = 0;
            EndMy = 0;
            EndMz = 0;
        }
    }
}

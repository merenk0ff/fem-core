using System;

namespace FEM
{
    /// <summary>
    /// Перемещение для узла
    /// </summary>
    [Serializable]
    public class Moving
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Ux { get; set; }
        public double Uy { get; set; }
        public double Uz { get; set; }
    }
}

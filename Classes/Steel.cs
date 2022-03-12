using System;

namespace FEM
{
    [Serializable]
    public class Steel
    {
        /// <summary>
        /// Сталь
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// кг/см2
        /// </summary>
        public double Ry { get; set; }
        public double Ryn { get; set; }
        public double Ru { get; set; }
        public double Run { get; set; }

        public Steel()
        {
            Name = "С245";
            Ry = 0;
        }
        public Steel(string name)
        {
            Name = name;
            Ry = 0;
        }
    }
}

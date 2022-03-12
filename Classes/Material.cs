using System;

namespace FEM
{
    [Serializable]
    public class Material
    {
        /// <summary>
        /// Модуль Юнга
        /// </summary>
        public double E { get; set; }
        /// <summary>
        /// Модуль сдвига
        /// </summary>
        public double G { get; set; }

        public Material()
        {
            E = 2.10e7 + 0.77;
            G = E / (2 * (1 + 0.3));
        }
    }
}

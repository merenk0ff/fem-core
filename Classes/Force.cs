using System;

namespace FEM
{
    /// <summary>
    /// Сила
    /// </summary>
    [Serializable]
    public class Force : IDisposable
    {
        /// <summary>
        /// Название нагрузки
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Сила вдоль оси Х
        /// </summary>
        public double Xforce { get; set; }
        /// <summary>
        /// Сила вдоль оси Y
        /// </summary>
        public double Yforce { get; set; }
        /// <summary>
        /// Сила вдоль оси Z
        /// </summary>
        public double Zforce { get; set; }
        /// <summary>
        /// Момент вокруг оси X
        /// </summary>
        public double Xmoment { get; set; }
        /// <summary>
        /// Момент вокруг оси Y
        /// </summary>
        public double Ymoment { get; set; }
        /// <summary>
        /// Момент вокруг оси Z
        /// </summary>
        public double Zmoment { get; set; }

        public void Dispose()
        {
            Zmoment = 0;
            Ymoment = 0;
            Xmoment = 0;
            Zforce = 0;
            Yforce = 0;
            Xforce = 0;
            Name = null;
        }
    }
}

using System;
using System.Collections.Generic;
using MathNet.Spatial.Euclidean;

namespace FEM
{
    [Serializable]
    public class Node : IDisposable
    {
        public Point3D Point { get; set; }
        public int Number { get; set; }
        /// <summary>
        /// Закрепление узла
        /// </summary>
        public List<Confinements> Confinement { get; set; }
        /// <summary>
        /// Группы нагрузок
        /// </summary>
        public List<ForceGroup> ForceGroups { get; set; }
        public List<Moving> MovingList { get; set; }
        /// <summary>
        /// Опорные реакции в случае если фундамент узел является опорным
        /// </summary>
        public List<Reactions> RectionList { get; set; }
        
        /// <summary>
        /// Усилие для которого происходит расчёт
        /// </summary>
        public double RcompForCalculationFoundation { get; set; }
        public double RQcompForCalculationFoundation { get; set; }
        /// <summary>
        /// Усилие для которого происходит расчёт
        /// </summary>
        public double RstretchForCalculationFoundation { get; set; }
        public double RQstretchForCalculationFoundation { get; set; }
        public bool IsLowerNode { get; set; }

        public Node()
        {
            Point = new Point3D();
            Number = 0;
            Confinement = new List<Confinements>();
            MovingList = new List<Moving>();
            RectionList = new List<Reactions>();
            IsLowerNode = false;
            ForceGroups = new List<ForceGroup>();
        }

        public Node(double x, double y, double z)
        {
            Point = new Point3D(x, y, z);
            Confinement = new List<Confinements>() { Confinements.Relize };
            MovingList = new List<Moving>();
            RectionList = new List<Reactions>();
            IsLowerNode = false;
            ForceGroups = new List<ForceGroup>();
        }
        public Node(double x, double y, double z, List<Confinements> confinements)
        {
            Point = new Point3D(x, y, z);
            Confinement = confinements;
            MovingList = new List<Moving>();
            RectionList = new List<Reactions>();
            IsLowerNode = false;
            ForceGroups = new List<ForceGroup>();
        }
        public Node(Point3D point, int number)
        {
            Point = point;
            Number = number;
            Confinement = new List<Confinements>() { Confinements.Relize };
            MovingList = new List<Moving>();
            RectionList = new List<Reactions>();
            IsLowerNode = false;
            ForceGroups = new List<ForceGroup>();
        }
        public Node(Point3D point)
        {
            Point = new Point3D(Math.Round(point.X, 4), Math.Round(point.Y, 4), Math.Round(point.Z, 4));
            Number = 0;
            Confinement = new List<Confinements>() { Confinements.Relize };
            MovingList = new List<Moving>();
            RectionList = new List<Reactions>();
            IsLowerNode = false;
            ForceGroups = new List<ForceGroup>();
        }

        public void Dispose()
        {
            MovingList.Clear();
            RectionList.Clear();
            Confinement.Clear();
        }
    }
}

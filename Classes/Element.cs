using System;
using System.Collections.Generic;
using MathNet.Spatial.Euclidean;

namespace FEM
{
    [Serializable]
    public class Element : IDisposable
    {
        public Profile Profile { get; set; }
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }
        public Line3D Line { get; set; }
        public int Number { get; set; }

        
        public GeometricCharacteristicsElement Characteristics { get; set; }
        /// <summary>
        /// Усилия в элементе
        /// </summary>
        public InternalStressElement InternalStress { get; set; }
        public List<InternalStressElement> InternalStressList { get; set; }
      
        /// <summary>
        /// Угол поворота сечения
        /// </summary>
        public int Angle { get; set; }

        public Element()
        {
            StartNode = new Node();
            EndNode = new Node();
            Line = new Line3D();
            Number = -1;
            Profile = new Profile();
            InternalStress = new InternalStressElement();
            Characteristics = new GeometricCharacteristicsElement();
            InternalStressList = new List<InternalStressElement>();
            Angle = 0;
        }

        public Element(Point3D StartPoint, Point3D EndPoint)
        {
            Number = -1;
            Line = new Line3D(
                new Point3D(Math.Round(StartPoint.X, 4), Math.Round(StartPoint.Y, 4), Math.Round(StartPoint.Z, 4)),
                new Point3D(Math.Round(EndPoint.X, 4), Math.Round(EndPoint.Y, 4), Math.Round(EndPoint.Z, 4)));
            StartNode = new Node(StartPoint);
            EndNode = new Node(EndPoint);
            Profile = new Profile();
            InternalStress = new InternalStressElement();
            Characteristics = new GeometricCharacteristicsElement();
            InternalStressList = new List<InternalStressElement>();
            Angle = 0;
        }
        public Element(Node startNode, Node endNode)
        {
            Number = -1;
            Line = new Line3D(
                new Point3D(Math.Round(startNode.Point.X, 4), Math.Round(startNode.Point.Y, 4), Math.Round(startNode.Point.Z, 4)),
                new Point3D(Math.Round(endNode.Point.X, 4), Math.Round(endNode.Point.Y, 4), Math.Round(endNode.Point.Z, 4)));
            StartNode = startNode;
            EndNode = endNode;
            Profile = new Profile();
            InternalStress = new InternalStressElement();
            Characteristics = new GeometricCharacteristicsElement();
            InternalStressList = new List<InternalStressElement>();
            Angle = 0;
        }
      

        public void Dispose()
        {
            InternalStressList.Clear();

        }
    }
}

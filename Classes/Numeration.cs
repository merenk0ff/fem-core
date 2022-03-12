using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Spatial.Euclidean;

namespace FEM
{
    class Numeration
    {
        /// <summary>
        /// Получение всех узлов башни c удалением дубликатов. Удаление дубликатов по расстоянию <0.001метра
        /// </summary>
        /// <param name="tower"></param>
        /// <returns></returns>
        public static List<Node> GetAllNodes(Construction construction)
        {
            //var myStopwatch = new System.Diagnostics.Stopwatch();
            //myStopwatch.Start();
            var list = new List<Node>();
            foreach (var element in construction.Elements)
            {
                list.Add(element.StartNode);
                list.Add(element.EndNode);
            }

            list = list.Distinct(new NodeComparer()).ToList();
            list = CheckNodesDist(list);
            //Сортировка узлов
            var sortedList = new List<Node>();
            var minPoint = new Point3D(double.MaxValue, double.MaxValue, double.MaxValue);
            var minNode = new Node();
            foreach (var node in list)
            {
                if (node.Point.X < minPoint.X && node.Point.Y < minPoint.Y && node.Point.Z < minPoint.Z)
                {
                    minPoint = node.Point;
                    minNode = node;
                }
            }
            sortedList.Add(minNode);
            for (var i = 0; i < list.Count; i++)
            {
                var dist = double.MaxValue;
                var minNodeLocal = new Node(-999999, -999999, -999999);
                for (var j = i + 1; j <= list.Count; j++)
                {
                    if (!sortedList.Contains(list[i]))
                    {
                        var curDist = sortedList.LastOrDefault().Point.DistanceTo(list[i].Point);
                        if (curDist < dist)
                        {
                            dist = curDist;
                            minNodeLocal = list[i];
                        }
                    }
                }
                if (minNodeLocal.Point != new Point3D(-999999, -999999, -999999))
                    sortedList.Add(minNodeLocal);
            }
            for (var i = 0; i < sortedList.Count; i++)
            {
                sortedList[i].Number = i;
            }

            list.Clear();
            list.AddRange(sortedList);


            //Это надо выполнить после нумерации узлов.
            foreach (var element in construction.Elements)
            {
                if (!list.Contains(element.StartNode))
                {
                    foreach (var node in list)
                    {
                        var dist = element.StartNode.Point.DistanceTo(node.Point);
                        if (dist < 0.001 && dist > 0)
                        {
                            element.StartNode.Point = node.Point;
                            element.StartNode.Number = node.Number;
                        }
                        else if (dist == 0)
                        {
                            element.StartNode.Number = node.Number;
                        }

                    }
                }

                if (!list.Contains(element.EndNode))
                {
                    foreach (var node in list)
                    {
                        var dist = element.EndNode.Point.DistanceTo(node.Point);
                        if (dist < 0.001 && dist > 0)
                        {
                            element.EndNode.Point = node.Point;
                            element.EndNode.Number = node.Number;
                        }
                        else if (dist == 0)
                        {
                            element.EndNode.Number = node.Number;
                        }
                    }
                }
            }
            //myStopwatch.Stop();
            //var t2 = myStopwatch.Elapsed;

            return list;
        }

        /// <summary>
        /// Сравниватель для удаления дублей узлов
        /// </summary>
        private class NodeComparer : IEqualityComparer<Node>
        {
            public bool Equals(Node x, Node y)
            {
                if (Object.ReferenceEquals(x.Point.X, y.Point)) return true;
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;
                return x.Point == y.Point;
            }
            public int GetHashCode(Node node)
            {
                return (0.4).GetHashCode();
            }
        }

        /// <summary>
        /// проверка расстояния м/у узлами и удаление близких
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Node> CheckNodesDist(List<Node> list)
        {
            var list2 = new List<int>();
            for (var i = 0; i < list.Count; i++)
            {
                for (var j = i + 1; j < list.Count; j++)
                {
                    var dist = list[i].Point.DistanceTo(list[j].Point);
                    if (dist <= 0.001)
                        list2.Add(j);
                }
            }
            list2.Sort();
            list2 = list2.AsEnumerable().Reverse().ToList();
            var listForOutput = new List<Node>();
            for (var i = 0; i < list.Count; i++)
            {
                if (!list2.Contains(i))
                    listForOutput.Add(list[i]);

            }

            return listForOutput;
        }

        public static void PrepareToFem(Construction construction)
        {
            //if (tower.Nodes == null)
            //    tower.Nodes = new List<Node>();
            //tower.Nodes = GetAllNodes(tower);
            DeleteCloneElementFromTower(construction);

        }
        /// <summary>
        /// Удаление дублей элементов
        /// </summary>
        /// <param name="tower"></param>
        public static void DeleteCloneElementFromTower(Construction construction)
        {
            var distinctList = GetElementDublicatList(construction.Elements);
            construction.Elements = distinctList;
        }
        /// <summary>
        /// Список элементов без дубликатов
        /// </summary>
        /// <param name="elementList"></param>
        /// <returns></returns>
        public static List<Element> GetElementDublicatList(List<Element> elementList)
        {
            var distinctList = new List<Element>();
            for (var i = 0; i < elementList.Count; i++)
            {
                for (var j = i; j < elementList.Count; j++)
                {
                    if ((elementList[i].Line.StartPoint != elementList[j].Line.StartPoint &&
                         elementList[i].Line.EndPoint != elementList[j].Line.EndPoint) ||
                        (elementList[i].Line.StartPoint != elementList[j].Line.EndPoint &&
                         elementList[i].Line.EndPoint != elementList[j].Line.StartPoint))
                    {
                        distinctList.Add(elementList[i]);
                        break;
                    }
                }
            }
            return distinctList;
        }


    }


}

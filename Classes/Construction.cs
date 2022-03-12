using System;
using System.Collections.Generic;

namespace FEM
{
    [Serializable]
    public class Construction : IDisposable
    {
        /// <summary>
        /// Список всех элементов конструкции
        /// </summary>
        public List<Element> Elements { get; set; }
        /// <summary>
        /// Все узлы конструкции
        /// </summary>
        public List<Node> Nodes { get; set; }
        /// <summary>
        /// Узлы в которые допускается приложение нагрузки
        /// </summary>
        public List<Node> LoadNodes { get; set; }
        public List<ForceGroup> ForceGroups { get; set; }
        public List<Profile> Sortament { get; set; }

        /// <summary>
        /// Опорные узлы конструкции
        /// </summary>
        public List<int> LowerNodesNumber { get; set; }
       
        public TimeSpan Time { get; set; }

        public bool IsNeedCorrectSortament { get; set; }

        public Construction()
        {
            Elements = new List<Element>();
            Nodes = new List<Node>();
            LoadNodes = new List<Node>();
            Sortament = new List<Profile>();
            LowerNodesNumber = new List<int>();
            Time = new TimeSpan();
        }

        public void Prepare()
        {
            Nodes = Numeration.GetAllNodes(this);
            Numeration.PrepareToFem(this);
        }
        public void Calculate()
        {
            Core.Calculate(this,true);
        }

        public void Dispose()
        {

            foreach (var element in Elements)
            {
                element.Dispose();
            }
            Elements.Clear();
            foreach (var node in Nodes)
            {
                node.Dispose();
            }
            Nodes.Clear();
            LoadNodes.Clear();
            LowerNodesNumber.Clear();

        }
    }
}

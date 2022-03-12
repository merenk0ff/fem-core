using System;
using System.Collections.Generic;

namespace FEM
{
    [Serializable]
    public class ForceGroup : IDisposable
    {
        public string Name { get; set; }

        public List<Force> Forces = new List<Force>();

        public void Dispose()
        {
            Forces.Clear();
        }
    }
}

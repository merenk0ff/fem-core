using System;
using System.Collections.Generic;
using FEM;
using MathNet.Spatial.Euclidean;

namespace TestConsole
{
    class Example1
    {
        static void Main(string[] args)
        {
            var construction = new Construction();
            construction.Nodes = new List<Node>()
            {
                new Node(new Point3D(0,0,0)),
                new Node(new Point3D(1,0,0)),
                new Node(new Point3D(0,1,0)),
                new Node(new Point3D(1,1,0)),
                new Node(new Point3D(0.5,0.5,1)),
                new Node(new Point3D(1,0.5,1))
            };
            construction.Elements = new List<Element>()
            {
                new Element(construction.Nodes[0], construction.Nodes[4]),
                new Element(construction.Nodes[1], construction.Nodes[4]),
                new Element(construction.Nodes[2], construction.Nodes[4]),
                new Element(construction.Nodes[3], construction.Nodes[4]),
                new Element(construction.Nodes[4], construction.Nodes[5])
            };

            for (var i = 0; i < construction.Elements.Count; i++) //TODO номера элементов обязательны...нужно упростить нумерацию
            {
                construction.Elements[i].Number = i;
            }
            for (var i = 0; i < construction.Nodes.Count; i++)//TODO номера элементов обязательны...нужно упростить нумерацию
            {
                construction.Nodes[i].Number = i;
            }
            //Задание закреплений
            for (var i = 0; i < 4; i++)
            {
                construction.Nodes[i].Confinement = new List<Confinements>()
                {
                    Confinements.FixedDx,
                    Confinements.FixedDy,
                    Confinements.FixedDz
                };
            }
            construction.Nodes[5].Confinement = new List<Confinements>()
                {
                    Confinements.FixedDx,
                    Confinements.FixedDy,
                    Confinements.FixedDz
                };
            //Задание групп в основном объекте
            construction.ForceGroups = new List<ForceGroup>//по факту не очень нужное поле... но число групп нагрузок в узлах и в самой конструкции должно совпадать
            {
                new ForceGroup()
                {
                    Forces = new List<Force>(),
                    Name = "Группа нагрузок первая"
                }
            };

            //Задание этого же количества групп в каждом узле с заданием сил по потребности
            for (var i = 0; i < 4; i++)
            {
                construction.Nodes[i].ForceGroups = new List<ForceGroup>()
                {
                    new ForceGroup()
                    {
                        Name = "Группа нагрузок первая",
                        Forces = new List<Force>()
                    }
                };
            }
            construction.Nodes[5].ForceGroups = new List<ForceGroup>()
                {
                    new ForceGroup()
                    {
                        Name = "Группа нагрузок первая",
                        Forces = new List<Force>()
                    }
                };
            construction.Nodes[4].ForceGroups = new List<ForceGroup>()
            {
                new ForceGroup()
                {
                    Name = "Группа нагрузок первая",
                    Forces = new List<Force>()
                    {
                        new Force()
                        {
                            Name = "Force",
                            Zforce = 1
                        }
                    }
                }
            };

            construction.Calculate();
            for (var i = 0; i < construction.ForceGroups.Count; i++)
            {
                foreach (var element in construction.Elements)
                {
                    Console.WriteLine(element.ToString(i));
                }
            }
        }
      
    }
}

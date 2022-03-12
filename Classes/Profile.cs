namespace FEM
{
    using System;

  
        [Serializable]
        public class Profile
        {
            /// <summary>
            /// Название профиля
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// ПЛощадь поперечного сечения профиля
            /// </summary>
            public double Area { get; set; }
            /// <summary>
            /// Момент инерции при свободном кручении
            /// </summary>
            public double It { get; set; }
            /// <summary>
            /// Момент инерции  вдоль оси Y
            /// </summary>
            public double Iy { get; set; }
            /// <summary>
            /// Момент инерции  вдоль оси Z
            /// </summary>
            public double Iz { get; set; }
            //Не используется
            public double Iv { get; set; }
            /// <summary>
            /// Масса погонного метра
            /// </summary>
            public double P { get; set; }
            public double ix { get; set; }
            public double iv { get; set; }
            /// <summary>
            /// Внутренний радиус скругления
            /// </summary>
            public double InnrerR { get; set; }
            public double CoverArea { get; set; }
            /// <summary>
            /// Толщина кголка
            /// </summary>
            public double T { get; set; }

            /// <summary>
            /// Тип сечения
            /// </summary>
            public string ScadType { get; set; }
            /// <summary>
            /// Номер сечения
            /// </summary>
            public int ScadNumber { get; set; }
            public Steel Steel { get; set; }

            public double Height { get; set; }
            public double Thinckness { get; set; }
            public int Risk { get; set; }
            public int MaxD { get; set; }

            public Profile()
            {
                //Name = "102X4";
                //Height = 102;
                //T = 4;
                //Area = 12.315 * Math.Pow(0.1, 4);
                //It = 296.177 * Math.Pow(0.1, 8);
                //Iy = 148.088 * Math.Pow(0.1, 8);
                //Iz = 148.088 * Math.Pow(0.1, 8);
                //Iv = 148.088 * Math.Pow(0.1, 8);
                //ScadNumber = 407;





                //Name = "L50x5";
                //Height = 50;
                //T = 5;
                //Thinckness = 5;
                //Area = 4.80000017 * Math.Pow(0.1, 4);
                //It = 0.370895816 * Math.Pow(0.1, 8);
                //Iy = 17.688430778 * Math.Pow(0.1, 8);
                //Iz = 4.69881886 * Math.Pow(0.1, 8);
                //Iv = 4.69881886 * Math.Pow(0.1, 8);
                //ix = 1.53 * Math.Pow(0.1, 2);
                //iv = 0.98 * Math.Pow(0.1, 2);
                //P = 3.77;
                //ScadType = "ce_equal";
                //ScadNumber = 25;




                //Name = "L180x11";
                //Area = 38.8 * Math.Pow(0.1, 4);
                //It = 14.887834348 * Math.Pow(0.1, 8);
                //Iy = 1933.100097656* Math.Pow(0.1, 8);
                //Iz = 1933.100097656 * Math.Pow(0.1, 8);
                //Iv = 499.779968262 * Math.Pow(0.1, 8);
                //P = 30.469999313;
                //CoverArea = 0;
                //ScadType = "ce_equal";
                ////ScadNumber = 37;
                //ScadNumber = 98;
                //Height = 180;
                //T = 11;
                //Steel = new Steel();

                Name = "L100x7";
                Area = 13.75 * Math.Pow(0.1, 4);
                It = 2.109 * Math.Pow(0.1, 8);
                Iy = 207.009994507 * Math.Pow(0.1, 8);
                Iz = 54.159999847 * Math.Pow(0.1, 8);
                Iv = 54.159999847 * Math.Pow(0.1, 8);
                P = 10.79;
                CoverArea = 0;
                ScadType = "ce_equal";
                //ScadNumber = 37;
                ScadNumber = 65;
                Height = 110;
                T = 7;
                Steel = new Steel();
            }
            public Profile(string name, double area, double ix, double iy, double Ix, double Iy, double Iz, double Iv, double p, double coverArea, string scadType, int scadNumber, string steel)
            {
                Name = name;
                Area = area * 0.0001;
                Ix = Ix * 0.00000001;
                Iy = Iy * 0.00000001;
                Iz = Iz * 0.00000001;
                Iv = Iv * 0.00000001;
                ix = ix * 0.01;
                iy = iy * 0.01;
                P = p;
                CoverArea = coverArea;
                ScadType = scadType;
                ScadNumber = scadNumber;
                Steel = new Steel(steel);
            }
            public Profile(string name, double area, double innerR, double _ix, double _iy, double _Ix, double _Iy, double _Iz, double _Iv, double p, double coverArea, int scadNumber, string steel)
            {
                Name = name;
                Area = area * 0.0001;
                It = _Ix * 0.00000001;
                Iy = _Iy * 0.00000001;
                Iz = _Iz * 0.00000001;
                Iv = _Iv * 0.00000001;
                ix = _ix * 0.01;
                iv = _iy * 0.01;
                P = p;
                CoverArea = coverArea;
                ScadNumber = scadNumber;
                Steel = new Steel(steel);
                InnrerR = innerR;
            }
            public Profile(string name, double height, double thinckness, double area, double innerR, double _ix, double _iy, double _Ix, double _Iy, double _Iz, double _Iv, double p, double coverArea, int scadNumber, string steel, int risk, int maxD)
            {
                Name = name;
                Height = height;
                Thinckness = thinckness;
                Area = area * 0.0001;
                InnrerR = innerR;
                It = _Ix * 0.00000001;
                Iy = _Iy * 0.00000001;
                Iz = _Iz * 0.00000001;
                Iv = _Iv * 0.00000001;
                ix = _ix * 0.01;
                iv = _iy * 0.01;
                P = p;
                CoverArea = coverArea;
                ScadNumber = scadNumber;
                Steel = new Steel(steel);
                Risk = risk;
                MaxD = maxD;

            }
        }
    }
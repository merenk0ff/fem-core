using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NLog;

namespace FEM
{
    class Core
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();
        public static void Calculate(Construction construction, bool cpuFlag = true)
        {

            _logger.Info("Подготовка МКЭ для  расчёта");
         
            //Очистка
            foreach (var element in construction.Elements)
            {
                element.InternalStressList.Clear();
            }
            foreach (var node in construction.Nodes)
            {
                node.RectionList.Clear();
                node.MovingList.Clear();
            }

            const double E = 2.10e7 + 0.77; //Модуль Юнга
            const double G = E / (2 * (1 + 0.3)); //Модуль сдвига

            if (construction.Nodes.Count < 2)
                return;
            var elementList = construction.Elements;
            var nodeList = construction.Nodes;
            //_logger.Info("Коллекция матриц жесткости КЭ в локальной системе координат");
            var kLocalList3D = GetStiffnessMatrix3D(elementList, E, G, construction);
            //_logger.Info("Получение коллекции матриц ортогональных преобразований КЭ");
            var tList3D = GetTransformatinMatrix3D(elementList);
            //_logger.Info("Квазидиагональная матрица ортогональных преобразований координат");
            var kvaziTmatrix = GetSpareKvaziMatrix3D(tList3D);
            //_logger.Info("Коллекция матриц жесткости КЭ в глобальной системе координат");
            var kGlobalList3D = new List<Matrix<double>>();


            for (var i = 0; i < elementList.Count; i++)
            {
                kGlobalList3D.Add(tList3D[i].Transpose().Multiply(kLocalList3D[i]).Multiply(tList3D[i]));
                elementList[i].StartNode.RectionList.Clear();
                elementList[i].EndNode.RectionList.Clear();
            }
            // _logger.Info("МАТРИЦА СООТВЕТСТВИЙ");
            var aMatrix = GetMatrixOfMatches3D(elementList, nodeList); //МАТРИЦА СООТВЕТСТВИЙ 
            //_logger.Info("Разряженая квазиддиагональная матрица");
            var spareKvazi = GetSpareKvaziMatrix3D(kGlobalList3D);
            //_logger.Info("Разряженая матрица жесткости конструкции");
            var spareMatrix = aMatrix.Transpose().Multiply(spareKvazi).Multiply(aMatrix);

            var forceVectorList = new List<double[]>();
            var group = 0;
            _logger.Info("Задание закреплений и расчет МКЭ");
            for (var f = 0; f < construction.ForceGroups.Count; f++)
            {
                TaskOfFixing3D(construction, spareMatrix); //Задание закреплений
                forceVectorList.Add(GenerateForceVector3D(construction, f));



                var forceVector = forceVectorList[f];

                var solve = new double[construction.Nodes.Count * 6];

                //РЕШЕНИЕ НА ЦПУ
                if (cpuFlag)
                {

                    solve = spareMatrix.Solve(DenseVector.Build.DenseOfArray(forceVector)).ToArray();

                    ////var solve2 = spareMatrix.Solve(DenseVector.Build.DenseOfArray(forceVector)).ToArray();
                    ////var solve2 = new LinearSystem(spareMatrix.AsArray(), forceVector).XVector;
                }
                else
                {

                    //var storage =
                    //    spareMatrix.Storage as
                    //        MathNet.Numerics.LinearAlgebra.Storage.SparseCompressedRowMatrixStorage<double>;
                    ////Содержит необходимую 
                    //// информацию о разреженной матрице в CSR формате
                    //var nonZeroValues = storage.EnumerateNonZero().ToArray(); //Получаем ненулевые элементы матрицы
                    //if (nonZeroValues.Length < 50)
                    //    _logger.Error("Подозрительно мало ненулевых элеменовв в " + construction.Name + ". Размер матрицы - " + spareMatrix.ColumnCount + "X" + spareMatrix.RowCount);
                    //solve = new double[spareMatrix.RowCount]; //Выделяется память под результат расчета

                    //var sp = new ManagedCuda.CudaSolve.CudaSolveSparse(); //Создаем решатель из библиотеки ManagedCuda
                    //var matrixDescriptor = new ManagedCuda.CudaSparse.CudaSparseMatrixDescriptor();
                    //// Создается дескриптор матрицы
                    //var tolerance = 0.1.Pow(12); //Точность расчета. Значение взято для иллюстрации

                    //sp.CsrlsvluHost(spareMatrix.RowCount, nonZeroValues.Length, matrixDescriptor, nonZeroValues,
                    //    storage.RowPointers, storage.ColumnIndices, forceVector,
                    //    tolerance, 1, solve); //Решение СЛАУ методом LU факторизации
                    //                          //Решение на ВидеоКарте
                    //                          //storage.Clear();


                }

                var calculateSolveVector = DenseVector.Build.DenseOfArray(solve);

                //Вектор перемещений в глобальной системе координат
                SolvedMovingToNodesGroups(construction, solve);

                //Помещение перемещений в узлы в глобальной системе координат
                var solveVectorToElements = aMatrix.Multiply(calculateSolveVector);
                //Приведённые в соответствие к элементам перемещения
                var s0 = spareKvazi.Multiply(solveVectorToElements); //Усилия у элементов в глобальной системе координат
                //Выборка опорных реакций
                foreach (var i in construction.LowerNodesNumber)
                {
                    var nS = 0.0;
                    var qyS = 0.0;
                    var qzS = 0.0;
                    foreach (var element in construction.Elements)
                    {
                        if (element.StartNode.Number == i)
                        {

                            nS += s0[element.Number * 12 + 2] * -1;
                            qyS += s0[element.Number * 12 + 0] * -1;
                            qzS += s0[element.Number * 12 + 1] * -1;
                        }
                        if (element.EndNode.Number == i)
                        {

                            nS += s0[element.Number * 12 + 2] * 1;
                            qyS += s0[element.Number * 12 + 0] * 1;
                            qzS += s0[element.Number * 12 + 1] * 1;
                        }

                    }
                    construction.Nodes[i].RectionList.Add(new Reactions() { N = nS, Qy = qyS, Qz = qzS });
                }
                var s1 = kvaziTmatrix.Multiply(s0).ToArray(); //Усилия у элементов в локальной системе координат

                InternalForcesVectorToElementsGroup(elementList, s1);
                
               group++;
                s0.Clear();
                solveVectorToElements.Clear();
                calculateSolveVector.Clear();
            }

          

            kGlobalList3D.Clear();
            kvaziTmatrix.Clear();
            spareKvazi.Clear();
            spareMatrix.Clear();
            tList3D.Clear();
            kLocalList3D.Clear();

            aMatrix.Clear();
            forceVectorList.Clear();
            


        }
        private static void SolvedMovingToNodesGroups(Construction construction, double[] solve)
        {
            for (var i = 0; i < construction.Nodes.Count; i++)
            {
                var moving = new Moving
                {
                    X = solve[i * 6],
                    Y = solve[i * 6 + 1],
                    Z = solve[i * 6 + 2],
                    Ux = solve[i * 6 + 3],
                    Uy = solve[i * 6 + 4],
                    Uz = solve[i * 6 + 5]
                };

                construction.Nodes[i].MovingList.Add(moving);
            }
        }

        private static void InternalForcesVectorToElementsGroup(List<Element> elementListFromFile, double[] s0)
        {
            for (var i = 0; i < elementListFromFile.Count; i++)
            {

                var stress = new InternalStressElement
                {
                    StartN = s0[i * 12 + 0] * (-1),
                    StartQy = Math.Abs(s0[i * 12 + +1]),
                    StartQz = Math.Abs(s0[i * 12 + +2]),
                    StartMx = 1 * s0[i * 12 + +3],
                    StartMy = -1 * s0[i * 12 + +4],
                    StartMz = -1 * s0[i * 12 + +5],
                    EndN = s0[i * 12 + 6],
                    EndQy = Math.Abs(s0[i * 12 + +7]),
                    EndQz = Math.Abs(s0[i * 12 + +8]),
                    EndMx = -1 * s0[i * 12 + +9],
                    EndMy = 1 * s0[i * 12 + +10],
                    EndMz = 1 * s0[i * 12 + +11]
                };

                elementListFromFile[i].InternalStressList.Add(stress);
            }
        }
       
        /// <summary>
        /// Сбор нагрузок по всем узлам и сведения их в суммарную по каждой оси с удалением нагрузок вдоль связей в каждом узле
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="forceGroup"></param>
        /// <returns></returns>
        private static double[] GenerateForceVector3D(Construction construction, int forceGroup)
        {
            var nodeList = construction.Nodes;
            var forceVector = new double[nodeList.Count * 6];
            for (var i = 0; i < nodeList.Count; i++)
            {
                
                if (nodeList[i].ForceGroups[forceGroup].Forces.Count == 0)
                    continue;
                foreach (var force in nodeList[i].ForceGroups[forceGroup].Forces)
                {
                    var conf = nodeList[i].Confinement;
                    var x = false;
                    var y = false;
                    var z = false;
                    var rx = false;
                    var ry = false;
                    var rz = false;
                    foreach (var confinementse in conf)
                    {
                        if (confinementse == Confinements.FixedDx)
                            x = true;
                        if (confinementse == Confinements.FixedDy)
                            y = true;
                        if (confinementse == Confinements.FixedDz)
                            z = true;
                        if (confinementse == Confinements.FixedRx)
                            rx = true;
                        if (confinementse == Confinements.FixedRy)
                            ry = true;
                        if (confinementse == Confinements.FixedRz)
                            rz = true;
                    }
                    forceVector[i * 6] = x ? 0 : forceVector[i * 6] + force.Xforce;
                    forceVector[i * 6 + 1] = y ? 0 : forceVector[i * 6 + 1] + force.Yforce;
                    forceVector[i * 6 + 2] = z ? 0 : forceVector[i * 6 + 2] + force.Zforce;
                    forceVector[i * 6 + 3] = rx ? 0 : forceVector[i * 6 + 3] + force.Xmoment;
                    forceVector[i * 6 + 4] = ry ? 0 : forceVector[i * 6 + 4] + force.Ymoment;
                    forceVector[i * 6 + 5] = rz ? 0 : forceVector[i * 6 + 5] + force.Zmoment;

                }


            }

            //forceVector[546*6 + 3] = 1;
            return forceVector;
        }

        /// <summary>
        /// Задание закреплений 3D
        /// </summary>
      
        private static void TaskOfFixing3D(Construction construction, Matrix<double> k0Matrix3D)
        {
            var nodeList = construction.Nodes;

            //Задание закреплений
            for (var i = 0; i < nodeList.Count; i++)
            {
                FixNode(nodeList, k0Matrix3D, i, false);
            }

        }
        /// <summary>
        /// Закрепление узла либо полное снятие связей с него
        /// </summary>
        /// <param name="nodeList">Коллекция узлов</param>
        /// <param name="k0Matrix3D">Матрица</param>
        /// <param name="i">Номер узла</param>
        /// <param name="relize">Если истина - с узла снимаются все связи, в том числе и диагональные</param>
        private static void FixNode(List<Node> nodeList, Matrix<double> k0Matrix3D, int i, bool relize)
        {
            if (nodeList[i].Confinement.Contains(Confinements.FixedDx) && !relize)
            {
                for (var j = 0; j < k0Matrix3D.ColumnCount; j++) //Вся строчка равна нулю
                {
                    k0Matrix3D[i * 6, j] = 0;
                }
                for (var j = 0; j < k0Matrix3D.RowCount; j++) //Вся колонка равна нулю
                {
                    k0Matrix3D[j, i * 6] = 0;
                }
                k0Matrix3D[i * 6, i * 6] = 1; //Диагональый элемент равен 1
            }

            if (nodeList[i].Confinement.Contains(Confinements.FixedDy) && !relize)
            {
                for (var j = 0; j < k0Matrix3D.ColumnCount; j++) //Вся строчка равна нулю
                {
                    k0Matrix3D[i * 6 + 1, j] = 0;
                }
                for (var j = 0; j < k0Matrix3D.RowCount; j++) //Вся колонка равна нулю
                {
                    k0Matrix3D[j, i * 6 + 1] = 0;
                }
                k0Matrix3D[i * 6 + 1, i * 6 + 1] = 1; //Диагональый элемент равен 1
            }
            if (nodeList[i].Confinement.Contains(Confinements.FixedDz) && !relize)
            {
                for (var j = 0; j < k0Matrix3D.ColumnCount; j++) //Вся строчка равна нулю
                {
                    k0Matrix3D[i * 6 + 2, j] = 0;
                }
                for (var j = 0; j < k0Matrix3D.RowCount; j++) //Вся колонка равна нулю
                {
                    k0Matrix3D[j, i * 6 + 2] = 0;
                }
                k0Matrix3D[i * 6 + 2, i * 6 + 2] = 1; //Диагональый элемент равен 1
            }
            if (nodeList[i].Confinement.Contains(Confinements.FixedRx) && !relize)
            {
                for (var j = 0; j < k0Matrix3D.ColumnCount; j++) //Вся строчка равна нулю
                {
                    k0Matrix3D[i * 6 + 3, j] = 0;
                }
                for (var j = 0; j < k0Matrix3D.RowCount; j++) //Вся колонка равна нулю
                {
                    k0Matrix3D[j, i * 6 + 3] = 0;
                }
                k0Matrix3D[i * 6 + 3, i * 6 + 3] = 1; //Диагональый элемент равен 1
            }
            if (nodeList[i].Confinement.Contains(Confinements.FixedRy) && !relize)
            {
                for (var j = 0; j < k0Matrix3D.ColumnCount; j++) //Вся строчка равна нулю
                {
                    k0Matrix3D[i * 6 + 4, j] = 0;
                }
                for (var j = 0; j < k0Matrix3D.RowCount; j++) //Вся колонка равна нулю
                {
                    k0Matrix3D[j, i * 6 + 4] = 0;
                }
                k0Matrix3D[i * 6 + 4, i * 6 + 4] = 1; //Диагональый элемент равен 1
            }
            if (nodeList[i].Confinement.Contains(Confinements.FixedRz) && !relize)
            {
                for (var j = 0; j < k0Matrix3D.ColumnCount; j++) //Вся строчка равна нулю
                {
                    k0Matrix3D[i * 6 + 5, j] = 0;
                }
                for (var j = 0; j < k0Matrix3D.RowCount; j++) //Вся колонка равна нулю
                {
                    k0Matrix3D[j, i * 6 + 5] = 0;
                }
                k0Matrix3D[i * 6 + 5, i * 6 + 5] = 1; //Диагональый элемент равен 1
            }
        }

        /// <summary>
        /// Получение матрицы соответствий
        /// </summary>
        /// <param name="elementList"></param>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        private static SparseMatrix GetMatrixOfMatches3D(List<Element> elementList, List<Node> nodeList)
        {

            var aMatrix = new SparseMatrix(elementList.Count * 12, nodeList.Count * 6);

            //Внесение шарниров в элемент
            //for (var l = 0; l < 12; l++)
            //{
            //    var v = elementList[k].LinksInElement.Links[l];
            //    for (var kk = 0; kk < 12; kk++)
            //    {
            //        currentMatrix[kk, l] = currentMatrix[kk, l] * v;//весь столбик
            //        currentMatrix[l, kk] = currentMatrix[l, kk] * v;//вся строка
            //    }
            //}

            for (var i = 0; i < elementList.Count; i++)
            {
                aMatrix[(i * 12), elementList[i].StartNode.Number * 6] = 1;
                aMatrix[(i * 12) + 1, elementList[i].StartNode.Number * 6 + 1] = 1;
                aMatrix[(i * 12) + 2, elementList[i].StartNode.Number * 6 + 2] = 1;
                aMatrix[(i * 12) + 3, elementList[i].StartNode.Number * 6 + 3] = 1;
                aMatrix[(i * 12) + 4, elementList[i].StartNode.Number * 6 + 4] = 1;
                aMatrix[(i * 12) + 5, elementList[i].StartNode.Number * 6 + 5] = 1;
                aMatrix[(i * 12) + 6, elementList[i].EndNode.Number * 6] = 1;
                aMatrix[(i * 12) + 7, elementList[i].EndNode.Number * 6 + 1] = 1;
                aMatrix[(i * 12) + 8, elementList[i].EndNode.Number * 6 + 2] = 1;
                aMatrix[(i * 12) + 9, elementList[i].EndNode.Number * 6 + 3] = 1;
                aMatrix[(i * 12) + 10, elementList[i].EndNode.Number * 6 + 4] = 1;
                aMatrix[(i * 12) + 11, elementList[i].EndNode.Number * 6 + 5] = 1;
                //var v = elementList[i].LinksInElement.Links;

                //aMatrix[(i * 12), elementList[i].StartNode.Number * 6] = 1 * v[0];
                //aMatrix[(i * 12) + 1, elementList[i].StartNode.Number * 6 + 1] = 1 * v[1];
                //aMatrix[(i * 12) + 2, elementList[i].StartNode.Number * 6 + 2] = 1 * v[2];
                //aMatrix[(i * 12) + 3, elementList[i].StartNode.Number * 6 + 3] = 1 * v[3];
                //aMatrix[(i * 12) + 4, elementList[i].StartNode.Number * 6 + 4] = 1 * v[4];
                //aMatrix[(i * 12) + 5, elementList[i].StartNode.Number * 6 + 5] = 1 * v[5];
                //aMatrix[(i * 12) + 6, elementList[i].EndNode.Number * 6] = 1 * v[6];
                //aMatrix[(i * 12) + 7, elementList[i].EndNode.Number * 6 + 1] = 1 * v[7];
                //aMatrix[(i * 12) + 8, elementList[i].EndNode.Number * 6 + 2] = 1 * v[8];
                //aMatrix[(i * 12) + 9, elementList[i].EndNode.Number * 6 + 3] = 1 * v[9];
                //aMatrix[(i * 12) + 10, elementList[i].EndNode.Number * 6 + 4] = 1 * v[10];
                //aMatrix[(i * 12) + 11, elementList[i].EndNode.Number * 6 + 5] = 1 * v[11];
            }
            return aMatrix;
        }

        private static SparseMatrix GetSpareKvaziMatrix3D(List<Matrix<double>> kGlobalList3D)
        {
            var sparse = new SparseMatrix(kGlobalList3D.Count * kGlobalList3D[0].RowCount, kGlobalList3D.Count * kGlobalList3D[0].ColumnCount);
            //var nonZeros = new List<double>();
            //var columnList = new List<short>();
            //var rowList = new List<short>();

            //var columnCount = 0;
            //var rowCount = 0;



            //var kvaziMatrix3D = Matrix<double>.Build.Dense(kGlobalList3D[0].RowCount * kGlobalList3D.Count, kGlobalList3D[0].ColumnCount * kGlobalList3D.Count);
            var kCount3D = 0;
            foreach (var kMatrix in kGlobalList3D)
            {
                for (var i = 0; i < kMatrix.RowCount; i++)
                {
                    for (var j = 0; j < kMatrix.ColumnCount; j++)
                    {
                        var row = kCount3D * kMatrix.RowCount + i;
                        var column = kCount3D * kMatrix.ColumnCount + j;
                        if (kMatrix[i, j] != 0)
                        {
                            sparse[row, column] = kMatrix[i, j];
                            //nonZeros.Add(kMatrix[i, j]);
                            //columnList.Add((short)column);
                            //rowList.Add((short)row);
                        }

                        //kvaziMatrix3D[kCount3D * kMatrix.RowCount + i, kCount3D * kMatrix.ColumnCount + j] = kMatrix[i, j];
                    }
                }
                kCount3D++;
            }

            //for (var i = 0; i < nonZeros.Count; i++)
            //{
            //    sparse[]
            //}

            return sparse;
        }
        /// <summary>
        /// Получение квазидиагональной матрицы 3L
        /// </summary>
        /// <param name="kGlobalList3D"></param>
        /// <returns></returns>
        private static Matrix<double> GetKvaziMatrix3D(List<Matrix<double>> kGlobalList3D)
        {
            var kvaziMatrix3D = Matrix<double>.Build.Dense(kGlobalList3D[0].RowCount * kGlobalList3D.Count, kGlobalList3D[0].ColumnCount * kGlobalList3D.Count);
            var kCount3D = 0;
            foreach (var kMatrix in kGlobalList3D)
            {
                for (var i = 0; i < kMatrix.RowCount; i++)
                {
                    for (var j = 0; j < kMatrix.ColumnCount; j++)
                    {
                        kvaziMatrix3D[kCount3D * kMatrix.RowCount + i, kCount3D * kMatrix.ColumnCount + j] = kMatrix[i, j];
                    }
                }
                kCount3D++;
            }
            return kvaziMatrix3D;
        }

        /// <summary>
        /// Получение матрицы ортогональных трансформации 3D
        /// </summary>
        /// <param name="elementList"></param>
        /// <returns></returns>
        private static List<Matrix<double>> GetTransformatinMatrix3D(List<Element> elementList)
        {
            var tList3D = new List<Matrix<double>>();

            foreach (var element in elementList)
            {
                //var currentMatrix = GenerateTmatrix(element);
                if (element.Number == 348)
                {

                }
                var currentMatrix = GenerateTmatrixNew(element);
                // var currentMatrix = GenerateTmatrixTekla(element, construction);

                var matrix = GetKvaziMatrix3D(new List<Matrix<double>>() { currentMatrix, currentMatrix, currentMatrix, currentMatrix });


                tList3D.Add(matrix);
            }
            return tList3D;
        }

        private static Matrix<double> GenerateTmatrixNew(Element element)
        {
            var sp = element.StartNode.Point;
            var ep = element.EndNode.Point;
            var matrix = Matrix<double>.Build.Dense(3, 3);

            var cxx = 0.0;
            var cxy = 0.0;
            var cxz = 0.0;

            var cyx = 0.0;
            var cyy = 0.0;
            var cyz = 0.0;

            var czx = 0.0;
            var czy = 0.0;
            var czz = 0.0;

            var v = ep - sp;
            if (Math.Abs(ep.X - sp.X) <= 1e-8 && Math.Abs(ep.Y - sp.Y) <= 1e-8) //вертикальный стержень
            {
                if (ep.Z > sp.Z)//снизу-вверх
                {
                    czx = 1;
                    cyy = 1;
                    cxz = -1;
                }
                else
                {
                    czx = -1;
                    cyy = 1;
                    cxz = 1;
                }
            }
            else
            {
                var l = element.Length();
                cxx = v.X / l;
                cyx = v.Y / l;
                czx = v.Z / l;
                var d = Math.Sqrt(cxx * cxx + cyx * cyx);
                cxy = -cyx / d;
                cyy = cxx / d;
                czy = 0;
                cxz = -cxx * czx / d;
                cyz = -cyx * czx / d;
                czz = d;
            }

            var teta = 45 + element.Angle;

            var s = Math.Sin(teta * Math.PI / 180.0);
            var c = Math.Cos(teta * Math.PI / 180.0);
            
            matrix[0, 0] = cxx;
            matrix[1, 0] = cxy * c + cxz * s;
            matrix[2, 0] = -cxy * s + cxz * c;

            matrix[0, 1] = cyx;
            matrix[1, 1] = cyy * c + cyz * s;
            matrix[2, 1] = -cyy * s + cyz * c;

            matrix[0, 2] = czx;
            matrix[1, 2] = czy * c + czz * s;
            matrix[2, 2] = -czy * s + czz * c;
            

            return matrix;
        }
        /// <summary>
        /// Получение матрицы жёсткости КЭ в локальной системе координат для 3D
        /// </summary>
        /// <param name="elementList"></param>
        /// <param name="e"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        private static List<Matrix<double>> GetStiffnessMatrix3D(List<Element> elementList, double e, double g, Construction construction)
        {
            var kLocalList3D = new List<Matrix<double>>();
            for (var k = 0; k < elementList.Count; k++)
            {
                var currentElement = elementList[k];
                
                if (construction.IsNeedCorrectSortament) // корректировка сечения в случае ошибки расчёта МКЭ
                {
                    currentElement.Profile.Area = currentElement.Profile.Area * 1.0101;
                    currentElement.Profile.It = currentElement.Profile.It * 1.0102;
                    currentElement.Profile.Iy = currentElement.Profile.Iy * 1.0103;
                    currentElement.Profile.Iz = currentElement.Profile.Iz * 1.0104;
                    currentElement.Profile.Iv = currentElement.Profile.Iv * 1.0105;
                }

                var currentElementLength = currentElement.Length();

                var currentMatrix = Matrix<double>.Build.Dense(12, 12);
                for (var j = 0; j < 12; j++)
                {
                    for (var i = 0; i < 12; i++)
                    {
                        if (i == j) //Если диагональ
                        {
                            if (i == 0 || i == 6)
                            {
                                currentMatrix[i, j] = e * currentElement.Profile.Area / currentElementLength;
                            }
                            else if (i == 1 || i == 7)
                            {
                                currentMatrix[i, j] = 12 * e * currentElement.Profile.Iz / currentElementLength.Pow(3);
                            }
                            else if (i == 2 || i == 8)
                            {
                                currentMatrix[i, j] = 12 * e * currentElement.Profile.Iy / currentElementLength.Pow(3);
                            }
                            else if (i == 3 || i == 9)
                            {
                                currentMatrix[i, j] = g * currentElement.Profile.It / currentElementLength;
                            }
                            else if (i == 4 || i == 10)
                            {
                                currentMatrix[i, j] = 4 * e * currentElement.Profile.Iy / currentElementLength;
                            }
                            else if (i == 5 || i == 11)
                            {
                                currentMatrix[i, j] = 4 * e * currentElement.Profile.Iz / currentElementLength;
                            }
                        }
                    }

                    currentMatrix[4, 2] = currentMatrix[2, 4] = currentMatrix[10, 2] = currentMatrix[2, 10]
                        = -6 * e * currentElement.Profile.Iy / currentElementLength.Pow(2);
                    currentMatrix[5, 1] =
                        currentMatrix[1, 5] =
                            currentMatrix[11, 1] =
                                currentMatrix[1, 11] = 6 * e * currentElement.Profile.Iz / currentElementLength.Pow(2);
                    currentMatrix[6, 0] = currentMatrix[0, 6] = -1 * e * currentElement.Profile.Area / currentElementLength;
                    currentMatrix[7, 1] = currentMatrix[1, 7] = -1 * currentMatrix[1, 1];
                    currentMatrix[7, 5] =
                        currentMatrix[5, 7] = currentMatrix[11, 7] = currentMatrix[7, 11] = -1 * currentMatrix[11, 1];
                    currentMatrix[8, 2] = currentMatrix[2, 8] = -1 * currentMatrix[2, 2];
                    currentMatrix[8, 4] =
                        currentMatrix[4, 8] = currentMatrix[10, 8] = currentMatrix[8, 10] = -1 * currentMatrix[4, 2];
                    currentMatrix[9, 3] = currentMatrix[3, 9] = -1 * currentMatrix[3, 3];

                    currentMatrix[10, 4] = currentMatrix[4, 10] = 2 * e * currentElement.Profile.Iy / currentElementLength;
                    currentMatrix[11, 5] = currentMatrix[5, 11] = 2 * e * currentElement.Profile.Iz / currentElementLength;
                }
                var secondMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
         {
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0,0,0,0,0},
         });
                var relizedDof = new List<int>();//удаляемые степени свободы

                if (relizedDof.Count > 0)
                {
                    #region Получение второй матрицы


                    var rCount = -1;
                    var cCount = -1;

                    //удаление лишних строк и столбцов
                    for (var r = 0; r < 12; r++)
                    {
                        if (!relizedDof.Contains(r))
                        {
                            rCount++;
                            for (var c = 0; c < 12; c++)
                            {
                                if (!relizedDof.Contains(c)) //если степень свободы не нужно выбрасывать
                                {
                                    cCount++;
                                    secondMatrix[rCount, cCount] = currentMatrix[r, c];
                                }
                            }
                            cCount = -1;
                        }
                    }
                    //заполнение столбцов крайних недиагональными значениями степеней свободы, которыйе нужно удалить
                    var relizedDoFcount = relizedDof.Aggregate(12, (current, i) => current - 1);
                    var range = relizedDoFcount; //размер матрицы, за исключением нулевых строк/столбцов
                    var doFnumber = 0;
                    for (var c = range; c < 12; c++) //номер столбца во Второй матрице
                    {
                        var i = relizedDof[doFnumber]; //текущая степень свободы
                        for (var r = 0; r < relizedDoFcount; r++) //Номер строчки во Второй матрице
                        {
                            for (var f = 0; f < 12; f++) //номер строчки в оригинальной матрице
                            {
                                if (!relizedDof.Contains(f))
                                {
                                    secondMatrix[r, c] = currentMatrix[f, i];
                                    r++;
                                }
                            }
                        }
                        //заполнение крайних строу не диагональными значениями степеней свободы, которыйе нужно удалить

                        doFnumber++;
                    }
                    //заполнение крайних строу не диагональными значениями степеней свободы, которыйе нужно удалить
                    doFnumber = 0;
                    for (var rr = range; rr < 12; rr++) //номер строки во Второй матрице
                    {
                        var i = relizedDof[doFnumber]; //текущая степень свободы
                        for (var cc = 0; cc < relizedDoFcount; cc++) //Номер столбца во Второй матрице
                        {
                            for (var kk = 0; kk < 12; kk++) //номер строчки в оригинальной матрице
                            {
                                if (!relizedDof.Contains(kk))
                                {
                                    secondMatrix[rr, cc] = currentMatrix[i, kk];
                                    cc++;
                                }
                            }
                        }
                        doFnumber++;
                    }
                    //заполнение диагональными значениями
                    doFnumber = 0;
                    for (var rrr = range; rrr < 12; rrr++) //номер строки во Второй матрице
                    {
                        var i = relizedDof[doFnumber]; //текущая строка в оригинальной матрице
                        var index = 0; //номер столбца в оригинальной матрице
                        for (var ccc = range; ccc < 12; ccc++) //номер строки во Второй матрице
                        {
                            secondMatrix[rrr, ccc] = currentMatrix[i, relizedDof[index]];
                            index++;
                        }
                        doFnumber++;
                    }

                    #endregion

                    //математические операции

                    var smallMatrix = secondMatrix.SubMatrix(range, 12 - range, range, 12 - range);
                    //матрица из элементов на пересечении
                    var rightMatrix = secondMatrix.SubMatrix(0, range, range, 12 - range);
                    var downMatrix = secondMatrix.SubMatrix(range, 12 - range, 0, range);
                    var mainPart = secondMatrix.SubMatrix(0, range, 0, range);


                    try
                    {
                        var a = smallMatrix.Inverse();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка инвертирования матрицы при введении шарниров в элементы: " + ex.Message);
                        _logger.Error("Ошибка инвертирования матрицы при введении шарниров в элементы: " + ex.Message);
                    }
                    var b = rightMatrix.Multiply(smallMatrix.Inverse());
                    var result = b.Multiply(downMatrix);
                    result = -result + mainPart;

                    for (var r = 0; r < 12; r++)//обнуление матрицы Ж КЭ
                    {
                        for (var c = 0; c < 12; c++)
                        {
                            currentMatrix[r, c] = 0;
                        }
                    }
                    var rrrr = -1;

                    for (var r = 0; r < 12; r++)//обнуление матрицы Ж КЭ
                    {
                        var cccc = -1;
                        if (!relizedDof.Contains(r))
                        {
                            rrrr++;
                            for (var c = 0; c < 12; c++)
                            {
                                if (!relizedDof.Contains(c))
                                {
                                    cccc++;
                                    currentMatrix[r, c] = result[rrrr, cccc];
                                }
                            }

                        }
                    }




                }

                kLocalList3D.Add(currentMatrix);

            }
           
            return kLocalList3D;
        }
    }
}

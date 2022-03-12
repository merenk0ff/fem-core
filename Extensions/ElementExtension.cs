namespace FEM
{
    public static class ElementExtension
    {
     
        /// <summary>
        /// Длина элемента
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static double Length(this Element element)
        {
            return element.StartNode.Point.DistanceTo(element.EndNode.Point);
        }

        public static string ToString(this Element element, int forceGroupNumber)
        {
            return $"Element number {element.Number} ForceGroup is {forceGroupNumber}\n" +
                   $"\tInternal Stress list\n" +
                   $"\t\tStartN = {element.InternalStressList[forceGroupNumber].StartN}\n" +
                   $"\t\tStartQy = {element.InternalStressList[forceGroupNumber].StartQy}\n" +
                   $"\t\tStartQz = {element.InternalStressList[forceGroupNumber].StartQz}\n" +
                   $"\t\tStartMx = {element.InternalStressList[forceGroupNumber].StartMx}\n" +
                   $"\t\tStartMy = {element.InternalStressList[forceGroupNumber].StartMy}\n" +
                   $"\t\tStartMz = {element.InternalStressList[forceGroupNumber].StartMz}\n" +
                   $"\t\tEndN = {element.InternalStressList[forceGroupNumber].EndN}\n" +
                   $"\t\tEndQy = {element.InternalStressList[forceGroupNumber].EndQy}\n" +
                   $"\t\tEndQz = {element.InternalStressList[forceGroupNumber].EndQz}\n" +
                   $"\t\tEndMx = {element.InternalStressList[forceGroupNumber].EndMx}\n" +
                   $"\t\tEndMy = {element.InternalStressList[forceGroupNumber].EndMy}\n" +
                   $"\t\tEndMz = {element.InternalStressList[forceGroupNumber].EndMz}\n" +
                   $"\tMoving List\n" +
                   $"\t\tX = {element.StartNode.MovingList[forceGroupNumber].X}\n" +
                   $"\t\tY = {element.StartNode.MovingList[forceGroupNumber].Y}\n" +
                   $"\t\tZ = {element.StartNode.MovingList[forceGroupNumber].Z}\n" +
                   $"\t\tUx = {element.StartNode.MovingList[forceGroupNumber].Ux}\n" +
                   $"\t\tUy = {element.StartNode.MovingList[forceGroupNumber].Uy}\n" +
                   $"\t\tUz = {element.StartNode.MovingList[forceGroupNumber].Uz}\n\n";
        }
    }
}

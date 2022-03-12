using System;
using System.ComponentModel;

namespace FEM
{
    [Serializable]
    public enum Confinements
    {
        [Description("Свободный")]
        Relize,
        [Description("Закреплён по Х")]
        FixedDx,
        [Description("Закреплён по Y")]
        FixedDy,
        [Description("Закреплён по Z")]
        FixedDz,
        [Description("Закреплёно вращение вокруг Х")]
        FixedRx,
        [Description("Закреплёно вращение вокруг Y")]
        FixedRy,
        [Description("Закреплёно вращение вокруг Z")]
        FixedRz
    }
}

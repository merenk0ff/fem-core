using System;
using System.Collections.Generic;

namespace FEM
{
    [Serializable]
    public class GeometricCharacteristicsElement//Всё по пункту 10.2.6 СП 16,13330,2011
    {
        
        /// <summary>
        /// Тип схемы по рис 15 п.10.2.6 СП 16,13330,2011
        /// </summary>
        public char ShemeType { get; set; }
        public double N { get; set; }
        public double Md { get; set; }
        public double Lm { get; set; }
        public double Ld { get; set; }
        public double Ldc { get; set; }
        /// <summary>
        /// Предельное значение лямбда
        /// </summary>
        public double Limitλ { get; set; }
        /// <summary>
        /// Условная гибкость
        /// </summary>
        public double λwithLine { get; set; }
        /// <summary>
        /// Гибкость
        /// </summary>
        public double λ { get; set; }
        /// <summary>
        /// Расчётная длина
        /// </summary>
        public double Lef { get; set; }
        public double Lc { get; set; }
        
       
        public double LdivImin { get; set; }

        public double Nm { get; set; }
        public double Nmd { get; set; }
        public double NmdDivNm { get; set; }
        /// <summary>
        /// Ф-ла 209
        /// </summary>
        public double am1 { get; set; }
        /// <summary>
        /// ф-ла 210
        /// </summary>
        public double am2 { get; set; }
        public double am3 { get; set; }
        /// <summary>
        /// ф-ла 211
        /// </summary>
        public double ad1 { get; set; }
        /// <summary>
        /// ф-ла 212
        /// </summary>
        public double ad2 { get; set; }
        public double amDivAd { get; set; }
        /// <summary>
        /// Отношение риски к ширине профиля
        /// </summary>
        public double RiskDivB { get; set; }
        /// <summary>
        /// σ сжатия
        /// </summary>
        public double SigmaCompression { get; set; }
        /// <summary>
        /// σ растяжения
        /// </summary>
        public double SigmaStretch { get; set; }
        public double Sigma { get; set; }
        /// <summary>
        /// Коэфициент условий работы. Сжатие
        /// </summary>
        public double YcComp { get; set; }
        /// <summary>
        /// Коэфициент условий работы. Растяжение
        /// </summary>
        public double YcStretch { get; set; }
      
        public double Phi { get; set; }
        /// <summary>
        /// Усилие сжатия
        /// </summary>
        public List<double> NcompressionList { get; set; }
        /// <summary>
        /// Усилие растяжения
        /// </summary>
        public List<double> NstretchList { get; set; }
        /// <summary>
        /// Усилие сжатия
        /// </summary>
        public double Ncompression { get; set; }
        public string NcompressionGroup { get; set; }
        /// <summary>
        /// Усилие растяжения
        /// </summary>
        public double Nstretch { get; set; }
        public string NstretchGroup { get; set; }
        public double LyambdaLimit { get; set; }
        public double LyambdaLimitComp { get; set; }
        public double LyambdaLimitStretch { get; set; }
        public double Alpha { get; set; }


        public GeometricCharacteristicsElement()
        {
            N = -1;
            NcompressionList = new List<double>();
            NstretchList = new List<double>();
        }
    }

}

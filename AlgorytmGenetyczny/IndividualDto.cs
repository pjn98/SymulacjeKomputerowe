﻿using System.Collections.Generic;

namespace AlgorytmGenetyczny
{
    public class IndividualDto
    {
        public List<string> ParameterList { get; set; }
        public List<double> NewParameterValueList { get; set; }
        public List<int> OldParameterValueList { get; set; }
        public double AdaptationFunctionValue { get; set; }
    }
}
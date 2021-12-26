using System;
using System.Collections.Generic;

namespace AlgorytmGenetyczny
{
    public class Algorithm
    {
        private readonly Random _random = new Random();

        public List<IndividualDto> CreatePool(int poolSize, int bitSize)
        {
            var individualList = new List<IndividualDto>();
            for (var i = 0; i < poolSize; i++) individualList.Add(CreateIndividual(bitSize));

            return individualList;
        }

        private IndividualDto CreateIndividual(int bitSize)
        {
            var parameterList = CreateParameters(bitSize);
            var oldParameterValueList = new List<int>
                {Helper.ConvertGrayToDecimal(parameterList[0]), Helper.ConvertGrayToDecimal(parameterList[1])};
            return new IndividualDto
            {
                ParameterList = parameterList,
                OldParameterValueList = oldParameterValueList
            };
        }

        private List<string> CreateParameters(int bitSize)
        {
            var maxValue = (int) Math.Pow(2, bitSize) - 1;
            var x = _random.Next(0, maxValue);
            var y = _random.Next(0, maxValue);
            var parameterList = new List<string> {Helper.ConvertDecimalToGray(x), Helper.ConvertDecimalToGray(y)};
            return parameterList;
        }

        private double GetAdaptationFunctionValue(List<int> parameterValueList)
        {
            return Math.Pow(parameterValueList[0], 2) + parameterValueList[0] * parameterValueList[1] +
                   2 * parameterValueList[0] + Math.Pow(parameterValueList[1], 2);
        }
    }
}
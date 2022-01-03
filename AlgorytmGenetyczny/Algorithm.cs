using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace AlgorytmGenetyczny
{
    public class Algorithm
    {
        private readonly Random _random = new Random();

        public void Simulation(int poolSize, int bitSize, int iterations, int leftBorder, int rightBorder)
        {
            var pool = CreatePool(poolSize, bitSize, leftBorder, rightBorder);
            foreach (var individual in pool)
            {
            }

            var newPool = new List<IndividualDto>();
        }

        public List<IndividualDto> CreatePool(int poolSize, int bitSize, int leftBorder, int rightBorder)
        {
            var individualList = new List<IndividualDto>();
            for (var i = 0; i < poolSize; i++) individualList.Add(CreateIndividual(bitSize, leftBorder, rightBorder));

            return individualList;
        }

        private IndividualDto CreateIndividual(int bitSize, int leftBorder, int rightBorder)
        {
            var parameterList = CreateParameters(bitSize);
            var oldParameterValueList = new List<int>
                {Helper.ConvertGrayToDecimal(parameterList[0]), Helper.ConvertGrayToDecimal(parameterList[1])};
            var newParameterValueList = GetNewParametersValue(oldParameterValueList, leftBorder, rightBorder, bitSize);
            var adaptationFunctionValue = GetAdaptationFunctionValue(newParameterValueList);
            return new IndividualDto
            {
                ParameterList = parameterList,
                OldParameterValueList = oldParameterValueList,
                NewParameterValueList = newParameterValueList,
                AdaptationFunctionValue = adaptationFunctionValue
            };
        }

        private List<string> CreateParameters(int bitSize)
        {
            var maxValue = (int) Math.Pow(2, bitSize) - 1;
            var x = _random.Next(0, maxValue);
            var y = _random.Next(0, maxValue);
            var parameterList = new List<string>
                {Helper.ConvertDecimalToGray(bitSize, x), Helper.ConvertDecimalToGray(bitSize, y)};
            return parameterList;
        }

        private double GetAdaptationFunctionValue(List<double> parameterValueList)
        {
            return Math.Pow(parameterValueList[0], 2) + parameterValueList[0] * parameterValueList[1] +
                   2 * parameterValueList[0] + Math.Pow(parameterValueList[1], 2);
        }

        private List<double> GetNewParametersValue(List<int> OldParameterValueList, int leftBorder, int rightBorder,
            int bitSize)
        {
            var oldValueX = OldParameterValueList[0];
            var oldValueY = OldParameterValueList[1];
            var oldMinValue = 0;
            var oldMaxValue = Math.Pow(2, bitSize) - 1;
            var newX = (oldValueX - oldMinValue) / (oldMaxValue - oldMinValue) * (rightBorder - leftBorder) +
                       leftBorder;
            var newY = (oldValueY - oldMinValue) / (oldMaxValue - oldMinValue) * (rightBorder - leftBorder) +
                       leftBorder;

            return new List<double> {newX, newY};
        }

        private IndividualDto HotDeckOperator(List<IndividualDto> individualList)
        {
            return individualList.OrderBy(x => x.AdaptationFunctionValue).FirstOrDefault();
        }

        private List<IndividualDto> TournamentOperator(List<IndividualDto> individualList)
        {
            return null;
        }

        public List<IndividualDto> SinglePointMutation(List<IndividualDto> individualList, int bitSize, int leftBorder, int rightBorder)
        {
            foreach (var individual in individualList)
            {
                var bitOnParameterX = _random.Next(0, bitSize);
                var bitOnParameterY = _random.Next(0, bitSize);
                individual.ParameterList[0] = ReplaceAt(individual.ParameterList[0], bitOnParameterX);
                individual.ParameterList[1] = ReplaceAt(individual.ParameterList[1], bitOnParameterY);
                var oldParameterValueList = new List<int>
                    {Helper.ConvertGrayToDecimal(individual.ParameterList[0]), Helper.ConvertGrayToDecimal(individual.ParameterList[1])};
                var newParameterValueList = GetNewParametersValue(oldParameterValueList, leftBorder, rightBorder, bitSize);
                var adaptationFunctionValue = GetAdaptationFunctionValue(newParameterValueList);
            }

            return individualList;
        }

        private static string ReplaceAt(string input, int index)
        {
            var chars = input.ToCharArray();
            chars[index] = chars[index] == '0' ? '1' : '0';
            return new string(chars);
        }
    }
}
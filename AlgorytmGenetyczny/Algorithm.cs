using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace AlgorytmGenetyczny
{
    public class Algorithm
    {
        private readonly Random _random = new Random();

        public List<IndividualDto> Simulation(int poolSize, int bitSize, int iterations, int leftBorder, int rightBorder, int preservedSize, int tournamentSize, int newGenotypeSize)
        {
            bitSize--;
            var pool = CreatePool(poolSize, bitSize, leftBorder, rightBorder);
            var newPool = new List<IndividualDto>();
            for (var i = 0; i < iterations; i++)
            {
                newPool = GetPreservedPool(pool, preservedSize, tournamentSize);
                var newGenotypePool =
                    GetNewGenotypePool(newPool, bitSize, newGenotypeSize, leftBorder, rightBorder);
                newGenotypePool.ForEach(item => newPool.Add(item));

                pool = newPool;
            }

            return newPool;
        }

        public List<IndividualDto> CreatePool(int poolSize, int bitSize, int leftBorder, int rightBorder)
        {
            var individualList = new List<IndividualDto>();
            for (var i = 0; i < poolSize; i++) individualList.Add(CreateIndividual(bitSize, leftBorder, rightBorder));

            return individualList;
        }

        private List<IndividualDto> GetPreservedPool(List<IndividualDto> pool, int preservedSize, int tournamentSize)
        {
            var bestIndividual = HotDeckOperator(pool);
            pool.Remove(bestIndividual);
            var newPool = new List<IndividualDto> { bestIndividual };
            for (var i = 0; i < preservedSize - 1; i++)
            {
                var winner = TournamentOperator(pool, tournamentSize);
                newPool.Add(winner);
                pool.Remove(winner);
            }

            return newPool;
        }

        private List<IndividualDto> GetNewGenotypePool(List<IndividualDto> pool, int bitSize, int newGenotypeSize, int leftBorder, int rightBorder)
        {
            var newPool = new List<IndividualDto>();

            for (var i = 0; i < newGenotypeSize; i++)
            {
                var parents = GetParticipants(pool, 2);
                var point = _random.Next(0, bitSize);

                var parameterX = string.Concat(parents[0].ParameterList[0].Substring(0, point),
                    parents[1].ParameterList[0].Substring(point, bitSize - point));
                var parameterY = string.Concat(parents[0].ParameterList[1].Substring(0, point),
                    parents[1].ParameterList[1].Substring(point, bitSize - point));

                var parameterList = new List<string> { parameterX, parameterY };
                parameterList = SinglePointMutation(parameterList, bitSize);
                var oldParameterValueList = new List<int>
                    {Helper.ConvertGrayToDecimal(parameterList[0]), Helper.ConvertGrayToDecimal(parameterList[1])};
                var newParameterValueList = GetNewParametersValue(oldParameterValueList, leftBorder, rightBorder, bitSize);
                var adaptationFunctionValue = GetAdaptationFunctionValue(newParameterValueList);
                newPool.Add(new IndividualDto
                {
                    ParameterList = parameterList,
                    OldParameterValueList = oldParameterValueList,
                    NewParameterValueList = newParameterValueList,
                    AdaptationFunctionValue = adaptationFunctionValue
                });
            }

            return newPool;
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

        private List<double> GetNewParametersValue(List<int> oldParameterValueList, int leftBorder, int rightBorder,
            int bitSize)
        {
            var oldValueX = oldParameterValueList[0];
            var oldValueY = oldParameterValueList[1];
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

        private IndividualDto TournamentOperator(List<IndividualDto> individualList, int tournamentSize)
        {
            var tournamentParticipants = GetParticipants(individualList, tournamentSize);
            var tournamentWinner = HotDeckOperator(tournamentParticipants);
            return tournamentWinner;
        }

        public List<string> SinglePointMutation(List<string> parameterList, int bitSize)
        {
            var bitOnParameterX = _random.Next(0, bitSize);
            var bitOnParameterY = _random.Next(0, bitSize);
            parameterList[0] = ReplaceAt(parameterList[0], bitOnParameterX);
            parameterList[1] = ReplaceAt(parameterList[1], bitOnParameterY);

            return parameterList;
        }

        private static string ReplaceAt(string input, int index)
        {
            var chars = input.ToCharArray();
            chars[index] = chars[index] == '0' ? '1' : '0';
            return new string(chars);
        }

        private List<IndividualDto> GetParticipants(List<IndividualDto> parentPool, int tournamentSize)
        {
            var randomNumbersList = new List<int>();
            var tournamentParticipants = new List<IndividualDto>();
            while (tournamentParticipants.Count != tournamentSize)
            {
                var number = _random.Next(0, parentPool.Count);
                if (!randomNumbersList.Contains(number))
                {
                    randomNumbersList.Add(number);
                    tournamentParticipants.Add(parentPool[number]);
                }
            }

            return tournamentParticipants;
        }
    }
}
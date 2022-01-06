using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace AlgorytmGenetyczny
{
    public class Algorithm
    {
        private readonly Random _random = new Random();

        public List<IndividualDto> Simulation(int poolSize, int bitSize, int iterations, int leftBorder, int rightBorder, int preservedSize, int tournamentSize)
        {
            var pool = CreatePool(poolSize, bitSize, leftBorder, rightBorder);

            var newPool = GetPreserverPool(pool, preservedSize, tournamentSize);

            return newPool;
        }

        public List<IndividualDto> CreatePool(int poolSize, int bitSize, int leftBorder, int rightBorder)
        {
            var individualList = new List<IndividualDto>();
            for (var i = 0; i < poolSize; i++) individualList.Add(CreateIndividual(bitSize, leftBorder, rightBorder));

            return individualList;
        }

        private List<IndividualDto> GetPreserverPool(List<IndividualDto> pool, int preservedSize, int tournamentSize)
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

        private IndividualDto TournamentOperator(List<IndividualDto> individualList, int tournamentSize)
        {
            var tournamentParticipants = GetTournamentParticipants(individualList, tournamentSize);
            var tournamentWinner = HotDeckOperator(tournamentParticipants);
            return tournamentWinner;
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
                individual.OldParameterValueList = oldParameterValueList;
                individual.NewParameterValueList = newParameterValueList;
                individual.AdaptationFunctionValue = adaptationFunctionValue;
            }

            return individualList;
        }

        private static string ReplaceAt(string input, int index)
        {
            var chars = input.ToCharArray();
            chars[index] = chars[index] == '0' ? '1' : '0';
            return new string(chars);
        }

        private List<IndividualDto> GetTournamentParticipants(List<IndividualDto> parentPool, int tournamentSize)
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

        public static T Clone<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            };
        }
    }
}
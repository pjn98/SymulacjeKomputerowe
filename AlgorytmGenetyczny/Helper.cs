using System;

namespace AlgorytmGenetyczny
{
    public static class Helper
    {
        private static string ConvertDecimalToBinary(int bitSize, int number)
        {
            var binaryNumber = Convert.ToString(number, 2);
            var binary = "";
            for (var i = 0; i < bitSize - binaryNumber.Length; i++)
            {
                binary += "0";
            }

            binary += binaryNumber;
            return binary;
        }

        private static int ConvertBinaryToDecimal(string number)
        {
            var decimalNumber = Convert.ToInt32(number, 2);
            return decimalNumber;
        }
        private static char xor_c(char a, char b)
        {
            return (a == b) ? '0' : '1';
        }
        private static char flip(char c)
        {
            return (c == '0') ? '1' : '0';
        }

        private static string ConvertBinaryToGray(string number)
        {
            var gray = "";
            gray += number[0];
            for (var i = 1; i < number.Length; i++)
            {
                gray += xor_c(number[i - 1],
                    number[i]);
            }

            return gray;
        }

        private static string ConvertGrayToBinary(string number)
        {
            String binary = "";

            binary += number[0];

            for (var i = 1; i < number.Length; i++)
            {
                if (number[i] == '0')
                    binary += binary[i - 1];
                else
                    binary += flip(binary[i - 1]);
            }
            return binary;
        }

        public static string ConvertDecimalToGray(int bitSize, int number)
        {
            var decimalToBinary = ConvertDecimalToBinary(bitSize, number);
            return ConvertBinaryToGray(decimalToBinary);
        }

        public static int ConvertGrayToDecimal(string number)
        {
            var grayToBinary = ConvertGrayToBinary(number);
            return ConvertBinaryToDecimal(grayToBinary);
        }
    }
}
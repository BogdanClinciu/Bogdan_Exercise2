using System.Collections.Generic;
using UnityEngine.UI;
using PolynomialMath;
using UnityEngine;
using System.Linq;
using System;

namespace PolynomialUtils
{
    /// <summary>Polynomial parsing.
    /// <para>This class contains the methods needed for parsing a polynomial to string.</para>
    /// </summary>
    public class PolynomialParser
    {
        public const string NO_POLYNOMIAL = "No polynomial entered.";

        private static readonly List<char> allowedCharList = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', ' ', '.'};
        private static readonly List<string> superscriptMap = new List<string>() {"⁰", "¹", "²", "³", "⁴", "⁵", "⁶", "⁷", "⁸", "⁹"};
        private const string SPACE = " ";
        private const string X_MARK = "x";
        private const string PLUS_MARK = "+";
        private const string MINUS_MARK = "-";


        /// <summary>Parses input string to Polynomial and updates math notation text.
        /// <seealso cref="FormatMathNotationPolynomial.cs"/>
        /// </summary>
        public static void ParsePolynomial(string input, Text mathNotationText, Polynomial polynomial)
        {
            if(input.Length > 0)
            {
                string[] splitInput = input.Split(' ');
                polynomial.elements.Clear();
                for (int i = splitInput.Length - 1; i >= 0; i--)
                {
                    float coeficient = 0;
                    int power = i;

                    if(splitInput[(splitInput.Length - 1) - power] != string.Empty)
                    {
                        if (float.TryParse(splitInput[(splitInput.Length - 1) - power], out coeficient))
                        {
                            polynomial.elements.Add(power, coeficient);
                            mathNotationText.text = FormatMathNotationPolynomial(polynomial);
                        }
                        else if (splitInput.Length.Equals(1) && splitInput[0].Equals("-"))
                        {
                            polynomial.elements.Add(power, -1);
                            mathNotationText.text = FormatMathNotationPolynomial(polynomial);
                        }
                    }
                }
            }
            else
            {
                mathNotationText.text = FormatMathNotationPolynomial(null);
            }
        }

        /// <summary>Returns the mathematical notation for the input polynomial.
        /// </summary>
        public static string FormatMathNotationPolynomial(Polynomial inputpolynomial)
        {
            if(inputpolynomial == null)
            {
                return NO_POLYNOMIAL;
            }

            string mathNotation = string.Empty;

            for (int i = 0; i < inputpolynomial.elements.Count ; i++)
            {
                KeyValuePair<int,float> element = inputpolynomial.elements.ElementAt(i);

                //We have chosen to hide 0 coeficient values so we only add non zero values to the mathematical notation string
                if(!element.Value.Equals(0))
                {
                    mathNotation    += DetermineSign(element.Value, (i.Equals(0)), mathNotation)
                                    + Mathf.Abs(element.Value)
                                    + ToSuperscript(inputpolynomial.elements.Keys.ElementAt(i))
                                    + SPACE;
                }
            }

            return mathNotation;
        }

        /// <summary>Returns the exponent character if it is necessary.
        /// </summary>
        private static string ToSuperscript(int value)
        {
            string superscript = string.Empty;

            if(value <= 1)
            {
                return (value.Equals(0)) ? superscript : X_MARK;
            }

            superscript += X_MARK;
            while(value > 0)
            {
                superscript = superscript.Insert(1, superscriptMap[value % 10]);
                value /= 10;
            }

            return superscript;
        }

        /// <summary>Returns the corect sign if any (for the value parameter) based on location in string.
        /// </summary>
        private static string DetermineSign(float value, bool isLast, string notationString)
        {
            if (value >= 0)
            {
                if(!isLast && notationString.Length > 0)
                {
                    return PLUS_MARK + SPACE;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return MINUS_MARK + SPACE;
            }
        }

        /// <summary>Returns curent polynomial input character if it is valid.
        /// </summary>
        /*Returns either an empty character or if the enrty is valid the curent input character
            special cases include the decimal point, minus and space characters.
            In these cases we check to see if the curent character would make sense within the context of the input string.
            For example the space character will not be returned if it is preceded by another space.
         */
        public static char PolynomialInputValidation(string input, int charIndex, char addedChar)
        {
            if(!allowedCharList.Contains(addedChar))
            {
                return new char();
            }
            else
            {
                if(addedChar.Equals('-'))
                {
                    if (charIndex.Equals(0) || input[charIndex - 1].Equals(' '))
                    {
                        return addedChar;
                    }
                    else
                    {
                        return new char();
                    }
                }
                else if(addedChar.Equals(' '))
                {
                    if(!charIndex.Equals(0) && !input[charIndex - 1].Equals(' ') && !input[charIndex - 1].Equals('-'))
                    {
                        return addedChar;
                    }
                    else
                    {
                        return new char();
                    }
                }
                else if (addedChar.Equals('.'))
                {
                    if(input.Length > 0 && Char.IsDigit(input[charIndex -1]))
                    {
                        bool canPlaceDecimalPoint = false;
                        for(int i = input.Length -1; i >= 0; i--)
                        {
                            if(input[i].Equals(' '))
                            {
                                canPlaceDecimalPoint = true;
                            }

                            if(input[i].Equals('.'))
                            {
                                canPlaceDecimalPoint = false;
                            }

                            canPlaceDecimalPoint = true;
                        }
                        return (canPlaceDecimalPoint) ? addedChar : new char();
                    }
                    else
                    {
                        return new char();
                    }
                }
                else
                {
                    return addedChar;
                }
            }
        }
    }

    public class UI_Utils
    {
        /// <summary>Sets button interactability to state for each button in the given array.
        /// </summary>
        public static void ToggleButtonInteractability(Button[] buttonArray, bool targetState)
        {
            foreach (Button button in buttonArray)
            {
                button.interactable = targetState;
            }
        }
    }
}

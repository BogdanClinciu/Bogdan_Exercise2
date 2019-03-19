﻿using System.Collections.Generic;
using UnityEngine.UI;
using PolynomeMath;
using UnityEngine;
using System.Linq;
using System;

namespace PolynomeUtils
{
    /// <summary>Polinomial parsing.
    /// <para>This class contains the methods needed for parsing a polynomial to string.</para>
    /// </summary>
    public class Parser
    {
        public const string NO_POLYNOME = "No polynome entered.";

        private static readonly List<char> allowedCharList = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', ' ', '.'};
        private static readonly List<string> superscriptMap = new List<string>() {"⁰", "¹", "²", "³", "⁴", "⁵", "⁶", "⁷", "⁸", "⁹"};
        private const string INVALID_ENTRY = "Something went wrong, please re-enter the coeficients.";
        private const string SPACE = " ";
        private const string X_MARK = "x";
        private const string PLUS_MARK = "+";
        private const string MINUS_MARK = "-";


        public static void ParsePolynome(string input, Text propperMathText, Polynome polynome)
        {
            if(input.Length > 0)
            {
                string[] splitInput = input.Split(' ');
                polynome.elements.Clear();
                for (int i = splitInput.Length - 1; i >= 0; i--)
                {
                    float coeficient = 0;
                    int power = i;

                    if(splitInput[(splitInput.Length - 1) - power] != string.Empty)
                    {
                        if (float.TryParse(splitInput[(splitInput.Length - 1) - power], out coeficient))
                        {
                            polynome.elements.Add(power, coeficient);
                            propperMathText.text = FormatMathNotationText(polynome);
                        }
                        else if (splitInput.Length.Equals(1) && splitInput[0].Equals("-"))
                        {
                            Debug.Log(splitInput.Length);
                            polynome.elements.Add(power, -1);
                            propperMathText.text = FormatMathNotationText(polynome);
                        }
                    }
                }
            }
            else
            {
                propperMathText.text = FormatMathNotationText(null);
            }
        }

        public static string ParsePolynome(Polynome inputPolynome)
        {
            return FormatMathNotationText(inputPolynome);
        }

        private static string FormatMathNotationText(Polynome inputPolynome)
        {
            if(inputPolynome == null)
            {
                return NO_POLYNOME;
            }

            string mathNotation = string.Empty;
            for (int i = 0; i < inputPolynome.elements.Count ; i++)
            {
                KeyValuePair<int,float> element = inputPolynome.elements.ElementAt(i);
                mathNotation    += DetermineSign(element.Value, (i.Equals(0)))
                                + Mathf.Abs(element.Value)
                                + ToSuperscript(inputPolynome.elements.Keys.ElementAt(i))
                                + SPACE;
            }

            return mathNotation;
        }

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

        private static string DetermineSign(float value, bool isLast)
        {
            if (value >= 0)
            {
                if(!isLast)
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

        public static char PolynomeInputValidation(string input, int charIndex, char addedChar)
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
        public static void ToggleButtonInteractability(Button[] buttonArray, bool targetState)
        {
            foreach (Button button in buttonArray)
            {
                button.interactable = targetState;
            }
        }
    }
}
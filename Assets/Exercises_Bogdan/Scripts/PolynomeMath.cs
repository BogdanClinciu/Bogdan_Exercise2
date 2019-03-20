using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PolynomeMath
{
    public class Polynome
    {
        /// <summary> Contains the power as key and coeficient as value for each element of the polynome.
        /// </summary>
        public Dictionary<int, float> elements;

        //DefaultConstructor
        public Polynome()
        {

        }

        //Specific constructor
        public Polynome(Dictionary<int, float> elements)
        {
            elements = new Dictionary<int, float>(elements);
        }

        //Clone constructor
        public Polynome(Polynome polynomeToClone)
        {
            elements = new Dictionary<int, float>(polynomeToClone.elements);
        }

        public Polynome(bool initializeElements)
        {
            if(initializeElements)
            {
                elements = new Dictionary<int, float>();
            }
        }
    }


    /// <summary>Simple operations with polynomials.
    /// <para>This class contains simple polynomial operations:.</para>
    /// <para>Addition, Sudtraction, Multiplication and calculated value.
    /// </para>
    /// <seealso cref="Polynome.cs"/>
    /// <seealso cref="FuncC"/>
    /// </summary>
    public class FuncS
    {

        /// <summary>Add two polynomials together, and returns the resulting polynomial.
        /// </summary>
        public static Polynome PAdd(Polynome p1, Polynome p2)
        {
            Polynome result = new Polynome();
            result.elements = new Dictionary<int, float>(p1.elements);

            foreach (int key in p2.elements.Keys)
            {
                if(result.elements.ContainsKey(key))
                {
                    result.elements[key] += p2.elements[key];

                    if(result.elements[key].Equals(0))
                    {
                        result.elements.Remove(key);
                    }
                }
                else
                {
                    result.elements.Add(key, p2.elements[key]);
                }
            }

            result.elements = result.elements.OrderByDescending(a => a.Key).ToDictionary(a => a.Key, a => a.Value);

            return result;
        }

        /// <summary>Subtracts the second polynomial from the first and returns the resulting polynomial.
        /// </summary>
        public static Polynome PSub(Polynome p1, Polynome p2)
        {
            Polynome result = new Polynome();
            result.elements = new Dictionary<int, float>(p1.elements);

            foreach (int key in p2.elements.Keys)
            {
                if(result.elements.ContainsKey(key))
                {
                    result.elements[key] -= p2.elements[key];

                    if(result.elements[key].Equals(0))
                    {
                        result.elements.Remove(key);
                    }
                }
                else
                {
                    result.elements.Add(key, p2.elements[key]);
                }
            }

            //sort ellements
            result.elements = result.elements.OrderByDescending(a => a.Key).ToDictionary(a => a.Key, a => a.Value);

            return result;
        }

        /// <summary>Multiplies two polynomials and returns the resulting polynomial.
        /// </summary>
        public static Polynome PMul(Polynome p1, Polynome p2)
        {
            Polynome result = new Polynome(true);

            for (int i = 0; i < p1.elements.Count; i++)
            {
                KeyValuePair<int, float> eOne = p1.elements.ElementAt(i);
                for (int j = 0; j < p2.elements.Count; j++)
                {
                    KeyValuePair<int, float> eTwo = p2.elements.ElementAt(j);
                    KeyValuePair<int, float> resultElement = new KeyValuePair<int, float>((eOne.Key + eTwo.Key),(eOne.Value * eTwo.Value));
                    if(result.elements.Keys.Contains(resultElement.Key))
                    {
                        //add the coeficients together
                        result.elements[resultElement.Key] += resultElement.Value;
                    }
                    else
                    {
                        result.elements.Add(resultElement.Key, resultElement.Value);
                    }
                }
            }
            result.elements = result.elements.OrderByDescending(a => a.Key).ToDictionary(a => a.Key, a => a.Value);
            return result;
        }

        /// <summary>Divides the first (numerator) polynomial by the second one(denominator) and returns result + remainder as polynomials.
        /// </summary>
        public static KeyValuePair<Polynome,Polynome> PDiv(Polynome p1, Polynome p2)
        {
            //In case the denominator polynomial har more ellements we simply return the fisrt as the remainder
            if(p2.elements.Count > p1.elements.Count)
            {
                return new KeyValuePair<Polynome, Polynome> (new Polynome(true), p1);
            }

            Polynome result = new Polynome(true);
            Polynome numerator = new Polynome(p1);      //above fraction line
            Polynome denominator = new Polynome(p2);    //below fraction line

            //we determine the initial amount of iterations of the long division algorith we pass trough
            int stepCount = numerator.elements.Count - 1;

            while (stepCount > 0)
            {
                //divide largest element of the numerator by the largets one of the denominator (determine multiplier)
                int key = numerator.elements.Keys.ElementAt(0) - denominator.elements.Keys.ElementAt(0);
                float val = numerator.elements.Values.ElementAt(0)/denominator.elements.Values.ElementAt(0);

                //if at this point the resulting exponent is 0 we know this pass is the only one we need
                if (numerator.elements.Keys.ElementAt(0) == 0)
                {
                    stepCount = 0;
                    break;
                }

                //We create the multiplier polynome with the key and value we determined above
                Polynome multiplier = new Polynome(true);
                multiplier.elements.Add(key, val);

                //At this point add the multiplier polynomial to the result
                result = PAdd(result, multiplier);

                //Create subtractor that we will subtract from the numerator
                Polynome subtractor = new Polynome(true);
                subtractor = PMul(multiplier, denominator);

                //We then subtract the subtractor from the numerator
                numerator = PSub(numerator, subtractor);

                //not quite sure why this is needed yet but to afraid to remove
                if (key == 0)
                {
                    stepCount = 0;
                    break;
                }

                stepCount --;
            }

            return new KeyValuePair<Polynome, Polynome>(result,numerator);
        }

        /// <summary>Evaluates the polynomial at the specified x value and returns the result as a string.
        /// </summary>
        public static string PValString(Polynome p1, float xValue)
        {
            float result = 0;

            for (int i = 0; i < p1.elements.Count; i++)
            {
                result += p1.elements.ElementAt(i).Value * Mathf.Pow(xValue, p1.elements.ElementAt(i).Key);
            }

            return result.ToString() + " for X=" + xValue;
        }

        /// <summary>Evaluates the polynomial at the specified x value and returns the result as a float.
        /// </summary>
        public static float PVal(Polynome p1, float xValue)
        {
            float result = 0;

            for (int i = 0; i < p1.elements.Count; i++)
            {
                result += p1.elements.ElementAt(i).Value * Mathf.Pow(xValue, p1.elements.ElementAt(i).Key);
            }

            return result;
        }
    }

    /// <summary>Complex operations with polynomials.
    /// <para>This class contains complex polynomial operations:.</para>
    /// <para>Derivation, Integration.
    ///</para>
    /// <seealso cref="Polynome.cs"/>
    /// <seealso cref="FuncS"/>
    /// </summary>
    public class FuncC
    {

        /// <summary>Derives a polynomial and returns the resulting polynomial.
        /// </summary>
        public static Polynome PDerive(Polynome p1)
        {
            Polynome result = new Polynome(true);

            for (int i = 0; i < p1.elements.Count; i++)
            {
                int deltaPower = p1.elements.Keys.ElementAt(i) - 1;
                float deltaValue = p1.elements.Keys.ElementAt(i) * p1.elements.Values.ElementAt(i);

                if(deltaPower >= 0 && deltaValue != 0)
                {
                    result.elements.Add(deltaPower, deltaValue);
                }
            }

            return result;
        }


        /// <summary>Integrates a polynomial and returns the resulting polynomial.
        /// </summary>
        public static Polynome PIntegrate(Polynome p1)
        {
            Polynome result = new Polynome(true);
            int denominator = p1.elements.ElementAt(0).Key;

            for (int i = 0; i < p1.elements.Count; i++)
            {
                result.elements.Add((p1.elements.ElementAt(i).Key + 1),(p1.elements.ElementAt(i).Value)/denominator);
            }

            return result;
        }
    }
}

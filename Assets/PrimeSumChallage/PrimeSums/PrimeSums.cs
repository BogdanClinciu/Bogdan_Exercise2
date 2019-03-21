using System;
using UnityEngine;

public class PrimeSums : MonoBehaviour
{
    [SerializeField]
    private int n = 1000;
    [SerializeField]
    private int maxRange = 10000;

    private bool[] prime;

    [ContextMenu("Find prime sum")]
    private void FindPrimeSum()
    {
        SieveOfEratosthenes(maxRange);
        Debug.Log("Sum of 1st N prime numbers is :" + PrimesSum(n));
    }

    private void SieveOfEratosthenes(int max)
    {
        prime = new bool[(max + 1)];

        for(int i = 0; i <= max; i++)
        {
            prime[i] = true ;
        }

        prime[1] = false;

        for (int i = 2; i * i <= max; i++)
        {
            // If curent prime is not changed, then it is a prime
            if (prime[i] == true)
            {
                // Set all multiples of curent number to non-prime
                for (int j = i * 2; j <= max; j += i)
                {
                    prime[j] = false;
                }
            }
        }
    }

    private int PrimesSum(int n)
    {
        // count of prime numbers
        int count = 0;
        int num = 1;
        int sum = 0;

        while (count < n)
        {
            if (prime[num])
            {
                sum += num;
                count++;
            }
            num++;
        }
        return sum;
    }
}

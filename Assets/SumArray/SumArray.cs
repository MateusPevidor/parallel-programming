using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumArray : MonoBehaviour
{

    public ComputeShader computeShader;

    private List<int> numbers;

    float startTime, endTime;

    void Start()
    {
        int numbersCount = (int) Mathf.Pow(2, 23); // 8.388.608
        int numThreads = 64 * 1 * 1;

        int[] result = new int[0];
        numbers = new List<int>();
        
        for (int i = 0; i < numbersCount; i++) {
            numbers.Add(Random.Range(0, 10));
        }

        startTime = Time.realtimeSinceStartup;

        while (numbersCount > 1) {
            ComputeBuffer numbersBuffer = new ComputeBuffer(numbersCount, sizeof(int));
            ComputeBuffer numbersBufferAux = new ComputeBuffer(numThreads, sizeof(int));

            numbersBuffer.SetData(numbers.ToArray());

            computeShader.SetFloat("k", numbersCount / numThreads);
            computeShader.SetBuffer(0, "numbers", numbersBuffer);
            computeShader.SetBuffer(0, "result", numbersBufferAux);
            computeShader.Dispatch(0, numThreads, 1, 1);

            result = new int[numThreads];
            numbersBufferAux.GetData(result);

            numbers = new List<int>(result);

            numbersCount = result.Length;
            numThreads /= 2;

            numbersBuffer.Dispose();
            numbersBufferAux.Dispose();
        }

        endTime = Time.realtimeSinceStartup;

        Debug.Log("Time: " + (endTime - startTime));
        Debug.Log("Result: " + result[0]);
    }
}

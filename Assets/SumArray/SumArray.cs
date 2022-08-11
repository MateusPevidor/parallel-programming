using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumArray : MonoBehaviour
{

    public ComputeShader computeShader;

    // public List<int> numbers;

    public float startTime, endTime;

    void Start()
    {
        // Sum(numbers, 4);
    }

    public int Sum(List<int> arr, int nThreads) {
        List<int> array = new List<int>(arr);

        int numbersCount = (int) Mathf.Pow(2, 23); // 8.388.608
        int numThreads = nThreads * 1 * 1;

        int[] result = new int[0];
        // array = new List<int>();
        
        // for (int i = 0; i < numbersCount; i++) {
        //     array.Add(Random.Range(0, 10));
        // }

        ComputeBuffer numbersBuffer = new ComputeBuffer(numbersCount, sizeof(int));
        ComputeBuffer numbersBufferAux = new ComputeBuffer(numThreads, sizeof(int));
        computeShader.SetBuffer(0, "result", numbersBufferAux);

        startTime = Time.realtimeSinceStartup;

        while (numbersCount > 1) {
            numbersBuffer.SetData(array.ToArray());

            computeShader.SetFloat("k", numbersCount / numThreads);
            computeShader.SetBuffer(0, "numbers", numbersBuffer);
            computeShader.Dispatch(0, numThreads, 1, 1);

            result = new int[numThreads];
            numbersBufferAux.GetData(result);

            array = new List<int>(result);

            numbersCount = result.Length;
            numThreads /= 2;
        }

        endTime = Time.realtimeSinceStartup;


        numbersBuffer.Dispose();
        numbersBufferAux.Dispose();

        // Debug.Log("[SumArr] Time: " + (endTime - startTime));
        // Debug.Log("Result: " + result[0]);

        return result[0];
    }
}

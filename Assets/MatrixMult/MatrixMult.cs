using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixMult : MonoBehaviour
{

    public ComputeShader computeShader;

    private List<int> A;
    private List<int> B;

    float startTime, endTime;

    void Start()
    {
        int size = (int) Mathf.Pow(2, 10); // 65.536
        int numThreads = 64 * 1 * 1;

        int[] result = new int[0];
        A = new List<int>();
        B = new List<int>();
        
        
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                A.Add(Random.Range(0, 10));
            }

            B.Add(Random.Range(0, 10));
        }

        startTime = Time.realtimeSinceStartup;

        int rowsByThread = size / numThreads;

        ComputeBuffer A_Buffer = new ComputeBuffer(size * size, sizeof(int));
        ComputeBuffer B_Buffer = new ComputeBuffer(size, sizeof(int));

        ComputeBuffer resultBuffer = new ComputeBuffer(size, sizeof(int));

        A_Buffer.SetData(A.ToArray());
        B_Buffer.SetData(B.ToArray());

        computeShader.SetFloat("rows", rowsByThread);
        computeShader.SetFloat("k", size);
        computeShader.SetBuffer(0, "A", A_Buffer);
        computeShader.SetBuffer(0, "B", B_Buffer);
        computeShader.SetBuffer(0, "result", resultBuffer);
        computeShader.Dispatch(0, numThreads, 1, 1);

        result = new int[size];
        resultBuffer.GetData(result);

        A_Buffer.Dispose();
        B_Buffer.Dispose();
        resultBuffer.Dispose();

        endTime = Time.realtimeSinceStartup;


        Debug.Log("Time: " + (endTime - startTime));
        for (int i = 0; i < result.Length; i++)
            Debug.Log("Result: " + result[i]);
    }
}

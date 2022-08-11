using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixMult : MonoBehaviour
{

    public ComputeShader computeShader;

    public float startTime, endTime;

    private List<int> _A, _B;

    public int size;
    public int nThreads;

    void Start()
    {
        SumArray sumArray = GetComponent<SumArray>();

        _A = new List<int>();
        _B = new List<int>();

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                _A.Add(j + i * size);
                // A.Add(Random.Range(0, 10));
            }

            _B.Add(i);
            // B.Add(Random.Range(0, 10));
        }

        startTime = Time.realtimeSinceStartup;

        List<int> _y = Mult(_A, _B);

        List<int> result = new List<int>();

        for (int i = 0; i < size; i++) {
            List<int> row = _y.GetRange(i * size, size);
            int r = sumArray.Sum(row, nThreads);
            result.Add(r);
        }

        endTime = Time.realtimeSinceStartup;
        Debug.Log("[MatMult] Time: " + (endTime - startTime));

    }

    public List<int> Mult(List<int> A, List<int> B) {
        int size = B.Count; // 65.536
        int numThreads = nThreads * 1 * 1;

        int[] result = new int[0];
        // A = new List<int>();
        // B = new List<int>();
        
        
        // for (int i = 0; i < size; i++) {
        //     for (int j = 0; j < size; j++) {
        //         A.Add(j + i * size);
        //         // A.Add(Random.Range(0, 10));
        //     }

        //     B.Add(i);
        //     // B.Add(Random.Range(0, 10));
        // }

        // startTime = Time.realtimeSinceStartup;

        int rowsByThread = size / numThreads;

        ComputeBuffer A_Buffer = new ComputeBuffer(size * size, sizeof(int));
        ComputeBuffer B_Buffer = new ComputeBuffer(size, sizeof(int));

        ComputeBuffer resultBuffer = new ComputeBuffer(size * size, sizeof(int));

        A_Buffer.SetData(A.ToArray());
        B_Buffer.SetData(B.ToArray());

        computeShader.SetFloat("rows", rowsByThread);
        computeShader.SetFloat("k", size);
        computeShader.SetBuffer(0, "A", A_Buffer);
        computeShader.SetBuffer(0, "B", B_Buffer);
        computeShader.SetBuffer(0, "result", resultBuffer);
        computeShader.Dispatch(0, numThreads, 1, 1);

        result = new int[size * size];
        resultBuffer.GetData(result);

        A_Buffer.Dispose();
        B_Buffer.Dispose();
        resultBuffer.Dispose();

        // endTime = Time.realtimeSinceStartup;


        // Debug.Log("[MatMult] Time: " + (endTime - startTime));
        // for (int i = 0; i < result.Length; i++)
            // Debug.Log("Result: " + result[i]);
        
        return new List<int>(result);
    }
}

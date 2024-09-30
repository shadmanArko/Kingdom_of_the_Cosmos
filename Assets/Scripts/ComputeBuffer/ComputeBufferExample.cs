using UnityEngine;

public class ComputeBufferExample : MonoBehaviour
{
    public ComputeShader computeShader;
    private int kernelHandle;
    private ComputeBuffer buffer;
    private const int SIZE = 1024000;

    void Start()
    {
        // Initialize data
        int[] data = new int[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            data[i] = i;
        }

        // Create compute buffer
        buffer = new ComputeBuffer(SIZE, sizeof(int));
        buffer.SetData(data);

        // Set up compute shader
        kernelHandle = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernelHandle, "result", buffer);

        // Run compute shader
        RunComputeShader();
    }

    void OnDestroy()
    {
        // Release the buffer
        if (buffer != null)
        {
            buffer.Release();
        }
    }

    [ContextMenu("Run compute buffer")]
    public void RunComputeShader()
    {
        // This method can be called from other scripts
        computeShader.Dispatch(kernelHandle, SIZE / 64, 1, 1);

        // Get and print results (you might want to modify this part based on your needs)
        int[] results = new int[SIZE];
        buffer.GetData(results);
        for (int i = 0; i < 10; i++)
        {
            Debug.Log($"Result[{i}] = {results[i]}");
        }
        Debug.Log($"size of array {results.Length}");

    }
}
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TensorFlowLite;
using System;

public class InGameCameraSegmentation : MonoBehaviour
{
    public Camera inGameCamera;
    public RenderTexture renderTexture;
    public RawImage outputImage;
    public RawImage inputImageUI; 
    public string modelFileName = "Models/RoadSegmentation.tflite";

    private Interpreter interpreter;
    private Texture2D cameraCaptureTexture;
    private Texture2D segmentedTexture;
    private Texture2D segmentedTestTexture;

    private int inputWidth = 128;
    private int inputHeight = 128;
    private int outputWidth = 128;
    private int outputHeight = 128;

    void Start()
    {
        string modelPath = Path.Combine(Application.streamingAssetsPath, modelFileName);
        byte[] modelData = FileUtil.LoadFile(modelPath);

        if (modelData == null || modelData.Length == 0)
        {
            Debug.LogError("Failed to load model data or model file is empty.");
            return;
        }
        
        Debug.Log("Model data successfully loaded. Size: " + modelData.Length + " bytes");

        InterpreterOptions options = new InterpreterOptions();
        interpreter = new Interpreter(modelData, options);
        interpreter.AllocateTensors();

        // Log tensor information for debugging
        var inputTensorInfo = interpreter.GetInputTensorInfo(0);
        var outputTensorInfo = interpreter.GetOutputTensorInfo(0);
        Debug.Log($"Input tensor shape: {string.Join(",", inputTensorInfo.shape)}");
        Debug.Log($"Output tensor shape: {string.Join(",", outputTensorInfo.shape)}");

        cameraCaptureTexture = new Texture2D(inputWidth, inputHeight, TextureFormat.RGB24, false);
        segmentedTexture = new Texture2D(outputWidth, outputHeight, TextureFormat.RGB24, false);
        segmentedTestTexture = new Texture2D(outputWidth, outputHeight, TextureFormat.RGB24, false);
    }

    void Update()
    {
        CaptureCameraImage();
        float[,,] inputTensor = PreprocessInput(cameraCaptureTexture);

        for (int y = 0; y < outputHeight; y++)
        {
            for (int x = 0; x < outputWidth; x++)
            {
                Color color = new Color(inputTensor[y, x, 0], inputTensor[y, x, 1], inputTensor[y, x, 2]);  // Grayscale intensity
                segmentedTestTexture.SetPixel(x, y, color);
            }
        }

        segmentedTestTexture.Apply();
        if (inputImageUI != null)
        {
            inputImageUI.texture = segmentedTestTexture;
        }
        
        // Reshape the input tensor to match the model's expected input shape
        float[] flattenedInput = new float[inputWidth * inputHeight * 3];
        for (int y = 0; y < inputHeight; y++)
        {
            for (int x = 0; x < inputWidth; x++)
            {
                for (int c = 0; c < 3; c++)
                {
                    flattenedInput[(y * inputWidth * 3) + (x * 3) + c] = inputTensor[y, x, c];
                }
            }
        }

        interpreter.SetInputTensorData(0, flattenedInput);
        interpreter.Invoke();

        // Get the output tensor data as a flattened array
        float[] flattenedOutput = new float[outputWidth * outputHeight];
        interpreter.GetOutputTensorData(0, flattenedOutput);

        // Reshape the output back to 2D
        float[,,] outputTensor = new float[outputHeight, outputWidth, 1];
        for (int y = 0; y < outputHeight; y++)
        {
            for (int x = 0; x < outputWidth; x++)
            {
                outputTensor[y, x, 0] = flattenedOutput[y * outputWidth + x];
            }
        }

        PostprocessOutput(outputTensor);
    }

    private void CaptureCameraImage()
    {
        if (renderTexture == null)
        {
            Debug.LogError("RenderTexture is not assigned.");
            return;
        }

        if (cameraCaptureTexture == null)
        {
            cameraCaptureTexture = new Texture2D(inputWidth, inputHeight, TextureFormat.RGB24, false);
        }

        inGameCamera.targetTexture = renderTexture;
        inGameCamera.Render();
        RenderTexture.active = renderTexture;

        cameraCaptureTexture = Resize(renderTexture, inputWidth, inputHeight);

        inGameCamera.targetTexture = null;
        RenderTexture.active = null;

    }

    private Texture2D Resize(RenderTexture renderTexture, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(renderTexture, rt);

        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        rt.Release();
        Destroy(rt);

        return result;
    }

    private float[,,] PreprocessInput(Texture2D inputImage)
    {
        // Create a float array to hold the normalized input image
        float[,,] inputTensor = new float[inputHeight, inputWidth, 3];

        // Resize and normalize the input image to match the TFLite model's input size
        Color[] pixels = inputImage.GetPixels();
        for (int y = 0; y < inputHeight; y++)
        {
            for (int x = 0; x < inputWidth; x++)
            {
                Color pixel = pixels[y * inputWidth + x];
                // Normalize pixel values to range 0-1
                inputTensor[y, x, 0] = pixel.r; // Red channel
                inputTensor[y, x, 1] = pixel.g; // Green channel
                inputTensor[y, x, 2] = pixel.b; // Blue channel
                Debug.Log($"{inputTensor[y, x, 0]}  {inputTensor[y, x, 1]}  {inputTensor[y, x, 2]}");
            }
        }

        // // Normalize the values (scale from 0-255 to 0-1)
        // for (int y = 0; y < inputHeight; y++)
        // {
        //     for (int x = 0; x < inputWidth; x++)
        //     {
        //         inputTensor[y, x, 0] /= 255f; // Normalize Red channel
        //         inputTensor[y, x, 1] /= 255f; // Normalize Green channel
        //         inputTensor[y, x, 2] /= 255f; // Normalize Blue channel
        //     }
        // }

        return inputTensor;
    }

    private void PostprocessOutput(float[,,] outputTensor)
    {
        for (int y = 0; y < outputHeight; y++)
        {
            for (int x = 0; x < outputWidth; x++)
            {
                // Apply sigmoid to each output value if it's not already a probability
                float intensity = outputTensor[y, x, 0];  // Value between 0 and 1
                Color color = new Color(intensity, intensity, intensity);  // Grayscale intensity
                segmentedTexture.SetPixel(x, y, color);
            }
        }

        segmentedTexture.Apply();
        outputImage.texture = segmentedTexture;
    }

    private float Sigmoid(float x)
    {
        return 1f / (1f + Mathf.Exp(-x));
    }

    private void OnDestroy()
    {
        interpreter?.Dispose();
        
        if (cameraCaptureTexture != null)
            Destroy(cameraCaptureTexture);
            
        if (segmentedTexture != null)
            Destroy(segmentedTexture);
    }
}
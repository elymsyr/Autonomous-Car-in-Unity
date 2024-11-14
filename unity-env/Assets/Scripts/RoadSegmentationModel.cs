using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;

public class InGameCameraSegmentation : MonoBehaviour
{
    public Camera inGameCamera;    // The in-game camera to capture the image
    public RenderTexture renderTexture; // A RenderTexture to render the camera output
    public RawImage outputImage;   // UI element to display the segmented output
    public NNModel onnxModel;      // The ONNX model for segmentation

    private IWorker worker;
    private Texture2D cameraCaptureTexture;
    private Texture2D segmentedTexture;

    private int inputWidth = 224;  // Adjust according to your ONNX model's input size
    private int inputHeight = 224; // Adjust according to your ONNX model's input size
    private int outputWidth = 224; // Adjust according to your ONNX model's output size
    private int outputHeight = 224; // Adjust according to your ONNX model's output size

    void Start()
    {
        // Initialize Barracuda with ONNX model
        var model = ModelLoader.Load(onnxModel);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);

        // Create textures for capturing the camera image and for the segmented output
        cameraCaptureTexture = new Texture2D(inputWidth, inputHeight, TextureFormat.RGB24, false);
        segmentedTexture = new Texture2D(outputWidth, outputHeight, TextureFormat.RGB24, false);
    }

    void Update()
    {
        // Capture an image from the in-game camera
        CaptureCameraImage();

        // Preprocess the captured image and run inference on the ONNX model
        Tensor inputTensor = PreprocessInput(cameraCaptureTexture);
        worker.Execute(inputTensor);
        Tensor outputTensor = worker.PeekOutput();

        // Postprocess the output and display the segmentation result
        PostprocessOutput(outputTensor);

        // Clean up tensors to prevent memory leaks
        inputTensor.Dispose();
        outputTensor.Dispose();
    }

    private void CaptureCameraImage()
    {
        // Set the target texture of the in-game camera to the renderTexture
        inGameCamera.targetTexture = renderTexture;
        inGameCamera.Render();

        // Read the pixels from the RenderTexture into the cameraCaptureTexture
        RenderTexture.active = renderTexture;
        cameraCaptureTexture.ReadPixels(new Rect(0, 0, inputWidth, inputHeight), 0, 0);
        cameraCaptureTexture.Apply();

        // Reset the target texture
        inGameCamera.targetTexture = null;
        RenderTexture.active = null;
    }

    private Tensor PreprocessInput(Texture2D inputImage)
    {
        // Create a tensor with the correct dimensions for the ONNX model input
        Tensor inputTensor = new Tensor(1, inputHeight, inputWidth, 3); // Assuming the input format is [1, H, W, C]

        // Resize and normalize the input image to match the ONNX model's input size
        Color[] pixels = inputImage.GetPixels();
        for (int y = 0; y < inputHeight; y++)
        {
            for (int x = 0; x < inputWidth; x++)
            {
                Color pixel = pixels[y * inputWidth + x];
                inputTensor[0, y, x, 0] = pixel.r; // Red channel
                inputTensor[0, y, x, 1] = pixel.g; // Green channel
                inputTensor[0, y, x, 2] = pixel.b; // Blue channel
            }
        }

        return inputTensor;
    }

    private void PostprocessOutput(Tensor outputTensor)
    {
        // Assuming the output is a segmentation mask with values between 0 and 1
        for (int y = 0; y < outputHeight; y++)
        {
            for (int x = 0; x < outputWidth; x++)
            {
                // Get the segmentation value for each pixel
                float segmentationValue = outputTensor[0, y, x, 0];

                // Colorize the output (for example, green for road, red for non-road)
                Color color = segmentationValue > 0.5f ? Color.green : Color.red;
                segmentedTexture.SetPixel(x, y, color);
            }
        }

        // Apply the changes to the texture and display it on the UI
        segmentedTexture.Apply();
        outputImage.texture = segmentedTexture;
    }

    private void OnDestroy()
    {
        // Clean up the Barracuda worker
        worker.Dispose();
    }
}

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UnitySocketClient : MonoBehaviour
{
    public Camera[] inGameCameras; // Array of cameras
    public RenderTexture renderTexture; // Shared render texture
    // public RawImage[] outputImages; // Array of output images for each camera
    private Texture2D cameraCaptureTexture;
    private int[] cameraPorts = { 3010, 3011 }; // Ports for each camera

    void Start()
    {
        cameraCaptureTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        for (int i = 0; i < inGameCameras.Length; i++)
        {
            int cameraIndex = i;
            StartCoroutine(SendImageToServer(cameraIndex));
        }
    }

    // Capture camera image and send it to Python for processing
    private IEnumerator SendImageToServer(int cameraIndex)
    {
        // Capture image from the camera
        CaptureCameraImage(cameraIndex);

        byte[] imageBytes = cameraCaptureTexture.EncodeToJPG(); // Convert image to byte array

        // Send image data to Python server via socket
        yield return StartCoroutine(SendDataToPythonServer(cameraIndex, imageBytes));

        // Receive processed data back from the server (segmentation output)
        byte[] receivedData = ReceiveDataFromServer(cameraIndex);
        if (receivedData != null)
        {
            // Process and display the received result (for example, a segmentation mask)
            Texture2D resultTexture = new Texture2D(128, 128);
            resultTexture.LoadImage(receivedData);  // Load the received image (if you send back an image)
            // outputImages[cameraIndex].texture = resultTexture;  // Display the result in the UI
        }
    }

    private void CaptureCameraImage(int cameraIndex)
    {
        // Capture the camera image into cameraCaptureTexture
        RenderTexture.active = renderTexture;
        inGameCameras[cameraIndex].targetTexture = renderTexture;
        inGameCameras[cameraIndex].Render();
        cameraCaptureTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        cameraCaptureTexture.Apply();
        RenderTexture.active = null;
    }

    private IEnumerator SendDataToPythonServer(int cameraIndex, byte[] imageData)
    {
        TcpClient client = new TcpClient("127.0.0.1", cameraPorts[cameraIndex]);
        NetworkStream stream = client.GetStream();

        // Send image data size first
        byte[] sizeData = BitConverter.GetBytes(imageData.Length);
        stream.Write(sizeData, 0, sizeData.Length);

        // Send the image data
        stream.Write(imageData, 0, imageData.Length);

        // Close the connection
        stream.Close();
        client.Close();

        yield return null;
    }

    private byte[] ReceiveDataFromServer(int cameraIndex)
    {
        byte[] receivedData = null;

        try
        {
            TcpClient client = new TcpClient("127.0.0.1", cameraPorts[cameraIndex]);
            NetworkStream stream = client.GetStream();

            // Read the incoming data (segmentation result) from Python
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                receivedData = ms.ToArray();
            }

            stream.Close();
            client.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data from server: " + e.Message);
        }

        return receivedData;
    }
}

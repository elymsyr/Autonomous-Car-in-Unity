using System;
using System.IO;
using System.Net.Sockets;
using System.Collections;
using UnityEngine;

public class CameraImageSender : MonoBehaviour
{
    public RenderTexture renderTexture;
    public int serverPort = 3010;
    public string serverIP = "127.0.0.1";

    private Texture2D cameraCaptureTexture;
    private TcpClient client;
    private NetworkStream stream;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(480, 480, 24);
            cam.targetTexture = renderTexture;
        }

        cameraCaptureTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        ConnectToServer();
        StartCoroutine(SendImageToServer());
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            Debug.Log("Connected to server on port " + serverPort);
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    IEnumerator SendImageToServer()
    {
        while (true)
        {
            CaptureCameraImage();
            byte[] imageBytes = cameraCaptureTexture.EncodeToJPG();
            SendDataToPythonServer(imageBytes);
            yield return new WaitForSeconds(0.1f);
        }
    }

    void CaptureCameraImage()
    {
        RenderTexture.active = renderTexture;
        cam.Render();

        cameraCaptureTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        cameraCaptureTexture.Apply();

        RenderTexture.active = null;
    }

    void SendDataToPythonServer(byte[] imageData)
    {
        if (client == null || !client.Connected)
        {
            Debug.LogError("Not connected to server.");
            return;
        }
        try
        {

            Debug.Log("Sending image data of size: " + imageData.Length);
            byte[] sizeData = BitConverter.GetBytes(imageData.Length);
            stream.Write(sizeData, 0, sizeData.Length);
            stream.Write(imageData, 0, imageData.Length);
            stream.Flush();
            Debug.Log("Image data sent to server.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending image data: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}

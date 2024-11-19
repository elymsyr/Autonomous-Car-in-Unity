using UnityEngine;
using TMPro;

public class InputDisplay : MonoBehaviour
{
    public InputManager inputManager;
    public CarController carController;
    private float barWidth = 100f;
    private Texture2D fillTexture;
    private Texture2D centerLineTexture;
    private int mode;
    public Transform rpmNeedle;
    public Transform kmhNeedle;
    public float NeedleRotationRPM = 126f;
    public float NeedleRotationKMH = 138f;
    public TextMeshProUGUI wheelInfoText1;
    public TextMeshProUGUI wheelInfoText2;
    void Start()
    {
        fillTexture = new Texture2D(1, 1);
        fillTexture.SetPixel(0, 0, Color.green);
        fillTexture.Apply();
        centerLineTexture = new Texture2D(1, 1);
        centerLineTexture.SetPixel(0, 0, Color.white);
        centerLineTexture.Apply();
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }

    void Update()
    {
        mode = inputManager.getMode;
        GaugeUpdate();
        UpdateWheelInfoDisplay();
    }

    void GaugeUpdate()
    {
        rpmNeedle.rotation = Quaternion.Euler(0, 0, (int)Mathf.Lerp(NeedleRotationRPM, -NeedleRotationRPM, carController.RPM / 7000));
        kmhNeedle.rotation = Quaternion.Euler(0, 0, (int)Mathf.Lerp(NeedleRotationKMH, -NeedleRotationKMH, carController.KMH / 120));
    }

    void OnGUI()
    {
        if (inputManager != null)
        {
            for (int i = 0; i < Display.displays.Length; i++)
            {
                RenderBarsOnDisplay(i);
            }

            RenderModeLetters();
        }
        else
        {
            GUI.Label(new Rect(10, 10, 300, 40), "InputManager not found!", GUIStyle.none);
        }
    }

    void RenderBarsOnDisplay(int displayIndex)
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;

        int screenWidth = Display.displays[displayIndex].systemWidth;
        int screenHeight = Display.displays[displayIndex].systemHeight;

        DrawBarBorder(new Rect(10, 10, barWidth, 20));

       
        float throttleFillWidth = inputManager.throttle * barWidth;
        GUI.DrawTexture(new Rect(10, 10, throttleFillWidth, 20), fillTexture);

        DrawBarBorder(new Rect(10, 90, barWidth, 20));

        float brakeFillWidth = inputManager.brake * barWidth;
        GUI.DrawTexture(new Rect(10, 90, brakeFillWidth, 20), fillTexture);

        DrawBarBorder(new Rect(10, 130, barWidth, 20));

        float handbrakeFillWidth = inputManager.handbrake * barWidth;
        GUI.DrawTexture(new Rect(10, 130, handbrakeFillWidth, 20), fillTexture);

        DrawBarBorder(new Rect(10, 50, barWidth, 20));
       
        float centerX = 10 + barWidth / 2;
        GUI.DrawTexture(new Rect(centerX, 50, 2, 20), centerLineTexture);

        if (inputManager.steer < 0)
        {
            float steerFillWidth = (-inputManager.steer) * (barWidth / 2);
            GUI.DrawTexture(new Rect(centerX - steerFillWidth, 50, steerFillWidth, 20), fillTexture);
        }

        if (inputManager.steer > 0)
        {
            float steerFillWidth = inputManager.steer * (barWidth / 2);
            GUI.DrawTexture(new Rect(centerX, 50, steerFillWidth, 20), fillTexture);
        }
    }

   
    void DrawBarBorder(Rect position)
    {
        GUI.Box(position, "", new GUIStyle
        {
            normal = { background = Texture2D.whiteTexture },
            border = new RectOffset(2, 2, 2, 2),
            padding = new RectOffset(2, 2, 2, 2)
        });
    }

   
    void RenderModeLetters()
    {
        GUIStyle letterStyle = new GUIStyle();
        letterStyle.fontSize = 40;
        letterStyle.normal.textColor = Color.white;

        Color rColor = (mode == -1) ? Color.green : Color.white;
        Color nColor = (mode == 0) ? Color.green : Color.white;
        Color aColor = (mode == 1) ? Color.green : Color.white;

        GUI.contentColor = rColor;
        GUI.Label(new Rect(10, 190, 50, 40), "R", letterStyle);

        GUI.contentColor = nColor;
        GUI.Label(new Rect(60, 190, 50, 40), "N", letterStyle);

        GUI.contentColor = aColor;
        GUI.Label(new Rect(110, 190, 50, 40), "A", letterStyle);
    }

    void UpdateWheelInfoDisplay()
    {
        if (wheelInfoText1 != null & wheelInfoText2 != null)
        {
            string wheelInfo1 = $"FR Wheel:\n   RPM = {carController.colliders.FRWheel.rpm:F2}\n   Brake Torque = {carController.colliders.FRWheel.brakeTorque:F2}\n" +
                               $"FL Wheel:\n   RPM = {carController.colliders.FLWheel.rpm:F2}\n   Brake Torque = {carController.colliders.FLWheel.brakeTorque:F2}";
            string wheelInfo2 = $"RR Wheel:\n   RPM = {carController.colliders.RRWheel.rpm:F2}\n   Brake Torque = {carController.colliders.RRWheel.brakeTorque:F2}\n" +
                               $"RL Wheel:\n   RPM = {carController.colliders.RLWheel.rpm:F2}\n   Brake Torque = {carController.colliders.RLWheel.brakeTorque:F2}";
            // Forward Friction = {carController.colliders.FRWheel.forwardFriction:F2}\n   Sideway Friction = {carController.colliders.FRWheel.sidewaysFriction:F2}
            // Forward Friction = {carController.colliders.FLWheel.forwardFriction:F2}\n   Sideway Friction = {carController.colliders.FLWheel.sidewaysFriction:F2}
            // Forward Friction = {carController.colliders.RRWheel.forwardFriction:F2}\n   Sideway Friction = {carController.colliders.RRWheel.sidewaysFriction:F2}
            // Forward Friction = {carController.colliders.RLWheel.forwardFriction:F2}\n   Sideway Friction = {carController.colliders.RLWheel.sidewaysFriction:F2}
            wheelInfoText1.text = wheelInfo1;
            wheelInfoText2.text = wheelInfo2;
        }
        else
        {
            Debug.LogWarning("WheelInfoText reference is not assigned in the Inspector!");
        }
    }
}

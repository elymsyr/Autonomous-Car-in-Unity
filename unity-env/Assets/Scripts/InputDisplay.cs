using UnityEngine;

public class InputDisplay : MonoBehaviour
{
    public InputManager inputManager; // Reference to the InputManager script
    private float throttleBarWidth = 200f; // Max width of the throttle bar
    private float steerBarWidth = 200f;    // Max width of the steer bar
    private Texture2D fillTexture; // Texture for the filled bar
    private Texture2D centerLineTexture; // Texture for the center line
    private int mode;
    // Start is called before the first frame update
    void Start()
    {
        // Create a solid color texture for the filled bars
        fillTexture = new Texture2D(1, 1);
        fillTexture.SetPixel(0, 0, Color.green); // Set the bar color to green
        fillTexture.Apply();

        // Create a texture for the center line (set to white or any color you prefer)
        centerLineTexture = new Texture2D(1, 1);
        centerLineTexture.SetPixel(0, 0, Color.white); // Set the center line color
        centerLineTexture.Apply();

        // Activate all connected displays
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        mode = inputManager.getMode;
        // Ensure inputManager is assigned, or try to find it automatically
        // if (inputManager == null)
        // {
        //     inputManager = Object.FindAnyObjectByType<InputManager>();
        // }

    }

    // OnGUI is called for rendering and handling GUI events
    void OnGUI()
    {
        if (inputManager != null)
        {
            // Render the bars on all active displays
            for (int i = 0; i < Display.displays.Length; i++)
            {
                RenderBarsOnDisplay(i);
            }

            // Render the mode letters
            RenderModeLetters();
        }
        else
        {
            // If inputManager is not found, display an error message
            GUI.Label(new Rect(10, 10, 300, 40), "InputManager not found!", GUIStyle.none);
        }
    }

    void RenderBarsOnDisplay(int displayIndex)
    {
        // Set up the style for the text
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;

        // Calculate the display offset (assuming all displays have the same size)
        int screenWidth = Display.displays[displayIndex].systemWidth;
        int screenHeight = Display.displays[displayIndex].systemHeight;

        // Draw the throttle bar background (border)
        DrawBarBorder(new Rect(10, 10, throttleBarWidth, 30));

        // Draw the actual throttle bar based on the throttle value
        float throttleFillWidth = inputManager.throttle * throttleBarWidth; // 0 to 1 fill
        GUI.DrawTexture(new Rect(10, 10, throttleFillWidth, 30), fillTexture); // Filled bar

        // Draw the steer bar background (border)
        DrawBarBorder(new Rect(10, 50, steerBarWidth, 30));

        // Draw the center line of the steer bar (0 point)
        float centerX = 10 + steerBarWidth / 2;
        GUI.DrawTexture(new Rect(centerX, 50, 2, 30), centerLineTexture); // Center line

        // Handle negative steer (filling from center to the left)
        if (inputManager.steer < 0)
        {
            float steerFillWidth = (-inputManager.steer) * (steerBarWidth / 2); // -1 to 0 fill to left
            GUI.DrawTexture(new Rect(centerX - steerFillWidth, 50, steerFillWidth, 30), fillTexture);
        }

        // Handle positive steer (filling from center to the right)
        if (inputManager.steer > 0)
        {
            float steerFillWidth = inputManager.steer * (steerBarWidth / 2); // 0 to 1 fill to right
            GUI.DrawTexture(new Rect(centerX, 50, steerFillWidth, 30), fillTexture);
        }
    }

    // Draw a border for the bar
    void DrawBarBorder(Rect position)
    {
        // Draw the border using a GUI box with no background, just a border
        GUI.Box(position, "", new GUIStyle
        {
            normal = { background = Texture2D.whiteTexture }, // This makes it visible and styled
            border = new RectOffset(2, 2, 2, 2), // Border thickness
            padding = new RectOffset(2, 2, 2, 2) // Padding to create space for the fill
        });
    }

    // Render the R, N, A mode letters
    void RenderModeLetters()
    {
        // Set up the style for the letters
        GUIStyle letterStyle = new GUIStyle();
        letterStyle.fontSize = 40;
        letterStyle.normal.textColor = Color.white;

        // Draw the 'R', 'N', 'A' letters with color based on the mode
        Color rColor = (mode == -1) ? Color.green : Color.white;
        Color nColor = (mode == 0) ? Color.green : Color.white;
        Color aColor = (mode == 1) ? Color.green : Color.white;

        // Draw the letters at the top of the screen
        GUI.contentColor = rColor;
        GUI.Label(new Rect(10, 100, 50, 40), "R", letterStyle);

        GUI.contentColor = nColor;
        GUI.Label(new Rect(60, 100, 50, 40), "N", letterStyle);

        GUI.contentColor = aColor;
        GUI.Label(new Rect(110, 100, 50, 40), "A", letterStyle);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlBar : MonoBehaviour
{
    public Image MyImage;
    public RectTransform MyRectTransform;
    public GameManager GameManagerRef;
    
    // Color transitioning
    public Color CurrentColor;
    public Color DesiredColor;
    public float ColorTransitionTimer;
    public ControlState PrevControlState;
    public Dictionary<ControlState, Color> ColorDictionary;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManagerRef = GameManager.GetReference();
        MyImage = GetComponent<Image>();
        MyRectTransform = GetComponent<RectTransform>();
        CurrentColor = Color.green;
        MyImage.color = CurrentColor;
        
        ColorDictionary = new Dictionary<ControlState, Color>();
        ColorDictionary.Add(ControlState.Stable, Color.green);
        ColorDictionary.Add(ControlState.Unstable, Color.yellow);
        ColorDictionary.Add(ControlState.Extreme, new Color(1f,0.5f,0f)); // orange
        ColorDictionary.Add(ControlState.Critical, Color.red);
        ColorDictionary.Add(ControlState.OutOfControl, Color.black);
    }

    // Update is called once per frame
    void Update()
    {
        ControlState currentState = GameManagerRef.GetCurrentControlState();
        
        // This is how we detect if we hit a new state.
        if (currentState != PrevControlState)
        {
            ColorTransitionTimer = 1f;
            DesiredColor = ColorDictionary[currentState];
        }
        
        // Lerp when transitioning
        if (ColorTransitionTimer > 0f)
        {
            MyImage.color = Color.Lerp(DesiredColor, CurrentColor, ColorTransitionTimer);
            ColorTransitionTimer -= Time.deltaTime;
            if (ColorTransitionTimer <= 0f)
            {
                ColorTransitionTimer = 0f;
                CurrentColor = DesiredColor;
            }
        }
        
        // Want to set this at the end of all logic for this frame.
        PrevControlState = currentState;
    }

    /// <summary>
    /// Adjusts the size of bar.
    /// </summary>
    public void UpdateSize(float control)
    {
        Vector2 size = MyRectTransform.sizeDelta;
        size.x = 500f * (control / 100f);
        MyRectTransform.sizeDelta = size;
    }
}

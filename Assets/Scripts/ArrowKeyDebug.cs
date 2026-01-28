using UnityEngine;
using UnityEngine.InputSystem;

public class ArrowKeyDebug : MonoBehaviour
{
    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.upArrowKey.wasPressedThisFrame)
        {
            Debug.Log("UpArrow 눌림");
        }
        if (keyboard.downArrowKey.wasPressedThisFrame)
        {
            Debug.Log("DownArrow 눌림");
        }
        if (keyboard.leftArrowKey.wasPressedThisFrame)
        {
            Debug.Log("LeftArrow 눌림");
        }
        if (keyboard.rightArrowKey.wasPressedThisFrame)
        {
            Debug.Log("RightArrow 눌림");
        }
    }
}



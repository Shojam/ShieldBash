using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour {

    public int GamepadCount = 4; // Number of gamepads to support

    private List<Controller> gamepads;     // Holds gamepad instances
    private static ControllerManager singleton; // Singleton instance
    public static ControllerManager Instance
    {
        get
        {
            if (singleton == null)
            {
                Debug.LogError("[GamepadManager]: Instance does not exist!");
                return null;
            }

            return singleton;
        }
    }

    // Initialize on 'Awake'
    void Awake()
    {
        // Found a duplicate instance of this class, destroy it!
        if (singleton != null && singleton != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            // Create singleton instance
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }

        GamepadCount = Mathf.Clamp(GamepadCount, 1, 4);

        gamepads = new List<Controller>();

        // Create specified number of gamepad instances
        for (int i = 0; i < GamepadCount; ++i)
            gamepads.Add(new Controller(i + 1));
    }

    // Normal Unity update
    void Update()
    {
        for (int i = 0; i < gamepads.Count; ++i)
            gamepads[i].Update();
    }

    // Refresh gamepad states for next update
    public void Refresh()
    {
        for (int i = 0; i < gamepads.Count; ++i)
            gamepads[i].Refresh();
    }

    public Controller GetGamepad(int index)
    {
        for (int i = 0; i < gamepads.Count;)
        {
            // Indexes match, return this gamepad
            if (gamepads[i].Index == (index - 1))
                return gamepads[i];
            else
                ++i;
        }

        Debug.LogError("[GamepadManager]: " + index + " is not a valid gamepad index!");

        return null;
    }

    public int ConnectedTotal()
    {
        int total = 0;

        for (int i = 0; i < gamepads.Count; ++i)
        {
            if (gamepads[i].IsConnected)
                total++;
        }

        return total;
    }
}

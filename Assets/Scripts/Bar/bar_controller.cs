using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class bar_controller : MonoBehaviour
{
    public Camera bar_camera;

    public static bar_controller Instance { get; private set; }
    


    public bool player_in_bar = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    void Update()
    {
        if (player_in_bar && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            exit_bar();
        }

        if (player_in_bar && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
          select_drink();
        }
    }

    void start_bar() {
        player_in_bar = true;
    }

    void exit_bar() {
        player_in_bar = false;
    }

    void select_drink() {
        Ray ray = bar_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            bottle_script clickedbottle = hit.collider.GetComponent<bottle_script>();

            Debug.Log(clickedbottle);
            if (clickedbottle != null)
            {
                Debug.Log(clickedbottle.bottle_data.price);
            }
        }
    }
}

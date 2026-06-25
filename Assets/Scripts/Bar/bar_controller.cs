using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class bar_controller : MonoBehaviour
{
    public Camera bar_camera;

    //UI
    public GameObject bar_dialogue_object;
    public TMP_Text nameText;
    public TMP_Text descText;
    public TMP_Text priceText;

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

    public void start_bar() {
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
                display_drink_info(clickedbottle.bottle_data);
            }
        }
    }

    void display_drink_info(bottle_SO data) {
        bar_dialogue_object.SetActive(true);

        nameText.text = data.name;
        descText.text = data.description;

        priceText.text = data.price.ToString();
    }
}

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

        if (player_in_bar && Mouse.current != null)
        {
            hover_drink();
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
        bar_dialogue_object.SetActive(false);
    }

    void hover_drink() {
        Ray ray = bar_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            bottle_script hoveredbottle = hit.collider.GetComponent<bottle_script>();

            if (hoveredbottle != null)
            {
                display_drink_info(hoveredbottle.bottle_data);
            }
            
            else
            {
                bar_dialogue_object.SetActive(false);
            }
        }
        else
        {
            bar_dialogue_object.SetActive(false);
        }
    }

    void display_drink_info(bottle_SO data) {
        bar_dialogue_object.SetActive(true);

        nameText.text = data.name;
        descText.text = data.description;

        priceText.text = data.price.ToString();
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
                if(clickedbottle.bottle_data.buff == Buff.luck) {
                    BuffManager.Instance.luckTier = clickedbottle.bottle_data.buff_tier;
                    Debug.Log("changing luck buff tier");
                }
                else
                    BuffManager.Instance.cashoutTier = clickedbottle.bottle_data.buff_tier;

                clickedbottle.gameObject.SetActive(false);
                GameManager.Instance.current_money -= clickedbottle.bottle_data.price;
                
                
            }

        }

    } 
}
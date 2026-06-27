using UnityEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class pause_script : MonoBehaviour
{
    public Movement player_settings;
    public GameObject canvas;

    private bool game_paused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (!game_paused) {
                canvas.SetActive(true);
                player_settings.SetInputEnabled(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
            
        
    }

    public void return_title() {
        SceneManager.LoadScene("title_screen"); //main game
    }

    public void back_to_game() {
        canvas.SetActive(false);
        player_settings.SetInputEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnSliderChanged(float value)
    {
        player_settings.mouseSensitivity = value;
    }

    public void OnVolumeChanged(float value) {
        AudioListener.volume = value;
    }
}

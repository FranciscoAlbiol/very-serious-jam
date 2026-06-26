using UnityEngine;
using UnityEngine.SceneManagement;

public class transition : MonoBehaviour
{
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void turnOff(){
        animator.SetTrigger("buh");
    }

    public void menu(){
        SceneManager.LoadScene("Main Scene");
    }

    public void restart(){
        SceneManager.LoadScene("Intro");
    }

    public void kill(){
        Application.Quit();
    }

    public void end(){
        SceneManager.LoadScene("Ending");
    }
}

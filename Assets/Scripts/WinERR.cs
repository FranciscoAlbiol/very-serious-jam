using UnityEngine;

public class WinERR : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Sprite sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void crash(){
        renderer.sprite = sprite;
    }
}

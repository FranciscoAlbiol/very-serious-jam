using UnityEngine;

public class card_view : MonoBehaviour
{   
    public quatro_card_SO card_data;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(quatro_card_SO data)
    {
        card_data = data;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && card_data != null)
        {
            spriteRenderer.sprite = card_data.card_sprite;
        }
        
    }
}

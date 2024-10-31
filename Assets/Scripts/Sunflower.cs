using UnityEngine;

public class GetSunflower : MonoBehaviour
{
    [SerializeField]

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerController>();
        if (player is not null)
        {
             Destroy(gameObject);
           
        }
    }
}
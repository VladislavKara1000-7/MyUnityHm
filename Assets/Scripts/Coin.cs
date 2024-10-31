
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerController>();

        if (player is not null)
        {
            player.AddCoins(coinValue);

            Destroy(gameObject);
        }
    }
}
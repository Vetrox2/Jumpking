using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameController controller;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
            controller.Finished();
    }
}

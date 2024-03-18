using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField]
    private GameController controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
            controller.Finished();
    }
}

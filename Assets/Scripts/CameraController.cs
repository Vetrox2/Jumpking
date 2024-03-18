using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float PosX;
    [SerializeField]
    private float AddToPosY;

    public void InitializeCameraPosition()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            transform.position = new Vector2(PosX, player.transform.position.y + AddToPosY);
        }
    }
}

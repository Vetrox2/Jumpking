using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float PosX = 1.5f;
    public float AddToPosY = 1.5f;

    public void InitializeCameraPosition()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            transform.position = new Vector2(PosX, player.transform.position.y + AddToPosY);
        }
    }
}

using UnityEngine;

public class CookieAnimation : MonoBehaviour
{
    public Vector2 direction;
    public float scrollSpeed = 5f;
    public float cookieSize = 100f;

    private float moved;
    
    void Update()
    {
        
        float move = scrollSpeed * Time.deltaTime;
        transform.Translate(direction * move);
        if ((moved += move) > cookieSize)
        {
            moved -= cookieSize;
            transform.Translate(direction * -cookieSize);
        }
    }


    private void OnValidate()
    {
        direction.Normalize();
    }
}

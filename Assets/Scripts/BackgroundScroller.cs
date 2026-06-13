using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float _xVelocity, _yVelocity;

    void Update()
    {
        // Continuously shift the UV Rect coordinates over time
        _img.uvRect = new Rect(
            _img.uvRect.position + new Vector2(_xVelocity, _yVelocity) * Time.deltaTime, 
            _img.uvRect.size
        );
    }
}

using UnityEngine;
using TMPro;

public class BlinkText : MonoBehaviour
{
    [SerializeField] private float blinkSpeed = 0.8f;
    private TextMeshProUGUI text;
    private float timer;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= blinkSpeed)
        {
            timer = 0f;
            text.enabled = !text.enabled; // parpadea
        }
    }
}

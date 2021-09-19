using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class StringModifier:MonoBehaviour
{
    TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void Modify(string message)
    {
        text.text = message;
    }
}
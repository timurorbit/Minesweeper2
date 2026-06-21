using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnGUI();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject a = new GameObject();
    }

    void OnGUI()
    {
        Debug.Log("Hello World");
    }
}

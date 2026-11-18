using UnityEngine;

public class DollGameObjects : MonoBehaviour
{
    [SerializeField] private GameObject HeartObject;
    [SerializeField] private GameObject CogWheelObject ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HeartObject.SetActive(false);
        CogWheelObject.SetActive(false);
    }

    // Update is called once per frame
    public void ShowObjects()
    {
        HeartObject.transform.parent.SetParent(null);
        CogWheelObject.transform.SetParent(null);
        HeartObject.SetActive(true);
        CogWheelObject.SetActive(true);
    }
}

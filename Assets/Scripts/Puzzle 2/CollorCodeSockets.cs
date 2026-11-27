using UnityEngine;

public class CollorCodeSockets : MonoBehaviour
{
    public string designatedCollor;
    public bool CollorMatch = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CodeDoll") != null)
        {
            if (other.gameObject.GetComponent<DollDataChecker>().dollData.collorData == designatedCollor)
            {
                CollorMatch = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<DollDataChecker>() != null)
        {
            if (other.gameObject.GetComponent<DollDataChecker>().dollData.collorData == designatedCollor)
            {
                CollorMatch = false;
            }
        }
    }
}

using UnityEngine;

public class LidScript : MonoBehaviour
{
    private int screwsDone = 0;

    private bool openLit = false;

    public void ScrewDone()
    {
        screwsDone += 1;

        if(screwsDone >= 1)
        {
            openLit = true;
        }
    }

    void Update()
    {
        if(openLit)
        {
            GetComponentInParent<Animator>().SetTrigger("OpenLit");
        }
    }
}

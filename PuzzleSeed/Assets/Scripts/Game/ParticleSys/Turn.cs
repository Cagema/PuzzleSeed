using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour
{
    void Start()
    {
        Invoke("DestroyTurnPart", 3f);
    }

    void DestroyTurnPart()
    {
        Destroy(this.gameObject);
    }
}

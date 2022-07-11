using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public static Target Instance;
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;

    [SerializeField] GameObject finishAreaPartPrefab;
    [SerializeField] Transform endTransform;
     private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject _go = Instantiate(finishAreaPartPrefab);
            _go.transform.position = endTransform.position;
            endTransform.position = new Vector3(0, 0, endTransform.position.z + 1);
        }
    }
}

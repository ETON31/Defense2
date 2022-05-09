using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    private float rotSpeed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {          
            if (gameObject.name == "Money(Clone)") 
            {
                GameManager.instance.AddCash(100);
            }
            if (gameObject.name == "Health(Clone)")
            {
                PlayerHealth.instance.RestoreHealth(30f);
            }

            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Market : MonoBehaviour
{
    public GameObject[] products;
    GameObject text;
    Text text0;
    Color notAllowed = new Color(255f, 0f, 0f, 255f);
    Color allowed = new Color(0f, 0f, 0f, 255f);
    public GameObject gun;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckAllowed(0, 100);
        CheckAllowed(1, 200);
    }

    private void CheckAllowed(int i, int cash)
    {
        if (GameManager.cash < cash)
        {
            products[i].GetComponent<Button>().interactable = false;
            text = products[i].transform.Find("Text").gameObject;
            text0 = text.GetComponent<Text>();
            text0.color = notAllowed;
        } 
        else
        {
            products[i].GetComponent<Button>().interactable = true;
            text = products[i].transform.Find("Text").gameObject;
            text0 = text.GetComponent<Text>();
            text0.color = allowed;
        }
    }

    public void Dummy1()
    {
        GameManager.instance.BuyProduct(100);
        gun.GetComponent<Gun>().AddAmmo(10);
    }

    public void Dummy2()
    {
        GameManager.instance.BuyProduct(200);
        PlayerHealth.instance.RestoreHealth(10f);
    }
}

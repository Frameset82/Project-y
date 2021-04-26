using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxOpen : MonoBehaviour
{


    public bool isPlayerEnter;
    public Animator anim;
    public GameObject item = null;
    public GameObject spawnPoint = null;
    bool isClosed;
    bool isGained;
    public Text gameText = null;



    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {

        if (isPlayerEnter && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("누름");
            isClosed = false;
            anim.SetTrigger("Open");
            Invoke("OpenBox", 1f);

        }

    }

    private void Awake()
    {
        isPlayerEnter = false;


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerEnter = true;
            gameText.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerEnter = false;
        }
    }
    void OpenBox()
    {
        if (spawnPoint != null)
            spawnPoint = Instantiate(item, spawnPoint.transform.position, item.transform.rotation);

        gameText.text = "Press E button to add the Item ! ! ! ";
        gameText.enabled = true;
    }
}

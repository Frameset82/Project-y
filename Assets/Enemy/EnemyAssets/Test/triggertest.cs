using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggertest : MonoBehaviour
{
    GameObject nearObject;

    bool iDown;
    bool gDown;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;
    bool sDown1;
    bool sDown2;
    int equipGrenadeIndex;
    public Camera followCamera;

    GameObject equipWeapon;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Grenade")
        {
            nearObject = other.gameObject;
            Debug.Log("1");
        }


    }

  
    void Interation()
    {
        if(iDown && nearObject !=null)
        {
            if(nearObject.tag =="Grenade") // 수류탄이면 
            {
                if (nearObject.tag == "Grenade")
                {
                    hasGrenades++;
                    Item item = nearObject.GetComponent<Item>();
                    int GrenadeIndex = item.value; // 수류탄 value == 0
                    hasWeapons[GrenadeIndex] = true;

                    Destroy(nearObject);

                }
            }
        }   
    }

    void GetInput()
    {
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        gDown = Input.GetButtonDown("Fire2");
    }

    private void Update()
    {
        GetInput();
        Interation();
        Swap();
        Grenade();
    }

    void Swap()
    {

        if (sDown1 && (!hasWeapons[0] || equipGrenadeIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipGrenadeIndex == 1))
            return;


        int GrenadeIndex = -1;
        if (sDown1) GrenadeIndex = 0;
        if (sDown2) GrenadeIndex = 1;
        if(sDown1 || sDown2)
        {
            if (equipWeapon != null)
                equipWeapon.SetActive(false);

            equipGrenadeIndex = GrenadeIndex;
            equipWeapon = weapons[GrenadeIndex];
            equipWeapon.SetActive(true);
  
        }
    }
  void Grenade()
    {
        if (hasGrenades == 0)
            return;
        if(gDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;
            if(Physics.Raycast(ray,out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 20;
                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                /*grenades[hasGrenades].SetActive(false);*/

            }
        }
    }


}

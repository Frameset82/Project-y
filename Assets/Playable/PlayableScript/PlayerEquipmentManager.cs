using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipmentManager : MonoBehaviour
{
    private string weaponRoot = "Player/Male/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/";
    public GameObject mainWeapon = null; // 1번무기
    public GameObject subWeapon = null; // 2번무기
    private int equipCount = 1; // main = 1 sub = 2
    public GameObject equipWeapon;
    private GameObject nearObject;
    private string weaponName;

    private GameObject changeImg;
    public Image mainWeaponImg;
    public Image subWeaponImg;
    public PlayerAnimation playerAnimation;

    public GameObject changeEquipment; // Panel
    public Image changeImg1;
    public Image changeImg2;

    public GameObject dropWeapon1; // 무기 두칸이 다 차있을경우 떨어트릴 무기
    public GameObject dropWeapon2;

    public Weapon weapon;
    void Interation()
    {
        if (Input.GetButtonDown("Interation") && nearObject != null)
        {
            if (nearObject.tag == "Weapon")
            {
                if (mainWeapon == null)
                {
                    dropWeapon1 = nearObject;
                    weaponName = nearObject.name;
                    mainWeapon = GameObject.Find(weaponRoot + weaponName);
                    mainWeapon.SetActive(true);
                    equipWeapon = mainWeapon;
                    changeImg = GameObject.Find(weaponRoot + weaponName + "/" + weaponName);
                    mainWeaponImg.sprite = changeImg.GetComponent<Image>().sprite;
                    changeImg.SetActive(false);
                    if (subWeapon != null)
                        subWeapon.SetActive(false);
                    equipCount = 1;
                    nearObject.SetActive(false);
                    nearObject = null;
                }
                else if (subWeapon == null)
                {
                    dropWeapon2 = nearObject;
                    weaponName = nearObject.name;
                    subWeapon = GameObject.Find(weaponRoot + weaponName);
                    subWeapon.SetActive(true);
                    equipWeapon = subWeapon;
                    changeImg = GameObject.Find(weaponRoot + weaponName + "/" + weaponName);
                    subWeaponImg.sprite = changeImg.GetComponent<Image>().sprite;
                    changeImg.SetActive(false);
                    if (mainWeapon != null)
                        mainWeapon.SetActive(false);
                    equipCount = 2;
                    nearObject.SetActive(false);
                    nearObject = null;
                }
                else if (mainWeapon != null && subWeapon != null) // 무기 둘다 가지고 있을 시 교체기능
                {
                    changeEquipment.SetActive(true);
                    changeImg1.sprite = mainWeaponImg.GetComponent<Image>().sprite;
                    changeImg2.sprite = subWeaponImg.GetComponent<Image>().sprite;
                }
            }
        }
    }

    void Swap()
    {
        if (Input.GetButtonDown("Swap1") && (mainWeapon == null || equipCount == 1))
            return;
        if (Input.GetButtonDown("Swap2") && (subWeapon == null || equipCount == 2))
            return;

        if (Input.GetButtonDown("Swap1") || Input.GetButtonDown("Swap2"))
        {
            if (equipCount == 1)
            {
                playerAnimation.Swap();
                mainWeapon.SetActive(false);
                subWeapon.SetActive(true);
                equipWeapon = subWeapon;
                equipCount = 2;
            }
            else if (equipCount == 2)
            {
                playerAnimation.Swap();
                subWeapon.SetActive(false);
                mainWeapon.SetActive(true);
                equipWeapon = mainWeapon;
                equipCount = 1;
            }
        }
    }

    public void ChangeMainWeapon()
    {
        GameObject dummyWeapon = (GameObject)Instantiate(dropWeapon1, gameObject.transform.position, gameObject.transform.rotation);
        dummyWeapon.name = dropWeapon1.name;
        dummyWeapon.SetActive(true);
        dropWeapon1 = nearObject;
        mainWeapon.SetActive(false);
        weaponName = nearObject.name;
        mainWeapon = GameObject.Find(weaponRoot + weaponName);
        mainWeapon.SetActive(true);
        equipWeapon = mainWeapon;
        changeImg = GameObject.Find(weaponRoot + weaponName + "/" + weaponName);
        mainWeaponImg.sprite = changeImg.GetComponent<Image>().sprite;
        changeImg.SetActive(false);
        if (subWeapon != null)
            subWeapon.SetActive(false);
        equipCount = 1;
        nearObject.SetActive(false);
        nearObject = null;
        changeEquipment.SetActive(false);
    }

    public void ChangeSubWeapon()
    {
        GameObject dummyWeapon = (GameObject)Instantiate(dropWeapon2, gameObject.transform.position, gameObject.transform.rotation);
        dummyWeapon.name = dropWeapon2.name;
        dummyWeapon.SetActive(true);
        dropWeapon2 = nearObject;
        subWeapon.SetActive(false);
        weaponName = nearObject.name;
        subWeapon = GameObject.Find(weaponRoot + weaponName);
        subWeapon.SetActive(true);
        equipWeapon = subWeapon;
        changeImg = GameObject.Find(weaponRoot + weaponName + "/" + weaponName);
        subWeaponImg.sprite = changeImg.GetComponent<Image>().sprite;
        changeImg.SetActive(false);
        if (mainWeapon != null)
            mainWeapon.SetActive(false);
        equipCount = 2;
        nearObject.SetActive(false);
        nearObject = null;
        changeEquipment.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Interation();
        Swap();
        if(equipWeapon != null)
            weapon = equipWeapon.GetComponent<Weapon>();
    }
}

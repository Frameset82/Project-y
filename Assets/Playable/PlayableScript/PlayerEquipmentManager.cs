using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipmentManager : MonoBehaviour
{
    private string weaponRoot = "Player/Male/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/";
    [Header("착용 무기")]
    public Weapon mainWeapon = null; // 1번무기
    public Weapon subWeapon = null; // 2번무기
    public static int equipCount = 0; // main = 1 sub = 2
    public Weapon equipWeapon; // 현제 착용중인 무기
    public GameObject nearObject;//플레이어와 가까이 있는 무기 오브젝트

    [Header("이미지관련(할당필요)")]
    public Image mainWeaponImg; //메인웨폰 이미지
    public Image subWeaponImg; // 서브웨폰 이미지
    private PlayerAnimation playerAnimation; // 플레이어 애니메이션 관리 스크립트
    public GameObject changeEquipment; // Panel
    public Image changeImg1;
    public Image changeImg2;

    private GameObject player;
    public Transform rifleTr;
    public Transform swordTr;
    public Transform meleeTr;
    public Transform spearTr;

    public void Interation()
    {
        switch (nearObject.tag)
        {
            case "Weapon":
                GetWeapon();
                break;
        }
    }
     
    public void GetWeapon()
    {
        WeaponTr();
        if(mainWeapon == null)
        {
            mainWeapon = nearObject.GetComponent<Weapon>();
            WeaponAnimChange(mainWeapon.wType);
            equipWeapon = mainWeapon;
            mainWeaponImg.sprite = mainWeapon.weaponSprite;
            equipCount = 1;
        }
        else if(subWeapon == null)
        {
            subWeapon = nearObject.GetComponent<Weapon>();
            WeaponAnimChange(subWeapon.wType);
            equipWeapon = subWeapon;
            subWeaponImg.sprite = subWeapon.weaponSprite;
            equipCount = 2;
        }
        else
        {
            changeEquipment.SetActive(true);
            changeImg1.sprite = mainWeaponImg.sprite;
            changeImg2.sprite = subWeaponImg.sprite;
            PlayerKeyboardInput.isShoot = true;
        }
    }

    public void WeaponTr()
    {
        nearObject.transform.SetParent(player.transform);
        Weapon newWeapon = nearObject.GetComponent<Weapon>();
        switch (newWeapon.wType)
        {
            case Weapon.WeaponType.Rifle:
                nearObject.transform.position = rifleTr.position;
                nearObject.transform.rotation = rifleTr.rotation;
                break;
            case Weapon.WeaponType.Melee:
                nearObject.transform.position = meleeTr.position;
                nearObject.transform.rotation = meleeTr.rotation;
                break;
            case Weapon.WeaponType.Sword:
                nearObject.transform.position = swordTr.position;
                nearObject.transform.rotation = swordTr.rotation;
                break;
            case Weapon.WeaponType.Spear:
                nearObject.transform.position = spearTr.position;
                nearObject.transform.rotation = spearTr.rotation;
                break;
        }
    }

    public void WeaponAnimChange(Weapon.WeaponType wType)
    {
        switch (wType)
        {
            case Weapon.WeaponType.Rifle:
                playerAnimation.playerAnimator.runtimeAnimatorController = playerAnimation.anim[3];
                break;
            case Weapon.WeaponType.Melee:
                playerAnimation.playerAnimator.runtimeAnimatorController = playerAnimation.anim[0];
                break;
            case Weapon.WeaponType.Sword:
                playerAnimation.playerAnimator.runtimeAnimatorController = playerAnimation.anim[1];
                break;
            case Weapon.WeaponType.Spear:
                playerAnimation.playerAnimator.runtimeAnimatorController = playerAnimation.anim[2];
                break;
        }
    }

    public void Swap() //무기 변경
    {
        if (equipCount == 1) //첫번째 무기를 들고 있을시
        {
            WeaponAnimChange(subWeapon.wType);
            mainWeapon.gameObject.SetActive(false);
            StartCoroutine(SwapCoroutine()); //무기 변경 애니메이션 코루틴 실행
            equipWeapon = subWeapon;
            subWeapon.gameObject.SetActive(false);
            equipCount = 2;
        }
        else if (equipCount == 2)
        {
            WeaponAnimChange(mainWeapon.wType);
            subWeapon.gameObject.SetActive(false);
            StartCoroutine(SwapCoroutine()); //무기 변경 애니메이션 코루틴 실행
            equipWeapon = mainWeapon;
            mainWeapon.gameObject.SetActive(false);
            equipCount = 2;
        }
    }

    // 버튼에 들어갈 메인 웨펀과 서브웨펀
    public void ChangeMainWeapon()
    {
        mainWeapon.gameObject.transform.SetParent(null);
        mainWeapon.transform.position = nearObject.transform.position;
        mainWeapon.transform.rotation = nearObject.transform.rotation;
        mainWeapon = nearObject.GetComponent<Weapon>();
        equipWeapon = mainWeapon;
        mainWeaponImg.sprite = mainWeapon.weaponSprite;
        equipCount = 1;
        changeEquipment.SetActive(false);
        WeaponTr();
        WeaponAnimChange(equipWeapon.wType);
    } 

    public void ChangeSubWeapon()
    {
        subWeapon.gameObject.transform.SetParent(null);
        subWeapon.transform.position = nearObject.transform.position;
        subWeapon.transform.rotation = nearObject.transform.rotation;
        subWeapon = nearObject.GetComponent<Weapon>();
        equipWeapon = subWeapon;
        subWeaponImg.sprite = subWeapon.weaponSprite;
        equipCount = 2;
        changeEquipment.SetActive(false);
        WeaponTr();
        WeaponAnimChange(subWeapon.wType);
    }

    private void OnTriggerStay(Collider other) //착용가능한 무기와 충돌시
    {
        nearObject = other.gameObject;
    }
    private void OnTriggerExit(Collider other)
    {
        nearObject = null;
    }

    void Awake()
    {
        playerAnimation = gameObject.GetComponent<PlayerAnimation>();
        player = GameObject.Find("Player");
    }

    public IEnumerator SwapCoroutine()
    {
        if (PlayerKeyboardInput.isDodge == false && PlayerKeyboardInput.isShoot == false)
        {
            yield return new WaitForSeconds(0.01f);
            playerAnimation.Swap();
            yield return new WaitForSeconds(1f);
        }
    }
}

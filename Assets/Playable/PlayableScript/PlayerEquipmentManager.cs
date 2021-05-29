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
    GameObject particleObj;

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
            ParticleDelete();
            mainWeapon = nearObject.GetComponent<Weapon>();
            WeaponAnimChange(mainWeapon.wType);
            equipWeapon = mainWeapon;
            mainWeaponImg.sprite = mainWeapon.weaponSprite;
            equipCount = 1;
            nearObject = null;
        }
        else if(subWeapon == null)
        {
            ParticleDelete();
            mainWeapon.gameObject.SetActive(false);
            subWeapon = nearObject.GetComponent<Weapon>();
            WeaponAnimChange(subWeapon.wType);
            equipWeapon = subWeapon;
            subWeaponImg.sprite = subWeapon.weaponSprite;
            equipCount = 2;
            nearObject = null;
        }
        else
        {
            changeEquipment.SetActive(true);
            changeImg1.sprite = mainWeaponImg.sprite;
            changeImg2.sprite = subWeaponImg.sprite;
            PlayerKeyboardInput.isShoot = true;
        }
    }

    public void ParticleDelete()
    { 
        particleObj = nearObject.transform.GetChild(0).gameObject;
        particleObj.SetActive(false);
    }

    public void WeaponTr()
    {
        nearObject.transform.SetParent(player.transform);
        Weapon newWeapon = nearObject.GetComponent<Weapon>();
        /*        switch (newWeapon.wType)
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
                }*/
        nearObject.transform.position = newWeapon.tr.position;
        nearObject.transform.rotation = newWeapon.tr.rotation;
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

    public void Swap(int count) //무기 변경
    {
        if (equipCount == 1 && count == 2) //첫번째 무기를 들고 있을시
        {
            PlayerKeyboardInput.isSwap = true;
            WeaponAnimChange(subWeapon.wType);
            mainWeapon.gameObject.SetActive(false);
            StartCoroutine(SwapCoroutine()); //무기 변경 애니메이션 코루틴 실행
            equipWeapon = subWeapon;
            subWeapon.gameObject.SetActive(true);
            equipCount = 2;
        }
        else if (equipCount == 2 && count == 1)
        {
            PlayerKeyboardInput.isSwap = true;
            WeaponAnimChange(mainWeapon.wType);
            subWeapon.gameObject.SetActive(false);
            StartCoroutine(SwapCoroutine()); //무기 변경 애니메이션 코루틴 실행
            equipWeapon = mainWeapon;
            mainWeapon.gameObject.SetActive(true);
            equipCount = 1;
        }
    }

    // 버튼에 들어갈 메인 웨펀과 서브웨펀
    public void ChangeMainWeapon()
    {
        // 들고있는 무기를 땅에 떨어트리는 과정
        mainWeapon.gameObject.SetActive(true);
        subWeapon.gameObject.SetActive(false);
        ParticleDelete();
        mainWeapon.transform.position = nearObject.transform.position;
        mainWeapon.transform.rotation = nearObject.transform.rotation;
        particleObj = mainWeapon.transform.GetChild(0).gameObject;
        particleObj.SetActive(true);
        mainWeapon.gameObject.transform.SetParent(null);
        // 무기를 장착하는 과정
        mainWeapon = nearObject.GetComponent<Weapon>();
        equipWeapon = mainWeapon;
        mainWeaponImg.sprite = mainWeapon.weaponSprite;
        equipCount = 1;
        WeaponTr();
        WeaponAnimChange(equipWeapon.wType);
        particleObj = mainWeapon.transform.GetChild(0).gameObject;
        particleObj.SetActive(false);
        changeEquipment.SetActive(false); // 패널 끄기
        PlayerKeyboardInput.isShoot = false;
    }

    public void ChangeSubWeapon()
    {
        // 들고있는 무기를 땅에 떨어트리는 과정
        subWeapon.gameObject.SetActive(true);
        mainWeapon.gameObject.SetActive(false);
        ParticleDelete();
        subWeapon.transform.position = nearObject.transform.position;
        subWeapon.transform.rotation = nearObject.transform.rotation;
        particleObj = subWeapon.transform.GetChild(0).gameObject;
        particleObj.SetActive(true);
        subWeapon.gameObject.transform.SetParent(null);
        // 무기를 장착하는 과정
        subWeapon = nearObject.GetComponent<Weapon>();
        equipWeapon = subWeapon;
        subWeaponImg.sprite = subWeapon.weaponSprite;
        equipCount = 2;
        WeaponTr();
        WeaponAnimChange(subWeapon.wType);
        particleObj = subWeapon.transform.GetChild(0).gameObject;
        particleObj.SetActive(false);
        changeEquipment.SetActive(false); // 패널 끄기
        PlayerKeyboardInput.isShoot = false;
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
        player = GameObject.Find("Player/Male/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipmentManager : MonoBehaviour
{
    private PlayerAnimation playerAnimation;
    private PlayerInfo playerInfo;
    private PlayerKeyboardInput playerKeyboardInput;
    private GameObject player;

    public string weaponRoot = "Player/Male/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/";
    public GameObject playerWeaponRoot;
    public GameObject nearObject;//플레이어와 가까이 있는 무기 오브젝트

    [Header("착용 무기")]
    public Weapon mainWeapon = null; // 1번무기
    public Weapon subWeapon = null; // 2번무기
    public static int equipCount = 0; // main = 1 sub = 2
    public Weapon equipWeapon; // 현제 착용중인 무기

    [Header("무기 이미지관련(할당필요)")]
    public Image mainWeaponImg; //메인웨폰 이미지
    public Image subWeaponImg; // 서브웨폰 이미지
    public GameObject changeEquipment; // Panel
    public Image changeImg1;
    public Image changeImg2;

    [Header("아이템관련")]
    public ActiveItem FirstItem = null;
    public ActiveItem SecondItem = null;
    public ActiveItem ThirdItem = null;

    [Header("아이템 이미지관련(할당필요)")]
    public Image FirstItemImg; // 첫번째 아이템
    public Image SecondItemImg; // 두번째 아이템
    public Image ThirdItemImg; // 세번째 아이템
    public GameObject changeItem; // Panel
    public Image iChangeImg1;
    public Image iChangeImg2;
    public Image iChangeImg3;

    GameObject particleObj;

    private void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        player = gameObject;
    }

    public void Interation()
    {
        switch (nearObject.tag)
        {
            case "Weapon":
                GetWeapon();
                break;
            case "Item":
                GetItem();
                break;
        }
    }

    public void GetWeapon()
    {
        WeaponTr();
        if (mainWeapon == null)
        {
            ParticleDelete();
            mainWeapon = nearObject.GetComponent<Weapon>();
            WeaponAnimChange(mainWeapon.wType);
            equipWeapon = mainWeapon;
            mainWeapon.OnEquip();
            mainWeaponImg.sprite = mainWeapon.weaponSprite;
            equipCount = 1;
            playerAnimation.ChangeAnimator();
            nearObject = null;
        }
        else if (subWeapon == null)
        {
            ParticleDelete();
            mainWeapon.gameObject.SetActive(false);
            subWeapon = nearObject.GetComponent<Weapon>();
            WeaponAnimChange(subWeapon.wType);
            equipWeapon = subWeapon;
            subWeapon.OnEquip();
            subWeaponImg.sprite = subWeapon.weaponSprite;
            equipCount = 2;
            playerAnimation.ChangeAnimator();
            nearObject = null;
        }
        else
        {
            playerKeyboardInput.isChange = true;
            changeEquipment.SetActive(true);
            changeImg1.sprite = mainWeaponImg.sprite;
            changeImg2.sprite = subWeaponImg.sprite;
            playerKeyboardInput.isShoot = true;
        }
    }

    private Renderer rend; // 렌더러 끄기 용
    public void GetItem()
    {
        nearObject.transform.SetParent(player.transform);
        if (FirstItem == null)
        {
            ParticleDelete();
            FirstItem = nearObject.GetComponent<ActiveItem>();
            FirstItemImg.sprite = FirstItem.ItemSprite;
            rend = nearObject.GetComponent<MeshRenderer>();
            rend.enabled = false;
            nearObject = null;
        }
        else if (SecondItem == null)
        {
            ParticleDelete();
            SecondItem = nearObject.GetComponent<ActiveItem>();
            SecondItemImg.sprite = SecondItem.ItemSprite;
            rend = nearObject.GetComponent<MeshRenderer>();
            rend.enabled = false;
            nearObject = null;
        }
        else if (ThirdItem == null)
        {
            ParticleDelete();
            ThirdItem = nearObject.GetComponent<ActiveItem>();
            ThirdItemImg.sprite = ThirdItem.ItemSprite;
            rend = nearObject.GetComponent<MeshRenderer>();
            rend.enabled = false;
            nearObject = null;
        }
        else
        {
            playerKeyboardInput.isChange = true;
            changeItem.SetActive(true);
            iChangeImg1.sprite = FirstItemImg.sprite;
            iChangeImg2.sprite = SecondItemImg.sprite;
            iChangeImg3.sprite = ThirdItemImg.sprite;
            playerKeyboardInput.isShoot = true;
        }
    }

    public void ParticleDelete()
    {
        particleObj = nearObject.transform.GetChild(0).gameObject;
        particleObj.SetActive(false);
    }

    public void WeaponTr()
    {
        nearObject.transform.SetParent(playerWeaponRoot.transform);
        Weapon newWeapon = nearObject.GetComponent<Weapon>();
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
            playerKeyboardInput.isSwap = true;
            WeaponAnimChange(subWeapon.wType);
            mainWeapon.gameObject.SetActive(false);
            StartCoroutine(SwapCoroutine()); //무기 변경 애니메이션 코루틴 실행
            equipWeapon = subWeapon;
            subWeapon.gameObject.SetActive(true);
            playerAnimation.ChangeAnimator();
            equipCount = 2;
        }
        else if (equipCount == 2 && count == 1)
        {
            playerKeyboardInput.isSwap = true;
            WeaponAnimChange(mainWeapon.wType);
            subWeapon.gameObject.SetActive(false);
            StartCoroutine(SwapCoroutine()); //무기 변경 애니메이션 코루틴 실행
            equipWeapon = mainWeapon;
            mainWeapon.gameObject.SetActive(true);
            playerAnimation.ChangeAnimator();
            equipCount = 1;
        }
    }

    // 버튼에 들어갈 메인 웨펀과 서브웨펀
    public void ChangeMainWeapon()
    {
        // 들고있는 무기를 땅에 떨어트리는 과정
        mainWeapon.gameObject.SetActive(true);
        mainWeapon.UnEquip();
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
        mainWeapon.OnEquip();
        mainWeaponImg.sprite = mainWeapon.weaponSprite;
        equipCount = 1;
        WeaponTr();
        WeaponAnimChange(equipWeapon.wType);
        particleObj = mainWeapon.transform.GetChild(0).gameObject;
        particleObj.SetActive(false);
        playerAnimation.ChangeAnimator();
        changeEquipment.SetActive(false); // 패널 끄기
        StateReset();
        playerKeyboardInput.isShoot = false;
    }

    public void ChangeSubWeapon()
    {
        // 들고있는 무기를 땅에 떨어트리는 과정
        subWeapon.gameObject.SetActive(true);
        subWeapon.UnEquip();
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
        subWeapon.OnEquip();
        subWeaponImg.sprite = subWeapon.weaponSprite;
        equipCount = 2;
        WeaponTr();
        WeaponAnimChange(subWeapon.wType);
        particleObj = subWeapon.transform.GetChild(0).gameObject;
        particleObj.SetActive(false);
        playerAnimation.ChangeAnimator();
        changeEquipment.SetActive(false); // 패널 끄기
        StateReset();
        playerKeyboardInput.isShoot = false;
    }

    public void ChangeFirstItem()
    {
        // 들고있는 아이템을 땅에 떨어트리는 과정
        ParticleDelete();
        particleObj = FirstItem.transform.GetChild(0).gameObject;
        particleObj.SetActive(true);
        rend = FirstItem.GetComponent<MeshRenderer>();
        rend.enabled = true;
        FirstItem.gameObject.transform.SetParent(null);
        // 무기를 장착하는 과정
        FirstItem = nearObject.GetComponent<ActiveItem>();
        FirstItemImg.sprite = FirstItem.ItemSprite;
        rend = FirstItem.GetComponent<MeshRenderer>();
        rend.enabled = false;
        particleObj = FirstItem.transform.GetChild(0).gameObject;
        particleObj.SetActive(false);
        changeItem.SetActive(false); // 패널 끄기
        StateReset();
        playerKeyboardInput.isShoot = false;
    }

    public void ChangeSecondItem()
    {
        // 들고있는 아이템을 땅에 떨어트리는 과정
        ParticleDelete();
        particleObj = SecondItem.transform.GetChild(0).gameObject;
        particleObj.SetActive(true);
        rend = SecondItem.GetComponent<MeshRenderer>();
        rend.enabled = true;
        SecondItem.gameObject.transform.SetParent(null);
        // 무기를 장착하는 과정
        SecondItem = nearObject.GetComponent<ActiveItem>();
        SecondItemImg.sprite = SecondItem.ItemSprite;
        rend = SecondItem.GetComponent<MeshRenderer>();
        rend.enabled = false;
        particleObj = FirstItem.transform.GetChild(0).gameObject;
        particleObj.SetActive(false);
        changeItem.SetActive(false); // 패널 끄기
        StateReset();
        playerKeyboardInput.isShoot = false;
    }

    public void ChangeThirdItem()
    {
        // 들고있는 아이템을 땅에 떨어트리는 과정
        ParticleDelete();
        particleObj = ThirdItem.transform.GetChild(0).gameObject;
        particleObj.SetActive(true);
        rend = ThirdItem.GetComponent<MeshRenderer>();
        rend.enabled = true;
        ThirdItem.gameObject.transform.SetParent(null);
        // 무기를 장착하는 과정
        ThirdItem = nearObject.GetComponent<ActiveItem>();
        ThirdItemImg.sprite = ThirdItem.ItemSprite;
        rend = ThirdItem.GetComponent<MeshRenderer>();
        rend.enabled = false;
        particleObj = ThirdItem.transform.GetChild(0).gameObject;
        particleObj.SetActive(false);
        changeItem.SetActive(false); // 패널 끄기
        StateReset();
        playerKeyboardInput.isShoot = false;
    }

    public void StateReset()
    {
        playerKeyboardInput.isDodge = false;
        playerKeyboardInput.isRight = false;
        playerKeyboardInput.isSwap = false;
        playerKeyboardInput.isChange = false;
    }

    private void OnTriggerStay(Collider other) //착용가능한 무기와 충돌시
    {
        if (other.tag == "Weapon" || other.tag == "Item")
        {
            nearObject = other.gameObject;
            if(other.tag == "Weapon")
                nearObject.GetComponent<Weapon>().SetPlayer(player);
            else if(other.tag == "Item")
                nearObject.GetComponent<ActiveItem>().SetPlayer(player);
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        nearObject = null;
    }

    void Awake()
    {
        
    }

    public IEnumerator SwapCoroutine()
    {
        if (playerKeyboardInput.isDodge == false && playerKeyboardInput.isShoot == false)
        {
            yield return new WaitForSeconds(0.01f);
            playerAnimation.Swap();
            yield return new WaitForSeconds(1f);
        }
    }
}

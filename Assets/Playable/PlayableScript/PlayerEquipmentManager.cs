using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipmentManager : MonoBehaviour
{
    private string weaponRoot = "Player/Male/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/";
    [Header("착용 무기")]
    public GameObject mainWeapon = null; // 1번무기
    public GameObject subWeapon = null; // 2번무기
    public static int equipCount = 1; // main = 1 sub = 2
    public GameObject equipWeapon; // 현제 착용중인 무기
    public GameObject nearObject;//플레이어와 가까이 있는 무기 오브젝트
    private string weaponName; //무기 이름

    private bool changeAnim = false; //애니메이션 전환 유무
    private GameObject changeImg; // 교체이미지

    [Header("이미지관련(할당필요)")]
    public Image mainWeaponImg; //메인웨폰 이미지
    public Image subWeaponImg; // 서브웨폰 이미지
    private PlayerAnimation playerAnimation; // 플레이어 애니메이션 관리 스크립트
    private Rigidbody playerRigidbody; // 캐릭터 리지드바디
    public GameObject changeEquipment; // Panel
    public Image changeImg1;
    public Image changeImg2;

    [Header("떨어트릴 무기")]
    public GameObject dropWeapon1; // 무기 두칸이 다 차있을경우 떨어트릴 무기
    public GameObject dropWeapon2;

    [Header("착용중 무기의 스크립트")]
    public Weapon mainWeaponScript;
    public Weapon subWeaponScript;
    public Weapon equipWeaponScript;

    public void Interation()
    {
        if (nearObject.tag == "Weapon") //가까이 있는 오브젝트의 태그가 무기일시
        {
            if (mainWeapon == null) //메인웨폰이 없으면
            {
                dropWeapon1 = nearObject; //드랍시 웨폰은 가까이 있는 오브젝트설정
                weaponName = nearObject.name; //무기이름을 오브젝트의 이름으로 설정
                mainWeapon = GameObject.Find(weaponRoot + weaponName); // 메인웨폰 할당
                mainWeapon.SetActive(true); // 메인웨폰 활성화
                equipWeapon = mainWeapon; // 착용중인 무기를 메인 웨폰으로 변경
                changeImg = GameObject.Find(weaponRoot + weaponName + "/" + weaponName); //이미지 바꾸기
                mainWeaponImg.sprite = changeImg.GetComponent<Image>().sprite; //메인 웨폰 이미지 바꾸기
                changeImg.SetActive(false); //
                if (subWeapon != null)//서브 웨폰이 null이 아닐시
                    subWeapon.SetActive(false); //서브웨폰 엑티브 false;
                equipCount = 1;
                nearObject.SetActive(false); //가까이 있는 오브젝트 비활성화
                nearObject = null; //가까이있는 오브젝트 지우기
            }
            else if (subWeapon == null) //서브 웨폰이 없으면
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
                PlayerKeyboardInput.isShoot = true;
                Time.timeScale = 0; // 시간 정지
            }
        }
    }
     
    public void Swap()//무기 변경
    {
        if (equipCount == 1) //첫번째 무기를 들고 있을시
        {
            mainWeapon.SetActive(false);//메인 웨폰 비활성화
            StartCoroutine(SwapCoroutine()); //무기 변경 애니메이션 코루틴 실행
            equipWeapon = subWeapon; //현재 착용중인 무기를 서브웨폰으로 변경
            subWeapon.SetActive(true); // 서브웨폰 활성화
            equipCount = 2; // 2번쨰 무기를 들고 있는 상태로 변경
            subWeaponScript.ChangeAnimator(); // 애니메이터 변경
        }
        else if (equipCount == 2)
        {
            subWeapon.SetActive(false);
            StartCoroutine(SwapCoroutine());
            equipWeapon = mainWeapon;
            mainWeapon.SetActive(true);
            equipCount = 1;
            mainWeaponScript.ChangeAnimator();
        }
    }

    // 버튼에 들어갈 메인 웨펀과 서브웨펀
    public void ChangeMainWeapon()
    {
        GameObject dummyWeapon = (GameObject)Instantiate(dropWeapon1, gameObject.transform.position, gameObject.transform.rotation);//더미 웨폰생성
        dummyWeapon.name = dropWeapon1.name; //더미 웨폰 이름 설정
        dummyWeapon.SetActive(true); //더미 웨폰 활성화
        dropWeapon1 = nearObject; //가까이 있는 웨폰을 더미웨폰으로 설정
        mainWeapon.SetActive(false); // 메인 웨폰 비활성화
        weaponName = nearObject.name; //
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
        PlayerKeyboardInput.isShoot = false;
        changeAnim = true;
        Time.timeScale = 1; // 시간정지 해제
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
        PlayerKeyboardInput.isShoot = false;
        changeAnim = true;
        Time.timeScale = 1; // 시간정지 해제
    } //2번째 무기를 드랍할 무기와 변경

    private void OnTriggerStay(Collider other) //착용가능한 무기와 충돌시
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
        playerAnimation = gameObject.GetComponent<PlayerAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mainWeapon != null)//첫번째 무기가 없으면
            mainWeaponScript = mainWeapon.GetComponent<Weapon>(); //메인웨폰 스크립트 가져오기
        if(subWeapon != null)
            subWeaponScript = subWeapon.GetComponent<Weapon>();
        if(equipWeapon != null)
            equipWeaponScript = equipWeapon.GetComponent<Weapon>();
        if (changeAnim)
        {
            equipWeaponScript.ChangeAnimator();
            changeAnim = false;
        }
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

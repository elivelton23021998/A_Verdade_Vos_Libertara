using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSwitcher : MonoBehaviour {

    private Inventory inventory;
    private AVVL_GameManager gameManager;

	public List<GameObject> ItemList = new List<GameObject>();
    public int currentItem = -1;

    [Header("Detecção da Parede")]
    public bool detectWall;
    public LayerMask HitMask;
    public float wallHitRange;

    public Animation WallDetectAnim;
    public string HideAnim;
    public string ShowAnim;

    [Header("Item No Início")]
    public bool startWithCurrentItem;
    public bool startWithoutAnimation;

    [Tooltip("O ItemID no banco de dados de inventário. Mantenha -1 se não for um item de inventário.")]
    public int startingItemID = -1; 

    [Header("Misc")]
    [Tooltip("ID deve ser sempre o objeto leve que você está usando no momento!")]
    public int currentLightObject = 0;

    [HideInInspector]
    public int weaponItem = -1;

    private bool hit;
    private bool handsFree;
    private bool switchItem;
    private bool isPressed;

	private int newItem = 0;
    private bool antiSpam;
    private bool spam;

    void Start()
    {
        inventory = transform.root.gameObject.GetComponentInChildren<ScriptManager>().GetScript<Inventory>();
        gameManager = transform.root.gameObject.GetComponentInChildren<ScriptManager>().GetScript<AVVL_GameManager>();

        if (startWithCurrentItem)
        {
            if (startingItemID > -1)
            {
                inventory.AddItem(startingItemID, 1);
            }

            if (!startWithoutAnimation)
            {
                SelectItem(currentItem);
            }
            else
            {
                ActivateItem(currentItem);
            }
        }
    }

    public void SelectItem(int id)
    {
        //if (IsBusy()) return;

        if (id != currentItem)
        {
            newItem = id;

            if (!CheckActiveItem())
            {
                SelectItem();
            }
            else
            {
                StartCoroutine(SwitchItem());
            }
        }
        else
        {
            DeselectItems();
        }
    }

    public void DeselectItems()
	{
        if (currentItem == -1) return;
        ItemList [currentItem].GetComponent<ISwitcher>().Deselect();
    }

    public void DisableItems()
    {
        if (currentItem == -1) return;
        ItemList[currentItem].GetComponent<ISwitcher>().Disable();
    }

    public int GetIDByObject(GameObject switcherObject)
    {
        return ItemList.IndexOf(switcherObject);
    }

    public GameObject GetCurrentItem()
    {
        if(currentItem != -1)
        {
            return ItemList[currentItem];
        }

        return null;
    }

    public bool IsBusy()
    {
        return transform.root.gameObject.GetComponentInChildren<ExamineManager>().isExamining || transform.root.gameObject.GetComponentInChildren<DragRigidbody>().CheckHold();
    }

    /// <summary>
    /// Verifique se todos os itens estão desativados
    /// </summary>
	bool CheckActiveItem()
	{
		for (int i = 0; i < ItemList.Count; i++) {
            bool ACState = ItemList[i].transform.GetChild(0).gameObject.activeSelf;
			if (ACState)
				return true;
		}
		return false;
	}

	IEnumerator SwitchItem()
	{
        switchItem = true;
        ItemList [currentItem].GetComponent<ISwitcher>().Deselect();

        yield return new WaitUntil (() => ItemList[currentItem].transform.GetChild(0).gameObject.activeSelf == false);

        ItemList[newItem].GetComponent<ISwitcher>().Select();
		currentItem = newItem;
        switchItem = false;
    }

	void SelectItem()
	{
        switchItem = true;
        ItemList [newItem].GetComponent<ISwitcher>().Select();
        currentItem = newItem;
        switchItem = false;
    }

    void Update()
    {
        if (!gameManager.scriptManager.ScriptGlobalState) return;

        if (WallDetectAnim && detectWall && !handsFree && currentItem != -1)
        {
            if (WallHit())
            {
                if (!hit)
                {
                    WallDetectAnim.Play(HideAnim);
                    if (ItemList[currentItem].GetComponent<ISwitcherWallHit>() != null)
                    {
                        ItemList[currentItem].GetComponent<ISwitcherWallHit>().OnWallHit(true);
                    }
                    hit = true;
                }
            }
            else
            {
                if (hit)
                {
                    WallDetectAnim.Play(ShowAnim);
                    if (ItemList[currentItem].GetComponent<ISwitcherWallHit>() != null)
                    {
                        ItemList[currentItem].GetComponent<ISwitcherWallHit>().OnWallHit(false);
                    }
                    hit = false;
                }
            }
        }

        if (!gameManager.isGrabbed)
        {
            if (!antiSpam)
            {
                //Mouse ScrollWheel Para trás - Desmarque o item atual
                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    if (currentItem != -1)
                    {
                        DeselectItems();
                    }
                }

                //Mouse ScrollWheel Avançar - Selecione o último item de arma
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    if (weaponItem != -1)
                    {
                        MouseWHSelectWeapon();
                    }
                }
            }
            else
            {
                if (!spam)
                {
                    StartCoroutine(AntiSwitchSpam());
                    spam = true;
                }
            }
        }
        else
        {
            antiSpam = true;
        }
    }

    void MouseWHSelectWeapon()
    {
        if (currentItem != weaponItem)
        {
            if (ItemList[weaponItem].GetComponent<WeaponController>() && inventory.CheckSWIDInventory(weaponItem))
            {
                SelectItem(weaponItem);
            }
        }
    }

    IEnumerator AntiSwitchSpam()
    {
        antiSpam = true;
        yield return new WaitForSeconds(1f);
        antiSpam = false;
        spam = false;
    }

    void FixedUpdate()
    {
        if (!CheckActiveItem() && !switchItem)
        {
            currentItem = -1;
        }

        if (!inventory.CheckSWIDInventory(weaponItem))
        {
            weaponItem = -1;
        }
    }

    bool GetItemsActive()
    {
        bool response = true;
        for (int i = 0; i < ItemList.Count; i++)
        {
            if (ItemList[i].transform.GetChild(0).gameObject.activeSelf)
            {
                response = false;
                break;
            }
        }
        return response;
    }

    /// <summary>
    /// Ative o item sem reproduzir a animação.
    /// </summary>
    public void ActivateItem(int switchID)
    {
        switchItem = true;
        ItemList[switchID].GetComponent<ISwitcher>().EnableItem();
        currentItem = switchID;
        newItem = switchID;
        switchItem = false;
    }

    public void FreeHands(bool free)
    {
        if (currentItem != -1)
        {
            if (free && !handsFree)
            {
                WallDetectAnim.wrapMode = WrapMode.Once;
                WallDetectAnim.Play(HideAnim);
                if (ItemList[currentItem].GetComponent<ISwitcherWallHit>() != null)
                {
                    ItemList[currentItem].GetComponent<ISwitcherWallHit>().OnWallHit(true);
                }
                handsFree = true;
            }
            else if (!free && handsFree)
            {
                WallDetectAnim.wrapMode = WrapMode.Once;
                WallDetectAnim.Play(ShowAnim);
                if (ItemList[currentItem].GetComponent<ISwitcherWallHit>() != null)
                {
                    ItemList[currentItem].GetComponent<ISwitcherWallHit>().OnWallHit(false);
                }
                handsFree = false;
            }
        }
    }

    bool WallHit()
    {
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, wallHitRange, HitMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (detectWall)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward * wallHitRange));
        }
    }
}
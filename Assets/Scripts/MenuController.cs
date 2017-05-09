using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

    public GameObject[] prefabArr;
    public short pageNumber = 0;
    private short currentNum = 0;
    private short maxPageNum = 4;
    private Texture[] pageTexture;
    public MeshRenderer[] pageDisplay;

    public Transform[] displayArr;
    public Transform[] displayPos; //This is the 6 box colliders on the menu

    public Transform dumpster;
    



    // Use this for initialization
    void Start () {
        Debug.Log(dumpster.transform.childCount);

        if (prefabArr.Length == 0)
        {
            prefabArr = Resources.LoadAll<GameObject>("Prefabs");
            pageTexture = Resources.LoadAll<Texture>("Texture");
            displayArr = new Transform[dumpster.transform.childCount];
            for (int i = 0; i < dumpster.transform.childCount; i++)
            {
                displayArr[i] = dumpster.GetChild(i);
            }
        }
        SetMenu(prefabArr, currentNum);

    }
	
	// Update is called once per frame
	void Update () {
        //If the page goes above or below the amount of pages we have then set it.
        if(pageNumber > maxPageNum)
        {
            pageNumber = 0;
        }
        else if(pageNumber < 0)
        {
            pageNumber = maxPageNum;
        }

        //Change the page of the menu if it has changed since the last known page.
	    if(currentNum != pageNumber)
        {
            currentNum = pageNumber;
            SetMenu(prefabArr, currentNum);
        }


        if(currentNum == maxPageNum)
        {
            for(int j = 0; j < displayArr.Length % 6; j++) //Taking into account if there is not enough objects on the last page
            {
                displayArr[j + (6 * currentNum)].transform.SetParent(displayPos[j]);
                displayArr[j + (6 * currentNum)].transform.localPosition = new Vector3(0.11f, 0.015f, -0.07f);
                displayArr[j + (6 * currentNum)].transform.rotation = displayPos[j].rotation * Quaternion.Euler(Vector3.right * 15f) * Quaternion.Euler(Vector3.up * 180f);
            }
        }
        else
        {
            //Move the objects on the menu onto the menu and keep em there.
            for (int i = 0; i < transform.childCount; i++)
            {
                displayArr[i + (6 * currentNum)].transform.SetParent(displayPos[i]);
                displayArr[i + (6 * currentNum)].transform.localPosition = new Vector3(0.11f, 0.015f, -0.07f);
                displayArr[i + (6 * currentNum)].transform.rotation = displayPos[i].rotation * Quaternion.Euler(Vector3.right * 15f) * Quaternion.Euler(Vector3.up * 180f);
            }
        }
        


    }


    void SetMenu(GameObject[] arr, short num)
    {
        //Remove all the displayed objects from the menu
        for(int i = 0; i < displayArr.Length; i++) //Currently it takes ALL objects. Can be improved to only take currently objects on the menu.
        {
            displayArr[i].transform.position = dumpster.position;
            displayArr[i].transform.SetParent(dumpster.transform);
        }

        //Choose which objects are currently on the menu, and place them correctly on the menu
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentChild;
            currentChild = this.gameObject.transform.GetChild(i);
            if (currentNum == maxPageNum)
            {
                if(i > displayArr.Length % 6)
                {
                    currentChild.GetComponent<InteractableItem>().worldPrefab = null;
                } else
                {
                    currentChild.GetComponent<InteractableItem>().worldPrefab = prefabArr[i + (6 * num)];

                }
            } else
            {
                currentChild.GetComponent<InteractableItem>().worldPrefab = prefabArr[i + (6 * num)];
            }
        }

        switch(num)
        {
            default:
                pageDisplay[0].material.mainTexture = pageTexture[0];
                pageDisplay[1].material.mainTexture = pageTexture[1];
                pageDisplay[2].material.mainTexture = pageTexture[2];
                break;
            /*case 0:
                pageDisplay[0].material.mainTexture = pageTexture[7];
                pageDisplay[1].material.mainTexture = pageTexture[0];
                pageDisplay[2].material.mainTexture = pageTexture[1];
                break;
            case 1:
                pageDisplay[0].material.mainTexture = pageTexture[0];
                pageDisplay[1].material.mainTexture = pageTexture[1];
                pageDisplay[2].material.mainTexture = pageTexture[2];
                break;
            case 2:
                pageDisplay[0].material.mainTexture = pageTexture[1];
                pageDisplay[1].material.mainTexture = pageTexture[2];
                pageDisplay[2].material.mainTexture = pageTexture[3];
                break;
            case 3:
                pageDisplay[0].material.mainTexture = pageTexture[2];
                pageDisplay[1].material.mainTexture = pageTexture[3];
                pageDisplay[2].material.mainTexture = pageTexture[4];
                break;
            case 4:
                pageDisplay[0].material.mainTexture = pageTexture[3];
                pageDisplay[1].material.mainTexture = pageTexture[4];
                pageDisplay[2].material.mainTexture = pageTexture[5];
                break;
            case 5:
                pageDisplay[0].material.mainTexture = pageTexture[4];
                pageDisplay[1].material.mainTexture = pageTexture[5];
                pageDisplay[2].material.mainTexture = pageTexture[6];
                break;
            case 6:
                pageDisplay[0].material.mainTexture = pageTexture[5];
                pageDisplay[1].material.mainTexture = pageTexture[6];
                pageDisplay[2].material.mainTexture = pageTexture[7];
                break;
            case 7:
                pageDisplay[0].material.mainTexture = pageTexture[6];
                pageDisplay[1].material.mainTexture = pageTexture[7];
                pageDisplay[2].material.mainTexture = pageTexture[0];
                break; */
        }
    }

    //Used to clear the menu just before it is disabled
    public void DisableMenu()
    {
        for (int i = 0; i < displayArr.Length; i++)
        {
            displayArr[i].transform.position = dumpster.position;
        }
    }
}

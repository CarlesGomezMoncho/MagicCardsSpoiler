using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetListUI : MonoBehaviour
{

    public GameObject setItemList;
    public GameObject content;
    public Animator confirmDeleteAnimator;

    private CanvasGroup canvasGroup;
    private float alphaCanvasGroup;
    private bool listUpdated;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        listUpdated = false;
        alphaCanvasGroup = 0;
    }

    void Update()
    {
        alphaCanvasGroup = canvasGroup.alpha;

        //si es veu (o comença a vores el menu, l'actualitzem)
        if (alphaCanvasGroup > 0 && !listUpdated)
        {
            
            foreach (Set set in SetController.instance.GetSets())
            {
                SetItemList newSetItemList;

                GameObject item = Instantiate(setItemList);
                item.transform.SetParent(content.transform);
                item.transform.localScale = new Vector3(1, 1, 1);

                newSetItemList = item.GetComponent<SetItemList>();
                newSetItemList.set = set;
                newSetItemList.animator = confirmDeleteAnimator;
            }

            listUpdated = true;
        }
    }

    public void CloseWindowSet()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetItemList : MonoBehaviour
{
    public Set set;
    public TextMeshProUGUI title;
    public Toggle toggle;
    public Animator animator;

    void Start()
    {
        if (set != null)
        {
            title.text = set.name;

            int setActive = PlayerPrefs.GetInt(Globals.Instance.SET + set.id + Globals.Instance.ACTIVE, 1);

            if (setActive == 1)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.isOn = false;
            }

            toggle.onValueChanged.AddListener(delegate { ActivateToggle(); });
        }
    }

    public void ActivateToggle()
    {
        if (toggle.isOn)
        {
            PlayerPrefs.SetInt(Globals.Instance.SET + set.id + Globals.Instance.ACTIVE, 1);
        }
        else
        {
            PlayerPrefs.SetInt(Globals.Instance.SET + set.id + Globals.Instance.ACTIVE, 0);
        }
    }

    public void ShowDeleteWindow()
    {
        animator.Play("Fade-in");
    }
}

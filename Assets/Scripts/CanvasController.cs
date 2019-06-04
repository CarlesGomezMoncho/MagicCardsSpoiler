using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public TextMeshProUGUI textDemo;

    public GameObject hamburgerPanel;
    public GameObject hamburgerButton;
    public TextMeshProUGUI hamburgerTitle;
    public GameController gameController;
    public GameObject[] buttons;

    private DeviceOrientation orientation;
    private CanvasScaler canvasScaler;

    void Start()
    {
        orientation = DeviceOrientation.Portrait;

        canvasScaler = GetComponent<CanvasScaler>();
    }

    void Update()
    {
        //sols si la orientació era de les contraries (landscape vs portrait) se canvia
        if (Input.deviceOrientation != orientation)
        {
            if((Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight) &&
                (orientation == DeviceOrientation.Portrait || orientation == DeviceOrientation.PortraitUpsideDown))
            //if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)
            {
                canvasScaler.matchWidthOrHeight = 0;
                SetLandScapeProperties();
                orientation = Input.deviceOrientation;

            }
            //else if (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
            else if ((Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) 
            && (orientation == DeviceOrientation.LandscapeLeft || orientation == DeviceOrientation.LandscapeRight))
            {
                canvasScaler.matchWidthOrHeight = 1;
                SetPortraitProperties();
                orientation = Input.deviceOrientation;

            }


        }

        textDemo.text = canvasScaler.matchWidthOrHeight + " - Actual: " + Input.deviceOrientation + " Anterior: " + orientation;
    }

    public void SetLandScapeProperties()
    {
        hamburgerPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400); //fem mes ample el panell al girar pantalla
        hamburgerButton.transform.localScale = new Vector3(2, 2, 0);
        foreach (GameObject g in buttons)
        {
            g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60);
            g.GetComponentInChildren<TextMeshProUGUI>().fontSize = 22;
        }
    }

    public void SetPortraitProperties()
    {
        hamburgerPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 225);
        hamburgerButton.transform.localScale = new Vector3(1, 1, 0);
        foreach (GameObject g in buttons)
        {
            g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 33.5f);
            g.GetComponentInChildren<TextMeshProUGUI>().fontSize = 14;
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        //se crida a esta funció quan se canvien les dimensions de la pantalla (per a esperar a que s'acave de girar la pantalla quan canviem a protrait o landscape)
        gameController.RefreshCards();
    }
}

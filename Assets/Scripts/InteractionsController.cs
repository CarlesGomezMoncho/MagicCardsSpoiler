using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InteractionsController : MonoBehaviour
{
    public TextMeshProUGUI textprova;

    public float orthoZoomSpeed = 0.5f;
    public RadialSlider zoomSlider;

    private Rigidbody2D rbCamera;
    private ScreenOrientation currentOrientation;

    private Vector3 initialMousePosition;
    private Vector3 holdMousePosition;
    private Vector3 endMousePosition;
    private bool uiClick = false;   //per a saber si aprem en ui o en cartes


    void Start()
    {
        //agafem el rigidbody de la camera
        rbCamera = Camera.main.GetComponent<Rigidbody2D>();

        //orientació actual
        currentOrientation = Screen.orientation;
    }

    void Update()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            rbCamera.AddForce(transform.up * scroll * 2, ForceMode2D.Impulse);
        }

        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // ... change the orthographic size based on the change in distance between the touches.
            //Camera.main.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
            //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 0.1f, 10f);
            

            zoomSlider.SliderValue += deltaMagnitudeDiff * orthoZoomSpeed;
            zoomSlider.SliderValue = Mathf.Clamp(zoomSlider.SliderValue, 0f, 10f);// Make sure the orthographic size never drops below zero.
            GetComponent<GameController>().SetZoom(zoomSlider);
            //textprova.text = Camera.main.orthographicSize.ToString();
            textprova.text = zoomSlider.SliderValue.ToString();
        }
        else
        {
            //cantrol del ratolí per a pujar i baixar la camera
            if (Input.GetMouseButtonDown(0))
            {
                initialMousePosition = Input.mousePosition;
                holdMousePosition = Input.mousePosition;    //guardem per si mantenim apretat en el següent frame

                rbCamera.velocity = Vector3.zero; //si toquem la pantalla, anulem completament la velocitat que portava

                // Check if the mouse was clicked over a UI element
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    uiClick = true;
                }
            }

            if (Input.GetMouseButton(0))
            {
                //si no estem eu UI
                if (!uiClick)
                {
                    float posY = cameraPos.y + ((holdMousePosition.y - Input.mousePosition.y) * 0.005f);
                    Camera.main.transform.position = new Vector3(cameraPos.x, posY, cameraPos.z);
                    holdMousePosition = Input.mousePosition;    //guardem per al següent frame

                    //anem moguent el click inicial en el temps, si ens parem en meitat click no hem de propulsar la llista de cartes
                    initialMousePosition = new Vector3(initialMousePosition.x, Mathf.Lerp(initialMousePosition.y, Input.mousePosition.y, Time.deltaTime * 10));
                }
            }

            //soltem el ratolí i apliquem la força restant
            if (Input.GetMouseButtonUp(0))
            {
                if (!uiClick)
                {
                    endMousePosition = Input.mousePosition;

                    Vector3 force = new Vector3(0, initialMousePosition.y - endMousePosition.y, 0);
                    rbCamera.AddForce(force * .1f, ForceMode2D.Impulse);
                }

                uiClick = false;
            }
        }
    }

}

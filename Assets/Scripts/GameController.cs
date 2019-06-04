using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject card;             //sprite base per a agafar medides
    public float cardSeparation = .02f; //separació entre cartes
    public string assetsUrl;
    public float cameraMaxTopPosition = 0.16f;//posició máxima a la que aplegarà la camera, ha de estar per baix de la barra del menú
    public float cameraIncrementDesp = 0.2f;

    public GameObject panelTop;

    public RadialSlider zoomSlider;

    private float cameraMaxTopPositionInit; //valor inicial per al màxim de de la càmera

    private List<GameObject> cardList;  //llistat de objetes carta
    private int cardsInRow = 0;         //quantes cartes es mostren en una fila de pantalla. Per a calcular la meitat de la pantalla i centrar la camera
    private Vector3 lastCardPos;        //posició de la última carta
    private int numFiles;               //nº de files en pantalla
    private Vector3 CameraInitialPos;// = new Vector3(0, 0, -10);  //posició inicial de la camera

    private Dictionary<int, AssetBundle> assetDictionary;    //diccionari de assets, un per cada set

    private bool refreshCards = false;  //per si volem refrescar la llista de cartes

    void Start()
    {
        cardList = new List<GameObject>();

        CameraInitialPos = Camera.main.transform.position;

        cameraMaxTopPositionInit = cameraMaxTopPosition;

        //creem un diccionari amb els assets, el id del set com a clau
        assetDictionary = new Dictionary<int, AssetBundle>();

        //carreguem assets de imatges i guardem en el diccionari
        LoadAssetBundle();

        //mostrem cartes en pantalla
        ShowCards();

        SetInitialZoom();
    }

    void Update()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        //float scroll = Input.GetAxis("Mouse ScrollWheel");

        //si refreshCards es true, cridem a showCards
        if (refreshCards)
        {
            ShowCards();
        }

        //evitem que sobrepase per dalt si fem scroll
        if (cameraPos.y > cameraMaxTopPosition)
        {
            Camera.main.transform.position = new Vector3(cameraPos.x, cameraMaxTopPosition, cameraPos.z);
        }
        else
        //evitem que sobrepase el màxim de numero files
        if (cameraPos.y < lastCardPos.y)//-2 per separar un poc del panell del menu(encara que separem despres menys)
        {
            //si te mes de una fila, continuem, sino no apretem dalt de tot, que no es moga
            if (numFiles > 2)   //canviat per proves a 2
                Camera.main.transform.position = new Vector3(cameraPos.x, lastCardPos.y, cameraPos.z);
            else
                Camera.main.transform.position = new Vector3(cameraPos.x, 0, cameraPos.z);
        }

        
        
    }

    public void ShowCards()
    {
        float offsetX = card.GetComponent<SpriteRenderer>().bounds.size.x / 2 + cardSeparation; //el tamany de mitja carta
        float offsetY = card.GetComponent<SpriteRenderer>().bounds.size.y / 2 + cardSeparation; //la altura de mitja carta

        //agafem totes les cartes (S'HAURÀ DE MODIFICAR PER A QUE TRAGA ALGUNES)
        List<Card> list = CardController.instance.GetCards();

        //resetejem camera
        Camera.main.transform.position = CameraInitialPos;

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, Camera.main.nearClipPlane)); //posició de la carta. La primera dalt de tot a l'esquerra

        //borrem totes les cartes existents
        foreach (GameObject g in cardList)
        {
            Destroy(g);
        }
        //borrem la llista
        cardList.Clear();

        int i = 0, j = 0; //per a posicionar en pantalla

        //recorrem tots els resultats de la consulta
        for (int inc = 0; inc < list.Count; inc++)
        {

            //si id de la carta es 0 pasem
            if (list[inc].id == 0)
            {
                continue;
            }

            //posició horitzontal de la carta, la posició inicial (pos.x) + mitja carta (el centre de la carta es la meitat) * el nº de cartes ja mostrades(offset es la meitat)
            float cardPosX = pos.x + offsetX + i * offsetX * 2;   
            float cardPosY = pos.y - offsetY - j * offsetY * 2;

            GameObject newCard = Instantiate(card, new Vector3(cardPosX, cardPosY, 0), Quaternion.identity);    //instanciem una nova carta en la posició calculada

            AssetBundle asset = assetDictionary[list[inc].set];                 //agafem el asset del set
            Sprite s = (Sprite)asset.LoadAsset(list[inc].image, typeof(Sprite));//carreguem el sprite de la carta de l'asset
            newCard.GetComponent<SpriteRenderer>().sprite = s;                  //canviem al sprite que ens indica la consulta
            
            cardList.Add(newCard);

            //si no cap la següent carta, canviem de linea (offsetX * 3 es carta i mitja mes)
            if (Camera.main.WorldToScreenPoint(new Vector3(cardPosX + offsetX * 3, cardPosY, 0)).x >= Screen.width)
            {
                //si hi han mes cartes per fila de les que hi ha en la variable, l'actualitzem. Sumem 1 per que i=0 es 1 carta
                if (cardsInRow < i + 1)
                    cardsInRow = i + 1;

                i = 0;    //tornem al inici de la fila per tant canviem a la col·lumna 0
                j++;    //canviem de fila

            }
            else
            {
                i++;    //si no canviem de linea, continuem incrementant de columna
            }

            //guardem la posició de la carta per si es la ultima
            lastCardPos = newCard.transform.position;
            numFiles = j;

        }

        //després de posar totes les cartes, reposicionem la càmera per a que les cartes queden centrades horitzontalment
        float ampleCarta = card.GetComponent<SpriteRenderer>().bounds.size.x + cardSeparation * 2;  //ample de la carta en worldSpace
        float ampleCamera = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x * 2;  //multiplique * 2 per que la coordenada x es situa a meitat pantalla
        float maxCards = Mathf.Floor(ampleCamera / ampleCarta); //cartes per fila

        //el minim de cartes per fila ha de ser 1
        if (maxCards == 0)
        {
            maxCards = 1;
        }

        float ampleTotal = ampleCarta * maxCards; //ample que ocupen les cartes en la fila
        float whiteSpace = ampleCamera - ampleTotal;//espai en blanc que queda

        Camera.main.transform.position = new Vector3(-whiteSpace / 2, CameraInitialPos.y + cameraMaxTopPosition, -10);  //centrem la camera (-whitespace/2 per que es la meitat del espai que queda, i l'afegim a la esquerra)

        refreshCards = false;   //finalment anulem la cridà a aquesta funció
    }

    public void LoadAssetBundle()
    {
        foreach (Set s in SetController.instance.GetSets())
        {
            //si es el 0 pasem de este
            if (s.id == 0)
            {
                continue;
            }

            //Debug.Log("nom: " + s.name + " id: " + s.id);

            //carreguem el asset
            Debug.Log(Application.persistentDataPath + assetsUrl + "/" + s.shortname.ToLower());
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + assetsUrl + "/" + s.shortname.ToLower());
            //si s'ha carregat
            if (assetBundle)
            {
                //el posem en el diccionari
                assetDictionary.Add(s.id, assetBundle);
            }
        }

    }

    public void RefreshCards()
    {
        refreshCards = true;
    }

    public void SetZoom(RadialSlider slider)
    {
        
        Camera.main.orthographicSize = slider.SliderValue + 1;
        //posem un marge fixe per a separar la carta un poc per dalt, depen del zoom de la camera
        cameraMaxTopPosition = (cameraMaxTopPositionInit * slider.SliderValue) + cameraIncrementDesp;

        PlayerPrefs.SetFloat("zoom", slider.SliderValue);
        ShowCards();
    }

    private void SetInitialZoom()
    {
        zoomSlider.SliderValue = PlayerPrefs.GetFloat("zoom", 2.67f);

        SetZoom(zoomSlider);
    }
}

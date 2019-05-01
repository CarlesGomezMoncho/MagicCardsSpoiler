using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject card;     //sprite base per a agafar medides
    public float cardSeparation = .02f; //separació entre cartes

    private List<GameObject> cardList;
    private int cardsInRow = 0;    //quantes cartes es mostren en una fila de pantalla. Per a calcular la meitat de la pantalla i centrar la camera
    private Vector3 lastCardPos;   //posició de la última carta
    private int numFiles;
    private Vector3 CameraInitialPos = new Vector3(0, 0, -10);


    void Start()
    {
        cardList = new List<GameObject>();
        CameraInitialPos = Camera.main.transform.position;

        ShowCards();
    }

    void Update()
    {
        
    }

    public void ShowCards()
    {
        float offsetX = card.GetComponent<SpriteRenderer>().bounds.size.x / 2 + cardSeparation; //el tamany de mitja carta
        float offsetY = card.GetComponent<SpriteRenderer>().bounds.size.y / 2 + cardSeparation; //la altura de mitja carta

        //agafem totes les cartes (S'HAURÀ DE MODIFICAR PER A QUE TRAGA ALGUNES)
        List<Card> list = CardController.instance.GetCards();

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, Camera.main.nearClipPlane)); //posició de la carta. La primera dalt de tot a l'esquerra

        int i = 0, j = 0; //per a posicionar en pantalla

        //recorrem tots els resultats de la consulta
        for (int inc = 0; inc < list.Count; inc++)
        {

            float cardPosX = pos.x + offsetX + i * offsetX * 2;   //posició horitzontal de la carta, la posició inicial (pos.x) + mitja carta (el centre de la carta es la meitat) * el nº de cartes ja mostrades(offset es la meitat)
            float cardPosY = pos.y - offsetY - j * offsetY * 2;

            GameObject newCard = Instantiate(card, new Vector3(cardPosX, cardPosY, 0), Quaternion.identity);//instanciem una nova carta en la posició calculada
            Resources.Load(Application.dataPath + "/images/Resources/" + SetController.instance.GetSet(list[inc].set).shortname);
            newCard.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(list[inc].image);        //canviem al sprite que ens indica la consulta
            
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
        float ampleTotal = ampleCarta * maxCards; //ample que ocupen les cartes en la fila
        float whiteSpace = ampleCamera - ampleTotal;//espai en blanc que queda

        Camera.main.transform.position = new Vector3(-whiteSpace / 2, CameraInitialPos.y, -10);  //centrem la camera (-whitespace/2 per que es la meitat del espai que queda, i l'afegim a la esquerra)
    }
}

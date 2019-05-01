using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    //sols voldrem una instancia de este objecte i de accés global
    public static CardController instance;

    private TblCards tblCards;

    private void Awake()
    {
        //If we don't currently have a game control...
        if (instance == null)
            //...set this one to be it...
            instance = this;
        //...otherwise...
        else if (instance != this)
            //...destroy this one because it is a duplicate.
            Destroy(gameObject);

        //creem i inicialitzem taula
        tblCards = new TblCards();
    }

    void Start()
    {
        

        //mostrem les cartes per consola
        /*foreach (Card c in tblCards.GetCards())
        {
            Debug.Log(c.ToString());
        }*/
    }

    /// <summary>
    /// Afegeix una carta completa (amb id inclosa)
    /// </summary>
    /// <param name="newCard"></param>
    /// <returns>true si es correcte, false si no</returns>
    public bool AddCard(Card newCard)
    {
        //busquem per si existeix alguna carta amb eixe id
        if (tblCards.GetCard(newCard.id) != null)
        {
            //si existeix ja una carta torna false
            Debug.LogError("Error al insertar nova carta, id ja existeix");

            return false;
        }
        else
        {
            //si no existeix afegeix la nova carta
            tblCards.AddCard(newCard);
        }

        return true;
    }

    public Card GetCard(int id)
    {
        return tblCards.GetCard(id);
    }

    public List<Card> GetCards()
    {
        return tblCards.GetCards();
    }

    public void EmptyTable()
    {
        tblCards.DeleteAllData();
    }
}

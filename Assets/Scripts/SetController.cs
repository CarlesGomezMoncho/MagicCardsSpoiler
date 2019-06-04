using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetController : MonoBehaviour
{
    //sols voldrem una instancia de este objecte i de accés global
    public static SetController instance;

    private TblSets tblSets;

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
        tblSets = new TblSets();
    }

    void Start()
    {
        

        //afegim sets demo
        //AddDemoSets();

        //mostrem els sets per consola
        /*foreach (Set s in tblSets.GetSets())
        {
            Debug.Log(s.ToString());
        }*/

    }


    private void AddDemoSets()
    {
        if (tblSets.GetSet(1) == null)
        {
            tblSets.AddSet(new Set(1, "demoset 1", "DS1", "2019-04-12"));
        }
        if (tblSets.GetSet(2) == null)
        {
            tblSets.AddSet(new Set(2, "demoset 2", "DS2", "2019-04-13"));
        }
        if (tblSets.GetSet(3) == null)
        {
            tblSets.AddSet(new Set(3, "demoset 3", "DS3", "2019-04-14"));
        }
    }

    /// <summary>
    /// Afegeix un set complet (amb id inclós)
    /// </summary>
    /// <param name="newSet"></param>
    /// <returns>true si es correcte, false si no</returns>
    public bool AddSet(Set newSet)
    {
        //busquem per si existeix algun set amb eixe id
        if (tblSets.GetSet(newSet.id) != null)
        {
            //si existeix ja un set torna false
            Debug.LogError("Error al insertar nou set, id ja existeix");

            return false;
        }
        else
        {
            //si no existeix afegeix el nou set
            tblSets.AddSet(newSet);
        }

        return true;
    }

    /// <summary>
    /// Afegeix un set amb id automàtic
    /// </summary>
    /// <param name="name"></param>
    /// <param name="shortName"></param>
    /// <param name="date"></param>
    /// <returns>true si ha pogut afegir el set en la bd</returns>
    public bool AddSet(string name, string shortName, string date)
    {
        //busquem el ultim id de set
        Debug.Log(tblSets.GetLastID());

        return true;
    }

    public Set GetSet(int id)
    {
        return tblSets.GetSet(id);
    }

    public List<Set> GetSets()
    {
        return tblSets.GetSets();
    }

    public void EmptyTable()
    {
        tblSets.DeleteAllData();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateManager: MonoBehaviour
{

    public string urlVersion = "http://stiwi.com/magicSpoilers/Version.txt";
    public string cardsUrl = "http://stiwi.com/magicSpoilers/Cards.csv";
    public string setsUrl = "http://stiwi.com/magicSpoilers/Sets.csv";
    public string imgUrl = "http://stiwi.com/magicSpoilers/img/";

    public GameObject updateAvailiableNotification;
    public GameObject checkUpdateNotification;
    public GameObject updatingNotification;
    public GameObject updateQuestionNotification;
    public GameObject updateCompletedNotification;

    private bool newVersionAvaliable = false;
    private int onlineVersion = 0;

    private bool setsUpdated = false;
    private bool cardsUpdated = false;

    private void Start()
    {
        //al iniciar comprovem si hi ha nova versió de la bd
        //StartCoroutine(CheckForUpdates());
    }


    private void Update()
    {

        //si hi ha actualitzacio disponible, esperem a que haja pasat 5 segons minim
        if (newVersionAvaliable && Time.realtimeSinceStartup >= 5)
        {
            //mostra notificació de que hi ha actualització disponible
            updateAvailiableNotification.SetActive(true);
        }

        //si s'està actualitzant la bd i ja s'ha acabat
        if (setsUpdated && cardsUpdated && updatingNotification.GetComponent<CanvasGroup>().alpha == 1)
        {
            updatingNotification.GetComponent<Animator>().Play("Fade-out");
            updateCompletedNotification.GetComponent<Animator>().Play("Fade-in");
        }

    }

    public void CheckUpdates()
    {
        StartCoroutine(CheckForUpdates());
    }

    private IEnumerator CheckForUpdates()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(urlVersion))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                //pasem lo que revem a un string
                string text = webRequest.downloadHandler.text;

                //intentem convertirlo en int
                if (int.TryParse(webRequest.downloadHandler.text, out int versio))
                {

                    onlineVersion = versio;

                    //comprovem si tenim una versió inferior
                    if (GetCurrentVersionDB() < versio)
                    {
                        //senyalem que hi ha una nova versió disponible
                        //newVersionAvaliable = true;

                        //tanquem notificació de comprovant actualització si esta oberta
                        if (checkUpdateNotification.GetComponent<CanvasGroup>().alpha == 1)
                        {
                            yield return new WaitForSeconds(2f);    //esperem 2 segons antes de tancar per a que primer se desplegue del tot el fade-in
                            checkUpdateNotification.GetComponent<Animator>().Play("Fade-out");
                        }
                        
                        //mostrem finestra de descarregar actualització
                        updateQuestionNotification.GetComponent<Animator>().Play("Fade-in");

                        //posem a fals que les taules estan actualitzades
                        cardsUpdated = false;
                        setsUpdated = false;
                    }
                    else
                    {
                        //ja està actualitzat
                        checkUpdateNotification.GetComponent<Animator>().Play("Fade-out");
                        updateCompletedNotification.GetComponent<Animator>().Play("Fade-in");
                    }
                }
                else
                {
                    Debug.Log("error al convertir el nº de versió: " + webRequest.downloadHandler.text);
                }
            }
        }

    }

    public void UpdateDB()
    {
        updatingNotification.GetComponent<Animator>().Play("Fade-in");
        StartCoroutine(GetRequestSets(setsUrl));
        StartCoroutine(GetRequestCards(cardsUrl));
        //SetCurrentVersionDB(onlineVersion);
    }

    private int GetCurrentVersionDB()
    {
        return PlayerPrefs.GetInt("version", 0);
    }

    private void SetCurrentVersionDB(int versio)
    {
        PlayerPrefs.SetInt("version", versio);
    }

    //esta funció segurament s'haurà de borrar
    /*public void DownloadCards()
    {
        StartCoroutine(GetRequestSets(setsUrl));
        //StartCoroutine(GetRequestCards(cardsUrl));
    }*/

    IEnumerator GetTexture(Card card)
    //public void GetTexture(Card card)
    {
        string url = imgUrl + SetController.instance.GetSet(card.set).shortname + "/" + card.image + ".png";

        Debug.Log(url);

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(uwr);

                byte[] bytes = texture.EncodeToPNG();
                Object.Destroy(texture);

                //creem directori si no existeix
                Directory.CreateDirectory(Application.dataPath + "/images/Resources/" + SetController.instance.GetSet(card.set).shortname);

                //guardem imatge
                File.WriteAllBytes(Application.dataPath + "/images/Resources/" + SetController.instance.GetSet(card.set).shortname + "/" + card.image + ".png", bytes);

                //Debug.Log(Application.dataPath + "/" + SetController.instance.GetSet(card.set).shortname + "/" + card.image + ".png");
            }
        }
    }

    IEnumerator GetRequestSets(string uri)
    {
        //List<Card> setsNous;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);

                //buidem taula actual
                SetController.instance.EmptyTable();

                //creem array de linies
                string[] text = webRequest.downloadHandler.text.Split('\n');
                //Debug.Log(text.Length);

                //creem cada entrada com a un nou objecte i l'afegim a una llista de objectes
                //setsNous = new List<Card>();

                foreach (string line in text)
                {
                    bool comilles = false;
                    char[] lineChar = line.ToCharArray();

                    //substituim cada posible "," dins de comilles dobles per "^" 
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == '"')
                            comilles = !comilles;

                        if (line[i] == ',' && comilles)
                            lineChar[i] = '^';
                    }
                    string newline = new string(lineChar);

                    //separem la linea per comes
                    string[] entries = newline.Split(',');

                    //restituim les ","
                    for (int i = 0; i < entries.Length; i++)
                    {
                        entries[i] = entries[i].Replace("^", ",");
                        entries[i] = entries[i].Trim('"');
                    }

                    //si s'ha pogut separar (hi han 8 paràmetres per a la carta)
                    if (entries.Length == 4)
                    {
                        //els id i set son enters, si fallen se posen a 0
                        int.TryParse(entries[0], out int id);

                        //cada entrada correspon a un parametre del objecte
                        Set newSet = new Set(id, entries[1], entries[2], entries[3]);

                        //Debug.Log(newSet.ToString());

                        //si podem afegir el set
                        if (SetController.instance.AddSet(newSet))
                        {
                            Debug.Log("set " + newSet.id + " afegit correctament");
                        }
                        else
                        {
                            Debug.Log("Set " + newSet.id + " no afegit, probablement ja existeix");
                        }
                    }

                }

                setsUpdated = true;
            }
        }
    }

    IEnumerator GetRequestCards(string uri)
    {
        //List<Card> cartesNoves;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);

                //creem array de linies
                string[] text = webRequest.downloadHandler.text.Split('\n');
                //Debug.Log(text.Length);

                //creem cada entrada com a un nou objecte i l'afegim a una llista de objectes
                //cartesNoves = new List<Card>();

                foreach (string line in text)
                {
                    bool comilles = false;
                    char[] lineChar = line.ToCharArray();

                    //substituim cada posible "," dins de comilles dobles per "^" 
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == '"')
                            comilles = !comilles;

                        if (line[i] == ',' && comilles)
                            lineChar[i] = '^';
                    }
                    string newline = new string(lineChar);

                    //separem la linea per comes
                    string[] entries = newline.Split(',');

                    //restituim les ","
                    for (int i = 0; i < entries.Length; i++)
                    {
                        entries[i] = entries[i].Replace("^", ",");
                        entries[i] = entries[i].Trim('"');
                    }
                    
                    //si s'ha pogut separar (hi han 8 paràmetres per a la carta)
                    if (entries.Length == 8)
                    {
                        //els id i set son enters, si fallen se posen a 0
                        int.TryParse(entries[0], out int id);
                        int.TryParse(entries[3], out int set);

                        //cada entrada correspon a un parametre del objecte
                        Card newCard = new Card(id, entries[1], entries[2], set, entries[4], entries[5], entries[6], entries[7]);
                        StartCoroutine(GetTexture(newCard));
                        Debug.Log(newCard.ToString());
                        //si podem afegir el set
                        if (CardController.instance.AddCard(newCard))
                        {
                            Debug.Log("Carta " + newCard.id + " afegida correctament");
                        }
                        else
                        {
                            Debug.Log("Carta " + newCard.id + " no afegida, probablement ja existeix");
                        }
                        Debug.Log(newCard.ToString());
                    }

                }

                cardsUpdated = true;
            }
        }
    }

    /*private IEnumerator downloadFile()
    {
        UnityWebRequest
        WWW www = new WWW(url);
        loadingScreen.SetActive(true);

        while (!www.isDone)
        {
            //Debug.Log("downloaded " + (www.progress * 100).ToString() + "%...");
            progressBar.value = www.progress;
            progressText.text = www.progress * 100f + "%";
            yield return null;
        }

        //per posar el progres final
        progressBar.value = www.progress;
        progressText.text = www.progress * 100f + "%";
        infoText.text = "Download Completed";
        okButton.gameObject.SetActive(true);

        string fullPath = Application.persistentDataPath + "/" + file;
        File.WriteAllBytes(fullPath, www.bytes);

        load.UpdateTablesFromFile(file);

        //Debug.Log("arxiu descarregat");
    }*/

}

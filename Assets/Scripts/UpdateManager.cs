using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateManager: MonoBehaviour
{

    public string urlVersion = "http://stiwi.com/magicSpoilers/version.txt";
    public string cardsUrl = "http://stiwi.com/magicSpoilers/cards.csv";
    public string setsUrl = "http://stiwi.com/magicSpoilers/sets.csv";
    public string imgUrl = "http://stiwi.com/magicSpoilers/img/";
    public string assetsUrl = "http://stiwi.com/magicSpoilers/assets/";

    public GameObject updateAvailiableNotification;
    public GameObject checkUpdateNotification;
    public GameObject updatingNotification;
    public GameObject updateQuestionNotification;
    public GameObject updateCompletedNotification;

    private bool newVersionAvaliable = false;
    private int onlineVersion = 0;

    private bool setsUpdated = false;
    private bool cardsUpdated = false;
    private bool assetsUpdated = false;

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
        if (setsUpdated && cardsUpdated && assetsUpdated && updatingNotification.GetComponent<CanvasGroup>().alpha == 1)
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
                        assetsUpdated = false;
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
        StartCoroutine(UpdateCoroutines());
    }

    private IEnumerator UpdateCoroutines()
    {
        StartCoroutine(GetRequestCards(cardsUrl));
        yield return StartCoroutine(GetRequestSets(setsUrl));//espera a que estiga la llista de sets, per que els assets la necesita
        StartCoroutine(GetRequestAssets(assetsUrl));
        //SetCurrentVersionDB(onlineVersion);   //COMENTAT PER A QUE SEMPRE ACTUALITZE, QUAN DEIXEM DE FER PROVES S'HA DE DESCOMENTAR, PER A QUE SI JA ESTÀ ACTUALITZAT, NO FASA RES MES
    }

    private int GetCurrentVersionDB()
    {
        return PlayerPrefs.GetInt("version", 0);
    }

    private void SetCurrentVersionDB(int versio)
    {
        PlayerPrefs.SetInt("version", versio);
        PlayerPrefs.Save();     //NO ESTIC SEGUR SI SE FA EN ALGUN ALTRE LLOC O QUE, SIMPLEMENT HE VIST QUE ACI NO ES FEIA I HO HE POSAT, IGUAL ESTA MAL
    }

    //esta funció segurament s'haurà de borrar
    /*public void DownloadCards()
    {
        StartCoroutine(GetRequestSets(setsUrl));
        //StartCoroutine(GetRequestCards(cardsUrl));
    }*/


    //CREC QUE ESTA FUNCIÓ NO ES GASTA, JA QUE SE FA PER ASSETBUNDLE
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

                    //si s'ha pogut separar (hi han 4 paràmetres per al set)
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
                    if (entries.Length == 9)
                    {
                        //els id i set son enters, si fallen se posen a 0
                        int.TryParse(entries[0], out int id);
                        int.TryParse(entries[1], out int cardNum);
                        int.TryParse(entries[4], out int set);

                        //cada entrada correspon a un parametre del objecte
                        Card newCard = new Card(id, cardNum, entries[2], entries[3], set, entries[5], entries[6], entries[7], entries[8]);
                        //StartCoroutine(GetTexture(newCard));  //HE COMENTAT LA CRIDÀ HA ESTA FUNCIÓ, AÇÒ NO ES GASTA?
                        //Debug.Log(newCard.ToString());
                        
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

    IEnumerator GetRequestAssets(string uri)
    {
        //creem dir primer si no existeix
        if (!Directory.Exists(Application.persistentDataPath + "/assetBundles/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/assetBundles/");
        }

        foreach (Set s in SetController.instance.GetSets())
        {
            string fullurl = uri + s.shortname.ToLower();
            Debug.Log("ruta online: " + fullurl);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(fullurl))
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

                    string fullPath = Application.persistentDataPath + "/assetBundles/" + s.shortname.ToLower();
                    File.WriteAllBytes(fullPath, webRequest.downloadHandler.data);
                }
            }
        }

        assetsUpdated = true;
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

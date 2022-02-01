using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;
using System.Globalization;

public class AdvisorSystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData)));
    private Family f_chatMessage = FamilyManager.getFamily(new AllOfComponents(typeof(ChatMessage)));

    public GameObject chatContent;
    private Transform unreadMessages;
    private GameObject playerMessagePrefab;
    private GameObject advisorMessagePrefab;
    private Animator panelAnimator;
    private ScrollRect scrollRect;

    public GameObject simulationData;
    private TimeScale time;
    private FrontierPermeability frontierPermeability;
    private Tax tax;
    private Remoteworking remoteworking;
    private ShortTimeWorking shortTimeWorking;
    private Masks masks;
    private Vaccine vaccine;

    public TMP_Text newChatNotif;

    public AudioSource audioEffect;

    private bool isOpen = false;
    private bool helpPlay = false;
    private float timeFirstNotif = -1;
    private int newMessages = 0;

    private float clickTime = 0;
    private float clickDelay = 0.5f;

    protected override void onStart()
    {
        unreadMessages = chatContent.transform.GetChild(0);
        playerMessagePrefab = chatContent.GetComponent<ChatPrefabs>().PlayerMessage;
        advisorMessagePrefab = chatContent.GetComponent<ChatPrefabs>().AdvisorMessage;
        panelAnimator = chatContent.GetComponentInParent<Animator>();
        scrollRect = chatContent.GetComponentInParent<ScrollRect>();

        // Récupération de l'échelle de temps
        time = simulationData.GetComponent<TimeScale>();
        // Récupération de données de la frontière
        frontierPermeability = simulationData.GetComponent<FrontierPermeability>();
        // Récupération de données des impôts de entreprises
        tax = simulationData.GetComponent<Tax>();
        // Récupération de données du télétravail
        remoteworking = simulationData.GetComponent<Remoteworking>();
        // Récupération de données du chômage partiel
        shortTimeWorking = simulationData.GetComponent<ShortTimeWorking>();
        // Récupération des masques
        masks = simulationData.GetComponent<Masks>();
        // Récupération des données du vaccin
        vaccine = simulationData.GetComponent<Vaccine>();

        f_chatMessage.addEntryCallback(OnNewMessage);
    }

    private void OnNewMessage (GameObject go)
    {
        foreach (ChatMessage cm in go.GetComponents<ChatMessage>())
        {
            GameObject newMessage = Object.Instantiate(advisorMessagePrefab);
            newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += cm.timeStamp;
            newMessage.transform.GetChild(0).Find("Who").GetComponent<TMP_Text>().text = cm.sender;
            newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = cm.messageBody;
            newMessage.transform.SetParent(chatContent.transform);
            newMessage.transform.localScale = new Vector3(1, 1, 1);
            GameObjectManager.removeComponent(cm);
            newMessages++;
        }
        if (timeFirstNotif == -1)
            timeFirstNotif = Time.time;
        audioEffect.Play();
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        if (timeFirstNotif != -1 && Time.time - timeFirstNotif > 15 && time.daysGone == 0 && !helpPlay)
        {
            GameObjectManager.addComponent<ChatMessage>(chatContent, new { sender = "Conseiller au numérique", timeStamp = "0", messageBody = "Pour passer aux jours suivants, cliquez sur le bouton Play." });
            helpPlay = true;
        }

        if (newMessages > 0)
        {
            if (newMessages < 100)
                newChatNotif.text = ""+ newMessages;
            else
                newChatNotif.text = "+99";

            newChatNotif.transform.parent.gameObject.SetActive(true);
        } else
            newChatNotif.transform.parent.gameObject.SetActive(false);

        if (time.newDay)
        {
            //---------------------------------------
            // Gestion des actions au niveau national
            //---------------------------------------
            // Gestion des frontières
            if (frontierPermeability.lastUpdate >= 0 && frontierPermeability.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone-1;
                if (frontierPermeability.currentState == 0)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous avez ouvert les frontières";
                else if (frontierPermeability.currentState == 1)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous avez restreint l'ouverture des frontières seulement au fret mondial et européen.";
                else if (frontierPermeability.currentState == 2)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous avez restreint l'ouverture des frontières seulement au fret européen.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous avez fermé totalement les frontières de la France.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion des aides aux entreprises
            if (tax.lastUpdate >= 0 && tax.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone-1;
                if (tax.currentState)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous annulez les charges des entreprises.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous ne soutenez plus les entreprises.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion du télétravail
            if (remoteworking.lastUpdate >= 0 && remoteworking.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                if (remoteworking.currentState)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous incitez les entreprises à mettre en place un télétravail massif.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous n'incitez plus au télétravail.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion du chômage partiel
            if (shortTimeWorking.lastUpdate >= 0 && shortTimeWorking.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                if (shortTimeWorking.currentState)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous ouvrez les conditions d'accès au chômage partiel.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous restreignez les conditions d'acceptation du chômage partiel.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion de la réquisition des masques
            if (masks.lastRequisitionUpdate >= 0 && masks.lastRequisitionUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                if (masks.requisition)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous réquisitionnez les masques pour le corps médical.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous ne réquisitionnez plus les masques.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion du boost de la production de masques
            if (masks.lastBoostProductionUpdate >= 0 && masks.lastBoostProductionUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                if (masks.boostProduction)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous soutenez toutes les entreprises qui augmentent leur production de masques homologués.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous n'incitez plus à la production de masques.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion de la production artisanale de masques
            if (masks.lastArtisanalProductionUpdate >= 0 && masks.lastArtisanalProductionUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                if (masks.selfProtectionPromoted)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous incitez la population à se confectionner leurs propres masques.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous n'incitez plus la population à se confectionner leurs propres masques.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion de la commande de masques
            if (masks.lastOrderPlaced >= 0 && masks.lastOrderPlaced == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous avez passé une nouvelle commande sur les marchés extérieurs de "+ masks.lastOrderAmount.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + " masques.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion de la commande de vaccins
            if (vaccine.lastOrderPlaced >= 0 && vaccine.lastOrderPlaced == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "Vous avez passé une nouvelle commande sur les marchés extérieurs de " + vaccine.lastOrderAmount.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + " doses de vaccin.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            //------------------------------------------------
            // Gestion des actions au niveau régional/national
            //------------------------------------------------
            foreach (GameObject territory_go in f_territories)
            {
                TerritoryData territory = territory_go.GetComponent<TerritoryData>();
                // Fermeture des écoles
                if (territory.closePrimarySchoolLastUpdate >= 0 && territory.closePrimarySchoolLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    msgContent.text = "Vous avez "+(territory.closePrimarySchool ? "fermé" : "ouvert")+" les écoles maternelles et primaires ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "sur tout le territoire.";
                    else if (territory.TerritoryName == "Mayotte" || territory.TerritoryName == "La Réunion")
                        msgContent.text += "à " + territory.TerritoryName + ".";
                    else
                        msgContent.text += "en " + (territory.TerritoryName == "La Corse" ? "Corse" : territory.TerritoryName) + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Fermeture des collèges
                if (territory.closeSecondarySchoolLastUpdate >= 0 && territory.closeSecondarySchoolLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    msgContent.text = "Vous avez " + (territory.closeSecondarySchool ? "fermé" : "ouvert") + " les collèges ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "sur tout le territoire.";
                    else if (territory.TerritoryName == "Mayotte" || territory.TerritoryName == "La Réunion")
                        msgContent.text += "à " + territory.TerritoryName + ".";
                    else
                        msgContent.text += "en " + (territory.TerritoryName == "La Corse" ? "Corse" : territory.TerritoryName) + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Fermeture des lycées
                if (territory.closeHighSchoolLastUpdate >= 0 && territory.closeHighSchoolLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    msgContent.text = "Vous avez " + (territory.closeHighSchool ? "fermé" : "ouvert") + " les lycées ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "sur tout le territoire.";
                    else if (territory.TerritoryName == "Mayotte" || territory.TerritoryName == "La Réunion")
                        msgContent.text += "à " + territory.TerritoryName + ".";
                    else
                        msgContent.text += "en " + (territory.TerritoryName == "La Corse" ? "Corse" : territory.TerritoryName) + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Fermeture des universités
                if (territory.closeUniversityLastUpdate >= 0 && territory.closeUniversityLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    msgContent.text = "Vous avez " + (territory.closeUniversity ? "fermé" : "ouvert") + " les universités ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "sur tout le territoire.";
                    else if (territory.TerritoryName == "Mayotte" || territory.TerritoryName == "La Réunion")
                        msgContent.text += "à " + territory.TerritoryName + ".";
                    else
                        msgContent.text += "en " + (territory.TerritoryName == "La Corse" ? "Corse" : territory.TerritoryName) + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Appel civique
                if (territory.callCivicismLastUpdate >= 0 && territory.callCivicismLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    if (territory.callCivicism)
                        msgContent.text = "Vous avez appelé à la responsabilité civile ";
                    else
                        msgContent.text = "Vous n'appelez plus à la responsabilité civile ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "sur tout le territoire.";
                    else if (territory.TerritoryName == "Mayotte" || territory.TerritoryName == "La Réunion")
                        msgContent.text += "à " + territory.TerritoryName + ".";
                    else
                        msgContent.text += "en " + (territory.TerritoryName == "La Corse" ? "Corse" : territory.TerritoryName) + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Fermeture des commerces
                if (territory.closeShopLastUpdate >= 0 && territory.closeShopLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    if (territory.closeShop)
                        msgContent.text = "Vous ordonnez la fermeture des commerces ";
                    else
                        msgContent.text = "Vous autorisez la réouverture des commerces ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "sur tout le territoire.";
                    else if (territory.TerritoryName == "Mayotte" || territory.TerritoryName == "La Réunion")
                        msgContent.text += "à " + territory.TerritoryName + ".";
                    else
                        msgContent.text += "en " + (territory.TerritoryName == "La Corse" ? "Corse" : territory.TerritoryName) + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Attestation de sortie
                if (territory.certificateRequiredLastUpdate >= 0 && territory.certificateRequiredLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    if (territory.certificateRequired)
                        msgContent.text = "Vous limitez le déplacement des citoyens ";
                    else
                        msgContent.text = "Vous autorisez les citoyens à se déplacer librement ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "sur tout le territoire.";
                    else if (territory.TerritoryName == "Mayotte" || territory.TerritoryName == "La Réunion")
                        msgContent.text += "à " + territory.TerritoryName + ".";
                    else
                        msgContent.text += "en " + (territory.TerritoryName == "La Corse" ? "Corse" : territory.TerritoryName) + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Restriction age
                if (territory.ageDependentLastUpdate >= 0 && territory.ageDependentLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    if (territory.ageDependent)
                        msgContent.text = "Vous interdisez aux citoyens âgés de " + territory.ageDependentMin +" à "+territory.ageDependentMax + " ans de sortir ";
                    else
                        msgContent.text = "Vous n'imposez plus d'interdiction de sortie liée à l'âge ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "sur tout le territoire.";
                    else if (territory.TerritoryName == "Mayotte" || territory.TerritoryName == "La Réunion")
                        msgContent.text += "à " + territory.TerritoryName + ".";
                    else
                        msgContent.text += "en " + (territory.TerritoryName == "La Corse" ? "Corse" : territory.TerritoryName) + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Boost lit réanimation
                Beds beds = territory.GetComponent<Beds>();
                if (beds.boostBedsLastUpdate >= 0 && beds.boostBedsLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    if (beds.boostBeds)
                        msgContent.text = "Vous augmentez le nombre de lits de réanimation dans les services de soin ";
                    else
                        msgContent.text = "Vous stoppez l'augmentation du nombre de lits de réanimation ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "sur tout le territoire.";
                    else if (territory.TerritoryName == "Mayotte" || territory.TerritoryName == "La Réunion")
                        msgContent.text += "à " + territory.TerritoryName + ".";
                    else
                        msgContent.text += "en " + (territory.TerritoryName == "La Corse" ? "Corse" : territory.TerritoryName) + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
    }

    public void TogglePanel()
    {
        if (isOpen)
            ClosePanel();
        else
        {
            panelAnimator.SetTrigger("Open");
            isOpen = true;
            clickTime = Time.time;
            scrollRect.verticalNormalizedPosition = 0;
        }
    }

    public void ClosePanel()
    {
        if (isOpen)
        {
            panelAnimator.SetTrigger("Close");
            isOpen = false;
            if (Time.time - clickTime > clickDelay)
            {
                unreadMessages.SetAsLastSibling();
                newMessages = 0;
            }
        }
    }
}
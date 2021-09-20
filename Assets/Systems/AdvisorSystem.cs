using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;
using System.Globalization;

public class AdvisorSystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData)));
    private Family f_chatMessage = FamilyManager.getFamily(new AllOfComponents(typeof(ChatMessage)));

    private GameObject chatContent;
    private Transform unreadMessages;
    private GameObject playerMessagePrefab;
    private GameObject advisorMessagePrefab;
    private Animator panelAnimator;
    private ScrollRect scrollRect;

    private AudioSource audioEffect;

    private TMP_Text newNotif;

    private TimeScale time;
    private FrontierPermeability frontierPermeability;
    private Tax tax;
    private Remoteworking remoteworking;
    private ShortTimeWorking shortTimeWorking;
    private Masks masks;
    private Vaccine vaccine;

    private bool isOpen = false;
    private bool helpPlay = false;
    private float timeFirstNotif = -1;
    private int newMessages = 0;

    private float clickTime = 0;
    private float clickDelay = 0.5f;

    public AdvisorSystem()
    {
        chatContent = GameObject.Find("ChatContent");
        unreadMessages = chatContent.transform.GetChild(0);
        playerMessagePrefab = chatContent.GetComponent<ChatPrefabs>().PlayerMessage;
        advisorMessagePrefab = chatContent.GetComponent<ChatPrefabs>().AdvisorMessage;
        panelAnimator = chatContent.GetComponentInParent<Animator>();
        scrollRect = chatContent.GetComponentInParent<ScrollRect>();

        GameObject simu = GameObject.Find("SimulationData");
        // Récupération de l'échelle de temps
        time = simu.GetComponent<TimeScale>();
        // Récupération de données de la frontière
        frontierPermeability = simu.GetComponent<FrontierPermeability>();
        // Récupération de données des impôts de entreprises
        tax = simu.GetComponent<Tax>();
        // Récupération de données du télétravail
        remoteworking = simu.GetComponent<Remoteworking>();
        // Récupération de données du chômage partiel
        shortTimeWorking = simu.GetComponent<ShortTimeWorking>();
        // Récupération des masques
        masks = simu.GetComponent<Masks>();
        // Récupération des données du vaccin
        vaccine = simu.GetComponent<Vaccine>();

        newNotif = GameObject.Find("ChatNotification").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();

        audioEffect = GameObject.Find("AudioEffects").GetComponent<AudioSource>();

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
            GameObjectManager.addComponent<ChatMessage>(chatContent, new { sender = "Digital advisor", timeStamp = "0", messageBody = "To move to the next day, click on Play button." });
            helpPlay = true;
        }

        if (newMessages > 0)
        {
            if (newMessages < 100)
                newNotif.text = ""+ newMessages;
            else
                newNotif.text = "+99";

            newNotif.transform.parent.gameObject.SetActive(true);
        } else
            newNotif.transform.parent.gameObject.SetActive(false);

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
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You opened up the frontiers.";
                else if (frontierPermeability.currentState == 1)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You restricted the opening of frontiers only to global and European freight.";
                else if (frontierPermeability.currentState == 2)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You restricted the opening of frontiers only to European freight.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You closed the frontiers of France.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion des aides aux entreprises
            if (tax.lastUpdate >= 0 && tax.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone-1;
                if (tax.currentState)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You cancel the business charges.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You no longer support businesses.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion du télétravail
            if (remoteworking.lastUpdate >= 0 && remoteworking.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                if (remoteworking.currentState)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You encourage companies to set up massive teleworking.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You no longer encourage teleworking.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion du chômage partiel
            if (shortTimeWorking.lastUpdate >= 0 && shortTimeWorking.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                if (shortTimeWorking.currentState)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You authorize short-time working.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You limit short-time working.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion de la réquisition des masques
            if (masks.lastRequisitionUpdate >= 0 && masks.lastRequisitionUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                if (masks.requisition)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You requisition the masks for the medical profession.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You no longer requisition masks.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion du boost de la production de masques
            if (masks.lastBoostProductionUpdate >= 0 && masks.lastBoostProductionUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                if (masks.boostProduction)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You support all companies that increase their production of approved masks.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You no longer encourage the production of masks.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion de la production artisanale de masques
            if (masks.lastArtisanalProductionUpdate >= 0 && masks.lastArtisanalProductionUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                if (masks.selfProtectionPromoted)
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You encourage the population to make their own masks.";
                else
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You no longer encourage the population to make their own masks.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion de la commande de masques
            if (masks.lastOrderPlaced >= 0 && masks.lastOrderPlaced == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You have ordered " + masks.lastOrderAmount.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + " masks.";
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Gestion de la commande de vaccins
            if (vaccine.lastOrderPlaced >= 0 && vaccine.lastOrderPlaced == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "You have ordered " + vaccine.lastOrderAmount.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + " doses of vaccine.";
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
                    msgContent.text = "You "+(territory.closePrimarySchool ? "closed" : "opened")+ " nursery and primary schools ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "throughout the country.";
                    else
                        msgContent.text += "in " + territory.TerritoryName + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Fermeture des collèges
                if (territory.closeSecondarySchoolLastUpdate >= 0 && territory.closeSecondarySchoolLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    msgContent.text = "You " + (territory.closeSecondarySchool ? "closed" : "opened") + " middle Schools ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "throughout the country.";
                    else
                        msgContent.text += "in " + territory.TerritoryName + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Fermeture des lycées
                if (territory.closeHighSchoolLastUpdate >= 0 && territory.closeHighSchoolLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    msgContent.text = "You " + (territory.closeHighSchool ? "closed" : "opened") + " high schools ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "throughout the country.";
                    else
                        msgContent.text += "in " + territory.TerritoryName + ".";
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Fermeture des universités
                if (territory.closeUniversityLastUpdate >= 0 && territory.closeUniversityLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.GetChild(0).Find("Timestamp").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    msgContent.text = "You " + (territory.closeUniversity ? "closed" : "opened") + " universities ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "throughout the country.";
                    else
                        msgContent.text += "in " + territory.TerritoryName + ".";
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
                        msgContent.text = "You called for civil liability ";
                    else
                        msgContent.text = "You no longer call for civil liability ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "throughout the country.";
                    else
                        msgContent.text += "in " + territory.TerritoryName + ".";
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
                        msgContent.text = "You order the shops to be closed ";
                    else
                        msgContent.text = "You authorize the reopening of shops ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "throughout the country.";
                    else
                        msgContent.text += "in " + territory.TerritoryName + ".";
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
                        msgContent.text = "You limit the movement of citizens ";
                    else
                        msgContent.text = "You allow citizens to move freely ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "throughout the country.";
                    else
                        msgContent.text += "in " + territory.TerritoryName + ".";
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
                        msgContent.text = "You forbid citizens aged " + territory.ageDependentMin +" to "+territory.ageDependentMax + " to go out ";
                    else
                        msgContent.text = "You no longer impose an age-related exit ban ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "throughout the country.";
                    else
                        msgContent.text += "in " + territory.TerritoryName + ".";
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
                        msgContent.text = "You increase the number of intensive care beds ";
                    else
                        msgContent.text = "You no longer increase the number of intensive care beds ";
                    if (territory.TerritoryName == "France")
                        msgContent.text += "throughout the country.";
                    else
                        msgContent.text += "in " + territory.TerritoryName + ".";
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
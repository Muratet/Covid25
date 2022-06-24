using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;

/// <summary>
/// This system is in charge to display messages provided by advisors in the advisor panel
/// </summary>
public class AdvisorSystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData)));
    private Family f_chatMessage = FamilyManager.getFamily(new AllOfComponents(typeof(ChatMessage)));

    /// <summary>
    /// The panel that contains advisors messages
    /// </summary>
    public GameObject chatContent;
    private Transform unreadMessages;
    private GameObject playerMessagePrefab;
    private GameObject advisorMessagePrefab;
    private Animator panelAnimator;
    private ScrollRect scrollRect;

    /// <summary></summary>
    public GameObject simulationData;
    private TimeScale time;
    private FrontierPermeability frontierPermeability;
    private Tax tax;
    private Remoteworking remoteworking;
    private ShortTimeWorking shortTimeWorking;
    private Masks masks;
    private Vaccine vaccine;

    /// <summary></summary>
    public TMP_Text newChatNotif;

    /// <summary></summary>
    public AudioSource audioEffect;

    /// <summary></summary>
    public Localization localization;

    private bool isOpen = false;
    private bool helpPlay = false;
    private float timeFirstNotif = -1;
    private int newMessages = 0;

    private float clickTime = 0;
    private float clickDelay = 0.5f;

    /// <summary>
    /// Singleton reference of this system
    /// </summary>
    public static AdvisorSystem instance;

    /// <summary>
    /// Construct this system
    /// </summary>
    public AdvisorSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        unreadMessages = chatContent.transform.GetChild(0);
        playerMessagePrefab = chatContent.GetComponent<ChatPrefabs>().PlayerMessage;
        advisorMessagePrefab = chatContent.GetComponent<ChatPrefabs>().AdvisorMessage;
        panelAnimator = chatContent.GetComponentInParent<Animator>();
        scrollRect = chatContent.GetComponentInParent<ScrollRect>();

        // Recovery of the time scale
        time = simulationData.GetComponent<TimeScale>();
        // Recovery of the border restrictions
        frontierPermeability = simulationData.GetComponent<FrontierPermeability>();
        // Recovery of the company restrictions
        tax = simulationData.GetComponent<Tax>();
        // Recovery of home working restrictions
        remoteworking = simulationData.GetComponent<Remoteworking>();
        // Recovery of partial unemployment restrictions
        shortTimeWorking = simulationData.GetComponent<ShortTimeWorking>();
        // Recovery of mask data
        masks = simulationData.GetComponent<Masks>();
        // Recovery of vaccine data
        vaccine = simulationData.GetComponent<Vaccine>();

        f_chatMessage.addEntryCallback(OnNewMessage);
    }

    private void OnNewMessage (GameObject go)
    {
        foreach (ChatMessage cm in go.GetComponents<ChatMessage>())
        {
            GameObject newMessage = Object.Instantiate(advisorMessagePrefab);
            newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += cm.timeStamp;
            newMessage.transform.Find("Header/Who").GetComponent<TMP_Text>().text = cm.sender;
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
            GameObjectManager.addComponent<ChatMessage>(chatContent, new { sender = localization.advisorTitleDigital, timeStamp = "0", messageBody = localization.advisorDigitalTexts[0] });
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
            //--------------------------------------------
            // Management of actions at the national level
            //--------------------------------------------
            // Border management
            if (frontierPermeability.lastUpdate >= 0 && frontierPermeability.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone-1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = localization.borderActions[frontierPermeability.currentState];
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Management of aid to companies
            if (tax.lastUpdate >= 0 && tax.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone-1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = localization.businessActions[tax.currentState ? 0 : 1];
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Management of home working
            if (remoteworking.lastUpdate >= 0 && remoteworking.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = localization.homeWorkingActions[remoteworking.currentState ? 0 : 1];
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Management of partial unemployment
            if (shortTimeWorking.lastUpdate >= 0 && shortTimeWorking.lastUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = localization.partialUnemploymentActions[shortTimeWorking.currentState ? 0 : 1];
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Management of the mask requisition
            if (masks.lastRequisitionUpdate >= 0 && masks.lastRequisitionUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = localization.maskRequisitionActions[masks.requisition ? 0 : 1];
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Management of the mask production boost
            if (masks.lastBoostProductionUpdate >= 0 && masks.lastBoostProductionUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = localization.maskNationalActions[masks.boostProduction ? 0 : 1];
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Management of the artisanal production of masks
            if (masks.lastArtisanalProductionUpdate >= 0 && masks.lastArtisanalProductionUpdate == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = localization.maskArtisanalActions[masks.selfProtectionPromoted ? 0 : 1];
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Mask order management
            if (masks.lastOrderPlaced >= 0 && masks.lastOrderPlaced == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = localization.getFormatedText(localization.maskOrderActions, masks.lastOrderAmount.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo));
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            // Vaccine order management
            if (vaccine.lastOrderPlaced >= 0 && vaccine.lastOrderPlaced == time.daysGone - 1)
            {
                GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = localization.getFormatedText(localization.vaccineOrderActions, vaccine.lastOrderAmount.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo));
                newMessage.transform.SetParent(chatContent.transform);
                newMessage.transform.localScale = new Vector3(1, 1, 1);
            }

            //-------------------------------------------------
            // Management of actions at regional/national level
            //-------------------------------------------------
            foreach (GameObject territory_go in f_territories)
            {
                TerritoryData territory = territory_go.GetComponent<TerritoryData>();
                // Primary schools closure
                if (territory.closePrimarySchoolLastUpdate >= 0 && territory.closePrimarySchoolLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "(" + territory.TerritoryName + ") "+localization.getFormatedText(localization.primarySchoolActions, territory.closePrimarySchool ? localization.closed : localization.opened);
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Middle schools closure
                if (territory.closeSecondarySchoolLastUpdate >= 0 && territory.closeSecondarySchoolLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "(" + territory.TerritoryName + ") " + localization.getFormatedText(localization.middleSchoolActions, territory.closeSecondarySchool ? localization.closed : localization.opened);
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // High schools closure
                if (territory.closeHighSchoolLastUpdate >= 0 && territory.closeHighSchoolLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "(" + territory.TerritoryName + ") " + localization.getFormatedText(localization.highSchoolActions, territory.closeHighSchool ? localization.closed : localization.opened);
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Universities closure
                if (territory.closeUniversityLastUpdate >= 0 && territory.closeUniversityLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "(" + territory.TerritoryName + ") " + localization.getFormatedText(localization.universityActions, territory.closeUniversity ? localization.closed : localization.opened);
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Call for civic responsibility
                if (territory.callCivicismLastUpdate >= 0 && territory.callCivicismLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "(" + territory.TerritoryName + ") " + localization.civicActions[territory.callCivicism ? 0 : 1];
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // shops closure
                if (territory.closeShopLastUpdate >= 0 && territory.closeShopLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "(" + territory.TerritoryName + ") " + localization.closeShopActions[territory.closeShop ? 0 : 1];
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Exit certificate required
                if (territory.certificateRequiredLastUpdate >= 0 && territory.certificateRequiredLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "(" + territory.TerritoryName + ") " + localization.certificateActions[territory.certificateRequired ? 0 : 1];
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Boost the number of intensive care beds
                Beds beds = territory.GetComponent<Beds>();
                if (beds.boostBedsLastUpdate >= 0 && beds.boostBedsLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    newMessage.transform.Find("TextContent").GetComponent<TMP_Text>().text = "(" + territory.TerritoryName + ") " + localization.boostBedActions[beds.boostBeds ? 0 : 1];
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
                // Age restriction
                if (territory.ageDependentLastUpdate >= 0 && territory.ageDependentLastUpdate == time.daysGone - 1)
                {
                    GameObject newMessage = Object.Instantiate(playerMessagePrefab);
                    newMessage.transform.Find("Header/Timestamp/Value").GetComponent<TMP_Text>().text += time.daysGone - 1;
                    TMP_Text msgContent = newMessage.transform.Find("TextContent").GetComponent<TMP_Text>();
                    msgContent.text = "(" + territory.TerritoryName + ") ";
                    if (territory.ageDependent)
                        msgContent.text += localization.getFormatedText(localization.ageDependentActions[0], territory.ageDependentMin, territory.ageDependentMax);
                    else
                        msgContent.text += localization.ageDependentActions[1];
                    newMessage.transform.SetParent(chatContent.transform);
                    newMessage.transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
    }

    /// <summary>
    /// Open/Close the advisor panel
    /// </summary>
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

    /// <summary>
    /// Close the advisor panel if it is openned
    /// </summary>
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
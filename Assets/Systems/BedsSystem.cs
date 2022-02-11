using UnityEngine;
using FYFY;
using TMPro;
using System.Globalization;

public class BedsSystem : FSystem
{
    private Family f_beds = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Beds)));

    public GameObject countrySimData;
    private TimeScale time;
    private VirusStats virusStats;

    public static BedsSystem instance;

    public BedsSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        // Récupération de l'échelle de temps
        time = countrySimData.GetComponent<TimeScale>();
        // Récupération des stats du virus
        virusStats = countrySimData.GetComponent<VirusStats>();
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        if (time.newDay)
        {
            foreach (GameObject bedsGO in f_beds)
            {
                Beds beds = bedsGO.GetComponent<Beds>();
                TerritoryData territory = bedsGO.GetComponent<TerritoryData>();

                if (beds.boostBeds)
                {
                    int newBeds = (beds.intensiveBeds_high - beds.intensiveBeds_current) / 8; // Le 8 permet de ralentir la croissance de la production (le temps de construire/acheter des respirateurs de réorganiser les services, de réquisitionner les hôpitaux et clinique privées...)
                    beds.intensiveBeds_current += newBeds;
                }

                // calcul du nombre de lits utilisés : comptabilisation du nombre de personne qui devraient pouvoir accéder à des lits de réanimation, on prend un fenêtre de 9 jours autour du pic de mortalité (même calcul fait dans DeadSystem)
                int criticAmount = 0;
                for (int day = Mathf.Max(0, (int)virusStats.deadlinessPeak - 4); day < Mathf.Min(virusStats.deadlinessPeak + 5, territory.numberOfInfectedPeoplePerDays.Length); day++)
                    criticAmount = (int)(territory.numberOfInfectedPeoplePerDays[day] * virusStats.seriousRatio);

                beds.intensiveBeds_need = criticAmount;

                if (beds.intensiveBeds_need > beds.intensiveBeds_current && beds.advisorNotification == -1 && territory.TerritoryName != "France")
                {
                    string msgBody = "Attention les hopitaux ";
                    if (territory.TerritoryName == "Mayotte" || territory.TerritoryName == "La Réunion")
                        msgBody += "à " + territory.TerritoryName;
                    else
                        msgBody += "en " + (territory.TerritoryName == "La Corse" ? "Corse" : territory.TerritoryName);
                    msgBody += " sont dépassés. Il y a trop de cas malades par rapport au nombre de lits de réanimation.";

                    GameObjectManager.addComponent<ChatMessage>(beds.gameObject, new { sender = "Fédération des hôpitaux", timeStamp = "" + time.daysGone, messageBody = msgBody });
                    beds.advisorNotification = 0;
                }
                else if (beds.intensiveBeds_need < beds.intensiveBeds_current && beds.advisorNotification > 10)
                    beds.advisorNotification = -1;
                else if (beds.advisorNotification != -1)
                    beds.advisorNotification++;

            }
        }
	}

    public void UpdateBedsUI(TMP_Text textUI)
    {
        Beds beds = MapSystem.territorySelected.GetComponent<Beds>();

        // Mise à jour du texte et de la couleur de l'utilisation des lits
        textUI.text = beds.intensiveBeds_need.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo) + " / " + beds.intensiveBeds_current.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
        textUI.color = new Color(2 * (float)beds.intensiveBeds_need / beds.intensiveBeds_current, 2 * (1f - (float)beds.intensiveBeds_need / beds.intensiveBeds_current), 0f);
    }
}
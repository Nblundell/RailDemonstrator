using UnityEngine;
using TMPro;

public class TextGUI : MonoBehaviour
{
    [SerializeField] GameObject fareMoneyCounter;
    [SerializeField] GameObject lostMoneyCounter;
    [SerializeField] GameObject peopleInjuredCounter;
    private double farelostMoney = 0;
    private double lostMoney = 0;
    private double money = 0;
    private int peopleInjured = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //handles text ui based on money
    void Start()
    {
        fareMoneyCounter.GetComponent<TMP_Text>().text = "£0";
        lostMoneyCounter.GetComponent<TMP_Text>().text = "£0";
        //MoneyCounter.GetComponent<TMP_Text>().text = "0";
    }
    public void addLostFareMoney(double moneyLost)
    {
        //adds money
        farelostMoney = farelostMoney + moneyLost;
        fareMoneyCounter.GetComponent<TMP_Text>().text = "£" + farelostMoney.ToString();
    }
    public void addLostMoney(double moneyLost)
    {
       
        lostMoney = lostMoney + moneyLost;
        if (lostMoney > 1000 && lostMoney < 1000000)
        {
            lostMoneyCounter.GetComponent<TMP_Text>().text = "£" + (lostMoney / 1000).ToString() + "K";
        }
        else if (lostMoney > 1000000)
        {
            lostMoneyCounter.GetComponent<TMP_Text>().text = "£" + (lostMoney / 1000000).ToString() + " Mil";
        }
        else
        {
            lostMoneyCounter.GetComponent<TMP_Text>().text = "£" + lostMoney.ToString();
        }
    }
    
    public void crash(int people)
    {
        Debug.Log(people);
        peopleInjured = peopleInjured + people;
        peopleInjuredCounter.GetComponent<TMP_Text>().text = peopleInjured.ToString();
        double temp = people * 30000 + 1000000;
        addLostMoney(temp);
    }
    // Update is called once per frame

}

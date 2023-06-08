using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{
    public int playerMoney;
    public int lemonsInventory;
    public int sugarInventory;
    public int currentDayNumber;
    public int currentDayIndex;
    public int rentDue;
    public int waterBillAmount;
    public int lemonsMarketPrice;
    public int sugarMarketPrice;
    public int playerPopularity;
    public int currentSceneIndex;
}

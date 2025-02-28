using UnityEngine;

namespace Gamepangin
{
    [CreateAssetMenu(fileName = "New Currency", menuName = "Gamepangin/Currency")]
    public class Currency : DataDefinition<Currency>
    {
        [SerializeField] private string currencyName;
        public string CurrencyName => currencyName;
        [SerializeField] private Sprite icon;
    }
}

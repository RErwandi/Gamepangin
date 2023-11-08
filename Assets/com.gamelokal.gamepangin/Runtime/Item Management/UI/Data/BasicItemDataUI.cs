using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamepangin.UI
{
    public class BasicItemDataUI : DataInfoUI<Item>
    {
        [SerializeField] private Image iconImg;
        [SerializeField] private TextMeshProUGUI stackTxt;
        
        protected override bool CanEnableInfo()
        {
            if (iconImg != null)
                iconImg.enabled = true;
            
            return true;
        }

        protected override void OnInfoUpdate()
        {
            var itemInfo = data.Definition;

            if (stackTxt != null)
            {
                stackTxt.text = data.StackCount.ToString();
            }

            if (iconImg != null)
            {
                iconImg.sprite = itemInfo.Icon;
            }
        }

        protected override void OnInfoDisabled()
        {
            if (stackTxt != null)
                stackTxt.text = string.Empty;

            if (iconImg != null)
                iconImg.enabled = false;
        }
    }
}
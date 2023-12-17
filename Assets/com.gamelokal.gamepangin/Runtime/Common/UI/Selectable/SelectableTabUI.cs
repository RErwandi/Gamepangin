using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gamepangin.UI
{
    [AddComponentMenu("Gamepangin/UI/Selectables/Selectable Tab")]
    public class SelectableTabUI : SelectableUI
    {
        #region Internal
        [System.Flags]
        protected enum SelectMode
        {
            None = 0,
            EnablePanel = 1,
            EnableObject = 2,
            Everything = EnablePanel | EnableObject
        }
        #endregion

        public string TabName
        {
            get
            {
                if (nameText == null)
                    return string.Empty;

                return nameText.text;
            }
            set
            {
                if (nameText != null)
                    nameText.text = value;
            }
        }

        [SerializeField]
        protected TextMeshProUGUI nameText;
        
        [SerializeField]
        protected SelectMode selectMode;

        [SerializeField, ShowIf(nameof(selectMode), SelectMode.EnablePanel)]
        protected PanelUI panelToEnable;

        [SerializeField, ShowIf(nameof(selectMode), SelectMode.EnableObject)]
        protected GameObject objectToEnable;


        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            if (isSelected)
            {
                if ((selectMode & SelectMode.EnablePanel) == SelectMode.EnablePanel)
                    panelToEnable.Show(true);

                if ((selectMode & SelectMode.EnableObject) == SelectMode.EnableObject)
                    objectToEnable.SetActive(true);
            }
        }

        public override void Deselect()
        {
            if (!isSelected)
                return;

            base.Deselect();

            if ((selectMode & SelectMode.EnablePanel) == SelectMode.EnablePanel)
                panelToEnable.Show(false);

            if ((selectMode & SelectMode.EnableObject) == SelectMode.EnableObject)
                objectToEnable.SetActive(false);

            onDeselected.Invoke(this);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (nameText == null)
                nameText = GetComponentInChildren<TextMeshProUGUI>();
        }
#endif
    }
}

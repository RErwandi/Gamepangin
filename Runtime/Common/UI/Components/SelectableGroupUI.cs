using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin.UI
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Gamepangin/UI/Selectable Group")]
    public class SelectableGroupUI : SelectableGroupBaseUI
    {
        #region Internal
        protected enum SelectableRegisterMode
        {
            None = 0,
            Disable = 2,
        }
        #endregion

        public override List<SelectableUI> RegisteredSelectables => selectables;

        public override SelectableUI Selected => selected;
        public override SelectableUI Highlighted => highlighted;

        public override event UnityAction<SelectableUI> SelectedChanged;
        public override event UnityAction<SelectableUI> HighlightedChanged;

        [SerializeField]
        private SelectableRegisterMode selectableRegisterMode = SelectableRegisterMode.None;

        protected readonly List<SelectableUI> selectables = new();
        protected SelectableUI selected;
        protected SelectableUI highlighted;


        public override void RegisterSelectable(SelectableUI selectable)
        {
            if (selectable == null)
                return;

            selectables.Add(selectable);

            if (selectableRegisterMode == SelectableRegisterMode.Disable)
            {
                DisableSelectable(selectable);
                return;
            }
        }

        public override void UnregisterSelectable(SelectableUI selectable)
        {
            if (selectable == null)
                return;

            selectables.Remove(selectable);
        }

        public override void SelectSelectable(SelectableUI selectable)
        {
            if (selectable == selected)
                return;

            var prevSelectable = selected;
            selected = selectable;

            if (prevSelectable != null)
                prevSelectable.Deselect();

            if (selectable != null)
                selectable.Select();

            OnSelectedChanged(selectable);
            SelectedChanged?.Invoke(selectable);
        }

        public override void HighlightSelectable(SelectableUI selectable)
        {
            highlighted = selectable;
            HighlightedChanged?.Invoke(selectable);
        }

        protected virtual void OnSelectedChanged(SelectableUI selectable) { }
    }
}

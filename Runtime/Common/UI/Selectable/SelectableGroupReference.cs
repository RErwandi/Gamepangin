using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin.UI
{
    [AddComponentMenu("Gamepangin/UI/Selectable Group Reference")]
    public class SelectableGroupReferenceUI : SelectableGroupBaseUI
    {
        public override SelectableUI Selected => referencedGroup.Selected;
        public override SelectableUI Highlighted => referencedGroup.Highlighted;
        public override List<SelectableUI> RegisteredSelectables => referencedGroup.RegisteredSelectables;

        public override event UnityAction<SelectableUI> SelectedChanged
        {
            add => referencedGroup.SelectedChanged += value;
            remove => referencedGroup.SelectedChanged -= value;
        }

        public override event UnityAction<SelectableUI> HighlightedChanged
        {
            add => referencedGroup.HighlightedChanged += value;
            remove => referencedGroup.HighlightedChanged -= value;
        }

        [SerializeField, Required]
        private SelectableGroupUI referencedGroup;


        public override void RegisterSelectable(SelectableUI selectable) => referencedGroup.RegisterSelectable(selectable);
        public override void UnregisterSelectable(SelectableUI selectable) => referencedGroup.UnregisterSelectable(selectable);
        public override void SelectSelectable(SelectableUI selectable) => referencedGroup.SelectSelectable(selectable);
        public override void HighlightSelectable(SelectableUI selectable) => referencedGroup.HighlightSelectable(selectable);
    }
}
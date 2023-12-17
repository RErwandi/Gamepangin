using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Gamepangin.UI
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [AddComponentMenu("Gamepangin/UI/Selectable")]
    public class SelectableUI : UIBehaviour, IMoveHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
    {
        #region Internal
        /// <summary>
        /// An enumeration of selected states of objects
        /// </summary>
        protected enum SelectionState
        {
            /// <summary>
            /// The UI object can be selected.
            /// </summary>
            Normal,

            /// <summary>
            /// The UI object is highlighted.
            /// </summary>
            Highlighted,

            /// <summary>
            /// The UI object is pressed.
            /// </summary>
            Pressed,

            /// <summary>
            /// The UI object is selected
            /// </summary>
            Selected,
        }
        
        [Serializable]
        protected sealed class SelectedEvent : UnityEvent<SelectableUI> { }
        #endregion

        public event UnityAction<SelectableUI> OnSelected
        {
            add => onSelected.AddListener(value); 
            remove => onSelected.RemoveListener(value);
        }
        
        public event UnityAction<SelectableUI> OnDeselected
        {
            add => onDeselected.AddListener(value);
            remove => onDeselected.RemoveListener(value);
        }

        /// <summary>
        /// The Navigation setting for this selectable object.
        /// </summary>
        public NavigationUI Navigation
        {
            get => navigation;
            set { if (SetStruct(ref navigation, value)) DoStateTransition(CurrentSelectionState, false); }
        }

        /// <summary>
        /// Is this object selectable.
        /// </summary>
        public bool IsSelectable
        {
            get => isSelectable && isInteractable;
            set
            {
                if (value != isSelectable)
                {
                    isSelectable = value;
                    DoStateTransition(CurrentSelectionState, false);
                }
            }
        }

        protected SelectionState CurrentSelectionState
        {
            get
            {
                if (isPointerDown)
                    return SelectionState.Pressed;
                if (hasParentGroup && parentGroup.Selected == this)
                    return SelectionState.Selected;
                if (isPointerInside)
                    return SelectionState.Highlighted;
                return SelectionState.Normal;
            }
        }

        [Tooltip("Can this object be selected?")]
        [SerializeField]
        protected bool isSelectable = true;

        // Navigation information.
        [SerializeField]
        protected NavigationUI navigation = NavigationUI.DefaultNavigation;

        [SerializeReference]
        protected IInteractableFeedbackUI[] feedbacks = Array.Empty<IInteractableFeedbackUI>();

        [SerializeField]
        protected SelectedEvent onSelected;
        
        [SerializeField]
        protected SelectedEvent onDeselected;

        protected static SelectableUI[] selectables = new SelectableUI[10];
        protected static int selectableCount = 0;
        protected SelectableGroupBaseUI parentGroup;
        protected bool hasParentGroup;
        protected bool enableCalled = false;
        protected int currentIndex = -1;

        protected bool isSelected;
        protected bool isPointerDown;
        protected bool isPointerInside;
        protected bool isInteractable = true;


        protected SelectableUI()
        { }

        /// <summary>
        /// Selects this Selectable.
        /// </summary>
        public void Select()
        {
            if (!isSelectable || !isInteractable)
                return;

            if (EventSystem.current != null && !EventSystem.current.alreadySelecting)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }

        /// <summary>
        /// Deselects this Selectable.
        /// </summary>
        public virtual void Deselect()
        {
            isSelected = false;
            EvaluateAndTransitionToSelectionState();
            
            onDeselected.Invoke(this);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            if (!isInteractable)
                return;

            // Only select if there's a parent group.
            if (hasParentGroup && !isSelected && isSelectable)
            {
                isSelected = true;
                parentGroup.SelectSelectable(this);
                EvaluateAndTransitionToSelectionState();
            }

            onSelected.Invoke(this);
        }

        /// <summary>
        /// Evaluate current state and transition to pressed state.
        /// </summary>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isPointerDown = true;
            EvaluateAndTransitionToSelectionState();
        }

        /// <summary>
        /// Evaluate eventData and transition to appropriate state.
        /// </summary>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            // Selection tracking
            if (isPointerInside && isInteractable)
                Select();

            isPointerDown = false;
            EvaluateAndTransitionToSelectionState();
        }

        /// <summary>
        /// Evaluate current state and transition to appropriate state.
        /// New state could be pressed or hover depending on pressed state.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;

            if (!isSelected)
                EvaluateAndTransitionToSelectionState();

            if (hasParentGroup)
                parentGroup.HighlightSelectable(this);
        }

        /// <summary>
        /// Evaluate current state and transition to normal state.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;

            if (!isSelected)
                EvaluateAndTransitionToSelectionState();

            if (hasParentGroup)
                parentGroup.HighlightSelectable(null);
        }

        /// <summary>
        /// Finds the selectable object next to this one.
        /// </summary>
        /// <remarks>
        /// The direction is determined by a Vector3 variable.
        /// </remarks>
        /// <param name="dir">The direction in which to search for a neighbouring Selectable object.</param>
        /// <returns>The neighbouring Selectable object. Null if none found.</returns>
        public SelectableUI FindSelectable(Vector3 dir)
        {
            dir = dir.normalized;
            Vector3 localDir = Quaternion.Inverse(transform.rotation) * dir;
            Vector3 pos = transform.TransformPoint(GetPointOnRectEdge(transform as RectTransform, localDir));
            float maxScore = Mathf.NegativeInfinity;
            float maxFurthestScore = Mathf.NegativeInfinity;
            float score = 0;

            bool wantsWrapAround = navigation.WrapAround && (navigation.Mode == NavigationUI.ModeEnum.Vertical || navigation.Mode == NavigationUI.ModeEnum.Horizontal);

            SelectableUI bestPick = null;
            SelectableUI bestFurthestPick = null;

            for (int i = 0; i < selectableCount; ++i)
            {
                SelectableUI sel = selectables[i];

                if (sel == this)
                    continue;

                if (!sel.IsSelectable || sel.navigation.Mode == NavigationUI.ModeEnum.None)
                    continue;

                if (sel.parentGroup != parentGroup)
                {
                    if (sel.hasParentGroup && hasParentGroup)
                    {
                        if (!(sel.parentGroup.RegisteredSelectables == parentGroup.RegisteredSelectables))
                            continue;
                    }
                    else
                        continue;
                }

#if UNITY_EDITOR
                // Apart from runtime use, FindSelectable is used by custom editors to
                // draw arrows between different selectables. For scene view cameras,
                // only selectables in the same stage should be considered.
                if (Camera.current != null && !UnityEditor.SceneManagement.StageUtility.IsGameObjectRenderedByCamera(sel.gameObject, Camera.current))
                    continue;
#endif

                var selRect = sel.transform as RectTransform;
                Vector3 selCenter = selRect != null ? (Vector3)selRect.rect.center : Vector3.zero;
                Vector3 myVector = sel.transform.TransformPoint(selCenter) - pos;

                // Value that is the distance out along the direction.
                float dot = Vector3.Dot(dir, myVector);

                // If element is in wrong direction and we have wrapAround enabled check and cache it if furthest away.
                if (wantsWrapAround && dot < 0)
                {
                    score = -dot * myVector.sqrMagnitude;

                    if (score > maxFurthestScore)
                    {
                        maxFurthestScore = score;
                        bestFurthestPick = sel;
                    }

                    continue;
                }

                // Skip elements that are in the wrong direction or which have zero distance.
                // This also ensures that the scoring formula below will not have a division by zero error.
                if (dot <= 0)
                    continue;

                // This scoring function has two priorities:
                // - Score higher for positions that are closer.
                // - Score higher for positions that are located in the right direction.
                // This scoring function combines both of these criteria.
                // It can be seen as this:
                //   Dot (dir, myVector.normalized) / myVector.magnitude
                // The first part equals 1 if the direction of myVector is the same as dir, and 0 if it's orthogonal.
                // The second part scores lower the greater the distance is by dividing by the distance.
                // The formula below is equivalent but more optimized.
                //
                // If a given score is chosen, the positions that evaluate to that score will form a circle
                // that touches pos and whose center is located along dir. A way to visualize the resulting functionality is this:
                // From the position pos, blow up a circular balloon so it grows in the direction of dir.
                // The first Selectable whose center the circular balloon touches is the one that's chosen.
                score = dot / myVector.sqrMagnitude;

                if (score > maxScore)
                {
                    maxScore = score;
                    bestPick = sel;
                }
            }

            if (wantsWrapAround && null == bestPick) return bestFurthestPick;

            return bestPick;
        }

        /// <summary>
        /// Find the selectable object to the left of this one.
        /// </summary>
        public SelectableUI FindSelectableOnLeft()
        {
            if (navigation.Mode == NavigationUI.ModeEnum.Explicit)
                return navigation.SelectOnLeft;

            if ((navigation.Mode & NavigationUI.ModeEnum.Horizontal) != 0)
                return FindSelectable(transform.rotation * Vector3.left);

            return null;
        }

        /// <summary>
        /// Find the selectable object to the right of this one.
        /// </summary>
        public SelectableUI FindSelectableOnRight()
        {
            if (navigation.Mode == NavigationUI.ModeEnum.Explicit)
                return navigation.SelectOnRight;

            if ((navigation.Mode & NavigationUI.ModeEnum.Horizontal) != 0)
                return FindSelectable(transform.rotation * Vector3.right);

            return null;
        }

        /// <summary>
        /// The Selectable object above current
        /// </summary>
        public SelectableUI FindSelectableOnUp()
        {
            if (navigation.Mode == NavigationUI.ModeEnum.Explicit)
                return navigation.SelectOnUp;

            if ((navigation.Mode & NavigationUI.ModeEnum.Vertical) != 0)
                return FindSelectable(transform.rotation * Vector3.up);

            return null;
        }

        /// <summary>
        /// Find the selectable object below this one.
        /// </summary>
        public SelectableUI FindSelectableOnDown()
        {
            if (navigation.Mode == NavigationUI.ModeEnum.Explicit)
                return navigation.SelectOnDown;

            if ((navigation.Mode & NavigationUI.ModeEnum.Vertical) != 0)
                return FindSelectable(transform.rotation * Vector3.down);

            return null;
        }

        /// <summary>
        /// Determine in which of the 4 move directions the next selectable object should be found.
        /// </summary>
        public void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Right:
                    Navigate(eventData, FindSelectableOnRight());
                    break;

                case MoveDirection.Up:
                    Navigate(eventData, FindSelectableOnUp());
                    break;

                case MoveDirection.Left:
                    Navigate(eventData, FindSelectableOnLeft());
                    break;

                case MoveDirection.Down:
                    Navigate(eventData, FindSelectableOnDown());
                    break;
            }
        }

        // Convenience function -- change the selection to the specified object if it's not null and happens to be active.
        private void Navigate(AxisEventData eventData, SelectableUI sel)
        {
            if (sel != null && sel.IsActive())
                eventData.selectedObject = sel.gameObject;
        }

        /// <summary>
        /// Whether the current selectable is being pressed.
        /// </summary>
        protected bool IsPressed()
        {
            if (!IsActive() || !isInteractable)
                return false;

            return isPointerDown;
        }

        // Change the button to the correct state
        private void EvaluateAndTransitionToSelectionState()
        {
            if (!IsActive() || !isInteractable)
                return;

            DoStateTransition(CurrentSelectionState, false);
        }

        private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();
        protected override void OnCanvasGroupChanged()
        {
            // Figure out if parent groups allow interaction
            // If no interaction is alowed... then we need
            // to not do that :)
            var groupAllowInteraction = true;
            Transform t = transform;
            while (t != null)
            {
                t.GetComponents(m_CanvasGroupCache);
                bool shouldBreak = false;
                for (var i = 0; i < m_CanvasGroupCache.Count; i++)
                {
                    // if the parent group does not allow interaction
                    // we need to break
                    if (m_CanvasGroupCache[i].enabled && !m_CanvasGroupCache[i].interactable)
                    {
                        groupAllowInteraction = false;
                        shouldBreak = true;
                    }
                    // if this is a 'fresh' group, then break
                    // as we should not consider parents
                    if (m_CanvasGroupCache[i].ignoreParentGroups)
                        shouldBreak = true;
                }
                if (shouldBreak)
                    break;

                t = t.parent;
            }

            //if (!groupAllowInteraction && m_HasParentGroup && m_IsSelected)
            //    m_ParentGroup.SelectSelectable(null);

            if (groupAllowInteraction != isInteractable)
            {
                isInteractable = groupAllowInteraction;
                DoStateTransition(CurrentSelectionState, true);
            }
        }

        // Call from unity if animation properties have changed
        protected override void OnDidApplyAnimationProperties() => DoStateTransition(CurrentSelectionState, true);

        protected override void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            var parent = transform.parent;
            if (parent != null && parent.TryGetComponent(out parentGroup))
            {
                hasParentGroup = true;
                parentGroup.RegisterSelectable(this);
            }
        }

        protected override void OnDestroy()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            if (hasParentGroup)
                parentGroup.UnregisterSelectable(this);
        }

        // Select on enable and add to the list.
        protected override void OnEnable()
        {
            //Check to avoid multiple OnEnable() calls for each selectable
            if (enableCalled)
                return;

            base.OnEnable();

            if (selectableCount == selectables.Length)
            {
                SelectableUI[] temp = new SelectableUI[selectables.Length * 2];
                Array.Copy(selectables, temp, selectables.Length);
                selectables = temp;
            }

            currentIndex = selectableCount;
            selectables[currentIndex] = this;
            selectableCount++;
            isPointerDown = false;

            if (hasParentGroup && parentGroup.Selected == this)
                OnSelect(null);

            DoStateTransition(CurrentSelectionState, true);

            enableCalled = true;
        }

        // Remove from the list.
        protected override void OnDisable()
        {
#if UNITY_EDITOR
            if (ApplicationManager.IsExiting)
                return;
#endif

            //Check to avoid multiple OnDisable() calls for each selectable
            if (!enableCalled)
                return;

            selectableCount--;

            // Update the last elements index to be this index
            selectables[selectableCount].currentIndex = currentIndex;

            // Swap the last element and this element
            selectables[currentIndex] = selectables[selectableCount];

            // null out last element.
            selectables[selectableCount] = null;

            Deselect();
            InstantClearState();
            base.OnDisable();

            enableCalled = false;
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            // If our parenting changes figure out if we are under a new CanvasGroup.
            OnCanvasGroupChanged();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && IsPressed())
                InstantClearState();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            enabled = true;

            if (feedbacks != null)
            {
                for (int i = 0; i < feedbacks.Length; i++)
                {
                    if (feedbacks[i] == null)
                        feedbacks[i] = new ColorTintFeedbackUI();

                    feedbacks[i].OnValidate(this);
                }
            }

            // OnValidate can be called before OnEnable, this makes it unsafe to access other components
            // since they might not have been initialized yet.
            if (isActiveAndEnabled)
            {
                // And now go to the right state.
                DoStateTransition(CurrentSelectionState, true);
            }
        }
#endif

        /// <summary>
        /// Clear any internal state from the Selectable (used when disabling).
        /// </summary>
        protected virtual void InstantClearState()
        {
            isPointerInside = false;
            isPointerDown = false;
            isSelected = false;

            DoStateTransition(SelectionState.Normal, true);
        }

        /// <summary>
        /// Transition the Selectable to the entered state.
        /// </summary>
        /// <param name="state">State to transition to</param>
        /// <param name="instant">Should the transition occur instantly.</param>
        private void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;

            switch (state)
            {
                case SelectionState.Normal:
                    {
                        for (int i = 0; i < feedbacks.Length; i++)
                        {
                            IInteractableFeedbackUI feedback = feedbacks[i];
                            feedback.OnNormal(instant);
                        }
                        break;
                    }
                case SelectionState.Highlighted:
                    {
                        for (int i = 0; i < feedbacks.Length; i++)
                        {
                            IInteractableFeedbackUI feedback = feedbacks[i];
                            feedback.OnHighlighted(instant);
                        }
                        break;
                    }
                case SelectionState.Pressed:
                    {
                        for (int i = 0; i < feedbacks.Length; i++)
                        {
                            IInteractableFeedbackUI feedback = feedbacks[i];
                            feedback.OnPressed(instant);
                        }
                        break;
                    }
                case SelectionState.Selected:
                    {
                        for (int i = 0; i < feedbacks.Length; i++)
                        {
                            IInteractableFeedbackUI feedback = feedbacks[i];
                            feedback.OnSelected(instant);
                        }
                        break;
                    }
                default:
                    return;
            }
        }

        private static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
                return Vector3.zero;
            if (dir != Vector2.zero)
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
            return dir;
        }
    }
}

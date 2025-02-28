using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gamepangin.UI
{
    [AddComponentMenu("Gamepangin/UI/Framed Selectable Group")]
    public class FramedSelectableGroupUI : SelectableGroupUI
    {
        #region Internal
        [System.Flags]
        private enum FrameSelectionMatchType
        {
            MatchColor = 1,
            MatchXSize = 2,
            MatchYSize = 4,
        }
        #endregion

        [Title("Frame Settings")]

        [SerializeField]
        private RectTransform selectionFrame;

        [SerializeField]
        private Vector2 selectionOffset = Vector2.zero;

        [SerializeField]
        private FrameSelectionMatchType selectionMatchFlags;

        private RectTransform frame;
        private bool frameActive = true;
        private SelectableUI lastSelectable;

        protected override void OnSelectedChanged(SelectableUI selectable)
        {
            if (frame == null)
                CreateFrame();

            if (selectable == null || selectable.gameObject.HasComponent(typeof(RaycastMask)))
            {
                EnableFrame(false);
                return;
            }

            if (lastSelectable != null)
            {
                if (lastSelectable.TryGetComponent<ItemSlotUI>(out var slotUI))
                {
                    slotUI.IsSelected = false;
                }
            }

            lastSelectable = selectable;
            
            EnableFrame(true);

            frame.SetParent(selectable.transform);
            frame.anchoredPosition = selectionOffset;
            frame.localRotation = Quaternion.identity;
            frame.localScale = Vector3.one;
            var localPos = frame.localPosition;
            frame.localPosition = new Vector3(localPos.x, localPos.y, 0f);

            bool matchXSize = (selectionMatchFlags & FrameSelectionMatchType.MatchXSize) == FrameSelectionMatchType.MatchXSize;
            bool matchYSize = (selectionMatchFlags & FrameSelectionMatchType.MatchYSize) == FrameSelectionMatchType.MatchYSize;

            if (matchXSize || matchYSize)
            {
                var frameSize = frame.sizeDelta;
                var selectableSize = ((RectTransform)selectable.transform).sizeDelta;

                frame.sizeDelta = new Vector2(matchXSize ? selectableSize.x : frameSize.x, matchYSize ? selectableSize.y : frameSize.y);
            }

            bool matchColor = (selectionMatchFlags & FrameSelectionMatchType.MatchColor) == FrameSelectionMatchType.MatchColor;

            if (matchColor && frame.TryGetComponent<Graphic>(out var frameGraphic))
                frameGraphic.color = selectable.GetComponent<Graphic>().color;
        }

        private void CreateFrame()
        {
            frame = Instantiate(selectionFrame);
            frameActive = frame.gameObject.activeSelf;
        }

        private void EnableFrame(bool enable)
        {
            if (frameActive == enable)
                return;

            frame.gameObject.SetActive(enable);
            frameActive = enable;
        }
    }
}

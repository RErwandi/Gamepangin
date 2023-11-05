using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    public class Hook : MonoBehaviour
    {
        [SerializeField] private GameObject reference;
#if UNITY_EDITOR
        [InlineButton(nameof(CreateNewHookAsset), "+")]
#endif
        [SerializeField] private HookData hookData;
        
        private void OnEnable()
        {
            hookData.GameObject = reference;
        }

        private void OnDisable()
        {
            if (hookData.GameObject == reference)
            {
                hookData.GameObject = null;
            }
        }
        
        private void OnValidate()
        {
            if (reference == null) reference = gameObject;
        }
        
#if UNITY_EDITOR
        private void CreateNewHookAsset()
        {
            ScriptableObjectCreator.ShowDialog<HookData>("Assets/", OnSuccessCreateNewDatabase);
        }

        private void OnSuccessCreateNewDatabase(HookData so)
        {
            hookData = so;
        }
#endif
    }
}
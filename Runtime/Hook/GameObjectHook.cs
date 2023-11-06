using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    public class GameObjectHook : MonoBehaviour
    {
        [SerializeField] private GameObject reference;
#if UNITY_EDITOR
        [InlineButton(nameof(CreateNewHookAsset), "+")]
#endif
        [SerializeField] private GameObjectHookData hookData;
        
        private void OnEnable()
        {
            hookData.Reference = reference;
        }

        private void OnDisable()
        {
            if (hookData.Reference == reference)
            {
                hookData.Reference = null;
            }
        }
        
        private void OnValidate()
        {
            if (reference == null) reference = gameObject;
        }
        
#if UNITY_EDITOR
        private void CreateNewHookAsset()
        {
            ScriptableObjectCreator.ShowDialog<GameObjectHookData>("Assets/", OnSuccessCreateNewDatabase);
        }

        private void OnSuccessCreateNewDatabase(GameObjectHookData so)
        {
            hookData = so;
        }
#endif
    }
}
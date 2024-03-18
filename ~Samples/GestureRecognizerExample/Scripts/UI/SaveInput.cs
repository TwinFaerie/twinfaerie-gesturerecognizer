using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace TF.GestureRecognizer.Sample
{
    [RequireComponent(typeof(TMP_InputField))]
    public class SaveInput : MonoBehaviour
    {
        public UnityEvent<string> OnApply;
        
        private TMP_InputField inputField;
        
        private void Start()
        {
            inputField = GetComponent<TMP_InputField>();
        }
        
        public void ApplyInput()
        {
            OnApply?.Invoke(inputField.text);
            inputField.text = string.Empty;
        }
    }
}

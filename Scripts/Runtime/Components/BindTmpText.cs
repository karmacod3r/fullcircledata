using FullCircleData.Attributes;
using FullCircleData.Properties;
using TMPro;
using UnityEngine;

namespace FullCircleData.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class BindTmpText: BestBehaviour
    {
        [ChangeListener(nameof(BindingFieldNameChanged))]
        public Observable<string> bindingFieldName;
        public Observable<string> text;
        
        [GetComponent] private TMP_Text textComponent;

        private void BindingFieldNameChanged()
        {
            text.Connect(transform, bindingFieldName.Value, TextChanged);
            text.StartObserving();
        }

        private void TextChanged()
        {
            textComponent.text = text.Value;
        }
    }
}
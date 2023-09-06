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
        [ChangeListener(nameof(TextChanged))]
        public Observable<string> format = new("{0}");
        
        [GetComponent] private TMP_Text textComponent;

        private void BindingFieldNameChanged()
        {
            text.Connect(transform, bindingFieldName.Value, TextChanged);
            text.StartObserving();
        }

        private void TextChanged()
        {
            textComponent.text = string.Format(format.Value, text.Value);
        }
    }
}
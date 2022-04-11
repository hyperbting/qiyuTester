using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Text.RegularExpressions;

namespace Qiyi.UI.InputField
{
    public class VrInputField :
        Selectable,
        IVrInputField,
        ICanvasElement,
        IUpdateSelectedHandler,
        IInputFieldController,
        IDragHandler,
        ISelectHandler,
        IDeselectHandler,
        IPointerClickHandler
    {

        public static string ReginFormat_Num = "^[0-9]*$";

        enum LineType
        {
            SingleLine,
            MultiLine,
        }

        #pragma warning disable CS0649

        [SerializeField]
        private Text _textComponent;

        [SerializeField]
        private GameObject _placeHolder;

        [SerializeField]
        private Color _selectionColor = new Color (168f / 255f, 206f / 255f, 255f / 255f, 192f / 255f);

        [SerializeField]
        private LineType _lineType = LineType.SingleLine;

        #pragma warning restore CS0649

        [SerializeField]
        private int _characterLimit = 0;
        public int CharacterLimit
        {
            get { return _characterLimit; }
            set
            {
                SetPropertyUtility.SetStruct(ref _characterLimit, Math.Max(0, value));
            }
        }

        [SerializeField]
        SGImeInputType sgImeInputType = SGImeInputType.TYPE_CLASS_TEXT;
        [SerializeField]
        SGImeTextType sgImeTextType = SGImeTextType.TYPE_TEXT_VARIATION_NORMAL;

        [SerializeField]
        private string _regexFormat = null;
        public string RegexFormat { get => _regexFormat; set => _regexFormat = value; }

        public OnValueChangedEvent onValueChanged;

        [Serializable]
        public class OnValueChangedEvent: UnityEvent<string>
        {

        }

        private AbstractInputField _impl;

        private AbstractInputField Impl {
            get {
                if (_impl == null) {
                    initialize ();
                }
                return _impl;
            }
        }

        private ITextComponentWrapper _editableText;

        public string TextValue {
            get { return Impl.TextValue; }
            set {
                Impl.TextValue = value;
                if (onValueChanged != null) {
                    onValueChanged.Invoke (TextValue);
                }
            }
        }

        public string text {
            get {
                return TextValue;
            }
            set {
                TextValue = value;
            }
        }



        public override void OnSelect (BaseEventData eventData)
        {
            base.OnSelect (eventData);
            ActivateInputField ();
        }

        public override void OnDeselect (BaseEventData eventData)
        {
            base.OnDeselect (eventData);
            // do not deactivate inputfield here because ondeselect maybe called
            // when user clicking on the keyboard to input.
        }

        public void ActivateInputField ()
        {
            VrInputField[] vrInputFields = FindObjectsOfType<VrInputField>();
            foreach (var vrInputField in vrInputFields)
            {
                vrInputField.DeactivateInputField();
            }
            ImeManager.Instance.Show(sgImeInputType, sgImeTextType);
            ImeDelegateImpl_kbd._inputField = this;           
            Impl.ActivateInputField();
        }

        public void DeactivateInputField ()
        {
            if (_impl != null) {
                _impl.DeactivateInputField ();
            }
        }

        protected override void OnEnable ()
        {
            base.OnEnable ();
            if (_impl == null) {
                initialize ();
            }
            RegisterTextComponentDirtyCallbacks (_textComponent);
        }

        protected override void OnDisable ()
        {
            DeactivateInputField ();
            UnregisterTextComponentDirtyCallbacks (_textComponent);
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild (this);
            base.OnDisable ();
        }

        public bool IsInteractive ()
        {
            if (Impl == null) {
                return false;
            }

            return Impl.IsInteractive () && IsInteractable ();
        }

        public void FinishInput ()
        {
            Impl.FinishInput ();
        }

        public void ProcessEvent (UnityEngine.Event evt)
        {
            if (evt.character != '\0' &&  CharacterLimit > 0 && TextValue.Length >= CharacterLimit)
                return;

            if (!string.IsNullOrEmpty(RegexFormat) && (evt.character != '\0'))
            {
                Regex reg = new Regex(RegexFormat);
               
                if (!reg.IsMatch(evt.character.ToString()))
                {
                    return;
                }             
            }          

            Impl.ProcessEvent (evt);
        }

        private void initialize ()
        {
            _editableText = new EditableText (_textComponent, new TextGenerator ());

            var caret = CreateCaret ();

            if (_lineType.Equals (LineType.SingleLine)) {
                _impl = new VrInputFieldImpl (caret,
                    new SingleLineInputProcessor (new System.Text.StringBuilder (), new CaretNavigator (caret, _editableText, this)),
                    this,
                    _editableText);
            } else {
                _impl = new MultiLineInputFieldImpl (caret,
                    new BaseTextProcessor (new System.Text.StringBuilder (), new CaretNavigator (caret, _editableText, this)),
                    this,
                    _editableText);
            }
        }

        private ICaret CreateCaret ()
        {
            Transform caretTR = _textComponent.transform.parent.Find(transform.name + " Caret");
            if (caretTR != null)
            {
                ICaret caretTemp = caretTR.GetComponent<ICaret>();
                caretTemp.InputFieldController = this;
                return caretTemp;
            }
            GameObject caretObject = new GameObject (transform.name + " Caret", typeof(DefaultCaret));
            caretObject.transform.SetParent (_textComponent.transform.parent);
            caretObject.hideFlags = HideFlags.DontSave;
            ICaret caret = caretObject.GetComponent<ICaret> ();
            caret.InputFieldController = this;
            return caret;
        }

        public void RegisterTextComponentDirtyCallbacks (Text textComponent)
        {
            Assert.IsNotNull (textComponent);

            // register for a notification when text component update its vertices.
            // this is happening as text component's or its parent's RectTransform changes.
            // should update the text and caret following the text component.
            textComponent.RegisterDirtyVerticesCallback (MarkGeometryAsDirty);
        }

        public void UnregisterTextComponentDirtyCallbacks (Text textComponent)
        {
            Assert.IsNotNull (textComponent);
            textComponent.UnregisterDirtyVerticesCallback (MarkGeometryAsDirty);
        }

        public void MarkGeometryAsDirty ()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying || UnityEditor.PrefabUtility.GetPrefabObject (gameObject) != null)
                return;
#endif
            // request update graphic by adding this ICanvasElement to CanvasUpdateRegistry's graphic rebuilding queue.
            // Rebuild() will be called on next canvas update. 
            CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild (this);
        }

        public void Rebuild (CanvasUpdate executing)
        {
            switch (executing) {
            case CanvasUpdate.LatePreRender:
                Impl.DrawCaretOrSelection (_editableText);
                break;
            }
        }

        public void LayoutComplete ()
        {
        }

        public void GraphicUpdateComplete ()
        {
        }

        public void UpdateText ()
        {
            Impl.UpdateText ();
        }

        public void PopulateText (string text)
        {
            _editableText.Populate (text, gameObject);
            MarkGeometryAsDirty ();
        }

#if UNITY_EDITOR
        UnityEngine.Event processingEvent = new UnityEngine.Event();

        public void OnUpdateSelected (BaseEventData eventData)
        {
            if (!IsInteractive ()) {
                return;
            }

            while (UnityEngine.Event.PopEvent (processingEvent)) {
                if (processingEvent.rawType == EventType.KeyDown) {
                    ProcessEvent (processingEvent);
                    continue;
                }

                switch (processingEvent.type) {
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                    switch (processingEvent.commandName) {
                    case "SelectAll":
                        processingEvent.keyCode = KeyCode.A;
                        processingEvent.modifiers = EventModifiers.Control;
                        ProcessEvent (processingEvent);
                        break;
                    }
                    break;
                }
            }

            eventData.Use ();
        }

        #else
       public void OnUpdateSelected(BaseEventData eventData){}
#endif

        public void UpdateDisplayText (string text)
        {
            if (sgImeTextType == SGImeTextType.TYPE_TEXT_VARIATION_PASSWORD 
                || sgImeTextType == SGImeTextType.TYPE_TEXT_VARIATION_WEB_PASSWORD)
            {
                int len = text.Length;
                string str = "";
                for (int i = 0; i < len; i++)
                {
                    str += "*";
                }
                _editableText.UpdateDisplayText(str);
            }
            else
            {
                _editableText.UpdateDisplayText(text);
            }      
            _placeHolder.SetActive (text.Equals (string.Empty));

            if (onValueChanged != null)
            {
                onValueChanged.Invoke(text);
            }
        }

        public void OnEndInput (string text)
        {
            // TODO: add event.
        }

        public void RegisterTextComponentDirtyCallbacks ()
        {
            RegisterTextComponentDirtyCallbacks (_textComponent);
        }

        public override void OnPointerDown (PointerEventData eventData)
        {
            if (!AcceptPointerEvent (eventData)) {
                return;
            }
            base.OnPointerDown (eventData);
            Impl.OnPointerDown (eventData);
            MarkGeometryAsDirty ();
        }

        // prevent pressed transition animation.
        protected override void DoStateTransition (SelectionState state, bool instant)
        {
            if (state.Equals (SelectionState.Pressed) && IsInteractive ()) {
                state = SelectionState.Highlighted;
            } else if (state.Equals (SelectionState.Disabled)) {
                DeactivateInputField ();
            }
            base.DoStateTransition (state, instant);
        }

        private bool AcceptPointerEvent (PointerEventData eventData)
        {
            return IsActive () &&
            IsInteractable () &&
            eventData.button == PointerEventData.InputButton.Left;
        }

        #region Drag to highlight

        public void OnDrag (PointerEventData eventData)
        {
            if (!AcceptPointerEvent (eventData)) {
                return;
            }

            Impl.OnDrag (eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSelect(eventData);
        }

        #endregion
    }
}

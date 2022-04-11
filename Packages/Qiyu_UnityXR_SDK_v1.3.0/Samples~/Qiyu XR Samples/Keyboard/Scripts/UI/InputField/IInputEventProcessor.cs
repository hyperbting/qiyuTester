﻿using UnityEngine;

namespace Qiyi.UI.InputField
{
    public interface IInputEventProcessor
    {
        bool ProcessEvent(UnityEngine.Event keyEvent, int caretIndex, int selectionIndex);

        string TextValue { set; get; }

        void SelectAll();
    }
}

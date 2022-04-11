using UnityEngine;
using System;

namespace Qiyi.InputMethod
{
	interface IInputMethodProvider
	{
		void Initialize (Action OnSuccess, Action OnFail, MonoBehaviour monoObject);

		void ShutDown ();

		Page Convert (string input);

		Page HandleSelection(string compositionString, int pageNumber, int selectedIndexInPage);

		Page PageDown ();
	}
}


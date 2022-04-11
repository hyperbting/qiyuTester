using System;
using UnityEngine;

namespace Qiyi.InputMethod
{
	public interface IInputMethodView
	{
		MonoBehaviour GetMonoContext ();

		void InitFail ();

		void InitSuccess ();
	}
}


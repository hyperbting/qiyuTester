using UnityEngine;
using System;

namespace Qiyi.InputMethod
{
	class InputMethod : MonoBehaviour, IInputMethodView
	{
		public static readonly int MAX_INPUT_LENGTH = 20;

		private static InputMethod _instance;

		private InputMethodImpl Impl{ set; get; }

		void Awake ()
		{
			if (_instance != null) {
				Debug.LogError ("More than one InputMethod instance was found in your scene. "
				+ "Ensure that there is only one InputMethod.");
				
				this.enabled = false;
				return;
			}

			Impl = new InputMethodImpl (this);
		}

		void OnDestroy ()
		{
			Impl.OnDestroy ();
			_instance = null;
		}

		#region IInputMethodView implementation

		public MonoBehaviour GetMonoContext ()
		{
			return this;
		}

		public void InitFail ()
		{
		}

		public void InitSuccess ()
		{
			_instance = this;
		}

		#endregion

		#region public api

		public static bool HandleInput (string input)
		{
            if (_instance == null) {
                Debug.LogError ("InputMethod has not been initialized successfully.");
                return false;
            }

			return _instance.Impl.HandleInput (input);
		}

		public static bool HandleSelection (
			string compositionString,
			int pageNumber,
			int indexInPage)
		{
            if (_instance == null) {
                Debug.LogError ("InputMethod has not been initialized successfully.");
                return false;
            }

			return _instance.Impl.HandleSelection (compositionString, pageNumber, indexInPage);
		}

		public static CandidateInfo GetCandidate ()
		{
            if (_instance == null) {
                Debug.LogError ("InputMethod has not been initialized successfully.");
                return null;
            }

			return _instance.Impl.GetCandidate (); 
		}

		public static string GetPinyinSep (int page, int index)
		{
            if (_instance == null) {
                Debug.LogError ("InputMethod has not been initialized successfully.");
                return string.Empty;
            }
			
            return _instance.Impl.GetPinyinSep (page, index);
		}

		#endregion

	}
}

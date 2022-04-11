using UnityEngine;
using System.Collections.Generic;

namespace Qiyi.InputMethod
{
	class InputMethodImpl
	{
		private IInputMethodProvider _imProvider;
		private ICandidateSource _candidateSource;
		private IInputMethodView _view;

		public InputMethodImpl (IInputMethodView view)
		{
			_view = view;

			if (_imProvider == null) {
				_imProvider = IMComponentFactory.CreateIMProvider ();
			}

			_imProvider.Initialize (OnInitSuccess, OnInitFail, _view.GetMonoContext ());
		}

		private void OnInitSuccess ()
		{
			Debug.Log ("input method engine initialization success.");
			if (_candidateSource == null) {
				_candidateSource = IMComponentFactory.CreateCandidateSource ();
			}
			_view.InitSuccess ();
		}

		private void OnInitFail ()
		{
			Debug.LogError ("input method engine initialization fail.");
			_view.InitFail ();

			// TODO:disable pinyin input.
		}

		private CandidateInfo OnRunOut ()
		{
			if (PageDown ()) {
				return GetCandidate ();
			}

            return CandidateInfo.NONE;
		}

		private bool PageDown ()
		{
			return _candidateSource.AddPage (_imProvider.PageDown ());
		}

		public void OnDestroy ()
		{
			_imProvider.ShutDown ();
		}

		public string GetPinyinSep (int page, int index)
		{
			if (page < 0
			    || index < 0
			    || _candidateSource.GetPages ().Count == 0) {

				return string.Empty;

			} else {
				List<Page> pages = _candidateSource.GetPages ();
                if (page >= pages.Count || index >= pages [page].Candidates.Count) {
					return string.Empty;
				}
				return pages [page].Candidates [index].Pinyin;
			}
		}

		public bool HandleInput (string input)
		{
			_candidateSource.Clear ();
			return _candidateSource.AddPage (_imProvider.Convert (input));
		}

		public bool HandleSelection (
			string compositionString,
			int pageNumber,
			int indexInPage)
		{

			_candidateSource.Clear ();

			return _candidateSource.AddPage (_imProvider.HandleSelection (
				compositionString, pageNumber, indexInPage));
		}

		public CandidateInfo GetCandidate ()
		{
			return _candidateSource.GetNextCandidate (OnRunOut);
		}

		private static class IMComponentFactory
		{
			static internal IInputMethodProvider CreateIMProvider ()
			{
				#if UNITY_EDITOR
				return new DummyIMProvider ();

				#else
				return new PinyinIMProvider ();
				#endif
			}

			static internal ICandidateSource CreateCandidateSource ()
			{
				return new CandidateSource ();
			}
		}
	}
}

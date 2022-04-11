using System;
using System.Collections.Generic;

namespace Qiyi.InputMethod
{
	class DummyIMProvider : IInputMethodProvider
	{

		private const uint PAGE_SIZE = 48;

		#region IInputMethodProvider implementation

		public void Initialize (Action OnSuccess, Action OnFail, UnityEngine.MonoBehaviour monoObject)
		{
			OnSuccess ();
		}

		public void ShutDown ()
		{
			
		}

		public Page Convert (string input)
		{
			List<CandidateInfo> candidates = GetCandidates (0, 0, input);

			Page page = new Page.Builder ()
				.PageSize (candidates.Count)
				.Candidates (candidates)
				.HasNextPage (false)
				.HasPrevPage (false)
				.Input (input)
				.PageNumber (0)
				.Build ();

			return page;
		}

		public Page HandleSelection (string compositionString, int pageNumber, int selectedIndexInPage)
		{
			if (String.IsNullOrEmpty (compositionString)) {
				return WordPrediction (pageNumber, selectedIndexInPage);
			} else {
				return Convert (compositionString);
			}
		}

		public Page PageDown ()
		{
			bool hasNextPage = false;	
			int pageNumber = 0;

			int count = 20;

			if (count == 0) {
				return null;
			}

			List<CandidateInfo> candidates = GetCandidates (count, pageNumber, "");

			Page page = new Page.Builder ()
				.PageSize (candidates.Count)
				.Candidates (candidates)
				.HasNextPage (hasNextPage && candidates.Count == PAGE_SIZE)
				.HasPrevPage (true)
				.PageNumber (pageNumber)
				.Build ();

			return page;
		}

		#endregion

		private Page WordPrediction (int pageNumber, int indexInPage)
		{
			List<CandidateInfo> candidates = new List<CandidateInfo> ();

			for (int i = 0; i < 20; i++) {
				candidates.Add (
					new CandidateInfo.Builder ()
					.PageNumber (0)
					.IndexInPage (i)
					.Word ("(" + pageNumber + ", " + indexInPage + ")")
					.MatchLength (0)
					.Build ()
				);
			}

			Page page = new Page.Builder ()
				.PageSize (candidates.Count)
				.Candidates (candidates)
				.HasNextPage (false)
				.HasPrevPage (false)
				.PageNumber (0)
				.Build ();

			return page;	
		}

		private List<CandidateInfo> GetCandidates (int count, int pageNumber, string pinyin)
		{
			List<CandidateInfo> candidates = new List<CandidateInfo> ();

			for (int i = 0; i < PAGE_SIZE; i++) {
				int matchLength = pinyin.Length;

				candidates.Add (
					new CandidateInfo.Builder ()
					.PageNumber (0)
					.IndexInPage (i)
					.Word ("dummy")
					.MatchLength (matchLength)
					.Pinyin (pinyin)
					.Build ()
				);
			}

			return candidates;
		}
	}
}


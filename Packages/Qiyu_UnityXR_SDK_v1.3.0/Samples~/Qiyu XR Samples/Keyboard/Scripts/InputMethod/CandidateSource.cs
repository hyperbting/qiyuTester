using System.Collections.Generic;
using System;

namespace Qiyi.InputMethod
{
	class CandidateSource : ICandidateSource
	{
		private int _globalIndex = 0;

		private List<Page> _pages = new List<Page> ();

		#region ISource implementation

		public List<Page> GetPages ()
		{
			return _pages;
		}

		public CandidateInfo GetNextCandidate (Func<CandidateInfo> RequestMorePage)
		{
			if (_pages.Count == 0) {
                return CandidateInfo.NONE;
			}

			int maxPageSize = _pages [0].PageSize;
            int pageNumber = _globalIndex / maxPageSize;

            if (pageNumber == _pages.Count) {
                if (_pages [_pages.Count - 1].HasNextPage) {
                    return RequestMorePage ();
                } 
                return CandidateInfo.NONE;
            }

            int indexInPage = _globalIndex % maxPageSize;
            if (_pages [pageNumber].Candidates.Count <= indexInPage) {
                return CandidateInfo.NONE;
            }

            _globalIndex++;
            return _pages [pageNumber].Candidates [indexInPage];
		}

		public void Clear ()
		{
			_globalIndex = 0;
			_pages.Clear ();
		}

		public bool AddPage (Page page)
		{
            if (Page.EMPTY.Equals (page)) {
				return false;
			}

			_pages.Add (page);
			return true;
		}

		#endregion

	}
}

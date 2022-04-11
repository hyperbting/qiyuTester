using System.Collections.Generic;

namespace Qiyi.InputMethod
{
	class Page
	{
        public static Page EMPTY = new Page ();

		public int PageSize{ private set; get; }

		public bool HasNextPage{ private set; get; }

		public bool HasPrevPage{ private set; get; }

		public string Input{ private set; get; }

		public int PageNumber{ private set; get; }

		public List<CandidateInfo> Candidates{ private set; get; }

		private Page ()
		{
		}

		public override string ToString ()
		{
			return string.Format (
				"[Page: PageSize={0}, HasNextPage={1}, HasPrevPage={2}, Input={3}, PageNumber={4}, Candidates={5}]", 
				PageSize, HasNextPage, HasPrevPage, Input, PageNumber, Candidates);
		}

		public class Builder
		{
			private int _pageSize;

			private bool _hasNextPage;

			private bool _hasPrevPage;

			private string _input;

			private int _pageNumber;

			private List<CandidateInfo> _candidates;

			public Builder PageSize (int pageSize)
			{
				this._pageSize = pageSize;
				return this;
			}

			public Builder HasNextPage (bool hasNextPage)
			{
				this._hasNextPage = hasNextPage;
				return this;
			}

			public Builder HasPrevPage (bool hasPrevPage)
			{
				this._hasPrevPage = hasPrevPage;
				return this;
			}

			public Builder Input (string input)
			{
				this._input = input;
				return this;
			}

			public Builder Candidates (List<CandidateInfo> candidates)
			{
				this._candidates = candidates;
				return this;
			}

			public Builder PageNumber (int num)
			{
				this._pageNumber = num;
				return this;
			}

			public Page Build ()
			{
				Page page = new Page ();

				page.Candidates = this._candidates;
				page.HasNextPage = this._hasNextPage;
				page.HasPrevPage = this._hasPrevPage;
				page.Input = this._input;
				page.PageSize = this._pageSize;
				page.PageNumber = this._pageNumber;

				return page;
			}
		}
	}
}

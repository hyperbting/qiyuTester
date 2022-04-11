namespace Qiyi.InputMethod
{

	public sealed class CandidateInfo
	{
        public static CandidateInfo NONE = new CandidateInfo ();

		public string Word{ private set; get; }

		public string Pinyin{ private set; get; }

		public int PageNumber{ private set; get; }

		public int IndexInPage{ private set; get; }

		public int MatchLength{ private set; get; }

		private CandidateInfo ()
		{
		}

		public class Builder
		{
			private string _word;

			private string _pinyin;

			private int _pageNumber;

			private int _indexInPage;

			private int _matchLength;

			public Builder Word (string word)
			{
				this._word = word;
				return this;
			}

			public Builder Pinyin (string pinyin)
			{
				this._pinyin = pinyin;
				return this;
			}

			public Builder PageNumber (int pageNumber)
			{
				this._pageNumber = pageNumber;
				return this;
			}

			public Builder IndexInPage (int index)
			{
				this._indexInPage = index;
				return this;
			}

			public Builder MatchLength(int matchLength){
				this._matchLength = matchLength;
				return this;
			}

			public CandidateInfo Build ()
			{
				CandidateInfo candidate = new CandidateInfo ();

				candidate.Word = this._word;
				candidate.Pinyin = this._pinyin;
				candidate.PageNumber = this._pageNumber;
				candidate.IndexInPage = this._indexInPage;
				candidate.MatchLength = this._matchLength;

				return candidate;
			}
		}
	}
}

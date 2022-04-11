using System;
using System.Collections.Generic;

namespace Qiyi.InputMethod
{
	interface ICandidateSource
	{
		CandidateInfo GetNextCandidate (Func<CandidateInfo> OnRunOut);

		bool AddPage (Page page);

		void Clear ();

		List<Page> GetPages ();
	}
}


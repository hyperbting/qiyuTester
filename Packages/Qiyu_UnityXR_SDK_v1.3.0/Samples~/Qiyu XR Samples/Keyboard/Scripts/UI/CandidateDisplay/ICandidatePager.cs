using System;

namespace Qiyi.InputMethod.Keyboard
{
	public interface ICandidatePager
	{
		Action<CandidateInfo> OnCandidateClickDelegate{ get; set; }

		void Show ();

		void Hide ();

		bool IsActive ();

		void UpdateWords ();
	}
}


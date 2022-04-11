using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

namespace Qiyi.InputMethod
{
	class PinyinIMProvider : IInputMethodProvider
	{
		private const int MAX_INPUT_LEGNTH = 20;
		private const int WORD_BUFFER_SIZE = 24;
		private const uint PAGE_SIZE = 48;
		private const string DLL_NAME = "sogou_ime";
		private const string DICT_FOLDER_NAME = "Dict";

		private Action _onSuccess, _onFail;
		private string _currentInput;
		private MonoBehaviour _monoContext;


		[DllImport (DLL_NAME, EntryPoint = "InitPinyin", ExactSpelling = true, CharSet = CharSet.Unicode)]
		private static extern bool InitPinyinNative (string dictPath, uint fuzzy, uint pageSize = 0);

		[DllImport (DLL_NAME, EntryPoint = "Convert", ExactSpelling = true, CharSet = CharSet.Unicode)]
		private static extern int ConvertNative 
		(
			string input, 
			StringBuilder pinyinSep,
			out bool hasNextPage
		);

		[DllImport (DLL_NAME, EntryPoint = "NextWord", ExactSpelling = true, CharSet = CharSet.Unicode)]
		private static extern int NextWordNative (StringBuilder builder, int index);

		/// <summary>
		/// Native api for word prediction.
		/// </summary>
		/// <returns>Prediction result count.</returns>
		/// <param name="pageNumber">Selected word's page number.</param>
		/// <param name="selected">Selected word's index.</param>
		/// <param name="hasNextPage">If has next page.</param>
		[DllImport (DLL_NAME, EntryPoint = "WordPredictionByIndex", ExactSpelling = true)]
		private static extern int WordPredictionNative
		(
			int pageNumber,
			int selected,
			out bool hasNextPage
		);

		[DllImport (DLL_NAME, EntryPoint = "DeactiveInputMethod", ExactSpelling = true)]
		private static extern void DeactiveInputMethodNative ();

		/// <summary>
		/// Native api for page down.
		/// </summary>
		/// <returns>result count of the page</returns>
		/// <param name="hasNextPage">if still has next page.</param>
		/// <param name="nativePageNumber">Native page number.</param>
		[DllImport (DLL_NAME, EntryPoint = "PageDown", ExactSpelling = true)]
		private static extern int PageDownNative (out bool hasNextPage, out int nativePageNumber);

		#region IMServiceProvider implementation

		public void Initialize (Action OnSuccess, Action OnFail, MonoBehaviour monoObject)
		{
			_onSuccess = OnSuccess;
			_onFail = OnFail;

			#if UNITY_EDITOR
			return;
			#else

			_monoContext = monoObject;

			// TODO: test memory usage and active/deactive time, choose trade-off policy. 
			string fromPath = Path.Combine (Application.streamingAssetsPath, DICT_FOLDER_NAME);

			//In Android = "jar:file://" + Application.dataPath + "!/assets/" 
			string toPath = Path.Combine (Application.persistentDataPath, DICT_FOLDER_NAME);

			if (!Directory.Exists (toPath)) {
				Directory.CreateDirectory (toPath);
			}

			monoObject.StartCoroutine (InitPinyinIme (fromPath, toPath));
			#endif
		}

		public void ShutDown ()
		{
			#if UNITY_EDITOR
			return;

			#elif UNITY_ANDROID

			_monoContext.StopAllCoroutines ();
			DeactiveInputMethodNative ();

			#endif
		}

		public Page Convert (string input)
		{
			if (input.Length > MAX_INPUT_LEGNTH) {
				Debug.LogError ("input length out of range.");
                return Page.EMPTY;
			}

			_currentInput = input;
			bool hasNextPage;

			StringBuilder pinyin = new StringBuilder (100);
			int resultCount = ConvertNative (input,
				                  pinyin,
				                  out hasNextPage);

			List<CandidateInfo> candidates = GetCandidates (resultCount, 0, pinyin.ToString ());

			Page page = new Page.Builder ()
				.PageSize (candidates.Count)
				.Candidates (candidates)
				.HasNextPage (hasNextPage && candidates.Count == PAGE_SIZE)
				.HasPrevPage (false)
				.Input (input)
				.PageNumber (0)
				.Build ();

			Debug.Log ("convert: " + page);
			return page;
		}

		public Page PageDown ()
		{
			bool hasNextPage;	
			int pageNumber;

			int count = PageDownNative (out hasNextPage, out pageNumber);

			if (count == 0) {
                return Page.EMPTY;
			}

			List<CandidateInfo> candidates = GetCandidates (count, pageNumber, "");

			Page page = new Page.Builder ()
				.PageSize (candidates.Count)
				.Candidates (candidates)
				.HasNextPage (hasNextPage && candidates.Count == PAGE_SIZE)
				.HasPrevPage (true)
				.PageNumber (pageNumber)
				.Build ();

			Debug.Log ("page down: " + page);
			return page;
		}

		public Page HandleSelection (string compositionString,
		                             int pageNumber, int selectedIndexInPage)
		{
			if (String.IsNullOrEmpty (compositionString)) {
				return WordPrediction (pageNumber, selectedIndexInPage);
			} else {
				return Convert (compositionString);
			}
		}

		#endregion

		private Page WordPrediction (int pageNumber, int selectedIndexInPage)
		{
			bool hasNextPage;
			int resultCount = WordPredictionNative (pageNumber, selectedIndexInPage, out hasNextPage);
			if (resultCount <= 0) {
                return Page.EMPTY;
			}
			List<CandidateInfo> candidates = GetPredictionCandidates (resultCount, 0);

			Page page = new Page.Builder ()
				.PageSize (candidates.Count)
				.Candidates (candidates)
				.HasNextPage (hasNextPage && candidates.Count == PAGE_SIZE)
				.HasPrevPage (false)
				.PageNumber (0)
				.Build ();

			Debug.Log ("word prediction: " + page);

			return page; 
		}

		private void ResetBuffer (StringBuilder builder)
		{
			builder.Length = 0;
			builder.Capacity = 0;
			builder.EnsureCapacity (WORD_BUFFER_SIZE);
		}

		/// <summary>
		/// Inits the pinyin IME. asychronously copy fictionary files 
		/// from streaming assets to android persistent data folder.
		/// </summary>
		private IEnumerator InitPinyinIme (string fromPath, string toPath)
		{

			// sogou ime engine system dictionary files. 
			// make sure to copy all of them with original names, 
			// otherwise engine will not be initialized successfully.

			string[] filesNamesToCopy = {
				"inputstatic.dat",
				"sgim_em.bin",
				"sgim_sw_ts.bin",
				"rand.bin",
				"sgim_im.bin",
				"sgim_swab.bin",
				"sgim_InputStr.bin",
				"sgim_ip.bin",
				"sgim_sys.bin",
				"sgim_aa.bin",
				"sgim_ma.bin",
				"sgim_tam.bin",
				"sgim_abg.bin",
				"sgim_name.bin",
				"sgim_tb.bin",
				"sgim_aid.bin",
				"sgim_ndata.bin",
				"sgim_uc.bin",
				"sgim_asso_trigger.bin",
				"sgim_pos.bin",
				"sgim_ucd.bin",
				"sgim_bc.bin",
				"sgim_pred.bin",
				"sgim_ui.bin",
				"sgim_bcd.bin",
				"sgim_prefix_table.bin",
				"sgim_um.bin",
				"sgim_bh.bin",
				"sgim_py.bin",
				"sgim_usrbg.bin",
				"sgim_bigram.bin",
				"sgim_qr.bin",
				"sgim_wb.bin",
				"sgim_blflx.bin",
				"sgim_shw.bin",
				"sgim_zly.bin",
				"sgim_bsa.bin",
				"sgim_smile.bin",
				"sys_en.bin",
				"sgim_cagram.bin",
				"sgim_sp.bin",
				"trid.bin",
				"sgim_cf.bin",
				"sgim_splt.bin",
				"trid_to_sim.bin",
				"sgim_cor.bin",
				"sgim_sw_sys.bin"	
			};

			foreach (string file in filesNamesToCopy) {
				WWW www = new WWW (Path.Combine (fromPath, file));
				yield return www;
				File.WriteAllBytes (Path.Combine (toPath, file), www.bytes);
			}

			if (!InitPinyinNative (toPath + "/", 0, PAGE_SIZE)) {
				Debug.LogError ("path: " + toPath);

				if (_onFail != null) {
					_onFail ();
				}
			} else {
				if (_onSuccess != null) {
					_onSuccess ();
				}
			}
		}

		private List<CandidateInfo> GetCandidates (int count, int pageNumber, string pinyin)
		{
			List<CandidateInfo> candidates = new List<CandidateInfo> ();
			StringBuilder buffer = new StringBuilder (WORD_BUFFER_SIZE);
			for (int i = 0; i < count; i++) {
				int matchLength = NextWordNative (buffer, i);

				// filter invalid word.
				if (matchLength < 1 || matchLength > _currentInput.Length) {
					continue;
				}

				candidates.Add (
					new CandidateInfo.Builder ()
					.PageNumber (pageNumber)
					.IndexInPage (i)
					.Word (buffer.ToString ())
					.MatchLength (matchLength)
					.Pinyin (pinyin)
					.Build ()
				);

				ResetBuffer (buffer);
			}

			return candidates;
		}

		private List<CandidateInfo> GetPredictionCandidates (int count, int pageNumber)
		{
			List<CandidateInfo> candidates = new List<CandidateInfo> ();
			StringBuilder buffer = new StringBuilder (WORD_BUFFER_SIZE);
			for (int i = 0; i < count; i++) {
				NextWordNative (buffer, i);

				candidates.Add (
					new CandidateInfo.Builder ()
					.PageNumber (pageNumber)
					.IndexInPage (i)
					.Word (buffer.ToString ())
					.Build ()
				);

				ResetBuffer (buffer);
			}

			return candidates;
		}
	}
}


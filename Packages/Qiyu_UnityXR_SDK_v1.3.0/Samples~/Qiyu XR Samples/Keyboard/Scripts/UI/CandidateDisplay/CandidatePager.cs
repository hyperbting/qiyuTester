using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Qiyi.InputMethod;
using System;
using System.Collections;

namespace Qiyi.InputMethod.Keyboard
{
    public class CandidatePager : MonoBehaviour, ICandidatePager
    {
        #pragma warning disable CS0649
        [SerializeField] private Button m_PageUpButton;
        [SerializeField] private Button m_PageDownButton;
        [SerializeField] private GameObject m_CandidateItemPrefab;
        [SerializeField] private GameObject m_LinePrefab;
        [SerializeField] private RectTransform m_ContentRect;
        [SerializeField] private Text m_InputPinyinText;
        [SerializeField] private float m_LineWidth;
        [SerializeField] private Button m_ExpandButton;
        [SerializeField] private Button m_FoldButton;
        [SerializeField] private float m_ItemMinWidth;
        [SerializeField] private int m_RowCount = 3;
        #pragma warning restore CS0649

        private const int UNIT_WORD_LENGTH = 3;
        private const float ANIMATION_DURATION = 0.2f;
        private const float PAGING_DURATION = 0.7f;
        private const int CANDIDATE_QUEUE_SIZE = 5;

        private float m_InitialRectWidth;
        private float m_FoldedHeight;
        private float m_ExpandedHeight;
        private float m_PagerScrolledOffset = 0;
        private CandidateItemPool m_CandidateItemPool;
        private List<CandidateItem> m_CandidateBindingQueue = new List<CandidateItem> ();
        private bool m_HasMoreCandidates = true;
        private bool m_HasPreviousPage = false;
        private bool m_IsExpanded = false;
        private int m_CurrentDisplayPage = 0;
        private int m_TotalPage = 0;
        private RectTransform _rect;
        private bool _isAnimating = false;

        private RectTransform CachedRect {
            get {
                if (_rect == null) {
                    _rect = GetComponent<RectTransform> ();
                }
                return _rect;
            }

            set { 
                _rect = value;
            }
        }

        public void Hide ()
        {
            gameObject.SetActive (false);
            m_PageUpButton.gameObject.SetActive (false);
            m_PageDownButton.gameObject.SetActive (false);
        }

        public void Show ()
        {
            gameObject.SetActive (true);
        }

        public bool IsActive ()
        {
            return gameObject.activeInHierarchy;
        }

        public void UpdateWords ()
        {
            if (!IsActive ()) {
                Show ();
            }

            PopulateFirstPage (OnCandidateClickDelegate);
        }

        public Action<CandidateInfo> OnCandidateClickDelegate {
            get;
            set;
        }

        #region lifecycle

        void Awake ()
        {
            m_CandidateItemPool = new CandidateItemPool (m_CandidateItemPrefab);
        }

        protected virtual void Start ()
        {
            SetupViews ();
            m_PageDownButton.onClick.AddListener (HandlePageDown);
            m_PageUpButton.onClick.AddListener (HandlePageUp);
            m_ExpandButton.onClick.AddListener (delegate {
                ExpandPanel ();
            });
            m_FoldButton.onClick.AddListener (delegate {
                FoldPanel ();
            });
        }

        private void SetupViews ()
        {
            m_InitialRectWidth = m_ContentRect.sizeDelta.x;
            m_FoldedHeight = CachedRect.sizeDelta.y;
            m_ContentRect.GetComponent<GridLayoutGroup> ().constraintCount = m_RowCount;
            m_ExpandedHeight = m_ContentRect.GetComponent<GridLayoutGroup> ()
                .cellSize.y * (m_RowCount - 1) + m_FoldedHeight;

            if (m_RowCount == 1) {
                m_ExpandButton.gameObject.SetActive (false);
                m_FoldButton.gameObject.SetActive (false);
                m_IsExpanded = true;
            } 
        }

        void OnDestroy ()
        {
            m_PageDownButton.onClick.RemoveAllListeners ();
            m_PageUpButton.onClick.RemoveAllListeners ();
            m_ExpandButton.onClick.RemoveAllListeners ();
            m_FoldButton.onClick.RemoveAllListeners ();
            Reset ();
        }

        #endregion

        private void ExpandPanel (bool animate = true)
        {
            if (!m_IsExpanded) {
                if (animate) {
                    StartCoroutine (ExpandPanelAnimation ());
                } else {
                    CachedRect.sizeDelta = new Vector2 (m_InitialRectWidth, m_ExpandedHeight);
                }
            }
            m_ExpandButton.gameObject.SetActive (false);
            m_FoldButton.gameObject.SetActive (true);

            m_IsExpanded = true;

            // diplay page up or down button if has more pages.
            UpdatePagerButtonsVisibility ();
        }

        private IEnumerator ExpandPanelAnimation ()
        {
            _isAnimating = true;
            float timer = 0f;
            while (timer < 1) {
                CachedRect.sizeDelta = Vector2.Lerp (
                    new Vector2 (m_InitialRectWidth, m_FoldedHeight),
                    new Vector2 (m_InitialRectWidth, m_ExpandedHeight),
                    timer
                );

                timer += Time.deltaTime / ANIMATION_DURATION;

                yield return new WaitForSeconds (Time.deltaTime / 2f);
            }

            _isAnimating = false;
        }

        private IEnumerator FoldPanelAnimation ()
        {
            _isAnimating = true;
            float timer = 0f;
            while (timer < 1) {
                CachedRect.sizeDelta = Vector2.Lerp (
                    new Vector2 (m_InitialRectWidth, m_ExpandedHeight),
                    new Vector2 (m_InitialRectWidth, m_FoldedHeight),
                    timer
                );

                timer += Time.deltaTime / ANIMATION_DURATION;

                yield return new WaitForSeconds (Time.deltaTime / 2f);
            }
            _isAnimating = false;
        }

        private void FoldPanel (bool animate = true)
        {
            if (m_IsExpanded) {
                if (animate) {
                    StartCoroutine (FoldPanelAnimation ());
                } else {
                    CachedRect.sizeDelta = new Vector2 (m_InitialRectWidth, m_FoldedHeight);
                }
            }
			
            m_ExpandButton.gameObject.SetActive (true);
            m_FoldButton.gameObject.SetActive (false);

            m_IsExpanded = false;

            // hide page up and down buttons.
            UpdatePagerButtonsVisibility ();
        }

        private void HandlePageDown ()
        {
            if (_isAnimating) {
                return;
            }

            if (m_HasMoreCandidates) {
                PopulatePage (OnCandidateClickDelegate);
            }

            m_CurrentDisplayPage++;
            m_PagerScrolledOffset -= m_InitialRectWidth;
            StartCoroutine (TranslateXBy (m_ContentRect, -m_InitialRectWidth));
            m_HasPreviousPage = true;
            m_PageUpButton.gameObject.SetActive (m_IsExpanded);

            if (!m_HasMoreCandidates && m_CurrentDisplayPage >= m_TotalPage) {
                m_PageDownButton.gameObject.SetActive (false);
            }
        }

        private void HandlePageUp ()
        {
            if (_isAnimating) {
                return;
            }

            m_PagerScrolledOffset += m_InitialRectWidth;
            StartCoroutine (TranslateXBy (m_ContentRect, m_InitialRectWidth));
            m_PageDownButton.gameObject.SetActive (m_IsExpanded);

            if (Mathf.Abs (m_PagerScrolledOffset) < 0.1) {
                m_HasPreviousPage = false;
            }

            if (!m_HasPreviousPage) {
                m_PageUpButton.gameObject.SetActive (false);
            }
            m_CurrentDisplayPage--;
        }

        private CandidateItem GetNextCandidateData ()
        {
            CandidateInfo candidate = InputMethod.GetCandidate ();

            if (candidate != CandidateInfo.NONE) {

                float width = CalculateItemWidth (
                                  candidate.Word.Length, UNIT_WORD_LENGTH, m_ItemMinWidth);
				
                CandidateItem item = new CandidateItem (candidate, width);
                return item;
            } 

            return null;
        }

        private void PopulatePage (Action<CandidateInfo> onItemClick)
        {
            for (int i = 0; i < m_RowCount; i++) {
                GameObject line = Instantiate (m_LinePrefab, m_ContentRect, false);
                line.transform.localPosition = Vector3.zero;
                line.transform.localScale = Vector3.one;
                line.transform.localRotation = Quaternion.identity;
                if (!PopulateLine (line.transform, onItemClick)) {
                    if (i == 0) {
                        m_TotalPage--;
                    }
                    break;
                }
            }

            if (m_HasMoreCandidates) {
                m_TotalPage++;
            }
        }

        private bool PopulateLine (Transform line, Action<CandidateInfo> onItemClick)
        {
            float availableLineSpace = m_LineWidth;

            while (availableLineSpace > 0) {

                // if there is candidate in queue, add it to this line.
                // else, get a new candidate to add.
                for (int i = m_CandidateBindingQueue.Count - 1; i >= 0; i--) {
                    if (availableLineSpace <= 0) {
                        break;
                    }

                    if (m_CandidateBindingQueue [i].ItemWidth <= availableLineSpace) {
                        BindCandidateObject (line, onItemClick, m_CandidateBindingQueue [i]);
                        availableLineSpace -= m_CandidateBindingQueue [i].ItemWidth;
                        m_CandidateBindingQueue.Remove (m_CandidateBindingQueue [i]);
                    }
                }

                if (availableLineSpace > 0) {
                    CandidateItem data = GetNextCandidateData ();
                    // no more word for current query.
                    if (data == null) {
                        m_HasMoreCandidates = false;
                        return m_CandidateBindingQueue.Count != 0;
                    }

                    if (data.ItemWidth <= 0) {
                        Debug.LogError ("candidate item width <= 0.");
                    }

                    if (data.ItemWidth <= availableLineSpace) {
                        BindCandidateObject (line, onItemClick, data);
                        availableLineSpace -= data.ItemWidth;
                    } else {
                        m_CandidateBindingQueue.Insert (0, data);
                        if (m_CandidateBindingQueue.Count == CANDIDATE_QUEUE_SIZE) {
							
                            // no candidate can fit this available space, begin a new line.
                            availableLineSpace = 0;
                        }
                    }
                }
            }

            return true;
        }

        // return 1, 2, 4, 8... times of unit width.
        private float CalculateItemWidth (int wordLength, int unitWordLength, float unitWidth)
        {
            int multiplyier = 1;

            while (unitWordLength * multiplyier < wordLength) {
                multiplyier *= 2;
                if (multiplyier * unitWidth > m_LineWidth) {
                    return m_LineWidth;
                }
            }
            return multiplyier * unitWidth;
        }

        private GameObject BindCandidateObject (
            Transform parent, 
            Action<CandidateInfo> onclick, 
            CandidateItem data)
        {
            GameObject item = m_CandidateItemPool.GetPooledObject (parent);
            item.GetComponent<Image> ().enabled = false;
            Button buttonComponent = item.GetComponent<Button> ();
            buttonComponent.onClick.RemoveAllListeners ();
            buttonComponent.onClick.AddListener (delegate {
                onclick (data.Info);
            });
            item.transform.GetChild (0).GetComponent<Text> ().text = data.Info.Word;
            item.GetComponent<LayoutElement> ().minWidth = data.ItemWidth;
            return item;
        }

        private void UpdateDisplayPinyin (string pinyin)
        {
            m_InputPinyinText.text = pinyin;
        }

        private void PopulateFirstPage (Action<CandidateInfo> onclick)
        {
            UpdateDisplayPinyin (InputMethod.GetPinyinSep (0, 0));
            m_TotalPage = 0;
            m_CurrentDisplayPage = 0;
            ClearContent ();

            m_ContentRect.localPosition = new Vector3 (
                m_ContentRect.localPosition.x - m_PagerScrolledOffset,
                m_ContentRect.localPosition.y,
                m_ContentRect.localPosition.z);

            m_PagerScrolledOffset = 0;
            m_ExpandButton.enabled = true;
            m_HasMoreCandidates = true;

            PopulatePage (onclick);
            if (m_HasMoreCandidates) {
                // pre-add another page for page down action.
                PopulatePage (onclick);
            }
            UpdatePagerButtonsVisibility ();
        }

        private void UpdatePagerButtonsVisibility ()
        {
            m_PageDownButton.gameObject.SetActive (
                (m_HasMoreCandidates || m_CurrentDisplayPage < m_TotalPage) && m_IsExpanded);

            m_PageUpButton.gameObject.SetActive (m_HasPreviousPage && m_IsExpanded);
        }

        private void ClearContent ()
        {
            if (m_ContentRect.childCount > 0) {
                List<Transform> lines = new List<Transform> ();
                List<GameObject> items = new List<GameObject> ();

                for (int i = 0; i < m_ContentRect.childCount; i++) {
                    Transform line = m_ContentRect.GetChild (i);
                    for (int j = 0; j < line.childCount; j++) {
                        items.Add (line.GetChild (j).gameObject);
                    }
                    line.DetachChildren ();
                    lines.Add (line);
                }

                for (int i = 0; i < lines.Count; i++) {
                    Destroy (lines [i].gameObject);
                }

                for (int i = 0; i < items.Count; i++) {
                    m_CandidateItemPool.RecycleObject (items [i]);
                }

                m_CandidateBindingQueue.Clear ();
                m_ExpandButton.enabled = false;
                m_HasMoreCandidates = false;
                m_HasPreviousPage = false;
                if (m_RowCount > 1) {
                    FoldPanel (false);
                }
            }
        }

        private void Reset ()
        {
            ClearContent ();
            UpdateDisplayPinyin (String.Empty);
            m_TotalPage = 0;
            m_CurrentDisplayPage = 0;
            m_ContentRect.sizeDelta = new Vector2 (m_InitialRectWidth, m_FoldedHeight);
            UpdatePagerButtonsVisibility ();
        }

        private class CandidateItem
        {
            public CandidateInfo Info{ private set; get; }

            public float ItemWidth{ private set; get; }

            public CandidateItem (CandidateInfo info, float width)
            {
                Info = info;
                ItemWidth = width;
            }
        }

        private IEnumerator TranslateXBy (Transform t, float offset)
        {
            _isAnimating = true;
            if (offset > float.Epsilon || offset < -float.Epsilon) {
                Vector3 currentPosition = t.localPosition;

                float speed = offset / PAGING_DURATION;
                float delta = speed * Time.deltaTime;

                while (Mathf.Abs (delta).CompareTo (Mathf.Abs (offset)) < 0) {
                    t.localPosition = new Vector3 (currentPosition.x + delta, currentPosition.y, currentPosition.z);
                    delta += speed * Time.deltaTime;
                    yield return null;
                }
                t.localPosition = new Vector3 (currentPosition.x + offset, currentPosition.y, currentPosition.z);
            }
            _isAnimating = false;
        }
    }
}

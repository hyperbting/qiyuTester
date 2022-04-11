using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Qiyi.InputMethod.Keyboard
{
	public class TextHoverEffect : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
	{
		private Text _text;
		private readonly Color _highlightedColor = new Color (48 / 255f, 254 / 255f, 242 / 255f);
		private readonly Color _normalColor = new Color (182 / 255f, 220 / 255f, 241 / 255f);

		void Awake ()
		{
			_text = GetComponent<Text> ();
		}

		void OnDisable ()
		{
			_text.color = _normalColor;
		}

		#region IPointerExitHandler implementation

		public void OnPointerExit (PointerEventData eventData)
		{
			_text.color = _normalColor;
		}

		#endregion

		#region IPointerEnterHandler implementation

		public void OnPointerEnter (PointerEventData eventData)
		{
			_text.color = _highlightedColor;
		}

		#endregion
	}
}


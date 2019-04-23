using UnityEngine;
using UnityEngine.UI;

namespace UnityPackages.UI {
	public class UICoverImage : MonoBehaviour {
		public Sprite sourceImage;
		public Color color = Color.white;
		public bool raycastTarget;

		private Image childImage;
		private AspectRatioFitter childARF;
		private bool isInitialized;

		public void GetComponents () {
			if (this.isInitialized == false) {

				// Clear child objects in case of variable leaks.
				foreach (Transform _child in this.transform) {
					if (Application.isPlaying == false)
						GameObject.DestroyImmediate (_child.gameObject);
					else
						GameObject.Destroy (_child.gameObject);
				}

				// Create game objects and ensure their components.
				var _childGameObject = new GameObject ("image");
				var _childImage = this.EnsureComponent<Image> (_childGameObject);
				var _childRect = this.EnsureComponent<RectTransform> (_childGameObject);
				var _childARF = this.EnsureComponent<AspectRatioFitter> (_childGameObject);
				var _parentImage = this.EnsureComponent<Image> (this.gameObject);
				var _parentMask = this.EnsureComponent<Mask> (this.gameObject);

				// Set variables that wont change.
				_childGameObject.hideFlags = HideFlags.HideInHierarchy;
				_childGameObject.transform.SetParent (this.transform);
				_childGameObject.transform.localScale = Vector3.one;
				_childGameObject.transform.localPosition = Vector3.zero;
				_childImage.preserveAspect = true;
				_childARF.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
				_childARF.aspectRatio = 1;
				_parentImage.raycastTarget = false;
				_parentMask.showMaskGraphic = false;

				// Ensure no editor loop and store needed variables.
				this.childARF = _childARF;
				this.childImage = _childImage;
				this.isInitialized = true;
			}
		}

		public void Format () {
			if (this.sourceImage == null)
				return;

			this.childImage.sprite = this.sourceImage;
			this.childImage.color = this.color;
			this.childImage.raycastTarget = this.raycastTarget;
			this.childARF.aspectRatio = (float) this.sourceImage.rect.width / (float) this.sourceImage.rect.height;
		}

		private void Awake () {
			this.GetComponents ();
		}

		private void Start () {
			this.Format ();
		}

		private void OnDrawGizmos () {
			if (Application.isPlaying == false) {
				this.GetComponents ();
				this.Format ();
			}
		}

		private C EnsureComponent<C> (GameObject targetGameObject) where C : Component {
			var _get = targetGameObject.GetComponent<C> ();
			if (_get != null)
				return _get;
			var _set = targetGameObject.AddComponent (typeof (C)).GetComponent<C> ();
			_set.hideFlags = HideFlags.HideInInspector;
			return _set;
		}
	}
}
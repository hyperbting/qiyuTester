using UnityEngine.XR;

namespace Unity.XR.Qiyu
{
    /// <summary>
    /// Input Usages, consumed by the UnityEngine.XR.InputDevice class in order to retrieve inputs.
    /// These usages are all Qiyu specific.
    /// </summary>
    public static class QiyuUsages
    {
        /// <summary>
        /// Represents the girp touch on Qiyu controllers.
        /// </summary>
        // public static InputFeatureUsage<bool> gripTouch = new InputFeatureUsage<bool>("Thumbrest");
        /// <summary>
        /// Represents the trigger touchof the Qiyu Controller.
        /// </summary>
        public static InputFeatureUsage<bool> triggerTouch = new InputFeatureUsage<bool>("IndexTouch");
        /// <summary>
        /// Represents the menu touch or home touch Qiyu Controller.
        /// </summary>
        // public static InputFeatureUsage<bool> menuTouch = new InputFeatureUsage<bool>("ThumbTouch");
    }
}

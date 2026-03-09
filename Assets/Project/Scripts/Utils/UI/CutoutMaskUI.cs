using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace VertigoSpin.Project.Scripts.Utils.UI
{
    /// <summary>
    /// A custom UI component that modifies the <see cref="Material"/> used for rendering to apply a stencil mask.
    /// </summary>
    public sealed class CutoutMaskUI : Image
    {
        public override Material materialForRendering
        {
            get
            {
                Material rendering = new(base.materialForRendering);
                rendering.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return rendering;
            }
        }
    }
}

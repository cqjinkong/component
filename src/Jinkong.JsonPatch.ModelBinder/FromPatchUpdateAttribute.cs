using Microsoft.AspNetCore.Mvc;

namespace Jinkong.JsonPatch.ModelBinder
{
    public class FromPatchUpdateAttribute : ModelBinderAttribute
    {
        public FromPatchUpdateAttribute() : base(typeof(PatchUpdateBinder))
        {
        }
    }
}